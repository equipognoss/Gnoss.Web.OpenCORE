var hayPropLangBusqCom = false;
var comprobarIdiomasRellenos = false;
var idiomasUsados = null;
var openSeaDragonInfoSem = '';
var TxtValorRdf = 'mTxtValorRdf';
var TxtRegistroIDs = 'mTxtRegistroIDs';
var TxtCaracteristicasElem = 'mTxtCaracteristicasElem';
var TxtElemEditados = 'mTxtElemEditados';
var TxtValorRdfHerencias = 'mTxtValorRdfHerencias';
var TxtNombreCatTesSem = 'mTxtNombreCatTesSem';
var TxtValoresGrafoDependientes = 'txtValoresGrafoDependientes';

$(function () {
    hayPropLangBusqCom = (GetPropLangBusqCom() != null);
    GuardandoCambios = false;
});

function RecogerValoresRDF() {
    return RecogerValoresRDF2(true);
}

function RecogerValoresRDF2(pValidarCampos) {
    comprobarIdiomasRellenos = false;
    var ok = RecogerValoresRDFInt(TxtValorRdf, TxtRegistroIDs, TxtCaracteristicasElem, TxtElemEditados, pValidarCampos);

    if (ok && hayPropLangBusqCom) {
        comprobarIdiomasRellenos = true;
        ObtenerIdiomasUsadosRecurso();
        ok = RecogerValoresRDFInt(TxtValorRdf, TxtRegistroIDs, TxtCaracteristicasElem, TxtElemEditados, pValidarCampos);
    }

    if (ok && !pValidarCampos) {
        //Si no hay que validar los campos, valido que por lo menos ha introducido el título
        $.each(TxtTitulosSem.split(','), function (index, value) {
            if ($('#' + value).is(":visible") && $('#' + value).val() == '') {
                ok = false;
                mensajeErrorFormSemPrinc = form.errordtitulo;
            }
        });
    }

    return ok;
}

function ObtenerIdiomasUsadosRecurso() {
    idiomasUsadosText = "";
    var valorRdf = $('#' + TxtValorRdf).val();
    var valoresConLang = valorRdf.split('[|lang|]');

    for (var i = 0; i < (valoresConLang.length - 1) ; i++) {
        if (valoresConLang[i].substring(0, valoresConLang[i].indexOf('@')) != 'null') {
            var idioma = valoresConLang[i].substring(valoresConLang[i].lastIndexOf('@') + 1);

            if (idioma.length == 2 && idiomasUsadosText.indexOf(idioma + ',') == -1) {
                idiomasUsadosText += idioma + ',';
            }
        }
    }

    idiomasUsados = idiomasUsadosText.split(',');
}

function RecogerValoresRDFInt(pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados, pValidar)
{
    mensajeErrorFormSemPrinc = textoFormSem.algunCampoMalIntro;
    if (document.getElementById(TxtElemEditados).value != '') {
        mensajeErrorFormSemPrinc = textoFormSem.hayPropEditandose;
        return false;
    }
	var subClasesIncompletas = false;
	if($('.cmbSubClass').length > 0){
		$('.cmbSubClass').each(function(){
			var that = $(this);
			if(that.val() == ""){
				subClasesIncompletas = true;
			}
		});
	}
	if(subClasesIncompletas){
		mensajeErrorFormSemPrinc = textoFormSem.SubtipoObligatorio;
		return false;
	}

    var caract = document.getElementById(pTxtCaract).value;
    var camposCorrectos = true;
    propErrorSemCms = '';
    var propLanguaje = GetPropLangBusqCom();
    
    caract = caract.substring(caract.indexOf('|') + 1);//Eliminio 1 parametro
    var caracteristicas = caract.split('|');
    for (var i=0;i<caracteristicas.length;i++)
    {
        var entProp = ObtenerNombreEntPropRestricciones(caracteristicas[i]);
        var entidad = entProp.split(',')[0];
        var propiedad = entProp.split(',')[1];
        var tipoProp = GetTipoPropiedad(entidad, propiedad, pTxtCaract);
        
        if (HayQueRecogerValorProp(caracteristicas[i], entidad))
        {
            var valorProp = ObtenerValorEntidadProp(entProp, pTxtIDs, pTxtCaract);

            if (tipoProp == 'FD' || tipoProp == 'CD')
            {
                var valorAgregado = false;
                var comprobarCampoVacio = true;

                if (typeof valorProp != 'undefined' && valorProp != null && EsPropiedadMultiIdioma(entidad, propiedad)) {
                    //Agregamos el valor actual por si es el de la pestaña actual, así no falla la validación
                    var valorPropMulti = GetValorEncode(valorProp);
                    valorPropMulti = ObtenerValorMultiIdiomaPesanyaActual(entidad, propiedad, valorPropMulti);
                    AgregarValorAPropiedad(entidad, propiedad, valorPropMulti, pTxtValores);
                    valorAgregado = true;
                    comprobarCampoVacio = !(valorPropMulti != null && valorPropMulti != '');
                }

                var campoCorrecto = true;
                if (pValidar)
                {
                    campoCorrecto = ComprobarValorCampoCorrectoInt(entidad, propiedad, valorProp, pTxtIDs, pTxtCaract, comprobarCampoVacio) && ComprobarMultiIdiomaCorrecto(entidad, propiedad, tipoProp);
                    camposCorrectos = camposCorrectos && campoCorrecto;
                }
                
                if (campoCorrecto && valorProp != null && !valorAgregado)
                {
                    var valorDefNoSelec = GetValorDefNoSelec(entidad, propiedad, pTxtCaract);
                    if (valorDefNoSelec == null || valorDefNoSelec == '' || valorDefNoSelec != valorProp) {
                        valorProp = GetValorEncode(valorProp);
                        AgregarValorAPropiedad(entidad, propiedad, valorProp, pTxtValores);
                    } else if (valorDefNoSelec == null || valorDefNoSelec == '' || valorDefNoSelec == valorProp) {
                        AgregarValorAPropiedad(entidad, propiedad, '', pTxtValores);
                    }
                }

                if (!campoCorrecto) {
                    propErrorSemCms += propiedad + ',' + entidad + '|';
                }
            }
            else if (tipoProp == 'VD')
            {
                DeleteElementoGuardado(entidad, propiedad, pTxtValores, pTxtElemEditados, 0); 
                var valorAntiguo = GetValorElementoGuardado(entidad, propiedad, pTxtValores, pTxtElemEditados, 0);
                while (valorAntiguo != null && valorAntiguo != '')
                {
                    DeleteElementoGuardado(entidad, propiedad, pTxtValores, pTxtElemEditados, 0);
                    valorAntiguo = GetValorElementoGuardado(entidad, propiedad, pTxtValores, pTxtElemEditados, 0);
                }
            
                var valoresProp = valorProp.split(',');
                for (var j=0;j<valoresProp.length;j++)
                {
                    if (valoresProp[j] != '')
                    {
                        PutElementoGuardado(entidad, propiedad, GetValorEncode(valoresProp[j].trim()), pTxtValores, pTxtElemEditados);
                    }
                }
                
                if (pValidar)
                {
                    var campoCorrecto = ComprobarCardinalidadPropiedad(entidad, propiedad, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados);
                    camposCorrectos = (campoCorrecto && camposCorrectos);

                    if (!campoCorrecto) {
                        propErrorSemCms += propiedad + ',' + entidad + '|';
                    }
                }
            }
            else if (tipoProp == 'FSEO' || tipoProp == 'CSEO')
            {
                var campoCorrecto = true;
                if (pValidar)
                {
                    campoCorrecto = ComprobarValorCampoCorrecto(entidad, propiedad, valorProp, pTxtIDs, pTxtCaract);
                    camposCorrectos = camposCorrectos && campoCorrecto;
                }
                
                if (campoCorrecto && valorProp != null)
                {
                    valorProp = GetValorEncode(valorProp);
                    AgregarValorAPropiedad(entidad, propiedad, valorProp, pTxtValores);
                }

                if (!campoCorrecto) {
                    propErrorSemCms += propiedad + ',' + entidad + '|';
                }
            }
        }
        else if (pValidar && EntidadEsPrincipalOPerteneceAPropFuncionalOCardiMenorIgualUno(entidad) && propLanguaje != propiedad)
        {
            var campoCorrecto = true;

            if (tipoProp == 'LD' || tipoProp == 'LO' || tipoProp == 'LSEO')
            {
                campoCorrecto = ComprobarCardinalidadPropiedad(entidad, propiedad, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados);
                camposCorrectos = (campoCorrecto && camposCorrectos);

                if (!campoCorrecto) {
                    propErrorSemCms += propiedad + ',' + entidad + '|';
                }
            }

            if (comprobarIdiomasRellenos && tipoProp != 'LO' && tipoProp != 'LSEO' && EsPropiedadMultiIdioma(entidad, propiedad) && (tipoProp == 'FD' || CardinalidadElementoMayorOIgualUno(entidad, propiedad, TxtCaracteristicasElem))) {
                campoCorrecto = ComprobarPropiedadTieneIdiomasUsados(entidad, propiedad);
                camposCorrectos = (campoCorrecto && camposCorrectos);

                if (!campoCorrecto) {
                    propErrorSemCms += propiedad + ',' + entidad + '|';
                }
            }
            //else if (tipoProp == 'FO' || tipoProp == 'CO')
            //{//TODO: ESTO FUNCIONA VACÍO???
                
            //}
        }
    }
    
    if (camposCorrectos) {
        TxtHackHayCambios = false;
		GuardandoCambios = true;
    }

    return camposCorrectos;
}

function HayQueRecogerValorProp(pCaracteristicas, pTipoEntidad)
{
    return (pCaracteristicas.length > 0 && ((pCaracteristicas.indexOf('recoger=true') != -1 && pCaracteristicas.indexOf('subclase=true') == -1) || (EntidadSubClaseSeleccionada(pTipoEntidad, true) && (pCaracteristicas.indexOf('tipo=FD,') != -1 || pCaracteristicas.indexOf('tipo=CD,') != -1 || pCaracteristicas.indexOf('tipo=VD,') != -1 || pCaracteristicas.indexOf('tipo=FSEO,') != -1 || pCaracteristicas.indexOf('tipo=CSEO,') != -1) && !PerteneceEntidadAAlgunGrupoPanelSinEditar(GetSuperClasesEntidad(pTipoEntidad)[0], TxtCaracteristicasElem, TxtElemEditados))))
}

function EntidadEsPrincipalOPerteneceAPropFuncionalOCardiMenorIgualUno(pEntidad) {
    
    if (pEntidad == '') {
        return false;
    }

    if (GetCaracteristicaPropiedad(pEntidad, '', TxtCaracteristicasElem, 'entPrincipal') == 'true') {
        return true;
    }

    var entProps = GetEntidadYPropiedadConEntidadComoRango(pEntidad);
    var funcionalOCardi = true;

    if (entProps.length == 0) {
        entProps = GetEntidadYPropiedadConEntidadComoRango(pEntidad + '_bis0');
    }

    if (entProps.length == 0) {
        entProps = GetEntidadYPropiedadConEntidadComoRango(pEntidad + '_bis1');
    }

    for (var i = 0; i < entProps.length; i++) {
        var entidad = entProps[i][0];
        var propiedad = entProps[i][1];

        var tipo = GetCaracteristicaPropiedad(entidad, propiedad, TxtCaracteristicasElem, 'tipo')

        if (tipo != 'FO' && tipo != 'CO') {
            funcionalOCardi = false;
        }
        else {
            funcionalOCardi = EntidadEsPrincipalOPerteneceAPropFuncionalOCardiMenorIgualUno(entidad)
        }
    }

    return funcionalOCardi;
}

function EntidadSubClaseSeleccionada(pTipoEntidad, pSoloSiComboVisible)
{
    if (pTipoEntidad != "") {
        var combosSubClase = $('select.cmbSubClass');

        for (var i = 0; i < combosSubClase.length; i++) {
            if (combosSubClase[i].value == pTipoEntidad && (!pSoloSiComboVisible || $(combosSubClase[i]).is(":visible"))) {
                return true;
            }
        }
    }

    return false;
}

function EntidadEsSubClase(pTipoEntidad) {
    var subClase = GetCaracteristicaPropiedad(pTipoEntidad, '', TxtCaracteristicasElem, 'subclase');

    return (subClase != null && subClase == 'true')
}

/**
 * Esta función obtiene el tipo de entidad editada, considerando si tiene una superclase y selecciona la subclase correspondiente si hay opciones disponibles.
 * @param {any} pTipoEntidad El tipo de entidad a obtener.
 * @returns
 */
function ObtenerTipoEntidadEditada(pTipoEntidad) {
    var superClase = GetCaracteristicaPropiedad(pTipoEntidad, '', TxtCaracteristicasElem, 'superclase');

    if (superClase != null && superClase == 'true') {
        var combosSubClase = $('select.cmbSubClass_' + ObtenerTextoGeneracionIDs(pTipoEntidad));

        if (combosSubClase.length > 0) {
            var tipoEntidad = combosSubClase[0].value;
            return tipoEntidad;
        }
    }

    return pTipoEntidad;
}

function ObtenerTextoGeneracionIDs(pTexto) {
    return pTexto.replace(/\//g,'_').replace(/\:/g,'_').replace(/\./g,'_').replace(/\#/g,'_');
}

function ObtenerNombreEntPropRestricciones(pCaracteristicas)
{
    var nombreEntProp = pCaracteristicas.substring(0, pCaracteristicas.indexOf(',') + 1);
    pCaracteristicas = pCaracteristicas.substring(pCaracteristicas.indexOf(',') + 1);
    nombreEntProp += pCaracteristicas.substring(0, pCaracteristicas.indexOf(','));
    return nombreEntProp;
}

function ObtenerValorEntidadProp(pEntPropID, pTxtIDs, pTxtCaract) {
    return ObtenerValorEntidadPropDeIdioma(pEntPropID, pTxtIDs, pTxtCaract, null)
}

function ObtenerValorEntidadPropDeIdioma(pEntPropID, pTxtIDs, pTxtCaract, pIdioma)
{
    var idControl = ObtenerControlEntidadProp(pEntPropID, pTxtIDs);
    var tipoCampo = ObtenerTipoCampo(pEntPropID, pTxtCaract);

    if (pIdioma != null) {
        idControl += '_lang_' + pIdioma;
    }
    
    if (pEntPropID == 'EmploymentPeriod,StartDate' || pEntPropID == 'EmploymentPeriod,EndDate' || pEntPropID == 'EmploymentPeriodPosition,StartDate' || pEntPropID == 'EmploymentPeriodPosition,EndDate' || pEntPropID == 'AttendancePeriod,StartDate' || pEntPropID == 'AttendancePeriod,EndDate' || pEntPropID == 'UserArea_PersonQualifications,FechaInicio' || pEntPropID == 'UserArea_PersonQualifications,FechaFin')
    {
        var dia = '01';
        var mes = ObtenerMesNumero(document.getElementById(idControl).children[0].value);
        var anio = document.getElementById(idControl).children[1].value;
        
        if (anio.length != 4 || mes == null)
        {
            return '';
        }
        else
        {
            return dia + '/' + mes + '/' + anio;
        }
    }
    else if (tipoCampo == 'Tiny')
    {
        if (document.getElementById(idControl) != null) {
            var valor = $('#' + idControl).val();

            if (valor == '<br>') {
                return '';
            }

            return valor.trim();
        }
    }
    else if (document.getElementById(idControl) != null)
    {
        return document.getElementById(idControl).value.trim();
    }
    else
    {
        return null;
    }
}

function ObtenerMesNumero(pMes)
{
    if (pMes == 'Enero'){return '01';}
    if (pMes == 'Febrero'){return '02';}
    if (pMes == 'Marzo'){return '03';}
    if (pMes == 'Abril'){return '04';}
    if (pMes == 'Mayo'){return'05';}
    if (pMes == 'Junio'){return '06';}
    if (pMes == 'Julio'){return '07';}
    if (pMes == 'Agosto'){return '08';}
    if (pMes == 'Septiembre'){return '09';}
    if (pMes == 'Octubre'){return '10';}
    if (pMes == 'Noviembre'){return '11';}
    if (pMes == 'Diciembre'){return '12';}
}

function ObtenerMesTexto(pMes)
{
    if (pMes == '01'){return 'Enero';}
    if (pMes == '02'){return 'Febrero';}
    if (pMes == '03'){return 'Marzo';}
    if (pMes == '04'){return 'Abril';}
    if (pMes == '05'){return 'Mayo';}
    if (pMes == '06'){return 'Junio';}
    if (pMes == '07'){return 'Julio';}
    if (pMes == '08'){return 'Agosto';}
    if (pMes == '09'){return 'Septiembre';}
    if (pMes == '10'){return 'Octubre';}
    if (pMes == '11'){return 'Noviembre';}
    if (pMes == '12'){return 'Diciembre';}
}

function ObtenerControlEntidadProp(pEntPropID, pTxtIDs)
{
    var ids = document.getElementById(pTxtIDs).value;
    
    var textoPropBuscar = pEntPropID + ',';
    if (ids.indexOf(textoPropBuscar) == -1)
    {
        textoPropBuscar = pEntPropID;
    }
    var trozoIDs = ids.substring(ids.indexOf(textoPropBuscar));
    trozoIDs = trozoIDs.substring(0, trozoIDs.indexOf('|'));
    
    return trozoIDs.replace(pEntPropID, '').substring(1);
}

function ObtenerTipoCampo(pEntPropID, pTxtCaract)
{
    var caract = document.getElementById(pTxtCaract).value;
    var caractProp = caract.substring(caract.indexOf(pEntPropID + ','));
    var prop = 'tipoCampo=';
    var tipoCampo = caractProp.substring(caractProp.indexOf(prop) + prop.length);
    tipoCampo = tipoCampo.substring(0, tipoCampo.indexOf(','));
    return tipoCampo;
}

function AgregarValorAPropiedad(pEntidad, pPropiedad, pValor, pTxtValores)
{
    var valorRdf = document.getElementById(pTxtValores).value;
    
    var entidadXML = '<' + pEntidad + '>';
    var entidadCierreXML = '</' + pEntidad + '>';
    var propiedadXML = '<' + pPropiedad + '>';
    var propiedadCierreXML = '</' + pPropiedad + '>';
    
    var trozo1 = valorRdf.substring(0, valorRdf.indexOf(entidadXML) + entidadXML.length);
    var trozo2 = valorRdf.substring(valorRdf.indexOf(entidadXML) + entidadXML.length, valorRdf.indexOf(entidadCierreXML));
    var trozo3 = valorRdf.substring(valorRdf.indexOf(entidadCierreXML));
    
    var trozoProp1 = '';
    
    while (trozo2.indexOf(propiedadXML) > 0)
    {
        var elementoSiguiente = trozo2.substring(1, trozo2.indexOf('>'));
        elementoSiguiente = '</' + elementoSiguiente + '>';
        trozoProp1 += trozo2.substring(0, trozo2.indexOf(elementoSiguiente) + elementoSiguiente.length);
        trozo2 = trozo2.substring(trozo2.indexOf(elementoSiguiente) + elementoSiguiente.length);
    }
    
    var trozoProp3 = trozo2.substring(trozo2.indexOf(propiedadCierreXML) + propiedadCierreXML.length);
    
    
    document.getElementById(pTxtValores).value = trozo1 + trozoProp1 + propiedadXML + pValor + propiedadCierreXML + trozoProp3 + trozo3;
}

function ObtenerTextoAntesElemento(pTexto, pElemento)
{
    var elementoXML = '<' + pElemento + '>';
    if (pTexto.indexOf(elementoXML) != -1)
    {
        return pTexto.substring(0, pTexto.indexOf(elementoXML));
    }
    else
    {
        return '';
    }
}

function ObtenerTextoElemento(pTexto, pElemento)
{
    var elementoXML = '<' + pElemento + '>';
    var elementoCierreXML = '</' + pElemento + '>';
    if (pTexto.indexOf(elementoXML) != -1)
    {
        return pTexto.substring(pTexto.indexOf(elementoXML), pTexto.indexOf(elementoCierreXML) + elementoCierreXML.length);
    }
    else
    {
        return '';
    }
}

function ObtenerValorTextoElemento(pTexto, pElemento)
{
    var elementoXML = '<' + pElemento + '>';
    var elementoCierreXML = '</' + pElemento + '>';
    if (pTexto.indexOf(elementoXML) != -1)
    {
        var texto = pTexto.substring(pTexto.indexOf(elementoXML), pTexto.indexOf(elementoCierreXML));
        texto = texto.substring(elementoXML.length);
        return texto;
    }
    else
    {
        return '';
    }
}

function ObtenerTextoDespuesElemento(pTexto, pElemento)
{
    var elementoCierreXML = '</' + pElemento + '>';
    if (pTexto.indexOf(elementoCierreXML) != -1)
    {
        return pTexto.substring(pTexto.indexOf(elementoCierreXML) + elementoCierreXML.length);
    }
    else
    {
        return '';
    }
}

function AgregarValorADataNoFuncionalProp(pEntidad, pPropiedad, pControlContValores, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados)
{
    TxtHackHayCambios = true;
    var valor = ObtenerValorEntidadProp(pEntidad + ',' + pPropiedad, pTxtIDs, pTxtCaract);
    if (valor != '' && ComprobarValorCampoCorrecto(pEntidad, pPropiedad, valor, pTxtIDs, pTxtCaract))
    {
        if (EsPropiedadMultiIdioma(pEntidad, pPropiedad)) {
            var idControlCampoPes = ObtenerControlEntidadProp(pEntidad + ',' + pPropiedad, TxtRegistroIDs).replace('Campo_', 'divContPesIdioma_');

            if (document.getElementById(idControlCampoPes) != null) {
                var li = $('li.active', $('#' + idControlCampoPes));
                var idiomaActual = li.attr('rel');
                valorAntiguo = $('#' + idControlCampoPes).attr('langActual');
                valor = IncluirTextoIdiomaEnCadena(valorAntiguo, GetValorEncode(valor), idiomaActual);

                if (!ComprobarMultiIdiomaCorrectoTexto(pEntidad, pPropiedad, valor)) {
                    return;
                }

                $('#' + idControlCampoPes).attr('langActual', '');
            }
            else { //Es multiIdioma sin pestaña
                valor += '@' + IdiomaDefectoFormSem + '[|lang|]';
                var idiomas = IdiomasConfigFormSem.split(',');
                for (var i = 0; i < idiomas.length; i++) {
                    if (idiomas[i] != IdiomaDefectoFormSem) {
                        var valorPropIdioma = ObtenerValorEntidadPropDeIdioma(pEntidad + ',' + pPropiedad, pTxtIDs, pTxtCaract, idiomas[i]);

                        if (valorPropIdioma != '') {
                            valor += valorPropIdioma + '@' + idiomas[i] + '[|lang|]';
                        }
                    }
                }
            }
        }

        var perteneceEntAGrupoPanel = PerteneceEntidadAAlgunGrupoPanelSinEditar(pEntidad, pTxtCaract, pTxtElemEditados);
        var entidadAlmacenar = pEntidad;

        if (perteneceEntAGrupoPanel)
        {
            entidadAlmacenar += '&ULT';
        }

        PutElementoGuardado(entidadAlmacenar, pPropiedad, GetValorEncode(valor), pTxtValores, pTxtElemEditados);

        DarValorControl(pEntidad + ',' + pPropiedad, pTxtIDs, pTxtCaract, '');

        if (EsPropiedadMultiIdiomaSinPesanya(pEntidad, pPropiedad)) {
            var idiomas = IdiomasConfigFormSem.split(',');
            for (var i = 0; i < idiomas.length; i++) {
                if (idiomas[i] != IdiomaDefectoFormSem) {
                    DarValorControlConIdioma(pEntidad + ',' + pPropiedad, pTxtIDs, pTxtCaract, '', idiomas[i]);
                }
            }
        }

        AgregarValorAContenedorGrupoValores(pControlContValores, valor, entidadAlmacenar, pPropiedad, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados);
        
        if (ExcedeCardinalidadMaxima(pEntidad, pPropiedad, pTxtValores, pTxtCaract, pTxtElemEditados))
        {
            var idControlCampo = ObtenerControlEntidadProp(pEntidad + ',' + pPropiedad, pTxtIDs);
            document.getElementById(idControlCampo.replace('Campo_','lbCrear_')).style.display = 'none';
        }
    }
}

function GuardarValorADataNoFuncionalProp(pEntidad, pPropiedad, pControlContValores, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados)
{
    var entidadAlmacenar = pEntidad;
    pEntidad = pEntidad.replace('&ULT', '');
    var valor = ObtenerValorEntidadProp(pEntidad + ',' + pPropiedad, pTxtIDs, pTxtCaract);
    
    if (valor != '' && ComprobarValorCampoCorrecto(pEntidad, pPropiedad, valor, pTxtIDs, pTxtCaract))
    {
        if (EsPropiedadMultiIdioma(pEntidad, pPropiedad)) {
            var idControlCampoPes = ObtenerControlEntidadProp(pEntidad + ',' + pPropiedad, TxtRegistroIDs).replace('Campo_', 'divContPesIdioma_');

            if (document.getElementById(idControlCampoPes) != null) {
                var li = $('li.active', $('#' + idControlCampoPes));
                var idiomaActual = li.attr('rel');
                valorAntiguo = $('#' + idControlCampoPes).attr('langActual');
                valor = IncluirTextoIdiomaEnCadena(valorAntiguo, GetValorEncode(valor), idiomaActual);

                if (!ComprobarMultiIdiomaCorrectoTexto(pEntidad, pPropiedad, valor)) {
                    return;
                }

                $('#' + idControlCampoPes).attr('langActual', '');
            }
            else { //Es multiIdioma sin pestaña
                valor += '@' + IdiomaDefectoFormSem + '[|lang|]';
                var idiomas = IdiomasConfigFormSem.split(',');
                for (var i = 0; i < idiomas.length; i++) {
                    if (idiomas[i] != IdiomaDefectoFormSem) {
                        var valorPropIdioma = ObtenerValorEntidadPropDeIdioma(pEntidad + ',' + pPropiedad, pTxtIDs, pTxtCaract, idiomas[i]);

                        if (valorPropIdioma != '') {
                            valor += valorPropIdioma + '@' + idiomas[i] + '[|lang|]';
                        }
                    }
                }
            }
        }

        SetValorElementoEditadoGuardado(entidadAlmacenar, pPropiedad, GetValorEncode(valor), pTxtValores, pTxtElemEditados);
        DarValorControl(pEntidad + ',' + pPropiedad, pTxtIDs, pTxtCaract, '');

        if (EsPropiedadMultiIdiomaSinPesanya(pEntidad, pPropiedad)) {
            var idiomas = IdiomasConfigFormSem.split(',');
            for (var i = 0; i < idiomas.length; i++) {
                if (idiomas[i] != IdiomaDefectoFormSem) {
                    DarValorControlConIdioma(pEntidad + ',' + pPropiedad, pTxtIDs, pTxtCaract, '', idiomas[i]);
                }
            }
        }

        GuardarValorEnContenedorGrupoValores(pControlContValores, valor, entidadAlmacenar, pPropiedad, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados);
        MarcarElementoEditado(entidadAlmacenar, pPropiedad, -1, pTxtElemEditados, pTxtCaract);
        EstablecerBotonesGrupoValores(pEntidad, pPropiedad, pTxtIDs);
        
        if (ExcedeCardinalidadMaxima(pEntidad, pPropiedad, pTxtValores, pTxtCaract, pTxtElemEditados))
        {
            var idControlCampo = ObtenerControlEntidadProp(pEntidad + ',' + pPropiedad, pTxtIDs);
            document.getElementById(idControlCampo.replace('Campo_','lbCrear_')).style.display = 'none';
        }
    }
}

function EliminarValorDeDataNoFuncionalProp(pNumElem, pEntidad, pPropiedad, pControlContValores, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados)
{
    TxtHackHayCambios = true;

    MarcarElementoEditado(pEntidad, pPropiedad, pNumElem, pTxtElemEditados, pTxtCaract);

    DeleteElementoEditadoGuardado(pEntidad, pPropiedad, pTxtValores, pTxtElemEditados);

    var idControl = ObtenerControlEntidadProp(pEntidad + ',' + pPropiedad, pTxtIDs);
    if (document.getElementById(idControl.replace('Campo_', 'chkImgPrincfileUpLoad_')) != null) {
        EliminarImagenPrincipalProp(pEntidad, pPropiedad);
    }

    DarValorControl(pEntidad + ',' + pPropiedad, pTxtIDs, pTxtCaract, '');

    if (EsPropiedadMultiIdiomaSinPesanya(pEntidad, pPropiedad)) {
        var idiomas = IdiomasConfigFormSem.split(',');
        for (var i = 0; i < idiomas.length; i++) {
            if (idiomas[i] != IdiomaDefectoFormSem) {
                DarValorControlConIdioma(pEntidad + ',' + pPropiedad, pTxtIDs, pTxtCaract, '', idiomas[i]);
            }
        }
    }

    GuardarValorEnContenedorGrupoValores(pControlContValores, '', pEntidad, pPropiedad, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados);
    MarcarElementoEditado(pEntidad, pPropiedad, -1, pTxtElemEditados, pTxtCaract);
    
    var idControlCampo = ObtenerControlEntidadProp(pEntidad + ',' + pPropiedad, pTxtIDs);
    document.getElementById(idControlCampo.replace('Campo_','lbCrear_')).style.display = '';
    document.getElementById(idControlCampo.replace('Campo_', 'lbGuardar_')).style.display = 'none';
    $('#' + idControlCampo.replace('Campo_', 'divContPesIdioma_')).attr('langActual', '');//Si es multiIdioma reiniciará los valres editados
}


/**
 * Esta función verifica si una entidad pertenece a algún grupo de paneles sin editar. 
 * Primero, obtiene todas las propiedades asociadas a la entidad mediante la función GetEntidadYPropiedadConEntidadComoRango. 
 * Luego, itera sobre estas propiedades para verificar si alguna de ellas está siendo editada. 
 * Si encuentra alguna propiedad en edición, devuelve false, indicando que la entidad no pertenece a ningún grupo de paneles sin editar. Si ninguna propiedad está siendo editada, utiliza la función PerteneceEntidadAlgonaProp_LO para determinar si la entidad pertenece a algún grupo de paneles. Si ninguna propiedad de la entidad está en edición y tampoco pertenece a algún grupo de paneles, devuelve false, indicando que la entidad no pertenece a ningún grupo de paneles sin editar.
 * @param {any} pEntidad El nombre de la entidad que se desea verificar.
 * @param {any} pTxtCaract El identificador del elemento HTML donde se almacenan las características de las propiedades.
 * @param {any} pTxtElemEditados El identificador del elemento HTML donde se almacenan los elementos XML editados.
 * @returns
 */
function PerteneceEntidadAAlgunGrupoPanelSinEditar(pEntidad, pTxtCaract, pTxtElemEditados)
{
    var entProps = GetEntidadYPropiedadConEntidadComoRango(pEntidad);

    for (var i = 0; i < entProps.length; i++) {
        var entidad = entProps[i][0];
        var propiedad = entProps[i][1];

        if (GetNumEdicionEntProp(entidad, propiedad, pTxtElemEditados) != -1) //Se está editando propiedad:
        {
            return false;
        }

        //var tipo = GetCaracteristicaPropiedad(entidad, propiedad, pTxtCaract, 'tipo');

        //if (tipo == 'LO' || tipo == 'CO') {
        //    return true;
        //}
    }
    
    return PerteneceEntidadAlgonaProp_LO(pEntidad);
}

function PerteneceEntidadAlgonaProp_LO(pEntidad) {
    var entProps = GetEntidadYPropiedadConEntidadComoRango(pEntidad);

    for (var i = 0; i < entProps.length; i++) {
        var entidad = entProps[i][0];
        var propiedad = entProps[i][1];

        var tipo = GetCaracteristicaPropiedad(entidad, propiedad, TxtCaracteristicasElem, 'tipo');

        if (tipo == 'LO') {
            return true;
        }

        return PerteneceEntidadAlgonaProp_LO(entidad);
    }

    return false;
}

function GetEntidadYPropiedadConEntidadComoRango(pEntidad) {
    var caract = document.getElementById(TxtCaracteristicasElem).value;
    var claveRango = 'rango=' + pEntidad + ',';
    var indiceSigRango = caract.indexOf(claveRango);

    if (indiceSigRango == -1) {
        var superClases = GetSuperClasesEntidad(pEntidad);

        if (superClases != null) {
            for (var i = 0; i < superClases.length; i++) {
                if (superClases[i] != '') {
                    claveRango = 'rango=' + superClases[i] + ',';
                    indiceSigRango = caract.indexOf(claveRango);

                    if (indiceSigRango != -1) {
                        break;
                    }
                }
            }
        }
    }

    var entProps = [];

    while (indiceSigRango != -1) {
        var caracteristica = caract.substring(0, indiceSigRango);
        caracteristica = caracteristica.substring(caracteristica.lastIndexOf('|') + 1);

        var entidad = caracteristica.substring(0, caracteristica.indexOf(','));
        caracteristica = caracteristica.substring(caracteristica.indexOf(',') + 1);
        var propiedad = caracteristica.substring(0, caracteristica.indexOf(','));

        entProps.push([entidad, propiedad]);

        caract = caract.substring(indiceSigRango + claveRango.length);
        indiceSigRango = caract.indexOf(claveRango);
    }

    return entProps;
}

function DarValorControl(pEntPropID, pTxtIDs, pTxtCaract, pValor)
{
    DarValorControlConIdioma(pEntPropID, pTxtIDs, pTxtCaract, pValor, null);
}

function DarValorControlConIdioma(pEntPropID, pTxtIDs, pTxtCaract, pValor, pIdioma)
{
    var idControl = ObtenerControlEntidadProp(pEntPropID, pTxtIDs);
    var tipoCampo = ObtenerTipoCampo(pEntPropID, pTxtCaract);

    if (pIdioma != null) {
        idControl += '_lang_' + pIdioma;
    }
    
    if (pEntPropID == 'EmploymentPeriod,StartDate' || pEntPropID == 'EmploymentPeriod,EndDate' || pEntPropID == 'EmploymentPeriodPosition,StartDate' || pEntPropID == 'EmploymentPeriodPosition,EndDate' || pEntPropID == 'AttendancePeriod,StartDate' || pEntPropID == 'AttendancePeriod,EndDate' || pEntPropID == 'UserArea_PersonQualifications,FechaInicio' || pEntPropID == 'UserArea_PersonQualifications,FechaFin')
    {
        if (pValor != '')
        {
            try
            {
                var mes = pValor.substring(3,5);
                var anio = pValor.substring(6);
                
                document.getElementById(idControl).children[0].value = ObtenerMesTexto(mes);
                document.getElementById(idControl).children[1].value = anio;
                
                document.getElementById(idControl).children[0].style.color = 'black';
                document.getElementById(idControl).children[1].style.color = 'black';
            }
            catch(ex){}
        }
        else
        {
            document.getElementById(idControl).children[0].selectedIndex =0;
            document.getElementById(idControl).children[1].value = 'Año';
            
            document.getElementById(idControl).children[0].style.color = 'gray';
            document.getElementById(idControl).children[1].style.color = 'gray';
        }
    }
    else if (tipoCampo == 'Tiny')
    {     
        /* Modificado para TinyMCE */

        // Asociar el valor al input
        if (idControl != '') {
            $('#' + idControl).val(pValor);
            // Obtener el id del TinyMCE asociado a este input
            const inputTinyIdRelated = $('#' + idControl).data("editorrelated");

            // Obtener los TinyMCE existentes
            const tinyMCEEditors = tinymce.get();
            // Buscar el TinyMCE asociado al input para cargar sus datos
            var tinyMCEForInput = Object.keys(tinyMCEEditors).map(function (key) {
                return tinyMCEEditors[key];
            }).find(function (tinyInstance) {
                return tinyInstance.id === inputTinyIdRelated;
            });

            // Asociar el valor al TinyMCE concreto
            if (tinyMCEForInput) {
                tinyMCEForInput.setContent(pValor);
            }
        }
    }
    else if (document.getElementById(idControl) != null)
    {
        
        if (document.getElementById(idControl).tagName == 'SELECT')
        {
            if (pValor == '')
            {
                document.getElementById(idControl).selectedIndex = 0;
            }
            else
            {
                document.getElementById(idControl).value = pValor;
            }
            
            if (pValor == '' && GetValorDefNoSelec(pEntPropID.split(',')[0], pEntPropID.split(',')[1], pTxtCaract) != null)
            {
                document.getElementById(idControl).style.color = "gray";
            }
            else
            {
                document.getElementById(idControl).style.color = "black";
            }
        }
        else
        {
            var valorAnterior = document.getElementById(idControl).value;
            document.getElementById(idControl).value = pValor;

            if (tipoCampo == 'Boleano') {
                document.getElementById('chkSi_' + idControl).checked = ((pValor == '' || pValor == 'true') && (valorAnterior == 'true' || valorAnterior == ''));
                document.getElementById('chkNo_' + idControl).checked = ((pValor == '' || pValor == 'false') && (valorAnterior == 'false'));

                if (pValor == '') {
                    if (valorAnterior == 'false') {
                        document.getElementById(idControl).value = 'false';
                    }
                    else {
                        document.getElementById(idControl).value = 'true';
                    }
                }
            }
            else if ((tipoCampo == 'DateTime' || tipoCampo == 'Date' || tipoCampo == 'Time') && pValor.indexOf('/') == -1 && pValor.length == 14) {
                document.getElementById(idControl).value = pValor.substring(6, 8) + '/' + pValor.substring(4, 6) + '/' + pValor.substring(0, 4);

                if (GetCaracteristicaPropiedad(pEntPropID.split(',')[0], pEntPropID.split(',')[1], TxtCaracteristicasElem, 'propFechaConHora') != null) {
                    document.getElementById(idControl).value += ' ' + pValor.substring(8, 10) + ':' + pValor.substring(10, 12) + ':' + pValor.substring(12);
                }
            }

            if (tipoCampo == 'Imagen' || tipoCampo == 'Archivo' || tipoCampo == 'Video' || tipoCampo == 'ArchivoLink') {
                if (pValor != '') {
                    $('#' + idControl.replace('Campo_', 'divAgregarArchivo_')).css('display', 'none');
                    $('#' + idControl.replace('Campo_', 'divArchivoAgregado_')).css('display', '');

                    if (tipoCampo == 'Imagen') {
                        var urlContent = $('input.inpt_baseURLContent').val();
                        $('#' + idControl.replace('Campo_', 'archVistaPre_')).html('<img src="' + urlContent + "/" + pValor + '" alt="' + pValor + '" />');
                    }
                    else {
                        var valorVisPre = pValor;

                        if ((tipoCampo == 'ArchivoLink' || tipoCampo == 'Archivo') && valorVisPre.indexOf('_') != -1 && valorVisPre.indexOf('.') != -1) {
                            var guidValor = valorVisPre.substring(0, valorVisPre.lastIndexOf("."));
                            guidValor = guidValor.substring(guidValor.lastIndexOf("_") + 1);

                            if (guidValor.length == 36) {
                                valorVisPre = valorVisPre.substring(0, valorVisPre.lastIndexOf("_")) + valorVisPre.substring(valorVisPre.lastIndexOf("."));
                            }
                        }

                        if (tipoCampo == 'ArchivoLink' && valorVisPre.indexOf('/') != -1) {
                            valorVisPre = valorVisPre.substring(valorVisPre.lastIndexOf('/') + 1);
                        }

                        $('#' + idControl.replace('Campo_', 'archVistaPre_')).html(valorVisPre);
                    }
                }
                else {
                    $('#' + idControl.replace('Campo_', 'divAgregarArchivo_')).css('display', '');
                    $('#' + idControl.replace('Campo_', 'divArchivoAgregado_')).css('display', 'none');

                    $('#' + idControl.replace('Campo_', 'archVistaPre_')).html();
                }

                if (document.getElementById(idControl.replace('Campo_', 'chkImgPrincfileUpLoad_')) != null) {
                    if (pValor != '') {
                        document.getElementById(idControl.replace('Campo_', 'chkImgPrincfileUpLoad_')).disabled = false;
                    }
                    else {
                        document.getElementById(idControl.replace('Campo_', 'chkImgPrincfileUpLoad_')).disabled = true;
                    }

                    var entProp = pEntPropID.split(',');
                    var imgPrinc = ObtenerImagenPrincipal(entProp[0], entProp[1]);

                    if (imgPrinc != '' && imgPrinc == pValor) {
                        document.getElementById(idControl.replace('Campo_', 'chkImgPrincfileUpLoad_')).checked = true;
                    }
                    else {
                        document.getElementById(idControl.replace('Campo_', 'chkImgPrincfileUpLoad_')).checked = false;
                    }
                }
            }
            else if (idControl.indexOf('selEnt_') != -1 && document.getElementById(idControl.replace('selEnt_', 'hack_')) != null) {
                if (pValor == '') {
                    document.getElementById(idControl.replace('selEnt_', 'hack_')).value = pValor;
                }
                else {
                    var textoEntidad = ObtenerTextoRepEntidadExterna(pValor);

                    if (textoEntidad != '') {
                        document.getElementById(idControl.replace('selEnt_', 'hack_')).value = textoEntidad;
                    }
                }
            }
            else {
                var entProp = pEntPropID.split(',');
                if (GetEsPropiedadGrafoDependiente(entProp[0], entProp[1], TxtCaracteristicasElem)) {
                    if (pValor != '') {
                        var valoresGraf = document.getElementById(TxtValoresGrafoDependientes).value;

                        if (valoresGraf.indexOf(pValor) != -1) {
                            var trozo = valoresGraf.substring(valoresGraf.indexOf(pValor));
                            trozo = trozo.substring(trozo.indexOf(',') + 1, trozo.indexOf('|'));
                            document.getElementById(idControl.replace('Campo_', 'hack_')).value = trozo;
                        }

                        HabilitarCamposGrafoDependientes(entProp[0], entProp[1]);
                    }
                    else {
                        //document.getElementById(idControl.replace('Campo_', 'hack_')).value = '';
                        EliminarValoresGrafoDependientes(entProp[0], entProp[1], true, false);
                    }
                }
                else if (document.getElementById(idControl.replace('Campo_', 'hackSec_')) != null) {
                    document.getElementById(idControl.replace('Campo_', 'hackSec_')).value = pValor;
                }
            }
        }
    }
}

function ReemplarIDsCatTesSemPorNombre(pValor) {
    var nombresCat = document.getElementById(TxtNombreCatTesSem).value;

    if (pValor.length > 0 && pValor.indexOf('|') == (pValor.length - 1)) {
        pValor = pValor.substring(0, pValor.length - 1);
    }

    var valorArray = pValor.split(',');
    pValor = '';

    for (var i = 0; i < valorArray.length; i++) {
        if (valorArray[i] != '') {
            if (nombresCat.indexOf(valorArray[i] + '|') != -1) {
                nombresCat = nombresCat.substring(nombresCat.indexOf(valorArray[i] + '|') + (valorArray[i] + '|').length);
                pValor += nombresCat.substring(0, nombresCat.indexOf('|||')) + ' > ';

                nombresCat = document.getElementById(TxtNombreCatTesSem).value;
            }
            else {
                pValor += valorArray[i] + ' > ';
            }
        }
    }

    if (pValor != '') {
        pValor = pValor.substring(0, pValor.length - 3);
    }

    return pValor;
}

function AgregarValorAContenedorGrupoValores(pControlCont, pValor, pEntidad, pPropiedad, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados)
{
    if (document.getElementById(pControlCont) != null) {
        if (pControlCont.indexOf('contEntSelec_') != -1 && document.getElementById(pControlCont.replace('contEntSelec_', 'bkHtmlSelEnt_')) != null) {
            pValor = ReemplarIDsCatTesSemPorNombre(pValor);
        }

        pValor = GetValorDecode(pValor);
        if (pValor.indexOf('[|lang|]') != -1) {
            pValor = ExtraerTextoIdioma(pValor, IdiomaDefectoFormSem)
        }

        if (document.getElementById(pControlCont).innerHTML == '')//Creo la tabla
        {
            document.getElementById(pControlCont).innerHTML = '<table><tbody>' + ObtenerFilaValorContenedorGrupoValores(pControlCont, pValor, 0, true, pEntidad, pPropiedad, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados) + '</tbody></table>';
        }
        else {
            var numElem = document.getElementById(pControlCont).children[0].children[0].children.length;
            document.getElementById(pControlCont).children[0].children[0].innerHTML += ObtenerFilaValorContenedorGrupoValores(pControlCont, pValor, numElem, true, pEntidad, pPropiedad, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados);
        }
    }
}

function GuardarValorEnContenedorGrupoValores(pControlCont, pValor, pEntidad, pPropiedad, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados)
{
    var numElem = GetNumEdicionEntProp(pEntidad, pPropiedad, pTxtElemEditados);
    if (pValor != '')
    {
        pValor = GetValorDecode(pValor);
        if (pValor.indexOf('[|lang|]') != -1) {
            pValor = ExtraerTextoIdioma(pValor, IdiomaDefectoFormSem)
        }

        document.getElementById(pControlCont).children[0].children[0].children[numElem].innerHTML = ObtenerFilaValorContenedorGrupoValores(pControlCont, pValor, numElem, false, pEntidad, pPropiedad, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados);
    }
    else //Hay que eliminarlo
    {
        var hijos = document.getElementById(pControlCont).children[0].children[0].children;
        var htmlFinal = '';
        
        for (var i=0;i<hijos.length;i++)
        {
            if (i != numElem)
            {
                var clase = hijos[i].className;
                
                if (i>numElem) //Invertimos clase
                {
                    if (clase == 'par')
                    {
                        clase = 'impar';
                    }
                    else
                    {
                        clase = 'par';
                    }
                    
                    htmlFinal += '<tr class="'+clase+'">' + hijos[i].innerHTML.replace(', \''+i,', \'' + (i-1)).replace('(\''+i,'(\''+(i-1)) + '</tr>';
                }
                else
                {
                    htmlFinal += '<tr class="'+clase+'">' + hijos[i].innerHTML + '</tr>';
                }
            }
        }
        
        document.getElementById(pControlCont).children[0].children[0].innerHTML = htmlFinal;
    }
}

function ObtenerFilaValorContenedorGrupoValores(pControlCont, pValor, pNumElem, pAgregarTR, pEntidad, pPropiedad, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados){
    var fila = '';
    var escaparComillaSimple = new RegExp('\'', 'g');
    if (pAgregarTR)
    {
        var claseFila = 'impar';
        
        if(pNumElem %2 ==0)
        {
            claseFila = 'par';
        }
        
        fila += '<tr class="'+claseFila+'">';
    }
    
    var funcionSel = 'SeleccionarElementoGrupoValores';
    var funcionEli = 'EliminarValorDeDataNoFuncionalProp';
    
    if (pControlCont.indexOf('contEntSelec_') != -1)//Es entidad seleccionada
    {
        funcionSel = 'SeleccionarObjectNoFuncionalSeleccEnt';
        funcionEli = 'EliminarObjectNoFuncionalSeleccEnt';

        if (document.getElementById(pControlCont.replace('contEntSelec_', 'hack_')) != null || document.getElementById(pControlCont.replace('contEntSelec_', 'bkHtmlSelEnt_')) != null) {
            funcionSel = '';
        }
    }

    fila += '<td><span>' + pValor + '</span></td>';

    if (funcionSel != '') {
        //fila += '<td><a onclick="' + funcionSel + '(\'' + pEntidad + '\', \'' + pPropiedad + '\', \'' + pNumElem + '\', \'' + pTxtValores + '\', \'' + pTxtIDs + '\', \'' + pTxtCaract + '\', \'' + pTxtElemEditados + '\')"><img src="' + GetUrlImg(pTxtCaract) + 'icoEditar.gif"></a></td>';                                
        fila += `<td class="tdaccion">
                    <a href="javascript: void(0);"style="color: var(--c-primario)" 
                        onclick="${funcionSel}('${pEntidad}', '${pPropiedad}','${pNumElem}','${pTxtValores}','${pTxtIDs}','${pTxtCaract}','${pTxtElemEditados}');">
                        <span class="material-icons pr-0">edit</span>
                    </a>
                </td>`;        
    }

    // Cambio por nuevo Front
    //var metodoEliminar = funcionEli + '(\\\'' + pNumElem + '\\\',\\\'' + pEntidad + '\\\', \\\'' + pPropiedad + '\\\', \\\'' + pControlCont + '\\\', \\\'' + pTxtValores + '\\\', \\\'' + pTxtIDs + '\\\', \\\'' + pTxtCaract + '\\\', \\\'' + pTxtElemEditados + '\\\');';
    var metodoEliminar = `${funcionEli}('${pNumElem}', '${pEntidad}', '${pPropiedad}', '${pControlCont}', '${pTxtValores}', '${pTxtIDs}', '${pTxtCaract}', '${pTxtElemEditados}')`;

    // Cambio por nuevo Front
    //fila += '<td><a class="remove" onclick="MostrarPanelConfirmacionEvento(event, \'' + textoFormSem.confimEliminar.replace('@1@', pValor) + '\', \'' + metodoEliminar + '\')"></a></td>';
    /*fila += `<td>
                <a class="remove" onclick="MostrarPanelConfirmacionEvento(event, '${textoFormSem.confimEliminar.replace('@1@', pValor)}', '${metodoEliminar}')"></a>
            </td>`;
    */
    fila += `<td>
                <a href="javascript: void(0)"                
                class="removeButton"
                data-showmodalcentered="1"
                onclick="
                        $('#modal-container').modal('show', this);
                        AccionFichaPerfil(
                            '${textoRecursos.Eliminar}',                                
                            '${borr.si.charAt(0).toUpperCase()}${borr.si.slice(1)}',
                            '${borr.no.charAt(0).toUpperCase()}${borr.no.slice(1)}',
                            '${textoFormSem.confimEliminar.replace('@1@', pValor.replace(escaparComillaSimple, '\\\''))}',
                            'sin-definir',
                            function () {
                              ${metodoEliminar};
                              $('#modal-container').modal('hide');
                            }
                        );
                        "
                >
                    <span class="material-icons pr-0">delete</span>
                </a>

            </td>`;    
    if (pAgregarTR)
    {
        fila += '</tr>';
    }
    
    return fila;
}

function GetUrlImg(pTxtCaract){
    var caract = document.getElementById(pTxtCaract).value;
    return caract.substring(0, caract.indexOf('|')).replace('$baseUrlStatic=', '') + '/img/';
}

function GetUrlVideos(pTxtCaract){
    var caract = document.getElementById(pTxtCaract).value;
    caract = caract.substring(caract.indexOf('$baseUrlContent='));
    return caract.substring(0, caract.indexOf('|')).replace('$baseUrlContent=', '') + '/videos/';
}

function GetUrlContent() {
    var caract = document.getElementById(TxtCaracteristicasElem).value;
    caract = caract.substring(caract.indexOf('$baseUrlContent='));
    return caract.substring(0, caract.indexOf('|')).replace('$baseUrlContent=', '') + '/';
}

function GetIDTxtHackImgPrincipal(pTxtCaract){
    var caract = document.getElementById(pTxtCaract).value;
    caract = caract.substring(caract.indexOf('$txtHackImgPrincipal='));
    return caract.substring(0, caract.indexOf('|')).replace('$txtHackImgPrincipal=', '');
}

function GetPropLangBusqCom() {
    if (document.getElementById(TxtCaracteristicasElem) == null) {
        return null;
    }

    var caract = document.getElementById(TxtCaracteristicasElem).value;
    
    if (caract.indexOf('$propLangBusqCom=') != -1) {
        caract = caract.substring(caract.indexOf('$propLangBusqCom='));
        return caract.substring(0, caract.indexOf('|')).replace('$propLangBusqCom=', '');
    }
    else {
        return null;
    }
}

function GetValorEncode(pValor){
    //return pValor.replace("<", "[--C]").replace(">", "[C--]")
    return pValor.replace(/\</g,'[--C]').replace(/\>/g, "[C--]");
}

function GetValorDecode(pValor){
    //return pValor.replace("[--C]", "<").replace("[C--]", ">")
    return pValor.replace(/\[--C]/g,'<').replace(/\[C--]/g, ">");
}

function SeleccionarElementoGrupoValores(pEntidad, pPropiedad, pNumElem, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados) {
    TxtHackHayCambios = true;
    var valorRdf = document.getElementById(pTxtValores).value;
    MarcarElementoEditado(pEntidad, pPropiedad, pNumElem, pTxtElemEditados, pTxtCaract);
    valor = GetValorElementoGuardado(pEntidad, pPropiedad, pTxtValores, pTxtElemEditados, pNumElem);
    valor = GetValorDecode(valor);

    var idControlCampo = ObtenerControlEntidadProp(pEntidad.replace('&ULT', '') + ',' + pPropiedad, pTxtIDs);
    var onclickGuardar = $('#' + idControlCampo.replace('Campo_', 'lbGuardar_')).attr('onclick');

    if (onclickGuardar.indexOf('&ULT') != -1) {
        onclickGuardar = onclickGuardar.replace('&ULT', '');
        $('#' + idControlCampo.replace('Campo_', 'lbGuardar_')).attr('onclick', onclickGuardar);
    }
    
    if (pEntidad.indexOf('&ULT') != -1)
    {
        pEntidad = pEntidad.substring(0, pEntidad.indexOf('&ULT'));
        $('#' + idControlCampo.replace('Campo_', 'lbGuardar_')).attr('onclick', onclickGuardar.replace(pEntidad, pEntidad +'&ULT'));
    }

    if (EsPropiedadMultiIdioma(pEntidad, pPropiedad)) {
        var idControlCampoPes = ObtenerControlEntidadProp(pEntidad + ',' + pPropiedad, TxtRegistroIDs).replace('Campo_', 'divContPesIdioma_');

        if (document.getElementById(idControlCampoPes) != null) {
            var li = $('li.active', $('#' + idControlCampoPes));
            var idiomaActual = li.attr('rel');
            $('#' + idControlCampoPes).attr('langActual', valor);
            valor = ExtraerTextoIdioma(valor, idiomaActual);
        }
        else { //Es multiIdioma sin pestaña
            var idiomas = IdiomasConfigFormSem.split(',');
            for (var i = 0; i < idiomas.length; i++) {
                if (idiomas[i] != IdiomaDefectoFormSem) {
                    DarValorControlConIdioma(pEntidad + ',' + pPropiedad, pTxtIDs, pTxtCaract, ExtraerTextoIdioma(valor, idiomas[i]), idiomas[i]);
                }
            }

            valor = ExtraerTextoIdioma(valor, IdiomaDefectoFormSem);
        }
    }
    
    DarValorControl(pEntidad + ',' + pPropiedad, pTxtIDs, pTxtCaract, valor);
    
    document.getElementById(idControlCampo.replace('Campo_','lbCrear_')).style.display = 'none';
    document.getElementById(idControlCampo.replace('Campo_', 'lbGuardar_')).style.display = '';
}

/**
 * Función que marca un elemento como editado en un campo de texto que contiene elementos editados. 
 * Si el elemento ya está marcado como editado, se actualiza su número de edición. 
 * Además, si el elemento es una propiedad de un grupo de paneles, también se marcan como editadas todas las propiedades hijas del grupo de paneles.
 * @param {any} pEntidad
 * @param {any} pPropiedad
 * @param {any} pNumElem
 * @param {any} pTxtElemEditados
 * @param {any} pTxtCaract
 */
function MarcarElementoEditado(pEntidad, pPropiedad, pNumElem, pTxtElemEditados, pTxtCaract){
    var editados = document.getElementById(pTxtElemEditados).value;
    var idElem = pEntidad;
    if (pPropiedad != null)
    {
        idElem += ',' + pPropiedad;
    }
    
    idElem += '=';
    
    if (editados.indexOf(idElem) != -1) //Elimino el elemento:
    {
        document.getElementById(pTxtElemEditados).value = editados.substring(0, editados.indexOf(idElem));
        editados = editados.substring(editados.indexOf(idElem));
        editados = editados.substring(editados.indexOf('|') + 1);
        document.getElementById(pTxtElemEditados).value += editados;
        
        if (EsPropiedadGrupoPaneles(pEntidad, pPropiedad, pTxtCaract)) //Desmarco todas sus propiedades hijas:
        {
            var entidadHija = GetRangoPropiedad(pEntidad, pPropiedad, pTxtCaract);
            var propiedades = GetPropiedadesEntidad(entidadHija, pTxtCaract);
            
            for (var i=0;i<propiedades.length;i++)
            {
                if (propiedades[i] != '')
                {
                    MarcarElementoEditado(entidadHija, propiedades[i], -1, pTxtElemEditados, pTxtCaract)
                }
            }
        }
    }
    
    if (pNumElem != -1)
    {
        document.getElementById(pTxtElemEditados).value += idElem + pNumElem + '|';
    }
}

/**
 * Esta función determina si una propiedad específica pertenece a un grupo de paneles, verificando si el tipo de propiedad es 'LO' (Lista de opciones).
 * @param {any} pEntidad El nombre de la entidad asociada a la propiedad.
 * @param {any} pPropiedad El nombre de la propiedad que se está evaluando.
 * @param {any} pTxtCaract El identificador del campo de texto que almacena las características relacionadas.
 * @returns
 */
function EsPropiedadGrupoPaneles(pEntidad, pPropiedad, pTxtCaract){
    return (GetTipoPropiedad(pEntidad, pPropiedad, pTxtCaract) == 'LO');
}

function GetValorElementoEnPosicion(pElemento, pNumElem, pValores){
    var i = pNumElem;
    var trozo = pValores;
    
    //Compruebo que no esté seleccionando un elemento candidato a ser agregado:
    if (pValores.indexOf('<||>') != -1)
    {
        var trozoAux = pValores.substring(pValores.indexOf('<||>') + 4);
        if (trozoAux.indexOf('<' + pElemento + '>') != -1)
        {
            trozo = trozoAux;
        }
    }
    
    while (i>0 && trozo.length > 0)
    {
        trozo = ObtenerTextoDespuesElemento(trozo, pElemento);
        i--;
    }
    
    return ObtenerValorTextoElemento(trozo, pElemento);
}

/**
 * La función GetNumEdicionEntProp se utiliza para obtener el número de edición de una entidad y propiedad específicas, en un contexto donde se lleva un seguimiento de los elementos editados. 
 * Esta función busca en un texto que contiene los elementos editados para encontrar el número de edición correspondiente a la entidad y propiedad proporcionadas.
 * @param {any} pEntidad: La entidad para la cual se desea obtener el número de edición.
 * @param {any} pPropiedad: La propiedad específica de la entidad para la cual se desea obtener el número de edición (puede ser null si no se aplica).
 * @param {any} pTxtElemEditados: El texto que contiene los elementos editados, donde se buscará el número de edición.
 * @returns {any} El número de edición de la entidad y propiedad especificadas, si se encuentra en el texto de elementos editados.  -1 si la entidad y propiedad no se han editado o no se encuentra el número de edición correspondiente.
 */ 
function GetNumEdicionEntProp(pEntidad, pPropiedad, pTxtElemEditados){
    var editados = document.getElementById(pTxtElemEditados).value;
    
    var ideElemento = pEntidad;
    
    if (pPropiedad != null)
    {
        ideElemento += ',' + pPropiedad;
    }
    
    ideElemento += '=';
    
    if (editados.indexOf(ideElemento) != -1)
    {
        var valor = editados.substring(editados.indexOf(ideElemento) + ideElemento.length);
        valor = valor.substring(0, valor.indexOf('|'));

        if (isNaN(valor)) {
            return valor;
        }
        else {
            return parseInt(valor);
        }
    }
    else
    {
        return -1;
    }
}

function MostrarPanelConfirmacionEvento(evento, pPregunta, pAccionSi){
    document.getElementById('lblPregunta_ConfirmacionEventos').innerHTML = pPregunta;
    
    //Si no se ejecuta la suguinete línea las funciones se van sumando
    $("#lbSi_ConfirmacionEventos").unbind('click');    
    
    $("#lbSi_ConfirmacionEventos").click( function(){
        eval(pAccionSi);        
        eval("document.getElementById('divPreguntaConfirmacionEventos').style.display='none';");    
    });
    
	if ((typeof CalcularTopPanelYMostrarEspecifico != 'undefined')) {
	    CalcularTopPanelYMostrarEspecifico(evento, 'divPreguntaConfirmacionEventos', pAccionSi);
	}
	else
	{
		CalcularTopPanelYMostrar(evento, 'divPreguntaConfirmacionEventos');
	}
}

function ComprobarModificaciones(evento, pPregunta, pAccionSi, pEntidadHija, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados)
{
    var modificaciones = SeHaModificadoEntidad(pEntidadHija, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados);
    
    if (modificaciones)
    {
        MostrarPanelConfirmacionEvento(evento, pPregunta, pAccionSi);
    }
    else
    {
        eval(pAccionSi);
    }
}

function SeHaModificadoEntidad(pEntidadHija, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados)
{
    var propiedades = GetPropiedadesEntidad(pEntidadHija, pTxtCaract);
    var modificada = false;
    
    for (var i=0;i<propiedades.length;i++)
    {
        if (propiedades[i] != '')
        {
            modificada = (modificada || SeHaModificadoPropiedadDeEntidad(pEntidadHija, propiedades[i], pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados));
        }
    }
    
    return modificada;
}

function SeHaModificadoPropiedadDeEntidad(pEntidad, pPropiedad, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados){
    var tipoProp = GetTipoPropiedad(pEntidad, pPropiedad, pTxtCaract);
    var modificado = false;
    
    if (tipoProp == 'FD' || tipoProp == 'CD')
    {
        var valorProp = ObtenerValorEntidadProp(pEntidad + ',' + pPropiedad, pTxtIDs, pTxtCaract);
        valorProp = GetValorEncode(valorProp);
        var valorGuardado = GetValorElementoEditadoGuardado(pEntidad, pPropiedad, pTxtValores, pTxtElemEditados);
        
        modificado = !(((valorProp == null || valorProp == '') && (valorGuardado == null || valorGuardado == '')) || valorProp == valorGuardado);
    }
    else if (tipoProp == 'VD')
    {
        var valorProp = ',' + ObtenerValorEntidadProp(pEntidad + ',' + pPropiedad, pTxtIDs, pTxtCaract) + ',';
        var valoresProp = valorProp.split(',');
        
        var valorPropGuardado = GetValorElementoGuardado(pEntidad, pPropiedad, pTxtValores, pTxtElemEditados, 0);
        var count = 0;
        while (valorPropGuardado != '')
        {
            count++;
            
            if (valorPropGuardado.indexOf(',' + valorPropGuardado + ',') == -1)
            {
                modificado = true;
                break;
            }
            
            valorProp = GetValorElementoGuardado(pEntidad, pPropiedad, pTxtValores, pTxtElemEditados, count);
        }
        
        if (!modificado)
        {
            for (var i=0;i<valoresProp.length;i++)
            {
                if (valoresProp[i] != '')
                {
                    count--;
                }
            }
            
            if (count != 0)
            {
                modificado = true;
            }
        }
    }
    else if (tipoProp == 'FO' || tipoProp == 'CO' || tipoProp == 'LD' || tipoProp == 'LO' || tipoProp == 'LSEO')
    {//TODO: ESTO FUNCIONA???
        var entidadHija = GetRangoPropiedad(pEntidad, pPropiedad, pTxtCaract);
        modificado = (modificado || SeHaModificadoEntidad(entidadHija, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados));
    }
    
    return modificado;
}

/**
 * Función que se encarga de agregar propiedades obligatorias de edición a una entidad dada.
 * @param {any} pEntidad La entidad a la que se agregarán las propiedades obligatorias.
 * @param {any} pPropiedadYaAgregada La propiedad que ya se ha agregado, si hay alguna. Esto se usa para evitar duplicados.
 * @param {any} pExtraEnt Texto adicional que se agrega al nombre de la entidad.
 */
function AgregarPropsEntidadObligatoriasEdicion(pEntidad, pPropiedadYaAgregada, pExtraEnt) {
    var propiedades = GetPropiedadesEntidad(pEntidad, TxtCaracteristicasElem);
    for (var i = 0; i < propiedades.length; i++) {
        if (propiedades[i] != '' && propiedades[i] != pPropiedadYaAgregada) {
            var tipoProp = GetTipoPropiedad(pEntidad, propiedades[i], TxtCaracteristicasElem);

            if (tipoProp == 'FO' || tipoProp == 'CO') {
                var entidadHija = GetRangoPropiedad(pEntidad, propiedades[i], TxtCaracteristicasElem);
                entidadHija = ObtenerTipoEntidadEditada(entidadHija);
                PutElementoGuardado(pEntidad + pExtraEnt, propiedades[i], '<' + entidadHija + '></' + entidadHija + '>', TxtValorRdf, TxtElemEditados);
            }
        }
    }
}

function ValorPropiedadRepetido(pEntidad, pPropiedad, valor) {
    var i = 0;
    var valorAntiguo = GetValorElementoGuardado(pEntidad, pPropiedad, TxtValorRdf, TxtElemEditados, i);

    while (valorAntiguo != null && valorAntiguo != '') {
        if (valorAntiguo == valor) {
            return true;
        }

        i++;
        valorAntiguo = GetValorElementoGuardado(pEntidad, pPropiedad, TxtValorRdf, TxtElemEditados, i);
    }

    return false;
}

function AgregarObjectNoFuncionalSeleccEnt(pEntidad, pPropiedad, pControlContValores)
{
    TxtHackHayCambios = true;
    var valor = ObtenerValorEntidadProp(pEntidad + ',' + pPropiedad, TxtRegistroIDs, TxtCaracteristicasElem);
    if (valor != '')
    {
        var perteneceEntAGrupoPanel = PerteneceEntidadAAlgunGrupoPanelSinEditar(pEntidad, TxtCaracteristicasElem, TxtElemEditados);
        if (!perteneceEntAGrupoPanel)
        {
            if (ValorPropiedadRepetido(pEntidad, pPropiedad, GetValorEncode(valor))) {
                DarValorControl(pEntidad + ',' + pPropiedad, TxtRegistroIDs, TxtCaracteristicasElem, '');
                return false;
            }

            PutElementoGuardado(pEntidad, pPropiedad, GetValorEncode(valor), TxtValorRdf, TxtElemEditados);
        }
        else
        {
            if (ValorPropiedadRepetido(pEntidad + '&ULT', pPropiedad, GetValorEncode(valor))) {
                DarValorControl(pEntidad + ',' + pPropiedad, TxtRegistroIDs, TxtCaracteristicasElem, '');
                return false;
            }

            var elemAgregado = PutElementoGuardado(pEntidad + '&ULT', pPropiedad, GetValorEncode(valor), TxtValorRdf, TxtElemEditados);

            if (elemAgregado) {
                AgregarPropsEntidadObligatoriasEdicion(pEntidad, pPropiedad, '&ULT');
            }
        }

        var idControlCampo = ObtenerControlEntidadProp(pEntidad + ',' + pPropiedad, TxtRegistroIDs);

        var textoInfo = '';

        if (typeof document.getElementById(idControlCampo).selectedIndex != "undefined") {
            document.getElementById(idControlCampo).options[document.getElementById(idControlCampo).selectedIndex].disabled = true;
            textoInfo = document.getElementById(idControlCampo).options[document.getElementById(idControlCampo).selectedIndex].text;
        }
        else if (document.getElementById(idControlCampo.replace("selEnt_", "hack_")) != null) {
            textoInfo = document.getElementById(idControlCampo.replace("selEnt_", "hack_")).value;
            document.getElementById(idControlCampo.replace("selEnt_", "hack_")).value = '';

            if (textoInfo == '') {
                var count = 0;
                var trozoID = 'extra_' + count + '_hack_';

                while (textoInfo == '' && document.getElementById(idControlCampo.replace("selEnt_", trozoID)) != null) {
                    textoInfo = document.getElementById(idControlCampo.replace("selEnt_", trozoID)).value;
                    document.getElementById(idControlCampo.replace("selEnt_", trozoID)).value = '';
                    count++;
                    trozoID = 'extra_' + count + '_hack_';
                }
            }
        }
        else if (document.getElementById(idControlCampo.replace("selEnt_", "hackTesNameSelEnt_")) != null) {
            textoInfo = document.getElementById(idControlCampo.replace("selEnt_", "hackTesNameSelEnt_")).value;

            if (textoInfo != '' && textoInfo.lastIndexOf(',') == textoInfo.length - 1) {
                textoInfo = textoInfo.substring(0, textoInfo.length - 1);
            }

            if (textoInfo != '' && textoInfo.lastIndexOf('|') == textoInfo.length - 1) {
                textoInfo = textoInfo.substring(0, textoInfo.length - 1);
            }

            textoInfo = textoInfo.replace(/\,/g, ' > ');
        }
        else {
            textoInfo = document.getElementById(idControlCampo).value;
            document.getElementById(idControlCampo).value = '';
        }
        
        DarValorControl(pEntidad + ',' + pPropiedad, TxtRegistroIDs, TxtCaracteristicasElem, '');
        AgregarValorAContenedorGrupoValores(pControlContValores, textoInfo, pEntidad, pPropiedad, TxtValorRdf, TxtRegistroIDs, TxtCaracteristicasElem, TxtElemEditados);

        if (pControlContValores != null && pControlContValores.indexOf('contEntSelec_') != -1 && document.getElementById(pControlContValores.replace('contEntSelec_', 'bkHtmlSelEnt_')) != null)
        {
            ResetearSeleccTesSem(pControlContValores.replace('contEntSelec_', 'selEnt_'));
        }
        
        if (ExcedeCardinalidadMaxima(pEntidad, pPropiedad, TxtValorRdf, TxtCaracteristicasElem, TxtElemEditados))
        {
            document.getElementById(idControlCampo.replace('selEnt_','lbCrear_')).style.display = 'none';
        }

        if (GetCaracteristicaPropiedad(pEntidad, pPropiedad, TxtCaracteristicasElem, 'propSelectEntDep') == 'true') {
            ObtenerSelectoresDependientes(pPropiedad, pEntidad, true);
        }
    }
}

function GuardarObjectNoFuncionalSeleccEnt(pEntidad, pPropiedad, pControlContValores)
{
    var valor = ObtenerValorEntidadProp(pEntidad + ',' + pPropiedad, TxtRegistroIDs, TxtCaracteristicasElem);
    
    if (valor != '')
    {
        SetValorElementoEditadoGuardado(pEntidad, pPropiedad, GetValorEncode(valor), TxtValorRdf, TxtElemEditados);

        var idControlCampo = ObtenerControlEntidadProp(pEntidad + ',' + pPropiedad, TxtRegistroIDs);

        var textoInfo = '';

        if (typeof document.getElementById(idControlCampo).selectedIndex != "undefined") {
            document.getElementById(idControlCampo).options[document.getElementById(idControlCampo).selectedIndex].disabled = true;
            textoInfo = document.getElementById(idControlCampo).options[document.getElementById(idControlCampo).selectedIndex].text;
        }
        else {
            textoInfo = document.getElementById(idControlCampo).value;
            document.getElementById(idControlCampo).value = '';
        }
        
        DarValorControl(pEntidad + ',' + pPropiedad, TxtRegistroIDs, TxtCaracteristicasElem, '');
        GuardarValorEnContenedorGrupoValores(pControlContValores, textoInfo, pEntidad, pPropiedad, TxtValorRdf, TxtRegistroIDs, TxtCaracteristicasElem, TxtElemEditados);
        MarcarElementoEditado(pEntidad, pPropiedad, -1, TxtElemEditados, TxtCaracteristicasElem);
        EstablecerBotonesObjectNoFuncionalSeleccEnt(pEntidad, pPropiedad);
        
        if (ExcedeCardinalidadMaxima(pEntidad, pPropiedad, TxtValorRdf, TxtCaracteristicasElem, TxtElemEditados))
        {
            document.getElementById(idControlCampo.replace('selEnt_','lbCrear_')).style.display = 'none';
        }

        if (GetCaracteristicaPropiedad(pEntidad, pPropiedad, TxtCaracteristicasElem, 'propSelectEntDep') == 'true') {
            ObtenerSelectoresDependientes(pPropiedad, pEntidad, true);
        }
    }
}

function SeleccionarObjectNoFuncionalSeleccEnt(pEntidad, pPropiedad, pNumElem)
{
    TxtHackHayCambios = true;
    var valorRdf = document.getElementById(TxtValorRdf).value;
    MarcarElementoEditado(pEntidad, pPropiedad, pNumElem, TxtElemEditados, TxtCaracteristicasElem);
    valor = GetValorElementoGuardado(pEntidad, pPropiedad, TxtValorRdf, TxtElemEditados, pNumElem); 
    
    if (pEntidad.indexOf('&ULT') != -1)
    {
        pEntidad = pEntidad.substring(0, pEntidad.indexOf('&ULT'));
    }
    
    valor = GetValorDecode(valor);
    DarValorControl(pEntidad + ',' + pPropiedad, TxtRegistroIDs, TxtCaracteristicasElem, valor);
    var idControlCampo = ObtenerControlEntidadProp(pEntidad + ',' + pPropiedad, TxtRegistroIDs);
    document.getElementById(idControlCampo.replace('selEnt_','lbCrear_')).style.display = 'none';
    document.getElementById(idControlCampo.replace('selEnt_','lbGuardar_')).style.display = '';
    
    if (typeof (document.getElementById(idControlCampo).options) != 'undefined') {
        document.getElementById(idControlCampo).options[document.getElementById(idControlCampo).selectedIndex].disabled = false;
    }
}

function EliminarObjectNoFuncionalSeleccEnt(pNumElem, pEntidad, pPropiedad, pControlContValores)
{
    TxtHackHayCambios = true;
    MarcarElementoEditado(pEntidad, pPropiedad, pNumElem, TxtElemEditados, TxtCaracteristicasElem);

    var valorAntiguo = GetValorElementoGuardado(pEntidad + '&ULT', pPropiedad, TxtValorRdf, TxtElemEditados, pNumElem);
    
    if (valorAntiguo != null && valorAntiguo != '') {
        DeleteElementoEditadoGuardado(pEntidad + '&ULT', pPropiedad, TxtValorRdf, TxtElemEditados);
    }
    else {
        valorAntiguo = GetValorElementoGuardado(pEntidad, pPropiedad, TxtValorRdf, TxtElemEditados, pNumElem);
        DeleteElementoEditadoGuardado(pEntidad, pPropiedad, TxtValorRdf, TxtElemEditados);
    }
    DarValorControl(pEntidad + ',' + pPropiedad, TxtRegistroIDs, TxtCaracteristicasElem, '');
    GuardarValorEnContenedorGrupoValores(pControlContValores, '', pEntidad, pPropiedad, TxtValorRdf, TxtRegistroIDs, TxtCaracteristicasElem, TxtElemEditados);
    MarcarElementoEditado(pEntidad, pPropiedad, -1, TxtElemEditados, TxtCaracteristicasElem);

    var idControlCampo = ObtenerControlEntidadProp(pEntidad + ',' + pPropiedad, TxtRegistroIDs);

    if (idControlCampo.indexOf('selEnt_') != -1 && document.getElementById(idControlCampo.replace('selEnt_', 'bkHtmlSelEnt_')) != null) {
        $('#' + idControlCampo.replace('selEnt_', 'divContControlTes_')).css('display', '');
        $('#' + idControlCampo.replace('selEnt_', 'contEntSelec_')).css('display', 'none');
    }
    else if (document.getElementById(idControlCampo.replace('selEnt_', 'hack_')) == null) {
        document.getElementById(idControlCampo.replace('selEnt_', 'lbCrear_')).style.display = '';
        document.getElementById(idControlCampo.replace('selEnt_', 'lbGuardar_')).style.display = 'none';
    }

    if (typeof document.getElementById(idControlCampo).options != 'undefined') {
        for (var i = 0; i < document.getElementById(idControlCampo).options.length; i++) {
            if (document.getElementById(idControlCampo).options[i].value == valorAntiguo) {
                document.getElementById(idControlCampo).options[i].disabled = false;
                break;
            }
        }
    }

    if (GetCaracteristicaPropiedad(pEntidad, pPropiedad, TxtCaracteristicasElem, 'propSelectEntDep') == 'true') {
        ObtenerSelectoresDependientes(pPropiedad, pEntidad, true);
    }

    // Cerrar el modal ya que el mensaje de confirmación de eliminación aparecerá en un modal
    $(`#modal-container`).modal('hide');
}

function EstablecerBotonesObjectNoFuncionalSeleccEnt(pEntidad, pPropiedad){
    var idControlCampo = ObtenerControlEntidadProp(pEntidad + ',' + pPropiedad, TxtRegistroIDs);
    document.getElementById(idControlCampo.replace('selEnt_','lbCrear_')).style.display = '';
    document.getElementById(idControlCampo.replace('selEnt_','lbGuardar_')).style.display = 'none';
}


/**
 * Esta función agrega un objeto no funcional a una propiedad de una entidad, actualizando los valores correspondientes en los campos de texto y elementos editados.
 * @param {any} pEntidad El nombre de la entidad a la cual se agregará el objeto no funcional.
 * @param {any} pPropiedad El nombre de la propiedad donde se agregará el objeto no funcional.
 * @param {any} pEntidadHija El nombre de la entidad no funcional que se agregará.
 * @param {any} pControlContValores El identificador del contenedor donde se mostrarán los valores.
 * @param {any} pTxtValores El identificador del campo de texto que contiene los valores de la entidad.
 * @param {any} pTxtIDs El identificador del campo de texto que contiene los IDs de los elementos.
 * @param {any} pTxtCaract El identificador del campo de texto que contiene las características de las propiedades.
 * @param {any} pTxtElemEditados El identificador del campo de texto que contiene los elementos editados.
 * @param {any} pVisibleContPaneles Indica si el contenedor de paneles es visible o no.
 * @param {any} pRaiz Indica si la entidad es la raíz del árbol o no.
 * @param {any} pAntiguoNumElem El número de elementos antiguos.
 * @returns
 */
function AgregarObjectNoFuncionalProp(pEntidad, pPropiedad, pEntidadHija, pControlContValores, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados, pVisibleContPaneles, pRaiz, pAntiguoNumElem) {
    TxtHackHayCambios = true;
    var rdfBK = document.getElementById(pTxtValores).value;
    var pEntidadHija = ObtenerTipoEntidadEditada(pEntidadHija);
   
    var perteneceEntPanelGrupoSinEditar = PerteneceEntidadAAlgunGrupoPanelSinEditar(pEntidad, pTxtCaract, pTxtElemEditados);
    var tipoProp = GetTipoPropiedad(pEntidad, pPropiedad, pTxtCaract);
    if (!perteneceEntPanelGrupoSinEditar /*||  tipoProp == 'FO' || tipoProp == 'CO'*/)
    {
        var elemAgregado = AgregarNuevaEntidadAProp(pEntidad, pPropiedad, pEntidadHija, pTxtValores, pTxtElemEditados);

        if (elemAgregado) {            
            AgregarPropsEntidadObligatoriasEdicion(pEntidadHija, null, '');
        }
    }
    else if (tipoProp != 'FO' && tipoProp != 'CO')
    {
        var elemAgregado = AgregarNuevaEntidadAProp(pEntidad + '&ULT', pPropiedad, pEntidadHija, pTxtValores, pTxtElemEditados);

        if (elemAgregado) {
            AgregarPropsEntidadObligatoriasEdicion(pEntidadHija, null, '&ULT');
        }
    }
    
    var camposCorrectos = AgregarPropiedadesDeNuevaEntidad(pEntidadHija, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados);

    if (tipoProp == 'FO' || tipoProp == 'CO') {
        AgregarNuevaEntidadCandidataAProp(pEntidad, pPropiedad, pEntidadHija);
    }
    
    if (!camposCorrectos)
    {
        document.getElementById(pTxtValores).value = rdfBK;
    }
    else
    {
        if (pRaiz) {
            LimpiarControlesEntidad(pEntidadHija, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados);
        }
        
        if (pControlContValores != '')
        {
            AgregarEntidadAContenedorGrupoPaneles(pControlContValores, pEntidad, pPropiedad, pEntidadHija, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados, pVisibleContPaneles, true);
        }
        
        if (ExcedeCardinalidadMaxima(pEntidad, pPropiedad, pTxtValores, pTxtCaract, pTxtElemEditados))
        {
            var idControlCampo = ObtenerControlEntidadProp(pEntidad + ',' + pPropiedad, pTxtIDs);
            
            if (idControlCampo != '')
            {
                document.getElementById(idControlCampo.replace('panel_contenedor_Entidades_','lbCrear_')).style.display = 'none';
            }
        }

        GuardarDocRecExtSelecEntEditable(pEntidad, pPropiedad, pAntiguoNumElem);

        //Borramos la entidad que está como candidata:
        DeleteElementoGuardado(null, pEntidadHija + '&ULT', pTxtValores, pTxtElemEditados, 0);
    }
    
    return camposCorrectos;
}


/**
 * Método que se encarga de guardar y manejar las propiedades no funcionales de una entidad en el contexto de una aplicación. 
 * Realiza operaciones como guardar los valores de las propiedades, eliminar elementos existentes, agregar nuevos elementos, y realizar varias operaciones relacionadas con la edición de entidades y propiedades.
 * @param {any} pEntidad: La entidad principal para la cual se están manejando las propiedades.
 * @param {any} pPropiedad: La propiedad específica que se está manipulando.
 * @param {any} pEntidadHija: La entidad secundaria relacionada con la propiedad.
 * @param {any} pControlContValores: El control que contiene los valores de las propiedades.
 * @param {any} pTxtValores: El texto que representa los valores de las propiedades.
 * @param {any} pTxtIDs: El texto que representa los IDs relacionados con las propiedades.
 * @param {any} pTxtCaract: El texto que representa las características de las propiedades.
 * @param {any} pTxtElemEditados: El texto que representa los elementos editados.
 * 
 * @return {boolean} camposCorrectos: Valor booleano que indica si se han guardado correctamente los campos. 
 */

function GuardarObjectNoFuncionalProp(pEntidad, pPropiedad, pEntidadHija, pControlContValores, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados){

    // Backup de los datos rdf
    var rdfBK = document.getElementById(pTxtValores).value;
    var entidadPadre = pEntidadHija;
    var pEntidadHija = ObtenerTipoEntidadEditada(pEntidadHija);
    var contendedorEntBK = null;
    var mostrarContenedorAlAgregar = false;
    // Comprobar el contenedor de Propiedades 
    if (pControlContValores != '')
    {
        // Guardar el HTML del contenedor de Propiedades 
        contendedorEntBK = document.getElementById(pControlContValores).innerHTML;
        if (document.getElementById(pControlContValores).style.display != 'none')
        {
            mostrarContenedorAlAgregar = true;
        }
    }

    // Obtener el la posición del item que se desea editar dentro de la lista de propiedades
    var numElem = GetNumEdicionEntProp(pEntidad, pPropiedad, pTxtElemEditados);

    SalvarPropiedadesNoFuncionalesEntidad(pEntidadHija, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados);
    
    var entidadBorrar = pEntidad;
    
    if (PerteneceEntidadAAlgunGrupoPanelSinEditar(pEntidad, pTxtCaract, pTxtElemEditados))
    {
        entidadBorrar += '&ULT';
    }
    
    DeleteElementoGuardado(entidadBorrar, pPropiedad, pTxtValores, pTxtElemEditados, numElem);
    
    if (pControlContValores != '')
    {        
        EliminarEntidadDeContenedorGrupoPaneles(pControlContValores, pEntidad, pPropiedad, numElem, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados);
    }
    
    MarcarElementoEditado(pEntidad, pPropiedad, -1, pTxtElemEditados, pTxtCaract);

    var camposCorrectos = AgregarObjectNoFuncionalProp(pEntidad, pPropiedad, pEntidadHija, pControlContValores, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados, mostrarContenedorAlAgregar, false, numElem);

    DeleteElementoGuardado(null, pEntidadHija + '&ULT', pTxtValores, pTxtElemEditados, 0);
    
    if (!camposCorrectos)
    {
        //Recuperamos el BK:
        document.getElementById(pTxtValores).value = rdfBK;
        
        if (contendedorEntBK != null)
        {
            document.getElementById(pControlContValores).innerHTML = contendedorEntBK;
            $(document.getElementById(pControlContValores)).show();
        }
        
        MarcarElementoEditado(pEntidad, pPropiedad, numElem, pTxtElemEditados, pTxtCaract);
    }
    else
    {
        //Movemos la entidad que está la última a su sitio original:
        MoverElementoGuardado(entidadBorrar, pPropiedad, pTxtValores, pTxtElemEditados, -1, numElem)
        if (pControlContValores != '')
        {
            MoverEntidadEnContenedorGrupoPaneles(pControlContValores, pEntidad, pPropiedad, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados, -1, numElem);
        }
        
        LimpiarControlesEntidad(pEntidadHija, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados);
        EstablecerBotonesEntidad(pEntidad, pPropiedad, true, pTxtIDs);
        
        if (ExcedeCardinalidadMaxima(pEntidad, pPropiedad, pTxtValores, pTxtCaract, pTxtElemEditados))
        {
            var idControlCampo = ObtenerControlEntidadProp(pEntidad + ',' + pPropiedad, pTxtIDs);
            document.getElementById(idControlCampo.replace('panel_contenedor_Entidades_','lbCrear_')).style.display = 'none';
        }

        if (entidadPadre != pEntidadHija) {
            //Lipio otras hijas:
            var subclases = GetSubClasesEntidad(entidadPadre);

            for (var i = 0; i < subclases.length; i++) {
                if (subclases[i] != '' && subclases[i] != pEntidadHija) {
                    LimpiarControlesEntidad(subclases[i], pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados);
                }
            }

            var combosSubClase = $('select.cmbSubClass_' + ObtenerTextoGeneracionIDs(entidadPadre));

            if (combosSubClase.length > 0) {
                combosSubClase[0].style.display = '';
            }
        }
    }
    
    return camposCorrectos;
}

function EliminarObjectNoFuncionalProp(pNumElem, pEntidad, pPropiedad, pEntidadHija, pControlContValores, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados){
    TxtHackHayCambios = true;
    var entidadBorrar = pEntidad;
    if (PerteneceEntidadAAlgunGrupoPanelSinEditar(pEntidad, pTxtCaract, pTxtElemEditados))
    {
        entidadBorrar += '&ULT';
    }

    DeleteElementoGuardado(entidadBorrar, pPropiedad, pTxtValores, pTxtElemEditados, pNumElem);
    if (pControlContValores != '')
    {
        EliminarEntidadDeContenedorGrupoPaneles(pControlContValores, pEntidad, pPropiedad, pNumElem, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados);
    }
    MarcarElementoEditado(pEntidad, pPropiedad, -1, pTxtElemEditados, pTxtCaract);
    LimpiarControlesEntidad(pEntidadHija, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados);
    
    EstablecerBotonesEntidad(pEntidad, pPropiedad, true, pTxtIDs);

    BorrarDocRecExtSelecEntEditable(pEntidad, pPropiedad, pNumElem);
    EliminarIDEntidadAuxiliar(pNumElem, pEntidad, pPropiedad, pEntidadHija);
}

function EliminarIDEntidadAuxiliar(pNumElem, pEntidad, pPropiedad, pEntidadHija) {
    DeleteElementoGuardado(pEntidad, pPropiedad, 'txtEntidadesOntoIDs', TxtElemEditados, pNumElem);
}

/**
 * Esta función agrega una nueva entidad a una propiedad especificada.
 * @param {any} pEntidad La entidad a la que se agregará la nueva entidad.
 * @param {any} pPropiedad La propiedad a la que pertenece la nueva entidad.
 * @param {any} pEntidadHija La nueva entidad que se agregará.
 * @param {any} pTxtValores El ID del elemento de texto que contiene los valores.
 * @param {any} pTxtElemEditados El ID del elemento de texto que contiene los elementos editados.
 * @returns
 */
function AgregarNuevaEntidadAProp(pEntidad, pPropiedad, pEntidadHija, pTxtValores, pTxtElemEditados){
    return PutElementoGuardado(pEntidad, pPropiedad, '<' + pEntidadHija + '></' + pEntidadHija + '>', pTxtValores, pTxtElemEditados);
}

function AgregarPropiedadesDeNuevaEntidad(pEntidadHija, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados){
    var propiedades = GetPropiedadesEntidad(pEntidadHija, pTxtCaract);
    var camposCorrectos = true;
    
    for (var i=0;i<propiedades.length;i++)
    {
        if (propiedades[i] != '')
        {
            camposCorrectos = (AgregarPropiedadDeEntidad(pEntidadHija, propiedades[i], pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados) && camposCorrectos);
        }
    }
    
    return camposCorrectos;
}

function AgregarPropiedadDeEntidad(pEntidad, pPropiedad, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados){
    var tipoProp = GetTipoPropiedad(pEntidad, pPropiedad, pTxtCaract);
    var camposCorrectos = true;

    if (tipoProp == 'FD' || tipoProp == 'CD' || tipoProp == 'FSEO' || tipoProp == 'CSEO')
    {
        var valorProp = ObtenerValorEntidadProp(pEntidad + ',' + pPropiedad, pTxtIDs, pTxtCaract);
        var campoCorrecto = ComprobarValorCampoCorrecto(pEntidad, pPropiedad, valorProp, pTxtIDs, pTxtCaract);
        camposCorrectos = camposCorrectos && campoCorrecto;

        if (campoCorrecto && EsPropiedadMultiIdioma(pEntidad, pPropiedad)) {
            var idControlCampoPes = ObtenerControlEntidadProp(pEntidad + ',' + pPropiedad, TxtRegistroIDs).replace('Campo_', 'divContPesIdioma_');

            if (document.getElementById(idControlCampoPes) != null) {
                var li = $('li.active', $('#' + idControlCampoPes));
                var idiomaActual = li.attr('rel');
                valorAntiguo = $('#' + idControlCampoPes).attr('langActual');
                valorProp = IncluirTextoIdiomaEnCadena(valorAntiguo, GetValorEncode(valorProp), idiomaActual);

                if (!ComprobarMultiIdiomaCorrectoTexto(pEntidad, pPropiedad, valorProp)) {
                    campoCorrecto = false;
                    camposCorrectos = false;
                }
            }
            else { //Es multiIdioma sin pestaña
                valorProp += '@' + IdiomaDefectoFormSem + '[|lang|]';
                var idiomas = IdiomasConfigFormSem.split(',');
                for (var i=0;i<idiomas.length;i++)
                {
                    if (idiomas[i] != IdiomaDefectoFormSem)
                    {
                        var valorPropIdioma = ObtenerValorEntidadPropDeIdioma(pEntidad + ',' + pPropiedad, pTxtIDs, pTxtCaract, idiomas[i]);

                        if (valorPropIdioma != '') {
                            valorProp += valorPropIdioma + '@' + idiomas[i] + '[|lang|]';
                        }
                    }
                }
            }
        }
        
        if (campoCorrecto && valorProp != null && valorProp != '')
        {
            var valorDefNoSelec = GetValorDefNoSelec(pEntidad, pPropiedad, pTxtCaract);
            if (valorDefNoSelec == null || valorDefNoSelec == '' || valorDefNoSelec != valorProp)
            {
                var valorAnt = GetValorElementoGuardado(pEntidad, pPropiedad, pTxtValores, pTxtElemEditados, 0);

                if (valorAnt != '') {
                    DeleteElementoGuardado(pEntidad, pPropiedad, pTxtValores, pTxtElemEditados, 0);
                }

                valorProp = GetValorEncode(valorProp);
                PutElementoGuardado(pEntidad, pPropiedad, valorProp, pTxtValores, pTxtElemEditados);
            }
        }
        else if (valorProp == null && ObtenerControlEntidadProp(pEntidad + ',' + pPropiedad, pTxtIDs) == '') {//Propiedad no editable de entidad, no se quiere que se pierda el valor
            valorProp = GetValorElementoGuardado(pEntidad + '&ULT', pPropiedad, pTxtValores, pTxtElemEditados, 0);

            if (valorProp != null && valorProp != '') {
                DeleteElementoGuardado(pEntidad + '&ULT', pPropiedad, pTxtValores, pTxtElemEditados, 0);
                PutElementoGuardado(pEntidad, pPropiedad, valorProp, pTxtValores, pTxtElemEditados);
            }
        }
    }
    else if (tipoProp == 'VD')
    {
        var valorProp = ObtenerValorEntidadProp(pEntidad + ',' + pPropiedad, pTxtIDs, pTxtCaract);

        if (valorProp != null) {
            var valoresProp = valorProp.split(',');
            for (var i = 0; i < valoresProp.length; i++) {
                if (valoresProp[i] != '') {
                    PutElementoGuardado(pEntidad, pPropiedad, GetValorEncode(valoresProp[i]), pTxtValores, pTxtElemEditados);
                }
            }
        }
        else if (ObtenerControlEntidadProp(pEntidad + ',' + pPropiedad, pTxtIDs) == '') {//Propiedad no editable de entidad, no se quiere que se pierda el valor
            var valorProp = GetValorElementoGuardado(pEntidad + '&ULT', pPropiedad, pTxtValores, pTxtElemEditados, 0);
            
            while (valorProp != '') {
                DeleteElementoGuardado(pEntidad + '&ULT', pPropiedad, pTxtValores, pTxtElemEditados, 0);
                PutElementoGuardado(pEntidad, pPropiedad, valorProp, pTxtValores, pTxtElemEditados);
                valorProp = GetValorElementoGuardado(pEntidad + '&ULT', pPropiedad, pTxtValores, pTxtElemEditados, 0);
            }
        }
        
        camposCorrectos = (ComprobarCardinalidadPropiedad(pEntidad, pPropiedad, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados) && camposCorrectos);
    }
    else if (tipoProp == 'LD' || tipoProp == 'LO' || tipoProp == 'LSEO')
    {
        var valorProp = GetValorElementoGuardado(pEntidad + '&ULT', pPropiedad, pTxtValores, pTxtElemEditados, 0);
        
        if (valorProp != '') //Valores en entidad auxiliar
        {
            while (valorProp != '')
            {
                DeleteElementoGuardado(pEntidad + '&ULT', pPropiedad, pTxtValores, pTxtElemEditados, 0);
                PutElementoGuardado(pEntidad, pPropiedad, valorProp, pTxtValores, pTxtElemEditados);
                valorProp = GetValorElementoGuardado(pEntidad + '&ULT', pPropiedad, pTxtValores, pTxtElemEditados, 0);
            }
            
			if (EntidadEstaVacia(pEntidad + '&ULT', pTxtValores))
			{
				DeleteElementoGuardado(null, pEntidad + '&ULT', pTxtValores, pTxtElemEditados, 0);
			}
        }
        
        camposCorrectos = (ComprobarCardinalidadPropiedad(pEntidad, pPropiedad, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados) && camposCorrectos);
    }
    else if (tipoProp == 'FO' || tipoProp == 'CO')
    {//TODO: ESTO FUNCIONA???
        var entidadHija = GetRangoPropiedad(pEntidad, pPropiedad, pTxtCaract);
        var contenedor = '';//TODO: Poner contenedor?
        camposCorrectos = (AgregarObjectNoFuncionalProp(pEntidad, pPropiedad, entidadHija, contenedor, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados, false, false, -1) && camposCorrectos);
    }
    
    return camposCorrectos;
}

function AgregarNuevaEntidadCandidataAProp(pEntidad, pPropiedad, pEntidadHija) {
    var valorProp = GetValorElementoGuardado(pPropiedad, pEntidadHija, TxtValorRdf, TxtElemEditados, 0);
    PutElementoGuardado(pEntidad, pPropiedad, '<' + pEntidadHija + '>' + valorProp + '</' + pEntidadHija + '>', TxtValorRdf, TxtElemEditados);
}

function EntidadEstaVacia(pEntidad, pTxtValores)
{
	var valores = document.getElementById(pTxtValores).value;
	return (valores.indexOf('<' + pEntidad + '>' + '</' + pEntidad + '>') != -1);
}

/**
 * La función SalvarPropiedadesNoFuncionalesEntidad se encarga de guardar las propiedades no funcionales de una entidad en un contexto específico de una aplicación. 
 * Realiza un ciclo a través de las propiedades de la entidad dada y guarda sus valores correspondientes en el texto de valores y elementos editados.
 * @param {any} pEntidad La entidad para la cual se desean guardar las propiedades no funcionales.
 * @param {any} pTxtValores El texto que representa los valores de las propiedades.
 * @param {any} pTxtIDs El texto que representa los IDs relacionados con las propiedades (no parece ser utilizado en esta función).
 * @param {any} pTxtCaract El texto que representa las características de las propiedades (no parece ser utilizado en esta función).
 * @param {any} pTxtElemEditados El texto que representa los elementos editados, donde se guardarán las nuevas propiedades.
 */
function SalvarPropiedadesNoFuncionalesEntidad(pEntidad, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados){
    DeleteElementoGuardado(null, pEntidad + '&ULT', pTxtValores, pTxtElemEditados, 0);
    var propiedades = GetPropiedadesEntidad(pEntidad, pTxtCaract);

    // Recorrer las propiedades de la entidad existentes según las características para coger el valor guardado
    for (var i=0;i<propiedades.length;i++)
    {
        if (propiedades[i] != '')
        {
            //var tipoProp = GetTipoPropiedad(pEntidad, propiedades[i], pTxtCaract);
            //if (tipoProp == 'LD' || tipoProp == 'LO' || tipoProp == 'LSEO')//&ULT
            //{
				//if (i==0)
				//{
					//DeleteElementoGuardado(null, pEntidad + '&ULT', pTxtValores, pTxtElemEditados, 0);
				//}

                // Obtener el valor de cada propiedad existente en el input "mTxtValorRdf"
                var valorProp = GetValorElementoGuardado(pEntidad, propiedades[i], pTxtValores, pTxtElemEditados, 0);
                if (valorProp != '' && valorProp != null)
                {
                    var j = 1;
                    while (valorProp != '')
                    {  
                        // Insertar el valor de la propiedad (valorProp) que tiene por nombre "propiedades[i]"
                        // extraída del input "mTxtValorRdf" y se añade con identificación ULT de última insercción
                        PutElementoGuardado(pEntidad + '&ULT', propiedades[i], valorProp, pTxtValores, pTxtElemEditados);
                        valorProp = GetValorElementoGuardado(pEntidad, propiedades[i], pTxtValores, pTxtElemEditados, j);
                        j++;
                    }
                }
            //}
        }
    }
}

function ComprobarValorCampoCorrecto(pEntidad, pPropiedad, pValor, pTxtIDs, pTxtCaract) {
    return ComprobarValorCampoCorrectoInt(pEntidad, pPropiedad, pValor, pTxtIDs, pTxtCaract, true)        
}

function ComprobarValorCampoCorrectoInt(pEntidad, pPropiedad, pValor, pTxtIDs, pTxtCaract, pCompValVacio){
    if (pValor != null) {
        pValor = pValor.trim();
    }

    if (pValor == '') {
        var caract = document.getElementById(pTxtCaract).value;

        if (pPropiedad.indexOf('_Rep_') != -1 || caract.indexOf(pEntidad + ',' + pPropiedad + '_Rep_') != -1) {
            var propRepetida = pPropiedad;

            if (propRepetida.indexOf('_Rep_') != -1) {
                propRepetida = propRepetida.substring(0, propRepetida.indexOf('_Rep_'));
            }

            var count = 0;
            pValor = ObtenerValorEntidadProp(pEntidad + ',' + propRepetida, pTxtIDs, pTxtCaract);

            while (pValor == '' && caract.indexOf(pEntidad + ',' + propRepetida + '_Rep_' + count) != -1)
            {
                pValor = ObtenerValorEntidadProp(pEntidad + ',' + propRepetida + '_Rep_' + count, pTxtIDs, pTxtCaract);
                count++;
            }
        }
    }

    ComprobarCambiosValor(pEntidad, pPropiedad, pValor);

    var tipoProp = GetTipoPropiedad(pEntidad, pPropiedad, pTxtCaract);
    var propContEntTieneTodasPropSinValor = ComprobarSiPropiedadPerteneceEntSinValorEnPropsDePropCard1(pEntidad, pPropiedad);

    if (!propContEntTieneTodasPropSinValor && pCompValVacio)
    {
        if ((tipoProp == 'FD' || tipoProp == 'FSEO') && pValor == '')
        {
            MostrarErrorPropiedad(pEntidad, pPropiedad, textoFormSem.propiedadObliValor, pTxtIDs);
            return false;
        }
        else if ((tipoProp == 'CD' || tipoProp == 'VD') && pValor == '' && CardinalidadElementoMayorOIgualUno(pEntidad, pPropiedad, pTxtCaract))
        {
            MostrarErrorPropiedad(pEntidad, pPropiedad, textoFormSem.propiedadObliValor, pTxtIDs);
            return false;
        }
    
        var valorDefNoSelec = GetValorDefNoSelec(pEntidad, pPropiedad, pTxtCaract);
    
        if (valorDefNoSelec != null && valorDefNoSelec != '' && valorDefNoSelec == pValor)
        {
            if (tipoProp != 'CD' || (tipoProp == 'CD' && CardinalidadElementoMayorOIgualUno(pEntidad, pPropiedad, pTxtCaract)))
            {
                MostrarErrorPropiedad(pEntidad, pPropiedad, textoFormSem.propiedadObliValor, pTxtIDs);
                return false;
            }
        }
    }

    if (pValor != '' && pValor != null)
    {
        var tipoCampo = GetTipoCampo(pEntidad, pPropiedad, pTxtCaract);
        if (tipoCampo == 'Date' || tipoCampo == 'DateTime' || tipoCampo == 'Time')
        {
            if (GetCaracteristicaPropiedad(pEntidad, pPropiedad, TxtCaracteristicasElem, 'propFechaConHora') != null) {
                if (!FechaConHoraCorrecta(pValor)) {
                    MostrarErrorPropiedad(pEntidad, pPropiedad, textoFormSem.errorFechaConHora, pTxtIDs);
                    return false;
                }
            }
            else if (!FechaCorrecta(pValor))
            {
                MostrarErrorPropiedad(pEntidad, pPropiedad, textoFormSem.errorFecha, pTxtIDs);
                return false;
            }
        }
        else if (tipoCampo == 'Entero')
        {
            var valorNum = Number(pValor);
            if (isNaN(valorNum) || !Number.isInteger(valorNum))
            {
                MostrarErrorPropiedad(pEntidad, pPropiedad, textoFormSem.errorNumEntero, pTxtIDs);
                return false;
            }
        }
        else if (tipoCampo == 'Numerico')
        {
            if (!EsDecimalValido(pValor))
            {
                MostrarErrorPropiedad(pEntidad, pPropiedad, textoFormSem.errorNum, pTxtIDs);
                return false;
            }
        }
        
        var numCaract = GetRestriccionNumCaract(pEntidad, pPropiedad, pTxtCaract);
        
        if (numCaract != null)
        {
            var tipoResCarac = numCaract.split(',')[0];
            var key = parseInt(numCaract.split(',')[1]);
            var value = parseInt(numCaract.split(',')[2]);
            
            if (tipoResCarac == '<' && pValor.length >= key)
            {
                MostrarErrorPropiedad(pEntidad, pPropiedad, textoFormSem.valorExcedeNumCarac.replace('@1@', key), pTxtIDs);
                return false;
            }
            else if(tipoResCarac == '>' && pValor.length <= key)
            {
                MostrarErrorPropiedad(pEntidad, pPropiedad, textoFormSem.valorMenosNumCarac.replace('@1@', key), pTxtIDs);
                return false;
            }
            else if(tipoResCarac == '=' && pValor.length != key)
            {
                MostrarErrorPropiedad(pEntidad, pPropiedad, textoFormSem.valorNoNumCarac.replace('@1@', key), pTxtIDs);
                return false;
            }
            else if(tipoResCarac == '-' && (pValor.length > value || pValor.length < key))
            {
                MostrarErrorPropiedad(pEntidad, pPropiedad, textoFormSem.valorNoEntreNumCarac.replace('@1@', key).replace('@2@', value), pTxtIDs);
                return false;
            }
        }
    }
    
    //Limpio el control:
    LimpiarHtmlControl(GetIDControlError(pEntidad, pPropiedad, pTxtIDs));
    //LimpiarHtmlControl(GetIDControlError(pEntidad, pPropiedad, pTxtIDs.replace('mGeneradorOWL_','mGeneradorOWLVP_')));
    
    if (pEntidad == 'UserArea_CandidatePerson' && pPropiedad == 'Tags')
    {
        LimpiarHtmlControl('divErrorTags');
    }

    if (typeof (personalizarFormKO) != 'undefined') {
        personalizarFormKO.init();
    }
    
    return true;
}

function ComprobarCambiosValor(pEntidad, pPropiedad, pValor) {
    var valorAntiguo = GetValorElementoEditadoGuardado(pEntidad, pPropiedad, TxtValorRdf, TxtElemEditados);

    if (valorAntiguo != null) {
        valorAntiguo = GetValorDecode(valorAntiguo);

        if (valorAntiguo != pValor) {
            TxtHackHayCambios = true;
        }
    }
}

function ComprobarSiPropiedadPerteneceEntSinValorEnPropsDePropCard1(pEntidad, pPropiedad) {
    var domSupCardi1 = GetCaracteristicaPropiedad(pEntidad, pPropiedad, TxtCaracteristicasElem, 'domSupCardi1');
    if (domSupCardi1 != null && domSupCardi1 == 'true') {
        return !EntidadTieneAlgunaPropiedadConValor(pEntidad);
    }

    return false;
}

function EntidadTieneAlgunaPropiedadConValor(pEntidad){
    var caract = document.getElementById(TxtCaracteristicasElem).value;
    caract = caract.substring(caract.indexOf('|') + 1);//Eliminio 1 parametro
    var caracteristicas = caract.split('|');

    for (var i=0;i<caracteristicas.length;i++)
    {
        var entProp = ObtenerNombreEntPropRestricciones(caracteristicas[i]);
        var entidad = entProp.split(',')[0];

        if (entidad == pEntidad)
        {
            var propiedad = entProp.split(',')[1];
            var tipoProp = GetTipoPropiedad(entidad, propiedad, TxtCaracteristicasElem);

            if (tipoProp == 'FD' || tipoProp == 'CD' || tipoProp == 'VD' || tipoProp == 'FSEO' || tipoProp == 'CSEO')
            {
                var valorProp = ObtenerValorEntidadProp(entProp, TxtRegistroIDs, TxtCaracteristicasElem);
                var valorDefNoSelec = GetValorDefNoSelec(entidad, propiedad, TxtCaracteristicasElem);

                if (valorProp != null && valorProp != '' && (valorDefNoSelec == null || valorDefNoSelec == '' || valorDefNoSelec != valorProp))
                {
                    return true;
                }
            }
            else
            {
                var numElemProp = GetNumElementosPropiedad(entidad, propiedad, TxtValorRdf, TxtElemEditados);

                if (numElemProp > 0)
                {
                    return true;
                }
            }
        }
    }

    return false;
}

function FechaCorrecta(pFecha) {
    try
    {
        pFecha = pFecha.trim();

        if (pFecha == '' || pFecha.length != 10 || pFecha.indexOf('/') != 2 || pFecha.lastIndexOf('/') != 5) {
            return false;
        }

        var diaTexto = pFecha.substring(0, 2);
        var mesTexto = pFecha.substring(3, 5);
        var anyoTexto = pFecha.substring(6);

        if (diaTexto.indexOf('0') == 0) {
            diaTexto = diaTexto.substring(1);
        }

        if (mesTexto.indexOf('0') == 0) {
            mesTexto = mesTexto.substring(1);
        }

        while (anyoTexto.length > 0 && anyoTexto.indexOf('0') == 0) {
            anyoTexto = anyoTexto.substring(1);
        }

        if (anyoTexto.length == 0) {
            anyoTexto = '0';
        }

        var dia = parseInt(diaTexto);
        var mes = parseInt(mesTexto);
        var anyo = parseInt(anyoTexto);

        if (dia.toString() == 'NaN' || mes.toString() == 'NaN' || anyo.toString() == 'NaN') {
            return false;
        }

        if (mes < 1 || mes > 12) {
            return false;
        }

        if (dia < 1 || dia > 31) {
            return false;
        }

        if (mes == 2 && dia > 29) {
            return false;
        }
        else if (dia > 30 && (mes == 4 || mes == 6 || mes == 9 || mes == 11)) {
            return false;
        }

        return true;
    }
    catch (ex) {
        return false;
    }
}

function FechaConHoraCorrecta(pFecha) {
    if (pFecha.indexOf(' ') == -1) {
        return false;
    }

    var fecha = pFecha.substring(0, pFecha.indexOf(' '));
    var hora = pFecha.substring(pFecha.indexOf(' ') + 1);

    if (hora.length != 8 || hora.indexOf(':') != 2 || hora.lastIndexOf(':') != 5) {
        return false;
    }

    var hh = parseInt(hora.split(':')[0]);
    var mm = parseInt(hora.split(':')[1]);
    var ss = parseInt(hora.split(':')[2]);

    if (hh.toString() == 'NaN' || mm.toString() == 'NaN' || ss.toString() == 'NaN') {
        return false;
    }

    if (hh < 0 || hh > 23) {
        return false;
    }

    if (mm < 0 || mm > 59) {
        return false;
    }

    if (ss < 0 || ss > 59) {
        return false;
    }

    return FechaCorrecta(fecha);
}

function EsDecimalValido(valor) {
  // Elimina espacios al principio y al final
  const limpio = valor.trim();

  // Valida que sea un número decimal positivo o negativo con punto como separador
  const regex = /^-?\d+(\.\d+)?$/;

  return regex.test(limpio);
}

function MostrarErrorPropiedad(pEntidad, pPropiedad, pError, pTxtIDs){
    crearErrorFormSem('<p>'+pError+'</p>', GetIDControlError(pEntidad, pPropiedad, pTxtIDs));
    
    if (pEntidad == 'UserArea_CandidatePerson' && pPropiedad == 'Tags')
    {
        crearErrorFormSem('<p>'+textoFormSem.malTags+'</p>', 'divErrorTags');
    }
}

function crearErrorFormSem(textoError, contenedor) {
    $('#contenedor').addClass("errorFormSem");
    if (document.getElementById(contenedor) != null) {
        var $c = $(document.getElementById(contenedor));
        if ($c.find('div.ko').length) { // si ya existe el div.ko ...
            $c.find('div.ko').css('display', 'block');
            var inp = $c.find('div.ko').html(textoError);

            if (typeof (inp.shakeIt) != "undefined") {
                inp.shakeIt();
            }
        } else { //... si no lo creamos
            if ($c[0].tagName == "DIV") {
                $c.html('<div class="ko" >' + textoError + '</div>');
                $c.find('div.ko').css('display', 'block');
                var inp = $c.find('div.ko').html(textoError);

                if (typeof (inp.shakeIt) != "undefined") {
                    inp.shakeIt();
                }
            }
        }
    }

    if (typeof (personalizarFormKO) != 'undefined') {
        personalizarFormKO.init();
    }
}

function GetIDControlError(pEntidad, pPropiedad, pTxtIDs) {
    var propError = pPropiedad;
    var propRepetida = pPropiedad;

    if (propRepetida.indexOf('_Rep_') != -1) {
        propRepetida = propRepetida.substring(0, propRepetida.indexOf('_Rep_'));
    }

    var count = 0;
    var control = ObtenerControlEntidadProp(pEntidad + ',' + propRepetida + '_Rep_' + count, pTxtIDs);

    while (document.getElementById(control) != null) {
        propError = propRepetida + '_Rep_' + count;
        count++;
        control = ObtenerControlEntidadProp(pEntidad + ',' + propRepetida + '_Rep_' + count, pTxtIDs);
    }

    return ObtenerControlEntidadProp(pEntidad + ',' + propError, pTxtIDs).replace('Campo_', 'divError_').replace('panel_contenedor_Entidades_', 'divError_').replace('selEnt_', 'divError_');
}

function ComprobarCardinalidadPropiedad(pEntidad, pPropiedad, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados) {
    if (EntidadEsSubClase(pEntidad) && !EntidadSubClaseSeleccionada(pEntidad, true)) {
        return true;
    }

    var cardinalidad = GetCardinalidadPropiedad(pEntidad, pPropiedad, pTxtCaract);
    
    if (cardinalidad != null)
    {
        var tipoCardi = cardinalidad.split(',')[0];
        var numCardi = parseInt(cardinalidad.split(',')[1]);
        var numElemProp = GetNumElementosPropiedad(pEntidad, pPropiedad, pTxtValores, pTxtElemEditados);
        
        if (tipoCardi == 'Cardinality' && numCardi != numElemProp)
        {
            if (numCardi == 1)
            {
                MostrarErrorPropiedad(pEntidad, pPropiedad, textoFormSem.cardi1, pTxtIDs);
            }
            else
            {
                MostrarErrorPropiedad(pEntidad, pPropiedad, textoFormSem.cardiVarios.replace('@1@', numCardi), pTxtIDs);
            }
            return false;
        }
        else if (tipoCardi == 'MaxCardinality' && numCardi < numElemProp)
        {
            if (numCardi == 1)
            {
                MostrarErrorPropiedad(pEntidad, pPropiedad, textoFormSem.maxCardi1, pTxtIDs);
            }
            else
            {
                MostrarErrorPropiedad(pEntidad, pPropiedad, textoFormSem.maxCardiVarios.replace('@1@', numCardi), pTxtIDs);
            }
            return false;
        }
        else if (tipoCardi == 'MinCardinality' && numCardi > numElemProp)
        {
            if (numCardi == 1)
            {
                MostrarErrorPropiedad(pEntidad, pPropiedad, textoFormSem.minCardi1, pTxtIDs);
            }
            else
            {
                MostrarErrorPropiedad(pEntidad, pPropiedad, textoFormSem.minCardiVarios.replace('@1@', numCardi), pTxtIDs);
            }
            return false;
        }
    }
    
    //Limpio el control:
    LimpiarHtmlControl(GetIDControlError(pEntidad, pPropiedad, pTxtIDs));
    
    if (pEntidad == 'UserArea_CandidatePerson' && pPropiedad == 'Tags')
    {
        LimpiarHtmlControl('divErrorTags');
    }
    
    return true;
}

function CardinalidadElementoMayorOIgualUno(pEntidad, pPropiedad, pTxtCaract){
    var cardinalidad = GetCardinalidadPropiedad(pEntidad, pPropiedad, pTxtCaract);
    if (cardinalidad != null)
    {
        try
        {
            var tipoCardi = cardinalidad.split(',')[0];
            var numCardi = parseInt(cardinalidad.split(',')[1]);
            
            if (tipoCardi == 'Cardinality' && numCardi >= 1)
            {
                return true;
            }
            else if (tipoCardi == 'MinCardinality' && numCardi >= 1)
            {
                return true;
            }
        }
        catch(ex){}
    }
    
    return false;
}

function ExcedeCardinalidadMaxima(pEntidad, pPropiedad, pTxtValores, pTxtCaract, pTxtElemEditados){
    var cardinalidad = GetCardinalidadPropiedad(pEntidad, pPropiedad, pTxtCaract);
    
    if (cardinalidad != null)
    {
        var tipoCardi = cardinalidad.split(',')[0];
        var numCardi = parseInt(cardinalidad.split(',')[1]);
        var numElemProp = GetNumElementosPropiedad(pEntidad, pPropiedad, pTxtValores, pTxtElemEditados);
        
        if ((tipoCardi == 'Cardinality' || tipoCardi == 'MaxCardinality') && numElemProp >= numCardi)
        {
            return true;
        }
    }
    
    return false;
}

function EstablecerBotonesEntidad(pEntidad, pPropiedad, pAgregar, pTxtIDs){
    var idControlCampo = ObtenerControlEntidadProp(pEntidad + ',' + pPropiedad, pTxtIDs);

    if (document.getElementById(idControlCampo.replace('panel_contenedor_Entidades_', 'lbCrear_')) == null) {
        return false;
    }
    
    if (pAgregar)
    {
        document.getElementById(idControlCampo.replace('panel_contenedor_Entidades_','lbCrear_')).style.display = '';
        document.getElementById(idControlCampo.replace('panel_contenedor_Entidades_','lbGuardar_')).style.display = 'none';
        document.getElementById(idControlCampo.replace('panel_contenedor_Entidades_','lbCancelar_')).style.display = 'none';
    }
    else
    {
        document.getElementById(idControlCampo.replace('panel_contenedor_Entidades_','lbCrear_')).style.display = 'none';
        document.getElementById(idControlCampo.replace('panel_contenedor_Entidades_','lbGuardar_')).style.display = '';
        document.getElementById(idControlCampo.replace('panel_contenedor_Entidades_','lbCancelar_')).style.display = '';
    }
    
    
}

function EstablecerBotonesGrupoValores(pEntidad, pPropiedad, pTxtIDs){
    var idControlCampo = ObtenerControlEntidadProp(pEntidad + ',' + pPropiedad, pTxtIDs);

    if (document.getElementById(idControlCampo.replace('Campo_', 'lbCrear_').replace('selEnt_', 'lbCrear_')).style.display != 'none' || document.getElementById(idControlCampo.replace('Campo_', 'lbGuardar_').replace('selEnt_', 'lbGuardar_')).style.display != 'none') {
        document.getElementById(idControlCampo.replace('Campo_', 'lbCrear_').replace('selEnt_', 'lbCrear_')).style.display = '';
        document.getElementById(idControlCampo.replace('Campo_', 'lbGuardar_').replace('selEnt_', 'lbGuardar_')).style.display = 'none';
    }
}

function LimpiarControlesEntidad(pEntidadHija, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados) {
    var entidadPadre = pEntidadHija;
    var subClases = GetSubClasesEntidad(pEntidadHija);

    if (subClases != null && subClases.length > 0) {
        for (var i = 0; i < subClases.length; i++) {
            if (EntidadSubClaseSeleccionada(subClases[i], false)) {
                pEntidadHija = subClases[i];
                break;
            }
        }
    }

    var propiedades = GetPropiedadesEntidad(pEntidadHija, pTxtCaract);

    for (var i = 0; i < propiedades.length; i++) {
        if (propiedades[i] != '') {
            LimpiarControlesPropiedadDeEntidad(pEntidadHija, propiedades[i], pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados);
        }
    }

    if (entidadPadre != pEntidadHija) {
        //Lipio otras hijas:
        for (var i = 0; i < subClases.length; i++) {
            if (subClases[i] != '' && subClases[i] != pEntidadHija) {
                LimpiarControlesEntidad(subClases[i], pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados);
            }
        }

        var combosSubClase = $('select.cmbSubClass_' + ObtenerTextoGeneracionIDs(entidadPadre));

        if (combosSubClase.length > 0) {
            combosSubClase[0].style.display = '';
        }
    }
}

function LimpiarControlesPropiedadDeEntidad(pEntidad, pPropiedad, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados){
	//TODO:Revisar
    var tipoProp = GetTipoPropiedad(pEntidad, pPropiedad, pTxtCaract);
    
    if (tipoProp == 'FD' || tipoProp == 'CD' || tipoProp == 'VD')
    {
        DarValorControl(pEntidad + ',' + pPropiedad, pTxtIDs, pTxtCaract, '');
        LimpiarHtmlControl(GetIDControlError(pEntidad, pPropiedad, pTxtIDs));

        if (EsPropiedadMultiIdiomaSinPesanya(pEntidad, pPropiedad)) {
            var idiomas = IdiomasConfigFormSem.split(',');
            for (var i=0;i<idiomas.length;i++)
            {
                if (idiomas[i] != IdiomaDefectoFormSem)
                {
                    DarValorControlConIdioma(pEntidad + ',' + pPropiedad, pTxtIDs, pTxtCaract, '', idiomas[i]);
                }
            }
        }
        else if (EsPropiedadMultiIdioma(pEntidad, pPropiedad)) {//Con pestaña por eliminación
            var controlPropMulti = ObtenerControlEntidadProp(pEntidad + ',' + pPropiedad, TxtRegistroIDs);

            if (controlPropMulti != null) {
                var idControlCampoPes = controlPropMulti.replace('Campo_', 'divContPesIdioma_');

                if (idControlCampoPes != null) {
                    $('#' + idControlCampoPes).attr('langActual', '');
                }
            }
        }
    }
    else if (tipoProp == 'LD' || tipoProp == 'LSEO')
    {
        DarValorControl(pEntidad + ',' + pPropiedad, pTxtIDs, pTxtCaract, '');
        var idControlCampo = ObtenerControlEntidadProp(pEntidad + ',' + pPropiedad, pTxtIDs);

        if (idControlCampo != '' && document.getElementById(idControlCampo.replace('Campo_', 'contedor_Valores_').replace('selEnt_', 'contEntSelec_')) != null) {
            document.getElementById(idControlCampo.replace('Campo_', 'contedor_Valores_').replace('selEnt_', 'contEntSelec_')).innerHTML = '';
            EstablecerBotonesGrupoValores(pEntidad, pPropiedad, pTxtIDs);
        }

        if (idControlCampo.length != 0 && $('#' + idControlCampo).length > 0 && $('#' + idControlCampo)[0].nodeName == 'SELECT') {
            $('option', $('#' + idControlCampo)[0]).removeAttr('disabled');
        }
    }
    else if (tipoProp == 'FO' || tipoProp == 'CO')
    {
        var entidadHija = GetRangoPropiedad(pEntidad, pPropiedad, pTxtCaract);
        LimpiarControlesEntidad(entidadHija, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados);
    }
    else if (tipoProp == 'LO')
    {
        LimpiarControlesEntidad(GetRangoPropiedad(pEntidad, pPropiedad, pTxtCaract), pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados);
        LimpiarContenedorGrupoPaneles(pEntidad, pPropiedad, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados);
        EstablecerBotonesEntidad(pEntidad, pPropiedad, true, pTxtIDs);
    }
    else if (tipoProp == 'FSEO' || tipoProp == 'CSEO')
    {
        DarValorControl(pEntidad + ',' + pPropiedad, pTxtIDs, pTxtCaract, '');
        LimpiarHtmlControl(GetIDControlError(pEntidad, pPropiedad, pTxtIDs));

        var idControl = ObtenerControlEntidadProp(pEntidad + ',' + pPropiedad, pTxtIDs);
        if (idControl.indexOf('selEnt_') != -1) 
        {
            var inputHack = $('#' + idControl.replace('selEnt_', 'hack_'));
            if (inputHack.prop("disabled") && inputHack.hasClass("autocompletarSelecEnt")) {
                inputHack.prop("disabled", false);
                var aspa = $('a.removeAutocompletar', $('#' + idControl).closest('div.cont'));
                if (aspa.length > 0) {
                    aspa.remove();
                }
            }
            if (document.getElementById(idControl.replace('selEnt_', 'bkHtmlSelEnt_')) != null) {
                ResetearSeleccTesSem(idControl);
                $('#' + idControl.replace('selEnt_', 'divContControlTes_')).css('display', '');
                $('#' + idControl.replace('selEnt_', 'contEntSelec_')).css('display', 'none');
            }
        }
    }
}

function AgregarEntidadAContenedorGrupoPaneles(pControlCont, pEntidad, pPropiedad, pEntidadHija, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados, pVisibleContPaneles, pAgregandoNuevoElem){
    if (pVisibleContPaneles)
    {
        document.getElementById(pControlCont).style.display = '';
    }

    if (document.getElementById(pControlCont) == null) {
        return false;
    }

    var numElem = (document.getElementById(pControlCont).children[0].children[0].children.length - 1);
    
    var contenedorHTML=document.getElementById(pControlCont).innerHTML;
    contenedorHTML = contenedorHTML.replace(document.getElementById(pControlCont).children[0].children[0].innerHTML, document.getElementById(pControlCont).children[0].children[0].innerHTML + ObtenerFilaValorContenedorGrupoPaneles(pControlCont, pEntidadHija, numElem, true, pEntidad, pPropiedad, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados, pAgregandoNuevoElem));
    document.getElementById(pControlCont).innerHTML=contenedorHTML;
    
}

function MoverEntidadEnContenedorGrupoPaneles(pControlCont, pEntidad, pPropiedad, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados, pNumOrigen, pNumDestino){
    if (pNumOrigen == -1)
    {
        pNumOrigen = document.getElementById(pControlCont).children[0].children[0].children.length - 2;
    }
    
    var valorOrgigen = document.getElementById(pControlCont).children[0].children[0].children[pNumOrigen + 1].innerHTML;
    valorOrgigen = valorOrgigen.replace(', \''+(pNumOrigen),', \'' + (pNumDestino)).replace('(\''+(pNumOrigen),'(\''+(pNumDestino))
    
    var hijos = document.getElementById(pControlCont).children[0].children[0].children;
    var htmlFinal = '<tr>' + document.getElementById(pControlCont).children[0].children[0].children[0].innerHTML + '</tr>';
    
    var numOrigen = pNumOrigen + 1;
    var numDestino = pNumDestino + 1;
    var clase = '';
    
    for (var i=1;i<hijos.length;i++)
    {
        if (i == numDestino)
        {
            clase = 'par';
            
            if(i %2 ==0)
            {
                clase = 'impar';
            }
            
            htmlFinal += '<tr class="'+clase+'">' + valorOrgigen + '</tr>';
        }
        
        if (i != numOrigen)
        {
            clase = hijos[i].className;
            
            if (i>=numDestino) //Invertimos clase
            {
                if (clase == 'par')
                {
                    clase = 'impar';
                }
                else
                {
                    clase = 'par';
                }
                
                htmlFinal += '<tr class="'+clase+'">' + hijos[i].innerHTML.replace(', \''+(i - 1),', \'' + (i)).replace('(\''+(i - 1),'(\''+(i)) + '</tr>';
            }
            else
            {
                htmlFinal += '<tr class="'+clase+'">' + hijos[i].innerHTML + '</tr>';
            }
        }
    }
    
    var contenedorHTML=document.getElementById(pControlCont).innerHTML;
    contenedorHTML=contenedorHTML.replace(document.getElementById(pControlCont).children[0].children[0].innerHTML,htmlFinal);       
    document.getElementById(pControlCont).innerHTML=contenedorHTML;
}
/**
 * Esta función elimina una entidad de un contenedor de grupo de paneles en la interfaz de usuario. 
 * Recorre los hijos del contenedor y reconstruye el contenido HTML del contenedor, excluyendo la entidad específica que se desea eliminar. 
 * Se utiliza para mantener la coherencia visual en la interfaz de usuario después de eliminar una entidad de un grupo de paneles.
 * @param {any} pControlCont El identificador del contenedor de grupo de paneles.
 * @param {any} pEntidad El nombre de la entidad que se va a eliminar del contenedor.
 * @param {any} pPropiedad La propiedad asociada a la entidad que se va a eliminar.
 * @param {any} pNumElem El número de elementos que se van a eliminar del contenedor.
 * @param {any} pTxtValores El identificador del campo de texto que almacena los valores relacionados.
 * @param {any} pTxtIDs El identificador del campo de texto que almacena los IDs relacionados.
 * @param {any} pTxtCaract El identificador del campo de texto que almacena las características relacionadas.
 * @param {any} pTxtElemEditados El identificador del campo de texto que almacena los elementos editados.
 * @returns
 */
function EliminarEntidadDeContenedorGrupoPaneles(pControlCont, pEntidad, pPropiedad, pNumElem, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados){
    var hijos = document.getElementById(pControlCont).children[0].children[0].children;
    if (hijos.length == 0)
    {
        return;
    }
    var htmlFinal = '<tr>' + document.getElementById(pControlCont).children[0].children[0].children[0].innerHTML + '</tr>';
    
    var numElem = parseInt(pNumElem) + 1;
    
    for (var i=1;i<hijos.length;i++)
    {
        if (i != numElem)
        {
            var clase = hijos[i].className;
            
            if (i>numElem) //Invertimos clase
            {
                if (clase == 'par')
                {
                    clase = 'impar';
                }
                else
                {
                    clase = 'par';
                }                                
                // Establecer los nuevos índides tanto para la edición como para el borrado
                htmlFinal += '<tr class="' + clase + '">' + hijos[i].innerHTML.replace(new RegExp('\'' + (i - 1), 'g'), '\'' + (i - 2)) + '</tr>';
            }
            else
            {
                htmlFinal += '<tr class="'+clase+'">' + hijos[i].innerHTML + '</tr>';
            }
        }
    }
    
    var contenedorHTML=document.getElementById(pControlCont).innerHTML;
    contenedorHTML=contenedorHTML.replace(document.getElementById(pControlCont).children[0].children[0].innerHTML,htmlFinal);       
    document.getElementById(pControlCont).innerHTML=contenedorHTML;
    
    if (document.getElementById(pControlCont).children[0].children[0].children.length == 1)
    {
        document.getElementById(pControlCont).style.display='none';
    }
}

function ObtenerFilaValorContenedorGrupoPaneles(pControlCont, pEntidadHija, pNumElem, pAgregarTR, pEntidad, pPropiedad, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados, pAgregandoNuevoElem) {
    var fila = '';
    // Valor final que se mostraría en la tabla. Usado para comprobar que no hay duplicados
    let valorFinalRow = "";    
    
    if (pAgregarTR)
    {
        var claseFila = 'impar';
        
        if(pNumElem %2 ==0)
        {
            claseFila = 'par';
        }
        
        fila += '<tr class="'+claseFila+'">';
    }
    var representantes = GetValorAtribRepresentantesEntidad(pEntidadHija, pTxtCaract);
    
    if (representantes != null)
    {
        if (!pAgregandoNuevoElem) {
            MarcarElementoEditado(pEntidad, pPropiedad, pNumElem, pTxtElemEditados, pTxtCaract);
        }
        
        for (var i=0;i<representantes.length;i++)
        {
            if (representantes[i] != '')
            {
                var propiedad = representantes[i].split(';')[0];
                var codigoRepre = representantes[i].split(';')[1];//TODO: Tratar
                
                var valor = '';
                var tipoProp = GetTipoPropiedad(pEntidadHija, propiedad, pTxtCaract);
                if (tipoProp == 'VD') {
                    var valorProp = GetValorElementoGuardado(pEntidadHija, propiedad, pTxtValores, pTxtElemEditados, 0);
                    var i = 1;
                    while (valorProp != "") {
                        //valorProp = GetValorDecode(valorProp);
                        valor += valorProp + ', ';
                        valorProp = GetValorElementoGuardado(pEntidadHija, propiedad, pTxtValores, pTxtElemEditados, i);
                        i++;
                    }

                    if (valor != '') {
                        valor = valor.substring(0, valor.length - 2);
                    }
                }
                else if (tipoProp == 'CSEO' || tipoProp == 'FSEO' || tipoProp == 'LSEO') {
                    valor = GetValorElementoGuardado(pEntidadHija, propiedad, pTxtValores, pTxtElemEditados, -1);
                    var idControl = ObtenerControlEntidadProp(pEntidadHija + ',' + propiedad, pTxtIDs);

                    if (idControl.indexOf('selEnt_') != -1 && idControl != 0 && $('#' + idControl).length > 0 && $('#' + idControl)[0].tagName == 'SELECT') {
                        var opcion = $('option[value="' + valor + '"]', $('#' + idControl)[0]);

                        if (opcion.length > 0) {
                            valor = opcion.text();
                        }
                    }
                    else if (idControl.indexOf('selEnt_') != -1 && (tipoProp == 'FSEO' || tipoProp == 'LSEO' || document.getElementById(idControl.replace('selEnt_', 'bkHtmlSelEnt_')) != null)) {
                        valor = ReemplarIDsCatTesSemPorNombre(valor);
                    }
                }
                else {
                    valor = GetValorElementoGuardado(pEntidadHija, propiedad, pTxtValores, pTxtElemEditados, -1);

                    if (EsPropiedadMultiIdioma(pEntidadHija, propiedad))
                    {
                        valor = ExtraerTextoIdioma(valor, IdiomaDefectoFormSem);
                    }
                }
                
                fila += '<td class="tdval"><span>'+valor+'</span></td>';
            }
        }
        
        MarcarElementoEditado(pEntidad, pPropiedad, -1, pTxtElemEditados, pTxtCaract);
    }
    else
    {
        let propiedades=GetPropiedadesEntidad(pEntidadHija, pTxtCaract);

        let pValor=GetValorElementoGuardado(pEntidadHija, propiedades[0], pTxtValores, pTxtElemEditados, 0)
				
		if (pValor.length!=0)
		{
			pValor=ReemplarIDsCatTesSemPorNombre(pValor);
            valorFinalRow = pValor;
			fila += '<td><span>' + GetValorDecode(pValor) +'</span></td>';
		}else
		{
		  return "";
		}
    }
    // Cambiado por nuevo Front
    // fila += '<td class="tdaccion"><a onclick="SeleccionarElementoGrupoPaneles(\''+pEntidad+'\', \''+pPropiedad+'\', \''+pEntidadHija+'\', \''+pNumElem+'\', \''+pTxtValores+'\', \''+pTxtIDs+'\', \''+pTxtCaract+'\', \''+pTxtElemEditados+'\')"><img src="'+GetUrlImg(pTxtCaract)+'icoEditar.gif"></a></td>';
    fila += `<td class="tdaccion">
             <a href="javascript: void(0);"style="color: var(--c-primario)" onclick="SeleccionarElementoGrupoPaneles('${pEntidad}','${pPropiedad}', '${pEntidadHija}', '${pNumElem}', '${pTxtValores}', '${pTxtIDs}', '${pTxtCaract}','${pTxtElemEditados}')">
                <span class="material-icons pr-0">edit</span>
             </a>
             </td>`;
    // var metodoEliminar = 'EliminarObjectNoFuncionalProp(\\\''+pNumElem+'\\\',\\\''+pEntidad+'\\\', \\\''+pPropiedad+'\\\', \\\''+pEntidadHija+'\\\', \\\''+pControlCont+'\\\', \\\''+pTxtValores+'\\\', \\\''+pTxtIDs+'\\\', \\\''+pTxtCaract+'\\\', \\\''+pTxtElemEditados+'\\\');';
    var metodoEliminar = `EliminarObjectNoFuncionalProp('${pNumElem}','${pEntidad}', '${pPropiedad}', '${pEntidadHija}', '${pControlCont}', '${pTxtValores}', '${pTxtIDs}', '${pTxtCaract}', '${pTxtElemEditados}');`;

    
     var textoEliminar=GetMensajeEliminar(pEntidad,pPropiedad,pTxtCaract);
    
    if(textoEliminar==null )
    {
        // Cambio por nuevo Front
        //fila += '<td class="tdaccion"><a class="remove" onclick="MostrarPanelConfirmacionEvento(event, \''+textoFormSem.confimEliminarEntidad+'\', \''+metodoEliminar+'\')"></a></td>';    
        fila += `<td class="tdaccion">                    
                    <a href="javascript:void(0)"    
                       class="remove"
                        data-showmodalcentered="1"
                        onclick="
                            $('#modal-container').modal('show', this);
                            AccionFichaPerfil(
                                '${textoRecursos.Eliminar}',                                
                                '${borr.si.charAt(0).toUpperCase()}${borr.si.slice(1)}',
                                '${borr.no.charAt(0).toUpperCase()}${borr.no.slice(1)}',
                                '${textoFormSem.confimEliminarEntidad}',
                                'sin-definir',
                                function(){
                                        ${metodoEliminar};
                                        $('#modal-container').modal('hide');
                                }
                            );
                        ">
                            <span class="material-icons pr-0">delete</span>
                    </a>                    

                </td>`;    
    }else
    {
        // Cambio por nuevo Front
        // fila += '<td class="tdaccion"><a class="remove" onclick="MostrarPanelConfirmacionEvento(event, \''+textoEliminar+'\', \''+metodoEliminar+'\')"></a></td>';    
        fila += `<td class="tdaccion">                 
                    <a href="javascript:void(0)"
                       class="remove"
                        data-showmodalcentered="1"
                        onclick="
                            $('#modal-container').modal('show', this);
                            AccionFichaPerfil(
                                '${textoRecursos.Eliminar}',
                                '${borr.si.charAt(0).toUpperCase()}${borr.si.slice(1)}',
                                '${borr.no.charAt(0).toUpperCase()}${borr.no.slice(1)}',
                                '${textoEliminar}',
                                'sin-definir',
                                function(){
                                        ${metodoEliminar};
                                        $('#modal-container').modal('hide')
                                }
                            );
                        ">
                            <span class="material-icons pr-0">delete</span>
                    </a>                    
                </td>`;   
    }
      
    if (pAgregarTR)
    {
        fila += '</tr>';
        /*// No añadir la fila si ya existe en el panel correspondiente
        const panelContenedorEntidades = $(`#${pControlCont}`);        
        // Evitar duplicados Buscar la fila que contiene el texto
        const filaValue = panelContenedorEntidades.find("table tbody tr td").filter(function() {
            return $(this).text().trim() === valorFinalRow;
        });

        // Si ya existe, evitar duplicados
        if (filaValue.length > 0) {
            fila = "";
        } 
        */    

    }
    
    return fila;
}

function LimpiarContenedorGrupoPaneles(pEntidad, pPropiedad, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados){
    var contenedor = ObtenerControlEntidadProp(pEntidad + ',' + pPropiedad, pTxtIDs);
    
    if (contenedor != '' && document.getElementById(contenedor) != null)
    {
        var htmlFinal = '<tr>' + document.getElementById(contenedor).children[0].children[0].children[0].innerHTML + '</tr>';
        
        var contenedorHTML=document.getElementById(contenedor).innerHTML;
        contenedorHTML=contenedorHTML.replace(document.getElementById(contenedor).children[0].children[0].innerHTML,htmlFinal);       
        document.getElementById(contenedor).innerHTML=contenedorHTML;
        
        document.getElementById(contenedor).style.display='none';
    }
}

function SeleccionarElementoGrupoPaneles(pEntidad, pPropiedad, pEntidadHija, pNumElem, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados){
    TxtHackHayCambios = true;
    DeleteElementoGuardado(null, pEntidadHija + '&ULT', pTxtValores, pTxtElemEditados, 0);
    MarcarElementoEditado(pEntidad, pPropiedad, pNumElem, pTxtElemEditados, pTxtCaract);
    DarValorAPropiedadesDeEntidad(pEntidadHija, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados);

    EstablecerBotonesEntidad(pEntidad, pPropiedad, false, pTxtIDs);

    var superClases = GetSuperClasesEntidad(pEntidadHija);

    if (superClases != null) {
        var combosSubClase = $('select.cmbSubClass_' + ObtenerTextoGeneracionIDs(superClases[0]));

        if (combosSubClase.length > 0) {
            if (!EntidadSubClaseSeleccionada(pEntidadHija, true)) {
                for (var i = 0; i < combosSubClase[0].options.length; i++) {
                    if (combosSubClase[0].options[i].value == pEntidadHija) {
                        combosSubClase[0].selectedIndex = i;
                        break;
                    }
                }
                
                AjustarHerederasEntidad(combosSubClase[0], superClases[0], false);
            }

            combosSubClase[0].style.display = 'none';
        }
    }
}

function DarValorAPropiedadesDeEntidad(pEntidad, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados){
    var propiedades = GetPropiedadesEntidad(pEntidad, pTxtCaract);
    
    for (var i=0;i<propiedades.length;i++)
    {
        if (propiedades[i] != '')
        {
            DarValorAPropiedadDeEntidad(pEntidad, propiedades[i], pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados);
        }
    }
}

function DarValorAPropiedadDeEntidad(pEntidad, pPropiedad, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados){
	//TODO Comprobar corrección.
    var tipoProp = GetTipoPropiedad(pEntidad, pPropiedad, pTxtCaract);
    var camposCorrectos = true;

    if (tipoProp == 'FD' || tipoProp == 'CD' || tipoProp == 'FSEO' || tipoProp == 'CSEO')
    {
        var valorProp = GetValorElementoEditadoGuardado(pEntidad, pPropiedad, pTxtValores, pTxtElemEditados);
        valorProp = GetValorDecode(valorProp);
        
        if (EsPropiedadMultiIdioma(pEntidad, pPropiedad)) {
            var idControlCampoPes = ObtenerControlEntidadProp(pEntidad + ',' + pPropiedad, TxtRegistroIDs).replace('Campo_', 'divContPesIdioma_');

            if (document.getElementById(idControlCampoPes) != null) {
                var li = $('li.active', $('#' + idControlCampoPes));
                var idiomaActual = li.attr('rel');
                $('#' + idControlCampoPes).attr('langActual', valorProp);
                valorProp = ExtraerTextoIdioma(valorProp, idiomaActual);
            }
            else { //Es multiIdioma sin pestaña
                var idiomas = IdiomasConfigFormSem.split(',');
                for (var i = 0; i < idiomas.length; i++) {
                    if (idiomas[i] != IdiomaDefectoFormSem) {
                        DarValorControlConIdioma(pEntidad + ',' + pPropiedad, pTxtIDs, pTxtCaract, ExtraerTextoIdioma(valorProp, idiomas[i]), idiomas[i]);
                    }
                }

                valorProp = ExtraerTextoIdioma(valorProp, IdiomaDefectoFormSem);
            }
        }

        DarValorControl(pEntidad + ',' + pPropiedad, pTxtIDs, pTxtCaract, valorProp);

        var idControl = ObtenerControlEntidadProp(pEntidad + ',' + pPropiedad, pTxtIDs);

        if (idControl.indexOf('selEnt_') != -1) {
            var inputHack = $('#' + idControl.replace('selEnt_', 'hack_'));
            if (inputHack.length > 0 && inputHack.hasClass("autocompletarSelecEnt")) {                
                let aspaOcultaClassName = ""; 
                // Comprobar si inputHack contiene algún valor
                if (inputHack.val()) {
                    // Si inputHack contiene algún valor, ponerlo como disabled
                    inputHack.prop("disabled", true);
                } else {
                    // Si inputHack no contiene ningún valor, quitar el atributo disabled
                    inputHack.prop("disabled", false);
                    aspaOcultaClassName = "d-none";
                }
                var contenedor = $('#' + idControl).closest('div.cont');
                // Controlar visibilidad del "X" en caso de que no exista valor                
                               
                let aspa = $('a.removeAutocompletar', contenedor);                               
                if (aspa.length == 0 ) {
                    // Añadir el "X" dentro del contenedor ".input-with-icon" si existe
                    const contenedorWithIcon = contenedor.find(".input-with-icon");
                    // Mostrar la (X) aunque no haya datos cuando se esté editando
                    if (contenedorWithIcon.length > 0){
                        contenedorWithIcon.append(`<a class="remove removeAutocompletar ${aspaOcultaClassName}"></a>`);                        
                    }else{
                        contenedor.append(`<a class="remove removeAutocompletar ${aspaOcultaClassName}"></a>`);
                    }                    
                    $('a.removeAutocompletar', contenedor).click(function () {
                        inputHack.val('');
                        inputHack.prop("disabled", false);
                        $(this).remove();
                    });
                }
            }
            if (document.getElementById(idControl.replace('selEnt_', 'bkHtmlSelEnt_')) != null) {
                MontarContenedorGrupoValores(pEntidad, pPropiedad, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados);
            }
        }
    }
    else if (tipoProp == 'VD')
    {
        var valorProp = GetValorElementoGuardado(pEntidad, pPropiedad, pTxtValores, pTxtElemEditados, 0);
        var valorPropFinal = '';
        var i = 1;
        while (valorProp != "")
        {
            valorProp = GetValorDecode(valorProp);
            valorPropFinal += valorProp + ', ';
            valorProp = GetValorElementoGuardado(pEntidad, pPropiedad, pTxtValores, pTxtElemEditados, i);
            i++;
        }
        
        if (valorPropFinal != '')
        {
            valorPropFinal = valorPropFinal.substring(0, valorPropFinal.length - 2);
        }
        
        DarValorControl(pEntidad + ',' + pPropiedad, pTxtIDs, pTxtCaract, valorPropFinal);
    }
    else if (tipoProp == 'LD' || tipoProp == 'LSEO')
    {
        MontarContenedorGrupoValores(pEntidad, pPropiedad, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados);
        DarValorControl(pEntidad + ',' + pPropiedad, pTxtIDs, pTxtCaract, '');
    }
    else if (tipoProp == 'FO' || tipoProp == 'CO')
    {
        var entidadHija = GetRangoPropiedad(pEntidad, pPropiedad, pTxtCaract);
        DarValorAPropiedadesDeEntidad(entidadHija, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados);
    }
    else if (tipoProp == 'LO')
    {
        MontarContenedorGrupoPaneles(pEntidad, pPropiedad, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados);
        LimpiarControlesEntidad(GetRangoPropiedad(pEntidad, pPropiedad, pTxtCaract), pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados);
    }
}

function MontarContenedorGrupoValores(pEntidad, pPropiedad, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados){
    var valorProp = GetValorElementoGuardado(pEntidad, pPropiedad, pTxtValores, pTxtElemEditados, 0);
    var idControl = ObtenerControlEntidadProp(pEntidad + ',' + pPropiedad, pTxtIDs);
    var idContGrupoValores = idControl.replace('Campo_', 'contedor_Valores_').replace('selEnt_', 'contEntSelec_');

    if (idContGrupoValores != '' && document.getElementById(idContGrupoValores) != null) {
        document.getElementById(idContGrupoValores).innerHTML = '';
        var i = 1;
        while (valorProp != "") {
            if (idControl.indexOf('selEnt_') != -1 && valorProp != '') {
                if (document.getElementById(idControl.replace('selEnt_', 'hack_')) != null) {
                    var textoEntidad = ObtenerTextoRepEntidadExterna(valorProp);
                    if (textoEntidad != '') {
                        valorProp = textoEntidad;
                    }
                }
                else if ($('#' + idControl)[0].nodeName == 'SELECT') {
                    var url = valorProp;

                    if (url.indexOf('/') != -1) {
                        url = url.substring(url.lastIndexOf('/') + 1);
                    }

                    var opciones = $('option[value$=' + url + ']', $('#' + idControl)[0]);

                    if (opciones.length > 0) {
                        valorProp = $(opciones[0]).text();
                    }
                }
            }

            valorProp = GetValorDecode(valorProp);
            AgregarValorAContenedorGrupoValores(idContGrupoValores, valorProp, pEntidad, pPropiedad, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados);
            valorProp = GetValorElementoGuardado(pEntidad, pPropiedad, pTxtValores, pTxtElemEditados, i);
            i++;
        }

        if (idContGrupoValores.indexOf('contEntSelec_') != -1 && document.getElementById(idContGrupoValores.replace('contEntSelec_', 'bkHtmlSelEnt_')) != null) {
            $('#' + idContGrupoValores.replace('contEntSelec_', 'divContControlTes_')).css('display', 'none');
            $('#' + idContGrupoValores).css('display', '');
        }
    }
}

function MontarContenedorGrupoPaneles(pEntidad, pPropiedad, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados){
    var entidadHija = GetRangoPropiedad(pEntidad, pPropiedad, pTxtCaract);
    var valorProp = GetValorElementoGuardado(pEntidad, pPropiedad, pTxtValores, pTxtElemEditados, 0);
    var idContGrupoPaneles = ObtenerControlEntidadProp(pEntidad + ',' + pPropiedad, pTxtIDs);
    LimpiarContenedorGrupoPaneles(pEntidad, pPropiedad, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados);
    
    var mostrarContenedorAlAgregar = false;
    
    try
    {
        var botonCrear = document.getElementById(idContGrupoPaneles.replace('panel_contenedor_Entidades_','lbCrear_'));
        if (botonCrear != null)
        {
            var valorBoton = botonCrear.attributes["onclick"].value;
            
            if (valorBoton.split(',')[9] == "true")
            {
                mostrarContenedorAlAgregar = true;
            }
        }
    }
    catch(ex){}
    
    var i = 1;
    while (valorProp != "")
    {
        if (valorProp != null && valorProp != '') {//Por si hay herencia y el rango no es el tipo de entidad correcto
            entidadHija = ObtenerTipoEntidadDeValorRDF(valorProp);
        }

        AgregarEntidadAContenedorGrupoPaneles(idContGrupoPaneles, pEntidad, pPropiedad, entidadHija, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados, mostrarContenedorAlAgregar, false);
        valorProp = GetValorElementoGuardado(pEntidad, pPropiedad, pTxtValores, pTxtElemEditados, i);
        i++;
    }
}

function ObtenerTipoEntidadDeValorRDF(pValorRDF){
    var tipoEnt = pValorRDF.substring(1, pValorRDF.indexOf('>'));
    return tipoEnt;
}

function GetPropiedadesEntidad(pEntidad, pTxtCaract){
    var caract = document.getElementById(pTxtCaract).value;
    var idEnti = '|' + pEntidad + ',';
    
    var propiedades = '';
    var indiceProp = caract.indexOf(idEnti);
    while (caract.length > 0 && indiceProp != -1)
    {
        var prop = caract.substring(indiceProp + idEnti.length);
        prop = prop.substring(0, prop.indexOf(','));
        propiedades += prop + ',';
        
        caract = caract.substring(indiceProp);
        caract = caract.substring(caract.indexOf('|') + 1);
        indiceProp = caract.indexOf(idEnti);
    }
    
    return propiedades.split(',');
}

function GetTipoPropiedad(pEntidad, pPropiedad, pTxtCaract){
    return GetCaracteristicaPropiedad(pEntidad, pPropiedad, pTxtCaract, 'tipo');
}

function GetTipoCampo(pEntidad, pPropiedad, pTxtCaract){
    return GetCaracteristicaPropiedad(pEntidad, pPropiedad, pTxtCaract, 'tipoCampo');
}

function GetValorDefNoSelec(pEntidad, pPropiedad, pTxtCaract){
    return GetCaracteristicaPropiedad(pEntidad, pPropiedad, pTxtCaract, 'valDefNoSelecc');
}

function GetRangoPropiedad(pEntidad, pPropiedad, pTxtCaract){
    return GetCaracteristicaPropiedad(pEntidad, pPropiedad, pTxtCaract, 'rango');
}

function GetMensajeEliminar(pEntidad, pPropiedad,pTxtCaract){
    return GetCaracteristicaPropiedad(pEntidad, pPropiedad, pTxtCaract, 'textoEliminar');
}

function GetValorAtribRepresentantesEntidad(pEntidad, pTxtCaract) {
    if (pEntidad.indexOf('_bis') != -1)
    {
        pEntidad = pEntidad.substring(0, pEntidad.indexOf('_bis'));
    }

    var atriRepre = GetCaracteristicaPropiedad(pEntidad, '', pTxtCaract, 'atrRepre');

    if (atriRepre != null && atriRepre != '')
    {
        return atriRepre.split('&');
    }
    else
    {
        return null;
    }
}

function GetRestriccionNumCaract(pEntidad, pPropiedad, pTxtCaract){
    elemento = 'numCaract=';
    var caract = document.getElementById(pTxtCaract).value;
    var idEnti = '|' + pEntidad + ',' + pPropiedad + ',';
    caract = caract.substring(caract.indexOf(idEnti) + 1);
    caract = caract.substring(0, caract.indexOf('|'));
    
    if (caract.indexOf(elemento) != -1)
    {
        caract = caract.substring(caract.indexOf(elemento) + elemento.length);
        var numCaract = caract.substring(0, caract.indexOf(','));
        
        caract = caract.substring(caract.indexOf(',') + 1);
        numCaract += ',' + caract.substring(0, caract.indexOf(','));
        
        caract = caract.substring(caract.indexOf(',') + 1);
        numCaract += ',' + caract.substring(0, caract.indexOf(','));
        
        return numCaract;
    }
    else
    {
        return null;
    }
}

function GetCardinalidadPropiedad(pEntidad, pPropiedad, pTxtCaract){
    elemento = 'cardi=';
    var caract = document.getElementById(pTxtCaract).value;
    var idEnti = '|' + pEntidad + ',' + pPropiedad + ',';
    caract = caract.substring(caract.indexOf(idEnti) + 1);
    caract = caract.substring(0, caract.indexOf('|'));
    
    if (caract.indexOf(elemento) != -1)
    {
        caract = caract.substring(caract.indexOf(elemento) + elemento.length);
        var cardi = caract.substring(0, caract.indexOf(','));
        
        caract = caract.substring(caract.indexOf(',') + 1);
        cardi += ',' + caract.substring(0, caract.indexOf(','));
        
        return cardi;
    }
    else
    {
        return null;
    }
}

function GetCapturarFlashPropiedad(pEntidad, pPropiedad, pTxtCaract){
    elemento = 'CapturarFlash=';
    var caract = document.getElementById(pTxtCaract).value;
    var idEnti = '|' + pEntidad + ',' + pPropiedad + ',';
    caract = caract.substring(caract.indexOf(idEnti) + 1);
    caract = caract.substring(0, caract.indexOf('|'));
    
    if (caract.indexOf(elemento) != -1)
    {
        caract = caract.substring(caract.indexOf(elemento) + elemento.length);
        var cardi = caract.substring(0, caract.indexOf(','));
        
        caract = caract.substring(caract.indexOf(',') + 1);
        cardi += ',' + caract.substring(0, caract.indexOf(','));
        
        return cardi;
    }
    else
    {
        return null;
    }
}

/**
 * Esta función obtiene una característica específica asociada a una propiedad de una entidad.
 * @param {any} pEntidad El nombre de la entidad asociada a la propiedad.
 * @param {any} pPropiedad El nombre de la propiedad de la cual se desea obtener la característica.
 * @param {any} pTxtCaract El identificador del campo de texto que contiene las características asociadas a las propiedades.
 * @param {any} pElemento El nombre de la característica que se quiere obtener.
 * @returns Si encuentra la característica, devuelve su valor. Si no encuentra la característica, retorna null.
 */
function GetCaracteristicaPropiedad(pEntidad, pPropiedad, pTxtCaract, pElemento){
    pElemento = pElemento + '=';
    var caract = document.getElementById(pTxtCaract).value;
    var idEnti = '|' + pEntidad + ',' + pPropiedad + ',';
    caract = caract.substring(caract.indexOf(idEnti) + 1);
    caract = caract.substring(0, caract.indexOf('|'));
    
    if (caract.indexOf(pElemento) != -1)
    {
        caract = caract.substring(caract.indexOf(pElemento));
        return caract.substring(pElemento.length, caract.indexOf(','));
    }
    else
    {
        return null;
    }
}

function GetCaracteristicaPropiedadMultiplesValores(pEntidad, pPropiedad, pTxtCaract, pElemento, pNumValores){
    pElemento = pElemento + '=';
    var caract = document.getElementById(pTxtCaract).value;
    var idEnti = '|' + pEntidad + ',' + pPropiedad + ',';
    caract = caract.substring(caract.indexOf(idEnti) + 1);
    caract = caract.substring(0, caract.indexOf('|'));
    
    if (caract.indexOf(pElemento) != -1)
    {
        caract = caract.substring(caract.indexOf(pElemento) + pElemento.length);
        var valorFinal = '';
        for (var i=0;i<pNumValores;i++)
        {
            valorFinal += caract.substring(0, caract.indexOf(',')) + ',';
            caract = caract.substring(caract.indexOf(',') + 1);
        }
        return valorFinal;
    }
    else
    {
        return null;
    }
}

function GetEsImagenVPMiniPropiedad(pEntidad, pPropiedad, pTxtCaract){
    return GetCaracteristicaPropiedad(pEntidad, pPropiedad, pTxtCaract, 'imagenMiniVP');
}

function GetSuperClasesEntidad(pEntidad) {
    var supers = GetCaracteristicaPropiedad(pEntidad, '', TxtCaracteristicasElem, 'superclases');

    if (supers != null)
    {
        var superclases = supers.split(';');
        return superclases;
    }

    return null;
}

function GetSubClasesEntidad(pEntidad) {
    var subs = GetCaracteristicaPropiedad(pEntidad, '', TxtCaracteristicasElem, 'subclases');

    if (subs != null)
    {
        var subclases = subs.split(';');
        return subclases;
    }

    return null;
}

function GetPropiedadesDependientes(pEntidad, pPropiedad) {
    var propEnt = GetCaracteristicaPropiedadMultiplesValores(pEntidad, pPropiedad, TxtCaracteristicasElem, 'propEntDependiente', 2);

    if (propEnt != null) {
        var array = propEnt.split(',');
        array[0] = array[0].substring(1, array[0].length - 1);
        array[1] = array[1].substring(1, array[1].length - 1);
        return array;
    }

    return null;
}

function GetEsPropiedadGrafoDependiente(pEntidad, pPropiedad, pTxtCaract) {
    return (GetCaracteristicaPropiedad(pEntidad, pPropiedad, pTxtCaract, 'propGrafoDep') != null);
}

function GetEsPropiedadGrafoDependienteSinPadres(pEntidad, pPropiedad) {
    return (GetCaracteristicaPropiedad(pEntidad, pPropiedad, TxtCaracteristicasElem, 'propGrafoDepSinPadres') != null);
}

/* Operaciones sobre el RDF */

/**
 * Función que extrae el valor de un elemento XML especificado dentro de un nodo XML. 
 * Después de encontrar el elemento deseado, elimina las etiquetas de apertura y cierre y devuelve solo el valor contenido dentro del elemento.
 * @param {any} pNodo: El nodo XML del cual se extrae el valor del elemento.
 * @param {any} pElemento: El nombre del elemento XML del cual se desea obtener el valor.
 * @param {any} pNumElemet: El número de elemento que se desea obtener en caso de que haya varios elementos con el mismo nombre.
 * @returns
 */
function ObtenerValorElementoXMLNodo(pNodo, pElemento, pNumElemet)
{
    var elemento = ObtenerElementoXMLNodo(pNodo, pElemento, pNumElemet);
    elemento = elemento.substring(elemento.indexOf('>') + 1);
    elemento = elemento.substring(0, elemento.lastIndexOf('</'));
    return elemento;
}

/**
 * Esta función busca y devuelve el fragmento de texto que contiene el elemento XML especificado dentro de un nodo XML.
 * Utiliza la función ObtenerElementosXMLNodo para obtener una lista de todos los elementos en el nodo XML y luego busca el elemento deseado por su nombre (pElemento) y su número de ocurrencia (pNumElemet).
 * @param {any} pNodo
 * @param {any} pElemento
 * @param {any} pNumElemet
 * @returns
 */
function ObtenerElementoXMLNodo(pNodo, pElemento, pNumElemet)
{
    var elementos = ObtenerElementosXMLNodo(pNodo);
    var elemento = '';
    
    for (var i=0;i<elementos.length;i++)
    {
        if (elementos[i].indexOf('<' + pElemento + '>') == 0)
        {
            if (pNumElemet == -1)
            {
                elemento = elementos[i];
            }
            else
            {
                pNumElemet--;
                
                if (pNumElemet < 0)
                {
                    elemento = elementos[i];
                    break;
                }
            }
        }
    }
    
    return elemento;
}

/**
 * Función que extrae y devuelve una lista de elementos XML contenidos dentro de un nodo XML dado. 
 * Toma una cadena de texto que representa el nodo XML (pNodo), elimina las etiquetas de apertura y cierre del nodo y luego itera sobre el contenido del nodo para identificar y extraer los elementos XML individuales.
 * @param {any} pNodo La cadena de texto que representa el nodo XML del cual se desean extraer los elementos.
 * @returns
 */
function ObtenerElementosXMLNodo(pNodo)
{
    pNodo = pNodo.substring(pNodo.indexOf('>') + 1);
    pNodo = pNodo.substring(0, pNodo.lastIndexOf('</'));
    var hijosTexto = '';
    
    while (pNodo != '')
    {
        var variacion = 1;
        hijosTexto += pNodo.substring(0, pNodo.indexOf('<') + 1);
        pNodo = pNodo.substring(pNodo.indexOf('<') + 1);
        hijosTexto += pNodo.substring(0, pNodo.indexOf('>') + 1);
        pNodo = pNodo.substring(pNodo.indexOf('>') + 1);
        
        while (variacion != 0)
        {
            hijosTexto += pNodo.substring(0, pNodo.indexOf('<') + 1);
            pNodo = pNodo.substring(pNodo.indexOf('<') + 1);
            
            if (pNodo.indexOf('/') == 0) //cierre
            {
                variacion--;
            }
            else
            {
                variacion++;
            }
            
            hijosTexto += pNodo.substring(0, pNodo.indexOf('>') + 1);
            pNodo = pNodo.substring(pNodo.indexOf('>') + 1);
        }
        
        hijosTexto += '[|||]';
    }
    
    if (hijosTexto != '')
    {
        hijosTexto = hijosTexto.substring(0, hijosTexto.length - 5);
        return hijosTexto.split('[|||]');
    }
    else
    {
        return [];
    }
}

/**
 * Agrega un nuevo elemento XML a un nodo XML existente. 
 * Puede agregar el nuevo elemento al final del nodo o justo después del último elemento del mismo tipo.
 * @param {any} pNodo El nodo XML al que se le agregará el nuevo elemento.
 * @param {any} pElemento El elemento XML que se va a agregar.
 * @param {any} pNumElemet (Opcional) El número del elemento que se va a agregar. Si es -1, el elemento se agrega al final del nodo.
 * @returns
 */
function AgregarElementoAXml(pNodo, pElemento, pNumElemet)
{
    var raiz = ObtenerElementoRaiz(pNodo);
    var elementoName = ObtenerElementoRaiz(pElemento);
    var xmlFinal = '<' + raiz + '>';
    var elementos = ObtenerElementosXMLNodo(pNodo);
    var ultimoEncontrador = false;
    var encontrado = false;
    var num = pNumElemet;
    
    if (elementos.length == 0)
    {
        xmlFinal += pElemento;
    }
    else
    {
        for (var i=0;i<elementos.length;i++)
        {
            if (elementos[i].indexOf('<' + elementoName + '>') == 0)
            {
                encontrado = true;
                ultimoEncontrador = true;
                
                if (pNumElemet != -1)
                {
                    num--;
                    
                    if (num == -1)
                    {
                        xmlFinal += pElemento;
                    }
                    
                    if (num < 0)
                    {
                        ultimoEncontrador = false;
                    }
                }
            }
            else if (ultimoEncontrador)
            {
                xmlFinal += pElemento;
                ultimoEncontrador = false;
            }
            
            xmlFinal += elementos[i];
        }
        
        if (!encontrado || ultimoEncontrador)
        {
            xmlFinal += pElemento;
        }
    }
    
    xmlFinal += '</' + raiz + '>';
    return xmlFinal;
}

function BorrarElementoDeXml(pNodo, pElemento, pNumElemet)
{
    var raiz = ObtenerElementoRaiz(pNodo);
    var xmlFinal = '<' + raiz + '>';
    var elementos = ObtenerElementosXMLNodo(pNodo);
    var agregar = true;
    var num = pNumElemet;
    
    for (var i=0;i<elementos.length;i++)
    {
        if (elementos[i].indexOf('<' + pElemento + '>') == 0) {
            if (!isNaN(num)) {
                if (pNumElemet == -1) {
                    agregar = (i < elementos.length - 1 && elementos[i + 1].indexOf('<' + pElemento + '>') == 0);
                }
                else {
                    num--;

                    if (num == -1) {
                        agregar = false;
                    }
                }
            }
            else if (elementos[i].indexOf('<' + pElemento + '>' + num + '<') == 0) {
                agregar = false;
            }
        }
        
        if (agregar)
        {
            xmlFinal += elementos[i];
        }
        else
        {
            agregar = true;
        }
    }
    
    xmlFinal += '</' + raiz + '>';
    return xmlFinal;
}

function ObtenerElementoRaiz(pNodo)
{
    if (pNodo != '')
    {
        return pNodo.substring(1, pNodo.indexOf('>'));
    }
    
    return '';
}

/**
 * Esta función agrega un elemento XML con un valor específico al contenido XML representado por la cadena de texto valorRdf. 
 * Primero verifica si el elemento padre ya existe en el XML. Si existe, agrega el nuevo elemento como hijo del elemento padre. 
 * Si el elemento padre no existe, agrega el nuevo elemento al final del XML. 
 * También verifica la existencia de elementos duplicados antes de agregar el nuevo elemento.
 * @param {any} pPadre
 * @param {any} pElemento
 * @param {any} pValor
 * @param {any} pTxtValores
 * @param {any} pTxtElemEditados
 * @returns
 */
/*
function PutElementoGuardado(pPadre, pElemento, pValor, pTxtValores, pTxtElemEditados) {
    var valorRdf = document.getElementById(pTxtValores).value;

    if (pPadre != null && pPadre != '' && valorRdf.indexOf('<' + pPadre + '>') != -1) {
        var indicesIniFinPadre = GetIndiceInicioFinElementoEditado(pPadre, valorRdf, pTxtElemEditados);
        var inicio = parseInt(indicesIniFinPadre.split(',')[0]);
        var fin = parseInt(indicesIniFinPadre.split(',')[1]);

        var cierrePadre = '</' + pPadre + '>';
        var elemElemento = '<' + pElemento + '>';
        var cierreElemento = '</' + pElemento + '>';

        var trozo1 = valorRdf.substring(0, inicio);
        var trozo2 = valorRdf.substring(inicio, (fin + cierrePadre.length));
        var trozo3 = valorRdf.substring(fin + cierrePadre.length);

        // Si el cierre del elemento no está presente, significa que el elemento aún no se ha cerrado dentro del padre.
        // Agregar el nuevo elemento justo antes de cerrar el padre.
        if (trozo2.indexOf(cierreElemento) == -1) //Agregamo justo antes de acabar el padre:
        {
            trozo2 = trozo2.substring(0, trozo2.length - cierrePadre.length);
            trozo2 += elemElemento + pValor + cierreElemento + cierrePadre;
        }
        else //Agregamos detrás del último elemento del mismo tipo dentro del padre:
        {
            // Si el cierre del elemento ya está presente en trozo2,
            // el elemento ya está cerrado dentro del padre.En este caso, agrega el nuevo elemento detrás del último elemento del mismo tipo
            // que ya existe dentro del padre.Esto se hace utilizando la función AgregarElementoAXml con un índice de - 1, lo que indica que el nuevo elemento debe agregarse al final
            trozo2 = AgregarElementoAXml(trozo2, elemElemento + pValor + cierreElemento, -1);
        }

        document.getElementById(pTxtValores).value = trozo1 + trozo2 + trozo3;
        return true;    

        /*
        // Añadir el item sólo si no existe. Evitar duplicados
        const currentValorRdf = valorRdf;
        // Variable para almacenar el resultado
        let isItemAdded = true;
        // Comprobar que no existe ningún item y añadirlo
        const pValorArray = pValor.split(',').map(valor => valor.replace('|', ''));
        for (let i = 0; i < pValorArray.length; i++) {
            if (currentValorRdf.indexOf(pValorArray[i]) === -1) {
                isItemAdded = false;
                // Al menos un elemento que no existe
                break;
            }
        }
        // Añadirlo sólo si no existen los items - Evitar duplicidad
        if (!isItemAdded) {
            document.getElementById(pTxtValores).value = trozo1 + trozo2 + trozo3;
            elementoAdded = true;
        }
        return elementoAdded;
        
    }
    /*
    else if (pPadre != null && pPadre != '' && valorRdf.indexOf('<' + pPadre + '&ULT>') != -1) {
        return PutElementoGuardado(pPadre + '&ULT', pElemento, pValor, pTxtValores, pTxtElemEditados)
    }
    else {
        var elemElemento = '<' + pElemento + '>';
        var cierreElemento = '</' + pElemento + '>';

        if (pPadre == null || pPadre == '') {
            document.getElementById(pTxtValores).value += elemElemento + pValor + cierreElemento;
        }
        else {
            if (pPadre.indexOf('&ULT') == -1) {
                pPadre += '&ULT';
            }

            document.getElementById(pTxtValores).value += '<' + pPadre + '>' + elemElemento + pValor + cierreElemento + '</' + pPadre + '>';
            return true;
        }
    }
    return false;
}*/


function PutElementoGuardado(pPadre, pElemento, pValor, pTxtValores, pTxtElemEditados){
    var valorRdf = document.getElementById(pTxtValores).value;
    
    if (pPadre != null && pPadre != '' && valorRdf.indexOf('<' + pPadre+ '>') != -1)
    {
        var indicesIniFinPadre = GetIndiceInicioFinElementoEditado(pPadre, valorRdf, pTxtElemEditados);
        var inicio = parseInt(indicesIniFinPadre.split(',')[0]);
        var fin = parseInt(indicesIniFinPadre.split(',')[1]);
        
        var cierrePadre = '</' + pPadre + '>';
        var elemElemento = '<' + pElemento + '>';
        var cierreElemento = '</' + pElemento + '>';
        
        var trozo1 = valorRdf.substring(0, inicio);
        var trozo2 = valorRdf.substring(inicio, (fin + cierrePadre.length));
        var trozo3 = valorRdf.substring(fin + cierrePadre.length);
        
        if (trozo2.indexOf(cierreElemento) == -1) //Agregamo justo antes de acabar el padre:
        {
            trozo2 = trozo2.substring(0, trozo2.length - cierrePadre.length);
            trozo2 += elemElemento + pValor + cierreElemento + cierrePadre;
        }
        else //Agregamos detrás del último elemento del mismo tipo dentro del padre:
        {
            trozo2 = AgregarElementoAXml(trozo2, elemElemento + pValor + cierreElemento, -1);
        }
        
        document.getElementById(pTxtValores).value = trozo1 + trozo2 + trozo3;
        return true;
    }
    else if (pPadre != null && pPadre != '' && valorRdf.indexOf('<' + pPadre+ '&ULT>') != -1)
    {
        return PutElementoGuardado(pPadre + '&ULT', pElemento, pValor, pTxtValores, pTxtElemEditados)
    }
    else
    {
        var elemElemento = '<' + pElemento + '>';
        var cierreElemento = '</' + pElemento + '>';
        
        if (pPadre == null || pPadre == '')
        {
            document.getElementById(pTxtValores).value += elemElemento + pValor + cierreElemento;
        }
        else
        {
            if (pPadre.indexOf('&ULT') == -1)
            {
                pPadre += '&ULT';
            }
            
            document.getElementById(pTxtValores).value += '<' + pPadre + '>' + elemElemento + pValor + cierreElemento + '</' + pPadre + '>';
            return true;
        }
    }

    return false;
}


/**
 * Función que obtiene el valor de un elemento XML que ha sido editado y guardado previamente. 
 * Primero, obtiene el número de edición del elemento mediante la función GetNumEdicionEntProp. Luego, utiliza este número para obtener el valor del elemento XML mediante la función GetValorElementoGuardado.
 * @param {any} pPadre
 * @param {any} pElemento
 * @param {any} pTxtValores
 * @param {any} pTxtElemEditados
 * @returns {any} Se devuelve el valor del elemento XML.
 */
function GetValorElementoEditadoGuardado(pPadre, pElemento, pTxtValores, pTxtElemEditados){
    var numElem = GetNumEdicionEntProp(pPadre, pElemento, pTxtElemEditados);
    return GetValorElementoGuardado(pPadre, pElemento, pTxtValores, pTxtElemEditados, numElem);
}

/**
 * Este bloque de código es una sección de un método o función que busca un elemento XML dentro de un texto que representa datos RDF (Resource Description Framework). 
 * Después de encontrar este elemento, obtiene su valor y lo devuelve.
 * @param {any} pPadre El ID del elemento HTML (normalmente un textarea) donde se almacenan los valores de los datos RDF.
 * @param {any} pElemento El elemento padre del elemento que se está buscando.
 * @param {any} pTxtValores El ID del elemento HTML (normalmente un input hidden) donde se almacenan los elementos editados.
 * @param {any} pTxtElemEditados El elemento XML que se está buscando.
 * @param {any} pNumElemet El número de elemento que se desea obtener, en caso de que haya varios elementos con el mismo nombre.
 * @returns
 */
function GetValorElementoGuardado(pPadre, pElemento, pTxtValores, pTxtElemEditados, pNumElemet){
    // Rdf o valores actuales que están almacenados
    var valorRdf = document.getElementById(pTxtValores).value;
    var indicesIniFinPadre = GetIndiceInicioFinElementoEditado(pPadre, valorRdf, pTxtElemEditados);
    // Posición inicial y final del padre 
    var inicio = parseInt(indicesIniFinPadre.split(',')[0]);
    var fin = parseInt(indicesIniFinPadre.split(',')[1]);
	
	if (inicio < 0 || fin < 0)
	{
		return '';
	}

    // Construir el cierre del padre
    var cierrePadre = '</' + pPadre + '>';
    // Construir el inicio y cierre del elemento del que se desea obtener los datos (propiedad)
    var elemElemento = '<' + pElemento + '>';
    var cierreElemento = '</' + pElemento + '>';
    // Extraer el contenido XML con los valores de la propiedad de la entidad que se desea analizar
    var trozo2 = valorRdf.substring(inicio, (fin + cierrePadre.length));
    // Obtener el valor que hay guardado en el "mTxtValorRdf" a partir de la propiedad pasada (pElemento)
    var valor = ObtenerValorElementoXMLNodo(trozo2, pElemento, pNumElemet);
    return valor;
}

function GetTodosValoresElementoGuardado(pPadre, pElemento) {
    var valores = [];
    var valorRdf = document.getElementById(TxtValorRdf).value;
    var elmPadre = '<' + pPadre + '>';
    var cierrePadre = '</' + pPadre + '>';
    var inicio = valorRdf.indexOf(elmPadre);
    var fin = valorRdf.indexOf(cierrePadre);

    while (inicio >= 0 && fin >= 0) {
        
        var elemElemento = '<' + pElemento + '>';
        var cierreElemento = '</' + pElemento + '>';

        var trozo2 = valorRdf.substring(inicio, (fin + cierrePadre.length));
        valorRdf = valorRdf.substring((fin + cierrePadre.length));
        inicio = valorRdf.indexOf(elmPadre);
        fin = valorRdf.indexOf(cierrePadre);

        var count = 0;
        var valor = ObtenerValorElementoXMLNodo(trozo2, pElemento, count);
        while (valor != '') {
            valores.push(valor);
            count++;
            valor = ObtenerValorElementoXMLNodo(trozo2, pElemento, count);
        }
    }

    return valores;
}

function SetValorElementoEditadoGuardado(pPadre, pElemento, pValor, pTxtValores, pTxtElemEditados){
    var numElem = GetNumEdicionEntProp(pPadre, pElemento, pTxtElemEditados);
    return SetValorElementoGuardado(pPadre, pElemento, pValor, pTxtValores, pTxtElemEditados, numElem);
}

function SetValorElementoGuardado(pPadre, pElemento, pValor, pTxtValores, pTxtElemEditados, pNumElemet){
    var valorRdf = document.getElementById(pTxtValores).value;
    var indicesIniFinPadre = GetIndiceInicioFinElementoEditado(pPadre, valorRdf, pTxtElemEditados);
    var inicio = parseInt(indicesIniFinPadre.split(',')[0]);
    var fin = parseInt(indicesIniFinPadre.split(',')[1]);
    
    var cierrePadre = '</' + pPadre + '>';
    var elemElemento = '<' + pElemento + '>';
    var cierreElemento = '</' + pElemento + '>';
    
    var trozo1 = valorRdf.substring(0, inicio);
    var trozo2 = valorRdf.substring(inicio, (fin + cierrePadre.length));
    var trozo3 = valorRdf.substring(fin + cierrePadre.length);
    
    if (trozo2.indexOf(elemElemento) == -1)
    {
        return PutElementoGuardado(pPadre, pElemento, pValor, pTxtValores, pTxtElemEditados);
    }
    else
    {
        trozo2 = BorrarElementoDeXml(trozo2, pElemento, pNumElemet);
        trozo2 = AgregarElementoAXml(trozo2, elemElemento + pValor + cierreElemento, pNumElemet);
    }
    
    document.getElementById(pTxtValores).value = trozo1 + trozo2 + trozo3;
}

function DeleteElementoEditadoGuardado(pPadre, pElemento, pTxtValores, pTxtElemEditados){
    var numElem = GetNumEdicionEntProp(pPadre, pElemento, pTxtElemEditados);
    return DeleteElementoGuardado(pPadre, pElemento, pTxtValores, pTxtElemEditados, numElem);
}

/**
 * La función DeleteElementoGuardado se encarga de eliminar un elemento específico de un texto que representa datos RDF (Resource Description Framework) guardados. 
 * Este elemento puede ser tanto un elemento principal como un elemento secundario dentro de otro elemento.
 * @param {any} pPadre
 * @param {any} pElemento
 * @param {any} pTxtValores
 * @param {any} pTxtElemEditados
 * @param {any} pNumElemet
 * @returns
 */
function DeleteElementoGuardado(pPadre, pElemento, pTxtValores, pTxtElemEditados, pNumElemet){
    var valorRdf = document.getElementById(pTxtValores).value;
    var trozo1 = '';
    var trozo2 = '';
    var trozo3 = '';
    var elemElemento = '<' + pElemento + '>';
    var cierreElemento = '</' + pElemento + '>';

    // Comprueba si el elemElemento no existe en valorRdf. Si no existe, no procede borrarlo
    if (valorRdf.indexOf(elemElemento) == -1)
    {
        return;
    }

    if (pPadre != null && pPadre != '')
    {        
        var indicesIniFinPadre = GetIndiceInicioFinElementoEditado(pPadre, valorRdf, pTxtElemEditados);
        var inicio = parseInt(indicesIniFinPadre.split(',')[0]);
        var fin = parseInt(indicesIniFinPadre.split(',')[1]);
        
        var cierrePadre = '</' + pPadre + '>';
        
        trozo1 = valorRdf.substring(0, inicio);
        trozo2 = valorRdf.substring(inicio, (fin + cierrePadre.length));
        trozo3 = valorRdf.substring(fin + cierrePadre.length);
        
        trozo2 = BorrarElementoDeXml(trozo2, pElemento, pNumElemet);
    }
    else
    {
        trozo2 = '<padreFiccQuitar>' + valorRdf.replace('<||>', '<elemFiccQuitar></elemFiccQuitar>') + '</padreFiccQuitar>';
        trozo2 = BorrarElementoDeXml(trozo2, pElemento, pNumElemet);
        trozo2 = trozo2.replace('<elemFiccQuitar></elemFiccQuitar>', '<||>').replace('<padreFiccQuitar>', '').replace('</padreFiccQuitar>', '');
    }
    
    document.getElementById(pTxtValores).value = trozo1 + trozo2 + trozo3;
}

function MoverElementoEditadoGuardado(pPadre, pElemento, pTxtValores, pTxtElemEditados, pNumDestino){
    var numElem = GetNumEdicionEntProp(pPadre, pElemento, pTxtElemEditados);
    return MoverElementoGuardado(pPadre, pElemento, pTxtValores, pTxtElemEditados, numElem, pNumDestino);
}

function MoverElementoGuardado(pPadre, pElemento, pTxtValores, pTxtElemEditados, pNumOrigen, pNumDestino){
    var valorRdf = document.getElementById(pTxtValores).value;
    var indicesIniFinPadre = GetIndiceInicioFinElementoEditado(pPadre, valorRdf, pTxtElemEditados);
    var inicio = parseInt(indicesIniFinPadre.split(',')[0]);
    var fin = parseInt(indicesIniFinPadre.split(',')[1]);
    
    var cierrePadre = '</' + pPadre + '>';
    var elemElemento = '<' + pElemento + '>';
    var cierreElemento = '</' + pElemento + '>';
    
    var trozo1 = valorRdf.substring(0, inicio);
    var trozo2 = valorRdf.substring(inicio, (fin + cierrePadre.length));
    var trozo3 = valorRdf.substring(fin + cierrePadre.length);

    var elemento = ObtenerElementoXMLNodo(trozo2, pElemento, pNumOrigen);
    trozo2 = BorrarElementoDeXml(trozo2, pElemento, pNumOrigen);
    trozo2 = AgregarElementoAXml(trozo2, elemento, pNumDestino);

    document.getElementById(pTxtValores).value = trozo1 + trozo2 + trozo3;
}

function PutOSetElementoEditadoGuardado(pPadre, pElemento, pValor, pTxtValores, pTxtElemEditados, pTxtCaract){
    var tipoProp = GetTipoPropiedad(pPadre, pElemento, pTxtCaract);
    
    if (tipoProp == 'FD' || tipoProp == 'CD')
    {
        return SetValorElementoEditadoGuardado(pPadre, pElemento, pValor, pTxtValores, pTxtElemEditados);
    }
    else
    {
        return PutElementoGuardado(pPadre, pElemento, pValor, pTxtValores, pTxtElemEditados);
    }
}

function GetNumElementosPropiedad(pPadre, pElemento, pTxtValores, pTxtElemEditados){
    var valorRdf = document.getElementById(pTxtValores).value;
    var indicesIniFinPadre = GetIndiceInicioFinElementoEditado(pPadre, valorRdf, pTxtElemEditados);
    var inicio = parseInt(indicesIniFinPadre.split(',')[0]);
    var fin = parseInt(indicesIniFinPadre.split(',')[1]);
	
	if (inicio < 0 || fin < 0)
	{
		return 0;
	}
    
    var elemPadre = '<' + pPadre + '>';
    var cierrePadre = '</' + pPadre + '>';
    
    var trozo2 = valorRdf.substring(inicio, (fin + cierrePadre.length));
    
    var numElem = 0;

    var elementos = ObtenerElementosXMLNodo(trozo2);
    
    for (var i=0;i<elementos.length;i++)
    {
        if (elementos[i].indexOf('<' + pElemento + '>') == 0)
        {
            numElem++;
        }
    }
    
    return numElem;
}

/**
 * La función GetIndiceInicioFinElementoEditado se utiliza para obtener los índices de inicio y fin de un elemento específico dentro de un texto que representa datos RDF (Resource Description Framework) guardados. 
 * Estos índices indican la posición inicial y final del elemento en el texto.
 * @param {any} pElemento
 * @param {any} pValores
 * @param {any} pTxtElemEditados
 * @returns
 */
function GetIndiceInicioFinElementoEditado(pElemento, pValores, pTxtElemEditados){
    var valorRdf = pValores;
    var editados = document.getElementById(pTxtElemEditados).value;
    var indiceInicio = GetIndiceInicioElementoEditado(pElemento, valorRdf, editados)
    valorRdf = valorRdf.substring(indiceInicio);
    
    var elementoCierreXML = '</' + pElemento + '>';
    var indiceFin = (indiceInicio + valorRdf.indexOf(elementoCierreXML))
    
    return indiceInicio + ',' + indiceFin;
}

/**
 * Método que se utiliza para obtener el índice de inicio de un elemento específico dentro de un texto que representa datos RDF (Resource Description Framework) guardados. 
 * Este índice indica la posición inicial del elemento en el texto.
 * @param {any} pElemento
 * @param {any} pValores
 * @param {any} pEditados
 * @returns
 */
function GetIndiceInicioElementoEditado(pElemento, pValores, pEditados){
    var editados = pEditados;
    var valores = pValores;
    var indiceElmen = 0;
    var entraRecursivo = false;
    
    var elementoXML = '<' + pElemento + '>';
    var elementoCierreXML = '</' + pElemento + '>';
    
    //Buscamos en todas las entidades, por si alguna es el padre de la buscada:
    while (editados.length > 0)
    {
        var elemEditados = editados.split('|')[0];
        if (elemEditados != '')
        {
            editados = editados.substring(editados.indexOf('|') + 1);
            
            var idElem = elemEditados.split('=')[0];
            var ent = idElem.split(',')[0];
            var prop = idElem.split(',')[1];
            
            if (ent != pElemento && prop != pElemento) //No es ni la propiedad ni la entidad
            {
                var numElem = elemEditados.split('=')[1];
                var contenidoEntidad = GetValorElementoEnPosicion(prop, numElem, valores);
                
                if (contenidoEntidad.indexOf(elementoXML) != -1)
                {
                    var elmeIdElem = '<' + prop + '>';
                    var cierreElmeIdElem = '</' + prop + '>';
                    var indiceCalculado = false;
                    
                    //Compruebo que no esté seleccionando un elemento candidato a ser agregado:
                    if (editados == '' && valores.indexOf('<||>') != -1)
                    {
                        var trozoAux = valores.substring(valores.indexOf('<||>') + 4);
                        if (trozoAux.indexOf(elmeIdElem) != -1)
                        {
                            indiceElmen += valores.indexOf('<||>') + 4;
                            valores = trozoAux;
                        }
                        else if (trozoAux.indexOf('<' + pElemento + '>') != -1) {//Quitar si va mal la edición, se ha puesto por Form Generic prado
                            indiceElmen += valores.indexOf('<||>') + 4;
                            valores = trozoAux;

                            indiceElmen += valores.indexOf('<' + pElemento + '>');
                            valores = valores.substring(valores.indexOf('<' + pElemento + '>'));
                            indiceCalculado = true;
                        }
                    }
                    
                    if (!indiceCalculado) {
                        indiceElmen += valores.indexOf(elmeIdElem);
                        valores = valores.substring(valores.indexOf(elmeIdElem));

                        for (var j = numElem; j > 0; j--) {
                            indiceElmen += valores.indexOf(cierreElmeIdElem) + cierreElmeIdElem.length;
                            valores = valores.substring(valores.indexOf(cierreElmeIdElem) + cierreElmeIdElem.length);
                        }

                        valores = valores.substring(0, valores.indexOf(cierreElmeIdElem) + cierreElmeIdElem.length);
                        if (ent == pElemento) {
                            entraRecursivo = true;
                        }
                    }
                }
            }
        }
        else
        {
            break;
        }
    }
    
    if (!entraRecursivo)
    {
        return (indiceElmen + valores.lastIndexOf(elementoXML));
    }
    else if (valores.indexOf(elementoXML) != -1)
    {
        return (indiceElmen + valores.indexOf(elementoXML));
    }
    else
    {
        return indiceElmen;
    }
}

function AgregarAchivoClick(event, pControlID, pTipo) {
    TxtHackHayCambios = true;
    document.getElementById('txtHackArchivoSelecc').value = pControlID + '|' + pTipo;

    if (navigator.appName == 'Microsoft Internet Explorer' || (navigator.userAgent.indexOf('Safari') != -1 && navigator.userAgent.indexOf('Chrome') == -1)) {
		if ((typeof CalcularTopPanelYMostrarEspecifico != 'undefined')) {
			CalcularTopPanelYMostrarEspecifico(event, 'divContArchiInicio');
		}
		else
		{
			CalcularTopPanelYMostrar(event, 'divContArchiInicio');
		}

		$('#fuExaminarSemCms').change(InicioCargarArchivo_SemCms);
    }
    else {
        $('#fuExaminarSemCms').trigger('click');
    }
}

/**
 * Método para descargar un fichero que ha sido subido previamente y que una vez guardado, se precisa su descarga
 * @param {any} pElement: Elemento sobre el que se ha hecho clic.
 */
async function DownloadAchivoClick(pElement) {
    // Encuentra el área de vista previa que contiene la imagen
    const dragDropArea = $(pElement).closest(
        ".dragdropArea-preview-wrap-content"
    );
    const imageElement = dragDropArea.find("img");

    if (imageElement.length > 0) {
        const imageUrl = imageElement.attr("src");

        try {
            // Realiza una petición para obtener la imagen como blob
            const response = await fetch(imageUrl);
            const blob = await response.blob();

            // Crea un enlace de descarga utilizando el blob
            const a = document.createElement("a");
            a.href = URL.createObjectURL(blob);
            // Usa el nombre del archivo de la URL
            a.download = imageUrl.split("/").pop();
            document.body.appendChild(a);
            a.click();
            document.body.removeChild(a);
        } catch (error) {
            console.log("Error al descargar la imagen: " + error.message);
        }
    } else {
        console.log("No se encontró la imagen para descargar a descargar.");
    }
}

/**
 * Método para cargar un archivo/imagen editando un recurso del tipo Objeto de conocimiento al arrastrar un elemento (Drag & Drop).
 * Se utiliza, (por ejemplo) en la vista _PropiedadOntoData.cshtml
 * @param {any} event
 * @param {any} pControlID
 * @param {any} pTipo
 */
 function AgregarArchivoDragEvent(event, pControlID, pTipo){	
	document.getElementById('txtHackArchivoSelecc').value = pControlID + '|' + pTipo;	
	const files = event.dataTransfer.files;
	// Input donde se cargarán los elementos arrastrados
	const inputFiles = $('#fuExaminarSemCms');
		
	if (files.length > 0){		
		TxtHackHayCambios = true;
		inputFiles.get(0).files = files;
        inputFiles.trigger("change");	
	}

	event.preventDefault();
	return false;
}

function AgregarArchivoComoPropiedad(pTxtHackID, pDocumentoID, pTxtHackImgPrincipal, pRutaImg, pRutaVideos, pRutaDocsLink, pDatosPeticion, pExtension){
    var txtHack = $('#' + pTxtHackID).val();
    $('#' + pTxtHackID).val('');
    
    var fileUpload = txtHack.split('|')[1];
    var elementoID = txtHack.split('|')[2];
    var documentoID = pDocumentoID;

    var txtValoresRDF = TxtValorRdf;
    var txtIDs = TxtRegistroIDs;
    var txtCaracteristicas = TxtCaracteristicasElem;
    var txtEditados = TxtElemEditados;
    
    var valorProp = '';
    
    if (fileUpload == 'videofileUpLoad')
    {
        valorProp = pRutaVideos + documentoID.substring(0, 2) + '/' + documentoID.substring(0, 4) + '/' + documentoID + '/' + elementoID + '.flv';
    }
    else if (fileUpload == 'archivofileUpLoad' || fileUpload == 'archivoLinkfileUpLoad')
    {
        if (fileUpload == 'archivoLinkfileUpLoad') {
            valorProp = pRutaDocsLink + documentoID.substring(0, 2) + '/' + documentoID.substring(0, 4) + '/' + documentoID + '/' + pDatosPeticion;
        }
        else {
            if (pDatosPeticion == 'OK') {
                valorProp = elementoID;

                if (valorProp.indexOf('.') != -1) {
                    valorProp = valorProp.substring(0, valorProp.lastIndexOf('.'));
                    valorProp += '_' + txtHack.split('|')[3] + elementoID.substring(elementoID.lastIndexOf('.'));
                }
                else {
                    valorProp += '_' + txtHack.split('|')[3];
                }
            }
            else
            {
                valorProp = pDatosPeticion;
            }
        }
    }
    else
    {
        valorProp = pRutaImg + documentoID.substring(0, 2) + '/' + documentoID.substring(0, 4) + '/' + documentoID + '/' + elementoID + pExtension;
    }
    
    var idCampoControl = txtHack.split('|')[0];
    
    var entProp = ObtenerEntidadPropiedadSegunID(idCampoControl, txtIDs);
    
    DarValorControl(entProp[0] + ',' + entProp[1], txtIDs, txtCaracteristicas, valorProp);

    if (entProp[0].indexOf('_bis') != -1) {
        try
        {
            DarValorControl(entProp[0].substring(0, entProp[0].indexOf('_bis')) + ',' + entProp[1], txtIDs, txtCaracteristicas, valorProp);
        }
        catch (ex) { }
    }
    
    $('#' + idCampoControl.replace('Campo_','divAgregarArchivo_')).css('display', 'none');
    $('#' + idCampoControl.replace('Campo_','divArchivoAgregado_')).css('display', '');

    if (idCampoControl.replace('Campo_', 'imgVistaPre_') != 0 && $('#' + idCampoControl.replace('Campo_', 'imgVistaPre_')).length > 0)
    {
        $('#' + idCampoControl.replace('Campo_', 'imgVistaPre_'))[0].src = valorProp;
    }
    
    if (document.getElementById(idCampoControl.replace('Campo_','chkImgPrincfileUpLoad_')) != null && document.getElementById(idCampoControl.replace('Campo_','chkImgPrincfileUpLoad_')).checked)
    {
        GuardarImagenPrincipalProp(entProp[0], entProp[1], valorProp);
       //document.getElementById(pTxtHackImgPrincipal).value = valorProp;
    }
    
    //TODO ALVARO: Recuperar capturas Flash
    /*var CapturarFlash = GetCapturarFlashPropiedad(entProp[0], entProp[1], txtCaracteristicas);
    
    if (CapturarFlash != null)
    {
        valorProp = pRutaImg + documentoID + '/' + txtHack.split('|')[3] + '.jpg';
         DarValorControl(CapturarFlash.split(',')[1] + ',' + CapturarFlash.split(',')[0], txtIDs, txtCaracteristicas, valorProp);
         
         $('#txtHackValorImgRepresentante').val(valorProp);
    }*/

    if (pDatosPeticion != null && pDatosPeticion.indexOf("OpenSeaDragon") != -1) {
        openSeaDragonInfoSem += valorProp + pDatosPeticion.substring(pDatosPeticion.indexOf('|')) + '|||';
    }

    if (typeof (FinAgregarArchivoComoPropiedad) != 'undefined') {
        FinAgregarArchivoComoPropiedad();
    }
}

function EliminarArchivoDePropiedad(pEntidad, pPropiedad, pTxtValores, pTxtIDs, pTxtCaract, pTxtElemEditados, pTxtHackImgPrincipal) {
    TxtHackHayCambios = true;

    if (EsPropiedadMultiIdioma(pEntidad, pPropiedad)) {
        idioma = ObtenerIdiomaPesanyaActual(pEntidad, pPropiedad);
        var valorAntiguo = GetValorElementoGuardado(pEntidad, pPropiedad, TxtValorRdf, TxtElemEditados, 0);
        var valorProp = IncluirTextoIdiomaEnCadena(valorAntiguo, '', idioma);
        SetValorElementoEditadoGuardado(pEntidad, pPropiedad, valorProp, pTxtValores, pTxtElemEditados);
    }
    else {
        SetValorElementoEditadoGuardado(pEntidad, pPropiedad, '', pTxtValores, pTxtElemEditados);
    }

    var idControl = ObtenerControlEntidadProp(pEntidad + ',' + pPropiedad, pTxtIDs);
    DarValorControl(pEntidad + ',' + pPropiedad, pTxtIDs, pTxtCaract, '');
    
    $('#' + idControl.replace('Campo_','divAgregarArchivo_')).css('display', '');
    $('#' + idControl.replace('Campo_','divArchivoAgregado_')).css('display', 'none');
    
    if (document.getElementById(idControl.replace('Campo_','chkImgPrincfileUpLoad_')) != null)
    {
        //document.getElementById(pTxtHackImgPrincipal).value = '';
        EliminarImagenPrincipalProp(pEntidad, pPropiedad);
    }

    // Cerrar el modal ya que el mensaje de confirmación de eliminación aparecerá en un modal
    $(`#modal-container`).modal('hide');    
}

function ObtenerEntidadPropiedadSegunID(idCampoControl, pTxtIDs){
    var ids = $('#' + pTxtIDs).val();
    
    if (ids.indexOf(idCampoControl) != -1)
    {
        ids = ids.substring(0, ids.indexOf(idCampoControl) - 1);
        ids = ids.substring(ids.lastIndexOf('|') + 1);
        return ids.split(',');
    }
    
    return null;
}

function SeleccionarImagenPrincipal(pCheck, pEntidad, pPropiedad) {
    var docExtID = GetDocRecExtSelecEntEditable(pEntidad, pPropiedad, true, null);

    if (pCheck.checked)
    {
        GuardarImagenPrincipal(docExtID, document.getElementById(pCheck.id.replace('chkImgPrincfileUpLoad_', 'Campo_')).value)

        //document.getElementById(pTxtImgPrincipal).value = document.getElementById(pCheck.id.replace('chkImgPrincfileUpLoad_', 'Campo_')).value;
    }
    else
    {
        EliminarImagenPrincipal(docExtID);
        //document.getElementById(pTxtImgPrincipal).value = '';
    }
}

function GuardarImagenPrincipalProp(pEntidad, pPropiedad, pValor) {
    var docExtID = GetDocRecExtSelecEntEditable(pEntidad, pPropiedad, true, null);
    GuardarImagenPrincipal(docExtID, pValor);
}

function GuardarImagenPrincipal(pDocID, pValor) {
    EliminarImagenPrincipal(pDocID);

    if (pValor != '') {
        if (pDocID == null) {
            pDocID = 'doc';
        }

        document.getElementById('txtHackValorImgRepresentante').value += pDocID + ',' + pValor + '|';
    }
}

function ObtenerImagenPrincipal(pEntidad, pPropiedad) {
    var docExtID = GetDocRecExtSelecEntEditable(pEntidad, pPropiedad, true, null);
    var vals = document.getElementById('txtHackValorImgRepresentante').value.split('|');

    if (docExtID == null) {
        docExtID = 'doc';
    }

    for (var i = 0; i < vals.length; i++) {
        if (vals[i] != '' && vals[i].indexOf(docExtID + ',') == 0) {
            return vals[i].split(',')[1];
        }
    }

    return '';
}

function EliminarImagenPrincipalProp(pEntidad, pPropiedad) {
    var docExtID = GetDocRecExtSelecEntEditable(pEntidad, pPropiedad, true, null);
    EliminarImagenPrincipal(docExtID);
}

function EliminarImagenPrincipal(pDocID) {
    var vals = document.getElementById('txtHackValorImgRepresentante').value.split('|');
    document.getElementById('txtHackValorImgRepresentante').value = '';

    if (pDocID == null) {
        pDocID = 'doc';
    }
}

function ClickCheckPropMultiple(pCheck, pValor, pTxtControlID)
{
    if (pCheck.checked)
    {
        document.getElementById(pTxtControlID).value += pValor + ',';
    }
    else
    {
        document.getElementById(pTxtControlID).value = document.getElementById(pTxtControlID).value.replace(pValor + ',', '');
    }
}

function SeleccOpcionTab(pLink, pOpcion)
{
    var ul = pLink.parentNode.parentNode;
    
    for (var i=0;i<ul.childNodes.length;i++)
    {
        var onclick = ul.childNodes[i].childNodes[0].attributes["onclick"].value;
        var tabId = onclick.substring(onclick.lastIndexOf(',') + 1);
        tabId = tabId.substring(tabId.indexOf('\'') + 1);
        tabId = tabId.substring(0, tabId.indexOf('\''));
        
        if (tabId != pOpcion)
        {
            document.getElementById(tabId).style.display='none';
            ul.childNodes[i].className = '';
        }
        else
        {
            document.getElementById(tabId).style.display='';
            ul.childNodes[i].className = 'activo';
        }
    }
}

function AgregarEntidadSeleccAutocompletar(pData) {
    var idHack = decodeURIComponent(pData[3]);
    var idControl = idHack.replace("hack_", "selEnt_");

    if (idControl.indexOf('extra_') != -1) {
        var idAux = idControl.substring(0, idControl.indexOf('extra_'));
        idControl = idControl.substring(idControl.indexOf('selEnt_'));
        idControl = idAux + idControl;

        document.getElementById(idControl.replace("selEnt_", "hack_")).value = '';
    }

    var count = 0;
    var trozoID = 'extra_' + count + '_hack_';

    while (document.getElementById(idControl.replace("selEnt_", trozoID)) != null) {

        if (document.getElementById(idControl.replace("selEnt_", trozoID)).id != idHack) {
            document.getElementById(idControl.replace("selEnt_", trozoID)).value = '';
        }

        count++;
        trozoID = 'extra_' + count + '_hack_';
    }

    var entProp = ObtenerEntidadPropiedadSegunID(idControl, TxtRegistroIDs)
    var propiedad = entProp[1];
    var entidad = entProp[0];

    document.getElementById(idControl).value = pData[2];
    var tipoProp = GetTipoPropiedad(entidad, propiedad, TxtCaracteristicasElem);

    if (tipoProp == 'LSEO') {
        var idBotonAceptar = idControl.replace("selEnt_", "lbCrear_");
        eval(document.getElementById(idBotonAceptar).attributes["onclick"].value);
    }
    else if (tipoProp == 'FO') {
        
    }

    if (tipoProp != 'LSEO') {
        var inputHack = $('#' + idHack);
        var inputSelEnt = $('#' + idControl);
        inputHack.prop("disabled", true);
        var contenedor = $('#' + idControl).closest('div.cont');        
        contenedor.append('<a class="remove removeAutocompletar"></a>');
        // Contenedor donde iría el "X"
        const contenedorWithIcon = contenedor.find(".input-with-icon");
        if (contenedorWithIcon.length > 0){
            contenedorWithIcon.append('<a class="remove removeAutocompletar"></a>');
        }else{
            contenedor.append('<a class="remove removeAutocompletar"></a>');
        }
        $('a.remove', contenedor).click(function () {
            inputHack.val('');
            inputSelEnt.val('');
            inputHack.prop("disabled", false);
            $(this).remove();
        });
    }

    AgregarNombresCatTesSem(pData[2] + '|' + pData[0]);
}

function AgregarEntidadSeleccListaCheck(pCheck, pIdControl, pValor, pEntidad, pPropiedad) {

    var idControl = decodeURIComponent(pIdControl);
    var entProp = ObtenerEntidadPropiedadSegunID(idControl, TxtRegistroIDs)
    var propiedad = entProp[1];
    var entidad = entProp[0];

    document.getElementById(idControl).value = pValor;
    var tipoProp = GetTipoPropiedad(entidad, propiedad, TxtCaracteristicasElem);

    if (tipoProp == 'LSEO') {
        if (!pCheck.checked) {
            MarcarElementoEditado(pEntidad, pPropiedad, pValor, TxtElemEditados, TxtCaracteristicasElem);
            DeleteElementoEditadoGuardado(pEntidad, pPropiedad, TxtValorRdf, TxtElemEditados);
            MarcarElementoEditado(pEntidad, pPropiedad, -1, TxtElemEditados, TxtCaracteristicasElem);
        }
        else {
            var idBotonAceptar = idControl.replace("selEnt_", "lbCrear_");
            eval(document.getElementById(idBotonAceptar).attributes["onclick"].value);
        }
    }
    else if (tipoProp == 'FO') {

    }

}

function AgregarEntidadSeleccTesSem(pLink, pDiv, pIdControl, pValor, pGrafo, pPropSol, pPropName, pPropCatID, pNombreCat, pDivCat, pContPadreSup, pNombreCatPadre) {

    if (!pLink.checked) {
        //pLink.checked = true;
        return false;
    }

    var idControl = decodeURIComponent(pIdControl);
    var idBKHtml = idControl.replace("selEnt_", "bkHtmlSelEnt_");
    var idNameTesHack = idControl.replace("selEnt_", "hackTesNameSelEnt_");
    var idDivTes = idControl.replace('selEnt_', 'divContControlTes_');

    var elemIdBKHtml = document.getElementById(idBKHtml);
    var elemIdDivTes = document.getElementById(idDivTes);
    var elempDiv = document.getElementById(pDiv);

    if ($(elemIdBKHtml).val() == '') {
        $(elemIdBKHtml).val($(elempDiv).html());
    }

    if (pDivCat != '') {
        var valor = document.getElementById(idControl).value;
        var valorNameCat = document.getElementById(idNameTesHack).value;

        if (valor.lastIndexOf('|') == (valor.length - 1)) {
            valor = valor.substring(0, valor.lastIndexOf(',') + 1);
            valorNameCat = valorNameCat.substring(0, valorNameCat.lastIndexOf(',') + 1);
        }

        valor += pValor + ",";
        valorNameCat += decodeURIComponent(pNombreCat.replace(/\+/g, ' ')) + ",";
        document.getElementById(idControl).value = valor;
        document.getElementById(idNameTesHack).value = valorNameCat;
        MostrarUpdateProgress();
        var elems = $(pLink).parent().parent().find('input');

        for (var i = 0; i < elems.length; i++) {
			if(elems[i] != pLink){
				elems[i].checked = false;
			}
        }

        var arg = {};
        arg.Graph = pGrafo;
        arg.CategoryUri = pValor;
        arg.RequestedProperty = pPropSol;
        arg.CategoryIdProperty = pPropCatID
        arg.PropertyName = pPropName;

        GnossPeticionAjax(urlPaginaActual + '/getTesSemChildren', arg, true).done(function (data) {
            var catsNombreCats = data.split('[[|||]]');
            $('#' + pDiv).html(ConstruirHtmlCatTesHijasTraidas(catsNombreCats[0], pDiv, pIdControl, pValor, pGrafo, pPropSol, pPropName, pPropCatID, pNombreCat, pDivCat, pContPadreSup, pNombreCatPadre));
            AgregarNombresCatTesSem(catsNombreCats[1]);
        }).fail(function (data) {
            var entProp = ObtenerEntidadPropiedadSegunID(idControl, TxtRegistroIDs);
            MostrarErrorPropiedad(entProp[0], entProp[1], data, TxtRegistroIDs);
        }).always(function () {
            OcultarUpdateProgress();
        });
    }
    else {
        var valor = document.getElementById(idControl).value;
        var valorNameCat = document.getElementById(idNameTesHack).value;

        if (valor.lastIndexOf('|') == (valor.length - 1)) {
            valor = valor.substring(0, valor.lastIndexOf(',') + 1);
            valorNameCat = valorNameCat.substring(0, valorNameCat.lastIndexOf(',') + 1);
        }

        valor += pValor + "|";
        valorNameCat += decodeURIComponent(pNombreCat.replace(/\+/g, ' ')) + "|";
        document.getElementById(idControl).value = valor;
        document.getElementById(idNameTesHack).value = valorNameCat;
        var elems = $(pLink).parent().parent().find('input');

        for (var i = 0; i < elems.length; i++) {
            elems[i].checked = false;
        }

        pLink.checked = true;
    }
}

function ConstruirHtmlCatTesHijasTraidas(pCategorias, pDiv, pIdControl, pValor, pGrafo, pPropSol, pPropName, pPropCatID, pNombreCat, pDivCat, pContPadreSup, pNombreCatPadre) {
    var html = '<div>';//Abierto
    var idCheckParent = "checkTesSem_" + guidGenerator();

    html += '<input id="' + idCheckParent + '" type="checkbox" checked="checked" onclick="QuitarEntidadSeleccTesSem(this, \'' + pContPadreSup + '\',\'' + pIdControl + '\', \'' + pValor + '\', \'' + pGrafo + '\', \'' + pPropSol + '\', \'' + pPropName + '\', \'' + pPropCatID + '\', \'' + pNombreCatPadre + '\', \'' + pDiv + '\');" />';
    html += '<label for="' + idCheckParent + '">' + decodeURIComponent(pNombreCat.replace(/\+/g, ' ')) + '</label>';
    html += '<div id="' + pDivCat + '" class="hijosTesSem">';//Abierto

    var categorias = pCategorias.split('[|||]');

    for (var i = 0; i < categorias.length; i++) {
        if (categorias[i] != '') {
            var categoria = categorias[i].split('|||');
            var idDiv = '';
            var idCheck = '';
            var id = guidGenerator();

            if (categoria[2] == '1') {//Con hijos
                idDiv = "divTesSem_" + id;
            }
            idCheck = "checkTesSem_" + id;

            if (idDiv == '') {//Sin hijos
                html += '<div>';
            }
            else {
                html += '<div id="' + idDiv + '">';
            }

            html += '<input id="' + idCheck + '" type="checkbox" onclick="AgregarEntidadSeleccTesSem(this, \'' + pDivCat + '\',\'' + pIdControl + '\', \'' + categoria[0] + '\', \'' + pGrafo + '\', \'' + pPropSol + '\', \'' + pPropName + '\', \'' + pPropCatID + '\', \'' + encodeURIComponent(categoria[1]) + '\', \'' + idDiv + '\', \'' + pDiv + '\', \'' + pNombreCat + '\');" />';
            html += '<label for="' + idCheck + '">' + categoria[1] + '</label>';

            html += '</div>';
        }
    }

    html += '</div></div>';

    return html;
}

function QuitarEntidadSeleccTesSem(pLink, pDiv, pIdControl, pValor, pGrafo, pPropSol, pPropName, pPropCatID, pNombreCat, pDivCat) {

    var idControl = decodeURIComponent(pIdControl);

    var valorActual = $('#' + idControl).val();
    valorActual = valorActual.substring(0, valorActual.indexOf(pValor + ','));
    document.getElementById(idControl).value = valorActual;

    if (valorActual == '') {
        ResetearSeleccTesSem(idControl);
    }
    else {
        var valorPadre = valorActual.substring(0, valorActual.length - 1);

        if (valorPadre.indexOf(',') != -1) {
            valorPadre = valorPadre.substring(valorPadre.lastIndexOf(',') + 1);
        }

        MostrarUpdateProgress();

        var arg = {};
        arg.Graph = pGrafo;
        arg.CategoryUri = valorPadre;
        arg.RequestedProperty = pPropSol;
        arg.CategoryIdProperty = pPropCatID
        arg.PropertyName = pPropName;

        GnossPeticionAjax(urlPaginaActual + '/getTesSemChildren', arg, true).done(function (data) {
            var catsNombreCats = data.split('[[|||]]');
			var contPadreSup = $('#' + pDiv).parent().parent().attr('id');
            $('#' + pDiv).html(ConstruirHtmlCatTesHijasTraidas(catsNombreCats[0], pDiv, pIdControl, valorPadre, pGrafo, pPropSol, pPropName, pPropCatID, pNombreCat, pDivCat, contPadreSup, $("#" + contPadreSup + " div label").first().text()));
            AgregarNombresCatTesSem(catsNombreCats[1]);
        }).fail(function (data) {
            var entProp = ObtenerEntidadPropiedadSegunID(idControl, TxtRegistroIDs);
            MostrarErrorPropiedad(entProp[0], entProp[1], data, TxtRegistroIDs);
        }).always(function () {
            OcultarUpdateProgress();
        });
    }
}

function ResetearSeleccTesSem(pIdControl) {
    var divID = pIdControl.replace("selEnt_", "divCheckEnt_");
    var bkID = pIdControl.replace("selEnt_", "bkHtmlSelEnt_");
    var idNameTesHack = pIdControl.replace("selEnt_", "hackTesNameSelEnt_");

    if ($('#' + bkID).val() != '') {
        $('#' + divID).html($('#' + bkID).val());
        $('#' + pIdControl).val('');
        $('#' + bkID).val('');
        $('#' + idNameTesHack).val('');
    }
}

function AgregarNombresCatTesSem(pNombres) {
    var nomArray = pNombres.split('|||');
    var catsNombres = document.getElementById(TxtNombreCatTesSem).value;
    for (var i = 0; i < nomArray.length; i++) {
        if (nomArray[i] != '') {
            var sujeto = nomArray[i].substring(0, nomArray[i].indexOf('|'));

            if (catsNombres.indexOf(sujeto + '|') == -1) {
                catsNombres += nomArray[i] + '|||';
            }
        }
    }

    document.getElementById(TxtNombreCatTesSem).value = catsNombres;
}

function ObtenerTextoRepEntidadExterna(pValor) {
    var nombres = document.getElementById(TxtNombreCatTesSem).value;

    if (nombres.indexOf(pValor + '|') != -1) {
        nombres = nombres.substring(nombres.indexOf(pValor + '|') + pValor.length + 1);
        nombres = nombres.substring(0, nombres.indexOf('|||'));
        return nombres;
    }

    return '';
}

function AjustarHerederasEntidad(pCombo, pClaseEnt, pSustituir) {
    var herederas = $('.' + 'SuperEnt_' + ObtenerTextoGeneracionIDs(pClaseEnt));
    var entidadAnterior = null;
    var entidadNueva = null;

    for (var i = 0; i < herederas.length; i++) {
        var tipoEntidad = $(herederas[i]).attr('typeEnt');

        if ($(herederas[i]).css('display') == '' || $(herederas[i]).css('display') == 'block') {
            entidadAnterior = tipoEntidad;
        }

        if (tipoEntidad != pCombo.value) {
            $(herederas[i]).css('display', 'none');
        }
        else {
            $(herederas[i]).css('display', '');
            entidadNueva = tipoEntidad;
        }
    }

    //Todo: Ajustar valor RDF para que desaparezca la entidad anterior y sus propiedades y se ponga la nueva 
    //solo si hay que hacerlo, si la propiedad no es funcional no.

    //Si estamos con una propiedad Object habrá que reiniciar todo lo que esté por abajo de la propiedad
    if (pSustituir && !PerteneceEntidadAAlgunGrupoPanelSinEditar(pClaseEnt, TxtCaracteristicasElem, TxtElemEditados)) {
        SustituirRdfEntidadHeredada(pClaseEnt, entidadAnterior, entidadNueva);
    }
}

function SustituirRdfEntidadHeredada(pClaseEnt, tipoEntAnt, tipoEntNueva)
{
    var rdfEntHer = ObtenerParteRDFEntidadHijaDeHerencia(pClaseEnt, tipoEntNueva);

    if (tipoEntAnt == null) {
        tipoEntAnt = pClaseEnt;
    }

    var rdf = document.getElementById(TxtValorRdf).value;
    var indiceEntidad = rdf.indexOf('<' + tipoEntAnt + '>');

    if (indiceEntidad != -1) {
        var trozo1 = rdf.substring(0, indiceEntidad);
        var trozo2 = rdf.substring(rdf.indexOf('</' + tipoEntAnt + '>') + tipoEntAnt.length + 3);
        document.getElementById(TxtValorRdf).value = trozo1 + rdfEntHer + trozo2;
    }

    LimpiarControlesEntidad(tipoEntAnt, TxtValorRdf, TxtRegistroIDs, TxtCaracteristicasElem, TxtElemEditados);
}

function ObtenerParteRDFEntidadHijaDeHerencia(pTipoSuperClase, pTipoEntidadHija) {
	if(pTipoEntidadHija == null){
		return "";
	}
    var rdfHerencia = document.getElementById(TxtValorRdfHerencias).value;
    rdfHerencia = rdfHerencia.substring(rdfHerencia.indexOf('<||>' + pTipoSuperClase));
    rdfHerencia = rdfHerencia.substring(rdfHerencia.indexOf('<|>' + pTipoEntidadHija + ',') + pTipoEntidadHija.length + 4);
    rdfHerencia = rdfHerencia.substring(0, rdfHerencia.indexOf('<|>'));

    return rdfHerencia;
}

function AgregarValorGrafoDependienteAutocompletar(pData) {

    var idControl = pData[3].replace("hack_", "Campo_");
    var entProp = ObtenerEntidadPropiedadSegunID(idControl, TxtRegistroIDs)
    var propiedad = entProp[1];
    var entidad = entProp[0];

    document.getElementById(idControl).value = pData[2];

    if (document.getElementById(TxtValoresGrafoDependientes).value.indexOf(pData[2]) == -1) {
        document.getElementById(TxtValoresGrafoDependientes).value += pData[2] + ',' + pData[0] + '|';
    }

    HabilitarCamposGrafoDependientes(entidad, propiedad);
}

function AgregarValorGrafoAutocompletar(pData) {

    var idControl = pData[2];
    document.getElementById(idControl).value = pData[0];
}

function AgregarValorGrupoGnossAutocompletar(pControlID, pData) {

    var idControl = pControlID.replace("hack_", "selEnt_");
    document.getElementById(idControl).value = pData[1];

    AgregarNombresCatTesSem(pData[1] + '|' + pData[0]);
}

function SeleccionarIdioma(pLink, pEntidad, pPropiedad, pIdioma, pMultiple) {
    var li = $('li.active', $(pLink).parent().parent());
    var idiomaActual = li.attr('rel');

    var valorProp = ObtenerValorEntidadProp(pEntidad + ',' + pPropiedad, TxtRegistroIDs, TxtCaracteristicasElem);
    var campoCorrecto = ComprobarValorCampoCorrectoInt(pEntidad, pPropiedad, valorProp, TxtRegistroIDs, TxtCaracteristicasElem, false);

    if (campoCorrecto && valorProp != null) {
        valorProp = GetValorEncode(valorProp);
        var valorAntiguo = '';
        var idControlCampoPes = '';

        /*if (!pMultiple) {
            valorAntiguo = GetValorElementoGuardado(pEntidad, pPropiedad, TxtValorRdf, TxtElemEditados, 0);
        }
        else {*/
            idControlCampoPes = ObtenerControlEntidadProp(pEntidad + ',' + pPropiedad, TxtRegistroIDs).replace('Campo_', 'divContPesIdioma_');
            valorAntiguo = $('#' + idControlCampoPes).attr('langActual');
        //}

        valorProp = IncluirTextoIdiomaEnCadena(valorAntiguo, valorProp, idiomaActual);

        /*if (!pMultiple) {
            AgregarValorAPropiedad(pEntidad, pPropiedad, valorProp, TxtValorRdf);
        }
        else {*/
            $('#' + idControlCampoPes).attr('langActual', valorProp);
        //}

        //li.attr('class', '');
        li.removeClass('active');
        //$(pLink).parent().attr('class', 'active');
        $(pLink).parent().addClass('active');
        DarValorControl(pEntidad + ',' + pPropiedad, TxtRegistroIDs, TxtCaracteristicasElem, GetValorDecode(ExtraerTextoIdioma(valorAntiguo, pIdioma)));
    }
}

function IncluirTextoIdiomaEnCadena(pCadena, pTexto, pIdioma) {
    var textoFinal = '';
    var marcaIdioma = '@' + pIdioma + '[|lang|]';

    if (pCadena.indexOf(marcaIdioma) != -1) {
        textoFinal = pCadena.substring(pCadena.indexOf(marcaIdioma) + marcaIdioma.length);
        pCadena = pCadena.substring(0, pCadena.indexOf(marcaIdioma));

        if (pCadena.indexOf('[|lang|]') != -1) {
            pCadena = pCadena.substring(0, pCadena.lastIndexOf('[|lang|]') + '[|lang|]'.length);
            textoFinal = pCadena + textoFinal;
        }
    }
    else if (pCadena.indexOf('[|lang|]') != -1) {
        textoFinal = pCadena;
    }

    if (pTexto != '') {
        pTexto += marcaIdioma;
    }

    textoFinal += pTexto;

    return textoFinal;
}

function ExtraerTextoIdioma(pTexto, pIdioma) {
    var marcaIdioma = '@' + pIdioma + '[|lang|]';
    var textoFin = '';

    if (pTexto.indexOf(marcaIdioma) != -1) {
        textoFin = pTexto.substring(0, pTexto.indexOf(marcaIdioma));

        if (textoFin.indexOf('[|lang|]') != -1) {
            textoFin = textoFin.substring(textoFin.lastIndexOf('[|lang|]') + '[|lang|]'.length);
        }
    }

    return textoFin;
}

function ObtenerIdiomaPesanyaActual(pEntidad, pPropiedad) {
    var idControlCampo = ObtenerControlEntidadProp(pEntidad + ',' + pPropiedad, TxtRegistroIDs);

    if (document.getElementById(idControlCampo.replace('Campo_', 'divContPesIdioma_')) != null) {//Idioma con pestaña
        var li = $('li.active', document.getElementById(idControlCampo.replace('Campo_', 'divContPesIdioma_')));
        var idiomaActual = li.attr('rel');

        return idiomaActual;
    }

    return null;
}

function ObtenerValorMultiIdiomaPesanyaActual(pEntidad, pPropiedad, pValorProp) {

    var idControlCampo = ObtenerControlEntidadProp(pEntidad + ',' + pPropiedad, TxtRegistroIDs);
    var idControlCampoPes = idControlCampo.replace('Campo_', 'divContPesIdioma_');

    if (document.getElementById(idControlCampoPes) != null) {//Idioma con pestaña
        var li = $('li.active', document.getElementById(idControlCampoPes));
        var idiomaActual = li.attr('rel');

        var valorAntiguo = $('#' + idControlCampoPes).attr('langActual');
        var valorProp = IncluirTextoIdiomaEnCadena(valorAntiguo, pValorProp, idiomaActual);

        return valorProp;
    }
    else {//Idioma sin pestaña
        var idiomas = IdiomasConfigFormSem.split(',');
        var valorProp = '';

        for (var i = 0; i < idiomas.length; i++) {
            if (idiomas[i] != '') {
                var idiomaObt = null;

                if (idiomas[i] != IdiomaDefectoFormSem) {
                    idiomaObt = idiomas[i];
                }

                var valorIdio = ObtenerValorEntidadPropDeIdioma(pEntidad + ',' + pPropiedad, TxtRegistroIDs, TxtCaracteristicasElem, idiomaObt);
                valorIdio = GetValorEncode(valorIdio);
                valorProp = IncluirTextoIdiomaEnCadena(valorProp, valorIdio, idiomas[i]);
            }
        }

        return valorProp;
    }

    ObtenerValorEntidadPropDeIdioma
}

function ComprobarMultiIdiomaCorrecto(pEntidad, pPropiedad, pTipoProp) {
    if (EsPropiedadMultiIdioma(pEntidad, pPropiedad)) {
        if (!comprobarIdiomasRellenos) {
            var valorProp = GetValorElementoGuardado(pEntidad, pPropiedad, TxtValorRdf, TxtElemEditados, 0);

            if (valorProp != null && valorProp != '' && valorProp.indexOf('@' + IdiomaDefectoFormSem + '[|lang|]') == -1 && (pTipoProp == 'FD' || CardinalidadElementoMayorOIgualUno(pEntidad, pPropiedad, TxtCaracteristicasElem))) {
                MostrarErrorPropiedad(pEntidad, pPropiedad, textoFormSem.propSinIdiomaDefecto, TxtRegistroIDs);
                return false;
            }
        }
        else if (pTipoProp == 'FD' || CardinalidadElementoMayorOIgualUno(pEntidad, pPropiedad, TxtCaracteristicasElem)) {
            return ComprobarPropiedadTieneIdiomasUsados(pEntidad, pPropiedad);
        }
    }

    return true;
}

function ComprobarPropiedadTieneIdiomasUsados(pEntidad, pPropiedad) {
    var valoresProp = GetTodosValoresElementoGuardado(pEntidad, pPropiedad);

    for (var i = 0; i < valoresProp.length; i++) {
        if (!ComprobarValorTieneIdiomasUsados(valoresProp[i])) {
            MostrarErrorPropiedad(pEntidad, pPropiedad, textoFormSem.errorNoTodosIdiomas, TxtRegistroIDs);
            return false;
        }
    }

    return true;
}

function ComprobarValorTieneIdiomasUsados(pValor) {
    for (var i = 0; i < idiomasUsados.length - 1; i++) {
        if (pValor.indexOf('@' + idiomasUsados[i] + '[|lang|]') == -1) {
            return false;
        }
    }

    return true;
}

function ComprobarMultiIdiomaCorrectoTexto(pEntidad, pPropiedad, pTexto) {
    var tipoProp = GetTipoPropiedad(pEntidad, pPropiedad, TxtCaracteristicasElem);

    if (pTexto != null && pTexto != '' && pTexto.indexOf('@' + IdiomaDefectoFormSem + '[|lang|]') == -1 && (tipoProp == 'FD' || CardinalidadElementoMayorOIgualUno(pEntidad, pPropiedad, TxtCaracteristicasElem))) {
        MostrarErrorPropiedad(pEntidad, pPropiedad, textoFormSem.propSinIdiomaDefecto, TxtRegistroIDs);
        return false;
    }

    return true;
}

function EsPropiedadMultiIdioma(pEntidad, pPropiedad) {
    return (GetCaracteristicaPropiedad(pEntidad, pPropiedad, TxtCaracteristicasElem, 'propMultiIdioma') == 'true');
}

function EsPropiedadMultiIdiomaSinPesanya(pEntidad, pPropiedad) {
    if (EsPropiedadMultiIdioma(pEntidad, pPropiedad)) {
        var idControlCampo = ObtenerControlEntidadProp(pEntidad + ',' + pPropiedad, TxtRegistroIDs);

        return (document.getElementById(idControlCampo.replace('Campo_', 'divContPesIdioma_')) == null);
    }

    return false;
}

function GuardarCampoFechaMesAnyo(pControl, pPropiedad, pEntidad) {
    var idControl = pControl.id.replace('mCmbMes','').replace('mTxtAnyo','')
    var valorCombo = $(document.getElementById(idControl + 'mCmbMes')).val();
    var valorAnyo = $(document.getElementById(idControl + 'mTxtAnyo')).val();
    idControl = ObtenerControlEntidadProp(pEntidad + ',' + pPropiedad, TxtRegistroIDs);

    var anyo = -1;

    try{
        anyo = parseInt(valorAnyo);
    }
    catch(e){}

    if (valorCombo != '0' && anyo >= 0) {
        valorAnyo = anyo.toString();

        while (valorAnyo.length < 4) {
            valorAnyo = '0' + valorAnyo;
        }

        $(document.getElementById(idControl)).val('01/' + valorCombo + '/' + valorAnyo);
        LimpiarHtmlControl(GetIDControlError(pEntidad, pPropiedad, TxtRegistroIDs));
    }
    else {
        if ($(document.getElementById(idControl)).val() != '' && valorAnyo != '' && valorCombo != '0') {
            MostrarErrorPropiedad(pEntidad, pPropiedad, textoFormSem.errorFecha, TxtRegistroIDs);
        }
        $(document.getElementById(idControl)).val('');
    }
}

function AgregarOnBlurCKEditors() {
    var textAreas = $('textarea.cke'),
        instanceName = "",
        i = 0;

    for (i = 0; i < textAreas.length; i++) {
        var onblurArea = $(textAreas[i]).attr('onblur');
        if (onblurArea != null) {
            instanceName = textAreas[i].id;
        
            /*
            Cambiar para TinyMCE
            if (typeof (CKEDITOR.instances[instanceName]) != "undefined") {
                CKEDITOR.instances[instanceName].blurPers = onblurArea;
                CKEDITOR.instances[instanceName].on('blur', function () { eval(this.blurPers); });
            }
            */
            const editors = tinymce.get();
            const relatedTinyMCEId = instanceName;            
            
            editors.forEach(function (editor) {
                if (editor.id === relatedTinyMCEId) {
                    // TinyMCE para obtención de datos es .getContent() no getData()
                    onblurArea = onblurArea.replace("this.getData()","this.getContent()");
                    // Asignar una propiedad personalizada para la instancia de TinyMCE
                    editor.blurPers = onblurArea;                    
                    // Adjuntar un controlador de eventos para el evento blur
                    editor.on('blur', function (e) {
                        // Llamar la función asociada con blurPers
                        eval(this.blurPers);
                    });
                }
            });
        }
    }
}

function AceptarArchivosCargaMas() {
    MostrarUpdateProgress();
    var arg = {};
    arg.InfoFiles = $('#txtHackCargaMasiva').val();

    GnossPeticionAjax(urlPaginaActual + '/savemasiveloadfiles', arg, true).done(function (data) {
        MostrarRecursosMasivosSubir(data);
    }).fail(function (data) {
        CrearErrorModfRec(data);
    }).always(function () {
        OcultarUpdateProgress();
    });
}

function PrepararCargaMasivaArchivos(data) {
    if (typeof (recursoEditandoCargMasID) != 'undefined') {
        $('#divContListaRecCarMas').html('');
        MostrarRecursosMasivosSubir(data);
    }
    else {
        $('#txtHackCargaMasiva').val(data);
        $('#htitPag').css('display', 'none');
        $('#htitPagCargMas').css('display', '');

        $('#divContLblErrorDocumento').html('');
        $('#SubirRecurso').css('display', 'none');
        $('.formSemEdicion').css('display', 'none');
        $('#panPrivacidadYSeguridad').css('display', 'none');
        $('#panAutoria').css('display', 'none');
        $('#lbPublicar').css('display', 'none');

        $('#divFuExaminarMasivo').css('display', '');

        $('#fuExaminarMasivo').change(InicioCargarMasivo_SemCms);
    }
}

function MostrarRecursosMasivosSubir(data) {
    $('#htitPagCargMas').css('display', 'none');
    $('#htitPagCargMas2').css('display', '');
    GenerarRecursosMasivosSubir(data);

    $('#divContLblErrorDocumento').html('');

    $('#divFuExaminarMasivo').css('display', 'none');

    $('#divContLblErrorDocumento').html('');
    $('#SubirRecurso').css('display', 'none');
    $('.formSemEdicion').css('display', 'none');
    $('#panPrivacidadYSeguridad').css('display', 'none');
    $('#panAutoria').css('display', 'none');
    $('#lbPublicar').css('display', 'none');

    $('#divContListaRecCarMas').css('display', '');
    $('#lbGuardarBorrador').css('display', '');
}

function GenerarRecursosMasivosSubir(data) {
    $('#txtHackCargaMasiva').val(data);

    GenerarHtmlRecursosMasivosSubir(false);

    $('#lbGuardarBorrador').attr('onclick', 'if (ComprobarRecMasOk()){IncioPublicarCarMas();}');
    $('#lbGuardarBorrador').attr('tag', 'if (ComprobarRecMasOk()){IncioPublicarCarMas();}');
    $('#lbGuardarBorrador').val(textoRecursos.publicar);
    $('#lbGuardarBorrador').css('display', '');
}

function GenerarHtmlRecursosMasivosSubir(publicado) {
    var recursos = $('#txtHackCargaMasiva').val().split('<|||||>');

    for (var i = 1; i < recursos.length; i++) {
        if (recursos[i] != '') {
            var atributos = recursos[i].split('[-|-]');
            var recursoID = atributos[0];
            var archivo = atributos[1];
            var titulo = atributos[2];
            var descrp = atributos[3];
            var tags = atributos[4];

            var linkRec = null;
            var recSub = $('#txtHackCargaMasivaRecSub').val();
            if (recSub.indexOf(recursoID) != -1) {
                linkRec = recSub.substring(recSub.indexOf(recursoID) + recursoID.length + 1);
                linkRec = linkRec.substring(0, linkRec.indexOf('&'));
            }

            $('#divContListaRecCarMas').append(ObtenerHtmlRecMasivos(recursoID, titulo, descrp, tags, archivo, publicado, linkRec));
        }
    }
}

function EditarRecursoCargaMasiva(recursoID) {
    MostrarUpdateProgress();

    var valores = document.getElementById('txtHackCargaMasiva').value;

    valores = valores.substring(valores.indexOf('<|||||>' + recursoID) + 7);
    valores = valores.substring(0, valores.indexOf("<|||||>"));

    var arg = {};
    arg.InfoFiles = valores;

    GnossPeticionAjax(urlPaginaActual + '/editmasiveloadresource', arg, true).done(function (data) {
        PrepararEdicionRecursoCargaMasiva(data);
    }).fail(function (data) {
        CrearErrorModfRec(data);
    }).always(function () {
        OcultarUpdateProgress();
    });

}

function PrepararEdicionRecursoCargaMasiva(data) {
    var bkpanCargaMasiva = $('#panCargaMasiva').html();
    var bktxtHackCargaMasiva = $('#txtHackCargaMasiva').val();
    var bktxtHackCargaMasivaRecSub = $('#txtHackCargaMasivaRecSub').val();
    $('#col02').html(data);

    $('#panCargaMasiva').html(bkpanCargaMasiva);
    $('#txtHackCargaMasiva').val(bktxtHackCargaMasiva);
    $('#txtHackCargaMasivaRecSub').val(bktxtHackCargaMasivaRecSub);
    $('#htitPag').css('display', 'none');

    pintarTagsInicio();
    $('#divContLblErrorDocumento').html('');

    $('#SubirRecurso').css('display', '');
    $('.formSemEdicion').css('display', '');
    $('#panPrivacidadYSeguridad').css('display', '');
    $('#panAutoria').css('display', '');
    $('#lbPublicar').css('display', '');

    $('#divContListaRecCarMas').css('display', 'none');
    $('#lbGuardarBorrador').css('display', 'none');
}

function InicioCargarMasivo_SemCms() {
    var doc = document.getElementById("fuExaminarMasivo");
    if (doc.value.length > 0) {
        var files = $("#fuExaminarMasivo").get(0).files;
        if (files.length > 0) {

            $('#lbGuardarBorrador').css('display', 'none');
            $('#imgEsperaArchivo').css('display', '');
            $('#lblErrorCargarArchivo').css('display', '');

            archivosAsincEnCola = files.length;

            for (var i = 0; i < files.length; i++) {
                var data = new FormData();
                data.append("File", files[i]);
                data.append("FileName", files[i].name);
                data.append("documentoID", documentoID);

                AgregarArchivoCargaMas(files[i].name);

                GnossPeticionAjax(urlPaginaActual + '/attachfile', data, true).done(function (data) {
                    archivosAsincEnCola--;
                    SubidoArchivoCargaMas(data);

                    if (archivosAsincEnCola == 0) {
                        FinCargarMasivo_SemCms(data);
                    }
                }).fail(function (data) {
                    $('#imgEsperaArchivo').css('display', 'none');
                    $('#lblErrorCargarArchivo').css('display', 'none');
                    $("#fuExaminarMasivo").val('');
                    CrearErrorModfRec(data);
                });
            }
        }

        return true;
    }
}

function AgregarArchivoCargaMas(archivo) {
    var html = '<p class="archCarMas" rel="' + archivo + '"><span>' + archivo + '</span> <span class="archMasAccion">' + textoRecursos.subiendoArchivo + '...</span></p>';
    $('#divRecMasAgregados').append(html);
}

function SubidoArchivoCargaMas(archivo) {
    $('.archCarMas').each(function () {
        if ($(this).attr('rel') == archivo) {
            $('.archMasAccion', this).html(textoRecursos.archivoSubido);
        }
    });
}

function FinCargarMasivo_SemCms(data) {
    $('#lbGuardarBorrador').css('display', '');
    $('#imgEsperaArchivo').css('display', 'none');
    $('#lblErrorCargarArchivo').css('display', 'none');

    $("#fuExaminarMasivo").val('');
}

function IncioPublicarCarMas() {
    $('#lbGuardarBorrador').css('display', 'none');
    $('#imgEsperaArchivo').css('display', '');
    $('#lblCargarPublicandoCarMas').css('display', '');

    MostrarUpdateProgress();
    var arg = {};
    arg.InfoFiles = $('#txtHackCargaMasiva').val();
    arg.InfoFilesAlreadyPublished = $('#txtHackCargaMasivaRecSub').val();

    GnossPeticionAjax(urlPaginaActual + '/publishmasiveload', arg, true).done(function (data) {
        FinPublicarCarMas(data);
    }).fail(function (data) {
        CrearErrorModfRec(data);
        $('#lbGuardarBorrador').css('display', '');
    }).always(function () {
        OcultarUpdateProgress();
        $('#imgEsperaArchivo').css('display', 'none');
        $('#lblCargarPublicandoCarMas').css('display', 'none');
    });
}

function FinPublicarCarMas(data) {
    var datos = data.split('|');
    $('#txtHackCargaMasivaRecSub').val(datos[2]);
    $('#divContListaRecCarMas').html('');

    GenerarHtmlRecursosMasivosSubir(true);

    if (datos[0] == 'OK') {
        $('#lnkCancelar').css('display', 'none');
        $('#lbGuardarBorrador').css('display', 'none');
        $('#lbGuardar').css('display', 'none');
        $('#lbIrAHome').css('display', '');

        $('#divContLblErrorDocumento').html('<div id="divOkErrorDoc" class="ok" style="display:block;"><p><span ID="lblErrorDocumento">' + datos[1] + '</span></p></div>');
    }
    else {
        $('#lbGuardarBorrador').attr('value', $('#lbGuardarBorrador').attr('valueaux'));
        $('#lbGuardarBorrador').css('display', '');
        CrearErrorModfRec(datos[1]);
    }

    TxtHackHayCambios = false;
}

function ComprobarRecMasOk() {
    var divsRecMas = $('.recursoMasivo');
    var error = false;

    for (var j = 0; j < divsRecMas.length; j++) {
        var errorInt = false;
        var titulos = $('.valTituloMas', divsRecMas[j]);

        for (var i = 0; i < titulos.length; i++) {
            if ($(titulos[i]).html().trim() == '') {
                error = true;
                errorInt = true;
            }
        }

        var descrip = $('.valDescpMas', divsRecMas[j]);

        for (var i = 0; i < descrip.length; i++) {
            if ($(descrip[i]).html().trim() == '') {
                error = true;
                errorInt = true;
            }
        }

        var tags = $('.valTagsoMas', divsRecMas[j]);

        for (var i = 0; i < tags.length; i++) {
            if ($(tags[i]).html().trim() == '') {
                error = true;
                errorInt = true;
            }
        }

        if (errorInt) {
            crearErrorFormSem('<p>' + textoFormSem.errorCarMasAtr + '</p>', divsRecMas[j].id.replace('recTemp_', 'recTempError_'));
        }
        else {
            $('#' + divsRecMas[j].id.replace('recTemp_', 'recTempError_')).html('');
        }
    }

    if (error) {
        crearErrorFormSem('<p>' + textoFormSem.errorCarMasAtr + '</p>', 'divContLblErrorDocumento');
    }
    else {
        $('#divContLblErrorDocumento').html('');
    }

    return !error;
}


function EditarPropsPrincRecCarMas(evento, pDocID) {
    $('#divContLblErrorDocumento').html('');
    var valores = document.getElementById('txtHackCargaMasiva').value;

    valores = valores.substring(valores.indexOf(pDocID) + pDocID.length + 5);
    valores = valores.substring(0, valores.indexOf("<|||||>")).split('[-|-]');

    if ($('#txtHackGuardPropPrincCarMas').val() == '') {
        $('#txtHackGuardPropPrincCarMas').val($('#divTitDespTagAux').html().replace('class="ckeAux', 'class="cke'))
    }

    $('div.divRecTempDesple').html('');
    $('#recTempDesple_' + pDocID).html($('#txtHackGuardPropPrincCarMas').val());

    var archivo = valores[0];

    if (archivo.indexOf(".") != -1) {
        var extension = archivo.substring(archivo.lastIndexOf("."));
        archivo = archivo.substring(0, archivo.lastIndexOf("."));
        archivo = archivo.substring(0, archivo.lastIndexOf("_"));
        archivo += extension;
    }
    else {
        archivo = archivo.Substring(0, archivo.LastIndexOf("_"));
    }

    $('#hTitDivTitDespTagAux').html($('#hTitDivTitDespTagAux').html().replace('@1@', archivo));

    var titulo = QuitarMultiidomaValor(valores[1]);
    var descp = QuitarMultiidomaValor(valores[2]);

    $('#txtTituloAux').val(titulo);
    $('#txtDescripcionAux').val(descp);
    $('#txtTagsAux').val(valores[3].replace(/\[-\|-]/g, '&'));
    $('#txtTagsAux_Hack').val('');

    ActivarAutocompletarRec('txtTagsAux', 'sioc_t:Tag');
    pintarTagsInicio();

    EnlazarEtiquetadoAutoFormSem('txtTituloAux', 'txtDescripcionAux', 'txtHackTagsTituloAux', 'txtHackTagsDescripcionAux', 'txtTagsAux');
   
    $('#lbGuardarTitDespTagGuarCatMas').unbind('click');
    $('#lbGuardarTitDespTagGuarCatMas').bind('click', function (evento) {
        GuardarPropPrinCargMas(pDocID);
    });

    $('#divContPanelBotonera').css('display', 'none');
}

function QuitarMultiidomaValor(valor){
    var barras = valor.indexOf('|||')
    if (barras != -1 && valor.indexOf('@') == barras - 3)
    {
        return valor.substring(0, valor.indexOf('@'));
    }

    return valor;
}

function CancelarGuardarPropPrinCargMas() {
    $('div.divRecTempDesple').html('');
    $('#divContPanelBotonera').css('display', '');
}

function GuardarPropPrinCargMas(pDocID) {
    if ($('#txtTituloAux').val() == '' || $('#txtDescripcionAux').val() == '' || $('#txtTagsAux_Hack').val() == '') {
        crearErrorFormSem('<p>' + textoFormSem.errorCarMasAtr + '</p>', 'recTempError_' + pDocID);
        retur;
    }

    var valores = document.getElementById('txtHackCargaMasiva').value;

    var trozo1 = valores.substring(0, valores.indexOf(pDocID) + pDocID.length + 5);
    valores = valores.substring(valores.indexOf(pDocID) + pDocID.length + 5);
    var trozo2 = valores.substring(valores.indexOf("<|||||>"));
    valores = valores.substring(0, valores.indexOf("<|||||>")).split('[-|-]');
    var valor = '';

    for (var i = 0; i < valores.length; i++) {
        if (i == 1) {
            valor += $('#txtTituloAux').val() + '[-|-]';
        }
        else if (i == 2) {
            valor += $('#txtDescripcionAux').val() + '[-|-]';
        }
        else if (i == 3) {
            valor += $('#txtTagsAux_Hack').val() + '[-|-]';
        }
        else {
            valor += valores[i] + '[-|-]';
        }
    }

    valor = valor.substring(0, valor.length - 5);
    document.getElementById('txtHackCargaMasiva').value = trozo1 + valor + trozo2;

    $('#recTemp_' + pDocID + ' span.valTituloMas').html($('#txtTituloAux').val());
    $('#recTemp_' + pDocID + ' div.valDescpMas').html($('#txtDescripcionAux').val());
    var tags = $('#txtTagsAux_Hack').val();

    if (tags.length > 0 && tags[tags.length - 1] == ',') {
        tags = tags.substring(0, tags.length - 1);
    }

    tags = tags.replace(/\,/g, ', ');
    $('#recTemp_' + pDocID + ' span.valTagsoMas').html(tags);

    $('#divContPanelBotonera').css('display', '');
    $('div.divRecTempDesple').html('');
    $('#recTempError_' + pDocID).html('');
}

function ObtenerSelectoresDependientes(pPropiedad, pEntidad, pMultiple) {
    var valorProp = '';

    if (!pMultiple) {
        valorProp = ObtenerValorEntidadProp(pEntidad + ',' + pPropiedad, TxtRegistroIDs, TxtCaracteristicasElem);
    }
    else {
        var valores = GetTodosValoresElementoGuardado(pEntidad, pPropiedad);

        for (var i = 0; i < valores.length; i++) {
            valorProp += valores[i] + ',';
        }
    }

    if (valorProp == '' || pMultiple) {
        DeshabilitarSelectoresDependientes(pPropiedad, pEntidad);

        if (valorProp == '') {
            return;
        }
    }

    var arg = {};
    arg.PropertyName = pPropiedad;
    arg.EntityType = pEntidad;
    arg.PropertyValue = valorProp;
    MostrarUpdateProgressTime(0);

    GnossPeticionAjax(urlPaginaActual + '/getdependentselectorsentities', arg, true).done(function (data) {
        var selectores = data.split('[[|||]]');
        for (var i = 0; i < selectores.length - 1; i++) {
            var datosSelector = selectores[i].split('[[|]]');
            var idControl = ObtenerControlEntidadProp(datosSelector[1] + ',' + datosSelector[0], TxtRegistroIDs);
            var htmlCombo = '<option value="">' + $('#' + idControl)[0].options[0].innerText + '</option>';
            
            for (var j=2;j<datosSelector.length - 1;j=j+2)
            {
                htmlCombo += '<option value="'+datosSelector[j]+'">'+datosSelector[j+1]+'</option>';
            }

            $('#' + idControl).html(htmlCombo);
            $('#' + idControl).removeAttr('disabled');

            if (GetCaracteristicaPropiedad(datosSelector[1], datosSelector[0], TxtCaracteristicasElem, 'propSelectEntDep') == 'true') {
                DeshabilitarSelectoresDependientes(datosSelector[0], datosSelector[1])
            }
        }
    }).fail(function (data) {
        MostrarErrorPropiedad(pEntidad, pPropiedad, data, TxtRegistroIDs);
    }).always(function () {
        OcultarUpdateProgress();
    });
}

function DeshabilitarSelectoresDependientes(pPropiedad, pEntidad) {
    var propsDepen = GetCaracteristicaPropiedad(pEntidad, pPropiedad, TxtCaracteristicasElem, 'propSelectEntDependiente');

    if (propsDepen != null && propsDepen != '') {
        var props = propsDepen.split(';;');
        for (var i = 0; i < props.length - 1; i++) {
            var entHija = props[i].split(';')[1];
            var propHija = props[i].split(';')[0];
            LimpiarControlesPropiedadDeEntidad(entHija, propHija, TxtValorRdf, TxtRegistroIDs, TxtCaracteristicasElem, TxtElemEditados);
            var idControl = ObtenerControlEntidadProp(entHija + ',' + propHija, TxtRegistroIDs);
            if (idControl != '')
            {
                $('#' + idControl).attr('disabled', 'disabled');
            }

            if (GetCaracteristicaPropiedad(entHija, propHija, TxtCaracteristicasElem, 'propSelectEntDep') == 'true') {
                DeshabilitarSelectoresDependientes(propHija, entHija);
            }
        }
    }
}

function GetDocRecExtSelecEntEditable(pEntidad, pPropiedad, pRecursivo, pNumElem) {
    var subOntos = $('#txtSubOntologias').val();
    var indiceProp = subOntos.indexOf(pEntidad + ',' + pPropiedad + '|');

    if (indiceProp != -1) {
        subOntos = subOntos.substring(indiceProp);
        subOntos = subOntos.substring(0, subOntos.indexOf('|||'));
        var numElem = 0;

        if (pNumElem != null) {
            numElem = pNumElem;
        }
        else {
            numElem = GetNumEdicionEntProp(pEntidad, pPropiedad, TxtElemEditados);
        }

        var docID = subOntos.substring(subOntos.indexOf('|' + numElem + ','));
        docID = docID.substring(docID.indexOf(',') + 1);
        docID = docID.substring(0, docID.indexOf(','));

        return docID;
    }
    else if (pRecursivo) {
        var entProps = GetEntidadYPropiedadConEntidadComoRango(pEntidad);

        for (var i = 0; i < entProps.length; i++) {
            var docID = GetDocRecExtSelecEntEditable(entProps[i][0], entProps[i][1], pRecursivo, pNumElem);

            if (docID != null) {
                return docID;
            }
        }
    }

    return null;
}

function GuardarDocRecExtSelecEntEditable(pEntidad, pPropiedad, pAntiguoNumElem) {

    var docExtID = GetDocRecExtSelecEntEditable(pEntidad, pPropiedad, false, pAntiguoNumElem);

    if (docExtID != null) {
        var subOntos = $('#txtSubOntologias').val().split('|||');
        var subOntosFinal = '';

        for (var i = 0; i < subOntos.length; i++) {
            if (subOntos[i].indexOf(pEntidad + ',' + pPropiedad + '|') == 0) {
                var datosSub = subOntos[i].split('|');

                if (datosSub[datosSub.length - 1].split(',')[1] == docExtID) {
                    var numElem = '0';

                    if (datosSub.length > 2)
                    {
                        numElem = datosSub[datosSub.length - 2].split(',')[0];
                        numElem++;
                    }

                    var newSub = subOntos[i].replace('|-1,' + docExtID + ',', '|' + numElem + ',' + docExtID + ',');
                    newSub += '|-1,' + guidGenerator() + ',';
                    subOntosFinal += newSub + '|||';
                }
                else {
                    subOntosFinal += subOntos[i] + '|||';
                }
            }
            else if (subOntos[i] != '') {
                subOntosFinal += subOntos[i] + '|||';
            }
        }

        $('#txtSubOntologias').val(subOntosFinal);
    }
}

function BorrarDocRecExtSelecEntEditable(pEntidad, pPropiedad, pNumElem) {
    var docExtID = GetDocRecExtSelecEntEditable(pEntidad, pPropiedad, false, pNumElem);

    if (docExtID != null) {
        var subOntos = $('#txtSubOntologias').val().split('|||');
        var subOntosFinal = '';

        for (var i = 0; i < subOntos.length; i++) {
            if (subOntos[i].indexOf(pEntidad + ',' + pPropiedad + '|') == 0) {
                var datosSub = subOntos[i].split('|');
                var newSub = datosSub[0];

                var count = 0;
                for (var j = 1; j < datosSub.length; j++) {
                    var datosSubInt = datosSub[j].split(',');

                    if (parseInt(datosSubInt[0]) != pNumElem) {
                        var num = '';

                        if (datosSubInt[0] == '-1') {
                            num = '-1';
                        }
                        else {
                            num = count;
                        }

                        newSub += '|' + num + ',' + datosSubInt[1] + ',' + datosSubInt[2];
                        count++;
                    }
                }

                subOntosFinal += newSub + '|||';
            }
            else if (subOntos[i] != '') {
                subOntosFinal += subOntos[i] + '|||';
            }
        }

        $('#txtSubOntologias').val(subOntosFinal);
    }
}