/**
 ***************************************************************************************
 * Logica / Operativas de comportamiento JS para la sección de Mantenimiento de la Comunidad del DevTools
 ***************************************************************************************
 **/



/**
* Operativa de funcionamiento de Reprocesado de recursos
*/
const operativaReprocesadoRecursos = {

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
        this.urlRegenerarLucene = `${this.urlBase}/regenerar-lucene`;
        this.urlRegenerarVirtuoso = `${this.urlBase}/regenerar-virtuoso`;
        this.urlRegenerarTriples = `${this.urlBase}/regenerar-triples`;
        this.urlNumTriples = `${this.urlBase}/numero-triples`;
    },


    /*
     * Inicializar elementos de la vista
     * */
    config: function (pParams) {

        this.btnRegenerarLucene = $(".btnRegenerarLucene");
        this.btnRegenerarVirtuoso = $(".btnRegenerarVirtuoso");
        this.btnRegenerarSearch = $(".btnRegenerarSearch");
        this.btnRegenerarTodosTriples = $(".btnRegenerartodosTriples");
        this.btnNumRegeneraciones = $(".btnNumRegeneraciones");
        this.btnRegenerarLuceneOC = $(".btnRegenerarLuceneOC");

        this.filtro;
    },

    /**
     * Lanzar comportamientos u operativas necesarias para el funcionamiento de la sección
     */
    triggerEvents: function () {
        const that = this;

        this.btnRegenerarLucene.off().on("click", function () {
            that.handleRegenerarLucene();
        });

        this.btnRegenerarVirtuoso.off().on("click", function () {
            that.handleRegenerarVirtuoso();
        });

        this.btnRegenerarSearch.off().on("click", function () {
            that.handleRegenerarTriples(true);
        });

        this.btnRegenerarTodosTriples.off().on("click", function () {
            that.handleRegenerarTriples(false);
        });

        this.btnNumRegeneraciones.off().on("click", function () {
            that.handleGetNumeroRegeneraciones();
        });

        this.btnRegenerarLuceneOC.off().on("click", function () {
            that.handleRegenerarLuceneOC();
        });
    },

    handleGetNumeroRegeneraciones(){
        const that = this;

        loadingMostrar();

        const dataPost = {
            pFiltros: $(".input-filtro").val().trim().split('|'),
        }


        GnossPeticionAjax(
            that.urlNumTriples,
            dataPost,
            true,
        ).done(function (data) {
            $(".num-recursos").html(data);
            that.filtro=$(".input-filtro").val().trim();
            $(".confirm-results").removeClass("d-none");
            loadingOcultar();
        }).fail(function (data) {
            loadingOcultar();
            mostrarNotificacion("error", data);
        })
    },

    handleRegenerarLucene: function () {
        const that = this;

        loadingMostrar();

        GnossPeticionAjax(
            that.urlRegenerarLucene,
            null,
            true,
        ).done(function (data) {
            loadingOcultar();
            mostrarNotificacion("success", data);
        }).fail(function (data) {
            mostrarNotificacion("error", data);
            loadingOcultar();
        })
    },

    handleRegenerarLuceneOC: function () {
        const that = this;

        const dataPost = {
            pOntologia: $(".cmbSelectObjetosConocimiento").val().trim(),
        }

        loadingMostrar();

        GnossPeticionAjax(
            that.urlRegenerarLucene,
            dataPost,
            true,
        ).done(function (data) {
            loadingOcultar();
            $("#modalRegenerarLuceneOc").modal('hide');
            mostrarNotificacion("success", data);

        }).fail(function (data) {
            mostrarNotificacion("error", data);
            loadingOcultar();
        })
    },

    handleRegenerarVirtuoso: function () {
        const that = this;

        loadingMostrar();
        $.get(
            that.urlRegenerarVirtuoso,
            null,
            function (data) {
                loadingOcultar();
                mostrarNotificacion("success", "Regeneración en curso");
            }
        );
    },

    handleRegenerarTriples: function (regenerarSoloSearch) {
        const that = this;

        loadingMostrar();

        const dataPost = {
            pFiltros: that.filtro.trim().split('|'),
            pRegenerarSoloSearch: regenerarSoloSearch,
        }

        GnossPeticionAjax(
            that.urlRegenerarTriples,
            dataPost,
            true,
        ).done(function (data) {
            loadingOcultar();
            $("#modalRegenerarTriples").modal('hide');
            mostrarNotificacion("success", "Regeneración en curso");
            
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

