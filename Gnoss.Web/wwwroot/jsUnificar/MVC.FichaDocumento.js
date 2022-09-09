function DesplegarPanelMVC(pPanelID) {
    var panel = $('#' + pPanelID);

    panel.children().css("display", "block");
    panel.css("display", "block");

    panel.children().children('#loading').css("display", "none");
    panel.children().children('#menssages').css("display", "none");
    panel.children().children('#action').css("display", "block");
}

function DesplegarAccionConPanelIDMVC(pPanelCopiar, pBoton, pPanelID) {
    var panel = $('#' + pPanelID);

    panel.children().children('#loading').css("display", "block");
    panel.children().children('#menssages').css("display", "none");
    panel.children().children('#action').css("display", "none");

    panel.children().css("display", "block");
    panel.css("display", "block");

    if (pBoton != null && typeof (pBoton.parentNode) != 'undefined') {
        DesactivarBotonesActivosDespl();
        pBoton.parentNode.className += ' active';
    }

    panel.children().children('#action').html('').append($('#' + pPanelCopiar).clone());
    panel.children().children('#loading').css("display", "none");
    panel.children().children('#menssages').css("display", "none");
    panel.children().children('#action').css("display", "block");
    
}

// Desplegar y mostrar la vista o contenido devuelto de una petición vía urlAccion para ser mostrado en el panel pPanelID
/**
 * 
 * @param urlAccion: URL de la petición que será pasada al controller para que este devuelva datos (Puede ser una vista y datos que se controlar�n en el modelo de la p�gina)
 * @param {any} pBoton: Botón que ha desplegado la acción.
 * @param {any} pPanelID: ID del panel donde se devolverá ese código HTML devuelto por la petición mandada en el parámetro urlAccion
 * @param {any} pArg: Argumentos adicionales
 */
function DeployActionInModalPanel(urlAccion, pBoton, pPanelID, pArg) {    
    // Panel principal (padre) donde se mostrar�n todos los paneles
    var panel = $('#' + pPanelID);    
    // Panel donde se mostrar� el contenido
    var panelContent = panel.children('#content');
    // Panel de mensajes de OK/KO del contenedor padre (Posible error en la carga del servidor)
    var panelMessagesResult = panel.children().children('#resource-message-results');    
    // Ocultado mensaje de errores por defecto
    panelMessagesResult.css("display", "none");
    // Comprobar posibles parámetros
    let params = '';
    if (pArg != undefined || pArg != '') {
        params = pArg;
    } else {
        params = null;
    }

    // Realizar la petici�n AJAX
    GnossPeticionAjax(urlAccion, params, true).done(function (data) {
        panelContent.html(data);        
        // Ocultar panel de mensajes mensajes
        panelMessagesResult.css("display", "none");       
        panelContent.css("display", "block");
        // Llamar a inicializar las DataTable dentro del modal para acci�n "Historico" en Recurso
        accionHistorial.montarTabla();
        // Llamar a inicializar los despliegues para acci�n "Categorizar" en Recurso        
        accionDesplegarCategorias.init();

        // Recargar CKEditor si hubiera alg�n Input con clase de CKEditor
        if ($(panelContent).find('.cke').length > 0) RecargarTodosCKEditor(); 

    }).fail(function (data) {
        if (data == 'invitado') {
            operativaLoginEmergente.init();
            OcultarUpdateProgress();
            CerrarPanelAccion(pPanelID);
        } else {            
            // Mostrar mensaje de error
            panelMessagesResult.css("display", "block");
            panelContent.css("display", "none");
            panelMessagesResult.children(".ok").css("display", "none");
            panelMessagesResult.children(".ok").css("display", "block");
            panelMessagesResult.children(".ok").html('error');            
        }
    });
}


// Resetear el contenido del contenedor Modal para que la informaci�n no est� visible y tenga que volver a cargarse de nuevo.
// El contenedor #modal-container es utilizado para albergar modales de un Recurso de tal manera que pueda reutilizarse cada vez que este se cierra.
// Ej: En ficha de recurso, el modal "Historico" se carga de forma din�mica. Cuando se cierre el contenedor padre, habr�a que quitar el contenido para volver a ser reutilizado
var resetModalContainer = {
    // Inicializar el comportamiento cuando la p�gina web est� cargada
    init: function () {        
        $("#modal-container").on("hidden.bs.modal", function () {            
            // Añadir la clase por defecto para que se muestre en el top de la página
            $(this).addClass("modal-top");
            // Panel que hay que resetear/rellenar con el initialContainerContent
            var panelToReset = $(this).find("#content");
            // HTML que cargaremos de nuevo una vez se cierre el formulario (resetearlo de inicio)
            var initialContainerContent = '';
            initialContainerContent += '<div class="modal-header">';
            initialContainerContent += '<p class="modal-title">';
            initialContainerContent += '<span class="spinner-border white-color mr-2"></span>';
            initialContainerContent += 'Cargando ...';
            initialContainerContent += '</p>';
            initialContainerContent += '<span class="material-icons cerrar" data-dismiss="modal" aria-label="Close">close</span>';
            initialContainerContent += '</div>';
            initialContainerContent += '<div class="modal-body"></div>';
            initialContainerContent += '<div id="resource-message-results" class="modal-footer">';
            initialContainerContent += '<div class="ok"></div>';
            initialContainerContent += '<div class="ko"></div>';
            initialContainerContent += '</div>';
            // Vuelvo a incluir el panel inicial para ser reutilizado (ocultar el contenido previo si se abri� antes el modal)
            panelToReset.html(initialContainerContent).fadeIn();
        });
        return;
    },

       
    // Vaciar el contenedor de un modal y dejarlo como "loading" hasta que este vuelva a llenarse con datos v�a API REST (Ficha Recurso: Eliminar - Eliminar Selectivo)
    // Sirve para volver a llenar un modal sin que este sea cerrado.
    resetModalContent: function () {
        // Contenedor padre de los modales
        var $modalContainer = $("#modal-container");
        // Panel donde se vaciar� el contenido actual para emular la carga (Loading)
        var panelToReset = $modalContainer.find("#content");
        // HTML que cargaremos de nuevo una vez se cierre el formulario (resetearlo de inicio)
        var initialContainerContent = '';
        initialContainerContent += '<div class="modal-header">';
        initialContainerContent += '<p class="modal-title">';
        initialContainerContent += '<span class="spinner-border white-color mr-2"></span>';
        initialContainerContent += 'Cargando ...';
        initialContainerContent += '</p>';
        initialContainerContent += '<span class="material-icons cerrar" data-dismiss="modal" aria-label="Close">close</span>';
        initialContainerContent += '</div>';
        initialContainerContent += '<div class="modal-body"></div>';
        initialContainerContent += '<div id="resource-message-results" class="modal-footer">';
        initialContainerContent += '<div class="ok"></div>';
        initialContainerContent += '<div class="ko"></div>';
        initialContainerContent += '</div>';
        // Vuelvo a incluir el panel inicial para ser reutilizado (ocultar el contenido previo si se abri� antes el modal)
        panelToReset.html(initialContainerContent);       
    },
};

// Activar al inicio de carga de la página
$(function () {
    // Inicialización de reseteo de contenedor de modales (Ficha Recurso)
    resetModalContainer.init();
    // Activacin de tooltips de bootstrap para que estén disponibles en toda la página
    $('[data-toggle="tooltip"]').tooltip();
});


function DesplegarAccionMVC(urlAccion, pBoton, pPanelID, pArg) {
    var panel = $('#' + pPanelID);

    panel.children().children('#loading').css("display", "block");
    panel.children().children('#menssages').css("display", "none");
    panel.children().children('#action').css("display", "none");

    panel.children().css("display", "block");
    panel.css("display", "block");

    if (pBoton != null && typeof (pBoton.parentNode) != 'undefined') {
        DesactivarBotonesActivosDespl();
        pBoton.parentNode.className += ' active';
    }

    GnossPeticionAjax(urlAccion, null, true).done(function (data) {
        panel.children().children('#action').html(data);
        panel.children().children('#loading').css("display", "none");
        panel.children().children('#menssages').css("display", "none");
        panel.children().children('#action').css("display", "block");
    }).fail(function (data) {
        if (data == 'invitado') {
            operativaLoginEmergente.init();
            OcultarUpdateProgress();
            CerrarPanelAccion(pPanelID);
        } else {
            panel.children().children('#loading').css("display", "none");
            panel.children().children('#menssages').css("display", "block");
            panel.children().children('#action').css("display", "none");

            panel.children().children('#menssages').children(".ok").css("display", "none");
            panel.children().children('#menssages').children(".ko").css("display", "block");
            panel.children().children('#menssages').children(".ko").html('ERROR');
        }
    });
}

/**
 * Acción para desplegar mensaje informativo una vez realizada una acción
 * @param {any} pPanelID: Panel id donde se ha de mostrar el mensaje informativo (antes de toastr)
 * @param {any} pOk: Valor booleano que indica si la acción ha sido OK o no.
 * @param {any} pHtml: Mensaje informativo que se mostrará al usuario.
 * @param {any} timeOut: Tiempo de espera hasta que se muestra la notificación correcta. (milisegundos)
 */
function DesplegarResultadoAccionMVC(pPanelID, pOk, pHtml, timeOut = 1500) {
    var panel = $('#' + pPanelID);

    /* No añadir <p> al mensaje de texto
     * if (pHtml != null && pHtml != '' && pHtml.indexOf('<p') != 0) {
        pHtml = '<p>' + pHtml + '</p>';
    }*/

    if (pOk) {
        /* Cambiado por nuevo Front -> Toastr
        panel.children().children('#menssages').children(".ok").css("display", "block");
        panel.children().children('#menssages').children(".ko").css("display", "none");

        panel.children().children('#menssages').children(".ok").html(pHtml);
        */

        // Hacer desaparecer el modal si la acción es correcta
        $('#modal-container').modal('hide');

        // Mostrar mensaje OK
        setTimeout(() => {
            mostrarNotificacion('success', pHtml);
        }, timeOut)
    }
    else {
        /* Cambiado por nuevo Front -> Toastr
        panel.children().children('#menssages').children(".ok").css("display", "none");
        panel.children().children('#menssages').children(".ko").css("display", "block");

        panel.children().children('#menssages').children(".ko").html(pHtml);
        */
        mostrarNotificacion('error', pHtml);
    }

    /* Cambiado por nuevo Front -> Toastr    
    panel.children().children('#loading').css("display", "none");
    panel.children().children('#menssages').css("display", "block");
    panel.children().children('#action').css("display", "none");

    panel.children().css("display", "block");
    panel.css("display", "block");
    */

    DesactivarBotonesActivosDespl();
}

function AccionRecurso_PintarVotos(data) {
    $('#panUtils1 .votos').remove();
    $('#panUtils1 .votosPositivos').remove();
    $('#panUtils1 .panelVotosSimple').remove();
    $('#panUtils1 .panelVotosAmpliado').remove();
    $('#panUtils1 .visitas').after(data);
    presentacionVotosRecurso.init();
}

function CambiarTextoPorTextoAux(that) {
    var texto = $(that).text().trim();
    var textoAux = $(that).attr('textoAux');

    $(that).html($(that).html().replace(texto, textoAux));
    $(that).attr('textoAux', texto);
}

function CambiarOnClickPorOnclickAux(that) {
    var onclick = $(that).attr('onclick');
    var onclickAux = $(that).attr('onclickAux');

    $(that).attr('onclick', onclickAux);
    $(that).attr('onclickAux', onclick);
}

/**
 * Acción de realizar voto positivo de un recurso. Realización de una forma más visual.
 * - Aparecerá un pequeño "Loading" durante la acción del voto
 * - Actualizará el num de votos cuando finalice la acción
 * - Estará disponible la acción inversa cuando finalice el voto realizado.
 * @param {any} that: El botón pulsado (Span) con el icono de thumbs_up_alt
 * @param {any} urlVotarRecurso: La URL para realizar el voto
 * @param {any} urlVotarRecursoInvertido: La URL para realizar el voto contrario/invertido
 */
function AccionRecurso_VotarPositivo(that, urlVotarRecurso, urlVotarRecursoInvertido) {
    $(that).parent().find('.eleccionUsuario').removeClass("eleccionUsuario");
    $(that).addClass("eleccionUsuario");

    // Par�metros
    var funcionVotarInvertido = "AccionRecurso_VotarEliminar(this, '" + urlVotarRecursoInvertido + "', '" + urlVotarRecurso + "')";
    var iconoVoto = "thumb_up_alt";
    //var iconoVotoInvertido = "thumb_down_alt";
    // Ser� el mismo icono pero cambiado el color
    var iconoVotoInvertido = iconoVoto;
    var nombreClaseVotoOK = "activo"
    var claseMaterialIcons = "material-icons";
    var $numVotos = $(that).parent().find(".number");
    var numVotosActual = parseInt($numVotos.text());

    // Clase de tipo "Loading"
    var loadingClass = "spinner-border spinner-border-sm texto-primario"       

    // Ocultar icono del voto actual para mostrar solo el "Loading"
    $(that).html("");    
    // Elimino la clase de material icons para poder cambiar el color
    $(that).removeClass(claseMaterialIcons);    
    // Muestrar loading hasta que se complete la petici�n de "Votar"
    $(that).addClass(loadingClass);

    if (urlVotarRecurso != "") {
        GnossPeticionAjax(urlVotarRecurso, null, true).done(function (data) {
            AccionRecurso_PintarVotos(data);
            EnviarAccGogAnac('Acciones sociales', 'Votar', urlVotarRecurso);
            if (typeof voteAnalitics != 'undefined') {
                voteAnalitics.voteResource(1);
            }
            // Cambiar icono a Voto negativo        
            $(that).html(iconoVotoInvertido);
            // Cambiar la funci�n a realizar a Voto negativo
            $(that).attr("onclick", funcionVotarInvertido);
            // A�ado de nuevo la clase de material icons
            $(that).addClass(claseMaterialIcons);
            // A�ado la clase de megusta directamente al padre
            $(that).parent().toggleClass(nombreClaseVotoOK);
            $(that).removeClass(loadingClass);
            // Cambiar el n�mero del voto realizado a +1
            numVotosActual += 1;
            $numVotos.html(numVotosActual);
        }).fail(function (data) {
            // A�ado de nuevo la clase de material icons
            $(that).addClass(claseMaterialIcons);
            // Cambiar el loading y volver a como estaba antes (original) del error        
            $(that).html(iconoVoto);
            $(that).removeClass(loadingClass);
            if (data == "invitado") { operativaLoginEmergente.init(); }
        });
    } else {
        // A�ado de nuevo la clase de material icons
        $(that).addClass(claseMaterialIcons);
        // Cambiar el loading y volver a como estaba antes (original) del error        
        $(that).html(iconoVoto);
        $(that).removeClass(loadingClass);
        operativaLoginEmergente.init();
    }
}

function AccionRecurso_VotarPositivoListado(that, urlVotarRecurso) {
    if ($(that).parents('.acciones').find('.acc_nomegusta').hasClass('active')) {
        var botonNoMeGusta = $(that).parents('.acciones').find('.acc_nomegusta a')[0];
        CambiarTextoPorTextoAux(botonNoMeGusta);
        $(botonNoMeGusta).parent().removeClass('active')
        CambiarOnClickPorOnclickAux($(botonNoMeGusta));
    }

    CambiarTextoPorTextoAux(that);
    $(that).parent().addClass("active");
    CambiarOnClickPorOnclickAux($(that));

    GnossPeticionAjax(urlVotarRecurso, null, true).done(function (data) {
        EnviarAccGogAnac('Acciones sociales', 'Votar', urlVotarRecurso);
        if (typeof voteAnalitics != 'undefined') {
            voteAnalitics.voteResource(1);
        }
    }).fail(function (data) {
        if (data == "invitado") { operativaLoginEmergente.init(); }
    });
}


function AccionRecurso_VotarNegativo(that, urlVotarRecurso) {
    $(that).parent().find('.eleccionUsuario').removeClass("eleccionUsuario");
    $(that).addClass("eleccionUsuario");

    GnossPeticionAjax(urlVotarRecurso, null, true).done(function (data) {
        AccionRecurso_PintarVotos(data);
        EnviarAccGogAnac('Acciones sociales', 'Votar Negativamente', urlVotarRecurso);
        if (typeof voteAnalitics != 'undefined') {
            voteAnalitics.voteResource(-1);
        }
    }).fail(function (data) {
        if (data == 'invitado') { operativaLoginEmergente.init(); }
    });
}


function AccionRecurso_VotarNegativoListado(that, urlVotarRecurso) {
    
    if ($(that).parents('.acciones').find('.acc_megusta').hasClass('active')) {
        var botonMeGusta = $(that).parents('.acciones').find('.acc_megusta a')[0];
        CambiarTextoPorTextoAux(botonMeGusta);
        $(botonMeGusta).parent().removeClass('active')
        CambiarOnClickPorOnclickAux($(botonMeGusta));
    }

    CambiarTextoPorTextoAux(that);
    $(that).parent().addClass("active");
    CambiarOnClickPorOnclickAux($(that));

    GnossPeticionAjax(urlVotarRecurso, null, true).done(function (data) {
        EnviarAccGogAnac('Acciones sociales', 'Votar Negativamente', urlVotarRecurso);
        if (typeof voteAnalitics != 'undefined') {
            voteAnalitics.voteResource(-1);
        }
    }).fail(function (data) {
        if (data == 'invitado') { operativaLoginEmergente.init(); }
    });
}

/**
 * Acci�n de realizar voto negativo de un recurso. Realizaci�n de una forma m�s visual.
 * - Aparecer� un peque�o "Loading" durante la acci�n del voto
 * - Actualizar� el num de votos cuando finalice la acci�n
 * - Estar� disponible la acci�n inversa cuando finalice el voto realizado.
 * @param {any} that: El bot�n pulsado (Span) con el icono de thumbs_up_alt
 * @param {any} urlVotarRecurso: La URL para realizar el voto
 * @param {any} urlVotarRecursoInvertido: La URL para realizar el voto contrario/invertido
 */

function AccionRecurso_VotarEliminar(that, urlVotarRecurso, urlVotarRecursoInvertido) {
    $(that).parent().find('.eleccionUsuario').removeClass("eleccionUsuario");

    // Par�metros
    var funcionVotarInvertido = "AccionRecurso_VotarPositivo(this, '" + urlVotarRecursoInvertido + "', '" + urlVotarRecurso + "')";
    var iconoVoto = "thumb_up_alt";
    //var iconoVotoInvertido = "thumb_up_alt";
    // Ser� el mismo icono pero cambiado el color
    var iconoVotoInvertido = iconoVoto;
    var nombreClaseVotoOK = "activo"
    var claseMaterialIcons = "material-icons";
    var $numVotos = $(that).parent().find(".number");
    var numVotosActual = parseInt($numVotos.text());

    // Clase de tipo "Loading"
    var loadingClass = "spinner-border spinner-border-sm texto-primario"

    // Ocultar icono del voto actual para mostrar solo el "Loading"
    $(that).html("");
    // Elimino la clase de material icons para poder cambiar el color
    $(that).removeClass(claseMaterialIcons);
    // Muestrar loading hasta que se complete la petici�n de "Votar"
    $(that).addClass(loadingClass);


    if (urlVotarRecurso != "") {
        GnossPeticionAjax(urlVotarRecurso, null, true).done(function (data) {
            AccionRecurso_PintarVotos(data);
            if (typeof voteAnalitics != 'undefined') {
                voteAnalitics.voteResource(0);
            }
            // Cambiar icono a Voto negativo        
            $(that).html(iconoVotoInvertido);
            // Cambiar la funci�n a realizar a Voto negativo
            $(that).attr("onclick", funcionVotarInvertido);
            // A�ado de nuevo la clase de material icons
            $(that).addClass(claseMaterialIcons);
            $(that).removeClass(loadingClass);
            // A�adimos la clase para el color a voto realizado (Like)
            $(that).parent().toggleClass(nombreClaseVotoOK);
            // Cambiar el n�mero del voto realizado a +1
            numVotosActual -= 1;
            $numVotos.html(numVotosActual);
        }).fail(function (data) {
            // A�ado de nuevo la clase de material icons
            $(that).addClass(claseMaterialIcons);
            // Cambiar el loading y volver a como estaba antes (original) del error        
            $(that).html(iconoVoto);
            $(that).removeClass(loadingClass);
            var error = data.substr(data.indexOf('.') + 1);
            if (error == 'Invitado') { operativaLoginEmergente.init(); }
        });
    } else {
        // A�ado de nuevo la clase de material icons
        $(that).addClass(claseMaterialIcons);
        // Cambiar el loading y volver a como estaba antes (original) del error        
        $(that).html(iconoVoto);
        $(that).removeClass(loadingClass);
        operativaLoginEmergente.init();
    }
}


function AccionRecurso_VotarEliminarListado(that, urlVotarRecurso) {
    if ($(that).parents('.acciones').find('.acc_megusta').hasClass('active')) {
        CambiarTextoPorTextoAux($(that).parents('.acciones').find('.acc_megusta a')[0]);
        $(that).parents('.acciones').find('.acc_megusta').removeClass('active')
    }
    if ($(that).parents('.acciones').find('.acc_nomegusta').hasClass('active')) {
        CambiarTextoPorTextoAux($(that).parents('.acciones').find('.acc_nomegusta a')[0]);
        $(that).parents('.acciones').find('.acc_nomegusta').removeClass('active')
    }

    CambiarOnClickPorOnclickAux($(that));

    GnossPeticionAjax(urlVotarRecurso, null, true).done(function (data) {
        if (typeof voteAnalitics != 'undefined') {
            voteAnalitics.voteResource(0);
        }
    }).fail(function (data) {
        var error = data.substr(data.indexOf('.') + 1);
        if (error == 'Invitado') { operativaLoginEmergente.init(); }
    });
}

function AccionRecurso_EnviarNewsletter_Aceptar(idioma, urlEnviarNewsletter, documentoID) {
    MostrarUpdateProgress();

    var dataPost = {
        Language: idioma
    }
    
    GnossPeticionAjax(urlEnviarNewsletter, dataPost, true).done(function (data) {
        DesplegarResultadoAccionMVC("despAccionRec_" + documentoID, true, data);
        OcultarUpdateProgress();
    }).fail(function (data) {
        DesplegarResultadoAccionMVC("despAccionRec_" + documentoID, false, data);
        OcultarUpdateProgress();
    });
}

function AccionRecurso_EnviarNewsletterGrupos_Aceptar(idioma, urlEnviarNewsletterGrupos, documentoID, listaGrupos) {
    MostrarUpdateProgress();

    if (listaGrupos.indexOf('&') == 0) {
        listaGrupos = listaGrupos.substr(1);
    }

    var dataPost = {
        Language: idioma,
        Groups: listaGrupos.split('&')
    }

    GnossPeticionAjax(urlEnviarNewsletterGrupos, dataPost, true).done(function (data) {
        DesplegarResultadoAccionMVC("despAccionRec_" + documentoID, true, data);
        OcultarUpdateProgress();
    }).fail(function (data) {
        DesplegarResultadoAccionMVC("despAccionRec_" + documentoID, false, data);
        OcultarUpdateProgress();
    });
}

function AccionRecurso_RestaurarVersion_Aceptar(urlRestaurar, documentoID) {
    MostrarUpdateProgress();
    
    GnossPeticionAjax(urlRestaurar, null, true).done(function (data) {
        window.location.href = data;
    }).fail(function (data) {
        var html = textoRecursos.restaurarFALLO;
        if (data != "") {
            html = data;
            html = html.substr(html.indexOf('|') + 1);
        }
        DesplegarResultadoAccionMVC("despAccionRec_" + documentoID, false, html);
        OcultarUpdateProgress();
    });
}

function AccionRecurso_Vincular_Aceptar(urlVincularRecurso, urlCargarVinculados, documentoID) {
    MostrarUpdateProgress();
    var urlDocVinculado = $("#txtUrlDocVinculado_" + documentoID).val();

    var datosPost = {
        UrlResourceLink: urlDocVinculado
    }
    GnossPeticionAjax(urlVincularRecurso, datosPost, true).done(function (data) {
        EnviarAccGogAnac('Acciones sociales', '"Vincular recurso', urlVincularRecurso);
        DesplegarResultadoAccionMVC("despAccionRec_" + documentoID, true, textoRecursos.recursoVinculadoOK);
        OcultarUpdateProgress();
        Vinculados_CargarVinculados(urlCargarVinculados);
    }).fail(function (data) {
        DesplegarResultadoAccionMVC("despAccionRec_" + documentoID, false, data);
        OcultarUpdateProgress();
    });
}

function AccionRecurso_MetaTitulo(urlMetatitulo, documentoID) {
    MostrarUpdateProgress();
    var txtMetaTitulo = $("#txtMetaTitulo_" + documentoID).val();

    var datosPost = {
        MetaTitle: txtMetaTitulo
    }
    GnossPeticionAjax(urlMetatitulo, datosPost, true).done(function (data) {
        EnviarAccGogAnac('Acciones sociales', 'Añadir Metatitulo', urlMetatitulo);
        DesplegarResultadoAccionMVC("despAccionRec_" + documentoID, true, textoRecursos.guardadoOK);
        OcultarUpdateProgress();
    }).fail(function (data) {
        DesplegarResultadoAccionMVC("despAccionRec_" + documentoID, false, data);
        OcultarUpdateProgress();
    });
}

function AccionRecurso_MetaDescripcion(urlMetaDescripcion, documentoID) {
    MostrarUpdateProgress();
    var txtMetaDescripcion = $("#txtMetaDescripcion_" + documentoID).val();

    var datosPost = {
        MetaDescripcion: txtMetaDescripcion
    }
    GnossPeticionAjax(urlMetaDescripcion, datosPost, true).done(function (data) {
        EnviarAccGogAnac('Acciones sociales', 'Añadir MetaDescripcion', urlMetaDescripcion);
        DesplegarResultadoAccionMVC("despAccionRec_" + documentoID, true, textoRecursos.guardadoOK);
        OcultarUpdateProgress();
    }).fail(function (data) {
        DesplegarResultadoAccionMVC("despAccionRec_" + documentoID, false, data);
        OcultarUpdateProgress();
    });
}

/**
 * Mostrar el mensaje correcto de recurso desvinculado. 
 * Seguidamente, realiza la petici�n para cargar los nuevos recursos vinculados.
 * @param {any} urlDesvincularRecurso
 * @param {any} urlCargarVinculados
 * @param {any} documentoID
 * @param {any} documentoDesVincID
 */
function AccionRecurso_DesVincular_Aceptar(urlDesvincularRecurso, urlCargarVinculados, documentoID, documentoDesVincID) {
    MostrarUpdateProgress();

    var datosPost = {
        ResourceUnLinkKey: documentoDesVincID
    }

    GnossPeticionAjax(urlDesvincularRecurso, datosPost, true).done(function () {
        // Cambiado por nuevo Front
        //DesplegarResultadoAccionMVC("despAccionRec_" + documentoDesVincID, true, textoRecursos.DesvincularOK);
        DesplegarResultadoAccionMVC("modal-dinamic-action-response", true, textoRecursos.DesvincularOK);
        // Esperar 1,5 segundos y ocultar el panel
        setTimeout(() => {
            $('#modal-container').modal('hide');
        }, 1500);
        OcultarUpdateProgress();      
        Vinculados_CargarVinculados(urlCargarVinculados);
    }).fail(function () {
        // Cambiado por nuevo Front
        //DesplegarResultadoAccionMVC("despAccionRec_" + documentoDesVincID, false, textoRecursos.DesvincularFALLO);
        DesplegarResultadoAccionMVC("modal-dinamic-action-response", false, textoRecursos.DesvincularFALLO);
        OcultarUpdateProgress();
    });
}

function Vinculados_CargarVinculados(urlCargarVinculados) {
    GnossPeticionAjax(urlCargarVinculados, null, true).done(function (data) {
        $('#panVinculadosInt').html(data);
        CompletadaCargaContextos();
    });
}

/**
 * Actualizar la acci�n para poder certificar un recurso. Debido al cambio del Front, ahora no se hace mediante "Select" sino con Radio
 * @param {any} urlPaginaCertificar: Url para realizar la acci�n de certificar un recurso.
 * @param {any} documentoID: Documento ID o identificador del recurso.
 * @param {any} textoCertificado: Parece ser simplemente el texto de "certificaci�n".
 */
function AccionRecurso_Certificar_Aceptar(urlPaginaCertificar, documentoID, textoCertificado) {
    MostrarUpdateProgress();
    // Por cambio en el Front
    // var comboCertificado = $("#comboCertificado_" + documentoID);
    // var valorSeleccionado = comboCertificado.find("option:selected").text();   

    // Cogemos el atributo "data-value" que ser� el valor de la label elegida
    var valorSeleccionado = $('input[name=certificar-recurso]:checked').attr("data-value");
    // Cogemos id o value (no el texto en bruto) de la opci�n seleccionada en el radio button.
    var opcionSeleccionada = $('input[name=certificar-recurso]:checked').attr("data-option");
    
    var dataPost = {
        //CertificationID: comboCertificado.val()
        CertificationID: opcionSeleccionada
    }
    
    GnossPeticionAjax(urlPaginaCertificar, dataPost, true).done(function (data) {
        if ($('#panUtils1').length > 0) {
            $('#contCertificado').remove();
            if (comboCertificado.val() != "00000000-0000-0000-0000-000000000000") {
                $('#panUtils1').append("<div id=\"contCertificado\"><p class=\"certificado\"><span class=\"icono\"></span>" + textoCertificado + ":<strong>" + valorSeleccionado + "</strong></p></div>");
            }
        }
        DesplegarResultadoAccionMVC("despAccionRec_" + documentoID, true, textoRecursos.certificacionOK);
        OcultarUpdateProgress();
    }).fail(function (data) {
        DesplegarResultadoAccionMVC("despAccionRec_" + documentoID, false, textoRecursos.certificacionFALLO);
        OcultarUpdateProgress();
    });
}

function AccionRecurso_Eliminar_Aceptar(urlEliminarDocumento, documentoID) {
    MostrarUpdateProgress();

    if (typeof resourceAnalitics != 'undefined') {
        resourceAnalitics.resourceDeleted();
    }

    GnossPeticionAjax(urlEliminarDocumento, null, true).done(function (data) {
        DesplegarResultadoAccionMVC("despAccionRec_" + documentoID, true, data);
    }).fail(function (data) {
        DesplegarResultadoAccionMVC("despAccionRec_" + documentoID, false, data);
    });
}

function AccionRecurso_EliminarSelectivo_Aceptar(urlEliminarDocumento, documentoid) {
    MostrarUpdateProgress();

    var div = $("#eliminarSelectivo_" + documentoid);

    var i = 0;
    var lista = {};
    div.parent().find('input:checked').each(function () {
        lista[i] = this.id;
        i++;
    });

    var dataPost = {
        SharedCommunities: lista
    }

    if (typeof resourceAnalitics != 'undefined') {
        resourceAnalitics.resourceDeleted();
    }

    GnossPeticionAjax(urlEliminarDocumento, dataPost, true);
}

function BloquearComentarios(btnBloquear, textDesbloquear, urlBloquearComentarios, documentoID) {
    MostrarUpdateProgress();

    //var dataPost = {
    //    callback: 'acciones|AccionRecurso_Comentarios_Bloquear'
    //}
    GnossPeticionAjax(urlBloquearComentarios, null, true).done(function (data) {
        var html = textoRecursos.comentBloqOK;

        var accion = $(btnBloquear).attr('onclick');
        accion = accion.replace("BloquearComentarios(", "DesbloquearComentarios(");
        accion = accion.replace("/lock-comments", "/unlock-comments");
        accion = accion.replace(textDesbloquear, $(btnBloquear).text());
        $(btnBloquear).attr('onclick', accion);

        $(btnBloquear).html($(btnBloquear).html().replace($(btnBloquear).text(), textDesbloquear));
        
        DesplegarResultadoAccionMVC("despAccionRec_" + documentoID, true, html);
        // Recargar la página para no permitir comentar un recurso y viceversa
        setTimeout(() => {
            window.location.reload();
        }, 1000);

        OcultarUpdateProgress();
    }).fail(function (data) {
        DesplegarResultadoAccionMVC("despAccionRec_" + documentoID, false, data);

        OcultarUpdateProgress();
    });
}

function DesbloquearComentarios(btnDesbloquear, textBloquear, urlDesbloquearComentarios, documentoID) {
    MostrarUpdateProgress();

    //var dataPost = {
    //    callback: 'acciones|AccionRecurso_Comentarios_DesBloquear'
    //}

    //$.post(urlDesbloquearComentarios, dataPost, function (data) {
    GnossPeticionAjax(urlDesbloquearComentarios, null, true).done(function (data) {
        var html = textoRecursos.comentDesBloqOK;

        var accion = $(btnDesbloquear).attr('onclick');
        accion = accion.replace("DesbloquearComentarios(", "BloquearComentarios(");
        accion = accion.replace("/unlock-comments", "/lock-comments");
        accion = accion.replace(textBloquear, $(btnDesbloquear).text());
        $(btnDesbloquear).attr('onclick', accion);

        $(btnDesbloquear).html($(btnDesbloquear).html().replace($(btnDesbloquear).text(), textBloquear));
        
        DesplegarResultadoAccionMVC("despAccionRec_" + documentoID, true, html);

        OcultarUpdateProgress();
    }).fail(function (data) {
        DesplegarResultadoAccionMVC("despAccionRec_" + documentoID, false, data);

        OcultarUpdateProgress();
    });
}

function CrearVersionDocSem(urlPagina, documentoID) {
    MostrarUpdateProgress();

    $.post(urlPagina + '/create-version-semantic-document', function (data) {
        var html = "";
        var ok = true;
        if (data.indexOf("OK") != 0) {
            ok = false;
            html = data;
            html = html.substr(html.indexOf('|') + 1);
            DesplegarResultadoAccionMVC("despAccionRec_" + documentoID, ok, html);

            OcultarUpdateProgress();
        }
        else {
            window.location = data.substr(data.indexOf('|') + 1);
        }
    });
}


function AccionRecurso_ReportPage_Aceptar(urlReportPage, documentoID) {
    MostrarUpdateProgress();
    var panelDespl = $('#despAccionRec_' + documentoID);
    var mensaje = $(".reportPage textarea", panelDespl).val();

    var dataPost = {
        message: mensaje
    }

    GnossPeticionAjax(urlReportPage, dataPost, true).done(function (data) {
        DesplegarResultadoAccionMVC("despAccionRec_" + documentoID, true, data);
    }).fail(function (error) {
        DesplegarResultadoAccionMVC("despAccionRec_" + documentoID, false, error);
    }).always(function () {
        OcultarUpdateProgress();
    });
}

function AccionRecurso_AgregarTags_Aceptar(urlAgregarTags, documentoID, tags, permisoEditarTags, urlBaseTags) {
    MostrarUpdateProgress();

    var listaTags = tags.split(',');

    var lista = {};
    for (var i = 0; i < listaTags.length; i++) {
        if (listaTags[i].trim() != "") {
            lista[i] = listaTags[i];
        }
    }

    var dataPost = {
        Tags: lista
    }
    
    GnossPeticionAjax(urlAgregarTags, dataPost, true).done(function () {
        var html = textoRecursos.tagsOK;

        EnviarAccGogAnac('Acciones sociales', 'Añadir etiquetas', urlAgregarTags);

        var htmlTags = "";
        //var separador = ", ";
        if (permisoEditarTags) {
            separador = "";
        }
        $.each(tags.split(','), function () {
            tag = this.trim();
            if (tag != "") {
                var enlaceCompleto = urlBaseTags + "/" + tag;
                htmlTags += "<li><a href=\"" + enlaceCompleto + "\" rel=\"sioc:topic\" resource=\"" + enlaceCompleto + "\"><span typeof=\"sioc_t:Tag\" property=\"dcterms:name\" about=\"" + enlaceCompleto + "\">" + tag + "</span></a></li>";
                //separador = ", ";
            }
        });
        
        // Cambio por nuevo Front
        //var ul = $('#despAccionRec_' + documentoID).parents('.wrapDescription').children('.group.etiquetas').find('ul');
        const ul = $('.group.etiquetas').find('.listTags');
        var ultimoLI = ul.children().last();

        if (ultimoLI.find('a').hasClass("verTodasCategoriasEtiquetas")) {
            if (permisoEditarTags) {
                ul.children().each(function () {
                    if (!$(this).find('a').hasClass("verTodasCategoriasEtiquetas")) {
                        $(this).remove();
                    }
                });
            }
            ultimoLI.before(htmlTags);
            
            if (ultimoLI.find('a').hasClass('mas')) {
                ultimoLI.find('a').click();
            }
        }
        else {
            if (permisoEditarTags) {
                ul.html(htmlTags);
            }
            else {
                ultimoLI.after(htmlTags);
            }
        }
        
        DesplegarResultadoAccionMVC("despAccionRec_" + documentoID, true, html);

        OcultarUpdateProgress();
    }).fail(function (data) {
        var html = textoRecursos.tagsFALLO;
        var numError = data;
        if (numError == "0") {
            html = form.errordtag;
        }

        DesplegarResultadoAccionMVC("despAccionRec_" + documentoID, false, html);

        OcultarUpdateProgress();
    });
}

function AccionRecurso_AgregarCategorias_Aceptar(urlAgregarCategorias, documentoID, categorias, permisoEditarCategorias, urlBaseCategorias) {
    MostrarUpdateProgress();

    var listaCategorias = categorias.split(',');

    var lista = {};
    for (var i = 0; i < listaCategorias.length; i++) {
        if (listaCategorias[i].trim() != "") {
            lista[i] = listaCategorias[i];
        }
    }

    var dataPost = {
        Categories: lista
    }
    
    GnossPeticionAjax(urlAgregarCategorias, dataPost, true).done(function () {
        var html = textoRecursos.categoriasOK;

        EnviarAccGogAnac('Acciones sociales', 'Añadir recurso a categoría', urlAgregarCategorias);

        var htmlCategorias = "";
        var separador = "";
        /*if (permisoEditarCategorias) {
            separador = "";
        }*/
        $.each(categorias.split(','), function () {
            categoriaID = this.trim();
            if (categoriaID != "") {
                var nombreCat = $('#despAccionRec_' + documentoID).find('.divCategorias').find('.' + categoriaID).find('label').text();
                var enlaceCompleto = urlBaseCategorias + "/" + nombreCat.toLowerCase().replace(' ', '-') + "/" + categoriaID;
                htmlCategorias += "<li>" + separador + "<a href=\"" + enlaceCompleto + "\"><span>" + nombreCat + "</span></a></li>";
                //separador = ", ";
            }
        });

        //Cambio por nuevo Front
        //var ul = $('#despAccionRec_' + documentoID).parents('.wrapDescription').children('.group.categorias').find('ul');
        const ul = $('.group.categorias').find('.listCat');
        var ultimoLI = ul.children().last();
           
        if (ultimoLI.find('a').hasClass("verTodasCategoriasEtiquetas")) {
            //if (permisoEditarCategorias) {
                ul.children().each(function () {
                    if (!$(this).find('a').hasClass("verTodasCategoriasEtiquetas")) {
                        $(this).remove();
                    }
                });
            //}
            ultimoLI.before(htmlCategorias);

            if (ultimoLI.find('a').hasClass('mas')) {
                ultimoLI.find('a').click();
            }
        }
        else {
            /*if (permisoEditarCategorias) {*/
                ul.html(htmlCategorias);
            /*}
            else {
                ultimoLI.after(htmlCategorias);
            }*/
        }
        
        DesplegarResultadoAccionMVC("despAccionRec_" + documentoID, true, html);

        OcultarUpdateProgress();
    }).fail(function (data) {
        var html = textoRecursos.categoriasFALLO;
        var numError = data;
        if (numError == "0") {
            html = form.errordcategoriaSelect;
        }

        DesplegarResultadoAccionMVC("despAccionRec_" + documentoID, false, html);

        OcultarUpdateProgress();
    });
}

/**
 * Método que se ejecuta desde la acción de "Compartir" para actualizar las comunidades existentes y así poder compartir el recurso donde se deseee.
 * @param {any} urlAccionCambiarCompartir: Llamada URL para proceder a realizar la acción de Compartir-Cambiar y así actualizar la lista
 * @param documentoID: El ID del documento o del recurso en cuestión
 * @param {any} personaID: El id de la persona que ha realizado la acción
 * @param {any} pListaUrlsAutocompletar
 */
function AccionRecurso_Compartir_Cambiar(urlAccionCambiarCompartir, documentoID, personaID, pListaUrlsAutocompletar) {
    MostrarUpdateProgress();
    var panelDespl = $('#despAccionRec_' + documentoID);
    var txtBaseRecursos = panelDespl.find('#ddlCompartir option:selected');

    var dataPost = {
        SelectedBR: txtBaseRecursos.val()
    }

    GnossPeticionAjax(urlAccionCambiarCompartir, dataPost, true).done(function (data) {
        var html = "";

        // Comprobar si es un objeto o array (La versión 5 devuelve un objeto de arrays)             
        if (!Array.isArray(data)) {
            // V5 -> Objeto de arrays -> Acceder al elemento 2 del objeto
            for (var i in data.$values) {
                html += data.$values[i].html;
            }
        } else {
            // V4 -> Array solo ha sido devuelto
            for (var i in data) {
                html += data[i].html;
            }
        }

        // Actualizarlo por el nuevo Front
        // panelDespl.find('#divSelCatTesauro').html(html);
        //panelDespl.find('.divCategorias').html(html);        
        panelDespl.find('#panCategoriasTesauroArbol').html(html);
        // Llamar a inicializar los despliegues para acci�n "Compartir" en Recurso        
        accionDesplegarCategorias.init()               

        panelDespl.find('#txtSeleccionados').val('');

        var liBaseRecursos = panelDespl.find('#panContenedorBRAgregadas').find('ul.icoEliminar #' + txtBaseRecursos.val());
        if (liBaseRecursos.length > 0)
        {
            var catSeleccionadas = liBaseRecursos.find('input').val().split('$$')[1]
            panelDespl.find('#txtSeleccionados').val(catSeleccionadas);
            $.each(catSeleccionadas.split(','), function () {
                if (this != "") {
                    panelDespl.find('#divSelCatTesauro').find('span.' + this + ' input').prop('checked', true);
                    var imgVerMas = panelDespl.find('#divSelCatTesauro').find('span.' + this + ' input').parents('.panHijos').parent().children('img');
                    $.each(imgVerMas, function () {
                        if (this.src.indexOf('verMas') > 0) {
                            MVCDesplegarTreeView(this);
                        }
                    });
                }
            });
        }

        if (panelDespl.find('#panEditoresLectores').length > 0)
        {
            if (txtBaseRecursos.attr("rel") == "private") {
                panelDespl.find('#panEditoresLectores').find('#panLectores').show();
                panelDespl.find('#panEditoresLectores').find('#panEditores').show();
            }
            else if (txtBaseRecursos.attr("rel") == "public") {
                panelDespl.find('#panEditoresLectores').find('#panLectores').hide();
                panelDespl.find('#panEditoresLectores').find('#panEditores').show();
            }
            else if (txtBaseRecursos.attr("rel") == "org") {
                panelDespl.find('#panEditoresLectores').find('#panLectores').hide();
                panelDespl.find('#panEditoresLectores').find('#panEditores').hide();
            }
        }

        AccionRecurso_Compartir_GenerarAutocompletar(documentoID, personaID, pListaUrlsAutocompletar);

        OcultarUpdateProgress();
    });
}

function AccionRecurso_AgregarABR_Aceptar(urlAceptarGuardarEn, urlCargarCompartidos, documentoID) {
    MostrarUpdateProgress();
    var panelDespl = $('#despAccionRec_' + documentoID);
    var categorias = panelDespl.find('#txtSeleccionados').val().split(',');

    var lista = {};
    for (var i = 0; i < categorias.length; i++) {
        if (categorias[i].trim() != "") {
            lista[i] = categorias[i];
        }
    }

    var dataPost = {
        categoriesList: lista
    }

    GnossPeticionAjax(urlAceptarGuardarEn, dataPost, true).done(function () {
        $('#resource_' + documentoID + ' .acc_guardar').remove();
        $('#divGroupAccionesRec .opAddPersonal').remove();

        DesplegarResultadoAccionMVC("despAccionRec_" + documentoID, true, textoRecursos.compartirBROK);

        if (typeof shareAnalitics != 'undefined') {
            shareAnalitics.sharePersonalSpace();
        }
    }).fail(function () {
        DesplegarResultadoAccionMVC("despAccionRec_" + documentoID, false, textoRecursos.compartirBRFALLO);

    }).always(function () {
        if ($('#divCompartido').length > 0) {
            var dataPost2 = {
                callback: 'Compartidos|CargarCompartidos'
            }
            $.post(urlCargarCompartidos, dataPost2, function (data) {
                $('#divCompartido').html(data);
            });
        }
        OcultarUpdateProgress();
    });
}

function AccionRecurso_AgregarCategoriaABR_Aceptar(urlAceptarGuardarEn, urlCargarCompartidos, documentoID) {
    MostrarUpdateProgress();
    var panelDespl = $('#despAccionRec_' + documentoID);
    panelDespl.find('#menssages.createCat').hide();
    panelDespl.find('#menssages.createCat .ko').hide();
    panelDespl.find('#menssages.createCat .ok').hide();
    var nombre = panelDespl.find('#inputNombreCategoria').val();
    var idCategoriaPadre = panelDespl.find('#selectCategoriaPadre').val();
    
    var dataPost = {
        categoryName: nombre,
        parentCategoryID: idCategoriaPadre
    }

    GnossPeticionAjax(urlAceptarGuardarEn, dataPost, true).done(function (mensaje) {   
        var marcados = panelDespl.find("#action input:checked");
        panelDespl.find("#action").html(mensaje);
        var txtSeleccionados = panelDespl.find('#txtSeleccionados');
        if(marcados.length > 0){
            for(var i = 0; i < marcados.length; i++){
                $("#" + marcados[i].id).prop( "checked", true );
                var claseInput = $("#" + marcados[i].id).parent().attr("class");
                txtSeleccionados.val(txtSeleccionados.val() + claseInput + ',');
            }
        }
        panelDespl.find('#menssages.createCat .ok').html('<p>' + textoRecursos.categoriasOK + '</p>');
        panelDespl.find('#menssages.createCat').show();
        panelDespl.find('#menssages.createCat .ok').show();
    }).fail(function (error) {
        panelDespl.find('#menssages.createCat').show();
        panelDespl.find('#menssages.createCat .ko').html('<p>' + error + '</p>');
        panelDespl.find('#menssages.createCat .ko').show();
    }).always(function () {
        panelDespl.find('#menssages.createCat').show();
        OcultarUpdateProgress();
    });
}

function AccionRecurso_Compartir_Aceptar(urlAceptarCompartir, documentoID, urlDocumento) {
    MostrarUpdateProgress();
    var panelDespl = $('#despAccionRec_' + documentoID);
    var inputsBaseRecursos = panelDespl.find('#panContenedorBRAgregadas').find('ul.icoEliminar li input');
    var categorias = {};
    var contCategorias = 0;
    $.each(inputsBaseRecursos, function () {
        categorias[contCategorias] = $(this).val();
        contCategorias++;
    });

    var editores = {};
    var txtEditores = panelDespl.find('#panEditores').find('#divContEditores').find('#txtHackEditores_' + documentoID);
    if (txtEditores.length > 0) {
        var listaEditores = txtEditores.val().split(',');
        var contEditores = 0;
        for (var i = 0; i < listaEditores.length; i++) {
            if (listaEditores[i].trim() != "") {
                editores[contEditores] = listaEditores[i];
                contEditores++;
            }
        }
    }

    var lectores = {};
    var txtLectores = panelDespl.find('#panLectores').find('#divContLectores').find('#txtHackLectores_' + documentoID);
    if (txtLectores.length > 0) {
        var listaLectores = txtLectores.val().split(',');
        var contLectores = 0;
        for (var i = 0; i < listaLectores.length; i++) {
            if (listaLectores[i].trim() != "") {
                lectores[contLectores] = listaLectores[i];
                contLectores++;
            }
        }
    }

    var visibleSoloEditores = panelDespl.find('#panLectores').find('#rbLectoresEditores').is(':checked');
    
    var dataPost = {
        Categories: categorias,
        Editors: editores,
        Readers: lectores,
        Private: visibleSoloEditores
    }

    GnossPeticionAjax(urlAceptarCompartir, dataPost, true).done(function () {
        DesplegarResultadoAccionMVC("despAccionRec_" + documentoID, true, textoRecursos.compartirOK);

        if (typeof shareAnalitics != 'undefined') {
            //$.each(categorias, function () {
            //    var br = this.split("$$")[0];
            //    shareAnalitics.shareCommunity(br);
            //});
            if (typeof shareAnalitics != 'undefined') {
                shareAnalitics.shareCommunity();
            }
        }

    }).fail(function () {
        DesplegarResultadoAccionMVC("despAccionRec_" + documentoID, false, textoRecursos.compartirFALLO);
    }).always(function () {
        OcultarUpdateProgress();
        //if ($('#divCompartido').length > 0) {
            var dataPost2 = {
                callback: "Compartidos|CargarCompartidos"
            }

            GnossPeticionAjax(urlDocumento, dataPost2, true).done(function (data) {
                // Comprobar que existe el elemento -> Si no, crearlo
                if (($(".info-recurso", "#contAutoresEditoresLectores").find("#divCompartido").length) == 0) {
                    const divCompartido = `<div class="group compartida" id="divCompartido"><p class="group-title">Compartida con: </p></div>`;
                    $(".info-recurso", "#contAutoresEditoresLectores").append(divCompartido);
                }
                $('#divCompartido').html(data);
            });
        //}
    });
}

function AccionRecurso_Compartir_GenerarAutocompletar(documentoID, personaID, pListaUrlsAutocompletar) {
    var panelDespl = $('#despAccionRec_' + documentoID);
    
    var panEditores = panelDespl.find('#panEditores').find('#divContEditores');
    var panLectores = panelDespl.find('#panLectores').find('#divContLectores');

    CargarAutocompletarLectoresEditoresPorBaseRecursos(panEditores.find('#txtEditores_' + documentoID), panelDespl.find('#ddlCompartir option:selected').val(), personaID, 'txtHackEditores_' + documentoID, pListaUrlsAutocompletar, 'despAccionRec_' + documentoID, 'panContenedorEditores', 'txtHackEditores_' + documentoID);

    CargarAutocompletarLectoresEditoresPorBaseRecursos(panLectores.find('#txtLectores_' + documentoID), panelDespl.find('#ddlCompartir option:selected').val(), personaID, 'txtHackLectores_' + documentoID, pListaUrlsAutocompletar, 'despAccionRec_' + documentoID, 'panContenedorLectores', 'txtHackLectores_' + documentoID);
}

function AccionRecurso_Compartir_AjustarPrivacidadRecurso(documentoID, pRbSelecc) {
    var panelDespl = $('#despAccionRec_' + documentoID);

    var panEditores = panelDespl.find('#panEditores').find('#divContEditores');
    var panLectores = panelDespl.find('#panLectores').find('#divContLectores');

    if (pRbSelecc.id == 'rbEditoresYo') {
        $.each(panelDespl.find('#panEditores').find('#panContenedorEditores > ul > li').children(), function (i, v)         {
            $(this).click();
        });
        panEditores.hide();
    }
    else if (pRbSelecc.id == 'rbEditoresOtros') {
        panEditores.show();
    }
    else if (pRbSelecc.id == 'rbLectoresComunidad' || pRbSelecc.id == 'rbLectoresEditores') {
        $.each(panelDespl.find('#panLectores').find('#panContenedorLectores > ul > li').children(), function (i, v)         {
            $(this).click();
        });
        panLectores.hide();
    }
    else if (pRbSelecc.id == 'rbLectoresEspecificos') {
        panLectores.show();
    }
}

/**
 * Método para agregar una categoría desde la ficha Recurso al acceder a "Compartir.
 * Cuando se hace click sobre una categoría, esta se posiciona en un panel informativo para que el usuario posteriormente, confirme la acción.
 * Se ha incluido un botón de "delete" para que quede más intuitivo y parecido al nuevo Front.
 * @param documentoID: El id del documento que será asociado a una determinada comunidad.
 */
 function AccionRecurso_Compartir_Agregar(documentoID) {
    MostrarUpdateProgress();

    var panelDespl = $('#despAccionRec_' + documentoID);

    var txtBaseRecursosID = panelDespl.find('#ddlCompartir option:selected').val();
    var txtSeleccionadas = panelDespl.find('#txtSeleccionados').val();
    // Cambiar debido al nuevo Front
    //var tieneCategorias = $('#divSelCatTesauro', panelDespl).children().length > 0;    
    var tieneCategorias = $('.divCategorias', panelDespl).children().length > 0;

    if (txtBaseRecursosID != "" && (txtSeleccionadas != "" || !tieneCategorias))
    {
        var html = "<li id='" + txtBaseRecursosID + "' class='d-flex align-content-start'>" +
        "   <input type=\"hidden\" value=\"" + txtBaseRecursosID + "$$" + txtSeleccionadas + "\" />" +
            "<span class=`mr-2`><strong>" + panelDespl.find('#ddlCompartir option:selected').text() + "</strong></span>";

        var numCat = 0;
        $.each(txtSeleccionadas.split(','), function () {
            if (this != "") {
                // Cambiar debido al nuevo Front
                // var nombreCat = panelDespl.find('#divSelCatTesauro').find('span.' + this + ' label').text();
                var nombreCat = panelDespl.find('.divCategorias').find('div.' + this + ' label').text();
                if (numCat > 0) {
                    html += "   <label>,</label>";
                }
                html += "   <label class='ml-2'>" + nombreCat + "</label>";

                numCat++;
            }
        });

        html += "   <a onclick=\"AccionRecurso_Compartir_Eliminar('" + txtBaseRecursosID + "', '" + documentoID + "')\" class=\"remove\"><span class=\"material-icons\" style=\"cursor:pointer\">delete</span></a>" +
        "</li>";

        panelDespl.find('#panContenedorBRAgregadas').find('ul.icoEliminar #' + txtBaseRecursosID).remove();
        panelDespl.find('#panContenedorBRAgregadas').find('ul.icoEliminar').append(html);

        panelDespl.find('#panContenedorBRAgregadas').show();
        panelDespl.find('#panBotonAceptar').show();
    }

    OcultarUpdateProgress();
}

function AccionRecurso_Compartir_Eliminar(baseRecursosID, documentoID) {
    var panelDespl = $('#despAccionRec_' + documentoID);
    var txtBaseRecursosID = panelDespl.find('#ddlCompartir option:selected').val();
    if (txtBaseRecursosID == baseRecursosID)
    {
        panelDespl.find('#txtSeleccionados').val('');
        panelDespl.find('#divSelCatTesauro').find('input:checked').prop('checked', false);
    }
    panelDespl.find('#panContenedorBRAgregadas').find('ul.icoEliminar #' + baseRecursosID).remove();

    if (panelDespl.find('#panContenedorBRAgregadas').find('ul.icoEliminar').children().length == 0)
    {
        panelDespl.find('#panContenedorBRAgregadas').hide();
        panelDespl.find('#panBotonAceptar').hide();
    }
}

function AccionRecurso_Compartir_Eliminar(baseRecursosID, documentoID) {
    var panelDespl = $('#despAccionRec_' + documentoID);
    var txtBaseRecursosID = panelDespl.find('#ddlCompartir option:selected').val();
    if (txtBaseRecursosID == baseRecursosID)
    {
        panelDespl.find('#txtSeleccionados').val('');
        panelDespl.find('#divSelCatTesauro').find('input:checked').prop('checked', false);
    }
    panelDespl.find('#panContenedorBRAgregadas').find('ul.icoEliminar #' + baseRecursosID).remove();

    if (panelDespl.find('#panContenedorBRAgregadas').find('ul.icoEliminar').children().length == 0)
    {
        panelDespl.find('#panContenedorBRAgregadas').hide();
        panelDespl.find('#panBotonAceptar').hide();
    }
}

/**
 * Acción para descompartir un recurso de una determinada comunidad
 * @param {any} urlDescompartir: Url a la que habrá que llamar para aplicar la acción de descompartir
 * @param {any} btnDescompartir: El botón de la papelera de la comunidad de la que se desea descompartir
 * @param {any} brID: 
 * @param {any} proyID: Id del proyecto
 * @param {any} orgID: Id de la organización
 */
function AccionRecurso_Descompartir(urlDescompartir, btnDescompartir, brID, proyID, orgID) {
    // Mostrar loading
    MostrarUpdateProgress();
    
    const dataPost = {
        BR: brID,
        ProyectID: proyID,
        OrganizationID: orgID
    };
    
    GnossPeticionAjax(urlDescompartir, dataPost, true)
        .done(function () {
            const ul = $(btnDescompartir).parent().parent();
            $(btnDescompartir).parent().remove();
            if (ul.children().length == 0) {
                ul.parent().remove();
            }
        })
        .always(function () {
            OcultarUpdateProgress();
        });    
}

function AccionRecurso_Duplicar_Aceptar(urlAceptarDuplicar, documentoID, urlDocumento) {
    MostrarUpdateProgress();
    var panelDespl = $('#despAccionRec_' + documentoID);

    var txtBaseRecursos = panelDespl.find('#ddlCompartir option:selected');

    if (txtBaseRecursos.length > 0) {
        var urlDuplicarRecurso = txtBaseRecursos[0].value;
        urlDuplicarRecurso += urlAceptarDuplicar;
        window.location.href = urlDuplicarRecurso;
    } else {
        DesplegarResultadoAccionMVC("despAccionRec_" + documentoID, false, textoRecursos.compartirFALLO);
    }
}

/**
 * Método para votar un recurso de tipo Encuesta
 * @param {any} urlPagina
 * @param {any} documentoID
 */
function AccionRecurso_Encuesta_Votar(urlPagina, documentoID) {
    MostrarUpdateProgress();
    var panelContenedor = $('#encuesta_' + documentoID);
    var opcionEncuesta = panelContenedor.find('input[type = radio]:checked');

    if (opcionEncuesta.length > 0) {
        var dataPost = {
            callback: 'Encuesta|Votar|' + opcionEncuesta.attr('id')
        }

        $.post(urlPagina, dataPost, function (data) {
            panelContenedor.html(data);            
            OcultarUpdateProgress();
        });
    }
    else {
        OcultarUpdateProgress();
    }
}

function AccionRecurso_Encuesta_VerResultados(urlPagina, documentoID) {
    MostrarUpdateProgress();
    var panelContenedor = $('#encuesta_' + documentoID);
    var dataPost = {
        callback: 'Encuesta|VerResultados'
    }

    $.post(urlPagina, dataPost, function (data) {
        panelContenedor.html(data);
        OcultarUpdateProgress();
    });
}

function Comentario_CrearComentario(urlCrearComentario, documentoID) {
    if ($('#txtNuevoComentario_' + documentoID).val() != '') {
        MostrarUpdateProgress();
        $('#error_' + documentoID).html('');
        var descripcion = encodeURIComponent($('#txtNuevoComentario_' + documentoID).val());
        var datosPost = {
            Description: descripcion
        };
        GnossPeticionAjax(urlCrearComentario, datosPost, true).done(function (data) {
            $('span#numComentarios').text(parseInt($('span#numComentarios').text()) + 1);
            $('#txtNuevoComentario_' + documentoID).val('');
            var html = "";
            // Comprobar si es un objeto o array (La versión 5 devuelve un objeto de arrays)             
            if (!Array.isArray(data)) {
                // V5 -> Objeto de arrays -> Acceder al elemento 2 del objeto
                for (var i in data.$values) {
                    html += data.$values[i].html;
                }
            } else {
                // V4 -> Array solo ha sido devuelto
                for (var i in data) {
                    html += data[i].html;
                }
            }

            $('#panComentarios').html(html);
            if ((typeof CompletadaCargaComentarios != 'undefined')) {
                CompletadaCargaComentarios();
            }
            MontarFechas();

            if (typeof commentsAnalitics != 'undefined') {
                var comentarioID = $('#panComentarios .comment').first().attr('id');
                commentsAnalitics.commentCreated(comentarioID);
            }

            OcultarUpdateProgress();
        });
    }
    else {
        $('#error_' + documentoID).html(comentarios.comentarioError);
    }
}

/**
 * Acción para rechazar una comunidad (Utilizado en V5)
 * @param {any} urlRechazar: Url de api al que realizar la petición
 * @param {any} peticionID: ID de la solicitud
 */
function Peticion_RechazarSolicitudNuevaComunidad(urlRechazar, peticionID) {
    MostrarUpdateProgress();

    //contamos comentarios hijos para restarselos
    var panComentario = $('#' + peticionID);
    var datosPost = {
        peticion_id: peticionID
    };
    GnossPeticionAjax(urlRechazar, datosPost, true).done(function (data) {
        $('span#panNumResultados').text(parseInt($('span#panNumResultados').text()) - 1);
        $('#' + peticionID).remove();

        OcultarUpdateProgress();
    });
}
/**
 * Acción para aceptar una comunidad (Utilizado en V5)
 * @param {any} urlRechazar: Url de api al que realizar la petición
 * @param {any} peticionID: ID de la solicitud
 */
function Peticion_AceptarSolicitudNuevaComunidad(urlRechazar, peticionID) {
    MostrarUpdateProgress();

    //contamos comentarios hijos para restarselos
    var panComentario = $('#' + peticionID);
    var datosPost = {
        peticion_id: peticionID
    };
    GnossPeticionAjax(urlRechazar, datosPost, true).done(function (data) {
        $('span#panNumResultados').text(parseInt($('span#panNumResultados').text()) - 1);
        $('#' + peticionID).remove();

        OcultarUpdateProgress();
    });
}
/**
 * Acción para rechazar a un usuario de la comunidad (Utilizado en V5)
 * @param {any} urlRechazar: Url de api al que realizar la petición
 * @param {any} solicitudID: ID de la solicitud
 */
function Solicitud_RechazarSolicitudUsuarioComunidad(urlRechazar, solicitudID) {
    MostrarUpdateProgress();

    //contamos comentarios hijos para restarselos
    var panComentario = $('#' + solicitudID);
    var datosPost = {
        solicitud_id: solicitudID
    };
    GnossPeticionAjax(urlRechazar, datosPost, true).done(function (data) {
        $('span#panNumResultados').text(parseInt($('span#panNumResultados').text()) - 1);
        $('#' + solicitudID).remove();

        OcultarUpdateProgress();
    });
}
/**
 * Acción para aceptar a un usuario de la comunidad (Utilizado en V5)
 * @param {any} urlAceptar: Url de api al que realizar la petición
 * @param {any} solicitudID: ID de la solicitud
 */
function Solicitud_AceptarSolicitudUsuarioComunidad(urlAceptar, solicitudID) {
    MostrarUpdateProgress();

    //contamos comentarios hijos para restarselos
    var panComentario = $('#' + solicitudID);
    var datosPost = {
        solicitud_id: solicitudID
    };
    GnossPeticionAjax(urlAceptar, datosPost, true).done(function (data) {
        $('span#panNumResultados').text(parseInt($('span#panNumResultados').text()) - 1);
        $('#' + solicitudID).remove();

        OcultarUpdateProgress();
    });
}


/**
 * Acción para eliminar un comentario dentro de la ficha de un recurso
 * @param {any} urlEliminar: Url de api al que realizar la petición de borrado
 * @param {any} comentarioID: ID del comentario que se desea eliminar
 */
function Comentario_EliminarComentario(urlEliminar, comentarioID) {
    MostrarUpdateProgress();

    //contamos comentarios hijos para restarselos
    var panComentario = $('#' + comentarioID);
    var numComentariosRestar = $(panComentario).find('.comentario-contenido').length;

    GnossPeticionAjax(urlEliminar, null, true).done(function (data) {
        $('span#numComentarios').text(parseInt($('span#numComentarios').text()) - numComentariosRestar);

        $('#panComentarios #' + comentarioID).remove();

        if ((typeof CompletadaCargaEliminarComentarios != 'undefined')) {
            CompletadaCargaEliminarComentarios();
        }

        if (typeof commentsAnalitics != 'undefined') {
            commentsAnalitics.commentDeleted(comentarioID);
        }

        OcultarUpdateProgress();
    });
}

function Comentario_EliminarComentarioAnterior(urlEliminar, comentarioID) {
    MostrarUpdateProgress();

    //contamos comentarios hijos para restarselos
    var panComentario = $('#' + comentarioID);
    var numComentariosRestar = $(panComentario).find('.comment-content').length;
    
    GnossPeticionAjax(urlEliminar, null, true).done(function (data) {
        $('span#numComentarios').text(parseInt($('span#numComentarios').text()) - numComentariosRestar);

        $('#panComentarios #' + comentarioID).remove();

        if ((typeof CompletadaCargaEliminarComentarios != 'undefined')) {
            CompletadaCargaEliminarComentarios();
        }

        if (typeof commentsAnalitics != 'undefined') {
            commentsAnalitics.commentDeleted(comentarioID);
        }

        OcultarUpdateProgress();
    });
}

/**  
 * Método que se ejecuta cuando se pulsa en "Editar" un comentario dentro de la ficha de un recurso
 * @param {any} urlResponder: Url del comentario que se editará 
 * @param {any} comentarioID: ID del comentario que será editado
 */
function Comentario_EditarComentario(urlEditar, comentarioID) {
    // Panel dinamico del modal padre donde se insertara la vista "hija"
    const $modalDinamicContentPanel = $('#modal-container').find('#modal-dinamic-content #content');

    // Contenido del comentario a editar
    const panComentario = $('#' + comentarioID);
    const panTextoComentario = $(panComentario).find('.comentario-contenido');
    const mensajeAntiguo = panTextoComentario.html();

    // Plantilla del panel html que se cargar� en el modal contenedor al pulsar en la acci�n
    let plantillaPanelHtml = '';
    // Cabecera del panel
    plantillaPanelHtml += '<div class="modal-header">';
    plantillaPanelHtml += '<p class="modal-title"><span class="material-icons">label</span>Editar comentario</p>';
    plantillaPanelHtml += '<span class="material-icons cerrar" data-dismiss="modal" aria-label="Close">close</span>';
    plantillaPanelHtml += '</div>';
    // Cuerpo del panel
    plantillaPanelHtml += '<div class="modal-body">';
    plantillaPanelHtml += '<div class="formulario-edicion">';
    // Cuerpo de la respuesta - TextArea
    plantillaPanelHtml += '<div class="form-group">';
    plantillaPanelHtml += '<textarea class="form-control cke mensajes" id="txtComentario_Editar_' + comentarioID + '" rows="3">' + mensajeAntiguo + '</textarea>';
    plantillaPanelHtml += '</div>';

    // Contenedor de mensajes y botones
    plantillaPanelHtml += '<div id="modal-dinamic-action-response">';
    // Posibles mensajes de info
    plantillaPanelHtml += '<div>';
    plantillaPanelHtml += '<div class="menssages_' + comentarioID + '" id="menssages_' + comentarioID + '">';
    plantillaPanelHtml += '<div class="ok"></div>';
    plantillaPanelHtml += '<div class="ko"></div>';
    plantillaPanelHtml += '</div>';
    plantillaPanelHtml += '</div>';
    // Panel de botones para la acci�n
    plantillaPanelHtml += '<div id="modal-dinamic-action-buttons" class="form-actions">'
    plantillaPanelHtml += '<button class="btn btn-primary">Editar comentario</button>'
    plantillaPanelHtml += '</div>';
    plantillaPanelHtml += '</div>';
    plantillaPanelHtml += '</div>';

    // Meter el código de la vista modal en el contenedor padre que viene identificado por el id #modal-container
    // En concreto, hay que buscar la etiqueta modal-dinamic-content #content e insertar el c�digo
    $modalDinamicContentPanel.html(plantillaPanelHtml);

    // Acceso a los botones
    const botones = $modalDinamicContentPanel.find('#modal-dinamic-action-buttons > button');
    // Recargar editor CKE
    RecargarTodosCKEditor();

    // Asignación de la función al botón "Sí" o de acción
    $(botones[0]).on("click", function () {
        Comentario_EnviarEditarComentario(urlEditar, comentarioID);
    });
}

/**
 * Acción de poder editar un comentario sin modal
 * @param {any} urlEditar
 * @param {any} comentarioID
 */
function Comentario_Editar_JIRA(urlEditar, comentarioID) {

    // 0 - Botón de editar, guardar, cancelar
    const $btnEditar = $(`#edit-comment-${comentarioID}`);
    const $btnEditarMobile = $(`#edit-comment-mobile-${comentarioID}`);
    const $btnCancelar = $(`#cancel-comment-${comentarioID}`);
    const $btnCancelarMobile = $(`#cancel-comment-mobile-${comentarioID}`);
    const $btnGuardar = $(`#save-comment-${comentarioID}`);
    const $btnGuardarMobile = $(`#save-comment-mobile${comentarioID}`);

    // 1 - Detectar la caja de comentarios 
    const $comentarioEditar = $(`#${comentarioID}`).find('.comentario-contenido');
    // 2- Texto del comentario que se desea editar
    const textoComentarioEditar = $comentarioEditar.html();
    // 3- Crear un input text area dinámico para el CKEditor
    let ckeEditor = `<div class="form-group">`;
    ckeEditor += `<textarea cols="20" rows="3" id="txtComentario_Editar_${comentarioID}" class="cke ckeSimple comentarios">${textoComentarioEditar}</textarea>`;
    ckeEditor += `</div>`;

    // 4- Ocultar el comentario actual 
    $comentarioEditar.fadeOut();
    // 5- Añadir el CKEditor / TextArea
    $comentarioEditar.parent().append(ckeEditor)

    // 6- Ocultar el botón de Editar (ya se está editando) y mostrar "Guardar" y "Cancelar"
    $btnEditar.parent().toggleClass('d-none');
    $btnEditarMobile.parent().toggleClass('d-none');
    $btnCancelar.parent().toggleClass('d-none');
    $btnCancelarMobile.parent().toggleClass('d-none');
    $btnGuardar.parent().toggleClass('d-none');
    $btnGuardarMobile.parent().toggleClass('d-none');

    // Recargar editor CKE
    RecargarTodosCKEditor();
}

/**
 * Acción de poder Cancelar un comentario sin modal
 * @param {any} comentarioID
 */
function Comentario_Cancelar_JIRA(comentarioID) {

    // 0 - Botón de editar, guardar, cancelar
    const $btnEditar = $(`#edit-comment-${comentarioID}`);
    const $btnEditarMobile = $(`#edit-comment-mobile-${comentarioID}`);
    const $btnCancelar = $(`#cancel-comment-${comentarioID}`);
    const $btnCancelarMobile = $(`#cancel-comment-mobile-${comentarioID}`);
    const $btnGuardar = $(`#save-comment-${comentarioID}`);
    const $btnGuardarMobile = $(`#save-comment-mobile${comentarioID}`);

    // 1 - Detectar la caja de comentarios que estará oculta
    const $comentarioEditar = $(`#${comentarioID}`).find('.comentario-contenido');
    // 2 - Detectar el CKEditor que estará visible
    const $ckEditor = $(`#txtComentario_Editar_${comentarioID}`);
    // 3 - Remover el CKEditor actual (el padre contenedor) ya que se ha cancelado la acción
    $ckEditor.parent().remove();
    // 4 - Volver a mostrar la caja anterior del texto    
    $comentarioEditar.fadeIn();

    // 5- Ocultar el botón de Editar (ya se está editando) y mostrar "Guardar" y "Cancelar"
    $btnEditar.parent().toggleClass('d-none');
    $btnEditarMobile.parent().toggleClass('d-none');
    $btnCancelar.parent().toggleClass('d-none');
    $btnCancelarMobile.parent().toggleClass('d-none');
    $btnGuardar.parent().toggleClass('d-none');
    $btnGuardarMobile.parent().toggleClass('d-none');

}

function Comentario_EditarComentarioAnterior(urlEditar, comentarioID) {
    var panComentario = $('#' + comentarioID);

    var panTextoComentario = $(panComentario).find('.comment-content');
    panTextoComentario.hide();

    var mensajeAntiguo = panTextoComentario.html();

    $(panComentario).find('.comment-enviar').remove();
    $(panComentario).find('.comment-responder').remove();

    var accion = "javascript:Comentario_EnviarEditarComentario('" + urlEditar + "', '" + comentarioID + "');";
    var claseCK = 'cke comentarios';

    var confirmar = $(['<div class="comment-enviar"><fieldset class="mediumLabels"><legend>', comentarios.editarComentario, '</legend><p><textarea class="' + claseCK + '" id="txtComentario_Editar_', comentarioID, '" rows="2" cols="20">' + mensajeAntiguo + '</textarea><p><label class="error" id="error_', comentarioID, '"></label></p><input type="button" onclick="', accion, '" value="', comentarios.guardar, '" class="btn btn-primary text medium sendComment"></p></fieldset></div>'].join(''));

    panTextoComentario.after(confirmar);

    RecargarTodosCKEditor();
}

/**
 * Acción de Enviar al servidor (api) el comentario editado
 * @param {any} urlEditar: URL donde hay que hacer la petición para realizar la edición del comentario
 * @param {any} comentarioID: Id del comentario que se desea editar
 */
function Comentario_EnviarEditarComentario_JIRA(urlEditar, comentarioID) {
    if ($('#txtComentario_Editar_' + comentarioID).val() != '') {
        MostrarUpdateProgress();

        var descripcion = $('#txtComentario_Editar_' + comentarioID).val();

        var datosPost = {
            Description: encodeURIComponent(descripcion)
        };

        GnossPeticionAjax(urlEditar, datosPost, true).done(function (data) {
            if (typeof commentsAnalitics != 'undefined') {
                commentsAnalitics.commentModified(comentarioID);
            }
            // Ocultar el modal al haber editado el comentario            
            const panComentario = $('#' + comentarioID);
            const panTextoComentario = $(panComentario).find('.comentario-contenido');
            panTextoComentario.html(descripcion);
            // Ocultamos ckEditor y volvemos a la posición original
            Comentario_Cancelar_JIRA(comentarioID);
        })
            .always(function () {
                OcultarUpdateProgress();
            });
    }
}

/**
 * Acción de Enviar al servidor (api) el comentario editado
 * @param {any} urlEditar: URL donde hay que hacer la petición para realizar la edición del comentario
 * @param {any} comentarioID: Id del comentario que se desea editar
 */
function Comentario_EnviarEditarComentario(urlEditar, comentarioID) {
    if ($('#txtComentario_Editar_' + comentarioID).val() != '') {
        MostrarUpdateProgress();
        $('#error_' + comentarioID).html('');
        var descripcion = $('#txtComentario_Editar_' + comentarioID).val();

        var datosPost = {
            Description: encodeURIComponent(descripcion)
        };

        GnossPeticionAjax(urlEditar, datosPost, true).done(function (data) {
            if (typeof commentsAnalitics != 'undefined') {
                commentsAnalitics.commentModified(comentarioID);
            }
            // Ocultar el modal al haber editado el comentario
            $('#modal-container').modal('hide');
            const panComentario = $('#' + comentarioID);
            const panTextoComentario = $(panComentario).find('.comentario-contenido');
            panTextoComentario.html(descripcion);
        })
        .always(function () {
            OcultarUpdateProgress();
        });
    }
    else {
        $('#error_' + comentarioID).html(comentarios.comentarioError);
    }
}

function Comentario_EnviarEditarComentarioAnterior(urlEditar, comentarioID) {
    if ($('#txtComentario_Editar_' + comentarioID).val() != '') {
        MostrarUpdateProgress();
        $('#error_' + comentarioID).html('');
        var descripcion = $('#txtComentario_Editar_' + comentarioID).val();

        var datosPost = {
            Description: encodeURIComponent(descripcion)
        };

        GnossPeticionAjax(urlEditar, datosPost, true).done(function (data) {
            if (typeof commentsAnalitics != 'undefined') {
                commentsAnalitics.commentModified(comentarioID);
            }
        });

        var panComentario = $('#' + comentarioID);
        var panTextoComentario = $(panComentario).find('.comment-content');
        $(panComentario).find('.comment-enviar').remove();

        panTextoComentario.html(descripcion);
        panTextoComentario.show();

        OcultarUpdateProgress();
    }
    else {
        $('#error_' + comentarioID).html(comentarios.comentarioError);
    }
}

/**  
 * Método que se ejecuta cuando se pulsa en "Responder" un comentario dentro de la ficha de un recurso
 * @param {any} urlResponder: Url a la que se le responderá 
 * @param {any} comentarioID: ID del comentario al que se desea responder
 */
function Comentario_ResponderComentario(urlResponder, comentarioID) {
    // Panel dinamico del modal padre donde se insertara la vista "hija"
    const $modalDinamicContentPanel = $('#modal-container').find('#modal-dinamic-content #content');

    // Plantilla del panel html que se cargar� en el modal contenedor al pulsar en la acci�n
    let plantillaPanelHtml = '';
    // Cabecera del panel
    plantillaPanelHtml += '<div class="modal-header">';
    plantillaPanelHtml += '<p class="modal-title"><span class="material-icons">label</span>Responder comentario</p>';
    plantillaPanelHtml += '<span class="material-icons cerrar" data-dismiss="modal" aria-label="Close">close</span>';
    plantillaPanelHtml += '</div>';
    // Cuerpo del panel
    plantillaPanelHtml += '<div class="modal-body">';
    plantillaPanelHtml += '<div class="formulario-edicion">';
    // Cuerpo de la respuesta - TextArea
    plantillaPanelHtml += '<div class="form-group">';
    plantillaPanelHtml += '<textarea class="form-control cke mensajes" id="txtComentario_Responder_' + comentarioID + '" rows="3"> </textarea>';
    plantillaPanelHtml += '</div>';

    // Contenedor de mensajes y botones
    plantillaPanelHtml += '<div id="modal-dinamic-action-response">';
    // Posibles mensajes de info
    plantillaPanelHtml += '<div>';
    plantillaPanelHtml += '<div class="menssages_' + comentarioID + '" id="menssages_' + comentarioID + '">';
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

    // Meter el código de la vista modal en el contenedor padre que viene identificado por el id #modal-container
    // En concreto, hay que buscar la etiqueta modal-dinamic-content #content e insertar el c�digo
    $modalDinamicContentPanel.html(plantillaPanelHtml);

    // Acceso a los botones
    const botones = $modalDinamicContentPanel.find('#modal-dinamic-action-buttons > button');
    // Recargar editor CKE
    RecargarTodosCKEditor();

    // Asignación de la función al botón "Sí" o de acción
    $(botones[0]).on("click", function () {
        Comentario_EnviarResponderComentario(urlResponder, comentarioID);
    });

}


function Comentario_ResponderComentarioAnterior(urlResponder, comentarioID) {
    var panComentario = $('#' + comentarioID);

    var panTextoComentario = $(panComentario).find('.comment-content');

    $(panComentario).find('.comment-enviar').remove();
    $(panComentario).find('.comment-responder').remove();

    var accion = "javascript:Comentario_EnviarResponderComentario('" + urlResponder + "', '" + comentarioID + "');";
    var claseCK = 'cke comentarios';

    var confirmar = $(['<div class="comment-responder"><fieldset class="mediumLabels"><legend>', comentarios.responderComentario, '</legend><p><textarea class="' + claseCK + '" id="txtComentario_Responder_', comentarioID, '" rows="2" cols="20"></textarea><p><label class="error" id="error_', comentarioID, '"></label></p><input type="button" onclick="', accion, '" value="', comentarios.enviar, '" class="text medium sendComment"></p></fieldset></div>'].join(''));

    panTextoComentario.after(confirmar);

    RecargarTodosCKEditor();
}

function Comentario_ResponderComentario_V2(urlResponder, comentarioID) {
    var panComentario = $('#' + comentarioID);

    var panTextoComentario = $(panComentario).find('.comment-content');

    $(panComentario).find('.comment-enviar').remove();
    $(panComentario).find('.comment-responder').remove();

    var accion = "javascript:Comentario_EnviarResponderComentario('" + urlResponder + "', '" + comentarioID + "');";
    var claseCK = 'cke comentarios';

    var confirmar = $(['<div class="escribir-comentario"><div class="publicador"></div><div class="escribe"><fieldset class="mediumLabels"><p><textarea class="' + claseCK + '" id="txtComentario_Responder_', comentarioID, '" rows="2" cols="20"></textarea><p><label class="error" id="error_', comentarioID, '"></label></p><input type="button" onclick="', accion, '" value="', comentarios.enviar, '" class="btn btn-primary text medium sendComment"></p></fieldset></div></div>'].join(''));

    panTextoComentario.after(confirmar);

    RecargarTodosCKEditor();
}

/**
 * Acción de Envio al servidor del comentario respondido
 * @param {any} urlResponder: URL de la api hacia donde hay que realizar la respuesta
 * @param {any} comentarioID: ID del comentario que se enviará.
 */
function Comentario_EnviarResponderComentario(urlResponder, comentarioID) {
    if ($('#txtComentario_Responder_' + comentarioID).val() != '') {
        MostrarUpdateProgress();
        $('#error_' + comentarioID).html('');
        var descripcion = encodeURIComponent($('#txtComentario_Responder_' + comentarioID).val().replace(/\n/g, ''));
        var datosPost = {
            Description: descripcion
        };

        GnossPeticionAjax(urlResponder, datosPost, true).done(function (data) {
            // Ocultar el modal al haber enviado correctamente la respuesta
            $('#modal-container').modal('hide');
            $('span#numComentarios').text(parseInt($('span#numComentarios').text()) + 1);
            var html = "";
            // Comprobar si es un objeto o array (La versión 5 devuelve un objeto de arrays)            
            if (!Array.isArray(data)) {
                // V5 -> Objeto de arrays -> Acceder al elemento 2 del objeto
                for (var i in data.$values) {
                    html += data.$values[i].html;
                }
            } else {
                // V4 -> Array solo ha sido devuelto
                for (var i in data) {
                    html += data[i].html;
                }
            }

            $('#panComentarios').html(html);
            if ((typeof CompletadaCargaComentarios != 'undefined')) {
                CompletadaCargaComentarios();
            }
            MontarFechas();

            if (typeof commentsAnalitics != 'undefined') {
                var comentarioRespuestaID = $('#panComentarios #' + comentarioID + ' .comment').first().attr('id');
                commentsAnalitics.commentCreated(comentarioRespuestaID);
            }
            OcultarUpdateProgress();
        });
    }
    else {
        $('#error_' + comentarioID).html(comentarios.comentarioError);
    }
}

function Comentario_VotarPositivo(that, urlVotarPositivo, comentarioID) {
    MostrarUpdateProgress();
    $(that).parent().find('.eleccionUsuario').removeClass("eleccionUsuario");
    $(that).addClass("eleccionUsuario");

    $.post(urlVotarPositivo, null, function (data) {
        var html = "";
        for (var i in data) {
            html += data[i].html;
        }
        $('#panComentarios').html(html);
        if ((typeof CompletadaCargaVotoComentario != 'undefined')) {
            CompletadaCargaVotoComentario();
        }
        MontarFechas();
        OcultarUpdateProgress();
    });
}

function Comentario_VotarNegativo(that, urlVotarNegativo, comentarioID) {
    MostrarUpdateProgress();
    $(that).parent().find('.eleccionUsuario').removeClass("eleccionUsuario");
    $(that).addClass("eleccionUsuario");

    $.post(urlVotarNegativo, null, function (data) {
        var html = "";
        for (var i in data) {
            html += data[i].html;
        }
        $('#panComentarios').html(html);
        if ((typeof CompletadaCargaVotoComentario != 'undefined')) {
            CompletadaCargaVotoComentario();
        }
        MontarFechas();
        OcultarUpdateProgress();
    });
}

function Comentario_EliminarVoto(that, urlEliminarVoto, comentarioID) {
    MostrarUpdateProgress();
    $(that).parent().find('.eleccionUsuario').removeClass("eleccionUsuario");

    $.post(urlEliminarVoto, null, function (data) {
        var html = "";
        for (var i in data) {
            html += data[i].html;
        }
        $('#panComentarios').html(html);
        if ((typeof CompletadaCargaVotoComentario != 'undefined')) {
            CompletadaCargaVotoComentario();
        }
        MontarFechas();
        OcultarUpdateProgress();
    });
}

function MontarFacetaFichaFormSem(pUrlFac, pGrafo, pFaceta, pParametros, pIdentidad, pLanguageCode, pPanelID) {

    pIdentidad = pIdentidad.toUpperCase();
    var servicio = new WS(pUrlFac, WSDataType.jsonp);

    var metodo = 'CargarFacetas';
    var params = {};
    params['pProyectoID'] = pGrafo;
    //params['bool_esMyGnoss'] = 'False';
    params['pEstaEnProyecto'] = true;
    params['pEsUsuarioInvitado'] = (pIdentidad == 'FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF');
    params['pIdentidadID'] = pIdentidad;
    params['pUbicacionBusqueda'] = 'MetahomeCatalogo';
    params['pParametros'] = pParametros;
    params['pLanguageCode'] = pLanguageCode;
    params['pPrimeraCarga'] = false;
    params['pAdministradorVeTodasPersonas'] = false;
    params['pTipoBusqueda'] = 0;
    params['pNumeroFacetas'] = 1;
    params['pFaceta'] = pFaceta;
    params['pGrafo'] = pGrafo;
    params['pFiltroContexto'] = '';
    params['pParametros_adiccionales'] = 'factFormSem=true';
    params['pUsarMasterParaLectura'] = false;

    servicio.call(metodo, params, function (data) {
        var descripcion = data;
        $('#' + pPanelID).show();
        $('#' + pPanelID).html(descripcion);
        $('.facetedSearchBox', $('#' + pPanelID)).hide();

        if ((typeof CompletadaCargaFacetasFichaRec != 'undefined')) {
            CompletadaCargaFacetasFichaRec();
        }
    });
}

function PaginarSelectorEnt(link, urlAccion, entidad, propiedad, elemsPag, totalElem) {
    var divPadre = $($(link).parent('.pagSelectEnt')[0]);

    if (divPadre.data('pag') == null) {
        divPadre.data('pag', 0);
        divPadre.data('elemsCarg', 1);
    }

    var pagActual = divPadre.data('pag');
    var elemsCarg = divPadre.data('elemsCarg');

    if (link.className == 'sigPagSelectEnt') {
        if (totalElem <= (elemsPag * (pagActual + 1))) {
            return;
        }

        pagActual++;

        if (pagActual > (elemsCarg - 1)) {
            var divCargando = $(divPadre.parent().children('.loadpagSelectEnt')[0]);
            var dataPost = { entidad: entidad, propiedad: propiedad, inicioPag: (elemsPag * pagActual) };
            GnossPeticionAjax(urlAccion, dataPost, true).done(function (data) {
                divCargando.before('<div class="pagSelEnt" style="display:none;">' + data + '</div>');
                elemsCarg++;
                divPadre.data('elemsCarg', elemsCarg);
            }).fail(function () {
                pagActual--;
            }).always(function () {
                divCargando.css('display', 'none');
                divPadre.css('display', '');
                AjustarDatosSelectorEntPaginado(pagActual, divPadre);
            });

            divCargando.css('display', '');
            divPadre.css('display', 'none');
        }
    }
    else if (pagActual > 0) {
        pagActual--;
    }

    divPadre.data('pag', pagActual);
    
    AjustarDatosSelectorEntPaginado(pagActual, divPadre);
}

function AjustarDatosSelectorEntPaginado(pagActual, divPadre) {
    var divDatos = divPadre.parent().children('.pagSelEnt');

    for (var i = 0; i < divDatos.length; i++) {
        if (i == pagActual) {
            $(divDatos[i]).css('display', '');
        }
        else {
            $(divDatos[i]).css('display', 'none');
        }
    }
}

function MontarFechaCliente() {
    if (typeof (diffHoras) == 'undefined' || diffHoras == null) {
        var fechaServidor = new Date($('#inpt_serverTime').val());
        $('p.publicacion, span.fechaPublicacion').each(function (index) {
            if ($(this).attr('content') != null) {
                var fechaRecurso = new Date($(this).attr('content'));
                var fechaCliente = new Date();
                //var diffHoras = parseInt((fechaServidor.getTime() / (1000 * 60 * 60)) - (fechaCliente.getTime() / (1000 * 60 * 60)));
                var diffMinutos = parseInt((fechaServidor.getTime() / (1000 * 60)) - (fechaCliente.getTime() / (1000 * 60)));
                var diffHoras = diffMinutos / 60;
                //redondeo
                var resto = diffMinutos % 60;
                if (resto / 60 > 0.5) {
                    if (diffHoras > 0) {
                        diffHoras = diffHoras + 1;
                    }
                    else {
                        diffHoras = diffHoras - 1;
                    }
                }
                fechaRecurso.setHours(fechaRecurso.getHours() - diffHoras);
                var dia = fechaRecurso.getDate();
                if (dia < 10) {
                    dia = '0' + dia;
                }
                var mes = fechaRecurso.getMonth() + 1;
                if (mes < 10) {
                    mes = '0' + mes;
                }
                //var fechaPintado = fechaRecurso.format("yyyy/MM/dd HH:mm");
                var fechaPintado = tiempo.fechaPuntos.replace('@1@', dia).replace('@2@', mes).replace('@3@', fechaRecurso.getFullYear());
                $(this).html(fechaPintado);
                $(this).show();
            }
        });
    }
}

function InitializeRouteMapGoogle(pDivID, pRoute, pColor) {
    try {
        var mapOptions = {
            zoom: 3,
            center: new google.maps.LatLng(0, 0),
            mapTypeId: google.maps.MapTypeId.TERRAIN
        };

        var map = new google.maps.Map(document.getElementById(pDivID), mapOptions);

        var mapbounds = new google.maps.LatLngBounds();

        pRoute = pRoute.replace(/\;/g, ',');
        var puntosParseados = JSON && JSON.parse(pRoute) || $.parseJSON(pRoute);
        var puntosRuta = [];
        for (var i = 0; i < puntosParseados["geo:coordinates"].length; i++) {
            puntosRuta[i] = new google.maps.LatLng(puntosParseados["geo:coordinates"][i][0], puntosParseados["geo:coordinates"][i][1]);

            mapbounds.extend(puntosRuta[i]);
        }

        map.fitBounds(mapbounds);
        map.setCenter(mapbounds.getCenter());

        if (pColor == '' || pColor == null) {
            pColor = '#FF0000';
        }

        var ruta = new google.maps.Polyline({
            path: puntosRuta,
            geodesic: true,
            strokeColor: pColor,
            strokeOpacity: 1.0,
            strokeWeight: 2
        });

        ruta.setMap(map);
    }
    catch (ex) {
    }
}

$(function () {
    $('input.btnAccionSemCms').click(function () { AccionFichaRecSemCms(this); });
});

function AccionFichaRecSemCms(boton) {
    if (typeof (AccionFichaRecSemCmsPersonalizado) != 'undefined') {
        AccionFichaRecSemCmsPersonalizado(boton);
        return;
    }

    var that = boton;
    var idBtn = $(boton).attr('id');
    idBtn = idBtn.replace('btn_', '');
    var arg = {};
    arg.ActionID = idBtn;
    arg.ActionEntityID = $(boton).attr('rel');
    $('#errorbtn_' + idBtn).remove();

    if (arg.ActionEntityID == null || arg.ActionEntityID == '') {
        return;
    }

    MostrarUpdateProgressTime(0);

    GnossPeticionAjax(urlActionSemCms, arg, true).done(function (data) {
        $(that).after('<div id="errorbtn_' + idBtn + '" class="ok" style="display:block">' + data + '</div>');
    }).fail(function (data) {
        var mensajeError = data;
        if (data == "NETWORKERROR") {
            mensajeError = 'Has perdido la conexión. Comprueba tu conexión a internet y verifica que tus cambios se han guardado correctamente.';
        }
        $(that).after('<div id="errorbtn_' + idBtn + '" class="ko" style="display:block">' + mensajeError + '</div>');
    }).always(function () {
        OcultarUpdateProgress();
    });
}