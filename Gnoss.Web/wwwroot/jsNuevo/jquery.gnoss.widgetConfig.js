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
 * Date: Thu May 12 15:04:36 2011 -0400
 */

var configWidgetGNOSS = {
    cssWidget: '.widget',
    cssWrapWidget: '.widget-usuario-gnoss',
    cssBodyWidget: '.widget-usuario-body-gnoss',
    cssFooterWidget: '.widget-usuario-foot-gnoss',
    cssWidgetConfig: '.widgetConfig',
    cssLogoGrande: '#linkComunidadLogo',
    cssLogoYTexto: '#linkComunidad',
    cssNumeroRecursos: '.cNumeroRecursos',
    cssAncho: '.cAncho',
    cssAlto: '.cAlto',
    cssAnchoMax: '.cAnchoMax',
    cssAltoMax: '.cAltoMax',
    cssAnchoMin: '.cAnchoMin',
    cssAltoMin: '.cAltoMin',
    cssIncluirScroll: '.cIncluirScroll',
    cssAutoRotacion: '.cAutoRotacion',
    cssColorExterior: '.cColorExterior input',
    cssColorInterior: '.cColorInterior input',
    cssColorEnlaces: '.cColorEnlaces input',
    cssBtProbar: '.btProbar',
    cssBtGenerar: '.btGenerar',
    cssUrlWidget: '.urlWidget',
    idCodigo: '#widget-codigo-gnoss',
    init: function () {
        this.config();
        this.aplicar();
        this.actualizarCodigo();
        this.engancharProbar();
        return;
    },
    config: function () {
        this.widget = $('#section ' + this.cssWidget);
        this.widgetConfig = $('#section ' + this.cssWidgetConfig);
        this.wrapWidget = $(this.cssWrapWidget, this.widget);
        this.bodyWidget = $(this.cssBodyWidget, this.widget);
        this.footerWidget = $(this.cssFooterWidget, this.widget);
        this.logoGrande = $(this.cssLogoGrande, this.widget);
        this.logoYTexto = $(this.cssLogoYTexto, this.widget);
        this.inputNumeroRecursos = $(this.cssNumeroRecursos, this.widgetConfig);
        this.inputAncho = $(this.cssAncho, this.widgetConfig);
        this.inputAlto = $(this.cssAlto, this.widgetConfig);
        this.anchoMaximo = $('#section ' + this.cssAnchoMax).html();
        this.altoMaximo = $('#section ' + this.cssAltoMax).html();
        this.anchoMinimo = $('#section ' + this.cssAnchoMin).html();
        this.altoMinimo = $('#section ' + this.cssAltoMin).html();
        this.checkedIncluirScroll = $(this.cssIncluirScroll, this.widgetConfig);
        this.checkedAutoRotacion = $(this.cssAutoRotacion, this.widgetConfig);
        this.inputColorExterior = $(this.cssColorExterior, this.widgetConfig);
        this.inputColorInterior = $(this.cssColorInterior, this.widgetConfig);
        this.inputColorEnlaces = $(this.cssColorEnlaces, this.widgetConfig);
        this.btProbar = $(this.cssBtProbar, this.widgetConfig);
        this.btGenerar = $(this.cssBtGenerar, this.widgetConfig);
        this.btAjustar = $(this.cssBtAjustar, this.widgetConfig);
        this.enlaces = $('a', this.bodyWidget);
        this.parrafos = $('p', this.bodyWidget);
        this.urlWidget = $(this.cssUrlWidget);
        this.codigo = $(this.idCodigo);
        return;
    },
    comprobarAncho: function () {
        var ancho = parseInt(this.inputAncho.val());
        if (ancho > this.anchoMaximo) {
            ancho = this.anchoMaximo;
        } else if (ancho < this.anchoMinimo) {
            ancho = this.anchoMinimo;
        };
        return ancho;
    },
    comprobarAlto: function () {
        var alto = parseInt(this.inputAlto.val());
        if (alto > this.altoMaximo) {
            alto = this.altoMaximo;
        } else if (alto < this.altoMinimo) {
            alto = this.altoMinimo;
        };
        return alto;
    },
    aplicar: function () {
        var that = this;
        if ($(".cLogo input[type=radio]:checked").val() == 'logoGrande') {
            this.logoGrande.css({ 'display': '' });
            this.logoYTexto.css({ 'display': 'none' });
        }
        else {
            this.logoGrande.css({ 'display': 'none' });
            this.logoYTexto.css({ 'display': '' });
        }
        var ancho = this.comprobarAncho();

        this.logoGrande.find('img').css({ 'max-width': ancho - 20 + 'px' });

        var alto = this.comprobarAlto(); 

        this.enlaces.each(function () {
            $(this).css('color', that.inputColorEnlaces.val());
        });
        var numero = $(".footerWidgetGNOSS input[type=radio]:checked").val();
        this.footerWidget.removeClass('default0');
        this.footerWidget.removeClass('default1');
        this.footerWidget.removeClass('default2');
        this.footerWidget.addClass('default' + numero);
        return;
    },
    actualizarCodigo: function () {
        var urlWidget = this.urlWidget.val();
        urlWidget += "&numRecursos=" + this.inputNumeroRecursos.val();
        var ancho = this.comprobarAncho();
        var alto = this.comprobarAlto();
        urlWidget += "&ancho=" + ancho + "&alto=" + alto;
        var tieneScroll = $(".cMovimiento input[type=radio]:checked").val() == 'scroll' ? 'true' : 'false';
        urlWidget += "&scroll=" + tieneScroll;
        if (this.inputColorExterior.length > 0) {
            urlWidget += "&marco=" + encodeURIComponent(this.inputColorExterior.val()) + "";
            urlWidget += "&fondo=" + encodeURIComponent(this.inputColorInterior.val()) + "";
            urlWidget += "&enlaces=" + encodeURIComponent(this.inputColorEnlaces.val()) + "";
            urlWidget += "&logo=" + $(".footerWidgetGNOSS input[type=radio]:checked").val();
        }

        if ($(".cLogo input[type=radio]:checked").val() == 'logoGrande') {
            urlWidget += "&logoCabecera=1"
        }

        var codigoWidget = "<iframe marginheight=\"0\" marginwidth=\"0\" scrolling=\"no\" frameborder=\"0\" width=\"" + ancho + "\" height=\"" + alto + "\" src=\"" + urlWidget + "\"></iframe>"

        $.get(urlWidget, function (data, status) {

            var style = data.substring(data.indexOf("<style>"));
            style = style.substring(0, style.indexOf("</style>") + "</style>".length);

            var bodyApertura = "<body class=\"bodyWidget-gnoss\">";
            var inicioBody = data.indexOf(bodyApertura) + bodyApertura.length;
            var html = data.substring(inicioBody);
            html = html.substring(0, html.indexOf("</body>"));

            $('#panContenedorWidget').html(style + html);

            $('#panContenedorWidget').css({
                'width': ancho + 'px'
            });

        });

        this.codigo.find('textarea').val(codigoWidget);
    },

    engancharProbar: function () {
        var that = this;
        this.btProbar.bind('click', function (evento) {
            evento.preventDefault();
            that.codigo.hide();
            that.aplicar();
            that.actualizarCodigo();
        });
    }
}
generarCodigoGNOSS = {
	btGenerar: '.btGenerar',
	idWidget: '#widget-codigo-gnoss',
	init: function(){
		this.config();
		var that = this;
		$(this.btGenerar).each(function(){
			$(this).bind('click', function(evento){
				//evento.preventDefault();
				that.widget.show();
			});
		})
	},
	config: function(){
		this.widget = $(this.idWidget);
		return;
	},
	enganchar: function(){
		return;
	}
}
$(function () {
    configWidgetGNOSS.init();
    generarCodigoGNOSS.init();
    if (typeof $.farbtastic != 'undefined') {
        var f = $.farbtastic('#colorpicker');
        var p = $('#colorpicker').css('opacity', 0.25);
        var selected;
        $('.aparienciaWidget input')
      .each(function () { f.linkTo(this); $(this).css('opacity', 0.75); })
      .focus(function () {
          if (selected) {
              $(selected).css('opacity', 0.75).removeClass('colorwell-selected');
          }
          f.linkTo(this);
          p.css('opacity', 1);
          $(selected = this).css('opacity', 1).addClass('colorwell-selected');
      });
    }
});