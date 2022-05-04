// Archivo JScript para la función Añadir a Gnoss Linkedin

EjecutarFunciones();

function EjecutarFunciones() {
    var codigo = document.createElement('script'); codigo.type = 'text/javascript'; codigo.id = 'gnoss-script-addtoGnoss';
    codigo.src = 'http://static.gnoss.com/jsUnificar/jquery.min.js?v=2_1_1730';
    (document.body || document.documentElement).appendChild(codigo);

    codigo = document.createElement('script'); codigo.type = 'text/javascript'; codigo.id = 'gnoss-script-addtoGnoss2';
    codigo.src = 'http://static.gnoss.com/jsUnificar/jquery-ui-personalized-1.6rc2.min.js?v=2_1_1730';
    (document.body || document.documentElement).appendChild(codigo);

    setTimeout("pasoParametros()", 5000);
}

function pasoParametros() {
    //Creamos el formulario
    var formularioAnyadir = document.createElement("form");
    formularioAnyadir.setAttribute("method", "post");
    formularioAnyadir.setAttribute("action", urlAddTo);
    formularioAnyadir.setAttribute("name", "theForm");
    formularioAnyadir.setAttribute("target", "_self");
    document.body.appendChild(formularioAnyadir);

    //Obtener Educación
    var miEducacion = obtenerEducacionComprimida();
    insertarDatosFormulario(formularioAnyadir, "miEducacion", miEducacion);
    //Obtener Experiencia
    var miExperiencia = obtenerExperienciaComprimida();
    insertarDatosFormulario(formularioAnyadir, "miExperiencia", miExperiencia);
    //Obtener Webs
    var misWebs = obtenerWebSitesComprimida();
    insertarDatosFormulario(formularioAnyadir, "misWebs", misWebs);
    //Obtener Intereses
    var misIntereses = obtenerInteresesComprimida();
    insertarDatosFormulario(formularioAnyadir, "misIntereses", misIntereses);
    //Obtener Grupos y Asociaciones
    var misGruposAsociaciones = obtenerGruposAsociacionesComprimida();
    insertarDatosFormulario(formularioAnyadir, "misGruposAsociaciones", misGruposAsociaciones);
    //Obtener Honores y Premios
    var misHonoresPremios = obtenerHonoresPremiosComprimida();
    insertarDatosFormulario(formularioAnyadir, "misHonoresPremios", misHonoresPremios);
    //Obtener Información Personal
    var miInformacionPersonal = obtenerInformacionPersonalComprimida();
    insertarDatosFormulario(formularioAnyadir, "miInformacionPersonal", miInformacionPersonal);
    //Obtener Especialidades
    var misEspecialidades = obtenerEspecialidadesComprimida();
    insertarDatosFormulario(formularioAnyadir, "misEspecialidades", misEspecialidades);
    //Obtener Extracto
    var miExtracto = obtenerExtractoComprimida();
    insertarDatosFormulario(formularioAnyadir, "miExtracto", miExtracto);
    //Obtener Formación
    var miFormacion = obtenerFormacionComprimida();
    insertarDatosFormulario(formularioAnyadir, "miFormacion", miFormacion);
    //Obtener Idiomas
    var misIdiomas = obtenerIdiomasComprimida();
    insertarDatosFormulario(formularioAnyadir, "misIdiomas", misIdiomas);
    //Obtener Publicaciones
    var misPublicaciones = obtenerPublicacionesComprimida();
    insertarDatosFormulario(formularioAnyadir, "misPublicaciones", misPublicaciones);


    //Hacemos un submit al formulario
    var count = 0;
    for (count = 0; count < document.forms.length; count++) {
        if (document.forms[count].name == 'theForm') {
            document.forms[count].submit();
        }
    }
}

function insertarDatosFormulario(theForm, nombreMisDatos, misDatos) {
    if (misDatos != null) //Si tiene datos los pasamos por un input oculto
    {
        //Agregamos los valores a pasar a gnoss
        var inputForm = document.createElement('input');
        inputForm.setAttribute('type', 'hidden');
        inputForm.setAttribute('name', nombreMisDatos);
        inputForm.setAttribute('value', misDatos)
        var count = 0;
        for (count = 0; count < document.forms.length; count++) {
            if (document.forms[count].name == 'theForm') {
                document.forms[count].appendChild(inputForm);
            }
        }
    }
}

function EliminarHtml(pTexto) {
    if (pTexto.indexOf('<') != -1) {
        var inicio = pTexto.indexOf('<');
        var fin = pTexto.indexOf('>')

        return EliminarHtml(pTexto.substring(0, inicio) + pTexto.substring(fin + 1));
    }
    else {
        return pTexto;
    }
}

function obtenerEducacionComprimida() {
    try {
        var ArrayEducacion = new Array(); var miEducacion; var numEducacion = 0;

        var divsEducacion = $("#profile-education > div.content > div > div");

        if ($("#profile-education > div.content > div > div").length == 0) {
            divsEducacion = $("#profile-education > div.content > div");
            //$.each($("#profile-education > div.content > div").children(), function(i, v)
        }

        //Recorro los elementos del div profile-education 
        $.each(divsEducacion.children(), function (i, v) {
            var lugarEducacion = ""; var EstudioCursado = ""; var categoriaEstudioCursado = "";
            var fechaInicial = ""; var fechaFinal = ""; var Descripcion = "";

            //Obtiene dos divs [header y content]
            var elementoActual = $(this);
            var claseActual = elementoActual.attr('class');
            var etiquetaHTML = v.tagName;

            switch (etiquetaHTML) {
                case "H3":
                    miEducacion = new Array(5);
                    numEducacion = numEducacion + 1;
                    lugarEducacion = $(this).children().text();
                    miEducacion[0] = lugarEducacion;
                    ArrayEducacion[numEducacion - 1] = miEducacion;
                    break;
                case "H4":
                    $.each($(this).children(), function (i, v) {
                        switch (jQuery.trim($(this).attr('class'))) {
                            case "major":
                                var EstudioCursado = $(this).text();
                                miEducacion[1] = EstudioCursado;
                                ArrayEducacion[numEducacion - 1] = miEducacion;
                                break;
                        }
                    });
                    break;
                case "P":
                    {
                        switch (claseActual) {
                            case "period": //Es la fecha
                                fechaInicial = "";
                                fechaFinal = "";
                                $.each($(this).children(), function (i, v) {
                                    if (fechaInicial == "")
                                        fechaInicial = $(this).text();
                                    else
                                        fechaFinal = $(this).text();
                                });
                                miEducacion[2] = fechaInicial;
                                miEducacion[3] = fechaFinal;
                                ArrayEducacion[numEducacion - 1] = miEducacion;
                                break;
                            case " desc details-education": //Es la descripción
                                var DescripcionTextoSinLimpiar = $(this).text();
                                var ArrayDescripcion = DescripcionTextoSinLimpiar.split("\n");
                                for (var i = 0; i < ArrayDescripcion.length; i++) {
                                    ArrayDescripcion[i] = jQuery.trim(ArrayDescripcion[i]);
                                    if (ArrayDescripcion[i] != "")
                                        Descripcion = Descripcion + ArrayDescripcion[i] + "\n";
                                }
                                miEducacion[4] = Descripcion;
                                ArrayEducacion[numEducacion - 1] = miEducacion;
                                break;
                        }
                        break;
                    }
            }
        });

        var numEducaciones = ArrayEducacion.length;
        var EducacionActual = "";
        for (var i = 0; i < numEducaciones; i++) {
            EducacionActual = EducacionActual + i + "[/sep/]" + ArrayEducacion[i][0] + "[/sep/]";
            EducacionActual = EducacionActual + ArrayEducacion[i][1] + "[/sep/]" + ArrayEducacion[i][2] + "[/sep/]";
            EducacionActual = EducacionActual + ArrayEducacion[i][3] + "[/sep/]" + ArrayEducacion[i][4] + "[/sep/]";
            EducacionActual = EducacionActual + ArrayEducacion[i][5];

            if (i + 1 != numEducaciones) {
                EducacionActual = EducacionActual + "[/sep/]";
            }
        }
        return EducacionActual;
    } catch (error) {
        return null;
    }
}

function obtenerExperienciaComprimida() {
    try {
        var ArrayExperiencia = new Array(); var miExperiencia; var numExperiencia = 0;

        //Recorro los elementos del div profile-experience
        $.each($("#profile-experience > div.content > div").children(), function (i, v) {
            var cargo = ""; var empresa = ""; var orgstats = "";
            var period = ""; var desc = "";

            //Obtiene dos divs [header y content]
            var elementoActual = $(this);
            var claseActual = jQuery.trim(elementoActual.attr('class'));
            var etiquetaHTML = jQuery.trim(v.tagName);

            //            switch(etiquetaHTML)
            //            {
            //                case "DIV":
            //                    $.each($(this).children(), function(i, v)
            //                    {
            //                        //Ahora estamos en h3 y h4
            //                        var etiquetaHTML = v.tagName;
            //                        alert(etiquetaHTML + ' ' + v.className);
            //                        switch(etiquetaHTML)
            //                        {
            //                            case "H3":
            //                                miExperiencia = new Array(5);
            //                                numExperiencia = numExperiencia + 1;
            //                                cargo = $(this).children().children().text();
            //                                alert(cargo);
            //                                miExperiencia[0] = cargo;
            //                                ArrayExperiencia[numExperiencia - 1] = miExperiencia;
            //                                break;
            //                            case "H4":
            //                                empresa = $(this).children().children().text();
            //                                miExperiencia[1] = empresa;
            //                                ArrayExperiencia[numExperiencia - 1] = miExperiencia;
            //                                break;
            //                        }
            //                    });
            //                    break;
            //                case "P":
            //                    alert('P ' + claseActual);
            //                    switch(claseActual)
            //                    {
            //                        case "orgstats":
            //                        case "orgstats organization-details":
            //                            orgstats = jQuery.trim($(this).text());
            //                            miExperiencia[2] = orgstats;
            //                            ArrayExperiencia[numExperiencia - 1] = miExperiencia;
            //                            break;
            //                        case "period":
            //                            period = jQuery.trim($(this).text());
            //                            miExperiencia[3] = period;
            //                            ArrayExperiencia[numExperiencia - 1] = miExperiencia;
            //                            break;
            //                        case "description":                            
            //                            desc = jQuery.trim($(this).text());
            //                            miExperiencia[4] = desc;
            //                            ArrayExperiencia[numExperiencia - 1] = miExperiencia;
            //                            break;
            //                    }
            //                    break;
            //            }

            var h3s = $(this).find("h3");

            if (h3s.length > 0) {
                miExperiencia = new Array(5);
                numExperiencia = numExperiencia + 1;
                cargo = h3s.children().children().text();
                miExperiencia[0] = cargo;
                ArrayExperiencia[numExperiencia - 1] = miExperiencia;
            }

            var h4s = $(this).find("h4");

            if (h4s.length > 0) {
                empresa = h4s.children().children().text();
                miExperiencia[1] = empresa;
                ArrayExperiencia[numExperiencia - 1] = miExperiencia;
            }

            var parrafos = $(this).find("p");

            for (var j = 0; j < parrafos.length; j++) {
                if (parrafos[j].className.indexOf("orgstats") != -1 || parrafos[j].className.indexOf("orgstats organization-details") != -1) {
                    orgstats = jQuery.trim(parrafos[j].innerHTML);
                    miExperiencia[2] = orgstats;
                    ArrayExperiencia[numExperiencia - 1] = miExperiencia;
                }
                else if (parrafos[j].className.indexOf("period") != -1) {
                    period = jQuery.trim(parrafos[j].innerHTML);
                    miExperiencia[3] = period;
                    ArrayExperiencia[numExperiencia - 1] = miExperiencia;
                }
                else if (parrafos[j].className.indexOf("description") != -1) {
                    desc = jQuery.trim(parrafos[j].innerHTML);
                    miExperiencia[4] = desc;
                    ArrayExperiencia[numExperiencia - 1] = miExperiencia;
                }
            }

        });


        var numExperiencias = ArrayExperiencia.length;
        var ExperienciaActual = "";
        for (var i = 0; i < numExperiencias; i++) {
            if (i + 1 == numExperiencias) {
                ExperienciaActual = ExperienciaActual + i + "[/sep/]" + ArrayExperiencia[i][0] + "[/sep/]" + ArrayExperiencia[i][1] + "[/sep/]";
                ExperienciaActual = ExperienciaActual + ArrayExperiencia[i][2] + "[/sep/]" + ArrayExperiencia[i][3] + "[/sep/]";
                ExperienciaActual = ExperienciaActual + ArrayExperiencia[i][4];
            }
            else {
                ExperienciaActual = ExperienciaActual + i + "[/sep/]" + ArrayExperiencia[i][0] + "[/sep/]" + ArrayExperiencia[i][1] + "[/sep/]";
                ExperienciaActual = ExperienciaActual + ArrayExperiencia[i][2] + "[/sep/]" + ArrayExperiencia[i][3] + "[/sep/]";
                ExperienciaActual = ExperienciaActual + ArrayExperiencia[i][4] + "[/sep/]";
            }
        }

        return ExperienciaActual;
    } catch (error) {
        return null;
    }
}

function obtenerWebSitesComprimida() {
    try {
        var ArrayWebSites = new Array(); numWebSites = 0;
        //Recorro los elementos del div profile-additional
        $.each($("#profile-additional > div.content > dl > dd#websites > ul").children(), function (i, v) {
            ArrayWebSites[numWebSites] = jQuery.trim($(this).html());
            numWebSites = numWebSites + 1;
        });

        var misWebs = "";
        for (var i = 0; i < ArrayWebSites.length; i++) {
            if (i + 1 == ArrayWebSites.length) {
                misWebs = misWebs + jQuery.trim(ArrayWebSites[i]);
            }
            else {
                misWebs = misWebs + jQuery.trim(ArrayWebSites[i]) + "[/sep/]";
            }
        }

        return misWebs;
    } catch (error) {
        return null;
    }
}

function obtenerInteresesComprimida() {
    try {
        var ArrayIntereses = new Array(); numIntereses = 0;
        //Recorro los elementos del div profile-additional
        $.each($("#profile-additional > div.content > dl > dd#interests > p").children(), function (i, v) {
            ArrayIntereses[numIntereses] = jQuery.trim($(this).html());
            numIntereses = numIntereses + 1;
        });

        var misIntereses = "";
        for (var i = 0; i < ArrayIntereses.length; i++) {
            if (i + 1 == ArrayIntereses.length) {
                misIntereses = misIntereses + jQuery.trim(ArrayIntereses[i]);
            }
            else {
                misIntereses = misIntereses + jQuery.trim(ArrayIntereses[i]) + "[/sep/]";
            }
        }

        return misIntereses;
    } catch (error) {
        return null;
    }
}

function obtenerEspecialidadesComprimida() {
    try {
        Especialidades = $("#profile-summary > div.content > div > p").text();

        var EspecialidadesDevolver = "";

        var arrayEspecialidades = Especialidades.split(",");
        for (var i = 0; i < arrayEspecialidades.length; i++) {
            if (i + 1 == arrayEspecialidades.length) {
                EspecialidadesDevolver = EspecialidadesDevolver + jQuery.trim(arrayEspecialidades[i]);
            }
            else {
                EspecialidadesDevolver = EspecialidadesDevolver + jQuery.trim(arrayEspecialidades[i]) + "[/sep/]";
            }
        }

        return EspecialidadesDevolver;
    } catch (error) {
        return null;
    }
}

function obtenerExtractoComprimida() {
    try {
        var Extracto = jQuery.trim($("#profile-summary > div.content > p").html()).replace(/<br[\s\S]*?>/gi, '[SaltoLineaBBCode_2341234123123123]');

        return Extracto;
    } catch (error) {
        return null;
    }
}


function obtenerFormacionComprimida() {
    try {
        var ArrayFormacion = new Array(); var miFormacion; var numFormacion = 0;
        //Recorro los elementos del div profile-certifications
        $.each($("#profile-certifications > div.content > ul").children(), function (i, v) {
            var nombreCertificacion = ""; var autoridadEmisora = "";
            var inicioFormacion = ""; var finFormacion = "";

            //Obtiene dos divs [header y content]
            var elementoActual = $(this);
            var claseActual = jQuery.trim(elementoActual.attr('class'));
            var etiquetaHTML = jQuery.trim(v.tagName);
            switch (etiquetaHTML) {
                case "LI":
                    $.each($(this).children(), function (i, v) {
                        //Ahora estamos en h3 y ul
                        var etiquetaHTML = v.tagName;
                        switch (etiquetaHTML) {
                            case "H3":
                                miFormacion = new Array(4);
                                numFormacion = numFormacion + 1;
                                nombreCertificacion = $(this).text();
                                miFormacion[0] = nombreCertificacion;
                                ArrayFormacion[numFormacion - 1] = miFormacion;
                                break;
                            case "UL":
                                $.each($(this).children(), function (i, v) {
                                    switch (jQuery.trim($(this).attr('class'))) {
                                        case "fn org":
                                            autoridadEmisora = $(this).text();
                                            miFormacion[1] = autoridadEmisora;
                                            break;
                                        case "":
                                            fechaInicial = "";
                                            fechaFinal = "";
                                            $.each($(this).children(), function (i, v) {
                                                if (fechaInicial == "")
                                                    fechaInicial = $(this).text();
                                                else
                                                    fechaFinal = $(this).text();
                                            });
                                            inicioFormacion = fechaInicial;
                                            finFormacion = fechaFinal;
                                            miFormacion[2] = inicioFormacion
                                            miFormacion[3] = finFormacion
                                            break;
                                    }
                                });
                                break;
                        }
                    });
                    break;
            }
        });

        var numFormaciones = ArrayFormacion.length;
        var FormacionActual = "";
        for (var i = 0; i < numFormaciones; i++) {
            FormacionActual = FormacionActual + i + "[/sep/]" + ArrayFormacion[i][0] + "[/sep/]" + ArrayFormacion[i][1] + "[/sep/]";
            FormacionActual = FormacionActual + ArrayFormacion[i][2] + "[/sep/]" + ArrayFormacion[i][3];

            if (i + 1 != numFormaciones) {
                FormacionActual = FormacionActual + "[/sep/]";
            }
        }
        return FormacionActual;
    } catch (error) {
        return null;
    }
}


function obtenerIdiomasComprimida() {
    try {
        var ArrayIdiomas = new Array(); var misIdiomas; var numIdioma = 0;
        //Recorro los elementos del div profile-certifications
        $.each($("#profile-languages > div.content > ul").children(), function (i, v) {
            var nombreIdioma = ""; var nivelIdioma = "";

            //Obtiene dos divs [header y content]
            var elementoActual = $(this);
            var claseActual = jQuery.trim(elementoActual.attr('class'));
            var etiquetaHTML = jQuery.trim(v.tagName);
            switch (etiquetaHTML) {
                case "LI":
                    $.each($(this).children(), function (i, v) {
                        //Ahora estamos en h3 y ul
                        var etiquetaHTML = v.tagName;
                        switch (etiquetaHTML) {
                            case "H3":
                                misIdiomas = new Array(2);
                                numIdioma = numIdioma + 1;
                                nombreIdioma = $(this).text();
                                misIdiomas[0] = nombreIdioma;
                                ArrayIdiomas[numIdioma - 1] = misIdiomas;
                                break;
                            case "SPAN":
                                nivelIdioma = $(this).text();
                                misIdiomas[1] = nivelIdioma;
                                break;
                        }
                    });
                    break;
            }
        });

        var numIdiomas = ArrayIdiomas.length;
        var IdiomaActual = "";
        for (var i = 0; i < numIdiomas; i++) {
            IdiomaActual = IdiomaActual + i + "[/sep/]" + ArrayIdiomas[i][0] + "[/sep/]" + ArrayIdiomas[i][1];

            if (i + 1 != numIdiomas) {
                IdiomaActual = IdiomaActual + "[/sep/]";
            }
        }
        return IdiomaActual;
    } catch (error) {
        return null;
    }
}

function obtenerPublicacionesComprimida() {
    try {
        var ArrayPublicacion = new Array(); var misPublicaciones; var numPublicacion = 0;
        //Recorro los elementos del div profile-certifications
        $.each($("#profile-publications > div.content > ul").children(), function (i, v) {
            var tituloPublicacion = ""; var editorialPublicacion = "";
            var fechaPublicacion = ""; var autorPublicacion = "";
            var resumenPublicacion = ""; var enlacePublicacion = "";

            //Obtiene dos divs [header y content]
            var elementoActual = $(this);
            var claseActual = jQuery.trim(elementoActual.attr('class'));
            var etiquetaHTML = jQuery.trim(v.tagName);
            switch (etiquetaHTML) {
                case "LI":
                    $.each($(this).children(), function (i, v) {
                        //Ahora estamos en h3 y ul
                        var etiquetaHTML = v.tagName;
                        var claseActual = jQuery.trim($(this).attr('class'));
                        switch (etiquetaHTML) {
                            case "H3":
                                misPublicaciones = new Array(6);
                                numPublicacion = numPublicacion + 1;
                                tituloPublicacion = $(this).text();
                                misPublicaciones[0] = tituloPublicacion;
                                ArrayPublicacion[numPublicacion - 1] = misPublicaciones;
                                $.each($(this).children(), function (i, v) {
                                    var hrefActual = jQuery.trim($(this).attr('href'));
                                    if (hrefActual != "") {
                                        enlacePublicacion = hrefActual;
                                        misPublicaciones[5] = enlacePublicacion;
                                    }
                                });
                                break;
                            case "UL":
                                $.each($(this).children(), function (i, v) {
                                    switch (jQuery.trim($(this).attr('class'))) {
                                        case "fn org":
                                            editorialPublicacion = $(this).text();
                                            misPublicaciones[1] = editorialPublicacion;
                                            break;
                                        case "dtstart":
                                            fechaPublicacion = $(this).text();
                                            misPublicaciones[2] = fechaPublicacion;
                                            break;
                                    }
                                });
                                break;
                            case "DIV":
                                switch (claseActual) {
                                    case "attribution":
                                        autorPublicacion = $(this).text();
                                        misPublicaciones[3] = autorPublicacion;
                                        break;
                                    case "summary":
                                        $.each($(this).children(), function (i, v) {
                                            var etiquetaHTML = jQuery.trim(v.tagName);
                                            if (etiquetaHTML == "P") {
                                                resumenPublicacion = $(this).text();
                                                misPublicaciones[4] = resumenPublicacion;
                                            }
                                        });
                                        break;
                                }
                                break;
                        }
                    });
                    break;
            }
        });

        var numPublicaciones = ArrayPublicacion.length;
        var PublicacionActual = "";
        for (var i = 0; i < numPublicaciones; i++) {
            PublicacionActual = PublicacionActual + i + "[/sep/]" + ArrayPublicacion[i][0] + "[/sep/]" + ArrayPublicacion[i][1] + "[/sep/]";
            PublicacionActual = PublicacionActual + ArrayPublicacion[i][2] + "[/sep/]" + ArrayPublicacion[i][3] + "[/sep/]";
            PublicacionActual = PublicacionActual + ArrayPublicacion[i][4] + "[/sep/]" + ArrayPublicacion[i][5];

            if (i + 1 != numPublicaciones) {
                PublicacionActual = PublicacionActual + "[/sep/]";
            }
        }
        return PublicacionActual;
    } catch (error) {
        return null;
    }
}

function obtenerGruposAsociacionesComprimida() {
    try {
        var ArrayGruposAsociaciones = new Array(); numGruposAsociaciones = 0;
        //Recorro los elementos del div profile-additional
        $.each($("#profile-additional > div.content > dl > dd#pubgroups > p").children(), function (i, v) {
            ArrayGruposAsociaciones[numGruposAsociaciones] = jQuery.trim($(this).text());
            numGruposAsociaciones = numGruposAsociaciones + 1;
        });

        var misGruposAsociaciones = "";
        for (var i = 0; i < ArrayGruposAsociaciones.length; i++) {
            if (i + 1 == ArrayGruposAsociaciones.length) {
                misGruposAsociaciones = misGruposAsociaciones + jQuery.trim(ArrayGruposAsociaciones[i]);
            }
            else {
                misGruposAsociaciones = misGruposAsociaciones + jQuery.trim((ArrayGruposAsociaciones[i])) + "[/sep/]";
            }
        }

        return misGruposAsociaciones;
    } catch (error) {
        return null;
    }
}

function obtenerHonoresPremiosComprimida() {
    try {
        var HonoresPremios = $("#profile-additional > div.content > dl > dd.honors > p").text();
        var HonoresPremiosDevolver = "";

        var arrayHonoresPremios = HonoresPremios.split(",");
        for (var i = 0; i < arrayHonoresPremios.length; i++) {
            if (i + 1 == arrayHonoresPremios.length) {
                HonoresPremiosDevolver = HonoresPremiosDevolver + jQuery.trim((arrayHonoresPremios[i]));
            }
            else {
                HonoresPremiosDevolver = HonoresPremiosDevolver + jQuery.trim((arrayHonoresPremios[i])) + "[/sep/]";
            }
        }

        return HonoresPremiosDevolver;
    } catch (error) {
        return null;
    }
}

function obtenerInformacionPersonalComprimida() {
    try {
        var ArrayInformacionPersonal = new Array(); numInformacionPersonal = 0;

        //Recorro los elementos del div profile-experience
        $.each($("#profile-personal > div.content > dl > dd").children(), function (i, v) {
            var elementoActual = $(this);
            var claseActual = jQuery.trim(elementoActual.attr('class'));
            var etiquetaHTML = jQuery.trim(v.tagName);

            ArrayInformacionPersonal[numInformacionPersonal] = jQuery.trim($(this).text());

            numInformacionPersonal = numInformacionPersonal + 1;
        });

        var InformacionPersonal = ""
        //Mostramos la información personal
        for (var i = 0; i < ArrayInformacionPersonal.length; i++) {
            if (i + 1 == ArrayInformacionPersonal.length) {
                InformacionPersonal = InformacionPersonal + ArrayInformacionPersonal[i];
            }
            else {
                InformacionPersonal = InformacionPersonal + ArrayInformacionPersonal[i] + "[/sep/]";
            }
        }

        return InformacionPersonal;
    } catch (error) {
        return null;
    }
}