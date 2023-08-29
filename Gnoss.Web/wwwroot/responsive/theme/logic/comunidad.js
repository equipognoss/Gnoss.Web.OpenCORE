/**
 ***************************************************************************************
 * Logica / Operativas de comportamiento JS para la sección de Comunidad del DevTools
 * *************************************************************************************
 */

/**
 * Operativa para navegación de la comunidad en la Home de DevTools
 * Dependiendo del click donde se haga, se hará una petición para que traiga la vista parcial correspondiente
 * y se muestre en el bloque central de pantalla
 * */
 const operativaNavegacionHome = {
    /**
     * Inicializar operativa
     */
    init: function (pParams) {
        this.config(pParams);
        this.pParams = pParams;
        this.configEvents();
    },
    /*
     * Inicializar elementos de la vista
     * */
    config: function (pParams) {
        // Contendor donde se deseará mostrar la vista parcial de la home dependiendo de la sección clickeada.
        this.sectionHomeContent = $(`#${pParams.idSectionHomeContent}`);
        // Cada una de las secciones para realizar la navegación.
        this.classNavigationSections = $(`.${pParams.classNavigationSections}`);                
        // Nombre de la clase de las secciones de navegación
        this.classNameNavigationSections = pParams.classNavigationSections;                
        // Url actual para realizar posibles peticiones
        this.url = window.location.href; 
        // Secciones de navegación del menú Home
        this.dataSections = pParams.dataSections;
        // Título de la sección de la home
        this.title = $(".h1-container h1");
        // Título de la sección de la home clickada
        this.titleSelected = "";
        // Menú lateral y menú dentro del menú lateral de navegación
        this.panel_lateral = $('.panel-lateral');
        this.menu = this.panel_lateral.find('#menu');
    },

    /**
    * Manejador de eventos
    */
    configEvents: function () {
        const that = this;
        // Controlar click de secciones  
        configEventByClassName(`${this.classNameNavigationSections}`, this.handleClickSection);
    },

    /**
     * Gestionar la navegación dependiendo del click realizado en las diferentes posibles secciones de la home
     */
    handleClickSection: function (element){
        const that = operativaNavegacionHome;

        const $jqueryElement = $(element);
        $jqueryElement.on("click", function(){						            
            // Mostrar ocultar panel izquierda en base a la selección
            const data = $(this).attr('data-section');
            const parent_section = $(this).attr('data-parent-section');
            that.titleSelected = $(this).attr('data-title');

            if (parent_section !== undefined) {
                that.menu.find('.' + data).get(0).click();
            } else {
                that.menu.find('a').removeClass('activo');
                that.classNavigationSections.remove();
                that.menu.find('.' + data + '> a').addClass('activo');
                that.menu.find('#' + data).collapse('show');
                
                // Navegación/Carga de la vista de la sección Home correspondiente 
                that.handleUrlHomeNavigation(data);
            }
        });	
    },

    /**
     * Realizar la navegación a la sección correspondiente haciendo petición según la sección de la home pulsada.
     * Devolverá una vista para ser mostrada en la sección central de la Home.
     * @param {string} data : Sección de clickeada del menú.
     */
    handleUrlHomeNavigation: function(data){
        const that = this;
        // Mostrar loading
        loadingMostrar();
        // Permitir navegación
        let allowUrlNavigation = true;

        // Realizar petición para obtener la vista correspondiente
        if (allowUrlNavigation){     
            const formData = {
                HomeSection: data,
            }       
            getVistaFromUrl(`${this.url}/load-home-section`, this.sectionHomeContent, formData, function(result, message){
                // OK
                if (result == requestFinishResult.ok){
                    if (message != undefined){
                        mostrarNotificacion("success", message);
                    }
                    // Cambiar el título
                    that.titleSelected.length > 0 && that.title.text(that.titleSelected);                    
                }else{
                    // KO
                    if (message != undefined){
                        mostrarNotificacion("error", message);
                    }else{
                        mostrarNotificacion("error", "Se ha producido un error en la navegación de la home. Inténtalo de nuevo más tarde o contacta con el administrador de la comunidad.");
                    }
                }                            
                loadingOcultar();
            }, true);            
        }
    },
};

/**
 * Operativa de funcionamiento de Información General de la Comunidad
 */
const operativaInformacionGeneral = {

     /**
     * Inicializar operativa
     */
    init: function (pParams) {
        this.pParams = pParams;
        this.config(pParams);
        this.configEvents();
        this.triggerEvents();
        // Mostrar la fecha fin de cierre de comunidad en base al nº de días de gracia.
        this.handleChangeCambiarFechaFin();        
    },

    /*
     * Inicializar elementos de la vista
     * */
    config: function (pParams) {
        
        // Elementos del DOM
        /* Nombre y Descripcion */
        this.nombreComunidad = $('#txtNombre');
        this.descripcionComunidad = $('#txtDescripcion');

        /* ImagenCabecera */
        this.panelImagenCabecera = $("#panelImagenCabecera"); // Panel donde se mostrará la vista de ImagenCabecera. Será donde se añada lo devuelto por backend al subir la imagen
        this.panelImagenFavicon = $("#panelImagenFavicon"); // Panel donde se mostrará la vista de Favicon. Será donde se añada lo devuelto por backend al subir la imagen
        this.imageHeadSrc = "#ImageHead_src"; // Id del Input oculto donde se guardará la imagen subida        
        this.imageFaviconSrc = "#ImageFavicon_src"; // Id del Input oculto donde se guardará la imagen subida        
        
        this.contenedorImagenCabecera = $("#contenedorImagenCabecera"); // Contenedor para poder hacer drag/drop de la imagen de la cabecera
        this.contenedorImagenFavicon = $("#contenedorImagenFavicon"); // Contenedor para poder hacer drag/drop de la imagen favicon
        
        this.previewImg = $(".image-uploader__cabecera"); // Imagen preview de la cabecera
        this.previewImgFavicon = $(".image-uploader__favicon"); // Imagen preview de favicon
        this.previewImgJcrop = // Imagen preview del recorte realizado
        this.contenedorImagenesCabeceraJcrop = $("#contenedorImagenesCabeceraJcrop"); // Contenedor de las imagenes de cabecera (Original + Preview de jcrop)
        this.contenedorImagenesFaviconJcrop = $("#contenedorImagenesFaviconJcrop"); // Contenedor de las imagenes de cabecera (Original + Preview de jcrop)        
        this.fileUploadChangeImgCabecera = $("#fileUploadChangeImgCabecera"); // Input para explorar imagen de cabecera (oculto)
        this.fileUploadChangeImgFavicon = $("#fileUploadChangeImgFavicon"); // Input para explorar imagen de cabecera (oculto)        
        this.panelAccionesImagenCabecera = $("#panelAccionesImagenCabecera"); // Contenedor de acciones de la imagen de cabecera (Cambiar/Eliminar imagen)
        this.panelAccionesImagenFavicon = $("#panelAccionesImagenFavicon"); // Contenedor de acciones de la imagen de Favicon (Cambiar/Eliminar imagen)
        this.btnCambiarImagenCabecera = $("#cambiarImagenCabecera"); // Botón de cambiar imagen cabecera
        this.btnCambiarImagenFavicon = $("#cambiarImagenFavicon"); // Botón de cambiar imagen favicon
        this.btnEliminarImagenCabecera = $("#eliminarImagenCabecera"); // Botón de eliminar imagen cabecera     
        this.btnEliminarImagenFavicon = $("#eliminarImagenFavicon"); // Botón de eliminar imagen cabecera
        this.urlBase = location.href; // Url base para poder añadir lo necesario para la ejecución de acciones.
        this.urlSubirImagenCabecera = `${location.href}/upload-head-image`; // Url de petición para realizar la subida de la imagen
        this.urlSubirImagenFavicon = `${location.href}/upload-favicon-image`; // Url de petición para realizar la subida de la imagen
        this.urlAbrirComunidad = `${location.href}/open-community`; // Url de petición para realizar la subida de la imagen
        this.urlChangeCommunityShortName = `${location.href}/rename-community-short-name`; // Url para cambiar el nombre corto de la comunidad
        this.urlChangeTypeCommunity = `${location.href}/change-community-type`; // Url para cambiar el tipo de la comunidad
        this.dropareaImagenCabecera = $('.js-image-uploader');  // Sección para aplicar el comportamiento del plugin imageDropArea
        // Parámetros X, Y de imagen de la cabecera de la comunidad
        this.imagenHeadRuta = $("#ImageHead_src");
        this.imagenHeadPost_X_0 = $("#ImageHead_Pos_X_0");
        this.imagenHeadPost_Y_0 = $("#ImageHead_Pos_Y_0");
        this.imagenHeadPost_X_1 = $("#ImageHead_Pos_X_1");
        this.imagenHeadPost_Y_1 = $("#ImageHead_Pos_Y_1");
        this.imagenHeadAlto = $("#ImageHead_Alto");
        this.imagenHeadAncho = $("#ImageHead_Ancho");
    
        /* Estado de la Comunidad */
        // Select/Combobox de estado de comunidad
        this.selectEstadoComunidad = $('#selectEstadoComunidad');
        // Panel contenedor donde se muestra el estado de la comunidad.
        this.panelEstadoComunidadContenedor = $('#panelEstadoComunidadContenedor');
        // Botón para abrir comunidad si está en "Definición"
        this.btnAbrirCom = $('#btnAbrirCom');
        // Input de fecha reapertura
        this.inputFechaReapertura = $("#txtFechaReapertura");
        // Input motivo del cierre
        this.inputMotivoCierre = $("#txtMotivoCierre");
        // Select días de gracia
        this.selectDiasGracia = $("#ddlDiasGracia");
        // Fecha de cierre de la comunidad. Calculado en base al nº de días de gracia
        this.fechaFinalGracia = $("#fechaFinalGracia");
        // Panel donde se cargarán los inputs con posibilidad de multiIdioma (Nombre, Descripción)
        this.panContenidoMultiIdioma = $('#panContenidoMultiIdioma');

        // Input (oculto) que guardará el estado de la comunidad para envío 
        this.cmbHiddenEstadoComunidad = $('#cmbEstado');
        // Paneles informativos de estado de la comunidad
        this.panelEstadoComunidadAbierto = $("#panelEstadoComunidadAbierto");
        this.panelEstadoComunidadCerrado = $("#panelEstadoComunidadCerrado");
        this.panelEstadoComunidadCerradoTemporalmente = $("#panelEstadoComunidadCerradoTemporalmente");
        this.panelEstadoComunidadCerrandose = $("#panelEstadoComunidadCerrandose");
        this.panelesEstadoComunidad = [this.panelEstadoComunidadAbierto, this.panelEstadoComunidadCerrado, this.panelEstadoComunidadCerradoTemporalmente, this.panelEstadoComunidadCerrandose];
        
         /* Tags de la comunidad */
        this.inputTagsComunidad = $("#txtTags_Hack");
        
        /* MultiIdiomas de la comunidad */
        // RadioButton para elegir si se desea multiIdioma
        this.radioBtnMultiIdiomaSi = $('#comunidad-traducciones-si');
        this.radioBtnMultiIdiomaNo = $('#comunidad-traducciones-no');
        // Select de idioma por defecto para la comunidad
        this.selectMultiIdioma = $('#ddlIdioma');
        // Checkbox oculto que guardará la selección de multiIdioma
        this.chkIdioma = $('#chkIdioma');

        /* Guardar Información General */
        // Botón para guardar
        this.btnGuardarInformacionGeneral = $("#btnGuardarInformacionGeneral");
        this.btnShowHideIconClassName = 'showHide-icon';
        // Objeto para guardar los datos a guardar
        this.Options = {};

        /* Operativa modal cambiar nombre corto de comunidad */
        // Botón para lanzar el cambio del nombre corto de la comunidad
        this.btnCambiarNombreCortoComunidad = $("#btnCambiarNombreCortoComunidad");
        // Input donde se cambiará el nombre corto de la comunidad
        this.inputNombreCortoComunidad = $("#inputNombreCortoComunidad");
        // Botón para guardar el nombre corto de la comunidad cambiado
        this.btnGuardarNombreCortoComunidad = $("#btnGuardarNombreCortoComunidad");
        // Label de ayuda para indicar que no se permite nombres cortos de la comunidad
        this.nombreCortoComunidadAyuda = $("#nombreCortoComunidadAyuda");
        // Label en la página principal donde se muestra el nombre corto de la comunidad
        this.lblNombreCortoComunidad = $("#lblNombreCortoComunidad");


        /* Operativa modal cambiar el tipo de la comunidad */
        // Botón para lanzar el cambio del nombre corto de la comunidad
        this.btnCambiarTipoComunidad = $("#btnCambiarTipoComunidad");
        // Select para elegir el tipo de comunidad
        this.selectTipoComunidad = $("#selectTipoComunidad");
        // Botón para guardar el tipo de la comunidad
        this.btnGuardarTipoComunidad = $("#btnGuardarTipoComunidad");
        // Label en la página principal donde se muestra el tipo de comunidad
        this.txtTipoAcceso = $("#txtTipoAcceso");
        // Icono del tipo de la comunidad
        this.iconTipoAcceso = $("#iconTipoAcceso");

        /* Modal para el cambio de tipo de Comunidad */
        // RadioButton con el tipo de comunidad
        this.rbCommunityTypeName = 'communityType';
        // Input de confirmación previo antes de cambio de comunidad
        this.txtConfirmCommunityChangeType = $('#txtConfirmCommunityChangeType');
        // Botón para guardar el cambio de tipo de comunidad
        this.btnGuardarTipoComunidad = $("#btnGuardarTipoComunidad");
        // Nombre del panel informativo del tipo de comunidad - Irá seguido del tipo de comunidad (0,1,2)
        this.panParrafoAccesoId = "panParrafoAcceso";
        // Panel que alerta por el cambio de tipo de comunidad
        this.alertConfirmChangeCommunityType = $("#alertConfirmChangeCommunityType");
        // Nombre actual de la comunidad (Comparación para confirmar el cambio de tipo de comunidad)
        this.currentCommunityName = $("#currentCommunityName");
        // Label que muestra el tipo de comunidad actual. Corresponde con el botón para llamar al cambio de tipo.
        this.txtTipoAcceso = $("#txtTipoAcceso");
        // Modal para cambiar el tipo de Comunidad
        this.modalChangeCommunityType = $("#modal-change-community-type");

        /* Modal para el cambio de nombre corto de Comunidad */
        // Botón para guardar el cambio de nombre corto de comunidad
        this.btnGuardarNombreCortoComunidad = $("#btnGuardarNombreCortoComunidad");
        // Panel que alerta por el cambio de nombre corto de comunidad
        this.alertConfirmChangeShortName = $("#alertConfirmChangeShortName");
        // Nombre actual de la comunidad 
        this.txtCommunityShortName = $("#txtCommunityShortName");        
        // Modal para cambiar el nombre corto de la comunidad
        this.modalChangeCommunityShortName = $("#modal-rename-community");


        // Parámetros para la operativa multiIdioma (helpers.js)
        this.operativaMultiIdiomaParams = {
            // Nº máximo de pestañas con idiomas a mostrar. Los demás quedarán ocultos
            numIdiomasVisibles: 3,
            // Establecer 1 tab por cada input (true, false)
            useOnlyOneTab: true,
            //panContenidoMultiIdioma: this.panContenidoMultiIdioma,
            panContenidoMultiIdiomaClassName: "panContenidoMultiIdioma",
        };
        
        // Valor actual de la comunidad (Comparación para confirmar el cambio de tipo de Comunidad)
        this.currentCommunityNameValue = this.currentCommunityName.html().trim();
        this.currentCommunityTypeValue = this.txtTipoAcceso.data("tipoaccesovalue");
        this.currentCommunityShortName = this.lblNombreCortoComunidad.data("shortname").trim();

        /// Inicializar el imageDropArea para Cabecera
        this.initImageDropArea();
        
        /// Inicializar el imageDropArea para Favicon
        // De momento la vista estará oculta, evitamos posibles errores
        if (this.fileUploadChangeImgFavicon.length > 0){
            this.initFaviconDropArea();
        }
    },

    /**
     * Configuración de eventos de elementos del Dom (Botones, Inputs...)     
     */
    configEvents: function () {
        const that = this;


        // Cierre del modal de cambio tipo de Comunidad        
        that.modalChangeCommunityShortName
            .on('show.bs.modal', (e) => {                                
            })
            .on('hide.bs.modal', (e) => {
                // Acción de ocultar el modal   
                // Vaciar el input de confirmación         
                that.txtCommunityShortName.val(this.lblNombreCortoComunidad.data("shortname").trim());
                that.txtCommunityShortName.trigger("keyup");                                
            }); 

        // Cierre del modal de cambio tipo de Comunidad        
        that.modalChangeCommunityType
            .on('show.bs.modal', (e) => {                                
            })
            .on('hide.bs.modal', (e) => {
                // Acción de ocultar el modal   
                // Vaciar el input de confirmación         
                that.txtConfirmCommunityChangeType.val("");
                that.txtConfirmCommunityChangeType.trigger("keyup");
                // Dejar el radiobutton anterior pulsando                
                $(`input[name="${this.rbCommunityTypeName}"]`).filter(`[data-value = ${that.currentCommunityTypeValue}]`).prop("checked", true);                                   
            });         

        
        /* ImagenCabecera */
        // Botón para cambiar/elegir nueva imagen de cabecera cuando ya haya una imagen cargada
        configEventByJqueryObject(that.btnCambiarImagenCabecera, function(element){
            // Vista cargada -> Evitar el "Node cannot be found in the current page."
            that.config(that.pParams);
            const $jqueryElement = $(element);
            $jqueryElement.off().on("click", function(){                
                that.fileUploadChangeImgCabecera.trigger("click");
            });	                        
        });  
        
        // Botón para eliminar la actual imagen de cabecera cuando ya haya una imagen cargada
        configEventByJqueryObject(that.btnEliminarImagenCabecera, function(element){
            // Vista cargada -> Evitar el "Node cannot be found in the current page."
            that.config(that.pParams);
            const $jqueryElement = $(element);
                $jqueryElement.off().on("click", function(){                
                // Quito la imagen de input y de la imagen
                $(that.imageHeadSrc).val('');
                that.previewImg.attr('src', '');
                // Ocultar panel de Imagen cargada, imagenPreview y acciones de la imagen
                that.contenedorImagenesCabeceraJcrop.addClass("d-none");                                                           
                // Mostrar el contenedor del drag and drop
                that.contenedorImagenCabecera.removeClass("d-none");                
            });	                        
        }); 

        /* FavIcon */
        // Botón para cambiar/elegir nueva imagen para Favicon
        configEventByJqueryObject(that.btnCambiarImagenFavicon, function(element){
            // Vista cargada -> Evitar el "Node cannot be found in the current page."
            that.config(that.pParams);
            const $jqueryElement = $(element);
            $jqueryElement.off().on("click", function(){                
                that.fileUploadChangeImgFavicon.trigger("click");
            });	                        
        });  
        
        // Botón para eliminar la actual imagen Favicon
        configEventByJqueryObject(that.btnEliminarImagenFavicon, function(element){
            // Vista cargada -> Evitar el "Node cannot be found in the current page."
            that.config(that.pParams);
            const $jqueryElement = $(element);
                $jqueryElement.off().on("click", function(){                
                // Quito la imagen de input y de la imagen
                $(that.imageFaviconSrc).val('');
                that.previewImgFavicon.attr('src', '');
                // Ocultar panel de Imagen cargada, imagenPreview y acciones de la imagen
                that.contenedorImagenesFaviconJcrop.addClass("d-none");                                                           
                // Mostrar el contenedor del drag and drop
                that.contenedorImagenFavicon.removeClass("d-none");                
            });	                        
        });         
        
        /* Estado de la Comunidad */
        // Button para abrir comunidad en definición
        that.btnAbrirCom.off().on("click", function(){            
            that.handleAbrirComunidad();            
        });

        // Select para cambiar el estado de la comunidad        
        configEventByJqueryObject(that.selectEstadoComunidad, function(element){
            // Vista cargada -> Evitar el "Node cannot be found in the current page."
            that.config(that.pParams);
            const $jqueryElement = $(element);
            $jqueryElement.off().on("change", function(e){                                
                const estadoSelected = e.target.value;
                that.handleChangeEstadoComunidad(estadoSelected); 
            });	  
        });

        // Select del nº de días de gracia para el cierre de la comunidad        
        configEventByJqueryObject(that.selectDiasGracia, function(element){
            // Vista cargada -> Evitar el "Node cannot be found in the current page."
            that.config(that.pParams);
            const $jqueryElement = $(element);
            $jqueryElement.off().on("change", function(e){                                
                that.handleChangeCambiarFechaFin();
            });	  
        });

        /* MultiIdiomas de la comunidad */
        this.radioBtnMultiIdiomaSi.off().on("click", function(){
            that.selectMultiIdioma.removeAttr('disabled');
            // Activar checkbox oculto de multiIdioma            
            that.chkIdioma.prop('checked', true);

        });
        this.radioBtnMultiIdiomaNo.off().on("click", function(){
            that.selectMultiIdioma.attr('disabled', 'disabled');
            // Ocultar checkbox oculto de multiIdioma
            that.chkIdioma.prop('checked', false);
        }); 




        // Input del nuevo nombre corto de la comunidad
        this.txtCommunityShortName.off().on("keyup", function(){
            // No permitir caracteres extraños ni mayúsculas
            $(this).val(this.value.replace(/[^a-zA-Z0-9 _ -]/g, '').trim());
            $(this).val(this.value.toLowerCase());
            // Mostrar error si hay menos de 3 caracteres
            comprobarInputNoVacio($(this), true, false, "El nombre corto de la comunidad debe contener al menos 3 caracteres en letras minúsculas.", 2);

            that.handleKeyUpCommunityShortName();
        });

        // Botón para guardar el nombre corto de la comunidad (modal)
        this.btnGuardarNombreCortoComunidad.off().on("click", function(){
            that.handleChangeCommunityShortName();
        });        


        // RadioButton para el tipo de cambio de comunidad
        $(`input[name="${this.rbCommunityTypeName}"]`).off().on("change", function(){
            that.communityTypeValueSelected = $(this).data("value");
            that.handleShowCommunityTypeDescription();
        });

        // Input para confirmación del tipo de comunidad
        this.txtConfirmCommunityChangeType.off().on("keyup", function(){                        
            that.handleCheckInputConfirmCommunityTypeChange();
        });

        // Botón para guardar el tipo de comunidad (modal)
        this.btnGuardarTipoComunidad.off().on("click", function(){
            that.handleChangeCommunityType();
        });
        
        /* Guardar todo */
        // Botón para guardar todos los datos de Información General        
        that.btnGuardarInformacionGeneral.off().on("click", function(){
            // Comprobar posibles errores de Nombre y Descripción
            const error = that.comprobarErroresGuardado();            

            if (error == false){
                // Sin errores, proceder a obtener datos para guardarlos                
                that.obtenerDatos();                                                                  
                // Proceder a realizar el guardado de datos
                that.guardarDatos();                                                            
            }
        });
    },

    /**
     * Lanzar comportamientos u operativas necesarias para el funcionamiento de la sección
     */
    triggerEvents: function(){
        const that = this;
        // Inicializar operativa multiIdioma
        operativaMultiIdioma.init(that.operativaMultiIdiomaParams);
        // Inicializar la operativa desde la vista ImagenCabecera.cshtml
        operativaJrop.init();
        // Inicializar la operativa para Favicon
        // De momento la vista estará oculta, evitamos posibles errores
        if (this.fileUploadChangeImgFavicon.length > 0){
            this.initFaviconDropArea();
        }
        // Cargar el posible mensaje según el estado de la comunidad
        that.handleChangeEstadoComunidad(that.cmbHiddenEstadoComunidad.val());

        // Inicializar el estado de paneles para cambio de tipo de comunidad
        that.handleShowCommunityTypeDescription();
    },


    /**
     * Método para controlar el valor introducido del nuevo nombre corto de la comunidad para validar si los datos introducidos son válidos
     */
    handleKeyUpCommunityShortName: function(){
        const that = this;

        const newCommunityShortName = this.txtCommunityShortName.val().trim();

        if (newCommunityShortName.length > 2 && newCommunityShortName != this.currentCommunityShortName.trim()){
            // Permitir el cambio de nombre corto de la comunidad
            // Mostrar el panel alerta           
            that.alertConfirmChangeShortName.fadeIn("slow", function() {
                $(this).removeClass("d-none");
            });
            // Quitar la opción de "disabled" en el botón
            setTimeout(function() {                
                that.btnGuardarNombreCortoComunidad.prop("disabled", false);
            }
            ,3000);
        }else{
            // No permitir el cambio de nombre corto de la comunidad
            // Quitar el panel de alerta            
            that.alertConfirmChangeShortName.fadeOut("slow", function() {
                $(this).addClass("d-none");
            });
            // Añadir la opción de "disabled"
            that.btnGuardarNombreCortoComunidad .prop("disabled", true);
        }  
    },

    /**
     * Método para acometer el cambio del nombre corto de la comunidad
     */
     
     handleChangeCommunityShortName: function(){
        
        const that = this;
        
        that.urlGuardarCambioNombreCortoComunidad = this.btnGuardarNombreCortoComunidad.data("url");
        that.oldCommunityShortName = this.currentCommunityShortName.trim();
        that.newCommunityShortName = that.txtCommunityShortName.val().trim();

        // Realizar el cambio del nombre corto de la comunidad
        // Mostrar loading
        loadingMostrar();

        const communityModel = {
            OldShortName: that.oldCommunityShortName,
            NewShortName: that.newCommunityShortName,
        };

        // Proceder a cambiar el tipo Comunidad
        GnossPeticionAjax(
            that.urlGuardarCambioNombreCortoComunidad,
            communityModel,
            true
        ).done(function (data) {
            // Guardado OK
            mostrarNotificacion("success", "Los cambios se han guardado correctamente");
            // Asignar el nuevo nombre corto de la comunidad
            that.lblNombreCortoComunidad.html( that.newCommunityShortName); 
            that.lblNombreCortoComunidad.data("shortname", that.newCommunityShortName);         
            setTimeout(function () {
                dismissVistaModal(that.modalChangeCommunityShortName);
                location.reload();
            }, 500);            
            
        }).fail(function (data) {
            // Guardado KO            
            mostrarNotificacion("error", "Se han producido un error al cambiar el nombre corto de la comunidad. Contacta con el administrador.");            
        }).always(function () {            
            loadingOcultar();
        });        
    },    



    /**
     * Método para acometer el cambio de comunidad
     */
    handleChangeCommunityType: function(){
        
        const that = this;
        // Realizar el cambio de comunidad si el tipo elegido es diferente al actual
        if(this.currentCommunityTypeValue == this.communityTypeValueSelected){
            mostrarNotificacion("error","Para cambiar el tipo de comunidad es necesario elegir uno diferente al actual.");
            return;
        }

        that.urlGuardarCambioTipoComunidad = that.btnGuardarTipoComunidad.data("url");
        that.communityShortName = that.btnGuardarTipoComunidad.data("communityshortname");

        // Realizar el cambio de la comunidad
        // Mostrar loading
        loadingMostrar();

        const communityModel = {
            CommunityShortName: that.communityShortName,
            AccessType: that.communityTypeValueSelected,
        };

        // Proceder a cambiar el tipo Comunidad
        GnossPeticionAjax(
            that.urlGuardarCambioTipoComunidad,
            communityModel,
            true
        ).done(function (data) {
            // Guardado OK
            mostrarNotificacion("success", "Los cambios se han guardado correctamente");            
            setTimeout(function () {
                dismissVistaModal(that.modalChangeCommunityType);
                location.reload();
            }, 500);            
            
        }).fail(function (data) {
            // Guardado KO            
            mostrarNotificacion("error", "Se han producido un error al cambiar el tipo de comunidad. Contacta con el administrador.");            
        }).always(function () {            
            loadingOcultar();
        });        
    },

    /**
     * Método para comprobar las teclas pulsadas que coincidan con lo deseado para habilitar o no el botón de "Guardar" el cambio de tipo de comunidad
     */
    handleCheckInputConfirmCommunityTypeChange: function(){
        const that = this;
        // Valor del input
        const inputValue = this.txtConfirmCommunityChangeType.val().trim();

        if (inputValue == that.currentCommunityNameValue){
            // Mostrar el panel alerta           
            that.alertConfirmChangeCommunityType.fadeIn("slow", function() {
                $(this).removeClass("d-none");
            });
            // Quitar la opción de "disabled" en el botón
            setTimeout(function() {                
                that.btnGuardarTipoComunidad.prop("disabled", false);
            }
            ,3000);
        }else{
            // Quitar el panel de alerta            
            that.alertConfirmChangeCommunityType.fadeOut("slow", function() {
                $(this).addClass("d-none");
            });
            // Añadir la opción de "disabled"
            that.btnGuardarTipoComunidad .prop("disabled", true);
        }   
    },    

    /**
     * Método para mostrar u ocultar paneles descriptivos según el tipo de comunidad pulsado     
     */
    handleShowCommunityTypeDescription: function(){
        const that = this;
        // Ocultar todos los paneles de descripción del tipo de comunidad
        for (let index = 0; index < 3; index++) {
            $(`#${this.panParrafoAccesoId}${index}`).addClass("d-none");
        }
        // Mostrado del panel correcto 
        that.communityTypeValueSelected = $(`input[name="${this.rbCommunityTypeName}"]:checked`).data("value");        
        $(`#${this.panParrafoAccesoId}${that.communityTypeValueSelected}`).removeClass("d-none")
    },


    /**
     * Funcionalidad a ejecutar cuando se haga subido la imagen de la cabecera mediante la funcionalidad dropArea
     * @param {HtmlString} htmlResponse : Respuesta que devuelve el método al subir la imagen de la cabecera
     * @param {Bool} success : Indica si la operación ha sido correcta o errónea.
     */
    onCompletionImageUploaded: function(htmlResponse, success){
        const that = operativaInformacionGeneral;
        if (success == true){
            // Subida de imagen de cabecera correcta
            // Ocultar contenedor para arrastrar imagenes
            that.contenedorImagenCabecera.addClass("d-none");
            // Mostrar panel de Imagenes JDROP + Acciones a realizar (Cambiar / Eliminar)
            that.contenedorImagenesCabeceraJcrop.removeClass("d-none"); 
            // Pintar en el panel la vista devuelta           
            that.panelImagenCabecera.html(htmlResponse);
            // Reiniciar  operativaJrop para las nuevas imágenes cargadas
            operativaJrop.init();
            // Reiniciar eventos para botones de las vistas nuevas añadidas
            that.configEvents();            
        }else{
            // Subida de la imagen de cabecera incorrecta o con errores.
            if (htmlResponse.length > 1){
                // Mostrar el error
                mostrarNotificacion("error", htmlResponse);
            }
        }
    },

    /**
     * Funcionalidad a ejecutar cuando se haga subido la imagen de la cabecera mediante la funcionalidad dropArea
     * @param {HtmlString} htmlResponse : Respuesta que devuelve el método al subir la imagen de la cabecera
     * @param {Bool} success : Indica si la operación ha sido correcta o errónea.
     */
     onCompletionImageFaviconUploaded: function(htmlResponse, success){
        const that = operativaInformacionGeneral;
        if (success == true){
            // Subida de imagen de favicon correcto
            // Ocultar contenedor para arrastrar imagenes
            that.contenedorImagenFavicon.addClass("d-none");
            // Mostrar panel de Imagenes JDROP + Acciones a realizar (Cambiar / Eliminar)
            that.contenedorImagenesFaviconJcrop.removeClass("d-none"); 
            // Pintar en el panel la vista devuelta           
            that.panelImagenFavicon.html(htmlResponse);
            // Reiniciar  operativaJrop para las nuevas imágenes cargadas
            operativaJropFavicon.init();
            // Reiniciar eventos para botones de las vistas nuevas añadidas
            that.configEvents();            
        }else{
            // Subida de la imagen favicon incorrecta o con errores.
            if (htmlResponse.length > 1){
                // Mostrar el error
                mostrarNotificacion("error", htmlResponse);
            }
        }
    },    

    /**
     * Inicializar el área del drop para poder cargar imágenes
     */
    initImageDropArea: function(){
        const that = this;
        // Inicializar plugin al dropArea para la cabecera
        this.dropareaImagenCabecera.imageDropArea({urlUploadImage: that.urlSubirImagenCabecera,
            urlUploadImageType: "fileUpload",
            panelAccionesImagen: that.panelAccionesImagenCabecera,
            contenedorImagen: that.contenedorImagenCabecera,
            previewImg: that.previewImg,
            inputHiddenImageLoaded: that.imageHeadSrc,
            panelVistaContenedor: that.panelImagenCabecera,
            completion: that.onCompletionImageUploaded,
        });
    },

    /**
     * Inicializar el área del drop para poder cargar imágenes para el Favicon
     */
     initFaviconDropArea: function(){
        const that = this;
        // Inicializar plugin al dropArea 
        this.dropareaImagenCabecera.imageDropArea({urlUploadImage: that.urlSubirImagenFavicon,
            urlUploadImageType: "fileUpload",
            panelAccionesImagen: that.panelAccionesImagenFavicon,
            contenedorImagen: that.contenedorImagenFavicon,
            previewImg: that.previewImgFavicon,
            inputHiddenImageLoaded: that.imageFaviconSrc,
            panelVistaContenedor: that.panelImagenFavicon,
            completion: that.onCompletionImageFaviconUploaded,
        });
    },    

    /**
     * Gestionar el cambio de estado de la comunidad
     * @param {String} element : El valor del estado que se ha seleccionado o cargado por el Select de "Estado de la comunidad"
     */
    handleChangeEstadoComunidad: function(estadoComunidad){
        const that = this;
        const estadoComunidadString =  that.selectEstadoComunidad.children("option:selected")[0].label;
        // No mostrar por defecto ninguno
        that.panelesEstadoComunidad.forEach(function(panel){panel.addClass("d-none");});
                
        // Controlar información a mostrar del estado de la comunidad
        switch(parseInt(estadoComunidad)) {
            case 1: // Cerrado temporalmente
              that.panelEstadoComunidadCerradoTemporalmente.removeClass("d-none");
              break;
            case 3: // Abierto
            that.panelEstadoComunidadAbierto.removeClass("d-none");
              break;
            case 4: // Cerrándose
                that.panelEstadoComunidadCerrandose.removeClass("d-none");
             break;              
            default: // 0 -> Cerrado
              break;
        }
        // Establecer el input en el campo oculto para envío de datos
        that.cmbHiddenEstadoComunidad.val(estadoComunidad);
        that.cmbHiddenEstadoComunidad.attr('data-estadocomunidadvalue', estadoComunidad);
        that.cmbHiddenEstadoComunidad.attr('data-estadocomunidadstring', estadoComunidadString);
    },

    /**
     * Abrir comunidad que está en definición
     */
    handleAbrirComunidad: function(){
        const that = this;
        // Mostrar loading
        loadingMostrar();
        // Realizar la petición para abrir la comunidad en definición
        GnossPeticionAjax(
            that.urlAbrirComunidad,
            null,
            true
        ).done(function (response) {
            // OK            
            // Añadir la respuesta de backEnd al panel informativo de "En definición"
            that.panelEstadoComunidadContenedor.html(response);
            // Mostrar información de que se ha abierto correctamente            
            mostrarNotificacion("success","La comunidad se ha abierto correctamente");
            // Visualizar o no los paneles correspondientes de estado de Comunidad (JS) y recargamos los datos
            this.cmbHiddenEstadoComunidad = $('#cmbEstado');
            that.cmbHiddenEstadoComunidad.val(this.cmbHiddenEstadoComunidad.val());
            that.panelEstadoComunidadAbierto = $("#panelEstadoComunidadAbierto");
            that.panelEstadoComunidadCerrado = $("#panelEstadoComunidadCerrado");
            that.panelEstadoComunidadCerradoTemporalmente = $("#panelEstadoComunidadCerradoTemporalmente");
            that.panelEstadoComunidadCerrandose = $("#panelEstadoComunidadCerrandose");
            that.panelesEstadoComunidad = [that.panelEstadoComunidadAbierto, that.panelEstadoComunidadCerrado, that.panelEstadoComunidadCerradoTemporalmente, that.panelEstadoComunidadCerrandose];

            that.handleChangeEstadoComunidad(that.cmbHiddenEstadoComunidad.val());
        }).fail(function (error) {
            // KO            
            // Mostrar el error al tratar de abrir la comunidad
            mostrarNotificacion("error", error);            
        }).always(function () {            
            loadingOcultar();
        });
    },

    /**
     * Mostrar la fecha final de cierre de la comunidad al modificar select de "Días de gracia"
     */
    handleChangeCambiarFechaFin: function(){
        const that = this;
        const miFecha= new Date();
        miFecha.setDate(miFecha.getDate() + (that.selectDiasGracia.val() * 1));
        const fechaFormato = tiempo.fechaBarras.replace('@1@', miFecha.getDate()).replace('@2@', (miFecha.getMonth()+1)).replace('@3@', miFecha.getFullYear());    
        that.fechaFinalGracia.text(fechaFormato);        
    },

    /***************************************************************/
    //////////////// COMPROBACIÓN Y GUARDADO DE DATOS ///////////////
    /***************************************************************/

    /**
     * Comprobará si hay errores antes de guardar datos en servidor
     * @returns (bool) Si todo es correcto devolverá "false". Si hay un error, devolverá "true"
     */
    comprobarErroresGuardado: function(){
        const that = this;
        // Control de posibles errores
        let error = false ;

        // Comprobar posibles errores de Nombre y Descripción si se usan múltiples Tabs para los inputs        
        if (that.operativaMultiIdiomaParams.useOnlyOneTab == false){
            if (that.comprobarErrorTitulo() || that.comprobarErrorDescripcion()){
                error = true;
            }
        }else{
            // Comprobar posibles errores de Nombre y Descripción si se un único tab de idiomas para todos los inputs
            if (that.comprobarErrorTituloUnicoTab() || that.comprobarErrorDescripcionUnicoTab()){
                error = true;
            }
        }

        return error;
    },

    /**
     * Comprobar que los datos para el título de la comunidad es correcto. Además, colocará los diferentes textos de idiomas en el input correspondiente para
     * su posterior guardado.
     * @returns (bool) Si todo es correcto devolverá "false". Si hay un error, devolverá "true"
     */
    comprobarErrorTitulo: function () {
        const that = this;
        const inputTitulo = that.nombreComunidad;
        const panMultiIdioma = $('#edicion_multiidioma_' + inputTitulo.attr("id"));
        let listaTextos = [];
        if (operativaMultiIdioma.listaIdiomas.length > 1){
            let textoMultiIdioma = "";
            let textoIdiomaDefecto = $('#edicion_' + inputTitulo.attr("id") + '_' + operativaMultiIdioma.idiomaPorDefecto + ' input').val();
            if (textoIdiomaDefecto == null || textoIdiomaDefecto == "") {                
                mostrarNotificacion("error", `El nombre en ${operativaMultiIdioma.listaIdiomas.findValueByKey(operativaMultiIdioma.idiomaPorDefecto)} no puede estar vacío.`);
                return true;
            }

            $.each(operativaMultiIdioma.listaIdiomas, function () {
                let idioma = this.key;
                let textoIdioma = $('#edicion_' + inputTitulo.attr("id") + '_' + idioma + ' input').val();
                if (textoIdioma == null || textoIdioma == "") {
                    textoIdioma = textoIdiomaDefecto;
                }
                else {
                    textoMultiIdioma += textoIdioma + "@" + idioma + "|||";
                }
                listaTextos.push({ "key": idioma, "value": textoIdioma });
            });
            inputTitulo.val(textoMultiIdioma);
        }
                
        return false;
    },

    /**
     * Comprobar que los datos para el título de la comunidad es correcto. Además, colocará los diferentes textos de idiomas en el input correspondiente para
     * su posterior guardado.
     * @returns (bool) Si todo es correcto devolverá "false". Si hay un error, devolverá "true"
     */
     comprobarErrorTituloUnicoTab: function () {
        const that = this;
        const inputTitulo = that.nombreComunidad;
        
        let listaTextos = [];  
        // Comprobar si el campo está vacío      
        let textoMultiIdioma = "";
        let textoIdiomaDefecto = $('#input_' + inputTitulo.attr("id") + '_' + operativaMultiIdioma.idiomaPorDefecto).val();
        if (textoIdiomaDefecto == null || textoIdiomaDefecto == "") {                
            mostrarNotificacion("error", `El nombre en ${operativaMultiIdioma.listaIdiomas.findValueByKey(operativaMultiIdioma.idiomaPorDefecto)} no puede estar vacío.`);
            return true;
        }
        
        // Recorrer los datos de los inputs para guardarlos  
        $.each(operativaMultiIdioma.listaIdiomas, function () {
            let idioma = this.key;
            //let textoIdioma = $('#edicion_' + inputTitulo.attr("id") + '_' + idioma + ' input').val();
            let textoIdioma = $('#input_' + inputTitulo.attr("id") + '_' + idioma).val();
            if (textoIdioma == null || textoIdioma == "") {
                textoIdioma = textoIdiomaDefecto;
            }
            else {
                textoMultiIdioma += textoIdioma + "@" + idioma + "|||";
            }
            listaTextos.push({ "key": idioma, "value": textoIdioma });
        });

        if (operativaMultiIdioma.listaIdiomas.length > 1){            
            inputTitulo.val(textoMultiIdioma);
        }else{
            inputTitulo.val(textoIdiomaDefecto);            
        }
        
        return false;
    },    

    /**
     * Comprobar que los datos para la descripción de la comunidad es correcto. Además, colocará los diferentes textos de idiomas en el input correspondiente para
     * su posterior guardado.
     * @returns (bool) Si todo es correcto devolverá "false". Si hay un error, devolverá "true"
     */
    comprobarErrorDescripcion: function () {
        var that = this;
        var inputDescription = that.descripcionComunidad;
        var panMultiIdioma = $('#edicion_multiidioma_' + inputDescription.attr("id"));
        var listaTextos = [];
        if (operativaMultiIdioma.listaIdiomas.length > 1 && panMultiIdioma.length > 0) {
            let textoMultiIdioma = "";            
            let textoIdiomaDefecto = $('#edicion_' + inputDescription.attr("id") + '_' + operativaMultiIdioma.idiomaPorDefecto + ' textarea').val();                  
            if (textoIdiomaDefecto == null || textoIdiomaDefecto == "") {                
                mostrarNotificacion("error", `La descripción en ${operativaMultiIdioma.listaIdiomas.findValueByKey(operativaMultiIdioma.idiomaPorDefecto)} no puede estar vacía.`);
                return true;
            }

            $.each(operativaMultiIdioma.listaIdiomas, function () {
                var idioma = this.key;
                var textoIdioma = $('#edicion_' + inputDescription.attr("id") + '_' + idioma + ' textarea').val();
                if (textoIdioma == null || textoIdioma == "") {
                    textoIdioma = textoIdiomaDefecto;
                }
                else {
                    textoMultiIdioma += textoIdioma + "@" + idioma + "|||";
                }
                listaTextos.push({ "key": idioma, "value": textoIdioma });
            });
            inputDescription.val(textoMultiIdioma);
        }
        return false;
    },

    /**
     * Comprobar que los datos para la descripción de la comunidad es correcto. Además, colocará los diferentes textos de idiomas en el input correspondiente para
     * su posterior guardado.
     * @returns (bool) Si todo es correcto devolverá "false". Si hay un error, devolverá "true"
     */
     comprobarErrorDescripcionUnicoTab: function () {
        var that = this;
        var inputDescription = that.descripcionComunidad;        
        var listaTextos = [];
        // Comprobar si el campo está vacío 
        let textoMultiIdioma = "";
        let textoIdiomaDefecto = $('#input_' + inputDescription.attr("id") + '_' + operativaMultiIdioma.idiomaPorDefecto).val();
        if (textoIdiomaDefecto == null || textoIdiomaDefecto == "") {    
            mostrarNotificacion("error", `La descripción en ${operativaMultiIdioma.listaIdiomas.findValueByKey(operativaMultiIdioma.idiomaPorDefecto)} no puede estar vacía.`);
            return true;
        }

        // Recorrer los datos de los inputs para guardarlos  
        $.each(operativaMultiIdioma.listaIdiomas, function () {
            var idioma = this.key;                
            let textoIdioma = $('#input_' + inputDescription.attr("id") + '_' + idioma).val()
            if (textoIdioma == null || textoIdioma == "") {
                textoIdioma = textoIdiomaDefecto;
            }
            else {
                textoMultiIdioma += textoIdioma + "@" + idioma + "|||";
            }
            listaTextos.push({ "key": idioma, "value": textoIdioma });
        });
        inputDescription.val(textoMultiIdioma);
        
        return false;
    },    


    /**
     * Recopilación de datos y asignación al objeto Options para enviarlos a backEnd para su guardado
     */
    obtenerDatos: function () {
        const that = this;
        // Estado
        that.Options['State.State'] = that.cmbHiddenEstadoComunidad.val();  //$("#cmbEstado").val();
        that.Options['State.ReOpenDate'] = that.inputFechaReapertura.val(); // $("#txtFechaReapertura").val();
        that.Options['State.CauseOfClose'] = encodeURIComponent(that.inputMotivoCierre.val()); // encodeURIComponent($("#txtMotivoCierre").val());
        that.Options['State.DaysOfGrace'] = that.selectDiasGracia.val(); // $("#ddlDiasGracia").val();
        // Nombre y Descripción
        that.Options['Name'] = that.nombreComunidad.val(); // $("#txtNombre").val();
        that.Options['Desciption'] = encodeURIComponent(that.descripcionComunidad.val()); //encodeURIComponent($("#txtDescripcion").val());
        // Banner de la comunidad
        that.Options['ImageHead.Ruta'] = that.imagenHeadRuta.val(); // $("#ImageHead_src").val();
        that.Options['ImageHead.Pos_X_0'] = that.imagenHeadPost_X_0.val(); // $("#ImageHead_Pos_X_0").val();
        that.Options['ImageHead.Pos_Y_0'] = that.imagenHeadPost_Y_0.val(); // $("#ImageHead_Pos_Y_0").val();
        that.Options['ImageHead.Pos_X_1'] = that.imagenHeadPost_X_1.val(); // $("#ImageHead_Pos_X_1").val();
        that.Options['ImageHead.Pos_Y_1'] = that.imagenHeadPost_Y_1.val(); // $("#ImageHead_Pos_Y_1").val();
        that.Options['ImageHead.Alto'] = that.imagenHeadAlto.val(); // $("#ImageHead_Alto").val();
        that.Options['ImageHead.Ancho'] = that.imagenHeadAncho.val(); // $("#ImageHead_Ancho").val();
 
        // Logo de la comunidad (no hace falta)
         that.Options['ImageLogo.Ruta'] = ''; // $("#ImageLogo_src").val();
         that.Options['ImageLogo.Pos_X_0'] = '0'; // $("#ImageLogo_Pos_X_0").val();
         that.Options['ImageLogo.Pos_Y_0'] = '0'; // $("#ImageLogo_Pos_Y_0").val();
         that.Options['ImageLogo.Pos_X_1'] = '0'; // $("#ImageLogo_Pos_X_1").val();
         that.Options['ImageLogo.Pos_Y_1'] = '0'; // $("#ImageLogo_Pos_Y_1").val();

        // Tags de la comunidad
        that.Options['Tags'] = that.inputTagsComunidad.val().trim();

        // Categorías de la comunidad
        var categoriasSeleccionadas = "";
        $("#divSelCatTesauro").find('input[type=checkbox]').each(function () {
            if ($(this).is(':checked')) {
                categoriasSeleccionadas += $(this).attr('id') + ",";
            }
        });       
        that.Options['SelectedCategories'] = categoriasSeleccionadas.split(',');

        // MultiIdioma
        that.Options['MultiLanguage'] = that.chkIdioma.is(':checked');
        that.Options['IdiomaPorDefecto'] = this.selectMultiIdioma.val();
    },

    /**
     * Método para guardar los datos. Se enviarán a backEnd para su guardado en BD.
     */
    guardarDatos: function(){
        const that = this;
        const urlGuardarDatos = `${that.urlBase}/save`;

        // Mostrar loading
        loadingMostrar();

        // Proceder a guardar los datos
        GnossPeticionAjax(
            urlGuardarDatos,
            that.Options,
            true
        ).done(function (data) {
            // Guardado OK
            mostrarNotificacion("success", "Los cambios se han guardado correctamente");
        }).fail(function (data) {
            // Guardado KO
            const error = data.split('|||');
            mostrarNotificacion("error", "Se han producido errores durante el guardado.");            
        }).always(function () {            
            loadingOcultar();
        });
    },  
}


/**
 * Operativa de funcionamiento de Tipo de Contenidos y Permisos
 */
const operativaContenidosYPermisos = {

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
                
        // Elementos del DOM

        /* Checkbox de selección */
        // Checkbox nivel Usuario
        this.chkUsuario = $('.chkUsu');
        this.chkClaseUsuario = 'chkUsu';
        this.checkUsuarioPlantilla = $('#checkUsuarioPlantilla');
        // Checkbox nivel Usuario para seleccionar Todos
        this.chkUsuarioTodos = $('#chkOntoUsu_todos');
        // Checkbox nivel Supervisor
        this.chkSupervisor = $('.chkSup');
        this.chkClaseSupervisor = 'chkSup';
        this.checkSupervisorPlantilla = $('#checkSupervisorPlantilla');
        // Checkbox nivel Supervisor para seleccionar Todos
        this.chkSupervisorTodos = $('#chkOntoSup_todos');
        this.checkAdministradorPlantilla = $('#checkAdministradorPlantilla');
        // Checkbox nivel Administrador
        this.chkAdministrador = $('.chkAdm');
        this.chkClaseAdministrador = 'chkAdm';
        // Checkbox nivel Administrador para seleccionar Todos
        this.chkAdministradorTodos = $('#chkOntoAdm_todos');
        // Checkbox para permitir descargar adjuntos a usuarios invitados
        this.chkPermitirUsuNoLoginDescargDoc = $('#chkPermitirUsuNoLoginDescargDoc');
        /* Modal de buscar grupos */   
        // Referencia al modal
        this.modalContainer = $("#modal-container");
        // Referencia del disparador del modal (Ej: El botón que ha lanzado Añadir/Editar grupos ). Se establecerá en el 'show.bs.modal' del modal
        this.triggerModalContainer = '';
        // Variable que se guardará a modo de atributo pasado del disparador del modal. De momento vacío. Se establece en 'show.bs.modal' del modal
        this.inputHiddenGruposHack = '';
        // Variables para guardar los nombres e ids de los grupos actuales (grupos a editar desde la página principal una vez se pulse en el botón para abrir el modal)
        this.idGroups = '';
        this.groupNames = '';        
        // Input tipo texto para buscar grupos desde el modal. Debe estar conectado con un servicio autocomplete
        this.txtBuscarGrupos = $('#txtBuscarGrupos');
        this.txtIdBuscarGrupos = "txtBuscarGrupos";     
        // Input oculto donde se añadirán los ids los grupos seleccionados buscados
        this.txtBuscarGrupos_Hack = $('#txtBuscarGrupos_Hack');
        this.txtIdBuscarGrupos_Hack = 'txtBuscarGrupos_Hack';
        // Input oculto donde se añadirán los nombres de los grupos seleccionados buscados
        this.txtBuscarGruposNombres_Hack = $('#txtBuscarGruposNombres_Hack');
        this.txtIdBuscarGruposNombres_Hack = 'txtBuscarGruposNombre_Hack';        
        // Contendor de los grupos seleccionados dentro del modal
        this.tagsContainer = $("#tagsContainer");
        this.idTagsContainer = "tagsContainer";
        // Botón para borrar grupo desde el modal
        this.btnClaseBorrarGrupoTagModal = 'borrarGrupoTagModal';
        // Botón para guardar grupos desde el modal
        this.btnGuardarAddGrupos = $('#btnGuardarAddGrupos'); 
        // Id del Botón para guardar grupos desde el modal
        this.idBtnGuardarAddGrupos = 'btnGuardarAddGrupos'; 
        /* Guardado de la pestaña */ 
        // Botón para guardar
        this.btnGuardarContenidosYPermisos = $('#btnGuardarContenidosYPermisos');       
        // Url base para realizar acciones en Backend (Ej: Guardado de datos)
        this.urlBase = `${location.href}`;        
        // Objeto de datos donde se guardará lo recopilado en la página para envío a backend
        this.DatosGuardado = {};
    },

    /**
     * Configuración de eventos de elementos del Dom (Botones, Inputs...)     
     */
    configEvents: function (pParams) {
        const that = this;

        // Click en un check de tipo Usuario
        this.chkUsuario.on("click", function () {
            // Fila correspondiente al check pulsado
            const fila = $(this).closest('tr');
            if ($(this).is(':checked')) {
                $(`.${that.chkClaseAdministrador}`, fila).prop('checked', true);
                $(`.${that.chkClaseSupervisor}`, fila).prop('checked', true);
            }
        });

        // Click en el check de usuario plantilla de oc
        this.checkUsuarioPlantilla.on("click", function () {
            if (!$(this).is(':checked')) {
                
                if (that.chkUsuarioTodos.is(':checked')) {
                    that.chkUsuarioTodos.trigger('click');
                }
                that.deshabilitarChkOntoTodos($(this));
            }
            else {
                that.habilitarChkOntoTodos($(this));
            }
        }); 
        
        // Click en el check de Supervisor de plantilla de OC        
        this.checkSupervisorPlantilla.on("click", function () {
            if (!$(this).is(':checked')) {
                if ($('#chkOntoSup_todos').is(':checked')) {
                    $('#chkOntoSup_todos').trigger('click');
                }
                that.deshabilitarChkOntoTodos($(this));
            }
            else {
                that.habilitarChkOntoTodos($(this));
            }       
        }); 

        // Click en el check de Supervisor de plantilla de OC        
        this.checkAdministradorPlantilla.on("click", function () {
            if (!$(this).is(':checked')) {
                if ($('#chkOntoAdm_todos').is(':checked')) {
                    $('#chkOntoAdm_todos').trigger('click');
                }
                that.deshabilitarChkOntoTodos($(this));
            }
            else {
                that.habilitarChkOntoTodos($(this));
            }       
        });                 
        
        // Click en check de tipo Usuario "TODOS"
        this.chkUsuarioTodos.on("click",function () {
            if (!$(this).is(':checked')) {
                that.desmarcarChkOntoTodos($(this));
            }
            else {
                that.marcarChkOntoTodos($(this));
            }
        });

        // Click en un check de tipo Supervisor
        this.chkSupervisor.on("click", function () {
            // Fila correspondiente al check pulsado
            const fila = $(this).closest('tr');
            if ($(this).is(':checked')) {
                $(`.${that.chkClaseAdministrador}`, fila).prop('checked', true);                
            }else{                
                $(`.${that.chkClaseUsuario}`, fila).prop('checked', false);                
            }
        });

        // Click en check de tipo Supervisor "TODOS"
        this.chkSupervisorTodos.on("click", function () {
            if (!$(this).is(':checked')) {
                that.desmarcarChkOntoTodos($(this));
            }
            else {
                that.marcarChkOntoTodos($(this));
            }
        });

        // Click en un check de tipo Administrador
        this.chkAdministrador.on("click", function () {
            // Fila correspondiente al check pulsado
            const fila = $(this).closest('tr');
            if (!$(this).is(':checked')) {
                $(`.${that.chkClaseSupervisor}`, fila).prop('checked', false);
                $(`.${that.chkClaseUsuario}`, fila).prop('checked', false);
            }
        }); 
        
        // Click en check de tipo Administrador "TODOS"      
        this.chkAdministradorTodos.on("click", function () {
            if (!$(this).is(':checked')) {
                that.desmarcarChkOntoTodos($(this));
            }
            else {
                that.marcarChkOntoTodos($(this));
            }
        }); 
                
        this.modalContainer.on('show.bs.modal', (e) => {
            // Aparición del modal
            // Conocer el botón disparador del modal para coger atributos necesarios. En este caso, el id del input oculto de los grupos para usarlo en el modal al pulsar en "Guardar"
            that.inputHiddenGruposHack = $(e.relatedTarget).data("inputhack");
            // Ids y nombre de los grupos actuales obtenidos a partir del botón que ha disparado el modal
            that.idGroups = $(e.relatedTarget).data("idgroups");
            that.groupNames = $(e.relatedTarget).data("groupnames");
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

        // Botón para guardar toda la información relativa a Permisos
        this.btnGuardarContenidosYPermisos.on("click", function(){
            // Obtener datos y guardado en objeto para posterior envío al servidor
            that.obtenerDatosGuardado();
            that.guardarContenidosYPermisos();            
        });

    },

    deshabilitarChkOntoTodos: function(checkInput){
        const that = this;
        if (checkInput.hasClass('chkAdm')) {
            checks = $('.chkOntoAdm, .chkOntoSup, .chkOntoUsu, #chkOntoAdm_todos, #chkOntoSup_todos, #chkOntoUsu_todos');
        } else if (checkInput.hasClass('chkSup')) {
            checks = $('.chkOntoSup, .chkOntoUsu, #chkOntoSup_todos, #chkOntoUsu_todos');
        } else if (checkInput.hasClass('chkUsu')) {
            checks = $('.chkOntoUsu, #chkOntoUsu_todos');
        }
        for (var i = 0; i < checks.length; i++) {
            $(checks[i]).attr('disabled', true);
        }
        that.checkearMostrarGruposOnto();
    },

    habilitarChkOntoTodos: function(checkInput){
        if (checkInput.hasClass('chkAdm')) {
            checks = $('.chkOntoAdm, #chkOntoAdm_todos');
        } else if (checkInput.hasClass('chkSup')) {
            checks = $('.chkOntoAdm, .chkOntoSup, #chkOntoAdm_todos, #chkOntoSup_todos');
        } else if (checkInput.hasClass('chkUsu')) {
            checks = $('.chkOntoAdm, .chkOntoSup, .chkOntoUsu, #chkOntoAdm_todos, #chkOntoSup_todos, #chkOntoUsu_todos');
        }
        for (var i = 0; i < checks.length; i++) {
            $(checks[i]).attr('disabled', false);
        }
    },

    /**
     * Método que desmarcará los checkbox cuando se haya pulsado sobre "Todos"
     * @param {checkbox} checkInput 
     */
    desmarcarChkOntoTodos: function (checkInput) {
        const that = this;
        if (checkInput.hasClass('chkAdm')) {
            checks = $('.chkOntoAdm, .chkOntoSup, .chkOntoUsu');
        } else if (checkInput.hasClass('chkSup')) {
            checks = $('.chkOntoSup, .chkOntoUsu');
        } else if (checkInput.hasClass('chkUsu')) {
            checks = $('.chkOntoUsu');
        }
        for (var i = 0; i < checks.length; i++) {
            $(checks[i]).prop('checked', false);
        }
        that.checkearMostrarGruposOnto();
    },

    /**
     * Método que marcará los checkbox cuando se haya pulsado sobre "Todos"
     * @param {checkbox} checkInput 
     */
    marcarChkOntoTodos: function (checkInput) {
        var that = this;
        if (checkInput.hasClass('chkAdm')) {
            checks = $('.chkOntoAdm');
        } else if (checkInput.hasClass('chkSup')) {
            checks = $('.chkOntoAdm, .chkOntoSup');
        } else if (checkInput.hasClass('chkUsu')) {
            checks = $('.chkOntoAdm, .chkOntoSup, .chkOntoUsu');
        }
        for (var i = 0; i < checks.length; i++) {
            $(checks[i]).prop('checked', true);
        }
        that.checkearMostrarGruposOnto();
    },

    /**
     * Método para ocultar la sección de 'Añadir grupos' ya que se ha seleccionado no desear permisos a nadie.Llamado desde 'desmarcarChkOntoTodos'
     * @param {*} param0 
     */
    checkearMostrarGruposOnto: function(){
        const checksUsu = $('.chkOntoUsu');
        for (var i = 0; i < checksUsu.length; i++) {
            var fila = $(checksUsu[i]).closest('tr');
            if ($(checksUsu[i]).is(':checked')) {
                $('.contenedorGrupos').addClass("d-none");
            }
            else {
                $('.contenedorGrupos').removeClass("d-none");
            }
        }        
    },


    // Configurar elementos relativos a la operativa de buscar y añadir grupos al modal. Se configurará cada vez que se lance o aparezca el modal
    operativaModalBuscarGrupos: function(){
        const that = this;

        // Configurar servicio autocompletar para "Buscar grupos" desde el modal.
        configEventById(that.txtIdBuscarGrupos, function(element){
            // Vista cargada -> Evitar el "Node cannot be found in the current page."
            // that.config(that.pParams);
            const $jqueryElement = $(element);
            // Asignación comportamiento autocomplete            
            $jqueryElement.autocomplete(
                null,
                {
                    url: $('#inpt_urlServicioAutocompletar').val() + "/AutoCompletarLectoresEditores",
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
                        proyecto: $('#inpt_proyID').val(),
                        bool_edicion: 'true',
                        bool_traergrupos: 'true',
                        bool_traerperfiles: 'false'
                    }
                }
            );            
        }); 

        // Método a ejecutar cuando se seleccione un item de resultados de autocompletar
        $(`#${that.txtIdBuscarGrupos}`).result(function (event, data, formatted) {
            // Gestionar el click realizado del autocomplete
            that.handleSeleccionarGrupoBuscado(this, data[0], data[1]);        
        });


        // Botón para poder eliminar grupos desde el modal                        
        configEventByClassName(`${that.btnClaseBorrarGrupoTagModal}`, function(element){
            const $jqueryElement = $(element);
            $jqueryElement.off().on("click", function(){ 
                const $grupoEliminado = $jqueryElement.parent().parent();                                                                              
                that.handleClickBorrarGrupoTagModal($grupoEliminado);
            });	                        
        });
          
        
        // Botón de guardar grupos del modal y cerrarlo
        $(`#${that.idBtnGuardarAddGrupos}`).on("click", function(){            
            that.handleClickGuardarAddGroups();
        });

        // Inicializar pintado de grupos iniciales en el modal (En caso de necesitar editar)
        that.pintarGruposInicio();
    },

    /**
     * Lanzar comportamientos u operativas necesarias para el funcionamiento de la sección
     */
    triggerEvents: function(){
        const that = this;                           
    },

    /**
     * Método para "guardar" o asignar los grupos a un determinado tipo de archivo desde el modal
     */
    handleClickGuardarAddGroups: function(){
        const that = this;
        // Input hidden del grupo de la página index donde hay que añadir los grupos seleccionados
        const $inputHiddenGruposHack = $(`#${that.inputHiddenGruposHack}`);
        // Añadir los inputs de los grupos ids a la lista de items
        $inputHiddenGruposHack.val($(`#${that.txtIdBuscarGrupos_Hack}`).val());
        // Añadir los ids de los grupos a la propiedad del botón lanzador del modal
        that.triggerModalContainer.data("groupnames", $(`#${that.txtIdBuscarGruposNombres_Hack}`).val());
        // Añadir los nombres de los grupos a la propiedad del botón lanzador del modal
        that.triggerModalContainer.data("idgroups", $(`#${that.txtIdBuscarGrupos_Hack}`).val());               

        // Ocultar el modal
        that.modalContainer.modal('hide');
    },

    /**
     * Método que se ejecutará cuando se seleccione un grupo buscado pulsando en un resultado de "autocomplete" desde el modal para ser añadido
     * @param {*} txtautocomp : Input de autocompletar desde el que se ha realizado la búsqueda
     * @param {*} nombre : Nombre del grupo seleccionado
     * @param {*} id : Id del grupo seleccionado
     */
    handleSeleccionarGrupoBuscado: function(txtautocomp, nombre, id){
        const that = this;        
        // Crear el "Tag" o los items para añadirlos al contenedor a modo de "Añadidos"
        that.handlePintarGrupoSeleccionado(nombre, id);
        // Vaciar el input de búsqueda de grupos y establecer el foco
        $(`#${that.txtIdBuscarGrupos}`).val('');        
    },

    /**
     * Eliminar el grupo seleccionado del contenedor de "grupos" dentro del modal
     * Eliminar el valor del input hidden y eliminará el "Tag" del grupo de forma visual.
     * @param {jqueryObject*}: Tag / grupo eliminado
     */
    handleClickBorrarGrupoTagModal: function($pGrupoEliminado){
        const that = this;

        // Buscar el input oculto y seleccionar la propiedad del id que corresponde con el grupo a eliminar
        const idGrupoEliminado = $($pGrupoEliminado.children()[1]).data("id");
        const nombreGrupoEliminado = $($pGrupoEliminado.children()[1]).data("nombre");
        // Buscar el id del grupo y eliminarlo del input
        let gruposActualesId = $(`#${that.txtIdBuscarGrupos_Hack}`).val().split(",");
        // Borrar el item del array y vuelve a construir los grupos separados por comas 
        gruposActualesId.splice( $.inArray(idGrupoEliminado, gruposActualesId), 1 );
        // Pasarle los datos de los grupos actuales al input hidden
        $(`#${that.txtIdBuscarGrupos_Hack}`).val(gruposActualesId.join(","));
        // Buscar el nombre del grupo y eliminarlo del input
        let gruposActualesNombre = $(`#${that.txtIdBuscarGruposNombres_Hack}`).val().split(",");
        // Borrar el item del array y vuelve a construir los grupos separados por comas 
        gruposActualesNombre.splice( $.inArray(nombreGrupoEliminado, gruposActualesNombre), 1 );
        // Pasarle los datos de los grupos actuales al input hidden
        $(`#${that.txtIdBuscarGruposNombres_Hack}`).val(gruposActualesNombre.join(","));
        // Eliminar el grupo del contenedor visual
        $pGrupoEliminado.remove();
    },


    /**
     * Método para pintar los grupos/tags de inicio nada más cargar el modal. Cogerá las propiedades del botón que ha abierto el modal
     * data-idgroups -> Lista de los ids de los grupos
     * data-namegroups -> Lista de los nombres de los grupos
     */
    pintarGruposInicio: function(){
        const that = this;
        // Arrays de los ids y nombres de los grupos actuales
        const listaIdGroups = that.idGroups.split(",");
        const listaGroupNames = that.groupNames.split(",");

        for (let index = 0; index < listaIdGroups.length; index++) {
            const groupId = listaIdGroups[index];
            const groupName = listaGroupNames[index];
            // Pintar la etiqueta correspondiente
            if (groupName != ''){
                // Pintar el Grupo a modo visual / Tag
                that.handlePintarGrupoSeleccionado(groupName, groupId);                
            }
        }
    },

    /**
     * Pintar la "etiqueta" del grupo una vez se ha seleccionado el item del autocomplete
     * @param {String} nombre: Nombre del grupo seleccionado de autocomplete
     * @param {*} id: Id del grupo seleccionado
     */
    handlePintarGrupoSeleccionado: function(nombre, id){
        const that = this;
        // Contendor donde pintar el grupo seleccionado
        const $gruposContainer = $(`#${that.idTagsContainer}`);
        // Plantilla html del grupo seleccionado
        let grupoHtmlTemplate = `
        <div class="tag" title="${nombre}">
            <div class="tag-wrap">
                <span class="tag-text" data-id="${id}">${nombre}</span>
                <span class="tag-remove borrarGrupoTagModal material-icons">close</span>
            </div>
            <input type="hidden" data-nombre="${nombre}" data-id="${id}" value="${nombre}">
        </div>
        `;
        // Añadir el item de forma visual
        $gruposContainer.append(grupoHtmlTemplate);
        
        // Añadir el id del grupo seleccionado al input oculto del modal        
        $(`#${that.txtIdBuscarGrupos_Hack}`).val($(`#${that.txtIdBuscarGrupos_Hack}`).val() + id + ",");         
        // Añadir el nombre del grupo seleccionado al input oculto del modal                
        $(`#${that.txtIdBuscarGruposNombres_Hack}`).val($(`#${that.txtIdBuscarGruposNombres_Hack}`).val() + nombre + ",");             
    },


    /************************************************************************************************************************ */
    // Métodos para guardado de la pestaña Permisos
    /************************************************************************************************************************ */

    /**
     * Construcción del objeto para posteriormente guardarlo mediante petición realizada con el método de "guardarTodo"
     */
    obtenerDatosGuardado: function () {
        const that = this;
        ///// será un item global that.DatosGuardado = {};

        let prefijo = 'DatosGuardado';
        var contPermisos = 0;

        $('#tablaPermisos tbody tr.permiso').each(function () {
            const tr = $(this);
            const tipoDocumento = tr.attr('id');
            let tipoPermiso = -1;

            if ($('.chkUsu', tr).is(':checked')) {
                tipoPermiso = 2;
            }
            else if ($('.chkSup', tr).is(':checked')) {
                tipoPermiso = 1;
            }
            else if ($('.chkAdm', tr).is(':checked')) {
                tipoPermiso = 0;
            }

            if (tipoDocumento == "FicheroServidor") {
                //Si el tipo de documento es fichero servidor, añadimos los tipos imagen y video
                let tipoPermisoFicheroServidor = tipoPermiso;
                let tipoPermisoVideo = tipoPermiso;
                let tipoPermisoImagen = tipoPermiso;
                if ($('input[name="tipoArchivo"]:checked').val() == 'Video') {
                    tipoPermisoFicheroServidor = -1;
                    tipoPermisoImagen = -1;
                }
                else if ($('input[name="tipoArchivo"]:checked').val() == 'Imagen') {
                    tipoPermisoFicheroServidor = -1;
                    tipoPermisoVideo = -1;
                }

                let prefijoClave = prefijo + '.PermisosDocumentacion[' + contPermisos + ']';
                that.DatosGuardado[prefijoClave + '.TipoDocumento'] = 'FicheroServidor';
                that.DatosGuardado[prefijoClave + '.TipoPermiso'] = tipoPermisoFicheroServidor;

                contPermisos++;
                prefijoClave = prefijo + '.PermisosDocumentacion[' + contPermisos + ']';
                that.DatosGuardado[prefijoClave + '.TipoDocumento'] = 'Video';
                that.DatosGuardado[prefijoClave + '.TipoPermiso'] = tipoPermisoVideo;

                contPermisos++;
                prefijoClave = prefijo + '.PermisosDocumentacion[' + contPermisos + ']';
                that.DatosGuardado[prefijoClave + '.TipoDocumento'] = 'Imagen';
                that.DatosGuardado[prefijoClave + '.TipoPermiso'] = tipoPermisoImagen;
            }
            else {
                let prefijoClave = prefijo + '.PermisosDocumentacion[' + contPermisos + ']';
                that.DatosGuardado[prefijoClave + '.TipoDocumento'] = tipoDocumento;
                that.DatosGuardado[prefijoClave + '.TipoPermiso'] = tipoPermiso;
            }

            contPermisos++;
        });

        var contPermisos = 0;
        $('#tablaPermisos tbody tr.permisoOnto').each(function () {
            var tr = $(this);
            var permiso = -1;

            if ($('.chkUsu', tr).is(':checked')) {
                permiso = 2;
            }
            else if ($('.chkSup', tr).is(':checked')) {
                permiso = 1;
            }
            else if ($('.chkAdm', tr).is(':checked')) {
                permiso = 0;
            }

            let prefijoClave = prefijo + '.PermisosDocumentacionSemantica[' + contPermisos + ']';
            that.DatosGuardado[prefijoClave + '.Ontologia'] = tr.attr('id') + ".owl";
            that.DatosGuardado[prefijoClave + '.TipoPermiso'] = permiso;
            that.DatosGuardado[prefijoClave + '.TipoDocumento'] = $(this).find(".nameOnto").html();
            let privacidadGrupos = tr.find('#filtroGrupos_' + tr.attr('id') + '_Hack').val().split(',');
            for (let i = 0; i < privacidadGrupos.length; i++) {
                if (privacidadGrupos[i].trim() != "") {
                    let prefijoPrivacidadGrupos = prefijoClave + '.PrivacidadGrupos[' + i + ']';
                    that.DatosGuardado[prefijoPrivacidadGrupos + '.Key'] = privacidadGrupos[i].trim().substr(2);
                    that.DatosGuardado[prefijoPrivacidadGrupos + '.Value'] = "";
                }
            }

            contPermisos++;
        });
        
        // Permitir descargar ficheros adjuntos a usuarios invitados
        that.DatosGuardado[prefijo + '.PermitirDescargarDocUsuInvitado'] = that.chkPermitirUsuNoLoginDescargDoc.is(':checked');
    },

    /**
     * Método para guardar toda la información editada o modificada relativa a los permisos.
     * Se ejecutará una vez se haga click en "Guardar todo"
     */
    guardarContenidosYPermisos: function(){
        const that = this;

        const urlGuardarDatos = `${that.urlBase.replace(location.search, "")}/save`;

        // Mostrar loading
        loadingMostrar();        

        // Proceder a guardar los datos
        GnossPeticionAjax(
            urlGuardarDatos,
            that.DatosGuardado,
            true
        ).done(function (data) {
            // OK 
            mostrarNotificacion("success", "Los cambios se han guardado correctamente");            
        }).fail(function (data) {
            // KO
            const error = data.split('|||');
            if (data != "Contacte con el administrador del Proyecto, no es posible atender la petición.") {
                // Mensaje error guardado 1
                mostrarNotificacion("error", "Ha habido errores en el guardado. Revisa los errores marcados");                
                if (error[0] == "ERROR_NOMBRE_CERTIFICACION_VACIO") {
                    // Mensaje error guardado 2
                    mostrarNotificacion("error", "No puede haber niveles de certificaci&#xF3;n con el nombre en blanco");                    
                }
            }
            else {
                that.mostrarErrorGuardadoFallo(data);
                // Mensaje error guardado 3
                let entornoBloqueado = "False";

                if (entornoBloqueado == "True") {
                    $('input.guardarTodo').before('<div class="error general">El entorno actual esta bloqueado. Hay que desplegar la versión de preproducción antes de hacer cambios</div>');
                    mostrarNotificacion("error", "El entorno actual esta bloqueado. Hay que desplegar la versión de preproducción antes de hacer cambios");
                }
                else {
                    mostrarNotificacion("error", data);                    
                }
            }
        }).always(function () {
            loadingOcultar();
        });
    },
}


/**
 * Operativa de funcionamiento de Gestión de Categorías de la comunidad
 */
 const operativaGestionCategorias = {

    /**
     * Inicializar operativa
     */
    init: function (pParams) {
        this.pParams = pParams;
        this.config(pParams);
        this.configEvents();
        this.triggerEvents();
        this.configRutas();               
    },

    /*
     * Inicializar elementos de la vista
     * */
    config: function (pParams) {
                
        // Elementos del DOM
        // Identificador del modal para confirmar la eliminación de una categoría
        this.modalDeleteCategoryClassName = "modal-confirmDelete";
        /* Elementos Dom listado de categorías */
        // Panel de Tabs con los diferentes idiomas
        this.tabIdiomas = $('#tabIdiomas');
        // Cada opción o Tab con el idioma correspondiente
        this.tabIdiomaItem = $('.tabIdiomaItem');
        // Panel contenedor del tesauro de categorías
        this.panelTesauro = $('#panTesauro');
        // Listado donde se muestran las categorías
        this.communityCategoryListClassName = 'id-added-categories-list';  
        // Cada una de las filas de la categoría que podrá arrastrarse
        this.categoryListItemClassName = 'category-row';
        // Clase del contenedor de los hijos de las categorias
        this.categoryChildrenPanelClassName = "categoryChildrenPanel";        
        // Botón icono para arrastrar items
        this.btnDragCategoryIconClassName = 'sortable-icon';      
        // Botón para añadir categorías
        this.btnAddCategorias = $("#btnAddCategory");
        // Checkbox para permitir crear categorías multilenguage
        this.chkMultiIdioma = $("#chkMultiIdioma");
        // Inputs ocultos para funcionamiento y llamadas a backEnd
        this.txtAccionesTesauroHack = $("#txtAccionesTesauroHack");
        this.txtCategoriasSeleccionadas = $("#txtCategoriasSeleccionadas");
        this.txtCategoriasExpandidas = $("#txtCategoriasExpandidas");
        this.txtGuardarObligatorio = $("#txtGuardarObligatorio");
        this.txtHackIdiomaTesauro = $("#txtHackIdiomaTesauro");
        this.txtHackIdiomaTesauroDefecto = $("#txtHackIdiomaTesauroDefecto");
        // Variable para guardar los parámetros comunes que se utilizarán para realizar peticiones a backend. 
        this.parametrosComunes = {};
        // Botones de acciones de Editar, Crear y Borrar categorías que se encuentran en cada fila de la propia categoría
        this.btnActionEditCategory = $(".action-edit");
        this.btnActionAddCategory = $(".action-add");
        this.btnActionDeleteCategory = $('.action-delete');
        // Botón de confirmación del borrado de la categoría
        this.btnConfirmDeleteCategoryClassName = "btnConfirmDeleteCategory"
        // Botón para plegar o desplegar categorías hijas
        this.btnPlegarDesplegarCategoriasHijas = $('.js-action-collapse');
        // Botones para aceptar o cancelar categorías sugeridas
        this.btnAcceptCategorySuggested = $('.btnAcceptCategorySuggested');
        this.btnRejectCategorySuggested = $('.btnRejectCategorySuggested');
        // Botón para plegar/desplegar categorías
        this.btnShowHideIconClassName = 'showHide-icon';

        /* Creación / Edición / Borrado de Categorías desde el modal */
        // Input del nombre de la categoría. Al ser multilenguaje puede haber varios
        this.inputIdioma = $(".inputIdioma");        
        // Botón para guardar la categoría recién creada desde el modal Crear categoría
        this.btnGuardarCategory = $('#btnGuardarCategory');
        this.btnGuardarCategoryInCategoryClassName = 'btnGuardarCategoryInCategory';
        // Botón para guardar la categoría eliminada desde el modal Eliminar categoría
        this.btnGuardarEliminarCategory = $('#btnGuardarBorrarCategory');
        // Botón para guardar la categoría una vez se ha editado el nombre desde el modal
        this.btnGuardarNombreCategory = $('#btnGuardarNombreCategory');
        // Combo o select donde se indica la nueva categoría donde se moverán los recursos a la categoría destino desde el modal "Eliminar categoria
        this.cmbMoverATrasEliminar = $('#MoverATrasEliminar');
        // Combo o select donde se indica la categoría "padre" donde crear la nueva categoría
        this.cmbCrearCategoriaEn = $('#cmbCrearCategoriaEn');
        // Combo para categorías huérfanas
        this.cmbMoverElementosTrasBorrar = $('#cmbMoverElementosTrasBorrar');
        // Botón de Sí para deshabilitar la edición multilenguaje de categorías
        this.btnSiDisableMultilenguageCategory = $('#btnSiDisableMultilenguageCategory');
        // Botón de No para No deshabilitar la edición multilenguaje de categorías
        this.btnNoDisableMultilenguageCategory = $('#btnNoDisableMultilenguageCategory');            
    
        /* Guardado categorías */
        // Botón para buardar toda la gestión de categorías
        this.btnGuardarCategorias = $('#btnGuardarCategorias');

        // Modal container para editar y gestionar categorías
        this.modalContainer = $('#modal-container');        

        // Flag que indica si se está borrando una determinada Categoria
        this.confirmDeleteCategory = false;        
    },

    /**
     * Configuración de eventos de elementos del Dom (Botones, Inputs...)     
     */
    configEvents: function (pParams) {
        const that = this;
                       
        this.modalContainer.off().on('show.bs.modal', (e) => {
            // Aparición del modal
            // Conocer el botón disparador del modal para coger atributos necesarios. 
            ////////that.KEY_A_GUARDAR = $(e.relatedTarget).data("atributo");            
            ////////that.KEY_A_GUARDAR = $(e.relatedTarget).data("atributo");            
            // Establecer quién ha disparado el modal para poder cambiar sus atributos una vez se guarden los grupos desde el modal
            that.triggerModalContainer = $(e.relatedTarget);
        })
        .off().on('hide.bs.modal', (e) => {
            // Acción de ocultar el modal
        })
        .off().on('hidden.bs.modal', (e) => {
            // Vaciar el modal
            resetearModalContainer();            
        }); 

        // Comportamientos del modal que de borrado de cláusulas   
        configEventByClassName(`${that.modalDeleteCategoryClassName}`, function(element){
            const $modalClause = $(element);
            $modalClause
            .on('show.bs.modal', (e) => {
                // Aparición del modal
                that.triggerModalContainer = $(e.relatedTarget);
            })
            .on('hide.bs.modal', (e) => {
                // Acción de ocultar el modal
                if (that.confirmDeleteCategory == false){
                    that.handleSetDeleteCategory(false);                
                }               
            }); 
        });        
        
        // Checkbox para habilitar/deshabilitar edición multilenguaje de categorías
        this.chkMultiIdioma.off().on("change", function(){
            that.handleChangeChangeActivarEdicionCategoriasMutilenguaje();
        });

        // Tab para cargar las categorías en el idioma seleccionado
        this.tabIdiomaItem.off().on("click", function(e){
            // Detectar el elemento seleccionado
            const jqueryElement = $(e.currentTarget);
            const idioma = jqueryElement.data("language");
            that.handleSelectCategoryMultiLanguage(idioma);
        });

        // Botón para plegar/desplegar categorías hijas
        this.btnPlegarDesplegarCategoriasHijas.off().on("click", function(e){
            const jqueryElement = $(e.currentTarget);
            const catIdPlegadaDesplegada = jqueryElement.data("categoryid");            
            // Saber si está desplegada o no plegada                
            const categoriaDesplegada = jqueryElement.parents().closest('.type-category').hasClass("show-content");
            if (!categoriaDesplegada){
                // La categoría aun no está desplegada
                that.txtCategoriasExpandidas.val(that.txtCategoriasExpandidas.val() + catIdPlegadaDesplegada + ','); 
            }else{
                // Eliminar la categoría ya que se ha plegado
                // Construir el array de categorías
                let categoriasExpandidasArray = that.txtCategoriasExpandidas.val().split(",");
                // Buscar y borrar el item del array
                categoriasExpandidasArray.splice( $.inArray(catIdPlegadaDesplegada, categoriasExpandidasArray), 1 );
                // Construir el string a partir del array y pasar los datos al input hidden                
                that.txtCategoriasExpandidas.val(categoriasExpandidasArray.join(","));
            }
        });
        
        
        // Botón para crear nuevas categorías.
        this.btnAddCategorias.off().on("click", function(e){
            // Vaciar por defecto posibles categorías seleccionadas para evitar que se creen dentro de alguna categoría seleccionada previamente
            that.handleEliminarMarcarElementoTesauroSeleccionado();
            // Construir los parámetros necesarios para realizar la petición
            that.obtenerParametrosComunes();
            // Hacer la gestión de "Crear categoría" para aparición del modal correspondiente
            that.handleShowAddCategorias();
        });

        // Botón de editar el nombre de una categoría. Corresponde con el botón de editar una determinada categoría
        this.btnActionEditCategory.off().on("click", function(e){
            // Detectar el elemento seleccionado
            const jqueryElement = $(e.currentTarget);
            // Dejar seleccionado dependiendo el botón marcado de la categoría
            that.handleMarcarElementoTesauroSeleccionado(jqueryElement);
            // Obtener parámetros comunes para realizar la petición de renombar categoría
            that.obtenerParametrosComunes();
            getVistaFromUrl(that.urlShowCambiarNombreCategoria,'modal-dinamic-content', that.parametrosComunes, '');
        });

        // Botón de creación de una categoría. Corresponde con el botón de crear una determinada categoría dentro de una categoría.
        this.btnActionAddCategory.off().on("click", function(e){
            const jqueryElement = $(e.currentTarget);
            // Dejar seleccionado dependiendo el botón marcado de la categoría
            that.handleMarcarElementoTesauroSeleccionado(jqueryElement);
            // Construir los parámetros necesarios para realizar la petición
            that.obtenerParametrosComunes();
            // Categoría padre donde crear la categoría nueva
            const parentCategoryId = jqueryElement.data("categoryid");
            // Hacer la gestión de "Crear categoría" para aparición del modal correspondiente
            that.handleShowAddCategoriasInParentCategory(parentCategoryId);
        });  
        
        // Botón de eliminación de una categoría. Corresponde con el botón de eliminar una determinada categoría.
        this.btnActionDeleteCategory.off().on("click", function(e){
            const jqueryElement = $(e.currentTarget);                                    
            // Guardar la fila activa seleccionada
            that.filaCategory = jqueryElement.closest('.category-row');
            // Dejar seleccionado dependiendo el botón marcado de la categoría
            that.handleMarcarElementoTesauroSeleccionado(jqueryElement);
            // Marcamos la categoria como para eliminar
            that.handleSetDeleteCategory(true);             
        });   
         
        // Botón para confirmar la eliminación de una categoría. Corresponde con el botón de la vista modal "Sí"
        configEventByClassName(`${that.btnConfirmDeleteCategoryClassName}`, function(element){
            const $confirmButton = $(element);
            $confirmButton.off().on("click", function(){                                                                            
                // Proceder al borrado de la categoría o a mostrar el modal para el borrado definitivo de la categoría
                that.handleShowDeleteCategory();                
            });	                        
        });           

        // Botón para guardar toda la información relativa a Categorías de la Comunidad
        this.btnGuardarCategorias.off().on("click", function(){
            // Gestionar el guardado de categorías
            that.guardarGestionCategorias();            
        });

        // Permitir el botón de guardar sólo cuando haya acciones pendientes
        this.txtAccionesTesauroHack.val() != "" ? this.btnGuardarCategorias.prop('disabled', false) : this.btnGuardarCategorias.prop('disabled', true)
               
        // Botón para aceptar las categoría sugerida
        this.btnAcceptCategorySuggested.off().on("click", function(e){
            const jqueryElement = $(e.currentTarget);
            const catId = jqueryElement.data("categoryid");            
            that.handleAcceptRejectCategorySuggested(catId, true);
        });

        // Botón para cancelar la categoría sugerida
        this.btnRejectCategorySuggested.on("click", function(e){
            const jqueryElement = $(e.currentTarget);
            const catId = jqueryElement.data("categoryid");            
            that.handleAcceptRejectCategorySuggested(catId, false);
        });

        // Botón para plegar o desplegar las categorías                
        configEventByClassName(`${that.btnShowHideIconClassName}`, function(element){
            const collapseButton = $(element);
            collapseButton.off().on("click", function(){   
                // Comprobar si está colapsado                                             
                that.handleShowHideChildrenPanel(collapseButton);
            });	                        
        });  
        
        // Botón para guardar una categoría desde el modal de "Crear categoría en Categoría"
        configEventByClassName(`${that.btnGuardarCategoryInCategoryClassName}`, function(element){
            const $jqueryElement = $(element);
            $jqueryElement.off().on("click", function(){ 
                // Categoría padre donde se desea crear la categoría
                const categoryId = $jqueryElement.data("category-id");
                that.handleAddCategoryInCategory(categoryId);                
            });	                                 
        });         
    },


    /**
     * Método para mostrar o colapsar (ocultar) el panel de los hijos de la categoría
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
        const tesauroRowItem = collapseButton.closest(`.${that.categoryListItemClassName}`);
        const panelCategoryChildrenItem = tesauroRowItem.find(`.${that.categoryChildrenPanelClassName}`).first();               

        if (collapseButton.hasClass(collapseClassName)){
            // Expandir o Mostrar el panel children
            collapseButton.removeClass(collapseClassName);
            collapseButton.addClass(expandClassName);
            collapseButton.html(collapseIcon);  
            panelCategoryChildrenItem.removeClass("d-none");          
        }else{            
            // Ocultar o colapsar el panel children
            collapseButton.addClass(collapseClassName);
            collapseButton.removeClass(expandClassName);
            collapseButton.html(expandIcon);
            panelCategoryChildrenItem.addClass("d-none");
        }

        // Ocultar siempre el panel de edición en caso de estar abierto
        ////////const tesauroItemRow = collapseButton.closest(`.${that.tesauroItemListItemClassName}`);
        //////// tesauroItemRow.find(`.${that.panelElementDetailClassName}`).first().removeClass("show");
    },

    /**
     * Configuración de las rutas de acciones necesarias para hacer peticiones a backend
     */
    configRutas: function(){
        /* Url de acciones */    
        this.urlBase = refineURL();                     
        // Url para obtener la vista modal de la acción de Crear Categorías        
        this.urlShowCrearCategoria = `${this.urlBase}/show-action/create`;
        // Url para obtener la vista modal de la acción de Crear categoría dentro de una categoría padre
        this.urlShowCrearCategoriaInCategory = `${this.urlBase}/load-modal-create-subcategory`;
        // Url para obtener la url para realizar la petición del guardado de categoría
        this.urlCrearCategoria = `${this.urlBase}/action/create`;
        // Url para guardado de todas las categorías
        this.urlGuardarCategorias = `${this.urlBase}/save`;
        // Url para eliminar una categoría
        this.urlEliminarCategoria = `${this.urlBase}/show-action/delete`;
        // Url para eliminación definitiva de categoría desde Modal
        this.urlEliminarCategoriaFromModal = `${this.urlBase}/action/delete`;
        // Url para mostrar modal para cambiar nombre
        this.urlShowCambiarNombreCategoria = `${this.urlBase}/show-action/change-name`;        
        // Url para cambiar el nombre de una categoría
        this.urlCambiarNombreCategoria = `${this.urlBase}/action/change-name`;
        // Url para aceptar la categoría sugerida
        this.urlAceptarCategoriaSugerida = `${this.urlBase}/action/acept-category`;
        // Url para rechazar la categoría sugerida
        this.urlRechazarCategoriaSugerida = `${this.urlBase}/action/reject-category`;
        // Url para cargar categorías multilenguaje
        this.urlCargarCategoriasMultilenguaje = `${this.urlBase}/action/multilanguaje`;
        // Url para mostrar el modal que pregunte si se desea cancelar la edición de multilenguaje
        this.urlShowDisableMutilenguageCategory = `${this.urlBase}/load-disable-multilenguage`;
        // Url para activar la acción de deshabilitar la edición multilenguaje
        this.urlDisableMultilenguajeCategoria =  `${this.urlBase}/action/onlylanguaje`;
        // Url para mover categorías. Se utiliza en la operativa 'operativaCategoriasSortable'
        this.urlMoveCategoria = `${this.urlBase}/action/move`;
        // Url para ordenar categorías. Se utiliza en la operativa 'operativaCategoriasSortable'
        this.urlSortCategoria = `${this.urlBase}/action/order` 
    },


    /**
     * Lanzar comportamientos u operativas necesarias para el funcionamiento de la sección
     */
    triggerEvents: function(){
        const that = this; 
        // Ejecutar comportamientos de categorías definidos en community.js
        CategoriasManagement.init(); 
        // Ejecutar comportamientos de categorías para Sortable 
        operativaCategoriasSortable.init();                          
        //operativaNestedSortable.init(that.communityCategoryListClassName, that.btnDragCategoryIconClassName, that.categoryListItemClassName, that.handleReorderItemsFinished);              
    },    

    // Configurar elementos relativos a la operativa de Añadir Categoria cuando se lance el modal
    operativaModalAddCategory: function(){
        const that = this;
        // Inicializar de detección de nuevos elementos en el DOM
        that.config(that.params);

        // Inicializar comportamientos de select2
        comportamientoInicial.iniciarSelects2()
        
        
        // Click del botón Crear categoría del modal
        that.btnGuardarCategory.on("click", function(e){
            that.handleAddCategory();
        });

        // Cuando se cargue el combox, detectar si es necesario dejar una opción por defecto seleccionada siempre que se seleccione crear una categoría
        // dentro de otra (pulsando en el botón de añadir categoría)
        that.cmbCrearCategoriaEn.ready(function(){
            that.handleSelectParentCategory();
        });
    },

    // Configurar elementos relativos a la operativa de Eliminar Categoria cuando se lance el modal
    operativaModalDeleteCategory: function(){
        const that = this;
        // Inicializar de detección de nuevos elementos en el DOM
        that.config(that.params);

        // Inicializar comportamientos de select2
        comportamientoInicial.iniciarSelects2()
        
        // Click del botón Crear categoría del modal
        that.btnGuardarEliminarCategory.on("click", function(e){
            // Proceder al borrado y mover categorías a categoría destino
            that.handleDeleteCategory();            
        });
    }, 
    
    // Configurar elementos relativos a la operativa de Renombar una Categoría cuando se lance el modal
    operativaModalRenameCategory: function(){
        const that = this;

        // Inicializar de detección de nuevos elementos en el DOM
        that.config(that.params);

        // Inicializar comportamientos de select2
        comportamientoInicial.iniciarSelects2()

        that.btnGuardarNombreCategory.on("click", function(){
            // Guardar el renombre de categoría            
            that.handleRenameCategory();
        });
    },

    // Configurar elementos relativos a la operativa de Deshabilitar edición multilenguaje de Categorias cuando se lance el modal
    operativaModalDisableMultilenguageCategory: function(){
        const that = this;

        // Inicializar de detección de nuevos elementos en el DOM
        that.config(that.params);

        // Botón de Sí para deshabilitar la edición multilenguaje de categorías
        that.btnSiDisableMultilenguageCategory.on("click", function(){             
            that.handleClickSiDesactivarEdicionMultiLenguageCategorias();          
        });

        // Botón de No para No deshabilitar la edición multilenguaje de categorías
        that.btnNoDisableMultilenguageCategory.on("click", function(){            
            that.handleClickNoDesactivarEdicionMultiLenguageCategorias();           
        });

    },


    /**
     * Método para devolver los parámetros de los inputs ocultos para realizar peticiones a backEnd
     */
    obtenerParametrosComunes: function(){
        const that = this;

        const params = {
            IdiomaSeleccionado: that.txtHackIdiomaTesauro.val(),
            multiLanguage: that.chkMultiIdioma.is(':checked'),
            CategoriasExpandidas: that.txtCategoriasExpandidas.val(),
            CategoriasSeleccionadas: that.txtCategoriasSeleccionadas.val(),
            PasosRealizados: that.txtAccionesTesauroHack.val()
        }
        that.parametrosComunes = params;        
    },


    /**
     * Método "completion/callback" que se ejecutará una vez finalice el evento de arrastrar/reordenar un item de categorías
     * @param {jqueryElement} itemMoved : Item que ha sido arrastrado o movido vía nestedSortable
     */
    /*
    handleReorderItemsFinished: function(itemMoved){
        const that = this;

        // Obtener el padre del item arrastrado                
        let $parentNode = itemMoved.parents("li").first();  
        
        // Obtener el id del padre para acción de mover item del Tesauro
        if ($parentNode.length > 0){                       
            //selectedCategoryToMove = $parentNode.data("parent-categories-move-categories");
        }  
    },*/


    /**
     * Acción de llamada a Backend para la creación de Categoría (Modal Añadir categorías)
     */
    handleShowAddCategorias: function(){
        const that = this;
                
        getVistaFromUrl(that.urlShowCrearCategoria, 'modal-dinamic-content', that.parametrosComunes, function(result, message){
            if (result != "OK"){
                dismissVistaModal();
                mostrarNotificacion("error", message);
            }
        });
    },

    
    /**
     * Acción de llamada a Backend para la creación de Categoría dentro de una categoría padre
     * @param {*} parentCategoryId Id de la categoría, que será la padre, donde se desea crear la categoría.
     */
    handleShowAddCategoriasInParentCategory: function(parentCategoryId){
        const that = this;
                
        // Categoría padre donde se desea crear la categoría                
        const params = { ...that.parametrosComunes, categoriaPadreID: parentCategoryId};        
        
        getVistaFromUrl(that.urlShowCrearCategoriaInCategory, 'modal-dinamic-content', params, function(result, message){
            if (result != "OK"){
                dismissVistaModal();
                mostrarNotificacion("error", message);
            }
        });
    },    
    

    /**
     * * Acción para saber qué elemento ha sido seleccionado al pulsar en alguno de los elementos o botones de acciones (Cambiar nombre, Crear nuevo...)
     * @param {jqueryElement} pNodo : Elemento seleccionado
     */
    handleMarcarElementoTesauroSeleccionado: function(pNodo){
        const that = this;
        // Coger el id y el nombre de la categoría seleccionada
        const categoryId = pNodo.data("categoryid");
        const categoryName = pNodo.data("categoryname");

        // Marcar categorías seleccionadas o marcadas
        const txtHack = that.txtCategoriasSeleccionadas
        txtHack.val(categoryId + ',');

        /* No hace falta solo se puede seleccionar una única categoría
        if (checked) {
            txtHack.val(txtHack.val() + catID + ',');
        }
        else {
            txtHack.val(txtHack.val().replace(catID + ',', ''));
        }
        */
    },

    /**
     * * Vaciar el input de elementos marcados para evitar problemas se ejecutará desde "config"     
     */
    handleEliminarMarcarElementoTesauroSeleccionado: function(){
        const that = this;
        // Marcar categorías seleccionadas o marcadas
        const txtHack = that.txtCategoriasSeleccionadas;
        txtHack.val('');
    },


    /**
     * Acción de crear una categoría al pulsar en el modal "Guardar". También se utiliza para crear una categoría dentro de una categoría padre
     * @param {bool} createCategoryInCategory Indica si se desea crear la categoría dentro de una categoría padre. Por defecto "false"
     * @param {string} parentCategoryId Id de la categoría padre donde se desea crear la categoría seleccionada
     */
    handleAddCategory: function(createCategoryInCategory = false, parentCategoryId = undefined){
        const that = this;
        // Mostrar loading
        loadingMostrar();
        // Categoría padre donde crear la nueva categoría
        const catPadre = createCategoryInCategory == false ? that.cmbCrearCategoriaEn.val() : parentCategoryId;
        // Nombre de la nueva categoría
        let nombreCat = "";
        // Permitir multilenguaje en categorías
        const esMultiIdioma = that.chkMultiIdioma.is(':checked');
        
        // Buscar los nombres de diferentes idiomas de la categoría
        that.inputIdioma.each(function () {
            if (!esMultiIdioma) {
                if ($(this).attr('rel') == that.txtHackIdiomaTesauro.val()) {
                    nombreCat = $(this).attr('rel') + "@@@" +$(this).val() + '$$$';
                }
            }
            else {
                nombreCat += $(this).attr('rel') + "@@@" +$(this).val() + '$$$';
            }
        });
        nombreCat = nombreCat.substring(0, nombreCat.length - 3);
    
        // Construir el objeto de parámetros comunes
        that.obtenerParametrosComunes()
        that.parametrosComunes.name = nombreCat;        
        that.parametrosComunes.parentKey = catPadre; 
        
        // Realizar el guardado de datos
        GnossPeticionAjax(
            that.urlCrearCategoria,
            that.parametrosComunes,
            true
        ).done(function (data) {
            // OK                
            // Cargar los datos en el contenedor del tesauro
            that.panelTesauro.html(data);
            // Nuevos elementos del DOM insertados -> Resetear comportamientos
            that.config(that.pParams);
            that.configEvents();
            that.triggerEvents();
            // Ocultar el modal y mostrar mensaje común de guardado correctamente                                                 
            dismissVistaModal(); 
        }).fail(function (data) {
            mostrarNotificacion("error", data);            
        }).always(function () {
            // Ocultar loading
            loadingOcultar();
        });        
    },
 
    /**
     * Acción de crear una categoría dentro de una categoría padre
     * @param {*} categoryId 
     */
    handleAddCategoryInCategory: function(categoryId){
        const that = this;                
        that.handleAddCategory(true, categoryId);
    },

    /**
     * Método para marcar o desmarcar la cláusula como "Eliminada" dependiendo de la elección vía Modal
     * @param {Bool} deleteCategory Valor que indicará si se desea eliminar o no la cláusula
     */
    handleSetDeleteCategory: function(deleteCategory){
        const that = this;

        if (deleteCategory){
            // Realizar el "borrado"                               
            // Añadir la clase de "deleted" a la fila de la página
            that.filaCategory.addClass("deleted");
        }else{                        
            //  Eliminar la clase de "deleted" a la fila de la página
            that.filaCategory.removeClass("deleted");
        }
    },


    /**
     * Acción de borrar una categoría al pulsar en el botón de "Borrar" de una determinada categoría. Si la categoría está en uso, se mostrará un modal para elegir una nueva
     * categoría destino.
     */
    handleShowDeleteCategory: function(){
        const that = this;

        // Mostrar loading
        loadingMostrar();
        // Obtener los parámetros comunes
        that.obtenerParametrosComunes();   

        // Realizar la petición        
        GnossPeticionAjax(that.urlEliminarCategoria, that.parametrosComunes, true).done(function (data) {  

            // Quitar/ocultar el modal de confirmación del borrado de categoría
            const modalDeleteCategory = $(`.${that.modalDeleteCategoryClassName}`);                                                     
            dismissVistaModal(modalDeleteCategory);                        
            // Detectar la información devuelta de Back -> ¿Hay categorías que se están usando?     
            if (data.$values == undefined){
                // Hay recursos usando esa categoría a borrar -> Cargar modal para seleccionar categoría destino                
                // Mostar el container
                that.modalContainer.modal('show');
                // Cargar el contenido en el modal tras 1 segundo
                that.modalContainer.find('#modal-dinamic-content').html(data);                              
                //$('#panAcciones').html(data.$values);                    
            }else if (data.$values.length == 1 && data.$values[0].updateTargetId == "tesauro"){
                // Restablecer el flag de borrado de la fila de la categoria
                that.confirmDeleteCategory = false;                
                // No hay categorias usándose -> Borrar y actualizar tesauro
                //that.panelTesauro.html(data.$values[0].html);                          
                const htmlData = createElementFromHTML(data.$values[0].html);
                const txtAccionesTesauroHackValue = $(htmlData).find("#txtAccionesTesauroHack").val();
                // Actualizar el input con las acciones del tesauro para su posterior guardado devueltas de Core/BackEnd
                that.txtAccionesTesauroHack.val(txtAccionesTesauroHackValue);
                // Eliminar la categoría del listado
                that.filaCategory.remove();
                // Nuevos elementos del DOM insertados -> Resetear comportamientos
                that.config(that.pParams);
                that.configEvents();
                that.triggerEvents();
            } 
        }).fail(function (data) {
            mostrarNotificacion("error", data);
        }).always(function () {
            loadingOcultar();
        });
    },

    /**
     * Acción de borrar una categoría al pulsar "Guardar" en el modal de "Borra categoria". Antes del borrado es necesario mover los recursos a otra categoria destino
     */
    handleDeleteCategory: function(){
        const that = this;
        // Guardar la nueva categoría destino
        const catSup =  that.cmbMoverATrasEliminar.val();

        if (catSup != '') {            
            // Cargar loading
            loadingMostrar();

            // Detectar si hay categorías húerfanas
            let mover = that.cmbMoverElementosTrasBorrar.val();;
            if (mover == undefined || mover == '') {
                mover = "TODOS";
            }

            // Obtener parámetros comunes
            that.obtenerParametrosComunes();            
            // Detectar las categorías que se procederá a eliminar. Obtenerlo de "txtCategoriasSeleccionadas". Quitar último caracter (,)
            const categorias = that.txtCategoriasSeleccionadas.val().slice(0, -1);
            that.parametrosComunes.CategoriasSeleccionadas = categorias;
            that.parametrosComunes.parentKey = catSup;
            that.parametrosComunes.moveTo = mover;
            
            // Realizar petición 
            GnossPeticionAjax(that.urlEliminarCategoriaFromModal, that.parametrosComunes, true).
            done(function (data) {
                // OK                
                // Cargar los datos en el contenedor del tesauro
                that.panelTesauro.html(data);
                // Nuevos elementos del DOM insertados -> Resetear comportamientos
                that.config(that.pParams);
                that.configEvents();
                that.triggerEvents();
                // Ocultar el modal y mostrar mensaje común de guardado correctamente                                                 
                dismissVistaModal();                
            }).fail(function (data) {
                // KO
                mostrarNotificacion("error", data);

            }).always(function () {
                loadingOcultar();
            });
        }
    },

    
    /**
     * Acción para renombrar una categoría al pulsar "Renombrar" en el modal de "Renombrar categoria".
     */
    handleRenameCategory: function(){
        const that = this;
        // Mostrar loading        
        loadingMostrar();
        // Seleccionar la categoría ID seleccionada a cambiar el nombre. Cogerla de txtHackC
        const categoria = that.txtCategoriasSeleccionadas.val().slice(0, -1);
        
        // Nombre de la nueva categoría
        let nombreCat = "";
        // Permitir multilenguaje en categorías
        const esMultiIdioma = that.chkMultiIdioma.is(':checked');

        that.inputIdioma.each(function () {
            if (!esMultiIdioma) {
                if ($(this).attr('rel') == that.txtHackIdiomaTesauro.val()) {
                    nombreCat = $(this).attr('rel') + "@@@" +$(this).val() + '$$$';
                }
            }
            else {
                nombreCat += $(this).attr('rel') + "@@@" +$(this).val() + '$$$';
            }
        });

        nombreCat = nombreCat.substring(0, nombreCat.length - 3);
        // Obtener los parámetros comunes
        that.obtenerParametrosComunes();
        that.parametrosComunes.name = nombreCat;
        that.parametrosComunes.categoryKey = categoria;

        // Realizar la petición
        GnossPeticionAjax(that.urlCambiarNombreCategoria, that.parametrosComunes, true)
        .done(function (data) {                        
            // OK                
            // Cargar los datos en el contenedor del tesauro
            that.panelTesauro.html(data);
            // Nuevos elementos del DOM insertados -> Resetear comportamientos
            that.config(that.pParams);
            that.configEvents();
            that.triggerEvents();
            // Ocultar el modal                                               
            dismissVistaModal();   
        }).fail(function (data) {
            // KO
            mostrarNotificacion("error", data);
        }).always(function () {
            loadingOcultar();
        });
    },


    /**
     * Acción para aceptar o rechazar la categoría sugerida. Se ejecuta al hacer click en el botón de "Aceptar"
     * @param {String} catId : Id de la categoría que se desea aceptar o rechazar
     * @param {Bool} acceptCategory: Valor booleano que indica si se desea aceptar la categoría. En caso contrario, se rechazará. Por defecto será 'true'
     */
    handleAcceptRejectCategorySuggested: function(catId, acceptCategory = true){
        const that = this;    
        // Mostrar loading
        loadingMostrar();
        // Construir los parámetros comunes
        that.obtenerParametrosComunes();
        // Añadir la categoria a aceptar
        that.parametrosComunes.CategoryKey = catId;

        // Comprobar la acción que se desea realizar (Aceptar o Rechazar la categoría)
        const url = acceptCategory == true ? that.urlAceptarCategoriaSugerida : that.urlRechazarCategoriaSugerida; 
    
        // Realizar la petición
        GnossPeticionAjax(url, that.parametrosComunes, true)
        .done(function (data) {
            // OK                                        
            // Cargar los datos en el contenedor del tesauro
            that.panelTesauro.html(data);
            // Nuevos elementos del DOM insertados -> Resetear comportamientos
            that.config(that.pParams);
            that.configEvents();
            that.triggerEvents();
            // Ocultar el modal                                               
            dismissVistaModal();         
        }).fail(function (data) {
            // KO
            mostrarNotificacion("error", data);            
        }).always(function () {
            // Ocultar loading
            loadingOcultar();
        });
    },


    /**
     * Acción para seleccionar la categoría padre cuando se decide crear una categoría dentro de otra (al pulsar el botón de Crear Categoría dentro de...)
     * @param {jQueryElement} cmbBox 
     */
    handleSelectParentCategory: function(cmbBox){
        const that = this;
        // Categoría padre donde se desea crear la nueva categoría
        const parentCatId = that.txtCategoriasSeleccionadas.val().slice(0, -1);
        if (parentCatId != ''){
            // Seleccionar la opción como "selected" correspondiente al parentCatId        
            that.cmbCrearCategoriaEn.val(parentCatId).change();
        }        
    },

    /**
     * Acción para cargar las actuales categorías en el idioma seleccionado según el TabBar pulsado
     * @param {String} language 
     */
    handleSelectCategoryMultiLanguage: function(language){        
        const that = this;
        // Establecer en el input el idioma seleccionado
        that.txtHackIdiomaTesauro.val(language);
        // Mostrar loading
        loadingMostrar();

        // Cargar los tabs de los idiomas
        //that.cargarTabIdiomas();
        
        // Cargar los parámetros comunes
        that.obtenerParametrosComunes();
        
        GnossPeticionAjax(that.urlCargarCategoriasMultilenguaje, that.parametrosComunes, true)
        .done(function (data) {                                    
            that.panelTesauro.html(data);
            // Nuevos elementos del DOM insertados -> Resetear comportamientos
            that.config(that.pParams);
            that.configEvents();
            that.triggerEvents();        
        }).fail(function (data) {
            mostrarNotificacion("error", data);            
        }).always(function () {
            // Ocultar loading            
            loadingOcultar();
        }); 
    },

    /**
     * Acción para cargar el modal que muestra la pregunta al usuario de si desea desactivar o activar la gestión multiidioma de categorías
     */
    handleChangeChangeActivarEdicionCategoriasMutilenguaje: function(){ 
        const that = this;        
        
        if (!that.chkMultiIdioma.is(':checked')) {
            // Mostrar el el contenedor modal
            that.modalContainer.modal('show');
            // Cargar la vista a mostrar en el modal
            getVistaFromUrl(that.urlShowDisableMutilenguageCategory, 'modal-dinamic-content');                
        } else {
            // Establecer el idioma por defecto del Tesauro
            that.txtHackIdiomaTesauro.val(that.txtHackIdiomaTesauroDefecto.val());
            // Cargar u ocultar los Tabs de multiIdiomas
            that.cargarTabIdiomas();
        }        
    },

    /**
     * Acción para desactivar la edición multiidioma de categorías. Se ejecutará desde el modal de "ActivarEdicionMultilenguaje" cuando se pulse en
     * que sí se desea desactivar la edición multilenguaje
     */
    handleClickSiDesactivarEdicionMultiLenguageCategorias: function(){
        const that = this;
        
        // Mostrar loading
        loadingMostrar();
        // Cargar u ocultar los Tabs de multiIdiomas
        that.cargarTabIdiomas();

        // Obtener los parámetros comunes
        that.obtenerParametrosComunes();
        
        // Realizar petición para desactivar Edición multilenguaje
        GnossPeticionAjax(that.urlDisableMultilenguajeCategoria, that.parametrosComunes, true)
        .done(function (data) {
            // OK                                
            // Cargar los datos en el contenedor del tesauro
            that.panelTesauro.html(data);
            // Nuevos elementos del DOM insertados -> Resetear comportamientos
            that.config(that.pParams);
            that.configEvents();
            that.triggerEvents();
            // Ocultar el modal y mostrar mensaje común de guardado correctamente                                                 
            dismissVistaModal();
        }).fail(function (data) {            
            mostrarNotificacion("error", data);
        }).always(function () {
            loadingOcultar();                        
        });
    },


    /**
     * Acción para cancelar la desactivación de la edición multiidioma de categorías. Se ejecutará desde el modal de "ActivarEdicionMultilenguaje" cuando se pulse en
     * que no se desea desactivar la edición multilenguaje
     */
    handleClickNoDesactivarEdicionMultiLenguageCategorias: function(){
        const that = this;
        // Dejar activado el multiIdioma
        that.chkMultiIdioma.prop("checked", true);        
        // Cerrar el modal
        dismissVistaModal();                
    },

    /**
     * Acción para cargar los tabs con los diferentes idiomas disponibles para categorías.
     * Esta acción es ejecutada desde 'handleSelectCategoryMultiLanguage'
     */
    cargarTabIdiomas: function(){
        const that = this;
        const esMultiIdioma = that.chkMultiIdioma.is(':checked');
        if (esMultiIdioma) {
            // Recorrer los idiomas disponibles
            /*that.tabIdiomaItem.each(function(){
                if ($(this).data("language") == that.txtHackIdiomaTesauro.val()) {
                    $(this).parent().addClass("d-none");
                }
                else {
                    $(this).parent().removeClass("d-none");                    
                }
            });
            */
            // Mostrar la barra de idiomas
            that.tabIdiomas.removeClass("d-none"); 
        }
        else {
            // Ocultar la barra de idiomas            
            that.tabIdiomas.addClass("d-none");            
        }
    },

    /************************************************************************************************************************ */
    // Métodos para guardado de la pestaña Categorías
    /************************************************************************************************************************ */

    /**
     * Método para guardar toda la información de la página de categorías.
     * Se ejecutará una vez se haga click en "Guardar todo"
     */
    guardarGestionCategorias: function(){
        const that = this;

        if (that.txtAccionesTesauroHack.val() != "") {
            // Mostrar loading 
            loadingMostrar();            
            // Obtener los datos Parámetros comunes para guardado                
            that.obtenerParametrosComunes();

            GnossPeticionAjax(that.urlGuardarCategorias, that.parametrosComunes, true).done(function (data) {
                // OK                
                // Cargar los datos en el contenedor del tesauro
                that.panelTesauro.html(data);
                // Nuevos elementos del DOM insertados -> Resetear comportamientos
                that.config(that.pParams);
                that.configEvents();
                that.triggerEvents();   
                // Mostrar guardado correcto
                mostrarNotificacion("success", "Se ha guardado correctamente.")  
            }).fail(function (data) {
                // KO
                mostrarNotificacion("error", data);                
            }).always(function () {
                loadingOcultar();
            });
        }
    },
}


/**
 * Operativa de funcionamiento de Gestión de certificados de la comunidad
 */
const operativaGestionCertificacion = {

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
        // Url para editar un certificado
        this.urlCreateEditCertification = `${this.urlBase}/load-create-edit-certification`;
        // Url para solicitar la confirmación de certificados con documentos asociados
        this.urlCreateAskDeleteCertificationWithDocsAsociados = `${this.urlBase}/load-confirm-delete-certification`;
        // Url para guardar la página de certificados
        this.urlGuardarDatosCertificados = `${this.urlBase}/save-certifications`;
    },    

    /*
     * Inicializar elementos de la vista
     * */
    config: function (pParams) {
            
        /* Contenedor modal */
        this.modalContainer = $("#modal-container");
        // Controlar si hiciera falta saber qué elemento ha disparado el modal-container
        this.triggerModalContainer = "";
        // Checkbox para activar/desactivar los niveles de certificación
        this.chkActivarNivelesCertificacion = $('#chkActivarNivelesCertificacion');
        // Panel informativo de personalización de los niveles de certificación
        this.panelCustomCertificationInformation = $('#panelCustomCertificationInformation');
        // Panel informativo de que la personalización está siendo elaborada
        this.panelCertificationInProgressInformation = $('#panelCertificationInProgressInformation');
        // Panel que contiene la sección de creación y edición de los diferentes certificados de la comunidad
        this.panelCreacionEdicionCertificados = $("#panelCreacionEdicionCertificados");
        // Panel que contiene los certificados a modo de listado
        this.panelListaCertificados = $('#id-added-certifications-list');
        // Input con la política de certificación en el ckEditor
        this.txtPoliticaCertificacion = $('#txtPoliticaCertificacion');

        // Botón para añadir una nueva certificación
        this.btnAddCertification = $('#btnAddCertification');
        // Botón para editar una determinada certificación
        this.btnActionEditCertificacion = $('.action-edit');
        // Botón para eliminar una determinada certificación
        this.btnActionDeleteCertificacion = $('.action-delete');    
        // Botón para guardar la gestión de certificados de recursos
        this.btnGuardarCertificacionRecursos = $('#btnGuardarCertificacionRecursos');

        /* Elementos del modal para editar/crear certificación */
        this.txtNombreCertificacion = $('#txtNombreCertificacion');
        this.txtNombreCertificacionId = 'txtNombreCertificacion';
        this.btnGuardarCertificacion = $('#btnGuardarCertificacion');
        this.btnGuardarCertificacionId = 'btnGuardarCertificacion';
        this.btnConfirmarBorrarCertificado = $('#btnConfirmarBorrarCertificado');
        this.btnConfirmarBorrarCertificadoId = 'btnConfirmarBorrarCertificado';
        this.btnCancelarBorrarCertificado = $('#btnCancelarBorrarCertificado');
        this.btnCancelarBorrarCertificadoId = 'btnCancelarBorrarCertificado';

        // Parámetro a enviar cuando cree o edite un certificado vía modal
        this.parametroCertificado = {};

        /* Guardar datos de certificados */
        this.DatosGuardado = {};

        /* Parámetros traidos de la vista */
        this.mensajes = pParams.mensajes;        
    },

    /**
     * Configuración de eventos de elementos del Dom (Botones, Inputs...)     
     */
     configEvents: function (pParams) {
        const that = this;
                       
        this.modalContainer.on('show.bs.modal', (e) => {
            // Aparición del modal            
        })
        .on('hide.bs.modal', (e) => {
            // Acción de ocultar el modal
        })
        .on('hidden.bs.modal', (e) => {
            // Vaciar el modal
            resetearModalContainer();            
        });

        // Checkbox para activar o desactivar los niveles de Certificación
        
        this.chkActivarNivelesCertificacion.off().on("click", function(){
            const jqueryElement = $(this);

            if (jqueryElement.is(':checked')) {
                // Mostrar la edición y creación de certificados
                that.panelCreacionEdicionCertificados.removeClass("d-none");
                //that.panelCustomCertificationInformation.addClass("d-none");                                
            }
            else {
                // Ocultar la edición de certificados                                
                that.panelCreacionEdicionCertificados.addClass("d-none");
                //that.panelCustomCertificationInformation.removeClass("d-none");
            }
        });
        

        // Botón para crear una nueva certificación
        this.btnAddCertification.off().on("click", function(){
            // Mostrar el modal cargando            
            that.modalContainer.modal('show');      
            // Creo nuevo guid para el nuevo certificado a crear
            const id = guidGenerator();     
            that.obtenerParametrosCertificado(id);
            // Cargar la vista para crear nueva certificación
            getVistaFromUrl(that.urlCreateEditCertification, 'modal-dinamic-content', that.parametroCertificado);
        });


        // Botón para editar una certificación existente        
        this.btnActionEditCertificacion.off().on("click", function(){
            // Mostrar el modal cargando            
            that.modalContainer.modal('show'); 
            // Construir el certificado a editar 
            const certificate = $(this).parents().closest(".certification-row");          
            const certificateId = certificate.data("id");
            const certificateName = certificate.data("nombre");
            const certificateOrder = certificate.data("orden").toString();
            // Cargamos el certificado a editar            
            that.obtenerParametrosCertificado(certificateId, certificateName, certificateOrder);            
            // Cargar la vista para crear nueva certificación a editar
            getVistaFromUrl(that.urlCreateEditCertification, 'modal-dinamic-content', that.parametroCertificado);
        });

        // Botón para eliminar una certificación existente o de reciente creación (que no ha sido guardada antes)
        this.btnActionDeleteCertificacion.off().on("click", function(){
            const certificate = $(this).parents().closest(".certification-row");
            that.handleCheckDeleteCertificado(certificate);
        });


        // Botón para guardar toda la información relativa a los certificados
        this.btnGuardarCertificacionRecursos.off().on("click", function(){
            // Gestionar el guardado de los certificados
            //let catIdPadre = $(itemEl).parents().closest('.type-category').data('id');
            that.handleGuardarCertificacionRecursos();
        });

    },   

    /**
     * Lanzar comportamientos u operativas necesarias para el funcionamiento de la sección
     */
    triggerEvents: function(){
        const that = this;  
        
        // Inicializar operativa para ordenar (sortable) certificados
        operativaCertificadosSortable.init();
        // Reiniciar / Resetear ckeditor (Personalizar mensaje de Certificación en construcción)
        ckEditorRecargarTodos();
    },  

    /**
     * Método para reestablecer los index de los elementos cuando se creen, borren o modifiquen el orden de los certificados
     */
    reorderCertificados: function(){
        const that = this;
        const certificados = that.panelListaCertificados.children();

        // Asegurarse del borrado e insercción correcta del elemento
        setTimeout(() => {
            let contador = 0;
            $.each(certificados, function () {                
                const certificado = $(this);
                certificado.data("orden",contador);
                contador += 1;
            });        
        }, 1000);        
    },
    
    /**
     * Método para construir el objeto o modelo cuando se decide crear o editar un certificado para ser mandado a backEnd
     * @param {*} pId : Id del certificado. Tendrá id siempre y cuando exista con anterioridad (no de reciente creación)
     * @param {*} pName : Nombre del certificado o valor que tendrá
     * @param {*} pPosition : Posición de ordenación del certificado
     */
    obtenerParametrosCertificado: function(pId = undefined, pName = undefined, pPosition = undefined){
        const that = this;
      // Comprobar datos antes de construir el objeto
        if ( pId == "" || pId == undefined){
          pId = "";
      }
      
      if (pName == "" || pName == undefined){
        pName = "";
      }

      if (pPosition == "" || pPosition == undefined){
        pPosition = "";
      }

      that.parametroCertificado.id = pId;
      that.parametroCertificado.name = pName;
      that.parametroCertificado.position = pPosition;
    },

    /**
     * Acción para crear un nuevo certificado desde el modal. Añadirá una nueva fila de certificado en la sección de certificados (id:id-added-certifications-list)
     * @param {String} pName : Nombre del certificado
     * @param {Bool} pCreatedRecentlyWithouSaving : Indica si se ha creado recientemente y no se ha guardado aun -> Está siendo editado. Por defecto será falso.
     * @param {String} pId : Id del certificado. Por defecto será vacío ya que es nuevo.
     */
    handleAddCertificado: function(pName, pCreatedRecentlyWithouSaving = false, pOrden = undefined, pId = ""){
        const that = this;

        // Controlar si es necesario añadir nueva fila de certificado
        let createNewRow = false;
        // Orden o posición del certificado a crear
        let ordenCertificado = "";
        // Calcular la nueva posición
        if (pOrden == undefined){
            ordenCertificado = that.panelListaCertificados.find('.component-wrap').length
        }else{
            ordenCertificado = pOrden;
        }

        // Comprobar si el recurso es de reciente creación
        let createdRecentlyProperty = "";
        if (pCreatedRecentlyWithouSaving == false && pOrden == undefined){
            // Nuevo certificado            
            createNewRow = true;                                       
        }else{
            // Editando certificado sin guardar previamente 
            const certificateRowEdited = $(that.panelListaCertificados.children()[pOrden]);
            // Cambiar parámetros para su correcta visualización
            certificateRowEdited.data("nombre", pName);
            certificateRowEdited.find(".component-name").text(pName);
        }

        if (createNewRow){    
            // Insertar un nuevo certificado en la lista y resetear config para operativa de los botones
            const templateCertificate = `
            <li
                class="component-wrap certification-row"
                data-id="${pId}"
                data-nombre="${pName}"
                data-orden="${ordenCertificado}"
                data-new="true">
                <div
                    class="component type-certification"
                    data-component-type="certification">
                    <div class="component-header-wrap">
                        <div class="component-header">
                            <div class="component-sortable js-component-sortable-certification">
                                <span class="material-icons-outlined sortable-icon">drag_handle</span>
                            </div>
                            <div class="component-header-content">
                                <div class="component-header-left">
                                    <div class="component-name-wrap">
                                        <span class="component-icon"></span>
                                        <span class="component-name">${pName}</span>
                                    </div>
                                </div>
                                <div class="component-header-right">
                                    <div class="component-actions-wrap">
                                        <ul class="no-list-style component-actions">
                                            <li>
                                                <a
                                                    class="action-edit round-icon-button js-action-edit-component"
                                                    style="cursor: pointer">
                                                    <span class="material-icons">edit</span>
                                                </a>
                                            </li>
                                            <li>
                                                <a
                                                    class="action-delete round-icon-button js-action-delete"
                                                    style="cursor: pointer">
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

            // Añadir el nuevo certificado
            that.panelListaCertificados.append(templateCertificate);        
            // Resetear comportamientos
            // Nuevos elementos del DOM insertados -> Resetear comportamientos
            that.config(that.pParams);
            that.configEvents();
            that.triggerEvents();
        }
    },


    /**
     * Acción para comprobar si se puede eliminar un certificado. Si el certificado tiene recursos asociados, se mostrará un mensaje para confirmar la eliminación
     * del certificado. En caso contrario se procederá a su eliminación
     * @param {jqueryObject} pCertificado : Fila del certificado que se desea eliminar
     */
    handleCheckDeleteCertificado(pCertificado){
        const that = this;
        const hasDocsAsociados = pCertificado.hasClass("docsAsociados");
        if (hasDocsAsociados){
            // Mostrar el modal
            that.modalContainer.modal('show');
            // Construir el certificado a eliminar        
            const certificateId = pCertificado.data("id");
            const certificateName = pCertificado.data("nombre");
            const certificateOrder = pCertificado.data("orden").toString();
            // Cargamos el certificado a editar            
            that.obtenerParametrosCertificado(certificateId, certificateName, certificateOrder);            
            // Cargar la vista para confirmar el borrado del certificado                        
            getVistaFromUrl(that.urlCreateAskDeleteCertificationWithDocsAsociados, 'modal-dinamic-content', that.parametroCertificado);                        
        }else{
            that.handleDeleteCertificado(pCertificado);
        }
    },

    /**
     * Acción que borra un certificado o una línea de certificado de la lista de certificados cuando se ha pulsado en la papelera.
     * @param {jqueryObject} pCertificado : Fila del certificado que se desea eliminar
     */
    handleDeleteCertificado: function(pCertificado){        
        const that = this;
        pCertificado.fadeOut("normal", function() {
            $(this).remove();
            // Reordenar certificados por haber sido eliminados
            that.reorderCertificados();
        });
    },
    
    // Configurar elementos relativos a la operativa de creación/edición de certificados de la comunidad cuando se lance el modal   
    operativaModalCreacionEdicionCertificados: function(){
        const that = this;

        // Botón para guardar/editar un determinado certificado desde el modal
        $(`#${that.btnGuardarCertificacionId}`).on("click", function(){
            const txtNombreCertificacion = $(`#${that.txtNombreCertificacionId}`);
            const isEmpty = comprobarInputNoVacio(txtNombreCertificacion, true, false, that.pParams.mensajes.errorCertificacionNombreVacio,0);
            if (isEmpty == true){
                return
            }

            // Obtener el nuevo nombre
            const nameCertificado = txtNombreCertificacion.val();
            const idCertificado = $(this).data("id");
            const ordenCertificado = $(this).data("orden").length == 0 ? undefined : $(this).data("orden");
            const dataCreatedRecently = $(this).data("new") == true ? true : false;
                        
            // Crear/Editar certificado
            that.handleAddCertificado(nameCertificado, dataCreatedRecently, ordenCertificado, idCertificado)            
            // Ocultar el modal            
            that.modalContainer.modal('hide');            
            // Reordenar posicion de elementos
            that.reorderCertificados();
        });  
    },

    // Configurar elementos relativos a la operativa de eliminación de certificados de la comunidad cuando se lance el modal (Confirmar eliminación)
    operativaModalConfirmarEliminarCertificado: function(){
        const that = this;

        // Boton de aceptar la confirmación del borrado del certificado
        $(`#${that.btnConfirmarBorrarCertificadoId}`).on("click", function(){
            // Obtener la fila del certificado a eliminar a partir del item que disparó el modal                
            const certificateIndexRowToDelete = $(this).data("orden");            
            const certificate = $(that.panelListaCertificados.children()[certificateIndexRowToDelete]);
            that.handleDeleteCertificado(certificate);
            // Ocultar el modalContainer
            that.modalContainer.modal("hide");                        
        });

        // Botón de cancelar la eliminación del borrado del certificado
        $(`#${that.btnCancelarBorrarCertificadoId}`).on("click", function(){
            // Ocultar el modalContainer
            that.modalContainer.modal("hide");
        });        
    },


    /********************************************************************************************/
    /************************** Guardar datos para envío a backend ******************************/
    /********************************************************************************************/

    /**
     * Acción para guardar los cambios y enviarlos a backend
     */
    handleGuardarCertificacionRecursos: function(){
        const that = this;

        // Mostrar loading
        loadingMostrar();
        
        // Reseteamos el objeto de datos guardado
        that.DatosGuardado = {};

        // Prefijo usado para guardar datos de Certificados
        const  prefijo = 'DatosGuardado';
        
        /* Construcción de parámetros para guardado */
        //Permitir activación o no de niveles de certificados
        that.DatosGuardado[prefijo + '.NivelesCertificacionDisponibles'] = that.chkActivarNivelesCertificacion.is(':checked');

        // Contador para la orden de certificados
        let contNiveles = 0;

        // Construir DatosGuardado con cada certificado
        that.panelListaCertificados.children().each(function(){
            let prefijoClave = prefijo + '.NivelesCertificacion[' + contNiveles + ']';            
            // Crear el objeto para id, nombre y orden
            that.DatosGuardado[prefijoClave + '.CertificacionID'] = $(this).data("id");
            that.DatosGuardado[prefijoClave + '.Nombre'] = $(this).data("nombre");
            that.DatosGuardado[prefijoClave + '.Orden'] = contNiveles;

            contNiveles++;
        });

        // Recoger la información del panel de política de certificación 
        that.DatosGuardado[prefijo + '.PoliticaCertificacion'] = encodeURIComponent(that.txtPoliticaCertificacion.val());

        /* Acción de guardado de datos en backend */
        GnossPeticionAjax(
            that.urlGuardarDatosCertificados,            
            that.DatosGuardado,
            true
        ).done(function (data) {
            // OK            
            mostrarNotificacion("success", "Los cambios se han guardado correctamente");        
        }).fail(function (data) {
            // KO
            var error = data.split('|||');
            if (data != "Contacte con el administrador del Proyecto, no es posible atender la petición.") {
                mostrarNotificacion("error", "Ha habido errores en el guardado de las pesta&#xF1;as, revisa los errores marcados");                                
                if (error[0] == "ERROR_NOMBRE_CERTIFICACION_VACIO") {                    
                    mostrarNotificacion("error", "No puede haber niveles de certificaci&#xF3;n con el nombre en blanco");
                }
            }
            else {
                let entornoBloqueado = "False";

                if (entornoBloqueado == "True") {
                    mostrarNotificacion("error", "El entorno actual esta bloqueado. Hay que desplegar la versión de preproducción antes de hacer cambios");
                }
                else {
                    mostrarNotificacion("error", data);                    
                }

                that.mostrarErrorGuardadoFallo(data);
            }
        }).always(function () {
            loadingOcultar();            
        });
    },
    
}


/**
 * Operativa de funcionamiento de Gestión de Miembros de la comunidad
 */
const operativaGestionMiembros = {

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
        // Url para cargar modal con datos del usuario para Administrar permisos de páginas desde "Administrar Miembros"
        this.urlLoadModalPagePermissions = `${this.urlBase}/load-modal-user-page-permissions`;
        this.urlSaveUserPagePermissions = `${this.urlBase}/save-user-page-permissions`;
        this.urlAddMemberAdmin = `${this.urlBase}/registro-usuario-admin`;
    },

    /*
     * Inicializar elementos de la vista
     * */
    config: function (pParams) {
        const that = this;                
        /* Sección */
        // Elemento del DOM
        // Cada una de las filas de resultados de tipo "Miembro/Persona" que aparecerá en la lista de resultados
        that.resourceProfileClassName = "resource-profile";
        // Contenedor modal para acciones
        that.modalContainer = $("#modal-container");
        // Botón para poder consultar los permisos de los que dispone un usuario en las páginas de administración
        that.btnLoadManagePagePermissionsClassName = "btnLoadManagePagePermissions";
        that.btnSaveUserPermissionsClassName = "btnSaveUserPermissions";
        that.checkAdminPaginasDisenyoClassName = "checkAdminPaginasDisenyo";
        that.checkAdminPaginasPaginasClassName = "checkAdminPaginasPaginas";
        that.checkAdminPaginasSemanticaClassName = "checkAdminPaginasSemantica";
        that.checkAdminPaginasTesuaroClassName = "checkAdminPaginasTesuaro";
        that.checkAdminPaginasTextoClassName = "checkAdminPaginasTexto";
        // Inputs y botón para registro de usuarios desde admin
        // that.txtNombreId = document.getElementById("txtNombre").value;
        // that.txtApellidosId = document.getElementById("txtApellidos").value;
        // that.txtEmailId = document.getElementById("txtEmail").value;
        // that.txtContrasenyaId = document.getElementById("txtContrasenya").value;
        that.btnAddMemberId = "btnAddMemberAdmin";

    },

    /**
     * Lanzar comportamientos u operativas necesarias para el funcionamiento de la sección
     */
     triggerEvents: function(){
        const that = this;   
        
        // Ejecutar operativa de Facetas/Calendarios
        operativaFechasFacetas.init();
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


        configEventByClassName(`${that.btnLoadManagePagePermissionsClassName}`, function(element){
            const $jqueryButton = $(element);
            $jqueryButton.off().on("click", function(){ 
                const identidadID = $(this).data("user-id");                                                                                              
                that.handleLoadModalWithUserPermissionsOnAdminPages(identidadID);
            });	                        
        });

        configEventByClassName(`${that.btnSaveUserPermissionsClassName}`, function(element){
            const $jqueryButton = $(element);
            $jqueryButton.off().on("click", function(){ 
                const identidadID = $(this).data("user-id");                                                                                              
                that.handleSaveUserPermissionsOnAdminPages(identidadID);
            });	                        
        });

        $(`#${this.btnAddMemberId}`).off().on("click", function(){
            that.handleAddNewMember();
        });
    },    


    /**
     ****************************************************************************************************************************
     * Acciones a realizar sobre los miembros de la comunidad (Enviar/No enviar Newsletters, Expulsar, Bloquear, Añadir miembro Cambiar rol...)
     * **************************************************************************************************************************
     */  


     /**
      * Método para añadir un nuevo miembro a la comunidad
      */
     handleAddNewMember: function(){
        const that = this;  

        that.txtNombreId = document.getElementById("txtNombre").value;
        that.txtApellidosId = document.getElementById("txtApellidos").value;
        that.txtEmailId = document.getElementById("txtEmail").value;
        that.txtContrasenyaId = document.getElementById("txtContrasenya").value;

         
        const arg = {
            nombre: that.txtNombreId,
            apellidos: that.txtApellidosId,
            correo: that.txtEmailId,
            contrasenya: that.txtContrasenyaId
        };

    
        loadingMostrar();
        GnossPeticionAjax(
            this.urlAddMemberAdmin,
            arg,
            true
        ).done(function (response) {
            // OK    
            // Ocultar modal
            loadingOcultar();
            pJqueryModalView = $("#add_member");
            dismissVistaModal(pJqueryModalView);
            mostrarNotificacion("sucess","usuario añadido correctmente");
        }).fail(function (error) {
            // KO
            loadingOcultar();                        
            mostrarNotificacion("error", error);            
        }).always(function () {            
            loadingOcultar();
        });

        

     },


    
    /**
     * Acción para realizar la acción sobre los miembros de la comunidad
     * @param {} url : Url donde se realizará la acción sobre el miembro de la comunidad
     * @param {*} dataPost : el objeto o callback para que backend sepa qué acción acometer
     */
    handleAccionMiembro: function(url, dataPost){
        // Realizar la petición para abrir la comunidad en definición
        GnossPeticionAjax(
            url,
            dataPost,
            true
        ).done(function (response) {
            // OK    
            // Ocultar modal
            dismissVistaModal();
        }).fail(function (error) {
            // KO                        
            mostrarNotificacion("error", error);            
        }).always(function () {            
            loadingOcultar();
        });
    },

    /**
     * Función para no enviar Newsletters a un determinado usuario
     * @param {*} url : Url a la que se realizará la petición
     */
    noEnviarNewsletter: function(url){
        const that = this;
        // Mostrar loading
        loadingMostrar();
        // Construcción del objeto
        const dataPost = {
            callback: "Accion_NoEnviarNewsletter"
        }
        that.handleAccionMiembro(url, dataPost);
    },

     /**
     * Función para permitir enviar Newsletters a un determinado usuario
     * @param {*} url : Url a la que se realizará la petición
     */
    enviarNewsletter: function(url){
        const that = this;
        // Mostrar loading
        loadingMostrar();
        // Construcción del objeto
        const dataPost = {
            callback: "Accion_EnviarNewsletter"
        }
        // Realizar la petición para abrir la comunidad en definición
        that.handleAccionMiembro(url, dataPost);
    },

     /**
     * Función para readmitir a un miembro de la comunidad
     * @param {*} url : Url a la que se realizará la petición
     */
    readmitirMiembro: function(url){
        const that = this;
        // Mostrar loading
        loadingMostrar();
        // Construcción del objeto
        const dataPost = {
            callback: "Accion_Readmitir"
        }        

        // Realizar la petición para abrir la comunidad en definición
        that.handleAccionMiembro(url, dataPost);
    },


     /**
     * Función para bloquear a un miembro de la comunidad
     * @param {*} url : Url a la que se realizará la petición
     */
    bloquearMiembro:function(url){
        const that = this;
        // Mostrar loading
        loadingMostrar();
        // Construcción del objeto
        const dataPost = {
            callback: "Accion_Bloquear"
        }

        // Realizar la petición para abrir la comunidad en definición
        that.handleAccionMiembro(url, dataPost);
    },

     /**
     * Función para desbloquear a un miembro de la comunidad
     * @param {*} url : Url a la que se realizará la petición
     */
    desbloquearMiembro:function(url){  
        const that = this;
        // Mostrar loading
        loadingMostrar();
        // Construcción del objeto
        const dataPost = {
            callback: "Accion_Desbloquear"
        }

        // Realizar la petición para abrir la comunidad en definición
        that.handleAccionMiembro(url, dataPost);
    },
    
    /**
     * Acción que se ejecuta cuando se pulsa sobre la acción de "Cambiar rol" disponible en un item/recurso de tipo "Perfil" encontrado por el buscador.  
     * @param {string} id: Identificador del recurso (en este caso de la persona) sobre el que se aplicará la acción 
     * @param {any} rol: El rol actual del recurso (Perfil) clickeado
     * @param {any} urlPagina: Pagina sobre la que se lanzará la llamada para realizar la acción de cambiar rol
     * @param {any} idModalPanel: Panel modal contenedor donde se insertará este HTML (Por defecto será #modal-container)
     */
    cambiarRolMiembro: function (id, rol, urlPagina, namePerson, idModalPanel = "#modal-container") {
        // Acción que se ejecutará al pulsar sobre el botón primario (Realizar la acción de cambiar rol)
        const accion = "operativaGestionMiembros.handleCambiarRolMiembro('" + urlPagina + "', '" + id + "', '" + rol + "');";
        // Permisos para pintar los checkbox a mostrar al usuario
        let checkedAdmin = '';
        let checkedSupervisor = '';
        let checkedUsuario = '';

        if (rol == 0) {
            checkedAdmin = ' checked';
        }
        else if (rol == 1) {
            checkedSupervisor = ' checked';
        }
        else if (rol == 2) {
            checkedUsuario = ' checked';
        }

        // Panel dinámico del modal padre donde se insertará la vista "hija"
        const $modalDinamicContentPanel = $('#modal-container').find('#modal-dinamic-content #content');

        // Plantilla del panel html que se cargará en el modal contenedor al pulsar en la acción
        var plantillaPanelHtml = '';
        // Cabecera del panel
        plantillaPanelHtml += '<div class="modal-header">';
            plantillaPanelHtml += '<p class="modal-title"><span class="material-icons">label</span>' + accionesUsuarioAdminComunidad.cambiarRol + ' a '+ namePerson + '</p>';
            plantillaPanelHtml += '<span class="material-icons cerrar" data-dismiss="modal" aria-label="Close">close</span>';
        plantillaPanelHtml += '</div>';
        // Cuerpo del panel
        plantillaPanelHtml += '<div class="modal-body">';
            plantillaPanelHtml += '<div class="formulario-edicion">';
                plantillaPanelHtml += '<div class="form-group">';
                    plantillaPanelHtml += '<label class="control-label">' + accionesUsuarioAdminComunidad.selecionaRol + '</label>';
                plantillaPanelHtml += '</div>';
                // Cuerpo del panel -> Opciones de checkbox a cargar
                plantillaPanelHtml += '<div class="form-group">';
                    plantillaPanelHtml += '<div class="themed primary custom-control custom-radio">';
                        plantillaPanelHtml += '<input name="cambiarRol_' + id + '" type="radio" id="cambiarRol_administrador' + id + '" value="0" class="custom-control-input"'+ checkedAdmin +'/>';
                        plantillaPanelHtml += '<label class="custom-control-label" for="cambiarRol_administrador' + id + '">'+ accionesUsuarioAdminComunidad.administrador +'</label>';
                    plantillaPanelHtml += '</div>';
                    plantillaPanelHtml += '<div name="cambiarRol' + id + '" class="themed primary custom-control custom-radio">';
                        plantillaPanelHtml += '<input name="cambiarRol_' + id + '" type="radio" id="cambiarRol_supervisor' + id + '" value="1" class="custom-control-input"'+ checkedSupervisor +'/>';
                        plantillaPanelHtml += '<label class="custom-control-label" for="cambiarRol_supervisor' + id + '">'+ accionesUsuarioAdminComunidad.supervisor +'</label>';
                    plantillaPanelHtml += '</div>';
                    plantillaPanelHtml += '<div class="themed primary custom-control custom-radio">';
                        plantillaPanelHtml += '<input name="cambiarRol_' + id + '" type="radio" id="cambiarRol_usuario' + id + '" value="2" class="custom-control-input"'+ checkedUsuario +'/>';
                        plantillaPanelHtml += '<label class="custom-control-label" for="cambiarRol_usuario' + id + '">'+ accionesUsuarioAdminComunidad.usuario +'</label>';
                    plantillaPanelHtml += '</div>';
                plantillaPanelHtml += '</div>'; 

            // Panel de botones para la acción
            plantillaPanelHtml += '<div id="modal-dinamic-action-buttons" class="form-actions">'
                plantillaPanelHtml += '<button class="btn btn-primary" onclick="'+ accion +'">' + accionesUsuarioAdminComunidad.cambiarRol + '</button>'            
            plantillaPanelHtml += '</div>';

        plantillaPanelHtml += '</div>';
        

        // Meter el código de la vista modal en el contenedor padre que viene identificado por el id #modal-container
        // En concreto, hay que buscar la etiqueta modal-dinamic-content #content e insertar el código
        $modalDinamicContentPanel.html(plantillaPanelHtml);
    },

     /**
     * Enviar la nueva petición del cambio de rol una vez se ha pulsado sobre el botón de "Cambiar rol"
     * @param {any} url: Url donde se lanzará la petición para cambiar el rol
     * @param {any} id: Identificador de la persona
     * @param {any} rol: Rol actual de la persona. Si es el mismo, no hará nada
     */
    handleCambiarRolMiembro: function(url, id, rol){
        const that = this;
        const rolNuevo = $('input[name="cambiarRol_' + id + '"]:checked').val();

        if (rolNuevo != rol) {
            // Mostrar loading
            loadingMostrar();            
            // Objeto para realizar la petición
            const dataPost = {
                callback: "Accion_CambiarRol",
                rol: rolNuevo
            }
            // Petición para realizar el cambio de rol
            GnossPeticionAjax(url, dataPost, true).done(function (data) {
                // Cambiar el nombre al botón para que solo sea clickable una vez se recargue la página
                that.cambiarTextoAndEliminarAtributos(id + '_CambiarRol', accionesUsuarioAdminComunidad.rolCambiado, ['onclick', 'data-target', 'data-toggle']);
                // Mostrar mensaje ok                
                mostrarNotificacion("success", accionesUsuarioAdminComunidad.rolCambiado)
                // Ocultar el modal
                dismissVistaModal();
            }).fail(function (data) {                
                mostrarNotificacion("error", "Se ha producido un error al tratar de cambiar el rol.");
            }).always(function (data) {
                // Ocultar loading
                loadingOcultar();
            });        
        }else{
            mostrarNotificacion("info", "El rol debe ser diferente al actual.");
        }
    },


 /**
     * Acción que se ejecuta cuando se pulsa sobre la acción de "Cambiar contraseña" disponible en un item/recurso de tipo "Perfil" encontrado por el buscador.  
     * @param {string} urlUsuario: Direccion url asociada al usuario 
     * @param {string} namePerson: Nombre completo del usuario
     * @param {any} idModalPanel: Panel modal contenedor donde se insertará este HTML (Por defecto será #modal-container)
     */
 cambiarPassword: function (urlUsuario, namePerson,idModalPanel = "#modal-container") {
    // Acción que se ejecutará al pulsar sobre el botón primario (Realizar la acción de cambiar contraseña)
    const accion = "operativaGestionMiembros.handleCambiarPassword('" + urlUsuario  + "');";

    // Panel dinámico del modal padre donde se insertará la vista "hija"
    const $modalDinamicContentPanel = $('#modal-container').find('#modal-dinamic-content #content');

    // Plantilla del panel html que se cargará en el modal contenedor al pulsar en la acción
    var plantillaPanelHtml = '';
    // Cabecera del panel
    plantillaPanelHtml += '<div class="modal-header">';
        plantillaPanelHtml += '<p class="modal-title"><span class="material-icons">edit</span>' + 'Cambiar contraseña' + ' a '+ namePerson + '</p>';
        plantillaPanelHtml += '<span class="material-icons cerrar" data-dismiss="modal" aria-label="Close">close</span>';
    plantillaPanelHtml += '</div>';
    // Cuerpo del panel
    plantillaPanelHtml += '<div class="modal-body">';
        plantillaPanelHtml += '<div class="formulario-edicion">';
            plantillaPanelHtml += '<div class="form-group">';
                plantillaPanelHtml += '<label class="control-label"> Asigna una nueva contraseña al usuario </label>';//cambiar texto
            plantillaPanelHtml += '</div>';
            // Cuerpo del panel -> TextBox a cargar
            plantillaPanelHtml += '<div class="form-group">';
                plantillaPanelHtml += ' <div class="form-group mb-3">';
                    plantillaPanelHtml += '<label class="control-label d-block">Nueva contraseña</label>';
                    plantillaPanelHtml += '<input type="password" class="form-control" name="txtContrasenya" id="txtContrasenya" />';
                    plantillaPanelHtml += '<small class="form-text text-mute">Nueva contraseña de acceso a la cuenta.</small>';
                plantillaPanelHtml += '</div>';
            plantillaPanelHtml += '</div>'; 

        // Panel de botones para la acción
        plantillaPanelHtml += '<div id="modal-dinamic-action-buttons" class="form-actions">'
            plantillaPanelHtml += '<button class="btn btn-primary" onclick="'+ accion +'">Cambiar contraseña</button>'            
        plantillaPanelHtml += '</div>';

    plantillaPanelHtml += '</div>';
    

    // Meter el código de la vista modal en el contenedor padre que viene identificado por el id #modal-container
    // En concreto, hay que buscar la etiqueta modal-dinamic-content #content e insertar el código
    $modalDinamicContentPanel.html(plantillaPanelHtml);
},

 /**
 * Enviar la nueva petición del cambio de contraseña una vez se ha pulsado sobre el botón de "Cambiar contraseña"
 * @param {any} urlUsuario: Direccion url asociada al usuario 
 */
handleCambiarPassword: function(urlUsuario){ 
    const that = this;
    const nombreCortoUsuario = urlUsuario.substring(urlUsuario.lastIndexOf("/") + 1);
    const passNueva = document.getElementById('txtContrasenya').value;
    url = `${this.urlBase}/cambiar-pass-admin`;
    
        // Mostrar loading
        loadingMostrar();            
        // Objeto para realizar la petición
        const dataPost = {
            pass: passNueva,
            nombreCortoUsu: nombreCortoUsuario,
        }

        GnossPeticionAjax(url, dataPost, true)
        .done(function (response) {
            // Ocultar el modal
            dismissVistaModal();
        }).fail(function (error) {                
            mostrarNotificacion("error", error);
        }).always(function () {
            // Ocultar loading
            loadingOcultar();
        });        
   
},




    /**
     * Acción para expulsar a una persona de una comunidad. Se ejecuta cuando (por ejemplo) se selecciona desde listado de personas, la opción de "Expulsar"
     * Se cargará un nuevo modal para hacer la gestión de la explusión
     * @param {any} urlPagina: Url a la que se realizará la petición para expulsar a la persona
     * @param {any} id: Identificador de la persona a la que se expulsará
     * @param {any} titulo: Titulo de la ventana modal
     * @param {any} textoBotonPrimario: Titulo del botón primario
     * @param {any} textoBotonSecundario: Titulo del botón secundario (No/Cancelar)
     * @param {any} texto: Explicación de la acción de expulsar usuario
     * @param {any} accionCambiarNombreHtml: Accion JS que servirá para cambiar el nombre del elemento una vez se proceda a expulsar a una persona.
     * @param {any} namePerson: Nombre de la persona la que se le aplicará la acción.
     * @param {any} idModalPanel: Panel modal contenedor donde se insertará este HTML (Por defecto será #modal-container)
     */
    expulsarMiembro: function (urlPagina, id, titulo, textoBotonPrimario, textoBotonSecundario, texto, accionCambiarNombreHtml, namePerson, idModalPanel = "#modal-container") {
    
        // Acción que se ejecutará al pulsar sobre el botón primario (Realizar la acción de Expulsar)
        const accion = "operativaGestionMiembros.handleExpulsarMiembro('" + urlPagina + "', '" + id + "');";

        // Panel dinámico del modal padre donde se insertará la vista "hija"
        const $modalDinamicContentPanel = $('#modal-container').find('#modal-dinamic-content #content');

        // Indicar la persona a la que se le aplica la acción
        const destinoAccionPersona = namePerson != "" ? `a ${namePerson}` : "";            

        // Plantilla del panel html que se cargará en el modal contenedor al pulsar en la acción
        var plantillaPanelHtml = '';
        // Cabecera del panel
        plantillaPanelHtml += '<div class="modal-header">';
            plantillaPanelHtml += '<p class="modal-title"><span class="material-icons">label</span>' + titulo + destinoAccionPersona +'</p>';
            plantillaPanelHtml += '<span class="material-icons cerrar" data-dismiss="modal" aria-label="Close">close</span>';
        plantillaPanelHtml += '</div>';
        // Cuerpo del panel
        plantillaPanelHtml += '<div class="modal-body">';
            plantillaPanelHtml += '<div class="formulario-edicion">';
                plantillaPanelHtml += '<div class="form-group">';
                    plantillaPanelHtml += '<label class="control-label">' + texto + '</label>';
                plantillaPanelHtml += '</div>';

            // Cuerpo del panel -> TextArea para enviar un email explicando la razón de la expulsión
                plantillaPanelHtml += '<div class="form-group">';
                    plantillaPanelHtml += '<label class="control-label d-block" for="txtMotivoExpulsion_'+ id +'">'+ accionesUsuarioAdminComunidad.motivoExpulsion +'</label>';
                    plantillaPanelHtml += '<textarea class="form-control" id="txtMotivoExpulsion_'+id+'" rows="3"></textarea>';
                plantillaPanelHtml += '</div>';

                plantillaPanelHtml += '<div id="modal-dinamic-action-response">';
            // Posibles mensajes de info
                    plantillaPanelHtml += '<div>';
                        plantillaPanelHtml += '<div id="menssages">';
                            plantillaPanelHtml += '<div class="ok"></div>';
                            plantillaPanelHtml += '<div class="ko"></div>';
                        plantillaPanelHtml += '</div>';
                    plantillaPanelHtml += '</div>';
                // Panel de botones para la acción
                    plantillaPanelHtml += '<div id="modal-dinamic-action-buttons" class="form-actions">'
                        plantillaPanelHtml += '<button data-dismiss="modal" class="btn btn-primary">' + textoBotonSecundario + '</button>'
                        plantillaPanelHtml += '<button class="btn btn-outline-primary ml-1" onclick="' + accion + '">'+ textoBotonPrimario + ", " + accionesUsuarioAdminComunidad.expulsarUsuario + '</button>'
                    plantillaPanelHtml += '</div>';
                plantillaPanelHtml += '</div>';
        plantillaPanelHtml += '</div>';

        // Insertar la plantilla Html en el modal        
        $modalDinamicContentPanel.html(plantillaPanelHtml);

        // Asignar acciones adicionales al botó primario (Cambiar nombre del html)
        // Acceso a los botones
        const botones = $modalDinamicContentPanel.find('#modal-dinamic-action-buttons > button');

        // Asignación de la función al botón "Sí" o de acción
        $(botones[1]).on("click", function () {
            // Ocultar el panel modal de bootstrap si hiciera falta        
        }).click(accionCambiarNombreHtml);
    },

    /**
     * Acción que se ejecutada una vez se confirma el deseo de expulsar a un miembro de la comunidad     
     * @param {any} urlPagina: Url donde se lanzará la petición para cambiar el rol
     * @param {any} id: Identificador de la persona     
     */
    handleExpulsarMiembro: function(url, id){

        if ($('#txtMotivoExpulsion_' + id).val() != '') {
            const motivo = $('#txtMotivoExpulsion_' + id).val();
            // Mostrar loading
            loadingMostrar();

            // Construcción del objeto para expulsar al usuario
            const dataPost = {
                callback: "Accion_Expulsar",
                motivo: motivo
            }
            // Realizar la petición para expulsar a un miembro
            GnossPeticionAjax(url, dataPost, true).done(function (data) {
                // OK
                mostrarNotificacion("success", accionesUsuarioAdminComunidad.miembroExpulsado); 
                // Cerrar el modalContainer
                dismissVistaModal();                                                
            }).fail(function (data) {
                // KO                                
                mostrarNotificacion("error", "Error al tratar de expulsar al miembro de la comunidad");
            }).always(function (data) {
                // Ocultar loading
                loadingOcultar();
            });
        }
        else {
            mostrarNotificacion("info", accionesUsuarioAdminComunidad.expulsionMotivoVacio);            
        }
    },

    /**
     * Eliminará los atributos del botón para que no pueda volver a ejecutar nada a menos que se vuelva a cargar la página web
     * Ej: Acciones que se hacen sobre un miembro de la comunidad ("No enviar newsletter, Bloquear, Expulsar")
     * @param {any} elementId: Elemento que se desea cambiar el nombre y eliminar atributos
     * @param {any} nombre: Nombre que tendrá el botón una vez se haya pulsado sobre él y las acciones se hayan realizado
     * @param {any} listaAtributos: Lista de atributos en formato String que serán eliminados del botón (Ej: "data-target", "href", "onclick")
     * */    
    cambiarTextoAndEliminarAtributos: function(elementId, nombre, listaAtributos){
        // Seleccionamos el elemento
        const element = $('#' + elementId);
        // Eliminar la lista de atributos deseados
        listaAtributos.forEach(atributo => $(element).removeAttr(atributo));
        // Cambiamos el nombre del elemento
        $(element).html(nombre)
        // Añadimos estilo para que no parezca que es "clickable"
        $(element).css('cursor', 'auto');
    },


    /**
     * Cargar los diferentes estados de los usuarios en la tabla una vez la petición de traer datos y acciones ha concluido     
     * Sustituirá el "loading" en cada columna una vez los datos hayan venido de backEnd vía JS desde ObtenerAccionesListadoMVC
     * */       
    cargarEstadosMiembrosEnTablaResultados: function(){
        const that = this;        
        // Todos los miembros que aparecen en la tabla de Miembros        
        // Ejecutar esta función una vez las acciones de los resultados han sido pintadas en pantalla
        const resourcesProfile = $(`.${that.resourceProfileClassName}`);
        // Recorrer sus acciones para obtener los resultados a mostrar en la tabla
        $.each(resourcesProfile, function () {
            const keyId = $(this).attr('id');
            // Valores por defecto a mostrar en la tabla si no hay datos                                                
            const boletinValue = $(this).find(`#${keyId}_EnviarNewsletter`).data("value") != undefined ? $(this).find(`#${keyId}_EnviarNewsletter`).data("value") : "-";
            const rolValue = $(this).find(`#${keyId}_CambiarRol`).data("value") != undefined ? $(this).find(`#${keyId}_CambiarRol`).data("value") : "-" ;
            const expulsadoValue = $(this).find(`#${keyId}_Expulsar`).data("value") != undefined ? $(this).find(`#${keyId}_Expulsar`).data("value") : "-" ;
            const bloqueadoValue = $(this).find(`#${keyId}_Bloquear`).data("value") != undefined ? $(this).find(`#${keyId}_Bloquear`).data("value") : "-" ;
            // Asignar los valores a la tabla
            $(`#rol_${keyId}`).html(rolValue);
            $(`#boletin_${keyId}`).html(boletinValue);
            $(`#expulsado_${keyId}`).html(expulsadoValue);
            $(`#bloqueado_${keyId}`).html(bloqueadoValue);
        });                
    },

    
    /**
     * Método para cargar el modal que mostrará los diferentes permisos que tendrá de un determinado usuario en las páginas de Administración
     * El modal obtenido se insertará dentro del "modal-container"
     * @param {*} identidadID 
     */
    handleLoadModalWithUserPermissionsOnAdminPages: function(identidadID){        
        const that = this;

        // identidadID del que se desean obtener los datos o permisos existentes
        const formData = {
            identidadID: identidadID,
        }

        // Petición para cargar los datos de los permisos de los que dispone el usuario en el modal
        getVistaFromUrl(that.urlLoadModalPagePermissions, `modal-dinamic-content`, formData, function(result){
            // OK
            if (result != requestFinishResult.ok){                   
                dismissVistaModal();         
                mostrarNotificacion("error", "Se ha producido un error al cargar los permisos de los que dispone el usuario seleccionado. Por favor, inténtalo de nuevo más tarde o contacta con el administrador.");
            }                            
            loadingOcultar();
        }, true);                 
    },
    
    /**
     * Método para guardar los permisos que tendrá un determinado usuario sobre las páginas de una comunidad
     * @param {*} identidadID Id del usuario al que se le asignarán los permisos sobre las páginas de Administración.
     */
    handleSaveUserPermissionsOnAdminPages: function(identidadID){
        const that = this;

        loadingMostrar();

        // Objeto para construir los permisos de los que dispondrá un usuario
        const permisosPaginaUsuario = {
            identidadID : identidadID,
            Disenyo: $(`.${that.checkAdminPaginasDisenyoClassName}`).is(':checked') ? true : false,
            Pagina: $(`.${that.checkAdminPaginasPaginasClassName}`).is(':checked') ? true : false,
            Semantica: $(`.${that.checkAdminPaginasSemanticaClassName}`).is(':checked') ? true : false,
            Tesauro: $(`.${that.checkAdminPaginasTesuaroClassName}`).is(':checked') ? true : false,
            Texto: $(`.${that.checkAdminPaginasTextoClassName}`).is(':checked') ? true : false,            
        };

        // Guardar configuración
        GnossPeticionAjax(
            that.urlSaveUserPagePermissions,
            permisosPaginaUsuario,
            true
        ).done(function (data) {
            // Guardado OK
            dismissVistaModal();                             
            setTimeout(function () {                                
                mostrarNotificacion("success", "Los cambios se han guardado correctamente");   
            }, 500);                        
        }).fail(function (data) {
            // Guardado KO            
            mostrarNotificacion("error", data);            
        }).always(function () {            
            loadingOcultar();
        });                   
    }
}


/**
 * Operativa de funcionamiento de Gestión de Interacción social de los usuarios en la comunidad
 */
const operativaGestionInteraccionSocial = {

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
        // Url para editar un certificado
        this.urlGuardarInteraccionSocial = `${location.href}/save-social-integration`;       
    },

    /*
     * Inicializar elementos de la vista
     * */
    config: function (pParams) {
            
        // Botón para guardar el contenido de Interacción social
        this.btnGuardarInteraccionSocial = $(`#btnGuardarInteraccionSocial`);
        // Panel de información dellada acerca de votaciones de recursos
        this.panelVotacionesRecursos = $(`#panelVotaciones`);        
        // Checkbox para activar las votaciones disponibles
        this.chkVotacionesDisponibles = $(`#VotacionesDisponibles`);
        // Checkbox para activar las votaciones negativas
        this.chkPermitirVotacionesNegativas = $(`#PermitirVotacionesNegativas`);
        // Checkbox para activar las invitaciones
        this.chkInvitacionesDisponibles = $(`#InvitacionesDisponibles`);
        // Checkbox para mostrar las votaciones de recursos
        this.chkMostrarVotaciones = $(`#MostrarVotaciones`);
        // Checkbox para permitir los comentarios
        this.chkComentariosDisponibles = $(`#ComentariosDisponibles`);
        // Checkbox para delegación de control de grupos a Supervisores
        this.chkSupervisoresPuedenAdministrarGrupos = $(`#SupervisoresPuedenAdministrarGrupos`);
        // Checkbox para permitir compartir recursos en otras comunidades
        this.chkCompartirRecursoPermitido = $(`#CompartirRecursoPermitido`);
        // Checkbox para permitir recursos privados
        this.chkPermitirRecursosPrivados = $(`#PermitirRecursosPrivados`);
        // Input donde se guarda el nº de caracteres en los boletines de Suscripción
        this.txtNumeroCaracteresDescripcionSuscripcion = $(`#NumeroCaracteresDescripcionSuscripcion`);
        
        // Objeto para guardar los datos y enviar a backend
        this.Options = {};
    },

    /**
     * Lanzar comportamientos u operativas necesarias para el funcionamiento de la sección
     */
     triggerEvents: function(){
        const that = this;                           
    },    

    /**
     * Configuración de eventos de elementos del Dom (Botones, Inputs...)     
     */
    configEvents: function (pParams) {
        const that = this;
                
        // Botón para guardar toda la información relativa a Interacción social
        this.btnGuardarInteraccionSocial.on("click", function(){
            // Gestionar el guardado de categorías
            that.handleGuardarInteraccionSocial();            
        });

        // Click en un Votaciones disponibles
        this.chkVotacionesDisponibles.on("click", function () {
            if ($(this).is(':checked')) {
                that.handleVerPanelVotaciones(true);
            }else{
                that.handleVerPanelVotaciones(false);
            }
        });
    
    }, 
    
    
    /**
     * Método para recoger y formar el objeto que se enviará a backend para su guardado
     */
    obtenerDatos: function(){       
        const that = this;
        
        that.Options['InvitacionesDisponibles'] = this.chkInvitacionesDisponibles.is(':checked');
        that.Options['VotacionesDisponibles'] = this.chkVotacionesDisponibles.is(':checked');
        that.Options['PermitirVotacionesNegativas'] = this.chkPermitirVotacionesNegativas.is(':checked');
        that.Options['MostrarVotaciones'] = this.chkMostrarVotaciones.is(':checked');
        that.Options['ComentariosDisponibles'] = this.chkComentariosDisponibles.is(':checked');
        that.Options['SupervisoresPuedenAdministrarGrupos'] = this.chkSupervisoresPuedenAdministrarGrupos.is(':checked');
        that.Options['CompartirRecursoPermitido'] = this.chkCompartirRecursoPermitido.is(':checked');
        that.Options['PermitirRecursosPrivados'] = this.chkPermitirRecursosPrivados.is(':checked');
        that.Options['NumeroCaracteresDescripcionSuscripcion'] = this.txtNumeroCaracteresDescripcionSuscripcion.val();
    },

    /**
     * Método para realizar el guardado de la configuración establecida para Interacción social
     */    
    handleGuardarInteraccionSocial: function(){        
        const that = this;                
        // Construir el objeto para envío a backend
        that.obtenerDatos();
        
        // Mostrar loading        
        loadingMostrar();

        // Hacer petición para realizar guardado
        GnossPeticionAjax(
            that.urlGuardarInteraccionSocial,
            that.Options,
            true
        ).done(function (data) {
            // OK
            mostrarNotificacion("success", "Los cambios se han guardado correctamente");            
        }).fail(function (data) {
            // KO
            const error = data.split('|||');
            if (data != "Contacte con el administrador del Proyecto, no es posible atender la petición.") {                
                mostrarNotificacion("error","Ha habido errores en el guardado");
            }
            else
            {                
                mostrarNotificacion("error",data);
            }
        }).always(function () {
            // Ocultar loading
            loadingOcultar();
        });
    },

    /**
     * Método para comprobar si se debe visualizar u ocultar el panel de Votaciones de recursos que contiene información detallada.
     * @param {Bool} show : Valor bool que indicará si se desea visualizar o no el panel de votaciones
     */
    handleVerPanelVotaciones: function(show){
        const that = this;
        if (show == true){
            that.panelVotacionesRecursos.removeClass("d-none");
        }else{
            that.panelVotacionesRecursos.addClass("d-none");
        }
    },
    

}

/**
 * Operativa de funcionamiento de Gestión de integración de Sharepoint
 */
const operativaGestionIntegracionSharepoint = {
    
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

        // Comprobación inicial de errores en inputs
        that.handleCheckForErrorsValues();

    },   

    /**
     * Configuración de las rutas de acciones necesarias para hacer peticiones a backend
     */
    configRutas: function (pParams) {
        // Url para editar un certificado
        this.urlBase = refineURL();
        this.urlSave = `${this.urlBase}/save`;     
        
        // Flag de control de errores
        this.errorsBeforeSaving = false;
    },  

    /*
    * Inicializar elementos de la vista
    * */
    config: function (pParams) {                
        // Inputs de sharepoint
        this.clientID = $("#clientID");
        this.tenantID = $("#tenantID");
        this.clientSecret = $("#clientSecret");
        // Urls
        this.urlLoginSharepoint = $("#urlLoginSharepoint");
        this.urlObtenerTokenSharepoint = $("#urlObtenerTokenSharepoint");
        this.urlRedireccionSharepoint = $("#urlRedireccionSharepoint");
        // Inputs hidden
        this.urlAdminConsent = $("#urlAdminConsent");
        this.dominio = $("#dominio");

        // Botón para guardar valores
        this.btnSaveSharepoint = $("#btnSaveSharepoint");
        // Checkbox de vinculación de datos de Sharepoint
        this.vincularOneDrive = $("#VincularOneDrive");

        // Links de ayuda y permisos
        this.linkSeeManual = $("#linkSeeManual");
        this.linkRequestPermissions = $("#linkRequestPermissions");
    },   

    /**
     * Configuración de eventos de elementos del Dom (Botones, Inputs...)     
     */
    configEvents: function (pParams) {
        const that = this;
               
        // Botón para conectarse con Microsoft o solicitar permisos para la aplicación
        this.linkRequestPermissions.on("click", function(){
            that.handleRequestMicrosoftPermission();
        });


        // Keyup del input de clienteId
        this.clientID.on("blur", function(){            
            that.handleCheckEmptyInputs();
        });

        // Keyup del input de clientSecret
        this.clientSecret.on("blur", function(){            
            that.handleCheckEmptyInputs();
        });
        
        // Keyup del input de tenandId
        this.tenantID.on("blur", function(){            
            that.handleCheckEmptyInputs();
        });

        // Focusout del input urlLoginSharepoint
        this.urlLoginSharepoint.on("blur", function(){
            that.handleCheckForErrorsValues();
        });

        // Focusout del input urlLoginSharepoint
        this.urlObtenerTokenSharepoint.on("blur", function(){
            that.handleCheckForErrorsValues();
        });

        // Focusout del input urlRedireccionSharepoint
        this.urlRedireccionSharepoint.on("blur", function(){
            that.handleCheckForErrorsValues();
        });

        // Botón para guardar la configuración de sharepoint
        this.btnSaveSharepoint.on("click", function(){
            that.handleSaveSharepointConfiguration();
        });
    },

    /**
     * Método para comprobar/validar la url de LoginSharepoint     
     * @returns bool: Devuelve si hay error en el input
     */
    handleCheckLoginSharepoint: function(){

        const input = this.urlLoginSharepoint;

        const inputVacio = comprobarInputNoVacio(input, true, false, "Este campo no puede estar vacío.", 0);
        if (inputVacio || (!input.val().includes("https://") && !input.val().includes("/login/LoginSharepoint")) ){
            comprobarInputNoVacio(input, true, false, "La URI introducida no es válida. Debe tener el formato https://dominio/login/LoginSharepoint.", 600);                
            // Con errores
            this.errorsBeforeSaving = true;
            return true;
        }else{
            // Sin errores
            this.errorsBeforeSaving = false;
            return false;
        }
    },

    /**
     * Método para comprobar/validar la url de TokenSharepoint     
     * @returns bool: Devuelve si hay error en el input
     */
     handleCheckTokenSharepoint: function(){

        const input = this.urlObtenerTokenSharepoint;

        const inputVacio = comprobarInputNoVacio(input, true, false, "Este campo no puede estar vacío.", 0);
        if (inputVacio || (!input.val().includes("https://") && !input.val().includes("/login/ObtenerTokenSharepoint")) ){            
            comprobarInputNoVacio(input, true, false, "La URI introducida no es válida, debe tener el formato https://dominio/login/ObtenerTokenSharepoint.", 600);
            // Con errores
            return true;
        }else{
            // Sin errores
            return false;
        }
    },    

    /**
     * Método para comprobar/validar la url de Redireccion Sharepoint     
     * @returns bool: Devuelve si hay error en el input
     */
    handleCheckRedireccionSharepoint: function(){

        const input = this.urlRedireccionSharepoint;

        const inputVacio = comprobarInputNoVacio(input, true, false, "Este campo no puede estar vacío.", 0);
        if (inputVacio || (!input.val().includes("https://") && !input.val().includes("/login/ObtenerTokenSharepoint")) ){
            comprobarInputNoVacio(input, true, false, "La URI introducida no es válida, debe tener el formato https://dominio/login/Redireccion.", 600);
            // Con errores
            return true;
        }else{
            // Sin errores
            return false;
        }
    },  
    
    /**
     * Método para comprobar que los inputs no estén vacíos
     */
    handleCheckEmptyInputs: function(){        
    
        if ( comprobarInputNoVacio(this.clientID, true, false, "Este campo no puede estar vacío.", 0) == true ||
             comprobarInputNoVacio(this.clientSecret, true, false, "Este campo no puede estar vacío.", 0) == true || 
             comprobarInputNoVacio(this.tenantID, true, false, "Este campo no puede estar vacío.", 0) == true){
                this.errorsBeforeSaving = true;                    
             
        }else{
            this.errorsBeforeSaving = false;                    
        }                  
        // Poner en disabled o enabled el botón para el guardado
        this.setSaveButtonDisable(this.errorsBeforeSaving);                    
    },

    /**
     * Método para solicitar permisos vía página de microsoft al pulsar sobre "Conceder permisos por parte de un administrador"
     */
    handleRequestMicrosoftPermission: function(){
        const that = this;
        // Construcción de la url de microsoft para solicitar consentimiento
        const urlAdminConsent = `https://login.microsoftonline.com/${this.tenantID.val().trim()}/adminconsent?client_id=${this.clientID.val().trim()}`; 
        // Abrir ventana con url construida para solicitar consentimiento de terceros (microsoft)                                   
        window.open(urlAdminConsent,'popup','width=800px,height=800px');     
    },


    /**
     * Método para comprobar que los datos introducidos sean correctos antes de realizar un posible guardado
     */
    handleCheckForErrorsValues: function(){
        const that = this;

        // Comprobar errores de datos introducidos
        if (this.handleCheckLoginSharepoint() == true && this.handleCheckTokenSharepoint() == true && this.handleCheckRedireccionSharepoint() == true){
            // KO. Hay errores
            this.errorsBeforeSaving = true;
        }else{
            // OK. No hay errores
            this.errorsBeforeSaving = false;            
        }  
        this.setSaveButtonDisable(this.errorsBeforeSaving);
    },

    /**
     * Método para cambiar el estado del botón de guardado dependiendo de la existencia de errores
     */
    setSaveButtonDisable: function(){
        this.btnSaveSharepoint.prop("disabled", this.errorsBeforeSaving);
    },


    /**
     * Método para guardar la configuración de Sharepoint
     */
    handleSaveSharepointConfiguration: function(){
        const that = this;

        if (this.errorsBeforeSaving == false){

            // Mostrar loading
            loadingMostrar();

            // Recogida de datos para envío a backend.            
            const urlLoginSharepoint = this.urlLoginSharepoint.val().trim();
            const urlObtenerTokenSharepoint = this.urlObtenerTokenSharepoint.val().trim();
            const urlRedireccionSharepoint = this.urlRedireccionSharepoint.val().trim();
            const dominio = this.dominio.val().trim();
            const clientID = this.clientID.val().trim();
            const clientSecret = this.clientSecret.val().trim();            
            const tenantID = this.tenantID.val().trim();
            // Checkbox de uso de oneDrive            
            const oneDriveChk = this.vincularOneDrive;
            const urlAdminConsent = `https://login.microsoftonline.com/${tenantID}/adminconsent?client_id=${clientID}`;
            let oneDrive = "False";
            if (oneDriveChk.is(':checked'))
            {
                oneDrive = "True";
            }
            
            // Construir objeto para envío de datos
            const dataPost = {
                DominioBase: dominio,
                ClientID: clientID,
                ClientSecret: clientSecret,
                TenantID: tenantID,
                UrlAdminConsent: urlAdminConsent,
                UrlLoginSharepoint: urlLoginSharepoint,
                UrlObtenerTokenSharepoint: urlObtenerTokenSharepoint,
                UrlRedireccionSharepoint: urlRedireccionSharepoint,
                PermitirOneDrive: oneDrive
            }

            GnossPeticionAjax(                
                this.urlSave,
                dataPost,
                true,
                false
            ).done(function (data) {
                that.mostrarGuardadoOK(urlAdminConsent);
            }).fail(function (data) {
                // Guardado KO            
                mostrarNotificacion("error","Se han producido errores en el guardado. Contacta con el administrador.");
            }).always(function () {            
                loadingOcultar();
            });   
          
        }
    },

    /**
     * Método para mostrar el guardado correcto
     */
    mostrarGuardadoOK: function(url){
        // Mostrar notificación OK
        mostrarNotificacion("success", "Los cambios se han guardado correctamente");
        setTimeout(function(){
            window.open(urlAdminConsent,'popup','width=800px,height=800px');
        },500);
    },

}



/**
 ***************************************************************************************
 * Operativas adicionales // Ej: Extraidas de community.js 
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
                    const catIdMovida = $(itemEl).find(".component").first().data("id"); // $(itemEl).children().closest('.component').data('id');                    
                    let catIdPadre = $(itemEl).parents().closest('.component').last().data('id');
                    // Si no existe catIdPadre, se está moviendo una categoría que no tiene padre -> Asignarle la catPadre por defecto
                    if (catIdPadre == undefined){
                        catIdPadre = '00000000-0000-0000-0000-000000000000';
                    }         
                    
                    // Obtener la categoría de la que se pondrá justo debajo de su inmediato superior
                    const categoriaDebajoDeCategoriaArrastrada = $($(itemEl).siblings()[evt.newDraggableIndex - 1]);
                    const catIdDebajoDe = categoriaDebajoDeCategoriaArrastrada.children().data("id");
                    // Posición de la categoría 
                    const newOrderCategory = evt.newDraggableIndex;

                    // Mover y posteriormente ordenar la categoría seleccionada                    
                    operativaCategoriasSortable.handleMoveCategory(catIdMovida, catIdPadre, function(isOk){                        
                        if (isOk){                                                        
                            operativaCategoriasSortable.handleSortCategory(catIdMovida, catIdPadre, newOrderCategory);
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
                // operativaGestionCategorias.panelTesauro.html(data);
                const htmlData = createElementFromHTML(data);
                const txtAccionesTesauroHackValue = $(htmlData).find("#txtAccionesTesauroHack").val();
                // Actualizar el input con las acciones del tesauro para su posterior guardado devueltas de Core/BackEnd
                operativaGestionCategorias.txtAccionesTesauroHack.val(txtAccionesTesauroHackValue);

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
     * @param {*} newOrderCategory : Posición en la que se posicionará la categoría movida
     */
    handleSortCategory: function(catIdMovida, catIdSuperior, newOrderCategory) {
        
        // Comprobar que la categoría superior al menos existe
        if (catIdSuperior != undefined) {
            // El loading sigue mostrándose desde handleMoveCategory -> No mostar nada
        
            const categorias = catIdMovida;

            // Obtener los parámetros comunes
            operativaGestionCategorias.obtenerParametrosComunes();
 
            // Construir objeto para mover categoría
            operativaGestionCategorias.parametrosComunes.CategoriasSeleccionadas =  categorias;
            operativaGestionCategorias.parametrosComunes.parentKey = catIdSuperior; 
            operativaGestionCategorias.parametrosComunes.newOrderCategory = newOrderCategory; 

            GnossPeticionAjax(operativaGestionCategorias.urlSortCategoria, operativaGestionCategorias.parametrosComunes, true)
            .done(function (data) {
                // OK               
                // Cargar los datos en el contenedor del tesauro. Obtener sólo los pasos anteriores                                
                // operativaGestionCategorias.panelTesauro.html(data);
                const htmlData = createElementFromHTML(data);
                const txtAccionesTesauroHackValue = $(htmlData).find("#txtAccionesTesauroHack").val();
                // Actualizar el input con las acciones del tesauro para su posterior guardado devueltas de Core/BackEnd
                operativaGestionCategorias.txtAccionesTesauroHack.val(txtAccionesTesauroHackValue);
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
 * Operativa para la ordenación mediante el arrastre de certificados
 * Usada en: operativaGestionCertificacion
 */
 const operativaCertificadosSortable = {

     init: function(){
        this.certifications.initCertifications();
     },

     certifications: {
         initCertifications: function () {
             var added_certifications = document.getElementById('id-added-certifications-list');
             var added_certifications_options = this.getAddedCertificationOptions();
             Sortable.create(added_certifications, added_certifications_options);
         },
         getAddedCertificationOptions: function () {
             return {
                 group: {
                     name: 'id-added-certifications-list',
                 },
                 sort: true,
                 dragoverBubble: true,
                 handle: '.js-component-sortable-certification',
                 onAdd: function (evt) {
                 },
                 onChoose: function (evt) {
                 },
                 onUnChoose: function (evt) {
                 },
                 onEnd: function(evt){
                   // Reordenar los certificados
                   operativaGestionCertificacion.reorderCertificados();
                 }
             }
         }
     },
 }

/**
 ***************************************************************************************
 * Plugins utilizados / 
 * *************************************************************************************
 */

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
          panelVistaContenedor: undefined, // Panel donde se cargará la vista devuelta por backen (toda la vista de la cabecera)
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
                        
            if(response.indexOf("<div") != -1){                            
                if (plugin.settings.panelVistaContenedor == undefined){
                    showImagePreview(response); 
                    // Ocultar contenedor para arrastrar imagenes
                    plugin.settings.contenedorImagen.addClass("d-none");                                       
                }else{
                    console.log("Imagen subida. Si se desean hacer más tareas, hacerlo vía completion");                
                    if (plugin.settings.completion != undefined){
                        plugin.settings.completion(response, true);
                    }
                }
                // Ocultar contenedor para arrastrar imagenes
                plugin.settings.contenedorImagen.addClass("d-none");
                // Mostrar panel de acciones de imagen (Cambiar / Eliminar)
                plugin.settings.panelAccionesImagen.removeClass("d-none");
            }else{
                plugin.settings.completion(response, false);
            }
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


