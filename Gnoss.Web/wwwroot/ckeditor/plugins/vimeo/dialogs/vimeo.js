function getParmVimeo(url) {
    var matches;
    if (matches = url.match(/vimeo.com\/(\d+)/)) {
        return matches[1];
    } else {
        return ("");
    }
}

(function () {
	var vimeoDialog = function (editor, dialogType) { 
		return {
			title: editor.lang.vimeo.title, 
			minWidth: CKEDITOR.env.ie && CKEDITOR.env.quirks ? 368 : 350, 
			minHeight: 240, 
			onShow: function () { 
				this.getContentElement('general', 'content').getInputElement().setValue('') 
			},
            onCancel: function () {
                $('#infoFooterVimeo').hide();
            },
            onOk: function () {
                $('#infoFooterVimeo').hide();
                var inputCode = this.getContentElement('general', 'content').getInputElement().getValue();
                if (inputCode.length < 1 || inputCode.indexOf('http') < 0 || getParmVimeo(inputCode).length < 1) {
                    $('#infoFooterVimeo').show();
                    return false;
                }
                var text = '<iframe src="https://player.vimeo.com/video/' + getParmVimeo(inputCode) + '?title=0&amp;byline=0&amp;portrait=0" width="560" height="309" frameborder="0" allowfullscreen="true"></iframe>';
                this.getParentEditor().insertHtml(text)
            }, 
			contents: 
			[
				{ 
					label: editor.lang.common.generalTab, 
					id: 'general', 
					elements: 
					[
						{ 
							type: 'html', 
							id: 'pasteMsg', 
							html: '<div style="white-space:normal;width:500px;"><img style="margin:5px auto;" src="' + CKEDITOR.getUrl(CKEDITOR.plugins.getPath('vimeo') + 'images/vimeo_large.png') + '"><br />' + editor.lang.vimeo.pasteMsg + '</div>' 
						}, { 
							type: 'html', 
							id: 'content', 
							style: 'width:340px;height:90px', 
							html: '<input size="25" style="' + 'border:1px solid black;' + 'background:white"><div id="infoFooterVimeo" style="display: none;"><br /><span style="color:red;">' + editor.lang.vimeo.infoFooter + '</span></div>', 
							focus: function () { this.getElement().focus() } 
						}
					]
				}
			]
		};
	};
	
    CKEDITOR.dialog.add('vimeo', function (editor) {
        return vimeoDialog(editor, 'vimeo');
    })
})();