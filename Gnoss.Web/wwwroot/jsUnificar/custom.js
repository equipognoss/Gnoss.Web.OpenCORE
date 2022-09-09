$( function() {
    $('input.noLabel[type=text]').focus( function() {
        var $this = $(this);
        this.cache = this.cache || $this.val(); // si la cache del input esta vacia la llenamos
        $this.val('');
    }).blur( function() {
        this.value = this.value || this.cache;
    });
    $('input.password.noLabel').focus( function() {
        $(this).addClass('noBgImg');
    }).blur( function() {
    if (!this.value) {
            $(this).removeClass('noBgImg');
        }
    });
    
    // Correccion bordes de input para IE
    $('input').filter('[type=radio], [type=checkbox]').css({
		border: 0,
		padding: 0,
		marginTop: 0
    });

    /* Definimos globalmente los desplegables.
     * Para montar un desplegable basta con definir un enlace que sirva de boton (a.desplegable)
     * y una capa con clase panel (div.panel). OJO, si se usa cualquiera de los dos elementos fuera
     * de su estructura preparada se producira un error de JS y un alert(...)
     */
	 
	//LOZA : Para los desplegables, es recomendable usar la funcion "Desplegar(Boton, PanelId)" en el onclick del enlace que despliega. (Ver ejemplo en BandejaBorradores.aspx)
    //crearDesplegables('a.desplegable', 'div.panel',{efecto:'blind', opciones:{direction:'vertical'}});
    crearDesplegables('#desplegarMenu', '#menuLateral',{velocidad: 300});
    //crearPestanyas('ul.pestanyas li', 'div.pestanya',{efecto:'slide', velocidad:600, opciones:{direction:'up'}})
	
	// cambiar la url mediante el select
	$('select.cambiarListado').change(function(){
		window.location=this.value;
	});
	
	// filtros de busqueda rapida
	$('.filtroRapido').find('input').keyup( function() {
		var $this = $(this);
		if (this.value.length > 2) {
			$this.addClass('activo');
		} else {
			$this.removeClass('activo');
		}
	});
	
	// duplicar campos de registro de organizacion
	$('div.agregarCampo').find('a').click(function() {
		var $campo = $(this).parents('div.agregarCampo').eq(0).find('input:last');
		$campo.clone().insertAfter($campo);
		return false;
	});
	
	// PSEUDO FINDER
	$.fn.extend({
		reajustar:function(){// funcion interna para recalcular las alturas del finder y el scroll interno
		    return this.each(function(){
		    	var h=0;
		    	var $mascara=$(this);
		    	var anchura = $mascara.children('div').width()-1;
		    	var niveles=$mascara.find('li.activo').length||1;
		    	$mascara.find('div:visible').each(function() {
		    		var hTemp=0;
		    		$(this).children().each( function() {
		    			var $this=$(this);
		    			hTemp+=parseFloat($this.css('paddingTop'));
		    			hTemp+=parseFloat($this.height());
		    			hTemp+=parseFloat($this.css('paddingBottom'));
		    		});
		    		h=(hTemp>h)?hTemp:h;
		    	});
			    $mascara.animate({height:h+'px'},200)
			    .children('div').eq(0).animate({left:-anchura*(niveles-1)+'px'}, 600);
			    if (niveles > 1) {
			    	$mascara.find('a.volver').fadeIn(200);
			    } else {
			    	$mascara.find('a.volver').fadeOut(200);
			    }
		    });
		},
	    desactivar:function(){
	    	return this.each(function(){
	    		$(this).find('li').andSelf().removeClass('activo')
	    		    .children().removeClass('activo')
	    		    .siblings('div').hide();
	    	});
	    }
	});
	$('div.pseudoFinder').each(function() {
		var $mascara = $(this);
		// mostrar un nivel inferior
		$mascara.find('div').siblings('a:not(.volver)').click( function() {
			var $li = $(this).parent();
			var $div = $li.children('div').show();
			if ($li.hasClass('activo')) return false; //si ya esta activado pasamos de todo
			// desactivamos los hermanos, reasignamos clases y reajuste de la capa
			$li.siblings('.activo').desactivar();
			$li.children('a').andSelf().addClass('activo');
			$mascara.reajustar();
			return false;
		});
		// enchufar en el target y realizar accion final
		$mascara.find('a:only-child').css('backgroundImage','none').click(function(){
			var sHtml;
			var prime=$mascara.find('a.activo:first').text();
			var ulti=$(this).text();
			prime=(prime.length>50)?prime.substring(0,47)+'...':prime;
			ulti=(ulti.length>50)?ulti.substring(0,47)+'...':ulti;
			sHtml=['<tr><th scope="row">',prime,'</th><td>',ulti,'</td><td><img src="img/blank.gif" alt="eliminar"/></td></tr>'].join('');
			$mascara.next('.targetPseudoFinder').show() //mostramos
			.find('tbody').append(sHtml) //metemos el HTML
			.find('img:last').click(function(){ // preparamos el evento de eliminar
				var $capa=$(this).parents('div').eq(1);
				$(this).parents('tr:first').remove();
				if (!$capa.find('tr').length) {
					$capa.slideUp();
				}
			});
			return false;
		});
		// volver atras
		$mascara.find('a.volver').click(function(){
			$mascara.find('li.activo:last').desactivar().end().reajustar();
			return false;
		});
	});
	
	// selector de #baseRecursos
	$('#baseRecursos div.listadoCategorias a').click(function() {
		$('#baseRecursos div.listadoCategorias a').removeClass('activo');
		$(this).addClass('activo');
		return false;
	});
	
	
	// desplegables de base de recursos
	$('#baseRecursos h3+div.panel a').click(function(){
		$(this).parents('.panel:first').prev().find('a').click();
	});
	
	// inputs condicionados a un select de "tipo de documentos" en el apartado de subir recursos
	$('#seleccionarRecurso,#tipoRecurso').find('select:eq(0)').change(function() {
		var $this = $(this);
		$this.find('option').each(function(){
			$('#'+this.value).hide();
		});
		$('#'+$this.val()).show();
	}).change();
	
	// CURRICULUM VITAE
	var checkearDatosMyGnoss = function(){
		var $this = $('#datosMyGnoss');
		if ($this.is(':checked')) {
			$this.parents('.cajaDestacadaLila:first').find('input:text').attr('disabled','disabled');
		} else {
			$this.parents('.cajaDestacadaLila:first').find('input:text').attr('disabled',false);
		}
	}
	// asignamos el evento al cargar y al pinchar en el checkbox
	$('#datosMyGnoss').click(checkearDatosMyGnoss);
	checkearDatosMyGnoss();
});

function Desplegar(pBoton, pPanel) {
    var boton   = $(pBoton),
    panel = $(document.getElementById(pPanel));
	boton.toggleClass('activo');
	panel.toggle();
	return false;
}

function MostrarImgFactorDafo(pPanel, pBaseURL, pClaveDafo, pClaveFactorDafo, pEsFactorNuevo, pRandom)
{
	panel = document.getElementById(pPanel + '_panel');
	if(panel.className == "noCargado")
	{
		img = document.getElementById(pPanel + '_grafico');
		var srcImg = pBaseURL + "/DafoFactorGraficoVotos.aspx?DafoID=" + pClaveDafo + "&FactorID=" + pClaveFactorDafo + "&FactorNuevo=" + pEsFactorNuevo + "&nocache=" + pRandom;
		img.src = srcImg;
		panel.className = "cargado"
	}
	DesplegarPanel(pPanel + '_panel');
}

function DesplegarTreeView(pImagen, pPanel) {
    var imagen   = $(pImagen);
    if(pImagen.src.indexOf('verMas')>0){
        pImagen.src = pImagen.src.replace('verMas','verMenos');
    }
    else{
        pImagen.src = pImagen.src.replace('verMenos','verMas');
    }
    DesplegarPanel(pPanel);
}

function marcarDespTreeView(pImagen,pIdTxt,pClave){   
    mTxt = document.getElementById(pIdTxt);
    if(pImagen.src.indexOf('verMas')>0){
        mTxt.value = mTxt.value.replace(pClave + ',','');
    }
    else{
        mTxt.value = mTxt.value + pClave + ',';
    }
}

function marcarElementoTreeView(pCheck, pIdTxt, pClave) {
    mTxt = $('#' + pIdTxt);

    if ($(pCheck).is(':checked')) {
        mTxt.val(mTxt.val() + pClave + ',');
    }
    else {
        mTxt.val(mTxt.val().replace(pClave + ',', ''));
    }
}

function marcarElementoSelCat(pCheck,pIdTxt,pClave){
    marcarElementoTreeView(pCheck, pIdTxt, pClave);
    try
    {
        $('#divSelCatLista').find('span.'+pClave+' input').attr('checked', $(pCheck).is(':checked'));
        $('#divSelCatTesauro').find('span.' + pClave + ' input').attr('checked', $(pCheck).is(':checked'));
    } 
    catch(Exception){
    }
}

function marcarSoloUnElementoSelCat(pCheck,pIdTxt,pClave){
    marcarElementoTreeView(pCheck, pIdTxt, pClave);

    try {
        $('#divSelCatLista').find('span.' + pClave + ' input').attr('checked', $(pCheck).is(':checked'));
        $('#divSelCatTesauro').find('span.' + pClave + ' input').attr('checked', $(pCheck).is(':checked'));
    }
    catch (Exception) {
    }
}

function marcarElementoSelCatEHijos(pCheck,pIdTxt,pClave)
{
    marcarElementoTreeView(pCheck, pIdTxt, pClave)

    try {
        $('#divSelCatLista').find('span.' + pClave + ' input').attr('checked', $(pCheck).is(':checked'));
        $('#divSelCatTesauro').find('span.' + pClave + ' input').attr('checked', $(pCheck).is(':checked'));
    }
    catch (Exception) {
    }
    
    var hijos = $(pCheck.parentNode.parentNode).children('div').children('div').children('span').children('input');
    hijos.each(
        function (){
            if ($(pCheck).is(':checked') != $(this).is(':checked')) {
                this.click();
            }
        }
    )
}

function filtrarListaSelCat(txt){
    cadena = txt.value;

    if(cadena.length>2){
        //Volvemos a inicializar todo
        try
        {
            for(i=0;i<CatEscondidas.length;i++){
                CatEscondidas[i].style.display = 'block';
            }
        }
		catch(error)
		{
		}
        CatEscondidas = new Array();
        txt.className = "activo";
        //filtramos y guardamos los div que escondemos
        categorias = $('div#divSelCatLista div div');
        for(i=0;i<categorias.length;i++){
            cat = categorias[i];
            if(cat.style.display != 'none' && $(cat).find('span label')[0].innerHTML.toLowerCase().indexOf(cadena.toLowerCase())<0){
                CatEscondidas[CatEscondidas.length] = cat;
                cat.style.display = 'none';
            }
        }
        
    }
    else{
        //volvemos a mostrar los contactos escondidos
            for(i=0;i<CatEscondidas.length;i++){
                CatEscondidas[i].style.display = 'block';
            }
        CatEscondidas = new Array();
        txt.className = "";
    }
}

/**
 * Método para visualizar u ocultar los radioButtons dependiendo del checkbox pulsado (UsuariosOrganizacion -> _AccionesProyOrg)
 * @param {any} pCheck
 * @param {any} pClave
 */
function marcarElementosSelProy(pCheck, pClave) {

	//var radioButtons = $('input:radio[name=tipo_' + pClave + ']', $(pCheck).closest('.proyectos'));
	// Cambiado por nuevo Front
	var radioButtons = $('input:radio[name=tipo_' + pClave + ']');
	var radio1 = radioButtons[0];
	var radio2 = radioButtons[1];

	if ($(pCheck).is(':checked')) {
		if (!$(radio1).is(':checked') && !$(radio2).is(':checked')) {
			$(radio1).prop('checked', true);
		}
		$(radio1).parent().removeClass("d-none");
		$(radio2).parent().removeClass("d-none");
	}
	else {
		$(radio1).parent().addClass("d-none");
		$(radio2).parent().addClass("d-none");
	}

}

function filtrarListaSelProy(txtBox){    
    filtro = txtBox.value;
    filtro=filtro.toLowerCase();
    filtro.replace(/á/g,'a').replace(/é/g,'e').replace(/í/g,'i').replace(/ó/g,'o').replace(/ú/g,'u');
    if(filtro.length>2){
        txtBox.className = "activo";        
        proyectos=$('tr.proyectos');       
        for(i=0;i<proyectos.length;i++)
        {
            proy = proyectos[i]; 
            textoProy=$(proy).find('td span')[0].innerHTML.toLowerCase().replace(/á/g,'a').replace(/é/g,'e').replace(/í/g,'i').replace(/ó/g,'o').replace(/ú/g,'u');
            if(textoProy.indexOf(filtro.toLowerCase())<0){
                proy.style.display = 'none';
            }else
            {
                proy.style.display = '';
            }
        }
    }else
    {
        txtBox.className = "";
        proyectos=$('tr.proyectos');
        for(i=0;i<proyectos.length;i++)
        {
            proyectos[i].style.display='';
        }
    }
}


function marcarElementosSelGrupAmigos(pCheck,pClave,pIdTxt,pSoloUnCheck){
    grupos=$('span.checkGrupo input');
    var txtAmiSel=document.getElementById(pIdTxt);
    if(pSoloUnCheck=='True')
    {
        var estaCheck=false;
        
        if ($(pCheck.childNodes[0]).is(':checked')) {
            estaCheck=true;
        }
        
        for(i=0;i<grupos.length;i++) {
            $(grupos[i]).attr('checked', false);
        }
        txtAmiSel.value="";
        $(pCheck.childNodes[0]).attr('checked', estaCheck);
    }
    if ($(pCheck.childNodes[0]).is(':checked')) {
        txtAmiSel.value=txtAmiSel.value + pClave + '|';
    }else
    {
        txtAmiSel.value = txtAmiSel.value.replace(pClave + '|','');
    }
}


function getElementPosition(elemID) {
    var offsetTrail = document.getElementById(elemID);
    var offsetLeft = 0;
    var offsetTop = 0;
    while (offsetTrail)
    {
        offsetLeft += offsetTrail.offsetLeft;
        offsetTop += offsetTrail.offsetTop;
        offsetTrail = offsetTrail.offsetParent;
    }
    if (navigator.userAgent.indexOf("Mac") != -1 && typeof document.body.leftMargin != "undefined" && navigator.appName=="Microsoft Internet Explorer" ) 
    {
        offsetLeft += parseInt(document.body.leftMargin);
        offsetTop += parseInt(document.body.topMargin);
    }
    return {left:offsetLeft, top:offsetTop};
} 

/**********  REGION JAVIER  **************/

function CheckDocEntidadPrimerNivel_Click(pElementoID, pTxtSeleccionadosID, pTxtCatDocumentacionID)
{
    if ($('#catPrimerNivel_' + pElementoID).is(':checked'))
    {
        DesmarcarDesHabilitarElementosPrimerNivelMenosUno(pElementoID, pTxtSeleccionadosID);
        
        document.getElementById(pTxtCatDocumentacionID).value = pElementoID;
    }
    else
    {
        var check = document.getElementById('catPrimerNivel_' + pElementoID);
        DesmarcarHijosElemento(check, pTxtSeleccionadosID);
        
        HabilitarTodosLosElementosArbol();
        
        document.getElementById(pTxtCatDocumentacionID).value = '';
    }
}

function DesHabilitarElementosNoSeleccionados(pElementoID)
{
    if (pElementoID != '00000000-0000-0000-0000-000000000000')
    {
        var checks = $('#divSelCatTesauro').find('input.catPrimerNivel');
        var i=0
        for (i=0;i<checks.length;i++)
        {
            if (checks[i].id.indexOf(pElementoID) == -1)
            {
                DesHabilitarElementoEHijos(checks[i]);
            }
        }
    }
}

function DesHabilitarElementoEHijos(pCheck)
{
    pCheck.disabled = true;
            
    var hijos = $(pCheck.parentNode.parentNode).children('div').children('div').children('span').children('input');
    var i =0;
    for (i=0; i<hijos.length;i++)
    {
       DesHabilitarElementoEHijos(hijos[i]);
    } 
}

function DesmarcarHijosElemento(pCheck, pTxtSeleccionadosID)
{
    var idElemento = pCheck.parentNode.className;
    $(pCheck).attr('checked', false);
    $('#' + pTxtSeleccionadosID).val($('#' + pTxtSeleccionadosID).val().replace(idElemento + ',', ''));
    
    var hijos = $(pCheck.parentNode.parentNode).children('div').children('div').children('span').children('input');
    var i =0;
    for (i=0; i<hijos.length;i++)
    {
       DesmarcarHijosElemento(hijos[i], pTxtSeleccionadosID);
    }
}

function DesmarcarDesHabilitarElementoEHijos(pCheck, pTxtSeleccionadosID)
{
    $(pCheck).attr('checked', false);
    $(pCheck).attr('disabled', true);

    var idElemento = pCheck.parentNode.className;

    $('#' + pTxtSeleccionadosID).val($('#' + pTxtSeleccionadosID).val().replace(idElemento + ',', ''));
            
    var hijos = $(pCheck.parentNode.parentNode).children('div').children('div').children('span').children('input');
    var i =0;
    for (i=0; i<hijos.length;i++)
    {
       DesmarcarElementoEHijos(hijos[i], pTxtSeleccionadosID);
    } 
}

function HabilitarTodosLosElementosArbol()
{
    var checks = $('#divSelCatTesauro').find('input.catPrimerNivel');
    for (i=0;i<checks.length;i++)
    {
        HabilitarElementoEHijos(checks[i]);
    }
}

function HabilitarElementoEHijos(pCheck)
{
    pCheck.disabled = false;
    
    var idElemento = pCheck.parentNode.className;    
    var hijos = $(pCheck.parentNode.parentNode).children('div').children('div').children('span').children('input');
    var i =0;
    for (i=0; i<hijos.length;i++)
    {
       HabilitarElementoEHijos(hijos[i]);
    } 
}

function DesmarcarDesHabilitarElementosPrimerNivelMenosUno(pElementoID, pTxtSeleccionadosID){
    var checks = $('#divSelCatTesauro').find('input.catPrimerNivel');
    var i=0
    for (i=0;i<checks.length;i++)
    {
        if (checks[i].id.indexOf(pElementoID) == -1)
        {
            DesmarcarDesHabilitarElementoEHijos(checks[i], pTxtSeleccionadosID);
        }
    }
}

function DesmarcarDesHabilitarElementoEHijos(pCheck, pTxtSeleccionadosID)
{
    $(pCheck).attr('checked', false);
    $(pCheck).attr('disabled', true);

    var idElemento = pCheck.parentNode.className;
    $('#' + pTxtSeleccionadosID).val($('#' + pTxtSeleccionadosID).val().replace(idElemento + ',', ''));
            
    var hijos = $(pCheck.parentNode.parentNode).children('div').children('div').children('span').children('input');
    var i =0;
    for (i=0; i<hijos.length;i++)
    {
       DesmarcarDesHabilitarElementoEHijos(hijos[i], pTxtSeleccionadosID);
    } 
}


/* ****    FIN REGION JAVIER   ***********/


//    if(cadena.length>2){
//        //Volvemos a inicializar todo
//            for(i=0;i<CatEscondidas.length;i++){
//                CatEscondidas[i].style.display = 'block';
//            }
//        CatEscondidas = new Array();
//        txt.className = "activo";
//        //filtramos y guardamos los div que escondemos
//        categorias = $('div#divSelCatLista div div');
//        for(i=0;i<categorias.length;i++){
//            cat = categorias[i];
//            if(cat.style.display != 'none' && $(cat).find('span label')[0].innerHTML.toLowerCase().indexOf(cadena.toLowerCase())<0){
//                CatEscondidas[CatEscondidas.length] = cat;
//                cat.style.display = 'none';
//            }
//        }
//        
//    }
//    else{
//        //volvemos a mostrar los contactos escondidos
//            for(i=0;i<CatEscondidas.length;i++){
//                CatEscondidas[i].style.display = 'block';
//            }
//        CatEscondidas = new Array();
//        txt.className = "";
//    }






//function DesplegarImgMas(pBoton,pPanel){
//    alert('Despliego');
//    var boton   = $(pBoton),
//    panel = $(document.getElementById(pPanel));
//    var img = boton.children();
//    var img2 = pBoton.firstChild;
//    alert(img2.attributes);    
//    if(panel.attr('style') == 'display: none;'){
//        
//        img.attr({ alt:'-', src:img.attr('src').replace('Mas','Menos') });
//        img.attr('alt') = '-';
//        alert('Cambio de mas a menos');
//    }
//    else{
//        img.attr({ alt:'+', src:img.attr('src').replace('Menos','Mas') });
//        alert('Cambio de menos a mas');
//    }
//    o = {efecto:'blind', opciones:{direction:'vertical'}};
//    if (!o.efecto) {
//        o.efecto = 'blind';
//        o.opciones =  {direction: 'vertical'};
//    }
//    
//			boton.toggleClass('activo');
//            panel.toggle(o.efecto, o.opciones, o.velocidad, o.callback);
//            return false;
//}

function DesplegarPanel(pPanel) {
            //var $panel = $(document.getElementsByName(pPanel)[0]);
            var $panel = $(document.getElementById(pPanel));
			if ($panel[0].style.display == 'none') {
			    $panel.show();
				//$panel['show']('blind', {direction:'vertical'}, 'fast');
			}
			else {
			    $panel.hide();
				//$panel['hide']('blind', {direction:'vertical'}, 'fast');
			}
			return false;
		}
		
function DesplegarOcultarPaneles(pPanel1, pPanel2) {
        var $panel = $(document.getElementById(pPanel1));
        var $panel2 = $(document.getElementById(pPanel2));
		if ($panel[0].style.display == 'none') {
		    $panel.show();
			//$panel['show']('blind', {direction:'vertical'}, 'fast');
		}
		else {
		    $panel.hide();
			//$panel['hide']('blind', {direction:'vertical'}, 'fast');
		}
		if ($panel2[0].style.display == 'none') {
		    $panel2.show();
			//$panel2['show']('blind', {direction:'vertical'}, 'fast');
		}
		else
		{
			$panel2.hide();
            //$panel2['hide']('blind', {direction:'vertical'}, 'fast');
		}
		return false;
		}
		
function DesplegarImgMas(pBoton,pPanel) {
            var $boton   = $(pBoton);
            var $panel = $(document.getElementById(pPanel));
            var $img = $boton.find('img');
            if($img.length == 0)
            {
                $img = $boton;
            }
            var source = $img.attr('src');
			if ('+' == $img.attr('alt')) {
				func = 'show';
				$img.attr({ alt:'-', src:source.replace('Mas','Menos') });
			} else if ('-' == $img.attr('alt')) {
				func = 'hide';
				$img.attr({ alt:'+', src:source.replace('Menos','Mas') });
			}
            if ($panel[0].tagName.toLowerCase() == 'div') {
				$panel[func]('blind', {direction:'vertical'}, 'fast');				
			} else {
				$panel[func]();
			}
			return false;
		}
		
/*   LOZA : funcion para desplegar el texto tipo ver mas */
function DesplegarDescripcionMas(pBoton,pPanel,pPanelReducido) {
            var $boton   = $(pBoton);
            var $panel = $(document.getElementById(pPanel));
            var $panelReducido = $(document.getElementById(pPanelReducido));
            var $img = $boton.find('img');
            if($img.length == 0)
            var source = $img.attr('src');
			if ('+' == $img.attr('alt')) {
			    //$panelReducido['hide']('blind', {direction:'vertical'}, 'fast');
			    document.getElementById(pPanelReducido).style.display = 'none';
				$panel.show('slow');
			} else if ('-' == $img.attr('alt')) {
			    $panel.hide('slow');
				//$panelReducido['show']('blind', {direction:'vertical'}, 'fast');
				document.getElementById(pPanelReducido).style.display = 'block';
			}
			return false;
}

/*David: Funcion para mostrar y ocultar dos paneles uno debajo de otro*/
function MostrarOcultarPanel(pPanel1,pPanel2)
{
    if(document.getElementById(pPanel1).style.display == 'none')
    {
        document.getElementById(pPanel1).style.display = 'block';
        document.getElementById(pPanel2).style.display = 'none';
    }
    else
    {
        document.getElementById(pPanel1).style.display = 'none';
        document.getElementById(pPanel2).style.display = 'block';
    }
	return false;
}

/*  LOZA : Funcion para desplegar panel desde img +   */
function DesplegarImgMasEnSpan(pBoton, pPanel) {
    var boton   = $(pBoton),
    panel = $(document.getElementById(pPanel));
        o = {efecto:'blind', opciones:{direction:'vertical'}};
    if (!o.efecto) {
        o.efecto = 'blind';
        o.opciones =  {direction: 'vertical'};
    }
    
	boton.toggleClass('activo');
    panel.toggle(o.efecto, o.opciones, o.velocidad, o.callback);
    
    var $botonOtro = $(pBoton)
    var $img = $botonOtro.find('img');
    var source = $img.attr('src');
    if ('+' == $img.attr('alt')) 
    {
		$img.attr({ alt:'-', src:source.replace('Mas','Menos') });
	} else if ('-' == $img.attr('alt')) {
		$img.attr({ alt:'+', src:source.replace('Menos','Mas') });
	}
    return false;
}

function DesplegarImgMasEnSpanSinAnimacion(pBoton, pPanel) {
    panel = (document.getElementById(pPanel));
    if (panel.style.display == 'none')
    {
        panel.style.display = 'inline';
    }
    else
    {
        panel.style.display = 'none';
    }
    
    var $botonOtro = $(pBoton)
    var $img = $botonOtro.find('img');
    var source = $img.attr('src');
    if ('+' == $img.attr('alt')) 
    {
		$img.attr({ alt:'-', src:source.replace('Mas','Menos') });
	} else if ('-' == $img.attr('alt')) {
		$img.attr({ alt:'+', src:source.replace('Menos','Mas') });
	}
    return false;
}


// drag and drop DAFO
function HabilitarDragDrop(){
	$("ol.dragDrop").sortable({
	    stop: function(event, ui) {
		   var padre = $(ui.item[0]).parent(),
		   hijos = padre.children();
		   hijos.removeClass('odd').removeClass('even');
		   hijos.filter(':odd').addClass('even');
		   hijos.filter(':even').addClass('odd');
		   
		   //Modificado por fernando
		   var i = 0;
		   var orden = "";
		   for(i; i<hijos.length; i++)
		   {
				orden += hijos[i].id + ",";
		   }
		   document.getElementById('ctl00_ctl00_CPH1_CPHContenido_txtHackOrdenFactores').value = orden;
		   eval(document.getElementById('ctl00_ctl00_CPH1_CPHContenido_lbHackOrdenFactores').href);
		},
		handle: 'span.dragDrop'
	});
}

InsertarScriptVotacionDafo();

//Inserta el script que cambia los radio buttons de la ficha de votación del dafo
//por el control de votación con los puntos y la X de cancelar voto
function InsertarScriptVotacionDafo()
{
//Sistema de votación DAFO

/*
 ### jQuery Star Rating Plugin v3.12 - 2009-04-16 ###
 * Home: http://www.fyneworks.com/jquery/star-rating/
 * Code: http://code.google.com/p/jquery-star-rating-plugin/
 *
	* Dual licensed under the MIT and GPL licenses:
 *   http://www.opensource.org/licenses/mit-license.php
 *   http://www.gnu.org/licenses/gpl.html
 ###
*/

/*# AVOID COLLISIONS #*/
;if(window.jQuery) (function($){
/*# AVOID COLLISIONS #*/
	
	// IE6 Background Image Fix
	//if ($.browser.msie) try { document.execCommand("BackgroundImageCache", false, true)} catch(e) { };
	// Thanks to http://www.visualjquery.com/rating/rating_redux.html
	
	// plugin initialization
	$.fn.rating = function(options){
		if(this.length==0) return this; // quick fail
		
		// Handle API methods
		if(typeof arguments[0]=='string'){
			// Perform API methods on individual elements
			if(this.length>1){
				var args = arguments;
				return this.each(function(){
					$.fn.rating.apply($(this), args);
    });
			};
			// Invoke API method handler
			$.fn.rating[arguments[0]].apply(this, $.makeArray(arguments).slice(1) || []);
			// Quick exit...
			return this;
		};
		
		// Initialize options for this call
		var options = $.extend(
			{}/* new object */,
			$.fn.rating.options/* default options */,
			options || {} /* just-in-time options */
		);
		
		// Allow multiple controls with the same name by making each call unique
		$.fn.rating.calls++;
		
		// loop through each matched element
		this
		 .not('.star-rating-applied')
			.addClass('star-rating-applied')
		.each(function(){
			
			// Load control parameters / find context / etc
			var control, input = $(this);
			var eid = (this.name || 'unnamed-rating').replace(/\[|\]/g, '_').replace(/^\_+|\_+$/g,'');
			var context = $(this.form || document.body);
			
			// FIX: http://code.google.com/p/jquery-star-rating-plugin/issues/detail?id=23
			//var raters = context.data('rating');
			var raters = ( options.resetAll ? { count:0 } : context.data('rating') || { 
count:0 } );

			if(!raters || raters.call!=$.fn.rating.calls) raters = { count:0, call:$.fn.rating.calls };			
			var rater = raters[eid];
			options.resetAll = false;
			// if rater is available, verify that the control still exists
			if(rater) control = rater.data('rating');
			
			
			if(rater && control)//{// save a byte!
				// add star to control if rater is available and the same control still exists
				
				control.count++;
			//}// save a byte!
			else{
				// create new control if first star or control element was removed/replaced
				// Initialize options for this raters
				control = $.extend(
					{}/* new object */,
					options || {} /* current call options */,
					($.metadata? input.metadata(): ($.meta?input.data():null)) || {}, /* metadata options */
					{ count:0, stars: [], inputs: [] }
				);
				
				// increment number of rating controls
				control.serial = raters.count++;
				
				// create rating element
				rater = $('<span class="star-rating-control"/>');
				input.before(rater);
				
				// Mark element for initialization (once all stars are ready)
				rater.addClass('rating-to-be-drawn');
				
				// Accept readOnly setting from 'disabled' property
				if(input.attr('disabled')) control.readOnly = true;
				
				// Create 'cancel' button
				rater.append(
					control.cancel = $('<div class="rating-cancel"><a title="' + control.cancel + '">' + control.cancelValue + '</a></div>')
					.mouseover(function(){
						$(this).rating('drain');
						$(this).addClass('star-rating-hover');
						//$(this).rating('focus');
					})
					.mouseout(function(){
						$(this).rating('draw');
						$(this).removeClass('star-rating-hover');
						//$(this).rating('blur');
					})
					.click(function(){
					 $(this).rating('select');
					})
					.data('rating', control)
				);
				
			}; // first element of group
			
			// insert rating star
			var star = $('<div class="star-rating rater-'+ control.serial +'" style=" display:' + this.style["display"] + '"><a name="' + this.name + '" title="' + (this.title || this.value) + '">' + this.value + '</a></div>');
			rater.append(star);
			
			// inherit attributes from input element
			if(this.id) star.attr('id', this.id);
			if(this.className) star.addClass(this.className);
			
			// Half-stars?
			if(control.half) control.split = 2;
			
			// Prepare division control
			if(typeof control.split=='number' && control.split>0){
				var stw = ($.fn.width ? star.width() : 0) || control.starWidth;
				var spi = (control.count % control.split), spw = Math.floor(stw/control.split);
				star
				// restrict star's width and hide overflow (already in CSS)
				.width(spw)
				// move the star left by using a negative margin
				// this is work-around to IE's stupid box model (position:relative doesn't work)
				.find('a').css({ 'margin-left':'-'+ (spi*spw) +'px' })
			};
			
			// readOnly?
			if(control.readOnly)//{ //save a byte!
				// Mark star as readOnly so user can customize display
				star.addClass('star-rating-readonly');
			//}  //save a byte!
			else//{ //save a byte!
			 // Enable hover css effects
				star.addClass('star-rating-live')
				 // Attach mouse events
					.mouseover(function(){
						$(this).rating('fill');
						$(this).rating('focus');
					})
					.mouseout(function(){
						$(this).rating('draw');
						$(this).rating('blur');
					})
					.click(function(){
						$(this).rating('select');
					})
				;
			//}; //save a byte!
			
			// set current selection
			if($(this).is(':checked'))	control.current = star;
			
			// hide input element
			input.hide();
			
			// backward compatibility, form element to plugin
			input.change(function(){
    $(this).rating('select');
   });
			
			// attach reference to star to input element and vice-versa
			star.data('rating.input', input.data('rating.star', star));
			
			// store control information in form (or body when form not available)
			control.stars[control.stars.length] = star[0];
			control.inputs[control.inputs.length] = input[0];
			control.rater = raters[eid] = rater;
			control.context = context;
			
			input.data('rating', control);
			rater.data('rating', control);
			star.data('rating', control);
			context.data('rating', raters);
  }); // each element
		
		// Initialize ratings (first draw)
		$('.rating-to-be-drawn').rating('draw').removeClass('rating-to-be-drawn');
		
		return this; // don't break the chain...
	};
	
	/*--------------------------------------------------------*/
	
	/*
		### Core functionality and API ###
	*/
	$.extend($.fn.rating, {
		// Used to append a unique serial number to internal control ID
		// each time the plugin is invoked so same name controls can co-exist
		calls: 0,
		
		focus: function(){
			var control = this.data('rating'); if(!control) return this;
			if(!control.focus) return this; // quick fail if not required
			// find data for event
			var input = $(this).data('rating.input') || $( this.tagName=='INPUT' ? this : null );
   // focus handler, as requested by focusdigital.co.uk
			if(control.focus) control.focus.apply(input[0], [input.val(), $('a', input.data('rating.star'))[0]]);
		}, // $.fn.rating.focus
		
		blur: function(){
			var control = this.data('rating'); if(!control) return this;
			if(!control.blur) return this; // quick fail if not required
			// find data for event
			var input = $(this).data('rating.input') || $( this.tagName=='INPUT' ? this : null );
   // blur handler, as requested by focusdigital.co.uk
			if(control.blur) control.blur.apply(input[0], [input.val(), $('a', input.data('rating.star'))[0]]);
		}, // $.fn.rating.blur
		
		fill: function(){ // fill to the current mouse position.
			var control = this.data('rating'); if(!control) return this;
			// do not execute when control is in read-only mode
			if(control.readOnly) return;
			// Reset all stars and highlight them up to this element
			this.rating('drain');
			this.prevAll().andSelf().filter('.rater-'+ control.serial).addClass('star-rating-hover');
		},// $.fn.rating.fill
		
		drain: function() { // drain all the stars.
			var control = this.data('rating'); if(!control) return this;
			// do not execute when control is in read-only mode
			if(control.readOnly) return;
			// Reset all stars
			control.rater.children().filter('.rater-'+ control.serial).removeClass('star-rating-on').removeClass('star-rating-hover');
		},// $.fn.rating.drain
		
		draw: function(){ // set value and stars to reflect current selection
			var control = this.data('rating'); if(!control) return this;
			// Clear all stars
			this.rating('drain');
			// Set control value
			if(control.current){
				control.current.data('rating.input').attr('checked','checked');
				control.current.prevAll().andSelf().filter('.rater-'+ control.serial).addClass('star-rating-on');
			}
			else
			 $(control.inputs).removeAttr('checked');
			// Show/hide 'cancel' button
			control.cancel[control.readOnly || control.required?'hide':'show']();
			// Add/remove read-only classes to remove hand pointer
			this.siblings()[control.readOnly?'addClass':'removeClass']('star-rating-readonly');
		},// $.fn.rating.draw
		
		select: function(value){ // select a value
			var control = this.data('rating'); if(!control) return this;
			// do not execute when control is in read-only mode
			if(control.readOnly) return;
			
			//========================================================================================
			// LOZA: Añado esto para guardar el valor anterior antes de votar
			//========================================================================================
			var votoAnterior = '0';
			if(control.current != null) //Antes teniamos voto
			{
				votoAnterior = control.current[0].textContent;
			}
			//========================================================================================
			// LOZA: Fin de añadido
			//========================================================================================
			
			// clear selection
			control.current = null;
			// programmatically (based on user input)
			if(typeof value!='undefined'){
			 // select by index (0 based)
				if(typeof value=='number')
 			 return $(control.stars[value]).rating('select');
				// select by literal value (must be passed as a string
				if(typeof value=='string')
					//return 
					$.each(control.stars, function(){
						if($(this).data('rating.input').val()==value) $(this).rating('select');
					});
			}
			else
				control.current = this[0].tagName=='INPUT' ? 
				 this.data('rating.star') : 
					(this.is('.rater-'+ control.serial) ? this : null);
			
			// Update rating control state
			this.data('rating', control);
			// Update display
			this.rating('draw');
			
			//========================================================================================
			// LOZA: Añado esto para que me establezca el valor actual en un txt
			//========================================================================================
			
			if(this[0] == control.cancel[0]) //Hemos pulsado el boton cancelar
			{
				var baseNombre = control.inputs[0].name;
				var txtHack = document.getElementById('txtHack'+baseNombre);
				if(txtHack.value == '0'){return;}
				txtHack.value = '0';
			}
			else	//Hemos pulsado una estrella
			{
				var baseNombre = this[0].childNodes[0].name;
				var txtHack = document.getElementById('txtHack'+baseNombre);
				if(txtHack.value == this[0].textContent){return;}
				txtHack.value = this[0].textContent;
			}
			
			//========================================================================================
			// LOZA: Fin de añadido
			//========================================================================================
			
			// find data for event
			var input = $( control.current ? control.current.data('rating.input') : null );
			// click callback, as requested here: http://plugins.jquery.com/node/1655
			if(control.callback) control.callback.apply(input[0], [input[0], control,votoAnterior]);// callback event
		},// $.fn.rating.select
		
		readOnly: function(toggle, disable){ // make the control read-only (still submits value)
			var control = this.data('rating'); if(!control) return this;
			// setread-only status
			control.readOnly = toggle || toggle==undefined ? true : false;
			// enable/disable control value submission
			if(disable) $(control.inputs).attr("disabled", "disabled");
			else     			$(control.inputs).removeAttr("disabled");
			// Update rating control state
			this.data('rating', control);
			// Update display
			this.rating('draw');
		},// $.fn.rating.readOnly
		
		disable: function(){ // make read-only and never submit value
			this.rating('readOnly', true, true);
		},// $.fn.rating.disable
		
		enable: function(){ // make read/write and submit value
			this.rating('readOnly', false, false);
		}// $.fn.rating.select
		
		
		
 });

	
	/*--------------------------------------------------------*/
	
	/*
		### Default Settings ###
		eg.: You can override default control like this:
		$.fn.rating.options.cancel = 'Clear';
	*/
	$.fn.rating.options = { //$.extend($.fn.rating, { options: {
	        resetAll: true,
			cancel: 'Cancel Rating',   // advisory title for the 'cancel' link
			cancelValue: '',           // value to submit when user click the 'cancel' link
			split: 0,                  // split the star into how many parts?
						
			// Width of star image in case the plugin can't work it out. This can happen if
			// the jQuery.dimensions plugin is not available OR the image is hidden at installation
			starWidth: 16,
			
			//NB.: These don't need to be pre-defined (can be undefined/null) so let's save some code!
			//half:     false,         // just a shortcut to control.split = 2
			required: false,         // disables the 'cancel' button so user can only select one of the specified values
			//readOnly: false,         // disable rating plugin interaction/ values cannot be changed
			//focus:    function(){},  // executed when stars are focused
			//blur:     function(){},  // executed when stars are focused
			//callback: function(){},  // executed when a star is clicked
			focus: function(value, link){
				// 'this' is the hidden form element holding the current value
				// 'value' is the value selected
				// 'element' points to the link element that received the click.
				var tip = $('#hover'+ this.name);
				tip[0].data = tip[0].data || tip.html();
				tip.html(link.title || 'value: '+value);
			},
			blur: function(value, link){
				var tip = $('#hover'+ this.name);
				tip.html(tip[0].data || '');
			},
			callback: function(input, control, votoViejo){
			    //Sea lo que sea, reseteamos el control
			    var eid = (this.name || 'unnamed-rating').replace(/\[|\]/g, '_').replace(/^\_+|\_+$/g,'');
			    var context = $(this.form || document.body);
			    var raters = context.data('rating');
                if(!raters || raters.call!=$.fn.rating.calls) raters = { count:0, call:$.fn.rating.calls };
                var rater = raters[eid];
			    rater = null;
			    
				if(control.current != null) //Hemos pulsado una estrella
				{
				    input.attributes['onclick'].value=input.attributes['onclick'].value.replace('%votoViejo%',votoViejo);
				    if(typeof(input.onclick) == 'function'){
					    input.onclick();
					}
					else{
					    eval(input.onclick);
					}
				}else{	//Hemos pulsado en el boton cancelar
				    control.inputs[0].attributes['onclick'].value=control.inputs[0].attributes['onclick'].value.replace('%votoViejo%',votoViejo);
				    control.inputs[0].attributes['onclick'].value=control.inputs[0].attributes['onclick'].value.replace('&1','&0');
					//eval(control.inputs[0].attributes['onclick'].value.replace('&1','&0'));
					if(typeof(control.inputs[0].onclick) == 'function'){
					    control.inputs[0].onclick();
					}
					else{
					    eval(control.inputs[0].attributes['onclick'].value);
					}
				}
			}
			
			
			  
 }; //} });
	/*--------------------------------------------------------*/
	
	/*
		### Default implementation ###
		The plugin will attach itself to file inputs
		with the class 'multi' when the page loads
	*/
	$(function(){
	 $('input[type=radio].star').rating();
	});
	
/*# AVOID COLLISIONS #*/
})(jQuery);
/*# AVOID COLLISIONS #*/

}


$(document).ready(function(){
    var duracion=500;
    if(navigator.appName=="Microsoft Internet Explorer")
    {
        duracion= 0;
    }    
	
	$('div.gallery').gallery({
		duration: 500,
		autoRotation: 5000,
		listOfSlides: '#carousel > ul > li',
		switcher: '.switcher>li',
		effect:true
	});
	$('div.gallery').gallery({
		duration: 500,
		autoRotation: 5000,
		listOfSlides: '.items > .item',
		switcher: '.switcher>li',
		effect:true
	});
	$('#comunidades').gallery({
		duration: duracion,
		//autoRotation: 5000,
		listOfSlides: ' .contenedor > .pag_comunidades',
		switcher: ' .contenedor > .botones > li > .botoncito',
		effect:true
	});
	$('#recursos').gallery({	
	    duration:duracion,	
		autoRotation: 5000,
		listOfSlides: ' .contenedor > .pag_recurso',
		switcher: ' .contenedor > .botones > li',
		effect:true
	});
});

(function($) {
	$.fn.gallery = function(options) { return new Gallery(this.get(0), options); };
	
	function Gallery(context, options) { this.init(context, options); };
	
	Gallery.prototype = {
		options:{},
		init: function (context, options){
			this.options = $.extend({
				duration: 1400,
				slideElement: 1,
				autoRotation: false,
				effect: false,
				listOfSlides: 'ul > li',
				switcher: false,
				disableBtn: false,
				nextBtn: 'a.link-next, a.btn-next, a.next',
				prevBtn: 'a.link-prev, a.btn-prev, a.prev',
				circle: true,
				direction: false,
				event: 'click',
				IE: false
			}, options || {});
			var _el = $(context).find(this.options.listOfSlides);
			if (this.options.effect) this.list = _el;
			else this.list = _el.parent();
			this.switcher = $(context).find(this.options.switcher);
			this.nextBtn = $(context).find(this.options.nextBtn);
			this.prevBtn = $(context).find(this.options.prevBtn);
			this.count = _el.index(_el.filter(':last'));
			
			if (this.options.switcher) this.active = this.switcher.index(this.switcher.filter('.active:eq(0)'));
			else this.active = _el.index(_el.filter('.active:eq(0)'));
			if (this.active < 0) this.active = 0;
			this.last = this.active;
			
			this.woh = _el.outerWidth(true);
			if (!this.options.direction) this.installDirections(this.list.parent().width());
			else {
				this.woh = _el.outerHeight(true);
				this.installDirections(this.list.parent().height());
			}
			
			if (!this.options.effect) {
				this.rew = this.count - this.wrapHolderW + 1;
				if (!this.options.direction) this.list.css({marginLeft: -(this.woh * this.active)});
				else this.list.css({marginTop: -(this.woh * this.active)});
			}
			else {
				this.rew = this.count;
				this.list.css({opacity: 0}).removeClass('active').eq(this.active).addClass('active').css({opacity: 1}).css('opacity', 'auto');
				this.switcher.removeClass('active').eq(this.active).addClass('active');
			}
			
			if (this.options.disableBtn) {
				if (this.count < this.wrapHolderW) this.nextBtn.addClass(this.options.disableBtn);
				if (this.active == 0) this.prevBtn.addClass(this.options.disableBtn);
			}
			
			this.initEvent(this, this.nextBtn, this.prevBtn, true);
			this.initEvent(this, this.prevBtn, this.nextBtn, false);
			
			if (this.options.autoRotation) this.runTimer(this);
			
			if (this.options.switcher) this.initEventSwitcher(this, this.switcher);
		},
		installDirections: function(temp){
			this.wrapHolderW = Math.ceil(temp / this.woh);
			if (((this.wrapHolderW - 1) * this.woh + this.woh / 2) > temp) this.wrapHolderWwrapHolderW--;
		},
		fadeElement: function(){
//			if ($.browser.msie && this.options.IE){
//				this.list.eq(this.last).css({opacity:0});
//				this.list.removeClass('active').eq(this.active).addClass('active').css({opacity:'auto'});
//			}
//			else{
				this.list.eq(this.last).animate({opacity:0}, {queue:false, duration: this.options.duration});
				this.list.removeClass('active').eq(this.active).addClass('active').animate({
					opacity:1
				}, {queue:false, duration: this.options.duration, complete: function(){
					$(this).css('opacity','auto');
				}});
//			}
			if (this.options.switcher) this.switcher.removeClass('active').eq(this.active).addClass('active');
			this.last = this.active;
		},
		scrollElement: function(){
			if (!this.options.direction) this.list.animate({marginLeft: -(this.woh * this.active)}, {queue:false, duration: this.options.duration});
			else this.list.animate({marginTop: -(this.woh * this.active)}, {queue:false, duration: this.options.duration});
			if (this.options.switcher) this.switcher.removeClass('active').eq(this.active).addClass('active');
		},
		runTimer: function($this){
			if($this._t) clearTimeout($this._t);
			$this._t = setInterval(function(){
				$this.toPrepare($this, true);
			}, this.options.autoRotation);
		},
		initEventSwitcher: function($this, el){
			el.bind($this.options.event, function(){
				$this.active = $this.switcher.index($(this));
				if($this._t) clearTimeout($this._t);
				if (!$this.options.effect) $this.scrollElement();
				else $this.fadeElement();
				if ($this.options.autoRotation) $this.runTimer($this);
				return false;
			});
		},
		initEvent: function($this, addEventEl, addDisClass, dir){
			addEventEl.bind($this.options.event, function(){
				if($this._t) clearTimeout($this._t);
				if ($this.options.disableBtn &&($this.count > $this.wrapHolderW)) addDisClass.removeClass($this.options.disableBtn);
				$this.toPrepare($this, dir);
				if ($this.options.autoRotation) $this.runTimer($this);
				return false;
			});
		},
		toPrepare: function($this, side){
			if (($this.active == $this.rew) && $this.options.circle && side) $this.active = -$this.options.slideElement;
			if (($this.active == 0) && $this.options.circle && !side) $this.active = $this.rew + $this.options.slideElement;
			for (var i = 0; i < $this.options.slideElement; i++){
				if (side) {
					if ($this.active + 1 > $this.rew) {
						if ($this.options.disableBtn && ($this.count > $this.wrapHolderW)) $this.nextBtn.addClass($this.options.disableBtn);
					}
					else $this.active++;
				}
				else{
					if ($this.active - 1 < 0) {
						if ($this.options.disableBtn && ($this.count > $this.wrapHolderW)) $this.prevBtn.addClass($this.options.disableBtn);
					}
					else $this.active--;
				}
			};
			if ($this.active == $this.rew && side) if ($this.options.disableBtn &&($this.count > $this.wrapHolderW)) $this.nextBtn.addClass($this.options.disableBtn);
			if ($this.active == 0 && !side) if ($this.options.disableBtn &&($this.count > $this.wrapHolderW)) $this.prevBtn.addClass($this.options.disableBtn);
			if (!$this.options.effect) $this.scrollElement();
			else $this.fadeElement();
		},
		stop: function(){
			if (this._t) clearTimeout(this._t);
		},
		play: function(){
			if (this._t) clearTimeout(this._t);
			if (this.options.autoRotation) this.runTimer(this);
		}
	}
}(jQuery));


