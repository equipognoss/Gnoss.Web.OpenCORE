var componentePaginado = {
	init: function(){
		this.content = $(content);
		this.componentes = this.content.find('.componentePaginado');
		if(this.componentes.length <= 0) return;
		this.ocultar();
		this.engancharPaginador();
	},
	carruselActivo: function(enlace){
		var enlace = enlace;
		this.componenteActivo = enlace.parents('.componentePaginado').first();
		this.itemsComponenteActivo = this.componenteActivo.find('.componente');
		this.paginadorActivo = this.componenteActivo.find('.paginador');
		this.paginadorContadorActivo = this.componenteActivo.find('.paginadorContador');
		this.contadorActual = this.paginadorContadorActivo.find('.actual');
		this.contadorTotal = this.paginadorContadorActivo.find('.total');
		return;
	},
	ocultarTodosItems: function(){
		this.itemsComponenteActivo.each(function(){
			var item = $(this);
			item.hide();
		});
		return;
	},
	mostrarItem: function(siguiente){
		var item = this.itemsComponenteActivo[siguiente];
		item = $(item);
		item.show();
		return;
	},	
	paginarAnterior: function(evento){
		var enlace = $(evento.target);
		this.carruselActivo(enlace);
		var actual = parseInt(this.contadorActual.text()) - 1;
		var total = parseInt(this.contadorTotal.text());
		var anterior = actual;
		if(anterior <= 0) anterior = total;
		this.contadorActual.text(anterior);
		this.ocultarTodosItems();
		this.mostrarItem(anterior - 1);
		return;
	},	
	paginarSiguiente: function(evento){
		var enlace = $(evento.target);
		this.carruselActivo(enlace);
		var actual = parseInt(this.contadorActual.text()) - 1;
		var total = parseInt(this.contadorTotal.text());
		var siguiente = actual + 1;
		if(siguiente >= total) siguiente = 0;
		this.contadorActual.text(siguiente + 1);
		this.ocultarTodosItems();
		this.mostrarItem(siguiente);
		return;
	},
	ocultar: function(){
		var that = this;
		this.componentes.each(function(indice){
			var componente = $(this);
			var items = componente.find('.componente');
			var paginador = componente.find('.paginador').addClass('jsActivado');
			var anterior = paginador.find('.paginadorAnterior a');
			var siguiente = paginador.find('.paginadorSiguiente a');
			var total = paginador.find('.total');
			var vertodas = componente.find('.listadoCompleto');
			if(vertodas) componente.addClass('paginadorConVerTodas');
			componente.addClass('jsComponentePaginado' + indice);
			var col = componente.parents('.col').first().addClass('jsColComponentePaginado' + indice);
			var row = componente.parents('.row').addClass('jsRowComponentePaginado' + indice);
			total.text(items.size());
			items.each(function(indice){
				var item = $(this);
				if(indice > 0) item.hide()
			});
			anterior.bind('click', function(evento){
				that.paginarAnterior(evento);
				evento.preventDefault();
			});
			siguiente.bind('click', function(evento){
				that.paginarSiguiente(evento);
				evento.preventDefault();
			})			
		})	
	},
	engancharPaginador: function(){
		return;
	}
}
var componenteTabulado = {
	init: function(){
		this.content = $(content);
		this.componentes = this.content.find('.tabsGroup');
		if(this.componentes.length <= 0) return;
		this.comportamiento();
		return;
	},
	ocultar: function(bloques){
		var bloques = bloques;
		bloques.each(function(){
			var bloque = $(this).hide();
		});
		return;
	},
	mostrar: function(bloques, numero){
		var bloques = bloques;
		var bloque = $(bloques[numero]);
		bloque.show();
		return;
	},
	desmarcar: function(tabs){
		var tabs = tabs;
		var lis = tabs.find('li');
		lis.each(function(){
			var item = $(this);
			item.removeClass('active');
		});
		return;
	},
	ocultarMostrarBloques: function(enlace){
		var enlace = enlace;
		var grupo = enlace.parents('.tabsGroup').first();
		var tabs = grupo.find('.tabspresentation').first();
		var bloques = grupo.children('.block');
		var numero = enlace.attr('rel');
		var li = enlace.parent();
		this.ocultar(bloques);
		this.mostrar(bloques, numero);
		this.desmarcar(tabs);
		li.addClass('active');
		return;
	},
	comportamiento: function(){
		var that = this;
		this.componentes.each(function(indice){
			var componente = $(this);
			var tabs = componente.children('.tabspresentation');
			var bloques = componente.children('.block');
			var enlaces = tabs.find('a');
			var enlaceActivo = tabs.find('li.active a');
			enlaces.each(function (indice) {
			    var enlace = $(this);
			    enlace.attr('rel', indice);
			    enlace.bind('click', function (evento) {
			        evento.preventDefault();
			        var enlace = $(evento.target);
			        that.ocultarMostrarBloques(enlace);
			    });
			});
			bloques.each(function(indice){
				var bloque = $(this);
				bloque.attr('rel', indice);
				if(indice != enlaceActivo.attr('rel')) bloque.hide();
			});
		})		
		return;
	}
}
$(function(){
	if(typeof body == 'undefined') 					body = $('body');
	if(typeof header == 'undefined') 				header = body.find('#header');
	if(typeof page == 'undefined') 					page = body.find('#page');
	if(typeof footer == 'undefined') 				footer = body.find('#footer');
	if(typeof section == 'undefined') 				section = page.find('#section');
	if(typeof content == 'undefined') 				content = page.find('#content');
	if(typeof isHome == 'undefined') 				isHome = false;
	if(typeof isListado == 'undefined') 			isListado = false;
	if(typeof isFicha == 'undefined') 				isFicha = false;
	if(typeof isOperativaRegistro == 'undefined') 	isOperativaRegistro = false;
	if(typeof isUsuarioLogeado == 'undefined')		isUsuarioLogeado = false;

	componentePaginado.init();
	componenteTabulado.init();
	
	if(body.hasClass('homeComunidad')){
		isHome = true;
		if(isUsuarioLogeado) body.addClass('homeUsuarioConectado');
	}else if(body.hasClass('operativaRegistro')){
		isOperativaRegistro = true; 
	}else if(body.hasClass('listadoComunidad')){
		isListado = true;
	}else if(body.hasClass('fichaComunidad')){
		isFicha = true;
	}	
})