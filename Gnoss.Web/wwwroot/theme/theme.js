/*
	Theme Name: GNOSS Front - MyGnoss base theme 
	Theme URI: http://dewenir.es

	Author: GNOSS Front
	Author URI: http://dewenir.es

	Description: Fichero base de customizaciÃ³n del tema de MyGNOSS.
	Version: 1.0
*/

var headerMinScroll = {
    posicionAnterior: 0,
    obj_top: 0,
    init: function () {
        this.config();
        this.scroll();
        return;
    },
    config: function () {
        this.body = body;
        return;
    },
    scroll: function () {
        var that = this;
        $(window).scroll(function () {
            that.lanzar();
        });
        return;
    },
    lanzar: function () {
        var obj = $(window);
        this.posicionAnterior = this.obj_top;
        this.obj_top = obj.scrollTop();
        if (this.obj_top < this.posicionAnterior || this.obj_top <= 10) {
            this.body.removeClass("headerMin");
        } else {
            this.body.addClass("headerMin");
        }
        return;
    }
};

var bodyScrolling = {
    obj_top: 0,
    init: function () {
        this.config();
        this.scroll();
        return;
    },
    config: function () {
        this.body = body;
        return;
    },
    scroll: function () {
        var that = this;
        $(window).scroll(function () {
            that.lanzar();
        });
        return;
    },
    lanzar: function () {
        var obj = $(window);
        this.obj_top = obj.scrollTop();
        if (this.obj_top <= 10) {
            this.body.removeClass("scrolling");
        } else {
            this.body.addClass("scrolling");
        }
        return;
    }
};

var closeSidebar = {
    init: function () {
        this.config();
        this.cerrar();
        return;
    },
    config: function () {
        this.body = body;
        return;
    },
    cerrar: function () {
        var localBody = this.body;
        var sidebars = localBody.find('.pmd-sidebar');
        var cerrar = sidebars.find('.header .cerrar');

        cerrar.on('click', function () {

            var item = $(this);
            var sidebar = item.parents('.pmd-sidebar');
            var id = sidebar.attr('id');
            var toggle = localBody.find('[data-target="' + id + '"]').first();

            if (toggle.length > 0) toggle.trigger('click');

        });

        return;
    }
};

var buscarSidebar = {
    init: function () {
        this.config();
        this.eventoBuscar();
        return;
    },
    config: function () {
        this.body = body;
        this.menuLateralBuscador = this.body.find('.menuLateralBuscador');
        return;
    },
    eventoBuscar: function () {

        var that = this;
        var header = this.menuLateralBuscador.find('.header');
        var input = header.find('.busqueda input');

        input.unbind('keyup');
        input.bind('keyup input', function () {
            var item = $(this);
            var itemVal = item.val();
            that.buscar(itemVal);
        });

        return;
    },
    buscar: function (valor) {

        var bodyLateral = this.menuLateralBuscador.find('.body');
        var lista = bodyLateral.find('ul li');

        lista.each(function (indice) {
            var item = $(this);
            var enlaceItem = item.children('a');
            var itemText = enlaceItem.text();

            item.removeClass('oculto');

            if (itemText.toLowerCase().indexOf(valor.toLowerCase()) < 0) {
                item.addClass('oculto');
            } else {
                item.parents('.oculto').removeClass('oculto');
            }

        });

        return;
    }
};

var buscarMovil = {
    init: function () {
        this.config();
        this.comportamiento();
        return;
    },
    config: function () {
        this.body = body;
        this.buscador = this.body.find('#buscador');
        return;
    },
    comportamiento: function () {

        var localBody = this.body;
        var btnBuscar = this.buscador.find('.botonBuscar');
        var cerrar = this.buscador.find('.cerrarBusqueda');
        var txtBusqueda = this.buscador.find('#txtBusquedaPrincipal');

        btnBuscar.on('click', function (e) {

            if (localBody.hasClass('buscando')) {
                localBody.removeClass('buscando');
            } else {
                localBody.addClass('buscando');
                txtBusqueda.focus();
            }

        });

        cerrar.on('click', function (e) {
            localBody.removeClass('buscando');
        });

        return;
    }
};

var nivelesMenu = {
    init: function () {
        this.config();
        this.evento();
        return;
    },
    config: function () {
        this.body = body;
        this.header = this.body.find('header');
        return;
    },
    evento: function () {

        var rowMenu = this.header.find('.row-menu');
        var a = rowMenu.find('li.parent > a');

        a.unbind('click');
        a.on('click', function (e) {

            e.preventDefault();
            var item = $(this);
            var li = item.parent();

            if (li.hasClass('open')) {
                li.removeClass('open');
            } else {
                if (li.parents('li.parent').length == 0) {
                    rowMenu.find('ul').first().children('li.parent').removeClass('open');
                }
                li.addClass('open');
            }

        });

        return;
    }
};

var cambioVistaListado = {
    init: function () {
        this.config();
        this.comportamiento();
        return;
    },
    config: function () {
        this.body = body;
        return;
    },
    comportamiento: function () {

        var accionesListado = this.body.find('.wrap-col > .acciones-listado, .cabecera-listados .acciones-listado');
        var visualizacion = accionesListado.find('.visualizacion');
        var resourceList = this.body.find('.wrap-col > .resource-list');
        var lis = visualizacion.find('li');
        var a = lis.children('a');

        a.on('click', function (e) {

            e.preventDefault();
            var item = $(this);
            var li = item.parent();
            var clase = li.data('class-resource-list');

            lis.removeClass('activeView');
            visualizacion.find('li[data-class-resource-list="' + clase + '"]').addClass('activeView');

            if (clase != "") {
                resourceList.removeClass('compacView listView');
                resourceList.addClass(clase);
            }
        });

        return;
    }
};

var masOpciones = {
    init: function () {
        this.config();
        this.comportamiento();
        return;
    },
    config: function () {
        this.body = body;
        return;
    },
    comportamiento: function () {

        var divMasOpciones = this.body.find('.mas-opciones');
        var toggle = divMasOpciones.children('.toggle');
        var cerrar = divMasOpciones.find('.cerrar');

        toggle.on('click', function (e) {

            var item = $(this);
            var padre = item.parent();

            if (padre.hasClass('open')) {
                padre.removeClass('open');
            } else {
                divMasOpciones.removeClass('open');
                padre.addClass('open');
            }

        });

        cerrar.on('click', function (e) {
            var item = $(this);
            var padre = item.parents('.mas-opciones');
            padre.removeClass('open');
        });
        /*
        this.body.bind('click', function(e){
          if (!$.contains(divMasOpciones.get(0), e.target)){
            if (e.target !== divMasOpciones.get(0)) divMasOpciones.removeClass('open');
          }
        });*/

        return;
    }
};

var limiteLongitudFacetas = {
    init: function () {
        return;
    }
};

var comportamientoFacetas = {
    init: function () {
        this.config();
        this.comportamiento();
        return;
    },
    config: function () {
        this.body = body;
        this.panFacetas = this.body.find('#panFacetas');
        return;
    },
    comportamiento: function () {

        var iconoDesplegar = this.panFacetas.find('.icono-desplegar');

        iconoDesplegar.off('click').on('click', function (e) {

            e.preventDefault();
            e.stopPropagation();

            var item = $(this);
            var a = item.parent();
            var label = a.parent();
            var li = label.parent();

            if (li.hasClass('abierta')) {
                li.removeClass('abierta');
            } else {
                li.addClass('abierta');
            }

        });

        return;
    }
};
function CompletadaCargaFacetas() {
    comportamientoFacetas.init();
    comportamientoCargaFacetas.init();
    if (typeof (window.comportamientoCargaFacetasComunidad) == 'function') {
        comportamientoCargaFacetasComunidad();
    }
}
var customizarModalResultados = {
    init: function () {
        this.config();
        this.eventos();
        this.eventosFacetas();
        this.eventoBuscar();
        return;
    },
    config: function () {
        this.body = body;
        this.modal = this.body.find('#modal-resultados');
        return;
    },
    recalcular: function () {
        var dialog = this.modal.find('.modal-dialog');
        var content = dialog.find('.modal-content');
        var wrapContent = content.find('.wrapContent');

        var h2 = wrapContent.children('h2');
        var buscador = wrapContent.children('.buscador-coleccion');
        var indiceLista = wrapContent.children('.indice-lista');

        var marginTopDialog = dialog.css('margin-top').replace('px', '');
        var marginBottomDialog = dialog.css('margin-bottom').replace('px', '');
        var paddingTopWrap = wrapContent.css('padding-top').replace('px', '');
        var paddingBottomWrap = wrapContent.css('padding-bottom').replace('px', '');
        var topWrap = wrapContent.css('top').replace('px', '');
        var altoH2 = h2.outerHeight(true);
        var altoBuscador = buscador.outerHeight(true);
        var paddingTopIndiceLista = indiceLista.css('padding-top').replace('px', '');
        var paddingBottomIndiceLista = indiceLista.css('padding-bottom').replace('px', '');

        var alto = this.modal.height() - marginTopDialog - marginBottomDialog - paddingTopWrap - paddingBottomWrap - topWrap - altoH2 - altoBuscador - paddingTopIndiceLista - paddingBottomIndiceLista;

        indiceLista.css('max-height', alto + 'px');
        return;
    },
    eventos: function () {

        var that = this;

        this.modal.on('shown.bs.modal', function (e) {
            that.recalcular();
        });

        $(window).resize(function (e) {
            that.recalcular();
        });

        this.modal.find('.buscador-coleccion input').on("focus", function () {
            $(this).val("");
        });

        return;
    },
    eventosFacetas: function () {

        var indiceLista = this.modal.find('.indice-lista');

        indiceLista.off('click').on('click', '.icono-desplegar', function (e) {
            e.preventDefault();
            e.stopPropagation();

            var item = $(this);
            var a = item.parent();
            var label = a.parent();
            var li = label.parent();

            if (li.hasClass('abierta')) {
                li.removeClass('abierta');
            } else {
                li.addClass('abierta');
            }
        })

        return;
    },
    eventoBuscar: function () {

        var that = this;
        var box = this.modal.find('.indice-lista .box');
        var inputBuscador = this.modal.find('.buscar .texto');

        inputBuscador.unbind('keyup');
        inputBuscador.bind('keyup input', function () {
            var item = $(this);
            var itemVal = item.val();
            box.removeClass('oculto');
            that.buscar(itemVal);
        });

        return;
    },
    buscar: function (valor) {

        var lista = this.modal.find('.listadoFacetas li');

        lista.each(function (indice) {
            var item = $(this);
            var enlaceItem = item.find('a');
            var itemText = enlaceItem.find('.textoFaceta').text();

            item.removeClass('oculto');

            if (itemText.toLowerCase().indexOf(valor.toLowerCase()) < 0) {
                item.addClass('oculto');
            } else {
                //item.removeHighlight().highlight(valor);
                item.parents('.oculto').removeClass('oculto');
            }

        });

        return;
    }
};

var buscarCabecera = {
    init: function () {
        this.config();
        //this.mascara();
        this.cabeceraListado();
        return;
    },
    config: function () {
        this.body = body;
        this.buscador = this.body.find('div[id="buscador"]');
        return;
    },
    mascara: function () {

        var localBody = this.body;
        var input = this.buscador.find('input.text');

        input.focusin(function () {

            var item = $(this);
            item.parent().addClass('busqueda-activa');
            $('#buscador .ac_results').css('display', 'block');

        }).focusout(function () {

            var item = $(this);
            item.parent().removeClass('busqueda-activa');
            localBody.removeClass('buscando');
            $('#buscador .ac_results').css('display', 'none');

        });

        return;
    },
    cabeceraListado: function () {

        var localBody = this.body;
        var header = this.body.find('header');
        var txtBusPrincipalInput = header.find('.textoBusquedaPrincipalInput');
        var cabeceraLis = this.body.find('.cabecera-listados');
        var botonBuscar = cabeceraLis.find('.botonBuscar');

        botonBuscar.bind('click', function (e) {
            txtBusPrincipalInput.addClass('busqueda-activa');
            localBody.addClass('buscando');
        });

        return;
    }
};

var enlaceRecursoFicha = {
    init: function () {
        this.config();
        this.comportamiento();
        return;
    },
    config: function () {
        this.body = body;
        return;
    },
    comportamiento: function () {

        var accionesRecurso = this.body.find('.acciones-recurso');
        var galeria = this.body.find('#galeria');
        var enlaceRecurso = galeria.find('.enlace-recurso');
        var tipo = enlaceRecurso.data('tipo');

        var opcionesDefecto = {
            selector: '.enlace-recurso'
        };

        switch (tipo) {
            case 'video':
                var lanzador = accionesRecurso.find('.ampliar');
                var aux = {};
                break;
            case 'enlace':
                var lanzador = accionesRecurso.find('.enlace');
                var aux = {
                    iframeMaxWidth: '80%'
                };
                break;
            case 'pdf':
                var lanzador = accionesRecurso.find('.ampliar');
                var aux = {
                    iframeMaxWidth: '80%'
                };
                break;
            case 'imagen':
                var lanzador = accionesRecurso.find('.ampliar');
                var aux = {};
                break;
            case '':
                break;

        }

        var opciones = $.extend(opcionesDefecto, aux);

        galeria.lightGallery(opciones);

        if (lanzador != undefined) {
            lanzador.on('click', function (e) {
                e.preventDefault();
                enlaceRecurso.trigger('click');
            });
        }

        return;
    }
};

var facetasMovil = {
    init: function () {
        this.config();
        this.comportamiento();
        return;
    },
    config: function () {
        this.body = body;
        this.main = this.body.find('main[role="main"]');
        return;
    },
    comportamiento: function () {

        var filtrarMovil = this.main.find('.acciones-listado .filtrar-movil a, .header-facetas a');
        var localBody = this.body;

        filtrarMovil.on('click', function (e) {
            localBody.toggleClass('facetas-movil');
        });

        return;
    }
};

var operativaFullWidth = {
    init: function () {
        this.config();
        this.lanzar();
        this.escalado();

        return;
    },
    config: function () {
        this.body = body;
        this.fullwidthrow = this.body.find('.fullwidthrow');
        this.main = this.body.find('main[role="main"] > .container');
        return;
    },
    lanzar: function () {
        var windows_width = $(window).width();
        var container_width = this.main.width();
        var anchoScrollbar = (this.body.width() > 767) ? 31.5 : 0;
        var margen = (windows_width - container_width + anchoScrollbar) / 2;

        var margenNegativo = parseFloat('-' + margen);

        this.fullwidthrow.each(function () {
            var item = $(this);
            item.css({
                "transform": "translateX(" + margenNegativo + "px)",
                "width": "100vw"
            });
        });

        return;
    },
    escalado: function () {
        var that = this;

        $(window).resize(function () {
            that.lanzar();
        });

        return;
    }
};

function comportamientoCargaFacetasComunidad() {

};


$.fn.reverse = [].reverse;

var body;

$(function () {
    body = $('body');

    closeSidebar.init();
    buscarSidebar.init();
    buscarMovil.init();
    nivelesMenu.init();
    masOpciones.init();
    buscarCabecera.init();
    bodyScrolling.init();

    if (body.find('.fullwidthrow').length > 0) {
        operativaFullWidth.init();
    }

    if (body.hasClass('listadoComunidad')) {
        cambioVistaListado.init();
        comportamientoFacetas.init();
        customizarModalResultados.init();
        facetasMovil.init();
    } else if (body.hasClass('fichaRecurso')) {
        enlaceRecursoFicha.init();
    }

});











/**
 * --------------------------------------------------------------------------
 * Propeller v1.3.1 (http://propeller.in): sidebar.js
 * Copyright 2016-2018 Digicorp, Inc.
 * Licensed under MIT (http://propeller.in/LICENSE)
 * --------------------------------------------------------------------------
*/

var pmdSidebar = function ($) {


    /**
     * ------------------------------------------------------------------------
     * Variables
     * ------------------------------------------------------------------------
    */

    var NAME = 'pmdSidebar';
    var JQUERY_NO_CONFLICT = $.fn[NAME];
    var isOpenWidth = 1200;

    var ClassName = {
        OPEN: 'pmd-sidebar-open',
        SLIDE_PUSH: 'pmd-sidebar-slide-push',
        RIGHT_FIXED: 'pmd-sidebar-right-fixed',
        LEFT_FIXED: 'pmd-sidebar-left-fixed',
        OVERLAY_ACTIVE: 'pmd-sidebar-overlay-active',
        BODY_OPEN: 'pmd-body-open',
        RIGHT: 'pmd-sidebar-right',
        NAVBAR_SIDEBAR: 'pmd-navbar-sidebar',
        LEFT: 'pmd-sidebar-left',
        PM_INI: ".pm-ini",
        IS_SLIDEPUSH: "is-slidepush"
    };

    var Selector = {
        BODY: 'body',
        PARENT_SELECTOR: '',
        OVERLAY: '.pmd-sidebar-overlay',
        SIDEBAR: '.pmd-sidebar',
        LEFT: '.' + ClassName.LEFT,
        RIGHT_FIXED: '.' + ClassName.RIGHT_FIXED,
        NAVBAR_SIDEBAR: '.' + ClassName.NAVBAR_SIDEBAR,
        SIDEBAR_HEADER: '#sidebar .sidebar-header',
        TOGGLE: '.pmd-sidebar-toggle',
        TOPBAR_FIXED: '.topbar-fixed',
        SIDEBAR_DROPDOWN: '.pmd-sidebar-nav .dropdown-menu',
        TOGGLE_RIGHT: '.pmd-sidebar-toggle-right',
        TOPBAR_TOGGLE: '.pmd-topbar-toggle',
        TOPBAR_CLOSE: '.topbar-close',
        NAVBAR_TOGGLE: '.pmd-navbar-toggle',
        PM_INI: ".pm-ini",
        IS_SLIDEPUSH: '.' + ClassName.IS_SLIDEPUSH
    };

    var Event = {
        CLICK: 'click'
    };


    /**
   * ------------------------------------------------------------------------
   * Functions
   * ------------------------------------------------------------------------
   */

    // Left sidebar toggle
    function onSidebarToggle(e) {
        var dataTarget = "#" + $(e.currentTarget).attr("data-target");
        $(dataTarget).toggleClass(ClassName.OPEN);

        var allSidebars = $(Selector.SIDEBAR);

        if (($(dataTarget).hasClass(ClassName.LEFT_FIXED) || $(dataTarget).hasClass(ClassName.RIGHT_FIXED)) && $(dataTarget).hasClass(ClassName.OPEN)) {
            $(Selector.OVERLAY).addClass(ClassName.OVERLAY_ACTIVE);
            //$(Selector.OVERLAY + '[data-rel="' + $(e.currentTarget).attr("data-target") + '"]').addClass(ClassName.OVERLAY_ACTIVE);
            $(Selector.BODY).addClass(ClassName.BODY_OPEN);
        } else {
            if (!allSidebars.hasClass(ClassName.OPEN)) {
                $(Selector.OVERLAY).removeClass(ClassName.OVERLAY_ACTIVE);
                //$(Selector.OVERLAY + '[data-rel="' + $(e.currentTarget).attr("data-target") + '"]').removeClass(ClassName.OVERLAY_ACTIVE);
                $(Selector.BODY).removeClass(ClassName.BODY_OPEN);
            }
        }

    }

    // Nave bar in Sidebar
    function onNavBarToggle() {
        $(Selector.NAVBAR_SIDEBAR).toggleClass(ClassName.OPEN);
        if (($(Selector.NAVBAR_SIDEBAR).hasClass(ClassName.NAVBAR_SIDEBAR)) && $(Selector.NAVBAR_SIDEBAR).hasClass(ClassName.OPEN)) {
            $(Selector.OVERLAY + '[data-rel="' + $(e.currentTarget).attr("data-target") + '"]').addClass(ClassName.OVERLAY_ACTIVE);
            $(Selector.BODY).addClass(ClassName.BODY_OPEN);
        } else {
            $(Selector.OVERLAY + '[data-rel="' + $(e.currentTarget).attr("data-target") + '"]').removeClass(ClassName.OVERLAY_ACTIVE);
            $(Selector.BODY).addClass(ClassName.BODY_OPEN);
        }
    }

    // Overlay
    function onOverlayClick(event) {
        var $this = $(event.currentTarget);
        //var rel = $this.data('rel');
        $this.removeClass(ClassName.OVERLAY_ACTIVE);
        $(Selector.SIDEBAR).removeClass(ClassName.OPEN);
        //$(Selector.SIDEBAR + "#" + rel).removeClass(ClassName.OPEN);
        $(Selector.NAVBAR_SIDEBAR).removeClass(ClassName.OPEN);
        $(Selector.BODY).removeClass(ClassName.BODY_OPEN);

        event.stopPropagation();
    }

    // On Window Resize
    function onResizeWindow(e) {
        var options = e.data.param1;
        var sideBarSelector = Selector.SIDEBAR;
        $(sideBarSelector).each(function () {
            var $this = $(this);
            var sideBarId = $this.attr("id");
            var isOpenWidth = $("a[data-target=" + sideBarId + "]").attr("is-open-width");
            if ($(window).width() < isOpenWidth) {
                if ($("#" + sideBarId).hasClass(ClassName.LEFT && ClassName.SLIDE_PUSH)) {
                    $("#" + sideBarId).removeClass(ClassName.OPEN + ' ' + ClassName.SLIDE_PUSH);
                    $("#" + sideBarId).addClass(ClassName.LEFT_FIXED + ' ' + ClassName.IS_SLIDEPUSH);
                } else {
                    $("#" + sideBarId).removeClass(ClassName.OPEN);
                }
            } else {
                if ($("#" + sideBarId).hasClass(ClassName.IS_SLIDEPUSH)) {
                    $("#" + sideBarId).addClass(ClassName.OPEN + ' ' + ClassName.SLIDE_PUSH);
                    $("#" + sideBarId).removeClass(ClassName.LEFT_FIXED);
                } else {
                    $("#" + sideBarId).addClass(ClassName.OPEN);
                }
            }
        });
        $(pmdSidebar.prototype.attachParentSelector(Selector.PARENT_SELECTOR, Selector.OVERLAY + '[data-rel="' + options.init + '"]')).removeClass(ClassName.OVERLAY_ACTIVE);
        $(Selector.BODY).removeClass(ClassName.BODY_OPEN);
    }

    /**
   * ------------------------------------------------------------------------
   * Initialization
   * ------------------------------------------------------------------------
   */

    var pmdSidebar = function () {
        _inherits(pmdSidebar, commons);
        function pmdSidebar(options) {
            var sideBarSelector = Selector.TOGGLE;
            if (Selector.PARENT_SELECTOR !== "" && Selector.PARENT_SELECTOR !== undefined) {
                sideBarSelector = Selector.TOGGLE + "[data-target=" + Selector.PARENT_SELECTOR.substr(1, Selector.PARENT_SELECTOR.length) + "]";
            }
            $(sideBarSelector).each(function () {
                var $this = $(this);
                var dataTarget = "#" + $this.attr("data-target");
                var dataPlacement = $this.attr("data-placement");
                var dataPosition = $this.attr("data-position");
                var isopen = $this.attr("is-open");
                var minsize = $this.attr("minsize");
                dataPlacement = dataPlacement || "";
                dataPosition = dataPosition || "";
                if ($(sideBarSelector).attr("data-target") === undefined) {
                    console.warn("You need to define 'data-target' attribute in the action button.");
                }
                if ($(Selector.SIDEBAR).attr("id") === undefined) {
                    console.warn("You need to add id='" + $this.attr("data-target") + "'in the sidebar container div like <aside id='" + $this.attr("data-target") + "'class='pmd-sidebar'>");
                }
                if (dataPlacement.toLowerCase() === "left") {
                    $(dataTarget).addClass(ClassName.LEFT);
                } else if (dataPlacement.toLowerCase() === "right") {
                    $(dataTarget).addClass(ClassName.RIGHT_FIXED);
                } else {
                    $(dataTarget).addClass(ClassName.LEFT);
                }
                if (dataPlacement.toLowerCase() === "left" && dataPosition.toLowerCase() === "slidepush") {
                    $(dataTarget).addClass(ClassName.SLIDE_PUSH);
                } else if (dataPlacement.toLowerCase() === "left" && dataPosition.toLowerCase() === "fixed") {
                    $(dataTarget).addClass(ClassName.LEFT_FIXED);
                } else if (dataPlacement.toLowerCase() === "right" && dataPosition.toLowerCase() === "slidepush") {

                } else if (dataPlacement.toLowerCase() === "right" && dataPosition.toLowerCase() === "fixed") {
                    $(dataTarget).addClass(ClassName.RIGHT_FIXED);
                } else {
                    $(dataTarget).addClass(ClassName.LEFT_FIXED);
                }
                if (isopen !== undefined && isopen !== null && (isopen === true || isopen === "true")) {
                    $(dataTarget).addClass(ClassName.OPEN);
                } else {
                    $(dataTarget).removeClass(ClassName.OPEN);
                }
                $(dataTarget + ' ' + Selector.SIDEBAR_DROPDOWN).off();
                $(dataTarget + ' ' + Selector.SIDEBAR_DROPDOWN).on(Event.CLICK, function (event) {
                    event.stopPropagation();
                });
                $(pmdSidebar.prototype.attachParentSelector(Selector.PARENT_SELECTOR, Selector.TOPBAR_TOGGLE)).off(Event.CLICK);
                $(pmdSidebar.prototype.attachParentSelector(Selector.PARENT_SELECTOR, Selector.TOPBAR_TOGGLE)).on(Event.CLICK, function (e) { $(Selector.TOPBAR_FIXED).toggleClass(ClassName.OPEN); });
                $(pmdSidebar.prototype.attachParentSelector(Selector.PARENT_SELECTOR, Selector.TOPBAR_CLOSE)).off(Event.CLICK);
                $(pmdSidebar.prototype.attachParentSelector(Selector.PARENT_SELECTOR, Selector.TOPBAR_CLOSE)).on(Event.CLICK, function () { $(Selector.TOPBAR_FIXED).removeClass(ClassName.OPEN); });
                $this.off(Event.CLICK);
                $this.on(Event.CLICK, onSidebarToggle);
                var isOpenWidth = $this.attr("is-open-width");
                if ($(window).width() < isOpenWidth) {
                    if ($(dataTarget).hasClass(ClassName.LEFT && ClassName.SLIDE_PUSH)) {
                        $(dataTarget).removeClass(ClassName.OPEN + ' ' + ClassName.SLIDE_PUSH);
                        $(dataTarget).addClass(ClassName.LEFT_FIXED + ' ' + ClassName.IS_SLIDEPUSH);
                    } else {
                        $(dataTarget).removeClass(ClassName.OPEN);
                    }
                } else {
                    if ($(dataTarget).hasClass(ClassName.IS_SLIDEPUSH)) {
                        $(dataTarget).addClass(ClassName.OPEN + ' ' + ClassName.SLIDE_PUSH);
                        $(dataTarget).removeClass(ClassName.LEFT_FIXED);
                    } else {
                        //			$(dataTarget).addClass(ClassName.OPEN);
                    }
                }

            });

            $(pmdSidebar.prototype.attachParentSelector(Selector.PARENT_SELECTOR, Selector.NAVBAR_TOGGLE)).off(Event.CLICK);
            $(pmdSidebar.prototype.attachParentSelector(Selector.PARENT_SELECTOR, Selector.NAVBAR_TOGGLE)).on(Event.CLICK, onNavBarToggle);
            $(pmdSidebar.prototype.attachParentSelector(Selector.PARENT_SELECTOR, Selector.OVERLAY)).off(Event.CLICK);
            $(pmdSidebar.prototype.attachParentSelector(Selector.PARENT_SELECTOR, Selector.OVERLAY)).on(Event.CLICK, onOverlayClick);

            //	$(window).unbind("resize");
            //$(window).resize({ param1: options }, onResizeWindow);
            (function (removeClass) {
                jQuery.fn.removeClass = function (value) {
                    if (value && typeof value.test === "function") {
                        for (var i = 0, l = this.length; i < l; i++) {
                            var elem = this[i];
                            if (elem.nodeType === 1 && elem.className) {
                                var classNames = elem.className.split(/\s+/);

                                for (var n = classNames.length; n--;) {
                                    if (value.test(classNames[n])) {
                                        classNames.splice(n, 1);
                                    }
                                }
                                elem.className = jQuery.trim(classNames.join(" "));
                            }
                        }
                    } else {
                        removeClass.call(this, value);
                    }
                    return this;
                };
            })(jQuery.fn.removeClass);
        }
        return pmdSidebar;
    }();


    /**
     * ------------------------------------------------------------------------
     * jQuery
     * ------------------------------------------------------------------------
    */

    var plugInFunction = function (arg) {
        if (this.selector !== "") {
            Selector.PARENT_SELECTOR = this.selector;
        }
        new pmdSidebar(arg);
    };
    $.fn[NAME] = plugInFunction;
    return pmdSidebar;

}(jQuery)();