/**
 ***************************************************************************************
 * Logica / Operativas de comportamiento JS para la sección de Grafos de conocimiento de la Comunidad del DevTools
 * *************************************************************************************
 */

/**
  * Operativa para la gestión/configuración de los objetos de conocimiento y las ontologías de la comunidad
  */
const operativaGestionObjetosConocimientoOntologias = {

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

    /**
     * Lanzar comportamientos u operativas necesarias para el funcionamiento de la sección
     */
    triggerEvents: function(){
        const that = this;
                       
        // Operativa multiIdiomas
        // Parámetros para la operativa multiIdioma (helpers.js)
        this.operativaMultiIdiomaParams = {
            // Nº máximo de pestañas con idiomas a mostrar. Los demás quedarán ocultos
            numIdiomasVisibles: 3,
            // Establecer 1 tab por cada input (true, false) - False es la forma vieja
            useOnlyOneTab: true,
            panContenidoMultiIdiomaClassName: "panContenidoMultiIdioma",
            // No permitir padding bottom y si padding top
            allowPaddingBottom: false,
            allowPaddingTop: true,
            paddingTopValue: 4,
            allowPaddingLeft: false, 
            allowPaddingTopInHeaders: false,           
            allowPaddingRight: false,
        };  

        // Inicializar operativa multiIdioma
        operativaMultiIdioma.init(this.operativaMultiIdiomaParams);        
        
        // Inicializar comportamientos de select2
        comportamientoInicial.iniciarSelects2(); 

        // Contabilizar el nº de objetos de conocimiento
        this.handleCheckNumberOfObjetosConocimiento(); 
        
        // Deshabilitar o habilitar los paneles GnossDragAndDrop si se desea utilizar los checkbox con ficheros genéricos
        // Cargar funcionalidad de "GnossDragAndDrop" para los ficheros
        this.initDragAndDropForObjetosConocimiento();
        // Reiniciar funcionamiento de ckEditor
        ckEditorRecargarTodos();
    },  
    
    /**
     * Inicializar y configurar los paneles de dragAndDrop para ficheros de ontologías
     */
    initDragAndDropForObjetosConocimiento: function(){
        const that = this;

        that.handleCheckDragAndDropStatus();
        dragAndDropGnoss();
    },

    /**
     * Configuración de las rutas de acciones necesarias para hacer peticiones a backend
     */
    configRutas: function () {
        // Url para editar un certificado
        this.urlBase = refineURL();
        this.urlCreateObjetoConocimiento = `${this.urlBase}/load-add-objeto-conocimiento`;
        // Url para guardar la ontología/nuevo objeto de conocimiento desde 0.
        this.urlSaveNewObjetoConocimiento = `${this.urlBase}/save-ontology`;        
        // Url para guardar la ontología o los datos del objeto de conocimiento ya existente (Editando)
        this.urlSaveObjetoConocimientoDetails = `${this.urlBase}/save-oc-details`;
        // Url para eliminar un objeto de conocimiento 
        this.urlDeleteObjetoConocimiento = `${this.urlBase}/delete-oc`;
        // Url para solo guardar los ficheros del objeto de conocimiento. El objeto de conocimiento debe existir.
        this.urlSaveObjetoConocimientoFicheros = `${this.urlBase}/save-ontology-details`;
        // Url para descargar el fichero de la ontología
        this.urlDownloadOntologyFile = `${this.urlBase}/download-ontology`;
        // Url para descargar el fichero de configuración la ontología
        this.urlDownloadOntologyConfigurationFile = `${this.urlBase}/download-ontology-configuration`;
        this.urlAddNewSubtipo = `${this.urlBase}/add-subtipo`;
        this.urlAddNewProperty = `${this.urlBase}/add-property`;
        this.urlAddNewCustomProperty = `${this.urlBase}/add-custom-property`;
        this.urlSaveObjetosConocimiento = `${this.urlBase}/save`;
        // Descarga de vistas y vistas java
        this.urlDownloadClasesVistas = `${this.urlBase}/download-classes`;
        this.urlDownloadClasesVistasJava = `${this.urlBase}/download-classes-java`;        
        
        // Url para cargar los elementos de un objeto de conocimiento secundario.
        this.urlLoadSecondaryEntities = `${this.urlBase}/load-elements-secondary-entity`;
        // Url para cargar los detalles de un elemento de un objeto de conocimiento secundario.
        this.urlLoadSecondaryEntityDetails = `${this.urlBase}/edit-element-secondary-entity`;
        // Url para crear un nuevo elemento o entidad en un objeto de conocimiento secundario.
        this.urlLoadCreateNewEntity = `${this.urlBase}/new-element-secondary-entity`;
        // Url para guardar los datos de un elemento de un objeto de conocimiento secundario que ha sido editado o creado nuevo
        this.urlSaveSecondaryEntityDetails = `${this.urlBase}/save-secondary-entity`;
        // Url para eliminar un elemento de un objeto de conocimiento secundario  
        this.urlDeleteSecondaryEntity = `${this.urlBase}/delete-secondary-entity`;
        // Url para cargar los ficheros históricos de un objeto de conocimiento
        this.urlLoadHistoryFiles = `${this.urlBase}/load-history-file-items`;

        
        // Objeto donde se guardarán las opciones para su guardado
        this.ListaObjetosConocimiento = {};

        // Fila del objeto de conocimiento que se está editando
        this.filaObjetoConocimiento = undefined;
        // Panel de propiedades activo
        this.panelConfigurarPropiedades = undefined;                        

        // Flag que indica si se está borrando un determinado objeto de conocimiento
        this.confirmDeleteObjetoConocimiento = false;
        // Flag que indica el modal que está abierto;
        this.currentModalOC = undefined;
        // Datos donde se guardarán los objetos para guardar los OC
        this.dataPost = new FormData();
    },  

    /*
    * Inicializar elementos de la vista
    * */
    config: function (pParams) {
        
        // Contenedor modal
        this.modalContainer = $("#modal-container");
        // Modal del objeto de conocimiento que está siendo editado
        this.modalObjetoConocimientoClassName = 'modal-objetoConocimiento';
        this.modalEdicionClassName = "modal-edicion";
        this.modalDeleteObjetoConocimientoClassName = 'modal-confirmDelete';

        /* Idiomas Tabs */
        // Tab de idioma de la página para cambiar la visualización en el idioma
        this.tabIdiomaItem = $(".tabIdiomaItem ");        
        // Cada una de las labels (Nombre literal) en los diferentes idiomas (por `data-languageitem`)
        this.labelLanguageComponent = $(".language-component");        

        // Contador del número de objetos de conocimiento existentes
        this.numResultadosObjetosConocimiento = $("#numObjetosConocimiento");
        this.btnAddSecondaryElementClassName = "btnAddSecondaryElement";

        // Contenedor de objetos de conocimiento
        this.objetosConocimientoListContainerId = 'js-community-objetosConocimiento-list';        
        this.objetosConocimientoListContainer = $(`#${this.objetosConocimientoListContainerId}`);
        // Nombre de la fila de cada objeto de conocimiento
        this.objetoConocimientoListItemClassName = "objetoConocimiento-row";
        
        // Botón de editar objeto de conocimiento
        this.btnEditObjetoConocimientoClassName = "btnEditObjetoConocimiento";
        // Botón para descargar plantilla de objeto de conocimiento
        this.btnDownloadTemplateObjetoConocimientoClassName = "btnDownloadTemplateObjetoConocimiento";
        // Botón para descargar archivo de configuración
        this.btnDownloadConfigurationFileObjetoConocimientoClassName = "btnDownloadConfigurationFileObjetoConocimiento";       
        // Botón para eliminar un objeto de conocimiento
        this.btnDeleteObjetoConocimientoClassName = "btnDeleteObjetoConocimiento";
        // Botón para confirmar la eliminación de un objeto de conocimiento
        this.btnConfirmDeleteObjetoConocimientoClassName = "btnConfirmDeleteObjetoConocimiento";
        // Botón de no confirmar el borrado de un objeto de conocimiento
        this.btnNotConfirmDeleteObjetoConocimientoClassName = "btnNotConfirmDeleteObjetoConocimiento";
        
        /* Modal de edición de un objeto de conocimiento */
        // Tab para visualizar "Ficheros de la ontología"
        this.tabFicherosOntologiaClassName = "tabFicherosOntologia";
        // Tabs de navegación por los paneles de un objeto de conocimiento (Plantillas, Configuración, Elementos, Avanzados)
        this.tabObjetoConocimientoNavigationClassName = "nav-objetoConocimiento";
        // Tab para visualizar "Configuración del objeto de conocimiento"
        this.tabConfiguracionObjetoConocimientoClassName = "tabConfiguracionObjetoConocimiento";
        // Tab para visualizar "Elementos del objeto de conocimiento (Secundario)"        
        this.tabConfiguracionElementosObjetoConocimientoClassName = "tabConfiguracionElementosObjetoConocimiento";        
        // Tab para visualizar "Presentación de recursos"
        this.tabConfiguracionPresentacionRecursosClassName = "tabConfiguracionPresentacionRecursos";
        // Tab para visualizar "Opciones avanzadas del objeto de conocimiento"
        this.tabConfiguracionAvanzadaObjetoConocimientoClassName = "tabConfiguracionAvanzadaObjetoConocimiento";
        // Tab para visualizar el "Historico de ficheros de una ontología"
        this.tabConfiguracionHistoricoFicherosObjetoConocimientoClassName = "tabConfiguracionHistoricoFicherosObjetoConocimiento";
        // Contenedor de los ficheros donde se mostrarán
        this.ontologyFileHistorialPanelClassName = "ontology-file-historial-list";
        // Inputs de Grafo
        this.inputGrafoObjetoConocimientoClassName = "inputGrafoObjetoConocimiento"

        /* Sección Creación Subtipos */
        // Listado donde se muestran los subtipos del objeto de conocimiento
        this.idAddedSubtipoListClassName = "id-added-subtipo-list";
        // Botón para añadir un subtipo
        this.linkAddSubtipoClassName = "linkAddSubtipo";
        // Cada fila subtipo
        this.subtipoRowClassName = "subtipo-row";
        // Botón para editar subtipo
        this.btnEditSubtipoClassName = "btnEditSubtipo";
        // Botón para eliminar subtipo
        this.btnDeleteSubtipoClassName = "btnDeleteSubtipo";

        /* Sección Creación propiedades */
        // Listado donde se muestran los subtipos del objeto de conocimiento
        this.idAddedPropertyListClassName = "id-added-property-list";
        // Botón para añadir una propiedad
        this.linkAddPropertyClassName = "linkAddProperty";
        // Botón para añadir una propiedad personalizada
        this.linkAddCustomPropertyClassName = "linkAddCustomProperty";        
        // Cada fila property
        this.propertyRowClassName = "property-row";
        // Botón para editar property
        this.btnEditPropertyClassName = "btnEditProperty";
        // Botón para eliminar subtipo
        this.btnDeletePropertyClassName = "btnDeleteProperty";
        // Input de Nombre propiedad
        this.inputNombrePropiedadValueClassName = "inputNombrePropiedadValue";
        // Input de Presentación propiedad
        this.inputPresentacionValueClassName = "inputPresentacionValue"; 

        /* Modal creación de Objeto de conocimiento */
        // RadioButton Objeto de conocimiento Primario
        this.crearObjetoConocimientoPrimario = $("#crearObjetoConocimientoPrimario");
        // RadioButton Objeto de conocimiento Secundario
        this.crearObjetoConocimientoSecundario = $("#crearObjetoConocimientoSecundario");
        // Nombre del valor (radioButton) del objeto de conocimiento a crear
        this.crearObjetoConocimientoName = "crearObjetoConocimiento";
        
        // Sección del Drag para la plantilla (owl)
        this.panelObjetoConocimientoOwlTemplateClassName = "panelObjetoConocimientoOwlTemplate";
        // Input type file de la plantilla (owl)
        this.objetoConocimientoOwlFileClassName = "dragOwl";
        // Botón de Reemplazar plantilla (owl)
        this.btnCancelObjetoConocimientoOwlTemplateClassName = "btnCancelObjetoConocimientoOwlTemplate";

        // Sección del Drag para la plantilla (html)
        this.panelObjetoConocimientoHtmlTemplateClassName = "panelObjetoConocimientoHtmlTemplate";
        // Input type file de la plantilla (html)
        this.objetoConocimientoHtmlFileClassName = "dragHtml";
        // Clase del checkbox para utilizar fichero genérico
        this.utilizarArchivoGenericoHtmlClassName = "utilizarArchivoGenericoHtml"; 
        // Botón de Reemplazar plantilla (html)
        this.btnCancelObjetoConocimientoHtmlTemplateClassName = "btnCancelObjetoConocimientoHtmlTemplate";

        // Sección del Drag para la plantilla (icono)
        this.panelObjetoConocimientoIconoImageTemplateClassName = "panelObjetoConocimientoIconoTemplate";
        // Input type file de la plantilla (Icono)
        this.objetoConocimientoIconoFileClassName = "dragIcono";
        // Clase del checkbox para utilizar fichero genérico
        this.utilizarArchivoGenericoIconoClassName = "utilizarArchivoGenericoIcono";
        // Botón de Reemplazar plantilla (icono)
        this.btnCancelObjetoConocimientoIconoTemplateClassName = "btnCancelObjetoConocimientoIconoTemplate";

        // Sección del Drag para la plantilla (css)
        this.panelObjetoConocimientoCssTemplateClassName = "panelObjetoConocimientoCssTemplate";
        // Input type file de la plantilla (Css)
        this.objetoConocimientoCssFileClassName = "dragCss";    
        // Clase del checkbox para utilizar fichero genérico
        this.utilizarArchivoGenericoCssClassName = "utilizarArchivoGenericoCss";  
        // Botón de Reemplazar plantilla (css)
        this.btnCancelObjetoConocimientoCssTemplateClassName = "btnCancelObjetoConocimientoCssTemplate";
        
        // Sección del Drag para la plantilla (js)
        this.panelObjetoConocimientoJsTemplateClassName = "panelObjetoConocimientoJsTemplate";
        // Input type file de la plantilla (Js)
        this.objetoConocimientoJsFileClassName = "dragJs"; 
        // Secciones del Drag para la plantilla (Todos los paneles )
        this.panelObjetoConocimientoTemplateClassName = "panelObjetoConocimientoTemplate";
        // Secciones con los ficheros de todas las plantillas
        this.panelObjetoConocimientoFilesClassName = "panelObjetoConocimientoFiles"
        // Clase del checkbox para utilizar fichero genérico
        this.utilizarArchivoGenericoJsClassName = "utilizarArchivoGenericoJs";
        // Botón de Reemplazar plantilla (js)
        this.btnCancelObjetoConocimientoJsTemplateClassName = "btnCancelObjetoConocimientoJsTemplate";

        // Checkbox de utilizarArchivoGenerico para todos los items
        this.chkUtilizarArchivoGenericoClassName = "utilizarArchivoGenerico";

        // Checkbox oculto para controlar si se desea "editar" una plantilla de un objeto de conocimiento
        this.chkEditObjetoConocimientoClassName = "chkEditObjetoConocimiento";
        // Botón para Cancelar o Reemplazar una plantilla
        this.btnCancelObjetoConocimientoClassName = "btnCancelObjetoConocimiento"; 

        // Botón para guardar el nuevo objeto de conocimiento
        this.btnSaveNewObjetoConocimientoClassName = "btnSaveNewObjetoConocimiento";
        // Panel donde se encuentran los DragAndDrop en el momento de crear un nuevo OC
        this.panelOntologyFilesClassName = "panelOntologyFiles";

        // Buscador de objetos de conocimiento
        this.txtBuscarObjetoConocimiento = $("#txtBuscarObjetoConocimiento");
        
        // Botón para crear un nuevo objeto de conocimiento
        this.btnNewOC = $("#btnNewOC");
        // Botones para descargar vistas y clases
        this.btnDownloadClasesYVistas = $("#btnDownloadClasesYVistas");
        this.btnDownloadClasesYVistasJava = $("#btnDownloadClasesYVistasJava");

        /* Objetos de conocimiento secundarios */
        // Buscador de elementos de un objeto de conocimiento secundario
        this.findSecundaryOntologyElementsClassName = "findSecundaryOntologyElements";
        // Contador de entidades que tiene cada objeto de conocimiento secundario
        this.numResultadosEntitiesClassName = "numResultadosEntities";
        // Elemento para mostrar o no los elementos que no se han guardado
        this.checkboxShowUnsavedItemsClassName = "checkboxShowUnsavedItems";
        // Cada una de las filas de los elementos que forman parte de una ontología secundaria
        this.elementRowClassName = "element-row";
        // Botón para aplicar el guardado de una instancia de un elemento que haya sido editado o modificado
        this.btnSaveInstanciaEntidadClassName = "btnSaveInstanciaEntidad";
        // Fila con mensaje que indica que no hay elementos asociados a un objeto de conocimiento secundario
        this.elementRowNoElements = "element-row-no-elements"
        // Botón para editar un elemento del objeto de conocimiento secundario
        this.btnEditElementClassName = "btnEditElement";
        // Botón para eliminar un elemento del objeto de conocimiento secundario
        this.btnDeleteElementClassName = "btnDeleteElement";
        // Panel donde se cargan los elementos del objeto de conocimiento secundario
        this.panelElementsSecondaryOntologyClassName = "element-list";
        // Panel de tipo collapse donde se mostrarán los datos del elemento
        this.panelElementDetailClassName = "panelElementDetail";
        // Contenedor de cada input del elemento de un objeto de conocimiento secundario
        this.propertyItemClassName = "property-item";
        // Input donde se muestra el ID del elemento de un objeto de conocimiento secundario
        this.sujEntidadSecClassName = "sujEntidadSec";        
        // Label donde se muestra el nombre del elemento de un objeto de conocimiento secundario
        this.labelElementNameClassName = "labelElementName";
        // Label donde se muestra información adicional del elemento (Donde se muestra que está siendo editado o aun no se ha guardado)
        this.labelElementExtraInfoClassName = "labelElementExtraInfo";     

        // Sección Histórico de ficheros
        // Número de ficheros "Históricos" encontrados de un objeto de conocimiento
        this.numResultadosHistoryFilesClassName = "numResultadosHistoryFiles";
        // Input para realizar búsqueda de elementos
        this.findHistoryFileElementsClassName = "findHistoryFileElements";
        // Cada una de las filas de los ficheros históricos
        this.historialRowClassName = "historial-row";
        
        // Botones para acciones de objeto secundario (MVCSem)
        this.btnDeleteGroupValueClassName = "btnDeleteGroupValue";
        this.btnEditGroupValueClassName = "btnEditGroupValue";

        this.btnDeleteObjectNoFuncionalPropClassName = "btnDeleteObjectNoFuncionalProp";

        
        // Botón para guardar un objeto
        this.btnSaveObjetoConocimientoClassName = "btnSave";
    },   

    /**
     * Configuración de eventos de elementos del Dom (Botones, Inputs...)     
     */
    configEvents: function (pParams) {
        const that = this;

        // Comportamientos del modal container 
        this.modalContainer.on('show.bs.modal', (e) => {
            // Aparición del modal
            // Establecer quién ha disparado el modal para poder cambiar sus atributos una vez se guarden los grupos desde el modal
            that.triggerModalContainer = $(e.relatedTarget); 
            that.currentModalOC = $(e.target);
        })
        .on('hide.bs.modal', (e) => {
            // Acción de ocultar el modal
        })
        .on('hidden.bs.modal', (e) => {
            // Vaciar el modal
            resetearModalContainer(); 
        });

        // Comportamientos del modal de borrado de OC
        configEventByClassName(`${that.modalDeleteObjetoConocimientoClassName}`, function(element){
            const $modalDelete = $(element);
            $modalDelete
            .on('show.bs.modal', (e) => {
                // Aparición del modal
                that.triggerModalContainer = $(e.relatedTarget);
            })
            .on('hide.bs.modal', (e) => {
                // Acción de ocultar el modal
                if (that.confirmDeleteObjetoConocimiento == false){
                    that.handleSetDeleteObjetoConocimiento(false);                
                }               
            }); 
        }); 
        
        // Comportamientos del modal de detalles del OC
        configEventByClassName(`${that.modalObjetoConocimientoClassName}`, function(element){
            const $modal = $(element);
            $modal
            .on('show.bs.modal', (e) => {
                // Aparición del modal
                that.triggerModalContainer = $(e.relatedTarget);
                that.currentModalOC = $(e.target);
            })
            .on('hide.bs.modal', (e) => {
                // Acción de ocultar el modal              
            }); 
        });
        
        // Tab de cada uno de los idiomas disponibles para cambiar la visualización y poder ver el nombre y url del idioma elegido
        this.tabIdiomaItem.off().on("click", function(){
            that.handleViewOcLanguageInfo();            
        });        

        // Botón para editar un objeto de conocimiento       
        configEventByClassName(`${that.btnEditObjetoConocimientoClassName}`, function(element){
            const $jqueryElement = $(element);
            $jqueryElement.off().on("click", function(){                                                 
                // Guardamos referencia de la fila del objeto de conocimiento a editar/crear
                that.filaObjetoConocimiento = $(this).closest(`.${that.objetoConocimientoListItemClassName}`);                            
                // Comprobar el tipo de objeto de conocimiento (Primario o Secundario) y establecer el Tab activo
                that.handleSetTabActiveForSeeDetails(that.filaObjetoConocimiento);
            });	                        
        });

        // Botón para añadir un nuevo subtipo al objeto de conocimiento
        configEventByClassName(`${that.linkAddSubtipoClassName}`, function(element){
            const $jqueryElement = $(element);
            $jqueryElement.off().on("click", function(){                                                                 
                that.handleAddSubtipo();             
            });	                        
        });

        // Botón para añadir una propiedad a la visualización del objeto de conocimiento
        configEventByClassName(`${that.linkAddPropertyClassName}`, function(element){
            const $jqueryElement = $(element);
            $jqueryElement.off().on("click", function(){                    
                that.panelConfigurarPropiedades = $(this).closest(".panelConfigurarPropiedades");                
                that.handleAddProperty(false);             
            });	                        
        });

        // Botón para añadir una propiedad personalizada a la visualización del objeto de conocimiento
        configEventByClassName(`${that.linkAddCustomPropertyClassName}`, function(element){
            const $jqueryElement = $(element);
            $jqueryElement.off().on("click", function(){                    
                that.panelConfigurarPropiedades = $(this).closest(".panelConfigurarPropiedades");                
                that.handleAddProperty(true);             
            });	                        
        });   
            
        // Input del nombre del Grafo
        configEventByClassName(`${that.inputGrafoObjetoConocimientoClassName}`, function(element){
            const $jqueryElement = $(element);
            $jqueryElement.on("keyup", function(e){                    
                const input = $(this);

                if (e.keyCode == 9){
                    // Accedido vía pulsación Tab
                    // Eliminar posible ".owl"
                    input.val(input.val().replace(".owl",""));
                    
                }else if (e.keyCode != 37 && e.keyCode != 38 && e.keyCode != 39 && e.keyCode != 40){
                    // Sólo caracteres minúsuculas y números
                    const formattedValue = input.val().replace(/[^a-zA-Z0-9]/g, '').trim().toLowerCase();             
                    input.val(formattedValue);
                }                            
            });	    
            
            $jqueryElement.on("focus", function(){                    
                const input = $(this);
                input.val(input.val().replace(".owl",""));                
            });            
            $jqueryElement.on("blur", function(){                    
                const input = $(this);
                const formattedValue = input.val();
                if (formattedValue.length > 0 ){
                    // Comprobar si dispone de ".owl". Si no dispone añadirlo
                    if (!formattedValue.includes(".owl")){
                        // Añadir owl ya que el usuario lo ha modificado
                        input.val(`${input.val()}.owl`);
                    }
                }
                // Comprobar que haya al menos un valor para grafo
                comprobarInputNoVacio(input, true, false, "El nombre del grafo del objeto de conocimiento no puede estar vacío.", 0);
            });	                                    
        });        
        
        // Input de presentación de la propiedad para que no estén vacío
        configEventByClassName(`${that.inputPresentacionValueClassName}`, function(element){
            const $jqueryElement = $(element);
            $jqueryElement.off().on("keyup", function(){                    
                const input = $(this);
                comprobarInputNoVacio(input, true, false, "El valor de presentación de la propiedad no puede estar vacío.", 0);                
            });	                        
        });

        // Input de nombre de la propiedad para que no estén vacío
        configEventByClassName(`${that.inputNombrePropiedadValueClassName}`, function(element){
            const $jqueryElement = $(element);
            $jqueryElement.off().on("keyup", function(){                    
                const input = $(this);
                comprobarInputNoVacio(input, true, false, "El nombre de la propiedad no puede estar vacío.", 0);                
            });	                        
        });  
        
        // Botón para guardar el objeto de conocimiento
        configEventByClassName(`${that.btnSaveObjetoConocimientoClassName}`, function(element){
            const $jqueryElement = $(element);
            $jqueryElement.off().on("click", function(){
                // Fila actual del objeto de conocimiento seleccionado                
                that.filaObjetoConocimiento = $jqueryElement.closest(`.${that.objetoConocimientoListItemClassName}`);                                
                that.handleSaveObjetoConocimiento();
            });	                        
        });  

        // Botón para descargar clases y vistas
        this.btnDownloadClasesYVistas.off().on("click", function(){
            that.handleDownloadClasesYVistas(false);
        });

        // Botón para descargar clases y vistas Java
        this.btnDownloadClasesYVistasJava.off().on("click", function(){
            that.handleDownloadClasesYVistas(true);
        });    
    
        
        // Botón para eliminar un subtipo de un objeto de conocimiento
        configEventByClassName(`${that.btnDeleteSubtipoClassName}`, function(element){
            const $jqueryElement = $(element);
            $jqueryElement.off().on("click", function(){ 
                const btnDeleted = $(this);                                   
                that.handleDeleteSubtipo(btnDeleted);            
            });	                        
        });          

        // Botón para eliminar una propiedad de un objeto de conocimiento
        configEventByClassName(`${that.btnDeletePropertyClassName}`, function(element){
            const $jqueryElement = $(element);
            $jqueryElement.off().on("click", function(){ 
                const btnDeleted = $(this);                                   
                that.handleDeleteProperty(btnDeleted);            
            });	                        
        });  
        
        // Botón para añadir un nuevo objeto de conocimiento
        this.btnNewOC.on("click", function(){
            that.handleLoadAddNewObjetoConocimiento(); 
        });

        // Botón para eliminar un objeto de conocimiento
        configEventByClassName(`${that.btnDeleteObjetoConocimientoClassName}`, function(element){
            const $jqueryElement = $(element);
            $jqueryElement.off().on("click", function(){ 
                const btnDeleted = $(this);                                                   
                // Fila correspondiente al objeto a eliminar
                that.filaObjetoConocimiento = btnDeleted.closest(`.${that.objetoConocimientoListItemClassName}`);                
                // Marcar la página como "Eliminar" a modo informativo al usuario
                that.handleSetDeleteObjetoConocimiento(true);       
            });	                        
        }); 
        

        // Botón para confirmar la eliminación de un objeto de conocimiento desde el modal
        configEventByClassName(`${that.btnConfirmDeleteObjetoConocimientoClassName}`, function(element){
            const confirmRemoveObjetoConocimiento = $(element);
            confirmRemoveObjetoConocimiento.off().on("click", function(){   
                // Confirmamos la eliminación
                that.confirmDeleteObjetoConocimiento = true;
                // Procedemos al borrado/guardado
                that.handleConfirmDeleteObjetoConocimiento();
            });	                        
        }); 

        // Tab de cada uno de los idiomas disponibles para cambiar la visualización en la lista
        this.tabIdiomaItem.off().on("click", function(){
            that.handleViewTranslateLanguageInfo();                        
            // Aplicar la búsqueda para el idioma activo
            that.timer = setTimeout(function () {                                                                                                        
                that.handleSearchObjetoConocimientoItem(that.txtBuscarObjetoConocimiento);
            }, 500);            
        });        

        // Búsquedas de objetos de conocimiento
        this.txtBuscarObjetoConocimiento.off().on("keyup", function (event) {
            const input = $(this);
            clearTimeout(that.timer);
            that.timer = setTimeout(function () {                                                                        
                // Acción de buscar / filtrar
                that.handleSearchObjetoConocimientoItem(input);                                         
            }, 500);
        }); 

        // RadioButton de Objeto Primario o secundario del modal al crear un objeto de conocimiento
        configEventByClassName(`${that.crearObjetoConocimientoName}`, function(element){
            const radioButton = $(element);
            radioButton.off().on("click", function(){   
                that.handleChangeObjetoConocimientoType();
            });	                        
        });        

        // Botón guardar en primera instancia en el momento de crear un nuevo objeto de conocimiento
        configEventByClassName(`${that.btnSaveNewObjetoConocimientoClassName}`, function(element){
            const btnSave = $(element);
            btnSave.off().on("click", function(){   
                that.handleGetTemplateFilesForObjetoConocimiento(true);
            });	                        
        });

        // Botón para mostrar u ocultar una sección de ficheros del objeto de conocimiento a editar
        configEventByClassName(`${that.btnCancelObjetoConocimientoClassName}`, function(element){
            const btn = $(element);
            btn.off().on("click", function(){   
                that.handleHideShowPanelToEditObjetoConocimiento(btn);
            });	                        
        });
        
        // Checkbox de utilizar archivo genérico para el fichero
        configEventByClassName(`${that.chkUtilizarArchivoGenericoClassName}`, function(element){
            const checkbox = $(element);
            checkbox.off().on("change", function(){   
                that.handleChangeGenericFileUsage(checkbox);                
            });	                        
        });
        
        // Input para buscar elementos secundarios de un objeto de conocimiento secundario
        configEventByClassName(`${that.findSecundaryOntologyElementsClassName}`, function(element){        
            const input = $(element);
            input.off().on("input", function(){                   
                clearTimeout(that.timer);
                that.timer = setTimeout(function () {                                                                        
                    // Acción de buscar / filtrar
                    that.handleSearchElementosObjetoConocimientoSecundario(input);                                                         
                }, 500); 
            });            
        });

        // Option del checkbox para mostrar sólo los elementos que no se han guardado o que están editados
        configEventByClassName(`${that.checkboxShowUnsavedItemsClassName}`, function(element){        
            const input = $(element);
            input.off().on("change", function(){                   
                const inputSearch = that.filaObjetoConocimiento.find(`.${that.findSecundaryOntologyElementsClassName}`);
                const showUnsavedItemsOnly = input.prop('checked');                
                that.handleSearchElementosObjetoConocimientoSecundario(inputSearch, showUnsavedItemsOnly);
            });            
        });
                          
        // Botón para editar/mostrar los datos de un elemento/entidad de un objeto de conocimiento secundario
        configEventByClassName(`${that.btnEditElementClassName}`, function(element){
            const editButton = $(element);
            editButton.off().on("click", function(){                  
                // Guardamos referencia de la fila del elemento del objeto de conocimiento secundario a editar/crear
                that.filaElementoObjetoConocimientoSecundario = $(this).closest(`.${that.elementRowClassName}`);                                            
                that.handleGetElementInfoOrCreateNewElementInfo(that.filaElementoObjetoConocimientoSecundario);                                
            });	                        
        });

        // Botón de "Añadir elemento" para añadir un elemento nuevo a un objeto de conocimiento secundario        
        configEventByClassName(`${that.btnAddSecondaryElementClassName}`, function(element){
            const addButton = $(element);
            addButton.off().on("click", function(){                                                            
                // Solicitar vista para crear un nuevo elemento/instancia
                that.handleLoadNewViewForSecondaryElementInstance();
            });	                        
        });
        
        // Inputs que haya dentro del panel -> Permitir guardado
        configEventByClassName(`${that.propertyItemClassName}`, function(element){            
            const propertyPanel = $(element);
            // Panel que contiene todo el panelCollapse de la propiedad
            const pPanelDetail = propertyPanel.closest(`.${that.panelElementDetailClassName}`);
            // Fila elemento que está siendo editada. Contenedor de todo
            const pElementRow = propertyPanel.closest(`.${that.elementRowClassName}`); 
            const pElementInput = propertyPanel.find("input:text");
            // Input change del tipo "Element"
            pElementInput.on("input", function(){     
                // Habilitar el botón Guardar Elemento ya que se desea "Guardar"
                // const btnAplicar = pPanelDetail.find(`.${that.btnSaveInstanciaEntidadClassName}`);
                // btnAplicar.prop('disabled', false);                                  
                // Detectar la fila del elemento editado
                that.filaElementoObjetoConocimientoSecundario = $(this).closest(`.${that.elementRowClassName}`);
                // Detectar el input del sujeto
                const inputEntidadSujeto = that.filaElementoObjetoConocimientoSecundario.find(`.${that.sujEntidadSecClassName}`);
                const containsWhiteSpace = that.handleCheckContainsWhiteSpace(inputEntidadSujeto);                
                that.handleEnableOrDisableApplyButton(!containsWhiteSpace);
                // Mostrar información de que se está editando
                const labelElementExtraInfo = pElementRow.find(`.${that.labelElementExtraInfoClassName}`);
                labelElementExtraInfo.removeClass("d-none");                 
                // Detectar cuál será el primer input que determinará el título del panel
                const firstInput = pPanelDetail.find("input[type=text]").first();
                if ($(this)[0] == $(firstInput)[0]){
                    // Actualizar el nombre en la fila
                    const labelElementName = pElementRow.find(`.${that.labelElementNameClassName}`);
                    labelElementName.html($(this).val());           
                }            
            });	 
        });
        
        // Botón de "Añadir elemento" para añadir un elemento nuevo a un objeto de conocimiento secundario        
        configEventByClassName(`${that.tabObjetoConocimientoNavigationClassName}`, function(element){
            const tabButton = $(element);
            tabButton.off().on("click", function(){                                                            
                // Mostrar u ocultar el botón de guardar general el objeto de conocimiento dependiendo en el tab pulsado (Ocultarlo solo en Elementos de OC secundarios)        
                const pHideSaveButton = tabButton.data("hide-general-save") == true;
                that.handleShowHideGeneralSaveButton(pHideSaveButton);
            });	                        
        }); 
        
        // Input del sujeto de la entidad       
        configEventByClassName(`${that.sujEntidadSecClassName}`, function(element){
            const input = $(element);
            input.off().on("blur", function(){    
                // Detectar la fila del elemento editado
                that.filaElementoObjetoConocimientoSecundario = input.closest(`.${that.elementRowClassName}`);
                // Si es de reciente creación es posible que esté vacío                                
                comprobarInputNoVacio(input, true, false, "El identificador del elemento no puede estar vacío.", 0);
                const containsWhiteSpace = that.handleCheckContainsWhiteSpace(input);                
                that.handleEnableOrDisableApplyButton(!containsWhiteSpace);
            }).on("keyup", function(){    
                // No permitir espacios en blanco
                this.value = this.value.replace(/[^\S*$/]/, '').trim();
                const containsWhiteSpace = that.handleCheckContainsWhiteSpace(input);
                that.handleEnableOrDisableApplyButton(!containsWhiteSpace);
            });	  	                        
        });

        // Botón de "Aplicar/Guardar" un elemento de un objeto de conocimiento de tipo secundario
        configEventByClassName(`${that.btnSaveInstanciaEntidadClassName}`, function(element){
            const button = $(element);
            button.off().on("click", function(){                                                            
                // Fila elemento que está siendo editada. Contenedor de todo
                const pElementRow = button.closest(`.${that.elementRowClassName}`);
                that.GuardarEntSec(pElementRow);
            });	                        
        });

        // Botón de eliminar un elemento de un objeto de conocimiento de tipo secundario
        configEventByClassName(`${that.btnDeleteElementClassName}`, function(element){
            const deleteButton = $(element);
            if (deleteButton.length > 0) {
                // Fila elemento que está siendo editada. Contenedor de todo
                const pElementRow = deleteButton.closest(`.${that.elementRowClassName}`);
                // Pasar la función como parámetro al plugin
                const pluginOptions = {
                    onConfirmDeleteMessage: () => that.handleDeleteSecondaryElement(pElementRow)
                }               
                deleteButton.confirmDeleteItemInModal(pluginOptions);
            }                         
        });               
        
        // Tab para cargar los posibles historicos de ficheros de una ontología        
        configEventByClassName(`${that.tabConfiguracionHistoricoFicherosObjetoConocimientoClassName }`, function(element){
            const tabButton = $(element);
            tabButton.off().on("click", function(){                    
                // Guardamos referencia de la fila del objeto de conocimiento a editar/crear
                that.filaObjetoConocimiento = $(this).closest(`.${that.objetoConocimientoListItemClassName}`);                                                                                    
                that.handleSetTabActiveHistoryFiles(that.filaObjetoConocimiento);   
            });	                                                   
        });  
        
        // Input para buscar elementos de ficheros históricos de un objeto de conocimiento
        configEventByClassName(`${that.findHistoryFileElementsClassName}`, function(element){        
            const input = $(element);
            input.off().on("input", function(){                   
                clearTimeout(that.timer);
                that.timer = setTimeout(function () {                                                                        
                    // Acción de buscar / filtrar
                    that.handleSearchElementosHistoricoObjetoConocimiento(input);                                                         
                }, 500); 
            });            
        }); 
        
        // Botón para eliminar un grupo de elementos del objeto de conocimiento de tipo secundario. Se aplicará el plugin para el comportamiento 
        // extraido de la propiedad data-handleclick del botón        
        configEventByClassName(`${that.btnDeleteGroupValueClassName}`, function(element){        
            const btnDeleteGroup = $(element);                 
            // Obtener la acción de eliminar para aplicarla al comportamiento del botón y asociarla al plugin
            if (btnDeleteGroup.length > 0) {
                // Fila elemento que está siendo editada. Contenedor de todo
                const pElementRow = btnDeleteGroup.closest(`.${that.elementRowClassName}`);
                const deleteFunction = btnDeleteGroup.data("handleclick");
                // Pasar la función como parámetro al plugin
                const pluginOptions = {                    
                    onConfirmDeleteMessage: () => eval(deleteFunction)
                }               
                btnDeleteGroup.confirmDeleteItemInModal(pluginOptions);
            }                                                                                             
        });  
        
        // Botón para eliminar un grupo de elementos del objeto de conocimiento de tipo secundario. Se aplicará el plugin para el comportamiento 
        // extraido de la propiedad data-handleclick del botón           
        configEventByClassName(`${that.btnDeleteObjectNoFuncionalPropClassName}`, function(element){        
            const btnDeleteObjectNoFuncionalProp = $(element);                 
            // Obtener la acción de eliminar para aplicarla al comportamiento del botón y asociarla al plugin
            if (btnDeleteObjectNoFuncionalProp.length > 0) {
                // Fila elemento que está siendo editada. Contenedor de todo
                const pElementRow = btnDeleteObjectNoFuncionalProp.closest(`.${that.elementRowClassName}`);
                const deleteFunction = btnDeleteObjectNoFuncionalProp.data("handleclick");
                // Pasar la función como parámetro al plugin
                const pluginOptions = {                    
                    onConfirmDeleteMessage: () => eval(deleteFunction)
                }               
                btnDeleteObjectNoFuncionalProp.confirmDeleteItemInModal(pluginOptions);
            }                                                                                             
        });
    },

    /**
     * Método para eliminar el objeto no funcional de una propiedad. Se ejecutará al hacer click en la papelera en cada lista dentro del modal.
     */
    EliminarObjectNoFuncionalProp: function(pNumElem, pEntidad, pPropiedad, pEntidadHija, pControlContValores, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados){
        const that = this;
        that.TxtHackHayCambios = true;
       
        var entidadBorrar = pEntidad;
        if (that.PerteneceEntidadAAlgunGrupoPanelSinEditar(pEntidad, pTxtCaract, pTxtElemEditados))
        {
            entidadBorrar += '&ULT';
        }
    
        that.DeleteElementoGuardado(entidadBorrar, pPropiedad, pTxtValores, pTxtElemEditados, pNumElem);
        if (pControlContValores != '')
        {            
            that.EliminarEntidadDeContenedorGrupoPaneles(pControlContValores, pEntidad, pPropiedad, pNumElem, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados);
        }

        that.MarcarElementoEditado(pEntidad, pPropiedad, -1, pTxtElemEditados, pTxtCaract);
        that.LimpiarControlesEntidad(pEntidadHija, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados);
        
        that.EstablecerBotonesEntidad(pEntidad, pPropiedad, true, pTxtIDs);
        
        that.BorrarDocRecExtSelecEntEditable(pEntidad, pPropiedad, pNumElem);               
        that.EliminarIDEntidadAuxiliar(pNumElem, pEntidad, pPropiedad, pEntidadHija);

        // Habilitar el botón de guardado
        that.handleEnableOrDisableApplyButton(true);        
    },

    GuardarObjectNoFuncionalProp: function(pEntidad, pPropiedad, pEntidadHija, pControlContValores, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados){
        const that = this;
                
        var rdfBK = isJqueryObject(pTxtValores) ? pTxtValores.val() : document.getElementById(pTxtValores).value;
        var entidadPadre = pEntidadHija;
        var pEntidadHija = that.ObtenerTipoEntidadEditada(pEntidadHija);
        var contendedorEntBK = null;
        var mostrarContenedorAlAgregar = false;
        if (pControlContValores != '')
        {
            contendedorEntBK = document.getElementById(pControlContValores).innerHTML;
            if (document.getElementById(pControlContValores).style.display != 'none')
            {
                mostrarContenedorAlAgregar = true;
            }
        }
        var numElem = that.GetNumEdicionEntProp(pEntidad, pPropiedad, pTxtElemEditados);
        
        that.SalvarPropiedadesNoFuncionalesEntidad(pEntidadHija, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados);
        
        var entidadBorrar = pEntidad;
        if (that.PerteneceEntidadAAlgunGrupoPanelSinEditar(pEntidad, pTxtCaract, pTxtElemEditados))
        {
            entidadBorrar += '&ULT';
        }
        that.DeleteElementoGuardado(entidadBorrar, pPropiedad, pTxtValores, pTxtElemEditados, numElem);
        
        if (pControlContValores != '')
        {
            that.EliminarEntidadDeContenedorGrupoPaneles(pControlContValores, pEntidad, pPropiedad, numElem, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados);
        }
        that.MarcarElementoEditado(pEntidad, pPropiedad, -1, pTxtElemEditados, pTxtCaract);
        
        var camposCorrectos = that.AgregarObjectNoFuncionalProp(pEntidad, pPropiedad, pEntidadHija, pControlContValores, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados, mostrarContenedorAlAgregar, false, numElem);
    
        that.DeleteElementoGuardado(null, pEntidadHija + '&ULT', pTxtValores, pTxtElemEditados, 0);
        
        if (!camposCorrectos)
        {
            //Recuperamos el BK:
            // document.getElementById(pTxtValores).value = rdfBK;
            isJqueryObject(pTxtValores) ? pTxtValores.val(rdfBK) : document.getElementById(pTxtValores).value = rdfBK;
            
            if (contendedorEntBK != null)
            {
                document.getElementById(pControlContValores).innerHTML = contendedorEntBK;
                $(document.getElementById(pControlContValores)).show();
            }
            
            MarcarElementoEditado(pEntidad, pPropiedad, numElem, pTxtElemEditados, pTxtCaract);
        }
        else
        {
            //Movemos la entidad que está la última a su sitio original:            
            that.MoverElementoGuardado(entidadBorrar, pPropiedad, pTxtValores, pTxtElemEditados, -1, numElem)
            if (pControlContValores != '')
            {                
                that.MoverEntidadEnContenedorGrupoPaneles(pControlContValores, pEntidad, pPropiedad, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados, -1, numElem);
            }
            
            that.LimpiarControlesEntidad(pEntidadHija, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados);
            that.EstablecerBotonesEntidad(pEntidad, pPropiedad, true, pTxtIDs);
            
            if (that.ExcedeCardinalidadMaxima(pEntidad, pPropiedad, pTxtValores, pTxtCaract, pTxtElemEditados))
            {                
                var idControlCampo = that.ObtenerControlEntidadProp(pEntidad + ',' + pPropiedad, pTxtIDs);
                document.getElementById(idControlCampo.replace('panel_contenedor_Entidades_','lbCrear_')).style.display = 'none';
            }
    
            if (entidadPadre != pEntidadHija) {
                //Lipio otras hijas:
                var subclases = that.GetSubClasesEntidad(entidadPadre);
    
                for (var i = 0; i < subclases.length; i++) {
                    if (subclases[i] != '' && subclases[i] != pEntidadHija) {
                        that.LimpiarControlesEntidad(subclases[i], pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados);
                    }
                }
    
                var combosSubClase = $('select.cmbSubClass_' + that.ObtenerTextoGeneracionIDs(entidadPadre));
    
                if (combosSubClase.length > 0) {
                    combosSubClase[0].style.display = '';
                }
            }
        }        
        return camposCorrectos;        
    },

    MoverEntidadEnContenedorGrupoPaneles: function(pControlCont, pEntidad, pPropiedad, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados, pNumOrigen, pNumDestino){
        const that = this;        

        if (pNumOrigen == -1)
        {
            // pNumOrigen = document.getElementById(pControlCont).children[0].children[0].children.length - 2;            
            pNumOrigen = document.getElementById(pControlCont).children[0].children.length - 1;
        }
        
        // Obtener la fila que ha sido editada
        //var valorOrgigen = document.getElementById(pControlCont).children[0].children[0].children[pNumOrigen + 1].innerHTML;
        var valorOrgigen = document.getElementById(pControlCont).children[0].children[pNumOrigen].innerHTML;
        // valorOrgigen = valorOrgigen.replace(', \''+(pNumOrigen),', \'' + (pNumDestino)).replace('(\\\''+(pNumOrigen),'(\\\''+(pNumDestino))
        
        // Obtener todos los items existentes en el contenedor                
        let hijos = document.getElementById(pControlCont).children[0].children;
        
        let htmlFinal = '';
        
        
        for (var i=0; i<hijos.length; i++)
        {
            // Recoger todos los elementos
            if (i == pNumDestino)
            {                              
                // var clase = hijos[i].className;
                if (i != pNumOrigen) //Invertimos clase
                {                
                    // Ordenar correctamente indicando el parámetro y restándolo según corresponda para la función EliminarObjectNoFuncionalProp                     
                    const hijoHtmlContent = hijos[i].outerHTML.replace(/'\d+'/g, function(match) {                        
                        let newPositionForJsFunction = parseInt(match.replace(/'/g, '')) - 1;
                        return "'" + newPositionForJsFunction + "'";
                    });                    
                    htmlFinal += hijoHtmlContent; 
                }
                else
                {
                    htmlFinal += hijos[i].outerHTML;
                }
            }else{
                htmlFinal += hijos[i].outerHTML;
            }
        }
                
        // Sustituir los items al contenedor no añadiendo el que se desea eliminar
        let contenedorHTML = document.getElementById(pControlCont).children[0].innerHTML;
        contenedorHTML = contenedorHTML.replace(document.getElementById(pControlCont).children[0].innerHTML, htmlFinal);
        document.getElementById(pControlCont).children[0].innerHTML = contenedorHTML;         
        
        /*
        var hijos = document.getElementById(pControlCont).children[0].children;
        var htmlFinal = '<tr>' + document.getElementById(pControlCont).children[0].children[0].children[0].innerHTML + '</tr>';
        
        var numOrigen = pNumOrigen + 1;
        var numDestino = pNumDestino + 1;
        var clase = '';
        
        for (var i=1;i<hijos.length;i++)
        {
            if (i == numDestino)
            {
                clase = 'par';
                
                if(i %2 ==0)
                {
                    clase = 'impar';
                }
                
                htmlFinal += '<tr class="'+clase+'">' + valorOrgigen + '</tr>';
            }
            
            if (i != numOrigen)
            {
                clase = hijos[i].className;
                
                if (i>=numDestino) //Invertimos clase
                {
                    if (clase == 'par')
                    {
                        clase = 'impar';
                    }
                    else
                    {
                        clase = 'par';
                    }
                    
                    htmlFinal += '<tr class="'+clase+'">' + hijos[i].innerHTML.replace(', \''+(i - 1),', \'' + (i)).replace('(\\\''+(i - 1),'(\\\''+(i)) + '</tr>';
                }
                else
                {
                    htmlFinal += '<tr class="'+clase+'">' + hijos[i].innerHTML + '</tr>';
                }
            }
        }
        
        var contenedorHTML = document.getElementById(pControlCont).innerHTML;
        contenedorHTML=contenedorHTML.replace(document.getElementById(pControlCont).children[0].children[0].innerHTML,htmlFinal);       
        document.getElementById(pControlCont).innerHTML=contenedorHTML;
        */

    },

    MoverElementoGuardado: function(pPadre, pElemento, pTxtValores, pTxtElemEditados, pNumOrigen, pNumDestino){
        const that = this;

        var valorRdf = isJqueryObject(pTxtValores) ? pTxtValores.val() : document.getElementById(pTxtValores).value;
        var indicesIniFinPadre = that.GetIndiceInicioFinElementoEditado(pPadre, valorRdf, pTxtElemEditados);
        var inicio = parseInt(indicesIniFinPadre.split(',')[0]);
        var fin = parseInt(indicesIniFinPadre.split(',')[1]);
        
        var cierrePadre = '</' + pPadre + '>';
        var elemElemento = '<' + pElemento + '>';
        var cierreElemento = '</' + pElemento + '>';
        
        var trozo1 = valorRdf.substring(0, inicio);
        var trozo2 = valorRdf.substring(inicio, (fin + cierrePadre.length));
        var trozo3 = valorRdf.substring(fin + cierrePadre.length);
    
        var elemento = that.ObtenerElementoXMLNodo(trozo2, pElemento, pNumOrigen);
        trozo2 = that.BorrarElementoDeXml(trozo2, pElemento, pNumOrigen);
        trozo2 = that.AgregarElementoAXml(trozo2, elemento, pNumDestino);
    
        document.getElementById(pTxtValores).value = trozo1 + trozo2 + trozo3;
    },

    SalvarPropiedadesNoFuncionalesEntidad: function(pEntidad, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados){
        const that = this;

        that.DeleteElementoGuardado(null, pEntidad + '&ULT', pTxtValores, pTxtElemEditados, 0);
        var propiedades = that.GetPropiedadesEntidad(pEntidad, pTxtCaract);
        
        for (var i=0;i<propiedades.length;i++)
        {
            if (propiedades[i] != '')
            {
                //var tipoProp = GetTipoPropiedad(pEntidad, propiedades[i], pTxtCaract);
                //if (tipoProp == 'LD' || tipoProp == 'LO' || tipoProp == 'LSEO')//&ULT
                //{
                    //if (i==0)
                    //{
                        //DeleteElementoGuardado(null, pEntidad + '&ULT', pTxtValores, pTxtElemEditados, 0);
                    //}
                    
                    var valorProp = that.GetValorElementoGuardado(pEntidad, propiedades[i], pTxtValores, pTxtElemEditados, 0);
                    if (valorProp != '' && valorProp != null)
                    {
                        var j = 1;
                        while (valorProp != '')
                        {
                            that.PutElementoGuardado(pEntidad + '&ULT', propiedades[i], valorProp, pTxtValores, pTxtElemEditados);
                            valorProp = that.GetValorElementoGuardado(pEntidad, propiedades[i], pTxtValores, pTxtElemEditados, j);
                            j++;
                        }
                    }
                //}
            }
        }        
    },

    /**
     * Método para editar el objeto no funcional de una propiedad. Se ejecutará al hacer click en la papelera en cada lista dentro del modal.
     */
    SeleccionarElementoGrupoPaneles: function(pEntidad, pPropiedad, pEntidadHija, pNumElem, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados){
        const that = this;
        
        that.TxtHackHayCambios = true;
        that.DeleteElementoGuardado(null, pEntidadHija + '&ULT', pTxtValores, pTxtElemEditados, 0);
        that.MarcarElementoEditado(pEntidad, pPropiedad, pNumElem, pTxtElemEditados, pTxtCaract);

        that.DarValorAPropiedadesDeEntidad(pEntidadHija, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados);
    
        that.EstablecerBotonesEntidad(pEntidad, pPropiedad, false, pTxtIDs);
    
        var superClases = that.GetSuperClasesEntidad(pEntidadHija);
    
        if (superClases != null) {
            var combosSubClase = $('select.cmbSubClass_' + that.ObtenerTextoGeneracionIDs(superClases[0]));
    
            if (combosSubClase.length > 0) {
                if (!that.EntidadSubClaseSeleccionada(pEntidadHija, true)) {
                    for (var i = 0; i < combosSubClase[0].options.length; i++) {
                        if (combosSubClase[0].options[i].value == pEntidadHija) {
                            combosSubClase[0].selectedIndex = i;
                            break;
                        }
                    }                    
                    that.AjustarHerederasEntidad(combosSubClase[0], superClases[0], false);
                }    
                combosSubClase[0].style.display = 'none';
            }
        }        
        
    },
    AjustarHerederasEntidad: function(pCombo, pClaseEnt, pSustituir){
        const that = this;

        var herederas = $('.' + 'SuperEnt_' + that.ObtenerTextoGeneracionIDs(pClaseEnt));
        var entidadAnterior = null;
        var entidadNueva = null;
    
        for (var i = 0; i < herederas.length; i++) {
            var tipoEntidad = $(herederas[i]).attr('typeEnt');
    
            if ($(herederas[i]).css('display') == '' || $(herederas[i]).css('display') == 'block') {
                entidadAnterior = tipoEntidad;
            }
    
            if (tipoEntidad != pCombo.value) {
                $(herederas[i]).css('display', 'none');
            }
            else {
                $(herederas[i]).css('display', '');
                entidadNueva = tipoEntidad;
            }
        }
    
        //Todo: Ajustar valor RDF para que desaparezca la entidad anterior y sus propiedades y se ponga la nueva 
        //solo si hay que hacerlo, si la propiedad no es funcional no.
    
        //Si estamos con una propiedad Object habrá que reiniciar todo lo que esté por abajo de la propiedad
        if (pSustituir && !that.PerteneceEntidadAAlgunGrupoPanelSinEditar(pClaseEnt, TxtCaracteristicasElem, TxtElemEditados)) {            
            that.SustituirRdfEntidadHeredada(pClaseEnt, entidadAnterior, entidadNueva);
        }        
    },

    SustituirRdfEntidadHeredada: function(pClaseEnt, tipoEntAnt, tipoEntNueva){
        const that = this;
        
        var rdfEntHer = that.ObtenerParteRDFEntidadHijaDeHerencia(pClaseEnt, tipoEntNueva);

        if (tipoEntAnt == null) {
            tipoEntAnt = pClaseEnt;
        }
    
        var rdf = document.getElementById(TxtValorRdf).value;
        var indiceEntidad = rdf.indexOf('<' + tipoEntAnt + '>');
    
        if (indiceEntidad != -1) {
            var trozo1 = rdf.substring(0, indiceEntidad);
            var trozo2 = rdf.substring(rdf.indexOf('</' + tipoEntAnt + '>') + tipoEntAnt.length + 3);
            document.getElementById(TxtValorRdf).value = trozo1 + rdfEntHer + trozo2;
        }
    
        that.LimpiarControlesEntidad(tipoEntAnt, TxtValorRdf, TxtRegistroIDs, TxtCaracteristicasElem, TxtElemEditados);
    },

    ObtenerParteRDFEntidadHijaDeHerencia: function(pTipoSuperClase, pTipoEntidadHija){
        const that = this;

        if(pTipoEntidadHija == null){
            return "";
        }
        var rdfHerencia = document.getElementById(TxtValorRdfHerencias).value;
        rdfHerencia = rdfHerencia.substring(rdfHerencia.indexOf('<||>' + pTipoSuperClase));
        rdfHerencia = rdfHerencia.substring(rdfHerencia.indexOf('<|>' + pTipoEntidadHija + ',') + pTipoEntidadHija.length + 4);
        rdfHerencia = rdfHerencia.substring(0, rdfHerencia.indexOf('<|>'));
    
        return rdfHerencia;        
    },

    DarValorAPropiedadesDeEntidad: function(pEntidad, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados){
        const that = this;

        const propiedades = that.GetPropiedadesEntidad(pEntidad, pTxtCaract);
    
        for (let i=0;i<propiedades.length;i++)
        {
            if (propiedades[i] != '')
            {                
                that.DarValorAPropiedadDeEntidad(pEntidad, propiedades[i], pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados);
            }
        }        
    },

    DarValorAPropiedadDeEntidad: function(pEntidad, pPropiedad, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados){
        const that = this;

        //TODO Comprobar corrección.
        var tipoProp = that.GetTipoPropiedad(pEntidad, pPropiedad, pTxtCaract);
        var camposCorrectos = true;

        if (tipoProp == 'FD' || tipoProp == 'CD' || tipoProp == 'FSEO' || tipoProp == 'CSEO')
        {
            var valorProp = that.GetValorElementoEditadoGuardado(pEntidad, pPropiedad, pTxtValores, pTxtElemEditados);
            valorProp = that.GetValorDecode(valorProp);
            
            if (that.EsPropiedadMultiIdioma(pEntidad, pPropiedad)) {
                var idControlCampoPes = that.ObtenerControlEntidadProp(pEntidad + ',' + pPropiedad, TxtRegistroIDs).replace('Campo_', 'divContPesIdioma_');

                if (document.getElementById(idControlCampoPes) != null) {
                    var li = $('li.active', $('#' + idControlCampoPes));
                    var idiomaActual = li.attr('rel');
                    $('#' + idControlCampoPes).attr('langActual', valorProp);
                    valorProp = ExtraerTextoIdioma(valorProp, idiomaActual);
                }
                else { //Es multiIdioma sin pestaña
                    var idiomas = IdiomasConfigFormSem.split(',');
                    for (var i = 0; i < idiomas.length; i++) {
                        if (idiomas[i] != IdiomaDefectoFormSem) {
                            that.DarValorControlConIdioma(pEntidad + ',' + pPropiedad, pTxtIDs, pTxtCaract, that.ExtraerTextoIdioma(valorProp, idiomas[i]), idiomas[i]);
                        }
                    }

                    valorProp = that.ExtraerTextoIdioma(valorProp, IdiomaDefectoFormSem);
                }
            }

            that.DarValorControl(pEntidad + ',' + pPropiedad, pTxtIDs, pTxtCaract, valorProp);

            var idControl = that.ObtenerControlEntidadProp(pEntidad + ',' + pPropiedad, pTxtIDs);

            if (idControl.indexOf('selEnt_') != -1) {
                var inputHack = $('#' + idControl.replace('selEnt_', 'hack_'));
                if (inputHack.length > 0 && inputHack.hasClass("autocompletarSelecEnt")) {
                    inputHack.prop("disabled", true);
                    var contenedor = $('#' + idControl).closest('div.cont');
                    var aspa = $('a.removeAutocompletar', contenedor);
                    if (aspa.length == 0) {
                        contenedor.append('<a class="remove removeAutocompletar"></a>');
                        $('a.removeAutocompletar', contenedor).click(function () {
                            inputHack.val('');
                            inputHack.prop("disabled", false);
                            $(this).remove();
                        });
                    }
                }
                if (document.getElementById(idControl.replace('selEnt_', 'bkHtmlSelEnt_')) != null) {                    
                    that.MontarContenedorGrupoValores(pEntidad, pPropiedad, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados);
                }
            }
        }
        else if (tipoProp == 'VD')
        {
            var valorProp = that.GetValorElementoGuardado(pEntidad, pPropiedad, pTxtValores, pTxtElemEditados, 0);
            var valorPropFinal = '';
            var i = 1;
            while (valorProp != "")
            {
                valorProp = that.GetValorDecode(valorProp);
                valorPropFinal += valorProp + ', ';
                valorProp = that.GetValorElementoGuardado(pEntidad, pPropiedad, pTxtValores, pTxtElemEditados, i);
                i++;
            }
            
            if (valorPropFinal != '')
            {
                valorPropFinal = valorPropFinal.substring(0, valorPropFinal.length - 2);
            }
            
            that.DarValorControl(pEntidad + ',' + pPropiedad, pTxtIDs, pTxtCaract, valorPropFinal);
        }
        else if (tipoProp == 'LD' || tipoProp == 'LSEO')
        {
            that.MontarContenedorGrupoValores(pEntidad, pPropiedad, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados);
            that.DarValorControl(pEntidad + ',' + pPropiedad, pTxtIDs, pTxtCaract, '');
        }
        else if (tipoProp == 'FO' || tipoProp == 'CO')
        {
            var entidadHija = that.GetRangoPropiedad(pEntidad, pPropiedad, pTxtCaract);
            that.DarValorAPropiedadesDeEntidad(entidadHija, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados);
        }
        else if (tipoProp == 'LO')
        {            
            that.MontarContenedorGrupoPaneles(pEntidad, pPropiedad, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados);
            that.LimpiarControlesEntidad(that.GetRangoPropiedad(pEntidad, pPropiedad, pTxtCaract), pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados);
        }
    },

    MontarContenedorGrupoPaneles: function(pEntidad, pPropiedad, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados){
        const that = this;

        var entidadHija = that.GetRangoPropiedad(pEntidad, pPropiedad, pTxtCaract);
        var valorProp = that.GetValorElementoGuardado(pEntidad, pPropiedad, pTxtValores, pTxtElemEditados, 0);
        var idContGrupoPaneles = that.ObtenerControlEntidadProp(pEntidad + ',' + pPropiedad, pTxtIDs);
        that.LimpiarContenedorGrupoPaneles(pEntidad, pPropiedad, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados);
        
        var mostrarContenedorAlAgregar = false;
        
        try
        {
            var botonCrear = document.getElementById(idContGrupoPaneles.replace('panel_contenedor_Entidades_','lbCrear_'));
            if (botonCrear != null)
            {
                var valorBoton = botonCrear.attributes["onclick"].value;
                
                if (valorBoton.split(',')[9] == "true")
                {
                    mostrarContenedorAlAgregar = true;
                }
            }
        }
        catch(ex){}
        
        var i = 1;
        while (valorProp != "")
        {
            if (valorProp != null && valorProp != '') {//Por si hay herencia y el rango no es el tipo de entidad correcto
                entidadHija = that.ObtenerTipoEntidadDeValorRDF(valorProp);
            }
    
            that.AgregarEntidadAContenedorGrupoPaneles(idContGrupoPaneles, pEntidad, pPropiedad, entidadHija, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados, mostrarContenedorAlAgregar, false);
            valorProp = that.GetValorElementoGuardado(pEntidad, pPropiedad, pTxtValores, pTxtElemEditados, i);
            i++;
        }        
    },

    MontarContenedorGrupoValores: function(pEntidad, pPropiedad, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados){
        const that = this;

        var valorProp = that.GetValorElementoGuardado(pEntidad, pPropiedad, pTxtValores, pTxtElemEditados, 0);
        var idControl = that.ObtenerControlEntidadProp(pEntidad + ',' + pPropiedad, pTxtIDs);
        var idContGrupoValores = idControl.replace('Campo_', 'contedor_Valores_').replace('selEnt_', 'contEntSelec_');
    
        if (idContGrupoValores != '' && document.getElementById(idContGrupoValores) != null) {
            document.getElementById(idContGrupoValores).innerHTML = '';
            var i = 1;
            while (valorProp != "") {
                if (idControl.indexOf('selEnt_') != -1 && valorProp != '') {
                    if (document.getElementById(idControl.replace('selEnt_', 'hack_')) != null) {
                        var textoEntidad = that.ObtenerTextoRepEntidadExterna(valorProp);
                        if (textoEntidad != '') {
                            valorProp = textoEntidad;
                        }
                    }
                    else if ($('#' + idControl)[0].nodeName == 'SELECT') {
                        var url = valorProp;
    
                        if (url.indexOf('/') != -1) {
                            url = url.substring(url.lastIndexOf('/') + 1);
                        }
    
                        var opciones = $('option[value$=' + url + ']', $('#' + idControl)[0]);
    
                        if (opciones.length > 0) {
                            valorProp = $(opciones[0]).text();
                        }
                    }
                }
    
                valorProp = that.GetValorDecode(valorProp);
                that.AgregarValorAContenedorGrupoValores(idContGrupoValores, valorProp, pEntidad, pPropiedad, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados);
                valorProp = that.GetValorElementoGuardado(pEntidad, pPropiedad, pTxtValores, pTxtElemEditados, i);
                i++;
            }
    
            if (idContGrupoValores.indexOf('contEntSelec_') != -1 && document.getElementById(idContGrupoValores.replace('contEntSelec_', 'bkHtmlSelEnt_')) != null) {
                $('#' + idContGrupoValores.replace('contEntSelec_', 'divContControlTes_')).css('display', 'none');
                $('#' + idContGrupoValores).css('display', '');
            }
        }        
    },

    EliminarIDEntidadAuxiliar: function(pNumElem, pEntidad, pPropiedad, pEntidadHija){
        const that = this;
        //that.DeleteElementoGuardado(pEntidad, pPropiedad, 'txtEntidadesOntoIDs', TxtElemEditados, pNumElem);

        const txtElemEditados = that.txtElemEditados == undefined ? "mTxtElemEditados" : that.txtElemEditados;

        that.DeleteElementoGuardado(pEntidad, pPropiedad, 'txtEntidadesOntoIDs', txtElemEditados, pNumElem);
    },
 
    BorrarDocRecExtSelecEntEditable: function(pEntidad, pPropiedad, pNumElem){
        const that = this;
        var docExtID = that.GetDocRecExtSelecEntEditable(pEntidad, pPropiedad, false, pNumElem);
        
        if (docExtID != null) {
            var subOntos = $('#txtSubOntologias').val().split('|||');
            var subOntosFinal = '';
        
            for (var i = 0; i < subOntos.length; i++) {
                if (subOntos[i].indexOf(pEntidad + ',' + pPropiedad + '|') == 0) {
                    var datosSub = subOntos[i].split('|');
                    var newSub = datosSub[0];
                
                    var count = 0;
                    for (var j = 1; j < datosSub.length; j++) {
                        var datosSubInt = datosSub[j].split(',');
                    
                        if (parseInt(datosSubInt[0]) != pNumElem) {
                            var num = '';
                        
                            if (datosSubInt[0] == '-1') {
                                num = '-1';
                            }
                            else {
                                num = count;
                            }
                        
                            newSub += '|' + num + ',' + datosSubInt[1] + ',' + datosSubInt[2];
                            count++;
                        }
                    }
                
                    subOntosFinal += newSub + '|||';
                }
                else if (subOntos[i] != '') {
                    subOntosFinal += subOntos[i] + '|||';
                }
            }
        
            $('#txtSubOntologias').val(subOntosFinal);
        }        
    },

    EliminarEntidadDeContenedorGrupoPaneles: function(pControlCont, pEntidad, pPropiedad, pNumElem, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados){ 
        const that = this;       
        const hijos = document.getElementById(pControlCont).children[0].children;
        if (hijos.length == 0)
        {
            return;
        }
        let htmlFinal = '';
        
        // Posición del elemento que se desea eliminar. Añadimos 1 porque en "AgregarEntidadAContenedorGrupoPaneles" se le añade uno para evitar el encabezado de una tabla (inexistente)
        const numElem = parseInt(pNumElem);
        
        for (var i=0; i<hijos.length; i++)
        {
            // Recoger todos los elementos que no se desean eliminar
            if (i != numElem)
            {                
                // var clase = hijos[i].className;
                if (i>numElem) //Invertimos clase
                {                
                    //htmlFinal += '<tr class="'+clase+'">' + hijos[i].innerHTML.replace(', \''+(i - 1),', \'' + (i-2)).replace('(\\\''+(i - 1),'(\\\''+(i-2)) + '</tr>';
                    // htmlFinal += hijos[i].innerHTML.replace(', \''+(i - 1),', \'' + (i-2)).replace('(\\\''+(i - 1),'(\\\''+(i-2));
                    // Ordenar correctamente indicando el parámetro y restándolo según corresponda para la función EliminarObjectNoFuncionalProp                     
                    const hijoHtmlContent = hijos[i].outerHTML.replace(/'\d+'/g, function(match) {                        
                        let newPositionForJsFunction = parseInt(match.replace(/'/g, '')) - 1;
                        return "'" + newPositionForJsFunction + "'";
                    });                    
                    htmlFinal += hijoHtmlContent; 
                }
                else
                {
                    htmlFinal += hijos[i].outerHTML;
                }
            }
        }
                
        // Sustituir los items al contenedor no añadiendo el que se desea eliminar
        let contenedorHTML = document.getElementById(pControlCont).children[0].innerHTML;
        contenedorHTML = contenedorHTML.replace(document.getElementById(pControlCont).children[0].innerHTML, htmlFinal);
        document.getElementById(pControlCont).children[0].innerHTML = contenedorHTML; 
        
    },

    SeleccionarElementoGrupoValores: function(pEntidad, pPropiedad, pNumElem, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados){
        const that = this;
        that.TxtHackHayCambios = true;

        var valorRdf = document.getElementById(pTxtValores).value;        
        that.MarcarElementoEditado(pEntidad, pPropiedad, pNumElem, pTxtElemEditados, pTxtCaract);
        valor = that.GetValorElementoGuardado(pEntidad, pPropiedad, pTxtValores, pTxtElemEditados, pNumElem);
        valor = that.GetValorDecode(valor);

        var idControlCampo = that.ObtenerControlEntidadProp(pEntidad.replace('&ULT', '') + ',' + pPropiedad, pTxtIDs);
        var onclickGuardar = $('#' + idControlCampo.replace('Campo_', 'lbGuardar_')).attr('onclick');

        if (onclickGuardar.indexOf('&ULT') != -1) {
            onclickGuardar = onclickGuardar.replace('&ULT', '');
            $('#' + idControlCampo.replace('Campo_', 'lbGuardar_')).attr('onclick', onclickGuardar);
        }
        
        if (pEntidad.indexOf('&ULT') != -1)
        {
            pEntidad = pEntidad.substring(0, pEntidad.indexOf('&ULT'));
            $('#' + idControlCampo.replace('Campo_', 'lbGuardar_')).attr('onclick', onclickGuardar.replace(pEntidad, pEntidad +'&ULT'));
        }

        if (that.EsPropiedadMultiIdioma(pEntidad, pPropiedad)) {
            var idControlCampoPes = that.ObtenerControlEntidadProp(pEntidad + ',' + pPropiedad, TxtRegistroIDs).replace('Campo_', 'divContPesIdioma_');

            if (document.getElementById(idControlCampoPes) != null) {
                var li = $('li.active', $('#' + idControlCampoPes));
                var idiomaActual = li.attr('rel');
                $('#' + idControlCampoPes).attr('langActual', valor);
                valor = that.ExtraerTextoIdioma(valor, idiomaActual);
            }
            else { //Es multiIdioma sin pestaña
                var idiomas = IdiomasConfigFormSem.split(',');
                for (var i = 0; i < idiomas.length; i++) {
                    if (idiomas[i] != IdiomaDefectoFormSem) {
                        that.DarValorControlConIdioma(pEntidad + ',' + pPropiedad, pTxtIDs, pTxtCaract, that.ExtraerTextoIdioma(valor, idiomas[i]), idiomas[i]);
                    }
                }

                valor = that.ExtraerTextoIdioma(valor, IdiomaDefectoFormSem);
            }
        }
        
        that.DarValorControl(pEntidad + ',' + pPropiedad, pTxtIDs, pTxtCaract, valor);
        
        document.getElementById(idControlCampo.replace('Campo_','lbCrear_')).style.display = 'none';
        document.getElementById(idControlCampo.replace('Campo_', 'lbGuardar_')).style.display = '';
    }, 
    
    GuardarValorADataNoFuncionalProp: function(pEntidad, pPropiedad, pControlContValores, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados){
        const that = this;

        var entidadAlmacenar = pEntidad;
        pEntidad = pEntidad.replace('&ULT', '');
        var valor = that.ObtenerValorEntidadProp(pEntidad + ',' + pPropiedad, pTxtIDs, pTxtCaract);
        
        if (valor != '' && that.ComprobarValorCampoCorrecto(pEntidad, pPropiedad, valor, pTxtIDs, pTxtCaract))
        {
            if (that.EsPropiedadMultiIdioma(pEntidad, pPropiedad)) {
                var idControlCampoPes = that.ObtenerControlEntidadProp(pEntidad + ',' + pPropiedad, TxtRegistroIDs).replace('Campo_', 'divContPesIdioma_');
    
                if (document.getElementById(idControlCampoPes) != null) {
                    var li = $('li.active', $('#' + idControlCampoPes));
                    var idiomaActual = li.attr('rel');
                    valorAntiguo = $('#' + idControlCampoPes).attr('langActual');
                    valor = that.IncluirTextoIdiomaEnCadena(valorAntiguo, that.GetValorEncode(valor), idiomaActual);
    
                    if (!that.ComprobarMultiIdiomaCorrectoTexto(pEntidad, pPropiedad, valor)) {
                        return;
                    }
    
                    $('#' + idControlCampoPes).attr('langActual', '');
                }
                else { //Es multiIdioma sin pestaña
                    valor += '@' + IdiomaDefectoFormSem + '[|lang|]';
                    var idiomas = IdiomasConfigFormSem.split(',');
                    for (var i = 0; i < idiomas.length; i++) {
                        if (idiomas[i] != IdiomaDefectoFormSem) {
                            var valorPropIdioma = that.ObtenerValorEntidadPropDeIdioma(pEntidad + ',' + pPropiedad, pTxtIDs, pTxtCaract, idiomas[i]);
    
                            if (valorPropIdioma != '') {
                                valor += valorPropIdioma + '@' + idiomas[i] + '[|lang|]';
                            }
                        }
                    }
                }
            }
            
            that.SetValorElementoEditadoGuardado(entidadAlmacenar, pPropiedad, that.GetValorEncode(valor), pTxtValores, pTxtElemEditados);
            that.DarValorControl(pEntidad + ',' + pPropiedad, pTxtIDs, pTxtCaract, '');
    
            if (that.EsPropiedadMultiIdiomaSinPesanya(pEntidad, pPropiedad)) {
                var idiomas = IdiomasConfigFormSem.split(',');
                for (var i = 0; i < idiomas.length; i++) {
                    if (idiomas[i] != IdiomaDefectoFormSem) {
                        that.DarValorControlConIdioma(pEntidad + ',' + pPropiedad, pTxtIDs, pTxtCaract, '', idiomas[i]);
                    }
                }
            }
    
            that.GuardarValorEnContenedorGrupoValores(pControlContValores, valor, entidadAlmacenar, pPropiedad, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados);
            that.MarcarElementoEditado(entidadAlmacenar, pPropiedad, -1, pTxtElemEditados, pTxtCaract);
            that.EstablecerBotonesGrupoValores(pEntidad, pPropiedad, pTxtIDs);
            
            if (that.ExcedeCardinalidadMaxima(pEntidad, pPropiedad, pTxtValores, pTxtCaract, pTxtElemEditados))
            {
                var idControlCampo = that.ObtenerControlEntidadProp(pEntidad + ',' + pPropiedad, pTxtIDs);
                document.getElementById(idControlCampo.replace('Campo_','lbCrear_')).style.display = 'none';
            }
        }
    },
    
    SetValorElementoEditadoGuardado: function(pPadre, pElemento, pValor, pTxtValores, pTxtElemEditados){        
        const that = this;
        var numElem = that.GetNumEdicionEntProp(pPadre, pElemento, pTxtElemEditados);
        
        return that.SetValorElementoGuardado(pPadre, pElemento, pValor, pTxtValores, pTxtElemEditados, numElem);        
    },

    SetValorElementoGuardado: function(pPadre, pElemento, pValor, pTxtValores, pTxtElemEditados, pNumElemet){
        const that = this;
        var valorRdf = isJqueryObject(pTxtValores) ? pTxtValores.val() : document.getElementById(pTxtValores).value; // document.getElementById(pTxtValores).value;
        
        var indicesIniFinPadre = that.GetIndiceInicioFinElementoEditado(pPadre, valorRdf, pTxtElemEditados);
        var inicio = parseInt(indicesIniFinPadre.split(',')[0]);
        var fin = parseInt(indicesIniFinPadre.split(',')[1]);
        
        var cierrePadre = '</' + pPadre + '>';
        var elemElemento = '<' + pElemento + '>';
        var cierreElemento = '</' + pElemento + '>';
        
        var trozo1 = valorRdf.substring(0, inicio);
        var trozo2 = valorRdf.substring(inicio, (fin + cierrePadre.length));
        var trozo3 = valorRdf.substring(fin + cierrePadre.length);
        
        if (trozo2.indexOf(elemElemento) == -1)
        {
            return that.PutElementoGuardado(pPadre, pElemento, pValor, pTxtValores, pTxtElemEditados);
        }
        else
        {
            
            trozo2 = that.BorrarElementoDeXml(trozo2, pElemento, pNumElemet);
            trozo2 = that.AgregarElementoAXml(trozo2, elemElemento + pValor + cierreElemento, pNumElemet);
        }
        
        // Tener en cuenta que pTxtValores no sea un jqueryObject        
        if (isJqueryObject(pTxtValores)){
            pTxtValores.val(trozo1 + trozo2 + trozo3);
        }else{            
            document.getElementById(pTxtValores).value = trozo1 + trozo2 + trozo3;
        }        
    },

    GetIndiceInicioFinElementoEditado: function(pElemento, pValores, pTxtElemEditados){
        const that = this;
        var valorRdf = pValores;
        var editados = document.getElementById(pTxtElemEditados).value;
    
        var indiceInicio = that.GetIndiceInicioElementoEditado(pElemento, valorRdf, editados)
        valorRdf = valorRdf.substring(indiceInicio);
        
        var elementoCierreXML = '</' + pElemento + '>';
        var indiceFin = (indiceInicio + valorRdf.indexOf(elementoCierreXML))
        
        return indiceInicio + ',' + indiceFin;        
    },    

    GetIndiceInicioElementoEditado: function(pElemento, pValores, pEditados){
        const that = this;
        
        var editados = pEditados;
        var valores = pValores;
        var indiceElmen = 0;
        var entraRecursivo = false;
        
        var elementoXML = '<' + pElemento + '>';
        var elementoCierreXML = '</' + pElemento + '>';
        
        //Buscamos en todas las entidades, por si alguna es el padre de la buscada:
        while (editados.length > 0)
        {
            var elemEditados = editados.split('|')[0];
            if (elemEditados != '')
            {
                editados = editados.substring(editados.indexOf('|') + 1);
                
                var idElem = elemEditados.split('=')[0];
                var ent = idElem.split(',')[0];
                var prop = idElem.split(',')[1];
                
                if (ent != pElemento && prop != pElemento) //No es ni la propiedad ni la entidad
                {
                    var numElem = elemEditados.split('=')[1];
                    var contenidoEntidad = that.GetValorElementoEnPosicion(prop, numElem, valores);
                    
                    if (contenidoEntidad.indexOf(elementoXML) != -1)
                    {
                        var elmeIdElem = '<' + prop + '>';
                        var cierreElmeIdElem = '</' + prop + '>';
                        var indiceCalculado = false;
                        
                        //Compruebo que no esté seleccionando un elemento candidato a ser agregado:
                        if (editados == '' && valores.indexOf('<||>') != -1)
                        {
                            var trozoAux = valores.substring(valores.indexOf('<||>') + 4);
                            if (trozoAux.indexOf(elmeIdElem) != -1)
                            {
                                indiceElmen += valores.indexOf('<||>') + 4;
                                valores = trozoAux;
                            }
                            else if (trozoAux.indexOf('<' + pElemento + '>') != -1) {//Quitar si va mal la edición, se ha puesto por Form Generic prado
                                indiceElmen += valores.indexOf('<||>') + 4;
                                valores = trozoAux;
    
                                indiceElmen += valores.indexOf('<' + pElemento + '>');
                                valores = valores.substring(valores.indexOf('<' + pElemento + '>'));
                                indiceCalculado = true;
                            }
                        }
                        
                        if (!indiceCalculado) {
                            indiceElmen += valores.indexOf(elmeIdElem);
                            valores = valores.substring(valores.indexOf(elmeIdElem));
    
                            for (var j = numElem; j > 0; j--) {
                                indiceElmen += valores.indexOf(cierreElmeIdElem) + cierreElmeIdElem.length;
                                valores = valores.substring(valores.indexOf(cierreElmeIdElem) + cierreElmeIdElem.length);
                            }
    
                            valores = valores.substring(0, valores.indexOf(cierreElmeIdElem) + cierreElmeIdElem.length);
                            if (ent == pElemento) {
                                entraRecursivo = true;
                            }
                        }
                    }
                }
            }
            else
            {
                break;
            }
        }
        
        if (!entraRecursivo)
        {
            return (indiceElmen + valores.lastIndexOf(elementoXML));
        }
        else if (valores.indexOf(elementoXML) != -1)
        {
            return (indiceElmen + valores.indexOf(elementoXML));
        }
        else
        {
            return indiceElmen;
        }
                
    },


    EliminarValorDeDataNoFuncionalProp: function(pNumElem, pEntidad, pPropiedad, pControlContValores, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados)
    {
        const that = this;
        let TxtHackHayCambios = true;

        const $pTxtIDs = $(`#${pTxtIDs}`);
        const $pTxtCaract = $(`#${pTxtCaract}`);        
        
        that.MarcarElementoEditado(pEntidad, pPropiedad, pNumElem, pTxtElemEditados, pTxtCaract);
        
        that.DeleteElementoEditadoGuardado(pEntidad, pPropiedad, pTxtValores, pTxtElemEditados);
        
        const idControl = that.ObtenerControlEntidadProp(pEntidad + ',' + pPropiedad, $pTxtIDs);
        if (document.getElementById(idControl.replace('Campo_', 'chkImgPrincfileUpLoad_')) != null) {            
            that.EliminarImagenPrincipalProp(pEntidad, pPropiedad);
        }
        
        that.DarValorControl(pEntidad + ',' + pPropiedad, $pTxtIDs, $pTxtCaract, '');
        
        if (that.EsPropiedadMultiIdiomaSinPesanya(pEntidad, pPropiedad)) {
            var idiomas = IdiomasConfigFormSem.split(',');
            for (var i = 0; i < idiomas.length; i++) {
                if (idiomas[i] != IdiomaDefectoFormSem) {                    
                    that.DarValorControlConIdioma(pEntidad + ',' + pPropiedad, $pTxtIDs, $pTxtCaract, '', idiomas[i]);
                }
            }
        }
        
        that.GuardarValorEnContenedorGrupoValores(pControlContValores, '', pEntidad, pPropiedad, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados);
        that.MarcarElementoEditado(pEntidad, pPropiedad, -1, pTxtElemEditados, pTxtCaract);
        
        const idControlCampo = that.ObtenerControlEntidadProp(pEntidad + ',' + pPropiedad, $pTxtIDs);
        document.getElementById(idControlCampo.replace('Campo_','lbCrear_')).style.display = '';
        document.getElementById(idControlCampo.replace('Campo_', 'lbGuardar_')).style.display = 'none';
        $('#' + idControlCampo.replace('Campo_', 'divContPesIdioma_')).attr('langActual', '');//Si es multiIdioma reiniciará los valres editados

        // Habilitar el botón de guardado        
        that.handleEnableOrDisableApplyButton(true);
    },

    GuardarValorEnContenedorGrupoValores: function(pControlCont, pValor, pEntidad, pPropiedad, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados)
    {
        const that = this;

        let $pTxtElemEditados = "";        
        
        // Tener en cuenta el control de si es un objeto jquery    
        if (isJqueryObject(pTxtElemEditados)){
            $pTxtElemEditados = pTxtElemEditados;
        }else{
            $pTxtElemEditados = $(`#${pTxtElemEditados}`);
        }

        const numElem = that.GetNumEdicionEntProp(pEntidad, pPropiedad, $pTxtElemEditados);
        if (pValor != '')
        {            
            pValor = that.GetValorDecode(pValor);
            if (pValor.indexOf('[|lang|]') != -1) {
                pValor = that.ExtraerTextoIdioma(pValor, IdiomaDefectoFormSem)
            }            
            // document.getElementById(pControlCont).children[0].children[0].children[numElem].innerHTML = that.ObtenerFilaValorContenedorGrupoValores(pControlCont, pValor, numElem, false, pEntidad, pPropiedad, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados);            
            document.getElementById(pControlCont).children[0].children[numElem].innerHTML = that.ObtenerFilaValorContenedorGrupoValores(pControlCont, pValor, numElem, false, pEntidad, pPropiedad, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados);
        }
        else //Hay que eliminarlo
        {            
            //var hijos = document.getElementById(pControlCont).children[0].children[0].children;
            const hijos = document.getElementById(pControlCont).children[0].children;

            let htmlFinal = '';
            
            for (var i=0;i<hijos.length;i++)
            {
                if (i != numElem)
                {                    
                    if (i>numElem)
                    {                  
                        // Ordenar correctamente indicando el parámetro y restándolo según corresponda para la función EliminarObjectNoFuncionalProp                     
                        const hijoHtmlContent = hijos[i].outerHTML.replace(/'\d+'/g, function(match) {                        
                            let newPositionForJsFunction = parseInt(match.replace(/'/g, '')) - 1;
                            return "'" + newPositionForJsFunction + "'";
                        });                    
                        htmlFinal += hijoHtmlContent;                         
                    }
                    else
                    {                                                
                        htmlFinal += hijos[i].outerHTML;                        
                    }
                }                                 
            }
            
            document.getElementById(pControlCont).children[0].innerHTML = htmlFinal;
        }
    },    

    EliminarImagenPrincipalProp: function(pEntidad, pPropiedad) {
        const that = this;        
        const docExtID = that.GetDocRecExtSelecEntEditable(pEntidad, pPropiedad, true, null);        
        that.EliminarImagenPrincipal(docExtID);
    },

    EliminarImagenPrincipalProp: function(pEntidad, pPropiedad) {
        const that = this;
        var docExtID = that.GetDocRecExtSelecEntEditable(pEntidad, pPropiedad, true, null);
        that.EliminarImagenPrincipal(docExtID);
    }, 
    
    EliminarImagenPrincipal: function(pDocID) {
        const that = this;
        var vals = document.getElementById('txtHackValorImgRepresentante').value.split('|');
        document.getElementById('txtHackValorImgRepresentante').value = '';
    
        if (pDocID == null) {
            pDocID = 'doc';
        }    
    },      
    
    GetDocRecExtSelecEntEditable: function(pEntidad, pPropiedad, pRecursivo, pNumElem) {
        const that = this;
        var subOntos = $('#txtSubOntologias').val();
        var indiceProp = subOntos.indexOf(pEntidad + ',' + pPropiedad + '|');
    
        if (indiceProp != -1) {
            subOntos = subOntos.substring(indiceProp);
            subOntos = subOntos.substring(0, subOntos.indexOf('|||'));
            var numElem = 0;
    
            if (pNumElem != null) {
                numElem = pNumElem;
            }
            else {

                numElem = that.GetNumEdicionEntProp(pEntidad, pPropiedad, $(TxtElemEditados));
            }
    
            var docID = subOntos.substring(subOntos.indexOf('|' + numElem + ','));
            docID = docID.substring(docID.indexOf(',') + 1);
            docID = docID.substring(0, docID.indexOf(','));
    
            return docID;
        }
        else if (pRecursivo) {            
            var entProps = that.GetEntidadYPropiedadConEntidadComoRango(pEntidad);
    
            for (var i = 0; i < entProps.length; i++) {                
                var docID = that.GetDocRecExtSelecEntEditable(entProps[i][0], entProps[i][1], pRecursivo, pNumElem);
    
                if (docID != null) {
                    return docID;
                }
            }
        }
    
        return null;
    },    

    DeleteElementoEditadoGuardado: function(pPadre, pElemento, pTxtValores, pTxtElemEditados){
        const that = this;        

        const $pTxtElemEditados = $(`#${pTxtElemEditados}`);        

        const numElem = that.GetNumEdicionEntProp(pPadre, pElemento, $pTxtElemEditados); 
        // Añadir a that. el ptxtValores para ser usado en DeleteElementoGuardado
        that.pTxtValores = $(`#${pTxtValores}`);       
        return that.DeleteElementoGuardado(pPadre, pElemento, pTxtValores, $pTxtElemEditados, numElem);
    },    

    MarcarElementoEditado: function(pEntidad, pPropiedad, pNumElem, pTxtElemEditados, pTxtCaract){
        const that = this;
        let $pTxtCaract = "";
        
        // Comprobar si es un objeto jquery
        if (isJqueryObject(pTxtCaract)){
            $pTxtCaract = pTxtCaract;
        }else{
            $pTxtCaract = $(`#${pTxtCaract}`);
        }

        // Comprobar si es un objeto jquery         
        let editados = isJqueryObject(pTxtElemEditados) ? pTxtElemEditados.val() :document.getElementById(pTxtElemEditados).value;
        let idElem = pEntidad;
        if (pPropiedad != null)
        {
            idElem += ',' + pPropiedad;
        }
        
        idElem += '=';
        
        if (editados.indexOf(idElem) != -1) //Elimino el elemento:
        {
            if (isJqueryObject(pTxtElemEditados)){
                pTxtElemEditados.val(editados.substring(0, editados.indexOf(idElem)));
            }else{
                document.getElementById(pTxtElemEditados).value = editados.substring(0, editados.indexOf(idElem));
            }
            
            editados = editados.substring(editados.indexOf(idElem));
            editados = editados.substring(editados.indexOf('|') + 1);

            if (isJqueryObject(pTxtElemEditados)){
                let newValue = pTxtElemEditados.val();
                newValue += editados;
                pTxtElemEditados.val(newValue);
            }else{
                document.getElementById(pTxtElemEditados).value += editados;
            }            
            
            
            if (that.EsPropiedadGrupoPaneles(pEntidad, pPropiedad, $pTxtCaract)) //Desmarco todas sus propiedades hijas:
            {                
                var entidadHija = that.GetRangoPropiedad(pEntidad, pPropiedad, $pTxtCaract);                
                var propiedades = that.GetPropiedadesEntidad(entidadHija, pTxtCaract);
                
                for (var i=0;i<propiedades.length;i++)
                {
                    if (propiedades[i] != '')
                    {
                        that.MarcarElementoEditado(entidadHija, propiedades[i], -1, pTxtElemEditados, pTxtCaract)
                    }
                }
            }
        }
        
        if (pNumElem != -1)
        {
            if (isJqueryObject(pTxtElemEditados)){
                let newValue = pTxtElemEditados.val();
                newValue += idElem + pNumElem + '|';
                pTxtElemEditados.val(newValue);
            }else{
                document.getElementById(pTxtElemEditados).value += idElem + pNumElem + '|';
            }                           
        }
    },

    GetPropiedadesEntidad: function (pEntidad, pTxtCaract){
        const that = this;
        var caract = isJqueryObject(pTxtCaract) ? pTxtCaract.val() : document.getElementById(pTxtCaract).value; // document.getElementById(pTxtCaract).value;
        var idEnti = '|' + pEntidad + ',';
        
        var propiedades = '';
        var indiceProp = caract.indexOf(idEnti);
        while (caract.length > 0 && indiceProp != -1)
        {
            var prop = caract.substring(indiceProp + idEnti.length);
            prop = prop.substring(0, prop.indexOf(','));
            propiedades += prop + ',';
            
            caract = caract.substring(indiceProp);
            caract = caract.substring(caract.indexOf('|') + 1);
            indiceProp = caract.indexOf(idEnti);
        }
        
        return propiedades.split(',');
    },    

    GetRangoPropiedad: function(pEntidad, pPropiedad, pTxtCaract){
        const that = this;    
        return that.GetCaracteristicaPropiedad(pEntidad, pPropiedad, pTxtCaract, 'rango');
    },

    EsPropiedadGrupoPaneles: function(pEntidad, pPropiedad, pTxtCaract){
        const that = this;
        return (that.GetTipoPropiedad(pEntidad, pPropiedad, pTxtCaract) == 'LO');        
    },

    
    /**
     * Método para buscar un elemento historico de un objeto de conocimiento
     * @param {*} input    
     */    
    handleSearchElementosHistoricoObjetoConocimiento: function(input){
        const that = this;
        // Cadena introducida para filtrar/buscar
        let cadena = input.val();
        // Eliminamos posibles tildes para búsqueda ampliada
        cadena = cadena.normalize('NFD').replace(/[\u0300-\u036f]/g, "").toLowerCase();

        // Listado de elementos donde se realizará la búsqueda
        const objetoHistoricoItems = that.filaObjetoConocimiento.find($(`.${that.historialRowClassName}`));

        // Recorrer los items/componentes para realizar la búsqueda de palabras clave
        $.each(objetoHistoricoItems, function(index){            
            const objetoHistoricoItem = $(this);
           
            // Seleccionar el nombre. Quitamos caracteres extraños, tildes para poder hacer bien la búsqueda
            const componentContenido = objetoHistoricoItem.find(".labelHistoryName").not('.d-none').html().trim().normalize("NFD").replace(/[\u0300-\u036f]/g, "").toLowerCase();

            if (componentContenido.includes(cadena)){
                // Mostrar la fila
                objetoHistoricoItem.removeClass("d-none");                
            }else{
                // Ocultar la fila
                objetoHistoricoItem.addClass("d-none");
            }                      
        });          
    },




    /**
     * Método para establecer una pestaña como activa al pulsar sobre "Editar objeto de conocimiento". Cargará los items o ficheros históricos si hay para el oc seleccionado
     * @param {*} pObjetoConocimientoRow: Fila del objeto de conocimiento que se está editando o sobre el que se desea editar la información
     */
    handleSetTabActiveHistoryFiles: function(pObjetoConocimientoRow){        
        const that = this;
        
        // Comprobar y hacer petición para cargar los históricos del Objeto de Conocimiento (Ficheros históricos, Owl, xml)
        if (!stringToBoolean(pObjetoConocimientoRow.data("download-history"))){                                
            const documentOnto = pObjetoConocimientoRow.data("documentid");
            that.handleGetHistoryElementItems(documentOnto);                    
        }                                           
    },  

    /**
     * Método para descargar las versiones/histórico de ficheros de una ontología. Se hará la petición cuando se pulse en el tab correspondiente
     * @param {*} documentOnto ID de la ontología de la que se desean descargar los ficheros
     */
    handleGetHistoryElementItems: function(documentOnto){
 
        const that = this;     
        
        // Panel donde se cargarán los elementos del objeto de conocimiento secundario        
        const panelHistoryFileElements = that.filaObjetoConocimiento.find(`.${that.ontologyFileHistorialPanelClassName}`);                                                                     
        loadingMostrar(panelHistoryFileElements)

        // Construir el objeto para consultar datos
        const dataPost = {
            ontoID: documentOnto,                        
        }
    
        // Realizar la petición para obtener la vista       
        GnossPeticionAjax(
            that.urlLoadHistoryFiles,
            dataPost,
            true
        ).done(function (view) {
            // Indicador de que ya se han descargado los items para no volver a pintarlas
            that.filaObjetoConocimiento.data("download-history","true");       
            panelHistoryFileElements.append(view);
            // Actualizar el contador de nº de ficheros históricos
            that.handleCheckNumberOfHistoryFiles();
        }).fail(function (data) {            
            mostrarNotificacion("error", data);
        }).always(function () {
            // Ocultar el loading
            loadingOcultar();
        });                    
    },

    /**
     * Método para contabilizar el nº de ficheros históricos que se han encontrado de un objeto de conocimiento
     */
    handleCheckNumberOfHistoryFiles: function(){        
        const that = this;
                
        // Título donde se muestran los resultados del objeto de conocimiento secundario a actualizar
        const labelResultadosHistorico = that.filaObjetoConocimiento.find(`.${that.numResultadosHistoryFilesClassName}`);
        // Mostrar el nº de items
        const numberResultados = that.filaObjetoConocimiento.find($(`.${that.historialRowClassName}`)).length;
        labelResultadosHistorico.html(numberResultados);                        
    }, 


    /**
     * Método para habilitar o deshabilitar el botón de "Aplicar" o guardado de un elemento o instancia de objeto secundario
     * @param {*} enable Valor booleano para indicar si se desea deshabilitar o no el botón de guardado
     */
    handleEnableOrDisableApplyButton: function(enable){
        const that = this;
        // Habilitar el botón Guardar Elemento ya que se desea "Guardar"
        const btnAplicar = that.filaElementoObjetoConocimientoSecundario.find(`.${that.btnSaveInstanciaEntidadClassName}`);                                  
        if (!enable){
            // Deshabilitar el botón de guardado
            btnAplicar.prop('disabled', true); 
        }else{
            // Habilitar el botón de guardado
            btnAplicar.prop('disabled', false); 
        }
    },
    

    /**
     * Método que indica si el input proporcionado contiene o no algún espacio en blanco.
     * @param {jqueryElement} input 
     * @returns Devuelve un valor Booleano indicando si tiene o no un espacio en blanco.
     */
    handleCheckContainsWhiteSpace: function(input){         
        return input.val().trim().indexOf(' ') >= 0 || input.val().trim().length <= 0;    
    },

    /**
     * Método para eliminar un elemento de un objeto de conocimiento secundario
     * Si es de reciente creación y aún no se ha guardado, se eliminará simplemente el panel.
     * Si por el contrario, el elemento existe, se procederá al borrado/petición para su ejecución.
     * @param {*} pElementRow 
     */
    handleDeleteSecondaryElement: function(pElementRow){
        const that = this;
                
        if (pElementRow.data("new-element") == true){
            // Eliminar la fila sin petición            
            pElementRow.fadeOut(300, function() { 
                $(this).remove(); 
                // Actualizar el nº de entidades del objeto de conocimiento
                that.handleCheckNumberOfEntities();
            });
            return;
        }        
         
        // Id del elemento a borrar
        const idsSujetoEntidad = pElementRow.data("sujeto-entidad");
        // Objeto a construir para realizar la petición
        const arg = {};            
        arg.SelectedInstances = idsSujetoEntidad;
        // Sujeto Entidad        
        arg.SujetoEntidad = idsSujetoEntidad;
        // Grafo asociado a la entidad a borrar
        arg.Grafo = that.filaObjetoConocimiento.data("grafo-actual");

        // Realizar petición para el borrado
        that.handleSaveEntSec(arg, false, pElementRow ,true);
        
    },

    /**
     * Método para ocultar o mostrar el botón de guardado general del objeto de conocimiento
     * @param {bool} hide : Indica si se desea mostrar o no el botón de Guardar el OC
     */
    handleShowHideGeneralSaveButton: function(hide){
        const that = this;
        // Botón guardado del OC
        const pSaveButton = that.filaObjetoConocimiento.find(`.${that.btnSaveObjetoConocimientoClassName}`);
        hide == true ? pSaveButton.addClass("d-none") : pSaveButton.removeClass("d-none");        
    },


    /**
     * Método para solicitar la vista para añadir un nuevo elemento/instancia al objeto de conocimiento secundario
     */
    handleLoadNewViewForSecondaryElementInstance: function(){
        const that = this;
        
        loadingMostrar();

        // Panel donde se cargará el item nuevo a crear
        const panelElementsSecondaryOntology = that.filaObjetoConocimiento.find(`.${that.panelElementsSecondaryOntologyClassName}`);
        const graphName = that.filaObjetoConocimiento.data("grafo-actual");                                                                    
        
        // Construir el objeto para solicitar la creación de un nuevo item
        const dataPost = {
            Grafo: graphName,                        
        }

        // Realizar la petición para obtener la vista       
        GnossPeticionAjax(
            that.urlLoadCreateNewEntity,
            dataPost,
            true
        ).done(function (view) {
            // Añadir la vista a añadir nueva al principio del listado de items            
            panelElementsSecondaryOntology.prepend(view);
            // Actualizar contador de nº de entidades
            that.handleCheckNumberOfEntities();
        }).fail(function (data) {            
            mostrarNotificacion("error", data);
        }).always(function () {
            // Ocultar el loading
            loadingOcultar();
        });        
    },

    /**
     * Método para obtener los items o elementos de un determinado objeto de conocimiento de tipo secundario. Hará una petición para obtener los items
     * en la vista de elementos
     * @param {*} graphName 
     */
    handleGetElementItems: function(graphName){   
        const that = this;     
        
        // Panel donde se cargarán los elementos del objeto de conocimiento secundario        
        const panelElementsSecondaryOntology = that.filaObjetoConocimiento.find(`.${that.panelElementsSecondaryOntologyClassName}`);                                                                     
        loadingMostrar(panelElementsSecondaryOntology)

        // Construir el objeto para consultar datos
        const dataPost = {
            Grafo: graphName,                        
        }
    
        // Realizar la petición para obtener la vista       
        GnossPeticionAjax(
            that.urlLoadSecondaryEntities,
            dataPost,
            true
        ).done(function (view) {
            // Indicador de que ya se han descargado los items para no volver a pintarlas
            that.filaObjetoConocimiento.data("download-items","true");
            // Habilitar el botón de "Añadir nuevo elemento" una vez se han descargado o la petición se ha realizado            
            const btnAddElement = that.filaObjetoConocimiento.find(`.${that.btnAddSecondaryElementClassName}`);
            btnAddElement.prop('disabled', false);            
            panelElementsSecondaryOntology.append(view);
            // Actualizar el  nº de items a mostrar
            that.handleCheckNumberOfEntities();
        }).fail(function (data) {            
            mostrarNotificacion("error", data);
        }).always(function () {
            // Ocultar el loading
            loadingOcultar();
        });            
    },

    /**
     * Método para obtener los datos de un determinado elemento que pertenece a un objeto de conocimiento de tipo secundario
     * @param {*} pElementRow 
     */
    handleGetElementInfoOrCreateNewElementInfo: function(pElementRow){
        const that = this;

        // Panel donde se desplegarán los detalles del elemento
        const panelElementDetail = pElementRow.find(`.${that.panelElementDetailClassName}`);        
        // Nombre del grafo o del objeto secundario del cual se desea editar el elemento
        const graphName = that.filaObjetoConocimiento.data("grafo-actual");
        // Sujeto entidad del elemento a editar
        const entitySubjet = pElementRow.data("sujeto-entidad"); 
        // Flag para detectar si se desea petición para editar datos
        let isInEditMode = true;
        // Url al que se desea realizar la petición
        let urlRequest = that.urlLoadSecondaryEntityDetails;
        // Parámetros para realizar la petición (Edición / Creación nuevo item)
        let dataPost = {
            Grafo: graphName,
        };      
                

        // Continuary comprobar la descarga si el elemento no es nuevo
        if (pElementRow.data("new-element") == true){
            return;
        }       
        
        // Comprobar si es necesario descargar los datos si no es un elemento nuevo a crear o si ya se han descargado          
        if (!stringToBoolean(pElementRow.data("download-items"))){                         
            // Comprobar si es necesario descargar o editar datos para realizar la petición             
            if (graphName.length == 0){
                // Crear elemento
                isInEditMode = false;
                urlRequest = that.urlLoadCreateNewEntity;
            }else{
                // Editar elemento: Añadir el SujetoEntidad                
                dataPost['SujetoEntidad'] = entitySubjet;                
            }
            // Mostrar loading en el panel collapsed donde se añadirán los datos/vista
            loadingMostrar(panelElementDetail);

            // Realizar la petición para la obtención de los datos (Vista) a editar o del nuevo item            
            GnossPeticionAjax(
                urlRequest,
                dataPost,
                true
            ).done(function (view) {
                // Indicador de que ya se han descargado los items para no volver a pintarlas
                pElementRow.data("download-items","true");
                // Panel donde se añadirá la vista                                
                panelElementDetail.prepend(view)
            }).fail(function (data) {            
                mostrarNotificacion("error", data);
            }).always(function () {
                // Ocultar el loading
                loadingOcultar();
            });            
        }
    },
       

    /**
     * Método para poder cambiar entre idiomas y poder visualizar los datos de los objetos de conocimiento en el idioma deseado sin tener que acceder al modal.
     * Gestionará el click en el tab de idiomas principal del oc
     */    
    handleViewOcLanguageInfo: function(){
        const that = this;
        // Comprobar el item que está activo en el tab obteniendo el data-language de la opción "active"
        setTimeout(function(){             
            // Detectar el tab item activo para conocer el idioma en el que se desean mostrar las páginas
            const tabLanguageActive = that.tabIdiomaItem.filter(".active");
            // Obtener el idioma del tabLanguageActivo
            const languageActive = tabLanguageActive.data("language");
            // Ocultar todas las labels y mostrar únicamente las del idioma seleccionado
            that.labelLanguageComponent.addClass("d-none");
            // Mostrar sólo las labelsLanguageComponent del idioma seleccionado
            that.labelLanguageComponent.filter(function () {
                return $(this).data("languageitem") == languageActive;
            }).removeClass("d-none");

        },250);         
    },

    /**
     * Método para descargar las clases y vistas. Se reutilizará para poder descargar las clases y vistas Java.
     * @param {bool} downloadClasesYVistasJava : Indica si se desea descargar las clases y vista de Java
     */
    handleDownloadClasesYVistas: function(downloadClasesYVistasJava){
        const that = this;
        // Url utilizada para realizar la descarga de Vistas (Vistas y clases Java si procede)
        var urlDownload = (downloadClasesYVistasJava ? this.urlDownloadClasesVistasJava : this.urlDownloadClasesVistas);
        loadingMostrar();
        
        jQuery.ajax({
            url: urlDownload,
            cache: false,
            xhr: function() {
                var xhr = new XMLHttpRequest();
                xhr.onreadystatechange = function() {
                    if (xhr.readyState == 2) {
                        if (xhr.status == 200) {
                            xhr.responseType = "blob";
                        } else {
                            xhr.responseType = "text";
                        }
                    }
                };
                return xhr;
            },
            error: function(xhr, textStatus, errorThrown) {
                mostrarNotificacion("error", xhr.responseText);
                loadingOcultar();
            },
            success: function(data) {
                const blob = new Blob([data], { type: "application/zip" });
                // Guardar el blob obtenido (FileSaver.min.js)
                saveAs(blob, "clasesGeneradas.zip");
                loadingOcultar();
            },
        });         

        /*
        jQuery.ajax({
            url: urlDownload,
            cache: false,
            xhr: function() {
                var xhr = new XMLHttpRequest();
                xhr.onreadystatechange = function() {
                    if (xhr.readyState == 2) {
                        if (xhr.status == 200) {
                            xhr.responseType = "blob";
                        } else {
                            xhr.responseType = "text";
                        }
                    }
                };
                return xhr;
            },
            error: function(xhr, textStatus, errorThrown) {
                // Here you are able now to access to the property "responseText"
                // as you have the type set to "text" instead of "blob".
                console.error(xhr.responseText);
            },
            success: function(data, status) {
                const blob = new Blob([this.response], { type: "application/zip" });
                // Guardar el blob obtenido (FileSaver.min.js)
                saveAs(blob, "clasesGeneradas.zip");
            }
        });
        
        
        /*
        GnossPeticionAjax(
            urlDownload,
            null,
            false,
            true,
        ).done(function (data) {
            // OK - Descargar fichero 
            const blob = new Blob([data], { type: "application/zip" });
            // Guardar el blob obtenido (FileSaver.min.js)
            saveAs(blob, "clasesGeneradas.zip");                            
        }).fail(function (data) {
            // KO
            mostrarNotificacion("error", data);                
        }).always(function () {
            // Ocultar loading
            loadingOcultar();
        });
        */

        // Realizar la petición para obtener la vista       
        /*GnossPeticionAjax(
            urlDownload,
            null,
            false,
            true,
            "blob",            
        ).done(function (data) {            
            // OK
            const isBlobFile = data instanceof Blob;
            if (isBlobFile){
                const blob = new Blob([data], { type: "application/zip" });
                // Guardar el blob obtenido (FileSaver.min.js)
                saveAs(blob, "clasesGeneradas.zip");                       
            }else{
                mostrarNotificacion("error", data);
            }            
        }).fail(function (data) {            
            mostrarNotificacion("error", data);
        }).always(function () {
            // Ocultar el loading
            loadingOcultar();
        });
        */         
        
        
         
    },

    /**
     * Método para comprobar cómo debería de estar el "DragAndDrop" del objeto de conocimiento dependiendo del estado de los checkboxes de "Uso del fichero genérico"
     * Este método convendría ser ejecutado al inicio de la operativa (triggerEvents)
     */
    handleCheckDragAndDropStatus: function(){
        const that = this;

        // Prevenir que GnossDragAndDrop haya cargado correctamente
        setTimeout(function(){
            const panelesObjetosConocimiento =  $(`.${that.panelObjetoConocimientoTemplateClassName}`);

            // Visualizar todos los paneles de DragAndDrop
            Array.from(panelesObjetosConocimiento).forEach(item => {                
                const panel = $(item);
                const useGenericFileCheckBox = panel.find(`.${that.chkUtilizarArchivoGenericoClassName}`);
                if (useGenericFileCheckBox.length > 0) {
                    that.handleChangeGenericFileUsage(useGenericFileCheckBox);
                }                                  
            });
        }
        ,1000);
    },

    /**
     * Método para controlar si se desea deshabilitar o no el uso del dragAndDrop por seleccionar el uso de un fichero genérico
     * @param {jqueryElement} checkbox 
     */
    handleChangeGenericFileUsage: function(checkbox){
        // Panel padre de todos los elementos de la plantilla
        const panelObjetoConocimientoTemplate = checkbox.closest( `.${this.panelObjetoConocimientoTemplateClassName}` );
        
        // Panel del objeto de conocimiento a mostrar/ocultar
        const dragAndDrop = panelObjetoConocimientoTemplate.find(".gdd-wrap");
    
        // Mostrar/Ocultar como disabled el drag & drop
        if (checkbox.is(":checked")){            
            dragAndDrop.addClass("inactiveArea");
        }else{            
            dragAndDrop.removeClass("inactiveArea");
        }
    },    

    /**
     * Método para mostrar u ocultar los paneles necesarios dependiendo del tipo de objeto de conocimiento a crear
     */
    handleChangeObjetoConocimientoType: function(){
        const that = this;
        // Obtener el tipo de objeto de conocimiento (Primario / Secundario)
        const objetoConocimientoType = that.currentModalOC.find($(`input[type=radio][name=${this.crearObjetoConocimientoName}]:checked`)).val();
        const buttonSave = that.currentModalOC.find($(`.${that.btnSaveNewObjetoConocimientoClassName}`));        

        // Visualizar panel de drag and drop de ficheros
        const panelFicherosOntologia = that.currentModalOC.find($(`.${that.panelOntologyFilesClassName}`));
        panelFicherosOntologia.removeClass("d-none");

        // Visualizar todos los paneles de DragAndDrop
        Array.from(that.currentModalOC.find($(`.${that.panelObjetoConocimientoTemplateClassName}`))).forEach(item => {                
            const panel = $(item);
            panel.removeClass("d-none");            
        });

        if (objetoConocimientoType== 'primario') {
            // Objeto de conocimiento primario
            buttonSave.prop("disabled", false);            
        }
        else if (objetoConocimientoType == 'secundario') {
            // Objeto de conocimiento secundario            
            buttonSave.prop("disabled", false);
            // Ocultar los paneles de DragAndDrop de CSS, JS e icono            
            that.currentModalOC.find($(`.${that.panelObjetoConocimientoJsTemplateClassName}`)).addClass("d-none");
            that.currentModalOC.find($(`.${that.panelObjetoConocimientoIconoImageTemplateClassName}`)).addClass("d-none");
            that.currentModalOC.find($(`.${that.panelObjetoConocimientoCssTemplateClassName}`)).addClass("d-none");
        }else{
            // Ninguno seleccionado -> Deshabilitar el botón
            buttonSave.prop("disabled", true);
            panelFicherosOntologia.addClass("d-none");
        }
    },

    /**
     * Método para cambiar la visualización al idioma deseado
     */
     handleViewTranslateLanguageInfo: function(){
        const that = this;
        // Comprobar el item que está activo en el tab obteniendo el data-language de la opción "active"
        setTimeout(function(){             
            // Detectar el tab item activo para conocer el idioma en el que se desean mostrar las páginas
            const tabLanguageActive = that.tabIdiomaItem.filter(".active");
            // Obtener el idioma del tabLanguageActivo
            const languageActive = tabLanguageActive.data("language");
            // Ocultar todas las labels y mostrar únicamente las del idioma seleccionado
            that.labelLanguageComponent.addClass("d-none");
            // Mostrar sólo las labelsLanguageComponent del idioma seleccionado
            that.labelLanguageComponent.filter(function () {
                return $(this).data("languageitem") == languageActive;
            }).removeClass("d-none");

        },250);
    }, 
    
    /**
     * Método para buscar un elemento de un objeto de conocimiento de tipo secundario
     * @param {jqueryElement} input : Input desde el que se ha realizado la búsqueda
     * @param {jqueryElement} showUnsavedItemsOnly : Balor booleano para mostrar únicamente aquellos items que han sido editados y aún no se han guardado. Por defecto, false.
     */
    handleSearchElementosObjetoConocimientoSecundario: function(input, showUnsavedItemsOnly = false){
        const that = this;
        // Cadena introducida para filtrar/buscar
        let cadena = input.val();
        // Eliminamos posibles tildes para búsqueda ampliada
        cadena = cadena.normalize('NFD').replace(/[\u0300-\u036f]/g, "").toLowerCase();

        // Listado de elementos donde se realizará la búsqueda
        const objetoConocimientoSecundarioItems = that.filaObjetoConocimiento.find($(`.${that.elementRowClassName}`));

        // Recorrer los items/compontes para realizar la búsqueda de palabras clave
        $.each(objetoConocimientoSecundarioItems, function(index){            
            const objetoConocimientoSecundarioItem = $(this);
            // Detectar los items que están sin guardar
            let isSaved = true;

            if (showUnsavedItemsOnly){
                // Obviar el item si este no está como "Cambios sin guardar"
                if(!objetoConocimientoSecundarioItem.find(`.${that.labelElementExtraInfoClassName}`).hasClass("d-none")){
                    // No está siendo editado
                    isSaved = false;
                }                
            }

            if (isSaved && showUnsavedItemsOnly){
                objetoConocimientoSecundarioItem.addClass("d-none");
            }
            else{
                // Seleccionar el nombre. Quitamos caracteres extraños, tildes para poder hacer bien la búsqueda
                const componentContenido = objetoConocimientoSecundarioItem.find(".labelElementName").not('.d-none').html().trim().normalize("NFD").replace(/[\u0300-\u036f]/g, "").toLowerCase();

                if (componentContenido.includes(cadena)){
                    // Mostrar la fila
                    objetoConocimientoSecundarioItem.removeClass("d-none");                
                }else{
                    // Ocultar la fila
                    objetoConocimientoSecundarioItem.addClass("d-none");
                }  
            }         
        });  
    },

    /**
     * Método para buscar un objeto de conocimiento
     * @param {jqueryElement} input : Input desde el que se ha realizado la búsqueda
     */
    handleSearchObjetoConocimientoItem: function(input){     
        const that = this;
        // Cadena introducida para filtrar/buscar
        let cadena = input.val();
        // Eliminamos posibles tildes para búsqueda ampliada
        cadena = cadena.normalize('NFD').replace(/[\u0300-\u036f]/g, "").toLowerCase();

        // Listado de componentes donde se buscará
        const objetoConocimientoItems = $(`.${that.objetoConocimientoListItemClassName}`);

        // Recorrer los items/compontes para realizar la búsqueda de palabras clave
        $.each(objetoConocimientoItems, function(index){
            const objetoConocimientoItem = $(this);
            // Seleccionar el nombre sólo "visible" de la traducción. Quitamos caracteres extraños, tildes para poder hacer bien la búsqueda
            const componentContenido = objetoConocimientoItem.find(".component-contenido").not('.d-none').html().trim().normalize("NFD").replace(/[\u0300-\u036f]/g, "").toLowerCase();
            const componentShortNameOntology = objetoConocimientoItem.find(".component-shortNameOntology").not('.d-none').html().trim().normalize("NFD").replace(/[\u0300-\u036f]/g, "").toLowerCase();

            if (componentContenido.includes(cadena) || componentShortNameOntology.includes(cadena)){
                // Mostrar la fila
                objetoConocimientoItem.removeClass("d-none");                
            }else{
                // Ocultar la fila
                objetoConocimientoItem.addClass("d-none");
            }            
        });                        
    },

    /**
     * Método para eliminar un objeto de conocimiento una vez se ha confirmado desde el modal.
     */
    handleConfirmDeleteObjetoConocimiento: function(){
        const that = this;
        
        loadingMostrar();

        // Construir el objeto para proceder a la eliminación del objeto de conocimiento
        const dataPost = {
            // Guid/DocumentoId de la ontología
            DocumentoId: that.filaObjetoConocimiento.data("documentid"),
            // Id Objeto conocimiento
            Id: that.filaObjetoConocimiento.attr('id'),                        
        }
        
        GnossPeticionAjax(
            that.urlDeleteObjetoConocimiento,
            dataPost,
            true
        ).done(function (data) {
            // OK al borrado del item
            // Ocultar el modal del borrado
            const modalDeleteObjetoConocimiento = that.filaObjetoConocimiento.find(`.${that.modalDeleteObjetoConocimientoClassName}`);                                          
            dismissVistaModal(modalDeleteObjetoConocimiento);   
            // Ocultar la fila que se desea eliminar y eliminar
            that.filaObjetoConocimiento.addClass("d-none");                                     
            that.filaObjetoConocimiento.remove();         
            that.handleCheckNumberOfObjetosConocimiento(); 
            // Restablecer el flag de borrado de la fila eliminada
            that.confirmDeleteObjetoConocimiento = false;   
            mostrarNotificacion("success", "Los cambios se han guardado correctamente");                             
        }).fail(function (data) {
            // KO borrado de objeto de conocimiento
            mostrarNotificacion("error", data);           
        }).always(function () {
            // Ocultar loading
            loadingOcultar();
        });                  
    },



    /**
     * Método para marcar o desmarcar el objeto de conocimiento como "Eliminado" dependiendo de la elección vía Modal
     * @param {Bool} Valor que indicará si se desea eliminar o no
     */
    handleSetDeleteObjetoConocimiento: function(deleteOC){        
        const that = this;

        if (deleteOC){
            // 1 - Marcar la opción "TabEliminada" a true
            that.filaObjetoConocimiento.find('[name="TabEliminada"]').val("true");           
            // 2 - Añadir la clase de "deleted" a la fila
            that.filaObjetoConocimiento.addClass("deleted");
        }else{
            // 1 - Marcar la opción "TabEliminada" a false
            that.filaObjetoConocimiento.find('[name="TabEliminada"]').val("false");            
            // 2 - Eliminar la clase de "deleted" a la fila
            that.filaObjetoConocimiento.removeClass("deleted");
        }    
    },

    /**
     * Método para hacer la petición para mostrar el modal de creación de un objeto de conocimiento
     */
     handleLoadAddNewObjetoConocimiento: function(){
        const that = this;

        getVistaFromUrl(that.urlCreateObjetoConocimiento, 'modal-dinamic-content', '', function(result){
            if (result != requestFinishResult.ok){
                // KO al cargar la vista
                mostrarNotificacion("error", "Error al tratar de crear un nuevo objeto de conocimiento.");
                dismissVistaModal();                                                                 
            }else{
                // Inicializar el componente de dragAndDropGnoss
                that.initDragAndDropForObjetosConocimiento();                 
            }              
        });
    },

    /**
     * Método para añadir un nuevo subtipo al objeto de conocimiento
     */
    handleAddSubtipo: function(){
        const that = this;                
        // Panel listado
        const currentListSubtipos = that.filaObjetoConocimiento.find(`.${that.idAddedSubtipoListClassName}`);
        
        // Mostrar loading para petición
        loadingMostrar();

        // Realizar la petición de nuevo filtro
        GnossPeticionAjax(
            that.urlAddNewSubtipo,
            null,
            true
       ).done(function (data) {
            // OK - Añadir subtipo            
            currentListSubtipos.append(data); 
            // Obtener el último filtro añadido
            const newSubtipoAdded = currentListSubtipos.children().last();                   
            // Mostrar los detalles del nuevo item
            newSubtipoAdded.find(`.${that.btnEditSubtipoClassName}`).trigger("click");            
       }).fail(function (data) {
            // KO en creación          
           mostrarNotificacion("error", data);
       }).always(function () {
            // Ocultar loading
            loadingOcultar();           
       });
    },

    
    /**
     * Método para eliminar un subtipo de un objeto de conocimiento
     * @param {jqueryElement} btnDeleted Botón pulsado para eliminar el subtipo
     */
    handleDeleteSubtipo: function(btnDeleted){
        const that = this;    
        // Fila del subtipo a eliminar
        const subtipoRowToDelete = btnDeleted.closest($(`.${this.subtipoRowClassName}`));  
        subtipoRowToDelete.remove();
    },


    /**
     * Método para añadir una nueva propiedad. Puede ser normal o personalizada/customizada (dependiendo desde qué panel se cree)
     * @param {*} isCustomProperty 
     */
    handleAddProperty: function(isCustomProperty = false){
        const that = this;

        // Tener en cuenta si hay que añadir una propiedad normal o personalizada
        let urlAddProperty = that.urlAddNewProperty; 
        if (isCustomProperty == true){
            urlAddProperty = that.urlAddNewCustomProperty;
        }

        // Panel listado 
        const currentListProperties = that.panelConfigurarPropiedades.find(`.${that.idAddedPropertyListClassName}`);
        
        // Mostrar loading para petición
        loadingMostrar();

        // Realizar la petición de nuevo filtro
        GnossPeticionAjax(
            urlAddProperty,
            null,
            true
       ).done(function (data) {
            // OK - Añadir subtipo            
            currentListProperties.append(data); 
            // Obtener el último filtro añadido
            const newPropertyAdded = currentListProperties.children().last();
            // Mostrar los detalles del nuevo item
            newPropertyAdded.find(`.${that.btnEditPropertyClassName}`).trigger("click");
            // Habilitar los ckEditor  
            ckEditorRecargarTodos();          
       }).fail(function (data) {
            // KO en creación          
           mostrarNotificacion("error", data);
       }).always(function () {
            // Ocultar loading
            loadingOcultar();           
       });
    },

    
    /**
     * Método para eliminar una propiedad de un objeto de conocimiento
     * @param {*} btnDeleted 
     */
    handleDeleteProperty: function(btnDeleted){
        const that = this;    
        // Fila de la propiedad a eliminar
        const propertyRowToDelete = btnDeleted.closest($(`.${this.propertyRowClassName}`));  
        propertyRowToDelete.remove();
    },

    /**
    * Método que se ejecutará al cargarse la web para saber el nº de traducciones existentes
    */
    handleCheckNumberOfObjetosConocimiento: function(){        
        const that = this;
        const numberObjetosConocimiento = that.objetosConocimientoListContainer.find($(`.${that.objetoConocimientoListItemClassName}`)).length;
        // Mostrar el nº de items                
        that.numResultadosObjetosConocimiento.html(numberObjetosConocimiento);
    },  

    /**
     * Método para comprobar errores, obtener datos y prepararlos para el guardado de datos vía "handleSave"
     * @param {*} completion : Función a ejecutar cuando se realice o complete este método.
     */
    handlePrepareObjetosConocimiento: function(){        
        const that = this;

        // Resetear posibles errores previos
        this.errorsBeforeSaving = false;
        // Comprobar que no haya errores para su posterior guardado
        this.comprobarErroresGuardado();

        if (that.errorsBeforeSaving == false) {
            // No hay errores -> Recoger datos para su posterior guardado
            // that.ListaObjetosConocimiento = {};
            that.ListaObjetosConocimiento = new FormData();

            let cont = 0;
            
            /* Guardar todos los datos de todos los objetos de conocimiento
            $(`.${that.objetoConocimientoListItemClassName}`).each(function () {
                that.obtenerDatosObjetoConocimiento($(this), cont++);
            });
            */
           // Guardar una única fila (Objeto de conocimiento editado)
            that.obtenerDatosObjetoConocimiento(that.filaObjetoConocimiento, cont);
            // Proceder al guardado
            // that.handleSave(completion);
        }
    },

    /**
     * Método para comprobar que no hay errores antes de guardar los datos en backEnd
     */
    comprobarErroresGuardado: function(){
        const that = this;        

        // Comprobar nombres vacíos
        that.comprobarNombresVacios();
      
        // Comprobar nameSpaces vacíos
        if (!that.errorsBeforeSaving){
            that.comprobarNamespacesVacios();
        }

        // Comprobar nombre Grafo vacíos
        if (!that.errorsBeforeSaving){
            that.comprobarGrafoVacio();
        }        
        
        // Comprobar campos vacíos
        if (!that.errorsBeforeSaving){
            that.comprobarCamposVacios();
        }
    },

    /**
     * Método para comprobar que los nombres no estén vacíos
     */
    comprobarNombresVacios: function(){
        const that = this;
    
        // Comprobación de que el nombre es correcto
        // Comprobar todos los inputs
        // let inputsNombre = $(`.${that.objetoConocimientoListItemClassName} input[name="Name"]:not(":disabled")`); 
        // Comproba el input de la fila analizada       
        const inputsNombre = that.filaObjetoConocimiento.find(`input[name="Name"]:not(":disabled")`);    // $(`.${that.objetoConocimientoListItemClassName} input[name="Name"]:not(":disabled")`);        
        // Comprobación de los nombres
        inputsNombre.each(function () {
            if (that.errorsBeforeSaving == false){
                that.comprobarNombreVacio($(this));
            }
        });

        // Comprobar el input de la fila analizada (ShortOntology que será RutaRelativa del OC)
        const inputsShortNameOntology = that.filaObjetoConocimiento.find(`input[name="ShortNameOntology"]:not(":disabled")`);
        // Comprobación de los ShortNameOntology
        inputsShortNameOntology.each(function () {
            if (that.errorsBeforeSaving == false){
                that.comprobarNombreVacio($(this), true);
            }
        });        
    },


    /**
     * Método para comprobar que el nombre corto y ó texto no esté vacío. Habrá que tener en cuenta el multiIdioma
     * @param {*} inputName: Input a comprobar     
     * @param {bool} allowEmpty: Valor que indica que el input en cuestión puede ser vacío o no.          
     */
    comprobarNombreVacio: function (inputName, allowEmpty = false) {
        const that = this
        
        // Contiene los tabs en idiomas y el div con los inputs en idiomas completo
        const formularioEdicion = inputName.closest(".formulario-edicion");
        const panInputsMultiIdioma = inputName.closest(".inputsMultiIdioma");
        // Contenedor donde se encuentran datos básicos
        const panMultiIdioma = formularioEdicion.find(".panContenidoMultiIdioma.basicInfo");

        // Id del input a tratar
        const inputId = inputName.attr("id");

        const listaTextos = [];
        
        if (operativaMultiIdioma.listaIdiomas.length > 1 && panMultiIdioma.length > 0) {
            let textoMultiIdioma = "";
            // Comprobar que hay al menos un texto por defecto para el nombre
            let textoIdiomaDefecto = panMultiIdioma.find(`#input_${inputId}_${operativaMultiIdioma.idiomaPorDefecto}`).val(); //panMultiIdioma.find('#edicion_' + that.IdiomaPorDefecto + ' input').val();
            
            // Permitir input multiIdiomas vacíos o no.
            if (allowEmpty == false){
                if (textoIdiomaDefecto == null || textoIdiomaDefecto == "") {
                    const fila = inputName.closest(".component-wrap.objetoConocimiento-row");
                    that.mostrarErrorNombreVacio(fila, panMultiIdioma.find(`#input_${inputId}_${operativaMultiIdioma.idiomaPorDefecto}`));
                    that.errorsBeforeSaving = true;
                    return true;
                }
            }

            // Se permiten nulos y el idioma por defecto es vacío (No se desean valores)
            if (allowEmpty == true && textoIdiomaDefecto.trim() == ""){
                inputName.val("");
                return
            }
            
            // Recorrer todos los idiomas para detectar posibles problemas con el nombre                
            $.each(operativaMultiIdioma.listaIdiomas, function () {
                // Obtención del Key del idioma
                const idioma = this.key;
                // Asignar el valor por defecto de la ruta al idioma si este no dispone de valor para Nombre
                let textoIdioma = $(`#input_${inputId}_${idioma}`).val();
                if (textoIdioma == null || textoIdioma == "") {
                    textoIdioma = textoIdiomaDefecto;
                    $(`#input_${inputId}_${idioma}`).val(textoIdioma);
                }
                // Escribir el nombre del multiIdioma en el campo Hidden
                textoMultiIdioma += textoIdioma + "@" + idioma + "|||";
                
                listaTextos.push({ "key": idioma, "value": textoIdioma });
            });            
            inputName.val(textoMultiIdioma);
        }else{
            // Sin multiIdioma.
            let textoIdiomaDefecto = panMultiIdioma.find(`#input_${inputId}_${operativaMultiIdioma.idiomaPorDefecto}`).val();
            // Establecer el nombre en el input correspondiente
            inputName.val(textoIdiomaDefecto);
        }    
    },

    /**
     * Método para mostrar el error del nombre vacío encontrado . Se muestra cuando el nombre en el idioma por defecto no existe.
     * @param {jqueryObject} fila : Elemento jquery de la fila correspondiente donde se ha encontrado el error.
     * @param {jqueryObject} input : Elemento jquery del input donde se ha producido el error. Puede que ser undefined
     */
     mostrarErrorNombreVacio: function(fila, input){                              
        if (input != undefined){
            comprobarInputNoVacio(input, true, false, "Esta información no puede estar vacía.", 0);
        }
        setTimeout(function(){  
            mostrarNotificacion("error", "Esta propiedad del objeto de conocimiento no pueden estar vacía.");
        },1000);  
    },  


    /**
     * Método para mostar el error de que el campo no puede estar vacío.
     */
    mostrarErrorCampoVacio: function(input){
        // var fila = input.closest('.row');
        mostrarNotificacion("error", "Este campo no puede estar vacío."); 
        comprobarInputNoVacio(input, true, false, "Esta información no puede estar vacía.", 0);
    },

    /**
     * Método para comprobar que los nameSpace no estén vacíos
     */
    comprobarNamespacesVacios: function(){
        const that = this;
    
        // Comprobación de que el nombre de la categoría es correcto
        // Comprobar todos los inputs
        //let inputsNombre = $(`.${that.objetoConocimientoListItemClassName} input[name="Namespace"]`);      
        // Comproba el input de la fila analizada       
        let inputsNombre = that.filaObjetoConocimiento.find(`input[name="Namespace"]:not(":disabled")`);    // $(`.${that.objetoConocimientoListItemClassName} input[name="Namespace"]`);        
        
        // Comprobación
        inputsNombre.each(function () {
            if ($(this).val() == "") {
                that.mostrarErrorCampoVacio($(this));
                that.errorsBeforeSaving = true;
            }
        });
    }, 

    /**
     * Método para comprobar que los campos no estén vacíos.
     */    
    comprobarCamposVacios: function(){
        const that = this;
        
        // Comprobar que los campos de Propiedad no estén vacíos
        // Comprobar todos los inputs
        // const inputsPropiedad = $(`.${that.objetoConocimientoListItemClassName} input[name="NombreProp"]`);   // $('.row.propiedad:not(".ui-state-disabled") input[name="NombreProp"]');
        // Comprobar los inputs de la fila analizada
        const inputsPropiedad = that.filaObjetoConocimiento.find(`input[name="NombreProp"]`);
        inputsPropiedad.each(function () {
            if ($(this).val() == "") {
                that.mostrarErrorCampoVacio($(this));
                that.errorsBeforeSaving = true;
            }
        });

        /* Los campos de PresentacionProp no son requeridos. Lo desactivo de momento
        if (that.errorsBeforeSaving == false){
            // Todos los inputs
            // const inputsPresentacionProp = $(`.${that.objetoConocimientoListItemClassName} input[name="PresentacionProp"]`);// const inputsNombre = $('.row.propiedad:not(".ui-state-disabled") input[name="PresentacionProp"]');
            // Input de la fila analizada
            const inputsPresentacionProp = that.filaObjetoConocimiento.find(`input[name="PresentacionProp"]`);
            inputsPresentacionProp.each(function () {
                if ($(this).val() == "") {
                    that.mostrarErrorCampoVacio($(this));
                    that.errorsBeforeSaving = true;                    
                }
            });
        }
        */
    },

    /**
     * Método para comprobar que los campos no estén vacíos.
     */    
     comprobarGrafoVacio: function(){
        const that = this;
        
        // Comprobar que los campos de Propiedad no estén vacíos
        // Comprobar todos los inputs
        // const inputsPropiedad = $(`.${that.objetoConocimientoListItemClassName} input[name="NombreProp"]`);   // $('.row.propiedad:not(".ui-state-disabled") input[name="NombreProp"]');
        // Comprobar los inputs de la fila analizada
        const inputsPropiedad = that.filaObjetoConocimiento.find(`input[name="Grafo"]`);
        inputsPropiedad.each(function () {
            if ($(this).val() == "") {
                that.mostrarErrorCampoVacio($(this));
                that.errorsBeforeSaving = true;
            }
        });
    },    
    
    /**
     * Método para recopilar los datos de los objetos de conocimiento para su posterior guardado.
     * @param {*} fila Fila que está siendo analizada
     * @param {*} num Contador para añadir al objeto a enviar a backend
     */
    obtenerDatosObjetoConocimiento: function(fila, num){
        
        const that = this;               

        // Contenido o datos de la cláusula dentro del modal
        const panelEdicion = fila.find(`.${that.modalObjetoConocimientoClassName}`);
        // Controlar si el objeto a guardar es primario o secundario (Secundario no requiere todos los datos)
        const esObjetoPrimario = fila.data("isprimary") == true;

        // Id 
        const id = fila.attr('id');        
        
        // Prefijo para crear el objeto a enviar a backend (Crear una lista)
        //const prefijoClave = 'ListaObjetosConocimiento[' + num + ']';
        // Prefijo para crear el objeto a enviar a backend (Sin que sea una lista)
        const prefijoClave = 'ObjetoConocimiento.ObjetoConocimiento.';

        // Guardar el documentId correspondiente al objeto de conocimiento para su posterior guardado en backend
        //that.ListaObjetosConocimiento[prefijoClave + 'DocumentoId'] = fila.data("documentid"); 
        that.ListaObjetosConocimiento.append(prefijoClave + 'DocumentoId', fila.data("documentid")); 
        // Obtener datos y checkbox
        //that.ListaObjetosConocimiento[prefijoClave + 'Ontologia'] = id;
        that.ListaObjetosConocimiento.append(prefijoClave + 'Ontologia', id); 
        //that.ListaObjetosConocimiento[prefijoClave + 'Deleted'] = panelEdicion.find('[name="TabEliminada"]').val();
        that.ListaObjetosConocimiento.append(prefijoClave + 'Deleted', panelEdicion.find('[name="TabEliminada"]').val()); 
        //that.ListaObjetosConocimiento[prefijoClave + 'Name'] = panelEdicion.find('[name="Name"]').val();
        that.ListaObjetosConocimiento.append(prefijoClave + 'Name', panelEdicion.find('[name="Name"]').val());         
        
        // Datos a guardar sólo si es un Objeto primario
        if (esObjetoPrimario == true){                
            //that.ListaObjetosConocimiento[prefijoClave + 'ShortNameOntology'] = panelEdicion.find('[name="ShortNameOntology"]').val(); 
            that.ListaObjetosConocimiento.append(prefijoClave + 'ShortNameOntology', panelEdicion.find('[name="ShortNameOntology"]').val()); 
            // Nombre del grafo Actual/Nuevo       
            //that.ListaObjetosConocimiento[prefijoClave + 'GrafoNuevo'] = panelEdicion.find('[name="Grafo"]').val();
            that.ListaObjetosConocimiento.append(prefijoClave + 'GrafoNuevo', panelEdicion.find('[name="Grafo"]').val()); 
            // Nombre del grafo Anterior/Actual
            //that.ListaObjetosConocimiento[prefijoClave + 'GrafoActual'] = fila.data('grafo-actual');
            that.ListaObjetosConocimiento.append(prefijoClave + 'GrafoActual', fila.data('grafo-actual')); 
            // NameSpace, Extra y TesauroExclusivo
            //that.ListaObjetosConocimiento[prefijoClave + 'Namespace'] = panelEdicion.find('[name="Namespace"]').val();
            that.ListaObjetosConocimiento.append(prefijoClave + 'Namespace', panelEdicion.find('[name="Namespace"]').val()); 
            //that.ListaObjetosConocimiento[prefijoClave + 'NamespaceExtra'] = panelEdicion.find('[name="NamespaceExtra"]').val().trim();
            that.ListaObjetosConocimiento.append(prefijoClave + 'NamespaceExtra', panelEdicion.find('[name="NamespaceExtra"]').val().trim()); 
            //that.ListaObjetosConocimiento[prefijoClave + 'NombreTesauroExclusivo'] = "" // Ocultarlo por el momento panelEdicion.find('[name="NombreTesauroExclusivo"]').val();
            that.ListaObjetosConocimiento.append(prefijoClave + 'NombreTesauroExclusivo', ""); // Ocultarlo por el momento panelEdicion.find('[name="NombreTesauroExclusivo"]').val();

            //that.ListaObjetosConocimiento[prefijoClave + 'CachearDatosSemanticos'] = $('[name="CachearDatosSemanticos"]', panelEdicion).is(':checked');
            that.ListaObjetosConocimiento.append(prefijoClave + 'CachearDatosSemanticos', $('[name="CachearDatosSemanticos"]', panelEdicion).is(':checked')); 
            //that.ListaObjetosConocimiento[prefijoClave + 'EsBuscable'] = $('[name="EsBuscable"]', panelEdicion).is(':checked');
            that.ListaObjetosConocimiento.append(prefijoClave + 'EsBuscable', $('[name="EsBuscable"]', panelEdicion).is(':checked')); 
            // Indicar que el objeto a guardar es Objeto primario
            that.ListaObjetosConocimiento.append(prefijoClave + 'EsObjetoPrimario', true); 

            // Obtener datos de Subtipos
            const panelSubTipos = panelEdicion.find(`.${that.idAddedSubtipoListClassName}`);                              
            
            let numSubTipo = 0;
            // Obtener - revisar cada subtipo
            $('.subtipo-row', panelSubTipos).each(function () {
                // Panel de detalles
                const panSubtipo = $(this).find(".fichaSubtipo-info");            
                // Prefijo para guardado de datos
                const prefijoSubTipoClave = prefijoClave + 'Subtipos[' + numSubTipo + ']';
                // Guardado de datos
                // that.ListaObjetosConocimiento[prefijoSubTipoClave + '.Key'] = panSubtipo.find('[name="Tipo"]').val();
                that.ListaObjetosConocimiento.append(prefijoSubTipoClave + '.Key', panSubtipo.find('[name="Tipo"]').val()); 
                //that.ListaObjetosConocimiento[prefijoSubTipoClave + '.Value'] = panSubtipo.find('[name="Nombre"]').val();
                that.ListaObjetosConocimiento.append(prefijoSubTipoClave + '.Value', panSubtipo.find('[name="Nombre"]').val()); 
                numSubTipo++;
            });   
            
            // Obtener datos de Propiedades - Presentación Listado
            const panelPresentacionListado = panelEdicion.find('.PresentacionListado');
            // Prefijo para guardado de datos
            const prefijoPresentacionListadoClave = prefijoClave + 'PresentacionListado';

            let numPropListado = 0;
            $('.property-row:not(.deleted)', panelPresentacionListado).each(function () {
                // Panel de detalles
                const panPropiedadListado = $(this).find(".fichaPropiedad-info");            
                // Prefijo para guardado de datos
                const prefijoPropiedadesListadoClave = prefijoPresentacionListadoClave + '.ListaPropiedades[' + numPropListado + ']';
                // Guardado de datos                                        
                // that.ListaObjetosConocimiento[prefijoPropiedadesListadoClave + '.Orden'] = numPropListado;
                that.ListaObjetosConocimiento.append(prefijoPropiedadesListadoClave + '.Orden', numPropListado); 
                //that.ListaObjetosConocimiento[prefijoPropiedadesListadoClave + '.Propiedad'] = panPropiedadListado.find('[name="NombreProp"]').val();
                that.ListaObjetosConocimiento.append(prefijoPropiedadesListadoClave + '.Propiedad', panPropiedadListado.find('[name="NombreProp"]').val()); 
                //that.ListaObjetosConocimiento[prefijoPropiedadesListadoClave + '.Presentacion'] = encodeURIComponent(panPropiedadListado.find('[name="PresentacionProp"]').val());
                that.ListaObjetosConocimiento.append(prefijoPropiedadesListadoClave + '.Presentacion', encodeURIComponent(panPropiedadListado.find('[name="PresentacionProp"]').val())); 
                numPropListado++;
            });   
            
            //that.ListaObjetosConocimiento[prefijoPresentacionListadoClave + '.MostrarDescripcion'] = panelPresentacionListado.find('[name="MostrarDescripcion"]').is(':checked');
            that.ListaObjetosConocimiento.append(prefijoPresentacionListadoClave + '.MostrarDescripcion', panelPresentacionListado.find('[name="MostrarDescripcion"]').is(':checked')); 
            //that.ListaObjetosConocimiento[prefijoPresentacionListadoClave + '.MostrarPublicador'] = panelPresentacionListado.find('[name="MostrarPublicador"]').is(':checked');
            that.ListaObjetosConocimiento.append(prefijoPresentacionListadoClave + '.MostrarPublicador', panelPresentacionListado.find('[name="MostrarPublicador"]').is(':checked')); 
            //that.ListaObjetosConocimiento[prefijoPresentacionListadoClave + '.MostrarEtiquetas'] = panelPresentacionListado.find('[name="MostrarEtiquetas"]').is(':checked');
            that.ListaObjetosConocimiento.append(prefijoPresentacionListadoClave + '.MostrarEtiquetas', panelPresentacionListado.find('[name="MostrarEtiquetas"]').is(':checked'));         
            //that.ListaObjetosConocimiento[prefijoPresentacionListadoClave + '.MostrarCategorias'] = panelPresentacionListado.find('[name="MostrarCategorias"]').is(':checked');                
            that.ListaObjetosConocimiento.append(prefijoPresentacionListadoClave + '.MostrarCategorias', panelPresentacionListado.find('[name="MostrarCategorias"]').is(':checked')); 

            // Obtener datos de Propiedades - Presentación Mosaico
            const panelPresentacionMosaico = panelEdicion.find('.PresentacionMosaico');
            // Prefijo para guardado de datos
            const prefijoPresentacionMosaicoClave = prefijoClave + 'PresentacionMosaico';

            let numPropMosaico = 0;
            $('.property-row:not(.deleted)', panelPresentacionMosaico).each(function () {
                // Panel de detalles
                const panPropiedadMosaico = $(this).find(".fichaPropiedad-info");            
                // Prefijo para guardado de datos
                const prefijoPropiedadesMosaicoClave = prefijoPresentacionMosaicoClave + '.ListaPropiedades[' + numPropMosaico + ']';
                // Guardado de datos                                        
                //that.ListaObjetosConocimiento[prefijoPropiedadesMosaicoClave + '.Orden'] = numPropMosaico;
                that.ListaObjetosConocimiento.append(prefijoPropiedadesMosaicoClave + '.Orden', numPropMosaico); 
                //that.ListaObjetosConocimiento[prefijoPropiedadesMosaicoClave + '.Propiedad'] = panPropiedadMosaico.find('[name="NombreProp"]').val();
                that.ListaObjetosConocimiento.append(prefijoPropiedadesMosaicoClave + '.Propiedad', panPropiedadMosaico.find('[name="NombreProp"]').val()); 
                //that.ListaObjetosConocimiento[prefijoPropiedadesMosaicoClave + '.Presentacion'] = encodeURIComponent(panPropiedadMosaico.find('[name="PresentacionProp"]').val());
                that.ListaObjetosConocimiento.append(prefijoPropiedadesMosaicoClave + '.Presentacion', encodeURIComponent(panPropiedadMosaico.find('[name="PresentacionProp"]').val())); 
                numPropMosaico++;
            });   
            
            //that.ListaObjetosConocimiento[prefijoPresentacionMosaicoClave + '.MostrarDescripcion'] = panelPresentacionMosaico.find('[name="MostrarDescripcion"]').is(':checked');
            that.ListaObjetosConocimiento.append(prefijoPresentacionMosaicoClave + '.MostrarDescripcion', panelPresentacionMosaico.find('[name="MostrarDescripcion"]').is(':checked')); 
            //that.ListaObjetosConocimiento[prefijoPresentacionMosaicoClave + '.MostrarPublicador'] = panelPresentacionMosaico.find('[name="MostrarPublicador"]').is(':checked');
            that.ListaObjetosConocimiento.append(prefijoPresentacionMosaicoClave + '.MostrarPublicador', panelPresentacionMosaico.find('[name="MostrarPublicador"]').is(':checked'));         
            //that.ListaObjetosConocimiento[prefijoPresentacionMosaicoClave + '.MostrarEtiquetas'] = panelPresentacionMosaico.find('[name="MostrarEtiquetas"]').is(':checked');
            that.ListaObjetosConocimiento.append(prefijoPresentacionMosaicoClave + '.MostrarEtiquetas', panelPresentacionMosaico.find('[name="MostrarEtiquetas"]').is(':checked'));         
            //that.ListaObjetosConocimiento[prefijoPresentacionMosaicoClave + '.MostrarCategorias'] = panelPresentacionMosaico.find('[name="MostrarCategorias"]').is(':checked');        
            that.ListaObjetosConocimiento.append(prefijoPresentacionMosaicoClave + '.MostrarCategorias', panelPresentacionMosaico.find('[name="MostrarCategorias"]').is(':checked'));         


            // Obtener datos de Mapa - Presentación Mapa
            const panelPresentacionMapa = panelEdicion.find('.PresentacionMapa');
            // Prefijo para guardado de datos
            const prefijoPresentacionMapaClave = prefijoClave + 'PresentacionMapa';

            let numPropMapa = 0;
            $('.property-row:not(.deleted)', panelPresentacionMapa).each(function () {
                // Panel de detalles
                const panPropiedadMapa = $(this).find(".fichaPropiedad-info");            
                // Prefijo para guardado de datos
                const prefijoPropiedadesMapaClave = prefijoPresentacionMapaClave + '.ListaPropiedades[' + numPropMapa + ']';
                // Guardado de datos                                        
                //that.ListaObjetosConocimiento[prefijoPropiedadesMapaClave + '.Orden'] = numPropMapa;
                that.ListaObjetosConocimiento.append(prefijoPropiedadesMapaClave + '.Orden', numPropMapa);         
                //that.ListaObjetosConocimiento[prefijoPropiedadesMapaClave + '.Propiedad'] = panPropiedadMapa.find('[name="NombreProp"]').val();
                that.ListaObjetosConocimiento.append(prefijoPropiedadesMapaClave + '.Propiedad', panPropiedadMapa.find('[name="NombreProp"]').val());         
                //that.ListaObjetosConocimiento[prefijoPropiedadesMapaClave + '.Presentacion'] = encodeURIComponent(panPropiedadMapa.find('[name="PresentacionProp"]').val());
                that.ListaObjetosConocimiento.append(prefijoPropiedadesMapaClave + '.Presentacion', encodeURIComponent(panPropiedadMapa.find('[name="PresentacionProp"]').val()));         
                numPropMapa++;
            });   
            
            //that.ListaObjetosConocimiento[prefijoPresentacionMapaClave + '.MostrarDescripcion'] = panelPresentacionMapa.find('[name="MostrarDescripcion"]').is(':checked');
            that.ListaObjetosConocimiento.append(prefijoPresentacionMapaClave + '.MostrarDescripcion', panelPresentacionMapa.find('[name="MostrarDescripcion"]').is(':checked'));         
            //that.ListaObjetosConocimiento[prefijoPresentacionMapaClave + '.MostrarPublicador'] = panelPresentacionMapa.find('[name="MostrarPublicador"]').is(':checked');
            that.ListaObjetosConocimiento.append(prefijoPresentacionMapaClave + '.MostrarPublicador', panelPresentacionMapa.find('[name="MostrarPublicador"]').is(':checked'));         
            //that.ListaObjetosConocimiento[prefijoPresentacionMapaClave + '.MostrarEtiquetas'] = panelPresentacionMapa.find('[name="MostrarEtiquetas"]').is(':checked');
            that.ListaObjetosConocimiento.append(prefijoPresentacionMapaClave + '.MostrarEtiquetas', panelPresentacionMapa.find('[name="MostrarEtiquetas"]').is(':checked'));                 
            //that.ListaObjetosConocimiento[prefijoPresentacionMapaClave + '.MostrarCategorias'] = panelPresentacionMapa.find('[name="MostrarCategorias"]').is(':checked');        
            that.ListaObjetosConocimiento.append(prefijoPresentacionMapaClave + '.MostrarCategorias', panelPresentacionMapa.find('[name="MostrarCategorias"]').is(':checked'));                         

            // Obtener datos de Mapa - Presentación Personalizado
            const panelPresentacionPersonalizada = panelEdicion.find('.PresentacionPersonalizado');
            // Prefijo para guardado de datos
            const prefijoPresentacionPersonalizadaClave = prefijoClave + 'PresentacionPersonalizado';

            let numPropPersonalizada = 0;
            $('.property-row:not(.deleted)', panelPresentacionPersonalizada).each(function () {
                // Panel de detalles
                const panPropiedadPersonalizada = $(this).find(".fichaPropiedad-info");            
                // Prefijo para guardado de datos
                const prefijoPropiedadesPersonalizadasClave = prefijoPresentacionPersonalizadaClave + '.ListaPropiedades[' + numPropPersonalizada + ']';
                // Guardado de datos             
                //that.ListaObjetosConocimiento[prefijoPropiedadesPersonalizadasClave + '.Orden'] = numPropPersonalizada;
                that.ListaObjetosConocimiento.append(prefijoPropiedadesPersonalizadasClave + '.Orden', numPropPersonalizada);                         
                //that.ListaObjetosConocimiento[prefijoPropiedadesPersonalizadasClave + '.Identificador'] = encodeURIComponent(panPropiedadPersonalizada.find('[name="Identificador"]').val());
                that.ListaObjetosConocimiento.append(prefijoPropiedadesPersonalizadasClave + '.Identificador', encodeURIComponent(panPropiedadPersonalizada.find('[name="Identificador"]').val()));
                //that.ListaObjetosConocimiento[prefijoPropiedadesPersonalizadasClave + '.Select'] = encodeURIComponent(panPropiedadPersonalizada.find('[name="Select"]').val());
                that.ListaObjetosConocimiento.append(prefijoPropiedadesPersonalizadasClave + '.Select', encodeURIComponent(panPropiedadPersonalizada.find('[name="Select"]').val()));
                //that.ListaObjetosConocimiento[prefijoPropiedadesPersonalizadasClave + '.Where'] = encodeURIComponent(panPropiedadPersonalizada.find('[name="Where"]').val().trim());                        
                that.ListaObjetosConocimiento.append(prefijoPropiedadesPersonalizadasClave + '.Where', encodeURIComponent(panPropiedadPersonalizada.find('[name="Where"]').val().trim()));
                numPropPersonalizada++;
            });      
                    
            // Obtener datos de Relacionados - Presentación Relacionados
            const panelPresentacionRelacionados = panelEdicion.find('.PresentacionRelacionados');
            // Prefijo para guardado de datos
            const prefijoPresentacionRelacionadosClave = prefijoClave + 'PresentacionRelacionados';

            let numPropRelacionados = 0;
            $('.property-row:not(.deleted)', panelPresentacionRelacionados).each(function () {
                // Panel de detalles
                const panPropiedadRelacionados = $(this).find(".fichaPropiedad-info");            
                // Prefijo para guardado de datos
                const prefijoPropiedadesRelacionadosClave = prefijoPresentacionRelacionadosClave + '.ListaPropiedades[' + numPropRelacionados + ']';
                // Guardado de datos                                        
                //that.ListaObjetosConocimiento[prefijoPropiedadesRelacionadosClave + '.Orden'] = numPropRelacionados;
                that.ListaObjetosConocimiento.append(prefijoPropiedadesRelacionadosClave + '.Orden', numPropRelacionados);
                //that.ListaObjetosConocimiento[prefijoPropiedadesRelacionadosClave + '.Propiedad'] = panPropiedadRelacionados.find('[name="NombreProp"]').val();
                that.ListaObjetosConocimiento.append(prefijoPropiedadesRelacionadosClave + '.Propiedad', panPropiedadRelacionados.find('[name="NombreProp"]').val());
                //that.ListaObjetosConocimiento[prefijoPropiedadesRelacionadosClave + '.Presentacion'] = encodeURIComponent(panPropiedadRelacionados.find('[name="PresentacionProp"]').val());
                that.ListaObjetosConocimiento.append(prefijoPropiedadesRelacionadosClave + '.Presentacion', encodeURIComponent(panPropiedadRelacionados.find('[name="PresentacionProp"]').val()));
                numPropRelacionados++;
            });   
            
            //that.ListaObjetosConocimiento[prefijoPresentacionRelacionadosClave + '.MostrarDescripcion'] = panelPresentacionRelacionados.find('[name="MostrarDescripcion"]').is(':checked');
            that.ListaObjetosConocimiento.append(prefijoPresentacionRelacionadosClave + '.MostrarDescripcion', panelPresentacionRelacionados.find('[name="MostrarDescripcion"]').is(':checked'));
            //that.ListaObjetosConocimiento[prefijoPresentacionRelacionadosClave + '.MostrarPublicador'] = panelPresentacionRelacionados.find('[name="MostrarPublicador"]').is(':checked');
            that.ListaObjetosConocimiento.append(prefijoPresentacionRelacionadosClave + '.MostrarPublicador', panelPresentacionRelacionados.find('[name="MostrarPublicador"]').is(':checked'));
            //that.ListaObjetosConocimiento[prefijoPresentacionRelacionadosClave + '.MostrarEtiquetas'] = panelPresentacionRelacionados.find('[name="MostrarEtiquetas"]').is(':checked');
            that.ListaObjetosConocimiento.append(prefijoPresentacionRelacionadosClave + '.MostrarEtiquetas', panelPresentacionRelacionados.find('[name="MostrarEtiquetas"]').is(':checked'));        
            //that.ListaObjetosConocimiento[prefijoPresentacionRelacionadosClave + '.MostrarCategorias'] = panelPresentacionRelacionados.find('[name="MostrarCategorias"]').is(':checked');          
            that.ListaObjetosConocimiento.append(prefijoPresentacionRelacionadosClave + '.MostrarCategorias', panelPresentacionRelacionados.find('[name="MostrarCategorias"]').is(':checked'));                                    
        }        
    },

    /**
     * Método de guardado en el que se envían los datos del objeto de conocimiento a backend (Ontología, Name, Presentación)
     */
    handleSave: function(completion = undefined, listaObjetosConocimiento){                
        const that = this;

        // Mostrar loading
        loadingMostrar();

        GnossPeticionAjax(
            that.urlSaveObjetoConocimientoDetails,
            listaObjetosConocimiento,
            true
        ).done(function (data) {
            if (completion != undefined){
                completion(requestFinishResult.ok, data);
            }else{
                mostrarNotificacion("success", "Los cambios se han guardado correctamente");                        
                // Se desean guardar las cláusulas -> Quitar clase de "newClause" para NO eliminar la cláusula al cerrar el modal
                $(".newObjetoConocimiento").removeClass("newObjetoConocimiento");                                                        
                    // Cerrar el modal y recargar la página para recargar los cambios sólo en edición o creación                
                    setTimeout(function() {                                  
                        // Modal para cerrar                
                        const modalObjetoConocimiento = $(that.filaObjetoConocimiento).find(`.${that.modalObjetoConocimientoClassName}`);                                          
                        dismissVistaModal(modalObjetoConocimiento);                                                    
                    },1000);            
            }                                     
        }).fail(function (data) {
            if (completion != undefined){
                completion(requestFinishResult.ko, data);
            }else{   
                if (data == ""){
                    mostrarNotificacion("error", "Se ha producido un error al tratar de guardar los datos. Contacta con el administrador");
                }else{
                    mostrarNotificacion("error", data);
                }                             
            }
        }).always(function () {
            // Ocultar loading
            loadingOcultar();
        });           
    },

    /**
         * Método para obtener el fichero adjuntado para la creación del Objeto de conocimiento 
         * @param {*} inputFile Input file del que se obtendrá el fichero.
         * @returns Devuelve el fichero del inputfile. Si no hay fichero, devuelve "undefined"
         */
    getFileFromInputFile(inputFile){        
        const file = inputFile.prop("files")[0];
        return file;
    },


    /**
     * Método para guardar el objeto de conocimiento. Es el primer paso a realizar para el guardado.
     * Guardará el tipo de objeto (Primario, Secundario) y las plantillas asociadas.      
     * @param {*} isNewObjetoConocimiento Indicar si se desea crear un nuevo oc. En caso contrario, solo recopilará los datos de la plantilla en la variable "that.dataPostPlantillasObjetoConocimiento"
     * @returns 
     */    
    handleGetTemplateFilesForObjetoConocimiento: function(isNewObjetoConocimiento){         
        const that = this;

        // Flag para detectar posibles errores al guardar datos
        that.errorsBeforeSaving = false;        

        const prefijo = isNewObjetoConocimiento == true ? "" : "ObjetoConocimiento.";

        // Objeto a enviar para guardar datos en backend con los datos de las Plantillas o ficheros 
        that.dataPostPlantillasObjetoConocimiento = new FormData();

        // Obtener el tipo de objeto de conocimiento (Primario / Secundario)
        const objetoConocimientoType = that.currentModalOC.find($(`input[type=radio][name=${this.crearObjetoConocimientoName}]:checked`)).val();
        // Botón para guardar el objeto de conocimiento
        const buttonSave = that.currentModalOC.find($(`.${that.btnSaveNewObjetoConocimientoClassName}`));
        
        // Asignar nuevo Objeto de conocimiento. Indicarlo cuando se va a crear el objeto en primera instancia. (Principal o Secundario)
        if (isNewObjetoConocimiento) {
            if (objetoConocimientoType == "primario"){
                that.dataPostPlantillasObjetoConocimiento.append(`${prefijo}Ontologia.Principal`, true);
            }else{
                that.dataPostPlantillasObjetoConocimiento.append(`${prefijo}Ontologia.Principal`, false);
            }            
        }
        
        // Owl
        this.inputFileOwlFile = that.currentModalOC.find($(`.${that.objetoConocimientoOwlFileClassName}`));        
        // Html
        this.inputFileHtmlFile = that.currentModalOC.find($(`.${that.objetoConocimientoHtmlFileClassName}`));        
        // Icono
        this.inputFileIconFile = that.currentModalOC.find($(`.${that.objetoConocimientoIconoFileClassName}`));        
        // CSS
        this.inputFileCssFile = that.currentModalOC.find($(`.${that.objetoConocimientoCssFileClassName}`)); 
        // JS
        this.inputFileJsFile = that.currentModalOC.find($(`.${that.objetoConocimientoJsFileClassName}`)); 
        

        const isNecessaryUpdateOwlTemplate = that.currentModalOC.find(`.${that.btnCancelObjetoConocimientoOwlTemplateClassName}`).hasClass("replace");
        // Obtener el fichero Owl
        const owlFile = that.getFileFromInputFile(this.inputFileOwlFile);
        if (owlFile == undefined && isNecessaryUpdateOwlTemplate){
            mostrarNotificacion("error", "Es necesario seleccionar un fichero owl para la creación del objeto de conocimiento.");
            that.errorsBeforeSaving = true;
            return;
        }        
        that.dataPostPlantillasObjetoConocimiento.append(`${prefijo}${this.inputFileOwlFile.attr('name')}`, owlFile);
        
        //Obtener el fichero Html/Xml
        const replaceXmlName = that.currentModalOC.find(`.${this.panelObjetoConocimientoHtmlTemplateClassName}`).find(`.${this.chkEditObjetoConocimientoClassName}`).attr("name"); 
        const replaceXmlValue = that.currentModalOC.find(`.${this.panelObjetoConocimientoHtmlTemplateClassName}`).find(`.${this.chkEditObjetoConocimientoClassName}`).is(":checked"); 
        const useGenericXmlName = that.currentModalOC.find(`.${this.panelObjetoConocimientoHtmlTemplateClassName}`).find(`.${this.utilizarArchivoGenericoHtmlClassName}`).attr("name");
        const useGenericXmlValue = that.currentModalOC.find(`.${this.panelObjetoConocimientoHtmlTemplateClassName}`).find(`.${this.utilizarArchivoGenericoHtmlClassName}`).is(":checked");

        // Asignar datos Html/Xml
        that.dataPostPlantillasObjetoConocimiento.append(`${prefijo}${replaceXmlName}`, replaceXmlValue);
        if (replaceXmlValue || isNewObjetoConocimiento) {
            const usarGenricoXML = useGenericXmlValue;
            // Por defecto lo dejo como false
            // var noUsarXML = $('#chkNoUseXML').is(':checked');
            const noUsarXML = false;
            that.dataPostPlantillasObjetoConocimiento.append(`${prefijo}${useGenericXmlName}`, useGenericXmlValue);
            //dataPost.append($('#chkNoUseXML').attr('name'), noUsarXML);
            that.dataPostPlantillasObjetoConocimiento.append(`${prefijo}Ontologia.NoUseXML`, noUsarXML);
            
            if (!usarGenricoXML && !noUsarXML) {
                const xmlFile = that.getFileFromInputFile(this.inputFileHtmlFile);
                if (xmlFile == undefined){
                    mostrarNotificacion("error", "Es necesario seleccionar un fichero XML para la creación del objeto de conocimiento.");
                    that.errorsBeforeSaving = true; 
                    return;
                }  
                that.dataPostPlantillasObjetoConocimiento.append(`${prefijo}${this.inputFileHtmlFile.attr('name')}`, xmlFile);
            }
        } 

        //Obtener el fichero Css
        const replaceCssName = that.currentModalOC.find(`.${this.panelObjetoConocimientoCssTemplateClassName}`).find(`.${this.chkEditObjetoConocimientoClassName}`).attr("name"); 
        const replaceCssValue = that.currentModalOC.find(`.${this.panelObjetoConocimientoCssTemplateClassName}`).find(`.${this.chkEditObjetoConocimientoClassName}`).is(":checked"); 
        const useGenericCssName = that.currentModalOC.find(`.${this.panelObjetoConocimientoCssTemplateClassName}`).find(`.${this.utilizarArchivoGenericoCssClassName}`).attr("name");
        const useGenericCssValue = that.currentModalOC.find(`.${this.panelObjetoConocimientoCssTemplateClassName}`).find(`.${this.utilizarArchivoGenericoCssClassName}`).is(":checked");

        // Asignar datos CSS
        that.dataPostPlantillasObjetoConocimiento.append(`${prefijo}${replaceCssName}`, replaceCssValue);
        if (replaceCssValue || isNewObjetoConocimiento) {
            const usarGenricoCss = useGenericCssValue;
            // Por defecto lo dejo como false
            // var noUsarXML = $('#chkNoUseXML').is(':checked');
            const noUsarCss = false;
            that.dataPostPlantillasObjetoConocimiento.append(`${prefijo}${useGenericCssName}`, useGenericCssValue);
            
            if (!usarGenricoCss && !noUsarCss) {
                const cssFile = that.getFileFromInputFile(this.inputFileCssFile);
                if (cssFile == undefined){
                    mostrarNotificacion("error", "Es necesario seleccionar un fichero CSS para la creación del objeto de conocimiento.");
                    that.errorsBeforeSaving = true; 
                    return;
                }  
                that.dataPostPlantillasObjetoConocimiento.append(`${prefijo}${this.inputFileCssFile.attr('name')}`, cssFile);
            }
        }   
        
        //Obtener el fichero Icono
        const replaceIconoName = that.currentModalOC.find(`.${this.panelObjetoConocimientoIconoImageTemplateClassName}`).find(`.${this.chkEditObjetoConocimientoClassName}`).attr("name"); 
        const replaceIconoValue = that.currentModalOC.find(`.${this.panelObjetoConocimientoIconoImageTemplateClassName}`).find(`.${this.chkEditObjetoConocimientoClassName}`).is(":checked"); 
        const useGenericIconoName = that.currentModalOC.find(`.${this.panelObjetoConocimientoIconoImageTemplateClassName}`).find(`.${this.utilizarArchivoGenericoIconoClassName}`).attr("name");
        const useGenericIconoValue = that.currentModalOC.find(`.${this.panelObjetoConocimientoIconoImageTemplateClassName}`).find(`.${this.utilizarArchivoGenericoIconoClassName}`).is(":checked");

        // Asignar datos Icono
        that.dataPostPlantillasObjetoConocimiento.append(`${prefijo}${replaceIconoName}`, replaceIconoValue);
        if (replaceIconoValue || isNewObjetoConocimiento) {
            const usarGenricoIcono = useGenericIconoValue;
            // Por defecto lo dejo como false
            // var noUsarXML = $('#chkNoUseXML').is(':checked');
            const noUsarIcono = false;
            that.dataPostPlantillasObjetoConocimiento.append(`${prefijo}${useGenericIconoName}`, useGenericIconoValue);
            
            if (!usarGenricoIcono && !noUsarIcono) {
                const iconFile = that.getFileFromInputFile(this.inputFileIconFile);
                if (iconFile == undefined){
                    mostrarNotificacion("error", "Es necesario seleccionar un icono para la creación del objeto de conocimiento.");
                    that.errorsBeforeSaving = true; 
                    return;
                }  
                that.dataPostPlantillasObjetoConocimiento.append(`${prefijo}${this.inputFileIconFile.attr('name')}`, iconFile);
            }
        }  
        
        //Obtener el fichero JS
        const replaceJsName = that.currentModalOC.find(`.${this.panelObjetoConocimientoJsTemplateClassName}`).find(`.${this.chkEditObjetoConocimientoClassName}`).attr("name"); 
        const replaceJsValue = that.currentModalOC.find(`.${this.panelObjetoConocimientoJsTemplateClassName}`).find(`.${this.chkEditObjetoConocimientoClassName}`).is(":checked"); 
        const useGenericJsName = that.currentModalOC.find(`.${this.panelObjetoConocimientoJsTemplateClassName}`).find(`.${this.utilizarArchivoGenericoJsClassName}`).attr("name");
        const useGenericJsValue = that.currentModalOC.find(`.${this.panelObjetoConocimientoJsTemplateClassName}`).find(`.${this.utilizarArchivoGenericoJsClassName}`).is(":checked");

        // Asignar datos JS
        that.dataPostPlantillasObjetoConocimiento.append(`${prefijo}${replaceJsName}`, replaceJsValue);
        if (replaceJsValue || isNewObjetoConocimiento) {  
            const usarGenericoJs = useGenericJsValue;          
            // Por defecto lo dejo como false
            // var noUsarXML = $('#chkNoUseXML').is(':checked');
            const noUsarJs = false;
            that.dataPostPlantillasObjetoConocimiento.append(`${prefijo}${useGenericJsName}`, useGenericJsValue);
            
            if (!usarGenericoJs && !noUsarJs) {
                const jsFile = that.getFileFromInputFile(this.inputFileJsFile);
                if (jsFile == undefined){
                    mostrarNotificacion("error", "Es necesario seleccionar un fichero js para la creación del objeto de conocimiento.");
                    that.errorsBeforeSaving = true; 
                    return;
                }  
                that.dataPostPlantillasObjetoConocimiento.append(`${prefijo}{this.inputFileJsFile.attr('name')}`, jsFile);
            }
        }
        
        // Proceder a guardar el OC
        if (isNewObjetoConocimiento && that.errorsBeforeSaving == false){
            // Guardar el nuevo objeto de conocimiento
            that.handleSaveNewObjetoConocimiento();
        }/*else{
            // Guardar el objeto de conocimiento editado (Plantillas)
            that.handleSaveObjetoConocimientoFicheros();            
        }*/
    },
    
    /**
     * Método para guardar los datos del objeto de conocimiento (existente) pero únicamente sus plantillas (Datos de that.dataPostPlantillasObjetoConocimiento)
     */
    handleSaveObjetoConocimientoFicheros: function(){
        const that = this;    
        
        loadingMostrar();

        GnossPeticionAjax(
            that.urlSaveObjetoConocimientoFicheros,            
            that.dataPostPlantillasObjetoConocimiento,
            true
        ).done(function (data) {
            mostrarNotificacion("success", "Los cambios se han guardado correctamente.");                                            
        }).fail(function (data) {
            mostrarNotificacion("error", data);            
        }).always(function () {
            loadingOcultar();
        });
    }, 

    /**
     * Método para proceder a guardar un nuevo objeto de conocimiento. Se creará un objeto de conocimiento desde 0 con los datos de las plantillas seleccionado 
     * del usuario vía modal.
     */
    handleSaveNewObjetoConocimiento: function(){
        const that = this;    
        
        loadingMostrar();

        GnossPeticionAjax(
            that.urlSaveNewObjetoConocimiento,            
            that.dataPostPlantillasObjetoConocimiento,
            true
        ).done(function (data) {
            loadingOcultar();
            mostrarNotificacion("success", "Los cambios se han guardado correctamente.");            
            // Ocultar el modal-container                                       
            setTimeout(function() {
                dismissVistaModal();                            
            },500);
            // Añadir la fila al listado de objetos de conocimiento
            that.objetosConocimientoListContainer.append(data);
            // Recargar funcionalidad inicial (plugins)
            that.triggerEvents();
            // Obtener la última fila correspondiente con el nuevo objeto de conocimiento añadido
            const objetoConocimientoNewRow = that.objetosConocimientoListContainer.children().last();
            // Guardar en variable la nueva fila del elemento creado
            that.filaObjetoConocimiento = objetoConocimientoNewRow;
            // Hacer click para abrir el modal de edición
            objetoConocimientoNewRow.find(`.${that.btnEditObjetoConocimientoClassName}`).trigger("click");
            // Establecer tab activo para visualizar las opciones avanzadas del objeto de conocimiento            
            that.handleSetTabActiveForSeeDetails(that.filaObjetoConocimiento);            
            // Asignar el modal actual         
            that.currentModalOC = objetoConocimientoNewRow.find(`.${that.modalObjetoConocimientoClassName}`);
                           
        }).fail(function (data) {
            loadingOcultar();
            mostrarNotificacion("error", data);                    
        });
    },

    /**
     * Método para establecer una pestaña como activa al pulsar sobre "Editar objeto de conocimiento"
     * @param {*} pObjetoConocimientoRow: Fila del objeto de conocimiento que se está editando o sobre el que se desea editar la información
     */
    handleSetTabActiveForSeeDetails: function(pObjetoConocimientoRow){        
        const that = this;
        // Comprobar el tipo de objeto de conocimiento (Primario o Secundario)
        if (pObjetoConocimientoRow.data("isprimary") == true){
            // Dejar marcado el tab para visualizar la "Configuración" del objeto de conocimiento dependiendo del tipo          
            pObjetoConocimientoRow.find(`.${that.tabConfiguracionObjetoConocimientoClassName}`).trigger("click");
        }else{
            // Dejar marcado el tab de "Elementos"
            pObjetoConocimientoRow.find(`.${that.tabConfiguracionElementosObjetoConocimientoClassName}`).trigger("click");
            // Comprobar y hacer petición para cargar los elementos del Objeto de Conocimiento secundario                    
            if (!stringToBoolean(pObjetoConocimientoRow.data("download-items"))){                                
                const graphName = pObjetoConocimientoRow.data("grafo-actual");
                that.handleGetElementItems(graphName);                    
                // Ocultar para este panel el botón de guardado general
                that.handleShowHideGeneralSaveButton(true);
            }                    
        }                
    },

    /**
     * Método para guardar el objeto de conocimiento. Este objeto de conocimiento ya existe, y se procederá a guardar su edición.
     */    
    handleSaveObjetoConocimiento: function(){
        const that = this;
        // Limpiar posibles errores anteriores
        that.errorsBeforeSaving = false;
        // Objeto para guardar la edición del objeto de conocimiento y la ontología
        that.dataPostEditarObjetoConocimientoYOntologiaModel = new FormData();

        // Flag por defecto para detectar si hace falta guardar datos de OC.
        let isNecessaryUpdateOntology = false;

        // Comprobar si es necesario guardar datos de plantillas del OC --> Coger los datos de la plantilla
        const isNecessaryUpdateOwlTemplate = that.filaObjetoConocimiento.find(`.${that.btnCancelObjetoConocimientoOwlTemplateClassName}`).hasClass("replace");
        const isNecessaryUpdateHtmlTemplate = that.filaObjetoConocimiento.find(`.${that.btnCancelObjetoConocimientoHtmlTemplateClassName}`).hasClass("replace");
        const isNecessaryUpdateIconoTemplate = that.filaObjetoConocimiento.find(`.${that.btnCancelObjetoConocimientoIconoTemplateClassName}`).hasClass("replace");
        const isNecessaryUpdateCssTemplate = that.filaObjetoConocimiento.find(`.${that.btnCancelObjetoConocimientoCssTemplateClassName}`).hasClass("replace");
        const isNecessaryUpdateJsTemplate = that.filaObjetoConocimiento.find(`.${that.btnCancelObjetoConocimientoJsTemplateClassName}`).hasClass("replace");

        // Si alguna plantilla se desea cambiar recoger los datos para hacer posteriormente el guardado
        if (isNecessaryUpdateOwlTemplate || isNecessaryUpdateHtmlTemplate || isNecessaryUpdateIconoTemplate || isNecessaryUpdateCssTemplate || isNecessaryUpdateJsTemplate){
            // Cargar los datos de las plantillas
            that.handleGetTemplateFilesForObjetoConocimiento(false);
            // Indicar que se desean guardar los ficheros ya que estos han cambiado   
            isNecessaryUpdateOntology = true;
            // Pasar el Name de la ontología            
            that.dataPostEditarObjetoConocimientoYOntologiaModel.append("ObjetoConocimiento.Ontologia.Name", that.filaObjetoConocimiento.find('[name="Name"]').val());
        }

        // Indicar si se han modificado los ficheros
        that.dataPostEditarObjetoConocimientoYOntologiaModel.append("ObjetoConocimiento.OntologiaModificada", isNecessaryUpdateOntology);
        that.dataPostEditarObjetoConocimientoYOntologiaModel.append("ObjetoConocimiento.ObjetoConocimientoModificado", true);
        // Añadir el OntologyID
        const ontologyId = that.filaObjetoConocimiento.data("documentid");
        that.dataPostEditarObjetoConocimientoYOntologiaModel.append("ObjetoConocimiento.Ontologia.OntologyID", ontologyId);
        
        // Descartar que no haya problemas
        if (that.errorsBeforeSaving == true){
            return;
        }

        // Construir datos de Ontologia en EditarObjetoConocimientoYOntologiaModel (sólo si es necesario)
        if (isNecessaryUpdateOntology == true){
            for(let pair of that.dataPostPlantillasObjetoConocimiento.entries()) {
                that.dataPostEditarObjetoConocimientoYOntologiaModel.append(pair[0], pair[1]);                                
            }           
        }
        
        // Construir el EditarObjetoConocimientoYOntologiaModel con las propiedades del Objeto de conocimiento
        that.handlePrepareObjetosConocimiento();        
        for(let pair of that.ListaObjetosConocimiento.entries()) {
            that.dataPostEditarObjetoConocimientoYOntologiaModel.append(pair[0], pair[1]);                                
        }  
        
        // Proceder al guardado de los datos
        if (that.errorsBeforeSaving == false){
            that.handleSave(function(result, data){
                if (result == requestFinishResult.ok){               
                    //Cerramos el modal cuadno los cambios hayan sido correctos
                    const modalObjetoConocimiento = $(that.filaObjetoConocimiento).find(`.${that.modalObjetoConocimientoClassName}`);
                    dismissVistaModal(modalObjetoConocimiento);
                    setTimeout(function() {                                  
                        // Mostrar mensaje datos guardados correctamente
                        mostrarNotificacion("success", "Los cambios se han guardado correctamente");
                    },1000);                                      
                }else{
                    mostrarNotificacion("error", data);
                }
            }, that.dataPostEditarObjetoConocimientoYOntologiaModel);
        }         
    },
    
    /**
     * Método para mostrar u ocultar el panel para editar un fichero de objeto de conocimiento.
     * Establecerá también el inputCheck al valor correspondiente según el estado del panel
     */
    handleHideShowPanelToEditObjetoConocimiento: function(button){        
        const that = this;
        
        // Panel padre de todos los elementos de la plantilla
        const panelObjetoConocimientoTemplate = button.closest( `.${this.panelObjetoConocimientoTemplateClassName}` );
        
        // Panel del objeto de conocimiento a mostrar/ocultar
        const panelObjetoConocimientoFiles = panelObjetoConocimientoTemplate.find(`.${this.panelObjetoConocimientoFilesClassName}`);

        // Checkbox para indicar si se desea o no editar el fichero
        const checkEditFile = panelObjetoConocimientoTemplate.find(`.${that.chkEditObjetoConocimientoClassName}`);
        // Cambiar el estado actual 
        checkEditFile.prop("checked", !checkEditFile.is(":checked"));        
        const checkEditFileValue = panelObjetoConocimientoTemplate.find(`.${that.chkEditObjetoConocimientoClassName}`).is(":checked");
        
        if (checkEditFileValue == true){            
            panelObjetoConocimientoFiles.removeClass("d-none");           
            // Cambiar el título del botón
            button.html("Cancelar");
            button.addClass("replace");
        }else{
            // Ocultar el panel del fichero
            panelObjetoConocimientoFiles.addClass("d-none");
            button.html("Reemplazar archivo");
            button.removeClass("replace");                       
        }        
    },
    
    /**
     * Método para proceder al guardado de un elemento de una entidad secundaria que ha sido creado nuevo o editado
     * @param {*} arg : Objeto de argumentos a enviar para el guardado del elemento nuevo o a editar
     * @param {bool} isNewElement : Valor booleano que indica si el elemento a guardar es nuevo. Si no lo es, se desea editar el item.     
     * @param {bool} deleteElement : Valor booleano que indica si la acción a realizar es el borrado del elemento. Valor por defecto es false.
     */
    handleSaveEntSec: function(arg, isNewElement, pElementRow ,deleteElement = false){
        const that = this;

        loadingMostrar();

        // Url donde se realizará la acción de guardado (Editar o guardar)
        let urlEndPoint = that.urlSaveSecondaryEntityDetails;
        // Url endpoint a construir dependiendo de la acción a ejecutar
        if (!deleteElement){
            arg.ElementoNuevo = isNewElement;
        }else{
            urlEndPoint = that.urlDeleteSecondaryEntity;            
        }   
                       
        GnossPeticionAjax(urlEndPoint, arg, true)
        .done(function (data) {            
            // OK Guardado correcto
            if (isNewElement){
                // Guardado correcto de Nuevo Elemento
                // Quitar el flag de la fila que identifica que era de reciente creación
                pElementRow.data("new-element","false");                 
                mostrarNotificacion("success","El elemento se ha guardado correctamente");                   
            }else if(deleteElement){
                // Guardado correcto de Elemento eliminado
                // Eliminar el row                
                pElementRow.fadeOut(300, function() { $(this).remove(); });
                // Elemento borrado correctamente
                mostrarNotificacion("success","El elemento se ha borrado correctamente");  
            }else{
                // Guardado correcto de Elemento Editado
                // Elemento editado correctamente
                mostrarNotificacion("success","El elemento se ha guardado correctamente");  
                // Ocultar el mensaje de "Editando" ya que el item ha sido guardado correctamente                
                const labelElementExtraInfo = pElementRow.find(`.${that.labelElementExtraInfoClassName}`);            
                // Ocultar el mensaje de "Editando" ya que el item ha sido guardado correctamente                                
                labelElementExtraInfo.addClass("d-none");                   
            }
            if (!deleteElement){
                // Actualizar datos del elemento si se ha editado o creado nuevo
                pElementRow.data("sujeto-entidad",arg.EntitySubject) ;
                const labelName = pElementRow.find(`.${that.labelElementNameClassName}`);
                pElementRow.data("nombre-entidad",labelName.html().trim());
                // Ocultar el mensaje de "Editando" ya que el item ha sido guardado correctamente                
                const labelElementExtraInfo = pElementRow.find(`.${that.labelElementExtraInfoClassName}`);            
                // Ocultar el mensaje de "Editando" ya que el item ha sido guardado correctamente                                                
                labelElementExtraInfo.addClass("d-none");
                // Añadir el flag como si los datos se hubieran descargado (Evitar carga del item al hacer click en "Edit")
                pElementRow.data("download-items","true");                
            }
        }).fail(function (data) {
            // KO Guardado
            mostrarNotificacion("error", data);
        }).always(function () {
            // Actualizar el nº de entidades del objeto de conocimiento secundario
            setTimeout(function(){
                that.handleCheckNumberOfEntities();
            },1000)            
            loadingOcultar();
        });        
    }, 

    /**
     * Método para actualizar el título donde se muestra el nº de entidades que hay en un objeto de conocimiento secundario
     */
    handleCheckNumberOfEntities: function(){
        const that = this;
                
        // Título donde se muestran los resultados del objeto de conocimiento secundario a actualizar
        const labelResultados = that.filaObjetoConocimiento.find(`.${that.numResultadosEntitiesClassName}`);
        // Mostrar el nº de items
        const numberResultados = that.filaObjetoConocimiento.find($(`.${that.elementRowClassName}`)).length;
        labelResultados.html(numberResultados);
    },

    /* Obtención de los datos y guardado de elementos secundarios de objetos de conocimiento secundarios */
    /***************************************************************************************************** */
    GuardarEntSec: function(rowElementoEntidadSecundaria) {
        const that = this;

        // Flag para determinar si hay que crear un nuevo elemento o no. Por defecto sí.
        let isNewElement = false;

        // Restablecer el flag del guardado de datos
        that.errorsBeforeSavingElement = false;

        if (!that.RecogerValoresRDF(true, rowElementoEntidadSecundaria)) {
            //CrearErrorEditEntSec(mensajeErrorFormSemPrinc);
            mostrarNotificacion("error", "ERROR");
            // Flag que indica que hay algún error para no hacer guardado
            that.errorsBeforeSavingElement = true;
            return;
        }
                    
        // Botón de "Añadir elemento" para añadir un elemento nuevo a un objeto de conocimiento secundario        

        const inputSujEntSec = rowElementoEntidadSecundaria.find(`.${that.sujEntidadSecClassName}`);
        // Comprobar que existe un sujeto en el item que se desea guardar. Comprobar el sujeto         
        comprobarInputNoVacio(inputSujEntSec, true, false, "El identificador del elemento no puede estar vacío.", 0);                        
        if ( inputSujEntSec.val().trim() == "" || inputSujEntSec.val().indexOf("/") != -1) {
            input.trigger("blur");
            that.errorsBeforeSavingElement = true;
            return;
        }
    
        if (that.errorsBeforeSavingElement == false){        
            // ArguMentos a enviar para petición
            const arg = {};
        
            if (rowElementoEntidadSecundaria.data("new-element") == true){
                // Guardar nuevo elemento
                isNewElement = true;
            }
        
            // Obtener valorRdf para envío vía petición
            if (that.txtValorRdf.length > 0){
                arg.Rdf = Encoder.htmlEncode(that.txtValorRdf.val());
            }
            // Sujeto de la entidad
            arg.EntitySubject = inputSujEntSec.val();
            arg.SujetoEntidad = inputSujEntSec.val();
            arg.Grafo = that.filaObjetoConocimiento.data("grafo-actual");

            ///////////// No utilizado para el nuevo método
            ///////////// arg.SelectedInstances = that.instanciaActual + ",";
            // Indicar si el elemento es nuevo o no
            arg.ElementoNuevo = isNewElement;
                                
            that.handleSaveEntSec(arg, isNewElement, rowElementoEntidadSecundaria, false);
        }
    },

    /**
     * Método para recoger los valores RDF. Es llamado desde GuardarEntSec
     * @param {*} pValidarCampos : Comprobar la validación de los campos. Por defecto "true".
     * @param {*} pRowElementoEntidadSecundaria : Fila del elemento que del que se desea obtener valores RDF.
     * @returns 
     */
    RecogerValoresRDF: function(pValidarCampos = true, pRowElementoEntidadSecundaria) {
        const that = this;
        // Por defecto, false
        that.comprobarIdiomasRellenos = false;
        that.hayPropLangBusqCom = false;            
        that.txtValorRdf = pRowElementoEntidadSecundaria.find("#mTxtValorRdf");
        that.txtRegistroIDs = pRowElementoEntidadSecundaria.find("#mTxtRegistroIDs");
        that.txtCaracteristicasElem = pRowElementoEntidadSecundaria.find("#mTxtCaracteristicasElem");
        that.txtElemEditados = pRowElementoEntidadSecundaria.find("#mTxtElemEditados");
        // Objeto que se enviará para guardado. Instanciarlo aquí en la recogida.
        that.instanciaActual = null;
        // Recoger valores RDFInt a partir de los datos
        const ok = that.RecogerValoresRDFInt(that.txtValorRdf, that.txtRegistroIDs, that.txtCaracteristicasElem, that.txtElemEditados, pValidarCampos, pRowElementoEntidadSecundaria);

        
        // Recolección de datos con multiIdiomas
        if (ok && that.hayPropLangBusqCom) {
            that.comprobarIdiomasRellenos = true;
            that.ObtenerIdiomasUsadosRecurso();
            ok = RecogerValoresRDFInt(that.txtValorRdf, that.txtRegistroIDs, that.txtCaracteristicasElem, that.txtElemEditados, pValidarCampos);
        }
    
        /* Ocultar de momento este control para devTools
        if (ok && !pValidarCampos) {
            //Si no hay que validar los campos, valido que por lo menos ha introducido el título
            $.each(TxtTitulosSem.split(','), function (index, value) {
                if ($('#' + value).is(":visible") && $('#' + value).val() == '') {
                    ok = false;
                    mensajeErrorFormSemPrinc = form.errordtitulo;
                }
            });
        }
        */
    
        return ok;

    },



    /**
     * Método segundo para la recolección de datos RDF. Es llamado desde RecogerValoresRDF
     * @param {*} pTxtValores 
     * @param {*} pTxtIDs 
     * @param {*} pTxtCaract 
     * @param {*} pTxtElemEditados 
     * @param {*} pValidar 
     * @param {*} pRowElementoEntidadSecundaria
     * @returns 
     */
    RecogerValoresRDFInt: function(pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados, pValidar, pRowElementoEntidadSecundaria){  
        const that = this;  
        mensajeErrorFormSemPrinc = textoFormSem.algunCampoMalIntro;
        /* Quitar control de si hay propEditandose
        if (document.getElementById(TxtElemEditados).value != '') {
            mensajeErrorFormSemPrinc = textoFormSem.hayPropEditandose;
            return false;
        }
        */
        // Comprobación de que no hay subclases incompletas a partir de un "SELECT"
        let subClasesIncompletas = false;
        if(pRowElementoEntidadSecundaria.find('.cmbSubClass').length > 0){
            pRowElementoEntidadSecundaria.find('.cmbSubClass').each(function(){
                var that = $(this);
                if(that.val() == ""){
                    subClasesIncompletas = true;
                }
            });
        }

        // Controlar error subClases incompletas
        if(subClasesIncompletas){
            mostrarNotificacion("error", "No puede haber subclases incompletas en el elemento del objeto secundario.");
            that.errorsBeforeSavingElement = true;
            return;
        }
    
        // Recoger la propiedad caract        
        let caract = pTxtCaract.val();// document.getElementById(pTxtCaract).value;
        let camposCorrectos = true;
        let propErrorSemCms = '';
        let propLanguaje = that.GetPropLangBusqCom();

        caract = caract.substring(caract.indexOf('|') + 1); //Eliminio 1 parametro
        // Obtener características y recorrerlas
        const caracteristicas = caract.split('|');

        for (let i=0; i<caracteristicas.length; i++)
        {
            
            const entProp = that.ObtenerNombreEntPropRestricciones(caracteristicas[i]);
            const entidad = entProp.split(',')[0];
            const propiedad = entProp.split(',')[1];
            const tipoProp = that.GetTipoPropiedad(entidad, propiedad, pTxtCaract);
    
            if (that.HayQueRecogerValorProp(caracteristicas[i], entidad))
            {                
                let valorProp = that.ObtenerValorEntidadProp(entProp, pTxtIDs, pTxtCaract);
    
                if (tipoProp == 'FD' || tipoProp == 'CD')
                {
                    let valorAgregado = false;
                    let comprobarCampoVacio = true;
                    
                    if (typeof valorProp != 'undefined' && valorProp != null && that.EsPropiedadMultiIdioma(entidad, propiedad)) {
                        //Agregamos el valor actual por si es el de la pestaña actual, así no falla la validación
                        let valorPropMulti = that.GetValorEncode(valorProp);                        
                        valorPropMulti = that.ObtenerValorMultiIdiomaPesanyaActual(entidad, propiedad, valorPropMulti);                        
                        that.AgregarValorAPropiedad(entidad, propiedad, valorPropMulti, pTxtValores);
                        valorAgregado = true;
                        comprobarCampoVacio = !(valorPropMulti != null && valorPropMulti != '');
                    }
    
                    var campoCorrecto = true;
                    if (pValidar)
                    {                                                
                        campoCorrecto = that.ComprobarValorCampoCorrectoInt(entidad, propiedad, valorProp, pTxtIDs, pTxtCaract, comprobarCampoVacio) &&                         
                        that.ComprobarMultiIdiomaCorrecto(entidad, propiedad, tipoProp);
                        camposCorrectos = camposCorrectos && campoCorrecto;                        
                    }
    
                    if (campoCorrecto && valorProp != null && !valorAgregado)
                    {
                        var valorDefNoSelec = that.GetValorDefNoSelec(entidad, propiedad, pTxtCaract);
                        if (valorDefNoSelec == null || valorDefNoSelec == '' || valorDefNoSelec != valorProp) {
                            valorProp = that.GetValorEncode(valorProp);
                            that.AgregarValorAPropiedad(entidad, propiedad, valorProp, pTxtValores);
                        } else if (valorDefNoSelec == null || valorDefNoSelec == '' || valorDefNoSelec == valorProp) {
                            that.AgregarValorAPropiedad(entidad, propiedad, '', pTxtValores);
                        }
                    }
    
                    if (!campoCorrecto) {
                        propErrorSemCms += propiedad + ',' + entidad + '|';
                    }
                }
                else if (tipoProp == 'VD')
                {                    
                    that.DeleteElementoGuardado(entidad, propiedad, pTxtValores, pTxtElemEditados, 0); 
                    var valorAntiguo = that.GetValorElementoGuardado(entidad, propiedad, pTxtValores, pTxtElemEditados, 0);
                    while (valorAntiguo != null && valorAntiguo != '')
                    {
                        that.DeleteElementoGuardado(entidad, propiedad, pTxtValores, pTxtElemEditados, 0);                        
                        valorAntiguo = that.GetValorElementoGuardado(entidad, propiedad, pTxtValores, pTxtElemEditados, 0);
                    }
                
                    var valoresProp = valorProp.split(',');
                    for (var j=0;j<valoresProp.length;j++)
                    {
                        if (valoresProp[j] != '')
                        {                                                        
                            that.PutElementoGuardado(entidad, propiedad, that.GetValorEncode(valoresProp[j].trim()), pTxtValores, pTxtElemEditados);
                        }
                    }
    
                    if (pValidar)
                    {                                                
                        var campoCorrecto = that.ComprobarCardinalidadPropiedad(entidad, propiedad, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados);
                        camposCorrectos = (campoCorrecto && camposCorrectos);
    
                        if (!campoCorrecto) {
                            propErrorSemCms += propiedad + ',' + entidad + '|';
                        }
                    }
                }
                else if (tipoProp == 'FSEO' || tipoProp == 'CSEO')
                {
                    var campoCorrecto = true;
                    if (pValidar)
                    {                        
                        campoCorrecto = that.ComprobarValorCampoCorrecto(entidad, propiedad, valorProp, pTxtIDs, pTxtCaract);
                        camposCorrectos = camposCorrectos && campoCorrecto;
                    }
    
                    if (campoCorrecto && valorProp != null)
                    {
                        valorProp = that.GetValorEncode(valorProp);
                        that.AgregarValorAPropiedad(entidad, propiedad, valorProp, pTxtValores);
                    }
    
                    if (!campoCorrecto) {
                        propErrorSemCms += propiedad + ',' + entidad + '|';
                    }
                }
            }
            
            else if (pValidar && that.EntidadEsPrincipalOPerteneceAPropFuncionalOCardiMenorIgualUno(entidad) && propLanguaje != propiedad)
            {
                
                var campoCorrecto = true;
    
                if (tipoProp == 'LD' || tipoProp == 'LO' || tipoProp == 'LSEO')
                {
                    campoCorrecto = that.ComprobarCardinalidadPropiedad(entidad, propiedad, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados);
                    camposCorrectos = (campoCorrecto && camposCorrectos);
    
                    if (!campoCorrecto) {
                        propErrorSemCms += propiedad + ',' + entidad + '|';
                    }
                }
    
                if (that.comprobarIdiomasRellenos && tipoProp != 'LO' && tipoProp != 'LSEO' && that.EsPropiedadMultiIdioma(entidad, propiedad) && (tipoProp == 'FD' || that.CardinalidadElementoMayorOIgualUno(entidad, propiedad, that.txtCaracteristicasElem))) {
                    
                    campoCorrecto = that.ComprobarPropiedadTieneIdiomasUsados(entidad, propiedad);
                    camposCorrectos = (campoCorrecto && camposCorrectos);
    
                    if (!campoCorrecto) {
                        propErrorSemCms += propiedad + ',' + entidad + '|';
                    }
                }
                //else if (tipoProp == 'FO' || tipoProp == 'CO')
                //{//TODO: ESTO FUNCIONA VACÍO???
    
                //}
            }
        }
    
        if (camposCorrectos) {            
            that.TxtHackHayCambios = false;
            that.GuardandoCambios = true;
        }
    
        return camposCorrectos;
    },

    EntidadEsPrincipalOPerteneceAPropFuncionalOCardiMenorIgualUno: function (pEntidad) {
        const that = this;
    
        if (pEntidad == '') {
            return false;
        }
            
        if (that.GetCaracteristicaPropiedad(pEntidad, '', that.txtCaracteristicasElem, 'entPrincipal') == 'true') {
            return true;
        }
    
        var entProps = that.GetEntidadYPropiedadConEntidadComoRango(pEntidad);
        var funcionalOCardi = true;
    
        if (entProps.length == 0) {
            entProps = that.GetEntidadYPropiedadConEntidadComoRango(pEntidad + '_bis0');
        }
    
        if (entProps.length == 0) {
            entProps = that.GetEntidadYPropiedadConEntidadComoRango(pEntidad + '_bis1');
        }
    
        for (var i = 0; i < entProps.length; i++) {
            var entidad = entProps[i][0];
            var propiedad = entProps[i][1];
    
            var tipo = that.GetCaracteristicaPropiedad(entidad, propiedad, that.txtCaracteristicasElem, 'tipo')
    
            if (tipo != 'FO' && tipo != 'CO') {
                funcionalOCardi = false;
            }
            else {
                funcionalOCardi = that.EntidadEsPrincipalOPerteneceAPropFuncionalOCardiMenorIgualUno(entidad)
            }
        }
    
        return funcionalOCardi;
    },  

    ComprobarValorCampoCorrecto: function(pEntidad, pPropiedad, pValor, pTxtIDs, pTxtCaract){
        const that = this;
        return that.ComprobarValorCampoCorrectoInt(pEntidad, pPropiedad, pValor, pTxtIDs, pTxtCaract, true)        
    },

    ComprobarCardinalidadPropiedad: function(pEntidad, pPropiedad, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados){
        const that = this;
        
        if (that.EntidadEsSubClase(pEntidad) && !that.EntidadSubClaseSeleccionada(pEntidad, true)) {
            return true;
        }

        var cardinalidad = that.GetCardinalidadPropiedad(pEntidad, pPropiedad, pTxtCaract);

        if (cardinalidad != null)
        {
            var tipoCardi = cardinalidad.split(',')[0];
            var numCardi = parseInt(cardinalidad.split(',')[1]);
            
            var numElemProp = that.GetNumElementosPropiedad(pEntidad, pPropiedad, pTxtValores, pTxtElemEditados);

            if (tipoCardi == 'Cardinality' && numCardi != numElemProp)
            {
                if (numCardi == 1)
                {                    
                    that.MostrarErrorPropiedad(pEntidad, pPropiedad, textoFormSem.cardi1, pTxtIDs);
                }
                else
                {
                    that.MostrarErrorPropiedad(pEntidad, pPropiedad, textoFormSem.cardiVarios.replace('@1@', numCardi), pTxtIDs);
                }
                return false;
            }
            else if (tipoCardi == 'MaxCardinality' && numCardi < numElemProp)
            {
                if (numCardi == 1)
                {
                    that.MostrarErrorPropiedad(pEntidad, pPropiedad, textoFormSem.maxCardi1, pTxtIDs);
                }
                else
                {
                    that.MostrarErrorPropiedad(pEntidad, pPropiedad, textoFormSem.maxCardiVarios.replace('@1@', numCardi), pTxtIDs);
                }
                return false;
            }
            else if (tipoCardi == 'MinCardinality' && numCardi > numElemProp)
            {
                if (numCardi == 1)
                {
                    that.MostrarErrorPropiedad(pEntidad, pPropiedad, textoFormSem.minCardi1, pTxtIDs);
                }
                else
                {
                    that.MostrarErrorPropiedad(pEntidad, pPropiedad, textoFormSem.minCardiVarios.replace('@1@', numCardi), pTxtIDs);
                }
                return false;
            }
        }

        //Limpio el control:                
        that.LimpiarHtmlControl(that.GetIDControlError(pEntidad, pPropiedad, pTxtIDs));

        if (pEntidad == 'UserArea_CandidatePerson' && pPropiedad == 'Tags')
        {
            that.LimpiarHtmlControl('divErrorTags');
        }

        return true;
    },


    LimpiarHtmlControl: function(pControl){
        const that = this;
        
        if (typeof pControl === "undefined" || pControl === null || pControl === ""){
            return;
        }        
        const inputToDisplayError = $(`#${pControl}`).parent().find("input");
        displayInputWithErrors(inputToDisplayError, false, "");        

        /*
        if (document.getElementById(pControl) != null)
        {
            //El siguiente código falla en Internet Explorer ya que no deja dar valor a innerHTML a algunos controles
            //document.getElementById(pControl).innerHTML = '';
            var nodosHijos=document.getElementById(pControl).childNodes
            
            for(i=0;i<nodosHijos.length;i++){
                document.getElementById(pControl).removeChild(nodosHijos[i]);
            }
        }*/
        
    },

    GetNumElementosPropiedad: function(pPadre, pElemento, pTxtValores, pTxtElemEditados){  
        const that = this;      
        var valorRdf = pTxtValores.val();// document.getElementById(pTxtValores).value;
        
        var indicesIniFinPadre = that.GetIndiceInicioFinElementoEditado(pPadre, valorRdf, pTxtElemEditados);
        var inicio = parseInt(indicesIniFinPadre.split(',')[0]);
        var fin = parseInt(indicesIniFinPadre.split(',')[1]);
        
        if (inicio < 0 || fin < 0)
        {
            return 0;
        }

        var elemPadre = '<' + pPadre + '>';
        var cierrePadre = '</' + pPadre + '>';

        var trozo2 = valorRdf.substring(inicio, (fin + cierrePadre.length));

        var numElem = 0;

        var elementos = that.ObtenerElementosXMLNodo(trozo2);

        for (var i=0;i<elementos.length;i++)
        {
            if (elementos[i].indexOf('<' + pElemento + '>') == 0)
            {
                numElem++;
            }
        }

        return numElem;        
    },

    EntidadEsSubClase: function(pTipoEntidad){       
        const that = this; 
        var subClase = that.GetCaracteristicaPropiedad(pTipoEntidad, '', that.txtCaracteristicasElem, 'subclase');
        return (subClase != null && subClase == 'true')        
    },

    PutElementoGuardado: function(pPadre, pElemento, pValor, pTxtValores, pTxtElemEditados){
        const that = this;
        
        var valorRdf = isJqueryObject(pTxtValores) ? pTxtValores.val() : document.getElementById(pTxtValores).value; //document.getElementById(pTxtValores).value;
        
        if (pPadre != null && pPadre != '' && valorRdf.indexOf('<' + pPadre+ '>') != -1)
        {
            var indicesIniFinPadre = that.GetIndiceInicioFinElementoEditado(pPadre, valorRdf, pTxtElemEditados);
            var inicio = parseInt(indicesIniFinPadre.split(',')[0]);
            var fin = parseInt(indicesIniFinPadre.split(',')[1]);

            var cierrePadre = '</' + pPadre + '>';
            var elemElemento = '<' + pElemento + '>';
            var cierreElemento = '</' + pElemento + '>';

            var trozo1 = valorRdf.substring(0, inicio);
            var trozo2 = valorRdf.substring(inicio, (fin + cierrePadre.length));
            var trozo3 = valorRdf.substring(fin + cierrePadre.length);

            if (trozo2.indexOf(cierreElemento) == -1) //Agregamo justo antes de acabar el padre:
            {
                trozo2 = trozo2.substring(0, trozo2.length - cierrePadre.length);
                trozo2 += elemElemento + pValor + cierreElemento + cierrePadre;
            }
            else //Agregamos detrás del último elemento del mismo tipo dentro del padre:
            {                
                trozo2 = that.AgregarElementoAXml(trozo2, elemElemento + pValor + cierreElemento, -1);
            }

            // document.getElementById(pTxtValores).value = trozo1 + trozo2 + trozo3;
            if (isJqueryObject(pTxtValores)){
                pTxtValores.val(trozo1 + trozo2 + trozo3);
            }else{                
                document.getElementById(pTxtValores).value = trozo1 + trozo2 + trozo3;
            }
            
            return true;
        }
        else if (pPadre != null && pPadre != '' && valorRdf.indexOf('<' + pPadre+ '&ULT>') != -1)
        {            
            return that.PutElementoGuardado(pPadre + '&ULT', pElemento, pValor, pTxtValores, pTxtElemEditados)
        }
        else
        {
            var elemElemento = '<' + pElemento + '>';
            var cierreElemento = '</' + pElemento + '>';

            if (pPadre == null || pPadre == '')
            {
                //document.getElementById(pTxtValores).value += elemElemento + pValor + cierreElemento;
                if (isJqueryObject(pTxtValores)){
                    pTxtValores.val(pTxtValores.val() + elemElemento + pValor + cierreElemento); 
                }else{                    
                    document.getElementById(pTxtValores).value += elemElemento + pValor + cierreElemento;                
                }    
            }
            else
            {
                if (pPadre.indexOf('&ULT') == -1)
                {
                    pPadre += '&ULT';
                }
            
                // document.getElementById(pTxtValores).value += '<' + pPadre + '>' + elemElemento + pValor + cierreElemento + '</' + pPadre + '>';                
                if (isJqueryObject(pTxtValores)){
                    pTxtValores.val(that.pTxtValores.val() + '<' + pPadre + '>' + elemElemento + pValor + cierreElemento + '</' + pPadre + '>');
                }else{
                    document.getElementById(pTxtValores).value += '<' + pPadre + '>' + elemElemento + pValor + cierreElemento + '</' + pPadre + '>';                    
                }                
                return true;
            }
        }

        return false;
    },

    
    GetUrlImg: function(pTxtCaract){
        var caract = pTxtCaract.val(); // document.getElementById(pTxtCaract).value;
        return caract.substring(0, caract.indexOf('|')).replace('$baseUrlStatic=', '') + '/img/';
    },
    
    GetUrlVideos: function(pTxtCaract){
        var caract = pTxtCaract.val(); // document.getElementById(pTxtCaract).value;
        caract = caract.substring(caract.indexOf('$baseUrlContent='));
        return caract.substring(0, caract.indexOf('|')).replace('$baseUrlContent=', '') + '/videos/';
    },
    
    GetUrlContent: function() {
        var caract = that.TxtCaracteristicasElem.val(); // document.getElementById(TxtCaracteristicasElem).value;
        caract = caract.substring(caract.indexOf('$baseUrlContent='));
        return caract.substring(0, caract.indexOf('|')).replace('$baseUrlContent=', '') + '/';
    },


    AgregarElementoAXml: function(pNodo, pElemento, pNumElemet){ 
        const that = this;               
        var raiz = that.ObtenerElementoRaiz(pNodo);
        var elementoName = that.ObtenerElementoRaiz(pElemento);
        var xmlFinal = '<' + raiz + '>';
        var elementos = that.ObtenerElementosXMLNodo(pNodo);
        var ultimoEncontrador = false;
        var encontrado = false;
        var num = pNumElemet;
        
        if (elementos.length == 0)
        {
            xmlFinal += pElemento;
        }
        else
        {
            for (var i=0;i<elementos.length;i++)
            {
                if (elementos[i].indexOf('<' + elementoName + '>') == 0)
                {
                    encontrado = true;
                    ultimoEncontrador = true;

                    if (pNumElemet != -1)
                    {
                        num--;

                        if (num == -1)
                        {
                            xmlFinal += pElemento;
                        }

                        if (num < 0)
                        {
                            ultimoEncontrador = false;
                        }
                    }
                }
                else if (ultimoEncontrador)
                {
                    xmlFinal += pElemento;
                    ultimoEncontrador = false;
                }

                xmlFinal += elementos[i];
            }

            if (!encontrado || ultimoEncontrador)
            {
                xmlFinal += pElemento;
            }
        }

        xmlFinal += '</' + raiz + '>';
        return xmlFinal;

    },

    DeleteElementoGuardado: function(pPadre, pElemento, pTxtValores, pTxtElemEditados, pNumElemet){ 
        const that = this;

        var valorRdf = isJqueryObject(pTxtValores) ? pTxtValores.val() : document.getElementById(pTxtValores).value;// document.getElementById(pTxtValores).value;
        var trozo1 = '';
        var trozo2 = '';
        var trozo3 = '';
        var elemElemento = '<' + pElemento + '>';
        var cierreElemento = '</' + pElemento + '>';
        
        if (valorRdf.indexOf(elemElemento) == -1)
        {
            return;
        }

        if (pPadre != null && pPadre != '')
        {
            var indicesIniFinPadre = that.GetIndiceInicioFinElementoEditado(pPadre, valorRdf, pTxtElemEditados);
            var inicio = parseInt(indicesIniFinPadre.split(',')[0]);
            var fin = parseInt(indicesIniFinPadre.split(',')[1]);

            var cierrePadre = '</' + pPadre + '>';

            trozo1 = valorRdf.substring(0, inicio);
            trozo2 = valorRdf.substring(inicio, (fin + cierrePadre.length));
            trozo3 = valorRdf.substring(fin + cierrePadre.length);

            trozo2 = that.BorrarElementoDeXml(trozo2, pElemento, pNumElemet);
        }
        else
        {
            trozo2 = '<padreFiccQuitar>' + valorRdf.replace('<||>', '<elemFiccQuitar></elemFiccQuitar>') + '</padreFiccQuitar>';
            trozo2 = that.BorrarElementoDeXml(trozo2, pElemento, pNumElemet);
            trozo2 = trozo2.replace('<elemFiccQuitar></elemFiccQuitar>', '<||>').replace('<padreFiccQuitar>', '').replace('</padreFiccQuitar>', '');
        }

        document.getElementById(pTxtValores).value = trozo1 + trozo2 + trozo3;
        
    },

    BorrarElementoDeXml: function(pNodo, pElemento, pNumElemet){
        const that = this;
        var raiz = that.ObtenerElementoRaiz(pNodo);
        var xmlFinal = '<' + raiz + '>';
        var elementos = that.ObtenerElementosXMLNodo(pNodo);
        var agregar = true;
        var num = pNumElemet;
        
        for (var i=0;i<elementos.length;i++)
        {
            if (elementos[i].indexOf('<' + pElemento + '>') == 0) {
                if (!isNaN(num)) {
                    if (pNumElemet == -1) {
                        agregar = (i < elementos.length - 1 && elementos[i + 1].indexOf('<' + pElemento + '>') == 0);
                    }
                    else {
                        num--;
    
                        if (num == -1) {
                            agregar = false;
                        }
                    }
                }
                else if (elementos[i].indexOf('<' + pElemento + '>' + num + '<') == 0) {
                    agregar = false;
                }
            }
            
            if (agregar)
            {
                xmlFinal += elementos[i];
            }
            else
            {
                agregar = true;
            }
        }
        
        xmlFinal += '</' + raiz + '>';
        return xmlFinal;        
    },

    ObtenerElementoRaiz: function(pNodo){
        const that = this;
        if (pNodo != '')
        {
            return pNodo.substring(1, pNodo.indexOf('>'));
        }        
        return '';
    },


    GetValorDefNoSelec: function(pEntidad, pPropiedad, pTxtCaract){
        const that = this;
        return that.GetCaracteristicaPropiedad(pEntidad, pPropiedad, pTxtCaract, 'valDefNoSelecc');
    },

    ComprobarMultiIdiomaCorrecto: function(pEntidad, pPropiedad, pTipoProp){
        const that = this;
        if (that.EsPropiedadMultiIdioma(pEntidad, pPropiedad)) {
            if (!that.comprobarIdiomasRellenos) {
                var valorProp = that.GetValorElementoGuardado(pEntidad, pPropiedad, that.txtValorRdf, that.txtElemEditados, 0);
            
                if (valorProp != null && valorProp != '' && valorProp.indexOf('@' + IdiomaDefectoFormSem + '[|lang|]') == -1 && (pTipoProp == 'FD' || that.CardinalidadElementoMayorOIgualUno(pEntidad, pPropiedad, that.txtCaracteristicasElem))) {
                    that.MostrarErrorPropiedad(pEntidad, pPropiedad, textoFormSem.propSinIdiomaDefecto, that.txtRegistroIDs);
                    return false;
                }
            }
            else if (pTipoProp == 'FD' || that.CardinalidadElementoMayorOIgualUno(pEntidad, pPropiedad, that.txtCaracteristicasElem)) {
                return that.ComprobarPropiedadTieneIdiomasUsados(pEntidad, pPropiedad);
            }
        }
        return true;        
    },


    ComprobarPropiedadTieneIdiomasUsados: function(){
        const that = this;

        var valoresProp = that.GetTodosValoresElementoGuardado(pEntidad, pPropiedad);

        for (var i = 0; i < valoresProp.length; i++) {
            if (!that.ComprobarValorTieneIdiomasUsados(valoresProp[i])) {
                that.MostrarErrorPropiedad(pEntidad, pPropiedad, textoFormSem.errorNoTodosIdiomas, that.txtRegistroIDs);
                return false;
            }
        }
        return true;
    },

    GetTodosValoresElementoGuardado: function(pPadre, pElemento){
        var valores = [];
        var valorRdf = that.txtValorRdf.val(); //document.getElementById(TxtValorRdf).value;
        var elmPadre = '<' + pPadre + '>';
        var cierrePadre = '</' + pPadre + '>';
        var inicio = valorRdf.indexOf(elmPadre);
        var fin = valorRdf.indexOf(cierrePadre);
    
        while (inicio >= 0 && fin >= 0) {
            
            var elemElemento = '<' + pElemento + '>';
            var cierreElemento = '</' + pElemento + '>';
    
            var trozo2 = valorRdf.substring(inicio, (fin + cierrePadre.length));
            valorRdf = valorRdf.substring((fin + cierrePadre.length));
            inicio = valorRdf.indexOf(elmPadre);
            fin = valorRdf.indexOf(cierrePadre);
    
            var count = 0;
            var valor = that.ObtenerValorElementoXMLNodo(trozo2, pElemento, count);
            while (valor != '') {
                valores.push(valor);
                count++;
                valor = that.ObtenerValorElementoXMLNodo(trozo2, pElemento, count);
            }
        }
        return valores;
    },

    ObtenerValorElementoXMLNodo: function(pNodo, pElemento, pNumElemet){
        const that = this;
        var elemento = that.ObtenerElementoXMLNodo(pNodo, pElemento, pNumElemet);
        elemento = elemento.substring(elemento.indexOf('>') + 1);
        elemento = elemento.substring(0, elemento.lastIndexOf('</'));
        return elemento;
    },

    ObtenerElementoXMLNodo: function(pNodo, pElemento, pNumElemet){
        const that = this;

        var elementos = that.ObtenerElementosXMLNodo(pNodo);
        var elemento = '';
        
        for (var i=0;i<elementos.length;i++)
        {
            if (elementos[i].indexOf('<' + pElemento + '>') == 0)
            {
                if (pNumElemet == -1)
                {
                    elemento = elementos[i];
                }
                else
                {
                    pNumElemet--;
                    
                    if (pNumElemet < 0)
                    {
                        elemento = elementos[i];
                        break;
                    }
                }
            }
        }
        
        return elemento;
    },

    ObtenerElementosXMLNodo: function(pNodo){
        const that = this;
        pNodo = pNodo.substring(pNodo.indexOf('>') + 1);
        pNodo = pNodo.substring(0, pNodo.lastIndexOf('</'));
        var hijosTexto = '';
        
        while (pNodo != '')
        {
            var variacion = 1;
            hijosTexto += pNodo.substring(0, pNodo.indexOf('<') + 1);
            pNodo = pNodo.substring(pNodo.indexOf('<') + 1);
            hijosTexto += pNodo.substring(0, pNodo.indexOf('>') + 1);
            pNodo = pNodo.substring(pNodo.indexOf('>') + 1);
            
            while (variacion != 0)
            {
                hijosTexto += pNodo.substring(0, pNodo.indexOf('<') + 1);
                pNodo = pNodo.substring(pNodo.indexOf('<') + 1);
                
                if (pNodo.indexOf('/') == 0) //cierre
                {
                    variacion--;
                }
                else
                {
                    variacion++;
                }
                
                hijosTexto += pNodo.substring(0, pNodo.indexOf('>') + 1);
                pNodo = pNodo.substring(pNodo.indexOf('>') + 1);
            }
            
            hijosTexto += '[|||]';
        }
        
        if (hijosTexto != '')
        {
            hijosTexto = hijosTexto.substring(0, hijosTexto.length - 5);
            return hijosTexto.split('[|||]');
        }
        else
        {
            return [];
        }
    },

    ComprobarValorTieneIdiomasUsados: function(){
        const that = this;
        for (var i = 0; i < idiomasUsados.length - 1; i++) {
            if (pValor.indexOf('@' + idiomasUsados[i] + '[|lang|]') == -1) {
                return false;
            }
        }
        return true;
    },

    MostrarErrorPropiedad: function(pEntidad, pPropiedad, pError, pTxtIDs){
        const that = this;
        that.crearErrorFormSem(pError, that.GetIDControlError(pEntidad, pPropiedad, pTxtIDs));
        
        if (pEntidad == 'UserArea_CandidatePerson' && pPropiedad == 'Tags')
        {
            that.crearErrorFormSem(textoFormSem.malTags, 'divErrorTags');
        }
    },

    GetIDControlError: function(pEntidad, pPropiedad, pTxtIDs) {
        const that = this;
        var propError = pPropiedad;
        var propRepetida = pPropiedad;
    
        if (propRepetida.indexOf('_Rep_') != -1) {
            propRepetida = propRepetida.substring(0, propRepetida.indexOf('_Rep_'));
        }
    
        var count = 0;
        var control = that.ObtenerControlEntidadProp(pEntidad + ',' + propRepetida + '_Rep_' + count, pTxtIDs);
    
        while (document.getElementById(control) != null) {
            propError = propRepetida + '_Rep_' + count;
            count++;
            control = that.ObtenerControlEntidadProp(pEntidad + ',' + propRepetida + '_Rep_' + count, pTxtIDs);
        }
    
        return that.ObtenerControlEntidadProp(pEntidad + ',' + propError, pTxtIDs).replace('Campo_', 'divError_').replace('panel_contenedor_Entidades_', 'divError_').replace('selEnt_', 'divError_');
    },    

    crearErrorFormSem: function (textoError, contenedor) {
        const that = this;

        const inputToDisplayError = $(`#${contenedor}`).parent().find("input");
        displayInputWithErrors(inputToDisplayError, true, textoError);


        /*$('#contenedor').addClass("errorFormSem");
        if (document.getElementById(contenedor) != null) {
            var $c = $(document.getElementById(contenedor));
            if ($c.find('div.ko').length) { // si ya existe el div.ko ...
                $c.find('div.ko').css('display', 'block');
                var inp = $c.find('div.ko').html(textoError);
    
                if (typeof (inp.shakeIt) != "undefined") {
                    inp.shakeIt();
                }
            } else { //... si no lo creamos
                if ($c[0].tagName == "DIV") {
                    $c.html('<div class="ko" >' + textoError + '</div>');
                    $c.find('div.ko').css('display', 'block');
                    var inp = $c.find('div.ko').html(textoError);
    
                    if (typeof (inp.shakeIt) != "undefined") {
                        inp.shakeIt();
                    }
                }
            }
        }
    
        if (typeof (personalizarFormKO) != 'undefined') {
            // EAD -> Quitar esta operativa en DevTools
            // personalizarFormKO.init();
        }*/
    },    
    

    CardinalidadElementoMayorOIgualUno: function(pEntidad, pPropiedad, pTxtCaract){  
        const that = this;      

        var cardinalidad = that.GetCardinalidadPropiedad(pEntidad, pPropiedad, pTxtCaract);
        if (cardinalidad != null)
        {
            try
            {
                var tipoCardi = cardinalidad.split(',')[0];
                var numCardi = parseInt(cardinalidad.split(',')[1]);

                if (tipoCardi == 'Cardinality' && numCardi >= 1)
                {
                    return true;
                }
                else if (tipoCardi == 'MinCardinality' && numCardi >= 1)
                {
                    return true;
                }
            }
            catch(ex){}
        }

        return false;
        
    },

    GetCardinalidadPropiedad: function(pEntidad, pPropiedad, pTxtCaract){
        const that = this;
        let elemento = 'cardi=';
        var caract = isJqueryObject(pTxtCaract) ? pTxtCaract.val() : document.getElementById(pTxtCaract).value; //pTxtCaract.val();// document.getElementById(pTxtCaract).value;
        var idEnti = '|' + pEntidad + ',' + pPropiedad + ',';
        caract = caract.substring(caract.indexOf(idEnti) + 1);
        caract = caract.substring(0, caract.indexOf('|'));
        
        if (caract.indexOf(elemento) != -1)
        {
            caract = caract.substring(caract.indexOf(elemento) + elemento.length);
            var cardi = caract.substring(0, caract.indexOf(','));
            
            caract = caract.substring(caract.indexOf(',') + 1);
            cardi += ',' + caract.substring(0, caract.indexOf(','));
            
            return cardi;
        }
        else
        {
            return null;
        }
    },


    GetValorElementoGuardado: function(pPadre, pElemento, pTxtValores, pTxtElemEditados, pNumElemet){
        const that = this;
        var valorRdf = isJqueryObject(pTxtValores) ? pTxtValores.val() : document.getElementById(pTxtValores).value;// document.getElementById(pTxtValores).value;
        var indicesIniFinPadre = that.GetIndiceInicioFinElementoEditado(pPadre, valorRdf, pTxtElemEditados);
        var inicio = parseInt(indicesIniFinPadre.split(',')[0]);
        var fin = parseInt(indicesIniFinPadre.split(',')[1]);
        
        if (inicio < 0 || fin < 0)
        {
            return '';
        }
        
        var cierrePadre = '</' + pPadre + '>';
        var elemElemento = '<' + pElemento + '>';
        var cierreElemento = '</' + pElemento + '>';
        
        var trozo2 = valorRdf.substring(inicio, (fin + cierrePadre.length));
        
        var valor = that.ObtenerValorElementoXMLNodo(trozo2, pElemento, pNumElemet);
        return valor;
    },

    GetIndiceInicioFinElementoEditado: function(pElemento, pValores, pTxtElemEditados){
        const that = this;
        var valorRdf = pValores;
        var editados = isJqueryObject(pTxtElemEditados) ? pTxtElemEditados.val() : document.getElementById(pTxtElemEditados).value; // document.getElementById(pTxtElemEditados).value;
        var indiceInicio = that.GetIndiceInicioElementoEditado(pElemento, valorRdf, editados)
        valorRdf = valorRdf.substring(indiceInicio);
        
        var elementoCierreXML = '</' + pElemento + '>';
        var indiceFin = (indiceInicio + valorRdf.indexOf(elementoCierreXML))
        
        return indiceInicio + ',' + indiceFin;

    },

    GetIndiceInicioElementoEditado: function (pElemento, pValores, pEditados){
        const that = this;
        var editados = pEditados;
        var valores = pValores;
        var indiceElmen = 0;
        var entraRecursivo = false;
        
        var elementoXML = '<' + pElemento + '>';
        var elementoCierreXML = '</' + pElemento + '>';
        
        //Buscamos en todas las entidades, por si alguna es el padre de la buscada:
        while (editados.length > 0)
        {
            var elemEditados = editados.split('|')[0];
            if (elemEditados != '')
            {
                editados = editados.substring(editados.indexOf('|') + 1);
                
                var idElem = elemEditados.split('=')[0];
                var ent = idElem.split(',')[0];
                var prop = idElem.split(',')[1];
                
                if (ent != pElemento && prop != pElemento) //No es ni la propiedad ni la entidad
                {
                    var numElem = elemEditados.split('=')[1];
                    var contenidoEntidad = that.GetValorElementoEnPosicion(prop, numElem, valores);
                    
                    if (contenidoEntidad.indexOf(elementoXML) != -1)
                    {
                        var elmeIdElem = '<' + prop + '>';
                        var cierreElmeIdElem = '</' + prop + '>';
                        var indiceCalculado = false;
                        
                        //Compruebo que no esté seleccionando un elemento candidato a ser agregado:
                        if (editados == '' && valores.indexOf('<||>') != -1)
                        {
                            var trozoAux = valores.substring(valores.indexOf('<||>') + 4);
                            if (trozoAux.indexOf(elmeIdElem) != -1)
                            {
                                indiceElmen += valores.indexOf('<||>') + 4;
                                valores = trozoAux;
                            }
                            else if (trozoAux.indexOf('<' + pElemento + '>') != -1) {//Quitar si va mal la edición, se ha puesto por Form Generic prado
                                indiceElmen += valores.indexOf('<||>') + 4;
                                valores = trozoAux;
    
                                indiceElmen += valores.indexOf('<' + pElemento + '>');
                                valores = valores.substring(valores.indexOf('<' + pElemento + '>'));
                                indiceCalculado = true;
                            }
                        }
                        
                        if (!indiceCalculado) {
                            indiceElmen += valores.indexOf(elmeIdElem);
                            valores = valores.substring(valores.indexOf(elmeIdElem));
    
                            for (var j = numElem; j > 0; j--) {
                                indiceElmen += valores.indexOf(cierreElmeIdElem) + cierreElmeIdElem.length;
                                valores = valores.substring(valores.indexOf(cierreElmeIdElem) + cierreElmeIdElem.length);
                            }
    
                            valores = valores.substring(0, valores.indexOf(cierreElmeIdElem) + cierreElmeIdElem.length);
                            if (ent == pElemento) {
                                entraRecursivo = true;
                            }
                        }
                    }
                }
            }
            else
            {
                break;
            }
        }
        
        if (!entraRecursivo)
        {
            return (indiceElmen + valores.lastIndexOf(elementoXML));
        }
        else if (valores.indexOf(elementoXML) != -1)
        {
            return (indiceElmen + valores.indexOf(elementoXML));
        }
        else
        {
            return indiceElmen;
        }
    },
    
    GetValorElementoEnPosicion: function(pElemento, pNumElem, pValores){
        const that = this;

        var i = pNumElem;
        var trozo = pValores;
        
        //Compruebo que no esté seleccionando un elemento candidato a ser agregado:
        if (pValores.indexOf('<||>') != -1)
        {
            var trozoAux = pValores.substring(pValores.indexOf('<||>') + 4);
            if (trozoAux.indexOf('<' + pElemento + '>') != -1)
            {
                trozo = trozoAux;
            }
        }
        
        while (i>0 && trozo.length > 0)
        {
            trozo = that.ObtenerTextoDespuesElemento(trozo, pElemento);
            i--;
        }
        
        return that.ObtenerValorTextoElemento(trozo, pElemento);
    },   

    ObtenerTextoDespuesElemento: function(pTexto, pElemento){    
        const that = this;
        var elementoCierreXML = '</' + pElemento + '>';
        if (pTexto.indexOf(elementoCierreXML) != -1)
        {
            return pTexto.substring(pTexto.indexOf(elementoCierreXML) + elementoCierreXML.length);
        }
        else
        {
            return '';
        }
    },

    ObtenerValorTextoElemento: function(pTexto, pElemento){  
        const that = this;              
        var elementoXML = '<' + pElemento + '>';
        var elementoCierreXML = '</' + pElemento + '>';
        if (pTexto.indexOf(elementoXML) != -1)
        {
            var texto = pTexto.substring(pTexto.indexOf(elementoXML), pTexto.indexOf(elementoCierreXML));
            texto = texto.substring(elementoXML.length);
            return texto;
        }
        else
        {
            return '';
        }
    },



    ComprobarValorCampoCorrectoInt: function (pEntidad, pPropiedad, pValor, pTxtIDs, pTxtCaract, pCompValVacio){
        const that = this;

        if (pValor != null) {
            pValor = pValor.trim();
        }
    
        if (pValor == '') {
            var caract = pTxtCaract.val();// document.getElementById(pTxtCaract).value;
    
            if (pPropiedad.indexOf('_Rep_') != -1 || caract.indexOf(pEntidad + ',' + pPropiedad + '_Rep_') != -1) {
                var propRepetida = pPropiedad;
    
                if (propRepetida.indexOf('_Rep_') != -1) {
                    propRepetida = propRepetida.substring(0, propRepetida.indexOf('_Rep_'));
                }
    
                var count = 0;
                pValor = that.ObtenerValorEntidadProp(pEntidad + ',' + propRepetida, pTxtIDs, pTxtCaract);
    
                while (pValor == '' && caract.indexOf(pEntidad + ',' + propRepetida + '_Rep_' + count) != -1)
                {
                    pValor = that.ObtenerValorEntidadProp(pEntidad + ',' + propRepetida + '_Rep_' + count, pTxtIDs, pTxtCaract);
                    count++;
                }
            }
        }
    
        
        that.ComprobarCambiosValor(pEntidad, pPropiedad, pValor);
    
        var tipoProp = that.GetTipoPropiedad(pEntidad, pPropiedad, pTxtCaract);
        var propContEntTieneTodasPropSinValor = that.ComprobarSiPropiedadPerteneceEntSinValorEnPropsDePropCard1(pEntidad, pPropiedad);
    
        if (!propContEntTieneTodasPropSinValor && pCompValVacio)
        {
            if ((tipoProp == 'FD' || tipoProp == 'FSEO') && pValor == '')
            {
                that.MostrarErrorPropiedad(pEntidad, pPropiedad, textoFormSem.propiedadObliValor, pTxtIDs);
                return false;
            }
            else if ((tipoProp == 'CD' || tipoProp == 'VD') && pValor == '' && that.CardinalidadElementoMayorOIgualUno(pEntidad, pPropiedad, pTxtCaract))
            {
                that.MostrarErrorPropiedad(pEntidad, pPropiedad, textoFormSem.propiedadObliValor, pTxtIDs);
                return false;
            }
        
            var valorDefNoSelec = that.GetValorDefNoSelec(pEntidad, pPropiedad, pTxtCaract);
        
            if (valorDefNoSelec != null && valorDefNoSelec != '' && valorDefNoSelec == pValor)
            {
                if (tipoProp != 'CD' || (tipoProp == 'CD' && that.CardinalidadElementoMayorOIgualUno(pEntidad, pPropiedad, pTxtCaract)))
                {
                    that.MostrarErrorPropiedad(pEntidad, pPropiedad, textoFormSem.propiedadObliValor, pTxtIDs);
                    return false;
                }
            }
        }
    
        if (pValor != '' && pValor != null)
        {            
            var tipoCampo = that.GetTipoCampo(pEntidad, pPropiedad, pTxtCaract);
            if (tipoCampo == 'Date' || tipoCampo == 'DateTime' || tipoCampo == 'Time')
            {                
                if (that.GetCaracteristicaPropiedad(pEntidad, pPropiedad, that.txtCaracteristicasElem, 'propFechaConHora') != null) {                    
                    if (!that.FechaConHoraCorrecta(pValor)) {
                        that.MostrarErrorPropiedad(pEntidad, pPropiedad, textoFormSem.errorFechaConHora, pTxtIDs);
                        return false;
                    }
                }
                else if (!that.FechaCorrecta(pValor))
                {                    
                    that.MostrarErrorPropiedad(pEntidad, pPropiedad, textoFormSem.errorFecha, pTxtIDs);
                    return false;
                }
            }
            else if (tipoCampo == 'Entero')
            {
                var valorNum = Number(pValor);
                if (isNaN(valorNum) || !Number.isInteger(valorNum))
                {
                    that.MostrarErrorPropiedad(pEntidad, pPropiedad, textoFormSem.errorNumEntero, pTxtIDs);
                    return false;
                }
            }
            else if (tipoCampo == 'Numerico')
            {
                var valorNum = parseFloat(pValor);
                if (isNaN(valorNum) || (valorNum + '').length != pValor.length)
                {
                    that.MostrarErrorPropiedad(pEntidad, pPropiedad, textoFormSem.errorNum, pTxtIDs);
                    return false;
                }
            }
            
            
            var numCaract = that.GetRestriccionNumCaract(pEntidad, pPropiedad, pTxtCaract);
            
            if (numCaract != null)
            {
                var tipoResCarac = numCaract.split(',')[0];
                var key = parseInt(numCaract.split(',')[1]);
                var value = parseInt(numCaract.split(',')[2]);
                
                if (tipoResCarac == '<' && pValor.length >= key)
                {
                    that.MostrarErrorPropiedad(pEntidad, pPropiedad, textoFormSem.valorExcedeNumCarac.replace('@1@', key), pTxtIDs);
                    return false;
                }
                else if(tipoResCarac == '>' && pValor.length <= key)
                {
                    that.MostrarErrorPropiedad(pEntidad, pPropiedad, textoFormSem.valorMenosNumCarac.replace('@1@', key), pTxtIDs);
                    return false;
                }
                else if(tipoResCarac == '=' && pValor.length != key)
                {
                    that.MostrarErrorPropiedad(pEntidad, pPropiedad, textoFormSem.valorNoNumCarac.replace('@1@', key), pTxtIDs);
                    return false;
                }
                else if(tipoResCarac == '-' && (pValor.length > value || pValor.length < key))
                {
                    that.MostrarErrorPropiedad(pEntidad, pPropiedad, textoFormSem.valorNoEntreNumCarac.replace('@1@', key).replace('@2@', value), pTxtIDs);
                    return false;
                }
            }
        }

        //Limpio el control:
        that.LimpiarHtmlControl(that.GetIDControlError(pEntidad, pPropiedad, pTxtIDs));
        //LimpiarHtmlControl(GetIDControlError(pEntidad, pPropiedad, pTxtIDs.replace('mGeneradorOWL_','mGeneradorOWLVP_')));
        
        if (pEntidad == 'UserArea_CandidatePerson' && pPropiedad == 'Tags')
        {
            that.LimpiarHtmlControl('divErrorTags');
        }
    
        if (typeof (personalizarFormKO) != 'undefined') {
            // EAD -> Quitar esta operativa en DevTools
            // personalizarFormKO.init();
        }
        
        return true;
    }, 

    GetRestriccionNumCaract: function(pEntidad, pPropiedad, pTxtCaract){
        elemento = 'numCaract=';
        var caract = isJqueryObject(pTxtCaract) ? pTxtCaract.val() : document.getElementById(pTxtCaract).value; //document.getElementById(pTxtCaract).value;
        var idEnti = '|' + pEntidad + ',' + pPropiedad + ',';
        caract = caract.substring(caract.indexOf(idEnti) + 1);
        caract = caract.substring(0, caract.indexOf('|'));
        
        if (caract.indexOf(elemento) != -1)
        {
            caract = caract.substring(caract.indexOf(elemento) + elemento.length);
            var numCaract = caract.substring(0, caract.indexOf(','));
            
            caract = caract.substring(caract.indexOf(',') + 1);
            numCaract += ',' + caract.substring(0, caract.indexOf(','));
            
            caract = caract.substring(caract.indexOf(',') + 1);
            numCaract += ',' + caract.substring(0, caract.indexOf(','));
            
            return numCaract;
        }
        else
        {
            return null;
        }
    },    
    
    ComprobarCambiosValor: function(pEntidad, pPropiedad, pValor){  
        const that = this;  
            
        var valorAntiguo = that.GetValorElementoEditadoGuardado(pEntidad, pPropiedad, that.txtValorRdf, that.txtElemEditados);
        
        if (valorAntiguo != null) {
            valorAntiguo = that.GetValorDecode(valorAntiguo);
        
            if (valorAntiguo != pValor) {
                that.TxtHackHayCambios = true;
            }
        }
    },

    GetValorDecode: function(pValor){
        const that = this;
        //return pValor.replace("[--C]", "<").replace("[C--]", ">")
        return pValor.replace(/\[--C]/g,'<').replace(/\[C--]/g, ">");    
    },

    GetValorElementoEditadoGuardado: function(pPadre, pElemento, pTxtValores, pTxtElemEditados){
        const that = this;        
        var numElem = that.GetNumEdicionEntProp(pPadre, pElemento, pTxtElemEditados);
        return that.GetValorElementoGuardado(pPadre, pElemento, pTxtValores, pTxtElemEditados, numElem);        
    },
    
    ComprobarSiPropiedadPerteneceEntSinValorEnPropsDePropCard1: function(pEntidad, pPropiedad){
        const that = this;
        
        var domSupCardi1 = that.GetCaracteristicaPropiedad(pEntidad, pPropiedad, that.txtCaracteristicasElem, 'domSupCardi1');
        if (domSupCardi1 != null && domSupCardi1 == 'true') {            
            return !that.EntidadTieneAlgunaPropiedadConValor(pEntidad);
        }

        return false;        
    },

    EntidadTieneAlgunaPropiedadConValor: function(pEntidad){
        
        var caract = that.txtCaracteristicasElem.val(); // document.getElementById(TxtCaracteristicasElem).value;
        caract = caract.substring(caract.indexOf('|') + 1);//Eliminio 1 parametro
        var caracteristicas = caract.split('|');
        
        for (var i=0;i<caracteristicas.length;i++)
        {
            var entProp = that.ObtenerNombreEntPropRestricciones(caracteristicas[i]);
            var entidad = entProp.split(',')[0];
        
            if (entidad == pEntidad)
            {
                var propiedad = entProp.split(',')[1];
                var tipoProp = that.GetTipoPropiedad(entidad, propiedad, that.txtCaracteristicasElem);
            
                if (tipoProp == 'FD' || tipoProp == 'CD' || tipoProp == 'VD' || tipoProp == 'FSEO' || tipoProp == 'CSEO')
                {
                    var valorProp = that.ObtenerValorEntidadProp(entProp, that.txtRegistroIDs, that.txtCaracteristicasElem);
                    var valorDefNoSelec = that.GetValorDefNoSelec(entidad, propiedad, that.txtCaracteristicasElem);
                
                    if (valorProp != null && valorProp != '' && (valorDefNoSelec == null || valorDefNoSelec == '' || valorDefNoSelec != valorProp))
                    {
                        return true;
                    }
                }
                else
                {
                    var numElemProp = that.GetNumElementosPropiedad(entidad, propiedad, that.txtValorRdf, that.txtElemEditados);
                
                    if (numElemProp > 0)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
        
    },

    GetTipoCampo: function(pEntidad, pPropiedad, pTxtCaract){
        const that = this;
        return that.GetCaracteristicaPropiedad(pEntidad, pPropiedad, pTxtCaract, 'tipoCampo');        
    },

    FechaConHoraCorrecta: function(pFecha){
        if (pFecha.indexOf(' ') == -1) {
            return false;
        }
    
        var fecha = pFecha.substring(0, pFecha.indexOf(' '));
        var hora = pFecha.substring(pFecha.indexOf(' ') + 1);
    
        if (hora.length != 8 || hora.indexOf(':') != 2 || hora.lastIndexOf(':') != 5) {
            return false;
        }
    
        var hh = parseInt(hora.split(':')[0]);
        var mm = parseInt(hora.split(':')[1]);
        var ss = parseInt(hora.split(':')[2]);
    
        if (hh.toString() == 'NaN' || mm.toString() == 'NaN' || ss.toString() == 'NaN') {
            return false;
        }
    
        if (hh < 0 || hh > 23) {
            return false;
        }
    
        if (mm < 0 || mm > 59) {
            return false;
        }
    
        if (ss < 0 || ss > 59) {
            return false;
        }
    
        return that.FechaCorrecta(fecha);
    },

    FechaCorrecta: function(pFecha){
        try
        {
            pFecha = pFecha.trim();
    
            if (pFecha == '' || pFecha.length != 10 || pFecha.indexOf('/') != 2 || pFecha.lastIndexOf('/') != 5) {
                return false;
            }
    
            var diaTexto = pFecha.substring(0, 2);
            var mesTexto = pFecha.substring(3, 5);
            var anyoTexto = pFecha.substring(6);
    
            if (diaTexto.indexOf('0') == 0) {
                diaTexto = diaTexto.substring(1);
            }
    
            if (mesTexto.indexOf('0') == 0) {
                mesTexto = mesTexto.substring(1);
            }
    
            while (anyoTexto.length > 0 && anyoTexto.indexOf('0') == 0) {
                anyoTexto = anyoTexto.substring(1);
            }
    
            if (anyoTexto.length == 0) {
                anyoTexto = '0';
            }
    
            var dia = parseInt(diaTexto);
            var mes = parseInt(mesTexto);
            var anyo = parseInt(anyoTexto);
    
            if (dia.toString() == 'NaN' || mes.toString() == 'NaN' || anyo.toString() == 'NaN') {
                return false;
            }
    
            if (mes < 1 || mes > 12) {
                return false;
            }
    
            if (dia < 1 || dia > 31) {
                return false;
            }
    
            if (mes == 2 && dia > 29) {
                return false;
            }
            else if (dia > 30 && (mes == 4 || mes == 6 || mes == 9 || mes == 11)) {
                return false;
            }
    
            return true;
        }
        catch (ex) {
            return false;
        }
    },

    AgregarValorAPropiedad: function(pEntidad, pPropiedad, pValor, pTxtValores){
        const that = this;

        var valorRdf = pTxtValores.val(); // document.getElementById(pTxtValores).value;            
        
        var entidadXML = '<' + pEntidad + '>';
        var entidadCierreXML = '</' + pEntidad + '>';
        var propiedadXML = '<' + pPropiedad + '>';
        var propiedadCierreXML = '</' + pPropiedad + '>';
        
        var trozo1 = valorRdf.substring(0, valorRdf.indexOf(entidadXML) + entidadXML.length);
        var trozo2 = valorRdf.substring(valorRdf.indexOf(entidadXML) + entidadXML.length, valorRdf.indexOf(entidadCierreXML));
        var trozo3 = valorRdf.substring(valorRdf.indexOf(entidadCierreXML));
        
        var trozoProp1 = '';
        
        while (trozo2.indexOf(propiedadXML) > 0)
        {
            var elementoSiguiente = trozo2.substring(1, trozo2.indexOf('>'));
            elementoSiguiente = '</' + elementoSiguiente + '>';
            trozoProp1 += trozo2.substring(0, trozo2.indexOf(elementoSiguiente) + elementoSiguiente.length);
            trozo2 = trozo2.substring(trozo2.indexOf(elementoSiguiente) + elementoSiguiente.length);
        }
        
        var trozoProp3 = trozo2.substring(trozo2.indexOf(propiedadCierreXML) + propiedadCierreXML.length);
        
        
        // document.getElementById(pTxtValores).value = trozo1 + trozoProp1 + propiedadXML + pValor + propiedadCierreXML + trozoProp3 + trozo3;
        pTxtValores.val(trozo1 + trozoProp1 + propiedadXML + pValor + propiedadCierreXML + trozoProp3 + trozo3);
    },


    ObtenerValorMultiIdiomaPesanyaActual: function(pEntidad, pPropiedad, pValorProp){
        const that = this;

        var idControlCampo = that.ObtenerControlEntidadProp(pEntidad + ',' + pPropiedad, that.txtRegistroIDs);
        var idControlCampoPes = idControlCampo.replace('Campo_', 'divContPesIdioma_');

        if (document.getElementById(idControlCampoPes) != null) {//Idioma con pestaña
            var li = $('li.active', document.getElementById(idControlCampoPes));
            var idiomaActual = li.attr('rel');

            var valorAntiguo = $('#' + idControlCampoPes).attr('langActual');
            var valorProp = that.IncluirTextoIdiomaEnCadena(valorAntiguo, pValorProp, idiomaActual);

            return valorProp;
        }
        else {//Idioma sin pestaña
            let idiomas = IdiomasConfigFormSem.split(',');
            let valorProp = '';

            for (var i = 0; i < idiomas.length; i++) {
                if (idiomas[i] != '') {
                    let idiomaObt = null;

                    if (idiomas[i] != IdiomaDefectoFormSem) {
                        idiomaObt = idiomas[i];
                    }

                    let valorIdio = that.ObtenerValorEntidadPropDeIdioma(pEntidad + ',' + pPropiedad, that.txtRegistroIDs, that.txtCaracteristicasElem, idiomaObt);
                    valorIdio = that.GetValorEncode(valorIdio);                    
                    valorProp = that.IncluirTextoIdiomaEnCadena(valorProp, valorIdio, idiomas[i]);
                }
            }

            return valorProp;
        }

        // A quíen llama? --> ObtenerValorEntidadPropDeIdioma        
    },
    
    IncluirTextoIdiomaEnCadena: function(pCadena, pTexto, pIdioma) {
        const that = this;
        var textoFinal = '';
        var marcaIdioma = '@' + pIdioma + '[|lang|]';
    
        if (pCadena.indexOf(marcaIdioma) != -1) {
            textoFinal = pCadena.substring(pCadena.indexOf(marcaIdioma) + marcaIdioma.length);
            pCadena = pCadena.substring(0, pCadena.indexOf(marcaIdioma));
    
            if (pCadena.indexOf('[|lang|]') != -1) {
                pCadena = pCadena.substring(0, pCadena.lastIndexOf('[|lang|]') + '[|lang|]'.length);
                textoFinal = pCadena + textoFinal;
            }
        }
        else if (pCadena.indexOf('[|lang|]') != -1) {
            textoFinal = pCadena;
        }
    
        if (pTexto != '') {
            pTexto += marcaIdioma;
        }
    
        textoFinal += pTexto;
    
        return textoFinal;
    },

    GetValorEncode: function(pValor){
        const that = this;
        return pValor.replace(/\</g,'[--C]').replace(/\>/g, "[C--]");
    },

    EsPropiedadMultiIdioma: function(pEntidad, pPropiedad) {
        const that = this;    

        if (that.txtCaracteristicasElem == undefined){
            that.txtCaracteristicasElem = "mTxtCaracteristicasElem";
        }else if (isJqueryObject(that.txtCaracteristicasElem)){
            that.txtCaracteristicasElem = that.txtCaracteristicasElem.prop("id");
        }else{
            // Si no es jquery ni un id sino un documentGetelementById
            that.txtCaracteristicasElem = "mTxtCaracteristicasElem";
        }               
        return (that.GetCaracteristicaPropiedad(pEntidad, pPropiedad, that.txtCaracteristicasElem, 'propMultiIdioma') == 'true');
    },

    /**
     * Método para obtener el valor de la propiedad de la entidad
     */
    ObtenerValorEntidadProp: function(pEntPropID, pTxtIDs, pTxtCaract){   
        const that = this;     
        return that.ObtenerValorEntidadPropDeIdioma(pEntPropID, pTxtIDs, pTxtCaract, null)        
    },

    ObtenerValorEntidadPropDeIdioma: function(pEntPropID, pTxtIDs, pTxtCaract, pIdioma){
        const that = this;
        const idControl = that.ObtenerControlEntidadProp(pEntPropID, pTxtIDs);
        const tipoCampo = that.ObtenerTipoCampo(pEntPropID, pTxtCaract);
    
        if (pIdioma != null) {
            idControl += '_lang_' + pIdioma;
        }
        
        if (pEntPropID == 'EmploymentPeriod,StartDate' || pEntPropID == 'EmploymentPeriod,EndDate' || pEntPropID == 'EmploymentPeriodPosition,StartDate' || pEntPropID == 'EmploymentPeriodPosition,EndDate' || pEntPropID == 'AttendancePeriod,StartDate' || pEntPropID == 'AttendancePeriod,EndDate' || pEntPropID == 'UserArea_PersonQualifications,FechaInicio' || pEntPropID == 'UserArea_PersonQualifications,FechaFin')
        {
            var dia = '01';
            var mes = that.ObtenerMesNumero(document.getElementById(idControl).children[0].value);
            var anio = document.getElementById(idControl).children[1].value;
            
            if (anio.length != 4 || mes == null)
            {
                return '';
            }
            else
            {
                return dia + '/' + mes + '/' + anio;
            }
        }
        else if (tipoCampo == 'Tiny')
        {
            if (document.getElementById(idControl) != null) {
                var valor = $('#' + idControl).val();
    
                if (valor == '<br>') {
                    return '';
                }
    
                return valor.trim();
            }
        }
        else if (document.getElementById(idControl) != null)
        {
            return document.getElementById(idControl).value.trim();
        }
        else
        {
            return null;
        }
    },

    ObtenerMesNumero: function(pMes){
        const that = this;
        if (pMes == 'Enero'){return '01';}
        if (pMes == 'Febrero'){return '02';}
        if (pMes == 'Marzo'){return '03';}
        if (pMes == 'Abril'){return '04';}
        if (pMes == 'Mayo'){return'05';}
        if (pMes == 'Junio'){return '06';}
        if (pMes == 'Julio'){return '07';}
        if (pMes == 'Agosto'){return '08';}
        if (pMes == 'Septiembre'){return '09';}
        if (pMes == 'Octubre'){return '10';}
        if (pMes == 'Noviembre'){return '11';}
        if (pMes == 'Diciembre'){return '12';}
    },
    
    ObtenerMesTexto: function(pMes){
        const that = this;
        if (pMes == '01'){return 'Enero';}
        if (pMes == '02'){return 'Febrero';}
        if (pMes == '03'){return 'Marzo';}
        if (pMes == '04'){return 'Abril';}
        if (pMes == '05'){return 'Mayo';}
        if (pMes == '06'){return 'Junio';}
        if (pMes == '07'){return 'Julio';}
        if (pMes == '08'){return 'Agosto';}
        if (pMes == '09'){return 'Septiembre';}
        if (pMes == '10'){return 'Octubre';}
        if (pMes == '11'){return 'Noviembre';}
        if (pMes == '12'){return 'Diciembre';}
    },   

    /**
     * Método para obtener control de la propiedad de la entidad
     * @param {*} pEntPropID 
     * @param {*} pTxtIDs 
     * @returns 
     */
    ObtenerControlEntidadProp: function (pEntPropID, pTxtIDs)
    {
        const that = this;

        const ids = isJqueryObject(pTxtIDs) ? pTxtIDs.val() : document.getElementById(pTxtIDs).value;// document.getElementById(pTxtIDs).value;
        
        let textoPropBuscar = pEntPropID + ',';
        if (ids.indexOf(textoPropBuscar) == -1)
        {
            textoPropBuscar = pEntPropID;
        }
        let trozoIDs = ids.substring(ids.indexOf(textoPropBuscar));
        trozoIDs = trozoIDs.substring(0, trozoIDs.indexOf('|'));
        
        return trozoIDs.replace(pEntPropID, '').substring(1);
    },

    /**
     * Método para obtener el tipo de campo
     * @param {*} pEntPropID 
     * @param {*} pTxtCaract 
     * @returns 
     */
    ObtenerTipoCampo: function (pEntPropID, pTxtCaract)
    {
        let caract = isJqueryObject(pTxtCaract) ? pTxtCaract.val() : document.getElementById(pTxtCaract).value; // pTxtCaract.val(); //document.getElementById(pTxtCaract).value;
        let caractProp = caract.substring(caract.indexOf(pEntPropID + ','));
        let prop = 'tipoCampo=';
        let tipoCampo = caractProp.substring(caractProp.indexOf(prop) + prop.length);
        tipoCampo = tipoCampo.substring(0, tipoCampo.indexOf(','));
        return tipoCampo;
    },    



    /**
     * Indica si es necesario recoger el valor de la propiedad. Se llama desde RecogerValoresRDFInt
     * @param {} pCaracteristicas 
     * @param {*} pTipoEntidad 
     * @returns 
     */
    HayQueRecogerValorProp: function(pCaracteristicas, pTipoEntidad){
        const that = this;
        return (pCaracteristicas.length > 0 && ((pCaracteristicas.indexOf('recoger=true') != -1 && pCaracteristicas.indexOf('subclase=true') == -1) || (that.EntidadSubClaseSeleccionada(pTipoEntidad, true) && (pCaracteristicas.indexOf('tipo=FD,') != -1 || pCaracteristicas.indexOf('tipo=CD,') != -1 || pCaracteristicas.indexOf('tipo=VD,') != -1 || pCaracteristicas.indexOf('tipo=FSEO,') != -1 || pCaracteristicas.indexOf('tipo=CSEO,') != -1) && !that.PerteneceEntidadAAlgunGrupoPanelSinEditar(that.GetSuperClasesEntidad(pTipoEntidad)[0], that.txtCaracteristicasElem, that.txtElemEditados))))
    }, 

    /**
     * Método para comprobar si la entidad pertenece a algún panel sin editar. Se llama desde HayQueRecogerValorProp.
     * @param {*} pEntidad 
     * @param {*} pTxtCaract 
     * @param {*} pTxtElemEditados 
     */
    PerteneceEntidadAAlgunGrupoPanelSinEditar: function(pEntidad, pTxtCaract, pTxtElemEditados){
        const that = this;
        
        const entProps = that.GetEntidadYPropiedadConEntidadComoRango(pEntidad);
        
        for (var i = 0; i < entProps.length; i++) {
            const entidad = entProps[i][0];
            const propiedad = entProps[i][1];
        
            if (that.GetNumEdicionEntProp(entidad, propiedad, pTxtElemEditados) != -1) //Se está editando propiedad:
            {
                return false;
            }
        
            //var tipo = GetCaracteristicaPropiedad(entidad, propiedad, pTxtCaract, 'tipo');
        
            //if (tipo == 'LO' || tipo == 'CO') {
            //    return true;
            //}
        }
        
        return that.PerteneceEntidadAlgonaProp_LO(pEntidad);
    },

    PerteneceEntidadAlgonaProp_LO: function(pEntidad) {
        const that = this;
        
        const entProps = that.GetEntidadYPropiedadConEntidadComoRango(pEntidad);
    
        for (let i = 0; i < entProps.length; i++) {
            const entidad = entProps[i][0];
            const propiedad = entProps[i][1];
            
            const tipo = that.GetCaracteristicaPropiedad(entidad, propiedad, that.txtCaracteristicasElem, 'tipo');
    
            if (tipo == 'LO') {
                return true;
            }
    
            return that.PerteneceEntidadAlgonaProp_LO(entidad);
        }
    
        return false;
    },


    /**
     * Método para obtener las super clases de la entidad. Se llama desde HayQueRecogerValorProp
     */
    GetSuperClasesEntidad: function(pEntidad){  
        const that = this;     
        
        // Dependiendo la acción de la que provenga, es posible que no exista that.txtCaracteristicasElem
        const pTxtCaracteristicasElem = that.txtCaracteristicasElem == undefined ? 'mTxtCaracteristicasElem' : that.txtCaracteristicasElem;
        
        const supers = that.GetCaracteristicaPropiedad(pEntidad, '', pTxtCaracteristicasElem, 'superclases');
        
        if (supers != null)
        {
            const superclases = supers.split(';');
            return superclases;
        }

        return null;        
    },


    /**
     * Método para obtener el número de edición de la propiedad de la entidad
     * @param {*} pEntidad 
     * @param {*} pPropiedad 
     * @param {*} pTxtElemEditados 
     * @returns 
     */
    GetNumEdicionEntProp: function(pEntidad, pPropiedad, pTxtElemEditados){
        const that = this;   

        let editados = isJqueryObject(pTxtElemEditados) ? pTxtElemEditados.val() : document.getElementById(pTxtElemEditados).value; // document.getElementById(pTxtElemEditados).value;
        
        let ideElemento = pEntidad;
        
        if (pPropiedad != null)
        {
            ideElemento += ',' + pPropiedad;
        }

        ideElemento += '=';

        if (editados.indexOf(ideElemento) != -1)
        {
            let valor = editados.substring(editados.indexOf(ideElemento) + ideElemento.length);
            valor = valor.substring(0, valor.indexOf('|'));
        
            if (isNaN(valor)) {
                return valor;
            }
            else {
                return parseInt(valor);
            }
        }
        else
        {
            return -1;
        }
    },

    /**
     * Método para obtener la propiedad y entidad como Rango. Se llama desde PerteneceEntidadAAlgunGrupoPanelSinEditar.
     * @param {*} pEntidad 
     * @returns 
     */
    GetEntidadYPropiedadConEntidadComoRango: function (pEntidad) {
        const that = this;
        let caract = "";

        if (that.txtCaracteristicasElem == undefined){
            caract = document.getElementById("mTxtCaracteristicasElem").value;
        }else if (isJqueryObject(that.txtCaracteristicasElem)){
            caract = that.txtCaracteristicasElem.val();
        }else{
            // Si no es jquery ni un id sino un documentGetelementById
            caract = document.getElementById("mTxtCaracteristicasElem").value;
        }

        //let caract = that.txtCaracteristicasElem.val(); // document.getElementById(TxtCaracteristicasElem).value;


        const claveRango = 'rango=' + pEntidad + ',';
        let indiceSigRango = caract.indexOf(claveRango);
    
        if (indiceSigRango == -1) {
            const superClases = that.GetSuperClasesEntidad(pEntidad);
    
            if (superClases != null) {
                for (let i = 0; i < superClases.length; i++) {
                    if (superClases[i] != '') {
                        claveRango = 'rango=' + superClases[i] + ',';
                        indiceSigRango = caract.indexOf(claveRango);
    
                        if (indiceSigRango != -1) {
                            break;
                        }
                    }
                }
            }
        }
    
        let entProps = [];
    
        while (indiceSigRango != -1) {
            let caracteristica = caract.substring(0, indiceSigRango);
            caracteristica = caracteristica.substring(caracteristica.lastIndexOf('|') + 1);
    
            const entidad = caracteristica.substring(0, caracteristica.indexOf(','));
            caracteristica = caracteristica.substring(caracteristica.indexOf(',') + 1);
            const propiedad = caracteristica.substring(0, caracteristica.indexOf(','));
    
            entProps.push([entidad, propiedad]);
    
            caract = caract.substring(indiceSigRango + claveRango.length);
            indiceSigRango = caract.indexOf(claveRango);
        }
    
        return entProps;
    },

    /**
     * Obtener la entidad subclase seleccionada. Se llama desde HayQueRecogerValorProp
     * @param {*} pTipoEntidad 
     * @param {*} pSoloSiComboVisible 
     * @returns 
     */
    EntidadSubClaseSeleccionada: function (pTipoEntidad, pSoloSiComboVisible){
        const that = this;

        if (pTipoEntidad != "") {
            var combosSubClase = $('select.cmbSubClass');
    
            for (var i = 0; i < combosSubClase.length; i++) {
                if (combosSubClase[i].value == pTipoEntidad && (!pSoloSiComboVisible || $(combosSubClase[i]).is(":visible"))) {
                    return true;
                }
            }
        }
    
        return false;
    },    
  

    /**
     * Método para obtener el tipo de propiedad. Se llama desde RecogerValoresRDFInt
     * @param {*} pEntidad 
     * @param {*} pPropiedad 
     * @param {*} pTxtCaract 
     * @returns 
     */
    GetTipoPropiedad: function(pEntidad, pPropiedad, pTxtCaract){
        const that = this;
        return that.GetCaracteristicaPropiedad(pEntidad, pPropiedad, pTxtCaract, 'tipo');
    },

    /**
     * Método para obtener la característica de la propiedad. Se llama desde GetTipoPropiedad, 
     * @param {*} pEntidad 
     * @param {*} pPropiedad 
     * @param {*} pTxtCaract 
     * @param {*} pElemento 
     * @returns 
     */
    GetCaracteristicaPropiedad: function(pEntidad, pPropiedad, pTxtCaract, pElemento){
        const that = this;

        pElemento = pElemento + '=';
        var caract = isJqueryObject(pTxtCaract) ? pTxtCaract.val() : document.getElementById(pTxtCaract).value; // document.getElementById(pTxtCaract).value;
        var idEnti = '|' + pEntidad + ',' + pPropiedad + ',';
        caract = caract.substring(caract.indexOf(idEnti) + 1);
        caract = caract.substring(0, caract.indexOf('|'));
        
        if (caract.indexOf(pElemento) != -1)
        {
            caract = caract.substring(caract.indexOf(pElemento));
            return caract.substring(pElemento.length, caract.indexOf(','));
        }
        else
        {
            return null;
        }
    },     

    /**
     * Método para obtener el nombre de las propiedades de las entidades con restricciones. Se llama desde RecogerValoresRDFInt
     * @param {} pCaracteristicas 
     * @returns 
     */
    ObtenerNombreEntPropRestricciones: function(pCaracteristicas){
        var nombreEntProp = pCaracteristicas.substring(0, pCaracteristicas.indexOf(',') + 1);
        pCaracteristicas = pCaracteristicas.substring(pCaracteristicas.indexOf(',') + 1);
        nombreEntProp += pCaracteristicas.substring(0, pCaracteristicas.indexOf(','));
        return nombreEntProp;
    },    
    
    /**
     * Método para obtener el idioma de la propiedad. Llamado desde RecogerValoresRDFInt
     * @returns 
     */
    GetPropLangBusqCom: function() {
        const that = this;
        /*if (document.getElementById(TxtCaracteristicasElem) == null) {
            
        }
        */
        if (that.txtCaracteristicasElem.val() == null || that.txtCaracteristicasElem.val() == "undefined"){
            return null;
        }
    
        var caract = that.txtCaracteristicasElem.val(); // document.getElementById(TxtCaracteristicasElem).value;
        
        if (caract.indexOf('$propLangBusqCom=') != -1) {
            caract = caract.substring(caract.indexOf('$propLangBusqCom='));
            return caract.substring(0, caract.indexOf('|')).replace('$propLangBusqCom=', '');
        }
        else {
            return null;
        }
    },

    SeleccionarIdioma: function(pLink, pEntidad, pPropiedad, pIdioma, pMultiple){
        const that = this;
        
        
        const li = $('li.active', $(pLink).parent().parent());
        const idiomaActual = li.attr('rel');
    
        let valorProp = that.ObtenerValorEntidadProp(pEntidad + ',' + pPropiedad, that.txtRegistroIDs, that.txtCaracteristicasElem);
        const campoCorrecto = that.ComprobarValorCampoCorrectoInt(pEntidad, pPropiedad, valorProp, that.txtRegistroIDs, that.txtCaracteristicasElem, false);


        if (campoCorrecto && valorProp != null) {
            
            valorProp = that.GetValorEncode(valorProp);
            let valorAntiguo = '';
            let idControlCampoPes = '';                
            
            idControlCampoPes = that.ObtenerControlEntidadProp(pEntidad + ',' + pPropiedad, that.txtRegistroIDs).replace('Campo_', 'divContPesIdioma_');
            valorAntiguo = $('#' + idControlCampoPes).attr('langActual');                            
            valorProp = that.IncluirTextoIdiomaEnCadena(valorAntiguo, valorProp, idiomaActual);

            $('#' + idControlCampoPes).attr('langActual', valorProp);
            
            li.removeClass('active');
            $(pLink).parent().addClass('active');                        
            that.DarValorControl(pEntidad + ',' + pPropiedad, that.txtRegistroIDs, that.txtCaracteristicasElem, that.GetValorDecode(that.ExtraerTextoIdioma(valorAntiguo, pIdioma)));             
        }

    },

    ExtraerTextoIdioma: function (pTexto, pIdioma) {
        const that = this;

        const marcaIdioma = '@' + pIdioma + '[|lang|]';
        let textoFin = '';
    
        if (pTexto.indexOf(marcaIdioma) != -1) {
            textoFin = pTexto.substring(0, pTexto.indexOf(marcaIdioma));
    
            if (textoFin.indexOf('[|lang|]') != -1) {
                textoFin = textoFin.substring(textoFin.lastIndexOf('[|lang|]') + '[|lang|]'.length);
            }
        }
    
        return textoFin;
    },    


    DarValorControl: function(pEntPropID, pTxtIDs, pTxtCaract, pValor){
        const that = this;
        that.DarValorControlConIdioma(pEntPropID, pTxtIDs, pTxtCaract, pValor, null);
    },


    DarValorControlConIdioma: function(pEntPropID, pTxtIDs, pTxtCaract, pValor, pIdioma){
        const that = this;
        
        const idControl = that.ObtenerControlEntidadProp(pEntPropID, pTxtIDs);
        const tipoCampo = that.ObtenerTipoCampo(pEntPropID, pTxtCaract);
    

        if (pIdioma != null) {
            idControl += '_lang_' + pIdioma;
        }
        
        if (pEntPropID == 'EmploymentPeriod,StartDate' || pEntPropID == 'EmploymentPeriod,EndDate' || pEntPropID == 'EmploymentPeriodPosition,StartDate' || pEntPropID == 'EmploymentPeriodPosition,EndDate' || pEntPropID == 'AttendancePeriod,StartDate' || pEntPropID == 'AttendancePeriod,EndDate' || pEntPropID == 'UserArea_PersonQualifications,FechaInicio' || pEntPropID == 'UserArea_PersonQualifications,FechaFin')
        {
            if (pValor != '')
            {
                try
                {
                    var mes = pValor.substring(3,5);
                    var anio = pValor.substring(6);
                    
                    document.getElementById(idControl).children[0].value = that.ObtenerMesTexto(mes);
                    document.getElementById(idControl).children[1].value = anio;
                    
                    document.getElementById(idControl).children[0].style.color = 'black';
                    document.getElementById(idControl).children[1].style.color = 'black';
                }
                catch(ex){}
            }
            else
            {
                document.getElementById(idControl).children[0].selectedIndex =0;
                document.getElementById(idControl).children[1].value = 'Año';
                
                document.getElementById(idControl).children[0].style.color = 'gray';
                document.getElementById(idControl).children[1].style.color = 'gray';
            }
        }
        else if (tipoCampo == 'Tiny')//TODO: Enlaces y cosas raras del tiny
        {
            /*if (document.getElementById('cke_contents_' + idControl) != null) {
                document.getElementById('cke_contents_' + idControl).children[0].contentWindow.document.body.innerHTML = pValor;
            }*/
            $('#' + idControl).val(pValor)
        }
        else if (document.getElementById(idControl) != null)
        {
            
            if (document.getElementById(idControl).tagName == 'SELECT')
            {
                if (pValor == '')
                {
                    document.getElementById(idControl).selectedIndex = 0;
                }
                else
                {
                    document.getElementById(idControl).value = pValor;
                }
                
                if (pValor == '' && GetValorDefNoSelec(pEntPropID.split(',')[0], pEntPropID.split(',')[1], pTxtCaract) != null)
                {
                    document.getElementById(idControl).style.color = "gray";
                }
                else
                {
                    document.getElementById(idControl).style.color = "black";
                }
            }
            else
            {
                const valorAnterior = document.getElementById(idControl).value;
                document.getElementById(idControl).value = pValor;
    
                if (tipoCampo == 'Boleano') {
                    document.getElementById('chkSi_' + idControl).checked = ((pValor == '' || pValor == 'true') && (valorAnterior == 'true' || valorAnterior == ''));
                    document.getElementById('chkNo_' + idControl).checked = ((pValor == '' || pValor == 'false') && (valorAnterior == 'false'));
    
                    if (pValor == '') {
                        if (valorAnterior == 'false') {
                            document.getElementById(idControl).value = 'false';
                        }
                        else {
                            document.getElementById(idControl).value = 'true';
                        }
                    }
                }
                else if ((tipoCampo == 'DateTime' || tipoCampo == 'Date' || tipoCampo == 'Time') && pValor.indexOf('/') == -1 && pValor.length == 14) {
                    document.getElementById(idControl).value = pValor.substring(6, 8) + '/' + pValor.substring(4, 6) + '/' + pValor.substring(0, 4);                    
                    if (that.GetCaracteristicaPropiedad(pEntPropID.split(',')[0], pEntPropID.split(',')[1], that.txtCaracteristicasElem, 'propFechaConHora') != null) {
                        document.getElementById(idControl).value += ' ' + pValor.substring(8, 10) + ':' + pValor.substring(10, 12) + ':' + pValor.substring(12);
                    }
                }
    
                if (tipoCampo == 'Imagen' || tipoCampo == 'Archivo' || tipoCampo == 'Video' || tipoCampo == 'ArchivoLink') {
                    if (pValor != '') {
                        $('#' + idControl.replace('Campo_', 'divAgregarArchivo_')).css('display', 'none');
                        $('#' + idControl.replace('Campo_', 'divArchivoAgregado_')).css('display', '');
    
                        if (tipoCampo == 'Imagen') {
                            const urlContent = $('input.inpt_baseURLContent').val();
                            $('#' + idControl.replace('Campo_', 'archVistaPre_')).html('<img src="' + urlContent + "/" + pValor + '" alt="' + pValor + '" />');
                        }
                        else {
                            let valorVisPre = pValor;
    
                            if ((tipoCampo == 'ArchivoLink' || tipoCampo == 'Archivo') && valorVisPre.indexOf('_') != -1 && valorVisPre.indexOf('.') != -1) {
                                let guidValor = valorVisPre.substring(0, valorVisPre.lastIndexOf("."));
                                guidValor = guidValor.substring(guidValor.lastIndexOf("_") + 1);
    
                                if (guidValor.length == 36) {
                                    valorVisPre = valorVisPre.substring(0, valorVisPre.lastIndexOf("_")) + valorVisPre.substring(valorVisPre.lastIndexOf("."));
                                }
                            }
    
                            if (tipoCampo == 'ArchivoLink' && valorVisPre.indexOf('/') != -1) {
                                valorVisPre = valorVisPre.substring(valorVisPre.lastIndexOf('/') + 1);
                            }
    
                            $('#' + idControl.replace('Campo_', 'archVistaPre_')).html(valorVisPre);
                        }
                    }
                    else {
                        $('#' + idControl.replace('Campo_', 'divAgregarArchivo_')).css('display', '');
                        $('#' + idControl.replace('Campo_', 'divArchivoAgregado_')).css('display', 'none');
    
                        $('#' + idControl.replace('Campo_', 'archVistaPre_')).html();
                    }
    
                    if (document.getElementById(idControl.replace('Campo_', 'chkImgPrincfileUpLoad_')) != null) {
                        if (pValor != '') {
                            document.getElementById(idControl.replace('Campo_', 'chkImgPrincfileUpLoad_')).disabled = false;
                        }
                        else {
                            document.getElementById(idControl.replace('Campo_', 'chkImgPrincfileUpLoad_')).disabled = true;
                        }
    
                        var entProp = pEntPropID.split(',');
                        var imgPrinc = ObtenerImagenPrincipal(entProp[0], entProp[1]);
    
                        if (imgPrinc != '' && imgPrinc == pValor) {
                            document.getElementById(idControl.replace('Campo_', 'chkImgPrincfileUpLoad_')).checked = true;
                        }
                        else {
                            document.getElementById(idControl.replace('Campo_', 'chkImgPrincfileUpLoad_')).checked = false;
                        }
                    }
                }
                else if (idControl.indexOf('selEnt_') != -1 && document.getElementById(idControl.replace('selEnt_', 'hack_')) != null) {
                    if (pValor == '') {
                        document.getElementById(idControl.replace('selEnt_', 'hack_')).value = pValor;
                    }
                    else {                        
                        var textoEntidad = that.ObtenerTextoRepEntidadExterna(pValor);
    
                        if (textoEntidad != '') {
                            document.getElementById(idControl.replace('selEnt_', 'hack_')).value = textoEntidad;
                        }
                    }
                }
                else {
                    const entProp = pEntPropID.split(',');
                    const txtCaracteristicasElem = that.txtCaracteristicasElem == undefined ? 'mTxtCaracteristicasElem' : 'mTxtCaracteristicasElem';
                    if (that.GetEsPropiedadGrafoDependiente(entProp[0], entProp[1], txtCaracteristicasElem)) {
                        if (pValor != '') {
                            var valoresGraf = document.getElementById(TxtValoresGrafoDependientes).value;
    
                            if (valoresGraf.indexOf(pValor) != -1) {
                                var trozo = valoresGraf.substring(valoresGraf.indexOf(pValor));
                                trozo = trozo.substring(trozo.indexOf(',') + 1, trozo.indexOf('|'));
                                document.getElementById(idControl.replace('Campo_', 'hack_')).value = trozo;
                            }                            
                            that.HabilitarCamposGrafoDependientes(entProp[0], entProp[1]);
                        }
                        else {
                            //document.getElementById(idControl.replace('Campo_', 'hack_')).value = '';                            
                            that.EliminarValoresGrafoDependientes(entProp[0], entProp[1], true, false);
                        }
                    }
                    else if (document.getElementById(idControl.replace('Campo_', 'hackSec_')) != null) {
                        document.getElementById(idControl.replace('Campo_', 'hackSec_')).value = pValor;
                    }
                }
            }
        }        
    },

    EliminarValoresGrafoDependientes: function (pEntidad, pPropiedad, pDeshabilitar, pRecursivo) {
        const that = this;
        
        const idControlCampo = that.ObtenerControlEntidadProp(pEntidad + ',' + pPropiedad, TxtRegistroIDs);
        //var valorBorrar = document.getElementById(idControlCampo).value;
        document.getElementById(idControlCampo).value = '';
    
        const idControlAuto = idControlCampo.replace("Campo_", "hack_");
        document.getElementById(idControlAuto).value = '';
    
    
        //    if (valorBorrar != '') {
        //        var valoresGraf = document.getElementById(TxtValoresGrafoDependientes).value;
        //        var valoresGrafDef = valoresGraf.substring(0, valoresGraf.indexOf(valorBorrar));
        //        valoresGraf = valoresGraf.substring(valoresGraf.indexOf(valorBorrar));
        //        valoresGraf = valoresGraf.substring(valoresGraf.indexOf('|') + 1);
        //        valoresGrafDef += valoresGraf;
        //        document.getElementById(TxtValoresGrafoDependientes).value = valoresGrafDef;
        //    }
     
        document.getElementById(idControlAuto).disabled = (pDeshabilitar && !that.GetEsPropiedadGrafoDependienteSinPadres(pEntidad, pPropiedad));
    
        if (pRecursivo) {
            const pronEntDep = that.GetPropiedadesDependientes(pEntidad, pPropiedad);
    
            if (pronEntDep != null) {
                that.EliminarValoresGrafoDependientes(pronEntDep[1], pronEntDep[0], true, pRecursivo);
            }
        }
    }, 

    GetEsPropiedadGrafoDependienteSinPadres: function(pEntidad, pPropiedad) {
        const that = this;        
        return (that.GetCaracteristicaPropiedad(pEntidad, pPropiedad, TxtCaracteristicasElem, 'propGrafoDepSinPadres') != null);
    },   

    HabilitarCamposGrafoDependientes: function(pEntidad, pPropiedad) {
        const that = this;

        const pronEntDep = that.GetPropiedadesDependientes(pEntidad, pPropiedad);
    
        if (pronEntDep != null) {
            var idControlAuto = that.ObtenerControlEntidadProp(pronEntDep[1] + ',' + pronEntDep[0], TxtRegistroIDs).replace("Campo_", "hack_");
            document.getElementById(idControlAuto).disabled = false;
        }
    },


    GetPropiedadesDependientes: function(pEntidad, pPropiedad){
        const that = this;

        var propEnt = that.GetCaracteristicaPropiedadMultiplesValores(pEntidad, pPropiedad, TxtCaracteristicasElem, 'propEntDependiente', 2);
    
        if (propEnt != null) {
            var array = propEnt.split(',');
            array[0] = array[0].substring(1, array[0].length - 1);
            array[1] = array[1].substring(1, array[1].length - 1);
            return array;
        }
    
        return null;        
    },

    GetCaracteristicaPropiedadMultiplesValores: function(pEntidad, pPropiedad, pTxtCaract, pElemento, pNumValores){
        const that = this;

        pElemento = pElemento + '=';
        var caract = document.getElementById(pTxtCaract).value;
        var idEnti = '|' + pEntidad + ',' + pPropiedad + ',';
        caract = caract.substring(caract.indexOf(idEnti) + 1);
        caract = caract.substring(0, caract.indexOf('|'));
        
        if (caract.indexOf(pElemento) != -1)
        {
            caract = caract.substring(caract.indexOf(pElemento) + pElemento.length);
            var valorFinal = '';
            for (var i=0;i<pNumValores;i++)
            {
                valorFinal += caract.substring(0, caract.indexOf(',')) + ',';
                caract = caract.substring(caract.indexOf(',') + 1);
            }
            return valorFinal;
        }
        else
        {
            return null;
        }
    },    

    GetCaracteristicaPropiedadMultiplesValores: function(pEntidad, pPropiedad, pTxtCaract, pElemento, pNumValores){        
        pElemento = pElemento + '=';
        let caract = document.getElementById(pTxtCaract).value;
        const idEnti = '|' + pEntidad + ',' + pPropiedad + ',';
        caract = caract.substring(caract.indexOf(idEnti) + 1);
        caract = caract.substring(0, caract.indexOf('|'));
        
        if (caract.indexOf(pElemento) != -1)
        {
            caract = caract.substring(caract.indexOf(pElemento) + pElemento.length);
            var valorFinal = '';
            for (var i=0;i<pNumValores;i++)
            {
                valorFinal += caract.substring(0, caract.indexOf(',')) + ',';
                caract = caract.substring(caract.indexOf(',') + 1);
            }
            return valorFinal;
        }
        else
        {
            return null;
        }        
    },

    GetEsPropiedadGrafoDependiente: function(pEntidad, pPropiedad, pTxtCaract) {
        const that = this;
        return (that.GetCaracteristicaPropiedad(pEntidad, pPropiedad, pTxtCaract, 'propGrafoDep') != null);
    },
    
    ObtenerTextoRepEntidadExterna: function(pValor) {
        const that = this;

        const txtNombreCatTesSem = that.txtNombreCatTesSem == undefined ? $("#mTxtNombreCatTesSem") : that.txtNombreCatTesSem;

        let nombres = txtNombreCatTesSem.val(); // document.getElementById(TxtNombreCatTesSem).value;
    
        if (nombres.indexOf(pValor + '|') != -1) {
            nombres = nombres.substring(nombres.indexOf(pValor + '|') + pValor.length + 1);
            nombres = nombres.substring(0, nombres.indexOf('|||'));
            return nombres;
        }
    
        return '';
    }, 

    AgregarValorADataNoFuncionalProp: function (pEntidad, pPropiedad, pControlContValores, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados){
        const that = this;        
        that.txtHackHayCambios = true;
        
        let valor = that.ObtenerValorEntidadProp(pEntidad + ',' + pPropiedad, pTxtIDs, pTxtCaract);
        if (valor != '' && that.ComprobarValorCampoCorrecto(pEntidad, pPropiedad, valor, pTxtIDs, pTxtCaract))
        {            
            if (that.EsPropiedadMultiIdioma(pEntidad, pPropiedad)) {
                var idControlCampoPes = that.ObtenerControlEntidadProp(pEntidad + ',' + pPropiedad, TxtRegistroIDs).replace('Campo_', 'divContPesIdioma_');
    
                if (document.getElementById(idControlCampoPes) != null) {
                    var li = $('li.active', $('#' + idControlCampoPes));
                    var idiomaActual = li.attr('rel');
                    valorAntiguo = $('#' + idControlCampoPes).attr('langActual');
                    
                    valor = that.IncluirTextoIdiomaEnCadena(valorAntiguo, that.GetValorEncode(valor), idiomaActual);
                    
                    if (!ComprobarMultiIdiomaCorrectoTexto(pEntidad, pPropiedad, valor)) {
                        return;
                    }
    
                    $('#' + idControlCampoPes).attr('langActual', '');
                }
                else { //Es multiIdioma sin pestaña
                    valor += '@' + IdiomaDefectoFormSem + '[|lang|]';
                    const idiomas = IdiomasConfigFormSem.split(',');
                    for (let i = 0; i < idiomas.length; i++) {
                        if (idiomas[i] != IdiomaDefectoFormSem) {                            
                            const valorPropIdioma = that.ObtenerValorEntidadPropDeIdioma(pEntidad + ',' + pPropiedad, pTxtIDs, pTxtCaract, idiomas[i]);
    
                            if (valorPropIdioma != '') {
                                valor += valorPropIdioma + '@' + idiomas[i] + '[|lang|]';
                            }
                        }
                    }
                }
            }
            
            const perteneceEntAGrupoPanel = that.PerteneceEntidadAAlgunGrupoPanelSinEditar(pEntidad, pTxtCaract, pTxtElemEditados);
            let entidadAlmacenar = pEntidad;
    
            if (perteneceEntAGrupoPanel)
            {
                entidadAlmacenar += '&ULT';
            }
            
            that.PutElementoGuardado(entidadAlmacenar, pPropiedad, that.GetValorEncode(valor), pTxtValores, pTxtElemEditados);
            
            that.DarValorControl(pEntidad + ',' + pPropiedad, pTxtIDs, pTxtCaract, '');
            
            if (that.EsPropiedadMultiIdiomaSinPesanya(pEntidad, pPropiedad)) {
                var idiomas = IdiomasConfigFormSem.split(',');
                for (var i = 0; i < idiomas.length; i++) {
                    if (idiomas[i] != IdiomaDefectoFormSem) {                        
                        that.DarValorControlConIdioma(pEntidad + ',' + pPropiedad, pTxtIDs, pTxtCaract, '', idiomas[i]);
                    }
                }
            }
            
            that.AgregarValorAContenedorGrupoValores(pControlContValores, valor, entidadAlmacenar, pPropiedad, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados);
            
            if (that.ExcedeCardinalidadMaxima(pEntidad, pPropiedad, pTxtValores, pTxtCaract, pTxtElemEditados))
            {                                
                const idControlCampo = that.ObtenerControlEntidadProp(pEntidad + ',' + pPropiedad, pTxtIDs);
                document.getElementById(idControlCampo.replace('Campo_','lbCrear_')).style.display = 'none';
            }
        }
    }, 

    AgregarObjectNoFuncionalProp: function(pEntidad, pPropiedad, pEntidadHija, pControlContValores, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados, pVisibleContPaneles, pRaiz, pAntiguoNumElem){
        const that = this;

        that.TxtHackHayCambios = true;

        var rdfBK = $(pTxtValores).val(); //document.getElementById(pTxtValores).value;
        var pEntidadHija = that.ObtenerTipoEntidadEditada(pEntidadHija);
        
        const perteneceEntPanelGrupoSinEditar = that.PerteneceEntidadAAlgunGrupoPanelSinEditar(pEntidad, pTxtCaract, pTxtElemEditados);
        
        const tipoProp = that.GetTipoPropiedad(pEntidad, pPropiedad, pTxtCaract);
        if (!perteneceEntPanelGrupoSinEditar /*||  tipoProp == 'FO' || tipoProp == 'CO'*/)
        {
            const elemAgregado = that.AgregarNuevaEntidadAProp(pEntidad, pPropiedad, pEntidadHija, pTxtValores, pTxtElemEditados);
    
            if (elemAgregado) {                
                that.AgregarPropsEntidadObligatoriasEdicion(pEntidadHija, null, '');
            }
        }
        else if (tipoProp != 'FO' && tipoProp != 'CO')
        {
            var elemAgregado = that.AgregarNuevaEntidadAProp(pEntidad + '&ULT', pPropiedad, pEntidadHija, pTxtValores, pTxtElemEditados);
    
            if (elemAgregado) {
                that.AgregarPropsEntidadObligatoriasEdicion(pEntidadHija, null, '&ULT');
            }
        }
        
        var camposCorrectos = that.AgregarPropiedadesDeNuevaEntidad(pEntidadHija, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados);
    
        if (tipoProp == 'FO' || tipoProp == 'CO') {            
            that.AgregarNuevaEntidadCandidataAProp(pEntidad, pPropiedad, pEntidadHija);
        }
        
        if (!camposCorrectos)
        {
            isJqueryObject(pTxtValores) ? pTxtValores.val(rdfBK) : document.getElementById(pTxtValores).value = rdfBK;
        }
        else
        {
            if (pRaiz) {                
                that.LimpiarControlesEntidad(pEntidadHija, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados);
            }
            
            if (pControlContValores != '')
            {
                that.AgregarEntidadAContenedorGrupoPaneles(pControlContValores, pEntidad, pPropiedad, pEntidadHija, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados, pVisibleContPaneles, true);
            }
            
            if (that.ExcedeCardinalidadMaxima(pEntidad, pPropiedad, pTxtValores, pTxtCaract, pTxtElemEditados))
            {
                var idControlCampo = that.ObtenerControlEntidadProp(pEntidad + ',' + pPropiedad, pTxtIDs);
                
                if (idControlCampo != '')
                {
                    document.getElementById(idControlCampo.replace('panel_contenedor_Entidades_','lbCrear_')).style.display = 'none';
                }
            }            
            that.GuardarDocRecExtSelecEntEditable(pEntidad, pPropiedad, pAntiguoNumElem);
    
            //Borramos la entidad que está como candidata:
            that.DeleteElementoGuardado(null, pEntidadHija + '&ULT', pTxtValores, pTxtElemEditados, 0);
        }
        
        return camposCorrectos;
    },

    GuardarDocRecExtSelecEntEditable: function(pEntidad, pPropiedad, pAntiguoNumElem){
        const that = this;
        var docExtID = that.GetDocRecExtSelecEntEditable(pEntidad, pPropiedad, false, pAntiguoNumElem);
    
        if (docExtID != null) {
            var subOntos = $('#txtSubOntologias').val().split('|||');
            var subOntosFinal = '';
    
            for (var i = 0; i < subOntos.length; i++) {
                if (subOntos[i].indexOf(pEntidad + ',' + pPropiedad + '|') == 0) {
                    var datosSub = subOntos[i].split('|');
    
                    if (datosSub[datosSub.length - 1].split(',')[1] == docExtID) {
                        var numElem = '0';
    
                        if (datosSub.length > 2)
                        {
                            numElem = datosSub[datosSub.length - 2].split(',')[0];
                            numElem++;
                        }
    
                        var newSub = subOntos[i].replace('|-1,' + docExtID + ',', '|' + numElem + ',' + docExtID + ',');
                        newSub += '|-1,' + guidGenerator() + ',';
                        subOntosFinal += newSub + '|||';
                    }
                    else {
                        subOntosFinal += subOntos[i] + '|||';
                    }
                }
                else if (subOntos[i] != '') {
                    subOntosFinal += subOntos[i] + '|||';
                }
            }
    
            $('#txtSubOntologias').val(subOntosFinal);
        }        
    },

    AgregarEntidadAContenedorGrupoPaneles: function(pControlCont, pEntidad, pPropiedad, pEntidadHija, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados, pVisibleContPaneles, pAgregandoNuevoElem){        
        const that = this;

        if (pVisibleContPaneles)
        {
            document.getElementById(pControlCont).style.display = '';
        }

        if (document.getElementById(pControlCont) == null) {
            return false;
        }

        const numElem = (document.getElementById(pControlCont).children[0].children.length - 1);

        // Contenedor de items
        let contenedorHTML = document.getElementById(pControlCont).children[0].innerHTML;
        const fila = that.ObtenerFilaValorContenedorGrupoPaneles(pControlCont, pEntidadHija, numElem, true, pEntidad, pPropiedad, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados, pAgregandoNuevoElem);        
        contenedorHTML = contenedorHTML.replace(document.getElementById(pControlCont).children[0].innerHTML, document.getElementById(pControlCont).children[0].innerHTML + fila);
        document.getElementById(pControlCont).children[0].innerHTML = contenedorHTML;            
    },

    /*
    ObtenerFilaValorContenedorGrupoPaneles: function(pControlCont, pEntidadHija, pNumElem, pAgregarTR, pEntidad, pPropiedad, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados, pAgregandoNuevoElem){        
        const that = this;
        
        var fila = '';
        
        if (pAgregarTR)
        {
            var claseFila = 'impar';

            if(pNumElem %2 ==0)
            {
                claseFila = 'par';
            }

            fila += '<tr class="'+claseFila+'">';
        }
        
        var representantes = that.GetValorAtribRepresentantesEntidad(pEntidadHija, pTxtCaract);

        if (representantes != null)
        {
            if (!pAgregandoNuevoElem) {
                that.MarcarElementoEditado(pEntidad, pPropiedad, pNumElem, pTxtElemEditados, pTxtCaract);
            }

            for (var i=0;i<representantes.length;i++)
            {
                if (representantes[i] != '')
                {
                    var propiedad = representantes[i].split(';')[0];
                    var codigoRepre = representantes[i].split(';')[1];//TODO: Tratar

                    var valor = '';
                    var tipoProp = that.GetTipoPropiedad(pEntidadHija, propiedad, pTxtCaract);
                    if (tipoProp == 'VD') {
                        var valorProp = that.GetValorElementoGuardado(pEntidadHija, propiedad, pTxtValores, pTxtElemEditados, 0);
                        var i = 1;
                        while (valorProp != "") {
                            //valorProp = GetValorDecode(valorProp);
                            valor += valorProp + ', ';
                            valorProp = that.GetValorElementoGuardado(pEntidadHija, propiedad, pTxtValores, pTxtElemEditados, i);
                            i++;
                        }

                        if (valor != '') {
                            valor = valor.substring(0, valor.length - 2);
                        }
                    }
                    else if (tipoProp == 'CSEO' || tipoProp == 'FSEO' || tipoProp == 'LSEO') {
                        valor = that.GetValorElementoGuardado(pEntidadHija, propiedad, pTxtValores, pTxtElemEditados, -1);
                        var idControl = that.ObtenerControlEntidadProp(pEntidadHija + ',' + propiedad, pTxtIDs);

                        if (idControl.indexOf('selEnt_') != -1 && $('#' + idControl).length > 0 && $('#' + idControl)[0].tagName == 'SELECT') {
                            var opcion = $('option[value="' + valor + '"]', $('#' + idControl)[0]);

                            if (opcion.length > 0) {
                                valor = opcion.text();
                            }
                        }
                        else if (idControl.indexOf('selEnt_') != -1 && (tipoProp == 'FSEO' || tipoProp == 'LSEO' || document.getElementById(idControl.replace('selEnt_', 'bkHtmlSelEnt_')) != null)) {
                            valor = that.ReemplarIDsCatTesSemPorNombre(valor);
                        }
                    }
                    else {
                        valor = that.GetValorElementoGuardado(pEntidadHija, propiedad, pTxtValores, pTxtElemEditados, -1);

                        if (that.EsPropiedadMultiIdioma(pEntidadHija, propiedad))
                        {
                            valor = that.ExtraerTextoIdioma(valor, IdiomaDefectoFormSem);
                        }
                    }

                    fila += '<td class="tdval"><span>'+valor+'</span></td>';
                }
            }

            that.MarcarElementoEditado(pEntidad, pPropiedad, -1, pTxtElemEditados, pTxtCaract);
        }
        else
        {
            let propiedades = that.GetPropiedadesEntidad(pEntidadHija, pTxtCaract.prop("id"));

            let pValor = that.GetValorElementoGuardado(pEntidadHija, propiedades[0], pTxtValores, pTxtElemEditados, 0)

            if (pValor.length != 0)
            {
                pValor = that.ReemplarIDsCatTesSemPorNombre(pValor);
                fila += '<td><span>' + that.GetValorDecode(pValor) +'</span></td>';
            }else
            {
              return "";
            }
        }
        fila += '<td class="tdaccion"><a onclick="SeleccionarElementoGrupoPaneles(\''+pEntidad+'\', \''+pPropiedad+'\', \''+pEntidadHija+'\', \''+pNumElem+'\', \''+pTxtValores+'\', \''+pTxtIDs+'\', \''+pTxtCaract+'\', \''+pTxtElemEditados+'\')"><img src="'+ that.GetUrlImg(pTxtCaract)+'icoEditar.gif"></a></td>';
        var metodoEliminar = 'EliminarObjectNoFuncionalProp(\\\''+pNumElem+'\\\',\\\''+pEntidad+'\\\', \\\''+pPropiedad+'\\\', \\\''+pEntidadHija+'\\\', \\\''+pControlCont+'\\\', \\\''+pTxtValores+'\\\', \\\''+pTxtIDs+'\\\', \\\''+pTxtCaract+'\\\', \\\''+pTxtElemEditados+'\\\');';
        
        var textoEliminar = "¿Desea eliminar el elemento seleccionado?";// that.GetMensajeEliminar(pEntidad,pPropiedad,pTxtCaract);

        if(textoEliminar==null )
        {
            fila += '<td class="tdaccion"><a class="remove" onclick="MostrarPanelConfirmacionEvento(event, \''+textoFormSem.confimEliminarEntidad+'\', \''+metodoEliminar+'\')"></a></td>';    
        }else
        {
            fila += '<td class="tdaccion"><a class="remove" onclick="MostrarPanelConfirmacionEvento(event, \''+textoEliminar+'\', \''+metodoEliminar+'\')"></a></td>';    
        }
        
        if (pAgregarTR)
        {
            fila += '</tr>';
        }

        return fila;  
    },
    */

    ObtenerFilaValorContenedorGrupoPaneles: function(pControlCont, pEntidadHija, pNumElem, pAgregarTR, pEntidad, pPropiedad, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados, pAgregandoNuevoElem){        
        const that = this;
        
        var fila = '';
                
        var representantes = that.GetValorAtribRepresentantesEntidad(pEntidadHija, pTxtCaract);

        if (representantes != null)
        {
            if (!pAgregandoNuevoElem) {
                that.MarcarElementoEditado(pEntidad, pPropiedad, pNumElem, pTxtElemEditados, pTxtCaract);
            }

            for (var i=0; i<representantes.length; i++)
            {
                if (representantes[i] != '')
                {
                    var propiedad = representantes[i].split(';')[0];
                    var codigoRepre = representantes[i].split(';')[1];//TODO: Tratar

                    var valor = '';
                    var tipoProp = that.GetTipoPropiedad(pEntidadHija, propiedad, pTxtCaract);
                    if (tipoProp == 'VD') {
                        var valorProp = that.GetValorElementoGuardado(pEntidadHija, propiedad, pTxtValores, pTxtElemEditados, 0);
                        var i = 1;
                        while (valorProp != "") {
                            //valorProp = GetValorDecode(valorProp);
                            valor += valorProp + ', ';
                            valorProp = that.GetValorElementoGuardado(pEntidadHija, propiedad, pTxtValores, pTxtElemEditados, i);
                            i++;
                        }

                        if (valor != '') {
                            valor = valor.substring(0, valor.length - 2);
                        }
                    }
                    else if (tipoProp == 'CSEO' || tipoProp == 'FSEO' || tipoProp == 'LSEO') {
                        valor = that.GetValorElementoGuardado(pEntidadHija, propiedad, pTxtValores, pTxtElemEditados, -1);
                        var idControl = that.ObtenerControlEntidadProp(pEntidadHija + ',' + propiedad, pTxtIDs);

                        if (idControl.indexOf('selEnt_') != -1 && $('#' + idControl).length > 0 && $('#' + idControl)[0].tagName == 'SELECT') {
                            var opcion = $('option[value="' + valor + '"]', $('#' + idControl)[0]);

                            if (opcion.length > 0) {
                                valor = opcion.text();
                            }
                        }
                        else if (idControl.indexOf('selEnt_') != -1 && (tipoProp == 'FSEO' || tipoProp == 'LSEO' || document.getElementById(idControl.replace('selEnt_', 'bkHtmlSelEnt_')) != null)) {
                            valor = that.ReemplarIDsCatTesSemPorNombre(valor);
                        }
                    }
                    else {
                        valor = that.GetValorElementoGuardado(pEntidadHija, propiedad, pTxtValores, pTxtElemEditados, -1);

                        if (that.EsPropiedadMultiIdioma(pEntidadHija, propiedad))
                        {
                            valor = that.ExtraerTextoIdioma(valor, IdiomaDefectoFormSem);
                        }
                    }

                    fila += '<td class="tdval"><span>'+valor+'</span></td>';
                }
            }

            that.MarcarElementoEditado(pEntidad, pPropiedad, -1, pTxtElemEditados, pTxtCaract);
        }
        else
        {
            let propiedades = "";             
            if (isJqueryObject(pTxtCaract)){
                propiedades = that.GetPropiedadesEntidad(pEntidadHija, pTxtCaract.prop("id"));
            }else{
                propiedades = that.GetPropiedadesEntidad(pEntidadHija, pTxtCaract);
            }            

            let pValor = that.GetValorElementoGuardado(pEntidadHija, propiedades[0], pTxtValores, pTxtElemEditados, 0);

            if (pValor.length != 0)
            {
                pValor = that.ReemplarIDsCatTesSemPorNombre(pValor);
                // Pinto la fila más abajo
                // fila += '<td><span>' + that.GetValorDecode(pValor) +'</span></td>';
            }else
            {
              return "";
            }
        }

        // Obtener los ids si se tratan de objetos jquery o no
        const pTxtValoresId = isJqueryObject(pTxtValores) ? pTxtValores.prop("id") : pTxtValores;
        const pTxtIDsId = isJqueryObject(pTxtIDs) ? pTxtIDs.prop("id") : pTxtIDs;
        const pTxtCaractId = isJqueryObject(pTxtCaract) ? pTxtCaract.prop("id") : pTxtCaract;
        const pTxtElemEditadosId = isJqueryObject(pTxtElemEditados) ? pTxtElemEditados.prop("id") : pTxtElemEditados;
        
        
        // Método para editar el item para colocar la información en los inputs
        //const metodoSeleccionarElementoGrupoPaneles = '"SeleccionarElementoGrupoPaneles(\''+pEntidad+'\', \''+pPropiedad+'\', \''+pEntidadHija+'\', \''+pNumElem+'\', \''+pTxtValores.prop("id")+'\', \''+pTxtIDs.prop("id")+'\', \''+pTxtCaract.prop("id")+'\', \''+pTxtElemEditados.prop("id")+'\')"';        
        const metodoSeleccionarElementoGrupoPaneles = `operativaGestionObjetosConocimientoOntologias.SeleccionarElementoGrupoPaneles('${pEntidad}','${pPropiedad}','${pEntidadHija}','${pNumElem + 1}','${pTxtValoresId}', '${pTxtIDsId}', '${pTxtCaractId}', '${pTxtElemEditadosId}')`;
        // Método para eliminar
        const metodoEliminar = `operativaGestionObjetosConocimientoOntologias.EliminarObjectNoFuncionalProp('${pNumElem + 1}','${pEntidad}','${pPropiedad}', '${pEntidadHija}', '${pControlCont}', '${pTxtValoresId}', '${pTxtIDsId}', '${pTxtCaractId}', '${pTxtElemEditadosId}');`;
        
        var textoEliminar = "¿Desea eliminar el elemento seleccionado?";// that.GetMensajeEliminar(pEntidad,pPropiedad,pTxtCaract);
        // Valor a mostrar

        if (representantes == null){        
            let propiedades = that.GetPropiedadesEntidad(pEntidadHija, pTxtCaractId);
            let pValor = that.GetValorElementoGuardado(pEntidadHija, propiedades[0], pTxtValores, pTxtElemEditados, 0);
            
            if (pValor.length != 0)
            {
                pValor = that.ReemplarIDsCatTesSemPorNombre(pValor);
                // Pinto la fila más abajo
                const pValorDecode = that.GetValorDecode(pValor);
            
                /*
                fila += `
                <li class="component-wrap containerConfirmDeleteItemInModal">
                    <div class="component">
                        <div class="component-header-wrap">
                            <div class="component-header">
                                <div class="component-header-content">
                                    <div class="component-header-left">
                                        <div class="component-name-wrap">
                                            <span class="component-item-name">
                                                ${pValorDecode}
                                            </span>
                                        </div>
                                    </div>
                `;                            
    
                                    // Controlar si se pueden mostrar "borrado" del item
                                    if (metodoEliminar != '') {
                                        fila +=`
                                            <div class="component-header-right">
                                                <div class="component-actions-wrap">
                                                    <ul class="no-list-style component-actions">
                                                        <li>
                                                            <a class="action-delete round-icon-button js-action-delete btnDeleteObjectNoFuncionalProp" href="javascript: void(0);" data-handleclick="${metodoEliminar}">
                                                                <span class="material-icons">delete</span>
                                                            </a>
                                                        </li>
                                                    </ul>
                                                </div>
                                            </div>
                                        `;
                                    }  
                // Footer de la fila                                          
                fila +=` 
                                </div>
                            </div>
                        </div>
                    </div>
                </li>
                `;*/ 

                fila += `
                <li class="component-wrap containerConfirmDeleteItemInModal">
                    <div class="component">
                        <div class="component-header-wrap">
                            <div class="component-header">
                                <div class="component-header-content">
                                    <div class="component-header-left">
                                        <div class="component-name-wrap">
                                            <span class="component-item-name">
                                                ${pValorDecode}
                                            </span>
                                        </div>
                                    </div>
        
                                    <div class="component-header-right">
                                        <div class="component-actions-wrap">
                                            <ul class="no-list-style component-actions">
                                            `;
                                                if (metodoEliminar != '') {
                                                    fila +=`
                                                    <li>
                                                        <a class="action-edit round-icon-button js-action-edit-component btnEditObjectNoFuncionalProp" href="javascript: void(0);" onclick="${metodoSeleccionarElementoGrupoPaneles}">
                                                            <span class="material-icons">edit</span>
                                                        </a>
                                                    </li>`;                  
                                                }  
                                                if (metodoEliminar !=""){
                                                    fila +=`
                                                    <li>
                                                        <a class="action-delete round-icon-button js-action-delete btnDeleteObjectNoFuncionalProp" href="javascript: void(0);" data-handleclick="${metodoEliminar}">
                                                            <span class="material-icons">delete</span>
                                                        </a>
                                                    </li>
                                                    `;
                                                }
                                            fila +=`                                                                            
                                            </ul>
                                        </div>
                                    </div>                                                              
                                </div>
                            </div>
                        </div>
                    </div>
                </li>
                `;                               
            }else
            {
              return "";
            }              
        }   

        return fila;  
    },    

    GetValorAtribRepresentantesEntidad: function(pEntidad, pTxtCaract){
        const that = this;
        if (pEntidad.indexOf('_bis') != -1)
        {
            pEntidad = pEntidad.substring(0, pEntidad.indexOf('_bis'));
        }
    
        var atriRepre = that.GetCaracteristicaPropiedad(pEntidad, '', pTxtCaract, 'atrRepre');
    
        if (atriRepre != null && atriRepre != '')
        {
            return atriRepre.split('&');
        }
        else
        {
            return null;
        }        
    },

    LimpiarControlesEntidad: function(pEntidadHija, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados){
        const that = this;
        const entidadPadre = pEntidadHija;
        
        const subClases = that.GetSubClasesEntidad(pEntidadHija);
    
        if (subClases != null && subClases.length > 0) {
            for (var i = 0; i < subClases.length; i++) {
                if (that.EntidadSubClaseSeleccionada(subClases[i], false)) {
                    pEntidadHija = subClases[i];
                    break;
                }
            }
        }
        
        // Tener en cuenta si es un objeto jquery o el id
        const currentpTxtCaract = isJqueryObject(pTxtCaract) ? pTxtCaract.prop("id") : pTxtCaract;
        var propiedades = that.GetPropiedadesEntidad(pEntidadHija, currentpTxtCaract);
    
        for (var i = 0; i < propiedades.length; i++) {
            if (propiedades[i] != '') {                
                that.LimpiarControlesPropiedadDeEntidad(pEntidadHija, propiedades[i], pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados);
            }
        }
    
        if (entidadPadre != pEntidadHija) {
            //Limpio otras hijas:
            for (var i = 0; i < subClases.length; i++) {
                if (subClases[i] != '' && subClases[i] != pEntidadHija) {
                    that.LimpiarControlesEntidad(subClases[i], pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados);
                }
            }
    
            var combosSubClase = $('select.cmbSubClass_' + that.ObtenerTextoGeneracionIDs(entidadPadre));
    
            if (combosSubClase.length > 0) {
                combosSubClase[0].style.display = '';
            }
        }        
    },

    LimpiarControlesPropiedadDeEntidad: function(pEntidad, pPropiedad, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados){
        const that = this;
        //TODO:Revisar
        var tipoProp = that.GetTipoPropiedad(pEntidad, pPropiedad, pTxtCaract);
        
        if (tipoProp == 'FD' || tipoProp == 'CD' || tipoProp == 'VD')
        {
            that.DarValorControl(pEntidad + ',' + pPropiedad, pTxtIDs, pTxtCaract, '');
            that.LimpiarHtmlControl(that.GetIDControlError(pEntidad, pPropiedad, pTxtIDs));
        
            if (that.EsPropiedadMultiIdiomaSinPesanya(pEntidad, pPropiedad)) {
                var idiomas = IdiomasConfigFormSem.split(',');
                for (var i=0;i<idiomas.length;i++)
                {
                    if (idiomas[i] != IdiomaDefectoFormSem)
                    {
                        that.DarValorControlConIdioma(pEntidad + ',' + pPropiedad, pTxtIDs, pTxtCaract, '', idiomas[i]);
                    }
                }
            }
            else if (that.EsPropiedadMultiIdioma(pEntidad, pPropiedad)) {//Con pestaña por eliminación
                var controlPropMulti = that.ObtenerControlEntidadProp(pEntidad + ',' + pPropiedad, TxtRegistroIDs);
            
                if (controlPropMulti != null) {
                    var idControlCampoPes = controlPropMulti.replace('Campo_', 'divContPesIdioma_');
                
                    if (idControlCampoPes != null) {
                        $('#' + idControlCampoPes).attr('langActual', '');
                    }
                }
            }
        }
        else if (tipoProp == 'LD' || tipoProp == 'LSEO')
        {
            that.DarValorControl(pEntidad + ',' + pPropiedad, pTxtIDs, pTxtCaract, '');
            var idControlCampo = that.ObtenerControlEntidadProp(pEntidad + ',' + pPropiedad, pTxtIDs);
        
            if (idControlCampo != '' && document.getElementById(idControlCampo.replace('Campo_', 'contedor_Valores_').replace('selEnt_', 'contEntSelec_')) != null) {
                document.getElementById(idControlCampo.replace('Campo_', 'contedor_Valores_').replace('selEnt_', 'contEntSelec_')).innerHTML = '';                
                that.EstablecerBotonesGrupoValores(pEntidad, pPropiedad, pTxtIDs);
            }
        
            if ($('#' + idControlCampo).length > 0 && $('#' + idControlCampo)[0].nodeName == 'SELECT') {
                $('option', $('#' + idControlCampo)[0]).removeAttr('disabled');
            }
        }
        else if (tipoProp == 'FO' || tipoProp == 'CO')
        {
            var entidadHija = that.GetRangoPropiedad(pEntidad, pPropiedad, pTxtCaract);
            that.LimpiarControlesEntidad(entidadHija, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados);
        }
        else if (tipoProp == 'LO')
        {
            that.LimpiarControlesEntidad(that.GetRangoPropiedad(pEntidad, pPropiedad, pTxtCaract), pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados);            
            that.LimpiarContenedorGrupoPaneles(pEntidad, pPropiedad, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados);            
            that.EstablecerBotonesEntidad(pEntidad, pPropiedad, true, pTxtIDs);
        }
        else if (tipoProp == 'FSEO' || tipoProp == 'CSEO')
        {
            that.DarValorControl(pEntidad + ',' + pPropiedad, pTxtIDs, pTxtCaract, '');
            that.LimpiarHtmlControl(that.GetIDControlError(pEntidad, pPropiedad, pTxtIDs));
        
            var idControl = that.ObtenerControlEntidadProp(pEntidad + ',' + pPropiedad, pTxtIDs);
            if (idControl.indexOf('selEnt_') != -1) 
            {
                var inputHack = $('#' + idControl.replace('selEnt_', 'hack_'));
                if (inputHack.prop("disabled") && inputHack.hasClass("autocompletarSelecEnt")) {
                    inputHack.prop("disabled", false);
                    var aspa = $('a.removeAutocompletar', $('#' + idControl).closest('div.cont'));
                    if (aspa.length > 0) {
                        aspa.remove();
                    }
                }
                if (document.getElementById(idControl.replace('selEnt_', 'bkHtmlSelEnt_')) != null) {
                    that.ResetearSeleccTesSem(idControl);
                    $('#' + idControl.replace('selEnt_', 'divContControlTes_')).css('display', '');
                    $('#' + idControl.replace('selEnt_', 'contEntSelec_')).css('display', 'none');
                }
            }
        }
    },

    ResetearSeleccTesSem: function(pIdControl){
        const that = this;

        const divID = pIdControl.replace("selEnt_", "divCheckEnt_");
        const bkID = pIdControl.replace("selEnt_", "bkHtmlSelEnt_");
        const idNameTesHack = pIdControl.replace("selEnt_", "hackTesNameSelEnt_");
        
        if ($('#' + bkID).val() != '') {
            $('#' + divID).html($('#' + bkID).val());
            $('#' + pIdControl).val('');
            $('#' + bkID).val('');
            $('#' + idNameTesHack).val('');
        }
        
    },

    EstablecerBotonesEntidad: function(pEntidad, pPropiedad, pAgregar, pTxtIDs){
        const that = this;

        var idControlCampo = that.ObtenerControlEntidadProp(pEntidad + ',' + pPropiedad, pTxtIDs);

        if (document.getElementById(idControlCampo.replace('panel_contenedor_Entidades_', 'lbCrear_')) == null) {
            return false;
        }
        
        if (pAgregar)
        {
            document.getElementById(idControlCampo.replace('panel_contenedor_Entidades_','lbCrear_')).style.display = '';
            document.getElementById(idControlCampo.replace('panel_contenedor_Entidades_','lbGuardar_')).style.display = 'none';
            document.getElementById(idControlCampo.replace('panel_contenedor_Entidades_','lbCancelar_')).style.display = 'none';
        }
        else
        {
            document.getElementById(idControlCampo.replace('panel_contenedor_Entidades_','lbCrear_')).style.display = 'none';
            document.getElementById(idControlCampo.replace('panel_contenedor_Entidades_','lbGuardar_')).style.display = '';
            document.getElementById(idControlCampo.replace('panel_contenedor_Entidades_','lbCancelar_')).style.display = '';
        }
        
        
    },

    LimpiarContenedorGrupoPaneles: function(pEntidad, pPropiedad, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados){
        const that = this;

        var contenedor = that.ObtenerControlEntidadProp(pEntidad + ',' + pPropiedad, pTxtIDs);
        
        if (contenedor != '' && document.getElementById(contenedor) != null)
        {
            var htmlFinal = '<tr>' + document.getElementById(contenedor).children[0].children[0].children[0].innerHTML + '</tr>';

            var contenedorHTML=document.getElementById(contenedor).innerHTML;
            contenedorHTML=contenedorHTML.replace(document.getElementById(contenedor).children[0].children[0].innerHTML,htmlFinal);       
            document.getElementById(contenedor).innerHTML=contenedorHTML;

            document.getElementById(contenedor).style.display='none';
        }               
    },

    EstablecerBotonesGrupoValores: function(pEntidad, pPropiedad, pTxtIDs){
        const that = this;
        var idControlCampo = that.ObtenerControlEntidadProp(pEntidad + ',' + pPropiedad, pTxtIDs);
        
        if (document.getElementById(idControlCampo.replace('Campo_', 'lbCrear_').replace('selEnt_', 'lbCrear_')).style.display != 'none' || document.getElementById(idControlCampo.replace('Campo_', 'lbGuardar_').replace('selEnt_', 'lbGuardar_')).style.display != 'none') {
            document.getElementById(idControlCampo.replace('Campo_', 'lbCrear_').replace('selEnt_', 'lbCrear_')).style.display = '';
            document.getElementById(idControlCampo.replace('Campo_', 'lbGuardar_').replace('selEnt_', 'lbGuardar_')).style.display = 'none';
        }        
    },

    GetSubClasesEntidad: function(pEntidad){
        const that = this;

        const txtCaracteristicasElem = that.txtCaracteristicasElem == undefined ? "mTxtCaracteristicasElem" : that.txtCaracteristicasElem;

        var subs = that.GetCaracteristicaPropiedad(pEntidad, '', txtCaracteristicasElem, 'subclases');

        if (subs != null)
        {
            var subclases = subs.split(';');
            return subclases;
        }
    
        return null;
    },

    AgregarNuevaEntidadCandidataAProp(pEntidad, pPropiedad, pEntidadHija) {
        const that = this;
        var valorProp = that.GetValorElementoGuardado(pPropiedad, pEntidadHija, TxtValorRdf, TxtElemEditados, 0);
        that.PutElementoGuardado(pEntidad, pPropiedad, '<' + pEntidadHija + '>' + valorProp + '</' + pEntidadHija + '>', TxtValorRdf, TxtElemEditados);
    },

    AgregarPropiedadesDeNuevaEntidad: function(pEntidadHija, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados){
        const that = this;
        
            //var propiedades = that.GetPropiedadesEntidad(pEntidadHija, pTxtCaract);
            var propiedades = "";            
            if (isJqueryObject(pTxtCaract)){
                propiedades = that.GetPropiedadesEntidad(pEntidadHija, pTxtCaract.prop("id"));
            }else{
                propiedades = that.GetPropiedadesEntidad(pEntidadHija, pTxtCaract);
            }
                        
            var camposCorrectos = true;
            
            for (var i=0;i<propiedades.length;i++)
            {
                if (propiedades[i] != '')
                {                    
                    camposCorrectos = (that.AgregarPropiedadDeEntidad(pEntidadHija, propiedades[i], pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados) && camposCorrectos);
                }
            }
            
            return camposCorrectos;               
    },

    AgregarPropiedadDeEntidad: function(pEntidad, pPropiedad, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados){
        const that = this;
        const tipoProp = that.GetTipoPropiedad(pEntidad, pPropiedad, pTxtCaract);
        var camposCorrectos = true;
    
        if (tipoProp == 'FD' || tipoProp == 'CD' || tipoProp == 'FSEO' || tipoProp == 'CSEO')
        {
            var valorProp = that.ObtenerValorEntidadProp(pEntidad + ',' + pPropiedad, pTxtIDs, pTxtCaract);
            var campoCorrecto = that.ComprobarValorCampoCorrecto(pEntidad, pPropiedad, valorProp, pTxtIDs, pTxtCaract);
            camposCorrectos = camposCorrectos && campoCorrecto;
    
            if (campoCorrecto && that.EsPropiedadMultiIdioma(pEntidad, pPropiedad)) {
                var idControlCampoPes = that.ObtenerControlEntidadProp(pEntidad + ',' + pPropiedad, TxtRegistroIDs).replace('Campo_', 'divContPesIdioma_');
    
                if (document.getElementById(idControlCampoPes) != null) {
                    var li = $('li.active', $('#' + idControlCampoPes));
                    var idiomaActual = li.attr('rel');
                    valorAntiguo = $('#' + idControlCampoPes).attr('langActual');
                    valorProp = that.IncluirTextoIdiomaEnCadena(valorAntiguo, that.GetValorEncode(valorProp), idiomaActual);
    
                    if (!that.ComprobarMultiIdiomaCorrectoTexto(pEntidad, pPropiedad, valorProp)) {
                        campoCorrecto = false;
                        camposCorrectos = false;
                    }
                }
                else { //Es multiIdioma sin pestaña
                    valorProp += '@' + IdiomaDefectoFormSem + '[|lang|]';
                    var idiomas = IdiomasConfigFormSem.split(',');
                    for (var i=0;i<idiomas.length;i++)
                    {
                        if (idiomas[i] != IdiomaDefectoFormSem)
                        {
                            var valorPropIdioma = that.ObtenerValorEntidadPropDeIdioma(pEntidad + ',' + pPropiedad, pTxtIDs, pTxtCaract, idiomas[i]);
    
                            if (valorPropIdioma != '') {
                                valorProp += valorPropIdioma + '@' + idiomas[i] + '[|lang|]';
                            }
                        }
                    }
                }
            }
            
            if (campoCorrecto && valorProp != null && valorProp != '')
            {
                var valorDefNoSelec = that.GetValorDefNoSelec(pEntidad, pPropiedad, pTxtCaract);
                if (valorDefNoSelec == null || valorDefNoSelec == '' || valorDefNoSelec != valorProp)
                {
                    var valorAnt = that.GetValorElementoGuardado(pEntidad, pPropiedad, pTxtValores, pTxtElemEditados, 0);
    
                    if (valorAnt != '') {
                        that.DeleteElementoGuardado(pEntidad, pPropiedad, pTxtValores, pTxtElemEditados, 0);
                    }
    
                    valorProp = that.GetValorEncode(valorProp);
                    that.PutElementoGuardado(pEntidad, pPropiedad, valorProp, pTxtValores, pTxtElemEditados);
                }
            }
            else if (valorProp == null && that.ObtenerControlEntidadProp(pEntidad + ',' + pPropiedad, pTxtIDs) == '') {//Propiedad no editable de entidad, no se quiere que se pierda el valor
                valorProp = that.GetValorElementoGuardado(pEntidad + '&ULT', pPropiedad, pTxtValores, pTxtElemEditados, 0);
    
                if (valorProp != null && valorProp != '') {
                    that.DeleteElementoGuardado(pEntidad + '&ULT', pPropiedad, pTxtValores, pTxtElemEditados, 0);
                    that.PutElementoGuardado(pEntidad, pPropiedad, valorProp, pTxtValores, pTxtElemEditados);
                }
            }
        }
        else if (tipoProp == 'VD')
        {
            var valorProp = that.ObtenerValorEntidadProp(pEntidad + ',' + pPropiedad, pTxtIDs, pTxtCaract);
    
            if (valorProp != null) {
                var valoresProp = valorProp.split(',');
                for (var i = 0; i < valoresProp.length; i++) {
                    if (valoresProp[i] != '') {
                        that.PutElementoGuardado(pEntidad, pPropiedad, that.GetValorEncode(valoresProp[i]), pTxtValores, pTxtElemEditados);
                    }
                }
            }
            else if (that.ObtenerControlEntidadProp(pEntidad + ',' + pPropiedad, pTxtIDs) == '') {//Propiedad no editable de entidad, no se quiere que se pierda el valor
                var valorProp = that.GetValorElementoGuardado(pEntidad + '&ULT', pPropiedad, pTxtValores, pTxtElemEditados, 0);
                
                while (valorProp != '') {
                    that.DeleteElementoGuardado(pEntidad + '&ULT', pPropiedad, pTxtValores, pTxtElemEditados, 0);
                    that.PutElementoGuardado(pEntidad, pPropiedad, valorProp, pTxtValores, pTxtElemEditados);
                    valorProp = that.GetValorElementoGuardado(pEntidad + '&ULT', pPropiedad, pTxtValores, pTxtElemEditados, 0);
                }
            }
            
            camposCorrectos = (that.ComprobarCardinalidadPropiedad(pEntidad, pPropiedad, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados) && camposCorrectos);
        }
        else if (tipoProp == 'LD' || tipoProp == 'LO' || tipoProp == 'LSEO')
        {
            var valorProp = that.GetValorElementoGuardado(pEntidad + '&ULT', pPropiedad, pTxtValores, pTxtElemEditados, 0);
            
            if (valorProp != '') //Valores en entidad auxiliar
            {
                while (valorProp != '')
                {
                    that.DeleteElementoGuardado(pEntidad + '&ULT', pPropiedad, pTxtValores, pTxtElemEditados, 0);
                    that.PutElementoGuardado(pEntidad, pPropiedad, valorProp, pTxtValores, pTxtElemEditados);
                    valorProp = that.GetValorElementoGuardado(pEntidad + '&ULT', pPropiedad, pTxtValores, pTxtElemEditados, 0);
                }
                
                if (that.EntidadEstaVacia(pEntidad + '&ULT', pTxtValores))
                {
                    that.DeleteElementoGuardado(null, pEntidad + '&ULT', pTxtValores, pTxtElemEditados, 0);
                }
            }
            
            camposCorrectos = (that.ComprobarCardinalidadPropiedad(pEntidad, pPropiedad, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados) && camposCorrectos);
        }
        else if (tipoProp == 'FO' || tipoProp == 'CO')
        {//TODO: ESTO FUNCIONA???
            var entidadHija = that.GetRangoPropiedad(pEntidad, pPropiedad, pTxtCaract);
            var contenedor = '';//TODO: Poner contenedor?
            camposCorrectos = (that.AgregarObjectNoFuncionalProp(pEntidad, pPropiedad, entidadHija, contenedor, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados, false, false, -1) && camposCorrectos);
        }
        
        return camposCorrectos;
    },

    EntidadEstaVacia: function(pEntidad, pTxtValores){
        var valores = document.getElementById(pTxtValores).value;
        return (valores.indexOf('<' + pEntidad + '>' + '</' + pEntidad + '>') != -1);
    },    

    AgregarPropsEntidadObligatoriasEdicion: function (pEntidad, pPropiedadYaAgregada, pExtraEnt) {
        const that = this;
        let propiedades = that.GetPropiedadesEntidad(pEntidad, that.txtCaracteristicasElem.prop("id")); // that.GetPropiedadesEntidad(pEntidad, TxtCaracteristicasElem);
        for (var i = 0; i < propiedades.length; i++) {
            if (propiedades[i] != '' && propiedades[i] != pPropiedadYaAgregada) {
                //const tipoProp = that.GetTipoPropiedad(pEntidad, propiedades[i], TxtCaracteristicasElem);
                const tipoProp = that.GetTipoPropiedad(pEntidad, propiedades[i], that.txtCaracteristicasElem);
    
                if (tipoProp == 'FO' || tipoProp == 'CO') {
                    let entidadHija = that.GetRangoPropiedad(pEntidad, propiedades[i], that.txtCaracteristicasElem);                    
                    entidadHija = that.ObtenerTipoEntidadEditada(entidadHija);                         
                    that.PutElementoGuardado(pEntidad + pExtraEnt, propiedades[i], '<' + entidadHija + '></' + entidadHija + '>', that.txtValorRdf, that.txtElemEditados);
                }
            }
        }
    },  
    
    ObtenerTipoEntidadEditada: function(pTipoEntidad){
        const that = this;
        var superClase = that.GetCaracteristicaPropiedad(pTipoEntidad, '', that.txtCaracteristicasElem, 'superclase');

        if (superClase != null && superClase == 'true') {            
            var combosSubClase = $('select.cmbSubClass_' + that.ObtenerTextoGeneracionIDs(pTipoEntidad));
    
            if (combosSubClase.length > 0) {
                var tipoEntidad = combosSubClase[0].value;
                return tipoEntidad;
            }
        }    
        return pTipoEntidad;
    },

    ObtenerTextoGeneracionIDs: function(pTexto){        
        return pTexto.replace(/\//g,'_').replace(/\:/g,'_').replace(/\./g,'_').replace(/\#/g,'_');        
    },

    AgregarNuevaEntidadAProp: function(pEntidad, pPropiedad, pEntidadHija, pTxtValores, pTxtElemEditados){
        const that = this;        
        return that.PutElementoGuardado(pEntidad, pPropiedad, '<' + pEntidadHija + '></' + pEntidadHija + '>', pTxtValores, pTxtElemEditados);
    },

    ExcedeCardinalidadMaxima: function(pEntidad, pPropiedad, pTxtValores, pTxtCaract, pTxtElemEditados){
        const that = this;
        const cardinalidad = that.GetCardinalidadPropiedad(pEntidad, pPropiedad, pTxtCaract);
        
        if (cardinalidad != null)
        {
            var tipoCardi = cardinalidad.split(',')[0];
            var numCardi = parseInt(cardinalidad.split(',')[1]);
            var numElemProp = that.GetNumElementosPropiedad(pEntidad, pPropiedad, pTxtValores, pTxtElemEditados);
            
            if ((tipoCardi == 'Cardinality' || tipoCardi == 'MaxCardinality') && numElemProp >= numCardi)
            {
                return true;
            }
        }
        
        return false;
    },   

    AgregarValorAContenedorGrupoValores: function(pControlCont, pValor, pEntidad, pPropiedad, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados)
    {
        const that = this;
        
        if (document.getElementById(pControlCont) != null) {
            if (pControlCont.indexOf('contEntSelec_') != -1 && document.getElementById(pControlCont.replace('contEntSelec_', 'bkHtmlSelEnt_')) != null) {
                
                pValor = that.ReemplarIDsCatTesSemPorNombre(pValor);
            }
            
            pValor = that.GetValorDecode(pValor);
            if (pValor.indexOf('[|lang|]') != -1) {                
                pValor = that.ExtraerTextoIdioma(pValor, IdiomaDefectoFormSem)
            }
    
            if (document.getElementById(pControlCont).innerHTML == '')//Creo la tabla
            {                
                // Creador UL en lugar de tabla - 
                // document.getElementById(pControlCont).innerHTML = '<table><tbody>' + that.ObtenerFilaValorContenedorGrupoValores(pControlCont, pValor, 0, true, pEntidad, pPropiedad, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados) + '</tbody></table>';                
                // Item a añadir
                const item = that.ObtenerFilaValorContenedorGrupoValores(pControlCont, pValor, 0, true, pEntidad, pPropiedad, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados);
                document.getElementById(pControlCont).innerHTML = `
                <ul class="no-list-style mt-2" style="background-color: #fff; border: 1px solid var(--c-gris-borde);">
                ${item}
                </ul>                                                        
                `; 
            }
            else {
                const numElem = document.getElementById(pControlCont).children[0].children.length;
                document.getElementById(pControlCont).children[0].innerHTML += that.ObtenerFilaValorContenedorGrupoValores(pControlCont, pValor, numElem, true, pEntidad, pPropiedad, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados);
            }
        }
    },

    /* Cambiado para DevTools */
    /*
    ObtenerFilaValorContenedorGrupoValores: function(pControlCont, pValor, pNumElem, pAgregarTR, pEntidad, pPropiedad, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados){
        const that = this;
        
        var fila = '';
        
        if (pAgregarTR)
        {
            var claseFila = 'impar';
            
            if(pNumElem %2 ==0)
            {
                claseFila = 'par';
            }
            
            fila += '<tr class="'+claseFila+'">';
        }
        
        var funcionSel = 'SeleccionarElementoGrupoValores';
        var funcionEli = 'EliminarValorDeDataNoFuncionalProp';
        
        if (pControlCont.indexOf('contEntSelec_') != -1)//Es entidad seleccionada
        {
            funcionSel = 'SeleccionarObjectNoFuncionalSeleccEnt';
            funcionEli = 'EliminarObjectNoFuncionalSeleccEnt';
    
            if (document.getElementById(pControlCont.replace('contEntSelec_', 'hack_')) != null || document.getElementById(pControlCont.replace('contEntSelec_', 'bkHtmlSelEnt_')) != null) {
                funcionSel = '';
            }
        }
    
        fila += '<td><span>' + pValor + '</span></td>';
    
        if (funcionSel != '') {
            fila += '<td><a onclick="' + funcionSel + '(\'' + pEntidad + '\', \'' + pPropiedad + '\', \'' + pNumElem + '\', \'' + pTxtValores.prop("id") + '\', \'' + pTxtIDs.prop("id") + '\', \'' + pTxtCaract.prop("id") + '\', \'' + pTxtElemEditados.prop("id") + '\')"><img src="' + that.GetUrlImg(pTxtCaract) + 'icoEditar.gif"></a></td>';
        }
    
        // Cambio por nuevo Front
        //var metodoEliminar = funcionEli + '(\\\'' + pNumElem + '\\\',\\\'' + pEntidad + '\\\', \\\'' + pPropiedad + '\\\', \\\'' + pControlCont + '\\\', \\\'' + pTxtValores + '\\\', \\\'' + pTxtIDs + '\\\', \\\'' + pTxtCaract + '\\\', \\\'' + pTxtElemEditados + '\\\');';
        var metodoEliminar = `${funcionEli}('${pNumElem}', '${pEntidad}', '${pPropiedad}', '${pControlCont}', '${pTxtValores}', '${pTxtIDs}', '${pTxtCaract}', '${pTxtElemEditados}')`;
    

        fila += `<td>
                    <button
                    type="button"
                    class="btn removeButton"
                    data-showmodalcentered="1"
                    onclick="
                            $('#modal-container').modal('show', this);
                            AccionFichaPerfil(
                                'Eliminar',
                                'SI',
                                'NO',
                                '${textoFormSem.confimEliminar.replace('@1@', pValor)}',
                                'sin-definir',
                                function () {
                                  ${metodoEliminar}
                                }
                            );
                            "
                    >
                        <span class="material-icons pr-0">delete</span>
                    </button>
    
                </td>`;
        
        
        if (pAgregarTR)
        {
            fila += '</tr>';
        }
        
        return fila;
        
    },
    */
    ObtenerFilaValorContenedorGrupoValores: function(pControlCont, pValor, pNumElem, pAgregarTR, pEntidad, pPropiedad, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados){
        const that = this;
        
        let fila = '';

        // Acciones a realizar por el item
        let funcionSel = 'operativaGestionObjetosConocimientoOntologias.SeleccionarElementoGrupoValores';
        let metodoSeleccionar = '';
        let funcionEli = 'operativaGestionObjetosConocimientoOntologias.EliminarValorDeDataNoFuncionalProp';
        
        if (pControlCont.indexOf('contEntSelec_') != -1)//Es entidad seleccionada
        {
            funcionSel = 'operativaGestionObjetosConocimientoOntologias.SeleccionarObjectNoFuncionalSeleccEnt';
            funcionEli = 'operativaGestionObjetosConocimientoOntologias.EliminarObjectNoFuncionalSeleccEnt';
    
            if (document.getElementById(pControlCont.replace('contEntSelec_', 'hack_')) != null || document.getElementById(pControlCont.replace('contEntSelec_', 'bkHtmlSelEnt_')) != null) {
                funcionSel = '';
            }
        }

        if (funcionSel != '') {
            // Establecer la función de seleccionar
            metodoSeleccionar = `${funcionSel}('${pEntidad}', '${pPropiedad}', '${pNumElem}', '${pTxtValores.prop("id")}', '${pTxtIDs.prop("id")}', '${pTxtCaract.prop("id")}', '${pTxtElemEditados.prop("id")}')`;            
        }

        /* Obtener la función Eliminar */
        const metodoEliminar = `${funcionEli}('${pNumElem}', '${pEntidad}', '${pPropiedad}', '${pControlCont}', '${pTxtValores.prop("id")}', '${pTxtIDs.prop("id")}', '${pTxtCaract.prop("id")}', '${pTxtElemEditados.prop("id")}')`;
        
        // Encabezado de la fila
        fila += `
        <li class="component-wrap containerConfirmDeleteItemInModal">
            <div class="component">
                <div class="component-header-wrap">
                    <div class="component-header">
                        <div class="component-header-content">
                            <div class="component-header-left">
                                <div class="component-name-wrap">
                                    <span class="component-item-name">
                                        ${pValor}
                                    </span>
                                </div>
                            </div>
  
                            <div class="component-header-right">
                                <div class="component-actions-wrap">
                                    <ul class="no-list-style component-actions">
                                    `;
                                        if (funcionEli != '') {
                                            fila +=`
                                            <li>
                                                <a class="action-edit round-icon-button js-action-edit-component btnEditGroupValue" href="javascript: void(0);" onclick="${metodoSeleccionar}">
                                                    <span class="material-icons">edit</span>
                                                </a>
                                            </li>`;                  
                                        }  
                                        if (metodoSeleccionar !=""){
                                            fila +=`
                                            <li>                                            
                                                <a class="action-delete round-icon-button js-action-delete btnDeleteGroupValue" href="javascript: void(0);" data-handleclick="${metodoEliminar}">
                                                    <span class="material-icons">delete</span>
                                                </a>
                                            </li>
                                            `;
                                        }
                                    fila +=`                                                                            
                                    </ul>
                                </div>
                            </div>                                                              
                        </div>
                    </div>
                </div>
            </div>
	    </li>
        `;

        return fila;
    },

    ReemplarIDsCatTesSemPorNombre: function(pValor){
        const that = this;
        let nombresCat = document.getElementById(that.txtNombreCatTesSem.prop("id")).value;
        
        if (pValor.length > 0 && pValor.indexOf('|') == (pValor.length - 1)) {
            pValor = pValor.substring(0, pValor.length - 1);
        }

        var valorArray = pValor.split(',');
        pValor = '';

        for (var i = 0; i < valorArray.length; i++) {
            if (valorArray[i] != '') {
                if (nombresCat.indexOf(valorArray[i] + '|') != -1) {
                    nombresCat = nombresCat.substring(nombresCat.indexOf(valorArray[i] + '|') + (valorArray[i] + '|').length);
                    pValor += nombresCat.substring(0, nombresCat.indexOf('|||')) + ' > ';
                
                    nombresCat = document.getElementById(that.txtNombreCatTesSem.prop("id")).value;
                }
                else {
                    pValor += valorArray[i] + ' > ';
                }
            }
        }

        if (pValor != '') {
            pValor = pValor.substring(0, pValor.length - 3);
        }
        return pValor;    
    },
    
    EsPropiedadMultiIdiomaSinPesanya: function(pEntidad, pPropiedad){
        const that = this;
        
        if (that.EsPropiedadMultiIdioma(pEntidad, pPropiedad)) {            
            const idControlCampo = that.ObtenerControlEntidadProp(pEntidad + ',' + pPropiedad, TxtRegistroIDs);
        
            return (document.getElementById(idControlCampo.replace('Campo_', 'divContPesIdioma_')) == null);
        }
        return false;        
    },
    
    ComprobarMultiIdiomaCorrectoTexto: function(pEntidad, pPropiedad, pTexto){
        const that = this;
        const tipoProp = that.GetTipoPropiedad(pEntidad, pPropiedad, TxtCaracteristicasElem);
        
        if (pTexto != null && pTexto != '' && pTexto.indexOf('@' + IdiomaDefectoFormSem + '[|lang|]') == -1 && (tipoProp == 'FD' || that.CardinalidadElementoMayorOIgualUno(pEntidad, pPropiedad, TxtCaracteristicasElem))) {
            that.MostrarErrorPropiedad(pEntidad, pPropiedad, textoFormSem.propSinIdiomaDefecto, TxtRegistroIDs);
            return false;
        }
        return true;    
    },

    /**
     * Método para configurar el autocomplete para inputs a añadir en una entidad secundaria para añadir items que precisan del servicio autocomplete
     * @param {*} control 
     * @param {*} grafo 
     * @param {*} entContenedora 
     * @param {*} propiedad 
     * @param {*} tipoEntidadSolicitada 
     * @param {*} propSolicitadas 
     * @param {*} extraWhere 
     * @param {*} idioma 
     */
    autocompletarSeleccionEntidad: function(control, grafo, entContenedora, propiedad, tipoEntidadSolicitada, propSolicitadas, extraWhere, idioma){
        let limite = 10;        

        if (extraWhere.indexOf('%7c%7c%7cLimite%3d') != -1) {
            limite = 200;
        }
    
        $(control).autocomplete(
        null,
        {
            url: $('#inpt_urlServicioAutocompletar').val() + "/AutoCompletarSeleccEntDocSem",
            delay: 0,
            scroll: false,
            selectFirst: false,
            minChars: 1,
            /*width: 170,*/
            cacheLength: 0,
            matchCase: true,
            pintarConcatenadores: true,
            max: limite,
            extraParams: {
                pGrafo: grafo,
                pEntContenedora: entContenedora,
                pPropiedad: propiedad,
                pTipoEntidadSolicitada: tipoEntidadSolicitada,
                pPropSolicitadas: propSolicitadas,
                pControlID: urlEncode(control.id),
                pExtraWhere: extraWhere,
                pIdioma: idioma
            }
        }
        );
        if (control.attributes["onfocus"] != null) {
            control.attributes.removeNamedItem('onfocus');
        }   
        
        /**
         * Comportamiento resultado cuando se selecciona una resultado de autocomplete de Perfiles
         Este comportamiento se añade directamente en jqueryAutoComplete asociado al método helpers "AgregarEntidadSeleccAutocompletar"
        $(control).result(function (event, data, formatted) {
            
        });        
        */

    },

    
    /**
     * Método que se dispara al hacer "autocomplete" de proopiedades externas. Este método es llamado desde la propia librería jqueryAutocomplete <-> helpers.AgregarEntidadSeleccAutocompletar
     * @param {objet} pData Datos que se han seleccionado del servicio autocomplete
     */    
    AgregarEntidadSeleccAutocompletar: function(pData){
        const that = this;
        
        var idHack = decodeURIComponent(pData[3]);
        var idControl = idHack.replace("hack_", "selEnt_");
    
        if (idControl.indexOf('extra_') != -1) {
            var idAux = idControl.substring(0, idControl.indexOf('extra_'));
            idControl = idControl.substring(idControl.indexOf('selEnt_'));
            idControl = idAux + idControl;
    
            document.getElementById(idControl.replace("selEnt_", "hack_")).value = '';
        }
    
        var count = 0;
        var trozoID = 'extra_' + count + '_hack_';
    
        while (document.getElementById(idControl.replace("selEnt_", trozoID)) != null) {
    
            if (document.getElementById(idControl.replace("selEnt_", trozoID)).id != idHack) {
                document.getElementById(idControl.replace("selEnt_", trozoID)).value = '';
            }
    
            count++;
            trozoID = 'extra_' + count + '_hack_';
        }
    

        //var entProp = ObtenerEntidadPropiedadSegunID(idControl, TxtRegistroIDs)
        var entProp = that.ObtenerEntidadPropiedadSegunID(idControl, that.txtRegistroIDs);
        var propiedad = entProp[1];
        var entidad = entProp[0];
    
        document.getElementById(idControl).value = pData[2];
        //var tipoProp = GetTipoPropiedad(entidad, propiedad, TxtCaracteristicasElem);
        var tipoProp = that.GetTipoPropiedad(entidad, propiedad, that.txtCaracteristicasElem);
    
        if (tipoProp == 'LSEO') {
            var idBotonAceptar = idControl.replace("selEnt_", "lbCrear_");
            eval(document.getElementById(idBotonAceptar).attributes["onclick"].value);
        }
        else if (tipoProp == 'FO') {
            
        }
    
        if (tipoProp != 'LSEO') {
            var inputHack = $('#' + idHack);
            var inputSelEnt = $('#' + idControl);
            inputHack.prop("disabled", true);
            var contenedor = $('#' + idControl).closest('div.cont');
            contenedor.append('<a class="remove removeAutocompletar">Eliminar</a>');
            $('a.remove', contenedor).click(function () {
                inputHack.val('');
                inputSelEnt.val('');
                inputHack.prop("disabled", false);
                $(this).remove();
            });
        }
    
        that.AgregarNombresCatTesSem(pData[2] + '|' + pData[0]);
    },

    AgregarNombresCatTesSem: function(pNombres){
        const that = this;
                
        var nomArray = pNombres.split('|||');

        //var catsNombres = document.getElementById(TxtNombreCatTesSem).value;
        var catsNombres = that.txtNombreCatTesSem.val();

        for (var i = 0; i < nomArray.length; i++) {
            if (nomArray[i] != '') {
                var sujeto = nomArray[i].substring(0, nomArray[i].indexOf('|'));
    
                if (catsNombres.indexOf(sujeto + '|') == -1) {
                    catsNombres += nomArray[i] + '|||';
                }
            }
        }
        
        // document.getElementById(TxtNombreCatTesSem).value = catsNombres;
        that.txtNombreCatTesSem.val(catsNombres);
    },

    ObtenerEntidadPropiedadSegunID: function(idCampoControl, pTxtIDs){
        
        var ids = pTxtIDs.val(); // $('#' + pTxtIDs).val();
    
        if (ids.indexOf(idCampoControl) != -1)
        {
            ids = ids.substring(0, ids.indexOf(idCampoControl) - 1);
            ids = ids.substring(ids.lastIndexOf('|') + 1);
            return ids.split(',');
        }
        
        return null;
    },
}

/**
  * Operativa para la gestión/configuración de Sparql
  */
const operativaGestionSparql = {

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

    /**
     * Lanzar comportamientos u operativas necesarias para el funcionamiento de la sección
     */
    triggerEvents: function(){
        const that = this;                       
        // Inicializar comportamientos de select2
        comportamientoInicial.iniciarSelects2(); 
    },   

    /**
     * Configuración de las rutas de acciones necesarias para hacer peticiones a backend
     */
    configRutas: function (pParams) {
        // Url para editar un certificado
        this.urlBase = refineURL();
        this.urlSave = `${this.urlBase}/save`;

    },  

    /*
    * Inicializar elementos de la vista
    * */
    config: function (pParams) {        
        // Contenedor modal
        this.modalContainer = $("#modal-container");
        // Modal para selección de grafo
        this.modalSelectGraph = $("#modal-select-graph");
        // Modal para mostrado de consulta Sparql realizada
        this.modalResultsGraphQuery = $("#modal-results-graph-query");

        // Input para buscar grafos en el modal
        this.txtBuscarGrafo = $("#txtBuscarGrafo");

        // Mensaje de grafos no seleccionados ("Ningún grafo añadido")
        this.infoGraphSelected = $("#infoGraphSelected");         
        // Botón para seleccionar los grafos seleccionados desde el modal para consulta Sparql
        this.btnSelectGraph = $("#btnSelectGraph");

        // Select, Where
        this.txtSelect = $("#txtSelect");
        this.txtWhere = $("#txtWhere");
        // Buscador de resultdos de consulta Sparql
        this.txtBusquedaGraphQuery = $("#txtBusquedaGraphQuery");
        // Textarea donde aparecerán los resultados de la consulta sparql
        this.graphResultsArea = $("#graphResultsArea");
        this.graphResultsNumber = $("#graphResultsNumber");
        // Botón para ver más prefijos de sparql
        this.btnShowMorePrefix = $("#btnShowMorePrefix");
        // Clase que indica que la fila del prefijo es adicional (Ya se han pintdo más de tres, por lo que es adicional)
        this.additionalPrefixClassName = "additional-prefix";

        // Botón para ejecutar una consulta Sparql
        this.btnExecuteSparql = $("#btnExecuteSparql");
        // Tabla donde se muestran los grafos seleccionados para la consulta sparql
        this.tablaGrafos = $("#tabla-grafos");
        // Body de la tabla de grafos seleccionados para consultas sparql
        this.tablaGrafosContent = $("#tabla-grafos-content");
        // Clase de cada fila del grafo para la consulta
        this.graphRowClassName = "graph-row";
        // Cada checkbox del modal para la selección de un grafo para la consulta sparql
        this.ckGraphItemClassName = "ckGraphItem";
        // Nombre de la clase del botón para eliminar un grafo de la selección
        this.btnDeleteGraphClassName = 'btnDeleteGraph';
        // Botón para copiar el grafo seleccionado
        this.btnCopyClassName = "btnCopy";
        // Url anterior del navegador almacenado a modo "backup" para habilitar/deshabilitar el botón
        this.historyUrlBrowser = document.referrer;        
    },   

    /**
     * Configuración de eventos de elementos del Dom (Botones, Inputs...)     
     */
    configEvents: function (pParams) {
        const that = this;

        // Comportamientos del modal container 
        this.modalContainer.on('show.bs.modal', (e) => {
            // Aparición del modal
            // Establecer quién ha disparado el modal para poder cambiar sus atributos una vez se guarden los grupos desde el modal
            that.triggerModalContainer = $(e.relatedTarget);                              
        })
        .on('hide.bs.modal', (e) => {
            // Acción de ocultar el modal
        })
        .on('hidden.bs.modal', (e) => {
            // Vaciar el modal
            resetearModalContainer(); 
        });

        // Comportamientos del modal de selección de grafos 
        this.modalSelectGraph.on('show.bs.modal', (e) => {
            // Aparición del modal
            // Establecer quién ha disparado el modal para poder cambiar sus atributos una vez se guarden los grupos desde el modal
            that.triggerModalContainer = $(e.relatedTarget);                              
        })
        .on('hide.bs.modal', (e) => {
            // Acción de ocultar el modal
        })
        .on('hidden.bs.modal', (e) => {
            // Vaciar el campo de resultados de la consulta sparql
            // Mismo comportamiento que al pulsar en "Guardar"
            that.handleSelectGraphs();
        });         


        // Comportamientos del modal de resultados 
        this.modalResultsGraphQuery.on('show.bs.modal', (e) => {
            // Aparición del modal
            // Establecer quién ha disparado el modal para poder cambiar sus atributos una vez se guarden los grupos desde el modal
            that.triggerModalContainer = $(e.relatedTarget);                              
            that.handleEnableBrowserBackButton(false);
        })
        .on('hide.bs.modal', (e) => {
            // Acción de ocultar el modal
        })
        .on('hidden.bs.modal', (e) => {
            // Vaciar el campo de resultados de la consulta sparql
            this.graphResultsArea.html();
            that.handleEnableBrowserBackButton(true);
        });  
        
        // Botón para seleccionar grafos para la consulta Sparql
        this.btnSelectGraph.on("click", function(){
            that.handleSelectGraphs();
        });

        // Botón para confirmar la eliminación de un objeto de conocimiento desde el modal
        configEventByClassName(`${that.btnDeleteGraphClassName}`, function(element){
            const btnDeleteGraph = $(element);
            btnDeleteGraph.off().on("click", function(){                   
                that.handleDeleteGraphFromSparql(btnDeleteGraph);
            });	                        
        });

        // Botón para copiar información del grafo
        configEventByClassName(`${that.btnCopyClassName}`, function(element){
            const btnCopy = $(element);
            btnCopy.off().on("click", function(){                   
                const itemToCopyValue = $(this).closest(`.${that.graphRowClassName}`).data("fuente");
                that.handleCopyToClickBoard(itemToCopyValue);
            });	                        
        }); 
        
        // Botón para ejecutar consulta Sparql                
        this.btnExecuteSparql.on("click", function(){
            that.handleExecuteSparql();
        });

        // Buscar resultados SparQL en la tabla
        this.txtBusquedaGraphQuery.off().on("keyup", function (event) {
            const input = $(this);
            clearTimeout(that.timer);
            that.timer = setTimeout(function () {                                                                        
                // Acción de buscar / filtrar
                that.handleSearchSparqlResults(input);                                         
            }, 500);
        }); 
        
        // Botón para mostrar u ocultar más prefijos de Sparql
        this.btnShowMorePrefix.off().on("click", function(){
            const button = $(this);
            if (button.data("show-all") == true){
                // Ocultar                 
                $(`.${that.additionalPrefixClassName}`).addClass("d-none");
                button.data("show-all", false);
                button.html("Mostrár");
            }else{
                // Mostrar todas
                $(`.${that.additionalPrefixClassName}`).removeClass("d-none");
                button.data("show-all", true);
                button.html("Ocultar");
            }
        });

        // Input para buscar el grafo
        this.txtBuscarGrafo.off().on("keyup", function (event) {
            clearTimeout(that.timer);
            that.timer = setTimeout(function () {                
                that.handleBuscarGrafo();                                                         
            }, 500);
        });
    },  

    /**
     * Método para permitir o no que el botón de "Volver" del navegador funcione o no para no perder las consultas SparQL.
     * @param {Bool} enableBrowserBackButton Indica si se desea habilitar o no el botón del navegador "Volver"
     */
    handleEnableBrowserBackButton: function(enableBrowserBackButton){
        const that = this;
        if (enableBrowserBackButton){
            // Habilitar el "BackButton"   
          // Bloquer el "BackButton"            
          window.history.pushState(null, "", that.historyUrlBrowser);        
          window.onpopstate = function() {
            window.history.back();
          };                 
        }else{
            // Bloquer el "BackButton"            
            window.history.pushState(null, "", window.location.href);        
            window.onpopstate = function() {
                window.history.pushState(null, "", window.location.href);
            };
        }
    },
    
    /**
     * Método para realizar búsquedas ocultado o mostrando los resultados que se encuentren en la tabla de resultados de sparql
     * @param {*} input Input que contiene el texto a buscar
     */
    handleSearchSparqlResults: function(input){
        const that = this;
        // Cadena introducida para filtrar/buscar
        let cadena = input.val();
        // Eliminamos posibles tildes para búsqueda ampliada
        cadena = cadena.normalize('NFD').replace(/[\u0300-\u036f]/g, "").toLowerCase();

        // Listado de resultados donde se buscará (Evitar la fila Header)
        const sparqlResults = that.graphResultsArea.find("tr:gt(0)");
                                    
        // Recorrer los items para realizar la búsqueda de palabras clave
        $.each(sparqlResults, function(index){
            const tableRow = $(this);
            // Seleccionar el nombre sólo "visible" de la traducción. Quitamos caracteres extraños, tildes para poder hacer bien la búsqueda
            const subject = $($(tableRow.find("td")[0])).html().trim().normalize("NFD").replace(/[\u0300-\u036f]/g, "").toLowerCase();;                        
            const predicate = $($(tableRow.find("td")[1])).html().trim().normalize("NFD").replace(/[\u0300-\u036f]/g, "").toLowerCase();
            const object = $($(tableRow.find("td")[2])).html().trim().normalize("NFD").replace(/[\u0300-\u036f]/g, "").toLowerCase();

            if (subject.includes(cadena) || predicate.includes(cadena) || object.includes(cadena)){
                // Mostrar la fila
                tableRow.removeClass("d-none");                
            }else{
                // Ocultar la fila
                tableRow.addClass("d-none");
            }            
        });

        // Actualizar el nº de resultados
        that.updateSparkResultsNumber();
    },

    /**
     * Método que mostrará el número de resultados de SparQL
     */
    updateSparkResultsNumber: function(){
        const that = this;
        let numberOfResults = that.graphResultsArea.find("tr:gt(0)").not('.d-none').length;
        let resultWord = "resultado"
        if (numberOfResults>1){
            resultWord += "s";
        }else if(numberOfResults == 0){
            numberOfResults = "";
            resultWord = "No hay resultados";
        }
        // Mostrar el nº de resultados
        that.graphResultsNumber.html(`${numberOfResults} ${resultWord}`);
    },

    /**
     * Método para eliminar un grafo para la consultar de sparql. Tambien lo quitará del checkbox del modal.
     * @param {*} btnDeleteGraph : Botón pulsado de eliminación del modal
     */
    handleDeleteGraphFromSparql: function(btnDeleteGraph){
        const that = this;
        // Fila a eliminar
        const tableRowGraph = btnDeleteGraph.closest(`.${that.graphRowClassName}`);
        const prefixGraph = tableRowGraph.data("prefix");

        // Eliminar la fila del grafo de la tabla
        tableRowGraph.remove();
        // Deseleccionar el checkbox del grafo a eliminar
        const ckBoxGraphSelected = $(`[data-prefix='${prefixGraph}']`);
        ckBoxGraphSelected.prop( "checked", false );
    },
    
    /**
     * Método para seleccionar grafos para búsqueda Sparql
     */
    handleSelectGraphs: function(){
        const that = this;

        // Seleccionar los grafos seleccionados
        const graphsCheckBoxListSelected = $(`.${this.ckGraphItemClassName}:checked`);        
        // Borrar los items de this.tablaGrafosContent
        this.tablaGrafosContent.html('');
        // Comprobar si no hay ninguna selección y mostrar/ocultar el mensaje de "Ningún grafo añadido"
        graphsCheckBoxListSelected.length == 0 ? this.infoGraphSelected.removeClass("d-none") : this.infoGraphSelected.addClass("d-none");

        // Crear con la plantilla necesaria los rows para la tabla 
        let templateGraphSelected = '';
        graphsCheckBoxListSelected.each(function () {            
            const prefix = $(this).data("prefix");
            const fuente = $(this).data("value");
            templateGraphSelected += `
            <tr class="${that.graphRowClassName}" data-prefix="${prefix}" data-fuente="${fuente}">
                <td class="td-prefix">${prefix}</td>
                <td class="td-fuente">
                    <a href="javascript: void(0);">
                        ${fuente}
                        <span class="material-icons btnCopy">content_copy</span>
                    </a>
                </td>
                <td class="td-acciones">
                    <div class="dropdown dropdown-select">
                        <a class="dropdown-toggle" data-toggle="dropdown">
                            <span class="material-icons">delete</span>
                            <span class="texto">Eliminar</span>
                        </a>
                        <div class="dropdown-menu basic-dropdown dropdown-icons dropdown-menu-right">
                            <a class="item-dropdown ${that.btnDeleteGraphClassName}">
                                <span class="material-icons">delete</span>
                                <span class="texto">Eliminar</span>
                            </a>
                        </div>
                    </div>
                </td>
            </tr> 
            `;
        });

        // Añadir el template a la tabla de grafos si es diferente a 0
        graphsCheckBoxListSelected.length > 0 && this.tablaGrafosContent.append(templateGraphSelected);
    },

    /**
     * Método para copiar al portapapeles el enlace del item pulsado.     
     * @param {String} urlString : Url o cadena que se mostrará al usuario y que se copiará al portapapeles
     */
    handleCopyToClickBoard(urlString){           
        copyTextToClipBoard(urlString);
    },

    /**
    * Método para realizar la búsqueda de un grafo
    */
    handleBuscarGrafo: function(){
        const that = this;
        let cadena = this.txtBuscarGrafo.val();
        // Eliminamos posibles tildes para búsqueda ampliada
        cadena = cadena.normalize('NFD').replace(/[\u0300-\u036f]/g, "").toLowerCase();
    
        // Cada una de las filas que muestran los grafos
        const filasGrafos = $(".graphListOntology");        

        // Buscar dentro de cada fila
        $.each(filasGrafos, function(index){
            const filaGrafo = $(this);
            // Seleccionamos el nombre del grafo y quitamos caracteres extraños, tiles para poder hacer bien la búsqueda
            const nombreGrafo = $(this).find(".td-valor").not('.d-none').html().trim().normalize("NFD").replace(/[\u0300-\u036f]/g, "").toLowerCase();
            if (nombreGrafo.includes(cadena)){
                // Mostrar fila resultado y sus respectivos padres
                filaGrafo.removeClass("d-none");
            }else{
                // Ocultar fila resultado
                filaGrafo.addClass("d-none");
            }            
        });
    },

    /**
     * Método para ejecutar una consultar Sparql
     */
    handleExecuteSparql: function(){
        const that = this;

        that.Options = {};
        // Información de valores FROM
        let valoresFrom = '';
    
        // Recogida de datos FROM
        Array.from($(`.${this.graphRowClassName}`)).forEach(element => {
            valoresFrom += $(element).data("fuente") + ",";
        });        


        // Quitar el último separador
        if (valoresFrom != ""){
            valoresFrom = (valoresFrom.substring(0, valoresFrom.length - 1));
        }else{
            mostrarNotificacion("error", "Elige un valor para el campo FROM");
            return;
        }

        // Comprobación del valor SELECT
        const valorSelect = this.txtSelect.val().trim();
        if (valorSelect.length == 0) {            
            mostrarNotificacion("error", "El campo SELECT no puede estar vacío");
            return;
        }

        // Construir objeto para petición Sparql
        that.Options['pSelect'] = valorSelect;
        that.Options['pFrom'] = valoresFrom;
        that.Options['pWhere'] = this.txtWhere.val().trim();

        // Mostrar loading
        loadingMostrar();

        GnossPeticionAjax(
        that.urlSave,
        that.Options,
        true
        ).done(function (data) {
            // OK - Mostrar la información obtenida de la consulta                                
            that.loadSparqlResults(data);
        }).fail(function (data) {
            // KO
            if (data) {
                mostrarNotificacion("error", data);
            } else {
                mostrarNotificacion("error", "Error al ejecutar la consulta");                
            }
        }).always(function () {
            // Ocultar loading
            loadingOcultar();
        });
    },

    /**
     * Método para mostrar los resultados obtenidos de la consulta Sparql 
     * @param {*} data Datos obtenidos de la consulta sparql
     */
    loadSparqlResults: function(data){        
        // Cargar datos en la sección del modal
        this.graphResultsArea.html(data);
        // Mostrar el modal        
        this.modalResultsGraphQuery.modal('show');
        // Resetear y estilizar el contenido 
        const containerFluid = this.graphResultsArea.find(".container-fluid");
        containerFluid.removeClass("container-fluid");  
        // Mostrar el nº de resultados de sparql
        this.updateSparkResultsNumber();
    },

}

/**
  * Operativa para la gestión de las cargas masivas
  */
const operativaGestionCargasMasivas = {

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

    /**
     * Lanzar comportamientos u operativas necesarias para el funcionamiento de la sección
     */
    triggerEvents: function(){
        const that = this;                       
        // Inicializar comportamientos de select2
        comportamientoInicial.iniciarSelects2(); 

        // Contador del número de cargas masivas existentes
        that.handleCheckNumberOfMassiveLoads();
    },   

    /**
     * Configuración de las rutas de acciones necesarias para hacer peticiones a backend
     */
    configRutas: function (pParams) {
        // Url para editar un certificado
        this.urlBase = refineURL();        

    },  

    /*
    * Inicializar elementos de la vista
    * */
    config: function (pParams) {        
        // Contenedor modal
        this.modalContainer = $("#modal-container");

        // Buscador de cargas masivas
        this.txtBuscarCargaMasiva = $("#txtBuscarCargaMasiva");
        // Nombre de la clase de cada fila de carga masiva
        this.filaCargaMasivaClassName = "masiveLoad-row";
        // Contenedor de las filas de cargas masivas
        this.cargasMasivasListContainer = $("#id-added-cargas-list");
        // Marcador de nº de cargas masivas
        this.numResultadosMassiveLoads = $("#numCargasMasivas");
    },   

    /**
     * Configuración de eventos de elementos del Dom (Botones, Inputs...)     
     */
    configEvents: function (pParams) {
        const that = this;

        // Comportamientos del modal container 
        this.modalContainer.on('show.bs.modal', (e) => {
            // Aparición del modal
            // Establecer quién ha disparado el modal para poder cambiar sus atributos una vez se guarden los grupos desde el modal
            that.triggerModalContainer = $(e.relatedTarget);                              
        })
        .on('hide.bs.modal', (e) => {
            // Acción de ocultar el modal
        })
        .on('hidden.bs.modal', (e) => {
            // Vaciar el modal
            resetearModalContainer(); 
        });

        // Input para realizar búsquedas
        this.txtBuscarCargaMasiva.off().on("keyup", function (event) {
            clearTimeout(that.timer);
            that.timer = setTimeout(function () {                
                that.handleSearchMassiveLoad();                                                         
            }, 500);
        });        
        
    },  
    

    /**
    * Método que se ejecutará al cargarse la web para saber el nº de cargas masivas existentes
    */
     handleCheckNumberOfMassiveLoads: function(){        
        const that = this;        
        const numberMassiveLoads = that.cargasMasivasListContainer.find($(`.${that.filaCargaMasivaClassName}`)).length;
        // Mostrar el nº de items                
        that.numResultadosMassiveLoads.html(numberMassiveLoads);
    },  
    
    /**
     * Método que se ejecutará para buscar cargas masivas
     */
    handleSearchMassiveLoad: function(){

        const that = this;
        let cadena = this.txtBuscarCargaMasiva.val();
        // Eliminamos posibles tildes para búsqueda ampliada
        cadena = cadena.normalize('NFD').replace(/[\u0300-\u036f]/g, "").toLowerCase();
        
        // Cada una de las filas que muestran la página
        const rowCargasMasivas = that.cargasMasivasListContainer.find(`.${that.filaCargaMasivaClassName}`);        

        // Buscar dentro de cada fila       
        $.each(rowCargasMasivas, function(index){
            const rowCarga = $(this);
            // Seleccionamos el nombre de la página y quitamos caracteres extraños, tiles para poder hacer bien la búsqueda
            const cargaName = $(this).find(".component-name").not('.d-none').html().trim().normalize("NFD").replace(/[\u0300-\u036f]/g, "").toLowerCase();
            const cargaId = $(this).find(".component-id").not('.d-none').html().trim().normalize("NFD").replace(/[\u0300-\u036f]/g, "").toLowerCase();
            const cargaDetails = $(this).find(".component-detalles").not('.d-none').html().trim().normalize("NFD").replace(/[\u0300-\u036f]/g, "").toLowerCase();
            
            if (cargaName.includes(cadena) || cargaId.includes(cadena) || cargaDetails.includes(cadena)){
                // Mostrar fila resultado
                rowCarga.removeClass("d-none");                
            }else{
                // Ocultar fila resultado
                rowCarga.addClass("d-none");
            }            
        });

    },

}

/**
  * Operativa para la gestión del borrado masivo
  */
const operativaGestionBorradoMasivo = {

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

    /**
     * Lanzar comportamientos u operativas necesarias para el funcionamiento de la sección
     */
    triggerEvents: function(){
        const that = this;                       

    },   

    /**
     * Configuración de las rutas de acciones necesarias para hacer peticiones a backend
     */
    configRutas: function (pParams) {
        // Url para editar un certificado
        this.urlBase = refineURL(); 
        this.urlDeleteOntologyResources = `${this.urlBase}/Borrar`;
        
        // Objeto que contendrá las ontologías a eliminar
        this.OntologiesToDelete = [];
        // Objeto a enviar a backend para guardado
        this.Options = {};
    },  

    /*
    * Inicializar elementos de la vista
    * */
    config: function (pParams) {        
        // Contenedor modal
        this.modalContainer = $("#modal-container");
        this.modalMassiveOntologyDelete = $("#modalMassiveOntologyDelete");

        // Botón para el borrado masivo de cada ontología
        this.btnDeleteOntologyMassive = $(".btnDeleteMassiveOntology");
        // Botón para borrar todas las ontologías seleccionadas
        this.btnDeleteAllSelected = $("#btnDeleteAllSelected");

        // Cada checkbox de selección del borrado de cada ontología        
        this.ckDeleteOntologyMassiveClassName = "ckboxDeleteOntologyMassive";
        this.ckDeleteOntologyMassive = $(`.${this.ckDeleteOntologyMassiveClassName}`);

        // Cada fila de ontología de datos cargados
        this.filaOntologyMassiveLoad = $(".row-ontologyMassiveItem");

        // Botón para confirmar el borrado de recursos asociados a una ontología (desde modal)
        this.btnDeleteConfirm = $("#btnDeleteConfirm");
    },   

    /**
     * Configuración de eventos de elementos del Dom (Botones, Inputs...)     
     */
    configEvents: function (pParams) {
        const that = this;

        // Comportamientos del modal container 
        this.modalContainer.on('show.bs.modal', (e) => {
            // Aparición del modal
            // Establecer quién ha disparado el modal para poder cambiar sus atributos una vez se guarden los grupos desde el modal
            that.triggerModalContainer = $(e.relatedTarget);                              
        })
        .on('hide.bs.modal', (e) => {
            // Acción de ocultar el modal
        })
        .on('hidden.bs.modal', (e) => {
            // Vaciar el modal
            resetearModalContainer(); 
        });

        // Comportamientos del modal container 
        this.modalMassiveOntologyDelete.on('show.bs.modal', (e) => {
            // Aparición del modal
            // Establecer quién ha disparado el modal para poder cambiar sus atributos una vez se guarden los grupos desde el modal
            that.triggerModalContainer = $(e.relatedTarget);                              
        })
        .on('hide.bs.modal', (e) => {
            // Acción de ocultar el modal
        })
        .on('hidden.bs.modal', (e) => {
            // Vaciar el modal
            // Desactivar todos los checkbox activados 
            that.ckDeleteOntologyMassive.each(function () {                
                $(this).prop( "checked", false );
            });
        });        

        // Botón para el borrado de todas los items seleccionados
        this.btnDeleteAllSelected.on("click", function(){
            that.handleGetOntologiesSelected();
        });

        // Botón para el borrado de una ontología en concreto
        this.btnDeleteOntologyMassive.on("click", function(){
            const deleteButton = $(this);
            that.handleSelectOntology(deleteButton);
        });

        // Botón del modal para confirmar el borrado de rcursos asociados a las ontologías seleccionadas
        this.btnDeleteConfirm.on("click", function(){
            that.handleDeleteOntologies();
        });
    }, 
    

    /**
     * Método para seleccionar una determinada ontología y abrir el modal para confirmar su borrado
     */
    handleSelectOntology: function(deleteButton){
        const that = this;

        // Desactivar todas aquellas que se hayan seleccionado previamente. Solo se desea del botón seleccionado
        that.ckDeleteOntologyMassive.each(function () {                
            $(this).prop( "checked", false );
        });

        // Inicialización de recursos de ontologías a borrar
        this.OntologiesToDelete = [];

        // Fila de la ontología de la que se desean eliminar recursos
        const rowSelected = deleteButton.closest(that.filaOntologyMassiveLoad);
        // Activar el checkbox de la ontología
        const checkbox = rowSelected.find(that.ckDeleteOntologyMassive)
        checkbox.prop( "checked", true );
        const ontologySelected = checkbox.data("value");
        // Añadir el item a las ontologías
        this.OntologiesToDelete.push(ontologySelected);

        // Mostrar el modal para el borrado
        that.modalMassiveOntologyDelete.modal("show");        
    },

    /**
     * Método para obtener las ontologías que se han seleccionado mediante checkbox
     */
    handleGetOntologiesSelected: function(){
        const that = this;

        // Reseteo de items a seleccionar/borrar
        this.OntologiesToDelete = [];

        // Recogida de items a eliminar
        const ontologiesSelected = $(`.${this.ckDeleteOntologyMassiveClassName}:checked`); 
        
        if (ontologiesSelected.length == 0){
            mostrarNotificacion("warning", "Selecciona antes al menos una ontología para eliminar sus recursos.");
            return;
        }
        // Crear el objeto a eliminar
        ontologiesSelected.each(function () {
            const ontologyId = $(this).data("value");
            that.OntologiesToDelete.push(ontologyId); 
        });

        // Mostrar el modal para el borrado
        that.modalMassiveOntologyDelete.modal("show");
    },

    /**
     * Método para hacer la petición de borrar los recursos de las ontologías seleccionadas
     */
    handleDeleteOntologies: function(){
        const that = this;

        if (this.OntologiesToDelete.length == 0){
            mostrarNotificacion("warning", "Selecciona antes al menos una ontología para eliminar sus recursos.");
            return;
        }
        loadingMostrar();

        // Creación del objeto para enviar a borrar
        that.Options['OntologiaSeleccionada'] = that.OntologiesToDelete;

        GnossPeticionAjax(
            that.urlDeleteOntologyResources,
            that.Options,
            true
        ).fail(function (response) {
            // KO en borrado
            mostrarNotificacion("error", "Se ha producido un error durante el borrado masivo. Por favor, contacta con el adminsitrador.");            
        }).done(function (response) {
            // OK en el borrado
            dismissVistaModal(that.modalMassiveOntologyDelete);
            setTimeout(function () {
                mostrarNotificacion("success", "Los datos se han borrado correctamente");            
            },500)            
        }).always(function (response) {
            // Ocultar loading
            loadingOcultar();            
        });
    },
}


const operativaGestionTesaurosSemanticos = {

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

    /**
     * Lanzar comportamientos u operativas necesarias para el funcionamiento de la sección
     */
    triggerEvents: function(){
        const that = this;           
        // Inicializar comportamientos de select2
        comportamientoInicial.iniciarSelects2();

        // Indicar el nº de items existente
        that.handleCheckNumberOfTesauros();

        // Operativa multiIdiomas
        // Parámetros para la operativa multiIdioma (helpers.js)
        this.operativaMultiIdiomaParams = {
            // Nº máximo de pestañas con idiomas a mostrar. Los demás quedarán ocultos
            numIdiomasVisibles: 3,
            // Establecer 1 tab por cada input (true, false) - False es la forma vieja
            useOnlyOneTab: true,
            panContenidoMultiIdiomaClassName: "panContenidoMultiIdioma",
            // No permitir padding bottom y si padding top
            allowPaddingBottom: false,                 
        };  

        // Inicializar operativa multiIdioma
        operativaMultiIdioma.init(this.operativaMultiIdiomaParams);
    },   

    /**
     * Configuración de las rutas de acciones necesarias para hacer peticiones a backend
     */
    configRutas: function (pParams) {
        // Url para editar un certificado
        this.urlBase = refineURL(); 
        // Url para cargar la vista para añadir un tesauro
        this.urlLoadCreateTesauro = `${this.urlBase}/load-add-thesaurus`;
        this.urlCreateTesauro = `${this.urlBase}/add-thesaurus`;
        // Url para obtener información de un determinado tesauro
        this.urlEditTesauro = `${this.urlBase}/editthesaurus`;
        this.urlDeleteTesauro = `${this.urlBase}/delete-thesaurus`;

        // Flag para controlar borrado de tesauros
        this.confirmDeleteTesauro = false;
        
    },  

    /*
    * Inicializar elementos de la vista
    * */
    config: function (pParams) {        
        // Contenedor modal
        this.modalContainer = $("#modal-container");
        this.modalNewTesauro = $("#modal-new-tesauro");
        this.modalDeleteTesauro = $("#modal-delete-tesauro");

        // Tab de idioma de la página para cambiar la visualización en el idioma deseado de los
        this.tabIdiomaItem = $(".tabIdiomaItem");
        // Cada una de las labels (Nombre y Url) en los diferentes idiomas (por `data-languageitem`) que contiene el valor en el idioma correspondiente
        this.labelLanguageComponentClassName = "language-component";

        // Buscador de tesauros
        this.txtBuscarTesauro = $("#txtBuscarTesauro");
        // Contenedor de objetos de conocimiento
        this.tesaurosListContainerId = 'id-added-tesauros-list';
        this.tesaurosListContainer = $(`#${this.tesaurosListContainerId}`);
        // Nombre de la fila de cada tesauro
        this.tesauroListItemClassName = "tesauro-row";
        // Botón para añadir un nuevo Tesauro
        this.btnNewTesauro = $("#btnNewTesauro");
        // Botón de editar tesauro
        this.btnEditTesauroClassName = "btnEditTesauro";                
        // Botón para eliminar un objeto de conocimiento
        this.btnDeleteTesauroClassName = "btnDeleteTesauro";
        // Botón para guardar un tesauro nuevo
        this.btnSaveTesauro = $("#btnSaveTesauro");
        // Botón para confirmar la eliminación de un objeto de conocimiento
        this.btnConfirmDeleteTesauroClassName = "btnConfirmDeleteTesauro";
        // Botón de no confirmar el borrado de un objeto de conocimiento
        this.btnNotConfirmDeleteTesauroClassName = "btnNotConfirmDeleteTesauro";
        // Contador de número de items existentes
        this.numTesaurosSemanticos = $("#numTesaurosSemanticos");      
        
        /* Inputs del modal de creación de Tesauro */
        // Nombre tesauro
        this.txtNombre = $("#txtNombre");
        this.txtNombreName = 'txtNombre';
        // Prefijo tesauro
        this.txtPrefijo = $("#txtPrefijo");
        // Fuente tesauro
        this.txtFuente = $("#txtFuente");
        // Ontología
        this.selectOntology = $("#selectOntology");  
                
        /* Elementos del Tesauro Edición Tesauro semántico a través del Modal */
        // Botón para añadir un elemento a un tesauro semántico
        this.btnAddTesauroSemanticoElementClassName = "btnAddTesauroSemanticoElement";
        // Panel donde se podrá añadir información para crear nuevos elementos en el Tesauro
        this.panelTesauroElementCreationClassName = "panelTesauroElementCreation";
        // Input del identificador del elemento del tesauro
        this.txtIdentificacionCreacionClassName = "txtIdentificacionCreacion";
        // Input del nombre del elemento del tesauro
        this.txtNombreCatPadreCreacionClassName = "txtNombreCatPadreCreacion";
        // Select de dónde se desea crear el elemento del tesauro
        this.cmbCrearCategoriaEnClassName = "cmbCrearCategoriaEn"; 
        // Botón para crear/guardar un elemento en un Tesauro semántico       
        this.btnCreateTesauroElementClassName = "btnCreateTesauroElement";
        // Panel contenedor de las propiedades extra del elemento tesauro a añadir/editar
        this.panelPropTesSemExtraClassName = "panelPropTesSemExtra";
        // Input para buscar elementos del tesauro semántico
        this.findTesauroSemanticoElementsClassName = "findTesauroSemanticoElements";
        // Botón para eliminar un item de un tesauro semántico
        this.btnDeleteTesauroItemDetailClassName = "btnDeleteTesauroItemDetail";
        // Botón para renombrar un item de un tesauro semántico
        this.btnSaveRenameTesauroCategoryItemClassName = "btnRenameTesauroCategoryItem"
        // Nombre de la fila hijo dentro de cada tesauro
        this.tesauroItemListItemClassName = "tesauro-item-row";
        // Lista donde se mostrarán los items del tesauro semántico
        this.tesauroElementListClassName = "id-added-tesauro-element-list";
        // Botón para plegar o desplegar los hijos items de los tesauros
        this.btnShowHideIconClassName = "showHide-icon";
        // Botón para arrastrar los items de los tesauros
        this.btnDragTesauroIconClassName = "sortable-icon";
        // Nombre del icono para plegar o desplegar
        this.iconShowChildrenMaterialIcon= "add_circle_outline";
        this.iconHideShowChildrenMaterialIcon = "remove_circle_outline";   
        // Clase del contenedor de los hijos del Tesauro
        this.tesauroChildrenPanelClassName = "tesauroChildrenPanel";
        // Número de items dentro del tesauro semántico
        this.numTesaurosSemanticosItemsClassName = "numTesaurosSemanticosItems";
        // Panel de tipo collapse donde se mostrarán los datos del elemento
        this.panelElementDetailClassName = "panelElementDetail";        
        // Id del contenedor donde se mostrarán los items del tesauro seleccionado
        this.tesauroListContainerId = "tesauroListContainer";
        // Variable donde se guardará el ontology-url del item que se desea editar para aplicar las acciones necesarias
        this.ontologyUrl = "";  
    },   

    /**
     * Configuración de eventos de elementos del Dom (Botones, Inputs...)     
     */
    configEvents: function (pParams) {
        const that = this;

        // Comportamientos del modal container 
        this.modalContainer.on('show.bs.modal', (e) => {
            // Aparición del modal
            // Establecer quién ha disparado el modal para poder cambiar sus atributos una vez se guarden los grupos desde el modal
            that.triggerModalContainer = $(e.relatedTarget);             
            that.ontologyUrl = that.triggerModalContainer.data("ontology-url");                           
        })
        .on('hide.bs.modal', (e) => {
            // Acción de ocultar el modal
        })
        .on('hidden.bs.modal', (e) => {
            // Vaciar el modal
            resetearModalContainer(); 
            that.ontologyUrl = "";
        });

        // Comportamientos del modal de creación de un tesauro
        this.modalNewTesauro.on('show.bs.modal', (e) => {
            // Aparición del modal
            // Establecer quién ha disparado el modal para poder cambiar sus atributos una vez se guarden los grupos desde el modal
            that.triggerModalContainer = $(e.relatedTarget);                              
        })
        .on('hide.bs.modal', (e) => {
            // Acción de ocultar el modal
        })
        .on('hidden.bs.modal', (e) => {
            // Vaciar el modal
            that.handleEmptyValuesFromModal();
        });

        // Comportamientos del modal de creación de un tesauro
        this.modalDeleteTesauro.on('show.bs.modal', (e) => {
            // Aparición del modal
            // Establecer quién ha disparado el modal para poder cambiar sus atributos una vez se guarden los grupos desde el modal
            that.triggerModalContainer = $(e.relatedTarget);                              
        })
        .on('hide.bs.modal', (e) => {
            // Acción de ocultar el modal
        })
        .on('hidden.bs.modal', (e) => {
            // Acción de ocultar el modal
            if (that.confirmDeleteTesauro == false){
                that.handleSetDeleteTesauro(false);                
            }   
        });
        
        // Keyup en el nombre de tesauro
        this.txtNombre.on("keyup", function(){
            const input = $(this);
            let isError = comprobarInputNoVacio(input, true, false, "El nombre del tesauro no puede estar vacío.", 0);
            if (isError == true){
                return;
            }           
            // Cambiar nombre en la fila correspondiente
            that.handleChangeNombreValueInTesauroRow();
        });

        // Keyup en el prefijo de tesauro
        this.txtPrefijo.on("keyup", function(){
            const input = $(this);
            comprobarInputNoVacio(input, true, false, "El prefijo del tesauro no puede estar vacío.", 0);         
        });        
        
        // Keyup en la fuente de tesauro
        this.txtFuente.on("keyup", function(){
            const input = $(this);
            let isError = comprobarInputNoVacio(input, true, false, "La fuente del tesauro no puede estar vacío.", 0);
            if (isError == true){
                return;
            }           
            // Cambiar la fuente en la fila correspondiente 
            that.handleChangeFuenteValueInTesauroRow();           
        });    
        
        // Change del tipo de ontología para el tesauro semántico
        this.selectOntology.on("change", function(){
            // Cambiar la fuente en la fila correspondiente 
            that.handleChangeOntologyValueInTesauroRow();           
        });  
    
        // Botón para crear un nuevo Tesauro
        this.btnNewTesauro.off().on("click", function(){
            // Obtener la url a crear para cargar el modal                        
            that.handleLoadCreateTesauro();
        });

        // Botón para editar el tesauro/mostrar información del tesauro        
        configEventByClassName(`${that.btnEditTesauroClassName}`, function(element){
            const $jqueryElement = $(element);
            $jqueryElement.off().on("click", function(){
                that.filaTesauro = $(this).closest(`.${that.tesauroListItemClassName}`);                
                that.handleEditTesauro();
            });	                        
        });

        // Botón para guardar un Tesauro de reciente creación
        this.btnSaveTesauro.on("click", function(){
            that.handleSaveTesauro();
        });

        // Botón para eliminar el tesauro/mostrar
        configEventByClassName(`${that.btnDeleteTesauroClassName}`, function(element){
            const $jqueryElement = $(element);
            $jqueryElement.off().on("click", function(){
                that.filaTesauro = $(this).closest(`.${that.tesauroListItemClassName}`);
                // Marcamos el tesauro para eliminar
                that.handleSetDeleteTesauro(true); 
            });	                        
        });

        // Botón para confirmar la eliminación de un tesauro desde el modal              
        configEventByClassName(`${that.btnConfirmDeleteTesauroClassName}`, function(element){
            const $confirmRemoveTesauro = $(element);
            $confirmRemoveTesauro.off().on("click", function(){   
                // Confirmamos la eliminación
                that.confirmDeleteTesauro = true;
                // Procedemos al borrado/guardado
                that.handleConfirmDeleteTesauro();                            
            });	                        
        }); 

        // Búsquedas de tesauros semánticos
        this.txtBuscarTesauro.off().on("keyup", function (event) {
            const input = $(this);
            clearTimeout(that.timer);
            that.timer = setTimeout(function () {                                                                        
                // Acción de buscar / filtrar
                that.handleSearchTesauroSemantico(input);                                         
            }, 500);
        }); 
        
        // Configurar el botón para expandir o contraer hijos del tesauro        
        configEventByClassName(`${that.btnShowHideIconClassName}`, function(element){
            const collapseButton = $(element);
            collapseButton.off().on("click", function(){   
                // Comprobar si está colapsado                                             
                that.handleShowHideChildrenPanel(collapseButton);
            });	                        
        });
        
        // Configurar la eliminación de un TesauroItem al pulsar en "Eliminar" 
        configEventByClassName(`${that.btnDeleteTesauroItemDetailClassName}`, function(element){
            const deleteButton = $(element);
            if (deleteButton.length > 0) {
                // Fila elemento que está siendo editada. Contenedor de todo
                const pElementRow = deleteButton.closest(`.${that.tesauroItemListItemClassName}`);
                // Pasar la función como parámetro al plugin
                const pluginOptions = {
                    onConfirmDeleteMessage: () => that.handleDeleteTesauroItem(deleteButton)
                }               
                deleteButton.confirmDeleteItemInModal(pluginOptions);
            }                         
        });         
        

        // Renombrar un TesauroItem en "Guardar"        
        configEventByClassName(`${that.btnSaveRenameTesauroCategoryItemClassName}`, function(element){
            const btnRenameSaveTesauroItem = $(element);
            btnRenameSaveTesauroItem.off().on("click", function(){   
                // Renombrar item del tesauro                         
                that.handleRenameTesauroItem(btnRenameSaveTesauroItem);
            });	                        
        }); 
        
        // Botón para crear un elemento en el tesauro semántico. Se mostrará el panel para crear el elemento
        configEventByClassName(`${that.btnAddTesauroSemanticoElementClassName}`, function(element){
            const btnAddTesauroElement = $(element);
            btnAddTesauroElement.off().on("click", function(){   
                // Mostrar panel Crear Elemento Tesauro                   
                that.handleShowHidePanelAddTesauroElement(false);
            });	                        
        });

        configEventByClassName(`${that.btnCreateTesauroElementClassName}`, function(element){
            const btnCreateTesauroElement = $(element);
            btnCreateTesauroElement.off().on("click", function(){   
                // Mostrar panel Crear Elemento Tesauro                   
                that.handleSaveElementTesauroSemantico();
            });	                        
        });

        // Input para realizar búsquedas dentro de los Elementos del tesauro
        configEventByClassName(`${that.findTesauroSemanticoElementsClassName}`, function(element){
            const inputSearchTesauroElement = $(element);
            inputSearchTesauroElement.off().on("keyup", function(){                   
                // Input donde se realizará la búsqueda        
                clearTimeout(that.timer);
                that.timer = setTimeout(function () {                
                    that.handleSearchTesauroElement();                                                         
                }, 500);
    
            });	                        
        });

        // Tab de cada uno de los idiomas disponibles para cambiar la visualización y poder ver el nombre y url del idioma elegido en la página principal
        this.tabIdiomaItem.off().on("click", function(){
            that.handleViewTesauroLanguageInfo();
        });                
    },

    /**
     * Método para poder cambiar entre idiomas y poder visualizar los datos de los Tesauros en el idioma deseado.
     * Gestionará el click en el tab de idiomas principal del tesauro
     */
    handleViewTesauroLanguageInfo: function(){
        const that = this;
        // Comprobar el item que está activo en el tab obteniendo el data-language de la opción "active"
        setTimeout(function(){             
            // Detectar el tab item activo para conocer el idioma en el que se desean mostrar las páginas
            const tabLanguageActive = that.tabIdiomaItem.filter(".active");
            // Obtener el idioma del tabLanguageActivo
            const languageActive = tabLanguageActive.data("language");
            // Ocultar todas las labels y mostrar únicamente las del idioma seleccionado
            $(`.${that.labelLanguageComponentClassName}`).addClass("d-none");
            // Mostrar sólo las labelsLanguageComponent del idioma seleccionado
            $(`.${that.labelLanguageComponentClassName}`).filter(function () {
                return $(this).data("languageitem") == languageActive;
            }).removeClass("d-none");
        },250);
    },    

    

    /**
     * Método para buscar elementos del tesauro en el modal
     */
    handleSearchTesauroElement: function(){
        const that = this;
        
        // Items del tesauro        
        const rowTesauroItems = $(`#${that.tesauroElementListClassName}`).find(`.${that.tesauroItemListItemClassName}`);        

        let cadena = $(`.${this.findTesauroSemanticoElementsClassName}`).val();
        // Eliminamos posibles tildes para búsqueda ampliada
        cadena = cadena.normalize('NFD').replace(/[\u0300-\u036f]/g, "").toLowerCase();
       
        if (cadena.length == 0) {
            rowTesauroItems.removeClass("d-none");            
        } else {
            rowTesauroItems.each(function (index, element) {
                const rowTesauroItem = $(this);
                // Elemento a analizar
                const tesauroNameItem = $(this).find(".component-elementName").not('.d-none').html().trim().normalize("NFD").replace(/[\u0300-\u036f]/g, "").toLowerCase();                            

                if (tesauroNameItem.includes(cadena)) {
                    rowTesauroItem.removeClass("d-none");
                    // Mostrar también a sus padres
                    const parents = rowTesauroItem.parents(".component-wrap");
                    parents.removeClass("d-none");
                    // Mostrar también los paneles hijos
                    const arr = Array.from(parents);
                    parents.length > 0 && arr.forEach( item => {                        
                        $(item).find(".tesauroChildrenPanel").removeClass("d-none");
                        // Cambiar el icono a desplegado
                        $(item).find(".showHide-icon").first().removeClass("collapsed").addClass("expanded").html("remove_circle_outline");
                    });
                } else {
                    rowTesauroItem.addClass("d-none");                    
                }               
            });
        }        



    },

    handleSaveElementTesauroSemantico: function(){
        const that = this;

        // Configuración de los parámetros para realizar la petición de guardado
        const arg = {};
        // Acción de Añadir
        arg.EditAction = 0;
        // Categoría donde se creará el elemento        
        arg.SelectedCategory = $(`.${that.cmbCrearCategoriaEnClassName}`).val();
        // Identificador de la categoría o del elemento a crear        
        arg.NewCategoryIdentifier = $(`.${that.txtIdentificacionCreacionClassName}`).val().trim();
        // Panel donde buscar los inputs con los idiomas a obtener
        const panelElementDetail = $(`.${that.panelTesauroElementCreationClassName}`);
        // Nuevo nombre de la categoría del tesauro para obtener los idiomas
        arg.NewCategoryName = that.getNombresTesauroItemMultiIdiomas(that.txtNombreCatPadreCreacionClassName, panelElementDetail);
            
        if (arg.SelectedCategory == '' || arg.NewCategoryIdentifier == '' || arg.NewCategoryName == '') {
            mostrarNotificacion("error",controlesRapidos.errorRellCampos);
            return;
        }
    
        if (!that.getValorPropsExtra(arg, false)) {
            return;
        }
    
        // Proceder al guardado de datos        
        that.handleSaveTesauroActions(arg);
    },  
    
    
    /**
     * Método para obtener los valores de la propiedad extra
     * @param {*} arg Objeto con los datos actuales. Se usará para añadir nuevos items según los valores extra existentes
     * @param {bool} edicion Indica si está siendo editado el elemento 
     * @returns Devuelve si todo ha sido correcto durante la recogida de datos extra
     */
    getValorPropsExtra: function(arg, edicion){
        const that = this;

        // Comprobar que no haya propiedades
        if ($(`.${that.panelPropTesSemExtraClassName}`).length == 0) {
            return true;
        }

        arg.CategoryExtraPropertiesValues = '';

        let claseInputs = that.panelPropTesSemExtraClassName;

        if (edicion) {
            claseInputs = 'divContEditarPropExtra';
        }

        let extraOk = true;

        // Obtener los datos de cada propiedad extra
        $('.' + claseInputs + ' .txtExtraCat').each(function () {
            if (extraOk == true){
            // Resetear el posible mensaje de error
            hideInputsWithErrors($(this));              
            arg.CategoryExtraPropertiesValues += $(this).attr('prop') + '|' + $(this).val().trim().replace(/\n/g, '');                    
            if ($(this).attr('objProp') != null) {
                if (!that.ValidUrl($(this).val())) {
                    extraOk = false;
                    mostrarNotificacion("error", textoFormSem.algunCampoMalIntro );
                    comprobarInputNoVacio($(this), true, false, textoFormSem.algunCampoMalIntro,99, 2);
                }
            }
            if ($(this).attr('lang') != null) {
                arg.CategoryExtraPropertiesValues += '@' + $(this).attr('lang') + '|||';
            }
            arg.CategoryExtraPropertiesValues += '[||]';                
            }            
        });

        arg.CategoryExtraPropertiesValues = encodeURIComponent(arg.CategoryExtraPropertiesValues);

        return extraOk;
    },

    /**
     * Método para validar una URL de una propiedad extra de un elemento del tesauro
     * @param {*} str 
     */
    ValidUrl: function(str){
        if (str == '') {
            return true;
        }
        else if (str.indexOf('http://') == -1 && str.indexOf('https://') == -1) {
            return false;
        }
        else if (str.indexOf(' ') != -1 || str.indexOf('    ') != -1) {
            return false;
        }
        return true;
    },


    /**
     * Método para mostrar u ocultar el panel para crear un elemento en el Tesauro
     * @param {bool} clearInputValues Indica si es necesario eliminar el contenido de los inputs que hay para crear una nueva propiedad del tesauro
     */
    handleShowHidePanelAddTesauroElement: function(clearInputValues){
        const that = this;
        
        if ($(`.${this.btnAddTesauroSemanticoElementClassName}`).hasClass("active")){
            // Ocultar panel            
            $(`.${this.panelTesauroElementCreationClassName}`).addClass("d-none");            
            $(`.${this.btnAddTesauroSemanticoElementClassName}`).removeClass("btn-primary").addClass("btn-outline-primary").removeClass("active");            
        }else{
            // Mostrar panel
            $(`.${this.panelTesauroElementCreationClassName}`).removeClass("d-none");
            $(`.${this.btnAddTesauroSemanticoElementClassName}`).removeClass("btn-outline-primary").addClass("btn-primary").addClass("active");            
        }          
        $(`.${this.txtIdentificacionCreacionClassName}`).focus();   
        
        
        // Limpieza de los inputs 
        if (clearInputValues == true){
            $('.' + this.panelTesauroElementCreationClassName).each(function () {                
                const panel = $(this);
                panel.find('input[type="text"]').each(function(){
                    const input = $(this);
                    // Vaciar el input 
                    input.is("input") && input.val('');
                    input.is("textarea") && input.html('');                        
                });                              
            });            
        }
    },   

    /**
     * Método para mostrar o colapsar (ocultar) el panel de los hijos del tesauro
     * @param {jqueryElement} collapseButton : Botón que dispara la visualización del panel hijo y que mostrará u ocultará según contenga ciertas clases
     */
    handleShowHideChildrenPanel: function(collapseButton){
        const that = this;
        // Clase para indicar que se puede expandir o mostrar el panel
        const expandClassName = "expanded";
        const collapseClassName = "collapsed";
        const expandIcon = "add_circle_outline";
        const collapseIcon = "remove_circle_outline";

        // Buscar el panel children correspondiente al botón de expandir/contraer
        const tesauroRowItem = collapseButton.closest(`.${that.tesauroItemListItemClassName}`);
        const panelTesauroChildrenItem = tesauroRowItem.find(`.${that.tesauroChildrenPanelClassName}`).first();               

        if (collapseButton.hasClass(collapseClassName)){
            // Expandir o Mostrar el panel children
            collapseButton.removeClass(collapseClassName);
            collapseButton.addClass(expandClassName);
            collapseButton.html(collapseIcon);  
            panelTesauroChildrenItem.removeClass("d-none");          
        }else{            
            // Ocultar o colapsar el panel children
            collapseButton.addClass(collapseClassName);
            collapseButton.removeClass(expandClassName);
            collapseButton.html(expandIcon);
            panelTesauroChildrenItem.addClass("d-none");
        }

        // Ocultar siempre el panel de edición en caso de estar abierto
        const tesauroItemRow = collapseButton.closest(`.${that.tesauroItemListItemClassName}`);
        tesauroItemRow.find(`.${that.panelElementDetailClassName}`).first().removeClass("show");
    },


    /**
     * Método para buscar y filtrar tesauros semánticos 
     * @param {jqueryItem} input Input donde se introduce el valor a buscar
     */
    handleSearchTesauroSemantico: function(input){
        const that = this;
        // Cadena introducida para filtrar/buscar
        let cadena = input.val();
        // Eliminamos posibles tildes para búsqueda ampliada
        cadena = cadena.normalize('NFD').replace(/[\u0300-\u036f]/g, "").toLowerCase();

        // Listado de componentes donde se buscará
        const tesaurosSemanticosItems = $(`.${that.tesauroListItemClassName}`);
        
        // Recorrer los items/compontes para realizar la búsqueda de palabras clave
        $.each(tesaurosSemanticosItems, function(index){
            const tesauroSemanticoItem = $(this);
            // Seleccionar el nombre sólo "visible" de la traducción. Quitamos caracteres extraños, tildes para poder hacer bien la búsqueda
            const componentName = tesauroSemanticoItem.find(".component-name").not('.d-none').html().trim().normalize("NFD").replace(/[\u0300-\u036f]/g, "").toLowerCase();
            const componentOntology = tesauroSemanticoItem.find(".component-ontologia").not('.d-none').html().trim().normalize("NFD").replace(/[\u0300-\u036f]/g, "").toLowerCase();
            const componentFuente = tesauroSemanticoItem.find(".component-fuente").not('.d-none').html().trim().normalize("NFD").replace(/[\u0300-\u036f]/g, "").toLowerCase();
            
            if (componentName.includes(cadena) || componentOntology.includes(cadena) || componentFuente.includes(cadena)){
                // Mostrar la fila
                tesauroSemanticoItem.removeClass("d-none");                
            }else{
                // Ocultar la fila
                tesauroSemanticoItem.addClass("d-none");
            }            
        });   
    },

    /**
    * Método que se ejecutará al cargarse la web para saber el nº de items
    */
     handleCheckNumberOfTesauros: function(){        
        const that = this;
        const numberOfItems = this.tesaurosListContainer.find($(`.${that.tesauroListItemClassName}`)).length;
        // Mostrar el nº de items                
        that.numTesaurosSemanticos.html(numberOfItems);
    }, 

    /**
    * Método que se ejecutará al cargarse la web para saber el nº de items que hay dentro de un tesauro
    */
    handleCheckNumberOfItemsInTesauros: function(){        
        const that = this;
        const numberOfItems = $(`#${this.tesauroElementListClassName}`).find($(`.${that.tesauroItemListItemClassName}`)).length;
        // Mostrar el nº de items                
        $(`.${that.numTesaurosSemanticosItemsClassName}`).html(numberOfItems);
    },             

    /**
     * Método para eliminar un tesauro ya que previamente se ha confirmado su eliminación
     */
    handleConfirmDeleteTesauro: function(){
        const that = this;

        // Obtener los datos a enviar a partir de la "fila seleccionada"        
        // Ontología
        const ontologyUrl = this.filaTesauro.find(".component-ontologia").html().trim();
        // Fuente
        const source = this.filaTesauro.find(".component-fuente").html().trim();        

        // Argumentos para el borrado
        const arg = {
            Source: source,
            Ontologia: ontologyUrl
        };

        // Mostrar loading
        loadingMostrar();

        GnossPeticionAjax(that.urlDeleteTesauro, arg, true)
        .done(function (data) {
            // OK borrado tesauro
            // Borrado de fila del tesauro
            that.filaTesauro.remove(); 
            // Actualizar el nº de items
            that.handleCheckNumberOfTesauros();                               
            // Ocultar modal de borrado
            dismissVistaModal(that.modalDeleteTesauro);
            // Resetamos el flag de confirmar borrado
            that.confirmDeleteTesauro = false;
            // Mostrar aviso de ok
            mostrarNotificacion("success", "Se ha borrado el tesauro correctamente.");            
        }).fail(function (data) {
            that.confirmDeleteTesauro = false;
            mostrarNotificacion("error", data);            
        }).always(function () {
            loadingOcultar();
        });       

    },

    /**
     * Método para establecer la fila a eliminar como "Borrado" o "No borrado"
     * @param {bool} deleteTesauro 
     */
    handleSetDeleteTesauro: function(deleteTesauro){
        const that = this;

        if (deleteTesauro){
            // Realizar el "borrado"                        
            // Añadir la clase de "deleted" a la fila
            that.filaTesauro.addClass("deleted");
        }else{            
            // Eliminar la clase de "deleted" a la fila de la página
            that.filaTesauro.removeClass("deleted");
        }
    },  

    /**
     * Método para cambiar el nombre una vez se escriba en la fila del tesauro "nuevo"
     */
    handleChangeNombreValueInTesauroRow: function(){
        const that = this;
        const value = this.txtNombre.val();
        // Añadir la información del tesauro en la fila correspondiente
        this.filaTesauro.find(".component-name").html(value);

        // Modificar el select si este no cambia de valor
        this.handleChangeOntologyValueInTesauroRow();
    },

    /**
     * Método para cambiar el nombre una vez se escriba en la fila del tesauro "nuevo"
     */
     handleChangeFuenteValueInTesauroRow: function(){
        const that = this;
        const value = this.txtFuente.val();
        // Añadir la información del tesauro en la fila correspondiente
        this.filaTesauro.find(".component-fuente").html(value);
        // Modificar el select si este no cambia de valor
        this.handleChangeOntologyValueInTesauroRow();        
    }, 
    
    /**
     * Método para cambiar el nombre una vez se escriba en la fila del tesauro "nuevo"
     */
     handleChangeOntologyValueInTesauroRow: function(){
        const that = this;
        const value = this.selectOntology.val();
        // Añadir la información del tesauro en la fila correspondiente
        this.filaTesauro.find(".component-ontologia").html(value);        
    },     

    /**
     * Método para vaciar el modal de creación de un tesauro ya sea porque se haya guardado porque se haya cancelado la creación.
     */
    handleEmptyValuesFromModal: function(){
        const that = this;

        // Vaciado de datos introducidos previamente        
        this.txtPrefijo.val('');
        this.txtFuente.val('');
        
        // Tener en cuenta si es de reciente creación y por tanto no se desea guardar
        if (that.filaTesauro.hasClass("newTesauro")){
            // Eliminar la fila que se acaba de eliminar                                
            that.filaTesauro.remove();
            // Actualizar el contador
            that.handleCheckNumberOfTesauros();
        }
    },

    /**
     * Método para cargar el modal para crear un nuevo Tesauro semántico
     */
    handleLoadCreateTesauro: function(){   
        const that = this;
       
        // Mostrar loading hasta que finalice la petición
        loadingMostrar();   

        // Realizar la petición de la cláusula a crear
        GnossPeticionAjax(                
            that.urlLoadCreateTesauro,
            null,
            true
        ).done(function (data) {
            // Devuelve la fila/row de la cláusula a crear
            //const lastTesauro = that.tesaurosListContainerId.children().last();                       
            // Añadir la nueva fila al listado            
            that.tesaurosListContainer.append(data);                        
            // Referencia a la nueva
            that.newTesauro = that.tesaurosListContainer.children().last();
            // Añadirle el flag de "nuevo tesauro" para saber que es de reciente creación.
            that.newTesauro.addClass("newTesauro");   
            // Establecer como fila seleccionada la nueva creada
            that.filaTesauro = that.newTesauro;    
            // Abrir el modal para crear el nuevo Tesauro. La información de edición del tesauro difiere del modal de creación
            that.modalNewTesauro.modal("show"); 
            // Inicializar eventos necesarios (operativa multiIdioma, select2 checkNumberOfTesauros)                                   
            that.triggerEvents();
        }).fail(function (data) {
            // Mostrar error al tratar de crear un item
            mostrarNotificacion("error", data);                
        }).always(function () {
            // Ocultar loading de la petición
            loadingOcultar();
        });            
    },

    /**
     * Método para crear el nuevo tesauro
     */
    handleSaveTesauro: function(){
        const that = this;
                
        // Guardado de valores a enviar
        if (that.comprobarNombresVacios() == true){
            return;
        }
        const nombre = $(`input[name="${that.txtNombreName}"]`).val();
        const ontologia = this.selectOntology.val()
        const prefijo = this.txtPrefijo.val();
        const source = this.txtFuente.val();

        if (nombre == "" || ontologia == "" || prefijo == ""){
            mostrarNotificacion("El tesauro debe tener un nombre, una ontología asociada y un prefijo.");
            return;
        }        
        // Creación del objeto
        const dataPost = {
            Nombre: nombre,
            Ontologia: ontologia,
            Prefijo: prefijo,
            Source: source
        }

        loadingMostrar();

        GnossPeticionAjax(
            this.urlCreateTesauro,
            dataPost,
            true
        ).done(function (data) {
            // OK Guardado correcto de Tesauro
            mostrarNotificacion("success", "Los cambios se han guardado correctamente"); 
            // Quitar el flag ya que se ha creado correctamente
            that.filaTesauro.removeClass("newTesauro");
            // Eliminamos la fila ya que añadimos a continuación la devuelta por backend
            that.filaTesauro.remove();
            that.tesaurosListContainer.append(data); 
            
            // Modal para cerrar                                
            dismissVistaModal(that.modalNewTesauro);                                                     

        }).fail(function (data) {
            mostrarNotificacion("error", "Se ha producido un error al tratar guardar el tesauro semántico. Contacta con el administrador.");
        }).always(function () {
            loadingOcultar();
        });
    },

    /**
     * Método para comprobar que no hay Nombres vacíos en las páginas creadas/editadas
     * @returns bool: Devuelve un valor booleano indicando si todo está OK para proceder con el guardado de las páginas.
     */
    comprobarNombresVacios: function(){
        const that = this;    
        
        // Flag para resetear la revisión de los nombres vacíos
        that.errorNombreVacio = false;

        // Inputs con los nombres de todas las páginas que hayan sido modificadas y no borradas (Más velocidad)        
        const inputsNombre = $(`.inputsMultiIdioma.basicInfo input[name="${that.txtNombreName}"]`);
        inputsNombre.each(function () {
            if (that.errorNombreVacio == false){                
                that.comprobarNombreVacio($(this));
            }  
        });
        return that.errorNombreVacio;
    },

    /**
     * Método para comprobar que el nombre de una página no está vacío.
     * @param {*} inputName: Input a comprobar
     * @returns bool: Devuelve un valor booleano indicando si todo está OK para proceder con el guardado de las páginas.
     */
    comprobarNombreVacio: function (inputName) {
        const that = this
        
        // Contiene los tabs en idiomas y el div con los inputs en idiomas completo
        const formularioEdicion = inputName.closest(".formulario-edicion");
        const panInputsMultiIdioma = inputName.closest(".inputsMultiIdioma");
        // Contenedor donde se encuentran datos básicos de la página (Nombre, Url/Ruta, MetaDescripción )
        const panMultiIdioma = formularioEdicion.find(".panContenidoMultiIdioma.basicInfo");

        // Id del input a tratar
        const inputId = inputName.attr("id");

        const listaTextos = [];
        
        if (operativaMultiIdioma.listaIdiomas.length > 1 && panMultiIdioma.length > 0) {
            let textoMultiIdioma = "";
            // Comprobar que hay al menos un texto por defecto para el nombre
            let textoIdiomaDefecto = panMultiIdioma.find(`#input_${inputId}_${operativaMultiIdioma.idiomaPorDefecto}`).val();
            
            if (textoIdiomaDefecto == null || textoIdiomaDefecto == "") {                
                that.mostrarErrorNombreVacio(panMultiIdioma.find(`#input_${inputId}_${operativaMultiIdioma.idiomaPorDefecto}`));
                that.errorNombreVacio = true;
                return true;
            }
            // Recorrer todos los idiomas para detectar posibles problemas con el nombre                
            $.each(operativaMultiIdioma.listaIdiomas, function () {
                // Obtención del Key del idioma
                const idioma = this.key;
                // Asignar el valor por defecto de la ruta al idioma si este no dispone de valor para Nombre
                let textoIdioma = $(`#input_${inputId}_${idioma}`).val();
                if (textoIdioma == null || textoIdioma == "") {
                    textoIdioma = textoIdiomaDefecto;
                    $(`#input_${inputId}_${idioma}`).val(textoIdioma);
                }
                // Escribir el nombre del multiIdioma en el campo Hidden
                textoMultiIdioma += textoIdioma + "@" + idioma + "|||";
                
                listaTextos.push({ "key": idioma, "value": textoIdioma });
            });            
            inputName.val(textoMultiIdioma);
        }else{
            // Sin multiIdioma.
            let textoIdiomaDefecto = panMultiIdioma.find(`#input_${inputId}_${operativaMultiIdioma.idiomaPorDefecto}`).val();
            // Establecer el nombre en el input correspondiente
            inputName.val(textoIdiomaDefecto);
        }
        return false;
    },  
    
    /**
     * Método para mostrar el error del nombre vacío encontrado en una página. Se muestra cuando el nombre de una página en el idioma por defecto no existe.     
     * @param {jqueryObject} input : Elemento jquery del input donde se ha producido el error. Puede que ser undefined
     */
    mostrarErrorNombreVacio: function(input){        
        if (input != undefined){
            comprobarInputNoVacio(input, true, false, "El nombre del tesauro no puede estar vacío.", 0);
        }

        setTimeout(function(){  
            mostrarNotificacion("error", "El nombre del tesauro no puede estar vacío.");
        },1000);  
    },    





    /**
     * Método para editar u obtener información de un determinado Tesauro semántico cuando se pulsa en "Edit"
     */
    handleEditTesauro: function(){
        const that = this;

        // Parámetros a enviar
        const arg = {};
        
        // Obtener los datos a enviar a partir de la "fila seleccionada"        
        // Ontología
        const ontologyUrl = this.filaTesauro.find(".component-ontologia").html().trim();
        // Fuente
        const source = this.filaTesauro.find(".component-fuente").html().trim();
        // Acción
        const editAction = 10;
        
        // Construcción del objeto
        arg.OntologyUrl = ontologyUrl;
        arg.SourceSemanticThesaurus = source;
        arg.EditAction = editAction;
                    
        getVistaFromUrl(that.urlEditTesauro, 'modal-dinamic-content', arg, function(result){
            if (result != requestFinishResult.ok){
                // KO al cargar la vista
                mostrarNotificacion("error", "Error al tratar de cargar los datos del tesauro semántico seleccionado.");
                dismissVistaModal();                                                                 
            }else{
                // OK -> Cargados los datos - Realizar alguna operativa al cargar los datos
                that.triggerEventsForTesauroModalItems();
                // Preparar selects y contar total de tesauros
                that.triggerEvents();
            }              
        });
    },


    /**
     * Método para ejecutar acciones u operativas necesarias una vez se carga el modal para editar un tesauro y sus items que contiene
     */
    triggerEventsForTesauroModalItems: function(){
        const that = this;
        // Configurar operativa de NestedSortable sin tecla ESC              
        // operativaNestedSortable.init(that.tesauroElementListClassName, that.btnDragTesauroIconClassName, that.tesauroItemListItemClassName, that.handleReorderItemsFinished); 
        // Configurar operativa de NestedSortable con tecla ESC              
        operativaNestedSortable.initAllowingEscapeButtonToCancelNestedSortableAction(that.tesauroElementListClassName, that.btnDragTesauroIconClassName, that.tesauroItemListItemClassName, that.handleReorderItemsFinished); 
        // Contabilizar los items del Tesauro
        that.handleCheckNumberOfItemsInTesauros();   
    },

    /**
     * Método "completion/callback" que se ejecutará una vez finalice el evento de arrastrar/reordenar un item del tesauro semántico
     * @param {jqueryElement} movedItem : Item que ha sido arrastrado o movido vía nestedSortable
     */
    handleReorderItemsFinished: function(movedItem){
        const that = operativaGestionTesaurosSemanticos;
        // Configuración de los parámetros para realizar la petición de guardado
        const arg = {};
        // Acción de Mover
        arg.EditAction = 3;        
        // Categoría destino. Por defecto será root/raíz
        let selectedCategoryToMove = "[RAIZ]";

        // Obtener el padre del item arrastrado                
        let $parentNode = movedItem.parents("li").first();  
        
        // Obtener el id del padre para acción de mover item del Tesauro
        if ($parentNode.length > 0){                       
            selectedCategoryToMove = $parentNode.data("parent-categories-move-categories");
        }  
        // Establecer la categoría a la que se moverá el tesauroItem
        arg.SelectedCategory = selectedCategoryToMove;
        // Categorías que se moverán o seleccionadas
        arg.SelectedCategories = movedItem.data("parent-categories-move-categories");           
        // Ejecutar y guardar acciones
        that.handleSaveTesauroActions(arg);
    },

    /**
     * Método que se ejecutará cuando se pulse en el botón de "Eliminar" un item del tesauro
     * @param {jqueryElement} deleteButton : Botón de eliminar sobre el que se ha pulsado.
     */
    handleDeleteTesauroItem: function(deleteButton){
        const that = this;

        // Obtener los datos a eliminar a partir del botón de borrado pulsado
        const tesauroItemRow = deleteButton.closest(`.${that.tesauroItemListItemClassName}`);

        // Configuración de los parámetros para realizar la petición de guardado
        const arg = {};
        // Acción de Eliminar
        arg.EditAction = 7;        
        // Tesauro item que se procederá a eliminar
        arg.SelectedCategories = tesauroItemRow.data("parent-categories-move-categories");

        // Ejecutar y guardar las acciones
        that.handleSaveTesauroActions(arg, function(result){
            if (result == requestFinishResult.ok){                           
                setTimeout(function() {                                  
                    // Eliminar la fila del item del tesauro
                    tesauroItemRow.remove();
                },800);                                      
            }
        });
    },     
    
    /**
     * Método que se ejecutará cuando se pulse en el botón de "Renombrar/Guardar" para guardar el nuevo nombre de la categoría del Tesauro
     * @param {jqueryElement} btnRenameSaveTesauroItem : Botón de guardar/renombrar sobre el que se ha pulsado.
     */
    handleRenameTesauroItem: function(btnRenameSaveTesauroItem){
        const that = this;
        
        const tesauroItemRow = btnRenameSaveTesauroItem.closest(`.${that.tesauroItemListItemClassName}`);
        // Panel donde está la información de la categoría que está siendo editada
        const panelElementDetail = tesauroItemRow.find(`.${that.panelElementDetailClassName}`).first();

        // Configuración de los parámetros para realizar la petición de guardado
        const arg = {};
        // Acción de Renombrar
        arg.EditAction = 1;
        arg.SelectedCategory = tesauroItemRow.data("parent-categories-move-categories");
        // Nuevo nombre de la categoría del tesauro
        arg.NewCategoryName = that.getNombresTesauroItemMultiIdiomas('txtNuevoNombre', panelElementDetail);

        if (arg.NewCategoryName.trim().length == 0){
            mostrarNotificacion("error", "Es necesario establecer el nuevo nombre de la categoría del tesauro");
            return
        }

        that.handleSaveTesauroActions(arg);                        
    },

    /**
     * Método para obtener el nombre en un único y múltiples idiomas del Tesauro item del que se desea renombrar
     * @param {*} inputId Id o clase de los inputs donde se almacenan los nuevos nombres de la categoría del Tesauro
     * @param {*} panelElementDetail Panel de la fila del tesauro donde están los inputs con los nombres hay que analizar o revisar 
     * @returns Devuelve el nombre de la categoría del tesauro en uno o múltiples idiomas
     */
    getNombresTesauroItemMultiIdiomas: function(inputId, panelElementDetail){
        const that = this;
        let nombre = '';

        if (panelElementDetail.find((`#${inputId}`)).length > 0) {
            // Sólo hay un idioma
            nombre = panelElementDetail.find((`.${inputId}`)).val();
        }
        else {
            // Hay múltiples idiomas
            let todosIdioOk = true;
    
            // Recorrer cada uno de los idiomas para construir su nombre
            panelElementDetail.find($(`.${inputId}`)).each(function () {
                if ($(this).val().trim() == '') {
                    todosIdioOk = false;
                    return;
                }                
                const idioma = $(this).data("language"); // this.className.replace(inputId + ' ', '');
                const inputValue = $(this).val();
                nombre += `${inputValue}@${idioma}|||`;
            });
    
            if (!todosIdioOk) {
                nombre = '';
            }
        }
        return nombre;
    },

    /**
     * Método para mandar los parámetros necesarios a almacenar para el guardado de los items del tesauro
     * @param {*} arg 
     * @param {*} completion 
     */
    handleSaveTesauroActions: function(arg, completion = undefined){
        const that = operativaGestionTesaurosSemanticos;

        // Datos extra para el guardado
        arg.ActionsBackUp = "" // encodeURIComponent($('#txtAccionesTesauroHack').val());
        arg.ExtraSemanticPropertiesValuesBK = "" // encodeURIComponent($('#txtHackExtraPropsTesSem').val());
        // Ontología seleccionada sobre el que se hará el guardado
        arg.OntologyUrl = that.ontologyUrl.split('|')[0];
        arg.SourceSemanticThesaurus = that.ontologyUrl.split('|')[1];        

        // Mostrar loading para la petición
        loadingMostrar();

        // Petición para guardado de Tesauro
        GnossPeticionAjax(that.urlEditTesauro, arg, true)
        .done(function (data) {            
            // OK - Guardado correcto                        
            // Cargar la vista (TesauroITEMS) en el contendor correspondiente
            if (arg.EditAction == 0 || arg.EditAction == 1){
                $(`#${that.tesauroListContainerId}`).html(data);
            }            
            // Cargar la operativa necesaria para los items del tesauro
            that.triggerEventsForTesauroModalItems();
            // Mostrar mensaje de OK al usuario
            switch (arg.EditAction) {

                case 0:
                    // Tesauro Item Añadido
                    mostrarNotificacion("success", "La categoría del tesauro se ha añadido correctamente."); 
                    // Ocultar el panel y vaciar los inputs rellenados
                    that.handleShowHidePanelAddTesauroElement(true);                
                    break;                    

                case 1:
                    // Tesauro Item Renombrado
                    mostrarNotificacion("success", "La categoría del tesauro se ha renombrado correctamente.");                    
                    break;                    

                case 7:
                    // Eliminado
                    mostrarNotificacion("success", "La categoría del tesauro se ha eliminado correctamente.");
                    break;                
            
                default:
                    mostrarNotificacion("success", "Los cambios se han guardado correctamente");
                    break;
            }

            completion != undefined && completion(requestFinishResult.ok);           
            
            /*$('#filtroRapido').val('');
            $('#panAccionesTes').html(data);
    
            if (arg.EditAction == 2) {
                $('#PanParaSelectorCategorias input[type=checkbox]').prop('disabled', 'disabled');
                $('#panMoverCategoria').show();
            }
            else if (arg.EditAction == 4) {
                $('#PanParaSelectorCategorias input[type=checkbox]').prop('disabled', 'disabled');
                $('#panOrdenarCategoria').show();
            }
            else if (arg.EditAction == 6 && $('#txtHackCatTesSel').val() != '') {
                $('#PanParaSelectorCategorias input[type=checkbox]').prop('disabled', 'disabled');
                $('#panEliminarCategoria').show();
            }
            else {
                $('#panCambiarNombreCategoria').hide();
                $('#panCrearCategoria').hide();
                $('#panEliminarCategoria').hide();
                $('#panMoverCategoria').hide();
                $('#panOrdenarCategoria').hide();
                $('#panEditarPropExtra').hide();
            }
    
            if (arg.EditAction == 9) {
                $('#divMensOK').html('<div class="ok"><p>' + textoRecursos.guardadoOK + '</p></div>');
                $('#txtAccionesTesauroHack').val('');
                VolerListTesEditTesSem();
            }
            */    
        }).fail(function (data) {
            // KO - Error al tratar de guardar los datos del Tesauro
            mostrarNotificacion("error", "Se ha producido un error al guardar la categoría del tesauro. Contacta con el administrador o inténtalo de nuevo más tarde.");
            completion != undefined && completion(requestFinishResult.ko);
        }).always(function () {
            loadingOcultar();            
        });
    },
}



const operativaGestionParametrosBusquedaPersonalizados = {

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

    /**
     * Lanzar comportamientos u operativas necesarias para el funcionamiento de la sección
     */
    triggerEvents: function(){
        const that = this;           

        // Indicar el nº de items existente
        that.handleCheckNumberOfParametrosBusqueda();
    },   

    /**
     * Configuración de las rutas de acciones necesarias para hacer peticiones a backend
     */
    configRutas: function (pParams) {
        // Url para editar un certificado
        this.urlBase = refineURL();
        this.urlSave = `${this.urlBase}/save`;
 
        // Flag para controlar borrado
        this.confirmDeleteParametroBusqueda = false;
        // Flag para controlar posibles errores
        this.errorsBeforeSaving = false;
        
    },  

    /*
    * Inicializar elementos de la vista
    * */
    config: function (pParams) {        
        // Contenedor modal
        this.modalContainer = $("#modal-container");
        this.modalParametroClassName = "modal-parametroBusqueda";
        this.modalDeleteParametro = $("#modal-delete-parametro-busqueda");

        // Buscador de parametros
        this.txtBuscarParametroBusqueda = $("#txtBuscarParametroBusqueda");
        // Contenedor de objetos de conocimiento
        this.parametroBusquedaListContainerId = 'id-added-parametroBusqueda-list';
        this.parametroBusquedaListContainer = $(`#${this.parametroBusquedaListContainerId}`);
        // Nombre de la fila de cada parametro de búsqueda
        this.parametroBusquedaListItemClassName = "parametroBusqueda-row";
        // Botón para añadir un nuevo parametroBusqueda
        this.btnNewParametroBusqueda = $("#btnNewParametroBusqueda");
        // Botón de editar parametro
        this.btnEditParametroBusquedaClassName = "btnEditParametroBusqueda";                
        // Botón para eliminar un parámetro
        this.btnDeleteParametroBusquedaClassName = "btnDeleteParametroBusqueda";
        // Botón para guardar un tesauro nuevo
        this.btnSaveParametroBusqueda = $("#btnSaveParametroBusqueda");
        // Botón para confirmar la eliminación de un parametro busqueda
        this.btnConfirmDeleteParametroBusquedaClassName = "btnConfirmDeleteParametroBusqueda";
        // Botón de no confirmar el borrado de un objeto de conocimiento
        this.btnNotConfirmDeleteParametroBusquedaClassName = "btnNotConfirmDeleteParametroBusqueda";
        // Contador de número de items existentes
        this.numParametroBusqueda = $("#numParametroBusqueda");

        /* Inputs del modal de creación de ParametroBusqueda */
        // Nombre
        this.txtNombreDelParametroBusquedaClassName = "txtNombreDelParametroBusqueda";
        // Where
        this.txtWhereDelParametroClassName = "txtWhereDelParametro";
        // OrderBy
        this.txtOrderByDelParametroClassName = "txtOrderByDelParametro";
        // Fuente tesauro
        this.txtWhereDeFacetaParametroClassName = "txtFuente";   
        // Botón para guardar el parámetro
        this.btnSaveParametroBusquedaClassName = "btnSaveParametroBusqueda";

        // Fila del parámetro que está siendo editada
        this.filaParametro = undefined;
        // Flags para confirmar la eliminación de un parámetro página
        this.confirmDeleteParametro = false;
    },   

    /**
     * Configuración de eventos de elementos del Dom (Botones, Inputs...)     
     */
    configEvents: function (pParams) {
        const that = this;

        // Comportamientos del modal container 
        this.modalContainer.on('show.bs.modal', (e) => {
            // Aparición del modal
            // Establecer quién ha disparado el modal para poder cambiar sus atributos una vez se guarden los grupos desde el modal
            that.triggerModalContainer = $(e.relatedTarget);                              
        })
        .on('hide.bs.modal', (e) => {
            // Acción de ocultar el modal
        })
        .on('hidden.bs.modal', (e) => {
            // Vaciar el modal
            resetearModalContainer(); 
        });

        // Comportamientos del modal de editar/crear
        configEventByClassName(`${that.modalParametroClassName}`, function(element){
            const $modal = $(element);
            $modal
            .on('show.bs.modal', (e) => {
                // Aparición del modal
                that.triggerModalContainer = $(e.relatedTarget);
            })
            .on('hide.bs.modal', (e) => {
               // Eliminar la fila si es de reciente creación y no se ha guardado
               if (that.filaParametro.hasClass("newParametro")){
                that.filaParametro.remove();
               }
            }); 
        }); 

        // Comportamientos del modal de eliminar parámetro
        that.modalDeleteParametro.
        on('show.bs.modal', (e) => {
            // Aparición del modal
            that.triggerModalContainer = $(e.relatedTarget);
        })
        .on('hide.bs.modal', (e) => {
            // Acción de ocultar el modal
            // Si el modal se cierra sin confirmar borrado, desactiva el borrado
            if (that.confirmDeleteParametro == false){
                that.handleSetDeleteParametro(false);                
            }       
        }); 
           
        // Botón para editar el parámetro
        configEventByClassName(`${that.btnEditParametroBusquedaClassName}`, function(element){
            const $jqueryElement = $(element);
            $jqueryElement.off().on("click", function(){
                that.filaParametro = $(this).closest(`.${that.parametroBusquedaListItemClassName}`);
            });	                        
        });

        // Búsquedas de parámetros de búsqueda
        this.txtBuscarParametroBusqueda.off().on("keyup", function (event) {
            const input = $(this);
            clearTimeout(that.timer);
            that.timer = setTimeout(function () {                                                                        
                // Acción de buscar / filtrar
                that.handleSearchParametroBusqueda(input);                                         
            }, 500);
        });        
        
        // Crear un nuevo parametro personalizado
        this.btnNewParametroBusqueda.on("click", function(){
            const url = $(this).data("url");
            that.handleLoadCreateParametroBusqueda(url);
        });


        // Botón/es para confirmar la eliminación de un parámetro página (Modal -> Sí)        
        configEventByClassName(`${that.btnConfirmDeleteParametroBusquedaClassName}`, function(element){
            const $jqueryElement = $(element);
            $jqueryElement.off().on("click", function(){
                that.confirmDeleteParametro = true;
                that.handleDeleteParametro();                
            });	                        
        });        
        
        // Guardar parámetro vía modal. Se procederá al guardado de todos.
        configEventByClassName(`${that.btnSaveParametroBusquedaClassName}`, function(element){
            const $jqueryElement = $(element);
            $jqueryElement.off().on("click", function(){
                that.handleSaveParametros();
            });	                        
        });

        // Botón para eliminar un parámetro de búsqueda
        configEventByClassName(`${that.btnDeleteParametroBusquedaClassName}`, function(element){
            const $btnDelete = $(element);
            $btnDelete.off().on("click", function(){                
                // Fila correspondiente a la pagina eliminada
                that.filaParametro = $btnDelete.closest(`.${that.parametroBusquedaListItemClassName}`);                
                // Marcar el parámetro como "Eliminar" a modo informativo al usuario
                that.handleSetDeleteParametro(true);
            });	                        
        });      
    },

    /**
     * Método para eliminar un parámetro previa confirmación realizada desde el modal
     */
     handleDeleteParametro: function(){
        const that = this;                  
        // 1 - Llamar al método para el guardado de parámetros
        that.handleSaveParametros(function(isOk, data){
            if (isOk == requestFinishResult.ok){                
                dismissVistaModal(that.modalDeleteParametro);
                // 3 - Ocultar loading
                loadingOcultar();                
                // 4 - Ocultar la fila que se desea eliminar
                that.filaParametro.addClass("d-none");
                // 5 - Eliminar la fila que se acaba de eliminar                                
                that.filaParametro.remove();
                // 6 - Actualizar el contador de nº de páginas            
                that.handleCheckNumberOfParametrosBusqueda();                                
                // El guardado ha sido correcto
                setTimeout(function() {
                    mostrarNotificacion("success", "El parámetro de búsqueda se ha eliminado correctamente.");
                },500);                
            }else{
                // Se ha producido un error en el borrado
                mostrarNotificacion("error", "Se ha producido un error al tratra de eliminar el parámetro. Contacta con el administrador.");
            }
        });      
    },     

    /**
     * Método para marcar o desmarcar el parámetro como "Eliminado" dependiendo de la elección vía Modal
     * @param {Bool} deleteParametro que indicará si se desea eliminar o no el parámetro
     */
    handleSetDeleteParametro: function(deleteParametro){
        const that = this;

        if (deleteParametro){
            // Realizar el "borrado" de la página
            // 1 - Marcar la opción "TabEliminada" a true
            that.filaParametro.find('[name="TabEliminada"]').val("true");           
            // 2 - Añadir la clase de "deleted" a la fila de la página
            that.filaParametro.addClass("deleted");
        }else{
            // 1 - Marcar la opción "TabEliminada" a false
            that.filaParametro.find('[name="TabEliminada"]').val("false");            
            // 2 - Eliminar la clase de "deleted" a la fila de la página
            that.filaParametro.removeClass("deleted");
        }
    },  

    /**
     * Método para recoger los datos y proceder a su guardado
     */
    handleSaveParametros: function(completion = undefined){
        const that = this;
        
        // Objeto con los datos a enviar para su guardado                
        that.ListaPestanyas = {};
        // Contador para crear el objeto de datos correctamente
        let cont = 0;        

        $(`.${this.parametroBusquedaListItemClassName}`).each(function () {
            that.getParametrosBusquedaData($(this), cont++);
        });
        
        // Guardar datos
        that.handleSave(completion);
        
    },

    /**
     * Método para guardar los datos recogidos de parámetros de búsqueda
     * @param {function} completion : Función o comportamiento a realizar una vez se realice el proceso de guardar parámetros. Por defecto será undefined
     */
    handleSave: function(completion = undefined){
        const that = this;

        // Mostrar loading
        loadingMostrar();

        // Realizar petición para el guardado de datos
        GnossPeticionAjax(
            that.urlSave,
            that.ListaPestanyas,
            true
        ).done(function (data) {
            // OK Guardado correcto
            // Quitar aquella fila el flag de nueva creación
            $(".newParametro").removeClass("newParametro");
            if (completion == undefined){
                // Mostrar mensaje OK y recargar la página
                const modal = $(that.filaParametro).find(`.${that.modalParametroClassName}`);                                          
                dismissVistaModal(modal);
                
                // Cerrar el modal y recargar la página para recargar los cambios sólo en edición o creación                
                setTimeout(function() {                                                                          
                    mostrarNotificacion("success", "Los cambios se han guardado correctamente.");                                     
                    // Recargar la página
                    location.reload();                                                   
                }
                ,500);            
            }else{
                // Devolver a completion para posterior ejecución
                completion(requestFinishResult.ok, data)
            }        

        }).fail(function (data) {
            const error = data.split('|||');
            if (error[0].startsWith("No es posible que")) {
                mostrarNotificacion("error", "No puede haber un paso obligatorio después de otro que no lo es.");
            }
            else if (error[0].startsWith("No es posible guardar")) {
                mostrarNotificacion("error", data);
            }
            else {
                mostrarNotificacion("error", "Se ha producido un error al tratar de guardar los datos. Por favor, contacta con el administrador.");
            }                      
        }).always(function () {
            // Ocultar el loading
            loadingOcultar();
        });
    },

    /**
     * Método para construir el objeto que se enviará para guardar los parámetros de búsqueda
     */
    getParametrosBusquedaData: function(fila, num){
        const that = this;

        // Panel modal donde se encuentra toda la información 
        const panelEdicion = fila.find('.modal-parametroBusqueda');
        // Prefijo para la construcción del objeto
        const prefijoClave = 'ListaPestanyas[' + num + ']';
        // Indicar si se desea o no eliminar
        that.ListaPestanyas[prefijoClave + '.Deleted'] = panelEdicion.find('[name="TabEliminada"]').val();

        // Comprobar si es un nuevo parámetro a crear
        const nueva = fila.hasClass("newParametro");
        that.ListaPestanyas[prefijoClave + '.Nueva'] = nueva;

        // Obtener de datos del parámetro de búsqueda
        const nombre = panelEdicion.find(`.${that.txtNombreDelParametroBusquedaClassName}`).val();
        const where = panelEdicion.find(`.${that.txtWhereDelParametroClassName}`).val().trim();
        const orderBy = panelEdicion.find(`.${that.txtOrderByDelParametroClassName}`).val().trim();
        const whereFaceta = panelEdicion.find(`.${that.txtWhereDelParametroClassName}`).val().trim();

        that.ListaPestanyas[prefijoClave + '.NombreParametro'] = nombre;
        that.ListaPestanyas[prefijoClave + '.WhereParametro'] = where;
        that.ListaPestanyas[prefijoClave + '.OrderByParametro'] = orderBy;
        that.ListaPestanyas[prefijoClave + '.WhereFacetaParametro'] = whereFaceta;    
    },
  
    
    /**
     * Url para crear un parámetro de búsqueda vía modal
     * @param {*} url 
     */
    handleLoadCreateParametroBusqueda: function(url){
        const that = this;
       
        // Mostrar loading hasta que finalice la petición
        loadingMostrar();   

        // Realizar la petición de la cláusula a crear
        GnossPeticionAjax(                
            url,
            null,
            true
        ).done(function (data) {            
            // Añadir la nueva fila al listado            
            that.parametroBusquedaListContainer.append(data);                        
            // Referencia a la nueva
            that.newParametro = that.parametroBusquedaListContainer.children().last();
            // Añadirle el flag de "nuevo" para saber que es de reciente creación.
            that.newParametro.addClass("newParametro");   
            // Establecer como fila seleccionada la nueva creada
            that.filaParametro = that.newParametro;    
            // Actualizar el nº de items
            that.handleCheckNumberOfParametrosBusqueda();                                                                        
            // Abrir el modal para editar/crear el nuevo parámetro añadido            
            that.newParametro.find(`.${that.btnEditParametroBusquedaClassName}`).trigger("click");
        }).fail(function (data) {
            // Mostrar error al tratar de crear un item
            mostrarNotificacion("error", data);                
        }).always(function () {
            // Ocultar loading de la petición
            loadingOcultar();
        });           
    },

    /**
     * Método para obtener el número total de parámetros existentes y actualizar su contador.
     */
    handleCheckNumberOfParametrosBusqueda: function(){
        const that = this;
        const numberParametrosBusqueda = that.parametroBusquedaListContainer.find($(`.${that.parametroBusquedaListItemClassName}`)).length;
        // Mostrar el nº de items                
        that.numParametroBusqueda.html(numberParametrosBusqueda);

        // No mostrar el item si no hay resultados
        numberParametrosBusqueda == 0 ? that.numParametroBusqueda.addClass("d-none") : that.numParametroBusqueda.removeClass("d-none");        
    },

    /**
     * 
     * @param {*} input 
     */    
    handleSearchParametroBusqueda: function(input){

        const that = this;
        // Cadena introducida para filtrar/buscar
        let cadena = input.val();
        // Eliminamos posibles tildes para búsqueda ampliada
        cadena = cadena.normalize('NFD').replace(/[\u0300-\u036f]/g, "").toLowerCase();

        // Listado de componentes donde se buscará
        const parametrosBusqueda = $(`.${that.parametroBusquedaListItemClassName}`);

        // Recorrer los items para realizar la búsqueda de palabras clave
        $.each(parametrosBusqueda, function(index){
            const parametroItem = $(this);
            // Seleccionar el nombre sólo "visible" de la traducción. Quitamos caracteres extraños, tildes para poder hacer bien la búsqueda
            const componentContenido = parametroItem.find(".component-name").html().trim().normalize("NFD").replace(/[\u0300-\u036f]/g, "").toLowerCase();
            const componentWhere = parametroItem.find(".component-where").html().trim().normalize("NFD").replace(/[\u0300-\u036f]/g, "").toLowerCase();
            const componentOrderBy = parametroItem.find(".component-orderBy").html().trim().normalize("NFD").replace(/[\u0300-\u036f]/g, "").toLowerCase();

            if (componentContenido.includes(cadena) || componentWhere.includes(cadena) || componentOrderBy.includes(cadena)){
                // Mostrar la fila
                parametroItem.removeClass("d-none");                
            }else{
                // Ocultar la fila
                parametroItem.addClass("d-none");
            }            
        });                     
    },
};