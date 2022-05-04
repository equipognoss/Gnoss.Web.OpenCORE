/**
 * @license Copyright (c) 2003-2013, CKSource - Frederico Knabben. All rights reserved.
 * For licensing, see LICENSE.html or http://ckeditor.com/license
 */

CKEDITOR.editorConfig = function( config ) {
	// Define changes to default configuration here. For example:
    //config.language = $('#inpt_Idioma').val();
	// config.uiColor = '#AADC6E';
	config.allowedContent = true; // PAra permitir JS en el editor de html.
};

CKEDITOR.config["toolbar_Gnoss-Mensajes"] = [
	['Cut', 'Copy', 'PasteText', 'PasteFromWord'],
	['Undo', 'Redo', '-', 'RemoveFormat'],
	['Bold', 'Italic', 'Underline'],
	['Table', '-', 'NumberedList', 'BulletedList', '-', 'Outdent', 'Indent'],
	['Maximize'],
	['JustifyLeft', 'JustifyCenter', 'JustifyRight', 'JustifyBlock'],
	['Link', 'Unlink'],
//	['FontSize'],
	['TextColor', 'BGColor']
];

CKEDITOR.config["toolbar_Gnoss-Blogs"] = [
	['Cut', 'Copy', 'PasteText', 'PasteFromWord'],
	['Undo', 'Redo', '-', 'RemoveFormat'],
	['Bold', 'Italic', 'Underline'],
	['Table', '-', 'NumberedList', 'BulletedList', '-', 'Outdent', 'Indent'],
	['Maximize'],
	['JustifyLeft', 'JustifyCenter', 'JustifyRight', 'JustifyBlock'],
	['Link', 'Unlink', 'Anchor'],
//	['FontSize'],
	['TextColor', 'BGColor'],
	['Imagesimple', 'Vimeo', 'Youtube', 'Smiley', 'SpecialChar']
];

CKEDITOR.config["toolbar_Gnoss-Recursos"] = [
	['Cut', 'Copy', 'PasteText', 'PasteFromWord'],
	['Undo', 'Redo', '-', 'RemoveFormat'],
	['Bold', 'Italic', 'Underline', 'Strike'],
	['Table', '-', 'NumberedList', 'BulletedList', '-', 'Outdent', 'Indent'],
	['Maximize'],
	['JustifyLeft', 'JustifyCenter', 'JustifyRight', 'JustifyBlock'],
	['Link', 'Unlink', 'Anchor'],
//	['FontSize'],
	['Format'],
	['TextColor', 'BGColor'],
	['Image','Vimeo', 'Youtube'],//,'Source', 'Htmlinsert'],
    //['Imagesimple'],
	['CodeSnippet']
];

CKEDITOR.config["toolbar_Gnoss-Bios"] = [
	['Cut', 'Copy', 'PasteText', 'PasteFromWord'],
	['Undo', 'Redo', '-', 'RemoveFormat'],
	['Bold', 'Italic', 'Underline'],
	['Table', '-', 'NumberedList', 'BulletedList', '-', 'Outdent', 'Indent'],
	['Maximize'],
	['JustifyLeft', 'JustifyCenter', 'JustifyRight', 'JustifyBlock'],
	['Link', 'Unlink'],
//	['FontSize'],
	['TextColor', 'BGColor']
];

CKEDITOR.config["toolbar_Gnoss-Dafo"] = [
	['Cut', 'Copy', 'PasteText', 'PasteFromWord'],
	['Undo', 'Redo', '-', 'RemoveFormat'],
	['Bold', 'Italic', 'Underline'],
	['Table', '-', 'NumberedList', 'BulletedList', '-', 'Outdent', 'Indent'],
	['Maximize'],
	['JustifyLeft', 'JustifyCenter', 'JustifyRight', 'JustifyBlock'],
	['Link', 'Unlink'],
//	['FontSize'],
	['TextColor', 'BGColor']
];

CKEDITOR.config["toolbar_Gnoss-Comentario"] = [
	['Link', 'Unlink'],
];

