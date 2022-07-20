function CargarAutocompletarGrupos(pTxtFiltroID, pTxtValSeleccID, pIdentidadID, pIdentidadMyGnossID, pIdentidadOrgID, pProyectoID, pListaUrlsAutocompletar) {
	$('#' + pTxtFiltroID).autocomplete(null, {
		servicio: new WS(pListaUrlsAutocompletar, WSDataType.jsonp),
		metodo: 'AutoCompletarGruposInvitaciones',
		delay: 0,
		multiple: true,
		scroll: false,
		selectFirst: false,
		minChars: 1,
		width: 300,
		cacheLength: 0,
		NoPintarSeleccionado: true,
		txtValoresSeleccID: pTxtValSeleccID,
		extraParams: {
			identidad: pIdentidadID,
			identidadMyGnoss: pIdentidadMyGnossID,
			identidadOrg: pIdentidadOrgID,
			proyecto: pProyectoID,
		}
	});
}

function CargarAutocompletarLectoresEditoresPorBaseRecursos(control, baseRecursosID, personaID, pTxtValSeleccID, pListaUrlsAutocompletar, panelContenedorID, panResultadosID, txtHackID) {
	control.unautocomplete().autocomplete(
		null,
		{
			servicio: new WS(pListaUrlsAutocompletar, WSDataType.jsonp),
			metodo: 'AutoCompletarLectoresEditoresPorBaseRecursos',
			delay: 0,
			multiple: true,
			scroll: false,
			selectFirst: false,
			minChars: 1,
			width: 300,
			cacheLength: 0,
			NoPintarSeleccionado: true,
			txtValoresSeleccID: pTxtValSeleccID,
			extraParams: {
				baserecursos: baseRecursosID,
				persona: personaID
			}
		}
	);
	control.result(function (event, data, formatted) {
		seleccionarAutocompletarMVC(data[0], data[1], control, panelContenedorID, panResultadosID, txtHackID);
	});
}

/* Seleccionar & eliminar Autocompletar*/
function seleccionarAutocompletarMVC(nombre, identidad, txtFiltro, panelContenedorID, panResultadosID, txtHackID) {
    txtFiltro.val('');

    $('#selector').css('display', 'none');

    var contenedor = $('#' + panelContenedorID).find('#' + panResultadosID);

    if (contenedor.html().trim().indexOf('<ul') == 0) {
        contenedor.html(contenedor.html().replace('<ul class=\"icoEliminar\">', '').replace('</ul>', ''));
    }
    // Cambiado por nuevo Front
    // contenedor.html('<ul class=\"icoEliminar\">' + contenedor.html() + '<li>' + nombre + '<a class="remove" onclick="javascript:eliminarAutocompletarMVC(this,\'' + identidad + '\',\'' + panelContenedorID + '\',\'' + panResultadosID + '\',\'' + txtHackID + '\');">' + textoRecursos.Eliminar + '</a></li>' + '</ul>');
    // Construyo el editor
    const editorSeleccionadoHtml = `
        <div class="tag">
            <div class="tag-wrap">
                <span class="tag-text">${nombre}</span>
                <span class="tag-remove material-icons"
                      onclick="javascript:eliminarAutocompletarMVC(this,'${identidad}','${panelContenedorID}','${panResultadosID}','${txtHackID}');">close
                </span>
                <input type="hidden" value="${identidad}"/>
            </div>
        </div>
    `;
    // Añadir editor al contenedor
    contenedor.append(editorSeleccionadoHtml);

    // Añadirlo al input txtHack
    var txtHack = $('#' + panelContenedorID).find('#' + txtHackID);
    txtHack.val(txtHack.val() + "," + identidad);

    contenedor.css('display', '');
}

function eliminarAutocompletarMVC(that, identidad, panelContenedorID, panResultadosID, txtHackID) {
    // Cambiado por nuevo front
    //$(that).parent().remove();
    $(that).parent().parent().remove();

    var txtHack = $('#' + panelContenedorID).find('#' + txtHackID);
    txtHack.val(txtHack.val().replace(',' + identidad, ''));

    if (txtHack.val() == "") {
        var contenedor = $('#' + panelContenedorID).find('#' + panResultadosID);
        contenedor.css('display', 'none');
    }
}


/*-------------------------------------------------------------------------------------------------*/


function ActividadReciente_MostrarMas(pUrlLoadMoreActivity, pUrlLoadActions, pTipoActividad, pNumPeticion, idPanel, pComponentKey, pProfileKey)
{
	MostrarUpdateProgress();
	var datosPost = {
	    NumPeticion: pNumPeticion,
	    TypeActivity: pTipoActividad,
	    ComponentKey: pComponentKey,
	    ProfileKey: pProfileKey
	};

	$.post(pUrlLoadMoreActivity, datosPost, function (data) {
		$("#actividadReciente_" + idPanel).replaceWith(data);
		OcultarUpdateProgress();
		ObtenerAccionesListadoMVC(pUrlLoadActions);
	});
}

//function AgregarFiltroPerfil(pUrlPagina, pCallBack, idPanel) {
//    MostrarUpdateProgress();
//    var datosPost = {
//        callback: pCallBack
//    };

//    $.post(pUrlPagina, datosPost, function (data) {
//        $("#actividadReciente_" + idPanel).replaceWith(data);
//        OcultarUpdateProgress();
//    });
//}

//Métodos para las acciones de los listados de las búsquedas

var ObteniendoAcciones = false;
function ObtenerAccionesListadoMVC(pUrlPagina) {
    if (!ObteniendoAcciones) {
        ObteniendoAcciones = true;
        var resources = $('.resource-list .resource');
        var idPanelesAcciones = '';
        var numDoc = 0;
        resources.each(function () {
            var recurso = $(this);
            var accion = recurso.find('.group.acciones.noGridView');
            if (accion.length == 1 && accion.attr('id') != null) {
                accion.attr('id', accion.attr('id') + '_' + numDoc);
                idPanelesAcciones += accion.attr('id') + ',';
                numDoc++;
            }
            var accionesusuario = recurso.find('.group.accionesusuario.noGridView');
            if (accionesusuario.length == 1 && accionesusuario.attr('id') != null) {
                accionesusuario.attr('id', accionesusuario.attr('id') + '_' + numDoc);
                idPanelesAcciones += accionesusuario.attr('id') + ',';
                numDoc++;
            }
        });

        if (idPanelesAcciones != '') {
            try {
                var datosPost = {
                    callback: "CargarListaAccionesRecursos",
                    listaRecursos: idPanelesAcciones
                };

                $.post(pUrlPagina, datosPost, function (data) {
                    ObteniendoAcciones = false;
                    var paneles = idPanelesAcciones.split(',')

                    if (data != "") {
                        if (!Array.isArray(data)) {
                            // V5 -> Objeto de arrays -> Acceder al elemento 2 del objeto
                            for (var i in data.$values) {
                                if (data.$values[i].updateTargetId.indexOf("AccionesRecursoListado_") == 0) {
                                    var docID = data.$values[i].updateTargetId.substr("AccionesRecursoListado_".length, 36);
                                    var proyID = data.$values[i].updateTargetId.substr(("AccionesRecursoListado_" + docID).length, 36);

                                    var panel = "";

                                    $.each(idPanelesAcciones.split(','), function (i, val) {

                                        if (val.indexOf("accionesListado_" + docID) == 0) {
                                            if (val.indexOf(proyID) > 0) {
                                                panel = val;
                                            }
                                            else if (panel == "") {
                                                panel = val;
                                            }
                                        }
                                    });

                                    if (typeof (PintarAccionesRecursoPersonalizado) != "undefined") {
                                        PintarAccionesRecursoPersonalizado($('#' + panel), data.$values[i].html);
                                    }
                                    else {
                                        $('#' + panel).replaceWith(data.$values[i].html);
                                    }
                                }
                            }
                        } else {
                            for (var i in data) {
                                if (data[i].updateTargetId.indexOf("AccionesRecursoListado_") == 0) {
                                    var docID = data[i].updateTargetId.substr("AccionesRecursoListado_".length, 36);
                                    var proyID = data[i].updateTargetId.substr(("AccionesRecursoListado_" + docID).length, 36);

                                    var panel = "";

                                    $.each(idPanelesAcciones.split(','), function (i, val) {

                                        if (val.indexOf("accionesListado_" + docID) == 0) {
                                            if (val.indexOf(proyID) > 0) {
                                                panel = val;
                                            }
                                            else if (panel == "") {
                                                panel = val;
                                            }
                                        }
                                    });

                                    if (typeof (PintarAccionesRecursoPersonalizado) != "undefined") {
                                        PintarAccionesRecursoPersonalizado($('#' + panel), data[i].html);
                                    }
                                    else {
                                        $('#' + panel).replaceWith(data[i].html);
                                    }
                                }
                            }
                        }
                    }

                });
            }
            catch (ex) { }
        }
        else { ObteniendoAcciones = false; }
    }
}

function DesplegarImgMasMVC(pBoton, pPanel) {
	var $boton = $(pBoton);
	var $panel = $(pPanel);
	var $img = $boton.find('img');
	if ($img.length == 0) {
		$img = $boton;
	}
	var source = $img.attr('src');
	if ($img.attr('alt') == '+') {
		$panel.show();
		$img.attr({ alt: '-', src: source.replace('Mas', 'Menos') });
	} else if ($img.attr('alt') == '-') {
		$panel.hide()
		$img.attr({ alt: '+', src: source.replace('Menos', 'Mas') });
	}

	return false;
}

function DesplegarUserImgMasMVC(pBoton, pPanel) {
    const $boton = $(pBoton);
    const $panel = $(pPanel);

    if ($boton.text().trim() == "person_search") {
        $boton.text("person");
    } else {
        $boton.text("person_search");
    }

    $panel.toggleClass("d-none");
}

function paginadorGadget_Siguiente(urlPagina, gadgetID) {
    var botonSiguiente = $('#btnSiguiente_' + gadgetID)
    if (!botonSiguiente.hasClass("desactivado")) {
        var botonAnterior = $('#btnAnterior_' + gadgetID)
        botonAnterior.removeClass("desactivado");
        var paneles = $('#' + gadgetID).find('.contexto.resource-list');
        var panelActivo = $('#' + gadgetID).find('.contexto.resource-list.activo');

        var panelSiguiente = panelActivo.next();

        panelActivo.hide();
        panelSiguiente.show();
        panelActivo.removeClass("activo");
        panelSiguiente.addClass("activo");

        var ultimoPanel = panelSiguiente.next('.contexto.resource-list').length == 0 || panelSiguiente.next('.contexto.resource-list').is(":empty");
        if (ultimoPanel) {
            botonSiguiente.addClass("desactivado");
            if (!panelSiguiente.next('.contexto.resource-list').is(":empty")) {
                panelSiguiente.after("<div class=\"contexto resource-list\" style=\"display:none\"></div>");

                var datapost = {
                    callback: "paginadorGadget",
                    gadgetid: gadgetID,
                    numPagina: paneles.length + 1,
                }

                $.post(urlPagina, datapost, function (data) {
                    var htmlRecursos = "";
                    for (var i in data) {
                        if (data[i].updateTargetId.indexOf("FichaRecursoMini_") == 0) {
                            htmlRecursos += data[i].html;
                        }
                    }
                    if (htmlRecursos != "") {
                        botonSiguiente.removeClass("desactivado");
                        panelSiguiente.next().remove();
                        panelSiguiente.after("<div class=\"contexto resource-list\" style=\"display:none\">" + htmlRecursos + "</div>");
                    }
                    CompletadaCargaContextos();
                });
            }
        } else
        {
            CompletadaCargaContextos();
        }
    }
}

function paginadorGadget_Anterior(urlPagina, gadgetID) {
    var botonAnterior = $('#btnAnterior_' + gadgetID)
    if (!botonAnterior.hasClass("desactivado")) {
        var botonSiguiente = $('#btnSiguiente_' + gadgetID)
        botonSiguiente.removeClass("desactivado");
        var paneles = $('#' + gadgetID).find('.contexto.resource-list');
        var panelActivo = $('#' + gadgetID).find('.contexto.resource-list.activo');

        var panelAnterior = panelActivo.prev();

        panelActivo.hide();
        panelAnterior.show();
        panelActivo.removeClass("activo");
        panelAnterior.addClass("activo");

        var ultimoPanel = panelAnterior.prev('.contexto.resource-list').length == 0;
        if (ultimoPanel) {
            botonAnterior.addClass("desactivado");
        }
        CompletadaCargaContextos();
    }
}

function paginadorVinculados_Siguiente(urlPagina) {
    var botonSiguiente = $('#btnSiguiente_vinc');
    if (!botonSiguiente.hasClass("desactivado")) {
        var botonAnterior = $('#btnAnterior_vinc');
        botonAnterior.removeClass("desactivado");
        var paneles = $('#panVinculadosInt').find('.resource-list.vinculados');
        var panelActivo = $('#panVinculadosInt').find('.resource-list.vinculados.activo');

        var panelSiguiente = panelActivo.next();

        panelActivo.hide();
        panelSiguiente.show();
        panelActivo.removeClass("activo");
        panelSiguiente.addClass("activo");

        var ultimoPanel = panelSiguiente.next().length == 0 || panelSiguiente.next().is(":empty");
        if (ultimoPanel) {
            botonSiguiente.addClass("desactivado");
            if (!panelSiguiente.next().is(":empty")) {
                panelSiguiente.after("<div class=\"resource-list vinculados\" style=\"display:none\"></div>");
                var datapost = {
                    page: paneles.length + 1
                }
                $.post(urlPagina + "/load-linked-resources", datapost, function (data) {
                    var htmlNuevo = $('<div/>').html(data).find('.resource-list.vinculados');
                    if (htmlNuevo.length) {
                        botonSiguiente.removeClass("desactivado");
                        panelSiguiente.next().remove();
                        panelSiguiente.after("<div class=\" resource-list vinculados\" style=\"display:none\">" + htmlNuevo.html() + "</div>");
                    }
                    CompletadaCargaContextos();
                });
            }
        } else {
            CompletadaCargaContextos();
        }
    }
}

function paginadorVinculados_Anterior(urlPagina) {
    var botonAnterior = $('#btnAnterior_vinc');
    if (!botonAnterior.hasClass("desactivado")) {
        var botonSiguiente = $('#btnSiguiente_vinc')
        botonSiguiente.removeClass("desactivado");
        var paneles = $('#panVinculadosInt').find('.resource-list.vinculados');
        var panelActivo = $('#panVinculadosInt').find('.resource-list.vinculados.activo');

        var panelAnterior = panelActivo.prev();

        panelActivo.hide();
        panelAnterior.show();
        panelActivo.removeClass("activo");
        panelAnterior.addClass("activo");

        var ultimoPanel = panelAnterior.prev('.resource-list.vinculados').length == 0;
        if (ultimoPanel) {
            botonAnterior.addClass("desactivado");
        }
        CompletadaCargaContextos();
    }
}

function GnossPeticionAjax(url, parametros, traerJson, redirectActive = true)
{
    var defr = $.Deferred();

    var esFormData = parametros instanceof FormData;

    $.ajax({
        url: url,
        type: "POST",
        headers: {
            Accept: traerJson ? 'application/json' : '*/*'
        },
        processData: !esFormData,
        contentType: esFormData ? false : 'application/x-www-form-urlencoded',
        data: parametros,
        xhr: function () {
            var xhr = new window.XMLHttpRequest();
            // Handle progress
            //Upload progress
            xhr.upload.addEventListener("progress", function (evt) {
                if (evt.lengthComputable) {
                    var percentComplete = evt.loaded / evt.total;
                    //Do something with upload progress
                    defr.notify(Math.round(percentComplete * 100));
                }
            }, false);

            return xhr;
        }
    }).done(function (response) {
        if (response == null || response.Status == undefined) {
            defr.resolve(response);
        }
        if (response.Status == "NOLOGIN") {
            //Hacer una peticion a la web, que nos devuelva la url del servicio de login + token
            //Hacer la peticion al servicio de login a recuperar la sesion
            //Si no estamos conectados, mostrar un panel de login
            //Si estamos conectados, re-llamar a esta funcion
            defr.reject("invitado");
        }
        else if (response.Status == "OK") {
            if (response.UrlRedirect != null)
            {
                if(redirectActive){ location.href = response.UrlRedirect; }
                else{ defr.resolve(response.UrlRedirect); }
            }
            else if (response.Html != null) {
                defr.resolve(response.Html);
            }
            else {
                defr.resolve(response.Message);
            }
        }
        else if (response.Status == "Error") {
            defr.reject(response.Message);
        }
    }).fail(function (er) {
        //Comprobar el error
        var newtWorkError = er.readyState == 0;// && er.statusText == "error";
        if (newtWorkError) {
            defr.reject("NETWORKERROR");
        }
        else {
            defr.reject(er.statusText);
        }
    });

    return defr;
}

var comportamientoNetworkError = {
    intentarRedirigirFichaRecurso: function (url, funcion) {
        if (documentoID != null && documentoID != '') {
            url += "?documentoID=" + documentoID;
        }
        this.urlObtenerFicha = url;
        this.funcionEjecutar = funcion;
        this.cont = 0;
        var that = this;
        //setTimeout(function () { that.obtenerFichaRecurso(); }, 1000);
        that.obtenerFichaRecurso();
    },
    obtenerFichaRecurso: function () {
        var that = this;
        $.ajax({
            url: that.urlObtenerFicha,
            method: 'GET',
            async: false,
            success: function (data) {
                //la ficha existe, redirigir
                if (data != '') {
                    document.location.href = data;
                }
                else {
                    if (that.cont < 10) {
                        that.cont++;
                        setTimeout(function () { that.obtenerFichaRecurso(); }, 1000);
                    }
                    else {
                        //mostrar error
                        that.obtenerMensajeError();
                    }
                }
            },
            error: function (data) {
                //la ficha no existe
                if (data.readyState == 0 && that.cont < 10) {
                    that.cont++;
                    setTimeout(function () { that.obtenerFichaRecurso(); }, 1000);
                }
                else {
                    //mostrar error
                    that.obtenerMensajeError();
                }
            }
        });
    },
    obtenerMensajeError: function () {
        this.funcionEjecutar('Has perdido la conexión y no se ha podido recuperar el recurso. Comprueba tu conexión a internet y verifica que tus cambios se han guardado correctamente ');
    }
}

function guidGenerator() {
    var S4 = function () {
        return (((1 + Math.random()) * 0x10000) | 0).toString(16).substring(1);
    };
    return (S4() + S4() + "-" + S4() + "-" + S4() + "-" + S4() + "-" + S4() + S4() + S4());
}

String.prototype.EndsWith = function (suffix) {
    return this.indexOf(suffix, this.length - suffix.length) !== -1;
};

String.prototype.StartsWith = function (searchString, position) {
    position = position || 0;
    return this.lastIndexOf(searchString, position) === position;
};

//Añadimos esta funcion que antes la añadia .Net
Date.prototype.format = function (format) {
    var date = this,
        day = date.getDate(),
        month = date.getMonth() + 1,
        year = date.getFullYear(),
        hours = date.getHours(),
        minutes = date.getMinutes(),
        seconds = date.getSeconds();

    if (!format) {
        format = "MM/dd/yyyy";
    }

    format = format.replace("MM", month.toString().replace(/^(\d)$/, '0$1'));

    if (format.indexOf("yyyy") > -1) {
        format = format.replace("yyyy", year.toString());
    } else if (format.indexOf("yy") > -1) {
        format = format.replace("yy", year.toString().substr(2, 2));
    }

    format = format.replace("dd", day.toString().replace(/^(\d)$/, '0$1'));

    if (format.indexOf("t") > -1) {
        if (hours > 11) {
            format = format.replace("t", "pm");
        } else {
            format = format.replace("t", "am");
        }
    }

    if (format.indexOf("HH") > -1) {
        format = format.replace("HH", hours.toString().replace(/^(\d)$/, '0$1'));
    }

    if (format.indexOf("hh") > -1) {
        if (hours > 12) {
            hours -= 12;
        }

        if (hours === 0) {
            hours = 12;
        }
        format = format.replace("hh", hours.toString().replace(/^(\d)$/, '0$1'));
    }

    if (format.indexOf("mm") > -1) {
        format = format.replace("mm", minutes.toString().replace(/^(\d)$/, '0$1'));
    }

    if (format.indexOf("ss") > -1) {
        format = format.replace("ss", seconds.toString().replace(/^(\d)$/, '0$1'));
    }

    return format;
};

/*Tesauros*/
function MVCDesplegarTreeView(pImagen) {

    if (typeof MarcarDesplegarTreeView == "function")
    {
        MarcarDesplegarTreeView(pImagen);
    }

    var imagen = $(pImagen);
    if (pImagen.src.indexOf('verMas') > 0) {
        pImagen.src = pImagen.src.replace('verMas', 'verMenos');
    }
    else {
        pImagen.src = pImagen.src.replace('verMenos', 'verMas');
    }
    MVCDesplegarPanel($(pImagen).parent().children('.panHijos'));
}

function MVCDesplegarPanel(pPanel) {
    if (pPanel.css("display") == 'none') {
        pPanel.show();
    }
    else {
        pPanel.hide();
    }
    return false;
}

/**
 * Método para filtrar elementos. En este caso, en la lista de Categor�as, modo "Lista" de la ficha Recurso.
 * Al teclear en el input, filtrará (ocultará) los elementos que no correspondan con el texto de la búsqueda
 * @param {any} txt
 * @param {any} panDesplID
 */
function MVCFiltrarListaSelCat(txt, panDesplID) {
    var cadena = $(txt).val();
    // Eliminamos posibles tildes para búsqueda ampliada
    cadena = cadena.normalize('NFD').replace(/[\u0300-\u036f]/g, "");
    var itemsListado = $('#' + panDesplID).find('#divSelCatLista').children('div');
    itemsListado.show();
    itemsListado.each(function () {
        //var nombreCat = $(this).find('span label').text();
        // Cambia al ser nuevo front - Nuevo estilo de cada item de categorías        
        var nombreCat = $(this).find('div div label').text();
        // Eliminamos posibles tildes para búsqueda ampliada
        nombreCat = nombreCat.normalize('NFD').replace(/[\u0300-\u036f]/g, "");
        if (nombreCat.toLowerCase().indexOf(cadena.toLowerCase()) < 0) {
            $(this).hide();
        }
    });
}

/**
 * Método para filtrar elementos. En este caso, en la lista de Categorías, modo "Árbol" de la ficha Recurso.
 * Al teclear en el input, filtrará (ocultará) los elementos que no correspondan con el texto de la búsqueda
 * @param {any} txt
 * @param {any} panDesplID
 */
function MVCFiltrarListaSelCatArbol(txt, id, completion = undefined) {
    var cadena = $(txt).val();
    // Eliminamos posibles tildes para búsqueda ampliada
    cadena = cadena.normalize('NFD').replace(/[\u0300-\u036f]/g, "");
    var itemsListado = $('#' + id).find('.divTesArbol .categoria-wrap');
    itemsListado.show();
    if (cadena == '') {
        //$('.boton-desplegar').removeClass('mostrar-hijos');
    } else {
        itemsListado.each(function () {
            var boton = $(this).find('.boton-desplegar');
            boton.removeClass('mostrar-hijos');
            var nombreCat = $(this).find('.categoria label').text();
            // Eliminamos posibles tildes para búsqueda ampliada
            nombreCat = nombreCat.normalize('NFD').replace(/[\u0300-\u036f]/g, "");
            boton.trigger('click');
            if (nombreCat.toLowerCase().indexOf(cadena.toLowerCase()) < 0) {
                $(this).hide();
            }

            var categoriaHijo = $(this).find('.panHijos').children('.categoria-wrap');
            categoriaHijo.each(function () {                
                var nombreCatHijo = $(this).find('.categoria label').text().normalize('NFD').replace(/[\u0300-\u036f]/g, "");
                if (nombreCatHijo.toLowerCase().indexOf(cadena.toLowerCase()) < 0) {
                    $(this).hide();
                }
            });
        });
    }
    if (completion != undefined) {
        completion();
    }    
}

/**
 * Función que filtrará (dejará visibles u ocultará) items en base a la búsqueda introducida
 * @param {any} txt: Será el input text en formato jquery donde se introduce la cadena de búsqueda
 * @param {any} panelId: Id del panel donde se han de buscar los items
 * @param {any} classItem: Clase del item que será ocultado o mostrado según la búsqueda
 * @param {any} classItemWhereToFind: Clases donde se ha de buscar lo introducido en el txt
 */
function MVCFiltrarItems(txt, panelId, classItem, classItemWhereToFind) {
    // Cadena de texto a buscar (minúsculas)
    var cadena = $(txt).val().toLowerCase();
    // Eliminamos posibles tildes para búsqueda ampliada
    cadena = cadena.normalize('NFD').replace(/[\u0300-\u036f]/g, "");
    // Items a buscar
    var itemsListado = $('#' + panelId).find(`.${classItem}`);

    itemsListado.each(function () {
        // Texto de cada item a buscar para poder comparar
        var nombre = $(this).find(`.${classItemWhereToFind}`).text().toLowerCase();
        // Eliminamos posibles tildes para búsqueda ampliada
        nombre = nombre.normalize('NFD').replace(/[\u0300-\u036f]/g, "");
        // Filtrar -> Ocultar/Mostrar
        if (nombre.indexOf(cadena) < 0) {
            $(this).hide();
        } else {
            $(this).show();
        }
    });
}


/**
 * Marcar cada uno de los checkbox que haya en un determinado panel
 * @param {any} pCheck: checks que se buscarán (input)
 * @param {any} panDesplID: el panel donde se buscarán los checks
 * @param {any} hackedInputId: opcional. El input donde se añadirán los checks
 */
function MVCMarcarTodosElementosCat(pCheck, panDesplID, hackedInputId = undefined) {
    // items seleccionados
    var txtSeleccionados = "";

    var itemsListado = $('#' + panDesplID).find('#divSelCatLista').children('div');

    var marcarChecks = $(pCheck).is(':checked');

    // Input donde se guardarán los input seleccionados/desmarcados
    var inputTxtSeleccionados = '';

    // Observar un panel u otro
    if (hackedInputId == undefined) {
        inputTxtSeleccionados = $('#' + panDesplID).find('#txtSeleccionados');
    } else {
        inputTxtSeleccionados = $('#' + panDesplID).find(`#${hackedInputId}`);
    }


    itemsListado.each(function () {
        //var claseInput = $(this).find('span').attr('class');
        const claseInput = $(this).find('input').attr('data-item');

        /*
        $('#' + panDesplID).find('#divSelCatTesauro').find('span.' + claseInput + ' input').prop('checked', marcarChecks);
        $('#' + panDesplID).find('#divSelCatLista').find('span.' + claseInput + ' input').prop('checked', marcarChecks);
        */
        $('#' + panDesplID).find('#divSelCatTesauro').find(`[data-item=${claseInput}]`).prop('checked', $(pCheck).is(':checked'));
        $('#' + panDesplID).find('#divSelCatLista').find(`[data-item=${claseInput}]`).prop('checked', $(pCheck).is(':checked'));

        if (marcarChecks) {
            txtSeleccionados += claseInput + ",";
        }
    });

    //$('#' + panDesplID).find(inputTxtSeleccionados).val(txtSeleccionados);
    inputTxtSeleccionados.val(txtSeleccionados);
}

/**
 * Acci�n que se ejecuta cuando se selecciona un item de categor�a para ser seleccionado e introducido en un input vac�o para as� tener control sobre el elemento que se ha seleccionado.
 * @param {any} pCheck: Ser� el input check que se ha seleccionado.
 * @param {any} panDesplID: El id del panel donde se encontrar� el input vac�o.
 * @param {any} hackedInputId: El id del inputId que estar� oculto que se utilizar� para establecer opciones que puedan servir para mandar al servidor. Si no se pasa nada, se har� caso al panDesplID. En caso contrario, se acceder� al panDesplIDSecundario para buscar ese input
 */
function MVCMarcarElementoSelCat(pCheck, panDesplID, hackedInputId = undefined) {
    // Debido al nuevo Front - No se accede al padre sino al ID del propio Input
    //var claseInput = $(pCheck).parent().attr("class");
    var claseInput = $(pCheck).attr("data-item");

    var txtSeleccionados = '';

    // Observar un panel u otro
    if (hackedInputId == undefined) {
        txtSeleccionados = $('#' + panDesplID).find('#txtSeleccionados');
    } else {
        txtSeleccionados = $('#' + panDesplID).find('#' + hackedInputId);
    }

    if ($(pCheck).is(':checked')) {
        txtSeleccionados.val(txtSeleccionados.val() + claseInput + ',');
    }
    else {
        txtSeleccionados.val(txtSeleccionados.val().replace(claseInput + ',', ''));
    }

    /*
     * $('#' + panDesplID).find('#divSelCatTesauro').find('span.' + claseInput + ' input').prop('checked', $(pCheck).is(':checked'));
    $('#' + panDesplID).find('#divSelCatLista').find('span.' + claseInput + ' input').prop('checked', $(pCheck).is(':checked'));
    */

    $('#' + panDesplID).find('#divSelCatTesauro').find(`[data-item=${claseInput}]`).prop('checked', $(pCheck).is(':checked'));
    $('#' + panDesplID).find('#divSelCatLista').find(`[data-item=${claseInput}]`).prop('checked', $(pCheck).is(':checked'));

    MVCComprobarChecks(panDesplID);
}

/**
 * Método para comprobar si se han chequeado todos los checks
 * @param {any} panDesplID
 */
function MVCComprobarChecks(panDesplID) {
    //var itemsListado = $('#' + panDesplID).find('#divSelCatLista').find('div span input');         
    const itemsListado = $('#' + panDesplID).find('#divSelCatLista').children('div').find("input");

    //var numItemsChecked = $('#' + panDesplID).find('#divSelCatLista').find('div span input:checked').length;
    const numItemsChecked = $('#' + panDesplID).find('#divSelCatLista').children('div').find("input:checked").length;

    if (numItemsChecked == itemsListado.length) {
        $('#chkSeleccionarTodos').prop('checked', true);
    }
    else {
        $('#chkSeleccionarTodos').prop('checked', false);
    }
}
/*Fin Tesauros*/
var inicializadoSubirRecurso = false;

function InicializarSubirRecursoExt(urlPaginaSubir) {
    if (inicializadoSubirRecurso) { return; }

    $("#linkNota").click(function () {
        MostrarUpdateProgress();

        GnossPeticionAjax(urlPaginaSubir + '/selectresource', { TypeResourceSelected: 0 }, true).always(function () {
            OcultarUpdateProgress();
        });
    });

    $("#lbSiguienteURL").click(function () { validarUrlExt(urlPaginaSubir, false); });

    $("#lbSiguienteReferencia").click(function () { validarDocFisicoExt(urlPaginaSubir, false); });

    $("#lbSiguienteWiki").click(function () {
        var url = document.getElementById("txtArticuloWiki").value;

        if (url == '') {
            return false;
        }

        MostrarUpdateProgress();

        GnossPeticionAjax(urlPaginaSubir + '/selectresource', { TypeResourceSelected: 3, Link: url }, true).fail(function (data) {
            $('#pWikiError').remove();
            $('#divWiki fieldset').append('<div id="pWikiError" class="ko" style="display:block;"><p>' + data + '</p></div>');
        }).always(function () {
            OcultarUpdateProgress();
        });
    });

    $("#lbSiguienteArchivo").click(function () { validarDocAdjuntarExt(urlPaginaSubir, false, null); });

    inicializadoSubirRecurso = true;
}

function mostrarPanSubirRecurso(nombre) {

    if (typeof (InicioMostrarPanSubirRecurso) != "undefined") {
        InicioMostrarPanSubirRecurso(nombre);
    }

    if (document.getElementById('panEnlRep') != null) {
        $('#panEnlRep').remove();
    }

    if (document.getElementById("divArchivo") != null) {
        document.getElementById("divArchivo").style.display = "none";
    }
    if (document.getElementById("divReferenciaDoc") != null) {
        document.getElementById("divReferenciaDoc").style.display = "none";
    }
    if (document.getElementById("divURL") != null) {
        document.getElementById("divURL").style.display = "none";
    }
    if (document.getElementById("divBrightcove") != null) {
        document.getElementById("divBrightcove").style.display = "none";
    }
    if (document.getElementById("divTOP") != null) {
        document.getElementById("divTOP").style.display = "none";
    }
    if (document.getElementById("divWiki") != null) {
        document.getElementById("divWiki").style.display = "none";
    }

    switch (nombre) {
        case 'Archivo':
            document.getElementById("divArchivo").style.display = "block";
            break;
        case 'Referencia':
            document.getElementById("divReferenciaDoc").style.display = "block";
            break;
        case 'URL':
            document.getElementById("divURL").style.display = "block";
            break;
        case 'Brightcove':
            document.getElementById("divBrightcove").style.display = "block";
            var srcAux = $('#iframeBrightcove').attr('srcAux');
            var onloadAux = $('#iframeBrightcove').attr('onloadAux');
            if (srcAux != null && srcAux != "") {
                $('#iframeBrightcove').attr('src', srcAux);
                $('#iframeBrightcove').removeAttr('srcAux');
            }
            if (onloadAux != null && onloadAux != "") {
                $('#iframeBrightcove').attr('onload', onloadAux);
                $('#iframeBrightcove').removeAttr('onloadAux');
            }
            break;
        case 'TOP':
            document.getElementById("divTOP").style.display = "block";
            var srcAux = $('#iframeTOP').attr('srcAux');
            var onloadAux = $('#iframeTOP').attr('onloadAux');
            if (srcAux != null && srcAux != "") {
                $('#iframeTOP').attr('src', srcAux);
                $('#iframeTOP').removeAttr('srcAux');
            }
            if (onloadAux != null && onloadAux != "") {
                $('#iframeTOP').attr('onload', onloadAux);
                $('#iframeTOP').removeAttr('onloadAux');
            }
            break;
        case 'Wiki':
            document.getElementById("divWiki").style.display = "block";
            break;
    }
}

/**
 * /**
 * Acci�n que se ejecuta para comprobar que el Link adjunto es correcto. Es el paso previo que se realiza antes de poder crear un recurso de tipo "Enlace externo" 
 * @param {any} urlPaginaSubir: Url o enlace escrito por el usuario
 * @param {any} omitirCompRep
 */
function validarUrlExt(urlPaginaSubir, omitirCompRep) {
    try //Intentamos validar la url
    {
        var lblUrl = document.getElementById("lblIntroducirURL");
        var url = document.getElementById("txtURLDoc");

        // Panel donde se mostrar�n posibles errores en la subida del un recurso de tipo Enlace Externo (Nuevo Front)
        const panelResourceFileErrorMessage = $('#modal-add-resource-link-messages-wrapper .ko');
        // Vaciar el panel de posibles errores anteriores y ocultarlo
        panelResourceFileErrorMessage.empty().hide(); 

        var regexURL = /^((([hH][tT][tT][pP][sS]?|[fF][tT][pP])\:\/\/)?([\w\.\-]+(\:[\w\.\&%\$\-]+)*@)?((([^\s\(\)\<\>\\\"\.\[\]\,@;:]+)(\.[^\s\(\)\<\>\\\"\.\[\]\,@;:]+)*(\.[a-zA-Z]{2,4}))|((([01]?\d{1,2}|2[0-4]\d|25[0-5])\.){3}([01]?\d{1,2}|2[0-4]\d|25[0-5])))(\b\:(6553[0-5]|655[0-2]\d|65[0-4]\d{2}|6[0-4]\d{3}|[1-5]\d{4}|[1-9]\d{0,3}|0)\b)?((\/[^\/][\w\.\,\?\'\\\/\+&%\$#\=~_\-@:]*)*[^\.\,\?\"\'\(\)\[\]!;<>{}\s\x7F-\xFF])?)$/i;
        if (url.value.length > 0 && url.value.match(regexURL)) {
            MostrarUpdateProgress();

            GnossPeticionAjax(urlPaginaSubir + '/selectresource', { TypeResourceSelected: 1, Link: url.value, SkipRepeat: omitirCompRep }, true).done(function (data) {
                // No ocultarlo para reintentar de nuevo
                //$('#divURL').hide();
                // Cambio por nuevo Front
                //$('#liURL').append(data);                
                // A�adir el mensaje y mostrarlo
                panelResourceFileErrorMessage.append(data).show();                                

            }).fail(function (data) {
                document.getElementById("lblIntroducirURL").style.color = "Red";
            }).always(function () {
                OcultarUpdateProgress();
            });

            return true;
        }
        else {
            lblUrl.style.color = "Red";
            return false;
        }
    } catch (e) {
        //Error provocado porque no existe el elemento url (no es un recurso con url)
    }
    return true;
}


function validarDocFisicoExt(urlPaginaSubir, omitirCompRep) {
    try //Intentamos validar un documento físico
    {
        var doc = document.getElementById("txtUbicacionDoc");

        if (doc.value.length > 0) {
            MostrarUpdateProgress();

            GnossPeticionAjax(urlPaginaSubir + '/selectresource', { TypeResourceSelected: 2, Link: doc.value, SkipRepeat: omitirCompRep }, true).done(function (data) {
                $('#divReferenciaDoc').hide();
                $('#liReferenciaDoc').append(data);
            }).fail(function (data) {
                document.getElementById("lblDescribaUbic").style.color = "Red";
            }).always(function () {
                OcultarUpdateProgress();
            });

            return true;
        } else {
            document.getElementById("lblDescribaUbic").style.color = 'Red';
            return false;
        }
    } catch (e) {
        //Error provado porque no existe el elemento doc (no es un elemento físico)
    }
}

/**
 * Inicialización de Input de tipo "File" para que coja el nombre del fichero seleccionado (Bootstrap)
 * ---------------
 */
$(document).on('change', '.custom-file-input', function (event) {
    $(this).next('.custom-file-label').html(event.target.files[0].name);
});

/**
 * Acci�n que se ejecuta para comprobar que el fichero adjunto es correcto. Es el paso previo que se realiza antes de poder crear un recurso de tipo "Adjunto"
 * @param {any} urlPaginaSubir: P�gina a la que se redireccionar� para completar la creaci�n del recurso de tipo "Adjunto"
 * @param {Boolean} omitirCompRep
 * @param {Boolean} extraArchivo
 */
function validarDocAdjuntarExt(urlPaginaSubir, omitirCompRep, extraArchivo) {
    try //Intentamos validar que haya un documento para adjuntar
    {
        var lblDoc = document.getElementById("lblSelecionaUnDoc");
        var doc = document.getElementById("fuExaminar");
        // Panel donde se mostrar�n posibles errores en la subida del archivo (Nuevo Front)
        const panelResourceFileErrorMessage = $('#modal-add-resource-file-messages-wrapper .ko');
        // Vaciar el panel de posibles errores anteriores y ocultarlo
        panelResourceFileErrorMessage.empty().hide(); 

        if (omitirCompRep) {
            MostrarUpdateProgress();

            GnossPeticionAjax(urlPaginaSubir + '/selectresource', { TypeResourceSelected: 4, ExtraFile: extraArchivo, SkipRepeat: omitirCompRep }, true).fail(function (data) {
                document.getElementById("lblDescribaUbic").style.color = "Red";
            }).always(function () {
                OcultarUpdateProgress();
            });
        }
        else if (doc.value.length > 0) {
            var data = new FormData();
            var files = $("#fuExaminar").get(0).files;
            if (files.length > 0) {
                MostrarUpdateProgressTime(0);

                var bar = $('#progressBarArchivo .bar');
                var percent = $('#progressBarArchivo .percent');

                var percentVal = '0%';
                bar.width(percentVal);
                percent.html(percentVal);

                $('#lbSiguienteArchivo').hide();
                $('#progressBarArchivo').show();

                data.append("TypeResourceSelected", 4);
                data.append("File", files[0]);
                data.append("FileName", files[0].name);

                GnossPeticionAjax(urlPaginaSubir + '/selectresource', data, true).done(function (data) {
                    // No ocultar nada para reitentar el env�o o subida de un fichero
                    //$('#divArchivo').hide();
                    // Cambio por nuevo Front
                    // $('#liArchivo').append(data);
                    // A�ado el error y muestro el div
                    panelResourceFileErrorMessage.append(data).show();
                    // Mostrar de nuevo bot�n de "Siguiente" para reintentar env�o de fichero
                    $('#lbSiguienteArchivo').show();
                    
                }).fail(function (data) {
                    document.getElementById("lblSelecionaUnDoc").style.color = "Red";
                    $('#pArchError').remove();
                    var mensajeError = data;
                    if (data == "NETWORKERROR") {
                        mensajeError = 'Has perdido la conexión. Comprueba tu conexión a internet e intenta adjuntar el recurso de nuevo.';
                    }
                    // Cambiado por nuevo front
                    // $('#divArchivo fieldset').append('<div id="pArchError" class="ko" style="display:block;"><p>' + mensajeError + '</p></div>');
                    panelResourceFileErrorMessage.append('<p>' + mensajeError + '</p>').show();
                    $('#lbSiguienteArchivo').show();
                    $('#progressBarArchivo').hide();
                }).progress(function (progreso) {
                    var percentVal = progreso + '%';
                    bar.width(percentVal);
                    percent.html(percentVal);
                }).always(function () {
                    OcultarUpdateProgress();
                });
            }
            return true;
        } else {
            lblDoc.style.color = 'Red';
            return false;
        }
    } catch (e) {
        //Error provado porque no existe el elemento doc (no es un recurso nuevo archivo)
    }
}

function comprobarSubidaBrightcove() {
    var iframe = document.getElementById("iframeBrightcove");
    var doc;
    if (!window.opera && document.all && document.getElementById) {
        doc = iframe.contentWindow.document;
    } else if (document.getElementById) {
        doc = iframe.contentDocument;
    }
    try {
        if (doc.getElementById("lblVideoBrightcoveOK") != null) {
            MostrarUpdateProgress();
            location.href = URLVideo;
        }
        if (doc.getElementById("lblAudioBrightcoveOK") != null) {
            MostrarUpdateProgress();
            location.href = URLAudio;
        }
    } catch (error) { }
}

function comprobarSubidaTOP() {
    var iframe = document.getElementById("iframeTOP");
    var doc;
    if (!window.opera && document.all && document.getElementById) {
        doc = iframe.contentWindow.document;
    } else if (document.getElementById) {
        doc = iframe.contentDocument;
    }
    try {
        if (doc.getElementById("lblVideoTOPOK") != null) {
            MostrarUpdateProgress();
            location.href = URLVideo;
        }
        if (doc.getElementById("lblAudioTOPOK") != null) {
            MostrarUpdateProgress();
            location.href = URLAudio;
        }
    } catch (error) { }
}

// NOTIFICACIONES PUSH

if ('serviceWorker' in navigator) {
    window.addEventListener("load", () => {
        navigator.serviceWorker.register("/ServiceWorker.js");
    });
}
if ('serviceWorker' in navigator) {
    window.addEventListener("load", () => {
        navigator.serviceWorker.register("/ServiceWorker.js")
            .then((reg) => {
                if (Notification.permission === "granted") {
                    getSubscription(reg);
                } else if (Notification.permission === "denied") {
                    blockSubscription();
                } else {
                    requestNotificationAccess(reg);
                }
            });
    });
}

function requestNotificationAccess(reg) {
    Notification.requestPermission(function (status) {
        if (status == "granted") {
            getSubscription(reg);
        } else if (status == "denied") {
            blockSubscription();
        }
    });
}

function blockSubscription() {
    var urlPeticion = location.protocol + "//" + location.host + "/PeticionesAJAX/DesuscribirseNotificacionesPush";

    GnossPeticionAjax(
        urlPeticion,
        null,
        true
    ).done(function (data) {

    }).fail(function (data) {

    }).always(function () {

    });
}

function fillSubscribeFields(sub) {
    var endpoint = sub.endpoint;
    var p256dh = arrayBufferToBase64(sub.getKey("p256dh"));
    var auth = arrayBufferToBase64(sub.getKey("auth"));
    var urlPeticion = location.protocol + "//" + location.host + "/PeticionesAJAX/SuscribirseNotificacionesPush";

    var datapost =
    {
        pEndpoint: endpoint,
        pP256dh: p256dh,
        pAuth: auth
    };

    GnossPeticionAjax(
        urlPeticion,
        datapost,
        true
    ).done(function (data) {

    }).fail(function (data) {

    }).always(function () {

    });
}

function arrayBufferToBase64(buffer) {
    var binary = '';
    var bytes = new Uint8Array(buffer);
    var len = bytes.byteLength;
    for (var i = 0; i < len; i++) {
        binary += String.fromCharCode(bytes[i]);
    }
    return window.btoa(binary);
}
// FIN NOTIFICACIONES PUSH