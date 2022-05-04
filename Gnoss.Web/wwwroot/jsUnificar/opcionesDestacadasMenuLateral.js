var opcionesDestacadasMenuLateral = {
	id: '#usuarioConectadoComunidades',
	idMenuSuperior: '#menuSuperiorComunidades',
	idPanelDesplegable: 'panelDesplegableOpciones',
	cssItem: '.item',
	cssPanel: '.opcionesPanel',
	cssOpcionPrincipal: '.opcionPrincipal',
	cssItemConOpciones: 'itemConOpcionesSegundoNivel',
	idItemOtrasIdentidades: '#otrasIdentidades',
	idIdentidad: '#identidad',
	paneles: [],
	cssActivo: 'activo',
	cssActivoOtras: 'activoOtras',
	timeoutOcultarMenu: '',
	ocultarMenu: false,
	init: function(){
		this.id = $(this.id);
		if(this.id.size() <= 0) return;
		this.crearPanelDesplegable();
		this.config();
		this.customizar();
		this.marcarOpcionesPrincipales();
		this.engancharComportamiento();
		this.engancharOtrasIdentidades();
		return;
	},
	config: function(){
		this.menuSuperior = this.id.find(this.idMenuSuperior);
		this.panelDesplegable = this.id.find('#' + this.idPanelDesplegable);
		this.buscador = $('#cabecera .busqueda');
		this.items= this.id.find(this.cssItem);
		this.opcionesPrincipales = this.id.find(this.cssOpcionPrincipal);
		this.otrasIdentidades = $(this.idItemOtrasIdentidades);
		this.enlaceOtrasIdentidades = this.otrasIdentidades.find('a');
		this.identidades = $(this.idIdentidad);
		return;
	},
	numeroTabs: function(){
		var that = this;
		this.itemsIdentidad = [];
		this.identidades.find('li').each(function(){
			var item = $(this);
			if(item.parents('ul.infoCuenta').size() <= 0){
				that.itemsIdentidad.push(item);
			};
		});
		return this.itemsIdentidad.length;
	},
	customizar: function(){
		var tabs = this.numeroTabs();
		if(tabs == 1) {
			var item = this.itemsIdentidad[0];
			$(item).addClass('identidadUnica');
		}else if(tabs > 1 && tabs <= 4){
			var item = this.itemsIdentidad[1];
			$(item).addClass('activoAnterior');
			item = this.itemsIdentidad[tabs - 1];
			$(item).addClass('ultimoItem');
		}else{
			var item = this.itemsIdentidad[1];
			$(item).addClass('activoAnterior');			
		}
		return;
	},
	crearPanelDesplegable: function(){
		this.id.append('<div id=\"' + this.idPanelDesplegable + '\"><\/div>\n');
		return;
	},
	marcarOpcionesPrincipales: function(){
		var that = this;
		this.items.each(function(){
			var item = $(this);
			var panel = item.find(that.cssPanel);
			if(panel.size() > 0){
				item.addClass(that.cssItemConOpciones);
				that.paneles.push(panel.html());
			} 
		});
		return;
	},
	plantilla: function(numero){
		var html = '';
		html += '<div class=\"opcionesPanel\">\n';
		html += this.paneles[numero];
		html += '<\/div>\n';
		return html;
	},
	mostrarOpcionesItem: function(numero){
		this.panelDesplegable.html(this.plantilla(numero));
		this.panelDesplegable.children('.opcionesPanel').hide();
		$(this.panelDesplegable.children('.opcionesPanel')).slideToggle(1000, function() {
		    this.timeoutOcultarMenu = setTimeout(function() {
                if(this.ocultarMenu)
                {
                    this.ocultarOpcionesItem(parent, that.cssActivo);
                    this.ocultarMenu = false;
                }
            }, 600);
            this.ocultarMenu = true;
		});
		return;
	},
	ocultarOpcionesItem: function(){
	    var that = this;
		$(this.panelDesplegable.children('.opcionesPanel')).slideToggle(1000, function() {
		    that.panelDesplegable.html('');
		});
		return;
	},
	desmarcarOpcionesItem: function(){
		var that = this;
		this.opcionesPrincipales.each(function(){
			$(this).removeClass(that.cssActivo);
		});
		return;
	},
	engancharOtrasIdentidades: function(){
		var that = this;
		var contenidoListadoOtrasIdentidades = this.otrasIdentidades.find('.listadoOtrasIdentidades');
		
		var enlace = that.enlaceOtrasIdentidades;
		var parent = enlace.parent().parent();
		var desplegable = that.panelDesplegable;
			
		//Al entrar en el enlace
		enlace.hover(function() {
		    if(parent.hasClass(that.cssActivoOtras))
		    {
		        clearTimeout(that.timeoutOcultarMenu);
		    }
		},
		//Al salir del enlace
		function() {
		    if(parent.hasClass(that.cssActivoOtras))
		    {
		        that.timeoutOcultarMenu = setTimeout(
		            function() {
		                desplegable.children('.opcionesPanel').slideUp(800, function() {parent.removeClass(that.cssActivoOtras);desplegable.removeClass('desplegarOtrasIdentidades');});
		            }
		         , 600);
		    }
		});	
		
		//Al entrar en el menú desplegado
        desplegable.hover(function() {
            if(parent.hasClass(that.cssActivoOtras))
            {
                clearTimeout(that.timeoutOcultarMenu);
            }
        },
        //Al salir del menú desplegado
        function() {
            if(parent.hasClass(that.cssActivoOtras))
            {
                that.timeoutOcultarMenu = setTimeout(
                    function() {
                        desplegable.children('.opcionesPanel').slideUp(800, function() {parent.removeClass(that.cssActivoOtras);});
                    }
                , 600);
            }
        });
		
		enlace.click(function() {
		    desplegable.children('.opcionesPanel').stop(true);
    	    
	        //Si está desplegado
	        if(parent.hasClass(that.cssActivoOtras))
	        {
	            desplegable.children('.opcionesPanel').slideUp(800, function() {parent.removeClass(that.cssActivoOtras);desplegable.removeClass('desplegarOtrasIdentidades');});
	        }
	        //Si no está desplegado
	        else
	        {
	            //Quitamos el timeout y las clases 'activo' que pueda haber
		        clearTimeout(that.timeoutOcultarMenu);
	            that.id.find('.' + that.cssItemConOpciones + ' ' + that.cssOpcionPrincipal + ' a').each(function(){
	                var enlace = $(this);
		            var parent = enlace.parent();
		            parent.removeClass(that.cssActivo);
	            });
				desplegable.addClass('desplegarOtrasIdentidades');
                
	            //Generamos el contenido del panel desplegable, y lo mostramos
	            parent.addClass(that.cssActivoOtras);
	            var html = '<div class=\"opcionesPanel\">\n' + $(contenidoListadoOtrasIdentidades).html() + '<\/div>\n';
	            desplegable.html(html);
	            desplegable.children('.opcionesPanel').hide();
	            desplegable.css('left',parent.position().left);
	            desplegable.children('.opcionesPanel').slideDown(800);
	        }
	        return false;
		});
		return;
	},
	engancharComportamiento: function(){
		var that = this;
		//Recorremos todos los enlaces del menú y les añadimos los comportamientos
		this.id.find('.' + this.cssItemConOpciones + ' ' + this.cssOpcionPrincipal + ' a').each(function(indice){
			var enlace = $(this);
			var parent = enlace.parent();
			var desplegable = that.panelDesplegable;
			
			//Al entrar en el enlace
			enlace.hover(function() {
			    if(parent.hasClass(that.cssActivo))
			    {
			        clearTimeout(that.timeoutOcultarMenu);
			    }
			},
			//Al salir del enlace
			function() {
			    if(parent.hasClass(that.cssActivo))
			    {
			        that.timeoutOcultarMenu = setTimeout(
			            function() {
			                parent.removeClass(that.cssActivo);
			                desplegable.children('.opcionesPanel').slideUp(800);
			            }
			        , 600);
			    }
			});
			
			//Al entrar en el menú desplegado
	        desplegable.hover(function() {
	            if(parent.hasClass(that.cssActivo))
	            {
	                clearTimeout(that.timeoutOcultarMenu);
	            }
	        },
	        //Al salir del menú desplegado
	        function() {
	            if(parent.hasClass(that.cssActivo))
	            {
	                that.timeoutOcultarMenu = setTimeout(
	                    function() {
	                        parent.removeClass(that.cssActivo);
	                        desplegable.children('.opcionesPanel').slideUp(800);
	                    }
	                , 600);
	            }
	        });
			
			//Al hacer click en el enlace
			enlace.click(function() {
			    desplegable.children('.opcionesPanel').stop(true);
			    that.enlaceOtrasIdentidades.parent().parent().removeClass(that.cssActivoOtras);
			    desplegable.removeClass('desplegarOtrasIdentidades');
			    
			    //Si está desplegado
			    if(parent.hasClass(that.cssActivo))
			    {
			        parent.removeClass(that.cssActivo);
			        desplegable.children('.opcionesPanel').slideUp(800);
			    }
			    //Si no está desplegado
			    else
			    {
			        //Quitamos el timeout y las clases 'activo' que pueda haber
			        clearTimeout(that.timeoutOcultarMenu);
		            that.id.find('.' + that.cssItemConOpciones + ' ' + that.cssOpcionPrincipal + ' a').each(function(){
		                var enlace = $(this);
			            var parent = enlace.parent();
			            parent.removeClass(that.cssActivo);
		            });
		            
			        //Generamos el contenido del panel desplegable, y lo mostramos
			        parent.addClass(that.cssActivo);
			        var html = '<div class=\"opcionesPanel\">\n' + parent.parent().children('.opcionesPanel').html() + '<\/div>\n';
			        desplegable.html(html);
			        desplegable.children('.opcionesPanel').hide();
			        desplegable.css('left',enlace.position().left);
			        desplegable.children('.opcionesPanel').slideDown(800);
			    }
			    return false;
			});
		})
		return;
	}
}
var reemplazarEncabezados = {
    cssGroup: '.group',
    init: function(){
        this.config();
        this.reemplazar();  
    },
    config: function(){
        this.encabezado = subname;
        this.group = $('#section ' + this.cssGroup);
        this.groupFirst = this.group[0];
        this.title = $('h2', this.groupFirst);
    },
    template: function(encabezado){
        var html = '';
        html = '<span class=\"subname\">';
        html += encabezado;
        html += '<\/span>';
        return html;
    },
    reemplazar: function(){
        var encabezado = this.encabezado.text();
        this.title.append(this.template(encabezado));
    }
};
var subname;
$(function(){
	//opcionesDestacadasMenuLateral.init();
	$('#perfilUsuarioGnossCargando').hide();
	$('#perfilUsuarioGnoss').show();
	
	subname = $('h2#subname');
	// Deprecado size()
    //if (subname.size() > 0) reemplazarEncabezados.init();
    if (subname.legnth > 0) reemplazarEncabezados.init();
})