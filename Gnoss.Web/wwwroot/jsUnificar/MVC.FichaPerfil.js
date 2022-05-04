/**
 * Acci�n de seguir a un usuario de una comunidad. Se ejecuta cuando se pulsa en el bot�n "Seguir"
 * @param {any} that: El bot�n que ha disparado la acci�n
 * @param {any} urlSeguirPerfil: URL a la que hay que llamar para realizar la llamda de "No seguir"
 */
function AccionPerfil_Seguir(that, urlSeguirPerfil) {

    // Icono del bot�n clickeado
    const buttonIcon = $(that).find("span.material-icons");
    const oldIconButton = buttonIcon.text();
    const oldTextButton = $(that).find(".texto").text();

    // Textos e iconos una vez pulsado el bot�n
    let newTextButton = "";
    let newIconButton = "";
    const loadingClass = "spinner-border spinner-border-sm mr-2 pr-0";


    // Acción de follow unfollow
    const followJsAction = $(that).attr('onclick');
    if (followJsAction.indexOf("unfollow") >= 0) {
        // Se desea NO seguir el perfil
        newTextButton = "Seguir";
        newIconButton = "person_add_alt_1";
    } else {
        // Se desea seguir el perfil
        newTextButton = "Dejar de seguir";
        newIconButton = "person_remove_alt_1";
    }

    // Quitar el icono para mostrar el loading
    buttonIcon.text('');
    // Mostrar el loading
    buttonIcon.addClass(loadingClass);

    GnossPeticionAjax(urlSeguirPerfil, null, true)
        .done(function (data) {
            // Quitar el Loading 
            buttonIcon.removeClass(loadingClass);
            // Añadir el nuevo método para seguir/no seguir
            if (data == "invitado") {
                operativaLoginEmergente.init();
                return;
            }
            $(that).attr('onclick', followJsAction.replace('follow', 'unfollow'));
            ChangeButtonAndText(that, newTextButton, newIconButton, null);
        })
        .fail(function (data) {
            // Dejar los iconos y textos anteriores del botón
            ChangeButtonAndText(that, oldTextButton, oldIconButton, null);

            if (data == "invitado") {
                operativaLoginEmergente.init();
                return;
            }
        })
        .always(function (data) {
            buttonIcon.removeClass(loadingClass);
        });
}

/**
 * Acción de seguir/no seguir a un usuario. Cambiará la lógica de todos los botones de seguimiento del usuario en la lista de recursos
 * @param {*} that
 * @param {*} urlSeguir
 * @param {string} followUser : Id del usuario (alias) del usuario para seguir/no seguir
 */
function AccionPerfil_Seguir_Listado(that, urlSeguir, followUser) {

    // Actual item sobre el que se ha presionado para Seguir/No seguir
    const currentFollowActionItem = $(that).parent();
    // Icono actual de la acción de seguir
    const currentFollowActionIcon = currentFollowActionItem.find(".icon");
    // Texto actual de la acción de seguir
    const currentFollowActionText = currentFollowActionItem.find(".texto");

    // Cada uno de los items que aparecerán en la lista para seguir o no al usuario
    const listFollowActionItem = $(`.followAction_${followUser}`);
    // Cada uno de los iconos que aparecerán en la lista para seguir o no al usuario
    const listFollowActionIcon = listFollowActionItem.find(".icon");
    const listFollowActionText = listFollowActionItem.find(".texto");


    // Acción de follow unfollow
    const followJsAction = listFollowActionText.attr('onclick');

    // Textos e iconos una vez pulsado el botón para seguir/no seguir
    const followText = "Seguir";
    const noFollowText = "Dejar de seguir";
    const followIcon = "person_add_alt_1";
    const noFollowIcon = "person_remove";
    const loadingClass = "spinner-border spinner-border-sm texto-primario";

    // 1 - Quitar el contenido del icono
    currentFollowActionIcon.text('');
    // 2 - Añadir la clase de loading (petición in progress)
    currentFollowActionIcon.addClass(loadingClass);

    GnossPeticionAjax(urlSeguir, null, true)
        .done(function (data) {
            // Finalizar Loading
            currentFollowActionIcon.removeClass(loadingClass);
            // Comprobar si se estaba siguiendo el perfil seleccionado
            if (currentFollowActionItem.attr('data-follow') != undefined) {
                // No se desea seguir al usuario - Se estaba siguiendo al usuario                             

                // Añadir texto de "Seguir" e icono a la lista
                listFollowActionText.text(followText);
                listFollowActionIcon.text(followIcon);
                // Quitar el atributo
                listFollowActionItem.removeAttr('data-follow');
                // Añadir la acción para seguir                
                listFollowActionText.attr('onclick', followJsAction.replace('unfollow', 'follow'));

            } else {
                // Se desea seguir al usuario - No se estaba siguiendo al usuario            
                // Añadir texto e icono de "No Seguir"
                // Cambiar la acción de Seguir/No seguir                
                listFollowActionText.text(noFollowText);
                listFollowActionIcon.text(noFollowIcon);
                listFollowActionItem.attr('data-follow', '');
                listFollowActionText.attr('onclick', followJsAction.replace('follow', 'unfollow'));

            }
        })
        .fail(function (data) {
            // Dejarlo como estaba
            if (currentFollowActionItem.attr('data-follow') != undefined || currentFollowActionItem.attr('data-follow') != false) {
                currentFollowActionText.text(noFollowText);
                currentFollowActionIcon.text(noFollowIcon);
            } else {
                currentFollowActionText.text(followText);
                currentFollowActionIcon.text(followIcon);
            }

        })
        .always(function (data) {
            currentFollowActionIcon.removeClass(loadingClass);
        });
}


/**
 * Acci�n de no seguir a un usuario de una comunidad. Se ejecuta cuando se pulsa en el bot�n "No seguir"
 * Cambiar� el nombre al bot�n indicando "Siguiendo"
 * Eliminar� el atributo onClick para que no se vuelva a ejecutar la acci�n del bot�n
 * @param {any} that: El bot�n que ha disparado la acci�n
 * @param {any} urlNoSeguirPerfil: URL a la que hay que llamar para realizar la llamda de "No seguir"
 */
function AccionPerfil_NoSeguir(that, urlNoSeguirPerfil) {

    // Icono del bot�n
    var buttonIcon = $(that).find("span.material-icons");
    // Texto del bot�n
    var textButton = $(buttonIcon).siblings();
    // Textos e iconos una vez pulsado el bot�n
    var newTextButton = "Sin seguimiento";
    var newIconButton = "person_outline";
    
    GnossPeticionAjax(urlNoSeguirPerfil, null, true).fail(function (data) {
        if (data == 'invitado') {
            operativaLoginEmergente.init();
        }
    });
    // Nuevo Front
    //$(that).parent().remove();
    // Cambiar el nombre e icono del bot�n
    ChangeButtonAndText(that, newTextButton, newIconButton, "btn-primary");
}

/**
 * Cambiar el texto, el icono (material-icons) y eliminar el evento click de un bot�n
 * @param {any} button: El bot�n que se desea modificar
 * @param {any} newTextButton: El nuevo texto que tendr� el bot�n
 * @param {any} newIconButton: El nuevo icono (material-icons) que tendr� el bot�n
 * @param {any} classToBeDeleted: La clase que se eliminar� del bot�n 
 */
function ChangeButtonAndText(button, newTextButton, newIconButton, classToBeDeleted) {

    // Icono del bot�n
    var buttonIcon = $(button).find("span.material-icons");
    var textButton = "";
    var icon = "";

    if (buttonIcon.length == 0) {
        // No hay botón --> Buscar un span
        buttonIcon = $(button).siblings();
        textButton = $(button);

    } else {
        textButton = $(buttonIcon).siblings();
    }

    // Cambiar nombre al bot�n 
    textButton.text(newTextButton);
    // Cambiar el icono al bot�n
    buttonIcon.text(newIconButton);
    // A�adir cursor: normal
    if (classToBeDeleted != undefined) {
        $(button).css("cursor", "auto");
        // Quitar el estilo de bot�n (no clickeable)
        $(button).removeClass(classToBeDeleted);
    }
}


/**
 * Acci�n que se ejecuta cuando se pulsa sobre las acciones disponibles de un perfil para mandar un "Email". 
 * @param {string} titulo: T�tulo que tendr� el panel modal 
 * @param {string} texto: El texto o mensaje a modo de t�tulo que se mostrar� para que el usuario sepa la acci�n que se va a realizar
 * @param {string} id: Identificador de la persona sobre el que se aplicar� la acci�n 
 * @param {any} idModalPanel: Panel modal contenedor donde se insertar� este HTML (Por defecto ser� #modal-container)
 * */
function AccionEnviarMensajeMVC(urlPagina, id, titulo, idModalPanel = "#modal-container") {
    
    // Panel din�mico del modal padre donde se insertar� la vista "hija"
    const $modalDinamicContentPanel = $('#modal-container').find('#modal-dinamic-content #content');

    // Declaraci�n de la acci�n que se realizar� al hacer click en Enviar mensaje
    var accion = "EnviarMensaje('" + urlPagina + "', '" + id + "');";
    
    // Plantilla del panel html que se cargar� en el modal contenedor al pulsar en la acci�n
    var plantillaPanelHtml = '';
    // Cabecera del panel
    plantillaPanelHtml += '<div class="modal-header">';
        plantillaPanelHtml += '<p class="modal-title"><span class="material-icons">label</span>' + titulo + '</p>';
        plantillaPanelHtml += '<span class="material-icons cerrar" data-dismiss="modal" aria-label="Close">close</span>';
    plantillaPanelHtml += '</div>';
    // Cuerpo del panel
    plantillaPanelHtml += '<div class="modal-body">';
        plantillaPanelHtml += '<div class="formulario-edicion">';
            // Asunto del email - InputText
            plantillaPanelHtml += '<div class="form-group">';
                plantillaPanelHtml += '<label for="txtAsunto_'+id+'">' + mensajes.enviarMensaje + '</label>';
                plantillaPanelHtml += '<input type="text" class="form-control" id="txtAsunto_'+id+'" rows="3"> </textarea>' ;
            plantillaPanelHtml += '</div>';

            // Cuerpo del email - TextArea
            plantillaPanelHtml += '<div class="form-group">';
                plantillaPanelHtml += '<label>' + mensajes.descripcion + '</label>';
                plantillaPanelHtml += '<textarea class="form-control cke mensajes" id="txtDescripcion_'+id+'" rows="3"> </textarea>' ;
            plantillaPanelHtml += '</div>';

        // Contenedor de mensajes y botones
        plantillaPanelHtml += '<div id="modal-dinamic-action-response">';
        // Posibles mensajes de info
            plantillaPanelHtml += '<div>';
                plantillaPanelHtml += '<div class="menssages_'+id+'" id="menssages">';
                    plantillaPanelHtml += '<div class="ok"></div>';
                    plantillaPanelHtml += '<div class="ko"></div>';
                plantillaPanelHtml += '</div>';
            plantillaPanelHtml += '</div>';
        // Panel de botones para la acci�n
            plantillaPanelHtml += '<div id="modal-dinamic-action-buttons" class="form-actions">'
                plantillaPanelHtml += '<button class="btn btn-primary">' + mensajes.enviar + '</button>'                
            plantillaPanelHtml += '</div>';
        plantillaPanelHtml += '</div>';
    plantillaPanelHtml += '</div>';

    // Meter el c�digo de la vista modal en el contenedor padre que viene identificado por el id #modal-container
    // En concreto, hay que buscar la etiqueta modal-dinamic-content #content e insertar el c�digo
    $modalDinamicContentPanel.html(plantillaPanelHtml);

    // Acceso a los botones
    const botones = $modalDinamicContentPanel.find('#modal-dinamic-action-buttons > button');

    RecargarTodosCKEditor();

    // Asignaci�n de la funci�n al bot�n "S�" o de acci�n
    $(botones[0]).on("click", function () {
        EnviarMensaje(urlPagina, id);
    });   
    
}

/**
 * Acci�n que se ejecuta para mandar un email a un contacto. Ej: Desde el perfil de un usuario, se puede pulsar en el bot�n "Escribir mensaje".
 * Esta acci�n es ejecutada desde AccionEnviarMensajeMVC
 * @param {any} urlPagina
 * @param {any} id
 */
function EnviarMensaje(urlPagina, id) {
    if ($('#txtAsunto_' + id).val() != '' && $('#txtDescripcion_' + id).val() != '') {
        var asunto = $('#txtAsunto_' + id).val();
        var mensaje = $('#txtDescripcion_' + id).val();
        MostrarUpdateProgress();

        var dataPost = {
            callback: "Accion_EnviarMensaje",
            asunto: asunto,
            mensaje: encodeURIComponent(mensaje.replace(/\n/g, ''))
        }

        GnossPeticionAjax(urlPagina, dataPost, true).done(function (data) {
            // Cambiado por nuevo Front
            //DesplegarResultadoAccionMVC("desplegable_" + id, true, mensajes.mensajeEnviado);
            DesplegarResultadoAccionMVC("modal-dinamic-action-response", true, mensajes.mensajeEnviado);
            // Esperar 1,5 segundos y ocultar el panel
            setTimeout(() => {
                $('#modal-container').modal('hide');
            }, 1500)
        }).fail(function (data) {
            //DesplegarResultadoAccionMVC("desplegable_" + id, false, "");
            DesplegarResultadoAccionMVC("modal-dinamic-action-response", false, "Se ha producido un error al enviar el mensaje. Por favor int�ntalo de nuevo m�s tarde.");
        }).always(function (data) {
            OcultarUpdateProgress();
        });
    }
    else {
        $('.menssages_' + id).find(".ko").css("display", "block");
        $('.menssages_' + id).find(".ko").html(mensajes.mensajeError);
    }
}

function AccionEnviarMensajeMVCTutor(urlPagina, id) {
    var $c = $('#' + id);

    if (CKEDITOR.instances["txtDescripcion_" + id] != null) {
        CKEDITOR.instances["txtDescripcion_" + id].destroy();
    }

    if ($c.children().length > 0) {
        //Eliminar el anterior
        $c.children().remove();
    }

    var accion = "javascript:EnviarMensajeTutor('" + urlPagina + "', '" + id + "');";

    var $confirmar = $(['<fieldset class="mediumLabels"><legend>', mensajes.enviarMensaje, '</legend><p><label for="txtAsunto_', id, '">', mensajes.asunto, '</label><input type="text" id="txtAsunto_', id, '" class="text big"></p><p><label for="txtDescripcion_', id, '">', mensajes.descripcion, '</label></p><p><textarea class="cke mensajes" id="txtDescripcion_', id, '" rows="2" cols="20"></textarea></p><p><label class="error" id="error_', id, '"></label></p><input type="button" onclick="', accion, '" value="', mensajes.enviar, '" class="text medium"></p></fieldset>'].join(''));
    //$confirmar.find('div').filter('.pregunta').fadeIn().end().filter('.mascara').show().fadeTo(600, 0.8);
    // Incrustamos el elemento a la vez que lo mostramos y definimos los eventos: 
    $confirmar.prependTo($c)
.find('button').click(function () { // Ambos botones hacen desaparecer la mascara
    $c.parents('.stateShowForm').css({ display: 'none' });
    $confirmar.css({ display: 'none' });
}).eq(0).click(accion); // pero solo el primero activa la funcionConfirmada

    if ($('#divContMensajesPerf').length > 0 && $('#divContMensajesPerf').html() == '') {
        $('#divContMensajesPerf').html($('#desplegable_' + id).parent().html());
        $('#desplegable_' + id).parent().html('');
    }

    MostrarPanelAccionDesp("desplegable_" + id, null);
    RecargarTodosCKEditor();
}


function EnviarMensajeTutor(urlPagina, id) {
    if ($('#txtAsunto_' + id).val() != '' && $('#txtDescripcion_' + id).val() != '') {
        var asunto = $('#txtAsunto_' + id).val();
        var mensaje = $('#txtDescripcion_' + id).val();
        MostrarUpdateProgress();

        var dataPost = {
            callback: "Accion_EnviarMensajeTutor",
            asunto: asunto,
            mensaje: encodeURIComponent(mensaje.replace(/\n/g, ''))
        }

        GnossPeticionAjax(urlPagina, dataPost, true).done(function (data) {
            DesplegarResultadoAccionMVC("desplegable_" + id, true, mensajes.mensajeEnviado);
        }).fail(function (data) {
            DesplegarResultadoAccionMVC("desplegable_" + id, false, data);
        }).always(function (data) {
            OcultarUpdateProgress();
        });
    }
    else {
        $('#error_' + id).html(mensajes.mensajeError);
    }
}

/**
 * Acción de mandar a API endPoint la acción de agregar un contacto 
 * @param {any} urlPagina: Url a para ejecutar la acción
 */
function AgregarContacto(urlPagina) {
    var dataPost = {
        callback: "Accion_AgregarContacto"
    }
    $.post(urlPagina, dataPost);
    // Cerrar el modal pasados 1,5 segundos
    setTimeout(() => {
        $('#modal-container').modal('hide');
    }, 1500);
}

/**
 * Acción de mandar a API endPoint la acci�n de Eliminar un contacto
 * @param {any} urlPagina: Url a para ejecutar la acci�n
 */
function EliminarContacto(urlPagina) {
    var dataPost = {
        callback: "Accion_EliminarContacto"
    }
    $.post(urlPagina, dataPost);
    // Cerrar el modal pasados 1,5 segundos
    setTimeout(() => {
        $('#modal-container').modal('hide');
    }, 1500);
}

function AgregarContactoOrg(urlPagina) {
	var dataPost = {
		callback: "Accion_AgregarContactoOrg"
	}
	$.post(urlPagina, dataPost);
}

function EliminarContactoOrg(urlPagina) {
	var dataPost = {
		callback: "Accion_EliminarContactoOrg"
	}
	$.post(urlPagina, dataPost);
}

function NotificarCorreccion(urlPagina) {
	var dataPost = {
		callback: "Accion_EnviarCorreccion"
	}
	$.post(urlPagina, dataPost);
}

function NotificarCorreccionDefinitiva(urlPagina) {
	var dataPost = {
		callback: "Accion_EnviarCorreccion",
		EnviarCorreccionDefinitiva: true
	}
	$.post(urlPagina, dataPost);
}

function ValidarCorreccion(urlPagina) {
	var dataPost = {
		callback: "Accion_ValidarCorreccion"
	}
	$.post(urlPagina, dataPost);
}

function EliminarPersona(urlPagina) {
	var dataPost = {
		callback: "Accion_EliminarPersona"
	}
	$.post(urlPagina, dataPost);
}

/**
 * Acci�n para expulsar a una persona de una comunidad. Se ejecuta cuando (por ejemplo) se selecciona desde listado de personas, la opci�n de "Expulsar"
 * Se cargar� un nuevo modal para hacer la gesti�n de la explusi�n
 * @param {any} urlPagina: Url a la que se realizar� la petici�n para expulsar a la persona
 * @param {any} id: Identificador de la persona a la que se expulsar�
 * @param {any} titulo: Titulo de la ventana modal
 * @param {any} textoBotonPrimario: Titulo del bot�n primario
 * @param {any} textoBotonSecundario: Titulo del bot�n secundario (No/Cancelar)
 * @param {any} texto: Explicaci�n de la acci�n de expulsar usuario
  * @param {any} accionCambiarNombreHtml: Accion JS que servir� para cambiar el nombre del elemento una vez se proceda a expulsar a una persona.
 * @param {any} idModalPanel: Panel modal contenedor donde se insertar� este HTML (Por defecto ser� #modal-container)
 */
function Expulsar(urlPagina, id, titulo, textoBotonPrimario, textoBotonSecundario, texto, accionCambiarNombreHtml, idModalPanel = "#modal-container") {
    
    // Acci�n que se ejecutar� al pulsar sobre el bot�n primario (Realizar la acci�n de Expulsar)
    var accion = "EnviarAccionExpulsar('" + urlPagina + "', '" + id + "');";

    // Panel din�mico del modal padre donde se insertar� la vista "hija"
    const $modalDinamicContentPanel = $('#modal-container').find('#modal-dinamic-content #content');

    // Plantilla del panel html que se cargar� en el modal contenedor al pulsar en la acci�n
    var plantillaPanelHtml = '';
    // Cabecera del panel
    plantillaPanelHtml += '<div class="modal-header">';
        plantillaPanelHtml += '<p class="modal-title"><span class="material-icons">label</span>' + titulo + '</p>';
        plantillaPanelHtml += '<span class="material-icons cerrar" data-dismiss="modal" aria-label="Close">close</span>';
    plantillaPanelHtml += '</div>';
    // Cuerpo del panel
    plantillaPanelHtml += '<div class="modal-body">';
        plantillaPanelHtml += '<div class="formulario-edicion">';
            plantillaPanelHtml += '<div class="form-group">';
                plantillaPanelHtml += '<label class="control-label">' + texto + '</label>';
            plantillaPanelHtml += '</div>';

        // Cuerpo del panel -> TextArea para enviar un email explicando la raz�n de la expulsi�n
            plantillaPanelHtml += '<div class="form-group">';
                plantillaPanelHtml += '<label for="txtMotivoExpulsion_'+ id +'">'+ accionesUsuarioAdminComunidad.motivoExpulsion +'</label>';
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
            // Panel de botones para la acci�n
                plantillaPanelHtml += '<div id="modal-dinamic-action-buttons" class="form-actions">'
                    plantillaPanelHtml += '<button data-dismiss="modal" class="btn btn-primary">' + textoBotonSecundario + '</button>'
                    plantillaPanelHtml += '<button class="btn btn-outline-primary ml-1" onclick="' + accion + '">'+ textoBotonPrimario + ", " + accionesUsuarioAdminComunidad.expulsarUsuario + '</button>'
                plantillaPanelHtml += '</div>';
            plantillaPanelHtml += '</div>';
    plantillaPanelHtml += '</div>';

    // Meter el c�digo de la vista modal en el contenedor padre que viene identificado por el id #modal-container
    // En concreto, hay que buscar la etiqueta modal-dinamic-content #content e insertar el c�digo
    $modalDinamicContentPanel.html(plantillaPanelHtml);

    // Asignar acciones adicionales al bot�n primario (Cambiar nombre del html)
    // Acceso a los botones
    const botones = $modalDinamicContentPanel.find('#modal-dinamic-action-buttons > button');

    // Asignaci�n de la funci�n al bot�n "S�" o de acci�n
    $(botones[1]).on("click", function () {
        // Ocultar el panel modal de bootstrap si hiciera falta        
    }).click(accionCambiarNombreHtml);
}

/**
 * Enviar la nueva petici�n del cambio de expulsi�n de una comunidad a un perfil sobre el que se ha pulsado el bot�n de "S�, Expulsar" del modal.
 * @param {any} urlPagina: Url donde se lanzar� la petici�n para cambiar el rol
 * @param {any} id: Identificador de la persona 
 */
function EnviarAccionExpulsar(urlPagina, id) {

    if ($('#txtMotivoExpulsion_' + id).val() != '') {
        var motivo = $('#txtMotivoExpulsion_' + id).val();
        MostrarUpdateProgress();

        var dataPost = {
            callback: "Accion_Expulsar",
            motivo: motivo
        }

        GnossPeticionAjax(urlPagina, dataPost, true).done(function (data) {
            // Cambiado por el nuevo front
            // DesplegarResultadoAccionMVC("desplegable_" + id, true, accionesUsuarioAdminComunidad.miembroExpulsado);
            DesplegarResultadoAccionMVC("modal-dinamic-action-response", true, accionesUsuarioAdminComunidad.miembroExpulsado);
            // Esperar 1 segundo y cerrar la ventana
            setTimeout(() => {
                $('#modal-container').modal('hide');
            }, 1500)
            
        }).fail(function (data) {
            // Cambiado por nuevo front
            //DesplegarResultadoAccionMVC("desplegable_" + id, false, "");
            DesplegarResultadoAccionMVC("modal-dinamic-action-response", false, "Error al tratar de expulsar al perfil de la comunidad. Por favor, int�ntalo de nuevo m�s tarde.");
        }).always(function (data) {
            OcultarUpdateProgress();
        });
    }
    else {
        // Cambiado por nuevo Front
        //$('#error_' + id).html(accionesUsuarioAdminComunidad.expulsionMotivoVacio);
        DesplegarResultadoAccionMVC("modal-dinamic-action-response", false, accionesUsuarioAdminComunidad.expulsionMotivoVacio);
    }
}

/**
 * Acci�n que se ejecuta cuando se pulsa sobre la acci�n de "Cambiar rol" disponible en un item/recurso de tipo "Perfil" encontrado por el buscador.  
 * @param {string} id: Identificador del recurso (en este caso de la persona) sobre el que se aplicar� la acci�n 
 * @param {any} rol: El rol actual del recurso (Perfil) clickeado
 * @param {any} urlPagina: Pagina sobre la que se lanzar� la llamada para realizar la acci�n de cambiar rol
 * @param {any} idModalPanel: Panel modal contenedor donde se insertar� este HTML (Por defecto ser� #modal-container)
 */
function CambiarRol(id, rol, urlPagina, idModalPanel = "#modal-container") {

    // Acci�n que se ejecutar� al pulsar sobre el bot�n primario (Realizar la acci�n de cambiar rol)
    var accion = "EnviarAccionCambiarRol('" + urlPagina + "', '" + id + "', '" + rol + "');";
    // Permisos para pintar los checkbox a mostrar al usuario
    var checkedAdmin = '';
    var checkedSupervisor = '';
    var checkedUsuario = '';

    if (rol == 0) {
        checkedAdmin = ' checked';
    }
    else if (rol == 1) {
        checkedSupervisor = ' checked';
    }
    else if (rol == 2) {
        checkedUsuario = ' checked';
    }

    // Panel din�mico del modal padre donde se insertar� la vista "hija"
    const $modalDinamicContentPanel = $('#modal-container').find('#modal-dinamic-content #content');

    // Plantilla del panel html que se cargar� en el modal contenedor al pulsar en la acci�n
    var plantillaPanelHtml = '';
    // Cabecera del panel
    plantillaPanelHtml += '<div class="modal-header">';
        plantillaPanelHtml += '<p class="modal-title"><span class="material-icons">label</span>' + accionesUsuarioAdminComunidad.cambiarRol + '</p>';
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

    plantillaPanelHtml += '<div id="modal-dinamic-action-response">';
    // Posibles mensajes de info
        plantillaPanelHtml += '<div>';
            plantillaPanelHtml += '<div id="menssages">';
                plantillaPanelHtml += '<div class="ok"></div>';
                plantillaPanelHtml += '<div class="ko"></div>';
            plantillaPanelHtml += '</div>';
        plantillaPanelHtml += '</div>';
    // Panel de botones para la acci�n
        plantillaPanelHtml += '<div id="modal-dinamic-action-buttons" class="form-actions">'
            plantillaPanelHtml += '<button class="btn btn-primary" onclick="'+ accion +'">' + accionesUsuarioAdminComunidad.cambiarRol + '</button>'            
        plantillaPanelHtml += '</div>';
    plantillaPanelHtml += '</div>';
plantillaPanelHtml += '</div>';

    // Meter el c�digo de la vista modal en el contenedor padre que viene identificado por el id #modal-container
    // En concreto, hay que buscar la etiqueta modal-dinamic-content #content e insertar el c�digo
    $modalDinamicContentPanel.html(plantillaPanelHtml);

}

/**
 * Enviar la nueva petici�n del cambio de rol una vez se ha pulsado sobre el bot�n de "Cambiar rol"
 * @param {any} urlPagina: Url donde se lanzar� la petici�n para cambiar el rol
 * @param {any} id: Identificador de la persona
 * @param {any} rol: Rol actual de la persona. Si es el mismo, no har�a nada
 */
function EnviarAccionCambiarRol(urlPagina, id, rol) {
    var rolNuevo = $('input[name="cambiarRol_' + id + '"]:checked').val();

    if (rolNuevo != rol) {
        MostrarUpdateProgress();

        var dataPost = {
            callback: "Accion_CambiarRol",
            rol: rolNuevo
        }

        GnossPeticionAjax(urlPagina, dataPost, true).done(function (data) {
            // Cambiado mensaje por el nuevo Front
            // CambiarNombreElemento(id + '_CambiarRol', accionesUsuarioAdminComunidad.rolCambiado);
            CambiarTextoAndEliminarAtributos(id + '_CambiarRol', accionesUsuarioAdminComunidad.rolCambiado, ['onclick', 'data-target', 'data-toggle']);
            DesplegarResultadoAccionMVC("modal-dinamic-action-response", true, accionesUsuarioAdminComunidad.rolCambiado);
            // Esperar 1 segundo y cerrar la ventana
            setTimeout(() => {
                $('#modal-container').modal('hide');
            }, 1500)
        }).fail(function (data) {
            DesplegarResultadoAccionMVC("modal-dinamic-action-response", false, "Error al tratar de cambiar el rol. Por favor, int�ntalo de nuevo m�s tarde.");
        }).always(function (data) {
            OcultarUpdateProgress();
        });        
    }
}

function Readmitir(urlPagina) {
    var dataPost = {
        callback: "Accion_Readmitir"
    }
    $.post(urlPagina, dataPost);

    setTimeout(() => {
        $('#modal-container').modal('hide');
    }, 1500)
}

function Bloquear(urlPagina) {
    var dataPost = {
        callback: "Accion_Bloquear"
    }
    $.post(urlPagina, dataPost);

    setTimeout(() => {
        $('#modal-container').modal('hide');
    }, 1500)
}

function Desbloquear(urlPagina) {
    var dataPost = {
        callback: "Accion_Desbloquear"
    }
    $.post(urlPagina, dataPost);

    setTimeout(() => {
        $('#modal-container').modal('hide');
    }, 1500)
}

function EnviarNewsletter(urlPagina) {
    var dataPost = {
        callback: "Accion_EnviarNewsletter"
    }
    $.post(urlPagina, dataPost);

    setTimeout(() => {
        $('#modal-container').modal('hide');
    }, 1500)
}

function NoEnviarNewsletter(urlPagina) {
    var dataPost = {
        callback: "Accion_NoEnviarNewsletter"
    }
    $.post(urlPagina, dataPost);

    setTimeout(() => {
        $('#modal-container').modal('hide');
    }, 1500)
}

function CambiarNombreElementoMVC(that, nombre) {
	$(that).html('');
	$(that).parent().html(nombre)
}