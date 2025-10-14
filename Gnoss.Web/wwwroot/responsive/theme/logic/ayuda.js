/**
 ***************************************************************************************
 * Logica / Operativas de comportamiento JS para la sección de Ayuda de la Comunidad del DevTools
 ***************************************************************************************
 **/


/**
 * Operativa de funcionamiento de Diagnóstico de problemas
 */
const operativaDiagnosticoProblemas = {

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
        this.urlBase = refineURL();
        this.urlCheckBusqueda = `${this.urlBase}/solucionar-busqueda`;
        

    },     
    

    /*
     * Inicializar elementos de la vista
     * */
    config: function (pParams) {    

        /* Diagnóstico de problemas */
        this.btnTerminoRecurso = $(".btnComprobarTerminoRecurso");
    },

    /**
     * Lanzar comportamientos u operativas necesarias para el funcionamiento de la sección
     */
     triggerEvents: function(){
        const that = this;
         
         this.btnTerminoRecurso.off().on("click", function () {
             that.handleCheckBusquedaTextoLibre();
         });
    },  

    /**
     * Método que le pasa al controlador los valores de los inputfields término y recurso para que trate de determinar por qué la búsqueda por texto libre no devuelve el recurso deseado
     * 
     */
    handleCheckBusquedaTextoLibre: function () {
        const that = this;
        $(".result-message").html(""); // Se resetean los mensajes anteriores
        $(".result-description").html("");

        const termino = $('.input-termino').val().trim(); 
        const recurso = $('.input-recurso').val().trim();
        loadingMostrar();
        const dataPost = {
            pTermino: termino,
            pRecurso: recurso,
        }

        GnossPeticionAjax(
            that.urlCheckBusqueda,
            dataPost,
            true,
        ).done(function (data) {
            const trozos = data.split('|');
            const mensaje = trozos[0];
            const descripcion = trozos[1];

            $(".result-wrapper").removeClass("d-none");
            $(".result-message").html(mensaje);
            $(".result-description").text(descripcion);
            loadingOcultar();
        }).fail(function (data) {
            mostrarNotificacion("error", data);
            loadingOcultar();
        })  
    },
    
    /**
     * Configuración de eventos de elementos del Dom (Botones, Inputs...)     
     */
     configEvents: function (pParams) {
        const that = this;
                       
        
    }, 

    /**
     ****************************************************************************************************************************
     * Acciones a realizar, funciones
     * **************************************************************************************************************************
     */

 
    /**
    * Método para copiar al portapapeles.     
    * @param {String} string : cadena que se mostrará al usuario y que se copiará al portapapeles
    */
    handleCopyToClipBoard(string) {
        copyTextToClipBoard(string);
    },

    
}




