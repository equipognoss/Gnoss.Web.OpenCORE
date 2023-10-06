
//****************************************************************************************************************/
/*
Métodos compartidos y útiles por todos los ficheros JS de la sección Administración
*/
//****************************************************************************************************************/


//****************************************************************************************************************/
// Utiles: Métodos / Extensiones / Variables Globales
/*****************************************************************************************************************/

// Variables para gestión de variables de la comunidad
let $_getVariables = { isset: false };
let $_getGlobalVariables = {};
// Uso de "body" para comportamientos JS
let body = $("body");


// Variables para resultados y facetas
let contResultados = 0;
let filtrosPeticionActual = '';

let funcionExtraResultados = "";
let funcionExtraFacetas = "";


// Objeto que contendrá opciones de configuración de la aplicación. Se establecerán en el método/operativa comportamientoInicial
const App = {};


/**
 * Objeto/Enum con los diferentes tipos de respuestas posibles
 */
const requestFinishResult = {
	ok: 'OK',
	ko: 'KO',
	errorInvitado: "ErrorInvitado",
	errorInvitadoMessage: "No dispones de credenciales para realizar esta acción. Por favor, inicia sesión con mayores privilegios y vuelva intentarlo de nuevo.",
	errorNoLogin: "ErrorNoLogin",
	errorNoLoginMessage: "No puedes acceder a esta función porque no estás autenticado en el sistema. Por favor, inicia sesión para continuar.",	
	message: "",   
}

/**
 * Método que permite convertir un valor String a boolean
 * @param {String} stringValue: String para controlar si es o no valor booleano 
 * @returns 
 */
const stringToBoolean = (stringValue) => {
    switch(stringValue?.toLowerCase()?.trim()){
        case "True":
		case "true": 
        case "yes": 
        case "1": 
          return true;

        
		case "false": 
		case "False":
        case "no": 
        case "0": 
        case null: 
        case undefined:
          return false;

        default: 
          return JSON.parse(stringValue);
    }
}

/**
 * Método para comprobar si un objeto en JS está vacío, es decir, no tiene propiedades
 * @param {object} jsObject
 * @returns 
 */
function isObjectEmpty(jsObject) {
	return Object.keys(jsObject).length === 0;
}

// Verificar si un objeto es un elemento de jQuery
function isJqueryObject(obj) {
	return obj instanceof jQuery || obj instanceof $;
}

/**
 * Extensión en Arrays para buscar objetos por "key"
 */
Array.prototype.findValueByKey = function (pKey) {
    var resultado = $.grep(this, function (array) {
        return array.key == pKey;
    });

    if (resultado[0] != null)
    {
        return resultado[0].value;
    }

    return null;
};


/* Permitir envío de Cookies a otro dominio */
$.ajaxSetup({
    crossDomain: true,
    xhrFields: {
        withCredentials: true
    }
});


/*
Extender la funcionalidad de String para detección del final e inicio de la cadena. Usado por ejemplo en 'ObtenerHash2'
*/
String.prototype.EndsWith = function (suffix) {
    return this.indexOf(suffix, this.length - suffix.length) !== -1;
};

String.prototype.StartsWith = function (searchString, position) {
    position = position || 0;
    return this.lastIndexOf(searchString, position) === position;
};


/**
 * Método que generar un guid o id para un elemento nuevo creado.
 */
const guidGenerator = function() {
    const S4 = function () {
        return (((1 + Math.random()) * 0x10000) | 0).toString(16).substring(1);
    };
    return (S4() + S4() + "-" + S4() + "-" + S4() + "-" + S4() + "-" + S4() + S4() + S4());
}

const intGenerator = function () {
	
	return (((1 + Math.random()) * 0x10000) | 0).toString(16).substring(1);
	
}


/**
 * Método para obtener todas las variables
 */
const $_GETAllVariables = function () {
    var scripts = document.getElementsByTagName("script");
    for (var i = 0; i < scripts.length; i++) {
        var script = (scripts[i].src + "").split("/");
        script = script[script.length - 1].split("?", 2);
        if (script.length > 1) {
            var parameters = script[1].split("&")
            for (var j = 0; j < parameters.length; j++) {
                var vars = parameters[j].split("=");
                if (!$_getVariables[script[0]]) $_getVariables[script[0]] = {};
                $_getVariables[script[0]][vars[0]] = vars[1];
                $_getGlobalVariables[vars[0]] = vars[1];
            }
        }
    }
    $_getVariables.isset = true;
};

/**
 * Método para coger la versión de una librería en cuestión. Ejemplo: Usado para ckEditor.js
 */
$_GET = function (paramToGet, jsFile) {
    if (!$_getVariables.isset)
        $_GETAllVariables();
    if (jsFile)
        return $_getVariables[jsFile][paramToGet];
    else
        return $_getGlobalVariables[paramToGet];
};

/** Métodos para encode y Decode (Usados en elementos secundarios al editar o crear nuevas instancias de objetos ) */
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


/**
 * Objeto para realizar Encode de caracteres. Utilizado por ejemplo en autocomplete.js
 * 
 */
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


/**

 */


/**
 * Método para hacer scroll hasta un elemento deseado. El scroll se realiza dentro de un modal. Si no se proporciona un modalIdToScroll se hará scroll en el modal-container
 * que es donde se cargan datos de forma dinámica y se utiliza para múltiples usos. 
 * @param {jqueryElement} jqueryElmentToScrollTo: Elemento jquery hasta donde se desea realizar el scroll
 * @param {string} modalIdToScroll: String o identificador del modal donde se desea realizar el scroll.
 * @param {string} completion: Función a realizar cuando se realice el scroll
 */
function scrollInModalView(jqueryElmentToScrollTo, modalIdToScroll = undefined, completion = undefined){

	// Si no se proporciona un modalIdToScroll se hará scroll directamente en el modal-container
	if (modalIdToScroll == undefined){
		modalIdToScroll = "modal-container";
	}

	const position = jqueryElmentToScrollTo.position();

	$(`#${modalIdToScroll}`).find(".modal-body")
	.animate(
		{ scrollTop: position.top -30 },
		{
			complete: function(){
				if (completion != undefined){
					completion();
				}
			}
		},
	2000);
}

/**
 * Método para hacer scroll hasta arriba del todo del modal.
 * @param {jqueryObject} modal: Objeto jquery que corresponde con el modal que se va a cerrar
 */
 function scrollInModalViewToTop(modal){
	modal.find(".modal-body").scrollTop(0);	
}



/**
 * Método para sustituir un texto de entrada en base a la búsqueda realizada. Utilizado por jqueryAutocomplete cuando se está introduciendo texto en el input de tipo
 * autoComplete.
 * @param {String} texto 
 * @param {String} busqueda 
 * @param {String} reemplazo 
 * @returns 
 */
function replaceAll(texto, busqueda, reemplazo) {
    var resultado = '';

    while (texto.toString().indexOf(busqueda) != -1) {
        resultado += texto.substring(0, texto.toString().indexOf(busqueda)) + reemplazo;
        texto = texto.substring(texto.toString().indexOf(busqueda) + busqueda.length, texto.length);
    }

    resultado += texto;

    return resultado;
}

/**
 * Función que reseteará o vaciará el contenido modal container para que se quede como la vista inicial de "Loading ..."
 * Este método se aconseja hacerlo una vez el modal container que es de dinámica creación sea cerrado por el usuario o de forma automática.
 */
function resetearModalContainer(){
	const $modalContainer = $("#modal-container");

	// Añadir la clase por defecto para que se muestre en el top de la página
	$modalContainer.addClass("modal-top");
	// Panel que hay que resetear/rellenar con el initialContainerContent
	let panelToReset = $modalContainer.find("#modal-dinamic-content");
	// HTML que cargaremos de nuevo una vez se cierre el formulario (resetearlo de inicio)
	let initialContainerContent = '';
	initialContainerContent += '<div id="content">';
	initialContainerContent += '<div class="modal-header">';
	initialContainerContent += '<p class="modal-title">';
	initialContainerContent += '<span class="spinner-border white-color mr-2"></span>';
	initialContainerContent += 'Cargando ...';
	initialContainerContent += '</p>';
	initialContainerContent += '<span class="material-icons cerrar" data-dismiss="modal" aria-label="Close">close</span>';
	initialContainerContent += '</div>';
	initialContainerContent += '<div class="modal-body"></div>';
	initialContainerContent += '</div>';
	// Incluir el panel inicial para ser reutilizado
	panelToReset.html(initialContainerContent).fadeIn();
};

/**
 * Métodos o comportamientos necesarios en la carga de la página
 */
const comportamientoInicial = {
	/**
	 * Iniciar funcionamiento de Selects con librería Select2
	 */
	iniciarSelects2: function (pParams) {		
		// Configurar Selects2
		this.select2 = $("body").find('.js-select2');
		this.defaultOptions = {
				minimumResultsForSearch: 10,
				width: '100%',
		};	
		// Montar Selects
		this.select2.select2(this.defaultOptions);	
	},
	
	/**
	 * Establecer el idioma de la aplicación o por defecto para, por ejemplo, mostrar en el idioma correcto los mensajes de error o success.
	 */
	setCurrentLanguage: function(){
		// Código del idioma actual de la plataforma
		App.LANGUAGE_CODE = $("#inpt_Idioma").val();
	},

	/**
	 * Método para resaltar en la página las posibles coincidencias buscadas por el usuario a través del buscador lateral de DevTools.
	 * Este método se ejecutará al cargar la página. Si existe un parámetro en la url de "helpersearcher", se buscarán posibles coincidencias en la web cargada.
	 */
	highlightHelperResults: function(){
		// Comprobar si hay parámetros para búsqueda "ayudada/sugerida" 
		const queryString = new URL(location.href).searchParams.get('helpersearcher')

		if (queryString != undefined && queryString.length > 3){	
			const myHilitor = new Hilitor("mainContent");
			myHilitor.setMatchType("left");
			// Aplicar búsqueda para resaltar las palabras
  			myHilitor.apply(queryString);
			setTimeout(function () {
				myHilitor.remove();
			},5000);

		}
	},
};

/**
 * Función para resaltar elementos en una página. Ej. Usado para sombrear o resaltar palabras que coincidan con la búsqueda realizada 
 * por el usuario desde el menú lateral de navegación (Búsqueda asistida)
 * @param {*} id : ID del html donde se deseará buscar. Si no se indica nada, tomará el body de la web.
 * @param {*} tag 
 */
function Hilitor(id, tag){
  // private variables
  var targetNode = document.getElementById(id) || document.body;
  var hiliteTag = tag || "MARK";
  var skipTags = new RegExp("^(?:" + hiliteTag + "|SCRIPT|FORM|SPAN)$");
  var colors = ["#006eff"];
  var wordColor = [];
  var colorIdx = 0;
  var matchRegExp = "";
  var openLeft = false;
  var openRight = false;

  // characters to strip from start and end of the input string
  var endRegExp = new RegExp('^[^\\w]+|[^\\w]+$', "g");

  // characters used to break up the input string into words
  var breakRegExp = new RegExp('[^\\w\'-]+', "g");

  this.setEndRegExp = function(regex) {
    endRegExp = regex;
    return endRegExp;
  };

  this.setBreakRegExp = function(regex) {
    breakRegExp = regex;
    return breakRegExp;
  };

  this.setMatchType = function(type)
  {
    switch(type)
    {
      case "left":
        this.openLeft = false;
        this.openRight = true;
        break;

      case "right":
        this.openLeft = true;
        this.openRight = false;
        break;

      case "open":
        this.openLeft = this.openRight = true;
        break;

      default:
        this.openLeft = this.openRight = false;

    }
  };

  this.setRegex = function(input)
  {
    input = input.replace(endRegExp, "");
    input = input.replace(breakRegExp, "|");
    input = input.replace(/^\||\|$/g, "");
    if(input) {
      var re = "(" + input + ")";
      if(!this.openLeft) {
        re = "\\b" + re;
      }
      if(!this.openRight) {
        re = re + "\\b";
      }
      matchRegExp = new RegExp(re, "i");
      return matchRegExp;
    }
    return false;
  };

  this.getRegex = function()
  {
    var retval = matchRegExp.toString();
    retval = retval.replace(/(^\/(\\b)?|\(|\)|(\\b)?\/i$)/g, "");
    retval = retval.replace(/\|/g, " ");
    return retval;
  };

  // recursively apply word highlighting
  this.hiliteWords = function(node)
  {
    if(node === undefined || !node) return;
    if(!matchRegExp) return;
    if(skipTags.test(node.nodeName)) return;

    if(node.hasChildNodes()) {
      for(var i=0; i < node.childNodes.length; i++)
        this.hiliteWords(node.childNodes[i]);
    }
    if(node.nodeType == 3) { // NODE_TEXT

      var nv, regs;

      if((nv = node.nodeValue) && (regs = matchRegExp.exec(nv))) {

        if(!wordColor[regs[0].toLowerCase()]) {
          wordColor[regs[0].toLowerCase()] = colors[colorIdx++ % colors.length];
        }

        var match = document.createElement(hiliteTag);
        match.appendChild(document.createTextNode(regs[0]));
        match.style.backgroundColor = wordColor[regs[0].toLowerCase()];
        //match.style.color = "#000";
		match.style.color = "#fff";

        var after = node.splitText(regs.index);
        after.nodeValue = after.nodeValue.substring(regs[0].length);
        node.parentNode.insertBefore(match, after);

      }
    }
  };

  // remove highlighting
  this.remove = function()
  {
    var arr = document.getElementsByTagName(hiliteTag), el;
    while(arr.length && (el = arr[0])) {
      var parent = el.parentNode;
      parent.replaceChild(el.firstChild, el);
      parent.normalize();
    }
  };

  // start highlighting at target node
  this.apply = function(input)
  {
    this.remove();
    if(input === undefined || !(input = input.replace(/(^\s+|\s+$)/g, ""))) {
      return;
    }
    if(this.setRegex(input)) {
      this.hiliteWords(targetNode);
    }
    return matchRegExp;
  };

}

//****************************************************************************************************************/
// Utiles: Funciones útiles que pueden aprovecharse en Backoffice.
/*****************************************************************************************************************/


/**
 * Método para crear un elemento HTML a partir de un string de HTML
 * @param {*} htmlString 
 * @returns 
 */
function createElementFromHTML(htmlString) {
	const div = document.createElement('div');
	div.innerHTML = htmlString.trim();
  
	// Change this to div.childNodes to support multiple top-level nodes.
	return div.firstChild;
}


/**
 * Método que devolverá el texto necesario en el idioma correspondiente dependiendo del idioma establecido en la comunidad (DevTools)
 * El idioma se obtendrá a partir del fichero "textosDevTools.js" que deberá estar precargado en la web. 
 * @param {*} languageKey 
 * @returns Devuelve el texto deseado en el idioma correspondiente según el languageKey pasado.
 */
function getTextoIdioma(languageKey){
	// Idioma detectado en la comunidad
	// App.LANGUAGE_CODE

	// Por defecto el idioma será en español si hay algún problema.
	let LANGUAGE_CODE = "";
	
	if(App.LANGUAGE_CODE == undefined){
		LANGUAGE_CODE = "es";
	}else{
		LANGUAGE_CODE = App.LANGUAGE_CODE;
	}
	const textoIdioma = TEXTOS_DEVTOOLS[LANGUAGE_CODE][languageKey];
	return textoIdioma;
}

/**
 * Método para comprobar si es un correo electrónico válido
 * @param {string} email 
 * @returns bool
 */
function isEmail(email) {
	var regex = /^([a-zA-Z0-9_.+-])+\@(([a-zA-Z0-9-])+\.)+([a-zA-Z0-9]{2,4})+$/;
	return regex.test(email);
}


/**
 * Método que construye un elemento HTML e inserta nodos HTML en él para poder hacer algún tratamiento o búsqueda necesario.
 * @param {*} htmlString : Texto Html que contiene los nodos html que se insertarán el el div que hará de "padre"
 * @returns : Devuelve el elemento padre (div) que contiene el html donde se realizarán posibles búsquedas o acciones deseadas.
 */
function createElementFromHTML(htmlString) {
	const div = document.createElement('div');
	div.innerHTML = htmlString.trim();  
	// Change this to div.childNodes to support multiple top-level nodes.
	return div;
}

/**
 * Método para copiar un texto al portapapeles y mostrar el aviso al usuario del copiado correcto
 * @param {String} text : Texto que se desea copiar
 * @returns 
 */
function copyTextToClipBoard(text){
	
	if (!navigator.clipboard) {
		fallbackCopyTextToClipboard(text);
		return;
	}
	navigator.clipboard.writeText(text).then(
		function () {
			mostrarNotificacion("success", "ID copiado al portapapeles");
		},
		function (err) {
			console.error(err);
		}
	);
}

/**
 * Método par copiar texto en el portapapeles. Se ejecutará dentro del método "copyTextToClipBoard" una vez el navegador sea compatible con esta característica.
 * @param {String} text : Texto que se desea copiar en el portapapeles
 */
function fallbackCopyTextToClipboard(text) {
	const textarea = document.createElement('textarea');
	textarea.textContent = text;
	document.body.appendChild(textarea);
  
	let selection = document.getSelection();
	let range = document.createRange();
	range.selectNode(textarea);
	selection.removeAllRanges();
	selection.addRange(range);
  
	try {
		const successful = document.execCommand("copy");
		if (successful) {
			mostrarNotificacion("success", "ID copiado al portapapeles");
		}
	} catch (err) {
		console.error(err);
	}
	selection.removeAllRanges();
	document.body.removeChild(textarea);
}




/**
 * Método para obtener la URL actual a partir del primer /. Permitirá construir la URL evitando urls adicionales para que sea la URL base inicial.
 * @param {string} url: Url que se desea "formatear". Si no se proporciona, tomará por defecto la página actual.
 * @returns Devuelve la url base
 */
function refineURL(url = undefined)
{
    //get full URL
    const currURL= url == undefined ? window.location.href : url; //get current address
    
    //Get the URL between what's after '/' and befor '?' 
    //1- get URL after'/'
    // const afterDomain= currURL.substring(currURL.lastIndexOf('/') + 1);
	const afterDomain= currURL;
    //2- get the part before '?'
    const beforeQueryString= afterDomain.split("?")[0];  
 
    return beforeQueryString;     
}


/**
 * Acción que se ejecutará y que mostrará información en un modal dinámico que es pintado vía JS.
 * La acción a realizar será de tipo "Sí | No".
 * Ej: Usado en sección "Miembros de la comunidad" (Expulsar, Enviar Newsletter, Bloquear usuario )
 * @param {string} titulo: Título que tendrá el panel modal
 * @param {any} textoBotonPrimario: Texto del botón primario
 * @param {any} textoBotonSecundario: Texto del botón primario
 * @param {string} texto: El texto o mensaje a modo de título que se mostrará para que el usuario sepa la acción que se va a realizar
 * @param {string} id: Identificador del recurso/persona/item sobre el que se aplicará la acción
 * @param {any} accion: Acción o función que se ejecutará cuando se pulse en el botón primario.
 * @param {any} namePerson: Nombre de la persona la que se le aplicará la acción.
 * @param {any} idModalPanel: Panel modal contenedor donde se insertará este HTML (Por defecto será #modal-container)
 */
 function MostrarAccionSiNoEnModal(titulo, textoBotonPrimario, textoBotonSecundario, texto, id, accion, namePerson = "", textoInferior=null, idModalPanel="#modal-container") {

    // Panel dinámico del modal padre donde se insertará la vista "hija"
    const $modalDinamicContentPanel = $('#modal-container').find('#modal-dinamic-content #content');  
	
	// Indicar la persona a la que se le aplica la acción
	const destinoAccionPersona = namePerson != "" ? `a ${namePerson}` : "";    

    // Plantilla del panel html que se cargará en el modal contenedor al pulsar en la acción
    var plantillaPanelHtml = '';
    // Cabecera del panel
    plantillaPanelHtml += '<div class="modal-header">';
        plantillaPanelHtml += '<p class="modal-title"><span class="material-icons">label</span>'+ titulo + destinoAccionPersona +'</p>';
        plantillaPanelHtml += '<span class="material-icons cerrar" data-dismiss="modal" aria-label="Close">close</span>';
    plantillaPanelHtml += '</div>';
    // Cuerpo del panel
    plantillaPanelHtml += '<div class="modal-body">';
        plantillaPanelHtml += '<div class="formulario-edicion">';
            plantillaPanelHtml += '<div class="form-group">';                
                plantillaPanelHtml += '<p>'+texto+'</p>';
            plantillaPanelHtml += '</div>';
            if (textoInferior != undefined) {
                if (textoInferior.length > 5) {
                    plantillaPanelHtml += '<div class="form-group">';
                    plantillaPanelHtml += '<label class="control-label">' + textoInferior + '</label>';
                    plantillaPanelHtml += '</div>';
                }
            }          
            // Panel de botones para la acción
            plantillaPanelHtml += '<div id="modal-dinamic-action-buttons" class="form-actions">'
                plantillaPanelHtml += '<button data-dismiss="modal" class="btn btn-outline-primary">'+textoBotonSecundario+'</button>'
                plantillaPanelHtml += '<button class="btn btn-primary ml-1">'+textoBotonPrimario+'</button>'
            plantillaPanelHtml += '</div>';                       
        plantillaPanelHtml += '</div>';        
    plantillaPanelHtml += '</div>'; 

    // Insertar el código Html en la vista del modal    
    $modalDinamicContentPanel.html(plantillaPanelHtml);       

    // Acceso a los botones
    const botones = $modalDinamicContentPanel.find('#modal-dinamic-action-buttons > button');

    // Asignación de la función al botón "Sí" de acción
    $(botones[1]).on("click", function () {
      // Ocultar el panel modal de bootstrap - De momento estará visible. Se oculará si se muestra mensaje de OK pasados 1.5 segundos
        //$('#modal-container').modal('hide');
    }).click(accion);
}

//****************************************************************************************************************/
// Utiles: Facetas y Resultados. Métodos y comportamientos necesarios
/*****************************************************************************************************************/

/**
 * Método de autocompletar facetas/etiquetas. Utilizado en sección de facetas de "Administrar Miembros"
 * @param {*} control 
 * @param {*} pClaveFaceta 
 * @param {*} pIncluirIdioma 
 */
function autocompletarEtiquetasTipado(control, pClaveFaceta, pIncluirIdioma) {
    //var tablaPropiaAutoCompletar = $('input.inpt_tablaPropiaAutoCompletar').val().toLowerCase() == "true";

    var extraFaceta = '';

    if (pIncluirIdioma) {
        extraFaceta = '[MultiIdioma]';
    }

    $(control).autocomplete(
		null,
		{
			servicio: new WS($('input.inpt_urlServicioAutocompletarEtiquetas').val(), WSDataType.jsonp),
			metodo: 'AutoCompletarTipado',
			//url: $('#input.inpt_urlServicioAutocompletarEtiquetas').val() + "/AutoCompletarTipado",
			//type: "POST",
			delay: 0,
			scroll: false,
			selectFirst: false,
			minChars: 1,
			width: 170,
			cacheLength: 0,
			extraParams: {
				pProyecto: $('input.inpt_proyID').val(),
				//pTablaPropia: tablaPropiaAutoCompletar,
				pFacetas: pClaveFaceta + extraFaceta,
				pOrigen: origenAutoCompletar,
				pIdentidad: $('input.inpt_identidadID').val(),
				pIdioma: $('input.inpt_Idioma').val(),
				botonBuscar: control.id + 'botonBuscar'
			}
		}
	);
    if (control.attributes["onfocus"] != null) {
        control.attributes.removeNamedItem('onfocus');
    }

    //pintarTagsInicio();
}


/**
 * Detectar que la carga de Facetas ha finalizado para asociarle comportamientos JS
 */
function CompletadaCargaFacetas(){
	comportamientoCargaFacetas.init();
	if(typeof (window.comportamientoCargaFacetasComunidad) == 'function') {
		comportamientoCargaFacetasComunidad();
	}		
}

/* 
* Detectar que la carga de Facetas ha finalizado para asociarle comportamientos JS
*/
var comportamientoCargaFacetas = {
    init: function () {
        // Longitud facetas por CSS
		// limiteLongitudFacetas.init();
		facetedSearch.init();
		$('.verMasFaceta').each(function () {
		    var enlace = $(this);
		    var params = enlace.attr('rel').split('|');
		    var faceta = params[0];
		    var controlID = params[1];
		    enlace.unbind("click").click(function (evento) {
		        evento.preventDefault();
		        VerFaceta(faceta, controlID);
		    });
		});
		return;
	}
}


// Búsquedas y Facetas en Administración
var facetedSearch = {
	noCollapse: 'noCollapse',
	init: function(){
		//this.config();
		//this.comportamiento();
	},
	config: function(){
		this.facetas = $('#facetedSearch .box:not(.' + this.noCollapse + '):not(.categories)');
		return;
	},
	ocultar: function(){
		this.facetas.each(function(){
			var faceta = $(this);
			var searchBox = $('.facetedSearchBox', faceta);
			if(faceta.height() < 72) return;
			searchBox.hide();
			faceta.css({
				'height': '72px',
				'background': '#f0f0f0',
				'border-bottom': 'none'
			});			
		});
		return;
	},
	comportamiento: function(){
		var that = this;
		this.facetas.each(function(){
			var faceta = $(this);
			var searchBox = $('.facetedSearchBox', faceta);
			if(faceta.height() < 72) return;
			searchBox.hide();
			faceta.css({
				'height': '72px',
				'overflow': 'hidden'
			});
			faceta.hover(
				function(){
					that.ocultar();
					searchBox.show();
					var altura = faceta.height();
					faceta.css({
						'height': '100%',
						'background': '#eee'
					}, 1000);
				},
				function(){
					return;
					searchBox.hide();
					faceta.css({
						'height': '72px',
						'background': '#f0f0f0',
						'border-bottom': 'none'
					});
				})
		});		
	}
}; 

/**
 * Buscar un parámetro de tipo GET
 * @param {any} parameterName: Nombre del parámetro a buscar
 */
 function findGetParameter(parameterName) {
    var result = null,
        tmp = [];
    location.search
        .substr(1)
        .split("&")
        .forEach(function (item) {
            tmp = item.split("=");
            if (tmp[0] === parameterName) result = decodeURIComponent(tmp[1]);
        });
    return result;
}

/**
 * Mostrar número de categorías para comportamiento de Facetas y Resultados
 */
var mostrarNumeroCategorias = {
    init: function () {
        this.comportamiento();
    },
    config: function () { return },
    comportamiento: function () {
        var categorias = $('#section .resource .categorias ul');
        categorias.each(function () {
            var ul = $(this);
            var verMas = ul.find('a.verTodasCategoriasEtiquetas');
            if (verMas.length == 0) {
                var lis = $('li', ul);
                if (lis.size() <= 3) return;
                lis.each(function (indice) {
                    if (indice > 2) $(this).hide();
                })
                ul.append('<li><a class=\"verTodasCategoriasEtiquetas mas\" href="#" title=\"ver todos\">' + form.masMIN + '...<\/a><\/li>');
            }
        });
        return;
    }
};

/**
 * Mostrar número de Etiquetas para comportamiento de Facetas y Resultados
 */
var mostrarNumeroEtiquetas = {
    init: function () {
        this.comportamiento();
    },
    config: function () { return },
    comportamiento: function () {
        $('#section  .resource .etiquetas ul').each(function () {
            var ul = $(this);
            var verMas = ul.find('a.verTodasCategoriasEtiquetas');
            if (verMas.length == 0) {
                var lis = $('li', ul);
                if (lis.size() <= 3) return;
                lis.each(function (indice) {
                    if (indice > 2) $(this).hide();
                })
                ul.append('<li><a class=\"verTodasCategoriasEtiquetas mas\" href="#" title=\"ver todos\">' + form.masMIN + '...<\/a><\/li>');
            }
        });
        return;
    }
};

/**
 * Ver todas las etiquetas de una categoría. Llamado en Facetas y Resultados
 */
var verTodasCategoriasEtiquetas = {
    init: function () {
        this.comportamiento();
    },
    config: function () { return },
    comportamiento: function () {
        $('#section  .resource .verTodasCategoriasEtiquetas').each(function () {
            $(this).unbind();
            $(this).bind('click', function (evento) {
                evento.preventDefault();
                var enlace = $(evento.target);
                var ul = enlace.parents('ul');
                var lis = $('li', ul);
                var total = lis.size();
                if (enlace.hasClass('mas')) {
                    lis.show();
                    enlace.removeClass('mas');
                    enlace.text(form.menos);
                } else {
                    lis.each(function (indice) {
                        var li = $(this);
                        if (indice > 2 && indice < total - 1) li.hide();
                    });
                    enlace.addClass('mas');
                    enlace.text(form.masMIN+'...');
                }
            })
        });
        return;
    }
};

/*
 Método usado en método FinalizarMontarFacetas
*/
function HayFiltrosActivos(pFiltros) {
    $('#ctl00_ctl00_txtBusqueda').val('');
    if (pFiltros != "") {
        var filtros = pFiltros.split('|');
        for (var i = 0; i < filtros.length; i++) {
            if (filtros[i].indexOf('pagina=') == -1 && filtros[i].indexOf('orden=') == -1 && filtros[i].indexOf('ordenarPor=') == -1) {
                if (filtros[i].indexOf('search=') != -1) {
                    $('#ctl00_ctl00_txtBusqueda').val(filtros[i].substring(filtros[i].indexOf('=') + 1).replace('|', ''));
                }
                return true;
            }
        }
    }
    return false;
}


let arriba;
/**
 * Método para subir al principio de la página si se está usando búsquedas por Facetas
 */
function SubirPagina() {
    if (document.body.scrollTop != 0 || document.documentElement.scrollTop != 0) {
        window.scrollBy(0, -50);
        arriba = setTimeout('SubirPagina()', 5);
    }
    else clearTimeout(arriba);
}

/**
 * Comportamiento necesario para poder pintar las propiedades de recursos, items o acciones de la lista de resultados obtenida.
 * Ej: En "Administrar Miembros de la Comunidad" se hace la petición para saber qué acciones se pueden realizar sobre cada miembro (No enviar Newsletter, Bloquear, Expulsar..)
 * Este método se llama una vez los resultados se han traido de JS "FinalizarMontarResultados".
 * 
 */
var ObteniendoAcciones = false;
function ObtenerAccionesListadoMVC(pUrlPagina) {

	const currentUrl = location.href;
	if (currentUrl.includes("administrar-miembros")){
		pUrlPagina += "admin";
	}

    if (!ObteniendoAcciones) {
        ObteniendoAcciones = true;
        var resources = $('.resource-list .resource');
        var idPanelesAcciones = '';
        var numDoc = 0;
        resources.each(function () {
            var recurso = $(this);
            var accion = recurso.find('.group.acciones.noGridView');
            if (accion.length == 1 && accion.attr('id') != null) {
                accion.attr('id', accion.attr('id') + '_' + numDoc);
                idPanelesAcciones += accion.attr('id') + ',';
                numDoc++;
            }
            var accionesusuario = recurso.find('.group.accionesusuario.noGridView');
            if (accionesusuario.length == 1 && accionesusuario.attr('id') != null) {
                accionesusuario.attr('id', accionesusuario.attr('id') + '_' + numDoc);
                idPanelesAcciones += accionesusuario.attr('id') + ',';
                numDoc++;
            }
        });

        if (idPanelesAcciones != '') {
            try {
                var datosPost = {
                    callback: "CargarListaAccionesRecursos",
                    listaRecursos: idPanelesAcciones
                };

                $.post(pUrlPagina, datosPost, function (data) {
                    ObteniendoAcciones = false;
                    var paneles = idPanelesAcciones.split(',')

                    if (data != "") {
                        if (!Array.isArray(data)) {
                            // V5 -> Objeto de arrays -> Acceder al elemento 2 del objeto
                            for (var i in data.$values) {
								if ($.isNumeric(i)){
									if (data.$values[i].updateTargetId.indexOf("AccionesRecursoListado_") == 0) {
										var docID = data.$values[i].updateTargetId.substr("AccionesRecursoListado_".length, 36);
										var proyID = data.$values[i].updateTargetId.substr(("AccionesRecursoListado_" + docID).length, 36);
	
										var panel = "";
	
										$.each(idPanelesAcciones.split(','), function (i, val) {
	
											if (val.indexOf("accionesListado_" + docID) == 0) {
												if (val.indexOf(proyID) > 0) {
													panel = val;
												}
												else if (panel == "") {
													panel = val;
												}
											}
										});
	
										if (typeof (PintarAccionesRecursoPersonalizado) != "undefined") {
											PintarAccionesRecursoPersonalizado($('#' + panel), data.$values[i].html);
										}
										else {
											$('#' + panel).replaceWith(data.$values[i].html);
										}
									}
								}
                            }
																			
							// Una vez las acciones han sido pintadas / tratar de actualizar el contenido de la tabla para los miembros
							if (typeof (operativaGestionMiembros.cargarEstadosMiembrosEnTablaResultados) != 'undefined') {
								// Existe la operativa de miembros - Actualizar contenido de la tabla miembros 
								if (data.$values.length > 0){
									operativaGestionMiembros.cargarEstadosMiembrosEnTablaResultados();			
								}              											
							} 
							
							
							
                        } else {
                            for (var i in data) {
                                if (data[i].updateTargetId.indexOf("AccionesRecursoListado_") == 0) {
                                    var docID = data[i].updateTargetId.substr("AccionesRecursoListado_".length, 36);
                                    var proyID = data[i].updateTargetId.substr(("AccionesRecursoListado_" + docID).length, 36);

                                    var panel = "";

                                    $.each(idPanelesAcciones.split(','), function (i, val) {

                                        if (val.indexOf("accionesListado_" + docID) == 0) {
                                            if (val.indexOf(proyID) > 0) {
                                                panel = val;
                                            }
                                            else if (panel == "") {
                                                panel = val;
                                            }
                                        }
                                    });

                                    if (typeof (PintarAccionesRecursoPersonalizado) != "undefined") {
                                        PintarAccionesRecursoPersonalizado($('#' + panel), data[i].html);
                                    }
                                    else {
                                        $('#' + panel).replaceWith(data[i].html);
                                    }
                                }
                            }
                        }
                    }
                });
            }
            catch (ex) { }
        }
        else { ObteniendoAcciones = false; } 
    }
}


/**
 * Inicializar comportamientos al cargar la página
 */
$(document).ready(function() {
    comportamientoInicial.iniciarSelects2();
	comportamientoInicial.setCurrentLanguage();
	comportamientoInicial.highlightHelperResults();
});

//****************************************************************************************************************/
// PETICIONES / FETCH DATA
/*****************************************************************************************************************/
/**
 *
 * [GnossPeticionAjax]
 * Realizar petición Ajax a una url pasándole parámetros en caso de ser necesario.
 * @param {String} pUrl : Url a la que se desea realizar la petición.
 * @param {object} pParametros : Parámetros a pasar al controlador en la petición. Debe ser un objeto JS. Por defecto, vacío.
 * @param {bool} pTraerJson : Indicar si se desea traer datos en formato Json de la petición realizada. Por defecto, true.
 * @param {bool} pRedirectActive : Indicar si se desea redirigir de forma automática a la url proporcionada. Por defecto será "true". Dependerá de la respuesta del Controller
 */

const GnossPeticionAjax = function (
	pUrl,
	pPparametros = "",
	pTraerJson = true,
	pRedirectActive = true
) {
	var defr = $.Deferred();

	var esFormData = pPparametros instanceof FormData;

	$.ajax({
		url: pUrl,
		type: "POST",
		headers: {
			Accept: pTraerJson ? "application/json" : "*/*",
		},
		processData: !esFormData,
		contentType: esFormData ? false : "application/x-www-form-urlencoded",
		data: pPparametros,
		xhr: function () {
			var xhr = new window.XMLHttpRequest();
			// Handle progress
			//Upload progress
			xhr.upload.addEventListener(
				"progress",
				function (evt) {
					if (evt.lengthComputable) {
						var percentComplete = evt.loaded / evt.total;
						//Do something with upload progress
						defr.notify(Math.round(percentComplete * 100));
					}
				},
				false
			);

			return xhr;
		},
	})
		.done(function (response) {
			if (response == null || response.Status == undefined) {
				defr.resolve(response);
			}
			if (response.Status == "NOLOGIN") {
				//Hacer una peticion a la web, que nos devuelva la url del servicio de login + token
				//Hacer la peticion al servicio de login a recuperar la sesion
				//Si no estamos conectados, mostrar un panel de login
				//Si estamos conectados, re-llamar a esta funcion
				defr.reject("invitado");
			} else if (response.Status == "OK") {
				if (response.UrlRedirect != null) {
					if (pRedirectActive) {
						location.href = response.UrlRedirect;
					} else {
						defr.resolve(response.UrlRedirect);
					}
				} else if (response.Html != null) {
					defr.resolve(response.Html);
				} else {
					defr.resolve(response.Message);
				}
			} else if (response.Status == "Error") {
				defr.reject(response.Message);
			}
		})
		.fail(function (er) {
			//Comprobar el error
			var newtWorkError = er.readyState == 0; // && er.statusText == "error";
			if (newtWorkError) {
				defr.reject("NETWORKERROR");
			} else {
				defr.reject(er.statusText);
			}
		});

	return defr;
};




/**
 * Método similar a getvistaFromUrl. Implementado para evitar posibles errores en Front (Ej: Footer compartido)
 * @param {*} urlAccion 
 * @param {*} pBoton 
 * @param {*} pPanelID 
 * @param {*} pArg 
 */
const DeployActionInModalPanel = function (urlAccion, pBoton, pPanelID, pArg){  
	getVistaFromUrl(urlAccion, pPanelID, pArg);
}

/**
* 
* [getVistaFromUrl]
* Cargar una vista a partir de una petición que será devuelta por el Controller para ser mostrada en una sección específica (id) del Layout
* La idea es mostrar este contenido cuando se pulse una opción del menú del panel izquierdo de la Administración. El controlador devolverá
* la vista para que sea pintada en la sección central (pPanelID) a modo asíncrono y sin tener que esperar a que se cargue una página nueva. (Efecto React)
* Puede utilizarse para insertar la vista en un modal también. De ser así, simplementar el lanzador del método tendrá que tener lo siguiente:
    'data-toggle="modal" data-target="#modal-container"' siendo el data-target una vista de tipo "modal" vacía.
/**
 * 
 * @param pUrlAccion: URL de la petición que será pasada al controller para que este devuelva la vista correspondiente 
 * @param {String|jQueryObject} pPanelID: ID del panel donde se devolverá ese código HTML devuelto por la petición mandada en el parámetro urlAccion. Se puede pasar si se necesita un objeto jQuery
 * @param {object} pArg: Argumentos adicionales pasados mediante un objeto JS.
 * @param {function} completion: Función a ejecutar con parámetro de OK o KO para poder controlar acción en la llamada a esta función. Por defecto será vacío.
 * @param {Boolean} showViewWithEffect: Indicar si se desea que haya un efecto de fadeIn cuando se vaya a pintar la vista. Por defecto "No"
 * @param {Boolean} loadViewInPanelId: Indicar si se desea que la vista se cargue directamente en el panel proporcionado. Por defecto Sí.
 */

const getVistaFromUrl = function (pUrlAccion, pPanelID, pArg, completion = '', showViewWithEffect = false, loadViewInPanelId = true) {
	// Panel principal donde se mostrará el contenido o la vista a mostrar
	let panel = '';
	if (pPanelID instanceof jQuery){
		panel = pPanelID;
	}else{
		panel = $("#" + pPanelID);
	}
	

	// Comprobación / Asignación de parámetros adicionales
	let params = "";
	if (pArg != undefined || pArg != "") {
		params = pArg;
	} else {
		params = null;
	}

	// Realizar la petición AJAX al Controlador para obtener la vista
	GnossPeticionAjax(pUrlAccion, params, true)
		.done(function (data) {
			// OK
			// Cargar el html de la vista que será data en el panel del dom correspondiente
			if (loadViewInPanelId == true){
				if (showViewWithEffect){
					panel.hide().html(data).fadeIn();
				}else{
					panel.html(data);
				}
				// Recargar CKEditor si hubiera algún Input con clase de CKEditor en la vista devuelta desde "backend"
				if ($(panel).find(".cke").length > 0) ckEditorRecargarTodos();
				// Devolver información a completion para función llamadora			
				if (completion != '')
				{
					completion(requestFinishResult.ok);
				}				
			}else{
				// Devolver la vista como parámetro para hacer lo que se desee con ella.
				completion(requestFinishResult.ok, data);
			}
			
		})
		.fail(function (data) {
			// KO al obtener la vista del controlador
			if (data == "invitado") {
				operativaLoginEmergente.init();
			} else {
				// Mostrar el mensaje del error
				mostrarNotificacion("error", data);
			}
			if (completion != ''){
				completion(requestFinishResult.ko);
			}
			
		})
		.always(function (data) {
			// De momento se realizará desde la función llamadora (Ej: Ocultar loading)
		});
};

/**
 * [dismissVistaModal]
 * Acción para hacer desaparecer una ventana modal. Se puede llamar de forma individual o usar el método 'dismissVistaYMostrarMensajeInformativo'
 * @param {*} pJqueryModalView : Objeto jquery que corresponderá con el modal que se desea quitar. Por defecto será undefined y por lo tanto se cerrará el modal dinámico "#modal-container". 
 */
const dismissVistaModal = function (pJqueryModalView = undefined) {

	// Comprobar si se desea hacer desaparecer un modal en concreto. Si no, por defecto será el "modal-container"
	if (pJqueryModalView == undefined){
		pJqueryModalView = $("#modal-container");
	}else{
		// Quitar clase del body
		$("body").removeClass("modal-open");
		// Quitar estilos añadidos por modal al body
		$("body").removeAttr("style");
		// Eliminar clase div modal-backdrop
		$(".modal-backdrop").remove();

	}
	// 2 - Cerrar modal
	pJqueryModalView.modal('hide'); 
}

/**
 * [dismissVistaYMostrarMensajeInformativo]
 * Acción para hacer desaparecer una ventana modal. Es posible añadir un mensaje informativo si se desea que aparecerá pasados X milisegundos al usuario. 
 * @param {String} pNotificationType :  Tipo de mensaje a mostrar al usuario. Pueden ser de tipo "success", "error", "warning".
 * @param {String} pMessageNotification : Mensaje que se desea mostrar al usuario a modo de información de la acción realizada: Ej: "Datos guardados correctamente"
 * @param {jqueryObject} pJqueryModalView: Objeto jquery que corresponderá con el modal que se desea quitar. Por defecto será undefined y por lo tanto se cerrará el modal dinámico "#modal-container".
 * @param {Int} pInMiliSeconds: Tiempo que pasará hasta que la vista modal desaparecerá. Por defecto, 1,5 segundos
 */
 const dismissVistaYMostrarMensajeInformativo = function (pNotificationType, pMessageNotification = undefined, pJqueryModalView = undefined, pInMiliSeconds = 1500) {

	// 1- Hacer desaparecer la vista modal
	dismissModalView(pJqueryModalView);
	
	// 2 - Mostrar mensaje
	if (pMessageNotification != undefined){		
		setTimeout(() => {
			mostrarNotificacion(pNotificationType, pMessageNotification);
		}, pInMiliSeconds)
	}
}

//****************************************************************************************************************/
// COMPROBACIÓN DE CAMPOS / INPUTS / ERRORES
/*****************************************************************************************************************/

/**
 * [eliminarErroresEnInputs]
 * Método para eliminar los mensajes de error de los inputs. Estos mensajes de error vienen definidos por clases de bootstrap (is-invalid y invalid-feedback).
 * Recorrerá todos los inputs y los hará desaparecer para mostrar información al usuario.
 */
const eliminarErroresEnInputs = function(){
	// Eliminación de las clase "in-invalid"
	$(".is-invalid").each(function(){
	    const element = $(this);
	    element.removeClass("is-invalid");
	});	
	// Eliminación del html referente al error           
	$(".invalid-feedback").each(function(){
	    const element = $(this);
	    element.remove();
	});
}



/**
 * [comprobarInputNoVacio]
 * Comprobar que el input pasado no está vacío. Si está vacío, se añadirá estilo para que el usuario pueda visualizar el "error".
 * Se recomienda que la estructura sea: 
 * 	- formGroup - Contenedor del input a validar
 * 	- label - Nombre del campo a rellenar
 * 	- input - Input a validar
 *  - small - Ayuda contextual del input (Opcional) 
 * @param  {jqueryObject} $pJqueryInput: Input en formato jquery del que se desea realizar la comprobación de si está o no vacío.
 * @param  {bool} pShowMensajeError: Indicar si se desea que si se produce un error, se desea mostrar el mensaje de error debajo del input.
 * @param {bool} pShowMensajeOk : Indicar si se desea remarcar el input como ok Por defecto será true.
 * @param  {string} pMensajeError: Mensaje del error a mostrar cuando no se escriba ningún valor en el input.
 * @param  {Int} pNumMinCaracteres: Nº de caracteres mínimo que debe de tener el input. Por defecto 1, es decir, no vacío.
 * @param  {Int} pNumMaxCaracteres: Nº de caracteres máximo que puede tener el input. Por defecto 9999, es decir, no hay límite máximo de caracteres.
 * @returns Devuelve true si hay algún error o false si no el input no está vacío
 */
const comprobarInputNoVacio = function($pJqueryInput, pShowMensajeError = false, pShowMensajeOk = true, pMensajeError = undefined, pNumMinCaracteres = 1, pNumMaxCaracteres = 9999){
	// Mensaje del error que se mostrará en pantalla		
	let error = '';	
	// Eliminar posible mensaje previo	
	$pJqueryInput.siblings(".invalid-feedback").remove();
		
	// Comprobar el numero de caracteres del campo
	const numCaracteres = $pJqueryInput.val().length;
	if ((numCaracteres <= pNumMinCaracteres)) {
		error = pMensajeError == undefined ? 'El texto no puede ser vacío' : pMensajeError;		
	}else if (numCaracteres > pNumMaxCaracteres) {
		error = pMensajeError == undefined ? 'Se ha excedido el número máximo de caracteres' : pMensajeError;	
	}		

	/* Si se ha producido un error, mostrarlo en el input para visualización del usuario */
	if (error.length > 0){
		$pJqueryInput.addClass('is-invalid');
		// Quitar como correcto el input si así se desea
		pShowMensajeOk && $pJqueryInput.removeClass('is-valid');
		// Mostrar el mensaje del error a modo informativo si se desea
		if (pShowMensajeError){
			const invalidFeedback = `<div class="invalid-feedback">${error}</div>`;						
			$pJqueryInput.after(invalidFeedback).hide().fadeIn();
		}
	}else{
		$pJqueryInput.removeClass('is-invalid');
		// Marcar como correcto el input si así se desea
		pShowMensajeOk && $pJqueryInput.addClass('is-valid');
	}

	// Enviar true o false si hay error
	if (error.length > 0){
		return true;
	}
	// No hay error
	return false;
}


/**
 * [comprobarInputConValidacion]
 * Comprobar que el input pasado no está vacío. Si está vacío, se añadirá estilo para que el usuario pueda visualizar el "error".
 * Se recomienda que la estructura sea: 
 * 	- formGroup - Contenedor del input a validar
 * 	- label - Nombre del campo a rellenar
 * 	- input - Input a validar
 *  - small - Ayuda contextual del input (Opcional) 
 * @param  {jqueryObject} $pJqueryInput: Input en formato jquery del que se desea realizar la comprobación de si está o no vacío.
 * @param  {bool} pShowMensajeError: Indicar si se desea que si se produce un error, se desea mostrar el mensaje de error debajo del input.
 * @param {bool} pShowMensajeOk : Indicar si se desea remarcar el input como ok Por defecto será true.
 * @param  {string} pMensajeError: Mensaje del error a mostrar cuando no se escriba ningún valor en el input.
 * @param  {Int} pNumMinCaracteres: Nº de caracteres mínimo que debe de tener el input. Por defecto 1, es decir, no vacío.
 * @param  {Int} pNumMaxCaracteres: Nº de caracteres máximo que puede tener el input. Por defecto 9999, es decir, no hay límite máximo de caracteres.
 */
 const comprobarInputConValidacion = function($pJqueryInput, pMetodoValidacion, pShowMensajeError = false, pShowMensajeOk = true, pMensajeError = undefined){
	// Mensaje del error que se mostrará en pantalla		
	let error = '';	
	let isOk = false;
	// Eliminar posible mensaje previo	
	$pJqueryInput.siblings(".invalid-feedback").remove();
		
	// pMetodoValidacion es una función pasada como parámetro en el momento de la llamada de 'comprobarInputConValidacion'
	error = pMetodoValidacion($pJqueryInput);	

	/* Si se ha producido un error, mostrarlo en el input para visualización del usuario */
	if (error.length > 0){
		$pJqueryInput.addClass('is-invalid');
		// Quitar como correcto el input si así se desea
		pShowMensajeOk && $pJqueryInput.removeClass('is-valid');
		// Mostrar el mensaje del error a modo informativo si se desea
		if (pShowMensajeError){
			const invalidFeedback = `<div class="invalid-feedback">${error}</div>`;						
			$pJqueryInput.after(invalidFeedback).hide().fadeIn();
		}
	}else{
		$pJqueryInput.removeClass('is-invalid');
		// Marcar como correcto el input si así se desea
		pShowMensajeOk && $pJqueryInput.addClass('is-valid');
	}

	if (error.length == 0){
		isOk = true;
	}
	return isOk;
}


/**
 * Método para mostrar u ocultar el input con error al estilo de Bootstrap
 * @param {*} pInput Input donde se colocará el error
 * @param {*} pShowError Valor booleano que indica si se desea o no mostrar el error
 * @param {*} pErrorMessage Mensaje del error a mostrar
 */
const displayInputWithErrors = function(pInput, pShowError, pErrorMessage){
	
	if (pShowError && !pInput.hasClass("is-invalid")){
		pInput.addClass('is-invalid');		
		// Mostrar el mensaje del error a modo informativo si se desea		
		const invalidFeedback = `<div class="invalid-feedback">${pErrorMessage}</div>`;						
		pInput.after(invalidFeedback).hide().fadeIn();				
	}
	else if (!pShowError){
		pInput.removeClass('is-invalid');
		const invalidFeedback = pInput.parent().find(".invalid-feedback");	
		// Mostrar el mensaje del error a modo informativo si se desea										
		invalidFeedback.remove();
	}
}

/**
 * Método para quitar el posible mensaje de error de un input. Es posible que el input tenga la propiedad "is-invalid"
 * y que a su vez tenga un posible mensaje de error asociado al input (invalid-feedback)
 * @param {*} input 
 */
const hideInputsWithErrors = function(input){

	// Eliminar "is-invalid" del input con error
	input.removeClass("is-invalid");
	// Eliminar posible mensaje adicional mostrado del error
	const errorMessage = input.siblings(".invalid-feedback");
	errorMessage.length > 0 && errorMessage.remove();
}


//****************************************************************************************************************/
// MENSAJES OK / MENSAJES KO // LOADING
/*****************************************************************************************************************/

/**
 * [mostrarNotificacion]
 * Mostrar mensajes informativos al usuario de tipo "success", "error", "warning".
 * Requiere librería toastr y librería de iconos Material Icons.
 * Ejemplo de uso: mostrarNotificacion('success', "Se ha desactivado correctamente el registro automático.");
 *
 * @param  {string} pTipo: Puede ser 'info', 'success', 'warning', 'error'. Será el tipo de mensaje a mostrar. Dependiendo del tipo, se mostrarán los mensajes de colores diferentes.
 * @param  {string} pContenido: Mensaje que se desea mostrar.
 */
const mostrarNotificacion = function (pTipo, pContenido) {
	toastr[pTipo](pContenido, "", {
		toastClass: "toast themed",
		positionClass: "toast-bottom-center",
		target: "body",
		closeHtml: '<span class="material-icons">close</span>',
		showMethod: "slideDown",
		timeOut: 5000,
		escapeHtml: false,
		closeButton: true,
	});
};

/**
 * [loadingMostrar]
 * Muestra una máscara de loading ya sea en pantalla completa o dentro de un elemento html deseado.
 * Requiere librería busy-load.
 * @param  {jQueryObject} pJqueryHtmlElemento: Elemento jQuery - Html. Por defecto el loading se mostrará en toda la pantalla.
 * Si se desea mostrar el loading dentro de un elemento html (Ej: div), proporcionar en pJqueryHtmlElemento el elemento html.
 * Ejemplo de uso
 * https://github.com/piccard21/busy-load
 */
const loadingMostrar = function (pJqueryHtmlElemento = undefined) {
	// Mostrar/ocultar loading por defecto en fullScreen
	let isFullScreenElement = true;

	if (pJqueryHtmlElemento != undefined) {
		isFullScreenElement = false;
	}

	// Custom spinner
	const customSpinner = `
		<div class="spinner-border" style="color:#006eff" role="status">
			<span class="sr-only">Loading ...</span>
		</div>
	`;

	// Configuración de parámetros para mostrar el efecto de loading
	const busyParams = {
		// spinner: "accordion", -> Por defecto será un loading normal		
		background: "rgba(255, 255, 255, 0.8)", // --> Color del fondo del loading
		color: "#006eff", // --> Color del loading
		animation: "fade", // -> Animación de aparición del Loading
		containerClass: "mascara-loading", // -> Nombre de la clase para el contenedor del loading
		containerItemClass: "mi-item-contenedor-mascara-loading",
		custom: $(customSpinner), // --> Custom spinner de bootstrap
	};

	// Mostrar loading
	if (!isFullScreenElement) {
		// Mostrar loading dentro del elemento jquery
		// Loading con spinner por defecto
		pJqueryHtmlElemento.busyLoad("show", busyParams);
	} else {
		// Mostrar loading de pantalla completa
		// Loading con spinner por defecto
		$.busyLoadFull("show", busyParams);		
	}
};

/**
 * [MostrarUpdateProgress]
 * Mostrar loading. Este método se usaba en el anterior Front/BackEnd. Sobreescribirlo para que llame directamente a loadingMostrar y evitar errores JS
 */
const MostrarUpdateProgress = function(){
	loadingMostrar();
};

/**
 * [loadingOcultar]
 * Requiere librería busy-load.
 * Ocultar cualquier loading que se muestre en pantalla (independientemente de que sea fullScreen o no )
 * Ejemplo de uso
 * https://github.com/piccard21/busy-load
 */
const loadingOcultar = function () {
	// Configuración de parámetros para mostrar el efecto de loading
	const busyParams = {
		animation: "fade", // -> Animación de desaparición del Loading
	};

	// Identificador de la clase "loading"
	const mascaraLoadingClass = "mascara-loading";
	const mascara = $(`.${mascaraLoadingClass}`);

	const loadingParent = mascara.parent();
	if (loadingParent.length > 0){
		// Permitir hacer scroll (Añadía la librería un no-scroll)
		loadingParent.removeClass("no-scroll");
		loadingParent.busyLoad("hide", busyParams);
	}else{		
		// El loading está dentro de un elemento del dom
		const loadingContainer = $('.busy-load-active');
		loadingContainer.busyLoad("hide", busyParams);
	}
};

/**
 * [OcultarUpdateProgress]
 * Ocultar loading. Este método se usaba en el anterior Front/BackEnd. Sobreescribirlo para que llame directamente a loadingOcultar y evitar errores JS
 */
const OcultarUpdateProgress = function(){
	loadingOcultar();
};


/**
 * Método para mostrar un "overlay" para dejar inhabilitada el contenedor donde se está superponiendo
 * @param {String} idClassContainerToCover Identificador o clase del contenedor donde se desea añadir el div overlay
 */
const overlayShowInContainer = function(idClassContainerToCover){

	// Variable que contendá el elemento del dom donde se añadirá el overlay
	let containerToCover = undefined;
	
	if ($(`#${idClassContainerToCover}`).length > 0){
		containerToCover = $(`#${idClassContainerToCover}`);
	}else if ($(`.${idClassContainerToCover}`).length > 0) {
		containerToCover = $(`.${idClassContainerToCover}`);
	}else{
		return
	}

	// Si hay múltiples items, escoger sólo uno
	containerToCover = containerToCover.length > 1 ? containerToCover[0] : containerToCover;
	const overlayWidth = containerToCover.width();
	const overlayHeight = containerToCover.height();

	const overlayTemplate = `
		<div id="overlayTemplate" style="
			position: absolute;			
			width: ${overlayWidth}px;
			background-color: rgba(255, 255, 255, 0.8);
			height: ${overlayHeight}px;
			z-index: 9999;">		
		</div>	
	`;
	containerToCover.prepend(overlayTemplate);
};

/**
 * Método para eliminar un "overlay" que ha sido previamente mostrado para ocultar una sección de una página
 */
 const overlayDestroy = function(){
	setTimeout(function() {                                        
		$(`#overlayTemplate`).fadeOut("normal", function() {
			$(this).remove();
		});		
	},500);	
};





//****************************************************************************************************************/
// BUSQUEDAS
/*****************************************************************************************************************/

/**
 * [filtrarItems]
 * Función para filtrar o dejar sólo visibles los items en base a la búsqueda introducida
 * @param {any} pTxt: Input text en formato jquery (objeto jQuery) donde se introduce la cadena a buscar
 * @param {any} pPanelId: Id del panel o sección donde se han de buscar los items
 * @param {any} pClassItem: Nombre de la clase del item que será ocultado o mostrado según la búsqueda
 * @param {any} pClassItemWhereToFind: Nombre de la clase donde se ha de buscar lo introducido en el txt
 */

const filtrarItems = function (
	pTxt,
	pPanelId,
	pClassItem,
	pClassItemWhereToFind
) {
	// Cadena de texto a buscar (minúsculas)
	var cadena = $(pTxt).val().toLowerCase();
	// Eliminamos posibles tildes para búsqueda ampliada
	cadena = cadena.normalize("NFD").replace(/[\u0300-\u036f]/g, "");
	// Items a buscar
	var itemsListado = $("#" + pPanelId).find(`.${pClassItem}`);

	itemsListado.each(function () {
		// Botón de plegar/desplegar hijos
		const boton = $(this).find('.boton-desplegar');		
		// Texto de cada item a buscar para poder comparar
		var nombre = $(this).find(`.${pClassItemWhereToFind}`).text().toLowerCase();
		// Eliminamos posibles tildes para búsqueda ampliada
		nombre = nombre.normalize("NFD").replace(/[\u0300-\u036f]/g, "");
		// Filtrar -> Ocultar/Mostrar
		if (nombre.indexOf(cadena) < 0) {
			$(this).hide();
			boton.removeClass('mostrar-hijos');
		} else {
			boton.addClass('mostrar-hijos');
			$(this).show();
		}
	});
};

/**
 * [filtrarListaItemsConHijos] - Revisar --> Bastará con el método filtrarItems()
 * Función para filtrar o dejar sólo visibles los items en base a la búsqueda introducida de una lista o árbol que pueda tener hijos a su vez
 * @param {any} pTxt: Será el input text en formato jquery (objeto jQuery) donde se introduce la cadena para buscar
 * @param {any} pIdPanelListadoItems: Id del panel o sección donde se han de buscar los items
 * @param {any} pClaseItemConPosiblesHijos: Clase de cada item que a su vez podrá tener hijos (items) (Ej: .divTesArbol .categoria-wrap)
 * @param {any} pClaseBotonDesplegarResultados: Clase que corresponderá con el botón para plegar o desplegar items hijos (Ej: boton-desplegar)
 * @param {any} pClaseEstiloBotonMostrarHijos: Estilo de clase que indicará la posibilidad de mostrar hijos de un padre (Ej: mostrar-hijos)
 * @param {any} pClaseTextoLabel: Nombre de la clase donde estará el texto tanto del padre como del item hijo (Ej: "categoria label")
 * @param {any} pClaseNombreDelPanelHijos: Nombre de la clase o panel donde estarán los hijos de un item padre (Ej: panHijos)
 * @param {any} pClaseNombreItemHijo: Nombre de la clase donde estará el nombre sección del hijo de un padre item de la lista (Ej: categoria-wrap)
 */

const filtrarListaItemsConHijos = function (
	pTxt,
	pIdPanelListadoItems,
	pClaseItemConPosiblesHijos,
	pClaseBotonDesplegarResultados,
	pClaseEstiloBotonMostrarHijos = "mostrar-hijos",
	pClaseTextoLabel,
	pClaseNombreDelPanelHijos,
	pClaseNombreItemHijo
) {
	let cadena = $(pTxt).val();
	// Eliminamos posibles tildes para búsqueda ampliada
	cadena = cadena.normalize("NFD").replace(/[\u0300-\u036f]/g, "");
	const itemsListado = $("#" + pIdPanelListadoItems).find(
		`.${pClaseItemConPosiblesHijos}`
	);
	// Por defecto los inicializa para que estén visibles
	itemsListado.show();

	itemsListado.each(function () {
		var boton = $(this).find(`.${pClaseBotonDesplegarResultados}`);
		boton.removeClass(`${pClaseEstiloBotonMostrarHijos}`);
		var nombreCat = $(this).find(`.${pClaseTextoLabel}`).text();
		// Eliminamos posibles tildes para búsqueda ampliada
		nombreCat = nombreCat.normalize("NFD").replace(/[\u0300-\u036f]/g, "");
		boton.trigger("click");
		if (nombreCat.toLowerCase().indexOf(cadena.toLowerCase()) < 0) {
			$(this).hide();
		}

		var categoriaHijo = $(this)
			.find(`.${pClaseNombreDelPanelHijos}`)
			.children(`.${pClaseNombreItemHijo}`);
		categoriaHijo.each(function () {
			var nombreCatHijo = $(this)
				.find(`.${pClaseTextoLabel}`)
				.text()
				.normalize("NFD")
				.replace(/[\u0300-\u036f]/g, "");
			if (nombreCatHijo.toLowerCase().indexOf(cadena.toLowerCase()) < 0) {
				$(this).hide();
			}
		});
	});
};

//****************************************************************************************************************/
// CKEditor. Métodos últiles compartidos para manejo de los ckEditor
/*****************************************************************************************************************/

/**
 * [ckEditorRecargarTodos]
 * Deshabilitar y volver a cargar todas las instancias del ckEditor de la web actual. Se utiliza para poder recargar inputs o textAreas de nueva creación
 * para que aparezca el editor ckEditor.
 */
const ckEditorRecargarTodos = function () {	
	const textAreas = $("textarea.cke");

	ckEditorDestruirTodos();

	if (textAreas.length > 0) {
		var urlbase = $("input.inpt_baseURL").val();

		if (document.URL.indexOf("https://") == 0) {
			if (urlbase.indexOf("https://") == -1) {
				urlbase = urlbase.replace("http", "https");
			}
		}
		var BasePath = CKEDITOR.basePath;
		var ImageBrowseUrl = urlbase + "/conector-ckeditor?v=0";
		var ImageUploadUrl = urlbase + "/conector-ckeditor?v=0";
		//var ImageBrowseUrl = urlbase + "/ConectorCKEditor.aspx";
		//var ImageUploadUrl = urlbase + "/ConectorCKEditor.aspx";
		//var ImageBrowseUrl = BasePath + "filemanager/browser/default/browser.html?Type=Image&Connector=" + BasePath + "filemanager/connectors/aspx/connector.aspx";
		//var ImageUploadUrl = BasePath + "filemanager/connectors/aspx/upload.aspx?Type=Image";
		var lang = $("#inpt_Idioma").val();

		$("textarea.cke.mensajes").ckeditor(function () {}, {
			language: lang,
			toolbar: "Gnoss-Mensajes",
			extraPlugins: '',
		});
		$("textarea.cke.recursos").ckeditor(function () {}, {
			language: lang,
			toolbar: "Gnoss-Recursos",
			extraPlugins: ['image2', 'codemirror'],
			filebrowserImageBrowseUrl: ImageBrowseUrl,
			filebrowserImageUploadUrl: ImageUploadUrl,	
		});
		$("textarea.cke.blogs").ckeditor(function () {}, {
			language: lang,
			extraPlugins: 'codemirror',
			toolbar: "Gnoss-Blogs",
		});
		$("textarea.cke.comentarios").ckeditor(function () {}, {
			language: lang,
			extraPlugins: '',
			toolbar: "Gnoss-Comentario",
		});

		$("textarea.cke.editorHtml").ckeditor(function () {}, {
			language: lang,
			toolbar: "Gnoss-Html",
			extraPlugins: ['image2', 'codemirror'],
			filebrowserImageBrowseUrl: ImageBrowseUrl,
			filebrowserImageUploadUrl: ImageUploadUrl,			
		});

		$("textarea.cke.toolBar_simple").ckeditor(function () {}, {
			language: lang,
			extraPlugins: '',
			toolbar: "Gnoss-Base",		
		});				

	}
};

/**
 * [ckEditorDestruirTodos]
 * Recorrer todas las instancias que hay en pantalla de ckEditor y los destruye. La idea es llamar al método ckEditorRecargarTodos para volver a inicializarlos.
 */
const ckEditorDestruirTodos = function () {
	let textAreas = $("textarea.cke"),
		instanceName = "",
		i = 0;

	for (i = 0; i < textAreas.length; i++) {
		instanceName = textAreas[i].id;

		if (CKEDITOR.instances[instanceName] != null) {
			try {
				CKEDITOR.instances[instanceName].destroy();
			} catch (error) {}
		}

		// Si hay ckEditors pero sin Id, destruirlos también
		// enrique
		if (!isObjectEmpty(CKEDITOR.instances) && instanceName == ""){
			for (var propiedad in CKEDITOR.instances) {
				if (CKEDITOR.instances.hasOwnProperty(propiedad)) {
					try{
						// Eliminar sólo si existe ya una configuración asociada
						if (!isObjectEmpty(CKEDITOR.instances[propiedad].config)){
							CKEDITOR.instances[propiedad].destroy();
						}						
					}catch(error){}				  
				}
			}
		}
		
	}
};


/**
 * Método para comprobar el valor correcto. Se ejecuta en el onBlur del input. Se utiliza en la operativa operativaGestionObjetosConocimientoOntologias. Al aplicarse en "onBlur" es necesario engancharla desde aquí
 * para que el método sea accesible.
 * 
 */
const ComprobarValorCampoCorrecto = function(pEntidad, pPropiedad, pValor, pTxtIDs, pTxtCaract, input){
	// Obtener la fila del elemento 
	const pRowElementoEntidadSecundaria = $(input).closest(`.${operativaGestionObjetosConocimientoOntologias.elementRowClassName}`);
	// Acceder a los mTxtRegistroIDs y a las mTxtCaracteristicasElem
	const $txtIds = pRowElementoEntidadSecundaria.find(`#${pTxtIDs}`);
	const $txtCaracteristicasElem = pRowElementoEntidadSecundaria.find(`#${pTxtCaract}`);
	// Guardar referencia de inputs necesarios para comprobación
	operativaGestionObjetosConocimientoOntologias.txtValorRdf = pRowElementoEntidadSecundaria.find("#mTxtValorRdf");
	operativaGestionObjetosConocimientoOntologias.txtRegistroIDs = $txtIds;
	operativaGestionObjetosConocimientoOntologias.txtCaracteristicasElem = $txtCaracteristicasElem;
	operativaGestionObjetosConocimientoOntologias.txtElemEditados = pRowElementoEntidadSecundaria.find("#mTxtElemEditados");

	// Comprobar valor correcto en "onBlur"
	operativaGestionObjetosConocimientoOntologias.ComprobarValorCampoCorrectoInt(pEntidad, pPropiedad, pValor, $txtIds, $txtCaracteristicasElem, true);
};

/**
 * Método para comprobar el valor correcto. Se ejecuta en el onBlur del input. Se utiliza en la operativa operativaGestionObjetosConocimientoOntologias. Al aplicarse en "onBlur" es necesario engancharla desde aquí
 * para que el método sea accesible.
 * 
 */
const ComprobarValorCampoCorrectoInt = function(pEntidad, pPropiedad, pValor, pTxtIDs, pTxtCaract, pCompValVacio, input){
	// Obtener la fila del elemento 
	const pRowElementoEntidadSecundaria = $(input).closest(`.${operativaGestionObjetosConocimientoOntologias.elementRowClassName}`);
	// Acceder a los mTxtRegistroIDs y a las mTxtCaracteristicasElem
	const $txtIds = pRowElementoEntidadSecundaria.find(`#${pTxtIDs}`);
	const $txtCaracteristicasElem = pRowElementoEntidadSecundaria.find(`#${pTxtCaract}`);
	// Guardar referencia de inputs necesarios para comprobación
	operativaGestionObjetosConocimientoOntologias.txtValorRdf = pRowElementoEntidadSecundaria.find("#mTxtValorRdf");
	operativaGestionObjetosConocimientoOntologias.txtRegistroIDs = $txtIds;
	operativaGestionObjetosConocimientoOntologias.txtCaracteristicasElem = $txtCaracteristicasElem;
	operativaGestionObjetosConocimientoOntologias.txtElemEditados = pRowElementoEntidadSecundaria.find("#mTxtElemEditados");

	// Comprobar valor correcto en "onBlur"
	operativaGestionObjetosConocimientoOntologias.ComprobarValorCampoCorrectoInt(pEntidad, pPropiedad, pValor, $txtIds, $txtCaracteristicasElem, true);
};

/**
 * Método para comprobar el valor correcto. Se ejecuta en el onBlur del input. Se utiliza en la operativa operativaGestionObjetosConocimientoOntologias. Al aplicarse en "onBlur" es necesario engancharla desde aquí
 * para que el método sea accesible.
 * 
 */
const SeleccionarIdioma = function(pLink, pEntidad, pPropiedad, pIdioma, pMultiple){	
	// Obtener la fila del elemento 
	const pRowElementoEntidadSecundaria = $(pLink).closest(`.${operativaGestionObjetosConocimientoOntologias.elementRowClassName}`);

	// Keys para acceder a pTxtIDs y pTxtCaract de forma manual
	const pTxtIDs = "mTxtRegistroIDs";
	const pTxtCaract = "mTxtCaracteristicasElem";

	// Acceder a los mTxtRegistroIDs y a las mTxtCaracteristicasElem
	const $txtIds = pRowElementoEntidadSecundaria.find(`#${pTxtIDs}`);
	const $txtCaracteristicasElem = pRowElementoEntidadSecundaria.find(`#${pTxtCaract}`);
	// Guardar referencia de inputs necesarios para comprobación
	operativaGestionObjetosConocimientoOntologias.txtValorRdf = pRowElementoEntidadSecundaria.find("#mTxtValorRdf");
	operativaGestionObjetosConocimientoOntologias.txtRegistroIDs = $txtIds;
	operativaGestionObjetosConocimientoOntologias.txtCaracteristicasElem = $txtCaracteristicasElem;
	operativaGestionObjetosConocimientoOntologias.txtElemEditados = pRowElementoEntidadSecundaria.find("#mTxtElemEditados");
	// Variables o IDs manuales para inputs ocultos utilizados en JS de elementos secundarios
	operativaGestionObjetosConocimientoOntologias.openSeaDragonInfoSem = "";
	operativaGestionObjetosConocimientoOntologias.txtValoresGrafoDependientes = "txtValoresGrafoDependientes";
	
	// SeleccionarIdioma al hacer click en el Tab del elemento secundario de ontología
	operativaGestionObjetosConocimientoOntologias.SeleccionarIdioma(pLink, pEntidad, pPropiedad, pIdioma, pMultiple);
};

/**
 * Método para añadir una propiedad al pulsar sobre "Añadir" editando una propiedad de una instancia secundaria
 * 
 */
const AgregarValorADataNoFuncionalProp = function(pButton, pEntidad, pPropiedad, pControlContValores, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados){	
	// Obtener la fila del elemento 
	const pRowElementoEntidadSecundaria = $(pButton).closest(`.${operativaGestionObjetosConocimientoOntologias.elementRowClassName}`);

	// Acceder a los mTxtRegistroIDs y a las mTxtCaracteristicasElem
	const $txtIds = pRowElementoEntidadSecundaria.find(`#${pTxtIDs}`);
	const $txtCaracteristicasElem = pRowElementoEntidadSecundaria.find(`#${pTxtCaract}`);
	// Guardar referencia de inputs necesarios para comprobación
	operativaGestionObjetosConocimientoOntologias.txtValorRdf = pRowElementoEntidadSecundaria.find("#mTxtValorRdf");
	operativaGestionObjetosConocimientoOntologias.txtRegistroIDs = $txtIds;
	operativaGestionObjetosConocimientoOntologias.txtCaracteristicasElem = $txtCaracteristicasElem;
	operativaGestionObjetosConocimientoOntologias.txtElemEditados = pRowElementoEntidadSecundaria.find("#mTxtElemEditados");	
	operativaGestionObjetosConocimientoOntologias.txtValores = pRowElementoEntidadSecundaria.find(`#${pTxtValores}`)// pTxtValores;
	
	// Variables o IDs manuales para inputs ocultos utilizados en JS de elementos secundarios
	operativaGestionObjetosConocimientoOntologias.openSeaDragonInfoSem = "";
	operativaGestionObjetosConocimientoOntologias.txtValoresGrafoDependientes = "txtValoresGrafoDependientes";
	
	// Iniciar el agregar valor	
	operativaGestionObjetosConocimientoOntologias.AgregarValorADataNoFuncionalProp(pEntidad, pPropiedad, pControlContValores, operativaGestionObjetosConocimientoOntologias.txtValores, operativaGestionObjetosConocimientoOntologias.txtRegistroIDs, $txtCaracteristicasElem, operativaGestionObjetosConocimientoOntologias.txtElemEditados);
};

const GuardarValorADataNoFuncionalProp = function(pButton, pEntidad, pPropiedad, pControlContValores, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados){	
	// Obtener la fila del elemento 
	const pRowElementoEntidadSecundaria = $(pButton).closest(`.${operativaGestionObjetosConocimientoOntologias.elementRowClassName}`);

	// Acceder a los mTxtRegistroIDs y a las mTxtCaracteristicasElem
	const $txtIds = pRowElementoEntidadSecundaria.find(`#${pTxtIDs}`);
	const $txtCaracteristicasElem = pRowElementoEntidadSecundaria.find(`#${pTxtCaract}`);
	// Guardar referencia de inputs necesarios para comprobación
	operativaGestionObjetosConocimientoOntologias.txtValorRdf = pRowElementoEntidadSecundaria.find("#mTxtValorRdf");
	operativaGestionObjetosConocimientoOntologias.txtRegistroIDs = $txtIds;
	operativaGestionObjetosConocimientoOntologias.txtCaracteristicasElem = $txtCaracteristicasElem;
	operativaGestionObjetosConocimientoOntologias.txtElemEditados = pRowElementoEntidadSecundaria.find("#mTxtElemEditados");	
	operativaGestionObjetosConocimientoOntologias.txtValores = pRowElementoEntidadSecundaria.find(`#${pTxtValores}`)// pTxtValores;
	
	// Variables o IDs manuales para inputs ocultos utilizados en JS de elementos secundarios
	operativaGestionObjetosConocimientoOntologias.openSeaDragonInfoSem = "";
	operativaGestionObjetosConocimientoOntologias.txtValoresGrafoDependientes = "txtValoresGrafoDependientes";
	
	// Iniciar el agregar valor	
	operativaGestionObjetosConocimientoOntologias.GuardarValorADataNoFuncionalProp(pEntidad, pPropiedad, pControlContValores, operativaGestionObjetosConocimientoOntologias.txtValores, operativaGestionObjetosConocimientoOntologias.txtRegistroIDs, $txtCaracteristicasElem, operativaGestionObjetosConocimientoOntologias.txtElemEditados);
};

/**
 * Método para agregar un objeto no funcional de una instancia de un objeto de conocimiento secundario.
 * @param {*} pEntidad 
 * @param {*} pPropiedad 
 * @param {*} pEntidadHija 
 * @param {*} pControlContValores 
 * @param {*} pTxtValores 
 * @param {*} pTxtIDs 
 * @param {*} pTxtCaract 
 * @param {*} pTxtElemEditados 
 * @param {*} pVisibleContPaneles 
 * @param {*} pRaiz 
 * @param {*} pAntiguoNumElem 
 */
const AgregarObjectNoFuncionalProp = function(pButton, pEntidad, pPropiedad, pEntidadHija, pControlContValores, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados, pVisibleContPaneles, pRaiz, pAntiguoNumElem){

	// Obtener la fila del elemento 
	const pRowElementoEntidadSecundaria = $(pButton).closest(`.${operativaGestionObjetosConocimientoOntologias.elementRowClassName}`);

	// Acceder a los mTxtRegistroIDs y a las mTxtCaracteristicasElem
	const $txtIds = pRowElementoEntidadSecundaria.find(`#${pTxtIDs}`);
	const $txtCaracteristicasElem = pRowElementoEntidadSecundaria.find(`#${pTxtCaract}`);
	// Guardar referencia de inputs necesarios para comprobación
	operativaGestionObjetosConocimientoOntologias.txtValorRdf = pRowElementoEntidadSecundaria.find("#mTxtValorRdf");
	operativaGestionObjetosConocimientoOntologias.txtRegistroIDs = $txtIds;
	operativaGestionObjetosConocimientoOntologias.txtCaracteristicasElem = $txtCaracteristicasElem;
	operativaGestionObjetosConocimientoOntologias.txtElemEditados = pRowElementoEntidadSecundaria.find("#mTxtElemEditados");	
	operativaGestionObjetosConocimientoOntologias.txtValores = pRowElementoEntidadSecundaria.find(`#${pTxtValores}`)// pTxtValores;
	operativaGestionObjetosConocimientoOntologias.txtNombreCatTesSem = pRowElementoEntidadSecundaria.find(`#mTxtNombreCatTesSem`);

	// operativaGestionObjetosConocimientoOntologias.AgregarObjectNoFuncionalProp(pEntidad, pPropiedad, pControlContValores, operativaGestionObjetosConocimientoOntologias.txtValores, operativaGestionObjetosConocimientoOntologias.txtRegistroIDs, $txtCaracteristicasElem, operativaGestionObjetosConocimientoOntologias.txtElemEditados);
	operativaGestionObjetosConocimientoOntologias.AgregarObjectNoFuncionalProp(pEntidad, pPropiedad, pEntidadHija, pControlContValores, operativaGestionObjetosConocimientoOntologias.txtValores, operativaGestionObjetosConocimientoOntologias.txtRegistroIDs, $txtCaracteristicasElem, operativaGestionObjetosConocimientoOntologias.txtElemEditados, pVisibleContPaneles, pRaiz, pAntiguoNumElem);	
};

const GuardarObjectNoFuncionalProp = function(pButton, pEntidad, pPropiedad, pEntidadHija, pControlContValores, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados){

	// Obtener la fila del elemento 
	const pRowElementoEntidadSecundaria = $(pButton).closest(`.${operativaGestionObjetosConocimientoOntologias.elementRowClassName}`);

	// Acceder a los mTxtRegistroIDs y a las mTxtCaracteristicasElem
	const $txtIds = pRowElementoEntidadSecundaria.find(`#${pTxtIDs}`);
	const $txtCaracteristicasElem = pRowElementoEntidadSecundaria.find(`#${pTxtCaract}`);
	// Guardar referencia de inputs necesarios para comprobación
	operativaGestionObjetosConocimientoOntologias.txtValorRdf = pRowElementoEntidadSecundaria.find("#mTxtValorRdf");
	operativaGestionObjetosConocimientoOntologias.txtRegistroIDs = $txtIds;
	operativaGestionObjetosConocimientoOntologias.txtCaracteristicasElem = $txtCaracteristicasElem;
	operativaGestionObjetosConocimientoOntologias.txtElemEditados = pRowElementoEntidadSecundaria.find("#mTxtElemEditados");	
	operativaGestionObjetosConocimientoOntologias.txtValores = pRowElementoEntidadSecundaria.find(`#${pTxtValores}`)// pTxtValores;
	operativaGestionObjetosConocimientoOntologias.txtNombreCatTesSem = pRowElementoEntidadSecundaria.find(`#mTxtNombreCatTesSem`);

	operativaGestionObjetosConocimientoOntologias.GuardarObjectNoFuncionalProp(pEntidad, pPropiedad, pEntidadHija, pControlContValores, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados);
};

const LimpiarControlesEntidad = function(pButton, pEntidadHija, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados){
	
	// Obtener la fila del elemento 
	const pRowElementoEntidadSecundaria = $(pButton).closest(`.${operativaGestionObjetosConocimientoOntologias.elementRowClassName}`);

	// Acceder a los mTxtRegistroIDs y a las mTxtCaracteristicasElem
	const $txtIds = pRowElementoEntidadSecundaria.find(`#${pTxtIDs}`);
	const $txtCaracteristicasElem = pRowElementoEntidadSecundaria.find(`#${pTxtCaract}`);
	// Guardar referencia de inputs necesarios para comprobación
	operativaGestionObjetosConocimientoOntologias.txtValorRdf = pRowElementoEntidadSecundaria.find("#mTxtValorRdf");
	operativaGestionObjetosConocimientoOntologias.txtRegistroIDs = $txtIds;
	operativaGestionObjetosConocimientoOntologias.txtCaracteristicasElem = $txtCaracteristicasElem;
	operativaGestionObjetosConocimientoOntologias.txtElemEditados = pRowElementoEntidadSecundaria.find("#mTxtElemEditados");	
	operativaGestionObjetosConocimientoOntologias.txtValores = pRowElementoEntidadSecundaria.find(`#${pTxtValores}`)// pTxtValores;
	operativaGestionObjetosConocimientoOntologias.txtNombreCatTesSem = pRowElementoEntidadSecundaria.find(`#mTxtNombreCatTesSem`);	

	operativaGestionObjetosConocimientoOntologias.LimpiarControlesEntidad(pEntidadHija, operativaGestionObjetosConocimientoOntologias.txtValores, $txtIds, $txtCaracteristicasElem, pTxtElemEditados);	
};


/**
 * Método que se dispara al hacer "autocomplete" de proopiedades externas. Este método es llamado desde la propia librería jqueryAutocomplete
 * @param {objet} selectedData Datos que se han seleccionado del servicio autocomplete
 */
const AgregarEntidadSeleccAutocompletar = function(selectedData){

	operativaGestionObjetosConocimientoOntologias.txtValorRdf = $("#mTxtValorRdf");
	operativaGestionObjetosConocimientoOntologias.txtRegistroIDs = $("#mTxtRegistroIDs");
	operativaGestionObjetosConocimientoOntologias.txtCaracteristicasElem = $("#mTxtCaracteristicasElem");
	operativaGestionObjetosConocimientoOntologias.txtElemEditados = $("#mTxtElemEditados");
	operativaGestionObjetosConocimientoOntologias.txtValores = $("#mTxtValorRdf");
	operativaGestionObjetosConocimientoOntologias.txtNombreCatTesSem = $("#mTxtNombreCatTesSem");	

	operativaGestionObjetosConocimientoOntologias.AgregarEntidadSeleccAutocompletar(selectedData)
};


//****************************************************************************************************************/
//****************************************************************************************************************/
/* Operativas de comportamiento compartidas
//****************************************************************************************************************/
//****************************************************************************************************************/

/**
 * Comportamiento para gestionar el multiidioma en inputs y textAreas.
 * En sitios multilingue se permitirá editar en varios idiomas. Ejemplo: Nombre y descripción de la comunidad.
 * @param {Int} numIdiomasVisibles: En el objeto pParams, numIdiomasVisibles será el nº de idiomas que se desea estén visibles en los Tabs. 
 * El resto se visualizarán en un dropdown menu.
 * @namespace operativaMultiIdioma
 * 
 */
const operativaMultiIdioma = {
	/**
	 * Acción para inicializar elementos y eventos
	 */
	init: function (pParams) {		
		const that = this;
		this.pParams = pParams;		
		this.config(pParams);
        // Carga de los idiomas
        this.cargarIdiomas();

		// Parámetro si deseamos que cada input NO esté en un tab diferente
		if (pParams.useOnlyOneTab == true){
			// Pintar los TabsHeaders y los tabsContents para cada fila (Es posible que haya múltiples filas con multiIdiomas)
			//const panelesConInputsMultiIdioma = $(".inputsMultiIdioma");
			const panelesConInputsMultiIdioma = $(".multiIdiomaContainer");
			// Si se desean pintar los inputs conjuntos en Tabs únicos
			$.each(panelesConInputsMultiIdioma, function () {
				const panelConMultiIdioma = $(this);
				that.pintarTabsHeaders(panelConMultiIdioma);
				that.pintarTabsContents(panelConMultiIdioma);
				// Eliminar la clase del contenedor del inputsMultiIdioma para que si hay otra carga no vuelva a revisar los mismos inputs
				panelConMultiIdioma.removeClass("multiIdiomaContainer"); 
			});			
		}else{
			// Si se desean pintar los inputs en tabs para cada input
        	// Asociar cada uno de los items encontrados en la página que disponen de idiomas
        	this.engancharMultiIdioma();		
		}

		// Recargar todos los posibles ckEditor creados por multiIdioma
		ckEditorRecargarTodos();

		// Configurar eventos si procede
		this.configEvents();
	},

	/*
	 * Configuración de los items en HTML y de los parámetros (nº de idiomas a mostrarse en los Tabs)
	 * */
	config: function (pParams) {
						
        // Idioma por defecto de la comunidad
        this.idiomaPorDefecto = $("#idiomaDefecto").val();
        // Idiomas de la comunidad disponibles
        this.idiomasComunidad = $("#idiomasComunidad").val().split("&&&");
        // Listado de idiomas que hay de momento. Se cargarán desde "cargaIdiomas"
        this.listaIdiomas = [];        
		// Inputs que requieren multiIdioma
		this.inputsMultiIdioma = $('.multiIdioma');

        if (pParams){
            // Nº de tabs de los idiomas que se van a mostrar. Los demás se meterán en un botón adicional
            this.numIdiomasVisibles = pParams.numIdiomasVisibles != undefined ? pParams.numIdiomasVisibles : 2;
			// Nombre de la clase del panel donde se añadirán todos tabsHeaders y tabsContent para multiIdiomas
			this.panContenidoMultiIdiomaClassName = pParams.panContenidoMultiIdiomaClassName != undefined ? pParams.panContenidoMultiIdiomaClassName : "panContenidoMultiIdioma";
			// ¿Permitir padding bottom o padding top? Dependiendo la vista interesará o no. Por defecto si que habrá padding
			this.allowPaddingBottom = pParams.allowPaddingBottom != undefined ? pParams.allowPaddingBottom : true;
			// Corresponderá con un número de 0 a 5 dependiendo del tipo de padding deseado
			this.paddingBottonValue = pParams.paddingBottomValue != undefined ? pParams.paddingBottomValue : undefined;
			this.allowPaddingTop = pParams.allowPaddingTop != undefined ? pParams.allowPaddingTop : undefined;			
			// Corresponderá con un número de 0 a 5 dependiendo del tipo de padding deseado
			this.paddingTopValue = pParams.paddingTopValue != undefined ? pParams.paddingTopValue : undefined;
			this.allowPaddingRight = pParams.allowPaddingRight != undefined ? pParams.allowPaddingRight : undefined;
			this.allowPaddingLeft = pParams.allowPaddingLeft != undefined ? pParams.allowPaddingLeft : undefined;
			this.allowPaddingTopInHeaders = pParams.allowPaddingTopInHeaders != undefined ? pParams.allowPaddingTopInHeaders : undefined;
        }        
	},
	/**
	 * Configuración de los eventos
	 */
	configEvents: function () {
		const that = this;
        
        $('body').on('click', 'a.languageOcultoItem', function(event) {
            // Idioma oculto seleccionado
            const languageOcultoSelected = $(event.target).text();
            // Lugar para posicionar el lenguage oculto seleccionado
            const languageOcultoLabel = $(event.target).parent().siblings().find("span");
            languageOcultoLabel.removeClass("material-icons");
            languageOcultoLabel.text(languageOcultoSelected);            
        });
	},

	/**
	 * Cargar la lista de idiomas disponibles y el idioma por defecto
	 * examinando los inputs ocultos de la página (#idiomaDefecto y #idiomasComunidad) y los almacena en that.listaIdiomas
	 */
	cargarIdiomas: function () {
		const that = this;
		// Guardar el Key| Value de cada idioma de la comunidad
		$.each(that.idiomasComunidad, function () {
			if (this != "") {
				that.listaIdiomas.push({
					key: this.split("|")[0],
					value: this.split("|")[1],
				});
			}
		});
	},

    /**
    * Asociar cada uno de los items encontrados en la página que disponen de idiomas para proceder a su montado 
    * o visualización del input mediante el método montarMultidioma que será donde se cargue la información correctamente
    */
    engancharMultiIdioma: function () {
        const that = this;        
        /* Guardo los inputs con la la información y contenido de MultiIdioma: inputs y textArea */
        that.inputsMultiIdioma = $('.multiIdioma');



        if (that.listaIdiomas.length > 1) {
            // Montar multiIdioma para cada input
            for (let i = 0; i < that.inputsMultiIdioma.length; i++) {
                const input = $(that.inputsMultiIdioma[i]);   
                // Eliminar la clase de "inputsMultiIdioma" para que si hay otra carga no vuelva a revisar estos mismos inputs
                input.removeClass("multiIdioma");
				// Eliminar la clase del contenedor del inputsMultiIdioma para que si hay otra carga no vuelva a revisar los mismos inputs
				input.parent().find(".multiIdiomaContainer").removeClass("multiIdiomaContainer");           
                that.montarMultidioma(input);                                                             
            }
            // Montar los paneles para cada input
            for (let i = 0; i < that.inputsMultiIdioma.length; i++) {
                const input = $(that.inputsMultiIdioma[i]);          
                that.montarMultiIdiomaTabPanel(input);                                                             
            }
        }else{
            // No hay multiIdioma -> Si es un textArea añadir la clase ckEditor           
            for (let i = 0; i < that.inputsMultiIdioma.length; i++) {                
                const input = $(that.inputsMultiIdioma[i]);     
                // Eliminar la clase de "inputsMultiIdioma" para que si hay otra carga no vuelva a revisar estos mismos inputs
                input.removeClass("multiIdioma");
				// Eliminar la clase del contenedor del inputsMultiIdioma para que si hay otra carga no vuelva a revisar los mismos inputs
				input.parent().find(".multiIdiomaContainer").removeClass("multiIdiomaContainer");   
                const esTextArea = input.is('textarea');     
                if (esTextArea){
                    input.addClass("cke mensajes");
					//ckEditorRecargarTodos();            
                }                
            }
        }        
    },


	/**
	 * Método para pintar el encabezado o las pestañas de los diferentes idiomas existentes. Para navegar a través de los tabContents
	 * @param {jqueryObject} panelConInputMultiIdioma : Panel donde se ha de buscar los inputs con multiIdioma para pintar su content en diferentes idiomas
	 */
	pintarTabsHeaders: function(panelConInputMultiIdioma){
		const that = this;
		// Nº de tabs que se han pintado
		let numTabsPintadas = 0;
		// Conocer si es necesario pintar Más lenguajes
		let buttonMoreLanguagesPintada = false;
        let esNecesarioPintarMoreLanguages = false;
		// Contenido html para los tabsHeaders
		let tabsHeaderHtml = '';

		// Controlar si se desea padding en los TabHeaders (Pestañas)
		const paddingTopClassName = that.allowPaddingTopInHeaders == false ? "pt-0" : "";		
		const paddingLeftClassName = that.allowPaddingLeft == false ? "pl-0" : "";
		const paddingRightClassName = that.allowPaddingRight == false ? "pr-0" : "";

		// Identificador del panel para visualizar los tabs si hay múltiples tabs multiIdioma
		const idPanelConInputMultiIdioma = panelConInputMultiIdioma.data("idmultiidiomatab") != "undefined" ? panelConInputMultiIdioma.data("idmultiidiomatab") : guidGenerator(); 
		
		// Proporcionar id automático para las tabs
		let idNavTabs = guidGenerator();

		// Cabecera de los items del Tab ("Español | Inglés ..."). Pintar cabecera si hay más de 1 idioma
		if (that.listaIdiomas.length > 1){
			tabsHeaderHtml = `<ul class="nav nav-tabs ${paddingLeftClassName} ${paddingRightClassName} ${paddingTopClassName}" role="tablist" id="edicion_multiidioma_${idNavTabs}">`;
			// Construcción de cada Tab para cada idioma
			$.each(that.listaIdiomas, function (id, idioma) {                   
				const key = idioma.key;
				const value = idioma.value;
				const esIdiomaPorDefecto = (key == that.idiomaPorDefecto);
			
				if (numTabsPintadas < that.numIdiomasVisibles){            
				// Construir cada item del Tab
				tabsHeaderHtml += ` <li class="nav-item" role="presentation">
										<a class="nav-link ${esIdiomaPorDefecto ? 'active' : ''}" 
										id="tab_${key}" 
										data-toggle="tab" 
										href="#nav_${idPanelConInputMultiIdioma}_${key}" 
										role="tab"
										aria-controls="nav_${idPanelConInputMultiIdioma}_${key}"
										aria-selected="${ esIdiomaPorDefecto ? 'true' : 'false' }">${value}</a>
									</li>
				`;
				// Controlar el nº de tabs pintadas
				numTabsPintadas += 1;
				}else{
				// Pintar el item "oculto" con un botón de "settings"
					if (buttonMoreLanguagesPintada == false){
						esNecesarioPintarMoreLanguages = true;
						// Pintar el tab de Más idiomas
						tabsHeaderHtml += `
									<li class="nav-item dropdown">
										<a class="nav-link dropdown-toggle languageOculto" data-toggle="dropdown" href="#" role="button" aria-haspopup="true" aria-expanded="false">
											<span>Otros idiomas</span>
										</a>
									<div class="dropdown-menu dropdown-menu-left basic-dropdown dropdown-idiomas">
						`;         
						buttonMoreLanguagesPintada = true;           				  
					}										
					if (buttonMoreLanguagesPintada == true){
						// Pintar las diferentes opciones ocultas
						tabsHeaderHtml += `
						<a class="item-dropdown languageOcultoItem" 
						id="tab_${key}"					
						href="#nav_${idPanelConInputMultiIdioma}_${key}"
						data-toggle="tab" 
						role="tab"
						aria-controls="#nav_${idPanelConInputMultiIdioma}_${key}"
						aria-selected="${ esIdiomaPorDefecto ? 'true' : 'false' }">${value}</a>
						`;                    
					}
				}
			});
		}

		if (esNecesarioPintarMoreLanguages == true){
            // Cerrar el div y li del botón "settings"
            tabsHeaderHtml += "</div></li>";
        }

		// Si hay más de un idioma, cerrar etiquetas y mostrar tabsHeaders
		if (that.listaIdiomas.length > 1){
        	// Cerrar cabecera de Tabs 
			tabsHeaderHtml += '</ul>';			
			// Mostrar tabs en pantalla en la sección correspodiente según la clase
			// that.pParams.panContenidoMultiIdioma.append(tabsHeaderHtml);
			// Añadir el panel del html en el item/fila correspondiente ya que puede haber múltiples filas
			panelConInputMultiIdioma.siblings(`.${that.panContenidoMultiIdiomaClassName}`).append(tabsHeaderHtml);	
		}		
	},


	/**
	 * Método para pintar los tabs contents de los idiomas. Cada input deberá ir en el tab del idioma correspondiente
	 * @param {jqueryObject} panelConInputMultiIdioma : Panel donde se ha de buscar los inputs con multiIdioma para pintar su content en diferentes idiomas
	 */
	pintarTabsContents: function(panelConInputMultiIdioma){
		const that = this;
		// Contador para recorrido de listado de idiomas a pintar
		let contador = 0;

		// Controlar si se desean padding superior e inferior al tab-content
		let paddingTopClassName = "";
		if (that.allowPaddingTop == false){
			paddingTopClassName = "pt-0";
		}else{
			// Se desea padding. Comprobar si hay un valor. Si no hay valor, dejar por defecto "vacío"
			paddingTopClassName = that.paddingTopValue != undefined ? `pt-${that.paddingTopValue}` : "";
		}
		
		let paddingBottomClassName = "";
		if (that.allowPaddingBottom == false){
			paddingBottomClassName = "pb-0";
		}else{
			// Se desea padding. Comprobar si hay un valor. Si no hay valor, dejar por defecto "0"
			paddingBottomClassName = that.paddingBottomValue != undefined ? `pb-${that.paddingBottomValue}` : "";
		}		

		const paddingLeftClassName = that.allowPaddingLeft == false ? "pl-0" : "";
		const paddingRightClassName = that.allowPaddingRight == false ? "pr-0" : "";
			
		// Tabs panels y contenido en multilenguaje
		let tabsContentHtml = `<div class="tab-content ${paddingTopClassName} ${paddingBottomClassName} ${paddingLeftClassName} ${paddingRightClassName}">`;
		
		// Obtener los textos por key y value de cada idioma
		const listaTextos = [];

		// Para cada idioma, revisar el input dentro de cada panel o fila correspondiente para que no coja todos los inputs juntos (inputText, textArea)
		// Recorrer cada inputs de cada panel de multiIdiomas
		$.each(panelConInputMultiIdioma.find(that.inputsMultiIdioma), function () {
			// Obtener el id del input para generar luego los ids necesarios. Comprobar que haya al menos id para el input			
			let inputId =  $(this).attr("id");
			// Generar id automático en caso de que no haya id (Ej: Filas con inputs con multiIdiomas)
			if (inputId == undefined){
				inputId = guidGenerator();
				// Asignar un id automático
				$(this).attr("id", inputId);
			}

			// Obtener el texto de la label que deberá acompañar al input (Nombre | Descripción)
			const inputLabelText =  $(this).data("labeltext");
			// Obtener el texto de ayuda que acompañará al input 
			const inputHelpText = $(this).data("helptext");
			// Detecta si el input es un textArea o no
			const esTextArea = $(this).is('textarea');
			// Detectar si se desea que tenga operativa CKEditor
			const textAreaWithCke = $(this).hasClass("noCKE") ? false : true;
			// Detectar posible callBack del input
			const onInputCallback = $(this).data('on-input-function');						
			// Obtener el texto en los diferentes idiomas del input que está analizándose
			let textoMultiIdioma = $(this).val().split('|||');

			// Comprobar que si no hay contenido para otros idiomas, pintar "vacío" para que pinte las labels, inputs de ese idioma
			// 1 - Recorrer los idiomas de la comunidad y comprobar que hay algo en ese idioma, si no hay añadir texto vacío
			// Eliminar el último item si este es vacío
			if (textoMultiIdioma[textoMultiIdioma.length-1].length == 1){
				textoMultiIdioma.pop();
			}
			
			// Array con los idiomas disponibles en la comunidad			
			let arrayLanguages = [];
			for (let i = 0; i < that.listaIdiomas.length; i++) {
				const keyIdioma = "@"+ that.listaIdiomas[i]["key"];
				arrayLanguages.push(keyIdioma);
			}

			// Nº de veces que se ha encontrado el item en los idiomas de la comunidad. Si es 0, el item no se ha encontrado ya que
			// puede que no tenga valor en otros idiomas y solo en el idioma por defecto de la comunidad
			let contadorVecesEncontradas = 0;
			// Encontrar los idiomas que no existen para el input
			let valueLanguagesInBlank = arrayLanguages.filter(element => 
				{
					var encontrado = null;
					for (let i = 0; i < textoMultiIdioma.length; i++) {
						if (textoMultiIdioma[i].includes(element)){
							contadorVecesEncontradas += 1;
							encontrado = true;
							return false;						
						}
						else
						{
							encontrado = false;
							//return false;
						}
					}
					if(!encontrado)
					{
						return element;
					}
				} 								
			);

			// Añadir los posibles idiomas en blanco al textoMultiIdioma
			if (valueLanguagesInBlank.length > 0){
				// Tener en cuenta si se ha encontrado el item en otros idiomas. En caso contrario, añadirle sólo para el idioma por defecto				
				// Añadirle un espacio al inicio del idioma el cual no se ha encontrado en BD
				valueLanguagesInBlank = valueLanguagesInBlank
				.map(function(item){
					let inputValue = '';
					if (contadorVecesEncontradas == 0){						
						// No se ha encontrado ningún idioma -> Insertar el del idioma por defecto
						if (item.includes(that.idiomaPorDefecto)){
							inputValue = ` ${textoMultiIdioma}${item}`;
						}else{
							inputValue = ` ${item}`;
						}
					}else{
						// Añadir el espacio para el idioma del que no se ha encontrado un value
						inputValue = ` ${item}`;
					}
					return inputValue;
				});
				// Construyo el array con los idiomas encontrados y no encontrados
				textoMultiIdioma.push(...valueLanguagesInBlank);				
			}

			// Ocultar el input original si no es textArea.
			$(this).addClass("d-none");
			// Eliminar la clase de "multiIdioma" para que si hay otra carga no vuelva a revisar estos mismos inputs
			$(this).removeClass("multiIdioma"); 			

			// Quitar de textoMultiIdioma el item que no contenga @ porque no hay idioma o es vacío (Para no crear inputs multiIdioma dinámicamente)
			textoMultiIdioma = textoMultiIdioma.filter(function(item){return item.includes("@") });		

			// Crear array con todos los idiomas
			$.each(textoMultiIdioma, function () {
				if (this != "") {
					var objetoIdioma = that.obtenerTextoYClaveDeIdioma(this);
					// Añado información de si es textArea o no
					objetoIdioma["esTextArea"] = esTextArea;
					// Añado el id del input para generar los ids necesarios
					objetoIdioma["id"] = inputId;
					// Añado el data atributo del labeltext para la label que acompañará al input
					objetoIdioma["labeltext"] = inputLabelText;										
					objetoIdioma["helptext"] = inputHelpText;
					// Añadir posible evento onInputCallback
					objetoIdioma["onInputCallback"] = onInputCallback;
					// Indicar si se desea ckEditor para el textArea
					objetoIdioma["textAreaWithCke"] = textAreaWithCke;
					listaTextos.push(objetoIdioma);									
				}
			});		
		});

		// Identificador del panel para visualizar los tabs si hay múltiples tabs multiIdioma
		const idPanelConInputMultiIdioma = panelConInputMultiIdioma.data("idmultiidiomatab") != "undefined" ? panelConInputMultiIdioma.data("idmultiidiomatab") : guidGenerator(); 

		// Pintado de los tabContent a partir de los idiomas existentes
		// Crear array con todos los idiomas
		$.each(that.listaIdiomas, function () {

			const keyIdioma = this.key;
			const esIdiomaPorDefecto = (keyIdioma == that.idiomaPorDefecto);

			// Pintado del content que albergará los diferentes inputs en el idioma correspondiente (Si hay más de 1 idioma)
			if (that.listaIdiomas.length > 1){			
				tabsContentHtml += `
				<div class="tab-pane fade ${esIdiomaPorDefecto ? 'active show' : ''} edicion_${keyIdioma}" 
					id="nav_${idPanelConInputMultiIdioma}_${keyIdioma}"					
					role="tabpanel" 
					aria-labelledby="nav_${idPanelConInputMultiIdioma}_${keyIdioma}">											
				`;
			}

			// Recorrer los inputs y coger los de los idiomas necesarios para construir el tabContent
			for (const texto of listaTextos) {                 
				const key = texto.key;
				// Si no hay nada de texto, quitar el espacio en blanco añadido
				const value = (texto.value.length == 1 && texto.value == " ") ? "" : texto.value ;							
				const labelText = texto.labeltext;
				const helpText = texto.helptext;
				const esTextArea = texto.esTextArea;
				const onInputCallback = texto.onInputCallback;	
				const textAreaWithCke = texto.textAreaWithCke;
				const textAreaWithCkeClassName = textAreaWithCke == true ? "cke editorHtml": ""; 
				const id = texto.id; 
			
				// Incluirlo en el tab ya que es el mismo idioma
				if (key == keyIdioma){
					if (!esTextArea){
						// Input normal
						tabsContentHtml += `
							<div class="form-group mb-3">
								<label class="control-label d-block">${labelText}</label>
								<input type="text"						  
									class="form-control"
									oninput="${onInputCallback != undefined ? onInputCallback : null}"
									data-language="${keyIdioma}"
									id="input_${id}_${key}" 
									value="${value.replace(/\'/g, "\'").replace(/\"/g, '&quot;')}"/>`
							// Añadir un helpText si es necesario
							if (helpText != undefined){
								tabsContentHtml += `<small class="form-text text-muted">${helpText}</small>`;
							}
						tabsContentHtml += `</div>`;																	
					}else{
						// Input de tipo textArea
						tabsContentHtml += `
							<div class="form-group mb-3">
								<label class="control-label d-block">${labelText}</label>
								<textarea type="text"
										class="${textAreaWithCkeClassName} form-control"
										id="input_${id}_${key}">${value.replace(/\'/g, "\'").replace(/\"/g, '&quot;')}</textarea>
							</div>
						`;
					}					
				}
			}
			// Cerrar el tabFade del idioma
			tabsContentHtml += `</div>`;
		});

		// Añadir el panel del html correspondiente donde se indica a través del plugin
		///that.pParams.panContenidoMultiIdioma.append(tabsContentHtml);	
		// Añadir el panel del html en el item/fila correspondiente ya que puede haber múltiples filas
		panelConInputMultiIdioma.siblings(`.${that.panContenidoMultiIdiomaClassName}`).append(tabsContentHtml);				
		//$('#edicion_multiidioma').after(tabsContentHtml);
		// Recargar posibles ckEditor en los inputs
		//ckEditorRecargarTodos();
	},



    /**
     * Método para obtener el texto y clave de los idiomas disponibles de la comunidad para posteriormente poder montarIdioma para cada input.
     * @param {*} pTexto : Array descompuesto en caracteres para poder buscar el idioma en el que está "@en" o "@es"... que corresponde con los últimos 3 caracteres.
     * @return Devuelve un objeto con el key del idioma y el value del idioma: Ejemplo: "es": "Español", "en", "English"
     */	
     obtenerTextoYClaveDeIdioma: function (pTexto) {
        const that = this;
        let idioma = that.idiomaPorDefecto;
        let textoIdioma = pTexto.trim();
        if (textoIdioma.length > 3) {
            var indice = 3;
            var auxIdioma = textoIdioma.substr(textoIdioma.length - 3);
            if (auxIdioma[0] == '-' && textoIdioma.length > 6) {
                indice = 6;
                auxIdioma = textoIdioma.substr(textoIdioma.length - 6);
            }
            if (auxIdioma[0] == '@') {
                idioma = auxIdioma.substr(1);
                textoIdioma = textoIdioma.substr(0, textoIdioma.length - indice);
            }
        }else if(textoIdioma.length == 3){
			// No hay texto para un idioma en concreto. Añadir un espacio y asignarle el idioma
			let auxIdioma = textoIdioma.substr(textoIdioma.length - 2);
			textoIdioma = " ";
			idioma = auxIdioma;
		}
        return { "key": idioma, "value": textoIdioma };
    },

    /**
     * Mostrar el input con los diferentes multiIdiomas disponibles en la comunidad
     * @param {jqueryObject} pInput: Objeto input jquery para montar inputs de tipo multiIdioma
     */
    montarMultidioma: function (pInput) {
        const that = this;
        let numTabsPintadas = 0;
        let buttonMoreLanguagesPintada = false;
        let esNecesarioPintarMoreLanguages = false;

        const esTextArea = pInput.is('textarea');
        if (esTextArea) { pInput.css("display", "none"); }
        else { pInput.attr("type", "hidden"); }

        const textoMultiIdioma = pInput.val().split('|||')
        const listaTextos = [];
        $.each(textoMultiIdioma, function () {
            if (this != "") {
                var objetoIdioma = that.obtenerTextoYClaveDeIdioma(this);
                listaTextos.push(objetoIdioma);
            }
        });

		// Configuración del id si tiene o no
		let inputIdentifier = "";
		if (pInput.attr("id") == undefined){
			inputIdentifier = guidGenerator();							
			pInput.attr("id", inputIdentifier);
		}		

        // Cabecera de los items del Tab ("Español | Inglés ...")
        let htmlMultIdioma = `
            <ul class="nav nav-tabs" role="tablist" id="edicion_multiidioma_${pInput.attr("id")}">        
        `;

        // Construcción de cada pestaña o enlace para poder abrir visualizar el input con el idioma correspondiente
        $.each(that.listaIdiomas, function (id, idioma) {                   
            const key = idioma.key;
            const value = idioma.value;
            const esIdiomaPorDefecto = (key == that.idiomaPorDefecto);

            if (numTabsPintadas < that.numIdiomasVisibles){            
            // Construir cada item del Tab
            htmlMultIdioma += ` <li class="nav-item" role="presentation">
                                    <a class="nav-link ${esIdiomaPorDefecto ? 'active' : ''}" 
                                    id="tab_${pInput.attr("id")}_${key}" 
                                    data-toggle="tab" 
                                    href="#edicion_${pInput.attr("id")}_${key}" 
                                    role="tab"
                                    aria-controls="edicion_${pInput.attr("id")}_${key}"
                                    aria-selected="${ esIdiomaPorDefecto ? 'true' : 'false' }">${value}</a>
                                </li>
            `;
            // Controlar el nº de tabs pintadas
            numTabsPintadas += 1;
            }else{
            // Pintar el item "oculto" con un botón de "settings"
                if (buttonMoreLanguagesPintada == false){
                    esNecesarioPintarMoreLanguages = true;
                    // Pintar el tab de Más idiomas
                    htmlMultIdioma += `
                                <li class="nav-item dropdown">
                                <a class="nav-link dropdown-toggle languageOculto" data-toggle="dropdown" href="#" role="button" aria-haspopup="true" aria-expanded="false">
					                <span class="material-icons">settings</span>
				                </a>
                                <div class="dropdown-menu">
                    `;         
                    buttonMoreLanguagesPintada = true;           				  
                }				
				if (buttonMoreLanguagesPintada == true){
                    // Pintar las diferentes opciones ocultas
                    htmlMultIdioma += `
                    <a class="dropdown-item languageOcultoItem" 
                    id="tab_${pInput.attr("id")}_${key}"
                    data-toggle="tab" href="#edicion_${pInput.attr("id")}_${key}"
                    role="tab"
                    aria-controls="#edicion_${pInput.attr("id")}_${key}"
                    aria-selected="${ esIdiomaPorDefecto ? 'true' : 'false' }">${value}</a>
                    `;  
				}			                                
            }
        });

        if (esNecesarioPintarMoreLanguages == true){
            // Cerrar el div y li del botón "settings"
            htmlMultIdioma += "</div></li>";
        }

        htmlMultIdioma += '</ul>';

        // Mostrar tabs en pantalla
        pInput.before(htmlMultIdioma);
    },

    /**
     * Obtener el texto del idioma para poder mostrarlo posteriormente en pantalla
     * @param {String} pKey : El key o id del tipo de idioma ("es -> Español, en-> Inglés...")
     * @param {bool} pEsIdiomaPorDefecto : Indica si el idioma que está siendo evaluado es el actual o no.
     * @param {object} pListaTextos : Objeto de elementos que contienen los textos en diferentes idiomas
     * @param {object} pTextoIdioma : Texto actual del idioma según el "key"
     * @return {String}: Devuelve el texto según el idioma correspondiente para el input.
     */
    getTextoIdioma: function(pKey, pEsIdiomaPorDefecto, pListaTextos, pTextoIdioma){

        if (pKey.indexOf('-') > 0) {
            const keyAux = pKey.substr(0, pKey.indexOf('-'));
            pTextoIdioma = pListaTextos.findValueByKey(keyAux);
        }
        if (pTextoIdioma == null || pTextoIdioma == '') {
            if (pEsIdiomaPorDefecto) {
                pTextoIdioma = pListaTextos[0].value;
            }
            else {
                pTextoIdioma = '';
            }
        }
        return pTextoIdioma;
    },

    /**
     * Montar el contenido del Tab en el idioma correspondiente cada input/textArea.
     * Se utiliza para que al pulsar en cada Tab, puedan visualizarse estos paneles dependiendo del idioma seleccionado.
     */
    montarMultiIdiomaTabPanel: function(pInput){
        const that = this;
        const esTextArea = pInput.is('textarea');
        let htmlMultIdioma = '<div class="tab-content">';

        const textoMultiIdioma = pInput.val().split('|||')
        const listaTextos = [];
        $.each(textoMultiIdioma, function () {
            if (this != "") {
                var objetoIdioma = that.obtenerTextoYClaveDeIdioma(this);
                listaTextos.push(objetoIdioma);
            }
        });

        $.each(that.listaIdiomas, function (id, idioma) {
            const key = idioma.key;
            const value = idioma.value;
            const esIdiomaPorDefecto = (key == that.idiomaPorDefecto);
            let textoIdioma = listaTextos.findValueByKey(key);
            if (textoIdioma == null || textoIdioma == '')
            {
				// Si no hay texto de un idioma concreto, dejarlo como "vacío"
				textoIdioma = "";
                // textoIdioma = that.getTextoIdioma(key, esIdiomaPorDefecto, listaTextos, textoIdioma);
            }
			
			// Cargar un input normal o textArea
			if (!esTextArea){
				htmlMultIdioma += `
					<div class="tab-pane fade show ${esIdiomaPorDefecto ? 'active' : ''} mt-2 edicion_${key}" 
					id="edicion_${pInput.attr("id")}_${key}" 
					role="tabpanel" aria-labelledby="${id}">
						<input type="text"						  
						  class="form-control"
						  id="input_${pInput.attr("id")}_${key}" 
						  value="${textoIdioma.replace(/\'/g, "\'").replace(/\"/g, '&quot;')}"/>
					</div>
            	`;
			}else{
				htmlMultIdioma += `
					<div rows="4" cols="50" 
						class="tab-pane fade show ${esIdiomaPorDefecto ? 'active' : ''} mt-2 edicion_${key}" 
						id="edicion_${pInput.attr("id")}_${key}"
						role="tabpanel" 
						aria-labelledby="${id}">
							<textarea type="text"
							class="cke mensajes form-control"
							id="input_${pInput.attr("id")}_${key}">
							${textoIdioma.replace(/\'/g, "\'").replace(/\"/g, '&quot;')}
							</textarea>
					</div>
            	`;
			}
        });

        htmlMultIdioma += `</div>`;
        // Añadir el panel del html correspondiente
        pInput.before(htmlMultIdioma);
        // Recargar posibles ckEditor en los inputs
        //ckEditorRecargarTodos();
    },
};

/**
 * Método para comprobar las fechas introducidas en datePicker. Heredado de Front de Comunidad para el comportamiento de Facetas de tipo "Calendario".
 * @param {*} fecha1 Fecha
 * @param {*} fecha2 Fecha 
 * @param {*} fechaCambiada 
 */
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

/**
 * Comprobar si el valor introducido es una fecha válida. Heredado de Front de Comunidad para el comportamiento de Facetas de tipo "Calendario".
 * @param {*} texbox 
 * @returns 
 */
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

/**
 * Método para obtener el rango a partir de un filtro de facetas (Ej: Facetas de tipo Fecha). Heredado de Front de la comunidad
 * @param {*} control1 
 * @param {*} textoInicioCtrl1 
 * @param {*} control2 
 * @param {*} textoInicioCtrl2 
 * @param {*} esFiltroFechas 
 * @returns 
 */
function ObtenerFiltroRango(control1, textoInicioCtrl1, control2, textoInicioCtrl2, esFiltroFechas) {
    var resultado = '';
    if (control1 != null && $(control1).val() != "" && $(control1).val() != textoInicioCtrl1 && control1.type != 'hidden') {
        var valor = $(control1).val();
        if (esFiltroFechas) {
            valor = cambiarFormatoFecha(valor);
        }
        resultado += valor;
    }
    resultado += '-';
    if (control2 != null && $(control2).val() != "" && $(control2).val() != textoInicioCtrl2 && control2.type != 'hidden') {
        var valor2 = $(control2).val();
        if (esFiltroFechas) {
            valor2 = cambiarFormatoFecha(valor2);
        }
        resultado += valor2;
    }

    return resultado;
}

/**
 * Método para cambiar una fecha en formato 01/02/2011 a valor "string" para realizar la búsqueda por facetas. Heredado de Front de la Comunidad. (Comportamiento de Faceta Meses)
 * @param {*} fecha 
 * @returns 
 */
function cambiarFormatoFecha(fecha) {    
    let cachos = fecha.split('/');    
    return cachos[2] + cachos[1] + cachos[0];
}


/**
 * Clase jquery para poder gestionar las búsquedas de fechas en Facetas (Mes pasado, semana pasada, semestre pasado)
 * Calculará la fecha teniendo en cuenta la opción pulsada para escribir el valor en el input "Desde" y "Hasta"
 * Se utilizará la librería moment.js para el trabajo con fechas
 * @namespace operativaFechasFacetas
 * */
const operativaFechasFacetas = {

    /**
     * Acción que dispara directamente la lógica de operativaFechas
     */
    init: function () {
        this.config();
        this.configEvents();
    },

    /**
     * Configuración de los elementos que tendrán eventos
     */
    config: function () {
        // Opción buscador de fechas "Última semana"    
        this.optionLastWeek = `facet-last-week`,
        // Opción buscador de fechas "Último mes"    
        this.optionLastMonth = `facet-last-month`,
        // Opción buscador de fechas "Último semestre"    
        this.optionLastSemester = `facet-last-semester`,
        // Opción buscador de fechas "Último semestre"    
        this.optionLastYear = `facet-last-year`,
        // Botón de buscar 
        this.searchButton = `searchButton`,
        // Input from-to
        this.inputFromDate = `facet-from`,
        this.inputToDate = `facet-to`;

    },

    /**
     * Configuración eventos de elementos HTML
     * */
    configEvents: function () {

        const that = this;

		/*
        // Opción LastWeek
        $(document).on('click', this.optionLastWeek, function (event) {
            that.getAndSetDate(this);
        });
		*/
		// Opción LastWeek
        configEventByClassName(`${that.optionLastWeek}`, function(element){
            const $jqueryElement = $(element);
            $jqueryElement.off().on("click", function(){ 
				that.getAndSetDate(this);
            });	                        
        });

        // Opción LastMonth
		/*
        $(document).on('click', this.optionLastMonth, function (event) {
            that.getAndSetDate(this);
        });
		*/

		// Opción LastMonth
        configEventByClassName(`${that.optionLastMonth}`, function(element){
            const $jqueryElement = $(element);
            $jqueryElement.off().on("click", function(){ 
				that.getAndSetDate(this);
            });	                        
        });

        // Opción Semester
		/*
        $(document).on('click', this.optionLastSemester, function (event) {
            that.getAndSetDate(this);
        });
		*/

		// Opción Semester
        configEventByClassName(`${that.optionLastSemester}`, function(element){
            const $jqueryElement = $(element);
            $jqueryElement.off().on("click", function(){ 
				that.getAndSetDate(this);
            });	                        
        });		

        // Opción Year
		/*
        $(document).on('click', this.optionLastYear, function (event) {
            that.getAndSetDate(this);
        });
		*/
		// Opción Year
        configEventByClassName(`${that.optionLastYear}`, function(element){
            const $jqueryElement = $(element);
            $jqueryElement.off().on("click", function(){ 
				that.getAndSetDate(this);
            });	                        
        });				
		

		// Configuración de posibles "DatePicker"		
		const oldYear = moment().format('YYYY') - 100;
		const currentYear = moment().format('YYYY');
		// DatePicker normales
		$('.hasDatepicker').datepicker({
			changeMonth: true,
			changeYear: true,
			yearRange: `${oldYear}:${currentYear}`,
		});
		// DatePicker con límite en Fecha
		$('.datepicker-minToday').datepicker({
			changeMonth: true,
			changeYear: true,        
			// Fecha mínima será hoy
			minDate: '0',       
		});	

		/* Evitar ocultamiento de faceta cuando se utiliza el datepicker para seleccionar meses */
		$(document).on('click', '#ui-datepicker-div, .ui-datepicker-prev, .ui-datepicker-next', function (e) {
    		e.stopPropagation();
		});	
    },

    /**
     * Calcular el plazo de tiempo deseado y establecerlo en los inputs "from" y "to"
     * */
    getAndSetDate: function (item) {
        let localLocale = moment();
        moment.locale('es');
        localLocale.locale(false);

        // Fecha inicial
        let startDate = '';
        // Fecha final (actual)
        let endDate = localLocale.format('L');

        // Coger la última semana del día actual (-De momento se escogen por días)
        //console.log(moment().subtract(1, 'weeks').startOf('isoWeek').format('L'));
        //console.log(moment().subtract(1, 'weeks').endOf('isoWeek').format('L'));

        if ($(item).hasClass(`facet-last-week`)) {
            startDate = localLocale.subtract(7, "days");
        } else if ($(item).hasClass(`facet-last-month`)) {
            startDate = localLocale.subtract(30, "days");
        } else if ($(item).hasClass(`facet-last-semester`)) {
            startDate = localLocale.subtract(180, "days");
        } else {
            // Selección del último año
            startDate = localLocale.subtract(365, "days");
        }

        // Botón para búsqueda,  inputs para establecer fechas
        const searchBtn = $(item).parent().parent().parent().find(`.${this.searchButton}`);
        const fromDateValue = $(item).parent().parent().parent().find(`.${this.inputFromDate}`);
        const toDateValue = $(item).parent().parent().parent().find(`.${this.inputToDate}`);

        // Escribir las fechas en inputs
        fromDateValue.val(startDate.format('L'));
        toDateValue.val(endDate);

        // Hacer click en botón de búsqueda
        searchBtn.trigger("click");
    },
}



/**
 * Operativa Jcrop de imágenes / banners
 * 
 * */

/**
 * Comportamiento para gestionar el recorte de imagenes. Lo que hace es permitir mostrar una visualización del recorte de la imagen para que el usuario sepa la sección seleccionado
 * de una imagen. Ejemplo de uso: Selección imagen cabecera de comunidad. 
  * El resto se visualizarán en un dropdown menu.
 * @namespace operativaJrop
 */
 const operativaJrop = {
	init: function(pParams){
		this.config(pParams);
		this.engancharJcrop();
		this.configEvents();
	},
	/**
	 * Configurador de elementos del DOM para operativa de JCROP de imágenes
	 * */
	config: function(pParams){	
		const that = this;
		// Imagen a realizar el jCrop
		this.$image = $("#image-uploader__cabecera");
		this.Pos_X_0 = $('#ImageHead_Pos_X_0').val();
		this.Pos_Y_0 = $('#ImageHead_Pos_Y_0').val();
		this.Pos_X_1 = $('#ImageHead_Pos_X_1').val();
		this.Pos_Y_1 = $('#ImageHead_Pos_Y_1').val();
		// Indicar si el plugin ya está lista para que no sobreescriba valores del jcrop
		this.isReady = false;
			
		this.options = {
			zoomable: false,
			// Ratio de la imagen a recortar
			aspectRatio: 16 / 9,
			preview: '.img-preview',
			// Evitar que salga fuera del "cropBox"
			viewMode: 2,
			// Establecer el recorte inicial del cropBox
			autoCrop: true,

			ready: function(e){
				// Inicializar el cropboxdata en las posiciones elegidas
				that.cropper.setCropBoxData({
					"top": parseInt(that.Pos_Y_0),
					"left": parseInt(that.Pos_X_0),
					"width": parseInt(that.Pos_X_1 - that.Pos_X_0),
					"height": parseInt(that.Pos_Y_1 - that.Pos_Y_0),
				});
				that.isReady = true;
			},

			crop: function (e) {
				// Establecer los límites establecidos del recorte de la imagen
				if (that.isReady == true){
					$('#ImageHead_Pos_X_0').val(Math.round(e.detail.x));
					$('#ImageHead_Pos_Y_0').val(Math.round(e.detail.y));								
					$('#ImageHead_Pos_X_1').val(Math.round(e.detail.x) + Math.round(e.detail.width));
					$('#ImageHead_Pos_Y_1').val(Math.round(e.detail.y) + Math.round(e.detail.height));
				}				
			}
		};
		this.originalImageURL = this.$image.attr('src');
		this.uploadedImageName = 'cropped.jpg';
		this.uploadedImageType = 'image/jpeg';
		this.uploadedImageURL;
	},

	engancharJcrop : function(){
		const that = this;
		// Creación del cropper
		this.cropper = new Cropper(that.$image[0], that.options);
		

		/*
		// Cropper						
		this.$image.on({
			ready: function (e) {
			},
			cropstart: function (e) {
			//console.log(e.type, e.detail.action);
			},
			cropmove: function (e) {
			
			},
			cropend: function (e) {
			
			},
			crop: function (e) {
			
			},
			zoom: function (e) {
			
			},			
		}).cropper(this.options);		*/
	},	
	
	configEvents: function(){
		  // Buttons
		if (!$.isFunction(document.createElement('canvas').getContext)) {
			$('button[data-method="getCroppedCanvas"]').prop('disabled', true);
		}
		
		if (typeof document.createElement('cropper').style.transition === 'undefined') {
			$('button[data-method="rotate"]').prop('disabled', true);
			$('button[data-method="scale"]').prop('disabled', true);
		}
		// Download
		/*if (typeof $download[0].download === 'undefined') {
			$download.addClass('disabled');
		}*/	
		
		// Options
		/*$('.docs-toggles').on('change', 'input', function () {
			var $this = $(this);
			var name = $this.attr('name');
			var type = $this.prop('type');
			var cropBoxData;
			var canvasData;
			if (!$image.data('cropper')) {
			return;
			}
			if (type === 'checkbox') {
			options[name] = $this.prop('checked');
			cropBoxData = $image.cropper('getCropBoxData');
			canvasData = $image.cropper('getCanvasData');
			options.ready = function () {
				$image.cropper('setCropBoxData', cropBoxData);
				$image.cropper('setCanvasData', canvasData);
			};
			} else if (type === 'radio') {
			options[name] = $this.val();
			}
			$image.cropper('destroy').cropper(options);
		});*/
	},
}


/**
 * Comportamiento para gestionar el recorte de imagenes. Lo que hace es permitir mostrar una visualización del recorte de la imagen para que el usuario sepa la sección seleccionado
 * de una imagen. Ejemplo de uso: Selección imagen cabecera de comunidad. 
  * El resto se visualizarán en un dropdown menu.
 * @namespace operativaJropFavicon
 */
 const operativaJropFavicon = {
	init: function(pParams){
		this.config(pParams);
		this.engancharJcrop();
		this.configEvents();
	},
	/**
	 * Configurador de elementos del DOM para operativa de JCROP de imágenes
	 * */
	config: function(pParams){	
		const that = this;
		// Imagen a realizar el jCrop
		this.$image = $("#image-uploader__favicon");
		this.Pos_X_0 = $('#ImageFavicon_Pos_X_0').val();
		this.Pos_Y_0 = $('#ImageFavicon_Pos_Y_0').val();
		this.Pos_X_1 = $('#ImageFavicon_Pos_X_1').val();
		this.Pos_Y_1 = $('#ImageFavicon_Pos_Y_1').val();
		// Indicar si el plugin ya está lista para que no sobreescriba valores del jcrop
		this.isReady = false;
			
		this.options = {
			zoomable: false,
			// Ratio de la imagen a recortar
			aspectRatio: 1 / 1,
			preview: '.img-preview-favicon',
			// Evitar que salga fuera del "cropBox"
			viewMode: 2,
			// Establecer el recorte inicial del cropBox
			autoCrop: true,

			ready: function(e){
				// Inicializar el cropboxdata en las posiciones elegidas
				that.cropper.setCropBoxData({
					"top": parseInt(that.Pos_Y_0),
					"left": parseInt(that.Pos_X_0),
					"width": parseInt(that.Pos_X_1 - that.Pos_X_0),
					"height": parseInt(that.Pos_Y_1 - that.Pos_Y_0),
				});
				that.isReady = true;
			},

			crop: function (e) {
				// Establecer los límites establecidos del recorte de la imagen
				if (that.isReady == true){
					$('#ImageFavicon_Pos_X_0').val(Math.round(e.detail.x));
					$('#ImageFavicon_Pos_Y_0').val(Math.round(e.detail.y));								
					$('#ImageFavicon_Pos_X_1').val(Math.round(e.detail.x) + Math.round(e.detail.width));
					$('#ImageFavicon_Pos_Y_0').val(Math.round(e.detail.y) + Math.round(e.detail.height));
				}				
			}
		};
		this.originalImageURL = this.$image.attr('src');
		this.uploadedImageName = 'cropped_favicon.jpg';
		this.uploadedImageType = 'image/jpeg';
		this.uploadedImageURL;
	},

	engancharJcrop : function(){
		const that = this;
		// Creación del cropper
		this.cropper = new Cropper(that.$image[0], that.options);	
	},	
	
	configEvents: function(){
	
	},
}

/**
 * Método para subir una imagen a una dirección proporcionada.
 */
function uploadImageFileFromInput(input, completion = undefined){
	
	//var panContenedor = $(input).closest('#contenedorPrincipal_propiedad2_' + idioma);
	//var panImg = $('.panelImg', panContenedor);	

	// Comprobar los ficheros subidos/seleccionados
	if (input.prop('files') && input.prop('files')[0]) {
	    const file = input.prop('files')[0];
	    const reader = new FileReader();
	    reader.onload = function (e) {
	        // Ruta del fichero subido
			const imgFile = e.target.result;			
	        //$('img', panImg).attr('src', imgFile);	
			// Datos del fichero subido
	        const data = imgFile.substr(imgFile.indexOf(';base64,') + 8);
	        //$('#propiedad2_' + idioma, panImg).val("File:" + file.name + ';Data:' + data);
			completion(data, imgFile, file);
	    }
	    reader.readAsDataURL(file);
	}	
	/*
	var selectorTipo = $('.selectorTipo', panContenedor);
	var subirIMG = $('.subirIMG', panContenedor);
	selectorTipo.hide();
	subirIMG.hide();
	panImg.show();
	*/
    
}


/**
 * Método para configurar una lista sortable normal
 * @param {*} idList Nombre del Id de la lista donde se encuentran los items que van a ser ordenados
 * @param {*} handleSortable Nombre de la clase del botón que servirá para hacer "drag/drop"
 * @param {*} onAddCompletion Función a ejecutarse cuando se complete el arrastre del item
 * @param {*} onChooseCompletion Función a ejecutarse cuando se complete la selección del item
 * @param {*} onUnChooseCompletion Función a ejecutarse cuando se complete la deselección del item
 * @param {*} onEndCompletion Función a ejecutarse cuando finalice el arrastre del item y este cambie de posición
 */
 function setupBasicSortableList (idList, handleSortable, onAddCompletion = undefined, onChooseCompletion = undefined, onUnChooseCompletion = undefined, onEndCompletion = undefined) {
    const sortableList = document.getElementById(`${idList}`);
    // Opciones de configuración de la lista 
    const sortable_options = {
        group: {
            name: `${idList}`,
        },
        sort: true,
        dragoverBubble: true,
        handle: `.${handleSortable}`,
        onAdd: function (evt) { onAddCompletion != undefined && onAddCompletion(evt)},
        onChoose: function (evt) {onChooseCompletion != undefined &&  onChooseCompletion(evt)},
        onUnChoose: function (evt) {onUnChooseCompletion != undefined &&  onUnChooseCompletion(evt)},
		onEnd: function (evt) {onEndCompletion != undefined &&  onEndCompletion(evt)},
    };
    // Aplicar la operativa sortable a la lista con la configuración
    Sortable.create(sortableList, sortable_options);        
}



/**
 * Comportamiento para gestionar el plegado/desplegado de categorías con hijos.  
 * El resto se visualizarán en un dropdown menu.
 * @namespace operativaDesplegarCategorias
 * 
 */

const operativaDesplegarCategorias = {
    init: function () {
        this.config();
        this.mostrarOcultarCategoriasHijas();
    },
    config: function () {
        this.pan_categorias = $('.divTesArbol.divCategorias');
        this.desplegables = this.pan_categorias.find('.boton-desplegar')
    },
    mostrarOcultarCategoriasHijas: function () {
        if (this.desplegables.length > 0) {
            this.desplegables.off('click').on('click', function () {
                $(this).toggleClass('mostrar-hijos');
            });
        }
    },
}

/**
 * Operativa NestedSortable a partir del DragAndDrop propio de jquery. Utilización en conjunción con el plugin NestedSortable
 */
const operativaNestedSortable = {
    
    /**
     * Método para iniciarlizar el comportamiento de inicializar el comportamiento de Ordenar items de una lista
     * @param {*} idItemsList : ID de la lista donde estarán los items a ordenar
     * @param {*} sortableIconClassName : Nombre de la clase del icono el cual se utilizará para ordenar o hacer drag del item
     * @param {*} itemsToDragClassName : Clase del item que será ordenado o arrastrado
     * @param {*} onHandleReorderItemsFinish : completion o callback que se desea ejecutar cuando se finalice el evento de ordenar o arrastrar item
     */
    init: function(idItemsList, sortableIconClassName, itemsToDragClassName, onHandleReorderItemsFinish = null){
        this.initPages(idItemsList, sortableIconClassName, itemsToDragClassName, onHandleReorderItemsFinish);
    },

    initPages: function(idItemsList, sortableIconClassName, itemsToDragClassName, onHandleReorderItemsFinish, allowEscapeActionToCancelNestedSortableAction = false){
        // Obtener la configuración para la NestedSortable
        const configNestedSortable = this.configNestedSortable(sortableIconClassName, itemsToDragClassName, onHandleReorderItemsFinish, allowEscapeActionToCancelNestedSortableAction);        
        // Aplicar el comportamiento NestedSortable
        const ns = $(`#${idItemsList}`).nestedSortable(configNestedSortable);
    },

	/**
     * Método para iniciarlizar el comportamiento de inicializar el comportamiento de Ordenar items de una lista. Este método contiene una propiedad por si se desea que exista la opción de
	 * pulsar la tecla de "ESC" para que se cancele la operación 
     * @param {*} idItemsList : ID de la lista donde estarán los items a ordenar
     * @param {*} sortableIconClassName : Nombre de la clase del icono el cual se utilizará para ordenar o hacer drag del item
     * @param {*} itemsToDragClassName : Clase del item que será ordenado o arrastrado
     * @param {*} onHandleReorderItemsFinish : completion o callback que se desea ejecutar cuando se finalice el evento de ordenar o arrastrar item
	 */
	initAllowingEscapeButtonToCancelNestedSortableAction: function(idItemsList, sortableIconClassName, itemsToDragClassName, onHandleReorderItemsFinish = null){
        this.initPages(idItemsList, sortableIconClassName, itemsToDragClassName, onHandleReorderItemsFinish, true);
    },	

    /**
     * Método para configurar el elemento de "NestedSortable"
     * @returns 
     */
    configNestedSortable: function(sortableIconClassName, itemsToDragClassName, onHandleReorderItemsFinish, allowEscapeActionToCancelNestedSortableAction = false){

        // Posición Top y Derecha del item arrastrado para controlar si hace falta moverlo si se suelta en la misma posición
        let startItemTop = "";
        let startItemLeft = "";
        let stopItemTop = "";
        let stopItemLeft = "";

        const nestedSortableConfiguration = {
            listType: "ul",
            forcePlaceholderSize: true,
            handle: `.${sortableIconClassName}`,
            items: `.${itemsToDragClassName}`,   // `.${itemsToDragClassName}`,             // '.page-row',
            opacity: .6,
            placeholder: 'ui-state-highlight',
            revert: 250,
            tabSize: 25,
            tolerance: 'pointer',
            toleranceElement: '> div',
            //maxLevels: 4,*/
            isTree: true,
            expandOnHover: 700,
            //startCollapsed: false,
            start: function(event, ui){
                const offsetTopItemMoved = ui.item[0].offsetTop;
                startItemTop = offsetTopItemMoved;
                const offsetLeft = ui.item[0].offsetLeft;
                startItemLeft = offsetLeft;
            },
            stop: function(event, ui){
                // Elemento movido
                const $itemEl = $(ui.item);

				// Si no existe la opción de poder pulsar en la tecla "ESC" para cancelar la acción continuar con la operativa
				if (allowEscapeActionToCancelNestedSortableAction == false){
					if (onHandleReorderItemsFinish != null){					
                    	onHandleReorderItemsFinish($itemEl);
                	}  
				}else{
					// Existe la operativa de si se pulsa en botón "ESC" durante la ordenación, controlar si se cancela o no.					    
					const offsetTopItemMoved = ui.item[0].offsetTop;
					stopItemTop = offsetTopItemMoved;
					const offsetLeft = ui.item[0].offsetLeft;
					stopItemLeft = offsetLeft;           
					// Comparar la posición origen con la final del item arrastrado
					if ( startItemTop != stopItemTop || startItemLeft != stopItemLeft){
						// Elemento movido - Continuar con la acción delegada para la ordenación
						if (onHandleReorderItemsFinish != null){					
							onHandleReorderItemsFinish($itemEl);
						}                                       
					} 
				}
            }
        }            
        return nestedSortableConfiguration;
    },
}


//****************************************************************************************************************/
// Configuración de clicks, changes, selección de checks para facilitar el comportamiento JS de los elementos HTML en vistas
/*****************************************************************************************************************/

/**
 * Método para configurar un observer en un elemento determinado para ejecutar un comportamiento cuando exista o se creen en pantalla

 * @param {String} pContainerClassName : Nombre de la clase del elemento HTML que si está en el DOM para aplicar comportamiento. Si es 'undefined', se buscará en el "body" directamente. 
 * @param {Bool} pIsClassNameProvided: Indicar true si se proporciona el nombre de la clase del botón o false si se proporciona el nombre de un identificador del DOM.
 * @param {String} pChildClassOrIdName : Nombre de la clase del elemento hijo que ha sido añadido al DOM. Por defecto undefined. Si se añade, mejor especificado para búsquedas
 * @param {function} pCallback : Devolverá el elemento jquery para poder ejecutar comportamiento jquery
 * @param {Object} pConfigObserver : Objeto JS para configurar cuando o cómo se desea que el Observe se dispare -> Por defecto (undefined) solo con hijos del elemento padre (no con atributos css)
 */
 function setupObserver(		 
	pContainerClassName,
	pIsClassNameProvided,
	pChildClassOrIdName,
	pCallback,
	pConfigObserver = undefined
) {
	// Creación del observador
	let observer = new MutationObserver(function (mutations) {
		mutations.forEach(function (mutation) {
			let newNodes = mutation.addedNodes; // DOM NodeList			
			if (newNodes !== null) {
				// Si se añaden nuevos nodos a DOM
				let $nodes = $(newNodes); // jQuery set
				$nodes.each(function () {
					const $node = $(this);
					/* Comprobar los elementos añadidos y aplicar comportamiento deseado */
					if (pChildClassOrIdName != undefined) {						
						// Comprobar el nodo o subnodo añadido												
						let $specific_nodes = undefined;
						if ($node.hasClass(`${pChildClassOrIdName}`) || $node.is(`#${pChildClassOrIdName}`)){
							// Buscar el nodo por clase o Id
							$specific_nodes = $node;
						}else if($node.children().hasClass(`${pChildClassOrIdName}`) ){
							// Buscar el nodo por clase dentro de un contenedor añadido
							$specific_nodes = $node.children(`.${pChildClassOrIdName}`);						
						}else if($node.find(`#${pChildClassOrIdName}`).length > 0 ){
							// Buscar el nodo por ID dentro de un contenedor añadido
							$specific_nodes = $node.find(`#${pChildClassOrIdName}`);
						}	
						else if ($node.find(`.${pChildClassOrIdName}`).length > 0 ){
							// Buscar el nodo por clase dentro de un contenedor añadido
							$specific_nodes = $node.find(`.${pChildClassOrIdName}`);
						}
						if ($specific_nodes != undefined && $specific_nodes.length > 0){
							$specific_nodes.each(function(){
								const $specific_node = $(this);
								pCallback($specific_node);
							});							
						}					
					}
				});
			}
		});
	});

	// Parámetros de configuración del observador
	let config = "";
	// Configuración del observador
	if (pConfigObserver == undefined) {
		config = {
			// attributes: true, -> Cambios en atributos
			childList: true,
			subtree: true,
			//characterData: true,
		};
	} else {
		config = pConfigObserver;
	}

	// Identificar el contenedor (padre) donde se configurará el Observer. Es posible que no se desea especificar un padre.
	let target = '';
	if (pContainerClassName != undefined){
		target = $(`.${pContainerClassName}`)[0];
	}else{
		target = $(`body`)[0];
	}
	
	if ($(target).length > 0) {
		// Aplicar el comportamiento actual a elementos que ya existen en la página -> No solo para los nuevos elementos creados
		const $nodes = '' ;
		if (pIsClassNameProvided == true){
			$(target).find(`.${pChildClassOrIdName}`);
		}else{
			$(target).find(`#${pChildClassOrIdName}`);
		}		
		pCallback($nodes);

		observer.observe(target, config);
	}
}
/*************************************************/

/**
 * Método para configurar un comportamiento/función (completion) a un determinado botón de forma dinámica no siendo necesario que este exista en el DOM
 * pudiendo asignarse un comportamiento JS cuando este se creen en pantalla según el nombre de la clase.
 * @param {*} pClassName: Nombre de la clase de los elementos del DOM a configurar
 * @param {*} pCompletion: Función o bloque de código que se ejecutará una vez se haga click en el botón
 */
const configEventByClassName = function (pClassName, pCompletion) {

	// Configuración dinámica para nuevos elementos del DOM
	setupObserver(
		undefined,
		true,
		pClassName,
		pCompletion,
		undefined
	);

	// Configuración manual para elementos actuales del DOM
	const $elements = $(`.${pClassName}`);
	$elements.each(function(e) {
		pCompletion($(this));
	});
};

/**
 * Método para configurar un comportamiento/función (completion) a un determinado botón de forma dinámica no siendo necesario que este exista en el DOM
 * pudiendo asignarse un comportamiento JS cuando este se creen en pantalla según el ID del elemento HTML. 
 * @param {*} pId: Id del elemento HTML a configurar 
 * @param {*} pCompletion: Función o bloque de código que se ejecutará una vez se haga click en el botón
 */
 const configEventById = function (pId, pCompletion) {

	// Configuración dinámica para nuevos elementos del DOM
	setupObserver(
		undefined,
		false,
		pId,
		pCompletion,
		undefined
	);
		
	// Configuración manual para elementos actuales del DOM
	const $elements = $(`#${pId}`);
	$elements.each(function(e) {
		pCompletion($(this));
	});
};

/**
 * Método para configurar un comportamiento/función (completion) a un determinado elemento no siendo necesario que este exista en el DOM
 * pudiendo asignarse un comportamiento JS cuando este se creen en pantalla según el ID del elemento HTML. En este caso, se pasa un objeto para identificar su id
 * @param {*} pId: Id del elemento HTML a configurar 
 * @param {*} pCompletion: Función o bloque de código que se ejecutará una vez se haga click en el botón
 */
 const configEventByJqueryObject = function (pJqueryObject, pCompletion) {

	const id = pJqueryObject.attr("id");
	// Configuración dinámica para nuevos elementos del DOM
	setupObserver(
		undefined,
		false,
		id,
		pCompletion,
		undefined
	);
		
	// Configuración manual para elementos actuales del DOM
	const $elements = $(`#${id}`);
	$elements.each(function(e) {
		pCompletion($(this));
	});
};


/**
 * Método para guardar la selección de un checkbox para separarla entre comas y guardarla en un input hidden para un posterior tratamiento/envío
 * a backEnd. El input es pasado como objeto jqueryObject.
 * @param {jqueryObject} $pCheck : El checkbox que se ha seleccionado/deseleccionado en fomato jqueryObjec.
 * @param {jQueryObject} $pInputGuardarSeleccion : El input hidden en formato jquery donde se almacenarán los diferentes inputs seleccionados en formato jqueryObject.
 * @param {String} pClave : El valor o la clave del item seleccionado. Será el valor que se mande a backEnd. Este valor será el almacenado en el input hidden.
 * @param {String} pDataValue : Sección del atributo para poder acceder al valor del item seleccioando. Ej: <input data-value="123"/> En este caso será value. Value será por defecto. 
 */
const marcarElementoCheckboxYGuardarloEnInputJquery = function ($pCheck, $pInputGuardarSeleccion, pDataValue = "value") {    
	// Atributo o valor del input seleccionado/deseleccionado	
	const valor = $pCheck.data(pDataValue);	

    if ($pCheck.is(':checked')) {
        $pInputGuardarSeleccion.val($pInputGuardarSeleccion.val() + valor + ',');
    }
    else {
        $pInputGuardarSeleccion.val($pInputGuardarSeleccion.val().replace(valor + ',', ''));
    }
}

/**
 * Método para guardar la selección de un checkbox para separarla entre comas y guardarla en un input hidden para un posterior tratamiento/envío
 * a backEnd. El input es pasado como objeto jqueryObject.
 * @param {String} pCheck : El checkbox que se ha seleccionado/deseleccionado en formato String
 * @param {String} $inputGuardarSeleccion : El input hidden es donde se almacenarán los diferentes inputs seleccionados en formato String.
 * @param {String} pClave : El valor o la clave del item seleccionado. Será el valor que se mande a backEnd. Este valor será el almacenado en el input hidden.
 */
 const marcarElementoCheckboxYGuardarloEnInput = function (pCheck, pInputGuardarSeleccion, pDataValue = "value") {    
	const $pInputGuardarSeleccion = $(`#${pInputGuardarSeleccion}`);
	const $pCheck = $(`#${pCheck}`);

    if ($pCheck.is(':checked')) {
         $pInputGuardarSeleccion.val( $pInputGuardarSeleccion.val() + pClave + ',');
    }
    else {
         $pInputGuardarSeleccion.val( $pInputGuardarSeleccion.val().replace(pClave + ',', ''));
    }
}


//****************************************************************************************************************/
// Configuración / trabajo con el LocalStorage
/*****************************************************************************************************************/

/**
 * Método para comprobar si está disponible el local storage del navegador
 * @param {any} type
 */
 const storageAvailable = function (type) {
    try {
        var storage = window[type],
            x = '__storage_test__';
        storage.setItem(x, x);
        storage.removeItem(x);
        return true;
    }
    catch (e) {
        return e instanceof DOMException && (
            // everything except Firefox
            e.code === 22 ||
            // Firefox
            e.code === 1014 ||
            // test name field too, because code might not be present
            // everything except Firefox
            e.name === 'QuotaExceededError' ||
            // Firefox
            e.name === 'NS_ERROR_DOM_QUOTA_REACHED') &&
            // acknowledge QuotaExceededError only if there's something already stored
            storage.length !== 0;
    }
}

//****************************************************************************************************************/
// Plugin creado para permitir la eliminación en línea de un elemento o fila que esté dentro de un modal
// con el objetivo de evitar solicitar la confirmación mediante un modal dentro de un modal.
// Al solicitar la eliminación, se mostrará una pequeña capa por encima del item deseado a eliminar para que se confirme su eliminación
// con los botones "Sí | No".
// Para que funcione, bastará con indicar la clase "containerConfirmDeleteItemInModal" en el padre o elemento contenedor donde se desea
// mostrar el mensaje de confirmación y añadir la clase "confirmDeleteItemInModal" al botón que disparará este comportamiento.
// Ejemplo actual de uso: Eliminación de elementos secundarios de una ontología secundaria (dentro del modal).
/*****************************************************************************************************************/
;(function ($) {
	$.confirmDeleteItemInModal = function (element, options) {
		// Opciones por Defecto del plugin
		// Propiedades privadas y accesibles desde el plugin
		var defaults = {
			confirmDeleteMessage: "¿Deseas eliminar el elemento seleccionado?",
			confirmYesOption: "Sí",
			confirmNoOption: "No",			
			parentContainerClassName: "containerConfirmDeleteItemInModal",

			// Método (callback) una vez se ha confirmado la eliminación del elemento
			onConfirmDeleteMessage: function () {},
		};

		// Uso de "plugin" para referenciar a la instancia del objeto
		const plugin = this;

		// Mantener el valor por defecto fusionado, y las opciones proporcionadas por el usuario
		// Propiedades del plugin disponibles a través de este objeto como:
		// plugin.settings.propertyName desde dentro del plugin o
		// element.data('pluginName').settings.propertyName desde fuera del plugin, donde "element" es el
		// elemento al que está vinculado el plugin;
		plugin.settings = {};

		var $element = $(element), // reference to the jQuery version of DOM element the plugin is attached to
			element = element; // reference to the actual DOM element

		// Constructor/metodo llamado cuando es creado el objeto when the object is created
		plugin.init = function () {
			// Propiedades finales del complemento combinadas por defecto y las proporcionadas por el usuario (si las hay)
			plugin.settings = $.extend({}, defaults, options);
			// Inicialización de comportamientos (clicks, mouseOvers) para cada elemento
			config($element);
		};


		// Métodos privados		
		/**
		 * Método para configurar eventos sobre los items del componente
		 * @param {*} $element 
		 */
		var config = function ($element) {
			// Detectar y asignar el contenedor padre del elemento Html para posicionamiento del mensaje de confirmación junto con los botones de acción
			getHtmlParent();

			$element.on({
				// Click
				click: function (event) {
					// Contenedor donde se añadirá el mensaje
					const container = plugin.settings.$htmlParent;	
					addConfirmDeletionMessageInContainer(container);				
				},				
			});
		};

		/**
		 * Método que añadirá la funcionalidad de una capa en el contendor donde se mostrará el mensaje
		 * @param {*} container Capa que hará de contenedor donde se añadirá esta capa para confirmar el borrado
		 */
		var addConfirmDeletionMessageInContainer = function(container){
			container.addClass("position-relative");
			const confirmDeleteItemInModal = `
			<div class="plugin__confirmDeleteItemInModal" style="opacity:0;">
				<span class="confirmDeleteItemInModal__label">${plugin.settings.confirmDeleteMessage}</span>
				<div class="confirmDeleteItemInModal__actions">
					<button class="btn btn-outline-primary confirmNoOption"">${plugin.settings.confirmNoOption}</button>
					<button class="btn btn-primary confirmYesOption">${plugin.settings.confirmYesOption}</button>
				</div>		
			</div>`						
					
			// Añadir el panel de confirmación
			container.append(confirmDeleteItemInModal);
			setTimeout(function(){				
				container.find(".plugin__confirmDeleteItemInModal").fadeTo(300, 1);
			},100);

			// Asignación de acciones a los botones
			container.find(".confirmNoOption").on("click", function(){
				dismissDeleteItemInModal($(this))
			});
			container.find(".confirmYesOption").on("click", function(){
				// Callback a ejecutar mandado por quien ha llamado al plugin				
				plugin.settings.onConfirmDeleteMessage != null && plugin.settings.onConfirmDeleteMessage();				
			});
		};
		
		/**
		 * Método para hacer dismiss el mensaje o panel de confirmación del borrado del elemento
		 * @param {*} notConfirmDeleteButton: Botón pulsado para cancelar la eliminación del elemento seleccionado
		 */
		var dismissDeleteItemInModal = function(notConfirmDeleteButton){			
			// Buscar el panel a eliminar a partir del botón seleccionado
			const panelConfirmDeleteItemInModal = notConfirmDeleteButton.closest(".plugin__confirmDeleteItemInModal");
			panelConfirmDeleteItemInModal.fadeOut(300, function() {
				$(this).remove();
			});			
		}

		/**
		 * Encontrar el padre o contenedor del elemento donde se desea mostrar el aviso para confirmar la eliminación
		 * La clase se almacenará en parentContainerClassName
		 */
		var getHtmlParent = function () {			
			if (plugin.settings.parentContainerClassName != undefined) {
				// Buscar el padre del por el nombre de la clase
				plugin.settings.$htmlParent = $element.closest(`.${plugin.settings.parentContainerClassName}`);
			} else {
				// Buscar el padre del elemento directamente
				plugin.settings.$htmlParent = $element.closest(`.${plugin.settings.parentContainerClassName}`);
			}
		};				

		// Inicio del plugin
		// Llamada del constructor
		plugin.init();
	};

	// Añadir el plugin al objeto jQuery.fn
	$.fn.confirmDeleteItemInModal = function (options) {
		// Iteración por cada elemento del DOM para asignarle el Plugin
		return this.each(function () {
			// if plugin has not already been attached to the element
			if (undefined == $(this).data("confirmDeleteItemInModal")) {
				// ´Creación nueva instancia del plugin
				// Pasar el elemento del DOM y asignarle los argumentos proporcionados por el usuario
				const plugin = new $.confirmDeleteItemInModal(this, options);
				$(this).data("confirmDeleteItemInModal", plugin);
			}
		});
	};
})(jQuery);



//****************************************************************************************************************/
// Clase singleton para controlar la descarga de ficheros CSS personalizados para la visualización de estilos personalizados
// dentro del ckEditor.
/*****************************************************************************************************************/

const CkEditorCustomStyleSheets = (function () {
	// Instancia única de la clase
	let instance; 

	// Constructor
	function CkEditorCustomStyleSheets() {
		// Contenedor del ckEditor
		this.parentContainer = undefined;		
		// Indicador de que el proceso de descarga del custom stylesheets está en proceso
		this.isDownloadingCustomStyleSheet = false;
		// Indicador de que ya se han descargado los ficheros de estilos personalizados para evitar realizar una segunda petición
		this.isDownloadedCustomStyleSheet = false;
		// Lista de los ficheros de estilos personalizados
		this.customStyleSheetList = [];
	}

	/**
	 * Método para establecer la lista de estilos a la clase CKEditor
	 * @param {*} pStyleSheetList 
	 */
	CkEditorCustomStyleSheets.prototype.setCustomStyleSheetList = function (pStyleSheetList) {
		instance.customStyleSheetList = pStyleSheetList;
	};

	/**
	 * Método para indicar que la descarga se ha realizado correctamente
	 * @param {bool} pValue Valor booleano a establecer en la propiedad
	 */
	CkEditorCustomStyleSheets.prototype.setDownloadedCustomStyleSheet = function (pValue) {
		instance.isDownloadedCustomStyleSheet = pValue;
	};

	/**
	 * Método para indicar que se está realizando la descarga o que la petición para realizar la descarga está en proceso
	 * @param {bool} pValue Valor booleano a establecer en la propiedad
	 */
	CkEditorCustomStyleSheets.prototype.setDownloadingCustomStyleSheet = function (pValue) {
		instance.isDownloadingCustomStyleSheet = pValue;
	};	
	
	/**
	 * Método para realizar la petición a la url correspondiente para obtener la vista que contiene los estilos personalizados
	 * @param {*} pUrlRequest Url a la que se va a realizar la petición (API) para la obtención de los ficheros personalizados
	 * @param {*} pCkeditorConfigFile Fichero config de la instancia del ckEditor 
	 */
	CkEditorCustomStyleSheets.prototype.getCustomStyleSheetsFromBackoffice = function (pUrlRequest, pCkeditorConfigFile, completion) {
		// Indicar que se está realizando la descarga				
		instance.isDownloadingCustomStyleSheet = true;
		//loadingMostrar();
		if (instance.isDownloadedCustomStyleSheet){
			instance.isDownloadingCustomStyleSheet = false;
			completion(false);
			return;
		};
		
		// Realizar la eptición
		GnossPeticionAjax(                
			pUrlRequest,
			null,
			true
		).done(function (data) {
			const htmlHeadCode = createElementFromHTML(data);
			const linkElements = $(htmlHeadCode).find("link");
			// Añadir los estilos personalizados obtenidos
			if (linkElements.length > 0){
				const cssFilesValue = $.map(linkElements, function(item) {
					const link = $(item).prop("href");
					return link;
				});				
				instance.customStyleSheetList = [...pCkeditorConfigFile.contentsCss, ...cssFilesValue];							
			}
			instance.isDownloadedCustomStyleSheet = true;
			instance.isDownloadingCustomStyleSheet = false;				
			completion(true);
		}).fail(function (data) {
			return null;
		}); 
	};	
		
	return {
		getInstance: function () {
		  if (!instance) {
			instance = new CkEditorCustomStyleSheets();
		  }
		  return instance;
		},
	};	
  })();


