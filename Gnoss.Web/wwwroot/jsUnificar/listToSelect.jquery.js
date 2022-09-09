/*
 * list to select  beta
 *
 * Copyright (c) 2011 felix tuesta
 *
 * Date: 14 / 02 / 2011
 * Library: jQuery
 * 
 */
(function($){
	$.fn.listToSelect = function(options){
		var defaults = { 
			start: -1,
			cssListaPlegada: 'plegada'			
		};
		var options = $.extend(defaults, options);  

		var componente = $(this);
		var lista = $('ul', componente);
		var boton = null;        

		return this.each(function(){
			initialize();
		});
		function initialize(){
			boton = $('p.desplegar a', componente);	
			comportamientoBotonDesplegar();			
		}
		
		function plegarDesplegarLista(){
			lista.hasClass(options.cssListaPlegada) ? lista.removeClass(options.cssListaPlegada) : lista.addClass(options.cssListaPlegada) ;
		}
		function comportamientoBotonDesplegar(){
			boton.bind('click', function(){		
			    plegarDesplegarLista();	
				this.blur();
				return false;
			})
		}
	};
})(jQuery);