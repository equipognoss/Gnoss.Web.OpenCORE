function ComprobarDatosRegistroLogeado() {
    var errorCondiciones = ComprobarClausulas();

    if (errorCondiciones.length > 0) {
        crearError(errorCondiciones, '#divKoCondicionesUso', false);
        return true;
    }
    else {
        $('#divKoCondicionesUso').html('');
        return false;
    }
}

function ComprobarDatosRegistro(pEdadMinimaRegistro) {
    var error = "";

    if ((typeof RecogerDatosExtra != 'undefined')) {
        error += RecogerDatosExtra();
    }

    error += AgregarErrorReg(error, ValidarNombrePersona('txtNombre', 'lblNombre'));
    error += AgregarErrorReg(error, ValidarApellidos('txtApellidos', 'lblApellidos'));
    error += ValidarCampoLogin('txtNombreUsuario');
    error += ValidacionesEmail('txtEmail', 'txtEmailTutor');
    /*
    error += AgregarErrorReg(error, ValidarEmailIntroducido('txtEmail', 'lblEmail'));*/
    error += AgregarErrorReg(error, ValidarContrasena('txtContrasenya', '', 'lblContrasenya', '', false));
    error += AgregarErrorReg(error, ValidarCampoNoVacio('txtCargo', 'txtCargo'));

    if (document.getElementById('txtFechaNac') != null) {
        error += AgregarErrorReg(error, ValidarFechaNacimientoMVC($('#txtFechaNac').val(), 'lblFechaNac', '' /*pEdadMinimaRegistro*/));
    }
    if (document.getElementById('#ddlPais') != null) {
        error += AgregarErrorReg(error, ValidarPais('ddlPais', 'lblPais'));
    }
    if (document.getElementById('#txtProvincia') != null) {
        error += AgregarErrorReg(error, ValidarProvincia('txtProvincia', 'lblProvincia', 'ddlProvincia'));
    }
    if (document.getElementById('#txtLocalidad') != null) {
        error += AgregarErrorReg(error, ValidarPoblacionOrg('txtLocalidad', 'lblLocalidad'));
    }
    if (document.getElementById('#ddlSexo') != null) {
        error += AgregarErrorReg(error, ValidarSexo('ddlSexo', 'lblSexo'));
    }

    //Compruebo cláusulas adicionales:
    var errorCondiciones = ComprobarClausulas();

    if (error.length > 0 || errorCondiciones.length > 0) {
        if (error.length > 0) {
            //crearError(error, '#divKodatosUsuario', false);
            $('#divKodatosUsuario').children().addClass('d-block');
            $('#divKodatosUsuario').children().html(error);
        }
        else {
            //$('#divKodatosUsuario').html('');
            $('#divKodatosUsuario').children().removeClass('d-block');
            $('#divKodatosUsuario').children().html('');
        }

        if (errorCondiciones.length > 0) {
            //crearError(errorCondiciones, '#divKoCondicionesUso', false);
            $('#divKoCondicionesUso').children().addClass('d-block');
            $('#divKoCondicionesUso').children().html(errorCondiciones);
        }
        else {
            $('#divKoCondicionesUso').children().removeClass('d-block');
            $('#divKoCondicionesUso').html('')
        }

        return true;
    } else {
        $('#divKodatosUsuario').html('');
        $('#divKoCondicionesUso').html('')

        return false;
    }
}

function ValidarCampoLogin(pTxtLogin) {
    var error = '';
    var textLogin = $('#' + pTxtLogin).val();
    if (textLogin != undefined && textLogin != '') {
        if (textLogin.length > 12) {
            error += '<p> El nombre del usuario no puede exceder de 12 caracteres </p>';
        }
    }
    return error;

}

function ValidacionesEmail(pTxtEmail, pTxtEmailTutor) {
    var error = "";
    var textEmail = $('#' + pTxtEmail).val();
    var textEmailTutor = $('#' + pTxtEmailTutor).val();
    if (textEmailTutor != undefined) {
        if (textEmail == '' && textEmailTutor == '') {
            error += '<p> Debes rellenar uno de los dos campos de Email. </p>';
        }
        else {
            if (ValidacionEmailTutor(pTxtEmailTutor, 'lblEmailTutor')) {
                error += AgregarErrorReg(error, ValidarEmailIntroducido(pTxtEmailTutor, 'lblEmailTutor'));
            }

        }
    }
    else {
        error += AgregarErrorReg(error, ValidarEmailIntroducido(pTxtEmail, 'lblEmail'));
    }

    return error;
}

function ValidacionEmail(pTxtMail, pLblMail) {
    var textEmail = $('#' + pTxtMail).val();
    if (textEmail == '') {
        return false;
    }
    return true;
}

function ValidacionEmailTutor(pTxtMail, pLblMail) {
    var textEmail = $('#' + pTxtMail).val();
    if (textEmail != undefined && textEmail == '') {
        return false;
    }
    return true;
}

function ComprobarClausulas() {
    error = "";
    var listaChecks = '';
    $('#condicionesUso input[type=checkbox]').each(function () {
        var that = $(this);
        if (that.is(':checked')) {
            listaChecks += that.attr('id') + ',';
        }
        that.next().css("color", "");
        if (!that.hasClass("optional")) {
            if (!that.is(':checked')) {
                that.next().css("color", "#E24973");
                if (that.parent().attr('id') == "liClausulaMayorEdad") {
                    error = '<p>' + form.mayorEdad + '</p>';
                } else {
                    error = '<p>' + form.aceptarLegal + '</p>';
                }
            }
        }
    });
    $('#clausulasSelecc').val(listaChecks);
    return error;
}

function MostrarPanelExtra() {
    $('#despleReg').show();
    $('#despleReg .stateShowForm').show();
    $('html,body').animate({ scrollTop: $('#despleReg').offset().top }, 'slow');
    return false;
}


function ComprobarEmailUsuario(pUrlPagina) {
    if (ValidarEmailIntroducido('txtEmail', 'lblEmail') == '') {
        var dataPost = {
            callback: "comprobarEmail",
            email: $('#txtEmail').val()
        }
        $.post(pUrlPagina, dataPost, function (data) {
            ProcesarEmailComprobado(data);
        });
    }
}

/**
 * Validar que el campo del email sea válido. 
 * Si es correcto, se añade una clase "is-valid"
 * Si es incorrecto (vacío) se añade una clase "is-invalid"
 * Añade además un div con un mensaje personalizado o lo oculta si todo es correcto.
 * @param {any} pDatosRecibidos
 */
function ProcesarEmailComprobado(pDatosRecibidos) {
    if ((typeof ProcesarEmailComprobadoPersonalizado != 'undefined')) {
        ProcesarEmailComprobadoPersonalizado(pDatosRecibidos);
    }
    else
    {
        if (pDatosRecibidos == 'KO') {
            if ($('#divKoEmail').length == 0) {
                $('#lblEmail').parent().after('<p id="divKoEmail"></p>');
            }
            var mensaje = '<p>' + form.emailrepetido + '</p>';

            if (typeof (MensajePersonalizado) != 'undefined' && MensajePersonalizado.length > 0) {
                mensaje = mensaje + '<p>' + MensajePersonalizado + '</p>'
            }

            crearError(mensaje, '#divKoEmail');
            //$('#lblEmail').attr('class', 'ko');
            $('#txtEmail').addClass('is-invalid');
            $('#txtEmail').removeClass('is-valid');
            
        } else {
            $('#divKoEmail').html('');
            //$('#lblEmail').attr('class', '');
            $('#txtEmail').addClass('is-valid');
            $('#txtEmail').removeClass('is-invalid');
        }
    }
}

function comprobarCheck(pCheck, pTxtHackID) {
    var num = parseInt($('#' + pTxtHackID).val().substring(0, $('#' + pTxtHackID).val().indexOf('||')));
    if (pCheck.checked) { num--; } else { num++; }
    $('#' + pTxtHackID).val(num + $('#' + pTxtHackID).val().substring($('#' + pTxtHackID).val().indexOf('||')))
}

function ComprobarCampoRegistroMVC(pCampo) {
    if (pCampo == 'NombreUsu') {
        ValidarNombreUsu('txtNombreUsuario', 'lblNombreUsuario');
    }
    else if (pCampo == 'Contra1') {
        ValidarContrasena('txtContrasenya', '', 'lblContrasenya', '', false);
    }
    else if (pCampo == 'Contra2') {
        ValidarContrasena('txtContrasenya', 'txtcContrasenya', 'lblContrasenya', 'lblConfirmarContrasenya', true);
    }
    else if (pCampo == 'Mail') {
        ValidarEmailIntroducido('txtEmail', 'lblEmail');
    }
    else if (pCampo == 'Nombre') {
        ValidarNombrePersona('txtNombre', 'lblNombre');
    }
    else if (pCampo == 'Apellidos') {
        ValidarApellidos('txtApellidos', 'lblApellidos');
    }
    else if (pCampo == 'Provincia') {
        ValidarProvincia('txtProvincia', 'lblProvincia', 'editProvincia');
    }
    else if (pCampo == 'Sexo') {
        ValidarSexo('editSexo', 'lblSexo');
    }
    else if (pCampo == 'CentroEstudios') {
        ValidarCentroEstudios('txtCentroEstudios', 'lbCentroEstudios');
    }
    else if (pCampo == 'AreaEstudios') {
        ValidarAreaEstudios('txtAreaEstudios', 'lbAreaEstudios');
    }
}

function validaFechaDDMMAAAA(fecha) {
    var dtCh = "/";
    var minYear = 1900;
    var maxYear = 2100;
    function isInteger(s) {
        var i;
        for (i = 0; i < s.length; i++) {
            var c = s.charAt(i);
            if (((c < "0") || (c > "9"))) return false;
        }
        return true;
    }
    function stripCharsInBag(s, bag) {
        var i;
        var returnString = "";
        for (i = 0; i < s.length; i++) {
            var c = s.charAt(i);
            if (bag.indexOf(c) == -1) returnString += c;
        }
        return returnString;
    }
    function daysInFebruary(year) {
        return (((year % 4 == 0) && ((!(year % 100 == 0)) || (year % 400 == 0))) ? 29 : 28);
    }
    function DaysArray(n) {
        var daysInMonth = [];
        for (var i = 1; i <= n; i++) {
            daysInMonth[i] = 31
            if (i == 4 || i == 6 || i == 9 || i == 11) { daysInMonth[i] = 30 }
            if (i == 2) { daysInMonth[i] = 29 }
        }
        return daysInMonth;
    }
    function isDate(dtStr) {
        var daysInMonth = DaysArray(12)
        var pos1 = dtStr.indexOf(dtCh)
        var pos2 = dtStr.indexOf(dtCh, pos1 + 1)
        var strDay = dtStr.substring(0, pos1)
        var strMonth = dtStr.substring(pos1 + 1, pos2)
        var strYear = dtStr.substring(pos2 + 1)
        strYr = strYear
        if (strDay.charAt(0) == "0" && strDay.length > 1) strDay = strDay.substring(1)
        if (strMonth.charAt(0) == "0" && strMonth.length > 1) strMonth = strMonth.substring(1)
        for (var i = 1; i <= 3; i++) {
            if (strYr.charAt(0) == "0" && strYr.length > 1) strYr = strYr.substring(1)
        }
        month = parseInt(strMonth)
        day = parseInt(strDay)
        year = parseInt(strYr)
        if (pos1 == -1 || pos2 == -1) {
            return false
        }
        if (strMonth.length < 1 || month < 1 || month > 12) {
            return false
        }
        if (strDay.length < 1 || day < 1 || day > 31 || (month == 2 && day > daysInFebruary(year)) || day > daysInMonth[month]) {
            return false
        }
        if (strYear.length != 4 || year == 0 || year < minYear || year > maxYear) {
            return false
        }
        if (dtStr.indexOf(dtCh, pos2 + 1) != -1 || isInteger(stripCharsInBag(dtStr, dtCh)) == false) {
            return false
        }
        return true
    }
    if (isDate(fecha)) {
        return true;
    } else {
        return false;
    }
}

function ValidarFechaNacimientoMVC(pFechaNacimiento, pLblFechaNacimiento, pEdadMinima) {
    var error = '';
    $('#' + pLblFechaNacimiento).removeClass("ko");
    mayor = false;

    if (pFechaNacimiento != null && validaFechaDDMMAAAA(pFechaNacimiento)) {

        fecha = new Date(pFechaNacimiento.split('/')[2], pFechaNacimiento.split('/')[1], pFechaNacimiento.split('/')[0]);
        hoy = new Date();

        if ((hoy.getFullYear() - fecha.getFullYear()) > pEdadMinima) {
            //Los ha cumplido en algún año anterior
            mayor = true;
        }
        else if ((hoy.getFullYear() - fecha.getFullYear()) == pEdadMinima) {
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

        if (!mayor) {
            error += '<p>' + form.mayorEdad.replace('18', pEdadMinima) + '</p>';
            $('#' + pLblFechaNacimiento).attr('class', 'ko');
        }
    }
    else {
        error += '<p>' + form.fechanacincorrecta + '</p>';
        $('#' + pLblFechaNacimiento).attr('class', 'ko');
    }

    return error;
}


function CargarFormLoginRegistro(urlPagina) {
    MostrarUpdateProgress();;
    var dataPost = {
        callback: "cargarFormLogin"
    };
    $.post(urlPagina, dataPost, function (data) {
        $('.box.box01').html(data);
        OcultarUpdateProgress();
    });
}


function ValidarCampoObligatorio(pTxtCampo, pLblCampo) {
    var error = '';
    if (pTxtCampo.val() == '') {
        error += '<p>' + form.camposVacios + '</p>';
        pLblCampo.attr('class', 'ko');
    }
    else {
        pLblCampo.attr('class', '');
    }
    return error;
}

function ValidarCampoSelectObligatorio(pTxtCampo, pLblCampo) {
    var error = '';
    if (pTxtCampo.val() == '00000000-0000-0000-0000-000000000000') {
        error += '<p>' + form.camposVacios + '</p>';
        pLblCampo.attr('class', 'ko');
    }
    else {
        pLblCampo.attr('class', '');
    }
    return error;
}

function ValidarCampoRadioObligatorio(pRadioButons, pLblCampo) {
    var error = '<p>' + form.camposVacios + '</p>';
    pLblCampo.attr('class', 'ko');
    pRadioButons.each(function () {
        if($(this).is(':checked')){
            pLblCampo.attr('class', '');
            error = '';
        }
    });
    return error;
}

function PrepararAutocompletar(inputID, argumentos, proyectoID) {
    $('#' + inputID).unautocomplete().unbind("focus")
    .autocomplete(
        null,
        {
            url: $('input.inpt_urlServicioAutocompletar').val() + "/AutoCompletarDatoExtraProyectoVirtuoso",
            delay: 0,
            scroll: false,
            selectFirst: false,
            minChars: 1,
            cacheLength: 0,
            extraParams: {
                pProyectoID: proyectoID,
                pOrigen: inputID,
                pArgumentos: function (e) { return ObtenerValoresArgumentos(argumentos); }
            }
        }
    );
}

function ObtenerValoresArgumentos(variables) {
    var valores = '';
    if (typeof (variables) != 'undefined' && variables != '') {
        var filtros = variables.split('|')
        if (filtros.length > 0 && filtros[0] != '') {
            for (var i = 0; i < filtros.length; i++) {
                valores += $('#' + filtros[i]).val() + '|';
            }
        }
    }
    return valores;
}