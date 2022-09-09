function getParmYoutube(url) {
    var matches;
    var re = new RegExp(/(?:youtube\.com\/(?:[^\/]+\/.+\/|(?:v|e(?:mbed)?)\/|.*[?&]v=)|youtu\.be\/)([^"&?\/ ]{11})/i);
    matches = url.match(re);
    if (matches) {
        return (matches[1]);
    } else {
        return ("");
    }
}

(function () {
	var youtubeDialog = function (editor, dialogType) { 
		return {
			title: editor.lang.youtube.title, 
			minWidth: CKEDITOR.env.ie && CKEDITOR.env.quirks ? 368 : 350, 
			minHeight: 240, 
			onShow: function () { 
				this.getContentElement('general', 'content').getInputElement().setValue('') 
			},
			onCancel: function () {
				$('#infoFooterYoutube').hide();
			},
			onOk: function () {
				$('#infoFooterYoutube').hide();
				var inputCode = this.getContentElement('general', 'content').getInputElement().getValue();
				if (inputCode.length < 1 || inputCode.indexOf('http') < 0 || getParmYoutube(inputCode).length < 1) {
					$('#infoFooterYoutube').show();
					return false;
				}
				if (document.cookie.match("redes sociales=no")) {
					var text = '<iframe title="YouTube video player" class="youtube-player" type="text/html" width="480" height="390" src="https://www.youtube-nocookie.com/embed/' + getParmYoutube(inputCode) + '?rel=0" frameborder="0"></iframe>'; this.getParentEditor().insertHtml(text)
				}
				else {
					var text = '<iframe title="YouTube video player" class="youtube-player" type="text/html" width="480" height="390" src="https://www.youtube.com/embed/' + getParmYoutube(inputCode) + '?rel=0" frameborder="0"></iframe>'; this.getParentEditor().insertHtml(text)
				}
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
							html: '<div style="white-space:normal;width:500px;"><img style="margin:5px auto;" src="' + CKEDITOR.getUrl(CKEDITOR.plugins.getPath('youtube') + 'images/youtube_large.png') + '"><br />' + editor.lang.youtube.pasteMsg + '</div>' 
						}, { 
							type: 'html', 
							id: 'content', 
							style: 'width:340px;height:90px', 
							html: '<input size="25" style="' + 'border:1px solid black;' + 'background:white"><div id="infoFooterYoutube" style="display: none;"><br /><span style="color:red;">' + editor.lang.youtube.infoFooter + '</span></div>', 
							focus: function () { this.getElement().focus() } 
						}
					]
				}
			]
		};
	};
	
    CKEDITOR.dialog.add('youtube', function (editor) {
        return youtubeDialog(editor, 'youtube');
    })
})();