/**
 ***************************************************************************************
 * Logica / Operativas de comportamiento JS para la sección de Estructura, en concreto, el CMS Builder de páginas de la Comunidad del DevTools
 * *************************************************************************************
 */

const operativaGestionCMSBuilder = {

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
     * Método a ejecutar para inicialización de funcionalidad necesaria 
     */
    triggerEvents: function(){
        const that = this;
        // Comprobar si hay columnas sin componentes, que no haya ningún espacio en blanco (En caso afirmativo no se visualiza el texto de ayuda)
        that.handleCheckComponentsListIfEmpty();   
        // Comprobar que no se está en modo "Wizard" y de ser así, mostrar un overlay en la sección de "Diseño"
        that.handleHideDesignAndComponentsPanelIfWizardActive();
        
        const guardadoSeguridadParams = {
            pageId: that.pParams.pageId,
        }
        operativaGestionCMSBuilderGuardadoSeguridad.init(guardadoSeguridadParams);
    },    

    /**
     * Configuración de las rutas de acciones necesarias para hacer peticiones a backend
     */
     configRutas: function (pParams) {
        this.urlBase = refineURL();
        this.urlSaveCMSPage = this.urlBase;
        this.urlSearchComponent = `${this.urlBase}/searchComponent`;
    },
    
    /*
     * Inicializar elementos de la vista
     * */
    config: function (pParams) {

        /* Componentes del Editor del CMS */
        // Editor de la página CMS
        this.cmsEditor = $(".builder-editor");
        // Wizard donde se muestran posibles plantillas para creación de la página CMS
        this.cmsWizard = $(".builder-start");
        // Área donde estarán todas las filas de la página. Aquí es donde se añadirán nuevas
        this.rowList = $("#cmsrow-list");
        // Area donde están los diseños básicos
        this.basicDesigns = $("#disenos-basicos");
        // Area donde están los diseños de columna predefinidos
        this.columnDesigns = $("#default-rows");
        // Panel lateral para elección de componentes y diseños
        this.leftPanelPageBuilder = $("#panel-lateral-page-builder");
        // Botón para añadir más filas a la página CMS
        this.btnAddRow = $("#btnAddRow");
        // Botón de acciones a realizar desde PageBuilder
        this.dropDownActionsCMSBuilder = $("#dropDownActionsCMSBuilder");

        // Botones de guardado de página CMS (Guardar borrador, publicar, descartar)
        this.btnSaveCMSPage = $(`.saveButton`);

        // Input para buscar componentes
        this.txtSearchComponentItem = $(`#filtro-componentes`);
        // Tab de Componentes
        this.componentsTabContent = $('#components-tab-content');
        // Listado de componentes existentes en la comunidad
        this.createdComponentsId = "list-created-components";
        this.createdComponents = $(`#${this.createdComponentsId}`);

        // Listado donde se alojarán los componentes listados (cada lista item)
        this.createdListComponentsId = "created-components";
        this.createdListComponents = $(`#${this.createdListComponentsId}`);

        // Tab de diseños
        this.designTabContent = $('#design-tab-content');

        /* Botones de acción de fila */
        this.btnEditRowClassName = 'btnEditRow';
        this.btnDeleteRowClassName = 'btnDeleteRow';
        this.btnDragRowClassName = 'btnDragRow';
        this.btnCopyRowClassName = 'btnCopyRow';

        /* Botones de acción de componente */        
        this.btnEditComponentFromCMSBuilderClassName = 'btnEditComponentFromCMSBuilder';
        this.btnEditComponentFromCMSBuilder = $(`#${this.btnEditComponentFromCMSBuilderClassName}`);

        // Estructura de la página
        // Filas que conforman la página CMS
        this.rowCmsClassName = 'js-cmsrow';
        // Columnas que conforman la página CMS
        this.columnCmsClassName = 'cmscolumn';
        // Componentes que conforman la página CMS
        this.componentCmsClassName = 'cmscomponent';
        // Área donde están los componentes en "Columnas". Resetearlos de inicio si no hay nada
        this.componentsList = $('.components-list');
        // Cada uno de los Selects que muestran las opciones de un componente (ej: Opción del componente menú que será activo)
        this.propiedadComponenteClassName = "propiedadComponente";

        /* Botones de acción de columna */
        this.btnDeleteColumnClassName = 'js-action-delete-column';

        // Rows pertenecientes a los paneles de "Componentes" y  "Diseños"
        this.componentListItemClassName = 'builder-item';

        // Contenedor modal de contenido dinámico
        this.modalContainer = $("#modal-container");

        // Botones de la vista "wizard" para elegir una plantilla predefinida para páginas CMS
        this.btnSelectTemplate = $(".btnSelectTemplate");

        // Objeto jquery que contiene el nuevo componente arrastrado (cuando se cree uno nuevo)
        this.newItemCreated = undefined;
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
            
            if (operativaGestionComponentsCMS.isNewCreatedFromCSMBuilder == true){
                // Eliminar la fila del componente nuevo añadido ( No se llega a guardar )
                setTimeout(function() {                                        
                    that.newItemCreated.fadeOut("normal", function() {
                        $(this).remove();
                    });
                    that.newItemCreated = undefined;
                },500);
            }      
            
        });

        // Búsquedas de componentes (KeyUp) del panel lateral izquierdo           
        this.txtSearchComponentItem.on("keyup", function (event) {
            const input = $(this);
            clearTimeout(that.timer);
            // Realizar búsqueda vía petición a CORE
            if (input.hasClass("search-fetch-request")){
                that.timer = setTimeout(function () {                                                                        
                    // Acción de buscar / filtrar componente
                    that.handleSearchComponentItemQuery(input);                                         
                }, 500);
            }else{
                // Realizar búsqueda sin peticiones y solo en el listado existente
                that.timer = setTimeout(function () {                                                                        
                    // Acción de buscar / filtrar componente
                    that.handleSearchComponentItem(input);                                         
                }, 500);
            }            
        });
        
        // Botones de guardado de página CMS (Vista previa, Guardar borrador...)
        this.btnSaveCMSPage.off().on("click", function(){
            const button = $(this);
            // Obtener el tipo de acción a realizar
            const action = button.data("action");
            that.handleSaveCMSPage(action);
        });

        // Botón para editar un componente dentro de una columna CMS        
        configEventByClassName(`${that.btnEditComponentFromCMSBuilderClassName}`, function(element){
            const $jqueryElement = $(element);
            $jqueryElement.off().on("click", function(){                                                 
                const editButton = $(this);                                
                that.handleLoadComponentModalForEdit(editButton);             
            });	                        
        });
        
        // Botón para elegir una plantilla predefinida para una página CMS
        this.btnSelectTemplate.off().on("click", function(){
            const button = $(this);
            that.handleSelectCMSTemplateWizard(button);
        });
    }, 


    /**
     * Cargar un overlay en el panel lateral para que no pueda estar activo si el wizard está activo
     */
    handleHideDesignAndComponentsPanelIfWizardActive: function(){
        if (this.cmsWizard.length > 0){
            // Wizard activo, mostrar overlay en el panel lateral
            overlayShowInContainer(this.leftPanelPageBuilder.attr("id"));
        }
    },

    /**
     * 
     * @param {jqueryElement} button Botón elegido para seleccionar una plantilla CMS. 
     */
    handleSelectCMSTemplateWizard: function(button){
        const that = this;
        // Plantilla a elegir
        const template = button.data("template");        

        switch(template) {
            case "template1":
                // code block
                that.addRowsWithColumnsFromWizard("12");
              break;
            case "template2":              
                that.addRowsWithColumnsFromWizard("12");
                that.addRowsWithColumnsFromWizard("8+4");
                that.addRowsWithColumnsFromWizard("4+4+4");              
                break;
            case "template3":
                that.addRowsWithColumnsFromWizard("12");
                that.addRowsWithColumnsFromWizard("6+6");
                that.addRowsWithColumnsFromWizard("4+4+4");                     
            default:
                that.addRowsWithColumnsFromWizard("12");
                break;              
        }
                
        // Mostrar el editor y ocultar el wizard        
        that.cmsWizard.fadeOut("normal", function() {
            // Visualizar el editor de páginas
            that.cmsEditor.removeClass("d-none");  
            // Visualizar las acciones permitidas del CMSBuilder
            that.dropDownActionsCMSBuilder.removeClass("d-none");
            that.cmsWizard.addClass("d-none");
            // Visualizar correctamente el panel lateral de diseños y componentes
            overlayDestroy();
        });
    },

    /**
     * Método que añadirá filas y columnas según el diseño deseado 
     * @param {String} numColumns : Diseño que se desea añadir (12, 6+6, ...)
     */
    addRowsWithColumnsFromWizard: function(numColumns){
        const that = this;

        // Item que se desea añadir de diseños preestablecidos
        const itemToAdd = that.columnDesigns.find($(`*[data-columns="${numColumns}"]`));

        // Añadir el item al editor manualmente
        const itemToAddCloned = itemToAdd.clone()
        itemToAddCloned.appendTo(that.rowList);

        // Información a proporcionar a Sortable para añadir el item de forma manual
        const informationToAdd = {
            item: itemToAddCloned,
            from: that.columnDesigns,
            to: that.rowList,
            clone: itemToAddCloned,
            clones: [],
            isTrusted: false,
            items: [],
            newDraggableIndex: 0,
            newIndex: 0,
            newIndicies: 0,
            oldDraggableIndex: 0,
            oldIndex: 0,
            oldIndicies: 0,
            target: that.rowList,
            type: "add",
        }

        // Añadimos el item proporcionándole la información necesaria
        PageBuilderEdition.rowsEdition.onDefaultRowAdded(informationToAdd);        
    },

    
    /**
     * Método para cargar el modal y poder editar un componente desde el editor de páginas CMS.
     * @param {jqueryElement} editButton Botón de editar componente pulsado
     */
    handleLoadComponentModalForEdit: function(editButton){
        const that = this;
        // Url para editar el componente seleccionado
        that.urlEditComponent = editButton.data("urleditcomponent");
        // Cargar la vista para mostrarla en el contenedor
        getVistaFromUrl(`${that.urlEditComponent}`, 'modal-dinamic-content', '', function(result){
            if (result == requestFinishResult.ok){                    
                // OK al cargar la vista
                // Añadir el estilo de tabs al modal 
                !that.modalContainer.hasClass("modal-con-tabs") && that.modalContainer.addClass("modal-con-tabs");
                // Asignar la url para guardar el componente.
                that.urlSaveComponent = editButton.data("urlsave");
                // Pasar la url para guardar componente a la operativa que gestiona el guardado del componente
                operativaGestionComponentsCMS.urlSaveComponent = that.urlSaveComponent;
                operativaGestionComponentsCMS.urlEditComponent = that.urlEditComponent;
                operativaGestionComponentsCMS.completion = that.onResultComponentSaved;
            }else{
                // KO al cargar la vista
                dismissVistaModal();
                mostrarNotificacion("error", "Error al cargar el componente para su edición");        
            }                
        });                        
    },


    /**
     * Método que se ejecutará cuando se arrastre un NUEVO componente al área de CMS Builder.
     * Se hará la llamada para mostrar el modal para crear el nuevo componente desde el propio CMS Builder.
     * @param {jqueyElement} $item 
     */
    onComponentAddedLoadModal: function($item){        
        const that = this;

         // Obtener las urls de creación (Llamada al modal para creación de un componente) y guardado del componente
         const urlSave = $item.data("urlsave");
         // Url para hacer la llamada al modal para solicitar el modal para su creación
         const url = $item.data("url");
                                                  
         // Url para guardar el componente nuevo
         operativaGestionComponentsCMS.urlSaveComponent = urlSave;         

        // Mostrar el modal donde se cargará la vista del componente nuevo
        that.modalContainer.modal('show');
        // Guardamos en global el nuevo item arrastrado
        that.newItemCreated = $item;
        // Pasarle el completion para que cuando finalice el guardado, se realicen tareas necesarias desde Builder
        operativaGestionComponentsCMS.completion = that.onResultComponentSaved;
        // Indicamos a operativaGestionComponentesCMS que el componente se está creando desde CSMBuilder
        operativaGestionComponentsCMS.isNewCreatedFromCSMBuilder = true;

        // Cargar la vista para crear un nuevo componente
        getVistaFromUrl(url, 'modal-dinamic-content', '', function(result){
            if (result != requestFinishResult.ok){
                // KO al cargar la vista
                mostrarNotificacion("error", "Error al cargar el componente para su creación.");
                dismissVistaModal();                                                     
            }                
        });         
    },

    /**
     * Método a modo de "completion" que se ejecutará cuando se realice el guardado de un componente.
     * El proceso de guardado se realizada desde 'operativaGestionComponentsCMS'. Por lo que para realizar tareas diferentes
     * se gestionará mediante esta función que es devuelta a modo de completion.     
     * @param {Object} requestResult: Indicará si la petición de guardado ha sido correcta o no
     * @param {String} data : Mensaje o data de la respuesta a la petición de guardado. Si se crea un nuevo componente, se dispondrá aquí del id del componente creado.
     */
    onResultComponentSaved: function(requestResult, data = undefined){
        const that = operativaGestionCMSBuilder;

        if (requestResult == requestFinishResult.ok){
            // OK guardado del componente            
            mostrarNotificacion("success", "El componente se ha guardado correctamente");            
            // Ocultar vista modal
            dismissVistaModal();                                                  
            // Cerrar el modal y recargar la página para recargar todos los ficheros            
            setTimeout(function() {
                dismissVistaModal();                            
            }
            ,1000);

            // Indicamos al flag que ya se ha creado para que al cerrar el modal no lo borre del área PageBuilder 
            operativaGestionComponentsCMS.isNewCreatedFromCSMBuilder = false;

            // Si el componente es nuevo (data != undefined)
            if (data != undefined){
                // 1-> Añadir el nuevo componente a la sección de "created-components" para que esté disponible para arrastrar de nuevo si hiciera falta
                const componentCreated = data;            
                that.handleAddNewComponentToCreatedComponent(componentCreated);
                // 2-> Añadir el id y rutas para editar el componente recién creado desde CMSBuilder (a partir del id)
                that.handleAddPropertiesToCreatedComponent(componentCreated);
            }        
        }else{
            // KO guardado del componente
            mostrarNotificacion("error", data);
        }
    },


    /**
     * Método para añadir las propiedades necesarias al nuevo componente que se ha añadido directamente desde el Page Builder
     * arrastrando y soltando en el área de "Edición de página"
     * Esto se realiza por si, el usuario, decide de nuevo pulsar en editar el componente recién creado
     * @param {object} componentCreated : Objeto que contiene los datos del componente nuevo creado.
     * La estructura del objeto componentCreated es la siguiente:      
        .name: Nombre del componente
        .type: Tipo del componente
        .id: Id del componente         
     */
    handleAddPropertiesToCreatedComponent: function(componentCreated){
        const that = this;
        // Id del nuevo item creado        
        const idComponent = componentCreated.id;
        // newItemCreated será el item nuevo que se ha arrastrado al área de PageBuilder (Asignar sus propiedades, url, urlSave y data-idcomponent);
        if (that.newItemCreated != undefined){
            that.newItemCreated.attr("id", idComponent);
            that.newItemCreated.data("idcomponent", idComponent);

            // Cambiar sus rutas
            // Url para editar y guardar el componente. Obtenerlas de pParams pasados desde la vista
            const urlSaveComponent = that.pParams.urlSaveComponentTemplate.replace("COMPONENT_KEY", idComponent); 
            const urlEditComponent = that.pParams.urlEditComponentTemplate.replace("COMPONENT_KEY", idComponent);
            // Asignar las rutas al componente añadido
            const btnEditComponent = that.newItemCreated.find(`.${that.btnEditComponentFromCMSBuilderClassName}`);
            btnEditComponent.data("urleditcomponent", urlEditComponent);
            btnEditComponent.data("urlsave", urlSaveComponent);
            // Asignar el nombre del componente
            that.newItemCreated.find('.component-name').html(componentCreated.name);


            // Componente editado -> Vaciarlo
            that.newItemCreated = undefined;            
        }
    },

    /**
     * Método para añadir al listado de componentes creados el nuevo componente creado a través del modal.
     * @param {object} componentCreated : Objeto que contiene los datos del componente nuevo creado.
     * La estructura del objeto componentCreated es la siguiente:      
        .name: Nombre del componente
        .type: Tipo del componente
        .id: Id del componente
     */
    handleAddNewComponentToCreatedComponent: function(componentCreated){
        const that = this;
        // Recoger los datos para la construcción del componente
        const componentName = componentCreated.name;
        const componentType = componentCreated.type;
        const idComponent = componentCreated.id;
        // Url para editar y guardar el componente. Obtenerlas de pParams pasados desde la vista
        const urlSave = that.pParams.urlSaveComponentTemplate.replace("COMPONENT_KEY", idComponent); 
        const urlEdit = that.pParams.urlEditComponentTemplate.replace("COMPONENT_KEY", idComponent);

        // Construir la plantilla del componente creado sólo si es de nueva creación
        if (stringToBoolean(operativaGestionComponentsCMS.pParams.isEdited) != true){
            const componentTemplate = `
            <li class="builder-item" 
                id="${idComponent}"
                data-id="${idComponent}" 
                data-urlsave="${urlSave}"
                data-urleditcomponent="${urlEdit}">
            
                <div class="builder-item-wrap">
                    <div class="name">
                        <span class="component-name">${componentName}</span>
                        <span class="type">${componentType}</span>
                    </div>
                    <div class="handler"><span class="material-icons">drag_indicator</span></div>
                </div>
            </li>                
            `;
        
            // Añadir el nuevo componente creado al principio de la lista de Componentes creados
            $(`#${that.createdListComponentsId}`).prepend(componentTemplate);
        }

    },

    /**
     * Método que comprueba en la carga de la página si hay columnas sin componentes para vaciar por completo esa sección
     * evitando espacios en blanco y permitiendo el mensaje de ayuda
     */
    handleCheckComponentsListIfEmpty: function(){
        const that = this;        
        
        $.each(that.componentsList, function(){
            const componentItem = $(this);
            const htmlColumnContent = componentItem.html().trim();
            // Si no hay nada mas que espacios eliminar su contenido y dejarlo vacío
            if (htmlColumnContent.length == 0){
                componentItem.html('');
            }
        });                        
    },
        

    /**
     * Método para buscar y filtrar componentes CMS haciendo petición a backEnd ya que hay muchos componentes en la comunidad. 
     * El nº máximo de componentes que se traerán será de 10.
     * @param {jqueryElement} input : Input donde se ha introducido la cadena de búsqueda del componente
     */    
    handleSearchComponentItemQuery: function(input){
        const that = this;

        // Cadena introducida para filtrar/buscar componentes
        let cadena = input.val();
        // Eliminamos posibles tildes para búsqueda ampliada
        cadena = cadena.normalize('NFD').replace(/[\u0300-\u036f]/g, "").toLowerCase();

        // Objeto para buscar un determinado componente
        const dataPost = {
            search: cadena,
        }

        // Mostrar loading en el área de listado de componentes
        loadingMostrar(that.createdComponents);

        // Petición al servidor para buscar un componente
        GnossPeticionAjax(
            `${that.urlSearchComponent}`,
            dataPost,
            true
        ).done(function (data) {
            // OK -> Items encontrados
            // Comprobar hay al menos resultados según el criterio introducido            
            if ($(data).find(".builder-item").length > 0){                
                that.createdComponents.hide().html(data).fadeIn();
                // Activar compontamiento de drag de componentes                
                PageBuilderEdition.componentsEdition.initComponents();
            }else{
                const templateSinComponentes = `<div><h4 style="text-align: center">No se han encontrado componentes</h4></div>`;
                that.createdComponents.hide().html(templateSinComponentes).fadeIn();
            }                   
        }).fail(function (data) {
            const templateSinComponentes = `<div><h4 style="text-align: center">No se han encontrado componentes</h4></div>`;
            that.createdComponents.hide().html(templateSinComponentes).fadeIn();            
           
        }).always(function () {
            // Ocultar loading
           loadingOcultar();
        });  
    },

    /**
     * Método para buscar y filtrar componentes CMS. Filtrará los items (visualizar u ocultar) siempre y cuando
     * coincida con la cadena introducida
     * @param {jqueryElement} input : Input donde se ha introducido la cadena de búsqueda del componente
     */
    handleSearchComponentItem: function(input){
        const that = this;

        // Cadena introducida para filtrar/buscar componentes
        let cadena = input.val();
        // Eliminamos posibles tildes para búsqueda ampliada
        cadena = cadena.normalize('NFD').replace(/[\u0300-\u036f]/g, "").toLowerCase();

        // Listado de componentes donde se buscará
        const componentItems = that.componentsTabContent.find($(`.${that.componentListItemClassName}`));

        // Recorrer los items/compontes para realizar la búsqueda de palabras clave
        $.each(componentItems, function(index){
            const componentItem = $(this);
            // Seleccionamos el nombre de la redirección y quitamos caracteres extraños, tiles para poder hacer bien la búsqueda
            const componentName = $(this).find(".component-name").html().trim().normalize("NFD").replace(/[\u0300-\u036f]/g, "").toLowerCase();
            const componentType = $(this).find(".type").html().trim().normalize("NFD").replace(/[\u0300-\u036f]/g, "").toLowerCase();
            if (componentName.includes(cadena) || componentType.includes(cadena) ){
                // Mostrar fila resultado y sus respectivos padres
                componentItem.removeClass("d-none");                
            }else{
                // Ocultar fila resultado
                componentItem.addClass("d-none");
            }            
        });                        
    },

    /**
     * Método para guardar la página CMS desde CMS Builder. El tipo de guardado puede ser:
     * Guardar borrador
     * Descartar borrador
     * Publicar
     * Vista previa      
     * @param {String} accion : Identificador string para saber cual de las acciones se desea realizar de las arriba mencionadas
     */
    handleSaveCMSPage: function(accion){
        const that = this;

        // Mostrar loading
        loadingMostrar();
        
        // Método a realizar -> Guardado
        let metodo = "";

        // Tipo de guardado a realizar
        switch (accion) {
            case "guardado":
                metodo = "save";
                break;
            case "publicar":
                metodo = "publish";
                break;   
            case "vistaPrevia":   
                metodo = "pre-view";
                setTimeout(function() {
                    loadingOcultar();
                },8000);
                break;                                                
            default:
                metodo = "discard";
                break;
        }
        
        if (metodo != "discard"){
            // Procesos de guardado
        
            // Objeto para envío de guardado a backEnd
            const dataPost = that.getDataForSavingCMSPage();            

            // Petición al servidor para guardado de página
            GnossPeticionAjax(
                `${that.urlSaveCMSPage}/${metodo}`,
                dataPost,
                true
            ).done(function (data) {
                // OK -> Guardado correcto           
               mostrarNotificacion("success", "La página CMS se ha guardado correctamente.");
               // Eliminar el backup que haya de la página en el localStorage
               operativaGestionCMSBuilderGuardadoSeguridad.deletePageDetailInfoInLocalStorage();

               setTimeout(function () {
                   location.reload();
               }, 1000);
            }).fail(function (data) {
                // KO -> Error en guardado
               if (data != "Contacte con el administrador del Proyecto, no es posible atender la petición.") {               
                   mostrarNotificacion("error", data);
               }
               else {
                mostrarNotificacion("error", data);
               }
            }).always(function () {
                // Ocultar loading
               loadingOcultar();
            });  
            
        }        
    },


    /**
     * Método que devolverá un objeto con los datos a guardar de la página CMS que se está editando.
     * 
     * @returns Devuelve un objeto con los datos a guardar de la página CMS
     */
    getDataForSavingCMSPage: function(){
        const that = this;

        // Datos a guardar de la estructura de la página
        let estructura = "";

        // Recorrer cada una de las filas existentes
        $(`.${that.rowCmsClassName}`).each(function () {
            const item = $(this);
            estructura += "|" + that.getCMSItem(item);
        });

        // Eliminar la clase component-saved (Flag) de los componentes revisados
        $(".component-saved").removeClass("component-saved");

        // Recogida de datos de opciones de página CMS (Modal)        
        const mostrarSoloCuerpoValue = $("input[name='rbMostrarSoloCuerpo']:checked").data("value") == "si" ? true : false;        

        const dataPost = {
            Estructura: estructura,
            OpcionesPropiedades: that.getComponetsProperties(),
            MostrarSoloCuerpo: mostrarSoloCuerpoValue,// $('#chkMostrarSoloCuerpo').is(':checked')
            FechaModificacion: $('#fechaModificacionPaginaCMS').val()
        }
        return dataPost;
    },

    /**
     * Método para obtener los datos de una fila de las filas del PageBuilder para poder guardar la estructura de la página.
     * Este método se ejecuta desde 'getDataForSavingCMSPage' durante el proceso de guardado de la página.      
     * @param {jqueryElement} elemento : Fila o columna dentro de una fila que está siendo analizada para obtener los datos existentes en ella
     */
    getCMSItem: function(elemento){
        const that = this;

        // Determinar si una row o una columna
        let bloque = false;
        if (elemento.attr('class').indexOf("cmscolumn") != -1 || elemento.attr('class').indexOf("cmsrow") != -1 ) {
            bloque = true;
        }

        /* Prefijo */
        let inicio = "componente_"; 
        // Si se trata de una columna, añadir el prefijo       
        if (bloque) {
            let className = elemento.data("columnclass") == undefined ? "" : elemento.data("columnclass");// elemento.data("aux") == undefined ? "" : elemento.data("aux"); // elemento.data("columnclass") == undefined ? "" : elemento.data("columnclass");
            // Formateo de los atributos de la fila
            let attributesRow = elemento.data("aux") == undefined ? "" : elemento.data("aux");
            // Formateo de la className
            className = replaceAll(className,"_", "@#*") + "_";
            if (attributesRow != ""){
                inicio = "bloque_" + attributesRow + className;
            }else{
                inicio = "bloque_" + className;                
            } 
        }

        // Construir el string del componente
        let strElemento = inicio + elemento.attr('id') + ":[";
        // Contador
        let i = 0;

        // Obtener las columnas y componentes posibles del elemento
        const columnsAndComponents = elemento.find(`.${that.columnCmsClassName}, .${that.componentCmsClassName}`);

        // Revisar los hijos del elemento que se está analizando
        columnsAndComponents.each(function () {        
            const item = $(this);
            if (item.attr('class').indexOf("component-saved") == -1){
                if (i > 0) {
                    strElemento += ",";
                }
                // Si hay más columnas dentro del elemento analizado o si hay componentes
                if (item.find('.cmscolumn').length > 0 || item.find('.cmscomponent').length > 0) {
                    strElemento += that.getCMSItem(item);
                } else {                
                    let bloqueIn = false;
                    // Comprobar si hay una columna dentro
                    if (item.attr('class').indexOf("cmscolumn") != -1) {
                        bloqueIn = true;
                    }
    
                    let inicio2 = "componente_";
                    if (bloqueIn) {
                        inicio2 = "bloque_" + item.data("columnclass") + "_"; // item.attr('aux') + "_";
                    }
                    // Añadir clase "component-saved" para que no vuelva a pasar por él
                    item.addClass("component-saved");
                    strElemento += inicio2 + item.attr('id') + ":[]";
                }
                i++;  
            }                                 
        });
        // Cerrar el string del item para su estructura
        strElemento += "]";
        return strElemento;
    },

    /**
     * Método para obtener las opciones de componentes. Ej: Opciones si se selecciona un componente de tipo Menú (Selección del item activo)
     */
    getComponetsProperties: function(){
        const that = this;

        let strOpcionComponente = "";
        
        $(`.${that.propiedadComponenteClassName}`).each(function () {            
            const item = $(this);
            const idBloque = item.data("idbloque");     // item.attr('idBloque');
            const idComponente = item.data("idcomponente"); // item.attr('idComponente');
            const idPropiedad = item.data("idpropiedad");  // item.attr('idPropiedad');
            const opcion = item.val();// item.val();
            if (opcion != "-") {
                strOpcionComponente += idBloque + "_" + idComponente + "_" + idPropiedad + "_" + opcion + "|";
            }
        });
        return strOpcionComponente;
    }

}


/**
 * Operativa para realizar el guardado de una página CMS a modo de backup en el LocalStorage 
 */ 
const operativaGestionCMSBuilderGuardadoSeguridad = {

    /**
     * Inicializar operativa
     */
     init: function (pParams) {
        this.pParams = pParams;
        this.config(pParams);
        this.configEvents();
        this.triggerEvents();            
    },

    /*
     * Inicializar elementos de la vista
     * */
    config: function (pParams) {

        // Modal de restaurar contenido de la página
        this.modalBackupCMSHtmlContent = $("#modal-backup-cms-html-content");
        /* Componentes del Editor del CMS */
        // Editor de la página CMS
        this.cmsEditor = $(".builder-editor");
        // Wizard donde se muestran posibles plantillas para creación de la página CMS
        this.cmsWizard = $(".builder-start");
        // Área donde estarán todas las filas de la página. Aquí es donde se añadirán nuevas
        this.rowList = $("#cmsrow-list");
        // Botones para confirmar o no la restauración de la copia de seguridad de la página cms
        this.btnConfirmRestorePageCMS = $(".btnConfirmRestorePageCMS");
        this.btnNotConfirmRestorePageCMS = $(".btnNotConfirmRestorePageCMS");    
        
        // Botón de acciones a realizar desde PageBuilder
        this.dropDownActionsCMSBuilder = $("#dropDownActionsCMSBuilder");

        // Flags de inicio de operativa
        // Local storage no disponible de momento
        this.isLocalStorageAvailable = false;
        // Indicador de si hay contenido previo de la página que se está editando
        this.isBackupContentAvailableForPageId = false;
        // Id de la página que será proporcionada por la operativa del CMS, que será donde se inicie esta operativa
        this.pageId = pParams.pageId;

        // Key que almacenará el id de la pagina
        this.keyIdCmsPage = "KEY_CMS_PAGE_ID";
        // Key que almacenará el contenido del html que se ha "copiado"
        this.keyCmsHtmlContent = "KEY_CMS_HTML_CONTENT_ID";
        // Key que almacenará valor booleano para saber si hay que guardar o no copia de seguridad de la página
        this.keyisNecessarySaveCmsHtmlContent = "KEY_IS_NECESSARY_SAVE_CMS_HTML_CONTENT";
        // Tiempo en el que se realizará cada vez una copia de seguridad (Milisegundos)
        this.timeToBackupContent = 35000;
    },

    /*
     * Configurar eventos de los elementos html
     * */    
    configEvents: function(){
        const that = this;

        // Restaurar la copia de seguridad de la página cms que se ha encontrado
        this.btnConfirmRestorePageCMS.on("click", function(){
            that.handleRestorePageCMS();            
        });

        // Cancelar la restauración de la copia de seguridad de la página cms que se ha encontrado
        this.btnNotConfirmRestorePageCMS.on("click", function(){
            that.handleNotRestorePageCMS();
        });

    },

    /**
     * Métodos que se ejecutarán al inicio de la operativa
     */
    triggerEvents: function(){
        const that = this;

        this.checkLocalStorage();
        setTimeout(function () {
            that.initBackupSaved();
            // Activar comportamientos para las rows del backup
            PageBuilderManagement.init(); 
        },1500);        
    },

    /**
     * Método para comprobar si el localstorage está disponible. Lo guardará en la variable para futuras acciones.
     */
     checkLocalStorage: function(){
        const that = this;
        if (storageAvailable('localStorage')) {
            // El storage está disponible   
            that.isLocalStorageAvailable = true;            
        }
    },    

    /**
     * Método que iniciará el copiado del estado de la página siempre y cuando el localStorage esté disponible
     */
    initBackupSaved: function(){
        const that = this;

        that.checkBackupContentAvailableForPageId();

        if (that.isBackupContentAvailableForPageId){
            // Preguntar si se desea restaurar el contenido actual almacenado en BD            
            that.modalBackupCMSHtmlContent.modal("show");
        }else{
            if (that.isLocalStorageAvailable == true){
                // Ejecutar copiado de backup de forma planificada
                that.initScheduleBackupCopy();
            }
        }
    },

    /**
     * Copiado programado del contenido de la página CMS
     */
    initScheduleBackupCopy: function(){
        const that = this;
        // Realizar copiado de página CMS cada X segundos
        setInterval(function(){            
            that.savePageDetailInfoInLocalStorage();
        }, that.timeToBackupContent/*30000*/);
    },

    /**
     * Método que copiará el contenido de la página que está siendo editada en localStorage a modo de backup
     */
    savePageDetailInfoInLocalStorage: function(){
        const that = this;
       
        // Sólo iniciará el copiado si no está en modo Wizard o la página ya existía con anterioridad
        if ( (this.cmsWizard.length > 0 && !this.cmsWizard.hasClass("d-none") ) ){                                    
            return;            
        }
        
        // No hacer nada si no hay una página id proporcionada
        if (this.pageId == "" || this.pageId == undefined){
            return;
        }

        // No hacer copia si no hay un flag que permite el guardado (Ej: El usuario no ha hecho aun ninguna acción de edición)        
        if (!that.allowSaveCmsHtmlContentInPageBuilder()){
            return;
        }

        // Contenido de la página CMS
        const backupData = that.rowList.html();
        // Guardar el contenido        
        localStorage.setItem(that.keyCmsHtmlContent, backupData); 
        // Guardar el id de la página               
        localStorage.setItem(that.keyIdCmsPage, that.pageId); 
    },

    /**
     * Método que eliminará el contenido almacenado en localStorage
     * Este método se ejecutará cuando se guarde de forma manual la página por el usuario
     */
     deletePageDetailInfoInLocalStorage: function(){
        const that = this;
        
        if (this.isLocalStorageAvailable){
            localStorage.removeItem(that.keyCmsHtmlContent);
            localStorage.removeItem(that.keyIdCmsPage);
            localStorage.removeItem(that.keyisNecessarySaveCmsHtmlContent);        
        }
    },

    /**
     * Método que comprobará si hay información previa de la página por si no se han guardado los cambios
     */
    checkBackupContentAvailableForPageId: function(){
        const that = this;
        let currentPageId = localStorage.getItem(that.keyIdCmsPage);

        if (currentPageId == that.pageId){
            that.isBackupContentAvailableForPageId = true;
        }else{
            that.isBackupContentAvailableForPageId = false;
        }
    },

    /**
     * Método para restaurar la página de backup al CMS Builder
     */
    handleRestorePageCMS: function(){
        const that = this;

        // Mostrar loading
        loadingMostrar();

        setTimeout(function(){
            // Contenido de la página del localStorage
            const htmlContent = localStorage.getItem(that.keyCmsHtmlContent);
            // Establecer el contenido de la página CMS
            that.rowList.html(htmlContent);                        
            // Ocultar loading            
            loadingOcultar();            
            // Ocultar el modal de restauración 
            dismissVistaModal(that.modalBackupCMSHtmlContent);
            // Si está el wizard, eliminarlo
            // Mostrar el editor y ocultar el wizard        
            that.cmsWizard.fadeOut("normal", function() {
                // Visualizar el editor de páginas
                that.cmsEditor.removeClass("d-none");  
                // Visualizar las acciones permitidas del CMSBuilder
                that.dropDownActionsCMSBuilder.removeClass("d-none");
                that.cmsWizard.addClass("d-none");
                // Visualizar correctamente el panel lateral de diseños y componentes
                overlayDestroy();
            });
 
            // Eliminar el item restaurado
            that.deletePageDetailInfoInLocalStorage();
            // Iniciar de nuevo la operativa
            that.triggerEvents();
        },1500);
        
    },

    /**
     * Método para no restaurar la vista de copia de seguridad.
     */
    handleNotRestorePageCMS: function(){
        const that = this;
        // Eliminar el item restaurado
        that.deletePageDetailInfoInLocalStorage();
        // Ocultar el modal de restauración 
        dismissVistaModal(that.modalBackupCMSHtmlContent);
        // Iniciar de nuevo la operativa
        that.triggerEvents();
    },
    
    
    /**
     * Método para establecer o no la señal de que alguna acción se ha realizado en el pageBuilder
     * @param {bool} changePerfomed : Valor booleano para indicar si hay alguna acción realizada. Si la hay (true), establecer la bandera a true, en caso contrario, 
     * eliminarla
     * Este método deberá ser llamado por métodos de operativa CMS que modifican la página del usuario
     */
    setIsNecessarySaveCmsHtmlContentInPageBuider: function(changePerformed){
        const that = this;
        if (that.isLocalStorageAvailable == true && changePerformed ){            
            // Indicar mediante Flag que se ha producido un cambio (para que se guarde una copia de la página CMS)
            localStorage.setItem(that.keyisNecessarySaveCmsHtmlContent, true);             
        }else{
            // Eliminar el flag 
            localStorage.removeItem(that.keyisNecessarySaveCmsHtmlContent);
        }
    },
    
    /**
     * Método que devuelve un valor booleano indicando si permite o no realizar el guardado automático de la página CMS.
     * Sólo se permitirá realizar el guardado automático si existe el key en el localStorage de 'keyisNecessarySaveCmsHtmlContent'
     * @returns bool: Devuelve un valor permitiendo o no realizar copias de seguridad del CMS
     */
    allowSaveCmsHtmlContentInPageBuilder: function(){
        const that = this;
        
        const isNecessarySaveCmsHtmlContentInPageBuilder = localStorage.getItem(that.keyisNecessarySaveCmsHtmlContent);

        if (isNecessarySaveCmsHtmlContentInPageBuilder != undefined){
            return isNecessarySaveCmsHtmlContentInPageBuilder;
        }
        return false;
    },

}