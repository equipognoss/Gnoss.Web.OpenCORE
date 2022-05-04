/*
..........................................................................
:: Links en ventana nueva                                               ::
..........................................................................
*/
$(document).ready(function() {
	$("a[rel=external]").attr({target: "_blank"})
});

var RecargarCKEditorInicio = true;

function RecargarTodosCKEditor() {
    RecargarCKEditorInicio = false;

    if (typeof (ckeCompletoComentarios) != 'undefined' && ckeCompletoComentarios == true) {
        $('textarea.cke.comentarios').removeClass('comentarios').addClass('recursos');
    }

    var textAreas = $('textarea.cke');

    DestruirTodosCKEditor();

    if (textAreas.length > 0) {
		var urlbase = $('input.inpt_baseURL').val();

		if (document.URL.indexOf('https://') == 0) {
			if (urlbase.indexOf('https://') == -1) {
				urlbase = urlbase.replace('http', 'https');
			}
		}
        var BasePath = CKEDITOR.basePath;
		var ImageBrowseUrl = urlbase + "/conector-ckeditor";
		var ImageUploadUrl = urlbase + "/conector-ckeditor";
		//var ImageBrowseUrl = urlbase + "/ConectorCKEditor.aspx";
		//var ImageUploadUrl = urlbase + "/ConectorCKEditor.aspx";
        //var ImageBrowseUrl = BasePath + "filemanager/browser/default/browser.html?Type=Image&Connector=" + BasePath + "filemanager/connectors/aspx/connector.aspx";
        //var ImageUploadUrl = BasePath + "filemanager/connectors/aspx/upload.aspx?Type=Image";
        var lang = $('#inpt_Idioma').val();

		$('textarea.cke.mensajes').ckeditor(function () { }, { language: lang, toolbar: 'Gnoss-Mensajes', filebrowserImageBrowseUrl: ImageBrowseUrl, filebrowserImageUploadUrl: ImageUploadUrl });
        $('textarea.cke.recursos').ckeditor(function () { }, { language: lang, toolbar: 'Gnoss-Recursos', filebrowserImageBrowseUrl: ImageBrowseUrl, filebrowserImageUploadUrl: ImageUploadUrl });
        $('textarea.cke.blogs').ckeditor(function () { }, { language: lang, toolbar: 'Gnoss-Blogs', filebrowserImageBrowseUrl: ImageBrowseUrl, filebrowserImageUploadUrl: ImageUploadUrl });
        $('textarea.cke.comentarios').ckeditor(function () { }, { language: lang, toolbar: 'Gnoss-Comentario' });
    }
}


function DestruirTodosCKEditor() {
    var textAreas = $('textarea.cke'),
        instanceName = "",
        i = 0;

    for (i = 0; i < textAreas.length; i++) {
        instanceName = textAreas[i].id;

        if (CKEDITOR.instances[instanceName] != null) {
            try {
                //El remove no funciona...
                //CKEDITOR.remove(CKEDITOR.instances[instanceName]);
                CKEDITOR.instances[instanceName].destroy();
            }
            catch (error) {
            }
        }
    }
}

function RecargarCKEditor(id) {
    var textArea = $('textarea.cke#' + id);

    DestruirCKEditor(id);

	var BasePath = CKEDITOR.basePath;
	var ImageBrowseUrl = BasePath + "filemanager/browser/default/browser.html?Type=Image&Connector=" + BasePath + "filemanager/connectors/aspx/connector.aspx";
	var ImageUploadUrl = BasePath + "filemanager/connectors/aspx/upload.aspx?Type=Image";
	var lang = $('#inpt_Idioma').val();

	$('textarea.cke.mensajes#' + id).ckeditor(function () { }, { language: lang, toolbar: 'Gnoss-Mensajes' });
	$('textarea.cke.recursos#' + id).ckeditor(function () { }, { language: lang, toolbar: 'Gnoss-Recursos', filebrowserImageBrowseUrl: ImageBrowseUrl, filebrowserImageUploadUrl: ImageUploadUrl });
	$('textarea.cke.blogs#' + id).ckeditor(function () { }, { language: lang, toolbar: 'Gnoss-Blogs', filebrowserImageBrowseUrl: ImageBrowseUrl, filebrowserImageUploadUrl: ImageUploadUrl });
	$('textarea.cke.comentarios#' + id).ckeditor(function () { }, { language: lang, toolbar: 'Gnoss-Comentario' });

}

function DestruirCKEditor(id) {
	if(CKEDITOR.instances[id] != null){
		CKEDITOR.instances[id].destroy();
	}
}

/*
..........................................................................
:: Comprobar email valido                                               ::
..........................................................................
*/
function validarEmail(sTesteo) {
    //var reEmail = /^(?:\w+\.?)*\w+@(?:\w+\.)+\w+$/;
    var reEmail = /^\w+([\.\-ñÑ+]?\w+)*@\w+([\.\-ñÑ]?\w+)*(\.\w{2,})+$/;
    return reEmail.test(sTesteo);
}

function validarFecha( sFecha ) {
    var reFecha = /\b(0?[1-9]|[12][0-9]|3[01])\/([1-9]|0[1-9]|1[0-2])\/(19|20\d{2})/;
    return reFecha.test( sFecha );
}

//Valida una fecha y además comprueba si es bisiesto o no el año.
function esFecha(dia,mes,anio) /* Devuelve si una fecha pasada sus tres parámetros [dia,mes,año] es válida */
{
	//Creo la cadena con la fecha en formato "dd/mm/yyyy"
	if(dia < 10)
		var miDia = '0' + dia;
	else
		var miDia = dia;
	if(mes < 10)
		var miMes = '0' + mes;
	else
		var miMes = mes;
	
	var miFecha = miDia + '/' + miMes + '/' + anio;

	//Comprobamos si es un formato correcto
	var objRegExp = /^([0][1-9]|[12][0-9]|3[01])(\/|-)(0[1-9]|1[012])\2(\d{4})$/;

	if(!objRegExp.test(miFecha))
	{
		return false; //Es una fecha incorrecta porque no cumple el formato
	}
	else
	{
		var strSeparator = miFecha.substring(2,3);

		//Creamos el array con los parámetros de la fecha [dia,mes,año]
		var arrayDate = miFecha.split(strSeparator);
		//Array con el número de días que tiene cada mes excepto febrero que se valida aparte
		var arrayLookup = { '01' : 31,'03' : 31,'04' : 30,'05' : 31, '06' : 30,'07' : 31,
		'08' : 31,'09' : 30,'10' : 31,'11' : 30,'12' : 31}

		var intDay = parseInt(arrayDate[0],10);
		var intMonth = parseInt(arrayDate[1],10);
		var intYear = parseInt(arrayDate[2],10);

		//Comprobamos que el valor del día y del mes sean correctos
		if (intMonth != null) 
		{
			if(intMonth != 2)
			{
				if (intDay <= arrayLookup[arrayDate[1]] && intDay != 0)
				{
					return true;
				}
			}
		}

		//Comprobamos si es febrero y si el valor del día es correcto [Cambia si es bisiesto o no el año]
		if (intMonth == 2)
		{
			if (intDay > 0 && intDay < 29)
			{
				return true;
			}
			else if (intDay == 29)
			{
				if ((intYear % 4 == 0) && (intYear % 100 != 0) || (intYear % 400 == 0))
				{
					return true;
				}
			}
		}
	}

	return false; //Cualquier otro valor, falso
}

//Validar una URL
function esURL(sURL)
{
    var regexURL = /^((([hH][tT][tT][pP][sS]?|[fF][tT][pP])\:\/\/)?([\w\.\-]+(\:[\w\.\&%\$\-]+)*@)?((([^\s\(\)\<\>\\\"\.\[\]\,@;:]+)(\.[^\s\(\)\<\>\\\"\.\[\]\,@;:]+)*(\.[a-zA-Z]{2,4}))|((([01]?\d{1,2}|2[0-4]\d|25[0-5])\.){3}([01]?\d{1,2}|2[0-4]\d|25[0-5])))(\b\:(6553[0-5]|655[0-2]\d|65[0-4]\d{2}|6[0-4]\d{3}|[1-5]\d{4}|[1-9]\d{0,3}|0)\b)?((\/[^\/][\w\.\,\?\'\\\/\+&%\$#\=~_\-@:]*)*[^\.\,\?\"\'\(\)\[\]!;<>{}\s\x7F-\xFF])?)$/i;
	if(sURL.length > 0 && sURL.match(regexURL)){
		return true;
	}else{
		return false;
	}
}

/*
..........................................................................
:: Plugin de jQuery para cambiar PNG's para IE6 dentro de un elemento   ::
:: Ej. uso: $('div.fulanito img').pngIE6()                              ::
:: El parametro 'blank' debe ser la ruta de un GIF transparente de 1x1  ::
:: --------------------------------------                               ::
:: Despues, plugin para agitar cosas molonamente con una sola orden     ::
:: --------------------------------------                               ::
:: Ademas, plugin para hacer fadeOut y despues destruir un elemento     ::
..........................................................................
*/
//jQuery.fn.extend({
////    pngIE6: function(blank) {
////        if (!($.browser.msie && $.browser.version < 7)) return this;
////        if (!blank) blank = 'img/blank.gif';
////        return this.each( function() {
////            this.style.filter = 'progid:DXImageTransform.Microsoft.AlphaImageLoader(src='+ this.src +',sizingMethod=image)';
////            this.src = blank;
////        });
////    },
//    shakeIt: function(o) {
//    	return this.each( function(){
//		    /* Almacenaremos en un atributo del objeto del DOM a mover los valores originales que poseia 
//		     * la primera que se llama a la funcion para evitar que al llamarla dos veces seguidas se
//		     * produzcan animaciones desde una posicion incorrecta del margen.
//		     * El objeto 'o' que se puede pasar como parametro esta destinado a la configuracion de la
//		     * animacion. Por ejemplo $('#idCualquiera').shakeit({velocidad: 200, amplitud: 40, veces: 5})
//		     */
//    		var $this = $(this),
//    		    o     = o || {},
//    		    mL    = this.mLCache || parseInt($this.css('marginLeft')),
//    		    mR    = this.mRCache || parseInt($this.css('marginRight')),
//    		    vel   = parseInt(o.velocidad) || 120,
//    		    ampl  = parseInt(o.amplitud) || 15,
//    		    veces = parseInt(o.veces) || 2;
//    		this.wCache = $this.css('width');
//    		$this.css('width', $this.width()+'px');
//    		this.mLCache = mL;
//    		this.mRCache = mR;
//    		for (var i = 0; i < veces; ++i) {
//    			$this.animate({
//    				marginLeft: (mL + ampl) + 'px',
//    				marginRight: (mR - ampl) + 'px'
//				}, vel).animate({
//					marginLeft: (mL - ampl) + 'px',
//					marginRight: (mR + ampl) + 'px'
//				}, vel);
//    		}
//    		// volvemos al estado primigenio
//    		$this.animate({
//    			marginLeft: mL + 'px',
//    			marginRight: mR + 'px'
//    		}, vel, function() {
//    			$this.css('width', this.wCache);
//    		});
//    	});
//    },
//    fundidoANada: function(o) {
//    	var o = o || {};
//    	o.velocidad = o.velocidad || 600;
//    	return this.each( function() {
//    		$(this).fadeOut(o.velocidad, function() {
//    			$(this).remove();
//    		});
//    	});
//    }
//});

/*
..........................................................................
:: Arreglo de PNG's pasado a filter:progid:... para elementos puntuales ::
..........................................................................
*/
//$( function() {
//    if (!($.browser.msie && $.browser.version < 7)) return; // SOLO PARA IE6!!!
//    var transparenciasIE = {
//        '#wrap':'scale',
//        'div.mascaraBoton':'crop',
//        '#footer':'crop',
//        '#nav a':'crop',
//        '#tarjetaLeft':'crop',
//        '#tarjetaRight':'crop',
//        'span.fotoPerfil a,span.fotoPerfilBlog a,span.fotoPerfilGrande a':'image',
//        'span.fotoPerfil a,span.fotoPerfilBlog a,span.fotoPerfilGrande a':'image',
//        'span.miniSnapshot a':'image',
//        'div.EstadoDafo0':'scale',
//        'div.EstadoDafo1':'scale',
//        'div.EstadoDafo2':'scale',
//        'div.EstadoDafo3':'scale'
//    };    
//    for (elem in transparenciasIE) {
//        $(elem).each( function() {
//        	var ruta,
//        		$this =  $(this);
//        	ruta  = $this.css('backgroundImage').replace('url(', '').replace(')', '');
//        	$this.get(0).style.filter = 'progid:DXImageTransform.Microsoft.AlphaImageLoader(src='+ ruta +',sizingMethod='+ transparenciasIE[elem] +')';
//        	$this.css('backgroundImage', 'none');
//        });
//    }
//});

/*
 * Desplegables genericos a peticion                                                                            
 * Ej. uso: crearDesplegables('selector jQuery boton', 'selector jQuery desplegables', {opciones: 'apetecaun'}) 
 * Opciones disponibles {
 *     efecto: string con cualquiera de los efectos de jQuery UI disponibles (por defecto: 'slide')
 *     opciones: objeto con las opciones del efecto de jQuery UI
 *               (por defecto: {direction: 'up'} si no se define el efecto
 *     			 las propias del jQuery UI si se define otro efecto)
 *     velocidad: duracion del efecto (por defecto: 600)
 * }
 */
function crearDesplegables(desplegar, desplegable, o) {
    var desplegar   = $(desplegar),
        desplegable = $(desplegable),
        incompatibles= false;
        o = o || {};
    if (desplegar.length != desplegable.length) {
        //alert('Error en la funcion crearDesplegables()\ndesplegar.length != desplegable.length');
        return false;
    };
    if (!o.efecto) {
        o.efecto = 'slide';
        o.opciones =  {direction: 'up'};
    }
    desplegar.each(function(indice) {
    	if (desplegable.eq(indice).find('form.busquedaAv').length) {
    		this.incompatibilidad = 'noBusquedaAv';
    	}
    	if (this.className.indexOf('activo') == -1) {
    		desplegable.eq(indice).hide();
    	} else {
			desplegable.eq(indice).show();
		}
        $(this).click( function() {
			$(this).toggleClass('activo');
            desplegable.eq(indice).toggle(o.efecto, o.opciones, o.velocidad, o.callback);
            if (this.incompatibilidad) {
                $('div.'+this.incompatibilidad).toggle(o.efecto, o.opciones, o.velocidad, o.callback);	
            }
            return false;
        });
    });
}

function crearPestanyas(pestanya, ficha, o) {
    var pestanya   = $(pestanya),
        ficha = $(ficha),
        o = o || {};
    // OJO!!!
    // DEFINIMOS VARIABLE GLOBAL:
    flagPestanyas = false;
    pestanya.eq(0).addClass('activo');
    ficha.not(':first').hide();
    if (pestanya.length != ficha.length) {
        //alert('Error en la funcion crearfichas()\npestanya.length != ficha.length');
        return false;
    };
    if (!o.efecto) {
        o.efecto = 'slide';
        o.opciones =  {direction: 'up'};
    }
    pestanya.each(function(indice) {
    	var $this = $(this);
        $this.click( function() {
        	if ($this.hasClass('activo') || flagPestanyas) return false;
        	flagPestanyas = true;
			ficha.filter(':visible').hide(o.efecto, o.opciones, o.velocidad, function() {
	        	pestanya.removeClass('activo');
				$this.addClass('activo');
				ficha.eq(indice).show(o.efecto, o.opciones, o.velocidad, function() {
					flagPestanyas = false;
				});
			});
            return false;
        });
    });
}

function crearError(textoError, contenedor) {
    crearError(textoError, contenedor, false);
}

function crearError(textoError, contenedor, moverScroll) {
    crearError(textoError, contenedor, moverScroll, false)
}

function crearError(textoError, contenedor, moverScroll, positionAbsolute) {
	// contenedor debe ser el elemento del DOM donde mostrar el error
	// o un String para llegar al elemento via jQuery
	var link = '';
	
	if(moverScroll){
	    link = '<a name="MiError" style="display:block;"></a>';
	}
	
	var $c = $(contenedor);
	if ($c.find('div.ko').length) { // si ya existe el div.ko ...
	    try
        {
	        $c.find('div.ko').html(link + textoError).shakeIt();
        }catch(err)
	    {	    
	    }


		if(positionAbsolute){
		    $c.find('div.ko')[0].style.position = 'absolute';
		}
	} else { //... si no lo creamos
		$('<div class="ko" style="display:none" >' + link + textoError + '</div>').prependTo($c).slideDown('fast');
		if(positionAbsolute){
		    $c.find('div.ko')[0].style.position = 'absolute';
		}
    }
	$c.find('div.ko').show();
	
	if(moverScroll){
	    document.location = '#MiError';
	}
}

function LimpiarHtmlControl(pControl)
{
    if (document.getElementById(pControl) != null)
    {
        //El siguiente código falla en Internet Explorer ya que no deja dar valor a innerHTML a algunos controles
        //document.getElementById(pControl).innerHTML = '';
        var nodosHijos=document.getElementById(pControl).childNodes
        
        for(i=0;i<nodosHijos.length;i++){
            document.getElementById(pControl).removeChild(nodosHijos[i]);
        }
    }
}

function mascaraCancelarAbsolute(texto, contenedor, funcionConfirmada) {
    var $confirmar = $(['<div><div class="pregunta"><span>', texto, '</span><button onclick="return false;" class="btMini">', borr.si, '</button><button onclick="return false;" class="btMini">', borr.no, '</button></div><div class="mascara"></div></div>'].join(''));

    $confirmar.css({
        'z-index': 200
    });

    $confirmar.find('div').css({
        height: $(contenedor).height() + 'px',
        paddingTop: ($(contenedor).height() / 2) + 'px',
        width: $(contenedor).width() + 'px',
        display: 'none',
        position: 'absolute'
    }).filter('.pregunta').fadeIn().end().filter('.mascara').show().fadeTo(600, 0.8);

    // Incrustamos el elemento a la vez que lo mostramos y definimos los eventos: 
    $confirmar.prependTo($(contenedor))
		.find('button').click(function () { // Ambos botones hacen desaparecer la mascara
		    $confirmar.fundidoANada();
		}).eq(0).click(funcionConfirmada); // pero solo el primero activa la funcionConfirmada
}

function mascaraCancelar(texto, contenedor, funcionConfirmada) {
	// contenedor debe ser el elemento del DOM que se pretende eliminar (o no)
	// o un String para llegar al elemento via jQuery
	
	/* Creamos el elemento del DOM y le asignamos estilos.
	 * Estructura creada:
	 * <div class="confirmar">
	 *     <div class="pregunta">
	 *         <span>Texto de confirmacion aqui</span>
     *         <button class="btMini">Si</button>
	 *         <button class="btMini">No</button>
	 *     </div>
	 *     <div class="mascara"></div>
	 * </div>
	 */
	 //TODO : LOZA --> Crear control para popups propios con mas contenido y personalizables.
    var $confirmar = $(['<div><div class="pregunta"><span>', texto, '</span><button onclick="return false;" class="btMini">', borr.si, '</button><button onclick="return false;" class="btMini">', borr.no, '</button></div><div class="mascara"></div></div>'].join(''));

    $confirmar.css({
        'z-index': 200
    });

    $confirmar.find('div').css({
	    height: $(contenedor).height() + 'px',
	    paddingTop: ($(contenedor).height() / 2) + 'px',
	    width: $(contenedor).width() + 'px',
	    display: 'none',
        position: 'fixed'
	}).filter('.pregunta').fadeIn().end().filter('.mascara').show().fadeTo(600, 0.8);

	// Incrustamos el elemento a la vez que lo mostramos y definimos los eventos: 
    $confirmar.prependTo($(contenedor))
		.find('button').click( function() { // Ambos botones hacen desaparecer la mascara
			$confirmar.fundidoANada();
		}).eq(0).click(funcionConfirmada); // pero solo el primero activa la funcionConfirmada
}

function mascaraCancelarSiNo(texto, contenedor, funcionConfirmada, funcionNo) {
	// contenedor debe ser el elemento del DOM que se pretende eliminar (o no)
	// o un String para llegar al elemento via jQuery
	var $c = $(contenedor);
	var $superC = $(contenedor).parents('[class^=wrap]').eq(0);
	
	var $confirmar = $(['<div class="confirmar"><div class="pregunta"><span>',texto,'</span><button onclick="return false;" class="btMini">',borr.si,'</button><button onclick="return false;" class="btMini">',borr.no,'</button></div><div class="mascara"></div></div>'].join(''));
	$confirmar.find('div').css({
		top       :$c.offset().top  + 'px',
		left      :$superC.offset().left + 'px',
		height    :($c.height()/2)+15+'px',
		paddingTop:($c.height()/2)-15+'px',
		width     :$superC.width() +'px',
		display: 'none'
	}).filter('.pregunta').fadeIn().end().filter('.mascara').show().fadeTo(600, 0.8);
	// Incrustamos el elemento a la vez que lo mostramos y definimos los eventos: 
	$confirmar.prependTo($superC)
		.find('button').click( function() { // Ambos botones hacen desaparecer la mascara
			$confirmar.fundidoANada();
		}).eq(0).click(funcionConfirmada);// pero solo el primero activa la funcionConfirmada
		
	$confirmar.find('button').eq(1).click(funcionNo);
}

function mascaraCancelar2(texto, contenedor, funcionConfirmada,textoInferior) {
	// contenedor debe ser el elemento del DOM que se pretende eliminar (o no)
	// o un String para llegar al elemento via jQuery
	var $c = $(contenedor);
	var $superC = $(contenedor).parents('[class^=wrap]').eq(0);
	
	var $anterior = $superC.children('[class^=confirmar]').eq(0);
	
	if($anterior.length > 0)
	{
	    //Eliminar el anterior
	    $superC.remove($anterior);
	}
	/* Creamos el elemento del DOM y le asignamos estilos.
	 * Estructura creada:
	 * <div class="confirmar">
	 *     <div class="pregunta">
	 *         <span>Texto de confirmacion aqui</span>
     *         <button class="btMini">Si</button>
	 *         <button class="btMini">No</button>
	 *     </div>
	 *     <div class="mascara"></div>
	 * </div>
	 */
	var $confirmar = $(['<div class="confirmar"><div class="pregunta"><span>',texto,'</span><br><br><button onclick="return false;" class="btMini">',borr.si,'</button><button onclick="return false;" class="btMini">',borr.no,'</button><p class="small"><br>',textoInferior,'</p></div><div class="mascara"></div></div>'].join(''));
	$confirmar.find('div').css({
		top       :$c.offset().top  + 'px',
		left      :$superC.offset().left + 'px',
		height    :$c.height()+'px',
		paddingTop:'20px',
		width     :$superC.width() +'px',
		display: 'none'
	}).filter('.pregunta').fadeIn().end().filter('.mascara').show().fadeTo(600, 0.8);
	// Incrustamos el elemento a la vez que lo mostramos y definimos los eventos: 
	$confirmar.prependTo($superC)
		.find('button').click( function() { // Ambos botones hacen desaparecer la mascara
			$confirmar.fundidoANada();
		}).eq(0).click(funcionConfirmada); // pero solo el primero activa la funcionConfirmada
}

function mascaraCancelarAlturaFija2Textos(texto, contenedor,altura, funcionConfirmada,textoInferior) {
	// contenedor debe ser el elemento del DOM que se pretende eliminar (o no)
	// o un String para llegar al elemento via jQuery
	var $c = $(contenedor);
	var $superC = $(contenedor).parents('[class^=wrap]').eq(0);
	
	var $anterior = $superC.children('[class^=confirmar]').eq(0);
	
	if($anterior.length > 0)
	{
	    //Eliminar el anterior
	    $superC.remove($anterior);
	}
	/* Creamos el elemento del DOM y le asignamos estilos.
	 * Estructura creada:
	 * <div class="confirmar">
	 *     <div class="pregunta">
	 *         <span>Texto de confirmacion aqui</span>
     *         <button class="btMini">Si</button>
	 *         <button class="btMini">No</button>
	 *     </div>
	 *     <div class="mascara"></div>
	 * </div>
	 */
	var $confirmar = $(['<div class="confirmar"><div class="pregunta"><span>',texto,'</span><br><br><button onclick="return false;" class="btMini">',borr.si,'</button><button onclick="return false;" class="btMini">',borr.no,'</button><p class="small"><br>',textoInferior,'</p></div><div class="mascara"></div></div>'].join(''));
	$confirmar.find('div').css({
		top       :$c.offset().top  + 'px',
		left      :$superC.offset().left + 'px',
		height    :altura+'px',
		paddingTop:'20px',
		width     :$superC.width() +'px',
		display: 'none'
	}).filter('.pregunta').fadeIn().end().filter('.mascara').show().fadeTo(600, 0.8);
	// Incrustamos el elemento a la vez que lo mostramos y definimos los eventos: 
	$confirmar.prependTo($superC)
		.find('button').click( function() { // Ambos botones hacen desaparecer la mascara
			$confirmar.fundidoANada();
		}).eq(0).click(funcionConfirmada); // pero solo el primero activa la funcionConfirmada
}

function mascaraAlerta(texto, contenedor) {
	var $c = $(contenedor);
	var $superC = $(contenedor).parents('[class^=wrap]').eq(0);
	var $confirmar = $(['<div class="confirmar"><div class="pregunta"><span>',texto,'</span><button onclick="return false;" class="btMini">Aceptar</button></div><div class="mascara"></div></div>'].join(''));
	$confirmar.find('div').css({
		top       :$c.offset().top  + 'px',
		left      :$superC.offset().left + 'px',
		height    :($c.height()/2)+15+'px',
		paddingTop:($c.height()/2)-15+'px',
		width     :$superC.width() +'px',
		display: 'none'
	}).filter('.pregunta').fadeIn().end().filter('.mascara').show().fadeTo(600, 0.8);
}


function miniConfirmar(texto, contenedor, funcionConfirmada) {
	// contenedor debe ser el elemento del DOM que se pretende eliminar (o no)
	// o un String para llegar al elemento via jQuery
	var $c = $(contenedor);
	/* Creamos el elemento del DOM y le asignamos estilos.
	 * Estructura creada:
	 * <div class="miniConfirmar">
	 *     <span>Texto de confirmacion aqui</span>
     *     <button class="btMini">Si</button>
	 *     <button class="btMini">No</button>
	 * </div>
	 */
	var $miniC = $(['<div class="miniConfirmar"><span>',texto,'</span><button class="btMini">',borr.si,'</button><button class="btMini">',borr.no,'</button></div>'].join(''));
	$miniC.css({display: 'none'}).fadeIn();
	// Incrustamos el elemento a la vez que lo mostramos y definimos los eventos: 
	$miniC.prependTo($c)
		.find('button').click( function() { // Ambos botones hacen desaparecer la mascara
			$miniC.fundidoANada();
		}).eq(0).click(funcionConfirmada); // pero solo el primero activa la funcionConfirmada
}

// menu de edicion desplegable para los listados
//$(function() {
//	$('div.editarElemento').find('li:last-child').addClass('ultimo').end()
//	.children('a,img').click(function(evento) {
//		var editar_eliminar = function(instaKill) {
//			$('div.editar-desplegado').removeClass('editar-desplegado');
//			if(instaKill){$clon.remove()}else{
//				$clon.fadeOut('fast',function(){$(this).remove()});
//			}
//		};
//		var $padre = $(evento.target).parent();
//		var $clon = $('#editar-clon');
//		if ($padre.hasClass('editar-desplegado') && $clon.length) {
//			clearTimeout($clon.get(0).temporizador);
//		} else {
//			//nos cargamos el que habia
//			editar_eliminar(true);
//			// y creamos el otro
//			$padre.addClass('editar-desplegado');
//			$clon = $padre.find('ul:first').clone(true).attr('id', 'editar-clon')
//			.appendTo('body').css({
//				opacity: '0',
//				display: 'block'
//			});
//			// ahora que el clon nuevo ya existe y tiene dimensiones podemos ajustar que aparezca donde interesa
//			$clon.css({
//				top: $padre.offset().top + $padre.height() + 'px',
//				left: $padre.offset().left + $padre.width() - $clon.width(),
//				opacity: '1',
//				display: 'none'
//			}).fadeIn('fast').add($padre).hover(function() {
//				clearTimeout($clon.get(0).temporizador);
//			}, function() {
//				$clon.get(0).temporizador = setTimeout(editar_eliminar, 1000);
//			});
//			
//		}
//		return false;
//		
//	});
//});

//LOZA : Funcion para desplegar el menu de acciones, aadirsela al onclick (onclick="javascript:mostrarMenu(event);") del enlace, y meter la imagen que lleve al lado dentro del propio enlace
function mostrarMenu(evento) {
        if (!evento) var evento = window.event;
		var editar_eliminar = function(instaKill) {
			$('div.editar-desplegado').removeClass('editar-desplegado');
			if(instaKill){$clon.remove()}else{
				$clon.fadeOut('fast',function(){$(this).remove()});
			}
		};
		if(!evento.target){
		var hijo = evento.srcElement;
		}
		else{
		var hijo = evento.target;
		}
		if(hijo.nodeName == 'IMG'){
		    hijo = $(hijo).parent();
		}
		var $padre = $(hijo).parent();
		
		
		var $clon = $('#editar-clon');
		if ($padre.hasClass('editar-desplegado') && $clon.length) {
			clearTimeout($clon.get(0).temporizador);
		} else {
			//nos cargamos el que habia
			editar_eliminar(true);
			// y creamos el otro
			$padre.addClass('editar-desplegado');
			$clon = $padre.find('ul:first').clone(true).attr('id', 'editar-clon')
			.appendTo('body').css({
				opacity: '0',
				display: 'block'
			});
			// ahora que el clon nuevo ya existe y tiene dimensiones podemos ajustar que aparezca donde interesa
			$clon.css({
				top: $padre.offset().top + $padre.height() + 'px',
				left: $padre.offset().left + $padre.width() - $clon.width(),
				opacity: '1',
				display: 'none'
			}).fadeIn('fast').add($padre).hover(function() {
				clearTimeout($clon.get(0).temporizador);
			}, function() {
				$clon.get(0).temporizador = setTimeout(editar_eliminar, 1000);
			});
			
		}
		return false;
		
	}

function realizarFuncion(funcion, contexto){
eval(funcion);
}


//LOZA : Funcion para cambiar entre dos pestaas con efecto slide verical
//DesplegarPestanyas(
//                      id del boton(o elemento de cabecera tipo LI) que hacemos click, 
//                      id del panel al que se asocia el elemento anterior,
//                      id del boton(o elemento de cabecera tipo LI) que se encontraba activo,
//                      id del panel al que se asocia el elemento anterior)
function DesplegarPestanyas(pBoton, pPanel,pBoton2,pPanel2) {
    var boton   = $(document.getElementById(pBoton));
    var boton2   =$(document.getElementById(pBoton2));
    if(boton2[0].className == 'activo'){
        panel = $(document.getElementById(pPanel));
        panel2 = $(document.getElementById(pPanel2));
        o = {efecto:'blind', opciones:{direction:'vertical'}};
        if (!o.efecto) {
            o.efecto = 'blind';
            o.opciones =  {direction: 'vertical'};
        }
        panel2.toggle(o.efecto, o.opciones, o.velocidad, function() {
		    boton.toggleClass('activo');
	        boton2.toggleClass('activo');
		    panel.toggle(o.efecto, o.opciones, o.velocidad, null);
	    });
        return false;
    }
}


function EjecutarScriptsIniciales(){

	//var id = setInterval("EjecutarScriptsIniciales2()",100);
	//setTimeout("clearInterval("+id+")",1000);
	setTimeout("EjecutarScriptsIniciales2()",1000);
}
//LOZA, funcion para ejecutar los scripts iniciales en cada callback, para aadir transparencias y demas
function EjecutarScriptsIniciales2(){
    jQuery.fn.extend({
//    pngIE6: function(blank) {
//        if ($.browser.msie && $.browser.version < 7)
//        {
//        if (!blank) blank = 'img/blank.gif';
//        return this.each( function() {
//            this.style.filter = 'progid:DXImageTransform.Microsoft.AlphaImageLoader(src='+ this.src +',sizingMethod=image)';
//            this.src = blank;
//            });
//        }

//    },
    shakeIt: function(o) {
    	return this.each( function(){
		    /* Almacenaremos en un atributo del objeto del DOM a mover los valores originales que poseia 
		     * la primera que se llama a la funcion para evitar que al llamarla dos veces seguidas se
		     * produzcan animaciones desde una posicion incorrecta del margen.
		     * El objeto 'o' que se puede pasar como parametro esta destinado a la configuracion de la
		     * animacion. Por ejemplo $('#idCualquiera').shakeit({velocidad: 200, amplitud: 40, veces: 5})
		     */
    		var $this = $(this),
    		    o     = o || {},
    		    mL    = this.mLCache || parseInt($this.css('marginLeft')),
    		    mR    = this.mRCache || parseInt($this.css('marginRight')),
    		    vel   = parseInt(o.velocidad) || 120,
    		    ampl  = parseInt(o.amplitud) || 15,
    		    veces = parseInt(o.veces) || 2;
    		this.wCache = $this.css('width');
    		$this.css('width', $this.width()+'px');
    		this.mLCache = mL;
    		this.mRCache = mR;
    		for (var i = 0; i < veces; ++i) {
    			$this.animate({
    				marginLeft: (mL + ampl) + 'px',
    				marginRight: (mR - ampl) + 'px'
				}, vel).animate({
					marginLeft: (mL - ampl) + 'px',
					marginRight: (mR + ampl) + 'px'
				}, vel);
    		}
    		// volvemos al estado primigenio
    		$this.animate({
    			marginLeft: mL + 'px',
    			marginRight: mR + 'px'
    		}, vel, function() {
    			$this.css('width', this.wCache);
    		});
    	});
    },
    fundidoANada: function(o) {
    	var o = o || {};
    	o.velocidad = o.velocidad || 600;
    	return this.each( function() {
    		$(this).fadeOut(o.velocidad, function() {
    			$(this).remove();
    		});
    	});
    }
});


//$( function() {
//    if (!($.browser.msie && $.browser.version < 7)) return; // SOLO PARA IE6!!!
//    var transparenciasIE = {
//        '#wrap':'scale',
//        'div.mascaraBoton':'crop',
//        '#footer':'crop',
//        '#nav a':'crop',
//        '#tarjetaLeft':'crop',
//        '#tarjetaRight':'crop',
//        'span.fotoPerfil a,span.fotoPerfilBlog a,span.fotoPerfilGrande a':'image',
//        'span.miniSnapshot a':'image',
//        'div.EstadoDafo0':'scale',
//        'div.EstadoDafo1':'scale',
//        'div.EstadoDafo2':'scale',
//        'div.EstadoDafo3':'scale',
//        'img':'crop'
//    };    
//    for (elem in transparenciasIE) {
//        $(elem).each( function() {
//        	var ruta,
//        		$this =  $(this);
//        	if($this.css('filter')==''){
//        	    ruta  = $this.css('backgroundImage').replace('url(', '').replace(')', '');
//        	    $this.get(0).style.filter = 'progid:DXImageTransform.Microsoft.AlphaImageLoader(src='+ ruta +',sizingMethod='+ transparenciasIE[elem] +')';
//        	    $this.css('backgroundImage', 'none');
//        	}
//        });
//    }
//    
//});

//Muestra los paneles con la clase panelOcultar
$( function() {
	var divsPanelOcultar =$('div.panelOcultar');
	var i;
	for(i=0;i<divsPanelOcultar.length;i++)
	{
        divsPanelOcultar[i].style.display = "block";
	}
});

//Muestra los '+' en los desplegables de las descripciones que tengan la clase TextoTiny
$( function() {
	var divsTextoTiny =$('div.TextoTiny');
	var i;
	for(i=0;i<divsTextoTiny.length;i++)
	{
	    if(divsTextoTiny[i].id != "")
	    {
		    if(divsTextoTiny[i].offsetHeight < divsTextoTiny[i].scrollHeight)
		    {
		        if($(document.getElementById(divsTextoTiny[i].id)).find('object').length > 0)
		        {
		            var objeto = $(document.getElementById(divsTextoTiny[i].id)).find('object')[0];
		            if(objeto.width > divsTextoTiny[i].offsetWidth)
		            {
		                objeto.height = objeto.height / (objeto.width / divsTextoTiny[i].offsetWidth);
		                objeto.width = divsTextoTiny[i].offsetWidth;
		            }
		        }
    		    
		        //$(document.getElementById(divsTextoTiny[i].id)).find('object').hide();
		        var objects = $(document.getElementById(divsTextoTiny[i].id)).find('object');
		        for (var count= 0;count<objects.length;count++)
		        {
		            var object = objects[count];
		            if (object != null)
		            {
		                if (object.innerHTML.indexOf('<param name="wmode" value="transparent">') == -1)
		                {
		                    object.innerHTML = '<param name="wmode" value="transparent">' + object.innerHTML;
		                }
    		            
    		            
                        if (object.innerHTML.indexOf('<embed') != -1 && object.innerHTML.indexOf('<embed wmode="transparent"') == -1)
	                    {
	                        object.innerHTML = object.innerHTML.replace('<embed','<embed wmode="transparent"');
	                    }
	                }
		        }
			    if(document.getElementById(divsTextoTiny[i].id + '_DesplegarTexto') != null)
			    {
				    document.getElementById(divsTextoTiny[i].id + '_DesplegarTexto').style['display'] = '';
			    }
		    }
		    else
		    {
			    divsTextoTiny[i].style['height'] = '';
		    }
		}
	}
});

//Oculta los difuminados en las descripciones que tengan la clase TextoDifuminado
$( function() {
	var divsTextoDifuminadoTiny =$('div.TextoDifuminado');
	var i;
	
	for(i=0;i<divsTextoDifuminadoTiny.length;i++)
	{	    
	    if(divsTextoDifuminadoTiny[i].offsetHeight >= divsTextoDifuminadoTiny[i].scrollHeight)
	    {
	         if(document.getElementById(divsTextoDifuminadoTiny[i].id + '_DifuminarTexto') != null)
		    {		
			    document.getElementById(divsTextoDifuminadoTiny[i].id + '_DifuminarTexto').style['display'] = 'none';
		    }
	    }		
	}
});

//Muestra los '+' en los desplegables de las etiquetas y categorias desplegarEtiquetas
$( function() {
	var divsDesplegarEtiquetas =$('div.desplegarEtiquetas');
	var i;
	for(i=0;i<divsDesplegarEtiquetas.length;i++)
	{
	    if(divsDesplegarEtiquetas[i].id != "")
	    {
			var imagenMas = $(document.getElementById(divsDesplegarEtiquetas[i].id)).find('img.mas')[0];
			var imagenMenos = $(document.getElementById(divsDesplegarEtiquetas[i].id)).find('img.menos')[0];
			
			imagenMas.style.display = "none";
			imagenMenos.style.display = "none";
			
		    if((divsDesplegarEtiquetas[i].offsetHeight * 2 + 2) < divsDesplegarEtiquetas[i].scrollHeight)
		    {
				imagenMas.style.display = "";
				imagenMenos.style.display = "";
			
                var alturaMaxima = divsDesplegarEtiquetas[i].getBoundingClientRect().top + divsDesplegarEtiquetas[i].offsetHeight -14; //offsetTop maximo del ultimo elemento
                var enlaces = $(divsDesplegarEtiquetas[i]).find('a');
                
			    var ultimoEnlaceFila = 0,
			        tieneImagenes = false, 
			        comprobar = true, 
			        j=0;
			     
				while(!tieneImagenes && j < enlaces.length)
		        {
		            if(comprobar)
		            {
						ultimoEnlaceFila = j;
						if(enlaces[j].getBoundingClientRect().top > alturaMaxima + 2 || enlaces[j].innerText == "")
						{
						    comprobar = false;
		                    //break;
		                }
		            }
		            if($(enlaces[j].parentNode).find('img').length > 0  && enlaces[j].parentNode.className.indexOf("desplegarEtiquetas") == -1)
		            {
		                tieneImagenes = true;
		            }
					j++
		        }
		        j = ultimoEnlaceFila - 1;

		        imagenMas.style.position = "relative";
		        imagenMas.style.top = "0px";
		        imagenMas.style.left = "0px";
		        imagenMas.style['z-index'] = "100";
		           
			    if ($(divsDesplegarEtiquetas[i]).find('span.tag').length > 0 || tieneImagenes || j<0)
			    {
			        imagenMas.style.display = "none";
			        divsDesplegarEtiquetas[i].style['height'] = '';
			    }
			    else
			    {
		            imagenMas.style.top = enlaces[j].getBoundingClientRect().top - imagenMas.getBoundingClientRect().top + 3 + 'px';
		            imagenMas.style.left = enlaces[j].getBoundingClientRect().right - imagenMas.getBoundingClientRect().right + imagenMas.offsetWidth + 3 + 'px';
			    }
		    }
		    else
		    {
			    divsDesplegarEtiquetas[i].style['height'] = '';
		    }
		}
	}
});

	//Oculta los paneles con la clase panelOcultar [Miguel]
    $( 
        function()
        {
	        var divsPanelOcultar = $('div.panelOcultar');
	        try
            {
                if (document.getElementById('ctl00_CPH1_desplegadosHack') != null)
                {
	                var desplegadosDiv = document.getElementById('ctl00_CPH1_desplegadosHack').value;
	                var i;
	                for(i=0;i<divsPanelOcultar.length;i++)
	                {
	                    //Miramos si no está desplegado para ocultarlo
	                    if (desplegadosDiv.match(divsPanelOcultar[i].id) == null)
	                    {
                            divsPanelOcultar[i].style.display = "none";
                        }
                        else
                        {
                            //Ponemos la ficha de desplegado activo [por si ha hecho F5 se repinte bien]
                            document.getElementById('Titulo' + divsPanelOcultar[i].id).className = "desplegable activo";
                        }
	                }
	            }
	        }
	        catch(err)
	        {
	            var i;
	            for(i=0;i<divsPanelOcultar.length;i++)
	            {
                    divsPanelOcultar[i].style.display = "none";
	            }
	        }
        }
     );

	//Pone el estilo a los CKEDITOR que lo hayan perdido
    $( 
        function()
        {
			try
			{
                var textAreas = $('.comentarios textarea.cke'),
                    instanceName = "",
                    config = "",
                    i = 0;

                for(i=0;i<textAreas.length;i++)
                {
                    instanceName = textAreas[i].id;					    
                    
					if(document.getElementById('cke_' + instanceName) == null)
					{
                        config = {toolbar : textAreas[i].className.split(' ')[1]};

						if(CKEDITOR.instances[instanceName] != null)
						{
						    var instance = CKEDITOR.instances[instanceName];
						    instance.destroy();
						    CKEDITOR.remove(instance);
						}
						CKEDITOR.replace(instanceName, config);
						CKEDITOR.document.getById( 'cke_contents_' + instanceName ).setStyle( 'height', '120px' );
						
						if(CKEDITOR.instances[instanceName] != null)
						{
						    var editor = CKEDITOR.instances[instanceName];
                            editor.on('paste', function(evt) {
                                evt.data.html=evt.data.html.replace(/\\u0000/g,'');
                                evt.data.html=evt.data.html.replace(/\\u00AD/g,'');
                                evt.data.html=evt.data.html.replace(/\\u0600/g,'');
                                evt.data.html=evt.data.html.replace(/\\u0604/g,'');
                                evt.data.html=evt.data.html.replace(/\\u070F/g,'');
                                evt.data.html=evt.data.html.replace(/\\u17B4/g,'');
                                evt.data.html=evt.data.html.replace(/\\u17B5/g,'');
                                evt.data.html=evt.data.html.replace(/\\u200C/g,'');
                                evt.data.html=evt.data.html.replace(/\\u200F/g,'');
                                evt.data.html=evt.data.html.replace(/\\u2028/g,'');
                                evt.data.html=evt.data.html.replace(/\\u202F/g,'');
                                evt.data.html=evt.data.html.replace(/\\u2060/g,'');
                                evt.data.html=evt.data.html.replace(/\\u206F/g,'');
                                evt.data.html=evt.data.html.replace(/\\uFEFF/g,'');
                                evt.data.html=evt.data.html.replace(/\\uFFF0/g,'');
                                evt.data.html=evt.data.html.replace(/\\uFFFF/g,'');
                                
                                var texto = evt.data.html;
	                            var expReg = [{busq:/style="[^"]*"/, reemp:''}]
                        					    
	                            for(var i=0; i< expReg.length; i++)
	                            {
		                            var expRegEnlace = expReg[i] ;
		                            var oMatch = texto.match( expRegEnlace.busq ) ;
		                            while(oMatch)
		                            {
			                            texto = texto.replace(oMatch, expRegEnlace.reemp);
			                            oMatch = texto.match( expRegEnlace.busq ) ;
		                            }
	                            }

	                            evt.data.html = texto;
		
                            }, editor.element.$);
                        }
					}
				}
			}
			catch(error)
			{
			}
        }
     );

	//Pone el estilo a los CKEDITOR que lo hayan perdido
    $( 
        function()
        {
			try
			{
			    if (RecargarCKEditorInicio) {
			        RecargarTodosCKEditor();
			    }
			}
			catch(error)
			{
			}
        }
     );

}

function DesplegarDescripcionMasNueva(imagen, panelId, alturaMin)
{
	var panel = document.getElementById(panelId);
	if(panel.offsetHeight < panel.scrollHeight)
	{
		EstirarPanel(panel);
	    $(imagen).find('img')[0].src = $(imagen).find('img')[0].src.replace('verMas.gif', 'verMenos.gif');
	    $(document.getElementById(panelId + '_DesplegarTexto')).find('span')[0].innerHTML = "";
		//$(document.getElementById(panelId)).find('object').show();
		
	}
	else
	{
		EncogerPanel(panel, alturaMin);
	    $(imagen).find('img')[0].src = $(imagen).find('img')[0].src.replace('verMenos.gif', 'verMas.gif');
	    $(document.getElementById(panelId + '_DesplegarTexto')).find('span')[0].innerHTML = "...";
		//$(document.getElementById(panelId)).find('object').hide();
	}
}

function DesplegarDescripcionConLeerMas(imagen, panelId, alturaMin)
{
	var panel = document.getElementById(panelId);
	if(panel.offsetHeight < panel.scrollHeight)
	{
		EstirarPanel(panel);
		
		var spans = $(imagen).find('span');
		if (spans.length > 0)
		{
		    spans[0].style.display = 'none';
		    $(imagen).find('img')[0].style.display = '';
		}
	}
	else
	{
		EncogerPanel(panel, alturaMin);
		
		var spans = $(imagen).find('span');
		if (spans.length > 0)
		{
		    spans[0].style.display = '';
		    $(imagen).find('img')[0].style.display = 'none';
		}
	}
}

function DesplegarEtiquetaMas(panelId, alturaMin)
{
	var imagen = $(document.getElementById(panelId)).find('img.mas')[0];
	var panel = document.getElementById(panelId);
	if(panel.offsetHeight < panel.scrollHeight)
	{
		EstirarPanel(panel);
		imagen.style.display = "none";
	}
	else
	{
		EncogerPanel(panel, alturaMin);
		imagen.style.display = "";
	}
}

function EstirarPanel(panel)
{
	if(panel.offsetHeight < panel.scrollHeight){
		panel.style.height = panel.offsetHeight + panel.scrollHeight / 20 + "px";
		//panel.style.height = panel.offsetHeight + 12 + "px";
		if(panel.offsetHeight > panel.scrollHeight){
			panel.style.height = panel.scrollHeight;
		}
		setTimeout("EstirarPanel(document.getElementById('" + panel.id + "'))",1);
	}
	else
	{
		panel.style.height = panel.scrollHeight + "px";
	}
}

function EncogerPanel(panel,alturaMin)
{
	if(panel.offsetHeight > alturaMin){
		panel.style.height = panel.offsetHeight - panel.scrollHeight / 20 + "px";
		//panel.style.height = panel.offsetHeight - 12 + "px";
		if(panel.offsetHeight <alturaMin){
			panel.style.height = alturaMin;
		}
		setTimeout("EncogerPanel(document.getElementById('" + panel.id + "')," + alturaMin + ")",1);
	}
	else
	{
		panel.style.height = alturaMin + "px";
	}
}

function isDate(texbox) {
    var fecha = texbox.value;
    var datePat = /^(\d{1,2})(\/|-)(\d{1,2})(\/|-)(\d{4})$/;
    var matchArray = fecha.match(datePat); // is the format ok?

    if (matchArray == null) {
        //alert("Please enter date as either mm/dd/yyyy or mm-dd-yyyy.");
        return false;
    }
    
    day = matchArray[1];// pasamos la fecha a variables
    month = matchArray[3];
    year = matchArray[5];

    if (month < 1 || month > 12) { // comprobamos el mes
        return false;
    }

    if (day < 1 || day > 31) {
        return false;
    }

    if ((month==4 || month==6 || month==9 || month==11) && day==31) {
        return false;
    }

    if (month == 2) { // comprobamos el 29 de febrero
        var isleap = (year % 4 == 0 && (year % 100 != 0 || year % 400 == 0));
        if (day > 29 || (day==29 && !isleap)) {
            return false;
        }
    }

    texbox.value = day + "/" + month + "/" + year;

    return true; // fecha es valida
}


function ComprobarFechas(fecha1, fecha2, fechaCambiada) {
    if (fecha1.value != calendario.desde && fecha1.value != calendario.desde && fecha1.value != "") {
        if (isDate(fecha1)) {
            if (fecha2.value != calendario.desde && fecha2.value != calendario.desde && fecha2.value != "") {
                var datePat = /^(\d{1,2})(\/|-)(\d{1,2})(\/|-)(\d{4})$/;
                if (fechaCambiada == "1") {
                    var fecha1Array = fecha1.value.match(datePat)
                    var fecha2Array = fecha2.value.match(datePat)
                }
                else {
                    var fecha2Array = fecha1.value.match(datePat)
                    var fecha1Array = fecha2.value.match(datePat)
                }

                var day1, day2, month1, month2, year1, year2;


                if (fecha1Array != null) {
                    day1 = fecha1Array[1]; // pasamos la fecha a variables
                    month1 = fecha1Array[3];
                }
                if (fecha2Array != null) {
                    day2 = fecha2Array[1];
                    month2 = fecha2Array[3];
                }
                
                if (fecha1Array != null) {
                    year1 = fecha1Array[5];
                }
                if (fecha2Array != null) {
                    year2 = fecha2Array[5];
                }

                var resultado = false;
                if (year1 > year2) {
                    resultado = true;
                }
                else {
                    if (year1 == year2) {
                        if (month1 > month2) {
                            resultado = true;
                        }
                        else {
                            if (month1 == month2) {
                                if (day1 > day2) {
                                    resultado = true;
                                }
                            }
                        }
                    }
                }
                if (resultado) {
                    if (fechaCambiada == "1") {
                        fecha1.value = calendario.desde;
                    }
                    else {
                        fecha1.value = calendario.hasta;
                    }
                }
            }
        }
        else {
            if (fechaCambiada == "1") {
                fecha1.value = calendario.desde;
            }
            else {
                fecha1.value = calendario.hasta;
            }
        }
    }
}

/*                                                                              Grafico Trabajo
 *---------------------------------------------------------------------------------------------

 */
$(function() {
	var $context = $('#graficoTrabajo'),
		$dt = $('dt', $context),
		$li = $('li', $context),
		maxH = 0,
		totalW = $context.width();
	
	$dt.each(function(index) {
		var $t = $(this),
			proposedW = Math.floor(totalW * parseInt($t.find('big').text(), 10)/100),
			deltaW = $t.width()-proposedW;
		// preparamos los css
		$t.css('cursor', 'pointer');
		$t.width( proposedW + 'px' );
		maxH = Math.max(maxH, $t.height());
		// preparamos los eventos del DD
		$t.mouseover(function() {
			$t.next().show().css({
				left:$t.offset().left + 'px',
				top:$t.offset().top - 12 - $t.next().height() + 'px'
			})
		}).mouseout(function() {
			$t.next().hide();
		});
	}).height(maxH);
});
/*                                                                                Tooltips (Tt)
 *---------------------------------------------------------------------------------------------
 */
$(function() {
	var posicionarTt = function(event) {
		var tPosX = event.pageX - 10;
		var tPosY = event.pageY - 17 - ($("div.tooltip").height() || 0);
		if(tPosY < window.scrollY + 15)
		{
		    tPosY = tPosY + 60;
		}
		$("div.tooltip").css({
			top: tPosY,
			left: tPosX
		});
	}

	var mostrarTt = function(event){
	    $("div.tooltip").remove();
		var textoTt = (this.tooltipData) ? this.tooltipData : $(this).text();
		$("<div class='tooltip' style='display:none;'>" + textoTt + "</div>")
		   .appendTo("body")
		   .fadeIn();
	    posicionarTt(event);
	}

	var ocultarTt = function() {
		$("div.tooltip").remove();
	}

	$(".conTt").each(function() {
		if (this.title) {
			this.tooltipData=this.title;
			this.removeAttribute('title');
		}
	}).hover(mostrarTt, ocultarTt).mousemove(posicionarTt);
});

/*                                                             Tooltips para freebase (conFbTt)
 *---------------------------------------------------------------------------------------------
 */
var necesarioPosicionar = true;
var mouseOnTooltip = false;
var cerrar = 0;
var tooltipActivo = '';

$(function() {
	var posicionarFreebaseTt = function(event) {
	    if(necesarioPosicionar && $("div.tooltip").length > 0){
		    var tPosX = event.pageX - 10;
		    var tPosY = event.pageY - 17 - ($("div.tooltip").height() || 0);
    		
		    var navegador = navigator.appName;
		    var anchoVentana = window.innerWidth;
		    var altoScroll = window.pageYOffset;
    		
            if (navegador == "Microsoft Internet Explorer") {
                anchoVentana = window.document.body.clientWidth;
                altoScroll = document.documentElement.scrollTop;
            }
    		
		    var sumaX = tPosX + $("div.tooltip").width() + 30;
		    if(sumaX > anchoVentana){
		        tPosX = anchoVentana - $("div.tooltip").width() - 30;
		    }
    		
		    if(tPosY < altoScroll){
		        tPosY = event.pageY + 12
		    }
    		
		    $("div.tooltip").css({
			    top: tPosY,
			    left: tPosX
		    });
		    necesarioPosicionar = false;
		}
	}

	var mostrarFreebaseTt = function(event){
	    var hayTooltip = $("div.tooltip").length != 0;
	    var tooltipDiferente = false;
	    
	    if(hayTooltip && tooltipActivo != '' && $(this).hasClass('conFbTt')){
	        tooltipDiferente = ($(this).text() != tooltipActivo);
	    }
	    
	    if(!hayTooltip || tooltipDiferente){
	        $("div.tooltip").remove();
		    var textoTt = (this.tooltipData) ? this.tooltipData : $(this).text();
		    tooltipActivo = $(this).text();
		    $("<div class='tooltip' style='display:none; width:350px; height:auto;padding:0;' onmousemove='javascript:mouseSobreTooltip()' onmouseover='javascript:mouseSobreTooltip()' onmouseout='javascript:mouseFueraTooltip()'><div class='relatedInfoWindow'><p class='poweredby'>Powered by <a href='http://www.gnoss.com'><strong>Gnoss</strong></a></p><div class='wrapRelatedInfoWindow'>" + textoTt + "</div> <p><em>" + $('input.inpt_avisoLegal').val() + "</em></p></div></div>")
		       .appendTo("body")
		       .fadeIn();	       
		       
		       $("div.tooltip").hover(mostrarFreebaseTt, ocultarFreebaseTt).mousemove(posicionarFreebaseTt);
		    if(tooltipDiferente){
		        necesarioPosicionar = true;
		    }
	        posicionarFreebaseTt(event);
	    }
	    cerrar++;
	}

	var ocultarFreebaseTt = function() {
		setTimeout(quitarFreebaseTt, 1000);
	}

	$(".conFbTt").each(function() {
		if (this.title) {
			this.tooltipData=this.title;
			this.removeAttribute('title');
		}
	}).hover(mostrarFreebaseTt, ocultarFreebaseTt).mousemove(posicionarFreebaseTt);
});


function quitarFreebaseTt(){
    cerrar--;
    if((cerrar <= 0) && (!mouseOnTooltip)){
        $("div.tooltip").remove();
        necesarioPosicionar = true;
    }
}

function mouseFueraTooltip()
{
    mouseOnTooltip = false;
    if(cerrar <= 0){
        setTimeout(quitarFreebaseTt, 1000);
    }
}

function mouseSobreTooltip()
{
    mouseOnTooltip = true;
}

function GuardarDescrHack(pCKeID, pHackID)
{
	//document.getElementById(pHackID).value = ObtenerValorCKEditor(pCKeID);
	document.getElementById(pHackID).value = $('#' + pCKeID).val();
}

function ObtenerValorCKEditor(pCKeID)
{
    try
    {
        var editor = CKEDITOR.instances[pCKeID];

	    if(editor)
	    {
		    var texto = editor.document.$.body.innerHTML;
		    var expReg = [{busq:/target="[^"]*"/g, reemp:''} ,
		                  {busq:/id="[^"]*"/g, reemp:''} ,
		                  {busq:/href="javascript:void\(0\)\/\*\d*\*\/"/g, reemp:''} ,
		                  {busq:/_cke_saved_href=/g, reemp:'href='}]

		    for(var i=0; i< expReg.length; i++)
		    {
			    var expRegEnlace = expReg[i] ;
			    var oMatch = texto.match( expRegEnlace.busq ) ;
			    if(oMatch)
			    {
			        for(var j=0; j< oMatch.length; j++)
			        {
				        texto = texto.replace(oMatch[j], expRegEnlace.reemp);
			        }
			    }
		    }
		    texto = texto.replace(/<a /g, '<a target="_blank" ');

		    return texto;
	    }
	    else
	    {
		    return  document.getElementById(pCKeID).value;
	    }
	}
	catch(ex)
	{
	    return null;
	}
}


//Reemplaza los contadores '(n)' p.e. en la bandeja de mensajes
function reemplazarContadores(idPanel, numero)
{
    var texto = $(idPanel).html();
    var expRegEnlace ='\\([0-9]*\\)';
    var re = new RegExp(expRegEnlace);
    var oMatch = texto.match(re);
    if(oMatch)
    {
        if(numero > 0)
        {
            texto = texto.replace(oMatch, '(' + numero + ')');
        }
        else
        {
            texto = texto.replace(oMatch, '');
        }
    }
    else
    {
        if(numero > 0)
        {
            texto = texto + '(' + numero + ')';
        }
    }
    $(idPanel).html(texto);
}

function ObtenerHash(){
	var hash = window.location.hash;
	if(hash != null && hash != ''){
		var posicion = hash.indexOf(hash)
		if(posicion > -1){
			return hash;
			}
	}
	return '';
}

function urlEncodeCharacter(c)
{
	return '%' + c.charCodeAt(0).toString(16);
}

function urlDecodeCharacter(str, c)
{
	return String.fromCharCode(parseInt(c, 16));
}

function urlEncode(s)
{
    var is_firefox = navigator.userAgent.toLowerCase().indexOf('firefox/') > -1;  
    if (is_firefox)
    {
        return encodeURIComponent(s).replace( /[^0-9a-z]/g, urlEncodeCharacter );
    }
    else
    {
        return encodeURIComponent(s);
    }
}

function urlDecode(s)
{
    var is_firefox = navigator.userAgent.toLowerCase().indexOf('firefox/') > -1;  
    if (is_firefox)
    {
        return decodeURIComponent(s).replace( /\%([0-9a-f]{2})/g, urlDecodeCharacter);
    }
    else
    {
        return decodeURIComponent(s);
    }
}

Encoder = {

	// When encoding do we convert characters into html or numerical entities
    EncodeType: "entity",  // entity OR numerical
    arr1: ['&nbsp;', '&iexcl;', '&cent;', '&pound;', '&curren;', '&yen;', '&brvbar;', '&sect;', '&uml;', '&copy;', '&ordf;', '&laquo;', '&not;', '&shy;', '&reg;', '&macr;', '&deg;', '&plusmn;', '&sup2;', '&sup3;', '&acute;', '&micro;', '&para;', '&middot;', '&cedil;', '&sup1;', '&ordm;', '&raquo;', '&frac14;', '&frac12;', '&frac34;', '&iquest;', '&Agrave;', '&Aacute;', '&Acirc;', '&Atilde;', '&Auml;', '&Aring;', '&AElig;', '&Ccedil;', '&Egrave;', '&Eacute;', '&Ecirc;', '&Euml;', '&Igrave;', '&Iacute;', '&Icirc;', '&Iuml;', '&ETH;', '&Ntilde;', '&Ograve;', '&Oacute;', '&Ocirc;', '&Otilde;', '&Ouml;', '&times;', '&Oslash;', '&Ugrave;', '&Uacute;', '&Ucirc;', '&Uuml;', '&Yacute;', '&THORN;', '&szlig;', '&agrave;', '&aacute;', '&acirc;', '&atilde;', '&auml;', '&aring;', '&aelig;', '&ccedil;', '&egrave;', '&eacute;', '&ecirc;', '&euml;', '&igrave;', '&iacute;', '&icirc;', '&iuml;', '&eth;', '&ntilde;', '&ograve;', '&oacute;', '&ocirc;', '&otilde;', '&ouml;', '&divide;', '&oslash;', '&ugrave;', '&uacute;', '&ucirc;', '&uuml;', '&yacute;', '&thorn;', '&yuml;', '&quot;', '&amp;', '&lt;', '&gt;', '&OElig;', '&oelig;', '&Scaron;', '&scaron;', '&Yuml;', '&circ;', '&tilde;', '&ensp;', '&emsp;', '&thinsp;', '&zwnj;', '&zwj;', '&lrm;', '&rlm;', '&ndash;', '&mdash;', '&lsquo;', '&rsquo;', '&sbquo;', '&ldquo;', '&rdquo;', '&bdquo;', '&dagger;', '&Dagger;', '&permil;', '&lsaquo;', '&rsaquo;', '&euro;', '&fnof;', '&Alpha;', '&Beta;', '&Gamma;', '&Delta;', '&Epsilon;', '&Zeta;', '&Eta;', '&Theta;', '&Iota;', '&Kappa;', '&Lambda;', '&Mu;', '&Nu;', '&Xi;', '&Omicron;', '&Pi;', '&Rho;', '&Sigma;', '&Tau;', '&Upsilon;', '&Phi;', '&Chi;', '&Psi;', '&Omega;', '&alpha;', '&beta;', '&gamma;', '&delta;', '&epsilon;', '&zeta;', '&eta;', '&theta;', '&iota;', '&kappa;', '&lambda;', '&mu;', '&nu;', '&xi;', '&omicron;', '&pi;', '&rho;', '&sigmaf;', '&sigma;', '&tau;', '&upsilon;', '&phi;', '&chi;', '&psi;', '&omega;', '&thetasym;', '&upsih;', '&piv;', '&bull;', '&hellip;', '&prime;', '&Prime;', '&oline;', '&frasl;', '&weierp;', '&image;', '&real;', '&trade;', '&alefsym;', '&larr;', '&uarr;', '&rarr;', '&darr;', '&harr;', '&crarr;', '&lArr;', '&uArr;', '&rArr;', '&dArr;', '&hArr;', '&forall;', '&part;', '&exist;', '&empty;', '&nabla;', '&isin;', '&notin;', '&ni;', '&prod;', '&sum;', '&minus;', '&lowast;', '&radic;', '&prop;', '&infin;', '&ang;', '&and;', '&or;', '&cap;', '&cup;', '&int;', '&there4;', '&sim;', '&cong;', '&asymp;', '&ne;', '&equiv;', '&le;', '&ge;', '&sub;', '&sup;', '&nsub;', '&sube;', '&supe;', '&oplus;', '&otimes;', '&perp;', '&sdot;', '&lceil;', '&rceil;', '&lfloor;', '&rfloor;', '&lang;', '&rang;', '&loz;', '&spades;', '&clubs;', '&hearts;', '&diams;'],
    arr2: ['&#160;', '&#161;', '&#162;', '&#163;', '&#164;', '&#165;', '&#166;', '&#167;', '&#168;', '&#169;', '&#170;', '&#171;', '&#172;', '&#173;', '&#174;', '&#175;', '&#176;', '&#177;', '&#178;', '&#179;', '&#180;', '&#181;', '&#182;', '&#183;', '&#184;', '&#185;', '&#186;', '&#187;', '&#188;', '&#189;', '&#190;', '&#191;', '&#192;', '&#193;', '&#194;', '&#195;', '&#196;', '&#197;', '&#198;', '&#199;', '&#200;', '&#201;', '&#202;', '&#203;', '&#204;', '&#205;', '&#206;', '&#207;', '&#208;', '&#209;', '&#210;', '&#211;', '&#212;', '&#213;', '&#214;', '&#215;', '&#216;', '&#217;', '&#218;', '&#219;', '&#220;', '&#221;', '&#222;', '&#223;', '&#224;', '&#225;', '&#226;', '&#227;', '&#228;', '&#229;', '&#230;', '&#231;', '&#232;', '&#233;', '&#234;', '&#235;', '&#236;', '&#237;', '&#238;', '&#239;', '&#240;', '&#241;', '&#242;', '&#243;', '&#244;', '&#245;', '&#246;', '&#247;', '&#248;', '&#249;', '&#250;', '&#251;', '&#252;', '&#253;', '&#254;', '&#255;', '&#34;', '&#38;', '&#60;', '&#62;', '&#338;', '&#339;', '&#352;', '&#353;', '&#376;', '&#710;', '&#732;', '&#8194;', '&#8195;', '&#8201;', '&#8204;', '&#8205;', '&#8206;', '&#8207;', '&#8211;', '&#8212;', '&#8216;', '&#8217;', '&#8218;', '&#8220;', '&#8221;', '&#8222;', '&#8224;', '&#8225;', '&#8240;', '&#8249;', '&#8250;', '&#8364;', '&#402;', '&#913;', '&#914;', '&#915;', '&#916;', '&#917;', '&#918;', '&#919;', '&#920;', '&#921;', '&#922;', '&#923;', '&#924;', '&#925;', '&#926;', '&#927;', '&#928;', '&#929;', '&#931;', '&#932;', '&#933;', '&#934;', '&#935;', '&#936;', '&#937;', '&#945;', '&#946;', '&#947;', '&#948;', '&#949;', '&#950;', '&#951;', '&#952;', '&#953;', '&#954;', '&#955;', '&#956;', '&#957;', '&#958;', '&#959;', '&#960;', '&#961;', '&#962;', '&#963;', '&#964;', '&#965;', '&#966;', '&#967;', '&#968;', '&#969;', '&#977;', '&#978;', '&#982;', '&#8226;', '&#8230;', '&#8242;', '&#8243;', '&#8254;', '&#8260;', '&#8472;', '&#8465;', '&#8476;', '&#8482;', '&#8501;', '&#8592;', '&#8593;', '&#8594;', '&#8595;', '&#8596;', '&#8629;', '&#8656;', '&#8657;', '&#8658;', '&#8659;', '&#8660;', '&#8704;', '&#8706;', '&#8707;', '&#8709;', '&#8711;', '&#8712;', '&#8713;', '&#8715;', '&#8719;', '&#8721;', '&#8722;', '&#8727;', '&#8730;', '&#8733;', '&#8734;', '&#8736;', '&#8743;', '&#8744;', '&#8745;', '&#8746;', '&#8747;', '&#8756;', '&#8764;', '&#8773;', '&#8776;', '&#8800;', '&#8801;', '&#8804;', '&#8805;', '&#8834;', '&#8835;', '&#8836;', '&#8838;', '&#8839;', '&#8853;', '&#8855;', '&#8869;', '&#8901;', '&#8968;', '&#8969;', '&#8970;', '&#8971;', '&#9001;', '&#9002;', '&#9674;', '&#9824;', '&#9827;', '&#9829;', '&#9830;'],

	isEmpty : function(val){
		if(val){
			return ((val===null) || val.length==0 || /^\s+$/.test(val));
		}else{
			return true;
		}
	},
	// Convert HTML entities into numerical entities
	HTML2Numerical : function(s){
	    return this.swapArrayVals(s, this.arr1, this.arr2);
	},	

	// Convert Numerical entities into HTML entities
	NumericalToHTML : function(s){
	    return this.swapArrayVals(s, this.arr2, this.arr1);
	},


	// Numerically encodes all unicode characters
	numEncode : function(s){
		
		if(this.isEmpty(s)) return "";

		var e = "";
		for (var i = 0; i < s.length; i++)
		{
			var c = s.charAt(i);
			if (c < " " || c > "~")
			{
				c = "&#" + c.charCodeAt() + ";";
			}
			e += c;
		}
		return e;
	},
	
	// HTML Decode numerical and HTML entities back to original values
	htmlDecode : function(s){

		var c,m,d = s;
		
		if(this.isEmpty(d)) return "";

		// convert HTML entites back to numerical entites first
		d = this.HTML2Numerical(d);
		
		// look for numerical entities &#34;
		arr=d.match(/&#[0-9]{1,5};/g);
		
		// if no matches found in string then skip
		if(arr!=null){
			for(var x=0;x<arr.length;x++){
				m = arr[x];
				c = m.substring(2,m.length-1); //get numeric part which is refernce to unicode character
				// if its a valid number we can decode
				if(c >= -32768 && c <= 65535){
					// decode every single match within string
					d = d.replace(m, String.fromCharCode(c));
				}else{
					d = d.replace(m, ""); //invalid so replace with nada
				}
			}			
		}

		return d;
	},		

	// encode an input string into either numerical or HTML entities
	htmlEncode : function(s,dbl){
			
		if(this.isEmpty(s)) return "";

		// do we allow double encoding? E.g will &amp; be turned into &amp;amp;
		dbl = dbl || false; //default to prevent double encoding
		
		// if allowing double encoding we do ampersands first
		if(dbl){
			if(this.EncodeType=="numerical"){
				s = s.replace(/&/g, "&#38;");
			}else{
				s = s.replace(/&/g, "&amp;");
			}
		}

		// convert the xss chars to numerical entities ' " < >
		s = this.XSSEncode(s,false);
		
		if(this.EncodeType=="numerical" || !dbl){
			// Now call function that will convert any HTML entities to numerical codes
			s = this.HTML2Numerical(s);
		}

		// Now encode all chars above 127 e.g unicode
		s = this.numEncode(s);

		// now we know anything that needs to be encoded has been converted to numerical entities we
		// can encode any ampersands & that are not part of encoded entities
		// to handle the fact that I need to do a negative check and handle multiple ampersands &&&
		// I am going to use a placeholder

		// if we don't want double encoded entities we ignore the & in existing entities
		if(!dbl){
			s = s.replace(/&#/g,"##AMPHASH##");
		
			if(this.EncodeType=="numerical"){
				s = s.replace(/&/g, "&#38;");
			}else{
				s = s.replace(/&/g, "&amp;");
			}

			s = s.replace(/##AMPHASH##/g,"&#");
		}
		
		// replace any malformed entities
		s = s.replace(/&#\d*([^\d;]|$)/g, "$1");

		if(!dbl){
			// safety check to correct any double encoded &amp;
			s = this.correctEncoding(s);
		}

		// now do we need to convert our numerical encoded string into entities
		if(this.EncodeType=="entity"){
			s = this.NumericalToHTML(s);
		}

		return s;					
	},

	// Encodes the basic 4 characters used to malform HTML in XSS hacks
	XSSEncode : function(s,en){
		if(!this.isEmpty(s)){
			en = en || true;
			// do we convert to numerical or html entity?
			if(en){
				s = s.replace(/\'/g,"&#39;"); //no HTML equivalent as &apos is not cross browser supported
				s = s.replace(/\"/g,"&quot;");
				s = s.replace(/</g,"&lt;");
				s = s.replace(/>/g,"&gt;");
			}else{
				s = s.replace(/\'/g,"&#39;"); //no HTML equivalent as &apos is not cross browser supported
				s = s.replace(/\"/g,"&#34;");
				s = s.replace(/</g,"&#60;");
				s = s.replace(/>/g,"&#62;");
			}
			return s;
		}else{
			return "";
		}
	},

	// returns true if a string contains html or numerical encoded entities
	hasEncoded : function(s){
		if(/&#[0-9]{1,5};/g.test(s)){
			return true;
		}else if(/&[A-Z]{2,6};/gi.test(s)){
			return true;
		}else{
			return false;
		}
	},

	// will remove any unicode characters
	stripUnicode : function(s){
		return s.replace(/[^\x20-\x7E]/g,"");
		
	},

	// corrects any double encoded &amp; entities e.g &amp;amp;
	correctEncoding : function(s){
		return s.replace(/(&amp;)(amp;)+/,"$1");
	},


	// Function to loop through an array swaping each item with the value from another array e.g swap HTML entities with Numericals
	swapArrayVals : function(s,arr1,arr2){
		if(this.isEmpty(s)) return "";
		var re;
		if(arr1 && arr2){
			//ShowDebug("in swapArrayVals arr1.length = " + arr1.length + " arr2.length = " + arr2.length)
			// array lengths must match
			if(arr1.length == arr2.length){
				for(var x=0,i=arr1.length;x<i;x++){
					re = new RegExp(arr1[x], 'g');
					s = s.replace(re,arr2[x]); //swap arr1 item with matching item from arr2	
				}
			}
		}
		return s;
	},

	inArray : function( item, arr ) {
		for ( var i = 0, x = arr.length; i < x; i++ ){
			if ( arr[i] === item ){
				return i;
			}
		}
		return -1;
	}

}

function OculatarHerramientaAddto()
{
    if($.browser.msie && $.browser.version < 7)
    {
        idIntervalo = setInterval("accederWeb()",500);
    }
}

function CambiarNombre(link, nombre1, nombre2)
{
    if(link.innerHTML == nombre1)
    {
        link.innerHTML = nombre2;
    }
    else
    {
        link.innerHTML = nombre1;
    }
}


var panelFicherosDisponibles = {
	idSection: '#section',
	cssDescripcion: '.descripcion',
	cssHeader: '.descripcion .header',
	cssPanelDesplegable: '.panel',
	idPanel: '#panelFicherosDisponibles',
	desplegable: '',
	enlace: '',
	linkPDFEs: 'gnossOnto:linkPDFEs',
	linkPDFEn: 'gnossOnto:linkPDFEn',
	cssFicheroPdf: '.isPdf',
	enlaces: [],
	literales: [],
	init: function(){
		this.config();
		this.crear();
		this.configPanel();
		this.ficheros();
		this.escribirFicheros();
		this.enganchar();
		return;
	},
	config: function(){
		this.section = $(this.idSection);
		this.header = $(this.cssHeader, this.section);
		return;
	},
	ficheros: function(){
		var ficheros = $(this.cssFicheroPdf, this.section);
		var enlaces = [];
		var literales = [];
		ficheros.each(function(){
			enlaces.push($('a', this));
			literales.push($(this).prev().html());
		});
		this.enlaces = enlaces;
		this.literales = literales;
		return;
	},
	escribirFicheros: function(){
		var enlace;
		var href;
		var html;		
		var lis = '';
		var that = this;
		$(this.enlaces).each(function(indice){
			enlace = that.enlaces[indice];
			enlace = $(enlace);
			href = enlace.attr('href');
			html = enlace.html();		
			lis += '<li>' + that.literales[indice] + ' <a href="' + href + '">' + html + '</a></li>';
		});
		this.ul.html(lis);
		return;
	},
	crear: function(){
		var encabezado = $('h3', this.header);
		encabezado.after(panelFicherosDisponibles.template());
		return;
	},
	configPanel: function(){
		this.panel = $(this.idPanel, this.section);
		this.desplegable = $(this.cssPanelDesplegable, this.panel);
		this.enlace = $('.pdf a', this.panel);
		this.ul = $('ul', this.panel);
		return;
	},	
	enganchar: function(){
		var that = this;
		this.enlace.bind('click', function(evento){
			evento.preventDefault();
			that.comportamiento(evento);
			return false;
		});
		return;
	},
	comportamiento: function(evento){
		this.desplegable.is(':visible')? this.desplegable.hide():this.desplegable.show();
	},
	template: function(){
		var html = '<div id="panelFicherosDisponibles">\n';
		html += '<p class="pdf"><a href="/">'+ form.fichaTecnicaPDF +'<\/a><\/p>';
		html += '<div class="panel">\n';	
		html += '<ul>\n';	
		html += '<li>No hay ficheros disponibles<\/li>\n';	
		html += '<\/ul>\n';	
		html += '<\/div>\n';	
		html += '<\/div>\n';	
		return html;
	}
};

$(function(){
	var ficheros = $('#section .isPdf');
	var isFicherosDisponibles = ficheros.length > 0;
	if(isFicherosDisponibles) panelFicherosDisponibles.init();
})