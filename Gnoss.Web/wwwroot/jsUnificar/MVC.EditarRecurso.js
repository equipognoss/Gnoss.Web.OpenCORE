var TxtTitulosSem = '';
var redirectAtEnd = true;

//REGION Subir Recurso

var indiceCombo = 0;

function validarDocAdjuntar(omitirCompRep, extraArchivo) {
    return validarDocAdjuntarExt(urlPaginaSubir, omitirCompRep, extraArchivo)
}

function validarDocFisico(omitirCompRep) {
    return validarDocFisicoExt(urlPaginaSubir, omitirCompRep);
}

function validarUrl(omitirCompRep) {
    return validarUrlExt(urlPaginaActual, omitirCompRep);
}

function getAbsolutePosition(element) {
    var r = { x: element.offsetLeft, y: element.offsetTop };
    if (element.offsetParent) {
        var tmp = getAbsolutePosition(element.offsetParent);
        r.x += tmp.x;
        r.y += tmp.y;
    }
    return r;
}

function InicializarSubirRecurso() {
    InicializarSubirRecursoExt(urlPaginaActual);
}

//FIN REGION Subir Recurso


//REGION Modificar Recurso

function validarCamposGuardado(pBorrador) {

    PintarTags($('#txtTags'));

    if ($('#txtAutores').length > 0) {
        PintarTags($('#txtAutores'));
    }

    if ($('#txtTitulo').val().trim() == '') {
        $('#divContLblErrorDocumento').addClass("errorTitulo");
        CrearErrorModfRec(form.errordtitulo);
        return false;
    }

    if (document.getElementById('txtDescripcion') != null && $('#txtDescripcion').val().trim() == '' && !($('#rbNewsletterDesdeArchivo').is(':checked'))) {
        $('#divContLblErrorDocumento').addClass("errorDescription");
        CrearErrorModfRec(form.errorddescripcion);
        return false;
    }

    if (document.getElementById('txtURLDoc') != null) {
        var regexURL = /^((([hH][tT][tT][pP][sS]?|[fF][tT][pP])\:\/\/)?([\w\.\-]+(\:[\w\.\&%\$\-]+)*@)?((([^\s\(\)\<\>\\\"\.\[\]\,@;:]+)(\.[^\s\(\)\<\>\\\"\.\[\]\,@;:]+)*(\.[a-zA-Z]{2,4}))|((([01]?\d{1,2}|2[0-4]\d|25[0-5])\.){3}([01]?\d{1,2}|2[0-4]\d|25[0-5])))(\b\:(6553[0-5]|655[0-2]\d|65[0-4]\d{2}|6[0-4]\d{3}|[1-5]\d{4}|[1-9]\d{0,3}|0)\b)?((\/[^\/][\w\.\,\?\'\\\/\+&%\$#\=~_\-@:]*)*[^\.\,\?\"\'\(\)\[\]!;<>{}\s\x7F-\xFF])?)$/i;
        var esSharepoint = ($('#txtURLDoc').val().includes("riamlab.sharepoint.com") || $('#txtURLDoc').val().includes("riamlab-my.sharepoint.com"));
        if (!$('#txtURLDoc').val().match(regexURL) && !esSharepoint) {
            CrearErrorModfRec(form.errordurl);
            return false;
        }
    }


    if (!pBorrador && $('#txtTags_Hack').val().trim() == '') {
        CrearErrorModfRec(form.errordtag);
        return false;
    }
    if (!pBorrador && $('#panDesplegableSelCat input.hackCatTesSel').val().trim() == '') {
        switch ($('#txtTipoDocHack').val()) {
            case "15":
                CrearErrorModfRec(form.errordcategoriapregunta);
                break
            case "16":
                CrearErrorModfRec(form.errordcategoriadebate);
                break
            case "18":
                CrearErrorModfRec(form.errordcategoriaencuesta);
                break
            default:
                CrearErrorModfRec(form.errordcategoria);
        }
        return false;
    }

    return true;
}

function CrearErrorModfRec(error) {
    $('#divContLblErrorDocumento').html('<div id="divKoErrorDoc" class="ko" style="display:block;"><p><span ID="lblErrorDocumento">' + error + '</span></p></div>');
}

function crearVersion(reemplazar) {
    if (reemplazar == 1) {
        document.getElementById('txtHackReemplazar').value = 'reemplazar';
    }

    EnviarDatosServidor('Publicar');
}




function eliminarFichero() {
    document.getElementById('hlDescargarRecurso').innerHTML = '';
    document.getElementById('hlDescargarRecurso').href = '';
    document.getElementById('txtEsNota').value = "Nota";
    document.getElementById('fuExaminar').value = "";
    document.getElementById('fuExaminar').style = "";
    $('#panEliminarAdjunto').hide();
}


function comprobarReemplazarArchivo() {
    var redireccionar = true;
    if (typeof multiplesEditores != "undefined" && multiplesEditores) {
        var fichero = document.getElementById('fuExaminar');
        document.getElementById('txtHackReemplazar').value = '';

        if (fichero != null) {
            if ((fichero.value != null) && (fichero.value != '')) {
                redireccionar = false;
                $('#divContLblErrorDocumento').html('<div id="panEnlRep"><div id="panels" class="stateShowForm" style="display:block"><div id="action" class="box form activeForm" style="display:block"><fieldset><p>' + form.generarversionauto + '</fieldset><fieldset><p><input type="button" value="' + borr.si + '" onclick="crearVersion(1)" /> <input type="button" value="' + borr.no + '" onclick="crearVersion(0)" /></p></fieldset></div></div></div>');
            }
        }
    }

    if (redireccionar) {
        EnviarDatosServidor('Publicar');
    }
}

function EnviarDatosServidor(pFuncion) {
    MostrarUpdateProgressTime(0);
    GuardarDisabled(false);

    var arg = {};

    arg.Key = documentoID;
    arg.documentoID = documentoID;
    arg.Draft = (pFuncion != 'Publicar' && pFuncion != 'GuardarRecursoRepetido' && pFuncion != 'Concurrencia|CrearVersion' && pFuncion != 'Concurrencia|Sobreescribir' && pFuncion != 'AutoGuardar');
    arg.SkipRepeat = (pFuncion == 'GuardarRecursoRepetido_Borrador' || pFuncion == 'GuardarRecursoRepetido' || pFuncion == 'Concurrencia|CrearVersion' || pFuncion == 'Concurrencia|Sobreescribir' || pFuncion == 'AutoGuardar');
    arg.RedirectionByConcurrency = (pFuncion == 'Concurrencia|Sobreescribir');
    arg.CreateVersionByConcurrency = (pFuncion == 'Concurrencia|CrearVersion');
    arg.AutoSave = (pFuncion == 'AutoGuardar');

    if (document.getElementById('txtHackArchivo') != null) {
        arg.TemporalFileName = document.getElementById('txtHackArchivo').value;
    }

    if (document.getElementById('txtTitulo') != null) {
        arg.Title = document.getElementById('txtTitulo').value;
    }

    var valorCKEditor = $('#txtDescripcion').val();
    if (valorCKEditor != null) {
        arg.Description = encodeURIComponent(valorCKEditor);
    }

    if (document.getElementById('txtTags_Hack') != null) {
        arg.Tags = document.getElementById('txtTags_Hack').value;
    }

    if (document.getElementById('txtVinculado_Hacks') != null) {
        arg.Vinculado = document.getElementById('txtVinculado_Hacks').value;
    }

    if ($("#modificacionVinculado")[0] != null && $("#modificacionVinculado")[0] != undefined) {
        var vinculados = $(".vinculado input[name='txtTitulo']");

        var numVinculado = vinculados.length;
        if (numVinculado > 0) {
            arg.Vinculado = "";

            vinculados.each(function () {
                if ($(this).val() != "") {
                    arg.Vinculado = arg.Vinculado + $(this).val() + ',';
                }
            });
        }
    } else {
        arg.Vinculado = "SinModificacion"
    }

    if (document.getElementById('txtHackCatBRAddToGnoss') != null) {
        arg.SelectedCategories = $('#txtHackCatBRAddToGnoss').val();;
    }
    else {
        var categorias = $('#panDesplegableSelCat input.hackCatTesSel').val();
        if (categorias != null) {
            arg.SelectedCategories = categorias;
        }
    }

    if (document.getElementById('fuExaminar') != null) {
        if (document.getElementById('fuExaminar').value != "") {
            arg.TipoFicheroEdit = '3';
        } else if (document.getElementById('txtEsNota') != null && document.getElementById('txtEsNota').value != "") {
            arg.TipoFicheroEdit = '8';
        }
    }


    if (document.getElementById('recursoEntero') != null) {
        if (document.getElementById('txtHackArchivo') != null && !document.getElementById('txtHackArchivo').value == "") {
            if (document.getElementById('fuExaminar') != null && document.getElementById('fuExaminar').value != "") {
                arg.Tipo = '3';
            }
        }
    }

    //editores
    if (document.getElementById('rbEditoresYo') != null) {
        arg.SpecificResourceEditors = $('#rbEditoresOtros').is(':checked');
    }

    var editores = $('#divContDespEdit input.hackUsuSeleccionadoRec').val();
    if (editores != null) {
        arg.ResourceEditors = editores;
    }

    arg.ResourceVisibility = 0;

    if (document.getElementById('rbAbierto') != null && $('#rbAbierto').is(':checked')) {
        arg.ResourceVisibility = 0;
    }

    if (document.getElementById('rbLectoresComunidad') != null && $('#rbLectoresComunidad').is(':checked')) {
        arg.ResourceVisibility = 1;
    }

    if (document.getElementById('rbLectoresEditores') != null && $('#rbLectoresEditores').is(':checked')) {
        arg.ResourceVisibility = 2;
    }

    if (document.getElementById('rbLectoresEspecificos') != null && $('#rbLectoresEspecificos').is(':checked')) {
        arg.ResourceVisibility = 3;
    }

    var lectores = $('#divContDespLect input.hackUsuSeleccionadoRec').val();
    if (lectores != null) {
        arg.ResourceReaders = lectores;
    }

    if (document.getElementById('chkCompartir') != null) {
        arg.ShareAllowed = $('#chkCompartir').is(':checked');
    }

    if (document.getElementById('rbNoAutorPropio') != null) {
        arg.CreatorIsAuthor = $('#rbAutorPropio').is(':checked');
    }

    if (document.getElementById('txtAutores_Hack') != null) {
        arg.Authors = document.getElementById('txtAutores_Hack').value;
    }

    if (document.getElementById('txtHackValorSelecLicencia') != null) {
        arg.License = document.getElementById('txtHackValorSelecLicencia').value;
    }

    if (document.getElementById('txtHackReemplazar') != null && document.getElementById('txtHackReemplazar').value != '') {
        arg.CreateVersionByReplaceAttachment = true;
    }

    if (document.getElementById('txtURLDoc') != null) {
        arg.Link = document.getElementById('txtURLDoc').value;
    }

    if (document.getElementById('txtUbicacionDoc') != null) {
        arg.Link = document.getElementById('txtUbicacionDoc').value;
    }

    if (document.getElementById('txtHackEnlaceDoc') != null) {
        arg.Link = document.getElementById('txtHackEnlaceDoc').value;
    }

    if (document.getElementById('txtTagsEnlaces') != null) {
        arg.TagsLinks = document.getElementById('txtTagsEnlaces').value;
    }

    if (document.getElementById('rbNewsletterManual') != null) {
        arg.NewsletterManual = $('#rbNewsletterManual').is(':checked');
    }

    if (document.getElementById('chkDocumentoProtegido') != null) {
        arg.Protected = $('#chkDocumentoProtegido').is(':checked');
    }

    if (document.getElementById('chkProtegerDocProtegido') != null) {
        arg.ProtectDocumentProtected = $('#chkProtegerDocProtegido').is(':checked');
    }

    if (document.getElementById('panRespuestasSup') != null) {
        // Cambiado por el nuevo Front
        //var inputRespuestas = $('#panRespuestasSup fieldset.encuestas input.respuestaEncuesta');
        var inputRespuestas = $('#panRespuestasSup div.encuestas input.respuestaEncuesta');
        var respuestas = '';

        inputRespuestas.each(function () {
            respuestas += $(this).val() + '[[&]]';
        });

        arg.PollResponses = respuestas;
    }

    if (document.getElementById('mTxtValorRdf') != null) {
        arg.RdfValue = document.getElementById('mTxtValorRdf').value;
    }

    if (document.getElementById('txtHackValorImgRepresentante') != null) {
        arg.ImageRepresentativeValue = document.getElementById('txtHackValorImgRepresentante').value;
    }

    if (document.getElementById('txtHackControlRep') != null && $('#txtHackControlRep').val() == 'omitir') {
        arg.SkipSemanticPropertyRepeat = true;
    }

    if (typeof (recursoEditandoCargMasID) != 'undefined') {
        arg.EditingMassiveResourceID = recursoEditandoCargMasID;
        arg.MassiveResourceLoadInfo = $('#txtHackCargaMasiva').val();
    }

    if (typeof (openSeaDragonInfoSem) != 'undefined') {
        arg.OpenSeaDragonInfo = openSeaDragonInfoSem;
    }

    if (document.getElementById('txtSubOntologias') != null) {
        arg.SubOntologiesExtInfo = document.getElementById('txtSubOntologias').value;
    }

    if (document.getElementById('txtEntidadesOntoIDs') != null) {
        var docID = document.URL.substring(0, document.URL.lastIndexOf('/'));
        docID = docID.substring(docID.lastIndexOf('/') + 1);
        var txtEntidadesOntoIDs = document.getElementById('txtEntidadesOntoIDs').value;
        if (docID != documentoID) {
            txtEntidadesOntoIDs = txtEntidadesOntoIDs.replace(new RegExp(docID, 'gi'), documentoID);
        }
        arg.EntityIDRegisterInfo = txtEntidadesOntoIDs;
    }

    // Antes de hacer guardar indicar que está "limpio" para avisar de que el formulario ha sido cambiado -> No avisará al usuario si cierra la ventana o actualiza
    if ($("#preventLeavingFormWithoutSaving").dirty != null) {
        $("#preventLeavingFormWithoutSaving").dirty("setAsClean");
    }
    
    
    GnossPeticionAjax(urlPaginaActual + '/saveresource', arg, true, redirectAtEnd).done(function (data) {
        if (formSemCargaMasiva) {
            PrepararCargaMasivaArchivos(data);
        }
        else if (!redirectAtEnd && data.indexOf('http' == 0)){
			if(typeof(onResourceSaved) == 'function'){
				onResourceSaved();
			}				
		}
        else if (data.indexOf('mensajeRepecion=') == -1) {
            if (data.indexOf('#divContPanelBotonera') != -1) {
                $('#divContPanelBotonera').css("display", "none");
            }
            $('#divContLblErrorDocumento').html(data);
        }
        else {
            CrearInfoRepeticionDatosFormSem(data);
        }
        OcultarUpdateProgress();
        GuardarEnabled();
    }).fail(function (data) {
        var error = data;
        var tipoError = '';

        if (data == "NETWORKERROR") {
            CrearErrorModfRec('Has perdido la conexión. Se está intentando recuperar el recurso, esta tarea puede llevar unos instantes.');
            comportamientoNetworkError.intentarRedirigirFichaRecurso(urlPaginaActual + '/checkedition', CrearErrorNetwork_GuardarRecurso);
        }
        else {
            if (data.indexOf("|") != 0) {
                tipoError = error.substr(0, error.indexOf('|'));
                $('#divContLblErrorDocumento').addClass(tipoError);
                error = error.substr(error.indexOf('|') + 1);
            }
            if (!formSemVirtual) {
                CrearErrorModfRec(error);
            }
            else {
                CrearErrorModfRec_FormSemVirtual(data);
            }
            GuardandoCambios = false;
            $('#divContPanelBotonera').show();
            OcultarUpdateProgress();
            GuardarEnabled();
        }
    });
    /*.always(function () {
        OcultarUpdateProgress();
        GuardarEnabled();
    });*/
}

function CrearErrorNetwork_GuardarRecurso(mensaje) {
    OcultarUpdateProgress();
    GuardarEnabled();
    var url = document.location.href.trim();
    var docId = documentoID;
    
    if (docId == null || docId == '') {
        if ($('#mTxtValorRdf').length > 0) {
            //es semántico y la url es del tipo editar-documento-creado/{docid}/{ontologiaid}
            url = url.substring(0, url.lastIndexOf('/'));
            docId = url.substring(url.lastIndexOf('/') + 1);
        }
        else {
            //no es semántico y la url es del tipo editar-recurso/{nombreRecurso}/{docid}
            docId = url.substring(url.lastIndexOf('/') + 1);
        }
    }
    var urlFicha = $('#inpt_baseUrlBusqueda').val() + "/" + textoRecursos.recurso + "/title/" + docId;
    mensaje += '<a href=\"' + urlFicha + '\" target="_blank">siguiendo este enlace<\/a>';
    CrearErrorModfRec(mensaje);
}

function checkOtrosAutores() {
    rb = document.getElementById('rbAutorPropio');
    AjustarPropiedadIntelectual(rb, document.getElementById('chkCompartir'));
}

function BtnAgregarAutores_Click() {
    document.getElementById('lbAgregarAutores').style.display = 'none';
    document.getElementById('fielAutores').className = 'labels';
}



function TextCambio_Click() {
    if ($("#modificacionVinculado")[0] == null) {
             $(".vinculados").append('<input type="hidden" id="modificacionVinculado">');
    }
}

function BtnAgregarVinculados_Click() {
    $(".vinculados").append('<div><input type="text" name="txtTitulo" class="text big" value="" onchange="TextCambio_Click()"><a onclick="event.preventDefault();$(this).parent().remove();TextCambio_Click();">x</a><div>');

    if($("#modificacionVinculado")[0] == null){
        $(".vinculados").append('<input type="hidden" id="modificacionVinculado">');
    } 
}

function ReajustarCompartir() {
    AjustarPropiedadIntelectual(document.getElementById('rbAutorPropio'), document.getElementById('chkCompartir'));
}

/**
 * Acción de inicio de carga de un fichero adjunto para la creación de un recurso
 * - Si hay o no algún error, asignará una clase de bootstrap para indicarlo.
 * - Establecer en el input type "file" el nombre del fichero que se ha seleccionado.
 */
function InicioCargarArchivo_ModfRec() {

    // Input de carga del fichero adjunto
    inputCargaFichero = $("#fuExaminar");

    inputCargaFichero.css('background-color', 'white');
    var doc = document.getElementById("fuExaminar");
    if (doc.value.length > 0) {
        var data = new FormData();
        var files = inputCargaFichero.get(0).files;
        if (files.length > 0) {
            $('#panEliminarAdjunto').show();
            document.getElementById('imgEsperaArchivo').style.display = '';
            document.getElementById('txtHackArchivo').value = guidGenerator() + files[0].name;
            //Para cuando viene del metodo nuevo.
            if (window.location.pathname.includes('anyadir-recurso')) {
                document.getElementById('txtHackEnlaceDoc').value = files[0].name;
            }

            GuardarDisabled(true);

            data.append("File", files[0]);
            data.append("FileName", document.getElementById('txtHackArchivo').value);
            data.append("documentoID", documentoID);

            GnossPeticionAjax(urlPaginaActual + '/attachfile', data, true).done(function (data) {
                // Cambiado para el nuevo Front
                //$("#fuExaminar").css('background-color', 'lime');
                // Añadir clase para indicar la subida del fichero ha sido correcta
                inputCargaFichero.toggleClass("is-valid");
                // Mostrar el nombre del fichero en el input de tipo Browser
                inputCargaFichero.next().html(files[0].name);

                document.getElementById('imgEsperaArchivo').style.display = 'none';
                document.getElementById('lblErrorCargarArchivo').style.display = 'none';
                GuardarEnabled();
                if (typeof (completadaCargaFicheroAdjunto) != "undefined")
                {
                    completadaCargaFicheroAdjunto.init();
                }
            }).fail(function (data) {
                // Añadir clase para indicar la subida del fichero ha sido erronea
                inputCargaFichero.toggleClass("is-invalid");
                ErrorCargarArchivo();
            });
        }

        return true;
    }
}

function ErrorCargarArchivo() {
    $("#fuExaminar").css('background-color', 'red');
    document.getElementById('imgEsperaArchivo').style.display = 'none';
    document.getElementById('lblErrorCargarArchivo').style.display = '';
    GuardarEnabled();
}

function GuardarDisabled(archivo) {
    var btnPub = $('#lbPublicar');
    var btnBorr = $('#lbGuardarBorrador');

    btnPub.attr('tag', btnPub.attr('onclick'));
    btnBorr.attr('tag', btnBorr.attr('onclick'));

    btnPub.attr('onclick', '');
    btnBorr.attr('onclick', '');

    btnPub.attr('tagText', btnPub.attr('value'));

    if (archivo) {
        btnPub.attr('value', textoRecursos.subiendoArchivo + "...");
    }
    else {
        btnPub.attr('value', textoRecursos.guardando + "...");
    }
}

function GuardarEnabled() {
    var btnPub = $('#lbPublicar');
    var btnBorr = $('#lbGuardarBorrador');

    btnPub.attr('onclick', btnPub.attr('tag'));
    btnBorr.attr('onclick', btnBorr.attr('tag'));

    btnPub.attr('tag', '');
    btnBorr.attr('tag', '');

    btnPub.attr('value', btnPub.attr('tagText'));
    btnPub.attr('tagText', '');
}

//Newsletter

function MostrarNewsletter(pBoton) {
    if (pBoton == 'manual') {
        document.getElementById('divNewsletter').className = 'oculto';
        document.getElementById('divDescripcion').className = '';
        document.getElementById('txtHackModosNewsletter').value = 'manual';
    }
    else {
        document.getElementById('divNewsletter').className = '';
        document.getElementById('divDescripcion').className = 'oculto';
        document.getElementById('txtHackModosNewsletter').value = 'archivo';
    }
}

function InicioCargarNewsletter() {
    $("#fuExaminarNewsletter").css('background-color', 'white');
    var doc = document.getElementById("fuExaminarNewsletter");
    if (doc.value.length > 0) {
        var data = new FormData();
        var files = $("#fuExaminarNewsletter").get(0).files;
        if (files.length > 0) {
            MostrarUpdateProgress();
            document.getElementById('txtHackArchivo').value = guidGenerator() + files[0].name;

            data.append("File", files[0]);
            data.append("FileName", document.getElementById('txtHackArchivo').value);
            data.append("IsNewsLetter", true);
            data.append("documentoID", documentoID);

            GnossPeticionAjax(urlPaginaActual + '/attachfile', data, true).done(function (data) {
                $("#iframeNewsletter").css('background-color', 'lime');
                $('#iframeNewsletter').attr('src', $('.inpt_baseURL').val() + '/objetonewsletter.aspx?docid=' + documentoID + '&temp=true');
                document.getElementById('txtHackModosNewsletter').value = 'archivo';
                OcultarUpdateProgress();
            }).fail(function (data) {
                $("#iframeNewsletter").css('background-color', 'red');
            });
        }

        return true;
    }
}

function newsletterLoad() {
    var iframe = document.getElementById('iframeNewsletter');
    iframe.style.display = '';
    try {
        if (!window.opera && document.all && document.getElementById) {
            iframe.height = iframe.contentWindow.document.body.scrollHeight;
        } else if (document.getElementById) {
            iframe.height = iframe.contentDocument.body.scrollHeight + 'px';
        }
    }
    catch (error) {
        iframe.height = 600;
    }
}

function AjustarPrivacidadRecurso(pRbSelecc) {
    if (pRbSelecc.id == 'rbEditoresYo') {
        document.getElementById('divContDespEdit').className = 'oculto';
    }
    else if (pRbSelecc.id == 'rbEditoresOtros') {
        document.getElementById('divContDespEdit').className = '';
    }
    else if (pRbSelecc.id == 'rbLectoresComunidad' || pRbSelecc.id == 'rbAbierto') {
        $('#spanCompartirPrivado').css('display', 'none');
        $('#spanCompartirPublico').css('display', '');

        document.getElementById('divContDespLect').className = 'oculto';
    }
    else if (pRbSelecc.id == 'rbLectoresEditores') {
        $('#spanCompartirPrivado').css('display', '');
        $('#spanCompartirPublico').css('display', 'none');

        document.getElementById('divContDespLect').className = 'oculto';
    }
    else if (pRbSelecc.id == 'rbLectoresEspecificos') {
        $('#spanCompartirPrivado').css('display', '');
        $('#spanCompartirPublico').css('display', 'none');

        document.getElementById('divContDespLect').className = '';
    }
}

function InicializarModificarRecurso(editandoFormSem) {
    if (CKEDITOR.instances.txtDescripcion == null) {
        RecargarTodosCKEditor();
    }

    if ($('#txtDescripcion').length > 0) {
        CKEDITOR.instances.txtDescripcion.on('blur', function () {
            var descripcion = $('#txtDescripcion').val();
            EtiquetadoAutomaticoDeRecursos($('#txtTitulo').val(), descripcion, $('#txtHackTagsDescripcion'), true);
        });
    }

    $('#txtTitulo').on('blur', function () {
        EtiquetadoAutomaticoDeRecursos(this.value, $('#txtDescripcion').val(), $('#txtHackTagsTitulo'), editandoFormSem);
    });

    ConfigurarSelectoresEditores();

    ActivarAutocompletarRec('txtTags', 'sioc_t:Tag');
    ActivarAutocompletarRec('txtAutores', 'gnoss:hasautor');

    $('#fuExaminar').change(InicioCargarArchivo_ModfRec);

    $('#fuExaminarNewsletter').change(InicioCargarNewsletter);

    $('#lbEliminarAutoGuardado').click(function () {
        MostrarUpdateProgress();

        var extraDelete = '';

        if (document.getElementById('txtHackEnlaceDoc') != null) {
            extraDelete = '?recuperarAutoGuardado=' + encodeURIComponent(document.getElementById('txtHackEnlaceDoc').value);
        }

        GnossPeticionAjax(urlPaginaActual + '/delteautosave' + extraDelete, {}, true).done(function (data) {
            $('#panelRecuperarAutoGuardado').remove();
        }).fail(function (data) {
            CrearErrorModfRec(data);
        }).always(function () {
            OcultarUpdateProgress();
        });
    });

    if ($('#cmbBaseRecursos').length > 0) {//Es peticion AddToGnoss
        EtiquetadoAutomaticoDeRecursos($('#txtTitulo').val(), $('#txtDescripcion').val(), $('#txtHackTagsTitulo'), editandoFormSem);
        $('#lbAddBr').click(AgregarBRAddToGnoss);
        $('#cmbBaseRecursos').on('change', CambiarBRAddToGnoss);
    }
}

function MarcarCatSelEditorTes(nodo) {
    var padre = $(nodo).parent();
    var checked = nodo.checked;
    var catID = padre.attr('class');

    padre = $(nodo).parents('.divTesArbol');

    if (padre.length == 0) {
        padre = $(nodo).parents('.divTesLista');
    }

    var chechkOtraVista = null;
    if (padre.attr('class') == 'divTesArbol') {
        chechkOtraVista = $('.divTesLista .' + catID + ' input');
    }
    else {
        chechkOtraVista = $('.divTesArbol .' + catID + ' input');
    }

    if (chechkOtraVista.length > 0) {
        chechkOtraVista[0].checked = checked;
    }

    var txtHack = $('.hackCatTesSel', padre.parent());

    if (checked) {
        txtHack.val(txtHack.val() + catID + ',');
    }
    else {
        txtHack.val(txtHack.val().replace(catID + ',', ''));
    }
}

function ConfigurarSelectoresEditores() {
    $('.filtroFacetaSelectUsuRec').autocomplete(
        null,
        {
            //servicio: new WS($('#inpt_urlServicioAutocompletar').val(), WSDataType.jsonp),
            //metodo: 'AutoCompletarLectoresEditores',
            url: $('#inpt_urlServicioAutocompletar').val() + "/AutoCompletarLectoresEditores",

            delay: 0,
            scroll: false,
            max: 30,
            selectFirst: false,
            minChars: 1,
            width: 300,
            cacheLength: 0,
            NoPintarSeleccionado: true,
            multiple: true,
            classTxtValoresSelecc: 'hackUsuSeleccionadoRec',

            extraParams: {
                grupo: '',
                identidad: idIdentidadActual,
                organizacion: idOrgDeIdentidadActual,
                proyecto: $('#inpt_proyID').val(),
                bool_edicion: 'true',
                bool_traergrupos: 'true',
                bool_traerperfiles: 'true'
            }
        }
    );

    $('.filtroFacetaSelectUsuRec').result(function (event, data, formatted) {
        AceptarEditorSelectorUsuRec(this, data[0], data[1]);
    });
}

/**
 * Método para eliminar el elemento (usuario seleccionado) cuando se pulsa en (x) tanto del txtHack oculto correspondiente como del contenedor que lo contiene
 * @param {any} nodo
 * @param {any} perfilID
 */
function EliminarUsuarioSelecUsu(nodo, perfilID) {
    var txtValores = $('.hackUsuSeleccionadoRec', $(nodo).parents('.divSelectorUsuRec'));
    txtValores.val(txtValores.val().replace(perfilID + ',', ''));
    // Actualizado debido al nuevo front
    //$(nodo).parent().remove();
    $(nodo).parent().parent().remove();
}

/**
 * Método que se ejecuta al seleccionar un usuario-editor una vez se ha buscado y "Autocompletar" ha proporcionado una lista de resultados
 * @param {any} txtautocomp: El inputText que ha lanzado a "Autocompletar"
 * @param {any} nombre: Texto resultado sobre el que se ha pulsado dentro de la lista de opciones que "Autocompletar" ha proporcionado
 * @param {any} id: Identificador del resultado que se ha seleccionado de entre la lista de opciones que "Autocompletar" ha proporcionado
 */
function AceptarEditorSelectorUsuRec(txtautocomp, nombre, id) {
    var padreTxt = $(txtautocomp).parents('.divSelectorUsuRec');
    var txtValores = $('.hackUsuSeleccionadoRec', padreTxt);
    txtValores.val(txtValores.val() + id + ',');

    // Cambiado para el nuevo front
    //$('.ususSeleccionados ul', padreTxt).append('<li>' + nombre + '<a class="remove" onclick="EliminarUsuarioSelecUsu(this, \'' + id + '\');">' + borr.eliminar + '</a></li>');
    var editorSeleccionadoHtml = '';
    editorSeleccionadoHtml += '<div class="tag">';
    editorSeleccionadoHtml += '<div class="tag-wrap">';
    editorSeleccionadoHtml += '<span class="tag-text">' + nombre + '</span>';
    editorSeleccionadoHtml += "<span class=\"tag-remove material-icons\" onclick=\"EliminarUsuarioSelecUsu(this,'"+id+"');\">close</span>";
    editorSeleccionadoHtml += '<input type="hidden" value="'+ id +'" />';
    editorSeleccionadoHtml += '</div>';
    // Añadir el item en el contenedor ususSeleccionados
    $('.ususSeleccionados', padreTxt).append(editorSeleccionadoHtml);
     
    $(txtautocomp).val('');
}



/**
 * Método que se ejecuta al seleccionar un usuario para formar parte como miembro de una comunidad del listado que ha proporcionado una lista de resultados autocomplete
 * @param {any} txtautocomp: El inputText que ha lanzado a "Autocompletar"
 * @param {any} nombre: Texto resultado sobre el que se ha pulsado dentro de la lista de opciones que "Autocompletar" ha proporcionado
 * @param {any} id: Identificador del resultado que se ha seleccionado de entre la lista de opciones que "Autocompletar" ha proporcionado
 */
function AceptarUsuarioMiembroComunidad(txtautocomp, nombre, id, identidad) {
    var padreTxt = $(txtautocomp).parents('#panAgregarMiembros');
    var txtValores = $('#txtHackInvitados', padreTxt);
    //txtValores.val(txtValores.val() + id + ',');
    txtValores.val(txtValores.val() + "&" + id);

    // Cambiado para el nuevo front
    //$('.ususSeleccionados ul', padreTxt).append('<li>' + nombre + '<a class="remove" onclick="EliminarUsuarioSelecUsu(this, \'' + id + '\');">' + borr.eliminar + '</a></li>');
    var editorSeleccionadoHtml = '';
    editorSeleccionadoHtml += '<div class="tag">';
    editorSeleccionadoHtml += '<div class="tag-wrap">';
    editorSeleccionadoHtml += '<span class="tag-text">' + nombre + '</span>';
    editorSeleccionadoHtml += "<span class=\"tag-remove material-icons\" onclick=\"EliminarUsuarioMiembroComunidad(this,'" + id + "');\">close</span>";
    editorSeleccionadoHtml += '<input type="hidden" value="' + id + '" />';
    editorSeleccionadoHtml += '</div>';
    // Añadir el item en el contenedor ususSeleccionados
    $('.ususSeleccionados', padreTxt).append(editorSeleccionadoHtml);

    $(txtautocomp).val('');
}

/**
 * Método para eliminar el elemento (usuario para formar parte de un grupo) cuando se pulsa en (x) tanto del txtHack oculto correspondiente como del contenedor que lo contiene
 * @param {any} nodo
 * @param {any} perfilID
 */
function EliminarUsuarioMiembroComunidad(nodo, perfilID) {
    var txtValores = $('.hackUsuSeleccionadoRec', $(nodo).parents('#panAgregarMiembros'));

    // Tener en cuenta que no haya el carácter "&". En algunos inputs no se hace separación con "," sino con "&".
    if (txtValores.val().includes("&")){
        txtValores.val(txtValores.val().replace("&"+ perfilID, ''));
    } else {
        txtValores.val(txtValores.val().replace(perfilID + ',', ''));
    }

    // Eliminar el item recién eliminado
    $(nodo).parent().parent().remove();
}

/**
 * Método para editar y crear un grupo de miembros para una comunidad.
 * Primero comprobará si los campos están vacíos y si no lo están procederá a realizar la petición
 * @param {any} inputs: Array de objetos que contiene input, label y si es necesario revisar que sea o no vacío
 * @param {any} labelError: Label donde se mostrará posibles errores.
 * @param {any} labelError: Url a la que habrá que realizar la petición de guardado
 * txtTitulo, txtDescripcion,txtTags_Hack,txtHackInvitados
 */
const EnviarDatosServidorCrearEditarGrupoComunidad = (inputs, labelError, urlSaveGroup) => {
    // Bandera para comprobar si hay algun input vacío o no
    let isInputEmpty = false;
    let errorMessage = '';
    // Ocultamos y vaciamos el el bloque donde estará el label de error por defecto
    labelError.parent().hide();
    labelError.empty();


    // Recorrer cada input y comprobar si están o no vacíos
    inputs.forEach(input => {
        if (input.checkValue === true) {
            errorMessage = ValidarCampoNoVacio(input.inputId, input.label);
            if (errorMessage != "") {
                isInputEmpty = true;
                // Mostramos el bloque donde estará el label de error
                labelError.parent().show();
                labelError.append(errorMessage);                
            }        
        }       
    });

    if (isInputEmpty === false) {
        // Realizar el guardado
        MostrarUpdateProgress();
        //Miembros
        var miembros = $('#' + inputs[3].inputId).val().replace(/\&/g, '[-|-]');

        var dataPost = {
            Titulo: $('#'+ inputs[0].inputId).val(),
            Descripcion: encodeURIComponent($('#' + inputs[1].inputId).val().replace(/\n/g, '')),
            Tags: $('#' + inputs[2].inputId).val(),
            Participantes: miembros
        }

        GnossPeticionAjax(urlSaveGroup, dataPost, true).done(function (data) {
            console.log(data);
            OcultarUpdateProgress();
        }).fail(function (data) {
            if (data == "ERROR Titulo") {
                //$('#txtTitulo').addClass('error');
                // Mostramos el bloque donde estará el label de error
                labelError.parent().show();
                labelError.append('<p>Se ha producido un error al guardar el grupo de la comunidad. Por favor inténtalo de nuevo más tarde.</p>');
                OcultarUpdateProgress();
            }
        });
    }    
}

function ActivarAutocompletarRec(pTxt, pNombreFaceta) {
    $('#' + pTxt).autocomplete(null, {
        //servicio: new WS($('#inpt_urlServicioAutocompletar').val(), WSDataType.jsonp),
        //metodo: 'AutoCompletarFacetas',
        url: $('#inpt_urlServicioAutocompletar').val() + "/AutoCompletarFacetas",
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
            proyecto: $('#inpt_proyID').val(),
            bool_esMyGnoss: ($('#inpt_bool_esMyGnoss').val().toLowerCase() == "true"),
            bool_estaEnProyecto: ($('#inpt_bool_estaEnProyecto').val().toLowerCase() == "true"),
            bool_esUsuarioInvitado: ($('#inpt_bool_esUsuarioInvitado').val().toLowerCase() == "true"),
            identidad: $('#inpt_identidadID').val(),
            nombreFaceta: pNombreFaceta,
            orden: '',
            filtrosContexto: '',
            languageCode: $('#inpt_Idioma').val(),
            parametros: '',
            tipo: '',
            perfil: '',
            organizacion: ''
        }
    }
    );
}

/**
 * Acción que se ejecuta desde la creación de un recurso de tipo Encuesta para añadir una respuesta adicional.   
 */
function AgregarRespuestaEncuesta() {
    // Cambiado por el nuevo Front
    //var respuestas = $('#panRespuestasSup fieldset.encuestas');
    var respuestas = $('#panRespuestasSup div.encuestas');

    if (respuestas.length > 0) {
        var num = respuestas.length + 1;
        // Cambiado por el nuevo Front
        //$('#panRespuestasSup #panRespuestas').append('<fieldset class="encuestas" id="panRespuesta' + num + '">' + $(respuestas[0]).html() + '</fieldset>');
        $('#panRespuestasSup #panRespuestas').append('<div class="form-group encuestas" id="panRespuesta' + num + '">' + $(respuestas[0]).html() + '</div>');        
        // Vaciamos el input por si contiene una pregunta anterior
        $('#panRespuestasSup #panRespuesta' + num + ' input.respuestaEncuesta').val('');

        AjustarRespuestasEncuesta();
    }
}

/**
 * Acción que se ejecuta después de que una respuesta haya sido añadida desde la creación de un recurso de tipo "Encuesta".
 * Le añadirá o no los botones de mover arriba/abajo
 */
function AjustarRespuestasEncuesta() {

    // Cambiado por nuevo diseño
    //var respuestas = $('#panRespuestasSup fieldset.encuestas');
    var respuestas = $('#panRespuestasSup div.encuestas');

    for (var i = 0; i < respuestas.length; i++) {
        if (i == 0) {
            /*$('.respuestaUp', respuestas[i]).css('display', 'none');*/
            $('.respuestaUp', respuestas[i]).addClass('disabled');
        }
        else {
            /*$('.respuestaUp', respuestas[i]).css('display', '');*/
            $('.respuestaUp', respuestas[i]).removeClass('disabled');
        }

        if (i < (respuestas.length - 1)) {
            /*$('.respuestaDown', respuestas[i]).css('display', '');*/
            $('.respuestaDown', respuestas[i]).removeClass('disabled');
        }
        else {
            /*$('.respuestaDown', respuestas[i]).css('display', 'none');*/
            $('.respuestaDown', respuestas[i]).addClass('disabled');
        }

        if (respuestas.length > 2) {
            /*$('.respuestaDelete', respuestas[i]).css('display', '');*/
            $('.respuestaDelete', respuestas[i]).removeClass('disabled');
        }
        else {
            /*$('.respuestaDelete', respuestas[i]).css('display', 'none');*/
            $('.respuestaDelete', respuestas[i]).addClass('disabled');
        }

        var label = $('label', respuestas[i])
        label.text(label.text().substring(0, label.text().lastIndexOf(' ')) + ' ' + (i + 1));

        $(respuestas[i]).attr('id', 'panRespuesta' + (i + 1));
    }
}

/**
 * Acción para subir de posición una respuesta al pulsar en el botón de respuestaUp
 * @param {any} boton: Botón pulsado
 */
function UpRespuestaEncuesta(boton) {
    // Cambiado por nuevo Front
    //var idRepuesta = parseInt($(boton).parents('fieldset.encuestas')[0].id.replace('panRespuesta', ''));
    var idRepuesta = parseInt($(boton).parents('div.encuestas')[0].id.replace('panRespuesta', ''));

    if (idRepuesta > 1) {
        // Cambiado por nuevo Front
        //var respuestas = $('#panRespuestasSup fieldset.encuestas');
        var respuestas = $('#panRespuestasSup div.encuestas');
        var inputAnt = $('.respuestaEncuesta', respuestas[idRepuesta - 2]);
        var inputPost = $('.respuestaEncuesta', respuestas[idRepuesta - 1]);
        var textoAnt = inputAnt.val();
        inputAnt.val(inputPost.val());
        inputPost.val(textoAnt);
    }
}

/**
 * Acción para bajar de posición una respuesta al pulsar en el botón de respuestaDown
 * @param {any} boton: Botón pulsado
 */
function DownRespuestaEncuesta(boton) {

    // Cambiado por nuevo Front
    //var idRepuesta = parseInt($(boton).parents('fieldset.encuestas')[0].id.replace('panRespuesta', ''));
    var idRepuesta = parseInt($(boton).parents('div.encuestas')[0].id.replace('panRespuesta', ''));
    // Cambiado por nuevo Front
    //var respuestas = $('#panRespuestasSup fieldset.encuestas');
    var respuestas = $('#panRespuestasSup div.encuestas');

    if (idRepuesta < respuestas.length) {

        var inputAnt = $('.respuestaEncuesta', respuestas[idRepuesta - 1]);
        var inputPost = $('.respuestaEncuesta', respuestas[idRepuesta]);
        var textoAnt = inputAnt.val();
        inputAnt.val(inputPost.val());
        inputPost.val(textoAnt);
    }
}

/**
 * Acción que se ejecuta cuando se pulsa en "Eliminar" una respueta de una encuesta en el momento de la creación de un recurso de tipo Encuesta.
 * @param {any} boton: Botón de eliminar encuesta pulsado
 */

function EliminarRespuestaEncuesta(boton) {
    // Cambiado por nuevo diseño
    //$(boton).parents('fieldset.encuestas').remove();
    $(boton).parents('div.encuestas').remove();
    AjustarRespuestasEncuesta();
}

//FIN REGION Modificar Recurso

//REGEN Form Sem

function InicializarModificarRecursoSemantico(txtsTitulos, txtsDescripciones, editandoFormSem) {
    $(document.body).prop('onBeforeUnload', 'ComprobarCambios();');

    if (CKEDITOR.instances.txtDescripcion == null) {
        RecargarTodosCKEditor();
    }

    if (txtsTitulos == '') {
        txtsTitulos = 'txtTitulo';
    }

    if (txtsDescripciones == '') {
        txtsDescripciones = 'txtDescripcion';
    }

    TxtTitulosSem = txtsTitulos;

    EnlazarEtiquetadoAutoFormSem(txtsTitulos, txtsDescripciones, 'txtHackTagsTitulo', 'txtHackTagsDescripcion', 'txtTags', editandoFormSem);
    AgregarOnBlurCKEditors();

    ConfigurarSelectoresEditores();

    ActivarAutocompletarRec('txtTags', 'sioc_t:Tag');
    ActivarAutocompletarRec('txtAutores', 'gnoss:hasautor');

    $('#fuExaminarSemCms').change(InicioCargarArchivo_SemCms);

    if (typeof ($('.calenFormSem').datepicker) != 'undefined') {
        // Preparar el .datePicker para que esté disponible en la web
        const oldYear = moment().format('YYYY') - 110;
        const currentYear = moment().format('YYYY');
        $('.calenFormSem').datepicker({
            changeMonth: true,
            changeYear: true,
            yearRange: `${oldYear}:${currentYear}`,
        });
    }
    if (typeof ($('.calenTimeFormSem').datetimepicker) != 'undefined') {
        $('.calenTimeFormSem').datetimepicker({
            format: 'd/m/Y H:i:s',
            defaultTime: '12:00',
            onSelectDate: AjustarHoraCalendario,
            onSelectTime: AjustarHoraCalendario,
            lang: $('#inpt_Idioma').val(),
            dayOfWeekStart: 1
        });
    }

    if (!formSemVirtual) {
        var existingHandler = window.onbeforeunload;
        window.onbeforeunload = function (event) {
            if (existingHandler) existingHandler(event);
            return ComprobarCambios();
        }
    }

    var aspaAutocompletar = $('a.removeAutocompletar');
    if (aspaAutocompletar.length > 0) {
        aspaAutocompletar.click(function () {
            var contenedor = $(this).parent();
            var inputAutocompletar = $('input.autocompletarSelecEnt', contenedor);
            var inputSelEntId = inputAutocompletar.attr('id').replace('hack_', 'selEnt_');
            var inputSelEnt = $('#' + inputSelEntId);
            if (inputSelEnt.length > 0) {
                inputSelEnt.val('');
            }
            inputAutocompletar.val('');
            inputAutocompletar.prop("disabled", false);
            $(this).remove();
        });
    }
}

function AjustarHoraCalendario(ct, $i) {
    var valor = $i.val();
    $i.val(valor.substring(0, valor.length - 2) + '00');
}

function EnlazarEtiquetadoAutoFormSem(pTitulos, pDescripciones, pHackTit, pHackDesc, pTags, pEditandoFormSem) {
    try {
        txtHackTagsTituloID = pHackTit;
        txtHackTagsDescripcionID = pHackDesc;
        txtTagsID = pTags;

        var tits = pTitulos.split(',');
        for (var i = 0; i < tits.length; i++) {
            $('#' + tits[i]).blur(function () {
                EtiquetadoAutomaticoDeRecursos(this.value, '', $('#' + txtHackTagsTituloID), pEditandoFormSem)
            });
        }

        //RecargarTodosCKEditor();
        var descrp = pDescripciones.split(',');
        for (var i = 0; i < descrp.length; i++) {
            if (descrp[i] != '' && $('#' + descrp[i]).length > 0) {
                CKEDITOR.instances[descrp[i]].on('blur', function () {
                    var descripcion = $('#' + this.name).val();
                    EtiquetadoAutomaticoDeRecursos('', descripcion, $('#' + txtHackTagsDescripcionID), false);
                });
            }

        }
    }
    catch (e) { }
}

function autocompletarGenericoFormSem(control, faceta, tipoResultado) {
    $(control).autocomplete(
    null,
    {
        servicio: new WS($('#inpt_urlServicioAutocompletar').val(), WSDataType.jsonp),
        metodo: 'AutoCompletarGrafoDocSem',
        delay: 0,
        scroll: false,
        selectFirst: false,
        minChars: 1,
        width: $(control).width,
        cacheLength: 0,
        matchCase: true,
        extraParams: {
            pFaceta: faceta,
            pTipoResultado: tipoResultado
        }
    }
    );
    control.attributes['onfocus'].value = ''
    control.attributes.removeNamedItem('onfocus')
}

function InicioCargarArchivo_SemCms() {
    var doc = document.getElementById("fuExaminarSemCms");
    if (doc.value.length > 0) {
        var data = new FormData();
        var files = $("#fuExaminarSemCms").get(0).files;
        if (files.length > 0) {
            MostrarUpdateProgress();
            MostrarLoadingArchivo();

            var idCampoControl = document.getElementById('txtHackArchivoSelecc').value.split('|')[0];
            var entProp = ObtenerEntidadPropiedadSegunID(idCampoControl, TxtRegistroIDs);
            var idioma = '';

            if (document.getElementById('txtHackArchivoSelecc').value.split('|')[1] != 'archivofileUpLoad' && document.getElementById('txtHackArchivoSelecc').value.split('|')[1] != 'archivoLinkfileUpLoad') {
                var guidAleatorio = guidGenerator();
                document.getElementById('txtHackArchivoSelecc').value += '|' + guidAleatorio;
            }
            else {
                var nombreFich = doc.value;
                if (nombreFich.indexOf("\\") != -1) {
                    nombreFich = nombreFich.substring(nombreFich.lastIndexOf("\\") + 1);
                }

                if (EsPropiedadMultiIdioma(entProp[0], entProp[1])) {
                    idioma = ObtenerIdiomaPesanyaActual(entProp[0], entProp[1]);

                    if (idioma != null && idioma != '') {
                        idioma = '@' + idioma;
                    }
                    else {
                        idioma = '';
                    }
                }

                var guidAleatorio = guidGenerator();
                document.getElementById('txtHackArchivoSelecc').value += '|' + nombreFich + '|' + guidAleatorio;
            }

            var extraSemCms = document.getElementById('txtHackArchivoSelecc').value + idioma;
            extraSemCms = entProp[1] + ',' + entProp[0] + extraSemCms.substring(extraSemCms.indexOf('|'));

            var docRecExtID = GetDocRecExtSelecEntEditable(entProp[0], entProp[1], true, null);

            if (docRecExtID != null) {
                extraSemCms += '|' + docRecExtID;
            }

            LimpiarHtmlControl(GetIDControlError(entProp[0], entProp[1], TxtRegistroIDs));

            data.append("File", files[0]);
            data.append("FileName", doc.value);
            data.append("ExtraSemCms", extraSemCms);
            data.append("documentoID", documentoID);

            if (typeof (InicioCargarArchivo_SemCmsPersonalizado) != 'undefined') {
                InicioCargarArchivo_SemCmsPersonalizado();
            }

            GnossPeticionAjax(urlPaginaActual + '/attachfile', data, true).done(function (dataResponse) {
                FinCargarArchivo_SemCms(dataResponse);
            }).fail(function (dataResponse) {
                var mensajeError = dataResponse;
                if (dataResponse == "NETWORKERROR") {
                    mensajeError = 'Has perdido la conexión. Comprueba tu conexión a internet e intenta adjuntar el recurso de nuevo.';
                }
                ErrorCargarArchivo_SemCms(mensajeError);
            });
        }

        return true;
    }
}

function FinCargarArchivo_SemCms(data) {
    if (navigator.appName == 'Microsoft Internet Explorer' || navigator.userAgent.indexOf('Safari') != -1) {
        $('#divContArchiInicio').css('display', 'none');
        //$('#fuExaminarSemCms' ).val('');
    }

    var nombreDoc = $('input[type="file"]').val();
    var extensionDoc = nombreDoc.substr(nombreDoc.lastIndexOf('.'));

    OcultarUpdateProgress();
    OcultarLoadingArchivo();
    $('#fuExaminarSemCms').val('');

    var docID = documentoID;
    
    if (docID == null || docID == '') {
        docID = document.URL.substring(0, document.URL.lastIndexOf('/'));
        docID = docID.substring(docID.lastIndexOf('/') + 1);
    }
    var txtHackID = 'txtHackArchivoSelecc';
    var idCampoControl = $('#' + txtHackID).val().split('|')[0];
    var entProp = ObtenerEntidadPropiedadSegunID(idCampoControl, TxtRegistroIDs);
    var docRecExtID = GetDocRecExtSelecEntEditable(entProp[0], entProp[1], true);

    if (docRecExtID != null) {
        docID = docRecExtID;
    }

    if (!formSemVirtual) {
        var rutaImg = 'imagenes/Documentos/imgsem/';
        var rutaVideosSem = 'VideosSemanticos/';
        var rutaDocLinks = 'doclinks/';

        var datosJCrop = GetCaracteristicaPropiedad(entProp[0], entProp[1], TxtCaracteristicasElem, 'UsarJcrop');
        if (datosJCrop != null) {
            var txtHack = $('#' + txtHackID).val();
            var fileUpload = txtHack.split('|')[1];
            var elementoID = txtHack.split('|')[2];

            var urlContent = $('input.inpt_baseURLContent').val();

            var valorProp = urlContent + '/' + rutaImg + "temp/" + docID.substring(0, 2) + '/' + docID.substring(0, 4) + '/' + docID + '/' + elementoID + extensionDoc;

            valorProp = valorProp + '?v=' + Math.round(Math.random() * 10000);

            var idCampoControl = txtHack.split('|')[0];
            var entProp = ObtenerEntidadPropiedadSegunID(idCampoControl, TxtRegistroIDs);

            MontarJCROP($('#panContenedorJcrop'), valorProp, entProp[1] + ',' + entProp[0], docID, datosJCrop);
        }
        else {
            AgregarArchivoComoPropiedad(txtHackID, docID, 'txtHackValorImgRepresentante', rutaImg, rutaVideosSem, rutaDocLinks, data, extensionDoc);
        }
    }
    else {
        var ontologiaID = document.URL.substring(document.URL.lastIndexOf('/') + 1);
        AgregarArchivoComoPropiedad('txtHackArchivoSelecc', docID, 'txtHackValorImgRepresentante', 'documentosvirtuales/' + ontologiaID + '/', 'documentosvirtuales/' + ontologiaID + '/', 'documentosvirtuales/' + ontologiaID + '/', data, extensionDoc);
    }

    if ((typeof CompletadaCargaSubidaArchivo_SemCms != 'undefined')) {
        CompletadaCargaSubidaArchivo_SemCms();
    }
}

function ErrorCargarArchivo_SemCms(pMens) {
    OcultarLoadingArchivo();
    OcultarUpdateProgress();
    $('#fuExaminarSemCms').val('');
    var idCampoControl = $('#txtHackArchivoSelecc').val().split('|')[0];
    var entProp = ObtenerEntidadPropiedadSegunID(idCampoControl, TxtRegistroIDs);

    if (pMens == null || pMens == '') {
        pMens = textoFormSem.errorSubirAdjunto;
    }

    MostrarErrorPropiedad(entProp[0], entProp[1], pMens, TxtRegistroIDs);

    if (typeof (FinErrorCargarArchivo_SemCms) != 'undefined') {
        FinErrorCargarArchivo_SemCms(pMens);
    }
}

function MostrarLoadingArchivo() {
    var control = document.getElementById('txtHackArchivoSelecc').value.split('|')[0];
    $('#' + control.replace('Campo_', 'divAgregarArchivo_')).css('display', 'none');
    $('#' + control.replace('Campo_', 'divArchivoAgregandose_')).css('display', '');
}

function OcultarLoadingArchivo() {
    var control = document.getElementById('txtHackArchivoSelecc').value.split('|')[0];
    $('#' + control.replace('Campo_', 'divAgregarArchivo_')).css('display', '');
    $('#' + control.replace('Campo_', 'divArchivoAgregandose_')).css('display', 'none');
}

function FinPeticionFormFan() {
    if (typeof (FinPeticionFormFanEspecifico) != 'undefined') {
        FinPeticionFormFanEspecifico();
    }
}

function CrearInfoRepeticionDatosFormSem(data) {
    var mensRep = data.substring(0, data.indexOf('|||mensajeRepecionNoContinua='));
    mensRep = mensRep.substring('mensajeRepecion='.length);
    var mensRepNoCont = data.substring(data.indexOf('|||mensajeRepecionNoContinua=') + '|||mensajeRepecionNoContinua='.length);

    var accionComun = "$('#divContLblErrorDocumento').html('');$('#panelBotonera').css('display', '');$('#txtHackControlRep').val('');";
    var textoBotonNo = null;
    var htmlMens = '';

    if (mensRepNoCont != '') {
        htmlMens = mensRepNoCont;
        textoBotonNo = categorias.aceptar;
    }
    else {
        htmlMens = mensRep;
        var accionSi = accionComun + "$('#txtHackControlRep').val('omitir');eval($('#lbPublicar').attr('onclick'));";

        htmlMens += '<input type="button" value="' + borr.si + '" onclick="' + accionSi + '" /> ';
        textoBotonNo = borr.no;
    }

    htmlMens += '<input type="button" value="' + textoBotonNo + '" onclick="' + accionComun + '" />';

    var html = '<div class="divMenRepFormSem confirmar"><div class="pregunta">' + htmlMens + '</div></div>';
    $('#divContLblErrorDocumento').html(html);
    $('#panelBotonera').css('display', 'none');
}

function CrearErrorModfRec_FormSemVirtual(data) {
    var javascript = data.substring(0, data.indexOf('|||mensaje='));
    javascript = javascript.substring('javascript='.length);
    var mensaje = data.substring(data.indexOf('|||mensaje=') + '|||mensaje='.length);

    $('#divContLblErrorDocumento').html('<div id="divKoErrorDoc" class="ko" style="display:block;"><p><span ID="lblErrorDocumento">' + mensaje + '</span></p></div>');
    eval(javascript);
}

//FIN REGION Form Sem

//INICIO REGION Base Recursos Personal

function InicializarEspacioPersonal() {
    $("#hlOrganizarCat").click(EditarCategoriasRecsBRPersonal);
    $("#hlEliminar").click(EliminarRecursosBRPersonal);
}

/**
 * Método para organizar recursos en categorías
 * @param {any} urlPeticion: Url donde se hará la petición para obtener datos en el modal
 */
function EditarCategoriasRecsBRPersonal(urlPeticion) {
    const recs = ObtenerRecursosSeleccionadosPorCheckBox("ListaRecursosCheckBox");

    if (recs == '') {
        return false;
    } else {
        // Abrir modal para proceder a la carga dinámica del mismo
        $('#modal-container').modal('toggle');
    }

    // Realizar petición para cargar los datos en el modal
    DeployActionInModalPanel(urlPeticion, this, 'modal-dinamic-content', { SelectedResources: recs });
    
    /*
    GnossPeticionAjax(urlPaginaActual + '/editcategories', { SelectedResources: recs }, true).done(function (data) {
        if ($('#panCategorias12Int').length == 0) {
            $('#btnAceptarCatsRecs').before('<div id="panCategorias12Int"></div>');
            $('#btnAceptarCatsRecs').click(AceptarCatsRecsBRPersonal);
        }

        $('#panCategorias12Int').html(data);
        $('#panContenedorCategorias').show();
    }).always(function () {
        OcultarUpdateProgress();
    });
    */
}

function AceptarCatsRecsBRPersonal() {
    var recs = ObtenerRecsSeleccionadosBR();

    if (recs == '') {
        return;
    }

    MostrarUpdateProgress();

    GnossPeticionAjax(urlPaginaActual + '/aceptcategories', { SelectedResources: recs, SelectedCategories: $('#panCategorias12Int input.hackCatTesSel').val() }, true).done(function (data) {
        $('#panContenedorCategorias').hide();
        FiltrarPorFacetas(ObtenerHash());
    }).fail(function () {
        $('#panCategorias12Int').append('<div class="ko" style="display:block;">' + data + '</div>');
    }).always(function () {
        OcultarUpdateProgress();
    });
}

/**
 * Método para guardar/aceptar la clasificación de recursos a una o varias categorías del Tesauro desde Espacio Personal
 * @param {any} urlAceptarCategorias
 */
function AceptarCatsRecsDesdeEspacioPersonal(urlAceptarCategorias) {
    // Mostrar Loading
    MostrarUpdateProgress();

    const recs = ObtenerRecursosSeleccionadosPorCheckBox("ListaRecursosCheckBox");

    if (recs == '') {
        return;
    }
    
    const categoriesSelected = $('#panDesplegableSelCat input.hackCatTesSel').val();

    GnossPeticionAjax(urlAceptarCategorias, { SelectedResources: recs, SelectedCategories: categoriesSelected }, true).done(function (data) {        
        // Cerrar contenedor 
        $('#modal-container').modal('hide');
        // Filtrar por Facetas
        FiltrarPorFacetas(ObtenerHash());
    }).fail(function (data) {
        // Mostrar aviso de error
        mostrarNotificacion("error", data);        
    }).always(function () {
        OcultarUpdateProgress();
    });
}

function EliminarRecursosBRPersonal() {
    var recs = ObtenerRecsSeleccionadosBR();

    if (recs == '') {
        return;
    }

    mostrarConfirmacionListado('panListado', textoRecursos.seguroEliminar, function () {
        EliminarRecursosBRPersonalDef(recs);
    });


}

function EliminarRecursosBRPersonalDef(recs) {
    MostrarUpdateProgress();

    GnossPeticionAjax(urlPaginaActual + '/deleteresources', { SelectedResources: recs }, true).done(function (data) {
        bool_usarMasterParaLectura = true;
        FiltrarPorFacetas(ObtenerHash());
    }).always(function () {
        OcultarUpdateProgress();
        // Cerrar el modal container
        $(function () {
            $('#modal-container').modal('toggle');
        });
    });
}

function ObtenerRecsSeleccionadosBR() {
    var recs = '';

    $('.selectorFuenteRSS input.checkbox').each(function () {
        if (this.checked) {
            recs += this.id.substring(this.id.indexOf('_') + 1) + ',';
        }
    });

    return recs;
}

/**
 * Obtener los checkbox que se han seleccionado según los checkbox seleccionados
 * @param {any} claseCSSInputCheckbox: Clase del checkbox que debe tener para comprobar si están o no checkeados y construir el parámetro
 * @param {any} pseudoCodigoCSS: Pseudocódigo para buscar input checkbox. Se usará si claseCSSInputCheckbox es vacío.
 */
function ObtenerRecursosSeleccionadosPorCheckBox(claseCSSInputCheckbox, pseudoCodigoCSS = "") {
    let recs = '';

    const claseCssOrPseudocodigo = claseCSSInputCheckbox.length == 0 ? pseudoCodigoCSS : `.${claseCSSInputCheckbox}`;

    $(claseCssOrPseudocodigo).each(function () {
        if (this.checked) {
            recs += this.id.substring(this.id.indexOf('_') + 1) + ',';
        }
    });

    return recs;
}

//FIN REGION Base Recursos Personal

//REGION Add To Gnoss

function AgregarBRAddToGnoss() {
    if ($('#txtHackCatTesSel').val() == '') {
        return;
    }

    var brsAdded = $('#txtHackCatBRAddToGnoss').val();
    var brActual = $('#cmbBaseRecursos').val();

    if (brsAdded.indexOf(brActual + '|') != -1) {
        var trozo1 = brsAdded.substring(0, brsAdded.indexOf(brActual + '|'));
        var trozo2 = brsAdded.substring(brsAdded.indexOf(brActual + '|'));
        trozo2 = trozo2.substring(trozo2.indexOf('|||') + 3);
        brsAdded = trozo1 + trozo2;
    }
    else {
        $('#listaCatBRAddToGnoss').append('<li id="li_' + brActual + '"></li>');
    }

    brsAdded += brActual + '|' + $('#txtHackCatTesSel').val() + '|||';
    $('#txtHackCatBRAddToGnoss').val(brsAdded);

    var htmlBR = '<strong>' + $('#cmbBaseRecursos')[0].selectedOptions[0].innerText + ' </strong><small>';
    var cats = $('#txtHackCatTesSel').val().split(',');

    for (var i = 0; i < cats.length - 1; i++) {
        var textoCat = $('.divTesArbol span.' + cats[i] + ' label').text();
        htmlBR += '<label class="TipoEnlace">' + textoCat + '</label>, ';
    }

    htmlBR = htmlBR.substring(0, htmlBR.length - 2);
    htmlBR += '</small><a onclick="EliminarBRAddToGnoss(\'' + brActual + '\')" class="remove"></a>';
    $('#listaCatBRAddToGnoss #li_' + brActual).html(htmlBR);
}

function CambiarBRAddToGnoss() {
    var arg = {};
    arg.ResourceSpaceID = $('#cmbBaseRecursos').val();
    MostrarUpdateProgress();

    GnossPeticionAjax(urlPaginaActual + '/getthesaurusaddtognoss', arg, true).done(function (data) {
        $('#panDesplegableSelCat').html(data);
    }).fail(function (data) {
        $('#panDesplegableSelCat').html('<div class="ko" style="display:block;"><p>' + data + '</p></div>');
    }).always(function () {
        OcultarUpdateProgress();
    });
}

function EliminarBRAddToGnoss(brActual) {
    var brsAdded = $('#txtHackCatBRAddToGnoss').val();

    if (brsAdded.indexOf(brActual + '|') != -1) {
        var trozo1 = brsAdded.substring(0, brsAdded.indexOf(brActual + '|'));
        var trozo2 = brsAdded.substring(brsAdded.indexOf(brActual + '|'));
        trozo2 = trozo2.substring(trozo2.indexOf('|||') + 3);
        brsAdded = trozo1 + trozo2;
    }

    $('#txtHackCatBRAddToGnoss').val(brsAdded);
    $('#listaCatBRAddToGnoss #li_' + brActual).remove();
}


/**
 * Clase jquery para poder gestionar acciones de EspacioPersonalGnoss.
 * Esta operativa se ejecuta cuando se accede a la sección "mis-recursos" de Gnoss.
 * 
 * */
const operativaEspacioPersonalGnoss = {
    /**
     * Acción para inicializar elementos y eventos
     */
    init: function () {
        this.config();
        this.configEvents();
    },

    /*
     * Botones y elementos de EspacioPersonal 
     * */
    config: function () {                
        // Botón para eliminar recursos
        this.btnDeleteResources = $("#deleteResources");
        // Elementos que se eliminarán cuando se pulse en la papelera
        this.resourcesSelected = "";
        this.numResourcesSelectedToDelete = "0";        
    },

    /**
     * Iniciar comportamientos de los botones para "Añadir, Ordenar, Eliminar" y comportamientos de select
     * */
    engancharComportamientos: function () {
        // Comportamientos del menú
        modalCategorizarRecursos.init();
        // Inicializar los SELECT js - select ya que son creadod dinámicamente
        $('.js-select2').select2();        
        // Inicializar categorías desplegables
        accionDesplegarCategorias.init();
        // Desplegar todas las categorías por defecto
        $('.boton-desplegar').trigger("click");
    },


    /*
     * Eventos o clicks en elementos
     * */
    configEvents: function () {
        const that = this;

        // Click realizado sobre "Borrar recurso"
        this.btnDeleteResources.on("click", function () {
            if (that.showDeleteResources() == true) {
                // Saber quién ha disparado el modal
                const triggered = $(this).attr('id')
                $('#modal-container').data('triggered', triggered);
                $('#modal-container').modal('show');
            };
        });

        // Cuando se muestre el modal container (Borrado múltiple de items, mostrar el aviso de Sí o No)
        $('#modal-container').on('show.bs.modal', function (e) {
            if ($(this).data('triggered') != undefined) {
                that.showDeleteItemsModalMessageInfo(); }
        });

        // Cuando se cierre el modal container
        $('#modal-container').on('hidden.bs.modal', function (e) {
            if (that.resourcesSelected != "undefined") {
                that.resourcesSelected = "";
                $('#modal-container').data('triggered', null);
            }
        });

        // Click para guardar una nueva categoría (Botón creado dinámicamente)
        $('#modal-container').on('click', "#guardar-nueva", function () {
            // Crear nueva categoría creada
            that.CrearCategoriaAdminEspPers();
        });

        // Click para acceder a la vista de "Renombrar categoría".
        $('#modal-container').on('click', "#renombrar-categoria", function () {
            if (!that.ComprobarSoloCatSelectAdminCatEspPersonal(false)) {
                return;
            }
            // Id de la categoría seleccionada
            const idCatSelect = $('#txtHackCatTesSel').val().split(',')[0];
            // Nombre de la categoría seleccionada            
            const nombreCatSelect = $("*").find(`[data-item=${idCatSelect}]`).siblings().text()
            // Establecer el nombre seleccionado en el input para proceder a su cambio/edición (Label e Input)
            $('#lblNombreAntiguoCat').text(nombreCatSelect);            
            $('#txtNuevoNombre').val(nombreCatSelect);
        });

        // Acción de borrado de Categoría/s.
        $('#modal-container').on('click', "#eliminar-categoria", function () {
            if (!that.ComprobarSoloCatSelectAdminCatEspPersonal(true)) {
                return;
            }            
            const arg = {};
            arg.EditAction = 6;
            arg.SelectedCategories = $('#txtHackCatTesSel').val();

            that.EnviarAccionAdminEspPers(arg);
        });



        // Click para guardar el renombrar nueva categoría (Botón creado dinámicamente)
        $('#modal-container').on('click', "#guardar-renombrar", function () {
            that.RenombrarCategoriaAdminEspPers();
        });

        // Click para guardar al mover categoría (Botón creado dinámicamente)
        $('#modal-container').on('click', "#guardar-mover", function () {
            that.MoverCategoriasAdminEspPers();
        });

        // Click para guardar al ordenar una categoría (Botón creado dinámicamente)
        $('#modal-container').on('click', "#guardar-ordenar", function () {
            that.OrdenarCategoriasAdminEspPers();
        });
        
        // Click para guardar toda la información generada
        $('#modal-container').on('click', "#btnGuardarInf", function () {
            that.GuardarTesauroAdminEspPers();
        });

        // Click para cancelar las acciones de guardar toda la información generada
        $('#modal-container').on('click', "#btnCancelarInf", function () {            
            $('#modal-container').modal('hide');
        });
        
        // Botón de Volver de "Creación Categoría" para volver al selector de categorías
        $('#modal-container').on('click', "#btnVolverAddCategory", function () {            
            $('#modal-container').attr('data-mostrar', 'categorias');
        });
    },

    /**
     * Acción para mostrar panel de "Borrar recursos"
     * */
    showDeleteResources: function () {
        const that = this;

        $.each($('input.ListaRecursosCheckBox:checkbox'), function () {
            if (this.checked) {
                that.resourcesSelected += this.id.substr(6) + ',';
                that.numResourcesSelectedToDelete++;
            }
        });

        // Mostrar popUp para indicar si se desean eliminar los mensajes
        if (this.numResourcesSelectedToDelete > 0) {
            // Mostrar el modal            
            return true
        }
        return false
    },

    /**
     * Acción que se ejecutará una vez se pulse en la papelera para eliminar múltiples items.     
     * Sólo se desea mostrar el modal si se ha llamado desde "Eliminación múltiple de mensajes".
     * */
    showDeleteItemsModalMessageInfo: function () {
        // Comprobar si se ha mostrado el modal-container seleccionando múltiples mensajes para borrar. En caso contrario, no hacer nada
        const that = this;
        let titulo = ""
        let mensaje = ""
        
        if (this.numResourcesSelectedToDelete > 1) {
            titulo = `Eliminar ${this.numResourcesSelectedToDelete} elementos seleccionados`;
            mensaje = `¿Deseas eliminar los ${this.numResourcesSelectedToDelete} elementos seleccionados?`
        } else {
            titulo = "Eliminar";
            mensaje = `¿Deseas eliminar el elemento seleccionado?`;
        }
        AccionFichaPerfil(titulo,
            'Sí',
            'No',
            mensaje,
            '',
            function () {
                that.deleteResources(that.resourcesSelected)
            },
            ''
        );

        // Restablecer num de items seleccionados
        this.numResourcesSelectedToDelete = 0;
    },

    /**
    * Acción de eliminar un único item. Se ejecuta cuando se pulse en el sí del modal.
    * @param {any} correoID: Id de los items que se desean eliminar 
    */
    deleteResources: function(resourcesID) {
        MostrarUpdateProgress();
        EliminarRecursosBRPersonalDef(resourcesID);
    },

    /**
     * Función para guardar la categoría recién creada en el servidor
     * */
    CrearCategoriaAdminEspPers: function () {                
        const arg = {};
        arg.EditAction = 0;
        arg.SelectedCategory = $('#cmbCrearCategoriaEn').val();
        arg.NewCategoryName = $('#txtNombreCatPadreCreacion').val();

        // Enviar los argumentos para crear los datos
        this.EnviarAccionAdminEspPers(arg);       
    },

    /**
     * Enviar la acción seleccionada en Editar espacio Perfil (Nueva, Ordenar, Mover, Renombrar, Eliminar categoría)
     * @param {any} arg: Objeto que contendrá los datos para realizar la acción con respecto a la categoría seleccionada
     */
    EnviarAccionAdminEspPers: function (arg) {
        const that = this;

        // Datos de input "AccionesBackup"
        arg.ActionsBackUp = $('#txtAccionesTesauroHack').val();
        
        MostrarUpdateProgress();

        GnossPeticionAjax(urlPaginaActual + '/editthesaurus', arg, true).done(function (data) {
            
            // Poner los datos nuevos devueltos en el Tesauro            
            $("#adminTesauro").html(data);
            // Reiniciar comportamientos
            that.engancharComportamientos();

            if (arg.EditAction == 0) {
                // Volver a visualizar las categorías y vaciar el input
                $('#btnVolverAddCategory').trigger("click");
                $('#txtNombreCatPadreCreacion').val('');
            }
           
            if (arg.EditAction == 9) {
                //$('#divMensOK').html('<div class="ok"><p>' + textoRecursos.guardadoOK + '</p></div>');
                // 1- Cerrar modal                
                $('#modal-container').modal('hide');
                //2- Mostrar el modal 
                setTimeout(() => {
                    mostrarNotificacion("success", textoRecursos.guardadoOK);
                }, 1500);                    
            }
            
            }).fail(function (data) {
                // Mostrar error                
                mostrarNotificacion("error", data);

            }).always(function () {
                OcultarUpdateProgress();
            });        
    },

    /**
     * Comprobar selección de categorías realizada
     * @param {any} permitirVarias
     */
    ComprobarSoloCatSelectAdminCatEspPersonal: function (permitirVarias) {               
        var catsSelect = $('#txtHackCatTesSel').val().split(',');
        if (!permitirVarias && catsSelect.length > 2) {
            // CrearErrorAdminCatEspPersonal(textoRecursos.soloUnaCat);
            // Mostrar mensaje de error con textoRecursos-soloUnaCat
            mostrarNotificacion("error", textoRecursos.soloUnaCat);
            return false;
        }
        else if (catsSelect.length == 0 || (catsSelect.length == 1 && catsSelect[0] == '')) {
            //CrearErrorAdminCatEspPersonal(textoRecursos.almenosunaCat);
            // Mostrar mensaje de error
            mostrarNotificacion("error", textoRecursos.almenosunaCat);
            return false;
        }
        return true;
    },

    /**
     * Acción de renombrar una categoría seleccionada en el Espacio personal
     * */
    RenombrarCategoriaAdminEspPers: function () {
        if (!this.ComprobarSoloCatSelectAdminCatEspPersonal(false)) {
            return;
        }

        // Creamos el objeto con los datos renombrados de la categoría
        const arg = {};
        arg.EditAction = 1;
        // Creamos el objeto con los datos renombrados de la categoría
        arg.SelectedCategory = $('#txtHackCatTesSel').val().split(',')[0];
        arg.NewCategoryName = $('#txtNuevoNombre').val();
        
        // Enviar los argumentos para crear los datos
        this.EnviarAccionAdminEspPers(arg);
    },

    /**
     * Acción para ordenar categorías de Espacio personal
     * */
    OrdenarCategoriasAdminEspPers: function () {                    
        if (!this.ComprobarSoloCatSelectAdminCatEspPersonal(true)) {
            return;
        }

        if ($('#cmbCategoriasOrdenar')[0].selectedIndex < 0) {
            return;
        }

        // Creamos el objeto con los datos renombrados de la categoría
        const arg = {};
        arg.EditAction = 5;
        arg.SelectedCategory = $('#cmbCategoriasOrdenar').val();
        arg.SelectedCategories = $('#txtHackCatTesSel').val();
        // Enviar los argumentos para crear los datos
        this.EnviarAccionAdminEspPers(arg);        
    },


    /**
     * Acción para mover categorías en Espacio personal
     * */
    MoverCategoriasAdminEspPers: function () {        
        if (!this.ComprobarSoloCatSelectAdminCatEspPersonal(true)) {
            return;
        }

        if ($('#cmbCategoriasMover')[0].selectedIndex < 0) {
            return;
        }

        // Creamos el objeto con los datos renombrados de la categoría
        const arg = {};
        arg.EditAction = 3;
        arg.SelectedCategory = $('#cmbCategoriasMover').val();
        arg.SelectedCategories = $('#txtHackCatTesSel').val();
        // Enviar los argumentos para crear los datos
        EnviarAccionAdminEspPers(arg);
    },

    /**
     * Función para guardar todos los cambios realizados en Espacio Personal
     * */
    GuardarTesauroAdminEspPers: function () {        
        // Construcción del objeto con datos para petición
        const arg = {};
        arg.EditAction = 9;        
        this.EnviarAccionAdminEspPers(arg);        
    },
};


//FIN REGION Add To Gnoss