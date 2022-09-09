function loadCallback(paramsCallback, controlPintado, funcionJS) {
    controlPintado.load(document.location.href + '?callback=' + paramsCallback, funcionJS);
}

var $_getVariables = { isset: false };
var $_getGlobalVariables = {};
var $_GETAllVariables = function () {
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
$_GET = function (paramToGet, jsFile) {
    if (!$_getVariables.isset)
        $_GETAllVariables();
    if (jsFile)
        return $_getVariables[jsFile][paramToGet];
    else
        return $_getGlobalVariables[paramToGet];
};

$(function () {
    if ($('#sidebar .selectorAdd').length > 0) {
        $('#sidebar .selectorAdd').listToSelect();
    }
});

if (document.cookie != "" && document.location.href.toLowerCase().indexOf('http://www.') == 0) {
    la_cookie = document.cookie.split("; ")
    fecha_fin = new Date
    fecha_fin.setDate(fecha_fin.getDate() - 1)
    for (i = 0; i < la_cookie.length; i++) {
        mi_cookie = la_cookie[i].split("=")[0]
        document.cookie = mi_cookie + "=;expires=" + fecha_fin.toGMTString()
    }
}

var contResultados = 0;
var filtrosPeticionActual = '';
//var contFacetas = 0;

var funcionExtraResultados = "";
var funcionExtraFacetas = "";

var ExpresionRegularCaracteresRepetidos = /(.)\1{2,}/;
var ExpresionRegularNombres = /^([a-zA-ZñÑüÜáéíóúÁÉÍÓÚàèìòùÀÈÌÒÙâêîôûÂÊÎÔÛ]+[a-zA-ZñÑüÜáéíóúÁÉÍÓÚàèìòùÀÈÌÒÙâêîôûÂÊÎÔÛ\s-]*[a-zA-ZñÑüÜáéíóúÁÉÍÓÚàèìòùÀÈÌÒÙâêîôûÂÊÎÔÛ]+)$/;
///^([a-zA-Z0-9-\sñÑüÜáéíóúÁÉÍÓÚàèìòùÀÈÌÒÙâêîôûÂÊÎÔÛ'´`Çç]{0,})$/;
var ExpresionNombreCorto = /^([a-zA-Z0-9ñÑüÜáéíóúÁÉÍÓÚàèìòùÀÈÌÒÙâêîôûÂÊÎÔÛ'´`çÇ-]{4,30})$/;
var ExpresionNombreCortoCentro = /^([a-zA-Z0-9ñÑüÜáéíóúÁÉÍÓÚàèìòùÀÈÌÒÙâêîôûÂÊÎÔÛ '´`çÇ-]{2,10})$/;

function pasarSegundaParteOrg() {
    var $pan1 = $('#datosCuenta');
    var $pan2 = $('#loginContactoPral');
    $pan1.fadeOut('', function () {
        $pan2.fadeIn();
    });
    return false;
}

function volverPrimeraParteOrg() {
    var $pan1 = $('#loginContactoPral');
    var $pan2 = $('#datosCuenta');
    $pan1.fadeOut('', function () {
        $pan2.fadeIn();
    });
    return false;
}

function pasarSegundaParteOrgUsu() {
    var error = '';

    if (document.getElementById('ctl00_CPH1_cbUsarMyGNOSS') != null) {
        if ($('#ctl00_CPH1_cbUsarMyGNOSS').is(':checked')) {
            if (($('#ctl00_CPH1_txtCargo').val() == '') || ($('#ctl00_CPH1_txtEmail').val() == '')) {
                if ($('#ctl00_CPH1_txtCargo').val() == '') {
                    error += '<p>' + form.camposVacios + '</p>';
                    document.getElementById('ctl00_CPH1_lblCargo').style.color = 'red';
                }
                else {
                    document.getElementById('ctl00_CPH1_lblCargo').style.color = '';
                }

                if ($('#ctl00_CPH1_txtEmail').val() == '') {
                    error += '<p>' + form.emailValido + '</p>';
                    document.getElementById('ctl00_CPH1_lblEmail').style.color = 'red';
                }
                else {
                    document.getElementById('ctl00_CPH1_lblEmail').style.color = '';
                }
            }
            else {
                document.getElementById('ctl00_CPH1_lblCargo').style.color = '';
                document.getElementById('ctl00_CPH1_lblEmail').style.color = '';
            }
        }
        else {
            if ($('#ctl00_CPH1_txtCargo').val() == '' || $('#ctl00_CPH1_txtEmail').val() == '') {
                if ($('#ctl00_CPH1_txtCargo').val() == '') {
                    error += '<p>' + form.camposVacios + '</p>';
                    document.getElementById('ctl00_CPH1_lblCargo').style.color = 'red';
                }
                else {
                    document.getElementById('ctl00_CPH1_lblCargo').style.color = '';
                }

                if ($('#ctl00_CPH1_txtEmail').val() == '') {
                    error += '<p>' + form.emailValido + '</p>';
                    document.getElementById('ctl00_CPH1_lblEmail').style.color = 'red';
                }
                else {
                    document.getElementById('ctl00_CPH1_lblEmail').style.color = '';
                }
            }
            else {
                document.getElementById('ctl00_CPH1_lblCargo').style.color = '';

                if (!validarEmail($('#ctl00_CPH1_txtEmail').val())) {
                    error += '<p>' + form.emailValido + '</p>';
                    document.getElementById('ctl00_CPH1_lblEmail').style.color = 'red';
                }
                else {
                    document.getElementById('ctl00_CPH1_lblEmail').style.color = '';
                }
            }
        }
    }


    if (!$('#ctl00_CPH1_cbLegal').is(':checked')) {
        error += '<p>' + form.aceptarLegal + '</p>';
        $('#ctl00_CPH1_lblAceptarCondiciones').attr('class', 'ko');
    }
    else {
        $('#ctl00_CPH1_lblAceptarCondiciones').attr('class', '');
    }


    if (error.length) {
        crearError(error, '#divError', true);
        return false;
    }
    else {
        return true;
    }
}

//                                                                       Busquedas guardadas
//------------------------------------------------------------------------------------------
$('ul.busquedasGuardadas a.miniEliminar').click(function () {
    miniConfirmar('Desea minieliminar esta b&uacute;squeda?', $(this).parents('li').eq(0), function () {
        alert('Funciona al uso');
    });
});

//                                                                          registro de clases
//--------------------------------------------------------------------------------------------
function registroClaseSiguiente(funcion, id, id2) {
    var error = '';

    if (!(document.getElementById('ctl00_CPH1_lblErrorLogo') == null || document.getElementById('ctl00_CPH1_lblErrorLogo').innerHTML == '')) {
        return false;
    }

    error += AgregarErrorReg(error, ValidarCentroClase('ctl00_CPH1_txtCentro', 'ctl00_CPH1_lblCentro'));
    error += AgregarErrorReg(error, ValidarAsignaturaClase('ctl00_CPH1_txtAsignatura', 'ctl00_CPH1_lblAsignatura'));
    error += AgregarErrorReg(error, ValidarCurso('ctl00_CPH1_txtCurso', 'ctl00_CPH1_lblCurso'));
    error += AgregarErrorReg(error, ValidarCPOrg('ctl00_CPH1_txtCP', 'ctl00_CPH1_lblCP'));
    error += AgregarErrorReg(error, ValidarPoblacionOrg('ctl00_CPH1_txtPoblacion', 'ctl00_CPH1_lblPoblacion'));
    error += AgregarErrorReg(error, ValidarDireccionSede('ctl00_CPH1_txtDireccionSede', 'ctl00_CPH1_lblDireccionSede'));

    error += AgregarErrorReg(error, ValidarCategorias('ctl00_CPH1_txtCat', 'ctl00_CPH1_lblCat', 'ctl00_CPH1_chkCrearComunidadPrivada'));

    error += AgregarErrorReg(error, ValidarPaisOrg('ctl00_CPH1_editPais', 'ctl00_CPH1_lblPaisResidencia'));

    error += AgregarErrorReg(error, ValidarProvinciaOrg('ctl00_CPH1_editProvincia', 'ctl00_CPH1_lblProvincia', 'ctl00_CPH1_txtProvincia'));

    error += AgregarErrorReg(error, ValidarNombreCortoCentro('ctl00_CPH1_txtNombreCortoCentro', 'ctl00_CPH1_lblNombreCortoCentro'));

    //Nombre corto de la asignatura
    error += AgregarErrorReg(error, ValidarNombreCortoAsig('ctl00_CPH1_txtNombreCortoAsignatura', 'ctl00_CPH1_lblNombreCortoAsignatura'));

    //Nombre corto del curso
    error += AgregarErrorReg(error, ValidarNombreCortoCursoIntroducido('ctl00_CPH1_txtCurso', 'ctl00_CPH1_lblCurso'));

    //Grupos    
    error += AgregarErrorReg(error, ValidarGrupos('ctl00_CPH1_txtGrupo', 'ctl00_CPH1_lblGrupo'));

    //Comprobamos si se ha seleccionado el tipo de clase    
    error += AgregarErrorReg(error, ValidarTipoClase('ctl00_CPH1_rbUni20', 'ctl00_CPH1_rbEdex', 'ctl00_CPH1_lblTipoClase'));

    if (error.length) {
        crearError(error, '#tikitakaOrg', true);
        return false;
    } else {
        if (document.getElementById('lblBtnPrefieroUsuario') != null) {
            document.getElementById('lblBtnPrefieroUsuario').style.display = 'none';
        }
        id = '#' + id;
        id2 = '#' + id2;

        //Esto es necesario ya que si no compara con el nombre acabado en <br/>        
        var a = $(id).val();
        if (a.match('<br/>') != null) {
            a = a.substring(0, a.indexOf('<br/>'));
        }

        var b = $(id2).val();
        //array url
        var b2 = b.split("<br/>");

        var clases = "";

        for (i = 0; i < b2.length; i++) {
            var arrayClase = b2[i].split("/");
            if (arrayClase[arrayClase.length - 1] != "") {
                clases = clases + "|" + arrayClase[arrayClase.length - 1];
            }
        }

        var checked = true;
        if ($('#ctl00_CPH1_chkCrearComunidadPrivada').length > 0) {
            checked = $('#ctl00_CPH1_chkCrearComunidadPrivada').is(':checked');
        }

        var comunidades = '';
        try {
            comunidades = $('#ctl00_CPH1_chkCrearComunidadPrivada').val();
        } catch (Exception) { }

        funcion = funcion.replace("$$", "&" + a.replace('\'', '\\\'') + "&" + clases + "&" + checked + "&" + comunidades);
        eval(funcion);
    }
}

function ComprobarCampoRegClase(pCampo) {
    var error = '';
    var panPintarError = '';

    if (pCampo == 'Centro') {
        error = ValidarCentroClase('ctl00_CPH1_txtCentro', 'ctl00_CPH1_lblCentro');
        panPintarError = 'divKoCentroClase';
    }
    else if (pCampo == 'Asignatura') {
        error = ValidarAsignaturaClase('ctl00_CPH1_txtAsignatura', 'ctl00_CPH1_lblAsignatura');
        panPintarError = 'divKoNombreAsig';
    }
    else if (pCampo == 'Curso') {
        error = ValidarCurso('ctl00_CPH1_txtCurso', 'ctl00_CPH1_lblCurso');
        error = ValidarNombreCortoCursoIntroducido('ctl00_CPH1_txtCurso', 'ctl00_CPH1_lblCurso');
        panPintarError = 'divKoCurso';
    }
    else if (pCampo == 'Categorias') {
        error = ValidarCategorias('ctl00_CPH1_txtCat', 'ctl00_CPH1_lblCat', 'ctl00_CPH1_chkCrearComunidadPrivada');
        panPintarError = 'divKoCategoria';
    }
    else if (pCampo == 'NombreCortoCentro') {
        error = ValidarNombreCortoCentro('ctl00_CPH1_txtNombreCortoCentro', 'ctl00_CPH1_lblNombreCortoCentro');
        panPintarError = 'divKoNombreCortoCentro';
    }
    else if (pCampo == 'Asig') {
        error = ValidarNombreCortoAsig('ctl00_CPH1_txtNombreCortoAsignatura', 'ctl00_CPH1_lblNombreCortoAsignatura');
        panPintarError = 'divKoNombreCortoAsig';
    }
    else if (pCampo == 'Grupos') {
        error = ValidarGrupos('ctl00_CPH1_txtGrupo', 'ctl00_CPH1_lblGrupo');
        panPintarError = 'divKoGrupo';
    }
    //    else if (pCampo == 'Curso')
    //    {
    //        error = ValidarNombreCortoCursoIntroducido('ctl00_CPH1_txtCurso','ctl00_CPH1_lblCurso');
    //        panPintarError = '';
    //    }
    else if (pCampo == 'TipoClase') {
        error = ValidarTipoClase('ctl00_CPH1_rbUni20', 'ctl00_CPH1_rbEdex', 'ctl00_CPH1_lblTipoClase');
        panPintarError = 'divKoTipoClase';
    }

    if (error != '') {
        crearError(error, '#' + panPintarError);
    }
    else {
        LimpiarHtmlControl(panPintarError);
    }
}

function ValidarCentroClase(pTxtCentro, pLblCentro) {
    var error = '';
    var centro = document.getElementById(pTxtCentro);
    if (centro != null && centro.value == '') {
        error += '<p>' + form.camposVacios + '</p>';
        $('#' + pLblCentro).attr('class', 'ko');
    }
    else {
        $('#' + pLblCentro).attr('class', '');
    }

    return error;
}

function ValidarAsignaturaClase(pTxtAsignatura, pLblAsignatura) {
    var error = '';
    var asignatura = document.getElementById(pTxtAsignatura);
    if (asignatura != null && asignatura.value == '') {
        error += '<p>' + form.camposVacios + '</p>';
        $('#' + pLblAsignatura).attr('class', 'ko');
    }
    else {
        $('#' + pLblAsignatura).attr('class', '');
    }

    return error;
}

function ValidarCurso(pTxtCurso, pLblCurso) {
    var error = '';
    var curso = document.getElementById(pTxtCurso);
    if (curso != null && (curso.value.length == 0 || curso.value.length > 10)) {
        error += '<p>' + form.camposVacios + '</p>';
        $('#' + pLblCurso).attr('class', 'ko');
    }
    else {
        $('#' + pLblCurso).attr('class', '');
    }

    return error;
}

function ValidarGrupos(pTxtGrupo, pLblGrupo) {
    var error = '';
    var grupos = document.getElementById(pTxtGrupo);

    var RegExPatternNombreCorto = /^([a-z,A-Z]{0,999})$/;
    var correcto = grupos.value.match(RegExPatternNombreCorto);

    if (!correcto) {
        error += '<p>' + form.grupoincorrecto + '</p>';
        $('#' + pLblGrupo).attr('class', 'ko');
    }
    else {
        $('#' + pLblGrupo).attr('class', '');
    }

    return error;
}

function ValidarCategorias(pTxtCat, pLblCat, pChkComPri) {
    var error = '';

    if ($('#' + pChkComPri).is(':checked')) {
        if ($('#' + pTxtCat).val() == '') {
            $('#' + pLblCat).attr('class', 'ko');
            error += '<p>' + form.camposVacios + '</p>';
        } else {
            $('#' + pLblCat).attr('class', '');
        }
    }

    return error;
}

function ValidarNombreCortoCentro(pTxtNombreCentro, pLblNombreCentro) {
    var error = '';

    //Nombre corto del centro
    var nombreCortoCentro = document.getElementById(pTxtNombreCentro).value;
    var errorNombreCortoCentro = !ValidarNombreCortoCentroYAsignatura(nombreCortoCentro);
    if (errorNombreCortoCentro) {
        error += '<p>' + form.nombrecortoincorrectocentro + '</p>';
        $('#' + pLblNombreCentro).attr('class', 'ko');
    }
    else {
        $('#' + pLblNombreCentro).attr('class', '');
    }

    return error;
}

function ValidarNombreCortoAsig(pTxtNombreAsig, pLblNombreAsig) {
    var error = '';

    //Nombre corto de la asignatura
    var nombreCortoAsignatura = document.getElementById(pTxtNombreAsig).value;
    var errorNombreCortoAsignatura = !ValidarNombreCortoCentroYAsignatura(nombreCortoAsignatura);
    if (errorNombreCortoAsignatura) {
        error += '<p>' + form.nombrecortoincorrectoasignatura + '</p>';
        $('#' + pLblNombreAsig).attr('class', 'ko');
    } else {
        $('#' + pLblNombreAsig).attr('class', '');
    }

    return error;
}

function ValidarNombreCortoCursoIntroducido(pTxtCurso, pLblCurso) {
    var error = '';

    //Nombre corto del curso
    var nombreCortoCurso = document.getElementById(pTxtCurso).value;
    var errorNombreCortoCurso = !ValidarNombreCortoCurso(nombreCortoCurso);
    if (errorNombreCortoCurso) {
        error += '<p>' + form.nombrecortoincorrectocurso + '</p>';
        $('#' + pLblCurso).attr('class', 'ko');
    } else {
        $('#' + pLblCurso).attr('class', '');
    }

    return error;
}

function ValidarTipoClase(pRbTipoClase1, pRbTipoClase2, pLblTipoClase) {
    var error = '';

    //Comprobamos si se ha seleccionado el tipo de clase
    var uniChecked = $('#' + pRbTipoClase1).is(':checked');
    var edexChecked = $('#' + pRbTipoClase2).is(':checked');
    if (!uniChecked && !edexChecked) {
        error += '<p>' + form.debesseleccionartipoclase + '</p>';
        $('#' + pLblTipoClase).attr('class', 'ko');
    } else {
        $('#' + pLblTipoClase).attr('class', '');
    }

    return error;
}

//                                                                  registro de organizaciones
//--------------------------------------------------------------------------------------------
function registroOrganizacionSiguiente(funcion, id, id2) {
    var error = '';

    if (!(document.getElementById('ctl00_CPH1_lblErrorLogo') == null || document.getElementById('ctl00_CPH1_lblErrorLogo').innerHTML == '')) {
        return false;
    }

    if ($('#ctl00_CPH1_txtRazonSocial').val() == ''
        || $('#ctl00_CPH1_editDia').val() == ''
        || (($('#ctl00_CPH1_txtProvincia').val() == '') && (document.getElementById('ctl00_CPH1_txtProvincia').style.display == ""))) {
        error += '<p>' + form.camposVacios + '</p>';
    }

    error += AgregarErrorReg(error, ValidarAlias('ctl00_CPH1_txtAlias', 'ctl00_CPH1_lblAlias'));
    error += AgregarErrorReg(error, ValidarRazonSocial('ctl00_CPH1_txtRazonSocial', 'ctl00_CPH1_lblRazonSocial'));
    error += AgregarErrorReg(error, ValidarDireccionSede('ctl00_CPH1_txtDireccionSede', 'ctl00_CPH1_lblDireccionSede'));
    error += AgregarErrorReg(error, ValidarCPOrg('ctl00_CPH1_txtCP', 'ctl00_CPH1_lblCP'));
    error += AgregarErrorReg(error, ValidarPoblacionOrg('ctl00_CPH1_txtPoblacion', 'ctl00_CPH1_lblPoblacion'));

    error += AgregarErrorReg(error, ValidarTipoOrg('ctl00_CPH1_ddlTipoOrganizacion', 'ctl00_CPH1_lblTipoOrganizacion'));

    error += AgregarErrorReg(error, ValidarSector('ctl00_CPH1_ddlSector', 'ctl00_CPH1_lblSector'));

    error += AgregarErrorReg(error, ValidarEmpleados('ctl00_CPH1_ddlEmpleados', 'ctl00_CPH1_lblNumeroEmpl'));

    error += AgregarErrorReg(error, ValidarPaisOrg('ctl00_CPH1_editPais', 'ctl00_CPH1_lblPaisResidencia'));

    error += AgregarErrorReg(error, ValidarProvinciaOrg('ctl00_CPH1_editProvincia', 'ctl00_CPH1_lblProvincia', 'ctl00_CPH1_txtProvincia'));

    error += AgregarErrorReg(error, ValidarNombreCortoOrgIntroducido('ctl00_CPH1_txtNombreCorto', 'ctl00_CPH1_lblNombreCorto'));

    if (error.length) {
        crearError(error, '#tikitakaOrg', true);
        return false;
    }
    else {
        if (document.getElementById('ctl00_CPH1_btnPrefieroUsuario') != null) {
            document.getElementById('ctl00_CPH1_btnPrefieroUsuario').style.display = 'none';
        }
        id = '#' + id;
        id2 = '#' + id2;
        funcion = funcion.replace("$$", "&" + $(id).val().replace('\'', '\\\'') + "&" + $(id2).val());
        eval(funcion);
    }
}

function ComprobarCampoRegOrg(pCampo) {
    var error = '';
    var panPintarError = '';

    if (pCampo == 'Alias') {
        error = ValidarAlias('ctl00_CPH1_txtAlias', 'ctl00_CPH1_lblAlias');
        panPintarError = 'divKoAlias';
    }
    else if (pCampo == 'RazonSocial') {
        error = ValidarRazonSocial('ctl00_CPH1_txtRazonSocial', 'ctl00_CPH1_lblRazonSocial');
        panPintarError = 'divKoRazonSocial';
    }
    else if (pCampo == 'DireccionSede') {
        error = ValidarDireccionSede('ctl00_CPH1_txtDireccionSede', 'ctl00_CPH1_lblDireccionSede');
        panPintarError = 'divKoDireccionSede';
    }
    else if (pCampo == 'CPOrg') {
        error = ValidarCPOrg('ctl00_CPH1_txtCP', 'ctl00_CPH1_lblCP');
        panPintarError = 'divKoCP';
    }
    else if (pCampo == 'Poblacion') {
        error = ValidarPoblacionOrg('ctl00_CPH1_txtPoblacion', 'ctl00_CPH1_lblPoblacion');
        panPintarError = 'divKoPoblacion';
    }
    else if (pCampo == 'TipoOrg') {
        error = ValidarTipoOrg('ctl00_CPH1_ddlTipoOrganizacion', 'ctl00_CPH1_lblTipoOrganizacion');
        panPintarError = 'divKoTipoOrg';
    }
    else if (pCampo == 'Sector') {
        error = ValidarSector('ctl00_CPH1_ddlSector', 'ctl00_CPH1_lblSector');
        panPintarError = 'divKoSector';
    }
    else if (pCampo == 'Empleados') {
        error = ValidarEmpleados('ctl00_CPH1_ddlEmpleados', 'ctl00_CPH1_lblNumeroEmpl');
        panPintarError = 'divKoEmpleados';
    }
    else if (pCampo == 'Pais') {
        error = ValidarPaisOrg('ctl00_CPH1_editPais', 'ctl00_CPH1_lblPaisResidencia');
        panPintarError = 'divKoPaisOrg';
    }
    else if (pCampo == 'Provincia') {
        error = ValidarProvinciaOrg('ctl00_CPH1_editProvincia', 'ctl00_CPH1_lblProvincia', 'ctl00_CPH1_txtProvincia');
        panPintarError = 'divKoProvincia';
    }
    else if (pCampo == 'NombreCortoOrg') {
        error = ValidarNombreCortoOrgIntroducido('ctl00_CPH1_txtNombreCorto', 'ctl00_CPH1_lblNombreCorto');
        panPintarError = 'divKoNombreCorto';
    }

    if (error != '') {
        crearError(error, '#' + panPintarError);
    }
    else {
        LimpiarHtmlControl(panPintarError);
    }
}

function ValidarAlias(pTxtAlias, pLblAlias) {
    var error = '';

    if ($('#' + pTxtAlias).val() == '') {
        error += '<p>' + form.camposVacios + '</p>';
        $('#' + pLblAlias).attr('class', 'ko');
    }
    else {
        $('#' + pLblAlias).attr('class', '');
    }

    //Comprobamos el alias
    if (document.getElementById(pTxtAlias) != null) {
        var aliasOrg = document.getElementById(pTxtAlias).value;
        var RegExPatternaliasOrg = /<|>$/;

        if (aliasOrg.match(RegExPatternaliasOrg) || aliasOrg.length > 30 || aliasOrg.indexOf(',') != -1) {
            error += '<p>' + form.formatoAlias + '</p>';
            $('#' + pLblAlias).attr('class', 'ko');
        }
    }

    return error;
}

function ValidarRazonSocial(pTxtRazonSocial, pLblRazonSocial) {
    var error = '';
    //Comprobamos razon social
    if (document.getElementById(pTxtRazonSocial) != null) {
        if (document.getElementById(pTxtRazonSocial).style.display != 'none' && $('#' + pTxtRazonSocial).val() == '') {
            error += '<p>' + form.camposVacios + '</p>';
            $('#' + pLblRazonSocial).attr('class', 'ko');
        }
        else {
            $('#' + pLblRazonSocial).attr('class', '');

            var razonSocialOrg = document.getElementById(pTxtRazonSocial).value;
            var RegExPatternrazonSocialOrg = /<|>$/;
            if (razonSocialOrg.match(RegExPatternrazonSocialOrg)) {
                error += '<p>' + form.formatoRazonSocial + '</p>';
                $('#' + pLblRazonSocial).attr('class', 'ko');
            }
        }
    }

    return error;
}

function ValidarDireccionSede(pTxtDireccion, pLblDireccion) {
    var error = '';

    if ($('#' + pTxtDireccion).val() == '') {
        error += '<p>' + form.camposVacios + '</p>';
        $('#' + pLblDireccion).attr('class', 'ko');
    }
    else {
        $('#' + pLblDireccion).attr('class', '');
    }

    //Comprobamos la dirección
    var direccionOrg = document.getElementById(pTxtDireccion).value;
    var RegExPatterndireccionOrg = /<|>$/;
    if (direccionOrg.match(RegExPatterndireccionOrg)) {
        error += '<p>' + form.formatoDireccion + '</p>';
        $('#' + pLblDireccion).attr('class', 'ko');
    }

    return error;
}

function ValidarCPOrg(pTxtCP, pLblCP) {
    var error = '';

    if ($('#' + pTxtCP).val() == '') {
        error += '<p>' + form.camposVacios + '</p>';
        $('#' + pLblCP).attr('class', 'ko');
    }
    else {
        $('#' + pLblCP).attr('class', '');
    }

    //Comprobamos el CP
    var CPOrg = document.getElementById(pTxtCP).value;
    var RegExPatternCPOrg = /<|>$/;
    if (CPOrg.match(RegExPatternCPOrg)) {
        error += '<p>' + form.formatoCP + '</p>';
        $('#' + pLblCP).attr('class', 'ko');
    }

    return error;
}

/**
 * Validar el campo poblaci�n introducido para el proceso de registro.
 * Si es correcto, se a�ade una clase "is-valid"
 * Si es incorrecto (vac�o) se a�ade una clase "is-invalid"
 * @param {any} pTxtPobla: Input de la poblaci�n
 * @param {any} pLblPobla: Label asociado al input poblaci�n
 */
function ValidarPoblacionOrg(pTxtPobla, pLblPobla) {
    var error = '';

    if ($('#' + pTxtPobla).val() == '') {
        error += '<p>' + form.camposVacios + '</p>';
        //$('#' + pLblPobla).attr('class', 'ko');
        $('#' + pTxtPobla).addClass('is-invalid');
        $('#' + pTxtPobla).removeClass('is-valid');
    }
    else {
        //$('#' + pLblPobla).attr('class', '');
        $('#' + pTxtPobla).addClass('is-valid');
        $('#' + pTxtPobla).removeClass('is-invalid');
    }

    //Comprobamos la localidad
    var localidadOrg = document.getElementById(pTxtPobla).value;
    var RegExPatternlocalidadOrg = /<|>$/;
    if (localidadOrg.match(RegExPatternlocalidadOrg)) {
        error += '<p>' + form.formatoLocalidad + '</p>';
        //$('#' + pLblPobla).attr('class', 'ko');
        $('#' + pTxtPobla).addClass('is-invalid');
        $('#' + pTxtPobla).removeClass('is-valid');
    }

    return error;
}

function ValidarTipoOrg(pDdlTipoOrg, pLblDdlTipoOrg) {
    var error = '';
    var comboOrg = document.getElementById(pDdlTipoOrg);

    if (comboOrg != null && comboOrg.selectedIndex == 0) {
        error += '<p>' + form.tipoorgincorrecta + '</p>';
        $('#' + pLblDdlTipoOrg).attr('class', 'ko');
    }
    else {
        $('#' + pLblDdlTipoOrg).attr('class', '');
    }

    return error;
}

function ValidarSector(pDdlSector, pLblDdlSector) {
    var error = '';
    var comboSector = document.getElementById(pDdlSector);

    if (comboSector != null && comboSector.selectedIndex == 0) {
        error += '<p>' + form.sectorincorrecto + '</p>';
        $('#' + pLblDdlSector).attr('class', 'ko');
    }
    else {
        $('#' + pLblDdlSector).attr('class', '');
    }

    return error;
}

function ValidarEmpleados(pDdlEmpleados, pLblDdlEmpleados) {
    var error = '';
    var comboEmpleados = document.getElementById(pDdlEmpleados);

    if (comboEmpleados != null && comboEmpleados.selectedIndex == 0) {
        error += '<p>' + form.numeroemplincorrecto + '</p>';
        $('#' + pLblDdlEmpleados).attr('class', 'ko');
    }
    else {
        $('#' + pLblDdlEmpleados).attr('class', '');
    }

    return error;
}

function ValidarPaisOrg(pEditPais, pLblEditPais) {
    var error = '';

    if ((document.getElementById(pEditPais) != null) && (document.getElementById(pEditPais).selectedIndex == 0)) {
        error += '<p>' + form.paisincorrecto + '</p>';
        $('#' + pLblEditPais).attr('class', 'ko');
    }
    else {
        $('#' + pLblEditPais).attr('class', '');
    }

    return error;
}

function ValidarProvinciaOrg(pEditProvincia, pLblEditProvincia, pTxtProvincia) {
    var error = '';

    if ((document.getElementById(pEditProvincia).style.display == "") && (document.getElementById(pEditProvincia).selectedIndex == 0)) {
        error += '<p>' + form.provinciaincorrecta + '</p>';
        $('#' + pLblEditProvincia).attr('class', 'ko');
    }
    else {
        $('#' + pLblEditProvincia).attr('class', '');
    }

    //Comprobamos la provincia   
    if (document.getElementById(pTxtProvincia) != null) {
        var provinciaOrg = document.getElementById(pTxtProvincia).value;

        var RegExPatternprovinciaOrg = /<|>$/;

        if (provinciaOrg.match(RegExPatternprovinciaOrg)) {
            error += '<p>' + form.formatoProvincia + '</p>';
            $('#' + pLblEditProvincia).attr('class', 'ko');
        }
    }

    if (($('#' + pTxtProvincia).val() == '') && (document.getElementById(pTxtProvincia).style.display == "")) {
        error += '<p>' + form.camposVacios + '</p>';
        $('#' + pLblEditProvincia).attr('class', 'ko');
    }

    return error;
}

function ValidarNombreCortoOrgIntroducido(pTxtNombre, pLblNombre) {
    var error = '';

    if (!ValidarNombreCortoUsuario($('#' + pTxtNombre).val())) {
        error += '<p>' + form.nombrecortoincorrecto + '</p>';
        $('#' + pLblNombre).attr('class', 'ko');
    }
    else {
        $('#' + pLblNombre).attr('class', '');
    }

    return error;
}


function validarCif(texto) {
    var pares = 0;
    var impares = 0;
    var suma;
    var ultima;
    var unumero;
    var uletra = new Array("J", "A", "B", "C", "D", "E", "F", "G", "H", "I");
    var xxx;

    var regular = new RegExp(/^[ABCDEFGHKLMNPQS]\d\d\d\d\d\d\d[0-9,A-J]$/g);

    if (!regular.exec(texto)) {
        error += '<p>' + form.nifincorrecto + '</p>';
    }
    else {
        ultima = texto.substr(8, 1);

        for (var cont = 1; cont < 7; cont++) {
            xxx = (2 * parseInt(texto.substr(cont++, 1))).toString() + "0";
            impares += parseInt(xxx.substr(0, 1)) + parseInt(xxx.substr(1, 1));
            pares += parseInt(texto.substr(cont, 1));
        }
        xxx = (2 * parseInt(texto.substr(cont, 1))).toString() + "0";
        impares += parseInt(xxx.substr(0, 1)) + parseInt(xxx.substr(1, 1));

        suma = (pares + impares).toString();
        unumero = parseInt(suma.substr(suma.length - 1, 1));
        unumero = (10 - unumero).toString();

        if (unumero == 10) unumero = 0;

        if (!((ultima == unumero) || (ultima == uletra[unumero])))
            error += '<p>' + form.nifincorrecto + '</p>';
    }
}

function validarNombreCortoComunidad(nombre) {
    //var RegExPatternNombreCorto = /^([a-zA-Z0-9ñÑüÜáéíóúÁÉÍÓÚàèìòùÀÈÌÒÙâêîôûÂÊÎÔÛ'´`çÇ-]{4,30})$/;
    // No aceptar guiones cortos ni medios para el nombre corto de la comunidad
    var RegExPatternNombreCorto = /^([a-zA-Z0-9ñÑ'´`çÇ]{4,30})$/;
    return (nombre.match(RegExPatternNombreCorto) && nombre != '');
}

//                                                                    formulario de registro
//------------------------------------------------------------------------------------------

function ComprobarCampoRegistro(paginaID, pCampo) {
    if (pCampo == 'NombreUsu') {
        ValidarNombreUsu(paginaID + '_txtNombreUsuario', paginaID + '_lblNombreUsuario');
    }
    else if (pCampo == 'Contra1') {
        ValidarContrasena(paginaID + '_txtContrasenya', '', paginaID + '_lblContrasenya', '', false);
    }
    else if (pCampo == 'Contra2') {
        ValidarContrasena(paginaID + '_txtContrasenya', paginaID + '_txtcContrasenya', paginaID + '_lblContrasenya', paginaID + '_lblConfirmarContrasenya', true);
    }
    else if (pCampo == 'Mail') {
        ValidarEmailIntroducido(paginaID + '_txtCorreoE', paginaID + '_lblEmail');
    }
    else if (pCampo == 'NombrePersonal') {
        ValidarNombrePersona(paginaID + '_txtNombrePersonal', paginaID + '_lblNombre');
    }
    else if (pCampo == 'Apellidos') {
        ValidarApellidos(paginaID + '_txtApellidos', paginaID + '_lblApellidos');
    }
    else if (pCampo == 'Provincia') {
        ValidarProvincia(paginaID + '_txtProvincia', paginaID + '_lblProvincia', paginaID + '_editProvincia');
    }
    else if (pCampo == 'Sexo') {
        ValidarSexo(paginaID + '_editSexo', paginaID + '_lblSexo');
    }
    else if (pCampo == 'CentroEstudios') {
        ValidarCentroEstudios(paginaID + '_txtCentroEstudios', paginaID + '_lbCentroEstudios');
    }
    else if (pCampo == 'AreaEstudios') {
        ValidarAreaEstudios(paginaID + '_txtAreaEstudios', paginaID + '_lbAreaEstudios');
    }
}


function registroUsuarioParte1(errores, paginaID, funcionComprobar) {
    var error = errores;
    $dC = $('#' + paginaID + '_datosUsuario');

    if ((typeof RecogerDatosExtra != 'undefined')) {
        error += RecogerDatosExtra();
    }

    error += AgregarErrorReg(error, ValidarNombrePersona(paginaID + '_txtNombrePersonal', paginaID + '_lblNombre'));
    error += AgregarErrorReg(error, ValidarApellidos(paginaID + '_txtApellidos', paginaID + '_lblApellidos'));
    error += AgregarErrorReg(error, ValidarPais(paginaID + '_ddlPais', paginaID + '_lblPais'));
    error += AgregarErrorReg(error, ValidarEmailIntroducido(paginaID + '_txtCorreoE', paginaID + '_lblEmail'));
    error += AgregarErrorReg(error, ValidarContrasena(paginaID + '_txtContrasenya', '', paginaID + '_lblContrasenya', '', false));

    var errorAvisoLegal = false;
    var error3 = '';

    //Fecha nacimiento / mayor de edad
    if (document.getElementById(paginaID + '_cbMayorEdad') != null) {
        if (!$('#' + paginaID + '_cbMayorEdad').is(':checked')) {
            errorAvisoLegal = true;
            error3 += '<p>' + form.mayorEdad + '</p>';
            $('#' + paginaID + '_lblMayorEdad').attr('class', 'ko');
        } else {
            $('#' + paginaID + '_lblMayorEdad').attr('class', '');
        }
    }
    if (document.getElementById(paginaID + '_editDia') != null) {
        error += AgregarErrorReg(error, ValidarFechaNacimiento(paginaID + '_editDia', paginaID + '_editMes', paginaID + '_editAnio', paginaID + '_lblFechaNac'));
    }

    //Compruebo cláusulas adicionales:
    if ($('#' + paginaID + '_txtHackClausulasSelecc').length && $('#' + paginaID + '_txtHackClausulasSelecc').val().indexOf('||') != -1) {
        var valorHackClau = $('#' + paginaID + '_txtHackClausulasSelecc').val();
        valorHackClau = valorHackClau.substring(0, valorHackClau.indexOf('||'));

        var color = '';
        if (valorHackClau != '0') {

            if (!errorAvisoLegal) {
                error3 += '<p>' + form.aceptarLegal + '</p>';
            }
            color = '#E24973';
        }

        var labelsClau = $('.clauAdicional');

        for (var i = 0; i < labelsClau.length; i++) {
            labelsClau[i].style.color = color;
        }
    }

    if (error.length || error3.length) {
        if (error.length) {
            crearError(error, '#divKodatosUsuario', false);
        }
        else {
            LimpiarHtmlControl('divKodatosUsuario');
        }

        if (error3.length) {
            crearError(error3, '#divKoCondicionesUso', false);
        }
        else {
            LimpiarHtmlControl('divKoCondicionesUso');
        }

        return false;
    } else {
        LimpiarHtmlControl('divKodatosUsuario');
        LimpiarHtmlControl('divKoDatosPersonales');
        LimpiarHtmlControl('divKoCondicionesUso');
        funcionComprobar = funcionComprobar.replace("$$", $('#' + paginaID + '_txtCorreoE').val() + "&" + $('#' + paginaID + '_txtNombrePersonal').val() + "&" + $('#' + paginaID + '_txtApellidos').val() + "&" + $('#' + paginaID + '_captcha').val());
        eval(funcionComprobar);

        return false;
    }
}


function registroUsuario_1(ultimoCheck, funcionComprobar) {
    var error = '',
        $dC = $('#ctl00_CPH1_datosUsuario');

    error += AgregarErrorReg(error, ValidarNombrePersona('ctl00_CPH1_txtNombrePersonal', 'ctl00_CPH1_lblNombre'));
    error += AgregarErrorReg(error, ValidarApellidos('ctl00_CPH1_txtApellidos', 'ctl00_CPH1_lblApellidos'));

    error += AgregarErrorReg(error, ValidarNombreUsu('ctl00_CPH1_txtNombreUsuario', 'ctl00_CPH1_lblNombreUsuario'));

    if (!ultimoCheck) {
        error += AgregarErrorReg(error, ValidarContrasena('ctl00_CPH1_txtContrasenya', 'ctl00_CPH1_txtcContrasenya', 'ctl00_CPH1_lblContrasenya', 'ctl00_CPH1_lblConfirmarContrasenya', true));
    }

    error += AgregarErrorReg(error, ValidarEmailIntroducido('ctl00_CPH1_txtCorreoE', 'ctl00_CPH1_lblEmail'));

    var error2 = '';
    error2 += AgregarErrorReg(error2, ValidarProvincia('ctl00_CPH1_txtProvincia', 'ctl00_CPH1_lblProvincia', 'ctl00_CPH1_editProvincia'));
    error2 += AgregarErrorReg(error2, ValidarSexo('ctl00_CPH1_editSexo', 'ctl00_CPH1_lblSexo'));
    error2 += AgregarErrorReg(error2, ValidarCentroEstudios('ctl00_CPH1_txtCentroEstudios', 'ctl00_CPH1_lbCentroEstudios'));
    error2 += AgregarErrorReg(error2, ValidarAreaEstudios('ctl00_CPH1_txtAreaEstudios', 'ctl00_CPH1_lbAreaEstudios'));
    error2 += AgregarErrorReg(error2, ComprobarFechaNac('ctl00_CPH1_editDia', 'ctl00_CPH1_editMes', 'ctl00_CPH1_editAnio', 'ctl00_CPH1_lblFechaNac', 'ctl00_CPH1_lblEdadMinima', 'ctl00_CPH1_'));

    var errorAvisoLegal = false;
    var error3 = '';

    //Compruebo cláusulas adicionales:
    if (document.getElementById('ctl00_CPH1_txtHackClausulasSelecc') != null && document.getElementById('ctl00_CPH1_txtHackClausulasSelecc').value.indexOf('||') != -1) {
        var valorHackClau = document.getElementById('ctl00_CPH1_txtHackClausulasSelecc').value;
        valorHackClau = valorHackClau.substring(0, valorHackClau.indexOf('||'));

        var color = '';
        if (valorHackClau != '0') {

            if (!errorAvisoLegal) {
                error3 += '<p>' + form.aceptarLegal + '</p>';
            }
            color = '#E24973';
        }

        var labelsClau = $('.clauAdicional');

        for (var i = 0; i < labelsClau.length; i++) {
            labelsClau[i].style.color = color;
        }
    }

    if (error.length || error2.length || error3.length) {
        if (error.length) {
            //crearError('<ul>'+error+'</ul>', '#ctl00_CPH1_datosUsuario', true);
            crearError(error, '#divKodatosUsuario', true);
        }
        else {
            LimpiarHtmlControl('divKodatosUsuario');
        }

        if (error2.length) {
            crearError(error2, '#divKoDatosPersonales', true);
            //$('body, html').animate({scrollTop: $dP.offset().top}, 600);
        }
        else {
            LimpiarHtmlControl('divKoDatosPersonales');
        }

        if (error3.length) {
            crearError(error3, '#divKoCondicionesUso', true);
        }
        else {
            LimpiarHtmlControl('divKoCondicionesUso');
        }

        return false;
    }
    else if (ultimoCheck == true) {
        // si no hay error y estamos comprobando el formulario por ultima vez (antes de enviarlo, por ejemplo)...
        return true;
    }
    else {
        LimpiarHtmlControl('divKodatosUsuario');
        LimpiarHtmlControl('divKoDatosPersonales');
        LimpiarHtmlControl('divKoCondicionesUso');
        funcionComprobar = funcionComprobar.replace("$$", "&" + $('#ctl00_CPH1_txtNombreUsuario').val() + "," + $('#ctl00_CPH1_txtCorreoE').val() + "," + $('#ctl00_CPH1_txtNombreCorto').val() + "," + $('#ctl00_CPH1_txtCaptcha').val());
        eval(funcionComprobar);
        //		// ... de otro modo ocultamos y mostramos las capas pertinentes y...
        //		$dC.fadeOut('', function() {
        //			$('#registroUsuario').find('div.ko').remove();
        //			$('#datosPersonales').fadeIn();
        //		});
        //		// ... evitamos el comportamiento normal del botoncico
        return false;
    }
}

function AgregarErrorReg(pErrores, pError) {
    if (pErrores.indexOf(pError) != -1)//El error ya está, no hay que agregarlo.
    {
        return '';
    }
    else {
        return pError;
    }
}

function ComprobarCampoRegUsuario(pCampo) {
    var error = '';
    var error2 = '';

    var panPintarError = '';

    if (pCampo == 'NombreUsu') {
        error = ValidarNombreUsu('ctl00_CPH1_txtNombreUsuario', 'ctl00_CPH1_lblNombreUsuario');
        panPintarError = 'divKoLogin';
    }
    else if (pCampo == 'Contra1') {
        error = ValidarContrasena('ctl00_CPH1_txtContrasenya', 'ctl00_CPH1_txtcContrasenya', 'ctl00_CPH1_lblContrasenya', 'ctl00_CPH1_lblConfirmarContrasenya', false);
        panPintarError = 'divKoContr';
    }
    else if (pCampo == 'Contra2') {
        error = ValidarContrasena('ctl00_CPH1_txtContrasenya', 'ctl00_CPH1_txtcContrasenya', 'ctl00_CPH1_lblContrasenya', 'ctl00_CPH1_lblConfirmarContrasenya', true);
        panPintarError = 'divKoContr';
    }
    else if (pCampo == 'Mail') {
        error = ValidarEmailIntroducido('ctl00_CPH1_txtCorreoE', 'ctl00_CPH1_lblEmail');
        panPintarError = 'divKoEmail';
    }
    else if (pCampo == 'NombrePersonal') {
        error = ValidarNombrePersona('ctl00_CPH1_txtNombrePersonal', 'ctl00_CPH1_lblNombre');
        panPintarError = 'divKoNombrePersonal';
    }
    else if (pCampo == 'Apellidos') {
        error = ValidarApellidos('ctl00_CPH1_txtApellidos', 'ctl00_CPH1_lblApellidos');
        panPintarError = 'divKiApellidos';
    }
    else if (pCampo == 'Provincia') {
        error2 = ValidarProvincia('ctl00_CPH1_txtProvincia', 'ctl00_CPH1_lblProvincia', 'ctl00_CPH1_editProvincia');
        panPintarError = 'divKoProvincia';
    }
    else if (pCampo == 'Sexo') {
        error2 = ValidarSexo('ctl00_CPH1_editSexo', 'ctl00_CPH1_lblSexo');
        panPintarError = 'divKoSexo';
    }
    else if (pCampo == 'CentroEstudios') {
        error2 = ValidarCentroEstudios('ctl00_CPH1_txtCentroEstudios', 'ctl00_CPH1_lbCentroEstudios');
        panPintarError = 'divKoCentroEstudios';
    }
    else if (pCampo == 'AreaEstudios') {
        error2 = ValidarAreaEstudios('ctl00_CPH1_txtAreaEstudios', 'ctl00_CPH1_lbAreaEstudios');
        panPintarError = 'divKoAreaEstudios';
    }

    if (error != '') {
        //crearError('<ul>'+error+'</ul>', '#ctl00_CPH1_datosUsuario');
        crearError(error, '#' + panPintarError);
    }
    else if (error2 == '') {
        LimpiarHtmlControl(panPintarError);
    }

    if (error2 != '') {
        //crearError('<ul>'+error2+'</ul>', '#fielDatosPersonales div.textBox:first');
        crearError(error2, '#' + panPintarError);
    }
    else if (error == '') {
        LimpiarHtmlControl(panPintarError);
    }
}

function ValidarNombreUsu(pTxtNombre, pLlbNombre) {
    var error = '';
    var RegExPatternUsuario = /^([a-zA-Z0-9-_.ñÑüÜáéíóúÁÉÍÓÚàèìòùÀÈÌÒÙâêîôûÂÊÎÔÛ'´`çÇ]{4,12})$/;

    // primero comprobamos los campos
    var longUsu = $('#' + pTxtNombre).val().length;

    if ((longUsu < 4) || (longUsu > 12)) {
        error += '<p>' + form.longitudUsuario + '</p>';
        $('#' + pLlbNombre).attr('class', 'ko');
    }
    else if (!$('#' + pTxtNombre).val().match(RegExPatternUsuario) || $('#' + pTxtNombre).val() == '') {
        error += '<p>' + form.formatoUsuario + '</p>';
        $('#' + pLlbNombre).attr('class', 'ko');
    }
    else {
        $('#' + pLlbNombre).attr('class', '');
    }

    return error;
}

/**
 * Validar la contrase�a del lusuario en el proceso de registro.
 * Si es correcto, se a�ade una clase "is-valid"
 * Si es incorrecto (vac�o) se a�ade una clase "is-invalid"
 * @param {any} pTxtContra
 * @param {any} pTxtContra2
 * @param {any} pLblContra
 * @param {any} pLblContra2
 * @param {any} pValidarContr2
 */
function ValidarContrasena(pTxtContra, pTxtContra2, pLblContra, pLblContra2, pValidarContr2) {
    var error = '';
    if ($('#' + pTxtContra).length > 0) {
        var RegExPatternPass = /(?!^[0-9]*$)(?!^[a-zA-ZñÑüÜ]*$)^([a-zA-ZñÑüÜ0-9#_$*]{6,12})$/;
        correcto = true;
        if (!$('#' + pTxtContra).val().match(RegExPatternPass)) {
            error += '<p>' + form.pwFormato + '</p>';
            correcto = false;
        }
        if (pValidarContr2 && $('#' + pTxtContra).val() != $('#' + pTxtContra2).val()) {
            error += '<p>' + form.pwIgual + '</p>';
            correcto = false;
        }
        if (correcto) {
            //$('#' + pLblContra).attr('class', '');
            $('#' + pTxtContra).addClass('is-valid');
            $('#' + pTxtContra).removeClass('is-invalid');

            if (pValidarContr2) {
                //$('#' + pLblContra2).attr('class', '');
                $('#' + pTxtContra2).addClass('is-valid');
                $('#' + pTxtContra2).removeClass('is-invalid');
            }
        } else {
            //$('#' + pLblContra).attr('class', 'ko');
            $('#' + pTxtContra).addClass('is-invalid');
            $('#' + pTxtContra).removeClass('is-valid');
            if (pValidarContr2) {
                //$('#' + pLblContra2).attr('class', 'ko');
                $('#' + pTxtContra2).addClass('is-invalid');
                $('#' + pTxtContra2).removeClass('is-valid');
            }
        }
    }
    return error;
}

/**
 * Validar el campo email introducido para el proceso de registro.
 * Si es correcto, se a�ade una clase "is-valid"
 * Si es incorrecto (vac�o) se a�ade una clase "is-invalid"
 * @param {any} pTxtMail: El input donde se ha rellenado con datos
 * @param {any} pLblMail: El label asociado a dicho input
 */
function ValidarEmailIntroducido(pTxtMail, pLblMail) {
    var error = '';

    if (!validarEmail($('#' + pTxtMail).val())) {
        error += '<p>' + form.emailValido + '</p>';
        //$('#' + pLblMail).attr('class', 'ko');
        $('#' + pTxtMail).addClass('is-invalid');
        $('#' + pTxtMail).removeClass('is-valid');
    } else {
        //$('#' + pLblMail).attr('class', '');
        $('#' + pTxtMail).addClass('is-valid');
        $('#' + pTxtMail).removeClass('is-invalid');
    }

    return error;
}

/**
 * Validar el nombre de la persona en el proceso de registro.
 * Si es correcto, se a�ade una clase "is-valid"
 * Si es incorrecto (vac�o) se a�ade una clase "is-invalid"
 * @param {any} pNombrePersona: El inputText
 * @param {any} pLblNombrePersona: El label relacionado correspondiente con el inputText
 */
function ValidarNombrePersona(pNombrePersona, pLblNombrePersona) {
    var error = '';

    if ($('#' + pNombrePersona).val().length == 0) {
        error += '<p>' + form.camposVacios + '</p>';
        // $('#' + pLblNombrePersona).attr('class', 'ko');
        $('#' + pNombrePersona).addClass('is-invalid');
        $('#' + pNombrePersona).removeClass('is-valid');
    } else {
        // $('#' + pLblNombrePersona).attr('class', '');        
        $('#' + pNombrePersona).addClass('is-valid');
        $('#' + pNombrePersona).removeClass('is-invalid');
    }

    //Comprobamos el formato de nombre de usuario
    var nombreUsuario = document.getElementById(pNombrePersona).value;
    var RegExPatternNombreUsuario = ExpresionRegularNombres;
    if (!nombreUsuario.match(RegExPatternNombreUsuario)) {
        error += '<p>' + form.formatoNombre + '</p>';
        $('#' + pLblNombrePersona).attr('class', 'ko');
    }

    if (nombreUsuario.match(ExpresionRegularCaracteresRepetidos)) {
        error += '<p>' + form.formatoNombreCaracteresRepetidos + '</p>';
        $('#' + pLblNombrePersona).attr('class', 'ko');
    }

    return error;
}

function ValidarFechaNacimiento(pDiaFechaNacimiento, pMesFechaNacimiento, pAnioFechaNacimiento, pLblFechaNacimiento) {
    var error = '';
    dia = document.getElementById(pDiaFechaNacimiento).value;
    mes = document.getElementById(pMesFechaNacimiento).value;
    anio = document.getElementById(pAnioFechaNacimiento).value;

    fecha = new Date(anio, mes, dia);
    hoy = new Date();

    mayor = false;

    if (dia > 0 && mes > 0 && anio > 0) {
        if ((hoy.getFullYear() - fecha.getFullYear()) > 18) {
            //Los ha cumplido en algún año anterior
            mayor = true;
        }
        else if ((hoy.getFullYear() - fecha.getFullYear()) == 18) {
            if (hoy.getMonth() > fecha.getMonth()) {
                //Los ha cumplido en algún mes anterior
                mayor = true;
            }
            //Los cumple durante el año en el que estamos
            else if (hoy.getMonth() == fecha.getMonth()) {
                //Los cumple durante el mes en el que estamos        
                if (hoy.getDate() >= fecha.getDate()) {
                    //Ya los ha cumplido
                    mayor = true;
                }
            }
        }
    }

    if (mayor) {
        document.getElementById(pLblFechaNacimiento).style.color = "";
    } else {
        error += '<p>' + form.fechanacincorrecta + '</p>';
        $('#' + pLblFechaNacimiento).attr('class', 'ko');
    }

    return error;
}

/**
 * Validar los apellidos de la persona en el proceso de registro.
 * Si es correcto, se a�ade una clase "is-valid"
 * Si es incorrecto (vac�o) se a�ade una clase "is-invalid"
 * @param {any} pTxtApellidos: El input de los apellidos
 * @param {any} pLblApellidos: El label asociado a los apellidos
 */
function ValidarApellidos(pTxtApellidos, pLblApellidos) {
    var error = '';

    if ($('#' + pTxtApellidos).val().length == 0) {
        error += '<p>' + form.camposVacios + '</p>';
        //$('#' + pLblApellidos).attr('class', 'ko');
        $('#' + pTxtApellidos).addClass('is-invalid');
        $('#' + pTxtApellidos).removeClass('is-valid');
    } else {
        //$('#' + pLblApellidos).attr('class', '');
        $('#' + pTxtApellidos).addClass('is-valid');
        $('#' + pTxtApellidos).removeClass('is-invalid');
    }

    //Comprobamos el formato de los apellidos del usuario
    var apellidosUsuario = document.getElementById(pTxtApellidos).value;
    //var RegExPatternApellidosUsuario = ExpresionRegularNombres;
    var RegExPatternApellidosUsuario = ExpresionRegularNombres;
    if (!apellidosUsuario.match(RegExPatternApellidosUsuario)) {
        error += '<p>' + form.formatoApellidos + '</p>';
        $('#' + pLblApellidos).attr('class', 'ko');
    }

    if (apellidosUsuario.match(ExpresionRegularCaracteresRepetidos)) {
        error += '<p>' + form.formatoApellidosCaracteresRepetidos + '</p>';
        $('#' + pLblApellidos).attr('class', 'ko');
    }

    return error;
}

function ValidarPais(pDDLPais, pLblEditPais) {
    var error = '';

    if ((document.getElementById(pDDLPais) != null) && (document.getElementById(pDDLPais).selectedIndex == 0)) {
        error += '<p>' + form.paisincorrecto + '</p>';
        $('#' + pLblEditPais).attr('class', 'ko');
    }
    else {
        $('#' + pLblEditPais).attr('class', '');
    }

    return error;
}

function ValidarProvincia(pTxtProvincia, pLblProvincia, pEditProvincia) {
    var error = '';

    if ($('#' + pTxtProvincia).val().length == 0 && document.getElementById(pTxtProvincia).style.display == '') {
        $('#' + pLblProvincia).attr('class', 'ko');
    } else {
        $('#' + pLblProvincia).attr('class', '');
    }

    //Comprobamos provincia
    if (document.getElementById(pTxtProvincia).style.display == "") {
        var provinciaUsuario = document.getElementById(pTxtProvincia).value;
        var RegExPatternprovinciaUsuario = /<|>$/;
        if (provinciaUsuario.match(RegExPatternprovinciaUsuario)) {
            error += '<p>' + form.formatoProvincia + '</p>';
            $('#' + pLblProvincia).attr('class', 'ko');
        }
    }

    if ((document.getElementById(pEditProvincia).style.display == '') && (document.getElementById(pEditProvincia).selectedIndex == 0)) {
        error += '<p>' + form.provinciaincorrecta + '</p>';
        $('#' + pLblProvincia).attr('class', 'ko');
    }
    if (($('#' + pTxtProvincia).val() == '') && (document.getElementById(pTxtProvincia).style.display == "")) {
        error += '<p>' + form.camposVacios + '</p>';
        $('#' + pLblProvincia).attr('class', 'ko');
    }

    return error;
}

function ValidarSexo(pNombrePersona, pLblNombrePersona) {
    var error = '';

    //Comprobamos el sexo
    if (document.getElementById(pNombrePersona).selectedIndex == 0) {
        error += '<p>' + form.sexoincorrecto + '</p>';
        $('#' + pLblNombrePersona).attr('class', 'ko');
    } else {
        $('#' + pLblNombrePersona).attr('class', '');
    }

    return error;
}

/**
 * Validar que el campo no está vacío (Ej: Proceso de registro, el campo "Cargo").
 * Si es correcto, se añade una clase "is-valid"
 * Si es incorrecto (vacío) se añade una clase "is-invalid"
* @param {any} pTxt: El inputText
 * @param {any} pLbl: El label relacionado correspondiente con el inputText
 * @param {bool} jqueryElement: Se indica si el elemento pasado est� ya en formato jquery, por lo que no hace falta analizarlo por ID
 */
function ValidarCampoNoVacio(pTxt, pLbl, jqueryElement = false) {
    var error = '';

    // Se ha pasado el elemento jquery, no el ID directamente
    if (jqueryElement == true) {
        if (pTxt.val() == '') {
            pTxt.addClass('is-invalid');
            error += '<p>' + form.camposVacios + '</p>';
        }
    } else {
        //Si es profesro comprobamos centro de estudios y area de estudios:
        if (document.getElementById(pTxt) != null && $('#' + pTxt).val() == '') {
            error += '<p>' + form.camposVacios + '</p>';
            //$('#' + pLbl).attr('class', 'ko');
            $('#' + pTxt).addClass('is-invalid');
            $('#' + pTxt).removeClass('is-valid');
        }
        else if (document.getElementById(pLbl) != null) {
            //$('#' + pLbl).attr('class', '');
            $('#' + pTxt).addClass('is-valid');
            $('#' + pTxt).removeClass('is-invalid');
        }
    }

    return error;
}

function ValidarCentroEstudios(pTxtCentro, pLblCentro) {
    var error = '';

    //Si es profesro comprobamos centro de estudios y area de estudios:
    if (document.getElementById(pTxtCentro) != null && $('#' + pTxtCentro).val() == '') {
        error += '<p>' + form.centroestudiosincorrecto + '</p>';
        $('#' + pLblCentro).attr('class', 'ko');
    }
    else if (document.getElementById(pLblCentro) != null) {
        $('#' + pLblCentro).attr('class', '');
    }

    return error;
}

function ValidarAreaEstudios(pTxtArea, pLblArea) {
    var error = '';

    if (document.getElementById(pTxtArea) != null && $('#' + pTxtArea).val() == '') {
        error += '<p>' + form.areaestudiosincorrecto + '</p>';
        $('#' + pLblArea).attr('class', 'ko');
    }
    else if (document.getElementById(pLblArea) != null) {
        $('#' + pLblArea).attr('class', '');
    }

    return error;
}

function fechaMayorQueLimite(pDiaSeleccionado, pMesSeleccionado, pAnioSeleccionado, pIDRelativoHacks) {
    var diaLimite = document.getElementById(pIDRelativoHacks + 'txtHackDiaLimite').value;
    var mesLimite = document.getElementById(pIDRelativoHacks + 'txtHackMesLimite').value;
    var anioLimite = document.getElementById(pIDRelativoHacks + 'txtHackAnioLimite').value;

    if (diaLimite.length < 2) {
        diaLimite = '0' + diaLimite;
    }

    if (mesLimite.length < 2) {
        mesLimite = '0' + mesLimite;
    }

    if (pDiaSeleccionado.length < 2) {
        pDiaSeleccionado = '0' + pDiaSeleccionado;
    }

    if (pMesSeleccionado.length < 2) {
        pMesSeleccionado = '0' + pMesSeleccionado;
    }

    if (pAnioSeleccionado > anioLimite) {
        return false;
    }
    else if (pAnioSeleccionado == anioLimite) {
        if (pMesSeleccionado > mesLimite) {
            return false;
        }
        else if (pMesSeleccionado == mesLimite) {
            if (pDiaSeleccionado > diaLimite) {
                return false;
            }
        }
    }

    return true;
}

function ComprobarFechaNac(pTxtDia, pTxtMes, pTxtAnio, pLblFechaNac, pLblEdadMinima, pInicioIDs) {
    var error = '';

    //Comprobamos la fecha
    var diaSeleccionado = document.getElementById(pTxtDia).selectedIndex;
    var mesSeleccionado = document.getElementById(pTxtMes).selectedIndex;
    var anioSeleccionado = document.getElementById(pTxtAnio).options[document.getElementById(pTxtAnio).selectedIndex].value;
    $('#' + pLblFechaNac).attr('class', '');
    if (!esFecha(diaSeleccionado, mesSeleccionado, anioSeleccionado)) {
        error += '<p>' + form.fechanacincorrecta + '</p>';
        $('#' + pLblFechaNac).attr('class', 'ko');
    }
    else if (!fechaMayorQueLimite(diaSeleccionado, mesSeleccionado, anioSeleccionado, pInicioIDs)) {
        //Si esta la fecha minima es un alumno, es decir 14 años la edad mínima
        if (document.getElementById(pLblEdadMinima) != null) {
            error += '<p>' + form.fechanacinsuficiente + '</p>';
        } else {
            error += '<p>' + form.fechanacinsuficiente18 + '</p>';
        }
    }

    return error;
}

function ValidarNombreCortoUsuario(nombre) {
    var RegExPatternNombreCorto = ExpresionNombreCorto;
    return (nombre.match(RegExPatternNombreCorto) && nombre != '');
}

function ValidarNombreCortoCentroYAsignatura(nombre) {
    var RegExPatternNombreCorto = ExpresionNombreCortoCentro;
    return (nombre.match(RegExPatternNombreCorto) && nombre != '');
}

function ValidarNombreCortoCurso(nombre) {
    var RegExPatternNombreCorto = /^([0-9]{1,10})$/;
    return (nombre.match(RegExPatternNombreCorto) && nombre != '');
}

function pasarSegundaParte() {
    $dC = $('#ctl00_CPH1_datosCuenta');
    $dC.fadeOut('', function () {
        //$('#registroUsuario').find('div.ko').remove();
        $('#ctl00_CPH1_datosPersonales').fadeIn();
    });
    $('#queEsEstoParte1').fadeOut('', function () {
        $('#queEsEstoParte2').fadeIn();
    });

}

function pasarPrimeraParte() {
    $dC = $('#ctl00_CPH1_datosPersonales');
    $dC.fadeOut('', function () {
        //document.getElementById('divErrores').className = '';
        $('#ctl00_CPH1_datosCuenta').fadeIn();
    });
    $('#queEsEstoParte2').fadeOut('', function () {
        $('#queEsEstoParte1').fadeIn();
    });
}

function registroUsuario_2(IdCaptcha, funcionComprobar) {
    if (!(document.getElementById('ctl00_CPH1_lblErrorFoto') == null || document.getElementById('ctl00_CPH1_lblErrorFoto').innerHTML == '')) {
        return false;
    }
    var $dP = $('#fielDatosPersonales'),
        error = '';
    // comprobamos el anterior fieldset, por si el usuario trastea por el DOM como si se creyese Tarzan
    //	if (!registroUsuario_1(true))
    //	{
    //		$('#registroUsuario button.anterior:eq(0)').click();
    //		return false;
    //	}
    // ahora comprobamos los campos personales
    $dP.find('label:contains(*)').each(function () {
        // todos aquellos label que contengan un asterisco:
        var $label = $(this);
        var $input = $label.next();
        if (!$input.val().length) {
            error += '<p>' + $label.text().replace('*', '') + form.obligatorio + '</p>';
        }
    });

    error += AgregarErrorReg(error, ValidarProvincia('ctl00_CPH1_txtProvincia', 'ctl00_CPH1_lblProvincia', 'ctl00_CPH1_editProvincia'));

    error += AgregarErrorReg(error, ValidarSexo('ctl00_CPH1_editSexo', 'ctl00_CPH1_lblSexo'));

    //    //Comprobamos provincia
    //    if(document.getElementById('ctl00_CPH1_txtProvincia').style.display==""){
    //	    var provinciaUsuario = document.getElementById('ctl00_CPH1_txtProvincia').value;
    //	    var RegExPatternprovinciaUsuario = /<|>$/;
    //	    if (provinciaUsuario.match(RegExPatternprovinciaUsuario))
    //	    {
    //		    error += '<li>'+form.formatoProvincia+'</li>';
    //		    document.getElementById('ctl00_CPH1_lblProvincia').style.color="red";
    //	    }
    //	}
    //	//Comprobamos codigopostal
    //	var CPUsuario = document.getElementById('ctl00_CPH1_txtCodigoPost').value;
    //	var RegExPatternCPUsuario = /<|>$/;
    //	if (CPUsuario.match(RegExPatternCPUsuario))
    //	{
    //		error += '<li>'+form.formatoCP+'</li>';
    //		document.getElementById('ctl00_CPH1_lblCP').style.color="red";
    //	}
    //	//Comprobamos localidad
    //	var localidadUsuario = document.getElementById('ctl00_CPH1_txtPoblacion').value;
    //	var RegExPatternlocalidadUsuario = /<|>$/;
    //	if (localidadUsuario.match(RegExPatternlocalidadUsuario))
    //	{
    //		error += '<li>'+form.formatoLocalidad+'</li>';
    //		document.getElementById('ctl00_CPH1_lblPoblacion').style.color="red";
    //	}

    error += AgregarErrorReg(error, ValidarCentroEstudios('ctl00_CPH1_txtCentroEstudios', 'ctl00_CPH1_lbCentroEstudios'));

    error += AgregarErrorReg(error, ValidarAreaEstudios('ctl00_CPH1_txtAreaEstudios', 'ctl00_CPH1_lbAreaEstudios'));

    error += AgregarErrorReg(error, ComprobarFechaNac('ctl00_CPH1_editDia', 'ctl00_CPH1_editMes', 'ctl00_CPH1_editAnio', 'ctl00_CPH1_lblFechaNac', 'ctl00_CPH1_lblEdadMinima', 'ctl00_CPH1_'));

    if (error.length) {
        crearError(error, '#fielDatosPersonales div.textBox:first', true);
        $('body, html').animate({ scrollTop: $dP.offset().top }, 600);
        return false;
    }
    else {
        IdCaptcha = '#' + IdCaptcha;
        funcionComprobar = funcionComprobar.replace("$$", "&" + $(IdCaptcha).val());
        eval(funcionComprobar);
    }
}

function registroProfesor(funcionComprobar) {
    var error = '';

    // primero comprobamos los campos
    if (!validarEmail($('#ctl00_CPH1_txtEmail').val())) {
        error += '<p>' + form.emailValido + '</p>';
    }

    if ($('#ctl00_CPH1_txtCentroEstudios').val() == '') {
        error += '<p>' + form.centroestudiosincorrecto + '</p>';
    }

    if ($('#ctl00_CPH1_txtAreaEstudios').val() == '') {
        error += '<p>' + form.areaestudiosincorrecto + '</p>';
    }

    if (error.length) {
        crearError(error, '#ctl00_CPH1_datosCuenta div.reg', true);
        return false;
    } else {
        funcionComprobar = funcionComprobar.replace("$$", "&" + $('#ctl00_CPH1_txtEmail').val() + "," + $('#ctl00_CPH1_txtCentroEstudios').val() + "," + $('#ctl00_CPH1_txtAreaEstudios').val());
        eval(funcionComprobar);
        //		// ... de otro modo ocultamos y mostramos las capas pertinentes y...
        //		$dC.fadeOut('', function() {
        //			$('#registroUsuario').find('div.ko').remove();
        //			$('#datosPersonales').fadeIn();
        //		});
        //		// ... evitamos el comportamiento normal del botoncico
        return false;
    }
}

// asignamos los eventos
$(function () {
    //$('#registroUsuario #datosCuenta button.siguiente').click(registroUsuario_1);
    $('#registroUsuario').submit(function () {
        return registroUsuario_2();
    });
    // boton anterior
    $('#wrap').filter('.registro').find('button.anterior').click(function () {
        var $this = $(this);
        $this.parents('form').find('fieldset:visible').fadeOut('', function () {
            $this.parents('fieldset').prev('fieldset').fadeIn();
        });
        return false;
    });
});

function irAnteriorUsu() {
    $('#ctl00_CPH1_datosPersonales').fadeOut('', function () {
        $('#ctl00_CPH1_datosCuenta').fadeIn();
    });
    $('#queEsEstoParte2').fadeOut('', function () {
        $('#queEsEstoParte1').fadeIn();
    });

    return false;
}

//                                                                 Agregar datos de contacto
//------------------------------------------------------------------------------------------

function crearDatosContacto() {
    var $dP = $('#fielDatosPersonales'),
        error = '';

    // ahora comprobamos los campos personales
    $dP.find('label:contains(*)').each(function () {
        // todos aquellos label que contengan un asterisco:
        var $label = $(this);
        var $input = $label.next();
        if (!$input.val().length) {
            error += '<p>' + $label.text().replace('*', '') + form.obligatorio + '</p>';
        }
    });

    //Comprobamos el formato de nombre de usuario
    var nombreUsuario = document.getElementById('ctl00_CPH1_txtNombrePersonal').value;
    var RegExPatternNombreUsuario = ExpresionRegularNombres;
    if (!nombreUsuario.match(RegExPatternNombreUsuario)) {
        error += '<p>' + form.formatoNombre + '</p>';
    }

    //Comprobamos el formato de los apellidos del usuario
    var apellidosUsuario = document.getElementById('ctl00_CPH1_txtApellidos').value;
    var RegExPatternApellidosUsuario = ExpresionRegularNombres;
    if (!apellidosUsuario.match(RegExPatternApellidosUsuario)) {
        error += '<p>' + form.formatoApellidos + '</p>';
    }

    if (document.getElementById('ctl00_CPH1_txtEmail').value != "") {
        if (!validarEmail(document.getElementById('ctl00_CPH1_txtEmail').value)) {
            error += '<p>' + form.emailValido + '</p>';
        }
    }

    if (error.length) {
        crearError(error, '#fielDatosPersonales div.textBox:first', true);
        $('body, html').animate({ scrollTop: $dP.offset().top }, 600);
        return false;
    }
    else {
        eval(document.getElementById('ctl00_CPH1_btnHackCrearDatos').href);
    }
}

//                                                                             Suscripciones
//------------------------------------------------------------------------------------------

function confirmarSuscripcion(control, accion, nombre) {
    var $this = $('#' + control);
    var texto = dialogo.confirmarSuscripcion.replace("@1@", nombre);
    mascaraCancelar(texto, $this, accion);
}

function eliminarSuscripcion(control, accion, nombre) {
    var $this = $('#' + control);
    var texto = dialogo.eliminarSuscripcion.replace("@1@", nombre);
    mascaraCancelar(texto, $this, accion);
}
function confirmarSuscripcionBlog(control, accion, nombre) {
    var $this = $('#' + control);
    var texto = dialogo.confirmarSuscripcionBlog.replace("@1@", nombre);
    mascaraCancelar(texto, $this, accion);
}

function eliminarSuscripcionBlog(control, accion, nombre) {
    var $this = $('#' + control);
    var texto = dialogo.eliminarSuscripcionBlog.replace("@1@", nombre);
    mascaraCancelar(texto, $this, accion);
}

//                                                                    listado de comunidades
//------------------------------------------------------------------------------------------
$(function () { // ejemplo de borrado de comunidad
    $('#wrap').filter('.comunidades').find('ul.imagenAsociada a.cancelarSuscr').click(function () {
        var $this = $(this);
        var $li = $this.parents('li').eq(0);
        mascaraCancelar(borr.suscripcion, $li.get(0), function () {
            alert('Aqui iria algun tipo de funcion para borrar "' + $li.find('h3').text() + '" de la base de datos o de donde se crea conveniente.');
        });
    });
});

//                                                                          listado de blogs
//------------------------------------------------------------------------------------------
$(function () { // ejemplo de borrado de suscripcion
    $('#wrap').filter('.blogs').find('ul.imagenAsociada a.cancelarSuscr').click(function () {
        var $this = $(this);
        var $li = $this.parents('li').eq(0);
        mascaraCancelar(borr.suscripcion, $li.get(0), function () {
            alert('Aqui iria algun tipo de funcion para borrar "' + $li.find('h3').text() + '" de la base de datos o de donde se crea conveniente.');
        });
    });
});

//                                                                     listado de borradores
//------------------------------------------------------------------------------------------
/*$( function() { // ejemplo de borrado de borrador
$('#wrap').filter('.borradores').find('ul.imagenAsociada a.cancelar').click( function() {
var $this = $(this); 
var $li = $this.parents('li').eq(0);
mascaraCancelar(borr.borrador, $li.get(0), function() {
alert('Aqui iria algun tipo de funcion para borrar "'+$li.find('h3').text()+'" de la base de datos o de donde se crea conveniente.');
});
});
});*/

function borrarBorrador(control, accion) {
    var $this = $(control);
    var $li = $this.parents('li').eq(0);
    mascaraCancelar(borr.borrador, $li.get(0), accion);
}
//                                                                 listado de suscripciones
//------------------------------------------------------------------------------------------
/*$( function() { // ejemplo de borrado de suscripcion
$('#wrap').filter('.suscripciones').find('ul.imagenAsociada a.cancelarSuscr').click( function() {
var $this = $(this); 
var $li = $this.parents('li').eq(0);
mascaraCancelar(borr.suscripcion, $li.get(0), function() {
alert('Aqui iria algun tipo de funcion para borrar "'+$li.find('h3').text()+'" de la base de datos o de donde se crea conveniente.');
});
});
});
*/
function borrarSuscripcion(control, accion) {
    var $this = $(control);
    var $li = $this.parents('li').eq(0);
    mascaraCancelar(borr.suscripcion, $li.get(0), accion);
}
//                                                                   listado de comentarios
//------------------------------------------------------------------------------------------

/* 
$( function() { // ejemplo de borrado de comentarios
$('#wrap').filter('.comentarios').find('ul.imagenAsociada a.cancelar').click( function() {
var $this = $(this); 
var $li = $this.parents('li').eq(0);
mascaraCancelar(borr.comentario, $li.get(0), function() {
alert('Aqui iria algun tipo de funcion para borrar "'+$li.find('h3').text()+'" de la base de datos o de donde se crea conveniente.');
});
});
});
*/
function borrarComentario(control, accion) { // ejemplo de borrado de comentarios
    var $this = $(control);
    var $li = $this.parents('li').eq(0);
    mascaraCancelar(borr.comentario, $li.get(0), accion);
}

//function AccionFichaPersona(texto,id,accion) { 
//		var $li = $(document.getElementById(id)); 
//		mascaraCancelar(texto, $li, accion);
//}


/**
 * Acci�n que se ejecuta cuando se pulsa sobre las acciones disponibles de un item/recurso de tipo "Perfil" encontrado por el buscador.
 * Las acciones que se podr�an realizar son (No/Enviar newsletter, No/Bloquear). Acciones tambi�n de vincular, desvincular recurso...
 * @param {string} titulo: T�tulo que tendr� el panel modal
 * @param {any} textoBotonPrimario: Texto del bot�n primario
 * @param {any} textoBotonSecundario: Texto del bot�n primario
 * @param {string} texto: El texto o mensaje a modo de t�tulo que se mostrar� para que el usuario sepa la acci�n que se va a realizar
 * @param {string} id: Identificador del recurso/persona sobre el que se aplicar� la acci�n
 * @param {any} accion: Acci�n o funci�n que se ejecutar� cuando se pulse en el bot�n de primario
 * @param {any} idModalPanel: Panel modal contenedor donde se insertar� este HTML (Por defecto ser� #modal-container)
 */
function AccionFichaPerfil(titulo, textoBotonPrimario, textoBotonSecundario, texto, id, accion, textoInferior = null, idModalPanel = "#modal-container") {

    // Panel din�mico del modal padre donde se insertar� la vista "hija"
    const $modalDinamicContentPanel = $('#modal-container').find('#modal-dinamic-content #content');

    // Plantilla del panel html que se cargar� en el modal contenedor al pulsar en la acci�n
    var plantillaPanelHtml = '';
    // Cabecera del panel
    plantillaPanelHtml += '<div class="modal-header">';
    plantillaPanelHtml += '<p class="modal-title"><span class="material-icons">label</span>' + titulo + '</p>';
    plantillaPanelHtml += '<span class="material-icons cerrar" data-dismiss="modal" aria-label="Close">close</span>';
    plantillaPanelHtml += '</div>';
    // Cuerpo del panel
    plantillaPanelHtml += '<div class="modal-body">';
    plantillaPanelHtml += '<div class="formulario-edicion">';
    plantillaPanelHtml += '<div class="form-group">';
    plantillaPanelHtml += '<p>' + texto + '</p>';
    plantillaPanelHtml += '</div>';
    if (textoInferior != undefined) {
        if (textoInferior.length > 5) {
            plantillaPanelHtml += '<div class="form-group">';
            plantillaPanelHtml += '<label class="control-label">' + textoInferior + '</label>';
            plantillaPanelHtml += '</div>';
        }
    }
    plantillaPanelHtml += '<div id="modal-dinamic-action-response">';
    // Posibles mensajes de info
    plantillaPanelHtml += '<div>';
    plantillaPanelHtml += '<div id="menssages">';
    plantillaPanelHtml += '<div class="ok"></div>';
    plantillaPanelHtml += '<div class="ko"></div>';
    plantillaPanelHtml += '</div>';
    plantillaPanelHtml += '</div>';
    // Panel de botones para la acci�n
    plantillaPanelHtml += '<div id="modal-dinamic-action-buttons" class="form-actions">'
    plantillaPanelHtml += '<button data-dismiss="modal" class="btn btn-primary">' + textoBotonSecundario + '</button>'
    plantillaPanelHtml += '<button class="btn btn-outline-primary ml-1">' + textoBotonPrimario + '</button>'
    plantillaPanelHtml += '</div>';
    plantillaPanelHtml += '</div>';
    plantillaPanelHtml += '</div>';

    // Meter el c�digo de la vista modal en el contenedor padre que viene identificado por el id #modal-container
    // En concreto, hay que buscar la etiqueta modal-dinamic-content #content e insertar el c�digo
    $modalDinamicContentPanel.html(plantillaPanelHtml);

    // Acceso a los botones
    const botones = $modalDinamicContentPanel.find('#modal-dinamic-action-buttons > button');

    // Asignaci�n de la funci�n al bot�n "S�" o de acci�n
    $(botones[1]).on("click", function () {
        // Ocultar el panel modal de bootstrap - De momento estar� visible. Se ocultar�a si se muestra mensaje de OK pasados 1.5 segundos
        //$('#modal-container').modal('hide');
    }).click(accion);
}

function AccionCrearComentario(clientID, id, pCKECompleto) {
    var $c = $(document.getElementById("nuevo_" + id));

    document.getElementById("nuevo_" + id).className = 'comment-content';


    var $anterior = $c.children();

    if ($anterior.length > 0) {
        //Eliminar el anterior
        $anterior.remove();
    }

    var accion = "javascript:CrearComentario('" + clientID + "', '" + id + "');";
    var claseCK = 'cke comentarios';

    if (pCKECompleto) {
        claseCK = 'cke recursos'
    }

    var $confirmar = $(['<fieldset class="mediumLabels"><legend>', comentarios.publicarcomentario, '</legend><p><textarea class="' + claseCK + '" id="txtComentario_', id, '" rows="2" cols="20"></textarea><p><label class="error" id="error_', id, '"></label></p><input type="button" onclick="', accion, '" value="', comentarios.enviar, '" class="text medium"></p></fieldset>'].join(''));
    $confirmar.prependTo($c)
        .find('button').click(function () { // Ambos botones hacen desaparecer la mascara
            $c.parents('.stateShowForm').css({ display: 'none' });
            $confirmar.css({ display: 'none' });
        }).eq(0).click(accion); // pero solo el primero activa la funcionConfirmada	

    RecargarTodosCKEditor();
}

function CrearComentario(clientID, id) {
    if ($('#txtComentario_' + id).val() != '') {
        var descripcion = $('#txtComentario_' + id).val().replace(/\|/g, '[-@@@1-]').replace(/,/g, '[-@@@2-]');
        MostrarUpdateProgress();

        var target = clientID.replace(/_/gi, "$");

        WebForm_DoCallback(target, 'ListadoComentarios_Editar|' + clientID + ',' + id + ',' + 'AgregarComentario' + ',' + document.getElementById('txtHackListDocDeComent').value + ',' + document.getElementById('txtHackMostrarSoloProyComent').value + ',' + descripcion, ReceiveServerData, '', null, false);
    }
    else {
        $('#error_' + id).html(comentarios.comentarioError);
    }
}

function AccionResponderComentario(clientID, id, pCKECompleto) {
    var $c = $(document.getElementById("respuesta_" + id));

    document.getElementById("respuesta_" + id).className = 'comment-content';

    document.getElementById(id).style.display = 'block';


    var $anterior = $c.children();

    if ($anterior.length > 0) {
        //Eliminar el anterior
        $anterior.remove();
    }

    var accion = "javascript:ResponderComentario('" + clientID + "', '" + id + "');";
    var claseCK = 'cke comentarios';

    if (pCKECompleto) {
        claseCK = 'cke recursos'
    }

    var $confirmar = $(['<fieldset class="mediumLabels"><legend>', comentarios.responderComentario, '</legend><p><textarea class="' + claseCK + '" id="txtComentario_', id, '" rows="2" cols="20"></textarea><p><label class="error" id="error_', id, '"></label></p><input type="button" onclick="', accion, '" value="', comentarios.enviar, '" class="text medium"></p></fieldset>'].join(''));
    $confirmar.prependTo($c)
        .find('button').click(function () { // Ambos botones hacen desaparecer la mascara
            $c.parents('.stateShowForm').css({ display: 'none' });
            $confirmar.css({ display: 'none' });
        }).eq(0).click(accion); // pero solo el primero activa la funcionConfirmada	

    RecargarTodosCKEditor();
}

function ResponderComentario(clientID, id) {
    if ($('#txtComentario_' + id).val() != '') {
        var descripcion = $('#txtComentario_' + id).val().replace(/\|/g, '[-@@@1-]').replace(/,/g, '[-@@@2-]');
        MostrarUpdateProgress();

        var target = clientID.replace(/_/gi, "$");

        WebForm_DoCallback(target, 'ListadoComentarios_Editar|' + clientID + ',' + id + ',' + 'guardarRespuestaComentario' + ',' + document.getElementById('txtHackListDocDeComent').value + ',' + document.getElementById('txtHackMostrarSoloProyComent').value + ',' + descripcion, ReceiveServerData, '', null, false);
    }
    else {
        $('#error_' + id).html(comentarios.comentarioError);
    }
}

function AccionEditarComentario(clientID, id, pCKECompleto) {
    var $c = $(document.getElementById("respuesta_" + id));


    var mensajeAntiguo = document.getElementById(id).innerHTML;

    if (mensajeAntiguo.indexOf('<ul class="principal">') > -1) {
        // Si encuentra el elemento principal, es Inevery y tenemos la lista de responder, eliminar.... etc....
        mensajeAntiguo = mensajeAntiguo.substr(0, mensajeAntiguo.indexOf('<ul class="principal">'));
    }

    document.getElementById(id).style.display = 'none';

    document.getElementById("respuesta_" + id).className = 'comment-content';

    var $anterior = $c.children();

    if ($anterior.length > 0) {
        //Eliminar el anterior
        $anterior.remove();
    }

    var accion = "javascript:EditarComentario('" + clientID + "', '" + id + "');";
    var claseCK = 'cke comentarios';

    if (pCKECompleto) {
        claseCK = 'cke recursos'
    }

    var $confirmar = $(['<fieldset class="mediumLabels"><legend>', comentarios.editarComentario, '</legend><p><textarea class="' + claseCK + '" id="txtComentario_', id, '" rows="2" cols="20">' + mensajeAntiguo + '</textarea><p><label class="error" id="error_', id, '"></label></p><input type="button" onclick="', accion, '" value="', comentarios.guardar, '" class="text medium"></p></fieldset>'].join(''));
    $confirmar.prependTo($c)
        .find('button').click(function () { // Ambos botones hacen desaparecer la mascara
            $c.parents('.stateShowForm').css({ display: 'none' });
            $confirmar.css({ display: 'none' });
        }).eq(0).click(accion); // pero solo el primero activa la funcionConfirmada	

    RecargarTodosCKEditor();
}

function EditarComentario(clientID, id) {
    if ($('#txtComentario_' + id).val() != '') {
        var descripcion = $('#txtComentario_' + id).val().replace(/\|/g, '[-@@@1-]').replace(/,/g, '[-@@@2-]');
        MostrarUpdateProgress();

        var target = clientID.replace(/_/gi, "$");
        WebForm_DoCallback(target, 'ListadoComentarios_Editar|' + clientID + ',' + id + ',' + 'guardarComentario' + ',' + document.getElementById('txtHackListDocDeComent').value + ',' + document.getElementById('txtHackMostrarSoloProyComent').value + ',' + descripcion, ReceiveServerData, '', null, false);
    }
    else {
        $('#error_' + id).html(comentarios.comentarioError);
    }
}

function AccionEnviarMensajeGrupo(clientID, id) {

    var $c = $(document.getElementById(id));

    if (CKEDITOR.instances["txtDescripcion_" + id] != null) {
        CKEDITOR.instances["txtDescripcion_" + id].destroy();
    }

    var $anterior = $c.children();

    if ($anterior.length > 0) {
        //Eliminar el anterior
        $anterior.remove();
    }

    var accion = "javascript:EnviarMensajeGrupo('" + clientID + "', '" + id + "');";

    var $confirmar = $(['<fieldset class="mediumLabels"><legend>', mensajes.enviarMensaje, '</legend><p><label for="txtAsunto_', id, '">', mensajes.asunto, '</label><input type="text" id="txtAsunto_', id, '" class="text big"></p><p><label for="txtDescripcion_', id, '">', mensajes.descripcion, '</label></p><p><textarea class="cke mensajes" id="txtDescripcion_', id, '" rows="2" cols="20"></textarea></p><p><label class="error" id="error_', id, '"></label></p><input type="button" onclick="', accion, '" value="', mensajes.enviar, '" class="text medium"></p></fieldset>'].join(''));
    //$confirmar.find('div').filter('.pregunta').fadeIn().end().filter('.mascara').show().fadeTo(600, 0.8);
    // Incrustamos el elemento a la vez que lo mostramos y definimos los eventos: 
    $confirmar.prependTo($c)
        .find('button').click(function () { // Ambos botones hacen desaparecer la mascara
            $c.parents('.stateShowForm').css({ display: 'none' });
            $confirmar.css({ display: 'none' });
        }).eq(0).click(accion); // pero solo el primero activa la funcionConfirmada

    MostrarPanelAccionDesp(clientID + "_desplegable_" + id, null);
    RecargarTodosCKEditor();
}


function AccionEnviarMensaje(clientID, id) {
    var $c = $(document.getElementById(id));

    if (CKEDITOR.instances["txtDescripcion_" + id] != null) {
        CKEDITOR.instances["txtDescripcion_" + id].destroy();
    }

    var $anterior = $c.children();

    if ($anterior.length > 0) {
        //Eliminar el anterior
        $anterior.remove();
    }

    var accion = "javascript:EnviarMensaje('" + clientID + "', '" + id + "');";

    var $confirmar = $(['<fieldset class="mediumLabels"><legend>', mensajes.enviarMensaje, '</legend><p><label for="txtAsunto_', id, '">', mensajes.asunto, '</label><input type="text" id="txtAsunto_', id, '" class="text big"></p><p><label for="txtDescripcion_', id, '">', mensajes.descripcion, '</label></p><p><textarea class="cke mensajes" id="txtDescripcion_', id, '" rows="2" cols="20"></textarea></p><p><label class="error" id="error_', id, '"></label></p><input type="button" onclick="', accion, '" value="', mensajes.enviar, '" class="text medium"></p></fieldset>'].join(''));
    //$confirmar.find('div').filter('.pregunta').fadeIn().end().filter('.mascara').show().fadeTo(600, 0.8);
    // Incrustamos el elemento a la vez que lo mostramos y definimos los eventos: 
    $confirmar.prependTo($c)
        .find('button').click(function () { // Ambos botones hacen desaparecer la mascara
            $c.parents('.stateShowForm').css({ display: 'none' });
            $confirmar.css({ display: 'none' });
        }).eq(0).click(accion); // pero solo el primero activa la funcionConfirmada

    if ($('#divContMensajesPerf').length > 0 && $('#divContMensajesPerf').html() == '') {
        $('#divContMensajesPerf').html($('#' + clientID + "_desplegable_" + id).parent().html());
        $('#' + clientID + "_desplegable_" + id).parent().html('');
    }

    MostrarPanelAccionDesp(clientID + "_desplegable_" + id, null);
    RecargarTodosCKEditor();
}


function AccionRechazarDesplegandoMensaje(clientID, textoRechazarConMensaje, textoRechazarSinMensaje) {
    //var $c = $(document.getElementById(clientID));

    if (CKEDITOR.instances["txtDescripcion_"] != null) {
        CKEDITOR.instances["txtDescripcion_"].destroy();
    }

    var id = "_rechazado";
    var accionSinMensaje = "javascript:RechazarSinMensaje();";
    var accionConMensaje = "javascript:RechazarConMensaje('" + clientID + "', '" + id + "');";

    var html = '<fieldset class="mediumLabels"><legend>' + mensajes.enviarMensaje + '</legend><p><label for="txtDescripcion' + id + '">' + mensajes.descripcion + '</label></p><p><textarea class="cke mensajes" id="txtDescripcion' + id + '" rows="2" cols="20"></textarea></p><p><label class="error" id="error' + id + '"></label></p><input type="button" onclick="' + accionConMensaje + '" value="' + textoRechazarConMensaje + '" class="text medium"><input type="button" onclick="' + accionSinMensaje + '" value="' + textoRechazarSinMensaje + '" class="text medium"></p></fieldset>';

    MostrarPanelAccionDesp(clientID, html);
    RecargarTodosCKEditor();
}

function RechazarSinMensaje() {
    WebForm_DoCallback('__Page', 'rechazarsinmensaje', ReceiveServerData, '', null, false);
}

function RechazarConMensaje(clientID, id) {
    if ($('#txtAsunto' + id).val() != '' && $('#txtDescripcion' + id).val() != '') {
        var descripcion = $('#txtDescripcion' + id).val().replace(/&/g, '[-|-]');
        MostrarUpdateProgress();
        WebForm_DoCallback('__Page', 'rechazarconmensaje&' + descripcion, ReceiveServerData, '', null, false);
    }
    else {
        $('#error' + id).html(mensajes.mensajeError);
    }
}


function EnviarMensajeGrupo(clientID, id) {
    if ($('#txtAsunto_' + id).val() != '' && $('#txtDescripcion_' + id).val() != '') {
        var asunto = $('#txtAsunto_' + id).val().replace(/&/g, '[-|-]');
        var descripcion = $('#txtDescripcion_' + id).val().replace(/&/g, '[-|-]');
        MostrarUpdateProgress();
        WebForm_DoCallback('__Page', 'FichaGrupo_EnviarMensaje&' + id + '&' + asunto + '&' + descripcion + '&' + clientID, ReceiveServerData, '', null, false);
        //AceptarPanelAccion(clientID + "_desplegable_" + id, true, mensajes.mensajeEnviado);
    }
    else {
        $('#error_' + id).html(mensajes.mensajeError);
    }
}

function AccionEditarNombreGrupo(texto, grupoId, textoInferior) {

    var $c = $(document.getElementById(grupoId));

    var $anterior = $c.children();

    if ($anterior.length > 0) {
        //Eliminar el anterior
        $anterior.remove();
    }

    var accion = "javascript:EditarNombreGrupo('" + grupoId + "');";

    var $confirmar = $(['<div><p>', texto, '</p><br><p class="small"><br>', textoInferior, '</p><br><input type="text" id=txtNombreGrupo_' + grupoId + '></input><input type="button" value="Aceptar" onclick="' + accion + '" class="btMini"></input></div>'].join(''));
    //$confirmar.find('div').filter('.pregunta').fadeIn().end().filter('.mascara').show().fadeTo(600, 0.8);
    // Incrustamos el elemento a la vez que lo mostramos y definimos los eventos: 
    $confirmar.prependTo($c)
        .find('button').click(function () { // Ambos botones hacen desaparecer la mascara
            $c.parents('.stateShowForm').css({ display: 'none' });
            $confirmar.css({ display: 'none' });
        }).eq(0).click(accion); // pero solo el primero activa la funcionConfirmada
}

function EditarNombreGrupo(grupoId) {

    var nombreGrupo = $('#txtNombreGrupo_' + grupoId).val().replace(/&/g, '[-|-]');

    contenedorNombre = document.getElementById('divNombre_' + grupoId);
    contenedorNombre.childNodes[0].textContent = nombreGrupo;

    CerrarPanelAccion('ListadoGenerico_controles_fichagrupo_ascx_desplegable_' + grupoId);
    WebForm_DoCallback('__Page', 'FichaGrupo_EditarNombreGrupo&' + grupoId + '&' + nombreGrupo + '&', null, '', null, false);
    //    AceptarPanelAccion("_desplegable_" + id, true, 'Grupo Editado');
}

function AccionEditarGrupo(grupoId) {

    WebForm_DoCallback('__Page', 'AgregarContactoAGrupo&' + grupoId + '&', ReceiveServerData, '', null, false);
}

function AccionFichaGrupo(texto, grupoId, accion, textoInferior) {
    var $c = $(document.getElementById(grupoId));

    var $anterior = $c.children();

    if ($anterior.length > 0) {
        //Eliminar el anterior
        $anterior.remove();
    }


    var $confirmar = $(['<div><p>', texto, '</p><br><p class="small"><br>', textoInferior, '</p><br><button onclick="return false;" class="btMini">', borr.si, '</button><button onclick="return false;" class="btMini">', borr.no, '</button></div>'].join(''));

    $confirmar.prependTo($c)
        .find('button').click(function () { // Ambos botones hacen desaparecer la mascara
            $c.parents('.stateShowForm').css({ display: 'none' });
            $confirmar.css({ display: 'none' });
        }).eq(0).click(accion); // pero solo el primero activa la funcionConfirmada
}

function EnviarMensaje(clientID, id) {
    if ($('#txtAsunto_' + id).val() != '' && $('#txtDescripcion_' + id).val() != '') {
        var asunto = $('#txtAsunto_' + id).val().replace(/&/g, '[-|-]');
        var descripcion = $('#txtDescripcion_' + id).val().replace(/&/g, '[-|-]');
        MostrarUpdateProgress();
        WebForm_DoCallback('__Page', 'FichaPerfil_EnviarMensaje&' + id + '&' + asunto + '&' + descripcion + '&' + clientID, ReceiveServerData, '', null, false);
        //AceptarPanelAccion(clientID + "_desplegable_" + id, true, mensajes.mensajeEnviado);
    }
    else {
        $('#error_' + id).html(mensajes.mensajeError);
    }
}

function AccionFichaPersona(texto, id, accion, textoInferior) {
    var $li = $(document.getElementById(id));
    mascaraCancelar2(texto, $li, accion, textoInferior);
}

function AccionAlerta(texto, id) {
    var $li = $(document.getElementById(id));
    mascaraCancelar(texto, $li);
}

function MostrarPopUp(texto, control, accion) {
    var $this = $(control);
    var $li = $this.parents('li').eq(0);
    mascaraCancelar(texto, $li.get(0), accion);
}

function aceptarInvitacion(control, accion) {
    var $this = $(control);
    var $li = $this.parents('li').eq(0);
    mascaraCancelar(invitaciones.aceptar, $li.get(0), accion);
}

function ignorarInvitacion(control, accion) {
    var $this = $(control);
    var $li = $this.parents('li').eq(0);
    mascaraCancelar(invitaciones.ignorar, $li.get(0), accion);
}


//                                                      funciones asociadas a base recursos
//------------------------------------------------------------------------------------------
//$( function() {
//	$('#selectoresBase img.ascendente').click(function(){
//		if ('img/onUp.gif' == this.src) return false;
//		this.src = 'img/onUp.gif'; 
//		$('#selectoresBase img.descendente').attr('src', 'img/offDown.gif');
//		alert('Recargar el listado?');
//		return false;
//	});
//	$('#selectoresBase img.descendente').click(function(){
//		if ('img/onDown.gif' == this.src) return false;
//		this.src = 'img/onDown.gif'; 
//		$('#selectoresBase img.ascendente').attr('src', 'img/offUp.gif');
//		alert('Recargar el listado?');
//		return false;
//	});

//	$('#baseRecursos a:contains(Aadir recurso a categora), button:contains(Editar Categorias), #anyadirACategoria, #anyadirEditores').click( function() {
//		// meto esto aqui porque imagino que se tendra que llamar por Ajax al listado,
//		// bastar con inyectarlo como ultimo elemento del <body/>.
//		// Ademas, podeis darle un identificador al <a> para no tener que buscarlo por
//		// el DOM usando su contenido =)
//		var $capa = $('#capaModal');
//		$capa.find('div.mascara').height($(document).height());
//		$capa.find('form.anyadirCategorias').css('top', ($('html').attr('scrollTop') || $('body').attr('scrollTop') || 0) + 'px')
//		$capa.fadeIn();
//		// una vez llamado deberian prepararse los eventos
//		$capa.find('a.icoEliminar').unbind('click').click(function(){
//			$capa.fadeOut();
//		});
//		return false;
//	});

//	$('#baseRecursos a:contains(Crear categora)').click(function() {
//		// id. que anterior
//		$('#baseRecursos div.ko').remove()
//		$('#editarCategoria').slideUp();
//		$('#crearCategoria').slideDown().find('button[type=reset]').unbind('click').click(function(){
//			$('#crearCategoria').slideUp();
//		});
//		return false;
//	});
//	
//	$('#baseRecursos a:contains(Editar)').click(function() {
//		// id. que anterior
//		var $a = $('#baseRecursos div.listadoCategorias ul:gt(0) a.activo');
//		if(!$a.length){
//			crearError('<p>'+baseRec.noCategoria+'</p>','div.listadoCategorias');
//			return false;
//		}
//		$('#baseRecursos div.ko').remove();
//		$('#crearCategoria').slideUp();
//		$('#editarCategoria').slideDown().find('input').val($a.text().replace(/\(\d*\)/,'')).find('button[type=reset]').unbind('click').click(function(){
//			$('#editarCategoria').slideUp();
//		});
//		return false;
//	});
//});


/*---------     Modificado todo by Javier     --------------*/

function MostrarPopUpSelectorCategorias() {
    // meto esto aqui porque imagino que se tendra que llamar por Ajax al listado,
    // bastar con inyectarlo como ultimo elemento del <body/>.
    // Ademas, podeis darle un identificador al <a> para no tener que buscarlo por
    // el DOM usando su contenido =)

    var $capa = $('#capaModal');
    var $iframe = null;
    //var $mask = $capa.find('div.mascara').height($(document).height());
    var cssMascaraCom = {
        'height': '100%',
        'width': '100%',
        'position': 'fixed',
        'top': 0,
        'left': 0
    }
    var $mask = $capa.find('div.mascara').css(cssMascaraCom);
    //$capa.find('div.anyadirCategorias').css('top', ($('html').attr('scrollTop') || $('body').attr('scrollTop') || 0) + 'px')
    /*var cssMascaraComCat = {
    'position': 'fixed',
    'top': 100
    }
    $capa.find('div.anyadirCategorias').css(cssMascaraComCat);*/
    $capa.fadeIn();
    //    if ($.browser.msie && $.browser.version < 7) {
    //        $iframe = $('<iframe></iframe>').css({
    //            position: 'absolute',
    //            top: 0,
    //            left: '50%',
    //            zIndex: parseInt($mask.css('zIndex')) - 1,
    //            width: '1000px',
    //            marginLeft: '-500px',
    //            height: $mask.height(),
    //            filter: 'mask()'
    //        }).insertAfter($mask);
    //    }
    // una vez llamado deberian prepararse los eventos
    $capa.find('a.icoEliminar').unbind('click').click(function () {
        $capa.fadeOut();
        if ($iframe) { $iframe.remove(); }
    });
    return false;
}

function MostrarPopUpSelectorEditoresYCat(pCapa) {
    //Lo siguiente se utiliza para que el popup aparezca centrado en la pantalla del usuario
    var posY = "0px";
    if (navigator.userAgent.toLowerCase().indexOf('chrome') > -1) {//si es chrome
        posY = document.body.scrollTop + (document.documentElement.clientHeight / 2);
    } else//si no es chrome
    {
        posY = document.documentElement.scrollTop + (document.documentElement.clientHeight / 2);
    }

    // meto esto aqui porque imagino que se tendra que llamar por Ajax al listado,
    // bastar con inyectarlo como ultimo elemento del <body/>.
    // Ademas, podeis darle un identificador al <a> para no tener que buscarlo por
    // el DOM usando su contenido =)

    //0 -> selector de categorias
    //1 -> selector de editores

    if (pCapa == 0) {
        document.getElementById('panEditores').style.display = 'none';
        document.getElementById('panSelectorLectores').style.display = 'none';
        if (document.getElementById('panCategorias') != null) {
            document.getElementById('panCategorias').style.display = 'block';
            document.getElementById('panCategorias').style.marginTop = posY - 300 + 'px';
        }
    }
    else if (pCapa == 1) {
        document.getElementById('panEditores').style.display = 'block';
        document.getElementById('panEditores').style.marginTop = posY - 300 + 'px';
        if (document.getElementById('panCategorias') != null) {
            document.getElementById('panCategorias').style.display = 'none';
        }
        document.getElementById('panSelectorLectores').style.display = 'none';
    }
    else {
        document.getElementById('panEditores').style.display = 'none';
        if (document.getElementById('panCategorias') != null) {
            document.getElementById('panCategorias').style.display = 'none';
        }
        document.getElementById('panSelectorLectores').style.display = 'block';
        document.getElementById('panSelectorLectores').style.marginTop = posY - 300 + 'px';
    }
    MostrarPopUpSelectorCategorias();

    /*var $capa = $('#capaModal');
    var $iframe = null;
    var $mask = $capa.find('div.mascara').height($(document).height());
    $capa.find('div.anyadirCategorias').css('top', ($('html').attr('scrollTop') || $('body').attr('scrollTop') || 0) + 'px')
    $capa.fadeIn();
    if ($.browser.msie && $.browser.version < 7) {
    $iframe = $('<iframe></iframe>').css({
    position:'absolute',
    top:0,
    left:'50%',
    zIndex:parseInt($mask.css('zIndex')) - 1,
    width:'1000px',
    marginLeft:'-500px',
    height:$mask.height(),
    filter:'mask()'
    }).insertAfter($mask);
    }
    // una vez llamado deberian prepararse los eventos
    $capa.find('a.icoEliminar').unbind('click').click(function(){
    $capa.fadeOut();
    if ($iframe) {$iframe.remove();}
    });
    return false;*/
}

function MostrarSelectorCategorias() {
    // meto esto aqui porque imagino que se tendra que llamar por Ajax al listado,
    // bastar con inyectarlo como ultimo elemento del <body/>.
    // Ademas, podeis darle un identificador al <a> para no tener que buscarlo por
    // el DOM usando su contenido =)

    if (document.getElementById('panCompartirDocPopUp') != null) {
        document.getElementById('panCompartirDocPopUp').style.display = 'none';
    }
    if (document.getElementById('panAgregarCatDocPopUp') != null) {
        document.getElementById('panAgregarCatDocPopUp').style.display = 'block';
    }
    if (document.getElementById('panAgregarTagDocPopUp') != null) {
        document.getElementById('panAgregarTagDocPopUp').style.display = 'none';
    }

    if (document.getElementById('panInfoEnvioTwitterPopUp') != null) {
        document.getElementById('panInfoEnvioTwitterPopUp').style.display = 'none';
    }
    MostrarPopUpSelectorCategorias();

    /*var $capa = $('#capaModal');
    var $iframe = null;
    var $mask = $capa.find('div.mascara').height($(document).height());
    $capa.find('div.anyadirCategorias').css('top', ($('html').attr('scrollTop') || $('body').attr('scrollTop') || 0) + 'px')
    $capa.fadeIn();
    if ($.browser.msie && $.browser.version < 7) {
    $iframe = $('<iframe></iframe>').css({
    position:'absolute',
    top:0,
    left:'50%',
    zIndex:parseInt($mask.css('zIndex')) - 1,
    width:'1000px',
    marginLeft:'-500px',
    height:$mask.height(),
    filter:'mask()'
    }).insertAfter($mask);
    }
    // una vez llamado deberian prepararse los eventos
    $capa.find('a.icoEliminar').unbind('click').click(function(){
    $capa.fadeOut();
    if ($iframe) {$iframe.remove();}
    });
    return false;*/
}

function MostrarEditorTags() {
    // meto esto aqui porque imagino que se tendra que llamar por Ajax al listado,
    // bastar con inyectarlo como ultimo elemento del <body/>.
    // Ademas, podeis darle un identificador al <a> para no tener que buscarlo por
    // el DOM usando su contenido =)

    document.getElementById('panCompartirDocPopUp').style.display = 'none';
    document.getElementById('panAgregarCatDocPopUp').style.display = 'none';
    document.getElementById('panAgregarTagDocPopUp').style.display = 'block';

    if (document.getElementById('panInfoEnvioTwitterPopUp') != null) {
        document.getElementById('panInfoEnvioTwitterPopUp').style.display = 'none';
    }

    MostrarPopUpSelectorCategorias();

    /*var $capa = $('#capaModal');
    var $iframe = null;
    var $mask = $capa.find('div.mascara').height($(document).height());
    $capa.find('div.anyadirCategorias').css('top', ($('html').attr('scrollTop') || $('body').attr('scrollTop') || 0) + 'px')
    $capa.fadeIn();
    if ($.browser.msie && $.browser.version < 7) {
    $iframe = $('<iframe></iframe>').css({
    position:'absolute',
    top:0,
    left:'50%',
    zIndex:parseInt($mask.css('zIndex')) - 1,
    width:'1000px',
    marginLeft:'-500px',
    height:$mask.height(),
    filter:'mask()'
    }).insertAfter($mask);
    }
    // una vez llamado deberian prepararse los eventos
    $capa.find('a.icoEliminar').unbind('click').click(function(){
    $capa.fadeOut();
    if ($iframe) {$iframe.remove();}
    });
    return false;*/
}

function MostrarCompartidorDocumentos() {
    // meto esto aqui porque imagino que se tendra que llamar por Ajax al listado,
    // bastar con inyectarlo como ultimo elemento del <body/>.
    // Ademas, podeis darle un identificador al <a> para no tener que buscarlo por
    // el DOM usando su contenido =)

    document.getElementById('panCompartirDocPopUp').style.display = 'block';
    if (document.getElementById('panAgregarCatDocPopUp') != null) {
        document.getElementById('panAgregarCatDocPopUp').style.display = 'none';
    }
    if (document.getElementById('panAgregarTagDocPopUp') != null) {
        document.getElementById('panAgregarTagDocPopUp').style.display = 'none';
    }

    if (document.getElementById('panInfoEnvioTwitterPopUp') != null) {
        document.getElementById('panInfoEnvioTwitterPopUp').style.display = 'none';
    }
    MostrarPopUpSelectorCategorias();
}

function MostrarInfoEnvioTwitter() {
    if (document.getElementById('panInfoEnvioTwitterPopUp') != null) {
        document.getElementById('panInfoEnvioTwitterPopUp').style.display = 'block';
    }
    if (document.getElementById('panCompartirDocPopUp') != null) {
        document.getElementById('panCompartirDocPopUp').style.display = 'none';
    }
    if (document.getElementById('panAgregarCatDocPopUp') != null) {
        document.getElementById('panAgregarCatDocPopUp').style.display = 'none';
    }
    if (document.getElementById('panAgregarTagDocPopUp') != null) {
        document.getElementById('panAgregarTagDocPopUp').style.display = 'none';
    }
    MostrarPopUpSelectorCategorias();
}

function MostrarVinculadorDocumentos() {
    document.getElementById('panCompartirDocPopUp').style.display = 'none';
    document.getElementById('panAgregarCatDocPopUp').style.display = 'none';
    document.getElementById('panAgregarTagDocPopUp').style.display = 'none';

    if (document.getElementById('panInfoEnvioTwitterPopUp') != null) {
        document.getElementById('panInfoEnvioTwitterPopUp').style.display = 'none';
    }
    document.getElementById('panVincularDocADoc').style.display = 'block';

    MostrarPopUpSelectorCategorias();
}

function CerrarSelectorCategorias() {
    CerrarCapaModal();
}

function CerrarCapaModal() {
    var $capa = $('#capaModal');
    $capa.fadeOut();
}

function CerrarCompartidorRecurso() {
    CerrarSelectorCategorias();
    document.getElementById('panCompartirDocPopUp').style.display = 'none';
}

function CerrarSelectorCualquiera(capa) {
    var $capa = $('#' + capa);
    $capa.fadeOut();
}

function AjustarPanelDesplegableBusqAvanzParaAutoCompTags() {
    if (document.getElementById('panBusquedaAv').style.overflow != "visible") {
        document.getElementById('panBusquedaAv').style.overflow = "visible";
    }
    else {
        document.getElementById('panBusquedaAv').style.overflow = "hidden";
    }
}

function CalcularTopPanelYMostrar(evento, panelID) {
    if (!evento.target) {
        var hijo = evento.srcElement;
    }
    else {
        var hijo = evento.target;
    }
    if (hijo.nodeName == 'IMG') {
        hijo = $(hijo).parent();
    }
    var $padre = $(hijo).parent();

    $(document.getElementById(panelID)).css({
        top: $padre.offset().top + $padre.height() - $(document.getElementById(panelID)).height() / 2 + 'px',
        display: ''
    });
    return false;
}

/*---------     Fin by Javier     --------------*/

/*---------     RIAM: Funcion para modificar un checkBox       --------------*/
function ModificarCheck(checkID, estado) {
    if ($('#' + checkID).length > 0) {
        $('#' + checkID).attr('checked', estado);
    }
}

/*---------     FIN RIAM: Funcion para modificar un checkBox       --------------*/

/*---------    REGION RESALTAR TAGS, BY ALTU    --------------------------------------------*/


//NOTA:   NO TOCAR SIN EL COSENTIMIENTO DE JAVIER.

function ResaltarTags(pListaTags) {
    var listaColoresResaltar = new Array(8);

    listaColoresResaltar[0] = "#8F529D"; //"rgb(143, 82, 157)";
    listaColoresResaltar[1] = "#4C8FB5"; //"rgb(76, 143, 181)";
    listaColoresResaltar[2] = "#E08552"; //"rgb(224, 133, 82)";
    listaColoresResaltar[3] = "#E55982"; //"rgb(229, 89, 130)";
    listaColoresResaltar[4] = "#C4A3CB"; //"rgb(196, 163, 203)";
    listaColoresResaltar[5] = "#B5DDF1"; //"rgb(181, 221, 241)";
    listaColoresResaltar[6] = "#F8C0A9"; //"rgb(248, 192, 169)";
    listaColoresResaltar[7] = "#F6ABCF"; //"rgb(246, 171, 207)";

    listaPlanaColoresResaltar = "#8F529D #4C8FB5 #E08552 #E55982 #C4A3CB #B5DDF1 #F8C0A9 #F6ABCF";
    listaArtiConjuPrep = ",el,la,los,las,un,una,lo,unos,unas,y,o,u,e,ni,a,con,de,del,en,para,por,al,";
    listaCaracteresExpurios = [" ", ",", "\"", "\'", "(", ")", ";", "<", ">", ":", "/"];

    var elementos = $('.Resaltable');
    var elementosTags = $('.TagResaltable');
    var listaTags = pListaTags.split(",");

    for (var i = 0; i < elementos.length; i++) {

        var texto = elementos[i].innerHTML;
        elementos[i].innerHTML = ObtenerTextoConEnfasisSegunLosTags(texto, listaTags, listaColoresResaltar, false);
    }

    for (var i = 0; i < elementosTags.length; i++) {

        var textoTags = elementosTags[i].innerHTML;
        //elementosTags[i].innerHTML = ObtenerTextoConEnfasisSegunLosTagsParaCadenaDeTags(textoTags, listaTags, listaColoresResaltar);
        elementosTags[i].innerHTML = ObtenerTextoConEnfasisSegunLosTags(textoTags, listaTags, listaColoresResaltar, false);
    }
}

function ObtenerTextoConEnfasisSegunLosTags(pTexto, pListaTags, pListaColoresResaltar, pExpandirPalabra) {
    var textoConEnfasis = pTexto;
    var count = 0;

    for (var i = 0; i < pListaTags.length; i++) {
        var tag = pListaTags[i];
        var textoEnMinusSinAcentos = QuitarAcentosConvertirMinuscula(textoConEnfasis);
        var tagEnMinusSinAcentos = QuitarAcentosConvertirMinuscula(tag);

        if (textoEnMinusSinAcentos.indexOf(tagEnMinusSinAcentos) != -1) {//Contiene todo el tag por lo enfatizo entero:

            textoConEnfasis = ProcesarTagEnTextoUnaOVariosVeces(textoConEnfasis, tag, count, pListaColoresResaltar, listaCaracteresExpurios, pExpandirPalabra);
        }
        else {
            //Puede que contenga alguna palabra del tag por lo que la enfatizo individualmente:
            var trozosTag = SepararTextoPorCarater(tag, ' ');

            for (var x = 0; x < trozosTag.length; x++) {
                var trozoTag = trozosTag[x];

                if (!EsArticuloOConjuncionOPreposicionesComunes(trozoTag)) {
                    var textoRecompuesto = "";
                    var separador = "";
                    var palabras = SepararTextoPorCarater(textoConEnfasis, ' ');

                    for (var z = 0; z < palabras.length; z++) {
                        var palabra = palabras[z];
                        var palabraLimpia = QuitarAcentosConvertirMinuscula(palabra);
                        var trozoTagLimpio = QuitarAcentosConvertirMinuscula(trozoTag);

                        if (palabraLimpia == trozoTagLimpio) {
                            textoRecompuesto += separador + AgregarEnfasisATexto(palabra, count, pListaColoresResaltar);
                        }
                        else if (pExpandirPalabra && palabraLimpia.indexOf(trozoTagLimpio) != -1) {
                            if (caracterEnPListaCaracteresExpurios(listaCaracteresExpurios, palabra.charAt(0))) {
                                textoRecompuesto += separador + palabra.charAt(0) + AgregarEnfasisATexto(palabra.substring(1), count, pListaColoresResaltar);
                            }
                            else if (caracterEnPListaCaracteresExpurios(listaCaracteresExpurios, palabra[palabra.length - 1])) {
                                textoRecompuesto += separador + AgregarEnfasisATexto(palabra.substring(0, palabra.length - 1), count, pListaColoresResaltar) + palabra.charAt(palabra.length - 1);
                            }
                            else {
                                textoRecompuesto += separador + AgregarEnfasisATexto(palabra, count, pListaColoresResaltar);
                            }
                        }
                        else {
                            textoRecompuesto += separador + palabra;
                        }
                        separador = " ";
                    }
                    textoConEnfasis = textoRecompuesto;
                }
            }
        }
        count++;

        if (count >= pListaColoresResaltar.length) {
            count = 0;
        }
    }
    return textoConEnfasis;
}

function ObtenerTextoConEnfasisSegunLosTagsParaCadenaDeTags(pTexto, pListaTags, pListaColoresResaltar) {
    var textoConEnfasis = pTexto;
    var count = 0;

    for (var i = 0; i < pListaTags.length; i++) {
        var tag = pListaTags[i];
        var tagContieneTrozoTag = false;
        var textoEnMinusSinAcentos = QuitarAcentosConvertirMinuscula(textoConEnfasis);
        var tagEnMinusSinAcentos = QuitarAcentosConvertirMinuscula(tag);

        if (textoEnMinusSinAcentos.indexOf(tagEnMinusSinAcentos) != -1) {//Contiene todo el tag por lo enfatizo el tag entero entero:
            tagContieneTrozoTag = true;
        }
        else {//Puede que contenga alguna palabra del tag por lo que la enfatizo individualmente:
            var trozosTag = SepararTextoPorCarater(tag, ' ');

            for (var x = 0; x < trozosTag.length; x++) {
                var trozoTag = trozosTag[x];

                if (!EsArticuloOConjuncionOPreposicionesComunes(trozoTag)) {
                    var palabras = SepararTextoPorCarater(textoConEnfasis, ' ');

                    for (var z = 0; z < palabras.length; z++) {
                        var palabra = palabras[z];

                        if (QuitarAcentosConvertirMinuscula(palabra) == QuitarAcentosConvertirMinuscula(trozoTag)) {
                            tagContieneTrozoTag = true;
                            break;
                        }
                    }
                }
            }
        }

        if (tagContieneTrozoTag && textoConEnfasis.indexOf("<span") == -1) {
            textoConEnfasis = AgregarEnfasisATexto(EliminarEspaciosExteriores(textoConEnfasis), count, pListaColoresResaltar);
        }
        else {
            textoConEnfasis = EliminarEspaciosExteriores(textoConEnfasis);
        }
        count++;

        if (count >= pListaColoresResaltar.Length) {
            count = 0;
        }
    }
    return textoConEnfasis;
}

function ProcesarTagEnTextoUnaOVariosVeces(pTexto, pTag, pEstiloResalto, pListaColoresResaltar, pListaCaracteresExpurios, pExpandirPalabra) {
    var textoConEnfasis = pTexto;

    var textoEnMinusSinAcentos = QuitarAcentosConvertirMinuscula(textoConEnfasis);
    var tagEnMinusSinAcentos = QuitarAcentosConvertirMinuscula(pTag);

    if (textoEnMinusSinAcentos.indexOf(tagEnMinusSinAcentos) != -1) {//Contiene todo el tag por lo enfatizo entero:

        if (pTexto.length > pTag.length) {
            var inicioTagEnTexto = textoEnMinusSinAcentos.indexOf(tagEnMinusSinAcentos);
            var finTagEnTexto = inicioTagEnTexto + pTag.length;

            if (pExpandirPalabra) {
                //Expandimos el tag hasta tener palabras completas, o el final del texto:
                while (inicioTagEnTexto != 0 && !caracterEnPListaCaracteresExpurios(pListaCaracteresExpurios, textoConEnfasis.charAt(inicioTagEnTexto - 1))) {
                    inicioTagEnTexto--;
                }
                while (finTagEnTexto != textoConEnfasis.length && !caracterEnPListaCaracteresExpurios(pListaCaracteresExpurios, textoConEnfasis.charAt(finTagEnTexto))) {
                    finTagEnTexto++;
                }

                textoConEnfasis = textoConEnfasis.substring(0, inicioTagEnTexto) + AgregarEnfasisATexto(textoConEnfasis.substring(inicioTagEnTexto, finTagEnTexto), pEstiloResalto, pListaColoresResaltar) + ProcesarTagEnTextoUnaOVariosVeces(textoConEnfasis.substring(finTagEnTexto), pTag, pEstiloResalto, pListaColoresResaltar, pListaCaracteresExpurios, pExpandirPalabra); //+ textoConEnfasis.substring(finTagEnTexto);
            }
            else {
                if ((inicioTagEnTexto == 0 || caracterEnPListaCaracteresExpurios(pListaCaracteresExpurios, textoConEnfasis.charAt(inicioTagEnTexto - 1))) && (finTagEnTexto == textoEnMinusSinAcentos.length || caracterEnPListaCaracteresExpurios(pListaCaracteresExpurios, textoConEnfasis.charAt(finTagEnTexto)))) { //Solo si la cadena contiene el propio pTag como palabra
                    textoConEnfasis = textoConEnfasis.substring(0, inicioTagEnTexto) + AgregarEnfasisATexto(textoConEnfasis.substring(inicioTagEnTexto, finTagEnTexto), pEstiloResalto, pListaColoresResaltar) + ProcesarTagEnTextoUnaOVariosVeces(textoConEnfasis.substring(finTagEnTexto), pTag, pEstiloResalto, pListaColoresResaltar, pListaCaracteresExpurios, pExpandirPalabra);
                }
                else {
                    textoConEnfasis = textoConEnfasis.substring(0, finTagEnTexto) + ProcesarTagEnTextoUnaOVariosVeces(textoConEnfasis.substring(finTagEnTexto), pTag, pEstiloResalto, pListaColoresResaltar, pListaCaracteresExpurios, pExpandirPalabra);
                }
            }
        }
        else {
            textoConEnfasis = AgregarEnfasisATexto(textoConEnfasis, pEstiloResalto, pListaColoresResaltar);
        }
    }
    return textoConEnfasis;
}

function caracterEnPListaCaracteresExpurios(pListaCaracteresExpurios, caracter) {
    for (i = 0; i < pListaCaracteresExpurios.length; i++) {
        if (pListaCaracteresExpurios[i] == caracter) {
            return true;
        }
    }
    return false;
}

function AgregarEnfasisATexto(pTexto, pEstiloResalto, pListaColoresResaltar) {
    if (pTexto.indexOf("style=") == -1 && pTexto.indexOf("<span class") == -1 && pTexto.indexOf("</span") == -1 && pTexto != "span" && pTexto != "class=" && pTexto != "tag" && pTexto != "background-color" && pTexto != "/span" && pTexto != "color" && pTexto != "#FFFFFF" && listaPlanaColoresResaltar.indexOf(pTexto) == -1) {
        return "<span class=\"tag\" style=\"color:#FFFFFF;background-color:" + pListaColoresResaltar[pEstiloResalto] + ";\">" + pTexto + "</span>";
    }
    else {
        return pTexto;
    }
}

function EliminarEspaciosExteriores(pTexto) {
    var textoSinEspacios = pTexto;
    var hayEspacios = true;

    while (textoSinEspacios.length > 0 && hayEspacios) {
        if (textoSinEspacios.charAt(0) == ' ') {
            textoSinEspacios = textoSinEspacios.substring(1);
        }
        else if (textoSinEspacios[textoSinEspacios.length - 1] == ' ') {
            textoSinEspacios = textoSinEspacios.substring(0, textoSinEspacios.length - 1);
        }
        else {
            hayEspacios = false;
        }
    }
    return textoSinEspacios;
}

function SepararTextoPorCarater(pTexto, pCaracter) {
    var palabras = pTexto.split(pCaracter);

    //Quito elementos vacíos:
    var textoAuxiliar = "";
    var separador = "";

    for (var i = 0; i < palabras.length; i++) {
        if (palabras[i] != '') {
            textoAuxiliar += separador + palabras[i];
            separador = ",";
        }
    }
    return textoAuxiliar.split(",");
}

function EsArticuloOConjuncionOPreposicionesComunes(pTexto) {
    if (pTexto.length > 4) {
        //No hay ninguna preposición, conjunción o artículo con las de 4 caracteres.
        return false;
    }
    else {
        return (listaArtiConjuPrep.indexOf(',' + pTexto + ',') != -1);
    }
}

function QuitarAcentosConvertirMinuscula(pTexto) {
    var textoLimpio = pTexto;

    textoLimpio = textoLimpio.replace(/\á/g, 'a');
    textoLimpio = textoLimpio.replace(/\Á/g, 'a');

    textoLimpio = textoLimpio.replace(/\é/g, 'e');
    textoLimpio = textoLimpio.replace(/\É/g, 'e');

    textoLimpio = textoLimpio.replace(/\í/g, 'i');
    textoLimpio = textoLimpio.replace(/\Í/g, 'i');

    textoLimpio = textoLimpio.replace(/\ó/g, 'o');
    textoLimpio = textoLimpio.replace(/\Ó/g, 'o');

    textoLimpio = textoLimpio.replace(/\ú/g, 'u');
    textoLimpio = textoLimpio.replace(/\Ú/g, 'u')

    textoLimpio = textoLimpio.toLowerCase();

    return textoLimpio;
}

/*--------    FIN REGION RESALTAR TAGS    ---------------------------------------------------*/

/*--------    REGION BUSQUEDA POR VARIAS CATEGORÍAS -----------------------------------------*/

function AceptarSelectorCatBusqueda(pTxtHackIds, pTxtControlSelector) {
    AceptarSelectorCatBusqueda(pTxtHackIds, pTxtControlSelector, true, false);
}

function AceptarSelectorCatBusqueda(pTxtHackIds, pTxtControlSelector, pFiltrarBusqueda, pCategoriasDocumentacion) {
    document.getElementById(pTxtHackIds).value = document.getElementById(pTxtControlSelector).value;

    var categoriaIDs = document.getElementById(pTxtHackIds).value;
    var divs = document.getElementById('divSelCatTesauro');
    document.getElementById('contenedorFiltrosCategorias').innerHTML = '';

    var nivel = 1;

    if (pCategoriasDocumentacion) {
        document.getElementById('contenedorFiltrosCatDocumentacion').innerHTML = '';
        nivel = 0;
    }

    var numSeleccionados = AgregarCategoriasASeleccion(divs, categoriaIDs, pTxtHackIds, pTxtControlSelector, nivel, pFiltrarBusqueda, pCategoriasDocumentacion);

    var divsLista = document.getElementById('divSelCatLista');
    AjustarCategoriasSeleccion(divsLista.children[1], categoriaIDs);

    if (document.getElementById('filtrosCategorias') != null) {
        if (numSeleccionados > 0) {
            document.getElementById('filtrosCategorias').style.display = '';
            //EjecutarScriptsIniciales();
        }
        else {
            document.getElementById('filtrosCategorias').style.display = 'none';
        }
    }

    if (pFiltrarBusqueda) {
        FiltrarBusqueda();
    }
}

function AgregarCategoriasASeleccion(pDivs, pCategoriaIDs, pTxtHackIds, pTxtControlSelector, pNivel, pFiltrarBusqueda, pCategoriasDocumentacion) {
    var numSeleccionados = 0;

    for (var i = 0; i < pDivs.children.length; i++) {
        if (pCategoriaIDs.indexOf(pDivs.children[i].children[1].className) != -1) {
            var contenedor = null;

            if (pNivel != 0) {
                contenedor = 'contenedorFiltrosCategorias';
            }
            else {
                contenedor = 'contenedorFiltrosCatDocumentacion';
            }

            document.getElementById(contenedor).innerHTML += '<a id="idTemp" onclick="EliminarCategoriaFiltroBusqueda(\'' + pDivs.children[i].children[1].className + '\',\'' + pTxtHackIds + '\', \'' + pTxtControlSelector + '\', ' + pNivel + ', ' + pFiltrarBusqueda + ', ' + pCategoriasDocumentacion + ');">' + pDivs.children[i].children[1].children[1].innerHTML + '</a> ';
            numSeleccionados++;
            $(pDivs.children[i].children[1].children[0]).attr('checked', true);
        }
        else if ($(pDivs.children[i].children[1].children[0]).is(':checked')) {
            $(pDivs.children[i].children[1].children[0]).attr('checked', false);
        }

        if (pDivs.children[i].children.length == 3) {
            numSeleccionados = numSeleccionados + AgregarCategoriasASeleccion(pDivs.children[i].children[2], pCategoriaIDs, pTxtHackIds, pTxtControlSelector, pNivel + 1, pFiltrarBusqueda, pCategoriasDocumentacion);
        }
    }

    return numSeleccionados;
}

function AjustarCategoriasSeleccion(pDivs, pCategoriaIDs) {
    for (var i = 0; i < pDivs.children.length; i++) {
        if (pCategoriaIDs.indexOf(pDivs.children[i].children[0].className) == -1) {
            $(pDivs.children[i].children[0].children[0]).attr('checked', false)
        }
        else {
            $(pDivs.children[i].children[0].children[0]).attr('checked', true)
        }

        if (pDivs.children[i].children.length == 2) {
            AjustarCategoriasSeleccion(pDivs.children[i].children[1], pCategoriaIDs);
        }
    }
}

function EliminarCategoriaFiltroBusqueda(pCategoriaID, pTxtHackIds, pTxtControlSelector, pNivel, pFiltrarBusqueda, pCategoriasDocumentacion) {
    document.getElementById(pTxtControlSelector).value = document.getElementById(pTxtControlSelector).value.replace(pCategoriaID + ',', '');

    if (pCategoriasDocumentacion && pNivel == 0) {
        document.getElementById(pTxtControlSelector).value = '';
        HabilitarTodosLosElementosArbol();
    }

    AceptarSelectorCatBusqueda(pTxtHackIds, pTxtControlSelector, pFiltrarBusqueda, pCategoriasDocumentacion);
}

function MostrarOcultarPanelCatBusqueda(pPanelCatID) {
    Desplegar(this, pPanelCatID);

    if (document.getElementById(pPanelCatID).style.display != 'none') {
        document.getElementById('aspnetForm').setAttribute('onclick', 'OcultarPanelCategoriasBusqueda(\'' + pPanelCatID + '\');');
    }
}

function OcultarPanelCategoriasBusqueda(pPanelCatID) {
    if (document.getElementById('txtHackNoCerrarSelector').value == '' && document.getElementById(pPanelCatID).style.display != 'none') {
        Desplegar(this, pPanelCatID);
    }

    document.getElementById('txtHackNoCerrarSelector').value = '';
}

function LimpiarCatSelecciondas(pTxtHackIds, pTxtControlSelector, pPanelCatID, pFiltrarBusqueda, pCategoriasDocumentacion) {
    if (pCategoriasDocumentacion) {
        HabilitarTodosLosElementosArbol();
    }

    document.getElementById(pTxtControlSelector).value = '';
    AceptarSelectorCatBusqueda(pTxtHackIds, pTxtControlSelector, pFiltrarBusqueda, pCategoriasDocumentacion);
    Desplegar(this, pPanelCatID);
}

function AjustarTopControl(pControlID) {
    //document.getElementById(pControlID).style.top = (((document.body.offsetHeight-17)/4) + document.documentElement.scrollTop)+'px';



    if (navigator.userAgent.toLowerCase().indexOf('chrome') > -1) {
        document.getElementById(pControlID).style.top = (200 + document.body.scrollTop) + 'px';
    } else//si no es chrome
    {
        document.getElementById(pControlID).style.top = (200 + document.documentElement.scrollTop) + 'px';
    }
}

function MostrarControles(pControles) {
    while (pControles.length > 1) {
        var controlID = '';
        if (pControles.indexOf(',') != -1) {
            controlID = pControles.substring(0, pControles.indexOf(','));
            pControles = pControles.replace(controlID + ',', '');
        }
        else {
            controlID = pControles;
            pControles = pControles.replace(controlID, '');
        }

        if (document.getElementById(controlID) != null) {
            $('#' + controlID).fadeIn();
        }
    }
}

function CalcularTagsSegunTitulo(pTituloID, pHackTituloID, pTagsID, pHackPalabrasNoRelevantesID, pHackSeparadores) {
    var tags = ObtenerTagsFrase(document.getElementById(pTituloID).value, pHackPalabrasNoRelevantesID, pHackSeparadores);
    var txtTags = ''; //toLowerCase()

    //Quito los tags antiguos:
    var txtHackTagsTitulo = document.getElementById(pHackTituloID).value
    var tagsActuales = document.getElementById(pTagsID).value.split(',');
    var separador = '';
    for (var j = 0; j < tagsActuales.length; j++) {
        var tag = tagsActuales[j].trim();
        if (tag != '' && txtHackTagsTitulo.indexOf('[&]' + tag + '[&]') == -1) {
            txtTags += separador + tag;
            separador = ', '
        }
    }

    //Agrego los tags nuevos:
    txtHackTagsTitulo = '';
    for (var i = 0; i < tags.length; i++) {
        if (tags[i] != '') {
            txtTags += separador + tags[i];
            separador = ', '
            txtHackTagsTitulo += '[&]' + tags[i] + '[&]';
        }
    }

    document.getElementById(pTagsID).value = txtTags;
    document.getElementById(pHackTituloID).value = txtHackTagsTitulo;
}

function ObtenerTagsFrase(pFrase, pHackPalabrasNoRelevantesID, pHackSeparadores) {
    var listaTags = new Array();
    var ListaPalabrasNoRelevantes = document.getElementById(pHackPalabrasNoRelevantesID).value;

    var numeroPalabrasDescartadas = 0;

    var subcadenas = ObtenerPalabras(pFrase, pHackSeparadores);

    var listaTagsTexto = '';
    for (var i = 0; i < subcadenas.length; i++) {
        var palabra = LimpiarPalabraParaTagGeneradoSegunTitulo(subcadenas[i].toLowerCase());

        if (palabra.indexOf('"') != -1 || palabra.indexOf('\'') != -1) {
            if (palabra.indexOf('"') == 0 || palabra.lastIndexOf('"') == palabra.length - 1) {
                palabra = palabra.replace(/"/g, '');
            }
            else if (palabra.indexOf('\'') == 0 || palabra.lastIndexOf('\'') == palabra.length - 1) {
                palabra = palabra.replace(/\'/g, '');
            }
        }

        if (ListaPalabrasNoRelevantes.indexOf('[&]' + palabra + '[&]') == -1 && listaTagsTexto.indexOf('[&]' + palabra + '[&]') == -1) {
            listaTags[listaTags.length] = palabra;
            listaTagsTexto += '[&]' + palabra + '[&]';
        }
        else {
            numeroPalabrasDescartadas++;
        }
    }

    if (listaTags.length != 0) {
        if (listaTags[listaTags.length - 1].indexOf(".") == (listaTags[listaTags.length - 1].length - 1)) {
            var ultima = listaTags[listaTags.length - 1];
            listaTags[listaTags.length - 1] = ultima.replace('.', '');
        }
    }

    return listaTags;
}

function LimpiarPalabraParaTagGeneradoSegunTitulo(pPalabra) {
    if (pPalabra.length == 0) {
        return pPalabra;
    }
    else if (pPalabra.indexOf('¿') == 0 || pPalabra.indexOf('?') == 0 || pPalabra.indexOf('¡') == 0 || pPalabra.indexOf('!') == 0) {
        return LimpiarPalabraParaTagGeneradoSegunTitulo(pPalabra.substring(1));
    }
    else if (pPalabra.lastIndexOf('¿') == (pPalabra.length - 1) || pPalabra.lastIndexOf('?') == (pPalabra.length - 1) || pPalabra.lastIndexOf('¡') == (pPalabra.length - 1) || pPalabra.lastIndexOf('!') == (pPalabra.length - 1)) {
        return LimpiarPalabraParaTagGeneradoSegunTitulo(pPalabra.substring(0, pPalabra.length - 1));
    }
    else {
        return pPalabra;
    }
}

function ObtenerPalabras(pFrase, pHackSeparadores) {
    var separadores = document.getElementById(pHackSeparadores).value + ' [&]';
    var listaSeparadores = separadores.split('[&]');
    var frase = [pFrase];

    for (var i = 0; i < listaSeparadores.length; i++) {
        frase = ObtenerPalabrasSeparadas(frase, listaSeparadores[i]);
    }

    return frase;
}

function ObtenerPalabrasSeparadas(pPalabras, pSeparador) {
    if (pSeparador != '') {
        var palabras = new Array();

        for (var i = 0; i < pPalabras.length; i++) {
            var palabrasInt = pPalabras[i].split(pSeparador);

            for (var j = 0; j < palabrasInt.length; j++) {
                palabras[palabras.length] = palabrasInt[j];
            }
        }

        return palabras;
    }
    else {
        return pPalabras;
    }
}

/*--------    FIN REGION BUSQUEDA POR VARIAS CATEGORÍAS    ----------------------------------*/


/*--------    REGION DAFOS ------------------------------------------------------------------*/

function CalcularNumFactoresSinVotar(elementID, txtHackID, voto) {
    var element = document.getElementById(elementID);
    var txtHack = document.getElementById(txtHackID).value;
    var numFactoresSinVotar = element.innerHTML.substring(element.innerHTML.indexOf('(', 0) + 1, element.innerHTML.length - 1);
    if (txtHack == 0 && voto > 0) {
        numFactoresSinVotar = parseFloat(numFactoresSinVotar) - 1;
    }
    else if (txtHack > 0 && voto == 0) {
        numFactoresSinVotar = parseFloat(numFactoresSinVotar) + 1;
    }
    element.innerHTML = element.innerHTML.substring(0, element.innerHTML.indexOf('(', 0) - 1) + ' (' + numFactoresSinVotar + ')';
    document.getElementById(txtHackID).value = voto;
}

/*--------    FIN REGION DAFOS    -----------------------------------------------------------*/


function cambiarFormatoFecha(fecha) {
    //Cambia una fecha en formato 01/02/2011 a 20110201
    var cachos;
    cachos = fecha.split('/');
    return cachos[2] + cachos[1] + cachos[0];
}



/*--------    REGION BLOGS ------------------------------------------------------------------*/

function AgregarCategoriaBlog(txtNombre, txtHack, panel, baseURL) {
    var categoria = txtNombre.val();
    if (txtHack.val().indexOf("##&##" + categoria + "##&##") == -1) {
        txtHack.val(txtHack.val() + categoria + "##&##");
    }
    PintarCategoriasBlog(txtHack, panel, baseURL);
    txtNombre.val('');
}

function PintarCategoriasBlog(txtHack, panel, baseURL) {
    panel.html('');
    var listaCat = txtHack.val().split("##&##");

    for (var i = 0; i < listaCat.length; i++) {
        var categoria = listaCat[i];
        if (categoria != "") {
            var html = "<label>" + categoria + "<a onclick=\"javascript:EliminarCategoriaBlog(this.parentNode.textContent, $('" + txtHack.selector + "'), $('" + panel.selector + "'), '" + baseURL + "');\"><img src='" + baseURL + "blank.gif' alt='" + borr.eliminar + "'/></a></label>"
            panel.html(panel.html() + html);
        }
    }
}

function EliminarCategoriaBlog(categoria, txtHack, panel, baseURL) {
    if (txtHack.val().indexOf("##&##" + categoria + "##&##") >= 0) {
        txtHack.val(txtHack.val().replace(categoria + "##&##", ""));
    }

    PintarCategoriasBlog(txtHack, panel, baseURL);
}

/*--------    FIN REGION BLOGS    -----------------------------------------------------------*/

function quitarFormatoHTML(cadena) {
    //return Encoder.htmlDecode(cadena.replace(/<[^>]+>/g,'').replace(/\n/g, '').replace(/^\s*|\s*$/g,""));
    return Encoder.htmlDecode(cadena.replace(/^\s*|\s*$/g, ""));
}





/*--------    REGION BUSCADOR FACETADO    -----------------------------------------------------------*/

function ObtenerHash() {
    var hash = window.location.hash;
    if (hash != null && hash != '') {
        var posicion = hash.indexOf(hash)
        if (posicion > -1) {
            return hash;
        }
    }
    return '';
}

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


function autocompletarGenerico(control, pClaveFaceta, pOrden, pParametros) {
    $(control).autocomplete(
        null,
        {
            //servicio: new WS($('input.inpt_urlServicioAutocompletar').val(), WSDataType.jsonp),
            //metodo: 'AutoCompletarFacetas',
            url: $('input.inpt_urlServicioAutocompletar').val() + "/AutoCompletarFacetas",
            type: "POST",
            delay: 0,
            minLength: 4,
            scroll: false,
            selectFirst: false,
            minChars: 4,
            width: 170,
            cacheLength: 0,
            extraParams: {
                proyecto: $('input.inpt_proyID').val(),
                bool_esMyGnoss: $('input.inpt_bool_esMyGnoss').val() == 'True',
                bool_estaEnProyecto: $('input.inpt_bool_estaEnProyecto').val() == 'True',
                bool_esUsuarioInvitado: $('input.inpt_bool_esUsuarioInvitado').val() == 'True',
                identidad: $('input.inpt_identidadID').val(),
                organizacion: $('input.inpt_organizacionID').val(),
                filtrosContexto: '',
                languageCode: $('input.inpt_Idioma').val(),
                perfil: perfilID,
                nombreFaceta: pClaveFaceta,
                orden: pOrden,
                parametros: replaceAll(replaceAll(replaceAll(pParametros.replace('#', ''), '%', '%25'), '#', '%23'), '+', "%2B"),
                tipo: $('input.inpt_tipoBusquedaAutoCompl').val(),
                botonBuscar: control.id + 'botonBuscar'
            }
        }
    );
    if (control.attributes["onfocus"] != null) {
        control.attributes.removeNamedItem('onfocus');
    }

    pintarTagsInicio();
}


function autocompletarUsuario(control, pClaveFaceta, pOrden, pParametros, pGrafo) {
    $(control).autocomplete(
        null,
        {
            //servicio: new WS($('input.inpt_urlServicioAutocompletar').val(), WSDataType.jsonp),
            //metodo: 'AutoCompletarFacetas',
            url: $('input.inpt_urlServicioAutocompletar').val() + "/AutoCompletarFacetas",
            type: "POST",
            delay: 0,
            minLength: 4,
            scroll: false,
            selectFirst: false,
            minChars: 4,
            width: 170,
            cacheLength: 0,
            extraParams: {
                proyecto: pGrafo,
                bool_esMyGnoss: $('input.inpt_bool_esMyGnoss').val() == 'True',
                bool_estaEnProyecto: $('input.inpt_bool_estaEnProyecto').val() == 'True',
                bool_esUsuarioInvitado: $('input.inpt_bool_esUsuarioInvitado').val() == 'True',
                identidad: $('input.inpt_identidadID').val(),
                organizacion: $('input.inpt_organizacionID').val(),
                perfil: perfilID,
                filtrosContexto: '',
                languageCode: $('input.inpt_Idioma').val(),
                nombreFaceta: pClaveFaceta,
                orden: pOrden,
                parametros: pParametros,
                tipo: $('input.inpt_tipoBusquedaAutoCompl').val(),
                botonBuscar: control.id + 'botonBuscar'
            }
        }
    );
    if (control.attributes["onfocus"] != null) {
        control.attributes.removeNamedItem('onfocus');
    }
}

function autocompletarGenericoConFiltroContexto(control, pClaveFaceta, pOrden, pParametros, pFiltroContexto) {
    var proyectoBusqueda = $('input.inpt_proyID').val();

    if (parametros_adiccionales.indexOf('proyectoOrigenID=') == 0) {
        proyectoBusqueda = parametros_adiccionales.substring(parametros_adiccionales.indexOf('proyectoOrigenID=') + 'proyectoOrigenID='.length);
        proyectoBusqueda = proyectoBusqueda.substring(0, proyectoBusqueda.indexOf('|'));
    }

    $(control).autocomplete(
        null,
        {
            //servicio: new WS($('input.inpt_urlServicioAutocompletar').val(), WSDataType.jsonp),
            //metodo: 'AutoCompletarFacetas',
            url: $('input.inpt_urlServicioAutocompletar').val() + "/AutoCompletarFacetas",
            type: "POST",
            delay: 0,
            minLength: 4,
            scroll: false,
            selectFirst: false,
            minChars: 4,
            width: 170,
            cacheLength: 0,
            extraParams: {
                proyecto: proyectoBusqueda,
                bool_esMyGnoss: $('input.inpt_bool_esMyGnoss').val() == 'True',
                bool_estaEnProyecto: $('input.inpt_bool_estaEnProyecto').val() == 'True',
                bool_esUsuarioInvitado: $('input.inpt_bool_esUsuarioInvitado').val() == 'True',
                identidad: $('input.inpt_identidadID').val(),
                organizacion: $('input.inpt_organizacionID').val(),
                filtrosContexto: pFiltroContexto,
                languageCode: $('input.inpt_Idioma').val(),
                perfil: perfilID,
                nombreFaceta: pClaveFaceta,
                orden: pOrden,
                parametros: encodeURIComponent(pParametros),
                tipo: $('input.inpt_tipoBusquedaAutoCompl').val(),
                botonBuscar: control.id + 'botonBuscar'
            }
        }
    );
    if (control.attributes["onfocus"] != null) {
        control.attributes.removeNamedItem('onfocus');
    }

    pintarTagsInicio();
}

function autocompletarEtiquetas(control, pClaveFaceta, pOrden, pParametros) {
    $(control).autocomplete(
        null,
        {
            servicio: new WS($('input.inpt_urlServicioAutocompletarEtiquetas').val(), WSDataType.jsonp),
            metodo: 'AutoCompletar',
            //url: $('input.inpt_urlServicioAutocompletarEtiquetas').val() + "/AutoCompletar",
            //type: "POST",
            delay: 0,
            scroll: false,
            selectFirst: false,
            minChars: 1,
            width: 170,
            cacheLength: 0,
            extraParams: {
                pProyecto: $('input.inpt_proyID').val(),
                pTablaPropia: true,
                pFaceta: pClaveFaceta,
                pOrigen: '',
                pIdentidad: $('input.inpt_identidadID').val(),
                botonBuscar: control.id + 'botonBuscar'
            }
        }
    );
    if (control.attributes["onfocus"] != null) {
        control.attributes.removeNamedItem('onfocus');
    }

    pintarTagsInicio();
}

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
            //url: $('input.inpt_urlServicioAutocompletarEtiquetas').val() + "/AutoCompletarTipado",
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

    pintarTagsInicio();
}

function autocompletarSeleccionEntidad(control, grafo, entContenedora, propiedad, tipoEntidadSolicitada, propSolicitadas, extraWhere, idioma) {
    var limite = 10;

    if (extraWhere.indexOf('%7c%7c%7cLimite%3d') != -1) {
        limite = 200;
    }

    $(control).autocomplete(
        null,
        {
            url: $('input.inpt_urlServicioAutocompletar').val() + "/AutoCompletarSeleccEntDocSem",
            type: "POST",
            delay: 0,
            scroll: false,
            selectFirst: false,
            minChars: 1,
            /*width: 170,*/
            cacheLength: 0,
            matchCase: true,
            pintarConcatenadores: true,
            max: limite,
            extraParams: {
                pGrafo: grafo,
                pEntContenedora: entContenedora,
                pPropiedad: propiedad,
                pTipoEntidadSolicitada: tipoEntidadSolicitada,
                pPropSolicitadas: propSolicitadas,
                pControlID: urlEncode(control.id),
                pExtraWhere: extraWhere,
                pIdioma: idioma
            }
        }
    );
    if (control.attributes["onfocus"] != null) {
        control.attributes.removeNamedItem('onfocus');
    }
}

function autocompletarSeleccionEntidadGruposGnoss(control, pIdentidad, pOrganizacion, pProyecto, pTipoSolicitud) {
    var limite = 10;

    $(control).autocomplete(null, {
        //servicio: new WS($('input.inpt_urlServicioAutocompletar').val(), WSDataType.jsonp),
        //metodo: 'AutoCompletarSeleccEntPerYGruposGnoss',
        url: $('input.inpt_urlServicioAutocompletar').val() + "/AutoCompletarSeleccEntPerYGruposGnoss",
        type: "POST",
        delay: 0,
        scroll: false,
        selectFirst: false,
        minChars: 1,
        /*width: 170,*/
        cacheLength: 0,
        matchCase: true,
        pintarConcatenadores: true,
        max: limite,
        txtValoresSeleccID: control.id.replace("hack_", "selEnt_"),
        extraParams: {
            identidad: pIdentidad,
            organizacion: pOrganizacion,
            proyecto: pProyecto,
            tipoSolicitud: pTipoSolicitud
        }
    });

    $(control).result(function (event, data, formatted) {
        AgregarValorGrupoGnossAutocompletar(control.id, data);
    });

    if (control.attributes["onfocus"] != null) {
        control.attributes.removeNamedItem('onfocus');
    }
}

function autocompletarGrafoDependiente(control, pEntidad, pPropiedad, grafo, tipoEntDep, propDepende, entPropDepende) {
    EliminarValoresGrafoDependientes(pEntidad, pPropiedad, false, true);
    //var idControl = control.id.replace("hack_", "Campo_");
    //document.getElementById(idControl).value = '';

    //control.value = '';
    var idValorPadre = '';

    if (propDepende != '' && entPropDepende != '') {
        var idControlCampo = ObtenerControlEntidadProp(entPropDepende + ',' + propDepende, TxtRegistroIDs);
        idValorPadre = document.getElementById(idControlCampo).value;

        if (idValorPadre == '') {
            return;
        }
    }

    $(control).unautocomplete();

    $(control).autocomplete(
        null,
        {
            url: $('input.inpt_urlServicioAutocompletar').val() + "/AutoCompletarGrafoDependienteDocSem",
            type: "POST",
            delay: 0,
            scroll: false,
            selectFirst: false,
            minChars: 1,
            width: 170,
            cacheLength: 0,
            matchCase: true,
            extraParams: {
                pGrafo: grafo,
                pTipoEntDep: tipoEntDep,
                pIDValorPadre: idValorPadre,
                pControlID: urlEncode(control.id)
            }
        }
    );
    //    if (control.attributes["onfocus"] != null) {
    //        control.attributes.removeNamedItem('onfocus');
    //    }
}

function EliminarValoresGrafoDependientes(pEntidad, pPropiedad, pDeshabilitar, pRecursivo) {
    var idControlCampo = ObtenerControlEntidadProp(pEntidad + ',' + pPropiedad, TxtRegistroIDs);
    //var valorBorrar = document.getElementById(idControlCampo).value;
    document.getElementById(idControlCampo).value = '';

    var idControlAuto = idControlCampo.replace("Campo_", "hack_");
    document.getElementById(idControlAuto).value = '';


    //    if (valorBorrar != '') {
    //        var valoresGraf = document.getElementById(TxtValoresGrafoDependientes).value;
    //        var valoresGrafDef = valoresGraf.substring(0, valoresGraf.indexOf(valorBorrar));
    //        valoresGraf = valoresGraf.substring(valoresGraf.indexOf(valorBorrar));
    //        valoresGraf = valoresGraf.substring(valoresGraf.indexOf('|') + 1);
    //        valoresGrafDef += valoresGraf;
    //        document.getElementById(TxtValoresGrafoDependientes).value = valoresGrafDef;
    //    }

    document.getElementById(idControlAuto).disabled = (pDeshabilitar && !GetEsPropiedadGrafoDependienteSinPadres(pEntidad, pPropiedad));

    if (pRecursivo) {
        var pronEntDep = GetPropiedadesDependientes(pEntidad, pPropiedad);

        if (pronEntDep != null) {
            EliminarValoresGrafoDependientes(pronEntDep[1], pronEntDep[0], true, pRecursivo);
        }
    }
}

function HabilitarCamposGrafoDependientes(pEntidad, pPropiedad) {
    var pronEntDep = GetPropiedadesDependientes(pEntidad, pPropiedad);

    if (pronEntDep != null) {
        var idControlAuto = ObtenerControlEntidadProp(pronEntDep[1] + ',' + pronEntDep[0], TxtRegistroIDs).replace("Campo_", "hack_");
        document.getElementById(idControlAuto).disabled = false;
    }
}


function CrearAutocompletarTags(control, pUrlAutocompletar, pProyectoID, pEsMyGnoss, pEstaEnProyecto, pEsUsuarioInvitado, pIdentidadID) {
    $(control).autocomplete(
        null,
        {
            //servicio: new WS(pUrlAutocompletar, WSDataType.jsonp),
            //metodo: 'AutoCompletarFacetas',
            url: pUrlAutocompletar + "/AutoCompletarFacetas",
            type: "POST",
            delay: 300,
            minLength: 4,
            multiple: true,
            scroll: false,
            selectFirst: false,
            minChars: 4,
            width: 300,
            cacheLength: 0,
            extraParams: {
                proyecto: pProyectoID,
                bool_esMyGnoss: pEsMyGnoss,
                bool_estaEnProyecto: pEstaEnProyecto,
                bool_esUsuarioInvitado: pEsUsuarioInvitado,
                identidad: pIdentidadID,
                nombreFaceta: 'sioc_t:Tag',
                orden: '',
                parametros: '',
                tipo: '',
                perfil: '',
                organizacion: '',
                filtrosContexto: '',
                languageCode: $('input.inpt_Idioma').val()
            }
        }
    );
    if (control.attributes["onfocus"] != null) {
        control.attributes.removeNamedItem('onfocus');
    }

    pintarTagsInicio();
}

/////////////ETIQUETADO AUTOMATICO//////////////////        
function EtiquetadoAutomaticoDeRecursos(titulo, descripcion, txtHack, pEsPaginaEdicion) {
    //var servicio = new WS($('input.inpt_urlEtiquetadoAutomatico').val(), WSDataType.jsonp);
    var servicio = $('input.inpt_urlEtiquetadoAutomatico').val();
    var params = {};
    params['ProyectoID'] = $('input.inpt_proyID').val();
    var numMax = 15000;
    titulo = urlEncode(titulo);
    descripcion = urlEncode(descripcion);
    if (descripcion.length < numMax) {
        var metodo = 'SeleccionarEtiquetas';
        params['titulo'] = titulo;
        params['descripcion'] = descripcion;
        /*servicio.call(metodo, params, function (data) {
            procesarTags(data, txtHack, pEsPaginaEdicion);
        });*/
        $.post(obtenerUrl($('input.inpt_urlEtiquetadoAutomatico').val()) + "/" + metodo, params, function (data) {
            procesarTags(data, txtHack, pEsPaginaEdicion);
        });
    } else {
        guid = guidGenerator().toLowerCase();
        params['identificadorPeticion'] = guid;
        EtiquetadoAutomaticoDeRecursosMultiple(titulo, descripcion, numMax, params, txtHack, pEsPaginaEdicion);
    }
}

function EtiquetadoAutomaticoDeRecursosMultiple(titulo, descripcion, numMax, params, txtHack, pEsPaginaEdicion) {
    //var servicio = new WS($('input.inpt_urlEtiquetadoAutomatico').val(), WSDataType.jsonp);
    var servicio = $('input.inpt_urlEtiquetadoAutomatico').val();
    if (descripcion.length <= numMax) {
        params['fin'] = "true";
        numMax = descripcion.length;
    } else {
        params['fin'] = "false";
    }
    var metodo = 'SeleccionarEtiquetasMultiple';
    var textoEnviar = descripcion.substring(0, numMax);

    var ultimPorcentaje = textoEnviar.lastIndexOf('%');
    if (params['fin'] == "false" && ultimPorcentaje > 0) {
        textoEnviar = textoEnviar.substring(0, ultimPorcentaje);
    }

    params['titulo'] = titulo;
    params['descripcion'] = textoEnviar;
    if (params['fin'] == "false" && ultimPorcentaje > 0) {
        descripcion = descripcion.substring(ultimPorcentaje);
    } else {
        descripcion = descripcion.substring(numMax);
    }

    $.post(obtenerUrl($('input.inpt_urlEtiquetadoAutomatico').val()) + "/" + metodo, params, function (data) {
        var siguiente = data.siguiente;

        if (siguiente == true) {
            EtiquetadoAutomaticoDeRecursosMultiple(titulo, descripcion, numMax, params, txtHack, pEsPaginaEdicion);
        }
        else {
            procesarTags(data, txtHack, pEsPaginaEdicion);
        }
    });

    /*servicio.call(metodo, params, function (data) {
        var siguiente = data.siguiente;

        if (siguiente == true) {
            EtiquetadoAutomaticoDeRecursosMultiple(titulo, descripcion, numMax, params, txtHack, pEsPaginaEdicion);
        }
        else {
            procesarTags(data, txtHack, pEsPaginaEdicion);
        }
    });*/
}

/**
 * Método que se ejecutará para procesar y mostrar los Tags sugeridos.
 * Ej: En el momento de creación de un recurso
 * @param {any} data
 * @param {any} txtHack
 * @param {any} pEsPaginaEdicion
 */
function procesarTags(data, txtHack, pEsPaginaEdicion) {
    var directos = data.directas.trim();
    var propuestos = data.propuestas.trim();
    var enlaces = data.enlaces.trim();

    if (pEsPaginaEdicion) {
        propuestos = directos + propuestos;
        directos = "";
    }

    var directosAnt = "";
    var propuestosAnt = "";

    directosAnt = txtHack.val().split('[&]')[0];
    if (txtHack.val().indexOf('[&]') != -1) {
        propuestosAnt = txtHack.val().split('[&]')[1];
    }


    // Cambiar por el nuevo Front
    //if (!$('#' + txtTagsID).parent().next().hasClass('propuestos')) {
    if (!$('#' + txtTagsID).parent().parent().parent().children().hasClass("propuestos")) {
        // Cambiar el contenedor donde se establecer�n las etiquetas propuestas para el nuevo Front
        // Original -> $('#' + txtTagsID).parent().after("<div class='propuestos' style='display:none'><p>" + form.tagsPropuestos + "</p><span class='contenedor'></span></div>");
        //$('#' + txtTagsID).parent().after('<div class="propuestos"><label class="control-label d-block mb-2">' + form.tagsPropuestos + '</label></p><span class="contenedor tag-list sugerencias"></span></div>');
        $('#' + txtTagsID).parent().parent().after('<div class="propuestos"><label class="control-label d-block mb-2">' + form.tagsPropuestos + '</label></p><span class="contenedor tag-list sugerencias"></span></div>');
    }

    $('#' + txtTagsID + "Enlaces").val(enlaces);

    // Añadir Tags directos según el contenido introducido (Título, Descripción...)
    if (directos != "" || txtHack.val() != "") {
        directos = AgregarTagsDirectosAutomaticos(directos, directosAnt, txtTagsID);
    }

    if (propuestos != "" || txtHack.val() != "") {
        propuestos = AgregarTagsPropuestosAutomaticos(propuestos, propuestosAnt, txtTagsID);
    }
    // Incluir el contenido a Hack solo si hay contenido
    txtHack.val(directos + '[&]' + propuestos);
}

function AgregarTagsDirectosAutomaticos(pListaTagsNuevos, pListaTagsViejos, pTagsID) {
    var tagsDescartados = "";

    if ($('#' + pTagsID + '_Hack').parent().next().next().hasClass('descartados')) {
        tagsDescartados = $('#' + pTagsID + '_Hack').parent().next().next().find('#txtHackDescartados').val();
    }

    var tagsManuales = "";
    var tagsAgregados = "";

    var tagsActuales = $('#' + pTagsID + '_Hack').val().split(',');

    //Recorrro los tags de la caja y guardo los manuales
    for (var j = 0; j < tagsActuales.length; j++) {
        var tag = tagsActuales[j].trim();

        var estaYaAgregada = pListaTagsViejos.indexOf(', ' + tag + ',') != -1;
        estaYaAgregada = estaYaAgregada || pListaTagsViejos.substring(0, tag.length + 1) == tag + ',';

        if (tag != '' && !estaYaAgregada) {
            tagsManuales += tag + ',';
        }
    }

    var tags = pListaTagsNuevos.trim().split(',');

    //Recorro los tags nuevos
    for (var i = 0; i < tags.length; i++) {
        var tag = tags[i].trim();

        var estaYaDescartada = tagsDescartados.indexOf(',' + tag + ',') != -1;
        estaYaDescartada = estaYaDescartada || tagsDescartados.substring(0, tag.length + 1) == tag + ',';

        var estaYaAgregada = tagsManuales.indexOf(',' + tag + ',') != -1;
        estaYaAgregada = estaYaAgregada || tagsManuales.substring(0, tag.length + 1) == tag + ',';

        if (tag != '' && !estaYaAgregada && !estaYaDescartada) {
            tagsAgregados += tag + ',';
        }
    }

    $('#' + pTagsID).val(tagsManuales + tagsAgregados);

    LimpiarTags($('#' + pTagsID));
    PintarTags($('#' + pTagsID));

    return tagsAgregados;
}

/**
 * Agregará la lista de Tags propuestos que se han obtenido una vez se ha introducido una gran descripción en el momento de la creación de un recurso.
 * @param {any} pListaTagsNuevos: Lista de Tags propuestos separados por comas que se deberán mostrar al usuario
 * @param {any} pListaTagsViejos: Lista de Tags anteriores a los propuestos.
 * @param {any} pTagsID: ID del input donde el usuario puede ir registrando nuevas Tags. En él también se encuentra el contenedor de tags que el usuario ha ido metiendo.
 */
function AgregarTagsPropuestosAutomaticos(pListaTagsNuevos, pListaTagsViejos, pTagsID) {
    var tagsManuales = "";
    var tagsAgregados = "";

    var tagsActuales = $('#' + pTagsID + '_Hack').val().split(',');

    //Recorrro los tags de la caja y guardo los manuales
    for (var i = 0; i < tagsActuales.length; i++) {
        var tag = tagsActuales[i].trim();
        if (tag != '') {
            tagsManuales += tag + ',';
        }
    }

    // Nuevo Front
    //var divPropuestos = $('#' + pTagsID).parent().next().find('.contenedor');
    var divPropuestos = $('#' + pTagsID).parent().parent().parent().find(".propuestos > .contenedor");


    //Recorro los tags viejos y si no estan en los nuevos, los quito
    divPropuestos.find('.tag').each(function () {
        var tag = $(this).attr('title');

        var estaYaAgregada = pListaTagsViejos.indexOf(',' + tag + ',') != -1;
        estaYaAgregada = estaYaAgregada || pListaTagsViejos.substring(0, tag.length + 1) == tag + ',';

        if (tag != '' && estaYaAgregada) {
            $(this).remove();
        }
    });

    var tags = pListaTagsNuevos.trim().split(',');

    if (tags != '' && tags.length > 0) {
        $('#' + pTagsID).parent().next().css('display', '');
    }

    //Recorro los tags nuevos
    for (i = 0; i < tags.length; i++) {
        var tag = tags[i].trim();
        if (tag != '') {
            tagsAgregados += tag + ',';
            var estilo = "";

            var estaYaAgregada = tagsManuales.indexOf(',' + tag + ',') != -1;
            estaYaAgregada = estaYaAgregada || tagsManuales.substring(0, tag.length + 1) == tag + ',';

            if (estaYaAgregada) {
                estilo = "style=\"display:none;\"";
            }

            // Crea la tag que se añadirá como propuesta
            var htmlTag = '';
            htmlTag += `<div class="tag" ${estilo} title="${tag}">`;
            htmlTag += `<div class="tag-wrap">`;
            htmlTag += `<span class="tag-text">${tag}</span>`;
            htmlTag += `<span class="tag-remove material-icons add">add</span>`;
            htmlTag += `</div>`;
            htmlTag += `</div>`;

            // Añadir el Tag al contenedor de Tags propuestas
            //divPropuestos.append("<div " + estilo + " class=\"tag\" title=\"" + tag + "\">" + tag + "<a class=\"add\" ></a></div>");
            divPropuestos.append(htmlTag);
        }
    }

    /**
    * Asignación a cada item generado la opción de "Añadir" el Tag al recurso. Al pulsar en (+), añadir el Tag al contendor de Tags del usuario           
    */
    $(divPropuestos.find('.tag-wrap .add')).each(function () {
        $(this).bind('click', function (evento) {
            var tag = $(this).parent().parent().attr('title');
            $('#' + pTagsID).val(tag);
            $(this).parent().parent().css('display', 'none');
            PintarTags($('#' + pTagsID));
        });
    });

    return tagsAgregados;
}

function ActualizacionDelDiccionarioEtiquetas() {
    var servicio = new WS($('input.inpt_urlEtiquetadoAutomatico').val(), WSDataType.jsonp);
    var metodo = 'ActualizarDiccionarioEtiquetas';
    var params = {};
    var textoPropuesto = $('#' + txtHackTagsTituloID).val() + $('#' + txtHackTagsDescripcionID).val();
    textoPropuesto = textoPropuesto.replace(/\[&\]\[&\]/g, ',').replace(/\[&\]/g, '');
    params['textoPropuesto'] = textoPropuesto;
    params['textoElegidoUsuario'] = $('#' + txtTagsID).val();
    params['ProyectoID'] = $('input.inpt_proyID').val();
    servicio.call(metodo, params, function (data) {
    });
}

function CargaDelDiccionarioEtiquetas() {
    var servicio = new WS($('input.inpt_urlEtiquetadoAutomatico').val(), WSDataType.jsonp);
    var metodo = 'CargarDiccionarioEtiquetas';
    var params = {};
    params['ProyectoID'] = $('input.inpt_proyID').val();
    servicio.call(metodo, params, function (data) {
    });
}

//Añado al tipo string de javascript las funciones startsWith y EndsWith 
String.prototype.endsWith = function (str) { return (this.match(str + "$") == str) };

String.prototype.startsWith = function (str) { return (this.match("^" + str) == str) };

function SepararFiltro(filtro) {
    var array = new Array(4);
    var indiceFiltro = filtro.indexOf('=') + 1;
    array[0] = filtro.replace(':', '_'); //id
    array[1] = filtro.substring(indiceFiltro, filtro.length); //title
    array[2] = array[1]; //innerHTML
    array[3] = 'javascript:AgregarFacetas(filtro, null);'; //onclick
    return array;
}

function Login(usuario, password) {
    document.getElementById('usuario').value = usuario;
    document.getElementById('password').value = password;
    document.getElementById('formLogin').submit();
}

function LoginConClausulasRegistro(usuario, password, condicionesUso) {
    document.getElementById('usuario').value = usuario;
    document.getElementById('password').value = password;
    document.getElementById('clausulasRegistro').value = condicionesUso;
    document.getElementById('formLogin').submit();
}

function CambiarUrlRedirectLoginAUrlActual() {
    var accion = $('#formLogin').attr('action');

    var indiceRedirect = accion.indexOf('&redirect');
    var indiceFinRedirect = accion.indexOf('&', indiceRedirect + 1);
    var parametroRedirect = accion.substring(indiceRedirect, indiceFinRedirect);

    accion = accion.replace(parametroRedirect, '&redirect=' + document.location.href);

    $('#formLogin').attr('action', accion);
}

function ValidarLogin(arg, context) {
    if (arg == 'entrar') {
        location.reload(true);
    }
    else if (arg == 'mostrarError') {
        $('#errorLogin').html('<p>' + form.errorLogin + '</p>');
        $('#errorLogin').css('display', '');
        return false;
    }
    else {
        window.location = arg;
    }
}

$(document).ready(function () {
    $(".loginButton").click(function () {
        $("#botonLogin").toggleClass("btLogin");
    });

    $("#txtUsuario").keydown(function (event) {
        if (event.which || event.keyCode) {
            if ((event.which == 13) || (event.keyCode == 13)) {
                return false;
            }
        }
    });

    $("#txtContraseña").keydown(function (event) {
        if (event.which || event.keyCode) {
            if ((event.which == 13) || (event.keyCode == 13)) {
                controlarEntradaMaster();
                return false;
            }
        }
    });

    $("#formularioLoginHeader input.submit").click(function () {
        controlarEntradaMaster();
    });

    $(".registroRedesSociales ul a").click(function () {
        window.open(this.href, 'auth', 'width=600,height=500');
        return false;
    });
});



function beginRequestHandle(sender, Args) {
    if (Args._postBackElement != null && Args._postBackElement.id != "ctl00_ctl00_CPH1_CPHBandejaContenido_TimerRefresco") {
        MostrarUpdateProgress();
    }
}

function endRequestHandle(sender, Args) {
    OcultarUpdateProgress();
}

var timeoutUpdateProgress;

function MostrarUpdateProgress() {    
    // Quitar la opción de que a los 15 segundos desaparezca el "Loading"
    //MostrarUpdateProgressTime(15000)
    MostrarUpdateProgressTime();
}

function MostrarUpdateProgressTime(time) {
    if ($('#mascaraBlanca').length > 0) {
        $('body').addClass('mascaraBlancaActiva');

        if (time > 0) {
            timeoutUpdateProgress = setTimeout("OcultarUpdateProgress()", time);
        }
    }
}

function OcultarUpdateProgress() {
    if ($('#mascaraBlanca').length > 0) {
        $('.popup').hide();
        $('#mascaraBlanca').hide()
        clearTimeout(timeoutUpdateProgress);
    }
}

$(function () {
    if ($('#inpt_Cookies').length > 0 && $('#inpt_Cookies').val().toLowerCase() == "true") {
        var obj = $(document);
        var obj_top = obj.scrollTop();
        obj.scroll(function () {
            var obj_act_top = $(this).scrollTop();
            if (obj_act_top != obj_top) {
                AceptarCookies();
            }
            obj_top = obj_act_top;
        });
    }
});

var cookiesAceptadas = false;

function AceptarCookies() {
    if (!cookiesAceptadas) {
        cookiesAceptadas = true;
        //WebForm_DoCallback('__Page', 'aceptamosCookies', ReceiveServerData, '', null, false);
    }
}

//$(document).ready(function () {
//    $('#formLogin').attr('action', $('input.inpt_UrlLogin').val());
//});


function controlarEntradaMaster() {
    var user = $('#txtUsuario').val();
    var pass = $('#txtContraseña').val();
    if (user.length > 0 && pass.length > 0) {
        Login(user, pass);
    } else {
        $('#errorLogin').html('<p>' + form.errorLoginVacio + '</p>');
        $('#errorLogin').css('display', '');
    }
}


//////////////////////////////////////////////////////////////////////////////////////////

//if(filtro.length > 1 || document.location.href.indexOf('/tag/') > -1){parametrosFacetas = " + "'AgregarFacetas|" + arg + "'; var parametrosResultados = " + "'ObtenerResultados|" + arg + "';
//var displayNone = '';
//if(HayFiltrosActivos(filtro)){
//document.getElementById('" + this.divFiltros.ClientID + "').setAttribute('style', 'padding-top:0px !important; ' + displayNone + ' margin-top:10px; ');}
//sb.AppendLine(Page.ClientScript.GetCallbackEventReference(this.Page, "parametrosResultados", "ReceiveServerData", String.Empty) + ";}
//else{
//document.getElementById('" + this.divFiltros.ClientID + "').setAttribute('style', 'padding-top:0px !important; margin-top:10px; display:none !important;');
//sb.AppendLine(Page.ClientScript.GetCallbackEventReference(this.Page, "parametrosFacetas", "ReceiveServerData", String.Empty) + ";}
//return false;
//}

/**
 * Comportamiento de facetas pop up para que se muestren todas las categorías de tipo "Tesauro" una vez se pulse en el botón "Ver más".
 * */
const comportamientoFacetasPopUpPlegado = {
    init: function () {
        this.config();
        // Faceta ID de la que se desean obtener sus subfacetas
        this.facetaActual = '';
    },
    config: function () {
        const that = this;

        // Posibles traducciones que se utilicen en los modales de Facetas
        this.FacetTranslations = {
            en: {
                searchBy: "Search by ",
            },
            es: {
                searchBy: "Buscar por ",
            },
        };

        // Lógica del Modal
        // Abrir modal        
        $(document).on('show.bs.modal', '.modal-resultados-lista', function (e) {
            // Faceta seleccionada
            that.facetaActual = $(e.relatedTarget).data('facetkey');
            that.facetaActualName = $(e.relatedTarget).data('facetname');
            // Registrar el id del modal abierto (para cargar los datos en el modal correspondiente)
            that.$modalLoaded = $(`#${e.target.id}`);
            // Clonar la faceta/mostrarlo en modal
            that.clonarFaceta();
            // Configurar eventos mostrados en el modal
            that.configEvents();
        });

        // Cerrar modal
        $(document).on('hidden.bs.modal', '.modal-resultados-lista', function (e) {
            // Vaciar el modal                 
            that.$modalLoaded.find('.listadoFacetas').empty();
            // Vaciar el título o cabecera titular
            that.$modalLoaded.find(".loading-modal-facet-title").text('');
        });
    },

    configEvents: function () {
        const that = this;

        // Buscador o filtrado de facetas cuando se inicie la escritura en el Input buscador dentro del modal         
        this.$modalLoaded.find(".buscador-coleccion .buscar .texto").keyup(function () {
            that.textoActual = that.eliminarAcentos($(this).val());
            that.filtrarElementos()
        });

        // Configurar click de la faceta
        this.$modalLoaded.find(".faceta").click(function (e) {
            AgregarFaceta($(this).attr("name"));
            // Cerrar modal
            that.$modalLoaded.modal('toggle');
            e.preventDefault();
        });
    },

    /**
     * Buscar las facetas y clonarlas en el modal    
     */
    clonarFaceta: function () {
        const that = this;

        // Buscar el panel de tipo faceta seleccionada
        this.facetaActual = this.facetaActual.replace(':', '_');
        this.panelFacetaActual = $(`#${this.facetaActual}`);

        // Establecer "Título" en la cabecera del modal y en el header      
        that.$modalLoaded.find('.loading-modal-facet-title').text(this.panelFacetaActual.find('.faceta-title').text());
        // No mostrar el título de la faceta
        // that.$modalLoaded.find('.faceta-title').text(this.panelFacetaActual.find('.faceta-title').text());

        // Recoger el html del panelFacetaActual para pintarlo donde corresponde
        const facetItems = this.panelFacetaActual.find('.listadoFacetas').children().clone();
        // Sustituir los plegar-desplegar correspondientes
        facetItems.find('.desplegarSubFaceta .material-icons').text('expand_more');

        that.$modalLoaded.find(`.listadoFacetas`).append(facetItems);

        // Placeholder del buscador
        if (configuracion.idioma == 'en') {
            that.$modalLoaded.find(".buscador-coleccion .buscar .texto").attr("placeholder", `${this.FacetTranslations.en.searchBy}${this.facetaActualName}`);
        } else {
            that.$modalLoaded.find(".buscador-coleccion .buscar .texto").attr("placeholder", `${this.FacetTranslations.es.searchBy}${this.facetaActualName}`);
        }

        // Activar plegado de facetas
        plegarSubFacetas.init();
    },

    /**
    * Comprobará la tecla pulsada, y si no se encuentra entre las excluidas, dará lo introducido por válido devolviendo true
    * @param {any} event: Evento o tecla pulsada en el teclado
    */
    validarKeyPulsada: function (event) {
        console.log(event.keycode);
        const keyPressed = this.ignoreKeysToBuscador.find(
            (key) => key == event.keyCode
        );
        if (keyPressed) {
            return false;
        }
        return true;
    },

    /**     
     * @param {any} texto: Texto introducido para buscar facetas (input)
     */
    eliminarAcentos: function (texto) {
        var ts = "";
        for (var i = 0; i < texto.length; i++) {
            var c = texto.charCodeAt(i);
            if (c >= 224 && c <= 230) {
                ts += "a";
            } else if (c >= 232 && c <= 235) {
                ts += "e";
            } else if (c >= 236 && c <= 239) {
                ts += "i";
            } else if (c >= 242 && c <= 246) {
                ts += "o";
            } else if (c >= 249 && c <= 252) {
                ts += "u";
            } else {
                ts += texto.charAt(i);
            }
        }
        return ts;
    },

    filtrarElementos: function () {
        const that = this;
        /* Controlar los items a filtrar */
        let itemsListado = "";
        if (this.$modalLoaded.find('.facetas-carpeta li').length == 0) {
            itemsListado = this.$modalLoaded.find('.listadoFacetas li');
        } else {
            itemsListado = this.$modalLoaded.find('.facetas-carpeta li');
        }
        itemsListado.show();
        if (that.textoActual == '') {
            // $('.boton-desplegar').removeClass('mostrar-hijos');
        } else {
            itemsListado.each(function () {
                var boton = $(this).find('.desplegarSubFaceta');
                //boton.removeClass('mostrar-hijos');
                var nombreCat = $(this).find('.textoFaceta').text();
                //boton.trigger('click');
                $(`.js-desplegar-facetas-modal`).trigger('click');
                if (nombreCat.toLowerCase().indexOf(that.textoActual.toLowerCase()) < 0) {
                    $(this).hide();
                }
                var categoriaHijo = $(this).find('ul').children('li');
                categoriaHijo.each(function () {
                    var nombreCatHijo = $(this).find('.textoFaceta').text();
                    if (nombreCatHijo.toLowerCase().indexOf(that.textoActual.toLowerCase()) < 0) {
                        $(this).hide();
                    }
                });
            });
        }
    },
};


/**
 * Comportamiento de facetas pop up para que se carguen una vez se pulse en el botón de "Ver mÃ¡s".
 * Se harÃ¡ la llamada para la obtención de Facetas y se muestran en un panel modal.  
 * */
const comportamientoFacetasPopUp = {
    init: function () {
        // Objetivo Observable
        const that = this;

        if ($("#panFacetas").length > 0 || $("#divFac").length > 0) {
            const target = $("#panFacetas").length == 0 ? $("#divFac")[0] : $("#panFacetas")[0];
            // Teclas que se ignorarán si se pulsan en el input para que no dispare la búsqueda (Flechas, Supr, Windows, Ctrol, Alt, Bloq. Mayus, Inicio, Alt, Escape)
            this.ignoreKeysToBuscador = [37, 38, 39, 40, 46, 91, 17, 18, 20, 36, 18, 27];
            this.timeWaitingForUserToType = 400; // Esperar 1 segundos a si el usuario ha dejado de escribir para iniciar búsqueda

            if (target != undefined) {
                // Creación del observador
                let observer = new MutationObserver(function (mutations) {
                    mutations.forEach(function (mutation) {
                        var newNodes = mutation.addedNodes; // DOM NodeList
                        if (newNodes !== null) { // Si se añaden nuevos nodos a DOM
                            var $nodes = $(newNodes); // jQuery set
                            $nodes.each(function () {
                                const $node = $(this);
                                // Configurar el servicio para elementos que sean de tipo 'verMasFacetasModal'                        
                                // Inicializamos el config del comportamiento FacetasPopUp si hay botones venidos de forma asíncrona de Facetas.
                                //if ($node.hasClass('verMasFacetasModal')) {
                                if ($node.find('.verMasFacetasModal').length > 0) {
                                    that.config();
                                    that.IndiceFacetaActual = 0;
                                }
                            });
                        }
                    });
                });

                // Configuración del observador:
                var config = {
                    attributes: true,
                    childList: true,
                    subtree: true,
                    characterData: true
                };

                // Activación del observador para panel de Facetas (cargadas asíncronamente) con su configuración
                observer.observe(target, config);
                // Carga manual inicial de "verMasFacetasModal"
                this.config();
                this.IndiceFacetaActual = 0;
            }

            // Configuración de los modales
            this.configModals();
        }
    },
    config: function () {
        //1Âº Nombre de la faceta
        //2Âº Titulo del buscador
        //3Âº True para ordenar por orden alfabÃ©tico, False para utilizar el orden por defecto
        var that = this;

        /* Esquema que tendrÃ¡
        this.facetasConPopUp = [
            [
                "sioc_t:Tag",
                "Busca por TAGS",
                true,
            ], //Tags            
        ];*/

        // Array de Facetas que tendrÃ¡ visualización con PopUp
        this.facetasConPopUp = [];

        // Recoger todos posibles botones de "Ver mÃ¡s" para construir un array de facetasConPopUp                
        this.facetsArray = $('.verMasFacetasModal');

        // Posibles traducciones que se utilicen en los modales de Facetas
        this.FacetTranslations = {
            en: {
                searchBy: "Search by ",
                loading: "Loading ...",
            },
            es: {
                searchBy: "Buscar por ",
                loading: "Cargando ...",
            },
        };

        $(this.facetsArray).each(function () {
            // Construir el objeto Array de la faceta para luego añadirlo a facetasConPopUp
            const facetaArray = [
                $(this).data('facetkey'), // Faceta para bÃºsqueda
                $(this).data('facetname'), // TÃ­tulo de la faceta
                true,                     // Ordenar por orden alfabÃ©tico,                
            ];
            that.facetasConPopUp.push(facetaArray);
        });

        for (i = 0; i < this.facetasConPopUp.length; i++) {
            // Faceta de tipo de bÃºsqueda
            var faceta = this.facetasConPopUp[i][0];
            var facetaSinCaracteres = faceta
                .replace(/\@@@/g, "---")
                .replace(/\:/g, "--");
        }
    },
    /**
    * Configuración de los comportamiento modales (Abrir y Cerrar modales)
    *
    */
    configModals: function () {
        const that = this;
        // Lógica del Modal
        // Abrir modal        
        $(document).on('show.bs.modal', '.modal-resultados-paginado', function (e) {
            // Conocer la faceta seleccionada
            const facetaActual = $(e.relatedTarget).data('facetkey');

            // Recorrer y buscar la posición en la que se encuentra la faceta seleccionada
            for (let i = 0; i < that.facetasConPopUp.length; i++) {
                if (that.facetasConPopUp[i][0] == facetaActual)
                    // Guardar el índice de facetasConPopUp
                    that.IndiceFacetaActual = i;
            }

            // Registrar el id del modal abierto (para cargar los datos en el modal correspondiente)
            that.$modalLoaded = $(`#${e.target.id}`);
            that.cargarFaceta();
        });

        // Cerrar modal
        $(document).on('hidden.bs.modal', '.modal-resultados-paginado', function (e) {
            // Vaciar el modal                 
            that.$modalLoaded.find('.resultados-wrap').empty();
            // Eliminar navegador
            that.$modalLoaded.find('.action-buttons-resultados').remove();
            // Volver a mostrar el "Loading"
            that.$modalLoaded.find('.modal-resultados-paginado').find('.loading-modal-facet').removeClass('d-none');
            // Establecer el tÃ­tulo o cabecera titular del modal original
            if (configuracion.idioma == 'en') {
                that.$modalLoaded.find(".loading-modal-facet-title").text(`${that.FacetTranslations.en.loading}`);
            } else {
                that.$modalLoaded.find(".loading-modal-facet-title").text(`${that.FacetTranslations.es.loading}`);
            }
        });
    },

    eliminarAcentos: function (texto) {
        var ts = "";
        for (var i = 0; i < texto.length; i++) {
            var c = texto.charCodeAt(i);
            if (c >= 224 && c <= 230) {
                ts += "a";
            } else if (c >= 232 && c <= 235) {
                ts += "e";
            } else if (c >= 236 && c <= 239) {
                ts += "i";
            } else if (c >= 242 && c <= 246) {
                ts += "o";
            } else if (c >= 249 && c <= 252) {
                ts += "u";
            } else {
                ts += texto.charAt(i);
            }
        }
        return ts;
    },

    cargarFaceta: function () {
        var that = this;
        var FacetaActual = that.facetasConPopUp[that.IndiceFacetaActual][0];
        var facetaSinCaracteres = FacetaActual.replace(/\@@@/g, "---").replace(
            /\:/g,
            "--"
        );
        this.paginaActual = 1;
        this.textoActual = "";
        this.fin = true;
        this.buscando = false;
        this.arrayTotales = null;
        var numFacetasTotales = 0;

        // Placeholder del buscador
        if (configuracion.idioma == 'en') {
            that.$modalLoaded.find(".buscador-coleccion .buscar .texto").attr("placeholder", `${this.FacetTranslations.en.searchBy}${that.facetasConPopUp[that.IndiceFacetaActual][1]}`);
        } else {
            that.$modalLoaded.find(".buscador-coleccion .buscar .texto").attr("placeholder", `${this.FacetTranslations.es.searchBy}${that.facetasConPopUp[that.IndiceFacetaActual][1]}`);
        }

        this.textoActual = "";

        // Configuración del servicio para llamar a las facetas deseadas
        var metodo = "CargarFacetas";
        var params = {};
        params["pProyectoID"] = $("input.inpt_proyID").val();
        params["pEstaEnProyecto"] =
            $("input.inpt_bool_estaEnProyecto").val() == "True";
        params["pEsUsuarioInvitado"] =
            $("input.inpt_bool_esUsuarioInvitado").val() == "True";
        params["pIdentidadID"] = $("input.inpt_identidadID").val();
        params["pParametros"] =
            "" +
            replaceAll(
                replaceAll(
                    replaceAll(
                        urlDecode(ObtenerHash2()).replace(/&/g, "|").replace("#", ""),
                        "%",
                        "%25"
                    ),
                    "#",
                    "%23"
                ),
                "+",
                "%2B"
            );
        params["pLanguageCode"] = $("input.inpt_Idioma").val();
        params["pPrimeraCarga"] = false;
        params["pAdministradorVeTodasPersonas"] = false;
        params["pTipoBusqueda"] = tipoBusqeda;
        params["pGrafo"] = grafo;
        params["pFiltroContexto"] = filtroContexto;
        params["pParametros_adiccionales"] =
            parametros_adiccionales + "|NumElementosFaceta=10000|";
        params["pUbicacionBusqueda"] = ubicacionBusqueda;
        params["pNumeroFacetas"] = -1;
        params["pUsarMasterParaLectura"] = bool_usarMasterParaLectura;
        params["pFaceta"] = FacetaActual;

        // Buscador o filtrado de facetas cuando se inicie la escritura en el Input buscador dentro del modal                
        that.$modalLoaded.find(".buscador-coleccion .buscar .texto")
            .keyup(function () {
                that.textoActual = that.eliminarAcentos($(this).val());
                that.paginaActual = 1;
                that.buscarFacetas();
            })
            .on('paste', function () {
                const input = $(this);
                setTimeout(function () {
                    input.keyup();
                }, 200);

            });


        // Petición al servicio para obtención de Facetas                
        $.post(obtenerUrl($('input.inpt_UrlServicioFacetas').val()) + "/" + metodo, params, function (data) {
            var htmlRespuesta = $("<div>").html(data);
            that.arrayTotales = new Array($(htmlRespuesta).find(".faceta").length);
            numFacetasTotales = that.arrayTotales;
            var i = 0;
            $(htmlRespuesta)
                .find(".faceta")
                .each(function () {
                    that.arrayTotales[i] = new Array(2);
                    that.arrayTotales[i][0] = that.eliminarAcentos(
                        $(this).text().toLowerCase()
                    );
                    that.arrayTotales[i][1] = $(this);
                    i++;
                });

            //Ordena por orden alfabÃ©tico
            if (that.facetasConPopUp[that.IndiceFacetaActual][2]) {
                that.arrayTotales = that.arrayTotales.sort(function (a, b) {
                    if (a[0] > b[0]) return 1;
                    if (a[0] < b[0]) return -1;
                    return 0;
                });
            }

            that.paginaActual = 1;
            // Header navegación de resultados de facetas
            const actionButtonsResultados = `<div class="action-buttons-resultados">
                                                <ul class="no-list-style">
                                                    <li class="js-anterior-facetas-modal">
                                                        <span class="material-icons">navigate_before</span>
                                                        <span class="texto">Anteriores</span>
                                                    </li>
                                                    <li class="js-siguiente-facetas-modal">
                                                        <span class="texto">Siguientes</span>
                                                        <span class="material-icons">navigate_next</span>
                                                    </li>
                                                </ul>
                                            </div>`;

            // Añadir al header de navegación de facetas (Anterior)
            that.$modalLoaded.find(".indice-lista.no-letra").prepend(actionButtonsResultados);

            // Configuración de los botones (Anterior/Siguiente)
            $(".js-anterior-facetas-modal").click(function () {

                if (!that.buscando && that.paginaActual > 1) {
                    that.buscando = true;
                    that.paginaActual--;
                    var hacerPeticion = true;
                    //$(".indice-lista .js-anterior-facetas-modal").hide();
                    $(".indice-lista ul").animate(
                        {
                            /*marginLeft: 30,
                            opacity: 0,*/
                        },
                        200,
                        function () {
                            if (hacerPeticion) {
                                that.buscarFacetas();
                                hacerPeticion = false;
                            }
                            $(".indice-lista ul").css({ /*marginLeft: -30*/ });
                            $(".indice-lista ul").animate(
                                {
                                    /*marginLeft: 20,
                                    opacity: 1,*/
                                },
                                200,
                                function () {
                                    $(".js-anterior-facetas-modal").show();
                                    // Left Animation complete.
                                }
                            );
                        }
                    );
                }
            });

            // Configuración del clicks de navegación (Siguiente)            
            $(".js-siguiente-facetas-modal").click(function () {

                if (!that.buscando && !that.fin) {
                    that.buscando = true;
                    that.paginaActual++;
                    var hacerPeticion = true;
                    //$(".indice-lista .js-siguiente-facetas-modal").hide();
                    $(".indice-lista ul").animate(
                        {
                            /*marginLeft: 30,
                            opacity: 0,*/
                        },
                        200,
                        function () {
                            if (hacerPeticion) {
                                that.buscarFacetas();
                                hacerPeticion = false;
                            }
                            $(".indice-lista ul").css({ /*marginLeft: -30*/ });
                            $(".indice-lista ul").animate(
                                {
                                    /*marginLeft: 20,
                                    opacity: 1,*/
                                },
                                200,
                                function () {
                                    $(".js-siguiente-facetas-modal").show();
                                    // Right Animation complete.
                                }
                            );
                        }
                    );
                }
            });
            // Buscar facetas y mostrarlas
            that.buscarFacetas();
        });
        // Si hay más de 1000 facetas, en lugar de buscar entre las traidas, las pedimos al servicio autocompletar
        if (numFacetasTotales < 1000) {
            that.$modalLoaded.find(".buscador-coleccion .buscar .texto").keyup(function () {
                that.textoActual = that.eliminarAcentos($(this).val());
                that.paginaActual = 1;
                that.buscarFacetas();
            });
        }
        else {
            that.$modalLoaded.find(".buscador-coleccion .buscar .texto").keyup(function (event) {
                //Comprobamos si la tecla es válida y si continua escribiendo
                if (that.validarKeyPulsada(event) == true) {
                    clearTimeout(that.timer);
                    that.timer = setTimeout(function () {
                        const query = that.eliminarAcentos(that.$modalLoaded.find(".buscador-coleccion .buscar .texto").val());
                        //Sí hay más de 3 caracteres hacemos la petición
                        if (query.length >= 4) {
                            let filtrosActivos = $("#panListadoFiltros").find("li a").not('.borrarFiltros');
                            let filtros = ''

                            filtrosActivos.each(function (index) {
                                filtros += $(this).attr("name") + '|';
                            });
                            filtros = filtros.substring(0, filtros.length - 1);
                            console.log(filtros);

                            //Inicializamos los parametros de la petición para el servicio autocompletar					
                            var proyectoBusqueda = $('input.inpt_proyID').val();
                            if (parametros_adiccionales.indexOf('proyectoOrigenID=') == 0) {
                                proyectoBusqueda = parametros_adiccionales.substring(parametros_adiccionales.indexOf('proyectoOrigenID=') + 'proyectoOrigenID='.length);
                                proyectoBusqueda = proyectoBusqueda.substring(0, proyectoBusqueda.indexOf('|'));
                            }
                            var metodo = "AutoCompletarFacetas";
                            var params = {};
                            params["orden"] = '0';
                            params["nombreFaceta"] = FacetaActual;
                            params["proyecto"] = proyectoBusqueda;
                            params["bool_esMyGnoss"] = $('input.inpt_bool_esMyGnoss').val() == 'True';
                            params["parametros"] = filtros;
                            params["bool_estaEnProyecto"] = $('input.inpt_bool_estaEnProyecto').val() == 'True';
                            params["bool_esUsuarioInvitado"] = $('input.inpt_bool_esUsuarioInvitado').val() == 'True';
                            params["identidad"] = $('input.inpt_identidadID').val();
                            params["q"] = query;
                            params["lista"] = '';
                            params["tipo"] = $('input.inpt_tipoBusquedaAutoCompl').val();
                            params["perfil"] = perfilID;
                            params["organizacion"] = $('input.inpt_organizacionID').val();
                            params["filtrosContexto"] = '';
                            params["languageCode"] = $('input.inpt_Idioma').val();

                            //Realizamos la petición al servicio autocompletar
                            $.post(obtenerUrl($('input.inpt_urlServicioAutocompletar').val()) + "/" + metodo, params, function (data) {
                                var listaFacetasDevueltas = data.split("\r\n");
                                var textoHtmlListaFacetas = `<div class="facetedSearch" id="facetedSearch">
												<div id="out_gnoss_hasfechapublicacion">
													<div id="in_gnoss_hasfechapublicacion">
														<div class="box" id="gnoss_hasfechapublicacion">
															<ul class="listadoFacetas">`;
                                listaFacetasDevueltas.forEach(function (facetaDevuelta) {
                                    //Montamos los resultados del servicio en una lista para pintarlos en el modal
                                    var indiceNumero = facetaDevuelta.lastIndexOf(" ");
                                    var textoNombreFaceta = facetaDevuelta.substring(0, indiceNumero);
                                    var textoNumeroFaceta = facetaDevuelta.substring(indiceNumero + 1);
                                    var enlace = document.location.href;

                                    if (document.location.href.indexOf("?") != -1) {
                                        enlace += `&${FacetaActual}=${textoNombreFaceta}`;
                                    }
                                    textoHtmlListaFacetas += `<li>
												<a rel="nofollow" href="${enlace}" class="faceta" name="${FacetaActual}=${textoNombreFaceta}" title="${textoNumeroFaceta[0]}">
													<span class="textoFaceta">${textoNombreFaceta} <span class="num-resultados">${textoNumeroFaceta} </span></span>
												</a>
											 </li>`
                                });
                                textoHtmlListaFacetas += `</ul></div></div></div><div style="display::none" id="panelFiltros"></div></div>`;
                                var htmlRespuesta = $("<div>").html(textoHtmlListaFacetas);
                                that.arrayTotales = new Array($(htmlRespuesta).find(".faceta").length);
                                numFacetasTotales = that.arrayTotales;
                                var i = 0;

                                $(htmlRespuesta)
                                    .find(".faceta")
                                    .each(function () {
                                        that.arrayTotales[i] = new Array(2);
                                        that.arrayTotales[i][0] = that.eliminarAcentos($(this).text().toLowerCase());
                                        that.arrayTotales[i][1] = $(this);
                                        i++;
                                    });

                                //Ordena por orden alfaético
                                if (that.facetasConPopUp[that.IndiceFacetaActual][2]) {
                                    that.arrayTotales = that.arrayTotales.sort(function (a, b) {
                                        if (a[0] > b[0]) return 1;
                                        if (a[0] < b[0]) return -1;
                                        return 0;
                                    });
                                }

                                that.paginaActual = 1;

                                //Buscamos y pintamos las facetas en el modal
                                that.buscarFacetas();
                            });
                        }

                    }, that.timeWaitingForUserToType);
                }
            });
        }
    },

    buscarFacetas: function () {
        buscando = true;
        const that = this;
        this.textoActual = this.textoActual.toLowerCase();

        // Limpio antes de mostrar datos - No harÃ­a falta si elimino todo con el cierre del modal
        that.$modalLoaded.find(".indice-lista.no-letra ul.listadoFacetas").remove();

        var facetaMin = (this.paginaActual - 1) * 22 + 1;
        var facetaMax = facetaMin + 21;

        var facetaActual = 0;
        var facetaPintadoActual = 0;
        var ul = $(`<ul class="listadoFacetas">`);

        this.fin = true;

        var arrayTextoActual = this.textoActual.split(" ");

        for (i = 0; i < this.arrayTotales.length; i++) {
            var nombre = this.arrayTotales[i][0];

            var mostrar = true;
            for (j = 0; j < arrayTextoActual.length; j++) {
                mostrar = mostrar && nombre.indexOf(arrayTextoActual[j]) >= 0;
            }

            if (facetaPintadoActual < 22 && mostrar) {
                facetaActual++;
                if (facetaActual >= facetaMin && facetaActual <= facetaMax) {
                    facetaPintadoActual++;
                    if (facetaPintadoActual == 1) {
                        ul = $(`<ul class="listadoFacetas">`);
                        that.$modalLoaded.find(".indice-lista.no-letra .resultados-wrap").append(ul);
                    } else if (facetaPintadoActual == 12) {
                        ul = $(`<ul class="listadoFacetas">`);
                        that.$modalLoaded.find(".indice-lista.no-letra .resultados-wrap").append(ul);
                    }
                    var li = $("<li>");
                    li.append(this.arrayTotales[i][1]);
                    ul.append(li);
                }
            }
            if (this.fin && facetaPintadoActual == 22 && mostrar) {
                this.fin = false;
            }
        }

        // Configurar click de la faceta
        $(".indice-lista .faceta").click(function (e) {
            AgregarFaceta($(this).attr("name"));
            // Cerrar modal
            that.$modalLoaded.modal('toggle');
            e.preventDefault();
        });

        this.buscando = false;
        // Establecer el tÃ­tulo o cabecera titular del modal
        that.$modalLoaded.find(".loading-modal-facet-title").text(
            that.facetasConPopUp[that.IndiceFacetaActual][1]
        );
        // Ocultar el Loading
        that.$modalLoaded.find('.loading-modal-facet').addClass('d-none');
    },
};

function VerFaceta(faceta, controlID) {
    if (document.getElementById(controlID + '_aux') == null) {
        $('#' + controlID).parent().html($('#' + controlID).parent().html() + '<div style="display:none;" id="' + controlID + '_aux' + '"></div>');
        $('#' + controlID + '_aux').html($('#' + controlID).html());

        var filtros = ObtenerHash2();

        if (typeof (filtroDePag) != 'undefined' && filtroDePag != '') {
            if (filtros != '') {
                filtros = filtroDePag + '|' + filtros;
            }
            else {
                filtros = filtroDePag;
            }
        }

        MontarFacetas(filtros, false, -1, '#' + controlID, faceta + '|vermas');
    }
    else {
        var htmlAux = $('#' + controlID + '_aux').html();
        $('#' + controlID + '_aux').html($('#' + controlID).html());
        $('#' + controlID).html(htmlAux);
        if (enlazarJavascriptFacetas) {
            enlazarFacetasBusqueda();
        }
        else {
            enlazarFacetasNoBusqueda();
        }
        CompletadaCargaFacetas();
    }
    return false;
}

function VerArbol(faceta, controlID) {
    var filtros = ObtenerHash2();
    //sb.AppendLine(Page.ClientScript.GetCallbackEventReference(this.Page, "filtros", "ReceiveServerData", String.Empty) + ";
    MontarFacetas(filtros, false, -1, '#' + controlID, faceta + '_Arbol');
    return false;
}

function CrearFiltroEliminarDeElemento(elemento) {
    var onclick = '';
    if (elemento.onclick != null) {
        onclick = elemento.onclick.toString();
    }
    CrearFiltroEliminar(elemento.name, elemento.title, onclick, elemento.innerHTML);
}

function CrearFiltroEliminar(name, title, onclick, innerHTML) {
    if (name.indexOf('search_') == 0) {
        $('#ctl00_ctl00_txtBusqueda').val('');
    }
    if (title != '') {
        innerHTML = title;
    }
    else if (innerHTML.charAt(innerHTML.length - 1) == ')') {
        var indiceParentesis = innerHTML.lastIndexOf('(') - 1;
        innerHTML = innerHTML.substring(0, indiceParentesis);
    }
    var nameNuevo = name;

    var elementosAgregado = $('li[name="' + nameNuevo + '"]');

    var agregame = document.getElementById(panFiltrosPulgarcito);
    if (agregame != null) {
        if (elementosAgregado.length == 0) {
            onclick = onclick.substr(onclick.indexOf('{') + 1);
            //            var nuevo = document.createElement('a');
            //            //nuevo.innerHTML = innerHTML;
            //            nuevo.setAttribute('onclick', onclick.substr(0, onclick.length - 1));
            //            nuevo.title = title;
            //            nuevo.id = nameNuevo;
            //            nuevo.name = nameNuevo;

            var li = document.createElement('li');
            li.setAttribute('name', nameNuevo);
            li.innerHTML = innerHTML + ' <a href="#" id="' + nameNuevo + '" name="' + nameNuevo + '" title="' + title + '" onclick="' + onclick.substr(0, onclick.length - 1) + '" class="remove">' + form.eliminar + '</a>';
            //li.appendChild(nuevo);

            agregame.appendChild(li);
        }
        else {
            agregame.removeChild(elementosAgregado[0]);
        }
    }
    if (agregame.childNodes.length > 0) {
        $('.group.filterSpace').css('display', '');
        $('.searchBy').css('display', '');
    }
    else {
        $('.searchBy').css('display', 'none');
    }

}

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

//Realizamos la petición


function replaceAll(texto, busqueda, reemplazo) {
    var resultado = '';

    while (texto.toString().indexOf(busqueda) != -1) {
        resultado += texto.substring(0, texto.toString().indexOf(busqueda)) + reemplazo;
        texto = texto.substring(texto.toString().indexOf(busqueda) + busqueda.length, texto.length);
    }

    resultado += texto;

    return resultado;
}

//montamos el contexto de los mensajes
function MontarContextoMensajes(pUsuarioID, pIdentidadID, pMensajeID, pLanguageCode, pParametrosBusqueda, pPanelID) {

    var servicio = new WS($('input.inpt_UrlServicioContextos').val(), WSDataType.jsonp);

    var metodo = 'CargarContextoMensajes';
    var params = {};

    params['usuarioID'] = pUsuarioID;
    params['identidadID'] = pIdentidadID;
    params['mensajeId'] = pMensajeID;
    params['languageCode'] = pLanguageCode;
    params['pParametrosBusqueda'] = pParametrosBusqueda;

    servicio.call(metodo, params, function (data) {
        var panel = $('#' + pPanelID);

        panel.html(data);

        //si divContexto tiene mensajes muestro el divContenedorContexto
        if (panel.children().children().length > 0) {
            document.getElementById('divContenedorContexto').style['display'] = "block";
        }
        else {
            document.getElementById('divContenedorContexto').style['display'] = "none";
        }

        //intentamos limpiar el panel que sobra
        if (panel.children('#ListadoGenerico_panContenedor').length > 0) {
            var panelResultados = $([panel.children('#ListadoGenerico_panContenedor').html()].join(''));
            panelResultados.appendTo(panel);
            panel.children('#ListadoGenerico_panContenedor').html('');
        }

        /* enganchar comportamiento mensajes */
        listadoMensajesMostrarAcciones.init();
        vistaCompactadaMensajes.init();

        if (typeof CompletadaCargaContextoMensajes == 'function') {
            CompletadaCargaContextoMensajes();
        }
    });
}

var utilMapas = {
    puntos: [],
    infowindow: null,
    mapbounds: null,
    gmarkers: [],
    gmarkersAgrupados: [],
    groutes: [],
    map: null,
    geocoder: null,
    markerCluster: null,
    markerClusterAgrupados: null,
    lat: 0,
    long: 0,
    zoom: 2,
    UltimaCoordenada: null,
    filtroCoordenadasMapaLat: null,
    filtroCoordenadasMapaLong: null,
    configuracionMapa: null,
    fichaRecurso: false,
    puntoRecurso: null,
    fichaMapa: 'listing-preview-map',
    contenedor: null,
    address: null,
    region: 'ES',
    IDMap: null,
    filtroWhere: null,
    listenerAdded: false,

    EstablecerFiltroCoordMapa: function (pFiltroLat, pFiltroLong) {
        this.filtroCoordenadasMapaLat = pFiltroLat;
        this.filtroCoordenadasMapaLong = pFiltroLong;
    },

    EstablecerParametrosMapa: function (pAddress, pRegion, pIDMap, pFiltroWhere) {
        this.address = pAddress;
        this.region = pRegion;
        this.IDMap = pIDMap;
        this.filtroWhere = pFiltroWhere;
    },

    MontarMapaResultados: function (pContenedor, pDatos) {
        this.contenedor = pContenedor;
        if (this.map == null) {
            var mapOptions = {
                zoom: this.zoom,
                minZoom: 2,
                center: new google.maps.LatLng(this.lat, this.long),
                mapTypeId: google.maps.MapTypeId.ROADMAP,
                panControl: true,
                panControlOptions: {
                    position: google.maps.ControlPosition.RIGHT_BOTTOM
                },
                zoomControlOptions: {
                    position: google.maps.ControlPosition.RIGHT_BOTTOM,
                    style: google.maps.ZoomControlStyle.LARGE
                }

            }

            if (this.fichaRecurso) {
                if (pDatos.length == 2) {
                    this.puntoRecurso = [pDatos[1].split(',')[2], pDatos[1].split(',')[3]];
                }
            } else {
                $(this.contenedor).attr('class', 'listadoRecursos mapabusqueda');
            }

            if ((typeof ConfigurarEstilosMapa != 'undefined')) {
                ConfigurarEstilosMapa(this.contenedor);
            }

            this.map = new google.maps.Map($(this.contenedor)[0], mapOptions);

            utilMapas.UltimaCoordenada = null;
            this.markerCluster = null;
            this.markerClusterAgrupados = null;

            if (this.address != null) {
                this.geocoder = new google.maps.Geocoder();

                this.geocoder.geocode({ 'address': this.address, 'region': this.region }, function (results, status) {
                    var latDocumentoID = null;
                    var longDocumentoID = null;
                    for (var i = 1; i < pDatos.length; i++) {
                        if (pDatos[i] != '') {
                            var datos = pDatos[i].split(',');
                            if (datos.length == 4) {
                                if (datos[1] == "documentoid") {
                                    latDocumentoID = datos[2];
                                    longDocumentoID = datos[3];
                                }
                            }
                        }
                    }

                    if (status == google.maps.GeocoderStatus.OK) {
                        if (latDocumentoID != null && longDocumentoID != null) {
                            utilMapas.map.setCenter(new google.maps.LatLng(latDocumentoID, longDocumentoID));
                            utilMapas.map.setZoom(13);
                        } else {
                            utilMapas.map.setCenter(results[0].geometry.location);
                            utilMapas.map.fitBounds(results[0].geometry.bounds);
                        }
                        utilMapas.UltimaCoordenada = utilMapas.map.getBounds();
                    }
                });
            }

            if (this.IDMap != null) {
                var layer = new google.maps.FusionTablesLayer({
                    query: {
                        select: 'geometry',
                        from: this.IDMap,
                        where: this.filtroWhere
                    },
                    styles: [{
                        polygonOptions:
                        {
                            fillColor: "#ffffff",
                            fillOpacity: 0.1
                        }
                    }],
                    map: utilMapas.map,
                    suppressInfoWindows: true
                });
            }
        }

        this.puntos = pDatos;
        this.mapbounds = new google.maps.LatLngBounds();
        this.gmarkers = [];
        this.infowindow = new google.maps.InfoWindow({ content: '' });

        var me = this;

        var puntosDefinidos = 0;

        this.CargarConfiguracionMapa();
        this.OcultarFichaMapa();

        for (var i = 0; i < this.groutes.length; i++) {
            this.groutes[i].setMap(null);
        }
        this.groutes = [];

        for (var i = 1; i < this.puntos.length; i++) {
            if (this.puntos[i] != '') {
                var datos = this.puntos[i].split(',');
                if (datos.length == 4) {
                    me.DefinirPunto(this.map, datos[0], datos[2], datos[3], datos[1]);
                } else {
                    var color = '';
                    if (datos.length > 5) {
                        color = datos[5];
                    }

                    me.DefinirRuta(this.map, datos[2], datos[3], datos[4].replace(/\;/g, ','), datos[1], color);
                }
                puntosDefinidos++;
            }
        }

        var filtros = ObtenerHash2();
        var filtrandoPorLatLong = filtros.indexOf(this.filtroCoordenadas > -1)

        if (!this.fichaRecurso && (puntosDefinidos > 0 && !filtrandoPorLatLong)) {
            this.map.fitBounds(this.mapbounds);
            this.map.setCenter(this.mapbounds.getCenter());
        }

        if (this.fichaRecurso) {
            this.map.setCenter(this.mapbounds.getCenter());
            if (this.puntos.length > 2) {
                this.map.fitBounds(this.mapbounds);
            } else {
                this.map.setZoom(13);
            }

        }

        this.PintarMarcas();

        if (!filtrandoPorLatLong) {
            if (this.map.getZoom() > 10) {
                this.map.setZoom(10);
            }
        }
        var that = this;

        if (!this.listenerAdded) {
            this.listenerAdded = true;
            google.maps.event.addListener(this.map, 'bounds_changed', function () {
                if (!that.fichaRecurso && utilMapas.filtroCoordenadasMapaLat != null && utilMapas.filtroCoordenadasMapaLong != null) {
                    that.OcultarFichaMapa();
                    var coordenadas = this.getBounds();
                    if (utilMapas.UltimaCoordenada == null) {
                        utilMapas.UltimaCoordenada = coordenadas;
                    } else if (utilMapas.UltimaCoordenada != coordenadas) {
                        utilMapas.UltimaCoordenada = coordenadas;
                        setTimeout("utilMapas.FiltrarPorCoordenadas('" + coordenadas + "')", 1000);
                    }
                }
            });
        }
    },
    OcultarFichaMapa: function () {
        $('#' + this.fichaMapa).attr('activo', 'false');
        $('#' + this.fichaMapa).hide();
    },

    MostrarFichaMapa: function () {
        $('#' + this.fichaMapa).attr('activo', 'true');
        $('#' + this.fichaMapa).show();
    }
    ,
    DefinirPunto: function (map, id, lat, lon, tipo) {
        lat = parseFloat(lat) + (Math.random() - 0.5) / 60000;
        lon = parseFloat(lon) + (Math.random() - 0.5) / 60000;
        var claveLatLog = lat + '&' + lon;

        var icon = null;
        if (this.configuracionMapa != null && this.configuracionMapa.ImagenesPorTipo != null && this.configuracionMapa.ImagenesPorTipo[tipo] != null) {
            icon = this.configuracionMapa.ImagenesPorTipo[tipo];
        }

        var me = this;
        var marker = new google.maps.Marker({
            position: new google.maps.LatLng(lat, lon),//,
            //map: map,
            icon: icon
            //title: "Hello World " + id
        });

        marker.documentoID = id.substring(id.lastIndexOf('/') + 1);
        marker.tipo = 'punto';

        if (this.configuracionMapa != null && this.configuracionMapa.EstilosGrupos.tipos != null && this.configuracionMapa.EstilosGrupos.tipos[tipo] != null) {
            var nombreGrupo = this.configuracionMapa.EstilosGrupos.tipos[tipo];
            for (var i = 0; i < this.gmarkersAgrupados.length; i++) {
                if (this.gmarkersAgrupados[i][0] == nombreGrupo) {
                    this.gmarkersAgrupados[i][1].push(marker);
                }
            }
        } else {
            this.gmarkers.push(marker);
        }

        if (id != "") {
            google.maps.event.addListener(marker, 'click', function () {
                me.CargarPunto(map, marker, claveLatLog, tipo);
            });
        }

        me.mapbounds.extend(marker.position);
    },

    CargarPunto: function (map, marker, claveLatLog, tipo) {
        var docIDs = marker.documentoID;

        if (this.configuracionMapa != null && this.configuracionMapa.popup != null && this.configuracionMapa.popup == 'personalizado') {
            //1º Obtenemos las coordenadas
            var overlay = new google.maps.OverlayView();
            overlay.draw = function () { };
            overlay.setMap(map);

            var panelY = 0;
            var panelX = 0;

            if (marker.tipo == 'punto') {
                var posicionMapa = overlay.getProjection().fromLatLngToContainerPixel(marker.getPosition());
                var posicionDiv = $(this.contenedor).offset();
                panelY = posicionMapa.y + posicionDiv.top + (marker.anchorPoint.y / 2);
                panelX = posicionMapa.x + posicionDiv.left;
            } else {
                panelX = currentMousePos.x;
                panelY = currentMousePos.y;
            }

            var that = this;

            //2º Ocultamos el panel
            that.OcultarFichaMapa();
            $('#' + this.fichaMapa).unbind();
            $('#' + this.fichaMapa).mouseleave(function () {
                that.OcultarFichaMapa();
            });
            $('#aspnetForm').mouseup(function () {
                that.OcultarFichaMapa();
            });
            $('#' + this.fichaMapa).attr('activo', 'true')

            TraerRecPuntoMapa(null, this.fichaMapa, claveLatLog, docIDs, '', panelX, panelY, tipo);
        } else {
            var me = this;
            me.infowindow.setContent('<div><p>' + form.cargando + '...</p></div>');
            me.infowindow.open(map, marker);
            TraerRecPuntoMapa(me, null, claveLatLog, docIDs);
        }
    },

    CargarConfiguracionMapa: function () {
        if (typeof (CargarConfiguracionMapaComunidad) != 'undefined') {
            this.configuracionMapa = CargarConfiguracionMapaComunidad();
        }
        if (this.configuracionMapa != null && this.configuracionMapa.EstilosGrupos != null) {
            this.gmarkersAgrupados = [];
            for (var tipo in this.configuracionMapa.EstilosGrupos.tipos) {
                var nombreGrupo = this.configuracionMapa.EstilosGrupos.tipos[tipo];
                var yaexiste = false;
                for (var i = 0; i < this.gmarkersAgrupados.length; i++) {
                    if (this.gmarkersAgrupados[i][0] == nombreGrupo) {
                        yaexiste = true;
                    }
                }
                if (!yaexiste) {
                    var array = new Array(2);
                    array[0] = nombreGrupo;
                    array[1] = [];
                    this.gmarkersAgrupados.push(array);
                }
            }
        }
    },

    DefinirRuta: function (map, tipo, id, puntos, docID, pColor) {
        try {
            var puntosParseados = JSON && JSON.parse(puntos) || $.parseJSON(puntos);
            var puntosRuta = [];
            for (var i = 0; i < puntosParseados[id].length; i++) {
                puntosRuta[i] = new google.maps.LatLng(puntosParseados[id][i][0], puntosParseados[id][i][1]);
            }

            if (pColor == '' || pColor == null) {
                pColor = ColorAleatorio();
            }

            var ruta = new google.maps.Polyline({
                path: puntosRuta,
                geodesic: true,
                strokeColor: pColor,
                //strokeColor: ColorAleatorio(),
                strokeOpacity: 1.0,
                strokeWeight: 2
            });

            var indicePuntoMedio = Math.floor(puntosParseados[id].length / 2);
            var coordenadasPtoMedio = puntosParseados[id][indicePuntoMedio];
            var lat = coordenadasPtoMedio[0];
            var lon = coordenadasPtoMedio[1];

            this.groutes.push(ruta);

            ruta.documentoID = docID.substring(docID.lastIndexOf('/') + 1);
            ruta.tipo = 'ruta';

            var me = this;
            google.maps.event.addListener(ruta, 'click', function () {
                var claveLatLog = puntosParseados[id][0][0] + '&' + puntosParseados[id][0][1];
                me.CargarPunto(map, ruta, claveLatLog, tipo);
            });

            ruta.setMap(map);

            this.DefinirPunto(map, docID, lat, lon, tipo);
        }
        catch (ex) { }
    },

    PintarMarcas: function () {
        var maxZoomLevel = 21;
        if (this.markerCluster == null) {
            this.markerCluster = new MarkerClusterer(this.map, this.gmarkers, { maxZoom: maxZoomLevel - 1 });
        } else {
            var marKersAntiguos = this.markerCluster.markers_;
            var marKersNuevos = this.gmarkers;

            var marKersAnyadir = this.PintarMarcasAux(marKersNuevos, marKersAntiguos);
            var marKersEliminar = this.PintarMarcasAux(marKersAntiguos, marKersNuevos);

            this.markerCluster.removeMarkers(marKersEliminar);
            this.markerCluster.addMarkers(marKersAnyadir);
        }

        if (this.markerClusterAgrupados == null) {
            this.markerClusterAgrupados = [];
            for (var i = 0; i < this.gmarkersAgrupados.length; i++) {
                this.markerClusterAgrupados[i] = new MarkerClusterer(this.map, this.gmarkersAgrupados[i][1], {
                    maxZoom: maxZoomLevel - 1,
                    styles: this.configuracionMapa.EstilosGrupos[this.gmarkersAgrupados[i][0]]
                });
            }
        } else {
            for (var i = 0; i < this.gmarkersAgrupados.length; i++) {
                var marKersAntiguos = this.markerClusterAgrupados[i].markers_;
                var marKersNuevos = this.gmarkersAgrupados[i][1];

                var marKersAnyadir = this.PintarMarcasAux(marKersNuevos, marKersAntiguos);
                var marKersEliminar = this.PintarMarcasAux(marKersAntiguos, marKersNuevos);

                this.markerClusterAgrupados[i].removeMarkers(marKersEliminar);
                this.markerClusterAgrupados[i].addMarkers(marKersAnyadir);
            }
        }
    },

    PintarMarcasAux: function (pMarkersA, pMarkersB) {
        //Devuelve un array con todos los markers de A que no esten en B
        var markers = [];
        for (var i = 0; i < pMarkersA.length; i++) {
            var existe = false;
            for (var j = 0; j < pMarkersB.length; j++) {
                if (pMarkersB[j].documentoID == pMarkersA[i].documentoID) {
                    existe = true;
                }
            }
            if (!existe) {
                markers.push(pMarkersA[i]);
            }
        }
        return markers;
    },

    AjustarBotonesVisibilidad: function () {
        /*
        var vistaMapa = ($('li.mapView').attr('class') == "mapView activeView");
        var vistaChart = ($('.chartView').attr('class') == "chartView activeView");
        */
        var vistaMapa = $(".item-dropdown.aMapView").hasClass("activeView");
        var vistaChart = $(".item-dropdown.aGraphView").hasClass("activeView");

        var mapView = $('.mapView');

        if (vistaMapa || vistaChart) {
            var listView = $('.listView');
            var gridView = $('.gridView');

            $('a', listView).unbind();
            $('a', listView).bind('click', function (evento) {
                evento.preventDefault();
                $('.listView').attr('class', 'listView activeView');
                $('.gridView').attr('class', 'gridView');
                $('.mapView').attr('class', 'mapView');
                $('.chartView').attr('class', 'chartView');
                $('div.mapabusqueda').attr('class', 'listadoRecursos');
                FiltrarPorFacetas(ObtenerHash2());
            });

            $('a', gridView).unbind();
            $('a', gridView).bind('click', function (evento) {
                evento.preventDefault();
                $('.listView').attr('class', 'listView');
                $('.gridView').attr('class', 'gridView activeView');
                $('.mapView').attr('class', 'mapView');
                $('.chartView').attr('class', 'chartView');
                $('div.mapabusqueda').attr('class', 'listadoRecursos');
                FiltrarPorFacetas(ObtenerHash2());
            });

            $('.panelOrdenContenedor').css('display', 'none');
        }
        else {
            $('.panelOrdenContenedor').css('display', '');
        }

        if (vistaMapa) {
            $('a', mapView).unbind();
            $('a', mapView).bind('click', function (evento) {
                evento.preventDefault();
            });
        }
        else {
            $('a', mapView).unbind();
            $('a', mapView).bind('click', function (evento) {
                evento.preventDefault();
                $('.listView').attr('class', 'listView');
                $('.gridView').attr('class', 'gridView');
                $('.mapView').attr('class', 'mapView activeView');
                $('.chartView').attr('class', 'chartView');
                utilMapas.map = null;
                FiltrarPorFacetas(ObtenerHash2());
            });
        }

        if ($('#ctl00_ctl00_CPH1_CPHContenido_liDescargaExcel').length > 0 && $('#ctl00_ctl00_CPH1_CPHContenido_liDescargaExcel').css('display') != 'none' && !$('#ctl00_ctl00_CPH1_CPHContenido_liDescargaExcel').is(':visible')) {
            $('#ctl00_ctl00_CPH1_CPHContenido_divVista').css('display', '');
        }
    },

    FiltrarPorCoordenadas: function (coordenadas) {
        if (utilMapas.UltimaCoordenada == coordenadas) {
            var paramsCoord = coordenadas.replace(/\)/g, '').replace(/\(/g, '').split(",")
            var minLat = paramsCoord[0].trim();
            var maxLat = paramsCoord[2].trim();
            var minLong = paramsCoord[1].trim();
            var maxLong = paramsCoord[3].trim();

            var filtro = ObtenerHash2();

            filtro = this.ReemplazarFiltro(filtro, utilMapas.filtroCoordenadasMapaLat, minLat + '-' + maxLat);
            filtro = this.ReemplazarFiltro(filtro, utilMapas.filtroCoordenadasMapaLong, minLong + '-' + maxLong);

            history.pushState('', 'New URL: ' + filtro, '?' + filtro);
            FiltrarPorFacetas(filtro);
        }
    },

    ReemplazarFiltro: function (filtros, tipoFiltro, filtro) {

        //Si el filtro ya existe, cambiamos el valor del filtro
        if (filtros.indexOf(tipoFiltro) == 0 || filtros.indexOf('&' + tipoFiltro) > 0) {
            var textoAux = filtros.substring(filtros.indexOf(tipoFiltro));
            if (textoAux.indexOf('&') > -1) {
                textoAux = textoAux.substring(0, textoAux.indexOf('&'));
            }
            filtros = filtros.replace(textoAux, tipoFiltro + '=' + filtro);
        }
        else {
            if (filtros.length > 0) { filtros += '&'; }
            filtros += tipoFiltro + '=' + filtro;
        }

        return filtros;
    }
}

function ColorAleatorio() {
    hexadecimal = new Array("0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "A", "B", "C", "D", "E", "F")
    color_aleatorio = "#";
    for (i = 0; i < 6; i++) {
        posarray = aleatorio(0, hexadecimal.length)
        color_aleatorio += hexadecimal[posarray]
    }
    return color_aleatorio
}

function aleatorio(inferior, superior) {
    numPosibilidades = superior - inferior
    aleat = Math.random() * numPosibilidades
    aleat = Math.floor(aleat)
    return parseInt(inferior) + aleat
}

var contador = 0;

var viewOptions = {

    id: '#viewOptions',

    cssResources: '.resourcesList',

    cssResourcesRow: 'row',

    cssResourcesGrid: 'grid',

    cssGridView: '.gridView',

    cssRowView: '.rowView',

    cssResource: '.resource',

    cssActive: 'activeView',

    isCalculadoOmega: false,

    alturas: [],

    init: function () {

        this.config();

        this.behaviours();

    },

    config: function () {

        this.componente = $(this.id);

        this.resourceList = this.componente.next();

        this.gridView = $(this.cssGridView, this.componente);

        this.rowView = $(this.cssRowView, this.componente);

        this.links = $('a', this.componente);

        this.resources = $(this.cssResource, this.resourceList);

        return;

    },

    calcularOmega: function () {

        var contador = 0;

        var masAlto = 0;

        var that = this;

        this.resources.each(function () {

            var recurso = $(this);

            var altura = recurso.height();

            if (altura > masAlto) masAlto = altura;

            contador++;

            if (contador == 3) {

                recurso.addClass('omega');

                that.alturas.push(masAlto);

                contador = 0;

                masAlto = 0;

            }

        });

        this.resources.each(function () {

            var recurso = $(this);

            recurso.css('height', masAlto + 'px');

        });

        this.isCalculadoOmega = true;

    },

    igualarAlturas: function () {

        var that = this;

        console.log(this.alturas);

        var contador = 0;

        this.resources.each(function (indice) {

            var recurso = $(this);

            var fila = parseInt(indice / 3);

            recurso.css('height', that.alturas[fila] + 'px');

        });

    },

    borrarAlto: function () {

        this.resources.each(function () {

            var recurso = $(this);

            recurso.css('height', 'auto');

        });

    },

    desmarcarActivo: function () {

        var that = this;

        this.links.each(function () {

            $(this).removeClass(that.cssActive);

        })

        return;

    },

    behaviours: function () {

        var that = this;

        this.gridView.bind('click', function (evento) {

            that.desmarcarActivo();

            $(this).addClass(that.cssActive);

            that.resourceList.removeClass(that.cssResourcesRow);

            that.resourceList.addClass(that.cssResourcesGrid);

            if (!that.isCalculadoOmega) that.calcularOmega();

            that.igualarAlturas();

            return false;

        });

        this.rowView.bind('click', function (evento) {

            that.desmarcarActivo();

            $(this).addClass(that.cssActive);

            that.resourceList.removeClass(that.cssResourcesGrid);

            that.resourceList.addClass(that.cssResourcesRow);

            that.borrarAlto();

            return false;

        });
    }
}


/*--------    REGION MASTER GNOSS    -----------------------------------------------------------*/

function aceptarNuevaNavegacion() {
    document.getElementById('capaModal').style.display = 'none';
}

//Codigo para crear el desplegable del login mediante jQuery


//function submitform() {
//    document.myform.submit();
//}


function OculatarHerramientaAddto() {
    //    if ($.browser.msie && $.browser.version < 7) {
    //        idIntervalo = setInterval("accederWeb()", 500);
    //    }
}

function CambiarNombre(link, nombre1, nombre2) {
    if (link.innerHTML == nombre1) {
        link.innerHTML = nombre2;
    }
    else {
        link.innerHTML = nombre1;
    }
}

/*--------    REGION SCRIPTS INICIALES   -----------------------------------------------------------*/

function ReceiveServerData(arg, context) {
    EjecutarScriptsIniciales();
    if (!(arg == "")) {
        var json = eval('(' + arg + ')');
        for (var i = 0; i < json.length; i++) {
            var object = json[i];
            if (object.render == true) {
                if (object.id != "") {
                    var element = document.getElementById(object.id);

                    if (element != null) {
                        if (object.reemplazarHtml) {
                            if (object.innerHTML == "") {
                                element.parentNode.removeChild(element);
                            }
                            else {
                                var divNuevoHtml = document.createElement("div");
                                divNuevoHtml.innerHTML = object.innerHTML;
                                element.parentNode.replaceChild(divNuevoHtml.childNodes[0], element);
                            }
                        }
                        else if (object.agregarHtml) {
                            element.innerHTML += object.innerHTML;
                        }
                        else if (object.agregarHtmlAlPrinc) {
                            element.innerHTML = object.innerHTML + element.innerHTML;
                        }
                        else {
                            if (element != null) { element.innerHTML = object.innerHTML; }
                        }
                    }
                } //Fin if(object.id!=\"\")
            } //Fin if(object.render == true), empieza esle
            else {
                var cadena = object.funcion.split('&');
                object.funcion = cadena[0];
                if (object.funcion == "Seleccionar") {
                    SeleccionarElemento(object.id);
                }
                else if (object.funcion == "DeSeleccionar") {
                    DeSeleccionarElemento(object.id);
                }
                else if (object.funcion == "Contraer") {
                    var element = document.getElementById(object.id);
                    element.style.display = "none";
                }
                else if (object.funcion == "HacerVisible") {
                    var element = document.getElementById(object.id);
                    element.style.display = "block";
                }
                else if (object.funcion == "Ocultar") {
                    var element = document.getElementById(object.id);
                    if (element != null) {
                        element.style.display = "none";
                    }
                }
                else if (object.funcion == "CambiarConector") {
                    var element = document.getElementById(object.id);
                    element.src = object.innerHTML;
                }
                else if (object.funcion == "CambiarCssClass") {
                    var element = document.getElementById(object.id);
                    element.className = object.innerHTML;
                }
                else if (object.funcion == "CambiarValor") {
                    var element = document.getElementById(object.id);
                    element.value = object.innerHTML;
                }
                else if (object.funcion == "CambiarAtributo") {
                    var element = document.getElementById(object.id);
                    element.setAttribute(object.atributo, object.valor);
                }
                else if (object.funcion == "CambiarAtributoStyle") {
                    var element = document.getElementById(object.id);
                    element.style[object.atributo] = object.valor;
                }
                else if (object.funcion == "CambiarID") {
                    var element = document.getElementById(object.id);
                    element.id = object.innerHTML;
                }
                else if (object.funcion == "Guardar") {
                    Guardar(object.innerHTML);
                }
                else if (object.funcion == "MarcarCheckBox") {
                    $('#' + object.id).attr('checked', true);
                }
                else if (object.funcion == "DesmarcarCheckBox") {
                    $('#' + object.id).attr('checked', false);
                }
                else if (object.funcion == "SweetTitles") {
                    if (window.sweetTitles) { sweetTitles.init(); }
                }
                else if (object.funcion == "MostrarErrorLogin") {
                    crearError('<p>' + form.errorLogin + '</p>', cadena[1]);
                }
                else if (object.funcion == "redirect") {
                    if (object.Direccion != null) {
                        window.location.href = object.Direccion;
                    }
                    else {
                        window.location.href = cadena[1];
                    }
                }
                else if (object.funcion == "hacerClick") {
                    document.getElementById(cadena[1]).click();
                }
                else if (object.funcion == "ejecutarFuncion") {
                    eval(object.innerHTML);
                }
                else if (object.funcion == "ejecutarFuncionConParametorObjeto") {
                    eval(object.metodoJS + '(object);');
                }
                else if (object.funcion == "ResaltarTags" && object.ListaTags != "") {
                    ResaltarTags(object.ListaTags);
                }
                else if (object.funcion == "reload") {
                    document.location.reload();
                }

                if (object.funcion == "TraerNumeroResultados") {
                    FiltrarBusquedaAvanzada(object.numResulCoincidentes);
                }

                if (object.funcion == "MostrarControles") {
                    MostrarControles(object.controles);
                }
            } //Fin else de if(object.render == true), empieza esle
        } //Fin foreach
    } //Fin if(!(arg == \"\"))
} //Fin function

//Seleccionar
function SeleccionarElemento(elementID) {
    var element = document.getElementById(elementID);
    element.style.border = "solid 1px black";
}
//DeSeleccionar
function DeSeleccionarElemento(elementID) {
    var element = document.getElementById(elementID);
    element.style.border = "";
}
function CambiarNombreElemento(elementID, nombre) {
    var element = document.getElementById(elementID);
    element.innerHTML = '';
    element.parentNode.innerHTML = nombre;
}
function CambiarTextoElemento(elementID, nombre) {
    var element = document.getElementById(elementID);
    element.innerHTML = nombre;
}

/**
 * Eliminar� los atributos del bot�n para que no pueda volver a ejecutar nada a menos que se vuelva a carguar la p�gina web
 * Ej: Acciones que se hacen sobre una persona ("No enviar newsletter, Bloquear...")
 * @param {any} elementId: Elemento que se desea cambiar el nombre y eliminar atributos
 * @param {any} nombre: Nombre que tendr� el bot�n una vez se haya pulsado sobre �l y las acciones se hayan realizado
 * @param {any} listaAtributos: Lista de atributos en formato String que ser�n eliminados del bot�n (Ej: "data-target", "href", "onclick")
 * */
function CambiarTextoAndEliminarAtributos(elementId, nombre, listaAtributos) {
    // Seleccionamos el elemento
    var element = $('#' + elementId);
    // Eliminar la lista de atributos deseados
    listaAtributos.forEach(atributo => $(element).removeAttr(atributo));
    // Cambiamos el nombre del elemento
    $(element).html(nombre)
    // A�adimos estilo para que no parezca que es "clickable"
    $(element).css('cursor', 'auto');
}

//Función que arregla pringues de Microsoft:
function WebForm_CallbackComplete() {
    for (var i = 0; i < __pendingCallbacks.length; i++) {
        callbackObject = __pendingCallbacks[i];
        if (callbackObject && callbackObject.xmlRequest && (callbackObject.xmlRequest.readyState == 4)) {
            WebForm_ExecuteCallback(callbackObject);
            if (__pendingCallbacks[i] != null && !__pendingCallbacks[i].async) {
                __synchronousCallBackIndex = -1;
            }
            __pendingCallbacks[i] = null;
            var callbackFrameID = "__CALLBACKFRAME" + i;
            var xmlRequestFrame = document.getElementById(callbackFrameID);
            if (xmlRequestFrame) {
                xmlRequestFrame.parentNode.removeChild(xmlRequestFrame);
            }
        }
    }
}

////Realizamos la petición
//function DesplegarDescripcionAJAX(pGuidDoc) {
//    PeticionesAJAX.CargarDescripcionRecurso(pGuidDoc, RecogerDescripcion, RecogerErroresAJAX);
//}

//function DesplegarDescripcionSuscripcionAJAX(pGuidDoc, pGuidSusc) {
//    PeticionesAJAX.CargarDescripcionRecursoSuscripcion(pGuidDoc, pGuidSusc, RecogerDescripcion, RecogerErroresAJAX);
//}

//function RecogerDescripcion(datosRecibidos) {
//    //Leemos los datos
//    var docID = datosRecibidos.substring(0, datosRecibidos.indexOf('|'));
//    var descripcion = datosRecibidos.substring(datosRecibidos.indexOf('|') + 1);
//    //Ocultamos la descripción corta y mostramos la larga
//    $('#DescripcionCorta_' + docID).hide();
//    $('#DescripcioLarga_' + docID).show();
//    $('#DescripcioLarga_' + docID).html(descripcion);
//    //Cambiamos la imágen del + y quitamos los puntos suspensivos
//    //La variable  $('input.inpt_baseURL').val() se define en la MasterGnoss
//    $('#DescripcionCorta_' + docID + '_DesplegarTexto').children('p').children('a').children('img').attr('src',  $('input.inpt_baseURL').val() + '/img/verMenos.gif');
//    $('#DescripcionCorta_' + docID + '_DesplegarTexto').children('p').children('span').html('');
//    //Quitamos el evento onclick del +
//    $('#DescripcionCorta_' + docID + '_DesplegarTexto').children('p').children('a').removeAttr('onclick');
//    //Creamos el evento que alterna entre las descripciones
//    $('#DescripcionCorta_' + docID + '_DesplegarTexto').children('p').children('a').toggle(function () {
//        $('#DescripcionCorta_' + docID).show();
//        $('#DescripcioLarga_' + docID).hide();
//        $('#DescripcionCorta_' + docID + '_DesplegarTexto').children('p').children('a').children('img').attr('src',  $('input.inpt_baseURL').val() + '/img/verMas.gif');
//        $('#DescripcionCorta_' + docID + '_DesplegarTexto').children('p').children('span').html('...');
//    },
//function () {
//    $('#DescripcionCorta_' + docID).hide();
//    $('#DescripcioLarga_' + docID).show();
//    $('#DescripcionCorta_' + docID + '_DesplegarTexto').children('p').children('a').children('img').attr('src',  $('input.inpt_baseURL').val() + '/img/verMenos.gif');
//    $('#DescripcionCorta_' + docID + '_DesplegarTexto').children('p').children('span').html('');
//});
//}

function RecogerErroresAJAX(error) {
    //alert(error);
}

function ObtenerNumMensajesSinLeerAJAX(pGuidIdent) {
    PeticionesAJAX.CargarNumMensajesSinLeer(pGuidIdent, RecogerNumMensajesSinLeer, RecogerErroresAJAX);
}

function RecogerNumMensajesSinLeer(datosRecibidos) {
    //Leemos los datos
    var arrayDatos = datosRecibidos.split('|');
    var numMensajesNoLeidos = arrayDatos[0];
    var numMensajesNoLeidosEliminados = arrayDatos[1];

    //Cambiamos el numero de mensajes recibidos
    reemplazarContadores('#ctl00_CPH1_hlCorreoRecibido', numMensajesNoLeidos);

    //Cambiamos el numero de mensajes recibidos
    reemplazarContadores('#ctl00_CPH1_hlCorreoEliminado', numMensajesNoLeidosEliminados);
}


function ObtenerNumElementosSinLeerAJAX(pGuidPerfilUsu, pGuidPerfilOrg, pGuidOrg, pEsBandejaOrg, pCaducidadSusc) {
    PeticionesAJAX.CargarNumElementosSinLeer(pGuidPerfilUsu, pGuidPerfilOrg, pGuidOrg, pEsBandejaOrg, pCaducidadSusc, RecogerNumElementosSinLeer, RecogerErroresAJAX);
}

function RecogerNumElementosSinLeer(datosRecibidos) {
    //Leemos los datos
    var arrayDatos = datosRecibidos.split('|');
    var numMensajesNoLeidos = arrayDatos[0];
    var numInvitacionesNoLeidas = arrayDatos[1];
    var numSuscripcionesNoLeidos = arrayDatos[2];
    var numComentariosNoLeidos = arrayDatos[3];

    //Cambiamos el numero de Mensajes
    reemplazarContadores('#ctl00_ctl00_CPH1_hlMensajes', numMensajesNoLeidos);

    //Cambiamos el numero de Comentarios
    reemplazarContadores('#ctl00_ctl00_CPH1_hlComentarios', numComentariosNoLeidos);

    //Cambiamos el numero de Invitaciones
    reemplazarContadores('#ctl00_ctl00_CPH1_hlInvitaciones', numInvitacionesNoLeidas);

    //Cambiamos el numero de Suscripciones
    reemplazarContadores('#ctl00_ctl00_CPH1_hlSuscripciones', numSuscripcionesNoLeidos);

}

var perfilID;
var perfilOrgID;
var organizacionID;
var esAdministradorOrg;
$(document).ready(function () {
    perfilID = $('input#inpt_perfilID').val();
    perfilOrgID = $('input#inpt_perfilOrgID').val();
    organizacionID = $('input#inpt_organizacionID').val();
    esAdministradorOrg = $('input#inpt_AdministradorOrg').val();
    refrescarNumElementosNuevos();
    //$('#descargarRDF').click(function () {
    //    var url = window.location.href;

    //    if (url.indexOf('?rdf') == -1 && url.indexOf('&rdf') == -1) {
    //        if (url.indexOf('?') == -1) {
    //            url += '?';
    //        }
    //        else {
    //            url += '&';
    //        }

    //        url += 'rdf';
    //    }

    //    window.open(url, '_blank');
    //    return false;
    //});
    engancharClicks();
});

function engancharClicks() {
    $('[clickJS]').each(function () {
        var control = $(this);
        var js = control.attr('clickJS');
        control.removeAttr('clickJS');
        control.click(function (evento) {
            evento.preventDefault();
            eval(js);
        });
    });
}


function refrescarNumElementosNuevos() {
    if (typeof (perfilID) != 'undefined' && perfilID != 'ffffffff-ffff-ffff-ffff-ffffffffffff' && perfilID != '00000000-0000-0000-0000-000000000000') {
        try {
            ObtenerNumElementosNuevosAJAX(perfilID, perfilOrgID, esAdministradorOrg);
            setTimeout("ObtenerNumElementosNuevosAJAX('" + perfilID + "', '" + perfilOrgID + "', '" + esAdministradorOrg + "')", 60000);
            setTimeout("ObtenerNumElementosNuevosAJAX('" + perfilID + "', '" + perfilOrgID + "', '" + esAdministradorOrg + "')", 240000);

            if ($('input.inpt_MantenerSesionActiva').length == 0 || $('input.inpt_MantenerSesionActiva').val().toLowerCase() == "true") {
                setInterval("ObtenerNumElementosNuevosAJAX('" + perfilID + "', '" + perfilOrgID + "', '" + esAdministradorOrg + "')", 600000);
            }

            //if ($('input.inpt_refescarContadoresSinLimite').val() == "1") {
            //    setInterval("ObtenerNumElementosNuevosAJAX('" + perfilID + "', '" + perfilOrgID + "', '" + esAdministradorOrg + "')", 600000);
            //}
            //else {
            //    setTimeout("ObtenerNumElementosNuevosAJAX('" + perfilID + "', '" + perfilOrgID + "', '" + esAdministradorOrg + "')", 600000);
            //}
        }
        catch (ex) { }
    }
}

function ObtenerNumElementosNuevosAJAX(pGuidPerfilUsu, pGuidPerfilOrg, pEsBandejaOrg) {
    var identCargarNov = '';
    var spanNov = $('span.novPerfilOtraIdent');

    if (spanNov.length > 0) {
        for (var i = 0; i < spanNov.length; i++) {
            identCargarNov += $(spanNov[i]).attr('id').substring($(spanNov[i]).attr('id').indexOf('_') + 1) + '&';
        }
    }
    PeticionesCookie.CargarCookie();
    PeticionesAJAX.CargarNumElementosNuevos(pGuidPerfilUsu, pGuidPerfilOrg, pEsBandejaOrg, identCargarNov, RepintarContadoresNuevosElementos, RecogerErroresAJAX);
}

function PeticionAJAX(pMetodo, pDatosPost, pFuncionOK, pFuncionKO) {
    var urlPeticion = location.protocol + "//" + location.host + "/PeticionesAJAX/" + pMetodo;
    GnossPeticionAjax(
        urlPeticion,
        pDatosPost,
        true
    ).done(function (response) {
        pFuncionOK(response)
    }).fail(function (response) {
        pFuncionKO(response);
    });
}

/**
 * M�todo que es ejecutado para mostrar informaci�n traida del backend como Mensajes nuevos, invitaciones nuevas, suscripciones nuevas...
 * @param {any} datosRecibidos
 */
/**
 * M�todo que es ejecutado para mostrar informaci�n traida del backend como Mensajes nuevos, invitaciones nuevas, suscripciones nuevas...
 * @param {any} datosRecibidos
 */
function RepintarContadoresNuevosElementos(datosRecibidos) {
    //Leemos los datos
    var arrayDatos = datosRecibidos.split('|');
    var numMensajesNuevos = arrayDatos[0];
    var numInvitacionesNuevos = arrayDatos[1];
    var numSuscripcionesNuevos = arrayDatos[2];
    var numComentariosNuevos = arrayDatos[3];

    var numMensajesSinLeer = arrayDatos[4];
    var numInvitacionesSinLeer = arrayDatos[5];
    var numSuscripcionesSinLeer = arrayDatos[6];
    var numComentariosSinLeer = arrayDatos[7];

    var numMensajesNuevosOrg = arrayDatos[8];
    var numMensajesSinLeerOrg = arrayDatos[9];
    var numInvitacionesNuevosOrg = arrayDatos[10];
    var numInvitacionesSinLeerOrg = arrayDatos[11];

    var numInvOtrasIdent = arrayDatos[12];

    // Identificaci�n de elementos HTML para controlar el n� de mensajes nuevos
    // Mensajes nuevos    
    const mensajesMenuNavegacionItem = document.querySelectorAll('.liMensajes')//$('#navegacion').find('.liMensajes');
    const suscripcionesMenuNavegacionItem = document.querySelectorAll('.liNotificaciones'); //$('#navegacion').find('.liNotificaciones');
    const comentariosMenuNavegacionItem = document.querySelectorAll('.liComentarios'); //$('#navegacion').find('.liComentarios');
    // Quitarlo -> No se utilizan 
    // const invitacionesMenuNavegacionItem = document.querySelectorAll('.liInvitaciones'); //$('#navegacion').find('.liInvitaciones');
    const contactosMenuNavegacionItem = document.querySelectorAll('.liContactos'); //$('#navegacion').find('.liContactos');


    //Cambiamos el numero de Mensajes
    DarValorALabel('infoNumMensajes', parseInt(numMensajesNuevos) + parseInt(numMensajesNuevosOrg));
    DarValorALabel('infoNumMensajesMobile', parseInt(numMensajesNuevos) + parseInt(numMensajesNuevosOrg));
    //Cambiamos el numero de Comentarios
    DarValorALabel('infoNumComentarios', numComentariosNuevos);
    DarValorALabel('infoNumComentariosMobile', numComentariosNuevos);
    //Cambiamos el numero de Invitaciones
    DarValorALabel('infoNumInvitaciones', parseInt(numInvitacionesNuevos) + parseInt(numInvitacionesNuevosOrg));
    DarValorALabel('infoNumInvitacionesMobile', parseInt(numInvitacionesNuevos) + parseInt(numInvitacionesNuevosOrg));
    //Cambiamos el numero de Notificaciones
    DarValorALabel('infoNumNotificaciones', parseInt(numInvitacionesNuevos) + parseInt(numInvitacionesNuevosOrg) + parseInt(numComentariosNuevos));
    DarValorALabel('infoNumNotificacionesMobile', parseInt(numInvitacionesNuevos) + parseInt(numInvitacionesNuevosOrg) + parseInt(numComentariosNuevos));
    //Cambiamos el numero de Suscripciones
    DarValorALabel('infoNumSuscriopciones', numSuscripcionesNuevos);
    DarValorALabel('infoNumSuscriopcionesMobile', numSuscripcionesNuevos);

    //Cambiamos el numero de Mensajes sin leer
    DarValorALabel('infNumMensajesSinLeer', parseInt(numMensajesSinLeer) + parseInt(numMensajesSinLeerOrg));
    DarValorALabel('infNumMensajesSinLeerMobile', parseInt(numMensajesSinLeer) + parseInt(numMensajesSinLeerOrg));
    // A�adir punto rojo de nuevos Mensajes
    if (parseInt(numMensajesNuevos) + parseInt(numMensajesNuevosOrg) > 0) {
        $(mensajesMenuNavegacionItem).addClass('nuevos');
    } else {
        $(mensajesMenuNavegacionItem).removeClass('nuevos');
    }
    //Cambiamos el numero de Comentarios sin leer
    DarValorALabel('infNumComentariosSinLeer', numComentariosSinLeer);
    DarValorALabel('infNumComentariosSinLeerMobile', numComentariosSinLeer);
    // A�adir punto rojo de 'nuevos' Comentarios
    if (parseInt(numComentariosNuevos) > 0) {
        $(comentariosMenuNavegacionItem).addClass('nuevos');
    } else {
        $(comentariosMenuNavegacionItem).removeClass('nuevos');
    }
    //Cambiamos el numero de Invitaciones sin leer
    DarValorALabel('infNumInvitacionesSinLeer', parseInt(numInvitacionesSinLeer) + parseInt(numInvitacionesSinLeerOrg));
    DarValorALabel('infNumInvitacionesSinLeerMobile', parseInt(numInvitacionesSinLeer) + parseInt(numInvitacionesSinLeerOrg));
    // A�adir punto rojo de 'sin leer' de Invitaciones - No se utilizan
    /*if (parseInt(numInvitacionesSinLeer) + parseInt(numInvitacionesSinLeerOrg) > 0) {
        $(invitacionesMenuNavegacionItem).addClass('nuevos');
    } else {
        $(invitacionesMenuNavegacionItem).removeClass('nuevos');
    }*/
    //Cambiamos el numero de Notificaciones sin leer
    DarValorALabel('infNumNotificacionesSinLeer', parseInt(numInvitacionesSinLeer) + parseInt(numInvitacionesSinLeerOrg) + parseInt(numComentariosSinLeer));
    DarValorALabel('infNumNotificacionesSinLeerMobile', parseInt(numInvitacionesSinLeer) + parseInt(numInvitacionesSinLeerOrg) + parseInt(numComentariosSinLeer));
    //Cambiamos el numero de Suscripciones sin leer
    DarValorALabel('infNumSuscripcionesSinLeer', numSuscripcionesSinLeer);
    DarValorALabel('infNumSuscripcionesSinLeerMobile', numSuscripcionesSinLeer);
    // A�adir punto rojo de nuevas Suscripciones
    if (parseInt(numSuscripcionesNuevos)) {
        $(suscripcionesMenuNavegacionItem).addClass('nuevos');
    } else {
        $(suscripcionesMenuNavegacionItem).removeClass('nuevos');
    }
    // Añadir punto rojo en el icono del usuario para saber si hay mensajes, notificaciones nuevas
    if ((parseInt(numMensajesSinLeer) + parseInt(numMensajesSinLeerOrg) > 0) || (parseInt(numComentariosSinLeer) > 0) || (parseInt(numSuscripcionesSinLeer))) {
        $('#user-panel-trigger').addClass("nuevos");
    } else {
        $('#user-panel-trigger').removeClass("nuevos");
    }

    if ($('.mgHerramientas').length > 0) {
        //Cambiamos el numero de Mensajes
        DarValorALabelNovedades('infoNumMensajesNovedades', numMensajesNuevos);
        //Cambiamos el numero de Comentarios
        DarValorALabelNovedades('infoNumComentariosNovedades', numComentariosNuevos);
        //Cambiamos el numero de Invitaciones
        DarValorALabelNovedades('infoNumInvitacionesNovedades', numInvitacionesNuevos);
        //Cambiamos el numero de Notificaciones
        DarValorALabelNovedades('infoNumNotificacionesNovedades', parseInt(numInvitacionesNuevos) + parseInt(numComentariosNuevos));
        //Cambiamos el numero de Suscripciones
        DarValorALabelNovedades('infoNumSuscriopcionesNovedades', numSuscripcionesNuevos);

        //Cambiamos el numero de Mensajes sin leer
        DarValorALabelPendientes('infoNumMensajesSinLeerNovedades', numMensajesSinLeer);
        //Cambiamos el numero de Comentarios sin leer
        DarValorALabelPendientes('infoNumComentariosSinLeerNovedades', numComentariosSinLeer);
        //Cambiamos el numero de Invitaciones sin leer
        DarValorALabelPendientes('infoNumInvitacionesSinLeerNovedades', numInvitacionesSinLeer);
        //Cambiamos el numero de Notificaciones sin leer
        DarValorALabelPendientes('infoNumNotificacionesSinLeerNovedades', parseInt(numInvitacionesSinLeer) + parseInt(numComentariosSinLeer));
        //Cambiamos el numero de Suscripciones sin leer
        DarValorALabelPendientes('infoNumSuscripcionesSinLeerNovedades', numSuscripcionesSinLeer);

        if (numMensajesNuevosOrg > 0 || numMensajesSinLeerOrg > 0 || numInvitacionesNuevosOrg > 0 || numInvitacionesSinLeerOrg > 0) {
            //Cambiamos el numero de Mensajes de Org
            DarValorALabelNovedades('infoNumMensajesNovedadesOrg', numMensajesNuevosOrg);
            //Cambiamos el numero de Invitaciones de Org
            DarValorALabelNovedades('infoNumInvitacionesNovedadesOrg', numInvitacionesNuevosOrg);
            //Cambiamos el numero de Notificaciones de Org
            DarValorALabelNovedades('infoNumNotificacionesNovedadesOrg', numInvitacionesNuevosOrg);
            //Cambiamos el numero de Mensajes sin leer de Org
            DarValorALabelPendientes('infoNumMensajesSinLeerNovedadesOrg', numMensajesSinLeerOrg);
            //Cambiamos el numero de Invitaciones sin leer de Org
            DarValorALabelPendientes('infoNumInvitacionesSinLeerNovedadesOrg', numInvitacionesSinLeerOrg);
            //Cambiamos el numero de Notificaciones sin leer de Org
            DarValorALabelPendientes('infoNumNotificacionesSinLeerNovedadesOrg', numInvitacionesSinLeerOrg);
        }
    }

    // Gestionar novedades de otras identidades
    if (numInvOtrasIdent != '') {
        var identRef = numInvOtrasIdent.split('&');

        for (var i = 0; i < identRef.length; i++) {
            if (identRef[i] != "") {
                var perfilID_infoNov = 'infoNov_' + identRef[i].split(':')[0];
                var perfilID_infoIdentidad = 'identidad_' + identRef[i].split(':')[0];
                var numNov = parseInt(identRef[i].split(':')[1]);
                DarValorALabel(perfilID_infoNov, numNov);
                // Añadir punto rojo de novedades
                if (numNov > 0) {
                    $(`.${perfilID_infoIdentidad}`).addClass('nuevos');
                } else {
                    $(`.${perfilID_infoIdentidad}`).removeClass('nuevos');
                }
            }
        }
    }
}

/**
 * Pintar el n�mero de elementos (mensajes sin leer, notificaciones, suscripciones) en la label correspondiente y a�ade la coletilla de "nuevos" o sin leer.
 * De momento elimino la opci�n de mostrar "nuevos" o "sin leer". 
 * @param {any} pLabelID
 * @param {any} pNumElementos
 */
function DarValorALabel(pLabelID, pNumElementos) {
    // Cambiado por nuevo Front para buscar por clase (El men� clonado tambi�n existe y no podr�a haber 2 elementos con un mismo ID)
    //if ($('#' + pLabelID).length > 0) {
    if ($('.' + pLabelID).length > 0) {
        // Cambiado por nuevo Front: Cambiado por nuevo Front para buscar por clase (El men� clonado tambi�n existe y no podr�a haber 2 elementos con un mismo ID). Hecho abajo
        // document.getElementById(pLabelID).innerHTML = pNumElementos;        

        if (pLabelID.indexOf('SinLeer') != -1) {
            // Cambiado por el nuevo Front. No deseamos que muestre "sin leer"
            // document.getElementById(pLabelID).innerHTML += '<span class="indentado">' + mensajes.sinLeer + '</span>';
        }
        else {
            // Cambiado por el nuevo Front. No deseamos que muestre "nuevos"
            // document.getElementById(pLabelID).innerHTML += '<span class="indentado">' + mensajes.nuevos + '</span>'
        }

        // Cambiado por nuevo Front para buscar por clase (El men� clonado tambi�n existe y no podr�a haber 2 elementos con un mismo ID)
        //if (pNumElementos > 0) { document.getElementById(pLabelID).style.display = ''; } else { document.getElementById(pLabelID).style.display = 'none'; }
        // 
        if (pNumElementos > 0) {
            document.querySelectorAll('.' + pLabelID).forEach(element => {
                element.style.display = '';
                element.innerHTML = pNumElementos;
            });
        }
        else {
            document.querySelectorAll('.' + pLabelID).forEach(element => element.style.display = 'none');
        }
    }
}

function DarValorALabelNovedades(pLabelID, pNumElementos) {
    if (document.getElementById(pLabelID) != null) {
        if (pNumElementos > 0) {
            document.getElementById(pLabelID).innerHTML = pNumElementos;
            document.getElementById(pLabelID).style.display = '';
        } else {
            document.getElementById(pLabelID).innerHTML = "";
            document.getElementById(pLabelID).style.display = 'none';
        }
    }
}

function DarValorALabelPendientes(pLabelID, pNumElementos) {
    try {
        if (pNumElementos > 0) {
            document.getElementById(pLabelID).innerHTML = " (" + pNumElementos + ")";
            document.getElementById(pLabelID).style.display = '';
        } else {
            document.getElementById(pLabelID).innerHTML = "";
            document.getElementById(pLabelID).style.display = 'none';
        }
    } catch (Exception) { }
}

function ObtenerNombreCortoUsuRegistroAJAX(pNombre, pApellidos) {
    PeticionesAJAX.ObtenerNombreCortoNuevoUsu(pNombre, pApellidos, ComponerUrlUsuario, ComponerUrlUsuario);
}

function ComponerUrlUsuario(datosRecibidos) {
    repintarUrl(datosRecibidos);
}

function ComprobarCorreoUsuRegistroAJAX(pCorreo, pMetodoJS) {
    PeticionesAJAX.ComprobarExisteCorreoUsuRegistro(pCorreo, pMetodoJS, pMetodoJS);
}

function ComprobarLoginUsuRegistroAJAX(pLogin, pMetodoJS) {
    PeticionesAJAX.ComprobarExisteLoginUsuRegistro(pLogin, pMetodoJS, pMetodoJS);
}


function FuncionObtenerNombreCortoOrgRegistroAJAX(pNombre) {
    PeticionesAJAX.ObtenerNombreCortoNuevaOrg(pNombre, ComponerUrlOrganizacion, ComponerUrlOrganizacion);
}

function ComponerUrlOrganizacion(datosRecibidos) {
    repintarUrl(datosRecibidos);
}

//function FiltrarBandejaMensajesAJAX(pFiltros) {
//    PeticionesAJAX.FiltrarBandejaMensajes(pFiltros, PintarResultados, PintarResultados);
//}

//function PintarResultados(datosRecibidos) {
//    ReceiveServerData(datosRecibidos, '');
//}

function mostrarConfirmacionListado(control, mensaje, accion) {

    var cont = $('#' + control);

    $('.confirmar').css('display', 'none');

    if (cont.children('.confirmar.eliminar').length > 0) {
        var anterior = cont.children('.confirmar.eliminar').eq(0);
        anterior.remove();
    }

    var htmlConfirmar =
        '<div class="confirmar eliminar confirmacionMultiple" style="display:block; z-index: 5000;">' +
        '<div class="mascara"></div>' +
        '<div class="pregunta"><span>' + mensaje + '</span>' +
        '<label class="btAzul boton botonancho floatRight right10"><strong><a class="botonConfirmacion">' + borr.si + '</a></strong></label>' +
        '<label class="btAzul boton botonancho floatRight right10"><strong><a class="botonConfirmacion">' + borr.no + '</a></strong></label>' +
        '</div>' +
        '</div>';

    var panConfirmar = $([htmlConfirmar].join(''));

    panConfirmar.prependTo(cont)
        .find('a.botonConfirmacion').click(function () { // Ambos botones hacen desaparecer la mascara
            panConfirmar.css('display', 'none');
        }).eq(0).click(accion); // pero solo el primero activa la funcionConfirmada
}

function mostrarConfirmacionSencilla(control, mensaje, accion) {
    mostrarConfirmacionSencillaEnPanel(control, mensaje, accion, 'ctl00_CPH1_divPanListado');
}

function mostrarConfirmacionSencillaEnPanel(control, mensaje, accion, panelID) {

    if (control.children('.confirmar').length > 0) {
        var anterior = control.children('.confirmar').eq(0);
        anterior.remove();
    }

    var altura = control.height() + 'px';
    var margin = (control.height() / 3) + 'px';
    //var top = control.position().top + 'px';
    var top = '0px';

    var htmlConfirmar =
        '<div class="confirmar confirmacionSencilla" style="height:' + altura + ';top:' + top + ';display:block;z-index:1000">' +
        '<div class="mascara"></div>' +
        '<div class="pregunta" style="margin-top:' + margin + '"><span>' + mensaje + '</span>' +
        '<label class="btAzul boton botonancho floatRight right10"><strong><a class="botonConfirmacion">' + borr.si + '</a></strong></label>' +
        '<label class="btAzul boton botonancho floatRight right10"><strong><a class="botonConfirmacion">' + borr.no + '</a></strong></label>' +
        '</div>' +
        '</div>';

    var panConfirmar = $([htmlConfirmar].join(''));

    panConfirmar.prependTo($('#' + panelID))
        .find('a.botonConfirmacion').click(function () { // Ambos botones hacen desaparecer la mascara
            panConfirmar.css('display', 'none');
        }).eq(0).click(accion); // pero solo el primero activa la funcionConfirmada
}

function mostrarConfirmacion(control, mensaje, accion) {
    var cont = document.getElementById(control);
    //Compruebo que este elemento no contiene mensajes de confirmación pendientes
    mascaraCancelar(mensaje, cont, accion);
}

function mostrarConfirmacion2(control, mensaje, accion, accion2) {
    var cont = document.getElementById(control);
    //Compruebo que este elemento no contiene mensajes de confirmación pendientes
    mascaraCancelarSiNo(mensaje, cont, accion, accion2);
}

function PrepararCapaModal(capaModal, mascara, seleccionarElementos, args) {
    var height = $(document).height() + 'px';
    var top = $('html').attr('scrollTop') + 'px';
    args = args.replace('$height$', height);
    args = args.replace('$top$', top);
    //console.info(args);
    window.setTimeout(function () {
        eval(args);
    }, 0);
    return false;
}

function MostrarCapaModal(capaModal, mascara, seleccionarElementos) {
    var $capa = $(capaModal);
    var capa = document.getElementById(capaModal);
    var mascara = document.getElementById(mascara);
    mascara.style.height = $(document).height() + 'px';
    var seleccionarElementos = document.getElementById(seleccionarElementos);
    seleccionarElementos.style.top = $('html').attr('scrollTop') + 'px';
    //// || $('body').attr('scrollTop') || 0) + 'px');

    $capa.fadeIn();
    //una vez llamado deberian prepararse los eventos
    $capa.find('a.icoEliminar').unbind('click').click(function () {
        $capa.fadeOut();
    });
    return false;
}

function EvitarEnvioRepetido(arg, elemento) {
    var element = document.getElementById(elemento);
    element.setAttribute("onclick", "return false;");
    window.setTimeout(function () {
        eval(arg);
    }, 0);
}

//desde aqui, evitar multiples postbacks
function Init(sender) {
    prm = Sys.WebForms.PageRequestManager.getInstance();
    //Ensure EnablePartialRendering isn't false which will prevent
    //accessing an instance of the PageRequestManager
    if (prm != null && prm) {
        if (!prm.get_isInAsyncPostBack()) {
            prm.add_initializeRequest(InitRequest);
        }
    }
}

var esperaID;
var timeout;
var continuar = false;
var _sender;
var _args;

function InitRequest(sender, args) {

    //if (prm.get_isInAsyncPostBack() & args.get_postBackElement().id =='btnRefresh') {
    //Could abort current request by using:  prm.abortPostBack();  
    //Cancel most recent request so that previous request will complete
    //and display : args.set_cancel(true);

    if (prm != null && prm.get_isInAsyncPostBack()) {
        //Esperamos a que termine el anterior postback
        clearInterval(esperaID);
        esperaID = setInterval("EsperarPostBack(prm, _sender, _args)", 100);
        //Cancelar el anterior postback si pasan 3 segundos
        _sender = sender;
        _args = args;
        clearTimeout(timeout);
        timeout = setTimeout("CancelarPostBack(prm, _sender, _args)", 3000);
        //args.set_cancel(true);
        //Anula el postback actual, aunque se relanzara cuando el anterior postback acabe o se cancele
        args.set_cancel(true);
    }
}

function EsperarPostBack(prm, sender, args) {
    if (prm == null || !prm.get_isInAsyncPostBack()) {
        clearTimeout(timeout);
        clearInterval(esperaID);
    }
}

function CancelarPostBack(prm, sender, args) {
    clearInterval(esperaID);
    if (prm != null) {
        prm.abortPostBack();
        var uniqueID = args.get_postBackElement().id.replace(/_/g, "$");
        prm._doPostBack(uniqueID, '');
    }
}
//hasta aqui, evitar multiples postbacks

function EndRequest(sender, args) {
    if (window.sweetTitles) { sweetTitles.init(); }

    // Check to see if there's an error on this request.
    //if (cambiosCurriculum){cambiosCurriculum.init();}

    if (args.get_error() != undefined) {
        var err = args.get_error();

        if (err.name != "Sys.WebForms.PageRequestManagerServerErrorException" || (err.httpStatusCode != 0 && err.httpStatusCode != 12030)) { alert(err.message); }
        // Let the framework know that the error is handled, 
        //  so it doesn't throw the JavaScript alert.
        args.set_errorHandled(true);

        EjecutarScriptsIniciales();
    }
}

function RecogerCheckComentarios() {
    var checksMarcados = '';
    //var checks = ObtenerElementosPorClase('checkSelectComent', 'input');
    var checks = $('input.checkSelectComent');
    for (var i = 0; i < checks.length; i++) {
        if ($(checks[i]).is(':checked')) { checksMarcados += checks[i].id.substring(checks[i].id.lastIndexOf('_') + 1) + ','; }
    }
    return checksMarcados;
}

function MarcarTodos(pCheck) {
    //var checks = ObtenerElementosPorClase('checkSelectComent', 'input');
    var checks = $('input.checkSelectComent');
    for (var i = 0; i < checks.length; i++) {
        $(checks[i]).attr('checked', $(pCheck).is(':checked'));
    }
}

function MarcarComentariosLeidos(pIdComent, pNumTotalComent, pContadorID, pMarcarLeidos) {
    var comentsID = pIdComent.split(',');
    var nombreClass = '';
    if (pMarcarLeidos) { nombreClass = 'busquedaDestacada'; }
    var nombreLinkMostrar = 'linkMarcarComentPerfilLeido_'; var nombreLinkOcultar = 'linkMarcarComentPerfilNOLeido_';
    if (pMarcarLeidos) { nombreLinkMostrar = 'linkMarcarComentPerfilNOLeido_'; nombreLinkOcultar = 'linkMarcarComentPerfilLeido_'; }
    for (var i = 0; i < comentsID.length; i++) {
        if (comentsID[i] != '') {
            document.getElementById('liComentario_' + comentsID[i]).className = nombreClass;
            document.getElementById(nombreLinkMostrar + comentsID[i]).style.display = ''; document.getElementById(nombreLinkOcultar + comentsID[i]).style.display = 'none';
            if (pMarcarLeidos) {
                document.getElementById('liComentario_' + comentsID[i]).style.backgroundColor = '#FFFFFF';
            }
            else {
                document.getElementById('liComentario_' + comentsID[i]).style.backgroundColor = '#F5F5F5';
            }
        }
    }
    if (document.getElementById(pContadorID).innerHTML.indexOf('(') != -1) {
        document.getElementById(pContadorID).innerHTML = document.getElementById(pContadorID).innerHTML.replace(document.getElementById(pContadorID).innerHTML.substring(document.getElementById(pContadorID).innerHTML.indexOf('(')), '(' + pNumTotalComent + ')');
    }
    else { document.getElementById(pContadorID).innerHTML += ' (' + pNumTotalComent + ')'; }

    if (pNumTotalComent > 0) { document.getElementById('infoNumComentariosSinLeer').innerHTML = pNumTotalComent; } else { document.getElementById('infoNumComentariosSinLeer').innerHTML = ''; }

    OcultarUpdateProgress();
}

function EstablecerContadorComentNoLeido(pNumTotalComent) {
    DarValorALabel('infNumComentariosSinLeer', pNumTotalComent);
}

function EstablecerContadorMensajesNuevos(pNumTotalComent) {
    DarValorALabel('infoNumMensajes', pNumTotalComent);
    DarValorALabelNovedades('infoNumMensajesNovedades', pNumTotalComent);
}

function DisminuirContadorMensajeNoLeido(pBandejaOrg) {
    try {

        var numMenText = document.getElementById('infNumMensajesSinLeer').innerHTML;
        var numMen = 0;

        if (numMenText.trim() != '') {
            if (numMenText.indexOf('<') != -1) {
                numMenText = numMenText.substring(0, numMenText.indexOf('<'));
            }

            numMen = parseInt(numMenText.trim()) - 1;
        }

        DarValorALabel('infNumMensajesSinLeer', numMen);

        var infBandeja = 'infoNumMensajesSinLeerNovedades';

        if (pBandejaOrg) {
            infBandeja = 'infoNumMensajesSinLeerNovedadesOrg';
        }

        numMenText = document.getElementById(infBandeja).innerHTML;
        numMen = 0;

        if (numMenText.trim() != '') {
            if (numMenText.indexOf('(') != -1) {
                numMenText = numMenText.substring(numMenText.indexOf('(') + 1);
                numMenText = numMenText.substring(0, numMenText.indexOf(')'));
            }

            numMen = parseInt(numMenText.trim()) - 1;
        }

        DarValorALabelPendientes(infBandeja, numMen);
    } catch (Exception) { }
}

function DisminuirContadorInvitacionesNoLeido(pBandejaOrg) {
    var numMenText = document.getElementById('infNumInvitacionesSinLeer').innerHTML;
    var numMen = 0;

    if (numMenText.trim() != '') {
        if (numMenText.indexOf('<') != -1) {
            numMenText = numMenText.substring(0, numMenText.indexOf('<'));
        }

        numMen = parseInt(numMenText.trim()) - 1;
    }

    DarValorALabel('infNumInvitacionesSinLeer', numMen);

    var infBandeja = 'infoNumInvitacionesSinLeerNovedades';

    if (pBandejaOrg) {
        infBandeja = 'infoNumInvitacionesSinLeerNovedadesOrg';
    }

    numMenText = document.getElementById(infBandeja).innerHTML;
    numMen = 0;

    if (numMenText.trim() != '') {
        if (numMenText.indexOf('(') != -1) {
            numMenText = numMenText.substring(numMenText.indexOf('(') + 1);
            numMenText = numMenText.substring(0, numMenText.indexOf(')'));
        }

        numMen = parseInt(numMenText.trim()) - 1;
    }

    DarValorALabelPendientes(infBandeja, numMen);
}

function DisminuirContadorSuscripcionesNoLeido(pNumDisminucion) {
    var numMenText = document.getElementById('infNumSuscripcionesSinLeer').innerHTML;
    var numMen = 0;

    if (numMenText.trim() != '') {
        if (numMenText.indexOf('<') != -1) {
            numMenText = numMenText.substring(0, numMenText.indexOf('<'));
        }

        numMen = parseInt(numMenText.trim()) - pNumDisminucion;
    }

    DarValorALabel('infNumSuscripcionesSinLeer', numMen);

    var infBandeja = 'infoNumSuscripcionesSinLeerNovedades';

    numMenText = document.getElementById(infBandeja).innerHTML;
    numMen = 0;

    if (numMenText.trim() != '') {
        if (numMenText.indexOf('(') != -1) {
            numMenText = numMenText.substring(numMenText.indexOf('(') + 1);
            numMenText = numMenText.substring(0, numMenText.indexOf(')'));
        }

        numMen = parseInt(numMenText.trim()) - pNumDisminucion;
    }

    DarValorALabelPendientes(infBandeja, numMen);
}


function RecogerValorCheckPendientes() {
    var valor = new String($('#chkSoloPedientesLeer').is(':checked'));
    if (document.getElementById('chkSoloComentUsuLeer') != null) {
        valor += ',' + new String($('#chkSoloComentUsuLeer').is(':checked')) + ',' + new String($('#chkSoloComentOrgLeer').is(':checked'));
    }
    return valor;
}

function CallServerSelectorGrupoAmigos(arg, context) {
    window.setTimeout(function () {
        eval(arg);
    }, 0);
}

function DarFormato(arg, context) {
    var element = document.getElementById(arg);
    var parametros = context.split("&");
    var accion = parametros[0];
    if (accion == 'negrita') {
        Envolver("'''", "'''", element);
    }
    else if (accion == 'cursiva') {
        Envolver("''", "''", element);
    }
    else if (accion == 'enlaceInterno') {
        Envolver("[[", "]]", element);
    }
    else if (accion == 'enlaceExterno') {
        Envolver("[", "]", element);
    }
    else if (accion == 'titulo1') {
        //Envolver("\\n==","==\\n", element);
        Envolver("==", "==", element);
    }
    else if (accion == 'titulo2') {
        //Envolver("\\n===","===\\n", element);
        Envolver("===", "===", element);
    }
    else if (accion == 'titulo3') {
        //Envolver("\\n====","====\\n", element);
        Envolver("====", "====", element);
    }
    else if (accion == 'imagen') {
        var imagen = document.getElementById(parametros[1]);
        Envolver("[[Imagen:" + imagen.value, "]]", element);
        imagen.value = "";
    }
}

function Envolver(tagOpen, tagClose, element) {
    var txtarea = element;
    var selText, isSample = false;
    if (document.selection && document.selection.createRange) { // IE/Opera            ////save window scroll position
        ////if (document.documentElement && document.documentElement.scrollTop)
        ////    var winScroll = document.documentElement.scrollTop
        ////else if (document.body)
        ////    var winScroll = document.body.scrollTop;

        //get current selection
        txtarea.focus();
        var range = document.selection.createRange();
        selText = range.text;
        //////insert tags
        ////checkSelectedText();
        range.text = tagOpen + selText + tagClose;
        //alert(range.text);
        //////mark sample text as selected
        ////if (isSample && range.moveStart) {
        ////    if (window.opera)
        ////        tagClose = tagClose.replace(/\n/g,'');
        ////    range.moveStart('character', - tagClose.length - selText.length); 
        ////    range.moveEnd('character', - tagClose.length); 
        ////}
        //range.select();
        //////restore window scroll position
        ////if (document.documentElement && document.documentElement.scrollTop)
        ////    document.documentElement.scrollTop = winScroll
        ////else if (document.body)
        ////    document.body.scrollTop = winScroll;
    }
    else if (txtarea.selectionStart || txtarea.selectionStart == '0') {// Mozilla
        //save textarea scroll position
        var textScroll = txtarea.scrollTop;
        //get current selection
        txtarea.focus();
        var startPos = txtarea.selectionStart;
        var endPos = txtarea.selectionEnd;
        selText = txtarea.value.substring(startPos, endPos);
        //insert tags
        //checkSelectedText();
        txtarea.value = txtarea.value.substring(0, startPos)
            + tagOpen + selText + tagClose
            + txtarea.value.substring(endPos, txtarea.value.length);
        //set new selection
        if (isSample) {
            //txtarea.selectionStart = startPos + tagOpen.length;
            //txtarea.selectionEnd = startPos + tagOpen.length + selText.length;
        } else {
            //txtarea.selectionStart = startPos + tagOpen.length + selText.length + tagClose.length;
            //txtarea.selectionEnd = txtarea.selectionStart;
        }
        //restore textarea scroll position
        txtarea.scrollTop = textScroll;
    }
}

function CallServerListadoBlogs(arg, context) {
    window.setTimeout(function () {
        eval(arg);
    }, 0);
}

function CallServerSelectorCategorias(arg, id, context) {
    window.setTimeout(function () {
        //eval(arg);
        WebForm_DoCallback(id, context, ReceiveServerData, '', null, false)
    }, 0);
}

function CallServerSelectorCategoriasBlog(arg, context) {
    window.setTimeout(function () {
        eval(arg);
    }, 0);
}

function CallServerSelectorEditoresBlog(arg, context) {
    window.setTimeout(function () {
        eval(arg);
    }, 0);
}

function addTag(evento, control) {
    var contenedor = $('#contenedorFiltros');
    var $this = $(control).parents('a');
    var texto = $this.text().replace(/\n/g, '').replace(/^\s*|\s*$/g, '');
    if (contenedor.parents('div.panel').is(':hidden')) {
        contenedor.parents('div.panel').show('blind', { direction: 'vertical' }, 600, function () {
            $this.effect('transfer', { to: objetivo }, 400);
        }).prevAll().find('a.desplegable').eq(0).addClass('activo');
    }
    var tagsDentro = contenedor.find('a');
    var estaDentro = false;
    for (i = 0; i < tagsDentro.length; i++) {
        if (tagsDentro[i].innerHTML == texto) { estaDentro = true; }
    }
    if (!estaDentro) {
        var objetivo = $(['<a id="idTemp">', texto, '</a><input type="hidden" name="filtros" value="', $this.text(), '" />'].join('')).appendTo(contenedor);
        contenedor.parent().andSelf().css('display', 'block');
        $this.effect('transfer', { to: objetivo }, 400);
        objetivo.click(function () {
            $(this).remove();
            eliminarTagsBusqueda(texto); return false;
            if (!contenedor.find('a').length) { contenedor.parent().andSelf().hide(); }
            return false;
        });
    }
    return false;
}

function CallServerSelectorCategorias(arg, id, context) {
    window.setTimeout(function () {
        //eval(arg);
        WebForm_DoCallback(id, context, ReceiveServerData, '', null, false)
    }, 0);
}

function marcarElementos(pCheck, pClave, pIdTxt) {
    var txtOrgSel = document.getElementById(pIdTxt);
    if ($(pCheck).is(':checked')) {
        txtOrgSel.value = txtOrgSel.value + pClave + '|';
    }
    else {
        txtOrgSel.value = txtOrgSel.value.replace(pClave + '|', '');
    }
}

function CallServerSelectorPersonas(arg, context) {
    window.setTimeout(function () {
        eval(arg);
    }, 0);
}

function CallServerSelectorRolUsuario(arg, context) {
    window.setTimeout(function () {
        eval(arg);
    }, 0);
}

var votar = null
function Votar(arg, elemento) {
    var element = document.getElementById(elemento);
    //console.info(element.value);
    votar = window.setTimeout(function () {
        CallServer(arg + "&" + element.value);
    }, 0);
}

var cambiarPagina = null;
function CambiarPagina(arg, context) {
    var pagina = document.getElementById('pagina');
    if (pagina != null) {
        var params = arg.split('&'); pagina.value = params[2];
    }
    var updateProgress = document.getElementById('ctl00_CPH1_UpdateProgress1');
    if (updateProgress != null) {
        updateProgress.style.display = "block";
    }
    //cambiarPagina = window.setTimeout(function() {

    CallServer(arg, context);
    //}, 0);
}


/* Funciones Desplegables */

function DesplegarAccion(pBoton, pPanelID, pPanelName, pCallBackPage, pArg) {
    var panel = document.getElementById(pPanelID);

    panel.children[0].children[0].style.display = 'block';
    panel.children[0].children[1].style.display = 'none';
    panel.children[0].children[2].style.display = 'none';
    panel.children[0].children[3].style.display = 'none';

    panel.children[0].style.display = 'block';
    panel.style.display = '';

    if (pBoton != null && typeof (pBoton.parentNode) != 'undefined') {
        DesactivarBotonesActivosDespl();
        pBoton.parentNode.className += ' active';
    }

    var target = '__Page';

    if (!pCallBackPage) {
        //target = pPanelID.replace(/\_/g,'$');
        target = pPanelName;
    }

    WebForm_DoCallback(target, 'CargarControlDesplegar' + pArg, ReceiveServerData, "", null, false);
}

function MostrarPanelAccionDesp(pPanelID, pHtml) {
    MostrarPanelAccionDesp(pPanelID, pHtml, false)
}

function MostrarPanelAccionDesp(pPanelID, pHtml, pPintarCargando) {
    MostrarPanelAccionDesp(pPaneID, pHtml, pPintarCargando, false);
}

function MostrarPanelAccionDesp(pPanelID, pHtml, pPintarCargando, pSoloAux) {
    var panel = document.getElementById(pPanelID);


    if (pHtml != null) {
        var html = pHtml.replace(/\j001j/g, '"').replace(/\j002j/g, '\'');
        panel.children[0].children[2].innerHTML = html;
    }
    if (!pSoloAux) {

        panel.children[0].children[0].style.display = 'none';
        panel.children[0].children[1].style.display = 'none';
        panel.children[0].children[2].style.display = 'block';
        panel.children[0].children[3].style.display = 'none';
        //panel.children[0].children[4].style.display = 'block';


        panel.children[0].style.display = 'block';
        panel.style.display = '';

    }

    else {
        panel.children[0].children[0].style.display = 'none';
        panel.children[0].children[1].style.display = 'none';
        panel.children[0].children[2].style.display = 'none';
        panel.children[0].children[3].style.display = 'block';
        panel.children[0].children[4].style.display = 'block';


        panel.children[0].style.display = 'block';
        panel.style.display = '';

    }
    if (pPintarCargando) {
        var htmlCargando = '<img id="ctl00_ctl00_controles_master_controlcargando_ascx_imgEspera" src="http://static.gnoss.net/img/espera.gif"><h3> Cargando...</h3>';
        panel.children[0].children[3].innerHTML = htmlCargando;
    }
}

function AceptarPanelAccion(pPanelID, pOk, pHtml) {
    var panel = document.getElementById(pPanelID);

    if (pHtml != null && pHtml != '' && pHtml.indexOf('<p') != 0) {
        pHtml = '<p>' + pHtml + '</p>';
    }

    if (pOk) {
        panel.children[0].children[1].children[0].style.display = 'block';
        panel.children[0].children[1].children[1].style.display = 'none';

        panel.children[0].children[1].children[0].innerHTML = pHtml;

        // Solo si ha ido bien
        panel.children[0].children[0].style.display = 'none';
        panel.children[0].children[2].style.display = 'none';
        panel.children[0].children[3].style.display = 'none';
    }
    else {
        panel.children[0].children[1].children[0].style.display = 'none';
        panel.children[0].children[1].children[1].style.display = 'block';

        panel.children[0].children[1].children[1].innerHTML = pHtml;
    }

    panel.children[0].children[1].style.display = 'block';
    panel.children[0].children[4].style.display = 'block';

    panel.children[0].style.display = 'block';
    panel.style.display = '';

    DesactivarBotonesActivosDespl();
}

function CerrarPanelAccion(pPanelID) {
    var panel = document.getElementById(pPanelID);
    panel.children[0].style.display = 'none';
    panel.style.display = 'none';

    DesactivarBotonesActivosDespl();
}

function DesactivarBotonesActivosDespl() {
    var btnActivos = $('.active');
    for (var i = 0; i < btnActivos.length; i++) {
        btnActivos[i].className = btnActivos[i].className.replace('active', '');
    }
}

/* Fin Funciones Desplegables */

var diffHoras = null;

/* Fechas actividad reciente */
function MontarFechas() {
    if (typeof (diffHoras) == 'undefined' || diffHoras == null || isNaN(diffHoras)) {
        var fechaServidor = new Date($('#inpt_serverTime').val());
        var fechaCliente = new Date();
        var diffHoras = parseInt((fechaServidor.getTime() / (1000 * 60 * 60)) - (fechaCliente.getTime() / (1000 * 60 * 60)));
    }

    $('.resource .fechaLive').each(function () {
        var enlace = $(this);
        enlace.removeClass("fechaLive");
        var fecha = enlace[0].innerHTML;
        var fechaRecurso = new Date(fecha);
        fechaRecurso.setHours(fechaRecurso.getHours() - diffHoras);
        DifFechasEvento(fechaRecurso.format("yyyy/MM/dd HH:mm"), enlace);
    });
}

function DifFechasEvento(fecha, contenedor) {
    var factual = new Date();

    var finicio = new Date(fecha);
    var dateDifDay = parseInt((factual.getTime() / (1000 * 60 * 60 * 24)) - (finicio.getTime() / (1000 * 60 * 60 * 24)));
    var difD = dateDifDay;
    if (dateDifDay < 7 && dateDifDay > 0) {
        var diaInicio = finicio.getDay();
        var diaActual = factual.getDay();
        if (diaInicio >= diaActual) {
            diaActual = diaActual + 7;
        }
        var difD = diaActual - diaInicio;
    }
    var difH = parseInt((factual.getTime() / (1000 * 60 * 60)) - (finicio.getTime() / (1000 * 60 * 60)));
    var difM = parseInt((factual.getTime() / (1000 * 60)) - (finicio.getTime() / (1000 * 60)));
    //Montamos la frase para el tiempo pasado
    var tiempoPasado = '';
    if (difD < 7) {
        if (difD == 0) {
            if (difH == 0) {
                if (difM <= 1) {
                    tiempoPasado = tiempo.hace + ' 1 ' + tiempo.minuto;
                }
                else {
                    tiempoPasado = tiempo.hace + ' ' + difM + ' ' + tiempo.minutos;
                }
            }
            else if (difH == 1) {
                tiempoPasado = tiempo.hace + ' 1 ' + tiempo.hora;
            }
            else {
                tiempoPasado = tiempo.hace + ' ' + difH + ' ' + tiempo.horas;
            }
        }
        else if (difD == 1) {
            tiempoPasado = tiempo.ayer;
        }
        else {
            tiempoPasado = tiempo.hace + ' ' + difD + ' ' + tiempo.dias;
        }
    }
    else {
        var dia = finicio.getDate();
        if (dia < 10) { dia = '0' + dia; }
        var mes = finicio.getMonth() + 1;
        if (mes < 10) { mes = '0' + mes; }

        //var fecha = dia + '/' + mes + '/' + finicio.getFullYear();
        var fechaPintado = tiempo.fechaBarras.replace('@1@', dia).replace('@2@', mes).replace('@3@', finicio.getFullYear());
        tiempoPasado = tiempo.eldia + ' ' + fechaPintado;
    }
    contenedor.html(tiempoPasado);
}

/* Fin fechas actividad reciente */

/* Enganchamos el evento click cuando es necesario, sabemos que en IE y las ultimas versiones de ff(16) y chrome(22) funciona bien*/
var is_ie = navigator.userAgent.indexOf("MSIE") > -1;
var is_chrome_nuevo = false;
if (navigator.userAgent.indexOf("Chrome/") > -1) {
    var nav = navigator.userAgent.substring(navigator.userAgent.indexOf("Chrome/") + 7);
    if (nav.indexOf(' ') > -1) {
        nav = nav.substring(0, nav.indexOf(' '));
    }
    if (nav.indexOf('.') > -1) {
        nav = nav.substring(0, nav.indexOf('.'));
    }
    if (parseInt(nav) > 21) {
        is_chrome_nuevo = true;
    }
}
var is_firefox_nuevo = false;
if (navigator.userAgent.indexOf("Firefox/") > -1) {
    var nav = navigator.userAgent.substring(navigator.userAgent.indexOf("Firefox/") + 8);
    if (nav.indexOf(' ') > -1) {
        nav = nav.substring(0, nav.indexOf(' '));
    }
    if (nav.indexOf('.') > -1) {
        nav = nav.substring(0, nav.indexOf('.'));
    }
    if (parseInt(nav) > 15) {
        is_firefox_nuevo = true;
    }
}

if (!is_ie && !is_chrome_nuevo && !is_firefox_nuevo) {
    HTMLElement.prototype.click = function () {
        var evt = this.ownerDocument.createEvent('MouseEvents');
        evt.initMouseEvent('click', true, true, this.ownerDocument.defaultView, 1, 0, 0, 0, 0, false, false, false, false, 0, null);
        this.dispatchEvent(evt);
    }
}

/* Fin Enganchamos el evento click cuando es necesario*/

function AnyadirGrupoAContacto(contactoId) {
    WebForm_DoCallback('__Page', 'AgregarGrupoAContacto&' + contactoId + '&', ReceiveServerData, '', null, false);

}

function ConectarConUsuario(pIdentidad, pNombre) {
    var panelResul = 'divResultadosConRec';

    if ($('#' + panelResul).length == 0) {
        panelResul = 'divResultados';
    }

    var numSeleccioados = 0;
    var args = 'ConectarConUsuario&' + pIdentidad;
    mostrarConfirmacionSencillaEnPanel($('#divEtiquetas' + pIdentidad).parent().parent(), invitaciones.invitarContactoAceptar.replace('@1@', pNombre).replace('@2@', pNombre), function () {
        MostrarUpdateProgress();
        WebForm_DoCallback('__Page', args, ReceiveServerData, '', null, false);
    }, panelResul);
}

function IgnorarUsuario(pIdentidad, pNombre) {
    var panelResul = 'divResultadosConRec';

    if ($('#' + panelResul).length == 0) {
        panelResul = 'divResultados';
    }

    var numSeleccioados = 0;
    var args = 'IgnorarUsuario&' + pIdentidad;
    mostrarConfirmacionSencillaEnPanel($('#divEtiquetas' + pIdentidad).parent().parent(), invitaciones.ingnorarContactoAceptar.replace('@1@', pNombre), function () {
        MostrarUpdateProgress();
        WebForm_DoCallback('__Page', args, ReceiveServerData, '', null, false);
    }, panelResul);
}


function IgnorarProyecto(pProyecto, pNombre) {
    var panelResul = 'divResultadosComRec';

    if ($('#' + panelResul).length == 0) {
        panelResul = 'divResultados';
    }

    var numSeleccioados = 0;
    var args = 'IgnorarProyecto&' + pProyecto;
    mostrarConfirmacionSencillaEnPanel($('#divEtiquetas' + pProyecto).parent().parent(), invitaciones.ingnorarProyAceptar.replace('@1@', pNombre), function () {
        MostrarUpdateProgress();
        WebForm_DoCallback('__Page', args, ReceiveServerData, '', null, false);
    }, panelResul);
}

function HacerseMiembroProy(pProyecto, pNombre) {
    var panelResul = 'divResultadosComRec';

    if ($('#' + panelResul).length == 0) {
        panelResul = 'divResultados';
    }

    var numSeleccioados = 0;
    var args = 'HacerseMiembroProy&' + pProyecto;
    mostrarConfirmacionSencillaEnPanel($('#divEtiquetas' + pProyecto).parent().parent(), invitaciones.haztemiembroProyAceptar.replace('@1@', pNombre), function () {
        MostrarUpdateProgress();
        WebForm_DoCallback('__Page', args, ReceiveServerData, '', null, false);
    }, panelResul);
}

function SolicitarAccesoProy(pProyecto, pNombre) {
    var panelResul = 'divResultadosComRec';

    if ($('#' + panelResul).length == 0) {
        panelResul = 'divResultados';
    }

    var numSeleccioados = 0;
    var args = 'SolicitarAccesoProy&' + pProyecto;
    mostrarConfirmacionSencillaEnPanel($('#divEtiquetas' + pProyecto).parent().parent(), invitaciones.solicitaraccesoProyAceptar.replace('@1@', pNombre), function () {
        MostrarUpdateProgress();
        WebForm_DoCallback('__Page', args, ReceiveServerData, '', null, false);
    }, panelResul);
}

/* Seleccionar & eliminar Autocompletar*/
function seleccionarAutocompletar(nombre, identidad, PanContenedorID, txtHackID, ContenedorMostrarID, txtFiltroID) {
    document.getElementById(txtFiltroID).value = '';
    $('#selector').css('display', 'none');
    contenedor = document.getElementById(PanContenedorID);
    if (contenedor.innerHTML.trim().indexOf('<ul') == 0) { contenedor.innerHTML = contenedor.innerHTML.replace('<ul class=\"icoEliminar\">', '').replace('</ul>', ''); }
    contenedor.innerHTML = '<ul class=\"icoEliminar\">' + contenedor.innerHTML + '<li>' + nombre + '<a class="remove" onclick="javascript:eliminarAutocompletar(this,\'' + identidad + '\',\'' + PanContenedorID + '\',\'' + txtHackID + '\',\'' + ContenedorMostrarID + '\');">' + textoRecursos.Eliminar + '</a></li>' + '</ul>';

    document.getElementById(txtHackID).value = document.getElementById(txtHackID).value + "&" + identidad;
    if (ContenedorMostrarID != null) {
        $('#' + ContenedorMostrarID + '').css('display', '');
    }
}

function eliminarAutocompletar(nombre, identidad, PanContenedorID, txtHackID, ContenedorMostrarID) {
    contenedor = document.getElementById(PanContenedorID);
    contenedor.children[0].removeChild(nombre.parentNode);
    document.getElementById(txtHackID).value = document.getElementById(txtHackID).value.replace('&' + identidad, '');
    if (ContenedorMostrarID != null && document.getElementById(txtHackID).value == '') {
        $('#' + ContenedorMostrarID + '').css('display', 'none');
    }
}

function ObtenerEntidadesLOD(pUrlServicio, pUrlBaseEnlaceTag, pDocumentoID, pEtiquetas, pIdioma) {
    var servicio = new WS(pUrlServicio, WSDataType.jsonp);

    var metodo = 'ObtenerEntidadesLOD';
    var params = {};
    params['documentoID'] = pDocumentoID;
    params['tags'] = urlEncode(pEtiquetas);
    params['urlBaseEnlaceTag'] = pUrlBaseEnlaceTag;
    params['idioma'] = pIdioma;
    servicio.call(metodo, params, function (data) {
        $('.listTags').find('a').each(function () {
            var tag = this.textContent;
            if (data[tag] != null) {
                $(this).parent().attr('title', data[tag]);
                $(this).parent().attr('class', 'conFbTt');
            }
        });
        $(".conFbTt").each(function () {
            if (this.title) {
                this.tooltipData = this.title;
                this.removeAttribute('title');
            }
        }).hover(mostrarFreebaseTt, ocultarFreebaseTt).mousemove(posicionarFreebaseTt);
    });
}


/*                                                             Tooltips para freebase (conFbTt)
*---------------------------------------------------------------------------------------------
*/

var necesarioPosicionar = true;
var mouseOnTooltip = false;
var tooltipActivo = '';
var cerrar = 0;

var posicionarFreebaseTt = function (event) {
    if (necesarioPosicionar && $("div.tooltip").length > 0) {
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
        if (sumaX > anchoVentana) {
            tPosX = anchoVentana - $("div.tooltip").width() - 30;
        }

        if (tPosY < altoScroll) {
            tPosY = event.pageY + 12
        }

        $("div.tooltip").css({
            top: tPosY,
            left: tPosX
        });
        necesarioPosicionar = false;
    }
}

var mostrarFreebaseTt = function (event) {
    var hayTooltip = $("div.tooltip").length != 0;
    var tooltipDiferente = false;

    if (hayTooltip && tooltipActivo != '' && $(this).hasClass('conFbTt')) {
        tooltipDiferente = ($(this).text() != tooltipActivo);
    }

    if (!hayTooltip || tooltipDiferente) {
        $("div.tooltip").remove();
        var textoTt = (this.tooltipData) ? this.tooltipData : $(this).text();
        tooltipActivo = $(this).text();
        $("<div class='tooltip entidadesEnlazadas' style='display:none; width:350px; height:auto;padding:0; opacity:1;' onmousemove='javascript:mouseSobreTooltip()' onmouseover='javascript:mouseSobreTooltip()' onmouseout='javascript:mouseFueraTooltip()'><div class='relatedInfoWindow'><p class='poweredby'>Powered by <a href='http://www.gnoss.com'><strong>Gnoss</strong></a></p><div class='wrapRelatedInfoWindow'>" + textoTt + "</div> <p><em>" + $('input.inpt_avisoLegal').val() + "</em></p></div></div>")
            .appendTo("body")
            .fadeIn();

        $("div.tooltip").hover(mostrarFreebaseTt, ocultarFreebaseTt).mousemove(posicionarFreebaseTt);
        if (tooltipDiferente) {
            necesarioPosicionar = true;
        }
        posicionarFreebaseTt(event);
    }
    cerrar++;
}

var ocultarFreebaseTt = function () {
    setTimeout(quitarFreebaseTt, 1000);
}

function quitarFreebaseTt() {
    cerrar--;
    if ((cerrar <= 0) && (!mouseOnTooltip)) {
        $("div.tooltip").remove();
        necesarioPosicionar = true;
    }
}

function mouseFueraTooltip() {
    mouseOnTooltip = false;
    if (cerrar <= 0) {
        setTimeout(quitarFreebaseTt, 1000);
    }
}

function mouseSobreTooltip() {
    mouseOnTooltip = true;
}

/*
*---------------------------------------------------------------------------------------------
*/

function ObtenerParametrosProyOrigen(pPestanya) {
    var pestanyas = hackBusquedaPestProyOrigen.split('[||]');

    if (pestanyas.length > 0) {
        for (var i = 0; i < pestanyas.length; i++) {
            if (pestanyas[i].split('[|]')[0] == pPestanya) {
                return pestanyas[i].split('[|]')[1];
            }
        }
    }

    return '';
}

function OnUploadCompleted(pRutaImg) {
    urlCKEImg.val(pRutaImg/*.replace('https://', 'http://')*/);
    botonCKEAceptar.click();
}

function ElemVisible(pClase) {
    var elems = $(pClase);
    for (var i = 0; i < elems.length; i++) {
        if ($(elems[i]).is(':visible')) {
            return elems[i];
        }
    }
}


function DesplegarAccionListado(pControl, pArg, pDocumentoID, pProyectoID) {
    //Limpio los paneles anteriores:
    $('.divContAccList').html('');

    ContenedorDesplBusqueda = $('.divContAccList', $(pControl).parent().parent().parent().parent())[0];
    $(ContenedorDesplBusqueda).html($('#divContDespBusq').html());

    var docIDArg = '&docBusqID=' + pDocumentoID + '&';
    if (pProyectoID != undefined && pProyectoID != '') {
        var proyIDArg = '&proyID=' + pProyectoID + '&';
    }

    DesplegarAccion(pControl, ClientDesplegarID, UniqueDesplegarID, false, docIDArg + proyIDArg + pArg);
}

function IntercambiarOnclickTag(pControl, pTexto1, pTexto2) {
    var clase = pControl.className;
    var controles = $('.' + clase);

    for (var i = 0; i < controles.length; i++) {
        $(controles[i]).html($(controles[i]).html().replace(pTexto1, pTexto2));

        var tag = controles[i].attributes['tag'].value;
        controles[i].attributes['tag'].value = controles[i].attributes['onclick'].value;
        controles[i].attributes['onclick'].value = tag;
    }
}



function AumentarNumElemento(pElemento) {
    try {
        if (pElemento.length > 0) {
            var texto = pElemento.html();

            if (texto != '') {
                if (texto.indexOf('+') != -1) {
                    texto = texto.replace('+', '');
                    var num = parseInt(texto) + 1;
                    pElemento.html(' + ' + num)
                }
                else if (texto.indexOf('-') != -1) {
                    texto = texto.replace('-', '');
                    var num = parseInt(texto) - 1;

                    if (num == 0) {
                        pElemento.html(num)
                    }
                    else {
                        pElemento.html(' - ' + num)
                    }
                }
                else {
                    var num = parseInt(texto) + 1;
                    pElemento.html(' + ' + num)
                }
            }
        }
    }
    catch (ex) { }
}

function EnviarAccGogAnac(pCat, pTipo, pLabel) {
    try {
        if (typeof pageTracker != 'undefined') {
            pageTracker._trackEvent(pCat, pTipo, pLabel);
        }
    } catch (ex) {
    }
}


var arriba;
function SubirPagina() {
    if (document.body.scrollTop != 0 || document.documentElement.scrollTop != 0) {
        window.scrollBy(0, -50);
        arriba = setTimeout('SubirPagina()', 5);
    }
    else clearTimeout(arriba);
}

function MostrarPopUpCentrado(url, width, height) {
    var left = (screen.width - width) / 2;
    var top = (screen.height - height) / 2;
    var windowProperties = 'height=' + height + ',width=' + width + ',top=' + top + ',left=' + left + ',scrollbars=NO,resizable=NO,menubar=NO,toolbar=NO,location=NO,statusbar=NO,fullscreen=NO';
    window.open(url, '', windowProperties);
}

/****** CHAT **********/

function ActivarChat(perfil, activar, nombre) {
    if ($('#divchatframe').length == 0) {
        var panel = document.createElement("div");
        //$(panel).css('left', '5px');
        //$(panel).css('position', 'fixed');
        //$(panel).css('width', '325px');
        //$(panel).css('padding', '2px');
        $(panel).attr('id', 'divchatframe');
        $(panel).attr('class', 'chat');
        $(panel).css('height', '20px');

        var panelInt = document.createElement("div");
        $(panelInt).attr('id', 'divChat');
        $(panelInt).css('display', 'none');
        $(panelInt).css('height', '50px');
        panel.appendChild(panelInt);

        var panelAlerta = document.createElement("div");
        $(panelAlerta).attr('id', 'divAlertaChat');
        $(panelAlerta).css('display', 'none');
        panel.appendChild(panelAlerta);


        var panelCerrar = document.createElement("div");
        $(panelCerrar).attr('class', 'menuChat');
        panel.appendChild(panelCerrar);

        var aCerrar = document.createElement("a");
        panelCerrar.appendChild(aCerrar);
        $(aCerrar).attr('onclick', 'desactivarUsuChat();');
        $(aCerrar).html(textChat.desconectar);
        $(aCerrar).attr('id', 'hlCerrChat');

        var aMaximizar = document.createElement("a");
        panelCerrar.appendChild(aMaximizar);
        $(aMaximizar).attr('onclick', 'ExpandirChat(' + activar + ');');
        $(aMaximizar).html(textChat.expandir);
        $(aMaximizar).attr('id', 'hlMaxChat');

        if (!activar) {
            $(aMaximizar).html('Conectar');
            $(aCerrar).css('display', 'none');
        }

        var aMinChat = document.createElement("a");
        panelCerrar.appendChild(aMinChat);
        $(aMinChat).attr('onclick', 'ContraerChat();');
        $(aMinChat).html(textChat.contraer);
        $(aMinChat).attr('id', 'hlMinChat');
        $(aMinChat).css('display', 'none');

        var inputPerfil = document.createElement("input");
        $(inputPerfil).css('display', 'none');
        $(inputPerfil).attr('type', 'text');
        $(inputPerfil).attr('id', 'txtHackPerfilActual');
        $(inputPerfil).val(perfil);
        panelCerrar.appendChild(inputPerfil);

        var inputNomAct = document.createElement("input");
        $(inputNomAct).css('display', 'none');
        $(inputNomAct).attr('type', 'text');
        $(inputNomAct).attr('id', 'txtHackNombreActual');
        panelCerrar.appendChild(inputNomAct);

        document.body.appendChild(panel);
        $('#txtHackNombreActual').val(nombre);
        ColocarChat();

        if (activar) {
            MontarChat();
        }
    }
    else {
        $('#divchatframe').css('display', '');
    }
}

function ColocarChat() {
    $('#divchatframe').css('top', ($(window).height() - $('#divchatframe').outerHeight() - 20));
}

function ExpandirChat(activado) {
    $('#divChat').css('height', '325px');
    $('#divchatframe').css('height', '345px');
    $('#divChat').css('display', '');
    $('#hlMaxChat').css('display', 'none');
    $('#hlMinChat').css('display', '');
    ColocarChat();

    if (!activado) {
        MontarChat();
        $('#hlMaxChat').attr('onclick', 'ExpandirChat(true);');
        $('#hlMaxChat').html('Expandir');
        $('#hlCerrChat').css('display', '');
    }
}

function ContraerChat() {
    $('#divChat').css('height', '50px');
    $('#divchatframe').css('height', '20px');
    $('#divChat').css('display', 'none');
    $('#hlMaxChat').css('display', '');
    $('#hlMinChat').css('display', 'none');
    ColocarChat();
    MostrarChatsActivos(true);
}

function MostrarCargandoChat() {
    $('#divChatAccion').html('<div class=\"menuChatInt\">...</div><div class="chatCargando"><span>' + textChat.cargando + '</span></div>');
    MostrarChatsActivos(false);
}

function MontarChat() {
    try {
        $('#divChat').html('<div id="divChatsActivos"></div><div id="divChatAccion"></div>');
        MostrarCargandoChat();

        wanachat = $.connection.myChatHub;

        //$('#txtHackUsu').val('');

        wanachat.client.addMessage = function (message, remitente, idPerfilRemitenteID, chatID, mensID) {
            if (idPerfilRemitenteID == null || idPerfilRemitenteID == $('#txtHackUsu').val()) {
                if (idPerfilRemitenteID != null) {
                    PintarMensaje(remitente, message, 1, mensID);
                    MarcarNumMenChat(chatID, 0);
                    wanachat.server.chatLeido($('#txtHackPerfilActual').val(), chatID);
                }
                else {
                    PintarMensaje(remitente, message, 2, mensID);
                }
            }
            else {
                MarcarNumMenChat(chatID, 1);
                MoverChatPrimero(chatID);

                //$('#txtHackMensajes').val($('#txtHackMensajes').val() + idPerfilRemitenteID + '|' + remitente + '|' + message + '|||');
                var alerta = message;

                if (alerta.length > 30) {
                    alerta = alerta.substring(0, 27) + '...';
                }

                MontarAlerta(remitente + ': ' + alerta, idPerfilRemitenteID, remitente, chatID, 0);
            }
        };

        wanachat.client.addHtml = function (divID, html, js) {
            if (html.indexOf('[|||]') != -1) {
                var varIdiomas = html.substring(0, html.indexOf('[|||]'));
                html = html.substring(html.indexOf('[|||]') + 5);
                var variables = varIdiomas.split('|||');

                for (var i = 0; i < variables.length; i++) {
                    html = html.replace('@' + variables[i] + '@', eval(variables[i]));
                }
            }

            $('#' + divID).html(html);

            if (js != '') {
                eval(js);
            }
        };

        wanachat.client.addMasMensajes = function (htmlNuevo) {
            if (htmlNuevo.indexOf('[|||]') != -1) {
                var varIdiomas = htmlNuevo.substring(0, htmlNuevo.indexOf('[|||]'));
                htmlNuevo = htmlNuevo.substring(htmlNuevo.indexOf('[|||]') + 5);
                var variables = varIdiomas.split('|||');

                for (var i = 0; i < variables.length; i++) {
                    htmlNuevo = htmlNuevo.replace('@' + variables[i] + '@', eval(variables[i]));
                }
            }

            var html = $('ul#listMessagesChat').html();
            html = html.substring(html.indexOf('</li>') + 5);
            html = htmlNuevo + html;
            $('ul#listMessagesChat').html(html);
        };

        wanachat.client.addChat = function (htmlNuevo) {
            $('#divChatsActivos ul').html(htmlNuevo + $('#divChatsActivos ul').html());
        };

        wanachat.client.addError = function (error) {
            MontarAlerta(error, null, null, null, 1);
        };

        $.connection.hub.start().done(function () { registrarUsuChat('0') }).fail(function () { MontarAlerta('registrar', null, null, null, 1); });
    }
    catch (err) { }
}

function PintarMensaje(remitente, message, tipo, mensID) {
    var mensaje = '<span class="mensRemi">' + remitente + ':</span><span class="mensChat">' + message + '</span><span class="mensFecha">' + ObtenerFecha() + '</span>';
    if (tipo == 0) {
        $('#listMessagesChat').append('<li id="liMen_' + mensID + '" class="ownerMessage mensAccionEnviando">' + mensaje + '<span class="mensAccionEnviando">' + textChat.enviando + '...</span></li>');
    }
    else if (tipo == 1) {
        $('#listMessagesChat').append('<li id="liMen_' + mensID + '" class="contactMessage">' + mensaje + '</li>');
    }
    else if (tipo == 2) {
        $('#liMen_' + mensID).html(mensaje + '<span class="mensAccionEnviado">' + textChat.enviado + '</span>');
        $('#liMen_' + mensID).attr('class', 'ownerMessage mensAccionEnviado');
    }

    DesplazarScrollMens();
}

function ObtenerFecha() {
    var f = new Date();
    return (f.getHours() + ":" + (f.getMinutes()));
}

function DesplazarScrollMens() {
    $('.divListMessages')[0].scrollTop = $('.divListMessages')[0].scrollHeight;
    $('#listMessagesChat')[0].scrollTop = $('#listMessagesChat')[0].scrollHeight;
}

function MarcarNumMenChat(chatID, num) {
    if ($('#divChatsActivos li#liChat_' + chatID + ' span.chatNoRead').length > 0) {
        var numText = '';

        if (num > 0) {
            var antNunText = $('#divChatsActivos li#liChat_' + chatID + ' span.chatNoRead').html()
            var antNun = 0;

            if (antNunText != '') {
                antNun = parseInt(antNunText.trim());
                num += antNun;
            }

            numText = num;
        }

        $('#divChatsActivos li#liChat_' + chatID + ' span.chatNoRead').html(numText);
    }
}

function MoverChatPrimero(chatID) {
    var chats = $('#divChatsActivos li#liChat_' + chatID);

    if (chats.length > 0) {
        var ul = chats.parent();
        var li = chats[0];
        chats.remove();
        ul.prepend(li);
    }
    else {
        //TODO: HACER -> Traer chat de server y pintar por ejemplo
        ComprobarConexionChat();
        wanachat.server.getChat($('#txtHackPerfilActual').val(), chatID);
    }
}

function MostrarChatsActivos(mostrar) {
    if (mostrar) {
        $('#txtHackUsu').val('')
        $('#divChatAccion').css('display', 'none');
        $('#divChatsActivos').css('display', '');
    }
    else {
        $('#divChatAccion').css('display', '');
        $('#divChatsActivos').css('display', 'none');
    }
}

function AgregarContactoChat() {
    MostrarCargandoChat();
    ComprobarConexionChat();
    wanachat.server.addContact($('#txtHackPerfilActual').val());
}

function registrarUsuChat(pInicio) {
    if ($.connection.hub.id != null && $.connection.myChatHub.connection.state === 1/*signalR.connectionState.connected*/) {
        //var wanachat = $.connection.myChatHub;
        wanachat.server.registrar($('#txtHackPerfilActual').val(), pInicio);

        if (pInicio == '0') {
            setTimeout('ComprobarConexionChat()', 300000);
        }
    }
    else {
        setTimeout('registrarUsuChat(\'' + pInicio + '\')', 100);
    }
}

function desactivarUsuChat() {
    ContraerChat();
    $('#hlMaxChat').html(textChat.conectar);
    $('#hlMaxChat').attr('onclick', 'ExpandirChat(false);');
    $('#hlCerrChat').css('display', 'none');

    ComprobarConexionChat();

    if ($.connection.hub.id != null && $.connection.myChatHub.connection.state === 1) {
        wanachat.server.desactivarChat($('#txtHackPerfilActual').val());

        $.connection.hub.disconnect();

        $('#divchatframe').attr('class', 'divChat disabled');
    }
}

function IniciarChat(idPerfilID, nombrePerfil, chatID) {
    MostrarCargandoChat();
    ComprobarConexionChat();
    wanachat.server.addChat($('#txtHackPerfilActual').val(), $('#txtHackNombreActual').val(), idPerfilID, nombrePerfil, chatID);
    MarcarNumMenChat(chatID, 0);
}

function EnviarMensChat(id) {
    var idMen = guidGenerator();
    PintarMensaje($('#txtHackNombreActual').val(), $('#txtMessage').val(), 0, idMen);
    ComprobarConexionChat();
    wanachat.server.send($('#txtMessage').val(), $('#txtHackPerfilActual').val(), $('#txtHackUsu').val(), $('#txtHackNombreActual').val(), id, idMen);
    $('#txtMessage').val('');
}

function ComprobarConexionChat() {
    if (!($.connection.hub.id != null && $.connection.myChatHub.connection.state === 1)) {
        $.connection.hub.start().done(function () { registrarUsuChat('1') }).fail(function () { MontarAlerta('registrar', null, null, null, 1); });
    }
}

function CargarMensAntChat(id, idUsu, numLlamada) {
    ComprobarConexionChat();
    wanachat.server.cargarMensAnt(id, idUsu, numLlamada);
}


function MontarAlerta(alerta, idPerfilID, nombrePerfil, chatID, tipo) {
    if (tipo == 0) {
        $('#divAlertaChat').html('<p><a onclick="ExpandirChat(true);OcultarAlerta();IniciarChat(\'' + idPerfilID + '\', \'' + nombrePerfil + '\', \'' + chatID + '\');">Ver</a> ' + alerta + '</p>');
    }
    else {
        var error = '';

        if (alerta == 'send') {
            error = textChat.errorSend;
        }
        else if (alerta == 'registrar') {
            error = textChat.errorReg;
        }
        else if (alerta == 'desactivarChat') {
            error = textChat.errorDesac;
        }
        else if (alerta == 'addContact') {
            error = textChat.errorContac;
        }
        else if (alerta == 'addChat') {
            error = textChat.errorChat;
        }
        else if (alerta == 'cargarMensAnt') {
            error = textChat.errorCargarMenAnt;
        }
        else if (alerta == 'send') {
            error = textChat.errorSend;
        }
        else if (alerta == 'send') {
            error = textChat.errorSend;
        }
        else if (alerta == 'send') {
            error = textChat.errorSend;
        }

        $('#divAlertaChat').html('<div class=\"ko\" style="display:block;"><p>' + error + '</p></div>');
    }
    var margin = 5 + $('#divAlertaChat').height();
    $('#divAlertaChat').css('margin-top', '-' + margin + 'px');
    $('#divAlertaChat').css('display', '');
    setTimeout('OcultarAlerta()', 5000);
}

function OcultarAlerta() {
    $('#divAlertaChat').css('display', 'none');
}


/****** FIN CHAT **********/

var currentMousePos = { x: -1, y: -1 };
//Devuelve la posisición del cursor 
$(document).mousemove(function (event) {
    currentMousePos.x = event.pageX;
    currentMousePos.y = event.pageY;
});

function Distancia(lat1, lon1, lat2, lon2) {
    rad = function (x) { return x * Math.PI / 180; }

    //Radio de la tierra en km
    var R = 6378.137;
    var dLat = rad(lat2 - lat1);
    var dLong = rad(lon2 - lon1);

    var a = Math.sin(dLat / 2) * Math.sin(dLat / 2) + Math.cos(rad(lat1)) * Math.cos(rad(lat2)) * Math.sin(dLong / 2) * Math.sin(dLong / 2);
    var c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1 - a));
    var d = R * c;

    //Retorna tres decimales
    return d.toFixed(3);
}

function ObtenerUrlParaCallBack() {
    var url = document.location.href;
    url = url.substr(0, url.indexOf('#'));
    return url;
}


$(document).ready(function () {
    $('a.linkDescargaFichero').click(function () {
        EnviarAccGogAnac('Descargar Fichero', 'Descargar Fichero', window.location.href);
    });
});


function chequearUsuarioLogueado(funcionRespuesta) {
    MostrarUpdateProgress();

    var request = new XMLHttpRequest();
    request.onreadystatechange = function () {
        if (request.readyState == 4) {
            eval(funcionRespuesta);
        }
    }
    var url = window.location.protocol + '//' + window.location.host + '/solicitarcookie.aspx'
    request.open('GET', url, true);
    request.withCredentials = true;
    request.send();
}

function Redirigir(response) {
    if (response != undefined) {
        window.location.href = response;
    }
}

var PeticionesCookie = {
    CargarCookie() {
        var urlPeticion = null;
        urlPeticion = $('#inpt_UrlLoginCookie').val() + "/RefrescarCookie";
        GnossPeticionAjax(
            urlPeticion,
            null,
            true
        ).done(function (response) {
        }).fail(function (response) {
        });
    }

}

var PeticionesAJAX = {
    CargarNumElementosNuevos: function (pGuidPerfilUsu, pGuidPerfilOrg, pEsBandejaOrg, identCargarNov, RepintarContadoresNuevosElementos, RecogerErroresAJAX) {
        var urlPeticion = null;
        if ($('#inpt_proyID').val() == '11111111-1111-1111-1111-111111111111') {
            urlPeticion = location.protocol + "//" + location.host + "/PeticionesAJAX/CargarNumElementosNuevos";
        } else {
            urlPeticion = $('#inpt_baseUrlBusqueda').val() + "/PeticionesAJAX/CargarNumElementosNuevos";
        }
        var datosPost =
        {
            pPerfilUsuarioID: pGuidPerfilUsu,
            pPerfilOrganizacionID: pGuidPerfilOrg,
            pBandejaDeOrganizacion: pEsBandejaOrg,
            pOtrasIdent: identCargarNov
        };
        GnossPeticionAjax(
            urlPeticion,
            datosPost,
            true
        ).done(function (response) {
            if (response.split('|').length == 1) {
                alert('Te has conectado en otro navegador.');
                window.location.href = response;
            } else {
                RepintarContadoresNuevosElementos(response)
            }
        }).fail(function (response) {
            RecogerErroresAJAX(response);
        });
    },
    ObtenerDatosUsuarioActual: function (FuncionEjecutar) {
        var urlPeticion = location.protocol + "//" + location.host + "/PeticionesAJAX/ObtenerDatosUsuarioActual";
        GnossPeticionAjax(
            urlPeticion,
            null,
            true
        ).done(function (response) {
            FuncionEjecutar(response)
        });
    }

}