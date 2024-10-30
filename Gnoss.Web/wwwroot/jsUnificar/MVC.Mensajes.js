var vistaCompactadaMensajes = {
    idCompactado: '.compacView',
    idResource: '.resource',
    idTitulos: '.title',
    init: function () {
        if (!this.condicion()) return;
        this.condicion();
        this.config();
        this.comportamiento();
    },
    condicion: function () {
        var condicion = false;
        // Cambiado por nuevo Front
        //this.listado = $('#col02 ' + this.idCompactado);
        this.listado = $(body).find('#panelResultados');
        if (this.listado.length > 0) condicion = true;
        return condicion;
    },
    config: function () {
        this.recursos = this.listado.find('.resource');
        return;
    },
    ajusteTitulo: function (titulo) {
        var titulo = titulo;
        var enlace = titulo.find('h4 a');
        var texto = enlace.text();
        console.log(texto);
        if (texto.length < 30) return;
        var recortado = texto.substring(0, 30) + ' ...';
        enlace.text(recortado);
        enlace.attr('title', texto);
        return;
    },
    ajusteContenido: function (contenido) {
        var contenido = contenido;
        contenido.hide();
        return;
    },
    ajusteInformacion: function (informacion) {
        var informacion = informacion;
        var divs = informacion.find('div');
        var de = $(divs[0]);
        var para = $(divs[1]);
        var fecha = para.next();
        //para.hide();
        de.addClass('mensajeDe');
        fecha.addClass('mensajeFecha').show();
        var enlace = de.find('a');
        var texto = enlace.text();
        if (texto.length < 18) return;
        var recortado = texto.substring(0, 18) + ' ...';
        enlace.text(recortado);
        enlace.attr('title', texto);
        return;
    },
    ajusteAcciones: function (acciones) {
        var acciones = acciones;
        acciones.hide();
        return;
    },
    comportamiento: function () {
        var that = this;
        this.recursos.each(function (indice) {
            var recurso = $(this);
            var titulo = recurso.find('.title');
            var contenido = recurso.find('.content');
            var informacion = recurso.find('.utils-2');
            var acciones = recurso.find('.acciones');
            that.ajusteTitulo(titulo);
            //            that.ajusteContenido(contenido);
            //            that.ajusteInformacion(informacion);
            //            that.ajusteAcciones(acciones);
        });
        return;
    }
}

var listadoMensajesMostrarAcciones = {
    idcompactview: '.compactView',
    idmensaje: '.resource',
    idutils: '.utils-2',
    idacciones: '.acciones',
    init: function () {
        this.config();
        // Cambiado por nuevo front
        // if (this.listado.size() > 0) this.comportamiento();
        if (this.listado.length > 0) this.comportamiento();
    },
    config: function () {
        this.listado = $(body).find('#panelResultados');
        this.mensajes = this.listado.find(this.idmensaje);
        return;
    },
    comportamiento: function () {
        var that = this;
        this.mensajes.each(function (indice) {
            var mensaje = $(this);
            var utils = mensaje.find(that.idutils);
            var acciones = mensaje.find(that.idacciones);
            mensaje.hover(
				function () {
				    $(this).addClass('over');
				    utils.hide();
				    acciones.show();
				},
				function () {
				    $(this).removeClass('over');
				    utils.show();
				    acciones.hide();
				}
			);
        });
        return;
    }
}
$(function () {
    //15.10.12
    vistaCompactadaMensajes.init();
    //mecanismoVistaCompactada.init();
    listadoMensajesMostrarAcciones.init();
});


/**
 * Clase jquery para poder gestionar la mensajería de GNOSS.
 * Esta operativa se ejecuta cuando se accede a la sección de mensajes de Gnoss.
 * 
 * */
const operativaMensajeriaGnoss = {
    /**
     * Acción para inicializar elementos y eventos
     */
    init: function (pParams) {
        this.config(pParams);
        this.configEvents();
    },
    /*
     * Opciones de configuración de la vista
     * */
    config: function (pParams) {
        operativaCrossDomainCookies.init();
        // Inicialización de botones (Select para selección de mensajes)        
        this.messageActionSelectAll = $(`#${pParams.actionButtons.idMessageActionSelectAll}`);
        this.messageActionSelectNone = $(`#${pParams.actionButtons.idMessageActionSelectNone}`);
        this.messageActionSelectRead = $(`#${pParams.actionButtons.idMessageActionSelectRead}`);
        this.messageActionSelectNotRead = $(`#${pParams.actionButtons.idMessageActionSelectNotRead}`);

        // Botón para eliminación múltiple de mensajes desde el listadod de mensajes
        this.deleteMultiplesMensajes = $("#deleteMultiplesMensajes");

        // Inicialización de parámetros (Url y Mensaje confirmar eliminar)
        this.urlBorradoMensajes = pParams.urlBorradoMensajes;
        this.msgConfirmacionEliminar = pParams.msgConfirmacionEliminar;
        // Por defecto el modal-container para selección borrado múltiple de mensajes no mostraría el mensaje para borrado múltiple, solo cuando se pulse sobre
        // el icono para borrado múltiple (EliminarMensajes)
        this.showDeleteMessageModalMessage = false
        // Nº de correos que se han seleccionado (Ej: se utiliza para borrar múltiples correos seleccionados)
        this.numCorreosSeleccionadosParaBorrar = 0;
        // Lista de correos seleccionados (Ej: Se utiliza para borrar múltiples correos seleccionados)
        this.correosSeleccionados = "";
        

        // Carga inicial de mensajes (Sólo para bandeja de mensajes)
        if (pParams.loadMessages == true) { this.FiltrarBandejaMensajes(ObtenerHash2()) };
    },

    /**
    * Configuración de los eventos 
    */
    configEvents: function () {
        // Referencia al objeto
        const that = this;

        // Seleccionar todos los mensajes
        this.messageActionSelectAll.click(function () {            
            that.MarcarTodosCorreos(true, true);
        });

        // Deseleccionar todos los mensajes
        this.messageActionSelectNone.click(function () {            
            that.MarcarTodosCorreos(false, false);
        });

        // Seleccionar los mensajes leídos
        this.messageActionSelectRead.click(function () {            
            that.MarcarTodosCorreos(true, false);
        });

        // Seleccionar los mensajes No leídos
        this.messageActionSelectNotRead.click(function () {           
            that.MarcarTodosCorreos(false, true);
        });

        // Borrado múltiple de mensajes
        this.deleteMultiplesMensajes.click(function () {
            that.EliminarCorreos();
        });

        // Cuando se muestre el modal container (Borrado múltiple de mensajes, mostrar el aviso de Sí o No)
        $('#modal-container').on('show.bs.modal', function () {
            that.showDeleteMessageModalMessageInfo();
        });
    },

    /**
    * Función para filtrar y mostrar los mensajes una vez la página ha sido cargada
    */
    FiltrarBandejaMensajes: function (filtro) {
        filtro = filtro.replace(/&/g, '|');
        //cada vez que entro limpio div id="divContexto" del contexto anterior
        $('#divContexto').html('');
        $('#divContenedorContexto').css('display', 'none');

        const url = document.location.href;
        const filtrar = (url.indexOf('/redirect#nuevo') == -1);

        if (filtrar) {
            if (filtro != '') {
                SubirPagina();

                var esListado = filtro.indexOf('recibidos') == 0 || filtro.indexOf('enviados') == 0 || filtro.indexOf('eliminados') == 0;

                if (esListado) {
                    MostrarUpdateProgress();

                    $('#panListado').css('display', '');
                    $('#panVerCorreo').css('display', 'none');
                    $('#panEnviarCorreo').css('display', 'none');

                    var tipoBandeja = filtro;
                    if (tipoBandeja.indexOf('|') > -1) {
                        tipoBandeja = tipoBandeja.substr(0, tipoBandeja.indexOf('|'));
                    }
                    $('.tipoBandeja').val(tipoBandeja);

                    switch (tipoBandeja) {
                        case 'recibidos':
                            $('#litTituloPag').html(mensajes.recibidos);
                            break;
                        case 'enviados':
                            $('#litTituloPag').html(mensajes.enviados);
                            break;
                        case 'eliminados':
                            $('#litTituloPag').html(mensajes.eliminados);
                            break;
                    }

                    if (tipoBandeja != 'recibidos') {
                        $($('#cmbOrdenarPor').children()[1]).css('display', 'none');
                    }

                    EstablecerContadorMensajesNuevos(0);

                    FiltrarPorFacetasGenerico(filtro);
                }
                else {
                    var filtrosFacetas = $('.tipoBandeja').val();

                    filtrosPeticionActual = filtrosFacetas;
                    MontarFacetas(filtrosFacetas, false, 1, '#divFac', 'dce:type');
                }
            }
            else {
                if ($('.tipoBandeja').val() == "") {
                    $('.tipoBandeja').val('recibidos');
                }

                history.pushState('', 'New URL: ' + $('.tipoBandeja').val(), '?' + $('.tipoBandeja').val());
                this.FiltrarBandejaMensajes(ObtenerHash2());
            }
        }
    },

    ReiniciarBandejaMensajes: function (pValorFaceta) {
        if (pValorFaceta == 'Entrada') {
            $('.tipoBandeja').val('recibidos');
        }
        else if (pValorFaceta == 'Enviados') {
            $('.tipoBandeja').val('enviados');
        }
        else if (pValorFaceta == 'Eliminados') {
            $('.tipoBandeja').val('eliminados');
        }
        var url = document.location.href;
        if (url.indexOf('?') != -1) {
            url = url.substring(0, url.indexOf('?'));
        }

        document.location.href = url + '?' + $('.tipoBandeja').val();
    },

    /**
     * Marca los corres como leídos o no leídos
     * @param {bool} leidos: Seleccionar los mensajes leídos como el valor bool
     * @param {any} noLeidos: Seleccionar los mensajes no leídos como el valor bool
     */
    MarcarTodosCorreos: function (leidos, noLeidos) {
        $.each($('input.ListaMensajesCheckBox:checkbox'), function () {
            if (leidos && noLeidos) {
                this.checked = true;
            } else if (!leidos && !noLeidos) {
                this.checked = false;
            } else if ($(this).parents('.no-leido').length > 0) {
                if (leidos) {
                    this.checked = false;
                } else {
                    this.checked = true;
                }
            } else {
                if (leidos) {
                    this.checked = true;
                } else {
                    this.checked = false;
                }
            }
        });
    },

    /**
     * Acción para eliminar múltiples mensajes seleccionados desde la lista.
     * */
    EliminarCorreos: function () {
        const that = this;       
        
        $.each($('input.ListaMensajesCheckBox:checkbox'), function () {
            if (this.checked) {
                that.correosSeleccionados += this.id.substr(4) + ',';
                that.numCorreosSeleccionadosParaBorrar++;
            }
        });

        // Mostrar popUp para indicar si se desean eliminar los mensajes
        if (this.numCorreosSeleccionadosParaBorrar > 0) {
            // Queremos que se muestre el modal con la acción programada para eliminar múltiples mensajes
            this.showDeleteMessageModalMessage = true
            // Mostrar el modal
            $('#modal-container').modal('show');
        }
    },

    /**
     * Acción que se ejecutará una vez se pulse en la papelera para eliminar múltiples mensajes.
     * Se muestra el modal desde EliminarCorreos, y desde aquí se llamará para construir el panel de Sí o No.
     * Sólo se desea mostrar el modal si se ha llamado desde "Eliminación múltiple de mensajes".
     * */
    showDeleteMessageModalMessageInfo: function () {
        // Comprobar si se ha mostrado el modal-container seleccionando múltiples mensajes para borrar. En caso contrario, no hacer nada
        const that = this;
        let titulo = ""
        let mensaje = ""

        // Configuración del objeto a mandar para eliminación de mensajes
        const params = {
            listMessages: this.correosSeleccionados,
            profileBox: $('.tipoBandeja').val()
        }
        

        if (this.showDeleteMessageModalMessage == true) {

            if (this.numCorreosSeleccionadosParaBorrar > 1) {
                titulo = `Eliminar ${this.numCorreosSeleccionadosParaBorrar} mensajes seleccionados`;
                mensaje = `¿Deseas eliminar los ${this.numCorreosSeleccionadosParaBorrar} mensajes seleccionados?`
            } else {
                titulo = "Eliminar mensaje";
                mensaje = `¿Deseas eliminar el mensaje seleccionado?`;
            }
            AccionFichaPerfil(titulo,
                'Sí',
                'No',
                mensaje,
                '',
                function () {                                                            
                    EliminarCorreoSeleccionado(that.correosSeleccionados)
                },
                ''
            );                                        
            // Volvemos a dejarlo por defecto
            this.showDeleteMessageModalMessage = false;        
        }
        // Restablecer num de mensajes seleccionados
        this.numCorreosSeleccionadosParaBorrar = 0;        
    },
};

/**
 * Funcionalidad de Bandeja de comentarios
 * */
/**
 * Método para marcar un comentario leido. Se ejecuta al hacer click en el botón de "Marcar comentario como Leído" desde Bandeja de Comentarios
 * @param {any} comentarioID
 */
function MarcarComentarioLeidoPorID(comentarioID) {
    MostrarUpdateProgress();
    const params = {
        listComments: comentarioID
    }
    GnossPeticionAjax(`${location.href}/markRead`, params, false).done(function () {
        //$("#comment_" + comentarioID).find("div.group.acciones").remove();
        $(`#${comentarioID}`).removeClass("no-leido")
        OcultarUpdateProgress();
    })
}


/**
 * Clase para gestionar el envío de mensajes de un usuario en la comunidad
 * Meterlo en fichero ---> MVC.Mensajes junto con operativaMensajeriaGnoss
 * */
const operativaEnvioMensajesGnoss = {

    /**
     * Acción para inicializar elementos y eventos
     * @param {any} pParams
     */
    init: function (pParams) {
        this.config(pParams);
        this.configEvents();
    },

    /**
     * Opciones de configuración de la vista (recoger ids para poder interactuar)
     * @param {any} pParams
     */
    config: function (pParams) {

        // Inicialización de las vistas
        this.txtDestinatario = $(`#${pParams.views.idTxtDestinatario}`);
        this.txtDestinatario_Hack = $(`#${pParams.views.idTxtDestinatario_Hack}`);
        this.txtDestinatarioOrg = $(`#${pParams.views.idTxtDestinatarioOrg}`);
        this.txtDestinatarioOrg_Hack = $(`#${pParams.views.idTxtDestinatarioOrg_Hack}`);
        this.txtAsunto = $(`#${pParams.views.idTxtAsunto}`);
        this.txtMensaje = $(`#${pParams.views.idTxtMensaje}`);
        this.divPanelInfo = $(`#${pParams.views.idDivPanelInfo}`);
        this.panelDestinatariosNoOrg = $(`#${pParams.views.idPanelDestinatariosNoOrg}`);
        this.panelDestinatariosOrg = $(`#${pParams.views.idPanelDestinatariosOrg}`);
        this.rbEnviarComoUsuario = $(`#${pParams.views.idRbEnviarComoUsuario}`);
        this.rbEnviarComoOrg = $(`#${pParams.views.idRbEnviarComoOrg}`);
        this.inputCuentaUsuario = $(`input[type=radio][name=${pParams.views.idRbEnviarComoOrg}]`);
        this.btnSendMessage = $(`#${pParams.views.idBtnSendMessage}`);

        // Identidades para autocompletar
        this.identidad = pParams.autocomplete.identidad;
        this.identidadOrg = pParams.autocomplete.identidadOrg;

        // Url para envío de mensaje
        this.urlSendMessage = pParams.views.urlSendMessage;

        // Inputs que NO podrán quedar vacíos
        this.inputsNoEmpty = [this.txtAsunto, this.txtMensaje];
    },

    /**
    * Configuración de los eventos de los elementos html (click, focus...)
    * */
    configEvents: function () {
        const that = this;

        // Botón de envío de mensaje
        this.btnSendMessage.on("click", function () {
            if (that.validarCampos(that.inputsNoEmpty) == true) {
                // Acción de enviar mensaje
                that.sendMessage();
            }
        });

        // RadioButton de tipo de perfil (Verificar si realmente existe o se muestra en pantalla)
        if (this.rbEnviarComoUsuario != undefined) {
            this.rbEnviarComoUsuario.on("click", function () {
                // Enviar email identidad Usuario
                that.panelDestinatariosNoOrg.removeClass('d-none');
                that.panelDestinatariosOrg.addClass('d-none');
                // Vaciar el txt de destinatarios de organización
            });
            this.rbEnviarComoOrg.on("click", function () {
                // Enviar email identidad Organización
                that.panelDestinatariosNoOrg.addClass('d-none');
                that.panelDestinatariosOrg.removeClass('d-none');
                // Vaciar el txt de destinatarios de identidad usuario

            });
        }

        // Configurar servicio autocompletar en input "Para"
        this.txtDestinatario.autocomplete(
            null,
            that.configAutocompletarService(that.identidad, that.identidadOrg)
        );

        // Configurar servicio autocompletar en input "Para" de "Organización" siempre que exista
        if (this.txtDestinatarioOrg != undefined) {
            this.txtDestinatarioOrg.autocomplete(
                null,
                that.configAutocompletarService(that.identidadOrg, that.identidadOrg)
            );
        }

        // Valor cambiado de inputs -> Avisar con sobreado rojo (o quitarlo) si es vacío campo obligatorio
        this.inputsNoEmpty.forEach(input => {
            input.on("change", function () {
                if ($(this).val().length == 0) {
                    $(this).addClass('is-invalid');
                } else {
                    $(this).removeClass('is-invalid');
                }
            });
        });
    },

    /**
     * Acción para configurar el servicio autcompletar según los parámetros pasados.
     * @param {any} identidad: Identidad del usuario 
     * @param {any} identidadOrg: Identidad de la organización para el servicio autocomplete
     * @param {any} bool_esGnossOrganiza: Bool de si es gnoss el organizador (Por defecto "FALSE")
     */
    configAutocompletarService: function (identidad, identidadOrg, bool_esGnossOrganiza = "FALSE") {

        return {

            //servicio: new WS($('input.inpt_urlServicioAutocompletar').val(), WSDataType.jsonp),
            //metodo: 'AutoCompletarEnvioMensajes',
            url: $('input.inpt_urlServicioAutocompletar').val() + '/AutoCompletarEnvioMensajes',
            type: "POST",
            delay: 200,
            multiple: true,
            scroll: false,
            selectFirst: false,
            minChars: 1,
            width: 300,
            cacheLength: 0,
            extraParams: {
                bool_esGnossOrganiza: 'FALSE',
                identidad: this.identidad,
                identidadOrg: this.identidadOrg,
            }
        }
    },

    /**
    * Validar que los campos aquí mencionados no estén vacíos
    * @param {any} inputs: Array de inputs para ser recorridos y verificar que ninguno de los aquí indicados estén vacíos
    * @returns {bool}: Devolverá true o false siempre y cuando los inputs pasados sean diferente de vacío
    */
    validarCampos: function (inputs) {
        let areInputsOK = false;
        let error = "";

        for (input of inputs) {
            if (input.length > 0) {
                error = ValidarCampoNoVacio(input, undefined, true);
                if (error.length > 0) { break }
            }
        }

        if (error.length > 0) {
            this.showInfoPanelErrorOrOK(false, true, error);
        } else {
            areInputsOK = true
            this.showInfoPanelErrorOrOK(false, false, undefined);
        }
        return areInputsOK;
    },

    /**
    * Mostrar el panel informativo con un mensaje de error o ko. Si los dos son falsos, el panel quedará oculto
    * @param {boolean} showOK: Si se desea mostrar el mensaje de OK
    * @param {boolean} showError: Si se desea mostrar el mensaje KO
    * @param {string} message: El mensaje que irá en el panel informativo
    */
    showInfoPanelErrorOrOK: function (showOK, showError, message) {
        const that = this;

        // Mostrar panel OK
        that.divPanelInfo.html(message);
        if (showOK) {
            that.divPanelInfo.addClass(this.okClass);
            that.divPanelInfo.removeClass(this.errorClass);

        } else if (showError) {
            // Mostrar panel Error
            that.divPanelInfo.removeClass(this.okClass);
            that.divPanelInfo.addClass(this.errorClass);
        } else {
            // Ocultar el panel de mensajes
            this.divPanelInfo.hide();
            return
        }
        // Mostrar el panel
        this.divPanelInfo.show();
    },

    /**
     * Acción de enviar mensaje cuando los datos no son vacíos y se ha pulsado en el botón de "Enviar".
     * */
    sendMessage: function () {

        const that = this;

        // Comprobar si la Organización está chequeado
        const Org = this.rbEnviarComoOrg.is("checked");

        // Lista de destinatarios
        let destinatarios = "";
        if (Org) {
            //Destinatario Perfil Organización
            destinatarios = this.txtDestinatarioOrg_Hack.val();
        } else {
            //Destinatario Perfil Usuario
            destinatarios = this.txtDestinatario_Hack.val();
        }

        // Comprobar destinatarios => Enviar mensaje si hay destinatarios
        if (destinatarios != "" && destinatarios.length > 1) {

            // Antes de enviar mensaje guardar eliminar prevención de avisar de que el formulario ha sido cambiado -> No avisará al usuario si cierra la ventana o actualiza
            if ($("#preventLeavingFormWithoutSaving").dirty != null) {
                $("#preventLeavingFormWithoutSaving").dirty("setAsClean");
            }

            // Mostrado de Loading
            MostrarUpdateProgress();
            const asunto = this.txtAsunto.val();
            const cuerpo = this.txtMensaje.val();
            // Obtener la accion a partir del ancla, almacenando la accion y el id del mensaje
            const urlAccion = ObtenerHash2();

            // Construir el objeto params para envío de mensaje
            const params = {
                Subject: asunto,
                Body: encodeURIComponent(cuerpo.replace(/\n/g, '')),
                Receivers: destinatarios,
                OrgIsSender: Org,
                UrlAction: urlAccion,
            }

            // Borrado de mensaje de error actual antes de hacer petición
            that.showInfoPanelErrorOrOK(false, false, undefined);

            // Eliminar el posible mensaje que haya a modo de backup
            operativaGestionEmailsGuardadoSeguridad.deleteEmailDetailInfoInLocalStorage();
            // Enviar el mensaje
            GnossPeticionAjax(this.urlSendMessage, params, true)
                .fail(function (data) {
                    that.showInfoPanelErrorOrOK(false, true, data);
                    // Volver a poner el formulario con prevención de salir de la página
                    if ($("#preventLeavingFormWithoutSaving").dirty != null) {
                        $("#preventLeavingFormWithoutSaving").dirty("setAsDirty");
                    }                    
                }).always(function (data) {                                        
                    // Ocultado de progress
                    OcultarUpdateProgress()
                });
        } else {
            // Mostrar aviso de que es necesario añadir algún destinatario
            this.showInfoPanelErrorOrOK(false, true, "Selecciona al menos un destinatario");
        }
    },
};


/**
 * Operativa para realizar el guardado de un mensaje Gnoss en caso de que se pierda o no se envíe
 */ 
const operativaGestionEmailsGuardadoSeguridad = {

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

        // Modal de restaurar email enviado
        this.modalBackupEmailContent = $("#modal-email-backup-content");
        
        // Botones para confirmar o no la restauración de la copia de seguridad de la página cms
        this.btnConfirmRestoreEmailContent = $(".btnConfirmRestoreEmailContent");
        this.btnNotConfirmRestoreEmailContent = $(".btnNotConfirmRestoreEmailContent");   
        // Input del cuerpo del mensaje
        this.textAreaCuerpoMensaje = $("#txtMensaje");         
        // Flags de inicio de operativa
        // Local storage no disponible de momento
        this.isLocalStorageAvailable = false;
        // Indicador de si hay contenido previo al enviar un email
        this.isBackupContentAvailableForEmail = false;

        // Key que almacenará el contenido del html que se ha "copiado". Cuerpo del email
        this.keyEmailContent = "KEY_EMAIL_CONTENT";
    },

    /*
     * Configurar eventos de los elementos html
     * */    
    configEvents: function(){
        const that = this;

        // Restaurar la copia de seguridad de la página cms que se ha encontrado
        this.btnConfirmRestoreEmailContent.on("click", function(){
            that.handleRestoreEmailContent();            
        });

        // Cancelar la restauración de la copia de seguridad de la página cms que se ha encontrado
        this.btnNotConfirmRestoreEmailContent.on("click", function(){
            that.handleNotRestoreEmailContent();
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
        },1500);                   
    },

    /**
     * Método para comprobar si el localstorage está disponible. Lo guardará en la variable para futuras acciones.
     */
     checkLocalStorage: function(){
        const that = this;
        if (that.storageAvailable('localStorage')) {
            // El storage está disponible   
            that.isLocalStorageAvailable = true;            
        }
    },    

    /**
     * Método que iniciará el copiado del estado de la página siempre y cuando el localStorage esté disponible
     */
    initBackupSaved: function(){
        const that = this;

        observarClaseTinyLoaded(function () {
            const editors = tinymce.get();
            const relatedTinyMCEId = $("#txtMensaje").data("editorrelated");
            // Iterar sobre las instancias para buscar la que coincide con el ID 'txtMensaje'
            editors.forEach(function (editor) {
                if (editor.id === relatedTinyMCEId) {
                    that.editorTinyMCE = editor;
                    that.checkBackupContentAvailableForEmail();
                    if (that.isBackupContentAvailableForEmail) {
                        // Preguntar si se desea restaurar el contenido actual almacenado          
                        that.modalBackupEmailContent.modal("show");
                    } else {
                        if (that.isLocalStorageAvailable == true) {
                            // Ejecutar copiado de backup de forma planificada
                            that.initScheduleBackupCopy();
                        }
                    }                    
                }
            });
        });

    },

    /**
     * Copiado programado del contenido de la página CMS
     */
    initScheduleBackupCopy: function(){
        const that = this;
        // Realizar copiado de página CMS cada 30 segundos
        setInterval(function(){            
            that.saveEmailBodyInLocalStorage();
        }, 15000/*30000*/);
    },

    /**
     * Método que copiará el contenido de la página que está siendo editada en localStorage a modo de backup
     */
    saveEmailBodyInLocalStorage: function(){
        const that = this;
       
        // Contenido de la página CMS
        let cuerpoMensaje = that.textAreaCuerpoMensaje.val();
        if (cuerpoMensaje.length > 0){
            cuerpoMensaje = encodeURIComponent(cuerpoMensaje.replace(/\n/g, ''));             
            const backupData = cuerpoMensaje;
            // Guardar el contenido        
            localStorage.setItem(that.keyEmailContent, backupData); 
        }        
    },

    /**
     * Método que eliminará el contenido almacenado en localStorage
     * Este método se ejecutará cuando se envíe el email de forma correcta por el usuario
     */
    deleteEmailDetailInfoInLocalStorage: function(){
        const that = this;
        
        if (this.isLocalStorageAvailable){
            localStorage.removeItem(that.keyEmailContent);          
        }
    },

    /**
     * Método que comprobará si hay información previa del email por si no se han guardado los cambios
     */
    checkBackupContentAvailableForEmail: function(){
        const that = this;

        let contentBackupEmail = localStorage.getItem(that.keyEmailContent);
        if (contentBackupEmail != null || contentBackupEmail != undefined){
            that.isBackupContentAvailableForEmail = true;
        }else{
            that.isBackupContentAvailableForEmail = false;
        }
    },

    /**
     * Método para restaurar la página de backup al CMS Builder
     */
    handleRestoreEmailContent: function(){
        const that = this;
               
        setTimeout(function(){
            // Contenido de la página del localStorage
            const htmlContent = decodeURIComponent(localStorage.getItem(that.keyEmailContent));
            // Establecer el contenido en ckeEditor para el cuerpo del mensaje
            // CKEDITOR.instances.txtMensaje.setData(htmlContent);            
            that.editorTinyMCE.setContent(htmlContent);
            // Eliminar el item restaurado
            that.deleteEmailDetailInfoInLocalStorage();
            // Iniciar de nuevo la operativa
            that.triggerEvents();
        },1500);
        
    },

    /**
     * Método para no restaurar la vista de copia de seguridad.
     */
    handleNotRestoreEmailContent: function(){
        const that = this;
        // Eliminar el item restaurado
        that.deleteEmailDetailInfoInLocalStorage();
        // Iniciar de nuevo la operativa
        that.triggerEvents();
    },

    /**
     * Método para comprobar si está disponible el local storage del navegador
     * @param {any} type
     */
    storageAvailable: function (type) {
        try {
            var storage = window[type],
                x = '__storage_test__';
            storage.setItem(x, x);
            storage.removeItem(x);
            return true;
        }
        catch (e) {
            return e instanceof DOMException && (
                // everything except Firefox
                e.code === 22 ||
                // Firefox
                e.code === 1014 ||
                // test name field too, because code might not be present
                // everything except Firefox
                e.name === 'QuotaExceededError' ||
                // Firefox
                e.name === 'NS_ERROR_DOM_QUOTA_REACHED') &&
                // acknowledge QuotaExceededError only if there's something already stored
                storage.length !== 0;
        }
    },   

}


/**
 * Acción de eliminar un único mensaje. Se ejecuta cuando se pulse en el sí del modal.
 * @param {any} correoID: Id del mensaje/mensajes que se desea eliminar 
 */
function EliminarCorreoSeleccionado(correoID) {
    MostrarUpdateProgress();

    // Obtenengo la url para hacer la petición de eliminación del mensaje
    const urlDeleteMessage = location.href.substr(0, location.href.indexOf('?')) + "/deleteMessages";

    var params = {
        listMessages: correoID,
        profileBox: $('.tipoBandeja').val()
    }
    GnossPeticionAjax(urlDeleteMessage, params, true)
}


