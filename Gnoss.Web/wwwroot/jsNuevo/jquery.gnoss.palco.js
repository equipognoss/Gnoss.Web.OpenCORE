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
 * Date: 15.10.2013.12.30 
 */

var listaDominiosSinPalco;

function operativaLogin(){
	console.log('operativaLogin');
	return;
}
function operativaGuardarEspacioPersonal(enlace, id){
	var body = $('body');
	if(body.hasClass('invitado')) operativaLogin();
	console.log('operativaGuardarEspacioPersonal');
	return;
}
function operativaMeGusta(enlace, id){
	console.log('operativaMeGusta');
	return;
}	
function operativaComentar(enlace, id){
	console.log('operativaComentar');
	return;
}

var abreEnVentanaNueva = {
    isFicha: false,
    idFichaCatalogo: 'fichaCatalogo',
    idFichaRecurso: 'fichaRecurso',
    isUsuarioInvitado: true,
    margen: 85,
    isOperativaLogin: false,
    init: function () {
        this.body = $('body');
        this.header = this.body.find('#header');
        if (this.body.hasClass(this.idFichaCatalogo) || this.body.hasClass(this.idFichaRecurso)) this.isFicha = true;
        if (!this.isFicha) return;
        this.config();
        this.comportamiento();
        return;
    },
    config: function () {
        this.recurso = this.body.find('#col02 .resource').first();
        this.title = this.recurso.find('.title');
        this.enlace = this.title.find('a');
        this.isUsuarioInvitado = this.body.hasClass('invitado');
        this.panels = this.recurso.find('#panels');
        this.operativaPanels = this.panels.parent();
        this.operativaPanelsPrevio = this.operativaPanels.prev();
        this.tipoRecurso = this.recurso.find('p.resourceType');
        return;
    },
    montarBotonCerrarPalco: function () {
        var that = this;
        this.cerrar = $('<a />').attr('href', '#').addClass('palco-cerrar').text('cerrar');
        this.palcoWrapHeader.append(this.cerrar);
        this.cerrar.bind('click', function (evento) {
            $('#section').append($('#mascaraBlanca'));
            $('#section').append($('.popup'));
            evento.preventDefault();
            if (that.isOperativaLogin) {
                var login = that.palcoLogin.children().hide();
                that.body.prepend(login);
            }
            that.operativaPanelsPrevio.after(that.operativaPanels);
            that.body.removeClass('activo');
            that.body.children().removeClass('palco-items');
            that.palco.remove();
            herramientasRecursoCompactado.init();
        })
        return;
    },
    montarLogoComunidad: function () {
        var logo = this.header.find('.logoCustomRIAM').clone();
        this.palcoWrapHeader.append(logo);
        return;
    },
    montarUsuarioPublicador: function () {
        var autor = this.recurso.find('.author').first().clone();
        this.palcoWrapHeader.append(autor);
        return;
    },
    montarTituloRecurso: function () {
        var titulo = this.recurso.find('.title').first().clone();
        var nombre = titulo.find('h1 a').text();
        var numCaracteresMax = 80;
        if (nombre.length > numCaracteresMax) {
            nombre = nombre.substring(0, numCaracteresMax) + '...';
        }
        titulo = $('<h1 />').text(nombre);
        this.palcoWrapHeader.append(titulo);
        return;
    },
    montarVisitas: function () {
        var visitas = this.recurso.find('.visitas').first().clone();
        visitas.addClass('group');
        this.palcoWrapHeader.append(visitas);
        return;
    },
    montarRedesSociales: function () {
        var contenedorRedesSociales = $('<div />').addClass('resource');
        var redes = this.recurso.find('.redesSocialesCompartir').first().clone();
        redes.addClass('group');
        var enlace = redes.find('.mostrarMas');
        enlace.bind('click', function (evento) {
            evento.preventDefault();
            var enlace = $(evento.target);
            if (!enlace.parent().hasClass('menos')) {
                var lis = enlace.parent().parent().find('li');
                lis.each(function () {
                    var li = $(this);
                    if (!li.hasClass('big') && !li.hasClass('mostrar')) li.show();
                });
                this.isRedesOcultas = false;
                enlace.parent().addClass('menos');
            } else {
                var lis = enlace.parent().parent().find('li');
                lis.each(function () {
                    var li = $(this);
                    if (!li.hasClass('big') && !li.hasClass('mostrarMas')) li.hide();
                });
                this.isRedesOcultas = true;
                enlace.parent().removeClass('menos');
            }
            return;
        })
        contenedorRedesSociales.append(redes)
        this.palcoWrapHeader.append(contenedorRedesSociales);
        return;
    },
    montarHerramientas: function () {
        var herramientas = this.recurso.find('#customAboutResource').first();
        if (herramientas.size() <= 0) return;
        var accionesPalco = $('.group.palco-tools-header');
        accionesPalco.remove();
        this.tools = $('<div />').addClass('group palco-tools-header');
        var ulTools = $('<ul />').addClass('opPrincipal');
        var liComentar = $('<li />').addClass('opComentar');
        var aComentar = $('<a />').attr('href', '#').text('Comentar');
        liComentar.append(aComentar);
        var opcionAddEspacioPersonal = herramientas.find('.opAddPersonal').clone();
        ulTools.append(opcionAddEspacioPersonal);
        var comentariosDisponibles = $('.oculto.comentariosDisponibles');
        if (comentariosDisponibles.length > 0) {
            ulTools.append(liComentar);
        }
        this.tools.append(ulTools);
        this.palcoHeader.append(this.tools);
        this.engancharHerramientas();
        return;
    },
    montarVotos: function () {
        if (this.tools != undefined) {
            var itemVotos = this.tools.children().find('.opVotos');
            itemVotos.remove();
        }
        var votos = this.recurso.find('.votos').first().clone();
        var literal = votos.find('.literal').hide();
        var numero = votos.find('strong').hide();
        var boolGusta = true;
        if (numero.text() == 0) {
            numero = "0";
        } else {
            numero.text(numero.text().replace('-', '- '));
            numero = numero.text().split(' ');
            numero = numero.filter(function (v) { return v !== '' });
            if (numero[0] == '-') {
                boolGusta = false;
            }
            numero = numero[1];
        }
        var texto = ' personas les gusta';
        if (numero == '1') {
            texto = ' persona le gusta';
        }
        if (!boolGusta) {
            texto = ' no personas les gusta';
            if (numero == '1') {
                texto = ' persona no le gusta';
            }
        }

        //REVISION DIDACTALIA
        //var span = $('<span />').addClass('gusta').text('| a ' + numero + texto);
        //votos.append(span);

        var li = $('<li />').addClass('opVotos').append(votos.html());
        if (this.tools != undefined) {
            this.tools.children().append(li);
        }
        return;
    },
    montarRedesSocialesNuevo: function () {
        var that = this;
        var redes = this.recurso.find('.redesSocialesCompartir').first().clone();
        var lis = redes.find('li');
        lis.each(function () {
            var li = $(this);
            if (li.hasClass('facebook')) {
                redes.find('a').text('Facebook');
            }
            if (li.hasClass('google') || li.hasClass('twitter') || li.hasClass('facebook')) {
                li.addClass('iconsRedesSociales');
                li.addClass('right');
                if (that.tools != undefined) {
                    that.tools.children().append(li);
                }
            }
        });
        return;
    },
    montarVolverFicha: function () {
        var that = this;
        if (this.tools != undefined) {
            var itemVolver = this.tools.children().find('.opVolver');
            itemVolver.remove();
        }
        var volver = $('<a/>').attr('href', this.enlace.attr('href')).attr('target', '_blank');
        volver.text('Ir a la Web');

        //REVISION DIDACTALIA
        //var spanIcono = $('<span/>').attr('title', 'enlace recurso externo').attr('class', 'icono');
        //volver.append(spanIcono);

        volver.bind('click', function (evento) {
            evento.preventDefault();
            if (that.isOperativaLogin) {
                var login = that.palcoLogin.children().hide();
                that.body.prepend(login);
            }
            that.operativaPanelsPrevio.after(that.operativaPanels);
            that.body.removeClass('activo');
            that.body.children().removeClass('palco-items');
            that.palco.remove();
            window.open(volver.attr('href'));
        })
        var li = $('<li />').addClass('opVolver').append(volver);

        if (this.tools != undefined) {
            this.tools.children().append(li);
        }
        this.montarRedesSocialesNuevo();
        return;
    },
    montarPalco: function () {
        var altura = $(window).height();
        var src = this.enlace.attr('href');
        if (altura <= 600) altura = 600;
        this.palco = $('<div />').attr('id', 'palco');
        this.mascara = $('<div />').attr('id', 'palco-mascara').hide();
        this.loading = $('<p />').addClass('palco-loading');
        this.palcoHeader = $('<div />').attr('id', 'palco-header').addClass('palco-header');
        this.palcoWrapHeader = $('<div />').addClass('palco-wrap-header').addClass('title');
        this.contenedorIframe = $('<div />').addClass('contenedorIframe');
        this.iframe = $('<iframe />').attr('src', src).addClass('palco-iframe').css('height', (altura - this.margen) + 'px');
        this.contenedorIframe.append(this.iframe);
        this.palcoHeader.append(this.palcoWrapHeader);
        this.palco.append(this.palcoHeader);
        this.palco.append(this.contenedorIframe);
        this.mascara.append(this.loading);
        this.palco.append(this.mascara);
        this.body.append(this.palco);
        //REVISION DIDACTALIA
        this.montarLogoComunidad();
        this.montarTituloRecurso();
        //REVISION DIDACTALIA
        //this.montarUsuarioPublicador();
        //REVISION DIDACTALIA
        //this.montarVisitas();
        //this.montarRedesSociales();
        this.montarHerramientas();
        this.montarVotos();
        this.montarBotonCerrarPalco();
        this.montarVolverFicha();
        this.palco.append($('#mascaraBlanca'));
        this.palco.append($('.popup'));
        return;
    },
    mostrarMascara: function () {
        var altura = this.iframe.height();
        this.mascara.css('height', (altura + 4) + 'px');
        this.mascara.show();
    },
    ocultarMascara: function () {
        var altura = this.iframe.height();
        this.mascara.hide();
    },
    mostrarLogin: function () {
        var altura = this.iframe.height();
        this.palcoLogin = $('<div />').attr('id', 'palco-login');
        var login = $('#formularioLoginHeader').show();
        this.palcoLogin.css('height', altura + 'px');
        this.palcoLogin.append(login);
        this.mascara.after(this.palcoLogin);
        this.isOperativaLogin = true;
        return;
    },
    traerOperativaSumarEspacioPersonal: function (url) {
        var that = this;
        var url = 'includes/operativas/agregarEspacioPersonal.php';
        var altura = this.iframe.height();
        var operativa = $('<div />').addClass('operativa');
        var formulario = $('<div />').addClass('formulario');
        var loading = $('<div />').attr('id', 'loading');
        var texto = $('<p />').text('Cargando formulario, un momento por favor');
        loading.append(texto);
        formulario.append(loading);
        operativa.append(formulario);
        this.mascara.after(operativa);
        var respuesta = $.ajax({
            url: url,
            cache: false,
            type: "GET",
            dataType: "html"
        })
		.done(function (pagina) {
		    var html = pagina;
		    setTimeout(function () {
		        formulario.html(html);
		    }, 600)
		    //var scrollTop = that.listado.scrollTop() + 710;
		    //that.listado.scrollTop(scrollTop);
		    //CompletadaCargaUsuariosVotanRecurso();
		    return;
		})
		.fail(function (jqXHR, textStatus) {
		    alert("Request failed: " + textStatus);
		})
        return;
    },
    mostrarOperativaSumarEspacioPersonal: function () {
        var that = this;
        var altura = this.iframe.height();
        var operativa = $('<div />').addClass('operativa');
        var formulario = $('<div />').addClass('formulario');
        operativa.append(formulario);
        formulario.append(this.operativaPanels);
        this.mascara.after(operativa);
        var cerrar = $('#panels .cerrar a');
        cerrar.bind('click', function (evento) {
            that.operativaPanelsPrevio.after(that.operativaPanels);
            that.ocultarOperativa();
        })
        return;
    },
    mostrarOperativaEspacioPersonal: function () {
        this.mostrarOperativaSumarEspacioPersonal();
        //engancharOperativa.init();
        return;
    },
    mostrarOperativaComentar: function () {
        var that = this;
        var altura = this.iframe.height();
        var operativa = $('<div />').addClass('operativa');
        var formulario = $('<div />').addClass('formulario');
        operativa.append(formulario);
        var nuevoFormulario = $('<div />').attr('id', 'nuevo_00000000-0000-0000-0000-000000000001');
        formulario.append(nuevoFormulario);
        var divTools = $('<div />').addClass('tools');
        formulario.append(divTools);
        var pTools = $('<p />');
        divTools.append(pTools);
        var aTools = $('<a />');
        pTools.append(aTools);
        aTools.text("cerrar");
        aTools.attr('class', 'hidePanel');
        this.mascara.after(operativa);
        AccionCrearComentario("ctl00_ctl00_CPH1_CPHContenido_mListadoComentarios", "00000000-0000-0000-0000-000000000001", false);
        aTools.bind('click', function (evento) {
            that.ocultarOperativa();
        })
        return;
    },
    ocultarOperativa: function () {
        var operativa = $('.operativa');
        operativa.remove();
        this.ocultarMascara();
    },
    engancharHerramientas: function () {
        var that = this;
        var enlaceEspacioPersonal = this.tools.find('.oGuardarEspacioPersonal');
        var enlaceComentar = this.tools.find('.opComentar a');
        enlaceEspacioPersonal.bind('click', function (evento) {
            var enlace = $(evento.target);
            that.ocultarOperativa();
            that.mostrarMascara();
            that.mostrarOperativaEspacioPersonal();
        })
        enlaceComentar.bind('click', function (evento) {
            var comentariosDisponibles = $('.oculto.comentariosDisponibles');
            if (comentariosDisponibles.hasClass('invitado')) {
                operativaLoginEmergente.init();
            } else {
                var enlace = $(evento.target);
                that.ocultarOperativa();
                that.mostrarMascara();
                that.mostrarOperativaComentar();
            }
        })
        return;
    },
    controlAlturaPalco: function () {
        var altura = $(window).height();
        this.iframe.css('height', (altura - this.margen) + 'px');
        return;
    },
    comportamiento: function () {
        var that = this;
        var html = '<span class=\"icono\" title=\"enlace recurso externo\"></span>';

        var isExterno = $(this.enlace).attr('target') == '_blank';

        if (isExterno && this.enlace.attr('href') != undefined) {
            var dominio = this.enlace.attr('href').replace("http://", "").replace("https://", "").replace("www.", "");
            if (dominio.indexOf("/") > -1) {
                dominio = dominio.substring(0, dominio.indexOf("/"));
            }
            for (i = 0; i < listaDominiosSinPalco.length; i++)
            {
                if (dominio.indexOf(listaDominiosSinPalco[i]) != -1)
                {
                    isExterno = false;
                }
            }
            //isExterno = listaDominiosSinPalco.indexOf(dominio) == -1
        }

        var extension = "";
        if (this.enlace != undefined && this.enlace.attr('href') != undefined && this.enlace.attr('href').lastIndexOf('.') != -1) {
            extension = this.enlace.attr('href').substring(this.enlace.attr('href').lastIndexOf('.'));
        }
        var extensionesEliminadas = new Array('.rar', '.zip', '.pdf', '.flv', '.txt');
        if (isExterno && ((this.enlace != undefined && this.enlace.attr('href') != undefined && this.enlace.attr('href').indexOf('VisualizarDocumento.aspx') != -1) || (this.tipoRecurso.length != 0 && !this.tipoRecurso.hasClass('hipervinculo') && !this.tipoRecurso.hasClass('semantico')) || extensionesEliminadas.indexOf(extension) != -1)) {
            isExterno = false;
        }

        if (isExterno) {
            this.body.addClass('palco');
            this.enlace.append(html);
            this.enlace.bind('click', function (evento) {
                evento.preventDefault();
                that.body.addClass('activo');
                that.body.children().addClass('palco-items');
                that.montarPalco();
            })
        }
        return;
    }
}	
//var completadaCargaAccionesComunidad = function(){
//	$('#customAboutResource .acciones a').bind('click', function(evento){
//		evento.preventDefault();
//		operativaLoginEmergente.init();
//	})
//}
