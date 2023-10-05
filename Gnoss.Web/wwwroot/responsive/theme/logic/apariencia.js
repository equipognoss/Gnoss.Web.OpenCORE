/**
 ***************************************************************************************
 * Logica / Operativas de comportamiento JS para la sección de Apariencia de la Comunidad del DevTools
 * *************************************************************************************
 */

/**
  * Operativa para la gestión y configuración de Vistas de la comunidad
  */
const operativaGestionVistas = {
    /**
    * Inicializar operativa
    */
     init: function (pParams) {
        this.pParams = pParams;
        this.config(pParams);
        this.configEvents();
        this.configRutas(); 
        this.triggerEvents(); 
        
        // Dejar seleccionado por defecto "Personalizadas"
        const facetaSeleccionada = $(`.${this.seleccionFacetaTipoPlantillaClassName}[data-type="personalizada"]`);
        facetaSeleccionada.trigger("click");
    },

    /**
     * Lanzar comportamientos u operativas necesarias para el funcionamiento de la sección
     */
    triggerEvents: function(){
        const that = this;           

        // Indicar el nº de items existente
        setTimeout(function(){
            that.handleCheckNumberOfVistas();
        },500)
        

        // Inicializar el componente de dragAndDropGnoss
        dragAndDropGnoss();        
    },   

    /**
     * Configuración de las rutas de acciones necesarias para hacer peticiones a backend
     */
    configRutas: function (pParams) {
        // Url para editar un certificado
        this.urlBase = refineURL();
        this.urlInvalidateViews = `${this.urlBase}/invalidateviews`;  
        this.urlShareViews = `${this.urlBase}/compartir-vistas-en-dominio`;  
    },  

    /*
    * Inicializar elementos de la vista
    * */
    config: function (pParams) {        
        // Contenedor modal
        this.modalContainer = $("#modal-container");
        this.modalDeleteView = $("#modal-delete-view"); 
        this.modalUploadViewClassName = "modal-subir-vista";
        this.modalNewPersonalization = $("#modal-new-view-personalizacion");
        this.modalShareDomain = $("#modal-share-views");
        // Buscador
        this.txtBuscarVistas = $("#txtBuscarVistas");
        // Contenedor de facetas
        this.vistaListContainerId = 'id-added-vista-list';
        this.vistaListContainer = $(`#${this.vistaListContainerId}`);        
        // Botón de la fila para "Subir una vista"
        this.btnUploadOriginalVistaClassName = "btnUploadOriginalVista";
        // Botón de la fila para "Subir una vista" personalizada
        this.btnUploadEditedVistaClassName = "btnUploadEditedVista";

        // Botón de la fila para "Descargar una vista"
        this.btnDownloadOriginalVistaClassName= "btnDownloadOriginalVista";
        this.btnDownloadEditedVistaClassName = "btnDownloadEditedVista"


        // Botón para eliminar una personalización de una vista (No CMS)
        this.btnDeletePersonalizacionClassName = "btnDeleteEditedVista";

        // Botón de la fila para eliminar una personalización del componente CMS
        this.btnDeletePersonalizacionCMSClassName = "btnDeletePersonalizacionCMS"; 
        this.btnSaveUploadPersonalizacionCMSClassName = "btnSaveUploadPersonalizacionCms";
        this.btnUploadPersonalizacionCMSClassName = "btnUploadPersonalizacionCMS"

        //Botón para compartir vistas en un dominio
        this.btnSaveSharedDomains = "btnSaveSharedDomains";
        this.btnStopSharing = "btnStopSharing";
        // Botón de confirmar la eliminación de una vista
        this.btnConfirmDeleteClassName = "btnConfirmDelete";
        
    
        // Panel de listado de facetas
        this.listadoFacetasCmsClassName = "facetasItems";
        // Clase de cada faceta item del listado
        this.facetaClassName = "faceta";

        // Botón para invalidar vistas
        this.btnInvalidarVistas = $("#btnInvalidarVistas");
                
        // Contador de número de items existentes
        this.numVistas = $("#numVistas");    
        // Contador de vistas de tipo web, facetas y resultados
        this.numVistasTotal = $("#numVistas");
        this.numResultados = $(".num-resultados");
        this.numViewWeb = $(".numViewWeb");
        this.numViewResultados = $(".numViewResultados");
        this.numViewFacetas = $(".numViewFacetas");
        this.numViewTodas = $(".numViewTodas");
        this.numViewObjetoConocimiento = $(".numViewObjetoConocimiento");
        this.numViewComponentesCMS = $(".numViewComponentesCMS");

        this.numViewTypeTodas = $(".numViewTypeTodas")
        this.numViewTypeOriginal = $(".numViewTypeOriginal");
        this.numViewTypePersonalizadas = $(".numViewTypePersonalizadas");

        // Contador de vistas de tipo Componentes CMS (Genérico)
        this.numView = $(".numView");


        /* Inputs del modal de creación */
        // Input del nombre de la vista a crear o editar
        this.txtViewNameClassName = "txtViewName";
        // Botón para guardar
        this.btnSaveUploadViewClassName = "btnSaveUploadView";
        // Input type file de la vista
        this.dragAndDropInputFileClassName = "dragAndDropInputFile";
        // Input o div donde se encuentra el nombre de la vista (En el modal de subir vista)
        this.valueViewClassName = "valueView";
        // Subtitulo o nombre completo de la vista en la fila
        this.valueViewInRowClassName = "component-name-subtitle";
        // Input del dominio donde se compartirán las vistas personalizadas
        this.txtDomainName = "txtDomainName";

        // Opciones de facetas para la selección de plantillas (Web, Facetas, Resultados)
        this.seleccionFacetaPlantillaClassName = "seleccionFacetaPlantilla"; 
        this.seleccionFacetaPlantillaComponenteCMSClassName = "seleccionFacetaPlantillaComponenteCMS";
        this.seleccionFacetaTipoPlantillaClassName = "seleccionFacetaTipoPlantilla"

        // Listado de items Web, Resultados, Facetas, Componentes CMS
        this.panResultados = $("#panResultados");
        this.vistaListWeb = $("#id-added-vistas-list-web");
        this.vistaListResultados = $("#id-added-vistas-list-resultados");
        this.vistaListFacetas = $("#id-added-vistas-list-facetas");
        this.vistaListComponentesCMS = $("#id-added-vistas-list-componentes-cms");

        // Contenedor de vistas (Web, Facetas, Resultados, CMS)
        this.componentListClassName = "js-community-vistas-list"; 
                
        // Selección en la faceta de tipo "Recurso, Listado de Recursos y Grupo de componentes" en vistas de tipo CMS.
        this.seleccionFacetaPlantillaComponenteCmsRecursoId = "seleccionFacetaPlantillaComponenteCmsRecurso"
        this.seleccionFacetaPlantillaComponenteCmsRecurso = $(`#${this.seleccionFacetaPlantillaComponenteCmsRecursoId}`);
        this.seleccionFacetaPlantillaComponenteCmsListadoDeRecursosId = "seleccionFacetaPlantillaComponenteCmsListadoDeRecursos";
        this.seleccionFacetaPlantillaComponenteCmsListadoDeRecursos = $(`#${this.seleccionFacetaPlantillaComponenteCmsListadoDeRecursosId}`);            
        this.seleccionFacetaPlantillaComponenteCmsGrupoComponentesId = "seleccionFacetaPlantillaComponenteCmsGrupoComponentes";
        this.seleccionFacetaPlantillaComponenteCmsGrupoComponentes = $(`#${this.seleccionFacetaPlantillaComponenteCmsGrupoComponentesId}`); 
                
        // Clase de facetas para tipo CMS: Recurso, Listado Recurso, Grupo de componentes
        this.seleccionFacetaPlantillaComponenteCMSRecursosClassName = "seleccionFacetaPlantillaComponenteCMSRecursos";
        // Botones para añadir personalización de Recurso, Listado de recursos y Grupo componentes
        this.btnAniadirPersonalizacionRecursosClassName = "btnAniadirPersonalizacionRecursos";
        // Personalización de "Recurso"
        this.btnAniadirPersonalizacionRecursoId = "btnAniadirPersonalizacionRecurso"
        this.btnAniadirPersonalizacionRecurso = $(`#${this.btnAniadirPersonalizacionRecursoId}`);
        // Personalización de "Listado"
        this.btnAniadirPersonalizacionListadoRecursoId = "btnAniadirPersonalizacionListadoRecurso";
        this.btnAniadirPersonalizacionListadoRecurso = $(`#${this.btnAniadirPersonalizacionListadoRecursoId}`);
        // Personalización de "Grupo"        
        this.btnAniadirPersonalizacionGrupoRecursoId = "btnAniadirPersonalizacionGrupoRecurso";
        this.btnAniadirPersonalizacionGrupoRecurso = $(`#${this.btnAniadirPersonalizacionGrupoRecursoId}`);

        // Panel padre contenedor de los filtros de facetas
        this.panFiltros = $("#panFiltros");
        // Panel contenedor de los filtros de facetas activas
        this.panListadoFiltros = $("#panListadoFiltros");
        // Botón de eliminación de todas las facetas activas
        this.btnBorrarFiltrosClassName = "borrarFiltros-wrap";
        // Botón unitario para cada borrado de cada faceta
        this.btnRemoveFacetaClassName = "remove-faceta";
        // Panel de facetas donde se encuentran Plantillas de Componentes CMS
        this.panelFacetasComponentesCMS = $("#panelFacetasComponentesCMS"); 
        
        // Panel de datos extra en el panel editar de una vista CMS de tipo "Recurso"
        this.panelDatosExtraClassName = "panelDatosExtra";        
        
        // Clase de la fila de la vista
        this.filaVistaClassName = "row-vista";
        // Fila de la vista que está siendo editada
        this.filaVista = undefined;            
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
            that.filaVista = undefined;
        });

        // Comportamientos del modal para eliminar una vista
        this.modalDeleteView.on('show.bs.modal', (e) => {
            // Aparición del modal
            // Establecer quién ha disparado el modal para poder cambiar sus atributos una vez se guarden los grupos desde el modal
            that.triggerModalContainer = $(e.relatedTarget);                              
        })
        .on('hide.bs.modal', (e) => {
            // Acción de ocultar el modal
        })
        .on('hidden.bs.modal', (e) => {
            // Vaciar el modal
            // Eliminar la posible row de "Eliminación"
            $(".deleted").removeClass("deleted");
            that.filaVista = undefined;
        });

        // Comportamientos del modal para personalizar una vista
        this.modalNewPersonalization.on('show.bs.modal', (e) => {
            // Aparición del modal
            // Establecer quién ha disparado el modal para poder cambiar sus atributos una vez se guarden los grupos desde el modal
            that.triggerModalContainer = $(e.relatedTarget);                              
        })
        .on('hide.bs.modal', (e) => {
            // Acción de ocultar el modal
        })
        .on('hidden.bs.modal', (e) => {
            // Resetear el contenido del modal
            // Vaciar el nombre
            $(e.currentTarget).find(`.${that.txtViewNameClassName}`).val("");
            // Quitar el posible fichero adjunto
            $(e.currentTarget).find(".gdd-delete").trigger("click");
            that.filaVista = undefined;                 
        });

        // Botón para Sobreescribir la vista original (fila)                        
        configEventByClassName(`${that.btnUploadOriginalVistaClassName}`, function(element){
            const $jqueryElement = $(element);
            $jqueryElement.off().on("click", function(){                 
                that.filaVista = $(this).closest(`.${that.filaVistaClassName}`);
            });	                        
        });  

        // Botón para Sobreescribir la vista personalizada (fila)                        
        configEventByClassName(`${that.btnUploadEditedVistaClassName}`, function(element){
            const $jqueryElement = $(element);
            $jqueryElement.off().on("click", function(){                 
                that.filaVista = $(this).closest(`.${that.filaVistaClassName}`);
            });	                        
        });          
                
        // Botón para subir una vista y sobreescribirla
        configEventByClassName(`${that.btnSaveUploadViewClassName}`, function(element){
            const $jqueryElement = $(element);
            $jqueryElement.off().on("click", function(){  
                const modal = $(this).closest(`.${that.modalUploadViewClassName}`);                
                that.handleSaveUploadView(modal);
            });	                        
        });

        // Botón para descargar una vista
        configEventByClassName(`${that.btnDownloadOriginalVistaClassName}`, function(element){
            const $jqueryElement = $(element);
            $jqueryElement.off().on("click", function(){                 
                that.filaVista = $(this).closest(`.${that.filaVistaClassName}`);
                that.handleDownloadView($jqueryElement);
            });	                        
        });

        // Botón para descargar una vista personalizada
        configEventByClassName(`${that.btnDownloadEditedVistaClassName}`, function(element){
            const $jqueryElement = $(element);
            $jqueryElement.off().on("click", function(){                 
                that.filaVista = $(this).closest(`.${that.filaVistaClassName}`);
                that.handleDownloadView($jqueryElement);
            });	                        
        });            
        
        // Botón para Eliminar una vista (No CMS)
        configEventByClassName(`${that.btnDeletePersonalizacionClassName}`, function(element){
            const $jqueryElement = $(element);
            $jqueryElement.off().on("click", function(){                 
                that.filaVista = $(this).closest(`.${that.filaVistaClassName}`);
                that.filaVista.addClass("deleted");
            });	                        
        });     

        // Botón para eliminar una vista - Marca en la variable la fila seleccionada
        configEventByClassName(`${that.btnDeletePersonalizacionCMSClassName}`, function(element){
            const $jqueryElement = $(element);
            $jqueryElement.off().on("click", function(){                 
                that.filaVista = $(this).closest(`.${that.filaVistaClassName}`); 
                // Añadir clase de "deleted" para marcarla como "borrado"
                that.filaVista.addClass("deleted");
            });	                        
        });           
        
        // Botón para que abrirá el modal para editar o añadir una nueva Vista CMS
        configEventByClassName(`${that.btnUploadPersonalizacionCMSClassName}`, function(element){
            const $jqueryElement = $(element);
            $jqueryElement.off().on("click", function(){                 
                that.filaVista = $(this).closest(`.${that.filaVistaClassName}`);
            });	                        
        });  

        // Botón para compartir personalizacion de vistas en un dominio
        configEventByClassName(`${that.btnSaveSharedDomains}`, function (element) {
            const $jqueryElement = $(element);
            $jqueryElement.off().on("click", function () {
                const dominio = that.modalShareDomain.find(`.${that.txtDomainName}`).val().trim();
                const urlPeticion = that.modalShareDomain.find(`.${that.btnSaveSharedDomains}`).data("url");
                that.handleShareDomain(that.modalShareDomain, dominio, urlPeticion);
            });	 
        });
        // Botón para dejar de compartir la personalización de vistas en un dominio
        configEventByClassName(`${that.btnStopSharing}`, function (element) {
            const $jqueryElement = $(element);
            $jqueryElement.off().on("click", function () {
                const urlPeticion = that.modalShareDomain.find(`.${that.btnStopSharing}`).data("url");
                that.handleStopSharing(that.modalShareDomain, urlPeticion);
            });
        });

        // Botón para guardar una nueva vista o editar una vista personalizada de tipo CMS        
        configEventByClassName(`${that.btnSaveUploadPersonalizacionCMSClassName}`, function(element){
            const $jqueryElement = $(element);
            $jqueryElement.off().on("click", function(){                 
                // Botón para extraer los datos/parámetros para enviar a backend. Puede ser llamado desde una fila o no. (Recursos)
                let buttonEditView = undefined;
                // Indica crear una personalización desde Head sólo si se crea desde el header. Por defecto "false"
                let createNewCustomizationComponent = false;
                if (that.filaVista == undefined){
                    // Creación desde Header                    
                    buttonEditView = that.triggerModalContainer;
                    createNewCustomizationComponent = true;
                }else{
                    buttonEditView = that.filaVista.find(`.${that.btnUploadPersonalizacionCMSClassName}`);
                }                            

                const accion = buttonEditView.data("action"); 
                const url = buttonEditView.data("url");
                const componenteCms = buttonEditView.data("componentecms");
                const idPersonalizacion = buttonEditView.data("personalizacion");            
                const modal = $(this).closest(`.${that.modalUploadViewClassName}`);                                        
                // Asignar bien el nombre si la vista es de reciente creacion (Añadir personalización)
                const nombreFichero = modal.find(`.${that.txtViewNameClassName}`).val().trim();
                that.handleSaveUploadViewCMS(accion, url, componenteCms, idPersonalizacion, nombreFichero, modal, false, createNewCustomizationComponent);                
            });	                        
        });        
        
        // Click para confirmar la eliminación de una vista personalizada (Ej: Componente CMS personalizado)        
        configEventByClassName(`${that.btnConfirmDeleteClassName}`, function(element){
            const $jqueryElement = $(element);
            $jqueryElement.off().on("click", function(){                 
                // Confirmar la eliminación de la vista
                that.handleDeleteView();
            });	                        
        });          

        // Botón para invalidar vistas
        this.btnInvalidarVistas.on("click", function(){
            that.handleInvalidateViews();
        });

        // Click para elegir una plantilla de tipo "Servicios" (Todas, Web, Resultados, Facetas, Componentes CMS)
        $(`.${this.seleccionFacetaPlantillaClassName}`).on("click", function(){
            const faceta = $(this);
            const tipoServicio = faceta.data("name");
            // Mostrar u ocultar Facetas por "Plantillas de Componentes"
            tipoServicio == "componentecms" && !faceta.hasClass("applied") ? that.panelFacetasComponentesCMS.removeClass("d-none") : that.panelFacetasComponentesCMS.addClass("d-none");
            that.handleAddOrRemoveFacetItemAsTag(faceta);                
        });  

        // Click para elegir el tipo de vistas/plantillas a mostrar (Todas, Originales, Personalizadas)
        $(`.${this.seleccionFacetaTipoPlantillaClassName}`).on("click", function(){
            const faceta = $(this);
            that.handleAddOrRemoveFacetItemAsTag(faceta);
        });

        // Click para elegir una u otra vista a ser mostrada de subtipo CMS (Html Libre ...)
        $(`.${this.seleccionFacetaPlantillaComponenteCMSClassName}`).on("click", function(){
            const faceta = $(this);            
            that.handleAddOrRemoveFacetItemAsTag(faceta);
            that.handleAddPersonalizacionTypeRecurso(faceta);
        });
        
        // Click en el botón de cancelar o eliminar tag/faceta
        configEventByClassName(`${that.btnRemoveFacetaClassName}`, function(element){
            const $jqueryElement = $(element);
            $jqueryElement.off().on("click", function(){                 
                // Eliminar la faceta seleccionada
                const facetaItem = $jqueryElement.parent("li");
                const tipoServicio = facetaItem.data("name");
                // Mostrar u ocultar Facetas por "Plantillas de Componentes"
                tipoServicio == "componentecms" && that.panelFacetasComponentesCMS.addClass("d-none");
                that.handleRemoveFacetaTag(facetaItem);
            });	                        
        });                         
                   
        // Búsquedas de facetas
        this.txtBuscarVistas.off().on("keyup", function (event) {
            const input = $(this);
            clearTimeout(that.timer);
            that.timer = setTimeout(function () {                                                                        
                // Acción de buscar / filtrar
                that.handleSearchVista(input);                                         
            }, 500);
        });      
    },


    /**
     * Método para buscar Facetas por Tags activos según la selección realizada por el usuario
     */
    handleSearchFacetaByActiveTags: function(){
        const that = this;

        // Guardar en un array la selección existente de filtros
        const filters = this.panListadoFiltros.children();
        // Vistas existentes
        const rowsVistas = this.panResultados.find(".row-vista");
        //let activeFilters = [];
        // Array de Nombre del servicio (Web, Resultados, Facetas, Componentes CMS, Objetos de conocimiento)
        let filterServiceName = [];        
        // Array de filtro de tipo (Originales, Personalizadas)
        let filterType = [];
        // Array de filtro de tipo de CMS 
        let filterCmsType = [];

        // Comprobar que haya al menos algún filtro, si no, mostrar todo
        if (filters.length == 0){
            rowsVistas.removeClass("d-none");
            // Actualizar contadores
            that.handleCheckNumberOfVistas();
            return;
        }

        filters.each(function(index, element) {           
           const filter = {
                tag : $(element).data("faceta-tag"),
                // Filtro de Plantillas de recursos 
                faceta : $(element).data("name") != undefined ? $(element).data("name") : "",
                // Filtro de Tipo de Plantillas
                filterType : $(element).data("type") != undefined ? $(element).data("type") : "",
                // Filtro de Tipo de plantillas CMS (Html Libre, Destacado...)
                filterCmsType: $(element).data("view-type") != undefined ? $(element).data("view-type") : "",                 
            }
           filter.faceta.length > 0 && filterServiceName.push(filter.faceta);
           filter.filterType.length > 0 && filterType.push(filter.filterType);
           filter.filterCmsType.length > 0 && filterCmsType.push(filter.filterCmsType);
        });
           
        rowsVistas.each(function(index, element) {           
            const row = $(element);
            // Resetear/Mostrar la vista
            row.removeClass("d-none");
            
            // Filtrar por Servicio (Web, Resultados, Facetas, Componentes CMS, Objetos de Conocimiento)
            if (filterServiceName.length > 0){
                if ( !(filterServiceName.indexOf(row.data("viewservicetype")) > -1) ){
                    // No Se ha encontrado
                    row.addClass("d-none");                                                                
                }
            }            

            // Filtrar por Tipo (Original, Personalizada)"
            if (filterType.length > 0){
                if ( !(filterType.indexOf(row.data("viewtype")) > -1) ){
                    // No se encuentra -> Ocultar
                    row.addClass("d-none"); 
                }             
            } 
            
            // Filtrar por tipo de Componente CMS             
            if (filterCmsType.length > 0){
                if ( !(filterCmsType.indexOf(row.data("parent")) > -1) ){
                    // No se encuentra -> Ocultar
                    row.addClass("d-none"); 
                }             
            }        
         });        

         // Actualizar contadores
         that.handleCheckNumberOfVistas();
    },


    /**
     * Añadir o eliminar faceta si se hace click sobre ella. Añadirá la faceta a modo de tag
     */
    handleAddOrRemoveFacetItemAsTag: function(faceta){
        const that = this;
        const facetaName = faceta.find(".textoFaceta") != undefined ? faceta.find(".textoFaceta").html() : "";
        const facetaDataName = faceta.data("name") != undefined ? faceta.data("name").trim() : "";
        const facetaDataType = faceta.data("type") != undefined ? faceta.data("type").trim() : "";
        const facetaDataViewType = faceta.data("view-type")!= undefined ? faceta.data("view-type").trim() : "";
                        
        // Desactivar botones de Personalización de recursos
        $(`.${that.btnAniadirPersonalizacionRecursosClassName}`).addClass("d-none");

        let isFacetaActive = false;
        this.panListadoFiltros.find(".faceta-name").each(function( index, element ) {    
            if ($(element).html().trim().indexOf(facetaName) > -1){
                isFacetaActive = true;
                return
            }
        });



        if (!isFacetaActive){

            // Permitir solo una faceta por "Servicio", "Tipo" y "TipoCMS"
            // Filtros activos por Servicio y eliminar si existen previos
            if (facetaDataName != ""){
                const filterByServiceActive = $('.activeFilter:not([data-name=""])');
                filterByServiceActive.length > 0 && that.handleRemoveFacetaTag(filterByServiceActive); 
            }
            
            // Filtros activos por Tipo y eliminar si existen previos
            if (facetaDataType != ""){
                const filterByTypeActive = $('.activeFilter:not([data-type=""])');
                filterByTypeActive.length > 0 && that.handleRemoveFacetaTag(filterByTypeActive); 
            }  
            

            // Filtros activos por TipoCMS y eliminar si existen previos
            if (facetaDataViewType != ""){
                const filterByTypeCMSActive = $('.activeFilter:not([data-view-type=""])');
                filterByTypeCMSActive.length > 0 && that.handleRemoveFacetaTag(filterByTypeCMSActive); 
            }             
                        
            // Añadir faceta para filtrado
            // Plantilla de la faceta
            const facetaTemplate = `
                <li class="activeFilter" data-faceta-tag="${facetaName}" data-name="${facetaDataName}" data-type="${facetaDataType}" data-view-type="${facetaDataViewType}">
                    <span class="faceta-name">${facetaName}</span>
                    <a rel="nofollow" class="remove faceta remove-faceta" name="${facetaName}" href="javascript: void();">eliminar</a>
                </li>
            `;
            // Añadirlo como activa
            faceta.addClass("applied");
            // Añadir la faceta al contenedor de facetas
            this.panListadoFiltros.append(facetaTemplate);             

        }else{
            // Eliminar faceta del filtrado  
            const tagFaceta = $(`*[data-faceta-tag="${facetaName}"]`);
            that.handleRemoveFacetaTag(tagFaceta);            
        }        
        that.handleManageSelectActiveFacetas();
    },


    /**
     * Método para eliminar una faceta de la sección "Tags"
     * @param {*} faceta 
     */
    handleRemoveFacetaTag: function(faceta){    
        const that = this;    
        // Buscar la faceta de "Facetas" para desactivar su "applied"
        const facetaName = faceta.data("faceta-tag").trim();
        // Faceta disponible en el panel lateral de "Facetas"        
        $(".col-facetas").find(`.textoFaceta`).each(function( index, element ) {    
            if ($(element).html().trim().indexOf(facetaName) > -1){
                $(this).parent().removeClass("applied");
                return
            }
        });
        faceta.remove(); 
        that.handleManageSelectActiveFacetas();       
    },

    /**
     * Método para mostrar u ocultar las vistas en base a la selección de facetas 
     */
    handleManageSelectActiveFacetas: function(){
        const that = this;

        // Mostrar u ocultar las facetas activas
        this.panListadoFiltros.children().length > 0 ? this.panListadoFiltros.parent().removeClass("d-none") : this.panListadoFiltros.parent().addClass("d-none");

        // Filtrar facetas según los filtros activos
        that.handleSearchFacetaByActiveTags();
    },


    /**
     * Método para mostrar u ocultar el botón correspondiente para añadir una personalización de una vista CMS de tipo Recurso, Tipo de recurso, Grupo de recursos.     
     * @param {*} faceta Si la faceta es undefined, no mostrará ningún botón de personalización, solo los ocultará.
     */
    handleAddPersonalizacionTypeRecurso: function (faceta = undefined){
        const that = this;

        // Ocultar todos los botones de "Personalización"
        Array.from($(`.${this.btnAniadirPersonalizacionRecursosClassName}`)).forEach(item => {                
            const button = $(item);
            button.addClass("d-none");            
        });

        // No continuar si no hay faceta "valida" para mostrar el botón de "Personalización"
        if (faceta == undefined || !faceta.hasClass('applied')){return;}

        let btnAddPersonalization = undefined;

        // Mostrar el botón válido
        const idFaceta = faceta.attr("id");
        switch (idFaceta) {
            case that.seleccionFacetaPlantillaComponenteCmsRecursoId:                                
                btnAddPersonalization = that.btnAniadirPersonalizacionRecurso;
                break;
            case that.seleccionFacetaPlantillaComponenteCmsListadoDeRecursosId:                
                btnAddPersonalization = that.btnAniadirPersonalizacionListadoRecurso;
                break;    
            case that.seleccionFacetaPlantillaComponenteCmsGrupoComponentesId:                
                btnAddPersonalization = that.btnAniadirPersonalizacionGrupoRecurso;
                break;                                     
            default:
                break;
        }

        // Mostrar el botón
        if (btnAddPersonalization != undefined) {
            btnAddPersonalization.removeClass("d-none");
        
            // Asignar el guid al botón
            const randomGuid = guidGenerator().replaceAll("-","");
            btnAddPersonalization.data("personalizacion", randomGuid);
        }
        
    },
   
    /**
     * Método para contabilidad el nº de items existentes
     */
    handleCheckNumberOfVistas: function(){        
        const that = this;


        // Actualizar el nº de items según el tipo "Originales, Personalizadas" teniendo el cuenta el filtro existente si lo hay
        const typeServiceActiveFilter = $('.activeFilter:not([data-type=""])'); 
        // Nº de vistas de tipo Web, Facetas, Resultados teniendo en cuenta el tipo de filtrado (Originales / Personalizadas)
        const numberWebViews = typeServiceActiveFilter.length > 0 ? $(`.${this.filaVistaClassName}[data-viewservicetype='web'][data-viewtype='${typeServiceActiveFilter.data("type")}']`).length : $(`.${this.filaVistaClassName}[data-viewservicetype='web']`).length;
        const numberFacetasViews = typeServiceActiveFilter.length > 0 ? $(`.${this.filaVistaClassName}[data-viewservicetype='facetas'][data-viewtype='${typeServiceActiveFilter.data("type")}']`).length : $(`.${this.filaVistaClassName}[data-viewservicetype='facetas']`).length;
        const numberResultadosViews = typeServiceActiveFilter.length > 0 ? $(`.${this.filaVistaClassName}[data-viewservicetype='resultados'][data-viewtype='${typeServiceActiveFilter.data("type")}']`).length : $(`.${this.filaVistaClassName}[data-viewservicetype='resultados']`).length;
        const numComponentesCMS = typeServiceActiveFilter.length > 0 ? $(`.${this.filaVistaClassName}[data-viewservicetype='componentecms'][data-viewtype='${typeServiceActiveFilter.data("type")}']`).length : $(`.${this.filaVistaClassName}[data-viewservicetype='componentecms']`).length;
        const numObjetoConocimiento = typeServiceActiveFilter.length > 0 ? $(`.${this.filaVistaClassName}[data-viewservicetype='objetoconocimiento'][data-viewtype='${typeServiceActiveFilter.data("type")}']`).length : $(`.${this.filaVistaClassName}[data-viewservicetype='objetoconocimiento']`).length;

        let numberResultadosComponentesCMSSubtipos = 0;
            
        // Asignar los resultados        
        this.numViewWeb.html(`(${numberWebViews})`);
        this.numViewResultados.html(`(${numberResultadosViews})`);
        this.numViewFacetas.html(`(${numberFacetasViews})`);
        this.numViewObjetoConocimiento.html(`(${numObjetoConocimiento})`);
        
        
        // Contabilizar el nº de plantillas de componentes CMS        
        Array.from( $(`.${that.seleccionFacetaPlantillaComponenteCMSClassName}`) ).forEach(facetaItem => {                
            const faceta = $(facetaItem);
            // Tipo de elementos a buscar para contabilizar
            const cmsType = faceta.data("view-type") != undefined ? faceta.data("view-type").trim() : "_sin_cms_type";
            // Nº de items encontrados
            // const numberViews = $(`[data-parent='${cmsType}']`).length : $(`[data-parent='${cmsType}']`).not(".d-none").length;            
            const numberViews = typeServiceActiveFilter.length > 0 ? $(`.${this.filaVistaClassName}[data-parent='${cmsType}'][data-viewtype='${typeServiceActiveFilter.data("type")}']`).length : $(`.${this.filaVistaClassName}[data-parent='${cmsType}']`).length;
            // Ir controlando el total de Resultados de Componentes CMS
            numberResultadosComponentesCMSSubtipos += numberViews;
            // Asignar el valor al numbeOfItems de la faceta correspondiente
            faceta.find(that.numView).html(`(${numberViews})`);
            // Ocultar la faceta de Tipo de CMS si no hay resultados
            // numberViews == 0 ? faceta.addClass("d-none") : faceta.removeClass("d-none");                        
        }); 
        
        // Asignar totales de Componentes CMS + Subtipo de Componentes CMS (Html Libre ...)                
        this.numViewComponentesCMS.html(`(${numComponentesCMS})`);
        // Nº de vistas de tipo de filtrado (Originales / Personalizadas) teniendo en cuenta el Servicio
        const serviceActiveFilter = $('.activeFilter:not([data-name=""])');
        // Actualizar el nº de items según el tipo "Originales, Personalizadas" teniendo el cuenta el filtro existente si lo hay        
        const numberWebTypeOriginalViews =  serviceActiveFilter.length > 0 ? $(`.${this.filaVistaClassName}[data-viewtype='original'][data-viewservicetype='${serviceActiveFilter.data("name")}']`).length : $(`.${this.filaVistaClassName}[data-viewtype='original'] `).length;
        const numberWebTypePersonalizaViews = serviceActiveFilter.length > 0 ? $(`.${this.filaVistaClassName}[data-viewtype='personalizada'][data-viewservicetype='${serviceActiveFilter.data("name")}']`).length : $(`.${this.filaVistaClassName}[data-viewtype='personalizada'] `).length;
        const numberTotalViews = $(`.${that.filaVistaClassName}`).not(".d-none").length;

        // Asignar contador según el tipo de vista        
        this.numViewTypeOriginal.html(`(${numberWebTypeOriginalViews})`);
        this.numViewTypePersonalizadas.html(`(${numberWebTypePersonalizaViews})`);        
        
        // Mostrar el nº de items total que se están visualizando                
        that.numVistasTotal.html(numberTotalViews); 
    },

    /**
     * Método que devolverá el id de la lista correspondiente de vistas para que se añadan los resultados obtenidos devueltos por backend
     * cuando se añadan o elimen nuevas vistas o personalizaciones (web, resultados, facetas, componentesCMS)     
     * @param {*} response : Layout de items que devolverá backend con la nueva lista de vistas actualizada. Será la que habrá que analizar para saber
     * que tipo de vistas se están devolviendo. Esta respuesta se añadirá a la lista correspondiente
     */
    updateListOfViewsByResponse: function(response){
        // Id de las vistas que se devolverá dependiendo de las vistas actualizadas
        let addedVistasList = undefined;

        // Crear contenedor e insertar la respuesta 
        const parentDiv = createElementFromHTML(response);
        // Buscar el primer item de elementos (data-viewservicetype) para saber qué tipo de vistas se han traido
        const firstViewElement = $(parentDiv).find("li.row-vista").first();
        // Conocer el tipo de servicio o vistas editadas
        const firstViewServiceType = firstViewElement.data("viewservicetype");

        // Identificar el ul o listas a actualizar con la respuesta de backEnd
        switch (firstViewServiceType) {
            case "web":
                addedVistasList = this.vistaListWeb;
                break;
            case "resultados": 
                addedVistasList = this.vistaListResultados;               
                break;          
            case "facetas": 
                addedVistasList = this.vistaListFacetas;               
                break;  
            case "componentecms":            
                addedVistasList = this.vistaListComponentesCMS;  
                break;                        
            default:
                break;
        }               
        addedVistasList != undefined && addedVistasList.html(response);        
    },

    
    /**
     * Método para sobreescribir una vista original. 
     * @param {*} modal Modal utilizado para la subida de la vista
     */
     handleSaveUploadView: function(modal){            
        const that = this;

        // Objeto a enviar a backend con los datos
        const dataPost = new FormData();

        const currentButtonSaveUploadView = that.filaVista.find(`.${this.btnSaveUploadViewClassName}`);
        const currentValueView = that.filaVista.find(`.${this.valueViewClassName}`);
        const currentDragAndDropInputFile = that.filaVista.find(`.${this.dragAndDropInputFileClassName}`);
        const isSemanticForm = that.filaVista.data("semanticform");

        // Obtener el key "Páginas Personalizables" | "Formularios Semánticos"
        const keyPaginasPersonalizables = currentButtonSaveUploadView.data("personalizables");
        const keyFormulariosSemanticos = currentButtonSaveUploadView.data("formulariossemanticos");  
        
        // Faceta a filtrar para mostrar los items una vez se cargen nuevos
        const facetaFilterItem = $(`.${that.listadoFacetasCmsClassName}`).find(".applied");
        // Detectar si el filtrado por CMS está activo
        const isComponentCmsFacetType = facetaFilterItem.hasClass("seleccionFacetaPlantillaComponenteCMS");        

        // Nombre del fichero de la vista
        const nombreFichero = currentValueView.html().trim();

        // const files = $(this).get(0).files;
        // Obtener el fichero que se ha seleccionado
        const file = currentDragAndDropInputFile.prop("files")[0];

        // Añadir el fichero seleccionado para su envío
        if (file != undefined) {            
            dataPost.append(currentDragAndDropInputFile.attr('name'), file);
        } else {            
            dataPost.append(currentDragAndDropInputFile.attr('name'), null);
        }    
                
        // Construir el objeto
        if (isSemanticForm != undefined){            
            // Añadir key de vista + nombre de la vista a actualizar
            dataPost.append(keyPaginasPersonalizables, '');
            // Añadir key de formulario semántico vacío
            dataPost.append(keyFormulariosSemanticos, nombreFichero);            
        }else{
            // Añadir key de vista + nombre de la vista a actualizar
            dataPost.append(keyPaginasPersonalizables, nombreFichero);
            // Añadir key de formulario semántico vacío
            dataPost.append(keyFormulariosSemanticos, '');
        }
    
        // Petición a realizar
        const urlAction = currentButtonSaveUploadView.data("url");
        // Tipo de acción
        const action = currentButtonSaveUploadView.data("action");
        // Añadir el tipo de acción al objeto dataPost
        dataPost.append("Accion", action);    
        
        loadingMostrar();

        // Realizar petición
        GnossPeticionAjax(
            urlAction,
            dataPost,
            true
        ).done(function (response) {
            if (action == 1 || action == 3) {
                // Descargar el fichero
                that.handleDescargarArchivo(response, nombreFichero);
            } else {                                                                         
                // Detectar la lista de componentes y actualizarla según la respuesta de vistas
                that.updateListOfViewsByResponse(response); 
                // Dejar marcada el listado de items activa                
                ////////that.handleSelectFaceta(facetaFilterItem, isComponentCmsFacetType);
                // Mostrar las vistas según los filtros
                that.handleSearchFacetaByActiveTags();
                // Cerrar el modal y esperar 1 segundo para mostrar el mensaje de OK
                dismissVistaModal(modal);                         
                setTimeout(function() {                
                    mostrarNotificacion("success", "Los cambios se han guardado correctamente");                                
                    that.triggerEvents();
                },1000); 
            }    

        }).fail(function (response) {            
            mostrarNotificacion("error", response);
        }).always(function () {
            // Ocultar loading de la petición
            loadingOcultar();
        });            
    },

    
    /**
     * Método para guardar una nueva personalización o editar una vista de tipo CMS 
     * @param {short} accion : Tipo de acción a realizar
     * @param {*} url : Url a la que se realizará la acción
     * @param {*} componenteCms : Key o identificador del componente al que se aplicará la acción
     * @param {*} idPersonalizacion : Id del componente al que se realizará la acción
     * @param {*} nombre : Nombre del fichero de la vista. Es posible que sea vacío (Si se edita la vista original)
     * @param {jqueryElement} modal : Vista modal que está abierta
     * @param {bool} isOriginalView : Indica si la vista a subir/editar es la original o una vista personalizada
     * @param {bool} isNewPersonalizacion : Indica si la vista a subir/editar es una personalización de tipo Recurso, Listado de Recursos y Grupo de Componentes. Deberá ser siempre 000-000...
     */
    handleSaveUploadViewCMS: function(accion, url, componenteCms, idPersonalizacion, nombre, modal, isOriginalView, isNewPersonalizacion = false){
        const that = this;
        
        // Construir el objeto para proceder a la eliminación de la vista
        const dataPost = new FormData();        
        
        // Url para petición
        const urlAction = url;

        // Obtener el key "Páginas Personalizables" | "Formularios Semánticos"
        const keyComponentePersonalizable = componenteCms;
        const keyPersonalizacion = "idPersonalizacion";
        // Value de la personalización del componente CMS
        let valueIdPersonalizacion = idPersonalizacion;
        const keyNombre = "Nombre";              
        // Nombre del fichero de la vista
        const nombreFichero = nombre.trim();     
        // Nombre de la vista a "editar"
        let currentValueView = undefined;
        if (that.filaVista != undefined){
            currentValueView = that.filaVista.find(`.${this.valueViewInRowClassName}`).html().trim();   
        }else{
            // Obtener el nombre de la vista para la personalización de un CMS de tipo Articulo
            const firstView = $(`.${that.componentListClassName}`).not(".d-none").children("li").not(".d-none").first();
            const viewName = firstView.data("defaultviewname");                              
            currentValueView = viewName;                                       
            valueIdPersonalizacion = !isNewPersonalizacion ? firstView.data("personalizacion") : "00000000-0000-0000-0000-000000000000";
        }
        // Añadir datos adicionales        
        // Tipo de acción
        const action = accion;
        
        // Añadir el fichero   
        const currentDragAndDropInputFile = modal.find(`.${this.dragAndDropInputFileClassName}`);
        // Obtener el fichero que se ha seleccionado
        const file = currentDragAndDropInputFile.prop("files")[0];                
        dataPost.append("Fichero", file);        

        // Añadir el tipo de acción al objeto dataPost
        dataPost.append("Accion", action);     
        // Añadir key de vista + nombre de la vista a actualizar de tipo CMS
        dataPost.append(keyComponentePersonalizable, currentValueView);            
        // Añadir el id de la personalización
        dataPost.append(keyPersonalizacion, valueIdPersonalizacion);
        // Añadir el nombre del fichero si procede (Solo para CMS personalizados)
        dataPost.append(keyNombre, nombreFichero);   
        
        // Faceta a filtrar para mostrar los items una vez se cargen nuevos
        const facetaFilterItem = $(`.${that.listadoFacetasCmsClassName}`).find(".applied");
        // Detectar si el filtrado por CMS está activo
        const isComponentCmsFacetType = facetaFilterItem.hasClass("seleccionFacetaPlantillaComponenteCMS");
                
        if (file == undefined){
            mostrarNotificacion("error", "Es necesario seleccionar un fichero para continuar."); 
            return;
        }

        if (nombreFichero == "" || nombreFichero.length == 0){
            mostrarNotificacion("error", "El nombre de la vista no puede estar vacío."); 
            return;
        }

        // Datos extra del componente si procede
        const panelDatosExtra = modal.find(`.${this.panelDatosExtraClassName}`);
        if (panelDatosExtra.length > 0)
        {
            // Recoger los datos extra                    
            Array.from(  panelDatosExtra.find($('input[type=checkbox]')) ).forEach(item => {                
                const checkbox = $(item);
                dataPost.append(checkbox.attr('name'), checkbox.is(':checked'));    
            });            
        }

        loadingMostrar();
        // Realizar petición
        GnossPeticionAjax(
            urlAction,
            dataPost,
            true
        ).done(function (response) {           
            // OK                        
            if (accion != 1 || accion != 3){    
                // Detectar la lista de componentes y actualizarla según la respuesta de vistas
                that.updateListOfViewsByResponse(response);             
                // Dejar marcada el listado de items activa                
                //////////////that.handleSelectFaceta(facetaFilterItem, isComponentCmsFacetType);
                // Mostrar las vistas según los filtros
                that.handleSearchFacetaByActiveTags();                
            }
            // Cerrar el modal y esperar 1 segundo para mostrar el mensaje de OK
            dismissVistaModal(modal);                         
            setTimeout(function() {                
                mostrarNotificacion("success", "Los cambios se han guardado correctamente");                                
                that.triggerEvents();
            },1000); 
        }).fail(function (response) {            
            // KO
            mostrarNotificacion("error", "Se ha producido un error al tratar de guardar los cambios. Contacta con el administrador");
        }).always(function () {
            // Ocultar loading de la petición
            loadingOcultar();
        });        

    },    

    
    /**
     * Método para descargar una vista.
     * @param {*} downloadButton Botón usado para la descarga. Puede descargarse la vista original o la vista personalizada.
     */
     handleDownloadView: function(downloadButton){            
        const that = this;

        // Objeto a enviar a backend con los datos
        const dataPost = new FormData();

        const currentButtonDownloadView = downloadButton// that.filaVista.find(`.${this.btnDownloadOriginalVistaClassName}`);

        const currentValueView = that.filaVista.find(`.${this.valueViewInRowClassName}`);        
        const isSemanticForm = that.filaVista.data("semanticform");
        const isComponenteCMS = that.filaVista.data("componentecms");

        // Obtener el key "Páginas Personalizables" | "Formularios Semánticos"
        const keyPaginasPersonalizables = currentButtonDownloadView.data("personalizables");
        const keyFormulariosSemanticos = currentButtonDownloadView.data("formulariossemanticos");
        const keyComponentePersonalizable = currentButtonDownloadView.data("componentecms");
        const keyPersonalizacion = "idPersonalizacion";
        // Value de la personalización del componente CMS
        const valueIdPersonalizacion = currentButtonDownloadView.data("personalizacion");
        const keyNombre = "Nombre";
        // Nombre del fichero para componente cms personalizado
        let valueNombre = currentButtonDownloadView.data("nombre");

        // Nombre del fichero de la vista
        const nombreFichero = currentValueView.html().trim();

        // Añadir el fichero como nulo ya que se desea descargar no subir       
        dataPost.append("Fichero", null);        
                
        // Construir el objeto
        if (isSemanticForm != undefined){  
            // Vista de tipo semántica          
            // Añadir key de vista + nombre de la vista a actualizar
            dataPost.append(keyPaginasPersonalizables, '');
            // Añadir key de formulario semántico vacío
            dataPost.append(keyFormulariosSemanticos, nombreFichero);            
        }else if(isComponenteCMS != undefined){
            // Añadir key de vista + nombre de la vista a actualizar de tipo CMS
            dataPost.append(keyComponentePersonalizable, nombreFichero);            
            // Añadir el id de la personalización
            dataPost.append(keyPersonalizacion, valueIdPersonalizacion);
            // Añadir el nombre del fichero si procede (Solo para CMS personalizados)
            dataPost.append(keyNombre, valueNombre);
        }
        else{
            // Añadir key de vista + nombre de la vista a actualizar
            dataPost.append(keyPaginasPersonalizables, nombreFichero);
            // Añadir key de formulario semántico vacío
            dataPost.append(keyFormulariosSemanticos, '');
        }
    
        // Petición a realizar
        const urlAction = currentButtonDownloadView.data("url");
        // Tipo de acción
        const action = currentButtonDownloadView.data("action");
        // Añadir el tipo de acción al objeto dataPost
        dataPost.append("Accion", action);    
        
        loadingMostrar();

        // Realizar petición
        GnossPeticionAjax(
            urlAction,
            dataPost,
            true
        ).done(function (response) {
            if (action == 1 || action == 3) {
                // Descargar el fichero                
                that.handleDescargarArchivo(response, nombreFichero);
            } else {                
                // Respuesta de la subida del fichero
                mostrarNotificacion("success",response);                            
            }    

        }).fail(function (response) {            
            mostrarNotificacion("error", response);
        }).always(function () {
            // Ocultar loading de la petición
            loadingOcultar();
        });            
    },  
    
    /**
     * Método para Descargar una vista una vista original cuando se confirma a través del modal para la eliminación
     */
    handleDeleteView: function(){
        const that = this;

        // Construir el objeto para proceder a la eliminación de la vista
        // Objeto a enviar a backend con los datos
        const dataPost = new FormData();
        let deleteCMSView = true; 

        let currentButtonDeleteView = "";
        // Obtener el borrado correcto de la fila (CMS o Elemento HTML normal)
        if (that.filaVista.find(`.${this.btnDeletePersonalizacionCMSClassName}`).length > 0){
            currentButtonDeleteView = that.filaVista.find(`.${this.btnDeletePersonalizacionCMSClassName}`);
        }else{
            deleteCMSView = false;
            currentButtonDeleteView = that.filaVista.find(`.${this.btnDeletePersonalizacionClassName}`);
        }
        
        // Detectar si es una vista semántica
        const isSemanticForm = that.filaVista.data("semanticform");
        const currentValueView = that.filaVista.find(`.${this.valueViewInRowClassName}`);        
        
        // Url para petición
        const urlAction = currentButtonDeleteView.data("url");

        // Obtener el key "Páginas Personalizables" | "Formularios Semánticos"
        const keyComponentePersonalizable = currentButtonDeleteView.data("componentecms");
        const keyFormulariosSemanticos = currentButtonDeleteView.data("formulariossemanticos");

        const keyPersonalizacion = "idPersonalizacion";
        // Value de la personalización del componente CMS
        const valueIdPersonalizacion = currentButtonDeleteView.data("personalizacion");
        const keyNombre = "Nombre";
        // Nombre del fichero para componente cms personalizado
        let valueNombre = currentButtonDeleteView.data("nombre");

        // Nombre del fichero de la vista
        const nombreFichero = currentValueView.html().trim();

        // Añadir el fichero como nulo ya que se desea descargar no subir       
        dataPost.append("Fichero", null);
        // Añadir datos adicionales        
        // Tipo de acción
        const action = currentButtonDeleteView.data("action");
        // Añadir el tipo de acción al objeto dataPost
        dataPost.append("Accion", action);     
        
        // Detectar si se desea eliminar un formulario semántico o una vista normal
        if (isSemanticForm != undefined){
            dataPost.append(keyFormulariosSemanticos, nombreFichero);
        }else{
            dataPost.append(keyComponentePersonalizable, nombreFichero);
        }
        // Borrar vista CMS
        if (deleteCMSView){
            // Añadir key de vista + nombre de la vista a actualizar de tipo CMS
                       
            // Añadir el id de la personalización
            dataPost.append(keyPersonalizacion, valueIdPersonalizacion);
            // Añadir el nombre del fichero si procede (Solo para CMS personalizados)
            dataPost.append(keyNombre, valueNombre);  
        } 
        
        // Faceta a filtrar para mostrar los items una vez se cargen nuevos
        const facetaFilterItem = $(`.${that.listadoFacetasCmsClassName}`).find(".applied");  
        // Detectar si el filtrado por CMS está activo
        const isComponentCmsFacetType = facetaFilterItem.hasClass("seleccionFacetaPlantillaComponenteCMS");              
        
        loadingMostrar();

        // Realizar petición
        GnossPeticionAjax(
            urlAction,
            dataPost,
            true
        ).done(function (response) {           
            // OK                        
            // Cerrar el modal y esperar 1 segundo para mostrar el mensaje de OK
            dismissVistaModal(that.modalDeleteView);            
            setTimeout(function() {                
                mostrarNotificacion("success", "El elemento se ha borrado correctamente");        
                // Detectar la lista de componentes y actualizarla según la respuesta de vistas
                that.updateListOfViewsByResponse(response); 
                // Dejar marcada el listado de items activa                
                /////////that.handleSelectFaceta(facetaFilterItem, isComponentCmsFacetType);                
                // Mostrar las vistas según los filtros
                that.handleSearchFacetaByActiveTags();                
                that.triggerEvents();
            },1000);              
            
        }).fail(function (response) {            
            // KO
            mostrarNotificacion("error", "Se ha producido un error al tratar de eliminar el elemento. Contacta con el administrador");
        }).always(function () {
            // Ocultar loading de la petición
            loadingOcultar();
        });
    },


    /**
     * Método para descargar un fichero/vista
     * @param {*} texto Respuesta o datos obtenidos para realizar la descarga del fichero
     * @param {*} nombreArchivo : Nombre del fichero a descargar
     */
    handleDescargarArchivo: function(texto, nombreArchivo){        
        if (nombreArchivo.indexOf('.cshtml') == -1) {
            nombreArchivo = nombreArchivo + ".cshtml";
        }
        var textoBlob = [];
        textoBlob.push(texto);
        var contenidoEnBlob = new Blob(textoBlob, {
            encoding: "UTF-8", type: "text/plain;charset=UTF-8"
        });

        var reader = new FileReader();
        reader.onload = function (event) {
            var save = document.createElement('a');
            save.href = event.target.result;
            save.target = '_blank';
            save.download = nombreArchivo || 'archivo.dat';
            var clicEvent = new MouseEvent('click', {
                'view': window,
                'bubbles': true,
                'cancelable': true
            });
            save.dispatchEvent(clicEvent);
            (window.URL || window.webkitURL).revokeObjectURL(save.href);
        };
        reader.readAsDataURL(contenidoEnBlob);               
    },

    /**
     * Método para invalidar vistas
     */
    handleInvalidateViews: function(){
        const that = this;

        // Objeto vacío a enviar para invalidar views
        const dataPost = new FormData();

        // Mostrar loading
        loadingMostrar();

        // Realizar petición
        GnossPeticionAjax(
            that.urlInvalidateViews,
            dataPost,
            true
        ).done(function (response) {                        
            // Respuesta de la subida del fichero
            mostrarNotificacion("success", response);                            
        }).fail(function (response) {            
            mostrarNotificacion("error", response);
        }).always(function () {
            // Ocultar loading de la petición
            loadingOcultar();
        });        
    },
    /**
     * Método para compartir la personalización de las vistas en un dominio
     */
    handleShareDomain: function (modal, dominio, url) {
        const dataPost = new FormData();

        dataPost.append("pDominio", dominio);
        // Mostrar loading
        loadingMostrar();

        // Realizar petición
        GnossPeticionAjax(
            url,
            dataPost,
            true
        ).done(function (response) {
            dismissVistaModal(modal);
            mostrarNotificacion("success", response);
            setTimeout(function () {
                window.location.reload();
            }, 1000); 
        }).fail(function (response) {
            mostrarNotificacion("error", response);
        }).always(function () {
            loadingOcultar();
        });       
    },
    /**
     * Método para dejar de compartir la personalización de la vistas en un dominio
     */
    handleStopSharing: function (modal, url) {
        const dataPost = new FormData();
        var listaDominios = [];
        var i = 0;

        Array.from(modal.find($('input[type=checkbox]'))).forEach(item => {
            const checkbox = $(item);
            if (checkbox.is(':checked')) {
                listaDominios[i] = checkbox.val();
                i++;
            }          
        });

        dataPost.append("pDominios", listaDominios);

        // Mostrar loading
        loadingMostrar();

        // Realizar petición
        GnossPeticionAjax(
            url,
            dataPost,
            true
        ).done(function (response) {
            dismissVistaModal(modal);
            mostrarNotificacion("success", response);
            setTimeout(function () {
                window.location.reload();
            }, 1000);
        }).fail(function (response) {
            mostrarNotificacion("error", response);
        }).always(function () {
            loadingOcultar();
        });
    },
    /**
     * Método para realizar búsquedas y filtrados
     * @param {*} input 
     */    
    handleSearchVista: function(input){

        const that = this;
        // Cadena introducida para filtrar/buscar
        let cadena = input.val();
        // Eliminamos posibles tildes para búsqueda ampliada
        cadena = cadena.normalize('NFD').replace(/[\u0300-\u036f]/g, "").toLowerCase().trim();

        if (cadena.length == 0){
            // Aplicar el filtrado actual si hay filtros activos
            that.handleSearchFacetaByActiveTags();
            return;
        }

        // Listado de componentes donde se buscará
        const vistas = $(`.${that.componentListClassName}`).not(".d-none").find($(`.${that.filaVistaClassName}`).not(".d-none"));                       

        // Recorrer los items para realizar la búsqueda de palabras clave
        $.each(vistas, function(index){
            const vistaItem = $(this);
            // Seleccionar el nombre sólo "visible" de la traducción. Quitamos caracteres extraños, tildes para poder hacer bien la búsqueda
            const componentTitulo = vistaItem.find(".component-name").html().trim().normalize("NFD").replace(/[\u0300-\u036f]/g, "").toLowerCase();                        
            const componentSubtitulo = vistaItem.find(".component-name-subtitle").html().trim().normalize("NFD").replace(/[\u0300-\u036f]/g, "").toLowerCase();

            if (componentTitulo.includes(cadena) || componentSubtitulo.includes(cadena)){
                // Mostrar la fila
                vistaItem.removeClass("d-none");                
            }else{
                // Ocultar la fila
                vistaItem.addClass("d-none");
            }            
        }); 
        
        // Ocultar botón de "Crear personalización" para evitar posibles errores
        // const faceta = $(`.${this.seleccionFacetaPlantillaClassName}.applied`);      
        // that.handleSelectFaceta(faceta, $("[data-name='componentescms']").hasClass("applied"), true ); 
        
        // Desactivar botones personalización si estuvieran activos      
        $(`.${that.btnAniadirPersonalizacionRecursosClassName}`).addClass("d-none");
    },
}
