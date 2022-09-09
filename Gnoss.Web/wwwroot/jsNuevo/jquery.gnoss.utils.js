/*!
 * jQuery JavaScript Library v1.6.1
 * http://jquery.com/
 *
 * Copyright 2011, John Resig
 * Dual licensed under the MIT or GPL Version 2 licenses.
 * http://jquery.org/license
 *
 * Includes Sizzle.js
 * http://sizzlejs.com/
 * Copyright 2011, The Dojo Foundation
 * Released under the MIT, BSD, and GPL Licenses.
 *
 * Date: 20.02.2014.11.30 
 */

// Permitir envío de Cookies a otro dominio
$.ajaxSetup({
    crossDomain: true,
    xhrFields: {
        withCredentials: true
    }
});


/**
 * Operativa para detectar comportamiento de navegación cuando se pulsa en "Back Button" del navegador
 * En ciertas páginas, se podían producir errores debido a que podían sobreescribir datos previos insertados: Ej: Creación de un recurso desde ckEditor
 * Esta operativa detecta este tipo de navegación y realiza una carga de la web
 */
 const operativaDetectarNavegacionBackButton = {
    init: function() {
        this.iniciarComportamiento();
    },

    /**
     * Comprobar si se ha pulsado en "back" del navegador. Si es así, y se encuentran elementos relativos a edición de recurso, 
     * obligar a recargar la página para obtener los datos siempre actualizados y evitar posible pérdida al sobreescribirlos
     */
    iniciarComportamiento: function(){
        
        window.onpageshow = function (event) {
            if (event.persisted) {
                // Se ha pulsado en "back" del navegador. Comprobar si es necesario recargar la página
                // ckeditor, radioButtons, checkbox
                 if ($(".cke, input[type='checkbox'],input[type='radio']").length > 1){
                    MostrarUpdateProgress();
                    // Desactivar el plugin dirty para que no muestr el mensaje
                    // Prevenir actualización de páginas cuando haya formularios "importantes". Avisar al usuario
                    if ($("#preventLeavingFormWithoutSaving").dirty != null) {
                        $("#preventLeavingFormWithoutSaving").dirty("setAsClean");
                    }                    
                    window.location.reload();
                 }                 
            }
        };
    },
};

/**
 * Clase jquery para poder hacer búsquedas desde un input para ocultar o mostrar una jerarquía de categorías
 * Ejemplo: Usado en el buscador de categorías de la página "Index"
 *
 * */
const operativaFiltroRapido = {
    /**
     * Acción para inicializar elementos y eventos
     */
    init: function () {
        this.config();
        this.configEvents();
    },
    /**
     * Configuración de los items html
     * */
    config: function () {
        this.inputClass = '.filtroRapido';
        this.input = $(this.inputClass);
        // Esperar 1 segundos a si el usuario ha dejado de escribir para iniciar búsqueda
        this.timeWaitingForUserToType = 500;
    },

    /**
     * Configuración de los eventos de los items html
     * */
     configEvents: function () {
        const that = this;
        that.isSearching = false;

        // Input donde se realizará la búsqueda
        this.input.on("keyup", function (event) {
            clearTimeout(that.timer);
            that.timer = setTimeout(function () {                
                // Obtener el id de la lista para saber qué lista filtrar
                const listaSelCatArbolId = that.input.data("filterlistid");
                if (!that.isSearching) {
                    that.isSearching = true;
                    MVCFiltrarListaSelCatArbol(that.input, listaSelCatArbolId, function () {
                        // Completion cuando finalice la búsqueda
                        that.isSearching = false
                    });
                }
                
            }, that.timeWaitingForUserToType);
        });
    },
};

/* Administración de Comunidad */
/**
 * Clase jquery para poder gestionar la administración de una comunidad (Vistas UsuariosOrganización)
 * - Comunidades, Grupos, Usuarios y modales de acciones
 * 
 * */
const operativaUsuariosOrganizacion = {
    /**
     * Acción para inicializar elementos y eventos
     */
    init: function () {
        this.configEvents();
    },

    /*
     * Eventos o clicks en elementos
     * */
    configEvents: function () {
        const that = this;

        // Obtener el id del botón (community) que ha abierto el modal modal container ya que se cargará ahí la gestión de "Añadir usuarios a la comunidad"        
        $(document).on('show.bs.modal', '#modal-container', function (e) {
            // Key obtenido del botón que ha abierto el modal
            if ($(e.relatedTarget).data('communitykey') == undefined) {
                that.keyItem = $(e.relatedTarget).data('keyitem');
            } else {
                that.communityKey = $(e.relatedTarget).data('communitykey');
            }
        });

        // Obtener el id del botón que ha abierto el modal (Necesario para posibles acciones)        
        $(document).on('show.bs.modal', '.getExternalIdFromModal', function (e) {
            // Key obtenido del botón que ha abierto el modal
            that.keyItem = $(e.relatedTarget).data('keyitem');
        });

        // Botón Aceptar del modal Abandonar la comunidad
        $(document).on('click', '.btnAceptarAbandonarCommunity', function () {
            // Id de la comunidad de la que se desea abandonar            
            const id = $(this).data("communitykey");
            // Item de la lista de la comunidad mostrada
            const communityListItem = $(`#${id}`);

            MostrarUpdateProgress();
            GnossPeticionAjax(
                $('#urlFilter').val() + '/leave-community/' + id,
                null,
                true
            ).done(function (data) {
                // Abandonado correcto la comunidad
                // 1 - Eliminar el panel del listado de comunidades
                communityListItem.remove();
                // 2 - Cerrar modal
                $(`#modalAbandonarCommunity_${id}`).modal('hide');
                // 3 - Mostrar mensaje OK
                setTimeout(() => {
                    mostrarNotificacion('success', data);
                }, 1500)

            }).fail(function (data) {
                // 1 - Mostrar error
                mostrarNotificacion('error', "No se ha podido abandonar la comunidad, algun miembro de la organización es administrador de la comunidad.");
            }).always(function () {
                OcultarUpdateProgress();
            });
        });

        // Botón Aceptar del modal "Aceptar Activación automático"
        $(document).on('click', '.btnAceptarActivarRegAuto', function () {
            // Id de la comunidad para activación registro automático
            const id = $(this).data("communitykey");
            // Item de la lista de la comunidad mostrada
            const communityListItem = $(`#${id}`);

            MostrarUpdateProgress();
            GnossPeticionAjax(
                $('#urlFilter').val() + '/activar-regauto/' + id,
                null,
                true
            ).done(function (data) {
                // 1 - Eliminar el panel del listado de comunidades
                communityListItem.remove();
                // 2 - Cerrar modal
                $(`#modalActivarRegAuto_${id}`).modal('hide');
                // 3 - Mostrar mensaje OK
                setTimeout(() => {
                    mostrarNotificacion('success', "Registro automático configurado correctamente.");
                }, 1500)
                // 4 - Recargar la página actual
                location.reload();
            }).fail(function (data) {
                // 1 - Mostrar error
                mostrarNotificacion('error', "No se ha podido activar el registro automatico.");
            }).always(function () {
                OcultarUpdateProgress();
            });
        });

        // Botón Aceptar del modal "Desactivar registro automático"
        $(document).on('click', '.btnAceptarDesactivarRegAuto', function () {
            // Id de la comunidad para activación registro automático
            const id = $(this).data("communitykey");
            // Item de la lista de la comunidad mostrada
            const communityListItem = $(`#${id}`);

            MostrarUpdateProgress();
            GnossPeticionAjax(
                $('#urlFilter').val() + '/desactivar-regauto/' + id,
                null,
                true
            ).done(function (data) {
                // 1 - Eliminar el panel del listado de comunidades
                communityListItem.remove();
                // 2 - Cerrar modal
                $(`#modalActivarRegAuto_${id}`).modal('hide');
                // 3 - Mostrar mensaje OK
                setTimeout(() => {
                    mostrarNotificacion('success', "Se ha desactivado correctamente el registro automático.");
                }, 1500)
                // 4 - Recargar la página actual
                location.reload();
            }).fail(function (data) {
                // 1 - Mostrar error
                mostrarNotificacion('error', "No se ha podido desactivar el registro automatico.");
            }).always(function () {
                OcultarUpdateProgress();
            });
        });

        // Botón Aceptar del modal "Agregar usuarios a Comunidad"
        $(document).on('click', '.btnAceptarAccionesProyOrg', function () {
            // Cada uno de los items que aparecen (usuarios/proyectos)
            const proyectos = $("#tblAccionesUsuOrgProyOrg").find(".proyecto");
            // Objeto para mandar la petición
            let datapost = {};
            // Contador de items
            let cont = 0;
            // Prefijos y métodos dependiendo si es página de Usuarios/Comunidades
            let metodo = "";
            let prefijo = "";
            let idItem = "";

            // Esta acción se realiza tanto para Usuarios como para Proyectos -> Tenerlo en cuenta para construir la URL 
            if (that.communityKey == undefined) {
                // Se está realizando acción de usuarios
                metodo = "asign-community";
                prefijo = "ProyectosAsignados";
                idItem = that.keyItem;
            } else {
                // Se está realizando acción de proyectos/comunidades                
                metodo = "asign-users";
                prefijo = "UsuariosAsignados";
                idItem = that.communityKey;
            }

            // Revisar cada uno de los proyectos
            proyectos.each(function () {
                // Construir el prefijo                
                var mPrefijo = `${prefijo}[${cont}]`;
                // Comprobar si el nombre está checkeado
                var checked = $('.chkProyecto', $(this)).is(':checked');
                // Obtener el id del proyecto / item
                var id = $(this).data("proyecto");
                //$(this).attr('id').replace('proyParticipa_', '');
                var tipo = 1;
                // Comprobar si el nombre/item está activado
                if (checked) {
                    var inputTipoSeleccionado = $('input[type=radio][name=tipo_' + id + ']:checked', $(this))
                    if (inputTipoSeleccionado.length > 0) {
                        tipo = $('input[type=radio][name=tipo_' + id + ']:checked', $(this)).val();
                    }
                }
                // Construcción del objeto POST
                datapost[mPrefijo + '.Key'] = id;
                datapost[mPrefijo + '.Participa'] = checked;
                datapost[mPrefijo + '.TipoParticipacion'] = tipo;

                cont++;
            });

            MostrarUpdateProgress();
            // Hacer petición al servidor con los datos 
            GnossPeticionAjax(
                $('#urlFilter').val() + `/${metodo}/` + idItem,
                datapost,
                true
            ).done(function (data) {
                // 1 - Cerrar el modal container
                $(`#modal-container`).modal('hide');
                // 2- Mostrar mensaje OK
                setTimeout(() => {
                    mostrarNotificacion('success', "Acción realizada correctamente.");
                }, 1500)
            }).fail(function (data) {
                // 1 - Mostrar error
                mostrarNotificacion('error', "Se ha producido un error. Por favor, inténtalo de nuevo más tarde.");
            }).always(function () {
                OcultarUpdateProgress();
            });
        });

        // Botón Aceptar del modal "Cambiar rol de usuario"
        $(document).on('click', '.btnAceptarCambiarRolUser', function () {

            var inputSeleccionado = $('input[name=SelectorRolUsuario_' + that.keyItem + ']:checked');
            var seleccion = inputSeleccionado.attr('rel');

            var datapost = {
                rol: seleccion
            }

            MostrarUpdateProgress();
            GnossPeticionAjax(
                $('#urlFilter').val() + '/change-rol/' + that.keyItem,
                datapost,
                true
            ).done(function (data) {
                // 1 - Cerrar el modal container
                $(`.getExternalIdFromModal`).modal('hide');
                // 2- Mostrar mensaje OK
                setTimeout(() => {
                    mostrarNotificacion('success', "Cambio de rol realizado correctamente.");
                }, 1500)

                // Input modificado -> Actualizarlo 
                const tipoUsuario = $('.tipoUsuario', $('#' + that.keyItem));
                tipoUsuario.text(inputSeleccionado.attr('aux'));
                tipoUsuario.attr('rel', seleccion);
            }).fail(function (data) {
                // 1 - Mostrar error
                mostrarNotificacion('error', "Se ha producido un error al cambiar el rol del usuario. Por favor, inténtalo de nuevo más tarde.");
            }).always(function () {
                OcultarUpdateProgress();
            });
        });

        // Botón Aceptar del modal "Expulsar a usuario de la comunidad"
        $(document).on('click', '.btnAceptarExpulsarUser', function () {
            MostrarUpdateProgress();
            GnossPeticionAjax(
                $('#urlFilter').val() + '/delete/' + that.keyItem,
                null,
                true
            ).done(function (data) {
                // 1 - Cerrar el modal container
                $(`.getExternalIdFromModal`).modal('hide');
                // 2- Mostrar mensaje OK
                setTimeout(() => {
                    mostrarNotificacion('success', "El usuario ha sido eliminado de la organización.");
                }, 1500)
                // 3 - Eliminar la fila del usuario
                $(`#${that.keyItem}`).remove();

            }).fail(function (data) {
                // 1 - Mostrar error
                mostrarNotificacion('error', "Se ha producido un error al tratar de expulsar al usuario de la comunidad. Por favor, inténtalo de nuevo más tarde.");
            }).always(function () {
                OcultarUpdateProgress();
            });
        });

        // Buscador de elementos al escribir en inputFiltro
        $(document).on('keyup', '.buscador-coleccion input.inputFiltro', function () {
            that.filtrarProyectosUsu();
        });

        // Botón de lupa para inicar búsqueda de miembros, grupos, comunidades
        $(document).on('click', '#inputLupaMiembros', function (event) {
            that.filtrar();
        });

        // Input de búsqueda de miembros, grupos, comunidades (Pulsación ENTER)
        $(document).on('keypress', '#txtFiltrarMiembros', function (event) {
            var keycode = (event.keyCode ? event.keyCode : event.which);
            if (keycode == '13') {
                that.filtrar();
                event.preventDefault();
                return false;
            }
        });

        // Checkbox para mostrar o no aquellos que participan en la comunidad
        $(document).on('change', '#mostrarParticipa', function () {
            that.filtrarProyectosUsu();
        });

        // Botones paginador de resultados
        $(document).on('click', '#NavegadorPaginas_Pag a', function () {
            var numPag = $(this).attr('aux');
            $('#numPagina').val(numPag);
            that.filtrar();
        });
    },

    /**
     * Método para filtrar usuarios del proyecto. Mostrará u ocultará los elementos que coincidan con la cadena buscada
     * */
    filtrarProyectosUsu: function () {
        const inputFiltro = $('.buscador-coleccion input.inputFiltro');
        const textoFiltro = inputFiltro.val().toLowerCase().replace(/á/g, 'a').replace(/é/g, 'e').replace(/í/g, 'i').replace(/ó/g, 'o').replace(/ú/g, 'u');
        let filtrarPorTexto = false;

        if (textoFiltro.length > 0) {
            filtrarPorTexto = true;
        }

        // Mostrar o no según el valor del checkbox de "Mostrar todos los que participan en la comunidad
        const mostrarTodos = !$('#mostrarParticipa').is(':checked');

        // Cada uno de los items (proyectos/usuarios)
        const proyectos = $("#tblAccionesUsuOrgProyOrg").find(".proyecto");;

        proyectos.each(function () {
            //var textoProy = $('span.nombreProy', this).text().trim().toLowerCase().replace(/á/g, 'a').replace(/é/g, 'e').replace(/í/g, 'i').replace(/ó/g, 'o').replace(/ú/g, 'u');
            // Texto del proyecto
            const textoProy = $('.custom-control-label', this).text().trim().toLowerCase().replace(/á/g, 'a').replace(/é/g, 'e').replace(/í/g, 'i').replace(/ó/g, 'o').replace(/ú/g, 'u');
            const mostrarPorFiltro = textoProy.indexOf(textoFiltro) >= 0;
            const checked = $('.chkProyecto', $(this)).is(':checked');
            if ((mostrarTodos || checked) && (!filtrarPorTexto || mostrarPorFiltro)) {
                $(this).show();
            }
            else {
                $(this).hide();
            }
        });
    },

    /**
     * Método para filtrar resultados al pulsar en paginador. Cargará nuevos resultados en la sección de "resultados".
     * */
    filtrar: function () {
        // Sección donde se mostrarán los resultados
        const $panelResults = $("#contentUsuariosOrganizacion");

        // Construcción del objeto para realizar la petición
        const dataPost = {
            Search: $('#txtFiltrarMiembros').val(),
            NumPage: $('#numPagina').val(),
            Type: -1 /*($('#tipoFiltro').length > 0 ? $('#tipoFiltro').val() : -1)*/,
            Order: "ASC"/*$('#ordenFiltros').val()*/
        }

        MostrarUpdateProgress();

        // Realizar la petición para obtención de resultados
        GnossPeticionAjax(
            $('#urlFilter').val() + '/filter',
            dataPost,
            true
        ).done(function (data) {
            // Pintar los resultados en la sección concreta
            $panelResults.html(data);
        }).fail(function (data) {
        }).always(function () {
            OcultarUpdateProgress();
        });
    },
};

/* Fin Administración de Comunidades */

function CompletadaCargaActividadReciente() {
    if (typeof (window.CompletadaCargaActividadRecienteComunidad) == 'function') {
        CompletadaCargaActividadRecienteComunidad();
    }
}
function CompletadaCargaBiografia() {
    if (typeof (window.CompletadaCargaBiografiaComunidad) == 'function') {
        CompletadaCargaBiografiaComunidad();
    }
}
function CompletadaCargaRecursos() {
    modoVisualizacionListados.init();
    if (typeof (window.CompletadaCargaRecursosComunidad) == 'function') {
        CompletadaCargaRecursosComunidad();
    }
}
function CompletadaCargaContextos() {
    engancharClicks();
	comportamientoRecursosVinculados.init();
	if(typeof (window.CompletadaCargaContextosComunidad) == 'function') {
		CompletadaCargaContextosComunidad();
	}		
}
function CompletadaCargaFacetas(){
	comportamientoCargaFacetas.init();
	if(typeof (window.comportamientoCargaFacetasComunidad) == 'function') {
		comportamientoCargaFacetasComunidad();
	}		
}
function CompletadaCargaUsuariosVotanRecurso(){
	comportamientoCargaUsuariosVotanRecurso.init();
	if(typeof (window.comportamientoCargaUsuariosVotanRecursoComunidad) == 'function') {
		comportamientoCargaUsuariosVotanRecursoComunidad();
	}		
}
function completadaCargaAcciones(){
    herramientasRecursoCompactado.init();        
    if (body.hasClass('palco') && body.hasClass('activo')) {
        abreEnVentanaNueva.montarHerramientas();
        abreEnVentanaNueva.montarVotos();
        abreEnVentanaNueva.montarVolverFicha();        
    }
	if(typeof (window.completadaCargaAccionesComunidad) == 'function') {
		completadaCargaAccionesComunidad();
	}	
	return;
}
function CompletadaCargaAccionesListado() {
    if (typeof (window.CompletadaCargaAccionesListadoComunidad) == 'function') {
        CompletadaCargaAccionesListadoComunidad();
    }
}
var comportamientoCargaUsuariosVotanRecurso = {
	init: function(){
		this.config();
		this.engancharEnlaceMasResultados();
		if(this.listado.find('.votoNegativo').size() <= 0) return;
		this.tabsVotosPositivosNegativos();
		return;
	},
	config: function(){
		this.body = body;
		this.panelAmpliado = this.body.find('#panelVotosAmpliado');
		this.listado = this.panelAmpliado.find('.resource-list');
		this.enlace = this.panelAmpliado.find('.masUsuriosVotosRecursos a');
		return;
	},
	tabsVotosPositivosNegativos: function(){
		var tabs = this.panelAmpliado.find('.tabsVotosPositivosNegativos');
		if(tabs.size() > 0) return;
		this.tabs = $('<div>').addClass('tabsVotosPositivosNegativos tabspresentation acciones');
		var ulTabs = $('<ul>');
		var liPositivosTabs = $('<li>').addClass('mostrarPositivos');
		var liNegativosTabs = $('<li>').addClass('mostrarNegativos');
		var liTodosTabs = $('<li>').addClass('mostrarTodos active');
		var aPositivosTabs = $('<a>').attr('href', '#').text('votos positivos');
		var aNegativosTabs = $('<a>').attr('href', '#').text('votos negativos');
		var aTodosTabs = $('<a>').attr('href', '#').text('todos');
		liPositivosTabs.append(aPositivosTabs);
		liNegativosTabs.append(aNegativosTabs);
		liTodosTabs.append(aTodosTabs);
		ulTabs.append(liTodosTabs);
		ulTabs.append(liPositivosTabs);
		ulTabs.append(liNegativosTabs);
		this.tabs.append(ulTabs);
		this.listado.before(this.tabs);
		this.engancharTipoVoto();
		return;
	},
	desmarcarTabs: function(){
		this.tabs.find('li').removeClass('active');
		return;
	},
	engancharTipoVoto: function(){
		var that = this;
		this.tabs.find('a').bind('click', function(evento){
			evento.preventDefault();
			var enlace = $(evento.target);
			var li = enlace.parent();
			that.desmarcarTabs();
			that.listado.removeClass('mostrarPositivos mostrarNegativos mostrarTodos');
			that.listado.addClass(li.attr('class'));
			li.addClass('active');
		})
		return;
	},
	engancharEnlaceMasResultados: function(){
		var that = this;
		this.enlace.bind('click', function(evento){
			evento.preventDefault();
			//var enlace = $(this);
			//var url = enlace.attr('href');
			/*setTimeout(function(){
				that.traerUsuariosVotosRecursos(url);
			}, 800);*/
			//WebForm_DoCallback('ctl00$ctl00$CPH1$CPHContenido$controles_ficharecurso_ascx', 'VerVotantes', ReceiveServerData, '', null, false);
			enlace.parent().remove();
		})
		return;
	},
	traerUsuariosVotosRecursos: function(url){
		var that = this;
		//var url = url;
		//var respuesta = $.ajax({
		//  url: url,
		//  type: "GET",
		//  dataType: "html"
		//})

	    GnossPeticionAjax(url, null, true)
		.done(function(pagina) {
			var html = pagina;
			that.panelAmpliado.find('.resource-list').first().append(html);	
			var scrollTop = that.listado.scrollTop() + 710;
			that.listado.scrollTop(scrollTop);
			CompletadaCargaUsuariosVotanRecurso();
			return;
		})
		.fail(function( jqXHR, textStatus ) {
		  alert( "Request failed: " + textStatus );
		})
		return;		
	}
}
var comportamientoCargaFacetas = {
    init: function () {
        // Longitud facetas por CSS
		// limiteLongitudFacetas.init();
		facetedSearch.init();
		$('.verMasFaceta').each(function () {
		    var enlace = $(this);
		    var params = enlace.attr('rel').split('|');
		    var faceta = params[0];
		    var controlID = params[1];
		    enlace.unbind("click").click(function (evento) {
		        evento.preventDefault();
		        VerFaceta(faceta, controlID);
		    });
		});
		return;
	}
}
var comportamientoRecursosVinculados = {
	init: function(){
		this.config();
		var groupsToSemanticView = this.columnaRelacionados.find('.moveToSemanticView')
        /* No existe size if(groupsToSemanticView.size() > 0) this.moveToSemanticView(groupsToSemanticView);	*/
        if (groupsToSemanticView.length > 0) this.moveToSemanticView(groupsToSemanticView);
		return;
	},
	config: function(){
		this.content = $('#content');
		this.columnaRelacionados = this.content.find('#col01');
		this.semanticView = this.content.find('.semanticView');
		return;
	},
	moveToSemanticView: function(groupsToSemanticView){
		var that = this;
		groupsToSemanticView.each(function(){
			var grupo = this;
			that.semanticView.append(grupo);
		});
		return;
	}
}
var limpiarGruposVaciosSemanticView = {
	init: function(){
		$('.semanticView .group').each(function(indice){
			var group = $(this);
			var contentGroup = group.find('.contentGroup');
			if(contentGroup.html() == '') group.remove();
		})
	}
}
var controladorLineas = {
	caracteres: 140,
	init: function(){
		this.config();
	},
	config: function(){
		var that = this;
		this.items = $('#col02 .semanticView .limitGroup');
		this.items.each(function(indice){
			var item = $(this);
			var contentGroup = item.find('.contentGroup');
			var texto = contentGroup.text();
			var arrayTexto = texto.split(' ');
			if(arrayTexto.length <= that.caracteres) return;			
			contentGroup.addClass('activado').hide()
			var css = item.attr('class');
			var cssArray = css.split(' ');
			var cssLimite = cssArray[cssArray.length - 1];
			if(cssLimite.indexOf('limit_') >= 0){
				that.carateres = cssLimite.substring(6, cssLimite.length);
			};

			var recorte = arrayTexto.slice(0, that.caracteres);
			var textoRecortado = '<p>';
			for(var contador = 0; contador < that.caracteres; contador++){
				textoRecortado += recorte[contador] + ' ';
			};
			textoRecortado += '</p>';
			var enlace = $('<a>').attr('class','leermas').attr('href','#').text('+ leer más').attr('title','leer resto de la entrada');
			var plegar = $('<a>').attr('class','plegar').attr('href','#').text('- plegar').attr('title','plegar contendio de la entrada');
			var divTextoRecortado = $('<div>').attr('class','textoRecortado');
			divTextoRecortado.append(textoRecortado);
			contentGroup.before(divTextoRecortado);
			contentGroup.append(plegar);
			divTextoRecortado.append(enlace);
			enlace.bind('click', function(evento){
				evento.preventDefault();
				that.comportamiento(evento);
			});
			plegar.bind('click', function(evento){
				evento.preventDefault();
				that.plegarEntrada(evento);
			});			
		});
		return;
	},
	comportamiento: function(evento){
		var enlace = $(evento.target);
		var group = enlace.parents('.group').first();
		var recorte = group.find('.textoRecortado');
		var original = group.find('.contentGroup');
		recorte.hide();
		original.show();
		return;
	},		
	plegarEntrada: function(evento){
		var enlace = $(evento.target);
		var group = enlace.parents('.group').first();
		var recorte = group.find('.textoRecortado');
		var original = group.find('.contentGroup');
		original.hide();
		recorte.show();
		return;
	}	
}
var modoVisualizacionListadosHomeCatalogo = {
	init: function(){
		this.body = body;
		this.grupos = this.body.find('#col02 .listadoRecursos');
		if(this.grupos.size() <= 0) return;
		this.config();
		this.preloader();
		return;
	},
	config: function(){
		return;
	},	
	preloader: function(){
		var that = this;
		var group = group;
		this.grupos.each(function(indice){
			var grupo = $(this);
			if(grupo.hasClass('listView')) return;
			that.calcular(grupo);
			var imagenes = grupo.find('.miniatura img');
			imagenes.each(function(indice){
				var imagen = $(this);
				imagen.load(function(evento){
					var imagen = $(evento.target);
					var grupo = imagen.parents('.listadoRecursos').first();
					that.calcular(grupo);
				})
			})
			
		})
		return;
	},	
	calcular: function(grupo){
		var that = this;
		var grupo = grupo;
		var contador = 0;
		var masAlto = 0;
		var resources = grupo.find('.resource');
		var alturas = [];
		resources.each(function(){
			var recurso = $(this);
			recurso.attr('style', '');
			//var categorias = recurso.find('.categorias')
			//var etiquetas = recurso.find('.etiquetas')
			//limitarNumeroItems.init(categorias);
			//limitarNumeroItems.init(etiquetas);
			var altura = recurso.height();
			if(altura > masAlto) masAlto = altura;
			contador ++;
			if(contador == 3){
				recurso.addClass('omega');
				alturas.push(masAlto);
				contador = 0;
				masAlto = 0;
			}
		});
		resources.each(function(indice){
			var recurso = $(this);
			var fila = parseInt(indice/3);
			recurso.css('height', alturas[fila] + 'px');
		});		
		this.isCalculadoOmega = true;
		return;
	}
};
var modoVisualizacionListados = {
    id: '#view',
    cssListView: 'listView',
    cssGridView: 'gridView',
    cssActiveView: 'activeView',
    cssResourceList: '.resource-list',
    isAlturaCalculada: false,
    isCalculadoOmega: false,
    isGridViewDefault: false,
    isLanzadaSeguridad: false,
    init: function () {
        body = $('body');
        this.body = body;
        var group = this.body.find('#col02 .listadoRecursos');
        this.config();
        this.setView();
        if (this.isGridViewDefault) {
            this.configGridView();
            this.preloader(group);
        } else {
            this.configListView();
        }        
        this.enganchar();
        return;
        //if(!this.isLanzadaSeguridad)this.seguridadImagenesRotas();
        //this.isLanzadaSeguridad = true;
    },
    config: function () {
        this.list = $(this.cssResourceList);
        this.view = $(this.id);
        this.listView = $('.' + this.cssListView, this.view);
        this.gridView = $('.' + this.cssGridView, this.view);
        return;
    },
    configGridView: function () {
        this.showGridView();
    },
    configListView: function () {
        this.showListView();
    },
    showGridView: function () {
        this.list.removeClass(this.cssListView);
        //this.list.addClass(this.cssGridView);
        this.list.addClass("mosaicView");
    },
    showListView: function () {
        //this.list.removeClass(this.cssGridView);
        this.list.removeClass("mosaicView");
        this.list.addClass(this.cssListView);
    },
    preloader: function (group) {
        var that = this;
        var group = group;
        //group.css('visibility','hidden');
        group.addClass('gridPreview');
        var imagenes = $('.miniatura img', group);
        var numeroImagenes = imagenes.length;

        for (var i=1;i<=5;i++)
        {
            setTimeout(function () {
                that.calcular(group);
            }, i*1000);
        }
        
        if (numeroImagenes <= 0) {
            this.calcular(group);
            //group.css('visibility','visible');
            group.removeClass('gridPreview');
        } else {
            var contador = 0;
            //sthis.view.hide();
            imagenes.load(function () {
                var imagen = $(this);
                contador++;
                if (contador > numeroImagenes - 1) {
                    that.calcular(group);
                    group.removeClass('gridPreview');
                    //group.css('visibility','visible');
                    that.view.show();
                }
            });
        };
        
        return;
    },
    seguridadImagenesRotas: function () {
        var that = this;
        setTimeout(function () {
            $('#section .listadoRecursos').each(function () {
                var group = $(this);
                var atributo = group.attr('style');
                var isGroupVisible = atributo.indexOf('hidden') <= 0;
                if (!isGroupVisible) {
                    that.calcular(group);
                    group.removeClass('gridPreview');
                    //group.css('visibility','visible');
                    that.view.show();
                }
            })
        }, 5000);
    },
    calcular: function (group) {
        var that = this;
        var contador = 0;
        var masAlto = 0;
        var resources = $('.resource', group);
        var alturas = [];
        resources.each(function () {
            var recurso = $(this);
            recurso.removeAttr('style');  
            var categorias = recurso.find('.categorias')
            var etiquetas = recurso.find('.etiquetas')
            limitarNumeroItems.init(categorias);
            limitarNumeroItems.init(etiquetas);
            var altura = recurso.height();
            if (altura > masAlto) masAlto = altura;
            contador++;
            if (contador == 3) {
                recurso.addClass('omega');
                alturas.push(masAlto);
                contador = 0;
                masAlto = 0;
            }
        });
        resources.each(function (indice) {
            var recurso = $(this);
            var fila = parseInt(indice / 3);
            recurso.css('height', alturas[fila] + 'px');
        });
        this.isCalculadoOmega = true;
        return;
    },
    setView: function () {
        //this.gridView.hasClass(this.cssActiveView) ? this.isGridViewDefault = true : this.isGridViewDefault = false;
        $(".activeView").hasClass("aMosaicView") ? this.isGridViewDefault = true : this.isGridViewDefault = false;
        return;
    },
    enganchar: function () {
        var that = this;
        $('a', this.listView).unbind();
        $('a', this.listView).bind('click', function (evento) {
            evento.preventDefault();
            that.showListView();
            that.listView.addClass(that.cssActiveView);
            that.gridView.removeClass(that.cssActiveView);
        });

        $('a', this.gridView).unbind();
        $('a', this.gridView).bind('click', function (evento) {
            evento.preventDefault();
            that.showGridView();
            that.gridView.addClass(that.cssActiveView);
            that.listView.removeClass(that.cssActiveView);
            that.calcular();
        });
        return;
    }
};
var seccion = {
	id: '#nav',
	secciones: ['home','indice','catalogo','recurso','debate', 'dafo', 'pregunta','encuesta', 'persona'],
	seccionActiva: 0,
	init: function(){
		this.config();
		this.buscarSeccion();
		this.desmarcar();
		this.marcar();
	},
	config: function(){
		this.nav = $(this.id);
		this.li = $('li', this.nav);
		return;
	},
	buscarSeccion: function(){
		var items = this.secciones.length;
		var url = window.location.href;
		for(var contador = 0; items > contador; contador ++ ){
			var nombreSeccion = this.secciones[contador];
			if(url.indexOf(nombreSeccion) >= 0) this.seccionActiva = contador;
		};
		return;
	},
	desmarcar: function(){
		this.li.each(function(){
			$(this).removeClass('activo');
		})
		return;
	},
	marcar: function(){
		var activo = $(this.li[this.seccionActiva]);
		activo.addClass('activo');
		return;
	}	
};
function desmarcarOpcionesGrupo(lis){
	lis.each(function(){
		var link = $('a', this);
		if(!link.hasClass('noGroup')){
			$(this).removeClass('active');
		};
	});
	return;
}
function ocultarPaneles(panels){
	var panels = panels.split(' ');
	var contador = panels.length;
	for(var i = 0; contador > i; i++ ){
		$('#' + panels[i]).hide();
	};				
}
var carrusel = {
	id: '#presentation',
	preloadImages: true,
	isImagesLoaded: false,
	numImagenActiva: 0,
	isPause: false,
	vueltas: 0,
	init: function(){
		this.config();
		if(!this.preloadImages) return;
		this.loader();
	},
	config: function(){
		this.carrusel = $(this.id);
		if(this.carrusel.hasClass('nopreload')) this.preloadImages = false;
		if(!this.preloadImages) return;
		this.view = $('.galeriaPresentacion', this.carrusel);
		this.items = $('.carrusel li', this.carrusel);
		this.imagenes = $('.carrusel li img', this.carrusel);
		// Deprecado size() 
        //this.numeroImagenes = this.imagenes.size();
        this.numeroImagenes = this.imagenes.length;
		return;
	},
	loader: function(){
		var that = this;
		var contador = 0;
		this.imagenes.each(function(indice){
			var imagen = $(this);
			imagen.parent().attr('rel', indice);
			var li = that.items[indice];
			if(indice > 0) $(li).hide();
			this.onload = function(){
				contador ++;
				var imagen = $(this);
				if(indice == that.numImagenActiva){
					var li = that.items[indice];
					$(li).fadeIn('slow');
				}
				imagen.addClass('loaded');
				if(contador == that.numeroImagenes){
					that.engancharEfecto();
					that.crearPasador();
				}
			};
		});
		this.seguridad();
		return;
	},
	crearPasador: function(){
		var itemsCarrusel = '';
		var html = '';
		this.items.each(function(indice){
			var muestra = indice + 1;
			itemsCarrusel += '<li><a href=\"#\" rel=\"' + indice + '\" >' + muestra + '<\/a><\/li>';
		});
		html += '<div id=\"pasadorCarrusel\">';
		html += '<ul>';
		html += itemsCarrusel;
		html += '<\/ul>';
		html += '<\/div>';
		this.view.append(html);
		this.pasador = $('#pasadorCarrusel', this.carrusel);
		this.enlacesPasador = $('a', this.pasador);
		this.marcarItemPasadorActivo(0);
		this.engancharPasador();
		return;
	},	
	desmarcarItemsPasadorActivo: function(){
		this.enlacesPasador.each(function(){
			$(this).removeClass('activo');
		});
		return;
	},	
	marcarItemPasadorActivo: function(numero){
		var enlace = this.enlacesPasador[numero];
		$(enlace).addClass('activo');
		return;
	},
	engancharPasador: function(){
		var that = this;
		this.enlacesPasador.each(function(){
			var enlace = $(this);
			enlace.bind('click', function(evento){
				evento.preventDefault();
				var numero = enlace.attr('rel');
				enlace.addClass('activo');
				that.isPause =  true;
				that.contadorIntervalos = 0;
				that.efectoPasador(numero);
			});
		})
		return;
	},
	efectoPasador: function(numero){
		var siguiente = numero;
		var liIn = $(this.items[siguiente]);
		var liOut = $(this.items[this.numImagenActiva]);
		this.desmarcarItemsPasadorActivo();	
		this.marcarItemPasadorActivo(siguiente);		
		liOut.fadeOut('fast', function(){
			liIn.hide().fadeIn('slow');			
		});		
		this.numImagenActiva = siguiente;	
		clearInterval(this.intervalo);
		return;
	},	
	efecto: function(){
		var siguiente = this.numImagenActiva + 1;
		if(siguiente >= this.numeroImagenes){
			siguiente = 0;
			this.vueltas ++;
			if(this.vueltas == 2) clearInterval(this.intervalo);
		}
		var liIn = $(this.items[siguiente]);
		var liOut = $(this.items[this.numImagenActiva]);
		this.desmarcarItemsPasadorActivo();
		this.marcarItemPasadorActivo(siguiente);
		liOut.fadeOut('fast', function(){
			liIn.hide().fadeIn('slow');			
		});	
		this.numImagenActiva = siguiente;	
		return;
	},
	seguridad: function(){
		var that = this;
		var imagen = this.imagenes[0];
		var isLoadedImage = false;
		imagen = $(imagen);
		setTimeout(function(){
			if(imagen.hasClass('loaded')) isLoadedImage = true;
			if(!isLoadedImage) {
				that.engancharEfecto();
				that.crearPasador();
			}
		}, 3000);			 
	},
	engancharEfecto: function(){
		var that = this;
		//this.engancharClick();
		this.contadorIntervalos = 0;
		this.intervalo = setInterval(function(){
			that.contadorIntervalos ++;
			if(that.contadorIntervalos >= 4){
				that.isPause = false;
				that.contadorIntervalos = 0;
			}
			that.efecto();
		}, 3000);
		return;
	}
}
var carruselLateralColumna  = {
	identificadorRecurso: '.resource',
	carruseles: ['.comiteCrea'],
	recursoVisible: 0,
	isPause: false,
	init: function(){
		var carrusel = this.carruseles[0];
		this.componente = $(carrusel);
		this.config();
		this.ocultar();
		this.mostrar(this.recursoVisible);
		this.crearPaginador();
		this.engancharPaginador();
		this.automatismo();
		return;
	},
	config: function(){
		this.recursos = $(this.identificadorRecurso, this.componente);
		return;
	},
	ocultar: function(){	
		this.recursos.each(function(){
			$(this).hide();
		})
		return;
	},
	mostrar: function(numero){
		var visible = this.recursos[numero];
		visible = $(visible);
		visible.show();
		return;
	},
	plantilla: function(){
		var html = '';
		html += '<div class=\"paginador\">';
		html += '<ul>';
		this.recursos.each(function(indice){
			if(indice == 0){
				html += '<li class=\"activo\"><a href=\"#\" rel=\"' + indice + '\">' + indice  + '<\/a><\/li>';
			}else{
				html += '<li><a href=\"#\" rel=\"' + indice + '\">' + indice  + '<\/a><\/li>';
			}
		});
		html += '<\/ul>';
		html += '<\/div>';
		return html;
	},
	crearPaginador: function(){
		this.componente.append(this.plantilla());
		this.paginador = $('.paginador', this.componente);
		this.itemsPaginador = $('a', this.paginador);
		return;
	},
	desmarcarPaginador: function(){
		this.itemsPaginador.each(function(){
			$(this).parent().removeClass('activo');
		});
		return;
	},
	automatismo: function(){
		var that = this;
		var contador = 0;
		setInterval(function(){
			contador++;
			if(contador >= 8){
				that.isPause = false;
				contador = 0;
			}
			if(that.isPause) return;
			that.recursoVisible ++;
            // Deprecado size()
            //if(that.recursoVisible >= that.itemsPaginador.size()) that.recursoVisible = 0;
            if (that.recursoVisible >= that.itemsPaginador.length) that.recursoVisible = 0;			
			that.siguiente();
			var activo = that.itemsPaginador[that.recursoVisible];
			activo = $(activo);
			activo.parent().addClass('activo');
		}, 4000);		
	},	
	siguiente: function(){
		this.ocultar();
		this.mostrar(this.recursoVisible);
		this.desmarcarPaginador();
		return;
	},
	engancharPaginador: function(){
		var that = this;
		this.itemsPaginador.each(function(contador){
			$(this).bind('click', function(evento){
				evento.preventDefault();
				that.isPause = true;
				var enlace = $(evento.target);
				var indice = enlace.attr('rel');
				that.recursoVisible = indice;
				that.siguiente();
				enlace.parent().addClass('activo');	
			});
		});
		return;
	}
}
var opcionesMenuIdentidad = {
	cssItemActivo: 'itemActivo',
	cssItemsOpcionesSegundoNivel: '.itemConOpcionesSegundoNivel',
	cssOpcionPrincipal: '.opcionPrincipal',
	idMenuSuperiorComunidades: '#menuSuperiorComunidades',
	idOtrasIdentidades: '#otrasIdentidades',
	idIdentidad: '#identidad',
	cssEnlaceOtrasIdentidades: '#otrasIdentidades .wrap a',
	cssListadoOtrasIdentidades: '.listadoOtrasIdentidades',
	init: function(){
        this.config();
        // Deprecado size()
        //if(this.menu.size() <= 0) return;
        if (this.menu.length <= 0) return;
		this.pantalla();
		this.enganchar();
		this.ocultarMenusInactividad();
	},
	config: function(){
		this.identidad = $(this.idIdentidad);
		this.menu = $(this.idMenuSuperiorComunidades);
		this.items = $(this.cssItemsOpcionesSegundoNivel, this.menu);
		this.enlaces = $(this.cssOpcionPrincipal + ' a', this.items);
		this.otrasIdentidades = $(this.cssOpcionPrincipal + ' a', this.items);
		this.enlaceOtrasIdentidades = $(this.cssEnlaceOtrasIdentidades, this.identidad);
		this.otrasIdentidades = $(this.cssListadoOtrasIdentidades, this.identidad);
		this.page = $('#page');
		return;
	},
	ocultarMenusInactividad: function(){
		var that = this;
		this.page.hover(
			function(){
				that.ocultar();
				that.ocultarOtrasIdentidades();
			},function(){return}
		);	
	},
	ocultar: function(){
		this.items.removeClass(this.cssItemActivo);
		return;
	},
	ocultarOtrasIdentidades: function(){
		this.enlaceOtrasIdentidades.parents('li').removeClass(this.cssItemActivo);
		this.otrasIdentidades.hide();
		return;
	},	
	pantalla: function(){
		var item = this.items[0];
		var principal = $(this.cssOpcionPrincipal, item);
		this.correccion = principal.offset().left;
	},
	comportamiento: function(evento){
		var enlace = $(evento.target);
		var item = enlace.parents(this.cssItemsOpcionesSegundoNivel);
		if(!item.hasClass(this.cssItemActivo)){
			this.ocultar();
			this.ocultarOtrasIdentidades();
			var principal = enlace.parent();
			principal = $(principal);
			var left = principal.offset().left - this.correccion;
			var opciones = $('.opcionesPanel', item);
			opciones.css('left', left + 'px');
			item.addClass(this.cssItemActivo);
		}else{
			item.removeClass(this.cssItemActivo);
		}
		return;
	},
	comportamientoOtrasIdentidades: function(evento){
		var enlace = $(evento.target);
		var item = enlace.parents('li');
		this.ocultar();
		if(!item.hasClass(this.cssItemActivo)){
			var left = item.offset().left - this.correccion;
			this.otrasIdentidades.css('left', left + 'px');
			this.otrasIdentidades.show();
			item.addClass(this.cssItemActivo);
		}else{
			this.ocultarOtrasIdentidades();
		}
		return;
	},	
	enganchar: function(){
		var that = this;
		this.enlaces.each(function(){
			$(this).bind('click', function(evento){
				evento.preventDefault();
				that.comportamiento(evento);
			})
		});
		this.enlaceOtrasIdentidades.bind('click', function(evento){
			evento.preventDefault();
			that.comportamientoOtrasIdentidades(evento);
		});
	}
};

var facetedSearch = {
	noCollapse: 'noCollapse',
	init: function(){
		//this.config();
		//this.comportamiento();
	},
	config: function(){
		this.facetas = $('#facetedSearch .box:not(.' + this.noCollapse + '):not(.categories)');
		return;
	},
	ocultar: function(){
		this.facetas.each(function(){
			var faceta = $(this);
			var searchBox = $('.facetedSearchBox', faceta);
			if(faceta.height() < 72) return;
			searchBox.hide();
			faceta.css({
				'height': '72px',
				'background': '#f0f0f0',
				'border-bottom': 'none'
			});			
		});
		return;
	},
	comportamiento: function(){
		var that = this;
		this.facetas.each(function(){
			var faceta = $(this);
			var searchBox = $('.facetedSearchBox', faceta);
			if(faceta.height() < 72) return;
			searchBox.hide();
			faceta.css({
				'height': '72px',
				'overflow': 'hidden'
			});
			faceta.hover(
				function(){
					that.ocultar();
					searchBox.show();
					var altura = faceta.height();
					faceta.css({
						'height': '100%',
						'background': '#eee'
					}, 1000);
				},
				function(){
					return;
					searchBox.hide();
					faceta.css({
						'height': '72px',
						'background': '#f0f0f0',
						'border-bottom': 'none'
					});
				})
		});		
	}
}; 
var limitarNumeroItems = {
	init: function(listado){
		this.listado = listado;
		this.listado.addClass('limitado');
		this.config();
		this.comportamiento();
		return;
	},
	config: function(){
		this.recurso = this.listado.parents('div.resource');
		this.enlace = this.recurso.find('.title a');
		return;
	},
	comportamiento: function(){
		var ul = this.listado.find('ul');
		var lis = this.listado.find('li');
		if(lis.size() <= 3) return;
		lis.each(function(indice){
			if(indice > 2) $(this).hide();
		});
		ul.append('<li><a class=\"verTodasCategoriasEtiquetas mas\" href="' + this.enlace.attr('href') + '" title=\"ver todos\">' + form.masMIN + '...<\/a><\/li>');
		return;
	}
};

var mostrarNumeroCategorias = {
    init: function () {
        this.comportamiento();
    },
    config: function () { return },
    comportamiento: function () {
        var categorias = $('#section .resource .categorias ul');
        categorias.each(function () {
            var ul = $(this);
            var verMas = ul.find('a.verTodasCategoriasEtiquetas');
            if (verMas.length == 0) {
                var lis = $('li', ul);
                if (lis.size() <= 3) return;
                lis.each(function (indice) {
                    if (indice > 2) $(this).hide();
                })
                ul.append('<li><a class=\"verTodasCategoriasEtiquetas mas\" href="#" title=\"ver todos\">' + form.masMIN + '...<\/a><\/li>');
            }
        });
        return;
    }
};

var mostrarNumeroEtiquetas = {
    init: function () {
        this.comportamiento();
    },
    config: function () { return },
    comportamiento: function () {
        $('#section  .resource .etiquetas ul').each(function () {
            var ul = $(this);
            var verMas = ul.find('a.verTodasCategoriasEtiquetas');
            if (verMas.length == 0) {
                var lis = $('li', ul);
                if (lis.size() <= 3) return;
                lis.each(function (indice) {
                    if (indice > 2) $(this).hide();
                })
                ul.append('<li><a class=\"verTodasCategoriasEtiquetas mas\" href="#" title=\"ver todos\">' + form.masMIN + '...<\/a><\/li>');
            }
        });
        return;
    }
};



/**
 * Calcular y recortar la longitud de las facetas que aparecen en el panel izquierdo de "B�squedas". 
 */
var limiteLongitudFacetas = {
    init: function () {
        this.comportamiento();
    },
    config: function () { return },
    comportamiento: function () {
        $('#facetedSearch .box li a .textoFaceta').each(function () {
            var digitos = 24;
            var hayNumero = false;
            var enlace = $(this);
            var textoEnlace = $.trim(enlace.text());
            var longitud = textoEnlace.length;
            var caracter = '';

            var p1 = longitud;

            if (textoEnlace.lastIndexOf('(') > 0) {
                p1 = textoEnlace.lastIndexOf('(');
            }

            if (enlace.parent().children('img').length > 0) {
                digitos--;
            }

            var margenLeft = enlace.css('margin-left');
            margenLeft = margenLeft.substring(0, margenLeft.length - 2);
            while (margenLeft >= 10) {
                margenLeft = margenLeft - 10;
                digitos--;
            }

            if (enlace.parents('ul').length > 1) {
                digitos = digitos - (enlace.parents('ul').length * 2);
            }

            hayNumero = (textoEnlace.charAt(textoEnlace.length - 1) == ')');
            if (hayNumero) {
                digitos = digitos - (longitud - p1);
            }

            var c1 = $.trim(textoEnlace.substring(0, p1));
            if (c1.length >= digitos) {
                longitud = digitos - 4;
                c1 = c1.substring(0, longitud);
                c1 = c1 + ' ...';
            }
            var textoEnlaceNuevo = c1;
            if (hayNumero) {
                var cantidad = textoEnlace.substring(p1 + 1, textoEnlace.length - 1);
                textoEnlaceNuevo += '<span>(';
                textoEnlaceNuevo += cantidad;
                textoEnlaceNuevo += ')<\/span>';
            }
            enlace.html(textoEnlaceNuevo);
        });
    }
};


var verTodasCategoriasEtiquetas = {
    init: function () {
        this.comportamiento();
    },
    config: function () { return },
    comportamiento: function () {
        $('#section  .resource .verTodasCategoriasEtiquetas').each(function () {
            $(this).unbind();
            $(this).bind('click', function (evento) {
                evento.preventDefault();
                var enlace = $(evento.target);
                var ul = enlace.parents('ul');
                var lis = $('li', ul);
                var total = lis.size();
                if (enlace.hasClass('mas')) {
                    lis.show();
                    enlace.removeClass('mas');
                    enlace.text(form.menos);
                } else {
                    lis.each(function (indice) {
                        var li = $(this);
                        if (indice > 2 && indice < total - 1) li.hide();
                    });
                    enlace.addClass('mas');
                    enlace.text(form.masMIN+'...');
                }
            })
        });
        return;
    }
};
var limpiarActividadRecienteHome = {
    init: function () {	
		var body = $('body');
        if(body.hasClass('fichaComunidad') || body.hasClass('fichaRecurso')) return;
        this.limpiar();
    },
    mostrarContenidos: function (content) {
        var items = content.children();
        items.hide();
        var itemsMostrados = 0;
        primerParrafoConImagen = false;
		var primerItem = items.first();
		var segundoItem = primerItem.next();
        var item = '';
		var longitud = 250;
        if (primerItem.hasClass('miniatura')) {
            primerItem.show();
            segundoItem.show();
            item = segundoItem;
        }else {
			var longitudPrimerParrafo = primerItem.text().length;
			if(longitudPrimerParrafo > longitud){
				primerItem.show();
                item = primerItem;
                // Eliminar los párrafos que no se muestran para que no generen espacio adicional excluyendo el primero
                for (var i = 1, l = items.length; i < l; i++) {
                    items[i].remove();
                }
			}else{
				primerItem.show();
				segundoItem.show();
				item = segundoItem;
				longitud = longitud - longitudPrimerParrafo;
			}
        }
		this.acortarItem(item, longitud);			
        this.buscarEnlace(item);
        return;
    },
    acortarItem: function (item, longitud) {
        var texto = item.text();
        if (texto.length > longitud) {
            var acortado = texto.substring(0, longitud);
            //item.text(acortado);
            // Añadir ... para que de sensación de continuidad
            item.text(acortado + "...");
        }
        return;
    },
    buscarEnlace: function (item) {
        var content = item.parents('div').first();
        var verMasRecurso = content.next();
		if(!verMasRecurso.hasClass('verMasRecurso')) return;
        var enlace = verMasRecurso.find('a');
        item.append(enlace);
		return;
    },
    resetear: function (item) {
        var isSpan = item.nodeName == 'SPAN';
        var item = $(item);
        if (!isSpan) item.attr('style', '');
        if (!item.hasClass('miniatura')) item.attr('class', '');
        item.attr('size', '');
        item.attr('face', '');
        return;
    },

    /**
    * Limpiar el contenido de recursos o comentarios para que no aparezcan formatos HTML. Adem�s de acortarlos textos de comentarios (Vista preliminar)".
    */
    limpiar: function () {
        var that = this;
        // Actualizado el item para que sepa donde limpiar el contenido
        $('.resource-list .descripcionResumida, #content .comment-content').each(function () {
            var content = $(this);
            if (!content.hasClass('limpio')) {
                var parrafos = $('p', content);
                var span = $('span', content);
                var font = $('font', content);
                var img = $('img', content);
                var div = $('div', content);
                var a = $('a', content);
                var ul = $('ul', content);
                var li = $('li', content);
                var items = [parrafos, span, font, img, div, a, ul, li];
                $.each(items, function () {
                    $.each(this, function () {
                        that.resetear(this);
                    });
                });
                parrafos.each(function (indice) {
                    var item = $(this);
                    if (item.hasClass('miniatura')) return;
                    var texto = item.text();
                    texto = $.trim(texto);
                    if (texto == '' || texto == ' ' || texto == '&nbsp' || texto == ' &nbsp' || texto == null) {
                        item.remove();
                    };
                });
                div.each(function (indice) {
                    var item = $(this);
                    var hasParrafos = false;
                    var parrafos = $('p', item);
                    // Deprecado la funci�n size -> Usar propiedad length
                    //if (parrafos.size() > 0) hasParrafos = true;                    
                    if (parrafos.length > 0) hasParrafos = true;
                    if (!hasParrafos) {
                        var html = '<p>';
                        html += item.html();
                        html += '<\/p>';
                        item.after(html);
                        item.remove();
                    } else {
                        item.after(item.html());
                        item.remove();
                    }
                });
                that.mostrarContenidos(content);                
                content.addClass('limpio');
            }
        })
        return;
    }
}
var pintarRecursoVideo = {
	css: '.recursoVideo',
	cssMiniatura: '.miniatura',
	cssListado: '.resource-list',
	cssItemsVideo: '#content .resource-list .descripcionResumida .recursoVideo',
	init: function(){
		this.config();
		this.comportamiento();
	},
	config: function(){
		this.itemsVideo = $(this.cssItemsVideo, '#section');
		return;
	},
	comportamiento: function(){
		this.itemsVideo.each(function(){
			var item = $(this);
			var enlace = $('a', item);
			var ruta = enlace.attr('href');
			item.append('<a href=\"' + ruta + '\" class=\"resourceTypeVideo\">recurso video<\/a>');
		});
		return;
	}
}
var viewGridHome = {
	resource: '.resource',
	init: function(listado){
		this.listado = $(listado);
		this.recursos = $(this.resource, this.listado);
		var contador = 0;
		this.recursos.each(function(){
			if(contador == 2){
				var recurso = $(this);
				recurso.addClass('omega');
				recurso.after('<div class=\"clearFile\"><\/div>');
				contador = -1;
			}
			contador ++;
		});
	}
}
var viewListDestacadoHome = {
	resource: '.resource',
	init: function(listado){
		this.listado = $(listado);
		this.recursos = $(this.resource, this.listado);
		var contador = 0;
		this.recursos.each(function(){
			if(contador == 1){
				var recurso = $(this);
				recurso.addClass('omega');
				recurso.after('<div class=\"clearFile\"><\/div>');
				contador = -1;
			}
			contador ++;
		});
	}
}
var recursoCompactado = {
	cssContendioExtendido: 'contendioExtendido',
	idCustomAboutResource: '#customAboutResource',
	opcionesDesplegables: ['li.licencia','li.certificado'],
	init: function(){
		this.config();
		this.enganchar();
		return;
	},
	config: function(){
		this.customAboutResource = $(this.idCustomAboutResource);
		this.customAboutResource.append('<div class=\"'+ this.cssContendioExtendido +'\"><p>contenido extendido<\/p><\/div>');
		this.contenidoExtendido = $('.' + this.cssContendioExtendido, this.customAboutResource);
		return;
	},
	enganchar: function(){
		var that = this;
		for (var contador = 0; contador < this.opcionesDesplegables.length; contador ++){
			var opcion = $(this.opcionesDesplegables[contador], this.customAboutResource);
			var etiqueta = opcion.find('span.label');
			etiqueta.addClass('activado');
			var lis = $('li', this.customAboutResource);
			etiqueta.bind('click', function(evento){
				evento.stopPropagation();
				var etiqueta = $(evento.target);
				var opcion = etiqueta.parent();
				var valor = opcion.find('span.value').html();
				if(!opcion.hasClass('activo')){
					lis.removeClass('activo');
					that.contenidoExtendido.html(valor);
					that.contenidoExtendido.show();	
					opcion.addClass('activo');
				}else{
					that.contenidoExtendido.hide();	
					opcion.removeClass('activo');
				}
			});
		};
		return;
	}
}

var iconografia = {
	cssItems: ['a.megusta', 'a.nomegusta'],
	cssIconizer: 'iconizer',
	init: function(){
		var that = this;
		$.each(this.cssItems, function(indice, valor){
			var items = $(valor);
			items.each(function(){
				var item = $(this);
				if(!item.hasClass(that.cssIconizer)){
					item.prepend('<span class="\icono"\><\/span>');
					item.addClass(that.cssIconizer);
				}
			});

		});
	}
}
var ajustarTextoLogoComunidad = {
	id: '#corporativo',
	css: '.content',
	cssClase: '.identificadorClase',
	isClase: false,
	hasLogoImagen: false,
	init: function(){
		this.config();
		if(this.hasLogoImagen) return;
		this.ajustar();
		return;
	},
	config: function(){
		this.caja = $(this.id);
		this.clase = $(this.cssClase, this.caja);
		// Deprecado size()
        if (this.clase.length > 0) this.isClase = true;
		this.wrapcaja = $(this.css, this.caja);
		this.encabezado = $('h1 a', this.caja);
		this.imagen = $('img', this.encabezado);
		// Deprecado size() 
        //if (this.imagen.size() > 0) this.hasLogoImagen = true;
        if (this.imagen.length > 0) this.hasLogoImagen = true;
		return;
	},
	ajustar: function(){
		this.wrapcaja.hide();
		var texto = this.encabezado.text();
		var caracteres = texto.length;
		this.wrapcaja.css({'margin-top':'10px'});
		if(caracteres > 70){
			this.encabezado.css({'font-size':'32px'});
		}else if(caracteres > 40 && caracteres <= 60){
			this.encabezado.css({'font-size':'36px'});			
		}else if(caracteres > 20 && caracteres <= 40){
			this.encabezado.css({'font-size':'47px'});
		}else{
			this.encabezado.css({'font-size':'60px'});
		}
		this.wrapcaja.show();
		return;
	}
}
var herramientasRecursoCompactado = {
    idCustomAboutResource: '#customAboutResource',
    cssResourceTools: '.acciones',
    cssUlPrincipal: '.principal',
    cssLiToSecondary: '.toSecondary',
    anchoMaxUlPrincipal: 540,
    isCreatedSecondary: false,
    init: function () {
        this.config();
        this.resources.addClass('activo');
        this.opcionesSecundarias();
        this.anchoUlPrincipal();
        this.iconografia();
        this.engancharMoreOptions();
        this.engancharMoreOptionsLayer();
    },
    config: function () {
        this.about = $(this.idCustomAboutResource);
        this.resources = this.about.find(this.cssResourceTools);
        this.ulPrincipal = this.resources.find(this.cssUlPrincipal);
        this.liPrincipal = this.ulPrincipal.find('li');
        this.lisToSecundary = this.ulPrincipal.find('li' + this.cssLiToSecondary);
        return;
    },
    engancharMoreOptionsLayer: function () {
        var capa = this.resources.find('.moreTools');
        var parent = capa.parent();
        capa.unbind().hover(
			function () {
			    return;
			}, function () {
			    parent.hasClass('showing') ? parent.removeClass('showing') : parent.addClass('showing');
			    return;
			}
		);
        return;
    },
    engancharMoreOptions: function () {
        var enlace = this.resources.find('.opMoreOptions > a');
        enlace.unbind().bind('click', function (evento) {
            evento.preventDefault();
            var parent = enlace.parent();
            parent.hasClass('showing') ? parent.removeClass('showing') : parent.addClass('showing');
        })
        return;
    },
    crearUlSecondary: function () {
        this.ulSecondary = $('<ul class=\"secondary\">');
        this.resources.append(this.ulSecondary);
        this.isCreatedSecondary = true;
        return;
    },
    opcionesSecundarias: function () {
		// Deprecado size();
        //if (this.lisToSecundary.size() > 0) {
        if (this.lisToSecundary.length > 0) {
            this.crearUlSecondary();
            this.ulSecondary.html(this.lisToSecundary);
            this.liPrincipal = this.ulPrincipal.find('li');
        }
        return;
    },
    iconografia: function () {
        var enlaces = this.resources.find('a');
        enlaces.each(function () {
            var enlace = $(this);
            enlace.prepend('<span><\/span>');
        })
    },
    htmlLisMoreOptions: function (lis) {
        var lis = $(lis);
        var html = '';
        lis.each(function () {
            var li = $(this);
            html += '<li>' + li.html() + '<\/li>';
        });
        //html += '<li class=\"opClose last\"><a href=\"#\">cerrar<\/a><\/li>';
        return html;
    },
    anchoUlPrincipal: function () {
        var that = this;
        if (this.ulPrincipal.width() > this.anchoMaxUlPrincipal) {
            var lisMoreOptions = [];
            var lisVisibleOptions = [];
            var ancho = 0;
            this.liPrincipal.each(function () {
                var li = this;
                ancho += ($(li).width() + 22);
                if (ancho >= that.anchoMaxUlPrincipal) {
                    lisMoreOptions.push(li);
                    $(li).remove();
                };
            })
            if (!this.isCreatedSecondary) this.crearUlSecondary();

            if (this.ulSecondary.find('.opMoreOptions').size()==0) {
                this.liMoreOptions = $('<li class=\"opMoreOptions\">');
                this.aliMoreOptions = $('<a href=\"#\">Más opciones<\/a>');
                this.divMoreOptions = $('<div class=\"moreTools\">');
                this.ulDivMoreOptions = $('<ul>');
                this.divMoreOptions.append(this.ulDivMoreOptions);
                this.ulDivMoreOptions.append(this.htmlLisMoreOptions(lisMoreOptions));
                this.liMoreOptions.append(this.aliMoreOptions);
                this.liMoreOptions.append(this.divMoreOptions);
                this.ulSecondary.append(this.liMoreOptions);
            }
        };
        return;
    }
}

var onlymembers = {
	init: function(){
		$('.onlyMembers').each(function(){
			var capa = $(this);
			var imagen = $('.image', capa);
			imagen.append('<div class=\"\wrap\"><\/div>');
			capa.append('<p class=\"message\">Solo miembros / Only members<\/p>');
			var wrap = $('.wrap', imagen);
			wrap.css('opacity', 0.6);
		})	
	}
}

var onlymembersContent = {
	init: function(){
		$('.onlyMembersContent').each(function(){
			var capa = $(this);
			var mensajePersonalizado = $('.mensajePersonalizadoSoloMiembros');
			var wrap = $('<div>').attr('class','wrap').css('opacity', 0.8);
			capa.prepend(wrap);
			if(mensajePersonalizado.size() > 0) return
			capa.append('<div class=\"message\"><p>Solo miembros / Only members<\/p><p><a href=\"mgLogin.php\">Accede y participa ...<\/a><\/p><\/div>');
		});	
	}
}

var subcategoriasMenuPrincipal = {
	idPrincipal: '#nav',
	init: function(){
		this.config();
		if(!this.hasSubcategorias) return;
		this.nav.addClass('activado');
		this.engancharComportamiento();
	},
	config: function(){
		this.page = $('#page');
		this.nav = $(this.idPrincipal);
		this.subcategorias = this.nav.find('ul ul');
		// Deprecado size()
        //this.hasSubcategorias = this.subcategorias.size() > 0;
        this.hasSubcategorias = this.subcategorias.length > 0;
	},
	engancharComportamiento: function(){
		var that = this;
		this.subcategorias.each(function(indice){
			var ul = $(this);
			var li = ul.parents('li').first();
			var a = li.find('a').first();
			li.addClass('hasSubcategorias');
			a.hover(
				function(){
					that.desmarcarTodos();
					var enlace = $(this);
					var li = enlace.parent().addClass('current on');
					//that.comportamiento(evento);
				},
				function(){
					var enlace = $(this);
					var li = enlace.parent().removeClass('current');					
					return;
				}
			);
			ul.hover(
				function(evento){				
					return;
				},
				function(evento){
					var ul = $(this);
					var li = ul.parent().removeClass('on');					
					return;
				}
			);		
		});
		this.page.hover(
			function(){
				that.desmarcarTodos()
			},
			function(){
				return
			}
		)	
		return;
	},
	desmarcarTodos: function(){
		var items = this.nav.find('.on');
		items.each(function(){
			var item = $(this);
			if(!item.hasClass('current')) item.removeClass('on')
		});
		return;
	}
}
var abreEnVentanaNueva = {
	isFicha: false,
	idFichaCatalogo: 'fichaCatalogo',
	idFichaRecurso: 'fichaRecurso',
	init: function(){
		this.body = $('body');
		if(this.body.hasClass(this.idFichaCatalogo) || this.body.hasClass(this.idFichaRecurso)) this.isFicha = true;
		if(!this.isFicha) return;
		this.config();
		this.comportamiento();
		return;
	},
	config: function(){
		this.recurso = this.body.find('#col02 .resource').first();
		this.title = this.recurso.find('.title');
		this.enlace = this.title.find('a');
		return;
	},
	comportamiento: function(){
		var html = '<span class=\"icono\" title=\"enlace recurso externo\"></span>';
		var isExterno = $(this.enlace).attr('target') == '_blank';
		if(isExterno) this.enlace.append(html);
		return;
	}
}
var redesSocialesRecursoCompactado = {
	idRedesSociales: '.redesSocialesCompartir',
	init: function(){
		this.config();
		this.ocultar();
		this.engancharComportamiento();
	},
	config: function(){
		this.redes = $('#content ' + this.idRedesSociales).first();
		this.ul = this.redes.find('ul');
		this.lis = this.redes.find('li');
		return;
	},
	ocultar: function(){
		this.lis.each(function(){
			var li = $(this);
			if(!li.hasClass('big') && !li.hasClass('mostrar')) li.hide();
		});
		this.isRedesOcultas = true;
		return;
	},
	mostrar: function(){
		this.lis.each(function(){
			var li = $(this);
			if(!li.hasClass('big') && !li.hasClass('mostrar')) li.show();
		});
		this.isRedesOcultas = false;
		return;
	},	
	engancharComportamiento: function(){
		var that = this;
		this.enlace = $('<a href="#">').text('mostrar');
		this.liMostrar = $('<li>').attr('class','mostrarMas').append(this.enlace);
		this.ul.append(this.liMostrar);
		this.enlace.bind('click', function(evento){
			evento.preventDefault();
			that.comportamiento(evento);
		})
		return;
	},
	comportamiento: function(evento){
		var enlace = $(evento.target);
		if(this.isRedesOcultas){
			this.mostrar();
			enlace.parent().addClass('menos');
		}else{
			this.ocultar();
			enlace.parent().removeClass('menos');		
		}
		return;
	}
}

var seleccionarPreferencias = {
    componentes: ['.fieldset01', '.fieldset02', '.fieldset03', '.fieldset04', '.fieldset05', '.fieldset06', '.fieldset07', '.fieldset08', '.fieldset09', '.fieldset010', '.fieldset011'],
	init:function(){
		var that = this;
		$.each(this.componentes, function(indice, valor){
			var item = $(valor);
			var items = item.find('li');
			var labels = item.find('label');
			var checks = item.find('input[type=checkbox]');
			items.each(function(contador){
				var item = $(this);
				item.addClass('item' + indice + contador);
			});
			item.addClass('activo');
			items.bind('click', function (evento) {
			    evento.preventDefault();
				that.comportamiento(evento);
			});
			labels.bind('click', function(evento){
				evento.preventDefault();
			});		
		});
	},
	changeCheck: function(li){
		var li = $(li);
		var check = li.find('input[type=checkbox]');
		li.hasClass('on') ? li.removeClass('on') : li.addClass('on');
		check.is(':checked') ? check.attr('checked', false) : check.attr('checked', true);
		return;
	},
	comportamiento:function(evento){
		var item = $(evento.target);
		var name = evento.target.nodeName;
		var li = item;
		if(name != 'LI') li = item.parents('li').first();
		this.changeCheck(li);
		return;
	}
}
var marcarObligatorios = {
	init: function(){
		this.datosTipoTexto();
		return;
	},
	datosTipoTexto: function(){
	    var campos = $('.fieldset01 input[type=text], .fieldset01 input[type=password], .fieldset01 select.dato');
		campos.each(function(indice){
			var campo = $(this);
			var padre = campo.parent();
			var label = padre.find('label');
			if(label.hasClass('datoObligatorio')){
				label.attr('title', 'campo obligatorio');
				label.prepend('<span class="datoObligatorio">*</span>');
			}
		})
		return;
	}
}
var customizeFile = {
	init: function(){
		$('.customizeFileUpload').each(function(){
			var contenedor = $(this);
			var enlaces = contenedor.find('a.cambiar');
			var selectorFile = contenedor.attr('rel');
			var contenedorFile = $('.' + selectorFile).hide();
			enlaces.bind('click', function(evento){
				evento.preventDefault();
				var enlace = $(evento.target);
				var contenedor = enlace.parents('.customizeFileUpload');
				var selectorFile = contenedor.attr('rel');
				var contenedorFile = $('.' + selectorFile);
				var selectorFile = contenedor.attr('rel');
				var file = contenedorFile.find('input[type="file"]');
				file.trigger('click');
			});								
		})
	}
};
var ajusteFechaPublicador = {
	init: function(){
		$('.publicacion').each(function(indice){
			var item = $(this);
			var recurso = item.parents('.resource');
			var author = recurso.find('.author');
			var hasAuthor = author.size() > 0;
			if(hasAuthor){
				item.addClass('enCajaAuthor');
				author.append(item)
			}
		});	
	}
}
var desplegableGenerico = {
	idView: '#view',
	cssItemDesplegable: 'desplegable',
	lis: [],
	init: function(){
		this.config();
		this.view.addClass('activado');
		if(this.view <= 0) return;
		this.enganchar();
	},
	config: function(){
		var that = this;
		this.view = $(this.idView);
		var li = this.view.find('li').first();
		var lis = li.siblings();
		this.lis.push(li);
		lis.each(function(indice, valor){
			var li = $(this);	
			if(li.text() == ''){
				li.remove();
			}else{
				that.lis.push(li);
			}
		})
		return;
	},
	ocultar: function(){
		$.each(this.lis, function(indice){
			var item = this;
			if(!item.hasClass('current')) item.addClass('off');
		})
		return;
	},
	comportamiento: function(evento){
		var enlace = $(evento.target);
		enlace.blur();
		var li = enlace.parents('li').first();
		li.addClass('current');
		this.ocultar();
		li.hasClass('off') ? li.removeClass('off') : li.addClass('off');
		li.removeClass('current');
		return;
	},	
	enganchar: function(){
		var that = this;
		$.each(this.lis, function(indice){
			var item = this;
			var enlace = item.find('a').first();
			var ul = item.find('ul');
			var lis = ul.find('li');
            // Deprecado size()
            //if (ul.size() > 0) {
            if (ul.length > 0) {
                // Deprecado size()
                //if (lis.size() > 0) {
                if (lis.length > 0) {
					enlace.addClass('principal');
					item.addClass('withOpciones off');
					enlace.bind('click', function(evento){
						evento.preventDefault();
						that.comportamiento(evento);
					})
				}else{
					item.hide();
				}
			}
		})
		return;
	}
}

// Cambiado por antiguo front
// Lo eliminar�
/*var marcarPasosFormulario = {
	init: function(){
		this.config();
		this.marcarItems();
		return;
	},
	config: function(){
		this.content = content;
		this.wraper = this.content.find('.formSteps').first();
		this.pasos = this.wraper.find('li');
		return;
	},
	marcarItems: function(){
		var items = this.pasos.size();
		this.pasos.each(function(indice){
			var item = $(this);
			item.addClass('item item0' + (indice + 1));
			if(indice == 0) item.addClass('first');
			if(indice == (items - 1)) item.addClass('last');
			if(item.hasClass('activo')){
				item.parent().addClass('activoItem0' + (indice + 1));
			}
		})
		return;
	}
	
}*/

var presentacionVotosRecurso = {
    isFicha: false,
    init: function () {
        var that = this;
        this.config();
        if (!this.isFicha) return;
        // Deprecado size()
        //if (this.enlace.size() <= 0) return;
        if (this.enlace.length <= 0) return;
        /*/*setTimeout(function(){
        that.traerUsuariosVotosRecursosSencillo();
        }, 1000)*/
        //WebForm_DoCallback('ctl00$ctl00$CPH1$CPHContenido$controles_ficharecursoinevery_ascx', 'VerVotantesSimple', ReceiveServerData, '', null, false);
        this.botonCerrarPanelAmpliado();
        this.enmascarar();
        this.enchangar();
        return;
    },
    config: function () {
        this.body = body;
        this.isFicha = this.body.hasClass('fichaComunidad') || this.body.hasClass('fichaComunidad');
        this.page = page;
        this.content = content;
        this.columna = this.content.find('#col02');
        this.resource = this.columna.find('.resource').first();
        this.utils = this.resource.find('.utils-1').addClass('js-activado');
        this.enlace = this.utils.find('.votosPositivos a');
        this.panelSencillo = this.utils.find('#panelVotosSimple');
        this.panelAmpliado = this.utils.find('#panelVotosAmpliado');
        return;
    },
    posicionar: function () {
        var ancho = $(window).width();
        var alto = $(window).height();
        var top = $(window).scrollTop();
        var anchoPanel = this.columna.width();
        var left = (ancho - anchoPanel) / 2;
        if (left < 0) left = 0;
        this.mascara.css({
            'width': ancho + 'px',
            'height': alto + 'px',
            'top': top + 'px'
        });
        this.panelAmpliado.css({
            'left': left + 'px',
            'top': (top + 100) + 'px'
        });
        return;
    },
    enmascarar: function () {
        this.mascara = $('<div>').addClass('mascaraPanelAmpliado').hide();
        this.body.append(this.mascara);
        return;
    },
    botonCerrarPanelAmpliado: function () {
        var that = this;
        var parrafo = $('<p>').addClass('cerrarPanelAmpliado');
        var enlace = $('<a>').attr('href', '#cerrarPanelAmpliado').text('cerrar');
        parrafo.append(enlace);
        this.panelAmpliado.append(parrafo);
        enlace.bind('click', function (evento) {
            evento.preventDefault();
            var enlace = $(evento.target);
            that.body.css("overflow", "auto");
            that.panelAmpliado.hide();
            that.mascara.hide();
        })
        return;
    },
    traerUsuariosVotosRecursosSencillo: function () {
        var that = this;
        ////var url = 'includes/recurso/usuariosVotaronRecursoSencillo.php';
        //WebForm_DoCallback('ctl00$ctl00$CPH1$CPHContenido$controles_ficharecursoinevery_ascx', 'VerVotantesSimple', ReceiveServerData, '', null, false);

        /////
        /*var respuesta = $.ajax({
        url: url,
        type: "GET",
        dataType: "html"
        })
        .done(function(pagina) {
        var html = pagina;
        that.panelSencillo.children().html(html);	
        return;
        })
        .fail(function( jqXHR, textStatus ) {
        alert( "Request failed: " + textStatus );
        })*/
        return;
    },
    traerUsuariosVotosRecursos: function () {
        //if ($('#ctl00_ctl00_CPH1_CPHContenido_controles_ficharecurso_ascx_divResource').length > 0) {
        //    WebForm_DoCallback('ctl00$ctl00$CPH1$CPHContenido$controles_ficharecurso_ascx', 'VerVotantes', ReceiveServerData, '', null, false);
        //}
        //else {
        //    WebForm_DoCallback('ctl00$ctl00$CPH1$CPHContenido$controles_ficharecursoinevery_ascx', 'VerVotantes', ReceiveServerData, '', null, false);
        //}
        var that = this;
        var url = document.location.href + '/load-voters';
        //var respuesta = $.ajax({
        //    url: url,
        //    type: "POST",
        //    dataType: "html"
        //})
        GnossPeticionAjax(url, null, true)
        .done(function (html) {
            that.panelAmpliado.find('.wrap').first().html(html);	
            CompletadaCargaUsuariosVotanRecurso();
            return;
        })
        //.fail(function( jqXHR, textStatus ) {
        //alert( "Request failed: " + textStatus );
        //})
        return;
    },
    enchangar: function () {
        var that = this;
        this.enlace.bind('click', function (evento) {
            evento.preventDefault();
            $(window).scrollTop(0);
            that.panelSencillo.hide();
            that.posicionar();
            that.body.append(that.panelAmpliado);
            that.body.css("overflow", "hidden");
            that.panelAmpliado.show();
            that.mascara.show();
            if (that.panelAmpliado.hasClass("no-data-panel")) {
                that.traerUsuariosVotosRecursos();
                /*setTimeout(function(){
                that.traerUsuariosVotosRecursos();
                }, 800);*/
                //if ($('#ctl00_ctl00_CPH1_CPHContenido_controles_ficharecurso_ascx_divResource').length > 0) {
                //    WebForm_DoCallback('ctl00$ctl00$CPH1$CPHContenido$controles_ficharecurso_ascx', 'VerVotantes', ReceiveServerData, '', null, false);
                //}
                //else {
                //    WebForm_DoCallback('ctl00$ctl00$CPH1$CPHContenido$controles_ficharecursoinevery_ascx', 'VerVotantes', ReceiveServerData, '', null, false);   
                //}
            }
            that.panelAmpliado.removeClass("no-data-panel");
        });
        this.enlace.hover(
			function () {
			    var enlace = $(this);
			    if (!that.panelAmpliado.is(':visible')) that.panelSencillo.show();
			},
			function () {
			    var enlace = $(this);
			    that.panelSencillo.hide();
			}
		);
        $(window).scroll(function () {
            if (that.panelAmpliado.is(':visible')) $(window).scrollTop(0);
        })
        return;
    },
    comportamiento: function () {
        return;
    }
}
/**
 * Clase jquery para poder gestionar la aparici� y login de una vista modal para que el usuario haga login.
 * Aparecer� siempre y cuando el usuario realice una acci�n y no disponga de permisos para ejecutarla.
 * */
var operativaLoginEmergente = {
    /**
     * Acci�n que dispara directamente el panel modal de Login
     */
    init: function () {
        this.config();
        this.showModal();
        this.doHashManagement();
        this.configEvents();
    },
    /*
     * Acci�n de cerrar la vista modal 
     */
    closeModal: function () {
        $(this.idModalPanel).modal('toggle');
		return;
    },
    /*
     * Acci�n de mostrar la vista modal
     * */
    showModal: function () {
        $(this.idModalPanel).modal('show');
        return;
    },
    /*
     * Opciones de configuraci�n de la vista con el formulario modal
     * */
    config: function () {
        // Inicializar las vistas cuando est�n visibles
        // Panel Modal
        this.idModalPanel = '#modal-login',
        // Referencia al formulario
        this.idForm = '#formPaginaLogin';
        this.bodyClassNameRegistro = 'operativaRegistro';

        // Captar el formulario Login si se est� en la p�gina Login. Si no est� en p�gina Login --> Coger el formulario del modal
        this.form = this.isLoginCurrentPage ? $('body').find(this.idForm) : $(this.idModalPanel).find(this.idForm);

        // Inputs y botones
        this.idInputEmail = '#usuario_Login',
        this.inputEmail = $(this.form).find(this.idInputEmail),
        this.idInputPassword= '#password_login',    
        this.inputPassword= $(this.form).find(this.idInputPassword),
        this.idButtonLogin= '#btnSubmit',
        this.buttonLogin= $(this.form).find(this.idButtonLogin),	
        // Paneles de error
        this.idLoginPanelError= '#loginError .ko',
        this.loginPanelError= $(this.form).find(this.idLoginPanelError),
        this.idLoginPanelErrorTwice= '#logintwice .ko',
        this.loginPanelErrorTwice= $(this.form).find(this.idLoginPanelErrorTwice),
        this.idLoginErrorAutenticacionExterna = '#loginErrorAutenticacionExterna .ko',
        this.loginErrorAutenticacionExterna = $(this.form).find(this.idLoginErrorAutenticacionExterna),
        this.panelesError = [this.loginPanelError, this.loginPanelErrorTwice, this.loginErrorAutenticacionExterna];
    },

    /**
    * Configuraci�n de los eventos (clicks, focus) de los inputs del panel/formulario  
    */
    configEvents: function () {
        // Referencia al 'emergente panel login'
        const that = this;

        // Bot�n Login
        this.buttonLogin.click(function (event) {
            // Ocultar por defecto posibles mensajes de error
            that.panelesError.forEach(panelError => panelError.hide());
            // Hacer login solo si los datos han sido introducidos
            if (that.validarCampos() == true) {
                // Mostrar loading
                MostrarUpdateProgress();
                that.form.submit();
            } else {
                that.loginPanelError.show();
            }
        });

        // Input Password (Hacer login si se pulsa "Enter" desde input password)
        this.inputPassword.keypress(function (event) {
            event.keyCode === 13 ? that.buttonLogin.click() : null
        });

    },
 
    /**
    * Comprobar que los campos (email y password) no est�n vac�os
    * @returns {bool}    
    */
    validarCampos: function () {
        return (this.inputEmail.val() != '' && this.inputPassword.val() != '');
    },

    /**
    * Comprobar si la p�gina actual es la p�gina principal de Login de una comunidad
    * Se puede saber comprobando si se dispone de una clase en el Login concreta    
    * @returns {bool}    
    */
    isLoginCurrentPage: function () {
        return $('body').hasClass(this.bodyClassNameRegistro);
    },

    /**
    * Gest�n de los Hash que hac�a en la p�gina Login anterior -> Gesti�n de errores
    * Solo ha de ejecutarse cuando el usuario se encuentre en la p�gina Login
    */
    doHashManagement: function () {
        if (this.isLoginCurrentPage()) {
            if (ObtenerHash() == '#error') {
                this.loginPanelError.show();
            }
            else if (ObtenerHash().indexOf('&') > 0) {
                var mensajeError = ObtenerHash().split('&')[1];
                if (mensajeError != '') {
                    $('#mensajeError').text(mensajeError);
                    this.loginPanelError.show();
                }
            }
            else if (document.location.href.endsWith('logintwice')) {
                this.loginPanelErrorTwice.show();
            }
            if (ObtenerHash() == '#errorAutenticacionExterna') {
                this.loginErrorAutenticacionExterna.show();
            }
        }        
    },
};

/**
 * Comprobar si el bot�n de may�sculas est� activado
 *
 * @param {Object} e A keypress event
 * @returns {Boolean} isCapsLock
 */
function isCapsLock(e) {
    e = (e) ? e : window.event;

    var charCode = false;
    if (e.which) {
        charCode = e.which;
    } else if (e.keyCode) {
        charCode = e.keyCode;
    }

    var shifton = false;
    if (e.shiftKey) {
        shifton = e.shiftKey;
    } else if (e.modifiers) {
        shifton = !!(e.modifiers & 4);
    }

    if (charCode >= 97 && charCode <= 122 && shifton) {
        return true;
    }

    if (charCode >= 65 && charCode <= 90 && !shifton) {
        return true;
    }

    return false;
};

/**
 * Clase jquery para poder gestionar el proceso de registro de un usuario
 * Este proceso en cuesti�n se encarga de la gesti�n del paso 1 de registro del usuario
 * Ej: Proceso de registro al acceder a la url: http://depuracion.net/comunidad/gnoss-developers-community/hazte-miembro
 * */
const operativaRegistroUsuarioPaso1 = {    
    /**
     * Acci�n para inicializar elementos y eventos
     */
    init: function (pParams) {
        this.config(pParams);
        this.configEvents();
    },

    /*
     * Opciones de configuraci�n de la vista con todas los inputs necesarios para realizar el registro del usuario
     * */
    config: function (pParams) {
        // Inicializaci�n de las vistas    
        this.txtFechaNac = $(`#${pParams.idTxtFechaNac}`);
        this.txtFechaNacDia = $(`#${pParams.idTxtFechaNacDia}`);
        this.txtFechaNacMes = $(`#${pParams.idTxtFechaNacMes}`);
        this.txtFechaNacAnio = $(`#${pParams.idTxtFechaNacAnio}`);
        this.txtNombre = $(`#${pParams.idTxtNombre}`);
        this.nombreCampoTxtNombre = pParams.nombreCampoTxtNombre;
        this.txtApellidos = $(`#${pParams.idTxtApellidos}`);
        this.nombreCampoTxtApellidos = pParams.nombreCampoTxtApellidos;
        this.txtCargo = $(`#${pParams.idTxtCargo}`);
        this.lblCargo = pParams.lblCargo;
        this.txtEmail = $(`#${pParams.idTxtEmail}`);
        this.txtNombreUsuario = $(`#${pParams.idTxtNombreUsuario}`);
        this.txtEmailTutor = $(`#${pParams.idTxtEmailTutor}`);
        this.txtContrasenya = $(`#${pParams.idTxtContrasenya}`);
        this.nombreCampoTxtContrasenya = pParams.nombreCampoTxtContrasenya;
        this.captcha = $(`#${pParams.idCaptcha}`);
        this.ddlPais = $(`#${pParams.idDdlPais}`);

        this.mensajePersonalizado = pParams.mensajePersonalizado;
        this.currentUrl = pParams.currentUrl;
        this.datepickerConfigOptions = pParams.datepickerConfigOptions;
    },

    /**
    * Configuración de los eventos 
    */
    configEvents: function () {
        // Referencia al objeto
        const that = this;

        // Input para Nombre (Quitar foco del input)
        this.txtNombre.blur(function () {
            ComprobarCampoRegistroMVC(that.nombreCampoTxtNombre);
        });

        // Input para Apellidos (Quitar foco del input)
        this.txtApellidos.blur(function () {
            ComprobarCampoRegistroMVC(that.nombreCampoTxtApellidos);
        });

        // Input para Cargo (Quitar foco del input)
        this.txtCargo.blur(function () {
            ValidarCampoNoVacio(that.txtCargo, that.lblCargo);
        });

        // Input para Cargo (Quitar foco del input)
        this.txtEmail.blur(function () {
            ComprobarEmailUsuario(that.currentUrl);
        });   

        // Input para Contrase�a (Quitar foco del input)
        this.txtContrasenya.blur(function () {
            ComprobarCampoRegistroMVC(that.nombreCampoTxtContrasenya);
        });

        // Configuraci�n del datepicker
        this.txtFechaNac.datepicker({
            changeMonth: true,
            changeYear: true,
            yearRange: that.datepickerConfigOptions.yearRange,
            maxDate: that.datepickerConfigOptions.maxdate,
        });       
    },
};

/**
 * Clase para poder gestionar las solicitudes de creación de comunidades
 *
 * */
const operativaSolicitudCreacionComunidad = {
    /**
     * Acción para para inicializar elementos y eventos
     * @param {any} pParams
     */
    init: function (pParams) {
        this.config(pParams);
        this.configEvents(pParams);
        // Establecer por defecto los paneles a mostrar descriptivos
        this.cambiarDescTipoAcceso();
    },

    config: function (pParams) {
        // Inicialización de las vistas
        this.txtNombreComunidad = $(`#${pParams.idTxtNombreComunidad}`);
        this.txtNombreCortoComunidad = $(`#${pParams.idTxtNombreCortoComunidad}`);
        this.cmbAcceso = $(`#${pParams.idCmbAcceso}`);
        this.lbEnviarSolicitud = $(`#${pParams.idLbEnviarSolicitud}`);
        this.cmbComunidadPadre = $(`#${pParams.idCmbComunidadPadre}`);
        this.panParrafoAcceso0 = $(`#${pParams.idPanParrafoAcceso0}`);
        this.panParrafoAcceso1 = $(`#${pParams.idPanParrafoAcceso1}`);
        this.panParrafoAcceso2 = $(`#${pParams.idPanParrafoAcceso2}`);
        this.panParrafoAcceso3 = $(`#${pParams.idPanParrafoAcceso3}`);
        this.panComunidadPadre = $(`#${pParams.idPanComunidadPadre}`);
        this.emptyGuid = pParams.emptyGuid;
        this.urlPost = pParams.urlPost;
    },

    configEvents: function () {
        const that = this;

        // KeyPress en Nombre de Comunidad
        this.txtNombreCortoComunidad.on("keydown", function (e) {
            if (e.which || e.keyCode) {
                if ((event.which == 13) || (event.keyCode == 13)) {
                    return false;
                }
            }
        });

        // KeyUp en Nombre corto de Comunidad
        this.txtNombreCortoComunidad.on("keyup", function (e) {
            this.value = this.value.replace(/[^a-zA-Z0-9 _ -]/g, '').trim();
            this.value = this.value.toLowerCase();
        });

        // KeyPress en Nombre corto de Comunidad
        this.txtNombreComunidad.on("keydown", function (e) {
            if (e.which || e.keyCode) {
                if ((event.which == 13) || (event.keyCode == 13)) {
                    return false;
                }
            }
        });

        // Cambiar tipo de comunidad a crear
        this.cmbAcceso.on("change", function (e) {
            that.cambiarDescTipoAcceso();
        });

        // Enviar solicutd de creación de tipo de comunidad
        this.lbEnviarSolicitud.on("click", function (e) {
            that.validarCampos();
        });

    },

    /**
     * Cambiar la descripción del tipo de comunidad según la selección del comboBox del tipo de comunidad a crear
     */
    cambiarDescTipoAcceso: function () {

        this.panParrafoAcceso0.hide();
        this.panParrafoAcceso1.hide();
        this.panParrafoAcceso2.hide();
        this.panParrafoAcceso3.hide();
        this.panComunidadPadre.hide();

        switch (this.cmbAcceso.val()) {
            case '0':
                this.panParrafoAcceso0.show();
                break;
            case '1':
                this.panParrafoAcceso1.show();
                break;
            case '2':
                this.panParrafoAcceso2.show();
                break;
            case '3':
                this.panParrafoAcceso3.show();
                this.panComunidadPadre.show();
                break;
        }
    },

    /**
    * Cambiar la descripción del tipo de comunidad según la selección del comboBox del tipo de comunidad a crear
    */
    validarCampos: function () {
        var nombreCom = $('#txtNombreComunidad').val();
        var nombreCorto = $('#txtNombreCortoComunidad').val();
        // Quitar tildes
        //nombreCorto = nombreCorto.normalize("NFD").replace(/[\u0300-\u036f]/g, "");        

        var descripcion = $('#txtDescripcionComunidad').val();

        var error = '';
        var RegExPatternnombreCom = /<|>$/;


        if (nombreCorto == '' || nombreCom == '' || descripcion == '') {
            error = form.campossolicitudcomunidadincompletos;
        }
        else if (nombreCom.match(RegExPatternnombreCom)) {
            error = form.nombrecomunidadincorrecto;
        }
        else if (!validarNombreCortoComunidad(nombreCorto)) {
            error = form.nombrecortocomunidadincorrecto;
        }

        if (error != '') {
            mostrarNotificacion("error", error);
        }
        else {
            MostrarUpdateProgress();
            const tipoComunidad = $('#cmbAcceso').val();
            let comunidadPadre = this.emptyGuid;
            if ($('#cmbComunidadPadre').length > 0) {
                comunidadPadre = $('#cmbComunidadPadre').val();
            }
            const args = nombreCom + '|' + nombreCorto + '|' + descripcion + '|' + tipoComunidad + '|' + comunidadPadre;
            var respuesta = '';
            // Construcción del objeto para hacer la petición
            var dataPost = {
                Name: nombreCom,
                ShortName: nombreCorto,
                Description: Encoder.htmlEncode(descripcion.replace(/\n/g, '')),
                Type: tipoComunidad,
                CommunityParent: comunidadPadre,
            }

            // Petición a realizar
            GnossPeticionAjax(
                this.urlPost,
                dataPost,
                true
            ).done(function (data) {
                // Mostrar ok
                mostrarNotificacion("success", data);
            }).fail(function (data) {
                // Mostrar error
                mostrarNotificacion("error", data);
            }).always(function () {
                OcultarUpdateProgress();
            })
        }
    },
};

/**
 * Clase para poder gestionar la edici�n de perfil de un usuario en la comunidad
 * 
 * */
 const operativaEditarPerfilUsuario = {

    /**
     * Acci�n para inicializar elementos y eventos
     * @param {any} pParams
     */
    init: function (pParams) {
        this.config(pParams);
        this.configEvents(pParams);
    },

    /**
     * Opciones de configuraci�n de la vista (recoger ids para poder interactuar)
     * @param {any} pParams
     */
    config: function (pParams) {

        // Inicializaci�n de las vistas
        this.deleteProfileImage = $(`#${pParams.perfilPersonal.idDeleteProfileImage}`),
        this.name = $(`#${pParams.perfilPersonal.idName}`);
        this.lastName = $(`#${pParams.perfilPersonal.idLastName}`);
        this.email = $(`#${pParams.perfilPersonal.idEmail}`);
        this.emailProfesional = $(`#${pParams.perfilPersonal.idEmailProfesional}`);
        this.bornDate = $(`#${pParams.perfilPersonal.idBornDate}`);
        this.country = $(`#${pParams.perfilPersonal.idCountry}`);
        this.region = $(`#${pParams.perfilPersonal.idRegion}`);
        this.location= $(`#${pParams.perfilPersonal.idLocation}`);
        this.postalCode = $(`#${pParams.perfilPersonal.idPostalCode}`);
        this.lang = $(`#${pParams.perfilPersonal.idLang}`);
        this.sex = $(`#${pParams.perfilPersonal.idSex}`);
        this.emailTutor = $(`#${pParams.perfilPersonal.idEmailTutor}`);
        this.idIsSearched = $(`#${pParams.perfilPersonal.idIsSearched}`);
        this.idIsExternalSearched = $(`#${pParams.perfilPersonal.idIsExternalSearched}`);
        this.chkUsarImagenPersonal = $(`#${pParams.perfilPersonal.idChkUsarImagenPersonal}`);

        // Perfil profesional
        this.nameOrganization = $(`#${pParams.perfilPersonal.idNameOrganization}`);
        this.countryOrganization = $(`#${pParams.perfilPersonal.idCountryOrganization}`);
        this.postalCodeOrganization = $(`#${pParams.perfilPersonal.idPostalCodeOrganization}`);
        this.locationOrganization = $(`#${pParams.perfilPersonal.idLocationOrganization}`);
        this.aliasOrganization = $(`#${pParams.perfilPersonal.idAliasOrganization}`);
        this.websiteOrganization = $(`#${pParams.perfilPersonal.idWebsiteOrganization}`);
        this.addressOrganization = $(`#${pParams.perfilPersonal.idAddressOrganization}`);
              
        // Edici�n secci�n Bio (CV)
        this.description = $(`#${pParams.curriculum.idDescription}`);
        this.tags = $(`#${pParams.curriculum.idTags}`);

        //Edici�n secci�n Redes Sociales
        this.urlUsuario = $(`#${pParams.redesSociales.idUrlUsuario}`);
        this.tblRedesSociales = $(`#${pParams.redesSociales.idTblRedesSociales}`);
        this.btnRedSocial = $(`#${pParams.redesSociales.idBtnRedSocial}`);
        this.twitterSocial = $(`#${pParams.redesSociales.idTwitterSocial}`);
        this.facebookSocial = $(`#${pParams.redesSociales.idFacebookSocial}`);
        this.linkedinSocial = $(`#${pParams.redesSociales.idLinkedinSocial}`);       

        // Se utilizar� la clase ya que hay muchos elementos para borrar (bot�n papelera con clase btnBorrarURL)
        this.classBorrarURL = pParams.redesSociales.idBtnBorrarUrl;
        this.btnBorrarUrl = $(`.${pParams.redesSociales.idBtnBorrarUrl}`);

        // Otros (Paneles, botones, url)
        this.divPanelInfo = $(`#${pParams.others.idDivPanelInfo}`);
        this.saveButton = $(`#${pParams.others.idSaveButton}`);
        this.urlPersonalProfileSaveProfile = pParams.others.urlPersonalProfileSaveProfile;
        this.urlPersonalProfileSaveBio = pParams.others.urlPersonalProfileSaveBio;
        this.urlPersonalProfileSaveSocialWebs = pParams.others.urlPersonalProfileSaveSocialWebs;
        this.urlImagenAnonima = pParams.others.urlImagenAnonima;

        // Inputs que NO podr�n quedar vac�os
        this.inputsNoEmpty = [this.name,
            this.lastName,
            this.email,
            this.bornDate,            
            this.emailProfesional,
            this.nameOrganization,
            this.countryOrganization,
            this.postalCodeOrganization,
            this.locationOrganization,
            this.addressOrganization
        ];

        // Nombre de las clases css para estilo de error o success (Bootstrap)
        this.okClass = "alert-success";
        this.errorClass = "alert-danger";
    },

    /**
     * Validar que los campos aqu� mencionados no est�n vac�os
     * @param {any} inputs: Array de inputs para ser recorridos y verificar que ninguno de los aqu� indicados est�n vac�os
     * @returns {bool}: Devolver� true o false siempre y cuando los inputs pasados sean diferente de vac�o
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
     * Mostrar el panel informativo con un mensaje de error o ko. Si los dos son falsos, el panel quedar� oculto
     * @param {boolean} showOK: Si se desea mostrar el mensaje de OK
     * @param {boolean} showError: Si se desea mostrar el mensaje KO
     * @param {string} message: El mensaje que ir� en el panel informativo
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
     * Configuraci�n de los eventos de los elementos html (click, focus...)
     * */
    configEvents: function (pParams) {
        const that = this;

        // Eliminar foto subida actual del usuario
        this.deleteProfileImage.on("click", function (e) {
            e.preventDefault();
            e.stopPropagation();
            that.eliminarImagenPerfil();
        });

        // Valor cambiado de inputs -> Avisar al usuario con sobreado rojo (o quitarlo) si es vac�o campo obligatorio
        this.inputsNoEmpty.forEach(input => {
            input.on("change", function () {
                if ($(this).val().length == 0) {                    
                    $(this).addClass('is-invalid');
                } else {                    
                    $(this).removeClass('is-invalid');
                }
            });
        });

        // Bot�n de guardado de los datos
        this.saveButton.on("click", function () {            
            if (that.validarCampos(that.inputsNoEmpty)) {
                // Guardar secci�n de datos personales (Nombre, Apellidos)
                that.savePersonalDataProfile();
                // Guardar secci�n de Curriculum (Tags, Descripcion)
                that.saveBioUserProfile(true);            
            } 
        });

        // Bot�n click para añadir url del input en perfil del usuario
        this.btnRedSocial.on("click", function () {
            that.addSocialWebsFromInputToTable(that.urlUsuario.val());
            // Vaciar el input rellenado
            that.urlUsuario.val('');
        });

        // Pulsaci�n Enter para guardado de URL en perfil de usuario
        this.urlUsuario.keypress(function (event) {
            if (event.keyCode === 13) {
                that.addSocialWebsFromInputToTable(that.urlUsuario.val());
                that.urlUsuario.val('');
            }
        });

        // Bot�n/Icono de papelera para borrar una red social-web
        $(document).on("click", `.${that.classBorrarURL}`, function () {
            const urlName = $(this).data("urlname");
            // Detectar si es twitter, facebook o linkedin y eliminarlo del input correspondiente
            const socialWebsInputs = $(".inputRedSocial").filter(function () {
                return $(this).val().includes(urlName.toLowerCase());
            });

            // Vaciar el input de la red social que se ha eliminado
            if (socialWebsInputs.length > 0) {
                socialWebsInputs.val('');
            }

            // Eliminar la red social de BD
            that.eliminarUrlRedSocial($(this));
        });

        // Evento change de país para que se carguen las C. Autónomas asociadas al país.
        this.country.change(function () {
            // Mostrado de Loading
            MostrarUpdateProgress();

            const dataPost = {
                callback: "CambiarPais",
                pais: $(this).val()
            }

            GnossPeticionAjax(
                location.href,
                dataPost,
                true
            ).done(function (response) {
                //that.region.parent().replaceWith(response);
                $(`#${pParams.perfilPersonal.idRegion}`).parent().replaceWith(response);
                // Reiniciar el combobox de select2 al haber sido creado de 0 siempre y cuando sea un elemento select
                if ($(`#${pParams.perfilPersonal.idRegion}`).prop('nodeName') == "SELECT") {
                    $(`#${pParams.perfilPersonal.idRegion}`).select2();
                }
            }).always(function () {
                OcultarUpdateProgress();
            });
        });

        // Input de red social Twitter cuando se salga de él, Revisar si existe en la tabla y eliminarla para sustituirla por la nueva introducida      
        this.twitterSocial.on("blur", function () {
            if ($(this).val().length > 0) {
                that.findAndUpdatePersonalSocialWeb("twitter", $(this));                
            }
        });

        // Input de red social Facebook cuando se salga de él, Revisar si existe en la tabla y eliminarla para sustituirla por la nueva introducida      
        this.facebookSocial.on("blur", function () {
            if ($(this).val().length > 0) {
                that.findAndUpdatePersonalSocialWeb("facebook", $(this));
            }
        });

        // Input de red social Facebook cuando se salga de él, Revisar si existe en la tabla y eliminarla para sustituirla por la nueva introducida      
        this.linkedinSocial.on("blur", function () {
            if ($(this).val().length > 0) {
                that.findAndUpdatePersonalSocialWeb("linkedin", $(this));
            }
        });

         // Checkbox en identidad profesional para cambiar la foto por la identidad personal
         this.chkUsarImagenPersonal.on("click", function () {
             that.setUsePersonalImage();
         });
     },

     /**
     * Acción para indicar que se desea utilizar la foto del perfil personal en la identidad profesional
     * */
     setUsePersonalImage: function () {
         const that = this;

         if (this.chkUsarImagenPersonal.is(':checked')) {
             // Ocultar la sección de imágen ya que se desea utiliza la misma que la del perfil personal
             $(`#panelLoadImageId`).hide();
         }
         else {
             $(`#panelLoadImageId`).show();
         }
         var dataPost = {
             callback: "UsarFotoPersonal",
             UsarFotoPersonal: that.chkUsarImagenPersonal.is(':checked')
         }
         GnossPeticionAjax(
             `${that.urlPersonalProfileSaveSocialWebs}`,
             dataPost,
             true
         );
     },

    /**
     * Acci�n de guardar los datos del perfil del secci�n 'Datos Personales' (Nombre, Apellidos...)
     * */
    savePersonalDataProfile: function () {
        const that = this;
        // Mostrado de Loading
        MostrarUpdateProgress();
        // Construcci�n de objeto formData
        const dataPost = new FormData();
        dataPost.append('peticionAJAX', true);

        // Recorrido de cada input para coger su name-value
        $("#formularioEdicionPerfilPersonal").find(':input').each(function () {
            let valor = "";
            if ($(this).is(':checkbox')) {
                valor = $(this).is(':checked')
            }
            else if (!$(this).is(':button')) {
                valor = $(this).val();
            }
            dataPost.append($(this).attr('name'), valor);
        });

        // Realizar petici�n de guardado de datos personales del perfil del usuario        
        GnossPeticionAjax(this.urlPersonalProfileSaveProfile, dataPost, true, false)
            .done(function (data) {
                //GuardadoCVRapido('OK'); 
                // Guardar las redes sociales y urls que están en la tabla de Redes sociales
                that.addSocialWebsFromTableAndInputsToDataBase();
                //that.showInfoPanelErrorOrOK(true, false, data);
                mostrarNotificacion('success', data);

            })
            .fail(function (data) {
                //that.showInfoPanelErrorOrOK(false, true, data);
                mostrarNotificacion('error', data);
            })
            .always(function () {
                OcultarUpdateProgress();
            });
    },

    /**
     * Acci�n de guardar los datos del perfil del usuario, secci�n 'Curriculum' (Tags, Descripci�n)   
     * */

    /**     
     * @param {boolean} isNecessaryToHaveTagsAndDescription: El API exige que haya al menos una descripci�n y un Tag para el guardado de estos datos. Tenerlo en cuenta. 
     * La idea es quitar esta restricci�n tal y como se ha comentado a Juan (29-06-2021)
     */
    saveBioUserProfile: function (isNecessaryToHaveTagsAndDescription) {

        const that = this;

        // Controlar que los items existan en la web (Organizaci�n no los suele cargar)
        if (this.tags.length > 0 && this.description.length > 0) {
            if (isNecessaryToHaveTagsAndDescription == true) {
                if (this.tags.val().length <= 1 && this.description.val().length == 0) {
                    return;
                }
            }
        } else {
            return;
        }
        // Mostrado de Loading
        MostrarUpdateProgress();

        
        // Construcci�n del objeto POST
        const dataPost = {
            Description: that.description.val(),                       
            Tags: that.tags.val()
        }
        GnossPeticionAjax(this.urlPersonalProfileSaveBio, dataPost, true, false)
            .done(function (data) {
        }).fail(function (data) {
            //GuardadoCVRapido('KO');            
        }).always(function () {
            OcultarUpdateProgress();
        });
    },

    /**
     * Buscará una cadena de una posible red social, la eliminará de la tabla y añadirá la nueva   
     * @param {any} socialWebName
     */
    findAndUpdatePersonalSocialWeb: function (socialWebName, item) {
        // Buscar posible fila que contenga la red social Twitter y eliminarla para actualizarla con el nuevo contenido
        const tableRow = $(`td`, this.tblRedesSociales).filter(function () {
            return $(this).text().includes(socialWebName);
        }).closest("tr");

        if (tableRow.length > 0) {
            tableRow.remove();
        }
        // Añadir la red social a la tabla
        this.addSocialWebsFromInputToTable($(item).val());
    },

    /**
     * Añadir a la tabla la url introducida que se haya introducido tanto en los inputs (redes sociales) como en los inputs.
     * */
    addSocialWebsFromInputToTable: function (url) {
        // Comprobar que hay al menos una red social
        if (url.length > 0) {
            let domainName = url.replace(/.+\/\/|www.|\..+/g, '');
            // Primera letra mayúscula
            domainName = domainName.charAt(0).toUpperCase() + domainName.slice(1);            
            // Mostrar / Montar la red social en la tabla
            this.montarUrlRedSocial(domainName, url);
        }
    },

    /**
     * Guardar las redes sociales que se encuentran en la tabla de redes sociales
     * */
    addSocialWebsFromTableAndInputsToDataBase: function () {
        const that = this;
        // Redes sociales que se encuentran en Tabla
        const socialWebs = $(".urlTitle", this.tblRedesSociales);

        // Comprobar que hay al menos una red social
        if (socialWebs.length > 0) {
            // Recorrer las redes sociales y hacer el guardado

            for (const socialWeb of socialWebs) {
                // Construyo objeto para guardar la BD
                const dataPost = {
                    callback: "AnyadirRedSocial",
                    url: $(socialWeb).text(),
                }
                // Realizar la petición de guardar en BD
                GnossPeticionAjax(this.urlPersonalProfileSaveSocialWebs, dataPost, true, false)
                    .done(function (data) {
                        console.log(`${data} guardada en BD`);
                        // Mostrar la tabla en caso de estar oculta
                        that.tblRedesSociales.removeClass('d-none');
                    }).fail(function (data) {
                        that.showInfoPanelErrorOrOK(false, true, data);
                    });
            }
        }
    },

    /**
     * Mostrar la url reci�n agregada en la tabla correspondiente de urls del usuario al haber sido agregada habiendo pulsado en el bot�n "A�adir"
     * @param {any} data: Nombre de la url. No se refiere a la URL o direcci�n de la web a�adida, sino al nombre propiamente dicho
     * @param {any} url: Url a�adida por el usuario
     */
    montarUrlRedSocial: function (data, url) {
        const htmlFila = `
            <tr>
              <td><img src="https://www.google.com/s2/favicons?domain=${url}" /></td>
              <td class="urlName">${data}</td>
              <td class="urlTitle">${url}</td>
              <td><span data-urlName="${data}"  class="material-icons-outlined ${this.classBorrarURL}" style="cursor: pointer" title="Eliminar" alt="Eliminar">delete</span></td>
            </tr>
        `;

        // Agregar la fila de la red social a la tabla
        this.tblRedesSociales.find('tbody').append(htmlFila);
        // Comprobar si la tabla está oculta
        if (this.tblRedesSociales.hasClass("d-none")) { this.tblRedesSociales.removeClass("d-none"); }
    },

    /**
     * Acci�n de eliminar una URL del servidor y tambi�n de la tabla de url del usuario     
     * @param {any} btnDeleteUrl: Bot�n de borrado pulsado
     */
    eliminarUrlRedSocial: function (btnDeleteUrl) {
        const that = this;

        // Nombre de la red que se desea borrar
        const nombreRed = btnDeleteUrl.data("urlname");        

        // Mostrar Loading
        MostrarUpdateProgress();
        // Construcci�n del objeto dataPost
        const dataPost = {
            callback: "EliminarRedSocial",
            nombreRed: nombreRed,
        }

        GnossPeticionAjax(this.urlPersonalProfileSaveSocialWebs, dataPost, true, false)
            .done(function (data) {
                // Eliminar la red social/web de la tabla
                var tBody = btnDeleteUrl.closest("tbody");
                // Eliminar la row de la tabla                
                btnDeleteUrl.fadeOut("normal", function () {
                    $(this).closest("tr").remove();
                    if ($('tr', tBody).length == 0) {
                        that.tblRedesSociales.addClass('d-none');
                    }
                });
                that.showInfoPanelErrorOrOK(false, false, undefined);
            }).fail(function (data) {
                //that.showInfoPanelErrorOrOK(false, true, data);
                // Eliminar la red social/web de la tabla
                var tBody = btnDeleteUrl.closest("tbody");
                // Eliminar la row de la tabla                
                btnDeleteUrl.fadeOut("normal", function () {
                    $(this).closest("tr").remove();
                    if ($('tr', tBody).length == 0) {
                        that.tblRedesSociales.addClass('d-none');
                    }
                });
            }).always(function () {
                OcultarUpdateProgress();
            });     
    },
     /**
     * Acción para poder eliminar la actual imagen del usuario
     * */
      eliminarImagenPerfil: function () {
        const that = this;
        // Mostrar el loading
        MostrarUpdateProgress();
        // Objeto para realizar la petición      
        const dataPost = {
            callback: "EliminarImagen"
        }
        // Realización de la petición de eliminar imagen del perfil
        GnossPeticionAjax(
            this.urlPersonalProfileSaveSocialWebs,
            dataPost,
            true
        ).done(function () {

            // Mostrar imagen por defecto del usuario
            /*$('#imgPerfil').attr('src', urlAnonimo);
            imgPerfil.attr('src', urlAnonimo.replace('_grande', '_peque'));*/

            // Establecer la imagen "anónima"
            const imagePlaceholder = $(`.image-uploader__img`);
            imagePlaceholder.attr('src', that.urlImagenAnonima);
            // Ocultar el botón para eliminar la imagen del perfil
            that.deleteProfileImage.addClass("d-none");

        }).fail(function () {
            mostrarNotificacion("error", "Se ha producido un error al eliminar la imagen actual del perfil.")
        }).always(function () {
            OcultarUpdateProgress();
        })
    }    
};

/**
 * Clase jquery para poder gestionar la solicitud de cambio de contrase�a de un usuario
 * 
 * */
const operativaSolicitarCambiarContrasenia = {
    /**
     * Acci�n para inicializar elementos y eventos
     */
    init: function (pParams) {
        this.config(pParams);
        this.configEvents();
    },
    /*
     * Opciones de configuraci�n de la vista con el formulario modal
     * */
    config: function (pParams) {
        // Inicializaci�n de IDS de las vistas        
        this.idTxtOldPassword = "#txtOldPassword";
        this.idTxtNewPassword = "#txtNewPassword";
        this.idTxtConfirmedPassword = "#txtConfirmedPassword";
        this.idBtnCambiarPassword = "#btnCambiarPassword";
        this.idWarningPanel = "#warning";
        this.idExpiredPanel = "#expiredPanel";
        this.idPasswordEmptyPanel = "#passwordEmptyPanel";
        this.idPasswordRequestInfoPanel = "#passwordRequestInfoPanel";

        // Url necesarias
        this.urlPasswordRequest = pParams.urlPasswordRequest;

        // Nombre de las clases css para estilo de error o success (Bootstrap)
        this.okClass = "alert-success";
        this.errorClass = "alert-danger";

        // Inicializaci�n de las vistas        
        this.txtOldPassword = $(this.idTxtOldPassword);
        this.txtConfirmedPassword = $(this.idTxtConfirmedPassword);
        this.txtNewPassword = $(this.idTxtNewPassword);
        this.btnCambiarPassword = $(this.idBtnCambiarPassword);
        this.warningPanel = $(this.idWarningPanel);
        this.passwordEmptyPanel = $(this.idPasswordEmptyPanel);
        this.passwordRequestInfoPanel = $(this.idPasswordRequestInfoPanel);
        this.panelesError = [this.warningPanel, this.passwordEmptyPanel];
        this.inputsFields = [this.txtOldPassword, this.txtConfirmedPassword, this.txtNewPassword];
    },

    /**
    * Comprobar que los campos (password old, password new y password confirmado ) no est�n vac�os
    * Comprobar que los password new y confirmado son iguales. En caso contrario, mostrar� un error
    * @returns {bool}    
    */
    validarCampos: function () {
        let isPasswordValid = false;
        if (this.txtOldPassword.val() != '' && this.txtNewPassword.val() != '' && this.txtConfirmedPassword.val() != '') {
            if (this.txtNewPassword.val() === this.txtConfirmedPassword.val()) {
                isPasswordValid = true;
            }
        } else {
            isPasswordValid = false;
        }
        return isPasswordValid;
    },

    /**
    * Configuraci�n de los eventos 
    */
    configEvents: function () {
        // Referencia al objeto
        const that = this;

        // Input Password Confirmado ENTER
        this.txtConfirmedPassword.keypress(function (event) {
            // Avisar con un mensaje si est�n activadas las may�sculas del teclado
            isCapsLock(event) ? that.warningPanel.fadeIn("slow") : that.warningPanel.fadeOut("slow");
            event.keyCode === 13 ? that.btnCambiarPassword.click() : null
        });

        // Bot�n de Aceptar - Solicitar cambio de contrase�a         
        this.btnCambiarPassword.click(function (event) {
            // Ocultar por defecto posibles mensajes de error
            that.hideErrorPanels();
            // Hacer login solo si los datos han sido introducidos
            if (that.validarCampos() == true) {
                // Realizar la petici�n de cambio de contrase�a
                that.cambiarPassword();
            } else {
                that.passwordEmptyPanel.fadeIn("slow");
            }
        });
    },

    /**
     * Ocultar los paneles informativos de errores
     * */
    hideErrorPanels() {
        this.panelesError.forEach(panelError => panelError.hide());
    },

    /**
      * Vaciar de los inputs utilizados (Ej: Una vez efectuada la solicitud, vaciar los inputs)
      */
    emptyInputs() {
        this.inputsFields.forEach(input => input.val(''));
    },

    /**
     * Funci�n para realizar la petici�n del cambio de contrase�a solicitado por el usuario
     * */
    cambiarPassword: function () {
        // Referencia al objeto
        const that = this;
        this.btnCambiarPassword.hide();
        MostrarUpdateProgress();
        // Construcci�n del objeto con los passwords
        const params = {
            OldPassword: that.txtOldPassword.val(),
            NewPassword: that.txtNewPassword.val(),
            ConfirmedPassword: that.txtConfirmedPassword.val(),
        };
        // Realizar la petici�n de cambio de password
        GnossPeticionAjax(that.urlPasswordRequest, params, false)
            .done(function () {
                // Ocultar posibles paneles de error
                that.hideErrorPanels();

                // Mostrar panel info con su clase
                that.passwordRequestInfoPanel.addClass(that.okClass);
                that.passwordRequestInfoPanel.removeClass(that.errorClass);
                var transfer = that.getParam("transferto", location.href);
                if (transfer != undefined) {
                    location.href = transfer;
                }
            })
            .fail(function () {
                // Mostrar panel info con su clase
                that.passwordRequestInfoPanel.removeClass(that.okClass);
                that.passwordRequestInfoPanel.addClass(that.errorClass);
            })
            .always(function (html) {
                // Mostrar el mensaje de success/error
                that.passwordRequestInfoPanel.html(html);
                // Mostrar el mensaje de error o de success
                that.passwordRequestInfoPanel.fadeIn("slow");
                // Mostrar de nuevo el bot�n para solicitar cambio de contrase�a
                that.btnCambiarPassword.fadeIn("slow");
                // Vaciar los inputs
                that.emptyInputs();
                // Ocultar el loading
                OcultarUpdateProgress();
            });
    },

    /**
     * Funci�n ejecutada desde cambiarPassword 
     * @param {any} param
     * @param {any} url
     */
    getParam: function (param, url) {
        /* Buscar a partir del signo de interrogaci�n ? */
        url = String(url.match(/\?+.+/));
        /* limpiar la cadena quit�ndole el signo ? */
        url = url.replace("?", "");
        /* Crear un array con parametro=valor */
        url = url.split("&");
        /*
        Recorrer el array url
        obtener el valor y dividirlo en dos partes a trav�s del signo =
        0 = parametro
        1 = valor
        Si el par�metro existe devolver su valor
        */
        x = 0;
        while (x < url.length) {
            p = url[x].split("=");
            if (p[0] == param) {
                return decodeURIComponent(p[1]);
            }
            x++;
        }
    }
};

/*
Plugin de CKEditor simplificado. Se activará sobre cualquier input que tenga la clase cke y cuyo "sibling" (activador del ckEditor) también tenga la clase 'ckeSimple'.
- El plugin hará que:
    - La "ToolBar" no esté visible si no se hace click en el input para añadir información.
    - La "ToolBar" esté visible si se hace click en el editor CKEditor.
    - La altura del CKEditor sea de 100px y si se está editando, que sea de 200px.
    - Si hay contenido, se mostrará el ckEditor. Si por el contrario está vacío, se verá de momento un único input.
*/
; (function ($) {

    // Declaración del plugin.
    $.fn.ckEditorSimple = function (method) {

        // Variables por defecto
        let defaults = {
            speed: 200,                                      // Velocidad con la que se expandirá el panel 'contents'
            isCKEditorContentPanelExpanded: true,            // Valor por defecto que indica que el panel Contents está o no expandido
            isCKEditorToolBarVisible: true,                 // Valor por defecto que indica que la Toolbar está o no visible
            isOnFocusCKEditor: false,                       // Valor por defecto que indica si está el foco en el ckEditor (El usuario está introduciendo datos)
            contentMinHeight: '40px',                       // Altura mínima de Contentl del CKEditor (contraido)
            contentMaxHeight: '250px',                      // Altura máxima del Content del CKEditor (desplegado)
        }

        // Variables por defecto + variables pasadas por parámetro
        let settings = {
            $ckEditor: undefined,                           // El propio ckEditor
            $ckEditorInstance: undefined,                   // La instancia jquery del CKEditor creado
            $ckEditorToolbar: undefined,                    // Toolbar del CKEditor
            $ckEditorContent: undefined,	                // Content del CKEditor
            ckeEditorContentLength: 0,                      // Longitud de textos que hay en el editor CKEditor
            isOnFocusCKEditor: false,                       // Valor por defecto que indica si está el foco en el ckEditor (El usuario está introduciendo datos)
        }

        // Métodos públicos que podrán ser llamados desde fuera del plugin
        const methods = {
            // Instanciación del plugin
            init: function (options) {
                return this.each(function () {
                    // Combinar las variables por defecto (defaults) + options pasadas
                    settings = $.extend({}, defaults, options)
                    const element = $(this);

                    // Inicio visualización del ckEditor -> Dejarlo desplegado si ya hay contenido (Ej: Editar un recurso)                            
                    if (!(settings.ckeEditorContentLength > 1)) {
                        helpers.animateToolbarCKEditor(false, !settings.isCKEditorContentPanelExpanded);
                    }

                    // Inicialización de observers
                    helpers.setupObserver();
                });
            },
        }

        // Métodos privados que serán de ayuda a esos públicos
        const helpers = {
            // Configuración del focus en la instancia del CKEditor para ocultar/no ocultar Toolbar y expandir/contraer el Contents de CKEditor
            // Observo posibles cambios en las clases del editor de CKEditor
            setupObserver: function () {
                let observer = new MutationObserver(function (mutations) {
                    mutations.forEach(function (mutation) {
                        if (mutation.attributeName === "class") {
                            attributeValue = $(mutation.target).prop(mutation.attributeName);
                            const arrayClassList = attributeValue.split(" ");
                            settings.isOnFocusCKEditor = false;
                            // Buscar si está o no focus en el CKEditor
                            if (jQuery.inArray("cke_focus", arrayClassList) != -1) {
                                // Está el Foco del CKEditor
                                settings.isOnFocusCKEditor = true
                            } else {
                                // No está el Foco del CKEditor
                                settings.isOnFocusCKEditor = false;
                            }
                            // Animar altura CKEditor siempre que sea diferente al valor actual
                            settings.isCKEditorContentPanelExpanded != settings.isOnFocusCKEditor && helpers.animateToolbarCKEditor(true, !settings.isCKEditorToolBarVisible);
                        }
                    });
                });

                // Observer para contenido introducido en ckEditor
                let contentObserver = new MutationObserver(function (mutations) {
                    mutations.forEach(function (mutation) {
                        if (mutation.addedNodes.length > 0 && mutation.target.className) {
                            helpers.animateToolbarCKEditor(true, true);
                        }
                        if (mutation.removedNodes.length >= 1 && !settings.isOnFocusCKEditor) {
                            // Cerrar la caja si no hay contenido y no está el focus en el ckEditor
                            helpers.animateToolbarCKEditor(true, false);
                        }
                    });
                });

                // Ejecutamos observer para comprobar posibles cambios en clases JS
                observer.observe(settings.$ckEditorInstance[0], {
                    attributes: true,
                });

                // Ejecutamos observer para comprobar posibles cambios en clases JS
                contentObserver.observe($(settings.$ckEditorContent).find("iframe").contents()[0], {
                    subtree: true,
                    childList: true,
                });
            },

            /*
            * Método para visualizar o no el Toolbar del CKEditor
            * @param {bool} animate: Indicar si se desea animar o no
            * @param {bool} extendPanel: Hacer más grande o no el Content del CKEditor         
            * */
            animateToolbarCKEditor: function (animate, extendPanel) {
                // Altura por defecto del Toolbar de ckEditor
                let toolbarHeight = 31;

                // Si hay texto o imagenes no hacer nada
                const spanItems = settings.$ckEditor.document.getBody().find("span").$.length != undefined ? settings.$ckEditor.document.getBody().find("span").$.length : 0;
                const textItems = settings.$ckEditor.document.getBody().getText().length;
                const editItems = spanItems + textItems;

                //if ((settings.$ckEditor.document.getBody().getText().length > 1)) {

                // Comprobar si está oculto ckEditor
                if (settings.$ckEditorToolbar.height() > 0) {
                    if (editItems > 1) {
                        return;
                    }
                }

                // Controlar tamaño de dispositivo para dar altura al toolbar de ckEditor
                const displayWidth = window.innerWidth;
                if (displayWidth < 740) {
                    toolbarHeight = 90;
                }

                // Altura que tendrá el panel
                const panelHeight = extendPanel ? toolbarHeight : 0;

                // Controlar animación (true/false)
                if (animate) {
                    settings.$ckEditorToolbar.animate({
                        height: panelHeight
                    }, settings.speed, function () {
                        // Guardamos flag de que el tamaño del panel ha sido modificado
                        settings.isCKEditorToolBarVisible = !settings.isCKEditorToolBarVisible;
                        // Animación visualización del $ckEditorContent
                        helpers.animateHeightCKEditor(animate, extendPanel);
                    });
                } else {
                    // Establecer tamaño sin animación               
                    settings.$ckEditorToolbar.css('height', panelHeight);
                    // Guardamos flag de que el tamaño del panel ha sido modificado
                    settings.isCKEditorToolBarVisible = !settings.isCKEditorToolBarVisible;
                    // Establecer visualización del ToolbarCKEditor sin animación
                    helpers.animateHeightCKEditor(animate, extendPanel);
                }
            },

            /*
            * Método para extender o hacer más grande o más pequeño el panel Contents de CKEditor (donde irá el contenido)
            * @param {bool} animate: Indicar si se desea animar o no
            * @param {bool} extendPanel: Hacer más grande o no el Content del CKEditor         
            * */
            animateHeightCKEditor: function (animate, extendPanel) {

                // Altura que tendrá el panel
                const panelHeight = extendPanel ? settings.contentMaxHeight : settings.contentMinHeight;

                // Quitar posible scrollbar si el panel está contraido
                extendPanel ? helpers.showScrollbar(true) : helpers.showScrollbar(false);

                // Controlar animación (true/false)
                if (animate) {
                    settings.$ckEditorContent.animate({
                        height: panelHeight
                    }, settings.speed, function () {
                        // Guardamos flag de que el tamaño del panel ha sido modificado
                        settings.isCKEditorContentPanelExpanded = !settings.isCKEditorContentPanelExpanded;                        
                    });
                } else {
                    // Establecer tamaño sin animación               
                    settings.$ckEditorContent.css('height', panelHeight);
                    // Guardamos flag de que el tamaño del panel ha sido modificado
                    settings.isCKEditorContentPanelExpanded = !settings.isCKEditorContentPanelExpanded;                    
                }
                helpers.showSendCommentButton(extendPanel);
            },

            /**
            * Método para ocultar o mostrar el botón de "Enviar comentario" que esté asociado a un ckEditor
            **/
            showSendCommentButton: function (showSendCommentButton) {
                // Contenedor asociado al ckEditor actual                
                const $containerSendComment = settings.$ckEditorInstance.parent().siblings(".accion-comentario");

                // Ocultar o mostrar el botón de "Enviar comentario"
                if ($containerSendComment.length > 0) {
                    showSendCommentButton ? $containerSendComment.css('visibility', 'visible').hide().fadeIn() : $containerSendComment.css('visibility', 'visible').hide().fadeOut();
                }
            },

            /*
            * Método mostrar u ocultar el scrollbar del contenido del ckeditor. Solo estaría disponible si el editor está desplegado (Por si hay contenido añadido)         
            * @param {bool} showScrollbar: Ocultar o permitir el scrollbar en el contenido del CKEditor
            * */
            showScrollbar: function (showScrollBar) {
                showScrollBar ? settings.$ckEditor.document.getBody().removeClass('overflow-hidden') : settings.$ckEditor.document.getBody().addClass('overflow-hidden');
            }
        }

        // run
        if (methods[method]) {
            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === "object" || !method) {
            return methods.init.apply(this, arguments);
        } else {
            $.error(`Método ${method} no existe dentro del plugion ckeSimple`);
        }
    }
})(jQuery);


/**
 * Clase jquery para poder gestionar las búsquedas de fechas en Facetas (Mes pasado, semana pasada, semestre pasado)
 * Calculará la fecha teniendo en cuenta la opción pulsada para escribir el valor en el input "Desde" y "Hasta"
 * Se utilizará la librería moment.js para el trabajo con fechas
 * 
 * */
var operativaFechasFacetas = {

    /**
     * Acción que dispara directamente la lógica de operativaFechas
     */
    init: function () {
        this.config();
        this.configEvents();
    },

    /**
     * Configuración de los elementos que tendrán eventos
     */
    config: function () {
        // Opción buscador de fechas "Última semana"    
        this.optionLastWeek = `.facet-last-week`,
            // Opción buscador de fechas "Último mes"    
            this.optionLastMonth = `.facet-last-month`,
            // Opción buscador de fechas "Último semestre"    
            this.optionLastSemester = `.facet-last-semester`,
            // Opción buscador de fechas "Último semestre"    
            this.optionLastYear = `.facet-last-year`,
            // Botón de buscar 
            this.searchButton = `.searchButton`,
            this.dropdownMenu = `.dropdown-menu`,
            // Input from-to
            this.inputFromDate = `.facet-from`,
            this.inputToDate = `.facet-to`;

    },

    /**
     * Configuración eventos de elementos HTML
     * */
    configEvents: function () {

        const that = this;

        // Opción LastWeek
        $(document).on('click', this.optionLastWeek, function (event) {
            that.getAndSetDate(this);
        });
        // Opción LastMonth
        $(document).on('click', this.optionLastMonth, function (event) {
            that.getAndSetDate(this);
        });
        // Opción Semester
        $(document).on('click', this.optionLastSemester, function (event) {
            that.getAndSetDate(this);
        });
        // Opción Year
        $(document).on('click', this.optionLastYear, function (event) {
            that.getAndSetDate(this);
        });
    },

    /**
     * Calcular el plazo de tiempo deseado y establecerlo en los inputs "from" y "to"
     * */
    getAndSetDate: function (item) {
        let localLocale = moment();
        moment.locale('es');
        localLocale.locale(false);

        // Fecha inicial
        let startDate = '';
        // Fecha final (actual)
        let endDate = localLocale.format('L');

        // Coger la última semana del día actual (-De momento se escogen por días)
        //console.log(moment().subtract(1, 'weeks').startOf('isoWeek').format('L'));
        //console.log(moment().subtract(1, 'weeks').endOf('isoWeek').format('L'));

        if ($(item).hasClass(`facet-last-week`)) {
            startDate = localLocale.subtract(7, "days");
        } else if ($(item).hasClass(`facet-last-month`)) {
            startDate = localLocale.subtract(30, "days");
        } else if ($(item).hasClass(`facet-last-semester`)) {
            startDate = localLocale.subtract(180, "days");
        } else {
            // Selección del último año
            startDate = localLocale.subtract(365, "days");
        }

        // Botón para búsqueda,  inputs para establecer fechas
        const searchBtn = $(item).parent().parent().parent().find(this.searchButton);
        const fromDateValue = $(item).parent().parent().parent().find(this.inputFromDate);
        const toDateValue = $(item).parent().parent().parent().find(this.inputToDate);

        // Escribir las fechas en inputs
        fromDateValue.val(startDate.format('L'));
        toDateValue.val(endDate);

        // Hacer click en botón de búsqueda
        searchBtn.trigger("click");
    },
}


/**
 * Clase jquery para poder gestionar la "peticion" de solicitud de cambio de contrase�a de un usuario
 * Este tipo de petici�n es ejecutada cuando el usuario ha solicitado cambio de contrase�a (por olvido), ha recibido un email y ha accedido a esa url para 
 * proceder a cambiar su contrase�a
 * 
 * */
const operativaPeticionCambiarContrasenia = {
    /**
     * Acci�n para inicializar elementos y eventos
     */
    init: function (pParams) {
        this.config(pParams);
        this.configEvents();
    },
    /*
     * Opciones de configuraci�n de la vista con el formulario modal
     * */
    config: function (pParams) {

        // Inicializaci�n de las vistas    
        this.txtLogin = $(`#${pParams.idTxtLogin}`);
        this.txtPasswordNueva = $(`#${pParams.idTxtPasswordNueva}`);
        this.txtPasswordConfirmar = $(`#${pParams.idTxtPasswordConfirmar}`);
        this.btnCambiarPassword = $(`#${pParams.idBtnCambiarPassword}`);
        this.btnRechazarPeticionCambiarPassword = $(`#${pParams.idBtnRechazarPeticionCambiarPassword}`);
        this.panelErroresInformativo = $(`#${pParams.idPanelErroresInformativo}`);
        this.bloqMayInfoPanel = $(`#${pParams.idBloqMayInfoPanel}`);
        this.panelCambioPassword = $(`#${pParams.idPanelCambioPassword}`);

        // Inputs
        this.inputsFields = [this.txtLogin, this.txtPasswordNueva, this.txtPasswordConfirmar];
        // Paneles de errores/info
        this.panelesError = [this.panelErroresInformativo, this.bloqMayInfoPanel];
        // Mensajes de error preconfigurados
        this.errorMsgNoUsuario = pParams.errorMsgNoUsuario;
        this.errorMsgNoPassword = pParams.errorMsgNoPassword;
        this.errorPasswordNoIguales = pParams.errorPasswordNoIguales;
        this.okRejectPasswordMessage = pParams.okRejectPasswordMessage;
        this.okPasswordCambiado = pParams.okPasswordCambiado;

        // Url necesarias
        this.urlPasswordRequest = pParams.urlPasswordRequest;
        this.urlRejectPasswordRequest = pParams.urlRejectPasswordRequest;

        // Comprobaci�n que los inputs han sido rellenados
        this.areInputsFilled = false;
        // Comprobaci�n que las contrase�as coinciden
        this.arePasswordsTheSame = false;


        // Nombre de las clases css para estilo de error o success (Bootstrap)
        this.okClass = "alert-success";
        this.errorClass = "alert-danger";
    },

    /**
    * Comprobar que los campos (password old, password new y password confirmado ) no est�n vac�os
    * y que las contrase�as introducidas coinciden
    * @returns {bool}    
    */
    validarCampos: function () {

        if (this.txtLogin.val() != '' && this.txtPasswordNueva.val() != '' && this.txtPasswordConfirmar.val() != '') {
            this.areInputsFilled = true;
            // Comprobar que las contrase�as introducidas son iguales
            if (this.txtPasswordNueva.val() === this.txtPasswordConfirmar.val()) {
                this.arePasswordsTheSame = true;
            } else {
                this.arePasswordsTheSame = false;
            }
        }
    },

    /**
    * Configuraci�n de los eventos 
    */
    configEvents: function () {
        // Referencia al objeto
        const that = this;

        // Input Password - Control de may�sculas
        this.txtPasswordNueva.keypress(function (event) {
            // Avisar con un mensaje si est�n activadas las may�sculas del teclado
            isCapsLock(event) ? that.bloqMayInfoPanel.fadeIn() : that.bloqMayInfoPanel.fadeOut();
        });

        // Input Password Confirmar - Control de may�sculas + Enter
        this.txtPasswordConfirmar.keypress(function (event) {
            // Avisar con un mensaje si est�n activadas las may�sculas del teclado
            isCapsLock(event) ? that.bloqMayInfoPanel.fadeIn() : that.bloqMayInfoPanel.fadeOut();
            event.keyCode === 13 ? that.btnCambiarPassword.click() : null
        });

        // Bot�n de Aceptar - Solicitar cambio de contrase�a         
        this.btnCambiarPassword.click(function (event) {
            // Ocultar por defecto posibles mensajes de error
            that.hideErrorPanels();
            // Hacer login solo si los datos han sido introducidos
            that.validarCampos();
            if (that.arePasswordsTheSame == true && that.areInputsFilled == true) {
                // Realizar la petici�n de cambio de contrase�a
                //that.cambiarPassword();
                that.cambiarPassword();
            } else {
                // Mostrar el panel de error correspondiente
                if (that.arePasswordsTheSame == true) {
                    that.panelErroresInformativo.html(that.errorPasswordNoIguales);
                } else if (that.txtLogin.val() == "") {
                    that.panelErroresInformativo.html(that.errorMsgNoUsuario);
                } else if (that.txtPasswordNueva.val() == "" || that.txtPasswordConfirmar.val() == "") {
                    that.panelErroresInformativo.html(that.errorMsgNoPassword);
                } else {
                    that.panelErroresInformativo.html(that.errorPasswordNoIguales);
                }
                that.panelErroresInformativo.fadeIn();
            }
        });


        // Link/ Bot�n de cancelar solicitud de cambio de contrase�a
        this.btnRechazarPeticionCambiarPassword.click(function () {
            that.rechazarPeticion();
        });
    },

    /**
     * Ocultar los paneles informativos de errores
     * */
    hideErrorPanels() {
        this.panelesError.forEach(panelError => {
            panelError.hide();
            panelError.val('');
        });
    },

    /**
      * Vaciar de los inputs utilizados (Ej: Una vez efectuada la solicitud, vaciar los inputs)
      */
    emptyInputs() {
        this.inputsFields.forEach(input => input.val(''));
    },

    /**
     * B�scar los par�metros mandados por URL.
     * Este m�todo es llamado desde la funci�n que realiza la petici�n de cambio de password
     * @param {any} param: 
     * @param {any} url: Url a la que se realiza la petici�n para cambio de contrase�a
     */
    getParam: function (param, url) {
        /* Buscar a partir del signo de interrogaci�n ? */
        url = String(url.match(/\?+.+/));
        /* limpiar la cadena quit�ndole el signo ? */
        url = url.replace("?", "");
        /* Crear un array con parametro=valor */
        url = url.split("&");
        /*
        Recorrer el array url
        obtener el valor y dividirlo en dos partes a trav�s del signo =
        0 = parametro
        1 = valor
        Si el par�metro existe devolver su valor
        */
        x = 0;
        while (x < url.length) {
            p = url[x].split("=");
            if (p[0] == param) {
                return decodeURIComponent(p[1]);
            }
            x++;
        }
    },

    /**
     * Funci�n para realizar la petici�n del cambio de contrase�a solicitado por el usuario
     * */
    cambiarPassword: function () {
        // Referencia al objeto
        const that = this;
        this.btnCambiarPassword.hide();
        MostrarUpdateProgress();
        // Construcci�n del objeto con los passwords
        const dataPost = {
            User: that.txtLogin.val(),
            Password: that.txtPasswordNueva.val(),
            PasswordConfirmed: that.txtPasswordConfirmar.val(),
        }

        // Realizar la petici�n de cambio de password
        GnossPeticionAjax(that.urlPasswordRequest, dataPost, true)
            .done(function () {
                // Ocultar posibles paneles de error
                that.hideErrorPanels();

                // Mostrar panel info con su clase
                that.panelErroresInformativo.addClass(that.okClass);
                that.panelErroresInformativo.removeClass(that.errorClass);
                that.panelErroresInformativo.html(that.okPasswordCambiado);
                var transfer = that.getParam("transferto", location.href);
                if (transfer != undefined) {
                    location.href = transfer;
                }
                // Destruimos o eliminamos el panel de inputs para que no pueda volver a solicitar cambio de contrase�a
                that.panelCambioPassword.remove()
            })
            .fail(function (html) {
                // Mostrar panel info con su clase
                that.panelErroresInformativo.removeClass(that.okClass);
                that.panelErroresInformativo.addClass(that.errorClass);
                // Mostrar el mensaje de error
                that.panelErroresInformativo.html(html);
            })
            .always(function (html) {
                // Mostrar el mensaje de error o de success
                that.panelErroresInformativo.fadeIn();
                // Vaciar los inputs
                that.emptyInputs();
                // Ocultar el loading
                OcultarUpdateProgress();
            });
    },

    /**
     * Funci�n para cancelar o rechazar la solicitud de cambio de contrase�a.
     * */
    rechazarPeticion: function () {
        const that = this;
        MostrarUpdateProgress();
        // Ocultar posibles paneles de error
        this.hideErrorPanels();

        GnossPeticionAjax(this.urlRejectPasswordRequest, null, true).done(function () {
            // Destruimos o eliminamos el panel de inputs para que no pueda volver a solicitar cambio de contrase�a
            that.panelCambioPassword.remove()
            that.btnCambiarPassword.remove()
            // Mostrar mensaje de cancelar la solicitud de cambio de password
            that.panelErroresInformativo.html(that.okRejectPasswordMessage);
            that.panelErroresInformativo.addClass(that.okClass);
            that.panelErroresInformativo.removeClass(that.errorClass);
            that.panelErroresInformativo.fadeIn();
            OcultarUpdateProgress();
        });
    },
};

/**
 * Clase jquery para poder realizar env�os de invitaciones a una comunidad y de links de recursos (desde la ficha de recurso) a correos o contactos de una comunidad.
 * Para acceder a esta vista se acceder� 
 *  - Desde la propia ficha de recurso (Enviar Link)
 *  - Panel lateral del usuario si dispone de permisos en la comunidad para enviar invitaciones 
 * */
const operativaEnviarResource_Link_Community_Invitation = {
    /**
     * Acci�n para inicializar elementos y eventos
     */
    init: function (pParams) {
        this.config(pParams);
        this.configEvents();
        if (pParams.autocompleteParams) {
            this.configAutocompleteService(pParams.autocompleteParams);
            this.configAutocompleteServiceForCommunityGroups(pParams.autocompleteParams)
        }
    },
    /*
     * Opciones de configuraci�n de la vista
     * */
    config: function (pParams) {
        // Inicializaci�n de las vistas  
        this.txtFiltro = $(`#${pParams.idTxtFiltro}`);
        this.txtFiltroGrupos = $(`#${pParams.idTxtFiltroGrupos}`);
        this.txtCorreoAInvitar = $(`#${pParams.idTxtCorreoAInvitar}`);
        this.buttonLitAniadirCorreo = $(`#${pParams.idButtonLitAniadirCorreo}`);
        this.txtHackInvitados = $(`#${pParams.idTxtHackInvitados}`);
        this.txtHackGrupos = $(`#${pParams.idTxtHackGrupos_invite_community}`);
        // El autocomplete necesita solo el nombre del input oculto
        this.txtHackInvitadosInputName = pParams.idTxtHackInvitados;
        // El autocomplete necesita solo el nombre del input oculto
        this.txtHackGruposInvitadosInputName = pParams.idTxtFiltroGrupos;

        this.panContenedorInvitados = $(`#${pParams.idPanContenedorInvitados}`);
        this.listaDestinatarios = $(`#${pParams.idListaDestinatarios}`);
        this.listaGrupos = $(`#${pParams.idPanContenedorGrupos}`);
        this.noDestinatarios = $(`#${pParams.idNoDestinatarios}`);
        this.btnEnviarInvitaciones = $(`#${pParams.idBtnEnviarInvitaciones}`);
        this.lblInfoCorreo = $(`#${pParams.idLblInfoCorreo}`);
        this.panelInfoInvitationSent = $(`#${pParams.idPanelInfoInvitationSent}`);

        // Campos especiales para env�o de link (idioma & notas/mensaje)                
        this.txtNotas = $(`#${pParams.idTxtNotas}`);
        this.dllIdioma = $(`#${pParams.idDlIdioma}`);

        // Paneles de error/info
        this.panelesInfo = [this.panelInfoInvitationSent];

        // Url necesarias para realizar petici�n        
        this.urlSend = pParams.urlSend;
    },

    /**
    * Ocultar los paneles informativos (errores o info)
    **/
    hideInfoPanels() {
        this.panelesInfo.forEach(panel => panel.hide());
    },

    /**
    * Configuraci�n de los eventos
    */
    configEvents: function () {
        const that = this;

        // Bot�n de Aceptar - Solicitar link para cambio de contrase�a
        this.btnEnviarInvitaciones.click(function (event) {
            // Ocultar por defecto posibles mensajes de error
            that.hideInfoPanels();
            // Comprobar que el input de invitados (el oculto que almacena los ids al menos tiene emails o contactos)
            if (that.validarCampos()) {
                // Realizar la petici�n de cambio de contrase�a
                that.enviarInvitacion_EnlaceSubmit();
            }
        });

        // Bot�n de A�adir correo 
        this.buttonLitAniadirCorreo.click(function () {
            let listaCorreos = that.txtCorreoAInvitar.val().trim().replace(/^\s*|\s*$/g, "").split(",");
            for (var i = 0; i < listaCorreos.length; i++) {
                let itemCorreo = listaCorreos[i].trim();
                if (!validarEmail(itemCorreo)) {
                    mostrarNotificacion('error', form.emailValido);
                } else {
                    that.crearInvitado(null, itemCorreo, itemCorreo, true);
                }
            }
        });

        // Configurar el borrado de elementos al pulsar en (x) de un item de los destinatarios
        this.listaDestinatarios.on('click', '.tag-remove', function (event) {
            const identidad = $(event.target).parent().parent().attr("id");
            that.eliminarUsuario(null, identidad);
        });


        // Configurar el borrado de grupos al pulsar en (x) de un item de grupos        
        this.listaGrupos.on('click', '.tag-remove', function (event) {
            const identidad = $(event.target).parent().parent().attr("id");
            that.eliminarGrupo(null, identidad);
        });
    },

    /**
    * Comprobar que el campo oculto que contiene posibles destinatarios tiene cierta longitud
    * @returns {bool}    
    */
    validarCampos: function () {
        return (this.txtHackInvitados.val().length > 3);
    },

    /**
     * Acci�n que se ejecuta cuando se realice una b�squeda escribiendo el nombre de un usuario y al pulsar en uno de los resultados devueltos por autocomplete.
     * Con los datos devueltos, construir� el item y lo  meter� en "panContenedorInvitados". *@
     * @param {any} ficha
     * @param {any} nombre: El nombre del usuario seleccionado
     * @param {any} identidad: La identidad del item seleccionado
     * @param {boolean} isAnEmail: Valor que indicar� si lo que se est� intentando a�adir es un contacto (normal) o un email de un usuario
     */
    crearInvitado: function (ficha, nombre, identidad, isAnEmail) {
        // Item que se a�adir� como elemento seleccionado
        let itemHtml = "";

        if (!isAnEmail) {
            itemHtml += `<div class="tag" id="${identidad}" data-item="${identidad}">`;
            itemHtml += `<div class="tag-wrap">`;
            itemHtml += `<span class="tag-text">${nombre}</span>`;
            itemHtml += `<span class="tag-remove material-icons">close</span>`;
            itemHtml += `</div>`;
            itemHtml += `</div>`;
            // A�adir la identidad al input de invitados
            this.txtHackInvitados.val(`${this.txtHackInvitados.val()}&${identidad}`);
        } else {
            // Construyo correos separados por comas
            const correos = this.txtCorreoAInvitar.val().split(',');
            // Validar si son correos v�lidos     
            for (let i = 0; i < correos.length; i++) {
                if (correos[i] != '') {
                    if (!validarEmail(correos[i].replace(/^\s*|\s*$/g, ""))) {
                        // No es email v�lido, muestra mensaje de error
                        this.lblInfoCorreo.html(form.emailValido);
                        this.lblInfoCorreo.parent().parent().fadeIn();
                        return;
                    } else {
                        this.lblInfoCorreo.parent().parent().fadeOut();
                    }
                }
            }
            // Recorrer array de correos para ser a�adidos a la vista
            for (let i = 0; i < correos.length; i++) {
                if (correos[i] != '') {
                    let data_item = correos[i].replace(/\@/g, '_');
                    data_item = data_item.replace(".", '_');
                    itemHtml += `<div class="tag" id="${correos[i]}" data-item="${data_item}">`;
                    itemHtml += `<div class="tag-wrap">`;
                    itemHtml += `<span class="tag-text">${correos[i]}</span>`;
                    itemHtml += `<span class="tag-remove material-icons">close</span>`;
                    itemHtml += `</div>`;
                    itemHtml += `</div>`;
                    // A�adir el correo al input de invitados
                    this.txtHackInvitados.val(`${this.txtHackInvitados.val()}&${correos[i].replace(/^\s*|\s*$/g, "")}`);
                }
            }
        }

        // A�adir el item en el contenedor de destinatarios
        this.listaDestinatarios.append(itemHtml);

        // Ocultar el panel de "No destinatarios" ya que hay a�adidos
        this.noDestinatarios.fadeOut();

        // Vaciamos el input donde se ha introducido al usuario
        isAnEmail ? this.txtCorreoAInvitar.val('') : this.txtFiltro.val('');
        // Quitamos posible mensaje de error de correo a�adido
        this.lblInfoCorreo.val('');
        this.lblInfoCorreo.hide();

        if (ficha != null) {
            ficha.style.display = 'none';
        }
    },

    /**
     * Acción que se ejecuta cuando se realice una b�squeda escribiendo el nombre de un grupo de la comunidad y se seleccione un item de resultados devueltos por autocomplete.
     * Con los datos devueltos, construir� el item y lo  meter� en "panContenedorInvitados". *@     
     * @param {any} nombre: El nombre del usuario seleccionado
     * @param {any} identidad: La identidad del item seleccionado     
     */
    crearGrupoInvitado: function (nombre, identidad) {
        // Item que se añadirá como elemento seleccionado
        let itemHtml = "";

        itemHtml += `<div class="tag" id="${identidad}" data-item="${identidad}">`;
        itemHtml += `<div class="tag-wrap">`;
        itemHtml += `<span class="tag-text">${nombre}</span>`;
        itemHtml += `<span class="tag-remove material-icons">close</span>`;
        itemHtml += `</div>`;
        itemHtml += `</div>`;

        // A�adir la identidad al input de grupos    
        this.txtHackGrupos.val(`${this.txtHackGrupos.val()}&${identidad}`);

        // A�adir el item en el contenedor de grupos
        this.listaGrupos.append(itemHtml);

        // Vaciamos el input donde se ha introducido el grupo a buscar
        this.txtFiltroGrupos.val('');
    },

    /**
     * Acci�n que eliminar� a un elemento al pulsar sobre su (x). Desaparecer� del contenedor y del input oculto que contiene
     * los items seleccionados para el env�o de la solicitud
     * @param {any} fichaId             
     */
    eliminarUsuario: function (fichaId, identidad) {

        // Eliminar la identidad al input construyendo el nuevo valor que tomar�
        let newTxtHackInvitados = this.txtHackInvitados.val().replace('&' + identidad, '');
        this.txtHackInvitados.val(newTxtHackInvitados);

        // Tratar de eliminar caracteres especiales para buscar el atributo de data-item (para casos de correos electr�nicos)
        let data_item = identidad.replace(/\@/g, '_');
        data_item = data_item.replace(".", '_');
        const itemToDelete = $(`*[data-item="${data_item}"]`);
        itemToDelete.remove();

        // Comprobar si hay items para mostrar u ocultar mensaje de "Ning�n destinatario..."
        const numItems = this.listaDestinatarios.children().length;
        numItems >= 1 ? this.noDestinatarios.hide() : this.noDestinatarios.show();
    },

    /**
         * Acci�n que eliminar� a un elemento al pulsar sobre su (x). Desaparecer� del contenedor y del input oculto que contiene
         * los items seleccionados para el env�o de la solicitud
         * @param {any} fichaId             
         */
    eliminarGrupo: function (fichaId, identidad) {

        // Eliminar la identidad al input construyendo el nuevo valor que tomará
        let newTxtHackInvitados = this.txtHackGrupos.val().replace('&' + identidad, '');
        this.txtHackGrupos.val(newTxtHackInvitados);

        // Tratar de eliminar caracteres especiales para buscar el atributo de data-item (para casos de correos electr�nicos)
        let data_item = identidad.replace(/\@/g, '_');
        data_item = data_item.replace(".", '_');
        const itemToDelete = $(`*[data-item="${data_item}"]`);
        itemToDelete.remove();
    },

    /**
     * Configuraci�n del servicio autocomplete para el input buscador de nombres
     * Se pasaran los par�metros necesarios los cuales se han obtenido de la vista
     * @param {any} autoCompleteParams
     */
    configAutocompleteService(autoCompleteParams) {
        const that = this;

        // Objeto que albergar� los extraParams para el servicio autocomplete
        let extraParams = {};

        // Configuraci�n de extraParams dependiendo isEcosistemasinMetaProyecto
        if (autoCompleteParams.isEcosistemasinMetaProyecto) {
            extraParams = {
                identidad: autoCompleteParams.identidad,
                identidadMyGnoss: autoCompleteParams.identidadMyGnoss,
                identidadOrg: autoCompleteParams.identidadOrg,
                proyecto: autoCompleteParams.proyecto,
                bool_esPrivada: autoCompleteParams.esPrivada
            }
        } else {
            extraParams = {
                identidad: autoCompleteParams.identidad,
                proyecto: autoCompleteParams.proyecto,
            }
        }
        // Configuraci�n del autocomplete para el input de b�squeda de nombres
        this.txtFiltro.autocomplete(
            null,
            {
                //servicio: new WS($(`#${autoCompleteParams.idInpt_urlServicioAutocompletar}`).val(), WSDataType.jsonp),
                //metodo: autoCompleteParams.metodo,
                url: $(`#${autoCompleteParams.idInpt_urlServicioAutocompletar}`).val() + '/' + autoCompleteParams.metodo,
                type: "POST",
                delay: 0,
                multiple: true,
                scroll: false,
                selectFirst: false,
                minChars: 1,
                width: 300,
                cacheLength: 0,
                NoPintarSeleccionado: true,
                txtValoresSeleccID: that.txtHackInvitadosInputName,
                extraParams,
            }
        );

        // Configuraci�n la acci�n select (cuando se seleccione un item de autocomplete)
        this.txtFiltro.result(function (event, data, formatted) {
            that.crearInvitado(null, data[0], data[1], false);
        });
    },

    /**
         * Configuraci�n del servicio autocomplete para el input buscador de nombres de grupos de la comunidad
         * Se pasaran los par�metros necesarios los cuales se han obtenido de la vista
         * @param {any} autoCompleteParams
         */
    configAutocompleteServiceForCommunityGroups(autoCompleteParams) {
        const that = this;

        // Configurar input de autocomplete para grupos de la comunidad
        this.txtFiltroGrupos.keydown(function (event) {
            if (event.which || event.keyCode) { if ((event.which == 13) || (event.keyCode == 13)) { return false; } }
        }).autocomplete(
            null,
            {
                url: $(`#${autoCompleteParams.idInpt_urlServicioAutocompletar}`).val() + '/' + "AutoCompletarGruposInvitaciones",
                delay: 0,
                multiple: true,
                scroll: false,
                selectFirst: false,
                minChars: 1,
                width: 300,
                cacheLength: 0,

                NoPintarSeleccionado: true,
                txtValoresSeleccID: that.txtHackGruposInvitadosInputName,

                extraParams: {
                    identidad: autoCompleteParams.identidad,
                    identidadMyGnoss: autoCompleteParams.identidadMyGnoss,
                    identidadOrg: autoCompleteParams.identidadOrg,
                    proyecto: autoCompleteParams.proyecto,
                }
            });

        // Configuraci�n la acci�n select (cuando se seleccione un item de autocomplete) para grupos de la comunidad
        this.txtFiltroGrupos.result(function (event, data, formatted) {
            that.crearGrupoInvitado(data[0], data[1]);
        });
    },

    /**
    * Acción de envío de la invitación de la comunidad o del enlace
    * Se disparará al pulsar el bot�n de "Enviar"
    */
    enviarInvitacion_EnlaceSubmit: function () {
        const that = this;

        MostrarUpdateProgress();

        // Construcci�n del objeto dataPost
        let dataPost = {};
        // Construir la URL teniendo en cuenta el tipo de env�o
        let newUrlRequest = "";
        //Tener en cuenta de si no existe el idioma -> Invitaci�n de comunidad
        if (this.dllIdioma.length == 0) {
            if (that.txtNotas.val() == undefined) {
                dataPost = {
                    Guests: that.txtHackInvitados.val(),
                    Message: encodeURIComponent(that.txtNotas.val().replace(/\n/g, '')),
                    Groups: that.txtHackGrupos.val()
                };
            }
            else {
                dataPost = {
                    Guests: that.txtHackInvitados.val(),
                    Message: encodeURIComponent(that.txtNotas.val().replace(/\n/g, '')),
                };
            }
            newUrlRequest = that.urlSend + document.location.search;
        } else {
            dataPost = {
                Receivers: that.txtHackInvitados.val(),
                Message: encodeURIComponent(that.txtNotas.val().replace(/\n/g, '')),
                Lang: that.dllIdioma.val()
            }
            newUrlRequest = that.urlSend;
        }

        GnossPeticionAjax(newUrlRequest,
            dataPost,
            true
        ).done(function (response) {
            claseDiv = "ok";
            // 2 - Cerrar modal                        
            $('#modal-container').modal('hide');
            // 3 - Mostrar mensaje OK
            setTimeout(() => {
                mostrarNotificacion('success', response);
            }, 1500)
        }).fail(function (response) {
            claseDiv = "ko";
            /* Mostrar error */
            // 3 - Mostrar mensaje KO
            setTimeout(() => {
                mostrarNotificacion('error', response);
            }, 1500)
        }).always(function (response) {
            OcultarUpdateProgress();
        });
    }
};

/**
 * Clase jquery para poder gestionar la suscripci�n de un usuario a las categor�as de una comunidad
 * Se puede acceder desde el panel lateral del usuario, pulsando en "Suscribirse".
 * Podr� elegir (mediante checkbox disponibles) las categor�as a las que suscribirse y si 
 * desea recibir newsletters de forma diaria o semanal
 * */

const operativaGestionarSuscripcionComunidad = {
    /**
     * Acci�n para inicializar elementos y eventos
     */
    init: function (pParams) {
        this.config(pParams);
        this.configEvents();
    },

    /*
     * Opciones de configuraci�n de la vista
     * */
    config: function (pParams) {
        // Inicializaci�n de las vistas             
        this.chkRecibirBoletin = $(`#${pParams.idChkRecibirBoletin}`);
        this.panelFrecuenciaRecibirBoletin = $(`#${pParams.idPanelFrecuenciaRecibirBoletin}`);
        this.radioNameSuscription = $(`#${pParams.nameSuscripcion}`);
        this.txtHackCatTesSel = $(`#${pParams.idTxtHackCatTesSel}`);
        this.panelInfoSuscripcionCategorias = $(`#${pParams.idPanelInfoSuscripcionCategorias}`);
        this.rbtnSuscripcionDiaria = $(`#${pParams.idRbtnSuscripcionDiaria}`);
        this.rbtnSuscripcionSemanal = $(`#${pParams.idRbtnSuscripcionSemanal}`);
        this.btnSaveSubscriptionPreferences = $(`#${pParams.idBtnSaveSubscriptionPreferences}`);

        // Valores por defecto de Recibir boletines y su frecuencia (por defecto, false)
        this.isReceivingNewsletters = false;
        this.isFrequencyDaily = false;
        this.isFrequencyWeekly = false;

        // Url necesaria a la que habr� que hacer la petici�n
        this.urlRequestCommunitySubscription = pParams.urlRequestCommunitySubscription;
        // Paneles de info/error                                     
        this.panelesInfo = [this.panelInfoSuscripcionCategorias];

        // Nombre de las clases css para estilo de error o success (Bootstrap)
        this.okClass = "alert-success";
        this.errorClass = "alert-danger";
    },

    /**
    * Ocultar los paneles informativos (errores o info)
    **/
    hideInfoPanels() {
        this.panelesInfo.forEach(panel => panel.hide());
    },

    /**
    * Configuraci�n de los eventos
    */
    configEvents: function () {
        const that = this;

        // Checkbox (si existe) para mostrar la frecuencia de boletines newsletter
        if (this.chkRecibirBoletin != undefined) {
            this.chkRecibirBoletin.on("click", function () {
                if ($(this).is(':checked')) {
                    that.panelFrecuenciaRecibirBoletin.fadeIn();
                    // Dejar checked por defecto una opci�n del radioButton (si antes no se ha seleccionado nada)
                    if (!that.rbtnSuscripcionDiaria.is(':checked') && !that.rbtnSuscripcionSemanal.is(':checked')) {
                        // Dejar por defecto la opci�n diaria checkeada
                        that.rbtnSuscripcionDiaria.prop("checked", true);
                    }
                } else {
                    that.panelFrecuenciaRecibirBoletin.fadeOut();
                }
            });
        }
        // Bot�n de guardar cambios/enviar al servidor
        this.btnSaveSubscriptionPreferences.on("click", function () {            
            // Comprobar los valores para el env�o
            that.checkRadioButtonsAndCheckValues();
            // Enviar los datos
            that.gestionSuscripcionComunidadSubmit();
        });
    },

    /**
     * M�todo para comprobar los checks y radioButtons para crear los valores booleanos
     * para el env�o de datos al servidor
     * Comprueba primero si existen los inputs y una vez comprobado, analiza los datos
     * */
    checkRadioButtonsAndCheckValues: function () {
        if (this.chkRecibirBoletin != undefined) {
            if (this.chkRecibirBoletin.is(':checked')) {
                this.isReceivingNewsletters = true;
                // Comprobaci�n diaria o semanal
                this.rbtnSuscripcionDiaria.is(':checked') ? this.isFrequencyDaily = true : this.isFrequencyDaily = false;
                this.rbtnSuscripcionSemanal.is(':checked') ? this.isFrequencyWeekly = true : this.isFrequencyWeekly = false;
            }
        } else {
            this.chkRecibirBoletin.is(':checked') ? this.isReceivingNewsletters = true : this.isReceivingNewsletters = false;
        }
    },

    /**
    * Acci�n de env�o de ajustes en suscripci�n de la comunidad
    * Se disparar� al pulsar el bot�n de "Enviar"
     * */
    gestionSuscripcionComunidadSubmit: function () {
        const that = this;

        // Mostrar loading
        MostrarUpdateProgress();
        // Ocultar posibles mensajes de info/error
        this.hideInfoPanels();
        // Construcci�n del objeto para enviar datos
        const dataPost = {
            SelectedCategories: that.txtHackCatTesSel.val(),
            ReceiveSubscription: that.isReceivingNewsletters,
            DailySubscription: that.isFrequencyDaily,
            WeeklySubscription: that.isFrequencyWeekly
        }

        // Envio de los datos
        GnossPeticionAjax(
            that.urlRequestCommunitySubscription,
            dataPost,
            true
        ).done(function () {            
            that.panelInfoSuscripcionCategorias.addClass(that.okClass);
            that.panelInfoSuscripcionCategorias.removeClass(that.errorClass);
        }).fail(function () {
            that.panelInfoSuscripcionCategorias.removeClass(that.okClass);
            that.panelInfoSuscripcionCategorias.addClass(that.errorClass);
        }).always(function (response) {
            // Mostrar el mensaje de error
            that.panelInfoSuscripcionCategorias.html(response);
            that.panelInfoSuscripcionCategorias.fadeIn();
            OcultarUpdateProgress();
        });
    },
};

/**
 * Clase jquery para poder gestionar la solicitud de recibir (o no) de newsletters de la comunidad
 * Se puede acceder desde el panel lateral del usuario, pulsando en "Recibir newsletter".
 *
 * */
const operativaSolicitarRecibirNewsletter = {
    /**
     * Acci�n para inicializar elementos y eventos
     */
    init: function (pParams) {
        this.config(pParams);
        this.configEvents();
    },
    /*
     * Opciones de configuraci�n de la vista
     * */
    config: function (pParams) {
        // Inicializaci�n de IDS de las vistas  
        this.idChkRecibirNewsletter = pParams.chkRecibirNewsletter;
        this.idChkNoRecibirNewsletter = pParams.chkNoRecibirNewsletter;
        this.btnSubmitReceiveNewsletters = $(`#${pParams.btnSubmitReceiveNewsletters}`);

        // Nombre de los inputs
        this.nameChkReceiveNewsletter = pParams.nameChkReceiveNewsletter;

        // Inicializaci�n de las vistas 
        // Panel de posibles mensajes
        this.chkRecibirNewsletterNameInfoPanel = $(`#${pParams.chkRecibirNewsletterNameInfoPanel}`);

        // Nombre de las clases css para estilo de error o success (Bootstrap)
        this.okClass = "alert-success";
        this.errorClass = "alert-danger";

        // Url necesaria a la que habr� que hacer la petici�n
        this.urlRequestReceiveNewsletters = pParams.urlRequestReceiveNewsletters;

        // Inicializaci�n de las vistas        
        this.panelesInfo = [this.chkRecibirNewsletterNameInfoPanel]; 
    },

    /**
    * Ocultar los paneles informativos (errores o info)
    **/
    hideInfoPanels() {
        this.panelesInfo.forEach(panel => panel.hide());
    },

    /**
    * Configuraci�n de los eventos
    */
    configEvents: function () {
        const that = this;

        // Bot�n de Aceptar - Solicitar env�o o no de newsletters
        this.btnSubmitReceiveNewsletters.click(function (event) {
            // Ocultar por defecto posibles mensajes de error
            that.hideInfoPanels();

            // Conocer el inputRadio activo. Ser� el que comparemos con idChkRecibirNewsletter o idChkNoRecibirNewsletter (Desea o no desea newsletters)
            that.checkRecibirNewsLetterValue = $(`input[name=${that.nameChkReceiveNewsletter}]:checked`).val();
            // Realizar la petici�n de cambio de contrase�a
            const isReceivingNewsletters = that.checkRecibirNewsLetterValue === that.idChkRecibirNewsletter ? true : false;
            that.recibirNewsletterSubmit(isReceivingNewsletters);
        });
    },

    /**
     * Acci�n de petici�n de recibir (o no) de newsletters. Se dispara cuando se pulsa en el bot�n submit del modal     
     * @param {Bool} option: Dependiendo del input pulsado, el usuario querr� o no recibir newsletters.
     */
    recibirNewsletterSubmit: function (option) {
        const that = this;
        MostrarUpdateProgress();
        that.hideInfoPanels();
        // Construcci�n del DataPost para enviar la petici�n
        const dataPost = {            
            recibirNewsletter: option,
        };

        // Realizar la petici�n 
        GnossPeticionAjax(
            this.urlRequestReceiveNewsletters,
            dataPost,
            true
        ).done(function () {
            // Ocultar posibles paneles de info/error
            that.hideInfoPanels();
            // Mostrar panel info con su clase
            that.chkRecibirNewsletterNameInfoPanel.addClass(that.okClass);
            that.chkRecibirNewsletterNameInfoPanel.removeClass(that.errorClass);

            // Ocultar modal pasados 1.5 segundos
            setTimeout(() => {
                $('#modal-receive-newsletters').modal('hide');
            }, 1500);

        }).fail(function () {
            // Ocultar posibles paneles de error
            that.hideInfoPanels();
            // Mostrar panel info con su clase
            that.chkRecibirNewsletterNameInfoPanel.removeClass(that.okClass);
            that.chkRecibirNewsletterNameInfoPanel.addClass(that.errorClass);
        }).always(function (response) {
            // Mostramos el mensaje informativo
            that.chkRecibirNewsletterNameInfoPanel.html(response);
            that.chkRecibirNewsletterNameInfoPanel.fadeIn();
            OcultarUpdateProgress();
        });
    },
};

/**
 * Clase jquery para poder la solicitud de restablecimiento de la contrase�a por parte del usuario.
 * Para acceder a esta vista, se acceder� desde un link en "Login" de "Olvido de contrase�a"
 *
 * */
const operativaOlvidoPassword = {
    /**
     * Acci�n para inicializar elementos y eventos
     */
    init: function (pParams) {
        this.config(pParams);
        this.configEvents();
    },
    /*
     * Opciones de configuraci�n de la vista
     * */
    config: function (pParams) {
        // Inicializaci�n de las vistas  
        this.txtUserLogin = $(`#${pParams.idTxtuserLogin}`);
        this.forgetPasswordInfoPanel = $(`#${pParams.idForgetPasswordInfoPanel}`);
        this.btnEnviar = $(`#${pParams.idBtnEnviar}`);

        // Paneles de error/info
        this.panelesInfo = [this.forgetPasswordInfoPanel];

        // Url necesarias para realizar petici�n
        this.urlForgetPasswordRequest = pParams.urlForgetPasswordRequest;

        // Mensajes preconfigurados de error
        this.msgInfoEmptyField = pParams.msgInfoEmptyField;
        this.msgErrorForgetPasswordRequest = pParams.msgErrorForgetPasswordRequest;                                      
    },

    /**
    * Ocultar los paneles informativos (errores o info)
    **/
    hideInfoPanels() {
        this.panelesInfo.forEach(panel => panel.hide());
    },

    /**
    * Configuraci�n de los eventos
    */
    configEvents: function () {
        const that = this;

        // Bot�n de Aceptar - Solicitar link para cambio de contrase�a
        this.btnEnviar.click(function (event) {
            // Ocultar por defecto posibles mensajes de error
            that.hideInfoPanels();
            // Comprobar que el input no es vac�o
            if (that.validarCampos()) {                
                // Realizar la petici�n de cambio de contrase�a
                that.cambiarPasswordSubmit();
            } else {                
                that.forgetPasswordInfoPanel.html(that.msgInfoEmptyField);
                that.forgetPasswordInfoPanel.fadeIn();
            }         
        });
    },

    /**
    * Comprobar que los campos indicados no est�n vac�os (email indicado por el usuario)
    * @returns {bool}    
    */
    validarCampos: function () {
        return (this.txtUserLogin.val() != '');
    },

    /**
    * Acci�n de petici�n de solicitar cambio de contrase�a por olvido
    * Se disparar� al pulsar el bot�n y al validar que el campo del email no est� vac�o
    */
    cambiarPasswordSubmit: function () {
        const that = this;
        // Construcci�n del objeto dataPost
        var dataPost = {
            User: this.txtUserLogin.val()
        }

        MostrarUpdateProgress();
        GnossPeticionAjax(this.urlForgetPasswordRequest, dataPost, true).fail(function () {
            // Mostrar mensaje de error con la informaci�n traida del backend
            that.forgetPasswordInfoPanel.html(that.msgErrorForgetPasswordRequest);
            that.forgetPasswordInfoPanel.fadeIn("slow");            
            OcultarUpdateProgress();
        });
    }
};


var engancharComportamientoIdiomas = {
	idiomas: [],
	init: function(){
		this.config();
		this.comprobarNumeroIdiomas();
		if(this.idiomas.length < 3) return;
		this.identidad.addClass('idiomasCustomizado');
		this.montarListado();
		this.montarDesplegable();
		this.enganchar();
		this.mostrarIdiomaActivo();
		this.controlarItemBeta();
		return;
	},
	config: function(){
		this.body = body;
		this.perfilUsuarioGnoss = this.body.find('#perfilUsuarioGnoss');
		this.identidad = this.perfilUsuarioGnoss.find('#identidadGNOSS');
		// Deprecado size()
        //if (this.identidad.size() <= 0) this.identidad = this.body.find('#identidad');
        if (this.identidad.length <= 0) this.identidad = this.body.find('#identidad');
		this.gnoss = this.identidad.find('#gnoss');
		return;
	},
	comprobarNumeroIdiomas: function(){
		var that = this;
		var items = this.gnoss.children();
		items.each(function(){
			var item = $(this);
			if(!item.hasClass('logo')) that.idiomas.push(this);
		})
		return;
	},
	mostrarIdiomaActivo: function(){
		var ul = this.capa.children();
		var items = ul.first().find('li');
		var primero = items.first();
		var segundo = primero.next();
		if(primero.children().hasClass('activo')) return;
		var opciones = this.capa.find('#idiomasSelector');
		var activo = opciones.find('.activo');
		var clonado = activo.clone();
		var lang = clonado.attr('lang');
		clonado.text(lang);
		var li = $('<li />');
		li.append(clonado);
		primero.before(li);
		segundo.remove();
		return;
	},	
	montarListado: function(){
		this.capa = $('<div />').attr('id', 'idiomas');
		var ul = $('<ul />');
		var li = $('<li />');
		this.desplegar = $('<a />').addClass('desplegar').text('desplegar');
		$.each(this.idiomas, function(indice){
			var item = $(this);
			var clonado = item.clone();
			if(item.children().hasClass('activo')){
				clonado.children().addClass('activo');
			}			
			if(indice < 2){
				ul.append(clonado);
			}
		});
		li.append(this.desplegar);
		ul.append(li);
		this.capa.append(ul);
		this.gnoss.before(this.capa);
		return;
	},	
	montarDesplegable: function(){		
		var div = $('<div />').attr('id', 'idiomasSelector');
		var ul = $('<ul />');
		$.each(this.idiomas, function(indice){
			var item = $(this);
			var enlace = item.children();
			var idioma = enlace.attr('title');
			var abreviatura = enlace.attr('lang');
			var texto = idioma + ' (' + abreviatura + ')';
			enlace.text(texto);
			ul.append(item);
		});		
		div.append(ul);
		this.capa.append(div);
		div.hover(
		function(){
			return;
		},
		function(){
			$(this).hide();
		})
		return;
	},	
	controlarItemBeta: function(){
		var beta = this.capa.find('.beta');
		this.capa.after(beta);
		return;
	},
	enganchar: function(){
		this.desplegar.bind('click', function(evento){
			evento.preventDefault();
			var enlace = $(evento.target);
			var padre = enlace.parents('div');
			var opciones = padre.find('#idiomasSelector');
			opciones.is(':visible') ? opciones.hide() : opciones.show();
		})
		return;
	}
}
var html, body, page, content;

$(function () {
    html = $('html');
    body = html.find('body');
    page = body.find('#page');
    content = page.find('#content');

    var navegador = navigator.userAgent;

    if (navegador.indexOf('MSIE 7.0') > 0) {
        body.addClass('msie7');
    } else if (navegador.indexOf('MSIE 8.0') > 0) {
        body.addClass('msie8');
    }

    $('.hidePanel').each(function () {
        var link = $(this);
        link.bind('click', function (event) {
            event.preventDefault();
            var panel = link.attr('href');
			var operativa
            $(panel).hide();
        });
    });
    $('.hideShowPanel').each(function () {
        var link = $(this);
        link.bind('click', function (event) {
            event.preventDefault();
            var panel = link.attr('href');
            var panels = link.attr('rel');
            var li = link.parent();
            var ul = li.parents('ul');
            var lis = $('li', ul);
            if (!link.hasClass('noGroup')) desmarcarOpcionesGrupo(lis);
            var hasOtherPanels = false;
            if (panels != null && panels != '') hasOtherPanels = true;
            if (hasOtherPanels) ocultarPaneles(panels);
            panel = $(panel);
            if (panel.is(':visible')) {
                li.removeClass('active');
                panel.slideUp();
            } else {
                li.addClass('active');
                /*if( $.browser.msie) panel.css('display','block');*/
                panel.slideDown();
            }
        });
    });

    /* longitud facetas... */
    // Longitud facetas por CSS
    // limiteLongitudFacetas.init();

    /* numero categorias */
    mostrarNumeroCategorias.init();

    /* numero etiquetas */
    mostrarNumeroEtiquetas.init();

    /* enganchar mas menos categorias y etiquetas */
    verTodasCategoriasEtiquetas.init();

    /* presentacion facetas */
    //facetedSearch.init();


    /* presentacion icono certificado */
    $('#section p.certificado').each(function () {
        $(this).prepend('<span class=\"icono\"><\/span>');
    })

    /* presentacion votos */

    presentacionVotosRecurso.init();

    /* 
    presentacion votos 
    @deprecated 19.09.2013
    $('#section p.votosPositivos a').each(function(){
    var enlace = $(this);
    var div = enlace.parents('div').first();
    var panel = div.find('.panelVotos');
    panel.hide();
    panel.addClass('activado');
    enlace.bind('click', function(evento){
    evento.preventDefault();
    panel.is(':visible') ? panel.hide() : panel.show(); 
			
    })
    });
    */

    /* carrusel home */
    carrusel.init();

    /* carrusel comite crea */
    carruselLateralColumna.init();

    /* opciones menu identidad */
    opcionesMenuIdentidad.init();

    /* marcar seccion del menu principal activa */
    //seccion.init();
    /* enganchar modo visualizacion listados */
    //modoVisualizacionListados.init();

    /* limpiar actividad reciente home */
    limpiarActividadRecienteHome.init();

    if (body.hasClass('homeCatalogo')) {
        modoVisualizacionListadosHomeCatalogo.init();
    }
    /* add icono video */
    pintarRecursoVideo.init();

    /* ficha. acerca de este recurso compactado */
    herramientasRecursoCompactado.init();
    redesSocialesRecursoCompactado.init();

    iconografia.init();

    /* texto logo */
    ajustarTextoLogoComunidad.init();

    /* only members */
    onlymembers.init();
    onlymembersContent.init();

    /* subcategorias menu principal 2012.10.08 */
    subcategoriasMenuPrincipal.init();

    /* subcategorias menu principal 2012.11.12 */
    abreEnVentanaNueva.init();

    /* desplegables modo visualizacion */
    desplegableGenerico.init();

    /* comportamiento idiomas v.2014.05.09.01 */
	engancharComportamientoIdiomas.init();

    /* paneles colapsables 2012.11.26 
    panelesColapsablesTresNiveles.init(); 
    */

    /* ajuste paginadores de recursos */
    $('#col01 .paginadorSiguienteAnterior').each(function () {
        var paginador = $(this);
        var grupo = paginador.parents('.group.resources');
        var contextoOtraComunidad = grupo.find('.context-header');
        if (contextoOtraComunidad.size() <= 0) grupo.addClass('grupoPaginado');
    });

    /* ajuste fecha publicador 
    */
    ajusteFechaPublicador.init();

    controladorLineas.init();
    limpiarGruposVaciosSemanticView.init();

    /* selector paso 01 registro */
    if (body.hasClass('registroPaso01')) seleccionarPreferencias.init();
    // Cambiado por nuevo Front
    // Eliminarlo porque no tiene sentido
    //if (body.hasClass('operativaRegistro')) marcarPasosFormulario.init();

    /* registro */
    // Deprecado size()
    //if ($('.formularioRegistroUsuarios').size() > 0) {
    if ($('.formularioRegistroUsuarios').length > 0) {
        marcarObligatorios.init();
    }
    /* customizador file */
    customizeFile.init()

    /* swf component fullscreen  
    var swfFullScreen = $('#section .swffullscreen a');
    if(swfFullScreen.size() > 0){
    swfFullScreen.fancybox({
    'padding'           : 0,
    'autoScale'     	: true,
    'transitionIn'		: 'none',
    'transitionOut'		: 'none',
    'width'				: '98%',
    'height'			: '90%'
    });
    }
    */

    /* Ajustar pestañas */
    var pestanyas= $('#header #nav .principal li');
    // Deprecado size()
    //if (pestanyas.find('activo').size() == 0)
    if (pestanyas.find('activo').length == 0)
    {
        var rutaActual=document.location.href;
        pestanyas.each(function () {
            var pestanya = $(this);
            var enlace= pestanya.find('a');
            if(enlace!=undefined && enlace.attr('href')!=undefined && enlace.attr('href')==rutaActual)
            {
                pestanya.addClass('activo');
            }
        });
    }    
});