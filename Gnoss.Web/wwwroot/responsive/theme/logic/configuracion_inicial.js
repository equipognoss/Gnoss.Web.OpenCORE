/**
 ***************************************************************************************
 * Logica / Operativas de comportamiento JS para la configuración inicial de una comunidad/proyecto
 * Esta operativa es la encargada de la configuración de aspectos relativos la configuración de rutas y el nombre y contraseña de usuario administrador.
 * *************************************************************************************
 */

/**
  * Operativa para la gestión y configuración inicial de la comunidad
  */
const operativaGestionConfiguracionInicial = {
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
        
        // Validar primero si hay datos o es la primera carga inicial para dejar por defecto el botón de guardar "deshabilitado"
        (this.txtNombreUsuario.val().length == 0 || this.txtPassword.val() == 0) && that.btnSaveInitialConfiguration.prop("disabled", true);                    
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
        
        
        // Tabs de navegación de paneles de configuración inicial
        this.tabDatosAcceso = $("#tabDatosAcceso");
        this.tabDatosDominio = $("#tabDatosDominio");
        // Input Nombre de usuario
        this.txtNombreUsuario = $("#txtNombreUsuario");
        // Input Password del usuario        
        this.txtPassword = $("#txtPassword");
        // Input Correo del usuario
        this.txtEmailUsuario = $("#txtEmailUsuario");        
        // Botón para generar password
        this.btnGeneratePassword = $("#btnGeneratePassword");
        // Input de la web inicial
        this.txtUrlWeb = $("#txtUrlWeb");
        // Input de la web de proyectos
        this.txtUrlProyectosWeb = $("#txtUrlProjects");
        // radioButton para uso de proyectos publicos y privados
        this.rbUseSameProjectsUrl = $(".rbUseSameProjectsUrl");
        // Panel que contiene la Url de proyectos privados
        this.panelUrlPrivateProjects = $("#panelUrlPrivateProjects");
        // Input de la web intraGnoss
        this.txtUrlIntragnoss = $("#txtUrlIntragnoss");        
        // Nombre de la comunidad
        this.txtNombreComunidad = $("#txtNombreComunidad");
        // Nombre corto de la comunidad
        this.txtNombreCortoComunidad = $("#txtNombreCortoComunidad");     
        // Nombre del panel informativo del tipo de comunidad - Irá seguido del tipo de comunidad (0,1,2)
        this.panParrafoAccesoId = "panParrafoAcceso";
        // RadioButton con el tipo de comunidad
        this.rbCommunityTypeName = 'communityType';
         
        
        // Botón para guardar la configuración inicial
        this.btnSaveInitialConfiguration = $("#btnSaveInitialConfiguration");  
        
        // Tipo de comunidad a crear (Valor de radioButton seleccionado)
        this.communityTypeValueSelected = undefined;
        // Por defecto se utilizarán las mismas URLS para proyectos públicios y privados
        this.useSameUrlPrivate = true;
    },   

    /**
     * Configuración de eventos de elementos del Dom (Botones, Inputs...)     
     */
    configEvents: function (pParams) {
        const that = this;       
                
        // Input del nombre administrador
        this.txtNombreUsuario.off().on("keyup", function(){
            // No permitir caracteres extraños ni mayúsculas
            $(this).val(this.value.replace(/[^a-zA-Z0-9 _ -]/g, '').trim());
            $(this).val(this.value.toLowerCase());                    
        }); 

        this.txtNombreUsuario.off().on("blur", function(){
            comprobarInputNoVacio($(this), true, false, "El nombre corto de la comunidad debe contener al menos 3 caracteres en minúsculas.", 2, 47);
        });
        
        // Correo electrónico del usuario
        this.txtEmailUsuario.off().on("blur", function(){
            if (!isEmail($(this).val().trim())){
                comprobarInputNoVacio($(this), true, false, "El correo electrónico del usuario debe ser válido.", 100, 101);
            }else{
                comprobarInputNoVacio($(this), true, false, "El correo electrónico del usuario debe ser válido.", 5, 47);
            }         
        });
        
        // Input de la contraseña del usuario
        this.txtPassword.on("keyup", function(){
            clearTimeout(that.timer);
            // No permitir espacios
            const inputPassword = $(this);
            $(this).val(this.value.replace(/[^a-zA-Z0-9 _ -]/g, '').trim());                                                
            that.timer = setTimeout(function () {                
                inputPassword.trigger("blur");                       
            },1000);                        
        });       
        
        this.txtPassword.on("blur", function(){
            // No permitir caracteres extraños ni mayúsculas
            const text = $(this).val().trim();
            const isPasswordValid = that.handleCheckValidPassword(text, true, true);
            isPasswordValid 
            ? comprobarInputNoVacio($(this), true, false, "La contraseña debe de tener entre 6 y 12 caracteres y al menos una letra y número", 5, 12)
            : comprobarInputNoVacio($(this), true, false, "La contraseña debe de tener entre 6 y 12 caracteres y al menos una letra y número", 1, 1);  
            
            isPasswordValid ? that.btnSaveInitialConfiguration.prop("disabled", false) : that.btnSaveInitialConfiguration.prop("disabled", true);
        });                 

        // Botón para generar un password aleatorio
        this.btnGeneratePassword.on("click", function(){
            that.handleGenerateRandomPassword();            
            that.txtPassword.trigger("blur");
        });  
        
        // Input de la URL de la web
        this.txtUrlWeb.off().on("blur", function(){
            // Mostrar error si hay menos de 3 caracteres
            comprobarInputNoVacio($(this), true, false, "Es necesario especificar la URL del proyecto web.", 7);
        });

        // Input de la URL de los proyectos web
        this.txtUrlProyectosWeb.off().on("blur", function(){
            // Mostrar error si hay menos de 3 caracteres
            comprobarInputNoVacio($(this), true, false, "Es necesario especificar la URL para los proyectos privados.", 7);
        });
        
        // Opción radioButton de utilizar la misma url para proyectos privados y públicos/restringidos
        this.rbUseSameProjectsUrl.off().on("click", function(){
            const radioButton = $(this);
            that.handleOptionUsarMismaWebsProyectosPublicosPrivados(radioButton);
        });

        // Input de la URL de intraGnoss
        this.txtUrlIntragnoss.off().on("blur", function(){
            // Mostrar error si hay menos de 3 caracteres
            const inputvalue = $(this).val();
            const isLastCharacter = inputvalue.slice(-1);
            isLastCharacter == "/"
            ? comprobarInputNoVacio($(this), true, false, "Es necesario especificar la URL de IntraGnoss y esta ha de finalizar con /.", 7)
            : comprobarInputNoVacio($(this), true, false, "Es necesario especificar la URL de IntraGnoss y esta ha de finalizar con /.", 7, 1);                        
        });            

        // Input de nombre de la comunidad
        this.txtNombreComunidad.off().on("blur", function(){
            // Mostrar error si hay menos de 3 caracteres
            comprobarInputNoVacio($(this), true, false, "El nombre de la comunidad debe contener al menos 3 caracteres.", 2, 47);
        });        
        
        // Input del nuevo nombre corto de la comunidad
        this.txtNombreCortoComunidad.off().on("keyup", function(){
            // No permitir caracteres extraños ni mayúsculas
            $(this).val(this.value.replace(/[^a-zA-Z0-9 _ -]/g, '').trim());
            $(this).val(this.value.toLowerCase());            
        });

        // Input del nuevo nombre corto de la comunidad
        this.txtNombreCortoComunidad.off().on("blur", function(){    
            // Mostrar error si hay menos de 3 caracteres
            comprobarInputNoVacio($(this), true, false, "El nombre corto de la comunidad debe contener al menos 3 caracteres en letras minúsculas.", 2, 47);
        });

        // RadioButton para el tipo de cambio de comunidad
        $(`input[name="${this.rbCommunityTypeName}"]`).off().on("change", function(){
            that.communityTypeValueSelected = $(this).data("value");
            that.handleShowCommunityTypeDescription();
        });
                
        // Botón para guardar
        this.btnSaveInitialConfiguration.on("click", function(){
            that.handleCheckErrorsAndSave();
        });
    },

    /**
     * Método para controlar si se desea o no utilizar la misma url pública para proyectos privados. Mostrará u ocultará el panel de la url privada     
     * @param {*} radioButton : Radiobutton seleccionado
     */
    handleOptionUsarMismaWebsProyectosPublicosPrivados: function(radioButton){
        const that = this;
        // Comprobar si la opción seleccionada es Si/No
        if (radioButton.data("value") == "si"){
            // Ocultar el panel de url privada de proyectos
            that.panelUrlPrivateProjects.addClass("d-none");
            that.useSameUrlPrivate = true;
        }else{
            // Mostrar el panel de url privada de proyectos
            that.panelUrlPrivateProjects.removeClass("d-none");
            that.useSameUrlPrivate = false;
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
     * Método para generar un password aleatorio y establecerlo en el input correspondiente
     */
    handleGenerateRandomPassword: function(){
        const that = this;
        // Generar password y establecerlo en el input correspondiente
        var result           = '';
        var characters       = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789';        
        var charactersLength = characters.length;
        for ( var i = 0; i < 12; i++ ) {            
            result += characters.charAt(Math.floor(Math.random() * charactersLength));
        }    
        const containsANumber = that.handleCheckValidPassword(result, true, false);
        // Generar password hasta que contenga al menos un número
        !containsANumber && this.handleGenerateRandomPassword();                
        const randomPassword = result;
        this.txtPassword.val(randomPassword);
    },

    /**
     * Método para comprobar la validez del password (Al menos 1 caracter y 1 número)
     */
    handleCheckValidPassword: function(text, mustContainsNumber = true, mustContainsCharacter = true){
        const that = this;
        let isPasswordValid = true;
        const regExp = /[a-zA-Z]/g;
        // Validez del password
        const containsNumber = /\d/.test(text);                    
        const containsLetter = regExp.test(text);

        if (mustContainsNumber && mustContainsCharacter){
            isPasswordValid = containsNumber && containsLetter;
        }else if(mustContainsNumber){
            isPasswordValid = containsNumber;
        }else{
            isPasswordValid = containsLetter;
        }
        return isPasswordValid;
    },

    /**
     * Método para comprobar que no se han encontrado errores antes de guardar los datos vía petición a backend
     */
    handleCheckErrorsAndSave: function(){
        const that = this;

        // Comprobar nombre de usuario        
        if (that.txtNombreUsuario.val().length < 2){
            // Mostrar el panel correspondiente y disparar el error
            that.tabDatosAcceso.trigger("click");
            this.txtNombreUsuario.trigger("blur");
            mostrarNotificacion("error", "El nombre del usuario administrador debe contener al menos 3 caracteres.");
            return;
        }   

        // Comprobar Email del usuario
        if (!isEmail(that.txtEmailUsuario.val().trim())){
            // Mostrar el panel correspondiente y disparar el error
            that.tabDatosAcceso.trigger("click");
            this.txtEmailUsuario.trigger("blur");
            mostrarNotificacion("error", "El correo electrónico del usuario debe ser válido.");
            return;
        } 

        // Comprobar contraseña del usuario        
        if (that.txtPassword.val().length < 2){
            // Mostrar el panel correspondiente y disparar el error
            that.tabDatosAcceso.trigger("click");
            this.txtPassword.trigger("blur");
            mostrarNotificacion("error", "La contraseña del usuario administrador debe ser al menos de 8 caracteres de longitud.");
            return;
        }     
        
        // Comprobar la web de proyectos públicos
        if (that.txtUrlWeb.val().length < 7){
            // Mostrar el panel correspondiente y disparar el error
            that.tabDatosDominio.trigger("click");
            this.txtUrlWeb.trigger("blur");
            mostrarNotificacion("error", "Es necesario especificar la URL para los proyectos públicos y restringidos.");
            return;
        }          

        // Comprobar la web de proyectos privados (Sólo si se desea que sean diferentes)
        if (that.useSameUrlPrivate == false){            
            if (that.txtUrlProyectosWeb.val().length < 7){
                // Mostrar el panel correspondiente y disparar el error
                that.tabDatosDominio.trigger("click");
                this.txtUrlProyectosWeb.trigger("blur");
                mostrarNotificacion("error", "Es necesario especificar la URL para los proyectos privados.");
                return;
            }                 
        }   

        // Comprobar nombre de la comunidad
        if (that.txtNombreComunidad.val().length < 2 || that.txtNombreComunidad.val().length > 48){
            // Mostrar el panel correspondiente y disparar el error
            that.tabDatosDominio.trigger("click");
            this.txtNombreComunidad.trigger("blur");
            mostrarNotificacion("error", "El nombre de la comunidad debe contener al menos 3 caracteres.");
            return;
        }

        // Comprobar nombre corto de la comunidad
        if (that.txtNombreCortoComunidad.val().length < 2 || that.txtNombreCortoComunidad.val().length > 48){
            // Mostrar el panel correspondiente y disparar el error
            that.tabDatosDominio.trigger("click");
            this.txtNombreCortoComunidad.trigger("blur");
            mostrarNotificacion("error", "El nombre corto de la comunidad debe contener al menos 3 caracteres en minúsculas.");
            return;
        }     
        
        // Comprobar que se ha seleccionado un tipo de comunidad
        if (that.communityTypeValueSelected == undefined){
            // Mostrar el panel correspondiente y disparar el error
            that.tabDatosDominio.trigger("click");            
            mostrarNotificacion("error", "Selecciona el tipo de comunidad válido antes de continuar.");
            return;            
        }

        // Mostrar loading
        loadingMostrar();
        
        // Construir objeto para envío de datos
        const dataPost = {
            UserName: that.txtNombreUsuario.val().trim(),
            UserEmail: that.txtEmailUsuario.val().trim(),
            UserPassword: that.txtPassword.val().trim(),
            UrlProyectosPublicos: that.txtUrlWeb.val().trim(),
            UrlProyectosPrivados: that.useSameUrlPrivate == true ? that.txtUrlWeb.val().trim() : that.txtUrlProyectosWeb.val().trim(),
            UseSameUrlForPrivateProjects: that.useSameUrlPrivate,
            UrlIntraGnoss: that.txtUrlIntragnoss.val().trim(),
            Name: that.txtNombreComunidad.val().trim(),
            ShortName: that.txtNombreCortoComunidad.val().trim(),
            Description: "",
            Type: that.communityTypeValueSelected,
            CommunityParent: undefined,            
        }
        
        // Petición de creación de datos 
        GnossPeticionAjax(                
            this.urlSave,
            dataPost,
            true,
            false
        ).done(function (data) {
            // OK Guardado
            mostrarNotificacion("success", "Datos guardados correctamente. Vas a ser desconectado porque los datos del usuario han sido modificados.");
            setTimeout(function () {
                window.location.replace(document.location.origin + "/desconectar/redirect/comunidad/" + $("#txtNombreCortoComunidad").val());
            }, 5000)
        }).fail(function (data) {
            // Guardado KO            
            mostrarNotificacion("error","Se han producido errores en el guardado. Contacta con el administrador.");
        }).always(function () {            
            loadingOcultar();
        });          

    },
}
