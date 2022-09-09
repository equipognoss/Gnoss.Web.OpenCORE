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

var isPendingSuccess = false;

var filter = {
	cssFilterSpace: '.filterSpace',
	cssSearchBy: '.searchBy',
	cssTags: '.tags',
	cssCounter: '.counter',
	isAppliedTags: false,
	init: function(){
		this.config();
	},
	config: function(){
		this.filterSpace = $(this.cssFilterSpace);
		this.searchBy = $(this.cssSearchBy, this.filterSpace);
		this.tags = $(this.cssTags, this.filterSpace);
		this.counter = $(this.cssCounter, this.filterSpace);
		this.number = $('strong', this.counter);
		if(!this.isAppliedTags){
			this.searchBy.hide();
			this.tags.hide();
		}
		return;
	},
	templateTag: function(tag){
		var literal = $(tag).text();
		var rel = $(tag).attr('rel');
		var html = '<li>';
		html += literal;
		html += ' <a href="#" rel="' + rel + '" class="remove">eliminar</a>';
		html += '</li>';
		return html;
	},
	addTag: function(tag){
		if(!this.isAppliedTags){
			this.tags.html('');		
			this.searchBy.show();
			this.tags.show();
		};		
		this.tags.append(this.templateTag(tag));
		this.isAppliedTags = true;
		return;
	},
	changeCounter: function(number){
		this.number.html(number);
		return;
	}
}
var results = {
	cssList: '.resource-list',
	init: function(){
		this.config();
	},
	config: function(){
		this.list = $(this.cssList);
		return;
	},
	loading: function(){
		this.list.html(this.templateLoading);
		return;
	},
	addContent: function(html){
		var that = this;
		setTimeout(function(){
			that.list.html(html);
			isPendingSuccess = false;
			// Cambiar por el nuevo Front. Al hacer click en una faceta de tipo tree, cargar los resultados en la zona de contenido
            //modoVisualizacionListados.init('#col02 .resource-list');            
            modoVisualizacionListados.init('#panResultados .resource-list');

		}, 500);
		return;
	},	
    /*
     * Función que se ejecuta cuando se pulsa en una opción de link de tipo Tree o ListTree en Facetas. 
     * Muestra un pensaje de Cargando hasta que los datos son devueltos por el servidor     
     */
	templateLoading: function(){
		//var html = '<p class="loading">procesing linked data...</p>';
        var html = "";
        html += '<div class="align-items-center mb-2">';
        html += '<div ';
        html += 'class="spinner-border texto-primario mr-2"';
        html += 'role="status"';
        html += 'aria-hidden="true"';           
        html += '></div>';
        html += '<strong>Cargando resultados ...</strong>';       
        html += '</div>';
		return html;
	}
}
function templateRecurso(resource){
	html_response = "<div class=\"resource\">";
	html_response += "<div class=\"box description\">";
	html_response += "<div class=\"group title\">";
	html_response += "<h4><a href=\"" + resource.url + "\">" + resource.titulo + "</a></h4>";
	html_response += "<p class=\"resourceType digital\"><span>tipo de documento<\/span><a href=\"#resource\">Archivo digital<\/a><\/p>";
	html_response += "</div>";
	html_response += "<div class=\"group content\">";
	html_response += resource.contenidoBreve;
	html_response += "<\/div>";
	html_response += "<div class=\"group utils-2\">";
	html_response += "<p>Autores: <a href=\"personas.php\">Equipo GNOSS, Dr. Martin Hepp<\/a><\/p>";
	html_response += "<p>Publicado el 21.04.10 por <a href=\"personas.php\">Equipo GNOSS<\/a><\/p>";
	html_response += "<p>Editores: <a href=\"personas.php\">Lina Aguirre, Equipo GNOSS<\/a>, <a href=\"personas.php\">Ricardo Alonso Maturana<\/a><\/p>";
	html_response += "<\/div>";
	html_response += "<div class=\"group categorias\">";
	html_response += "<p>Categorías:<\/p>";
	html_response += "<ul>";
	html_response += "<li><a href=\"recursos.php\">Representación del conocimiento/ Knowledge representation,<\/a><\/li>";
	html_response += "<li><a href=\"recursos.php\">Vocabularios Semánticos/Data Vocabularies,<\/a><\/li>";
	html_response += "<li><a href=\"recursos.php\">Ontologías/ Ontologies,<\/a><\/li>";
	html_response += "<li><a href=\"recursos.php\">Marketing semántico/Semantic marketing<\/a><\/li>";
	html_response += "<\/ul>";
	html_response += "<\/div>";
	html_response += "<div class=\"group etiquetas\">";
	html_response += "<p>Etiquetas: <\/p>";
	html_response += "<ul>";
	html_response += "<li><a href=\"recursos.php\">data,<\/a><\/li>";
	html_response += "<li><a href=\"recursos.php\">e-commerce,<\/a><\/li>";
	html_response += "<li><a href=\"recursos.php\">linked data,<\/a><\/li>";
	html_response += "<li><a href=\"recursos.php\">ontología,<\/a><\/li>";
	html_response += "<li><a href=\"recursos.php\">owl,<\/a><\/li>";
	html_response += "<li><a href=\"recursos.php\">posicionamiento,<\/a><\/li>";
	html_response += "<li><a href=\"recursos.php\">rdf,<\/a><\/li>";
	html_response += "<li><a href=\"recursos.php\">rdfa,<\/a><\/li>";
	html_response += "<li><a href=\"recursos.php\">rdf-s,<\/a><\/li>";
	html_response += "<li><a href=\"recursos.php\">search,<\/a><\/li>";
	html_response += "<li><a href=\"recursos.php\">searchmonkey,<\/a><\/li>";
	html_response += "<li><a href=\"recursos.php\">sem,<\/a><\/li>";
	html_response += "<li><a href=\"recursos.php\">semantic web,<\/a><\/li>";
	html_response += "<li><a href=\"recursos.php\">seo,<\/a><\/li>";
	html_response += "<li><a href=\"recursos.php\">servicios,<\/a><\/li>";
	html_response += "<li><a href=\"recursos.php\">web,<\/a><\/li>";
	html_response += "<li><a href=\"recursos.php\">yahoo<\/a><\/li>";
	html_response += "<\/ul>";
	html_response += "</div>";	
	html_response += "<\/div>";
	html_response += "<\/div>";
	return html_response;
}
function suggest(){
  $.ajax({
    data: "parametro1=valor1&amp;parametro2=valor2",
	cache: false,
    type: "GET",
    dataType: "json",
    url: "includes/recursos/data.php",
    success: function(resources){ 
		var html_response = "";
		$.each(resources, function(indice, resource){
			if(indice == 'resumen'){
				filter.changeCounter(resource.numero);
			}else{
				html_response += templateRecurso(resource);
			}
		});
		results.addContent(html_response);
	},
	error: function(e, xhr){
		//console.log(e);
	}	
   });
}
$(function(){
	filter.init();
	results.init();
	$('.layout02 #facetedSearch .box ul a').each(function(){
		$(this).bind('click', function(event){
			var enlace = $(this);
			var li = enlace.parent().html();
			if(!enlace.hasClass('applied')){
				if(isPendingSuccess == false){
					isPendingSuccess = true;
					filter.addTag(li);
					results.loading();
					enlace.addClass('applied');
					suggest();
				};
			};
			event.preventDefault();
		})
	});
})