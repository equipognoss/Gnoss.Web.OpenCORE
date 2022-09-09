//Region Editar Categorias Tes Sem

function InicializarEdicionTesSem() {
    $('#lbMostarCreacionCategoria').click(DesplegarCrearTesauro);
    $('#lbMostarCambiarNombreCategoria').click(DesplegarCambiarNombreTesauro);
    $('#lbMostarMoverCategoria').click(DesplegarMoverTesauro);
    $('#lbMostarOrdenarCategoria').click(DesplegarOrdenarTesauro);
    $('#lbMostarEliminacionCategoria').click(DesplegarEliminarTesauro);
    $('#lbMostarEdicionPropExtraCategoria').click(DesplegarEditarPropExtra);
    $('#lbGuardarInf').click(GuardarTesauroEditTesSem);
    $('#lbVolver').click(VolerListTesEditTesSem);
    $(document.body).attr('onBeforeUnload', 'return ComprobarCambiosEditTesSem();');
    $('#btnOkPerderDatos').click(function () {
        $('#txtAccionesTesauroHack').val('');
        VolerListTesEditTesSem();
    });
    $('#btnNoPerderDatos').click(function () {
        $('#panInfoPerderDatos').hide();
    });

    //InicializarAccionesEditTesSem();
}

function InicializarAccionesEditTesSem() {
    $('#filtroRapido').on('keyup', function () {
        FiltarCatEditTesSem(this.value);
    });

    $('#lbCrearCategoria').click(CrearCategoriaEditTesSem);
    $('#lbCambiarNombreCategoria').click(RenombrarCategoriaEditTesSem);
    $('#lbMoverCategoria').click(MoverCategoriasEditTesSem);
    $('#lbOrdenarCategoria').click(OrdenarCategoriasEditTesSem);
    $('#lbMoverTodoTrasEliminarCategoria').click(EliminarCategoriasEditTesSem);
    $('#lbEditarPropExtra').click(EditarPropExtra);

    if ($('.propTesSemExtra').length == 0) {
        $('#liMostarEdicionPropExtraCategoria').hide();
    }
    else {
        $('#liMostarEdicionPropExtraCategoria').show();
        $('.divContEditarPropExtra').html($('.propTesSemExtra').html().replace(/ck_/g, 'cke_'));
        RecargarTodosCKEditor();
    }
}

function EditarPropExtra() {
    CrearErrorEditTesSem(null);
    var arg = {};
    arg.EditAction = 11;
    arg.SelectedCategory = $('.divContEditarPropExtra').data('stringCatId');

    if (arg.SelectedCategory == null || arg.SelectedCategory == '') {
        CrearErrorEditTesSem(controlesRapidos.errorRellCampos);
        return;
    }

    if (!ObtenerValorPropsExtra(arg, true)) {
        return;
    }

    EnviarAccionEditTesSem(arg);
}

function DesplegarEditarPropExtra() {
    if (!ComprobarSoloCatSelectEditTesSem(false)) {
        return;
    }

    //$('.divContEditarPropExtra').html($('.propTesSemExtra').html());

    var idCatSelect = $('#txtHackCatTesSel').val().split(',')[0];
    var stringCatId = $('span.' + idCatSelect).attr('rel');
    var nombreCatSelect = $('.divTesLista span.' + idCatSelect + ' label').text();

    if (nombreCatSelect.indexOf(' (') != -1) {
        nombreCatSelect = nombreCatSelect.substring(0, nombreCatSelect.lastIndexOf(' ('));
    }

    $('#pTitEditPropExtra strong').text(nombreCatSelect);
    $('.divContEditarPropExtra').data('stringCatId', stringCatId);

    DarValorCamposPropsExtraCategoria(stringCatId);

    $('#panOrdenarCategoria').slideUp();
    $('#panMoverCategoria').slideUp();
    $('#panCrearCategoria').slideUp();
    $('#panEliminarCategoria').slideUp();
    $('#panCambiarNombreCategoria').slideUp();
    $('#panEditarPropExtra').slideDown();
}

function DarValorCamposPropsExtraCategoria(categoriaID) {
    //$('.divContEditarPropExtra .txtExtraCat').val('');

    if ($('#txtHackExtraPropsTesSem').val().indexOf(categoriaID) != -1) {
        var props = $('#txtHackExtraPropsTesSem').val().substring($('#txtHackExtraPropsTesSem').val().indexOf(categoriaID));
        props = props.substring(0, props.indexOf('[|||]'));
        props = props.substring(props.indexOf('|') + 1);
        arrayProps = props.split('[||]');

        for (var i = 0; i < arrayProps.length; i++) {
            if (arrayProps[i] != '') {
                var prop = arrayProps[i].split('|')[0];
                var value = arrayProps[i].substring(arrayProps[i].indexOf('|') + 1);

                if (value.indexOf('|||') != -1 && value.indexOf('@') != -1) {
                    var idioma = value.substring(value.lastIndexOf('@') + 1);
                    idioma = idioma.replace('|||', '');
                    value = value.substring(0, value.lastIndexOf('@'));

                    if ($('.divContEditarPropExtra .txtExtraCat[prop="' + prop + '"][lang="' + idioma + '"]').length > 0) {
                        $('.divContEditarPropExtra .txtExtraCat[prop="' + prop + '"][lang="' + idioma + '"]').val(value);
                    }
                    else if ($('.divContEditarPropExtra .txtExtraCat[prop="' + prop + '"]').length > 0) {
                        $('.divContEditarPropExtra .txtExtraCat[prop="' + prop + '"]').val(value);
                    }
                }
                else {
                    $('.divContEditarPropExtra .txtExtraCat[prop="' + prop + '"]').val(value);
                }
            }
        }
    }
}

function VolerListTesEditTesSem() {
    if ($('#txtAccionesTesauroHack').length > 0 && $('#txtAccionesTesauroHack').val() != '') {
        $($('#panInfoPerderDatos fieldset p')[0]).html(form.perderdatosbio);
        $('#panInfoPerderDatos').show();
        return;
    }

    $('#panInfoPerderDatos').hide();
    $('#divEditTesSem').hide();
    $('#listTesSemEdit').show();
    $('#panAccionesTes').html('');
    CrearErrorEditTesSem(null);
}

function EditarTesSem(botonEditar, urlOnto, source) {
    if (ComprobarCambiosEditTesSem() != null) {
        if (!confirm(ComprobarCambiosEditTesSem())) {
            return false;
        }
    }

    MostrarUpdateProgress();

    $('.row').children('.panEdicion').removeClass('edit')

    var fila = botonEditar.closest('.row');
    var panEditar = fila.children('.panEdicion');
    panEditar.addClass('edit');

    $('#divEditTesSemInt').hide();
    //$('#divCargandoTesSem').show();
    $('#divEditTesSem').show();

    $('#divEditTesSem').appendTo(panEditar);

    CrearErrorEditTesSem(null);
    $('#divMensOK').html('');

    var arg = {};
    arg.OntologyUrl = urlOnto;
    arg.SourceSemanticThesaurus = source;
    arg.EditAction = 10;

    GnossPeticionAjax(urlPaginaActual + '/editthesaurus', arg, true).done(function (data) {
        $('#panAccionesTes').html(data);
        $('#divEditTesSemInt').show();
        $('#lbGuardarInf').show();
        InicializarAccionesEditTesSem();
    }).fail(function (data) {
        CrearErrorEditTesSem(data);
    }).always(function () {
        $('#divCargandoTesSem').hide();
        OcultarUpdateProgress();
    });
}

function EliminarTesSem(botonEditar, urlOnto, source) {
    if (ComprobarCambiosEditTesSem() != null) {
        if (!confirm(ComprobarCambiosEditTesSem())) {
            return false;
        }
    }

    MostrarUpdateProgress();

    $('.row').children('.panEdicion').removeClass('edit')

    var fila = botonEditar.closest('.row');

    $('#divEditTesSemInt').hide();
    $('#divCargandoTesSem').hide();
    $('#divEditTesSem').hide();

    $('#divEditTesSem').appendTo($('#divAuxTesSem'));

    CrearErrorEditTesSem(null);
    $('#divMensOK').html('');

    var arg = {
        Source: source,
        Ontologia: urlOnto
    };

    GnossPeticionAjax(urlPaginaActual + '/delete-thesaurus', arg, true).done(function (data) {
        fila.remove();
    }).fail(function (data) {
        CrearErrorEditTesSem(data);
    }).always(function () {
        OcultarUpdateProgress();
    });
}

function CrearErrorEditTesSem(error) {
    if (error == null) {
        $('#divErroTesSem').html('');
    }
    else {
        $('#divErroTesSem').html('<div class="ko" style="display:block"><p>' + error + '</p></div>');
    }
}

function EliminarCategoriasEditTesSem() {
    CrearErrorEditTesSem(null);
    if (!ComprobarSoloCatSelectEditTesSem(false)) {
        return;
    }

    if ($('#cmbMoverATrasEliminar')[0].selectedIndex < 1) {
        return;
    }

    var guidCat = $('#txtHackCatTesSel').val().split(',')[0];
    var stringCatId = $('span.' + guidCat).attr('rel');

    var arg = {};
    arg.EditAction = 7;

    if ($('#cmbMoverElementosTrasBorrar').length > 0 && $('#cmbMoverElementosTrasBorrar').val() == 'HUERFANOS') {
        arg.EditAction = 8;
    }

    arg.SelectedCategory = $('#cmbMoverATrasEliminar').val();
    arg.SelectedCategories = stringCatId;

    EnviarAccionEditTesSem(arg);
}

function OrdenarCategoriasEditTesSem() {
    CrearErrorEditTesSem(null);
    if (!ComprobarSoloCatSelectEditTesSem(true)) {
        return;
    }

    if ($('#cmbCategoriasOrdenar')[0].selectedIndex < 0) {
        return;
    }

    var arg = {};
    arg.EditAction = 5;
    arg.SelectedCategory = $('#cmbCategoriasOrdenar').val();
    arg.SelectedCategories = $('#txtHackCatTesSel').val();

    EnviarAccionEditTesSem(arg);
}

function MoverCategoriasEditTesSem() {
    CrearErrorEditTesSem(null);
    if (!ComprobarSoloCatSelectEditTesSem(false)) {
        return;
    }

    if ($('#cmbCategoriasMover')[0].selectedIndex < 0) {
        return;
    }

    var guidCat = $('#txtHackCatTesSel').val().split(',')[0];
    var stringCatId = $('span.' + guidCat).attr('rel');

    var arg = {};
    arg.EditAction = 3;
    arg.SelectedCategory = $('#cmbCategoriasMover').val();
    arg.SelectedCategories = stringCatId;

    EnviarAccionEditTesSem(arg);
}

function RenombrarCategoriaEditTesSem() {
    CrearErrorEditTesSem(null);
    if (!ComprobarSoloCatSelectEditTesSem(false)) {
        return;
    }

    var guidCat = $('#txtHackCatTesSel').val().split(',')[0];
    var stringCatId = $('span.' + guidCat).attr('rel');

    var arg = {};
    arg.EditAction = 1;
    arg.SelectedCategory = stringCatId;
    arg.NewCategoryName = ObtenerNombreCatMultiIdioTesSem('txtNuevoNombre');

    EnviarAccionEditTesSem(arg);
}

function CrearCategoriaEditTesSem() {
    CrearErrorEditTesSem(null);
    var arg = {};
    arg.EditAction = 0;
    arg.SelectedCategory = $('#cmbCrearCategoriaEn').val();
    arg.NewCategoryIdentifier = $('#txtIdentificacionCreacion').val();

    arg.NewCategoryName = ObtenerNombreCatMultiIdioTesSem('txtNombreCatPadreCreacion');

    if (arg.SelectedCategory == '' || arg.NewCategoryIdentifier == '' || arg.NewCategoryName == '') {
        CrearErrorEditTesSem(controlesRapidos.errorRellCampos);
        return;
    }

    if (!ObtenerValorPropsExtra(arg, false)) {
        return;
    }

    EnviarAccionEditTesSem(arg);
}

function ObtenerValorPropsExtra(arg, edicion) {
    if ($('.propTesSemExtra').length == 0) {
        return true;
    }

    arg.CategoryExtraPropertiesValues = '';
    var claseInputs = 'propTesSemExtra';

    if (edicion) {
        claseInputs = 'divContEditarPropExtra';
    }

    var extraOk = true;

    $('.' + claseInputs + ' .txtExtraCat').each(function () {
        $(this).parent().css('background-color', '');
        arg.CategoryExtraPropertiesValues += $(this).attr('prop') + '|' + $(this).val().trim().replace(/\n/g, '');

        if ($(this).attr('objProp') != null) {
            if (!ValidURL($(this).val())) {
                extraOk = false;
                CrearErrorEditTesSem(textoFormSem.algunCampoMalIntro);
                $(this).parent().css('background-color', 'red');
            }
        }

        if ($(this).attr('lang') != null) {
            arg.CategoryExtraPropertiesValues += '@' + $(this).attr('lang') + '|||';
        }

        arg.CategoryExtraPropertiesValues += '[||]';
    });

    arg.CategoryExtraPropertiesValues = encodeURIComponent(arg.CategoryExtraPropertiesValues);

    return extraOk;
}

function ValidURL(str) {
    if (str == '') {
        return true;
    }
    else if (str.indexOf('http://') == -1 && str.indexOf('https://') == -1) {
        return false;
    }
    else if (str.indexOf(' ') != -1 || str.indexOf('    ') != -1) {
        return false;
    }

    return true;
}

function ObtenerNombreCatMultiIdioTesSem(txtid) {
    var nombre = '';
    if ($('#' + txtid).length > 0) {
        nombre = $('#' + txtid).val();
    }
    else {
        var todosIdioOk = true;

        $('.' + txtid).each(function () {
            if ($(this).val().trim() == '') {
                todosIdioOk = false;
                return;
            }

            var idioma = this.className.replace(txtid + ' ', '');
            nombre += $(this).val() + '@' + idioma + "|||";
        });

        if (!todosIdioOk) {
            nombre = '';
        }
    }

    return nombre;
}

function EnviarAccionEditTesSem(arg) {
    arg.ActionsBackUp = encodeURIComponent($('#txtAccionesTesauroHack').val());
    arg.ExtraSemanticPropertiesValuesBK = encodeURIComponent($('#txtHackExtraPropsTesSem').val());
    arg.OntologyUrl = $('#txtHackUrlSourceTesSem').val().split('|')[0];
    arg.SourceSemanticThesaurus = $('#txtHackUrlSourceTesSem').val().split('|')[1];
    $('#divMensOK').html('');
    MostrarUpdateProgressTime(0);
    CrearErrorEditTesSem(null);

    GnossPeticionAjax(urlPaginaActual + '/editthesaurus', arg, true).done(function (data) {
        $('#filtroRapido').val('');
        $('#panAccionesTes').html(data);

        if (arg.EditAction == 2) {
            $('#PanParaSelectorCategorias input[type=checkbox]').prop('disabled', 'disabled');
            $('#panMoverCategoria').show();
        }
        else if (arg.EditAction == 4) {
            $('#PanParaSelectorCategorias input[type=checkbox]').prop('disabled', 'disabled');
            $('#panOrdenarCategoria').show();
        }
        else if (arg.EditAction == 6 && $('#txtHackCatTesSel').val() != '') {
            $('#PanParaSelectorCategorias input[type=checkbox]').prop('disabled', 'disabled');
            $('#panEliminarCategoria').show();
        }
        else {
            $('#panCambiarNombreCategoria').hide();
            $('#panCrearCategoria').hide();
            $('#panEliminarCategoria').hide();
            $('#panMoverCategoria').hide();
            $('#panOrdenarCategoria').hide();
            $('#panEditarPropExtra').hide();
        }

        if (arg.EditAction == 9) {
            $('#divMensOK').html('<div class="ok"><p>' + textoRecursos.guardadoOK + '</p></div>');
            $('#txtAccionesTesauroHack').val('');
            VolerListTesEditTesSem();
        }

        InicializarAccionesEditTesSem();
    }).fail(function (data) {
        CrearErrorEditTesSem(data);
    }).always(function () {
        OcultarUpdateProgress();
        $('.actionButtons').show();
    });
}

function GuardarTesauroEditTesSem() {
    CrearErrorEditTesSem(null);
    var arg = {};
    arg.EditAction = 9;
    $('.actionButtons').hide();
    $('#lbGuardarInf').hide();

    EnviarAccionEditTesSem(arg);
}

function ComprobarCambiosEditTesSem() {
    if ($('#txtAccionesTesauroHack').length > 0 && $('#txtAccionesTesauroHack').val() != '') {
        return form.perderdatosbio;
    }
}

function FiltarCatEditTesSem(valor) {
    $('#txtHackCatTesSel').val('');
    $('#PanParaSelectorCategorias input[type=checkbox]').prop('checked', '');

    valor = valor.toLocaleLowerCase();

    if (valor == '') {
        $('#rbArbol').trigger('click');
        $('.divTesLista div').show();
    }
    else {
        $('#rbLista').trigger('click');
        $('#PanParaSelectorCategorias .divTesLista span label').each(function () {
            if ($(this).text().toLocaleLowerCase().indexOf(valor) != -1) {
                $(this).parent().parent().show();
            }
            else {
                $(this).parent().parent().hide();
            }
        });
    }
}

function ComprobarSoloCatSelectEditTesSem(permitirVarias) {
    CrearErrorEditTesSem(null);
    var catsSelect = $('#txtHackCatTesSel').val().split(',');
    if (!permitirVarias && catsSelect.length > 2) {
        CrearErrorEditTesSem(textoRecursos.soloUnaCat);
        return false;
    }
    else if (catsSelect.length == 0 || (catsSelect.length == 1 && catsSelect[0] == '')) {
        CrearErrorEditTesSem(textoRecursos.almenosunaCat);
        return false;
    }

    return true;
}

function DesplegarCrearTesauro() {
    document.getElementById('txtHackAccionActual').value = 'CrearCategoria';
    $('#panOrdenarCategoria').slideUp();
    $('#panMoverCategoria').slideUp();
    $('#panCrearCategoria').slideDown();
    $('#panEliminarCategoria').slideUp();
    $('#panCambiarNombreCategoria').slideUp();
    $('#panEditarPropExtra').slideUp();
}

function DesplegarCambiarNombreTesauro() {
    if (!ComprobarSoloCatSelectEditTesSem(false)) {
        return;
    }

    var idCatSelect = $('#txtHackCatTesSel').val().split(',')[0];
    var nombreCatSelect = $('.divTesLista span.' + idCatSelect + ' label').text();

    if (nombreCatSelect.indexOf(' (') != -1) {
        nombreCatSelect = nombreCatSelect.substring(0, nombreCatSelect.lastIndexOf(' ('));
    }

    $('#lblNombreAntiguoCat').text(nombreCatSelect);
    $('#txtNuevoNombre').val(nombreCatSelect);

    $('#panOrdenarCategoria').slideUp();
    $('#panMoverCategoria').slideUp();
    $('#panCrearCategoria').slideUp();
    $('#panEliminarCategoria').slideUp();
    $('#panCambiarNombreCategoria').slideDown();
    $('#panEditarPropExtra').slideUp();
}

function DesplegarMoverTesauro() {
    if (!ComprobarSoloCatSelectEditTesSem(false)) {
        return;
    }

    var guidCat = $('#txtHackCatTesSel').val().split(',')[0];
    var stringCatId = $('span.' + guidCat).attr('rel');

    CrearErrorEditTesSem(null);
    var arg = {};
    arg.EditAction = 2;
    arg.SelectedCategories = stringCatId;

    EnviarAccionEditTesSem(arg);
}

function DesplegarOrdenarTesauro() {
    if (!ComprobarSoloCatSelectEditTesSem(true)) {
        return;
    }

    CrearErrorEditTesSem(null);
    var arg = {};
    arg.EditAction = 4;
    arg.SelectedCategories = $('#txtHackCatTesSel').val();

    EnviarAccionEditTesSem(arg);
}



function DesplegarEliminarTesauro() {
    if (!ComprobarSoloCatSelectEditTesSem(false)) {
        return;
    }

    var guidCat = $('#txtHackCatTesSel').val().split(',')[0];
    var stringCatId = $('span.' + guidCat).attr('rel');

    CrearErrorEditTesSem(null);
    var arg = {};
    arg.EditAction = 6;
    arg.SelectedCategories = stringCatId;

    EnviarAccionEditTesSem(arg);
}

//Fin Region Editar Categorias Tes Sem

// Region Editar Entidades Secundarias

var urlOntoEntSecActual = null;
var instanciaActual = null;

function InicializarEdicionEntSec() {
    $('#lbCrearNuevInsEntSec').click(CrearNuevInsEntSec);
    $('#lbEditarInsEntSec').click(EditarInsEntSec);
    $('#lbEliminarInsEntSec').click(EliminarInsEntSec);
    $('#lbVolverEntSec').click(VolverEntSec);
    $('#lbCrearEntSec').click(GuardarEntSec);
    $('#lbGuardarEntSec').click(GuardarEntSec);
}

function EditarEntSec(urlOnto, source) {
    $('#listEntSecEdit').hide();
    $('#divEditEntSecInt').hide();
    $('#divCargandoEntSec').show();
    $('#divEditEntSec').show();
    CrearErrorEditEntSec(null);

    var arg = {};
    arg.OntologyUrl = urlOnto;
    arg.EditAction = 0;
    
    GnossPeticionAjax(urlPaginaActual + '/editsecundaryentity', arg, true).done(function (data) {
        $('#panAccionesEntSec').html(data);
        $('#divEditEntSecInt').show();
        urlOntoEntSecActual = urlOnto;
        InicializarEdicionEntSec();
    }).fail(function (data) {
        CrearErrorEditEntSec(data);
        $('#listEntSecEdit').show();
    }).always(function () {
        $('#divCargandoEntSec').hide();
    });
}

function CrearErrorEditEntSec(error) {
    $('#divErroEntSecAux').remove();

    if (error == null) {
        $('#divErroEntSec').html('');
    }
    else {
        $('#divErroEntSec').html('<div class="ko" style="display:block"><p>' + error + '</p></div>');

        var windowTop = $(document).scrollTop();
        var windowBottom = windowTop + window.innerHeight;
        var elementPositionTop = $('#divErroEntSec').offset().top;
        var elementPositionBottom = elementPositionTop + $('#divErroEntSec').height();

        if (elementPositionTop > windowBottom || elementPositionBottom < windowTop) {
            $('#updBotonera').after('<div id="divErroEntSecAux">'+$('#divErroEntSec').html()+'</div>');
        }
    }
}

function VolverEntSec() {
    $('#divEditEntSec').hide();
    $('#listEntSecEdit').show();
}

function CrearNuevInsEntSec() {
    var arg = {};
    arg.EditAction = 1;
    instanciaActual = null;

    EnviarAccionEditEntSec(arg);
}

function EditarInsEntSec() {
    var ids = ObtenerOntoSecSelecc();

    if (ids == '')
    {
        CrearErrorEditEntSec(textoRecursos.almenosunElem);
        return;
    }
    else if (ids.split(',').length > 2)
    {
        CrearErrorEditEntSec(textoRecursos.soloUnElem);
        return;
    }

    instanciaActual = ids.split(',')[0];

    var arg = {};
    arg.EditAction = 2;
    arg.SelectedInstances = ids;

    EnviarAccionEditEntSec(arg);
}

function EliminarInsEntSec() {
    var ids = ObtenerOntoSecSelecc();

    if (ids == '') {
        CrearErrorEditEntSec(textoRecursos.almenosunElem);
        return;
    }

    var arg = {};
    arg.EditAction = 3;
    arg.SelectedInstances = ids;

    EnviarAccionEditEntSec(arg);
}

function GuardarEntSec() {
    if (!RecogerValoresRDF()) {
        CrearErrorEditEntSec(mensajeErrorFormSemPrinc);
        return;
    }

    if ($('#sujEntSec').val().trim() == '' || $('#sujEntSec').val().indexOf('/') != -1)
    {
        $('p.sujEntSec label').css('color', 'red');
        return;
    }

    $('p.sujEntSec label').css('color', '');

    var arg = {};

    if (this.id == 'lbCrearEntSec') {
        arg.EditAction = 4;
    }
    else {
        arg.EditAction = 5;
    }

    if (document.getElementById('mTxtValorRdf') != null) {
        arg.RdfValue = Encoder.htmlEncode(document.getElementById('mTxtValorRdf').value);
    }

    arg.EntitySubject = $('#sujEntSec').val();
    arg.SelectedInstances = instanciaActual + ',';

    EnviarAccionEditEntSec(arg);
}

function EnviarAccionEditEntSec(arg) {
    CrearErrorEditEntSec(null);
    arg.OntologyUrl = urlOntoEntSecActual;
    MostrarUpdateProgressTime(0);

    GnossPeticionAjax(urlPaginaActual + '/editsecundaryentity', arg, true).done(function (data) {
        $('#panAccionesEntSec').html(data);
        InicializarEdicionEntSec();
        
        if (arg.EditAction == 2) {
            var sujeto = arg.SelectedInstances.split(',')[0];
            sujeto = sujeto.substring(sujeto.lastIndexOf('/') + 1);
            $('#sujEntSec').val(sujeto);
            $('#sujEntSec').attr('disabled', 'disabled');
        }

    }).fail(function (data) {
        CrearErrorEditEntSec(data);
    }).always(function () {
        OcultarUpdateProgress();
    });
}

function ObtenerOntoSecSelecc() {
    var ids = '';

    $('input.chkEntSec').each(function () {
        if (this.checked) {
            ids += $(this).attr('rel') + ',';
        }
    });
    
    return ids;
}

// Fin Region Editar Entidades Secundarias

// Region Editar Grafos Simples

var grafoSimpGrafActual = null;

function InicializarEdicionSimpGraf() {
    $('#lbCrearNuevInsSimpGraf').click(function () {
        $('#panCrearSimpGraf').slideDown();
    });
    $('#lbEliminarInsSimpGraf').click(EliminarInsSimpGraf);
    $('#lbVolverSimpGraf').click(VolverSimpGraf);
    $('#btnCrearSimpGraf').click(CrearNuevInsSimpGraf);
}

function EditarSimpGraf(grafo) {
    $('#listSimpGrafEdit').hide();
    $('#divEditSimpGrafInt').hide();
    $('#divCargandoSimpGraf').show();
    $('#divEditSimpGraf').show();
    CrearErrorEditSimpGraf(null);

    var arg = {};
    arg.Graph = grafo;
    arg.EditAction = 0;

    GnossPeticionAjax(urlPaginaActual + '/editsimplegraph', arg, true).done(function (data) {
        $('#panAccionesSimpGraf').html(data);
        $('#divEditSimpGrafInt').show();
        grafoSimpGrafActual = grafo;
        InicializarEdicionSimpGraf();
    }).fail(function (data) {
        CrearErrorEditSimpGraf(data);
        $('#listSimpGrafEdit').show();
    }).always(function () {
        $('#divCargandoSimpGraf').hide();
    });
}

function CrearNuevInsSimpGraf() {
    $('#lbCrearSimpGraf').css('color', '');

    if ($('#txtCrearSimpGraf').val().trim() == '') {
        $('#lbCrearSimpGraf').css('color', 'red');
        return;
    }

    var arg = {};
    arg.EditAction = 1;
    arg.NewElement = $('#txtCrearSimpGraf').val();
    EnviarAccionEditSimpGraf(arg);
}

function EliminarInsSimpGraf() {
    var ids = ObtenerInstGrafSelecc();

    if (ids == '') {
        CrearErrorEditSimpGraf(textoRecursos.almenosunElem);
        return;
    }

    var arg = {};
    arg.EditAction = 2;
    arg.SelectedInstances = ids;
    EnviarAccionEditSimpGraf(arg);
}

function EnviarAccionEditSimpGraf(arg) {
    CrearErrorEditSimpGraf(null);
    arg.Graph = grafoSimpGrafActual;
    MostrarUpdateProgressTime(0);

    GnossPeticionAjax(urlPaginaActual + '/editsimplegraph', arg, true).done(function (data) {
        $('#panAccionesSimpGraf').html(data);
        InicializarEdicionSimpGraf();
    }).fail(function (data) {
        CrearErrorEditSimpGraf(data);
    }).always(function () {
        OcultarUpdateProgress();
    });
}

function VolverSimpGraf() {
    $('#divEditSimpGraf').hide();
    $('#listSimpGrafEdit').show();
}

function ObtenerInstGrafSelecc() {
    var ids = '';

    $('input.chkSimpGraf').each(function () {
        if (this.checked) {
            ids += $(this).attr('rel') + ',';
        }
    });

    return ids;
}

function CrearErrorEditSimpGraf(error) {
    $('#divErroSimpGrafAux').remove();

    if (error == null) {
        $('#divErroSimpGraf').html('');
    }
    else {
        $('#divErroSimpGraf').html('<div class="ko" style="display:block"><p>' + error + '</p></div>');

        var windowTop = $(document).scrollTop();
        var windowBottom = windowTop + window.innerHeight;
        var elementPositionTop = $('#divErroSimpGraf').offset().top;
        var elementPositionBottom = elementPositionTop + $('#divErroSimpGraf').height();

        if (elementPositionTop > windowBottom || elementPositionBottom < windowTop) {
            $('#updBotonera').after('<div id="divErroSimpGrafAux">' + $('#divErroSimpGraf').html() + '</div>');
        }
    }
}

//Fin Region Editar Grafos Simples