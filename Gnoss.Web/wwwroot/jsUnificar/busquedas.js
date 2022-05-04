var estamosFiltrando = false;

window.onpopstate = function (e) {
    if (window.location.href.indexOf('?') != -1 || window.location.href.indexOf('#') == -1) {
        if (estamosFiltrando) {
            FiltrarPorFacetas(ObtenerHash2());
            e.preventDefault();
        }
        estamosFiltrando = true;
    }
    estamosFiltrando = true;
};

var enlazarJavascriptFacetas = false;

function enlazarFiltrosBusqueda() {
    // Cambiado por nuevo Front
    /*$('.limpiarfiltros')
    .unbind()
    .click(function (e) {
        LimpiarFiltros();
        e.preventDefault();
    });*/

    // Permitir borrar filtros de búsqueda
    $("#divFiltros")
    .unbind()
    .on('click', '.limpiarfiltros', function (event) {
            LimpiarFiltros();
            event.preventDefault();
    });
    $("#panFiltros")
        .unbind()
        .on('click', '.limpiarfiltros', function (event) {
            LimpiarFiltros();
            event.preventDefault();
    });
          
    $('.panelOrdenContenedor select.filtro')
    .unbind()
        .change(function (e) {            
        AgregarFiltro('ordenarPor', $(this).val(), true);
    });

    // Configurar la selecci�n de ordenaci�n de los resultados al pulsar en "Ordenado por"    
    $("#panel-orderBy a.item-dropdown")
        // En ordenación, no mostraba el icono seleccionado ya que lo "desmontaba".
        //.unbind()
        .click(function (e) {                    
            var orderBy = $(this).attr("data-orderBy");
            var filtroOrder = $(this).attr("data-order");
            const concatFilterAndOrder = orderBy + "|" + filtroOrder.split("|")[1];
            AgregarFiltro('ordenarPor', concatFilterAndOrder, true);
    });

    // Orden Ascedente o Descedente
    $('#panel-orderAscDesc a.item-dropdown')
        .click(function (e) {
            var filtro = $(this).attr("data-order");
            AgregarFiltro(filtro.split('|')[0], filtro.split('|')[1], false);
            //e.preventDefault();
        });

    $('.panelOrdenContenedor a.filtroV2')
    .unbind()
    .click(function (e) {
        var filtro = $(this).attr("name");
        AgregarFiltro(filtro.split('-')[0], filtro.split('-')[1], false);
        e.preventDefault();
    });
    $('.paginadorResultados a.filtro')
    .unbind()
    .click(function (e) {
        var filtro = $(this).attr("name");
        AgregarFiltro(filtro.split('|')[0], filtro.split('|')[1], false);

        if (typeof searchAnalitics != 'undefined') {
            searchAnalitics.pageSearch(filtro.split('|')[1]);
        }
        e.preventDefault();
    });
}

function enlazarFacetasBusqueda() {
    $('.facetedSearchBox .filtroFaceta')
	.unbind()
	.keydown(function (event) {
	    if ($(this).val().indexOf('|') > -1) {
	        $(this).val($(this).val().replace(/\|/g, ''));
	    };

	    if (event.which || event.keyCode) {
	        if ((event.which == 13) || (event.keyCode == 13)) {
	            return false;
	        }
	    } else {
	        return true;
	    };
	});

    var desde = '';
    var hasta = '';
    if ($('.facetedSearchBox .searchButton').parents('.facetedSearchBox').find('input.hasDatepicker').length > 0) {
        desde = $('.facetedSearchBox .searchButton').parents('.facetedSearchBox').find('input.hasDatepicker')[0].value;
        hasta = $('.facetedSearchBox .searchButton').parents('.facetedSearchBox').find('input.hasDatepicker')[1].value;
    }

    $('.facetedSearchBox .searchButton')
	.unbind()
	.click(function (event) {
	    if ($(this).parents('.facetedSearchBox').find('.filtroFaceta').length == 1) {
	        if ($(this).parents('.facetedSearchBox').find('.filtroFacetaTesauroSemantico').length == 1 && $(this).parents('.facetedSearchBox').find('.filtroFaceta').val().indexOf('@' + $('input.inpt_Idioma').val()) == -1) {
	            AgregarFaceta($(this).parents('.facetedSearchBox').find('.filtroFaceta').attr('name') + '=' + $(this).parents('.facetedSearchBox').find('.filtroFaceta').val() + '@' + $('input.inpt_Idioma').val());
	        } else {
	            AgregarFaceta($(this).parents('.facetedSearchBox').find('.filtroFaceta').attr('name') + '=' + $(this).parents('.facetedSearchBox').find('.filtroFaceta').val());
	        }
	    } else {
	        var filtroBusqueda = $(this).attr('name');
	        var fechaDesde = $(this).parents('.facetedSearchBox').find('input')[0];
	        var fechaHasta = $(this).parents('.facetedSearchBox').find('input')[1];
	        var formatoFecha = false;

	        if (typeof ($(this).parents('.facetedSearchBox').find('input.hasDatepicker')[0]) != 'undefined') {
	            formatoFecha = true;
	        }

	        if (desde == '') {
	            desde = $('input.inpt_Desde').val();
	        }
	        if (hasta == '') {
	            hasta = $('input.inpt_Hasta').val();
	        }

	        var filtro = ObtenerFiltroRango(fechaDesde, desde, fechaHasta, hasta, formatoFecha);

	        if (filtro != '-') {
	            AgregarFaceta(filtroBusqueda + '=' + filtro);
	        }
	    }
	    return false;
	});

    $('.facetedSearch a.faceta')
	.unbind()
	.click(function (e) {
	    AgregarFaceta($(this).attr("name"));
        // Quitar el panel de filtrado para móvil para visualizar resultados correctamente
        $(body).removeClass("facetas-abiertas");
	    e.preventDefault();
	});
    $('.facetedSearch input.faceta')
	.unbind()
	.click(function (e) {
	    AgregarFaceta($(this).attr("name"));
	    e.preventDefault();
	});
    $('.facetedSearch a.faceta.grupo')
	.unbind()
	.click(function (e) {
	    AgregarFacetaGrupo($(this).attr("name"));
	    e.preventDefault();
	});

    $('#descargarRDF')
    .unbind()
	.click(function (e) {
	    var filtros = ObtenerHash2();
	    var url = document.location.href;

	    if (url.indexOf('?') != -1) {
	        url = url.substring(0, url.indexOf('?'));
	    }

	    var filtroRdf = '?rdf';
	    if (filtros != '') {
	        filtros = '?' + filtros;
	        filtroRdf = '&rdf';
	    }

	    url = url + filtros + filtroRdf;
	    $('#descargarRDF').prop('href', url);
	    eval($('#descargarRDF').href);
	});
}

function enlazarFacetasNoBusqueda() {
    $('.facetedSearchBox .filtroFaceta')
	.unbind()
	.keydown(function (event) {
	    if ($(this).val().indexOf('|') > -1) {
	        $(this).val($(this).val().replace(/\|/g, ''));
	    };
	    if (event.which || event.keyCode) {
	        if ((event.which == 13) || (event.keyCode == 13)) {
	            return false;
	        }
	    } else {
	        return true;
	    };
	});

    $('.facetedSearchBox .searchButton')
	.unbind()
	.click(function (event) {
	    event.preventDefault();
	    var urlRedirect = $(this).attr('href') + '?' + $(this).parent().find('.filtroFaceta').attr('name') + '=' + $(this).parent().find('.filtroFaceta').val();
	    window.location.href = urlRedirect;
	});
}

var cargarFacetas = true;

function AgregarFiltro(tipoFiltro, filtro, reiniciarPag) {
    estamosFiltrando = true;
    cargarFacetas = false;
    var filtros = ObtenerHash2();

    var tipoOrden = "";
	if (tipoFiltro ==  "ordenarPor" && filtro.split("|").length>1)
	{
        tipoOrden = filtro.split("|")[1];
        filtro = filtro.split("|")[0];
    }

    //Si el filtro ya existe, cambiamos el valor del filtro
    if (tipoFiltro == "ordenarPor" && filtros.indexOf(tipoFiltro + '=' + filtro) == 0 || filtros.indexOf('&' + tipoFiltro + '=' + filtro) > 0) {
        var tipoFiltroOrden = "orden";
        var filtroOrden = "asc";

	    if (filtros.indexOf(tipoFiltroOrden + '=') == 0 || filtros.indexOf('&' + tipoFiltroOrden + '=') > 0) {
            if (filtros.indexOf(tipoFiltroOrden + '=' + filtroOrden) == 0 || filtros.indexOf('&' + tipoFiltroOrden + '=' + filtroOrden) > 0) {
		  
	            filtroOrden = "desc";
	        }
            if (tipoOrden != "") {
		  
                filtroOrden = tipoOrden;
            }

            var textoAux = filtros.substring(filtros.indexOf(tipoFiltroOrden + '='));
            if (textoAux.indexOf('&') > -1) {
                textoAux = textoAux.substring(0, textoAux.indexOf('&'));
            }
            filtros = filtros.replace(textoAux, tipoFiltroOrden + '=' + filtroOrden);
        }
        else {
            if (filtros.length > 0) { filtros += '&'; }
            filtros += tipoFiltroOrden + '=' + filtroOrden;
        }
    }
    else if (filtros.indexOf(tipoFiltro + '=') == 0 || filtros.indexOf('&' + tipoFiltro + '=') > 0) {
        var textoAux = filtros.substring(filtros.indexOf(tipoFiltro + '='));
        if (textoAux.indexOf('&') > -1) {
            textoAux = textoAux.substring(0, textoAux.indexOf('&'));
        }
        filtros = filtros.replace(textoAux, tipoFiltro + '=' + filtro);

        //Si el filtro es un ordenar por y viene con asc o desc, lo ponemos
        if (tipoFiltro == "ordenarPor" && tipoOrden != "") {
            var textoAux2 = filtros.substring(filtros.indexOf('orden='));
            if (textoAux2.indexOf('&') > -1) {
                textoAux2 = textoAux2.substring(0, textoAux2.indexOf('&'));
            }
            filtros = filtros.replace(textoAux2, 'orden=' + tipoOrden);
        }
    }
    else {
        if (filtros.length > 0) { filtros += '&'; }
        filtros += tipoFiltro + '=' + filtro;

        //Si el filtro es un ordenar por y viene con asc o desc, lo ponemos
        if (tipoFiltro == "ordenarPor" && tipoOrden != "") {
            filtros += '&orden=' + tipoOrden;
        }
    }

    if (reiniciarPag == true) {
        var pagina = 'pagina'
        if (filtros.indexOf(pagina) > -1) {
            if (filtros.indexOf(pagina + '&') > -1) {
                textoAux = filtros.substring(filtros.indexOf(pagina + '&'));
            }
            else if (filtros.indexOf('&' + pagina) > -1) {
                textoAux = filtros.substring(filtros.indexOf('&' + pagina));
            }
            else {
                textoAux = filtros.substring(filtros.indexOf(pagina));
            }

            filtros = filtros.replace(textoAux, '');
        }
    }

    history.pushState('', 'New URL: ' + filtros, '?' + filtros);
    FiltrarPorFacetas(ObtenerHash2());
}

function AgregarFiltroAutocompletar(claveFaceta, filtro) {
    var url = document.location;
    var separador = '?';
    if (url.indexOf("?") != -1) {
        separador = '&';
    }
    document.location = url + separador + claveFaceta + '=' + filtro;
}

var clickEnFaceta = false;

function AgregarFaceta(faceta) {
    faceta = faceta.replace(/%22/g, '"');
    estamosFiltrando = true;
    //var filtros = ObtenerHash2().replace(/%20/g, ' ');
    var filtros = ObtenerHash2();
    filtros = replaceAll(filtros, '%26', '---AMPERSAND---');
    filtros = decodeURIComponent(filtros);
    filtros = replaceAll(filtros, '---AMPERSAND---', '%26');
	


    var esFacetaTesSem = false;

    if (faceta.indexOf('|TesSem') != -1) {
        esFacetaTesSem = true;
        faceta = faceta.replace('|TesSem', '');
    }

    var eliminarFacetasDeGrupo = '';
    if (faceta.indexOf("rdf:type=") != -1 && filtros.indexOf(faceta) != -1) {
        //Si faceta es RDF:type y filtros la contiene, hay que eliminar las las que empiezen por el tipo+;
        eliminarFacetasDeGrupo = faceta.substring(faceta.indexOf("rdf:type=") + 9) + ";";
    }

    var filtrosArray = filtros.split('&');
    filtros = '';

    var tempNamesPace = '';
    if (faceta.indexOf('|replace') != -1) {
        tempNamesPace = faceta.substring(0, faceta.indexOf('='));
        faceta = faceta.replace('|replace', '');
    }

    var facetaDecode = decodeURIComponent(faceta);
    var contieneFiltro = false;

    for (var i = 0; i < filtrosArray.length; i++) {
        if (filtrosArray[i] != '' && filtrosArray[i].indexOf('pagina=') == -1) {
            if (eliminarFacetasDeGrupo == '' || filtrosArray[i].indexOf(eliminarFacetasDeGrupo) == -1) {
                if (tempNamesPace == '' || (tempNamesPace != '' && filtrosArray[i].indexOf(tempNamesPace) == -1)) {
                    filtros += filtrosArray[i] + '&';
                }
            }
        }

        if (filtrosArray[i] != '' && (filtrosArray[i] == faceta || filtrosArray[i] == facetaDecode)) {
            contieneFiltro = true;
        }
    }

    if (filtros != '') {
        filtros = filtros.substring(0, filtros.length - 1);
    }
    if (faceta.indexOf('search=') == 0) {
	 
        $('h1 span#filtroInicio').remove();
    }

    if (typeof (filtroDePag) != 'undefined' && filtroDePag.indexOf(faceta) != -1) {
        var url = document.location.href;
        //var filtros = '';

        if (filtros != '') {
            filtros = '?' + filtros.replace(/ /g, '%20');
            //filtros = '?' + encodeURIComponent(filtros);
        }

        if (url.indexOf('?') != -1) {
            //filtros = url.substring(url.indexOf('?'));
            url = url.substring(0, url.indexOf('?'));
        }

        if (url.substring(url.length - 1) == '/') {
            url = url.substring(0, (url.length - 1));
        }

        //Quito los dos ultimos trozos:
        url = url.substring(0, url.lastIndexOf('/'));
        url = url.substring(0, url.lastIndexOf('/'));

        if (filtroDePag.indexOf('skos:ConceptID') != -1) {
            var busAvazConCat = false;

            if (typeof (textoCategoria) != 'undefined') {
                //busAvazConCat = (url.indexOf('/' + textoCategoria) == (url.length - textoCategoria.length - 1));
                if (url.indexOf(textoComunidad + '/') != -1) {
                    var trozosUrl = url.substring(url.indexOf(textoComunidad + '/')).split('/');
                    busAvazConCat = (trozosUrl[2] == textoCategoria);
                }
            }

            url = url.substring(0, url.lastIndexOf('/'));

            if (busAvazConCat) {
                url += '/' + textoBusqAvaz;
            }
        }


        MostrarUpdateProgress();

        document.location = url + filtros;
        return;
    }
    else if (!contieneFiltro) {
        //Si no existe el filtro, lo a?adimos
        if (filtros.length > 0) { filtros += '&'; }
        filtros += faceta;

        if (typeof searchAnalitics != 'undefined') {
            searchAnalitics.facetsSearchAdd(faceta);
        }
    }
    else {
        filtros = '';

        for (var i = 0; i < filtrosArray.length; i++) {
            if (filtrosArray[i] != '' && filtrosArray[i] != faceta && filtrosArray[i] != facetaDecode) {
            //if (filtrosArray[i] != '' && (filtrosArray[i].indexOf(faceta) == -1 || filtrosArray[i].indexOf(facetaDecode)) == -1)) {
                filtros += filtrosArray[i] + '&';
            }
        }

        if (filtros != '') {
            filtros = filtros.substring(0, filtros.length - 1);
        }

        if (!esFacetaTesSem && typeof searchAnalitics != 'undefined') {
            searchAnalitics.facetsSearchRemove(faceta);
        }
    }

    filtros = filtros.replace(/&&/g, '&');
    if (filtros.indexOf('&') == 0) {
        filtros = filtros.substr(1, filtros.length);
    }
    if (filtros[filtros.length - 1] == '&') {
        filtros = filtros.substr(0, filtros.length - 1);
    }

    filtros = filtros.replace(/\\\'/g, '\'');
    filtros = filtros.replace('|', '%7C');

    history.pushState('', 'New URL: ' + filtros, '?' + filtros);
    FiltrarPorFacetas(ObtenerHash2());
    EscribirUrlForm(filtros);
}

function EscribirUrlForm(filtros) {
    var accion = $('#aspnetForm').attr('action');
    if (accion != undefined) {
        if (accion.indexOf('?') != -1) {
            accion = accion.substring(0, accion.indexOf('?'));
        }
        accion += '?' + filtros;
        $('#aspnetForm').attr('action', accion);
    }
}

function AgregarFacetaGrupo(faceta) {
 
    estamosFiltrando = true;
    var filtros = ObtenerHash2();
    filtros = replaceAll(filtros, '%26', '---AMPERSAND---');
    filtros = decodeURIComponent(filtros);
    filtros = replaceAll(filtros, '---AMPERSAND---', '%26');

    var agregar = false;
    if (filtros.indexOf("default;" + faceta) == -1) {
        agregar = true;
    }

    var filtrosArray = filtros.split('&');
    filtros = '';

    for (var i = 0; i < filtrosArray.length; i++) {
        if (filtrosArray[i] != '' && filtrosArray[i].indexOf('pagina=') == -1) {
            //Quita los default
            filtros += filtrosArray[i].replace("default;rdf:type", "rdf:type") + '&';
        }
    }

    if (agregar) {
	 
        if (filtros.indexOf(faceta) != -1) {
            //Si lo contiene lo reemplaza
            filtros = filtros.replace(faceta, "default;" + faceta);
        } else {
            //Si no lo contiene lo a?ade
            filtros += "default;" + faceta;
        }
    }

    filtros = filtros.replace(/&&/g, '&');
    if (filtros.indexOf('&') == 0) {
        filtros = filtros.substr(1, filtros.length);
    }
    if (filtros[filtros.length - 1] == '&') {
        filtros = filtros.substr(0, filtros.length - 1);
    }

    filtros = filtros.replace(/\\\'/g, '\'');

    history.pushState('', 'New URL: ' + filtros, '?' + filtros);
    FiltrarPorFacetas(ObtenerHash2());
}

function LimpiarFiltros() {
    if (typeof (filtroDePag) != 'undefined' && filtroDePag != '') {
        var url = document.location.href;

        if ($('.limpiarfiltros') != undefined && $('.limpiarfiltros').attr('href') != undefined && $('.limpiarfiltros').attr('href') != '') {
            url = $('.limpiarfiltros').attr('href');
        }
        else {
            if (url.indexOf('?') != -1) {
                url = url.substring(0, url.indexOf('?'));
            }

            if (url.substring(url.length - 1) == '/') {
                url = url.substring(0, (url.length - 1));
            }

            //Quito los dos ultimos trozos:
            url = url.substring(0, url.lastIndexOf('/'));
            url = url.substring(0, url.lastIndexOf('/'));

            //if (filtroDePag.indexOf('skos:ConceptID') != -1) {
            //    url = url.substring(0, url.lastIndexOf('/'));
            //}
            if (filtroDePag.indexOf('skos:ConceptID') != -1) {
                var busAvazConCat = false;

                if (typeof (textoCategoria) != 'undefined') {
                    //busAvazConCat = (url.indexOf('/' + textoCategoria) == (url.length - textoCategoria.length - 1));
                    if (url.indexOf(textoComunidad + '/') != -1) {
                        var trozosUrl = url.substring(url.indexOf(textoComunidad + '/')).split('/');
                        busAvazConCat = (trozosUrl[2] == textoCategoria);
                    }
                }

                url = url.substring(0, url.lastIndexOf('/'));

                if (busAvazConCat) {
                    url += '/' + textoBusqAvaz;
                }
            }
        }

        MostrarUpdateProgress();

        document.location = url;
        return;
    }
    $('h1 span#filtroInicio').remove();

    history.pushState('', 'New URL: ', '?');
    FiltrarPorFacetas('');
}

function ObtenerHash2() {
    var url = window.location.href;

    if (url.EndsWith("&rss") || url.EndsWith("?rss") || url.EndsWith("&rdf") || url.EndsWith("?rdf")) {
        url = url.substr(0, url.length - 4);
    }
    else if (url.StartsWith("rss&") || url.StartsWith("rdf&")) {
        url = url.substr(4);
    }

    if (url.indexOf('?') > 1) {
        return url.substr(url.indexOf('?') + 1);
    }
    return "";
}

var primeraCargaDeFacetas = true;

function FiltrarPorFacetas(filtro) {
    if (typeof FiltrarBandejaMensajes != "undefined") {
        return FiltrarBandejaMensajes(filtro);
    }
    else if (typeof FiltrarPerfilUsuario != "undefined") {
        return FiltrarPerfilUsuario(filtro);
    }
    else if (typeof ProcesarFiltro != "undefined") {
        return ProcesarFiltro(filtro);
    }
    return FiltrarPorFacetasGenerico(filtro);
}

function FiltrarPorFacetasGenerico(filtro) {
    filtro = filtro.replace(/&/g, '|');

    if (typeof (filtroDePag) != 'undefined' && filtroDePag != '') {
        if (filtro != '') {
            filtro = filtroDePag + '|' + filtro;
        }
        else {
            filtro = filtroDePag;
        }
    }
    //Si hay orden por relevancia pero no hay filtro search, quito el orden para que salga el orden por defecto
    //    if(QuitarOrdenReleavanciaSinSearch(filtro))
    //    {
    //        return false;
    //    }
    filtrosPeticionActual = filtro;

    var rdf = false;
    if (filtro.indexOf('?rdf') != -1 && ((filtro.indexOf('?rdf') + 4) == filtro.length)) {
        filtro = filtro.substring(0, filtro.length - 4);
        document.location.hash = document.location.hash.substring(0, document.location.hash.length - 4);
        rdf = true;
    }

    enlazarJavascriptFacetas = true;

    var arg = filtro;

    /*
    var vistaMapa = ($('li.mapView').attr('class') == "mapView activeView");
    var vistaChart = ($('.chartView').attr('class') == "chartView activeView");
    */

    var vistaMapa = $('li.mapView').hasClass('activeView');
    var vistaChart = $('.chartView').hasClass('activeView');

    if (!primeraCargaDeFacetas && !vistaMapa) {
        MostrarUpdateProgress();
    }

    var parametrosFacetas = 'ObtenerResultados';

    var gruposPorTipo = $('#facetedSearch.facetedSearch .listadoAgrupado ').length>0;

    if (cargarFacetas && !gruposPorTipo) {
        if (typeof panFacetas != "undefined" && panFacetas != "" && $('#' + panFacetas).length > 0 && !primeraCargaDeFacetas && !gruposPorTipo) {
            $('#' + panFacetas).html('')
        }
        if (numResultadosBusq != "" && $('#' + numResultadosBusq).length > 0 && !primeraCargaDeFacetas) {
            $('#' + numResultadosBusq).html('')
            $('#' + numResultadosBusq).css('display', 'none');
        }
        if (!clickEnFaceta && panFiltrosPulgarcito != "" && $('#' + panFiltrosPulgarcito).length > 0 && !primeraCargaDeFacetas) {
            $('#' + panFiltrosPulgarcito).html('')
        }
    }

    if (!vistaMapa) {
        SubirPagina();
    }

    if (typeof idNavegadorBusqueda != "undefined") {
        $('#' + idNavegadorBusqueda).html('');
        $('#' + idNavegadorBusqueda).css('display', 'none');
    }

    if (!vistaMapa && !primeraCargaDeFacetas) {
        // Vaciar el contenido actual de resultados - Nuevo Front
        // document.getElementById(updResultados).innerHTML = '';
        $(`#${updResultados}`).find(".resource-list-wrap").html('');
        $('#' + updResultados).attr('style', '');
    }

    clickEnFaceta = false;
    var primeraCarga = false;

    if (filtro.length > 1 || document.location.href.indexOf('/tag/') > -1 || (filtroContexto != null && filtroContexto != '')) {
        parametrosFacetas = 'AgregarFacetas|' + arg;
        var parametrosResultados = 'ObtenerResultados|' + arg;
        if (!cargarFacetas) {
            var parametrosResultados = 'ObtenerResultadosSinFacetas|' + arg;
        }
        //cargarFacetas
        var displayNone = '';
        document.getElementById('query').value = parametrosFacetas;
        if (HayFiltrosActivos(filtro) && (tipoBusqeda != 12 || filtro.indexOf("=") != -1)) {
            $('#' + divFiltros).css('display', '');
            $('#' + divFiltros).css('padding-top', '0px !important');
            //$('#' + divFiltros).css('margin-top', '10px');
        }
        var pLimpiarFilt = $('p', $('#' + divFiltros)[0]);

        if (pLimpiarFilt != null && pLimpiarFilt.length > 0) {
            if (!(filtro.length > 1 || document.location.href.indexOf('/tag/') > -1)) {
                pLimpiarFilt[0].style.display = 'none';
            }
            else {
                pLimpiarFilt[0].style.display = '';
            }
        }
    }
    else {
        primeraCarga = true;
        $('#' + divFiltros).css('display', 'none');
        $('#' + divFiltros).css('padding-top', '0px !important');
        //$('#' + divFiltros).css('margin-top', '10px');
    }

    if (rdf) {
        eval(document.getElementById('rdfHack').href);
    }

    var tokenAfinidad = guidGenerator();

    if (vistaMapa || !primeraCargaDeFacetas) {
        MontarResultados(filtro, primeraCarga, 1, '#' + panResultados, tokenAfinidad);
    }

    if (panFacetas != "" && (cargarFacetas || document.getElementById(panFacetas).innerHTML == '')) {
        var inicioFacetas = 1;

        MontarFacetas(filtro, primeraCarga, inicioFacetas, '#' + panFacetas, null, tokenAfinidad);
    }

    primeraCargaDeFacetas = false;
    cargarFacetas = true;

    var txtBusquedaInt = $('.aaCabecera .searchGroup .text')
    var textoSearch = 'search=';
    if ((filtro.indexOf(textoSearch) > -1) && txtBusquedaInt.length > 0) {
        var filtroSearch = filtro.substring(filtro.indexOf(textoSearch) + textoSearch.length);
        if (filtroSearch.indexOf('|') > -1) {
            filtroSearch = filtroSearch.substring(0, filtroSearch.indexOf('|'));
        }

        txtBusquedaInt.val(decodeURIComponent(filtroSearch));
        txtBusquedaInt.blur();
    }
    CambiarOrden(filtro);
    return false;
}

function CambiarOrden(hash) {
    if ($('.panelOrdenContenedor select.filtro').length > 0) {
        var controlOrden = $('.panelOrdenContenedor select.filtro');

        var ordenarPor = 'ordenarPor=';
        var search = (hash.indexOf('search=') != -1);
        var noOrden = (hash.indexOf(ordenarPor) == -1);

        //controlOrden.find('option:selected').removeAttr('selected');
        $('.panelOrdenContenedor select.filtro option[selected]').removeAttr('selected');

        if (noOrden) {
            //Si no hay orden, compruebo cu?l es la opci?n de orden que debe estar activa
            var opcion = controlOrden.find('option[value="' + ordenPorDefecto + '"]');
            var opcionSearch = controlOrden.find('option[value="' + ordenEnSearch + '"]');
            var ordenSearch = false;
            var ordenSearchPorDefecto = (ordenEnSearch == ordenPorDefecto);

            if (search) {
                //Si viene el par?metro search, pongo a seleccionado el orden en search
                opcion = opcionSearch;
                ordenSearch = true;
            }

            if (!ordenSearchPorDefecto) {
                //Si el orden por defecto no es el mismo que el orden en search, no ser? visible salvo que est? seleccionado
                if (!ordenSearch) {
                    opcionSearch.attr('style', 'display:none');
                }
                else {
                    opcionSearch.removeAttr('style');
                }
            }
        }
        else if (noOrden != -1) {
            //Obtengo el orden actual para seleccionar esa opci?n en el combo 
            var orden = hash.substring(hash.indexOf(ordenarPor) + ordenarPor.length);
            var indiceBarraFin = orden.indexOf('|');
            if (indiceBarraFin != -1) {
                orden = orden.substring(0, indiceBarraFin);
            }

            var opcion = controlOrden.find('option[value="' + orden + '"]');
        }

        if (opcion.length > 0) {
            //Marco el orden actual como seleccionado
            opcion.attr('selected', 'selected');

            if (opcion.length > 0 && opcion.prevObject.length > 0) {
                opcion.prevObject[0].selectedIndex = opcion[0].index;
            }
        }
    }
}

function MontarFechaCliente() {
    if (typeof (diffHoras) == 'undefined' || diffHoras == null) {
        var fechaServidor = new Date($('#inpt_serverTime').val());
        $('p.publicacion strong').each(function (index) {
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

var bool_usarMasterParaLectura = false;
$(document).ready(function () {
    bool_usarMasterParaLectura = $('input.inpt_usarMasterParaLectura').val() == 'True';
});
var finUsoMaster = null;

//Realizamos la peticion
function MontarResultados(pFiltros, pPrimeraCarga, pNumeroResultados, pPanelID, pTokenAfinidad) {
    contResultados = contResultados + 1;
    if (document.getElementById('ctl00_ctl00_CPH1_CPHContenido_txtRecursosSeleccionados') != null) {
        document.getElementById('ctl00_ctl00_CPH1_CPHContenido_txtRecursosSeleccionados').value = '';
        document.getElementById('ctl00_ctl00_CPH1_CPHContenido_lblErrorMisRecursos').style.display = 'none';
    }
    var servicio = new WS($('input.inpt_UrlServicioResultados').val(), WSDataType.jsonp);

    var paramAdicional = parametros_adiccionales;

    /*
    if ($('li.mapView').attr('class') == "mapView activeView") {
        paramAdicional += 'busquedaTipoMapa=true';
    }*/
    /*
    if ($('.chartView').attr('class') == "chartView activeView") {
        paramAdicional = 'busquedaTipoChart=' + chartActivo + '|' + paramAdicional;
    }*/

    if ($('li.mapView').hasClass('activeView')) {
        paramAdicional += 'busquedaTipoMapa=true';
    }


    if ($('.chartView').hasClass('activeView')) {
        paramAdicional = 'busquedaTipoChart=' + chartActivo + '|' + paramAdicional;
    }

    var metodo = 'CargarResultados';
    var params = {};

    if (bool_usarMasterParaLectura) {
        if (finUsoMaster == null) {
            finUsoMaster = new Date();
            finUsoMaster.setMinutes(finUsoMaster.getMinutes() + 1);
        }
        else {
            var fechaActual = new Date();
            if (fechaActual > finUsoMaster) {
                bool_usarMasterParaLectura = false;
                finUsoMaster = null;
            }
        }
    }

    params['pUsarMasterParaLectura'] = bool_usarMasterParaLectura;
    params['pProyectoID'] = $('input.inpt_proyID').val();
    params['pEsUsuarioInvitado'] = $('input.inpt_bool_esUsuarioInvitado').val() == 'True';

    if (typeof (identOrg) != 'undefined') {
	 
        params['pIdentidadID'] = identOrg;
    }
    else {
	 
        params['pIdentidadID'] = $('input.inpt_identidadID').val();
    }
    params['pParametros'] = '' + pFiltros.replace('#', '');
    params['pLanguageCode'] = $('input.inpt_Idioma').val();
    params['pPrimeraCarga'] = pPrimeraCarga == "True";
    params['pAdministradorVeTodasPersonas'] = adminVePersonas == "True";
    params['pTipoBusqueda'] = tipoBusqeda;
    params['pNumeroParteResultados'] = pNumeroResultados;
    params['pGrafo'] = grafo;
    params['pFiltroContexto'] = filtroContexto;
    params['pParametros_adiccionales'] = paramAdicional;
    params['cont'] = contResultados;
    params['tokenAfinidad'] = pTokenAfinidad;

    // Montar resultados obtenidos por al hacer clic Facetas
    $.post(obtenerUrl($('input.inpt_UrlServicioResultados').val()) + "/" + metodo, params, function (response) {
        if (params['cont'] == contResultados) {
            var data = response
            if (response.Value != null) {
                data = response.Value;
            }

            var vistaMapa = (params['pParametros_adiccionales'].indexOf('busquedaTipoMapa=true') != -1);
            var vistaChart = (params['pParametros_adiccionales'].indexOf('busquedaTipoChart=') != -1);

            var descripcion = data;

            var funcionJS = '';
            if (descripcion.indexOf('###ejecutarFuncion###') != -1) {
                var funcionJS = descripcion.substring(descripcion.indexOf('###ejecutarFuncion###') + '###ejecutarFuncion###'.length);
                funcionJS = funcionJS.substring(0, funcionJS.indexOf('###ejecutarFuncion###'));

                descripcion = descripcion.replace('###ejecutarFuncion###' + funcionJS + '###ejecutarFuncion###', '');
            }

            if (tipoBusqeda == 12) {
                var panelListado = $(pPanelID).parent();
                panelListado.html('<div id="' + pPanelID.replace('#', '') + '"></div><div id="' + panResultados.replace('#', '') + '"></div>')

                var panel = $(pPanelID);
                panel.css('display', 'none');
                panel.html(descripcion);
                panelListado.append(panel.find('.resource-list').html())
                panel.find('.resource-list').html('');
            } else if (!vistaMapa && !vistaChart) {                                
                // Mostrar los resultados dentro de "panResultados / panelResultados -->resource-list-wrap"
                //$(pPanelID).html(descripcion);
                let contenedorPrincipal = $(document).find(`${pPanelID} .resource-list-wrap`).length > 0
                    ? $(document).find(`${pPanelID} .resource-list-wrap`)
                    : $(document).find("#panelResultados .resource-list-wrap");
                // Mostrar los resultados
                contenedorPrincipal.html(descripcion);
            }
            else {
                var arraydatos = descripcion.split('|||');

                if ($('#panAuxMapa').length == 0) {
                    $(pPanelID).parent().html($(pPanelID).parent().html() + '<div id="panAuxMapa" style="display:none;"></div>');
                }

                if (vistaMapa) {
                    $('#panAuxMapa').html('<div id="numResultadosRemover">' + arraydatos[0] + '</div>');
                }

                if (vistaChart) {
                    datosChartActivo = arraydatos;
                    $(pPanelID).html('<div id="divContChart"></div>');
                    eval(jsChartActivo);
                }
                else {
                    utilMapas.MontarMapaResultados(pPanelID, arraydatos);
                }
            }
            FinalizarMontarResultados(paramAdicional, funcionJS, pNumeroResultados, pPanelID);
        }
        if (MontarResultadosScroll.pagActual != null) {
            MontarResultadosScroll.pagActual = 1;
            //MontarResultadosScroll.cargarScroll();
        }
    }, "json");
}

/**
 * Comportamiento de Scrolling para cargar más resultados al llegar al final de la página
 * */
const scrollingListadoRecursos = {
    init: function () {
        this.config();
        //this.crearCargando();
        this.scrollingListado();
        return;
    },
    config: function () {
        /* Contenedor donde se encuentran los resultados (Recursos, Mensajes...) */
        this.contenedorPrincipal = $(document).find("#panResultados .resource-list-wrap").length > 0
            ? $(document).find("#panResultados .resource-list-wrap")
            : $(document).find("#panelResultados .resource-list-wrap");
        return;
    },
    scrollingListado: function () {
        var that = this;
        MontarResultadosScroll.init("#footer", ".resource");

        MontarResultadosScroll.CargarResultadosScroll = function (data) {
            var htmlRespuesta = document.createElement("div");
            htmlRespuesta.innerHTML = data;
            $(htmlRespuesta)
                // Buscar cada resultado item
                .find("article")
                .each(function () {
                    var resource = $(this);
                    // Añadir cada item encontrado a la lista de resultados                  
                    that.contenedorPrincipal.append(this);
                });
            that.cargandoScrolling(data);
            // Formatear el contenido que ha sido traido al haber hecho scroll
            limpiarActividadRecienteHome.init();
        };

        return;
    },
    cargandoScrolling: function (data) {
        var cargandoOld = $(".loading-more-results");
        // Ocultar el loading
        if (cargandoOld.length != 0) cargandoOld.addClass("d-none");
        //if (data.length != 0) this.crearCargando();

        return;
    },

    /**
     * Mostrar el Loading de nuevo
     * @returns 
     */
    crearCargando: function () {
        /* Añadir Cargando después del listado */
        const cargandoOld = $(".loading-more-results");
        cargandoOld.removeClass("d-none");
        return;
    },
};

///Uso:
/// <summary>
/// Engancha el comporatamiento del scroll
/// </summary>
/// <param name="pFooter">Identificador Jquery del elemnto que lanzará la búsqueda al llegar al scroll</param>
/// <param name="pResource">Identificador Jquery de los elementos que representan los recursos</param>
/// <returns></returns>
/// MontarResultadosScroll.init(pFooter,pResource);			

/// <summary>
/// Método que recibe los resultados
/// </summary>
/// <param name="pData">HTML con los resultados</param>
/// <returns></returns>
/// MontarResultadosScroll.CargarResultadosScroll = function (pData) {
///     var htmlRespuesta = document.createElement("div");
///     htmlRespuesta.innerHTML = pData;
///     $(htmlRespuesta).find('.resource').each(function () {
///         $('#panResultados .resource').last().after(this)
///     });
/// }

/**
 * Comportamiento de mostrado de Resultados cuando se hace Scrolling. Funcionamiento junto con scrollingListadoRecursos
 * */
var MontarResultadosScroll = {
    footer: null,
    item: null,
    pagActual: null,
    active: true,
    init: function (idFooterJQuery, idItemJQuery, allowFirstLoadScrolling = false) {
        this.pagActual = 1;
        this.footer = $(idFooterJQuery);
        this.item = idItemJQuery;
        // Configurar que no cargue resultados nada más cargar la web
        //this.allowScrolling = allowFirstLoadScrolling;
        // Controlar que solo haga una petición cada vez
        this.isLoadingData = false;
        this.cargarScroll();
        return;
    },
    cargarScroll: function () {
        var that = this;
        this.waypointMoreResults = new Waypoint({
            element: that.footer,
            handler: function (direction) {
                if (direction == "down" && that.isLoadingData == false) {
                    // Indicar que se va a tramitar una petición de datos para que no haga varias
                    that.isLoadingData = true;
                    /* Antes de hacer petición visualizar el "Loading" */
                    scrollingListadoRecursos.crearCargando();
                    /* Realizar petición al servidor */
                    that.peticionScrollResultados().done(function (data) {
                        // Respuesta obtenida -> Ocultar loading
                        scrollingListadoRecursos.cargandoScrolling();
                        var htmlRespuesta = document.createElement("div");
                        htmlRespuesta.innerHTML = data;
                        if ($(htmlRespuesta).find(that.item).length > 0) {
                            that.CargarResultadosScroll(data);
                            // La petición ha terminado. Permitir hacer más peticiones
                            that.isLoadingData = false;
                        } else {
                            that.CargarResultadosScroll('');
                            // No se traen más datos -> Eliminar scrolling
                            // that.destroyScroll();
                            // La petición ha terminado. Permitir hacer más peticiones
                            that.isLoadingData = false;
                        }
                        if ((typeof CompletadaCargaRecursos != 'undefined')) {
                            CompletadaCargaRecursos();
                        }
                        if (typeof (urlCargarAccionesRecursos) != 'undefined') {
                            ObtenerAccionesListadoMVC(urlCargarAccionesRecursos);
                        }
                    });
                } else {
                    that.allowScrolling = true;
                }
            },

            offset: 'bottom-in-view' // Disparar petición cuando se visualice el footer
        })

        return;
    },
    destroyScroll: function () {
        this.waypointMoreResults.destroy();
        return;
    },
    peticionScrollResultados: function () {
        var defr = $.Deferred();
        //Realizamos la peticion 
        if (this.pagActual == null) {
            this.pagActual = 1;
        }
        this.pagActual++;
        //var servicio = new WS($('input.inpt_UrlServicioResultados').val(), WSDataType.jsonp);
        var filtros = ObtenerHash2().replace(/&/g, '|');

        if (typeof (filtroDePag) != 'undefined' && filtroDePag != '') {
            if (filtros != '') {
                filtros = filtroDePag + '|' + filtros;
            }
            else {
                filtros = filtroDePag;
            }
        }

        filtros += "|pagina=" + this.pagActual;
        var params = {};

        params['pUsarMasterParaLectura'] = bool_usarMasterParaLectura;
        params['pProyectoID'] = $('input.inpt_proyID').val();
        params['pEsUsuarioInvitado'] = $('input.inpt_bool_esUsuarioInvitado').val() == 'True';
        params['pIdentidadID'] = $('input.inpt_identidadID').val();
        params['pParametros'] = '' + filtros.replace('#', '');
        params['pLanguageCode'] = $('input.inpt_Idioma').val();
        params['pPrimeraCarga'] = false;
        params['pAdministradorVeTodasPersonas'] = adminVePersonas.toString().toLowerCase() == "true";
        params['pTipoBusqueda'] = tipoBusqeda;
        params['pNumeroParteResultados'] = 1;
        params['pGrafo'] = grafo;
        params['pFiltroContexto'] = filtroContexto;
        params['pParametros_adiccionales'] = parametros_adiccionales;
        params['cont'] = contResultados;


        $.post(obtenerUrl($('input.inpt_UrlServicioResultados').val()) + "/CargarResultados", params, function (response) {
            if (params['cont'] == contResultados) {
                var data = response
                if (response.Value != null) {
                    data = response.Value;
                }
                defr.resolve(data);
            }
        }, "json");
        return defr;
    }
}


///Uso:
/// <summary>
/// Engancha el comporatamiento de la paginación de resultados
/// </summary>
/// <param name="pEnlaceResource">Identificador Jquery de los elementos que tienen enlaces del recurso representan los recursos</param>
/// <param name="panResultados">Identificador Jquery del elemnto que identifica que se trata de una página de búsqueda</param>
/// <returns></returns>
/// MontarPaginacionBusquedaResultados.init(pEnlaceResource,'#panResultados');			

/// <summary>
/// Método que recibe los resultados
/// </summary>
/// <param name="pBusquedaArray">Array con los enlaces de los recursos de la búsqueda</param>
/// <param name="pIndice">Indice del recurso actual</param>
/// <returns></returns>
///MontarPaginacionBusquedaResultados.MontarBotonera = function (pBusquedaArray, pIndice) {
///    if (pIndice > 0) {
///        //Botón atrás
///        var botonAnterior = '<a href="' + pBusquedaArray[pIndice - 1] + '">Anterior</a>';
///        $(botonAnterior).insertBefore('footer');
///    }
///    if (pIndice < pBusquedaArray.length - 1) {
///        //Botón siguiente
///        var botonSiguiente = '<a href="' + pBusquedaArray[pIndice + 1] + '">Siguiente</a>';
///        $(botonSiguiente).insertBefore('footer');
///    }
//}
var MontarPaginacionBusquedaResultados = {
    numActual: 0,
    itemEnlace: null,
    pagBusqueda: null,
    cookiesEnabled: null,
    init: function (idEnlaceItemJQuery, idContenedorPaginaBusqueda) {
        this.itemEnlace = idEnlaceItemJQuery;
        this.pagBusqueda = idContenedorPaginaBusqueda;
        if ($(this.pagBusqueda).length > 0) {
            //Enganchar paginación de la búsqueda de resultados
            this.cargarPaginacion();
        }
        //Inseratr botones paginación
        this.botonesPaginacion();
        return;
    }, comprobarCookies: function () {
        if (this.cookiesEnabled == null) {
            var dt = new Date();
            dt.setSeconds(dt.getSeconds() + 60);
            document.cookie = "cookietest=1; expires=" + dt.toGMTString();
            this.cookiesEnabled = document.cookie.indexOf("cookietest=") != -1;
            dt.setSeconds(dt.getSeconds() - 120);
            document.cookie = "cookietest=1; expires=" + dt.toGMTString();
        }
        return this.cookiesEnabled;
    },
    cargarPaginacion: function () {
        var urlBusqueda = window.location.href;
        var guid = guidGenerator();
        var recursos = [];

        // Modificamos urls y almacenamos
        $(this.itemEnlace).each(function () {
            var newUrl = $(this).attr('href');
            if (newUrl.indexOf("?") > 0) {
                newUrl = newUrl.substring(0, newUrl.indexOf("?"));
            }

            newUrl += "?searchid=" + guid;
            $(this).attr('href', newUrl);
            if ($.inArray(newUrl, recursos) == -1) {
                recursos.push(newUrl);
            }
        });

        var vars = [], hash;
        if (window.location.href.indexOf('?') > -1) {
            var hashes = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
            for (var i = 0; i < hashes.length; i++) {
                hash = hashes[i].split('=');
                vars.push([hash[0], hash[1]]);
            }
        }

        urlBusqueda = "";
        pagina = 1;
        for (var j = 0; j < vars.length; j++) {
            if (vars[j][0] == "pagina") {
                pagina = vars[j][1];
            } else {
                urlBusqueda += "&" + vars[j][0] + "=" + vars[j][1];
            }
        }

        var InfoBusqueda = {
            resourceList: recursos,
            searchUrl: urlBusqueda,
            tipoBusqueda: tipoBusqeda,
            grafo: grafo,
            filtroContexto: filtroContexto,
            parametros_adiccionales: parametros_adiccionales,
            paginaMin: pagina,
            paginaMax: pagina
        }
        var listaBusquedas = this.comprobarCookies() ? JSON.parse(localStorage.getItem("searchids")) : null;
        if (listaBusquedas == null) {
            listaBusquedas = [];
        } else if (listaBusquedas.length >= 20) {
            localStorage.removeItem(listaBusquedas.shift());
        }
        listaBusquedas.push(guid);
        if (this.comprobarCookies()) {
            localStorage.setItem("searchids", JSON.stringify(listaBusquedas));
            localStorage.setItem(guid, JSON.stringify(InfoBusqueda));
        }
    }, botonesPaginacion: function () {
        this.numActual++;
        var searchid = window.location.search;
        searchid = searchid.substring(searchid.indexOf("searchid=") + "searchid=".length);
        var InfoBusqueda = this.comprobarCookies() ? JSON.parse(localStorage.getItem(searchid)) : null;
        if (InfoBusqueda != null) {
            var indice = $.inArray(window.location.href, InfoBusqueda.resourceList);
            if (indice == InfoBusqueda.resourceList.length - 1 && this.numActual < 2) {
                //Si estamos en el último recurso
                //Cargamos la página siguiente
                this.peticionPaginaSiguiente(searchid, InfoBusqueda, true);
            } else if (indice == 0 && InfoBusqueda.paginaMin > 1 && this.numActual < 2) {
                //Si estamos en el primer recurso y la página es mayor a la 1
                //Cargamos la página anterior
                this.peticionPaginaSiguiente(searchid, InfoBusqueda, false);
            } else if (indice != -1) {
                //Si no es ninguno de los dos casos anteriores y el recurso está en la lista montamos los botones
                this.MontarBotonera(InfoBusqueda.resourceList, indice);
            }
        }
    }, peticionPaginaSiguiente: function (searchID, InfoBusqueda, siguiente) {
        var pagina = 0;
        if (siguiente) {
            InfoBusqueda.paginaMax = InfoBusqueda.paginaMax + 1;
            pagina = InfoBusqueda.paginaMax;
        } else {
            InfoBusqueda.paginaMin = InfoBusqueda.paginaMin - 1;
            pagina = InfoBusqueda.paginaMin;
        }

        var that = this;
        var servicio = new WS($('input.inpt_UrlServicioResultados').val(), WSDataType.jsonp);
        var filtros = InfoBusqueda.searchUrl.substring(InfoBusqueda.searchUrl.indexOf("?") + 1).replace(/&/g, '|');
        filtros += "|pagina=" + pagina;
        var params = {};

        params['pUsarMasterParaLectura'] = bool_usarMasterParaLectura;
        params['pProyectoID'] = $('input.inpt_proyID').val();
        params['pEsUsuarioInvitado'] = $('input.inpt_bool_esUsuarioInvitado').val() == 'True';
        params['pIdentidadID'] = $('input.inpt_identidadID').val();
        params['pParametros'] = '' + filtros.replace('#', '');
        params['pLanguageCode'] = $('input.inpt_Idioma').val();
        params['pPrimeraCarga'] = false;
        params['pAdministradorVeTodasPersonas'] = false;
        params['pTipoBusqueda'] = InfoBusqueda.tipoBusqueda;
        params['pNumeroParteResultados'] = 1;
        params['pGrafo'] = InfoBusqueda.grafo;
        params['pFiltroContexto'] = InfoBusqueda.filtroContexto;
        params['pParametros_adiccionales'] = InfoBusqueda.parametros_adiccionales;

        $.post(obtenerUrl($('input.inpt_UrlServicioResultados').val()) + "/CargarResultados", params, function (response) {
            var data = response
            if (response.Value != null) {
                data = response.Value;
            }

            // Almacenamos urls
            var htmlRespuesta = document.createElement("div");
            htmlRespuesta.innerHTML = data;
            var listaRecursos = [];
            $(htmlRespuesta).find(that.itemEnlace).each(function () {
                var newUrl = $(this).attr('href') + "?searchid=" + searchID;
                if ($.inArray(newUrl, listaRecursos) == -1) {
                    listaRecursos.push(newUrl);
                }
            });

            if (listaRecursos.length > 0) {
                if (siguiente) {
                    InfoBusqueda.resourceList = InfoBusqueda.resourceList.concat(listaRecursos);
                } else {
                    InfoBusqueda.resourceList = listaRecursos.concat(InfoBusqueda.resourceList);
                }
                if (that.comprobarCookies()) {
                    //Almacenamos los nuevos recursos
                    localStorage.setItem(searchID, JSON.stringify(InfoBusqueda));
                }
            }
            //Montamos la paginacion
            that.botonesPaginacion();
        }, "json");
    }
}


function FinalizarMontarResultados(paramAdicional, funcionJS, pNumeroResultados, pPanelID) {

    var vistaMapa = (paramAdicional.indexOf('busquedaTipoMapa=true') != -1);
    var vistaChart = (paramAdicional.indexOf('busquedaTipoChart=') != -1);

    if (pNumeroResultados == 1) {
        if (typeof googleAnalyticsActivo !== "undefined" && googleAnalyticsActivo !== null && googleAnalyticsActivo) {
            RegistrarVisitaGoogleAnalytics();
        }
    }

    $('#divFiltrar').hide();
    $(pPanelID).show();

    $('#' + panFiltrosPulgarcito).css('display', '');

    EjecutarScriptsIniciales();
    if (pNumeroResultados == 1) { OcultarUpdateProgress(); viewOptions.init(); MontarNumResultados(); }

    if (funcionJS != '') {
        eval(funcionJS);
    }

    if (tipoBusqeda == 12 || tipoBusqeda == 11) {
        var vistaCompacta = $('#col02 .resource-list.compactview').length > 0;
        $('#col02 .resource-list').find('.resource').each(function (indice) {
            var mensaje = $(this);
            var utils = mensaje.find('.utils-2');
            var acciones = mensaje.find('.acciones');
            utils.show();
            acciones.show();
            mensaje.removeClass('over');
            if (vistaCompacta) { acciones.hide(); }
            mensaje.hover(
		        function () {
		            $(this).removeClass('over');
		            utils.show();
		            acciones.show();
		        },
		        function () {
		            $(this).removeClass('over');
		            utils.show();
		            acciones.show();
		        }
	        );
        });
    }
    MontarFechas();

    limpiarActividadRecienteHome.init();

    enlazarFiltrosBusqueda();

    if (typeof listadoMensajesMostrarAcciones != 'undefined') {
        /* Lanzar el evento de las imagenes */
        listadoMensajesMostrarAcciones.init();
    }

    /* numero categorias */
    mostrarNumeroCategorias.init();

    /* numero etiquetas */
    mostrarNumeroEtiquetas.init();

    /* enganchar mas menos categorias y etiquetas */
    verTodasCategoriasEtiquetas.init();

    /* pintar iconos de los videos*/
    pintarRecursoVideo.init();

    /* Es listado de mensajes */
    if (tipoBusqeda == 12) {
        listado = $('#col02 .listadoMensajes .resource-list.compactview');
        if (listado.length > 0) {
            listadoMensajesMostrarAcciones.init();
            $('#col02 .listadoMensajes .resource-list .resource h4 a').each(function (indice) {
                var enlaceTitulo = $(this);
                var asunto = enlaceTitulo.attr("title");

                if (asunto != null && asunto.length > 33) {
                    enlaceTitulo.text(asunto.substring(0, 30) + "...");
                }
            });
        }
    }
    else if ((typeof customizarListado != 'undefined')) {
        /* En los listados de Inevery, hay que ejecutar este script */
        customizarListado.init();
    }

    if ((typeof CompletadaCargaRecursos != 'undefined')) {
        CompletadaCargaRecursos();
    }

    /* enganchar vista listado-mosaico*/
    if (!vistaMapa && !vistaChart) {
        modoVisualizacionListados.init(pPanelID);
    }

    utilMapas.AjustarBotonesVisibilidad();

    //Si hay resultados mostramos la caja de b?squeda
    if ((document.getElementById('ListadoGenerico_panContenedor') != null || vistaMapa || vistaChart) && document.getElementById('ctl00_ctl00_CPH1_CPHContenido_divCajaBusqueda') != null) {
        document.getElementById('ctl00_ctl00_CPH1_CPHContenido_divCajaBusqueda').style.display = "";
    }

    if (typeof scriptBusqueda != 'undefined') {
        if (scriptBusqueda != null && scriptBusqueda != "") {
            eval(scriptBusqueda);
        }
    }

    if (pNumeroResultados == 1 && funcionExtraResultados != "") {
        eval(funcionExtraResultados);
    }

    if (typeof (urlCargarAccionesRecursos) != 'undefined') {
        ObtenerAccionesListadoMVC(urlCargarAccionesRecursos);
    }
    MontarFechaCliente();
}

function TraerRecPuntoMapa(me, panelVerMas, claveLatLog, docIDs, docIDsExtra, panelVerMasX, panelVerMasY, tipo) {
    if (me == null) {
        $('#' + panelVerMas).html('<div><p>' + form.cargando + '...</p></div>');
    }

    var docIDsExtra = '';

    if (((docIDs.length + 1) / 37) > 10) {
        docIDsExtra = docIDs.substring(37 * 10)
        docIDs = docIDs.substring(0, 37 * 10);
    }

    var servicio = new WS($('input.inpt_UrlServicioResultados').val(), WSDataType.jsonp);

    var metodo = 'ObtenerFichaRecurso';
    var params = {};
    params['bool_usarMasterParaLectura'] = bool_usarMasterParaLectura;
    params['proyecto'] = $('input.inpt_proyID').val();
    params['bool_esMyGnoss'] = $('input.inpt_bool_esMyGnoss').val() == 'True';
    params['bool_estaEnProyecto'] = $('input.inpt_bool_estaEnProyecto').val() == 'True';
    params['bool_esUsuarioInvitado'] = $('input.inpt_bool_esUsuarioInvitado').val() == 'True';
    params['identidad'] = $('input.inpt_identidadID').val();
    params['parametros'] = '';
    params['languageCode'] = $('input.inpt_Idioma').val();
    params['tipoBusqueda'] = tipoBusqeda;
    params['filtroContexto'] = filtroContexto;
    params['parametros_adiccionales'] = parametros_adiccionales;
    params['cont'] = contResultados;
    params['documentoID'] = docIDs;


    servicio.call(metodo, params, function (data) {
        if (docIDsExtra != '') {
            var idRecMas = guidGenerator();
            data += '<div id="mapaMasRec_' + idRecMas + '" class="verMasMapa"><a href="javascript:TraerRecPuntoMapa(null, \'mapaMasRec_' + idRecMas + '\', \'' + claveLatLog + '\',\'' + docIDsExtra + '\')">' + form.vermas + '</a></div>';
        }

        if (me != null) {
            me.infowindow.setContent(data);
            $('.fichaMapa').parent().css('overflow', 'hidden');
        }
        else {
            $('#' + panelVerMas).html(data);
            if (panelVerMasX != null && panelVerMasY != null) {
                $('#' + panelVerMas).removeAttr('class');
                if (tipo != null) {
		   
                    $('#' + panelVerMas).addClass(tipo);
                }

                $('#' + panelVerMas).attr('posx', panelVerMasX);
                $('#' + panelVerMas).attr('posy', panelVerMasY);

                $('#' + panelVerMas).css("top", '0px');
                $('#' + panelVerMas).css("left", '0px');

                ReposicionarFichaMapa();

                if ($('#' + panelVerMas).attr('activo') == 'true') {
                    $('#' + panelVerMas).show();
                }

                if ((typeof CompletadaCargaFichaMapaComunidad != 'undefined')) {
                    CompletadaCargaFichaMapaComunidad();
                }
            }
        }
        limpiarActividadRecienteHome.init();
    });
}

function ReposicionarFichaMapa() {
    var fichaMapa = $('#' + utilMapas.fichaMapa);
    var posX = 0;
    var posY = 0;
    posX = parseFloat(fichaMapa.attr('posx'));
    posY = parseFloat(fichaMapa.attr('posy'));

    //Anchura y altura del panel que mostramos
    var anchura = fichaMapa.width();
    var altura = fichaMapa.height();

    //Anchura y altura de la ventana que estamos viendo
    var centroNavegadorVentanaX = $(window).width() / 2;
    var centroNavegadorVentanaY = $(window).height() / 2;

    //Coordenadas relativas punto
    var panelVerMasXRelativo = posX - $(window).scrollLeft();
    var panelVerMasYRelativo = posY - $(window).scrollTop();

    var margenX = 35;
    var margenY = 40;
    if (panelVerMasXRelativo > centroNavegadorVentanaX) {
        posX = posX - anchura - margenX;
        fichaMapa.addClass('indicaDerecha');
    } else {
        posX = posX + margenX;
        fichaMapa.addClass('indicaIzquierda');
    }

    if (panelVerMasYRelativo > centroNavegadorVentanaY) {
        posY = posY - altura + margenY;
        fichaMapa.addClass('indicaInferior');
    } else {
        posY = posY - margenY;
        fichaMapa.addClass('indicaSuperior');
    }

    fichaMapa.css("left", posX + 'px');
    fichaMapa.css("top", posY + 'px');
    fichaMapa.css("z-index", '99999');

}

function SeleccionarChart(pCharID, pJsChart) {
    $('.listView').attr('class', 'listView');
    $('.gridView').attr('class', 'gridView');
    $('.mapView').attr('class', 'mapView');
    $('.chartView').attr('class', 'chartView activeView');

    chartActivo = pCharID;
    jsChartActivo = pJsChart;

    FiltrarPorFacetas(ObtenerHash2());
}

function ExportarBusqueda(pExportacionID, pNombreExportacion, pFormatoExportacion) {
    if (pExportacionID != "" && pNombreExportacion != "" && $('#ParametrosExportacion').length > 0 && $('#FormExportarBusqueda').length > 0 && pFormatoExportacion != "") {
        $('#ParametrosExportacion').val(pExportacionID + '|' + pNombreExportacion + '|' + pFormatoExportacion);
        $('#FormExportarBusqueda').submit();
    }
}

function MontarFacetas(pFiltros, pPrimeraCarga, pNumeroFacetas, pPanelID, pFaceta, pTokenAfinidad) {
    pFiltros = pFiltros.replace(/&/g, '|');
    if (mostrarFacetas) {
        //contFacetas = contFacetas + 1;

        var verMas = false;

        if (pFaceta != null && pFaceta.indexOf('|vermas') != -1) {
            verMas = true;
            pFaceta = pFaceta.substring(0, pFaceta.lastIndexOf('|'));
        }

        var servicio = new WS($('input.inpt_UrlServicioFacetas').val(), WSDataType.jsonp);

        var paramAdicional = parametros_adiccionales;

        if ($('li.mapView').attr('class') == "mapView activeView") {
            paramAdicional += 'busquedaTipoMapa=true';
        }

        var metodo = 'CargarFacetas';
        var params = {};


        params['pProyectoID'] = $('input.inpt_proyID').val();
        params['pEstaEnProyecto'] = $('input.inpt_bool_estaEnProyecto').val() == 'True';
        params['pEsUsuarioInvitado'] = $('input.inpt_bool_esUsuarioInvitado').val() == 'True';
        params['pIdentidadID'] = $('input.inpt_identidadID').val();
        params['pParametros'] = '' + pFiltros.replace('#', '');
        params['pLanguageCode'] = $('input.inpt_Idioma').val();
        params['pPrimeraCarga'] = pPrimeraCarga.toString().toLowerCase() == "true";
        params['pAdministradorVeTodasPersonas'] = adminVePersonas.toString().toLowerCase() == "true";
        params['pTipoBusqueda'] = tipoBusqeda;
        params['pGrafo'] = grafo;
        params['pFiltroContexto'] = filtroContexto;
        if (typeof urlBusqueda != "undefined") {
            params['pUrlPaginaActual'] = urlBusqueda;
        }
        params['pParametros_adiccionales'] = paramAdicional;
        params['pUbicacionBusqueda'] = ubicacionBusqueda;
        params['pNumeroFacetas'] = pNumeroFacetas;
        params['pUsarMasterParaLectura'] = bool_usarMasterParaLectura;
        params['pFaceta'] = pFaceta;
        params['tokenAfinidad'] = pTokenAfinidad;

        $.post(obtenerUrl($('input.inpt_UrlServicioFacetas').val()) + "/" + metodo, params, function (data) {
            if (decodeURI(pFiltros) == decodeURI(filtrosPeticionActual) || pFiltros == replaceAll(filtrosPeticionActual, '&', '|') || (typeof filtrosDeInicio !== "undefined" && (pFiltros == filtrosDeInicio || pFiltros == replaceAll(filtrosDeInicio, '|', '&'))) /*|| pFiltros.indexOf(filtrosPeticionActual) >= 0 Esto hace que al quitar filtros se amontonen facetas*/) {
                if (pFaceta == null && (pNumeroFacetas == 1 || pNumeroFacetas == 2)) {
                    MontarFacetas(pFiltros, pPrimeraCarga, pNumeroFacetas + 1, pPanelID, pFaceta, pTokenAfinidad);
                }
                var descripcion = data;
                if (descripcion.indexOf("class=\"box") != -1 && document.getElementById('facetaEncuentra') != null) {
                    document.getElementById('facetaEncuentra').style.display = '';
                }
                $(pPanelID).show();

                if (pFaceta != null && data != "" && pNumeroFacetas == -1) {
                    var divFacetaID = pFaceta.replace(/\:/g, '_').replace(/\@/g, '-');
                    var divFacetaID_out = '#out_' + divFacetaID;
                    if ($(divFacetaID_out).length > 0) {
                        $(divFacetaID_out).show();
                    }
                    else {
                        //No existe el panel out_faceta. Inserto en el panel original el contenido del resultante. 
                        $('#' + divFacetaID).replaceWith(data);
                    }
                }

                if (pFaceta == null || pFaceta == '' || pNumeroFacetas == 1) {
					if( pNumeroFacetas == 1)
					{
						$('#' + panFacetas).html('');
					}
                    var panelFacetas = pPanelID;
                    if ($('#facetedSearch').length) {
                        panelFacetas = '#facetedSearch';
                        descripcion = $('<div>' + descripcion + '</div>').find("#facetedSearch").html();
                    }

                    if (pNumeroFacetas > 1 && $(".listadoAgrupado").length) {
                        panelFacetas = "#" + $(".listadoAgrupado").attr("aux");
                    }

                    if (pNumeroFacetas == 1) {
                        if (!descripcion.replace('<div id="facetedSearch" class="facetedSearch">', '').replace('</div>', '').replace('</div>', '').trim().startsWith('<div id="panelFiltros" style="display:none">')) {
                            $('#facetedSearch').css('display', 'block');
                        } else {
                            //Ocultamos el panel de facetas
                            if ($('#facetedSearch').length == 0) {
                                $('#facetedSearch').css('display', 'none')
                            }
                            $(panelFacetas).css('display', 'none')
                        }
                    }
                    $(panelFacetas).append(descripcion);

                    facetedSearch.init();
                }
                else {
                    //Si viene el parámetro pFaceta, se está rellenando una faceta, hay que sustituir el contenido anterior por el actual. 

                    if (verMas) {
                        descripcion = $('#' + divFacetaID, $(data.trim())).html();
                        var htmlVerMas = $('p.moreResults', $(pPanelID)).html();
                        if (typeof (htmlVerMas) != 'undefined') {
                            htmlVerMas = htmlVerMas.substring(0, htmlVerMas.indexOf('>') + 1) + form.vermenos + '</a>';
                            descripcion += '<p class="moreResults">' + htmlVerMas + '</p>';
                        }
                        if (descripcion == null) {
						 
                            descripcion = data.trim();
                        }
                    }

                    if (descripcion.indexOf('id="' + pPanelID.substr(1) + '"') != -1) {
                        //La descripción ya contiene el panel, lo sustituyo.
                        $(pPanelID).replaceWith(descripcion);
                    }
                    else {
                        $(pPanelID).html(descripcion);
                    }
                    facetedSearch.init();
                }

                if (pNumeroFacetas == 1) { MontarPanelFiltros(); }
                /* presentacion facetas */
                // Longitud facetas por CSS
		        // limiteLongitudFacetas.init();

                //}

                if (pNumeroFacetas == 3) {
                    if ($(".filtroFacetaFecha").length > 0) {
					 
                        $(".filtroFacetaFecha").datepicker();
                    }

                    if ($("div.divdatepicker").length > 0) {
                        IniciarFacetaCalendario();
                    }
                } else if (pNumeroFacetas == -1 && $("div.divdatepicker").length > 0) {
                    IniciarFacetaCalendario(pFiltros);
                }


                if (enlazarJavascriptFacetas) {
                    enlazarFacetasBusqueda();
                }
                else {
                    enlazarFacetasNoBusqueda();
                }

                if ((typeof CompletadaCargaFacetas != 'undefined')) {
                    /* En los listados de Inevery, hay que ejecutar este script */
                    CompletadaCargaFacetas();
                }

                if (funcionExtraFacetas != "") {
                    eval(funcionExtraFacetas);
                }
            }
        });
    }
    else {
        var col1 = document.getElementById('col01');
        if (col1 != null) {
            $('#col01').css('display', 'none');
            $('#col02').css('float', 'left');
        }
    }
    primeraCargaDeFacetas = false;
}

function IniciarFacetaCalendario(fechaInicioCalendario) {

    //Cogemos los eventos
    $("div.divdatepicker").each(function () {
        var events = new Array();
        var i = 0;
        $(this).parent().children('ul').children().each(function () {
            var fecha = $(this).children('a').attr('title');
            var filtro = $(this).children('a').attr('name');

            fecha = fecha.substring(fecha.indexOf('=') + 1);
            var agno = parseInt(fecha.substr(0, 4));
            var mes = parseInt(fecha.substr(4, 2)) - 1;
            var dia = parseInt(fecha.substr(6, 2));
            events[i] = { Title: filtro, Date: new Date(agno, mes, dia, 0, 0, 0, 0) };
            i++;
        });


        var changingDate = false;
        var weekSelected = false;

        //Iniciamos el datepicker
        $(this).datepicker({
            showWeek: true,
            beforeShowDay: function (date) {
                //Revisamos si coincide
                var result = [true, '', null];
                var matching = $.grep(events, function (event) {
                    return event.Date.valueOf() === date.valueOf();
                });

                if (matching.length) {
                    result = [true, 'css-class-to-highlight', ''];
                }

                return result;
            },
            onSelect: function (dateText) {
                if (!weekSelected) {
                    var date,
                        i = 0,
                        event = null;
                    var day = parseInt(dateText.substr(0, 2));
                    var month = parseInt(dateText.substr(3, 2)) - 1;
                    var year = parseInt(dateText.substr(6, 4));
                    selectedDate = new Date(year, month, day, 0, 0, 0, 0);

                    /* Determine if the user clicked an event: */
                    while (i < events.length && !event) {
                        date = events[i].Date;

                        if (selectedDate.valueOf() === date.valueOf()) {
                            event = events[i];
                        }
                        i++;
                    }
                    if (event != null) {
                        AgregarFaceta(event.Title + '|replace');
                    }
                }
            },
            onChangeMonthYear: function (year, month) {
                if (changingDate) {
                    return;
                }
                //Lamamos al servicio de facetas para que recargue los elementos del mes siguiente.
                VerFechasSiguienteMes($(this).attr('name'), $(this).attr('name').replace(':', '_'), year, month);
            }
        });

        if (typeof (fechaInicioCalendario) != 'undefined' && fechaInicioCalendario != '') {

            var filtrosArray = fechaInicioCalendario.split('|');
            var fecha = '';
            for (var i = 0; i < filtrosArray.length; i++) {
                if (filtrosArray[i] != '' && filtrosArray[i].indexOf($(this).attr('name')) >= 0) {
                    fecha = filtrosArray[i].substr($(this).attr('name'));
                }
            }

            fecha = fecha.substr(fecha.indexOf('=') + 1) + '00';

            var agno = fecha.substr(0, 4);
            var mes = fecha.substr(4, 2);
            var dia = fecha.substr(6, 2);
            var inicio = new Date(agno, mes, dia, 0, 0, 0, 0);

            changingDate = true;
            $(this).datepicker("setDate", inicio);
            changingDate = false;
        }

        //Cargar elementos siguientes
        //Dar valor a los nuevas columnas
        $("td.ui-datepicker-week-col").on("click", function () {
            weekSelected = true;

            //Me filtra por el d?a al que le hago click...
            $(this).next().click();

            weekSelected = false;

            var fromDate = $("div.divdatepicker").datepicker("getDate");
            var toDate = $("div.divdatepicker").datepicker("getDate");
            toDate.setDate(toDate.getDate() + 6);

            var monthFrom = fromDate.getMonth() + 1;
            var monthTo = toDate.getMonth() + 1;
            if (monthFrom <= 9) {
                monthFrom = '0' + monthFrom;
            }
            if (monthTo <= 9) {
                monthTo = '0' + monthTo;
            }

            var dateFrom = fromDate.getDate();
            var dateTo = toDate.getDate();
            if (dateFrom <= 9) {
                dateFrom = '0' + dateFrom;
            }

            if (dateTo <= 9) {
                dateTo = '0' + dateTo;
            }

            var filtroFrom = fromDate.getFullYear() + '' + monthFrom + '' + dateFrom;
            var filtroTo = toDate.getFullYear() + '' + monthTo + '' + dateTo;
            var filtroSemantico = $("div.divdatepicker").attr('name');
            AgregarFaceta(filtroSemantico + '=' + filtroFrom + '-' + filtroTo + '|replace');
        });

        $('div.ui-datepicker-title').on('click', function () {
            var date = $("div.divdatepicker").datepicker("getDate");
            var year = date.getFullYear();
            var month = date.getMonth() + 1;
            var dia = new Date(year, month, 0).getDate();

            if (month <= 9) {
                month = '0' + month;
            }
            if (dia <= 9) {
                dia = '0' + dia;
            }

            var filtro = year + '' + month + '01' + "-" + year + '' + month + '' + dia;
            var filtroSemantico = $("div.divdatepicker").attr('name');
            AgregarFaceta(filtroSemantico + '=' + filtro + '|replace');
        });

    });
}

function VerFechasSiguienteMes(faceta, controlID, year, month) {
    var filtros = ObtenerHash2();
    if (month <= 9) {
        month = '0' + month;
    }

    //Usamos el m?todo de limpiar filtro para limpiar el filtro que quramos.
    if (filtros.indexOf(faceta) >= 0) {
        filtros.split('&');
        var filtrosArray = filtros.split('&');
        filtros = '';

        for (var i = 0; i < filtrosArray.length; i++) {
            if (filtrosArray[i] != '' && filtrosArray[i].indexOf(faceta) == -1) {
                filtros += filtrosArray[i] + '&';
            } else if (filtrosArray[i] != '' && filtrosArray[i].indexOf('|') >= 0 && filtrosArray[i].indexOf(faceta) >= 0) {
                filtros += filtrosArray[i].substring(filtrosArray[i].indexOf('|'));
            }
        }
    }

    filtros += "|" + faceta + "=" + year + month;

    filtrosPeticionActual = "|" + faceta + "=" + year + month;

    MontarFacetas(filtros, false, -1, '#' + controlID, faceta + '|vermas');
}

function FinalizarMontarFacetas() {

    facetedSearch.init();

    MontarPanelFiltros();

    if (enlazarJavascriptFacetas) {
        enlazarFacetasBusqueda();
    }
    else {
        enlazarFacetasNoBusqueda();
    }

    if (HayFiltrosActivos(filtrosPeticionActual)) {
        $('#' + divFiltros).css('display', '');
        $('#' + divFiltros).css('padding-top', '0px !important');
        //$('#' + divFiltros).css('margin-top', '10px');
    }

    if ((typeof CompletadaCargaFacetas != 'undefined')) {
        /* En los listados de Inevery, hay que ejecutar este script */
        CompletadaCargaFacetas();
    }
}

$(document).ready(function () {
    /*$('.searchGroup .encontrar').click(function (event) {
        var txt = $(this).parent().find('.text');
        if ($(this).parent().find('#criterio').attr('origen') != undefined) {
            //Buscadores CMS
            var criterio = $(this).parent().find('#criterio').attr('origen');
            window.location.href = ObtenerUrlBusqueda(criterio) + txt.val();
            return false;
        }
        if (txt.hasClass('text') && txt.val() != '') {
            //Resto de buscadores
            window.location.href = $('input.inpt_baseUrlBusqueda').val() + '/recursos?search=' + txt.val();
            return false;
        }
        return false;
    });*/

    $('.searchGroup .text').keydown(function (event) {
        if ($(this).val().indexOf('|') > -1) {
            $(this).val($(this).val().replace(/\|/g, ''));
        };
        if (event.which || event.keyCode) {
            if ((event.which == 13) || (event.keyCode == 13)) {
                $(this).parent().find('.encontrar').click();
                return false;
            }
        } else {
            return true;
        };
    });

    $('.aaCabecera .searchGroup .text')
    .unbind()
    .keydown(function (event) {
        if ($(this).val().indexOf('|') > -1) {
            $(this).val($(this).val().replace(/\|/g, ''));
        };
        if (event.which || event.keyCode) {
            if ((event.which == 13) || (event.keyCode == 13)) {
                $(this).parent().find('.encontrar').click();
                return false;
            }
        } else {
            return true;
        };
    });

    $('.searchGroup .text').focus(function (event) {
        if ($(this).attr('class') == 'text defaultText') {
            $(this).attr('class', 'text');
            $(this).attr('value', '');
        }
    });

    //Buscador de la cabecera superior - Buscador de algunos buscadores (Comentarios, Mensajes...)
    $('.searchGroup .encontrar')
    .unbind()
    .click(function (event) {
        var ddlCategorias = $('.fieldsetGroup .ddlCategorias').val();
        var url = ObtenerUrlBusqueda(ddlCategorias);
        if (typeof ($('.tipoBandeja').val()) != 'undefined' && ddlCategorias == 'Mensaje') {
            //Es una búsqueda en la bandeja de mensajes
            url = url.replace('search=', $('.tipoBandeja').val() + '&search=')
        }
        var parametros = $('.searchGroup .text').val();
        var autocompletar = $('.ac_results .ac_over');
        if (typeof (autocompletar) != 'undefined' && autocompletar.length > 0 && typeof ($('.ac_results .ac_over')[0].textContent) != 'undefined') {
		 
            parametros = $('.ac_results .ac_over')[0].textContent;
        }

        if (parametros == '') {
            url = url.replace('?search=', '').replace('/tag/', '');
        }
        window.location.href = url + parametros;
    });
});



$(document).ready(function () {
    if ($('#finderSection').length > 0) {
        var urlPaginaActual = $('.inpt_urlPaginaActual').val();
        if (typeof (urlPaginaActual) != 'undefined') {
            $('#inputLupa').click(function (event) {
                window.location.href = urlPaginaActual + "?search=" + encodeURIComponent($('#finderSection').val()); 
            });
        }

        $('#finderSection').keydown(function (event) {
            if ($(this).val().indexOf('|') > -1) {
                $(this).val($(this).val().replace(/\|/g, ''));
            };
            if (event.which || event.keyCode) {
                if ((event.which == 13) || (event.keyCode == 13)) {
                    $(this).parent().find('.findAction').click();
                    return false;
                }
            } else {
                return true;
            };
        });

        if (typeof (origenAutoCompletar) == 'undefined') {
            origenAutoCompletar = ObtenerOrigenAutoCompletarBusqueda($('input.inpt_tipoBusquedaAutoCompl').val());
            if (origenAutoCompletar == '') {
                var pathName = window.location.pathname;

                pathName = pathName.substr(pathName.lastIndexOf('/') + 1);
                if (pathName.indexOf('?') > 0) {
                    pathName = pathName.substr(0, pathName.indexOf('?'));
                }

                origenAutoCompletar = pathName;
            }
        }

        //var tablaPropiaAutoCompletar = $('input.inpt_tablaPropiaAutoCompletar').val().toLowerCase() == 'true';
        var urlServicioAutocompletar = $('.inpt_urlServicioAutocompletar').val();
        if (urlServicioAutocompletar.indexOf('autocompletarEtiquetas') > 0) {
            var proyID = $('.inpt_proyID').val();
            var facetasBusqPag = $('.inpt_facetasBusqPag').val();
            var identidadID = $('.inpt_identidadID').val();

            $('#finderSection').autocomplete(
			null,
			{
			    servicio: new WS(urlServicioAutocompletar, WSDataType.jsonp),
			    metodo: 'AutoCompletarTipado',
			    delay: 0,
			    scroll: false,
			    selectFirst: false,
			    minChars: 1,
			    width: 'auto',
			    max: 25,
			    cacheLength: 0,
			    extraParams: {
			        pProyecto: proyID,
			        //pTablaPropia: tablaPropiaAutoCompletar,
			        pFacetas: facetasBusqPag,
			        pOrigen: origenAutoCompletar,
			        pIdentidad: $('input.inpt_identidadID').val(),
			        pIdioma: $('input.inpt_Idioma').val(),
			        maxwidth: '420px',
			        botonBuscar: 'inputLupa'
			    }
			}
			);
        } else {
            var proyID = $('.inpt_proyID').val();
            var facetasBusqPag = $('.inpt_facetasBusqPag').val();
            var identidadID = $('.inpt_identidadID').val();
            var bool_esMyGnoss = $('.inpt_bool_esMyGnoss').val() == 'True';
            var bool_estaEnProyecto = $('.inpt_bool_estaEnProyecto').val() == 'True';
            var bool_esUsuarioInvitado = $('input.inpt_bool_esUsuarioInvitado').val() == 'True';
            var orgID = $('input.inpt_organizacionID').val();
            var perfilID = $('input#inpt_perfilID').val();
            var parametros = $('.inpt_parametros').val();
            var tipo = $('.inpt_tipoBusquedaAutoCompl').val();
            var facetasBusqPag = $('.inpt_facetasBusqPag').val();
            $('#finderSection').autocomplete(
				null,
				{
				    //servicio: new WS(urlServicioAutocompletar, WSDataType.jsonp),
				    //metodo: 'AutoCompletarFacetas',
				    url: urlServicioAutocompletar + "/AutoCompletarFacetas",
                    type: "POST",
				    delay: 0,
				    minLength: 4,
				    scroll: false,
				    selectFirst: false,
				    minChars: 4,
				    width: 190,
				    cacheLength: 0,
				    extraParams: {
				        proyecto: proyID,
				        bool_esMyGnoss: bool_esMyGnoss == true,
				        bool_estaEnProyecto: bool_estaEnProyecto == true,
				        bool_esUsuarioInvitado: bool_esUsuarioInvitado == true,
				        identidad: identidadID,
				        organizacion: orgID,
				        filtrosContexto: '',
				        languageCode: $('input.inpt_Idioma').val(),
				        perfil: perfilID,
				        //pTablaPropia: tablaPropiaAutoCompletar,
				        pFacetas: facetasBusqPag,
				        pOrigen: origenAutoCompletar,
				        nombreFaceta: 'search',
				        orden: '',
				        parametros: parametros,
				        tipo: tipo,
				        botonBuscar: 'inputLupa'
				    }
				}
			);
        }
    }
});

$(document).ready(function () {
    PreparaAutoCompletarComunidad();
});

function PreparaAutoCompletarComunidad() {
    $('input.autocompletar').each(function () {
        var txtBusqueda = this;
        var ddlCategorias = $(this).parent().parent().find('.ddlCategorias');
        var pOrigen = '';
        var pTipoDdlCategorias = '';
        var facetasAutoComTip = '';
        if ($(this).attr('origen') != undefined) {
            pTipoDdlCategorias = $(this).attr('origen');
            pOrigen = ObtenerOrigenAutoCompletarBusqueda($(this).attr('origen'));
            facetasAutoComTip = ObtenerFacetasAutocompletar($(this).attr('origen'));
        } else if (typeof (ddlCategorias.val()) != 'undefined' && ddlCategorias.val() != '') {
            pOrigen = ObtenerOrigenAutoCompletarBusqueda(ddlCategorias.val());
            facetasAutoComTip = ObtenerFacetasAutocompletar(ddlCategorias.val());
            pTipoDdlCategorias = ddlCategorias.val();
        } else if (typeof ($('input.inpt_tipoBusquedaAutoCompl').val()) != 'undefined') {
            if ($('input.inpt_tipoBusquedaAutoCompl').val() != '') {
                pOrigen = origenAutoCompletar;
                facetasAutoComTip = ObtenerFacetasAutocompletar($('input.inpt_tipoBusquedaAutoCompl').val());
            }
            pTipoDdlCategorias = $('input.inpt_tipoBusquedaAutoCompl').val();
        }

        //Este trozo hace que la caja de arriba de b?squeda intente buscar en la pesta?a donde est?s en vez de donde se indica. Adem?s de que a veces no se puede buscar en la pest? que te encuentras (Ej: Indice)
        /*if (pOrigen == '') {
            var pathName = window.location.pathname;

            pathName = pathName.substr(pathName.lastIndexOf('/') + 1);
            if (pathName.indexOf('?') > 0) {
                pathName = pathName.substr(0, pathName.indexOf('?'));
            }

            pOrigen = pathName;
        }*/

        var urlServicioAutocompletarEtiquetas = $('input.inpt_urlServicioAutocompletarEtiquetas').val();

        var identidadID = $('input.inpt_identidadID').val();
        var proyID = $('input.inpt_proyID').val();
        var organizacionID = $('input.inpt_organizacionID').val();
        var perfilID = $('input#inpt_perfilID').val();

        var btnBuscarID = '';
        // Size() deprecado
        //if ($(this).parent().find('.encontrar').size() > 0) {
          if ($(this).parent().find('.encontrar').length > 0) {
            btnBuscarID = $(this).parent().find('.encontrar').attr('id');
        } else {
            return;
        }

        var urlServicioAutocompletar = $('input.inpt_urlServicioAutocompletar').val();
        var bool_esMyGnoss = $('input.inpt_bool_esMyGnoss').val() == 'True';
        var bool_estaEnProyecto = $('input.inpt_bool_estaEnProyecto').val() == 'True';
        var bool_esUsuarioInvitado = $('input.inpt_bool_esUsuarioInvitado').val() == 'True';
        if (facetasAutoComTip == '') {
		 
            facetasAutoComTip = $('input.inpt_FacetasProyAutoCompBuscadorCom').val();
        }
        //var tablaPropiaAutoCompletar = $('input.inpt_tablaPropiaAutoCompletar').val().toLowerCase() == "true";
        var autocompletarProyectoVirtuoso = $('input.inpt_AutocompletarProyectoVirtuoso').val();

        $(this).unautocomplete();
        if (ddlCategorias.val() != 'MyGNOSSMeta' && ddlCategorias.val() != 'Contribuciones en Recursos' && ddlCategorias.val() != 'RecursoPerfilPersonal' && ddlCategorias.val() != 'Mensaje' && ddlCategorias.val() != 'Blog' && ddlCategorias.val() != 'Comunidad' && (ddlCategorias.val() != 'PerYOrg' || proyID != '11111111-1111-1111-1111-111111111111') && typeof (autocompletarProyectoVirtuoso) == 'undefined') {
            $(this).autocomplete(
                null,
                {
                    servicio: new WS(urlServicioAutocompletarEtiquetas, WSDataType.jsonp),
                    metodo: 'AutoCompletarTipado',
                    delay: 0,
                    scroll: false,
                    selectFirst: false,
                    minChars: 1,
                    width: 'auto',
                    max: 25,
                    cacheLength: 0,
                    extraParams: {
                        pProyecto: proyID,
                        //pTablaPropia: tablaPropiaAutoCompletar,
                        pFacetas: facetasAutoComTip,
                        pOrigen: pOrigen,
                        pIdentidad: identidadID,
                        pIdioma: $('input.inpt_Idioma').val(),
                        maxwidth: '389px',
                        botonBuscar: btnBuscarID
                    }
                });
        } else {
            $(this).autocomplete(
			null,
			{
			    //servicio: new WS(urlServicioAutocompletar, WSDataType.jsonp),
			    //metodo: 'AutoCompletarFacetas',
			    url: urlServicioAutocompletar + "/AutoCompletarFacetas",
			    type: "POST",
			    delay: 0,
			    minLength: 4,
			    scroll: false,
			    selectFirst: false,
			    minChars: 4,
			    width: 190,
			    cacheLength: 0,
			    extraParams: {
			        proyecto: proyID,
			        bool_esMyGnoss: bool_esMyGnoss == 'True',
			        bool_estaEnProyecto: bool_estaEnProyecto == 'True',
			        bool_esUsuarioInvitado: bool_esUsuarioInvitado == 'True',
			        identidad: identidadID,
			        organizacion: organizacionID,
			        filtrosContexto: '',
			        languageCode: $('input.inpt_Idioma').val(),
			        perfil: perfilID,
			        nombreFaceta: 'search',
			        orden: '',
			        parametros: '',
			        tipo: pTipoDdlCategorias,
			        botonBuscar: btnBuscarID
			    }
			});
        }
    });
}

var wrap = '';

function ObtenerUrlBusqueda(tipo) {
    var tamagno = $('input.inpt_tipo_busqueda').length;
    for (var i = 0; i < tamagno; i++) {
        var valor = ($('input.inpt_tipo_busqueda')[i]).value;
        if (valor.startsWith("ub_")) {
            valor = valor.substring(3);
        }
        if (valor.split('@')[0] == tipo) {
            return valor.split('@')[1];
        }
    }

    //Devolvemos el por defecto.
    return ($('input.inpt_tipo_busqueda')[tamagno - 1]).value.split('|')[1];
}

function ObtenerOrigenAutoCompletarBusqueda(tipo) {
    var tamagno = $('input.inpt_OrigenAutocompletar').length;
    for (var i = 0; i < tamagno; i++) {
        var valor = ($('input.inpt_OrigenAutocompletar')[i]).value;
        if (valor.startsWith("oa_")) {
            valor = valor.substring(3);
        }
        if (valor.split('@')[0] == tipo) {
            return valor.replace(tipo + '@', '');
        }
    }

    //Devolvemos el por defecto.
    return '';
}

function ObtenerFacetasAutocompletar(tipo) {
    var tamagno = $('input.inpt_FacetasAutocompletar').length;
    for (var i = 0; i < tamagno; i++) {
        var valor = ($('input.inpt_FacetasAutocompletar')[i]).value;
        if (valor.startsWith("fa_")) {
            valor = valor.substring(3);
        }
        if (valor.split('@')[0] == tipo) {
            return valor.replace(tipo + '@', '');
        }
    }

    //Devolvemos el por defecto.
    return '';
}

function MontarPanelFiltros() {
    if (mostrarCajaBusqueda) {
        if (document.getElementById('panelFiltros') != null && document.getElementById('panelFiltros').innerHTML != "" && document.getElementById(panFiltrosPulgarcito) != null) {
            $('#' + panFiltrosPulgarcito).html($('#panelFiltros').html());
            $('#panelFiltros').html('');

            $('.group.filterSpace').css('display', '');
            $('.searchBy').css('display', '');
            $('.tags').css('display', '');
        }
    }
}

function MontarNumResultados() {
    if ($('#' + idNavegadorBusqueda).length > 0) {
        if ($('#navegadorRemover').find('.indiceNavegacion').length > 0) {
            $('#' + idNavegadorBusqueda).html($('#navegadorRemover').html());
            $('#' + idNavegadorBusqueda).css('display', '')
        }
        $('#navegadorRemover').remove();
    }

    if ($('#numResultadosRemover').length > 0) {
        if (mostrarCajaBusqueda) {
            $('#' + numResultadosBusq).html($('#numResultadosRemover').html());
            $('#' + numResultadosBusq).css('display', '');
            $('.group.filterSpace').css('display', '');
        }
        $('#numResultadosRemover').remove();
    }
}

//M?todos para las acciones de los listados de las b?squedas
function ObtenerAccionesListado(jsEjecutar) {
    var resources = $('.resource-list .resource');
    var idPanelesAcciones = '';
    var numDoc = 0;
    resources.each(function () {
        var recurso = $(this);
        var accion = recurso.find('.group.acciones.noGridView');
        if (accion.length == 1) {
            accion.attr('id', accion.attr('id') + '_' + numDoc);
            idPanelesAcciones += accion.attr('id') + ',';
            numDoc++;
        }
        var accionesusuario = recurso.find('.group.accionesusuario.noGridView');
        if (accionesusuario.length == 1) {
            accionesusuario.attr('id', accionesusuario.attr('id') + '_' + numDoc);
            idPanelesAcciones += accionesusuario.attr('id') + ',';
            numDoc++;
        }
    });

    if (jsEjecutar == null) {
	 
        jsEjecutar = "";
    }

    if (idPanelesAcciones != '') {
        try {
		 
            WebForm_DoCallback(UniqueDesplegarID, 'CargarControlDesplegar&ObtenerAcciones&' + idPanelesAcciones + "&jsEjecutar=" + jsEjecutar, ReceiveServerData, '', null, false);
        }
        catch (ex) { }
    } else if (jsEjecutar != null) {
        eval(jsEjecutar);
    }
}

function AccionListadoActivarDesactivar(boton) {
    if (boton.parent().attr('class') == null || boton.parent().attr('class') == '') {
        boton.parent().parent().children().each(function () {
            $(this).attr('class', '');
        });
        boton.parent().attr('class', 'active');
    } else {
        boton.parent().attr('class', '');
    }
}

function AccionListadoCambiarOnClickPorOnclickAux(boton) {
    boton.attr('onclickAux2', $(this).attr('onclick'));
    boton.attr('onclick', $(this).attr('onclickAux'));
    boton.attr('onclickAux', $(this).attr('onclickAux2'));
    boton.removeAttr('onclickAux2');
}


function FormatearFacetasGraficas() {
    //Componentes
    var facetasBarras = $('.componenteFaceta.graficoBarras');
    facetasBarras.each(function () {
        if (!$(this).hasClass("formateado")) {
            //Recorremos cada faceta
            var faceta = $(this);
            PintarGraficoFaceta(faceta, 'barras');
            faceta.addClass("formateado");
        }
    });

    var facetasSectores = $('.componenteFaceta.graficoSectores');
    facetasSectores.each(function () {
        if (!$(this).hasClass("formateado")) {
            //Recorremos cada faceta
            var faceta = $(this);
            PintarGraficoFaceta(faceta, 'sectores');
            faceta.addClass("formateado");
        }
    });
}

function PintarGraficoFaceta(faceta, estilo) {
    faceta.hide();
    var listaElementos = faceta.find('.facetedSearch ul li');
    var titulo = faceta.find('.faceta-title').text();

    var array = new Array();
    array[0] = new Array();
    array[0][0] = 'Propiedad';
    array[0][1] = 'Número';

    var num = 1;

    var arrayEnlaces = new Array();
    var total = 0;
    listaElementos.each(function () {
        //Recorremos cada elemento de la faceta
        var elemento = $(this);

        var enlace = elemento.find('a').attr('href');
        var nombreSinFormato = elemento.find('a').text();

        var nombre = nombreSinFormato.substring(0, nombreSinFormato.indexOf('(')).trim();
        var numero = nombreSinFormato.substring(nombreSinFormato.indexOf('(') + 1);
        numero = numero.substring(0, numero.indexOf(')'));

        array[num] = new Array();
        array[num][0] = nombre;
        array[num][1] = parseFloat(numero);
        arrayEnlaces[num - 1] = enlace;
        num++;
    });

    google.load("visualization", '1.1', { packages: ["corechart"] });
    google.setOnLoadCallback(drawChart);
    function drawChart() {
        var data = google.visualization.arrayToDataTable(array);

        var view = new google.visualization.DataView(data);
        view.setColumns([0, 1,
                    {
                        calc: "stringify",
                        sourceColumn: 1,
                        type: "string",
                        role: "annotation"
                    }]);

        var options = {
            title: titulo
        };

        var idGrafico = guidGenerator();
        faceta.html("<div id='div_cms_faceta" + idGrafico + "'></div>");
        var chart = null;

        if (estilo == 'barras') {
            chart = new google.visualization.BarChart(document.getElementById('div_cms_faceta' + idGrafico));
            options = {
                title: titulo,
                legend: { position: 'none' }
            };
        } else if (estilo == 'sectores') {
            chart = new google.visualization.PieChart(document.getElementById('div_cms_faceta' + idGrafico));
        }
        faceta.show();
        chart.draw(view, options);
        google.visualization.events.addListener(chart, 'select', function () {
            var enlace = '';
            var selection = chart.getSelection();
            for (var i = 0; i < selection.length; i++) {
                var item = selection[i];
                if (estilo == 'barras') {
                    if (item.row != null && item.column != null) {
                        if (item.column == '1') {
                            enlace = arrayEnlaces[item.row];
                        }
                    }
                } else if (estilo == 'sectores') {
                    enlace = arrayEnlaces[item.row];
                }
            }
            if (enlace != '') {
                document.location.href = enlace;
            }
        });
    }
}

function obtenerUrl(service) {
    var url = service;
    if (url.indexOf(',') != -1) {
        var urlMultiple = url.split(',');
        if (indicesWS[service] == null) {
		 
            indicesWS[service] = null;
        }
        if (indicesWS[service] == null) {
		 
            indicesWS[service] = this.aleatorio(0, urlMultiple.length - 1);
        } else if (indicesWS[service] > urlMultiple.length - 1) {
		 
            indicesWS[service] = 0;
        }
        url = urlMultiple[indicesWS[service]];
        indicesWS[service]++;
    }
    return url;
}

function aleatorio(inferior, superior) {
    numPosibilidades = superior - inferior;
    aleat = Math.random() * numPosibilidades;
    aleat = Math.round(aleat);
    return parseInt(inferior) + aleat;
}