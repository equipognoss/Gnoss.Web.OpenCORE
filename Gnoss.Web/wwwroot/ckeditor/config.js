/**
 * @license Copyright (c) 2003-2013, CKSource - Frederico Knabben. All rights reserved.
 * For licensing, see LICENSE.html or http://ckeditor.com/license
 */

CKEDITOR.editorConfig = function( config ) {
	// Define changes to default configuration here. For example:
    //config.language = $('#inpt_Idioma').val();
	// config.uiColor = '#AADC6E';

	// Evitar saltos de línea en ckEditor
	config.autoParagraph = false;

	// Configuración de visualizador de código fuente HTML para CKEditor
	setupCodeMirrorForHtmlViewer(config);
	loadAdditionalCSS(config);
};

/**
 * Método para cargar los ficheros CSS de la comunidad con el objetivo de ver la previsualización correcta del HTML en vista preliminar.
 * @param {*} config 
 */
function loadAdditionalCSS(config){

	// CSS de comunidad para logar una previsualización de los componentes HTML personalizados
	const cssFilesArray = $(".styleForCkEditorPreview");
		
	if (cssFilesArray.length > 0){
		const cssFilesValue = $.map(cssFilesArray, function(item) {
			const link = $(item).prop("href");
			return link;
		});
		config.contentsCss = cssFilesValue;
	} 
}


/**
 * Método para configurar el visualizador de código HTML, CSS y JS al crear componentes HTML y seleccionar la vista de "Source"
 * @param {*} config 
 */
function setupCodeMirrorForHtmlViewer(config){

	// Permitir inputs vacíos en Html
	for(let tag in CKEDITOR.dtd.$removeEmpty){
		CKEDITOR.dtd.$removeEmpty[tag] = false;
	}

	// Permitir cualquier contenido HTML a insertar
	config.allowedContent = true;

	// Evitar párrafos automáticamente
	config.autoParagraph = false;
	// Autocompletar al pegar código HTML
	config.autoIndent_onPaste = true;
	//Configuración de indentación 
	config.codeSnippet_theme = 'monokai_sublime';
	config.codeSnippet_indentation = '2';
	// Resaltador de código HTML en Vista código (Codemirror)
	config.extraPlugins = 'codemirror';
		
	// Configuración del resaltador HTML
	config.codemirror = {
		// Whether or not you want Brackets to automatically close themselves
		autoCloseBrackets: true,
		 // Whether or not you want tags to automatically close themselves
		autoCloseTags: true,
		 // Whether or not to automatically format code should be done when the editor is loaded
		autoFormatOnStart: true, 
		// Whether or not to automatically format code which has just been uncommented
		autoFormatOnUncomment: true,
		// Whether or not to continue a comment when you press Enter inside a comment block
		continueComments: true,
		 // Whether or not you wish to enable code folding (requires 'lineNumbers' to be set to 'true')
		enableCodeFolding: true,
		// Whether or not to enable code formatting
		enableCodeFormatting: true,
		// Whether or not to enable search tools, CTRL+F (Find), CTRL+SHIFT+F (Replace), CTRL+SHIFT+R (Replace All), CTRL+G (Find Next), CTRL+SHIFT+G (Find Previous)
		enableSearchTools: true,
		// Whether or not to highlight all matches of current word/selection
		highlightMatches: true,
		 // Whether, when indenting, the first N*tabSize spaces should be replaced by N tabs
		indentWithTabs: false,
		 // Whether or not you want to show line numbers
		lineNumbers: true,
		// Whether or not you want to use line wrapping
		lineWrapping: true,
		 // Define the language specific mode 'htmlmixed' for html  including (css, xml, javascript), 'application/x-httpd-php' for php mode including html, or 'text/javascript' for using java script only 
		mode: 'htmlmixed',
		// Whether or not you want to highlight matching braces
		matchBrackets: true,
		// Whether or not you want to highlight matching tags
		matchTags: true,
		// Whether or not to show the showAutoCompleteButton   button on the toolbar
		showAutoCompleteButton: false,
		 // Whether or not to show the comment button on the toolbar
		showCommentButton: false,
		// Whether or not to show the format button on the toolbar
		showFormatButton: false,
		 // Whether or not to show the search Code button on the toolbar
		showSearchButton: true,
		 // Whether or not to show Trailing Spaces
		showTrailingSpace: true,
		// Whether or not to show the uncomment button on the toolbar
		showUncommentButton: false,
		 // Whether or not to highlight the currently active line
		styleActiveLine: true,
		 // Set this to the theme you wish to use (codemirror themes)
		theme: 'default',
		// "Whether or not to use Beautify for auto formatting On start
		useBeautifyOnStart: false
	};	

	// Lenguajes a utilizar en CodeMirror
	config.codeSnippet_languages = {
		html: 'HTML',
		css: 'CSS',
		js: 'JavaScript'
	};	
}


CKEDITOR.config["toolbar_Gnoss-Mensajes"] = [
	// Quitar Pegar por funcionamiento en ckEditor
	// ['Cut', 'Copy', 'PasteText', 'PasteFromWord'],
	['Undo', 'Redo', '-', 'RemoveFormat'],
	['Bold', 'Italic', 'Underline'],
	['Table', '-', 'NumberedList', 'BulletedList', '-', 'Outdent', 'Indent'],
	['Maximize'],
	['JustifyLeft', 'JustifyCenter', 'JustifyRight', 'JustifyBlock'],
	['Link', 'Unlink'],
//	['FontSize'],
	['TextColor', 'BGColor'],
	['Image', 'Smiley'],
	'/',
];

CKEDITOR.config["toolbar_Gnoss-Blogs"] = [
	// Quitar Pegar por funcionamiento en ckEditor
	// ['Cut', 'Copy', 'PasteText', 'PasteFromWord'],
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
	// Quitar Pegar por funcionamiento en ckEditor
	// ['Cut', 'Copy', 'PasteText', 'PasteFromWord'],	
	['Maximize','Source','ShowBlocks'],
	['Undo', 'Redo', '-', 'RemoveFormat'],
	['Bold', 'Italic', 'Underline', 'Strike'],
	['Table', '-', 'NumberedList', 'BulletedList', '-', 'Outdent', 'Indent'],
	['Maximize'],
	['JustifyLeft', 'JustifyCenter', 'JustifyRight', 'JustifyBlock'],
	['Link', 'Unlink', 'Anchor'],
//	['FontSize'],
	['Format'],
	['TextColor', 'BGColor'],
	['Image', 'Vimeo', 'Youtube']//, 'Htmlinsert'
];

CKEDITOR.config["toolbar_Gnoss-Bios"] = [
	// Quitar Pegar por funcionamiento en ckEditor
	// ['Cut', 'Copy', 'PasteText', 'PasteFromWord'],
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
	// Quitar Pegar por funcionamiento en ckEditor
	// ['Cut', 'Copy', 'PasteText', 'PasteFromWord'],
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

	// Evitar errores de copiar/pegar por incompatibilidad del navegador
	event.editor.on("beforeCommandExec", function (event) {
		// Show the paste dialog for the paste buttons and right-click paste
		if (event.data.name == "paste") {
			event.editor._.forcePasteDialog = false;
		}
		// No mostrar el cuadro de dialogo de Ctrl+Shift+V
		if (event.data.name == "pastetext" && event.data.commandData.from == "keystrokeHandler") {
			event.cancel();
		}
	});

	// Eliminar la opción de pegar del ckeditor	
	if (event.editor.contextMenu) {
		event.editor.removeMenuItem('paste');
	}

	// Input para controlar si se permite el uso de JS dentro del ckEditor
	const inputElement = $(event.editor.element.$);
	if (!inputElement.data("allow-js")){
		event.editor.on('change', validateContent);
		event.editor.on('paste', validateContent);
	} 		

	/**
	 * Método para evitar que se pueda introducir código Javascript en editor CKEditor
	 */	
    function validateContent() {
		var content = event.editor.getData(); // Obtener contenido del editor
		// Realizar validación, por ejemplo, buscando código JavaScript no deseado
		if (content.match(/<script[\s\S]*?>[\s\S]*?<\/script>/gi)) {
		  // Se encontró código JavaScript no deseado
		  //alert('Por seguridad, el uso de Javascript no está permitido en el editor.');
		  mostrarNotificacion("info", 'Por seguridad, el uso de Javascript no está permitido en el editor');
		  // Limpiar contenido del editor		  
		  setTimeout(function(){
			event.editor.setData('');
		  },1000)
		}
	}	
});

