
DeadSimpleRenderer = function (canvas) {
    var canvas = $(canvas).get(0)
    var ctx = canvas.getContext("2d");
    //var ctx = arbor.Graphics(canvas);
    var particleSystem = null
    var sys = null;

    var that = {
        //
        // the particle system will call the init function once, right before the
        // first frame is to be drawn. it's a good place to set up the canvas and
        // to pass the canvas size to the particle system
        //
        init: function (system) {
            // save a reference to the particle system for use in the .redraw() loop
            particleSystem = system

            particleSystem.parameters().repulsion = 15000;
            particleSystem.parameters().gravity = true;

            // inform the system of the screen dimensions so it can map coords for us.
            // if the canvas is ever resized, screenSize should be called again with
            // the new dimensions
            particleSystem.screenSize(canvas.width, canvas.height)
            //particleSystem.screenPadding(80) // leave an extra 80px of whitespace per side
            particleSystem.screenPadding(20,80,20,20) // leave an extra 80px of whitespace per side
        },

        // 
        // redraw will be called repeatedly during the run whenever the node positions
        // change. the new positions for the nodes can be accessed by looking at the
        // .p attribute of a given node. however the p.x & p.y values are in the coordinates
        // of the particle system rather than the screen. you can either map them to
        // the screen yourself, or use the convenience iterators .eachNode (and .eachEdge)
        // which allow you to step through the actual node objects but also pass an
        // x,y point in the screen's coordinate system
        // 

        redraw: function () {
            pintados++;

            if ((pintados > 300)) {
                particleSystem.stop();
                return;
            }

            ctx.clearRect(0, 0, canvas.width, canvas.height)
            //gfx.clear();
            flechas = [];

            particleSystem.eachEdge(function (edge, pt1, pt2) {
                // edge: {source:Node, target:Node, length:#, data:{}}
                // pt1:  {x:#, y:#}  source position in screen coords
                // pt2:  {x:#, y:#}  target position in screen coords

                if (typeof PintarUnionGrafoPersonalizadoCom != 'undefined') {
                    return PintarUnionGrafoPersonalizadoCom(ctx, edge, pt1, pt2);
                }

                // draw a line from pt1 to pt2
                //ctx.strokeStyle = "rgba(255,255,255, .333)"
                ctx.strokeStyle = "black"
                //ctx.lineWidth = 1 + 4 * edge.data.weight
                ctx.lineWidth = 1;

                if (flechaSeleccionada.length > 0 && flechaSeleccionada[0] === edge.source && flechaSeleccionada[1] === edge.target)
                {
                    ctx.lineWidth = 2;
                }

                ctx.beginPath()
                ctx.moveTo(pt1.x, pt1.y)
                ctx.lineTo(pt2.x, pt2.y)
                ctx.stroke()

                flechas.push({ edge: edge, tail: pt1, head: pt2 });
            })

            particleSystem.eachNode(function (node, pt) {
                // node: {mass:#, p:{x,y}, name:"", data:{}}
                // pt:   {x:#, y:#}  node position in screen coords

                if (typeof PintarNodoGrafoPersonalizadoCom != 'undefined') {
                    return PintarNodoGrafoPersonalizadoCom(ctx, node, pt);
                }

                // draw a rectangle centered at pt
                /*var w = 10

                var centerX = pt.x - w / 2;
                var centerY = pt.y - w / 2;*/
                var centerX = pt.x;
                var centerY = pt.y;
                var entPrinc = (node.data.nivel == 0);//Comprobamos si es la entidad principal.

                var radius = 10;
                
                if (entPrinc) {
                    radius = 50;
                }

                node.data.geomet = 'circle';
                node.data.radio = radius;

                ctx.beginPath();
                ctx.arc(centerX, centerY, radius, 0, 2 * Math.PI, false);
                //ctx.fillStyle = node.data.color;
                ctx.fillStyle = 'white';
                ctx.fill();
                ctx.lineWidth = 1;
                ctx.strokeStyle = 'black';

                if (nodoSeleccionado != null && nodoSeleccionado === node) {
                    ctx.lineWidth = 2;
                }

                ctx.stroke();

                var letra = radius / 4;

                if (letra < 8) {
                    letra = 8;
                }

                ctx.font = letra + 'pt Calibri';
                ctx.fillStyle = 'black';

                if (entPrinc) {
                    ctx.textAlign = 'center';
                    ctx.fillText(titDoc, centerX, centerY + 3);
                }
                else if (node.data.numElemGrupo != null) {
                    ctx.textAlign = 'center';
                    ctx.fillText(node.data.numElemGrupo, centerX, centerY + 3);
                }
                else {
                    ctx.textAlign = 'left';
                    ajusteDeTexto(ctx, ObtenerTextoDeIdiomaGrafos(node.data.titulo), centerX + radius + 5, centerY + 3, 100, 12);
                }

            })
        }
    }
    return that
}

var docActualID = null;
var titDoc = null;
var propEnlace = null;
var tooltip = null;
var lastEvent = null;
var dataGrafo = null;
var urlBusqGrafo = null;
var urlBusqGrafoDbpedia = null;
var flechas = [];
var ultPropVista = null;
var extraDatosOnto = null;
var nodoSeleccionado = null;
var flechaSeleccionada = [];
var pintados = 0;

function MontarGrafoFicRec(docID, titulo, grafoID, propEnl, nodosLimNivel, urlIntra, extra, urlBusqueda, tipoRec, pUrlBusquedaGrafoDbpedia) {
    docActualID = docID;
    titDoc = ObtenerTextoDeIdiomaGrafos(titulo);
    propEnlace = propEnl;
    urlBusqGrafo = urlBusqueda;
    if (pUrlBusquedaGrafoDbpedia != undefined && pUrlBusquedaGrafoDbpedia != null && pUrlBusquedaGrafoDbpedia != '') {
        urlBusqGrafoDbpedia = pUrlBusquedaGrafoDbpedia;
    }
    else {
        urlBusqGrafoDbpedia = urlBusqueda;
    }



    if ($('#divContGrafo').length == 0) {
        if ($('.resource .wrapDescription .formSemLectura').length > 0) {
            $('.resource .wrapDescription .formSemLectura').after('<div id="divContGrafo"></div>');
        }
        else if ($('.resource .wrapDescription .categorias').length > 0) {
            $('.resource .wrapDescription .categorias').before('<div id="divContGrafo"></div>');
        }
        else {
            $('.resource .wrapDescription').after('<div id="divContGrafo"></div>');
        }
    }

    $('#divContGrafo').html($('.popup').html());

    //Petición CallBack
    //PeticionesAJAX.ObtenerRelacionesGrafoFichRec(docID, grafoID, propEnl, nodosLimNivel, urlIntra, extra, $('input.inpt_Idioma').val(), tipoRec, FinTraerDatosGrafoAJAX, FinTraerDatosGrafoAJAX);
}

function FinTraerDatosGrafoAJAX(datos) {
    if (typeof FinTraerDatosGrafoAJAXPersonalizado != 'undefined') {
        FinTraerDatosGrafoAJAXPersonalizado(datos);
        return;
    }

    if (datos != '') {
        //Creo el canvas:
        $('#divContGrafo').html('<canvas width="' + $('.resource .wrapDescription').width() + '" height="600" id="can_' + docActualID + '"></canvas>');

        PintarGrafo('can_' + docActualID, datos);
    }
    else {
        $('#divContGrafo').html('');
    }
}

function PintarGrafo(canvasID, datos) {
    try {
        var canvas = $('#' + canvasID);

        sys = arbor.ParticleSystem(1000, 800, 0.5) // create the system with sensible repulsion/stiffness/friction
        sys.renderer = DeadSimpleRenderer("#" + canvasID) // our newly created renderer will have its .init() method called shortly by sys...
    
        var data = eval('(' + datos + ')');    

        if (typeof TratarDatosNodoGrafoPersonalizado != 'undefined') {
            data = TratarDatosNodoGrafoPersonalizado(data);
        }

        dataGrafo = data;

        sys.graft({ nodes: data.nodes, edges: data.edges })

        ActivarToopTip(canvasID);
    } catch (e) { }
}


function ActivarToopTip(canvasID)
{
    var canvas = $('#' + canvasID);

    if ($('#qtip-fullcalendar').length == 0) {
        tooltip = $('<div/>').qtip({
            id: 'fullcalendar',
            prerender: true,
            content: {
                text: ' ',
                title: {
                    button: true
                }
            },
            position: {
                my: 'bottom center',
                at: 'top center',
                target: 'mouse',
                viewport: $('#' + canvasID),
                adjust: {
                    mouse: false,
                    scroll: false
                }
            },
            show: false,
            hide: false,
            style: 'qtip-light'
        }).qtip('api');
    }

	 
    $(canvas).mousedown(function (e) {
        if (typeof ClickNodoGrafoPersonalizado != 'undefined') {
            return ClickNodoGrafoPersonalizado(e);
        }

        var pos = $(this).offset();
        var p = {x:e.pageX-pos.left, y:e.pageY-pos.top}
        selected = nearest = dragged = sys.nearest(p);

        if (selected.node !== null){
			//// dragged.node.tempMass = 10000
            //dragged.node.fixed = true;
				
            if ((nearest.node.data.geomet == 'circle' && nearest.distance <= nearest.node.data.radio) || (nearest.node.data.geomet == 'rectangle'
                && ((p.x > nearest.node.data.cordX && p.x < (nearest.node.data.cordX + (nearest.node.data.longX / 2))) || (p.x < nearest.node.data.cordX && p.x > (nearest.node.data.cordX - (nearest.node.data.longX / 2))))
                && ((p.y > nearest.node.data.cordY && p.y < (nearest.node.data.cordY + (nearest.node.data.longY / 2))) || (p.y < nearest.node.data.cordY && p.y > (nearest.node.data.cordY - (nearest.node.data.longY / 2)))))) {
				//alert(nearest.node.name);
				PintarHtmlToopTip(nearest.node, e);
            }
            else //Miro flechas
            {
                var margenError = 6;

                for (var i = 0; i < flechas.length; i++) {

                    if (((flechas[i].tail.x > flechas[i].head.x && p.x < flechas[i].tail.x && p.x > flechas[i].head.x)
                        || (flechas[i].tail.x < flechas[i].head.x && p.x > flechas[i].tail.x && p.x < flechas[i].head.x))
                        && ((flechas[i].tail.y > flechas[i].head.y && p.y < flechas[i].tail.y && p.y > flechas[i].head.y)
                        || (flechas[i].tail.y < flechas[i].head.y && p.y > flechas[i].tail.y && p.y < flechas[i].head.y))) {

                        var difx = flechas[i].tail.x - flechas[i].head.x;
                        var dify = flechas[i].tail.y - flechas[i].head.y;
                        if (difx < 0) { difx = difx * -1 }
                        if (dify < 0) { dify = dify * -1 }

                        if (difx > dify) {
                            var interseccion = checkLineIntersection(p.x, p.y, p.x, flechas[i].head.y, flechas[i].tail.x, flechas[i].tail.y, flechas[i].head.x, flechas[i].head.y);
                            dify = p.y - interseccion.y;

                            if (dify < 0) { dify = dify * -1 }

                            if (dify < margenError) {
                                //alert('Flecha de ' + flechas[i].edge.source.name + ' a ' + flechas[i].edge.target.name);
                                PintarHtmlRelacionToopTip(flechas[i], e);
                                return false;
                            }
                        }
                        else {
                            var interseccion = checkLineIntersection(p.x, p.y, p.y, flechas[i].head.x, flechas[i].tail.x, flechas[i].tail.y, flechas[i].head.x, flechas[i].head.y);
                            difx = p.x - interseccion.x;

                            if (difx < 0) { difx = difx * -1 }

                            if (difx < margenError) {
                                //alert('Flecha de ' + flechas[i].edge.source.name + ' a ' + flechas[i].edge.target.name);
                                PintarHtmlRelacionToopTip(flechas[i], e);
                                return false;
                            }
                        }
                    }
                }
            }
        }
        return false;
    });

    //$('.resource .wrapDescription .categorias').before('<label id="ratx"></label><label id="raty"></label>');

    $(canvas).mousemove(function (e) {
        if (typeof MoveGrafoPersonalizado != 'undefined') {
            return MoveGrafoPersonalizado(e);
        }

        var pos = $(this).offset();
        var p = {x:e.pageX-pos.left, y:e.pageY-pos.top}
        selected = nearest = dragged = sys.nearest(p);

        body.css('cursor', '');

        //$('#ratx').html('x: ' + p.x);
        //$('#raty').html(' y: ' + p.y);

        if (nodoSeleccionado != null || flechaSeleccionada.length > 0) {
            nodoSeleccionado = null;
            flechaSeleccionada = [];
            pintados = 0;
            sys.renderer.redraw();
        }

        if (selected != null && selected.node !== null){
				
            if ((nearest.node.data.geomet == 'circle' && nearest.distance <= nearest.node.data.radio) || (nearest.node.data.geomet == 'rectangle'
                && ((p.x > nearest.node.data.cordX && p.x < (nearest.node.data.cordX + (nearest.node.data.longX / 2))) || (p.x < nearest.node.data.cordX && p.x > (nearest.node.data.cordX - (nearest.node.data.longX / 2))))
                && ((p.y > nearest.node.data.cordY && p.y < (nearest.node.data.cordY + (nearest.node.data.longY / 2))) || (p.y < nearest.node.data.cordY && p.y > (nearest.node.data.cordY - (nearest.node.data.longY / 2))))))
            {
                //alert('Encima nodo: ' + nearest.node.name);
                body.css('cursor', 'pointer');
                nodoSeleccionado = nearest.node;
                pintados = 0;
                sys.renderer.redraw();
                return false;
            }
            else //Miro flechas
            {
                var margenError = 6;

                for (var i = 0; i < flechas.length; i++) {

                    if (((flechas[i].tail.x > flechas[i].head.x && p.x < flechas[i].tail.x && p.x > flechas[i].head.x)
                        || (flechas[i].tail.x < flechas[i].head.x && p.x > flechas[i].tail.x && p.x < flechas[i].head.x))
                        && ((flechas[i].tail.y > flechas[i].head.y && p.y < flechas[i].tail.y && p.y > flechas[i].head.y)
                        || (flechas[i].tail.y < flechas[i].head.y && p.y > flechas[i].tail.y && p.y < flechas[i].head.y))) {

                        var difx = flechas[i].tail.x - flechas[i].head.x;
                        var dify = flechas[i].tail.y - flechas[i].head.y;
                        if (difx < 0) { difx = difx * -1 }
                        if (dify < 0) { dify = dify * -1 }

                        if (difx > dify) {
                            var interseccion = checkLineIntersection(p.x, p.y, p.x, flechas[i].head.y, flechas[i].tail.x, flechas[i].tail.y, flechas[i].head.x, flechas[i].head.y);
                            dify = p.y - interseccion.y;

                            if (dify < 0) { dify = dify * -1 }

                            if (dify < margenError) {
                                //alert('Flecha de ' + flechas[i].edge.source.name + ' a ' + flechas[i].edge.target.name);
                                body.css('cursor', 'pointer');
                                flechaSeleccionada = [];
                                flechaSeleccionada.push(flechas[i].edge.source);
                                flechaSeleccionada.push(flechas[i].edge.target);
                                pintados = 0;
                                sys.renderer.redraw();
                                return false;
                            }
                        }
                        else {
                            var interseccion = checkLineIntersection(p.x, p.y, p.y, flechas[i].head.x, flechas[i].tail.x, flechas[i].tail.y, flechas[i].head.x, flechas[i].head.y);
                            difx = p.x - interseccion.x;

                            if (difx < 0) { difx = difx * -1 }

                            if (difx < margenError) {
                                //alert('Flecha de ' + flechas[i].edge.source.name + ' a ' + flechas[i].edge.target.name);
                                body.css('cursor', 'pointer');
                                flechaSeleccionada = [];
                                flechaSeleccionada.push(flechas[i].edge.source);
                                flechaSeleccionada.push(flechas[i].edge.target);
                                pintados = 0;
                                sys.renderer.redraw();
                                return false;
                            }
                        }
                    }
                }
            }

            
        }
        return false;
    });
}

function PintarHtmlRelacionToopTip(flecha, event) {
    if (typeof PintarHtmlRelacionToopTipPersonalizado != 'undefined') {
        return PintarHtmlRelacionToopTipPersonalizado(flecha, event);
    }

    PintarHtmlCargandoGrafo(event);
    TraerRecPropiedadGrafo(flecha.edge);
}

function PintarHtmlToopTip(nodo, event) {
    if (typeof PintarHtmlToopTipPersonalizado != 'undefined') {
        return PintarHtmlToopTipPersonalizado(nodo, event);
    }

    if (nodo.data.numElemGrupo != null) {
        if (nodo.data.nombreSubTipo == null) {
            tooltip.set({
                'content.text': '<p><a target="_blank" href="' + urlBusqGrafo + '?' + nodo.data.propRelGrupo + '=' + nodo.data.sujRelGrupo + '">' + textoRecursos.VerRecVincGrafoRec.replace('@1@', nodo.data.numElemGrupo) + '</a></p>'
            });
        }
        else {
            if (nodo.data.GuidsMostrarPop != 'undefined' && nodo.data.GuidsMostrarPop != '') {
                PintarHtmlCargandoGrafo(event);
                TraerRecPuntoGrafo(nodo.data.GuidsMostrarPop, nodo.data.nombreSubTipo);
            }
            else {
                tooltip.set({
                    'content.text': '<p><a target="_blank" href="' + urlBusqGrafoDbpedia + '?' + nodo.data.propRelGrupo + '=' + nodo.data.sujRelGrupo + '&' + nodo.data.proNombreSubTipo + '=' + nodo.data.nombreSubTipo + '@' + $('input.inpt_Idioma').val() + '">' + textoRecursos.VerRecVincGrafoRecSubTipo.replace('@1@', nodo.data.numElemGrupo).replace('@2@', nodo.data.nombreSubTipo) + '</a></p>'
                });
            }
        }
        tooltip.reposition(event);
        tooltip.show(event);
        lastEvent = event;
    }
    else {
        PintarHtmlCargandoGrafo(event);
        TraerRecPuntoGrafo(nodo.name);
    }
}

function PintarHtmlCargandoGrafo(event) {
    if (typeof PintarHtmlCargandoGrafoPersonalizado != 'undefined') {
        PintarHtmlCargandoGrafoPersonalizado(event);
    } else {
        tooltip.set({
            'content.text': '<p>' + form.cargando + '</p>'
        });
        tooltip.reposition(event);
        tooltip.show(event);
        lastEvent = event;
    }
}

function TraerRecPropiedadGrafo(conexion) {
    var ontologiaID = '';

    if (extraDatosOnto != null && extraDatosOnto.indexOf('|' + conexion.source.data.tipo + ',') != -1) {
        ontologiaID = extraDatosOnto.substring(extraDatosOnto.indexOf('|' + conexion.source.data.tipo + ',') + conexion.source.data.tipo.length + 2);
        ontologiaID = ontologiaID.substring(0, ontologiaID.indexOf('|'));
    }

    ultPropVista = conexion.data.propConexion;
    var arg = {};

    arg.callback = 'obtenernombrepropiedadgrafo',
    arg.ontologia = ontologiaID;
    arg.proyectoID = $('input.inpt_proyID').val();
    arg.propConexion = ultPropVista;
    arg.tipoEntidad = conexion.source.data.tipo;
    arg.subTiposEntidad = conexion.source.data.subTipos;
    arg.idioma = $('input.inpt_Idioma').val();

    GnossPeticionAjax(urlPaginaCallBackGrafos, arg, true).done(function (data) {
        FinTraerNombrePropGrafoAJAX(data);
    }).fail(function (data) {
        FinTraerNombrePropGrafoAJAX(data);
    });
}

function FinTraerNombrePropGrafoAJAX(datos) {
    if (datos != '' && datos.indexOf('[|Extra|]') != -1) {
        extraDatosOnto = datos.substring(0, datos.indexOf('[|Extra|]'));
        datos = datos.substring(datos.indexOf('[|Extra|]') + '[|Extra|]'.length);
    }

    if (datos == '') {
        tooltip.set({
            'content.text': '<div class="propLink"><p>' + ultPropVista + '</p></div>'
        });
    }
    else {
        tooltip.set({
            'content.text': '<div class="propLink"><p>' + datos + '</p></div>'
        });
    }

    tooltip.reposition(lastEvent);
    tooltip.show(lastEvent);

    if ((typeof CompletadaCargaFichaGrafo != 'undefined')) {
        CompletadaCargaFichaGrafo();
    }
}

function TraerRecPuntoGrafo(docID, nombreSubTipo) {
    var divAgrupacion = '';
    var divCierreAgrupacion = '';
    var numAgrupacion = 0;
    if (docID.includes(',')) {
               
        var arraydocID = docID.split(',');
        docID = '';
        for (var i = 0; i < arraydocID.length; i++) {
            if (arraydocID[i].indexOf('_bis_') != -1) {
                arraydocID[i] = arraydocID[i].substring(0, arraydocID[i].indexOf('_bis_'));
            }

            docID = docID + arraydocID[i].substring(arraydocID[i].lastIndexOf('/') + 1) + ',';
            numAgrupacion = numAgrupacion + 1;
        }
        docID = docID.slice(0, -1);
        if (nombreSubTipo != undefined)
        {
            divAgrupacion = '<div class="fichaMapa multiple"> <p class="titulo">' + numAgrupacion + mensajes.agrupacionGrafoTitulo + nombreSubTipo + '</p>';
            divCierreAgrupacion = '</div>';
        }
        else
        {
            divAgrupacion = '<div class="fichaMapa multiple"> <p class="titulo">' + numAgrupacion + mensajes.agrupacionGrafoTitulo + '</p>';
            divCierreAgrupacion = '</div>';
        }
        
    }
    else {
        if (docID.indexOf('_bis_') != -1) {
            docID = docID.substring(0, docID.indexOf('_bis_'));
        }

        docID = docID.substring(docID.lastIndexOf('/') + 1);
    }

    var contResultados = '';

    var metodo = $('input.inpt_UrlServicioResultados').val() + '/ObtenerFichaRecurso';
    var params = {};

    params['proyecto'] = $('input.inpt_proyID').val();

    if ($('input.inpt_proyIDDbpedia').val() != undefined) {
        params['proyecto'] = $('input.inpt_proyIDDbpedia').val();
    }

    params['identidad'] = $('input.inpt_identidadID').val();
    params['languageCode'] = $('input.inpt_Idioma').val();
    params['cont'] = contResultados;
    params['documentoID'] = docID;
    params['urlBusqueda'] = urlBusqGrafoDbpedia;

    GnossPeticionAjax(metodo, params, true).done(function (data) {
        PintarHtmlToopTipDef(divAgrupacion + JSON.parse(data).d + divCierreAgrupacion)
    });
}

function PintarHtmlToopTipDef(nuevoHtml) {
    tooltip.set({
        'content.text': nuevoHtml
    });
    tooltip.reposition(lastEvent);
    tooltip.show(lastEvent);

    if ((typeof CompletadaCargaFichaGrafo != 'undefined')) {
        CompletadaCargaFichaGrafo();
    }
}

function CompletadaCargaFichaGrafo() {
    if ((typeof CompletadaCargaFichaGrafoComunidad != 'undefined')) {
        CompletadaCargaFichaGrafoComunidad();
    }
}


function checkLineIntersection(line1StartX, line1StartY, line1EndX, line1EndY, line2StartX, line2StartY, line2EndX, line2EndY) {
    // if the lines intersect, the result contains the x and y of the intersection (treating the lines as infinite) and booleans for whether line segment 1 or line segment 2 contain the point
    var denominator, a, b, numerator1, numerator2, result = {
        x: null,
        y: null,
        onLine1: false,
        onLine2: false
    };
    denominator = ((line2EndY - line2StartY) * (line1EndX - line1StartX)) - ((line2EndX - line2StartX) * (line1EndY - line1StartY));
    if (denominator == 0) {
        return result;
    }
    a = line1StartY - line2StartY;
    b = line1StartX - line2StartX;
    numerator1 = ((line2EndX - line2StartX) * a) - ((line2EndY - line2StartY) * b);
    numerator2 = ((line1EndX - line1StartX) * a) - ((line1EndY - line1StartY) * b);
    a = numerator1 / denominator;
    b = numerator2 / denominator;

    // if we cast these lines infinitely in both directions, they intersect here:
    result.x = line1StartX + (a * (line1EndX - line1StartX));
    result.y = line1StartY + (a * (line1EndY - line1StartY));
    /*
            // it is worth noting that this should be the same as:
            x = line2StartX + (b * (line2EndX - line2StartX));
            y = line2StartX + (b * (line2EndY - line2StartY));
            */
    // if line1 is a segment and line2 is infinite, they intersect if:
    if (a > 0 && a < 1) {
        result.onLine1 = true;
    }
    // if line2 is a segment and line1 is infinite, they intersect if:
    if (b > 0 && b < 1) {
        result.onLine2 = true;
    }
    // if line1 and line2 are segments, they intersect if both of the above are true
    return result;
}

function ajusteDeTexto(ctx, texto, x, y, maxWidth, alturaDeLinea) {
    // crea el array de las palabras del texto
    var palabrasRy = texto.split(" ");
    // inicia la variable var lineaDeTexto
    var lineaDeTexto = "";
    // un bucle for recorre todas las palabras
    for (var i = 0; i < palabrasRy.length; i++) {
        var testTexto = lineaDeTexto + palabrasRy[i] + " ";
        // calcula la anchura del texto textWidth
        var textWidth = ctx.measureText(testTexto).width;
        // si textWidth > maxWidth
        if (textWidth > maxWidth && i > 0) {
            // escribe en el canvas la lineaDeTexto
            ctx.fillText(lineaDeTexto, x, y);
            // inicia otra lineaDeTexto         
            lineaDeTexto = palabrasRy[i] + " ";
            // incrementa el valor de la variable y
            //donde empieza la nueva lineaDeTexto
            y += alturaDeLinea;
        } else {// de lo contrario, si textWidth <= maxWidth
            lineaDeTexto = testTexto;
        }
    }// acaba el bucle for
    // escribe en el canvas la última lineaDeTexto
    ctx.fillText(lineaDeTexto, x, y);
}


var intersect_line_box = function (p1, p2, boxTuple) {
    if (boxTuple.data.cordX == null) {
        return null;
    }

    if (boxTuple.data.geomet == 'circle') {
        var x1 = p1.x - p2.x;
        var y1 = p1.y - p2.y;

        var div = y1 / x1;
        var x = Math.sqrt((boxTuple.data.radio * boxTuple.data.radio) / (1 + (div * div)))
        var y = Math.sqrt((boxTuple.data.radio * boxTuple.data.radio) - (x * x));

        if (p1.x < p2.x) {
            x = x * -1;
        }

        if (p1.y < p2.y) {
            y = y * -1;
        }

        x += p2.x;
        y += p2.y;

        return { x: x, y: y };
    }
    else {
        var newX = p2.x;
        var newY = p2.y;
        var difX = (p2.x - p1.x);
        var difY = (p2.y - p1.y);

        if (difX < 0) {
            difX = difX * -1;
        }

        if (difY < 0) {
            difY = difY * -1;
        }

        var lineaAux = null;
        var medioX = (boxTuple.data.longX / 2);
        var medioY = (boxTuple.data.longY / 2);

        if (difX > difY) {
            if (p2.x > p1.x) {
                newX = p2.x - medioX;
            }
            else {
                newX = p2.x + (boxTuple.data.longX / 2);
            }

            lineaAux = { x1: newX, y1: (newY + medioY), x2: newX, y2: (newY - medioY) };
        }
        else {
            if (p2.y > p1.y) {
                newY = p2.y - medioY;
            }
            else {
                newY = p2.y + medioY;
            }

            lineaAux = { x1: (newX - medioX), y1: newY, x2: (newX + medioX), y2: newY };
        }

        var punto = checkLineIntersection(lineaAux.x1, lineaAux.y1, lineaAux.x2, lineaAux.y2, p1.x, p1.y, p2.x, p2.y);

        return { x: punto.x, y: punto.y };
    }
}


var intersect_line_box2 = function (p1, p2, boxTuple) {
    if (boxTuple.data.cordX == null) {
        return null;
    }

    var newX = p2.x;
    var newY = p2.y;
    var difX = (p2.x - p1.x);
    var difY = (p2.y - p1.y);

    if (difX < 0) {
        difX = difX * -1;
    }

    if (difY < 0) {
        difY = difY * -1;
    }

    if (difX > difY) {
        if (p2.x > p1.x) {
            newX = p2.x - (boxTuple.data.longX / 2);
        }
        else {
            newX = p2.x + (boxTuple.data.longX / 2);
        }
    }
    else {
        if (p2.y > p1.y) {
            newY = p2.y - (boxTuple.data.longY / 2);
        }
        else {
            newY = p2.y + (boxTuple.data.longY / 2);
        }
    }

    return { x: newX, y: newY };
}

function ObtenerTextoDeIdiomaGrafos(pTexto)
{
    if (pTexto != null)
    {
        var pIdioma = $('input.inpt_Idioma').val();
        var nombres = pTexto.split('|||');

        var nombreFinal = '';

        for (var i=0;i<nombres.length;i++)
        {
            var nombre = nombres[i];

            if (nombre != '') {
                if (nombre.indexOf("@" + pIdioma) == nombre.length - 3) {
                    //Si es el idioma elegido lo seleccionamos y acabamos
                    nombreFinal = nombre.substring(0, nombre.length - 3);
                    break;
                }
                else if (nombreFinal == '') {
                    //Si aun no tiene ningún idioma seleccionado lo seleccionamos
                    if (nombre.length > 3 && nombre.substring(nombre.length - 3)[0] == "@") {
                        nombreFinal = nombre.substring(0, nombre.length - 3);
                    }
                    else {
                        nombreFinal = nombre;
                    }
                }
            }
        }

        return nombreFinal;
    }

    return pTexto;
}