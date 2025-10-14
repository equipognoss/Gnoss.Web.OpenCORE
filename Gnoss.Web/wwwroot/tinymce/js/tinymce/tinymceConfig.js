/**
 * Operativa para la gestión y edición de TinyCME para su versión V6. Es posible pasar una configuración o toolbar diferente
 * operativaTinyMceConfig.init({ toolbar: ["bold", "italic", "link"] }); // Personaliza el toolbar 
 */
const operativaTinyMceConfig = {

    /**
     * Inicializar operativa
     */
    init: function (pParams) {
        this.pParams = pParams;
        this.config(pParams);
        this.configEvents();
        this.configRutas();
        this.triggerEvents();     
    },

    /*
     * Inicializar elementos de la vista
     * */
    config: function (pParams) {     
        this.Options = {};
        // Flag que indica que se han descargado las hojas de estilo personalizadas
        this.customCssForTinyMCEditorPreviewDownload = false;
        // Idioma por defecto del Editor
        this.tinyMCEditorLanguage = "es";
        this.tinyLoadedClassName = "tinyMCE-loaded";
        this.tinyLoadedFinishedClassName = "tinyMCE-loaded-finished";
        // Keys para guardar en caché los CSS y el Timestamp
        this.CUSTOM_CSS_FOR_TINY_MCE_KEY = "CUSTOM_CSS_FOR_TINY_MCE";
        this.CUSTOM_CSS_FOR_TINY_MCE_TIMESTAMP_KEY = "CUSTOM_CSS_FOR_TINY_MCE_TIMESTAMP";
        // Tiempo que se cacheará los ficheros CSS - 7 días en milisegundos
        this.CUSTOM_CSS_CACHE_EXPIRATION_TIME = 7 * 24 * 60 * 60 * 1000;        
        // Dominio para guardado de CSS asignado
        this.currentDomain = $("#inpt_baseUrlBusqueda").val();
        this.widthToDisplayToolbarsInRows = 650;
        this.useSingleLineToolbars = false;
        // Listado de ficheros CSS personalizados para logar una vista preliminar.
        this.customCssListFiles = [];
        // Listado de ficheros CSS que se usan en DevTools y pueden usarse en Front (Bootstrap y otras librerías de estilos)
        this.basicCssListFiles = [];
        // De momento se permite todos los elementos HTML/Script excepto para las toolbar que no sean EditorHTML
        this.validElements = "*[.*]";
        this.allowJSElements = "true";
        // Plugins adicionales a usar // MathJax, MathType
        // this.additionalPlugins = ""; Versión MathType de eqneditor
        this.additionalPlugins = {};        
        // CSS de comunidad para logar una previsualización de los componentes HTML personalizados
        this.cssFilesArray = $(".styleForCkEditorPreview"); 
        // Indicador de que la hoja de estilos para modificar el propio TinyMCE se ha añadido correctamente
        this.isSetLinkCssForTinyInHead = false;
    },

    /**
     * Configuración de eventos de elementos del Dom (Botones, Inputs...)     
     */
    configEvents: function (pParams) {
    },

    /**
     * Método para configurar las rutas necesarias para el uso de la operativa
     */
    configRutas: function(){
        const that = this;

		this.urlbase = $('input.inpt_baseURL').val();

		if (document.URL.indexOf('https://') == 0) {
			if (this.urlbase.indexOf('https://') == -1) {
				this.urlbase = this.urlbase.replace('http', 'https');
			}
		}    
        // Configuración de las rutas a utilizar para la carga de imágenes    
        this.ImageBrowseUrl = `${this.urlbase}/conector-ckeditor?v=0`;
        this.ImageUploadUrl = `${this.urlbase}/conector-ckeditor?v=0`;
        // Configuración ruta para obtener las hojas de estilo customizadas para previsualización del editor    
	    this.urlGetHojaDeEstilosPersonalizado = `${$("#inpt_baseUrlBusqueda").val()}/custom-stylesheet/get-custom-css`;
    },

    /**
     * Lanzar comportamientos u operativas necesarias para el funcionamiento de la sección
     */
    triggerEvents: async function(){
        const that = this;

        // Cargar hoja de estilos personalizada
        this.getBasicCssForTinyMceEditorPreview();
        // Realizar petición para cargar los customizados. Realizar una única comprobación para las inistancias de los TinyMCE        
        await this.getCustomCssForTinyMceEditorPreview();

        // Observador para editores
        setTimeout(() => {
            setupObserverForTinyMCE(['cke', 'tcme'], function(element) {
                that.setLoadingForTinyMCEInstance();
            });
        }, 500);        
    },


    /**
     * Establecer el loading para la instancia del TinyMCE. Se usa para mostrar un pequeño loading mientras se carga el editor
     * y se cargan los ficheros CSS necesarios para la previsualización del editor.     
     * @param {*} jqueryInputsForTinyMceEditor Instancias de los inputs que se van a cargar como TinyMCE
     */
    setLoadingForTinyMCEInstance: function(){
        const that = this;
        // Asignación de los inputs TinyMCE a los inputs necesarios        
        const jqueryInputsForTinyMceEditor = $(`.cke:not(.${this.tinyLoadedClassName}), .tcme:not(.${this.tinyLoadedClassName})`);
        // Mostrar pequeño loading
        $.each(jqueryInputsForTinyMceEditor, function(){            
            const tinyMCEditor = $(this);
            $(this).addClass(that.tinyLoadedClassName);
            loadingMostrar(tinyMCEditor.parent());                                 
        });
        
        this.initTinyMCEditor(jqueryInputsForTinyMceEditor);    
    },

    /**
     * Método que inicializará la carga de los ficheros CSS para la previsualización en el CKEditor y posteriormente
     * creará la instancia del TinyMCEditor donde corresponde
     * La carga de los ficheros CSS sólo se iniciaría una vez con la carga de la operativa
     * @param {*} jqueryInputsForTinyMceEditor Instancias de los inputs que se van a cargar como TinyMCE
     */
    initTinyMCEditor: function(jqueryInputsForTinyMceEditor){
        const that = this;
        // Usa la configuración por defecto si no se proporciona una toolbar
        let toolbar = this.pParams?.toolbar;

        // Establecer el idioma para el Editor
        this.handleGetCustomLanguageForTinyMCE();
       
        // Aplicar la toolbar a todos los inputs
        $.each(jqueryInputsForTinyMceEditor, function(){            
            const tinyMCEditorInput = $(this); 
            toolbar = that.handleGetTinyMCEditorToolbarForInput(tinyMCEditorInput);                                                                
            that.createTinyMCEInstanceWithToolbar(tinyMCEditorInput, toolbar);
        });

    },  
    
    /**
     * Método para construir el toolbar del editor en base al input textArea. Según la clase que tenga ese input, se asociará un toolbar u otro
     * @param {*} tinyMCEditorInput 
     * @returns Devuelve el array con los plugins del toolbar necesarios
     */
    handleGetTinyMCEditorToolbarForInput: function(tinyMCEditorInput){                
        let toolbar = [];
        // Obtener las clase del input
        const inputClasses = tinyMCEditorInput.attr('class').split(' ');
        let useMathPlugin = inputClasses.includes('mathJax') ? true : false;

        this.useSingleLineToolbars = (tinyMCEditorInput.parent().outerWidth() > this.widthToDisplayToolbarsInRows) || (tinyMCEditorInput.parent().outerWidth() == 0)  ;

        // Lógica para seleccionar el toolbar basado en las clases del input
        if (inputClasses.includes('editorHtml')) {
            toolbar = this.getCompleteToolbar();                        
        } else if (inputClasses.includes('recursos')) {
            toolbar = this.getResourcesToolbar();
        } else if (inputClasses.includes('comentarios')) {
            toolbar = this.getComentariesToolbar();
        } else{
            toolbar = this.getBasicToolbar();
        }

        // Añadir fórmulas matemáticas si así se precisa
        if (useMathPlugin)
        {        
            // Licencia Libre: 
            this.useSingleLineToolbars
            ? toolbar += " formula"
            : toolbar.push('formula');        

            this.additionalPlugins = this.handleMathJaxExternalPlugin(useMathPlugin);
            // Requerido para el plugin de MathType
            this.validElements = "*[.*]";
        }

        return toolbar;
    },

    /**
     * Método para generar un id random para la instancia a crear
     * @param {*} length 
     * @returns 
     */
    createRandomId: function(length) {
        const caracteres = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz';
        let id = '';
        for (let i = 0; i < length; i++) {
            id += caracteres.charAt(Math.floor(Math.random() * caracteres.length));
        }
        return id;
    },    

    /**
     * Método para crear el TinyMCE con el toolbar correspondiente
     * @param {*} jqueryInputForTinyMceInstance Input/TextArea donde se creará el TinyMCE
     * @param {*} customToolbar El toolbar asociado que estará disponible para el input/textArea correspondiente
     */
    createTinyMCEInstanceWithToolbar: function(jqueryInputForTinyMceInstance, customToolbar){       
        const that = this;
        let previousId = null;
            

        // Crear un ID nuevo para evitar problemas de creación de instancias del editor
        const newId = this.createRandomId(6);
        // Comprobar si queryItem tiene un ID asignado
        if (jqueryInputForTinyMceInstance.attr('id')) {
            previousId = jqueryInputForTinyMceInstance.attr('id');        
        }
        // Asignar el nuevo ID al elemento y asignarlo a partir del newId
        jqueryInputForTinyMceInstance.attr('id', newId);
        jqueryInputForTinyMceInstance = $(`#${newId}`);
        
        // Formateo correcto del input por si se usa <pre><code></code></pre>
        const queryItemContent = this.handleConvertPreCodeIntoCompatibleCode(jqueryInputForTinyMceInstance);                     
        jqueryInputForTinyMceInstance.val(queryItemContent);        

        // Input para hacer instancia del Editor
        const queryItem = jqueryInputForTinyMceInstance[0];

        // Convierte el array de estilos a una cadena separada por comas
        const cssFilesString = this.basicCssListFiles.concat(this.customCssListFiles).join(","); 
        
        // Obtener personalizaciones sobre el propio TinyMCE
        this.setLinkCssForTinyInHead();
                          
        tinymce.init({
            target: queryItem,
            // Añadido licencia GPU versión gratuita para V7 - https://www.tiny.cloud/docs/tinymce/latest/license-key/?utm_campaign=console_license_key_message&utm_source=tinymce&utm_medium=referral
            license_key: 'gpl',
            // Método para obtener el idioma para el TinyMCE
            language: this.tinyMCEditorLanguage,           
            branding: false,                        
            plugins: "link, lists, image, table, media, codesample, code, fullscreen, preview, accordion, wordcount, lists advlist, anchor, autolink, formula, quickbars, codeeditor",
            //autosave_ask_before_unload: true,
            //autosave_interval: '20s',
            //autosave_prefix: 'tinymce-autosave-{path}{query}-{id}-',
            //autosave_restore_when_empty: true,
            // autosave_retention: '30m',           
            toolbar: customToolbar,                        
            // Configurar QuickToolbars para selección de items (Sólo para selección de texto)
            quickbars_insert_toolbar: '',
            quickbars_selection_toolbar: 'bold italic underline | bullist numlist | quicklink',
            contextmenu: false, // Disable TinyMCE context menu   
            // CodeEditor -> Estilos
            codeeditor_themes_pack: "twilight merbivore",
            codeeditor_font_size: 14,
            // No mostrar sección en la que el usuario está (<p></p>) en el footer del TinyMCE
            elementpath: false,
            // Anchor plugin
            allow_html_in_named_anchor: true,
            external_plugins: this.additionalPlugins,            
            // Permitir cualquier elemento
            valid_elements: '',                                               
            // No permitir Barra de Menú
            menubar: false,
            // Estilos personalizados para previsualizar contenido
            content_css: cssFilesString,
            // Estilos base para los scrollbars
            content_style: this.contentStyle,
            // Tipo de archivos a aceptar
            file_picker_types: 'image',                                  
            // Método para la subida de imágenes 
            images_upload_handler: this.handleUploadImageFromTinyMCE,       
            toolbar_mode: 'sliding',                            
            // Configurar para actualizar el contenido en input oculto asociado al TinyMCE
            setup: function (editor) {
                // Añadir propiedad para controlar si se está o no escribiendo en la instancia
                editor.isTyping = false;
                let timeout = null;

                // Estilos al contenedor del editor
                editor.on('init', function () {
                    // Espaciado al editor
                    editor.getBody().style.padding = '20px';
                    // Ocultar el loading                    
                    loadingOcultar(jqueryInputForTinyMceInstance.parent());                                        
                    // Devolver su id previo si existía
                    jqueryInputForTinyMceInstance.attr('id', previousId); 
                    // Asignarle una propiedad para saber el padre de este input (su relacionado)
                    jqueryInputForTinyMceInstance.data("editorrelated", editor.id);                 
                    // Asignar clase de que ha finalizado la instancia de la clase
                    jqueryInputForTinyMceInstance.addClass(`${that.tinyLoadedFinishedClassName}`);                                                           
                    // Permitir menú contextual del navegador
                    editor.getBody().addEventListener('contextmenu', function (e) {
                        // Evitar el menú contextual de TinyMCE
                        e.stopPropagation();
                    }, false);
                });
                editor.on('input paste', function (event) {
                    // Establece editor.isTyping en true cuando se está escribiendo
                    editor.isTyping = true;
                    // Cancelar el timeout anterior (si existe)
                    clearTimeout(timeout);

                    if (event.type === 'paste') {
                        // Si es un evento de pegado, esperar un corto periodo antes de obtener el contenido
                        setTimeout(function () {                            
                            const content = editor.getContent();
                            // Actualizar el valor del input oculto
                            jqueryInputForTinyMceInstance.val(content);
                            // Iniciar un nuevo timeout para el evento 'change'
                            timeout = setTimeout(function () {
                                if (editor.isTyping) {
                                    // Disparar el evento 'change' solo si aún se está escribiendo o pegando
                                    editor.isTyping = false;
                                    jqueryInputForTinyMceInstance.trigger('change');
                                }
                            }, 500);
                        }, 100); // Esperar 50ms antes de obtener el contenido                        
                    }else{
                        // Obtener el contenido del editor
                        const content = editor.getContent();
                        // Actualizar el valor del input oculto
                        jqueryInputForTinyMceInstance.val(content);
                        // Iniciar un nuevo timeout para el evento 'change'
                        timeout = setTimeout(function () {
                            if (editor.isTyping) {
                                // Disparar el evento 'change' solo si aún se está escribiendo
                                editor.isTyping = false;
                                jqueryInputForTinyMceInstance.trigger('change');                            
                            }
                        }, 500);
                    }
                });
                editor.on('change', function () {
                    if (!editor.isTyping) {
                        // Obtener el contenido del editor
                        const content = editor.getContent();
                        // Actualizar el valor del input oculto
                        jqueryInputForTinyMceInstance.val(content);
                    }
                });             
            },
            // Permitir ocultar el Tab "General" del plugin de Image y dejar activo el de "Cargar"
            init_instance_callback: function (editor) {
                editor.on("OpenWindow", function(e) {
                  const uploadBtns = document.querySelectorAll(".tox-dialog__body-nav-item.tox-tab")
                    if(uploadBtns.length === 2) {
                      // uploadBtns[0].style.display = "none";
                      uploadBtns[1].click();
                     }
                 })
            },

            // Método para asignar el código disponible
            codesample_languages: this.handleGetCodeSampleLanguages(),            

            // MathJax Configuración
            // We recommend to set 'draggable_modal' to true to avoid overlapping issues
            // with the different UI modal dialog windows implementations between core and third-party plugins on TinyMCE.
            // @see: https://github.com/wiris/html-integrations/issues/134#issuecomment-905448642
            draggable_modal: true,                       
            // This option allows you to introduce mathml formulas with wiris plugins.
            // Not enabling this, will provide formulas from beeing created and rendered.
            // extended_valid_elements: '*[.*]',            
            // Por seguridad lo deshabilito. Sólo se habilitará para ToolbarCompleto
            extended_valid_elements: this.validElements,    
            // Permitir etiquetas style            
            valid_children: '+body[style],+body[link],+body[div],+body[*]',
            // Desactivar la verificación de HTML
            verify_html: false,
            // Permitir el uso de figCaption en imágenes
            image_caption: true,
            // No modificar los enlaces
            convert_urls: false,
            paste_as_text: this.getCopyAsTextPlainConfiguration(jqueryInputForTinyMceInstance),
        });        
    },

    /**
     * Método que añadirá un link con el estilo personalizado "tinymceConfig.css" que contendrá configuración especial para modificar aspectos de TinyMCE
     * tales como Botones, headers, z.index necesarios para proyectos.
     * Se añade aquí para evitar que en proyectos tengan que añadir la hoja externa directamente.
     * De esta forma se gestiona directamente en la propia operativa.
     * Sólo se añadirá si esta no ha sido añadida con anterioridad
     */
    setLinkCssForTinyInHead: function(){        
        if (!this.isSetLinkCssForTinyInHead){
            try {
                // Fichero de estilos que modifican el propio TinyMCE de forma dinámica y añadirlos al Head        
                const tinyMceConfigCSS = this.getCurrentScriptUrl().replace("tinymceConfig.js","tinymceConfig.css");                
                if (tinyMceConfigCSS.trim() !== '') {
                    // Crea un nuevo elemento <link> con los estilos CSS externos
                    var linkTinyMceConfigCSS = $(`<link rel="stylesheet" type="text/css" href="${tinyMceConfigCSS}">`);
                    linkTinyMceConfigCSS.appendTo('head'); 
                    this.isSetLinkCssForTinyInHead = true;
                } else {
                    console.log("El nombre del archivo tinymceConfig.css está vacío.");
                }
            } catch (error) {
                console.log("Error al tratar de cargar el fichero tinymceConfig.css");
            } 
        }      
    },
    
    /**
     * Método que devuelve la ruta en la que se encuentra el fichero Javascript que se está ejecutando.
     * Se utilizará para añadir la hoja CSS para personalizar items del propio TinyMCE de forma dinámica.
     */
    getCurrentScriptUrl: function(){
        try {
          throw new Error();
        } catch (error) {
          const stackTrace = error.stack || '';
          const match = stackTrace.match(/(https?:\/\/.+\.js)/);
          return match ? match[1] : '';
        }
    },      

    /**
     * Método para devolver el plugin de wiris de MathType. Se ha de cargar en external_plugins
     * @returns Objeto con la configuración de TinyMCE de Wiris
     */
    handleMathJaxExternalPlugin: function(addMathTypePlugin = false){        
        if (addMathTypePlugin){
            return {
                'tiny_mce_wiris': `plugins/wiris/mathtype-tinymce6/plugin.min.js`,
            };       
        }        
        return {};
    },

    /**
     * Método que construye un array con los lenguajes disponibles a usar en el plugin de CodeSample
     * @returns Devuelve un array con los lenguajes disponibles. Se añade SPARQL.
     */
    handleGetCodeSampleLanguages: function(){
        return [
            { text: 'HTML/XML', value: 'markup' },
            { text: 'JavaScript', value: 'javascript' },
            { text: 'CSS', value: 'css' },
            { text: 'PHP', value: 'php' },
            { text: 'Ruby', value: 'ruby' },
            { text: 'Python', value: 'python' },
            { text: 'Java', value: 'java' },
            { text: 'C', value: 'c' },
            { text: 'C#', value: 'csharp' },
            { text: 'C++', value: 'cpp' },
            { text: 'SPARQL', value: 'sparql' }, // Añadido SPARQL
            { text: 'Turtle', value: 'turtle' } // Añadido Turtle
        ];
    },

    /**
     * Método para obtener el idioma de la página para establecerselo al TinyMCE. Por defecto, si hay algún error, se establecerá el idioma de español "es".
     * @returns 
     */
    handleGetCustomLanguageForTinyMCE: function(){        
        const currentLanguage = $("#inpt_Idioma").val().trim();
        if (currentLanguage.length != 0)
        {
            this.tinyMCEditorLanguage = currentLanguage;
        }        
    },

    /**
     * Método para realizar la subida de una imagen para el servidor de GNOSS. Una vez finalizada la subida/carga de imagen, proporcionará la URL para pulsar en ACEPTAR 
     * @param {*} blobInfo 
     * @param {*} progress 
     * @returns 
     */
    handleUploadImageFromTinyMCE: function (blobInfo, progress) {
        return new Promise((resolve, reject) => {
            const formData = new FormData();
            formData.append('image', blobInfo.blob(), blobInfo.filename());
    
            fetch(operativaTinyMceConfig.ImageBrowseUrl, {
                method: 'POST',
                body: formData
            })
            .then(response => {
                if (!response.ok) {
                    throw new Error('HTTP Error: ' + response.status);
                }
                return response.json();
            })
            .then(data => {
                if (!data || typeof data.url !== 'string') {
                    throw new Error('Invalid JSON: ' + JSON.stringify(data));
                }
                resolve(data.url);
            })
            .catch(error => {
                reject('Error al subir la imagen: ' + error.message);
                console.error('Error al subir la imagen:', error);
            });
        });
    },

    /**
     * Método para convertir el código existente dentro del Input donde se va a mostrar el TinyMCE en código legible para
     * que el plugin de CodeBlocks lo muestre correctamente en su edición o carga inicializal.
     */
    handleConvertPreCodeIntoCompatibleCode: function(input){                       
        // Encuentra todas las coincidencias de <pre>...</pre> en el texto proporcionado
        var regex = /<pre.*?>[\s\S]*?<code.*?>([\s\S]*?)<\/code><\/pre>/gi;
        let preHTML = input.val();
        var matches = preHTML.match(regex);
        
        // Si no hay coincidencias, devuelve el HTML original sin cambios
        if (!matches) {
            return preHTML;
        }
        
        // Itera sobre todas las coincidencias encontradas
        for (var i = 0; i < matches.length; i++) {
            var preCodeHTML = matches[i];
            // Obtén el contenido HTML dentro de las etiquetas <code>
            var codeContent = preCodeHTML.replace(/<pre.*?>[\s\S]*?<code.*?>([\s\S]*?)<\/code><\/pre>/i, '$1');
            // Realiza las conversiones necesarias en el contenido HTML
            var convertedContent = codeContent.replace(/<|>/g, function(match) {
                return (match === '<') ? '&lt;' : '&gt;';
            });
            // Reemplaza el contenido HTML original con el contenido convertido
            preHTML = preHTML.replace(codeContent, convertedContent);
        }
        
        // Devuelve el HTML con las conversiones realizadas
        return preHTML;        

    },   

    /**
     * Método para cargar las hojas de estilo customizadas por el usuario dentro del editor con el objetivo de poder ver
     * fielmente cómo queda el componente CMS en vivo con los estilos a usar en el front de la comunidad.
     * Esta función hace una petición a la vista "_HojasDeEstiloPersonalizado" para obtener los ficheros CSS que ahí están indicados y 
     * crea un array con estilos para ser añadidos posteriormente al editor
     * El sistema cachea las vistas 
     * Se utilizará el controller: AdministrarHead/GetHojaDeEstilosPersonalizado
     */  
    getCustomCssForTinyMceEditorPreview: function() {        
        let cssFilesValue = [];

        const that = this;

        // Eliminar posibles entradas antiguas del localStorage
        this.cleanExpiredLocalStorageEntries();          
            
        // Comprobar si los estilos están en el LocalStorage y si son válidos para el dominio actual
        const isCustomCssForTinyMceInCache = this.checkAndGetCustomCssForTinyMceInCache();
    
        // Si están en caché, no es necesario realizar la petición
        if (isCustomCssForTinyMceInCache) {
            return Promise.resolve();
        }

        return new Promise(function(resolve, reject) {
            // Realizar la petición
            GnossPeticionAjax(                
                that.urlGetHojaDeEstilosPersonalizado,
                null,
                true
            ).done(function(data) {
                // Crear un objeto jQuery a partir de la cadena HTML
                const $html = $(data.trim());                
                // Encontrar todos los elementos <link> dentro del objeto jQuery            
                const linkElements = $html.filter("link");                        
                // Añadir los estilos personalizados obtenidos
                if (linkElements.length > 0) {
                    cssFilesValue = linkElements.map(function() {
                        return $(this).prop("href");
                    }).get();
                    // Guardar los estilos en el LocalStorage con el nombre específico del dominio
                    localStorage.setItem(`${that.CUSTOM_CSS_FOR_TINY_MCE_KEY}_${that.currentDomain}`, JSON.stringify(cssFilesValue));
                    localStorage.setItem(`${that.CUSTOM_CSS_FOR_TINY_MCE_TIMESTAMP_KEY}_${that.currentDomain}`, new Date().getTime());
                }              
                // OK Asignación de los estilos personalizados
                that.customCssListFiles = cssFilesValue;
                resolve();
            }).fail(function(data) {
                // KO. Error en la petición.
                reject(data);
            }); 
        });
    },


    /**
     * Método para comprobar si los estilos personalizados para el TinyMCE están en caché y, si es así, los carga.
     * Este método se utiliza para evitar realizar una petición al servidor si los estilos ya están en caché.
     * Si están en caché, se cargan desde el LocalStorage y se asignan a la variable customCssListFiles.
     * Si no están en caché, devuelve false.
     * @returns {boolean} Devuelve true si los estilos están en caché, false en caso contrario.
     */
    checkAndGetCustomCssForTinyMceInCache: function(){        
        // Comprobar si los estilos están en el LocalStorage y si son válidos para el dominio actual
        const cachedCss = localStorage.getItem(`${this.CUSTOM_CSS_FOR_TINY_MCE_KEY}_${this.currentDomain}`);
        const cachedCssTimestamp = localStorage.getItem(`${this.CUSTOM_CSS_FOR_TINY_MCE_TIMESTAMP_KEY}_${this.currentDomain}`);

        if (cachedCss && cachedCssTimestamp) {
            const cacheExpiration = this.CUSTOM_CSS_CACHE_EXPIRATION_TIME;
            const currentTime = new Date().getTime();
            if (currentTime - cachedCssTimestamp < cacheExpiration) {
                cssFilesValue = JSON.parse(cachedCss);
                this.customCssListFiles = cssFilesValue;
                return true;
            }
        }
        return false;
    },


    /**
     * Método para limpiar el localStorage eliminando las entradas caducadas por dominio para no generar contenido residual del CSS.
     */
    cleanExpiredLocalStorageEntries: function() {
        const that = this;

        const currentTime = new Date().getTime();
        const fifteenDaysAgo = currentTime - (15 * 24 * 60 * 60 * 1000); // 15 días en milisegundos
    
        // Recorrer todas las claves del localStorage
        for (let i = 0; i < localStorage.length; i++) {
            const key_TIMESTAMP_KEY = localStorage.key(i);
            // Comprobar si la clave está relacionada con los estilos personalizados y su timestamp es antiguo
            if (key_TIMESTAMP_KEY.startsWith(`${this.CUSTOM_CSS_FOR_TINY_MCE_TIMESTAMP_KEY}`)) {                                                                
                // Clave del dominio a eliminar
                const startIndexDomain = this.CUSTOM_CSS_FOR_TINY_MCE_TIMESTAMP_KEY.length;                
                // Obtener el substring que contiene el dominio
                const domainSubstring = key_TIMESTAMP_KEY.substring(startIndexDomain);
                const keyDomainTinyMCE = `${this.CUSTOM_CSS_FOR_TINY_MCE_KEY}${domainSubstring}`;
                const lastUpdatedTimestamp = parseInt(localStorage.getItem(`${this.CUSTOM_CSS_FOR_TINY_MCE_TIMESTAMP_KEY}${domainSubstring}`));
                // Comprobar si la entrada está caducada y eliminarla si es necesario
                if (lastUpdatedTimestamp && lastUpdatedTimestamp < fifteenDaysAgo) {
                    localStorage.removeItem(key_TIMESTAMP_KEY);
                    localStorage.removeItem(keyDomainTinyMCE);
                }
            }
        }
    },

    /**
     * Establecer si se desea (true/false) que el copiar y pegar texto plano (sin estilos) esté por defecto activado. 
     * @param {*} jqueryInputForTinyMceInstance 
     * @returns {bool} Devuelve valor booleano indicando si estará o no activado el copiar como texto plano. Por defecto "false".
     */
    getCopyAsTextPlainConfiguration: function(jqueryInputForTinyMceInstance){
        return jqueryInputForTinyMceInstance.hasClass("cke-copy-plain-text") || jqueryInputForTinyMceInstance.hasClass("tiny-copy-plain-text");
    },       

    /**
     * Método para cargar las hojas de estilos que se utilizan en la plataforma, tanto para Front como en Backoffice (Bootstrap y librerías similares para lograr una visualización más o menos correcta)
     * Sobre estos css es posible que apliquen personalizaciones realizadas por el usuario las cuales se añadirán en getCustomCssForTinyMceEditorPreview
     */
    getBasicCssForTinyMceEditorPreview: function(){        
        // Código CSS para los scrollbars del TinyMCE
        this.contentStyle = 
        `
            /* Estilos para el scrollbar vertical */
            /* Cambiar el color del scrollbar */
            ::-webkit-scrollbar {
            width: 8px;
            background-color: #f0f0f0;
            }
            
            /* Cambiar el color de la barra del scrollbar */
            ::-webkit-scrollbar-thumb {
            background-color: #c0c0c0;
            }
            
            /* Estilos para el scrollbar horizontal (si es necesario) */
            /* Cambiar el color del scrollbar */
            ::-webkit-scrollbar:horizontal {
            height: 8px;
            background-color: #f0f0f0;
            }
            
            /* Cambiar el color de la barra del scrollbar */
            ::-webkit-scrollbar-thumb:horizontal {
            background-color: #c0c0c0;
            }        
        `;
        
        // Comprobar que no es necesario la carga de los CSS ya que ya han sido cargados con anterioridad
        if (this.basicCssListFiles.length > 0){
            return;
        }

        let cssFilesValue = [];

        if (this.cssFilesArray.length > 0){
            cssFilesValue = $.map(this.cssFilesArray, function(item) {
                const link = $(item).prop("href");
                return link;
            });
            this.basicCssListFiles = cssFilesValue;
        }    
    },

    /**
     * Método que devuelve la configuración por defecto del toolbar para una configuración completa
     * @returns Array de botones para el toolbar por defecto
     */
    getCompleteToolbar: function(){        
        if (this.useSingleLineToolbars){        
                return `undo redo | fullscreen codeeditor codesample preview | aligncenter alignjustify alignleft alignright | image table media link anchor | forecolor backcolor bold italic underline strikethrough pastetext | hr outdent indent bullist numlist subscript superscript | styles fontfamily fontsize`;            
        }
        
        return ['undo redo | fullscreen codeeditor codesample preview | aligncenter alignjustify alignleft alignright | outdent indent image table media hr link anchor',
                'styles fontfamily fontsize forecolor backcolor bold italic underline strikethrough | bullist numlist subscript superscript pastetext',]
    },

    /**
     * Método que devuelve la configuración por defecto del toolbar para una configuración completa
     * @returns Array de botones para el toolbar por defecto
     */
    getBasicToolbar: function(){         
        // No permitir scripts en editor
        this.validElements = ""; 
        this.allowJSElements = false; 
        
        if (this.useSingleLineToolbars){                    
            return `fullscreen | aligncenter alignjustify alignleft alignright | image table media link anchor | forecolor backcolor bold italic underline strikethrough pastetext | hr outdent indent bullist numlist subscript superscript | styles`;  

        }        
        return ['fullscreen | aligncenter alignjustify alignleft alignright | outdent indent image table media hr link anchor',
                'styles forecolor backcolor bold italic underline strikethrough | bullist numlist subscript superscript pastetext',]
        
    },

    /**
     * Método que devuelve la configuración por defecto del toolbar para una configuración completa
     * @returns Array de botones para el toolbar por defecto
     */
    getResourcesToolbar: function(){           
        // No permitir scripts en editor. Permitir etiquetas script para creación de recursos        
        this.allowJSElements = false; 
                
        if (this.useSingleLineToolbars){                    
            return `fullscreen codeeditor codesample | aligncenter alignjustify alignleft alignright | image table media link anchor | forecolor backcolor bold italic underline strikethrough pastetext | hr outdent indent bullist numlist subscript superscript | styles`;  
        }
        
        return ['fullscreen codeeditor codesample | aligncenter alignjustify alignleft alignright | outdent indent image table media hr link anchor',
                'styles forecolor backcolor bold italic underline strikethrough | bullist numlist subscript superscript pastetext',]        
    },    
      
    /**
     * Método que devolverá la configuración básica del toolbar
     * @returns Devuelve un array con los botones necesarios para el toolbar básico
     */
    getComentariesToolbar: function(){        
        // No permitir scripts en editor
        this.validElements = "";
        this.allowJSElements = false; 
        return ['link'];
    },     
};