var globalIsContrayendo = false;
var confirmacionEliminacionMultiple = {
	cssConfirmacion: '.confirmacionMultiple',
	items: [
		'.listToolBar .delete a'
	],	
	init: function(){
		this.config();
		return;
	},
	config: function(){
		this.confirmacion = $(this.cssConfirmacion);
		var that = this;
		$.each(this.items, function(indice){
			var item = $(that.items[indice]);
			item.bind('click', function(evento){
				that.engancharEvento(evento);
				evento.preventDefault();
			});
		});
		return;
	},
	engancharEvento: function(evento){
		this.mostrarConfirmacion();
		return;
	},
	mostrarConfirmacion: function(){
		this.confirmacion.show();
	}
}
var confirmacionEliminacionSencilla = {
	cssConfirmacion: '.confirmacionSencilla',
	items: [
		'.resource-list .resource .delete'
	],	
	init: function(){
		this.config();
		return;
	},
	config: function(){
		this.confirmacion = $(this.cssConfirmacion);
		this.pregunta = this.confirmacion.find('.pregunta');
		var that = this;
		$.each(this.items, function(indice){
			var item = $(that.items[indice]);
			item.bind('click', function(evento){
				that.engancharEvento(evento);
				evento.preventDefault();
			});
		});
		return;
	},
	engancharEvento: function(evento){
		this.currentBoton = $(evento.target);
		this.encontrarRecurso();
		this.mostrarConfirmacion();
		return;
	},
	encontrarRecurso: function(){
		this.currentRecurso = this.currentBoton.parents('.resource');
		return;
	},
	mostrarConfirmacion: function(){
		var altura = this.currentRecurso.height() + 'px';
		var margin = (this.currentRecurso.height() / 3) + 'px';
		var top = this.currentRecurso.position().top + 'px';
		this.confirmacion.css({
			'height': altura,
			'top': top
		});
		this.pregunta.css('margin-top', margin);
		
		this.confirmacion.show();
	}
}
var desplegableUsuario = {
	idEspacios: '#usuarioConectado',
	cssEnlace: '.enlaceDesplegable',
	cssDesplegable: '.desplegable',
	cssListadoAmpliado: '.listadoAmpliado',
	init: function(){
		this.config();
		this.engancharComportamiento();
	},
	config: function(){
		this.espacios = $(this.idEspacios);
		this.enlaces = this.espacios.find(this.cssEnlace);
	},
	engancharComportamiento: function(){
		var that = this;
		this.enlaces.each(function(indice){
			var enlace = $(this);
			enlace.bind('click', function(evento){
				evento.preventDefault();
				that.comportamiento(evento);
			});
		});
		return;
	},
	ocultarTodos: function(){
		this.enlaces.each(function(indice){
			var parrafo = $(this);
			var div = parrafo.parent();
			parrafo.removeClass('activo');
			div.removeClass('showDesplegable');
		});
		return;
	},
	comportamiento: function(evento){
		var parrafo = $(evento.target);
		if(!parrafo.is('P')){
			parrafo = parrafo.parents('p').first();
		};
		var div = parrafo.parent();
		if(!parrafo.hasClass('activo')) this.ocultarTodos();
		parrafo.hasClass('activo') ? parrafo.removeClass('activo') : parrafo.addClass('activo');
		div.hasClass('showDesplegable') ? div.removeClass('showDesplegable') : div.addClass('showDesplegable');
		return;	
	}
}

var desplegablesEspacios = {
	idEspacios: '#espacios',
	cssEnlace: '.enlaceDesplegable',
	cssDesplegable: '.desplegable',
	cssListadoAmpliado: '.listadoAmpliado',
	init: function(){
		this.config();
		this.engancharComportamiento();
	},
	config: function(){
		this.espacios = $(this.idEspacios);
		this.enlaces = this.espacios.find(this.cssEnlace);
	},
	engancharComportamiento: function(){
		var that = this;
		this.enlaces.each(function(indice){
			var enlace = $(this);
			enlace.bind('click', function(evento){
				evento.preventDefault();
				that.comportamiento(evento);
			});
		});
		return;
	},
	ocultarTodos: function(){
		this.enlaces.each(function(indice){
			var enlace = $(this);
			var li = enlace.parent();
			enlace.removeClass('activo');
			li.removeClass('showDesplegable');
		});
		return;
	},
	comportamiento: function(evento){
		var enlace = $(evento.target);
		var li = enlace.parent();
		if(!enlace.hasClass('activo')) this.ocultarTodos();
		enlace.hasClass('activo') ? enlace.removeClass('activo') : enlace.addClass('activo');
		li.hasClass('showDesplegable') ? li.removeClass('showDesplegable') : li.addClass('showDesplegable');
		return;	
	}
}

var desplegablesEspaciosGNOSS = {
	idEspacios: '#identidadGNOSS',
	cssEnlace: '.enlaceDesplegable',
	cssDesplegable: '.desplegable',
	cssListadoAmpliado: '.listadoAmpliado',
	init: function(){
		this.config();
		this.engancharComportamiento();
	},
	config: function(){
		this.espacios = $(this.idEspacios);
		this.enlaces = this.espacios.find(this.cssEnlace);
	},
	engancharComportamiento: function(){
		var that = this;
		this.enlaces.each(function(indice){
			var enlace = $(this);
			enlace.bind('click', function(evento){
				evento.preventDefault();
				that.comportamiento(evento);
			});
		});
		return;
	},
	ocultarTodos: function(){
		this.enlaces.each(function(indice){
			var enlace = $(this);
			var li = enlace.parent();
			enlace.removeClass('activo');
			li.removeClass('showDesplegable');
		});
		return;
	},
	comportamiento: function(evento){
		var enlace = $(evento.target);
		var li = enlace.parent();
		if(!enlace.hasClass('activo')) this.ocultarTodos();
		enlace.hasClass('activo') ? enlace.removeClass('activo') : enlace.addClass('activo');
		li.hasClass('showDesplegable') ? li.removeClass('showDesplegable') : li.addClass('showDesplegable');
		return;	
	}
}

var desplegablesOtrasIdentidades = {
	idEspacios: '#otrasIdentidades',
	cssEnlace: '.enlaceDesplegable',
	cssDesplegable: '.desplegable',
	cssListadoAmpliado: '.listadoAmpliado',
	init: function(){
		this.config();
		this.engancharComportamiento();
	},
	config: function(){
		this.espacios = $(this.idEspacios);
		this.enlaces = this.espacios.find(this.cssEnlace);
	},
	engancharComportamiento: function(){
		var that = this;
		this.enlaces.each(function(indice){
			var enlace = $(this);
			enlace.bind('click', function(evento){
				evento.preventDefault();
				that.comportamiento(evento);
			});
		});
		return;
	},
	ocultarTodos: function(){
		var that = this;
		this.enlaces.each(function(indice){
			var enlace = $(this);
			enlace.removeClass('activo');
			that.espacios.removeClass('showDesplegable');
		});
		return;
	},
	comportamiento: function(evento){
		var enlace = $(evento.target);
		if(!enlace.hasClass('activo')) this.ocultarTodos();
		enlace.hasClass('activo') ? enlace.removeClass('activo') : enlace.addClass('activo');
		this.espacios.hasClass('showDesplegable') ? this.espacios.removeClass('showDesplegable') : this.espacios.addClass('showDesplegable');
		return;	
	}
}

var ampliarContraerListados = {
	css: '.mostrarListadoAmpliado',
	cssVerTodas: '.mostrarListadoTodos',
	init: function(){
		this.config();
		this.engancharComportamiento();
	},
	config: function(){
		this.enlaces = $(this.css);
	},
	engancharComportamiento: function(){
		var that = this;
		this.enlaces.each(function(indice){
			var parrafo = $(this);
			var enlace = parrafo.find('a');
			enlace.bind('click', function(evento){
				evento.preventDefault();
				that.comportamiento(evento);
			});					
		});
	},
	comportamiento: function(evento){
		var enlace = $(evento.target);
		var contenedor = enlace.parent().parent();
		var ulAmpliado = contenedor.find('ul.listadoAmpliado');
		var ulListado = ulAmpliado.prev();
		var pVerTodos = enlace.parent().next();
		var hasVerTodos = pVerTodos.size() > 0;
		var texto = enlace.text();
		var textoAlt = enlace.attr('rel');
		enlace.text(textoAlt);
		enlace.attr('rel', texto);
		if(ulAmpliado.is(':visible')){
			ulAmpliado.hide();
			globalIsContrayendo = true;
		}else{
			if(hasVerTodos){
				ulListado.hide();
				enlace.parent().hide();
				pVerTodos.show();
				ulAmpliado.show();
				
			}else{
				ulAmpliado.show();
			}
			globalIsContrayendo = false;
		}
	}
}

var ocultarDesplegables = {
	idPage: '#page',
	init: function(){
		$(this.idPage).hover( function(){
			if(globalIsContrayendo) return;
			desplegablesEspacios.ocultarTodos();
			desplegableUsuario.ocultarTodos();
			desplegablesOtrasIdentidades.ocultarTodos();
			desplegablesEspaciosGNOSS.ocultarTodos();
			desplegablesIdentidad.ocultarTodos();
		},function(){});
	}
}

var hoverDesplegable = {
	desplegables: '.desplegable',
	init: function(){
		$(this.desplegables).each(function(){
			var desplegable = $(this);
			desplegable.hover(
			function(){
				globalIsContrayendo = false;
			},function(){});			
		});
	}
}

var montarModuloOtrasIdentidades = {
	id: '#otrasIdentidades',
	cssDesplegable: '.panelMasIdentidades',
	n: 4,
	isMuchasIdentidades: false,
	init: function(){
		this.config();
		this.determinarItemsAMostrar();
	},
	config: function(){
		this.modulo = $(this.id);
		this.desplegable = this.modulo.find(this.cssDesplegable);
		this.ulVisible = this.modulo.find('ul').first();
		this.botonMasIdentidades = this.ulVisible.find('li.masIdentidades');
		this.ul = this.desplegable.find('ul').first();
		this.lis = this.ul.find('li');
		return;
	},
	determinarItemsAMostrar: function(){	
		// Deprecado size() 
        // var numeroItems = this.lis.size();
        var numeroItems = this.lis.length;
		if(numeroItems == 0){
			this.modulo.remove();
		}else{
			if(numeroItems > this.n) this.isMuchasIdentidades = true;
			this.mostrarLosNPrimeros()
		}
		return;
	},
	mostrarLosNPrimeros: function(){
		var that = this;
		if(!this.isMuchasIdentidades){
			this.desplegable.remove();
			this.botonMasIdentidades.remove();
			this.lisMostrar = this.lis.splice(0, this.n);
		}else{
			this.lisMostrar = this.lis.splice(0, this.n - 1);			
		}	
		this.lisMostrar = $(this.lisMostrar.reverse());
		this.lisMostrar.each(function(indice){
			var li = $(this);
			that.ulVisible.prepend(li);
		});
		return;
	}
}
marcarItemsDesplegables = {
	css: '.enlaceDesplegable',
	init: function(){
		this.comportamiento();
	},
	config: function(){
		return;
	},
	comportamiento: function(){
		var enlaces = $(this.css);
		enlaces.each(function(){
			var enlace = $(this);
			if(enlace.parents('.identidadGNOSS').size() > 0) return;
			enlace.addClass('menuDesplegable');
		});
		return;
	}
}
var marcarIdentidadProfesor = {
	cssProfesor: '.profesor',
	init: function(){
		this.config();
		this.marcar();
	},
	config: function(){
		this.identidades = $('#otrasIdentidades ' + this.cssProfesor);
		return;
	},
	marcar: function(){
		this.identidades.each(function(){
			var identidad = $(this);
			var title = identidad.attr('title');
			var li = identidad.parent();
			li.css('position','relative');
			li.append('<span class="identidadProfesor" title="' + title + '">identidad profesor</span>');
		});
		return;
	}
}

var desplegablesIdentidad = {
	idEspacios: '#identidad',
	cssEnlace: '.enlaceDesplegable',
	cssDesplegable: '.desplegable',
	cssListadoAmpliado: '.listadoAmpliado',
	init: function(){
		this.config();
		this.engancharComportamiento();
	},
	config: function(){
		this.espacios = $(this.idEspacios);
		this.enlaces = this.espacios.find(this.cssEnlace);
	},
	engancharComportamiento: function(){
		var that = this;
		this.enlaces.each(function(indice){
			var enlace = $(this);
			enlace.bind('click', function(evento){
				evento.preventDefault();
				that.comportamiento(evento);
			});
		});
		return;
	},
	ocultarTodos: function(){
		this.enlaces.each(function(indice){
			var enlace = $(this);
			var li = enlace.parent();
			enlace.removeClass('activo');
			li.removeClass('showDesplegable');
		});
		return;
	},
	comportamiento: function(evento){
		var enlace = $(evento.target);
		var li = enlace.parent();
		if(!enlace.hasClass('activo')) this.ocultarTodos();
		enlace.hasClass('activo') ? enlace.removeClass('activo') : enlace.addClass('activo');
		li.hasClass('showDesplegable') ? li.removeClass('showDesplegable') : li.addClass('showDesplegable');
		return;	
	}
}
var buscadorCabecera = {
    idBuscador: '#buscador',
    cssSearchGroup: '.searchGroup',
    literales: [],
    init: function (id) {
        var buscador = id || this.idBuscador;
        var that = this;
        this.buscador = $(buscador);
        this.config();
        this.wrapBuscador.show();
        this.anchoSearchGroup = this.searchGroup.width();
        // Nuevo Front. Cargaba recursos, y m�s opciones cuando el usuario no estaba registrado (Lo montaba despu�s del buscador). No tiene sentido
        //this.montarSelector();
        // A simple vista, no monta ninguna opci�n. Parece ser un submen�. Ahora con nuevo Front no har�a falta.
        //this.montarOpciones();
        this.marcarDefaultInput();
        // Asocia clase cuando hay "focus". Lo elimino de momento
        //this.engancharInput();
        // Comportamiento para paneles y subpaneles (Desplegado, no desplegado). Para el nuevo Front, lo elimino de momento
        //this.engancharSelector();
        // C�lculo din�mico de paneles del viejo front. De momento lo oculto
        //this.engancharOpciones();
        // Comportamiento de establecer "selected" a opciones. Para el nuevo Front, lo elimino de momento
        //this.defaultSeleccionado();
        // Calcular ancho para el input de buscador. Para el nuevo Front, de momento lo elimino
        /*setTimeout(function () {
            that.calcularAnchoInputDisponible();
        }, 200)*/
        return;
    },
    config: function () {
        this.wrapBuscador = $('fieldset', this.buscador);
        this.searchGroup = $(this.cssSearchGroup, this.buscador);
        this.defaultSelect = $('select', this.buscador);
        this.defaultOptions = $('option', this.defaultSelect);
        this.defaultInput = $('input[type=text]', this.buscador);
        this.anchoBuscador = this.buscador.width();
        return;
    },
    marcarDefaultInput: function () {
        this.defaultInput.addClass('defaultText');
        this.defaultInputText = this.defaultInput.attr('value');
        return;
    },
    defaultSeleccionado: function () {
        var that = this;
        this.defaultOptions.each(function (indice) {
            var option = $(this);
            if (option.attr('selected')) that.enlaceSelector.html(that.literales[indice]);
        });
    },
    engancharInput: function () {
        var that = this;
        this.defaultInput.bind('focus', function () {
            var texto = that.defaultInput.val();
            if (texto == '' || texto == that.defaultInputText) {
                that.defaultInput.attr('value', '');
                that.defaultInput.removeClass('defaultText');
            }
        });
        this.defaultInput.bind('blur', function () {
            var texto = that.defaultInput.val();
            if (texto == '' || texto == that.defaultInputText) {
                that.defaultInput.addClass('defaultText');
                that.defaultInput.attr('value', that.defaultInputText);
            }
        });
        return;
    },
    deseleccionar: function () {
        this.defaultOptions.each(function () {
            $(this).removeAttr('selected');
        });
        return;
    },
    engancharOpciones: function () {
        var that = this;
        var enlaces = $('a', this.opciones);
        enlaces.each(function () {
            var enlace = $(this);
            enlace.bind('click', function (evento) {
                evento.preventDefault();
                enlace = $(evento.target);
                var indice = enlace.attr('rel');
                that.enlaceSelector.html(that.literales[indice]);
                that.deseleccionar();
                var opcionSeleccionada = that.defaultOptions[indice];
                $(opcionSeleccionada).attr('selected', 'selected');
                that.defaultSelect[0].selectedIndex = indice;
                that.calcularAnchoInputDisponible();
                if (typeof (window.PreparaAutoCompletarComunidad) == 'function') {
                    PreparaAutoCompletarComunidad();
                }
            });
        })
        return;
    },
    calcularAnchoInputDisponible: function () {
        var anchoBuscador = this.anchoBuscador;
        var anchoSearchGroup = this.anchoSearchGroup;
        var anchoSelector = this.selector.width();
        var anchoDefaultInput = anchoSearchGroup - (anchoSelector + 54);
        // Si el usuario no está en ninguna comunidad, el buscador se quedaba en 0px (No tiene sentido) -> Cambiado por nuevo Front
        //this.defaultInput.css('width', anchoDefaultInput + 'px');
        return;
    },
    engancharSelector: function () {
        var that = this;
        this.selector.hover(function () {
            that.opciones.show();
            that.selector.addClass('desplegado');
            that.calcularAnchoInputDisponible();
        }, function () {
            that.opciones.hide();
            that.selector.removeClass('desplegado');
            that.calcularAnchoInputDisponible();
        });
        return;
    },
    plantillaSelector: function () {
        var html = '<div id=\"selector\">';
        html += '<p class=\"seleccionado\"><a href=\"#\"><\/a><span><\/span><\/p>';
        html += '<div id=\"opciones\">';
        html += '<\/div>';
        html += '<\/div>';
        return html;
    },
    montarSelector: function () {
        this.searchGroup.prepend(this.plantillaSelector());
        this.selector = $('#selector', this.buscador);
        this.enlaceSelector = $('a', this.selector);
        this.opciones = $('#opciones', this.selector);
        return;
    },
    montarOpciones: function () {
        var that = this;
        var options = $('option', this.defaultSelect);
        var html = '<ul>';
        options.each(function (indice) {
            var opcion = $(this);
            var literal = opcion.html();
            that.literales[indice] = literal;
            html += '<li><a href=\"#\" rel=\"' + indice + '\">';
            html += literal;
            html += '<\/li><\/a>';
        });
        if (options.length == 1) {
            this.selector.hide();
        }
        this.opciones.html(html);
        return;
    }
}
$(function () {

    /* buscador */
    buscadorCabecera.init();

	confirmacionEliminacionMultiple.init();
	confirmacionEliminacionSencilla.init();
	
	marcarIdentidadProfesor.init();
	montarModuloOtrasIdentidades.init();
	desplegablesIdentidad.init();
	desplegablesEspacios.init();
	desplegableUsuario.init();
	desplegablesOtrasIdentidades.init();
	desplegablesEspaciosGNOSS.init();
	ampliarContraerListados.init();
	ocultarDesplegables.init();
	hoverDesplegable.init();
	marcarItemsDesplegables.init();
});