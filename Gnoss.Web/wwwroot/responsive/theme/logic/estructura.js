/**
 ***************************************************************************************
 * Logica / Operativas de comportamiento JS para la sección de Estrucuta de páginas de la Comunidad del DevTools
 * *************************************************************************************
 */


/**
 * Operativa de funcionamiento de Gestión de páginas de la comunidad
 */
const operativaGestionPaginas = {

    /**
     * Inicializar operativa
     */
     init: function (pParams) {
        this.pParams = pParams;
        // Permitir o no guardar datos (CI/CD -> Pasado desde la vista)
        if (this.allowSaveData == undefined){
            this.allowSaveData = pParams.allowSaveData;
        }        
        this.config(pParams);
        this.configEvents();
        this.configRutas(); 
        this.triggerEvents();                    
    },

    /**
     * Configuración de las rutas de acciones necesarias para hacer peticiones a backend
     */
    configRutas: function (pParams) {
        // Url de la página actual donde se encuentra el listado de páginas        
        this.urlPagina = refineURL();
        // Url para crear una nueva página
        this.urlAddNewPage = `${this.urlPagina}/new-tab`;
        // Url para guardar todas las páginas listadas de la sección
        this.urlSaveAllPages = `${this.urlPagina}/save`; 
        // Url para crear un nuevo filtro de ordenación
        this.urlAddNewFilter = `${this.urlPagina}/new-filtro-orden`; 
        // Url para crear una nueva exportación
        this.urlAddNewExportation = `${this.urlPagina}/new-exportation`; 
        // Url para crear una nueva propiedad de la exportación
        this.urlAddNewExportationProperty = `${this.urlPagina}/new-exportation-property`; 
        // Url para crear una nueva faceta
        this.urlAddNewFacet = `${this.urlPagina}/new-faceta`; 
        // Url para cargar las facetas existentes asociadas a una página
        this.urlLoadFacetList = `${this.urlPagina}/cargar-facetas`; 
        // Url para realizar la petición para el guardado de Páginas
        this.urlSavePages = `${this.urlPagina}/save`; 

        // Clase de cada una fila de las páginas existentes
        this.pageRowClassName = "page-row";
        // Contador de páginas
        this.numPaginas = $("#numPaginas");
        
        // Variable que contendrá la fila o página activa. Por defecto "undefined"
        this.filaPagina = this.filaPagina != undefined ? this.filaPagina : undefined;
    },     
    

    /*
     * Inicializar elementos de la vista
     * */
    config: function (pParams) {
            
        /* Idiomas Tabs */
        // Tab de idioma de la página para cambiar la visualización en el idioma deseado de las páginas
        this.tabIdiomaItem = $(".tabIdiomaItem ");
        // Cada una de las labels (Nombre y Url) en los diferentes idiomas (por `data-languageitem`) que contiene el valor en el idioma correspondiente
        this.labelLanguageComponentClassName = "language-component";

        /* Guardado de todas las páginas */
        // Botón para guardado de todas la estructura de las páginas
        this.btnGuardarEstructuraPaginas = $('#btnGuardarPaginas');
        // Botón para guardado de una página individual (Modal)
        this.btnGuardarPagina = $(".btnGuardarPagina");
        // Input para buscar páginas en el listado de páginas
        this.inputTxtBuscarPagina = $('#txtBuscarPagina');
        // Contenedor de todas las páginas de la comunidad
        this.pageListContainer = $('#id-added-pages-list');

        /* Control de comportamiento de radioButtons */
        // Abrir en nueva Ventana (si/no)
        this.radioButtonTabOpenInNewWindow = $(".TabOpenInNewWindow");
        // Página visible en el menú (si/no)
        this.radioButtonTabVisibleClassName = "TabVisible";
        this.radioButtonTabVisible = $(`.${this.radioButtonTabVisibleClassName}`);
        // Página activa/publicada (si/no)
        this.radioButtonTabActiveClassName = "TabActive";
        this.radioButtonTabActive = $(`.${this.radioButtonTabActiveClassName}`);
        // Página de tipo Home editable vía CMS
        this.radioButtonTabHomeCMS = $(".TabHomeCMS");
        // Página de tipo Home editable vía CMS. Elegir el tipo de edición CMS. (Todos los usuarios / Home distintas para miembros y no miembros)
        this.radioButtonTabTypeHomeCMS = $(".TabTypeHomeCMS");
        this.radioButtonTabTypeHomeCMSClassName = "TabTypeHomeCMS";
        // Select/Combo para editar privacidad de la página
        this.cmbEditarPrivacidad = $(".cmbEditarPrivacidad");
        // Pagina visible para usuarios sin acceso (si/no)
        this.radioButtonTabVisibleSinAcceso = $(".TabVisibleSinAcceso");
        // Checkbox tipo de vistas para los resultados en páginas 
        this.checkboxSeleccionVista = $(".checkboxSeleccionVista");

        // Clase del Span para mostrar el nombre y url de la página
        this.componentPageNameClassName = "component-name";
        this.componentUrlPageNameClassName = "component-url";
        // Estado de la página (Activa: Sí, No) 
        this.componentEstadoPageClassName = "component-estado";
        this.componentEstadoSiPageClassName = "component-estado-si";
        this.componentEstadoNoPageClassName = "component-estado-no";
        // Visibilidad de la página (Visible: Si, No)
        this.componentVisibilidadPageClassName = "component-visible";
        this.componentVisibilidadSiPageClassName = "component-visible-si";
        this.componentVisibilidadNoPageClassName = "component-visible-no";
        // Botones para añadir páginas: Tipo de páginas según su data-pagetype*/
        this.btnAddPage = $(".linkAddNewPage");
        // Botón para editar la página. Ejecutará el mostrado del modal para su edición        
        this.btnEditPage = $(".btnEditPage");
        // Botones de opción para configurar la página Home vía CMS (HomeCmsTodosLosUsarios, HomeCmsMiembros, HomeCmsNoMiembros)
        this.btnEditCmsHomeClassName = "btnEditCmsHome";
        this.btnEditCmsHomeTodosUsuariosClassName = "btnEditCmsHomeTodosUsuarios";
        this.btnEditCmsMiembrosClassName = "btnEditCmsHomeMiembros";
        this.btnEditCmsNoMiembrosClassName = "btnEditCmsHomeNoMiembros";
        this.checkboxTabHomeMiembrosCMSClassName = "TabHomeMiembrosCMS";
        this.checkboxTabHomeNoMiembrosCMSClassName = "TabHomeNoMiembrosCMS";
        // Botón para añadir el valor del input de autocomplete para filtros
        this.anadirFiltroAutocomplete = "anadirFiltroAutocomplete";
        // Botón para eliminar el filtro de páginas
        this.tagRemoveFiltro = "tag-remove-filtro";
        // Botón para añadir el filtro del input autocomplete
        this.btnAddFilterPanFilter = "btnAddFilterPanFilter" 

        // Botón para eliminar la página
        this.btnDeletePage = $(".btnDeletePage");
        // Botón para confirmar el borrado de la página (Modal)
        this.btnConfirmDeletePage = $(".btnConfirmDeletePage");
        // Botón de no confirmar el borrado de la página (Modal)
        this.btnNotConfirmDeletePage = $(".btnNotConfirmDeletePage");        

        /* Privacidad de la página */
        this.inputPrivacidadPerfiles = $('input[name="TabPrivacidadPerfiles"]');
        // Input para buscar grupos y añadirlos a modo excepción de privacidad de la página
        this.inputPrivacidadGrupos = $('input[name="TabPrivacidadGrupos"]');
        // Paneles de configuración de privacidad
        this.panelEditPrivacy = $(".edit-privacy");
        this.panelEditPrivacyVisiblesUsuariosSinAcceso = $(".edit-privacy-panel-visibles-usuarios-sin-acceso");
        this.panelEditPrivacyHtmlAlternativo = $(".edit-privacy-panel-html-alternativo");
        this.panelEditPrivacyPrivacidadPerfilYGrupos = $(".edit-privacy-panel-privacidad-perfiles-grupos");

        /* Editar página tipo Home vía CMS */
        // Paneles informativos
        // Opciones de configurar la Home para Usuarios distintos
        this.panelTabHomeCMSUsersHomeDistintas = $(".panelTabHomeCMSUsersHomeDistintas");
        // Opciones para configurar la Home para Miembros y no miembros
        this.panelTabHomeCMSMiembrosNoMiembros = $(".panelTabHomeCMSMiembrosNoMiembros"); 
        
        /* Sección Personalizar opciones de búsqueda */
        // Link para personalizar las opciones de búsqueda de una página
        this.linkEditarOpcionesBusqueda = $(".linkEditarOpcionesBusqueda");
        // Panel donde se encuentra el panel para editar las opciones de búsqueda
        this.panelEditarOpcionesDeBusqueda = $(".editarOpcionesBusqueda");        

        /* Sección editar Filtros de Orden al editar página */
        // Botón para "Crear filtros de orden personalizados" en tipo de página "Búsqueda"
        this.linkEditarFiltroOrden = $('.linkEditarFiltroOrden');
        // Panel donde se encuentran la opción de "Añadir filtro" y los demás filtros
        this.panelEditarFiltroOrden = $(".editarFiltroOrden");
        // Botón para "Añadir filtro de orden"
        this.linkAddFiltroOrden = $('.linkAddFiltroOrden');
        // Listado de filtros personalizados para una determinada página
        this.listCustomFiltersClassName = 'id-added-filter-list';
        // Cada uno de los paneles de filtros personalizados
        this.listCustomFilters = $(`.${this.listCustomFiltersClassName}`);                
        // Icono de filtro para ordenar
        this.sortableFilterIconClassName = "sortable-filter";
        // Botón para editar el filtro
        this.btnEditFilter = $(".btnEditFilter");
        // Botón para eliminar el filtro
        this.btnDeleteFilter = $(".btnDeleteFilter");
        // Input del valor del filtro
        this.inputFiltroValue = $(".inputFiltroValue"); 
        // Input del campo filtro para búsquedas semánticas
        //this.inputCampoFiltro = $(".inputCampoFiltro");

        /* Sección editar Exportacion de búsqueda al editar página */
        // Botón para "Crear exportaciones" en tipo de página "Búsqueda"
        this.linkEditarExportaciones = $('.linkEditarExportaciones');
        // Panel donde se encuentran la opción de "Añadir exportaciones"
        this.panelEditarExportaciones = $(".editarExportaciones");
        // Botón para "Añadir exportaciones"
        this.linkAddExportation = $('.linkAddExportation');
        // Listado de exportaciones para una determinada página
        this.listExportations = $('.id-added-exportation-list');
        // Botón para editar el filtro
        this.btnEditExportation = $(".btnEditExportation");
        // Botón para eliminar el filtro
        this.btnDeleteExportation = $(".btnDeleteExportation"); 
        // Input del nombre de la exportación 
        this.inputNombreExportacion = $(".inputNombreExportacion"); 
                
        /* Sección editar Propiedades de exportación de búsquedas */
        // Botón para "Añadir filtro de orden"
        this.linkAddPropertyExportation = $(".linkAddPropertyExportation");
        // Panel donde se encuentran la opción de "Añadir propiedades"
        this.panelEditarPropiedades = $(".edit-exportation-properties");
        // Listado de propiedades de una exportación de una página
        this.listExportationPropertyClassName = "id-added-exportation-property-list";
        this.listProperties = $(`.${this.listExportationPropertyClassName}`);        
        // Icono de exportaciones para ordenar
        this.sortableExportationIconClassName = "sortable-export-property";       
        // Botón para editar la propiedad de una exportación
        this.btnEditProperty = $(".btnEditProperty");
        // Botón para eliminar la propiedad de una exportación
        this.btnDeletePropertyClassName = "btnDeleteProperty";
        this.btnDeleteProperty = $(`.${this.btnDeletePropertyClassName}`); 
        // Input donde se colocará el nombr ede la propiedad
        this.inputNombrePropiedad = $(".inputNombrePropiedad"); 
                
        // Gestion de Tags (Usuarios lectores, privados...)
        this.btnRemoveTagUsuarioClassName = "tag-remove";
        // Input oculto que contendrá los perfiles elegidos para la privacidad de la página
        this.inputTabValoresPrivacidadPerfiles = $('[name="TabValoresPrivacidadPerfiles"]');      
        // Input oculto que contendrá los perfiles elegidos para la privacidad de la página
        this.inputTabValoresPrivacidadGrupos = $('[name="TabValoresPrivacidadGrupos"]'); 

        /* Sección Dashboard */
        //TFG Fran
        // Botón edición Dashboard
        this.linkEditarOpcionesDashboard = $('.linkEditarOpcionesDashboard');
        // Botón añadir Asistente
        this.linkAgregarAsistente = $('.linkAgregarAsistente');
        //Enganchar comportamiento de asistente
        this.linkCambiarOpciones = $('select[name="tGrafico"]');
        this.linkCambiarOpcionesClassName = 'cmbSelectTgrafico';

        // Botón añadir Grafico
        this.linkAnyadirGrafico = $('select[name="btnAnyadir"]');
        // Botón agregar dataset
        this.linkAnyadirDatasetClassName = 'btnAgregarDataset';
        // Botón añadir Columna
        this.linkAgregarColumnaClassName = "btnAgregarColumna";
        // Botón eliminar asiste
        this.linkEliminarAsistente = $('span.elimAsis');
        // Botón opciones labels
        this.linkOpcionesLabels = $('input[name="btnOpcionesLabels"]');
        // Botón cambiar nombre pestanya
        this.linkCambiarNombrePestanya = $('input[name="nGrafico"]');
        // Botón agregar propiedad
        this.linkAgregarPropiedad = $('input[name="btnAgregarPropiedad"]');
        // Botón opciones propiedad
        this.linkOpcionesPropiedad = $('input[name="btnOpcionesPropiedad"]');
        // Clase del botón para añadir gráfico
        this.btnAddGraphClassName = "btnAddGraph";
        // Clase para el botón de eliminar un asistente de gráfico
        this.btnDeleteGraphAssistantClassName = "btnDeleteGraphAssistant";
        // Fila de gráfico asistente 
        this.asistenteRowClassName = "asistente-row";
        // Botón para editar y mostrar el asistente para gráficos
        this.btnEditGraphAssistantClassName = "btnEditGraphAssistant";
        // Botón para editar el dataset de un gráfico
        this.btnEditDataSetGraphAssistantClassName = "btnEditDataSetGraphAssistant";
        // Botón para editar el dataset de un gráfico de tipo tabla
        this.btnEditDataSetNoAgrupacionGraphAssistantClassName = "btnEditDataSetNoAgrupacionGraphAssistant";
        // Contenedor de cada uno de los asistentes para el gráfico
        this.assistantGraphInfoPanelClassName = "assistant-graph-info";
        // Panel contenedor de los gráficos (ul -> li) para su previsualización - Sortable
        this.graphPreviewAllClassName = "graphPreviewAll";
        // Icono para la ordenación de gráficos de la página
        this.sortableGraphicIconClassName = "js-component-sortable-graph"; 
        
        //Enganchar comportamiento dataset
        // Botón opciones dataset
        this.linkOpcionesDataset = $('input[name="btnOpcionesDataset"]');
        // Botón opciones agrupacion
        this.linkOpcionesAgrupacion = $('input[name="btnOpcionesAgrupacion"]');
        // Botón agregar propiedad
        this.linkAgregarAgrupacion = $('input[name="btnAgregarAgrupacion"]');
        // Botón agregar propiedad dataset
        this.linkAgregarPropiedadDat = $('input[name="btnAgregarPropiedadDat"]');
        // Botón opciones propiedad
        this.linkCambiarNombrePestanyaDataSet = $('input[name="nombreDataset"]');
        
        /* Sección editar Facetas al editar página */
        // Botón para "Visualizar/Cargar facetas"
        this.linkEditarFacet = $('.linkEditarFacetas');
        // Panel donde se encuentran la opción de "Añadir Facetas"
        this.panelEditarFacet = $(".editarFacetas");

        // Botón para "Añadir una nueva faceta"
        this.linkAddFacet = $('.linkAddFacet');
        // Enlace para cargar las facetas existentes en la comunidad y listarlas en la sección correspondiente
        this.linkLoadFacetList = $('.linkLoadFacetList');
        // Listado de facetas para una determinada página
        this.listFacets = $('.id-added-facet-list');
        // Botón para editar la faceta
        this.btnEditFacet = $(".btnEditFacet");
        // Botón para eliminar la faceta filtro
        this.btnDeleteFacet = $(".btnDeleteFacet");
        // Select/Combobox para elegir una faceta en el momento de ser esta añadida
        this.selectListaFacetas = $(".cmbListaFacetas");
        // Select/Combobox de los objetos de conocimiento
        this.selectListaOc = ('[name="ListaOC"]');
        // Select/Combobox de los datos auxiliares para seleccionar OC y Facetas
        this.selectFacetOcAux = $(".cmbSelectAux");
        
        // Modal container dinámico
        this.modalContainer = $('#modal-container');
        // Modal de cada una de las páginas
        this.modalPageClassName = "modal-page";
        // Modal de eliminación de páginas
        this.modalDeletePageClassName = "modal-confirmDelete";

        /* Flags para controlar URL de la página */
        // Flag para detectar error en el input por ruta vacía
        this.errorRutaVacia = false;
        this.errorRutaRepetida = false;
        // Flag para detectar error por nombre vacío de páginas.
        this.errorNombreVacio = false;
        // Flag para detectar error en filtros de orden
        this.errorFiltrosOrden = false;
        // Flag para detectar error en Exportaciones
        this.errorExportaciones = false; 
        // Flag para detectar errores durante la obtención de datos de páginas             
        this.errorDuranteObtenerDatosPestaya = false;


        /* Flags para confirmar la eliminación de una página */
        this.confirmDeletePage = false;
        // Flag para indicar el item a borrar para ser eliminado una vez se guarde
        this.pendingToBeRemovedClassName = "pendingToBeRemoved";

    }, 

    /**
     * Lanzar comportamientos u operativas necesarias para el funcionamiento de la sección
     */
     triggerEvents: function(){
        const that = this;
        
        // Ejecutar comportamientos de páginas para Sortable -> Sólo si se pueden guardar datos (CI/CD)
        if (stringToBoolean(this.allowSaveData) == true){
            // operativaPaginasSortable.init(); 
            operativaPaginasNestedSortable.init();
        }

        // Inicializar comportamientos de select2
        comportamientoInicial.iniciarSelects2(); 
        
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
        
        // Setup de sortable tabla para los filtros (Dentro de cada página)
        const listCustomFilterPanels = $(`.${that.listCustomFiltersClassName}`);        
        $.map( listCustomFilterPanels, function( item ) {            
            const id = $(item).prop("id");            
            setupBasicSortableList(id, that.sortableFilterIconClassName, undefined, undefined, undefined, undefined);
        });

        // Setup de sortable tabla para las exportaciones (Dentro de cada página)          
        const listExportationProperty = $(`.${that.listExportationPropertyClassName}`);        
        $.map( listExportationProperty, function( item ) {            
            const id = $(item).prop("id");            
            setupBasicSortableList(id, that.sortableExportationIconClassName, undefined, undefined, undefined, undefined);
        });
     
        that.setupFiltersForAutocomplete();
    },     
    
    /**
     * Configuración de eventos de elementos del Dom (Botones, Inputs...)     
     */
     configEvents: function (pParams) {
        const that = this;
                       
        this.modalContainer.on('show.bs.modal', (e) => {
            // Aparición del modal
            that.triggerModalContainer = $(e.relatedTarget);
        })
        .on('hide.bs.modal', (e) => {
            // Acción de ocultar el modal
        })
        .on('hidden.bs.modal', (e) => {
            // Vaciar el modal
            resetearModalContainer();            
        });

        // Comportamientos del modal que son individuales para la edición de páginas
        $(`.${this.modalPageClassName}`).on('show.bs.modal', (e) => {
            // Aparición del modal
            that.triggerModalContainer = $(e.relatedTarget);
        })
        .on('hide.bs.modal', (e) => {
            // Acción de ocultar el modal
            // El modal que se va a ocultar/cerrar
            const currentModal = $(e.currentTarget);                                
            that.handleClosePageModal(currentModal);
        });

        // Comportamientos del modal que son individuales para el borrado de páginas
        $(`.${this.modalDeletePageClassName}`).on('show.bs.modal', (e) => {
            // Aparición del modal
        })
        .on('hide.bs.modal', (e) => {
            // Acción de ocultar el modal
            // Si el modal se cierra sin confirmar borrado, desactiva el borrado
            if (that.confirmDeletePage == false){
                that.handleSetDeletePage(false);                
            }            
        });        

        // Tab de cada uno de los idiomas disponibles para cambiar la visualización y poder ver el nombre y url del idioma elegido en la página principal
        this.tabIdiomaItem.off().on("click", function(){
            that.handleViewPageLanguageInfo();
        });


        // Botón/es para añadir un tipo de página a la comunidad
        this.btnAddPage.off().on("click", function(){
            const pageType = $(this).data("pagetype");
            const nameonto = $(this).data("nameonto");
            that.handleAddNewPage(pageType, nameonto);            
        });

        // Input para realizar búsquedas de páginas
        this.inputTxtBuscarPagina.off().on("keyup", function (event) {
            clearTimeout(that.timer);
            that.timer = setTimeout(function () {                
                that.handleSearchPageByTitle();                                                         
            }, 500);
        });


        // Botón/es para eliminar una determinada página al pulsar en el botón de papelera
        this.btnDeletePage.off().on("click", function(){            
            const btnDeleted = $(this);
            that.setBtnSaveActive();
            // Fila correspondiente a la pagina eliminada
            that.filaPagina = btnDeleted.closest('.page-row');
            //that.handleDeletePage(that.filaPagina, true);
            // Marcar la página como "Eliminar" a modo informativo al usuario
            that.handleSetDeletePage(true);
        });


        // Botón/es para confirmar la eliminación de una página (Modal -> Sí)
        this.btnConfirmDeletePage.off().on("click", function(){
            that.confirmDeletePage = true;
            that.handleDeletePage();
        });

        // Botón para editar una determinada página. Habilitará el botón de Guardar todo ya que es posible que se hagan cambios y sean necesario guardarlos
        this.btnEditPage.off().on("click", function(){
            that.setBtnSaveActive();
            // Establecer la clase de "modified" a la fila
            that.filaPagina = $(this).closest('.page-row');
            that.filaPagina.addClass("modified");
        });

        // Checkbox de selección tipo de vista para resultados. (Listado, Mosaico, Mapa...)        
        this.checkboxSeleccionVista.off().on("click", function(){
            that.checkViewResultsSelected($(this));            
        });
        
        // Opción radioButton de Pagina visible para usuarios sin acceso (si/no)
        this.radioButtonTabVisibleSinAcceso.off().on("click", function(){
            that.handleOptionOpenTabVisibleSinAcceso();
        }); 
        
        
        // Controlar visibilidad de la página
        configEventByClassName(`${that.radioButtonTabVisibleClassName}`, function(element){
            const $jqueryElement = $(element);
            $jqueryElement.off().on("click", function(){                 
                that.handleOnInputVisibilidadPageChanged($jqueryElement);
            });	                          
        });

        // Controlar el estado de la página
        configEventByClassName(`${that.radioButtonTabActiveClassName}`, function(element){
            const $jqueryElement = $(element);
            $jqueryElement.off().on("click", function(){                                                                                                         
                that.handleOnInputEstadoPageChanged($jqueryElement)
            });	                                 
        });   
        
        
        /* Eventos relativos a la Creación de filtros personalizados */
        //******************************************************
        // Botón para mostrar filtros / crear nuevos filtros siempre que no haya ninguno
        this.linkEditarFiltroOrden.off().on("click", function(){
            const btnSelected = $(this);
            that.handleShowCreateCustomFilters(btnSelected);
        });

        // Botón para eliminar un determinado filtro personalizado de una página        
        this.btnDeleteFilter.off().on("click", function(){
            const btnDeleted = $(this);
            // Fila correspondiente a la opción editada
            that.filaPagina = btnDeleted.closest('.page-row');
            that.handleDeleteCustomFilter(that.filaPagina, btnDeleted, true);            
        });

        // Botón para crear/añadir un nuevo filtro personalizado
        this.linkAddFiltroOrden.off().on("click", function(){
            const btnSelected = $(this);
            that.handleAddCustomFilter(btnSelected);
        });

        /* Eventos relativos a Personalizar opciones de búsqueda */
        //****************************************************** 
        this.linkEditarOpcionesBusqueda.off().on("click", function(){
            const btnSelected = $(this);
            that.handleShowSearchOptions(btnSelected);            
        });


        /* Eventos relativos a la Creación de Exportaciones */
        //******************************************************

        // Botón para mostrar exportaciones / crear nuevas exportaciones siempre que no haya ninguna
        this.linkEditarExportaciones.off().on("click", function(){
            const btnSelected = $(this);
            that.handleShowCreateCustomExportations(btnSelected);
        });
        
        // Botón para eliminar una determinada exportación de una página
        this.btnDeleteExportation.off().on("click", function(){
            const btnDeleted = $(this);
            // Fila correspondiente a la opción editada
            that.filaPagina = btnDeleted.closest('.page-row');
            that.handleDeleteCustomExportation(that.filaPagina, btnDeleted, true);            
        }); 
        
        // Botón para crear/añadir una nueva exportación
        this.linkAddExportation.off().on("click", function(){
            const btnSelected = $(this);
            that.handleAddCustomExportation(btnSelected);
        }); 
        
        // Botón para crear/añadir una propiedad dentro de una exportación
        this.linkAddPropertyExportation.off().on("click", function(){
            const btnSelected = $(this);
            that.handleAddPropertyExportation(btnSelected);
        });

        // Botón para eliminar una determinada propiedad de la exportación de una página
        configEventByClassName(`${that.btnDeletePropertyClassName}`, function(element){
            const deleteButton = $(element);
            if (deleteButton.length > 0) {
                // Fila del elemento a eliminar
                const pElementRow = deleteButton.closest('.property-row');
                // Pasar la función como parámetro al plugin
                const pluginOptions = {
                    onConfirmDeleteMessage: () => that.handleDeleteCustomPropertyExportation(pElementRow)
                }               
                deleteButton.confirmDeleteItemInModal(pluginOptions);
            }                         
        });         

        /* Eventos relativos a la Creación de Dashboard */
        //******************************************************

        // Botón para crear/añadir un nuevo Dashboard
        this.linkEditarOpcionesDashboard.off().on("click", function () {
            const btnSelected = $(this);
            that.handleEditDashboard(btnSelected);
        });

         // Botón añadir Asistente
         this.linkAgregarAsistente.off().on("click", function () {
             const btnSelected = $(this);
             that.handleAnyadirAsistente(btnSelected);
             // Añadir ordenación de gráficos
             that.setupBasicSortableListForGraphics();
         });
        
        // Botón agregar dataset
        configEventByClassName(`${that.linkCambiarOpcionesClassName}`, function(element){        
            const select = $(element);
            select.on("change", function () {                
                that.handleCambiarOpciones(select);                
            });                   
        });
                 
         // Botón añadir Grafico
         this.linkAnyadirGrafico.off().on("click", function () {
             const btnSelected = $(this);
             that.handleAnyadirGrafico(btnSelected);
         });
         
        // Botón agregar dataset
        configEventByClassName(`${that.linkAnyadirDatasetClassName}`, function(element){        
            const buttonAddDataSet = $(element);
            buttonAddDataSet.off().on("click", function () {
                that.handleAddDataset(buttonAddDataSet);                
            });                   
        });         

        // Botón añadir Columna
        configEventByClassName(`${that.linkAgregarColumnaClassName}`, function(element){        
            const buttonAddColumn = $(element);
            buttonAddColumn.off().on("click", function () {
                that.handleAgregarColumna(buttonAddColumn);               
            });                   
        });           
                  
         // Botón opciones labels
         this.linkOpcionesLabels.off().on("click", function () {
             const btnSelected = $(this);
             that.handleOpcionesLabels(btnSelected);
         });
         
         // Botón cambiar nombre pestanya
         this.linkCambiarNombrePestanya.off().on("keyup", function () {
             const btnSelected = $(this);
             that.cambiarNombreTituloPestanya(btnSelected);
         });
         
         // Botón agregar propiedad
         this.linkAgregarPropiedad.off().on("click", function () {
             const btnSelected = $(this);
             that.handleAgregarPropiedad(btnSelected);
         });
         
         // Botón opciones propiedad
         this.linkOpcionesPropiedad.off().on("click", function () {
             const btnSelected = $(this);
             that.handleOpcionesPropiedad(btnSelected);
         });
         
         // Botón opciones dataset
         this.linkOpcionesDataset.off().on("click", function () {
             const btnSelected = $(this);
             that.handleOpcionesDataset(btnSelected);
         });
         
         // Botón opciones agrupacion
         this.linkOpcionesAgrupacion.off().on("click", function () {
             const btnSelected = $(this);
             that.handleOpcionesAgrupacion(btnSelected);
         });
         
         // Botón agregar propiedad
         this.linkAgregarAgrupacion.off().on("click", function () {
             const btnSelected = $(this);
             that.handleAgregarAgrupacion(btnSelected);
         });
         
         // Botón agregar propiedad dataset
         this.linkAgregarPropiedadDat.off().on("click", function () {
             const btnSelected = $(this);
             that.handleAgregarPropiedadDat(btnSelected);
         });
         
         // Botón opciones propiedad
         this.linkCambiarNombrePestanyaDataSet.off().on("keyup", function () {
             const btnSelected = $(this);
             that.cambiarNombrePestanya(btnSelected);
         });

         // Botón Añadir gráfico
         configEventByClassName(`${that.btnAddGraphClassName}`, function (element) {
             const $jqueryButton = $(element);
             $jqueryButton.off().on("click", function () {
                 that.handleAnyadirGrafico($jqueryButton);                 
             });
         }); 

        // Botón de eliminar un Gráfico o asistente de gráfico de una página de búsqueda semántica
        configEventByClassName(`${that.btnDeleteGraphAssistantClassName}`, function(element){        
            const deleteButton = $(element);
            if (deleteButton.length > 0) {
                // Fila elemento que está siendo editada. Contenedor de todo                
                const pElementRow = deleteButton.closest(`.${that.asistenteRowClassName}`);
                // Pasar la función como parámetro al plugin
                const pluginOptions = {
                    onConfirmDeleteMessage: () => that.handleDeleteGraphAssistantElement(pElementRow)
                }               
                deleteButton.confirmDeleteItemInModal(pluginOptions);
            }                         
        });          

        /* Eventos relativos a la Creación de Facetas */
        //******************************************************
        
        // Botón para crear/añadir una nueva faceta
        this.linkAddFacet.off().on("click", function(){
            const btnSelected = $(this);
            that.handleAddFacet(btnSelected);
        });
        
        // Botón para la selección de faceta-objeto de conocimiento en la página
        this.selectListaFacetas.off().on("change", function(){
            const btnSelected = $(this);
            that.handleSetFacetWithOC(btnSelected);
        });

        // Botón para eliminar una faceta que ha sido previamente añadida o existe en la lista de facetas de la página
        this.btnDeleteFacet.off().on("click", function(){
            const btnDeleted = $(this);
            // Fila correspondiente a la opción eliminada
            that.filaPagina = btnDeleted.closest('.page-row');
            that.handleDeleteFacet(that.filaPagina, btnDeleted, true);              
        });

        
        /* Eventos relativos a la edición de página tipo Home */
        //******************************************************

        // Opción radioButton de página de tipo Home editable vía CMS
        this.radioButtonTabHomeCMS.off().on("click", function(){
            const radioButton = $(this);
            that.handleOptionEditarPaginaHomeCms(radioButton);
        });
        
        // Opción para elegir el tipo de Edición de página Home vía CMS
        this.radioButtonTabTypeHomeCMS.off().on("click", function(){
            const radioButton = $(this);
            that.handleOptionEditarPaginaHomeCmsMiembrosNoMiembros(radioButton);
        });
        
        
        /* Eventos relativos a la privacidad de la página */
        //**************************************************
        // Select para cambiar la privacidad de las páginas
        this.cmbEditarPrivacidad.off().on("change", function(){
            const select = $(this);
            that.handleOptionEditarPrivacidad(select);
        });

        // Botón (X) para poder eliminar usuarios/grupos desde el Tag                           
        configEventByClassName(`${that.btnRemoveTagUsuarioClassName}`, function(element){
            const $jqueryElement = $(element);
            $jqueryElement.off().on("click", function(){                 
                const $itemRemoved = $jqueryElement.parent().parent();                                                                              
                // that.handleClickBorrarGrupoTagModal($grupoEliminado);
                that.filaPagina = $itemRemoved.closest('.page-row');
                that.handleClickBorrarTagItem($itemRemoved);                
            });	                        
        });

        // Comportamiento autocomplete a input de selección de Perfiles para Privacidad de página
        this.inputPrivacidadPerfiles.autocomplete(
            null,
            {
                url: $('#inpt_urlServicioAutocompletar').val() + "/AutoCompletarLectoresEditores",
                delay: 0,
                scroll: false,
                max: 30,
                selectFirst: false,
                minChars: 1,
                width: 300,
                cacheLength: 0,
                NoPintarSeleccionado: true,
                multiple: true,
                //classTxtValoresSelecc: 'privacidadPerfiles',

                extraParams: {
                    grupo: '',
                    identidad: $('#inpt_identidadID').val(),
                    organizacion: $('#inpt_organizacionID').val() == "00000000-0000-0000-0000-000000000000" ? "" : $('#inpt_organizacionID').val(),
                    proyecto: $('#inpt_proyID').val(),
                    bool_edicion: 'true',
                    bool_traergrupos: 'false',
                    bool_traerperfiles: 'true'
                }
            }
        );

        
        /**
         * Comportamiento resultado cuando se selecciona una resultado de autocomplete de Perfiles
         */
        this.inputPrivacidadPerfiles.result(function (event, data, formatted) {
            //that.aceptarEditorSelectorUsuRec(this, data[0], data[1]);
            const dataName = data[0];
            const dataId = data[1];
            const input = $(event.currentTarget);
            that.handleSelectAutocompleteItem(input, dataName, dataId);
        });

        // Comportamiento autocomplete a input de selección de Grupos para Privacidad de página
        this.inputPrivacidadGrupos.autocomplete(
            null,
            {
                url: $('#inpt_urlServicioAutocompletar').val() + "/AutoCompletarLectoresEditores",
                delay: 0,
                scroll: false,
                max: 30,
                selectFirst: false,
                minChars: 1,
                width: 300,
                cacheLength: 0,
                NoPintarSeleccionado: true,
                multiple: true,
                //classTxtValoresSelecc: 'privacidadGrupos',

                extraParams: {
                    grupo: '',
                    identidad: $('#inpt_identidadID').val(),
                    organizacion: $('#inpt_organizacionID').val() == "00000000-0000-0000-0000-000000000000" ? "" : $('#inpt_organizacionID').val(),
                    proyecto: $('#inpt_proyID').val(),
                    bool_edicion: 'true',
                    bool_traergrupos: 'true',
                    bool_traerperfiles: 'false'
                }
            }
        );

        // Comportamiento resultado cuando se selecciona una resultado de autocomplete de Grupos
        this.inputPrivacidadGrupos.result(function (event, data, formatted) {
            //that.aceptarEditorSelectorUsuRec(this, data[0], data[1]);
            const dataName = data[0];
            const dataId = data[1];
            const input = $(event.currentTarget);
            that.handleSelectAutocompleteItem(input, dataName, dataId);
        });

        // Botón para guardar toda la información relativa a Administración de páginas
        this.btnGuardarEstructuraPaginas.off().on("click", function(){
            // Gestionar el guardado de páginas
            that.handleSavePages();          
        });   

        // Botón para guardar la información de una página
        this.btnGuardarPagina.off().on("click", function(){
            const btnGuardarPagina = $(this);
            // Fila correspondiente a la opción eliminada
            that.filaPagina = btnGuardarPagina.closest('.page-row');
            // Gestionar el guardado de páginas            
            that.handleSaveCurrentPage();                    
        });   
        
        
        // Validación para datos que no puedan estar vacíos (Nombre exportación)
        this.inputNombreExportacion.off().on("keyup", function(){
            const input = $(this);
            comprobarInputNoVacio(input, true, false, "Los nombres de las exportaciones no pueden estar vacíos.", 0);
        });
                                
        // Validación para datos que no puedan estar vacíos (Nombre propiedad)        
        this.inputNombrePropiedad.off().on("keyup", function(){
            const input = $(this);
            comprobarInputNoVacio(input, true, false, "Los campos de las propiedades no pueden estar vacíos.", 0);
        });

        // Validación de filtro para que no puedan estar vacíos (Filtro value)
        this.inputFiltroValue.off().on("keyup", function(){
            const input = $(this);
            comprobarInputNoVacio(input, true, false, "El nombre y el filtro no pueden estar vacíos.", 0);
        });

        // Checkbox para definir las páginas CMS de la home (Miembros)
        configEventByClassName(`${that.checkboxTabHomeMiembrosCMSClassName}`, function(element){
            const $checkbox = $(element);
            $checkbox.off().on("click", function(){ 
                const paginaRow = $($checkbox).closest(`.${that.pageRowClassName}`);
                that.handleOnSetHomeCmsPageType(paginaRow);
            });	                        
        });

        // Checkbox para definir las páginas CMS de la home (No Miembros)
        configEventByClassName(`${that.checkboxTabHomeNoMiembrosCMSClassName}`, function(element){
            const $checkbox = $(element);
            $checkbox.off().on("click", function(){      
                const paginaRow = $($checkbox).closest(`.${that.pageRowClassName}`);
                that.handleOnSetHomeCmsPageType(paginaRow);                
            });	                        
        });  


        // Botón (X) para poder eliminar filtros                           
        configEventByClassName(`${that.tagRemoveFiltro}`, function(element){            
            const $jqueryElement = $(element);
            $jqueryElement.off().on("click", function(){      
                const paginaRow = $($jqueryElement).closest(`.${that.pageRowClassName}`);
                const $itemRemoved = $jqueryElement.parent().parent();                                                                                                              
                that.handleDeleteFiltro($itemRemoved,paginaRow);                
            });	                        
        }); 

        //Añade el evento al input de filtros para que cree el filtro al pulsarse la tecla enter
        configEventByClassName(`${that.anadirFiltroAutocomplete}`, function(element){              
            const $input = $(element);
            $input.on("keyup", function(event){ 
                if(event.key == "Enter"){
                    event.preventDefault();
                    const paginaRow = $($input).closest(`.${that.pageRowClassName}`);                    
                    that.handleCrearFiltro($input.val(),paginaRow);
                    $input[0].value = "";
                }    
            });
        }); 
        
        
        // Botón para añadir el filtro elegido vía autocomplete        
        configEventByClassName(`${that.btnAddFilterPanFilter}`, function(element){              
            const $addButton = $(element);
            $addButton.on("click", function(event){ 
                // Input de autocompletar a partir del panel autocompletar
                const inputAutocompletar = $addButton.parent().find(`.${that.anadirFiltroAutocomplete}`);
                const paginaRow = inputAutocompletar.closest(`.${that.pageRowClassName}`);
                that.handleCrearFiltro(inputAutocompletar.val(), paginaRow);
                inputAutocompletar.val("");
            });
        });
    }, 

    /**
     ****************************************************************************************************************************
     * Acciones a realizar, funciones
     * **************************************************************************************************************************
     */ 

    /**
     * Método que se ejecuta al influir en el input del filtro. Genera una lista de valores de autocompletado
     */
    setupFiltersForAutocomplete: function () { 
        const that = this;

        const inputFiltro = $(`.${"anadirFiltroAutocomplete"}`);
       

        inputFiltro.autocomplete(
            null,
            {
                url: $('#inpt_urlServicioAutocompletar').val() + "/AutoCompletarOntologia",
                delay: 0,
                scroll: false,
                selectFirst: false,
                minChars: 1,
                width: 300,
                cacheLength: 0,
                NoPintarSeleccionado: true,
                multiple: true,
                extraParams: {
                    grupo: '',
                    identidad: $('#inpt_identidadID').val(),
                    organizacion: $('#inpt_organizacionID').val() == "00000000-0000-0000-0000-000000000000" ? "" : $('#inpt_organizacionID').val(),
                    proyecto: $('#inpt_proyID').val()
                }
            }
        );
        //Comportamiento resultado cuando se selecciona una resultado de autocomplete 
        inputFiltro.result(function (event, data, formatted) {
            // Añadir el resultado al posible valor existente del input de autocomplete             
            $(this).val($(this).val() + data[0]);               
        });        
    },

    /**
     * Método para añadir una condicion al contenedor
     * 
     */
    handleCrearFiltro: function(filtro,paginaRow){
        const that = this;
        if(filtro.length > 0){

        

        // Crear el item y añadirlo al contenedor
        let item = '';
        item += '<div class="tag" id="'+ filtro +'">';
            item += '<div class="tag-wrap">';
                item += '<span class="tag-text">' + filtro + '</span>';
                item += "<span class=\"tag-remove tag-remove-filtro material-icons remove\">close</span>";                
            item += '</div>';
        item += '</div>';        
               
        // Contenedor de items donde se añadirá el nuevo item seleccionado para su visualización
        const currentContenedorCondiciones =  paginaRow.find('#panListadoFiltros'); 
        currentContenedorCondiciones.append(item);

        // Input oculto donde se añadirá el nuevo item seleccionado

        const inputHack = paginaRow.find("#auxFiltrosSeleccionados");                
        inputHack.val(inputHack.val() + filtro + '|');

        }
    },
    

    /**
     * Método para quitar la condicion una vez se ha pulsado en (x)
     * 
     */
    handleDeleteFiltro: function(itemRemoved,paginaRow){
        const that = this;
        // Etiqueta del item eliminado        
        //const key = itemRemoved.data("key"); 
        const filtro = itemRemoved.prop("id");
        //const data = itemRemoved.find(".tag-text").html().trim();
        // Panel de objetos de conocimiento que está siendo editado
        const currentPanelObjetosConocimiento = paginaRow.find(`.${"panFiltrosAuto"}`);    
        // Input oculto donde se añadirá el nuevo item seleccionado
        const inputHack = currentPanelObjetosConocimiento.find(`.${"auxFiltrosSeleccionados"}`);              
        // Obtener los items los items que hay
        const itemsId = inputHack.val().split("|");
        // Eliminar el id seleccionado
        itemsId.splice( $.inArray(filtro, itemsId), 1 );
        // Pasarle los datos de los grupos actualizados al input hidden
        inputHack.val(itemsId.join("|")); 

        // Eliminar el tag/item del panel de tags
        itemRemoved.remove();

    },



    /**
     * Método para mostrar u ocultar los botones para permitir editar una página de tipo Home vía CMS     
     * @param {*} checkbox Checkbox seleccionado para la elección de página Home CMS 
     */
    handleOnSetHomeCmsPageType: function(paginaRow){
        const that = this;

        // Ocultar todos los botones que posibilitan la edición de la página Home vía CMS
        const btnEditCmsHomeTypeOptions = paginaRow.find($(`.${that.btnEditCmsHomeClassName}`));
        $.each(btnEditCmsHomeTypeOptions, function(){
            $(this).addClass("d-none");
        });

        // Home para todos los usuarios / Home distinta para miembros y no miembros (Unica / Distinta)                
        const typeHomeCmsValue = paginaRow.find($(`*[type="radio"].${that.radioButtonTabTypeHomeCMSClassName}:checked`)).data("value");

        if (typeHomeCmsValue == "Distinta"){
            // Home para miembros
            const isHomeCmsForMiembrosAvailable = paginaRow.find($(`.${that.checkboxTabHomeMiembrosCMSClassName}`)).is(':checked') ? true : false;
            // Home para no miembros
            const isHomeCmsForNoMiembrosAvailable = paginaRow.find($(`.${that.checkboxTabHomeNoMiembrosCMSClassName}`)).is(':checked') ? true : false;

            // Mostrar botones para la edición de la Home (Miembros)
            const btnEditCmsHomeMiembrosCMS = $(`.${that.btnEditCmsMiembrosClassName}`);
            isHomeCmsForMiembrosAvailable == true && paginaRow.find(btnEditCmsHomeMiembrosCMS).removeClass("d-none");            
            // Mostrar botones para la edición de la Home (No Miembros)
            const btnEditCmsHomeNoMiembrosCMS = $(`.${that.btnEditCmsNoMiembrosClassName}`);
            isHomeCmsForNoMiembrosAvailable == true && paginaRow.find(btnEditCmsHomeNoMiembrosCMS).removeClass("d-none");                    
        }else if (typeHomeCmsValue == "Unica"){
            const btnEditCmsHomeTodosUsuarios = $(`.${that.btnEditCmsHomeTodosUsuariosClassName}`);
            paginaRow.find(btnEditCmsHomeTodosUsuarios).removeClass("d-none");
        }
    },


    /**
     * Método que se ejecuta al cambiar el valor de un input. Esta función está enlazada desde la vista, pero enlazada desde el helpers.js ya que 
     * se utiliza para ejecutar un comportamiento sobre inputs dinámicos generados de forma dinámica mediante la operativaMultiIdioma.
     * @param {*} event 
     */
    handleOnInputTabNameChanged: function(event){
        const that = this;
        // Input que ha sido actualizado
        const input = $(event.currentTarget);
        // Idioma del input que ha sido actualizado
        const language = input.data("language");
        // Valor actual del input
        const value = input.val().trim();

        // Página a actualizar
        const pageRow = input.closest(`.${that.pageRowClassName}`);
        // Nombre a actualizar en el listado de páginas        
        let componentPageName = undefined ;
        
        if (language != undefined){
            componentPageName = pageRow.find(`.${that.componentPageNameClassName}`).filter(`[data-languageitem='${language}']`);
        }else{
            componentPageName = pageRow.find(`.${that.componentPageNameClassName}`);
        }
                
        // Actualizar el contenido de la faceta
        componentPageName.html(value);
    },

    /**
     * Método que se ejecuta al cambiar el valor de un input. Esta función está enlazada desde la vista, pero enlazada desde el helpers.js ya que 
     * se utiliza para ejecutar un comportamiento sobre inputs dinámicos generados de forma dinámica mediante la operativaMultiIdioma.
     * @param {*} event 
     */
    handleOnInputTabUrlChanged: function(event){
        const that = this;
        // Input que ha sido actualizado
        const input = $(event.currentTarget);
        // Idioma del input que ha sido actualizado
        const language = input.data("language");
        // Valor actual del input
        const value = input.val().trim();

        // Página a actualizar
        const pageRow = input.closest(`.${that.pageRowClassName}`);
        // Nombre a actualizar en el listado de páginas        
        let componentUrlPageName = undefined ;
        
        if (language != undefined){
            componentUrlPageName = pageRow.find(`.${that.componentUrlPageNameClassName}`).filter(`[data-languageitem='${language}']`);
        }else{
            componentUrlPageName = pageRow.find(`.${that.componentUrlPageNameClassName}`);
        }
                
        // Actualizar el contenido de la faceta
        componentUrlPageName.html(value);
    }, 
    
    
    /**
     * Método para actualizar el estado de la página al cambiar el estado de visible o no de la página (Visible Sí, No)
     * @param {bool} isActive Indica si la página se ha establecido como Activa o no.
     */
    handleOnInputVisibilidadPageChanged: function(input){
        const that = this;        
        // Página a actualizar
        const pageRow = input.closest(`.${that.pageRowClassName}`);
        // Labels asociadas a la página de la lista de páginas
        const visibilityTrueLabel = pageRow.find(`.${that.componentVisibilidadSiPageClassName}`).first();
        const visibilityFalseLabel = pageRow.find(`.${that.componentVisibilidadNoPageClassName}`).first();
        // Obtener Sí o No (del elemento clickado)
        const inputValue = input.data("value");

        if (inputValue == "si"){
            // Mostrar la etiqueta de visibility "Sí"
            visibilityTrueLabel.removeClass("d-none");
            visibilityFalseLabel.addClass("d-none");
        }else{
            visibilityTrueLabel.addClass("d-none");
            visibilityFalseLabel.removeClass("d-none");
        }
    },

    /**
     * Método para actualizar el estado de la página al cambiar el estado de la página (Activa, No Activa o publicada)
     * @param {bool} isVisible Indica si la página se ha establecido como Visible o no.
     */
    handleOnInputEstadoPageChanged: function(input){
        const that = this;

        // Página a actualizar
        const pageRow = input.closest(`.${that.pageRowClassName}`);
        // Labels asociadas a la página de la lista de páginas
        const estadoTrueLabel = pageRow.find(`.${that.componentEstadoSiPageClassName}`).first();
        const estadoFalseLabel = pageRow.find(`.${that.componentEstadoNoPageClassName}`).first();
        // Obtener Sí o No (del elemento clickado)
        const inputValue = input.data("value");

        if (inputValue == "si"){
            // Mostrar la etiqueta de visibility "Sí"
            estadoTrueLabel.removeClass("d-none");
            estadoFalseLabel.addClass("d-none");
        }else{
            estadoTrueLabel.addClass("d-none");
            estadoFalseLabel.removeClass("d-none");
        }
    },    

    /**
     * Método que se ejecutará cuando se cierre el modal de detalles de una página
     * @param {jqueryObject} currentModal Modal que se va a cerrar
     */
    handleClosePageModal: function(currentModal){
        const that = this;

        // Hacer scroll top cuando se cierre el modal
        scrollInModalViewToTop(currentModal);
        // Colapsar todos los menús desplegables del modal para que no se queden abiertos
        const collapsePanels = currentModal.find('.collapse');
        collapsePanels.collapse("hide");
        // Quitar la opción de guardar cambios si no se pulsa en "Guardar". Si se pulsa en guardar se procederá al guardado
        that.filaPagina.removeClass("modified");

        // Tener en cuenta si la página es de reciente creación y por tanto no se desea guardar
        if (that.filaPagina.hasClass("newPage")){
            // Eliminar la fila que se acaba de eliminar                                
            that.filaPagina.remove();
            // Actualizar el contador de nº de páginas            
            that.updateNumPages();
        }
    },
    
    /**
     * Método para poder activar el botón de "Guardar". Por defecto estaba "disabled". Se activará cuando se cree una nueva página o se edita una ya existente.
     */
    setBtnSaveActive: function(){        
        this.btnGuardarEstructuraPaginas.prop("disabled", false);
    },

    /**
     * Método para realizar búsquedas de páginas
     */
    handleSearchPageByTitle: function(){
        const that = this;
        let cadena = this.inputTxtBuscarPagina.val();
        // Eliminamos posibles tildes para búsqueda ampliada
        cadena = cadena.normalize('NFD').replace(/[\u0300-\u036f]/g, "").toLowerCase();
        
        // Cada una de las filas que muestran la página
        const rowPages = $("#id-added-pages-list").find(".component-wrap.page-row");        

        // Buscar dentro de cada fila       
        $.each(rowPages, function(index){
            const rowPage = $(this);
            // Seleccionamos el nombre de la página y quitamos caracteres extraños, tiles para poder hacer bien la búsqueda
            const pageName = $(this).find(".component-name").not('.d-none').html().trim().normalize("NFD").replace(/[\u0300-\u036f]/g, "").toLowerCase();
            const urlPage = $(this).find(".component-url").not('.d-none').html().trim().normalize("NFD").replace(/[\u0300-\u036f]/g, "").toLowerCase();
            if (pageName.includes(cadena) || urlPage.includes(cadena)){
                // Mostrar fila resultado y sus respectivos padres
                rowPage.removeClass("d-none");
                rowPage.parents("li.component-wrap").removeClass("d-none");
            }else{
                // Ocultar fila resultado
                rowPage.addClass("d-none");
            }            
        });
    },

    /**
     * Método para poder cambiar entre idiomas y poder visualizar los datos de las páginas en el idioma deseado sin tener que acceder al modal de cada página.
     * Gestionará el click en el tab de idiomas principal de la página
     */
    handleViewPageLanguageInfo: function(){
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
     * Método para añadir una nueva página pulsando en el botón de "+Nueva Página" y una de las opciones disponibles
     * @param {short} pageType Tipo de página que se desea añadir. Dependiendo del tipo, se cargará un modal u otro
     */
    handleAddNewPage: function(pageType, nameonto){
        const that = this;

        // Evitar posibles errores si no hay tipo de página elegida
        if (pageType != undefined){
            // Mostrar loading hasta que finalice la petición para crear una nueva página
            loadingMostrar();    
            // Construir el objeto/modelo para petición del tipo de página a crear
            const dataPost = {
                TipoPestanya: pageType,
                nameonto: nameonto
            }

            // Realizar la petición de la página a crear
            GnossPeticionAjax(                
                that.urlAddNewPage,
                dataPost,
                true
            ).done(function (data) {
                // Devuelve la fila/row del tipo de página creada
                const lastPage = that.pageListContainer.children().last();
                // Panel de edición de la última página
                const editionLastPagePanel = lastPage.children(".modal-con-tabs");
                // Número identificativo del orden de la última página
                // const orden = editionLastPagePanel.find('[name="TabOrden"]').val();                
                let orden = -Infinity;            
                $(".page-row").each(function() {
                    orden = Math.max(orden, parseInt($(this).find('[name="TabOrden"]').val()));
                });
                
                // Añadir la nueva fila al listado de tablas
                that.pageListContainer.append(data);
                // Referencia a la nueva página añadida
                const newPage = that.pageListContainer.children().last();
                // Panel de edición de la nueva página creada
                const editionNewPagePanel = newPage.children(".modal-con-tabs");
                // Establecerle el nuevo orden a la nueva página
                editionNewPagePanel.find('[name="TabOrden"]').val(parseInt(orden) + 1);
                    
                // Reiniciar la operativa de gestión Páginas para los nuevos items
                operativaGestionPaginas.init()                

                // Abrir el modal para poder editar/gestionar la nueva página añadida                     
                newPage.find(that.btnEditPage).trigger("click");
    
                // Quitar opciones de creación de página si se encuentran entre la 8 y 11 y que sea distinto de la 16
                if (pageType < 8 || pageType > 11 && pageType != 16) {                                          
                    $(`[data-pagetype=${pageType}]`).parent().remove();
                }                  
                // Habilitar el botón de Guardar páginas ya que se ha añadido una nueva página.
                that.setBtnSaveActive();                
                // Aumentar el nº del contador de páginas
                that.updateNumPages()                
                
            }).fail(function (data) {
                // Mostrar error al tratar de crear una nueva página
                mostrarNotificacion("error", data);                
            }).always(function () {
                // Ocultar loading de la petición
                loadingOcultar();
            });            
        }
    },


    /**
     * Método para marcar una página como "Eliminada" o no Eliminada. 
     * Si se decide eliminar, no se elimina por completo hasta que se guarda la sección de páginas.
     * @param {*} filaPagina : Fila de la página que se deseará eliminar o revertir su eliminación
     * @param {bool} deletePage : Valor que indica si se desea borrar la página o no
     */
    /*handleDeletePage: function(filaPagina ,deletePage){
        const that = this;
        // Botón de eliminación y edición de la página actual                
        const btnEliminarCurrentPage = that.filaPagina.find(that.btnDeletePage);
        const btnEditCurrentPage = that.filaPagina.find(that.btnEditPage);
        let btnRevertirPaginaEliminada = that.filaPagina.find(".btnRevertDeletePage");
        // Area donde están los botones de acción (Editar, Eliminar, Revertir)
        const areaComponentActions = that.filaPagina.find(".component-actions");

        if (deletePage){
            // Realizar el "borrado" de la página
            // 1 - Marcar la opción "TabEliminada" a true
            that.filaPagina.find('[name="TabEliminada"]').val("true");
            // 2 - Crear y añadir un botón de revertir el borrado de la página
            btnRevertirPaginaEliminada = `
            <li>
                <a class="action-delete round-icon-button js-action-delete btnRevertDeletePage" href="javascript: void(0);">
                    <span class="material-icons">restore_from_trash</span>
                </a>
            </li>
            `;
            // 3 - Ocultar los botones de editar y eliminar la página actual
            btnEliminarCurrentPage.addClass("d-none");
            // Dejarlo como disabled
            btnEditCurrentPage.addClass("inactiveLink");
            // 4 - Añadir el botón de revertir
            areaComponentActions.append(btnRevertirPaginaEliminada);            
            // 5- Añadir comportamiento de revertir la página            
            that.filaPagina.find(".btnRevertDeletePage").off().on("click", function(){
                that.handleDeletePage(filaPagina, false);
            });
            // 6- Añadir la clase de "deleted" a la fila de la página
            that.filaPagina.addClass("deleted");


        }else{
            // Revertir el "borrado" de la página
            // 1 - Marcar la opción "TabEliminada" a false
            that.filaPagina.find('[name="TabEliminada"]').val("false");
            // 2 - Eliminar el botón de revertir la página
            btnRevertirPaginaEliminada.remove();

            // 3 - Mostrar de nuevo los botones de editar y eliminar la página actual
            btnEliminarCurrentPage.removeClass("d-none");
            btnEditCurrentPage.removeClass("inactiveLink");    
            // 4 - Quitar la clase de "deleted" a la fila de la página
            that.filaPagina.removeClass("deleted");
        }
    },*/


    /**
     * Método para marcar o desmarcar la página como "Eliminada" dependiendo de la elección vía Modal
     * @param {Bool} Valor que indicará si se desea eliminar o no la página
     */
    handleSetDeletePage: function(deletePage){
        const that = this;

        if (deletePage){
            // Realizar el "borrado" de la página
            // 1 - Marcar la opción "TabEliminada" a true
            that.filaPagina.find('[name="TabEliminada"]').val("true");           
            // 2 - Añadir la clase de "deleted" a la fila de la página
            that.filaPagina.addClass("deleted");
        }else{
            // 1 - Marcar la opción "TabEliminada" a false
            that.filaPagina.find('[name="TabEliminada"]').val("false");            
            // 2 - Eliminar la clase de "deleted" a la fila de la página
            that.filaPagina.removeClass("deleted");
        }
    },  
    
    /**
     * Método para eliminar una página previa confirmación realizada desde el modal     
     */
     handleDeletePage: function(){
        const that = this;                  
        // 1 - Llamar al método para el guardado de páginas        
        that.handleSavePages(function(error){
            if (error == false){
                // Resetear el estado de borrado de la página
                that.confirmDeletePage = false;                
                // 2 - Ocultar el modal de eliminación de la página
                //const modalDeletePage = $(that.filaPagina).find(`.${that.modalDeletePageClassName}`);                                          
                const modalDeletePage = $(`.${that.modalDeletePageClassName}`);                                          
                dismissVistaModal(modalDeletePage);
                // 3 - Ocultar loading
                loadingOcultar();                
                // 4 - Ocultar la fila que se desea eliminar
                that.filaPagina.addClass("d-none");
                // 5 - Eliminar la fila que se acaba de eliminar                                
                that.filaPagina.remove();
                // 6 - Actualizar el contador de nº de páginas            
                that.updateNumPages(); 
            }else{
                mostrarNotificacion("error", "Se ha producido un error al tratar de eliminar la página. Contacta con el administrador de la comunidad");
            } 
        });  
      
    }, 
    
    /**
     * Método para actualizar el nº de páginas existentes (Añadir, Eliminar...) 
     * @param {bool} added : Booleano que indicará si se añade una página. En caso contrario se restará.
     */
    updateNumPages: function(){
        const that = this;

        const pageNumber = this.pageListContainer.find($(`.${that.pageRowClassName}`)).length;
        // Mostrar el nº de items                
        that.numPaginas.html(pageNumber);

        // No mostrar el item si no hay resultados
        pageNumber == 0 ? that.numPaginas.addClass("d-none") : that.numPaginas.removeClass("d-none");
    },

    /**
     * Método para comprobar la vista seleccionada en páginas de tipo Búsqueda.
     * Comprobará que no quede la página sin niguna vista seleccionada.
     * @param {jqueryObject} vistaSeleccionada : Objeto jquery que corresponde con el checkbox de la vista que se ha seleccionado
     */
    checkViewResultsSelected: function (vistaSeleccionada) {        
        const that = this;
        // Input de las vistas disponibles para la página
        const inputVistas = vistaSeleccionada.closest('.panelVistasDisponibles').find('input[type="checkbox"]');
        // RadioButton para establecer la vista por defecto
        const inputRadio = vistaSeleccionada.parent().parent().find("input[type=radio]");

        // Contador del nº de vistasque hay activas
        let contSeleccionadas = 0;
        inputVistas.each(function () {
            if ($(this).is(':checked')) {
                if (inputRadio.is(':checked')) {
                    var radioCheck = $(this).parent().parent().find("input[type=radio]");
                    radioCheck.prop('checked', 'true');
                }
                contSeleccionadas++;
            }
        });

        // Todas las vistas deseleccionadas
        if (contSeleccionadas > 0) {
            if (vistaSeleccionada.is(':checked')) {                
                inputRadio.prop("disabled", false);
            }
            else {
                inputRadio.prop("disabled", true);
            }
        }
        else {
            vistaSeleccionada.prop('checked', true);
            if (contSeleccionadas == 1) {
                inputRadio.prop("disabled", false);
            }
        }
        // Mostrar la vista del mapa si se ha pulsado sobre ella
        if (vistaSeleccionada.hasClass('mapa')) {
            that.showMapViewInfo(vistaSeleccionada);
        }
    },

    /**
     * Método para mostrar/ocultar la vista con los detalles o información relativos a la vista de tipo "Mapa" 
     * al pulsar el checkbox de "Vistas de resultados" de una página
     * @param {jqueryObject} vistaSeleccionada 
     */
    showMapViewInfo: function (vistaSeleccionada) {
        const bloqueVistaMapa = vistaSeleccionada.parent().parent().parent().find('.panelVistaMapa');
        if (vistaSeleccionada.is(':checked')) {
            bloqueVistaMapa.removeClass("d-none");
        }
        else {            
            bloqueVistaMapa.addClass("d-none");
        }
    },

    /* Comportamiento de result para AutoComplete (Selección de items de resultados autocomplete) */    
    // ******************************************************
    
    /**
     * Método para la selección realizada de un item haciendo uso de AutoComplete 
     * @param {string} input: Input que ha lanzado el autocomplete
     * @param {string} dataName: Nombre o dato nombre del usuario seleccionado del panel autoComplete
     * @param {string} dataId : Dato Id correspondiente al item seleccionado dle panel autoComplete
     */
    handleSelectAutocompleteItem: function(input, dataName, dataId){
        // Panel/Sección donde se ha realizado toda la operativa
        const tagsSection = $(input).parent().parent();

        // Contenedor de items donde se añadirá el nuevo seleccionado para su visualización
        const tagContainer = tagsSection.find(".tag-list");
        // Input oculto donde se añadirá el nuevo item seleccionado
        const inputHack = tagsSection.find("input[type=hidden]").first();
        // Añadido el id del item seleccionado al inputHack                
        inputHack.val(inputHack.val() + dataId + ',');
    
        // Etiqueta del item seleccionado
        let editorSeleccionadoHtml = '';
        editorSeleccionadoHtml += '<div class="tag" id="'+ dataId +'">';
            editorSeleccionadoHtml += '<div class="tag-wrap">';
                editorSeleccionadoHtml += '<span class="tag-text">' + dataName + '</span>';
                editorSeleccionadoHtml += "<span class=\"tag-remove material-icons\">close</span>";
                editorSeleccionadoHtml += '<input type="hidden" value="'+ dataId +'" />';
            editorSeleccionadoHtml += '</div>';
        editorSeleccionadoHtml += '</div>';

        // Añadir el item en el contenedor de items para su visualización
        tagContainer.append(editorSeleccionadoHtml);

        // Vaciar el input donde se ha escrito 
        $(input).val('');
    },
   
    /* Comportamiento de los Tags (Usuarios / Grupos de privacidad de una página) */    
    // ******************************************************

    /**
     * Método para gestionar el borrado de un item de tipo Tag cuando se pulsa en el botón (x). Se utilizará para el borrado de items de perfiles o grupos
     * de privacidad de una página
     * Borrará el item de la pantalla y eliminará el item del input oculto (inputHack) que es el que recoge todos los valores seleccionados
     * @param {jqueryObject} itemDeleted 
     */
     handleClickBorrarTagItem: function(itemDeleted){
        const that = this;

        // Panel o sección donde se encuentra el panel de Tags a Eliminar (input_hack)
        const panelTagItem = itemDeleted.parent().parent();

        // Buscar el input oculto y seleccionar la propiedad del id que corresponde con el grupo a eliminar
        const idItemDeleted = itemDeleted.prop("id"); 
        // Items id dependiendo del tipo a borrar (Perfil, Grupo)
        let itemsId = "";
        // Input del que habrá que eliminar el item seleccionado (Perfil, Grupo)
        let $inputHack = undefined;   

        if (idItemDeleted.includes("g_")){
            // Borrar un grupo en la sección de Grupos correspondiente            
            $inputHack = panelTagItem.find(that.inputTabValoresPrivacidadGrupos);                  
        }else{
            // Borrar un perfil en la sección de perfil correspondiente                       
            $inputHack = panelTagItem.find(that.inputTabValoresPrivacidadPerfiles);
        }

        itemsId = that.filaPagina.find($inputHack).val().split(",");
        itemsId.splice( $.inArray(idItemDeleted, itemsId), 1 );
        // Pasarle los datos de los grupos actuales al input hidden
        that.filaPagina.find($inputHack).val(itemsId.join(",")); 
        // Eliminar el grupo del contenedor visual
        itemDeleted.remove();
     },
    


    /* Comportamientos sección Privacidad de la página */
    // ******************************************************
    /**
     * Método para ocultar o visualizar los paneles relativos a la privacidad de una página. Se activa cuando cambia el estado del "Select"
     * de "Privacidad" de una página
     * @param {*} select Select/comboBox que ha activado este comportamiento
     */
    handleOptionEditarPrivacidad: function(select){
        const that = this;
        // Fila correspondiente a la opción editada
        that.filaPagina = select.closest('.page-row');

        // Ocultar todos los paneles para mostrar luego el deseado
        that.filaPagina.find(that.panelEditPrivacy).addClass("d-none");        
        that.filaPagina.find(that.panelEditPrivacyVisiblesUsuariosSinAcceso).addClass("d-none");
        that.filaPagina.find(that.panelEditPrivacyHtmlAlternativo).addClass("d-none");
        that.filaPagina.find(that.panelEditPrivacyPrivacidadPerfilYGrupos).addClass("d-none");
        
        // Controlar la aparición del panel deseado según la privacidad elegida
        if (select.val() > 0) {
            if (select.val() == 1){
                // Privacidad Privada
                that.filaPagina.find(that.panelEditPrivacy).removeClass("d-none"); 
                that.filaPagina.find(that.panelEditPrivacyVisiblesUsuariosSinAcceso).removeClass("d-none");
                that.filaPagina.find(that.panelEditPrivacyHtmlAlternativo).removeClass("d-none");
            }else if(select.val() == 2){
                // Privacidad Lectores
                that.filaPagina.find(that.panelEditPrivacy).removeClass("d-none");
                that.filaPagina.find(that.panelEditPrivacyVisiblesUsuariosSinAcceso).removeClass("d-none");
                that.filaPagina.find(that.panelEditPrivacyHtmlAlternativo).removeClass("d-none");
                that.filaPagina.find(this.panelEditPrivacyPrivacidadPerfilYGrupos).removeClass("d-none");                
            }
        }
    },


    /* Comportamientos sección creación de filtros personalizados */
    // ******************************************************
    /**
     * Método para visualizar la sección de crear filtros personalizados y ocultar este mismo enlace que lo ha activado
     * @param {*} button Botón o enlace que ha sido pulsado para visualizar la sección de Filtros personalizados
     */
    handleShowCreateCustomFilters: function(button){
        const that = this;
        // Encontrar la fila/página seleccionada
        that.filaPagina = button.closest('.page-row');        
        // Panel listado de filtros personalizados a mostrar ya que ha sido pulsado la creación de filtros -> Mostrarlo
        const currentPanelEditarFiltroOrden = that.filaPagina.find(that.panelEditarFiltroOrden);
        currentPanelEditarFiltroOrden.removeClass("d-none");
        // Ocultar / destruir el propio botón de visualizar filtros
        button.remove();
    },

    /**
     * Método para crear un nuevo filtro personalizado. Añadirá el row del filtro en el panel correspondiente de la página editada / seleccionada
     * @param {*} button 
     */
    handleAddCustomFilter: function(button){
        
        const that = this;
        // Encontrar la fila/página seleccionada
        that.filaPagina = button.closest('.page-row');        
        // Panel listado de filtros personalizados
        const currentListCustomFilters = that.filaPagina.find(that.listCustomFilters);
        
        // Mostrar loading para petición
        loadingMostrar();

        // Realizar la petición de nuevo filtro
        GnossPeticionAjax(
            that.urlAddNewFilter,
           null,
           true
       ).done(function (data) {
            // OK - Añadir filtro
            // Añadir el nuevo filtro creado al listado de filtros
            currentListCustomFilters.append(data); 
            // Obtener el último filtro añadido
            const newFilterAdded = currentListCustomFilters.children().last();                   
            // Reiniciar la operativa de gestión Páginas para los nuevos items
            operativaGestionPaginas.init();
            // Mostrar los detalles del nuevo filtro creado
            newFilterAdded.find(that.btnEditFilter).trigger("click");            
       }).fail(function (data) {
            // KO en creación del filtro           
           mostrarNotificacion("error", data);
       }).always(function () {
            // Ocultar loading
            loadingOcultar();           
       });
    },

    /**
     * Método para marcar un filtro personalizado como "Eliminado". 
     * Si se decide eliminar, no se elimina por completo hasta que se guarda la sección de páginas.
     * @param {*} filaPagina : Fila de la página que se deseará eliminar o revertir su eliminación 
     * @param {*} btnDelted : Botón para borrar o revertir el borrado del filtro
     * @param {bool} deleteFilter : Valor booleano de si se desea eliminar el filtro
     */
     handleDeleteCustomFilter: function(filaPagina, btnDeleted, deleteFilter){
        const that = this;
        
        // Fila de la página que está siendo seleccionada        
        const filaFiltro = btnDeleted.closest('.filter-row');
        
        // Botón de eliminación y edición de la página actual
        const btnEliminarCurrentFilterPage = filaFiltro.find(that.btnDeleteFilter);
        const btnEditCurrentFilterPage = filaFiltro.find(that.btnEditFilter);
        let btnRevertirFiltroPaginaEliminada = filaFiltro.find(".btnRevertDeleteFilterPage");
        // Area donde están los botones de acción (Editar, Eliminar, Revertir)
        const areaComponentActions = filaFiltro.find(".component-actions");
        
        if (deleteFilter){
            // Realizar el "borrado" del filtro
            // 1 - Marcar la opción "TabEliminada" a true           
            filaFiltro.find('[name="TabEliminada"]').val("true");                                   
            // 2 - Crear y añadir un botón de revertir el borrado de la página
            btnRevertirFiltroPaginaEliminada = `
            <li>
                <a class="action-delete round-icon-button js-action-delete btnRevertDeleteFilterPage" href="javascript: void(0);">
                    <span class="material-icons">restore_from_trash</span>
                </a>
            </li>
            `;
            // 3 - Ocultar los botones de editar y eliminar la página actual
            btnEliminarCurrentFilterPage.addClass("d-none");
            // Dejarlo como disabled
            btnEditCurrentFilterPage.addClass("inactiveLink");
            // 4 - Añadir el botón de revertir
            areaComponentActions.append(btnRevertirFiltroPaginaEliminada);            
            // 5- Añadir comportamiento de revertir la página            
            filaFiltro.find(".btnRevertDeleteFilterPage").off().on("click", function(){
                that.handleDeleteCustomFilter(filaPagina, $(this), false);
            });
            // 6- Añadir la clase de "deleted" a la fila de la página            
            filaFiltro.addClass("deleted");
            
            // 7- Colapsar el panel si se desea eliminar y este está abierto
            const collapseId = filaFiltro.data("collapseid");
            $(`#${collapseId}`).removeClass("show");
        }else{
            // Revertir el "borrado" de la página
            // 1 - Marcar la opción "TabEliminada" a false            
            filaFiltro.find('[name="TabEliminada"]').val("false");  
            // 2 - Eliminar el botón de revertir la filtro de la página
            btnRevertirFiltroPaginaEliminada.remove();
            // 3 - Mostrar de nuevo los botones de editar y eliminar la página actual
            btnEliminarCurrentFilterPage.removeClass("d-none");
            btnEditCurrentFilterPage.removeClass("inactiveLink");    
            // 4 - Quitar la clase de "deleted" a la fila de la página            
            filaFiltro.removeClass("deleted");
        }
    },  

    /* Comportamientos sección personalizar opciones de búsqueda */
    // ************************************************************
    
    /**
     * Método para visualizar la sección de "Personalizar opciones de búsqueda" y ocultar este mismo enlace que lo ha activado
     * @param {} button : Botón o enlace que ha sido pulsado para visualizar las opciones de búsqueda de la página
     */
    handleShowSearchOptions: function(button){        
        const that = this;
        // Encontrar la fila/página seleccionada
        that.filaPagina = button.closest('.page-row');        
        // Panel listado de personalizar opciones de búsqueda
        const currentPanelEditarOpcionesBusqueda = that.filaPagina.find(that.panelEditarOpcionesDeBusqueda);        
        // Ocultar / destruir el propio botón de visualizar exportaciones
        currentPanelEditarOpcionesBusqueda.removeClass("d-none");        
        button.remove();        
    },


    
    
    /* Comportamientos sección creación de exportaciones personalizadas */
    // ******************************************************

    /**
     * Método para visualizar la sección de crear exportaciones personalizadas y ocultar este mismo enlace que lo ha activado
     * @param {*} button : Botón o enlace que ha sido pulsado para visualizar la sección de Filtros personalizados
     */
     handleShowCreateCustomExportations: function(button){
        const that = this;
        // Encontrar la fila/página seleccionada
        that.filaPagina = button.closest('.page-row');        
        // Panel listado de exportaciones personalizados a mostrar ya que ha sido pulsado la creación de exportaciones -> Mostrarlo
        const currentPanelEditarExportation = that.filaPagina.find(that.panelEditarExportaciones);        
        // Ocultar / destruir el propio botón de visualizar exportaciones
        currentPanelEditarExportation.removeClass("d-none");        
        button.remove();
    },

    /**
     * Método para marcar una exportación como "Eliminado". 
     * Si se decide eliminar, no se elimina por completo hasta que se guarda la sección de páginas.
     * @param {*} filaPagina : Fila de la página que se deseará eliminar o revertir su eliminación 
     * @param {*} btnDeleted : Botón para borrar o revertir el borrado de la exportación
     * @param {bool} deleteExportation : Valor booleano de si se desea eliminar la exportación
     */
    handleDeleteCustomExportation: function(filaPagina, btnDeleted, deleteExportation){
       const that = this;
       
       // Fila de la página que está siendo seleccionada        
       const filaExportation = btnDeleted.closest('.exportation-row');
       
       // Botón de eliminación y edición de la página actual
       const btnEliminarCurrentExportationPage = filaExportation.find(that.btnDeleteExportation);
       const btnEditCurrentExportationPage = filaExportation.find(that.btnEditExportation);
       let btnRevertDeleteExportationPage = filaExportation.find(".btnRevertDeleteExportationPage");
       // Area donde están los botones de acción (Editar, Eliminar, Revertir)
       const areaComponentActions = filaExportation.find(".component-actions");
       
       if (deleteExportation){
           // Realizar el "borrado" del filtro
           // 1 - Marcar la opción "TabEliminada" a true           
           filaExportation.find('[name="TabEliminada"]').val("true");                                   
           // 2 - Crear y añadir un botón de revertir el borrado de la página
           btnRevertDeleteExportationPage = `
           <li>
               <a class="action-delete round-icon-button js-action-delete btnRevertDeleteExportationPage" href="javascript: void(0);">
                   <span class="material-icons">restore_from_trash</span>
               </a>
           </li>
           `;
           // 3 - Ocultar los botones de editar y eliminar la página actual
           btnEliminarCurrentExportationPage.addClass("d-none");
           // Dejarlo como disabled
           btnEditCurrentExportationPage.addClass("inactiveLink");
           // 4 - Añadir el botón de revertir
           areaComponentActions.append(btnRevertDeleteExportationPage);            
           // 5- Añadir comportamiento de revertir la página            
           filaExportation.find(".btnRevertDeleteExportationPage").off().on("click", function(){
               that.handleDeleteCustomExportation(filaPagina, $(this), false,);
           });
           // 6- Añadir la clase de "deleted" a la fila de la página            
           filaExportation.addClass("deleted");
           
           // 7- Colapsar el panel si se desea eliminar y este está abierto
           const collapseId = filaExportation.data("collapseid");
           $(`#${collapseId}`).removeClass("show");
       }else{
           // Revertir el "borrado" de la página
           // 1 - Marcar la opción "TabEliminada" a false            
           filaExportation.find('[name="TabEliminada"]').val("false");  
           // 2 - Eliminar el botón de revertir la filtro de la página
           btnRevertDeleteExportationPage.remove();
           // 3 - Mostrar de nuevo los botones de editar y eliminar la página actual
           btnEliminarCurrentExportationPage.removeClass("d-none");
           btnEditCurrentExportationPage.removeClass("inactiveLink");    
           // 4 - Quitar la clase de "deleted" a la fila de la página            
           filaExportation.removeClass("deleted");
       }
    }, 

    /**
     * Método para crear una nueva exportación. Añadirá el row del filtro en el panel correspondiente de la página editada / seleccionada
     * @param {*} button Botón que se ha pulsado para añadir la exportación
     */
     handleAddCustomExportation: function(button){
        
        const that = this;
        // Encontrar la fila/página seleccionada
        // that.filaPagina = button.closest('.page-row');        
        // Panel listado de filtros personalizados
        const currentListCustomExportations = that.filaPagina.find(that.listExportations);
        
        // Mostrar loading para petición
        loadingMostrar();

        // Realizar la petición de nueva exportación
        GnossPeticionAjax(
            that.urlAddNewExportation,
           null,
           true
       ).done(function (data) {
            // OK - Añadir exportación
            // Añadir la nueva exportación
            currentListCustomExportations.append(data); 
            // Obtener el último filtro añadido
            const newExportationAdded = currentListCustomExportations.children().last();                   
            // Reiniciar la operativa de gestión Páginas para los nuevos items
            operativaGestionPaginas.init();
            // Mostrar los detalles de la nueva exportación
            newExportationAdded.find(that.btnEditExportation).trigger("click");            
       }).fail(function (data) {
            // KO en creación del filtro           
           mostrarNotificacion("error", data);
       }).always(function () {
            // Ocultar loading
            loadingOcultar();           
       });
    },   
    
    
    /* Comportamientos sección propiedades de exportaciones */
    // ******************************************************

    /**
     * Método para marcar una propiedad de una exportación como "Eliminada". 
     * Si se decide eliminar, no se elimina por completo hasta que se guarda la sección de páginas.
     * @param {*} pElementRow : Fila de la página que se deseará eliminar o revertir su eliminación 
     */
     handleDeleteCustomPropertyExportation: function(pElementRow){
        const that = this;                  
        // Eliminar la fila sin petición            
        pElementRow.fadeOut(300, function() { 
            // Ocultar la vista a eliminar y asignarle la propiedad de "deleted"
            $(this).find('[name="TabEliminada"]').val("true");                       
            $(this).addClass(`d-none ${that.pendingToBeRemovedClassName}`);
        });                
     }, 


     /**
     * Método para crear una propiedad dentro de la sección exportación. Añadirá el row de la propiedad en el panel correspondiente de la página editada / seleccionada
     * @param {*} button :Botón que se ha pulsado para añadir la propiedad
     */
    handleAddPropertyExportation: function(button){
        
        const that = this;
        // Encontrar la fila/página seleccionada
        that.filaPagina = button.closest('.page-row');        
        that.filaExportacion = button.closest(".exportation-row");

        // Panel listado de exportaciones
        const currentListPropertyExportations = that.filaExportacion.find(that.listProperties);
        
        // Mostrar loading para petición
        loadingMostrar();

        // Realizar la petición de nuevo propiedad para la exportación
        GnossPeticionAjax(
            that.urlAddNewExportationProperty,
            null,
            true
       ).done(function (data) {
            // OK - Añadir exportación
            // Añadir la nueva exportación
            currentListPropertyExportations.append(data); 
            // Obtener la última propiedad añadida
            const newPropertyAdded = currentListPropertyExportations.children().last();                   
            // Reiniciar la operativa de gestión Páginas para los nuevos items
            operativaGestionPaginas.init();
            // Mostrar los detalles de la nueva exportación
            newPropertyAdded.find(that.btnEditProperty).trigger("click");            
       }).fail(function (data) {
            // KO en creación de la propiedad filtro           
           mostrarNotificacion("error", data);
       }).always(function () {
            // Ocultar loading
            loadingOcultar();           
       });
    }, 

    /* Comportamientos sección facetas de la página */
    // ******************************************************

    /**
     * Método para poder editar un dashboard
     * @param {jqueryObject} button : Botón que ha sido pulsado para editar el dashboard
     */
    handleEditDashboard: function (button) {
        const that = this;

        // Fila de la página que está siendo editada y de la que se desesa cargar los gráficos
        const fila = button.closest('.modal-page');
        fila.find('.editarOpcionesDashboard').show();
        //var ejemplos = fila.find('.ejemplosGraficos')[0];
        /*new Sortable(ejemplos, {
            animation: 150,
            ghostClass: 'blue-background-class'
        });        
        tamFijo = (ejemplos.clientWidth - 40) / 3;
        */
        const ejemplos = fila.find(".generados")[0];
        tamFijo = (ejemplos.clientWidth - 40) / 3;
        button.parent().remove();
        // Detectar los botones de añadir para cargarlos
        //var btnsAnyadir = fila.find('.asistente input[name="btnAnyadir"]');
        const btnsAnyadir = fila.find(`.${that.btnAddGraphClassName}`);        
        for (i = 0; i < btnsAnyadir.length; i++) {
            //btnsAnyadir[i].click();
            const btnAnyadir = $(btnsAnyadir[i]);
            this.handleAnyadirGrafico(btnAnyadir);
        }
        // Permitir ordenación de los gráficos
        that.setupBasicSortableListForGraphics();
    },

    /**
     * Método para poder añadir un nuevo asistente
     * @param {jqueryObject} button : Botón que ha sido pulsado para añadir un asistente
     */
    handleAnyadirAsistente: function (button) {
        const that = this;
        // Mostrar loading para petición
        loadingMostrar();
        // Realizar la petición de nuevo asistente
        GnossPeticionAjax(
            this.urlPagina + '/new-asistente',
            null,
            true
        ).done(function (data) {
            // Página que está siendo editada
            const pageItem = button.closest(`.${that.modalPageClassName}`);
            //var listaAsistentes = button.parent().parent().find('ul.asistentesLista');
            const graphAssistantList = pageItem.find("ul.asistentesLista");
            // Añadir el nuevo asistente
            graphAssistantList.append(data);
            const asistenteNuevo = graphAssistantList.children().last();            
            //OperativaAcciones.init(idPanel);
            // Mostrar el nuevo gráfico recién creado
            asistenteNuevo.find(`.${that.btnEditGraphAssistantClassName}`).trigger('click');
            
            // Panel de todos los gráficos donde se añadirá el nuevo
            const panelPreviewAll = pageItem.find(`.${that.graphPreviewAllClassName}`);
            const graphicId = `${asistenteNuevo.prop("id")}GrafPreview`;
            // Añadir el gráfico nuevo a la sección de Previsualización general
            const newGraphicDetail = `
            <li class="graphicOrderRow">
                <div class="component-sortable js-component-sortable-graph">
                    <span class="material-icons-outlined sortable-icon ui-sortable-handle-graphs">drag_handle</span>
                </div>
                <div id="${graphicId}" class="generados previewGraph"></div>
            </li>            
            `;
            panelPreviewAll.append(newGraphicDetail);
            // Cargar operativa para los nuevos items            
            operativaGestionPaginas.init();            
        }).fail(function (data) {
            console.log("ERROR =>" + data);
        }).always(function () {
            loadingOcultar();
        });
    },

    /**
     * Método para eliminar un asistente de gráfico de una página de búsqueda semántica
     * Sustituye a handleEliminarAsistente
     * Si es de reciente creación y aún no se ha guardado, se eliminará simplemente el panel.
     * Si por el contrario, el elemento existe, se procederá al borrado/petición para su ejecución.
     * @param {*} pElementRow 
     */
    handleDeleteGraphAssistantElement: function(pElementRow){
        const that = this;

        // Eliminar la fila sin petición            
        pElementRow.fadeOut(300, function() { 
            $(this).remove(); 
        });
        
        // Eliminar la fila de la previsualización del gráfico correspondiente con el que se desea eliminar
        const grafId = pElementRow.prop("id");
        const grafPreviewRow = $(`#${grafId}GrafPreview`).parent();
        grafPreviewRow.remove();
    },    


    /**
     * Método para agregar un nuevo dataset
     * @param {jqueryObject} button : Botón que ha sido pulsado para añadir un nuevo dataset
     */
    handleAddDataset: function (button) {
        const that = this;
        // Mostrar loading para petición
        loadingMostrar();
        // Realizar la llamada para añadir dataset
        GnossPeticionAjax(
            this.urlPagina + '/new-dataset',
            null,
            true
        ).done(function (data) {            
            // Gráfico donde añadir el dataset que está siendo editada 
            const assistantGraphInfoPanel = button.closest(`.${that.assistantGraphInfoPanelClassName}`);
            const datasetsParent = assistantGraphInfoPanel.find(".opcionesGraf").not(".d-none");
            const datasetsList = datasetsParent.find("ul.datasetsLista");
            // Añadir el nuevo dataset
            datasetsList.append(data);
            const dataSetNuevo = datasetsList.children().last();            
            // Mostrar el nuevo gráfico recién creado            
            dataSetNuevo.find(`.${that.btnEditDataSetGraphAssistantClassName}`).trigger('click');
            // Cargar operativa para los nuevos items            
            operativaGestionPaginas.init();          
        }).fail(function (data) {
            console.log("ERROR =>" + data);
        }).always(function () {
            loadingOcultar();
        });
    },

    /**
     * Método para añadir nueva columna en el dataset
     * @param {jqueryObject} button : Botón que ha sido pulsado para añadir Nueva columna
     */
    handleAgregarColumna: function (button) {
        const that = this;
        // Mostrar loading para petición
        loadingMostrar();
        // Realizar la llamada para añadir nueva propiedad
        GnossPeticionAjax(
            this.urlPagina + '/new-propiedad',
            null,
            true
        ).done(function (data) {
            // Gráfico donde añadir el dataset que está siendo editada 
            const assistantGraphInfoPanel = button.closest(`.${that.assistantGraphInfoPanelClassName}`);
            const datasetsParent = assistantGraphInfoPanel.find(".opcionesGraf").not(".d-none");
            const datasetsList = datasetsParent.find("ul.datasetsLista");
            // Añadir el nuevo dataset
            datasetsList.append(data);
            const dataSetNuevo = datasetsList.children().last();            
            // No mostrar los colores para el dataset
            if (assistantGraphInfoPanel.find('select[name="tGrafico"]').val() == "4") {
                dataSetNuevo.find('.pColor').hide();
            }

            // Mostrar el dataset recién creado                    
            dataSetNuevo.find(`.${that.btnEditDataSetNoAgrupacionGraphAssistantClassName}`).trigger('click');
            // Cargar operativa para los nuevos items            
            operativaGestionPaginas.init();               
        }).fail(function (data) {            
            mostrarNotificacion("error", data);
        }).always(function () {
            loadingOcultar();
        });
    },

    /**
     * Método para eliminar un asistente
     * @param {jqueryObject} button : Botón que ha sido pulsado para eliminar un asistente
     */
    /*handleEliminarAsistente: function (button) {
        var asistente = button.closest('.asistente');
        var id = asistente.prop('id');
        var divGraf = asistente.closest('.editarDashboard').find('#' + id + 'Graf');
        if (divGraf.length == 1) {
            divGraf.remove();
        }
        asistente.remove();
    },
    */

    /**
     * Método para cambiar las opciones del dashboard
     * @param {jqueryObject} button : Botón que ha sido pulsado para cambiar las opciones del dashboard
     */
    handleCambiarOpciones: function (button) {
        var tipo = button.val();
        if (tipo != "0") {
            var opciones = button.closest('.panEdicion').find('.opciones');
            var opBarras = opciones.find('.opcionesBarras');
            var opLineas = opciones.find('.opcionesLineas');
            var opCirculos = opciones.find('.opcionesCirculos');
            var opTabla = opciones.find('.opcionesTabla');
            var opHeat = opciones.find('.opcionesHeatMap');
            var opLabels = opciones.find('.opLabels');

            switch (tipo) {
                case "1":
                    opBarras.removeClass("d-none");
                    opLineas.addClass("d-none");
                    opCirculos.addClass("d-none");
                    opTabla.addClass("d-none");
                    opHeat.addClass("d-none");
                    opLabels.removeClass("d-none");
                    break;
                case "2":
                    opBarras.addClass("d-none");
                    opLineas.removeClass("d-none");
                    opCirculos.addClass("d-none");
                    opTabla.addClass("d-none");
                    opHeat.addClass("d-none");
                    opLabels.removeClass("d-none");
                    break;
                case "3":
                    opBarras.addClass("d-none");
                    opLineas.addClass("d-none");
                    opCirculos.removeClass("d-none");
                    opTabla.addClass("d-none");
                    opHeat.addClass("d-none");
                    opLabels.removeClass("d-none");
                    break;
                case "4":
                    opBarras.addClass("d-none");
                    opLineas.addClass("d-none");
                    opCirculos.addClass("d-none");
                    opTabla.removeClass("d-none");
                    opHeat.addClass("d-none");
                    opLabels.addClass("d-none");
                    break;
                case "5":
                    opBarras.addClass("d-none");
                    opLineas.addClass("d-none");
                    opCirculos.addClass("d-none");
                    opTabla.addClass("d-none");
                    opHeat.removeClass("d-none");
                    opLabels.addClass("d-none");
                    break;
            }
        }
    },

    /**
     * Método para añadir un grafico
     * @param {jqueryObject} button : Botón que ha sido pulsado para añadir un grafico
     */
    handleAnyadirGrafico: function (button) {
        var that = this;
        var asistente = button.closest('.asistente');
        // Modal que está siendo editado        
        const currentModal = asistente.closest(`.${that.modalPageClassName}`);       
        
        var tipo = asistente.find('select[name="tGrafico"]').val();
        if (tipo != 0) {

            var select = asistente.find('input[name="selectGrafico"]').val();
            var where = asistente.find('textarea[name="whereGrafico"]')[0].value;
            var groupby = asistente.find('input[name="groupbyGrafico"]').val();
            var orderby = asistente.find('input[name="orderbyGrafico"]').val();
            var limit = asistente.find('input[name="limitGrafico"]').val();
            var idChart = asistente.prop('id');
            //var tipoContexto = asistente.closest('.panEdicion').find('input[name="TabCampoFiltro"]').val();
            // const currentModal = asistente.closest(`.${that.modalPageClassName}`);
            var tipoContexto = currentModal.find('input[name="TabCampoFiltro"]').val();                     

            var dataPost = {
                select: select,
                where: where,
                groupby: groupby,
                orderby: orderby,
                limit: limit,
                tipoContexto: tipoContexto,
                idChart: idChart
            }

            // Mostrar loading en el panel de visualización del gráfico
            const panelPreview = asistente.find(".previewGraph");
            loadingMostrar(panelPreview);
            // Mostrar loading en el panel de visualización de todos los gráficos
            const panelAllPreview = currentModal.find(".ejemplosGraficos");                    
            loadingMostrar(panelAllPreview);

            // Realizar la llamada para cargar los resultados
            GnossPeticionAjax(
                this.urlPagina + '/cargarResultados',
                dataPost,
                true
            ).done(function (data) {
                if (data != '"0resultados|||"' && data != "") {
                    var divEjemplos = button.closest('.editarOpcionesDashboard').find('.ejemplosGraficos');
                    var divGraf = divEjemplos[0];

                    var nombre = asistente.find('input[name="nGrafico"]').val();
                    var tamGrafico = asistente.find('select[name="tamGrafico"]').val();

                    var tam = tamFijo;
                    switch (tamGrafico) {
                        case "1x":
                            tam = tamFijo;
                            break;
                        case "2x":
                            tam = 2 * tamFijo + 4;
                            break;
                        case "3x":
                            tam = 3 * tamFijo + 8;
                            break;
                    }

                    var mtGrafico = asistente.find('input[name="mtGrafico"]').prop('checked');

                    switch (tipo) {
                        case "1":
                            var opciones = asistente.find('.opcionesBarras');
                            break;
                        case "2":
                            var opciones = asistente.find('.opcionesLineas');
                            break;
                        case "3":
                            var opciones = asistente.find('.opcionesCirculos');
                            break;
                        case "4":
                            var opciones = asistente.find('.opcionesTabla');;
                            break;
                        case "5":
                            var opciones = asistente.find('.opcionesHeatMap');;
                            break;
                    }

                    var etiquetas = [];
                    var labelNombre = asistente.find('input[name="labelsGrafico"]').val();
                    if (labelNombre != "") {
                        etiquetas.push(labelNombre);
                    }
                    var datosDatasets = opciones.find('input[name="datosDataset"]');
                    if (tipo != 3) {
                        for (var i = 0; i < datosDatasets.length; i++) {
                            etiquetas.push(datosDatasets[i].value);
                        }
                    } else {
                        etiquetas.push(datosDatasets.val());
                    }

                    var ordenes = that.orden(select, etiquetas);

                    // var divNuevo = divEjemplos.find('#' + idChart + 'Graf')[0];                    
                    let divNuevo = $('#' + idChart + 'Graf')[0];
                    if (divNuevo == null) {
                        divNuevo = document.createElement("div");
                        divNuevo.className = "generados";
                        divNuevo.id = idChart + 'Graf';
                        divGraf.appendChild(divNuevo);
                    } else {
                        divNuevo.innerHTML = "";
                    }
                    //divNuevo.style = "width:" + tam + "px; height:" + tam + "px; display:inline-block";
                    divNuevo.style = "width:" + tam + "px; height:" + tam + "px;";
                    if (tipo != "3") {
                        //divNuevo.style = "width:" + tam + "px; height:" + tam / 2 + "px; display:inline-block";
                        divNuevo.style = "width:" + tam + "px; height:" + tam / 2 + "px;";
                    }
                    
                    // Panel donde estarán todos los gráficos para su previsualización
                    const previewGraphsPanel = currentModal.find(".previewGraphsPanel");
                    //let divNuevoPreviewGraph = previewGraphsPanel.find(('#' + idChart + 'Graf')[0]);
                    let divNuevoPreviewGraph = previewGraphsPanel.find(('#' + idChart + 'GrafPreview'))[0];
                    if (divNuevoPreviewGraph == null) {
                        divNuevoPreviewGraph = document.createElement("div");
                        divNuevoPreviewGraph.className = "generados";
                        divNuevoPreviewGraph.id = idChart + 'Graf';
                        // Falta esto. Ver qué hace ahora --> divGraf.appendChild(divNuevo);
                    } else {
                        divNuevoPreviewGraph.innerHTML = "";
                    }
                    //divNuevo.style = "width:" + tam + "px; height:" + tam + "px; display:inline-block";
                    divNuevoPreviewGraph.style = "width:" + tam + "px; height:" + tam + "px;";
                    if (tipo != "3") {
                        //divNuevo.style = "width:" + tam + "px; height:" + tam / 2 + "px; display:inline-block";
                        divNuevoPreviewGraph.style = "width:" + tam + "px; height:" + tam / 2 + "px;";
                    }                    

                    
                    if (tipo != "4" && tipo != "5") {
                        var datos;
                        var config = {
                            data: datos,
                            options:
                            {
                                responsive: true,
                                plugins:
                                {
                                    legend: {
                                        position: 'top',
                                    }, title: {
                                        display: mtGrafico,
                                        text: nombre
                                    }
                                }
                            }
                        };

                        switch (tipo) {
                            case "1":
                                datos = that.dataBarrasLineas(asistente, tipo, data, ordenes);
                                config.type = 'bar';
                                var horizontal = asistente.find('input[name="horizontal"]').prop('checked');
                                if (horizontal) {
                                    config.options.indexAxis = 'y';
                                }
                                break;
                            case "2":
                                datos = that.dataBarrasLineas(asistente, tipo, data, ordenes);
                                config.type = 'line';
                                break;
                            case "3":
                                datos = that.dataCirculos(data, ordenes);
                                config.type = 'pie';
                                config.options.plugins.legend.display = false;
                                break;
                        }

                        config.data = datos;
                        // Añadir previsualización del gráfico en sección del gráfico
                        that.ejemploGrafico(divNuevo, config);
                        // Añadir previsualización del gráfico en sección previsualización de todos los gráficos
                        that.ejemploGrafico(divNuevoPreviewGraph, config);                        
                    } else {
                        switch (tipo) {
                            case "4":
                                that.dibujarTabla(asistente, data, ordenes, divNuevo);
                                // Dibujar el gráfico en la sección de Previsualización de todos los gráficos
                                that.dibujarTabla(asistente, data, ordenes, divNuevoPreviewGraph);
                                break;
                            case "5":
                                that.dibujarHeatMap(asistente, data, ordenes, divNuevo, tam, mtGrafico);
                                // Dibujar el gráfico en la sección de Previsualización de todos los gráficos
                                that.dibujarHeatMap(asistente, data, ordenes, divNuevoPreviewGraph, tam, mtGrafico);
                                break;
                        }
                    }                    
                    asistente.addClass('asisAnyadido');
                }
            }).fail(function (data) {                
                mostrarNotificacion("error", data);
            }).always(function () {
                loadingOcultar();
            });
        }
    },

    /**
     * Método para configurar el sortable básico para la lista de gráficos
     */
    setupBasicSortableListForGraphics: function(){
        const that = this;                    

        const graphPreviewPanel = $(`.${this.graphPreviewAllClassName}`);
        $.each(graphPreviewPanel, function(){            
            const graphPreviewPanelId = $(this).prop("id");                        
            setupBasicSortableList(graphPreviewPanelId, that.sortableGraphicIconClassName, undefined, undefined, undefined, undefined); 
        });                 
    },

    orden: function (select, etiquetas) {
        var ordenes = [];
        var aux = [];

        for (var i = 0; i < etiquetas.length; i++) {
            aux.push(select.indexOf(etiquetas[i]));
        }
        function comparar(a, b) { return a - b; }
        var copia = aux.slice();
        aux.sort(comparar);
        for (var i = 0; i < aux.length; i++) {
            ordenes.push(aux.indexOf(copia[i]));
        }

        return ordenes;
    },

    dibujarTabla: function (asistente, data, ordenes, divNuevo) {
        var datos = data.split("|||");
        var nombres = asistente.find('.opcionesTabla input[name="nombreDataset"]');
        var data = new google.visualization.DataTable();
        for (var i = 0; i < nombres.length; i++) {
            data.addColumn('string', nombres[i].value);
        }
        var filas = [];
        for (var i = 1; i < datos.length - 1; i++) {
            var filaDatos = datos[i].split("@@@");
            var fila = [];
            for (var j = 0; j < filaDatos.length - 1; j++) {
                fila.push(filaDatos[ordenes[j]]);
            }
            filas.push(fila)

        }
        data.addRows(filas);
        var table = new google.visualization.Table(divNuevo);
        table.draw(data, { width: '100%', height: '100%' });
    },

    dibujarHeatMap: function (asistente, data, ordenes, divNuevo, tam, mtGrafico) {

        var datos = data.split("|||");
        var nombre = asistente.find('input[name="nGrafico"]').val();
        var ejeX = [];
        var ejeY = [];
        for (var i = 1; i < datos.length - 1; i++) {
            var filaDatos = datos[i].split("@@@");
            if (ejeX.indexOf(filaDatos[ordenes[0]]) < 0) {
                ejeX.push(filaDatos[ordenes[0]]);
            }
            if (ejeY.indexOf(filaDatos[ordenes[1]]) < 0) {
                ejeY.push(filaDatos[ordenes[1]]);
            }
        }
        var datosGraf = [];
        for (var i = 1; i < datos.length - 1; i++) {
            var filaDatos = datos[i].split("@@@");
            var fila = [ejeX.indexOf(filaDatos[ordenes[0]]), ejeY.indexOf(filaDatos[ordenes[1]]), parseInt(filaDatos[ordenes[2]])];
            datosGraf.push(fila);
        }

        var chart = Highcharts.chart(divNuevo.id, {
            chart: {
                type: 'heatmap',
                marginTop: 40,
                marginBottom: 80,
                plotBorderWidth: 1
            },

            title: {
                text: null
            },

            xAxis: {
                categories: ejeX
            },

            yAxis: {
                categories: ejeY,
                title: null,
                reversed: true
            },

            colorAxis: {
                min: 0,
                minColor: '#FFFFFF',
                maxColor: asistente.find('.opcionesHeatMap').find('input[name="colorDataset"]')[2].value
            },

            legend: {
                align: 'right',
                layout: 'vertical',
                margin: 0,
                verticalAlign: 'top',
                y: 25,
                symbolHeight: tam / 2 - 120
            },

            tooltip: {
                formatter: function () {
                    return this.point.series['xAxis'].categories[this.point['x']] + '<br/>' + this.series.name + '<br/>' + this.point.series['yAxis'].categories[this.point['y']] + '<br/><b>' + this.point.value + '</b>';
                }
            },

            series: [{
                name: nombre,
                borderWidth: 1,
                data: datosGraf,
                dataLabels: {
                    enabled: true,
                    color: '#000000'
                }
            }],
        });

        if (mtGrafico) {
            chart.update({
                title: {
                    text: nombre
                }
            });
        }
    },

    /**
     * Método para almacenar los datos de los circulos del Dashboard
     * @param {jqueryObject} asistente : asistente
     * @param {jqueryObject} tipo : tipo de opcion
     * @param {jqueryObject} data : Datos para los circulos del Dashboard
     * @param {jqueryObject} ordenes : Ordenes para los circulos del Dashboard
     */
    dataBarrasLineas: function (asistente, tipo, data, ordenes) {
        var datos = data.split("|||")
        var labels = [];
        for (var i = 1; i < datos.length - 1; i++) {
            labels.push(datos[i].split("@@@")[ordenes[0]]);
        }
        var datasets = [];
        var fill = false;
        var trasparencia = "";
        var tipoS = '.opcionesBarras';
        if (tipo == "2") {
            tipoS = '.opcionesLineas';
            fill = asistente.find('[name="area"]').prop('checked');
            if (fill) {
                trasparencia = "30";
            }
        }
        //var colores = asistente.find(tipoS + ' input[class="colores"]');
        const panel = asistente.find(tipoS);
        const colores = panel.find(".colores");
        //const nombres = asistente.find(tipoS + ' input[name="nombreDataset"]');
        const nombres = panel.find('input[name="nombreDataset"]');

        for (var i = 0; i < colores.length; i++) {
            var dat = [];
            for (var j = 1; j < datos.length; j++) {
                dat.push(datos[j].split("@@@")[ordenes[i + 1]]);
            }
            var dataset = {
                label: nombres[i].value,
                data: dat,
                fill: fill,
                borderColor: colores[i].value,
                backgroundColor: colores[i].value + trasparencia,
            };
            datasets.push(dataset);
        }
        var conf = {
            labels: labels,
            datasets: datasets
        };
        return conf;
    },

    /**
     * Método para almacenar los datos de los circulos del Dashboard
     * @param {jqueryObject} data : Datos para los circulos del Dashboard
     * @param {jqueryObject} ordenes : Ordenes para los circulos del Dashboard
     */
    dataCirculos: function (data, ordenes) {
        var datos = data.split("|||")
        var labels = [];
        for (var i = 1; i < datos.length - 1; i++) {
            labels.push(datos[i].split("@@@")[ordenes[0]]);
        }

        var dat = [];
        for (var j = 1; j < datos.length; j++) {
            dat.push(datos[j].split("@@@")[ordenes[1]]);
        }

        var colores = [];
        for (var j = 0; j < DATA_COUNT; j++) {
            colores.push("#" + Math.floor(Math.random() * 16777215).toString(16));
        }

        var dataset = {
            label: 'Dataset',
            data: dat,
            backgroundColor: colores
        };

        var conf = {
            labels: labels,
            datasets: [dataset]
        };
        return conf;
    },

    /**
     * Método para generar el grafico nuevo
     * @param {jqueryObject} divNuevo : Datos para los circulos del Dashboard
     * @param {jqueryObject} config : Ordenes para los circulos del Dashboard
     */
    ejemploGrafico: function (divNuevo, config) {
        var canvas = document.createElement("canvas");
        canvas.className = "graficoCtx";
        divNuevo.appendChild(canvas);
        var ctx = canvas.getContext('2d');

        new Chart(ctx, config);
    },

    /**
     * Método para las opciones del Dataset
     * @param {jqueryObject} button : Botón que ha sido pulsado para sacar las opciones del dataset
     */
    handleOpcionesDataset: function (button) {
        const fila = button.closest('.row-dataset');
        const panEditar = fila.find('.panEdicion');
        panEditar.find('.opcionesDataset').show();
        panEditar.find('.opcionesAgrupacion').hide();
        panEditar.find('input[name="btnOpcionesAgrupacion"]').show();
        panEditar.find('input[name="btnAgregarAgrupacion"]').hide();
        button.hide();
    },

    /**
     * Método para las opciones de agrupación
     * @param {jqueryObject} button : Botón que ha sido pulsado 
     */
    handleOpcionesAgrupacion: function (button) {
        const fila = button.closest('.row-dataset');
        const panEditar = fila.find('.panEdicion');
        panEditar.find('.opcionesAgrupacion').show();
        panEditar.find('.opcionesDataset').hide();
        panEditar.find('input[name="btnOpcionesDataset"]').show();
        panEditar.find('input[name="btnAgregarPropiedadDat"]').show();
        panEditar.find('input[name="btnAgregarAgrupacion"]').show();
        button.hide();
    },

    /**
     * Método para 
     * @param {jqueryObject} button : Botón que ha sido pulsado 
     */
    handleOpcionesLabels: function (button) {
        var fila = button.closest('.asistente-row');
        var panEditar = fila.find('.panEdicion');
        panEditar.find('.opcionesLabels').show();
        panEditar.find('.opcionesPropiedad').hide();
        panEditar.find('input[name="btnOpcionesPropiedad"]').show();
        panEditar.find('input[name="btnAgregarPropiedad"]').hide();
        button.hide();
    },

    /**
     * Método para las opciones de la propiedad
     * @param {jqueryObject} button : Botón que ha sido pulsado 
     */
    handleOpcionesPropiedad: function (button) {
        var fila = button.closest('.asistente-row');
        var panEditar = fila.find('.panEdicion');
        panEditar.find('.opcionesPropiedad').show();
        panEditar.find('.opcionesLabels').hide();
        panEditar.find('input[name="btnOpcionesLabels"]').show();
        panEditar.find('input[name="btnAgregarPropiedad"]').show();
        button.hide();
    },

    /**
     * Método para agregar propiedad
     * @param {jqueryObject} button : Botón que ha sido pulsado 
     */
    handleAgregarPropiedad: function (button) {
        var that = this;
        var propiedad = button.closest('.opciones').find('input[name="propiedadGrafico"]').val().trim();
        var consulta = button.closest('.panEdicion').find('.consulta');
        var datos = that.transformarPropiedad(propiedad);
        button.closest('.opciones').find('input[name="labelsGrafico"]').val(datos[0]);
        that.agregarDatosConsulta(consulta, datos);
    },
    
    /**
     * Método para agregar propiedades al dataset
     * @param {jqueryObject} button : Botón que ha sido pulsado 
     */
    handleAgregarPropiedadDat: function (button) {
        var that = this;
        var propiedad = button.closest('.panEdicion').find('.opcionesAgrupacion').find('input[name="propiedadDataset"]').val().trim();
        var consulta = button.closest('.opciones').closest('.panEdicion').find('.consulta');
        var datos = that.transformarPropiedad(propiedad);
        var variable = datos[0];
        button.closest('.panEdicion').find('input[name="datosDataset"]').val(variable);
        var tipo = button.closest('.asistente').find('select[name="tGrafico"]').val();
        if (tipo == "4") {
            datos[1] = "optional{" + datos[1].slice(0, -2) + "}.";
        }
        if (tipo == "5") {
            that.agregarGroupBy(consulta, variable)
        }
        that.agregarDatosConsulta(consulta, datos);
    },
    
    /**
     * Método para 
     * @param {jqueryObject} button : Botón que ha sido pulsado 
     */
    handleAgregarAgrupacion: function (button) {
        var that = this;
        var tipo = button.closest('.panEdicion').find('select[name="agrupacionDataset"]').val();
        var sobre = button.closest('.panEdicion').find('input[name="agrupacionsobreDataset"]').val().trim();
        var tipoGraf = button.closest('.asistente').find('select[name="tGrafico"]').val();
        var datos;
        if (sobre == "Sujeto" && tipo == "count") {
            datos = ["count(distinct(?s)) as ?cont_suj ", ""]
            button.closest('.panEdicion').find('input[name="datosDataset"]').val("?cont_suj");
        } else {
            var propiedad = that.transformarPropiedad(sobre);
            if (tipo == "count" && tipoGraf != "5") {
                datos = ["count(distinct(" + propiedad[0] + ")) as " + propiedad[0] + "_" + tipo + " ", propiedad[1]]
            } else {
                datos = [tipo + "(" + propiedad[0] + ") as " + propiedad[0] + "_" + tipo + " ", propiedad[1]]
            }
            button.closest('.panEdicion').find('input[name="datosDataset"]').val(propiedad[0] + "_" + tipo);
        }
        var consulta = button.closest('.opciones').closest('.panEdicion').find('.consulta');

        that.agregarDatosConsulta(consulta, datos);
    },
    
    /**
     * Método para transformar la propiedad
     * @param {jqueryObject} propiedad : Botón que ha sido pulsado 
     */
    transformarPropiedad: function (propiedad) {
        var niveles = propiedad.split('@@@');
        var sujeto = "?s";
        var where = "";
        for (var i = 0; i < niveles.length; i++) {
            var objeto = "?" + niveles[i].replace(':', '');
            var condicion = sujeto + " " + niveles[i] + " " + objeto + ". ";
            where = where + condicion;
            sujeto = objeto;
        }
        return [sujeto, where];
    },

    /**
     * Método para agregar datos a la consulta
     * @param {jqueryObject} consulta : Consulta
     * @param {jqueryObject} datos : Datos
     */
    agregarDatosConsulta: function (consulta, datos) {
        var select = consulta.find('input[name="selectGrafico"]');
        var where = consulta.find('textarea[name="whereGrafico"]');
        select[0].value = select[0].value + datos[0] + " ";
        where[0].value = where[0].value + datos[1];
    },

    /**
     * Método para agrupar por 
     * @param {jqueryObject} consulta : Consulta
     * @param {jqueryObject} datos : Datos
     */
    agregarGroupBy: function (consulta, datos) {
        var groupBy = consulta.find('input[name="groupbyGrafico"]');
        groupBy[0].value = groupBy[0].value + datos + " ";
    },


        /**
     * Método para Cambiar el título del gráfico
     * @param {jqueryObject} inputName : Botón que ha sido pulsado 
     */
    cambiarNombreTituloPestanya: function(inputName){
        const nombrePestanya = inputName.val();
        const fila = inputName.closest('.asistente-row');
        const cabecera = fila.find('.component-header')

        const nombreFila = cabecera.find('.component-assistantName');

        if (nombrePestanya == '') {
            nombreFila.html(nombreFila.attr('aux'));
        }
        else {
            nombreFila.html(nombrePestanya);
        }
    },

    /**
     * Método para Cambiar el nombre de la pestanya
     * @param {jqueryObject} inputName : Botón que ha sido pulsado 
     */
    cambiarNombrePestanya: function (inputName) {
        const nombrePestanya = inputName.val();
        const fila = inputName.closest('.row-dataset');
        const cabecera = fila.find('.component-header')

        const nombreFila = cabecera.find('.component-assistantName');

        if (nombrePestanya == '') {
            nombreFila.html(nombreFila.attr('aux'));
        }
        else {
            nombreFila.html(nombrePestanya);
        }
    },
        
    /**
     * Método para 
     * @param {jqueryObject} button : Botón que ha sido pulsado 
     */
    handle: function (button) {
        // Mostrar loading para petición
        loadingMostrar();
        // Realizar la llamada para
    },

    /* Comportamientos sección facetas de la página */
    // ******************************************************
    
    /**
     * Método para poder añadir una nueva faceta
     * @param {jqueryObject} button : Botón que ha sido pulsado para añadir una nueva faceta
     */
    handleAddFacet: function(button){

        const that = this;
        // Encontrar la fila/página seleccionada
        that.filaPagina = button.closest('.page-row');        
        // Panel listado de las facetas
        const currentListFacet = that.filaPagina.find(that.listFacets);
        
        // Mostrar loading para petición
        loadingMostrar();

        // Realizar la petición de nuevo filtro
        GnossPeticionAjax(
            that.urlAddNewFacet,
           null,
           true
       ).done(function (data) {        
            // OK - Añadir Faceta
            // Añadir la nueva faceta creada al listado de facetas
            currentListFacet.append(data); 
            // Obtener la última faceta añadida
            const newFacetAdded = currentListFacet.children().last();                   
            // Reiniciar la operativa de gestión Páginas para los nuevos items
            operativaGestionPaginas.init();
            // Lanzar el comportamiento de selección de faceta para cargar correctamente los OC
            newFacetAdded.find(that.selectListaFacetas).trigger("change");
            // Mostrar los detalles de la nueva faceta creada
            newFacetAdded.find(that.btnEditFacet).trigger("click"); 

       }).fail(function (data) {
            // KO en creación del filtro           
           mostrarNotificacion("error", data);
       }).always(function () {
            // Ocultar loading
            loadingOcultar();           
       });
    },

    /**
     * Método para poder asociar una faceta con un objeto de conocimiento dentro de una página
     * @param {jqueryObject} button : Botón o ComboBox que ha sido pulsado para asociar un OC a una faceta
     */
    handleSetFacetWithOC: function(button){
        const that = this;

        // Encontrar la fila/página seleccionada
        that.filaPagina = button.closest('.page-row'); 
        const filaFaceta = button.closest(".facet-row");

        const currentSelectListaOc = filaFaceta.find(that.selectListaOc);
        const currentSelectListaFacetas = button;
        const currentSelectDatosAux = filaFaceta.find(that.selectFacetOcAux);

        // Vaciar el select de lista de objetos de conocimiento        
        currentSelectListaOc.empty();
        // Obtener la faceta seleccioanda
        const facetaSeleccionada = currentSelectListaFacetas.children('option:selected').attr('name');
        const facetaSeleccionadaString = currentSelectListaFacetas.children('option:selected').html().trim();          
        // Objetos del select aux
        const optionsSelectDatosAux = currentSelectDatosAux.find("option.objAux");
        // Facetas existentes obtenidas del select
        const facetas = currentSelectListaFacetas.find('option.facetas');

        // Rellenar el select de OC en base a la faceta seleccionada
        facetas.each(function () {
            // Obtener el nombre de la faceta
            const facetaName = $(this).attr('name');
            // Rellenar opciones en base a la selección
            optionsSelectDatosAux.each(function () {
                const obj = $(this).attr('name');
                const obj2 = $(this).val();
                if (obj == facetaName && facetaSeleccionada == facetaName) {
                    currentSelectListaOc.append('<option value="' + obj + '">' + obj2 + '</option>');
                }
            });
        });

        // Cambiar el nombre de la faceta en la fila       
         filaFaceta.find(`.component-facetName`).html(facetaSeleccionadaString);           
    },  
    
    
    /**
     * Método para marcar una Faceta como "Eliminada" o no Eliminada. 
     * Si se decide eliminar, no se elimina por completo hasta que se guarda la sección de páginas.
     * @param {*} filaFaceta : Fila de la faceta que se deseará eliminar o revertir su eliminación
     * @param {bool} deleteFacet : Valor que indica si se desea borrar la faceta o no
     */     
     handleDeleteFacet: function(filaPagina, btnDeleted, deleteFacet){
        const that = this;
        
        // Fila de la página que está siendo seleccionada        
        const filaFaceta = btnDeleted.closest('.facet-row');
        
        // Botón de eliminación y edición de la página actual
        const btnEliminarCurrentFacet = filaFaceta.find(that.btnDeleteFacet);
        const btnEditCurrentFacet = filaFaceta.find(that.btnEditFacet);
        let btnRevertirFacetaEliminada = filaFaceta.find(".btnRevertDeleteFacet");
        // Area donde están los botones de acción (Editar, Eliminar, Revertir)
        const areaComponentActions = filaFaceta.find(".component-actions");
        
        if (deleteFacet){
            // Realizar el "borrado" del filtro
            // 1 - Marcar la opción "TabEliminada" a true           
            filaFaceta.find('[name="TabEliminada"]').val("true");                                   
            // 2 - Crear y añadir un botón de revertir el borrado de la página
            btnRevertirFacetaEliminada = `
            <li>
                <a class="action-delete round-icon-button js-action-delete btnRevertDeleteFacet" href="javascript: void(0);">
                    <span class="material-icons">restore_from_trash</span>
                </a>
            </li>
            `;
            // 3 - Ocultar los botones de editar y eliminar la página actual
            btnEliminarCurrentFacet.addClass("d-none");
            // Dejarlo como disabled
            btnEditCurrentFacet.addClass("inactiveLink");
            // 4 - Añadir el botón de revertir
            areaComponentActions.append(btnRevertirFacetaEliminada);            
            // 5- Añadir comportamiento de revertir la página            
            filaFaceta.find(".btnRevertDeleteFacet").off().on("click", function(){
                that.handleDeleteFacet(filaPagina, $(this), false);
            });
            // 6- Añadir la clase de "deleted" a la fila de la página            
            filaFaceta.addClass("deleted");
            
            // 7- Colapsar el panel si se desea eliminar y este está abierto
            const collapseId = filaFaceta.data("collapseid");
            $(`#${collapseId}`).removeClass("show");
        }else{
            // Revertir el "borrado" de la página
            // 1 - Marcar la opción "TabEliminada" a false            
            filaFaceta.find('[name="TabEliminada"]').val("false");  
            // 2 - Eliminar el botón de revertir la filtro de la página
            btnRevertirFacetaEliminada.remove();
            // 3 - Mostrar de nuevo los botones de editar y eliminar la página actual
            btnEliminarCurrentFacet.removeClass("d-none");
            btnEditCurrentFacet.removeClass("inactiveLink");    
            // 4 - Quitar la clase de "deleted" a la fila de la página            
            filaFaceta.removeClass("deleted");
        }
    },

    /**
     * Método para cargar las facetas existentes y listarlas en la sección correspondiente de la página editada
     * Solo estará disponible este método al hacer click en una label siempre que haya facetas asociadas a la página
     * @param {jqueryObject} button 
     */
    handleLoadFacetList: function(button){

        const that = this;
        // Encontrar la fila/página seleccionada
        that.filaPagina = button.closest('.page-row');        
        // Panel listado de las facetas
        const currentListFacet = that.filaPagina.find(that.listFacets);        
        // Mostrar loading para petición
        loadingMostrar();    
        // Realizar la petición de nuevo filtro
        GnossPeticionAjax(
            that.urlLoadFacetList,
           null,
           true
       ).done(function (data) {        
            // OK - Cargar las facetas existentes
            currentListFacet.append(data); 
            // Eliminar el click de "Ver facetas" ya que las facetas ya se han cargado
            button.off();
            // Reiniciar la operativa de gestión Páginas para los nuevos items
            operativaGestionPaginas.init();
       }).fail(function (data) {
            // KO en carga de facetas existentes           
           mostrarNotificacion("error", data);
       }).always(function () {
            // Ocultar loading
            loadingOcultar();           
       });        
    }, 


    /* Comportamientos sección Editar página tipo Home vía CMS */
    // ******************************************************
    /**
     * Método para ocultar o visualizar los paneles relativos a la edición de la página de tipo Home vía CMS. 
     * Se activa cuando se pulsa en las opciones radioButton de 'Editar la Home con el CMS'     
     * @param {*} radioButton  RadioButton que ha sido pulsado
     */
    handleOptionEditarPaginaHomeCms: function(radioButton){
        const that = this;
        // Fila correspondiente a la opción editada
        const paginaRow = radioButton.closest('.page-row');

        // Comprobar si la opción seleccionada es Si/No
        if (radioButton.data("value") == "si"){
            // Mostrar paneles de configuración Pagina CMS
            paginaRow.find(that.panelTabHomeCMSUsersHomeDistintas).removeClass("d-none");
            // Controlar aparición de botones para edición de la Home
            that.handleOnSetHomeCmsPageType(paginaRow);
        }else{
            // Ocultar paneles de configuración Página CMS
            paginaRow.find(that.panelTabHomeCMSUsersHomeDistintas).addClass("d-none");
            // Ocultar todos los botones que posibilitan la edición de la página Home vía CMS
            const btnEditCmsHomeTypeOptions = paginaRow.find($(`.${that.btnEditCmsHomeClassName}`));
            $.each(btnEditCmsHomeTypeOptions, function(){
                $(this).addClass("d-none");
            });           
        }
    },

    /**
     * Método para ocultar o visualizar el panel de edición de la página de tipo Home vía CMS para elegir opciones de Home para miembros o distinta para miembros y no miembros. 
     * Se activa cuando se pulsa en las opciones radioButton de 'Editar la Home con el CMS'     
     * @param {*} radioButton  RadioButton que ha sido pulsado
     */
    handleOptionEditarPaginaHomeCmsMiembrosNoMiembros: function(radioButton){
        const that = this;
        // Fila correspondiente a la opción editada
        const paginaRow = radioButton.closest('.page-row');

        // Comprobar si la opción seleccionada es Si/No
        if (radioButton.data("value") == "Distinta"){
            // Mostrar panel de Home para miembros / Home para no miembros            
            paginaRow.find(that.panelTabHomeCMSMiembrosNoMiembros).removeClass("d-none");            
        }else{
            // Ocultar paneles de configuración Página CMS
            paginaRow.find(that.panelTabHomeCMSMiembrosNoMiembros).addClass("d-none");            
        }
        that.handleOnSetHomeCmsPageType(paginaRow);
    },

    
    /**
     * Método que se ejecuta cuando se ordena o arrastra una página (Ordenar o jerarquía)
     * teniendo en cuenta el arrastre realizado de una de las páginas.
     * @param {jqueryObject} $itemEl : Objeto o fila jquery correspondiente con la página que ha sido arrastrada
     */
    handleReorderPages: function($itemEl){
        const that = this;
       
       // Obtener el padre del item arrastrado                
       let $parentNode = $itemEl.parents("li").first();
       // Si sólo se ordena y no se hace "Jerarquía", asignarlo correctamente (root)                
       if ($parentNode.length == 0){
           // Arrastrado al root de Páginas
           $parentNode = that.pageListContainer;
           // Asignar el parentKey "root" al $itemEl arrastrado                    
           $itemEl.find('[name="ParentTabKey"]').val("00000000-0000-0000-0000-000000000000");
       }else{
           // Arrastrado dentro de una página
           $parentNode = $parentNode;
           // Asignar el parentKey al $itemEl que corresponde con el $parentNode
           $itemEl.find('[name="ParentTabKey"]').val($parentNode.data("pagekey"));
           // Actualizar el TabOrden del parentNode
           //$parentNode.find('[name="TabOrden"]').val($parentNode.index());
       }
       // Asignar la posición de la página                
       //$itemEl.find('[name="TabOrden"]').val($itemEl.index());                                                                 
       // Proceder al guardado de la página
       that.handleSaveCurrentPage(false);        
    },


    /**
     * 
     * @param {jqueryObject} $itemEl : El elemento que se ha arrastrado
     * @param {jqueryObject} $parentNode : Posible nodo padre del item arrastrado
     * @returns Array de páginas ordenadas con el padre establecido correctamente
     */
    createCustomArrayWithPageInfo: function($itemEl, $parentNode){
        const that = this;
        // Páginas actuales existentes
        const pages = $(".page-row");
        const newPagesArray = [];

        // Reordenación de todas las páginas una vez se ha finalizado el arrastre de una de ellas       
        for (let index = 0; index < pages.length; index++) {
            const page = pages[index];

            // parentID de la página analizada
            let parentId =  $(page).find('[name="ParentTabKey"]').last().val(); 
            
            // Detectar si el item analizado es el que se ha arrastrado
            if ($(page).data("pagekey") == $itemEl.data("pagekey")){
                // Controlar el parentId del elemento arrastrado. Si ni siquiera tiene pagekey, es root
                if ($parentNode.data("parentkey") != undefined){
                    // Se ha arrastrado dentro de un elemento padre 
                    const parentKey = $parentNode.data('parentkey');
                    $(page).find('[name="ParentTabKey"]').last().val(parentKey);  
                    parentId = parentKey;                                      
                }else{
                    // Se ha arrastrado al root (sin padre)
                    $(page).find('[name="ParentTabKey"]').last().val('00000000-0000-0000-0000-000000000000');
                    parentId = '00000000-0000-0000-0000-000000000000'; 
                }
            }
                        
            // Orden de la página actual          
            // let orden = $(page).find('[name="TabOrden"]').val();
        
            let pageInfo = {
                pageKey: $(page).data("pagekey"),
                parentId: parentId,
                orden: parseInt(index),
            }
            newPagesArray.push(pageInfo);
        }
        return newPagesArray        
    },

    /* Guardado de páginas o pestañas de la sección estructura */
    /************************************************************/
    
    /**
     * 
     * @param {bool} reloadPage Indicar si se desea recargar la página actual. Por defecto true.
     */
    handleSaveCurrentPage: function(reloadPage = false){
        const that = this;

        that.handleSavePages(function(error){
            if (error == false){
                // 2 - Ocultar el modal
                const modalPage = $(that.filaPagina).find(`.${that.modalPageClassName}`);                                          
                dismissVistaModal(modalPage);
                if (reloadPage == true){                    
                    location.reload();
                }                                             
            } 
        });
    },


    /**
     * Método que se ejecutará cuando se pulse en el botón de "Guardar páginas" para proceder al guardado de la información de las páginas de la sección
     * estructura
     * @param {*} completion : Acciones a realizar una vez finalice el procesado de guardardo de páginas
     */
    handleSavePages: function ( completion ) {
        var that = this;
        // Mostrar loading
        loadingMostrar();
        

        // Lista de páginas
        const rowPages = $("#id-added-pages-list").find(".component-wrap.page-row");

        // Se desean guardar las páginas -> Quitar clase de "newPage" para NO eliminar la página de reciente creación
        $(".newPage").removeClass("newPage");

        // Resetear los errores globales previos (Flags urlRepetidos, urlVacíos,)
        that.errorRutaRepetida = false;
        that.errorRutaVacia = false;
        that.errorNombreVacio = false;
        that.errorFiltrosOrden = false;
        that.errorExportaciones = false;
        that.errorDuranteObtenerDatosPestaya = false;
        
        // Eliminar todos los posibles mensajes de error anteriores
        eliminarErroresEnInputs();

        // Comprobar que no hay errores antes de proceder con el guardado
        if (!that.comprobarErroresGuardado()) {
            // Listado de pestañas/páginas donde se irán añadiendo para su posterior guardado
            that.ListaPestanyas = {};
            let cont = 0;           
            // Recorrer cada página para obtener los datos y guardarlos            
            rowPages.each(function () {
                if (that.errorDuranteObtenerDatosPestaya == false){
                    that.obtenerDatosPestanya($(this), cont++);
                }                
            });

            // Realizar Guardado de páginas si no se han producido errores
            if (!that.errorDuranteObtenerDatosPestaya){
                that.savePages( function(savePagesError){
                    error = savePagesError;
                    // Resetear flag de confirmar eliminación de página
                    if (error == true){that.confirmDeletePage = false;} 
                    loadingOcultar();
                    // Eliminar los items pendientes de ser eliminados y no mostrados una vez se ha guardado la página
                    $(`.${that.pendingToBeRemovedClassName}`).remove();
                    completion != undefined && completion(error);
                });                
            }else{
                // Se ha producido algún error durante el guardado
                completion != undefined && completion(true);
                loadingOcultar();
            }
        }
        else {
            loadingOcultar();
        }

    },

    /**
     * Método que mostrará un mensaje si se ha producido un error durante el proceso de guardado.
     */
    mostrarErrorGuardado: function(){
        var esPre = "False";
        var entornoBloqueado = "False";

            if (esPre == "True") {
                $('input.guardarTodo').before('<div class="error general">No se permite guardar porque estás en el entorno de preproducción</div > ');
            }
            else if (entornoBloqueado == "True") {
                $('input.guardarTodo').before('<div class="error general">El entorno actual esta bloqueado. Hay que desplegar la versión de preproducción antes de hacer cambios</div>');
            }
            else {
                $('input.guardarTodo').before('<div class="error general">Ha habido errores en el guardado de las pesta&#xF1;as, revisa los errores marcados</div > ');
            }
    },


    /**
     * Método que comprobará si hay algún error antes de proceder con el guardado de las páginas. Se ejecuta desde 'handleSavePages'.
     * @returns bool: Devolverá un valor booleano indicando si hay algún error o no.
     */
    comprobarErroresGuardado: function(){
        const that = this;
        let error = false;

        // Comprobación de URL de las páginas (Vacías y Repetidas)
        if (that.comprobarUrlsRepetidas()) {
            error = true;
        }
        
        // Comprobación del nombre de las páginas (Vacíos)
        if (!error && that.comprobarNombresVacios()) {
            error = true;
        }
        // Comprobación de los nombres cortos de las páginas (Repetidos) -> No tiene sentido
        /*if (!error && that.comprobarNombresCortosRepetidos()) {
            error = true;
        }
        */

        // Comprobación del nombre de los textos por defecto de buscadores (Páginas de tipo buscador semántico)
        if (!error && that.comprobarTextosBuscadorPorDefecto()) {
            error = true;            
        }

        // Comprobar los filtros Orden de las páginas para que no estén vacíos o tengan nombre
        if (!error && that.comprobarFiltrosOrden()) {
            error = true;
        }
        
        if (!error && that.comprobarExportaciones()) {
            error = true;
        }
        // Comprobar/Asignar metaDescripciones si no hay errores previos en los diferentes idiomas
        if (!error) {
            this.comprobarMetadescripciones();
        }
        
        return error;
    },

    /**
     * Método para comprobar que no hay urls repetidas en las páginas creadas
     * @returns bool: Devuelve un valor booleano indicando si todo está OK para proceder con el guardado de las páginas.
     */
    comprobarUrlsRepetidas: function(){
        const that = this;              
        // Inputs con las rutas de páginas que hayan sido modificadas y no borradas (Más velocidad)
        const inputsRutas = $('.page-row:not(".deleted").modified input[name="TabUrl"]:not(":disabled")');
        inputsRutas.each(function () {
            // Comprobar las rutas de las páginas para que no estén repetidas
            if (that.errorRutaRepetida == false && that.errorRutaVacia == false){
                that.comprobarUrlRepetida($(this))
            }            
        });
        return that.errorRutaRepetida || that.errorRutaVacia;
    },

    /**
     * Método para comprobar que no hay Nombres vacíos en las páginas creadas/editadas
     * @returns bool: Devuelve un valor booleano indicando si todo está OK para proceder con el guardado de las páginas.
     */
    comprobarNombresVacios: function(){
        const that = this;        
        // Inputs con los nombres de todas las páginas que hayan sido modificadas y no borradas (Más velocidad)        
        const inputsNombre = $('.page-row:not(".deleted").modified .inputsMultiIdioma.basicInfo input[name="TabName"]:not(":disabled")'); // $('.row.pestanya:not(".ui-state-disabled") .modified input[name = "TabName"]:not(":disabled")');
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
        
        //const panMultiIdioma = $('#edicion_multiidioma', inputName.parent());
        // Panel multiIdioma donde se encuentra el input
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
            let textoIdiomaDefecto = panMultiIdioma.find(`#input_${inputId}_${operativaMultiIdioma.idiomaPorDefecto}`).val(); //panMultiIdioma.find('#edicion_' + that.IdiomaPorDefecto + ' input').val();
            
            if (textoIdiomaDefecto == null || textoIdiomaDefecto == "") {
                const fila = inputName.closest(".component-wrap.page-row");
                that.mostrarErrorNombreVacio(fila, panMultiIdioma.find(`#input_${inputId}_${operativaMultiIdioma.idiomaPorDefecto}`));
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
     * @param {jqueryObject} fila : Elemento jquery de la fila correspondiente a donde se ha encontrado el error.
     * @param {jqueryObject} input : Elemento jquery del input donde se ha producido el error. Puede que ser undefined
     */
    mostrarErrorNombreVacio: function(fila, input){        
        /*var inputUrl = $('input[name = "TabName"]', fila).first();*/                
        const btnEditPage = $(".btnEditPage", fila);
        // Abrir el modal para acceder a modificar el nombre de la página
        btnEditPage.trigger( "click" );

        if (input != undefined){
            comprobarInputNoVacio(input, true, false, "El nombre de la página no puede estar vacío.", 0);
        }

        setTimeout(function(){  
            mostrarNotificacion("error", "El nombre de la página no puede estar vacío.");
        },1000);  
    },

    /**
     * Método para comprobar y recoger los textos por defecto para el buscador de una página semántica
     * @returns bool: Devuelve un valor booleano indicando si todo está OK para proceder con el guardado de las páginas.
     */    
    comprobarTextosBuscadorPorDefecto: function(){
        const that = this;        
        // Inputs con los nombres de todas las páginas que hayan sido modificadas y no borradas (Más velocidad)        
        const inputsTextoBuscadorPorDefecto = $('.page-row:not(".deleted").modified .inputsMultiIdioma.basicInfo input[name="TabFiltroTextoDefectoBuscador"]:not(":disabled")');
        inputsTextoBuscadorPorDefecto.each(function () {            
            that.comprobarTextoBuscadorPorDefecto($(this));              
        });
        // Nunca habrá error ya que el texto puede ser vacío
        return false;
    },

     /**
     * Método para comprobar que el nombre de una página no está vacío.
     * @param {*} inputTextoDefecto: Input a comprobar
     * @returns bool: Devuelve un valor booleano indicando si todo está OK para proceder con el guardado de las páginas.
     */
    comprobarTextoBuscadorPorDefecto: function(inputTextoDefecto){
        const that = this
        
        // Panel multiIdioma donde se encuentra el input
        // Contiene los tabs en idiomas y el div con los inputs en idiomas completo
        const formularioEdicion = inputTextoDefecto.closest(".formulario-edicion");
        const panInputsMultiIdioma = inputTextoDefecto.closest(".inputsMultiIdioma");
        // Contenedor donde se encuentran datos básicos de la página (Nombre, Url/Ruta, MetaDescripción )
        const panMultiIdioma = formularioEdicion.find(".panContenidoMultiIdioma.basicInfo");

        // Id del input a tratar
        const inputId = inputTextoDefecto.attr("id");

        const listaTextos = [];
        
        if (operativaMultiIdioma.listaIdiomas.length > 1 && panMultiIdioma.length > 0) {
            let textoMultiIdioma = "";
            // Comprobar que hay al menos un texto por defecto para el texto
            let textoIdiomaDefecto = panMultiIdioma.find(`#input_${inputId}_${operativaMultiIdioma.idiomaPorDefecto}`).val(); //panMultiIdioma.find('#edicion_' + that.IdiomaPorDefecto + ' input').val();

            // Recorrer todos los idiomas para detectar posibles problemas con el text                
            $.each(operativaMultiIdioma.listaIdiomas, function () {
                // Obtención del Key del idioma
                const idioma = this.key;
                // Asignar el valor por defecto de la ruta al idioma si este no dispone de valor para texto
                let textoIdioma = $(`#input_${inputId}_${idioma}`).val();
                if (textoIdioma == null || textoIdioma == "") {
                    textoIdioma = textoIdiomaDefecto;
                    $(`#input_${inputId}_${idioma}`).val(textoIdioma);
                }
                // Escribir el texto del multiIdioma en el campo Hidden
                textoMultiIdioma += textoIdioma + "@" + idioma + "|||";
                
                listaTextos.push({ "key": idioma, "value": textoIdioma });
            });            
            inputTextoDefecto.val(textoMultiIdioma);
        }else{
            // Sin multiIdioma.
            let textoIdiomaDefecto = panMultiIdioma.find(`#input_${inputId}_${operativaMultiIdioma.idiomaPorDefecto}`).val();
            // Establecer el nombre en el input correspondiente
            inputTextoDefecto.val(textoIdiomaDefecto);
        }
        return false;
    },


    /**
     * Método para comprobar que el corto de una página no está repetido.
     * @returns bool: Devuelve un valor booleano indicando si todo está OK para proceder con el guardado de las páginas.
     */
    comprobarNombresCortosRepetidos: function(){
        const that = this;
        let errorRepetido = false;
        
        // var inputsNombresCortos = $('.row.pestanya:not(".ui-state-disabled") .modified input[name = "TabShortName"]');
        //const inputsNombresCortos = $('.page-row:not(".deleted") input[name="TabShortName"]:not(":disabled")');
        // Inputs con nombres cortos de páginas que hayan sido modificadas y no borradas (Más velocidad)        
        const inputsNombresCortos = $('.page-row:not(".deleted").modified input[name="TabShortName"]:not(":disabled")');
        
        inputsNombresCortos.each(function () {
            if (that.comprobarNombreCortoRepetido($(this))) {
                errorRepetido = true;
            }
        });
        return errorRepetido;        
    },

    /**
     * Método para comprobar el corto de una determinada página no está repetido.
     * @param {jqueryObject} inputNombreCorto : Input a comprobar
     * @returns bool: Devuelve un valor booleano indicando si todo está OK para proceder con el guardado de las páginas.
     */
    comprobarNombreCortoRepetido: function (inputNombreCorto) {
        var that = this;
        $('.error', inputNombreCorto.parent()).remove();

        var fila = inputNombreCorto.closest('.row');

        var inputsNombresCortos = $('.row.pestanya:not(".ui-state-disabled") input[name = "TabShortName"]');
        var nombreCorto = inputNombreCorto.val();
        var errorRepetido = false;
        var inputRepetido = null;

        if (nombreCorto != "") {
            inputsNombresCortos.each(function () {
                var inputCompare = $(this);
                if (inputCompare.closest('.row').attr('id') != fila.attr('id')) {
                    if (inputCompare.val() == nombreCorto) {
                        errorRepetido = true;
                        inputRepetido = inputCompare;
                    }
                }
            });
        }
        if (errorRepetido) {
            that.mostrarErrorNombreCortoRepetido(fila);
            if (inputRepetido != null) {
                var filaRepetidp = inputRepetido.closest('.row');
                that.mostrarErrorUrlRepetida(filaRepetida);
            }
        }
        return errorRepetido;
    },   
    
    /**
     * Método para mostrar el error debido a que el nombre corto de la página está repetido
     * @param {*} fila 
     */
    mostrarErrorNombreCortoRepetido: function (fila) {
        var inputNombreCorto = $('input[name = "TabShortName"]', fila).first();
        inputNombreCorto.after("<span class=\"error\">El Nombre Corto no puede estar repetido</span>");
        $('.panEdicion', fila).addClass('edit');
        $('.panEdicion', fila).addClass('modified');
    },  

    /**
     * Método para comprobar que la URL de una página no esté repetida. Recorrerá todas las filas buscando los campos URL de los idiomas, y los comparará con 
     * los idiomas del inputUrl.
     * @param {*} inputUrl : Input a comprobar
     * @returns bool: Devuelve un valor booleano indicando si todo está OK para proceder con el guardado de las páginas.
     */
    comprobarUrlRepetida: function(inputUrl){
        const that = this;
        // Panel multiIdioma donde se encuentra el input
        // Contiene los tabs en idiomas y el div con los inputs en idiomas completo
        const formularioEdicion = inputUrl.closest(".formulario-edicion");
        const panInputsMultiIdioma = inputUrl.closest(".inputsMultiIdioma");
        // Contenedor donde se encuentran datos básicos de la página (Nombre, Url/Ruta, MetaDescripción )
        const panMultiIdioma = formularioEdicion.find(".panContenidoMultiIdioma.basicInfo");

        // Id del input a tratar
        const inputId = inputUrl.attr("id");

        const listaTextos = [];

        // Comprobar que hay múltiples idiomas para edición de páginas
        if (operativaMultiIdioma.listaIdiomas.length > 1 && panMultiIdioma.length > 0) {
            let rutaPorDefecto = formularioEdicion.find(`#input_${inputId}_${operativaMultiIdioma.idiomaPorDefecto}`).val(); //panMultiIdioma.find('.edicion_' + that.IdiomaPorDefecto + ' input').val();
            if (rutaPorDefecto == "") {
                that.errorRutaVacia = true;
            }
            else {
                // Input oculto donde se crearán los idiomas asignados para una determinada página
                let textoMultiIdioma = "";
                $.each(operativaMultiIdioma.listaIdiomas, function () {
                    // Obtención del Key del idioma
                    const idioma = this.key;
                    // Acceso al valor del input del idioma analizado
                    //let textoIdioma = panMultiIdioma.find('#edicion_' + idioma + ' input').val();
                    let textoIdioma = $(`#input_${inputId}_${idioma}`).val();
                    // Asignar el valor por defecto de la ruta al idioma si este no dispone de valor para URL
                    if (textoIdioma == null || textoIdioma == "") {
                        textoIdioma = rutaPorDefecto;                        
                        $(`#input_${inputId}_${idioma}`).val(textoIdioma);
                    }
                    // Escribir el nombre del multiIdioma en el campo Hidden
                    textoMultiIdioma += textoIdioma + "@" + idioma + "|||";
                
                    listaTextos.push({ "key": idioma, "value": textoIdioma });
                });
                inputUrl.val(textoMultiIdioma);
            }
        }else{
            // Sin multiIdioma.
            let textoIdiomaDefecto = panMultiIdioma.find(`#input_${inputId}_${operativaMultiIdioma.idiomaPorDefecto}`).val();
            // Establecer el nombre en el input correspondiente
            inputUrl.val(textoIdiomaDefecto);
        }
        // Fila correspondiente a la página cuyo valor URL está siendo analizada
        const fila = inputUrl.closest('.component-wrap.page-row');

        // Comprobar si hay error por ruta vacía
        if (that.errorRutaVacia) {
            that.mostrarErrorUrlVacia(fila, formularioEdicion.find(`#input_${inputId}_${operativaMultiIdioma.idiomaPorDefecto}`));
            return that.errorRutaVacia;
        }

        // Comprobación de que las Rutas existentes de que no estárán repetidas
        //const inputsRutas = $('.page-row:not(".deleted") input[name="TabUrl"]'); // $('.row.pestanya:not(".ui-state-disabled") input[name = "TabUrl"]');
        // const inputsRutas = $('.page-row:not(".deleted") input[name="TabUrl"]'); // $('.row.pestanya:not(".ui-state-disabled") input[name = "TabUrl"]');
        // Obtener los inputs de las rutas de las páginas menos las disabled (Página Home)
        const inputsRutas = $('.page-row:not(".deleted") input[name="TabUrl"]:not(":disabled")');
        // Obtención de la ruta
        const ruta = inputUrl.val();
        const tipoRuta = $("input[name='TabType']", fila).val();
        let errorRepetida = false;
        let inputRepetida = null;

        // Comprobación del tipo de Enlace
        if (tipoRuta != "EnlaceInterno" && tipoRuta != "EnlaceExterno") {
            if (that.errorRutaRepetida == false){
                inputsRutas.each(function () {
                    // Input a comparar
                    const inputCompare = $(this);
                    // Id del input a comparar
                    const inputCompareId = $(this).attr("id");
                    // Fila que se comparará
                    const filaCompare = inputCompare.closest('.component-wrap.page-row');
                    // Tipo de ruta a comparar
                    const tipoRutaCompare = $("input[name='TabType']", filaCompare).val();
    
                    if (filaCompare.attr('id') != fila.attr('id') && tipoRutaCompare != "EnlaceExterno" && tipoRutaCompare != "EnlaceInterno") {
                        // Panel donde se muestran los datos en los diferentes idiomas de cada página
                        //const panCompareMultiIdioma = $('.inputsMultiIdioma', inputCompare.parent().parent());
                        const panCompareMultiIdioma = inputCompare.parent().parent();
                        
                        if (operativaMultiIdioma.listaIdiomas.length > 1 && panCompareMultiIdioma.length > 0) {
                            // Revisar el input en cada idioma para que no estén repetidas    
                            $.each(operativaMultiIdioma.listaIdiomas, function () {
                                const idioma = this.key;                             
                                // Acceder a los inputs en todos los idiomas
                                const formularioEdicion = inputUrl.closest(".formulario-edicion");
                                const panMultiIdioma = formularioEdicion.find(".panContenidoMultiIdioma.basicInfo");
                                // Contenido del idioma a revisar
                                let textoIdioma = "";
                                // Obtener campos URL diferentes a pagina de tipo Home -> Home no puede cambiar la url
                                if (inputCompareId != undefined){
                                    textoIdioma = $(`#input_${inputCompareId}_${idioma}`).val();
                                    // Establecer en el idioma la url con el idioma por defecto
                                    if (textoIdioma == null || textoIdioma == "") {
                                        // Establecer el valor del idioma por defecto en el idioma que no contiene un valor URL
                                        textoIdioma = $(`#input_${inputCompareId}_${operativaMultiIdioma.idiomaPorDefecto}`).val();
                                        $(`#input_${inputCompareId}_${idioma}`).val(textoIdioma);
                                    }
                                    // Buscar si ya existe la url insertada
                                    if (listaTextos.findValueByKey(idioma) == textoIdioma) {
                                        errorRepetida = true;
                                        inputRepetida = inputCompare;
                                        return;
                                    }
                                }
                            });
                        }
                        else if (operativaMultiIdioma.listaIdiomas.length > 1) {
                            if (inputCompare.val() == "") {
                                $.each(operativaMultiIdioma.listaIdiomas, function () {
                                    let idioma = this.key
                                    if (listaTextos.findValueByKey(idioma) == "") {
                                        errorRepetida = true;
                                        inputRepetida = inputCompare;
                                    }
                                });
                            }
                            else {
                                let textoMultiIdioma = inputCompare.val().split('|||');
    
                                $.each(textoMultiIdioma, function () {
                                    if (this != "") {
                                        /*var objetoIdioma = that.obtenerTextoYClaveDeIdioma(this);
                                        */
                                        if (listaTextos.findValueByKey(objetoIdioma.key) == objetoIdioma.value) {
                                            errorRepetida = true;
                                            inputRepetida = inputCompare;
                                        }
                                    }
                                });
                            }
                        }
                        else {
                            if (inputCompare.val() == ruta) {
                                errorRepetida = true;
                                inputRepetida = inputCompare;
                            }
                        }
                    }
                });
            }
        }
        if (errorRepetida) {
            that.errorRutaRepetida = true;
            that.mostrarErrorUrlRepetida(fila);
            /* Abrir la página con la que está repetida
                if (inputRepetida != null) {
                var filaRepetida = inputRepetida.closest('.component-wrap.page-row');
                that.mostrarErrorUrlRepetida(filaRepetida);
            }*/
        }
        return errorRepetida;
    },

    /**
     * Método para mostrar el error debido a que la URL de una página está repetida
     * @param {jqueryObject} fila : Fila correspondiente al input donde se ha encontrado el error
     */
    mostrarErrorUrlRepetida: function (fila) {
        // var inputUrl = $('input[name = "TabUrl"]', fila).first();
        // inputUrl.after("<span class=\"error\">La Ruta no puede estar repetida</span>");
        // Botón de la página para editar detalles
        const btnEditPage = $(".btnEditPage", fila);
        // Abrir el modal para acceder a modificar la URL de la página
        // btnEditPage.trigger( "click" );
        setTimeout(function(){
            mostrarNotificacion("error", "La ruta de la página no puede estar repetida.");
        },1000);        
    },  
    
    /**
     * Método para mostrar el error debido a que la URL de una página está vacía.
     * @param {*} fila 
     * @param {*} input: Input correspondiente donde se ha encontrado error por estar vacío.
     */
    mostrarErrorUrlVacia: function (fila, input) {
        const btnEditPage = $(".btnEditPage", fila);
        // Abrir el modal para acceder a modificar la URL de la página
        // btnEditPage.trigger( "click" );

        if (input != undefined){
            comprobarInputNoVacio(input, true, false, "La ruta de la página no puede estar vacía.", 0);
        }

        setTimeout(function(){
            mostrarNotificacion("error", "La ruta de la página no puede estar vacía.");
        },1000);          
    },    
    
    /**
     * Método para comprobar el orden de los filtros de orden de las páginas
     * @returns
     */
    comprobarFiltrosOrden: function(){
        const that = this;
                
        //const filtros = $('.page-row:not(".deleted") .filter-row:not(".deleted")'); // $('.row.pestanya:not(".ui-state-disabled") .modified ol.filtrosOrdenSortable li.row.filtroOrden:not(".ui-state-disabled")');
        // Filtro de orden de todas las páginas a revisar que hayan sido modificadas y no borradas (Más velocidad)
        const filtros = $('.page-row:not(".deleted").modified .filter-row:not(".deleted")');

        filtros.each(function () {
            if (that.errorFiltrosOrden == false){
                that.comprobarFiltroOrden($(this));                
            }
        });
        return that.errorFiltrosOrden;
    },


    /**
     * Método para comprobar el orden del filtro de una determinada página.
     * @param {jqueryObject} filaFiltroOrden : Objeto jquery que corresponde con la fila donde se encuentra el filtro
     * @returns : bool: Devuelve un valor que indica si se puede proceder al guardado de las páginas.
     */
    comprobarFiltroOrden: function(filaFiltroOrden){
        const that = this
        
        // Panel donde se podrá localizar el input que almacena el valor del input para el filtroOrden
        const panelOrderFilterInfo = filaFiltroOrden.find('.filter-order-info');
        // Filas de páginas
        const filasPaginas = $("#id-added-pages-list").find(".component-wrap.page-row");
        // Fila de la página cuyos filtros de orden están siendo revisados
        const filaPagina = panelOrderFilterInfo.closest(filasPaginas);

        // Inputs que contendrá todos los idiomas del filtro en concreto y el nombre del filtro
        const inputFilterValueLanguages = panelOrderFilterInfo.find('[name="TabName"]');
        const inputId = inputFilterValueLanguages.attr('id');
        const inputFilterName = panelOrderFilterInfo.find('[name="Filtro"]');


        let textoMultiIdioma = "";
        // Comprobar que hay al menos un texto por defecto para el nombre del filtro
        let textoIdiomaDefecto = $(`#input_${inputId}_${operativaMultiIdioma.idiomaPorDefecto}`).val();
        if (textoIdiomaDefecto == null || textoIdiomaDefecto == "") {
            const fila = filaFiltroOrden.closest(".component-wrap.page-row");
            that.mostrarErrorCampoFiltroOrdenVacio(filaFiltroOrden, filaPagina, $(`#input_${inputId}_${operativaMultiIdioma.idiomaPorDefecto}`));
            that.errorFiltrosOrden = true;
            return that.errorFiltrosOrden;
        }
        
        // Recorrer el contenido de los inputs en los diferentes idiomas al input correspondiente
        $.each(operativaMultiIdioma.listaIdiomas, function () {
            // Obtención del Key del idioma
            const idioma = this.key;
            // Asignar el valor por defecto de la ruta al idioma si este no dispone de valor para el Nombre del filtro
            let textoIdioma = $(`#input_${inputId}_${idioma}`).val();
            if (textoIdioma == null || textoIdioma == "") {
                textoIdioma = textoIdiomaDefecto;
                $(`#input_${inputId}_${idioma}`).val(textoIdioma);
            }
            // Escribir el nombre del multiIdioma en el campo Hidden
            textoMultiIdioma += textoIdioma + "@" + idioma + "|||";
        });            
        inputFilterValueLanguages.val(textoMultiIdioma);


        // Comprobar que el filtro orden tiene asignado nombre y valor
        if (inputFilterValueLanguages.val().trim() == "" || inputFilterName.val().trim() == "") {
            that.mostrarErrorCampoFiltroOrdenVacio(filaFiltroOrden, filaPagina, $(`#input_${inputId}_${operativaMultiIdioma.idiomaPorDefecto}`));
            that.errorFiltrosOrden = true;
            return that.errorFiltrosOrden;
        }
        return that.errorFiltrosOrden;
    },

    /**
     * Método para mostrar el error de que una página tiene un filtro de orden no tiene valor o no tiene nombre asignado.
     * @param {jqueryObject} filaFiltroOrden : Fila del Filtro orden que no tiene valor
     * @param {jqueryObject} filaPagina : Fila de la página que está siendo revisada
     */
    mostrarErrorCampoFiltroOrdenVacio: function (filaFiltroOrden, filaPagina, input){

        // Obtener el identificador de la página con error
        const pageId = filaPagina.attr("id");
        // Construcción del identificador del modal
        const modalId = `modal-configuracion-pagina_${pageId}`;

        // Botón para editar la página
        const btnEditPage = $(".btnEditPage", filaPagina);
        // Botón para editar el filtroOrden
        const btnEditFilter = $(".btnEditFilter", filaFiltroOrden);

        // Abrir el modal y el la fila del filtro que tiene errores para ser editados
        btnEditPage.trigger( "click" );       
       
        // Mostrar el mensaje del error en el input correspondiente
        if (input != undefined){
            comprobarInputNoVacio(input, true, false, "El nombre y el filtro no pueden estar vacios.", 0);
        }          

        setTimeout(function(){ 
            // Realizar scroll hasta el elemento indicado
            scrollInModalView(btnEditFilter, modalId, function(){                
                btnEditFilter.trigger( "click" );
                mostrarNotificacion("error", "El nombre y el filtro no pueden estar vacios.");
            });            
        },1000);  
  
    },

    /**
     * Método para mostrar el error de que una página (de tipo búsqueda semántica) el campo filtro está vacío.
     * @param {String} pageId : Id de la página que está siendo tratada
     * @param {jqueryObject} input : Input donde que ha producido el error.
     */
     mostrarErrorCampoFiltroVacio: function (pageId, input){

        // Obtener el identificador de la página con error
        const filaPagina = $(`.page-row#${pageId}`);
        // Construcción del identificador del modal
        const modalId = `modal-configuracion-pagina_${pageId}`;

        // Botón para editar la página
        const btnEditPage = $(".btnEditPage", filaPagina);

        // Abrir el modal y el la fila del filtro que tiene errores para ser editados
        btnEditPage.trigger( "click" );  

        // Mostrar el mensaje del error en el input correspondiente
        if (input != undefined){            
            comprobarInputNoVacio(input, true, false, "El Campo Filtro de la página no puede estar vacío", 0);
        }          

        setTimeout(function(){ 
            // Realizar scroll hasta el elemento indicado
            scrollInModalView(input, modalId, function(){                                
                mostrarNotificacion("error", "El Campo Filtro de la página no puede estar vacío.");
            });            
        },1000);  

    },    


    
    /**
     * Método para comprobar las exportaciones existentes en las páginas
     * @returns bool: Devuelve valor que indica si se puede proceder al guardado de páginas
     */
    comprobarExportaciones: function(){
        const that = this;

        // Exportaciones de todas las páginas a revisar que hayan sido modificadas y no borradas (Más velocidad)
        const exportaciones = $('.page-row:not(".deleted").modified .exportation-row:not(".deleted")');
        
        exportaciones.each(function () {
            if (that.errorExportaciones == false){
                that.comprobarExportacion($(this))
            }
        });
        return that.errorExportaciones;
    },


    /**
    * Método para comprobar las exportaciones creadas de las páginas
    * @param {jqueryObject} filaExportacion : Objeto jquery que corresponde con la fila donde se encuentra el la exportación a revisar
    * @returns : bool: Devuelve un valor que indica si se puede proceder al guardado de las páginas.
    */
    comprobarExportacion: function(filaExportacion){
        const that = this
        
        // Panel donde se podrá localizar el nombre de la exportación a revisar
        const panelExportationInfo = filaExportacion.find('.exportation-info');
        // Filas de páginas
        const filasPaginas = $("#id-added-pages-list").find(".component-wrap.page-row");
        // Fila de la página cuyos filtros de orden están siendo revisados
        const filaPagina = panelExportationInfo.closest(filasPaginas);

        // Input del nombre de la exportación
        const inputExportationName = panelExportationInfo.find('[name="Nombre"]').first();                

        // Comprobar que el Nombre de la exportación NO está vacío
        if (inputExportationName.val().trim() == "") {
            that.mostrarErrorNombreExportacionVacio(filaPagina, filaExportacion, inputExportationName);
            that.errorExportaciones = true;
            return true;
        }
        
        // Comprobación de que hay al menos 1 propiedad asociada a la fila exportación que se está revisando
        const propiedades = filaExportacion.find(".exportation-info").find('.property-row:not(".deleted")');
                
        if (propiedades.length == 0) {
            that.mostrarErrorExportacionSinPropiedades(filaPagina, filaExportacion);
            that.errorExportaciones = true;
            return true;
        }

        // Comprobación de que si hay propiedades, estás contiene datos válidos
        propiedades.each(function () {
            if (that.comprobarPropiedadExportacion($(this))) {
                that.errorExportaciones = true;
                return;
            }
        });
        return that.errorExportaciones;
    },

    /**
     * Método para mostrar el error cuando el nombre de una exportación está vacío. La comprobación se hace al pulsar en "Guardar" las páginas
     * @param {jqueryObject} filaPagina : Fila de la página que está siendo revisada
     * @param {jqueryObject} filaExportacion : Fila de la exportación que ha dado el error
     * @param {*} inputExportationName : Input que se ha encontrado con valor vacío
     */
    mostrarErrorNombreExportacionVacio: function(filaPagina, filaExportacion, inputExportationName){
        // Filas de páginas
        const filasPaginas = $("#id-added-pages-list").find(".component-wrap.page-row");

        // Obtener el identificador de la página con error
        const pageId = filaPagina.attr("id");
        // Construcción del identificador del modal
        const modalId = `modal-configuracion-pagina_${pageId}`

        // Botón para editar la página
        const btnEditPage = $(".btnEditPage", filaPagina);
        // Botón para editar la exportación
        const btnEditExportation = $(".btnEditExportation", filaExportacion);  
        
        // Abrir el modal y los paneles donde se encuentra el error
        btnEditPage.trigger( "click" );
        btnEditExportation.trigger("click");        

        // Mostrar el mensaje del error en el input correspondiente
        if (inputExportationName != undefined){
            comprobarInputNoVacio(inputExportationName, true, false, "Los nombres de las exportaciones no pueden estar vacíos.", 0);
        }

        setTimeout(function(){ 
           
            // Realizar scroll hasta el elemento indicado
            scrollInModalView(btnEditExportation, modalId, function(){                
                mostrarNotificacion("error", "Los nombres de las exportaciones no pueden estar vacíos.");
            });            
        },1000);  
    },

    /**
     * Método para mostrar el error cuando una exportació no tiene asigandas propiedades
     * @param {jqueryObject} filaPagina : Fila de la página que está siendo revisada
     * @param {jqueryObject} filaExportacion : Fila de la exportación que ha dado el error
     */
    mostrarErrorExportacionSinPropiedades: function(filaPagina, filaExportacion){

        // Filas de páginas
        const filasPaginas = $("#id-added-pages-list").find(".component-wrap.page-row");

        // Obtener el identificador de la página con error
        const pageId = filaPagina.attr("id");
        // Construcción del identificador del modal
        const modalId = `modal-configuracion-pagina_${pageId}`

        // Botón para editar la página
        const btnEditPage = $(".btnEditPage", filaPagina);
        // Botón para editar la exportación
        const btnEditExportation = $(".btnEditExportation", filaExportacion);  
        
        // Abrir el modal y los paneles donde se encuentra el error
        btnEditPage.trigger( "click" );
        btnEditExportation.trigger("click");        

        setTimeout(function(){ 
            // Realizar scroll hasta el elemento indicado
            scrollInModalView(btnEditExportation, modalId, function(){                
                mostrarNotificacion("error", "Las exportaciones deben tener por lo menos una propiedad.");
            });            
        },1000);  

    },

    /**
     * Método para comprobar que la propiedad asignada a una exportación es válida (Dispone de Nombre y de propiedad)
     * @param {jqueryObject} filaPropiedad : Fila de la propiedad que va a ser revisada
     */
    comprobarPropiedadExportacion: function(filaPropiedad){
        const that = this
        // Fila de exportación que está siendo revisada donde se contiene toda la información
        const panelExportationInfo = filaPropiedad.find('.property-info');

        // Inputs de información de la exportación        
        // Input del nombre de la exportación
        const inputPropertyName = panelExportationInfo.find('[name="Nombre"]');
        // Input con el nombre de la propiedad asociada a dicha propiedad
        const inputPropertyPropertyName = panelExportationInfo.find('[name="Propiedad"]');
        
        if (inputPropertyName.val().trim() == "" || inputPropertyPropertyName.val().trim() == "") {
            that.mostrarErrorCampoPropiedadVacio(filaPropiedad, inputPropertyPropertyName);
            return true;
        }

        return false;        
    },
    
    /**
     * Método para mostrar el error de que el campo de una propiedad no puede estar vacío.
     * @param {*} filaPropiedad 
     * @param {*} input 
     */
    mostrarErrorCampoPropiedadVacio: function(filaPropiedad, input){
        // Filas de páginas
        const filasPaginas = $("#id-added-pages-list").find(".component-wrap.page-row");
        // Fila de la página cuyas propiedades están con el error
        const filaPagina = filaPropiedad.closest(filasPaginas);
        const filaExportation = filaPropiedad.closest(".exportation-row");
        // Obtener el identificador de la página con error
        const pageId = filaPagina.attr("id");
        // Construcción del identificador del modal
        const modalId = `modal-configuracion-pagina_${pageId}`
        
        // Botón para editar la página
        const btnEditPage = $(".btnEditPage", filaPagina);
        // Botón para editar la exportación
        const btnEditExportation = $(".btnEditExportation", filaExportation);        
        // Botón para editar la propiedad de la exportación
        const btnEditProperty = $(".btnEditProperty", filaPropiedad);
        
        // Abrir el modal y los paneles donde se encuentra el error
        btnEditPage.trigger( "click" );
        btnEditExportation.trigger("click");
        btnEditProperty.trigger( "click" );

        if (input != undefined){
            comprobarInputNoVacio(input, true, false, "Los campos de las propiedades no pueden estar vacios.", 0);
        }

        setTimeout(function(){ 
            // Realizar scroll hasta el elemento indicado
            scrollInModalView(btnEditExportation, modalId, function(){                
                mostrarNotificacion("error", "Los campos de las propiedades no pueden estar vacios.");
            });            
        },1000);         
    },

    /**
     * Método para comprobar que no hay Metadescripciones vacíos en las páginas creadas/editadas     
     */
    comprobarMetadescripciones: function(){
        const that = this;        
        // Inputs con las metadescripciones de todas las páginas
        // const inputsMetadescripcion = $('.page-row:not(".deleted") .inputsMultiIdioma.basicInfo input[name="TabMetaDescription"]:not(":disabled")');
        // Textareas con las metadescripciones 
        const inputsMetadescripcion = $('.page-row:not(".deleted") .inputsMultiIdioma.basicInfo [name="TabMetaDescription"]:not(":disabled")');
        inputsMetadescripcion.each(function () {
            that.comprobarMetadescripcion($(this));            
        });        
    },

    /**
     * Método para validar la metadescripción de una página. Se aplicará la metadescripción del idioma por defecto para todos los idiomas si no hay una establecida.
     * @param {*} inputMetadescription: Input a comprobar
     * @returns bool: Devuelve un valor booleano indicando si todo está OK para proceder con el guardado de las páginas.
     */
    comprobarMetadescripcion: function(inputMetadescription){  
        const that = this

        // Contiene los tabs en idiomas y el div con los inputs en idiomas completo
        const formularioEdicion = inputMetadescription.closest(".formulario-edicion");
        const panInputsMultiIdioma = inputMetadescription.closest(".inputsMultiIdioma");
        // Contenedor donde se encuentran datos básicos de la página (Nombre, Url/Ruta, MetaDescripción )
        const panMultiIdioma = formularioEdicion.find(".panContenidoMultiIdioma.basicInfo");

        // Id del input a tratar
        const inputId = inputMetadescription.attr("id");

        const listaTextos = [];
        
        if (operativaMultiIdioma.listaIdiomas.length > 1 && panMultiIdioma.length > 0) {
            let textoMultiIdioma = "";
            // Seleccionar el texto del idioma por defecto para la metadescripción
            let textoIdiomaDefecto = panMultiIdioma.find(`#input_${inputId}_${operativaMultiIdioma.idiomaPorDefecto}`).val();
            
            // Recorrer todos los idiomas para asignarlos al input correspondiente. Si algún idioma no tiene metadescripción, asignarle la del idioma por defecto
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
            inputMetadescription.val(textoMultiIdioma);
        }else{
            // Sin multiIdioma.
            let textoIdiomaDefecto = panMultiIdioma.find(`#input_${inputId}_${operativaMultiIdioma.idiomaPorDefecto}`).val();
            // Establecer el nombre en el input correspondiente
            inputMetadescription.val(textoIdiomaDefecto);
        }
        return false;
    },
    

    /**
     * 
     */

    /**
     * Método para obtener los datos de cada página para proceder a su guardado. Se ejecuta desde 'handleSavePages'
     * @param {*} fila : Fila de la que hay que recoger los datos
     * @param {*} num : Nº de la página que se está analizando     
     */
    obtenerDatosPestanya: function(fila, num){        
        const that = this;
        // Id de la página
        const id = fila.attr('id');
        // Contenido o datos de la página dentro del modal
        // const panelEdicion = fila.find('.modal-page').last();
        const panelEdicion = fila.find(`#modal-configuracion-pagina_${id}`);
        // Prefijo para guardado de la pestaña/página
        const prefijoClave = 'ListaPestanyas[' + num + ']';

        // Recogida de datos principales de la página 
        that.ListaPestanyas[prefijoClave + '.Key'] = fila.attr('id');
        that.ListaPestanyas[prefijoClave + '.Url'] = panelEdicion.find('[name="TabUrl"]').val();
        that.ListaPestanyas[prefijoClave + '.EsUrlPorDefecto'] = panelEdicion.find('[name="TabUrl"]').is(':disabled');
        that.ListaPestanyas[prefijoClave + '.ShortName'] = panelEdicion.find('[name="TabShortName"]').val();
        that.ListaPestanyas[prefijoClave + '.Type'] = panelEdicion.find('[name="TabType"]').val();
        that.ListaPestanyas[prefijoClave + '.Deleted'] = panelEdicion.find('[name="TabEliminada"]').val();
        that.ListaPestanyas[prefijoClave + '.ParentTabKey'] = panelEdicion.find('[name="ParentTabKey"]').val();
        that.ListaPestanyas[prefijoClave + '.Order'] = num; //panelEdicion.find('[name="TabOrden"]').val();
        that.ListaPestanyas[prefijoClave + '.FechaModificacion'] = panelEdicion.find('[name="TabFechaModificacion"]').val();

        // Indicador de si la página ha sido editada / recién creada
        const modified = fila.hasClass("modified");
        that.ListaPestanyas[prefijoClave + '.Modified'] = modified;

        // Recoger los datos de la página que haya podido sufrir cambios
        if (modified)
        {
            // Recoger datos básicos de la página
            that.ListaPestanyas[prefijoClave + '.Name'] = panelEdicion.find('[name="TabName"]').val();
            that.ListaPestanyas[prefijoClave + '.EsNombrePorDefecto'] = panelEdicion.find('[name="TabName"]').is(':disabled');
            that.ListaPestanyas[prefijoClave + '.OpenInNewWindow'] = panelEdicion.find(`#TabOpenInNewWindow_SI_${fila.attr('id')}`).is(':checked'); // panelEdicion.find('[name="TabOpenInNewWindow"]').is(':checked');
            that.ListaPestanyas[prefijoClave + '.ClassCSSBody'] = panelEdicion.find('[name="TabClassCSSBody"]').val();
            that.ListaPestanyas[prefijoClave + '.MetaDescription'] = panelEdicion.find('[name="TabMetaDescription"]').val();
            
            that.ListaPestanyas[prefijoClave + '.Active'] = panelEdicion.find(`#TabActive_SI_${fila.attr('id')}`).is(':checked'); // panelEdicion.find('[name="TabActive"]').is(':checked');
            that.ListaPestanyas[prefijoClave + '.Visible'] = panelEdicion.find(`#TabVisible_SI_${fila.attr('id')}`).is(':checked'); // panelEdicion.find('[name="TabVisible"]').is(':checked');

            // Comprobación del tipo de Privacidad
            that.ListaPestanyas[prefijoClave + '.Privacidad'] = panelEdicion.find('[name="TabPrivacidad"]').val();

            // Comprobación de detalles de Privacidad (¿Existe Panel de privacidad?)
            let visibleSinAcceso = undefined;
            if (panelEdicion.find('.edit-privacy').length > 0){
                // Input 'Html alternativo'
                that.ListaPestanyas[prefijoClave + '.HtmlAlternativoPrivacidad'] = encodeURIComponent(panelEdicion.find('[name="TabHtmlAlternativoPrivacidad"]').val().replace(/\n/g, ''));
                const tabVisibleSinAccesoFila = `TabVisibleSinAcceso_${fila.attr('id')}`;
                // Input 'Visible en el menú para usuarios sin acceso'
                visibleSinAcceso = $("input[name="+tabVisibleSinAccesoFila+"]:checked").data("value") == "si" ? true : false;                                        

                // Comprobación de Perfiles/Grupos para la Privacidad de la página
                if (panelEdicion.find('[name="TabPrivacidad"]').val() == '2') {

                    // Añadir perfiles de usuarios privacidad de la página
                    const privacidadPerfiles = panelEdicion.find('[name="TabValoresPrivacidadPerfiles"]').val().split(',');
                    for (let i = 0; i < privacidadPerfiles.length; i++) {
                        if (privacidadPerfiles[i] != "") {
                            const prefijoPrivacidadPerfiles = prefijoClave + '.PrivacidadPerfiles[' + i + ']';
                            that.ListaPestanyas[prefijoPrivacidadPerfiles + '.Key'] = privacidadPerfiles[i];
                            that.ListaPestanyas[prefijoPrivacidadPerfiles + '.Value'] = "";
                        }
                    }

                    // Añadir perfiles de grupos de usuarios para la privacidad de la página
                    const privacidadGrupos = panelEdicion.find('[name="TabValoresPrivacidadGrupos"]').val().split(',');
                    for (let i = 0; i < privacidadGrupos.length; i++) {
                        if (privacidadGrupos[i].trim() != "") {
                            var prefijoPrivacidadGrupos = prefijoClave + '.PrivacidadGrupos[' + i + ']';
                            that.ListaPestanyas[prefijoPrivacidadGrupos + '.Key'] = privacidadGrupos[i].substr(2);
                            that.ListaPestanyas[prefijoPrivacidadGrupos + '.Value'] = "";
                        }
                    }
                }
            }

            // Establecer valor de 'Visible en el menú para usuarios sin acceso'
            that.ListaPestanyas[prefijoClave + '.VisibleSinAcceso'] = visibleSinAcceso;

            // Input TabHomeCMS para "Editar la Home con el CMS" 
            const tabHomeCMSName = `TabHomeCMS_${fila.attr('id')}`; 
            const tabHomeCheckBox = $("input[name="+tabHomeCMSName+"]");
            
            if (tabHomeCheckBox.length > 0){
                const homeCMS = $("input[name="+tabHomeCMSName+"]:checked").data("value") == "si" ? true : false;

                let homeTodosUsuarios = false,
                    homeMiembros = false,
                    homeNoMiembros = false;
                // Controlar si la página se desea para todos los usuarios
                if (homeCMS == true) {
                    const tipoHomeCMSName = `TabTypeHomeCMS_${fila.attr('id')}`;

                    const tipoHomeCMS = panelEdicion.find("[name="+ tipoHomeCMSName +"]:checked").data("value");
                    if (tipoHomeCMS == 'Unica') {
                        homeTodosUsuarios = true;
                    }
                    else {                                            
                        homeMiembros = panelEdicion.find(`[name="${that.checkboxTabHomeMiembrosCMSClassName}"]`).is(':checked');
                        homeNoMiembros = panelEdicion.find(`[name="${that.checkboxTabHomeNoMiembrosCMSClassName}"]`).is(':checked');
                    }
                }

                // Establecer valores para editar Home con CMS
                const prefijoHomeCMSClave = prefijoClave + '.HomeCMS';
                that.ListaPestanyas[prefijoHomeCMSClave + '.HomeTodosUsuarios'] = homeTodosUsuarios;
                that.ListaPestanyas[prefijoHomeCMSClave + '.HomeMiembros'] = homeMiembros;
                that.ListaPestanyas[prefijoHomeCMSClave + '.HomeNoMiembros'] = homeNoMiembros;
            }

            // Panel para editar opciones de búsqueda de la página (si dispone de la opción)
            const panelEditarBusqueda = panelEdicion.find('.editarOpcionesBusqueda');
            if (panelEditarBusqueda.length > 0) {
                // Prefijo para guardado en BD
                const prefijoBusquedaClave = prefijoClave + '.OpcionesBusqueda';

                that.ListaPestanyas[prefijoBusquedaClave + '.ValoresPorDefecto'] = 'true';
                // Seleccionar el campo Filtro de búsqueda
                if (panelEditarBusqueda.find('.auxFiltrosSeleccionados').val().trim() == ""){
                    that.errorDuranteObtenerDatosPestaya = true;
                    // Mostrar el error
                    const inputTabCampoFiltroError = panelEditarBusqueda.find('.auxFiltrosSeleccionados');
                    that.mostrarErrorCampoFiltroVacio(id, inputTabCampoFiltroError);
                    return;
                }
                
                that.ListaPestanyas[prefijoBusquedaClave + '.CampoFiltro'] = panelEditarBusqueda.find('.auxFiltrosSeleccionados').val();

                // Panel que contiene el orden de los filtros
                const panelFiltrosOrden = panelEdicion.find('.id-added-filter-list');
                // Comprobar si hay filtros de orden
                if (panelFiltrosOrden.length > 0) {
                    // Prefijo para guardado en BD
                    const prefijoFiltrosOrden = prefijoBusquedaClave + '.FiltrosOrden';
                    // Nº del filtro
                    let numFiltro = 0;                    
                    // Recorrer cada filtro para la obtención de sus datos
                    $('.filter-row', panelFiltrosOrden).each(function () {
                        // Panel de detalles del filtro
                        const panFiltro = $(this).find(".filter-order-info"); // $(this).children('.panEdicion');
                        // Prefijo para guardado en BD
                        const prefijoFiltroOrdenClave = prefijoFiltrosOrden + '[' + numFiltro + ']';
                        // Obtención de los datos del Filtro
                        that.ListaPestanyas[prefijoFiltroOrdenClave + '.Orden'] = numFiltro;
                        that.ListaPestanyas[prefijoFiltroOrdenClave + '.Deleted'] = panFiltro.find('[name="TabEliminada"]').val();
                        that.ListaPestanyas[prefijoFiltroOrdenClave + '.Nombre'] = panFiltro.find('[name="TabName"]').val();
                        that.ListaPestanyas[prefijoFiltroOrdenClave + '.Filtro'] = panFiltro.find('[name="Filtro"]').val();

                        numFiltro++;
                    });                    

                }

                // Establecer valores para búsquedas y Filtros de página
                that.ListaPestanyas[prefijoBusquedaClave + '.NumeroResultados'] = panelEditarBusqueda.find('[name="TabNumeroResultados"]').val();                                        
                // Input 'Mostrar facetas"
                const tabMostrarFacetasName = `TabMostrarFacetas_${fila.attr('id')}`;                
                that.ListaPestanyas[prefijoBusquedaClave + '.MostrarFacetas'] = $("input[name="+tabMostrarFacetasName+"]:checked").data("value") == "si" ? true : false; 
                // Input 'Agrupar facetas por tipo'
                const tabMostrarFacetasPorTipoName = `TabAgruparFacetasPorTipo_${fila.attr('id')}`;                
                that.ListaPestanyas[prefijoBusquedaClave + '.AgruparFacetasPorTipo'] = $("input[name="+tabMostrarFacetasPorTipoName+"]:checked").data("value") == "si" ? true : false; 
                // Input 'Mostrar página en el buscador de la cabecera' - Deprecado CORE-4941 - De momento lo pongo como true para el CICD
                const tabMostrarEnBusquedaCabeceraName = `TabMostrarEnBusquedaCabecera_${fila.attr('id')}`;                
                //that.ListaPestanyas[prefijoBusquedaClave + '.MostrarEnBusquedaCabecera'] = $("input[name="+tabMostrarEnBusquedaCabeceraName+"]:checked").data("value") == "si" ? true : false;                
                that.ListaPestanyas[prefijoBusquedaClave + '.MostrarEnBusquedaCabecera'] = true;                
                // Input 'Mostrar caja de busqueda' - Deprecado CORE-4941 - De momento lo pongo como true para el CICD
                const tabMostrarCajaBusquedaName = `TabMostrarCajaBusqueda_${fila.attr('id')}`;                
                // that.ListaPestanyas[prefijoBusquedaClave + '.MostrarCajaBusqueda'] = $("input[name="+tabMostrarCajaBusquedaName+"]:checked").data("value") == "si" ? true : false;                
                that.ListaPestanyas[prefijoBusquedaClave + '.MostrarCajaBusqueda'] = true;                
                
                // Input 'Proyecto origen de búsqueda'
                that.ListaPestanyas[prefijoBusquedaClave + '.ProyectoOrigenBusqueda'] = panelEditarBusqueda.find('[name="TabProyectoOrigenBusqueda"]').val();
                // Input 'Ocultar resultados sin filtros'
                const tabOcultarResultadosSinFiltrosName = `TabOcultarResultadosSinFiltros_${fila.attr('id')}`;                
                that.ListaPestanyas[prefijoBusquedaClave + '.OcultarResultadosSinFiltros'] = $("input[name="+tabOcultarResultadosSinFiltrosName+"]:checked").data("value") == "si" ? true : false;
                // Input 'Texto de busqueda sin resultados'
                that.ListaPestanyas[prefijoBusquedaClave + '.TextoBusquedaSinResultados'] = panelEditarBusqueda.find('[name="TabTextoBusquedaSinResultados"]').val();
                // Input 'Ignorar la privacidad en búsqueda'
                const tabIgnorarPrivacidadEnBusquedaName = `TabIgnorarPrivacidadEnBusqueda_${fila.attr('id')}`;                
                that.ListaPestanyas[prefijoBusquedaClave + '.IgnorarPrivacidadEnBusqueda'] = $("input[name="+tabIgnorarPrivacidadEnBusquedaName+"]:checked").data("value") == "si" ? true : false;
                // Input 'Omitir la carga inicial de facetas y resultados'
                const tabOmitirCargaInicialFacetasResultadosName = `TabOmitirCargaInicialFacetasResultados_${fila.attr('id')}`;                
                that.ListaPestanyas[prefijoBusquedaClave + '.OmitirCargaInicialFacetasResultados'] = $("input[name="+tabOmitirCargaInicialFacetasResultadosName+"]:checked").data("value") == "si" ? true : false;
                // Input 'Filtro de RelacionMandatory'
                that.ListaPestanyas[prefijoBusquedaClave + '.RelacionMandatory'] = panelEditarBusqueda.find('[name="TabFiltroRelacionMandatory"]').val();
                // Input 'Filtro de TextoDefectoBuscador'                
                that.ListaPestanyas[prefijoBusquedaClave + '.TextoDefectoBuscador'] = panelEdicion.find('[name="TabFiltroTextoDefectoBuscador"]').length > 0 ? panelEdicion.find('[name="TabFiltroTextoDefectoBuscador"]').val() : "";
                // Opciones de "Vistas disponibles" de resultados (Listado, Mosaico, Mapa...)
                // Prefijo para guardado en BD
                const prefijoBusquedaOpcionesVistasClave = prefijoBusquedaClave + '.OpcionesVistas';

                // Input de cual será la vista por defecto para visualizar los resultados
                that.ListaPestanyas[prefijoBusquedaOpcionesVistasClave + '.VistaPorDefecto'] = panelEditarBusqueda.find('[name="' + id + '_VistaPorDefecto"]:checked').val();
                // Input 'Vista Listado'
                that.ListaPestanyas[prefijoBusquedaOpcionesVistasClave + '.VistaListado'] = panelEditarBusqueda.find('[name="TabVistaListado"]').is(':checked');
                // Input 'Vista Mosaico'
                that.ListaPestanyas[prefijoBusquedaOpcionesVistasClave + '.VistaMosaico'] = panelEditarBusqueda.find('[name="TabVistaMosaico"]').is(':checked');
                // Input 'Vista Mapa'
                that.ListaPestanyas[prefijoBusquedaOpcionesVistasClave + '.VistaMapa'] = panelEditarBusqueda.find('[name="TabVistaMapa"]').is(':checked');
                // Input 'Vista Gráfico'
                that.ListaPestanyas[prefijoBusquedaOpcionesVistasClave + '.VistaGrafico'] = panelEditarBusqueda.find('[name="TabVistaGrafico"]').is(':checked');
                // Input datos de la vista "Mapa"
                that.ListaPestanyas[prefijoBusquedaOpcionesVistasClave + '.PosicionCentralMapa'] = panelEditarBusqueda.find('[name="TabPosicionCentralMapa"]').val();
                that.ListaPestanyas[prefijoBusquedaOpcionesVistasClave + '.PropiedadLatitud'] = ''; // No existe -> Manda undefined panelEditarBusqueda.find('[name="TabPropiedadLatitud"]').val();
                that.ListaPestanyas[prefijoBusquedaOpcionesVistasClave + '.PropiedadLongitud'] = ''; // No existe -> Manda undefined panelEditarBusqueda.find('[name="TabPropiedadLongitud"]').val();
            }  

            //TFG FRAN
            var panelOpcionesDashboard = panelEdicion.find('.editarOpcionesDashboard');
            if (panelOpcionesDashboard.length > 0) {
                var prefijoDashboardClave = prefijoClave + '.OpcionesDashboard';

                var panelAsistentes = panelEdicion.find('.editarAsistentes');
                if (panelAsistentes.length > 0) {
                    //Obtenemos los id de los ejemplos de los gráficos
                    var divGraficos = panelEdicion.find('.ejemplosGraficos');
                    var graficos = divGraficos.find('.generados');
                    var idGraficos = [];
                    var numAsistente = 0;
                    for (var i = 0; i < graficos.length; i++) {
                        idGraficos.push(graficos[i].id);
                    }

                    //Recorremos los asistentes que hayan sido añadidos
                    $('.asistente-row.asisAnyadido', panelAsistentes).each(function () {
                        var panAsistente = $(this).children('.panEdicion');
                        numAsistente = idGraficos.indexOf($(this).attr('id') + 'GrafPreview');
                        var prefijoAsistenteClave = prefijoDashboardClave + '[' + numAsistente + ']';

                        that.ListaPestanyas[prefijoAsistenteClave + '.AsisID'] = panAsistente.closest('.asistente-row').attr('id');
                        that.ListaPestanyas[prefijoAsistenteClave + '.Orden'] = numAsistente;
                        that.ListaPestanyas[prefijoAsistenteClave + '.Nombre'] = panAsistente.find('[name="nGrafico"]').val();
                        that.ListaPestanyas[prefijoAsistenteClave + '.Select'] = panAsistente.find('[name="selectGrafico"]').val();
                        var where = "";
                        where = panAsistente.find('[name="whereGrafico"]')[0].value;
                        where = where + "|||" + panAsistente.find('[name="groupbyGrafico"]').val();
                        where = where + "|||" + panAsistente.find('[name="orderbyGrafico"]').val();
                        where = where + "|||" + panAsistente.find('[name="limitGrafico"]').val();
                        that.ListaPestanyas[prefijoAsistenteClave + '.Where'] = where;
                        that.ListaPestanyas[prefijoAsistenteClave + '.Titulo'] = panAsistente.find('[name="mtGrafico"]').prop('checked');
                        that.ListaPestanyas[prefijoAsistenteClave + '.Tamano'] = panAsistente.find('[name="tamGrafico"]').val();

                        var tipo = panAsistente.find('[name="tGrafico"]').val();
                        that.ListaPestanyas[prefijoAsistenteClave + '.Tipo'] = tipo;
                        var prefijoDatasets = prefijoAsistenteClave + '.OpcionesDatasets';
                        if (tipo == '1') {
                            var divBarras = panAsistente.find('.opcionesBarras');
                            that.ListaPestanyas[prefijoAsistenteClave + '.PropExtra'] = divBarras.find('[name="horizontal"]').prop('checked');
                            that.ListaPestanyas[prefijoAsistenteClave + '.Labels'] = panAsistente.find('[name="labelsGrafico"]').val();
                            var panelDatasets = divBarras.find('.datasets');
                            var numDataset = 0;
                            //Recorremos datasets
                            $('.dataset', panelDatasets).each(function () {
                                var prefijoDatasetClave = prefijoDatasets + '[' + numDataset + ']';
                                that.ListaPestanyas[prefijoDatasetClave + '.DatasetID'] = $(this).attr('id');
                                that.ListaPestanyas[prefijoDatasetClave + '.Datos'] = $(this).find('[name="datosDataset"]').val();
                                that.ListaPestanyas[prefijoDatasetClave + '.Nombre'] = $(this).find('[name="nombreDataset"]').val();
                                that.ListaPestanyas[prefijoDatasetClave + '.Color'] = $(this).find('[name="colorDataset"]').val();
                                that.ListaPestanyas[prefijoDatasetClave + '.Orden'] = numDataset;
                                numDataset++;
                            });
                        }
                        if (tipo == '2') {
                            var divLineas = panAsistente.find('.opcionesLineas');
                            that.ListaPestanyas[prefijoAsistenteClave + '.PropExtra'] = divLineas.find('[name="area"]').prop('checked');
                            that.ListaPestanyas[prefijoAsistenteClave + '.Labels'] = panAsistente.find('[name="labelsGrafico"]').val();
                            var panelDatasets = divLineas.find('.datasets');
                            var numDataset = 0;
                            //Recorremos datasets
                            $('.dataset', panelDatasets).each(function () {
                                var prefijoDatasetClave = prefijoDatasets + '[' + numDataset + ']';
                                that.ListaPestanyas[prefijoDatasetClave + '.DatasetID'] = $(this).attr('id');
                                that.ListaPestanyas[prefijoDatasetClave + '.Datos'] = $(this).find('[name="datosDataset"]').val();
                                that.ListaPestanyas[prefijoDatasetClave + '.Nombre'] = $(this).find('[name="nombreDataset"]').val();
                                that.ListaPestanyas[prefijoDatasetClave + '.Color'] = $(this).find('[name="colorDataset"]').val();
                                that.ListaPestanyas[prefijoDatasetClave + '.Orden'] = numDataset;
                                numDataset++;
                            });
                        }
                        if (tipo == '3') {
                            var divCirculos = panAsistente.find('.opcionesCirculos');
                            that.ListaPestanyas[prefijoAsistenteClave + '.Labels'] = panAsistente.find('[name="labelsGrafico"]').val();
                            var prefijoDatasetClave = prefijoDatasets + '[0]';
                            that.ListaPestanyas[prefijoDatasetClave + '.DatasetID'] = divCirculos.find('.dataset').attr('id');
                            that.ListaPestanyas[prefijoDatasetClave + '.Datos'] = divCirculos.find('[name="datosDataset"]').val();
                            that.ListaPestanyas[prefijoDatasetClave + '.Nombre'] = 'Dataset';
                            that.ListaPestanyas[prefijoDatasetClave + '.Color'] = 'Aleatorio';
                            that.ListaPestanyas[prefijoDatasetClave + '.Orden'] = 0;
                        }
                        if (tipo == '4') {
                            var divTabla = panAsistente.find('.opcionesTabla');
                            that.ListaPestanyas[prefijoAsistenteClave + '.Labels'] = '';
                            var panelDatasets = divTabla.find('.datasets');
                            var numDataset = 0;
                            //Recorremos datasets
                            $('.dataset', panelDatasets).each(function () {
                                var prefijoDatasetClave = prefijoDatasets + '[' + numDataset + ']';
                                that.ListaPestanyas[prefijoDatasetClave + '.DatasetID'] = $(this).attr('id');
                                that.ListaPestanyas[prefijoDatasetClave + '.Datos'] = $(this).find('[name="datosDataset"]').val();
                                that.ListaPestanyas[prefijoDatasetClave + '.Nombre'] = $(this).find('[name="nombreDataset"]').val();
                                that.ListaPestanyas[prefijoDatasetClave + '.Color'] = 'Aleatorio';
                                that.ListaPestanyas[prefijoDatasetClave + '.Orden'] = numDataset;
                                numDataset++;
                            });
                        }
                        if (tipo == '5') {
                            var divHeat = panAsistente.find('.opcionesHeatMap');
                            that.ListaPestanyas[prefijoAsistenteClave + '.Labels'] = '';
                            var panelDatasets = divHeat.find('.datasets');
                            var numDataset = 0;
                            //Recorremos datasets
                            $('.dataset', panelDatasets).each(function () {
                                var prefijoDatasetClave = prefijoDatasets + '[' + numDataset + ']';
                                that.ListaPestanyas[prefijoDatasetClave + '.DatasetID'] = $(this).attr('id');
                                that.ListaPestanyas[prefijoDatasetClave + '.Datos'] = $(this).find('[name="datosDataset"]').val();
                                that.ListaPestanyas[prefijoDatasetClave + '.Nombre'] = $(this).find('[name="nombreDataset"]').val();
                                if (numDataset < 2) {
                                    that.ListaPestanyas[prefijoDatasetClave + '.Color'] = 'Aleatorio';
                                } else {
                                    that.ListaPestanyas[prefijoDatasetClave + '.Color'] = $(this).find('[name="colorDataset"]').val();
                                }
                                that.ListaPestanyas[prefijoDatasetClave + '.Orden'] = numDataset;
                                numDataset++;
                            });
                        }
                    });
                }

            }

            
            // Comprobación de facetas de la página (Si hay facetas asociadas)
            const panelEdicionFacetas = panelEdicion.find('.id-added-facet-list');
            if (panelEdicionFacetas.length > 0) {
                // Nº de la faceta
                let numFaceta = 0;
                // Recorrer cada faceta para la obtención de sus datos
                $('.facet-row', panelEdicionFacetas).each(function () {
                    // Prefijo para guardado en BD
                    const prefijoFacetasClave = prefijoClave + '.ListaFacetas[' + numFaceta + ']';

                    // Panel de detalles de la faceta
                    const panFaceta = $(this).find(".facet-info");
                    // Select / ComboBox de facetas
                    const selectorFacetas = panFaceta.find('.cmbListaFacetas');
                    // Select / ComboBox de Objetos de conocimiento
                    const selectorOC = panFaceta.find('.selectObjetosConocimiento');                    

                    that.ListaPestanyas[prefijoFacetasClave + '.Faceta'] = selectorFacetas.children('option:selected').val();
                    that.ListaPestanyas[prefijoFacetasClave + '.ObjetoConocimiento'] = selectorOC.children('option:selected').text();
                    that.ListaPestanyas[prefijoFacetasClave + '.Deleted'] = panFaceta.find('[name="TabEliminada"]').val();
                    that.ListaPestanyas[prefijoFacetasClave + '.ClavePestanya'] = fila.attr('id');
                    //that.ListaPestanyas[prefijoFacetasClave + '.Key'] = selectorFacetas.children('option:selected').val();
                    //that.ListaPestanyas[prefijoFacetasClave + '.Value'] = selectorOC.children('option:selected').text();
                    numFaceta++;                        
                });                
            }

            // Comprobación de las exportaciones de la página (Si hay exportaciones asociadas)
            const panelEditarExportaciones = panelEdicion.find('.id-added-exportation-list');
            if (panelEditarExportaciones.length > 0){
                // Nº de la exportación
                let numExport = 0;
                // Recorrer cada exportación para la obtención de sus datos
                $('.exportation-row', panelEditarExportaciones).each(function () {
                    // Prefijo para guardado en BD
                    const prefijoExportacionesClave = prefijoClave + '.ListaExportaciones[' + numExport + ']';

                    // Panel de detalles de la exportacion
                    const panExportacion = $(this).find(".exportation-info");
                    
                    // Establecer datos de la exportación
                    that.ListaPestanyas[prefijoExportacionesClave + '.Key'] = $(this).attr('id');
                    that.ListaPestanyas[prefijoExportacionesClave + '.Orden'] = numExport;
                    that.ListaPestanyas[prefijoExportacionesClave + '.Deleted'] = panExportacion.find('[name="TabEliminada"]').val();
                    that.ListaPestanyas[prefijoExportacionesClave + '.Nombre'] = panExportacion.find('[name="Nombre"]').val(); 
                    
                    // Obtener los grupos de la exportación
                    const privacidadGruposExport = panExportacion.find('[name="TabValoresPrivacidadGrupos"]').val().split(',');
                    for (let i = 0; i < privacidadGruposExport.length; i++) {
                        if (privacidadGruposExport[i].trim() != "") {
                            const prefijoPrivacidadGrupos = prefijoExportacionesClave + '.GruposPermiso[' + i + ']';
                            that.ListaPestanyas[prefijoPrivacidadGrupos + '.Key'] = privacidadGruposExport[i].substr(2);
                            that.ListaPestanyas[prefijoPrivacidadGrupos + '.Value'] = "";
                        }
                    }  
                    
                    // Obtener las propiedades de la exportación
                    let numPropExport = 0;
                    // Recorrer cada propiedad de exportación para la obtención de sus datos
                    $('.property-row', panExportacion).each(function () {
                        // Panel de detalles de la propiedad
                        const panPropExportacion = $(this).find(".property-info");
                        // Prefijo para guardado en BD
                        const prefijoPropExportacionesClave = prefijoExportacionesClave + '.ListaPropiedades[' + numPropExport + ']';
                        
                        // Establecer datos de la exportación
                        that.ListaPestanyas[prefijoPropExportacionesClave + '.Orden'] = numPropExport;
                        that.ListaPestanyas[prefijoPropExportacionesClave + '.Deleted'] = panPropExportacion.find('[name="TabEliminada"]').val();
                        that.ListaPestanyas[prefijoPropExportacionesClave + '.Nombre'] = panPropExportacion.find('[name="Nombre"]').val();
                        that.ListaPestanyas[prefijoPropExportacionesClave + '.Ontologia'] = panPropExportacion.find('[name="Ontologia"]').val();
                        that.ListaPestanyas[prefijoPropExportacionesClave + '.Propiedad'] = panPropExportacion.find('[name="Propiedad"]').val();
                        that.ListaPestanyas[prefijoPropExportacionesClave + '.DatoExtraPropiedad'] = panPropExportacion.find('[name="DatoExtraPropiedad"]').val();

                        numPropExport++;
                    });
                    numExport++;  
                }); 
                              
            }
            
            // Devolvería false ya que no hay errores durante obtención de datos de las páginas
            return that.errorDuranteObtenerDatosPestaya;

            // Comprobación de "Idiomas disponibles --> Parece que esta opción no está operativa vía HTML"
            /* PENDIENTE
            const panelIdiomasDisponibles = $('.idiomasDisponibles', panelEdicion);
            if (panelIdiomasDisponibles.length > 0) {
                var inputsIdioimas = $('input.idioma[type="checkbox"]:checked', panelIdiomasDisponibles)
                var numIdioma = 0;
                inputsIdioimas.each(function () {
                    var prefijoIdioma = prefijoClave + '.ListaIdiomasDisponibles[' + numIdioma + ']';

                    that.ListaPestanyas[prefijoIdioma] = $(this).val();

                    numIdioma++;
                });
            } 
            */           
        }
    },

    /**
     * Método para proceder al guardado de las páginas en BD.
     * El guardado se realiza una vez se ha comprobado que no hay errores (comprobarErroresGuardado) y que se han obtenidos los datos de las páginas modificadas (obtenerDatosPestanya)
     * Se envían las páginas que se han modificado
     * @param {*} completion: Cuando finalice de procesar la tarea, conjunto de acciones que se desean realizar
     */
    savePages: function(completion){
        const that = this;

        // Realizar la petición para el guardado de páginas
        GnossPeticionAjax(
            that.urlSavePages,
            that.ListaPestanyas,
            true
        ).done(function (data) {
            // OK - Mostrar el mensaje de guardado correcto
            mostrarNotificacion("success","Los cambios se han guardado correctamente");                        
            completion != undefined && completion(false);             
        }).fail(function (data) {
            // KO - Mostrar el error del guardado de páginas realizado
            let error = data.split('|||');
            if (data != "Contacte con el administrador del Proyecto, no es posible atender la petición.") {
                that.mostrarErrorGuardado();
                if (error[0] == "RUTA REPETIDA") {
                    that.mostrarErrorUrlRepetida($('#' + error[1]));
                    completion != undefined && completion(true);
                }
                else if (error[0] == "NOMBRE VACIO") {
                    that.mostrarErrorNombreVacio($('#' + error[1]));
                    completion != undefined && completion(true);
                }
                else if (error[0] == "PROYECTO_ORIGEN_BUSQUEDA_PRIVADO") {
                    that.mostrarErrorPoyectoOrigenBusquedaPrivado($('#' + error[1]));
                    completion != undefined && completion(true);
                } else if (error[0] == "invitado") {
                    mostrarNotificacion("error", "La sesión de usuario ha caducado. Accede con tu usuario y credenciales para poder continuar.")
                    completion != undefined && completion(true);
                } else if (error[0] == "ERROR CONCURRENCIA") {
                    mostrarNotificacion("error", error[1])
                    completion != undefined && completion(true);
                }
            }
            else
            {
                that.mostrarErrorGuardadoFallo(data);
                completion != undefined && completion(true);
            }

        }).always(function () {
            // Ocultar el loading
            loadingOcultar();
        });
    },

    /**
     * Método para mostrar posibles errores durante el proceso del guardado de páginas. Estos errores son devueltos por el servidor
     * una vez el método "savePages" ha sido ejecutado
     */
    mostrarErrorGuardado: function () {
        let esPre = "False";
        let entornoBloqueado = "False";

            if (esPre == "True") {
                mostrarNotificacion("error", "No se permite guardar porque estás en el entorno de preproducción");                
            }
            else if (entornoBloqueado == "True") {                
                mostrarNotificacion("error", "El entorno actual esta bloqueado. Hay que desplegar la versión de preproducción antes de hacer cambios");
            }
            else {
                mostrarNotificacion("error", "Ha habido errores en el guardado de las pesta&#xF1;as, revisa los errores marcados");                
            }
    },  


    /**
     * Método para mostrar posibles errores durante el proceso del guardado de páginas. Estos errores son devueltos por el servidor
     * una vez el método "savePages" ha sido ejecutado
     * @param {jqueryObject} fila 
     */
    mostrarErrorPoyectoOrigenBusquedaPrivado: function (fila) {
        const that = this;
        const inputUrl = $('input[name = "TabName"]', fila).first();
        mostrarNotificacion("error", "El proyecto no puede ser privado");        
    },    




}

var tamFijo = 0;
const DATA_COUNT = 7;

/**
 * Operativa de funcionamiento de Gestión de servicios externos
 */
const operativaGestionServiciosExternos = {

    /**
     * Inicializar operativa
     */
    init: function (pParams) {
        this.pParams = pParams;
        // Permitir o no guardar datos (CI/CD -> Pasado desde la vista)
        if (this.allowSaveData == undefined){
            this.allowSaveData = pParams.allowSaveData;
        }        
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
        that.handleCheckNumberOfExternalServices();
        
        // Configurar arrastrar servicios externos                            
        setupBasicSortableList(that.externalServicesListContainerId, that.sortableExternalServiceIconClassName, undefined, undefined, undefined, undefined);    
    },

    /**
     * Configuración de las rutas de acciones necesarias para hacer peticiones a backend
     */
    configRutas: function (pParams) {        
        // Url base
        this.urlBase = refineURL();
        // Url para traer el modal para crear nuevo servicio externo
        this.urlAddNewExternalService = `${this.urlBase}/load-new-item`;
        // Url para guardar los servicios externos
        this.urlSaveExternalServices = `${this.urlBase}/save`;
        // Url para guardar la url base para los servicios externos
        this.urlSaveBaseServices = `${this.urlBase}/saveUrl`;
    }, 
    
    /*
     * Inicializar elementos de la vista
     * */
    config: function (pParams) {

        // Botón para crear un nuevo servicio externo
        this.linkAddExternalServiceClassName = "linkAddExternalService";
            
        /* Modales */
        // Modal container dinámico
        this.modalContainer = $('#modal-container');
        // Modal para la edición de la url base
        this.modalUrlBaseService = $("#modal-external-service-base");
        // Modal de cada uno de los servicios externos
        this.modalExternalServiceClassName = "modal-external-service";        
        // Modal de eliminación de páginas
        this.modalDeleteExternalServiceClassName = "modal-confirmDelete";

        // Botón para editar la Url del servicio de la comunidad       
        this.btnEditComunityUrlServiceIdName = "btnEditComunityUrlService";
        this.btnEditComunityUrlService = $(`#${this.btnEditComunityUrlServiceIdName}`);
        // Botón para guardar la urlBaseService 
        this.btnSaveUrlBaseServiceId = "btnSaveUrlBaseService";
        this.btnSaveUrlBaseService = $(`#${this.btnSaveUrlBaseServiceId}`);
        // Input que contiene la url base del servicio (Modal de edición url base)
        this.urlBaseService = $("#urlBaseService");

        // Clase de cada una de las filas del servicio externo        
        this.externalServiceListItemClassName = "external-service-row";               

        // Buscador de sección
        this.txtBuscarServicioExterno = $("#txtBuscarServicioExterno");
        // Nº de elementos
        this.numResultadosServiciosExternos = $("#numServiciosExternos");
                   
        // Contenedor de servicios externos
        this.externalServicesListContainerId = 'id-added-external-services-list';
        this.externalServicesListContainer = $(`#${this.externalServicesListContainerId}`);
        // Icono para arrastrar un servicio externo        
        this.sortableExternalServiceIconClassName = 'js-component-sortable-external-service';
       
        // Label del nombre del servcio en el listado
        this.componentNombreClassName = "component-nombre";
        // Label de la Url del servicio en el listado
        this.componentUrlClassName = "component-url";
        // Label de la fecha del servicio
        this.componentFechaClassName = "component-fecha";

        /* Acciones */
        // Botón para edición
        this.btnEditExternalServiceClassName = "btnEditExternalService";
        // Botón para el borrado
        this.btnDeleteExternalServiceClassName = "btnDeleteExternalService"
        /* Elementos del modal */
        // Input del nombre del servicio
        this.inputNombreServicioExternoClassName = "inputNombreServicioExterno";
        // Input del url del servicio
        this.inputUrlServicioExternoClassName = "inputUrlServicioExterno";
        // Botón para confirmar la eliminación del servicio externo
        this.btnConfirmDeleteExternalServiceClassName = "btnConfirmDeleteExternalService";                
        // Botón para guardar datos del modal
        this.btnGuardarExternalServiceClassName = "btnGuardarExternalService";   

        // Variable que contendrá la fila o página activa. Por defecto "undefined"
        this.filaExternalService = this.filaExternalService != undefined ? this.filaExternalService : undefined;

        // Flags
        this.confirmDeleteExternalService = false;
    },  
    
    /**
     * Configuración de eventos de elementos del Dom (Botones, Inputs...)     
     */
    configEvents: function (pParams) {
        const that = this;
                       
        this.modalContainer.on('show.bs.modal', (e) => {
            // Aparición del modal
            that.triggerModalContainer = $(e.relatedTarget);
        })
        .on('hide.bs.modal', (e) => {
            // Acción de ocultar el modal
        })
        .on('hidden.bs.modal', (e) => {
            // Vaciar el modal
            resetearModalContainer();            
        });

        // Comportamientos del modal que de borrado de items 
        configEventByClassName(`${this.modalExternalServiceClassName}`, function(element){
            const $modal = $(element);
            $modal
            .on('show.bs.modal', (e) => {
                // Aparición del modal            
                that.triggerModalContainer = $(e.relatedTarget);
            })
            .on('hide.bs.modal', (e) => {
                // Acción de ocultar el modal
                // El modal que se va a ocultar/cerrar
                const currentModal = $(e.currentTarget);                                
                that.handleCloseExternalServiceModal(currentModal);            
            }); 
        }); 

        // Comportamientos del modal que son individuales para el borrado de servicios externos
        $(`.${this.modalDeleteExternalServiceClassName}`).on('show.bs.modal', (e) => {
            // Aparición del modal
        })
        .on('hide.bs.modal', (e) => {
            // Acción de ocultar el modal
            // Si el modal se cierra sin confirmar borrado, desactiva el borrado
            if (that.confirmDeleteExternalService == false){
                that.handleSetDeleteExternalService(false);                
            }            
        }); 

        configEventByClassName(`${that.linkAddExternalServiceClassName}`, function(element){
            const $jqueryElement = $(element);
            $jqueryElement.off().on("click", function(){                 
                const button = $(this);
                that.handleAddNewExternalService();                
            });	                        
        });  

        // Botón/es para editar o abrir el modal
        configEventByClassName(`${that.btnEditExternalServiceClassName}`, function(element){
            const $jqueryElement = $(element);
            $jqueryElement.off().on("click", function(){                 
                const button = $(this);
                // Fila correspondiente a la pagina eliminada
                that.filaExternalService = button.closest(`.${that.externalServiceListItemClassName}`);
                // Añadir class de "modified"
                that.filaExternalService.addClass("modified");
            });	                        
        });          

        // Botón/es para eliminar un determinado servicio externo al pulsar en el botón de papelera
        configEventByClassName(`${that.btnDeleteExternalServiceClassName}`, function(element){
            const $jqueryElement = $(element);
            $jqueryElement.off().on("click", function(){                 
                const button = $(this);
                // Fila correspondiente a la pagina eliminada
                that.filaExternalService = button.closest(`.${that.externalServiceListItemClassName}`);
                // Marcar la página como "Eliminar" a modo informativo al usuario
                that.handleSetDeleteExternalService(true);             
            });	                        
        });  

        // Botón para confirmar la eliminación de un objeto de conocimiento desde el modal
        configEventByClassName(`${that.btnConfirmDeleteExternalServiceClassName}`, function(element){
            const confirmRemoveExternalService = $(element);
            confirmRemoveExternalService.off().on("click", function(){   
                // Confirmamos la eliminación                
                that.confirmDeleteExternalService = true;
                // Procedemos al borrado/guardado
                that.handleConfirmDeleteExternalService();
            });	                        
        });       
        
        // Input del nombre del servicio editado desde el modal
        configEventByClassName(`${that.inputNombreServicioExternoClassName}`, function(element){
            const input = $(element);            
            input.off().on("keyup", function(){   
                // Editamos el título del modal y el de la fila correspondiente
                const modalTitle = that.filaExternalService.find(".modal-title > .community-name");
                const rowTitle = that.filaExternalService.find(`.${that.componentNombreClassName}`);
                // Actualizar los datos
                const inputValue = $(this).val().trim();
                modalTitle.html(inputValue);
                rowTitle.html(inputValue);
                // Formatear el nombre del servicio
                $(this).val($(this).val().replace(/[^a-zA-Z0-9 _ -]/g, '').trim());
                $(this).val($(this).val().toLowerCase());
                // Actualizar el nombre del servicio
                that.handleUpdateUrlService($(this));
            });	                        
        });  
        
        // Input del nombre del servicio editado desde el modal
        configEventByClassName(`${that.inputNombreServicioExternoClassName}`, function(element){
            const input = $(element);            
            input.on("blur", function(){   
                comprobarInputNoVacio(input, true, false, "El nombre del servicio no puede estar vacío.", 0);
            });	                        
        });

        // Input de la url del servicio editado desde el modal
        configEventByClassName(`${that.inputUrlServicioExternoClassName}`, function(element){
            const input = $(element);            
            input.off().on("keyup", function(){   
                // Editamos el título de la fila correspondiente                
                const rowTitle = that.filaExternalService.find(`.${that.componentUrlClassName}`);
                // Actualizar los datos
                const inputValue = $(this).val().trim();
                rowTitle.html(inputValue);
            });	                        
        });  
        
        // Input de la url del servicio editado desde el modal (blur)
        configEventByClassName(`${that.inputUrlServicioExternoClassName}`, function(element){
            const input = $(element);            
            input.on("blur", function(){   
                comprobarInputNoVacio(input, true, false, "La url del servicio externo no puede estar vacío.", 0);
            });	                        
        });  
                
        // Botón para guardar el servicio externo desde el modal
        configEventByClassName(`${that.btnGuardarExternalServiceClassName}`, function(element){
            const saveButton = $(element);
            saveButton.off().on("click", function(){
                that.handleSave();
            });	                        
        }); 
        
        // Botón para editar la url base del servicio
        this.btnSaveUrlBaseService.on("click", function(){
            // Guardar la base url del servicio
            console.log("Guardar URL BASE. Será necesario actualizar.");
            that.handleSaveUrlBaseServices();
        });

        // Input para almacenar la url base del servicio.
        this.urlBaseService.off().on("blur", function(){
            comprobarInputNoVacio($(this), true, false, "La URL base para el servicio no puede estar vacía.", 0);
        });

        // Búsquedas de servicios externos
        this.txtBuscarServicioExterno.off().on("keyup", function (event) {
            const input = $(this);
            clearTimeout(that.timer);
            that.timer = setTimeout(function () {                                                                        
                // Acción de buscar / filtrar
                that.handleSearchExternalServiceItem(input);                                         
            }, 500);
        }); 
    }, 

    /**
     * Método para guardar/editar la url base para los servicios externos
     */
    handleSaveUrlBaseServices: function(){
        const that = this;
        const urlBaseService = that.urlBaseService.val().trim();
        // Reseteo valores para guardado
        that.OptionsUrl = {};

        if (urlBaseService.length == 0){
            return;
        }

        // Construir el parámetro para enviar a la url base service
        that.OptionsUrl['ServiceName'] = urlBaseService;
        // Mostrar loading
        loadingMostrar();

        GnossPeticionAjax(
        that.urlSaveBaseServices,
        that.OptionsUrl,
        true
        ).done(function (data) {            
            mostrarNotificacion("success", "Los cambios se han guardado correctamente.");
            // Cerrar el modal y recargar la página para recargar la web con los servicios          
            setTimeout(function() {
                dismissVistaModal(that.modalUrlBaseService);                            
                location.reload();
            }
            ,1000);            
        }).fail(function (data) {
            mostrarNotificacion("error", "Se han producido errores en el guardado de la URL base.");
        }).always(function () {
            loadingOcultar();
        });
    },
    
    /**
     * Método para actualizar la Url de un servicio en particular cuando se actualice el nombre del servicio
     * @param {*} inputNombre Input que contendrá el nombre del servicio que se está modificando     
     */
    handleUpdateUrlService: function(inputNombre){
        const that = this;

        // Obtener el nombre del servicio
        const nombre = inputNombre.val().trim();
        const inputUrl = that.filaExternalService.find('[name="Url"]');        
        // Obtener el nombre de la url base del servicio (input oculto)
        const serviceName = $("#urlBaseService").val();
        const newUrlExternalService = serviceName.replace('{ServiceName}', nombre);
        inputUrl.val(newUrlExternalService);
        // Actualizar también el nombre en la fila
        inputUrl.trigger("keyup");
    },

    /**
     * Método que ejecutará la revisión de los servicios externos y su posterior guardado.              
     */
    handleSave: function(){
        const that = this;

        that.handleSaveExternalServices(function(result, data){
            if (result == requestFinishResult.ok){
                //Ocultar el modal                                
                const modalEditExternalService = $(that.filaExternalService).find(`.${that.modalExternalServiceClassName}`);                                          
                dismissVistaModal(modalEditExternalService);                                                              
                               
                // Actualizar el contador de nº de servicios externos
                that.handleCheckNumberOfExternalServices();                        
                loadingOcultar();                 
                // Mostrar ok
                mostrarNotificacion("success", "Los cambios se han guardado correctamente.");                                                                           
            }else{           
                const error = data.split('|||');                    
                if (error[0] == "RUTA REPETIDA") {                        
                    mostrarNotificacion("error", error[1]);  
                }
                else if (error[0] == "NOMBRE VACIO") {                        
                    mostrarNotificacion("error", error[1]);
                }
                else if (error[0] == "PROYECTO_ORIGEN_BUSQUEDA_PRIVADO") {
                    mostrarNotificacion("error", error[1]);                        
                }
            }             
        });           
    },   
    
    /**
     * Método para guardar los servicios externos. Este método se ejecuta cuando se pulsa en el modal de "Guardar"
     * @param {*} completion : Función a ejecutar cuando se realice o complete este método.
     */
    handleSaveExternalServices: function(completion = undefined){
        const that = this;
        
        // Resetear flag de errores para realizar la comprobación de nuevo
        that.errorsBeforeSaving = false;

        // Mostrar loading
        loadingMostrar();
        
        // Objeto de las categorías a guardar en backend
        that.ListaServiciosExternos = {};
        let cont = 0;

        // Comprobación de erres (No permitir valores vacíos)
        that.checkErrorsBeforeSaving();

        // Si todo ha ido bien, proceder con la recogida de datos y su guardado
        if (that.errorsBeforeSaving == false) {
            // Obtener los datos de los servicios externos. Recorrer y obtener su información
            that.externalServicesListContainer.find($(`.${that.externalServiceListItemClassName}`)).each(function () {                
                that.obtenerDatosServicioExterno($(this), cont++);                
            });

            // Guardar categorías y seguídamente cookiesButton
            GnossPeticionAjax(                
                that.urlSaveExternalServices,
                that.ListaServiciosExternos,
                true
            ).done(function (data) {                
                // OK Guardado de servicios externos
                $(".newExternalService").removeClass("newExternalService");                                        
                if (completion != undefined){
                    completion(requestFinishResult.ok, data);
                }else{
                    mostrarNotificacion("success", "Los cambios se han guardado correctamente");
                }                              
            }).fail(function (data) {
                // KO al guardar servicios externos
                if (completion != undefined){
                    completion(requestFinishResult.ko, data);
                }else{
                    const error = data.split('|||');                    
                    if (error[0] == "RUTA REPETIDA") {                        
                        mostrarNotificacion("error", error[1]);  
                    }
                    else if (error[0] == "NOMBRE VACIO") {                        
                        mostrarNotificacion("error", error[1]);
                    }
                    else if (error[0] == "PROYECTO_ORIGEN_BUSQUEDA_PRIVADO") {
                        mostrarNotificacion("error", error[1]);                        
                    }                                      
                }                                                                    
            }).always(function () {
                if (completion == undefined){
                    loadingOcultar();
                }                
            });
        }else{
            // Hay errores en la introducción de datos
            loadingOcultar();
        }
    }, 

    /**
     * Método para recoger los datos de los servicios externos y construir el objeto para envío a backend
     * @param {jqueryElement} fila : Fila del servicio externo del que se desea obtener la información
     * @param {*} num : Contador para construir el objeto
     */
    obtenerDatosServicioExterno: function(fila, num){
        const that = this;
        
        // Contenido o datos de la cookie dentro del modal
        const panelEdicion = fila.find('.modal-external-service');

        // Prefijo para guardado en bd
        const prefijoClave = 'ListaPestanyas[' + num + ']';

        // Comprobar si está o no eliminada        
        that.ListaServiciosExternos[prefijoClave + '.Deleted'] = panelEdicion.find('[name="TabEliminada"]').val();
        // Comprobar si el servicio es de reciente creación
        const nueva = panelEdicion.hasClass("newExternalService");
        that.ListaServiciosExternos[prefijoClave + '.Nueva'] = nueva;

        // Nombre del servicio
        const nombre = panelEdicion.find('[name="Nombre"]').val().trim();
        that.ListaServiciosExternos[prefijoClave + '.NombreServicio'] = nombre;
        // Url del servicio
        const url = panelEdicion.find('[name="Url"]').val().trim();        
        that.ListaServiciosExternos[prefijoClave + '.UrlServicio'] = url;
    },
    
    /**
     * Método para comprobar que los datos son correctos
     */
    checkErrorsBeforeSaving: function(){
        const that = this;
        
        // Flag para control de errores
        that.errorsBeforeSaving = false;                
        
        // Comprobar nombres y url que no estén vacíos
        that.comprobarNombresUrlVacios();
        
        if (that.errorsBeforeSaving == true){
            mostrarNotificacion("error", "Los servicios no pueden tener un nombre ni una url vacía. Comprueba los datos e inténtalo de nuevo.");
        }
    },


    /**
     * Método para comprobar que no existen servicios externos con nombre o url vacía.
     */
    comprobarNombresUrlVacios: function(){         
        const that = this;               
        // Comprobación de que el nombre y la Url no son vacíos        
        let inputsNombre = $(`.${that.externalServiceListItemClassName} input[name="Nombre"]:not(":disabled")`);                
        let inputsUrl = $(`.${that.externalServiceListItemClassName} input[name="Url"]`);
        // Comprobación de los nombres
        inputsNombre.each(function () {
            if (that.errorsBeforeSaving == false){
                if ($(this).val().trim().length == 0){
                    that.errorsBeforeSaving = true;
                    return;
                }
            }
        });

        // Comprobacion de urls
        if (that.errorsBeforeSaving == true){
            // Comprobación de las Urls
            inputsUrl.each(function () {
                if (that.errorsBeforeSaving == false){
                    if ($(this).val().trim().length == 0){
                        that.errorsBeforeSaving = true;
                        return;
                    }
                }
            });        
        }
    },



    /**
     * Método que se ejecutará cuando se cierre el modal de detalles de un servicio externo
     * @param {jqueryObject} currentModal Modal que se va a cerrar
     */    
    handleCloseExternalServiceModal: function(currentModal){
        const that = this;

        // Quitar la opción de guardar cambios si no se pulsa en "Guardar". Si se pulsa en guardar se procederá al guardado
        that.filaExternalService.removeClass("modified");

        // Tener en cuenta si la página es de reciente creación y por tanto no se desea guardar
        if (that.filaExternalService.hasClass("newExternalService")){
            // Eliminar la fila que se acaba de eliminar                                
            that.filaExternalService.remove();
            // Actualizar el contador
            that.handleCheckNumberOfExternalServices();
        }
    },    


    /**
     * Método para añadir un nuevo servicio externo pulsando en el botón de "+Nuevo servicio".
     *    
     */
    handleAddNewExternalService: function(){
        const that = this;
        
        // Mostrar loading hasta que finalice la petición para crear una nueva página
        loadingMostrar();    

        // Realizar la petición para obtener el modal para crear un nuevo servicio externo
        
        GnossPeticionAjax(                
            that.urlAddNewExternalService,
            undefined,
            true
        ).done(function (data) {           
            // Añadir la nueva fila al listado
            that.externalServicesListContainer.append(data);
            // Referencia al nuevo servicio añadido
            that.filaExternalService = that.externalServicesListContainer.children().last();
            // Panel de edición de la nueva página creada
            const editionNewExternalServicePanel = that.filaExternalService.children(".modal-external-service");                           
            // Abrir el modal para poder editar/gestionar la nueva página añadida                              
            that.filaExternalService.find(`.${that.btnEditExternalServiceClassName}`).trigger("click");                                 
            // Actualizar el contador
            that.handleCheckNumberOfExternalServices()                            
        }).fail(function (data) {
            // Mostrar error al tratar de crear un servicio externo
            mostrarNotificacion("error", data);                
        }).always(function () {
            // Ocultar loading de la petición
            loadingOcultar();
        });                    
    },


    /**
     * Método para eliminar un servicio externo una vez se ha confirmado desde el modal.
     */
    handleConfirmDeleteExternalService: function(){
        const that = this;
        
        that.handleSaveExternalServices((function(result, data){
            if (result == requestFinishResult.ok){
                // 2 - Ocultar el modal de eliminación 
                const modalDeleteExternalService = $(`.${that.modalDeleteExternalServiceClassName}`);
                dismissVistaModal(modalDeleteExternalService);
                // 3 - Ocultar loading
                loadingOcultar();                
                // 4 - Ocultar la fila que se desea eliminar
                that.filaExternalService.addClass("d-none");
                // 5 - Eliminar la fila que se acaba de eliminar                                
                that.filaExternalService.remove();
                // 6 - Actualizar el contador
                that.handleCheckNumberOfExternalServices();
                // Restablecer el flag de borrado de la fila de la cookie
                that.confirmDeleteExternalService = false;
                mostrarNotificacion("success", "Los cambios se han guardado correctamente");
            }else{
                mostrarNotificacion("error", "Se ha producido un error al tratar de eliminar el servicio externo. Contacta con el administrador de la comunidad.");
            }             
        }));    
    },         

        /*loadingMostrar();

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
        */                 
   

    /**
     * Método para buscar un servicio externo
     * @param {jqueryElement} input : Input desde el que se ha realizado la búsqueda
     */
    handleSearchExternalServiceItem: function(input){     
        const that = this;
        // Cadena introducida para filtrar/buscar
        let cadena = input.val();
        // Eliminamos posibles tildes para búsqueda ampliada
        cadena = cadena.normalize('NFD').replace(/[\u0300-\u036f]/g, "").toLowerCase();

        // Listado de componentes donde se buscará
        const externalServiceItems = $(`.${that.externalServiceListItemClassName}`);

        // Recorrer los items/compontes para realizar la búsqueda de palabras clave
        $.each(externalServiceItems, function(index){
            const externalServiceItem = $(this);
            // Seleccionar el nombre sólo "visible" de la traducción. Quitamos caracteres extraños, tildes para poder hacer bien la búsqueda
            const componentNombre = externalServiceItem.find(`.${that.componentNombreClassName}`).not('.d-none').html().trim().normalize("NFD").replace(/[\u0300-\u036f]/g, "").toLowerCase();
            const componentUrl = externalServiceItem.find(`.${that.componentUrlClassName}`).not('.d-none').html().trim().normalize("NFD").replace(/[\u0300-\u036f]/g, "").toLowerCase();            

            if (componentNombre.includes(cadena) || componentUrl.includes(cadena)){
                // Mostrar la fila
                externalServiceItem.removeClass("d-none");                
            }else{
                // Ocultar la fila
                externalServiceItem.addClass("d-none");
            }            
        });                        
    },    
    
    /**
    * Método que se ejecutará al cargarse la web para saber el nº de traducciones existentes
    */
    handleCheckNumberOfExternalServices: function(){        
        const that = this;
        const numberExternalServices = that.externalServicesListContainer.find($(`.${that.externalServiceListItemClassName}`)).length;
        // Mostrar el nº de items                
        that.numResultadosServiciosExternos.html(numberExternalServices);
    },      
    
    /**
     * Método para marcar o desmarcar el item como "Eliminad" dependiendo de la elección vía Modal
     * @param {Bool} Valor que indicará si se desea eliminar o no el item
     */
    handleSetDeleteExternalService: function(deleteExternalService){
        const that = this;

        if (deleteExternalService){
            // Realizar el "borrado" de la página
            // 1 - Marcar la opción "TabEliminada" a true
            that.filaExternalService.find('[name="TabEliminada"]').val("true");           
            // 2 - Añadir la clase de "deleted" a la fila de la página
            that.filaExternalService.addClass("deleted");
        }else{
            // 1 - Marcar la opción "TabEliminada" a false
            that.filaExternalService.find('[name="TabEliminada"]').val("false");            
            // 2 - Eliminar la clase de "deleted" a la fila de la página
            that.filaExternalService.removeClass("deleted");
        }
    },  

}



/**
 * Operativa de funcionamiento de Edición multimedia CMS
 */
const operativaGestionMultimediaCMS = {

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
     * Configuración de las rutas de acciones necesarias para hacer peticiones a backend
     */
     configRutas: function (pParams) {
        // Url para llamar al modal para crear un nuevo item multimedia
        this.urlBase = refineURL();
        this.urlLoadAddNewMultimediaItem = `${this.urlBase}/load-new-item`;
        // Url de petición para realizar la subida de la imagen. Hace un POST a la urlBase
        this.urlSubirImagen = this.urlBase; 
        // Url para llamar al modal para confirmar borrado de un item multimedia
        this.urlLoadDeleteMultimediaItem = `${this.urlBase}/load-delete-multimedia-item`;
        // Url de petición para eliminar un recurso multimedia. Hace un POST a la urlBase
        this.urlDeleteMultimediaItem = this.urlBase;
    },
    
    /*
     * Inicializar elementos de la vista
     * */
    config: function (pParams) {
        
        /* Buscador de items multimedia */
        // Input de buscador de items multimedia
        this.txtBuscarCMSMultimedia = $("#txtBuscarCMSMultimedia");
        // Lupa del buscador
        this.inputLupa = $("#inputLupa");

        /* Nuevos items multimedia */
        // Botón para añadir un nuevo item multimedia
        this.btnAddMultimediaItem = $("#btnAddMultimediaItem");
        // Botón para eliminar items multimedia
        this.btnDeleteMultimediaItem = $(".btnDeleteMultimediaItem");
        // Botón para copiar la url del item multimedia
        this.btnCopyMultimediaItem = $(".btnCopyMultimediaItem");

        // Contenedor modal de contenido dinámico
        this.modalContainer = $("#modal-container");
        
    },  
    
    /**
     * Configuración de eventos de elementos del Dom (Botones, Inputs...)     
     */
     configEvents: function (pParams) {
        const that = this;
                       
        this.modalContainer.on('show.bs.modal', (e) => {
            // Aparición del modal
            // Conocer el botón disparador del modal para coger atributos necesarios. 
            ////////that.KEY_A_GUARDAR = $(e.relatedTarget).data("atributo");            
            ////////that.KEY_A_GUARDAR = $(e.relatedTarget).data("atributo");            
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


        // Click en el botón para añadir un nuevo item multimedia a través de la carga de un modal
        this.btnAddMultimediaItem.on("click", function(){
            // Mostrar el modal cargando            
            that.modalContainer.modal('show');      
            // Cargar la vista para crear nueva certificación
            getVistaFromUrl(that.urlLoadAddNewMultimediaItem, 'modal-dinamic-content', '');
        });   
        
        // Click en el botón para copiar la url del item multimedia
        this.btnCopyMultimediaItem.on("click", function(){
            const urlString = $(this).data("url");            
            that.handleCopyToClickBoard(urlString);
        });
        
        // Realizar búsquedas de items multimedia al escribir (KeyUp)        
        this.txtBuscarCMSMultimedia.on("keyup", function(e){
            // Buscar al pulsar tecla Intro
            if (e.keyCode == 13){
                that.handleSearchMultimediaItem();
            }
        });

        // Click en la lupa para realizar búsquedas de items multimedia
        this.inputLupa.on("click", function(){
            that.handleSearchMultimediaItem();
        });
    }, 
    
    /**
     * Lanzar comportamientos u operativas necesarias para el funcionamiento de la sección
     */
    triggerEvents: function(){
        const that = this;                           
    }, 
    
    // Configurar elementos relativos a la operativa de Añadir Multiemdia Item cuando se lance el modal
    operativaModalAddMultimediaItem: function(){
        const that = this;

        /* Drop Imagenes para subir */
        // Sección para aplicar el comportamiento del plugin imageDropArea
        this.dropareaImage = $('.js-image-uploader');  
        // Contenedor para poder hacer drag/drop de la imagen de la cabecera
        this.contenedorImagen = $("#contenedorImagen"); 
        // Imagen preview de la cabecera
        this.previewImg = $(".image-uploader-multimedia-item");
        // Id del Input oculto donde se guardará la imagen subida 
        this.imageMultimediaSrc = "#image-multimedia-src";         

        // Inicializar operativa imageDropArea
        that.initImageDropArea();

    },

   
    
    /* Carga de imágen multimedia */
    /**
     * Inicializar el área del drop para poder cargar imágenes
     */
     initImageDropArea: function(){
        const that = this;
        // Inicializar plugin al dropArea para la cabecera
        this.dropareaImage.imageDropArea({
            urlUploadImage: that.urlSubirImagen,
            urlUploadImageType: "fileUpload",
            panelAccionesImagen: undefined,
            contenedorImagen: that.contenedorImagen,
            previewImg: that.previewImg,
            inputHiddenImageLoaded: that.imageMultimediaSrc,
            panelVistaContenedor: undefined,
            completion: that.onCompletionMultimediaItemUploaded,
        });
    },

    /**
     * Funcionalidad a ejecutar cuando se suba un fichero multimedia CMS (Imagen o Fichero) mediante la funcionalidad dropArea
     * @param {HtmlString} htmlResponse : Respuesta que devuelve el método al subir la imagen de la cabecera
     * @param {Bool} success : Indica si la operación ha sido correcta o errónea.
     */
     onCompletionMultimediaItemUploaded: function(htmlResponse, success){
        if (success == true){
            // Subida correcta del fichero                    
            mostrarNotificacion("success", "Carga de elementos multimedia realizada correctamente.");
            // Cerrar el modal y recargar la página para recargar todos los ficheros            
            setTimeout(function() {
                dismissVistaModal();            
                location.reload();
            }
            ,1000);
            
        }else{            
            mostrarNotificacion("error", "Se ha producido un error en la subida del fichero multimedia. Por favor contacta con el administrador.");
            dismissVistaModal();
        }
    },
    
    // Configurar elementos relativos a la operativa de Añadir Multiemdia Item cuando se lance el modal
    operativaModalDeleteMultimediaItem: function(){
        const that = operativaGestionMultimediaCMS;

        // Botón para confirmar la eliminación de un recurso multimedia
        this.btnDeleteMultimediaItem = $('.btn-delete-multimedia-item'); 
        
        // Click para confirmar la eliminación de un recurso multimedia
        this.btnDeleteMultimediaItem.on("click", function(){
            const nombreComponente = that.triggerModalContainer.data("name");            
            that.handleDeleteMultimediaItem(nombreComponente);
        });
    },  
    

    /* Gestión de Acciones de items multimedia */
    /**************************************** */

    /**
     * Método para confirmar la eliminación de un item multimedia una vez pulsado "Sí" en el modal
     * @param {String} nombreComponente 
     */
    handleDeleteMultimediaItem: function (nombreComponente) {
        
        // Mostrar loading
        loadingMostrar();
        
        // Objeto a enviar para la eliminación del recurso multimedia
        const datosPost =
        {
            callback: "eliminarComponenteMultimedia",
            nombreComponente: nombreComponente
        };

        $.post(`${this.urlDeleteMultimediaItem}`, datosPost,
        function (data) {            
            if (data.indexOf("OK") == 0) {
                // OK                
                // 1- Cerrar el modal y esperar 1 segundo para mostrar el mensaje de OK
                dismissVistaModal();
                mostrarNotificacion("success", "Elemento multimedia borrado correctamente");
                setTimeout(function() {
                    // 2- Recargar la página
                    document.location = document.location;
                },1000);                                            
            }
            else {
                // KO
                mostrarNotificacion("error", data);                
            }
            loadingOcultar();
        });
    },

    /**
     * Método para copiar al portapapeles el enlace del item pulsado.     
     * @param {String} urlString : Url o cadena que se mostrará al usuario y que se copiará al portapapeles
     */
    handleCopyToClickBoard(urlString){           
        copyTextToClipBoard(urlString);
    },


    /* Métodos para visualizar datos haciendo uso del paginador / Buscador */
    /********************************************************** */
    /**
     * Método para aplicar filtro/petición para listar items (Ej: Click en página de paginador de resultados)
     * @param {*} filtro 
     */
    AgregarFiltroComponentes: function (filtro) {
        
        // Mostrar loading
        loadingMostrar();
        // Parámetro del filtro a usar en carga de items
        const parametroNuevo = filtro.substr(0, filtro.indexOf("="));
        // Valor del filtro 
        const valorNuevo = filtro.substr(filtro.indexOf("=") + 1);
        // Filtros actuales 
        let filtrosActuales = "";
        // Cargar los filtros actuales que hay a través de la URL
        if (document.location.href.indexOf('?') > -1) {
            filtrosActuales = document.location.href.substr(document.location.href.indexOf('?') + 1);
        }

        // Comprobación de filtros para eliminar o añadir a url para su petición
        if (filtrosActuales.indexOf(filtro) != -1) {
            //Contiene el filtro tal cual, lo eliminamos
            filtrosActuales = filtrosActuales.replace(filtro + "&", "");
            filtrosActuales = filtrosActuales.replace("&" + filtro, "");
            filtrosActuales = filtrosActuales.replace(filtro, "");
        } else if (filtrosActuales.indexOf(parametroNuevo + "=") != -1) {
            //Contiene el filtro pero con diferente valor, lo cambiamos
            inicioFiltro = filtrosActuales.indexOf(parametroNuevo + "=");
            finfiltro = filtrosActuales.indexOf("&", inicioFiltro) - 1;
            filtroAntiguo = "";
            if (finfiltro < 0) {
                filtroAntiguo = filtrosActuales.substr(inicioFiltro);
            } else {
                filtroAntiguo = filtrosActuales.substr(inicioFiltro, finfiltro - inicioFiltro);
            }
            filtrosActuales = filtrosActuales.replace(filtroAntiguo, filtro);
        } else {
            //No contiene el filtro, lo añadimos
            if (filtrosActuales == "") {
                filtrosActuales = filtro;
            } else {
                filtrosActuales += "&" + filtro;
            }
        }

        if (filtrosActuales.indexOf("?") == -1) {
            filtrosActuales = "?" + filtrosActuales;
        }
        let nuevaPagina = "";
        if (document.location.href.indexOf("?") == -1) {
            nuevaPagina = document.location.href + filtrosActuales;
        } else {
            indice = document.location.href.indexOf("?");
            nuevaPagina = document.location.href.substr(0, indice) + filtrosActuales;
        }

        // Carga de la página con el nuevo filtro
        document.location = nuevaPagina;
    }, 


    /**
     * Método para eliminar filtros activos de búsquedas de recursos multimedia en la sección de filtros activos
     * @param {} filtro 
     */
    LimpiarFiltroComponentes: function(filtro){
        // Mostrar loading
        loadingMostrar();
        // Página a cargar
        let nuevaPagina = "";
        indice = document.location.href.indexOf("?");
        nuevaPagina = document.location.href.substr(0, indice + 1);
        // Recarga de la página la página
        document.location = nuevaPagina;        
    },
    

    /**
     * Método para realizar búsquedas cuando se escriba en el input del buscador de items multimedia
     */
    handleSearchMultimediaItem: function(){    
        const that = this;
        let searchString = this.txtBuscarCMSMultimedia.val();
        // Eliminamos posibles tildes para búsqueda ampliada
        searchString = searchString.normalize('NFD').replace(/[\u0300-\u036f]/g, "").toLowerCase();
        if (searchString != ""){
            that.AgregarFiltroComponentes(`search=${searchString}`);
        }    
    }
    
}

/**
 * Operativa de funcionamiento de Editar Componentes CMS  
 */

const operativaGestionComponentsCMS = {

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
     * Configuración de las rutas de acciones necesarias para hacer peticiones a backend
     */
     configRutas: function (pParams) {
        // Url para llamar al modal para crear un nuevo item multimedia
        this.urlBase = refineURL();
        // Variable donde se guardará la url necesaria para el guardado del componente
        this.urlSaveComponent = ``;
    },
    
    /*
     * Inicializar elementos de la vista
     * */
    config: function (pParams) {
        
        // Idiomas existentes en la comunidad
        this.listaIdiomas = [];
        // Botón para volver atrás en la jerarquía de modales. Usado para posible navegación en "Grupo de componentes"
        this.backModalNavigationClassName = "backModalNavigation";

        // InputHack donde estarán los idiomas de la comunidad
        this.idiomasComunidadId = "idiomasComunidad"

        /* Buscador de items multimedia */
        // Input de buscador de items multimedia
        this.txtBuscarComponentItem = $("#txtBuscarComponente");
        // Lupa del buscador
        this.inputLupa = $("#inputLupa");
        // Input del nombre corto del componente
        this.inputNombreCortoComponenteId = "nombreCortocomponente"; 

        /* Nuevos items de Componentes */
        // Botones de tipo de componentes multimedia a añadir
        this.btnAddComponentItem = $(".btnAddComponentItem");
        // Botón para eliminar componentes 
        this.btnDeleteComponentItemClassName = 'btnDeleteComponentItem';
        this.btnDeleteComponentItem = $(`.${this.btnDeleteComponentItemClassName}`);
        // Botón para editar componente
        this.btnEditComponentItem = $(".btnEditComponentItem");
        // Botón para copiar la url del item multimedia
        this.btnCopyComponentItem = $(".btnCopyMultimediaItem");
        // Botón para guardar un determinado componente        
        this.btnSaveComponentId = "btnSaveComponent";
        this.btnSaveComponent = $(`#${this.btnSaveComponentClassName}`);
        // Contenedor de cada uno de los componentes CMS dentro de un Grupo de Componentes
        this.communityComponentsListClassName = "js-community-components-list";

        /* Elementos de la vista modal */
        // Botón para explorar una imagen y cargarla en el componente         
        this.inputLoadImageClassName = "inputLoadImage";
        this.inputLoadImage = $(`.${this.inputLoadImageClassName}`);
        // Botón para eliminar una imagen del componente_
        this.btnDeleteImageClassName = "btnDeleteImage";
        this.btnDeleteImage = $(`.${this.btnDeleteImageClassName}`);
        this.inputRequiredClassName = "required";
        this.inputRequired = $(`.${this.inputRequiredClassName}`);
        // Lista donde estarán los componentes de un componente de tipo "Contenedor de componentes"
        this.componentListClassName = "component-list";
        this.componentList = $(`.${this.componentListClassName}`);
        // Botón para buscar un componente y añadirlo a la lista de componentes
        this.btnAddComponentClassName = 'btnAddComponent';
        this.btnAddComponent = $(`.${this.btnAddComponentClassName}`);

        // Botón para añadir un nuevo componente a una lista de componentes
        this.btnAddNewComponentByIdClassName = "btnAddNewComponentById";
        this.btnAddNewComponentById = $(`.${this.btnAddNewComponentByIdClassName}`);
        // Botón para eliminar un componente del contenedor de componentes
        this.btnDeleteComponentFromContainerListClassName = "btnDeleteComponentFromContainerList";
        this.btnDeleteComponentFromContainerList = $(`.${this.btnDeleteComponentFromContainerListClassName}`);
        // Botón para editar un componente dentro del contenedor de componentes
        this.btnEditComponentFromContainerListClassName = "btnEditComponentFromContainerList";
        this.btnEditComponentFromContainerList = $(`.${this.btnEditComponentFromContainerListClassName}`);
        // Botón para visualizar un recurso dentro del contenedor de componentes/recursos
        this.btnViewResourceFromContainerListClassName = "btnViewResourceFromContainerList";
        this.btnViewResourceFromContainerList = $(`.${this.btnViewResourceFromContainerListClassName}`);        


        // Input para buscar componentes y asignarlos al "Contenedor de componentes"
        this.txtSearchIdComponentIdName = 'txtSearchIdComponent';
        this.txtSearchIdComponent = $(`#${this.txtSearchIdComponentIdName}`);

        // Panel de privacidad para buscar perfiles y grupos si el componente es "Privado"
        this.privacyPanelClassName = 'edit-privacy-panel-privacidad-perfiles-grupos';
        this.privacyPanel = $(`.${this.privacyPanelClassName}`);

        // RadioButton para seleccionar un componente de tipo privado o no. Mostrará u ocultará el panel de selección de perfiles/grupos        
        this.rbPrivacyComponentClassName = 'chkEditarPrivacidad';

        // Input de privacidad de perfiles para componentes añadir
        this.inputPrivacidadPerfilesComponentesIdName = 'privacidadPerfiles';
        this.inputPrivacidadPerfilesComponentes = $(`#${this.inputPrivacidadPerfilesComponentesIdName}`);
        // Input hack para guardar la privacidad de los perfiles asiganados a un componente
        this.txtHackComponentesPrivacyClassName = 'txtHackInvitadosPagina';
        this.txtHackComponentesPrivacy = $(`.${this.txtHackComponentesPrivacyClassName}`);
        this.btnRemoveTagProfileGroupClassName = 'tag-remove';

        // Botón para confirmar la eliminación de un recurso multimedia desde el modal de eliminación
        this.btnDeleteComponenteItemClassName = 'btn-delete-component-item'; 
        this.btnDeleteComponenteItem = $(`.${this.btnDeleteComponenteItemClassName}`);

        // Cada uno de los contenedores (idiomas) para los menus
        this.contenedorPrincipalMenuListClassName = "contenedorPrincipalMenuList";
        this.contenedorPrincipalMenuList = $(`.${this.contenedorPrincipalMenuListClassName}`);
        // Lista donde se alojarán los items del menú (Para un componente de tipo Menú)
        this.menuListContainerClassName = "js-community-components-menu-option-list"; 
        this.menuListContainer = $(`.${this.menuListContainerClassName}`);
        // Botón para crear o añadir un nuevo item al componente CMS de tipo menú
        this.linkAddOptionMenuClassName = 'linkAddOptionMenu';
        this.linkAddOptionMenu = $(`.${this.linkAddOptionMenuClassName}`);
        // Filas de los diferentes menú items del componente CMS de tipo Menú
        this.menuOptionRowClassName = 'menuOption-row';
        this.menuOptionRow = $(`.${this.menuOptionRowClassName}`);
        // Botón de eliminación de un menu-item dentro del componente CMS de tipo "Menú"
        this.btnDeleteMenuOptionClasName = 'btnDeleteMenuOption';
        this.btnDeleteMenuOption = $(`#${this.btnDeleteMenuOptionClasName}`);
        // Input del nombre y enlace del menú item
        this.inputNombreMenuItemClassName = 'inputNombreMenuItem';
        this.inputEnlaceMenuItemClassName = 'inputEnlaceMenuItem';
        // Input del nombre y enlace del menú item
        this.inputNombreMenuItem = $(`.${this.inputNombreMenuItemClassName}`);
        this.inputEnlaceMenuItem = $(`.${this.inputEnlaceMenuItemClassName}`);
        // Panel collapse que contiene la informaicón del menuItem collapse
        this.menuItemInfoClassName = 'menuItem-info';
        this.menuItemInfo = $(`#${this.inputEnlaceMenuItemClassName}`);
        // Input del nombre del formulario en el tipo de componente "Envío de correo"
        this.inputFormOptionNameClassName = 'inputNombreCampoItem';
        this.inputFormOptionName = $(`.${this.inputNombreCampoItemClassName}`);
        // Botón para añadir un nuevo campo de formulario al componente tipo "Envío de correo"
        this.linkAddMailOptionClassName = 'linkAddMailOption';
        this.linkAddMailOption = $(`.${this.linkAddMailOptionClassName}`);
        // Filas de los diferentes inputs para un formulario del componente CMS de tipo "Envío de correo"
        this.formOptionRowClassName = 'inputForm-row';
        this.formOptionRow = $(`.${this.formOptionRowClassName}`); 
        // Lista contenedora de los items del formulario
        this.menuFormListContainerClassName = "js-community-components-form-option-list";
        this.menuFormListContainer = $(`.${this.menuFormListContainerClassName}`);
        // Botón para eliminar un input del componente CMS de tipo "Envío de correo"
        this.btnDeleteFormInputClassName = `btnDeleteFormInput`;
        this.btnDeleteFormInput = $(`.${this.btnDeleteFormInputClassName}`);

        // Contenedor modal de contenido dinámico
        this.modalContainer = $("#modal-container");

        // Fila del componente que se está editando
        this.filaComponente = undefined;
        // Url que se utiliza para editar el componente
        this.urlEditComponent = undefined;
        // Url que se utiliza para guardar el componente. Se utilizará también en 'estructura_cms-builder' para poder guardar un componente desde el CMS Builder
        this.urlSaveComponent = undefined;       
        // Url que se utiliza para eliminar un componente        
        this.urlDeleteComponent = undefined;
        // Flag para controlar que sólo se comprueben y se pinten los menú items una sola vez
        this.isShowMenuItemsTriggered = false;
        // Función de retorno que utilizará para el guardado de un componente. Si es undefined, el componente se está editando en la sección de Estructura -> Listado de Componentes CMS
        // En caso contrario, se está utilizando desde otra sección (Ej: CMS Page Builder)
        this.completion = undefined ;
        // Flag que detecta si el componente es creado nuevo a través del PageBuilder. Sólo se gestionaría desde "estructura_cms-builder.js"
        this.isNewCreatedFromCSMBuilder = false;
        // Array de componentesCMS para almacenar la navegación para la edición de estos a través de vía modal cuando se edite un "Grupo de componentes desde modal" y se necesite realizar la navegación
        this.listComponentArray = [];
    },  
    
    /**
     * Configuración de eventos de elementos del Dom (Botones, Inputs...)     
     */
     configEvents: function (pParams) {
        const that = this;

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
            // Quitar la clasea de modal-con-tabs
            $(e.currentTarget).hasClass("modal-con-tabs") && that.modalContainer.removeClass("modal-con-tabs");  
            // Establecer a falso la preparación del MenuList para que vuelva a estar listo por si se abre un nuevo menú.
            that.isShowMenuItemsTriggered = false;      
        });

        // Botón del "header" para realizar la navegación jerárquica por componentes en Grupo de componentes
        configEventByClassName(`${that.backModalNavigationClassName}`, function(element){
            const $jqueryElement = $(element);
            $jqueryElement.off().on("click", function(){                 
                const button = $(this);                
                // Gestionar la navegación hacia atrás del modal desde "Grupo de componentes"
                that.setRemoveModalViewFromModalContainer();
            });	                        
        });        

        // Click en el botón para añadir un nuevo componente cms a través de la carga de un modal
        this.btnAddComponentItem.off().on("click", function(){                              
            // Url para crear un determinado componente (Modal)
            const urlModal = $(this).data("url");
            // Url para guardar el componente nuevo
            that.urlSaveComponent = $(this).data("urlsave");
            // Cargar la vista para crear un nuevo componente
            getVistaFromUrl(urlModal, 'modal-dinamic-content', '', function(result){
                if (result != requestFinishResult.ok){
                    // KO al cargar la vista
                    mostrarNotificacion("error", "Error al cargar el componente para su creación.");
                    dismissVistaModal();                                                     
                }else{
                    // Añadir el estilo de tabs al modal 
                    !that.modalContainer.hasClass("modal-con-tabs") && that.modalContainer.addClass("modal-con-tabs");                    
                }               
            });
        });   

        // Click en el botón para editar un nuevo componente cms
        this.btnEditComponentItem.off().on("click", function(){
            const editButton = $(this);
            // Seleccionar la fila del componente que se editará
            that.filaComponente = editButton.closest('.component-row');   
            // Url para editar el componente seleccionado
            that.urlEditComponent = $(this).data("urleditcomponent").replace("load-modal","");
            // Cargar la vista para mostrarla en el contenedor
            getVistaFromUrl(`${that.urlEditComponent}load-modal`, 'modal-dinamic-content', '', function(result){
                if (result == requestFinishResult.ok){                    
                    // OK al cargar la vista
                    // Añadir el estilo de tabs al modal 
                    !that.modalContainer.hasClass("modal-con-tabs") && that.modalContainer.addClass("modal-con-tabs");
                    // Asignar la url para guardar el componente
                    that.urlSaveComponent = editButton.data("urlsave");
                }else{
                    // KO al cargar la vista
                    dismissVistaModal();
                    mostrarNotificacion("error", "Error al cargar el componente para su edición");        
                }                
            });            
        });

        // Click en el botón para copiar la url del item del componente
        this.btnCopyComponentItem.on("click", function(){
            const urlString = $(this).data("url");            
            that.handleCopyToClickBoard(urlString);
        });
        
        // Realizar búsquedas de items multimedia al escribir (KeyUp)        
        this.txtBuscarComponentItem.on("keyup", function(e){
            // Buscar al pulsar tecla Intro
            if (e.keyCode == 13){
                that.handleSearchComponentItem();
            }
        });

        // Click en la lupa para realizar búsquedas de items multimedia
        this.inputLupa.on("click", function(){
            that.handleSearchComponentItem();
        });
        
        // Botón para cargar una imagen en un componente
        configEventByClassName(`${that.inputLoadImageClassName}`, function(element){
            const $jqueryElement = $(element);
            $jqueryElement.off().on("change", function(){                 
                const inputButton = $(this);
                const idioma = inputButton.data("idiomapanel");              
                that.handleLoadImageForComponent(inputButton, idioma);                
            });	                        
        }); 

        // Botón para eliminar una imagen en un componente
        configEventByClassName(`${that.btnDeleteImageClassName}`, function(element){
            const $jqueryElement = $(element);
            $jqueryElement.off().on("click", function(){                 
                const inputButton = $(this);
                const idioma = inputButton.data("idiomapanel");              
                that.handleDeleteImageForComponent(inputButton, idioma);                
            });	                        
        });   
        
        // Controlar los inputs que son "required" para que no sean "nulos"
        configEventByClassName(`${that.inputRequiredClassName}`, function(element){
            const $jqueryElement = $(element);
            $jqueryElement.off().on("input", function(){                 
                const input = $(this);
                comprobarInputNoVacio(input, true, false, "Esta propiedad no puede estar vacía.", 0);                          
            });	                        
        });

        // Añadir un componente a un contenedor de componentes
        configEventByClassName(`${that.btnAddNewComponentByIdClassName}`, function(element){
            const $jqueryElement = $(element);
            $jqueryElement.off().on("click", function(){                 
                const button = $(this);                
                that.handleAddNewComponentToComponentsContainer(button);
            });	                        
        });
        
        // Cargar los nombres de los componentes dentro del contenedor de componentes (Editando un Grupo de Componentes)
        configEventByClassName(`${that.componentListClassName}`, function(element){
            const $listComponent = $(element);
            // Lista de componentes aparecida en pantalla                       
            if ($listComponent.length > 0){                
                that.handleLoadAndCheckComponentName($listComponent);
            }                         
        });  

        
        // Click para editar un componente dentro de un contenedor de componentes
        configEventByClassName(`${that.btnEditComponentFromContainerListClassName}`, function(element){
            const $jqueryElement = $(element);
            $jqueryElement.off().on("click", function(){                 
                const button = $(this);     
                // Url para realizar la petición para editar un componente            
                const urlLoadModalComponent = button.data("urleditcomponent");
                that.handleLoadComponentFromComponentContainer(urlLoadModalComponent, button);
            });	            
        }); 


        // Click para eliminar un componente dentro de un contenedor de componentes
        configEventByClassName(`${that.btnDeleteComponentFromContainerListClassName}`, function(element){
            const $jqueryElement = $(element);
            $jqueryElement.off().on("click", function(){                 
                const button = $(this);                
                that.handleDeleteComponentFromComponentContainer(button);
            });	            
        }); 
        
        
        // Buscador de componentes al hacer keyUp en el input de búsqueda de Componentes        
        configEventById(this.txtSearchIdComponentIdName, function(element){
            const $jqueryElement = $(element);
            $jqueryElement.off().on("keyup", function(e){                 
                const input = $(this);                                
                // Buscar / Comprobar el componente al pulsar tecla Intro                
                if (e.keyCode == 13){
                    that.handleSearchComponentItemAndCheckIfNotExist(input);
                }
            });	
        });

        // Botón de añadir / Buscar nuevo componente a contenedor de compontentes
        configEventByClassName(this.btnAddComponentClassName, function(element){
            const $jqueryElement = $(element);
            $jqueryElement.off().on("click", function(e){                 
                const button = $(this);                                                                
                // Input para realizar la búsqueda de componentes
                const input = $(`#${that.txtSearchIdComponentIdName}`);
                that.handleSearchComponentItemAndCheckIfNotExist(input);                
            });	
        }); 

        // RadioButton para cambiar la privacidad del componente
        configEventByClassName(this.rbPrivacyComponentClassName, function(element){
            const $jqueryElement = $(element);
            $jqueryElement.off().on("change", function(e){                 
                const radioButton = $(this);                                                                                                
                that.handleShowHidePrivacyPanelForComponent();                
            });	
        }); 

        // Botón (X) para poder eliminar usuarios/grupos desde el Tag para la privacidad del componente                          
        configEventByClassName(`${that.btnRemoveTagProfileGroupClassName}`, function(element){
            const $jqueryElement = $(element);
            $jqueryElement.off().on("click", function(){                 
                const $itemRemoved = $jqueryElement.parent().parent();                                                                                                              
                that.handleClickRemoveTagItem($itemRemoved);                
            });	                        
        });        

        // Input de buscador de perfiles y grupos para determinar la privacidad del componente            
        configEventById(`${that.inputPrivacidadPerfilesComponentesIdName}`, function(element){
            const $input = $(element);            
            if ($input.length > 0){                                
                that.handleConfigureAutoCompleteForInputPrivacidadPerfiles($input);            
            }
        });

        // Botón de confirmación de eliminación de un componente CMS desde modal
        configEventByClassName(`${that.btnDeleteComponenteItemClassName}`, function(element){
            const $jqueryElement = $(element);
            $jqueryElement.off().on("click", function(){ 
                // Obtener la url de confirmar el borrado del botón que abre el modal
                that.urlDeleteComponent = that.triggerModalContainer.data("url");
                that.handleConfirmRemoveComponentItem(that.urlDeleteComponent);                
            });	                        
        }); 

        // Cargar el compontamiento Drag/Sort para los menu items cuando aparezca el contenedor de menú items
        configEventByClassName(`${that.menuListContainerClassName}`, function(element){
            const $listComponent = $(element);
            // Lista de menu items aparecida en pantalla                       
            if ($listComponent.length > 0){                                
                that.configureDragSortMenuListComponent();
                // Ejecutar la ordenación de los items en base al nivel de cada menu item. Hacerlo sólo una vez
                if (that.isShowMenuItemsTriggered == false){
                    that.showMenuItemsBasedOnItsLevel();
                }
            }                         
        }); 
        
        // Botón para añadir un nuevo enlace o menú item al componente CMS de tipo "Menú"
        configEventByClassName(`${that.linkAddOptionMenuClassName}`, function(element){
            const $jqueryElement = $(element);
            $jqueryElement.off().on("click", function(){                 
                that.handleAddNewMenuItem($jqueryElement);                
            });	                        
        });  
        
        // Botón para eliminar un enlace o menú item del componente CMS de tipo "Menú"
        configEventByClassName(`${that.btnDeleteMenuOptionClasName}`, function(element){
            const $jqueryElement = $(element);
            $jqueryElement.off().on("click", function(){                 
                that.handleDeleteMenuOptionItem($jqueryElement);                
            });	                        
        });

        // Input del nombre del menú item. Controlar el cambio de nombre        
        configEventByClassName(`${that.inputNombreMenuItemClassName}`, function(element){
             const $jqueryElement = $(element);
             $jqueryElement.off().on("keyup", function(){                 
                const input = $(this);
                that.handleKeyUpForNameMenuItem(input);
             });	                        
        });

        // Input del nombre del menú item. Controlar el cambio de enlace       
        configEventByClassName(`${that.inputEnlaceMenuItemClassName}`, function(element){
            const $jqueryElement = $(element);
            $jqueryElement.off().on("keyup", function(){                 
                const input = $(this); 
                that.handleKeyUpForUrlMenuItem(input);
            });	                        
        });    
        
        // Disparar el comportamiento de select cuando aparezca select con la clase
        configEventByClassName('js-select2', function(element){
            const $jqueryElement = $(element);
            if ($jqueryElement.length > 0){                                
                comportamientoInicial.iniciarSelects2();
            }                        
        });

        // Input del nombre de un campo del formulario del tipo de componente "Envío de correo"        
        configEventByClassName(`${that.inputFormOptionNameClassName}`, function(element){
            const $jqueryElement = $(element);
            $jqueryElement.off().on("keyup", function(){                 
               const input = $(this);
               that.handleKeyUpForNameFormItem(input);
            });	                        
        }); 
       
        // Botón para añadir un nuevo campo al formulario para el tipo de componente "Envío de correo"                
        configEventByClassName(`${that.linkAddMailOptionClassName}`, function(element){
            const $jqueryElement = $(element);
            $jqueryElement.off().on("click", function(){                 
                that.handleAddNewFormItem($jqueryElement);                
            });	                        
        }); 
        
        // Botón para eliminar un cmapo del formulario del tipo de componente "Envío de correo"
        configEventByClassName(`${that.btnDeleteFormInputClassName}`, function(element){
            const $jqueryElement = $(element);
            $jqueryElement.off().on("click", function(){                 
                that.handleDeleteFormItem($jqueryElement);                
            });	                        
        });
        
        // Cargar el compontamiento Drag/Sort para los menu items cuando aparezca el contenedor de menú items
        configEventByClassName(`${that.menuFormListContainerClassName}`, function(element){
            const $listComponent = $(element);
            // Lista de menu items aparecida en pantalla                       
            if ($listComponent.length > 0){                                
                that.configureDragSortFormItemsListComponent();                
            }                         
        });     
        
        // Botón para el guardado del componente        
        configEventById(`${that.btnSaveComponentId }`, function(element){
            const $button= $(element);            
            $button.off().on("click", function(){                 
                that.handleSaveComponent();                                
            });	                         
        });

        configEventById(`${that.idiomasComunidadId}`, function(element){
            const $input = $(element);
            // Input hack en pantalla
            if ($input.length > 0){                                
                that.getAndSetIdiomasComunidad($input);                
            }  
        });       
    },

    /**
     * Método para obtener y establecer en la variable 'idiomasComunidad' los idiomas existentes
     * para el posterior guardado correcto de los componentes CMS
     * @param {jqueryElement} input: Input donde se almacenan los idiomas de la comunidad
     */
    getAndSetIdiomasComunidad: function(input){
        const that = this;
        // Reseteo de idiomas para cargalos una vez se edite/cree el componente
        that.listaIdiomas = [];
        const idiomasComunidad = input.val().split('&&&');
        $.each(idiomasComunidad, function () {
            if (this != "") {
                that.listaIdiomas.push(this.split('|')[0]);
            }
        });
    },

    /**
     * Método para eliminar un determinado campo del formulario del componente "Envío de correo"
     * @param {jqueryElement} button 
     */
    handleDeleteFormItem: function(button){
        const that = this;
        // Id panel de donde eliminar el nuevo item para el menú (En el idioma deseado)
        const idPanel = button.data("idpanel");        
        // Buscar la fila a eliminar según el botón de eliminar pulsado
        const menuOptionRow = button.closest(`.${that.formOptionRowClassName}`);
        // Eliminar el item (y subItems) del DOM
        menuOptionRow.remove();
    },


    /**
     * Método para añadir un nuevo campo al formulario en el tipo de componente CMS "Envío de correo"
     * @param {jqueryElement} button Botón para añadir un nuevo campo al formulario
     */
    handleAddNewFormItem: function(button){
        const that = this;
        // Id panel donde añadir el nuevo item para el menú (En el idioma deseado)
        const idPanel = button.data("idpanel");
        // Menú de items donde añadir el nuevo elemento del Menú (teniendo en cuenta el idioma en el que se encuentra)
        const formListItems = $(`#contenedor_${idPanel}`);        
        // Nº de elementos de items 
        const numFormItems = formListItems.find(`.${that.formOptionRowClassName}`).length;
        // Generar nº aleatorio para el collapse panel
        const idCollapsePanel = `panel_collapse_${guidGenerator().substring(0, 5)}`;
        // Generar los inputsId para el campoId, radioButton de obligatorio y de Select
        const campoId = `${idPanel}_${numFormItems}_txt`;        
        const tipoCampoId = `${idPanel}_${numFormItems}_ddlist`;
        
        const templateFormItem = 
        `
        <li class="component-wrap inputForm-row elementos_${idPanel}" draggable="false">
            <div class="component">
                <div class="component-header-wrap">
                    <div class="component-header">
                        <div class="component-sortable js-component-form-item-sortable-component">
                            <span class="material-icons-outlined sortable-icon">drag_handle</span>
                        </div>
                        <div class="component-header-content">
                            <div class="component-header-left">
                                <div class="component-name-wrap">                                    
                                    <span class="component-name">
                                        <span class="component-form-option-name"></span>																						
                                    </span>
                                </div>
                            </div>
                            <div class="component-header-right">
                                <div class="component-actions-wrap">
                                    <ul class="no-list-style component-actions">
                                        <li>                                            
                                            <a
                                                class="action-delete round-icon-button js-action-delete btnEditFormInput"
                                                href="javascript: void(0);"													
                                                data-toggle="collapse" 
                                                data-target="#${idCollapsePanel}"									   
                                                role="button" 
                                                aria-expanded="false" 
                                                aria-controls="${idCollapsePanel}"
                                                draggable="false">
                                                <span class="material-icons">edit</span>
                                            </a>
                                        </li>
                                        <li>                                            
                                            <a
                                                class="action-delete round-icon-button js-action-delete btnDeleteFormInput"
                                                href="javascript: void(0);"													
                                                data-idpanel="${idPanel}"
                                                draggable="false">
                                                <span class="material-icons">delete</span>
                                            </a>
                                        </li>
                                    </ul>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>            
            <div class="collapse menuItem-info" id="${idCollapsePanel}">					
                <div class="card card-body">												                    
                    <div class="form-group mb-4">
                        <label class="control-label d-block">Nombre</label>
                        <input
                            type="text"
                            id="${campoId}"
                            data-labeltext="Nombre"	
                            placeholder="Nombre"
                            class="form-control ${that.inputFormOptionNameClassName}"
                            value=""/>
                        <small class="form-text text-muted mb-2">Nombre del campo del formulario.</small>
                    </div>	                                    
                    <div class="form-group mb-3 esCampoObligatorio">
                        <label class="control-label d-block">Campo obligatorio</label>
                        <div class="form-check form-check-inline">
                            <input
                                id="esCampoObligatorio_SI_${campoId}"
                                class="form-check-input esCampoObligatorio"
                                data-value="si"
                                type="radio"
                                checked
                                name="esCampoObligatorio_${campoId}"/>
                            <label
                                class="form-check-label"
                                for="esCampoObligatorio_SI_${campoId}">Sí</label>
                        </div>
                        <div class="form-check form-check-inline">
                            <input
                                id="esCampoObligatorio_NO_${campoId}"
                                class="form-check-input esCampoObligatorio"
                                type="radio"
                                data-value="no"                                
                                name="esCampoObligatorio_${campoId}"/>
                            <label
                                class="form-check-label"
                                for="esCampoObligatorio_NO_${campoId}">No</label>
                        </div>
                        <small class="form-text text-muted mb-2 mt-n2">El campo es obligatorio que sea rellenado en el momento del envío del formulario.</small>
                    </div>                                    
                    <div class="form-group mb-4 tipoCampo">
                        <label class="control-label d-block">Tipo de campo</label>                        
                        <select name="tipoCampo"
                                class="cmbTipoCampo js-select2"
                                id="${tipoCampoId}"
                                tabindex="0"
                                aria-hidden="false">
                            <option selected value="0">Corta</option>
                            <option value="1">Larga</option>
                        </select>
                    </div>
                    <small class="form-text text-muted mb-2 mt-n2">Selecciona el tipo de campo del formulario.</small>
                </div>
            </div>	
        </li>	
        `;

        // Añadir el nuevo item del formulario menú al contenedor especificado
        formListItems.find(`.${that.menuFormListContainerClassName}`).append(templateFormItem);
        // Botón de editar el menu item recién creado
        const formItemEditButtonCreated = $(`*[data-target="#${idCollapsePanel}"]`);
        // Colapsar todos los menús desplegables del modal para que no se queden abiertos
        const collapsePanels = that.modalContainer.find('.collapse');
        collapsePanels.collapse("hide");        
        // Hacer click para abrir por defecto el nuevo item agregado
        formItemEditButtonCreated.trigger("click");        
    },

    /**
     * Método para configurar la posibilidad de hacer drag & drop en los items del componente de tipo "Envío de correo"
     */
    configureDragSortFormItemsListComponent: function () {
        const that = this;
        /* Lista donde estarán los links del menú */
        const components_lists = document.querySelectorAll(`.${that.menuFormListContainerClassName}`);
        const componentsOptions = that.getDragSortFormItemsListOptions();
        components_lists.forEach((component_list) => {
            Sortable.create(component_list, componentsOptions);
        });
    },

    /**
     * Método para obtener la configuración para la lista DragSort del componente CMS "Envío de correo"
     * @returns Devuelve un objeto con la configuración para las listas Draggable y Sortable
     */
    getDragSortFormItemsListOptions: function () {
        const that = this;
        return {
            group: {                
                name: `${that.menuFormListContainerClassName}`,
            },
            sort: true,
            fallbackOnBody: true,
            swapThreshold: 0.65,
            animation: 150,
            handle: ".js-component-form-item-sortable-component",
            onAdd: function (evt) {},
            onChoose: function (evt) {},
            onUnChoose: function (evt) {},            
            onEnd: function (evt) {},
        };
    },    


    /**
     * Método para controlar el cambio de nombre de un campo del formulario
     * @param {jqueryElement} input 
     */
    handleKeyUpForNameFormItem: function(input){
        const that = this;
        // Detectar la fila a editar
        const formOptionRow = input.closest('.inputForm-row');  
        const newMenuNameItem = input.val();                 
        // Nombre del parámetro a cambiar al hacer keyUp
        const menuItemNameValue = formOptionRow.find(".component-form-option-name");
        // Asignar lo tecleado (Nuevo nombre del parámetro)
        menuItemNameValue.html(newMenuNameItem.trim());  
    },

    /**
     * Método para cambiar el nombre del menu item cuando se esté editando
     * @param {jqueryElement} input 
     */
    handleKeyUpForNameMenuItem: function(input){                        
        const that = this;
        // Detectar la fila a editar
        const menuOptionRow = input.closest('.menuOption-row');  
        const newMenuNameItem = input.val();  
        // Obtener el id del input que se está modificando para modificar el label correspondiente y cambiar el nombre para acceder a su label
        const labelNombreId = input.attr("id").replace("txt","label");
        // Nombre del parámetro a cambiar al hacer keyUp
        const menuItemNameValue = menuOptionRow.find(`#${labelNombreId}`);
        // Asignar lo tecleado (Nuevo nombre del parámetro)
        menuItemNameValue.html(newMenuNameItem.trim());        
    },

    /**
     * Método para cambiar el enlace del menu item cuando se esté editando
     * @param {jqueryElement} input 
     */
    handleKeyUpForUrlMenuItem: function(input){
        const that = this;
        // Detectar la fila a editar
        const menuOptionRow = input.closest('.menuOption-row');  
        const newMenuUrlItem = input.val();   
        // Obtener el id del input que se está modificando para modificar el label correspondiente y cambiar el nombre para acceder a su label
        const labelEnlaceId = input.attr("id").replace("txt","label");          
        // Nombre del parámetro a cambiar al hacer keyUp
        const menuItemUrlValue = menuOptionRow.find(`#${labelEnlaceId}`);
        // Asignar lo tecleado (Nuevo nombre del parámetro)
        menuItemUrlValue.html(newMenuUrlItem.trim());         
    },    

    /**     
     * Método para eliminar la opción del menú elegida. Si esta incluye menús hijos también se eliminarán.
     * @param {jqueryElement} button Botón que se ha pulsado para eliminar el menú item
     */
    handleDeleteMenuOptionItem: function(button){
        const that = this;
        // Id panel de donde eliminar el nuevo item para el menú (En el idioma deseado)
        const idPanel = button.data("idpanel");
        // Menú de items donde buscar el elemento del Menú a eliminar (teniendo en cuenta el idioma en el que se encuentra)
        const menuListItems = $(`#contenedor_${idPanel}`);
        // Buscar la fila a eliminar según el botón de eliminar pulsado
        const menuOptionRow = button.closest(`.${that.menuOptionRowClassName}`);
        // Eliminar el item (y subItems) del DOM
        menuOptionRow.remove();
    }, 

    /**
     * Método para añadir un nuevo menú item al componente CMS de tipo Menú
     * @param {jqueryElement} button : Botón que se ha pulsado para crear un nuevo menú item
     */
    handleAddNewMenuItem: function(button){
        const that = this;
        // Id panel donde añadir el nuevo item para el menú (En el idioma deseado)
        const idPanel = button.data("idpanel");
        // Menú de items donde añadir el nuevo elemento del Menú (teniendo en cuenta el idioma en el que se encuentra)
        const menuListItems = $(`#contenedor_${idPanel}`);
        // Panel donde se encuentran todos los menu-items. Añadirlo al primero (Root)
        const panelListItems = menuListItems.find(`.${that.menuListContainerClassName}`).first();        
        // Nº de elementos de items 
        const numMenuItems = menuListItems.find(`.${that.menuOptionRowClassName}`).length;
        // Generar nº aleatorio para el collapse panel
        const idCollapsePanel = `panel_collapse_${guidGenerator().substring(0, 5)}`;  
        // Generar los inputsId para el nombre y la url del menú a añadir        
        const labelNombreId = `${idPanel}_${numMenuItems}_label_nombre`;
        const inputNombreId = `${idPanel}_${numMenuItems}_txt_nombre`;
        const labelUrlId = `${idPanel}_${numMenuItems}_label_enlace`;
        const inputUrlId = `${idPanel}_${numMenuItems}_txt_enlace`;

        // Añadir un nuevo item al final en menuListItems
        const templateMenuItem =
        `
        <li
            class="component-wrap menuOption-row elementos_${idPanel}"
            data-componentid=""
            data-nivel="0"            
            draggable="false">
            <div class="component">
                <div class="component-header-wrap">
                    <div class="component-header">
                        <div class="component-sortable js-component-menu-item-sortable-component">
                            <span class="material-icons-outlined sortable-icon">drag_handle</span>
                        </div>
                        <div class="component-header-content">
                            <div class="component-header-left">
                                <div class="component-name-wrap">                                    
                                    <span class="component-name">
                                        <span id="${labelNombreId}" class="component-menu-option-name"></span>
                                        <span> | </span>
                                        <span id="${labelUrlId}" class="component-menu-option-url"></span>
                                    </span>
                                </div>
                            </div>
                            <div class="component-header-right">
                                <div class="component-actions-wrap">
                                    <ul class="no-list-style component-actions">
                                        <li>                                            
                                            <a
                                                class="action-delete round-icon-button js-action-delete btnEditMenuOption"
                                                href="javascript: void(0);"
                                                data-componentid=""
                                                data-toggle="collapse" 
                                                data-target="#${idCollapsePanel}"									   
                                                role="button" 
                                                aria-expanded="false" 
                                                aria-controls="${idCollapsePanel}"
                                                draggable="false">
                                                <span class="material-icons">edit</span>
                                            </a>
                                        </li>
                                        <li>                                            
                                            <a
                                                class="action-delete round-icon-button js-action-delete btnDeleteMenuOption"
                                                href="javascript: void(0);"
                                                data-componentid=""
                                                draggable="false">
                                                <span class="material-icons">delete</span>
                                            </a>
                                        </li>
                                    </ul>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="component-content">
                <ul class="js-community-components-menu-option-list component-list no-list-style" data-parentKey="">

                </ul>
            </div>
            <div class="collapse menuItem-info" id="${idCollapsePanel}">					
                <div class="card card-body">												
                    
                    <div class="form-group mb-4">
                        <label class="control-label d-block">Nombre</label>
                        <input
                            type="text"
                            id="${inputNombreId}"
                            data-labeltext="Nombre"	
                            placeholder="Nombre"
                            class="form-control ${that.inputNombreMenuItemClassName}"
                            value=""/>
                        <small class="form-text text-muted mb-2">Nombre del item del menú.</small>
                    </div>			
                                
                    <div class="form-group mb-2">
                        <label class="control-label d-block">Enlace</label>
                        <input
                            type="text"									
                            id="${inputUrlId}"
                            placeholder="Url del menú"
                            class="form-control ${that.inputEnlaceMenuItemClassName}"
                            value=""/>
                        <small class="form-text text-muted mb-2">Enlace del item del menú.</small>
                    </div>			
                </div>
            </div>	
        </li>	        
        `; 

        // Añadir el nuevo item del menú al contenedor especificado de Menú Item (Idioma)
        panelListItems.append(templateMenuItem);
        // Botón de editar el menu item recién creado
        const menuItemeditButtonCreated = $(`*[data-target="#${idCollapsePanel}"]`);
        // Colapsar todos los menús desplegables del modal para que no se queden abiertos
        const collapsePanels = that.modalContainer.find('.collapse');
        collapsePanels.collapse("hide");        
        // Hacer click para abrir por defecto el nuevo item agregado
        menuItemeditButtonCreated.trigger("click");
    },


    /**
     * Método para configurar la posibilidad de hacer drag & drop en los menús items de tipo "Menú Item"
     */
    configureDragSortMenuListComponent: function () {
        const that = this;
        /* Lista donde estarán los links del menú */
        const components_lists = document.querySelectorAll(`.${that.menuListContainerClassName}`);
        const componentsOptions = that.getDragSortMenuListOptions();
        components_lists.forEach((component_list) => {
            Sortable.create(component_list, componentsOptions);
        });
    },
    /**
     * Método para obtener la configuración para la lista DragSort de MenuList items
     * @returns Devuelve un objeto con la configuración para las listas Draggable y Sortable
     */
    getDragSortMenuListOptions: function () {
        const that = this;
        return {
            group: {
                name: "js-community-components-menu-option-list",
            },
            sort: true,
            fallbackOnBody: true,
            swapThreshold: 0.65,
            animation: 150,
            handle: ".js-component-menu-item-sortable-component",
            onAdd: function (evt) {},
            onChoose: function (evt) {},
            onUnChoose: function (evt) {},            
            /**
             * Tener en cuenta el nuevo nivel del MenuItem arrastrado
             */
            onEnd: function (evt) {
                // Elemento arrastrado
                const $item = $(evt.item);
                // Nivel del item arrastrado. Por defecto, 0 (Movido dentro del raíz)
                let itemNivel = 0
                // Lista padre destino del elemento arrastrado
                const $listDestiny = $(evt.to);                       
                // Obtener item padre del menú (para saber si se está poniendo a modo de subnivel)
                const parentList = $listDestiny.closest(`.${that.menuOptionRowClassName}`);
                if (parentList.length > 0){
                    // El item se ha insertado dentro de un item - Obtener el data-level de ese item
                    itemNivel = parentList.data("nivel") + 1;                
                }
                // Asignar el nivel al item arrastrado
                $item.data("nivel", itemNivel);
                // Asignar el índice y cambiar su clase (Para los idiomas configurados, es, en...)
                // propiedad30_es_0, propiedad30_en_índice
                $item.data("indice", evt.newIndex);                                                                                
            },
        };
    },

    /**
     * Método que mostrará al usuario los menú items del menú basados en el nivel que tengan estos para dejar los items, indentados
     * y con el padre correspondiente
     */
     showMenuItemsBasedOnItsLevel: function(){
        const that = this;
        that.isShowMenuItemsTriggered = true;
        
        // Lista de contenedores de los menús (MultiIdiomas)
        const contenedoresMenus = $(`.${that.contenedorPrincipalMenuListClassName}`);
        loadingMostrar($(contenedoresMenus[0]));

        $.each(contenedoresMenus, function(){
            const contenedor = $(this);
            // Panel raíz (El primero) donde se alojan todos los menú items (Del idioma en cuestión)
            const rootPanelListItemMenu = contenedor.find(`.${that.menuListContainerClassName}`).first();
            // Menú items existentes del Menú en el idioma deseado
            const rowItems = rootPanelListItemMenu.find(`.${that.menuOptionRowClassName}`);
            // Buscar dentro de cada fila 
            $.each(rowItems, function(index){
                // Cada menu item analizado
                const rowItem = $(this);
                let nivel = rowItem.data("nivel");

                // El menú item está dentro del padre anterior
                if (nivel != 0){
                    // Menú item que será el padre de este nuevo menu item
                    const parentRowItem = rowItem.prev() ;
                    // Encontrar el lugar de los hijos donde se añadirá
                    if (nivel == 1){
                        // Si el nivel es 1, hay que meterlo en el nivel 0 del padre
                        nivel -= 1;
                    }
                    const parentRowChildrenListItems = $(parentRowItem.find(`.${that.menuListContainerClassName}`)[nivel]);
                    parentRowChildrenListItems.append(rowItem);                    
                }                        
            });     
        });
        setTimeout(function(){loadingOcultar();},500);        
    },
    
    /**
     * Método para eliminar un determinado componente habiendo pulsado en la confirmación desde el modal 
     * @param {string} urlDeleteComponent Url para realizar la petición para proceder a la eliminación de un componente
     */
    handleConfirmRemoveComponentItem: function(urlDeleteComponent){        
        const that = this;
        // Mostrar loading
        loadingMostrar();
        
        // Realizar la petición para el borrado del componente
        GnossPeticionAjax(
            urlDeleteComponent,
            null,
            true
        ).done(function (data) {
            // OK - Borrado del componente correcto
            mostrarNotificacion("success", "Componente eliminado correctamente");                        
            // Ocultar el modal
            dismissVistaModal();                        
            setTimeout(function(){                
                // Recargar la página sin filtros activos
                that.LimpiarFiltroComponentes(true);                
            },500);            
        }).fail(function (data) {
            mostrarNotificacion("error", data);            
        }).always(function () {
            // Ocultar loading
            loadingOcultar();                        
        });        
    },

    LimpiarFiltroComponentes: function (filtro) {
        // Mostrar loading
        loadingMostrar();
        let nuevaPagina = "";
        const indice = document.location.href.indexOf("?");
        nuevaPagina = document.location.href.substr(0, indice + 1);
        // Recargar la página
        document.location = nuevaPagina;
    },
    
    /**
     * Método para eliminar un item/Tag de la privacidad de componentes.
     * @param {jqueryElement} itemDeleted 
     */
    handleClickRemoveTagItem: function(itemDeleted){        
        const that = this;        
        // Buscar el input oculto y seleccionar la propiedad del id que corresponde con el grupo a eliminar
        const idItemDeleted = itemDeleted.prop("id"); 
        // Items id dependiendo del tipo a borrar (Perfil, Grupo)
        let itemsId = "";
        // Input del que habrá que eliminar el item seleccionado (Perfil, Grupo)
        let $inputHack = $(`#${that.txtHackComponentesPrivacyClassName}`);   
        
        itemsId = $inputHack.val().split(",");
        itemsId.splice( $.inArray(idItemDeleted, itemsId), 1 );
        // Pasarle los datos de los grupos actuales al input hidden
        $inputHack.val(itemsId.join(",")); 
        // Eliminar el grupo del contenedor visual
        itemDeleted.remove();
    }, 

    /**
     * Método para mostrar u ocultar el panel de privacidad de un componente     
     */
    handleShowHidePrivacyPanelForComponent: function(){
        const that = this;
        // Panel de privacidad a mostrar u ocultar        
        const panelPrivacidad = $(`.${that.privacyPanelClassName}`);
                
        if ($("input[name="+that.rbPrivacyComponentClassName+"]:checked").data("value") == "si"){
            // Mostrar el panel
            panelPrivacidad.removeClass('d-none');
        }else{
            // Ocultar el panel
            panelPrivacidad.addClass('d-none');
        } 
    },

    /**
     * Método para configurar el autoComplete para la búsqueda de perfiles y grupos para asociarlos a un componente
     * @param {jqueryElement} input: Input al que se le aplicará el comportamiento de autoComplete
     */
    handleConfigureAutoCompleteForInputPrivacidadPerfiles: function(input){
        const that = this;

        // Configuración autoComplete
        input.autocomplete(
            null,
            {
                url: $('#inpt_urlServicioAutocompletar').val() + "/AutoCompletarLectoresEditores",
                delay: 0,
                scroll: false,
                max: 30,
                selectFirst: false,
                minChars: 1,
                width: 300,
                cacheLength: 0,
                NoPintarSeleccionado: true,
                multiple: true,
                // Clase del input donde se gestionarán los Invitados al componente
                classTxtValoresSelecc: `${that.txtHackComponentesPrivacyClassName}`,

                extraParams: {
                    grupo: '',
                    identidad: $('#inpt_identidadID').val(),
                    organizacion: '',
                    proyecto: $('#inpt_proyID').val(),
                    bool_edicion: 'false',
                    bool_traergrupos: 'true',
                    bool_traerperfiles: 'true'
                }
            }
        );

        // Configuración del método result (Selección realizada )
        input.result(function (event, data, formatted) {                                
            const dataName = data[0];
            const dataId = data[1];
            const input = $(event.currentTarget);
            that.handleSelectAutocompleteProfileForPrivacyComponent(input, dataName, dataId);
        });
    },

    /**
     * Método que se ejecutará cuando se seleccione un perfil o grupo para asignar la privacidad de un componente
     * @param {jqueryElement} input : Input que ha disparado el autoComplete
     * @param {*} dataName: Nombre
     * @param {*} dataId : Id del elemento seleccionado
     */
    handleSelectAutocompleteProfileForPrivacyComponent: function(input, dataName, dataId){
        // Panel/Sección donde se ha realizado toda la operativa
        const tagsSection = $(input).parent().parent();

        // Contenedor de items donde se añadirá el nuevo seleccionado para su visualización
        const tagContainer = tagsSection.find(".tag-list");
        // Input oculto donde se añadirá el nuevo item seleccionado
        const inputHack = tagsSection.find("input[type=hidden]").first();
        // Añadido el id del item seleccionado al inputHack                
        inputHack.val(inputHack.val() + dataId + ',');
    
        // Etiqueta del item seleccionado
        let privacyProfileComponent = '';
        privacyProfileComponent += '<div class="tag" id="'+ dataId +'">';
            privacyProfileComponent += '<div class="tag-wrap">';
                privacyProfileComponent += '<span class="tag-text">' + dataName + '</span>';
                privacyProfileComponent += "<span class=\"tag-remove material-icons\">close</span>";
                privacyProfileComponent += '<input type="hidden" value="'+ dataId +'" />';
            privacyProfileComponent += '</div>';
        privacyProfileComponent += '</div>';

        // Añadir el item en el contenedor de items para su visualización
        tagContainer.append(privacyProfileComponent);

        // Vaciar el input donde se ha escrito 
        $(input).val('');        
    },

    
    /**
     * Método para cargar una imagen seleccionada a través de un input type file. Cargará la foto no en el servidor sino la dejará mostrada al usuario en 
     * la sección correspondiente     
     * @param {jqueryElement} input : Input que ha disparado la acción para buscar una imagen a cargar
     */
    handleLoadImageForComponent: function(input, idioma){
        // Cargar la imagen y actuar en completion
        uploadImageFileFromInput(input, function(data, imgFile, file){
            // Establecer la imagen en la src correspondiente
            //$('img', panImg).attr('src', imgFile);
            const imgView = input.parent().parent().find("img");
            // Input hidden donde asociada a la propiedad            
            const inputProperty = $(`#${input.data("idpanel")}`);
            imgView.attr("src", imgFile);
            // Establecer la propiedad de la imagen en el input hidden            
            inputProperty.val("File:" + file.name + ';Data:' + data);
            // Visualizar el panel de la imagen para su eliminación si fuera necesario
            // Panel contenedor de de la imagen
            const panImg = input.parent().parent().find(".panelImg");            
            panImg.removeClass("d-none");
        });
    }, 

    /**
     * Método para eliminar una imagen existente en un componente CMS.
     * @param {jqueryElement} input : Botón pulsado para el borrado de la imagen
     * @param {string} idioma : Idioma en el que se desea borrar la imagen del componente
     */
    handleDeleteImageForComponent: function(input, idioma){

        const idPanel = $(input).data("idpanel");
        // Imagen a eliminar
        const imgView = input.parent().parent().find("img");
        // Panel contenedor de de la imagen
        const panImg = input.parent().parent();
        // Input de selección de imagen
        const imgFile = $(`#file_${idPanel}`);

        // Vaciar datos: 
        // El input de la propiedad de la imagen
        $(`#${idPanel}`).val('');
        // La imagen a visualizar
        imgView.attr('src', ''); 
        // Input que contiene los datos de la imagen
        imgFile.val('');   
        
        // Ocultar el panel de la imagen
        panImg.addClass("d-none");
    },

    /**
     * Método para añadir un nuevo componente a una lista de componentes (Contenedor de Componentes vía Listas Ids)
     * @param {jqueryElement} button 
     */
    handleAddNewComponentToComponentsContainer: function(button){
        console.log("Añadir nuevo componente al contenedor de componentes");
    },

    /**
     * Método para añadir un nuevo componente a una lista de componentes (Contenedor de Componentes vía Listas Ids).
     * Comprobará también si el componente existe o ya está repetido en la lista de componentes para añadirlo o mostrar un mensaje de error.
     * @param {jqueryElement} input Input donde se ha escrito el componente a buscar/añadir
     */
    handleSearchComponentItemAndCheckIfNotExist: function(input){
        const that = this;
            
        if (input != undefined && input.val().trim() !=""){
            loadingMostrar();
            // Listado de componentes id a revisar
            let listaIDs = {};
            const newComponentIdToAdd = input.val().trim();
            // Construyo la petición para averiguar información del id componente escrito
            listaIDs['listaIDs[' + 0 + ']'] = newComponentIdToAdd;

            // Construir la url a partir del botón de editar pulsado para abrir el modal +"check". Dependerá de si está siendo editado o creado de nuevo.
            let urlCheckComponentName = '';     

            if (stringToBoolean(that.pParams.isEdited) == true){
                // El componente está siendo editado
                urlCheckComponentName = `${that.urlEditComponent.replace("load-modal","").trim()}check`;                                        
            }else{
                // El componente es nuevo
                urlCheckComponentName = `${that.urlSaveComponent}`.replace("save","check");
            }

            // Realizar la petición
            GnossPeticionAjax(
                urlCheckComponentName,
                listaIDs,
                true
            ).done(function (data) {
                loadingOcultar();

                // Comprobación del componente a añadir
                if (data[0].Error == ""){
                    // Comprobar que no existe el componente ya añadido                    
                    const existComponent = $(`*[data-componentid='${newComponentIdToAdd}']`);
                    if (existComponent.length == 0){
                        // OK - Añadir el componente ya que no existe
                        that.addNewComponentToListComponentContainer(data[0], input);
                    }else{
                        // Mostrar mensaje de error de no añadir mensaje
                        mostrarNotificacion("error", "No se pueden añadir componentes repetidos");
                    }
                }else{
                    // Se ha producido un error
                    mostrarNotificacion("error", data[0].Error);
                }                             
            });
        }        
    },

    /**
     * Método para añadir un nuevo componente al contenedor de componentes.
     * Se añadirá a la lista de componentes y se añadirá el valor en el inputHack correspondiente
     * @param {dictionary} componentData 
     * @param {jqueryElement} Input que se ha utilizado para realizar la búsqueda
     */
    addNewComponentToListComponentContainer: function(componentData, input){
        const that = this;
        // Lista donde estarían todos los componentes        
        const componentList = $(`.${that.componentListClassName}`);        
                
        // Obtener el id del input hack
        const inputHackPropertyId = $('.contenedorListaIds').data("idpanel");
        // InputHack donde se añadirá el nuevo componente
        const inputHackProperty = $(`#${inputHackPropertyId}`);        

        // Plantilla de la fila del componente a añadir
        const templateComponentRow = `
        <li class="component-wrap component-row" data-componentid="${componentData.Identificador}" draggable="false">
            <div class="component">
                <div class="component-header-wrap">			
                    <div class="component-header">            
                        <div class="component-sortable js-component-sortable-component">
                            <span class="material-icons-outlined sortable-icon">drag_handle</span>
                        </div>            
                        <div class="component-header-content">
                            <div class="component-header-left">                                    
                                <div class="component-name-wrap">                            
                                    <span class="component-name">							
                                        <span class="component-real-name">
                                            ${componentData.TextoEnlace}
                                        </span>							                                        
                                        <span class="component-name-id">
                                            ${componentData.Identificador}
                                        </span>
                                    </span>
                                </div>
                            </div>                            
                            <div class="component-header-right">
                                <div class="component-actions-wrap">
                                    <ul class="no-list-style component-actions">                                
                                        <li>
                                            <a
                                                class="action-delete round-icon-button js-action-delete btnDeleteComponentFromContainerList"
                                                href="javascript: void(0);"
                                                data-componentid="${componentData.Identificador}"
                                                draggable="false">
                                                <span class="material-icons">delete</span>
                                            </a>
                                        </li>
                                    </ul>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </li>        
        `;

        // Añadir el nuevo item a la lista de componentes
        componentList.append(templateComponentRow);
        // Añadir el id del nuevo componente al inputHack
        inputHackProperty.val(`${inputHackProperty.val()}${componentData.Identificador},`);
        // Vaciar el input de la búsqueda realizada
        input.val('');
    },

    
    /**
     * Método para comprobar y cargar los nombres de los componentes en las filas correspondientes 
     * a partir de los ids de los componentes listados. Se harán peticiones a backEnd para comprobar los nombres de los componentes por componentId
     * @param {jqueryElement} componentList : Lista donde se mostrarán los componentes asociados al contenedor de componentes
     */
    handleLoadAndCheckComponentName: function(componentList){
        const that = this;
        // Listado de componentes id a revisar
        let listaIDs = {};

        // Listado de componentes existentes en el contenedor
        const componentItems = componentList.find(".component-row");

        // Comprobar que hay componentes para realizar las peticiones
        if (componentItems.length > 0) {

            // Iniciar la operativa de Drag & Drop para cada componente
            ComponentesPageManagement.init();

            let i = 0;
            componentItems.each(function () {
                // Crear el objeto para realizar la petición de comprobación de nombres                
                listaIDs['listaIDs[' + i + ']'] = $(this).data("componentid");
                i++;
            });

            // Construir la url a partir del botón de editar pulsado para abrir el modal +"check". Eliminar posibles "load-modal" si viene de Builder-CMS            
            const urlCheckComponentName = `${that.urlEditComponent.replace("load-modal","").trim()}check`;

            // Realizar la petición
            GnossPeticionAjax(
                urlCheckComponentName,
                listaIDs,
                true
            ).done(function (data) {
                let i = 0;
                componentItems.each(function () {
                    const $componentItem = $(this);                                           
                    const resultado = data[i];
                    that.showComponentResult(resultado, $componentItem);
                    i++;                                        
                });
            });
        }
    },

    /**
     * Método para mostrar la información del componente después de haber realizado la comprobación.
     * La comprobación consistirá en obtener el nombre del componente según el id del mismo. 
     * Pintará el nombre del componente en la fila correspondiente
     * @param {*} resultado : Resultado de la petición realizada a backend. Es un array de diccionarios con la información de los componentes asociados
     * @param {*} $componentItem : Cada una de las filas correspondientes a los componentes cargados asociados dentro del contenedor
     */
    showComponentResult: function(resultado, $componentItem){
        const that = this;
        // Lugar donde se mostrará el nombre del componente y se eliminará el spinner de carga inicial.
        const componentName = $componentItem.find($(".component-real-name"));        
        const componentUrl = resultado.UrlEnlace;
        const componentRealName = resultado.TextoEnlace;
        // Quitar el spinner y mostrar el nombre correspondiente a cada componente        
        $componentItem.find(componentName).html(componentRealName);

        // Cargar el enlace en el icono del recurso (Para recursos de tipo Listados estáticos o dinámicos)
        const btnViewResourceFromContainerList = $componentItem.find(`.${that.btnViewResourceFromContainerListClassName}`);
        // Añadir el enlace si existe el botón
        if (btnViewResourceFromContainerList.length > 0){
            // Habilitar el botón y asignar la url del recurso
            btnViewResourceFromContainerList.prop("disabled", false);
            btnViewResourceFromContainerList.attr("href", componentUrl);
        }         
    },
      
    
    /**
     * Lanzar comportamientos u operativas necesarias para el funcionamiento de la sección
     */
    triggerEvents: function(){
        const that = this;   
    }, 
    
    // Configurar elementos relativos a la operativa de Editar Componente Item cuando se lance el modal
    operativaModalEditComponenteItem: function(pParams){
        const that = operativaGestionComponentsCMS;

        // Establecemos los pParams para la operativa principal
        that.pParams = pParams;

        /* Eventos a disparar para funcionamiento del modal */
        // Inicializar comportamientos de select2
        comportamientoInicial.iniciarSelects2();
        // Recargar ckEditors si hubiera para la edición de componentes CMS
        ckEditorRecargarTodos();  
    },   
    
    /* Guardado de Componentes */
    /************************* */
    /**
     * Método para guardar un determinado componente CMS una vez se pulse en el botón "Guardar" del modal
     */
    handleSaveComponent: function(){
        const that = this;

        // Privacidad del componente                        
        const isComponentPrivate = $(`#chkEditarPrivacidad_SI`).is(':checked');
        
        // Comprobar si se ha seleccionado un componente como privado, que al menos haya algún usuario seleccionado
        if (that.isNecessaryAssignUsersToPrivateComponent(isComponentPrivate) == true){
            mostrarNotificacion("error", "El componente es privado pero no se han seleccionado perfiles. Selecciona al menos un perfil.");
            return;
        }

        // Mostrar loading
        loadingMostrar();
        // Objeto de datos del componentes para ser mandados cuando se proceda a su guardado
        let datosComponente = {};    
        // Prefijo para el guardado del objeto CMS para guardado en BD
        let prefijoClave = 'Componente';

        datosComponente[prefijoClave + '.Private'] = isComponentPrivate;        
        
        // Privacidad del componente (Obtener los grupos/perfiles para el componente)
        if (isComponentPrivate) {
            // Revisión de Perfiles y Grupos asignados al componente al ser Privado
            const privacidad = $('#txtHackInvitadosPagina').val().split(',');
            let contGrupos = 0;
            let contPerfiles = 0;            
            for (let i = 0; i < privacidad.length; i++) {
                let idPrivacidad = privacidad[i].trim();
                if (idPrivacidad != "") {
                    if (idPrivacidad.substr(0, 2) == 'g_')
                    {
                        const prefijoPrivacidadGrupos = prefijoClave + '.GruposPrivacidad[' + contGrupos + ']';                        
                        datosComponente[prefijoPrivacidadGrupos + '.Key'] = privacidad[i].substr(2);
                        datosComponente[prefijoPrivacidadGrupos + '.Value'] = "";
                        contGrupos++;
                    }
                    else
                    {
                        const prefijoPrivacidadPerfiles = prefijoClave + '.PerfilesPrivacidad[' + contPerfiles + ']';
                        datosComponente[prefijoPrivacidadPerfiles + '.Key'] = privacidad[i];
                        datosComponente[prefijoPrivacidadPerfiles + '.Value'] = "";
                        contPerfiles++;
                    }                    
                }
            }
        }
        
        // Disponibilidad del componente en los idiomas (PENDIENTE - CUANDO SE HAN DE VISUALIZAR LOS IDIOMAS DISPONIBLES )
        if ($('#chkSelectIdioma').is(':checked') && $('input.idiomaMulti').length > 0)
        {
            var prefijoIdiomasDisponibles = prefijoClave + '.ListaIdiomasDisponibles';
            var todosIdiomas = $('input.idiomaMulti').is(':checked');
            if (!todosIdiomas) {
                var items = $('input.idioma');
                var i = 0;
                items.each(function () {
                    var item = $(this);
                    if (item.is(':checked')) {
                        datosComponente[prefijoIdiomasDisponibles + '[' + i + ']'] = item.prop('lang');
                        i++;
                    }
                });
            }
        }
                
        // Nombre del componente
        datosComponente[prefijoClave + '.Name'] = $('#nombrecomponente').val();
       
        // Nombre corto del componente
        const nombreCortoComponente = $(`#${that.inputNombreCortoComponenteId}`);
        if (that.isInputIsEmpty(nombreCortoComponente, true)){
            loadingOcultar();
            return;
        }        
        datosComponente[prefijoClave + '.ShortName'] = $(`#${that.inputNombreCortoComponenteId}`).val();
        
        // Estado del componente (Activo -> Sí / No)
        const isComponentActive = $(`#activocomponente_SI`).is(':checked');
        datosComponente[prefijoClave + '.Active'] = isComponentActive;
        
        // Componente de acceso público
        if ($('#accesopublicocomponente_SI').length > 0) {
            const isPublicAccess = $(`#accesopublicocomponente_SI`).is(':checked');
            datosComponente[prefijoClave + '.AccesoPublicoComponente'] = isPublicAccess;
        }

        // Personalización del componente
        if ($('#personalizacioncomponente').length > 0) {
            datosComponente[prefijoClave + '.PersonalizacionSeleccionada'] = $('#personalizacioncomponente').val();
        }

        // Caducidad del componente -> Si no, 6 será por defecto
        if ($('#caducidadComponente').length > 0) {
            datosComponente[prefijoClave + '.CaducidadSeleccionada'] = $('#caducidadComponente').val();
        }else{
            datosComponente[prefijoClave + '.CaducidadSeleccionada'] = '6';
        }

        // Estilos del componentes
        datosComponente[prefijoClave + '.Styles'] = $('#estiloscomponente').val();

        datosComponente[prefijoClave + '.FechaModificacion'] = $('#fechaModificacion').val();
        // Propiedades correspondientes al Componente CMS editado
        const properties = $('#hackProperties').val().split('|||');

        // Comprobación del valor de las propiedades para su guardado
        $.each(properties, function (key, value) {
            if (value != "") {
                // Prefijo de propiedades para el guardado en BD
                const prefijoPropiedades = prefijoClave + '.Properties[' + key + ']';
                if (value == "propiedad25") {                    
                    // Asignar los valores introducidos al inputHack de la propiedad correspondiente ( Componente: 'Envío email')
                    that.Propiedad25_RecalcularValor()
                }                
                else if (value == "propiedad30") {
                    that.Propiedad30_RecalcularListaCampos();
                }                
                else if (value == "propiedad6") {
                    that.Propiedad6_RecalcularListaID();
                }                
                else{
                    let valorMultiIdioma = "";
                    let esMultiIdioma = false;
                    $.each(that.listaIdiomas, function (keyidioma, valueidioma) {
                        if ($('#' + value + '_' + valueidioma).length > 0 || $(`*[data-propertyid="${value}_${valueidioma}"]`).length > 0 ) {
                            esMultiIdioma = true;
                            let valorCampo = '';
                            if ($(`*[data-propertyid="${value}_${valueidioma}"]`).length == 0){
                                valorCampo = $('#' + value + '_' + valueidioma).val();    
                                if (valorCampo != '') {
                                    valorMultiIdioma += $('#' + value + '_' + valueidioma).val() + '@' + valueidioma + '|||';
                                }                                
                            }else{
                                // Coger los textArea a partir de data-propertyid ya que no me permite usar id (ckeEditor)
                                valorCampo = $(`*[data-propertyid="${value}_${valueidioma}"]`).val();    
                                if (valorCampo != '') {
                                    valorMultiIdioma += $(`*[data-propertyid="${value}_${valueidioma}"]`).val() + '@' + valueidioma + '|||';
                                }                                
                            }
                        }
                    });
                    if (esMultiIdioma) {
                        $('#' + value).val(valorMultiIdioma);
                    }
                }

                // Asignación de propiedades del componente
                let valorPropiedad = "";

                if ($('#' + value).length > 0 || $(`*[data-propertyid="${value}"]`).length > 0 ) {                    
                    const propiedad = $(`#${value}`).length > 0 ? $(`#${value}`) : $(`*[data-propertyid="${value}"]`);
                    // Obtener el valor de la propiedad
                    let valorCampo = propiedad.val();
                
                    // Tener en cuenta si el tipo de propiedad es radioButton para obtener true/false
                    if (propiedad.is(':radio')) {
                        valorCampo = propiedad.is(':checked');
                    }
                    valorPropiedad = valorCampo;
                }
                // Asignación de valores para guardado en backEnd
                datosComponente[prefijoPropiedades + '.TipoPropiedadCMS'] = value.replace("propiedad", "");
                datosComponente[prefijoPropiedades + '.Value'] = encodeURIComponent(valorPropiedad);
            }
        });


        // Realización de la petición del guardado
        GnossPeticionAjax(
            that.urlSaveComponent,
            datosComponente,
            true
        ).done(function (data) {                    
            if (that.completion != undefined){
                // Se está realizando el guardado desde otra sección (Ej: Page Builder).
                // Data: Id del componente Nuevo creado (Necesario para Page Builder)
                // Construyo un objeto con los datos del componente creado (Nombre, Tipo, Id)
                const componentObject = {
                    name: `${datosComponente['Componente.Name']}`,
                    type: "Nuevo componente CMS",
                    id: data,
                };

                if ($(`.${that.backModalNavigationClassName}`).length == 0){
                    that.completion(requestFinishResult.ok, componentObject);
                }else{
                    // Navegación al ancestro del Modal del Grupo de componentes
                    mostrarNotificacion("success", "El componente se ha guardado correctamente");
                    that.setRemoveModalViewFromModalContainer();
                }    
            }else{
                // OK -> Guardado correcto
                mostrarNotificacion("success", "El componente se ha guardado correctamente");                            
                // Cerrar el modal y recargar la página para recargar todos los componentes CMS - Tener en cuenta posible navegación del ancestro por Grupo de Componentes                            
                if ($(`.${that.backModalNavigationClassName}`).length == 0){
                    setTimeout(function() {
                        dismissVistaModal();            
                        location.reload();
                    }
                    ,1000);
                }else{
                    // Navegación al ancestro del Modal del Grupo de componentes
                    that.setRemoveModalViewFromModalContainer();
                }                                        
            }
        }).fail(function (data) {
            // KO al guardar el componente             
            if (that.completion != undefined){
                // Se está realizando el guardado desde otra sección (Ej: Page Builder)
                that.completion(requestFinishResult.ko, data);
            }else{
                mostrarNotificacion("error", data);
            }               
        }).always(function () {
            // Ocultar loading
            loadingOcultar();            
        });        
    },

    /**
     * Método para comprobar si el componente a guardar es privado. En caso de ser privado, comprobar que al menos hay un perfil seleccionado.
     * @param {*} isComponentPrivate 
     * @returns Devuelve un valor booleano indicando de si es necesario asignar algún perfil al componente debido a que este es privado.
     */
    isNecessaryAssignUsersToPrivateComponent: function(isComponentPrivate){

        if (isComponentPrivate == true){
            // Comprobar si hay o no perfiles asignados
            const perfilesComponente = $('#txtHackInvitadosPagina').val().split(',');
            if (perfilesComponente.length == 1 && perfilesComponente == ""){
                return true;
            }
        }
        return false;
    },

    /**
     * Método para recalcular el valor del inputHack una vez se proceda al guardado para el componente CMS de tipo 'Envío correo'
     */
    Propiedad25_RecalcularValor: function() {
        const that = this;
        
        const elementos = $('.elementos_propiedad25_' + that.listaIdiomas[0]);
        let nuevosElementos = ''
        // Vaciar el valor actual para construir los nuevos con las nuevas propiedades editadas
        $('#propiedad25').val('');
        elementos.each(function (indice) {
            const elemento = $(this);
            let valorTxt = '';

            // Nombre
            $.each(that.listaIdiomas, function (keyidioma, valueidioma) {
                const valorCampo = $('#propiedad25_' + valueidioma + '_' + indice + '_txt').val();
                if (valorCampo != '') {
                    valorTxt += $('#propiedad25_' + valueidioma + '_' + indice + '_txt').val() + '@' + valueidioma + '|||';
                }
            });        
                       
            // Input obligatorio (Activo -> Sí / No)            
            const checkName = $(elemento.find('.esCampoObligatorio input')[0]).prop('name');
            const isInputRequired = $(`input[name="${checkName}_SI"]`).is('checked');
            const valorChk = isInputRequired;
            // Tipo de Input (Corto/Largo)                    
            const valorSelect = elemento.find(`select[name="tipoCampo"]`).val(); 

            // Asignar los nuevos datos/editados al inputHack
            $('#propiedad25').val($('#propiedad25').val() + valorTxt + '&&&' + valorChk + '&&&' + valorSelect + '###');
        });
    },


    /**
     * Método para recalcular el valor del inputHack una vez se proceda al guardado para el componente CMS de tipo 'Menú'
     */
     Propiedad30_RecalcularListaCampos: function() {
        const that = this;

        const elementos = $('.elementos_propiedad30_' + that.listaIdiomas[0]);
        let nuevosElementos = '';
        let idiomaPorDefecto = $("#idiomaDefecto").val(); 
        // Reseteamos el campo para guardar nuevos
        $('#propiedad30').val('');
        let nuevaPosicion = 0;
        elementos.each(function(indice){
            const elemento = $(this);
            let valorTxtNombre='';
            let valorTxtEnlace='';
            let valorTxtOriginalPosition = '';

            // Nuevo nivel e índice de la propiedad del menú
            const newNivel = elemento.first().data("nivel");          
            const newIndice = elemento.first().data("indice");          

            for (let i=0; i< that.listaIdiomas.length; i++) {
                const valueidioma = that.listaIdiomas[i];
        
                if (valueidioma != idiomaPorDefecto && elemento.find($('#propiedad30_' + valueidioma + '_' + indice + '_txt_nombre')).val() == undefined){
                    // Si no hay valores para los demás idiomas, generarlo a partir del idioma por defecto
                    //valorTxtNombre += $('#propiedad30_' + idiomaPorDefecto + '_' + indice + '_txt_nombre').val() + '@' + valueidioma + '|||';
                    valorTxtNombre += elemento.find(".inputNombreMenuItem").val() + '@' + valueidioma + '|||'; //elemento.find($('#propiedad30_' + valueidioma + '_' + indice + '_txt_nombre')).val() + '@' + valueidioma + '|||';
                    //valorTxtEnlace += $('#propiedad30_' + idiomaPorDefecto + '_' + indice + '_txt_enlace').val() + '@' + valueidioma + '|||';
                    valorTxtEnlace += elemento.find(".inputEnlaceMenuItem").val() + '@' + valueidioma + '|||'; //elemento.find($('#propiedad30_' + valueidioma + '_' + indice + '_txt_enlace')).val() + '@' + valueidioma + '|||';                     
                    //elemento.find($('#propiedad30_' + valueidioma + '_' + indice + '_txt_nombre')).val()
                }else{
                    //valorTxtNombre += $('#propiedad30_' + valueidioma + '_' + indice + '_txt_nombre').val() + '@' + valueidioma + '|||';
                    valorTxtNombre += elemento.find(".inputNombreMenuItem").val() + '@' + valueidioma + '|||'; // elemento.find($('#propiedad30_' + valueidioma + '_' + indice + '_txt_nombre')).val() + '@' + valueidioma + '|||';                
                    //valorTxtEnlace += $('#propiedad30_' + valueidioma + '_' + indice + '_txt_enlace').val() + '@' + valueidioma + '|||';
                    valorTxtEnlace += elemento.find(".inputEnlaceMenuItem").val() + '@' + valueidioma + '|||'; // elemento.find($('#propiedad30_' + valueidioma + '_' + indice + '_txt_enlace')).val() + '@' + valueidioma + '|||';
                }                
            }

            /*
            $.each(that.listaIdiomas, function (keyidioma, valueidioma) {
                if (valueidioma != idiomaPorDefecto && $('#propiedad30_' + valueidioma + '_' + indice + '_txt_nombre').val() == undefined){
                    // Si no hay valores para los demás idiomas, generarlo a partir del idioma por defecto
                    valorTxtNombre += $('#propiedad30_' + idiomaPorDefecto + '_' + indice + '_txt_nombre').val() + '@' + valueidioma + '|||';
                    valorTxtEnlace += $('#propiedad30_' + idiomaPorDefecto + '_' + indice + '_txt_enlace').val() + '@' + valueidioma + '|||';                     
                }else{
                    valorTxtNombre += $('#propiedad30_' + valueidioma + '_' + indice + '_txt_nombre').val() + '@' + valueidioma + '|||';
                    valorTxtEnlace += $('#propiedad30_' + valueidioma + '_' + indice + '_txt_enlace').val() + '@' + valueidioma + '|||';
                }
            });
            */
            // Nivel del enlace en el menú
            let nivel = parseInt(elemento.data("nivel"));

            valorTxtOriginalPosition = elemento.data("originalposition") != undefined ? parseInt(elemento.data("originalposition")) : nuevaPosicion; // parseInt(elemento.data("originalposition"));
            // Establecer los datos de los items del menú
            $('#propiedad30').val($('#propiedad30').val() + nivel + '&&&' + valorTxtNombre + '&&&' + valorTxtEnlace + '&&&' + valorTxtOriginalPosition + '&&&' + '###');
            // Cambiar la posición del menú a la nueva posición por si ha cambiado algo
            elemento.data("originalposition",nuevaPosicion);
            nuevaPosicion++
        });
    },


    /**
     * Método para recalcular el valor del inputHack una vez se proceda al guardado para el componente CMS de tipo Componente'   '
     */
    Propiedad6_RecalcularListaID: function(){
        const that = this;
        // Obtener los componentes del listado de componentes del contenedor
        const elementos = $(".component-list").find(".component-row");
        // Nuevo listado de componentes a construir
        let nuevosElementos = '';
        // Vaciar los posibles componentes actuales para añadir nuevos
        $('#propiedad6').val('');
        elementos.each(function(){
            const elemento = $(this);
            // Obtener el id de cada componente
            const valor = elemento.find('.component-name-id').html().trim();
            // Añadir los componentes nuevos
            if($('#propiedad6').val()==''){$('#propiedad6').val(valor);}
            else{$('#propiedad6').val($('#propiedad6').val()+','+valor);}
        });
    },
    

    /* Gestión de Acciones de componentes */
    /**************************************** */

    /**
     * Método para cargar la vista para editar un componente desde un componente de tipo Grupo de contenedores
     * @param {*} urlLoadModalComponent 
     * @param {*} button Botón usado para poder cargar la vista para la edición del componente
     */
    handleLoadComponentFromComponentContainer: function(urlLoadModalComponent, button){
        const that = this;
        // Mostrar loading
        loadingMostrar();

        // Hacer petición y cargar el contenido del modal dentro de "modal-dinamic-content"
        getVistaFromUrl(urlLoadModalComponent, 'modal-dinamic-content', '', function(result, data){
            if (result != requestFinishResult.ok){
                loadingOcultar();
                // KO al cargar la vista
                mostrarNotificacion("error", "Error al cargar el componente para su creación.");                                                                    
            }else{
                // Guardar el contenido del modal solicitado que contiene la edición del "Grupo de Componentes"
                that.setRemoveModalViewFromModalContainer(data, button);
                // Añadir el estilo de tabs al modal 
                !that.modalContainer.hasClass("modal-con-tabs") && that.modalContainer.addClass("modal-con-tabs");  
                // Ocultar modal
                loadingOcultar();                  
            }               
        },false, false);        
    },

    /**
     * Método para controlar la navegación jerarquica de los modales para un grupo de componentes.
     * Si se añade, se guardará la vista del contenedor actual en el array para poder realizar la navegación jerarquica
     * Si se decido no añadir, se querrá eliminar la vista actual, y mostrar la vista última del array de vistas para la navegación.
     * @param {*} pViewData : Vista devuelta por la petición para que sea mostrada en el modal. Si es vacía, se quiere eliminar la vista actual del modal-container. Si no se pasa una vista (undefined)
     * se entiende que se desea eliminar la vista actual del contenedor y añadir la previa del array
     * @param {*} button : Botón que ha disparado la carga de la vista modal para poder editar el hijo
     */
    setRemoveModalViewFromModalContainer: function(pViewData = undefined, button = undefined){
        const that = this;
        // Contenedor modal padre
        // that.modalContainer y vista donde se ha de añadir pViewData: modal-dinamic-content

        // Botón para navegación 
        const btnBackModal = $(`.${that.backModalNavigationClassName}`).length == 0 ? `<span style="cursor:pointer" class="material-icons ${that.backModalNavigationClassName}" alt="Volver" title="Volver" aria-label="Close">arrow_back</span>` : $(".backNavigation").length;
        // Botón para cerrar el modal cuando se cargue
        let btnCloseModal = undefined;
        // Obtener el área donde se posicionará la vista
        const modalDinamicContent = that.modalContainer.find("#modal-dinamic-content");
        // Obtener el modal que se está mostrando actualmente (Padre del Componente CMS que está siendo editado)
        const currentModalView = that.modalContainer.find("#modal-dinamic-content")[0].outerHTML;

        if (pViewData){
            // Guardar la URL correspondiente al Componente a mostrar. Tener en cuenta primero la del "Padre" o Contenedor de Componentes
            const currentModalViewUrlSave = that.listComponentArray.length > 0 ? button.data("urlsave") : that.urlSaveComponent;
            const currentModalViewData = {
                currentModalView,
                currentModalViewUrlSave,
            }
            // Establecer la URL del componente actual a guardar ya que ha sido guardada correctamente en el Array
            that.urlSaveComponent = button.data("urlsave");
            // Añadir la nueva vista modal al modalContainer y al array            
            that.listComponentArray.push(currentModalViewData);
            // Añadir la vista actual al modal-dinamic-content para la visualización del hijo
            modalDinamicContent.html(pViewData);
            // Localizar el botón de cerrar el modal
            btnCloseModal = that.modalContainer.find(".cerrar");
            // Ocultar el botón de Cerrar para que no esté disponible
            btnCloseModal.addClass("d-none");
            // Añadir el botón de la navegación
            that.modalContainer.find(".modal-header").append(btnBackModal);
        }else{
            // Obtener la vista padre del componente del array para realizar la navegación al "padre"
            const parentModalView = that.listComponentArray.at(-1);
            // Quitar la vista modal actual del modalContainer y del array 
            that.listComponentArray.pop();
            // Añadir la vista actual al modal-dinamic-content para la visualización del padre
            modalDinamicContent.html(parentModalView.currentModalView);
            btnCloseModal = that.modalContainer.find(".cerrar");
            // Ocultar el botón de Cerrar para que no esté disponible sólo si no hay más hijos pendientes en la jerarquía para la navegación           
            that.listComponentArray.length > 0 && btnCloseModal.addClass("d-none");
            // Añadir el botón de la navegación si hay más hijos pendientes
            that.listComponentArray.length > 0 && that.modalContainer.find(".modal-header").append(btnBackModal);
            // Añadir la URL del guardado del componente que se está mostrando en ese momento
            that.urlSaveComponent = parentModalView.currentModalViewUrlSave;
            // Evitar comportamientos extraños con los select2 al volver hacia atrás (Eliminarlos e inicializar los select2)
            $(".select2-container").remove();
            comportamientoInicial.iniciarSelects2();
        }
    },

    /**
     * Método para borrar un componente CMS de un componente de tipo "Grupo de componentes". Este método se ejecuta cuando se está editando un 
     * componente de tipo "Grupo de componentes" y se procede a eliminar un componente de su "contenedor".
     * Se procederá a eliminar la row del item a borrar y de quitar el id del input hack que contiene los componentes asociados
     * @param {jqueryElement} deleteButton : Botón de eliminación que se ha pulsado para eliminar un componente del contenedor de componentes
     */
    handleDeleteComponentFromComponentContainer: function(deleteButton){
        const that = this;
        
        // Row a eliminar del contenedor de componentes        
        const componentRow = deleteButton.closest('.component-row');
        // Id del componente a borrar        
        const idItemDeleted = deleteButton.data("componentid");
        // Obtener el id del input hack
        const inputHackPropertyId = deleteButton.closest('.contenedorListaIds').data("idpanel");
        const inputHackProperty = $(`#${inputHackPropertyId}`);

        // Quitar el id del inputHack
        // Listado de componentes en input hack
        let componentsListId = "";

        componentsListId = inputHackProperty.val().split(",");
        componentsListId.splice( $.inArray(idItemDeleted, componentsListId), 1 );
        // Pasarle los datos de los grupos actuales al input hidden
        inputHackProperty.val(componentsListId.join(",")); 
        // Eliminar la fila de componentes
        componentRow.remove();
    },


    /**
     * Método para copiar al portapapeles el enlace del item pulsado.     
     * @param {String} urlString : Url o cadena que se mostrará al usuario y que se copiará al portapapeles
     */
    handleCopyToClickBoard(urlString){           
        copyTextToClipBoard(urlString);
    },


    /* Métodos para visualizar datos haciendo uso del paginador / Buscador */
    /********************************************************** */
    /**
     * Método para aplicar filtro/petición para listar items (Ej: Click en página de paginador de resultados)
     * @param {*} filtro 
     */
    AgregarFiltroComponentes: function (filtro) {
        
        // Mostrar loading
        loadingMostrar();
        // Parámetro del filtro a usar en carga de items
        const parametroNuevo = filtro.substr(0, filtro.indexOf("="));
        // Valor del filtro 
        const valorNuevo = filtro.substr(filtro.indexOf("=") + 1);
        // Filtros actuales 
        let filtrosActuales = "";
        // Cargar los filtros actuales que hay a través de la URL
        if (document.location.href.indexOf('?') > -1) {
            filtrosActuales = document.location.href.substr(document.location.href.indexOf('?') + 1);
        }

        // Comprobación de filtros para eliminar o añadir a url para su petición
        if (filtrosActuales != "") {
            var filtrosNuevos = "";
            var filtrosArray = filtrosActuales.split('&');
            var and = '?';
            var contieneFiltro = false;

            for (var i = 0; i < filtrosArray.length; i++) {
                var parametroActual = filtrosArray[i].substr(0, filtrosArray[i].indexOf('='));
                var valorActual = filtrosArray[i].substr(filtrosArray[i].indexOf('=') + 1);

                if (parametroActual == parametroNuevo) {
                    contieneFiltro = true;
                    if (valorActual != valorNuevo) {
                        filtrosNuevos += and + filtro;
                        and = '&';
                    }
                }
                else if (parametroActual != 'pagina') {
                    filtrosNuevos += and + filtrosArray[i];
                    and = '&';
                }
            }

            if (!contieneFiltro) {
                filtrosNuevos += and + filtro;
            }

            filtrosActuales = filtrosNuevos;
        }
        else {
            //No tenía filtros, lo añadimos
            filtrosActuales = '?' + filtro;
        }

        nuevaPagina = "";
        if (document.location.href.indexOf('?') == -1) {
            nuevaPagina = document.location.href + filtrosActuales;
        } else {
            indice = document.location.href.indexOf('?');
            nuevaPagina = document.location.href.substr(0, indice) + filtrosActuales;
        }        

        // Carga de la página con el nuevo filtro
        document.location = nuevaPagina;
    }, 


    /**
     * Método para eliminar filtros activos de búsquedas de recursos multimedia en la sección de filtros activos
     * @param {} filtro 
     */
    LimpiarFiltroComponentes: function(filtro){
        // Mostrar loading
        loadingMostrar();
        // Página a cargar
        let nuevaPagina = "";
        indice = document.location.href.indexOf("?");
        nuevaPagina = document.location.href.substr(0, indice + 1);
        // Recarga de la página la página
        document.location = nuevaPagina;        
    },
    

    /**
     * Método para realizar búsquedas cuando se escriba en el input del buscador de items multimedia
     */
    handleSearchComponentItem: function(){    
        const that = this;
        let searchString = this.txtBuscarComponentItem.val();
        // Eliminamos posibles tildes para búsqueda ampliada
        searchString = searchString.normalize('NFD').replace(/[\u0300-\u036f]/g, "").toLowerCase();
        if (searchString != ""){
            that.AgregarFiltroComponentes(`search=${searchString}`);
        }    
    },

    /**
     * Método para comprobar si un determinado input está vacío para mostrar un posible error
     * @param {*} input 
     * @param {*} showError 
     * @returns 
     */
    isInputIsEmpty: function(input, showError = false){
        const that = this;        
        let isInputEmpty = false;
        isInputEmpty = input.val().trim().length == 0;

        if (showError && isInputEmpty){
            that.showErrorEmptyProperty(input);
        }
        return isInputEmpty;
    },

    /**
     * Método para mostrar el error debido a que el input no se ha insertado
     * @param {*} input: Input correspondiente donde se ha encontrado error por estar vacío.
     */
    showErrorEmptyProperty: function (input) {        
        const that = this;
        comprobarInputNoVacio(input, true, false, "Esta propiedad no puede estar vacía.", 0);                    
    },    
}

/**
 * Operativa gestión de Redirecciones de la comunidad
 */
const operativaGestionRedirecciones = {
    
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
     * Configuración de las rutas de acciones necesarias para hacer peticiones a backend
     */
     configRutas: function (pParams) {
        // Url base
        this.urlBase = refineURL();
        // Url para la creación de nuevas redirecciones
        this.urlAddNewRedirection = `${this.urlBase}/new-redirection`;
        // Url para el guardado de redirecciones
        this.urlSaveRedirections = `${this.urlBase}/save`;
        // Url para mostrar confirmación de eliminación de una redirección
        this.urlLoadConfirmDeleteRedirection = `${this.urlBase}/load-delete-redirection-item`;
    },    

    /*
     * Inicializar elementos de la vista
     * */
    config: function (pParams) {
        
        // Contenedor modal de contenido dinámico
        this.modalContainer = $("#modal-container");
        this.modalRedirectionClassName = "modal-redirection";

        // Botón para añadir una nueva redirección
        this.btnAddRedirectionItem = $("#btnAddRedirectionItem");   

        // Botón de editar la redirección
        this.btnEditRedirection = $(".btnEditRedirection");
        // Botón de eliminar una redirección
        this.btnDeleteRedirection = $(".btnDeleteRedirection");
        
        // Contenedor de todas las redirecciones de la comunidad
        this.redirectionListContainer = $('#id-added-redirections-list');

        // Fila que se seleccionará cuando se edite o se desee borrar
        this.filaRedireccion = undefined;
        this.cmbEditarTipoRedirection = $(".cmbEditarTipoRedirection");
        // Panel de redirección directa
        this.panelDirectRedirectionNameClass = "redireccionDirecta";
        // Panel de redirección parametrizada
        this.panelParametrisedRedirectionNameClass = "parametrizada";
        // Input para cambiar la URL origen de la redirección
        this.inputUrlOrigen = $(".inputUrlOrigen");
        // Input para cambiar el destino de la url
        this.inputUrlDestino = $(".inputUrlDestino");
        // Label de cada fila donde se muestra la Url de origen de la redirección
        this.componentUrlOrigen = $(".componentUrlOrigen");
        // Label de cada fila donde se muestra la Url de destino de la redirección
        this.componentUrlDestino = $(".component-url");
        // Botón para guardar la redirección desde el modal
        this.btnSaveRedirection = $(".btnSaveRedirection");
        
        // Botón para añadir nuevos parámetros a una redirección
        this.linkAddParam = $(".linkAddParam");
        // Label donde se muestra el valor del parámetro
        this.componentParameterNameClassName = "component-parameterName";
        // Input donde se escribirá el valor del parámetro
        this.inputParameterUrlValueClassName = "inputParameterUrlValue";
        // Nombre de la clase de los botones para eliminar un parámetro de una redirección de tipo "Parametrizada".
        this.btnDeleteParameterClassName = "btnDeleteParameter";


        /* Buscador de items multimedia */
        // Input de buscador de items multimedia
        this.txtBuscarRedireccion = $("#txtBuscarRedireccion");
        this.inputLupa = $("#inputLupa");

        // Datos a mandar para el guardado
        this.ListaRedirecciones = {};

        /* Flags para usar en Redirecciones */
        // Flag para confirmar la eliminación de una redirección
        this.confirmDeleteRedirection = false;
        this.isErrorBeforeSaving = false;        
        
    },  
    
    /**
     * Configuración de eventos de elementos del Dom (Botones, Inputs...)     
     */
     configEvents: function (pParams) {
        const that = this;
                       
        this.modalContainer.off().on('show.bs.modal', (e) => {
            // Aparición del modal       
            // Establecer quién ha disparado el modal para poder cambiar sus atributos una vez se guarden los grupos desde el modal
            that.triggerModalContainer = $(e.relatedTarget);
        })
        .on('hide.bs.modal', (e) => {
            // Acción de ocultar el modal
        })
        .on('hidden.bs.modal', (e) => {
            // Comprobar si al cerrar el modal, se desea eliminar la redirección (El modal de eliminación se carga aquí)
            if (that.confirmDeleteRedirection == false){                                
                that.handleSetDeleteRedirection(false);
            }
            // Vaciar el modal
            resetearModalContainer();
        });


        // Comportamientos del modal que son individuales para la edición de redirecciones
        $(`.${this.modalRedirectionClassName}`).off().on('show.bs.modal', (e) => {
            // Aparición del modal
            that.triggerModalContainer = $(e.relatedTarget);
        })
        .on('hide.bs.modal', (e) => {
            // Acción de ocultar el modal
            // El modal que se va a ocultar/cerrar
            const currentModal = $(e.currentTarget); 
            // Operativa al cerrar el modal                                           
            that.handleCloseRedirectionModal(currentModal);
        });

        // Botón para añadir una nueva redireccion
        this.btnAddRedirectionItem.off().on("click", function(){
            that.handleAddNewRedirection();
        });

        // Botón para editar una redirección                
        this.btnEditRedirection.off().on("click", function(){            
            // Establecer la clase de "modified" a la fila
            that.filaRedireccion = $(this).closest('.redirection-row');
            that.filaRedireccion.addClass("modified");
        });

        // Botón/es para eliminar una determinada redirección al pulsar en el botón de papelera
        this.btnDeleteRedirection.off().on("click", function(){            
            const btnDeleted = $(this);            
            // Fila correspondiente a la pagina eliminada
            that.filaRedireccion = btnDeleted.closest('.redirection-row');            
            // Marcar la redirección como "Eliminar" a modo informativo al usuario
            that.handleSetDeleteRedirection(true);
            // Cargar la vista de forma dinámica en el modal-container para confirmar el borrado de la redirección
            getVistaFromUrl(that.urlLoadConfirmDeleteRedirection, 'modal-dinamic-content', '');
        });        
        
        // Input para realizar búsquedas de redirecciones
        this.txtBuscarRedireccion.off().on("keyup", function(e){                                
            clearTimeout(that.timer);
            that.timer = setTimeout(function () {                
                that.handleSearchRedirection();                                                         
            }, 500);
        });

        // Click en la lupa para realizar búsquedas de items multimedia
        this.inputLupa.off().on("click", function(){            
            that.handleSearchRedirection();
        });

        // Select para el tipo de redirección de la redirección
        this.cmbEditarTipoRedirection.off().on("change", function(){
            const select = $(this);
            that.handleOptionEditRedirectionType(select);
            // Cambiar el tipo de redirección en la fila de la redirección
            that.handleSetOptionEditRedirectionType(select);
        });

        // Botón para añadir parámetros a una redirección parametrizada
        this.linkAddParam.off().on("click", function(){
            const button = $(this);
            // Fila de redirección que está siendo modificada/editada
            that.filaRedireccion = button.closest('.redirection-row');            
            that.handleAddParameter();
        });

        // Botón para eliminar un parámetro de una determinada redirección
        configEventByClassName(`${that.btnDeleteParameterClassName}`, function(element){
            const $jqueryElement = $(element);
            $jqueryElement.off().on("click", function(){ 
                // Fila del atributo que se desea eliminar
                const button = $(this);
                that.filaParametro = button.closest('.parameter-row');  
                // Eliminar la fila del parámetro
                that.filaParametro.remove();
            });	                        
        });        

        // KeyUp cuando se cambia el nombre de un atributo parametrizado                        
        configEventByClassName(`${that.inputParameterUrlValueClassName}`, function(element){
            const $jqueryElement = $(element);
            $jqueryElement.off().on("keyup", function(){ 
                // Fila de redirección que está siendo modificada/editada
                const input = $(this);
                that.filaRedireccion = input.closest('.redirection-row');  
                that.handleChangeParameterName(input);
            });	                        
        });

        // Botón para guardar redirección
        this.btnSaveRedirection.off().on("click", function(){
            that.handleSaveCurrentRedirection();
        });

        // Keyup cuando se cambie la URL de destino
        this.inputUrlDestino.off().on("keyup", function(){            
            const input = $(this);                        
            comprobarInputNoVacio(input, true, false, "La url de destino no puede estar vacía para un tipo de redirección directa.", 0);
            that.handleSetUrlRedirectionDestiny(input);
        }); 
        
        // Keyup cuando se cambie la URL de origen
        this.inputUrlOrigen.off().on("keyup", function(){
            const input = $(this);
            comprobarInputNoVacio(input, true, false, "La url de origen no puede estar vacía.", 0);
            that.handleSetUrlRedirectionOrigen(input);            
        });        
    }, 
    
    /**
     * Lanzar comportamientos u operativas necesarias para el funcionamiento de la sección
     */
    triggerEvents: function(){
        const that = this;  
        // Inicializar comportamientos de select2
        comportamientoInicial.iniciarSelects2();         
    }, 
    
    // Configurar elementos relativos a la operativa de Añadir Multiemdia Item cuando se lance el modal
    operativaModalDeleteRedirectionItem: function(){
        const that = operativaGestionRedirecciones;

        // Botón de confirmación de eliminación de la redirección
        this.btnConfirmDeleteRedirection = $(".btnConfirmDeleteRedirection");     
        // Botón de no confirmar la eliminación de la redirección
        this.btnRejectDeleteRedirection = $(".btnRejectDeleteRedirection"); 
        

        // Botón para confirmar la eliminación de la redirección
        this.btnConfirmDeleteRedirection.off().on("click", function(){
            that.confirmDeleteRedirection = true;
            that.handleDeleteRedirection();
        });

        // Botón para no confirmar la eliminación de la redirección
        this.btnRejectDeleteRedirection.off().on("click", function(){            
            that.confirmDeleteRedirection = false;            
        });
    },

    /**
     * Método para realizar búsquedas de redirecciones
     */
    handleSearchRedirection: function(){
        const that = this;
        
        let cadena = this.txtBuscarRedireccion.val();
        // Eliminamos posibles tildes para búsqueda ampliada
        cadena = cadena.normalize('NFD').replace(/[\u0300-\u036f]/g, "").toLowerCase();
        
        // Cada una de las filas que muestran las redirecciones
        const rowRedirections = $("#id-added-redirections-list").find(".redirection-row");        

        // Buscar dentro de cada fila       
        $.each(rowRedirections, function(index){
            const rowRedirection = $(this);
            // Seleccionamos el nombre de la redirección y quitamos caracteres extraños, tiles para poder hacer bien la búsqueda
            const redirectionName = $(this).find(".component-name").not('.d-none').html().trim().normalize("NFD").replace(/[\u0300-\u036f]/g, "").toLowerCase();
            if (redirectionName.includes(cadena)){
                // Mostrar fila resultado y sus respectivos padres
                rowRedirection.removeClass("d-none");
                rowRedirection.parents("li.component-wrap").removeClass("d-none");
            }else{
                // Ocultar fila resultado
                rowRedirection.addClass("d-none");
            }            
        });        
    },
    
    /**
     * Método que aplicará funcionalidad cuando se cierre el modal de edición de una redirección
     * @param {jqueryObject} currentModal : Modal que se va a cerrar
     */
    handleCloseRedirectionModal: function(currentModal){
        const that = this;

        // Hacer scroll top cuando se cierre el modal
        scrollInModalViewToTop(currentModal);
        // Colapsar todos los menús desplegables del modal para que no se queden abiertos
        const collapsePanels = currentModal.find('.collapse');
        collapsePanels.collapse("hide");
        // Quitar la opción de guardar cambios si no se pulsa en "Guardar". Si se pulsa en guardar se procederá al guardado
        that.filaRedireccion.removeClass("modified");

        // Tener en cuenta si la página es de reciente creación y por tanto no se desea guardar
        if (that.filaRedireccion.hasClass("newRedirection")){
            // Eliminar la fila que se acaba de eliminar                                
            that.filaRedireccion.remove();                              
        }
    },

    /**
     * 
     * @param {bool} deleteRedirection Marcar la redirección como borrada para informar al usuario acerca de la acción a realizar
     */
    handleSetDeleteRedirection: function(deleteRedirection){
        const that = this;

        if (deleteRedirection){
            // Realizar el "borrado" de la redirección
            // 1 - Marcar la opción "TabEliminada" a true
            that.filaRedireccion.find('[name="TabEliminada"]').val("true");           
            // 2 - Añadir la clase de "deleted" a la fila de la redireccion
            that.filaRedireccion.addClass("deleted");
        }else{
            // 1 - Marcar la opción "TabEliminada" a false
            that.filaRedireccion.find('[name="TabEliminada"]').val("false");            
            // 2 - Eliminar la clase de "deleted" a la fila de la página
            that.filaRedireccion.removeClass("deleted");
        }
    },
  
    
    /**
     * Método para cambiar el tipo de redirección (Directa/Parametrizada)
     * @param {JqueryElement} select Componente jquery Select/ComboBox que se ha activado
     */
    handleOptionEditRedirectionType: function(select){
        const that = this;
        // Fila correspondiente al parámetro a editar
        that.filaRedireccion = select.closest('.redirection-row');
        // Paneles del tipo de redirección
        const panelDirectRedirection = that.filaRedireccion.find(`.${that.panelDirectRedirectionNameClass}`);
        const panelParametrisedRedirection = that.filaRedireccion.find(`.${that.panelParametrisedRedirectionNameClass}`);

        if (select.val() == "Direct") {
            // Tipo de redirección: Directa
            // Mostrar el panel de redirección "Directa"
            panelDirectRedirection.removeClass("d-none");
            panelParametrisedRedirection.addClass("d-none");
        }else{
            // Tipo de redirección: Parametrizada
            panelParametrisedRedirection.removeClass("d-none");
            panelDirectRedirection.addClass("d-none");
        }
    }, 

    /**
     * Método para establecer el tipo de redirección (Directa/Parametrizada) en la fila de redirección editada
     * @param {JqueryElement} select Componente jquery Select/ComboBox que se ha activado
     */
    handleSetOptionEditRedirectionType: function(select){
        const that = this;
        // Fila correspondiente al parámetro a editar
        that.filaRedireccion = select.closest('.redirection-row');
        // Establecer la opción marcada
        const typeSelectedValue = select.children("option:selected").data("optionvalue");
        // Tipo de redirección indicada en la fila
        const redirectionTypeRow = that.filaRedireccion.find(".component-tipo");
        // Establecer la opción seleccionada en la fila
        redirectionTypeRow.html(typeSelectedValue.trim());

        // Input del destinoUrl para mostrar o no el valor en la fila si es tipo "Direct"

        const input = that.filaRedireccion.find(this.inputUrlDestino);
        that.handleSetUrlRedirectionDestiny(input);
    },    

    /**
     * Método para cambiar la visualización del parámetro cuando se esté escribiendo el valor del parámetro de la redirección
     * @param {jqueryElement} input Input donde se está escribiendo el nuevo valor del parámetro 
     */
    handleChangeParameterName: function(input){
        const that = this;
        const newParameterValue = input.val();                 
        // Fila del parámetro que está siendo modificado
        const filaParameter = input.closest('.parameter-row');
        // Nombre del parámetro a cambiar al hacer keyUp
        const parameterNameValue = filaParameter.find(".component-parameterName");
        // Asignar lo tecleado (Nuevo nombre del parámetro)
        parameterNameValue.html(newParameterValue.trim());
    },

    
     /**
     * Método para cambiar la URL de destino cuando se esté escribiendo el valor del parámetro de la redirección
     * @param {jqueryElement} input Input donde se está escribiendo el nuevo valor del parámetro 
     */
    handleSetUrlRedirectionDestiny: function(input){
        const that = this;
        // Fila correspondiente al parámetro a editar
        that.filaRedireccion = input.closest('.redirection-row');
        // Select del tipo de Redirección
        const selectUrlType = that.filaRedireccion.find(that.cmbEditarTipoRedirection);
        // Input de la url de destino
        const urlDestination = that.filaRedireccion.find(input);
        // Label donde se mostrará el destino actualizado (Siempre que sea de tipo directo)
        const urlDestinoRowValue = that.filaRedireccion.find(that.componentUrlDestino);
        // Establecer el valor
        urlDestinoRowValue.html(input.val().trim());

        // Controlar el tipo de redirección
        if (selectUrlType.children("option:selected").val() != "Direct"){
            urlDestinoRowValue.addClass("d-none");
        }else{
            urlDestinoRowValue.removeClass("d-none");
        }
    },

     /**
     * Método para cambiar la URL de origen cuando se esté escribiendo el valor del parámetro de la redirección
     * @param {jqueryElement} input Input donde se está escribiendo el nuevo valor del parámetro 
     */
    handleSetUrlRedirectionOrigen: function(input){
        const that = this;
        // Fila correspondiente al parámetro a editar
        that.filaRedireccion = input.closest('.redirection-row');        
        // Label donde se mostrará el destino actualizado (Siempre que sea de tipo directo)
        const urlOrigenRowValue = that.filaRedireccion.find(that.componentUrlOrigen);
        // Establecer el valor
        urlOrigenRowValue.html(input.val().trim());
    },

    /**
     * Método para añadir un nuevo parámetro. Cargará la fila con el panel desplegable para editar los datos de la redirección
     */
    handleAddParameter: function(){
        const that = this;
        // Id de la redirección editada
        const id = this.filaRedireccion.attr("id");
        // Nº aleatorio para funcionamiento del panel collapse
        const idCollapsePanel = guidGenerator();
        // Panel de listado de parámetros
        const panelParameters = this.filaRedireccion.find(".parameter-list") ;        
        // Nº de parámetros actuales dentro del panel
        const newParameterId = panelParameters.find(".parameter-row").length;

        // Plantilla Html para añadir el parámetro al DOM
        const parameterHTML = `
            <li
            class="component-wrap parameter-row"
            id="${id}"
            data-parameterkey="${id}"
            data-collapseid="panel_collapse_${idCollapsePanel}_${newParameterId}">
            <div class="component">
                <div class="component-header-wrap">
                    <div class="component-header">
                        <div class="component-no-sortable">
                            <span class="material-icons-outlined sortable-icon">alt_route</span>
                        </div>
                        <div class="component-header-content">
                            <div class="component-header-left">
                                <div class="component-name-wrap">
                                    <span class="component-parameterName"></span>
                                </div>
                            </div>
                            <div class="component-header-right">
                                <div class="component-actions-wrap">
                                    <ul class="no-list-style component-actions">
                                        <li>
                                            <a
                                                class="action-edit round-icon-button js-action-edit-component btnEditParameter"
                                                data-toggle="collapse"
                                                data-target="#panel_collapse_${idCollapsePanel}_${newParameterId}"
                                                role="button"
                                                aria-expanded="true"
                                                aria-controls="panel_collapse_${idCollapsePanel}_${newParameterId}">
                                                <span class="material-icons">edit</span>
                                            </a>
                                        </li>
                                        <li>
                                            <a
                                                class="action-delete round-icon-button js-action-delete btnDeleteParameter"
                                                href="javascript: void(0);">
                                                <span class="material-icons">delete</span>
                                            </a>
                                        </li>
                                    </ul>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        
            <div
                class="parameter-order-info collapse show"
                id="panel_collapse_${idCollapsePanel}_${newParameterId}">
                <div class="card card-body">
                    <div class="form-group mb-4">
                        <label class="control-label d-block">Valor parámetro</label>
                        <input
                            type="text"                            
                            data-labeltext="Valor del parámetro"
                            placeholder="Valor del parámetro"
                            class="form-control inputParameterUrlValue"
                            value=""
                        />
                        <small class="form-text text-muted">Valor del parámetro de la redirección.</small>
                    </div>
        
                    <div class="form-group mb-2">
                        <label class="control-label d-block">Url de destino</label>
                        <input
                            type="text"                            
                            placeholder="Url de destino"
                            class="form-control inputParameterUrlDestinationValue"
                            value=""
                        />
                        <small class="form-text text-muted mb-2">Url de destino de la redirección.</small>
                    </div>
                </div>
            </div>
        </li>        
        `;

        // Añadir la plantilla al panel de parámetros de redirecciones
        panelParameters.append(parameterHTML);
        
    },

    /**
     * Método para añadir una nueva redirección
     */
    handleAddNewRedirection: function(){
        const that = this;
        
        // Mostrar loading
        loadingMostrar();

        // Realizar la petición de crear nueva redirección
        GnossPeticionAjax(
            that.urlAddNewRedirection,
            null,
            true
        ).done(function (data) {
            // OK
            // Añadir la nueva fila al listado de redirecciones
            that.redirectionListContainer.append(data);
            // Referencia a la nueva página añadida
            const newRedirection = that.redirectionListContainer.children().last();
            // Reiniciar la operativa de gestión de Redirecciones para los nuevos items
            operativaGestionRedirecciones.init()                
            // Abrir el modal para poder editar/gestionar la nueva redirección añadida                                             
            newRedirection.find(that.btnEditRedirection).trigger("click");              
        }).fail(function (data) {
            console.log("ERROR =>" + data);
            mostrarNotificacion("error", data);
        }).always(function () {
            // Ocultar loading
            loadingOcultar();            
        });
    },

    /* Acciones para guardar datos de redirecciones */
    /************************************************/

    /**
     * Método para resetear errores antes de realizar el guardado (Flags, mensajes de error en inputs)
     */
    resetErrors: function(){
        const that = this;
        // Resetear el flag de guardado
        that.isErrorBeforeSaving = false;
        // Resetear los mensajes de error en los inputs
        eliminarErroresEnInputs();
    },


    /**
     * Método para eliminar una redirección previa confirmación realizada desde el modal     
     */
     handleDeleteRedirection: function(){
        const that = this;                  
        // 1 - Llamar al método para el guardado de páginas        
        that.handleSaveRedirections(function(error){
            if (error == false){
                // 2 - Ocultar el modal de eliminación de la página (En este caso modal-container)                
                dismissVistaModal();
                // 3 - Ocultar loading
                loadingOcultar();                
                // 4 - Ocultar la fila que se desea eliminar
                that.filaRedireccion.addClass("d-none");
                // 5 - Eliminar la fila que se acaba de eliminar                                
                that.filaRedireccion.remove();                
            }else{
                mostrarNotificacion("error", "Se ha producido un error al tratar de eliminar la redirección. Contacta con el administrador de la comunidad");
            } 
        }); 
    },     


    /**
     * Método para comenzar el proceso de guardado de datos. Comprobar errores, recopilar datos y proceder al guardado de
     * redirecciones
     * @param {function} completion : Método a ejecutar cuando finalice el metodo de guardado de datos
     */
    handleSaveRedirections: function (completion) {
        const that = this;

        // Resetear todos los flags por errores de guardado
        that.resetErrors();
        
        // Comprobar datos antes de guardar
        that.checkErrorsBeforeSaving();
        // Si no se ha producido ningún error (Datos no vacíos) proceder a guardarlo en servidor
        if (that.isErrorBeforeSaving == false) {                    
            that.getRedireccionData();
            // Realizar la petición para guardado de redirecciones
            that.saveRedirections(function(error){                
                if (error == false){
                    // Cambios / Nueva redirección guardada correctamente -> Quitar clase de "newRedirection" y "modified" para NO eliminar la redirección al cerrar el modal
                    $(".newRedirection").removeClass("newRedirection");
                    $(".modified").removeClass("modified");
                }
                completion != undefined && completion(error);
            });
            
        }else{
            // Errores encontrados -> No guardar
            completion != undefined && completion(true)
        }
    }, 


    /**
     * Método para ejecutar el guardado cuando se pulse en el botón "Guardar" del modal de una redirección en concreto.
     */
     handleSaveCurrentRedirection: function(){
        const that = this;

        that.handleSaveRedirections(function(error){
            if (error == false){
                // Ocultar el modal -> Ok en guardado
                const modalRedirection = $(that.filaRedireccion).find(`.${that.modalRedirectionClassName}`);                                          
                dismissVistaModal(modalRedirection);                             
            }
        });
    },    


    /**
     * Método para realizar la petición del guardado de las redirecciones posteriormente a que se haya comprobado que los datos están correctamente introducidos.
     * @param {function} completion : Método a ejecutar cuando el proceso de guardado finalice
     */
    saveRedirections: function(completion){
        const that = this;
        // Mostrar loading 
        loadingMostrar();

        // Realizar la petición de guardado
        GnossPeticionAjax(
            that.urlSaveRedirections,
            that.ListaRedirecciones,
            true
        ).done(function (data) {
            // OK guardado            
            // Mostrar mensaje de guardado correcto            
            mostrarNotificacion("success", "Los cambios se han guardado correctamente");
            // Sin errores -> OK
            completion != undefined && completion(false);
            
        }).fail(function (data) {
            // KO en guardado de datos
            const error = data.split('|||');                        
            mostrarNotificacion("error", "Se ha producido un error al tratar de guardar la redirección. Por favor contacta con el administrador.");
            // Con errores -> KO
            completion != undefined && completion(false);

        }).always(function () {
            OcultarUpdateProgress();
        });        
    },

    /**
     * Método que comprobará los datos de redirecciones antes de realizar el guardado
     */
     checkErrorsBeforeSaving: function(){
        const that = this;
        that.checkEmptyFields();
    },    
    

    /**
     * Método que construirá el objeto de Lista de redirecciones para posteriormente guardarlos
     */
    getRedireccionData: function(){
        const that = this;
        
        // Resetear los datos a guardar
        that.ListaRedirecciones = {};

        // Indice para creación del objeto a guardar
        let num = 0;       

        $('.redirection-row').each(function () {
            const fila = $(this);
            if (fila.hasClass('modified') || fila.hasClass('deleted')) {
                // Panel modal que contiene toda la información
                const panelEdicion = fila.find('.modal-redirection');
                const id = fila.attr('id');                
                const selectTipoRedireccion = fila.find(that.cmbEditarTipoRedirection);
                const TipoRedireccion = selectTipoRedireccion.children("option:selected").val();
                
                // Panel del que habrá que obtener los campos o parámetros (Directa | Parametrizada)
                let panCampos = '';

                if (TipoRedireccion == 'Direct') {
                    panCampos = panelEdicion.find('.redireccionDirecta');
                }
                else if (TipoRedireccion == 'Parameterised') {
                    panCampos = panelEdicion.find('.parametrizada');
                }

                // Prefijo clave para la construcción de items
                const prefijoClave = 'ListaRedirecciones[' + num + ']';
                that.ListaRedirecciones[prefijoClave + '.Key'] = id;

                // Url de origen
                that.ListaRedirecciones[prefijoClave + '.OriginalUrl'] = panelEdicion.find(that.inputUrlOrigen).val();
                // Destino Url de momento vacía (Dependiendo del tipo de redirección)
                that.ListaRedirecciones[prefijoClave + '.DestinationUrl'] = '';
                // Preservar filtros de momento falso (Dependiendo del tipo de redirección)
                that.ListaRedirecciones[prefijoClave + '.PreserveFilters'] = false;
                // Indicar si la redirección se ha de borrar
                that.ListaRedirecciones[prefijoClave + '.DeleteRedirection'] = fila.hasClass('deleted');
                // Indicar que la redirección es nueva o editada para backend
                that.ListaRedirecciones[prefijoClave + '.Edited'] = true;

                // Obtener valores si la redirección es Directa
                if (TipoRedireccion == 'Direct') {
                    // Tipo de redirección
                    that.ListaRedirecciones[prefijoClave + '.RedirectionType'] = 'Direct';
                    // Destino Url
                    that.ListaRedirecciones[prefijoClave + '.DestinationUrl'] = panCampos.find(that.inputUrlDestino).val();
                    // Mantener filtros url original                    
                    that.ListaRedirecciones[prefijoClave + '.PreserveFilters'] = panCampos.find(`#rbMantenerFiltrosOrigen_SI_${id}`).is(':checked'); //panCampos.find('[name="mantenerFiltros"]').is(':checked');
                }
                else{
                    // Obtener valores si la redirección es Parametrizada
                    that.ListaRedirecciones[prefijoClave + '.RedirectionType'] = 'Parameterised';
                    // Recorrer los parámetros de la Redirección
                    
                    // Cada uno de los paneles que contienen los parámetros de la fila que está siendo analizada
                    const panelParametros = fila.find('.parameter-order-info');

                    // Recorrer los parámetros
                    panelParametros.each(function (i, item) { 
                        // Prefijo del parámetro para guardarlo en BD
                        const prefijoClaveParametro = prefijoClave + '.ParameterValues[' + i + ']';
                        // Nombre del parámetro 
                        const inputNombreParametro = fila.find(".inputNombreParametro");                                             
                        // Inputs de Valor y Destino
                        const inputValor = $(this).find(".inputParameterUrlValue");
                        const inputUrlDestino = $(this).find(".inputParameterUrlDestinationValue");

                        // Asignación de los datos de parámetros
                        that.ListaRedirecciones[prefijoClave + '.ParameterName'] = inputNombreParametro.val();
                        that.ListaRedirecciones[prefijoClaveParametro + '.Value'] = inputValor.val();
                        that.ListaRedirecciones[prefijoClaveParametro + '.DestinationUrl'] = inputUrlDestino.val();                                                                                             
                    });                
                }
                num++;
            }
        });
      },

    /**
     * Método para comprobar los campos vacíos de cada redirección (Url origen, Url destino)
     */
    checkEmptyFields: function () {
        const that = this;

        // Comprobar los campos vacíos de todas las redirecciones (solo de las modificadas y no de las que se desean)        
        $('.redirection-row:not(".deleted").modified').each(function () {            
            // Comprobar errores siempre que no haya errores previos
            if (that.isErrorBeforeSaving == false){
                const fila = $(this);
                that.checkUrlOrigen(fila);
                that.checkUrlDestino(fila);                               
            }            
        });
    },


    /**
     * Método que comprobará la URL origen de una redirección. Es un proceso a realizar antes del guardado
     * ejecutado en el método checkEmptyFields
     * @param {jqueryElement} fila Fila de la redirección a revisar
     */
    checkUrlOrigen: function(fila){
        const that = this;
        const input = fila.find(that.inputUrlOrigen);
        // Comprobar que no esté vacía
        if (input.val() == ""){            
            that.isErrorBeforeSaving = true;
        }
        
        if (that.isErrorBeforeSaving == true) {
            that.showErrorUrlOrigenEmpty(input);
            that.showSavingError();
        }
    },

    /**
     * Método que comprobará la URL destino de una redirección. Es un proceso a realizar antes del guardado
     * ejecutado en el método checkEmptyFields
     * @param {jqueryElement} fila Fila de la redirección a revisar
     */
     checkUrlDestino: function(fila){
        const that = this;
        const input = fila.find(that.inputUrlDestino);
        
        // Select del tipo de redirección
        const selectUrlType = fila.find(that.cmbEditarTipoRedirection);
   
        // Comprobar si el tipo de redirección es Directa para comprobar que la URL destino no está vacía        
        if (selectUrlType.children("option:selected").val() == "Direct"){
            // Tipo Directa            
            // Comprobar que no esté vacía
            if (input.val() == ""){            
                that.isErrorBeforeSaving = true;
            }
            if (that.isErrorBeforeSaving == true) {
                that.showErrorUrlDestinoEmpty(input);
                that.showSavingError();
            }                     
        }else{
            // Tipo Parametrizada -> Comprobar que hay parámetros                    
            // Comprobar que el nombre del parámetro no es vacío
            const inputNombreParametro = fila.find(".inputNombreParametro");
            if (inputNombreParametro.val() == ""){            
                that.isErrorBeforeSaving = true;
                that.showErrorNombreParametroEmpty(inputNombreParametro);
                return;
            }
            
            // Cada uno de los paneles que contienen los parámetros de la fila que está siendo analizada
            const panelParametros = fila.find('.parameter-order-info');
            // Comprobar que hay parámetros
            that.isErrorBeforeSaving = panelParametros.length <= 0;
            if (!that.isErrorBeforeSaving) {
                panelParametros.each(function (item) {
                    if (!that.isErrorBeforeSaving) {
                        // Analizar que el Valor y la Url de destino estén rellenados
                        const inputValor = $(this).find(".inputParameterUrlValue");
                        const inputUrlDestino = $(this).find(".inputParameterUrlDestinationValue");
                        let inputConError = undefined;
                        if (inputValor.val() == ""){
                            inputConError = inputValor;
                        }

                        if (inputUrlDestino.val()==""){
                            inputConError = inputUrlDestino;
                        }
                        // Marcar que se ha encontrado un error
                        if (inputConError != undefined){
                            that.isErrorBeforeSaving = true;
                            that.showGenericError(inputConError, "Los parametros deben tener todos sus campos rellenados.");
                            return;
                        }
                    }
                });
            }
            else{
                // Error -> No hay parámetros
                that.showErrorNoParameters();
            }
        }
    }, 
    
    /**
     * Método para mostrar un error genérico en un input en concreto con un mensaje proporcionado
     */
    showGenericError: function(input, message){
        comprobarInputNoVacio(input, true, false, message, 0);
    },

    /**
     * Método para mostrar el error por nombre del parámetro vacío en tipo de redirección Parametrizada.
     * @param {jqueryElement} input Input donde se ha encontrado el error
     */
     showErrorNombreParametroEmpty: function(inputUrl){        
        comprobarInputNoVacio(inputUrl, true, false, "El nombre del parámetro no puede estar vacío.", 0);
    },


    /**
     * Método para mostrar el error por URL origen vacía.
     * @param {jqueryElement} inputUrl Input donde se ha encontrado el error
     */
    showErrorUrlOrigenEmpty: function(inputUrl){        
        comprobarInputNoVacio(inputUrl, true, false, "La url de origen no puede estar vacía.", 0);
    },

    /**
     * Método para mostrar el error por URL destino vacía.
     * @param {jqueryElement} inputUrl Input donde se ha encontrado el error
     */
    showErrorUrlDestinoEmpty: function(inputUrl){        
        comprobarInputNoVacio(inputUrl, true, false, "La url destino no puede estar vacía.", 0);
    },   
    
    /**
     * Método para mostrar el error por falta de parámetros en una redirección de tipo Parametrizada     
     */
    showErrorNoParameters: function(){        
        mostrarNotificacion("error", "Las redirecciones parametrizadas tienen que tener, al menos, un parametro con todos sus campos rellenados");
    },

    /**
     * Método para mostrar el mensaje de error general
     * @param {*} claveMensaje 
     */
    showSavingError: function (claveMensaje) {
        var mensaje = 'Ha habido errores en el guardado de las redirecciones, revisa los errores marcados';
        if (claveMensaje == 'ERRORURLORIGENVACIA') {
            mensaje = 'La url de origen no puede estar vac&#xED;a';
        }
        if (claveMensaje == 'ERRORURLDESTINOVACIA') {
            mensaje = 'La url de destino no puede estar vac&#xED;a';
        }
        if (claveMensaje == 'ERRORNOMPARAMETROVACIO') {
            mensaje = 'El nombre del par&#xE1;metro no puede estar vac&#xED;o';
        }
        if (claveMensaje == 'ERRORSINPARAMETROS') {
            mensaje = 'Las redirecciones parametrizadas tienen que tener, al menos, un parametro con todos sus campos rellenados';
        }
        if (claveMensaje == 'ERRORSINCAMBIOS') {
            mensaje = 'No existen cambios en las redirecci&#xF3;nes que guardar';
        }
        $('input.guardarTodo').before('<div class="error general">' + mensaje + '</div>');
    },
}


/**
 ***************************************************************************************
 * Operativas adicionales
 * *************************************************************************************
 */

 /**
  * Operativa para la ordenación e inserción de categorías mediante el arrastre de items
  * Usada en: operativaGestionCategorias
  */
 const operativaCategoriasSortable = {
    init: function () {
        this.categories.init();        
    },

    /**
     * Gestión de categorías para el sortable
     */
    categories: {
        init: function () {
            this.initCategories();            
        },

        /**
         * Inicializar las categorías (padre) para el sortable
         */       
        initCategories: function () {
            const categories_lists = document.querySelectorAll(
                ".js-community-categories-list"
            );
            const categoriesOptions = this.getAddedCategoryOptions();
            categories_lists.forEach((category_list) => {
                Sortable.create(category_list, categoriesOptions);
            });
        },        

        /**
         * Recoger categorías padre y asignar eventos y configuración          
         */

         getAddedCategoryOptions: function () {
            var that = this;
            return {
                group: {
                    name: "js-community-categories-list",
                },
                sort: true,
                fallbackOnBody: true,
		        swapThreshold: 0.65,
                animation: 150,
                handle: ".js-component-sortable-category",
                onAdd: function (evt) {},
                onChoose: function (evt) {},
                onUnChoose: function (evt) {},
                onEnd: function (evt) {
                    const itemEl = evt.item;  // dragged HTMLElement

                    // Insertar categoría dentro de ...
                    const catIdMovida = $(itemEl).children().closest('.component').data('id');                    
                    let catIdPadre = $(itemEl).parents().closest('.component').data('id');
                    // Si no existe catIdPadre, se está moviendo una categoría que no tiene padre -> Asignarle la catPadre por defecto
                    if (catIdPadre == undefined){
                        catIdPadre = '00000000-0000-0000-0000-000000000000';
                    }                                                                                        
                    // Mover y posteriormente ordenar la categoría seleccionada                    
                    operativaCategoriasSortable.handleMoveCategory(catIdMovida, catIdPadre, function(isOk){                        
                        if (isOk){
                            // Ordenar categoría y situarla debajo de su inmediato superior
                            const categoriaDebajoDeCategoriaArrastrada = $($(itemEl).siblings()[evt.newDraggableIndex - 1]);
                            const catIdDebajoDe = categoriaDebajoDeCategoriaArrastrada.children().data("id");
                            operativaCategoriasSortable.handleSortCategory(catIdMovida, catIdDebajoDe);
                        }
                    });
                },
            };
        },        
    },

    /**
     * Acción de mover la categoría arrastrada dentro de la categoría indicada
     * @param {String} catIdMovida : Id de la categoría que se está moviendo
     * @param {String} catIdPadre : Id de la categoría padre donde se está moviendo
     * @param {function} completion : Bloque de código que se ejecutará cuando finalice el mover categorías
     */
    handleMoveCategory: function(catIdMovida, catIdPadre, completion){      
        const that = this;        
                 
        if (catIdPadre != '') {
            // Mostrar loading en el tesauro
            loadingMostrar();

            // Seleccionar las categoría a mover según el ID
            const categorias = catIdMovida;
 
            // Obtener los parámetros comunes
            operativaGestionCategorias.obtenerParametrosComunes();
 
            // Construir objeto para mover categoría
            operativaGestionCategorias.parametrosComunes.CategoriasSeleccionadas =  categorias;
            operativaGestionCategorias.parametrosComunes.parentKey = catIdPadre;            
 
            GnossPeticionAjax(operativaGestionCategorias.urlMoveCategoria, operativaGestionCategorias.parametrosComunes, true)
            .done(function (data) {
                // OK                
                // Cargar los datos en el contenedor del tesauro                
                operativaGestionCategorias.panelTesauro.html(data);
                // Nuevos elementos del DOM insertados -> Resetear comportamientos
                operativaGestionCategorias.config(operativaCategoriasSortable.pParams);
                operativaGestionCategorias.configEvents();
                operativaGestionCategorias.triggerEvents();   
                // Reiniciar comportamiento de operativaCategoriasSortable
                operativaCategoriasSortable.init();
                // La ejecución se ha realizado correctamente
                completion(true);
            }).fail(function (data) {                                
                // Se ha producido algún error
                mostrarNotificacion("error",data)                
                loadingOcultar();
                completion(false);
            }).always(function () {
                // No ocultar ocultar loading ya que se ocultará en handleSortCategory que se realiza justo después
                // loadingOcultar();
            });
        }    

    },

    /**
     * Acción de ordenar la categoría arrastrada. Se colocará justo debajo de inmediata superior
     * @param {*} catIdMovida: Id de la categoría que se está moviendo
     * @param {*} catIdSuperior : Id de la categoría donde la categoría movida se colocará debajo.
     */
    handleSortCategory: function(catIdMovida, catIdSuperior) {
        
        // Comprobar que la categoría superior al menos existe
        if (catIdSuperior != undefined) {
            // El loading sigue mostrándose desde handleMoveCategory -> No mostar nada
        
            const categorias = catIdMovida;

            // Obtener los parámetros comunes
            operativaGestionCategorias.obtenerParametrosComunes();
 
            // Construir objeto para mover categoría
            operativaGestionCategorias.parametrosComunes.CategoriasSeleccionadas =  categorias;
            operativaGestionCategorias.parametrosComunes.parentKey = catIdSuperior; 

            GnossPeticionAjax(operativaGestionCategorias.urlSortCategoria, operativaGestionCategorias.parametrosComunes, true)
            .done(function (data) {
                // OK               
                // Cargar los datos en el contenedor del tesauro                
                operativaGestionCategorias.panelTesauro.html(data);
                // Nuevos elementos del DOM insertados -> Resetear comportamientos
                operativaGestionCategorias.config(operativaCategoriasSortable.pParams);
                operativaGestionCategorias.configEvents();
                operativaGestionCategorias.triggerEvents();   
                // Reiniciar comportamiento de operativaCategoriasSortable
                operativaCategoriasSortable.init();
            }).fail(function (data) {
                mostrarNotificacion("error", data);
            }).always(function () {
                // Ocultar loading
                loadingOcultar();
            });
        }else{
            // Ocultar loading de la acción handleMoveCategory
            loadingOcultar();
        }
    },
};


/**
  * Operativa para la ordenación e inserción de Páginas mediante el arrastre de items
  * Usada en: operativaGestionPaginas
  * OBSOLETO --> Sustituida por operativaPaginasNestedSortable
  */
 const operativaPaginasSortable = {

    init: function () {
        this.initPages();
    },
    initPages: function () {
        const pages_lists = document.querySelectorAll(
            '.js-community-pages-list'
        );
        const pagesOptions = this.getAddedPageOptions();
        pages_lists.forEach((page_list) => {
            Sortable.create(page_list, pagesOptions);
        });
    },
    getAddedPageOptions: function () {
        return {
            group: {
                name: 'js-community-pages-list',
            },
            sort: true,            
            swapThreshold: 0.1,
            animation: 300,            
            handle: '.js-component-sortable-page',
            onAdd: function (evt) {},
            onChoose: function (evt) {},
            onUnChoose: function (evt) {},
            onEnd: function(evt){
                // Acción finalizada de arrastrar/ordenar páginas
                const $itemEl = $(evt.item);  // dragged HTMLElement                
                // Destino del item arrastrado
                const $parentNode = $(evt.to); 
                // Index nuevo de la página movida
                const newIndex = evt.newDraggableIndex;
                operativaGestionPaginas.handleReorderPages($itemEl, $parentNode);
            },
        };
    }, 

 };


const operativaPaginasNestedSortable = {
    
    // Inicializar el comportamiento 
    init: function(){
        this.initPages();
    },

    initPages: function(){
        // Obtener la configuración para la NestedSortable
        const configNestedSortable = this.configNestedSortable();        
        // Aplicar el comportamiento NestedSortable
        this.ns = $('#id-added-pages-list').nestedSortable(configNestedSortable);
    },

    /**
     * Método para configurar el elemento de "NestedSortable"
     * @returns 
     */
    configNestedSortable: function(){
        const that = this;
        // Posición Top y Derecha del item arrastrado para controlar si hace falta moverlo si se suelta en la misma posición
        let startItemTop = "";
        let startItemLeft = "";
        let stopItemTop = "";
        let stopItemLeft = "";

        const nestedSortableConfiguration = {
            listType: "ul",
            forcePlaceholderSize: true,
            handle: '.js-component-sortable-page',            
            items: '.page-row',
            opacity: .6,
            placeholder: 'ui-state-highlight',
            revert: 250,
            tabSize: 25,
            tolerance: 'pointer',
            toleranceElement: '> div',
            //maxLevels: 4,*/
            isTree: true,
            expandOnHover: 700,
            //startCollapsed: false,  
            start: function(event, ui){
                const offsetTopItemMoved = ui.item[0].offsetTop;
                startItemTop = offsetTopItemMoved;
                const offsetLeft = ui.item[0].offsetLeft;
                startItemLeft = offsetLeft;
            },                      
            stop: function(event, ui){    
                var flag = ui.flag;       
                const offsetTopItemMoved = ui.item[0].offsetTop;
                stopItemTop = offsetTopItemMoved;
                const offsetLeft = ui.item[0].offsetLeft;
                stopItemLeft = offsetLeft;           
                // Comparar la posición origen con la final del item arrastrado
                if ( startItemTop != stopItemTop || startItemLeft != stopItemLeft){
                    // Elemento movido
                    const $itemEl = $(ui.item);
                    operativaGestionPaginas.handleReorderPages($itemEl);                                       
                }                                
            },                     
        }            
        return nestedSortableConfiguration;
    },
}



 // Plugin para cargar imagen de cabecera desde sección Comunidad -> Información general
; (function ($) {
    $.imageDropArea = function (element, options) {
        var defaults = {
          inputSelector: ".image-uploader__input", // Input para poder elegir una nueva imagen
          dropAreaSelector: ".image-uploader__drop-area", // Sección donde se puede elegir una nueva imagen o arrastrarla
          preview: ".image-uploader__preview",
          previewImg: ".image-uploader__img",
          errorDisplay: ".image-uploader__error", 
          panelAccionesImagen: "", // Panel de acciones que contiene botones para cambiar o eliminar imagen 
          urlUploadImage: document.location.href, // Url para poder subir la imagen
          urlDeleteImage: document.location.href, // Url para poder eleminar imagen  (si se precisa)
          contenedorImagen: "", // Contendor donde se encuentra la zona para arrastrar o seleccionar nueva imagen
          urlUploadImageType: "fileUpload",
          inputHiddenImageLoaded: "#inputHiddenImageLoaded",
          panelVistaContenedor: undefined, // Panel donde se cargará la vista devuelta por backend (toda la vista de la cabecera)
          completion: options.completion, // Funcionalidad que se realizará cuando se complete la acción de subir la imagen
        };
        var plugin = this;
        
        plugin.settings = {};

        plugin.init = function () {
          plugin.settings = $.extend({}, defaults, options);
          plugin["input"] = $(plugin.settings.inputSelector);
          plugin["dropAreaSelector"] = $(plugin.settings.dropAreaSelector);
          plugin["preview"] = $(plugin.settings.preview);
          plugin["previewImg"] = $(plugin.settings.previewImg);
          plugin["errorDisplay"] = $(plugin.settings.errorDisplay);
          onInputChange();
          addDragAndDropEvents();

        };

        var onInputChange = function () {
          plugin.input.change(function () {
            var data = new FormData();
            var files = plugin.input.get(0).files;
            if (files.length > 0) {
              // Mostrar Loading
              loadingMostrar();              
              data.append(defaults.urlUploadImageType, files[0]);
              $.ajax({
                url: plugin.settings.urlUploadImage,
                type: "POST",
                processData: false,
                contentType: false,
                data: data,
                success: function (response) {
                  // Subida de imagen correcta
                  //hideError();
                  onSuccesResponse(response);
                },
                error: function (er) {
                    // Error en la subida de la imagen
                    loadingOcultar();
                    mostrarNotificacion("error", er);
                    //displayError(er.statusText);
                    //loadingOcultar();        
                },
              });
            }            
          });
        };

        /**
         * Ocultar error en panel configurado
         * @param {String} error : Mensaje de error
         */
        var displayError = function (error) {
          plugin.errorDisplay.find(".ko").text(error);
          plugin.errorDisplay.find(".ko").show();
          plugin.preview.removeClass("show-preview");
        };
        /**
         * Ocultar error en panel configurado
         */
        var hideError = function () {
          plugin.errorDisplay.find(".ko").hide();
        };

        /**
         * Método que se ejecuta cuando la subida de la imagen ha sido correcta. Llamada desde onInputChange desde onSuccessResponse
         * @param {} response 
         */
        var onSuccesResponse = function (response) {                           
            plugin.settings.completion(response, true);            
            loadingOcultar();
        };

        /**
         * Mostrar la imagen subida en el lugar correspondiente
         * @param {*} response 
         */
        var showImagePreview = function (response) {
            // Url de servicio de recursos
            var urlContent = $('input.inpt_baseURLContent').val();
            plugin.previewImg.attr(
            "src",
            urlContent + "/" + response
            );
            //plugin.preview.addClass("show-preview");
        };

        var addDragAndDropEvents = function () {
          plugin.dropAreaSelector
            .off("dragenter dragover")
            .on("dragenter dragover", function (e) {
              e.preventDefault();
              e.stopPropagation();
            });

          plugin.dropAreaSelector.off("click").on("click", function (e) {
            e.preventDefault();
            e.stopPropagation();
            plugin.input.trigger("click");
          });

          plugin.dropAreaSelector.off("dragleave").on("dragleave", function (e) {
            e.preventDefault();
            e.stopPropagation();
          });

          plugin.dropAreaSelector.off("drop").on("drop", function (e) {
            e.preventDefault();
            e.stopPropagation();
            let dt = e.originalEvent.dataTransfer;
            let files = dt.files;
            plugin.input.get(0).files = files;
            plugin.input.trigger("change");
          });
        };
        plugin.init();
      };

      // add the plugin to the jQuery.fn object
      $.fn.imageDropArea = function (options) {
        return this.each(function () {
          if (undefined == $(this).data("imageDropArea")) {
            var plugin = new $.imageDropArea(this, options);
            $(this).data("imageDropArea", plugin);
          }
        });
      };
})(jQuery);
