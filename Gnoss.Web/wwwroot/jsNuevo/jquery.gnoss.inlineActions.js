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
var addPersonalSpace = {};
var shared = {};
var createVersion = {};

var action = {
	idPanels: '#panels',
	idLoading: '#loading',
	idMenssages: '#menssages',
	idAction: '#action',
	init: function(event){
		this.config(event);
		this.repositionPanels();
		this.marcarBoton();
		this.actionOrderForm();
		this.engancharCloseForm();
	},
	config: function(event){
		this.enlace = $(event.target);
		this.boton = this.enlace.parent();
		this.panelAcciones = this.enlace.parents('div.acciones');
		this.panels = $(this.idPanels);
		this.loading = $(this.idLoading, this.panels);
		this.menssages = $(this.idMenssages, this.panels);
		this.action = $(this.idAction, this.panels);
		this.cerrar = $('p.cerrar a', this.panels);
		return;
	},
	engancharCloseForm: function(){
		var that = this;
		this.cerrar.bind('click',function(){
			that.desmarcarBoton();
			that.stateHideAllPanels();
		});
		return;
	},
	repositionPanels: function(){
		$('div.acciones li.active').each(function(){
			$(this).removeClass('active');
		});
		this.panelAcciones.after(this.panels);
		return;
	},
	marcarBoton: function(){
		this.boton.addClass('active');
		return;
	},
	desmarcarBoton: function(){
		this.boton.removeClass('active');
		return;
	},
	customSubmit: function(){
		var that = this;
		var buttom = $('input[type=submit]', this.action);;
		buttom.bind('click', function(event){
			event.preventDefault();
			that.actionSubmitForm();
		})
	},
	actionOrderForm: function(){
		var that = this;
		this.stateShowLoading();
		setTimeout(function(){
			that.stateShowForm();
			that.customSubmit();
		},600);		
		return;
	},
	actionSubmitForm: function(){
		var that = this;
		this.stateShowLoading();
		setTimeout(function(){
			that.stateShowMenssages();
			that.desmarcarBoton();
		},600)		
		return;
	},
	stateShowLoading: function(){
		this.panels.attr('class','stateLoading');
		this.panels.show();
		this.loading.show();
		this.menssages.hide();
		this.action.hide();
		return;
	},
	stateShowForm: function(){
		this.panels.attr('class','stateShowForm');
		this.loading.hide();
		this.menssages.hide();
		this.action.show();	
		return;	
	},
	stateShowMenssages: function(){
		this.panels.attr('class','stateShowMenssages');
		this.loading.hide();
		this.menssages.show();
		this.action.hide();		
		return;	
	},
	stateHideAllPanels: function(){
		this.panels.attr('class','');
		this.panels.hide();
		this.loading.hide();
		this.menssages.hide();
		this.action.hide();
		return;
	}
};

$(function(){
	$('.inlineActionResource').each(function(){
		var link = $(this);
		link.bind('click', function(event){
			event.preventDefault();
			action.init(event);
		});
	});
	$('.cerrar a').each(function(){
		var link = $(this);
		link.bind('click', function(event){
			event.preventDefault();
			action.init(event);
		});
	});	
	
})