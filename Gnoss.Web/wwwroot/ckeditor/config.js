/**
 * @license Copyright (c) 2003-2013, CKSource - Frederico Knabben. All rights reserved.
 * For licensing, see LICENSE.html or http://ckeditor.com/license
 */

CKEDITOR.editorConfig = function( config ) {
	// Define changes to default configuration here. For example:
    //config.language = $('#inpt_Idioma').val();
	// config.uiColor = '#AADC6E';
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
	['TextColor', 'BGColor'],
	['Image', 'Smiley']
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
	['Image', 'Vimeo', 'Youtube', 'Smiley', 'SpecialChar']
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
	['Image', 'Vimeo', 'Youtube']//, 'Htmlinsert']
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

/* Aplicar CKEditorSimple */
CKEDITOR.on('instanceReady', function(event) {
	// Id automático (autonumérico) que genera la instancia por si hay más elementos ckEditor
	const ckEditorId = event.editor.id;
	// La instancia del ckEditor, en concreto todo el contenedor que se está cargando
	const $ckEditorInstance = $(event.editor.container.$);

	// Busca si input que dispara el CKEditor tiene la clase ckeSimple. Si la tiene, se desea que se aplique este comportamiento
	const ckEditorParent = $ckEditorInstance.siblings('.ckeSimple');	

	// Aplicar la operativa de CKEditorSimple si existe la clase ckeSimple
	if (ckEditorParent.length > 0){
		// La barra de herramientas del ckEditor
		const $ckEditorToolbar = $ckEditorInstance.find(`#${ckEditorId}_top`);
		// Contenedor donde irá el texto del ckEditor
		const $ckEditorContent = $ckEditorInstance.find(`#${ckEditorId}_contents`);

		// Longitud del posible texto que haya en el editor
		const ckeEditorContentLength= event.editor.document.getBody().getText().length;
		
		const options = {
			$ckEditor: event.editor,							// El propio ckEditor
			$ckEditorInstance: $ckEditorInstance, 				// La instancia jquery del CKEditor creado
			$ckEditorToolbar: $ckEditorToolbar,   				// Toolbar del CKEditor
			$ckEditorContent: $ckEditorContent,	  				// Content del CKEditor
			ckeEditorContentLength: ckeEditorContentLength, 	// Longitud de textos que hay en el editor CKEditor
		}
		$ckEditorInstance.ckEditorSimple(options); 
	}	
});

