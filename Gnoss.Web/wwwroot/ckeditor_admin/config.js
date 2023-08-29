/**
 * @license Copyright (c) 2003-2023, CKSource Holding sp. z o.o. All rights reserved.
 * For licensing, see https://ckeditor.com/legal/ckeditor-oss-license
 */

CKEDITOR.editorConfig = function( config ) {
	// Define changes to default configuration here. For example:
	// config.language = 'fr';
	// config.uiColor = '#AADC6E';

	// Añadir Plugin image2
	config.extraPlugins = 'image2';	

	setupAutoGrow(config);
	// Configuración de visualizador de código fuente HTML para CKEditor
	setupCodeMirrorForHtmlViewer(config);
	loadAdditionalCSS(config);	
	loadCustomCSS(config);		
	setupCkEditorToolBars();

};


/**
 * Método para configurar las difierentes toolbars que se utilizarán en la plataforma
 */

function setupCkEditorToolBars(){

	CKEDITOR.config["toolbar_Gnoss-Base"] = [
		['Maximize','JustifyLeft','JustifyCenter','JustifyRight','JustifyBlock', 'NumberedList','BulletedList','Table','Link', 'Unlink', 'Anchor'],
		'/',
		['Bold','Italic','Underline','Strike','Subscript','Superscript'],
		['Styles','Format','Font','FontSize','TextColor','BGColor'],			
	];

	CKEDITOR.config["toolbar_Gnoss-Mensajes"] = [
		['Maximize','JustifyLeft','JustifyCenter','JustifyRight','JustifyBlock', 'NumberedList','BulletedList','Image','Table', 'Link', 'Unlink', 'Anchor'],
		'/',
		['Bold','Italic','Underline','Strike','Subscript','Superscript'],
		['Styles','Format','Font','FontSize','TextColor','BGColor'],			
	];

	CKEDITOR.config["toolbar_Gnoss-Blogs"] = [
		['Maximize','JustifyLeft','JustifyCenter','JustifyRight','JustifyBlock', 'NumberedList','BulletedList','Table', 'Link', 'Unlink', 'Anchor'],
		'/',
		['Bold','Italic','Underline','Strike','Subscript','Superscript'],
		['Styles','Format','Font','FontSize','TextColor','BGColor'],			
	];

	CKEDITOR.config["toolbar_Gnoss-Recursos"] = [
		['Maximize','JustifyLeft','JustifyCenter','JustifyRight','JustifyBlock', 'NumberedList','BulletedList','Image','Table', 'Link', 'Unlink', 'Anchor'],
		'/',
		['Bold','Italic','Underline','Strike','Subscript','Superscript'],
		['Styles','Format','Font','FontSize','TextColor','BGColor'],			
	];	

	CKEDITOR.config["toolbar_Gnoss-Comentario"] = [
		['Link', 'Unlink'],	
	];		

	CKEDITOR.config["toolbar_Gnoss-Html"] = [					
		['Maximize','Source','ShowBlocks'],
		['JustifyLeft','JustifyCenter','JustifyRight','JustifyBlock', 'NumberedList','BulletedList', 'Image','Table'],
		'/',
		['Bold','Italic','Underline','Strike','Subscript','Superscript'],
		['Styles','Format','Font','FontSize','TextColor','BGColor'],								
	];
}


/**
 * Método para configurar el crecimiento automático del ckEditor en base al código que este contenga
 * @param {*} config 
 */
function setupAutoGrow(config){
	config.extraPlugins = 'autogrow';
	config.autoGrow_minHeight = 200;
	config.autoGrow_maxHeight = 600;
	config.autoGrow_bottomSpace = 50;
}

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
 * Método para cargar las hojas de estilo customizadas por el usuario dentro del ckEditor con el objetivo de poder ver
 * fielmente cómo queda el componente CMS en vivo con los estilos a usar en el front de la comunidad
 */
function loadCustomCSS(config){

	// Método para cargar la capa de personalización que se encuentra en la vista _HojaDeEstilosPersonalizado.cshtml
	const urlGetHojaDeEstilosPersonalizado = `${$("#inpt_baseUrlBusqueda").val()}/custom-stylesheet/get-custom-css`;

	const ckEditorCustomStyleSheet = CkEditorCustomStyleSheets.getInstance();

	if (ckEditorCustomStyleSheet.isDownloadingCustomStyleSheet == false ){
		ckEditorCustomStyleSheet.getCustomStyleSheetsFromBackoffice(urlGetHojaDeEstilosPersonalizado, config, function(downloaded){
			if (ckEditorCustomStyleSheet.customStyleSheetList.length > 0){
				config.contentsCss = ckEditorCustomStyleSheet.customStyleSheetList;
				// Recargar los ckEditor con los nuevos estilos sólo si se ha procedido a la descarga
				if (downloaded){				
					setTimeout(function(){															
						loadingOcultar();
						ckEditorRecargarTodos();										
					},1000);
				}else{
					loadingOcultar();
				}
			}
		});
	}

	// Si se ha descargado ya, aplicar los estilos correspondientes 
	if (ckEditorCustomStyleSheet.isDownloadedCustomStyleSheet && ckEditorCustomStyleSheet.customStyleSheetList.length > 0){
		config.contentsCss = ckEditorCustomStyleSheet.customStyleSheetList;
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

/**
 * Método para configurar los botones del Toolbar de CKEditor
 * @param {*} config 
 */
function setupToolbarButtons(config){
	// Configuración de botones del Toolbar
	config.toolbarGroups = [
		{ name: 'forms', groups: [ 'forms' ] },
		{ name: 'tools', groups: [ 'tools' ] },
		{ name: 'insert', groups: [ 'insert' ] },
		{ name: 'links', groups: [ 'links' ] },
		{ name: 'document', groups: [ 'mode', 'document', 'doctools' ] },
		{ name: 'clipboard', groups: [ 'clipboard', 'undo' ] },
		{ name: 'editing', groups: [ 'find', 'selection', 'spellchecker', 'editing' ] },
		'/',
		{ name: 'basicstyles', groups: [ 'basicstyles', 'cleanup' ] },
		{ name: 'paragraph', groups: [ 'list', 'indent', 'blocks', 'align', 'bidi', 'paragraph' ] },
		'/',
		{ name: 'styles', groups: [ 'styles' ] },
		{ name: 'colors', groups: [ 'colors' ] },
		{ name: 'others', groups: [ 'others' ] },
		{ name: 'about', groups: [ 'about' ] }
	];

	config.removeButtons = 'Save,NewPage,Preview,Templates,Cut,Undo,Copy,Redo,Paste,PasteText,PasteFromWord,Find,Replace,SelectAll,Scayt,ImageButton,CopyFormatting,RemoveFormat,Smiley,PageBreak,About,Print,ExportPdf,Anchor,Blockquote,BidiLtr,BidiRtl,Language,Styles,Form,Checkbox,Radio,TextField,Textarea,Select,Button,HiddenField,SpecialChar,Iframe,CreateDiv';	
}

/**
 * Método para configurar los botones del Toolbar de CKEditor sin HTML
 * @param {*} config 
 */
function setupToolbarNormalButtons(config){
	config.toolBarBasic = [
		{ name: 'document', groups: [ 'mode', 'document', 'doctools' ] },
		{ name: 'clipboard', groups: [ 'clipboard', 'undo' ] },
		{ name: 'editing', groups: [ 'find', 'selection', 'spellchecker', 'editing' ] },
		{ name: 'forms', groups: [ 'forms' ] },
		'/',
		{ name: 'basicstyles', groups: [ 'basicstyles', 'cleanup' ] },
		{ name: 'paragraph', groups: [ 'list', 'indent', 'blocks', 'align', 'bidi', 'paragraph' ] },
		{ name: 'links', groups: [ 'links' ] },
		{ name: 'insert', groups: [ 'insert' ] },
		'/',
		{ name: 'styles', groups: [ 'styles' ] },
		{ name: 'colors', groups: [ 'colors' ] },
		{ name: 'tools', groups: [ 'tools' ] },
		{ name: 'others', groups: [ 'others' ] },
		{ name: 'about', groups: [ 'about' ] }
	];

	config.removeButtons = 'Source,Save,NewPage,ExportPdf,Preview,Print,Templates,Cut,Copy,Paste,PasteText,PasteFromWord,Undo,Redo,Find,Replace,SelectAll,Scayt,Form,Checkbox,Radio,TextField,Textarea,Select,Button,ImageButton,HiddenField,CopyFormatting,RemoveFormat,Outdent,Indent,Blockquote,CreateDiv,BidiLtr,BidiRtl,Language,Link,Anchor,Unlink,SpecialChar,PageBreak,Iframe,button2,button1,button3,divs,ShowBlocks,About,Maximize';	
}


// Indentación para elementos HTML
CKEDITOR.on( 'instanceReady', function( ev ) {

	// Obtener el contendor de la instancia para mostrar el loading donde corresponda si se están descargando CSS customizados
	const editor = ev.editor;
	// Obtén el contenedor DOM
    const ckEditorContainer = editor.container.$; 
	// Obtener instancia que gestiona la descarga del ckEditorCustomStyleSheet
	const ckEditorCustomStyleSheet = CkEditorCustomStyleSheets.getInstance();
	ckEditorCustomStyleSheet.parentContainer = $(ckEditorContainer);
	
	
	if (ckEditorCustomStyleSheet.isDownloadingCustomStyleSheet == true){
		loadingMostrar(ckEditorCustomStyleSheet.parentContainer);
	}else{
		console.log("Ocultar");
	}
		

	const arrayHtmlElements = ['div','p','a','img','ul','ol','li','header','nav','section','article','aside','footer','h1','h2','h3','h4','h5','h6','ul','li','select','option','h1','h2','h3','h4','h5','h6','span','input','label','article','button','figure','figcaption'];
	arrayHtmlElements.forEach(element => ev.editor.dataProcessor.writer.setRules( element, {
		indent: true,
		breakBeforeOpen: true,
		breakAfterOpen: false,
		breakBeforeClose: false,
		breakAfterClose: true
	}));


	// Eliminar el botón de "About" del plugin "CodeMirror"
	$('.cke_button__codemirrorabout').addClass("d-none");
});
