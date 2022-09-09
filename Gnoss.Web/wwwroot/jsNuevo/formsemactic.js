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
 * Date: 02.07.2013.12.30 
 */
var semanticViewGruposColapsables = {
	selector: '.collapsableGroup',
	init: function(){
		this.config();
		this.mecanismo();
		return;
	},
	config: function(){
		this.content = content;
		this.formulario = this.content.find('.semanticView');
		this.grupos = this.formulario.find(this.selector);
		return;
	},
	comportamiento: function(evento){
		var enlace = $(evento.target);
		var group = enlace.parents(this.selector).first();
		var contentGroup = group.find('.contentGroup').first();
		if(contentGroup.hasClass('jsContentGroupHide')){
			group.removeClass('jsGroupHide');
			contentGroup.removeClass('jsContentGroupHide');
			enlace.removeClass('on');
		}else{
			group.addClass('jsGroupHide');
			contentGroup.addClass('jsContentGroupHide');
		}
		return;
	},
	mecanismo: function(){
		var that = this;
		this.grupos.each(function(){
			var mostrar = $('<span>').addClass('mostrar').text('mostrar ');
			var ocultar = $('<span>').addClass('ocultar').text('ocultar ');
			var enlace = $('<a>').addClass('desplegar').attr('href', '#mostrarOcultar').attr('title', 'mostrar / ocultar contenido');
			enlace.append(mostrar);
			enlace.append(ocultar);
			var grupo = $(this).addClass('jsGroupHide');
			var contentGroup = grupo.find('.contentGroup').first().addClass('jsContentGroupHide');
			grupo.append(enlace);
			enlace.bind('click', function(evento){
				evento.preventDefault();
				that.comportamiento(evento);
			})
		})
		return;
	}
}

var customizarLineValues = {
	marcador: '.lineValues',
	init: function(){
		this.config();
		this.customizar();
		return;
	},
	config: function(){
		this.formulario = formulario;
		this.grupos = this.formulario.find(this.marcador);
		return;
	},
	customizar: function(){
		this.grupos.each(function(){
			var grupo = $(this);
			var items = grupo.children();
			var primer = items.first();
			var valor = primer.find('.values');
			var valores;
			items.each(function(indice){
				if(indice == 0) return;
				var item = $(this);
				var values = item.find('.values').css('border','1px solid red');
				valor.append(values.html());
				item.remove();
			})
		})
		return;
	}
}
var customizarTableFormats = {
	marcador: '.groupTableFormat',
	init: function(){
		this.config();
		this.customizar();
		return;
	},
	config: function(){
		this.formulario = formulario;
		this.grupos = this.formulario.find(this.marcador);
		return;
	},
	crearTabla: function(content, tabla){
		content.prepend(tabla);
		return;
	},
	encabezadosTabla: function(tabla, primera){
		var items = primera.children();
		var tr = $('<tr />').addClass('table-tr-header');
		items.each(function(indice){
			var item = $(this);
			var literal = item.find('.lb').first();
			var th = $('<th />').addClass('table-th-0' + indice);
			th.append(literal);
			tr.append(th);
		})
		tabla.append(tr);
		return;
	},
	cuerpoTabla: function(tabla, filas){
		filas.each(function(indice){
			var fila = $(this);
			var items = fila.children();
			var tr = $('<tr />').addClass('table-tr-0' + indice);
			items.each(function(indice){
				var item = $(this);
				var td = $('<td />').addClass('table-td-0' + indice);
				td.append(item);
				tr.append(td);
			});
			tabla.append(tr);
		});
		return;
	},	
	agruparMultievaluados: function(content){
		var filas = content.children();
		filas.each(function(){
			var fila = $(this);
			var grupos = fila.children();
			var marcadorActivo = '';
			var grupoActivo;
			grupos.each(function(indice){
				var grupo = $(this);
				var marcador = grupo.attr('class');
				marcador = marcador.split(' ');
				marcador = marcador[1];
				if(marcadorActivo != marcador){
					var div = $('<div />').addClass('grupo0' + indice);
					div.append(grupo);
					fila.append(div);
					marcadorActivo = marcador;
					grupoActivo = div;
				}else{
					grupoActivo.append(grupo);
				}
			})
		})
		return;
	},
	customizar: function(){
		var that = this;
		this.grupos.each(function(){
			var grupo = $(this);
			var content = grupo.find('.contentGroup');
			that.agruparMultievaluados(content);
			var filas = content.children();
			var primera = filas.first().clone();
			var tabla = $('<table />');
			that.crearTabla(content, tabla);
			that.encabezadosTabla(tabla, primera);
			that.cuerpoTabla(tabla, filas);
		})
		return;
	}
}
var customizarAgruparPropertyValue = {
	marcador: '.groupProperty',
	init: function(){
		this.config();
		this.customizar();
		return;
	},
	config: function(){
		this.formulario = formulario;
		this.grupos = this.formulario.find(this.marcador);
		return;
	},		
	buscarValores: function(grupo, property){
		var grupo = grupo;
		var property = property;
		var longitud = property.length;
		var marcador = property.substring(9, longitud);
		var values = grupo.find('.value');
		var primero;
		var valuesPrimero;
		values.each(function(indice){
			var value = $(this);
			var valueProperty = value.attr('property');
			if(valueProperty != marcador) return;
			if(indice == 0){
				primero = value.parents('div').first();
				valuesPrimero = primero.find('.values');
			}else{
				value.parents('div').first().remove();
				valuesPrimero.append(value);
			}
			
		})
		return;
	},
	customizar: function(){
		var that = this;
		this.grupos.each(function(){
			var grupo = $(this);
			var property = '';
			var marcas = grupo.attr('class').split(' ');
			$.each(marcas, function(indice, valor){
				if(valor.indexOf('property-') >= 0) property = valor;
			});
			if(property == '') return;
			that.buscarValores(grupo, property);
			
		});
		return;
	}
}
var formulario;
$(function(){
	formulario = $('#col02 .semanticView');
	semanticViewGruposColapsables.init();
	customizarTableFormats.init();
	customizarLineValues.init();
	customizarAgruparPropertyValue.init();
})