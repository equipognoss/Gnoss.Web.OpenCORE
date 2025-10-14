

//TFG LAYO
/**
 * Operativa para la gestión de asistente de despliegue de aplicaciones 
 */

/***********************************************************************************************
 



/*******************************************************************************************/
function refineURL(url = undefined) {
    //get full URL
    const currURL = url == undefined ? window.location.href : url; //get current address

    //Get the URL between what's after '/' and befor '?' 
    //1- get URL after'/'
    // const afterDomain= currURL.substring(currURL.lastIndexOf('/') + 1);
    const afterDomain = currURL;
    //2- get the part before '?'
    const beforeQueryString = afterDomain.split("?")[0];

    return beforeQueryString;
}

const operativaAplicacionEspecifica = {

    /**
   * Inicializar operativa
   */
    init: function (pParams) {
        this.pParams = pParams;
        this.config(pParams);
        this.configEvents();
        this.configRutas();
    },
    /**
   * Configuración de las rutas de acciones necesarias para hacer peticiones a backend
   */
    configRutas: function (pParams) {
        this.urlBase = refineURL();
        //elimina un volumen compartido
        this.urlDeleteVolumenCompartido = `${this.urlBase}/eliminarvolumencompartido`;
        //edita una aplicacion especifica
        this.urlSaveVolumenCompartido = `${this.urlBase}/editaplicacionespecifica`;
        //obtiene los volumenes compartidos de una aplicacion
        this.urlObtenerVolumenCompartidoAplicacion = `${this.urlBase}/obtenervolumencompartidoaplicacion`; 
        this.urlCompilarAplicacionConDatos = `${this.urlBase}/Compilar-Aplicacion-Especifica-Con-Datos`; 
    },
    config: function (pParams) {

        // Boton para guardar un volumen compartido
        this.bton_guardarVolumenCompartido = $("#btn_anadir_vol");
        //Span del desplegable para editar volumenes compartidos
        this.spanEditarVolumenCompartido = $(".liEditarVolumenCompartido");
        //Span del desplegable para eliminar volumenes compartidos
        this.spanEliminarVolumenCompartido = $(".liEliminarVolumenCompartido");
        //Boton NO en el modal de eliminar volumenes compartidos
        this.btnNoEliminarVolumenCompartido = $("#btn_No_Eliminar_Volumen_Compartido_Modal");
        //Boton SI en el modal de eliminar volumenes compartidos
        this.btnSiEliminarVolumenCompartido = $("#btn_Si_Eliminar_Volumen_Compartido_Modal");
        //Boton EDITAR en el modal de editar volumenes compartidos
        this.btnEditarVolumenCompartido = $("#btn_editar_vol_comp");
        //Span del desplegable para editar aplicaciones especificas
        this.spanEditarAplicacionEspecifica = $(".liEditarAplicacionEspecifica");
        //Span del desplegable para eliminar aplicaciones especificas
        this.spanEliminarAplicacionEspecifica = $(".liEliminarAplicacionEspecifica");
        //Boton NO en el modal de eliminar aplicaiones especificas
        this.btnNoEliminarAplicacionEspecifica = $("#btn_No_Eliminar_Aplicacion_Especifica_Modal");
        //Boton SI en el modal de eliminar aplicaciones especificas
        this.btnSiEliminarAplicacionEspecifica = $("#btn_Si_Eliminar_Aplicacion_Especifica_Modal");
        //Span para desplegar la aplicacion
        this.btnDesplegarAplicacionEspecifica = $(".liDesplegarAplicacionEspecifica");
        //Boton NO en el modal de desplegar aplicaiones especificas
        this.btnNoDesplegarAplicacionEspecifica = $("#btn_No_Desplegar_Aplicacion_Especifica_Modal");
        //Boton SI en el modal de desplegar aplicaciones especificas
        this.btnSiDesplegarAplicacionEspecifica = $("#btn_Si_Desplegar_Aplicacion_Especifica_Modal");
        //Boton NO en el modal de eliminar volumen compartido de manera individual
        this.btnNoEliminarVolumenCompartidoAplicacion = $("#btn_No_Eliminar_Volumen_Compartido_Aplicacion_Modal");
        //Boton SI en el modal de elimnar volumen compartido de manera individual
        this.btnSiEliminarVolumenCompartidoAplicacion = $("#btn_Si_Eliminar_Volumen_Compartido_Aplicacion_Modal");
        //Boton NO en el modal de eliminar volumen de la aplicacion de manera individual
        this.btnNoEliminarVolumenIndividualAplicacion = $("#btn_No_Eliminar_Volumen_Individual_Aplicacion_Modal");
        //Boton SI en el modal de elimnar volumen de la plicaion de manera individual
        this.btnSiEliminarVolumenIndividualAplicacion = $("#btn_Si_Eliminar_Volumen_Individual_Aplicacion_Modal");
        //Boton NO en el modal de eliminar variable de la aplicacion de manera individual
        this.btnNoEliminarVariableIndividualAplicacion = $("#btn_No_Eliminar_Variable_Aplicacion_Modal");
        //Boton SI en el modal de elimnar variable de la plicaion de manera individual
        this.btnSiEliminarVariableIndividualAplicacion = $("#btn_Si_Eliminar_Variable_Aplicacion_Modal");
        //Boton NO en el modal de desplegar aplicaiones especificas post compilado
        this.btnNoDesplegarAplicacionEspecificaPostCompilado = $("#btn_No_Desplegar_Aplicacion_Especifica_Post_Compilado_Modal");
        //Boton SI en el modal de desplegar aplicaciones especificas post compilado
        this.btnSiDesplegarAplicacionEspecificaPostCompilado = $("#btn_Si_Desplegar_Aplicacion_Especifica_Post_Compilado_Modal");
        //Span del desplegable para desplegar aplicaciones especificas compiladas correctamente
        this.spanDesplegarAplicacionEspecificaPostCompilado = $(".liDesplegarAplicacionEspecificaPost");




        // Objeto donde se guardará la configuración para su envío a "backend"
        this.options = {};
    },

    /**
     * Configuración de eventos de elementos del Dom (Botones, Inputs...)     
     */
    configEvents: function (pParams) {
        const that = this;


        // Botón para editar los volumeens en el desplegable
        this.spanEditarVolumenCompartido.click(function () {
            that.handleShowModalEditVol(this.id);
        });
        //Boton para eliminar volumenes compartidos
        this.btnSiEliminarVolumenCompartido.click(function () {
            that.handleDeleteVol();
        });
        //Boton que te muestra el modal de eliminar volumen compartido
        this.spanEliminarVolumenCompartido.click(function () {
            that.handleShowModalDeleteVol(this.id);
        });
        //Boton que te oculta el modal de eliminar volumen compartido
        this.btnNoEliminarVolumenCompartido.click(function () {
            that.handleHideModalDeleteVol();
        });
        //Boton que te oculta el modal de eliminar variable
        this.btnNoEliminarVariableIndividualAplicacion.click(function () {
            that.handleHideModalDeleteVar();
        });
        // Botón para mostrar el modal de editar las aplicaciones especificas
        this.spanEditarAplicacionEspecifica.click(function () {
            that.handleShowModalEditEspecificApp(this.id);
        });
        //Boton para eliminar aplicaciones especificas
        this.btnSiEliminarAplicacionEspecifica.click(function () {
            that.handleDeleteApp(this.id);
        });
        //Boton para ocultar el modal al eliminar las aplicaciones especificas
        this.btnNoEliminarAplicacionEspecifica.click(function () {
            that.handleHideModalDeleteApp();
        });
        //Boton para mostrar el modal para eliminar una aplicacion especifica
        this.spanEliminarAplicacionEspecifica.click(function () {
            that.handleShowModalDeleteEspecificApp(this.id);
        });
        //Boton para mostrar el modal para desplegar una aplicacion
        this.btnDesplegarAplicacionEspecifica.click(function () {
            that.handleShowModalDeployApp(this.id);
        });
        //Boton para despelgar aplicaciones especificas
        this.btnSiDesplegarAplicacionEspecifica.click(function () {
            that.handleDeployApp();
        });
        //Boton para ocultar el modal de desplegar las aplicaciones especificas
        this.btnNoDesplegarAplicacionEspecifica.click(function () {
            that.handleHideModalDeployApp();
        });
        //Boton para mostrar el modal para desplegar una aplicacion que se ha compilado correctamente
        this.spanDesplegarAplicacionEspecificaPostCompilado.click(function () {
            that.handleShowModalDeployPostApp(this.id);
        });
        //Boton para despelgar aplicaciones especificas compiladas a través del formulario (es dev)
        this.btnSiDesplegarAplicacionEspecificaPostCompilado.click(function () {
            that.handleDeployPostApp(false);
        });
        //Boton para ocultar el modal de desplegar las aplicaciones especificas compiladas correctamente
        this.btnNoDesplegarAplicacionEspecificaPostCompilado.click(function () {
            that.handleHideModalDeployPostApp();
        });
        //Boton ocultar modal de eliminar volumenes compartidos de manera individual
        this.btnNoEliminarVolumenCompartidoAplicacion.click(function(){
            that.handleHideModalDeleteSharedVolumeApp();
        });
        //Boton eliminar volumenes compartidos de manera individual
        this.btnSiEliminarVolumenCompartidoAplicacion.click(function () {
            that.handleDeleteSharedVolumeApp();
        });
        //Boton ocultar modal de eliminar volumenes de la app de manera individual
        this.btnNoEliminarVolumenIndividualAplicacion.click(function () {
            that.handleHideModalDeleteIndVolumeApp();
        });
        //Boton eliminar volumenes de la app de manera individual
        this.btnSiEliminarVolumenIndividualAplicacion.click(function () {
            that.handleDeleteIndVolumeApp();
        });
        //Boton eliminar variables de la app de manera individual
        this.btnSiEliminarVariableIndividualAplicacion.click(function () {
            that.handleDeleteVarApp();
        });
        


    },

    /**
     * Método para guardar el id del volumen compartido . 
     */
    handleShowModalEditVol: function (id) {
        var liEditar = $("#" + id);
        var idVolumenCompartido = id.replace("li_Editar_", "");
        var nombreVolumenAplicacion = $("#span_" + idVolumenCompartido).text();
        $("#nombreVolCompEdicion").val(nombreVolumenAplicacion);
        $('#VolumenCompartidoIdModalEditar').val(idVolumenCompartido);
        $('#volumen_compartido_editar').modal('show');
    },
    /**
     * Método para editar un volumen compartido. 
     */
    handleShowModalEditVol: function (id) {
        var liEditar = $("#" + id);
        var idVolumenCompartido = id.replace("li_Editar_", "");
        var nombreVolumenAplicacion = $("#span_" + idVolumenCompartido).text();
        $("#nombreVolCompEdicion").val(nombreVolumenAplicacion);
        $('#VolumenCompartidoIdModalEditar').val(idVolumenCompartido);
        $('#volumen_compartido_editar').modal('show');
    },
    /**
     * Metodo para mostrar el modal al eliminar la aplicacion especifica
     */
    handleShowModalDeleteEspecificApp: function (id) {
        var urlIC = $('#inpt_apiIntegracionContinua').val()
        var that = this;
        var liEditar = $("#" + id);
        var idAplicacionEspecifica = id.replace("li_Eliminar_", "");
        $('#AplicacionEspecificaIdModalEliminar').val(idAplicacionEspecifica); 
        $('#modal-delete-page-aplicacion-especifica').modal('show');

    },
    /**
     * Método para mostrar el modal de eliminar 
     */
    handleShowModalDeleteVol: function (id) {
        var liEliminar = $("#" + id);
        var idVolumenCompartido = id.replace("li_Eliminar_", "");
        $('#VolumenCompartidoIdModalEliminar').val(idVolumenCompartido);
        $('#modal-delete-page-volumen-compartido').modal('show');
    },
    /**
     * Método para ocultar el modal de eliminar volumen compartido de una aplicacion. 
     */
    handleHideModalDeleteSharedVolumeApp: function () {
        $('#modal-delete-page-volumen-compartido-aplicacion').modal('hide');
    },
    /**
     * Método para ocultar el modal de eliminar variable de una aplicacion. 
     */
    handleHideModalDeleteVar: function () {
        $('#modal-delete-page-variable-aplicacion').modal('hide');
    },
    /**
    * Método para ocultar el modal de eliminar volumen individual de una aplicacion. 
    */
    handleHideModalDeleteIndVolumeApp: function () {
        $('modal-delete-page-volumen-individual-aplicacion').modal('hide');
    },
    /**
     * Método para ocultar el modal de eliminar aplicaion. 
     */
    handleHideModalDeleteApp: function () {
        $('#modal-delete-page-aplicacion-especifica').modal('hide');
    },
    /**
     * Método para ocultar el modal de eliminar volumen compartido. 
     */
    handleHideModalDeleteVol: function () {
        $('modal-delete-page-volumen-compartido').modal('hide');
    },
    /**
     * Método para eliminar un volumen compartido. 
     */
    handleDeleteVol: function () {
        var idVolumenCompartido = $('#VolumenCompartidoIdModalEliminar').val()
        var nombreVolumenAplicacion = $("#span_" + idVolumenCompartido).text();
        //Post para eliminar el volumen compartido en base de datos
        //Post a savevol para guardar el volumen creado en base de datos
        var that = this;
        var contador_aux_vol = 0;
        that.ListaVolumenes = {};
        that.ListaVolumenes['ListaVolumenes[' + contador_aux_vol + '].Tipo'] = true;
        that.ListaVolumenes['ListaVolumenes[' + contador_aux_vol + '].Nombre'] = nombreVolumenAplicacion;
        that.ListaVolumenes['ListaVolumenes[' + contador_aux_vol + '].VolumenID'] = document.getElementById("VolumenCompartidoIdModalEliminar").value;
        loadingMostrar();
        GnossPeticionAjax(
            that.urlDeleteVolumenCompartido,
            that.ListaVolumenes,
            true
        ).done(function (data) {
            contador_aux_vol--;
            pJqueryModalView = $("#volumen_compartido");
            dismissVistaModal(pJqueryModalView);
            location.reload(); //recargamos la pagina
            mostrarNotificacion("success", "Volumen compartido eliminado con éxito.");
            loadingOcultar();
           

        }).fail(function (data) {
            loadingOcultar();
            var error = data.split('|||');
            if (error[0].startsWith("errorVolEliminado")) {
                mostrarNotificacion("errorAlEliminarColCompartido", error[1])
            }
            else {
                mostrarNotificacion("error", error[1]);
            }
        }).always(function () {
            OcultarUpdateProgress();
        });

    },

    /**
* Método para enseñar el modal para editar una aplicacion específica. 
*/
    handleShowModalEditEspecificApp: function (id) {
        //Eliminamos las fials antiguas
        var tbody = document.getElementById("bodyVolumenesAnadirApp");

        // Obtiene una referencia a todas las filas del tbody excepto la primera
        var filas = tbody.getElementsByTagName("tr");

        // Elimina todas las filas del tbody excepto la primera
        for (var i = filas.length - 1; i > 0; i--) {
            filas[i].parentNode.removeChild(filas[i]);
        }


        //Eliminamos las fials antiguas
        var tbody = document.getElementById("bodyVariablesAnadirApp");

        // Obtiene una referencia a todas las filas del tbody excepto la primera
        var filas = tbody.getElementsByTagName("tr");

        // Elimina todas las filas del tbody excepto la primera
        for (var i = filas.length - 1; i > 0; i--) {
            filas[i].parentNode.removeChild(filas[i]);
        }
        var urlIC = $('#inpt_apiIntegracionContinua').val()
        var that = this;
        var liEditar = $("#" + id);
        var idAplicacionEspecifica = id.replace("li_Editar_", "");
        var nombreAplicacionEspecifica = $("#spanNombre_" + idAplicacionEspecifica).text();
        var nombreCortoAplicacionEspecifica = $("#spanNombreCorto_" + idAplicacionEspecifica).text();
        var repositorioAplicacionEspecifica = $("#spanRepositorio_" + idAplicacionEspecifica).text();
        var dockerfileAplicacionEspecifica = $("#spanDockerFile_" + idAplicacionEspecifica).text();
        var tipoAplicacionEspecifica = $("#tipoApp_" + idAplicacionEspecifica).val();
        var rutaAplicacionEspecifica = $("#rutaRelativa_" + idAplicacionEspecifica).val();
        if (tipoAplicacionEspecifica=="value") {
            checkBoxTipoApp.value = "Web"
            var txtRutaRel = $("#txtRutaRelativa");
            txtRutaRel.val(rutaAplicacionEspecifica);
                /*const textRutaRelativa = document.createElement("input");
                textRutaRelativa.setAttribute("id", "textRutaRelativa");
                textRutaRelativa.setAttribute("type", "text");
                textRutaRelativa.setAttribute("class", "form-control");
                textRutaRelativa.setAttribute("value", rutaAplicacionEspecifica);
                const labelRutaRelativa = document.createElement("label");
                labelRutaRelativa.setAttribute("id", "labelRutaRelativa");
                labelRutaRelativa.textContent = "Ruta Relativa:"
                contenedorRutaRelativa.appendChild(labelRutaRelativa);
                contenedorRutaRelativa.appendChild(textRutaRelativa);*/
        }
        $('#AplicacionIdModalEditar').val(idAplicacionEspecifica);
        that.idAppEspecifica = idAplicacionEspecifica;
        $("#nombre_servicio").val(nombreAplicacionEspecifica);
        $("#git").val(repositorioAplicacionEspecifica);
        $("#tokengit").val($("#tokenGit_"+idAplicacionEspecifica).val());
        $("#dockerfile").val(dockerfileAplicacionEspecifica);
        $("#rama").val($("#rama_" + idAplicacionEspecifica).val());
        loadingMostrar();
        var j = 0; //variable para luego seguir añadiendo en la tabla los volumenes de la applicacoin
        var dataPost = {
            aplicacionId: that.idAppEspecifica
        }
        GnossPeticionAjax(
            //that.urlObtenerVolumenCompartidoAplicacion,
            `${urlIC}/aplicaciones/obtener-volumen-compartido-aplicacion`,
            //that.idAppEspecifica,
            dataPost,
            true
        ).done(function (data) {
            var volumencompartidoAplicacion = data
            for (var i = 0; i < volumencompartidoAplicacion.length; i++) {
                var rutaVol = volumencompartidoAplicacion[i].RutaVolumenCompartido
                var nombreVol = volumencompartidoAplicacion[i].Volumen.Nombre

                //Añadir boton eliminar y añadir a al tabla
                const trFila = document.createElement("tr");
                trFila.setAttribute("id", "trFila_" + contador_vol_modal);
                //boton eliminar
                const tdBotonEliminar = document.createElement("td");
                tdBotonEliminar.setAttribute("id", "tdBotonEliminar_" + contador_vol_modal);
                tdBotonEliminar.setAttribute("class", "tdBotonEliminar");
                const botonEliminarVol = document.createElement("button");
                botonEliminarVol.setAttribute("class", "btn btn-secondary btn_eliminar_vol_modal_anadirapp");
                botonEliminarVol.setAttribute("id", "button_eliminar_vol_modal_anadirapp");
                botonEliminarVol.setAttribute("data-contador", contador_vol_modal);
                botonEliminarVol.setAttribute("name", "Eliminar");
                botonEliminarVol.textContent = "Eliminar";
                botonEliminarVol.setAttribute("type", "button");
                botonEliminarVol.setAttribute("style", "float:right");
               


                //Añadir el input hidden con el id del volumen compartido
                const inputId = document.createElement("input");
                inputId.setAttribute("type", "hidden");
                inputId.setAttribute("id", "selectId_" + contador_vol_modal);
                inputId.setAttribute("value", volumencompartidoAplicacion[i].Volumen.VolumenID);
                trFila.appendChild(inputId);
                contenedor_volumenes.appendChild(trFila);

                const tdNombre = document.createElement("td");
                tdNombre.setAttribute("id", "tdNombre_" + contador_vol_modal);
                const labelNombre = document.createElement("label");
                labelNombre.setAttribute("id", "label_vol" + contador_vol_modal);
                labelNombre.setAttribute("value", nombreVol);
                labelNombre.setAttribute("class", "VolNombre");
                labelNombre.appendChild(document.createTextNode(nombreVol));
                tdNombre.appendChild(labelNombre);
                trFila.appendChild(tdNombre);
                contenedor_volumenes.appendChild(trFila);


                const tdRuta = document.createElement("td");
                tdRuta.setAttribute("id", "tdRuta_" + contador_vol_modal);
                const labelRuta = document.createElement("label");
                labelRuta.setAttribute("id", "labelRuta_vol" + contador_vol_modal);
                labelRuta.setAttribute("value", rutaVol);
                labelRuta.setAttribute("class", "VolRuta");
                labelRuta.appendChild(document.createTextNode(rutaVol));
                tdRuta.appendChild(labelRuta);
                trFila.appendChild(tdRuta);
                contenedor_volumenes.appendChild(trFila);

                const tdTipo = document.createElement("td");
                tdTipo.setAttribute("id", "tdTipo_" + contador_vol_modal);
                const labelTipo = document.createElement("label");
                labelTipo.setAttribute("id", "labelTipo_vol" + contador_vol_modal);
                labelTipo.setAttribute("value", "Compartido");
                labelTipo.setAttribute("class", "VolTipos");
                labelTipo.appendChild(document.createTextNode("Compartido"));
                tdTipo.appendChild(labelTipo);
                trFila.appendChild(tdTipo);
                contenedor_volumenes.appendChild(trFila);

                for (var i = 0; i < combobox_vol.length; i++) {
                    if (combobox_vol.options[i].value == nombreVol) {
                        combobox_vol.remove(i);
                    }
                }
                
                botonEliminarVol.onclick = function () { //funcionamiento del boton eliminar volumen
                    var fila = botonEliminarVol.closest('tr');
                    // Buscar el elemento input dentro de la fila
                    var input = fila.querySelector('input[type="hidden"]');
                    // Obtener el valor del atributo id del input
                    var valueInput = input.value;
                    $('#VolumenCompartidoAplicacionIdVolumenModalEliminar').val(valueInput)
                    $('#VolumenCompartidoAplicacionIdAplicacionModalEliminar').val(idAplicacionEspecifica)
                    $('#modal-delete-page-volumen-compartido-aplicacion').modal('show');
                    let contador_vol = $(this).data("contador");
                    $('#contadorVolumenCompartidoEliminar').val(contador_vol)
                    // var liEliminar = $("#" + id);
                    //var idVolumenCompartido = id.replace("li_Eliminar_", "");
                    //$('#VolumenCompartidoIdModalEliminar').val(idVolumenCompartido);
                    //$('#modal-delete-page-volumen-compartido-aplicacion').show();
                };

                tdBotonEliminar.appendChild(botonEliminarVol);
                trFila.appendChild(tdBotonEliminar);
                contenedor_volumenes.appendChild(trFila);

                contador_aux_vol_app++;
                contador_checkboxes++;
                contador_vol_modal++;
                contadorQueSiempreAumentaVolumen++;
                j = i;
            }

            GnossPeticionAjax(
                //that.urlObtenerVolumenCompartidoAplicacion,
                `${urlIC}/aplicaciones/obtener-volumen-aplicacion-y-variables`,
                //that.idAppEspecifica,
                dataPost,
                true
            ).done(function (data) {
                var volumenAplicacion = data.Volumenes
                for (var i = 0; i < volumenAplicacion.length; i++) {
                    var nombreVolumenApp = volumenAplicacion[i].NombreVolumen;
                    var rutaVolumenApp = volumenAplicacion[i].Ruta;
                    const trFila = document.createElement("tr");
                    trFila.setAttribute("id", "trFila_" + contador_vol_modal);
                    const tdBotonEliminar = document.createElement("td");
                    tdBotonEliminar.setAttribute("id", "tdBotonEliminar" + contador_vol_modal);
                    const botonEliminarVol = document.createElement("button");
                    botonEliminarVol.setAttribute("class", "btn btn-secondary btn_eliminar_vol_modal_anadirapp");
                    botonEliminarVol.setAttribute("id", "button_eliminar_vol_modal_anadirapp");
                    botonEliminarVol.setAttribute("data-contador", contador_vol_modal);
                    botonEliminarVol.setAttribute("name", "Eliminar");
                    botonEliminarVol.textContent = "Eliminar";
                    botonEliminarVol.setAttribute("type", "button");
                    botonEliminarVol.setAttribute("style", "float:right");
                    botonEliminarVol.onclick = function () {
                        var fila = botonEliminarVol.closest('tr');
                        // Buscar el elemento input dentro de la fila
                        var input = fila.querySelector('input[type="hidden"]');
                        // Obtener el valor del atributo id del input
                        var valueInput = input.value;
                        $('#VolumenIndividualAplicacionIdVolumenModalEliminar').val(valueInput)
                        $('#VolumenIndividualAplicacionIdAplicacionModalEliminar').val(idAplicacionEspecifica)
                        $('#modal-delete-page-volumen-individual-aplicacion').modal('show');
                        let contador_vol = $(this).data("contador");
                        $('#contadorVolumenIndividualEliminar').val(contador_vol)
                    };
                    const inputId = document.createElement("input");
                    inputId.setAttribute("type", "hidden");
                    inputId.setAttribute("id", "selectId_" + contador_vol_modal);
                    inputId.setAttribute("value", volumenAplicacion[i].VolumenID);
                    trFila.appendChild(inputId);
                    contenedor_volumenes.appendChild(trFila);
                    const tdNombre = document.createElement("td");
                    tdNombre.setAttribute("id", "tdNombre_" + contador_vol_modal);
                    //const div_Nombre_Vol = document.getElementById("contenedor_variable_nombre"); // Obtener el div contenedor donde se agregará el label
                    const labelNombre = document.createElement("label");
                    labelNombre.setAttribute("id", "label_vol" + contador_vol_modal);
                    labelNombre.setAttribute("value", nombreVolumenApp);
                    labelNombre.setAttribute("class", "VolNombre");
                    labelNombre.appendChild(document.createTextNode(nombreVolumenApp));
                    tdNombre.appendChild(labelNombre);
                    trFila.appendChild(tdNombre);
                    contenedor_volumenes.appendChild(trFila);

                    const tdRuta = document.createElement("td");
                    tdRuta.setAttribute("id", "tdRuta_" + contador_vol_modal);
                    //const div_Nombre_Vol = document.getElementById("contenedor_variable_nombre"); // Obtener el div contenedor donde se agregará el label
                    const labelRuta = document.createElement("label");
                    labelRuta.setAttribute("id", "labelRuta_vol" + contador_vol_modal);
                    labelRuta.setAttribute("value", rutaVolumenApp);
                    labelRuta.setAttribute("class", "VolRuta");
                    labelRuta.appendChild(document.createTextNode(rutaVolumenApp));
                    tdRuta.appendChild(labelRuta);
                    trFila.appendChild(tdRuta);
                    contenedor_volumenes.appendChild(trFila);

                    const tdTipo = document.createElement("td");
                    tdTipo.setAttribute("id", "tdTipo_" + contador_vol_modal);
                    //const div_Nombre_Vol = document.getElementById("contenedor_variable_nombre"); // Obtener el div contenedor donde se agregará el label
                    const labelTipo = document.createElement("label");
                    labelTipo.setAttribute("id", "labelTipo_vol" + contador_vol_modal);
                    labelTipo.setAttribute("value", "Individual");
                    labelTipo.setAttribute("class", "VolTipos");
                    labelTipo.appendChild(document.createTextNode("Individual"));
                    tdTipo.appendChild(labelTipo);
                    trFila.appendChild(tdTipo);
                    contenedor_volumenes.appendChild(trFila);

                    tdBotonEliminar.appendChild(botonEliminarVol);
                    trFila.appendChild(tdBotonEliminar);
                    contenedor_volumenes.appendChild(trFila);

                    contador_vol_modal++;
                    document.getElementById("nombre_vol_individual").value = "";
                    document.getElementById("ruta_vol_ind_modal").value = "";

                }

                //Añadir variables de la aplicacion
                var variableAplicacion = data.Variables
                for (var i = 0; i < variableAplicacion.length; i++) {
                    var nombreVariable = variableAplicacion[i].NombreVariable;
                    var valorPre = variableAplicacion[i].ValorPreproduccion;
                    var valorPro = variableAplicacion[i].ValorProduccion;
                    var valorDev = variableAplicacion[i].ValorDesarrollo;
                    /*let labelVariable = document.createElement("label");
                    labelVariable.textContent = nombreVariable;
                    labelVariable.setAttribute("id", "nombre_variable_" + contador_var_modal);
                    labelVariable.setAttribute("class", "variablesNombre");
                    labelVariable.setAttribute("for", "input_variable");*/

                    //boton eliminar
                    const trFila = document.createElement("tr");
                    trFila.setAttribute("id", "trVariable_" + contador_var_modal);
                    const inputId = document.createElement("input");
                    inputId.setAttribute("type", "hidden");
                    inputId.setAttribute("id", "selectId_" + contador_var_modal);
                    inputId.setAttribute("value", variableAplicacion[i].VariableID);
                    trFila.appendChild(inputId);

                    contenedor_volumenes.appendChild(trFila);
                    const tdBotonEliminar = document.createElement("td");
                    tdBotonEliminar.setAttribute("id", "tdBotonEliminarVar_" + contador_var_modal);
                    const botonEliminarVar = document.createElement("button");
                    botonEliminarVar.setAttribute("class", "btn btn-secondary btn_eliminar_var_modal_anadirapp");
                    botonEliminarVar.setAttribute("id", "btn_eliminar_var_modal_anadirapp");
                    botonEliminarVar.setAttribute("data-contador", contador_var_modal);
                    botonEliminarVar.setAttribute("name", "Eliminar");
                    botonEliminarVar.textContent = "Eliminar";
                    botonEliminarVar.setAttribute("type", "button");
                    botonEliminarVar.onclick = function () { //funcionamiento del boton eliminar variable
                        var fila = botonEliminarVar.closest('tr');
                        // Buscar el elemento input dentro de la fila
                        var input = fila.querySelector('input[type="hidden"]');
                        // Obtener el valor del atributo id del input
                        var valueInput = input.value;
                        $('#IdVariableModalEliminar').val(valueInput)
                        $('#IdAplicacionVariableModalEliminar').val(idAplicacionEspecifica)
                        $('#modal-delete-page-variable-aplicacion').modal('show');
                        let contador_vol = $(this).data("contador");
                        $('#contadorVaraibleEliminar').val(contador_vol)
                        /*let contador_var = $(this).data("contador");
                        //const filaEliminar = $("#bodyVolumenes").find("tr")[contador_var];
                        const filaEliminar = document.getElementById("trVariable_" + contador_var);
                        contenedor_variables.removeChild(filaEliminar);
                        contador_var_modal--;*/
                    };
                    tdBotonEliminar.appendChild(botonEliminarVar);
                   
                    var celdaNombreVariable = document.createElement("td");
                    var txtNombreVariable = document.createElement("span");
                    txtNombreVariable.setAttribute("id", "nombre_variable_" + contador_var_modal);
                    txtNombreVariable.setAttribute("class", "variablesNombre language-component component-name");
                    txtNombreVariable.textContent = nombreVariable;
                    celdaNombreVariable.appendChild(txtNombreVariable);
                    trFila.appendChild(celdaNombreVariable);

                    //CONTENEDOR VARIABLES DESARROLLO
                    const td_des_var = document.createElement("td");
                    const divContenedor_des = document.getElementById("contenedor_variable_des");
                    const textBoxVariableDes = document.createElement("input");
                    textBoxVariableDes.setAttribute("type", "text");
                    textBoxVariableDes.setAttribute("id", "input_variable_des_" + contador_var_modal);
                    textBoxVariableDes.setAttribute("class", "variablesDesarrollo form-control");
                    //añadir al input el valor de desarrollo
                    textBoxVariableDes.setAttribute("value", valorDev);
                    td_des_var.appendChild(textBoxVariableDes);
                    trFila.appendChild(td_des_var);

                    //CONTENEDOR VARIABLES PRE
                    const td_pre_var = document.createElement("td");
                    const textBoxVariablePre = document.createElement("input");
                    textBoxVariablePre.setAttribute("type", "text");
                    textBoxVariablePre.setAttribute("id", "input_variable_pre_" + contador_var_modal);
                    textBoxVariablePre.setAttribute("class", "variablesPreproduccion form-control");
                    textBoxVariablePre.setAttribute("value", valorPre);
                    td_pre_var.appendChild(textBoxVariablePre);
                    trFila.appendChild(td_pre_var);

                    //CONTENEDOR VARIABLES PRO
                    const td_pro_var = document.createElement("td");
                    const textBoxVariablePro = document.createElement("input");
                    textBoxVariablePro.setAttribute("type", "text");
                    textBoxVariablePro.setAttribute("id", "input_variable_pro_" + contador_var_modal); 
                    textBoxVariablePro.setAttribute("class", "variablesProduccion form-control");
                    textBoxVariablePro.setAttribute("value", valorPro);

                    td_pro_var.appendChild(textBoxVariablePro);
                    trFila.appendChild(td_pro_var);
                    //Añado bootn eliminar al final
                    trFila.appendChild(tdBotonEliminar);
                    contenedor_variables.appendChild(trFila);
                    contador_var_modal++;
                    contador_checkboxes++;

                    document.getElementById("nombre_variable").value = "";
                }

                $('#anadir_app_modal').modal('show');

            }).fail(function (data) {
                loadingOcultar();
                var error = data.split('|||');
                if (error[0].startsWith("errorVolumenAplicacion")) {
                    mostrarNotificacion("errorVolumenAplicacion", error[1])
                }
                else {
                    mostrarNotificacion("error", error[1]);
                }
            }).always(function () {
                OcultarUpdateProgress();
            });
        }).fail(function (data) {
            loadingOcultar();
            var error = data.split('|||');
            if (error[0].startsWith("errorVolumenCompartidoAplicacion")) {
                mostrarNotificacion("errorVolumenCompartidoAplicacion", error[1])
            }
            else {
                mostrarNotificacion("error", error[1]);
            }
        }).always(function () {
            OcultarUpdateProgress();
        });
    
        
    },
    /**
   * Método para eliminar volumen compartido asociado a una aplicacion
   */
    handleDeleteSharedVolumeApp: function () {
        var urlIC = $('#inpt_apiIntegracionContinua').val()
        var dataVolPost = {
            appId: $('#VolumenCompartidoAplicacionIdAplicacionModalEliminar').val(),
            volumenId: $('#VolumenCompartidoAplicacionIdVolumenModalEliminar').val()
        }
        GnossPeticionAjax(
            `${urlIC}/aplicaciones/eliminar-volumen-compartido-aplicacion`,
            //that.idAppEspecifica,
            dataVolPost,
            true
        ).done(function () {
            var volumenIdEliminar = $('#VolumenCompartidoAplicacionIdVolumenModalEliminar').val()
            contador_vol = $('#contadorVolumenCompartidoEliminar').val();
            const filaEliminar = document.getElementById("trFila_" + contador_vol);
            var nombre_volumen_compartido = document.getElementById("label_vol" + contador_vol).innerHTML;
            var selectbox = document.getElementById("volumen");
            var option = document.createElement('option');
            option.text = nombre_volumen_compartido;
            option.value = nombre_volumen_compartido;
            option.id = volumenIdEliminar;
            selectbox.appendChild(option);
            contenedor_volumenes.removeChild(filaEliminar);
            contador_vol_modal--;
            mostrarNotificacion("success", "Volumen individual asociado a la aplicación eliminado con éxito.");
            loadingOcultar();
            $('#modal-delete-page-volumen-compartido-aplicacion').modal('hide');
        }).fail(function (data) {

            const error = data.split('|||');
            mostrarNotificacion("error", error);

        }).always(function () {
            loadingOcultar();
        });       
     
    },
    /**
 * Método para eliminar volumen asociado a una aplicacion de manera individual
 */
    handleDeleteIndVolumeApp: function () {
        var urlIC = $('#inpt_apiIntegracionContinua').val()
        var dataVolPost = {
            appId: $('#VolumenIndividualAplicacionIdAplicacionModalEliminar').val(),
            volumenId: $('#VolumenIndividualAplicacionIdVolumenModalEliminar').val()
        }
        GnossPeticionAjax(
            `${urlIC}/aplicaciones/eliminar-volumen-aplicacion-individual`, 
            //that.idAppEspecifica,
            dataVolPost,
            true
        ).done(function () {
            let contador_ind = $('#contadorVolumenIndividualEliminar').val();
            const filaEliminar = document.getElementById("trFila_" + contador_ind);
            contenedor_volumenes.removeChild(filaEliminar);
            contador_vol_modal--;
            mostrarNotificacion("success", "Volumen asociado a la aplicación eliminado con éxito.");
            loadingOcultar();
            $('#modal-delete-page-volumen-individual-aplicacion').modal('hide');
        }).fail(function (data) {

            const error = data.split('|||');
            mostrarNotificacion("error", error);

        }).always(function () {
            loadingOcultar();
        });

    }, 
       /**
 * Método para eliminar variables de una aplicacion de manera individual
 */
    handleDeleteVarApp: function () {
        var urlIC = $('#inpt_apiIntegracionContinua').val()
        var dataVolPost = {
            appId: $('#IdAplicacionVariableModalEliminar').val(),
            VariableId: $('#IdVariableModalEliminar').val()
        }
        GnossPeticionAjax(
            `${urlIC}/aplicaciones/eliminar-variable-aplicacion-individual`,
            //that.idAppEspecifica,
            dataVolPost,
            true
        ).done(function () {
            let contador_ind = $('#contadorVaraibleEliminar').val();
            const filaEliminar = document.getElementById("trVariable_" + contador_ind);
            contenedor_variables.removeChild(filaEliminar);
            contador_var_modal--;
            mostrarNotificacion("success", "Variable asociado a la aplicación eliminado con éxito.");
            loadingOcultar();
            $('#modal-delete-page-variable-aplicacion').modal('hide');
        }).fail(function (data) {

            const error = data.split('|||');
            mostrarNotificacion("error", error);

        }).always(function () {
            loadingOcultar();
        });

    },
    /**
     * Método para eliminar aplicaciones especificas. 
     */
    handleDeleteApp: function () {
        var urlIC = $('#inpt_apiIntegracionContinua').val()
        loadingMostrar();
        var dataPost = {
            aplicacionId: $('#AplicacionEspecificaIdModalEliminar').val()
        }
        GnossPeticionAjax(
            that.urlPagina + '/removeapp',
            dataPost,
            true
        ).done(function (data) {
            location.reload(); //recargamos la pagina
            mostrarNotificacion("success", "Aplicación específica eliminada con éxito.");
            loadingOcultar();


        }).fail(function (data) {
            loadingOcultar();
            var error = data.split('|||');
            if (error[0].startsWith("errorAppEliminada")) {
                mostrarNotificacion("errorAlEliminarAplicacionEspecifica", error[1])
            }
            else {
                mostrarNotificacion("error", error[1]);
            }
        }).always(function () {
            OcultarUpdateProgress();
        });

    },

    /**Método para mostrar el modal de desplegar una aplicacion
     * 
     */
    handleShowModalDeployApp: function (id) {
        var liDesplegar = $("#" + id);
        var idAplicacionEspecifica = id.replace("li_Desplegar_", "");
        $('#AplicacionEspecificaIdModalDesplegar').val(idAplicacionEspecifica);
        $('#modal-deploy-page-aplicacion-especifica').modal('show');
    },
    /**
    * Método para ocultar el modal de desplegar una aplicacion especifica. 
    */
    handleHideModalDeployApp: function () {
        $('#modal-deploy-page-aplicacion-especifica').modal('hide');
    },
    /**
     * Metodo para compilar una aplicacion
     */
    handleDeployApp: function () {
        var urlCompilar = that.urlPagina + '/Compilar-Aplicacion-Especifica-Con-Datos';
        loadingMostrar();
        var dataPost = {
            AplicacionId: $('#AplicacionEspecificaIdModalDesplegar').val()
        }
        GnossPeticionAjax(
            `${urlCompilar}`,
            dataPost,
            true
        ).done(function (data) {
            loadingOcultar();
        }).fail(function (data) {
            loadingOcultar();
            mostrarNotificacion("error", "Error al compilar la aplicacion");
        }).always(function (data) {
            if (data == 'OK') {
                mostrarNotificacion("success", "Aplicación específica compilada con éxito.");
                loadingOcultar();
                //metodo ajax para hacer dockercompose para cada proyecto se genera 1
                GnossPeticionAjax(
                    that.urlPagina + '/crear-docker-compose',
                    true
                ).done(function (dataDocker) { 
                    location.reload();
                    loadingOcultar();
                }).fail(function (dataDocker) {
                    loadingOcultar();
                    mostrarNotificacion("error", "Error al hacer la peticion para generar el Docker Compose");
                }).always(function (dataDocker) {
                    if (dataDocker == 'OK') {
                        mostrarNotificacion("success", "Docker compose del proyecto generado correctamente");
                        loadingOcultar();
                    } else {
                        mostrarNotificacion("error", "Error al generar el docker compose");
                    }
                    OcultarUpdateProgress();
                });
            } else {
                mostrarNotificacion("error", "Error al compilar la aplicacion");
            }
            OcultarUpdateProgress();
        });


    },
    /**Método para mostrar el modal de desplegar una aplicacion
     * 
     */
       handleShowModalDeployPostApp: function (id) {
        var liDesplegar = $("#" + id);
           var idAplicacionEspecifica = id.replace("li_DesplegarAppPostCompilado_", "");
           $('#AplicacionEspecificaIdModalDesplegarPostCompilado').val(idAplicacionEspecifica);
        $('#modal-deploy-post-compilation-page-aplicacion-especifica').modal('show');
    },
    /**
    /**
    * Método para ocultar el modal de desplegar una aplicacion especifica. 
    */
    handleHideModalDeployPostApp: function () {
        $('#modal-deploy-post-compilation-page-aplicacion-especifica').modal('hide');
    },
    /**
     * Metodo para desplegar una aplicacion
     */
    handleDeployPostApp: function (entorno) {
        var urlIC = $('#inpt_apiIntegracionContinua').val()
        loadingMostrar();
        var dataPost = {
            AplicacionId: $('#AplicacionEspecificaIdModalDesplegarPostCompilado').val(),
            pDesplegarEntornoSuperior: entorno
        }
        GnossPeticionAjax(
            that.urlPagina + '/deploy-app-post-compilation',
            dataPost,
            true
        ).done(function (data) {
            loadingOcultar();
            
        }).fail(function (data) {
            loadingOcultar();
            mostrarNotificacion("Error al desplegar la aplicación compilada", data);
        }).always(function (data) {
            if (data == 'OK') {
                mostrarNotificacion("success", "Aplicación específica desplegada con éxito.");
                $('#modal-deploy-post-compilation-page-aplicacion-especifica').modal('hide');
                loadingOcultar();
            } else {
                mostrarNotificacion("error", "Error al desplegar la aplicacion");
            }
            OcultarUpdateProgress();
        });


    }

}

 /**
  * Operativa para la gestión de SEO y Analytics de la Comunidad y Plataforma
  */
const operativaGestionSeoAnalytics = {

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
        // Comproba la visualización inicial de los paneles según el tipo de Script
        that.handleScriptTypeChanged();
    },    

    /**
     * Configuración de las rutas de acciones necesarias para hacer peticiones a backend
     */
     configRutas: function (pParams) {
        this.urlBase = refineURL();  
        this.urlSave = `${this.urlBase}/save`;
    },
    
    /*
     * Inicializar elementos de la vista
     * */
    config: function (pParams) {

        // Contenedor modal
        this.modalContainer = $("#modal-container");
        /* RadioButtons de la página */
        // RadioButton de ¿Desea añadir un script propio o utilizar el de la plataforma?
        this.rbScriptName = 'script';
        this.rbScript = $(`input[name="${this.rbScriptName}"]`);
        // RadioButton de tipo de indexación deseada
        this.rbIndexacionName = 'indexacion';
        this.rbIndexacionScript = $(`input[name="${this.rbIndexacionName}"]`);
        /* Inputs y valores de configuración de la plataforma */
        this.inputCodigoGoogleAnalyticsPropio = $("#CodigoGoogleAnalyticsPropio");
        this.txtAreaScriptGoogleAnalyticsPropio = $("#ScriptGoogleAnalyticsPropio");
        this.inputCodigoGoogleAnalyticsDefecto = $("#CodigoGoogleAnalyticsDefecto");        
        this.txtAreaScriptGoogleAnalyticsDefecto = $("#ScriptGoogleAnalyticsDefecto");
        
        this.CodigoGoogleAnalytics = $("#CodigoGoogleAnalytics");
        this.txtAreaScriptGoogleAnalytics = $("#ScriptGoogleAnalytics")
        /* Paneles según el tipo de script */
        this.panelScriptPropio = $("#panelScriptPropio");
        this.panelScriptDefecto = $("#panelScriptDefecto");
        this.panelGooglePlataforma = $("#panelGooglePlataforma");
        
        // Botón para guardar los cambios
        this.btnSave = $(".btnSave");

        // Objeto donde se guardará la configuración para su envío a "backend"
        this.options = {};

        // Flag indicador de si es necesario que haya un código de Google Analytics (por defecto true)
        this.isNecessaryGoogleAnalayticsCode = true;
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
        
        // Cambios de radioButtons de Indexación 
        this.rbScript.on("change", function(){           
            that.handleScriptTypeChanged();
        });    
        
        // Botón para guardar los datos
        this.btnSave.on("click", function(){
            that.handleSaveAll();
        });
        
        // Input de script de google analytics propio
        that.txtAreaScriptGoogleAnalyticsPropio.on("keyup", function(){
            const input = $(this);
            comprobarInputNoVacio(input, true, false, "Debes introducir un script para la configuración de Google Analytics.", 0);
        });
        
        // Input de código de google analytics de la plataforma
        that.CodigoGoogleAnalytics.on("keyup", function(){
            const input = $(this);             
            // Comprobar necesidad de ingresar código de analytics 
            that.handleCheckIfGoogleAnalyticsNecessary();            
        });
        
        // Input de script de google analytics de la plataforma
        that.txtAreaScriptGoogleAnalytics.on("keyup", function(){
            const input = $(this);
            comprobarInputNoVacio(input, true, false, "Debes introducir un script para la configuración de Google Analytics.", 0);
        });       
    }, 

    /**
     * Método para comprobar si es necesario mostrar un mensaje de "error" cuando no haya un código de google analytics introducido.
     */
    handleCheckIfGoogleAnalyticsNecessary: function(){
        const that = this;

        // Confirmar si es necesario un código de analytics (Siempre que haya @@codigoga@@ en #CodigoGoogleAnalytics)
        const codigoGoogleAnalyticsValue = that.txtAreaScriptGoogleAnalytics.val();
                        
        that.isNecessaryGoogleAnalayticsCode = codigoGoogleAnalyticsValue.includes("@@codigoga@@");

        if (that.isNecessaryGoogleAnalayticsCode == true){            
            comprobarInputNoVacio(that.CodigoGoogleAnalytics, true, false, "Debes introducir un código para la configuración de Google Analytics.", 0);
        }
    },


    


    /**
     * Método para mostrar u ocultar diferentes paneles dependiendo del tipo de indexación seleccionada.
     */
    handleScriptTypeChanged: function(){        
        const that = this;
        const scriptValue = $(`input[name="${that.rbScriptName}"]:checked`).val();

        // Mostrar / Ocultar los paneles según la selección del tipo de Script 
        this.panelScriptPropio.addClass("d-none");
        this.panelScriptDefecto.addClass("d-none");
        this.panelGooglePlataforma.addClass("d-none");

        switch (scriptValue) {
            case "Si":
                this.panelScriptPropio.removeClass("d-none");
                that.isNecessaryGoogleAnalayticsCode = false;
                break;                                
            case "No":
                this.panelGooglePlataforma.removeClass("d-none");
                that.isNecessaryGoogleAnalayticsCode = true;
                break;                 
            case "Defecto":
                this.panelScriptDefecto.removeClass("d-none");
                that.isNecessaryGoogleAnalayticsCode = false;
                break;        
            default:                 
                break;
        }
    },

    /**
     * Método para proceder a guardar los datos. 
     * Se comprobará que no hay errores y se procederá a recoger los datos introducidos para su posterior guardado.
     */
    handleSaveAll: function(){        
        const that = this;
        
        // Comprobar que no haya errores antes de realizar el guardado
        if (!that.checkErrors()) {
            console.log("sin errores")
            that.options = {};
            that.getData();
            that.save();
        }
        else {
            mostrarNotificacion("error", "Hay información pendiente por rellenar para configurar correctamente la configuración SEO");            
        }
    },

    /**
     * Método para comprobar que no se han producido errores durante la recogida de datos para la construcción del objeto a enviar a backend.
     * @returns bool: Devuelve true o false indicando si se ha encontrado algún error antes de poder enviar los datos para el guardado.
     */
    checkErrors: function(){        
        const that = this;
        let error = false;

        // Tipo de script de la plataforma
        const codigoSeleccionado = $(`input:radio[name="${that.rbScriptName}"]:checked`).val();
        let script = "";
        let codigo = "";
        if (codigoSeleccionado == 'Si') {
            // Comprobar que no estén vacíos
            /*if (that.inputCodigoGoogleAnalyticsPropio.val() == "" || that.txtAreaScriptGoogleAnalyticsPropio.val() == "") {*/
            if (that.txtAreaScriptGoogleAnalyticsPropio.val() == "") {
                that.inputCodigoGoogleAnalyticsPropio.trigger("keyup");                
                that.txtAreaScriptGoogleAnalyticsPropio.trigger("keyup");                                
                error = true;
            }
        } else if (codigoSeleccionado == 'No') {
            if (that.CodigoGoogleAnalytics.val() == "" || that.txtAreaScriptGoogleAnalytics.val() == "") {
                that.CodigoGoogleAnalytics.trigger("keyup");                
                that.txtAreaScriptGoogleAnalytics.trigger("keyup");                                  
                error = true;
            }
        } else {
            if (that.inputCodigoGoogleAnalyticsDefecto.val() == "" || that.txtAreaScriptGoogleAnalyticsDefecto.val() == "") {
                that.inputCodigoGoogleAnalyticsDefecto.trigger("keyup");                
                that.txtAreaScriptGoogleAnalyticsDefecto.trigger("keyup");                 
                error = true;
            }
        }
        return error;
    }, 

    /**
     * Método para obtener los datos que se han introducido y construir el objeto con la información a mandar a backEnd.
     */
    getData: function(){
        const that = this;
        let valorParametro = '';
        // Indexación de Robots        
        const valorSeleccionado = $(`input:radio[name="${that.rbIndexacionName}"]:checked`).val();        
        // Construcción del objeto "RobotsBusqueda"
        that.options['RobotsBusqueda'] = valorSeleccionado;

        if (valorSeleccionado == 'all') {
            valorParametro = '1';
        } else if (valorSeleccionado == 'noindex,nofollow') {
            valorParametro = '0';
        }
        // Construcción del objeto "ValorRobotsBusqueda"
        that.options['ValorRobotsBusqueda'] = valorParametro;

        // Tipo de Script seleccionado
        const codigoSeleccionado = $(`input:radio[name="${that.rbScriptName}"]:checked`).val();
        // Código y script seleccionado
        let script = "";
        let codigo = "";
        if (codigoSeleccionado == 'Si') {            
            that.options['ScriptGoogleAnalyticsPropio'] = encodeURIComponent(that.txtAreaScriptGoogleAnalyticsPropio.val());
        } else if (codigoSeleccionado == 'No') {
            that.options['CodigoGoogleAnalytics'] = encodeURIComponent(that.CodigoGoogleAnalytics.val());
        } else {
            codigo = this.inputCodigoGoogleAnalyticsDefecto.val();
            script = this.txtAreaScriptGoogleAnalyticsDefecto.val();             
        }
        // Construcción del objeto Google Analytics
        //that.options['CodigoGoogleAnalytics'] = encodeURIComponent(codigo);
        //that.options['ScriptGoogleAnalytics'] = encodeURIComponent(script);        
    },

    /**
     * Método para proceder al guardado de los datos recogidos en la página. Este proceso se realiza una vez
     * se ha comprobado que se han introducido los datos de forma correcta.
     */
    save: function(){
        const that = this;

        // Mostrar loading
        loadingMostrar();

        GnossPeticionAjax(
            that.urlSave,
            that.options,
            true
        ).done(function (data) {
            mostrarNotificacion("success", "Los cambios se han guardado correctamente");            
        }).fail(function (data) {
            const error = data.split('|||');
            if (data != "Contacte con el administrador del Proyecto, no es posible atender la petición.") {                
                mostrarNotificacion("error", "Se han producido errores durante el proceso de guardado. Por favor, inténtalo de nuevo más tarde o contacta con el administrador.");
            }
            else {                
                mostrarNotificacion("error", data);
            }
        }).always(function () {
            loadingOcultar();
        });        
    },
}


/**
 * Operativa para la gestión de Matomo 
 */
const operativaGestionMatomo = {

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
    triggerEvents: function () {
        const that = this;
    },

    /**
     * Configuración de las rutas de acciones necesarias para hacer peticiones a backend
     */
    configRutas: function (pParams) {
        this.urlBase = refineURL();
        this.urlRestoreCurrentUser = `${this.urlBase}/restore-current-user`;
        this.urlRestoreMatomoPassword = `${this.urlBase}/resetore-admin-matomo-password`;
    },

    /*
     * Inicializar elementos de la vista
     * */
    config: function (pParams) {

        this.modalResetMatomoPassword = $("#modal-change-matomo-password");
        this.btnRestoreCurrentUserId = "btnRestoreMatomoUser";
        this.btnCancelRestoreCurrentUserId = "btnCancelRestoreCurrentUser"; 
        this.txtNewMatomoPasswordId = "txtNewMatomoPassword";

        this.modalResetAdminMatomoPassword = $("#modal-change-matomo-admin-password");
        this.btnRestoreAdminMatomoPasswordId = "btnSetMatomoPassword";
        this.btnCancelRestoreAdminMatomoPasswordId = "btnCancelSetMatomoPassword";
        this.txtNewAminMatomoPasswordId = "txtNewMatomoAdminPassword";
    },

    /**
     * Configuración de eventos de elementos del Dom (Botones, Inputs...)     
     */
    configEvents: function (pParams) {
        const that = this;

        // Botón para resetear los datos dentro del modal de current user
        configEventById(`${that.btnRestoreCurrentUserId}`, function (element) {
            const button = $(element);
            button.off().on("click", function () {
                that.handleRestoreCurrentUser();
            });
        });

        // Botón para resetear los datos dentro del modal de admin matomo
        configEventById(`${that.btnRestoreAdminMatomoPasswordId}`, function (element) {
            const button = $(element);
            button.off().on("click", function () {
                that.handleRestoreMatomoAdminPassword();
            });
        });

        // Comportamientos del modal de restore current user
        that.modalResetMatomoPassword.on('show.bs.modal', (e) => {
            // Aparición del modal           
        }).on('hide.bs.modal', (e) => {
            // Acción de ocultar el modal
        }).on('hidden.bs.modal', (e) => {
            // Vaciar el modal
            $(`#${that.txtNewMatomoPasswordId}`).val("");
        });    

        // Comportamientos del modal de restore matomo admin password
        that.modalResetAdminMatomoPassword.on('show.bs.modal', (e) => {
            // Aparición del modal           
        }).on('hide.bs.modal', (e) => {
            // Acción de ocultar el modal
        }).on('hidden.bs.modal', (e) => {
            // Vaciar el modal
            $(`#${that.txtNewAminMatomoPasswordId}`).val("");
        });    
    },

    /**
     * 
     * */
    handleRestoreCurrentUser: function () {
        const that = this;

        const newPassword = $(`#${that.txtNewMatomoPasswordId}`).val().trim();

        if (newPassword.length == 0) {
            mostrarNotificacion("error", "La contraseña no puede estar vacía");
            return;
        }

        // Mostrar loading
        loadingMostrar();

        const params = {
            pPassword: newPassword,
        };

        GnossPeticionAjax(
            that.urlRestoreCurrentUser,
            params,
            true
        ).done(function (data) {
            that.modalResetMatomoPassword.modal("hide");
            setTimeout(function () {               
                mostrarNotificacion("success", "Se ha restablecido el usuario correctamente");
            }, 1000);  

        }).fail(function (data) {

            const error = data.split('|||');            
            mostrarNotificacion("error", error);            

        }).always(function () {
            loadingOcultar();
        });        

    },

    /**
    * 
    * */
    handleRestoreMatomoAdminPassword: function () {
        const that = this;

        const newPasswordMatomo = $(`#${that.txtNewAminMatomoPasswordId}`).val().trim();

        if (newPasswordMatomo.length == 0) {
            mostrarNotificacion("error", "La contraseña no puede estar vacía");
            return;
        }

        // Mostrar loading
        loadingMostrar();

        const params = {
            pPassword: newPasswordMatomo,
        };

        GnossPeticionAjax(
            that.urlRestoreMatomoPassword,
            params,
            true
        ).done(function (data) {
            that.modalResetAdminMatomoPassword.modal("hide");
            setTimeout(function () {
                mostrarNotificacion("success", "Se ha restablecido la contraseña del usuario \"matomo\" correctamente");
            }, 1000);

        }).fail(function (data) {

            const error = data.split('|||');
            mostrarNotificacion("error", error);

        }).always(function () {
            loadingOcultar();
        });

    },
   
}

/**
 * Operativa para la gestión de Datos Extra
 */
const operativaGestionDatosExtra = {

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
    triggerEvents: function () {
        const that = this;

        this.operativaMultiIdiomaParams = {
            // Nº máximo de pestañas con idiomas a mostrar. Los demás quedarán ocultos
            numIdiomasVisibles: 3,
            // Establecer 1 tab por cada input (true, false) - False es la forma vieja
            useOnlyOneTab: true,
            panContenidoMultiIdiomaClassName: "panContenidoMultiIdioma",
            // No permitir padding bottom y si padding top
            allowPaddingBottom: false,
            allowPaddingTop: true,
        };

        // Inicializar operativa multiIdioma
        operativaMultiIdioma.init(this.operativaMultiIdiomaParams); 
    },

    /**
     * Configuración de las rutas de acciones necesarias para hacer peticiones a backend
     */
    configRutas: function (pParams) {
        this.urlBase = refineURL();
        this.urlSaveNewExtraData = `${this.urlBase}/new-extra-data`;
        this.urlSaveEditExtraData = `${this.urlBase}/edit-extra-data`;
        this.urlDeleteExtraData = `${this.urlBase}/delete-extra-data`;
        this.urlEditModal = `${this.urlBase}/load-edit-modal`;
    },

    /*
     * Inicializar elementos de la vista
     * */
    config: function (pParams) {
        this.modalContainer = $("#modal-container");
        this.modalNewExtraData = $("#modal-new-extra-data");
        this.modalEditExtraData = $("#modal-edit-extra-data");
        this.modalDeleteExtraData = $("#modal-delete-extra-data")
        this.opcionesCheck = $("#TypeExtraData_OPCIONES");
        //this.btnDeleteExtraDataClassName = "btnConfirmDeleteExtraData";
        this.btnDeleteExtraDataClassName = "btnDeleteExtraData";
        //this.btnEditExtraDataClassName = "btnSaveEditData";
        this.btnEditExtraDataClassName = "btnEditExtraData";
        this.btnConfirmDeleteExtraDataClassName = "btnConfirmDeleteExtraData";
        this.btnDeleteExtraData = $(`.${this.btnConfirmDeleteExtraDataClassName}`);   
        this.btnEditExtraData = $(`.${this.btnEditExtraDataClassName}`);
        this.btnSaveExtraDataClassName = 'btnSaveExtraData';
        this.btnSaveExtraData = $(`.${this.btnSaveExtraDataClassName}`);
        this.btnAddOpcion = $("#btnAddOpcion");
        this.btnAddOpcionEditClassName = "btnAddOpcionEdit";
        this.btnAddOpcionEdit = $(`.${this.btnAddOpcionEditClassName}`);
        this.txtOpcion = $("#txtOpcion");
        this.txtOpcionEditClassName = "txtOpciones";
        this.txtOpcionEdit = $(`.${this.txtOpcionEditClassName}`);
        this.btnRemoveTagOptionClassName = 'tag-remove-option';
        this.btnRemoveTagOptionEditClassName = 'tag-remove-option-edit';
        this.inputOpcion = $('[name="Opciones"]');
        this.translateListItemClassName = "translate-row";
        this.btnSaveEditExtraDataClassName = "btnSaveEditData";
        this.tabIdiomaItem = $(".tabIdiomaItem ");
        this.labelLanguageComponent = $(".language-component");
    },

    /**
     * Configuración de eventos de elementos del Dom (Botones, Inputs...)     
     */
    configEvents: function (pParams) {
        const that = this;

        configEventByClassName(`${that.btnConfirmDeleteExtraDataClassName}`, function (element) {
            const $deleteButton = $(element);
            $deleteButton.off().on("click", function () {
                // Mostar loading y cerrar modal --> Se recargará la pagina al realizar el guardado desde el Controller
                var idDato = that.filaTraduccion.find("input[type=hidden]").first().val();
                that.handleDeleteExtraData(idDato);
            });
        });

        this.modalContainer.on('show.bs.modal', (e) => {
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
            });

        this.btnAddOpcion.on("click", function () {
            // Añadir propiedad si se ha escrito algo
            if (that.txtOpcion.length > 0) {
                that.handleSelectOption($(this));
            }
        }); 

        this.btnAddOpcionEdit.on("click", function () {
            // Añadir propiedad si se ha escrito algo
            if (that.txtOpcion.length > 0) {
                that.handleSelectOption($(this));
            }
        }); 

        $("input[name='TypeExtraData']").change(function () {
            $(".editarOpcionesDatoExtra").toggle();
        });

        that.modalNewExtraData.on('show.bs.modal', (e) => {
            // Aparición del modal           
        }).on('hide.bs.modal', (e) => {
            // Acción de ocultar el modal
        });

        that.modalEditExtraData.on('show.bs.modal', (e) => {
            // Aparición del modal           
        }).on('hide.bs.modal', (e) => {
            // Acción de ocultar el modal
        });

        this.modalDeleteExtraData.on('show.bs.modal', (e) => {
            // Aparición del modal
        })
            .on('hide.bs.modal', (e) => {
                // Acción de ocultar el modal
                // Si el modal se cierra sin confirmar borrado, desactiva el borrado
                //if (that.confirmDeleteTranslate == false) {
                //    that.handleSetDeleteTranslate(false);
                //}
            }); 

        configEventByClassName(`${that.btnEditExtraDataClassName}`, function (element) {
            const $editButton = $(element);
            $editButton.off().on("click", function () {
                // Fila de la traducción correspondiente
                that.filaTraduccion = $(this).closest(`.${that.translateListItemClassName}`);
                // Obtener la url a editar     
                that.handleLoadCreateEditData(this.id);
            });
        }); 

        configEventByClassName(`${that.btnDeleteExtraDataClassName}`, function (element) {
            const $deleteButton = $(element);
            $deleteButton.off().on("click", function () {
                // Fila de la traducción correspondiente
                that.filaTraduccion = $(this).closest(`.${that.translateListItemClassName}`);
            });
        });

        configEventByClassName(`${that.btnSaveExtraDataClassName}`, function (element) {
            const $saveButton = $(element);
            $saveButton.off().on("click", function () {
                // Mostar loading y cerrar modal --> Se recargará la pagina al realizar el guardado desde el Controller
                that.handleSaveExtraData();
            });
        });

        configEventByClassName(`${that.btnSaveEditExtraDataClassName}`, function (element) {
            const $saveButton = $(element);
            $saveButton.off().on("click", function () {
                // Mostar loading y cerrar modal --> Se recargará la pagina al realizar el guardado desde el Controller
                that.handleSaveEditExtraData();
            });
        });

        configEventByClassName(`${that.btnExtraDataClassName}`, function (element) {
            const $confirmDeleteButton = $(element);
            $confirmDeleteButton.off().on("click", function () {
                that.DeleteExtraData = true;
                that.handleDeleteExtraData();
            });
        });  

        // Botón (X) para poder eliminar items desde el Tag                           
        configEventByClassName(`${that.btnRemoveTagOptionClassName}`, function (element) {
            const $jqueryElement = $(element);
            $jqueryElement.off().on("click", function () {
                const $itemRemoved = $jqueryElement.parent().parent();
                that.handleClickBorrarTagOption($itemRemoved);
            });
        });

        // Botón (X) para poder eliminar items desde el Tag editando                          
        configEventByClassName(`${that.btnRemoveTagOptionEditClassName}`, function (element) {
            const $jqueryElement = $(element);
            $jqueryElement.off().on("click", function () {
                const $itemRemoved = $jqueryElement.parent().parent();
                that.handleClickBorrarTagOptionEdit($itemRemoved);
            });
        });
        
        configEventByClassName(`${that.btnAddOpcionEditClassName}`, function (element) {
            const $jqueryElement = $(element);
            $jqueryElement.off().on("click", function () {
                if (that.txtOpcion.length > 0) {
                    that.handleSelectOptionEdit($(this));
                }
            });
        });

        this.tabIdiomaItem.off().on("click", function () {
            that.handleViewExtraDataLanguageInfo();
        });
    },
    handleClickBorrarTagOption: function (itemDeleted) {
        const that = this;

        // Panel o sección donde se encuentra el panel de Tags a Eliminar (input_hack)
        const panelTagItem = itemDeleted.parent().parent();

        // Buscar el input oculto y seleccionar la propiedad corresponde con item a eliminar
        const idItemDeleted = itemDeleted.prop("title");
        // Items id dependiendo del tipo a borrar (Perfil, Grupo)
        let itemsId = "";
        // Input del que habrá que eliminar el item seleccionado
        let inputOpcion = that.inputOpcion;       

        let $inputHack = panelTagItem.find(inputOpcion);

        itemsId = $inputHack.val().split(",");
        itemsId.splice($.inArray(idItemDeleted, itemsId), 1);
        // Pasarle los datos al input hidden
        $inputHack.val(itemsId.join(","));
        // Eliminar el item del contenedor visual
        itemDeleted.remove();
    },  
    handleClickBorrarTagOptionEdit: function (itemDeleted) {
        const that = this;

        // Panel o sección donde se encuentra el panel de Tags a Eliminar (input_hack)
        const panelTagItem = itemDeleted.parent().parent();

        // Buscar el input oculto y seleccionar la propiedad corresponde con item a eliminar
        const idItemDeleted = itemDeleted.prop("title");
        // Items id dependiendo del tipo a borrar (Perfil, Grupo)
        let itemsId = "";
        let id = that.filaTraduccion.find("input[type=hidden]").first().val();
        // Input del que habrá que eliminar el item seleccionado

        let $inputHack = panelTagItem.find("#txtOpcion_Hack_"+id);

        itemsId = $inputHack.val().split(",");
        itemsId.splice($.inArray(idItemDeleted, itemsId), 1);
        // Pasarle los datos al input hidden
        $inputHack.val(itemsId.join(","));
        // Eliminar el item del contenedor visual
        itemDeleted.remove();
    },
    handleSelectOption: function () {
        const that = this;

        // Añadir propiedad para autocompletar
        let propertyId = guidGenerator();
        const propertyValue = that.txtOpcion.val().trim();

        if (propertyValue.trim().length == 0) {
            return;
        }

        // Panel/Sección donde se ha realizado toda la operativa
        let tagsSection = that.txtOpcion.parent().parent();

        // Contenedor de items donde se añadirá el nuevo seleccionado para su visualización
        let tagContainer = tagsSection.find(".tag-list");
        // Input oculto donde se añadirá el nuevo item seleccionado
        let inputHack = tagsSection.find("input[type=hidden]").first();
        // Añadido el id del item seleccionado al inputHack                
        inputHack.val(inputHack.val() + propertyValue + ',');

        // Etiqueta del item seleccionado        
        let autocompletePropertyTag = '';
        autocompletePropertyTag += '<div class="tag" id="' + propertyId + '" title="' + propertyValue + '">';
        autocompletePropertyTag += '<div class="tag-wrap">';
        autocompletePropertyTag += '<span class="tag-text">' + propertyValue + '</span>';
        autocompletePropertyTag += "<span class=\"tag-remove tag-remove-option material-icons\">close</span>";
        autocompletePropertyTag += '</div>';
        autocompletePropertyTag += '</div>';

        // Añadir el item en el contenedor de items para su visualización
        tagContainer.append(autocompletePropertyTag); 
    },
    handleViewExtraDataLanguageInfo: function () {

        const that = this;
        // Comprobar el item que está activo en el tab obteniendo el data-language de la opción "active"
        setTimeout(function () {
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

        }, 250);
    },
    handleSelectOptionEdit: function () {
        const that = this;

        // Añadir propiedad para autocompletar
        let propertyId = guidGenerator();
        var id = $("#modal-dinamic-content").find("input[type=hidden]").first().val();
        var txtOpcionEdit = "txtOpcion_"+id;
        const propertyValue = $("#"+txtOpcionEdit).val().trim();

        if (propertyValue.trim().length == 0) {
            return;
        }

        // Contenedor de items donde se añadirá el nuevo seleccionado para su visualización
        let tagContainer = $("#modal-dinamic-content").find("#tagsContainer_opciones_" + id);
        // Input oculto donde se añadirá el nuevo item seleccionado
        let inputHack = $("#modal-dinamic-content").find("#txtOpcion_Hack_" + id);
        // Añadido el id del item seleccionado al inputHack                
        inputHack.val(inputHack.val() + propertyValue + ',');

        // Etiqueta del item seleccionado        
        let autocompletePropertyTag = '';
        autocompletePropertyTag += '<div class="tag" id="' + propertyId + '" title="' + propertyValue + '">';
        autocompletePropertyTag += '<div class="tag-wrap">';
        autocompletePropertyTag += '<span class="tag-text">' + propertyValue + '</span>';
        autocompletePropertyTag += '<span class="tag-remove tag-remove-option-edit material-icons">close</span>';
        autocompletePropertyTag += '</div>';
        autocompletePropertyTag += '</div>';

        // Añadir el item en el contenedor de items para su visualización
        tagContainer.append(autocompletePropertyTag);
    },
    handleSaveExtraData: function () {
        const that = this;
        // Bandera para controlar posibles errores antes de realizar guardado
        let error = false;

        let tagsSection = that.txtOpcion.parent().parent();

        // Contenedor de items donde se añadirá el nuevo seleccionado para su visualización
        let tagContainer = tagsSection.find(".tag-list");
        // Input oculto donde se añadirá el nuevo item seleccionado
        let inputHack = tagsSection.find("input[type=hidden]").first();
        //var nombre = $("#inptNombreNuevoDato").val();
        var nombreCorto = $("#inptNombreCortoNuevoDato").val();
        var predicado = $("#inptURINuevoDato").val();
        var orden = $("#inptOrdenNuevoDato").val();
        var tipo = $('#TypeExtraData_OPCIONES')[0].checked ? "Opcion" : "TextoLibre";
        var obligatorio = $('#chkObligatorioNuevoDato')[0].checked;
        var visible = $('#chkVisibleNuevoDato')[0].checked;

        var opciones = "";
        if (tipo == "Opcion") {
            opciones = inputHack.val();
        }

        let textoIdiomaDefecto = $(`#input_inptNombreNuevoDato_${operativaMultiIdioma.idiomaPorDefecto}`).val();
        var nombre = "";
        $.each(operativaMultiIdioma.listaIdiomas, function () {
            // Obtención del Key del idioma
            const idioma = this.key;
            // Asignar el valor por defecto de la ruta al idioma si este no dispone de valor para Nombre
            let textoIdioma = $(`#input_inptNombreNuevoDato_${idioma}`).val();
            if (textoIdioma == null || textoIdioma == "") {
                textoIdioma = textoIdiomaDefecto;
                $(`#input_inptNombreNuevoDato_${idioma}`).val(textoIdioma);
            }
            // Escribir el nombre del multiIdioma en el campo Hidden
            nombre += textoIdioma + "@" + idioma + "|||";

            //listaTextos.push({ "key": idioma, "value": textoIdioma });
        });

        const params = {
            pNombre: nombre,
            pNombreCorto: nombreCorto,
            pTipo: tipo,
            pOpciones: opciones,
            pObligatorio: obligatorio,
            pOrden: orden,
            pVisible: visible,
            pPredicadoRDF: predicado
        };

        // Mostrar loading
        loadingMostrar();

        GnossPeticionAjax(
            that.urlSaveNewExtraData,
            params,
            true
        ).done(function (data) {
            // OK - Mostrar el mensaje de guardado correcto
            mostrarNotificacion("success", "El dato extra ha sido creado correctamente");

            setTimeout(function () {
                location.reload();
            }, 1000);

        }).fail(function (data) {
            mostrarNotificacion("error", data);
        }).always(function () {
            // Ocultar el loading
            loadingOcultar();
        });
    },
    handleSaveEditExtraData: function () {
        const that = this;

        var modal = $("#modal-dinamic-content");
        var id = modal.find("input[type=hidden]").first().val();
        // Contenedor de items donde se añadirá el nuevo seleccionado para su visualización
        let tagContainer = modal.find("#tagsContainer_opciones_" + id);
        // Input oculto donde se añadirá el nuevo item seleccionado
        let inputHack = modal.find("#txtOpcion_Hack_" + id);
        //var nombre = modal.find("#inptNombre_" + id).val();
        var nombreCorto = modal.find("#inptNombreCorto_" + id).val();
        var predicado = modal.find("#inptURI_"+id).val();
        var tipo = modal.find("#TypeExtraDataEdit_"+id)[0].checked ? "Opcion" : "TextoLibre";
        var obligatorio = modal.find("#chkObligatorio_" + id)[0].checked;
        var visible = modal.find("#chkVisible_" + id)[0].checked;
        var orden = modal.find("#inptOrden_"+id).val();
        var opciones = "";

        if (tipo == "Opcion") {
            opciones = inputHack.val();
        }
        // nuevo
        let textoIdiomaDefecto = $(`#input_inptNombre_${id}_${operativaMultiIdioma.idiomaPorDefecto}`).val();
        var nombre = "";

        $.each(operativaMultiIdioma.listaIdiomas, function () {
            // Obtención del Key del idioma
            const idioma = this.key;
            // Asignar el valor por defecto de la ruta al idioma si este no dispone de valor para Nombre
            let textoIdioma = $(`#input_inptNombre_${id}_${idioma}`).val();
            if (textoIdioma == null || textoIdioma == "") {
                textoIdioma = textoIdiomaDefecto;
                $(`#input_inptNombre_${id}_${idioma}`).val(textoIdioma);
            }
            // Escribir el nombre del multiIdioma en el campo Hidden
            nombre += textoIdioma + "@" + idioma + "|||";

            //listaTextos.push({ "key": idioma, "value": textoIdioma });
        });

        const params = {
            pDatoExtraID: id,
            pNombre: nombre,
            pNombreCorto: nombreCorto,
            pTipo: tipo,
            pOpciones: opciones,
            pObligatorio: obligatorio,
            pOrden: orden,
            pPredicadoRDF: predicado,
            pVisible: visible
        };

        // Mostrar loading
        loadingMostrar();

        GnossPeticionAjax(
            that.urlSaveEditExtraData,
            params,
            true
        ).done(function (data) {
            // OK - Mostrar el mensaje de guardado correcto
            mostrarNotificacion("success", "El dato extra ha sido creado correctamente");

            setTimeout(function () {
                location.reload();
            }, 1000);

        }).fail(function (data) {
            mostrarNotificacion("error", data);
        }).always(function () {
            // Ocultar el loading
            loadingOcultar();
        });
    },
    handleDeleteExtraData: function (id) {
        const that = this;

        loadingMostrar();

        const params = {
            pDatoExtraID: id
        };

        GnossPeticionAjax(
            that.urlDeleteExtraData,
            params,
            true
        ).done(function (data) {
            // OK - Mostrar el mensaje de guardado correcto
            mostrarNotificacion("success", "El dato extra ha sido eliminado correctamente");

            setTimeout(function () {
                location.reload();
            }, 1000);

        }).fail(function (data) {
            mostrarNotificacion("error", data);
        }).always(function () {
            // Ocultar el loading
            loadingOcultar();
        });
    },
    handleEditExtraData: function () {
        const that = this;

        loadingMostrar();

        GnossPeticionAjax(
            that.urlEditExtraData,
            null,
            true
        ).done(function (data) {
            // OK - Mostrar el mensaje de guardado correcto
            mostrarNotificacion("success", "El dato extra ha sido eliminado correctamente");

            setTimeout(function () {
                location.reload();
            }, 1000);

        }).fail(function (data) {
            mostrarNotificacion("error", data);
        }).always(function () {
            // Ocultar el loading
            loadingOcultar();
        });
    },
    handleLoadCreateEditData: function (id) {
        const that = this;

        // Cargar la vista para editar o crear una nueva traducción
        getVistaFromUrl(that.urlEditModal+"?pDatoID="+id, 'modal-dinamic-content', '', function (result) {
            if (result != requestFinishResult.ok) {
                // KO al cargar la vista
                mostrarNotificacion("error", "Error al tratar de crear/editar un dato extra.");
                dismissVistaModal();
            }
        });
    },
}


/**
  * Operativa para la gestión de Traducciones de la plataforma
  */
const operativaGestionTraducciones = {
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
        
        // Contabilizar el nº de traducciones existentes
        that.handleCheckNumberOfTranslations();
    },    

    /**
     * Configuración de las rutas de acciones necesarias para hacer peticiones a backend
     */
     configRutas: function (pParams) {
        // Url base para gestión de traducciones
        this.urlBase = refineURL();
        this.urlDeleteTranslate = `${this.urlBase}/borrartraduccion`;
    },    

    /*
     * Inicializar elementos de la vista
     * */
    config: function (pParams) {

        // Contenedor modal
        this.modalContainer = $("#modal-container");
        this.modalDeleteTranslate = $("#modal-delete-translate");

        // Tab de idioma de la página para cambiar la visualización en el idioma deseado de las páginas
        this.tabIdiomaItem = $(".tabIdiomaItem ");        
        // Cada una de las labels (Nombre literal) en los diferentes idiomas (por `data-languageitem`)
        this.labelLanguageComponent = $(".language-component");
        // Cada una de las filas donde se encuentran las traducciones
        this.translateListItemClassName = 'translate-row';
        // Botón para poder copiar al portapapeles una traducción
        this.btnTranslateCopyClassName = "btnTranslateCopy";

        // Contador del nº de traducciones existentes
        this.numResultados = $('.numResultados');

        // Input para realizar búsquedas
        this.txtBuscarTraduccion = $("#txtBuscarTraduccion");
        // Input de la lupa para realizar búsquedas
        this.inputLupa = $("#inputLupa");
        // Botón para editar una traducción existente
        this.btnEditTranslateClassName = 'btnEditTranslate';
        this.btnEditTranslate = $(`.${this.btnEditTranslateClassName}`);
        // Botón para eliminar una traducción
        this.btnDeleteTranslateClassName = 'btnDeleteTranslate';
        this.btnDeleteTranslate = $(`.${this.btnDeleteTranslateClassName}`);
        // Botón para guardar desde el modal la traducción
        this.btnSaveTranslateClassName = 'btnSaveTranslate';
        this.btnSaveTranslate = $(`.${this.btnSaveTranslateClassName}`);

        // Botón para crear una nueva traducción
        this.btnAddTranslation = $('#btnAddTranslation');
        // Input text del identificador de la traducción
        this.txtIdTranslationIdName = 'TextoID'
        this.txtIdTranslation = $(`#${this.txtIdTranslationIdName}`);

        // Formulario para edición y creación de una traducción (Submit para edición/creación)
        this.formCreateEditTranslateIdName = 'formCreateEditTranslate';
        this.formCreateEditTranslate = $(`#${this.formCreateEditTranslateIdName}`);

        // Botón para confirmar la eliminación de la traducción
        this.btnConfirmDeleteTranslateClassName = "btnConfirmDeleteTranslate";
        this.btnConfirmDeleteTranslate = $(`.${this.btnConfirmDeleteTranslateClassName}`);

        // Url que se usará para editar una determinada traducción
        this.urlEditCreateTranslate = '';
        // Fila que está siendo editada
        this.filaTraduccion = '';
        // Id de la traducción que se desea eliminar
        this.translateId = '';
        // Flag para detectar si hay que borrar o no la traducción
        this.confirmDeleteTranslate = false;
    },   
    
    /**
     * Configuración de eventos de elementos del Dom (Botones, Inputs...)     
     */
     configEvents: function (pParams) {
        const that = this;
                       
        this.modalContainer.on('show.bs.modal', (e) => {    
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
        });

        // Comportamientos del modal que son individuales para el borrado de páginas
        this.modalDeleteTranslate.on('show.bs.modal', (e) => {
            // Aparición del modal
        })
        .on('hide.bs.modal', (e) => {
            // Acción de ocultar el modal
            // Si el modal se cierra sin confirmar borrado, desactiva el borrado
            if (that.confirmDeleteTranslate == false){
                that.handleSetDeleteTranslate(false);                
            }            
        });         
        

        // Botón para copiar al portapapeles una traducción
        configEventByClassName(`${that.btnTranslateCopyClassName}`, function(element){
            const $copyButton = $(element);
            $copyButton.off().on("click", function(){ 
                const translateId = $(this).data("translateid");                     
                that.handleCopyToClickBoard(translateId);                
            });	                        
        });          

        // Botón para editar una traducción        
        configEventByClassName(`${that.btnEditTranslateClassName}`, function(element){
            const $editButton = $(element);
            $editButton.off().on("click", function(){   
                // Fila de la traducción correspondiente
                that.filaTraduccion = $(this).closest(`.${that.translateListItemClassName}`);
                // Obtener la url a editar     
                that.urlEditCreateTranslate = $(this).data("urledit");                
                that.handleLoadCreateEditTranslate();                
            });	                        
        });  

        // Botón para borrar una traducción        
        configEventByClassName(`${that.btnDeleteTranslateClassName}`, function(element){
            const $deleteButton = $(element);
            $deleteButton.off().on("click", function(){   
                // Fila correspondiente a la traducción eliminada
                that.filaTraduccion = $(this).closest(`.${that.translateListItemClassName}`);  
                // Obtener el id de la categoría a eliminar
                that.translateId = $(this).data("translate-id");                           
                // Marcar la traducción como "Eliminar" a modo informativo al usuario
                that.handleSetDeleteTranslate(true);                          
            });	                        
        });  
        
        // Botón para confirmar la eliminación de una traducción vía modal        
        configEventByClassName(`${that.btnConfirmDeleteTranslateClassName}`, function(element){   
            const $confirmDeleteButton = $(element);
            $confirmDeleteButton.off().on("click", function(){   
                that.confirmDeleteTranslate = true;
                that.handleConfirmDeleteTranslate();                            
            });	                                                       
        }); 

        
        // Botón para guardar una traducción        
        configEventByClassName(`${that.btnSaveTranslateClassName}`, function(element){
            const $saveButton = $(element);
            $saveButton.off().on("click", function(){                                                      
                // Mostar loading y cerrar modal --> Se recargará la pagina al realizar el guardado desde el Controller
                that.handleSaveTranslate();                         
            });	                        
        });

        // Input del id correspondiente a una nueva traducción desde el Modal
        configEventById(`${that.txtIdTranslationIdName}`, function(element){
            const $input = $(element);
            $input.off().on("keyup", function(){                                                      
                comprobarInputNoVacio($input, true, false, "El id de la traducción no puede estar vacío.", 0);
            });	    
        });
        

        // Botón para crear una nueva traducción
        this.btnAddTranslation.on("click", function(){
            // Obtener la url a crear
            that.urlEditCreateTranslate = $(this).data("urlcreatetranslate"); 
            that.handleLoadCreateEditTranslate();
        });

        // Tab de cada uno de los idiomas disponibles para cambiar la visualización en la lista de traducciones
        this.tabIdiomaItem.off().on("click", function(){
            that.handleViewTranslateLanguageInfo();            
            // Aplicar la búsqueda para el idioma activo
            that.timer = setTimeout(function () {                                                                                                        
                that.handleSearchTranslateItem(that.txtBuscarTraduccion);
            }, 500);            
        });


        // Búsquedas de traducciones
        this.txtBuscarTraduccion.on("keyup", function (event) {
            const input = $(this);
            clearTimeout(that.timer);
            that.timer = setTimeout(function () {                                                                        
                // Acción de buscar / filtrar traducción
                that.handleSearchTranslateItem(input);                                         
            }, 500);
        });         
    },

    /**
     * Método para marcar o desmarcar la traducción como "Eliminada" dependiendo de la elección vía Modal
     * @param {Bool} Valor que indicará si se desea eliminar o no la traducción
     */
     handleSetDeleteTranslate: function(deleteTranslate){
        const that = this;

        if (deleteTranslate){            
            // Añadir la clase de "deleted" a la fila de la traducción
            that.filaTraduccion.addClass("deleted");
        }else{          
            // Eliminar la clase de "deleted" a la fila de la página
            that.filaTraduccion.removeClass("deleted");
        }
    }, 

    /**
     * Método para copiar al portapapeles el enlace del item pulsado.     
     * @param {String} translateId : Id de la traducción a copiar
     */
    handleCopyToClickBoard: function(translateId){
        copyTextToClipBoard(translateId);
    },


    /**
     * Método para buscar una traducción independientemente de su idioma
     * @param {jqueryElement} input : Input desde el que se ha realizado la búsqueda
     */
     handleSearchTranslateItem: function(input){
        const that = this;

        // Cadena introducida para filtrar/buscar componentes
        let cadena = input.val();
        // Eliminamos posibles tildes para búsqueda ampliada
        cadena = cadena.normalize('NFD').replace(/[\u0300-\u036f]/g, "").toLowerCase();

        // Listado de componentes donde se buscará
        const translateItems = $(`.${that.translateListItemClassName}`);

        // Recorrer los items/compontes para realizar la búsqueda de palabras clave
        $.each(translateItems, function(index){
            const translateItem = $(this);
            // Seleccionar el nombre sólo "visible" de la traducción. Quitamos caracteres extraños, tildes para poder hacer bien la búsqueda
            const componentLiteral = translateItem.find(".component-literal").not('.d-none').html().trim().normalize("NFD").replace(/[\u0300-\u036f]/g, "").toLowerCase();
            const componentKey = translateItem.find(".component-name").html().trim().normalize("NFD").replace(/[\u0300-\u036f]/g, "").toLowerCase();

            if (componentLiteral.includes(cadena) || componentKey.includes(cadena)){
                // Mostrar la fila
                translateItem.removeClass("d-none");                
            }else{
                // Ocultar la fila
                translateItem.addClass("d-none");
            }            
        });                        
    },

    /**
     * Método que se ejecutará al cargarse la web para saber el nº de traducciones existentes
     */
    handleCheckNumberOfTranslations: function(){
        const that = this;

        const numberTranslations = $(".translate-row").length;
        that.numResultados.html(numberTranslations);
    },

    /**
     * Método para cambiar la visualización al idioma de la traducción deseada
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
     * Método para enviar la la petición de guardado de traducción si los datos están correctos
     */
    handleSaveTranslate: function(){
        const that = this;
        // Bandera para controlar posibles errores antes de realizar guardado
        let error = false;
        
        const txtIdTranslate = $(`#${that.txtIdTranslationIdName}`);

        // Mostrar loading
        loadingMostrar();

        // Comprobar que si está el ID, no esté vacío
        if (txtIdTranslate.length > 0){
            // Comprobar que no está vacío
            if (txtIdTranslate.val().trim().length == 0){
                txtIdTranslate.trigger("keyup");
                error = true;
            }
        }

        if (error == false){
            // Ejecutar submit del formulario para el envío de la información de la traducción Editada/Nueva                        
            $(`#${that.formCreateEditTranslateIdName}`).submit();
            setTimeout(function(){
                dismissVistaModal();
            },1500);               
        }else{
            loadingOcultar();
        }      
    },

    /**
     * Método para mostrar el modal para editar/crear una nueva traducción
     */
     handleLoadCreateEditTranslate: function(){
        const that = this;

        // Cargar la vista para editar o crear una nueva traducción
        getVistaFromUrl(that.urlEditCreateTranslate, 'modal-dinamic-content', '', function(result){
            if (result != requestFinishResult.ok){
                // KO al cargar la vista
                mostrarNotificacion("error", "Error al tratar de crear/editar una traducción de la comunidad.");
                dismissVistaModal();                                                     
            }else{
                // Añadir el estilo de tabs al modal 
                !that.modalContainer.hasClass("modal-con-tabs") && that.modalContainer.addClass("modal-con-tabs");
            }                
        });        
    },

    /**
     * Método que confirmará la eliminación de una traducción
     */
    handleConfirmDeleteTranslate: function(){
        const that = this;
        // alert.log("Pendiente borrado - CORE-4307");

        if (that.translateId != ""){
            // ID de la traducción a eliminar            
            const params = {textoID: that.translateId};
            loadingMostrar();
            // Realizar la petición para el guardado de páginas
            GnossPeticionAjax(
                that.urlDeleteTranslate,
                params,
                true
            ).done(function (data) {
                // OK - Mostrar el mensaje de guardado correcto
                mostrarNotificacion("success","La traducción se ha borrado correctamente"); 
                // Vaciar la categoría a eliminar
                that.translateId = "";  
                // Resetear el estado de borrado de la traducción
                that.confirmDeleteTranslate = false;                
                // 2 - Ocultar el modal de eliminación de la traducción                                                                                                
                dismissVistaModal(that.modalDeleteTranslate);
                // 3 - Ocultar loading
                loadingOcultar();                
                // 4 - Ocultar la fila que se desea eliminar
                that.filaTraduccion.addClass("d-none");
                // 5 - Eliminar la fila que se acaba de eliminar                                
                that.filaTraduccion.remove();
                // 6 - Actualizar el contador de nº de traducciones                    
                that.handleCheckNumberOfTranslations();                         
            }).fail(function (data) {
                mostrarNotificacion("error",data);
            }).always(function () {
                // Ocultar el loading
                loadingOcultar();
            });
        }
    }
}


/* Operativa para la gestión de la Descarga e Importación de ficheros CSV para las Traducciones*/
const operativaGestionDescargaTraducciones = {

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
                           
        // Modal para selección de descarga de recursos por fecha
        this.modalDownloadResourcesId = "modalDownloadResources";
        // Modal para la importación de traducciones
        this.modalImportTranslateId = "modalImportTranslate";

        // Input para buscar items en la sección de "Descarga de traducciones"
        this.txtBuscarDescargaTraduccion = $('#txtBuscarDescargaTraduccion');
        this.translateTypeNavigationButtonClassName = 'tabTranslateTypeItem';

        // Botones para descargar cada uno de los items de las traducciones
        this.btnDownloadTranslateClassName = 'btnDownloadTranslate';
        // Cada una de las filas que contienen las traducciones
        this.translateRowClassName = "translate-row";

        // Botón para descarga la selección de las traducciones
        this.btnDownloadTranslateSelectedId = "btnDownloadTranslateSelected";
        this.btnDownloadTranslateSelected = $(`#${this.btnDownloadTranslateSelectedId}`);
        // Botón para importar traducciones
        this.btnImportTranslateId = "btnImportTranslate";
        this.btnImportTranslateId = $(`#${this.btnImportTranslateId}`);             
        // Checkbox para marcar o desmarcar todos los items
        this.checkAllTextsId = "checkAllTexts";
        this.checkAllTexts = $(`#${this.checkAllTextsId}`);

        // Contenedores de las traducciones de los items de tipo "Texto, Recursos, Xml"
        this.translateListClassName = "component-translate-list";

        // Checkbox de cada uno de los inputs (Single checkbox) para marcar o desmarcar la selección
        this.checkBoxDownloadSingleInputClassName = "input-download-item";


        // Modal para descarga de recursos por fecha
        // RadioButton para controlar si se desean descargar recursos por fecha de creación (sí/no)
        this.radioButtonDownloadBetweenDateClassName = "downloadBetweenDate";
        // Panel que permite seleccionar las fechas para la descarga de recursos (Inicio / Fin)
        this.panelBetweenDatesId = "panelBetweenDates";
        this.panelBetweenDates = $(`#${this.panelBetweenDatesId}`);
        // Inputs con fecha inicial y final
        this.inputStartDateId = "startDate";
        this.inputStartDate = $(`#${this.inputStartDateId}`);
        this.inputEndDateId = "endDate";
        this.inputEndDate = $(`#${this.inputEndDateId}`);
        // Botón para descargar traducciones desde el modal (Recursos por fecha)
        this.btnConfirmDownloadId = "btnConfirmDownload"
        this.btnConfirmDownload = $(`#${this.btnConfirmDownloadId}`);

        // Input file para seleccionar un fichero
        this.inputTranslateFileId = "inputTranslateFile";
        this.inputTranslateFile = $(`#${this.inputTranslateFileId}`);
        // Botón para subir el fichero de las traducciones
        this.btnConfirmImportTranslateFileId = "btnConfirmUpload";
        this.btnConfirmImportTranslateFile = $(`#${this.btnConfirmImportTranslateFileId}`);
        // Botón para validar el fichero de las traducciones
        this.btnConfirmValidateTranslateFileId = "btnConfirmValidate";
        this.btnConfirmValidateTranslateFile = $(`#${this.btnConfirmValidateTranslateFileId}`);        


        // Objeto que almacenará los items a enviar para su guardado
        this.dataPostDownloadItems = new FormData();
        // Guardar la opción seleccionada de la navegación para saber qué traducciones mostrar. Por defecto "textos"
        this.translateTypeItem = "text";
        // Dependiendo la opción pulsada, el nombre a generar al descargar los ficheros (En caso de ser múltiple)
        this.translateTypeFileName = "Traducciones"; 
        // Array para almacenar los items que se seleccionarán
        this.translateRowsSelected = [];

        
        // Flag para guardar la existencia de errores
        this.isError = false;
        // Mensajes pasados desde la vista
        this.messages = pParams.messages; 
    },

    /**
     * Configuración de rutas de la web
     */
    configRutas: function(){
        const that = this;
        
        this.urlBase = refineURL();  
        this.urlDownloadTranslate = `${this.urlBase}/download`;        
        this.urlUploadTranslate = `${this.urlBase}/upload`;
    },

    /**
     * Lanzar comportamientos u operativas necesarias para el funcionamiento de la sección
     */
    triggerEvents: function(){
        const that = this;  
        // Mostrar por defecto el panel de traducciones activo
        that.handleShowTranslateByTypeSelected();
    },

    /**
     * Configuración de eventos de elementos del Dom (Botones, Inputs...)     
     */
    configEvents: function (pParams) {
        const that = this;
        

        // Controlar el cierre del modal de "Descarga" para quitar los inputs de fechas
        // Comportamientos del modal que son individuales para el borrado de trazas
        $(`#${this.modalDownloadResourcesId}`).on('show.bs.modal', (e) => {
            // Aparición del modal
        })
        .on('hide.bs.modal', (e) => {
            // Acción de ocultar el modal
            // Vaciar los inputs de "Desde" "Fin" y dejar chequeado el radio input de "No"            
            that.inputStartDate.val("");
            that.inputEndDate.val("");
            $(`.${that.radioButtonDownloadBetweenDateClassName}[data-value="no"]`).prop('checked', true);
            $(`.${that.radioButtonDownloadBetweenDateClassName}`).trigger("change");
        });          

        // Input para buscar traducciones
        this.txtBuscarDescargaTraduccion.off().on("keyup", function (event) {
            clearTimeout(that.timer);
            that.timer = setTimeout(function () {                
                that.handleSearchTranslateByName();                                                         
            }, 500);
        });        

        // Botón para conocer navegación entre los distintos ficheros de traducciones
        configEventByClassName(`${that.translateTypeNavigationButtonClassName}`, function(element){
            const btnTabTranslateTypeItem = $(element);
            btnTabTranslateTypeItem.off().on("click", function(){
                that.translateTypeItem = btnTabTranslateTypeItem.data("value");
                that.handleShowTranslateByTypeSelected();
                // Desactivar los checkbox seleccionados                
                $(`#${that.checkAllTextsId}`).prop('checked', false); 
                that.handleCheckUncheckAllTranslateItems();               
            });	                        
        });        

        // Botón para solicitar la descarga de un único fichero
        configEventByClassName(`${that.btnDownloadTranslateClassName}`, function(element){
            const btnSingleTranslateDownload = $(element);
            btnSingleTranslateDownload.off().on("click", function(){
                // Array de rows a descargar. En este caso, sólo hay una fila (Descargar vía botón)
                const downloadRows = [btnSingleTranslateDownload.closest(`.${that.translateRowClassName}`)];                
                that.handleGetDownloadFileFromRows(downloadRows);  
                // Proceder a realizar la descarga
                that.handleDownloadTranslations();              
            });	                        
        });        
        
        // Checkbox de cada traducción
        configEventByClassName(`${that.checkBoxDownloadSingleInputClassName}`, function(element){
            const translateCheckbox = $(element);
            translateCheckbox.off().on("change", function(){                                
                that.handleCheckTranslateSelected();                                            
            });	                        
        });   
                
        // Checkbox de selección múltiple
        configEventById(`${that.checkAllTextsId}`, function(element){
            const $checkAllTextsId = $(element);
            $checkAllTextsId.off().on("change", function(){   
                that.handleCheckUncheckAllTranslateItems();
            });	                        
        });                  
        
        // Checkbox de selección múltiple
        configEventById(`${that.btnDownloadTranslateSelectedId}`, function(element){
            const buttonDownloadTranslateSelected = $(element);
            buttonDownloadTranslateSelected.off().on("click", function(){                                                                   
                // Selecciona todos los checkboxes seleccionados
                const translateCheckbox = $(`.${that.checkBoxDownloadSingleInputClassName}:checked`);
                // Obtener las filas correspondientes de los checbox seleccionados
                let downloadRows = [];
                // Recorrer los items/compontes para realizar la búsqueda de palabras clave
                $.each(translateCheckbox, function(index){
                    const translateItem = $(this); 
                    const translateRow = translateItem.closest(`.${that.translateRowClassName}`)                    
                    downloadRows.push(translateRow);
                });                
                that.handleGetDownloadFileFromRows(downloadRows);  
                
                // Lanzar el modal para descargar recursos si se han seleccionado los recursos
                if (that.translateTypeItem == "resource"){
                    // Modal para selección por fechas para recursos (Textos / Ontologías)                    
                    $(`#${that.modalDownloadResourcesId}`).modal('show');                    
                }else{
                    that.handleDownloadTranslations();
                }             
            });	                        
        }); 
        
        // RadioButton para la selección de descarga de recursos por fecha inicio/fin
        configEventByClassName(`${that.radioButtonDownloadBetweenDateClassName}`, function(element){
            const $radioButton = $(element);
            $radioButton.off().on("change", function(){
                const showDatePanel = $radioButton.data("value") == "si" ? true : false;
                that.handleShowPanelVisibilityForPanelDates(showDatePanel);
            });	                        
        });    
        
        // Botón para confirmar la descarga de Traducciones de recursos vía modal
        configEventById(`${that.btnConfirmDownloadId}`, function(element){
            const confirmDownloadButton = $(element);
            confirmDownloadButton.off().on("click", function(){
                // Obtener rangos de fechas antes de descargar
                that.handleGetBetweenDatesForDownloadTranslate();

                // Proceder a descargar las traducciones vía modal
                that.handleDownloadTranslations(function(downloadOk){
                    // Ocultar el modal de descarga
                    downloadOk && $(`#${that.modalDownloadResourcesId}`).modal('hide');                                         
                });
            });	                        
        });    
        
        // Botón para subir el fichero seleccionado        
        configEventById(`${that.btnConfirmImportTranslateFileId}`, function(element){
            const btnImportTranslate = $(element);
            btnImportTranslate.off().on("click", function(){
                that.handleManageFileSelected(false);
            });	                        
        });  
        
        // Botón para validar el fichero seleccionado        
        configEventById(`${that.btnConfirmValidateTranslateFileId}`, function(element){
            const btnImportTranslate = $(element);
            btnImportTranslate.off().on("click", function(){
                that.handleManageFileSelected(true);
            });	                        
        });          
    },

    /**
     * Método para gestionar lo que se desea realizar con el fichero (Subir o Subir y validarlo)
     * @param {*} file 
     * @param {*} validateFile 
     */
    handleManageFileSelected: function(validateFile){
        const that = this;

        // Obtener el fichero a subir      
        const file = that.getFileFromInputFile(that.inputTranslateFile);
        if (file != undefined){
            // Importar o Validar el fichero
            // Datos a enviar para la validación/importación
            const dataPost = new FormData();
            // Indicar si se desea "validar" los datos 
            dataPost.append("file", file);                       
            dataPost.append("validar", validateFile);
                       
            // Mostrar loading de carga
            loadingMostrar();    
            const ajax = new XMLHttpRequest();
            ajax.open("POST", that.urlUploadTranslate, true);
            ajax.responseType = "blob";
            ajax.onreadystatechange = function () {
                if (this.readyState == 4) {
                    if (this.status == 200) {
                        const respuesta = this.statusText;
    
                        if (respuesta.indexOf("Error") > -1 || respuesta == null || this.response.size == 0) {
                            // Error en Backend / Ok si no hay respuesta y se ha pulsado en validar
                            if (validateFile){
                                mostrarNotificacion("success", "No se han encontrado errores en la validación del fichero de traducciones.");                            
                            }else{
                                // La respuesta puede ser 0 si se ha subido el fichero
                                if (!validateFile){
                                    // Importación correcta
                                    mostrarNotificacion("success", "El fichero de traducciones se ha importado correctamente.");
                                }else{
                                    mostrarNotificacion("error", that.messages.validar);
                                }                                
                            }                            
                            loadingOcultar();
                        }
                        else if (validateFile) {
                            const blob = new Blob([this.response], { type: "text/plain" });
                            mostrarNotificacion("error", that.messages.errorValidar);                                                  
                            saveAs(blob, "Errores.txt");
                            loadingOcultar();
                        } else {
                             const blob = new Blob([this.response], { type: "application/octet-stream" });
                            saveAs(blob, "traducciones.zip");
                             loadingOcultar();
                        }
                    }else if (this.status == 500){
                        validateFile 
                        ? mostrarNotificacion("error", that.messages.errorInternoValidar)
                        : mostrarNotificacion("error", that.messages.errorInternoImportar);
                        loadingOcultar();
                    }                   
                }
            };
            ajax.send(dataPost);                
        }else{
            validateFile 
            ? mostrarNotificacion("error", that.messages.errorNoHayArchivoValidar)
            : mostrarNotificacion("error", that.messages.errorNoHayArchivoImportar);
        }
    },


    /**
     * 
     */
    handleValidateTranslateFile: function(file){
        const that = this;
        // Validar el fichero seleccionado
        that.handleImportTranslateFile(file, true);
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
     * Método para controlar la visibilidad del panel para la selección o no de Fecha inicio y fin para la descarga de traducción de recursos
     * @param {*} showDatePanel 
     */
    handleShowPanelVisibilityForPanelDates: function(showDatePanel){
        const that = this;
        showDatePanel ? that.panelBetweenDates.removeClass("d-none") : that.panelBetweenDates.addClass("d-none");
    },

    /**
     * Método para realizar búsquedas de traducciones a descargar
     */
    handleSearchTranslateByName: function(){
        const that = this;

        let cadena = this.txtBuscarDescargaTraduccion.val();
        // Eliminamos posibles tildes para búsqueda ampliada
        cadena = cadena.normalize('NFD').replace(/[\u0300-\u036f]/g, "").toLowerCase();
        
        // Cada una de las filas que muestran la traducción        
        const translateRows = $(`.${that.translateRowClassName}`);        

        // Buscar dentro de cada fila       
        $.each(translateRows, function(index){
            const rowTranslate = $(this);
            // Seleccionamos el nombre de la traducción quitando tildes y caracteres extraños
            const translateName = $(this).find(".component-name").not('.d-none').html().trim().normalize("NFD").replace(/[\u0300-\u036f]/g, "").toLowerCase();
            if (translateName.includes(cadena)){
                // Mostrar fila resultado y sus respectivos padres
                rowTranslate.removeClass("d-none");
            }else{
                // Ocultar fila resultado
                rowTranslate.addClass("d-none");
            }            
        });
    },    

    /**
     * Método para mostrar el panel de traducciones correspondiente dependiendo del item seleccionado (Textos, XML, Recursos)
     */
    handleShowTranslateByTypeSelected: function(){
        const that = this;

        // Ocultar todos los contenedores excepto el seleccionado
        const translateLists = $(`.${that.translateListClassName}`);
        $.each(translateLists, function(index){
            const translateList = $(this);
            translateList.data("value") == that.translateTypeItem ? translateList.removeClass("d-none") : translateList.addClass("d-none"); 
        }); 

    },

    /**
     * Comprobar que hay items seleccionados para habilitar o no el botón de descarga múltiple
     */
    handleCheckTranslateSelected: function(){
        const that = this;

        // Vaciar posible selección anterior
        this.translateRowsSelected = [];
                
        // Selecciona todos los checkboxes seleccionados
        const translateCheckbox = $(`.${that.checkBoxDownloadSingleInputClassName}:checked`);

        // Habilitar/InHabilitar el botón de "Descarga de selección"
        translateCheckbox.length > 0 ? that.btnDownloadTranslateSelected.prop('disabled', false) : that.btnDownloadTranslateSelected.prop('disabled', true)        

        // Guardar los items seleccionados
        translateCheckbox.each(function() {
            const translateRow = $(this).closest(`.${that.translateRowClassName}`);
            that.translateRowsSelected = [...that.translateRowsSelected, translateRow];           
        });
    },


    /**
     * Método para marcar/desmarcar todos los items cuando se haga clic en el checkbox de "Todos"
     */
    handleCheckUncheckAllTranslateItems: function(){
        const that = this;

        // Controlar si se desean marcar los checkbox de traducciones que se están visualizando actualmente
        const setCheckboxAsSelected = $(`#${that.checkAllTextsId}`).prop('checked');

        // Desmarcar/Marcar todos los items        
        $(`.${that.checkBoxDownloadSingleInputClassName}`).each(function() {
            // Accede a cada checkbox individualmente
            const checkbox = $(this);
            // Desmarcarlo por defecto
            checkbox.prop('checked', false);

            // Activar sólo aquellas que están visibles
            if (setCheckboxAsSelected){
                const isContainerVisible = !$(this).closest(`.${that.translateListClassName}`).hasClass("d-none");
                isContainerVisible && checkbox.prop('checked', true);                
            }                                    
        });

        // Controlar el botón de descarga múltiple
        const translateCheckbox = $(`.${that.checkBoxDownloadSingleInputClassName}:checked`);

        if (translateCheckbox.length > 0){
            that.btnDownloadTranslateSelected.prop("disabled", false);
        }else{
            that.btnDownloadTranslateSelected.prop("disabled", true);
        }        
    },

    /**
     * Método para obtener o construir el item para la descarga de los ficheros
     */
    handleGetDownloadFileFromRows: function(downloadRows){
        const that = this;
        // Referencias a los posibles datos a descargar
        let OntologiasPrincipales = "";
        let OntologiaPrincipalXML = "";
        let OntologiasSecundarias = "";
        let taxonomy = "";        

        // Reseteo del objeto a enviar para descargar Traducciones
        that.dataPostDownloadItems = new FormData();
        // Reseteo de errores en el sistema para permitir o no descargar las traducciones
        that.isError = false;

        // Construir el objeto a enviar a partir de los items seleccionados a través de las rows
        $.each( downloadRows, function() {            
            // Cada una de las filas de las que se desean descargar los datos
            const row = $(this);
            const translateType = row.data("type");

            // Comprobar el Tipo (Recurso, Texto, XML )
            if (translateType == "text"){
                // Añadir el Nombre del "text"
                const translateName = row.data("value");
                that.dataPostDownloadItems.append(translateName, true);                
            }else if(translateType == "xml" || translateType == "resource" ){
                const nombreOntology = row.data("value");
                // Evaluar el tipo de XML a descargar
                if (nombreOntology.indexOf("ontologiaPrincipal_") >= 0) {
                    OntologiasPrincipales += nombreOntology.substring(nombreOntology.indexOf("_") + 1) + ',';
                }
                else if (nombreOntology.indexOf("ontologiaPrincipalXML_") >= 0) {
                    OntologiaPrincipalXML += nombreOntology.substring(nombreOntology.indexOf("_") + 1) + ',';
                }
                else if (nombreOntology.indexOf("ontologiaSecundaria_") >= 0) {
                    OntologiasSecundarias += nombreOntology.substring(nombreOntology.indexOf("_") + 1) + ',';
                }
                else if (nombreOntology.indexOf("taxonomy_") >= 0) {
                    taxonomy += nombreOntology.substring(nombreOntology.indexOf("_") + 1) + ',';
                }                

                // Revisar lista de Ontologias principales Recursos
                const mListaOntologiasPrincipales = OntologiasPrincipales.split(",");

                for (let i = 0; i < mListaOntologiasPrincipales.length ; i++) {
                    if (!mListaOntologiasPrincipales[i] == "") {
                        that.dataPostDownloadItems.append("mListaOntologiasPrincipales[" + mListaOntologiasPrincipales[i] + "]", mListaOntologiasPrincipales[i]);
                    }
                }                

                // Revisar lista de Ontologias principales XML
                const mListaOntologiaPrincipalXML = OntologiaPrincipalXML.split(",");

                for (let i = 0; i < mListaOntologiaPrincipalXML.length ; i++) {
                    if (!mListaOntologiaPrincipalXML[i] == "") {
                        that.dataPostDownloadItems.append("mListaOntologiasPrincipalesXML[" + mListaOntologiaPrincipalXML[i] + "]", mListaOntologiaPrincipalXML[i]);
                    }
                }             
            }    
        });             
    },

    /**
     * Método para obtener la información de las fechas para descargar los recursos deseados
     */
    handleGetBetweenDatesForDownloadTranslate: function(){
        const that = this;
        // Obtener la fecha inicio y fin si se ha seleccionado                  
        const downloadResourcesByDate = $(`.${that.radioButtonDownloadBetweenDateClassName}:checked`).data("value") == "si"  ? true : false;
        if (downloadResourcesByDate){                    
            const startDateValue = that.inputStartDate.val();
            const endDateValue = that.inputEndDate.val();

            // Controlar datos de la fecha Inicial
            const dateIn = new Date(startDateValue);
            // Controlar datos de la fecha Final
            const dateFin = new Date(endDateValue);            

            if (dateIn > dateFin || isNaN(dateIn) || isNaN(dateFin) ) {
                that.isError = true;                
                mostrarNotificacion("error", that.messages.errorFecha);                        
                return;
            }

            if (!isNaN(dateIn)) {
                dayIn = dateIn.getDate();
                monthIn = dateIn.getMonth() + 1;
                yearIn = dateIn.getFullYear();
                that.dataPostDownloadItems.append("fechaInicio", dayIn + '/' + monthIn + '/' + yearIn);
            }

            if (!isNaN(dateFin)) {
                dayFin = dateFin.getDate();
                monthFin = dateFin.getMonth() + 1;
                yearFin = dateFin.getFullYear();
                that.dataPostDownloadItems.append("fechaFin", dayFin + '/' + monthFin + '/' + yearFin);
            }
        }  
    },

    /**
     * Método para realizar la petición a descargar las traducciones deseadas
     */
    handleDownloadTranslations: function(completion = undefined){
        const that = this;

        // Proceder a la descarga si no hay errores
        if (that.isError == true){
            return;
        }

        // Mostrar loading
        loadingMostrar();

        // Petición realizada para descargar ficheros
        const ajax = new XMLHttpRequest();
        ajax.open("POST", that.urlDownloadTranslate, true);
        ajax.responseType = "blob";
        ajax.onreadystatechange = function () {
            if (this.readyState == 4) {
                if (this.status == 200) {
                    const respuesta = this.statusText;
                    if (respuesta.indexOf("Error") > -1 || respuesta == null || this.response.size == 0) {
                        mostrarNotificacion("error", that.messages.errorNoHayArchivos);
                        loadingOcultar();
                        // Ejecutar callBack si es necesario
                        completion != undefined && completion(false)
                    }
                    else {
                        var blob = new Blob([this.response], { type: "application/octet-stream" });
                        saveAs(blob, `${that.translateTypeFileName}.xlsx`);
                        loadingOcultar();
                        // Ejecutar callBack si es necesario
                        completion != undefined && completion(true)
                    }
                }
            }
        };
        
        ajax.send(that.dataPostDownloadItems);               
    },

}

/**
 * Operativa para la gestión de Cache
 */
const operativaGestionCache = {

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

    configRutas: function(){
        // Url para editar un certificado
        this.urlBase = refineURL();
        // Url para cargar los items de la caché de la Comunidad a partir de una búsqueda realizada
        this.urlLoadCacheItems = `${this.urlBase}/load-cache-items`;
        // Url para borrar caché de la comunidad (Todo)
        this.urlDeleteCommunityCache = `${this.urlBase}/borrar-todo`;
        // Url para borrar caché item
        this.urlDeleteCacheItem = `${this.urlBase}/borrar`;
        // Url para borrar caché de recursos (Toda)
        this.urlDeleteResourceCache = `${this.urlBase}/borrar-cache-recursos`;        
        // Url para borrar caché item de tipo Recurso
        this.urlDeleteCacheResourceItem = `${this.urlBase}/borrar-recurso`;               
        // Url para borrar la caché de los recuross RDF (Toda)
        this.urlDeleteCacheResourcesXML = `${this.urlBase}/borrar-RDF`;
        // Url para borrar caché item de tipo Recurso RDF
        this.urlDeleteCacheRdfResourceItem = `${this.urlBase}/borrar-RDF-recursos`;
        // Url para borrar caché de búsquedas
        this.urlDeleteSearchCache = `${this.urlBase}/borrar-busquedas`;
        // Url para guardar la configuración de las cachés de búsqueda activas
        this.urlSaveCache = `${this.urlBase}/guardar-configuracion-cache`;
        // Url para borrar caché seleccionado
        this.urlDeleteSelectedCache = `${this.urlBase}/borrar-seleccionados`;

    },

    /*
     * Inicializar elementos de la vista
     * */
    config: function (pParams) {
        
        // Contenedor modal
        this.modalContainer = $("#modal-container");  
        this.modalDeleteCacheCommunity = $("#modalDeleteCacheCommunity");
        this.modalDeleteClassName = "modal-confirmDelete";

        // Botón para gestión de borrado (Comunidad, Recursos, RecursosRDF)
        this.btnDeleteCacheCommunityId = "btnDeleteCacheCommunity"; 
        this.btnDeleteCacheCommunity = $(`#${this.btnDeleteCacheCommunityId}`);
        this.btnDeleteCacheResourcesId = "btnDeleteCacheResources"; 
        this.btnDeleteCacheResources = $(`#${this.btnDeleteCacheResourcesId}`);                 
        this.btnDeleteCacheRdfResourcesId = "btnDeleteCacheRdfResources"; 
        this.btnDeleteCacheRdfResources = $(`#${this.btnDeleteCacheRdfResourcesId}`);          

        // Contenido del modal Borrado Caché de Comunidad
        // RadioButton para selección del tipo de borrado (Total o selección -> Sí / No)
        this.radioButtonDeleteCacheCommunityClassName = "deleteCacheCommunity";
        // Botón para confirmar el borrado de la caché de toda la comunidad
        this.btnConfirmDeleteAllCacheCommunityId = "btnConfirmDeleteAllCacheCommunity";
        this.btnConfirmDeleteAllCacheCommunity= $(`#${this.btnConfirmDeleteAllCacheCommunityId}`);

        // Botón para confirmar el borrado de la cache de recursos de toda la comunidad
        this.btnConfirmDeleteAllResourceCacheId = "btnConfirmDeleteAllResourceCache";
        this.btnConfirmDeleteAllResourceCache = $(`#${this.btnConfirmDeleteAllResourceCacheId}`);

        // Botón para confirmar el borrado de la cache de recursos de toda la comunidad
        this.btnConfirmDeleteAllResourceRdfCacheId = "btnConfirmDeleteAllResourceRdfCache";
        this.btnConfirmDeleteAllResourceRdfCache = $(`#${this.btnConfirmDeleteAllResourceRdfCacheId}`);        

        // Input para búsquedas de Caché/Comunidad (Comunidad, Recursos y Recursos Caché)
        this.inputTextBuscarCacheClassName = "txtBuscarCache";
        // Botón para la eliminación de una determinada cache de comunidad
        this.btnDeleteCommunityCacheElementClassName = "btnDeleteCommunityCacheElement";
        // Cada una de las filas listadas de Caché filtradas a partir del buscador
        this.elementRowClassName = "element-row";
        // Panel para realizar búsquedas de cachés
        this.panelCommunityCacheFilterClassName = "panelCommunityCacheFilter";
        // Panel o contenedor de items de la caché
        this.panelCacheFilterContainerClassName = "panelCacheFilter__container";
        // Panel de acciones (Todo y Selección) para borrado de cachés
        this.panelDeleteAllCacheCommunityActionsClassName = "panelDeleteAllCacheCommunityActions";
        this.panelDeleteSelectiveCacheCommunityActionsClassName = "panelDeleteSelectiveCacheCommunityActions";
        // TextArea para la selección múltiple de cache de recursos RDF
        this.txtAreaCacheResourceRdfListId = "txtAreaCacheResourceRdfList"; 
        this.txtAreaCacheResourceRdfList = $(`#${this.txtAreaCacheResourceRdfListId}`);
        // Botón para confirmar el borrado selectivo de cache de recursos RDF
        this.btnBorrarSeleccionRdfId = "btnBorrarSeleccionRDF";
        this.btnBorrarSeleccionRdf = $(`#${this.btnBorrarSeleccionRdfId}`);

        // Contenido del modal para confirmar borrado de Caché de búsquedas
        this.btnBorrarCacheBusquedasId = "btnBorrarCacheBusquedas"; 
        this.btnBorrarCacheBusquedas = $(`#${this.btnBorrarCacheBusquedasId}`);

        this.btnGuardarCachesId = "btnGuardarConfiguracionCachesBusqueda";
        this.btnGuardarCaches = $(`#${this.btnGuardarCachesId}`);

        // Flag para guardar el tipo de borrado a realizar (Selección o Todo). Por defecto, borrar todo
        this.deleteAllCommunityCache = true;
        // Flags para detectar el tipo de borrado a realizar
        this.typeDeleteCommunity = false;
        this.typeDeleteResources = false;
        this.typeDeleteRdfResources = false;   
        

        //Botón para marcar todos los checkboxes
        this.btnCheckAllSearchResultsId = "btnCheckAllSearchResultsId";
     
        //Botón para desmarcar todos los checkboxes
        this.btnUncheckAllSearchResultsId = "btnUncheckAllSearchResultsId";
     
        //Botón para eliminar el cache seleccionado (checkbox marcados)
        this.btnDeleteSelectedCacheId = "btnConfirmDeleteSelectedCache";
       
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
        
        // Controlar el cierre del de cierre de cachés
        $(`.${this.modalDeleteClassName}`).on('show.bs.modal', (e) => {
            // Aparición del modal
        })
        .on('hide.bs.modal', (e) => {
            // Resetear el tipo de borrado (Comunidad, Recursos o RecursosRDF)
            that.resetCacheTypeToDelete();
        }); 

        // Botón para establecer el tipo de borrado a gestionar (Comunidad)
        configEventById(`${that.btnDeleteCacheCommunityId}`, function(element){
            const $input = $(element);
            $input.off().on("click", function(){                 
                that.typeDeleteCommunity = true;                          
            });	                
        });

        // Botón para establecer el tipo de borrado a gestionar (Recursos)
        configEventById(`${that.btnDeleteCacheResourcesId}`, function(element){
            const $input = $(element);
            $input.off().on("click", function(){                 
                that.typeDeleteResources = true;                          
            });	                
        }); 

        // Botón para establecer el tipo de borrado a gestionar (RecursosRDF)
        configEventById(`${that.btnDeleteCacheRdfResourcesId}`, function(element){
            const $input = $(element);
            $input.off().on("keyup", function(){                 
                that.typeDeleteRdfResources = true;                          
            });	                
        });    

        configEventById(`${that.btnGuardarCachesId}`, function (element) {
            const $input = $(element);
            $input.off().on("click", function () {
                that.handleSaveConfigCache();  
            });
        });
        
        // RadioButton para controlar la selección del borrado de caché de la Comunidad (Selección / Todo)
        configEventByClassName(`${that.radioButtonDeleteCacheCommunityClassName}`, function(element){
            const $radioButton = $(element);
            $radioButton.off().on("click", function(){                 
                // Guardar el tipo de borrado en deleteAllCommunityCache                
                that.handleSelectCacheCommunityTypeToDelete($radioButton);            
                // Mostrar u ocultar el panel 
                const modal = $radioButton.closest(`.${that.modalDeleteClassName}`);
                that.handleShowHideCommunitySearchCachePanel(modal);    
            });	                        
        });  

        // Buscador para cargar los elementos de la caché de la comunidad
        configEventByClassName(`${that.inputTextBuscarCacheClassName}`, function(element){
            const $input = $(element);
            $input.off().on("keyup", function(){                 
                clearTimeout(that.timer);
                that.timer = setTimeout(function () {                                                                        
                    // Guardar el tipo de borrado en deleteAllCommunityCache              
                    that.handleLoadCommunityCacheItems($input, that.typeDeleteCommunity, that.typeDeleteResources, that.typeDeleteRdfResources);                                          
                }, 500);                            
            });	                
        });        

        const input = document.getElementById("vacio");
        const divMostrar = document.getElementById("divCheck");

        input.addEventListener("input", function () {
            if (input.value.length === 0) {
                divMostrar.style.display = "none";
            } else {
                divMostrar.style.display = "inline-block";
            }
        });

        // Elemento para seleccionar todos los resultados de filtrado
        $('#btnCheckAllSearchResultsId').change(function () {
            if ($(this).prop('checked') == true) {
                that.handleCheckAllSearchResults();
            }
            else {
                that.handleUncheckAllSearchResults();
            }

        });

        // Elemento para desmarcar todos los resultados de filtrado
        configEventById(`${that.btnUncheckAllSearchResultsId}`, function (element) {
            const $checkItem = $(element);
            $checkItem.off().on("click", function () {
                that.handleUncheckAllSearchResults();
            });
        });

        // Elemento para eliminar todos los items seleccionados
        configEventById(`${that.btnDeleteSelectedCacheId}`, function (element) {
            const $checkItem = $(element);
            $checkItem.off().on("click", function () {
                let listaSeleccionados = that.handleGetSelectedItems();
                that.handleDeleteCacheItemsSelected(listaSeleccionados);
            });
        });

        // Elemento para eliminar cada item caché listado en el modal
        configEventById(`${that.btnDeleteCommunityCacheElementClassName}`, function(element){
            const $deleteItem = $(element);
            $deleteItem.off().on("click", function(){                                 
                // Guardar el tipo de borrado en deleteAllCommunityCache
                that.handleDeleteCacheItem($deleteItem);                
            });	                
        });  
                
        // Elemento para borrar la caché entera de la Comunidad
        configEventById(`${that.btnConfirmDeleteAllCacheCommunityId}`, function(element){
            const $deleteButton = $(element);
            $deleteButton.off().on("click", function(){                                                                
                that.handleDeleteCommunityCache($deleteButton);
            });	                
        });  

        // Elemento para borrar la caché de búsquedas de la Comunidad
        configEventById(`${that.btnBorrarCacheBusquedasId}`, function(element){
            const $deleteButton = $(element);
            $deleteButton.off().on("click", function(){                                                                
                that.handleDeleteSearchCache($deleteButton);
            });	                
        });  
        
        // Elemento para borrar la caché de recursos
        configEventById(`${that.btnConfirmDeleteAllResourceCacheId}`, function(element){
            const $deleteButton = $(element);
            $deleteButton.off().on("click", function(){                                                                
                that.handleDeleteResourceCache($deleteButton);
            });	                
        }); 

        // Elemento para borrar la caché de recursos Rdf
        configEventById(`${that.btnConfirmDeleteAllResourceRdfCacheId}`, function(element){
            const $deleteButton = $(element);
            $deleteButton.off().on("click", function(){                                                                
                that.handleDeleteCacheResourceRdf($deleteButton);
            });	                
        });         
        
        
        // Elemento para borrar la selección de cache de recursos RDF
        configEventById(`${that.btnBorrarSeleccionRdfId}`, function(element){
            const $deleteButton = $(element);
            $deleteButton.off().on("click", function(){                                                                
                that.handleDeleteResourceRdfCacheItems($deleteButton);
            });	                
        });         
        
        // TextArea para el borrado múltiple de cache de recursos RDF
        configEventById(`${that.txtAreaCacheResourceRdfListId}`, function(element){
            const $textArea = $(element);
            $textArea.off().on("blur", function(){                                                                
                that.handleFormatCacheResourceRdfList($textArea);
            });	                
        });  
        // TextArea para el borrado múltiple de cache de recursos RDF
        configEventById(`${that.txtAreaCacheResourceRdfListId}`, function(element){
            const $textArea = $(element);
            $textArea.on("input", function(){                                                                
                that.handleEnableOrDisabledDeleteCacheResourceRdfButton($textArea);
            });	                
        });                      
    },

    /**
     * Método para eliminar la selección de recursos RDF a partir de lo introducido en el textarea
     * @param {*} $deleteButton Botón pulsado para ejecutar el borrado de la caché
     */
    handleDeleteResourceRdfCacheItems: function($deleteButton){
        const that = this;

        const resourceRdfList = that.txtAreaCacheResourceRdfList.val();
        
        // Objeto con los datos de recursos RDF a enviar
        const dataPost = {
            pRecursos: resourceRdfList
        }

        // Mostrar loading
        loadingMostrar();
        // Modal a ocultar cuando se ejecute el borrado
        const modal = $deleteButton.closest(`.${that.modalDeleteClassName}`);
        
        GnossPeticionAjax(            
            that.urlDeleteCacheRdfResourceItem,
            dataPost,
            true
        ).done(function (data) {
            // OK 
            dismissVistaModal(modal);                                              
            setTimeout(function(){
                mostrarNotificacion("success", "La caché de los recursos RDF seleccionados ha sido borrada correctamente.");  
            },1000);                                      
        }).fail(function (data) {            
            mostrarNotificacion("error", data);
        }).always(function () {
            // Ocultar el loading
            loadingOcultar();
        });
    },

    handleSaveConfigCache: function () {
        const that = this;

        let cachesActivas = $('#CachesDeBusquedasActivas').is(":checked");
        let cachesAnonimas = $('#CachesAnonimas').is(":checked");;
        let duracionConsulta = $('#DuracionConsulta').val();
        let tiempoExpiracion = $('#TiempoExpiracion').val();
        let tiempoExpiracionCachesDeUsuario = $('#TiempoExpiracionCachesUsuarios').val();
        let tiempoRecalcularCaches = $('#TiempoRecalcularCaches').val();

        const dataPost = {
            pCacheBusquedaActiva: cachesActivas,
            pCachesAnonimas: cachesAnonimas,
            pDuracionConsulta: duracionConsulta,
            pTiempoExpiracion: tiempoExpiracion,
            pTiempoExpiracionCachesDeUsuario: tiempoExpiracionCachesDeUsuario,
            pTiempoRecalcularCaches: tiempoRecalcularCaches
        }

        loadingMostrar();
        GnossPeticionAjax(
            that.urlSaveCache,
            dataPost,
            true
        ).done(function (data) {
            setTimeout(function () {
                mostrarNotificacion("success", "La configuración de las cachés se ha guardado correctamente.");
            }, 1000);
        }).fail(function (data) {
            mostrarNotificacion("error", data);
        }).always(function () {
            loadingOcultar();
        });
    },

    /**
     * Método para formatear lo que el usuario añada en el textArea
     * @param {*} $textArea TextArea donde se copiará los IDs de los recursos RDF para su posterior eliminación
     */
    handleFormatCacheResourceRdfList: function($textArea){
        const that = this;

        //Obtener el texto del textarea
        let currentTextAreaValue = $textArea.val(); 
        // Dividir el texto en un array de elementos utilizando espacios y comas como separadores
        const idElements = currentTextAreaValue.split(/[ ,]+/); 
        // Unir los elementos con saltos de línea como separadores
        currentTextAreaValue = idElements.join("\n"); 
        $textArea.val(currentTextAreaValue);               
    },

    /**
     * Método para habilitar o deshabilitar el botón de borrado de recursos RDF de selección múltiple
     * siempre y cuando haya algún caracter escrito
     */
    handleEnableOrDisabledDeleteCacheResourceRdfButton: function($textArea){
        const that = this;

        const currentTextAreaValue = $textArea.val();
        // Habilitar o deshabilitar el botón para borrar selección de RDF
        currentTextAreaValue.trim().length > 0 
        ? that.btnBorrarSeleccionRdf.prop('disabled', false) 
        : that.btnBorrarSeleccionRdf.prop('disabled', true);          
    },

      

    /**
     * Método para resetear el tipo de caché a eliminar para saber qué peticiones hacer y poder reutilizar las vistas existentes
     */
    resetCacheTypeToDelete: function(){
        const that = this;

        // Flags para detectar el tipo de borrado a realizar
        that.typeDeleteCommunity = false;
        that.typeDeleteResources = false;
        that.typeDeleteRdfResources = false;
    },


    /**
     * Método para eliminar toda la caché de búsquedas. Esta acción se ejecuta desde el modal.
     * @param {*} $deleteButton Botón pulsado para ejecutar el borrado de la caché
     */
    handleDeleteSearchCache: function($deleteButton){
        const that = this;

        // Mostrar loading
        loadingMostrar();
        // Modal a ocultar cuando se ejecute el borrado
        const modal = $deleteButton.closest(`.${that.modalDeleteClassName}`);
        
        GnossPeticionAjax(            
            that.urlDeleteSearchCache,
            null,
            true
        ).done(function (data) {
            // OK 
            dismissVistaModal(modal);                                              
            setTimeout(function(){
                mostrarNotificacion("success", "La caché de búsquedas ha sido borrada correctamente.");  
            },1000);                                      
        }).fail(function (data) {            
            mostrarNotificacion("error", data);
        }).always(function () {
            // Ocultar el loading
            loadingOcultar();
        });
    },

    /**
     * Método para eliminar toda la caché de recursos. Esta acción se ejecuta desde el modal.
     * @param {*} $deleteButton Botón pulsado para ejecutar el borrado de la caché
     */
    handleDeleteResourceCache: function($deleteButton){
        const that = this;

        // Mostrar loading
        loadingMostrar();
        // Modal a ocultar cuando se ejecute el borrado
        const modal = $deleteButton.closest(`.${that.modalDeleteClassName}`);
        
        GnossPeticionAjax(            
            that.urlDeleteResourceCache,
            null,
            true
        ).done(function (data) {
            // OK 
            // Eliminar posible contenido si se han buscado items previamente
            modal.find(`.${that.panelCacheFilterContainerClassName}`).children().remove();            
            dismissVistaModal(modal);                                              
            setTimeout(function(){
                mostrarNotificacion("success", "La caché de recursos ha sido borrada correctamente.");  
            },1000);                                      
        }).fail(function (data) {            
            mostrarNotificacion("error", data);
        }).always(function () {
            // Ocultar el loading
            loadingOcultar();
        });
    },       
    
    /**
     * Método para eliminar toda la caché de la comunidad. Esta acción se ejecuta desde el modal, eligiendo borrado total de la cache de la comunidad
     * @param {*} $deleteButton Botón pulsado para ejecutar el borrado de la caché
     */
    handleDeleteCommunityCache: function($deleteButton){
        const that = this;

        // Mostrar loading
        loadingMostrar();
        // Modal a ocultar cuando se ejecute el borrado
        const modal = $deleteButton.closest(`.${that.modalDeleteClassName}`);

        GnossPeticionAjax(            
            that.urlDeleteCommunityCache,
            null,
            true
        ).done(function (data) {
            // OK 
            dismissVistaModal(modal);                                              
            // Eliminar posible contenido si se han buscado items previamente
            modal.find(`.${that.panelCacheFilterContainerClassName}`).children().remove();
            setTimeout(function(){
                mostrarNotificacion("success", "La caché de la comunidad ha sido borrada correctamente.");  
            },1000);                                      
        }).fail(function (data) {            
            mostrarNotificacion("error", data);
        }).always(function () {
            // Ocultar el loading
            loadingOcultar();
        });
    },

    /**
     * Método para eliminar un elemento item de una búsqueda realizada en concreto
     */
    handleDeleteCacheItem: function($deleteItem){
        const that = this;
                
        // Fila de la caché a eliminar
        const cacheRow = $deleteItem.closest(`.${that.elementRowClassName}`);
        // Clave a eliminar
        const cacheId = $deleteItem.data("value");
        // Url donde se realizará el borrado dependiendo del tipo de item seleccionado (Comunidad, Recurso, RecursoRDF)
        let url = "";
        if (that.typeDeleteCommunity == true){
            // Borrar elemento de comunidad
            url = that.urlDeleteCacheItem;
        }else if (that.typeDeleteResources == true){            
            url = that.urlDeleteCacheResourceItem;
        }        
                
        // Construir el objeto para eliminación
        const dataPost = {
            clave: cacheId,
        };   

        // Mostrado de loading                           
        loadingMostrar(cacheRow);
             
        GnossPeticionAjax(            
            url,
            dataPost,
            true
        ).done(function (data) {
            // OK              
            // Eliminar la fila eliminada        
            mostrarNotificacion("success", "El elemento de la caché ha sido borrado correctamente.");                        
            // Eliminar la fila una vez se ha procedido a su eliminación
            setTimeout(function(){
                cacheRow.remove();
            },1000);
        }).fail(function (data) {            
            mostrarNotificacion("error", data);
        }).always(function () {
            // Ocultar el loading
            loadingOcultar();
        });        
    },

    /**
     * Método para eliminar un elemento item de una búsqueda realizada en concreto cuyo checkbox está marcado para su eliminación
     */
    handleDeleteCacheItemForChbx: function (deleteItem) {
        const that = this;

        // Url donde se realizará el borrado dependiendo del tipo de item seleccionado (Comunidad, Recurso, RecursoRDF)
        let url = that.urlDeleteSelectedCache;
       
        //Recoger values de la lista de elementos
        let valuesItems = [];
        deleteItem.forEach(function (checkElement) {
            valuesItems.push(checkElement.value);
        });

        // Construir el objeto para eliminación
        const dataPost = {
            claves: valuesItems,
        };

        let cacheRow;
        //let $elementParse;
        
        GnossPeticionAjax(
            url,
            dataPost,
            true
        ).done(function (data) {
            //Eliminar todas las filas 
            deleteItem.forEach(function (checkElement) {
               
                cacheRow = checkElement.closest(`.${that.elementRowClassName}`);
                cacheRow.remove();
            });
            mostrarNotificacion("success", "Elementos de la caché han sido borrados correctamente.");

        }).fail(function (data) {
            mostrarNotificacion("error", data);
        }).always(function () {
            // Ocultar el loading
            loadingOcultar();
        });
    },

    /**
     * Método para eliminar items seleccionados
     */
    handleDeleteCacheItemsSelected: function (selectedItems) {
        const that = this;

        if (selectedItems.length > 0) {
            loadingMostrar();
            that.handleDeleteCacheItemForChbx(selectedItems);
           // mostrarNotificacion("success", "Elementos de la caché han sido borrados correctamente.");
        } else {
            mostrarNotificacion("error", "Ningún elemento seleccionado");
        }
                        
    },
    
    /**
     * Método para seleccionar/marcar todos los resultados de la búsqueda
     */
    handleCheckAllSearchResults: function() {
        document.querySelectorAll('.id-added-element-list input[type=checkbox]').forEach(function (checkElement) {
            checkElement.checked = true;
        });
    },

    /**
    * Método para desmarcar todos los resultados de la búsqueda
    */
    handleUncheckAllSearchResults: function() {
        document.querySelectorAll('.id-added-element-list input[type=checkbox]').forEach(function (checkElement) {
            checkElement.checked = false;
        });
    },

    /**
    * Método para recoger en lista todos los elementos con checkbox marcado
    */
    handleGetSelectedItems: function() {
        let seleccionados = [];
        document.querySelectorAll('.id-added-element-list input[type=checkbox]').forEach(function (checkElement) {

            if (checkElement.checked == true) {
                seleccionados.push(checkElement);
            }

        });

        return seleccionados;
    },

    /**
     * Método para eliminar toda la cache de recursos RDF
     */
    handleDeleteCacheResourceRdf: function($deleteButton){
        const that = this;
            
        // Mostrar loading
        loadingMostrar();
        // Modal a ocultar cuando se ejecute el borrado
        const modal = $deleteButton.closest(`.${that.modalDeleteClassName}`);
        
        GnossPeticionAjax(            
            that.urlDeleteCacheResourcesXML,
            null,
            true
        ).done(function (data) {
            // OK 
            dismissVistaModal(modal);                                              
            setTimeout(function(){
                mostrarNotificacion("success", "La caché de recursos RDF ha sido borrada correctamente.");  
            },1000);                                      
        }).fail(function (data) {            
            mostrarNotificacion("error", data);
        }).always(function () {
            // Ocultar el loading
            loadingOcultar();
        });    
    },
    
    /**
     * Método para cargar y listar los elementos caché de una comunidad en base a la búsqueda
     * @param {*} $input Input utilizado para realizar la búsqueda
     * @param {bool} searchCommunityCache Indica si la petición es para realizar la carga de elementos de la caché de la comunidad
     * @param {bool} searchResourcesCache Indica si la petición es para realizar la carga de elementos de la caché de recursos
     * @param {bool} searchRdfResourcesCache Indica si la petición es para realizar la carga de elementos de la caché de recursos RDF
     */
    handleLoadCommunityCacheItems: function($input, searchCommunityCache, searchResourcesCache, searchRdfResourcesCache ){
        const that = this;

        // Modal y contenedor donde se cargarán los items nuevos
        const modal = $input.closest(`.${that.modalDeleteClassName}`);
        const panelContainerCacheItems = modal.find(`.${that.panelCacheFilterContainerClassName}`);
        // Panel para mostrado del loading en la búsqueda
        const panelCommunityCacheFilter = modal.find(`.${that.panelCommunityCacheFilterClassName}`);

        // Cadena introducida para filtrar/buscar
        let cadena = $input.val();
        // Eliminamos posibles tildes para búsqueda ampliada
        cadena = cadena.normalize('NFD').replace(/[\u0300-\u036f]/g, "").toLowerCase();

        // No permitir búsquedas con textos vacíos
        if (!cadena.length > 0){
            return;
        }

        // Construir el objeto para la búsqueda de Elementos Cache 
        const dataPost = {
            cacheSearch: cadena,
            cacheCommunity: searchCommunityCache,
            cacheResources: searchResourcesCache
        };
        
        // Mostrado de loading    
        loadingMostrar(panelCommunityCacheFilter);
             
        GnossPeticionAjax(            
            that.urlLoadCacheItems,
            dataPost,
            true
        ).done(function (data) {
            // OK 
            // Añadir los items a la vista                    
            panelContainerCacheItems.html(data);
        }).fail(function (data) {            
            mostrarNotificacion("error", data);
        }).always(function () {
            // Ocultar el loading
            loadingOcultar();
        });            
    },


    /**
     * Método para mostrar u ocultar el panel para la búsqueda de caches
     * @param {*} modal 
     */    
    handleShowHideCommunitySearchCachePanel: function(modal){
        const that = this;

        // Panel de búsquedas
        const searchCachePanel = modal.find(`.${that.panelCommunityCacheFilterClassName}`);
        // Panel de acciones
        const actionsDeleteAllCachePanel = modal.find(`.${that.panelDeleteAllCacheCommunityActionsClassName}`);
        const actionsDeleteSelectiveCachePanel = modal.find(`.${that.panelDeleteSelectiveCacheCommunityActionsClassName}`);
        
        // Mostrar u ocultar el panel y panel de acciones
        if (that.deleteAllCommunityCache == true){
            // Ocultar la selección de cache y acciones de cancelar
            searchCachePanel.addClass("d-none");
            actionsDeleteSelectiveCachePanel.addClass("d-none");
            actionsDeleteAllCachePanel.removeClass("d-none");
        }else{
            // Mostrar la selección de cache y acciones de cancelar
            searchCachePanel.removeClass("d-none");
            actionsDeleteSelectiveCachePanel.removeClass("d-none");
            actionsDeleteAllCachePanel.addClass("d-none");            
        }
    },

    /**
     * Método para mostrar u ocultar el tipo de borrado de Caché (Todo o Selección)
     * @param {*} input RadioButton que ha sido seleccionado
     */
    handleSelectCacheCommunityTypeToDelete: function(input){
        const that = this;
        that.deleteAllCommunityCache = input.data("value") == "si" ? true : false;
    },
}



/**
 *  Operativa para la descarga de configuraciones
 * */
const operativaDescargaConfiguracion = {
    /**
    * Inicializar operativa
    */
    init: function (pParams) {
        this.pParams = pParams;
        this.config(pParams);
        this.configEvents();
        this.configRutas();
    },

    configRutas: function () {
        // Url base
        this.urlBase = refineURL();
        // Url para descargar las configuraciones
        this.urlDownloadConfig = `${this.urlBase}/download`;
    },

    /*
     * Inicializar elementos de la vista
     * */
    config: function (pParams) {

        // Input para buscar items 
        this.txtBuscarDescargaConfig = $('#txtBuscarDescargaConfiguracion');

        // Botones para descargar cada una de las configuraciones
        this.btnDownloadSingleConfigClassName = 'btnDownloadSingleConfig';
        // Cada una de las filas que contienen las configuraciones
        this.configRowClassName = "config-row";

        // Botón para descarga la selección de las configuraciones
        this.btnDownloadConfigId = "btnDownloadConfig";
        this.btnDownloadConfig = $(`#${this.btnDownloadConfigId}`);


        // Checkbox para marcar o desmarcar todos los items
        this.checkAllTextsId = "checkAllTexts";
        this.checkAllTexts = $(`#${this.checkAllTextsId}`);

        // Contenedores de las config 
        this.configListClassName = "component-config-list";

        // Checkbox de cada uno de los inputs (Single checkbox) para marcar o desmarcar la selección
        this.checkBoxDownloadSingleInputClassName = "input-download-item";


        // Objeto que almacenará los items a enviar para su guardado
        this.dataPostDownloadItems = new FormData();
        // Array para almacenar los items que se seleccionarán
        this.configRowsSelected = [];


        // Flag para guardar la existencia de errores
        this.isError = false;
    },


    /**
     * Configuración de eventos de elementos del Dom (Botones, Inputs...)     
     */
    configEvents: function (pParams) {
        const that = this;

        // Botón para la descarga de configuraciones
        this.btnDownloadConfig.on("click", function () {
            that.handleDownloadConfig();
        });

        // Botón para la descarga de una única configuración
        configEventByClassName(`${that.btnDownloadSingleConfigClassName}`, function (element) {
            const btnSingleConfigDownload = $(element);
            btnSingleConfigDownload.off().on("click", function () {
                // Array de rows a descargar. En este caso, sólo hay una fila (Descargar vía botón)              
                const downloadRows = [btnSingleConfigDownload.closest(`.${that.configRowClassName}`)];

                that.handleDownloadSingleConfig(downloadRows);
            });
        });


        // Input para buscar configuraciones
        this.txtBuscarDescargaConfig.off().on("keyup", function (event) {
            clearTimeout(that.timer);
            that.timer = setTimeout(function () {
                that.handleSearchConfigByName();
            }, 500);
        });  


 
        // Checkbox de cada configuración
        configEventByClassName(`${that.checkBoxDownloadSingleInputClassName}`, function (element) {
            const configCheckbox = $(element);
            configCheckbox.off().on("change", function () {
                that.handleCheckConfigSelected();
            });
        });


        // Checkbox de selección múltiple
        configEventById(`${that.checkAllTextsId}`, function (element) {
            const $checkAllTextsId = $(element);
            $checkAllTextsId.off().on("change", function () {
                that.handleCheckUncheckAllConfigItems();
            });
        });                 
    },


   /**
    * Método para obtener las selecciones para la descarga de configuraciones mediante petición a backEnd
    */
    handleDownloadConfig: function () {
        const that = this;

        // Listado de configuraciones a descargar
        const pListaConfiguracion = new FormData();

        // Listado de configuraciones seleccionadas
        const listSelectedConfig = $(`input[type="checkbox"].${that.checkBoxDownloadSingleInputClassName}:checked`);

        // Recorrer los items/checkbox activos para crear el objeto
        $.each(listSelectedConfig, function (index) {
            const service = $(this);
            // Se añaden los checks seleccionados al pListaConfiguracion

            const prefijoClave = `pListaConfiguracion[${index}]`;

            pListaConfiguracion.append(prefijoClave + '.Nombre', service.data("name"));
            pListaConfiguracion.append(prefijoClave + '.Seleccionada', service.is(':checked'));
            pListaConfiguracion.append(prefijoClave + '.Url', service.data("url"));
        });

        // Comprobar que haya checks seleccionados
        if (listSelectedConfig.length == 0) {
            mostrarNotificacion("info", "Selecciona al menos una configuración para descagar");
            return;
        }

        loadingMostrar();
        // Petición ajax
        const ajax = new XMLHttpRequest();
        ajax.open("POST", that.urlDownloadConfig, true);
        ajax.responseType = "blob";
        ajax.onreadystatechange = function () {
            if (this.readyState == 4) {
                if (this.status == 200) {
                    const respuesta = this.statusText;
                    if (respuesta.indexOf("Error") > -1 || respuesta == null || this.response.size == 0) {
                        // KO
                        mostrarNotificacion("error", "Se ha producido un error en la descarga del fichero zip");
                        loadingOcultar();
                    }
                    else {
                        //OK
                        var blob = new Blob([this.response], { type: "application/octet-stream" });
                        saveAs(blob, 'configuraciones.zip');
                        mostrarNotificacion("success", "Descarga completada");
                        loadingOcultar();
                    }
                }
            }
        };
        ajax.send(pListaConfiguracion); 
    },

    /**
    * Método para descargar una única configuración
    */
    handleDownloadSingleConfig: function (downloadRows) {
        const that = this;
        const pListaConfiguracion = new FormData();

        const prefijoClave = 'pListaConfiguracion[0]';
        
        pListaConfiguracion.append(prefijoClave + '.Nombre', downloadRows[0].data("name"));
        pListaConfiguracion.append(prefijoClave + '.Seleccionada', true);
        pListaConfiguracion.append(prefijoClave + '.Url', downloadRows[0].data("url"));

        loadingMostrar();
        // Petición ajax
        const ajax = new XMLHttpRequest();
        ajax.open("POST", that.urlDownloadConfig, true);
        ajax.responseType = "blob";
        ajax.onreadystatechange = function () {
            if (this.readyState == 4) {
                if (this.status == 200) {
                    const respuesta = this.statusText;
                    if (respuesta.indexOf("Error") > -1 || respuesta == null || this.response.size == 0) {
                        // KO
                        mostrarNotificacion("error", "Se ha producido un error en la descarga del fichero zip");
                        loadingOcultar();
                    }
                    else {
                        //OK
                        var blob = new Blob([this.response], { type: "application/octet-stream" });
                        saveAs(blob, 'configuraciones.zip');
                        mostrarNotificacion("success", "Descarga completada");
                        loadingOcultar();
                    }
                }
                else if (this.status == 500)
                {
                    mostrarNotificacion("error", "Se ha producido un error en la descarga del fichero zip");
                }
            }
        };
        ajax.send(pListaConfiguracion); 
    },



    /*
     * Método para la búsqueda de configuraciones para descargar
     */
    handleSearchConfigByName: function () {
        const that = this;

        let cadena = this.txtBuscarDescargaConfig.val();
        // Eliminamos posibles tildes para búsqueda ampliada
        cadena = cadena.normalize('NFD').replace(/[\u0300-\u036f]/g, "").toLowerCase();

        // Cada una de las filas que muestran la traducción        
        const configRows = $(`.${that.configRowClassName}`);

        // Buscar dentro de cada fila       
        $.each(configRows, function (index) {
            const rowConfig = $(this);
            // Seleccionamos el nombre de la traducción quitando tildes y caracteres extraños
            const configName = $(this).find(".component-name").not('.d-none').html().trim().normalize("NFD").replace(/[\u0300-\u036f]/g, "").toLowerCase();
            if (configName.includes(cadena)) {
                // Mostrar fila resultado y sus respectivos padres
                rowConfig.removeClass("d-none");
            } else {
                // Ocultar fila resultado
                rowConfig.addClass("d-none");
            }
        });
    },


    handleCheckConfigSelected: function () {
        const that = this;

        // Vaciar posible selección anterior
        this.configRowsSelected = [];

        // Selecciona todos los checkboxes seleccionados
        const configCheckbox = $(`.${that.checkBoxDownloadSingleInputClassName}:checked`);

        // Habilitar/InHabilitar el botón de "Descarga de selección"
        configCheckbox.length > 0 ? that.btnDownloadConfig.prop('disabled', false) : that.btnDownloadConfig.prop('disabled', true)

        // Guardar los items seleccionados
        configCheckbox.each(function () {
            const configRow = $(this).closest(`.${that.configRowClassName}`);
            that.configRowsSelected = [...that.configRowsSelected, configRow];
        });
    },


    /**
     * Método para marcar/desmarcar todos los items cuando se haga clic en el checkbox de "Todos"
     */
    handleCheckUncheckAllConfigItems: function () {
        const that = this;

        // Controlar si se desean marcar los checkbox de configuraciones que se están visualizando actualmente
        const setCheckboxAsSelected = $(`#${that.checkAllTextsId}`).prop('checked');

        // Desmarcar/Marcar todos los items        
        $(`.${that.checkBoxDownloadSingleInputClassName}`).each(function () {
            // Accede a cada checkbox individualmente
            const checkbox = $(this);
            // Desmarcarlo por defecto
            checkbox.prop('checked', false);

            // Activar sólo aquellas que están visibles
            if (setCheckboxAsSelected) {
                const isContainerVisible = !$(this).closest(`.${that.configListClassName}`).hasClass("d-none");
                isContainerVisible && checkbox.prop('checked', true);
            }
        });

        // Controlar el botón de descarga múltiple
        const configCheckbox = $(`.${that.checkBoxDownloadSingleInputClassName}:checked`);

        if (configCheckbox.length > 0) {
            that.btnDownloadConfig.prop("disabled", false);
        } else {
            that.btnDownloadConfig.prop("disabled", true);
        }
    },

}


/**
 *  Operativa para la subida de configuraciones
 **/
const operativaSubirConfiguracion = {
    /**
    * Inicializar operativa
    */
    init: function (pParams) {
        this.pParams = pParams;
        this.config(pParams);
        this.configEvents();
        this.configRutas();
    },

    configRutas: function () {
        // Url para editar un certificado
        this.urlBase = refineURL();
        // Url para descargar las configuraciones
        this.urlUploadConfig = `${this.urlBase}/upload`;

        // Input file para seleccionar un fichero
        this.inputConfigZipId = "inputConfigZip";
        this.inputConfigZip = $(`#${this.inputConfigZipId}`);
    },

    /*
     * Inicializar elementos de la vista
     * */
    config: function (pParams) {

        // Contenedor modal
        this.modalContainer = $("#modal-container");
        //Checkbox para seleccionar una configuración
        this.configCheckbox = "select-config";
        // Cada una de las filas que muestran las configuraciones en el modal
        this.serviceRowClassName = "service-row";
        // Label donde se muestra el nombre de la configuracion
        this.serviceNameClassName = "service-name";
        // Boton para descargar las configuraciones
        this.btnUploadConfigClassName = 'btnUploadConfig';
        this.btnUploadConfigId = "btnUploadConfig";
        this.btnUploadConfig = $(`#${this.btnUploadConfigId}`);
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


        // Botón para la subida de configuraciones
        configEventByClassName(`${that.btnUploadConfigClassName}`, function (element) {
            const $btnUploadConfig = $(element);
            $btnUploadConfig.off().on("click", function () {
                that.handleUploadConfig();
            });
        });
    },

    /**
     * Método para pasar a backEnd el fichero zip a subir
     */
    handleUploadConfig: function () {
        const that = this;

        // Se recoge el zip del input
        var inputFichero = $('input[name=inputConfigZip]')[0];

        if (inputFichero.files.length > 0) {
            loadingMostrar();

            var dataPost = new FormData();
            dataPost.append("pFicheroZip", inputFichero.files[0]);
            // Envío del zip mediante una petición ajax
            $.ajax({
                url: that.urlUploadConfig,
                type: "POST",
                processData: false,
                contentType: false,
                data: dataPost
            }).done(function (data) {
                mostrarNotificacion("success", data);
            }).fail(function (data) {
                mostrarNotificacion("error", data);
            }).always(function () {
                loadingOcultar();
            });
        }
    },
}


/**
 * Operativa para la gestión de Trazas
 */
const operativaGestionTrazas = {

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

    configRutas: function(){
        // Url para editar un certificado
        this.urlBase = refineURL();
        // Url para cargar el modal para poder activar o añadir una nueva traza a cualquier servicio deseado 
        this.urlLoadModalAddTraza = `${this.urlBase}/load-modal-add-traza`;
        // Url para guardar las trazas
        this.urlSaveServiceTrazas = `${this.urlBase}/save`;


    },

    /*
     * Inicializar elementos de la vista
     * */
    config: function (pParams) {
        
        // Contenedor modal
        this.modalContainer = $("#modal-container");  
        // Modales usados para borrar/desactivar trazas
        this.modalDeleteTrazaClassName = "modal-confirmDelete";

        // Input de búsqueda de trazas
        this.txtBuscarTrazaId = "txtBuscarTraza";
        this.txtBuscarTraza = $(`#${this.txtBuscarTrazaId}`);

        // Contenedor de listado de trazas
        this.trazasListContainerId = "id-added-trazas-list";
        this.trazasListContainer = $(`#${this.trazasListContainerId}`);
        // ListItem de cada traza que se muestra en el listado
        this.trazaRowClassName = "traza-row";
        // Contador que almacenará el nº de trazas activas
        this.numTrazas = $("#numTrazas");
        
        // Botones para eliminación y activación de trazas
        this.btnDeleteTrazasId = "btnDeleteTrazas";
        this.btnDeleteTrazas = $(`#${this.btnDeleteTrazasId}`);
        this.btnActivateTrazaId = "btnActivateTraza";
        this.btnActivateTraza = $(`#${this.btnActivateTrazaId}`);
        // Botón para confirmar la eliminación de todas las trazas
        this.btnConfirmDeleteAllTrazasClassName = "btnConfirmDeleteAllTrazas";
        // Botón para confirmar la eliminación de una única traza
        this.btnConfirmDeleteTrazaClassName = "btnConfirmDeleteTraza";

        // Botón para eliminación individual de la traza
        this.btnDeleteTrazaClassName = "btnDeleteTraza";

        /* Modal para la activación de trazas en servicios */
        // Input para búsqueda del servicio para trazas
        this.txtBuscarServicioId = "txtBuscarServicio";
        // Botón para confirmar la activación de la traza        
        this.btnConfirmActivateTrazasClassName = 'btnConfirmActivateTrazas'; 
        // Inputs de características del servicio a configurar
        this.inputTrazaDurationClassName = "inputTrazaDuration";
        this.inputTrazaCaducidadClassName = "inputTrazaCaducidad";
        // Botón para activar una traza
        this.addTrazaServiceClassName = "addTrazaService";
        // Cada una de las filas que muestran el servicio en el modal
        this.serviceRowClassName = "service-row";
        // Label donde se muestra el nombre del servicio
        this.serviceNameClassName = "service-name";
        // Label donde se muestra el nombre de la traza en el listado de trazas
        this.trazaNameClassName = "component-name";

        // Flags para guardar
        // Fila que almacenará el servicio que se desea eliminar de forma individual
        this.filaService = undefined;
        // Flags para confirmar la eliminación de una traza
        this.confirmDeleteTraza = false;
    },

    /**
     * Lanzar comportamientos u operativas necesarias para el funcionamiento de la sección
     */
    triggerEvents: function(){
        const that = this;  
        that.handleCheckNumberOfTrazas();      
    }, 
    
    /**
     * Método para comprobar el número de trazas existentes
     */
      handleCheckNumberOfTrazas: function(){
        const that = this;

        const numberOfTrazas = $(`.${that.trazaRowClassName}`).length;
        that.numTrazas.html(numberOfTrazas);
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
                
        // Comportamientos del modal que son individuales para el borrado de trazas
        $(`.${this.modalDeleteTrazaClassName}`).on('show.bs.modal', (e) => {
            // Aparición del modal
        })
        .on('hide.bs.modal', (e) => {
            // Acción de ocultar el modal
            // Si el modal se cierra sin confirmar borrado, desactiva el borrado
            if (that.confirmDeleteTraza == false){
                that.handleSetDeleteService(false);                                
            }            
        });         

        // Input para buscar servicios dentro del modal
        configEventById(`${that.txtBuscarTrazaId}`, function(element){
            const $inputBuscarTrazas = $(element);
            $inputBuscarTrazas.off().on("keyup", function(){   
                clearTimeout(that.timer);
                that.timer = setTimeout(function () {                
                    that.handleSearchTrazas($inputBuscarTrazas);                                                         
                }, 500);          
            });	                        
        }); 

        // Botón para crear o activar una nueva traza a servicios
        this.btnActivateTraza.off().on("click", function(){    
            that.handleLoadCreateTraza();
        });

        // Botón para confirmar la activación de trazas en servicios seleccionados desde el modal
        configEventByClassName(`${that.btnConfirmActivateTrazasClassName}`, function(element){
            const $btnConfirmActivateTraza = $(element);
            $btnConfirmActivateTraza.off().on("click", function(){   
                that.handleSaveActivateTrazas();                
            });	                        
        });
        
        // Botón para confirmar la eliminación de todas las trazas
        configEventByClassName(`${that.btnConfirmDeleteAllTrazasClassName}`, function(element){
            const $btnConfirmDeleteAllTrazas = $(element);
            $btnConfirmDeleteAllTrazas.off().on("click", function(){   
                that.handleDeleteAllActiveTrazas();                
            });	                        
        });

        // Botón para confirmar la eliminación de la traza seleccionada
        configEventByClassName(`${that.btnConfirmDeleteTrazaClassName}`, function(element){
            const $btnConfirmDeleteTrazaSelected = $(element);
            $btnConfirmDeleteTrazaSelected.off().on("click", function(){   
                that.handleDeleteTrazaSelected();                
            });	                        
        });        

        // Input para buscar servicios dentro del modal
        configEventById(`${that.txtBuscarServicioId}`, function(element){
            const $inputBuscarServicio = $(element);
            $inputBuscarServicio.off().on("keyup", function(){   
                clearTimeout(that.timer);
                that.timer = setTimeout(function () {                
                    that.handleSearchServices($inputBuscarServicio);                                                         
                }, 500);          
            });	                        
        });  

        // Botón para eliminar una determinada traza
        configEventByClassName(`${that.btnDeleteTrazaClassName}`, function(element){
            const $btnDeleteTraza = $(element);
            $btnDeleteTraza.off().on("click", function(){                   
                // Fila correspondiente al servicio a desactivar                
                that.filaService = $btnDeleteTraza.closest(`.${that.trazaRowClassName}`);
                // Marcar la traza/servicio como "Eliminar" a modo informativo al usuario
                that.handleSetDeleteService(true);
            });	                        
        });        
    },

    /**
     * Método para marcar o desmarcar la traza del servicio asociado como "Eliminado" dependiendo de la elección vía Modal
     * @param {Bool} Valor que indicará si se desea eliminar o no la traza del servicio
     */
    handleSetDeleteService: function(deleteService){
        const that = this;

        if (deleteService){
            // Realizar el "borrado" de la traza                
            // Añadir la clase de "deleted" a la fila del servicio
            that.filaService.addClass("deleted");
        }else{          
            // Eliminar la clase de "deleted" a la fila del servicio
            that.filaService != undefined && that.filaService.removeClass("deleted");
        }
    },

    /** 
     * Método para buscar trazas activas en el listado de trazas
     * @param {JqueryObject} input : Input del que se desea buscar un determinado servicio
     */    
    handleSearchTrazas: function(input){
        const that = this;
        let cadena = input.val();
        // Eliminamos posibles tildes para búsqueda ampliada
        cadena = cadena.normalize('NFD').replace(/[\u0300-\u036f]/g, "").toLowerCase();
        // Cada una de las filas que muestran la traza        
        const trazaRows = $(`.${that.trazaRowClassName}`);        

        // Buscar dentro de cada fila       
        $.each(trazaRows, function(index){
            const trazaRow = $(this);
            // Seleccionamos el nombre de la página y quitamos caracteres extraños, tiles para poder hacer bien la búsqueda            
            const trazaName = $(this).find(`.${that.trazaNameClassName}`).not('.d-none').html().trim().normalize("NFD").replace(/[\u0300-\u036f]/g, "").toLowerCase();
            if (trazaName.includes(cadena)){
                // Mostrar fila resultado
                trazaRow.removeClass("d-none");
            }else{
                // Ocultar fila resultado
                trazaRow.addClass("d-none");
            }            
        });
    },

    /**
     * Método para buscar por servicios dentro del modal para mostrar u ocultar y facilitar su búsqueda al usuario
     * @param {JqueryObject} input : Input del que se desea buscar un determinado servicio
     */
    handleSearchServices: function(input){
        const that = this;
        let cadena = input.val();
        // Eliminamos posibles tildes para búsqueda ampliada
        cadena = cadena.normalize('NFD').replace(/[\u0300-\u036f]/g, "").toLowerCase();
        // Cada una de las filas que muestran la traza        
        const trazaRows = $(`.${that.serviceRowClassName}`);        

        // Buscar dentro de cada fila       
        $.each(trazaRows, function(index){
            const serviceRow = $(this);
            // Seleccionamos el nombre de la página y quitamos caracteres extraños, tiles para poder hacer bien la búsqueda
            const serviceName = $(this).find(`.${that.serviceNameClassName}`).not('.d-none').html().trim().normalize("NFD").replace(/[\u0300-\u036f]/g, "").toLowerCase();
            if (serviceName.includes(cadena)){
                // Mostrar fila resultado
                serviceRow.removeClass("d-none");
            }else{
                // Ocultar fila resultado
                serviceRow.addClass("d-none");
            }            
        });        
    },

    /**
     * Método que recogerá todas las trazas existentes en la comunidad y solicitará su borrado a backEnd
     */
    handleDeleteAllActiveTrazas: function(){
        const that = this;

        // Listado de trazas nuevas a activar
        const trazaList = {};       

        // Los botones indican que las trazas está activas        
        const listActiveTrazas = $(`.${that.btnDeleteTrazaClassName}`);

        // Recorrer los items/checkbox activos para crear el objeto
        $.each(listActiveTrazas, function(index){
            const service = $(this);
            // Construcción de la traza para añadirlo como Array         
            const prefijoClave = `pTrazas[${index}]`;
            trazaList[prefijoClave + '.Nombre'] = service.data("name").trim();
            // Indicar que no se desea activar, sino, desactivar -> Borrar
            trazaList[prefijoClave + '.Activa'] = false;            
        });

        // Realizar la petición para el guardado 
        if (listActiveTrazas.length == 0){
            mostrarNotificacion("info", "No hay trazas activas en la comunidad");
            return;
        }        

        // Método para guardar las trazas (Desactivar todas las trazas existentes)
        that.handleSaveTrazas(trazaList, true, true);   
    },

    /**
     * Método para recoger los datos de la traza que se desea eliminar para posteriormente hacer el guardado en Backend
     */
    handleDeleteTrazaSelected: function(){
        const that = this;

        // Listado de trazas nuevas a activar
        const trazaList = {};       

        // La clase "deleted" de la fila cuál es la traza a eliminar
        const listTrazaToDelete =  $(`.${that.trazaRowClassName}.deleted`).find($(`.${that.btnDeleteTrazaClassName}`));

        // Recorrer los items/checkbox activos para crear el objeto
        $.each(listTrazaToDelete, function(index){
            const service = $(this);
            // Construcción de la traza para añadirlo como Array         
            const prefijoClave = `pTrazas[${index}]`;
            trazaList[prefijoClave + '.Nombre'] = service.data("name").trim();
            // Indicar que no se desea activar, sino, desactivar -> Borrar
            trazaList[prefijoClave + '.Activa'] = false;            
        });

        // Método para guardar las trazas (Desactivar todas las trazas existentes)
        that.handleSaveTrazas(trazaList, true, false);            
    },

    /**
     * Método para obtener las trazas seleccionadas con los parámetros establecidos desde el modal y realizar el guardado o activación de
     * las mismas mediante petición a backEnd
     */
    handleSaveActivateTrazas: function(){
        const that = this;

        // Recoger los datos y las trazas a guardar
        const trazaDuration = $(`#${that.inputTrazaDurationClassName}`).val().trim();        
        const trazaCaducidad = $(`#${that.inputTrazaCaducidadClassName}`).val().trim();
        // Listado de trazas nuevas a activar
        const trazaList = {};        

        // Listado de servicios seleccionados para establecer las trazas
        const listServiceTrazas = $(`input[type="checkbox"].${that.addTrazaServiceClassName}:checked`);

        // Recorrer los items/checkbox activos para crear el objeto
        $.each(listServiceTrazas, function(index){
            const service = $(this);
            // Construcción de la traza para añadirlo como Array         
            const prefijoClave = `pTrazas[${index}]`;
            trazaList[prefijoClave + '.Nombre'] = service.data("name").trim();
            trazaList[prefijoClave + '.Duracion'] = trazaDuration;
            trazaList[prefijoClave + '.Caducidad'] = trazaCaducidad;
            trazaList[prefijoClave + '.Activa'] = service.is(':checked');            
        });

        // Realizar la petición para el guardado 
        if (listServiceTrazas.length == 0){
            mostrarNotificacion("info", "Indica al menos un servicio del que deseas activar sus trazas");
            return;
        }
        // Método para guardar las trazas (Activar)
        that.handleSaveTrazas(trazaList, false, false);                 
    },


    /**
     * Método para guardar las trazas ya sea para borrar o eliminar en BackEnd
     * @param {*} trazaList Lista de trazas a enviar tanto para guardar como para eliminar
     * @param {*} deleteTrazas Booleano que indica si se desean añadir o eliminar trazas. Se realiza para controlar el mensaje de success o error en la petición
     * @param {*} deleteAllTrazas Booleano que indica si se ha procedido a borrar todas las trazas
     */
    handleSaveTrazas: function(trazaList, deleteTrazas, deleteAllTrazas = false){
        const that = this;

        // Mostrado de loading                   
        loadingMostrar();

        // Realizar la petición para el guardado        
        GnossPeticionAjax(
            that.urlSaveServiceTrazas,
            trazaList,
            true
        ).done(function (data) {
            // OK
            // Resetear el estado de borrado de la traza seleccionada (por defecto)
            that.confirmDeleteTraza = false;            

            // Cargar los nuevos datos en el contenedor de trazas
            that.trazasListContainer.html(data);

            // Cerrar modal en concreto dependiendo de la acción
            deleteTrazas == false
            ? dismissVistaModal() 
            : dismissVistaModal($(".modal-confirmDelete.show"));

            setTimeout(function() {                                                                 
                if (deleteTrazas == false){
                    // Trazas añadidas   
                    mostrarNotificacion("success", "Las trazas se han activado correctamente.");   
                }else{
                    // Trazas eliminadas    
                    mostrarNotificacion("success", "Las trazas se han desactivado correctamente.");               
                    if (deleteAllTrazas == false){
                        // Eliminar la traza seleccionada
                        that.filaService.remove();
                    }else{
                        // Eliminar todas las trazas
                        $(`.${that.trazaRowClassName}`).remove();                        
                    }                                       
                }

                // Actualizar contador de trazas
                that.handleCheckNumberOfTrazas();                             
            }
            ,1000);
        }).fail(function (data) {            
            mostrarNotificacion("error", data);
        }).always(function () {
            // Ocultar el loading
            loadingOcultar();
        });         
    },
    

    /**
     * Método para realizar la llamada para mostrar el modal de activación de una traza.
     */
    handleLoadCreateTraza: function(){
        const that = this;

        // Cargar la vista para  crear o activar una nueva traza
        getVistaFromUrl(that.urlLoadModalAddTraza, 'modal-dinamic-content', '', function(result){
            if (result != requestFinishResult.ok){
                // KO al cargar la vista
                mostrarNotificacion("error", "Error al tratar de crear nuevas trazas. Por favor, inténtalo de nuevo más tarde.");
                dismissVistaModal();                                                     
            }        
        });
    },
}


/**
  * Operativa para la gestión de las cláusulas legales y de registro de la Comunidad
  */
const operativaGestionClausulasLegales = {
    
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
        };  

        // Inicializar operativa multiIdioma
        operativaMultiIdioma.init(this.operativaMultiIdiomaParams);
        
        // Contar el nº de cláusulas
        that.handleCheckNumberOfClausulas();

        setupBasicSortableList(that.clauseListContainerId, that.sortableClauseIconClassName);        
    },    

    /**
     * Configuración de las rutas de acciones necesarias para hacer peticiones a backend
     */
     configRutas: function (pParams) {
        // Url para editar un certificado
        this.urlBase = refineURL();
        this.urlSave = `${this.urlBase}/save`;
        
        // Url que se usará para editar una determinada cláusula
        this.urlEditClausula = '';        
        // Url para crear la cláusula (Llamada al modal)
        this.urlCreateClausula = '';
        // Tipo de cláusula a crear
        this.clauseType = '';
        // Fila que está siendo editada
        this.filaClausula = '';
        // Nueva cláusula creada y flag de si se ha guardado o no.
        this.newClause = undefined;
        this.clausulaSaved = false;
        // Objeto donde se guardarán las cláusulas para su posterior guardado
        this.ListaClausulas = {};
        // Flag de comprobación de errores de cláusulas antes de guardar
        this.errorsBeforeSaving = false;
        // Flag que indica si se está borrando una determinada Cláusula        
        this.confirmDeleteClause = false;
    }, 
    
    /*
     * Inicializar elementos de la vista
     * */
    config: function (pParams) {

        // Contenedor modal
        this.modalContainer = $("#modal-container");
        // Identificador del modal para editar/crear nueva cláusula
        this.modalClauseClassName = "modal-clause";
        // Identificador del modal para eliminar una cláusula
        this.modalDeleteClauseClassName = "modal-confirmDelete";        

        // Tab de idioma de la página para cambiar la visualización en el idioma deseado de las páginas
        this.tabIdiomaItem = $(".tabIdiomaItem ");        
        // Cada una de las labels (Nombre literal) en los diferentes idiomas (por `data-languageitem`)
        this.labelLanguageComponent = $(".language-component");
        // Cada una de las filas donde se encuentran las traducciones
        this.clausulaListItemClassName = 'clausula-row';
        // Contador del nº de cláusulas existentes
        this.numResultados = $('.numResultados');
        // Botón para añadir una cláusula
        this.btnAddClausula = $('.btnAddClauseItem');
        // Botón para confirmar borrado de cláusula desde modal
        this.btnConfirmDeleteClausulaClassName = 'btnConfirmDeleteClausula';        
        this.btnConfirmDeleteClausula = `${this.btnConfirmDeleteClausulaClassName}`;  
        
        // Botón para rechazar la eliminación de una cláusula desde el modal (No)
        this.btnNotConfirmDeleteClausulaClassName = 'btnNotConfirmDeleteClausula';
        this.btnNotConfirmDeleteClausula = `${this.btnNotConfirmDeleteClausulaClassName}`;

        // Botón para editar una cláusula existente
        this.btnEditClausulaClassName = 'btnEditClausula';
        this.btnEditClausula = $(`.${this.btnEditClausulaClassName}`);
        // Botón de guardar cláusula dentro del modal
        this.btnGuardarClausulaClassName = 'btnGuardarClausula';        
        this.btnGuardarClausula = $(`.${this.btnGuardarClausulaClassName}`);

        // Contenedor de todas las cláusulas de la comunidad
        this.clauseListContainerId = 'id-added-clauses-list';
        this.clauseListContainer = $(`#${this.clauseListContainerId}`);
        // Icono para arrastar una fila
        this.sortableClauseIconClassName = 'js-component-sortable-clause';

        // Botón para eliminar una cláusula        
        this.btnDeleteClausulaClassName = 'btnDeleteClausula';
        this.btnDeleteClausula = $(`.${this.btnDeleteClausulaClassName}`);

        // Input para realizar búsquedas
        this.txtBuscarClausula = $("#txtBuscarClausula");
        // Input de la lupa para realizar búsquedas
        this.inputLupa = $("#inputLupa");        
    },   
    
    /**
     * Configuración de eventos de elementos del Dom (Botones, Inputs...)     
     */
     configEvents: function (pParams) {
        const that = this;
              
        // Modal container
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

        // Comportamientos del modal que son individuales para la edición de cláusulas   
        configEventByClassName(`${that.modalClauseClassName}`, function(element){
            const $modalClause = $(element);
            $modalClause
            .on('show.bs.modal', (e) => {
                // Aparición del modal
                that.triggerModalContainer = $(e.relatedTarget);
            })
            .on('hide.bs.modal', (e) => {
                // Acción de ocultar el modal            
                const currentModal = $(e.currentTarget);                                
                that.handleCloseClauseModal(currentModal);
            }); 
        });
        
        // Comportamientos del modal que de borrado de cláusulas   
        configEventByClassName(`${that.modalDeleteClauseClassName}`, function(element){
            const $modalClause = $(element);
            $modalClause
            .on('show.bs.modal', (e) => {
                // Aparición del modal
                that.triggerModalContainer = $(e.relatedTarget);
            })
            .on('hide.bs.modal', (e) => {
                // Acción de ocultar el modal
                if (that.confirmDeleteClause == false){
                    that.handleSetDeleteClause(false);                
                }               
            }); 
        });         

        // Botón para crear una nueva traducción
        this.btnAddClausula.off().on("click", function(){
            // Obtener la url a crear para cargar el modal
            that.urlCreateClausula = $(this).data("urlcreateclause");
            // Obtener el tipo de cláusula a crear
            that.clauseType = $(this).data("type");            
            that.handleLoadCreateClause();
        });

        // Tab de cada uno de los idiomas disponibles para cambiar la visualización en la lista de cláusulas
        this.tabIdiomaItem.off().on("click", function(){
            that.handleViewClausulaLanguageInfo();                        
            // Aplicar la búsqueda para el idioma activo
            that.timer = setTimeout(function () {                                                                                                        
                that.handleSearchClausulaItem(that.txtBuscarClausula);
            }, 500);            
        });

        // Botón para editar una cláusula        
        configEventByClassName(`${that.btnEditClausulaClassName}`, function(element){
            const $editClausula = $(element);
            $editClausula.off().on("click", function(){   
                // Guardar la fila activa seleccionada
                that.filaClausula = $editClausula.closest('.clausula-row');
            });	                        
        });
        
        // Botón para eliminar una cláusula        
        configEventByClassName(`${that.btnDeleteClausulaClassName}`, function(element){
            const $removeClause = $(element);
            $removeClause.off().on("click", function(){   
                // Guardar la fila activa seleccionada
                that.filaClausula = $removeClause.closest('.clausula-row');
                // Marcamos la cláusula como para eliminar
                that.handleSetDeleteClause(true);                                            
            });	                        
        }); 

        // Botón para confirmar la eliminación de una cláusula desde el modal              
        configEventByClassName(`${that.btnConfirmDeleteClausulaClassName}`, function(element){
            const $confirmRemoveClause = $(element);
            $confirmRemoveClause.off().on("click", function(){   
                // Confirmamos la eliminación de la cláusula
                that.confirmDeleteClause = true;
                // Procedemos al borrado/guardado
                that.handleConfirmDeleteClause();
            });	                        
        }); 
                
        // Búsquedas de traducciones
        this.txtBuscarClausula.off().on("keyup", function (event) {
            const input = $(this);
            clearTimeout(that.timer);
            that.timer = setTimeout(function () {                                                                        
                // Acción de buscar / filtrar cláusula
                that.handleSearchClausulaItem(input);                                         
            }, 500);
        });  
                
        // Botón de guardar cláusula del modal
        configEventByClassName(`${that.btnGuardarClausulaClassName}`, function(element){
            const $saveButton = $(element);
            $saveButton.off().on("click", function(){ 
                that.handleSaveClauses();                
            });	                        
        }); 
    },

    /**
     * Método para marcar o desmarcar la cláusula como "Eliminada" dependiendo de la elección vía Modal
     * @param {Bool} deleteClause Valor que indicará si se desea eliminar o no la cláusula
     */
     handleSetDeleteClause: function(deleteClause){
        const that = this;

        if (deleteClause){
            // Realizar el "borrado" de la página
            // 1 - Marcar la opción "TabEliminada" a true
            that.filaClausula.find('[name="TabEliminada"]').val("true");           
            // 2 - Añadir la clase de "deleted" a la fila de la página
            that.filaClausula.addClass("deleted");
        }else{
            // 1 - Marcar la opción "TabEliminada" a false
            that.filaClausula.find('[name="TabEliminada"]').val("false");            
            // 2 - Eliminar la clase de "deleted" a la fila de la página
            that.filaClausula.removeClass("deleted");
        }
    },
    
    /**
     * Método para guardar las cláusulas una vez se ha procedido a confirmar la eliminación de la deseada
     */
    handleConfirmDeleteClause: function(){
        const that = this;
        that.handleSaveClauses(function(result){
            if (result == requestFinishResult.ok){
                // 2 - Ocultar el modal de eliminación de la cláusula
                const modalDeleteClause = $(that.filaClausula).find(`.${that.modalDeleteClauseClassName}`);                                          
                dismissVistaModal(modalDeleteClause);
                // 3 - Ocultar loading
                loadingOcultar();                
                // 4 - Ocultar la fila que se desea eliminar
                that.filaClausula.addClass("d-none");
                // 5 - Eliminar la fila que se acaba de eliminar                                
                that.filaClausula.remove();
                // 6 - Actualizar el contador de nº de cláusulas                            
                that.handleCheckNumberOfClausulas();
                // Restablecer el flag de borrado de la fila de la cláusula
                that.confirmDeleteClause = false;
            }else{
                mostrarNotificacion("error", "Se ha producido un error al tratar de eliminar la cláusula. Contacta con el administrador de la comunidad.");
            }             
        });    
    }, 


    /**
     * Método que se ejecutará cuando se cierre el modal de detalles de una clausula
     * @param {jqueryObject} currentModal Modal que se va a cerrar
     */
     handleCloseClauseModal: function(currentModal){
        const that = this;

        // Tener en cuenta si la cláusula es de reciente creación y por tanto no se desea guardar
        if (that.filaClausula.hasClass("newClause")){
            // Eliminar la fila que se acaba de eliminar                                
            that.filaClausula.remove();
            // Actualizar el contador de items
            that.handleCheckNumberOfClausulas();
        }
    },
    
    /**
     * Método para guardar las cláusulas existentes. Este método se ejecuta cuando se pulsa en el modal de "Guardar"
     * @param {*} completion : Función a ejecutar cuando se realice o complete este método.
     */
    handleSaveClauses: function(completion = undefined){
        const that = this;

        // Resetear el flag de control de errores de guardado para iniciar su comprobación
        that.errorsBeforeSaving = false;

        // Comprobar errores de guardado antes de enviar a guardar
        that.checkErrorsBeforeSaving();
        
        // Si todo ha ido bien, continuar con la recogida de datos
        if (that.errorsBeforeSaving == false) {
            that.ListaClausulas = {};
            let cont = 0;
            // Recorrer todas las cláusulas para comprobación de los datos introducidos antes de su guardado
            $(`.${that.clausulaListItemClassName}`).each(function () {
                that.getClauseData($(this), cont++);
            });
            // Envío de petición para el guardado de cláusulas a backend 
            that.handleSave(function(result, data){
                if (result == requestFinishResult.ok){
                    // OK guardado                                                                               
                    mostrarNotificacion("success","Los cambios se han guardado correctamente");
                    // Se desean guardar las cláusulas -> Quitar clase de "newClause" para NO eliminar la cláusula al cerrar el modal
                    $(".newClause").removeClass("newClause");                                                        
                    // Cerrar el modal y recargar la página para recargar los cambios sólo en edición o creación
                    if (that.confirmDeleteClause == false){
                        setTimeout(function() {                                  
                            // Modal para cerrar                
                            const modalPage = $(that.filaClausula).find(`.${that.modalClauseClassName}`);                                          
                            dismissVistaModal(modalPage);                            
                            location.reload();
                        }
                        ,1000);
                    }else{
                        // Se está borrando alguna cláusula
                        completion(result);                        
                    }          
                }else{
                    // KO guardado
                    const error = data.split('|||');
                    mostrarNotificacion("error", "Ha habido errores en el guardado de las cláusulas. Por favor revisa los errores marcados");
                    if (error[0] == "NOMBRE VACIO") {
                        that.mostrarErrorNombreVacio( that.filaClausula, $('#' + error[1]));
                    }
                }
            });
        }
        else {   
            // Error en datos a enviar a backend         
            mostrarNotificacion("error", "Hay información pendiente de ser rellenada en la cláusula antes de poder ser guardada.");
        }
    },

    /**
     * Método para enviar datos a backend para su guardado una vez se ha comprobado que no hay errores y se han recogido todos los datos
     * @param {function} completion Función anónima que se ejecutará cuando se realice la petición de guardado
     */
    handleSave: function(completion){
        const that = this;

        // Mostrar loading
        loadingMostrar();

        // Realizar la petición para el guardado        
        GnossPeticionAjax(
            that.urlSave,
            that.ListaClausulas,
            true
        ).done(function (data) {
            // OK Datos guardados correctamente
            completion( requestFinishResult.ok, data);            
        }).fail(function (data) {
            completion( requestFinishResult.ko, data); 
        }).always(function () {
            // Ocultar el loading
            loadingOcultar();
        });        

    },


    /**
     * Método para obtener los datos de cada cláusula para construir el objeto que será enviado a backend para su posterior guardado 
     * @param {jqueryElement} rowClause Fila que contiene la cláusula que será analizada
     * @param {*} cont : Contador para construir el objeto a enviar a backend
     */
    getClauseData: function(rowClause, cont){        
        const that = this;

        // Contenido o datos de la cláusula dentro del modal
        const panelEdicion = rowClause.find(`.${that.modalClauseClassName}`);
        // Id de la cláusula
        const id = rowClause.attr('id');
        // Obtener el tipo de cláusula a guardar
        const TipoClausula = panelEdicion.find('[name="TipoClausula"]').val();
        // Prefijo para creació del objeto y envío a backEnd
        const prefijoClave = 'ListaClausulas[' + cont + ']';

        // Obtener información de Key, Deleted, Orden y Type
        that.ListaClausulas[prefijoClave + '.Key'] = id;
        that.ListaClausulas[prefijoClave + '.Deleted'] = panelEdicion.find('[name="TabEliminada"]').val();
        that.ListaClausulas[prefijoClave + '.Orden'] = cont;
        that.ListaClausulas[prefijoClave + '.Type'] = TipoClausula;

        // Título de la cláusula. Por defecto vacía porque algunas no disponen de título
        that.ListaClausulas[prefijoClave + '.Title'] = ''
        if (TipoClausula != 'Opcional') {
            that.ListaClausulas[prefijoClave + '.Title'] = panelEdicion.find('[name="Title"]').val();
        }
        // Texto2 de la cláusula. Por defecto vacía porque algunas no disponen de Text2
        that.ListaClausulas[prefijoClave + '.Text2'] = ''
        if (TipoClausula == 'PoliticaCookiesUrlPagina')
        {
            that.ListaClausulas[prefijoClave + '.Text1'] = encodeURIComponent(panelEdicion.find('[name="TextoCabeceraClausula"]').val());
            that.ListaClausulas[prefijoClave + '.Text2'] = encodeURIComponent(panelEdicion.find('[name="TextoPaginaClausula"]').val());
            that.ListaClausulas[prefijoClave + '.CookieName'] = encodeURIComponent(panelEdicion.find('[name="TextoNombreCookie"]').val());
        }
        else {
            that.ListaClausulas[prefijoClave + '.Text1'] = encodeURIComponent(panelEdicion.find('[name="TextoClausula"]').val());
        }
    
    }, 

    /**
     * Método para comprobar que no se encuentra ningún error antes de realizar el guardado de las cláusulas.
     * El resultado de esas comprobaciones se guarda en 'errorsBeforeSaving'
     */
    checkErrorsBeforeSaving: function(){
        const that = this;
        
        // Flag para control de errores
        that.errorsBeforeSaving = false;                
        
        /* Comprobar nombres vacíos */
        that.comprobarNombresVacios()             
        
        if (that.errorsBeforeSaving == false){
            that.comprobarContenidosVacios()
        }        
    },

    /**
     * Método para comprobar que los nombres cortos/textos de las cláusulas no esté vacío.
     */
     comprobarNombresVacios: function(){
        const that = this;

        let inputsNombre = $(`.${that.clausulaListItemClassName} input[name="Title"]:not(":disabled")`);        
        // Comprobación de los nombres
        inputsNombre.each(function () {
            if (that.errorsBeforeSaving == false){
                that.comprobarNombreVacio($(this));
            }
        });        
    },

    /**
     * Método para comprobar que el nombre corto y ó texto de la cláusula no esté vacío. Habrá que tener en cuenta el multiIdioma
     * @param {*} inputName: Input a comprobar     
     */
     comprobarNombreVacio: function (inputName) {
        const that = this
        
        //const panMultiIdioma = $('#edicion_multiidioma', inputName.parent());
        // Panel multiIdioma donde se encuentra el input
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
            
            if (textoIdiomaDefecto == null || textoIdiomaDefecto == "") {
                const fila = inputName.closest(".component-wrap.clausula-row");
                that.mostrarErrorNombreVacio(fila, panMultiIdioma.find(`#input_${inputId}_${operativaMultiIdioma.idiomaPorDefecto}`));
                that.errorsBeforeSaving = true;
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
    },

    /**
     * Método para mostrar el error del nombre vacío encontrado en una cláusula. Se muestra cuando el nombre de una cláusula en el idioma por defecto no existe.
     * @param {jqueryObject} fila : Elemento jquery de la fila correspondiente a donde se ha encontrado el error.
     * @param {jqueryObject} input : Elemento jquery del input donde se ha producido el error. Puede que ser undefined
     */
     mostrarErrorNombreVacio: function(fila, input){        
                      
        const btnEditClausula = $(".btnEditClausula", fila);
        // Abrir el modal para acceder a modificar el nombre de la página
        // btnEditClausula.trigger( "click" );

        if (input != undefined){
            // No mostrar al poder ser multiIdioma
            // comprobarInputNoVacio(input, true, false, "Esta información de la cláusula no puede estar vacío.", 0);
        }

        /*setTimeout(function(){  
            mostrarNotificacion("error", "Esta información de la cláusula no puede estar vacío.");
        },1000);  
        */
    },    

    /**
     * Método para comprobar que el contenido de las cláusulas no está vacío (TextArea...)
     */
    comprobarContenidosVacios: function () {
        const that = this;        

        let inputsNombre = $(`.${that.clausulaListItemClassName} textarea[name="TextoClausula"]:not(":disabled")`);
        // Comprobación del TextoClausula
        inputsNombre.each(function () {
            if (that.errorsBeforeSaving == false){
                that.comprobarNombreVacio($(this));
            }
        });
        inputsNombre = $(`.${that.clausulaListItemClassName} textarea[name="TextoCabeceraClausula"]:not(":disabled")`);
        // Comprobación del TextoCabeceraCláusula
        inputsNombre.each(function () {
            if (that.errorsBeforeSaving == false){
                that.comprobarNombreVacio($(this));
            }
        });

        inputsNombre = $(`.${that.clausulaListItemClassName} textarea[name="TextoPaginaClausula"]:not(":disabled")`);
        // Comprobación del TextoPaginaClausula
        inputsNombre.each(function () {
            if (that.errorsBeforeSaving == false){
                that.comprobarNombreVacio($(this));
            }
        });                    
    },
 
    /**
     * Método para cargar el modal para crear una nueva cláusula
     */
    handleLoadCreateClause: function(){
        const that = this;
       
        // Mostrar loading hasta que finalice la petición para crear una nueva cláusula
        loadingMostrar();   

        // Construir el objeto/modelo para petición del tipo de la cláusula a crear
        const dataPost = {
            TipoClausula: parseInt(that.clauseType),
        }

        // Realizar la petición de la cláusula a crear
        GnossPeticionAjax(                
            that.urlCreateClausula,
            dataPost,
            true
        ).done(function (data) {
            // Devuelve la fila/row de la cláusula a crear
            const lastClausePage = that.clauseListContainer.children().last();
            // Panel de edición de la última cláusula
            const editionLastClausePanel = lastClausePage.children(".modal-con-tabs");           
            // Añadir la nueva fila al listado de cláusulas
            that.clauseListContainer.append(data);                        
            // Referencia a la nueva cláusula añadida
            that.newClause = that.clauseListContainer.children().last();
            // Añadirle el flag de "nueva cláusula" para saber que es de reciente creación.
            that.newClause.addClass("newClause");   
            // Establecer como fila seleccionada la nueva creada
            that.filaClausula = that.newClause;
            // Panel de edición de la nueva cláusula creada
            const editionNewClausePanel = that.newClause.children(".modal-con-tabs");                                
            // Buscar nuevos items para asignarles comportamientos
            operativaGestionClausulasLegales.config();
            // Disparar eventos necesarios para nuevos items (Ej: multiIdioma)
            operativaGestionClausulasLegales.triggerEvents();
            // Abrir el modal para poder editar/gestionar la nueva página añadida                     
            that.newClause.find(that.btnEditClausula).trigger("click");                               
            
        }).fail(function (data) {
            // Mostrar error al tratar de crear una nueva página
            mostrarNotificacion("error", data);                
        }).always(function () {
            // Ocultar loading de la petición
            loadingOcultar();
        });        
    },

    /**
     * Método para cambiar la visualización al idioma de la cláusula deseada
     */
     handleViewClausulaLanguageInfo: function(){

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
     * Método que se ejecutará al cargarse la web para saber el nº de traducciones existentes
     */
    handleCheckNumberOfClausulas: function(){
        const that = this;
        const numberClausulas = $(`.${that.clausulaListItemClassName}`).length;
        that.numResultados.html(numberClausulas);
    },
    
    /**
     * Método para buscar una cláusula independientemente de su idioma
     * @param {jqueryElement} input : Input desde el que se ha realizado la búsqueda
     */
     handleSearchClausulaItem: function(input){
        const that = this;

        // Cadena introducida para filtrar/buscar componentes
        let cadena = input.val();
        // Eliminamos posibles tildes para búsqueda ampliada
        cadena = cadena.normalize('NFD').replace(/[\u0300-\u036f]/g, "").toLowerCase();

        // Listado de componentes donde se buscará
        const clausulaItems = $(`.${that.clausulaListItemClassName}`);

        // Recorrer los items/compontes para realizar la búsqueda de palabras clave
        $.each(clausulaItems, function(index){
            const clausulaItem = $(this);
            // Seleccionar el nombre sólo "visible" de la traducción. Quitamos caracteres extraños, tildes para poder hacer bien la búsqueda
            const componentContenido = clausulaItem.find(".component-contenido").not('.d-none').html().trim().normalize("NFD").replace(/[\u0300-\u036f]/g, "").toLowerCase();
            const componentName = clausulaItem.find(".component-name").html().trim().normalize("NFD").replace(/[\u0300-\u036f]/g, "").toLowerCase();

            if (componentContenido.includes(cadena) || componentName.includes(cadena)){
                // Mostrar la fila
                clausulaItem.removeClass("d-none");                
            }else{
                // Ocultar la fila
                clausulaItem.addClass("d-none");
            }            
        });                        
    },  

}

/**
  * Operativa para la gestión de las cookies de la Comunidad y sus categorías
  */
const operativaGestionCookies = {

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
        };  

        // Inicializar operativa multiIdioma
        operativaMultiIdioma.init(this.operativaMultiIdiomaParams);        
        // Inicializar comportamientos de select2
        comportamientoInicial.iniciarSelects2(); 

        // Contabilizar el nº de cookies existentes
        this.handleCheckNumberOfCookies();

        // Configurar arrastrar cookies                            
        // Cookies
        setupBasicSortableList(that.cookieListContainerId, that.sortableCookieIconClassName);        
        // Categoría de cookies        
        setupBasicSortableList(that.cookieCategoryListContainerId, that.sortableCookieIconClassName);        
    },   
    
    /**
     * Configuración de las rutas de acciones necesarias para hacer peticiones a backend
     */
     configRutas: function (pParams) {
        // Url para editar un certificado
        this.urlBase = refineURL(); 
        this.urlSaveCategories = `${this.urlBase}/save-category`;
        this.urlSaveCookies = `${this.urlBase}/save-cookie`;

        // Filas que está siendo editada
        this.filaCookie = '';
        this.filaCategoriaCookie = '';
        // Url de creación de una cookie
        this.urlAddCookie = '';
        // Tipo de cookie a crear
        this.cookieType = '';
        // Control de errores. Inicio a false
        this.errorsBeforeSaving = false;
        // Flag que indica si se está borrando una determinada cookie
        this.confirmDeleteCookie = false;

        // Objeto de las categorías y cookies a guardar en backend
        this.ListaCategorias = {};
        this.ListaCookies = {};
    },  
    
    /*
     * Inicializar elementos de la vista
     * */
    config: function (pParams) {

        // Contenedor modal
        this.modalContainer = $("#modal-container");
        // Identificador del modal para editar/crear nueva cookie
        this.modalCookieClassName = "modal-cookie";
        // Identificador del modal para eliminar una cookie
        this.modalDeleteCookieClassName = "modal-confirmDelete"; 
        
        // Botón para visualizar/ocultar las Categorías de cookiesCategoryContainer
        this.btnVerCategorias = $("#btnVerCategorias");

        // Tab de idioma de la página para cambiar la visualización en el idioma deseado de las páginas
        this.tabIdiomaItem = $(".tabIdiomaItem ");        
        // Cada una de las labels (Nombre literal) en los diferentes idiomas (por `data-languageitem`)
        this.labelLanguageComponent = $(".language-component");
        // Cada una de las filas donde se encuentran las cookies
        this.cookieListItemClassName = 'cookie-row';
        // Cada una de las filas donde se encuentran las categorías de las cookies
        this.categoryCookieListItemClassName = 'cookie-row';        
        // Contador del nº de cookies existentes
        this.numResultadosCookies = $('#numCookies');
        // Contador de nº de categorias cookies
        this.numResultadosCategoryCookies = $('#numCategoryCookies');
        
        // Botón para confirmar borrado de una cookie desde modal
        this.btnConfirmDeleteCookieClassName = 'btnConfirmDeleteCookie';        
        this.btnConfirmDeleteCookie = `${this.btnConfirmDeleteCookieClassName}`;  
        // Botón para rechazar la eliminación de una cookie desde el modal (No)
        this.btnNotConfirmDeleteCookieClassName = 'btnNotConfirmDeleteCookie';
        this.btnNotConfirmDeleteCookie = `${this.btnNotConfirmDeleteCookieClassName}`;
        // Botón para confirmar borrado de una categoria cookie desde modal
        this.btnConfirmDeleteCategoryCookieClassName = 'btnConfirmDeleteCategoryCookie';        
        this.btnConfirmDeleteCategoryCookie = `${this.btnConfirmDeleteCategoryCookieClassName}`;  
        // Botón para rechazar la eliminación de una categoria cookie desde el modal (No)
        this.btnNotConfirmDeleteCategoryCookieClassName = 'btnNotConfirmDeleteCategoryCookie';
        this.btnNotConfirmDeleteCategoryCookie = `${this.btnNotConfirmDeleteCategoryCookieClassName}`;

        // Botón de guardar cookie dentro del modal
        this.btnGuardarCookieClassName = 'btnGuardarCookie';        
        this.btnGuardarCookie = $(`.${this.btnGuardarCookieClassName}`);

        // Botón para poder guardar de golpe las cookies añadidas de youtube y Google
        this.btnGuardarCookiesYoutubeGoogle = $(".btnGuardarCookiesYoutubeGoogle");

        // botón para guardar la categoría de la cookie del modal        
        this.btnGuardarCategoryCookieClassName = 'btnGuardarCategoryCookie';        
        this.btnGuardarCategoryCookie = $(`.${this.btnGuardarCookieClassName}`);

        // Contenedor de todas las cookies de la comunidad
        this.cookieListContainerId = 'id-added-cookies-list';
        this.cookieListContainer = $(`#${this.cookieListContainerId}`);
        // Icono para arrastar una fila
        this.sortableCookieIconClassName = 'js-component-sortable-cookie';

        // Contenedor global de las categorías cookies.
        this.cookieCategoryContainer = $("#cookiesCategoryContainer");
        // Contenedor de todas las categorías cookies de la comunidad
        this.cookieCategoryListContainerId = 'id-added-category-cookies-list';
        this.cookieCategoryListContainer = $(`#${this.cookieCategoryListContainerId}`);

        // Botón para eliminar una cookie        
        this.btnDeleteCookieClassName = 'btnDeleteCookie';
        this.btnDeleteCookie = $(`.${this.btnDeleteCookieClassName}`);
        // Botón para editar una cookie existente
        this.btnEditCookieClassName = 'btnEditCookie';
        this.btnEditCookie = $(`.${this.btnEditCookieClassName}`);

        // Botón para eliminar una categoria cookie        
        this.btnDeleteCategoryCookieClassName = 'btnDeleteCategoryCookie';
        this.btnDeleteCategoryCookie = $(`.${this.btnDeleteCookieClassName}`);
        // Botón para editar una categoria cookie existente
        this.btnEditCategoryCookieClassName = 'btnEditCategoryCookie';
        this.btnEditCategoryCookie = $(`.${this.btnEditCategoryCookieClassName}`);
        

        // Botón para añadir cookies (analíticas, técnicas...)
        this.linkAddNewCookie = $(".linkAddNewCookie");
        // Botón para añadir una category cookie
        this.linkAddCategory = $(".linkAddCategory");
        // Botón para añadir cookie analitica de Google
        this.linkAddGoogleCookieAnalytics = $(".linkAddGoogleCookieAnalytics");
        // Botón para añadir cookie analitica de Youtube
        this.linkAddYoutubeCookie = $(".linkAddYoutubeCookie");

        // Input para realizar búsquedas
        this.txtBuscarCookie = $("#txtBuscarCookie");
        // Input de la lupa para realizar búsquedas
        this.inputLupa = $("#inputLupa");        
    },   
    
    /**
     * Configuración de eventos de elementos del Dom (Botones, Inputs...)     
     */
     configEvents: function (pParams) {
        const that = this;
              
        // Modal container
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

        // Comportamientos del modal que son individuales para la edición de Cookies        
        configEventByClassName(`${that.modalCookieClassName}`, function(element){
            const $modalClause = $(element);
            $modalClause
            .on('show.bs.modal', (e) => {
                // Aparición del modal
                that.triggerModalContainer = $(e.relatedTarget);
            })
            .on('hide.bs.modal', (e) => {
                // Acción de ocultar el modal            
                const currentModal = $(e.currentTarget);                                
                that.handleCloseCookieModal(currentModal);
            }); 
        });

        
        // Comportamientos del modal que de borrado de cookies   
        configEventByClassName(`${that.modalDeleteCookieClassName}`, function(element){
            const $modalClause = $(element);
            $modalClause
            .on('show.bs.modal', (e) => {
                // Aparición del modal
                that.triggerModalContainer = $(e.relatedTarget);
            })
            .on('hide.bs.modal', (e) => {
                // Acción de ocultar el modal
                if (that.confirmDeleteCookie == false){
                    that.handleSetDeleteCookie(false);                
                }               
            }); 
        });
                 
        // Botón para editar una cookie   
        configEventByClassName(`${that.btnEditCookieClassName}`, function(element){
            const $editCookie = $(element);
            $editCookie.off().on("click", function(){   
                // Guardar la fila activa seleccionada
                that.filaCookie = $editCookie.closest('.cookie-row');
                // Activamos el flag de activación de la Cookie modificada
                that.filaCookie.find('[name="CookieModificada"]').val("True");
            });	                        
        });

        // Botón para editar una categoría de cookie
        configEventByClassName(`${that.btnEditCategoryCookieClassName}`, function(element){
            const $editCookie = $(element);
            $editCookie.off().on("click", function(){   
                // Guardar la fila activa seleccionada
                that.filaCookie = $editCookie.closest('.cookie-row');
                // Activamos el flag de activación de la Categoria modificada
                that.filaCookie.find('[name="CategoriaModificada"]').val("True");
                
            });	                        
        });        

        // Botón para confirmar la eliminación de una cookie desde el modal              
        configEventByClassName(`${that.btnConfirmDeleteCookieClassName}`, function(element){
            const $confirmRemoveCookie = $(element);
            $confirmRemoveCookie.off().on("click", function(){   
                // Confirmamos la eliminación de la cláusula
                that.confirmDeleteCookie = true;
                // Procedemos al borrado/guardado
                that.handleConfirmDeleteCookie();
            });	                        
        }); 

        // Botón para confirmar la eliminación de una categoria cookie desde el modal
        configEventByClassName(`${that.btnConfirmDeleteCategoryCookieClassName}`, function(element){
            const $confirmRemoveCategoryCookie = $(element);
            $confirmRemoveCategoryCookie.off().on("click", function(){   
                // Confirmamos la eliminación de la cookie
                that.confirmDeleteCookie = true;
                // Procedemos al borrado/guardado
                that.handleConfirmDeleteCookie();
            });	                        
        }); 

        // Botón para visualizar u ocultar el listado de categorías cookies
        this.btnVerCategorias.off().on("click", function(){            
            that.handleDisplayHideCookiesCategory();
        });    
        
        // Tab de cada uno de los idiomas disponibles para cambiar la visualización en la lista de cláusulas
        this.tabIdiomaItem.off().on("click", function(){            
            that.handleViewCookieLanguageInfo();                        
            // Aplicar la búsqueda para el idioma activo
            that.timer = setTimeout(function () {                                                                                                        
                that.handleSearchCookieItem(that.txtBuscarCookie);
            }, 500);            
        });        
            
        // Búsquedas de cookies
        this.txtBuscarCookie.off().on("keyup", function (event) {
            const input = $(this);
            clearTimeout(that.timer);
            that.timer = setTimeout(function () {                                                                        
                // Acción de buscar / filtrar cookie
                that.handleSearchCookieItem(input);                                         
            }, 500);
        });  

        // Botón para añadir una nueva cookie 
        this.linkAddNewCookie.off().on("click", function(){
            // Obtener la url a crear para cargar el modal
            that.urlAddCookie = $(this).data("urladdcookie");
            // Obtener el dato de la cláusula
            that.cookieType = $(this).data("cookietype");            
            that.handleLoadAddCookie();
        });

        // Botón para añadir una nueva categoría cookie 
        this.linkAddCategory.off().on("click", function(){
            // Obtener la url a crear para cargar el modal
            that.urlAddCookie = $(this).data("urladdcookie");          
            that.handleLoadAddCategoryCookie();
        });

        // Botón para añadir una nueva cookie analítica de google
        this.linkAddGoogleCookieAnalytics.off().on("click", function(){
            // Obtener la url a crear para cargar el modal
            that.urlAddCookie = $(this).data("urladdcookie");
            // Obtener el dato de la cláusula                       
            that.handleLoadAddGoogleOrYoutubeCookie($(this));
        });
        
        // Botón para añadir una nueva cookie analítica de youtube
        this.linkAddYoutubeCookie.off().on("click", function(){
            // Obtener la url a crear para cargar el modal
            that.urlAddCookie = $(this).data("urladdcookie");
            // Obtener el dato de la cláusula                       
            that.handleLoadAddGoogleOrYoutubeCookie($(this));
        });          
        
        // Botón del modal para guardar la cookie
        configEventByClassName(`${that.btnGuardarCookieClassName}`, function(element){
            const $btnSaveCookie = $(element);
            $btnSaveCookie.off().on("click", function(){                               
                // Realizamos el guardado de las cookies
                that.handleSave();                                            
            });	                        
        });

        // Botón del modal para guardar la categoria cookie
        configEventByClassName(`${that.btnGuardarCategoryCookieClassName}`, function(element){
            const $btnSaveCategoryCookie = $(element);
            $btnSaveCategoryCookie.off().on("click", function(){                               
                // Realizamos el guardado de las cookies
                that.handleSave();                                            
            });	                        
        });   
        
        // Botón para guardar las cookies de Google/Youtube
        that.btnGuardarCookiesYoutubeGoogle.off().on("click", function(){
            // Realizamos el guardado de las cookies
            that.handleSave(true);
        });      
        

        // Botón para eliminar una cookie
        configEventByClassName(`${that.btnDeleteCookieClassName}`, function(element){
            const $removeCookie = $(element);
            $removeCookie.off().on("click", function(){   
                // Guardar la fila activa seleccionada
                that.filaCookie = $removeCookie.closest('.cookie-row');
                // Marcamos la cookie como para eliminar
                that.handleSetDeleteCookie(true);                                            
            });	                        
        }); 

        // Botón para eliminar una categoría cookie        
        configEventByClassName(`${that.btnDeleteCategoryCookieClassName}`, function(element){
            const $removeCategoryCookie = $(element);
            $removeCategoryCookie.off().on("click", function(){   
                // Guardar la fila activa seleccionada
                that.filaCookie = $removeCategoryCookie.closest('.cookie-row');
                // Marcamos la cookie como para eliminar
                that.handleSetDeleteCookie(true);                                        
            });	                        
        }); 	        
    },

    /**
     * Método para buscar una cookie
     * @param {jqueryElement} input : Input desde el que se ha realizado la búsqueda
     */
    handleSearchCookieItem: function(input){
        const that = this;

        // Cadena introducida para filtrar/buscar
        let cadena = input.val();
        // Eliminamos posibles tildes para búsqueda ampliada
        cadena = cadena.normalize('NFD').replace(/[\u0300-\u036f]/g, "").toLowerCase();

        // Listado de items donde se buscará
        const cookieItems = $(`.${that.cookieListItemClassName}`);

        // Recorrer los items/compontes para realizar la búsqueda de palabras clave
        $.each(cookieItems, function(index){
            const cookieItem = $(this);
            // Seleccionar el nombre sólo "visible" de la traducción. Quitamos caracteres extraños, tildes para poder hacer bien la búsqueda
            const componentContenido = cookieItem.find(".component-contenido").not('.d-none').html().trim().normalize("NFD").replace(/[\u0300-\u036f]/g, "").toLowerCase();
            const componentTipo = cookieItem.find(".component-tipo").html().trim().normalize("NFD").replace(/[\u0300-\u036f]/g, "").toLowerCase();

            if (componentContenido.includes(cadena) || componentTipo.includes(cadena)){
                // Mostrar la fila
                cookieItem.removeClass("d-none");                
            }else{
                // Ocultar la fila
                cookieItem.addClass("d-none");
            }            
        });        
    },

    /**
     * Método para cambiar la visualización al idioma de la cookie deseada
     */
    handleViewCookieLanguageInfo: function(){
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
     * Método para mostrar u ocultar el listado de categorías cookies de la comunidad
     */
    handleDisplayHideCookiesCategory: function(){
        const that = this;
        that.btnVerCategorias.toggleClass("btnActivo");
        // Mostrar u ocultar el listado de categorias de cookies
        that.btnVerCategorias.hasClass("btnActivo") ? that.cookieCategoryContainer.removeClass("d-none") : that.cookieCategoryContainer.addClass("d-none")        
    },

    /**
     * Método para guardar las cookies una vez se ha procedido a confirmar la eliminación de la deseada
     */
    handleConfirmDeleteCookie: function(){
        const that = this;
        that.handleSaveCookies(function(result, data){
            if (result == requestFinishResult.ok){
                // 2 - Ocultar el modal de eliminación de la cookie
                const modalDeleteCookie = $(that.filaCookie).find(`.${that.modalDeleteCookieClassName}`);                                          
                dismissVistaModal(modalDeleteCookie);
                // 3 - Ocultar loading
                loadingOcultar();                
                // 4 - Ocultar la fila que se desea eliminar
                that.filaCookie.addClass("d-none");
                // 5 - Eliminar la fila que se acaba de eliminar                                
                that.filaCookie.remove();
                // 6 - Actualizar el contador de nº de cookies
                that.handleCheckNumberOfCookies();
                // Restablecer el flag de borrado de la fila de la cookie
                that.confirmDeleteCookie = false;
                mostrarNotificacion("success", "Los cambios se han guardado correctamente");
            }else{
                mostrarNotificacion("error", "Se ha producido un error al tratar de eliminar la cookie. Contacta con el administrador de la comunidad.");
            }             
        });    
    }, 

    /**
     * Método para marcar o desmarcar la cookie como "Eliminada" dependiendo de la elección vía Modal
     * @param {Bool} deleteCookie Valor que indicará si se desea eliminar o no la cookie
     */
     handleSetDeleteCookie: function(deleteCookie){
        const that = this;

        if (deleteCookie){
            // Realizar el "borrado"
            // 1 - Marcar la opción "TabEliminada" a true
            that.filaCookie.find('[name="TabEliminada"]').val("true");           
            // 2 - Añadir la clase de "deleted" a la fila de la página
            that.filaCookie.addClass("deleted");
        }else{
            // 1 - Marcar la opción "TabEliminada" a false
            that.filaCookie.find('[name="TabEliminada"]').val("false");            
            // 2 - Eliminar la clase de "deleted" a la fila de la página
            that.filaCookie.removeClass("deleted");
        }
    },    

    /**
     * Método que se ejecutará cuando se cierre el modal de detalles de una cookie
     * @param {jqueryObject} currentModal Modal que se va a cerrar
     */
     handleCloseCookieModal: function(currentModal){
        const that = this;

        // Tener en cuenta si la cookie es de reciente creación y por tanto no se desea guardar
        if (that.filaCookie.hasClass("newCookie")){
            // Eliminar la fila que se acaba de eliminar                                
            that.filaCookie.remove();
            // Actualizar el contador de items
            that.handleCheckNumberOfCookies();
        }
    }, 
    
    /**
     * Método que se ejecutará al cargarse la web para saber el nº de traducciones existentes
     */
     handleCheckNumberOfCookies: function(){
        const that = this;
        const numberCookies = that.cookieListContainer.find($(`.${that.cookieListItemClassName}`)).length;
        const numberCategoryCookie = that.cookieCategoryListContainer.find($(`.${that.categoryCookieListItemClassName}`)).length;         
        
        that.numResultadosCookies.html(numberCookies);
        that.numResultadosCategoryCookies.html(numberCategoryCookie);
    },    
        
    /**
     * Método para crear/añadir una nueva cookie de tipo Técnica o analítica y que por lo tanto puede ser "editada por el usuario"
     */
    handleLoadAddCookie: function(){
        const that = this;
       
        // Mostrar loading hasta que finalice la petición para crear una nueva cláusula
        loadingMostrar();   

        // Construir el objeto/modelo para petición del tipo de la cláusula a crear
        const dataPost = {            
            Categoria: that.cookieType,
        }

        // Realizar la petición de la cookie a crear
        GnossPeticionAjax(                
            that.urlAddCookie,
            dataPost,
            true
        ).done(function (data) {
            // Devuelve la fila/row de la cookie a crear
            const lastCookieRow = that.cookieListContainer.children().last();         
            // Añadir la nueva fila al listado de cookies
            that.cookieListContainer.append(data);                        
            // Referencia a la nueva cookie añadida
            that.newCookie = that.cookieListContainer.children().last();
            // Añadirle el flag de "nueva cláusula" para saber que es de reciente creación.
            that.newCookie.addClass("newCookie");   
            // Establecer como fila seleccionada la nueva creada
            that.filaCookie = that.newCookie;
            // Panel de edición de la nueva cláusula creada
            const editionNewCookiePanel = that.newCookie.children(".modal-con-tabs");                                
            // Buscar nuevos items para asignarles comportamientos
            operativaGestionCookies.config();
            // Disparar eventos necesarios para nuevos items (Ej: multiIdioma)
            operativaGestionCookies.triggerEvents();
            // Actualizar contador de cookies
            that.handleCheckNumberOfCookies();
            // Abrir el modal para poder editar/gestionar la nueva cookie añadida            
            setTimeout(function() {
                that.newCookie.find(that.btnEditCookie).trigger("click");
            },500);              

            
        }).fail(function (data) {
            // Mostrar error al tratar de crear una nueva cookie
            mostrarNotificacion("error", data);                
        }).always(function () {
            // Ocultar loading de la petición
            loadingOcultar();
        });        
    },

    /**
     * Método para crear/añadir una nueva cookie analítica de Google o Youtube
     * @param {jqueryElement} cookiesButton : Botón de creación de cookies que se ha pulsado (Google, Youtube)
     */
     handleLoadAddGoogleOrYoutubeCookie: function(cookiesButton){
        const that = this;
       
        // Mostrar loading hasta que finalice la petición para crear una nueva cláusula
        loadingMostrar();   

        // Realizar la petición de la cookie a crear
        GnossPeticionAjax(                
            that.urlAddCookie,
            "",
            true
        ).done(function (data) {
            // Devuelve la fila/row de la cookie a crear
            const lastCookieRow = that.cookieListContainer.children().last();         
            // Añadir la nueva fila al listado de cookies
            that.cookieListContainer.append(data);                        
            // Referencia a la nueva cláusula añadida
            that.newCookie = that.cookieListContainer.children().last();
            // Actualizar contador de cookies
            that.handleCheckNumberOfCookies();  
            // Eliminar el botón de cookies sobre el que se ha pulsado (No es posible añadir más cookies de ese tipo)            
            cookiesButton.remove();                          
            // Buscar nuevos items para asignarles comportamientos
            operativaGestionCookies.config();
            // Disparar eventos necesarios para nuevos items (Ej: multiIdioma)
            operativaGestionCookies.triggerEvents(); 
            // Mostrar botón para guardado de cookies de youtube/google
            that.btnGuardarCookiesYoutubeGoogle.removeClass("d-none");
        }).fail(function (data) {
            // Mostrar error al tratar de crear una nueva cookie
            mostrarNotificacion("error", data);                
        }).always(function () {
            // Ocultar loading de la petición
            loadingOcultar();
        });        
    }, 
    
    /**
     * Método para crear/añadir una nueva categoría de cookies cookie      
     */
     handleLoadAddCategoryCookie: function(){
        const that = this;
       
        // Mostrar loading hasta que finalice la petición para crear una nueva cláusula
        loadingMostrar();   

        // Realizar la petición de la cookie a crear
        GnossPeticionAjax(                
            that.urlAddCookie,
            "",
            true
        ).done(function (data) {
            // Mostrar el panel de categorías si está oculto (Creando nueva categoría)
            that.btnVerCategorias.addClass("btnActivo");
            that.cookieCategoryContainer.removeClass("d-none");

            // Devuelve la fila/row de la cookie a crear
            const lastCookieRow = that.cookieCategoryListContainer.children().last();                     
            // Añadir la nueva fila al listado de categorías de cookies
            that.cookieCategoryListContainer.append(data);                        
            // Referencia a la nueva cookie añadida
            that.newCookie = that.cookieCategoryListContainer.children().last();
            // Añadirle el flag de "nueva cookie" para saber que es de reciente creación.
            that.newCookie.addClass("newCookie");   
            // Establecer como fila seleccionada la nueva creada
            that.filaCookie = that.newCookie;
            // Panel de edición de la nueva cláusula creada
            const editionNewCookiePanel = that.newCookie.children(".modal-con-tabs");                                
            // Buscar nuevos items para asignarles comportamientos
            operativaGestionCookies.config();
            // Disparar eventos necesarios para nuevos items (Ej: multiIdioma)
            operativaGestionCookies.triggerEvents();            
            // Abrir el modal para poder editar/gestionar la nueva category cookie añadida    
            setTimeout(function() {
                that.newCookie.find(that.btnEditCategoryCookie).trigger("click");    
            },500);            
            // Actualizar contador de cookies
            that.handleCheckNumberOfCookies();                                   
        }).fail(function (data) {
            // Mostrar error al tratar de crear una nueva cookie
            mostrarNotificacion("error", data);                
        }).always(function () {
            // Ocultar loading de la petición
            loadingOcultar();
        });        
    },  
    
    /**
     * Método que ejecutará la revisión de cookies y su posterior guardado de categoria de cookies y de cookies.
     * Este método ejecutará por tanto el "handleSaveCookies".         
     * @param {bool} savingYoutubeGoogleCookies Indica si se ha pulsado para guardar las cookies de Google y Youtube. En este caso, no hace falta
     * hacer dismiss de ningún modal.
     */
    handleSave: function(savingYoutubeGoogleCookies = false){
        const that = this;

        that.handleSaveCookies(function(result, data){
            if (result == requestFinishResult.ok){
                //Ocultar el modal de edición de la cookie
                if (savingYoutubeGoogleCookies == true){
                    const modalEditCookie = $(that.filaCookie).find(`.${that.modalCookieClassName}`);                                          
                    dismissVistaModal(modalEditCookie);                                                              
                }                
                // Actualizar el contador de nº de cookies
                that.handleCheckNumberOfCookies();                        
                loadingOcultar();                 
                // Mostrar ok
                mostrarNotificacion("success", "Los cambios se han guardado correctamente");    
                // Ocultar el botón de guardado general (si se han añadido cookies de youtube/google)
                that.btnGuardarCookiesYoutubeGoogle.addClass("d-none");                                                
                // 6 - Reload de la página
                setTimeout(function() {                                                                                                                         
                    location.reload();
                }
                ,1000);
            }else{
                mostrarNotificacion("error", data);
            }             
        });           
    },

    /**
     * Método para guardar las cookies existentes. Este método se ejecuta cuando se pulsa en el modal de "Guardar"
     * @param {*} completion : Función a ejecutar cuando se realice o complete este método.
     */
    handleSaveCookies: function(completion = undefined){
        const that = this;
        
        // Mostrar loading
        loadingMostrar();
        
        // Objeto de las categorías a guardar en backend
        that.ListaCategorias = {};
        that.ListaCookies = {};
        let cont = 0;

        // Comprobación de erres antes de proceder al guardado - Comprobación de categorías cookies
        that.checkErrorsBeforeSaving();

        // Si todo ha ido bien, proceder con la recogida de datos y su guardado
        if (that.errorsBeforeSaving == false) {
            // Obtener los datos de las categorías. Recorrer y obtener su información
            that.cookieCategoryListContainer.find($(`.${that.categoryCookieListItemClassName}`)).each(function () {
                that.obtenerDatosCategorias($(this), cont++);                
            });

            // Restablecer contador para creación de cookies
            cont = 0;
            // Obtener los datos de las cookies. Recorrer y obtener su información
            that.cookieListContainer.find($(`.${that.cookieListItemClassName}`)).each(function () {
                
                that.obtenerDatosCookies($(this), cont++);                
            });

            // Guardar categorías y seguídamente cookiesButton
            GnossPeticionAjax(
                that.urlSaveCategories,
                that.ListaCategorias,
                true
            ).done(function (data) {
                // OK - Guardado Categorías -> Guardar cookies                
                GnossPeticionAjax(
                    that.urlSaveCookies,
                    that.ListaCookies,
                    true
                ).done(function (data) {  
                    // OK Guardar cookies                
                    $(".newCookie").removeClass("newCookie");                                        
                    if (completion != undefined){
                        completion(requestFinishResult.ok, data);
                    }else{
                        mostrarNotificacion("success", "Los cambios se han guardado correctamente");
                    }
                }).fail(function (data) {
                    // KO al guardar Cookies                                                                             
                    if (completion != undefined){
                        completion(requestFinishResult.ko, data);
                    }else{
                        mostrarNotificacion("error", data);    
                    }                    
                }).always(function () {
                    if (completion == undefined){
                        loadingOcultar();
                    }    
                });                
            }).fail(function (data) {
                // KO al guardar Categorias Cookies                                
                if (completion != undefined){
                    completion(requestFinishResult.ko, data);
                }else{
                    mostrarNotificacion("error", data);    
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
     * Método para recoger los datos de las categorías y construir el objeto para envío a backend
     * @param {jqueryElement} fila : Fila de la categoría cookie de la que se desea obtener la información
     * @param {*} num : Contador para construir el objeto de las categorías cookies
     */
    obtenerDatosCategorias: function(fila, num){
        const that = this;

        // Id de la categoría cookie de la que se obtendrán los datos
        const id = fila.attr('id');
        
        // Contenido o datos de la cookie dentro del modal
        const panelEdicion = fila.find('.modal-cookie');

        // Prefijo para guardado en bd
        const prefijoClave = 'ListaCategorias[' + num + ']';

        // Id de la categoría cookie
        that.ListaCategorias[prefijoClave + '.CategoriaID'] = id;
        // Nombre
        that.ListaCategorias[prefijoClave + '.Nombre'] = panelEdicion.find('[name="CategoryName"]').val();
        // Nombre corto
        that.ListaCategorias[prefijoClave + '.NombreCorto'] = panelEdicion.find('[name="CategoryShortName"]').val();
        // Descripción
        that.ListaCategorias[prefijoClave + '.Descripcion'] = panelEdicion.find('[name="CategoryDescription"]').val();
        // Borrado?
        that.ListaCategorias[prefijoClave + '.Deleted'] = panelEdicion.find('[name="TabEliminada"]').val();
        // Categoría Modificada?
        that.ListaCategorias[prefijoClave + '.EsModificada'] = panelEdicion.find('[name="CategoriaModificada"]').val();

    },


    /**
     * Método para recoger los datos de las cookies y construir el objeto para envío a backend
     * @param {jqueryElement} fila : Fila de la cookie de la que se desea obtener la información
     * @param {*} num : Contador para construir el objeto de las cookies
     */
    obtenerDatosCookies: function(fila, num){

        const that = this;
        const id = fila.attr('id');

        // Contenido o datos de la cookie dentro del modal
        const panelEdicion = fila.find('.modal-cookie');

        // Prefijo para guardado en bd
        const prefijoClave = 'ListaCookies[' + num + ']';

        // Id de la cookie
        that.ListaCookies[prefijoClave + '.CookieID'] = id;
        // Nombre de la cookie
        that.ListaCookies[prefijoClave + '.Nombre'] = panelEdicion.find('[name="CookieName"]').val();
        // Categoria de la cookie
        that.ListaCookies[prefijoClave + '.Categoria'] = panelEdicion.find('[name="TabCategoria"]').val();
        // Descripción de la cookie
        that.ListaCookies[prefijoClave + '.Descripcion'] = panelEdicion.find('[name="CookieDescription"]').val();
        // Tipo de duración de la cookie
        that.ListaCookies[prefijoClave + '.Tipo'] = panelEdicion.find('[name="TabTipoDuracion"]').val();
        // Ha sido eliminada?
        that.ListaCookies[prefijoClave + '.Deleted'] = panelEdicion.find('[name="TabEliminada"]').val();
        // Ha sido modificada?
        that.ListaCookies[prefijoClave + '.EsModificada'] = panelEdicion.find('[name="CookieModificada"]').val();        
    },

    /**
     * Método para comprobar que no se encuentra ningún error antes de realizar el guardado.
     * El resultado de esas comprobaciones se guarda en 'errorsBeforeSaving'
     */
    checkErrorsBeforeSaving: function(){
        const that = this;
        
        // Flag para control de errores
        that.errorsBeforeSaving = false;                
        
        // Comprobar nombres vacíos
        that.comprobarNombresVacios();            
        
        // Comprobar nombres y descripciones de Cookies y categorías
        if (that.errorsBeforeSaving == false){
            that.comprobarContenidosVacios()
        }  
    },

    /**
     * Método para comprobar que los nombres cortos/textos no esté vacío.
     */
     comprobarNombresVacios: function(){
        const that = this;

        // Comprobación de que el nombre de la categoría es correcto
        let inputsNombre = $(`.${that.categoryCookieListItemClassName} input[name="CategoryName"]:not(":disabled")`);        
        // Comprobación de los nombres
        inputsNombre.each(function () {
            if (that.errorsBeforeSaving == false){
                that.comprobarNombreVacio($(this));
            }
        });
        
        // Comprobación de que el nombre de la cookie es correcta
        if (that.errorsBeforeSaving == false){                            
            inputsNombre = $(`.${that.cookieListItemClassName} input[name="CookieName"]:not(":disabled")`);            
            // Comprobación de los nombres
            inputsNombre.each(function () {
                if (that.errorsBeforeSaving == false){
                    that.comprobarNombreVacio($(this));
                }
            });
        }
    },
    /**
     * Método para comprobar que el nombre corto y ó texto no esté vacío. Habrá que tener en cuenta el multiIdioma
     * @param {*} inputName: Input a comprobar     
     */
     comprobarNombreVacio: function (inputName) {
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
            
            if (textoIdiomaDefecto == null || textoIdiomaDefecto == "") {
                const fila = inputName.closest(".component-wrap.cookie-row");
                that.mostrarErrorNombreVacio(fila, panMultiIdioma.find(`#input_${inputId}_${operativaMultiIdioma.idiomaPorDefecto}`));
                that.errorsBeforeSaving = true;
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
    },
    
    /**
     * Método para comprobar que el contenido no está vacío (TextArea...)
     */
     comprobarContenidosVacios: function () {
        const that = this;        

        let inputsNombre = $(`.${that.categoryCookieListItemClassName} textarea[name = "CategoryDescription"]:not(":disabled")`);
        
        // Comprobación del texto explicativo de la categoría cookie
        inputsNombre.each(function () {
            if (that.errorsBeforeSaving == false){
                that.comprobarNombreVacio($(this));
            }
        });  
        
        // Comprobación de que la descripción de la cookie es correcta
        if (that.errorsBeforeSaving == false){                            
            inputsNombre = $(`.${that.cookieListItemClassName} textarea[name="CookieDescription"]:not(":disabled")`);            
            // Comprobación de los nombres
            inputsNombre.each(function () {
                if (that.errorsBeforeSaving == false){
                    that.comprobarNombreVacio($(this));                
                }
            });
        }        

    },

    /**
     * Método para mostrar el error del nombre vacío encontrado en una cookie. Se muestra cuando el nombre de una cookie en el idioma por defecto no existe.
     * @param {jqueryObject} fila : Elemento jquery de la fila correspondiente a donde se ha encontrado el error.
     * @param {jqueryObject} input : Elemento jquery del input donde se ha producido el error. Puede que ser undefined
     */
     mostrarErrorNombreVacio: function(fila, input){                              
        if (input != undefined){
            comprobarInputNoVacio(input, true, false, "Esta información no puede estar vacía.", 0);
        }
        setTimeout(function(){  
            mostrarNotificacion("error", "El nombre y descripción no puede estar vacío.");
        },1000);  
    },  
}


/**
  * Operativa para la gestión/configuración del buzón de correo de la comunidad
  */
const operativaGestionConfiguracionBuzonCorreo = {

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
        this.urlSaveConfiguration = `${this.urlBase}/save`;
        this.urlValidarCorreo = `${this.urlBase}/validarCorreo`;
        // Objeto donde se guardarán las opciones para su guardado
        this.Options = {};
    },  

    /*
    * Inicializar elementos de la vista
    * */
    config: function (pParams) {
        
        // Input de email
        this.email = $("#Email");
        // Input de SMTP
        this.smtp = $("#SMTP");
        // Input de puerto
        this.port = $("#Port");
        // Input de nombre de usuario
        this.user = $("#User");
        // Input de password
        this.password = $("#Password");
        // Input de tipo de servidor
        this.serverType = $("#Type");
        // RadioButton Sí/No de uso de SSL
        this.ssl_si = $("#SSL_SI");
        this.ssl_no = $("#SSL_NO");
        // Input de email de sugerencias
        this.suggestEmail = $("#SuggestEmail");  
        
        // Botón para guardado
        //this.btnSave = $("#btnSave");
        this.btnValidarEmail = $("#btnValidarEmail");
        
    },   

    /**
     * Configuración de eventos de elementos del Dom (Botones, Inputs...)     
     */
    configEvents: function (pParams) {
        const that = this;
        
        /*
        // Botón para guardar los datos de configuración del buzón
        this.btnSave.off().on("click", function(){   
            that.crearModalEmail();                                       
            //that.handleSave();
        });    */    
        
        // Botón para validar el email y guardar los datos
        this.btnValidarEmail.off().on("click", function(){   
            that.handleObtenerdatos();
            that.handleValidarEmail();                                       
            
        });
        
    },


    /**
     * Método para recoger los datos de la configuración para el guardado.
     */
    handleObtenerdatos: function(){
        const that = this;

        // Objeto que se utilizará para el guardado de configuración del buzón
        that.Options = {};

        // Prefijo para el guardado de los datos
        const prefijoConfCorreo = "ConfiguracionCorreo";
        // Recogida de datos para guardado
        that.Options[prefijoConfCorreo + '.Email'] = that.email.val();
        that.Options[prefijoConfCorreo + '.SMTP'] = that.smtp.val();
        that.Options[prefijoConfCorreo + '.Port'] = that.port.val();
        that.Options[prefijoConfCorreo + '.User'] = that.user.val();
        that.Options[prefijoConfCorreo + '.Password'] = that.password.val();
        that.Options[prefijoConfCorreo + '.Type'] = that.serverType.val();
        that.Options[prefijoConfCorreo + '.SSL'] = that.ssl_si.is(':checked');
        that.Options[prefijoConfCorreo + '.SuggestEmail'] = that.suggestEmail.val();
        that.Options[prefijoConfCorreo + '.Destinatario'] = document.getElementById("txtCorreoValidar").value;                  
    },

    
     /**
      * Método para validar el email
      */
     handleValidarEmail: function(){
        const that = this;  
           
        loadingMostrar();            
    
        GnossPeticionAjax(
            this.urlValidarCorreo,
            that.Options,
            true
        ).done(function (response) {
            // Ocultar modal
            pJqueryModalView = $("#validar-correo");
            dismissVistaModal(pJqueryModalView);
            mostrarNotificacion("success","El correo electrónico se ha configurado con éxito");
        }).fail(function (error) {
            // KO                        
            mostrarNotificacion("error", error);            
        }).always(function () {            
            loadingOcultar();
        });

        

     },

    /**
     * Método para guardar los datos de la configuración.
     */    
    handleSave: function(){
        const that = this;

        // Mostrar loading
        loadingMostrar();

        // Realizar petición para el guardado
        GnossPeticionAjax(
        that.urlSaveConfiguration,
        that.Options,
        true
        ).done(function (data) {
            mostrarNotificacion("success","El correo electrónico se ha configurado con éxito");
        }).fail(function (data) {
            // KO en el guardado de datos                        
            mostrarNotificacion("error", data);            
        }).always(function () {
            // Ocultar loading
            loadingOcultar();
        });
    },
}

/**
  * Operativa para la gestión/configuración del MetaAdministrador
  */
operativaGestionConfiguracionMetaAdministrador = {
    
    /**
     * Inicializar operativa
     */
     init: function (pParams) {
        this.pParams = pParams;
        this.config(pParams);
        this.configEvents();
        this.configRutas();
        this.triggerEvents();
        this.handleShowAutomaticRegistration();
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
        this.urlSaveConfiguration = `${this.urlBase}/save`;
        this.urlShutdown = `${this.urlBase}/shutdown`;
        // Objeto donde se guardarán las opciones para su guardado
        this.Options = {};
    },  

    /*
    * Inicializar elementos de la vista
    * */
    config: function (pParams) {
        

        /* Inputs de Permisos */
        this.CMSActivado = $("#CMSActivado");
        this.AdministracionPaginasPermitido = $("#AdministracionPaginasPermitido");
        this.AdministracionSemanticaPermitido = $("#AdministracionSemanticaPermitido");
        this.AdministracionDesarrolladoresPermitido = $("#AdministracionDesarrolladoresPermitido");
        this.EventosDisponibles = $("#EventosDisponibles");
        this.AdministracionVistasPermitido = $("#AdministracionVistasPermitido");
        // Panel contenedor del Select PersonalizacionIDVistas
        this.panelPersonalizacionVistas = $(".editarVista");
        // Panel de aleste que avisa de que se va a reiniciar la aplicación
        this.panelAvisoReinicioAplicacion = $("#panelAvisoReinicioAplicacion");
        // Panel informativo de que las vistas se van a eliminar
        this.panelAlertaPersonalizacionVistas = $("#panelAlertaPersonalizacionVistas");
        this.PersonalizacionIDVistas = $("#PersonalizacionIDVistas");        
        this.NoUsarVistasDelEcosistema = $("#NoUsarVistasDelEcosistema");
        this.VistasActivadas = $("#VistasActivadas");
        this.CargasMasivasDisponibles = $("#CargasMasivasDisponibles");
        this.OcultarCambiarPassword = $("#OcultarCambiarPassword");
        this.DuplicarRecursosDisponible = $("#DuplicarRecursosDisponible");
        this.NombrePoliticaCookies = $("#NombrePoliticaCookies");
        /* Inputs de Permisos CMS */
        this.ConsultaSparql = $("#ConsultaSparql");
        this.PreguntaTIC = $("#PreguntaTIC");
        this.BuscadorSPARQL = $("#BuscadorSPARQL");
        this.FichaDescripcionDocumento = $("#FichaDescripcionDocumento");
        this.ConsultaSQLServer = $("#ConsultaSQLServer");
        /* Inputs de Apariencia */
        this.CabeceraSimple = $("#CabeceraSimple");
        this.EtiquetasConLOD = $("#EtiquetasConLOD");
        this.AgruparEventosNuevosUsuarios = $("#AgruparEventosNuevosUsuarios");
        this.RdfDisponibles = $("#RdfDisponibles");
        this.AvisoCookie = $("#AvisoCookie");
        this.VersionCSS = $("#VersionCSS");
        this.VersionJS = $("#VersionJS");
        this.Copyright = $("#Copyright");
        this.EnlaceContactoPiePagina = $("#EnlaceContactoPiePagina");
        this.NumeroRecursosRelacionados = $("#NumeroRecursosRelacionados");
        this.CapturasImgSizeAlto = $("#CapturasImgSizeAlto");
        this.CapturasImgSizeAncho = $("#CapturasImgSizeAncho");
        this.CargarEditoresLectoresEnBusqueda = $("#CargarEditoresLectoresEnBusqueda");
        this.FilasPorPagina = $("#FilasPorPagina");
        this.EnlaceContactoPiePagina = $("#EnlaceContactoPiePagina");        
        /* Inputs de Usuarios */
        this.FechaNacimientoObligatoria = $("#FechaNacimientoObligatoria"); 
        this.BiosCortas = $("#BiosCortas"); 
        this.PrivacidadObligatoria = $("#PrivacidadObligatoria"); 
        this.InvitacionesPorContactoDisponibles = $("#InvitacionesPorContactoDisponibles"); 

        this.EnviarNotificacionesDeSuscripciones = $("#EnviarNotificacionesDeSuscripciones"); 
        this.RecibirNewsletterDefecto = $("#RecibirNewsletterDefecto"); 
        this.SuscribirATodaComunidad = $("#SuscribirATodaComunidad"); 
        this.PermitirDescargaIdentidadInvitada = $("#PermitirDescargaIdentidadInvitada"); 
        this.CaducidadPassword = $("#CaducidadPassword");   
        
        this.SolicitarCookieLogin = $("#SolicitarCookieLogin");
        this.RegistroAbierto = $("#RegistroAbierto");
        this.DiaEnvioSuscripcion = $("#DiaEnvioSuscripcion");


        /* Inputs de Sistemas */
        this.SiteMapActivado = $("#SiteMapActivado");
        this.GoogleDrive = $("#GoogleDrive");
        this.ProyectoSinNombreCortoEnURL = $("#ProyectoSinNombreCortoEnURL");
        this.RegistroAutomatico = $("#RegistroAutomatico");
        this.RegistroAutomaticoCheckbox = $("#RegistroAutomaticoCheckbox");
        this.ReiniciarAplicacion = $("#ReiniciarAplicacion");
        this.SegundosDormirNewsletterPorCorreo = $("#SegundosDormirNewsletterPorCorreo");
        this.Replicacion = $("#Replicacion");        
        /* Inputs de Semántica */
        this.OcultarFacetasDeOntologiasEnRecursosCuandoEsMultiple = $("#OcultarFacetasDeOntologiasEnRecursosCuandoEsMultiple");
        this.TerceraPeticionFacetasPlegadas = $("#TerceraPeticionFacetasPlegadas");
        this.TieneGrafoDbPedia = $("#TieneGrafoDbPedia");
        this.TextoInvariableTesauroSemantico = $("#TextoInvariableTesauroSemantico");
        this.NumeroFacetasPrimeraPeticion = $("#NumeroFacetasPrimeraPeticion");
        this.NumeroFacetasSegundaPeticion = $("#NumeroFacetasSegundaPeticion");
        this.AlgoritmoPersonasRecomendadas = $("#AlgoritmoPersonasRecomendadas");
        this.FacetasCostosasTerceraPeticion = $("#FacetasCostosasTerceraPeticion");
        this.PropiedadContenidoMultiIdioma = $("#PropiedadContenidoMultiIdioma");
        this.PropiedadCMSMultiIdioma = $("#PropiedadCMSMultiIdioma");
        this.PropiedadesConEnlacesDbpedia = $("#PropiedadesConEnlacesDbpedia");
        this.ExcepcionBusquedaMovil = $("#ExcepcionBusquedaMovil");
        this.PermitirMayusculas = $("#PermitirMayusculas");
        /* Inputs de Redes sociales */
        this.LoginFacebook = $("#LoginFacebook");
        this.LoginGoogle = $("#LoginGoogle");
        this.LoginTwitter = $("#LoginTwitter");        
        /* Inputs de Proyectos */        
        this.linkSetupProjects = $("#linkSetupProjects");        
        this.confProyectos = $("#confProyectos");
        /* Inputs de Estilos */
        this.RutaEstilos = $("#RutaEstilos");
        /* Inputs de ServiceBus */
        this.ServiceBusSegundos = $("#ServiceBusSegundos");
        this.ServiceBusReintentos = $("#ServiceBusReintentos");
        /* Botón de guardado */
        this.btnSave = $("#btnSave");
    },   

    /**
     * Configuración de eventos de elementos del Dom (Botones, Inputs...)     
     */
    configEvents: function (pParams) {
        const that = this;
            
        // Botón para visualizar la configuración de proyectos
        this.linkSetupProjects.on("click", function(){
            const button = $(this);
            that.confProyectos.removeClass("d-none");
            button.remove();
        });

        this.btnSave.on("click", function(){
            that.handleObtenerdatos();
            that.handleSaveMetaAdministradorConfiguration();
        });

        // Posibilidad de activar vistas personalizadas y qué vistas escoger
        this.VistasActivadas.on("click", function(){
            that.handleShowHideProjectViews();
        });

        this.ProyectoSinNombreCortoEnURL.on("click", function (){
            that.handleShowRebootAlert();
            that.handleShowAutomaticRegistration();
        });
    },

    /**
     * Método para controlar la visualización del panel de alerta de reinicio de la aplicación
     */
    handleShowRebootAlert: function () {
        const that = this;
        // Detectar si se desea mostrar o no el panel de alerta de reinicio de la aplicación
        if (!that.panelAvisoReinicioAplicacion.hasClass("d-flex"))
        {
            that.panelAvisoReinicioAplicacion.addClass("d-flex");
        }
        else
        {
            that.panelAvisoReinicioAplicacion.removeClass("d-flex");
        }
    },

    handleShowAutomaticRegistration: function () {
        const that = this;
        if (that.ProyectoSinNombreCortoEnURL.is(':checked'))
        {
            that.RegistroAutomatico.removeClass("d-none");
        }
        else
        {
            that.RegistroAutomatico.addClass("d-none");
            that.RegistroAutomaticoCheckbox.prop('checked', false);
        }
    },

    /**
     * Método para controlar la visualización de qué vistas personalizadas utilizar para la comunidad     
     */
    handleShowHideProjectViews: function(){
        const that = this;        
        // Detectar si se desea mostrar o no el panel para la selección de vistas personalizadas        
        const showProjectViewsPanel = that.VistasActivadas.is(':checked');
        
        // Mostrar personalización de vistas si las hay
        showProjectViewsPanel ? that.panelPersonalizacionVistas.removeClass("d-none") : that.panelPersonalizacionVistas.addClass("d-none");
        // Mostrar alerta de destrucción de vistas    
        
        if (showProjectViewsPanel){
            that.panelAlertaPersonalizacionVistas.addClass("d-none");
            that.panelAlertaPersonalizacionVistas.removeClass("d-flex");            
        }else{
            that.panelAlertaPersonalizacionVistas.removeClass("d-none");
            that.panelAlertaPersonalizacionVistas.addClass("d-flex");            
        }        
    },

    /**
     * Método para recoger los datos de la configuración para el guardado.
     */
    handleObtenerdatos: function(){
        const that = this;
        
        // Vaciado del objeto para su posterior construcción
        that.Options = {};

        
        that.Options['CMSActivado'] = that.CMSActivado.is(':checked');
        that.Options['AdministracionSemanticaPermitido'] = that.AdministracionSemanticaPermitido.is(':checked');
        that.Options['AdministracionPaginasPermitido'] = that.AdministracionPaginasPermitido.is(':checked');
        that.Options['AdministracionVistasPermitido'] = that.AdministracionVistasPermitido.is(':checked');
        that.Options['AdministracionDesarrolladoresPermitido'] = that.AdministracionDesarrolladoresPermitido.is(':checked');
        that.Options['TextoInvariableTesauroSemantico'] = that.TextoInvariableTesauroSemantico.is(':checked');
        that.Options['VistasActivadas'] = that.VistasActivadas.is(':checked');

        if (that.VistasActivadas.is(':checked') && that.PersonalizacionIDVistas.length > 0)
        {
            that.Options['PersonalizacionIDVistas'] = that.PersonalizacionIDVistas.val();
        }

        if ( that.NoUsarVistasDelEcosistema.length > 0) {
            that.Options['NoUsarVistasDelEcosistema'] = that.NoUsarVistasDelEcosistema.is(':checked');
            that.Options['TieneVistasEcosistema'] = 'true';
        }

        that.Options['CabeceraSimple'] = that.CabeceraSimple.is(':checked');        
        that.Options['EtiquetasConLOD'] = that.EtiquetasConLOD.is(':checked');        
        that.Options['AgruparEventosNuevosUsuarios'] = that.AgruparEventosNuevosUsuarios.is(':checked');

        that.Options['SiteMapActivado'] = that.SiteMapActivado.is(':checked');

        that.Options['BiosCortas'] = that.BiosCortas.is(':checked');
        that.Options['RdfDisponibles'] = that.RdfDisponibles.is(':checked');
        that.Options['CargasMasivasDisponibles'] = that.CargasMasivasDisponibles.is(':checked');
        that.Options['ConsultaSparql'] = that.ConsultaSparql.is(':checked');        
        that.Options['PreguntaTIC'] = that.PreguntaTIC.is(':checked');
        that.Options['BuscadorSPARQL'] = that.BuscadorSPARQL.is(':checked');
        that.Options['FechaNacimientoObligatoria'] = that.FechaNacimientoObligatoria.is(':checked');
        that.Options['ConsultaSQLServer'] = that.ConsultaSQLServer.is(':checked');
        that.Options['PrivacidadObligatoria'] = that.PrivacidadObligatoria.is(':checked');
        that.Options['EventosDisponibles'] = that.EventosDisponibles.is(':checked');

        if (that.SolicitarCookieLogin.length > 0) {
            that.Options['SolicitarCoockieLogin'] = that.SolicitarCookieLogin.is(':checked');
        }else{
            that.Options['SolicitarCoockieLogin'] = false; // Esta opción no está habilitada para esta comunidad/proyecto
        }

        that.Options['InvitacionesPorContactoDisponibles'] = that.InvitacionesPorContactoDisponibles.is(':checked');
        that.Options['AvisoCookie'] = that.AvisoCookie.is(':checked');
        that.Options['RecibirNewsletterDefecto'] = that.RecibirNewsletterDefecto.is(':checked');
        that.Options['EnviarNotificacionesDeSuscripciones'] = that.EnviarNotificacionesDeSuscripciones.is(':checked');

        // Era 1 o 0. Enviarlo teniendo en cuenta el "checkbox"        
        //that.Options['AceptacionComunidadesAutomatica'] = that.AceptacionComunidadesAutomatica.is(':checked') ? '1' : '0';
        // Era 1 o 0. Enviarlo teniendo en cuenta el "checkbox"
        //that.Options['CargarIdentidadesDeProyectosPrivadosComoAmigos'] = that.CargarIdentidadesDeProyectosPrivadosComoAmigos.is(':checked') ? '1' : '0';

        that.Options['SuscribirATodaComunidad'] = that.SuscribirATodaComunidad.is(':checked');
        that.Options['GoogleDrive'] = that.GoogleDrive.is(':checked');
        that.Options['OcultarFacetasDeOntologiasEnRecursosCuandoEsMultiple'] = that.OcultarFacetasDeOntologiasEnRecursosCuandoEsMultiple.is(':checked');
        that.Options['TerceraPeticionFacetasPlegadas'] = that.TerceraPeticionFacetasPlegadas.is(':checked');
        that.Options['TieneGrafoDbPedia'] = that.TieneGrafoDbPedia.is(':checked');
        that.Options['ProyectoSinNombreCortoEnURL'] = that.ProyectoSinNombreCortoEnURL.is(':checked');
        that.Options['RegistroAutomaticoCheckbox'] = that.RegistroAutomaticoCheckbox.is(':checked');
        that.Options['ReiniciarAplicacion'] = that.ReiniciarAplicacion.is(':checked');
        

        if (that.RegistroAbierto.length > 0) {
            that.Options['RegistroAbierto'] = that.RegistroAbierto.is(':checked');
        }else{
            that.Options['RegistroAbierto'] = false; // Esta opción no está habilitada para esta comunidad/proyecto
        }

        that.Options['PermitirDescargaIdentidadInvitada'] = that.PermitirDescargaIdentidadInvitada.is(':checked');
        that.Options['CargarEditoresLectoresEnBusqueda'] = that.CargarEditoresLectoresEnBusqueda.is(':checked');

        that.Options['VersionCSS'] = that.VersionCSS.val();
        that.Options['VersionJS'] = that.VersionJS.val();
        that.Options['NumeroRecursosRelacionados'] = that.NumeroRecursosRelacionados.val();
        that.Options['Copyright'] = that.Copyright.val();
        that.Options['EnlaceContactoPiePagina'] = that.EnlaceContactoPiePagina.val();
        that.Options['AlgoritmoPersonasRecomendadas'] = that.AlgoritmoPersonasRecomendadas.val();

        that.Options['CapturasImgSizeAlto'] = that.CapturasImgSizeAlto.val();
        that.Options['CapturasImgSizeAncho'] = that.CapturasImgSizeAncho.val();

        that.Options['DiaEnvioSuscripcion'] = that.DiaEnvioSuscripcion.val();
        that.Options['SegundosDormirNewsletterPorCorreo'] = that.SegundosDormirNewsletterPorCorreo.val();

        that.Options['NumeroFacetasPrimeraPeticion'] = that.NumeroFacetasPrimeraPeticion.val();
        that.Options['NumeroFacetasSegundaPeticion'] = that.NumeroFacetasSegundaPeticion.val();

        that.Options['LoginFacebook'] = that.LoginFacebook.val();
        that.Options['LoginGoogle'] = that.LoginGoogle.val();
        that.Options['LoginTwitter'] = that.LoginTwitter.val();
        that.Options['FacetasCostosasTerceraPeticion'] = that.FacetasCostosasTerceraPeticion.val();
        that.Options['PropiedadContenidoMultiIdioma'] = that.PropiedadContenidoMultiIdioma.val();
        that.Options['PropiedadCMSMultiIdioma'] = that.PropiedadCMSMultiIdioma.is(':checked');
        that.Options['PropiedadesConEnlacesDbpedia'] = that.PropiedadesConEnlacesDbpedia.val();
        that.Options['ExcepcionBusquedaMovil'] = that.ExcepcionBusquedaMovil.val();
        that.Options['ServiceBusSegundos'] = that.ServiceBusSegundos.val();
        that.Options['ServiceBusReintentos'] = that.ServiceBusReintentos.val();

        if (that.RegistroAbierto.length > 0) {
            that.Options['RegistroAbiertoEnComunidad'] = that.RegistroAbierto.is(':checked');
        }else{
            that.Options['RegistroAbiertoEnComunidad'] = false; // Esta opción no está habilitada para esta comunidad/proyecto
        }

        that.Options['Replicacion'] = that.Replicacion.is(':checked');
        that.Options['FilasPorPagina'] = that.FilasPorPagina.val();
        that.Options['PermitirMayusculas'] = that.PermitirMayusculas.is(':checked');
        //Recorrer los que tienen class="comSinRegistro" // Proyectos
        let texto = undefined;
        if ($('.comSinRegistro').length != 0)
        {
            let mapa = new Map();            
            for(i = 0; i< $('.comSinRegistro').length; i++)
            {
                var item = $('.comSinRegistro')[i];
                if (i != 0) {
                    texto = texto + ",";
                }
                texto = texto +item.id+ ":"+item.checked;
                mapa.set(item.id, item.checked);
            }
        }
        that.Options['ProyectosView'] = texto;
        that.Options['RutaEstilos'] = that.RutaEstilos.val();
    },

    /**
     * Método para guardar los datos de la configuración de metaAdministrador
     */    
     handleSaveMetaAdministradorConfiguration: function(){
        const that = this;

        // Mostrar loading
        loadingMostrar();       
            
        // Realizar petición para guardado
        GnossPeticionAjax(
        that.urlSaveConfiguration,
        that.Options,
        true
        ).done(function (data) {
            // OK Guardado
            if (data == "shutdown")
            {
                mostrarNotificacion("success", "Los cambios se han guardado correctamente y se va a proceder al reinicio de la aplicacion");
                setTimeout(function () {
                    // Realizar petición para apagar la aplicacion
                    GnossPeticionAjax(
                        that.urlShutdown,
                        true
                    ).done(function (data) {
                        // OK Guardado
                        mostrarNotificacion("success", data);
                    }).fail(function (data) {
                        // KO Guardado
                        mostrarNotificacion("error", data);
                    }).always(function () {
                        loadingOcultar();
                    });
                }, 5000);
            }
            else
            {
                mostrarNotificacion("success", "Los cambios se han guardado correctamente");
            }
        }).fail(function (data) {
            // KO Guardado
            const error = data.split('|||');
            if (data != "") {
                mostrarNotificacion("error", data);
            }
            else
            {
                mostrarNotificacion("error", "Se ha producido un error al guardar, inténtelo de nuevo más tarde.");
            }
        }).always(function () {
            loadingOcultar();
        });       
    },    
}

/**
  * Operativa para la gestión/configuración de la plataforma 
  */
operativaGestionConfiguracionPlataforma = {
    
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
        that.loadInitialLanguageValues();
    },   

    /**
     * Configuración de las rutas de acciones necesarias para hacer peticiones a backend
     */
    configRutas: function (pParams) {
        // Url para editar un certificado
        this.urlBase = refineURL(); 
        this.urlSaveConfiguration = `${this.urlBase}/save`;
        this.urlShutdown = `${this.urlBase}/shutdown`;
        this.urlAddCustomLanguage = `${this.urlBase}/add-custom-language`;
        // Objeto donde se guardarán las opciones para su guardado
        this.Options = {};
    },  

    /*
    * Inicializar elementos de la vista
    * */
    config: function (pParams) {
        /* Inputs de Usuarios */
        this.EcosistemaSinBandejaSuscripciones = $("#EcosistemaSinBandejaSuscripciones");
        this.DatosDemograficosPerfil = $("#DatosDemograficosPerfil");
        this.PanelMensajeImportarContactos = $("#PanelMensajeImportarContactos");
        this.PerfilGlobalEnComunidadPrincipal = $("#PerfilGlobalEnComunidadPrincipal");
        this.PestanyaImportarContactosCorreo = $("#PestanyaImportarContactosCorreo");
        this.RegistroAutomaticoEcosistema = $("#RegistroAutomaticoEcosistema");
        this.SeguirEnTodaLaActividad = $("#SeguirEnTodaLaActividad");
        this.EcosistemaSinHomeUsuario = $("#EcosistemaSinHomeUsuario");
        this.RecibirNewsletterDefecto = $("#RecibirNewsletterDefecto"); 
        this.PerfilPersonalDisponible = $("#PerfilPersonalDisponible");
        this.MostrarGruposIDEnHtml = $("#MostrarGruposIDEnHtml");                        
        this.GenerarGrafoContribuciones = $("#GenerarGrafoContribuciones");
        this.ReiniciarAplicacion = $("#ReiniciarAplicacion");
        this.MantenerSesionActiva = $("#MantenerSesionActiva");
        this.NoEnviarCorreoSeguirPerfil = $("#NoEnviarCorreoSeguirPerfil");
        this.EdadLimiteRegistroEcosistema = $("#EdadLimiteRegistroEcosistema");
        this.UrlHomeConectado = $("#UrlHomeConectado");
        this.OntologiasNoLive = $("#OntologiasNoLive");
        this.InputDuracionCookieUsuario = $("#InputDuracionCookieUsuario");
        this.SelectDuracionCookieUsuario = $("#SelectDuracionCookieUsuario");
        /* Inputs de Sistemas */
        this.CodigoGoogleAnalyticsProyecto = $("#CodigoGoogleAnalyticsProyecto");
        this.DominiosPermitidosCORS = $("#DominiosPermitidosCORS");
        this.UrlsPropiasProyecto = $("#UrlsPropiasProyecto");
        this.Idiomas = $("#Idiomas");
        // Input de la web inicial
        this.txtUrlWeb = $("#txtUrlWeb");
        // Input de la web de proyectos
        this.txtUrlProyectosWeb = $("#txtUrlProjects");
        // radioButton para uso de proyectos publicos y privados
        this.rbUseSameProjectsUrl = $(".rbUseSameProjectsUrl");
        // Panel que contiene la Url de proyectos privados
        this.panelUrlPrivateProjects = $("#panelUrlPrivateProjects");
        /* Inputs de Redes Sociales */
        this.DominiosEmailLoginRedesSociales = $("#DominiosEmailLoginRedesSociales");  
        /* Datos del ecosistema */
        this.EcosistemaSinMetaproyecto = $("#EcosistemaSinMetaproyecto");
        
        // Radio Button para el idioma por defecto
        this.radioButtonIdiomas = $("input[type='radio'][name='defaultLanguage']")
        // Variable que guarda el idioma por defecto seleccionado
        this.defaultLanguage = $("input[name='defaultLanguage']:checked").data("language");
        // Checkbox con los diferentes idiomas
        this.checkBoxIdiomas = $(".languageOption");
        // Linea con los diferentes idiomas personalizados
        this.customLanguageClassName = "custom-language-row";
      
        // Desaparece por uso de checkbox this.ExtensionesImagenesCMSMultimedia = $("#ExtensionesImagenesCMSMultimedia");
        // Checkbox con los diferentes idiomas
        this.checkBoxExtensionesImagenesCmsMultimedia = $(".extensionImageOption");        
        // Desaparece por uso de checkbox this.ExtensionesDocumentosCMSMultimedia = $("#ExtensionesDocumentosCMSMultimedia");
        this.checkBoxExtensionesDocumentosCMSMultimedia = $(".extensionDocumentOption");
        this.panelAvisoReinicioAplicacionPlataforma = $("#panelAvisoReinicioAplicacionPlataforma");


        this.Copyright = $("#Copyright");
        /* Urls de configuración */
        this.UrlIntragnoss = $("#UrlIntragnoss");
        this.UrlBaseService = $("#UrlBaseService");
        this.DominioPaginasAdministracion = $("#DominioPaginasAdministracion");
        /* Comunidades excluidas de la personalización */
        this.ComunidadesExcluidaPersonalizacion = $("#ComunidadesExcluidaPersonalizacion");
        /* Versiones */
        this.VersionJSEcosistema = $("#VersionJSEcosistema");        
        this.VersionCSSEcosistema = $("#VersionCSSEcosistema");
        /* Login */        
        this.LoginUnicoPorUsuario = $("#LoginUnicoPorUsuario");
        this.LoginUnicoUsuariosExcluidos = $("#LoginUnicoUsuariosExcluidos");
        this.PerfilPersonalDisponible = $("#PerfilPersonalDisponible");                                        
        /* Comunidad */ 
        this.EnviarNotificacionesDeSuscripciones = $("#EnviarNotificacionesDeSuscripciones"); 
        this.AceptacionComunidadesAutomatica = $("#AceptacionComunidadesAutomaticas");       
        this.CargarIdentidadesDeProyectosPrivadosComoAmigos = $("#CargarIdentidadesDeProyectosPrivadosComoAmigos"); 
        /* Grafos */ 
        this.GrafoMetaBusquedaComunidades = $("#GrafoMetaBusquedaComunidades"); 
        // Botón para guardar la configuración                        
        this.btnSave = $("#btnSave");
        // Botón para abrir el modal para añadir un nuevo idioma personalizado
        this.btnAddLanguage = $("#btnAddLanguage");
        // Botón para añadir un nuevo idioma personalizado
        this.btnAddCustomLanguage = $("#btnAddCustomLanguage");
        // Botón para editar un idioma personalizado
        this.btnEditCustomLanguage = $("#btnEditCustomLanguage");
        // Botón para abrir el modal para editar un idioma personalizado
        this.btnEditCustomLanguageRowClassName = "btnEditCustomLanguageRow";
        // Botón para eliminar un idioma personalizado
        this.btnDeleteCustomLanguage = "btnDeleteCustomLanguage";
        /* Inputs del modal _add-language */
        this.inputClaveIdioma = $("#inputClaveIdioma");
        this.inputValorIdioma = $("#inputValorIdioma");
        // Lista con los idiomas personalizados
        this.customLanguageList = $("#customLanguageList");
        // Modal para añadir un idioma parsonalizado
        this.modalAddLanguage = $("#modal-add-language");
        // Nombre del idioma personalizado
        this.componentCustomLanguageNameClassName = "component-customLanguageName";

        // Por defecto se utilizarán las mismas URLS para proyectos públicios y privados
        this.useSameUrlPrivate = true;

        this.arrayCheckBoxLanguageDefaultValues = [];
        this.inicialListCustomLanguage = [];
        this.listCustomLanguage = [];
        this.listLanguageKeys = [];

        // APLICAR UN COMP
    },   

    /**
     * Configuración de eventos de elementos del Dom (Botones, Inputs...)     
     */
    configEvents: function (pParams) {
        const that = this;

        // Comportamientos del modal container 
        this.modalAddLanguage.on('show.bs.modal', (e) => {
            // Aparición del modal
        })
        .on('hidden.bs.modal', (e) => {
            // Vaciar el modal
            that.inputClaveIdioma.val("");
            that.inputValorIdioma.val("");
            that.inputClaveIdioma.removeAttr("disabled");
            that.btnEditCustomLanguage.addClass("d-none");
            that.btnAddCustomLanguage.addClass("d-none");
        });

        // Opción radioButton de utilizar la misma url para proyectos privados y públicos/restringidos
        this.rbUseSameProjectsUrl.off().on("click", function(){
            const radioButton = $(this);
            that.handleOptionUsarMismaWebsProyectosPublicosPrivados(radioButton);
        });

        // Botón para guardar los cambios
        this.btnSave.on("click", function(){
            that.handleObtenerdatos();
            that.handleSaveOpcionesPlataforma();
        });

        this.radioButtonIdiomas.on("click", function () {
            that.defaultLanguage = $("input[name='defaultLanguage']:checked").data("language");
        });

        this.checkBoxIdiomas.on("click", function () {
            that.comprobarCambiosEnIdiomas();
            let closestRadioButton = $(this).closest("ul").find("input[name='defaultLanguage']");
            if ($(this).is(":checked")) {
                closestRadioButton.removeAttr("disabled");
                // Si no hay idioma por defecto seleccionado se marca este como idioma por defecto
                if (that.defaultLanguage.length == 0) {
                    closestRadioButton.attr("checked", true);
                    that.defaultLanguage = closestRadioButton.data("language");
                }
            } else {
                if (closestRadioButton.is(":checked")) {
                    // Si tenia elegido este idioma por defecto, quitamos el checked de su radio button
                    closestRadioButton.removeAttr("checked");

                    // Se busca el idioma que este seleccionado mas próximo y se marca su radio button
                    let closestLanguageAvailable = $($("#platform-languages li.component-wrap input[type='checkbox']:checked")[0])
                    let newClosestRadioButton = closestLanguageAvailable.closest("ul").find("input[name='defaultLanguage']");
                    newClosestRadioButton.attr("checked", true);

                    // Se actualiza el idioma por defecto
                    that.defaultLanguage = newClosestRadioButton.data("language") == undefined ? "" : newClosestRadioButton.data("language") 
                }

                // Se deshabilita su radio button para que no pueda ser seleccionado hasta que vuelva a marcar el checkbox
                closestRadioButton.attr("disabled", true);
            };
        });

        // Botón para abrir el modal para añadir un nuevo idioma personalizado
        this.btnAddLanguage.on("click", function () {
            that.btnAddCustomLanguage.removeClass("d-none");
        });

        // Botón para añadir un nuevo idioma personalizado
        this.btnAddCustomLanguage.on("click", function () {
            that.handleAddCustomLaguage();
            that.comprobarCambiosEnIdiomas();
        });

        // Botón para editar un idioma personalizado
        this.btnEditCustomLanguage.on("click", function () {
            that.handleEditCustomLaguage();
            that.comprobarCambiosEnIdiomas();
        });

        // Botón para eliminar un idioma personalizado
        configEventByClassName(`${that.btnDeleteCustomLanguage}`, function (element) {
            const editButton = $(element);
            editButton.off().on("click", function () {
                that.handleDeleteCustomLanguage(editButton);
                that.comprobarCambiosEnIdiomas();
            });
        });

        // Botón para abrir el modal para editar un idioma personalizado
        configEventByClassName(`${that.btnEditCustomLanguageRowClassName}`, function (element) {
            const editButton = $(element);
            editButton.off().on("click", function () {
                that.handlePrepareModalForEditLanguage(editButton);
            });
        });
        
    },

    /**
     * Método para eliminar un idioma personalizado
     * @param {any} editButton boton que ha ejecutado este metodo
     */
    handleDeleteCustomLanguage: function (editButton) {
        const that = this;

        var customLanguageRowEdited = editButton.closest(`.${that.customLanguageClassName}`);
        var idiomaPersonalizado = customLanguageRowEdited.data("language");
        var key = idiomaPersonalizado.slice(0, 2);

        // Elimina la clave de la lista de claves
        that.listLanguageKeys = that.listLanguageKeys.filter(function (item) {
            return item !== key;
        });
        // Elimina el idoma personalizaco de la lista
        that.listCustomLanguage = that.listCustomLanguage.filter(function (item) {
            return item !== idiomaPersonalizado;
        });
        // Elimina el idioma personalizado de la vista
        customLanguageRowEdited.remove();
    },

    /**
     * Método para preparar el modal para mostrar los datos y conocer la fila del idioma que está siendo editada
     * @param {any} editButton boton que ha ejecutado este metodo
     * */
    handlePrepareModalForEditLanguage: function (editButton) {
        const that = this;

        that.customLanguageRowEdited = editButton.closest(`.${that.customLanguageClassName}`);

        that.inputClaveIdioma.attr('disabled', 'disabled');
        var claveValor = that.customLanguageRowEdited.attr("data-language").split("|");
        that.inputClaveIdioma.val(claveValor[0]);
        that.inputValorIdioma.val(claveValor[1]);

        that.btnEditCustomLanguage.removeClass("d-none");
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
     * Método que carga en el array los valores por defecto de los idiomas
     */
    loadInitialLanguageValues: function () {
        const that = this;
        $.each(that.checkBoxIdiomas, function () {
            const $this = $(this);
            const id = $this.attr('id'); //id del idioma
            const checked = $this.prop('checked');// $this.is(":checked"); //true o false
            const keyLanguage = $this.attr('data-language').slice(0, 2);
            that.arrayCheckBoxLanguageDefaultValues.push({ id: id, checked: checked });
            that.listLanguageKeys.push(keyLanguage);
        });

        // Comprobar custom language al cargar la página
        $.each($(`.${that.customLanguageClassName}`), function () {
            const $this = $(this);
            const customLanguage = $this.attr('data-language');
            that.inicialListCustomLanguage.push(customLanguage);
        });
    },

    /**
     * Método que comprueba si ha habido algún cambio en la configuracion de los idiomas. Si hay cambios muestra el panel de aviso
     */
    comprobarCambiosEnIdiomas: function () {
        var b = false; //no hay cambios
        const that = this;
        $.each(that.checkBoxIdiomas, function () {
            const $this = $(this);
            const id = $this.attr('id');
            const checked = $this.is(":checked");
            // Comprobar si hay algún cambio en las checkbox
            if (!b) {
                var checkedLanguageInitial = that.arrayCheckBoxLanguageDefaultValues.find(objeto => objeto.id === id); 
                if (checked != checkedLanguageInitial.checked) {
                    // Hay algun cambio - return
                    b = true;                   
                }
            }
            // Comprobar si hay algún cambio en los idiomas personalizados
            if (!b) {
                $.each(that.listCustomLanguage, function () {
                    if (!that.inicialListCustomLanguage.includes(this.toString())) {
                        b = true;
                    }
                });
            }
            if (!b) {
                $.each(that.inicialListCustomLanguage, function () {
                    if (!that.listCustomLanguage.includes(this.toString())) {
                        b = true;
                    }
                });
            }

        });
        // Controlar aparición del panel de alerta
        if (b) {
            that.panelAvisoReinicioAplicacionPlataforma.addClass("d-flex");
        }else {
            that.panelAvisoReinicioAplicacionPlataforma.removeClass("d-flex");
        }
    },

    /**
     * Metodo para añadir un nuevo idioma personalizado
     */
    handleAddCustomLaguage: function () {
        var error = false;

        const that = this;
        const inputClaveIdioma = that.inputClaveIdioma.val().trim();
        const inputValorIdioma = that.inputValorIdioma.val().trim();

        // Comprobar si el id tiene exactamente 2 caracteres
        if (inputClaveIdioma.length != 2) {
            mostrarNotificacion("error", "El id tiene que tener 2 caracteres exactamente");
            return;
        }

        // Comprobar si el id no existe ya
        $.each(that.listLanguageKeys, function () {
            if (this == inputClaveIdioma) {
                mostrarNotificacion("error", `El id '${inputClaveIdioma}' ya existe`);
                error = true;
                return;
            }
        });
        if (error) {
            return;
        }

        // Añadir el nuevo id a la lista de claves de idiomas
        that.listLanguageKeys.push(inputClaveIdioma);
        // Añadir el nuevo isoma a la lista de idiomas personalizados
        that.listCustomLanguage.push(`${inputClaveIdioma}|${inputValorIdioma}`);

        // Todo parece estar correcto
        that.getCustomLangueHtmlTemplate(inputClaveIdioma, inputValorIdioma);

        that.modalAddLanguage.modal('toggle');

    },

    /**
     * Metodo para editar un idioma personalizado
     */
    handleEditCustomLaguage: function () {
        const that = this;
        const inputClaveIdioma = that.inputClaveIdioma.val().trim();
        const inputValorIdioma = that.inputValorIdioma.val().trim();

        var idiomaPersonalizado = that.customLanguageRowEdited.attr("data-language");
        var claveValor = idiomaPersonalizado.split("|");
        var clave = claveValor[0];

        // Elimina la el idioma personalizado de la lista
        that.listCustomLanguage = that.listCustomLanguage.filter(function (item) {
            return item !== idiomaPersonalizado;
        });

        // Añada el idioma personalizado a la lista con los cambios realizados
        that.listCustomLanguage.push(`${inputClaveIdioma}|${inputValorIdioma}`);

        // Edita los cambios en la vista para el idioma personalizado
        that.customLanguageRowEdited.attr("data-language", `${clave}|${inputValorIdioma}`);
        that.customLanguageRowEdited.find(`.${that.componentCustomLanguageNameClassName}`).html(inputValorIdioma);

        that.modalAddLanguage.modal('toggle');
    },

    /**
    * Metodo para cargar el Html de un idioma personalizado en la vista
    */
    getCustomLangueHtmlTemplate: function (keyLanguage, valueLanguage) {
        const that = this;
        const dataPost = {
            customLanguage: `${keyLanguage}|${valueLanguage}`
        };

        // Mostrar loading
        loadingMostrar(); 
        
        GnossPeticionAjax(
            that.urlAddCustomLanguage,
            dataPost,
            true
        ).done(function (data) {
            // OK
            that.customLanguageList.append(data);
        }).always(function () {
            loadingOcultar();
        });
        
    },

    /**
     * Método para recoger los datos de la configuración para el guardado.
     */
    handleObtenerdatos: function(){
        const that = this;
        
        // Vaciado del objeto para su posterior construcción
        that.Options = {};

        that.Options['EcosistemaSinBandejaSuscripciones'] = that.EcosistemaSinBandejaSuscripciones.is(':checked');
        // Desaparece that.Options['EcosistemaSinContactos'] = that.EcosistemaSinContactos.is(':checked');
        // Desaparece - Dar valor por defecto si se utiliza that.Options['VersionFotoDocumentoNegativo'] = that.VersionFotoDocumentoNegativo.is(':checked');
        // Desaparece that.Options['CVUnicoPorPerfil'] = that.CVUnicoPorPerfil.is(':checked');
        that.Options['DatosDemograficosPerfil'] = that.DatosDemograficosPerfil.is(':checked'); 
        that.Options['EcosistemaSinMetaproyecto'] = that.EcosistemaSinMetaproyecto.is(':checked');
        that.Options['PanelMensajeImportarContactos'] = that.PanelMensajeImportarContactos.is(':checked');
        that.Options['PerfilGlobalEnComunidadPrincipal'] = that.PerfilGlobalEnComunidadPrincipal.is(':checked');
        that.Options['PestanyaImportarContactosCorreo'] = that.PestanyaImportarContactosCorreo.is(':checked');
        that.Options['RegistroAutomaticoEcosistema'] = that.RegistroAutomaticoEcosistema.is(':checked');
        that.Options['SeguirEnTodaLaActividad'] = that.SeguirEnTodaLaActividad.is(':checked');
        // Desaparece that.Options['UsarSoloCategoriasPrivadasEnEspacioPersonal'] = that.UsarSoloCategoriasPrivadasEnEspacioPersonal.is(':checked');
        that.Options['EcosistemaSinHomeUsuario'] = that.EcosistemaSinHomeUsuario.is(':checked');
        // Desaparece that.Options['NotificacionesAgrupadas'] = that.NotificacionesAgrupadas.is(':checked');
        that.Options['RecibirNewsletterDefecto'] = that.RecibirNewsletterDefecto.is(':checked');
        that.Options['PerfilPersonalDisponible'] = that.PerfilPersonalDisponible.is(':checked');
        that.Options['MostrarGruposIDEnHtml'] = that.MostrarGruposIDEnHtml.is(':checked');
        that.Options['GenerarGrafoContribuciones'] = that.GenerarGrafoContribuciones.is(':checked');
        that.Options['ReiniciarAplicacion'] = that.ReiniciarAplicacion.is(':checked');
        that.Options['MantenerSesionActiva'] = that.MantenerSesionActiva.is(':checked');
        that.Options['NoEnviarCorreoSeguirPerfil'] = that.NoEnviarCorreoSeguirPerfil.is(':checked');
        that.Options['LoginUnicoPorUsuario'] = that.LoginUnicoPorUsuario.is(':checked');
        that.Options['EnviarNotificacionesDeSuscripciones'] = that.EnviarNotificacionesDeSuscripciones.is(':checked');
        that.Options['EdadLimiteRegistroEcosistema'] = that.EdadLimiteRegistroEcosistema.val();
        // Desaparece that.Options['SegundosMaxSesionBloqueada'] = that.SegundosMaxSesionBloqueada.val();
        // Desaparece - Variable de entorno that.Options['TamanioPoolRedis'] = that.TamanioPoolRedis.val();
        // Desaparece that.Options['UbicacionLogs'] = that.UbicacionLogs.val();
        // Desaparece that.Options['UbicacionTrazas'] = that.UbicacionTrazas.val();        
        that.Options['CodigoGoogleAnalyticsProyecto'] = that.CodigoGoogleAnalyticsProyecto.val();
        that.Options['DominiosPermitidosCORS'] = that.DominiosPermitidosCORS.val();

        // Desaparece por UrlProyectosPublicos y URLProyectosPrivados that.Options['UrlsPropiasProyecto'] = that.UrlsPropiasProyecto.val();
       
        // Web para proyectos/comunidades privadas o públicas        
        that.Options['UrlProyectosPublicos'] = that.txtUrlWeb.val().trim();        
        that.Options['UrlProyectosPrivados'] = that.useSameUrlPrivate == true ? that.txtUrlWeb.val().trim() : that.txtUrlProyectosWeb.val().trim(),
        that.Options['UseSameUrlForPrivateProjects'] = that.useSameUrlPrivate;     

        // Desaparece that.Options['ConexionEntornoPreproduccion'] = that.ConexionEntornoPreproduccion.val();
        // Desaparece that.Options['CorreoSolicitudes'] = that.CorreoSolicitudes.val();
        // Desaparece that.Options['CorreoSugerencias'] = that.CorreoSugerencias.val();
        // Desaparece that.Options['DominiosSinPalco'] = that.DominiosSinPalco.val();
        // Desaparece that.Options['HashTagEntorno'] = that.HashTagEntorno.val();


        // Tener en cuenta que si no hay idiomas, mandar los "por defecto"
        //that.Options['Idiomas'] = that.Idiomas.val().trim().length == 0 ? "es|Español&&&en|English&&&pt|Portuguese&&&ca|Català&&&eu|Euskera&&&gl|Galego&&&fr|Français&&&de|Deutsch&&&it|Italiano" : that.Idiomas.val().trim();
        // Recorrer checkbox y construir la cadena de idiomas
        let idiomasValue = '';
        if (that.defaultLanguage != undefined) {
            idiomasValue = `${that.defaultLanguage}&&&`;
        }
        $.each(that.checkBoxIdiomas, function() {
            const $this = $(this);            
            if($this.is(":checked") && !idiomasValue.includes($this.data("language"))) {                
                idiomasValue += `${$this.data("language")}&&&`;
            }
        });

        $.each($(`.${that.customLanguageClassName}`), function () {
            const $this = $(this);
            if (!idiomasValue.includes($this.data("language"))) {
                idiomasValue += `${$this.data("language")}&&&`;
            }
        });
        // Eliminar los 3 últimos caracteres sobrantes (&&&) y establecer valor por defecto si no hay ninguno
        that.Options['Idiomas'] = idiomasValue.trim().length > 0 ? idiomasValue.slice(0,-3) : "";

        // Desaparece that.Options['LoginFacebook'] = that.LoginFacebook.val();
        // Desaparece that.Options['LoginGoogle'] = that.LoginGoogle.val();
        // Desaparece that.Options['LoginTwitter'] = that.LoginTwitter.val();
        //that.Options['NombreEspacioPersonal'] = that.NombreEspacioPersonal.val();
        that.Options['Copyright'] = that.Copyright.val().trim();
        // Desaparece that.Options['VisibilidadPerfil'] = that.VisibilidadPerfil.val();
        that.Options['OntologiasNoLive'] = that.OntologiasNoLive.val().trim();
        // Desaparece that.Options['ImplementationKey'] = that.ImplementationKey.val();
        that.Options['UrlHomeConectado'] = that.UrlHomeConectado.val().trim();
        // Desaparece that.Options['GoogleRecaptchaSecret'] = that.GoogleRecaptchaSecret.val();
        that.Options['DominiosEmailLoginRedesSociales'] = that.DominiosEmailLoginRedesSociales.val().trim();
        that.Options['DuracionCookieUsuario'] = that.InputDuracionCookieUsuario.val().trim() + that.SelectDuracionCookieUsuario.val().trim();
        
        // Imagenes compatibles. Recorrer checkbox
        // that.Options['ExtensionesImagenesCMSMultimedia'] = that.ExtensionesImagenesCMSMultimedia.val().trim().length == 0 ? ".jpg&&&.jpeg&&&.png&&&.gif" : that.ExtensionesImagenesCMSMultimedia.val().trim();
        let extensionesImagenesCmsMultimediaValue = "";
        $.each(that.checkBoxExtensionesImagenesCmsMultimedia, function() {
            const $this = $(this);            
            if($this.is(":checked")) {                
                extensionesImagenesCmsMultimediaValue += `${$this.data("extension")}&&&`;
            }
        });
        // Eliminar los 3 últimos caracteres sobrantes (&&&) y establecer valor por defecto si no hay ninguno
        that.Options['ExtensionesImagenesCMSMultimedia'] = extensionesImagenesCmsMultimediaValue.trim().length > 0 ? extensionesImagenesCmsMultimediaValue.slice(0,-3) : ".jpg&&&.jpeg&&&.png&&&.gif";
        
        // Documentos compatibles. Recorrer checkbox 
        // that.Options['ExtensionesDocumentosCMSMultimedia'] = that.ExtensionesDocumentosCMSMultimedia.val().trim().length == 0? ".pdf&&&.txt&&&.doc&&&.docx" : that.ExtensionesDocumentosCMSMultimedia.val().trim();

        
        let extensionesDocumentosCMSMultimediaValue = "";
        $.each(that.checkBoxExtensionesDocumentosCMSMultimedia, function() {
            const $this = $(this);            
            if($this.is(":checked")) {                
                extensionesDocumentosCMSMultimediaValue += `${$this.data("extension")}&&&`;
            }
        });
        // Eliminar los 3 últimos caracteres sobrantes (&&&) y establecer valor por defecto si no hay ninguno
        that.Options['ExtensionesDocumentosCMSMultimedia'] = extensionesDocumentosCMSMultimediaValue.trim().length > 0 ? extensionesDocumentosCMSMultimediaValue.slice(0,-3) : ".pdf&&&.txt&&&.doc&&&.docx";        

        // Desaparece that.Options['ipFTP'] = that.ipFTP.val();
        // Desaparece that.Options['puertoFTP'] = that.puertoFTP.val();
        // Desaparece that.Options['UrlContent'] = that.UrlContent.val();
        that.Options['UrlIntragnoss'] = that.UrlIntragnoss.val().trim();
        // Desaparece that.Options['UrlIntragnossServicios'] = that.UrlIntragnossServicios.val();
        that.Options['UrlBaseService'] = that.UrlBaseService.val().trim();
        that.Options['DominioPaginasAdministracion'] = that.DominioPaginasAdministracion.val().trim();
        that.Options['ComunidadesExcluidaPersonalizacion'] = that.ComunidadesExcluidaPersonalizacion.val().trim();
        that.Options['VersionJSEcosistema'] = that.VersionJSEcosistema.val().trim();
        that.Options['VersionCSSEcosistema'] = that.VersionCSSEcosistema.val().trim();
        // Desaparece that.Options['PasosAsistenteCreacionComunidad'] = that.PasosAsistenteCreacionComunidad.val();
        // Desaparece that.Options['ScriptGoogleAnalytics'] = that.ScriptGoogleAnalytics.val();
        // Desaparece that.Options['TipoCabecera'] = that.TipoCabecera.val();
        // Desaparece that.Options['TamanioPoolRedis'] = that.TamanioPoolRedis.val();
        // Desaparece that.Options['usarHTTPSParaDominioPrincipal'] = that.usarHTTPSParaDominioPrincipal.val();
        // Desaparece that.Options['GrafoMetaBusquedaRecursos'] = that.GrafoMetaBusquedaRecursos.val();
        // Desaparece  that.Options['GrafoMetaBusquedaPerYOrg'] = that.GrafoMetaBusquedaPerYOrg.val();
        that.Options['GrafoMetaBusquedaComunidades'] = that.GrafoMetaBusquedaComunidades.val().trim();


        /* ejemplo
        that.Options['CMSActivado'] = that.CMSActivado.is(':checked');
        that.Options['AdministracionSemanticaPermitido'] = that.AdministracionSemanticaPermitido.is(':checked');
        that.Options['AdministracionPaginasPermitido'] = that.AdministracionPaginasPermitido.is(':checked');
        that.Options['AdministracionVistasPermitido'] = that.AdministracionVistasPermitido.is(':checked');
        that.Options['AdministracionDesarrolladoresPermitido'] = that.AdministracionDesarrolladoresPermitido.is(':checked');
        that.Options['TextoInvariableTesauroSemantico'] = that.TextoInvariableTesauroSemantico.is(':checked');
        that.Options['VistasActivadas'] = that.VistasActivadas.is(':checked');

        if (that.VistasActivadas.is(':checked') && that.PersonalizacionIDVistas.length > 0)
        {
            that.Options['PersonalizacionIDVistas'] = that.PersonalizacionIDVistas.val();
        }
        */

        /*
        if ( that.NoUsarVistasDelEcosistema.length > 0) {
            that.Options['NoUsarVistasDelEcosistema'] = that.NoUsarVistasDelEcosistema.is(':checked');
            that.Options['TieneVistasEcosistema'] = 'true';
        }
        */


    },

    /**
     * Método para guardar los datos de la configuración de la plataforma
     */    
     handleSaveOpcionesPlataforma: function(){
        const that = this;

        // Mostrar loading
        loadingMostrar();       
            
        // Realizar petición para guardado
        GnossPeticionAjax(
        that.urlSaveConfiguration,
        that.Options,
        true
        ).done(function (data) {
            // OK Guardado
            if (data == "shutdown") {
                mostrarNotificacion("success", "Los cambios se han guardado correctamente y se va a proceder al reinicio de la aplicacion");
                setTimeout(function () {
                    // Realizar petición para apagar la aplicacion
                    GnossPeticionAjax(
                        that.urlShutdown,
                        true
                    ).done(function (data) {
                        // OK Guardado
                        mostrarNotificacion("success", data);
                    }).fail(function (data) {
                        // KO Guardado
                        mostrarNotificacion("error", data);
                    }).always(function () {
                        loadingOcultar();
                    });
                }, 5000);
            }
            else {
                mostrarNotificacion("success", "Los cambios se han guardado correctamente");
            }         
        }).fail(function (data) {
            // KO Guardado          
            mostrarNotificacion("error", data);
        }).always(function () {
            loadingOcultar();
        }); 
 

    },    
}

/**
 * Operativa de funcionamiento de Tipo de Contenidos y Permisos
 */
const operativaEstadoServicios = {
    
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

        // Mostrar los items según la selección
        that.handleShowServiceType();

        // Ejecutar peticiones para conocer el estado de los servicios
        that.handleCheckServices();
    },    

    /*
     * Inicializar elementos de la vista
     * */
    config: function (pParams) {
                                    
        // Botón para realizar comprobaciones de los servicios
        this.btnCheckServicesId = "btnCheckServices";
        // Tab item de cada uno de los servicios que hay disponibles
        this.tabServiciosItemClassName = "tabServiciosItem";
        // Cada una de las filas que contiene un servicio (Servicio genérico)
        this.serviceRowClassName = "service-row";
        // Cada una de las filas que contiene un servicio de tipo Web
        this.serviceWebRowClassName = "service-web";                
        // Cada una de las filas que contiene un servicio de tipo Database
        this.serviceDatabaseRowClassName = "service-database";        
        // Cada una de las filas que contiene un servicio de tipo Background
        this.serviceBackgroundRowClassName = "service-background"; 
        // Contenedor de los servicios
        this.serviceListId = "id-added-service-list";
        // Número/Contador de servicios visibles
        this.numResultadosServicios = $(".numResultados");
        // Buscador de servicios
        this.txtBuscarServicio = $("#txtBuscarServicio");
        // Panel de error que avisa de que no se ha podido borrar el recurso
        this.panelErrorDeleteResource = $("#panelErrorDeleteResource");
    },  
    
    /**
     * Configuración de las rutas de acciones necesarias para hacer peticiones a backend
     */
    configRutas: function (pParams) {
        this.urlBase = refineURL();
        this.urlComprobarServicioOntologias = `${this.urlBase}/comprobar-servicio-ontologias`;
        this.urlComprobarServicioInterno = `${this.urlBase}/comprobar-servicio-interno`;
        this.urlComprobarServicioDocumentos = `${this.urlBase}/comprobar-servicio-documentos`;
        this.urlComprobarApi = `${this.urlBase}/comprobar-api`;
        this.urlComprobarRelatedVirtuoso = `${this.urlBase}/comprobar-related-virtuoso`;
        this.urlComprobarSearchGraphGenerator = `${this.urlBase}/comprobar-search-graph-generator`;
        this.urlCrearRecurso = `${this.urlBase}/crear-recurso`;
        this.urlComprobarVirtuoso = `${this.urlBase}/comprobar-virtuoso`;
        this.urlComprobarReplicacionVirtuoso = `${this.urlBase}/comprobar-replicacion-virtuoso`;
        this.urlComprobarGeneradorMiniaturas = `${this.urlBase}/comprobar-generador-miniaturas`;
        this.urlComprobarAutocompleteApiLucene = `${this.urlBase}/comprobar-autocomplete-api-lucene`;
        this.urlComprobarVisitRegistry = `${this.urlBase}/comprobar-visit-registry`;
        this.urlComprobarVisitCluster = `${this.urlBase}/comprobar-visit-cluster`;
        this.urlEliminarRecurso = `${this.urlBase}/eliminar-recurso`;
        this.urlComprobarRedis = `${this.urlBase}/comprobar-redis`;
        this.urlComprobarRabbitMQ = `${this.urlBase}/comprobar-rabbitmq`;
        this.urlComprobarHaproxi = `${this.urlBase}/comprobar-haproxy`;
        
        this.urlComprobarAutocomplete =`${this.urlBase}/comprobar-autocomplete` ;
        this.urlComprobarDeploy = `${this.urlBase}/comprobar-deploy`;
        this.urlComprobarFacets = `${this.urlBase}/comprobar-facets` ;
        this.urlComprobarIdentityServer = `${this.urlBase}/comprobar-identityserver`;
        this.urlComprobarLabeler = `${this.urlBase}/comprobar-labeler` ;
        this.urlComprobarLogin = `${this.urlBase}/comprobar-login`;
        this.urlComprobarOauth = `${this.urlBase}/comprobar-oauth`;
        this.urlComprobarResults = `${this.urlBase}/comprobar-results`;
        this.urlComprobarCacheRefresh = `${this.urlBase}/comprobar-cacherefresh`;
        this.urlComprobarCommunityWall = `${this.urlBase}/comprobar-communitywall`;
        this.urlComprobarDistributor = `${this.urlBase}/comprobar-distributor`;
        this.urlComprobarMail = `${this.urlBase}/comprobar-mail`;
        this.urlComprobarSocialCacheRefresh = `${this.urlBase}/comprobar-socialcacherefresh`;
        this.urlComprobarUserWall = `${this.urlBase}/comprobar-userwall`;
        this.urlComprobarReplicacionBidireccional = `${this.urlBase}/comprobar-replicacion-bidireccional`;
        this.urlComprobarAutocompleteGenerator = `${this.urlBase}/comprobar-autocomplete-generator`;


    },    

    /**
     * Configuración de eventos de elementos del Dom (Botones, Inputs...)     
     */
    configEvents: function (pParams) {
        const that = this;            

        // Botón para comprobar el estado de las instalaciones de los servicios
        configEventById(`${that.btnCheckServicesId}`, function(element){
            const $input = $(element);
            $input.off().on("click", function () {
                that.handleDeleteServiceStatus();
                that.triggerEvents();
            });	    
        });  


        // Cada uno de los tabs para mostrar el servicio deseado en base al item activo
        configEventByClassName(`${that.tabServiciosItemClassName}`, function(element){
            const $copyButton = $(element);
            $copyButton.off().on("click", function(){                 
                setTimeout(function(){
                    that.handleShowServiceType();                
                },250);                            
            });	                        
        });

        // Input para realizar búsquedas de servicios
        this.txtBuscarServicio.off().on("keyup", function (event) {
            clearTimeout(that.timer);
            that.timer = setTimeout(function () {                
                that.handleSearchService();                                                         
            }, 500);
        });        
    },

    /**
     * Metodo para borrar los estados de los servicios en la vista
     */
    handleDeleteServiceStatus: function () {
        $("#id-added-service-list").find(".okStatus").removeClass("d-flex");
        $("#id-added-service-list").find(".koStatus").removeClass("d-flex");
        $("#id-added-service-list").find(".okStatus").find(".material-icons").attr("data-icon", "");
        $("#id-added-service-list").find(".okStatus").find(".material-icons").text("");
        $("#id-added-service-list").find(".koStatus").find(".material-icons").attr("data-icon", "");
        $("#id-added-service-list").find(".koStatus").find(".material-icons").text("");
        $("#id-added-service-list").find(".statusDescription").text("");
        $("#id-added-service-list").find(".loadingStatus").removeClass("d-none");
    },

    /**
    * Método para mostrar el nº de servicios existentes que están siendo visualizados (Web, Database, Background)
    */
    handleCheckNumberOfServices: function(){        
        const that = this;
        const numberVisibleServices = $(`#${that.serviceListId}`).find(`.${that.serviceRowClassName}:not(.d-none)`).length;        
        // Mostrar el nº de items                
        that.numResultadosServicios.html(numberVisibleServices);
    },         

    /**
     * Método para mostrar los servicios dependiendo del tab que esté activo
     */
    handleShowServiceType: function(){
        const that = this;
        // Servicios a mostrar según el item activo
        let servicesToShow = undefined;
        // Todos los servicios
        const serviceItems = $(`.${that.serviceRowClassName}`);
        // Servicios web
        const webServiceItems = $(`.${that.serviceWebRowClassName}`);
        // Servicios Database
        const databaseServiceItems = $(`.${that.serviceDatabaseRowClassName}`);
        // Servicios background
        const backgroundServiceItems = $(`.${that.serviceBackgroundRowClassName}`);

        // Obtener qué servicio está activo
        const tabActiveService = $(`.${that.tabServiciosItemClassName}.active`);
        const tabActiveServiceType = tabActiveService.data("service");

        // Determinar los servicios según el servicio activo
        switch (tabActiveServiceType) {
            case "web":
                servicesToShow = webServiceItems;
            break;
            case "database":
                servicesToShow = databaseServiceItems;
            break;        
            case "background":
                servicesToShow = backgroundServiceItems;
            break;                      
        
            default:
                break;
        }        

        // Ocultar todos los servicios        
        $.each(serviceItems, function(index){
            const serviceRow = $(this);        
            // Ocultar la fila
            serviceRow.addClass("d-none");                       
        });

        // Mostrar los servicios activos
        $.each(servicesToShow, function(index){
            const serviceRow = $(this);        
            // Ocultar la fila
            serviceRow.removeClass("d-none");                       
        });

        // Actualizar el contador para mostrar los servicios visibles
        that.handleCheckNumberOfServices();
    },    

    /**
     * Método para comprobar el estado de los servicios
     */
    handleCheckServices: function () {
        const that = this;
        var json = "";

        // Comprobar Api
        that.handleCheckServiceStatus(that.urlComprobarApi);
        // Comprobar servicio Archivos
        that.handleCheckServiceStatus(that.urlComprobarServicioOntologias);
        // Comprobar servicio Documentos
        that.handleCheckServiceStatus(that.urlComprobarServicioDocumentos);
        // Comprobar servicio Interno
        that.handleCheckServiceStatus(that.urlComprobarServicioInterno);
        // Comprobar servicio RelatedVirtuoso
        that.handleCheckServiceStatus(that.urlComprobarRelatedVirtuoso);
        // Comprobar servicio AutocompleteApiLucene
        that.handleCheckServiceStatus(that.urlComprobarAutocompleteApiLucene);
        // Comprobar servicio ReplicaciónVirtuoso
        that.handleCheckServiceStatus(that.urlComprobarReplicacionVirtuoso);
        // Comprobar servicio ThumbnailGenerator
        that.handleCheckServiceStatus(that.urlComprobarGeneradorMiniaturas);
        // Comprobar servicio Visit Registry
        that.handleCheckServiceStatus(that.urlComprobarVisitRegistry); 
        // Comprobar servicio Visit Cluster
        that.handleCheckServiceStatus(that.urlComprobarVisitCluster); 
        // Comprobar servicio Search Graph Generator
        that.handleCheckServiceStatus(that.urlComprobarSearchGraphGenerator);
        // Comprobar servicio Redis
        that.handleCheckServiceStatus(that.urlComprobarRedis);
        // Comprobar servicio Virtuoso
        that.handleCheckServiceStatus(that.urlComprobarVirtuoso);
        // Comprobar servicio RabbitMQ
        that.handleCheckServiceStatus(that.urlComprobarRabbitMQ);
        // Comprobar servicio Haproxy
        that.handleCheckServiceStatus(that.urlComprobarHaproxi);

        
        // Comprobar servicio Autocomplete
        that.handleCheckServiceStatus(that.urlComprobarAutocomplete);
        // Comprobar servicio Deploy
        that.handleCheckServiceStatus(that.urlComprobarDeploy);
        // Comprobar servicio Facets
        that.handleCheckServiceStatus(that.urlComprobarFacets);
        // Comprobar servicio IdentityServer
        that.handleCheckServiceStatus(that.urlComprobarIdentityServer);
        // Comprobar servicio Labeler
        that.handleCheckServiceStatus(that.urlComprobarLabeler);
        // Comprobar servicio Login
        that.handleCheckServiceStatus(that.urlComprobarLogin);
        // Comprobar servicio Oauth
        that.handleCheckServiceStatus(that.urlComprobarOauth);
        // Comprobar servicio Results
        that.handleCheckServiceStatus(that.urlComprobarResults);
        
        // Comprobar servicio CacheRefresh
        that.handleCheckServiceStatus(that.urlComprobarCacheRefresh);
        // Comprobar servicio CommunityWall
        that.handleCheckServiceStatus(that.urlComprobarCommunityWall);
        // Comprobar servicio Distributor
        that.handleCheckServiceStatus(that.urlComprobarDistributor);
        // Comprobar servicio Mail
        that.handleCheckServiceStatus(that.urlComprobarMail);
        // Comprobar servicio SocialCacheRefresh
        that.handleCheckServiceStatus(that.urlComprobarSocialCacheRefresh);
        // Comprobar servicio UserWall
        that.handleCheckServiceStatus(that.urlComprobarUserWall);
        // Comprobar servicio Replicacion Bidireccional
        that.handleCheckServiceStatus(that.urlComprobarReplicacionBidireccional);
        // Comprobar servicio Autocomplete Generator
        that.handleCheckServiceStatus(that.urlComprobarAutocompleteGenerator);

        
        //// Comprobar Rabbit y servicio SearchGraphGenerator
        //GnossPeticionAjax(
        //    that.urlComprobarRabbitSearchGraphGenerator,
        //    null,
        //    true
        //).done(function (data) {
        //    json = JSON.parse(data);
        //    that.printServiceStatus(json);
        //});
        //// Comprobar servicio Redis
        //that.handleCheckServiceStatus(that.urlComprobarRedis);
    }, 

    /**
     * Método que comprobaro el estado del servicio segun la url pasada
     * @param {string} url: Url a la que se realiza la petición para consultar el estado del servicio
     * @param {any} options: Parametros que necesita el méntodo que comprueba el estado del servicio
     */
    handleCheckServiceStatus: function (url) {
        const that = this;

        GnossPeticionAjax(
            url,
            null,
            true
        ).done(function (data) {
            var json = JSON.parse(data);
            that.printServiceStatus(json);
        });
    },

    /**
     * Método que pinta en la vista el estado del servicio
     * @param {any} data: modelo que contiene los datos para mostrar el estado del servicio en la vista
     */
    printServiceStatus: function (data) {
        $(`#${data.service}`).find(`.${data.status}`).addClass("d-flex");
        $(`#${data.service}`).find(`.${data.status}`).find(".material-icons").attr("data-icon", `${data.icon}`);
        $(`#${data.service}`).find(`.${data.status}`).find(".material-icons").text(data.icon);
        $(`#${data.service}`).find(".statusDescription").html(data.description.replace(/\n/g, '<br/>'));
        $(`#${data.service}`).find(".loadingStatus").addClass("d-none");
    },
     

    /**
     * Metodo para eliminar un recurso
     * @param {string} recursoID: recurso a eliminar
     */
    handleDeleteResource: async function (recursoID) {
        const that = this;

        GnossPeticionAjax(
            that.urlEliminarRecurso + "?recursoID=" + recursoID,
            null,
            true
        ).done(function (data) {
            that.panelErrorDeleteResource.html(data);
        });
    },

    /**
     * Método para realizar búsquedas de servicios
     */        
    handleSearchService: function(){
        const that = this;
       
        let cadena = this.txtBuscarServicio.val();
        // Eliminamos posibles tildes para búsqueda ampliada
        cadena = cadena.normalize('NFD').replace(/[\u0300-\u036f]/g, "").toLowerCase();
        
        // Cada una de las filas que muestran la página
        const rowServices = $(`#${that.serviceListId}`).find(`.${that.serviceRowClassName}`).not('.d-none');

        if (cadena.trim().length == 0){
            that.handleShowServiceType();
            return;
        }

        // Buscar dentro de cada fila       
        $.each(rowServices, function(index){
            const rowService = $(this);
            // Seleccionamos el nombre del servicio y quitamos caracteres extraños, tiles para poder hacer bien la búsqueda
            const serviceName = $(this).find(".component-name").html().trim().normalize("NFD").replace(/[\u0300-\u036f]/g, "").toLowerCase();

            if (serviceName.includes(cadena)){
                // Mostrar fila resultado y sus respectivos padres
                rowService.removeClass("d-none");                
            }else{
                // Ocultar fila resultado
                rowService.addClass("d-none");
            }            
        });        
    },
    
    
}

/**
 * Operativa de funcionamiento Administrar Servicios Externos
 */
const operativaEventosExternos = {
    /**
     * Inicializar operativa
     */
    init: function (pParams) {
        this.pParams = pParams;
        this.config(pParams);
        this.configEvents();
        this.configRutas();
    },

    configRutas: function () {
        // Url para editar un certificado
        this.urlBase = refineURL();

        // Url para guardar los cambios de un evento
        this.urlUpdateExternalEvent = `${this.urlBase}/save`

        // Url para habilitar/deshabilitar la configuracion de proyecto
        this.urlUpdateExternalEventConfig = `${this.urlBase}/save-config`

        this.urlUpdateCredentials = `${this.urlBase}/save-creds`
    },

    config: function (pParams) {
        // Boton que abre el modal de confirmacion
        this.btnUpdateExternalEventClassName = 'btnUpdateExternalEvent'
        // Boton para confirmar la actualizacion de una traza
        this.btnConfirmUpdateExternalEventClassName = 'btnConfirmUpdateExternalEvent';
        // Boton para actualizar la configuracion de eventos por proyecto
        this.btnUpdateExternalEventConfigIdName = 'btnUpdateExternalEventProjectConfig';
        // Boton para actualizar las credenciales de rabbit
        this.btnGuardarCredencialesIdName = 'btnGuardarCredenciales';

        // Id modal habilitar evento
        this.idEnableEventModal = 'modal-enable-evento-externo';
        // Id modal deshabilitar evento
        this.idDisableEventModal = 'modal-disable-evento-externo';
        // Id del input de usuario de rabbit
        this.idUsuarioRabbitExterno = 'pUsuarioRabbitExterno';
        // Id del input de contraseña de rabbit
        this.idPasswordRabbitExterno = 'pPasswordRabbitExterno'

        // Nombre del servicio actual
        this.externalEventNameClassName = 'externalEventName';

        // Clase de fila de evento
        this.externalEventRowClassName = 'external-event-row';

        // Flags para guaradr
        // Nombre del evento a actualizar
        this.externalEventName = undefined;
    },

    configEvents: function (pParams) {
        const that = this;

        // Boton de confirmacion de actualizacion de estado de un evento
        configEventByClassName(`${that.btnConfirmUpdateExternalEventClassName}`, function (element) {
            const $btnConfirmUpdateExternalEventClassName = $(element);
            $btnConfirmUpdateExternalEventClassName.off().on("click", function () {
                that.handleUpdateEvent();
            });
        });   

        // Boton para guardar el evento selccionado
        configEventByClassName(`${that.btnUpdateExternalEventClassName}`, function (element) {
            const $btnUpdateExternalEvent = $(element);
            $btnUpdateExternalEvent.off().on("click", function () {
                var filaEvento = $btnUpdateExternalEvent.closest(`.${that.externalEventRowClassName}`);
                // Nombre del evento que se va a actualizar              
                // Ejemplo: Eventos de XXXX -> Necesitamos la ultima parte [XXXX]
                that.externalEventName = filaEvento.find(`.${that.externalEventNameClassName}`).attr('value');
            });
        }); 

        // Boton para habilitar/deshabilitar la configuracion de eventos de proyecto
        configEventById(`${that.btnUpdateExternalEventConfigIdName}`, function (element) {
            const $btnUpdateExternalEventoConfig = $(element);
            $btnUpdateExternalEventoConfig.off().on("click", function () {
                that.handleUpdateConfig();
            });
        }); 
        // Boton para guardar las nuevas credenciales 
        configEventById(`${that.btnGuardarCredencialesIdName}`, function (element) {
            const $btnGuardarCredenciales = $(element);
            $btnGuardarCredenciales.off().on("click", function () {
                that.checkInputs();
            });
        })
    },
    handleUpdateEvent: function () {
        const that = this;

        // Mostrado de loading                   
        loadingMostrar();

        let dataPost = {
            pEvento: that.externalEventName
        }

        // Realizar la petición para actualizar el campo
        GnossPeticionAjax(
            that.urlUpdateExternalEvent,
            dataPost,
            true
        ).done(function (data) {
            mostrarNotificacion("success", "Se ha actualizado el evento seleccionado");
        }).fail(function (data) {
            mostrarNotificacion("error",data)
        }).always(function () {
            loadingOcultar();
            location.reload();
        })
    },

    handleUpdateConfig: function () {
        const that = this;

        // Mostrado de loading                   
        loadingMostrar();

        // Realizar la petición para actualizar el campo
        GnossPeticionAjax(
            that.urlUpdateExternalEventConfig,
            null,
            true
        ).done(function (data) {
            mostrarNotificacion("success", "Se ha actualizado el evento seleccionado");
        }).fail(function (data) {
            mostrarNotificacion("error", data)
        }).always(function () {
            loadingOcultar();
            location.reload();
        })
    },

    checkInputs: function () {
        const that = this;
        let inputNombre = $(`#${that.idUsuarioRabbitExterno}`).val();

        if (inputNombre == undefined || inputNombre.length == 0) {
            mostrarNotificacion("error", "El nombre de usuario no puede estar vacio");
            return
        }

        let inputPassword = $(`#${that.idPasswordRabbitExterno}`).val();

        if (inputPassword == undefined || inputPassword.length == 0) {
            mostrarNotificacion("error", "La contraseña no puede estar vacio");
            return
        }

        var data = {
            Usuario: inputNombre,
            Password: inputPassword
        }

        that.handleUpdateCredentials(data);
    },

    handleUpdateCredentials: function (data) {
        const that = this;

        // Mostrado de loading                   
        loadingMostrar();

        // Realizar la petición para actualizar el campo
        GnossPeticionAjax(
            that.urlUpdateCredentials,
            data,
            true
        ).done(function (data) {
            mostrarNotificacion("success", "Se han actualizado las credenciales correctamente");
        }).fail(function (data) {
            mostrarNotificacion("error", data)
        }).always(function () {
            loadingOcultar();
            location.reload();
        })
    }
}