/*
Copyright (c) 2003-2011, CKSource - Frederico Knabben. All rights reserved.
For licensing, see LICENSE.html or http://ckeditor.com/license
*/

(function () {
    var imageDialog = function (editor, dialogType) {
        // Load image preview.
        var IMAGE = 1,
			LINK = 2,
			PREVIEW = 4,
			CLEANUP = 8,
			regexGetSize = /^\s*(\d+)((px)|\%)?\s*$/i,
			regexGetSizeOrEmpty = /(^\s*(\d+)((px)|\%)?\s*$)|^$/i,
			pxLengthRegex = /^\d+px$/;

        var onSizeChange = function () {
            var value = this.getValue(), // This = input element.
				dialog = this.getDialog(),
				aMatch = value.match(regexGetSize); // Check value
            if (aMatch) {
                if (aMatch[2] == '%')			// % is allowed - > unlock ratio.
                    switchLockRatio(dialog, false); // Unlock.
                value = aMatch[1];
            }

            // Only if ratio is locked
            if (dialog.lockRatio) {
                var oImageOriginal = dialog.originalElement;
                if (oImageOriginal.getCustomData('isReady') == 'true') {
                    if (this.id == 'txtHeight') {
                        if (value && value != '0')
                            value = Math.round(oImageOriginal.$.width * (value / oImageOriginal.$.height));
                        if (!isNaN(value))
                            dialog.setValueOf('info', 'txtWidth', value);
                    }
                    else		//this.id = txtWidth.
                    {
                        if (value && value != '0')
                            value = Math.round(oImageOriginal.$.height * (value / oImageOriginal.$.width));
                        if (!isNaN(value))
                            dialog.setValueOf('info', 'txtHeight', value);
                    }
                }
            }
            updatePreview(dialog);
        };

        var updatePreview = function (dialog) {
            //Don't load before onShow.
            if (!dialog.originalElement || !dialog.preview)
                return 1;

            // Read attributes and update imagePreview;
            dialog.commitContent(PREVIEW, dialog.preview);
            return 0;
        };

        // Custom commit dialog logic, where we're intended to give inline style
        // field (txtdlgGenStyle) higher priority to avoid overwriting styles contribute
        // by other fields.
        function commitContent() {
            var args = arguments;
            var inlineStyleField = this.getContentElement('advanced', 'txtdlgGenStyle');
            inlineStyleField && inlineStyleField.commit.apply(inlineStyleField, args);

            this.foreach(function (widget) {
                if (widget.commit && widget.id != 'txtdlgGenStyle')
                    widget.commit.apply(widget, args);
            });
        }

        var resetSize = function (dialog) {
            var oImageOriginal = dialog.originalElement;
            if (oImageOriginal.getCustomData('isReady') == 'true') {
                var widthField = dialog.getContentElement('info', 'txtWidth'),
					heightField = dialog.getContentElement('info', 'txtHeight');
                widthField && widthField.setValue(oImageOriginal.$.width);
                heightField && heightField.setValue(oImageOriginal.$.height);
            }
            updatePreview(dialog);
        };

        var setupDimension = function (type, element) {
            if (type != IMAGE)
                return;

            function checkDimension(size, defaultValue) {
                var aMatch = size.match(regexGetSize);
                if (aMatch) {
                    if (aMatch[2] == '%')				// % is allowed.
                    {
                        aMatch[1] += '%';
                        switchLockRatio(dialog, false); // Unlock ratio
                    }
                    return aMatch[1];
                }
                return defaultValue;
            }

            var dialog = this.getDialog(),
				value = '',
				dimension = this.id == 'txtWidth' ? 'width' : 'height',
				size = element.getAttribute(dimension);

            if (size)
                value = checkDimension(size, value);
            value = checkDimension(element.getStyle(dimension), value);

            this.setValue(value);
        };

        return {
            //title: editor.lang.image[dialogType == 'htmlinsert' ? 'title' : 'titleButton'],
            title: form.htmlincrustado,
            minWidth: 420,
            minHeight: 360,
            onOk: function () {
			
				var html = editor.document.createElement('div');
				html.$.className = 'ckeHtmlIncrustado';
				this.commitContent(html);
				editor.insertElement(html);
            },
            onLoad: function () {

                if (this.getContentElement('info', 'ratioLock')) {
                    this.addFocusable(doc.getById(btnResetSizeId), 5);
                    this.addFocusable(doc.getById(btnLockSizesId), 5);
                }

                this.commitContent = commitContent;
				
				var textarea = ElemVisible('.cke_dialog_contents textarea');
				textarea.rows = 20;
				textarea.cols = 90;
            },
            onHide: function () {
                if (this.preview)
                    this.commitContent(CLEANUP, this.preview);

                if (this.originalElement) {
                    this.originalElement.removeListener('load', onImgLoadEvent);
                    this.originalElement.removeListener('error', onImgLoadErrorEvent);
                    this.originalElement.removeListener('abort', onImgLoadErrorEvent);
                    this.originalElement.remove();
                    this.originalElement = false; 	// Dialog is closed.
                }

                delete this.imageElement;
            },
            contents: [
				{
				    id: 'info',
				    label: editor.lang.image.infoTab,
				    accessKey: 'I',
				    elements:
					[
						{
						    id: 'txtHtmlIncrus',
						    type: 'textarea',
						    label: form.html,
							required: true,
							 commit: function (element) {
								//element.data('cke-saved-src', this.getValue());
								element.setHtml(this.getValue());
							}
						}
					]
				}
			]
        };
    };

    CKEDITOR.dialog.add('htmlinsert', function (editor) {
        return imageDialog(editor, 'htmlinsert');
    });

})();
