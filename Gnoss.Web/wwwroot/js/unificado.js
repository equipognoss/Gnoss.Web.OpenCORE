/*global.js*/
/*
..........................................................................
:: Links en ventana nueva                                               ::
..........................................................................
*/

let bool_usarMasterParaLectura = false;
// Permitir envío de Cookies a otro dominio
$(document).ready(function () {
    operativaCrossDomainCookies.init();

    // Inicializar operativa de TinyCME
    operativaTinyMceConfig.init();
    // Añadir clicks a items con la propiedad engancharclicks
    engancharClicks();
    // Búsquedas y filtrado de resultados    
    bool_usarMasterParaLectura = $('input.inpt_usarMasterParaLectura').val() == 'True';

    // Inicialización de datePickers
    initializeDatePickers();
    // Prevenir la actualización de páginas con formularios importantes
    preventLeavingFormWithoutSaving();
    // Refrescar los Waypoints para el correcto funcionamiento del "scroll infinito"
    refreshWaypoints();
    // Configurar el comportamiento del contenedor modal antes de que este se muestre
    setupModalBehavior();
    // Configurar el comportamiento del "back button" del navegador
    initializeBackButtonNavigation();
    // Configuración de Zoom de imágenes mediante el plugin zoomin
    initializeImageZooming();
    // Inicialización de reseteo de contenedor de modales
    resetModalContainer.init();
    // tooltips de bootstrap para que estén disponibles en toda la página
    $('[data-toggle="tooltip"]').tooltip();
    // Configuración de botón para FichaRecSemCms
    $('input.btnAccionSemCms').click(function () { AccionFichaRecSemCms(this); });
    // Añadir a los enlaces con la propiedad "rel=external" que se abran en una página nueva
    $("a[rel=external]").attr({target: "_blank"});
    // Inicializa el comportamiento de los tooltips de Freebase.
    initFreebase();
});

/* Evitar ocultamiento de faceta cuando se utiliza el datepicker para seleccionar meses */
$(document).on('click', '#ui-datepicker-div, .ui-datepicker-prev, .ui-datepicker-next', function (e) {
    e.stopPropagation();
});

/**
 * Inicialización de Input de tipo "File" para que coja el nombre del fichero seleccionado (Bootstrap)
 */
$(document).on('change', '.custom-file-input', function (event) {
    $(this).next('.custom-file-label').html(event.target.files[0].name);
});

/**
 * Asigna controladores de eventos click a los elementos con el atributo `clickJS`.
 * Esta función busca todos los elementos que tienen el atributo `clickJS`, asigna el contenido de este atributo como un controlador de eventos click, y luego elimina el atributo del elemento.
 * @returns {void}
 */
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

const operativaCrossDomainCookies = {
    init: function () {
        //if (location.protocol == 'https:')
        $.ajaxSetup({
            crossDomain: true,
            xhrFields: {
                withCredentials: true
            }
        });
    }
}

/**
 * Método para escapar caracteres extraños para evitar inyección de código
 * @param {*} text Texto a escapar
 * @returns Devuelve el texto limpio para evitar inyección de código, por ejemplo, en buscadores
 */
function escapeHTML(text) {
    var map = {
        '&': '&amp;',
        '<': '&lt;',
        '>': '&gt;',
        //'"': '&quot;',
        "'": '&#039;'
    };
    return text.replace(/[&<>']/g, function (m) { return map[m]; });
}

/**
 * Método que devuelve si el texto contiene caracteres HTML a escapar
 * @param {*} text Texto a analizar
 * @return {*} Indica si contiene caracteres HTML que es necesario "escapar"
 */
function containsEscapeHTML(text) {
    const specialCharacters = ['&amp;', '&lt;', '&gt;', '&quot;', '&#039;'];
    for (let i = 0; i < specialCharacters.length; i++) {
        if (text.includes(specialCharacters[i])) {
            return true;
        }
    }
    return false;
}

/**
 * Operativa para detectar comportamiento de navegación cuando se pulsa en "Back Button" del navegador
 * En ciertas páginas, se podían producir errores debido a que podían sobreescribir datos previos insertados: Ej: Creación de un recurso desde ckEditor
 * Esta operativa detecta este tipo de navegación y realiza una carga de la web
 */
const operativaDetectarNavegacionBackButton = {
    init: function () {
        this.iniciarComportamiento();
    },

    /**
     * Comprobar si se ha pulsado en "back" del navegador. Si es así, y se encuentran elementos relativos a edición de recurso, 
     * obligar a recargar la página para obtener los datos siempre actualizados y evitar posible pérdida al sobreescribirlos
     */
    iniciarComportamiento: function () {

        window.onpageshow = function (event) {
            if (event.persisted) {
                // Se ha pulsado en "back" del navegador. Comprobar si es necesario recargar la página
                // ckeditor, radioButtons, checkbox
                if ($(".cke, input[type='checkbox'],input[type='radio']").length > 1) {
                    MostrarUpdateProgress();
                    // Desactivar el plugin dirty para que no muestr el mensaje
                    // Prevenir actualización de páginas cuando haya formularios "importantes". Avisar al usuario
                    if ($("#preventLeavingFormWithoutSaving").dirty != null) {
                        $("#preventLeavingFormWithoutSaving").dirty("setAsClean");
                    }
                    window.location.reload();
                }
            }
        };
    },
};


/**
 * Clase jquery para poder hacer búsquedas desde un input para ocultar o mostrar una jerarquía de categorías
 * Ejemplo: Usado en el buscador de categorías de la página "Index"
 *
 * */
const operativaFiltroRapido = {
    /**
     * Acción para inicializar elementos y eventos
     */
    init: function () {
        this.config();
        this.configEvents();
    },
    /**
     * Configuración de los items html
     * */
    config: function () {
        this.inputClass = '.filtroRapido';
        this.input = $(this.inputClass);
        // Esperar 1 segundos a si el usuario ha dejado de escribir para iniciar búsqueda
        this.timeWaitingForUserToType = 500;
    },

    /**
     * Configuración de los eventos de los items html
     * */
    configEvents: function () {
        const that = this;
        that.isSearching = false;

        // Input donde se realizará la búsqueda
        this.input.on("keyup", function (event) {
            clearTimeout(that.timer);
            that.timer = setTimeout(function () {
                // Obtener el id de la lista para saber qué lista filtrar
                const listaSelCatArbolId = that.input.data("filterlistid");
                if (!that.isSearching) {
                    that.isSearching = true;
                    MVCFiltrarListaSelCatArbol(that.input, listaSelCatArbolId, function () {
                        // Completion cuando finalice la búsqueda
                        that.isSearching = false
                    });
                }

            }, that.timeWaitingForUserToType);
        });
    },
};


/* Administración de Comunidad */
/**
 * Clase jquery para poder gestionar la administración de una comunidad (Vistas UsuariosOrganización)
 * - Comunidades, Grupos, Usuarios y modales de acciones
 * 
 * */
const operativaUsuariosOrganizacion = {
    /**
     * Acción para inicializar elementos y eventos
     */
    init: function () {
        this.configRutas();
        this.configEvents();
    },
    /**
     * Configuración de las rutas de acciones necesarias para hacer peticiones a backend
     */
    configRutas: function () {
        // Url base
        this.urlBase = window.location.href.split("?")[0];
    },
    /*
     * Eventos o clicks en elementos
     * */
    configEvents: function () {
        const that = this;

        // Obtener el id del botón (community) que ha abierto el modal modal container ya que se cargará ahí la gestión de "Añadir usuarios a la comunidad"        
        $(document).on('show.bs.modal', '#modal-container', function (e) {
            // Key obtenido del botón que ha abierto el modal
            if ($(e.relatedTarget).data('communitykey') == undefined) {
                that.keyItem = $(e.relatedTarget).data('keyitem');
            } else {
                that.communityKey = $(e.relatedTarget).data('communitykey');
            }
        });

        // Obtener el id del botón que ha abierto el modal (Necesario para posibles acciones)        
        $(document).on('show.bs.modal', '.getExternalIdFromModal', function (e) {
            // Key obtenido del botón que ha abierto el modal
            that.keyItem = $(e.relatedTarget).data('keyitem');
        });

        // Botón Aceptar del modal Abandonar la comunidad
        $(document).on('click', '.btnAceptarAbandonarCommunity', function () {
            // Id de la comunidad de la que se desea abandonar            
            const id = $(this).data("communitykey");
            // Item de la lista de la comunidad mostrada
            const communityListItem = $(`#${id}`);

            MostrarUpdateProgress();
            GnossPeticionAjax(
                $('#urlFilter').val() + '/leave-community/' + id,
                null,
                true
            ).done(function (data) {
                // Abandonado correcto la comunidad
                // 1 - Eliminar el panel del listado de comunidades
                communityListItem.remove();
                // 2 - Cerrar modal
                $(`#modalAbandonarCommunity_${id}`).modal('hide');
                // 3 - Mostrar mensaje OK
                setTimeout(() => {
                    mostrarNotificacion('success', data);
                }, 1500)

            }).fail(function (data) {
                // 1 - Mostrar error
                mostrarNotificacion('error', "No se ha podido abandonar la comunidad, algun miembro de la organización es administrador de la comunidad.");
            }).always(function () {
                OcultarUpdateProgress();
            });
        });

        // Botón Aceptar del modal "Aceptar Activación automático"
        $(document).on('click', '.btnAceptarActivarRegAuto', function () {
            // Id de la comunidad para activación registro automático
            const id = $(this).data("communitykey");
            // Item de la lista de la comunidad mostrada
            const communityListItem = $(`#${id}`);

            MostrarUpdateProgress();
            GnossPeticionAjax(
                $('#urlFilter').val() + '/activar-regauto/' + id,
                null,
                true
            ).done(function (data) {
                // 1 - Eliminar el panel del listado de comunidades
                communityListItem.remove();
                // 2 - Cerrar modal
                $(`#modalActivarRegAuto_${id}`).modal('hide');
                // 3 - Mostrar mensaje OK
                setTimeout(() => {
                    mostrarNotificacion('success', "Registro automático configurado correctamente.");
                }, 1500)
                // 4 - Recargar la página actual
                location.reload();
            }).fail(function (data) {
                // 1 - Mostrar error
                mostrarNotificacion('error', "No se ha podido activar el registro automatico.");
            }).always(function () {
                OcultarUpdateProgress();
            });
        });

        // Botón Aceptar del modal "Desactivar registro automático"
        $(document).on('click', '.btnAceptarDesactivarRegAuto', function () {
            // Id de la comunidad para activación registro automático
            const id = $(this).data("communitykey");
            // Item de la lista de la comunidad mostrada
            const communityListItem = $(`#${id}`);

            MostrarUpdateProgress();
            GnossPeticionAjax(
                $('#urlFilter').val() + '/desactivar-regauto/' + id,
                null,
                true
            ).done(function (data) {
                // 1 - Eliminar el panel del listado de comunidades
                communityListItem.remove();
                // 2 - Cerrar modal
                $(`#modalActivarRegAuto_${id}`).modal('hide');
                // 3 - Mostrar mensaje OK
                setTimeout(() => {
                    mostrarNotificacion('success', "Se ha desactivado correctamente el registro automático.");
                }, 1500)
                // 4 - Recargar la página actual
                location.reload();
            }).fail(function (data) {
                // 1 - Mostrar error
                mostrarNotificacion('error', "No se ha podido desactivar el registro automatico.");
            }).always(function () {
                OcultarUpdateProgress();
            });
        });

        // Botón Aceptar del modal "Agregar usuarios a Comunidad"
        $(document).on('click', '.btnAceptarAccionesProyOrg', function () {
            // Cada uno de los items que aparecen (usuarios/proyectos)
            const proyectos = $("#tblAccionesUsuOrgProyOrg").find(".proyecto");
            // Objeto para mandar la petición
            let datapost = {};
            // Contador de items
            let cont = 0;
            // Prefijos y métodos dependiendo si es página de Usuarios/Comunidades
            let metodo = "";
            let prefijo = "";
            let idItem = "";

            // Esta acción se realiza tanto para Usuarios como para Proyectos -> Tenerlo en cuenta para construir la URL 
            if (that.communityKey == undefined) {
                // Se está realizando acción de usuarios
                metodo = "asign-community";
                prefijo = "ProyectosAsignados";
                idItem = that.keyItem;
            } else {
                // Se está realizando acción de proyectos/comunidades                
                metodo = "asign-users";
                prefijo = "UsuariosAsignados";
                idItem = that.communityKey;
            }

            // Revisar cada uno de los proyectos
            proyectos.each(function () {
                // Construir el prefijo                
                var mPrefijo = `${prefijo}[${cont}]`;
                // Comprobar si el nombre está checkeado
                var checked = $('.chkProyecto', $(this)).is(':checked');
                // Obtener el id del proyecto / item
                var id = $(this).data("proyecto");
                //$(this).attr('id').replace('proyParticipa_', '');
                var tipo = 1;
                // Comprobar si el nombre/item está activado
                if (checked) {
                    var inputTipoSeleccionado = $('input[type=radio][name=tipo_' + id + ']:checked', $(this))
                    if (inputTipoSeleccionado.length > 0) {
                        tipo = $('input[type=radio][name=tipo_' + id + ']:checked', $(this)).val();
                    }
                }
                // Construcción del objeto POST
                datapost[mPrefijo + '.Key'] = id;
                datapost[mPrefijo + '.Participa'] = checked;
                datapost[mPrefijo + '.TipoParticipacion'] = tipo;

                cont++;
            });

            MostrarUpdateProgress();
            // Hacer petición al servidor con los datos 
            GnossPeticionAjax(
                $('#urlFilter').val() + `/${metodo}/` + idItem,
                datapost,
                true
            ).done(function (data) {
                // 1 - Cerrar el modal container
                $(`#modal-container`).modal('hide');
                // 2- Mostrar mensaje OK
                setTimeout(() => {
                    mostrarNotificacion('success', "Acción realizada correctamente.");
                }, 1500)
            }).fail(function (data) {
                // 1 - Mostrar error
                mostrarNotificacion('error', "Se ha producido un error. Por favor, inténtalo de nuevo más tarde.");
            }).always(function () {
                OcultarUpdateProgress();
            });
        });

        // Botón Aceptar del modal "Cambiar rol de usuario"
        $(document).on('click', '.btnAceptarCambiarRolUser', function () {

            var inputSeleccionado = $('input[name=SelectorRolUsuario_' + that.keyItem + ']:checked');
            var seleccion = inputSeleccionado.attr('rel');

            var datapost = {
                rol: seleccion
            }

            MostrarUpdateProgress();
            GnossPeticionAjax(
                $('#urlFilter').val() + '/change-rol/' + that.keyItem,
                datapost,
                true
            ).done(function (data) {
                // 1 - Cerrar el modal container
                $(`.getExternalIdFromModal`).modal('hide');
                // 2- Mostrar mensaje OK
                setTimeout(() => {
                    mostrarNotificacion('success', "Cambio de rol realizado correctamente.");
                }, 1500)

                // Input modificado -> Actualizarlo 
                const tipoUsuario = $('.tipoUsuario', $('#' + that.keyItem));
                tipoUsuario.text(inputSeleccionado.attr('aux'));
                tipoUsuario.attr('rel', seleccion);
            }).fail(function (data) {
                // 1 - Mostrar error
                mostrarNotificacion('error', "Se ha producido un error al cambiar el rol del usuario. Por favor, inténtalo de nuevo más tarde.");
            }).always(function () {
                OcultarUpdateProgress();
            });
        });

        // Botón Aceptar del modal "Expulsar a usuario de la comunidad"
        $(document).on('click', '.btnAceptarExpulsarUser', function () {
            MostrarUpdateProgress();
            GnossPeticionAjax(
                $('#urlFilter').val() + '/delete/' + that.keyItem,
                null,
                true
            ).done(function (data) {
                // 1 - Cerrar el modal container
                $(`.getExternalIdFromModal`).modal('hide');
                // 2- Mostrar mensaje OK
                setTimeout(() => {
                    mostrarNotificacion('success', "El usuario ha sido eliminado de la organización.");
                }, 1500)
                // 3 - Eliminar la fila del usuario
                $(`#${that.keyItem}`).remove();

            }).fail(function (data) {
                // 1 - Mostrar error
                mostrarNotificacion('error', "Se ha producido un error al tratar de expulsar al usuario de la comunidad. Por favor, inténtalo de nuevo más tarde.");
            }).always(function () {
                OcultarUpdateProgress();
            });
        });

        // Buscador de elementos al escribir en inputFiltro
        $(document).on('keyup', '.buscador-coleccion input.inputFiltro', function () {
            that.filtrarProyectosUsu();
        });

        // Botón de lupa para inicar búsqueda de miembros, grupos, comunidades
        $(document).on('click', '#inputLupaMiembros', function (event) {
            that.filtrar();
        });

        // Input de búsqueda de miembros, grupos, comunidades (Pulsación ENTER)
        $(document).on('keypress', '#txtFiltrarMiembros', function (event) {
            var keycode = (event.keyCode ? event.keyCode : event.which);
            if (keycode == '13') {
                that.filtrar();
                event.preventDefault();
                return false;
            }
        });

        // Checkbox para mostrar o no aquellos que participan en la comunidad
        $(document).on('change', '#mostrarParticipa', function () {
            that.filtrarProyectosUsu();
        });

        // Botones paginador de resultados
        $(document).on('click', '#NavegadorPaginas_Pag a', function () {
            var numPag = $(this).attr('aux');
            $('#numPagina').val(numPag);
            const pressedButton = $(this);
            that.filtrar(pressedButton);
        });

        // Opciones para filtrar por tipo de Usuario
        $(document).on('click', '#panel-filterType a.item-dropdown', function () {
            var tipo = $(this).attr('rel');
            $('#tipoRol').val(tipo);
            that.filtrar();
        });
    },

    /**
     * Método para filtrar usuarios del proyecto. Mostrará u ocultará los elementos que coincidan con la cadena buscada
     * */
    filtrarProyectosUsu: function () {
        const inputFiltro = $('.buscador-coleccion input.inputFiltro');
        const textoFiltro = inputFiltro.val().toLowerCase().replace(/á/g, 'a').replace(/é/g, 'e').replace(/í/g, 'i').replace(/ó/g, 'o').replace(/ú/g, 'u');
        let filtrarPorTexto = false;

        if (textoFiltro.length > 0) {
            filtrarPorTexto = true;
        }

        // Mostrar o no según el valor del checkbox de "Mostrar todos los que participan en la comunidad
        const mostrarTodos = !$('#mostrarParticipa').is(':checked');

        // Cada uno de los items (proyectos/usuarios)
        const proyectos = $("#tblAccionesUsuOrgProyOrg").find(".proyecto");;

        proyectos.each(function () {
            //var textoProy = $('span.nombreProy', this).text().trim().toLowerCase().replace(/á/g, 'a').replace(/é/g, 'e').replace(/í/g, 'i').replace(/ó/g, 'o').replace(/ú/g, 'u');
            // Texto del proyecto
            const textoProy = $('.custom-control-label', this).text().trim().toLowerCase().replace(/á/g, 'a').replace(/é/g, 'e').replace(/í/g, 'i').replace(/ó/g, 'o').replace(/ú/g, 'u');
            const mostrarPorFiltro = textoProy.indexOf(textoFiltro) >= 0;
            const checked = $('.chkProyecto', $(this)).is(':checked');
            if ((mostrarTodos || checked) && (!filtrarPorTexto || mostrarPorFiltro)) {
                $(this).show();
            }
            else {
                $(this).hide();
            }
        });
    },

    /**
     * Método para filtrar resultados al pulsar en paginador. Cargará nuevos resultados en la sección de "resultados".
     * */
    filtrar: function (pressedButton) {
        const that = this;
        // Sección donde se mostrarán los resultados
        const $panelResults = $("#contentUsuariosOrganizacion");

        // Construcción del objeto para realizar la petición
        const dataPost = {
            Search: $('#txtFiltrarMiembros').val(),
            TipoRol: $('#tipoRol').val(),
            NumPage: $('#numPagina').val(),
            Type: -1 /*($('#tipoFiltro').length > 0 ? $('#tipoFiltro').val() : -1)*/,
            Order: "ASC"/*$('#ordenFiltros').val()*/
        }

        loadingMostrar();

        // Construir la url para realizar la petición
        const urlFilter = `${that.urlBase}/filter`;

        // Realizar la petición para obtención de resultados
        GnossPeticionAjax(
            //$('#urlFilter').val() + '/filter',
            urlFilter,
            dataPost,
            true
        ).done(function (data) {
            // Pintar los resultados en la sección concreta
            $panelResults.html(data);
        }).fail(function (data) {
        }).always(function () {
            loadingOcultar();
        });
    },
};

/* Fin Administración de Comunidades */


/**
 * Método para confirmar el abandono de un usuario de una comunidad
 * @param {any} url
 */
function confirmLeaveCommunity(url) {
    MostrarUpdateProgress();
    GnossPeticionAjax(
        url,
        null,
        true
    ).done(function (data) {
        // Abandono correcto de la comunidad - Redirigir a la url devuelta por el controller
        window.location.href = data;
    }).fail(function (data) {
        // Error al abandonar la comunidad
        mostrarNotificacion('error', data);
    }).always(function () {
        OcultarUpdateProgress();
        });
}

/*
..........................................................................
:: Comprobar email valido                                               ::
..........................................................................
*/

/**
 * Valida si una cadena es una dirección de correo electrónico válida.
 * 
 * @param {string} sTesteo - La cadena de texto a validar como correo electrónico.
 * @returns {boolean} - `true` si la cadena es un correo electrónico válido, `false` en caso contrario.
 */
function validarEmail(sTesteo) {    
    var reEmail = /^\w+([\.\-ñÑ+]?\w+)*@\w+([\.\-ñÑ]?\w+)*(\.\w{2,})+$/;
    return reEmail.test(sTesteo);
}

/**
 * Valida si una fecha pasada sus tres parámetros [dia, mes, año] es válida.
 * @param {number} dia - El día del mes (1-31).
 * @param {number} mes - El mes del año (1-12).
 * @param {number} anio - El año (cuatro dígitos).
 * @returns {boolean} - `true` si la fecha es válida, `false` en caso contrario.
 */
function esFecha(dia,mes,anio)
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

    var miFecha = miDia + '/' + miMes + '/' + anio;

    //Comprobamos si es un formato correcto
    var objRegExp = /^([0][1-9]|[12][0-9]|3[01])(\/|-)(0[1-9]|1[012])\2(\d{4})$/;

    if (!objRegExp.test(miFecha)) {
        return false; //Es una fecha incorrecta porque no cumple el formato
    }
    else {
        var strSeparator = miFecha.substring(2, 3);

        //Creamos el array con los parámetros de la fecha [dia,mes,año]
        var arrayDate = miFecha.split(strSeparator);
        //Array con el número de días que tiene cada mes excepto febrero que se valida aparte
        var arrayLookup = {
            '01': 31, '03': 31, '04': 30, '05': 31, '06': 30, '07': 31,
            '08': 31, '09': 30, '10': 31, '11': 30, '12': 31
        }

        var intDay = parseInt(arrayDate[0], 10);
        var intMonth = parseInt(arrayDate[1], 10);
        var intYear = parseInt(arrayDate[2], 10);

        //Comprobamos que el valor del día y del mes sean correctos
        if (intMonth != null) {
            if (intMonth != 2) {
                if (intDay <= arrayLookup[arrayDate[1]] && intDay != 0) {
                    return true;
                }
            }
        }

        //Comprobamos si es febrero y si el valor del día es correcto [Cambia si es bisiesto o no el año]
        if (intMonth == 2) {
            if (intDay > 0 && intDay < 29) {
                return true;
            }
            else if (intDay == 29) {
                if ((intYear % 4 == 0) && (intYear % 100 != 0) || (intYear % 400 == 0)) {
                    return true;
                }
            }
        }
    }

    return false; //Cualquier otro valor, falso
}

/**
 * Crea un mensaje de error en un contenedor especificado. 
 * @param {string} textoError - El texto del mensaje de error a mostrar.
 * @param {string|HTMLElement} contenedor - El contenedor donde se mostrará el mensaje de error.
 */
function crearError(textoError, contenedor) {
    crearError(textoError, contenedor, false);
}

/**
 * Crea un mensaje de error en un contenedor especificado. 
 * @param {string} textoError - El texto del mensaje de error a mostrar.
 * @param {string|HTMLElement} contenedor - El contenedor donde se mostrará el mensaje de error.
 * @param {boolean} [moverScroll=false] - Indica si se debe desplazar la vista al mensaje de error.
 */
function crearError(textoError, contenedor, moverScroll) {
    crearError(textoError, contenedor, moverScroll, false)
}

/**
 * Crea un mensaje de error en un contenedor especificado. 
 * @param {string} textoError - El texto del mensaje de error a mostrar.
 * @param {string|HTMLElement} contenedor - El contenedor donde se mostrará el mensaje de error.
 * @param {boolean} [moverScroll=false] - Indica si se debe desplazar la vista al mensaje de error.
 * @param {boolean} [positionAbsolute=false] - Indica si el mensaje de error debe tener una posición absoluta.
 */
function crearError(textoError, contenedor, moverScroll, positionAbsolute) {
	var link = '';
	
	if(moverScroll){
	    link = '<a name="MiError" style="display:block;"></a>';
	}
	
	var $c = $(contenedor);
    if ($c.find('div.ko').length) { // si ya existe el div.ko ...
        try {
            $c.find('div.ko').html(link + textoError).shakeIt();
        } catch (err) {
        }
        var $c = $(contenedor);
        if ($c.find('div.ko').length) { // si ya existe el div.ko ...
            try {
                $c.find('div.ko').html(link + textoError).shakeIt();
            } catch (err) {
            }


            if (positionAbsolute) {
                $c.find('div.ko')[0].style.position = 'absolute';
            }
        } else { //... si no lo creamos
            $('<div class="ko" style="display:none" >' + link + textoError + '</div>').prependTo($c).slideDown('fast');
            if (positionAbsolute) {
                $c.find('div.ko')[0].style.position = 'absolute';
            }
        }
        $c.find('div.ko').show();

        if (moverScroll) {
            document.location = '#MiError';
        }
    }
}

/**
 * Limpia todo el contenido HTML de un control especificado.
 * @param {string} pControl - El ID del control HTML cuyo contenido será limpiado.
 */
function LimpiarHtmlControl(pControl)
{
    if (document.getElementById(pControl) != null)
    {
        var nodosHijos=document.getElementById(pControl).childNodes 
        for(i=0;i<nodosHijos.length;i++){
            document.getElementById(pControl).removeChild(nodosHijos[i]);
        }
    }
}

/**
 * Método para convertir una determinada fecha a partir de UTC a la fecha actual según el navegador del usuario.
 */ 
function convertDateFromUTC(utcDate) {
    const fechaUTC = new Date(utcDate);
    const formatter = new Intl.DateTimeFormat("es-ES", {
        day: '2-digit',
        month: '2-digit',
        year: 'numeric',
        hour: '2-digit',
        minute: '2-digit',
        hour12: false,
        timeZone: Intl.DateTimeFormat().resolvedOptions().timeZone
    });

    return formatter.format(fechaUTC).replace(',', '');
}

/**
 * Verifica si la fecha proporcionada en un campo de entrada es válida y ajusta el formato de la fecha.
 * 
 * @param {HTMLInputElement} texbox - El campo de entrada de texto que contiene la fecha a validar.
 * @returns {boolean} - Retorna `true` si la fecha es válida, de lo contrario `false`.
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

    if ((month == 4 || month == 6 || month == 9 || month == 11) && day == 31) {
        return false;
    }

    if (month == 2) { // comprobamos el 29 de febrero
        var isleap = (year % 4 == 0 && (year % 100 != 0 || year % 400 == 0));
        if (day > 29 || (day == 29 && !isleap)) {
            return false;
        }
    }

    texbox.value = day + "/" + month + "/" + year;

    return true; // fecha es valida
}


/**
 * Compara dos fechas y corrige la fecha cambiada si la primera es mayor que la segunda.
 * 
 * @param {HTMLInputElement} fecha1 - Primer campo de entrada de texto que contiene la primera fecha a comparar.
 * @param {HTMLInputElement} fecha2 - Segundo campo de entrada de texto que contiene la segunda fecha a comparar.
 * @param {string} fechaCambiada - Indicador que define cuál de las fechas se debe ajustar ("1" para `fecha1` y otro valor para `fecha2`).
 * @returns {void} - No retorna ningún valor, ajusta `fecha1` si es necesario.
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

/*                                                             Tooltips para freebase (conFbTt)
 *---------------------------------------------------------------------------------------------
 */
var necesarioPosicionar = true;
var mouseOnTooltip = false;
var cerrar = 0;
var tooltipActivo = '';
/**
 * Inicializa el comportamiento de los tooltips de Freebase.
 */
function initFreebase() {
    ocultarFreebaseTt();
    $(".conFbTt")
        .each(function() {
            if (this.title) {
                this.tooltipData = this.title;
                this.removeAttribute('title');
            }
        })
        .hover(mostrarFreebaseTt, ocultarFreebaseTt)
        .mousemove(posicionarFreebaseTt);
}

/**
 * Obtiene el valor del hash de la URL actual.
 * 
 * @returns {string} - El valor del hash de la URL o una cadena vacía si no hay hash.
 */
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


/**
 * Codifica una cadena de texto en formato URL.
 * 
 * @param {string} s - La cadena de texto a codificar.
 * @returns {string} - La cadena codificada en formato URL.
 */
function urlEncode(s)
{
    var is_firefox = navigator.userAgent.toLowerCase().indexOf('firefox/') > -1;  
    if (is_firefox)
    {        
        return encodeURIComponent(s);
    }
    else {
        return encodeURIComponent(s);
    }
}

/**
 * Decodifica una cadena de texto en formato URL.
 * 
 * @param {string} s - La cadena de texto a decodificar.
 * @returns {string} - La cadena decodificada desde formato URL.
 */
function urlDecode(s)
{
    var is_firefox = navigator.userAgent.toLowerCase().indexOf('firefox/') > -1;  
    if (is_firefox)
    {        
        return decodeURIComponent(s);
    }
    else {
        return decodeURIComponent(s);
    }
}

/**
 * Un objeto para la codificación y decodificación de caracteres HTML y Unicode.
 * 
 * @namespace
 */
Encoder = {

    // When encoding do we convert characters into html or numerical entities
    EncodeType: "entity",  // entity OR numerical
    arr1: ['&nbsp;', '&iexcl;', '&cent;', '&pound;', '&curren;', '&yen;', '&brvbar;', '&sect;', '&uml;', '&copy;', '&ordf;', '&laquo;', '&not;', '&shy;', '&reg;', '&macr;', '&deg;', '&plusmn;', '&sup2;', '&sup3;', '&acute;', '&micro;', '&para;', '&middot;', '&cedil;', '&sup1;', '&ordm;', '&raquo;', '&frac14;', '&frac12;', '&frac34;', '&iquest;', '&Agrave;', '&Aacute;', '&Acirc;', '&Atilde;', '&Auml;', '&Aring;', '&AElig;', '&Ccedil;', '&Egrave;', '&Eacute;', '&Ecirc;', '&Euml;', '&Igrave;', '&Iacute;', '&Icirc;', '&Iuml;', '&ETH;', '&Ntilde;', '&Ograve;', '&Oacute;', '&Ocirc;', '&Otilde;', '&Ouml;', '&times;', '&Oslash;', '&Ugrave;', '&Uacute;', '&Ucirc;', '&Uuml;', '&Yacute;', '&THORN;', '&szlig;', '&agrave;', '&aacute;', '&acirc;', '&atilde;', '&auml;', '&aring;', '&aelig;', '&ccedil;', '&egrave;', '&eacute;', '&ecirc;', '&euml;', '&igrave;', '&iacute;', '&icirc;', '&iuml;', '&eth;', '&ntilde;', '&ograve;', '&oacute;', '&ocirc;', '&otilde;', '&ouml;', '&divide;', '&oslash;', '&ugrave;', '&uacute;', '&ucirc;', '&uuml;', '&yacute;', '&thorn;', '&yuml;', '&quot;', '&amp;', '&lt;', '&gt;', '&OElig;', '&oelig;', '&Scaron;', '&scaron;', '&Yuml;', '&circ;', '&tilde;', '&ensp;', '&emsp;', '&thinsp;', '&zwnj;', '&zwj;', '&lrm;', '&rlm;', '&ndash;', '&mdash;', '&lsquo;', '&rsquo;', '&sbquo;', '&ldquo;', '&rdquo;', '&bdquo;', '&dagger;', '&Dagger;', '&permil;', '&lsaquo;', '&rsaquo;', '&euro;', '&fnof;', '&Alpha;', '&Beta;', '&Gamma;', '&Delta;', '&Epsilon;', '&Zeta;', '&Eta;', '&Theta;', '&Iota;', '&Kappa;', '&Lambda;', '&Mu;', '&Nu;', '&Xi;', '&Omicron;', '&Pi;', '&Rho;', '&Sigma;', '&Tau;', '&Upsilon;', '&Phi;', '&Chi;', '&Psi;', '&Omega;', '&alpha;', '&beta;', '&gamma;', '&delta;', '&epsilon;', '&zeta;', '&eta;', '&theta;', '&iota;', '&kappa;', '&lambda;', '&mu;', '&nu;', '&xi;', '&omicron;', '&pi;', '&rho;', '&sigmaf;', '&sigma;', '&tau;', '&upsilon;', '&phi;', '&chi;', '&psi;', '&omega;', '&thetasym;', '&upsih;', '&piv;', '&bull;', '&hellip;', '&prime;', '&Prime;', '&oline;', '&frasl;', '&weierp;', '&image;', '&real;', '&trade;', '&alefsym;', '&larr;', '&uarr;', '&rarr;', '&darr;', '&harr;', '&crarr;', '&lArr;', '&uArr;', '&rArr;', '&dArr;', '&hArr;', '&forall;', '&part;', '&exist;', '&empty;', '&nabla;', '&isin;', '&notin;', '&ni;', '&prod;', '&sum;', '&minus;', '&lowast;', '&radic;', '&prop;', '&infin;', '&ang;', '&and;', '&or;', '&cap;', '&cup;', '&int;', '&there4;', '&sim;', '&cong;', '&asymp;', '&ne;', '&equiv;', '&le;', '&ge;', '&sub;', '&sup;', '&nsub;', '&sube;', '&supe;', '&oplus;', '&otimes;', '&perp;', '&sdot;', '&lceil;', '&rceil;', '&lfloor;', '&rfloor;', '&lang;', '&rang;', '&loz;', '&spades;', '&clubs;', '&hearts;', '&diams;'],
    arr2: ['&#160;', '&#161;', '&#162;', '&#163;', '&#164;', '&#165;', '&#166;', '&#167;', '&#168;', '&#169;', '&#170;', '&#171;', '&#172;', '&#173;', '&#174;', '&#175;', '&#176;', '&#177;', '&#178;', '&#179;', '&#180;', '&#181;', '&#182;', '&#183;', '&#184;', '&#185;', '&#186;', '&#187;', '&#188;', '&#189;', '&#190;', '&#191;', '&#192;', '&#193;', '&#194;', '&#195;', '&#196;', '&#197;', '&#198;', '&#199;', '&#200;', '&#201;', '&#202;', '&#203;', '&#204;', '&#205;', '&#206;', '&#207;', '&#208;', '&#209;', '&#210;', '&#211;', '&#212;', '&#213;', '&#214;', '&#215;', '&#216;', '&#217;', '&#218;', '&#219;', '&#220;', '&#221;', '&#222;', '&#223;', '&#224;', '&#225;', '&#226;', '&#227;', '&#228;', '&#229;', '&#230;', '&#231;', '&#232;', '&#233;', '&#234;', '&#235;', '&#236;', '&#237;', '&#238;', '&#239;', '&#240;', '&#241;', '&#242;', '&#243;', '&#244;', '&#245;', '&#246;', '&#247;', '&#248;', '&#249;', '&#250;', '&#251;', '&#252;', '&#253;', '&#254;', '&#255;', '&#34;', '&#38;', '&#60;', '&#62;', '&#338;', '&#339;', '&#352;', '&#353;', '&#376;', '&#710;', '&#732;', '&#8194;', '&#8195;', '&#8201;', '&#8204;', '&#8205;', '&#8206;', '&#8207;', '&#8211;', '&#8212;', '&#8216;', '&#8217;', '&#8218;', '&#8220;', '&#8221;', '&#8222;', '&#8224;', '&#8225;', '&#8240;', '&#8249;', '&#8250;', '&#8364;', '&#402;', '&#913;', '&#914;', '&#915;', '&#916;', '&#917;', '&#918;', '&#919;', '&#920;', '&#921;', '&#922;', '&#923;', '&#924;', '&#925;', '&#926;', '&#927;', '&#928;', '&#929;', '&#931;', '&#932;', '&#933;', '&#934;', '&#935;', '&#936;', '&#937;', '&#945;', '&#946;', '&#947;', '&#948;', '&#949;', '&#950;', '&#951;', '&#952;', '&#953;', '&#954;', '&#955;', '&#956;', '&#957;', '&#958;', '&#959;', '&#960;', '&#961;', '&#962;', '&#963;', '&#964;', '&#965;', '&#966;', '&#967;', '&#968;', '&#969;', '&#977;', '&#978;', '&#982;', '&#8226;', '&#8230;', '&#8242;', '&#8243;', '&#8254;', '&#8260;', '&#8472;', '&#8465;', '&#8476;', '&#8482;', '&#8501;', '&#8592;', '&#8593;', '&#8594;', '&#8595;', '&#8596;', '&#8629;', '&#8656;', '&#8657;', '&#8658;', '&#8659;', '&#8660;', '&#8704;', '&#8706;', '&#8707;', '&#8709;', '&#8711;', '&#8712;', '&#8713;', '&#8715;', '&#8719;', '&#8721;', '&#8722;', '&#8727;', '&#8730;', '&#8733;', '&#8734;', '&#8736;', '&#8743;', '&#8744;', '&#8745;', '&#8746;', '&#8747;', '&#8756;', '&#8764;', '&#8773;', '&#8776;', '&#8800;', '&#8801;', '&#8804;', '&#8805;', '&#8834;', '&#8835;', '&#8836;', '&#8838;', '&#8839;', '&#8853;', '&#8855;', '&#8869;', '&#8901;', '&#8968;', '&#8969;', '&#8970;', '&#8971;', '&#9001;', '&#9002;', '&#9674;', '&#9824;', '&#9827;', '&#9829;', '&#9830;'],

    isEmpty: function (val) {
        if (val) {
            return ((val === null) || val.length == 0 || /^\s+$/.test(val));
        } else {
            return true;
        }
    },
    // Convert HTML entities into numerical entities
    HTML2Numerical: function (s) {
        return this.swapArrayVals(s, this.arr1, this.arr2);
    },

    // Convert Numerical entities into HTML entities
    NumericalToHTML: function (s) {
        return this.swapArrayVals(s, this.arr2, this.arr1);
    },


    // Numerically encodes all unicode characters
    numEncode: function (s) {

        if (this.isEmpty(s)) return "";

        var e = "";
        for (var i = 0; i < s.length; i++) {
            var c = s.charAt(i);
            if (c < " " || c > "~") {
                c = "&#" + c.charCodeAt() + ";";
            }
            e += c;
        }
        return e;
    },

    // HTML Decode numerical and HTML entities back to original values
    htmlDecode: function (s) {

        var c, m, d = s;

        if (this.isEmpty(d)) return "";

        // convert HTML entites back to numerical entites first
        d = this.HTML2Numerical(d);

        // look for numerical entities &#34;
        arr = d.match(/&#[0-9]{1,5};/g);

        // if no matches found in string then skip
        if (arr != null) {
            for (var x = 0; x < arr.length; x++) {
                m = arr[x];
                c = m.substring(2, m.length - 1); //get numeric part which is refernce to unicode character
                // if its a valid number we can decode
                if (c >= -32768 && c <= 65535) {
                    // decode every single match within string
                    d = d.replace(m, String.fromCharCode(c));
                } else {
                    d = d.replace(m, ""); //invalid so replace with nada
                }
            }
        }

        return d;
    },

    // encode an input string into either numerical or HTML entities
    htmlEncode: function (s, dbl) {

        if (this.isEmpty(s)) return "";

        // do we allow double encoding? E.g will &amp; be turned into &amp;amp;
        dbl = dbl || false; //default to prevent double encoding

        // if allowing double encoding we do ampersands first
        if (dbl) {
            if (this.EncodeType == "numerical") {
                s = s.replace(/&/g, "&#38;");
            } else {
                s = s.replace(/&/g, "&amp;");
            }
        }

        // convert the xss chars to numerical entities ' " < >
        s = this.XSSEncode(s, false);

        if (this.EncodeType == "numerical" || !dbl) {
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
        if (!dbl) {
            s = s.replace(/&#/g, "##AMPHASH##");

            if (this.EncodeType == "numerical") {
                s = s.replace(/&/g, "&#38;");
            } else {
                s = s.replace(/&/g, "&amp;");
            }

            s = s.replace(/##AMPHASH##/g, "&#");
        }

        // replace any malformed entities
        s = s.replace(/&#\d*([^\d;]|$)/g, "$1");

        if (!dbl) {
            // safety check to correct any double encoded &amp;
            s = this.correctEncoding(s);
        }

        // now do we need to convert our numerical encoded string into entities
        if (this.EncodeType == "entity") {
            s = this.NumericalToHTML(s);
        }

        return s;
    },

    // Encodes the basic 4 characters used to malform HTML in XSS hacks
    XSSEncode: function (s, en) {
        if (!this.isEmpty(s)) {
            en = en || true;
            // do we convert to numerical or html entity?
            if (en) {
                s = s.replace(/\'/g, "&#39;"); //no HTML equivalent as &apos is not cross browser supported
                s = s.replace(/\"/g, "&quot;");
                s = s.replace(/</g, "&lt;");
                s = s.replace(/>/g, "&gt;");
            } else {
                s = s.replace(/\'/g, "&#39;"); //no HTML equivalent as &apos is not cross browser supported
                s = s.replace(/\"/g, "&#34;");
                s = s.replace(/</g, "&#60;");
                s = s.replace(/>/g, "&#62;");
            }
            return s;
        } else {
            return "";
        }
    },

    // returns true if a string contains html or numerical encoded entities
    hasEncoded: function (s) {
        if (/&#[0-9]{1,5};/g.test(s)) {
            return true;
        } else if (/&[A-Z]{2,6};/gi.test(s)) {
            return true;
        } else {
            return false;
        }
    },

    // will remove any unicode characters
    stripUnicode: function (s) {
        return s.replace(/[^\x20-\x7E]/g, "");

    },

    // corrects any double encoded &amp; entities e.g &amp;amp;
    correctEncoding: function (s) {
        return s.replace(/(&amp;)(amp;)+/, "$1");
    },


    // Function to loop through an array swaping each item with the value from another array e.g swap HTML entities with Numericals
    swapArrayVals: function (s, arr1, arr2) {
        if (this.isEmpty(s)) return "";
        var re;
        if (arr1 && arr2) {
            //ShowDebug("in swapArrayVals arr1.length = " + arr1.length + " arr2.length = " + arr2.length)
            // array lengths must match
            if (arr1.length == arr2.length) {
                for (var x = 0, i = arr1.length; x < i; x++) {
                    re = new RegExp(arr1[x], 'g');
                    s = s.replace(re, arr2[x]); //swap arr1 item with matching item from arr2	
                }
            }
        }
        return s;
    },

    inArray: function (item, arr) {
        for (var i = 0, x = arr.length; i < x; i++) {
            if (arr[i] === item) {
                return i;
            }
        }
        return -1;
    }

}



/**
 * Agrega o elimina una clave a una lista en un campo de texto basado en el estado de un checkbox.
 * 
 * @param {HTMLElement} pCheck - El elemento checkbox que determina si agregar o eliminar la clave.
 * @param {string} pIdTxt - El ID del campo de texto donde se agregará o eliminará la clave.
 * @param {string} pClave - La clave que se agregará o eliminará de la lista en el campo de texto.
 */
function marcarElementoTreeView(pCheck, pIdTxt, pClave) {
    mTxt = $('#' + pIdTxt);

    if ($(pCheck).is(':checked')) {
        mTxt.val(mTxt.val() + pClave + ',');
    }
    else {
        mTxt.val(mTxt.val().replace(pClave + ',', ''));
    }
}

/**
 * Marca o desmarca un elemento en dos listas diferentes y actualiza el estado de los checkboxes en dos secciones del DOM.
 * 
 * @param {HTMLElement} pCheck - El elemento checkbox que determina si se debe marcar o desmarcar el elemento.
 * @param {string} pIdTxt - El ID del campo de texto que contiene una lista de claves separadas por comas.
 * @param {string} pClave - La clave que se marcará o desmarcará en las dos secciones del DOM.
 */
function marcarElementoSelCat(pCheck,pIdTxt,pClave){
    marcarElementoTreeView(pCheck, pIdTxt, pClave);
    try {
        $('#divSelCatLista').find('span.' + pClave + ' input').attr('checked', $(pCheck).is(':checked'));
        $('#divSelCatTesauro').find('span.' + pClave + ' input').attr('checked', $(pCheck).is(':checked'));
    }
    catch (Exception) {
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
				
/**/
/*ejemplosUso.js*/
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

//                                                                  registro de organizaciones
//--------------------------------------------------------------------------------------------

/**
 * Validar el campo población introducido para el proceso de registro.
 * Si es correcto, se añade una clase "is-valid"
 * Si es incorrecto (vacío) se añade una clase "is-invalid"
 * @param {any} pTxtPobla: Input de la población
 * @param {any} pLblPobla: Label asociado al input población
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

function validarNombreCortoComunidad(nombre) {
    //var RegExPatternNombreCorto = /^([a-zA-Z0-9ñÑüÜáéíóúÁÉÍÓÚàèìòùÀÈÌÒÙâêîôûÂÊÎÔÛ'´`çÇ-]{4,30})$/;
    // No aceptar guiones cortos ni medios para el nombre corto de la comunidad
    //var RegExPatternNombreCorto = /^([a-zA-Z0-9ñÑ'´`çÇ]{4,30})$/;
    // let RegExPatternNombreCorto = /[^a-zA-Z0-9 _ -]/g; // /^([^a-zA-Z0-9]{4,30})$/;    
    return (nombre != '' && nombre.length > 4 && nombre.length < 30);
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

function AgregarErrorReg(pErrores, pError) {
    if (pErrores.indexOf(pError) != -1)//El error ya está, no hay que agregarlo.
    {
        return '';
    }
    else {
        return pError;
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
 * Validar la contraseña del lusuario en el proceso de registro.
 * Si es correcto, se añade una clase "is-valid"
 * Si es incorrecto (vacío) se añade una clase "is-invalid"
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
 * Si es correcto, se añade una clase "is-valid"
 * Si es incorrecto (vacío) se añade una clase "is-invalid"
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
 * Si es correcto, se añade una clase "is-valid"
 * Si es incorrecto (vacío) se añade una clase "is-invalid"
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
 * Si es correcto, se añade una clase "is-valid"
 * Si es incorrecto (vacío) se añade una clase "is-invalid"
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
 * @param {bool} jqueryElement: Se indica si el elemento pasado está ya en formato jquery, por lo que no hace falta analizarlo por ID
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

/**
 * Acción que se ejecuta cuando se pulsa sobre las acciones disponibles de un item/recurso de tipo "Perfil" encontrado por el buscador.
 * Las acciones que se podrán realizar son (No/Enviar newsletter, No/Bloquear). Acciones también de vincular, desvincular recurso...
 * @param {string} titulo: Título que tendrá el panel modal
 * @param {any} textoBotonPrimario: Texto del botón primario
 * @param {any} textoBotonSecundario: Texto del botón primario
 * @param {string} texto: El texto o mensaje a modo de título que se mostrará para que el usuario sepa la acción que se va a realizar
 * @param {string} id: Identificador del recurso/persona sobre el que se aplicará la acción
 * @param {any} accion: Acción o función que se ejecutará cuando se pulse en el botón de primario
 * @param {any} idModalPanel: Panel modal contenedor donde se insertará este HTML (Por defecto será #modal-container)
 */
function AccionFichaPerfil(titulo, textoBotonPrimario, textoBotonSecundario, texto, id, accion, textoInferior = null, idModalPanel = "#modal-container") {

    // Panel dinámico del modal padre donde se insertará la vista "hija"
    const $modalDinamicContentPanel = $('#modal-container').find('#modal-dinamic-content #content');

    // Plantilla del panel html que se cargará en el modal contenedor al pulsar en la acción
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
    // Panel de botones para la acción
    plantillaPanelHtml += '<div id="modal-dinamic-action-buttons" class="form-actions">'
    plantillaPanelHtml += '<button data-dismiss="modal" class="btn btn-primary">' + textoBotonSecundario + '</button>'
    plantillaPanelHtml += '<button class="btn btn-outline-primary ml-1">' + textoBotonPrimario + '</button>'
    plantillaPanelHtml += '</div>';
    plantillaPanelHtml += '</div>';
    plantillaPanelHtml += '</div>';

    // Meter el código de la vista modal en el contenedor padre que viene identificado por el id #modal-container
    // En concreto, hay que buscar la etiqueta modal-dinamic-content #content e insertar el código
    $modalDinamicContentPanel.html(plantillaPanelHtml);

    // Acceso a los botones
    const botones = $modalDinamicContentPanel.find('#modal-dinamic-action-buttons > button');

    // Asignación de la función al botón "Sí" o de acción
    $(botones[1]).on("click", function () {
        // Ocultar el panel modal de bootstrap - De momento estará visible. Se ocultará si se muestra mensaje de OK pasados 1.5 segundos
        //$('#modal-container').modal('hide');
    }).click(accion);
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

/**
 * Cambia una fecha en formato dd/mm/yyyy a yyyymmdd.
 *
 * @param {string} fecha - La fecha en formato dd/mm/yyyy.
 * @returns {string} - La fecha en formato yyyymmdd.
 */
function cambiarFormatoFecha(fecha) {
    //Cambia una fecha en formato 01/02/2011 a 20110201
    var cachos;
    cachos = fecha.split('/');
    return cachos[2] + cachos[1] + cachos[0];
}

/*--------    REGION BUSCADOR FACETADO    -----------------------------------------------------------*/

/**
 * Obtiene el fragmento hash de la URL actual.
 *
 * @returns {string} - El valor del hash de la URL actual, o una cadena vacía si no hay hash.
 */
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

/**
 * Obtiene un filtro de rango basado en los valores de dos controles de entrada.
 *
 * @param {HTMLElement} control1 - El primer control de entrada.
 * @param {string} textoInicioCtrl1 - El valor de texto inicial para el primer control.
 * @param {HTMLElement} control2 - El segundo control de entrada.
 * @param {string} textoInicioCtrl2 - El valor de texto inicial para el segundo control.
 * @param {boolean} esFiltroFechas - Indica si los valores son fechas y deben formatearse.
 * @returns {string} - Una cadena que representa el rango, en el formato 'valor1-valor2'.
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
 * Configura el control de autocompletar para un campo de entrada.
 *
 * @param {HTMLElement} control - El control de entrada al que se aplicará el autocompletar.
 * @param {string} pClaveFaceta - La clave de faceta para la autocompletar.
 * @param {number} pOrden - El orden para la autocompletar.
 * @param {string} pParametros - Parámetros adicionales para la autocompletar.
 */
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


/**
 * Configura el control de autocompletar para un campo de entrada de usuario.
 *
 * @param {HTMLElement} control - El control de entrada al que se aplicará el autocompletar.
 * @param {string} pClaveFaceta - La clave de faceta para el autocompletar.
 * @param {number} pOrden - El orden para el autocompletar.
 * @param {string} pParametros - Parámetros adicionales para el autocompletar.
 * @param {string} pGrafo - El identificador del proyecto o grafo.
 */
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

/**
 * Configura el control de autocompletar para un campo de entrada con filtro de contexto.
 *
 * @param {HTMLElement} control - El control de entrada al que se aplicará el autocompletar.
 * @param {string} pClaveFaceta - La clave de faceta para el autocompletar.
 * @param {number} pOrden - El orden para el autocompletar.
 * @param {string} pParametros - Parámetros adicionales para el autocompletar.
 * @param {string} pFiltroContexto - Filtro de contexto adicional.
 */
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

/**
 * Configura el control de autocompletar para etiquetas con tipado.
 *
 * @param {HTMLElement} control - El control de entrada al que se aplicará el autocompletar.
 * @param {string} pClaveFaceta - La clave de faceta para el autocompletar.
 * @param {boolean} pIncluirIdioma - Indica si se debe incluir el idioma en los parámetros adicionales.
 */
function autocompletarEtiquetasTipado(control, pClaveFaceta, pIncluirIdioma) {
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
                pTipoAutocompletar: tipoAutocompletar,
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

/**
 * Configura el control de autocompletar para la selección de entidades.
 *
 * @param {HTMLElement} control - El control de entrada al que se aplicará el autocompletar.
 * @param {string} grafo - El identificador del grafo.
 * @param {string} entContenedora - La entidad contenedora.
 * @param {string} propiedad - La propiedad de la entidad.
 * @param {string} tipoEntidadSolicitada - El tipo de entidad solicitada.
 * @param {string} propSolicitadas - Las propiedades solicitadas.
 * @param {string} extraWhere - Condiciones adicionales para el autocompletar.
 * @param {string} idioma - El idioma para la solicitud.
 */
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
                identidad: $('input.inpt_identidadID').val(),
                pProyectoID: $('.inpt_proyID').val(),
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


/**
 * Configura el control de autocompletar para la selección de entidades y grupos en Gnoss.
 *
 * @param {HTMLElement} control - El control de entrada al que se aplicará el autocompletar.
 * @param {string} pIdentidad - El identificador de la identidad del usuario.
 * @param {string} pOrganizacion - El identificador de la organización del usuario.
 * @param {string} pProyecto - El identificador del proyecto en el que se realiza la búsqueda.
 * @param {string} pTipoSolicitud - El tipo de solicitud que se está realizando.
 */
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


/**
 * Configura el control de autocompletar para una entidad dependiente en un grafo específico.
 *
 * @param {HTMLElement} control - El control de entrada al que se aplicará el autocompletar.
 * @param {string} pEntidad - El identificador de la entidad principal para la cual se está configurando el autocompletar.
 * @param {string} pPropiedad - El identificador de la propiedad de la entidad principal.
 * @param {string} grafo - El identificador del grafo donde se realizará la búsqueda.
 * @param {string} tipoEntDep - El tipo de entidad dependiente que se está buscando.
 * @param {string} propDepende - La propiedad de la entidad dependiente que se está buscando.
 * @param {string} entPropDepende - La entidad y propiedad en las que se basa la dependencia.
 */
function autocompletarGrafoDependiente(control, pEntidad, pPropiedad, grafo, tipoEntDep, propDepende, entPropDepende) {
    EliminarValoresGrafoDependientes(pEntidad, pPropiedad, false, true);

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
            //servicio: new WS($('input.inpt_urlServicioAutocompletar').val(), WSDataType.jsonp),
            //metodo: 'AutoCompletarGrafoDependienteDocSem',
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

/**
 * Elimina los valores de un campo en un grafo dependiente y opcionalmente deshabilita el campo.
 * También puede eliminar valores de campos dependientes recursivamente.
 *
 * @param {string} pEntidad - El identificador de la entidad cuyo campo se está modificando.
 * @param {string} pPropiedad - El identificador de la propiedad del campo que se está modificando.
 * @param {boolean} pDeshabilitar - Indica si el campo debe ser deshabilitado después de eliminar su valor.
 * @param {boolean} pRecursivo - Indica si la eliminación de valores y la desactivación deben realizarse de manera recursiva en campos dependientes.
 */
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


/**
 * Habilita un campo de autocompletar dependiente basado en una entidad y una propiedad específicas.
 *
 * Esta función obtiene las propiedades dependientes de una entidad y propiedad dadas, y habilita el campo de autocompletar asociado si existe.
 *
 * @param {string} pEntidad - El identificador de la entidad para la cual se están habilitando los campos dependientes.
 * @param {string} pPropiedad - El identificador de la propiedad para la cual se están habilitando los campos dependientes.
 */
function HabilitarCamposGrafoDependientes(pEntidad, pPropiedad) {
    var pronEntDep = GetPropiedadesDependientes(pEntidad, pPropiedad);

    if (pronEntDep != null) {
        var idControlAuto = ObtenerControlEntidadProp(pronEntDep[1] + ',' + pronEntDep[0], TxtRegistroIDs).replace("Campo_", "hack_");
        document.getElementById(idControlAuto).disabled = false;
    }
}


/**
 * Crea un campo de autocompletar para etiquetas con soporte para múltiples selecciones.
 *
 * Esta función inicializa un campo de autocompletar en el control especificado, utilizando una URL de servicio para obtener las etiquetas. El campo soporta múltiples selecciones y está configurado con parámetros específicos relacionados con el proyecto, la identidad, y el estado del usuario.
 *
 * @param {HTMLElement} control - El control HTML en el que se va a configurar el autocompletar para etiquetas. Este parámetro debe ser un elemento de entrada en el cual se aplicará la funcionalidad de autocompletar.
 * @param {string} pUrlAutocompletar - La URL del servicio de autocompletar que proporciona las etiquetas. Esta URL debe apuntar a un endpoint que maneje solicitudes de autocompletar.
 * @param {string} pProyectoID - El ID del proyecto para el cual se están obteniendo las etiquetas. Este parámetro se utiliza para filtrar las etiquetas relacionadas con un proyecto específico.
 * @param {boolean} pEsMyGnoss - Valor booleano que indica si el contexto es MyGnoss. Se utiliza para ajustar el comportamiento del autocompletar en función de si el entorno es MyGnoss o no.
 * @param {boolean} pEstaEnProyecto - Valor booleano que indica si el usuario está actualmente en un proyecto. Este parámetro se utiliza para ajustar el comportamiento del autocompletar según el estado del usuario en el proyecto.
 * @param {boolean} pEsUsuarioInvitado - Valor booleano que indica si el usuario es un usuario invitado. Este parámetro se utiliza para ajustar el comportamiento del autocompletar en función de si el usuario tiene un rol de invitado.
 * @param {string} pIdentidadID - El ID de identidad del usuario para el cual se están obteniendo las etiquetas. Este parámetro se utiliza para asociar las etiquetas con una identidad específica.
 */
function CrearAutocompletarTags(control, pUrlAutocompletar, pProyectoID, pEsMyGnoss, pEstaEnProyecto, pEsUsuarioInvitado, pIdentidadID) {
    $(control).autocomplete(
        null,
        {
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

/**
 * Realiza un etiquetado automático de recursos basado en el título y la descripción del recurso.
 * Si la descripción es demasiado larga, delega la tarea en una función que maneja el etiquetado en varias solicitudes.
 *
 * @param {string} titulo - El título del recurso que se va a etiquetar. Este parámetro es la base para generar las etiquetas.
 * @param {string} descripcion - La descripción del recurso que se va a etiquetar. Este texto se usa para generar etiquetas adicionales.
 * @param {string} txtHack - El ID del campo de entrada donde se deben colocar las etiquetas generadas. Este campo se actualiza con las nuevas etiquetas.
 * @param {boolean} pEsPaginaEdicion - Valor booleano que indica si el contexto es una página de edición. Este parámetro ajusta el comportamiento del etiquetado para una página de edición o una vista general.
 */
function EtiquetadoAutomaticoDeRecursos(titulo, descripcion, txtHack, pEsPaginaEdicion) {
    var params = {};
    params['ProyectoID'] = $('input.inpt_proyID').val();
    params['documentoID'] = documentoID;
    var ext = $('#txtHackEnlaceDoc').val();
    if (ext != undefined) {
        ext = ext.split(".").pop();
        ext = "." + ext;
        params['extension'] = ext;
    }
    var numMax = 15000;
    titulo = urlEncode(titulo);
    descripcion = urlEncode(descripcion);
    if (descripcion.length < numMax) {
        var metodo = 'SeleccionarEtiquetas';
        params['titulo'] = titulo;
        params['descripcion'] = descripcion;

        $.post(obtenerUrl($('input.inpt_urlEtiquetadoAutomatico').val()) + "/" + metodo, params, function (data) {
            procesarTags(data, txtHack, pEsPaginaEdicion);
        });
    } else {
        guid = guidGenerator().toLowerCase();
        params['identificadorPeticion'] = guid;
        EtiquetadoAutomaticoDeRecursosMultiple(titulo, descripcion, numMax, params, txtHack, pEsPaginaEdicion);
    }
}

/**
 * Realiza el etiquetado automático de recursos en múltiples solicitudes si la descripción es demasiado larga para procesarla en una sola petición.
 * Divide la descripción en partes y envía cada parte al servicio web para el etiquetado.
 *
 * @param {string} titulo - El título del recurso que se está etiquetando. Este parámetro se usa para la generación de etiquetas.
 * @param {string} descripcion - La descripción del recurso que se está etiquetando. Este texto se divide en partes si es demasiado largo.
 * @param {number} numMax - El número máximo de caracteres para procesar en una sola solicitud.
 * @param {Object} params - Los parámetros para la solicitud al servicio de etiquetado automático. Estos parámetros incluyen detalles sobre el recurso y la petición.
 * @param {string} txtHack - El ID del campo de entrada donde se deben colocar las etiquetas generadas. Este campo se actualiza con las nuevas etiquetas.
 * @param {boolean} pEsPaginaEdicion - Valor booleano que indica si el contexto es una página de edición. Este parámetro ajusta el comportamiento del etiquetado según el contexto.
 */
function EtiquetadoAutomaticoDeRecursosMultiple(titulo, descripcion, numMax, params, txtHack, pEsPaginaEdicion) {        
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
    data = JSON.parse(data);
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
        // Cambiar el contenedor donde se establecerá las etiquetas propuestas para el nuevo Front
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

/**
 * Agrega nuevos tags a una lista de tags existentes, evitando duplicados y considerando tags descartados.
 * Actualiza el campo de entrada con los tags manuales y los nuevos tags agregados.
 *
 * @param {string} pListaTagsNuevos - Una cadena de texto con los nuevos tags a agregar, separados por comas.
 * @param {string} pListaTagsViejos - Una cadena de texto con los tags existentes en la lista, separados por comas.
 * @param {string} pTagsID - El ID del campo de entrada donde se encuentran los tags actuales y donde se agregarán los nuevos tags.
 * @returns {string} - Una cadena de texto con los nuevos tags agregados, separados por comas.
 */
function AgregarTagsDirectosAutomaticos(pListaTagsNuevos, pListaTagsViejos, pTagsID) {
    var tagsDescartados = "";

    if ($('#' + pTagsID + '_Hack').parent().next().next().hasClass('descartados')) {
        tagsDescartados = $('#' + pTagsID + '_Hack').parent().next().next().find('#txtHackDescartados').val();
    }

    var tagsManuales = "";
    var tagsAgregados = "";

    var tagsActuales = $('#' + pTagsID + '_Hack').length > 0
        ? $('#' + pTagsID + '_Hack').val().split(',')
        : [];

    //Recorro los tags de la caja y guardo los manuales
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

    // Pintar tags a partir del input sólo si existe
    $('#' + pTagsID).length > 0 && PintarTags($('#' + pTagsID));

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

    var tagsActuales = $('#' + pTagsID + '_Hack').length > 0
        ? $('#' + pTagsID + '_Hack').val().split(',')
        : [];

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


/**
 * Extiende el prototipo de `String` para añadir el método `endsWith`.
 * 
 * Este método verifica si una cadena de texto termina con una subcadena específica.
 * 
 * @param {string} str - La subcadena con la que se verifica si la cadena de texto termina.
 * @returns {boolean} - Devuelve `true` si la cadena de texto termina con la subcadena especificada, `false` en caso contrario.
 */
String.prototype.endsWith = function (str) { return (this.match(str + "$") == str) };


/**
 * Extiende el prototipo de `String` para añadir el método `startsWith`.
 * 
 * Este método verifica si una cadena de texto comienza con una subcadena específica.
 * 
 * @param {string} str - La subcadena con la que se verifica si la cadena de texto empieza.
 * @returns {boolean} - Devuelve `true` si la cadena de texto empieza con la subcadena especificada, `false` en caso contrario.
 */
String.prototype.startsWith = function (str)
{ return (this.match("^" + str) == str) };


let timeoutUpdateProgress;

/**
 * Muestra una máscara blanca de progreso de actualización.
 * 
 * Esta función llama a `MostrarUpdateProgressTime` sin parámetros, lo que hace que la máscara blanca se muestre sin establecer un temporizador.
 * 
 * @example
 * MostrarUpdateProgress();
 */
function MostrarUpdateProgress() {
    MostrarUpdateProgressTime();
}

/**
 * Muestra una máscara blanca de progreso de actualización con un temporizador opcional para ocultar la máscara.
 * 
 * Esta función añade una clase a `<body>` para activar una máscara blanca de progreso. Si se proporciona un tiempo positivo, la máscara se oculta después de ese tiempo.
 * 
 * @param {number} [time=0] - Tiempo en milisegundos para el temporizador de ocultar la máscara. Si es mayor que 0, `OcultarUpdateProgress` se llama después de ese tiempo. Si no se proporciona o se establece en 0, el temporizador no se establece.
 * 
 */
function MostrarUpdateProgressTime(time) {
    if ($('#mascaraBlanca').length > 0) {
        $('body').addClass('mascaraBlancaActiva');

        if (time > 0) {
            timeoutUpdateProgress = setTimeout("OcultarUpdateProgress()", time);
        }
    }
}

/**
 * Método para observar la posible aparición de un editor TinyMCE
 * @param {Array} classNames - Lista de clases a observar
 * @param {Function} callback - Función a ejecutar cuando se detecta una clase
 */
function setupObserverForTinyMCE(classNames, callback) {
    // Función recursiva para buscar elementos con las clases deseadas
    function buscarElementos(node) {
        if (node.nodeType === Node.ELEMENT_NODE) {
            classNames.forEach(function (className) {
                if ($(node).hasClass(className)) {
                    callback($(node));
                }
            });
        }
        node.childNodes.forEach(buscarElementos);
    }

    // Ejecutar la búsqueda en el documento completo
    buscarElementos(document.body);

    // Configurar el MutationObserver para observar cambios en el DOM
    let observer = new MutationObserver(function (mutations) {
        mutations.forEach(function (mutation) {
            mutation.addedNodes.forEach(function (node) {
                buscarElementos(node);
            });
        });
    });
    observer.observe(document.body, { childList: true, subtree: true });
}

/**
 * Método para observar la posible aparición de un editor TinyMCE basado en su ID
 * @param {string} id - ID a observar
 * @param {Function} callback - Función a ejecutar cuando se detecta el ID
 */
function setupObserverForTinyMCEById(id, callback) {
    // Función recursiva para buscar elementos con el ID deseado
    function buscarElementos(node) {
        if (node.nodeType === Node.ELEMENT_NODE) {
            if (node.id === id) {
                callback(node);
            }
        }
        node.childNodes.forEach(buscarElementos);
    }

    // Ejecutar la búsqueda en el documento completo
    buscarElementos(document.body);

    // Configurar el MutationObserver para observar cambios en el DOM
    let observer = new MutationObserver(function (mutations) {
        mutations.forEach(function (mutation) {
            mutation.addedNodes.forEach(function (node) {
                buscarElementos(node);
            });
        });
    });
    observer.observe(document.body, { childList: true, subtree: true });
}

// Función para observar la aparición de una determinada clase y ejecutar un callback
function observarClaseTinyLoaded(callback) {
    // Crear un MutationObserver para observar cambios en el DOM
    var observer = new MutationObserver(function (mutations) {
        mutations.forEach(function (mutation) {
            // Verificar si se ha agregado la clase  a algún input o textarea
            if (mutation.type === 'attributes' && mutation.attributeName === 'class') {
                var target = mutation.target;
                // Verificar si el elemento es un input o textarea y si su clase contiene "tinyLoaded"
                if ((target.tagName === 'INPUT' || target.tagName === 'TEXTAREA') && target.classList.contains(operativaTinyMceConfig.tinyLoadedFinishedClassName)) {
                    // Ejecutar el callback
                    callback(target);
                }
            }
        });
    });

    // Observar cambios en el DOM, enfocándose en el atributo class de los nodos
    observer.observe(document.body, { attributes: true, subtree: true });
}


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

/* Funciones nuevas para uso de loading con librería busyLoad */
/**
 * [loadingMostrar]
 * Muestra una máscara de loading ya sea en pantalla completa o dentro de un elemento html deseado.
 * Requiere librería busy-load.
 * @param  {jQueryObject} pJqueryHtmlElemento: Elemento jQuery - Html. Por defecto el loading se mostrará en toda la pantalla.
 * @param  {jQueryObject} loadingText: Texto que se desea mostrar junto con el loading. Por defecto no se muestra.
 * Si se desea mostrar el loading dentro de un elemento html (Ej: div), proporcionar en pJqueryHtmlElemento el elemento html.
 * Ejemplo de uso
 * https://github.com/piccard21/busy-load
 */
const loadingMostrar = function (pJqueryHtmlElemento = undefined, loadingText = undefined) {
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
        text: loadingText != undefined ? loadingText : "",
        textPosition: "bottom",
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
}

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
    if (loadingParent.length > 0) {
        // Permitir hacer scroll (Añadía la librería un no-scroll)
        loadingParent.removeClass("no-scroll");
        loadingParent.busyLoad("hide", busyParams);
    } else {
        // El loading está dentro de un elemento del dom
        const loadingContainer = $('.busy-load-active');
        loadingContainer.busyLoad("hide", busyParams);
    }
};


/**
 * Oculta la máscara blanca de progreso de actualización y las ventanas emergentes.
 * 
 * Esta función oculta los elementos de la interfaz de usuario relacionados con la máscara blanca de progreso, detiene cualquier temporizador de progreso en curso, y oculta todos los elementos con la clase `popup`.
 * 
 */
function OcultarUpdateProgress() {
    if ($('#mascaraBlanca').length > 0) {
        $('.popup').hide();
        $('#mascaraBlanca').hide()
        clearTimeout(timeoutUpdateProgress);
    }
}


/**
 * Ejecuta una función cuando el usuario hace scroll en la página, pero solo si el valor de un campo oculto indica que se deben aceptar cookies.
 * **Uso común:**
 * - Este código se usa para ejecutar la función `AceptarCookies` cuando el usuario interactúa con la página mediante el scroll, bajo la condición de que un campo específico (`#inpt_Cookies`) indique que es necesario aceptar cookies.
 */
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

let cookiesAceptadas = false;

function AceptarCookies() {
    if (!cookiesAceptadas) {
        cookiesAceptadas = true;
    }
}

/**
 * **`comportamientoFacetasPopUpPlegado`** es un objeto que gestiona el comportamiento de un modal relacionado con facetas en una aplicación web.
 * Este objeto maneja la apertura, configuración, y el cierre del modal, así como la carga y visualización de facetas semánticas o clonadas.
 * 
 * **Propiedades:**
 * - `facetaActual`: {string} ID de la faceta actual seleccionada.
 * - `facetaActualName`: {string} Nombre de la faceta actual.
 * - `FacetTranslations`: {object} Traducciones para los textos del modal en diferentes idiomas.
 * - `$modalLoaded`: {jQuery object} Referencia al modal actualmente cargado.
 * - `facetaTesauroSemantico`: {boolean} Indica si la faceta actual es de tipo tesauro semántico.
 * - `arrayTotales`: {Array} Array que contiene las facetas y sus subfacetas.
 * - `textoActual`: {string} Texto actual del campo de búsqueda en el modal.
 */
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
            that.facetaTesauroSemantico = $(e.relatedTarget).data('facetissemantic');

            // Establecer el título en el modal correspondiente
            that.$modalLoaded.find(".loading-modal-facet-title").text(that.facetaActualName);

            if (that.facetaTesauroSemantico == "True") {
                that.obtenerFacetaTesuaroSemantico();
            }
            else {
                // Clonar la faceta/mostrarlo en modal
                that.clonarFaceta();
            }

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

    obtenerFacetaTesuaroSemantico: function () {
        const that = this;

        // Configuración del servicio para llamar a las facetas deseadas
        var metodo = "CargarFacetas";
        var params = {};
        params["pProyectoID"] = $("input.inpt_proyID").val();
        params["pEstaEnProyecto"] = $("input.inpt_bool_estaEnProyecto").val() == "True";
        params["pEsUsuarioInvitado"] = $("input.inpt_bool_esUsuarioInvitado").val() == "True";
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
        params["pParametros_adiccionales"] = parametros_adiccionales + "|NumElementosFaceta=10000|";
        params["pUbicacionBusqueda"] = ubicacionBusqueda;
        params["pNumeroFacetas"] = -1;
        params["pUsarMasterParaLectura"] = bool_usarMasterParaLectura;
        params["pFaceta"] = that.facetaActual;


        // Petición al servicio para obtención de Facetas                
        $.post(obtenerUrl($('input.inpt_UrlServicioFacetas').val()) + "/" + metodo, params, function (data) {
            var htmlRespuesta = $("<div>").html(data);
            that.arrayTotales = new Array($(htmlRespuesta).find(".faceta").length);
            that.facetaTesuaroSemantico = $(htmlRespuesta).find(".tesaroSemantico").length > 0;
            numFacetasTotales = that.arrayTotales;
            var i = 0;
            $(htmlRespuesta)
                .find(".faceta")
                .each(function () {
                    that.arrayTotales[i] = new Array(3);
                    that.arrayTotales[i][0] = that.eliminarAcentos(
                        $(this).text().toLowerCase()
                    );
                    that.arrayTotales[i][1] = $(this);
                    // Obtenemos las facetas hijas
                    that.arrayTotales[i][2] = $(this).siblings();
                    i++;
                });

            // Limpio antes de mostrar datos - No harí­a falta si elimino todo con el cierre del modal
            that.$modalLoaded.find(".indice-lista.no-letra ul.listadoFacetas").remove();

            for (i = 0; i < that.arrayTotales.length; i++) {
                if (!that.arrayTotales[i][1].hasClass("isChildren")) {
                    ul = $(`<ul class="listadoFacetas">`);
                    that.$modalLoaded.find(".indice-lista.no-letra .resultados-wrap").append(ul);

                    var li = $("<li>");
                    li.append(that.arrayTotales[i][1]);
                    if (that.arrayTotales[i][2].length > 0) {
                        that.PintarFacetasHijas(that.arrayTotales[i][2], li);
                    }

                    ul.append(li);
                }

                // Ocultar el Loading
                that.$modalLoaded.find('.loading-modal-facet').addClass('d-none');
            }

            // Configurar click de las faceta
            $(".indice-lista .faceta").off().click(function (e) {
                AgregarFaceta($(this).attr("name"));
                // Cerrar modal
                that.$modalLoaded.modal('toggle');
                e.preventDefault();
            });

            //Añadir comportamiento desplegable para facetas de tipo tesauro semántico
            plegarSubFacetas.init();

        });
    },

    configEvents: function () {
        const that = this;

        // Buscador o filtrado de facetas cuando se inicie la escritura en el Input buscador dentro del modal         
        this.$modalLoaded.find(".buscador-coleccion .buscar .texto").off().keyup(function () {
            that.textoActual = that.eliminarAcentos($(this).val());
            that.filtrarElementos()
        });

        // Configurar click de la faceta
        this.$modalLoaded.find(".faceta").off().click(function (e) {
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
        this.facetaActual = this.facetaActual.replace(/[@:]/g, '_');
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
        if (that.textoActual != '') {
            itemsListado.each(function () {
                // Eliminamos posibles tildes para búsqueda ampliada
                var nombreCat = $(this).find('.textoFaceta').text().normalize('NFD').replace(/[\u0300-\u036f]/g, "").toLowerCase();

                //Compruebo si el nombre de la faceta actual tiene el texto que estoy buscando. Si no lo tiene oculta la faceta
                if (nombreCat.toLowerCase().indexOf(that.textoActual.toLowerCase()) < 0) {
                    $(this).hide();
                }
            });

            //Desplegamos las facetas que no coinciden con la búsqueda
            $(`.js-desplegar-facetas-modal`).trigger('click');
        }
    },

    /**
     * Método para pintar las facetas hijas de de tipo Tesauro para plegar o desplegar su visualización
     * @param facetasHijas: Cada una de las facetas hijas contenidas por parte de una faceta "padre"
     * @param listaTodasFacetas: Faceta padre que contiene todas las subfacetas a mostrar. Debe ir en un "ul"
     */
    PintarFacetasHijas: function (facetasHijas, listaTodasFacetas) {
        let conjuntoFacetasHijas = `
        <ul>            
            ${facetasHijas.html()}
        </ul>
        `;
        listaTodasFacetas.append(conjuntoFacetasHijas);
    },
};

/**
 * Objeto que maneja el comportamiento de los pop-ups de facetas en la interfaz de usuario.
 * Este objeto se encarga de observar cambios en el DOM para la aparición de elementos de facetas,
 * configurar el comportamiento de los modales para la visualización y búsqueda de facetas, y manejar
 * la lógica de interacción con los servicios de facetas.
 */
const comportamientoFacetasPopUp = {
    init: function () {
        // Objetivo Observable
        const that = this;

        if ($("#panFacetas").length > 0 || $("#divFac").length > 0) {
            const target = $("#panFacetas").length == 0 ? $("#divFac")[0] : $("#panFacetas")[0];

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
        //1º Nombre de la faceta
        //2º Titulo del buscador
        //3º True para ordenar por orden alfabético, False para utilizar el orden por defecto
        var that = this;

        /* Esquema que tendrÃ¡
        this.facetasConPopUp = [
            [
                "sioc_t:Tag",
                "Busca por TAGS",
                true,
            ], //Tags            
        ];*/

        // Array de Facetas que tendrá visualización con PopUp
        this.facetasConPopUp = [];

        // Recoger todos posibles botones de "Ver más" para construir un array de facetasConPopUp                
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
            var ordenAlfabetico = $(this).data('facetalphabeticalorder');
            if (ordenAlfabetico == undefined) {
                ordenAlfabetico = "True";
            }
            // Construir el objeto Array de la faceta para luego añadirlo a facetasConPopUp
            const facetaArray = [
                $(this).data('facetkey'),               // Faceta para búsqueda
                $(this).data('facetname'),              // Título de la faceta
                $(this).data('facetalphabeticalorder'), // Ordenar por orden alfabético,
            ];
            that.facetasConPopUp.push(facetaArray);
        });

        for (i = 0; i < this.facetasConPopUp.length; i++) {
            // Faceta de tipo de búsqueda
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

        var facetaTesuaroSemantico = false;

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

        // Petición al servicio para obtenciÃ³n de Facetas                
        $.post(obtenerUrl($('input.inpt_UrlServicioFacetas').val()) + "/" + metodo, params, function (data) {
            var htmlRespuesta = $("<div>").html(data);
            that.arrayTotales = new Array($(htmlRespuesta).find(".faceta").length);
            that.facetaTesuaroSemantico = $(htmlRespuesta).find(".tesaroSemantico").length > 0;
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
            // Header navegaciÃ³n de resultados de facetas
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

            // Añadir al header de navegaciÃ³n de facetas (Anterior)
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
    ordernarPorNumeroResultados: function () {

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
        that.numPaginasTotales = Math.ceil(that.arrayTotales.length / 22);


        var ul = $(`<ul class="listadoFacetas">`);
        // Petición al servicio para obtención de Facetas
        this.fin = true;

        // Controlar botones de siguiente y anteriores si es necesario su mostrado según el nº de facetas
        if (that.paginaActual == 1) {
            $(".js-anterior-facetas-modal").addClass("d-none");
        } else {
            $(".js-anterior-facetas-modal").removeClass("d-none");
        }
        if (that.paginaActual < that.numPaginasTotales) {
            $(".js-siguiente-facetas-modal").removeClass("d-none");
        } else {
            $(".js-siguiente-facetas-modal").addClass("d-none");
        }

        $(".js-siguiente-facetas-modal").removeAttr("style")
        $(".js-anterior-facetas-modal").removeAttr("style");

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


/**
 * Muestra una faceta en un control específico, gestionando el contenido y las facetas relacionadas.
 * 
 * @param {string} faceta - La clave de la faceta que se desea visualizar.
 * @param {string} controlID - El ID del control HTML en el que se va a mostrar la faceta.
 * @returns {boolean} - Siempre devuelve `false`.
 * 
 * @example
 * // Muestra la faceta 'categoria' en el control con ID 'control1'.
 * VerFaceta('categoria', 'control1');
 */
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


/**
 * Determina si hay filtros activos en un conjunto de filtros dado.
 * 
 * @param {string} pFiltros - Una cadena con filtros separados por el carácter `|`.
 * @returns {boolean} - Devuelve `true` si hay filtros activos, de lo contrario, devuelve `false`.
 * 
 * @example
 * // Verifica si hay filtros activos en la cadena de filtros.
 * HayFiltrosActivos('search=camisa|categoria=ropa|orden=asc');
 * // Devuelve `true` porque hay un filtro de búsqueda activo.
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

/**
 * Verifica si hay filtros activos en una cadena de filtros dada.
 * 
 * @param {string} pFiltros - Una cadena que contiene filtros separados por el carácter `|`, en el formato `clave=valor`.
 * @returns {boolean} - Devuelve `true` si hay al menos un filtro activo distinto de `pagina`, `orden`, o `ordenarPor`. Devuelve `false` si no hay filtros activos o solo hay filtros de `pagina`, `orden`, o `ordenarPor`.
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


/**
 * Reemplaza todas las ocurrencias de una subcadena en una cadena de texto por otra subcadena.
 * 
 * @param {string} texto - La cadena de texto en la que se realizarán los reemplazos.
 * @param {string} busqueda - La subcadena que se buscará y reemplazará en el texto.
 * @param {string} reemplazo - La subcadena que reemplazará cada ocurrencia de la subcadena de búsqueda.
 * @returns {string} - La cadena de texto con todas las ocurrencias de `busqueda` reemplazadas por `reemplazo`.
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
 * Carga el contexto de mensajes en un panel específico, mostrando u ocultando el contenedor de contexto según corresponda.
 * 
 * @param {number} pUsuarioID - El ID del usuario asociado al contexto de mensajes.
 * @param {number} pIdentidadID - El ID de identidad asociado al contexto de mensajes.
 * @param {number} pMensajeID - El ID del mensaje que se desea cargar en el contexto.
 * @param {string} pLanguageCode - El código del idioma para el contexto del mensaje.
 * @param {Object} pParametrosBusqueda - Parámetros de búsqueda adicionales que se enviarán al servicio para cargar el contexto.
 * @param {string} pPanelID - El ID del panel donde se mostrará el contexto del mensaje.
 * 
 * @returns {void}
 */
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

        if (panel.children().children().length > 0) {
            document.getElementById('divContenedorContexto').style['display'] = "block";
        }
        else {
            document.getElementById('divContenedorContexto').style['display'] = "none";
        }

        if (panel.children('#ListadoGenerico_panContenedor').length > 0) {
            var panelResultados = $([panel.children('#ListadoGenerico_panContenedor').html()].join(''));
            panelResultados.appendTo(panel);
            panel.children('#ListadoGenerico_panContenedor').html('');
        }

        listadoMensajesMostrarAcciones.init();
        vistaCompactadaMensajes.init();

        if (typeof CompletadaCargaContextoMensajes == 'function') {
            CompletadaCargaContextoMensajes();
        }
    });
}


var currentMousePos = { x: -1, y: -1 };
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


/**
 * Genera un número entero aleatorio dentro del rango especificado, incluyendo el límite inferior pero excluyendo el límite superior.
 * 
 * Esta función devuelve un número entero aleatorio entre `inferior` (inclusive) y `superior` (exclusivo). Por ejemplo, si `inferior` es 1 y `superior` es 10, la función puede devolver cualquier número del 1 al 9.
 * 
 * @param {number} inferior - El límite inferior del rango, un número entero inclusivo.
 * @param {number} superior - El límite superior del rango, un número entero exclusivo.
 * @returns {number} - Un número entero aleatorio entre `inferior` (inclusive) y `superior` (exclusivo).
 */
function aleatorio(inferior, superior) {
    numPosibilidades = superior - inferior
    aleat = Math.random() * numPosibilidades
    aleat = Math.floor(aleat)
    return parseInt(inferior) + aleat
}

/*--------    REGION SCRIPTS INICIALES   -----------------------------------------------------------*/

/**
 * Eliminará los atributos del botón para que no pueda volver a ejecutar nada a menos que se vuelva a cargar la página web
 * Ej: Acciones que se hacen sobre una persona ("No enviar newsletter, Bloquear...")
 * @param {any} elementId: Elemento que se desea cambiar el nombre y eliminar atributos
 * @param {any} nombre: Nombre que tendrá el botón una vez se haya pulsado sobre él y las acciones se hayan realizado
 * @param {any} listaAtributos: Lista de atributos en formato String que serán eliminados del botón (Ej: "data-target", "href", "onclick")
 * */
function CambiarTextoAndEliminarAtributos(elementId, nombre, listaAtributos) {
    // Seleccionamos el elemento
    var element = $('#' + elementId);
    // Eliminar la lista de atributos deseados
    listaAtributos.forEach(atributo => $(element).removeAttr(atributo));
    // Cambiamos el nombre del elemento
    $(element).html(nombre)
    // Añadimos estilo para que no parezca que es "clickable"
    $(element).css('cursor', 'auto');
}

function RecogerErroresAJAX(error) {
    console.log(error)
}

let perfilID;
let perfilOrgID;
let organizacionID;
let esAdministradorOrg;
$(document).ready(function () {
    perfilID = $('input#inpt_perfilID').val();
    perfilOrgID = $('input#inpt_perfilOrgID').val();
    organizacionID = $('input#inpt_organizacionID').val();
    esAdministradorOrg = $('input#inpt_AdministradorOrg').val();
    refrescarNumElementosNuevos();   
});



/**
 * Refresca los elementos nuevos mediante llamadas AJAX periódicas.
 * 
 * Este método verifica si `perfilID` está definido y es válido, luego realiza una llamada a `ObtenerNumElementosNuevosAJAX` para obtener elementos nuevos.
 * Además, configura llamadas repetidas a `ObtenerNumElementosNuevosAJAX` con diferentes intervalos dependiendo del estado de la sesión activa.
 * 
 * @function
 * @returns {void}
 */
function refrescarNumElementosNuevos() {
    if (typeof (perfilID) != 'undefined' && perfilID != 'ffffffff-ffff-ffff-ffff-ffffffffffff' && perfilID != '00000000-0000-0000-0000-000000000000') {
        try {
            ObtenerNumElementosNuevosAJAX(perfilID, perfilOrgID, esAdministradorOrg);
            setTimeout("ObtenerNumElementosNuevosAJAX('" + perfilID + "', '" + perfilOrgID + "', '" + esAdministradorOrg + "')", 60000);
            setTimeout("ObtenerNumElementosNuevosAJAX('" + perfilID + "', '" + perfilOrgID + "', '" + esAdministradorOrg + "')", 240000);

            if ($('input.inpt_MantenerSesionActiva').length == 0 || $('input.inpt_MantenerSesionActiva').val().toLowerCase() == "true") {
                setInterval("ObtenerNumElementosNuevosAJAX('" + perfilID + "', '" + perfilOrgID + "', '" + esAdministradorOrg + "')", 600000);
            }        
        }
        catch (ex) { }
    }
}


/**
 * Realiza una solicitud AJAX para obtener el número de elementos nuevos.
 * 
 * Esta función construye un identificador de nuevos elementos a partir de elementos `<span>` con la clase `novPerfilOtraIdent` y llama a `PeticionesAJAX.CargarNumElementosNuevos` para obtener los nuevos elementos.
 * 
 * @function
 * @param {string} pGuidPerfilUsu - El GUID del perfil del usuario.
 * @param {string} pGuidPerfilOrg - El GUID del perfil de la organización.
 * @param {boolean} pEsBandejaOrg - Indica si es la bandeja de entrada de la organización.
 * @returns {void}
 */
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

/**
 * Método que es ejecutado para mostrar información traida del backend como Mensajes nuevos, invitaciones nuevas, suscripciones nuevas...
 * @param {any} datosRecibidos
 */
function RepintarContadoresNuevosElementos(datosRecibidos) {
    //Leemos los datos
    var arrayDatos = datosRecibidos.split('|');
    var numMensajesNuevos = arrayDatos[0];
    var numInvitacionesNuevos = arrayDatos[1];
    var numSuscripcionesNuevos = arrayDatos[2];
    var numComentariosNuevos = arrayDatos[3];
    var numNotificacionesNuevos = arrayDatos[4];

    var numMensajesSinLeer = arrayDatos[5];
    var numInvitacionesSinLeer = arrayDatos[6];
    var numSuscripcionesSinLeer = arrayDatos[7];
    var numComentariosSinLeer = arrayDatos[8];
    var numNotificacionesSinLeer = arrayDatos[9];

    var numMensajesNuevosOrg = arrayDatos[10];
    var numMensajesSinLeerOrg = arrayDatos[11];
    var numInvitacionesNuevosOrg = arrayDatos[12];
    var numInvitacionesSinLeerOrg = arrayDatos[13];

    var numInvOtrasIdent = arrayDatos[14];

    // Identificación de elementos HTML para controlar el nº de mensajes nuevos
    // Mensajes nuevos    
    const mensajesMenuNavegacionItem = document.querySelectorAll('.liMensajes')//$('#navegacion').find('.liMensajes');
    const suscripcionesMenuNavegacionItem = document.querySelectorAll('.liSuscripciones'); //$('#navegacion').find('.liNotificaciones');
    const notificacionesMenuNavegacionItem = document.querySelectorAll('.liNotificaciones');
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

    DarValorALabel('infoNumNotificacionesPersonalizadas', parseInt(numNotificacionesNuevos) + parseInt(numNotificacionesSinLeer));
    DarValorALabel('infoNumNotificacionesMobilePersonalizadas', parseInt(numNotificacionesNuevos) + parseInt(numNotificacionesSinLeer));
    //Cambiamos el numero de Suscripciones
    DarValorALabel('infoNumSuscriopciones', numSuscripcionesNuevos);
    DarValorALabel('infoNumSuscriopcionesMobile', numSuscripcionesNuevos);

    //Cambiamos el numero de Mensajes sin leer
    DarValorALabel('infNumMensajesSinLeer', parseInt(numMensajesSinLeer) + parseInt(numMensajesSinLeerOrg));
    DarValorALabel('infNumMensajesSinLeerMobile', parseInt(numMensajesSinLeer) + parseInt(numMensajesSinLeerOrg));
    // Añadir punto rojo de nuevos Mensajes
    if (parseInt(numMensajesNuevos) + parseInt(numMensajesNuevosOrg) > 0) {
        $(mensajesMenuNavegacionItem).addClass('nuevos');
    } else {
        $(mensajesMenuNavegacionItem).removeClass('nuevos');
    }
    //Cambiamos el numero de Comentarios sin leer
    DarValorALabel('infNumComentariosSinLeer', numComentariosSinLeer);
    DarValorALabel('infNumComentariosSinLeerMobile', numComentariosSinLeer);
    // Añadir punto rojo de 'nuevos' Comentarios
    if (parseInt(numComentariosNuevos) > 0) {
        $(comentariosMenuNavegacionItem).addClass('nuevos');
    } else {
        $(comentariosMenuNavegacionItem).removeClass('nuevos');
    }
    //Cambiamos el numero de Invitaciones sin leer
    DarValorALabel('infNumInvitacionesSinLeer', parseInt(numInvitacionesSinLeer) + parseInt(numInvitacionesSinLeerOrg));
    DarValorALabel('infNumInvitacionesSinLeerMobile', parseInt(numInvitacionesSinLeer) + parseInt(numInvitacionesSinLeerOrg));
    // Añadir punto rojo de 'sin leer' de Invitaciones - No se utilizan
    /*if (parseInt(numInvitacionesSinLeer) + parseInt(numInvitacionesSinLeerOrg) > 0) {
        $(invitacionesMenuNavegacionItem).addClass('nuevos');
    } else {
        $(invitacionesMenuNavegacionItem).removeClass('nuevos');
    }*/
    //Cambiamos el numero de Notificaciones sin leer
    DarValorALabel('infNumNotificacionesSinLeer',  parseInt(numNotificacionesSinLeer));
    DarValorALabel('infNumNotificacionesSinLeerMobile', parseInt(numNotificacionesSinLeer));

    if (numNotificacionesSinLeer > 0) {
        document.getElementById('notificacionesPersonalizadas').hidden = false
    } else {
        document.getElementById('notificacionesPersonalizadas').hidden = true
    }
    // Añadir punto rojo de 'nuevas' Notificaciones
    if (parseInt(numNotificacionesNuevos)) {
        $(notificacionesMenuNavegacionItem).addClass('nuevos');
    } else {
        $(notificacionesMenuNavegacionItem).removeClass('nuevos');
    }
    //Cambiamos el numero de Suscripciones sin leer
    DarValorALabel('infNumSuscripcionesSinLeer', numSuscripcionesSinLeer);
    DarValorALabel('infNumSuscripcionesSinLeerMobile', numSuscripcionesSinLeer);
    // Añadir punto rojo de nuevas Suscripciones
    if (parseInt(numSuscripcionesNuevos)) {
        $(suscripcionesMenuNavegacionItem).addClass('nuevos');
    } else {
        $(suscripcionesMenuNavegacionItem).removeClass('nuevos');
    }
    // Añadir punto rojo en el icono del usuario para saber si hay mensajes, notificaciones nuevas
    if ((parseInt(numMensajesSinLeer) + parseInt(numMensajesSinLeerOrg) > 0) || (parseInt(numComentariosSinLeer) > 0) || (parseInt(numSuscripcionesSinLeer)) || (parseInt(numNotificacionesSinLeer) > 0)) {
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
 * Pintar el número de elementos (mensajes sin leer, notificaciones, suscripciones) en la label correspondiente y añade la coletilla de "nuevos" o sin leer.
 * De momento elimino la opción de mostrar "nuevos" o "sin leer". 
 * @param {any} pLabelID
 * @param {any} pNumElementos
 */
function DarValorALabel(pLabelID, pNumElementos) {
    // Cambiado por nuevo Front para buscar por clase (El menú clonado también existe y no podrá haber 2 elementos con un mismo ID)
    //if ($('#' + pLabelID).length > 0) {
    if ($('.' + pLabelID).length > 0) {
        // Cambiado por nuevo Front: Cambiado por nuevo Front para buscar por clase (El menú clonado también existe y no podrá haber 2 elementos con un mismo ID). Hecho abajo
        // document.getElementById(pLabelID).innerHTML = pNumElementos;        

        if (pLabelID.indexOf('SinLeer') != -1) {
            // Cambiado por el nuevo Front. No deseamos que muestre "sin leer"
            // document.getElementById(pLabelID).innerHTML += '<span class="indentado">' + mensajes.sinLeer + '</span>';
        }
        else {
            // Cambiado por el nuevo Front. No deseamos que muestre "nuevos"
            // document.getElementById(pLabelID).innerHTML += '<span class="indentado">' + mensajes.nuevos + '</span>'
        }

        // Cambiado por nuevo Front para buscar por clase (El menú clonado también existe y no podrá haber 2 elementos con un mismo ID)
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

/**
 * Actualiza el contenido y la visibilidad de un elemento `<label>` basado en el número de elementos nuevos.
 * 
 * Esta función actualiza el contenido de un elemento `<label>` con el ID proporcionado para mostrar un número de elementos nuevos. Si el número de elementos es mayor que 0, el número se muestra y el elemento se hace visible. Si es 0, el contenido se borra y el elemento se oculta.
 * 
 * @function
 * @param {string} pLabelID - El ID del elemento `<label>` cuyo contenido se actualizará.
 * @param {number} pNumElementos - El número de elementos nuevos a mostrar en el `<label>`.
 * @returns {void}
 */
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


/**
 * Actualiza el contenido y la visibilidad de un elemento `<label>` para mostrar el número de elementos pendientes.
 * 
 * Esta función actualiza el contenido de un elemento `<label>` con el ID proporcionado para mostrar el número de elementos pendientes entre paréntesis. Si el número de elementos es mayor que 0, se muestra el número en el `<label>` y el elemento se hace visible. Si es 0, el contenido se borra y el elemento se oculta.
 * 
 * @function
 * @param {string} pLabelID - El ID del elemento `<label>` cuyo contenido se actualizará.
 * @param {number} pNumElementos - El número de elementos pendientes a mostrar en el `<label>`.
 * @returns {void}
 */
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


/**
 * Muestra una confirmación sencilla en un panel con un mensaje y dos opciones de respuesta.
 * 
 * Esta función muestra un cuadro de confirmación sobre un control específico en un panel. El cuadro de confirmación contiene un mensaje y dos botones para aceptar o cancelar la acción. Solo el primer botón ejecutará la acción proporcionada.
 * 
 * @function
 * @param {jQuery} control - El elemento sobre el cual se mostrará la confirmación.
 * @param {string} mensaje - El mensaje que se mostrará en el cuadro de confirmación.
 * @param {Function} accion - La función que se ejecutará si el usuario confirma la acción.
 * @param {string} panelID - El ID del panel donde se insertará el cuadro de confirmación.
 * @returns {void}
 */
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

/**
 * Establece el contador de mensajes nuevos y actualiza los elementos correspondientes en la interfaz de usuario. Utilizado desde la bandeja de mensajería. 
 * @param {number} pNumTotalComent - El número total de comentarios nuevos.
 */
function EstablecerContadorMensajesNuevos(pNumTotalComent) {
    DarValorALabel('infoNumMensajes', pNumTotalComent);
    DarValorALabelNovedades('infoNumMensajesNovedades', pNumTotalComent);
}


/**
 * Disminuye el contador de mensajes no leídos en la interfaz de usuario.
 * 
 * Esta función decrementa el contador de mensajes no leídos en dos lugares diferentes de la interfaz de usuario: uno que siempre está presente y otro que varía dependiendo del valor de `pBandejaOrg`.
 * 
 * @function
 * @param {boolean} pBandejaOrg - Indica si se deben actualizar los mensajes en la bandeja de organización. Si es verdadero, actualiza el contador de mensajes en la bandeja de organización; de lo contrario, actualiza el contador general.
 * @returns {void}
 */
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


/**
 * Selecciona un elemento de autocompletado y actualiza los elementos de la interfaz de usuario correspondientes.
 * 
 * @param {string} nombre - El nombre del elemento que se va a mostrar en la lista de selección.
 * @param {string} identidad - La identidad única del elemento que se está seleccionando.
 * @param {string} PanContenedorID - El ID del contenedor donde se mostrará el elemento seleccionado.
 * @param {string} txtHackID - El ID del campo de texto oculto donde se almacenan las identidades de los elementos seleccionados.
 * @param {string} [ContenedorMostrarID] - El ID del contenedor que se mostrará después de seleccionar el elemento (opcional).
 * @param {string} txtFiltroID - El ID del campo de texto de filtro que se limpiará al seleccionar un elemento.
 * 
 * @returns {void}
 */
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


/**
 * Elimina un elemento de autocompletado de la lista de elementos seleccionados y actualiza el campo de texto oculto.
 * 
 * @param {HTMLElement} nombre - El elemento HTML del enlace de eliminación que invoca esta función.
 * @param {string} identidad - La identidad única del elemento que se está eliminando.
 * @param {string} PanContenedorID - El ID del contenedor donde se encuentra el elemento a eliminar.
 * @param {string} txtHackID - El ID del campo de texto oculto donde se almacenan las identidades de los elementos seleccionados.
 * @param {string} [ContenedorMostrarID] - El ID del contenedor que se oculta si no quedan más elementos seleccionados (opcional).
 * 
 * @returns {void}
 */
function eliminarAutocompletar(nombre, identidad, PanContenedorID, txtHackID, ContenedorMostrarID) {
    contenedor = document.getElementById(PanContenedorID);
    contenedor.children[0].removeChild(nombre.parentNode);
    document.getElementById(txtHackID).value = document.getElementById(txtHackID).value.replace('&' + identidad, '');
    if (ContenedorMostrarID != null && document.getElementById(txtHackID).value == '') {
        $('#' + ContenedorMostrarID + '').css('display', 'none');
    }
}


/**
 * Obtiene información sobre entidades LOD (Linked Open Data) desde un servicio web y actualiza el tooltip de los elementos en la lista de etiquetas.
 * 
 * @param {string} pUrlServicio - La URL del servicio web que proporciona datos LOD.
 * @param {string} pUrlBaseEnlaceTag - La URL base para crear enlaces a las etiquetas en LOD.
 * @param {string} pDocumentoID - El ID del documento para el que se obtendrán las entidades LOD.
 * @param {string[]} pEtiquetas - Una lista de etiquetas (tags) para buscar en el servicio web.
 * @param {string} pIdioma - El idioma en el que se deben obtener las descripciones de las etiquetas.
 * 
 * @returns {void}
 */
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

var arriba;
/**
 * Desplaza la página hacia arriba en pequeños incrementos hasta que llegue a la parte superior.
 * 
 * Esta función simula el desplazamiento continuo hacia arriba de la página, en un intento de llevar el contenido hasta la parte superior del viewport.
 * 
 * @returns {void}
 */
function SubirPagina() {
    if (document.body.scrollTop != 0 || document.documentElement.scrollTop != 0) {
        window.scrollBy(0, -50);
        arriba = setTimeout('SubirPagina()', 5);
    }
    else clearTimeout(arriba);
}


/**
 * Abre una ventana emergente centrada en la pantalla con las propiedades especificadas.
 *
 * @param {string} url - La URL que se cargará en la ventana emergente.
 * @param {number} width - El ancho de la ventana emergente en píxeles.
 * @param {number} height - La altura de la ventana emergente en píxeles.
 *
 * @returns {void}
 */
function MostrarPopUpCentrado(url,width,height)
{
    var left = (screen.width - width) / 2;
    var top = (screen.height - height) / 2;
    var windowProperties = 'height=' + height + ',width=' + width + ',top=' + top + ',left=' + left + ',scrollbars=NO,resizable=NO,menubar=NO,toolbar=NO,location=NO,statusbar=NO,fullscreen=NO';
    window.open(url, '', windowProperties);
}


/**
 * Redirige la página actual a una nueva URL especificada.
 * 
 * @param {string} response - La URL a la que se redirigirá si se define.
 */
function Redirigir(response) {
    if (response != undefined) {
        window.location.href = response;
    }
}

/**
 * Objeto para manejar peticiones relacionadas con cookies.
 */
var PeticionesCookie = {
    /**
     * Método para cargar y refrescar la cookie a través de una petición AJAX.
     */    
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

/**
 * Objeto para manejar peticiones AJAX.
 */
var PeticionesAJAX = {
    /**
     * Método para cargar el número de elementos nuevos.
     * 
     * @param {string} pGuidPerfilUsu - Identificador del perfil del usuario.
     * @param {string} pGuidPerfilOrg - Identificador del perfil de la organización.
     * @param {boolean} pEsBandejaOrg - Indica si es la bandeja de la organización.
     * @param {string} identCargarNov - Identificadores adicionales para cargar novedades.
     * @param {function} RepintarContadoresNuevosElementos - Función para repintar los contadores de nuevos elementos.
     * @param {function} RecogerErroresAJAX - Función para manejar los errores de AJAX.
     */    
    CargarNumElementosNuevos: function (pGuidPerfilUsu, pGuidPerfilOrg, pEsBandejaOrg, identCargarNov, RepintarContadoresNuevosElementos, RecogerErroresAJAX) {
        var urlPeticion = null;
        var rutaEjecucionWeb = $('#inpt_rutaEjecucionWeb').val();
        if (rutaEjecucionWeb == null || rutaEjecucionWeb == undefined) {
            rutaEjecucionWeb = "";
        }
        if ($('#inpt_proyID').val() == '11111111-1111-1111-1111-111111111111') {
            urlPeticion = location.protocol + "//" + location.host + "/" + rutaEjecucionWeb + "PeticionesAJAX/CargarNumElementosNuevos";
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

    /**
     * Método para obtener los datos del usuario actual.
     * 
     * @param {function} FuncionEjecutar - Función a ejecutar con los datos del usuario actual.
     */    
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


/*busquedas.js*/ 
let estamosFiltrando = false;

/**
 * Cuando el usuario navega hacia atrás o hacia adelante, el código verifica si los filtros deben ser reaplicados. Esto es importante para mantener el estado de la interfaz de usuario coherente con la URL actual.
 * Previniendo el comportamiento por defecto del evento popstate, se asegura que la página no se recargue o cambie inesperadamente. Esto proporciona una experiencia de usuario más fluida y controlada.
 */
window.onpopstate = function (e) {
    if (window.location.href.indexOf('?') != -1 || window.location.href.indexOf('#') == -1) {
        if (estamosFiltrando) {
            FiltrarPorFacetas(ObtenerHash2());
            e.preventDefault();
        }
        estamosFiltrando = true;
    }
    estamosFiltrando = true;
};

let enlazarJavascriptFacetas = false;


/**
 * Configura los eventos para los filtros de búsqueda y la paginación.
 * 
 * - Configura el evento `click` para limpiar los filtros de búsqueda.
 * - Configura el evento `change` en los selectores de ordenación para aplicar el nuevo orden.
 * - Configura el evento `click` en los elementos de ordenación y paginación para aplicar filtros correspondientes.
 */
function enlazarFiltrosBusqueda() {
    // Permitir borrar filtros de búsqueda
    $("#divFiltros")
        .unbind()
        .on('click', '.limpiarfiltros', function (event) {
            LimpiarFiltros();
            event.preventDefault();
        });
    $("#panFiltros")
        .unbind()
        .on('click', '.limpiarfiltros', function (event) {
            LimpiarFiltros();
            event.preventDefault();
        });

    $('.panelOrdenContenedor select.filtro')
        .unbind()
        .change(function (e) {
            AgregarFiltro('ordenarPor', $(this).val(), true);
        });

    // Configurar la selección de ordenará de los resultados al pulsar en "Ordenado por"
    $("#panel-orderBy a.item-dropdown")
        // En ordenación, no mostraba el icono seleccionado ya que lo "desmontaba".
        //.unbind()
        .click(function (e) {
            var orderBy = $(this).attr("data-orderBy");
            var filtroOrder = $(this).attr("data-order");
            const concatFilterAndOrder = orderBy + "|" + filtroOrder.split("|")[1];
            AgregarFiltro('ordenarPor', concatFilterAndOrder, true);
        });

    // Orden Ascedente o Descedente
    $('#panel-orderAscDesc a.item-dropdown')
        .click(function (e) {
            var filtro = $(this).attr("data-order");
            AgregarFiltro(filtro.split('|')[0], filtro.split('|')[1], false);
            //e.preventDefault();
        });

    $('.panelOrdenContenedor a.filtroV2')
        .unbind()
        .click(function (e) {
            var filtro = $(this).attr("name");
            AgregarFiltro(filtro.split('-')[0], filtro.split('-')[1], false);
            e.preventDefault();
        });
    $('.paginadorResultados a.filtro')
        .unbind()
        .click(function (e) {
            var filtro = $(this).attr("name");
            AgregarFiltro(filtro.split('|')[0], filtro.split('|')[1], false);

            if (typeof searchAnalitics != 'undefined') {
                searchAnalitics.pageSearch(filtro.split('|')[1]);
            }
            e.preventDefault();
        });
}


/**
 * Configura los eventos para la faceta de búsqueda en un panel de búsqueda con facetas.
 * 
 * - Configura el evento `keydown` en los campos de facetas para permitir la búsqueda al presionar Enter.
 * - Configura el evento `click` en el botón de búsqueda para agregar facetas de búsqueda o aplicar filtros por fecha.
 * - Configura el evento `click` en los enlaces de facetas para agregar facetas individuales o grupos de facetas.
 * - Configura el evento `click` en el enlace para descargar RDF con los filtros aplicados.
 */
function enlazarFacetasBusqueda() {
    $('.facetedSearchBox .filtroFaceta')
        .unbind()
        .keydown(function (event) {
            if ($(this).val().indexOf('|') > -1) {
                $(this).val($(this).val().replace(/\|/g, ''));
            };

	    if (event.which || event.keyCode) {
	        if ((event.which == 13) || (event.keyCode == 13)) {	            
                // Iniciar la búsqueda                        
                const inputContainer = $(this).closest('div');                
                const searchButton = inputContainer.find('.searchButton');                
                searchButton.click();                
                event.preventDefault();
	        }
	    } else {
	        return true;
	    };
	});

    var desde = '';
    var hasta = '';
    if ($('.facetedSearchBox .searchButton').parents('.facetedSearchBox').find('input.hasDatepicker').length > 0) {
        desde = $('.facetedSearchBox .searchButton').parents('.facetedSearchBox').find('input.hasDatepicker')[0].value;
        hasta = $('.facetedSearchBox .searchButton').parents('.facetedSearchBox').find('input.hasDatepicker')[1].value;
    }

    $('.facetedSearchBox .searchButton')
        .unbind()
        .click(function (event) {
            if ($(this).parents('.facetedSearchBox').find('.filtroFaceta').length == 1) {
                if ($(this).parents('.facetedSearchBox').find('.filtroFacetaTesauroSemantico').length == 1 && $(this).parents('.facetedSearchBox').find('.filtroFaceta').val().indexOf('@' + $('input.inpt_Idioma').val()) == -1) {
                    AgregarFaceta($(this).parents('.facetedSearchBox').find('.filtroFaceta').attr('name') + '=' + $(this).parents('.facetedSearchBox').find('.filtroFaceta').val() + '@' + $('input.inpt_Idioma').val());
                } else {
                    AgregarFaceta($(this).parents('.facetedSearchBox').find('.filtroFaceta').attr('name') + '=' + $(this).parents('.facetedSearchBox').find('.filtroFaceta').val());
                }
            } else {
                var filtroBusqueda = $(this).attr('name');
                var fechaDesde = $(this).parents('.facetedSearchBox').find('input')[0];
                var fechaHasta = $(this).parents('.facetedSearchBox').find('input')[1];
                var formatoFecha = false;

                if (typeof ($(this).parents('.facetedSearchBox').find('input.hasDatepicker')[0]) != 'undefined') {
                    formatoFecha = true;
                }

                if (desde == '') {
                    desde = $('input.inpt_Desde').val();
                }
                if (hasta == '') {
                    hasta = $('input.inpt_Hasta').val();
                }

                var filtro = ObtenerFiltroRango(fechaDesde, desde, fechaHasta, hasta, formatoFecha);

                if (filtro != '-') {
                    AgregarFaceta(filtroBusqueda + '=' + filtro);
                }
            }
            return false;
        });

    $('.facetedSearch a.faceta')
        .unbind()
        .click(function (e) {
            AgregarFaceta($(this).attr("name"));
            // Quitar el panel de filtrado para móvil para visualizar resultados correctamente
            $(body).removeClass("facetas-abiertas");
            e.preventDefault();
        });
    $('.facetedSearch input.faceta')
        .unbind()
        .click(function (e) {
            AgregarFaceta($(this).attr("name"));
            e.preventDefault();
        });
    $('.facetedSearch a.faceta.grupo')
        .unbind()
        .click(function (e) {
            AgregarFacetaGrupo($(this).attr("name"));
            e.preventDefault();
        });

    $('#descargarRDF')
        .unbind()
        .click(function (e) {
            var filtros = ObtenerHash2();
            var url = document.location.href;

            if (url.indexOf('?') != -1) {
                url = url.substring(0, url.indexOf('?'));
            }

            var filtroRdf = '?rdf';
            if (filtros != '') {
                filtros = '?' + filtros;
                filtroRdf = '&rdf';
            }

            url = url + filtros + filtroRdf;
            $('#descargarRDF').prop('href', url);
            eval($('#descargarRDF').href);
        });
}


/**
 * Configura los eventos para facetas en un panel de búsqueda que no está directamente relacionado con la búsqueda.
 * 
 * - **Configura el evento `keydown`** en los campos de facetas para evitar la adición de caracteres `|` y prevenir la acción por defecto de Enter.
 * - **Configura el evento `click`** en el botón de búsqueda para redirigir a una URL construida con los filtros aplicados.
 */
function enlazarFacetasNoBusqueda() {
    $('.facetedSearchBox .filtroFaceta')
        .unbind()
        .keydown(function (event) {
            if ($(this).val().indexOf('|') > -1) {
                $(this).val($(this).val().replace(/\|/g, ''));
            };
            if (event.which || event.keyCode) {
                if ((event.which == 13) || (event.keyCode == 13)) {
                    return false;
                }
            } else {
                return true;
            };
        });

    $('.facetedSearchBox .searchButton')
        .unbind()
        .click(function (event) {
            event.preventDefault();
            var urlRedirect = $(this).attr('href') + '?' + $(this).parent().find('.filtroFaceta').attr('name') + '=' + $(this).parent().find('.filtroFaceta').val();
            window.location.href = urlRedirect;
        });
}

let cargarFacetas = true;


/**
 * Añade o actualiza un filtro en la URL de la página actual y aplica los filtros de facetas correspondientes.
 * 
 * - **Actualiza** el filtro en la URL basándose en el tipo de filtro y el valor proporcionado.
 * - **Actualiza** la ordenación de los resultados si el filtro es de tipo 'ordenarPor'.
 * - **Reinicia** la paginación si el parámetro `reiniciarPag` es `true`.
 * - **Actualiza** la vista con los nuevos filtros aplicados.
 * 
 * @param {string} tipoFiltro - Tipo de filtro a aplicar (por ejemplo, 'ordenarPor', 'categoria', etc.).
 * @param {string} filtro - Valor del filtro a aplicar.
 * @param {boolean} reiniciarPag - Indica si se debe reiniciar la paginación.
 */
function AgregarFiltro(tipoFiltro, filtro, reiniciarPag) {
    estamosFiltrando = true;
    cargarFacetas = false;
    var filtros = ObtenerHash2();

    var tipoOrden = "";
    if (tipoFiltro == "ordenarPor" && filtro.split("|").length > 1) {
        tipoOrden = filtro.split("|")[1];
        filtro = filtro.split("|")[0];
    }

    //Si el filtro ya existe, cambiamos el valor del filtro
    if (tipoFiltro == "ordenarPor" && filtros.indexOf(tipoFiltro + '=' + filtro) == 0 || filtros.indexOf('&' + tipoFiltro + '=' + filtro) > 0) {
        var tipoFiltroOrden = "orden";
        var filtroOrden = "asc";

        if (filtros.indexOf(tipoFiltroOrden + '=') == 0 || filtros.indexOf('&' + tipoFiltroOrden + '=') > 0) {
            if (filtros.indexOf(tipoFiltroOrden + '=' + filtroOrden) == 0 || filtros.indexOf('&' + tipoFiltroOrden + '=' + filtroOrden) > 0) {

                filtroOrden = "desc";
            }
            if (tipoOrden != "") {

                filtroOrden = tipoOrden;
            }

            var textoAux = filtros.substring(filtros.indexOf(tipoFiltroOrden + '='));
            if (textoAux.indexOf('&') > -1) {
                textoAux = textoAux.substring(0, textoAux.indexOf('&'));
            }
            filtros = filtros.replace(textoAux, tipoFiltroOrden + '=' + filtroOrden);
        }
        else {
            if (filtros.length > 0) { filtros += '&'; }
            filtros += tipoFiltroOrden + '=' + filtroOrden;
        }
    }
    else if (filtros.indexOf(tipoFiltro + '=') == 0 || filtros.indexOf('&' + tipoFiltro + '=') > 0) {
        var textoAux = filtros.substring(filtros.indexOf(tipoFiltro + '='));
        if (textoAux.indexOf('&') > -1) {
            textoAux = textoAux.substring(0, textoAux.indexOf('&'));
        }
        filtros = filtros.replace(textoAux, tipoFiltro + '=' + filtro);

        //Si el filtro es un ordenar por y viene con asc o desc, lo ponemos
        if (tipoFiltro == "ordenarPor" && tipoOrden != "") {
            var textoAux2 = filtros.substring(filtros.indexOf('orden='));
            if (textoAux2.indexOf('&') > -1) {
                textoAux2 = textoAux2.substring(0, textoAux2.indexOf('&'));
            }
            filtros = filtros.replace(textoAux2, 'orden=' + tipoOrden);
        }
    }
    else {
        if (filtros.length > 0) { filtros += '&'; }
        filtros += tipoFiltro + '=' + filtro;

        //Si el filtro es un ordenar por y viene con asc o desc, lo ponemos
        if (tipoFiltro == "ordenarPor" && tipoOrden != "") {
            filtros += '&orden=' + tipoOrden;
        }
    }

    if (reiniciarPag == true) {
        var pagina = 'pagina'
        if (filtros.indexOf(pagina) > -1) {
            if (filtros.indexOf(pagina + '&') > -1) {
                textoAux = filtros.substring(filtros.indexOf(pagina + '&'));
            }
            else if (filtros.indexOf('&' + pagina) > -1) {
                textoAux = filtros.substring(filtros.indexOf('&' + pagina));
            }
            else {
                textoAux = filtros.substring(filtros.indexOf(pagina));
            }

            filtros = filtros.replace(textoAux, '');
        }
    }

    history.pushState('', 'New URL: ' + filtros, '?' + filtros);
    if ($('#divCharts').length != 0) {
        SeleccionarDashboard();
    } else {
        FiltrarPorFacetas(ObtenerHash2());
    }
}


/**
 * Añade un filtro a la URL de la página actual para una faceta específica.
 * Redirige la página a la nueva URL con el filtro aplicado.
 * 
 * @param {string} claveFaceta - La clave de la faceta para la que se está aplicando el filtro.
 * @param {string} filtro - El valor del filtro a aplicar para la faceta.
 */
function AgregarFiltroAutocompletar(claveFaceta, filtro) {
    var url = document.location;
    var separador = '?';
    if (url.indexOf("?") != -1) {
        separador = '&';
    }
    document.location = url + separador + claveFaceta + '=' + filtro;
}

let clickEnFaceta = false;

/**
 * Agrega una faceta a la URL actual para aplicar un nuevo filtro o eliminar uno existente.
 * Actualiza la URL con el nuevo conjunto de filtros y realiza acciones adicionales dependiendo del estado del filtro.
 * 
 * @param {string} faceta - La faceta a agregar o eliminar, que puede incluir un valor de filtro específico.
 */
function AgregarFaceta(faceta) {
    faceta = faceta.replace(/%22/g, '"');
    estamosFiltrando = true;
    //var filtros = ObtenerHash2().replace(/%20/g, ' ');
    var filtros = ObtenerHash2();
    filtros = replaceAll(filtros, '%26', '---AMPERSAND---');
    filtros = decodeURIComponent(filtros);
    filtros = replaceAll(filtros, '---AMPERSAND---', '%26');



    var esFacetaTesSem = false;

    if (faceta.indexOf('|TesSem') != -1) {
        esFacetaTesSem = true;
        faceta = faceta.replace('|TesSem', '');
    }

    var eliminarFacetasDeGrupo = '';
    if (faceta.indexOf("rdf:type=") != -1 && filtros.indexOf(faceta) != -1) {
        //Si faceta es RDF:type y filtros la contiene, hay que eliminar las las que empiezen por el tipo+;
        eliminarFacetasDeGrupo = faceta.substring(faceta.indexOf("rdf:type=") + 9) + ";";
    }

    var filtrosArray = filtros.split('&');
    filtros = '';

    var tempNamesPace = '';
    if (faceta.indexOf('|replace') != -1) {
        tempNamesPace = faceta.substring(0, faceta.indexOf('='));
        faceta = faceta.replace('|replace', '');
    }

    var facetaDecode = decodeURIComponent(faceta);
    var contieneFiltro = false;

    for (var i = 0; i < filtrosArray.length; i++) {
        if (filtrosArray[i] != '' && filtrosArray[i].indexOf('pagina=') == -1) {
            if (eliminarFacetasDeGrupo == '' || filtrosArray[i].indexOf(eliminarFacetasDeGrupo) == -1) {
                if (tempNamesPace == '' || (tempNamesPace != '' && filtrosArray[i].indexOf(tempNamesPace) == -1)) {
                    filtros += filtrosArray[i] + '&';
                }
            }
        }

        if (filtrosArray[i] != '' && (filtrosArray[i] == faceta || filtrosArray[i] == facetaDecode)) {
            contieneFiltro = true;
        }
    }

    if (filtros != '') {
        filtros = filtros.substring(0, filtros.length - 1);
    }
    if (faceta.indexOf('search=') == 0) {

        $('h1 span#filtroInicio').remove();
    }

    if (typeof (filtroDePag) != 'undefined' && filtroDePag.indexOf(faceta) != -1) {
        var url = document.location.href;
        //var filtros = '';

        if (filtros != '') {
            filtros = '?' + filtros.replace(/ /g, '%20');
            //filtros = '?' + encodeURIComponent(filtros);
        }

        if (url.indexOf('?') != -1) {
            //filtros = url.substring(url.indexOf('?'));
            url = url.substring(0, url.indexOf('?'));
        }

        if (url.substring(url.length - 1) == '/') {
            url = url.substring(0, (url.length - 1));
        }

        //Quito los dos ultimos trozos:
        url = url.substring(0, url.lastIndexOf('/'));
        url = url.substring(0, url.lastIndexOf('/'));

        if (filtroDePag.indexOf('skos:ConceptID') != -1) {
            var busAvazConCat = false;

            if (typeof (textoCategoria) != 'undefined') {
                //busAvazConCat = (url.indexOf('/' + textoCategoria) == (url.length - textoCategoria.length - 1));
                if (url.indexOf(textoComunidad + '/') != -1) {
                    var trozosUrl = url.substring(url.indexOf(textoComunidad + '/')).split('/');
                    busAvazConCat = (trozosUrl[2] == textoCategoria);
                }
            }

            url = url.substring(0, url.lastIndexOf('/'));

            if (busAvazConCat) {
                url += '/' + textoBusqAvaz;
            }
        }


        MostrarUpdateProgress();

        document.location = url + filtros;
        return;
    }
    else if (!contieneFiltro) {
        //Si no existe el filtro, lo a?adimos
        if (filtros.length > 0) { filtros += '&'; }
        filtros += faceta;

        if (typeof searchAnalitics != 'undefined') {
            searchAnalitics.facetsSearchAdd(faceta);
        }
    }
    else {
        filtros = '';

        for (var i = 0; i < filtrosArray.length; i++) {
            if (filtrosArray[i] != '' && filtrosArray[i] != faceta && filtrosArray[i] != facetaDecode) {
                //if (filtrosArray[i] != '' && (filtrosArray[i].indexOf(faceta) == -1 || filtrosArray[i].indexOf(facetaDecode)) == -1)) {
                filtros += filtrosArray[i] + '&';
            }
        }

        if (filtros != '') {
            filtros = filtros.substring(0, filtros.length - 1);
        }

        if (!esFacetaTesSem && typeof searchAnalitics != 'undefined') {
            searchAnalitics.facetsSearchRemove(faceta);
        }
    }

    filtros = filtros.replace(/&&/g, '&');
    if (filtros.indexOf('&') == 0) {
        filtros = filtros.substr(1, filtros.length);
    }
    if (filtros[filtros.length - 1] == '&') {
        filtros = filtros.substr(0, filtros.length - 1);
    }

    filtros = filtros.replace(/\\\'/g, '\'');
    filtros = filtros.replace('|', '%7C');

    history.pushState('', 'New URL: ' + filtros, '?' + filtros);
    if ($('#divCharts').length != 0) {
        SeleccionarDashboard();
    } else {
        FiltrarPorFacetas(ObtenerHash2());
    }
}

/**
 * Agrega una faceta a un grupo de facetas en la URL actual, o la marca como `default` si ya existe.
 * El método maneja las facetas en un contexto de grupo, permitiendo marcar facetas existentes como `default` o agregarlas si no están presentes.
 * 
 * @param {string} faceta - La faceta del grupo a agregar o marcar como `default`.
 */
function AgregarFacetaGrupo(faceta) {
    estamosFiltrando = true;
    var filtros = ObtenerHash2();
    filtros = replaceAll(filtros, '%26', '---AMPERSAND---');
    filtros = decodeURIComponent(filtros);
    filtros = replaceAll(filtros, '---AMPERSAND---', '%26');

    var agregar = false;
    if (filtros.indexOf("default;" + faceta) == -1) {
        agregar = true;
    }

    var filtrosArray = filtros.split('&');
    filtros = '';

    for (var i = 0; i < filtrosArray.length; i++) {
        if (filtrosArray[i] != '' && filtrosArray[i].indexOf('pagina=') == -1) {
            //Quita los default
            filtros += filtrosArray[i].replace("default;rdf:type", "rdf:type") + '&';
        }
    }

    if (agregar) {

        if (filtros.indexOf(faceta) != -1) {
            //Si lo contiene lo reemplaza
            filtros = filtros.replace(faceta, "default;" + faceta);
        } else {
            //Si no lo contiene lo a?ade
            filtros += "default;" + faceta;
        }
    }

    filtros = filtros.replace(/&&/g, '&');
    if (filtros.indexOf('&') == 0) {
        filtros = filtros.substr(1, filtros.length);
    }
    if (filtros[filtros.length - 1] == '&') {
        filtros = filtros.substr(0, filtros.length - 1);
    }

    filtros = filtros.replace(/\\\'/g, '\'');

    history.pushState('', 'New URL: ' + filtros, '?' + filtros);
    FiltrarPorFacetas(ObtenerHash2());
}

/**
 * Limpia todos los filtros aplicados en la URL actual y redirige a una URL sin filtros.
 * 
 * Este método maneja dos escenarios:
 * 1. Si se encuentra una referencia a `filtroDePag`, limpia los filtros basándose en una URL específica.
 * 2. Si no hay referencia a `filtroDePag`, limpia todos los filtros y redirige a la URL base.
 */
function LimpiarFiltros() {
    if (typeof (filtroDePag) != 'undefined' && filtroDePag != '') {
        var url = document.location.href;

        if ($('.limpiarfiltros') != undefined && $('.limpiarfiltros').attr('href') != undefined && $('.limpiarfiltros').attr('href') != '') {
            url = $('.limpiarfiltros').attr('href');
        }
        else {
            if (url.indexOf('?') != -1) {
                url = url.substring(0, url.indexOf('?'));
            }

            if (url.substring(url.length - 1) == '/') {
                url = url.substring(0, (url.length - 1));
            }

            //Quito los dos ultimos trozos:
            url = url.substring(0, url.lastIndexOf('/'));
            url = url.substring(0, url.lastIndexOf('/'));

            //if (filtroDePag.indexOf('skos:ConceptID') != -1) {
            //    url = url.substring(0, url.lastIndexOf('/'));
            //}
            if (filtroDePag.indexOf('skos:ConceptID') != -1) {
                var busAvazConCat = false;

                if (typeof (textoCategoria) != 'undefined') {
                    //busAvazConCat = (url.indexOf('/' + textoCategoria) == (url.length - textoCategoria.length - 1));
                    if (url.indexOf(textoComunidad + '/') != -1) {
                        var trozosUrl = url.substring(url.indexOf(textoComunidad + '/')).split('/');
                        busAvazConCat = (trozosUrl[2] == textoCategoria);
                    }
                }

                url = url.substring(0, url.lastIndexOf('/'));

                if (busAvazConCat) {
                    url += '/' + textoBusqAvaz;
                }
            }
        }

        MostrarUpdateProgress();

        document.location = url;
        return;
    }
    $('h1 span#filtroInicio').remove();

    history.pushState('', 'New URL: ', '?');
    FiltrarPorFacetas('');
}

function ObtenerHash2() {
    var url = window.location.href;

    if (url.EndsWith("&rss") || url.EndsWith("?rss") || url.EndsWith("&rdf") || url.EndsWith("?rdf")) {
        url = url.substr(0, url.length - 4);
    }
    else if (url.StartsWith("rss&") || url.StartsWith("rdf&")) {
        url = url.substr(4);
    }

    if (url.indexOf('?') > 1) {
        return url.substr(url.indexOf('?') + 1);
    }
    return "";
}

let primeraCargaDeFacetas = true;

/**
 * Filtra el contenido de la página en función del filtro proporcionado.
 * 
 * Este método determina cuál de varias funciones de filtrado debe ser llamada, basándose en la existencia de funciones específicas en el ámbito global:
 * - Llama a `FiltrarBandejaMensajes` si está definida.
 * - Llama a `FiltrarPerfilUsuario` si `FiltrarBandejaMensajes` no está definida pero `FiltrarPerfilUsuario` sí.
 * - Llama a `ProcesarFiltro` si ninguna de las anteriores está definida pero `ProcesarFiltro` sí.
 * - Llama a `FiltrarPorFacetasGenerico` si ninguna de las funciones anteriores está definida.
 * 
 * @param {string} filtro - El filtro que se aplicará para ajustar el contenido de la página.
 * @returns {any} El resultado de la función de filtrado llamada.
 */
function FiltrarPorFacetas(filtro) {
    if (typeof FiltrarBandejaMensajes != "undefined") {
        return FiltrarBandejaMensajes(filtro);
    }
    else if (typeof FiltrarPerfilUsuario != "undefined") {
        return FiltrarPerfilUsuario(filtro);
    }
    else if (typeof ProcesarFiltro != "undefined") {
        return ProcesarFiltro(filtro);
    }
    return FiltrarPorFacetasGenerico(filtro);
}


/**
 * Aplica el filtro proporcionado para ajustar los resultados de búsqueda o la vista de facetas en la página.
 * 
 * Este método maneja diversas configuraciones y actualizaciones en función del filtro recibido, ajustando vistas y contenidos según sea necesario.
 * 
 * @param {string} filtro - El filtro a aplicar a los resultados de búsqueda o a las facetas de la página.
 * @returns {boolean} Retorna `false` al final de la ejecución.
 */
function FiltrarPorFacetasGenerico(filtro) {
    filtro = filtro.replace(/&/g, '|');

    if (typeof (filtroDePag) != 'undefined' && filtroDePag != '') {
        if (filtro != '') {
            filtro = filtroDePag + '|' + filtro;
        }
        else {
            filtro = filtroDePag;
        }
    }
    //Si hay orden por relevancia pero no hay filtro search, quito el orden para que salga el orden por defecto
    //    if(QuitarOrdenReleavanciaSinSearch(filtro))
    //    {
    //        return false;
    //    }
    filtrosPeticionActual = filtro;

    var rdf = false;
    if (filtro.indexOf('?rdf') != -1 && ((filtro.indexOf('?rdf') + 4) == filtro.length)) {
        filtro = filtro.substring(0, filtro.length - 4);
        document.location.hash = document.location.hash.substring(0, document.location.hash.length - 4);
        rdf = true;
    }

    enlazarJavascriptFacetas = true;

    var arg = filtro;

    /*
    var vistaMapa = ($('li.mapView').attr('class') == "mapView activeView");
    var vistaChart = ($('.chartView').attr('class') == "chartView activeView");
    */

    //var vistaMapa = $('li.mapView').hasClass('activeView');
    //var vistaChart = $('.chartView').hasClass('activeView');
    var vistaMapa = $(".item-dropdown.aMapView").hasClass("activeView");
    var vistaChart = $(".item-dropdown.aGraphView").hasClass("activeView");



    if (!primeraCargaDeFacetas && !vistaMapa) {
        MostrarUpdateProgress();
    }

    var parametrosFacetas = 'ObtenerResultados';

    var gruposPorTipo = $('#facetedSearch.facetedSearch .listadoAgrupado ').length > 0;

    if (cargarFacetas && !gruposPorTipo) {
        if (typeof panFacetas != "undefined" && panFacetas != "" && $('#' + panFacetas).length > 0 && !primeraCargaDeFacetas && !gruposPorTipo) {
            $('#' + panFacetas).html('')
        }
        if (numResultadosBusq != "" && $('#' + numResultadosBusq).length > 0 && !primeraCargaDeFacetas) {
            $('#' + numResultadosBusq).html('')
            $('#' + numResultadosBusq).css('display', 'none');
        }
        if (!clickEnFaceta && panFiltrosPulgarcito != "" && $('#' + panFiltrosPulgarcito).length > 0 && !primeraCargaDeFacetas) {
            $('#' + panFiltrosPulgarcito).html('')
        }
    }

    if (!vistaMapa) {
        SubirPagina();
    }

    if (typeof idNavegadorBusqueda != "undefined") {
        $('#' + idNavegadorBusqueda).html('');
        $('#' + idNavegadorBusqueda).css('display', 'none');
    }

    if (!vistaMapa && !primeraCargaDeFacetas) {
        // Vaciar el contenido actual de resultados - Nuevo Front
        // document.getElementById(updResultados).innerHTML = '';
        $(`#${updResultados}`).find(".resource-list-wrap").html('');
        $('#' + updResultados).attr('style', '');
    }

    clickEnFaceta = false;
    var primeraCarga = false;

    if (filtro.length > 1 || document.location.href.indexOf('/tag/') > -1 || (filtroContexto != null && filtroContexto != '')) {
        parametrosFacetas = 'AgregarFacetas|' + arg;
        var parametrosResultados = 'ObtenerResultados|' + arg;
        if (!cargarFacetas) {
            var parametrosResultados = 'ObtenerResultadosSinFacetas|' + arg;
        }
        //cargarFacetas
        var displayNone = '';
        document.getElementById('query').value = parametrosFacetas;
        if (HayFiltrosActivos(filtro) && (tipoBusqeda != 12 || filtro.indexOf("=") != -1)) {
            $('#' + divFiltros).css('display', '');
            $('#' + divFiltros).css('padding-top', '0px !important');
            //$('#' + divFiltros).css('margin-top', '10px');
        }
        var pLimpiarFilt = $('p', $('#' + divFiltros)[0]);

        if (pLimpiarFilt != null && pLimpiarFilt.length > 0) {
            if (!(filtro.length > 1 || document.location.href.indexOf('/tag/') > -1)) {
                pLimpiarFilt[0].style.display = 'none';
            }
            else {
                pLimpiarFilt[0].style.display = '';
            }
        }
    }
    else {
        primeraCarga = true;
        $('#' + divFiltros).css('display', 'none');
        $('#' + divFiltros).css('padding-top', '0px !important');
        //$('#' + divFiltros).css('margin-top', '10px');
    }

    if (rdf) {
        eval(document.getElementById('rdfHack').href);
    }

    var tokenAfinidad = guidGenerator();

    if ((vistaMapa || !primeraCargaDeFacetas) && (!vistaChart || typeof (chartActivo) != "undefined")) {
        MontarResultados(filtro, primeraCarga, 1, '#' + panResultados, tokenAfinidad);
    }

    if (panFacetas != "" && (cargarFacetas || document.getElementById(panFacetas).innerHTML == '' && noGrafico)) {
        var inicioFacetas = 1;

        MontarFacetas(filtro, primeraCarga, inicioFacetas, '#' + panFacetas, null, tokenAfinidad);
    }

    primeraCargaDeFacetas = false;
    cargarFacetas = true;

    var txtBusquedaInt = $('.aaCabecera .searchGroup .text')
    var textoSearch = 'search=';
    if ((filtro.indexOf(textoSearch) > -1) && txtBusquedaInt.length > 0) {
        var filtroSearch = filtro.substring(filtro.indexOf(textoSearch) + textoSearch.length);
        if (filtroSearch.indexOf('|') > -1) {
            filtroSearch = filtroSearch.substring(0, filtroSearch.indexOf('|'));
        }

        txtBusquedaInt.val(decodeURIComponent(filtroSearch));
        txtBusquedaInt.blur();
    }
    CambiarOrden(filtro);
    return false;
}

let noGrafico = true;
/**
 * Ajusta el orden de los resultados en la interfaz de usuario basándose en el parámetro `hash` proporcionado.
 * 
 * Este método actualiza la selección del filtro de orden en un elemento `select` basado en el hash de la URL.
 * Maneja el caso en el que no hay un parámetro de orden explícito en el hash y también gestiona el estado de opciones relacionadas con búsquedas.
 * 
 * @param {string} hash - El hash de la URL que contiene el parámetro de orden.
 * @returns {void}
 */
function CambiarOrden(hash) {
    if ($('.panelOrdenContenedor select.filtro').length > 0) {
        var controlOrden = $('.panelOrdenContenedor select.filtro');

        var ordenarPor = 'ordenarPor=';
        var search = (hash.indexOf('search=') != -1);
        var noOrden = (hash.indexOf(ordenarPor) == -1);

        //controlOrden.find('option:selected').removeAttr('selected');
        $('.panelOrdenContenedor select.filtro option[selected]').removeAttr('selected');

        if (noOrden) {
            //Si no hay orden, compruebo cu?l es la opci?n de orden que debe estar activa
            var opcion = controlOrden.find('option[value="' + ordenPorDefecto + '"]');
            var opcionSearch = controlOrden.find('option[value="' + ordenEnSearch + '"]');
            var ordenSearch = false;
            var ordenSearchPorDefecto = (ordenEnSearch == ordenPorDefecto);

            if (search) {
                //Si viene el par?metro search, pongo a seleccionado el orden en search
                opcion = opcionSearch;
                ordenSearch = true;
            }

            if (!ordenSearchPorDefecto) {
                //Si el orden por defecto no es el mismo que el orden en search, no ser? visible salvo que est? seleccionado
                if (!ordenSearch) {
                    opcionSearch.attr('style', 'display:none');
                }
                else {
                    opcionSearch.removeAttr('style');
                }
            }
        }
        else if (noOrden != -1) {
            //Obtengo el orden actual para seleccionar esa opci?n en el combo 
            var orden = hash.substring(hash.indexOf(ordenarPor) + ordenarPor.length);
            var indiceBarraFin = orden.indexOf('|');
            if (indiceBarraFin != -1) {
                orden = orden.substring(0, indiceBarraFin);
            }

            var opcion = controlOrden.find('option[value="' + orden + '"]');
        }

        if (opcion.length > 0) {
            //Marco el orden actual como seleccionado
            opcion.attr('selected', 'selected');

            if (opcion.length > 0 && opcion.prevObject.length > 0) {
                opcion.prevObject[0].selectedIndex = opcion[0].index;
            }
        }
    }
}


let finUsoMaster = null;
/**
 * Envía una solicitud al servidor para obtener resultados basados en los filtros y configuraciones actuales, y actualiza la interfaz de usuario con esos resultados.
 * 
 * @param {string} pFiltros - Los filtros actuales aplicados a la búsqueda, que se enviarán al servidor para obtener resultados.
 * @param {boolean|string} pPrimeraCarga - Indica si es la primera carga de resultados. Se usa para configurar el comportamiento inicial de la carga.
 * @param {number} pNumeroResultados - El número de resultados que se deben mostrar. Utilizado para la paginación.
 * @param {string} pPanelID - El ID del panel donde se mostrarán los resultados.
 * @param {string} pTokenAfinidad - Un token de afinidad utilizado para asegurar que los resultados de búsqueda sean consistentes.
 * 
 * @returns {void}
 */
function MontarResultados(pFiltros, pPrimeraCarga, pNumeroResultados, pPanelID, pTokenAfinidad) {
    contResultados = contResultados + 1;
    if (document.getElementById('ctl00_ctl00_CPH1_CPHContenido_txtRecursosSeleccionados') != null) {
        document.getElementById('ctl00_ctl00_CPH1_CPHContenido_txtRecursosSeleccionados').value = '';
        document.getElementById('ctl00_ctl00_CPH1_CPHContenido_lblErrorMisRecursos').style.display = 'none';
    }

    var paramAdicional = parametros_adiccionales;

    if ($('.mapView').hasClass('activeView')) {
        paramAdicional += 'busquedaTipoMapa=true';
    }

    if ($('.chartView').hasClass('activeView')) {
        paramAdicional = 'busquedaTipoChart=' + chartActivo + '|' + paramAdicional;
    }

    var metodo = 'CargarResultados';
    var params = {};

    if (bool_usarMasterParaLectura) {
        if (finUsoMaster == null) {
            finUsoMaster = new Date();
            finUsoMaster.setMinutes(finUsoMaster.getMinutes() + 1);
        }
        else {
            var fechaActual = new Date();
            if (fechaActual > finUsoMaster) {
                bool_usarMasterParaLectura = false;
                finUsoMaster = null;
            }
        }
    }

    params['pUsarMasterParaLectura'] = bool_usarMasterParaLectura;
    params['pProyectoID'] = $('input.inpt_proyID').val();
    params['pEsUsuarioInvitado'] = $('input.inpt_bool_esUsuarioInvitado').val() == 'True';

    if (typeof (identOrg) != 'undefined') {

        params['pIdentidadID'] = identOrg;
    }
    else {

        params['pIdentidadID'] = $('input.inpt_identidadID').val();
    }
    params['pParametros'] = '' + pFiltros.replace('#', '');
    params['pLanguageCode'] = $('input.inpt_Idioma').val();
    params['pPrimeraCarga'] = pPrimeraCarga == "True";
    params['pAdministradorVeTodasPersonas'] = adminVePersonas == "True";
    params['pTipoBusqueda'] = tipoBusqeda;
    params['pNumeroParteResultados'] = pNumeroResultados;
    params['pGrafo'] = grafo;
    params['pFiltroContexto'] = filtroContexto;
    params['pParametros_adiccionales'] = paramAdicional;
    params['cont'] = contResultados;
    params['tokenAfinidad'] = pTokenAfinidad;

    // Montar resultados obtenidos por al hacer clic Facetas
    var vistaChart = (params['pParametros_adiccionales'].indexOf('busquedaTipoChart=') != -1);
    if (vistaChart && $('.chartView').length == 0) {
        //datosChartActivo = arraydatos;
        //$(pPanelID).html('<div id="divContChart"></div>');
        //eval(jsChartActivo);
        PintarGrafico(metodo, params, asistente, paramAdicional, '', pNumeroResultados, pPanelID);
    } else {
        $.post(obtenerUrl($('input.inpt_UrlServicioResultados').val()) + "/" + metodo, params, function (response) {
            if (params['cont'] == contResultados) {
                var data = response;
                if (response.Value != null) {
                    data = response.Value;
                }

                var vistaMapa = (params['pParametros_adiccionales'].indexOf('busquedaTipoMapa=true') != -1);
                var vistaChart = (params['pParametros_adiccionales'].indexOf('busquedaTipoChart=') != -1);

                var descripcion = data;

                var funcionJS = '';
                if (descripcion.indexOf('###ejecutarFuncion###') != -1) {
                    var funcionJS = descripcion.substring(descripcion.indexOf('###ejecutarFuncion###') + '###ejecutarFuncion###'.length);
                    funcionJS = funcionJS.substring(0, funcionJS.indexOf('###ejecutarFuncion###'));

                    descripcion = descripcion.replace('###ejecutarFuncion###' + funcionJS + '###ejecutarFuncion###', '');
                }

                if (tipoBusqeda == 12) {
                    var panelListado = $(pPanelID).parent();
                    panelListado.html('<div id="' + pPanelID.replace('#', '') + '"></div><div id="' + panResultados.replace('#', '') + '"></div>')

                    var panel = $(pPanelID);
                    panel.css('display', 'none');
                    panel.html(descripcion);
                    panelListado.append(panel.find('.resource-list').html())
                    panel.find('.resource-list').html('');
                } else if (!vistaMapa && !vistaChart) {

                    // Verificar y añadir la clase "resource-list" a pPanelID si aún no la tiene
                    const pPanel = $(document).find(pPanelID);
                    if (!pPanel.hasClass("resource-list")) {
                        pPanel.addClass("resource-list");
                    }

                    // Montar resultados en el contenedor correcto
                    const contenedor = pPanel.length > 0
                        ? pPanel
                        : $(document).find("#panelResultados.resource-list");

                    // Tener en cuenta la posible existencia de un div adicional de clase ".resource-list-wrap"
                    let contenedorPrincipal = contenedor.find(".resource-list-wrap").length > 0
                        ? $(contenedor.find(".resource-list-wrap")[0])
                        : contenedor;

                    // Si no se ha encontrado ningún contenedor para datos, hacerlo directamente
                    if (contenedorPrincipal.length === 0) {
                        contenedorPrincipal = $(".resource-list-wrap");
                    }

                    // Verificar si `.resource-list-wrap` ya existe en `contenedor`
                    if (contenedorPrincipal.hasClass("resource-list-wrap")) {
                        // Si existe, insertar `descripcion` dentro del div `.resource-list-wrap`
                        contenedorPrincipal.html(descripcion);
                    } else {
                        // Si no existe, envolver `descripcion` en un nuevo div con clase `.resource-list-wrap`
                        contenedorPrincipal.html(`<div class="resource-list-wrap">${descripcion}</div>`);
                    }

                    // Hacer que las propiedades se oculten bien al volver de gráfico
                    var texto = $("#buscador .texto").text();
                    var listados = $('.content-properties .listado');
                    var mosaicos = $('.content-properties .mosaico');

                    if (texto === "Listado" || texto === "Compacto") {
                        listados.removeClass('hidden').attr('hidden', false);
                        mosaicos.addClass('hidden').attr('hidden', true);
                        if (texto === "Compacto") {
                            contenedor.removeClass('listView');
                            contenedor.addClass('compacView');
                        }
                    } else if (texto === "Mosaico") {
                        listados.addClass('hidden').attr('hidden', true);
                        mosaicos.removeClass('hidden').attr('hidden', false);
                    }

                }
                else {
                    var arraydatos = descripcion.split('|||');

                    if ($('#panAuxMapa').length == 0) {
                        $(pPanelID).parent().html($(pPanelID).parent().html() + '<div id="panAuxMapa" style="display:none;"></div>');
                    }

                    if (vistaMapa) {
                        $('#panAuxMapa').html('<div id="numResultadosRemover">' + arraydatos[0] + '</div>');
                    }

                    if (vistaChart) {
                        datosChartActivo = arraydatos;
                        $(pPanelID).html('<div id="divContChart"></div>');
                        eval(jsChartActivo);
                        //PintarGrafico(datosChartActivo);
                    }
                    else {
                        utilMapas.MontarMapaResultados(pPanelID, arraydatos);
                    }
                }
                FinalizarMontarResultados(paramAdicional, funcionJS, pNumeroResultados, pPanelID);
            }
            if (MontarResultadosScroll.pagActual != null) {
                MontarResultadosScroll.pagActual = 1;
                MontarResultadosScroll.setupCargarScroll();
            }
        }, "json");
    }
}

/**
 * Envía una solicitud al servidor para obtener datos y genera un gráfico o una tabla en el panel especificado usando esos datos.
 * 
 * @param {string} metodo - El método del servidor que se llama para obtener los datos del gráfico o tabla.
 * @param {Object} params - Un objeto que contiene los parámetros para la solicitud del servidor.
 * @param {Object} asistente - Un objeto que contiene información sobre el asistente de gráficos, incluyendo título, nombre, tamaño, tipo y datasets.
 * @param {string} paramAdicional - Parámetros adicionales a enviar con la solicitud.
 * @param {string} funcionJS - Una función de JavaScript a ejecutar después de crear el gráfico o tabla.
 * @param {number} pNumeroResultados - El número de resultados a mostrar, utilizado para la paginación.
 * @param {string} pPanelID - El ID del panel donde se mostrará el gráfico o tabla.
 * 
 * @returns {void}
 */
function PintarGrafico(metodo, params, asistente, paramAdicional, funcionJS, pNumeroResultados, pPanelID) {
    $.post(obtenerUrl($('input.inpt_UrlServicioResultados').val()) + "/" + metodo, params, function (response) {
        if (response != '0resultados|||' && response != "") {
            var asistenteTitulo = asistente.titulo;
            var asistenteName = asistente.asistenteName;
            var asistenteTamanyo = asistente.tamanyo;
            var asistenteTipo = asistente.tipo;
            var asistentePropExtra = asistente.propExtra;
            var asistenteDatasets = asistente.listDataset['$values'];
            var asistentesOrdenes = asistente.ordenes;

            var arrayDatos = response.split('|||');

            var panResultados = document.getElementById("divCharts");
            var divNuevo = document.createElement("div");
            divNuevo.className = "generados";
            divNuevo.id = asistente.key;
            divNuevo.setAttribute('data-sort', asistente.orden);
            panResultados.appendChild(divNuevo);

            var tipo = asistenteTipo;
            var tam = tamFijo;
            switch (asistenteTamanyo) {
                case "1x":
                    tam = tamFijo;
                    break;
                case "2x":
                    tam = 2 * tamFijo + 4;
                    break;
                case "3x":
                    tam = 3 * tamFijo + 8;
                    break;
            }
            if (tipo != 3) {
                divNuevo.style = "width:" + tam + "px; height:" + tam / 2 + "px; display:inline-block";
            } else {
                divNuevo.style = "width:" + tam + "px; height:" + tam + "px; display:inline-block";
            }


            if (tipo < 4) {
                var data
                var config = {
                    data: data,
                    options:
                    {
                        responsive: true,
                        plugins:
                        {
                            legend: {
                                position: 'top',
                            }, title: {
                                display: asistenteTitulo,
                                text: asistenteName
                            }
                        }
                    }
                };
                var fill = false;
                switch (tipo) {
                    case 1:
                        data = DataBarrasLineas(arrayDatos, asistenteDatasets, asistentesOrdenes, fill);
                        config.type = 'bar';
                        var horizontal = asistentePropExtra;
                        if (horizontal) {
                            config.options.indexAxis = 'y';
                        }
                        break;
                    case 2:
                        fill = asistentePropExtra;
                        data = DataBarrasLineas(arrayDatos, asistenteDatasets, asistentesOrdenes, fill);
                        config.type = 'line';
                        break;
                    case 3:
                        data = DataCirculos(arrayDatos, asistentesOrdenes);
                        config.type = 'pie';
                        config.options.plugins.legend.display = false;
                        break;
                }

                config.data = data;
                EjemploGrafico(divNuevo, config);
            } else {
                switch (tipo) {
                    case 4:
                        DibujarTabla(asistenteDatasets, arrayDatos, asistentesOrdenes, divNuevo);
                        break;
                    case 5:
                        DibujarHeatMap(asistenteDatasets, arrayDatos, asistentesOrdenes, divNuevo, asistenteName, tam, asistenteTitulo);
                        break;
                }
            }
            var $wrap = $('#divCharts');
            $wrap.find('.generados').sort(function (a, b) {
                return + a.getAttribute('data-sort') -
                    +b.getAttribute('data-sort');
            }).appendTo($wrap);
        }

        FinalizarMontarResultados(paramAdicional, '', pNumeroResultados, pPanelID);
        MontarResultadosScroll.destroyScroll();
    }, "json");
}


/**
 * Genera una tabla usando Google Visualization DataTable con los datos proporcionados y la dibuja en el `div` especificado.
 * 
 * @param {Array} asistenteDatasets - Un array de objetos que contiene los datos para las columnas de la tabla.
 * @param {Array} arrayDatos - Un array de cadenas que contiene las filas de datos, separadas por `@@@`.
 * @param {Array} asistentesOrdenes - Un array que define el orden de las columnas en los datos.
 * @param {HTMLElement} divNuevo - El `div` donde se mostrará la tabla generada.
 * 
 * @returns {void}
 */
function DibujarTabla(asistenteDatasets, arrayDatos, asistentesOrdenes, divNuevo) {
    var data = new google.visualization.DataTable();
    for (var i = 0; i < asistenteDatasets.length; i++) {
        data.addColumn('string', asistenteDatasets[i].nombre);
    }
    var filas = [];
    for (var i = 1; i < arrayDatos.length - 1; i++) {
        var filaDatos = arrayDatos[i].split("@@@");
        var fila = [];
        for (var j = 0; j < filaDatos.length - 1; j++) {
            fila.push(filaDatos[asistentesOrdenes[j]]);
        }
        filas.push(fila)
    }
    data.addRows(filas);
    var table = new google.visualization.Table(divNuevo);
    table.draw(data, { width: '100%', height: '100%' });
}


/**
 * Crea un gráfico de mapa de calor (heatmap) usando Highcharts con los datos proporcionados y lo muestra en el `div` especificado.
 * 
 * @param {Array} asistenteDatasets - Un array de objetos que contiene información sobre los datasets, incluyendo color de la escala del mapa de calor.
 * @param {Array} arrayDatos - Un array de cadenas que contiene los datos del mapa de calor, con datos separados por `@@@`.
 * @param {Array} asistentesOrdenes - Un array que define el orden de las columnas en los datos. Se utiliza para identificar los valores del eje X, eje Y y los valores de los datos.
 * @param {HTMLElement} divNuevo - El `div` HTML donde se generará y mostrará el gráfico de mapa de calor.
 * @param {String} asistenteName - El nombre del asistente que se usará como título de la serie en el gráfico.
 * @param {Number} tam - El tamaño del gráfico, se utiliza para ajustar el tamaño de la leyenda.
 * @param {Boolean} mtGrafico - Determina si se debe agregar un título al gráfico.
 * 
 * @returns {void}
 */
function DibujarHeatMap(asistenteDatasets, arrayDatos, asistentesOrdenes, divNuevo, asistenteName, tam, mtGrafico) {

    var ejeX = [];
    var ejeY = [];
    for (var i = 1; i < arrayDatos.length - 1; i++) {
        var filaDatos = arrayDatos[i].split("@@@");
        if (ejeX.indexOf(filaDatos[asistentesOrdenes[0]]) < 0) {
            ejeX.push(filaDatos[asistentesOrdenes[0]]);
        }
        if (ejeY.indexOf(filaDatos[asistentesOrdenes[1]]) < 0) {
            ejeY.push(filaDatos[asistentesOrdenes[1]]);
        }
    }
    var datosGraf = [];
    for (var i = 1; i < arrayDatos.length - 1; i++) {
        var filaDatos = arrayDatos[i].split("@@@");
        var fila = [ejeX.indexOf(filaDatos[asistentesOrdenes[0]]), ejeY.indexOf(filaDatos[asistentesOrdenes[1]]), parseInt(filaDatos[asistentesOrdenes[2]])];
        datosGraf.push(fila);
    }

    var chart = Highcharts.chart(divNuevo.id, {
        chart: {
            type: 'heatmap',
            marginTop: 40,
            marginBottom: 80,
            plotBorderWidth: 1
        },

        title: {
            text: null
        },

        xAxis: {
            categories: ejeX
        },

        yAxis: {
            categories: ejeY,
            title: null,
            reversed: true
        },

        colorAxis: {
            min: 0,
            minColor: '#FFFFFF',
            maxColor: asistenteDatasets[2].color
        },

        legend: {
            align: 'right',
            layout: 'vertical',
            margin: 0,
            verticalAlign: 'top',
            y: 25,
            symbolHeight: tam / 2 - 120
        },

        tooltip: {
            formatter: function () {
                return this.point.series['xAxis'].categories[this.point['x']] + '<br/>' + this.series.name + '<br/>' + this.point.series['yAxis'].categories[this.point['y']] + '<br/><b>' + this.point.value + '</b>';
            }
        },

        series: [{
            name: asistenteName,
            borderWidth: 1,
            data: datosGraf,
            dataLabels: {
                enabled: true,
                color: '#000000'
            }
        }],
    });

    if (mtGrafico) {
        chart.update({
            title: {
                text: asistenteName
            }
        });
    }
}


/**
 * Genera el formato de datos necesario para crear gráficos de barras o líneas en Chart.js.
 * 
 * @param {Array} arrayDatos - Un array de cadenas con los datos a ser usados en el gráfico. Los datos están separados por `@@@`.
 * @param {Array} asistenteDatasets - Un array de objetos que define los datasets del gráfico, incluyendo nombre, color y otros atributos.
 * @param {Array} asistentesOrdenes - Un array que define el orden de los datos en `arrayDatos`. El primer índice es para las etiquetas y los índices siguientes son para los datos.
 * @param {Boolean} fill - Define si el gráfico debe tener el área de las barras o líneas rellena.
 * 
 * @returns {Object} Un objeto con las etiquetas y datasets necesarios para Chart.js.
 */
function DataBarrasLineas(arrayDatos, asistenteDatasets, asistentesOrdenes, fill) {
    var labels = [];
    for (var i = 1; i < arrayDatos.length - 1; i++) {
        labels.push(arrayDatos[i].split("@@@")[asistentesOrdenes[0]]);
    }
    var datasets = [];
    var transparencia = "";
    if (fill) {
        transparencia = "30";
    }
    for (var i = 0; i < asistenteDatasets.length; i++) {
        var dat = [];
        for (var j = 1; j < arrayDatos.length; j++) {
            dat.push(arrayDatos[j].split("@@@")[asistentesOrdenes[i + 1]]);
        }
        var dataset = {
            label: asistenteDatasets[i].nombre,
            data: dat,
            fill: fill,
            borderColor: asistenteDatasets[i].color,
            backgroundColor: asistenteDatasets[i].color + transparencia,
        };
        datasets.push(dataset);
    }
    var data = {
        labels: labels,
        datasets: datasets
    };
    return data;
}

/**
 * Genera el formato de datos necesario para crear un gráfico de círculos (pie o doughnut) en Chart.js.
 * 
 * @param {Array} arrayDatos - Un array de cadenas con los datos a ser usados en el gráfico. Los datos están separados por `@@@`.
 * @param {Array} asistentesOrdenes - Un array que define el orden de los datos en `arrayDatos`. El primer índice es para las etiquetas y el segundo índice es para los valores.
 * 
 * @returns {Object} Un objeto con las etiquetas y datasets necesarios para Chart.js.
 */
function DataCirculos(arrayDatos, asistentesOrdenes) {
    var labels = [];
    for (var i = 1; i < arrayDatos.length - 1; i++) {
        labels.push(arrayDatos[i].split("@@@")[asistentesOrdenes[0]]);
    }

    var colores = [];
    var dat = [];
    for (var j = 1; j < arrayDatos.length; j++) {
        dat.push(arrayDatos[j].split("@@@")[asistentesOrdenes[1]]);
        colores.push("#" + Math.floor(Math.random() * 16777215).toString(16));
    }

    var dataset = {
        label: 'Dataset',
        data: dat,
        backgroundColor: colores
    };

    var conf = {
        labels: labels,
        datasets: [dataset]
    };
    return conf;
}

/**
 * Crea un gráfico en un nuevo canvas dentro del elemento div proporcionado, usando la configuración proporcionada.
 * 
 * @param {HTMLElement} divNuevo - El elemento div donde se creará el gráfico. Debe estar presente en el DOM.
 * @param {Object} config - Un objeto de configuración para Chart.js, que define el tipo de gráfico, datos y opciones.
 * 
 * @returns {void}
 */
function EjemploGrafico(divNuevo, config) {
    var canvas = document.createElement("canvas");
    canvas.className = "graficoCtx";
    divNuevo.appendChild(canvas);
    var ctx = canvas.getContext('2d');

    new Chart(ctx, config);
}

/**
 * Comportamiento de Scrolling para cargar más resultados al llegar al final de la página
 * */
const scrollingListadoRecursos = {
    init: function () {
        this.config();
        //this.crearCargando();
        this.scrollingListado();
        return;
    },
    config: function () {
        /* Contenedor donde se encuentran los resultados (Recursos, Mensajes...) */
        this.contenedor = $(document).find("#panResultados.resource-list").length > 0
            ? $(document).find("#panResultados.resource-list")
            : $(document).find("#panelResultados.resource-list");

        // Tener en cuenta la posible existencia de un div adicional de clase ".resource-list-wrap"
        this.contenedorPrincipal = this.contenedor.find(".resource-list-wrap").length > 0
            ? this.contenedor.find(".resource-list-wrap")
            : this.contenedor;

        return;
    },
    scrollingListado: function () {
        var that = this;
        MontarResultadosScroll.init("#footer", ".resource");

        MontarResultadosScroll.CargarResultadosScroll = function (data) {
            var htmlRespuesta = document.createElement("div");
            htmlRespuesta.innerHTML = data;
            // Establecer la configuración del contenedor principal por si se ha destruido
            that.config();
            $(htmlRespuesta)
                // Buscar cada resultado item
                .find('.resource')
                .each(function () {
                    var resource = $(this);
                    // Añadir cada item encontrado a la lista de resultados                  
                    that.contenedorPrincipal.append(this);
                });
            that.cargandoScrolling(data);
        };

        return;
    },
    cargandoScrolling: function (data) {
        var cargandoOld = $(".loading-more-results");
        // Ocultar el loading
        if (cargandoOld.length != 0) cargandoOld.addClass("d-none");
        //if (data.length != 0) this.crearCargando();

        return;
    },

    /**
     * Mostrar el Loading de nuevo
     * @returns 
     */
    crearCargando: function () {
        /* Añadir Cargando después del listado */
        const cargandoOld = $(".loading-more-results");
        cargandoOld.removeClass("d-none");
        return;
    },
};

/**
 * Comportamiento de mostrado de Resultados cuando se hace Scrolling. Funcionamiento junto con scrollingListadoRecursos
 * Sólo funcionará si existe un div con id "footer".
 * */
var MontarResultadosScroll = {
    // Umbral de desplazamiento para cargar más resultados (90% de la página)
    threshold: 0.95,
    footer: null,
    item: null,
    pagActual: null,
    active: true,
    init: function (idFooterJQuery, idItemJQuery, allowFirstLoadScrolling = false) {
        this.pagActual = 1;
        this.footer = $(idFooterJQuery);
        this.item = idItemJQuery;
        // Controlar que solo haga una petición cada vez
        this.isLoadingData = false;
        // Configurar carga mediante scroll
        this.footer.length > 0 && this.setupCargarScroll();
        return;
    },

    /**
     * Método que configurará el método mediante el cual se traerán los resultados al hacer scroll.
     * Si el #footer tiene la propiedad de "fixed", se implementará la lógica con cargarScrollForFixedFooter. En caso contrario se lanzará con cargarScroll
     */
    setupCargarScroll: function () {
        const that = this;

        // Verifica si el elemento con clase "footer" tiene la posición "fixed"
        if (that.footer.css('position') === 'fixed') {
            that.cargarScrollForFixedFooter();
        } else {
            that.cargarScroll();
        }
    },
    
    comprobarScroll: function (direction) {
        var numContenedoresChar = $("#divContChart").length;

        // Devuelve true si se está haciendo scroll hacia abajo, no se están cargando datos y la página de búsqueda no se está visualizando en modo gráfico
        return direction === "down" && this.isLoadingData === false && numContenedoresChar == 0;
    },

    cargarScroll: function () {
        var that = this;  // Mantén la referencia de `this` en `that`
        this.waypointMoreResults = new Waypoint({
            element: that.footer,
            handler: function (direction) {
                // Esperar a realizar la petición
                setTimeout(function () {
                    if (that.comprobarScroll(direction)) {
                        // Gestionar la petición de los datos
                        that.handleLoadDataFromScroll();
                    }
                }, 500);
            },
            offset: '100%' // Disparar petición cuando se visualice el footer
        });
    },



    /* Cargar Infinite Scroll para cuando el footer está fixed */
    cargarScrollForFixedFooter: function () {
        var that = this;
        window.addEventListener('scroll', function () {
            if (!that.isLoadingData) {
                var scrollPercentage = (window.scrollY + window.innerHeight) / document.body.scrollHeight;
                if (scrollPercentage >= that.threshold) {
                    that.isLoadingData = true;
                    // Gestionar la petición de los datos
                    that.handleLoadDataFromScroll();
                }
            }
        });
    },

    /**
     * Método que realizará la lógica de petición de los datos y la gestión de estos en el contenedor de resultados.
     * Es llamado cargarScrollForFixedFooter o desde cargarScroll dependiendo de si hay o no footer "fixed"
     */
    handleLoadDataFromScroll: function () {
        const that = this;

        that.isLoadingData = true;
        /* Antes de hacer petición visualizar el "Loading" */
        scrollingListadoRecursos.crearCargando();
        /* Realizar petición al servidor */
        const peticionScrollResultadosPromise = that.peticionScrollResultados();
        peticionScrollResultadosPromise.then(function (data) {
            let htmlRespuesta = document.createElement("div");
            htmlRespuesta.innerHTML = data;
            if ($(htmlRespuesta).find(that.item).length > 0) {
                that.CargarResultadosScroll(data);
            } else {
                that.CargarResultadosScroll('');                
                // La petición ha terminado. Permitir hacer más peticiones
                that.isLoadingData = false;
            }

            // A modo de "completion", ejecutar CompletadaCargaRecursos si está definida
            if (typeof CompletadaCargaRecursos != 'undefined') {
                CompletadaCargaRecursos();
            }

            if (typeof (urlCargarAccionesRecursos) != 'undefined') {
                ObtenerAccionesListadoMVC(urlCargarAccionesRecursos);
            }
        }).catch(function (error) {
            console.error("Error al traer resultados vía scroll:", error);
        }).always(function () {
            that.isLoadingData = false;
            // Fin de petición -> Ocultar loading
            scrollingListadoRecursos.cargandoScrolling();
        });
    },

    destroyScroll: function () {
        this.waypointMoreResults.destroy();
        return;
    },
    peticionScrollResultados: function () {
        var defr = $.Deferred();
        //Realizamos la peticion 
        if (this.pagActual == null) {
            this.pagActual = 1;
        }
        this.pagActual++;
        //var servicio = new WS($('input.inpt_UrlServicioResultados').val(), WSDataType.jsonp);
        var filtros = ObtenerHash2().replace(/&/g, '|');

        if (typeof (filtroDePag) != 'undefined' && filtroDePag != '') {
            if (filtros != '') {
                filtros = filtroDePag + '|' + filtros;
            }
            else {
                filtros = filtroDePag;
            }
        }

        filtros += "|pagina=" + this.pagActual;
        var params = {};

        params['pUsarMasterParaLectura'] = bool_usarMasterParaLectura;
        params['pProyectoID'] = $('input.inpt_proyID').val();
        params['pEsUsuarioInvitado'] = $('input.inpt_bool_esUsuarioInvitado').val() == 'True';
        params['pIdentidadID'] = $('input.inpt_identidadID').val();
        params['pParametros'] = '' + filtros.replace('#', '');
        params['pLanguageCode'] = $('input.inpt_Idioma').val();
        params['pPrimeraCarga'] = false;
        params['pAdministradorVeTodasPersonas'] = adminVePersonas.toString().toLowerCase() == "true";
        params['pTipoBusqueda'] = tipoBusqeda;
        params['pNumeroParteResultados'] = 1;
        params['pGrafo'] = grafo;
        params['pFiltroContexto'] = filtroContexto;
        params['pParametros_adiccionales'] = parametros_adiccionales;
        params['cont'] = contResultados;


        $.post(obtenerUrl($('input.inpt_UrlServicioResultados').val()) + "/CargarResultados", params, function (response) {
            if (params['cont'] == contResultados) {
                var data = response
                if (response.Value != null) {
                    data = response.Value;
                }
                defr.resolve(data);
            }
        }, "json")
            .fail(function (jqXHR, textStatus, errorThrown) {
                // Manejar el error
                defr.reject(errorThrown); // Rechazar la promesa con el mensaje de error
            });
        // Devolver la promesa para que el consumidor la maneje
        return defr.promise(); //         
    }
}


///Uso:
/// <summary>
/// Engancha el comporatamiento de la paginación de resultados
/// </summary>
/// <param name="pEnlaceResource">Identificador Jquery de los elementos que tienen enlaces del recurso representan los recursos</param>
/// <param name="panResultados">Identificador Jquery del elemnto que identifica que se trata de una página de búsqueda</param>
/// <returns></returns>
/// MontarPaginacionBusquedaResultados.init(pEnlaceResource,'#panResultados');			

/// <summary>
/// Método que recibe los resultados
/// </summary>
/// <param name="pBusquedaArray">Array con los enlaces de los recursos de la búsqueda</param>
/// <param name="pIndice">Indice del recurso actual</param>
/// <returns></returns>
///MontarPaginacionBusquedaResultados.MontarBotonera = function (pBusquedaArray, pIndice) {
///    if (pIndice > 0) {
///        //Botón atrás
///        var botonAnterior = '<a href="' + pBusquedaArray[pIndice - 1] + '">Anterior</a>';
///        $(botonAnterior).insertBefore('footer');
///    }
///    if (pIndice < pBusquedaArray.length - 1) {
///        //Botón siguiente
///        var botonSiguiente = '<a href="' + pBusquedaArray[pIndice + 1] + '">Siguiente</a>';
///        $(botonSiguiente).insertBefore('footer');
///    }
//}
var MontarPaginacionBusquedaResultados = {
    numActual: 0,
    itemEnlace: null,
    pagBusqueda: null,
    cookiesEnabled: null,
    init: function (idEnlaceItemJQuery, idContenedorPaginaBusqueda) {
        this.itemEnlace = idEnlaceItemJQuery;
        this.pagBusqueda = idContenedorPaginaBusqueda;
        if ($(this.pagBusqueda).length > 0) {
            //Enganchar paginación de la búsqueda de resultados
            this.cargarPaginacion();
        }
        //Inseratr botones paginación
        this.botonesPaginacion();
        return;
    }, comprobarCookies: function () {
        if (this.cookiesEnabled == null) {
            var dt = new Date();
            dt.setSeconds(dt.getSeconds() + 60);
            document.cookie = "cookietest=1; expires=" + dt.toGMTString();
            this.cookiesEnabled = document.cookie.indexOf("cookietest=") != -1;
            dt.setSeconds(dt.getSeconds() - 120);
            document.cookie = "cookietest=1; expires=" + dt.toGMTString();
        }
        return this.cookiesEnabled;
    },
    cargarPaginacion: function () {
        var urlBusqueda = window.location.href;
        var guid = guidGenerator();
        var recursos = [];

        // Modificamos urls y almacenamos
        $(this.itemEnlace).each(function () {
            var newUrl = $(this).attr('href');
            if (newUrl.indexOf("?") > 0) {
                newUrl = newUrl.substring(0, newUrl.indexOf("?"));
            }

            newUrl += "?searchid=" + guid;
            $(this).attr('href', newUrl);
            if ($.inArray(newUrl, recursos) == -1) {
                recursos.push(newUrl);
            }
        });

        var vars = [], hash;
        if (window.location.href.indexOf('?') > -1) {
            var hashes = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
            for (var i = 0; i < hashes.length; i++) {
                hash = hashes[i].split('=');
                vars.push([hash[0], hash[1]]);
            }
        }

        urlBusqueda = "";
        pagina = 1;
        for (var j = 0; j < vars.length; j++) {
            if (vars[j][0] == "pagina") {
                pagina = vars[j][1];
            } else {
                urlBusqueda += "&" + vars[j][0] + "=" + vars[j][1];
            }
        }

        var InfoBusqueda = {
            resourceList: recursos,
            searchUrl: urlBusqueda,
            tipoBusqueda: tipoBusqeda,
            grafo: grafo,
            filtroContexto: filtroContexto,
            parametros_adiccionales: parametros_adiccionales,
            paginaMin: pagina,
            paginaMax: pagina
        }
        var listaBusquedas = this.comprobarCookies() ? JSON.parse(localStorage.getItem("searchids")) : null;
        if (listaBusquedas == null) {
            listaBusquedas = [];
        } else if (listaBusquedas.length >= 20) {
            localStorage.removeItem(listaBusquedas.shift());
        }
        listaBusquedas.push(guid);
        if (this.comprobarCookies()) {
            localStorage.setItem("searchids", JSON.stringify(listaBusquedas));
            localStorage.setItem(guid, JSON.stringify(InfoBusqueda));
        }
    }, botonesPaginacion: function () {
        this.numActual++;
        var searchid = window.location.search;
        searchid = searchid.substring(searchid.indexOf("searchid=") + "searchid=".length);
        var InfoBusqueda = this.comprobarCookies() ? JSON.parse(localStorage.getItem(searchid)) : null;
        if (InfoBusqueda != null) {
            var indice = $.inArray(window.location.href, InfoBusqueda.resourceList);
            if (indice == InfoBusqueda.resourceList.length - 1 && this.numActual < 2) {
                //Si estamos en el último recurso
                //Cargamos la página siguiente
                this.peticionPaginaSiguiente(searchid, InfoBusqueda, true);
            } else if (indice == 0 && InfoBusqueda.paginaMin > 1 && this.numActual < 2) {
                //Si estamos en el primer recurso y la página es mayor a la 1
                //Cargamos la página anterior
                this.peticionPaginaSiguiente(searchid, InfoBusqueda, false);
            } else if (indice != -1) {
                //Si no es ninguno de los dos casos anteriores y el recurso está en la lista montamos los botones
                this.MontarBotonera(InfoBusqueda.resourceList, indice);
            }
        }
    }, peticionPaginaSiguiente: function (searchID, InfoBusqueda, siguiente) {
        var pagina = 0;
        if (siguiente) {
            InfoBusqueda.paginaMax = InfoBusqueda.paginaMax + 1;
            pagina = InfoBusqueda.paginaMax;
        } else {
            InfoBusqueda.paginaMin = InfoBusqueda.paginaMin - 1;
            pagina = InfoBusqueda.paginaMin;
        }

        var that = this;
        var filtros = InfoBusqueda.searchUrl.substring(InfoBusqueda.searchUrl.indexOf("?") + 1).replace(/&/g, '|');
        filtros += "|pagina=" + pagina;
        var params = {};

        params['pUsarMasterParaLectura'] = bool_usarMasterParaLectura;
        params['pProyectoID'] = $('input.inpt_proyID').val();
        params['pEsUsuarioInvitado'] = $('input.inpt_bool_esUsuarioInvitado').val() == 'True';
        params['pIdentidadID'] = $('input.inpt_identidadID').val();
        params['pParametros'] = '' + filtros.replace('#', '');
        params['pLanguageCode'] = $('input.inpt_Idioma').val();
        params['pPrimeraCarga'] = false;
        params['pAdministradorVeTodasPersonas'] = false;
        params['pTipoBusqueda'] = InfoBusqueda.tipoBusqueda;
        params['pNumeroParteResultados'] = 1;
        params['pGrafo'] = InfoBusqueda.grafo;
        params['pFiltroContexto'] = InfoBusqueda.filtroContexto;
        params['pParametros_adiccionales'] = InfoBusqueda.parametros_adiccionales;

        $.post(obtenerUrl($('input.inpt_UrlServicioResultados').val()) + "/CargarResultados", params, function (response) {
            var data = response
            if (response.Value != null) {
                data = response.Value;
            }

            // Almacenamos urls
            var htmlRespuesta = document.createElement("div");
            htmlRespuesta.innerHTML = data;
            var listaRecursos = [];
            $(htmlRespuesta).find(that.itemEnlace).each(function () {
                var newUrl = $(this).attr('href') + "?searchid=" + searchID;
                if ($.inArray(newUrl, listaRecursos) == -1) {
                    listaRecursos.push(newUrl);
                }
            });

            if (listaRecursos.length > 0) {
                if (siguiente) {
                    InfoBusqueda.resourceList = InfoBusqueda.resourceList.concat(listaRecursos);
                } else {
                    InfoBusqueda.resourceList = listaRecursos.concat(InfoBusqueda.resourceList);
                }
                if (that.comprobarCookies()) {
                    //Almacenamos los nuevos recursos
                    localStorage.setItem(searchID, JSON.stringify(InfoBusqueda));
                }
            }
            //Montamos la paginacion
            that.botonesPaginacion();
        }, "json");
    }
}


/**
 * Realiza el post-procesamiento de la carga de resultados, incluyendo ajustes en la vista, ejecución de scripts, y otras configuraciones.
 * 
 * @param {string} paramAdicional - Parámetros adicionales para determinar el tipo de vista (mapa, gráfico, etc.).
 * @param {string} funcionJS - Código JavaScript adicional a ejecutar después de cargar los resultados.
 * @param {number} pNumeroResultados - Número de resultados de la búsqueda.
 * @param {string} pPanelID - ID del panel donde se mostrarán los resultados.
 * 
 * @returns {void}
 */
function FinalizarMontarResultados(paramAdicional, funcionJS, pNumeroResultados, pPanelID) {

    var vistaMapa = (paramAdicional.indexOf('busquedaTipoMapa=true') != -1);
    var vistaChart = (paramAdicional.indexOf('busquedaTipoChart=') != -1);

    if (pNumeroResultados == 1) {
        if (typeof googleAnalyticsActivo !== "undefined" && googleAnalyticsActivo !== null && googleAnalyticsActivo) {
            RegistrarVisitaGoogleAnalytics();
        }
    }

    $('#divFiltrar').hide();
    $(pPanelID).show();

    $('#' + panFiltrosPulgarcito).css('display', '');

    if (pNumeroResultados == 1) { OcultarUpdateProgress(); MontarNumResultados(); }

    if (funcionJS != '') {
        eval(funcionJS);
    }

    if (tipoBusqeda == 12 || tipoBusqeda == 11) {
        var vistaCompacta = $('#col02 .resource-list.compactview').length > 0;
        $('#col02 .resource-list').find('.resource').each(function (indice) {
            var mensaje = $(this);
            var utils = mensaje.find('.utils-2');
            var acciones = mensaje.find('.acciones');
            utils.show();
            acciones.show();
            mensaje.removeClass('over');
            if (vistaCompacta) { acciones.hide(); }
            mensaje.hover(
                function () {
                    $(this).removeClass('over');
                    utils.show();
                    acciones.show();
                },
                function () {
                    $(this).removeClass('over');
                    utils.show();
                    acciones.show();
                }
            );
        });
    }
    enlazarFiltrosBusqueda();

    if (typeof listadoMensajesMostrarAcciones != 'undefined') {
        /* Lanzar el evento de las imagenes */
        listadoMensajesMostrarAcciones.init();
    }

    /* Es listado de mensajes */
    if (tipoBusqeda == 12) {
        listado = $('#col02 .listadoMensajes .resource-list.compactview');
        if (listado.length > 0) {
            listadoMensajesMostrarAcciones.init();
            $('#col02 .listadoMensajes .resource-list .resource h4 a').each(function (indice) {
                var enlaceTitulo = $(this);
                var asunto = enlaceTitulo.attr("title");

                if (asunto != null && asunto.length > 33) {
                    enlaceTitulo.text(asunto.substring(0, 30) + "...");
                }
            });
        }
    }
    else if ((typeof customizarListado != 'undefined')) {
        /* En los listados de Inevery, hay que ejecutar este script */
        customizarListado.init();
    }
    
    if ((typeof CompletadaCargaRecursos != 'undefined')) {
        CompletadaCargaRecursos();
    }

    utilMapas.AjustarBotonesVisibilidad();

    //Si hay resultados mostramos la caja de b?squeda
    if ((document.getElementById('ListadoGenerico_panContenedor') != null || vistaMapa || vistaChart) && document.getElementById('ctl00_ctl00_CPH1_CPHContenido_divCajaBusqueda') != null) {
        document.getElementById('ctl00_ctl00_CPH1_CPHContenido_divCajaBusqueda').style.display = "";
    }

    if (typeof scriptBusqueda != 'undefined') {
        if (scriptBusqueda != null && scriptBusqueda != "") {
            eval(scriptBusqueda);
        }
    }

    if (pNumeroResultados == 1 && funcionExtraResultados != "") {
        eval(funcionExtraResultados);
    }

    if (typeof (urlCargarAccionesRecursos) != 'undefined') {
        ObtenerAccionesListadoMVC(urlCargarAccionesRecursos);
    }    


}


/**
 * Trae la información adicional sobre un punto en el mapa y muestra el contenido en un panel o infowindow.
 * 
 * @param {Object} me - Objeto del marcador en el mapa que tiene el infowindow. Si es null, se actualiza un panel en lugar del infowindow.
 * @param {string} panelVerMas - ID del panel donde se mostrará la información adicional si `me` es null.
 * @param {string} claveLatLog - Clave para la latitud y longitud del punto en el mapa.
 * @param {string} docIDs - Identificadores de documentos a incluir en la solicitud.
 * @param {string} docIDsExtra - Identificadores de documentos adicionales para la solicitud.
 * @param {number} panelVerMasX - Posición X del panel si `me` es null.
 * @param {number} panelVerMasY - Posición Y del panel si `me` es null.
 * @param {string} tipo - Tipo de clase CSS a aplicar al panel de "ver más".
 * 
 * @returns {void}
 */
function TraerRecPuntoMapa(me, panelVerMas, claveLatLog, docIDs, docIDsExtra, panelVerMasX, panelVerMasY, tipo) {
    if (me == null) {
        $('#' + panelVerMas).html('<div><p>' + form.cargando + '...</p></div>');
    }

    var docIDsExtra = '';

    if (((docIDs.length + 1) / 37) > 10) {
        docIDsExtra = docIDs.substring(37 * 10)
        docIDs = docIDs.substring(0, 37 * 10);
    }

    var servicio = new WS($('input.inpt_UrlServicioResultados').val(), WSDataType.jsonp);

    var metodo = 'ObtenerFichaRecurso';
    var params = {};
    params['bool_usarMasterParaLectura'] = bool_usarMasterParaLectura;
    params['proyecto'] = $('input.inpt_proyID').val();
    params['bool_esMyGnoss'] = $('input.inpt_bool_esMyGnoss').val() == 'True';
    params['bool_estaEnProyecto'] = $('input.inpt_bool_estaEnProyecto').val() == 'True';
    params['bool_esUsuarioInvitado'] = $('input.inpt_bool_esUsuarioInvitado').val() == 'True';
    params['identidad'] = $('input.inpt_identidadID').val();
    params['parametros'] = '';
    params['languageCode'] = $('input.inpt_Idioma').val();
    params['tipoBusqueda'] = tipoBusqeda;
    params['filtroContexto'] = filtroContexto;
    params['parametros_adiccionales'] = parametros_adiccionales;
    params['cont'] = contResultados;
    params['documentoID'] = docIDs;


    servicio.call(metodo, params, function (data) {
        if (docIDsExtra != '') {
            var idRecMas = guidGenerator();
            data += '<div id="mapaMasRec_' + idRecMas + '" class="verMasMapa"><a href="javascript:TraerRecPuntoMapa(null, \'mapaMasRec_' + idRecMas + '\', \'' + claveLatLog + '\',\'' + docIDsExtra + '\')">' + form.vermas + '</a></div>';
        }

        if (me != null) {
            me.infowindow.setContent(data);
            $('.fichaMapa').parent().css('overflow', 'hidden');
        }
        else {
            $('#' + panelVerMas).html(data);
            if (panelVerMasX != null && panelVerMasY != null) {
                $('#' + panelVerMas).removeAttr('class');
                if (tipo != null) {

                    $('#' + panelVerMas).addClass(tipo);
                }

                $('#' + panelVerMas).attr('posx', panelVerMasX);
                $('#' + panelVerMas).attr('posy', panelVerMasY);

                $('#' + panelVerMas).css("top", '0px');
                $('#' + panelVerMas).css("left", '0px');

                ReposicionarFichaMapa();

                if ($('#' + panelVerMas).attr('activo') == 'true') {
                    $('#' + panelVerMas).show();
                }

                if ((typeof CompletadaCargaFichaMapaComunidad != 'undefined')) {
                    CompletadaCargaFichaMapaComunidad();
                }
            }
        }
    });
}

/**
 * Reposiciona el panel de información en el mapa para que se ajuste a la ventana del navegador.
 * Asegura que el panel sea visible y no se desborde de la ventana.
 * 
 * @returns {void}
 */
function ReposicionarFichaMapa() {
    var fichaMapa = $('#' + utilMapas.fichaMapa);
    var posX = 0;
    var posY = 0;
    posX = parseFloat(fichaMapa.attr('posx'));
    posY = parseFloat(fichaMapa.attr('posy'));

    //Anchura y altura del panel que mostramos
    var anchura = fichaMapa.width();
    var altura = fichaMapa.height();

    //Anchura y altura de la ventana que estamos viendo
    var centroNavegadorVentanaX = $(window).width() / 2;
    var centroNavegadorVentanaY = $(window).height() / 2;

    //Coordenadas relativas punto
    var panelVerMasXRelativo = posX - $(window).scrollLeft();
    var panelVerMasYRelativo = posY - $(window).scrollTop();

    var margenX = 35;
    var margenY = 40;
    if (panelVerMasXRelativo > centroNavegadorVentanaX) {
        posX = posX - anchura - margenX;
        fichaMapa.addClass('indicaDerecha');
    } else {
        posX = posX + margenX;
        fichaMapa.addClass('indicaIzquierda');
    }

    if (panelVerMasYRelativo > centroNavegadorVentanaY) {
        posY = posY - altura + margenY;
        fichaMapa.addClass('indicaInferior');
    } else {
        posY = posY - margenY;
        fichaMapa.addClass('indicaSuperior');
    }

    fichaMapa.css("left", posX + 'px');
    fichaMapa.css("top", posY + 'px');
    fichaMapa.css("z-index", '99999');

}


/**
 * Actualiza la vista de la página para mostrar un gráfico específico y aplica un filtro basado en facetas.
 * Cambia el estado de la vista a `chartView` y guarda el ID del gráfico activo junto con el JavaScript asociado.
 *
 * @param {string} pCharID - El ID del gráfico que se debe mostrar.
 * @param {string} pJsChart - El JavaScript asociado al gráfico que se debe ejecutar.
 * 
 * @returns {void}
 */
function SeleccionarChart(pCharID, pJsChart) {
    $('.listView').attr('class', 'listView');
    $('.gridView').attr('class', 'gridView');
    $('.mapView').attr('class', 'mapView');
    $('.mosaicView').attr('class', 'mosaicView');

    $('.chartView').each(function () {
        if (!$(this).hasClass('activeView')) {
            $(this).addClass('activeView');
        }
    });

    chartActivo = pCharID;
    jsChartActivo = pJsChart;

    FiltrarPorFacetas(ObtenerHash2());
}

/**
 * Configura el panel de gráficos y aplica filtros basados en facetas.
 * Actualiza la vista a `chartView`, calcula el tamaño fijo de los gráficos, y filtra los datos según las facetas.
 *
 * @returns {void}
 */
function SeleccionarDashboard() {
    var json = jsonAsistente;

    var data = json;
    var asistentes = data['$values'];

    $('#panResultados').html('<div id="divCharts" class="chartView activeView"></div>');

    tamFijo = ($('#panResultados')[0].clientWidth - 40) / 3;

    for (var i = 0; i < asistentes.length; i++) {
        asistente = asistentes[i];
        chartActivo = asistente.key;

        FiltrarPorFacetas(ObtenerHash2());
        noGrafico = false;
    }
    noGrafico = true;
}


/**
 * Envía un formulario para exportar una búsqueda a un formato específico.
 * La función valida los parámetros y establece los valores necesarios antes de enviar el formulario de exportación.
 *
 * @param {string} pExportacionID - ID de la exportación, utilizado para identificar la exportación en el servidor.
 * @param {string} pNombreExportacion - Nombre de la exportación, para describir el archivo exportado.
 * @param {string} pFormatoExportacion - Formato de exportación deseado (por ejemplo, 'CSV', 'PDF').
 * @returns {void}
 */
function ExportarBusqueda(pExportacionID, pNombreExportacion, pFormatoExportacion) {
    if (pExportacionID != "" && pNombreExportacion != "" && $('#ParametrosExportacion').length > 0 && $('#FormExportarBusqueda').length > 0 && pFormatoExportacion != "") {
        $('#ParametrosExportacion').val(pExportacionID + '|' + pNombreExportacion + '|' + pFormatoExportacion);
        $('#FormExportarBusqueda').submit();
    }
}


/**
 * Configura y envía una solicitud para cargar y mostrar facetas en la interfaz de usuario basada en los filtros y otros parámetros proporcionados.
 * 
 * @param {string} pFiltros - Filtros aplicados a la búsqueda, reemplaza '&' por '|'.
 * @param {boolean} pPrimeraCarga - Indica si es la primera vez que se cargan las facetas.
 * @param {number} pNumeroFacetas - Número de facetas a cargar.
 * @param {string} pPanelID - ID del panel donde se mostrarán las facetas.
 * @param {string|null} pFaceta - Faceta específica a cargar, puede incluir '|vermas'.
 * @param {string} pTokenAfinidad - Token de afinidad para la sesión actual.
 */
function MontarFacetas(pFiltros, pPrimeraCarga, pNumeroFacetas, pPanelID, pFaceta, pTokenAfinidad) {
    pFiltros = pFiltros.replace(/&/g, '|');
    if (mostrarFacetas) {
        //contFacetas = contFacetas + 1;

        var verMas = false;

        if (pFaceta != null && pFaceta.indexOf('|vermas') != -1) {
            verMas = true;
            pFaceta = pFaceta.substring(0, pFaceta.lastIndexOf('|'));
        }

        var paramAdicional = parametros_adiccionales;

        if ($('.mapView').attr('class') == "mapView activeView") {
            paramAdicional += 'busquedaTipoMapa=true';
        }

        var metodo = 'CargarFacetas';
        var params = {};


        params['pProyectoID'] = $('input.inpt_proyID').val();
        params['pEstaEnProyecto'] = $('input.inpt_bool_estaEnProyecto').val() == 'True';
        params['pEsUsuarioInvitado'] = $('input.inpt_bool_esUsuarioInvitado').val() == 'True';
        params['pIdentidadID'] = $('input.inpt_identidadID').val();
        params['pParametros'] = '' + pFiltros.replace('#', '');
        params['pLanguageCode'] = $('input.inpt_Idioma').val();
        params['pPrimeraCarga'] = pPrimeraCarga.toString().toLowerCase() == "true";
        params['pAdministradorVeTodasPersonas'] = adminVePersonas.toString().toLowerCase() == "true";
        params['pTipoBusqueda'] = tipoBusqeda;
        params['pGrafo'] = grafo;
        params['pFiltroContexto'] = filtroContexto;
        if (typeof urlBusqueda != "undefined") {
            params['pUrlPaginaActual'] = urlBusqueda;
        }
        params['pParametros_adiccionales'] = paramAdicional;
        params['pUbicacionBusqueda'] = ubicacionBusqueda;
        params['pNumeroFacetas'] = pNumeroFacetas;
        params['pUsarMasterParaLectura'] = bool_usarMasterParaLectura;
        params['pFaceta'] = pFaceta;
        params['tokenAfinidad'] = pTokenAfinidad;

        $.post(obtenerUrl($('input.inpt_UrlServicioFacetas').val()) + "/" + metodo, params, function (data) {
            if (decodeURI(pFiltros) == decodeURI(filtrosPeticionActual) || pFiltros == replaceAll(filtrosPeticionActual, '&', '|') || (typeof filtrosDeInicio !== "undefined" && (pFiltros == filtrosDeInicio || pFiltros == replaceAll(filtrosDeInicio, '|', '&'))) /*|| pFiltros.indexOf(filtrosPeticionActual) >= 0 Esto hace que al quitar filtros se amontonen facetas*/) {
                if (pFaceta == null && (pNumeroFacetas == 1 || pNumeroFacetas == 2)) {
                    MontarFacetas(pFiltros, pPrimeraCarga, pNumeroFacetas + 1, pPanelID, pFaceta, pTokenAfinidad);
                }
                var descripcion = data;
                if (descripcion.indexOf("class=\"box") != -1 && document.getElementById('facetaEncuentra') != null) {
                    document.getElementById('facetaEncuentra').style.display = '';
                }
                $(pPanelID).show();

                if (pFaceta != null && data != "" && pNumeroFacetas == -1) {
                    var divFacetaID = pFaceta.replace(/\:/g, '_').replace(/\@/g, '-');
                    var divFacetaID_out = '#out_' + divFacetaID;
                    if ($(divFacetaID_out).length > 0) {
                        $(divFacetaID_out).show();
                    }
                    else {
                        //No existe el panel out_faceta. Inserto en el panel original el contenido del resultante. 
                        $('#' + divFacetaID).replaceWith(data);
                    }
                }

                if (pFaceta == null || pFaceta == '' || pNumeroFacetas == 1) {
                    if (pNumeroFacetas == 1) {
                        $('#' + panFacetas).html('');
                    }
                    var panelFacetas = pPanelID;
                    if ($('#facetedSearch').length) {
                        panelFacetas = '#facetedSearch';
                        descripcion = $('<div>' + descripcion + '</div>').find("#facetedSearch").html();
                    }

                    if (pNumeroFacetas > 1 && $(".listadoAgrupado").length) {
                        panelFacetas = "#" + $(".listadoAgrupado").attr("aux");
                    }

                    if (pNumeroFacetas == 1) {
                        if (!descripcion.replace('<div id="facetedSearch" class="facetedSearch">', '').replace('</div>', '').replace('</div>', '').trim().startsWith('<div id="panelFiltros" style="display:none">')) {
                            $('#facetedSearch').css('display', 'block');
                        } else {
                            //Ocultamos el panel de facetas
                            if ($('#facetedSearch').length == 0) {
                                $('#facetedSearch').css('display', 'none')
                            }
                            $(panelFacetas).css('display', 'none')
                        }
                    }
                    $(panelFacetas).append(descripcion);
                }
                else {
                    //Si viene el parámetro pFaceta, se está rellenando una faceta, hay que sustituir el contenido anterior por el actual. 

                    if (verMas) {
                        descripcion = $('#' + divFacetaID, $(data.trim())).html();
                        var htmlVerMas = $('p.moreResults', $(pPanelID)).html();
                        if (typeof (htmlVerMas) != 'undefined') {
                            htmlVerMas = htmlVerMas.substring(0, htmlVerMas.indexOf('>') + 1) + form.vermenos + '</a>';
                            descripcion += '<p class="moreResults">' + htmlVerMas + '</p>';
                        }
                        if (descripcion == null) {

                            descripcion = data.trim();
                        }
                    }

                    if (descripcion.indexOf('id="' + pPanelID.substr(1) + '"') != -1) {
                        //La descripción ya contiene el panel, lo sustituyo.
                        $(pPanelID).replaceWith(descripcion);
                    }
                    else {
                        $(pPanelID).html(descripcion);
                    }
                }

                if (pNumeroFacetas == 1) { MontarPanelFiltros(); }
                /* presentacion facetas */
                // Longitud facetas por CSS
                // limiteLongitudFacetas.init();

                //}

                if (pNumeroFacetas == 3) {
                    if ($(".filtroFacetaFecha").length > 0) {

                        $(".filtroFacetaFecha").datepicker();
                    }

                    if ($("div.divdatepicker").length > 0) {
                        IniciarFacetaCalendario();
                    }
                } else if (pNumeroFacetas == -1 && $("div.divdatepicker").length > 0) {
                    IniciarFacetaCalendario(pFiltros);
                }


                if (enlazarJavascriptFacetas) {
                    enlazarFacetasBusqueda();
                }
                else {
                    enlazarFacetasNoBusqueda();
                }

                if ((typeof CompletadaCargaFacetas != 'undefined')) {
                    /* En los listados de Inevery, hay que ejecutar este script */
                    CompletadaCargaFacetas();
                }

                if (funcionExtraFacetas != "") {
                    eval(funcionExtraFacetas);
                }
            }
        });
    }
    else {
        var col1 = document.getElementById('col01');
        if (col1 != null) {
            $('#col01').css('display', 'none');
            $('#col02').css('float', 'left');
        }
    }
    primeraCargaDeFacetas = false;
}

/**
 * Inicializa el componente de selección de fechas (datepicker) y configura los eventos para manejar la selección de fechas y semanas.
 *
 * @param {string} [fechaInicioCalendario] - Fecha inicial para el calendario, en formato 'YYYYMMDD'.
 */
function IniciarFacetaCalendario(fechaInicioCalendario) {

    //Cogemos los eventos
    $("div.divdatepicker").each(function () {
        var events = new Array();
        var i = 0;
        $(this).parent().children('ul').children().each(function () {
            var fecha = $(this).children('a').attr('title');
            var filtro = $(this).children('a').attr('name');

            fecha = fecha.substring(fecha.indexOf('=') + 1);
            var agno = parseInt(fecha.substr(0, 4));
            var mes = parseInt(fecha.substr(4, 2)) - 1;
            var dia = parseInt(fecha.substr(6, 2));
            events[i] = { Title: filtro, Date: new Date(agno, mes, dia, 0, 0, 0, 0) };
            i++;
        });


        var changingDate = false;
        var weekSelected = false;

        //Iniciamos el datepicker
        $(this).datepicker({
            showWeek: true,
            beforeShowDay: function (date) {
                //Revisamos si coincide
                var result = [true, '', null];
                var matching = $.grep(events, function (event) {
                    return event.Date.valueOf() === date.valueOf();
                });

                if (matching.length) {
                    result = [true, 'css-class-to-highlight', ''];
                }

                return result;
            },
            onSelect: function (dateText) {
                if (!weekSelected) {
                    var date,
                        i = 0,
                        event = null;
                    var day = parseInt(dateText.substr(0, 2));
                    var month = parseInt(dateText.substr(3, 2)) - 1;
                    var year = parseInt(dateText.substr(6, 4));
                    selectedDate = new Date(year, month, day, 0, 0, 0, 0);

                    /* Determine if the user clicked an event: */
                    while (i < events.length && !event) {
                        date = events[i].Date;

                        if (selectedDate.valueOf() === date.valueOf()) {
                            event = events[i];
                        }
                        i++;
                    }
                    if (event != null) {
                        AgregarFaceta(event.Title + '|replace');
                    }
                }
            },
            onChangeMonthYear: function (year, month) {
                if (changingDate) {
                    return;
                }
                //Lamamos al servicio de facetas para que recargue los elementos del mes siguiente.
                VerFechasSiguienteMes($(this).attr('name'), $(this).attr('name').replace(':', '_'), year, month);
            }
        });

        if (typeof (fechaInicioCalendario) != 'undefined' && fechaInicioCalendario != '') {

            var filtrosArray = fechaInicioCalendario.split('|');
            var fecha = '';
            for (var i = 0; i < filtrosArray.length; i++) {
                if (filtrosArray[i] != '' && filtrosArray[i].indexOf($(this).attr('name')) >= 0) {
                    fecha = filtrosArray[i].substr($(this).attr('name'));
                }
            }

            fecha = fecha.substr(fecha.indexOf('=') + 1) + '00';

            var agno = fecha.substr(0, 4);
            var mes = fecha.substr(4, 2);
            var dia = fecha.substr(6, 2);
            var inicio = new Date(agno, mes, dia, 0, 0, 0, 0);

            changingDate = true;
            $(this).datepicker("setDate", inicio);
            changingDate = false;
        }

        //Cargar elementos siguientes
        //Dar valor a los nuevas columnas
        $("td.ui-datepicker-week-col").on("click", function () {
            weekSelected = true;

            //Me filtra por el d?a al que le hago click...
            $(this).next().click();

            weekSelected = false;

            var fromDate = $("div.divdatepicker").datepicker("getDate");
            var toDate = $("div.divdatepicker").datepicker("getDate");
            toDate.setDate(toDate.getDate() + 6);

            var monthFrom = fromDate.getMonth() + 1;
            var monthTo = toDate.getMonth() + 1;
            if (monthFrom <= 9) {
                monthFrom = '0' + monthFrom;
            }
            if (monthTo <= 9) {
                monthTo = '0' + monthTo;
            }

            var dateFrom = fromDate.getDate();
            var dateTo = toDate.getDate();
            if (dateFrom <= 9) {
                dateFrom = '0' + dateFrom;
            }

            if (dateTo <= 9) {
                dateTo = '0' + dateTo;
            }

            var filtroFrom = fromDate.getFullYear() + '' + monthFrom + '' + dateFrom;
            var filtroTo = toDate.getFullYear() + '' + monthTo + '' + dateTo;
            var filtroSemantico = $("div.divdatepicker").attr('name');
            AgregarFaceta(filtroSemantico + '=' + filtroFrom + '-' + filtroTo + '|replace');
        });

        $('div.ui-datepicker-title').on('click', function () {
            var date = $("div.divdatepicker").datepicker("getDate");
            var year = date.getFullYear();
            var month = date.getMonth() + 1;
            var dia = new Date(year, month, 0).getDate();

            if (month <= 9) {
                month = '0' + month;
            }
            if (dia <= 9) {
                dia = '0' + dia;
            }

            var filtro = year + '' + month + '01' + "-" + year + '' + month + '' + dia;
            var filtroSemantico = $("div.divdatepicker").attr('name');
            AgregarFaceta(filtroSemantico + '=' + filtro + '|replace');
        });

    });
}

/**
 * Configura y envía una solicitud para cargar facetas de búsqueda para el siguiente mes, actualizando los filtros aplicados.
 *
 * @param {string} faceta - Faceta específica a actualizar.
 * @param {string} controlID - ID del control donde se mostrarán los resultados.
 * @param {number} year - Año del siguiente mes.
 * @param {number} month - Mes a cargar (1-12).
 */
function VerFechasSiguienteMes(faceta, controlID, year, month) {
    var filtros = ObtenerHash2();
    if (month <= 9) {
        month = '0' + month;
    }

    //Usamos el m?todo de limpiar filtro para limpiar el filtro que quramos.
    if (filtros.indexOf(faceta) >= 0) {
        filtros.split('&');
        var filtrosArray = filtros.split('&');
        filtros = '';

        for (var i = 0; i < filtrosArray.length; i++) {
            if (filtrosArray[i] != '' && filtrosArray[i].indexOf(faceta) == -1) {
                filtros += filtrosArray[i] + '&';
            } else if (filtrosArray[i] != '' && filtrosArray[i].indexOf('|') >= 0 && filtrosArray[i].indexOf(faceta) >= 0) {
                filtros += filtrosArray[i].substring(filtrosArray[i].indexOf('|'));
            }
        }
    }

    filtros += "|" + faceta + "=" + year + month;

    filtrosPeticionActual = "|" + faceta + "=" + year + month;

    MontarFacetas(filtros, false, -1, '#' + controlID, faceta + '|vermas');
}


/**
 * Finaliza el proceso de montaje de facetas, inicializando la búsqueda facetada y configurando los filtros y eventos necesarios.
 */
function FinalizarMontarFacetas() {
    MontarPanelFiltros();

    if (enlazarJavascriptFacetas) {
        enlazarFacetasBusqueda();
    }
    else {
        enlazarFacetasNoBusqueda();
    }

    if (HayFiltrosActivos(filtrosPeticionActual)) {
        $('#' + divFiltros).css('display', '');
        $('#' + divFiltros).css('padding-top', '0px !important');
        //$('#' + divFiltros).css('margin-top', '10px');
    }

    if ((typeof CompletadaCargaFacetas != 'undefined')) {
        /* En los listados de Inevery, hay que ejecutar este script */
        CompletadaCargaFacetas();
    }
}

$(document).ready(function () {
    $('.searchGroup .encontrar').click(function (event) {
        var txt = $(this).parent().find('.text');
        if ($(this).parent().find('#criterio').attr('origen') != undefined) {
            //Buscadores CMS
            var criterio = $(this).parent().find('#criterio').attr('origen');
            window.location.href = ObtenerUrlBusqueda(criterio) + txt.val();
            return false;
        }
        if (txt.hasClass('text') && txt.val() != '') {
            var nombreSearch = $('.inpt_searchPersonalizadoActivo').val();
            if (nombreSearch == "" || nombreSearch == null || nombreSearch == "search") {
                nombreSearch = "search";
            }
            //Resto de buscadores
            window.location.href = $('input.inpt_baseUrlBusqueda').val() + '/recursos?' + nombreSearch +'=' + txt.val();
            return false;
        }
        return false;
    });

    // Buscador CMS
    $('.searchGroup .encontrar_cms').click(function (event) {
        // Recoger el contenido para iniciar búsqueda
        const txt = $(this).parent().find('.text');
        if ($(this).parent().find('#criterio').attr('origen') != undefined) {
            //Buscadores CMS
            // Hay datos relativos a un proyecto --> Realizar búsqueda disparando el submit del formulario
            const btnSubmitBuscadorCMS = txt.parent().find(".encontrar_cms_submit");
            // Formulario de búsqueda
            const form = $(this).parents(".encontrar_cms_form");
            const actionUrl = form.attr("action");
            // Hacer submit del formulario del formulario para hacer petición POST de búsqueda
            //btnSubmitBuscadorCMS.trigger("click");                   
            // Acceder a la página construyendo la url más la búsqueda
            // window.location.href = `${actionUrl}?search=${encodeURIComponent(txt.val())}`;
            const criterio = $(this).parent().find('#criterio').attr('origen');
            window.location.href = ObtenerUrlBusqueda(criterio) + txt.val();
        }
    });

    // Formulario CMS
    $(".encontrar_cms_form").on("submit", function (e) {
        e.preventDefault();
        // Ejecutar comportamiento de click en la lupa de búsqueda
        $('.searchGroup .encontrar_cms').trigger("click");
        return false;
    });

    $('.searchGroup .text').keydown(function (event) {
        if ($(this).val().indexOf('|') > -1) {
            $(this).val($(this).val().replace(/\|/g, ''));
        };
        if (event.which || event.keyCode) {
            if ((event.which == 13) || (event.keyCode == 13)) {
                $(this).parent().find('.encontrar').click();
                return false;
            }
        } else {
            return true;
        };
    });

    $('.aaCabecera .searchGroup .text')
        .unbind()
        .keydown(function (event) {
            if ($(this).val().indexOf('|') > -1) {
                $(this).val($(this).val().replace(/\|/g, ''));
            };
            if (!$(this).hasClass('ac_input') && (event.which || event.keyCode)) {
                if ((event.which == 13) || (event.keyCode == 13)) {
                    $(this).parent().find('.encontrar').click();
                    return false;
                }
            } else {
                return true;
            };
        });

    $('.searchGroup .text').focus(function (event) {
        if ($(this).attr('class') == 'text defaultText') {
            $(this).attr('class', 'text');
            $(this).attr('value', '');
        }
    });

    //Buscador de la cabecera superior - Buscador de algunos buscadores (Comentarios, Mensajes...)
    $('.searchGroup .encontrar')
        .unbind()
        .click(function (event) {
            const input = $(this).parent().find('.text');
            if (input.hasClass("input_buscador_cms")) {
                // Busqueda a realizar vía Caja CMS
                $(".encontrar_cms_form").trigger("submit");
            } else {
                var ddlCategorias = $('.fieldsetGroup .ddlCategorias').val();
                var url = ObtenerUrlBusqueda(ddlCategorias);
                if (typeof ($('.tipoBandeja').val()) != 'undefined' && ddlCategorias == 'Mensaje') {
                    //Es una búsqueda en la bandeja de mensajes
                    url = url.replace('search=', $('.tipoBandeja').val() + '&search=')
                }
                var parametros = $('.searchGroup .text').val();
                var autocompletar = $('.ac_results .ac_over');
                var nombreSearch = $('.inpt_searchPersonalizadoActivo').val();
                if (nombreSearch == "" || nombreSearch == null || nombreSearch == "search") {
                    nombreSearch = "search";
                }

                if (typeof (autocompletar) != 'undefined' && autocompletar.length > 0 && typeof ($('.ac_results .ac_over')[0].textContent) != 'undefined') {

                    parametros = $('.ac_results .ac_over')[0].textContent;
                }

                if (parametros == '') {
                    url = url.replace('?' + nombreSearch +'=', '').replace('/tag/', '');
                }
                window.location.href = url + parametros;
            }
        });
});


$(document).ready(function () {
    if ($('#finderSection').length > 0) {
        var urlPaginaActual = $('.inpt_urlPaginaActual').val();
        if (typeof (urlPaginaActual) != 'undefined') {
            $('#inputLupa').click(function (event) {
                // Ejecución de búsqueda
                const searchString = escapeHTML($('#finderSection').val());

                var nombreSearch = $('.inpt_searchPersonalizadoActivo').val();
                if (nombreSearch == "" || nombreSearch == null || nombreSearch == "search") {
                    nombreSearch = "search";
                }

                window.location.href = urlPaginaActual + "?"+nombreSearch+"=" + encodeURIComponent(searchString); 
            });
        }

        $('#finderSection').keydown(function (event) {
            if ($(this).val().indexOf('|') > -1) {
                $(this).val($(this).val().replace(/\|/g, ''));
            };
            if (event.which || event.keyCode) {
                if ((event.which == 13) || (event.keyCode == 13)) {
                    $(this).parent().find('.findAction').click();
                    return false;
                }
            } else {
                return true;
            };
        });

        if (typeof (origenAutoCompletar) == 'undefined') {
            origenAutoCompletar = ObtenerOrigenAutoCompletarBusqueda($('input.inpt_tipoBusquedaAutoCompl').val());
            if (origenAutoCompletar == '') {
                var pathName = window.location.pathname;

                pathName = pathName.substr(pathName.lastIndexOf('/') + 1);
                if (pathName.indexOf('?') > 0) {
                    pathName = pathName.substr(0, pathName.indexOf('?'));
                }

                origenAutoCompletar = pathName;
            }
        }

        //var tablaPropiaAutoCompletar = $('input.inpt_tablaPropiaAutoCompletar').val().toLowerCase() == 'true';
        var urlServicioAutocompletar = $('.inpt_urlServicioAutocompletar').val();
        if (urlServicioAutocompletar.indexOf('autocompletarEtiquetas') > 0) {
            var proyID = $('.inpt_proyID').val();
            var facetasBusqPag = $('.inpt_facetasBusqPag').val();
            var identidadID = $('.inpt_identidadID').val();

            $('#finderSection').autocomplete(
			null,
			{
			    servicio: new WS(urlServicioAutocompletar, WSDataType.jsonp),
                metodo: 'AutoCompletarTipado',
                //url: urlServicioAutocompletar + "/AutoCompletarTipado",
                //type: "POST",
			    delay: 0,
			    scroll: false,
			    selectFirst: false,
			    minChars: 1,
			    width: 'auto',
			    max: 25,
			    cacheLength: 0,
			    extraParams: {
			        pProyecto: proyID,
			        //pTablaPropia: tablaPropiaAutoCompletar,
			        pFacetas: facetasBusqPag,
			        pOrigen: origenAutoCompletar,
			        pIdentidad: $('input.inpt_identidadID').val(),
			        pIdioma: $('input.inpt_Idioma').val(),
			        maxwidth: '420px',
			        botonBuscar: 'inputLupa'
			    }
			}
			);
        } else {
            var proyID = $('.inpt_proyID').val();
            var facetasBusqPag = $('.inpt_facetasBusqPag').val();
            var identidadID = $('.inpt_identidadID').val();
            var bool_esMyGnoss = $('.inpt_bool_esMyGnoss').val() == 'True';
            var bool_estaEnProyecto = $('.inpt_bool_estaEnProyecto').val() == 'True';
            var bool_esUsuarioInvitado = $('input.inpt_bool_esUsuarioInvitado').val() == 'True';
            var orgID = $('input.inpt_organizacionID').val();
            var perfilID = $('input#inpt_perfilID').val();
            var parametros = $('.inpt_parametros').val();
            var tipo = $('.inpt_tipoBusquedaAutoCompl').val();
            var facetasBusqPag = $('.inpt_facetasBusqPag').val();
            $('#finderSection').autocomplete(
				null,
				{
				    //servicio: new WS(urlServicioAutocompletar, WSDataType.jsonp),
				    //metodo: 'AutoCompletarFacetas',
                    url: urlServicioAutocompletar + "/AutoCompletarFacetas",
                    type: "POST",
				    delay: 0,
				    minLength: 4,
				    scroll: false,
				    selectFirst: false,
				    minChars: 4,
				    width: 190,
				    cacheLength: 0,
				    extraParams: {
				        proyecto: proyID,
				        bool_esMyGnoss: bool_esMyGnoss == true,
				        bool_estaEnProyecto: bool_estaEnProyecto == true,
				        bool_esUsuarioInvitado: bool_esUsuarioInvitado == true,
				        identidad: identidadID,
				        organizacion: orgID,
				        filtrosContexto: '',
				        languageCode: $('input.inpt_Idioma').val(),
				        perfil: perfilID,
				        //pTablaPropia: tablaPropiaAutoCompletar,
				        pFacetas: facetasBusqPag,
				        pOrigen: origenAutoCompletar,
				        nombreFaceta: 'search',
				        orden: '',
				        parametros: parametros,
				        tipo: tipo,
				        botonBuscar: 'inputLupa'
				    }
				}
			);
        }
    }
});

$(document).ready(function () {
    PreparaAutoCompletarComunidad();
});

/**
 * Configura el autocompletado para todos los campos de entrada con la clase `autocompletar`.
 * El autocompletado se basa en el origen, facetas, y parámetros específicos del contexto de la búsqueda.

 * @returns {void}
 */
function PreparaAutoCompletarComunidad() {
    $('input.autocompletar').each(function () {
        var txtBusqueda = this;
        var ddlCategorias = $(this).parent().parent().find('.ddlCategorias');
        var pOrigen = '';
        tipoAutocompletar = -1;
        var pTipoDdlCategorias = '';
        var facetasAutoComTip = '';
        if ($(this).attr('origen') != undefined) {
            pTipoDdlCategorias = $(this).attr('origen');
            pOrigen = ObtenerOrigenAutoCompletarBusqueda($(this).attr('origen'));
            tipoAutocompletar = ObtenerTipoAutoCompletarBusqueda($(this).attr('origen'));
            facetasAutoComTip = ObtenerFacetasAutocompletar($(this).attr('origen'));
        } else if (typeof (ddlCategorias.val()) != 'undefined' && ddlCategorias.val() != '') {
            pOrigen = ObtenerOrigenAutoCompletarBusqueda(ddlCategorias.val());
            facetasAutoComTip = ObtenerFacetasAutocompletar(ddlCategorias.val());
            tipoAutocompletar = ObtenerTipoAutoCompletarBusqueda(ddlCategorias.val());
            pTipoDdlCategorias = ddlCategorias.val();
        } else if (typeof ($('input.inpt_tipoBusquedaAutoCompl').val()) != 'undefined') {
            if ($('input.inpt_tipoBusquedaAutoCompl').val() != '') {
                pOrigen = origenAutoCompletar;
                facetasAutoComTip = ObtenerFacetasAutocompletar($('input.inpt_tipoBusquedaAutoCompl').val());
                tipoAutocompletar = ObtenerTipoAutoCompletarBusqueda($('input.inpt_tipoBusquedaAutoCompl').val());
            }
            pTipoDdlCategorias = $('input.inpt_tipoBusquedaAutoCompl').val();
        }

        //Este trozo hace que la caja de arriba de b?squeda intente buscar en la pesta?a donde est?s en vez de donde se indica. Adem?s de que a veces no se puede buscar en la pest? que te encuentras (Ej: Indice)
        /*if (pOrigen == '') {
            var pathName = window.location.pathname;

            pathName = pathName.substr(pathName.lastIndexOf('/') + 1);
            if (pathName.indexOf('?') > 0) {
                pathName = pathName.substr(0, pathName.indexOf('?'));
            }

            pOrigen = pathName;
        }*/

        var urlServicioAutocompletarEtiquetas = $('input.inpt_urlServicioAutocompletarEtiquetas').val();

        var identidadID = $('input.inpt_identidadID').val();
        var limitAutocomplete = 25;
        if ($(this).attr('gnoss-autocomplete-limit') != undefined) {
            limitAutocomplete = parseInt($(this).attr('gnoss-autocomplete-limit'));
        }
        var proyID = $('input.inpt_proyID').val();
        var organizacionID = $('input.inpt_organizacionID').val();
        var perfilID = $('input#inpt_perfilID').val();

        var btnBuscarID = '';
        // Size() deprecado
        //if ($(this).parent().find('.encontrar').size() > 0) {
        if ($(this).parent().find('.encontrar').length > 0) {
            btnBuscarID = $(this).parent().find('.encontrar').attr('id');
        } else {
            return;
        }

        var urlServicioAutocompletar = $('input.inpt_urlServicioAutocompletar').val();
        var bool_esMyGnoss = $('input.inpt_bool_esMyGnoss').val() == 'True';
        var bool_estaEnProyecto = $('input.inpt_bool_estaEnProyecto').val() == 'True';
        var bool_esUsuarioInvitado = $('input.inpt_bool_esUsuarioInvitado').val() == 'True';
        if (facetasAutoComTip == '') {

            facetasAutoComTip = $('input.inpt_FacetasProyAutoCompBuscadorCom').val();
        }
        //var tablaPropiaAutoCompletar = $('input.inpt_tablaPropiaAutoCompletar').val().toLowerCase() == "true";
        var autocompletarProyectoVirtuoso = $('input.inpt_AutocompletarProyectoVirtuoso').val();

        $(this).unautocomplete();
        if (ddlCategorias.val() != 'MyGNOSSMeta' && ddlCategorias.val() != 'Contribuciones en Recursos' && ddlCategorias.val() != 'RecursoPerfilPersonal' && ddlCategorias.val() != 'Mensaje' && ddlCategorias.val() != 'Blog' && ddlCategorias.val() != 'Comunidad' && (ddlCategorias.val() != 'PerYOrg' || proyID != '11111111-1111-1111-1111-111111111111') && typeof (autocompletarProyectoVirtuoso) == 'undefined') {
            $(this).autocomplete(
                null,
                {
                    servicio: new WS(urlServicioAutocompletarEtiquetas, WSDataType.jsonp),
                    metodo: 'AutoCompletarTipado',
                    //url: urlServicioAutocompletarEtiquetas + "/AutoCompletarTipado",
                    //type: "POST",
                    delay: 0,
                    scroll: false,
                    selectFirst: false,
                    minChars: 1,
                    width: 'auto',
                    max: limitAutocomplete,
                    cacheLength: 0,
                    extraParams: {
                        pProyecto: proyID,
                        //pTablaPropia: tablaPropiaAutoCompletar,
                        pFacetas: facetasAutoComTip,
                        pOrigen: pOrigen,
                        pTipoAutocompletar: tipoAutocompletar,
                        pIdentidad: identidadID,
                        pIdioma: $('input.inpt_Idioma').val(),
                        maxwidth: '389px',
                        botonBuscar: btnBuscarID
                    }
                });
        } else {
            $(this).autocomplete(
                null,
                {
                    //servicio: new WS(urlServicioAutocompletar, WSDataType.jsonp),
                    //metodo: 'AutoCompletarFacetas',
                    url: urlServicioAutocompletar + "/AutoCompletarFacetas",
                    type: "POST",
                    delay: 0,
                    minLength: 4,
                    scroll: false,
                    selectFirst: false,
                    minChars: 4,
                    width: 190,
                    cacheLength: 0,
                    extraParams: {
                        proyecto: proyID,
                        bool_esMyGnoss: bool_esMyGnoss == 'True',
                        bool_estaEnProyecto: bool_estaEnProyecto == 'True',
                        bool_esUsuarioInvitado: bool_esUsuarioInvitado == 'True',
                        identidad: identidadID,
                        organizacion: organizacionID,
                        filtrosContexto: '',
                        languageCode: $('input.inpt_Idioma').val(),
                        perfil: perfilID,
                        nombreFaceta: 'search',
                        orden: '',
                        parametros: '',
                        tipo: pTipoDdlCategorias,
                        botonBuscar: btnBuscarID
                    }
                });
        }
    });
}


/**
 * Obtiene la URL de búsqueda asociada a un tipo de búsqueda específico.
 * La función busca en los elementos de entrada con la clase `inpt_tipo_busqueda` para encontrar una URL que corresponda al tipo de búsqueda dado.
 * Si no se encuentra una coincidencia, devuelve la URL por defecto del último elemento.
 *   
 * @param {string} tipo - El tipo de búsqueda para el que se desea obtener la URL.
 * @returns {string} La URL de búsqueda asociada al tipo de búsqueda especificado, o la URL por defecto del último elemento si no se encuentra una coincidencia.
 */
function ObtenerUrlBusqueda(tipo) {
    var tamagno = $('input.inpt_tipo_busqueda').length;
    for (var i = 0; i < tamagno; i++) {
        var valor = ($('input.inpt_tipo_busqueda')[i]).value;
        if (valor.startsWith("ub_")) {
            valor = valor.substring(3);
        }
        if (valor.split('@')[0] == tipo) {
            return valor.split('@')[1];
        }
    }

    //Devolvemos el por defecto.
    return ($('input.inpt_tipo_busqueda')[tamagno - 1]).value.split('@')[1];
}


/**
 * Obtiene el origen de autocompletado de búsqueda asociado a un tipo de búsqueda específico.
 * La función busca en los elementos de entrada con la clase `inpt_OrigenAutocompletar` para encontrar un origen que corresponda al tipo de búsqueda dado.
 * Si no se encuentra una coincidencia, devuelve una cadena vacía como valor por defecto.
 * 
 * @param {string} tipo - El tipo de búsqueda para el que se desea obtener el origen de autocompletado.
 * @returns {string} El origen de autocompletado asociado al tipo de búsqueda especificado, o una cadena vacía si no se encuentra una coincidencia.
 */
function ObtenerOrigenAutoCompletarBusqueda(tipo) {
    var tamagno = $('input.inpt_OrigenAutocompletar').length;
    for (var i = 0; i < tamagno; i++) {
        var valor = ($('input.inpt_OrigenAutocompletar')[i]).value;
        if (valor.startsWith("oa_")) {
            valor = valor.substring(3);
        }
        if (valor.split('@')[0] == tipo) {
            return valor.replace(tipo + '@', '');
        }
    }

    //Devolvemos el por defecto.
    return '';
}

function ObtenerTipoAutoCompletarBusqueda(tipo) {
    var tamagno = $('input.inpt_OrigenAutocompletar').length;
    for (var i = 0; i < tamagno; i++) {
        var valor = ($('input.inpt_OrigenAutocompletar')[i]).value;
        if (valor.startsWith("oa_")) {
            valor = valor.substring(3);
        }
        if (valor.split('@')[0] == tipo) {
            if ($('input.inpt_OrigenAutocompletar')[i].dataset.tipoautocomplete != undefined) {
                var tipoAutocompletar = $('input.inpt_OrigenAutocompletar')[i].dataset.tipoautocomplete;
                if (Number.isInteger(parseInt(tipoAutocompletar))) {
                    return parseInt(tipoAutocompletar);
                }
                else {
                    return -1
                }
            }
            else {
                return -1;
            }
        }
    }

    //Devolvemos el por defecto.
    return -1;
}
/**
 * Obtiene las facetas de autocompletado asociadas a un tipo de búsqueda específico.
 * La función busca en los elementos de entrada con la clase `inpt_FacetasAutocompletar` para encontrar facetas que correspondan al tipo de búsqueda dado.
 * Si no se encuentra una coincidencia, devuelve una cadena vacía como valor por defecto.
 *
 * @function ObtenerFacetasAutocompletar
 * @param {string} tipo - El tipo de búsqueda para el cual se desean obtener las facetas de autocompletado.
 * @returns {string} Las facetas de autocompletado asociadas al tipo de búsqueda especificado, o una cadena vacía si no se encuentra una coincidencia.
 */
function ObtenerFacetasAutocompletar(tipo) {
    var tamagno = $('input.inpt_FacetasAutocompletar').length;
    for (var i = 0; i < tamagno; i++) {
        var valor = ($('input.inpt_FacetasAutocompletar')[i]).value;
        if (valor.startsWith("fa_")) {
            valor = valor.substring(3);
        }
        if (valor.split('@')[0] == tipo) {
            return valor.replace(tipo + '@', '');
        }
    }

    //Devolvemos el por defecto.
    return '';
}

/**
 * Monta el panel de filtros en la interfaz de usuario si se debe mostrar la caja de búsqueda.
 * La función transfiere el contenido del panel de filtros a otro elemento especificado por `panFiltrosPulgarcito`.
 * Luego, limpia el contenido del panel de filtros y asegura que ciertos elementos de la interfaz estén visibles.  
 * @returns {void} No devuelve ningún valor. Solo realiza modificaciones en el DOM.
 */
function MontarPanelFiltros() {
    if (mostrarCajaBusqueda) {
        if (document.getElementById('panelFiltros') != null && document.getElementById('panelFiltros').innerHTML != "" && document.getElementById(panFiltrosPulgarcito) != null) {
            $('#' + panFiltrosPulgarcito).html($('#panelFiltros').html());
            $('#panelFiltros').html('');

            $('.group.filterSpace').css('display', '');
            $('.searchBy').css('display', '');
            $('.tags').css('display', '');
        }
    }
}

/**
 * Actualiza el panel de resultados de búsqueda en la interfaz de usuario, incluyendo el manejo de resultados no encontrados y la visualización del número total de resultados.
 * La función transfiere el contenido del panel `numResultadosRemover` a otro elemento, muestra un mensaje si no se encuentran resultados, y gestiona la visibilidad de los elementos relevantes. 
 * @returns {void} No devuelve ningún valor. Solo realiza modificaciones en el DOM.
 */
function MontarNumResultados() {

    // Input para buscar
    const finderSection = $("#finderSection");
    // Panel informativo de resultados no encontrados 
    let panelInfoNoResultsFound = '';

    if ($('#' + idNavegadorBusqueda).length > 0) {
        if ($('#navegadorRemover').find('.indiceNavegacion').length > 0) {
            $('#' + idNavegadorBusqueda).html($('#navegadorRemover').html());
            $('#' + idNavegadorBusqueda).css('display', '')
        }
        $('#navegadorRemover').remove();
    }

    if ($('#numResultadosRemover').length > 0) {
        if (mostrarCajaBusqueda) {
            // Nº total de resultados obtenidos
            const numResultados = parseInt($('#numResultadosRemover').text());
            // Cadena utilizada para búsqueda
            var queryString = findGetParameter("search");

            // No se han encontrado resultados - Mostrar aviso siempre que se realice alguna búsqueda
            if (numResultados == 0 && queryString != undefined) {
                queryString = queryString.replace('<', '&lt;');
                queryString = queryString.replace('>', '&gt;');
                // Si existe el buscador, introducir la cadena que ha usado el usuario para realizar la búsqueda con resultados 0.
                if (finderSection.length > 0) {
                    finderSection.val(queryString);
                }

                // Panel informativo de que no se han encontrado resultados (Idioma ES y EN)
                if (configuracion.idioma == 'en') {
                    panelInfoNoResultsFound = `
                        <div id="no-results" class="d-flex">
	                        <span class="material-icons-outlined"> info </span>
	                        <div class="no-results-content ml-2">		                        
			                    <p>The search for <strong>${queryString}</strong> did not return any results.​</p>		                        
		                        <p>Suggestions:​</p>
		                        <ul>
                                    <li>Check that all words are spelled correctly.​</li>
			                        <li>Try using other word.​</li>
			                        <li>Try using more general words.​</li>
			                        <li>Try using fewer words.</li>
		                        </ul>
	                        </div>
                        </div>
                    `;
                } else {
                    panelInfoNoResultsFound = `
                        <div id="no-results" class="d-flex">
	                        <span class="material-icons-outlined"> info </span>
	                        <div class="no-results-content ml-2">
		                        <p>
			                        La búsqueda de <strong>${queryString}</strong> no obtuvo ningún resultado.
		                        </p>
		                        <p>Sugerencias:</p>
		                        <ul>
			                        <li>
				                        Comprueba que todas las palabras están escritas
				                        correctamente.
			                        </li>
			                        <li>Intenta usar otras palabras.</li>
			                        <li>Intenta usar palabras más generales.</li>
			                        <li>Prueba a usar menos palabras.</li>
		                        </ul>
	                        </div>
                        </div>
                    `;
                }

                // Ocultar panel de filtros o div filtros ya que no es necesario
                $('#divFiltros').length > 0 ? $('#divFiltros').css("display", "none") : $('#panFiltros').css("display", "none");

                // Añadir panel de sin resultados
                $(".resource-list-wrap").find("p").remove();
                $(".resource-list-wrap").append(panelInfoNoResultsFound);
            }
            $('#' + numResultadosBusq).html($('#numResultadosRemover').html());
            $('#' + numResultadosBusq).css('display', '');
            $('.group.filterSpace').css('display', '');
        }
        $('#numResultadosRemover').remove();
    }
}

/**
 * Obtiene el valor de un parámetro de búsqueda en la URL actual.
 * La función busca en los parámetros de la cadena de consulta de la URL el valor asociado con el nombre de parámetro proporcionado.
 * @param {string} parameterName - El nombre del parámetro de búsqueda cuyo valor se desea obtener.
 * @returns {string|null} El valor del parámetro de búsqueda o `null` si el parámetro no está presente en la URL.
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
 * Recorre todos los elementos de facetas gráficas en la página y aplica un formato gráfico a cada uno de ellos.
 * La función se encarga de verificar el tipo de gráfico que debe ser renderizado (barras o sectores) y asegura que cada faceta solo se formatee una vez. 
 * @returns {void} No devuelve ningún valor. Solo realiza modificaciones en el DOM.
 */
function FormatearFacetasGraficas() {
    //Componentes
    var facetasBarras = $('.componenteFaceta.graficoBarras');
    facetasBarras.each(function () {
        if (!$(this).hasClass("formateado")) {
            //Recorremos cada faceta
            var faceta = $(this);
            PintarGraficoFaceta(faceta, 'barras');
            faceta.addClass("formateado");
        }
    });

    var facetasSectores = $('.componenteFaceta.graficoSectores');
    facetasSectores.each(function () {
        if (!$(this).hasClass("formateado")) {
            //Recorremos cada faceta
            var faceta = $(this);
            PintarGraficoFaceta(faceta, 'sectores');
            faceta.addClass("formateado");
        }
    });
}


/**
 * Configura y pinta un gráfico en una faceta gráfica usando Google Charts.
 * La función maneja dos estilos de gráficos: 'barras' y 'sectores'.
 * 
 * @param {jQuery} faceta - Elemento jQuery que representa la faceta gráfica a pintar.
 * @param {string} estilo - El estilo del gráfico a dibujar, puede ser 'barras' o 'sectores'.
 * @returns {void} No devuelve ningún valor. Solo realiza modificaciones en el DOM y dibuja el gráfico.
 */
function PintarGraficoFaceta(faceta, estilo) {
    faceta.hide();
    var listaElementos = faceta.find('.facetedSearch ul li');
    var titulo = faceta.find('.faceta-title').text();

    var array = new Array();
    array[0] = new Array();
    array[0][0] = 'Propiedad';
    array[0][1] = 'Número';

    var num = 1;

    var arrayEnlaces = new Array();
    var total = 0;
    listaElementos.each(function () {
        //Recorremos cada elemento de la faceta
        var elemento = $(this);

        var enlace = elemento.find('a').attr('href');
        var nombreSinFormato = elemento.find('a').text();

        var nombre = nombreSinFormato.substring(0, nombreSinFormato.indexOf('(')).trim();
        var numero = nombreSinFormato.substring(nombreSinFormato.indexOf('(') + 1);
        numero = numero.substring(0, numero.indexOf(')'));

        array[num] = new Array();
        array[num][0] = nombre;
        array[num][1] = parseFloat(numero);
        arrayEnlaces[num - 1] = enlace;
        num++;
    });

    google.load("visualization", '1.1', { packages: ["corechart"] });
    google.setOnLoadCallback(drawChart);
    function drawChart() {
        var data = google.visualization.arrayToDataTable(array);

        var view = new google.visualization.DataView(data);
        view.setColumns([0, 1,
            {
                calc: "stringify",
                sourceColumn: 1,
                type: "string",
                role: "annotation"
            }]);

        var options = {
            title: titulo
        };

        var idGrafico = guidGenerator();
        faceta.html("<div id='div_cms_faceta" + idGrafico + "'></div>");
        var chart = null;

        if (estilo == 'barras') {
            chart = new google.visualization.BarChart(document.getElementById('div_cms_faceta' + idGrafico));
            options = {
                title: titulo,
                legend: { position: 'none' }
            };
        } else if (estilo == 'sectores') {
            chart = new google.visualization.PieChart(document.getElementById('div_cms_faceta' + idGrafico));
        }
        faceta.show();
        chart.draw(view, options);
        google.visualization.events.addListener(chart, 'select', function () {
            var enlace = '';
            var selection = chart.getSelection();
            for (var i = 0; i < selection.length; i++) {
                var item = selection[i];
                if (estilo == 'barras') {
                    if (item.row != null && item.column != null) {
                        if (item.column == '1') {
                            enlace = arrayEnlaces[item.row];
                        }
                    }
                } else if (estilo == 'sectores') {
                    enlace = arrayEnlaces[item.row];
                }
            }
            if (enlace != '') {
                document.location.href = enlace;
            }
        });
    }
}

/**
 * Obtiene una URL a partir de un servicio dado, eligiendo una URL de una lista separada por comas si es necesario.
 * 
 * @function obtenerUrl
 * @param {string} service - El nombre del servicio para obtener la URL correspondiente.
 * @returns {string} La URL seleccionada del servicio.
 */
function obtenerUrl(service) {
    var url = service;
    if (url.indexOf(',') != -1) {
        var urlMultiple = url.split(',');
        if (indicesWS[service] == null) {

            indicesWS[service] = null;
        }
        if (indicesWS[service] == null) {

            indicesWS[service] = this.aleatorio(0, urlMultiple.length - 1);
        } else if (indicesWS[service] > urlMultiple.length - 1) {

            indicesWS[service] = 0;
        }
        url = urlMultiple[indicesWS[service]];
        indicesWS[service]++;
    }
    return url;
}

/*WS.js*/ 
WSDataType = { json: "json", jsonp: "jsonp" };

/**
 * Objeto con dos propiedades estáticas que representan los tipos de datos aceptados por el servicio web: json para JSON y jsonp para JSONP
 */
WS = function(service, dataType) {
    this.service = service;
    if (dataType)
        this.dataType = dataType;
};

var indicesWS = {};

/**
 * Define las propiedades y métodos que todas las instancias de la clase WS compartirán.
 * Estos métodos permiten a las instancias de WS interactuar con servicios web a través de solicitudes HTTP, manejar URLs de servicio, y generar números aleatorios.
 * */
WS.prototype = {
    dataType: WSDataType.json,
    service: null,
    call: function (pMethod, pArgs, pCallback, pError) {
        var url = null;
        var service = this.service;
        service = this.obtenerUrl(service);
        if (service[service.length - 1] !== "/") service += "/";

        // Si la dataType es jsonp, formateamos los parámetros correctamente.
        if (this.dataType === WSDataType.jsonp) {
            // Envolvemos en comillas los parámetros que deben tener comillas
            for (var key in pArgs) {
                if (pArgs.hasOwnProperty(key) && key === 'callback') {
                    // Aseguramos que las comillas no se codifiquen como %22
                    pArgs[key] = `"${pArgs[key]}"`;  // Agregar comillas alrededor de los valores
                }
            }

            // Generamos la URL con los parámetros correctamente formateados
            let baseUrl = service + pMethod;
            let paramString = `?`;

            // Concatenar todos los parámetros manualmente
            for (var key in pArgs) {
                if (pArgs.hasOwnProperty(key)) {
                    paramString += `${key}=${pArgs[key]}&`;
                }
            }
            // Elimina el último "&" extra
            url = baseUrl + paramString.slice(0, -1);
        } else {
            url = service + pMethod;
        }

        // Si el método es "AutoCompletarTipado", vaciamos los argumentos
        if (pMethod === "AutoCompletarTipado") {
            pArgs = null;
        }

        $.ajax({
            type: this.dataType === WSDataType.json ? "POST" : "POST",
            url: url,
            data: pArgs,
            cache: false,
            contentType: "application/x-www-form-urlencoded",
            dataType: this.dataType
        })
            .done(function (response) {
                if (pCallback) {
                    if (response.d != null) {
                        pCallback(response.d);
                    } else {
                        pCallback(response);
                    }
                }
            })
            .fail(function (data) {
                if (pError) {
                    pError();
                }
            });
    },
    obtenerUrl: function (service) {
        var url = service;
        if (url.indexOf(',') !== -1) {
            var urlMultiple = url.split(',');
            if (indicesWS[service] == null) {
                indicesWS[service] = null;
            }
            if (indicesWS[service] == null) {
                indicesWS[service] = this.aleatorio(0, urlMultiple.length - 1);
            } else if (indicesWS[service] > urlMultiple.length - 1) {
                indicesWS[service] = 0;
            }
            url = urlMultiple[indicesWS[service]];
            indicesWS[service]++;
        }
        return url;
    },
    aleatorio: function (inferior, superior) {
        const numPosibilidades = superior - inferior;
        let aleat = Math.random() * numPosibilidades;
        aleat = Math.round(aleat);
        return parseInt(inferior) + aleat;
    }
};


/**
 * Se ejecuta al completar la carga de contextos.
 * Esta función realiza las siguientes acciones:
 * - Llama a la función `engancharClicks()` para gestionar eventos de clic en la interfaz de usuario.
 * @returns {void}
 */
function CompletadaCargaContextos() {
    engancharClicks();	
}


/**
 * Se ejecuta al completar la carga de facetas.
 * Esta función realiza las siguientes acciones:
 * - Llama al método `init()` de `comportamientoCargaFacetas` para inicializar comportamientos relacionados con las facetas.
 * - Verifica si `window.comportamientoCargaFacetasComunidad` es una función y, si es así, la ejecuta. (Encontrado en theme.js)
 * @name CompletadaCargaFacetas
 * @returns {void}
 */
function CompletadaCargaFacetas() {
    comportamientoCargaFacetas.init();
    if (typeof (window.comportamientoCargaFacetasComunidad) == 'function') {
        comportamientoCargaFacetasComunidad();
    }
}

/**
 * Inicializa los comportamientos relacionados con las facetas de búsqueda.
 * 
 * Esta función realiza las siguientes acciones:
 * - Llama al método `init()` de `facetedSearch` para inicializar la búsqueda facetada.
 * - Asocia eventos de clic a los elementos con la clase `verMasFaceta` para mostrar más información sobre una faceta específica.
 * @returns {void}
 */
var comportamientoCargaFacetas = {
    init: function () {
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

/**
 * Inicializa el panel modal de Login y gestiona los eventos relacionados.
 * 
 * Llama a los métodos de configuración, muestra el modal, gestiona el hash y configura los eventos necesarios para el panel de login.
 * Aparecerá siempre y cuando el usuario realice una acción y no disponga de permisos para ejecutarla.
 * 
 * @name operativaLoginEmergente.init
 * @memberof operativaLoginEmergente
 * @returns {void}
 */
var operativaLoginEmergente = {
    /**
     * Acción que dispara directamente el panel modal de Login
     */
    init: function () {
        this.config();
        this.showModal();
        this.doHashManagement();
        this.configEvents();
    },
    /*
     * Acción de cerrar la vista modal 
     */
    closeModal: function () {
        $(this.idModalPanel).modal('toggle');
        return;
    },
    /*
     * Acción de mostrar la vista modal
     * */
    showModal: function () {
        // Cerrar el "modal container"
        $("#modal-container").modal('hide');
        let modalToShow = $(this.idModalPanel).length > 0 ? $(this.idModalPanel) : $(this.idModalRestrictedAccess);
        modalToShow.modal('show');
    },
    /*
     * Opciones de configuración de la vista con el formulario modal
     * */
    config: function () {
        // Inicializar las vistas cuando están visibles
        // Panel Modal
        this.idModalPanel = '#modal-login';
        this.idModalRestrictedAccess = "#modal-restringed-community-access";
        // Referencia al formulario
        this.idForm = '#formPaginaLogin';
        this.formClassName = "formPaginaLogin"
        this.bodyClassNameRegistro = 'operativaRegistro';

        // Captar el formulario Login si se está en la página Login. Si no está en página Login --> Coger el formulario del modal
        this.form = this.isLoginCurrentPage ? $('body').find(this.idForm) : $(this.idModalPanel).find(this.idForm);
        this.loginForms = $(`.${this.formClassName}`);


        // Inputs y botones
        this.idInputEmail = '#usuario_Login';
        this.inputEmailClassName = "usuario_Login";
        this.inputEmail = $(this.form).find(this.idInputEmail),
            this.idInputPassword = '#password_login';
        this.inputPasswordClassName = "password_login";
        this.inputPassword = $(this.form).find(this.idInputPassword);
        this.idButtonLogin = '#btnSubmit';
        this.buttonLoginClassName = 'btnSubmit';
        this.buttonLogin = $(this.form).find(this.idButtonLogin);
        // Paneles de error
        this.idLoginPanelError = '#loginError .ko';
        this.loginPanelError = $(this.form).find(this.idLoginPanelError);
        this.loginPanelErrorClassName = "loginErrorKo";
        this.idLoginPanelErrorTwice = '#logintwice .ko';
        this.loginPanelErrortwiceKoClassName = "logintwiceKo";
        this.loginPanelErrorTwice = $(this.form).find(this.idLoginPanelErrorTwice);
        this.idLoginErrorAutenticacionExterna = '#loginErrorAutenticacionExterna .ko';
        this.loginErrorAutenticacionExterna = $(this.form).find(this.idLoginErrorAutenticacionExterna);
        this.loginErrorAutenticacionExternaClassName = "loginErrorAutenticacionExternaKo";
        this.idLoginErrorBloqueado = '#loginErrorBloqueado .ko';
        this.loginErrorBloqueado = $(this.form).find(this.idLoginErrorBloqueado);
        this.loginErrorBloqueadoClassName = "loginErrorBloqueadoKo";
        this.panelesError = [this.loginPanelError, this.loginPanelErrorTwice, this.loginErrorAutenticacionExterna, this.loginErrorBloqueado];
        this.panelesErrorWithClassName = $(".ko");

        // Flag Indicador de que el botón Login se ha configurado
        this.isButtonLoginIsConfigured = false;
    },

    /**
    * Configuración de los eventos (clicks, focus) de los inputs del panel/formulario  
    */
    configEvents: function () {
        // Referencia al 'emergente panel login'
        const that = this;

        that.isButtonLoginIsConfigured = $(`.${this.buttonLoginClassName}`).length > 0 ? true : false;

        // Botón Login - Vía Clase
        $(`.${this.buttonLoginClassName}`).click(function (event) {
            // Ocultar por defecto posibles mensajes de error
            $.each(that.panelesErrorWithClassName, function () {
                $(this).hide();
            });
            // Hacer login solo si los datos han sido introducidos
            const loginButton = $(this);
            const currentForm = loginButton.closest(`.${that.formClassName}`);
            if (that.validarCampos(loginButton) == true) {
                // Mostrar loading
                MostrarUpdateProgress();
                currentForm.submit();
            } else {
                const loginPanelError = currentForm.find(`.${that.loginPanelErrorClassName}`);
                loginPanelError.show();
            }
        });

        // Input Password (Hacer login si se pulsa "Enter" desde input password) vía ID
        $(`.${this.inputPasswordClassName}`).keypress(function (event) {
            event.keyCode === 13 ? that.buttonLogin.click() : null
        });

        // Configurar botón si no hay por clase
        if (that.isButtonLoginIsConfigured == false) {
            // Botón Login vía ID
            this.buttonLogin.click(function (event) {
                // Ocultar por defecto posibles mensajes de error
                that.panelesError.forEach(panelError => panelError.hide());
                // Hacer login solo si los datos han sido introducidos
                if (that.validarCamposById() == true) {
                    // Mostrar loading
                    MostrarUpdateProgress();
                    that.form.submit();
                } else {
                    that.loginPanelError.show();
                }
            });

            // Input Password (Hacer login si se pulsa "Enter" desde input password) vía ID
            this.inputPassword.keypress(function (event) {
                event.keyCode === 13 ? that.buttonLogin.click() : null
            });
        }
    },

    /**
    * Comprobar que los campos (email y password) no están vacíos
    * @returns {bool}    
    */
    validarCampos: function (loginButton) {
        // Encontrar el formulario desde el que se está haciendo la validación o el Login
        const currentForm = loginButton.closest(`.${this.formClassName}`);
        const inputEmail = currentForm.find(`.${this.inputEmailClassName}`);
        const inputPassWord = currentForm.find(`.${this.inputPasswordClassName}`);
        return (inputEmail.val() != '' && inputPassWord.val() != '');
    },

    /**
    * Comprobar que los campos (email y password) no están vacíos
    * @returns {bool}    
    */
    validarCamposById: function () {
        return (this.inputEmail.val() != '' && this.inputPassword.val() != '');
    },


    /**
    * Comprobar si la página actual es la página principal de Login de una comunidad
    * Se puede saber comprobando si se dispone de una clase en el Login concreta    
    * @returns {bool}    
    */
    isLoginCurrentPage: function () {
        return $('body').hasClass(this.bodyClassNameRegistro);
    },

    /**
    * Gestán de los Hash que hacía en la página Login anterior -> Gestión de errores
    * Solo ha de ejecutarse cuando el usuario se encuentre en la página Login
    */
    doHashManagement: function () {
        const that = this;

        that.isButtonLoginIsConfigured = $(`.${this.buttonLoginClassName}`).length > 0 ? true : false;

        let loginPanelError = undefined;
        let loginPanelErrorTwice = undefined;
        let loginPanelErrorAutenticacionExterna = undefined;
        let loginPanelErrorBloqueado = undefined;

        // Paneles de error según clase
        if (that.isButtonLoginIsConfigured) {
            loginPanelError = $(`.${that.loginPanelErrorClassName}`);
            loginPanelErrorTwice = $(`.${that.loginPanelErrortwiceKoClassName}`);
            loginPanelErrorAutenticacionExterna = $(`.${that.loginErrorAutenticacionExternaClassName}`);
            loginPanelErrorBloqueado = $(`.${that.loginErrorBloqueadoClassName}`);
        } else {
            // Coger los paneles según ID
            loginPanelError = this.loginPanelError;
            loginPanelErrorTwice = this.loginPanelErrorTwice;
            loginPanelErrorAutenticacionExterna = this.loginErrorAutenticacionExterna;
            loginPanelErrorBloqueado = this.loginErrorBloqueado;
        }

        if (this.isLoginCurrentPage()) {
            if (ObtenerHash() == '#error') {
                //this.loginPanelError.show();                                
                loginPanelError.show();
            }
            else if (ObtenerHash().indexOf('&') > 0) {
                var mensajeError = ObtenerHash().split('&')[1];
                if (mensajeError != '') {
                    //$('#mensajeError').text(mensajeError);
                    loginPanelError.show();
                }
            }
            else if (document.location.href.endsWith('logintwice')) {
                loginPanelErrorTwice.show();
            }
            if (ObtenerHash() == '#errorAutenticacionExterna') {
                loginPanelErrorAutenticacionExterna.show();
            }
            if (ObtenerHash() == '#UsuarioBloqueado') {
                loginPanelErrorBloqueado.show();
            }
        }
    },
};

/**
 * Comprobar si el botón de mayúsculas está activado
 *
 * @param {Object} e A keypress event
 * @returns {Boolean} isCapsLock
 */
function isCapsLock(e) {
    e = (e) ? e : window.event;

    var charCode = false;
    if (e.which) {
        charCode = e.which;
    } else if (e.keyCode) {
        charCode = e.keyCode;
    }

    var shifton = false;
    if (e.shiftKey) {
        shifton = e.shiftKey;
    } else if (e.modifiers) {
        shifton = !!(e.modifiers & 4);
    }

    if (charCode >= 97 && charCode <= 122 && shifton) {
        return true;
    }

    if (charCode >= 65 && charCode <= 90 && !shifton) {
        return true;
    }

    return false;
};


/**
 * Inicializa la operativa de doble factor.
 * 
 * Configura parámetros, eventos y rutas necesarios para el proceso de autenticación de doble factor.
 * 
 * @function
 * @name operativaGestionDobleFactor.init
 * @memberof operativaGestionDobleFactor
 * @param {Object} pParams - Parámetros de configuración para la operativa de doble factor.
 * @param {string} pParams.urlBase - La URL base para las peticiones al backend.
 * @returns {void}
 */
const operativaGestionDobleFactor = {

    /**
     * Inicializar operativa
     */
    init: function (pParams) {
        this.pParams = pParams;
        this.config(pParams);
        this.configEvents();
        this.configRutas();
    },

    /**
     * Configuración de las rutas de acciones necesarias para hacer peticiones a backend
     */
    configRutas: function () {
        // Url base

        //this.urlBase = pParams.urlBase; //refineURL()
        this.urlBase = window.location.href.split("?")[0];
        this.urlError = `${this.urlBase}?error=true`;
        this.urlAceptarToken = `${this.urlBase}/aceptar-token`;
    },

    /*
     * Inicializar elementos de la vista
     * */
    config: function (pParams) {

        // Contenedor modal de contenido dinámico
        this.btnSubmit = $("#btnSubmit");
        this.inputTokenAutenticacion = $("#token_autenticacion");
        this.formAutenticacionDobleFactor = $("#formAutenticacionDobleFactor");
    },

    /**
     * Configuración de eventos de elementos del Dom (Botones, Inputs...)     
     */
    configEvents: function (pParams) {
        const that = this;

        that.btnSubmit.on("click", function (e) {
            that.validarToken();
        });
    },

    /**
     * Valida que el token pasado cumpla con el formato
     */
    validarToken: function (pParams) {
        const that = this;
        const inputToken = that.inputTokenAutenticacion.val().trim();
        var regexToken = /^([A-Za-z0-9]{6})$/;

        if (!inputToken.match(regexToken)) {
            Redirigir(that.urlError);
            return;
        }

        that.handleCreateSubmitUrl();

        MostrarUpdateProgress();
        that.formAutenticacionDobleFactor.submit();

    },

    /**
     * Añade al form action el valor del token introducido en el input
     */
    handleCreateSubmitUrl: function () {
        const that = this;
        // URL
        const urlAction = that.formAutenticacionDobleFactor.prop("action");
        const token = that.inputTokenAutenticacion.val().trim();
        newUrlAction = `${urlAction}&tokenDobleAutenticacion=${token}`;

        // Asignación de nueva URL
        this.formAutenticacionDobleFactor.prop("action", newUrlAction);
    },
};

/**
 * Clase jquery para poder gestionar el proceso de registro de un usuario
 * Este proceso en cuestión se encarga de la gestión del paso 1 de registro del usuario
 * Ej: Proceso de registro al acceder a la url: http://depuracion.net/comunidad/gnoss-developers-community/hazte-miembro
 * */
const operativaRegistroUsuarioPaso1 = {
    /**
     * Acción para inicializar elementos y eventos
     */
    init: function (pParams) {
        this.config(pParams);
        this.configEvents();
    },

    /*
     * Opciones de configuración de la vista con todas los inputs necesarios para realizar el registro del usuario
     * */
    config: function (pParams) {
        // Inicialización de las vistas
        this.txtFechaNac = $(`#${pParams.idTxtFechaNac}`);
        this.txtFechaNacDia = $(`#${pParams.idTxtFechaNacDia}`);
        this.txtFechaNacMes = $(`#${pParams.idTxtFechaNacMes}`);
        this.txtFechaNacAnio = $(`#${pParams.idTxtFechaNacAnio}`);
        this.txtNombre = $(`#${pParams.idTxtNombre}`);
        this.nombreCampoTxtNombre = pParams.nombreCampoTxtNombre;
        this.txtApellidos = $(`#${pParams.idTxtApellidos}`);
        this.nombreCampoTxtApellidos = pParams.nombreCampoTxtApellidos;
        this.txtCargo = $(`#${pParams.idTxtCargo}`);
        this.lblCargo = pParams.lblCargo;
        this.txtEmail = $(`#${pParams.idTxtEmail}`);
        this.txtNombreUsuario = $(`#${pParams.idTxtNombreUsuario}`);
        this.txtEmailTutor = $(`#${pParams.idTxtEmailTutor}`);
        this.txtContrasenya = $(`#${pParams.idTxtContrasenya}`);
        this.nombreCampoTxtContrasenya = pParams.nombreCampoTxtContrasenya;
        this.captcha = $(`#${pParams.idCaptcha}`);
        this.ddlPais = $(`#${pParams.idDdlPais}`);

        this.mensajePersonalizado = pParams.mensajePersonalizado;
        this.currentUrl = pParams.currentUrl;
        this.datepickerConfigOptions = pParams.datepickerConfigOptions;
    },

    /**
    * Configuración de los eventos 
    */
    configEvents: function () {
        // Referencia al objeto
        const that = this;

        // Input para Nombre (Quitar foco del input)
        this.txtNombre.blur(function () {
            ComprobarCampoRegistroMVC(that.nombreCampoTxtNombre);
        });

        // Input para Apellidos (Quitar foco del input)
        this.txtApellidos.blur(function () {
            ComprobarCampoRegistroMVC(that.nombreCampoTxtApellidos);
        });

        // Input para Cargo (Quitar foco del input)
        this.txtCargo.blur(function () {
            ValidarCampoNoVacio(that.txtCargo, that.lblCargo);
        });

        // Input para Cargo (Quitar foco del input)
        this.txtEmail.blur(function () {
            ComprobarEmailUsuario(that.currentUrl);
        });

        // Input para Contraseña (Quitar foco del input)
        this.txtContrasenya.blur(function () {
            ComprobarCampoRegistroMVC(that.nombreCampoTxtContrasenya);
        });

        // Configuración del datepicker
        this.txtFechaNac.datepicker({
            changeMonth: true,
            changeYear: true,
            yearRange: that.datepickerConfigOptions.yearRange,
            maxDate: that.datepickerConfigOptions.maxdate,
        });
    },
};

/**
 * Inicializa elementos y eventos para la solicitud de creación de una comunidad.
 * 
 * Configura los elementos del DOM, los eventos y establece los paneles descriptivos según el tipo de acceso.
 * 
 * @function
 * @name operativaSolicitudCreacionComunidad.init
 * @memberof operativaSolicitudCreacionComunidad
 * @param {Object} pParams - Parámetros de configuración para la solicitud de creación de comunidad.
 * @param {string} pParams.idTxtNombreComunidad - ID del campo de texto para el nombre de la comunidad.
 * @param {string} pParams.idTxtNombreCortoComunidad - ID del campo de texto para el nombre corto de la comunidad.
 * @param {string} pParams.idCmbAcceso - ID del comboBox para seleccionar el tipo de acceso.
 * @param {string} pParams.idLbEnviarSolicitud - ID del botón para enviar la solicitud de creación.
 * @param {string} pParams.idCmbComunidadPadre - ID del comboBox para seleccionar la comunidad padre.
 * @param {string} pParams.idPanParrafoAcceso0 - ID del panel de descripción del tipo de acceso 0.
 * @param {string} pParams.idPanParrafoAcceso1 - ID del panel de descripción del tipo de acceso 1.
 * @param {string} pParams.idPanParrafoAcceso2 - ID del panel de descripción del tipo de acceso 2.
 * @param {string} pParams.idPanParrafoAcceso3 - ID del panel de descripción del tipo de acceso 3.
 * @param {string} pParams.idPanComunidadPadre - ID del panel de comunidad padre.
 * @param {string} pParams.emptyGuid - GUID vacío utilizado como valor por defecto para la comunidad padre.
 * @param {string} pParams.urlPost - URL para enviar la solicitud de creación de la comunidad.
 * @returns {void}
 */
const operativaSolicitudCreacionComunidad = {
    /**
     * Acción para para inicializar elementos y eventos
     * @param {any} pParams
     */
    init: function (pParams) {
        this.config(pParams);
        this.configEvents(pParams);
        // Establecer por defecto los paneles a mostrar descriptivos
        this.cambiarDescTipoAcceso();
    },

    config: function (pParams) {
        // Inicialización de las vistas
        this.txtNombreComunidad = $(`#${pParams.idTxtNombreComunidad}`);
        this.txtNombreCortoComunidad = $(`#${pParams.idTxtNombreCortoComunidad}`);
        this.cmbAcceso = $(`#${pParams.idCmbAcceso}`);
        this.lbEnviarSolicitud = $(`#${pParams.idLbEnviarSolicitud}`);
        this.cmbComunidadPadre = $(`#${pParams.idCmbComunidadPadre}`);
        this.panParrafoAcceso0 = $(`#${pParams.idPanParrafoAcceso0}`);
        this.panParrafoAcceso1 = $(`#${pParams.idPanParrafoAcceso1}`);
        this.panParrafoAcceso2 = $(`#${pParams.idPanParrafoAcceso2}`);
        this.panParrafoAcceso3 = $(`#${pParams.idPanParrafoAcceso3}`);
        this.panComunidadPadre = $(`#${pParams.idPanComunidadPadre}`);
        this.emptyGuid = pParams.emptyGuid;
        this.urlPost = pParams.urlPost;
    },

    configEvents: function () {
        const that = this;

        // KeyPress en Nombre de Comunidad
        this.txtNombreCortoComunidad.on("keydown", function (e) {
            if (e.which || e.keyCode) {
                if ((event.which == 13) || (event.keyCode == 13)) {
                    return false;
                }
            }
        });

        // KeyUp en Nombre corto de Comunidad
        this.txtNombreCortoComunidad.on("keyup", function (e) {
            this.value = this.value.trim().replaceAll(' ', '-');
            this.value = this.value.replace(/[^a-zA-Z0-9_-]/g, '');
            this.value = this.value.toLowerCase();
        });

        // KeyPress en Nombre corto de Comunidad
        this.txtNombreComunidad.on("keydown", function (e) {
            if (e.which || e.keyCode) {
                if ((event.which == 13) || (event.keyCode == 13)) {
                    return false;
                }
            }
        });

        // Cambiar tipo de comunidad a crear
        this.cmbAcceso.on("change", function (e) {
            that.cambiarDescTipoAcceso();
        });

        // Enviar solicutd de creación de tipo de comunidad
        this.lbEnviarSolicitud.on("click", function (e) {
            that.validarCampos();
        });

    },

    /**
     * Cambiar la descripción del tipo de comunidad según la selección del comboBox del tipo de comunidad a crear
     */
    cambiarDescTipoAcceso: function () {

        this.panParrafoAcceso0.hide();
        this.panParrafoAcceso1.hide();
        this.panParrafoAcceso2.hide();
        this.panParrafoAcceso3.hide();
        this.panComunidadPadre.hide();

        switch (this.cmbAcceso.val()) {
            case '0':
                this.panParrafoAcceso0.show();
                break;
            case '1':
                this.panParrafoAcceso1.show();
                break;
            case '2':
                this.panParrafoAcceso2.show();
                break;
            case '3':
                this.panParrafoAcceso3.show();
                this.panComunidadPadre.show();
                break;
        }
    },

    /**
    * Cambiar la descripción del tipo de comunidad según la selección del comboBox del tipo de comunidad a crear
    */
    validarCampos: function () {
        var nombreCom = $('#txtNombreComunidad').val();
        var nombreCorto = $('#txtNombreCortoComunidad').val();
        var idiomaCom = $('#defaultCommunityLanguge').val();
        // Quitar tildes
        //nombreCorto = nombreCorto.normalize("NFD").replace(/[\u0300-\u036f]/g, "");        

        var descripcion = $('#txtDescripcionComunidad').val();

        var error = '';
        var RegExPatternnombreCom = /<|>$/;


        if (nombreCorto == '' || nombreCom == '' || descripcion == '' || idiomaCom == '') {
            error = form.campossolicitudcomunidadincompletos;
        }
        else if (nombreCom.match(RegExPatternnombreCom)) {
            error = form.nombrecomunidadincorrecto;
        }
        else if (!validarNombreCortoComunidad(nombreCorto)) {
            error = form.nombrecortocomunidadincorrecto;
        }

        if (error != '') {
            mostrarNotificacion("error", error);
        }
        else {
            MostrarUpdateProgress();
            const tipoComunidad = $('#cmbAcceso').val();
            let comunidadPadre = this.emptyGuid;
            if ($('#cmbComunidadPadre').length > 0) {
                comunidadPadre = $('#cmbComunidadPadre').val();
            }
            const args = nombreCom + '|' + nombreCorto + '|' + descripcion + '|' + tipoComunidad + '|' + comunidadPadre;
            var respuesta = '';
            // Construcción del objeto para hacer la petición
            var dataPost = {
                Name: nombreCom,
                ShortName: nombreCorto,
                Description: Encoder.htmlEncode(descripcion.replace(/\n/g, '')),
                Type: tipoComunidad,
                CommunityParent: comunidadPadre,
                Language: idiomaCom
            }

            // Petición a realizar
            GnossPeticionAjax(
                this.urlPost,
                dataPost,
                true
            ).done(function (data) {
                // Mostrar ok
                mostrarNotificacion("success", data);
            }).fail(function (data) {
                // Mostrar error
                mostrarNotificacion("error", data);
            }).always(function () {
                OcultarUpdateProgress();
            })
        }
    },
};


/**
 * Operativa/Objeto que contiene las operaciones para gestionar la edición del perfil de un usuario
 */
const operativaEditarPerfilUsuario = {

    /**
     * Acción para inicializar elementos y eventos
     * @param {any} pParams
     */
    init: function (pParams) {
        this.config(pParams);
        this.configEvents(pParams);
    },

    /**
     * Opciones de configuración de la vista (recoger ids para poder interactuar)
     * @param {any} pParams
     */
    config: function (pParams) {

        // Inicialización de las vistas
        this.deleteProfileImage = $(`#${pParams.perfilPersonal.idDeleteProfileImage}`);
        this.name = $(`#${pParams.perfilPersonal.idName}`);
        this.lastName = $(`#${pParams.perfilPersonal.idLastName}`);
        this.email = $(`#${pParams.perfilPersonal.idEmail}`);
        this.emailProfesional = $(`#${pParams.perfilPersonal.idEmailProfesional}`);
        this.bornDate = $(`#${pParams.perfilPersonal.idBornDate}`);
        this.twofactorauthentication = $(`#${pParams.perfilPersonal.idTwoFactorAuthentication}`);
        this.country = $(`#${pParams.perfilPersonal.idCountry}`);
        this.region = $(`#${pParams.perfilPersonal.idRegion}`);
        this.location = $(`#${pParams.perfilPersonal.idLocation}`);
        this.postalCode = $(`#${pParams.perfilPersonal.idPostalCode}`);
        this.lang = $(`#${pParams.perfilPersonal.idLang}`);
        this.sex = $(`#${pParams.perfilPersonal.idSex}`);
        this.emailTutor = $(`#${pParams.perfilPersonal.idEmailTutor}`);
        this.idIsSearched = $(`#${pParams.perfilPersonal.idIsSearched}`);
        this.idIsExternalSearched = $(`#${pParams.perfilPersonal.idIsExternalSearched}`);
        this.chkUsarImagenPersonal = $(`#${pParams.perfilPersonal.idChkUsarImagenPersonal}`);

        // Perfil profesional
        this.nameOrganization = $(`#${pParams.perfilPersonal.idNameOrganization}`);
        this.countryOrganization = $(`#${pParams.perfilPersonal.idCountryOrganization}`);
        this.postalCodeOrganization = $(`#${pParams.perfilPersonal.idPostalCodeOrganization}`);
        this.locationOrganization = $(`#${pParams.perfilPersonal.idLocationOrganization}`);
        this.aliasOrganization = $(`#${pParams.perfilPersonal.idAliasOrganization}`);
        this.websiteOrganization = $(`#${pParams.perfilPersonal.idWebsiteOrganization}`);
        this.addressOrganization = $(`#${pParams.perfilPersonal.idAddressOrganization}`);

        // edición sección Bio (CV)
        this.description = $(`#${pParams.curriculum.idDescription}`);
        this.tags = $(`#${pParams.curriculum.idTags}`);

        //edición sección Redes Sociales
        this.urlUsuario = $(`#${pParams.redesSociales.idUrlUsuario}`);
        this.tblRedesSociales = $(`#${pParams.redesSociales.idTblRedesSociales}`);
        this.btnRedSocial = $(`#${pParams.redesSociales.idBtnRedSocial}`);
        this.twitterSocial = $(`#${pParams.redesSociales.idTwitterSocial}`);
        this.facebookSocial = $(`#${pParams.redesSociales.idFacebookSocial}`);
        this.linkedinSocial = $(`#${pParams.redesSociales.idLinkedinSocial}`);

        // Se utilizará la clase ya que hay muchos elementos para borrar (botón papelera con clase btnBorrarURL)
        this.classBorrarURL = pParams.redesSociales.idBtnBorrarUrl;
        this.btnBorrarUrl = $(`.${pParams.redesSociales.idBtnBorrarUrl}`);

        // Otros (Paneles, botones, url)
        this.divPanelInfo = $(`#${pParams.others.idDivPanelInfo}`);
        this.saveButton = $(`#${pParams.others.idSaveButton}`);
        this.urlPersonalProfileSaveProfile = pParams.others.urlPersonalProfileSaveProfile;
        this.urlPersonalProfileSaveBio = pParams.others.urlPersonalProfileSaveBio;
        this.urlPersonalProfileSaveSocialWebs = pParams.others.urlPersonalProfileSaveSocialWebs;
        this.urlImagenAnonima = pParams.others.urlImagenAnonima;

        // Inputs que NO podrán quedar vacíos
        this.inputsNoEmpty = [this.name,
        this.lastName,
        this.email,
        this.bornDate,
        this.emailProfesional,
        this.nameOrganization,
        this.countryOrganization,
        this.postalCodeOrganization,
        this.locationOrganization,
        this.addressOrganization
        ];

        // Nombre de las clases css para estilo de error o success (Bootstrap)
        this.okClass = "alert-success";
        this.errorClass = "alert-danger";
    },

    /**
     * Validar que los campos aquí mencionados no están vacíos
     * @param {any} inputs: Array de inputs para ser recorridos y verificar que ninguno de los aquí indicados están vacíos
     * @returns {bool}: Devolverá true o false siempre y cuando los inputs pasados sean diferente de vacío
     */
    validarCampos: function (inputs) {
        let areInputsOK = false;
        let error = "";

        for (input of inputs) {
            if (input.length > 0) {
                error = ValidarCampoNoVacio(input, undefined, true);
                if (error.length > 0) { break }
            }
        }

        if (error.length > 0) {
            this.showInfoPanelErrorOrOK(false, true, error);
        } else {
            areInputsOK = true
            this.showInfoPanelErrorOrOK(false, false, undefined);
        }
        return areInputsOK;
    },

    /**
     * Mostrar el panel informativo con un mensaje de error o ko. Si los dos son falsos, el panel quedará oculto
     * @param {boolean} showOK: Si se desea mostrar el mensaje de OK
     * @param {boolean} showError: Si se desea mostrar el mensaje KO
     * @param {string} message: El mensaje que irá en el panel informativo
     */
    showInfoPanelErrorOrOK: function (showOK, showError, message) {
        const that = this;

        // Mostrar panel OK
        that.divPanelInfo.html(message);
        if (showOK) {
            that.divPanelInfo.addClass(this.okClass);
            that.divPanelInfo.removeClass(this.errorClass);

        } else if (showError) {
            // Mostrar panel Error
            that.divPanelInfo.removeClass(this.okClass);
            that.divPanelInfo.addClass(this.errorClass);
        } else {
            // Ocultar el panel de mensajes
            this.divPanelInfo.hide();
            return
        }
        // Mostrar el panel
        this.divPanelInfo.show();
    },

    /**
     * Configuración de los eventos de los elementos html (click, focus...)
     * */
    configEvents: function (pParams) {
        const that = this;

        // Eliminar foto subida actual del usuario
        this.deleteProfileImage.on("click", function (e) {
            e.preventDefault();
            e.stopPropagation();
            that.eliminarImagenPerfil();
        });

        // Valor cambiado de inputs -> Avisar al usuario con sobreado rojo (o quitarlo) si es vacío campo obligatorio
        this.inputsNoEmpty.forEach(input => {
            input.on("change", function () {
                if ($(this).val().length == 0) {
                    $(this).addClass('is-invalid');
                } else {
                    $(this).removeClass('is-invalid');
                }
            });
        });

        // Botón de guardado de los datos
        this.saveButton.on("click", function () {
            if (that.validarCampos(that.inputsNoEmpty)) {
                // Guardar sección de datos personales (Nombre, Apellidos)
                that.savePersonalDataProfile();
                // Guardar sección de Curriculum (Tags, Descripcion)
                that.saveBioUserProfile(false);
            }
        });

        // Botón click para añadir url del input en perfil del usuario
        this.btnRedSocial.on("click", function () {
            that.addSocialWebsFromInputToTable(that.urlUsuario.val());
            // Vaciar el input rellenado
            that.urlUsuario.val('');
        });

        // Pulsación Enter para guardado de URL en perfil de usuario
        this.urlUsuario.keypress(function (event) {
            if (event.keyCode === 13) {
                that.addSocialWebsFromInputToTable(that.urlUsuario.val());
                that.urlUsuario.val('');
            }
        });

        // Botón/Icono de papelera para borrar una red social-web
        $(document).on("click", `.${that.classBorrarURL}`, function () {
            const urlName = $(this).data("urlname");
            // Detectar si es twitter, facebook o linkedin y eliminarlo del input correspondiente
            const socialWebsInputs = $(".inputRedSocial").filter(function () {
                return $(this).val().includes(urlName.toLowerCase());
            });

            // Vaciar el input de la red social que se ha eliminado
            if (socialWebsInputs.length > 0) {
                socialWebsInputs.val('');
            }

            // Eliminar la red social de BD
            that.eliminarUrlRedSocial($(this));
        });

        // Evento change de país para que se carguen las C. Autónomas asociadas al país.
        this.country.change(function () {
            // Mostrado de Loading
            MostrarUpdateProgress();

            const dataPost = {
                callback: "CambiarPais",
                pais: $(this).val()
            }

            GnossPeticionAjax(
                location.href,
                dataPost,
                true
            ).done(function (response) {
                //that.region.parent().replaceWith(response);
                $(`#${pParams.perfilPersonal.idRegion}`).parent().replaceWith(response);
                // Reiniciar el combobox de select2 al haber sido creado de 0 siempre y cuando sea un elemento select
                if ($(`#${pParams.perfilPersonal.idRegion}`).prop('nodeName') == "SELECT") {
                    $(`#${pParams.perfilPersonal.idRegion}`).select2();
                }
            }).always(function () {
                OcultarUpdateProgress();
            });
        });

        // Input de red social Twitter cuando se salga de él, Revisar si existe en la tabla y eliminarla para sustituirla por la nueva introducida      
        this.twitterSocial.on("blur", function () {
            if ($(this).val().length > 0) {
                that.findAndUpdatePersonalSocialWeb("twitter", $(this));
            }
        });

        // Input de red social Facebook cuando se salga de él, Revisar si existe en la tabla y eliminarla para sustituirla por la nueva introducida      
        this.facebookSocial.on("blur", function () {
            if ($(this).val().length > 0) {
                that.findAndUpdatePersonalSocialWeb("facebook", $(this));
            }
        });

        // Input de red social Facebook cuando se salga de él, Revisar si existe en la tabla y eliminarla para sustituirla por la nueva introducida      
        this.linkedinSocial.on("blur", function () {
            if ($(this).val().length > 0) {
                that.findAndUpdatePersonalSocialWeb("linkedin", $(this));
            }
        });
        // Checkbox en identidad profesional para cambiar la foto por la identidad personal
        this.chkUsarImagenPersonal.on("click", function () {
            that.setUsePersonalImage();
        });

        this.twofactorauthentication.on("click", function () {
            that.changeTwoFactorAuthentication();
        });

    },

    /**
     * Acción para indicar que se desea utilizar la autenticacion de doble factor
     * */
    changeTwoFactorAuthentication: function () {
        const that = this;

        if (this.twofactorauthentication.is(':checked')) {
            // Ocultar la sección de imágen ya que se desea utiliza la misma que la del perfil personal
            $(`#idTwoFactorAuthentication`).hide();
        }
        else {
            $(`#idTwoFactorAuthentication`).show();
        }
        GnossPeticionAjax(
            `${that.urlPersonalProfileSaveProfile}`,
            dataPost,
            true
        );
    },

    /**
     * Acción para indicar que se desea utilizar la foto del perfil personal en la identidad profesional
     * */
    setUsePersonalImage: function () {
        const that = this;

        if (this.chkUsarImagenPersonal.is(':checked')) {
            // Ocultar la sección de imágen ya que se desea utiliza la misma que la del perfil personal
            $(`#panelLoadImageId`).hide();
        }
        else {
            $(`#panelLoadImageId`).show();
        }
        var dataPost = {
            callback: "UsarFotoPersonal",
            UsarFotoPersonal: that.chkUsarImagenPersonal.is(':checked')
        }
        GnossPeticionAjax(
            `${that.urlPersonalProfileSaveSocialWebs}`,
            dataPost,
            true
        );
    },

    /**
     * Acción de guardar los datos del perfil del sección 'Datos Personales' (Nombre, Apellidos...)
     * */
    savePersonalDataProfile: function () {
        const that = this;
        // Mostrado de Loading
        MostrarUpdateProgress();
        // Construcción de objeto formData
        const dataPost = new FormData();
        dataPost.append('peticionAJAX', true);

        // Recorrido de cada input para coger su name-value
        $("#formularioEdicionPerfilPersonal").find(':input').each(function () {
            let valor = "";
            if ($(this).is(':checkbox')) {
                valor = $(this).is(':checked')
            }
            else if (!$(this).is(':button')) {
                valor = $(this).val();
            }
            dataPost.append($(this).attr('name'), valor);
        });

        // Realizar petición de guardado de datos personales del perfil del usuario        
        GnossPeticionAjax(that.urlPersonalProfileSaveProfile, dataPost, true, false)
            .done(function (data) {
                //GuardadoCVRapido('OK'); 
                // Guardar las redes sociales y urls que están en la tabla de Redes sociales
                that.addSocialWebsFromTableAndInputsToDataBase();
                //that.showInfoPanelErrorOrOK(true, false, data);
                mostrarNotificacion('success', data);

            })
            .fail(function (data) {
                //that.showInfoPanelErrorOrOK(false, true, data);
                mostrarNotificacion('error', data);
            })
            .always(function () {
                OcultarUpdateProgress();
            });
    },

    /**
     * Acción de guardar los datos del perfil del usuario, sección 'Curriculum' (Tags, Descripción)   
     * */

    /**     
     * @param {boolean} isNecessaryToHaveTagsAndDescription: El API exige que haya al menos una descripción y un Tag para el guardado de estos datos. Tenerlo en cuenta. 
     * La idea es quitar esta restricción tal y como se ha comentado a Juan (29-06-2021)
     */
    saveBioUserProfile: function (isNecessaryToHaveTagsAndDescription) {

        const that = this;

        // Controlar que los items existan en la web (Organización no los suele cargar)
        if (this.tags.length > 0 && this.description.length > 0) {
            if (isNecessaryToHaveTagsAndDescription == true) {
                if (this.tags.val().length <= 1 && this.description.val().length == 0) {
                    return;
                }
            }
        } else {
            return;
        }
        // Mostrado de Loading
        MostrarUpdateProgress();


        // Construcción del objeto POST
        const dataPost = {
            Description: that.description.val(),
            Tags: that.tags.val()
        }
        GnossPeticionAjax(this.urlPersonalProfileSaveBio, dataPost, true, false)
            .done(function (data) {
            }).fail(function (data) {
                //GuardadoCVRapido('KO');            
            }).always(function () {
                OcultarUpdateProgress();
            });
    },

    /**
     * Buscará una cadena de una posible red social, la eliminará de la tabla y añadirá la nueva   
     * @param {any} socialWebName
     */
    findAndUpdatePersonalSocialWeb: function (socialWebName, item) {
        // Buscar posible fila que contenga la red social Twitter y eliminarla para actualizarla con el nuevo contenido
        const tableRow = $(`td`, this.tblRedesSociales).filter(function () {
            return $(this).text().includes(socialWebName);
        }).closest("tr");

        if (tableRow.length > 0) {
            tableRow.remove();
        }
        // Añadir la red social a la tabla
        this.addSocialWebsFromInputToTable($(item).val());
    },

    /**
     * Añadir a la tabla la url introducida que se haya introducido tanto en los inputs (redes sociales) como en los inputs.
     * */
    addSocialWebsFromInputToTable: function (url) {
        // Comprobar que hay al menos una red social
        if (url.length > 0) {
            let domainName = url.replace(/.+\/\/|www.|\..+/g, '');
            // Primera letra mayúscula
            domainName = domainName.charAt(0).toUpperCase() + domainName.slice(1);
            // Mostrar / Montar la red social en la tabla
            this.montarUrlRedSocial(domainName, url);
        }
    },

    /**
     * Guardar las redes sociales que se encuentran en la tabla de redes sociales
     * */
    addSocialWebsFromTableAndInputsToDataBase: function () {
        const that = this;
        // Redes sociales que se encuentran en Tabla
        const socialWebs = $(".urlTitle", this.tblRedesSociales);

        // Comprobar que hay al menos una red social
        if (socialWebs.length > 0) {
            // Recorrer las redes sociales y hacer el guardado

            for (const socialWeb of socialWebs) {
                // Construyo objeto para guardar la BD
                const dataPost = {
                    callback: "AnyadirRedSocial",
                    url: $(socialWeb).text(),
                }
                // Realizar la petición de guardar en BD
                GnossPeticionAjax(this.urlPersonalProfileSaveSocialWebs, dataPost, true, false)
                    .done(function (data) {
                        console.log(`${data} guardada en BD`);
                        // Mostrar la tabla en caso de estar oculta
                        that.tblRedesSociales.removeClass('d-none');
                    }).fail(function (data) {
                        that.showInfoPanelErrorOrOK(false, true, data);
                    });
            }
        }
    },

    /**
     * Mostrar la url recién agregada en la tabla correspondiente de urls del usuario al haber sido agregada habiendo pulsado en el botón "Añadir"
     * @param {any} data: Nombre de la url. No se refiere a la URL o dirección de la web añadida, sino al nombre propiamente dicho
     * @param {any} url: Url añadida por el usuario
     */
    montarUrlRedSocial: function (data, url) {
        const htmlFila = `
            <tr>
              <td><img src="https://www.google.com/s2/favicons?domain=${url}" /></td>
              <td class="urlName">${data}</td>
              <td class="urlTitle">${url}</td>
              <td><span data-urlName="${data}"  class="material-icons-outlined ${this.classBorrarURL}" style="cursor: pointer" title="Eliminar" alt="Eliminar">delete</span></td>
            </tr>
        `;

        // Agregar la fila de la red social a la tabla
        this.tblRedesSociales.find('tbody').append(htmlFila);
        // Comprobar si la tabla está oculta
        if (this.tblRedesSociales.hasClass("d-none")) { this.tblRedesSociales.removeClass("d-none"); }
    },

    /**
     * Acción de eliminar una URL del servidor y también de la tabla de url del usuario     
     * @param {any} btnDeleteUrl: Botón de borrado pulsado
     */
    eliminarUrlRedSocial: function (btnDeleteUrl) {
        const that = this;

        // Nombre de la red que se desea borrar
        const nombreRed = btnDeleteUrl.data("urlname");

        // Mostrar Loading
        MostrarUpdateProgress();
        // Construcción del objeto dataPost
        const dataPost = {
            callback: "EliminarRedSocial",
            nombreRed: nombreRed,
        }

        GnossPeticionAjax(this.urlPersonalProfileSaveSocialWebs, dataPost, true, false)
            .done(function (data) {
                // Eliminar la red social/web de la tabla
                var tBody = btnDeleteUrl.closest("tbody");
                // Eliminar la row de la tabla                
                btnDeleteUrl.fadeOut("normal", function () {
                    $(this).closest("tr").remove();
                    if ($('tr', tBody).length == 0) {
                        that.tblRedesSociales.addClass('d-none');
                    }
                });
                that.showInfoPanelErrorOrOK(false, false, undefined);
            }).fail(function (data) {
                //that.showInfoPanelErrorOrOK(false, true, data);
                // Eliminar la red social/web de la tabla aunque no sea borrada del servidor ya que no existe todavía                
                var tBody = btnDeleteUrl.closest("tbody");
                // Eliminar la row de la tabla                
                btnDeleteUrl.fadeOut("normal", function () {
                    $(this).closest("tr").remove();
                    if ($('tr', tBody).length == 0) {
                        that.tblRedesSociales.addClass('d-none');
                    }
                });
            }).always(function () {
                OcultarUpdateProgress();
            });
    },
    /**
     * Acción para poder eliminar la actual imagen del usuario
     * */
    eliminarImagenPerfil: function () {
        const that = this;
        // Mostrar el loading
        MostrarUpdateProgress();
        // Objeto para realizar la petición      
        const dataPost = {
            callback: "EliminarImagen"
        }
        // Realización de la petición de eliminar imagen del perfil
        GnossPeticionAjax(
            this.urlPersonalProfileSaveSocialWebs,
            dataPost,
            true
        ).done(function () {

            // Mostrar imagen por defecto del usuario
            /*$('#imgPerfil').attr('src', urlAnonimo);
            imgPerfil.attr('src', urlAnonimo.replace('_grande', '_peque'));*/

            // Establecer la imagen "anónima"
            const imagePlaceholder = $(`.image-uploader__img`);
            imagePlaceholder.attr('src', that.urlImagenAnonima);
            // Ocultar el botón para eliminar la imagen del perfil
            that.deleteProfileImage.addClass("d-none");

        }).fail(function () {
            mostrarNotificacion("error", "Se ha producido un error al eliminar la imagen actual del perfil.")
        }).always(function () {
            OcultarUpdateProgress();
        })
    }
};

/**
 * Clase jquery para poder gestionar la solicitud de cambio de contraseña de un usuario
 * 
 * */
const operativaSolicitarCambiarContrasenia = {
    /**
     * Acción para inicializar elementos y eventos
     */
    init: function (pParams) {
        this.config(pParams);
        this.configEvents();
    },
    /*
     * Opciones de configuración de la vista con el formulario modal
     * */
    config: function (pParams) {
        // Inicialización de IDS de las vistas
        this.idTxtOldPassword = "#txtOldPassword";
        this.idTxtNewPassword = "#txtNewPassword";
        this.idTxtConfirmedPassword = "#txtConfirmedPassword";
        this.idBtnCambiarPassword = "#btnCambiarPassword";
        this.idWarningPanel = "#warning";
        this.idExpiredPanel = "#expiredPanel";
        this.idPasswordEmptyPanel = "#passwordEmptyPanel";
        this.idPasswordRequestInfoPanel = "#passwordRequestInfoPanel";

        // Url necesarias
        this.urlPasswordRequest = pParams.urlPasswordRequest;

        // Nombre de las clases css para estilo de error o success (Bootstrap)
        this.okClass = "alert-success";
        this.errorClass = "alert-danger";

        // Inicialización de las vistas
        this.txtOldPassword = $(this.idTxtOldPassword);
        this.txtConfirmedPassword = $(this.idTxtConfirmedPassword);
        this.txtNewPassword = $(this.idTxtNewPassword);
        this.btnCambiarPassword = $(this.idBtnCambiarPassword);
        this.warningPanel = $(this.idWarningPanel);
        this.passwordEmptyPanel = $(this.idPasswordEmptyPanel);
        this.passwordRequestInfoPanel = $(this.idPasswordRequestInfoPanel);
        this.panelesError = [this.warningPanel, this.passwordEmptyPanel];
        this.inputsFields = [this.txtOldPassword, this.txtConfirmedPassword, this.txtNewPassword];
    },

    /**
    * Comprobar que los campos (password old, password new y password confirmado ) no están vacíos
    * Comprobar que los password new y confirmado son iguales. En caso contrario, mostrará un error
    * @returns {bool}    
    */
    validarCampos: function () {
        let isPasswordValid = false;
        if (this.txtOldPassword.val() != '' && this.txtNewPassword.val() != '' && this.txtConfirmedPassword.val() != '') {
            if (this.txtNewPassword.val() === this.txtConfirmedPassword.val()) {
                isPasswordValid = true;
            }
        } else {
            isPasswordValid = false;
        }
        return isPasswordValid;
    },

    /**
    * Configuración de los eventos 
    */
    configEvents: function () {
        // Referencia al objeto
        const that = this;

        // Input Password Confirmado ENTER
        this.txtConfirmedPassword.keypress(function (event) {
            // Avisar con un mensaje si están activadas las mayúsculas del teclado
            isCapsLock(event) ? that.warningPanel.fadeIn("slow") : that.warningPanel.fadeOut("slow");
            event.keyCode === 13 ? that.btnCambiarPassword.click() : null
        });

        // Botón de Aceptar - Solicitar cambio de contraseña         
        this.btnCambiarPassword.click(function (event) {
            // Ocultar por defecto posibles mensajes de error
            that.hideErrorPanels();
            // Hacer login solo si los datos han sido introducidos
            if (that.validarCampos() == true) {
                // Realizar la petición de cambio de contraseña
                that.cambiarPassword();
            } else {
                that.passwordEmptyPanel.fadeIn("slow");
            }
        });
    },

    /**
     * Ocultar los paneles informativos de errores
     * */
    hideErrorPanels() {
        this.panelesError.forEach(panelError => panelError.hide());
    },

    /**
      * Vaciar de los inputs utilizados (Ej: Una vez efectuada la solicitud, vaciar los inputs)
      */
    emptyInputs() {
        this.inputsFields.forEach(input => input.val(''));
    },

    /**
     * Función para realizar la petición del cambio de contraseña solicitado por el usuario
     * */
    cambiarPassword: function () {
        // Referencia al objeto
        const that = this;        
        this.btnCambiarPassword.prop('disabled', true);
        
        MostrarUpdateProgress();
        // Construcción del objeto con los passwords
        const params = {
            OldPassword: that.txtOldPassword.val(),
            NewPassword: that.txtNewPassword.val(),
            ConfirmedPassword: that.txtConfirmedPassword.val(),
        };
        // Realizar la petición de cambio de password
        GnossPeticionAjax(that.urlPasswordRequest, params, true)
            .done(function () {
                // Ocultar posibles paneles de error
                that.hideErrorPanels();

                // Mostrar panel info con su clase
                that.passwordRequestInfoPanel.addClass(that.okClass);
                that.passwordRequestInfoPanel.removeClass(that.errorClass);
                var transfer = that.getParam("transferto", location.href);
                if (transfer != undefined) {
                    location.href = transfer;
                }
            })
            .fail(function () {
                // Mostrar panel info con su clase
                that.passwordRequestInfoPanel.removeClass(that.okClass);
                that.passwordRequestInfoPanel.addClass(that.errorClass);
            })
            .always(function (html) {
                // Mostrar el mensaje de success/error
                that.passwordRequestInfoPanel.html(html);
                // Mostrar el mensaje de error o de success
                that.passwordRequestInfoPanel.fadeIn("slow");
                // Mostrar de nuevo el botón para solicitar cambio de contraseña
                that.btnCambiarPassword.fadeIn("slow");
                // Vaciar los inputs
                that.emptyInputs();
                // Ocultar el loading
                OcultarUpdateProgress();
                // Volver a habilitar el botón
                that.btnCambiarPassword.prop('disabled', false);
            });
    },

    /**
     * Función ejecutada desde cambiarPassword 
     * @param {any} param
     * @param {any} url
     */
    getParam: function (param, url) {
        /* Buscar a partir del signo de interrogación ? */
        url = String(url.match(/\?+.+/));
        /* limpiar la cadena quitándole el signo ? */
        url = url.replace("?", "");
        /* Crear un array con parametro=valor */
        url = url.split("&");
        /*
        Recorrer el array url
        obtener el valor y dividirlo en dos partes a través del signo =
        0 = parametro
        1 = valor
        Si el parámetro existe devolver su valor
        */
        x = 0;
        while (x < url.length) {
            p = url[x].split("=");
            if (p[0] == param) {
                return decodeURIComponent(p[1]);
            }
            x++;
        }
    }
};

/**
 * Clase jquery para poder gestionar las búsquedas de fechas en Facetas (Mes pasado, semana pasada, semestre pasado)
 * Calculará la fecha teniendo en cuenta la opción pulsada para escribir el valor en el input "Desde" y "Hasta"
 * 
 * */
var operativaFechasFacetas = {

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
        this.optionLastWeek = `.facet-last-week`,
            // Opción buscador de fechas "Último mes"    
            this.optionLastMonth = `.facet-last-month`,
            // Opción buscador de fechas "Último semestre"    
            this.optionLastSemester = `.facet-last-semester`,
            // Opción buscador de fechas "Último semestre"    
            this.optionLastYear = `.facet-last-year`,
            // Botón de buscar 
            this.searchButton = `.searchButton`,
            this.dropdownMenu = `.dropdown-menu`,
            // Input from-to
            this.inputFromDate = `.facet-from`,
            this.inputToDate = `.facet-to`;

    },

    /**
     * Configuración eventos de elementos HTML
     * */
    configEvents: function () {

        const that = this;

        // Opción LastWeek
        $(document).on('click', this.optionLastWeek, function (event) {
            that.getAndSetDate(this);
        });
        // Opción LastMonth
        $(document).on('click', this.optionLastMonth, function (event) {
            that.getAndSetDate(this);
        });
        // Opción Semester
        $(document).on('click', this.optionLastSemester, function (event) {
            that.getAndSetDate(this);
        });
        // Opción Year
        $(document).on('click', this.optionLastYear, function (event) {
            that.getAndSetDate(this);
        });
    },

    /**
     * Calcular el plazo de tiempo deseado y establecerlo en los inputs "from" y "to"
     * */
    getAndSetDate: function (item) {
        let now = new Date();
        let dateFormatter = new Intl.DateTimeFormat("es-ES", {
            day: "2-digit",
            month: "2-digit",
            year: "numeric"
        });

        // Fecha inicial
        let startDate = new Date(now);
        // Fecha final (actual)
        let endDate = dateFormatter.format(now);

        if ($(item).hasClass(`facet-last-week`)) {
            startDate.setDate(startDate.getDate() - 7)
        } else if ($(item).hasClass(`facet-last-month`)) {
            startDate.setMonth(startDate.getMonth() - 1);
            // Si salta el caso de que es 31 de Marzo y quiero ver el mes anterior
            // Mostrará el 28/29 de Febrero en vez de una fecha incorrecta
            if (startDate.getDate() !== now.getDate()) {
                start.setDate(0);
            }
        } else if ($(item).hasClass(`facet-last-semester`)) {
            startDate.setMonth(startDate.getMonth() - 6);
            // Si estamos en un mes con 31 dias y 6 meses atrás acaba en un mes de 30 o 28 dias
            // mostrará el ultimo dia de dicho mes. Y no una fecha incorrecta
            if (startDate.getDate() !== now.getDate()) {
                start.setDate(0);
            }
        } else {
            // Selección del último año
            startDate.setFullYear(startDate.getFullYear() - 1);
        }

        // Botón para búsqueda,  inputs para establecer fechas
        const searchBtn = $(item).parent().parent().parent().find(this.searchButton);
        const fromDateValue = $(item).parent().parent().parent().find(this.inputFromDate);
        const toDateValue = $(item).parent().parent().parent().find(this.inputToDate);

        // Escribir las fechas en inputs
        fromDateValue.val(dateFormatter.format(startDate));
        toDateValue.val(endDate);

        // Hacer click en botón de búsqueda
        searchBtn.trigger("click");
    },
}


/**
 * Clase jquery para poder gestionar la "peticion" de solicitud de cambio de contraseña de un usuario
 * Este tipo de petición es ejecutada cuando el usuario ha solicitado cambio de contraseña (por olvido), ha recibido un email y ha accedido a esa url para 
 * proceder a cambiar su contraseña
 * 
 * */
const operativaPeticionCambiarContrasenia = {
    /**
     * Acción para inicializar elementos y eventos
     */
    init: function (pParams) {
        this.config(pParams);
        this.configEvents();
    },
    /*
     * Opciones de configuración de la vista con el formulario modal
     * */
    config: function (pParams) {

        // Inicialización de las vistas
        this.txtLogin = $(`#${pParams.idTxtLogin}`);
        this.txtPasswordNueva = $(`#${pParams.idTxtPasswordNueva}`);
        this.txtPasswordConfirmar = $(`#${pParams.idTxtPasswordConfirmar}`);
        this.btnCambiarPassword = $(`#${pParams.idBtnCambiarPassword}`);
        this.btnRechazarPeticionCambiarPassword = $(`#${pParams.idBtnRechazarPeticionCambiarPassword}`);
        this.panelErroresInformativo = $(`#${pParams.idPanelErroresInformativo}`);
        this.bloqMayInfoPanel = $(`#${pParams.idBloqMayInfoPanel}`);
        this.panelCambioPassword = $(`#${pParams.idPanelCambioPassword}`);

        // Inputs
        this.inputsFields = [this.txtLogin, this.txtPasswordNueva, this.txtPasswordConfirmar];
        // Paneles de errores/info
        this.panelesError = [this.panelErroresInformativo, this.bloqMayInfoPanel];
        // Mensajes de error preconfigurados
        this.errorMsgNoUsuario = pParams.errorMsgNoUsuario;
        this.errorMsgNoPassword = pParams.errorMsgNoPassword;
        this.errorPasswordNoIguales = pParams.errorPasswordNoIguales;
        this.okRejectPasswordMessage = pParams.okRejectPasswordMessage;
        this.okPasswordCambiado = pParams.okPasswordCambiado;

        // Url necesarias
        this.urlPasswordRequest = pParams.urlPasswordRequest;
        this.urlRejectPasswordRequest = pParams.urlRejectPasswordRequest;

        // Comprobación que los inputs han sido rellenados
        this.areInputsFilled = false;
        // Comprobación que las contraseñas coinciden
        this.arePasswordsTheSame = false;


        // Nombre de las clases css para estilo de error o success (Bootstrap)
        this.okClass = "alert-success";
        this.errorClass = "alert-danger";
    },

    /**
    * Comprobar que los campos (password old, password new y password confirmado ) no están vacíos
    * y que las contraseñas introducidas coinciden
    * @returns {bool}    
    */
    validarCampos: function () {

        if (this.txtLogin.val() != '' && this.txtPasswordNueva.val() != '' && this.txtPasswordConfirmar.val() != '') {
            this.areInputsFilled = true;
            // Comprobar que las contraseñas introducidas son iguales
            if (this.txtPasswordNueva.val() === this.txtPasswordConfirmar.val()) {
                this.arePasswordsTheSame = true;
            } else {
                this.arePasswordsTheSame = false;
            }
        }
    },

    /**
    * Configuración de los eventos 
    */
    configEvents: function () {
        // Referencia al objeto
        const that = this;

        // Input Password - Control de mayúsculas
        this.txtPasswordNueva.keypress(function (event) {
            // Avisar con un mensaje si están activadas las mayúsculas del teclado
            isCapsLock(event) ? that.bloqMayInfoPanel.fadeIn() : that.bloqMayInfoPanel.fadeOut();
        });

        // Input Password Confirmar - Control de mayúsculas + Enter
        this.txtPasswordConfirmar.keypress(function (event) {
            // Avisar con un mensaje si están activadas las mayúsculas del teclado
            isCapsLock(event) ? that.bloqMayInfoPanel.fadeIn() : that.bloqMayInfoPanel.fadeOut();
            event.keyCode === 13 ? that.btnCambiarPassword.click() : null
        });

        // Botón de Aceptar - Solicitar cambio de contraseña         
        this.btnCambiarPassword.click(function (event) {
            // Ocultar por defecto posibles mensajes de error
            that.hideErrorPanels();
            // Hacer login solo si los datos han sido introducidos
            that.validarCampos();
            if (that.arePasswordsTheSame == true && that.areInputsFilled == true) {
                // Realizar la petición de cambio de contraseña
                //that.cambiarPassword();
                that.cambiarPassword();
            } else {
                // Mostrar el panel de error correspondiente
                if (that.arePasswordsTheSame == true) {
                    that.panelErroresInformativo.html(that.errorPasswordNoIguales);
                } else if (that.txtLogin.val() == "") {
                    that.panelErroresInformativo.html(that.errorMsgNoUsuario);
                } else if (that.txtPasswordNueva.val() == "" || that.txtPasswordConfirmar.val() == "") {
                    that.panelErroresInformativo.html(that.errorMsgNoPassword);
                } else {
                    that.panelErroresInformativo.html(that.errorPasswordNoIguales);
                }
                that.panelErroresInformativo.fadeIn();
            }
        });


        // Link/ Botón de cancelar solicitud de cambio de contraseña
        this.btnRechazarPeticionCambiarPassword.click(function () {
            that.rechazarPeticion();
        });
    },

    /**
     * Ocultar los paneles informativos de errores
     * */
    hideErrorPanels() {
        this.panelesError.forEach(panelError => {
            panelError.hide();
            panelError.val('');
        });
    },

    /**
      * Vaciar de los inputs utilizados (Ej: Una vez efectuada la solicitud, vaciar los inputs)
      */
    emptyInputs() {
        this.inputsFields.forEach(input => input.val(''));
    },

    /**
     * Búscar los parámetros mandados por URL.
     * Este método es llamado desde la función que realiza la petición de cambio de password
     * @param {any} param: 
     * @param {any} url: Url a la que se realiza la petición para cambio de contraseña
     */
    getParam: function (param, url) {
        /* Buscar a partir del signo de interrogación ? */
        url = String(url.match(/\?+.+/));
        /* limpiar la cadena quitándole el signo ? */
        url = url.replace("?", "");
        /* Crear un array con parametro=valor */
        url = url.split("&");
        /*
        Recorrer el array url
        obtener el valor y dividirlo en dos partes a través del signo =
        0 = parametro
        1 = valor
        Si el parámetro existe devolver su valor
        */
        x = 0;
        while (x < url.length) {
            p = url[x].split("=");
            if (p[0] == param) {
                return decodeURIComponent(p[1]);
            }
            x++;
        }
    },

    /**
     * Función para realizar la petición del cambio de contraseña solicitado por el usuario
     * */
    cambiarPassword: function () {
        // Referencia al objeto
        const that = this;
        this.btnCambiarPassword.prop('disabled', true);

        MostrarUpdateProgress();
        // Construcción del objeto con los passwords
        const dataPost = {
            User: that.txtLogin.val(),
            Password: that.txtPasswordNueva.val(),
            PasswordConfirmed: that.txtPasswordConfirmar.val(),
        }

        // Realizar la petición de cambio de password
        GnossPeticionAjax(that.urlPasswordRequest, dataPost, true)
            .done(function () {
                // Ocultar posibles paneles de error
                that.hideErrorPanels();

                // Mostrar panel info con su clase
                that.panelErroresInformativo.addClass(that.okClass);
                that.panelErroresInformativo.removeClass(that.errorClass);
                that.panelErroresInformativo.html(that.okPasswordCambiado);
                var transfer = that.getParam("transferto", location.href);
                if (transfer != undefined) {
                    location.href = transfer;
                }
                // Destruimos o eliminamos el panel de inputs para que no pueda volver a solicitar cambio de contraseña
                that.panelCambioPassword.remove()
            })
            .fail(function (html) {
                // Mostrar panel info con su clase
                that.panelErroresInformativo.removeClass(that.okClass);
                that.panelErroresInformativo.addClass(that.errorClass);
                // Mostrar el mensaje de error
                that.panelErroresInformativo.html(html);
            })
            .always(function (html) {
                // Mostrar el mensaje de error o de success
                that.panelErroresInformativo.fadeIn();
                // Vaciar los inputs
                that.emptyInputs();
                // Ocultar el loading
                OcultarUpdateProgress();
                that.btnCambiarPassword.prop('disabled', false);
            });
    },

    /**
     * Función para cancelar o rechazar la solicitud de cambio de contraseña.
     * */
    rechazarPeticion: function () {
        const that = this;
        MostrarUpdateProgress();
        // Ocultar posibles paneles de error
        this.hideErrorPanels();

        GnossPeticionAjax(this.urlRejectPasswordRequest, null, true).done(function () {
            // Destruimos o eliminamos el panel de inputs para que no pueda volver a solicitar cambio de contraseña
            that.panelCambioPassword.remove()
            that.btnCambiarPassword.remove()
            // Mostrar mensaje de cancelar la solicitud de cambio de password
            that.panelErroresInformativo.html(that.okRejectPasswordMessage);
            that.panelErroresInformativo.addClass(that.okClass);
            that.panelErroresInformativo.removeClass(that.errorClass);
            that.panelErroresInformativo.fadeIn();
            OcultarUpdateProgress();
        });
    },
};

/**
 * Clase jquery para poder realizar envíos de invitaciones a una comunidad y de links de recursos (desde la ficha de recurso) a correos o contactos de una comunidad.
 * Para acceder a esta vista se accederá 
 *  - Desde la propia ficha de recurso (Enviar Link)
 *  - Panel lateral del usuario si dispone de permisos en la comunidad para enviar invitaciones 
 * */
const operativaEnviarResource_Link_Community_Invitation = {
    /**
     * Acción para inicializar elementos y eventos
     */
    init: function (pParams) {
        this.config(pParams);
        this.configEvents();
        if (pParams.autocompleteParams) {
            this.configAutocompleteService(pParams.autocompleteParams);
            this.configAutocompleteServiceForCommunityGroups(pParams.autocompleteParams)
        }
    },
    /*
     * Opciones de configuración de la vista
     * */
    config: function (pParams) {
        // Inicialización de las vistas
        // Inicialización de las vistas
        this.txtFiltro = $(`#${pParams.idTxtFiltro}`);
        this.txtFiltroGrupos = $(`#${pParams.idTxtFiltroGrupos}`);
        this.txtCorreoAInvitar = $(`#${pParams.idTxtCorreoAInvitar}`);
        this.buttonLitAniadirCorreo = $(`#${pParams.idButtonLitAniadirCorreo}`);
        this.txtHackInvitados = $(`#${pParams.idTxtHackInvitados}`);
        this.txtHackGrupos = $(`#${pParams.idTxtHackGrupos_invite_community}`);
        // El autocomplete necesita solo el nombre del input oculto
        this.txtHackInvitadosInputName = pParams.idTxtHackInvitados;
        // El autocomplete necesita solo el nombre del input oculto
        this.txtHackGruposInvitadosInputName = pParams.idTxtFiltroGrupos;

        this.panContenedorInvitados = $(`#${pParams.idPanContenedorInvitados}`);
        this.listaDestinatarios = $(`#${pParams.idListaDestinatarios}`);
        this.listaGrupos = $(`#${pParams.idPanContenedorGrupos}`);
        this.noDestinatarios = $(`#${pParams.idNoDestinatarios}`);
        this.btnEnviarInvitaciones = $(`#${pParams.idBtnEnviarInvitaciones}`);
        this.lblInfoCorreo = $(`#${pParams.idLblInfoCorreo}`);
        this.panelInfoInvitationSent = $(`#${pParams.idPanelInfoInvitationSent}`);

        // Campos especiales para envío de link (idioma & notas/mensaje)                
        this.txtNotas = $(`#${pParams.idTxtNotas}`);
        this.dllIdioma = $(`#${pParams.idDlIdioma}`);

        // Paneles de error/info
        this.panelesInfo = [this.panelInfoInvitationSent];

        // Url necesarias para realizar petición        
        this.urlSend = pParams.urlSend;
    },

    /**
    * Ocultar los paneles informativos (errores o info)
    **/
    hideInfoPanels() {
        this.panelesInfo.forEach(panel => panel.hide());
    },

    /**
    * Configuración de los eventos
    */
    configEvents: function () {
        const that = this;

        // Botón de Aceptar - Solicitar link para cambio de contraseña
        this.btnEnviarInvitaciones.click(function (event) {
            // Ocultar por defecto posibles mensajes de error
            that.hideInfoPanels();
            // Comprobar que el input de invitados (el oculto que almacena los ids al menos tiene emails o contactos)
            if (that.validarCampos()) {
                // Realizar la petición de cambio de contraseña
                that.enviarInvitacion_EnlaceSubmit();
            }
        });

        // Botón de Añadir correo 
        this.buttonLitAniadirCorreo.click(function () {
            let listaCorreos = that.txtCorreoAInvitar.val().trim().replace(/^\s*|\s*$/g, "").split(",");
            for (var i = 0; i < listaCorreos.length; i++) {
                let itemCorreo = listaCorreos[i].trim();
                if (!validarEmail(itemCorreo)) {
                    mostrarNotificacion('error', form.emailValido);
                } else {
                    that.crearInvitado(null, itemCorreo, itemCorreo, true);
                }
            }
        });

        // Configurar el borrado de elementos al pulsar en (x) de un item de los destinatarios
        this.listaDestinatarios.on('click', '.tag-remove', function (event) {
            const identidad = $(event.target).parent().parent().attr("id");
            that.eliminarUsuario(null, identidad);
        });


        // Configurar el borrado de grupos al pulsar en (x) de un item de grupos        
        this.listaGrupos.on('click', '.tag-remove', function (event) {
            const identidad = $(event.target).parent().parent().attr("id");
            that.eliminarGrupo(null, identidad);
        });
    },

    /**
    * Comprobar que el campo oculto que contiene posibles destinatarios tiene cierta longitud
    * @returns {bool}    
    */
    validarCampos: function () {
        return (this.txtHackInvitados.val().length > 3);
    },

    /**
     * Acción que se ejecuta cuando se realice una búsqueda escribiendo el nombre de un usuario y al pulsar en uno de los resultados devueltos por autocomplete.
     * Con los datos devueltos, construirá el item y lo  meterá en "panContenedorInvitados". *@
     * @param {any} ficha
     * @param {any} nombre: El nombre del usuario seleccionado
     * @param {any} identidad: La identidad del item seleccionado
     * @param {boolean} isAnEmail: Valor que indicará si lo que se está intentando añadir es un contacto (normal) o un email de un usuario
     */
    crearInvitado: function (ficha, nombre, identidad, isAnEmail) {
        // Item que se añadirá como elemento seleccionado
        let itemHtml = "";

        if (!isAnEmail) {
            itemHtml += `<div class="tag" id="${identidad}" data-item="${identidad}">`;
            itemHtml += `<div class="tag-wrap">`;
            itemHtml += `<span class="tag-text">${nombre}</span>`;
            itemHtml += `<span class="tag-remove material-icons">close</span>`;
            itemHtml += `</div>`;
            itemHtml += `</div>`;
            // Añadir la identidad al input de invitados
            this.txtHackInvitados.val(`${this.txtHackInvitados.val()}&${identidad}`);
        } else {
            // Construyo correos separados por comas
            const correos = this.txtCorreoAInvitar.val().split(',');
            // Validar si son correos válidos     
            for (let i = 0; i < correos.length; i++) {
                if (correos[i] != '') {
                    if (!validarEmail(correos[i].replace(/^\s*|\s*$/g, ""))) {
                        // No es email válido, muestra mensaje de error
                        this.lblInfoCorreo.html(form.emailValido);
                        this.lblInfoCorreo.parent().parent().fadeIn();
                        return;
                    } else {
                        this.lblInfoCorreo.parent().parent().fadeOut();
                    }
                }
            }
            // Recorrer array de correos para ser añadidos a la vista
            for (let i = 0; i < correos.length; i++) {
                if (correos[i] != '') {
                    let data_item = correos[i].replace(/\@/g, '_');
                    data_item = data_item.replace(".", '_');
                    itemHtml += `<div class="tag" id="${correos[i]}" data-item="${data_item}">`;
                    itemHtml += `<div class="tag-wrap">`;
                    itemHtml += `<span class="tag-text">${correos[i]}</span>`;
                    itemHtml += `<span class="tag-remove material-icons">close</span>`;
                    itemHtml += `</div>`;
                    itemHtml += `</div>`;
                    // Añadir el correo al input de invitados
                    this.txtHackInvitados.val(`${this.txtHackInvitados.val()}&${correos[i].replace(/^\s*|\s*$/g, "")}`);
                }
            }
        }

        // Añadir el item en el contenedor de destinatarios
        this.listaDestinatarios.append(itemHtml);

        // Ocultar el panel de "No destinatarios" ya que hay añadidos
        this.noDestinatarios.fadeOut();

        // Vaciamos el input donde se ha introducido al usuario
        isAnEmail ? this.txtCorreoAInvitar.val('') : this.txtFiltro.val('');
        // Quitamos posible mensaje de error de correo añadido
        this.lblInfoCorreo.val('');
        this.lblInfoCorreo.hide();

        if (ficha != null) {
            ficha.style.display = 'none';
        }
    },

    /**
     * Acción que se ejecuta cuando se realice una búsqueda escribiendo el nombre de un grupo de la comunidad y se seleccione un item de resultados devueltos por autocomplete.
     * Con los datos devueltos, construirá el item y lo  meterá en "panContenedorInvitados". *@     
     * @param {any} nombre: El nombre del usuario seleccionado
     * @param {any} identidad: La identidad del item seleccionado     
     */
    crearGrupoInvitado: function (nombre, identidad) {
        // Item que se añadirá como elemento seleccionado
        let itemHtml = "";

        itemHtml += `<div class="tag" id="${identidad}" data-item="${identidad}">`;
        itemHtml += `<div class="tag-wrap">`;
        itemHtml += `<span class="tag-text">${nombre}</span>`;
        itemHtml += `<span class="tag-remove material-icons">close</span>`;
        itemHtml += `</div>`;
        itemHtml += `</div>`;

        // Añadir la identidad al input de grupos    
        this.txtHackGrupos.val(`${this.txtHackGrupos.val()}&${identidad}`);

        // Añadir el item en el contenedor de grupos
        this.listaGrupos.append(itemHtml);

        // Vaciamos el input donde se ha introducido el grupo a buscar
        this.txtFiltroGrupos.val('');
    },

    /**
     * Acción que eliminará a un elemento al pulsar sobre su (x). Desaparecerá del contenedor y del input oculto que contiene
     * los items seleccionados para el envío de la solicitud
     * @param {any} fichaId             
     */
    eliminarUsuario: function (fichaId, identidad) {

        // Eliminar la identidad al input construyendo el nuevo valor que tomará
        let newTxtHackInvitados = this.txtHackInvitados.val().replace('&' + identidad, '');
        this.txtHackInvitados.val(newTxtHackInvitados);

        // Tratar de eliminar caracteres especiales para buscar el atributo de data-item (para casos de correos electrónicos)
        let data_item = identidad.replace(/\@/g, '_');
        data_item = data_item.replace(".", '_');
        const itemToDelete = $(`*[data-item="${data_item}"]`);
        itemToDelete.remove();

        // Comprobar si hay items para mostrar u ocultar mensaje de "Ningún destinatario..."
        const numItems = this.listaDestinatarios.children().length;
        numItems >= 1 ? this.noDestinatarios.hide() : this.noDestinatarios.show();
    },

    /**
         * Acción que eliminará a un elemento al pulsar sobre su (x). Desaparecerá del contenedor y del input oculto que contiene
         * los items seleccionados para el envío de la solicitud
         * @param {any} fichaId             
         */
    eliminarGrupo: function (fichaId, identidad) {

        // Eliminar la identidad al input construyendo el nuevo valor que tomará
        let newTxtHackInvitados = this.txtHackGrupos.val().replace('&' + identidad, '');
        this.txtHackGrupos.val(newTxtHackInvitados);

        // Tratar de eliminar caracteres especiales para buscar el atributo de data-item (para casos de correos electrónicos)
        let data_item = identidad.replace(/\@/g, '_');
        data_item = data_item.replace(".", '_');
        const itemToDelete = $(`*[data-item="${data_item}"]`);
        itemToDelete.remove();
    },

    /**
     * Configuración del servicio autocomplete para el input buscador de nombres
     * Se pasaran los parámetros necesarios los cuales se han obtenido de la vista
     * @param {any} autoCompleteParams
     */
    configAutocompleteService(autoCompleteParams) {
        const that = this;

        // Objeto que albergará los extraParams para el servicio autocomplete
        let extraParams = {};

        // Configuración de extraParams dependiendo isEcosistemasinMetaProyecto
        if (autoCompleteParams.isEcosistemasinMetaProyecto) {
            extraParams = {
                identidad: autoCompleteParams.identidad,
                identidadMyGnoss: autoCompleteParams.identidadMyGnoss,
                identidadOrg: autoCompleteParams.identidadOrg,
                proyecto: autoCompleteParams.proyecto,
                bool_esPrivada: autoCompleteParams.esPrivada
            }
        } else {
            extraParams = {
                identidad: autoCompleteParams.identidad,
                proyecto: autoCompleteParams.proyecto,
            }
        }
        // Configuración del autocomplete para el input de búsqueda de nombres
        this.txtFiltro.autocomplete(
            null,
            {
                //servicio: new WS($(`#${autoCompleteParams.idInpt_urlServicioAutocompletar}`).val(), WSDataType.jsonp),
                //metodo: autoCompleteParams.metodo,
                url: $(`#${autoCompleteParams.idInpt_urlServicioAutocompletar}`).val() + '/' + autoCompleteParams.metodo,
                type: "POST",
                delay: 0,
                multiple: true,
                scroll: false,
                selectFirst: false,
                minChars: 1,
                width: 300,
                cacheLength: 0,
                NoPintarSeleccionado: true,
                txtValoresSeleccID: that.txtHackInvitadosInputName,
                extraParams,
            }
        );

        // Configuración la acción select (cuando se seleccione un item de autocomplete)
        this.txtFiltro.result(function (event, data, formatted) {
            that.crearInvitado(null, data[0], data[1], false);
        });
    },

    /**
     * Configuración del servicio autocomplete para el input buscador de nombres de grupos de la comunidad
     * Se pasaran los parámetros necesarios los cuales se han obtenido de la vista
     * @param {any} autoCompleteParams
     */
    configAutocompleteServiceForCommunityGroups(autoCompleteParams) {
        const that = this;

        // Configurar input de autocomplete para grupos de la comunidad
        this.txtFiltroGrupos.keydown(function (event) {
            if (event.which || event.keyCode) { if ((event.which == 13) || (event.keyCode == 13)) { return false; } }
        }).autocomplete(
            null,
            {
                url: $(`#${autoCompleteParams.idInpt_urlServicioAutocompletar}`).val() + '/' + "AutoCompletarGruposInvitaciones",
                delay: 0,
                multiple: true,
                scroll: false,
                selectFirst: false,
                minChars: 1,
                width: 300,
                cacheLength: 0,

                NoPintarSeleccionado: true,
                txtValoresSeleccID: that.txtHackGruposInvitadosInputName,

                extraParams: {
                    identidad: autoCompleteParams.identidad,
                    identidadMyGnoss: autoCompleteParams.identidadMyGnoss,
                    identidadOrg: autoCompleteParams.identidadOrg,
                    proyecto: autoCompleteParams.proyecto,
                }
            });

        // Configuración la acción select (cuando se seleccione un item de autocomplete) para grupos de la comunidad
        this.txtFiltroGrupos.result(function (event, data, formatted) {
            that.crearGrupoInvitado(data[0], data[1]);
        });
    },

    /**
    * Acción de envío de la invitación de la comunidad o del enlace
    * Se disparará al pulsar el botón de "Enviar"
    */
    enviarInvitacion_EnlaceSubmit: function () {
        const that = this;

        MostrarUpdateProgress();

        // Construcción del objeto dataPost
        let dataPost = {};
        // Construir la URL teniendo en cuenta el tipo de envío
        let newUrlRequest = "";
        //Tener en cuenta de si no existe el idioma -> Invitación de comunidad
        if (this.dllIdioma.length == 0) {
            if (that.txtNotas.val() == undefined) {
                dataPost = {
                    Guests: that.txtHackInvitados.val(),
                    Message: encodeURIComponent(that.txtNotas.val().replace(/\n/g, '')),
                    Groups: that.txtHackGrupos.val()
                };
            }
            else {
                dataPost = {
                    Guests: that.txtHackInvitados.val(),
                    Message: encodeURIComponent(that.txtNotas.val().replace(/\n/g, '')),
                    Groups: that.txtHackGrupos.val()
                };
            }
            newUrlRequest = that.urlSend + document.location.search;
        } else {
            dataPost = {
                Receivers: that.txtHackInvitados.val(),
                Message: encodeURIComponent(that.txtNotas.val().replace(/\n/g, '')),
                Lang: that.dllIdioma.val()
            }
            newUrlRequest = that.urlSend;
        }

        GnossPeticionAjax(newUrlRequest,
            dataPost,
            true
        ).done(function (response) {
            claseDiv = "ok";
            // 2 - Cerrar modal                        
            $('#modal-container').modal('hide');
            // 3 - Mostrar mensaje OK
            setTimeout(() => {
                mostrarNotificacion('success', response);
            }, 1500)
        }).fail(function (response) {
            claseDiv = "ko";
            /* Mostrar error */
            // 3 - Mostrar mensaje KO
            setTimeout(() => {
                mostrarNotificacion('error', response);
            }, 1500)
        }).always(function (response) {
            OcultarUpdateProgress();
        });
    }
};

/**
 * Clase jquery para poder enviar un recurso a los participantes de grupos de la comunidad
 *  - Desde la ficha de un recurso
 * */
const operativaEnviarRecursoParticipantesGruposComunidad = {

    /**
    * Inicializar operativa
    */
    init: function (pParams) {
        this.pParams = pParams;
        this.config(pParams);
        this.configEvents();
        if (pParams.autocompleteParams) {
            this.configAutocompleteService(pParams.autocompleteParams)
        }
        this.configRutas(pParams);
        this.triggerEvents();
    },

    /**
     * Lanzar comportamientos u operativas necesarias para el funcionamiento de la sección
     */
    triggerEvents: function () {
        const that = this;

        // Permitir enviar o no dependiendo del botón del envío
        this.handleCheckIfLanguageIsSelected();
    },

    /*
    * Inicializar elementos de la vista
    * */
    config: function (pParams) {
        const that = this;

        // Input para buscar grupos de autocomplete
        this.txtFilterGroupsId = "txtFilterGroupsNewsletter";
        this.txtFilterGroups = $(`#${this.txtFilterGroupsId}`);
        // Input hack para la selección de items
        this.txtHackGroupsSendNewsletterId = "txtHackGroupsSendNewsletter";
        this.txtHackGroupsSendNewsletter = $(`#${this.txtHackGroupsSendNewsletterId}`);
        // Botón para hacer el envío de la newsletter
        this.btnSendNewsletterToCommunityGroupsId = "btnSendNewsletterToCommunityGroups";
        this.btnSendNewsletterToCommunityGroups = $(`#${this.btnSendNewsletterToCommunityGroupsId}`);
        // Panel o contenedor de los usuarios seleccionados para el envío
        this.panGroupsContainerSendNewsletterId = "panGroupsContainerSendNewsletter";
        this.panGroupsContainerSendNewsletter = $(`#${this.panGroupsContainerSendNewsletterId}`);
        // Checkbox/RadioButton de los idiomas en los que se desea enviar la newsletter
        this.selectLanguageOptionsClassName = "rbSelectNewsletterLanguage";
        this.selectLanguageOptions = $(`.${this.selectLanguageOptionsClassName}`);

        // Flag para controlar si hay al menos una opción seleccionada
        this.isLanguageOptionSelected = false;
        this.languageSelected = "";
    },

    /**
     * Configuración de las rutas de acciones necesarias para hacer peticiones a backend
     */
    configRutas: function (pParams) {
        // Url para editar un certificado
        this.urlSendResourceToCommunityGroups = pParams.urlSend;
    },

    /**
     * Configuración de eventos de elementos del Dom (Botones, Inputs...)     
     */
    configEvents: function (pParams) {
        const that = this;

        // Configurar el borrado de grupos al pulsar en (x) de un item de grupos        
        this.panGroupsContainerSendNewsletter.on('click', '.tag-remove', function (event) {
            const groupId = $(event.target).parent().parent().attr("id");
            that.removeGroup(groupId);
            that.handleCheckIfLanguageIsSelected();
        });

        // Agrega un evento change a los radio buttons
        this.selectLanguageOptions.on("change", function () {
            that.handleCheckIfLanguageIsSelected();
            // Guardar el idioma seleccionado
            that.languageSelected = $(this).data("language");
        });

        // Acción de enviar la newsletter a los grupos elegidos
        this.btnSendNewsletterToCommunityGroups.on("click", function () {
            that.handleSendNewsletterToCommunityGroups();
        });
    },

    /**
     * Método para controlar si hay al menos un idioma seleccionado para posibilitar el envío de la newsletter
     */
    handleCheckIfLanguageIsSelected: function () {
        const that = this;

        // Flag para comprobar que haya grupos seleccionados para el envío
        let areGroupsSelected = false;

        // Tener en cuenta que hay grupos destinatarios        
        var txtHackGroupsWithoutComma = this.txtHackGroupsSendNewsletter.val().replace(/,/g, '');

        // Verifica si el valor resultante sin comas está vacío o no
        if (txtHackGroupsWithoutComma.trim() != '') {
            // Con destinatarios
            areGroupsSelected = true;
        }

        // Verifica si al menos un radio button está seleccionado
        if (that.selectLanguageOptions.is(":checked") && areGroupsSelected) {
            // Habilita el botón
            that.btnSendNewsletterToCommunityGroups.prop("disabled", false);
            that.isLanguageOptionSelected = true;
        } else {
            // Deshabilita el botón si ninguno está seleccionado
            that.btnSendNewsletterToCommunityGroups.prop("disabled", true);
            that.isLanguageOptionSelected = false;
        }
    },

    /**
     * Método para configurar el servicio autocomplete/select
     */
    configAutocompleteService: function (autoCompleteParams) {
        const that = this;

        // Objeto que albergará los extraParams para el servicio autocomplete
        let extraParams = {};

        // Configuración de extraParams dependiendo isEcosistemasinMetaProyecto
        if (autoCompleteParams.isEcosistemasinMetaProyecto) {
            extraParams = {
                lista: ",",
                identidad: autoCompleteParams.identidad,
                identidadMyGnoss: autoCompleteParams.identidadMyGnoss,
                identidadOrg: autoCompleteParams.identidadOrg,
                proyecto: autoCompleteParams.proyecto,
                bool_esPrivada: autoCompleteParams.esPrivada
            }
        } else {
            extraParams = {
                lista: ",",
                identidad: autoCompleteParams.identidad,
                proyecto: autoCompleteParams.proyecto,
            }
        }
        // Configuración del autocomplete para el input de búsqueda de nombres
        this.txtFilterGroups.autocomplete(
            null,
            {
                url: $(`#${autoCompleteParams.idInpt_urlServicioAutocompletar}`).val() + '/' + autoCompleteParams.metodo,
                type: "POST",
                delay: 0,
                multiple: true,
                scroll: false,
                selectFirst: false,
                minChars: 1,
                width: 300,
                cacheLength: 0,
                NoPintarSeleccionado: true,
                txtValoresSeleccID: that.txtHackGroupsSendNewsletterId,
                extraParams,
            }
        );

        // Configuración la acción select (cuando se seleccione un item de autocomplete)
        this.txtFilterGroups.result(function (event, data, formatted) {
            that.handleSelecGroup(data[0], data[1]);
            that.handleCheckIfLanguageIsSelected();
        });
    },

    /**
     * Método para seleccionar un item a partir de la lista de autocomplete
     */
    /**
     * Método para seleccionar un item a partir de la lista de autocomplete
     * @param groupName: Nombre del grupo seleccionado
     * @param groupId: Id o Key del grupo seleccionado
     */
    handleSelecGroup: function (groupName, groupId) {
        const that = this;

        // Item que se añadirá como elemento seleccionado
        let itemHtml = "";

        itemHtml += `<div class="tag" id="${groupId}" data-item="${groupId}">`;
        itemHtml += `<div class="tag-wrap">`;
        itemHtml += `<span class="tag-text">${groupName}</span>`;
        itemHtml += `<span class="tag-remove material-icons">close</span>`;
        itemHtml += `</div>`;
        itemHtml += `</div>`;

        // Añadir la identidad al input de grupos (Separado por comas)            
        this.txtHackGroupsSendNewsletter.val(`${this.txtHackGroupsSendNewsletter.val()},${groupId}`);

        // Añadir el item en el contenedor de grupos
        this.panGroupsContainerSendNewsletter.append(itemHtml);

        // Vaciamos el input donde se ha introducido el grupo a buscar
        this.txtFilterGroups.val('');
    },

    /**
     * Acción que eliminará a un elemento al pulsar sobre su (x). Desaparecerá del contenedor y del input oculto que contiene
     * los items seleccionados para el envío de la solicitud
     * @param {any} groupId             
     */
    removeGroup: function (groupId) {

        // Eliminar la groupId al input construyendo el nuevo valor que tomará
        let newTxtHackInvitados = this.txtHackGroupsSendNewsletter.val().replace(',' + groupId, '');
        this.txtHackGroupsSendNewsletter.val(newTxtHackInvitados);

        // Tratar de eliminar caracteres especiales para buscar el atributo de data-item (para casos de correos electrónicos)
        let data_item = groupId.replace(/\@/g, '_');
        data_item = data_item.replace(".", '_');
        const itemToDelete = $(`*[data-item="${data_item}"]`);
        itemToDelete.remove();
    },

    /**
     * Método para enviar las newsletter a los grupos elegidos de la comunidad
     */
    handleSendNewsletterToCommunityGroups: function () {
        const that = this;

        // Obtén el valor de los grupos
        const txtGroupsSendNewsletterValue = this.txtHackGroupsSendNewsletter.val();
        // Generar un array excluyendo posibles espacios vacíos
        const arrayItems = txtGroupsSendNewsletterValue.split(',');
        const arrayGroupsSendNewsletterValue = arrayItems.filter(function (item) {
            return item.trim() !== '';
        });


        MostrarUpdateProgress();

        // Construcción del objeto dataPost
        let dataPost = {
            Language: that.languageSelected,
            Groups: arrayGroupsSendNewsletterValue,
        };

        GnossPeticionAjax(
            that.urlSendResourceToCommunityGroups,
            dataPost,
            true
        ).done(function (response) {
            // Cerrar modal y mostrar                         
            $('#modal-container').modal('hide');
            // 3 - Mostrar mensaje OK
            setTimeout(() => {
                mostrarNotificacion('success', response);
            }, 1500)
        }).fail(function (response) {
            setTimeout(() => {
                mostrarNotificacion('error', response);
            }, 1500)
        }).always(function (response) {
            OcultarUpdateProgress();
        });
    },
};

/**
 * Clase jquery para poder gestionar la suscripción de un usuario a las categorías de una comunidad
 * Se puede acceder desde el panel lateral del usuario, pulsando en "Suscribirse".
 * Podrá elegir (mediante checkbox disponibles) las categorías a las que suscribirse y si 
 * desea recibir newsletters de forma diaria o semanal
 * */

const operativaGestionarSuscripcionComunidad = {
    /**
     * Acción para inicializar elementos y eventos
     */
    init: function (pParams) {
        this.config(pParams);
        this.configEvents();
    },

    /*
     * Opciones de configuración de la vista
     * */
    config: function (pParams) {
        // Inicialización de las vistas
        this.chkRecibirBoletin = $(`#${pParams.idChkRecibirBoletin}`);
        this.panelFrecuenciaRecibirBoletin = $(`#${pParams.idPanelFrecuenciaRecibirBoletin}`);
        this.radioNameSuscription = $(`#${pParams.nameSuscripcion}`);
        this.txtHackCatTesSel = $(`#${pParams.idTxtHackCatTesSel}`);
        this.panelInfoSuscripcionCategorias = $(`#${pParams.idPanelInfoSuscripcionCategorias}`);
        this.rbtnSuscripcionDiaria = $(`#${pParams.idRbtnSuscripcionDiaria}`);
        this.rbtnSuscripcionSemanal = $(`#${pParams.idRbtnSuscripcionSemanal}`);
        this.btnSaveSubscriptionPreferences = $(`#${pParams.idBtnSaveSubscriptionPreferences}`);

        // Valores por defecto de Recibir boletines y su frecuencia (por defecto, false)
        this.isReceivingNewsletters = false;
        this.isFrequencyDaily = false;
        this.isFrequencyWeekly = false;

        // Url necesaria a la que habrá que hacer la petición
        this.urlRequestCommunitySubscription = pParams.urlRequestCommunitySubscription;
        // Paneles de info/error                                     
        this.panelesInfo = [this.panelInfoSuscripcionCategorias];

        // Nombre de las clases css para estilo de error o success (Bootstrap)
        this.okClass = "alert-success";
        this.errorClass = "alert-danger";
    },

    /**
    * Ocultar los paneles informativos (errores o info)
    **/
    hideInfoPanels() {
        this.panelesInfo.forEach(panel => panel.hide());
    },

    /**
    * Configuración de los eventos
    */
    configEvents: function () {
        const that = this;

        // Checkbox (si existe) para mostrar la frecuencia de boletines newsletter
        if (this.chkRecibirBoletin != undefined) {
            this.chkRecibirBoletin.on("click", function () {
                if ($(this).is(':checked')) {
                    that.panelFrecuenciaRecibirBoletin.fadeIn();
                    // Dejar checked por defecto una opción del radioButton (si antes no se ha seleccionado nada)
                    if (!that.rbtnSuscripcionDiaria.is(':checked') && !that.rbtnSuscripcionSemanal.is(':checked')) {
                        // Dejar por defecto la opción diaria checkeada
                        that.rbtnSuscripcionDiaria.prop("checked", true);
                    }
                } else {
                    that.panelFrecuenciaRecibirBoletin.fadeOut();
                }
            });
        }
        // Botón de guardar cambios/enviar al servidor
        this.btnSaveSubscriptionPreferences.on("click", function () {
            // Comprobar los valores para el envío
            that.checkRadioButtonsAndCheckValues();
            // Enviar los datos
            that.gestionSuscripcionComunidadSubmit();
        });
    },

    /**
     * Método para comprobar los checks y radioButtons para crear los valores booleanos
     * para el envío de datos al servidor
     * Comprueba primero si existen los inputs y una vez comprobado, analiza los datos
     * */
    checkRadioButtonsAndCheckValues: function () {
        if (this.chkRecibirBoletin != undefined) {
            if (this.chkRecibirBoletin.is(':checked')) {
                this.isReceivingNewsletters = true;
                // Comprobación diaria o semanal
                this.rbtnSuscripcionDiaria.is(':checked') ? this.isFrequencyDaily = true : this.isFrequencyDaily = false;
                this.rbtnSuscripcionSemanal.is(':checked') ? this.isFrequencyWeekly = true : this.isFrequencyWeekly = false;
            }
        } else {
            this.chkRecibirBoletin.is(':checked') ? this.isReceivingNewsletters = true : this.isReceivingNewsletters = false;
        }
    },

    /**
    * Acción de envío de ajustes en suscripción de la comunidad
    * Se disparará al pulsar el botón de "Enviar"
     * */
    gestionSuscripcionComunidadSubmit: function () {
        const that = this;

        // Mostrar loading
        MostrarUpdateProgress();
        // Ocultar posibles mensajes de info/error
        this.hideInfoPanels();
        // Construcción del objeto para enviar datos
        const dataPost = {
            SelectedCategories: that.txtHackCatTesSel.val(),
            ReceiveSubscription: that.isReceivingNewsletters,
            DailySubscription: that.isFrequencyDaily,
            WeeklySubscription: that.isFrequencyWeekly
        }

        // Envio de los datos
        GnossPeticionAjax(
            that.urlRequestCommunitySubscription,
            dataPost,
            true
        ).done(function () {
            that.panelInfoSuscripcionCategorias.addClass(that.okClass);
            that.panelInfoSuscripcionCategorias.removeClass(that.errorClass);
        }).fail(function () {
            that.panelInfoSuscripcionCategorias.removeClass(that.okClass);
            that.panelInfoSuscripcionCategorias.addClass(that.errorClass);
        }).always(function (response) {
            // Mostrar el mensaje de error
            that.panelInfoSuscripcionCategorias.html(response);
            that.panelInfoSuscripcionCategorias.fadeIn();
            OcultarUpdateProgress();
        });
    },
};

/**
 * Clase jquery para poder gestionar la solicitud de recibir (o no) de newsletters de la comunidad
 * Se puede acceder desde el panel lateral del usuario, pulsando en "Recibir newsletter".
 *
 * */
const operativaSolicitarRecibirNewsletter = {
    /**
     * Acción para inicializar elementos y eventos
     */
    init: function (pParams) {
        this.config(pParams);
        this.configEvents();
    },
    /*
     * Opciones de configuración de la vista
     * */
    config: function (pParams) {
        // Inicialización de IDS de las vistas  
        this.idChkRecibirNewsletter = pParams.chkRecibirNewsletter;
        this.idChkNoRecibirNewsletter = pParams.chkNoRecibirNewsletter;
        this.btnSubmitReceiveNewsletters = $(`#${pParams.btnSubmitReceiveNewsletters}`);

        // Nombre de los inputs
        this.nameChkReceiveNewsletter = pParams.nameChkReceiveNewsletter;

        // Inicialización de las vistas
        // Panel de posibles mensajes
        this.chkRecibirNewsletterNameInfoPanel = $(`#${pParams.chkRecibirNewsletterNameInfoPanel}`);

        // Nombre de las clases css para estilo de error o success (Bootstrap)
        this.okClass = "alert-success";
        this.errorClass = "alert-danger";

        // Url necesaria a la que habrá que hacer la petición
        this.urlRequestReceiveNewsletters = pParams.urlRequestReceiveNewsletters;

        // Inicialización de las vistas
        this.panelesInfo = [this.chkRecibirNewsletterNameInfoPanel];
    },

    /**
    * Ocultar los paneles informativos (errores o info)
    **/
    hideInfoPanels() {
        this.panelesInfo.forEach(panel => panel.hide());
    },

    /**
    * Configuración de los eventos
    */
    configEvents: function () {
        const that = this;

        // Botón de Aceptar - Solicitar envío o no de newsletters
        this.btnSubmitReceiveNewsletters.click(function (event) {
            // Ocultar por defecto posibles mensajes de error
            that.hideInfoPanels();

            // Conocer el inputRadio activo. Será el que comparemos con idChkRecibirNewsletter o idChkNoRecibirNewsletter (Desea o no desea newsletters)
            that.checkRecibirNewsLetterValue = $(`input[name=${that.nameChkReceiveNewsletter}]:checked`).val();
            // Realizar la petición de cambio de contraseña
            const isReceivingNewsletters = that.checkRecibirNewsLetterValue === that.idChkRecibirNewsletter ? true : false;
            that.recibirNewsletterSubmit(isReceivingNewsletters);
        });
    },

    /**
     * Acción de petición de recibir (o no) de newsletters. Se dispara cuando se pulsa en el botón submit del modal     
     * @param {Bool} option: Dependiendo del input pulsado, el usuario querrá o no recibir newsletters.
     */
    recibirNewsletterSubmit: function (option) {
        const that = this;
        MostrarUpdateProgress();
        that.hideInfoPanels();
        // Construcción del DataPost para enviar la petición
        const dataPost = {
            recibirNewsletter: option,
        };

        // Realizar la petición 
        GnossPeticionAjax(
            this.urlRequestReceiveNewsletters,
            dataPost,
            true
        ).done(function () {
            // Ocultar posibles paneles de info/error
            that.hideInfoPanels();
            // Mostrar panel info con su clase
            that.chkRecibirNewsletterNameInfoPanel.addClass(that.okClass);
            that.chkRecibirNewsletterNameInfoPanel.removeClass(that.errorClass);

            // Ocultar modal pasados 1.5 segundos
            setTimeout(() => {
                $('#modal-receive-newsletters').modal('hide');
            }, 1500);

        }).fail(function () {
            // Ocultar posibles paneles de error
            that.hideInfoPanels();
            // Mostrar panel info con su clase
            that.chkRecibirNewsletterNameInfoPanel.removeClass(that.okClass);
            that.chkRecibirNewsletterNameInfoPanel.addClass(that.errorClass);
        }).always(function (response) {
            // Mostramos el mensaje informativo
            that.chkRecibirNewsletterNameInfoPanel.html(response);
            that.chkRecibirNewsletterNameInfoPanel.fadeIn();
            OcultarUpdateProgress();
        });
    },
};

/**
 * Clase jquery para poder la solicitud de restablecimiento de la contraseña por parte del usuario.
 * Para acceder a esta vista, se accederá desde un link en "Login" de "Olvido de contraseña"
 *
 * */
const operativaOlvidoPassword = {
    /**
     * Acción para inicializar elementos y eventos
     */
    init: function (pParams) {
        this.config(pParams);
        this.configEvents();
    },
    /*
     * Opciones de configuración de la vista
     * */
    config: function (pParams) {
        // Inicialización de las vistas
        this.txtUserLogin = $(`#${pParams.idTxtuserLogin}`);
        this.forgetPasswordInfoPanel = $(`#${pParams.idForgetPasswordInfoPanel}`);
        this.btnEnviar = $(`#${pParams.idBtnEnviar}`);

        // Paneles de error/info
        this.panelesInfo = [this.forgetPasswordInfoPanel];

        // Url necesarias para realizar petición
        this.urlForgetPasswordRequest = pParams.urlForgetPasswordRequest;

        // Mensajes preconfigurados de error
        this.msgInfoEmptyField = pParams.msgInfoEmptyField;
        this.msgErrorForgetPasswordRequest = pParams.msgErrorForgetPasswordRequest;
    },

    /**
    * Ocultar los paneles informativos (errores o info)
    **/
    hideInfoPanels() {
        this.panelesInfo.forEach(panel => panel.hide());
    },

    /**
    * Configuración de los eventos
    */
    configEvents: function () {
        const that = this;

        // Botón de Aceptar - Solicitar link para cambio de contraseña
        this.btnEnviar.click(function (event) {
            // Ocultar por defecto posibles mensajes de error
            that.hideInfoPanels();
            // Comprobar que el input no es vacío
            if (that.validarCampos()) {
                // Realizar la petición de cambio de contraseña
                that.cambiarPasswordSubmit();
            } else {
                that.forgetPasswordInfoPanel.html(that.msgInfoEmptyField);
                that.forgetPasswordInfoPanel.fadeIn();
            }
        });
    },

    /**
    * Comprobar que los campos indicados no están vacíos (email indicado por el usuario)
    * @returns {bool}    
    */
    validarCampos: function () {
        return (this.txtUserLogin.val() != '');
    },

    /**
    * Acción de petición de solicitar cambio de contraseña por olvido
    * Se disparará al pulsar el botón y al validar que el campo del email no está vacío
    */
    cambiarPasswordSubmit: function () {
        const that = this;
        // Construcción del objeto dataPost
        var dataPost = {
            User: this.txtUserLogin.val()
        }

        MostrarUpdateProgress();
        GnossPeticionAjax(this.urlForgetPasswordRequest, dataPost, true).fail(function () {
            // Mostrar mensaje de error con la información traida del backend
            that.forgetPasswordInfoPanel.html(that.msgErrorForgetPasswordRequest);
            that.forgetPasswordInfoPanel.fadeIn("slow");
            OcultarUpdateProgress();
        });
    }
};

/**
 * Inicializa los datepickers en la página.
 */
function initializeDatePickers() {
    const currentYear = new Date().getFullYear();
    const oldYear = currentYear - 100;
    const maxYear = currentYear + 100;

    // DatePickers normales
    $('.datepicker').datepicker({
        changeMonth: true,
        changeYear: true,
        yearRange: `${oldYear}:${maxYear}`,
    });

    // DatePickers con límite en Fecha
    $('.datepicker-minToday').datepicker({
        changeMonth: true,
        changeYear: true,
        // Fecha mínima será hoy
        minDate: '0',
    });
}

/**
 * Previene la actualización de la página cuando hay formularios importantes sin guardar.
 */
function preventLeavingFormWithoutSaving() {
    const form = $("#preventLeavingFormWithoutSaving");
    if (form.dirty != null) {
        form.dirty({ preventLeaving: true });
    }
}

/**
 * Configura el comportamiento del modal-container antes de que se muestre.
 */
function setupModalBehavior() {
    $('#modal-container').on('shown.bs.modal', function(e) {
        // Comprobar si hay que mostrar el contenedor centrado en pantalla o no
        const elementThatTriggeredModal = $(e.relatedTarget);
        if (elementThatTriggeredModal.data("showmodalcentered")) {
            // Eliminar la clase por defecto de mostrarse en el top
            $(this).removeClass("modal-top");
        }
    });
}

/**
 * Inicializa el comportamiento de navegación del botón back del navegador.
 */
function initializeBackButtonNavigation() {
    if (typeof operativaDetectarNavegacionBackButton !== 'undefined' && operativaDetectarNavegacionBackButton.init) {
        operativaDetectarNavegacionBackButton.init(); 
    }
}

/**
 * Refresca los waypoints en la página.
 */
function refreshWaypoints() {
    if (typeof Waypoint !== 'undefined') {
        setInterval(function() {
            Waypoint.refreshAll();            
        }, 1000);
    }
}

/**
 * Inicializa el comportamiento de zoom en imágenes.
 */
function initializeImageZooming() {
    if (typeof operativaImagesZooming !== 'undefined' && operativaImagesZooming.init) {
        operativaImagesZooming.init();
    }
}

/*MVC.FichaDocumento.js*/ 
// Desplegar y mostrar la vista o contenido devuelto de una petición vía urlAccion para ser mostrado en el panel pPanelID
/**
 * 
 * @param urlAccion: URL de la petición que será pasada al controller para que este devuelva datos (Puede ser una vista y datos que se controlarán en el modelo de la página)
 * @param {any} pBoton: Botón que ha desplegado la acción.
 * @param {any} pPanelID: ID del panel donde se devolverá ese código HTML devuelto por la petición mandada en el parámetro urlAccion
 * @param {any} pArg: Argumentos adicionales
 */
function DeployActionInModalPanel(urlAccion, pBoton, pPanelID, pArg) {
    // Panel principal (padre) donde se mostrarán todos los paneles
    var panel = $('#' + pPanelID);
    // Panel donde se mostrará el contenido
    var panelContent = panel.children('#content');
    // Panel de mensajes de OK/KO del contenedor padre (Posible error en la carga del servidor)
    var panelMessagesResult = panel.children().children('#resource-message-results');
    // Ocultado mensaje de errores por defecto
    panelMessagesResult.css("display", "none");
    // Comprobar posibles parámetros
    let params = '';
    if (pArg != undefined || pArg != '') {
        params = pArg;
    } else {
        params = null;
    }

    // Realizar la petición AJAX
    GnossPeticionAjax(urlAccion, params, true).done(function (data) {
        panelContent.html(data);
        // Ocultar panel de mensajes mensajes
        panelMessagesResult.css("display", "none");
        panelContent.css("display", "block");
        // Llamar a inicializar las DataTable dentro del modal para acción "Historico" en Recurso
        accionHistorial.montarTabla();
        // Llamar a inicializar los despliegues para acción "Categorizar" en Recurso        
        accionDesplegarCategorias.init();
    }).fail(function (data) {
        if (data == 'invitado') {
            operativaLoginEmergente.init();
            OcultarUpdateProgress();
            CerrarPanelAccion(pPanelID);
        } else {
            // Mostrar mensaje de error
            panelMessagesResult.css("display", "block");
            panelContent.css("display", "none");
            panelMessagesResult.children(".ok").css("display", "none");
            panelMessagesResult.children(".ok").css("display", "block");
            panelMessagesResult.children(".ok").html('error');
        }
    });
}

/**
 * Función para descargar un fichero desde SharePoint.
 * Esta función obtiene el ID del recurso desde un elemento con el ID 'recurso_id',
 * limpia la URL actual de ciertos parámetros y fragmentos específicos, y construye
 * una nueva URL para solicitar la descarga del fichero desde SharePoint. Finalmente,
 * abre una nueva ventana o pestaña del navegador para iniciar la descarga.
 */
function DescargarFicheroSharepoint() {
    let recurso_id = $("#recurso_id").val();
    let recurso_version_id = $("#recurso_version_id").val();
    let resource_id = recurso_id == recurso_version_id ? recurso_version_id : recurso_version_id;
    let urlSinVersioned = window.location.href.replace('?versioned', '');
    let urlSinCreated = urlSinVersioned.replace('?created', '');
    let urlSinModified = urlSinCreated.replace('?modified', '');
    urlSinModified = urlSinModified.replace('#', '');
    let peticion = urlSinModified + "/download-sharepoint?RecursoID=" + resource_id;

    window.open(peticion, '_blank');
}

function RealizarTransicion(urlRealizarTransicion, transicionID) {
    MostrarUpdateProgress();

    var dataPost = {
        pTransicionID: transicionID
    }

    GnossPeticionAjax(urlRealizarTransicion, dataPost, true).done(function (data) {
        mostrarNotificacion("success", data);
        setTimeout(function () {
            location.reload();
        }, 1000);
    }).fail(function (data) {
        mostrarNotificacion("error", data);
    }).always(function () {        
        OcultarUpdateProgress();
    });
}

/**
 * Función para crear una versión rápida de un recurso en SharePoint.
 * 
 * Esta función obtiene el ID del recurso desde un elemento con el ID 'recurso_id',
 * limpia la URL actual de ciertos parámetros y fragmentos específicos, y construye
 * una nueva URL para solicitar la creación de una versión rápida del recurso.
 * Realiza una petición AJAX a la URL construida, muestra un progreso de actualización
 * y maneja las respuestas de éxito o error.
 */
function CrearVersionRapidaEnlaceSharepoint() {
    let recurso_id = $("#recurso_id").val();
    let urlSinVersioned = window.location.href.replace('?versioned', '');
    let urlSinCreated = urlSinVersioned.replace('?created', '');
    let urlSinModified = urlSinCreated.replace('?modified', '');

    let url = urlSinModified.replace("/recurso/", "/version-rapida-recurso/");
    url = url.replace('#', '');
    url += "?pDocumentoID=" + recurso_id;
    MostrarUpdateProgress();
    GnossPeticionAjax(
        url,
        '',
        true,
        false
    ).done(function (data) {
        // Correcto
        OcultarUpdateProgress();
        mostrarNotificacion("success", "La versión se ha creado correctamente.");
        window.location.href = data;
    }).fail(function (data) {
        OcultarUpdateProgress();
        // Error
        if (data.includes("http") && data.length > 0) {
            window.location.href = data;
        } else {
            mostrarNotificacion("error", "Se ha producido un error al tratar de crear una versión.");
        }
    });
}

/**
 * Función para desvincular un recurso de SharePoint.
 * 
 * Esta función obtiene el ID del recurso desde un elemento con el ID 'recurso_id',
 * limpia la URL actual de ciertos parámetros y fragmentos específicos, y construye
 * una nueva URL para solicitar la desvinculación del recurso de SharePoint.
 * Realiza una petición AJAX a la URL construida y maneja las respuestas de éxito o error.
 */
function DesvincularSharepoint() {
    let recurso_id = $("#recurso_id").val();
    let recurso_version_id = $("#recurso_version_id").val();
    let resource_id = recurso_id == recurso_version_id ? recurso_version_id : recurso_version_id;
    let urlSinVersioned = window.location.href.replace('?versioned', '');
    let urlSinCreated = urlSinVersioned.replace('?created', '');
    let urlSinModified = urlSinCreated.replace('?modified', '');
    let txtRecurso = $("#txtRecurso").val();
    let txtMetodo = $("#desvincularSharepointMetodo").val();

    let url = urlSinModified.replace(`/${txtRecurso}/`, `/${txtMetodo}/`);
    url = url.replace('#', '');
    url += "?pDocumentoID=" + resource_id;
    GnossPeticionAjax(
        url,
        '',
        true,
        false
    ).done(function (data) {
        mostrarNotificacion("success", "El recurso ha sido desvinculado correctamente.");
        setTimeout(function () {
            location.reload();
        }, 1000);
    }).fail(function (data) {
        mostrarNotificacion("error", "Se ha producido un error al tratar de desvincular el recurso.");
    });

}


/**
 * Objeto para gestionar el reseteo del contenido del modal.
 * 
 * Este objeto contiene métodos para inicializar el comportamiento del modal al ser cerrado,
 * y para vaciar el contenido del modal y mostrar un estado de "cargando".
 */
var resetModalContainer = {
    // Inicializar el comportamiento cuando la página web está cargada
    init: function () {
        $("#modal-container").on("hidden.bs.modal", function () {
            // Añadir la clase por defecto para que se muestre en el top de la página
            $(this).addClass("modal-top");
            // Panel que hay que resetear/rellenar con el initialContainerContent
            var panelToReset = $(this).find("#content");
            // HTML que cargaremos de nuevo una vez se cierre el formulario (resetearlo de inicio)
            var initialContainerContent = '';
            initialContainerContent += '<div class="modal-header">';
            initialContainerContent += '<p class="modal-title">';
            initialContainerContent += '<span class="spinner-border white-color mr-2"></span>';
            initialContainerContent += 'Cargando ...';
            initialContainerContent += '</p>';
            initialContainerContent += '<span class="material-icons cerrar" data-dismiss="modal" aria-label="Close">close</span>';
            initialContainerContent += '</div>';
            initialContainerContent += '<div class="modal-body"></div>';
            initialContainerContent += '<div id="resource-message-results" class="modal-footer">';
            initialContainerContent += '<div class="ok"></div>';
            initialContainerContent += '<div class="ko"></div>';
            initialContainerContent += '</div>';
            // Vuelvo a incluir el panel inicial para ser reutilizado (ocultar el contenido previo si se abrió antes el modal)
            panelToReset.html(initialContainerContent).fadeIn();
        });
        return;
    },


    // Vaciar el contenedor de un modal y dejarlo como "loading" hasta que este vuelva a llenarse con datos vía API REST (Ficha Recurso: Eliminar - Eliminar Selectivo)
    // Sirve para volver a llenar un modal sin que este sea cerrado.
    resetModalContent: function () {
        // Contenedor padre de los modales
        var $modalContainer = $("#modal-container");
        // Panel donde se vaciará el contenido actual para emular la carga (Loading)
        var panelToReset = $modalContainer.find("#content");
        // HTML que cargaremos de nuevo una vez se cierre el formulario (resetearlo de inicio)
        var initialContainerContent = '';
        initialContainerContent += '<div class="modal-header">';
        initialContainerContent += '<p class="modal-title">';
        initialContainerContent += '<span class="spinner-border white-color mr-2"></span>';
        initialContainerContent += 'Cargando ...';
        initialContainerContent += '</p>';
        initialContainerContent += '<span class="material-icons cerrar" data-dismiss="modal" aria-label="Close">close</span>';
        initialContainerContent += '</div>';
        initialContainerContent += '<div class="modal-body"></div>';
        initialContainerContent += '<div id="resource-message-results" class="modal-footer">';
        initialContainerContent += '<div class="ok"></div>';
        initialContainerContent += '<div class="ko"></div>';
        initialContainerContent += '</div>';
        // Vuelvo a incluir el panel inicial para ser reutilizado (ocultar el contenido previo si se abrió antes el modal)
        panelToReset.html(initialContainerContent);
    },
};


/**
 * Realiza una petición AJAX para obtener y mostrar contenido en un panel específico.
 *
 * @param {string} urlAccion - La URL de la acción que se desea ejecutar.
 * @param {HTMLElement} pBoton - El botón que disparó la acción (puede ser null).
 * @param {string} pPanelID - El ID del panel donde se mostrará el contenido.
 * @param {object} pArg - Argumentos adicionales (opcional).
 */
function DesplegarAccionMVC(urlAccion, pBoton, pPanelID, pArg) {
    var panel = $('#' + pPanelID);

    panel.children().children('#loading').css("display", "block");
    panel.children().children('#menssages').css("display", "none");
    panel.children().children('#action').css("display", "none");

    panel.children().css("display", "block");
    panel.css("display", "block");

    if (pBoton != null && typeof (pBoton.parentNode) != 'undefined') {
        //DesactivarBotonesActivosDespl();
        pBoton.parentNode.className += ' active';
    }

    GnossPeticionAjax(urlAccion, null, true).done(function (data) {
        panel.children().children('#action').html(data);
        panel.children().children('#loading').css("display", "none");
        panel.children().children('#menssages').css("display", "none");
        panel.children().children('#action').css("display", "block");
    }).fail(function (data) {
        if (data == 'invitado') {
            operativaLoginEmergente.init();
            OcultarUpdateProgress();
            CerrarPanelAccion(pPanelID);
        } else {
            panel.children().children('#loading').css("display", "none");
            panel.children().children('#menssages').css("display", "block");
            panel.children().children('#action').css("display", "none");

            panel.children().children('#menssages').children(".ok").css("display", "none");
            panel.children().children('#menssages').children(".ko").css("display", "block");
            panel.children().children('#menssages').children(".ko").html('ERROR');
        }
    });
}

/**
 * Acción para desplegar mensaje informativo una vez realizada una acción
 * @param {any} pPanelID: Panel id donde se ha de mostrar el mensaje informativo (antes de toastr)
 * @param {any} pOk: Valor booleano que indica si la acción ha sido OK o no.
 * @param {any} pHtml: Mensaje informativo que se mostrará al usuario.
 * @param {any} timeOut: Tiempo de espera hasta que se muestra la notificación correcta. (milisegundos)
 */
function DesplegarResultadoAccionMVC(pPanelID, pOk, pHtml, timeOut = 1500) {
    var panel = $('#' + pPanelID);

    if (pOk) {
        // Hacer desaparecer el modal si la acción es correcta
        $('#modal-container').modal('hide');
        // Mostrar mensaje OK
        setTimeout(() => {
            mostrarNotificacion('success', pHtml);
        }, timeOut)
    }
    else {        
        mostrarNotificacion('error', pHtml);
    }
}

/**
 * Acción de realizar voto positivo de un recurso. Realización de una forma más visual.
 * - Aparecerá un pequeño "Loading" durante la acción del voto
 * - Actualizará el num de votos cuando finalice la acción
 * - Estará disponible la acción inversa cuando finalice el voto realizado.
 * @param {any} that: El botón pulsado (Span) con el icono de thumbs_up_alt
 * @param {any} urlVotarRecurso: La URL para realizar el voto
 * @param {any} urlVotarRecursoInvertido: La URL para realizar el voto contrario/invertido
 */
function AccionRecurso_VotarPositivo(that, urlVotarRecurso, urlVotarRecursoInvertido) {
    $(that).parent().find('.eleccionUsuario').removeClass("eleccionUsuario");
    $(that).addClass("eleccionUsuario");

    // Parámetros
    var funcionVotarInvertido = "AccionRecurso_VotarEliminar(this, '" + urlVotarRecursoInvertido + "', '" + urlVotarRecurso + "')";
    var iconoVoto = "thumb_up_alt";
    var iconoVotoInvertido = iconoVoto;
    var nombreClaseVotoOK = "activo"
    var claseMaterialIcons = "material-icons";
    var $numVotos = $(that).parent().find(".number");
    var numVotosActual = parseInt($numVotos.text());

    // Clase de tipo "Loading"
    var loadingClass = "spinner-border spinner-border-sm texto-primario"

    // Ocultar icono del voto actual para mostrar solo el "Loading"
    $(that).html("");
    // Elimino la clase de material icons para poder cambiar el color
    $(that).removeClass(claseMaterialIcons);
    // Muestrar loading hasta que se complete la petición de "Votar"
    $(that).addClass(loadingClass);
    if (urlVotarRecurso != "") {

        //Evento matomo 
        var matomoConfigurado = $('#inpt_matomoConfigurado').val();

        if (matomoConfigurado != undefined && matomoConfigurado == 'True') {
            var recursoUrl = urlVotarRecurso.replace("/vote-positive", "");
            var matomoConfigurado = $('#inpt_matomoConfigurado').val();
            _paq.push(['setCustomDimension', 4, recursoUrl]);

            _paq.push(['trackEvent', 'Evento recurso', 'Voto', 'Me gusta']);
        }

        GnossPeticionAjax(urlVotarRecurso, null, true).done(function (data) {            
            // EnviarAccGogAnac('Acciones sociales', 'Votar', urlVotarRecurso);
            if (typeof voteAnalitics != 'undefined') {
                voteAnalitics.voteResource(1);
            }
            // Cambiar icono a Voto negativo        
            $(that).html(iconoVotoInvertido);
            // Cambiar la función a realizar a Voto negativo
            $(that).attr("onclick", funcionVotarInvertido);
            // Añado de nuevo la clase de material icons
            $(that).addClass(claseMaterialIcons);
            // Añado la clase de megusta directamente al padre
            $(that).parent().toggleClass(nombreClaseVotoOK);
            $(that).removeClass(loadingClass);
            // Cambiar el número del voto realizado a +1
            numVotosActual += 1;
            $numVotos.html(numVotosActual);
        }).fail(function (data) {
            // Añado de nuevo la clase de material icons
            $(that).addClass(claseMaterialIcons);
            // Cambiar el loading y volver a como estaba antes (original) del error        
            $(that).html(iconoVoto);
            $(that).removeClass(loadingClass);
            if (data == "invitado") { operativaLoginEmergente.init(); }
        });
    } else {
        // Añado de nuevo la clase de material icons
        $(that).addClass(claseMaterialIcons);
        // Cambiar el loading y volver a como estaba antes (original) del error        
        $(that).html(iconoVoto);
        $(that).removeClass(loadingClass);
        operativaLoginEmergente.init();
    }
}

/**
 * Acción de realizar voto negativo de un recurso. Realizarán de una forma más visual.
 * - Aparecerá un pequño "Loading" durante la acción del voto
 * - Actualizará el num de votos cuando finalice la acción
 * - Estará disponible la acción inversa cuando finalice el voto realizado.
 * @param {any} that: El botón pulsado (Span) con el icono de thumbs_up_alt
 * @param {any} urlVotarRecurso: La URL para realizar el voto
 * @param {any} urlVotarRecursoInvertido: La URL para realizar el voto contrario/invertido
 */

function AccionRecurso_VotarEliminar(that, urlVotarRecurso, urlVotarRecursoInvertido) {
    $(that).parent().find('.eleccionUsuario').removeClass("eleccionUsuario");

    // Parámetros
    var funcionVotarInvertido = "AccionRecurso_VotarPositivo(this, '" + urlVotarRecursoInvertido + "', '" + urlVotarRecurso + "')";
    var iconoVoto = "thumb_up_alt";
    //var iconoVotoInvertido = "thumb_up_alt";
    // Será el mismo icono pero cambiado el color
    var iconoVotoInvertido = iconoVoto;
    var nombreClaseVotoOK = "activo"
    var claseMaterialIcons = "material-icons";
    var $numVotos = $(that).parent().find(".number");
    var numVotosActual = parseInt($numVotos.text());

    // Clase de tipo "Loading"
    var loadingClass = "spinner-border spinner-border-sm texto-primario"

    // Ocultar icono del voto actual para mostrar solo el "Loading"
    $(that).html("");
    // Elimino la clase de material icons para poder cambiar el color
    $(that).removeClass(claseMaterialIcons);
    // Muestrar loading hasta que se complete la petición de "Votar"
    $(that).addClass(loadingClass);


    if (urlVotarRecurso != "") {
        GnossPeticionAjax(urlVotarRecurso, null, true).done(function (data) {            
            if (typeof voteAnalitics != 'undefined') {
                voteAnalitics.voteResource(0);
            }
            // Cambiar icono a Voto negativo        
            $(that).html(iconoVotoInvertido);
            // Cambiar la función a realizar a Voto negativo
            $(that).attr("onclick", funcionVotarInvertido);
            // Añado de nuevo la clase de material icons
            $(that).addClass(claseMaterialIcons);
            $(that).removeClass(loadingClass);
            // Añadimos la clase para el color a voto realizado (Like)
            $(that).parent().toggleClass(nombreClaseVotoOK);
            // Cambiar el número del voto realizado a +1
            numVotosActual -= 1;
            $numVotos.html(numVotosActual);
        }).fail(function (data) {
            // Añado de nuevo la clase de material icons
            $(that).addClass(claseMaterialIcons);
            // Cambiar el loading y volver a como estaba antes (original) del error        
            $(that).html(iconoVoto);
            $(that).removeClass(loadingClass);
            var error = data.substr(data.indexOf('.') + 1);
            if (error == 'Invitado') { operativaLoginEmergente.init(); }
        });
    } else {
        // Añado de nuevo la clase de material icons
        $(that).addClass(claseMaterialIcons);
        // Cambiar el loading y volver a como estaba antes (original) del error        
        $(that).html(iconoVoto);
        $(that).removeClass(loadingClass);
        operativaLoginEmergente.init();
    }
}


/**
 * Envía una solicitud AJAX para enviar un boletín informativo en un idioma específico.
 *
 * @param {string} idioma - El idioma en el que se enviará el boletín.
 * @param {string} urlEnviarNewsletter - La URL a la que se enviará la solicitud AJAX.
 * @param {string} documentoID - El ID del documento asociado con el boletín.
 */
function AccionRecurso_EnviarNewsletter_Aceptar(idioma, urlEnviarNewsletter, documentoID) {
    MostrarUpdateProgress();

    var dataPost = {
        Language: idioma
    }

    GnossPeticionAjax(urlEnviarNewsletter, dataPost, true).done(function (data) {
        DesplegarResultadoAccionMVC("despAccionRec_" + documentoID, true, data);
        OcultarUpdateProgress();
    }).fail(function (data) {
        DesplegarResultadoAccionMVC("despAccionRec_" + documentoID, false, data);
        OcultarUpdateProgress();
    });
}


/**
 * AccionRecurso_EnviarNewsletterGrupos_Aceptar
 * Envía una solicitud AJAX para enviar un boletín informativo a un conjunto de grupos en un idioma específico.
 *
 * @param {string} idioma - El idioma en el que se enviará el boletín.
 * @param {string} urlEnviarNewsletterGrupos - La URL a la que se enviará la solicitud AJAX.
 * @param {string} documentoID - El ID del documento asociado con el boletín.
 * @param {string} listaGrupos - Una cadena con la lista de grupos separados por '&'.
 */
function AccionRecurso_EnviarNewsletterGrupos_Aceptar(idioma, urlEnviarNewsletterGrupos, documentoID, listaGrupos) {
    MostrarUpdateProgress();

    if (listaGrupos.indexOf('&') == 0) {
        listaGrupos = listaGrupos.substr(1);
    }

    var dataPost = {
        Language: idioma,
        Groups: listaGrupos.split('&')
    }

    GnossPeticionAjax(urlEnviarNewsletterGrupos, dataPost, true).done(function (data) {
        DesplegarResultadoAccionMVC("despAccionRec_" + documentoID, true, data);
        OcultarUpdateProgress();
    }).fail(function (data) {
        DesplegarResultadoAccionMVC("despAccionRec_" + documentoID, false, data);
        OcultarUpdateProgress();
    });
}


/** 
 * Envía una solicitud AJAX para restaurar una versión de un documento y redirige al usuario a la URL proporcionada.
 *
 * @param {string} urlRestaurar - La URL a la que se enviará la solicitud AJAX para restaurar la versión del documento.
 * @param {string} documentoID - El ID del documento cuyo estado se está restaurando.
 */
function AccionRecurso_MostrarHistorial_Aceptar(urlRestaurar, documentoID) {
    MostrarUpdateProgress();

    GnossPeticionAjax(urlRestaurar, null, true).done(function (data) {
        window.location.href = data;
    }).fail(function (data) {
        var html = textoRecursos.restaurarFALLO;
        if (data != "") {
            html = data;
            html = html.substr(html.indexOf('|') + 1);
        }
        DesplegarResultadoAccionMVC("despAccionRec_" + documentoID, false, html);
        OcultarUpdateProgress();
    });
}


/** 
 * Envía una solicitud AJAX para vincular un recurso con una URL dada y actualiza la lista de recursos vinculados.
 *
 * @param {string} urlVincularRecurso - La URL a la que se enviará la solicitud AJAX para vincular un recurso.
 * @param {string} urlCargarVinculados - La URL para cargar la lista actualizada de recursos vinculados después de completar la acción.
 * @param {string} documentoID - El ID del documento al que se está vinculando un recurso.
 */
function AccionRecurso_Vincular_Aceptar(urlVincularRecurso, urlCargarVinculados, documentoID) {
    MostrarUpdateProgress();
    var urlDocVinculado = $("#txtUrlDocVinculado_" + documentoID).val();

    var datosPost = {
        UrlResourceLink: urlDocVinculado
    }
    GnossPeticionAjax(urlVincularRecurso, datosPost, true).done(function (data) {
        // EnviarAccGogAnac('Acciones sociales', '"Vincular recurso', urlVincularRecurso);
        DesplegarResultadoAccionMVC("despAccionRec_" + documentoID, true, textoRecursos.recursoVinculadoOK);
        OcultarUpdateProgress();
        Vinculados_CargarVinculados(urlCargarVinculados);
    }).fail(function (data) {
        DesplegarResultadoAccionMVC("despAccionRec_" + documentoID, false, data);
        OcultarUpdateProgress();
    });
}


/** 
 * Envía una solicitud AJAX para vincular un recurso a SharePoint usando un enlace proporcionado y actualiza el estado del recurso en la interfaz de usuario.
 *
 * @param {boolean} prueba - Indicador para determinar si es una prueba (no se usa en la implementación actual, pero se mantiene para compatibilidad).
 * @param {string} documentoID - El ID del documento al que se está vinculando un recurso.
 */
function AccionRecurso_VincularSharepoint(prueba, documentoID) {
    MostrarUpdateProgress();
    var enlace = $("#txtUrlDocVinculado_" + documentoID).val();
    let urlSinVersioned = location.href.replace('?versioned', '');
    let urlSinCreated = urlSinVersioned.replace('?created', '');
    let urlSinModified = urlSinCreated.replace('?modified', '');
    let url = urlSinModified.replace("/recurso/", "/link-resourceSP/");
    url = url.replace('#', '');
    //url += "?pEnlace=" + enlace + "&pDocumentoID=" + documentoID;
    var datosPost = {
        pEnlace: enlace,
        pDocumentoID: documentoID
    }
    GnossPeticionAjax(url, datosPost, true, true).done(function (data) {
        //mostrarNotificacion("success", "El recurso ha sido desvinculado correctamente.");
        DesplegarResultadoAccionMVC("despAccionRec_" + documentoID, true, "El recurso ha sido vinculado correctamente.");
        OcultarUpdateProgress();
        setTimeout(function () {
            location.reload();
        }, 1000);
    }).fail(function (data) {
        DesplegarResultadoAccionMVC("despAccionRec_" + documentoID, false, data);
        OcultarUpdateProgress();
        //mostrarNotificacion("error", "Se ha producido un error al tratar de desvincular el recurso.");
    });
}

/**
 * Mostrar el mensaje correcto de recurso desvinculado. 
 * Seguidamente, realiza la petición para cargar los nuevos recursos vinculados.
 * @param {any} urlDesvincularRecurso
 * @param {any} urlCargarVinculados
 * @param {any} documentoID
 * @param {any} documentoDesVincID
 */
function AccionRecurso_DesVincular_Aceptar(urlDesvincularRecurso, urlCargarVinculados, documentoID, documentoDesVincID) {
    MostrarUpdateProgress();

    var datosPost = {
        ResourceUnLinkKey: documentoID
    }

    GnossPeticionAjax(urlDesvincularRecurso, datosPost, true).done(function () {
        // Cambiado por nuevo Front
        //DesplegarResultadoAccionMVC("despAccionRec_" + documentoDesVincID, true, textoRecursos.DesvincularOK);
        DesplegarResultadoAccionMVC("modal-dinamic-action-response", true, textoRecursos.DesvincularOK);
        // Esperar 1,5 segundos y ocultar el panel
        setTimeout(() => {
            $('#modal-container').modal('hide');
        }, 1500);
        OcultarUpdateProgress();
        Vinculados_CargarVinculados(urlCargarVinculados);
    }).fail(function () {
        // Cambiado por nuevo Front
        //DesplegarResultadoAccionMVC("despAccionRec_" + documentoDesVincID, false, textoRecursos.DesvincularFALLO);
        DesplegarResultadoAccionMVC("modal-dinamic-action-response", false, textoRecursos.DesvincularFALLO);
        OcultarUpdateProgress();
    });
}


/**
 * Carga contenido HTML en un contenedor específico usando una solicitud AJAX y ejecuta una función de finalización una vez completada la carga.
 *
 * @param {string} urlCargarVinculados - La URL para la solicitud AJAX para cargar el contenido de los recursos vinculados.
 */
function Vinculados_CargarVinculados(urlCargarVinculados) {
    GnossPeticionAjax(urlCargarVinculados, null, true).done(function (data) {
        $('#panVinculadosInt').html(data);
        CompletadaCargaContextos();
    });
}

/**
 * Actualizar la acción para poder certificar un recurso. Debido al cambio del Front, ahora no se hace mediante "Select" sino con Radio
 * @param {any} urlPaginaCertificar: Url para realizar la acción de certificar un recurso.
 * @param {any} documentoID: Documento ID o identificador del recurso.
 * @param {any} textoCertificado: Parece ser simplemente el texto de "certificación".
 */
function AccionRecurso_Certificar_Aceptar(urlPaginaCertificar, documentoID, textoCertificado) {
    MostrarUpdateProgress();
    // Por cambio en el Front
    // var comboCertificado = $("#comboCertificado_" + documentoID);
    // var valorSeleccionado = comboCertificado.find("option:selected").text();   

    // Cogemos el atributo "data-value" que será el valor de la label elegida
    var valorSeleccionado = $('input[name=certificar-recurso]:checked').attr("data-value");
    // Cogemos id o value (no el texto en bruto) de la opción seleccionada en el radio button.
    var opcionSeleccionada = $('input[name=certificar-recurso]:checked').attr("data-option");

    var dataPost = {
        //CertificationID: comboCertificado.val()
        CertificationID: opcionSeleccionada
    }

    GnossPeticionAjax(urlPaginaCertificar, dataPost, true).done(function (data) {
        if ($('#panUtils1').length > 0) {
            $('#contCertificado').remove();
            if (comboCertificado.val() != "00000000-0000-0000-0000-000000000000") {
                $('#panUtils1').append("<div id=\"contCertificado\"><p class=\"certificado\"><span class=\"icono\"></span>" + textoCertificado + ":<strong>" + valorSeleccionado + "</strong></p></div>");
            }
        }
        DesplegarResultadoAccionMVC("despAccionRec_" + documentoID, true, textoRecursos.certificacionOK);
        OcultarUpdateProgress();
    }).fail(function (data) {
        DesplegarResultadoAccionMVC("despAccionRec_" + documentoID, false, textoRecursos.certificacionFALLO);
        OcultarUpdateProgress();
    });
}


/**
 * Envía una solicitud AJAX para eliminar un recurso y muestra el resultado de la acción en un contenedor específico.
 *
 * @param {string} urlEliminarDocumento - La URL para la solicitud AJAX para eliminar el recurso.
 * @param {number|string} documentoID - El identificador del documento que se va a eliminar.
 */
function AccionRecurso_Eliminar_Aceptar(urlEliminarDocumento, documentoID) {
    MostrarUpdateProgress();

    if (typeof resourceAnalitics != 'undefined') {
        resourceAnalitics.resourceDeleted();
    }

    GnossPeticionAjax(urlEliminarDocumento, null, true).done(function (data) {
        DesplegarResultadoAccionMVC("despAccionRec_" + documentoID, true, data);
    }).fail(function (data) {
        OcultarUpdateProgress();
        DesplegarResultadoAccionMVC("despAccionRec_" + documentoID, false, data);
    });
}

/** 
 * Envía una solicitud AJAX para eliminar selectivamente recursos basados en una lista de comunidades compartidas seleccionadas.
 *
 * @param {string} urlEliminarDocumento - La URL para la solicitud AJAX para eliminar los recursos.
 * @param {number|string} documentoid - El identificador del documento sobre el que se está realizando la operación de eliminación selectiva.
 */
function AccionRecurso_EliminarSelectivo_Aceptar(urlEliminarDocumento, documentoid) {
    MostrarUpdateProgress();

    var div = $("#eliminarSelectivo_" + documentoid);

    var i = 0;
    var lista = {};
    div.parent().find('input:checked').each(function () {
        lista[i] = this.id;
        i++;
    });

    var dataPost = {
        SharedCommunities: lista
    }

    if (typeof resourceAnalitics != 'undefined') {
        resourceAnalitics.resourceDeleted();
    }

    GnossPeticionAjax(urlEliminarDocumento, dataPost, true);
}


/**
 * Envía una solicitud AJAX para bloquear los comentarios en un recurso y actualiza el botón de interfaz de usuario en consecuencia.
 *
 * @param {HTMLElement} btnBloquear - El botón que al hacer clic bloquea los comentarios. Se actualizará para desbloquear los comentarios.
 * @param {string} textDesbloquear - El texto que se mostrará en el botón después de que se bloqueen los comentarios.
 * @param {string} urlBloquearComentarios - La URL para la solicitud AJAX que bloquea los comentarios en el recurso.
 * @param {number|string} documentoID - El identificador del documento sobre el que se están bloqueando los comentarios.
 */
function BloquearComentarios(btnBloquear, textDesbloquear, urlBloquearComentarios, documentoID) {
    MostrarUpdateProgress();

    GnossPeticionAjax(urlBloquearComentarios, null, true).done(function (data) {
        var html = textoRecursos.comentBloqOK;

        var accion = $(btnBloquear).attr('onclick');
        accion = accion.replace("BloquearComentarios(", "DesbloquearComentarios(");
        accion = accion.replace("/lock-comments", "/unlock-comments");
        accion = accion.replace(textDesbloquear, $(btnBloquear).text());
        $(btnBloquear).attr('onclick', accion);

        $(btnBloquear).html($(btnBloquear).html().replace($(btnBloquear).text(), textDesbloquear));

        DesplegarResultadoAccionMVC("despAccionRec_" + documentoID, true, html);
        // Recargar la página para no permitir comentar un recurso y viceversa
        setTimeout(() => {
            window.location.reload();
        }, 1000);

        OcultarUpdateProgress();
    }).fail(function (data) {
        DesplegarResultadoAccionMVC("despAccionRec_" + documentoID, false, data);

        OcultarUpdateProgress();
    });
}


/**
 * DesbloquearComentarios
 * Envía una solicitud AJAX para desbloquear los comentarios en un recurso y actualiza el botón de interfaz de usuario en consecuencia.
 *
 * @param {HTMLElement} btnDesbloquear - El botón que al hacer clic desbloquea los comentarios. Se actualizará para bloquear los comentarios.
 * @param {string} textBloquear - El texto que se mostrará en el botón después de que se desbloqueen los comentarios.
 * @param {string} urlDesbloquearComentarios - La URL para la solicitud AJAX que desbloquea los comentarios en el recurso.
 * @param {number|string} documentoID - El identificador del documento sobre el que se están desbloqueando los comentarios.
 */
function DesbloquearComentarios(btnDesbloquear, textBloquear, urlDesbloquearComentarios, documentoID) {
    MostrarUpdateProgress();

    GnossPeticionAjax(urlDesbloquearComentarios, null, true).done(function (data) {
        var html = textoRecursos.comentDesBloqOK;
        var accion = $(btnDesbloquear).attr('onclick');
        accion = accion.replace("DesbloquearComentarios(", "BloquearComentarios(");
        accion = accion.replace("/unlock-comments", "/lock-comments");
        accion = accion.replace(textBloquear, $(btnDesbloquear).text());
        $(btnDesbloquear).attr('onclick', accion);
        $(btnDesbloquear).html($(btnDesbloquear).html().replace($(btnDesbloquear).text(), textBloquear));
        DesplegarResultadoAccionMVC("despAccionRec_" + documentoID, true, html);
        OcultarUpdateProgress();

    }).fail(function (data) {
        DesplegarResultadoAccionMVC("despAccionRec_" + documentoID, false, data);
        OcultarUpdateProgress();
    });
}

/**
 * Envía una solicitud AJAX para crear una versión semántica de un documento y redirige a una nueva página o muestra un mensaje de resultado según la respuesta.
 * @param {string} urlPagina - La URL base para la solicitud AJAX. Se le agrega `/create-version-semantic-document` para formar la URL completa.
 * @param {number|string} documentoID - El identificador del documento para el cual se está creando una versión semántica.
 */
function CrearVersionDocSem(urlPagina, documentoID) {
    MostrarUpdateProgress();

    $.post(urlPagina + '/create-version-semantic-document', function (data) {
        var html = "";
        var ok = true;
        if (data.indexOf("OK") != 0) {
            ok = false;
            html = data;
            html = html.substr(html.indexOf('|') + 1);
            DesplegarResultadoAccionMVC("despAccionRec_" + documentoID, ok, html);

            OcultarUpdateProgress();
        }
        else {
            window.location = data.substr(data.indexOf('|') + 1);
        }
    });
}

/**
 * AccionRecurso_AgregarTags_Aceptar
 * Envía una solicitud AJAX para agregar una lista de etiquetas (tags) a un recurso y actualiza la interfaz de usuario en función del resultado.
 *
 * @param {string} urlAgregarTags - La URL para la solicitud AJAX que se utiliza para agregar las etiquetas al recurso.
 * @param {number|string} documentoID - El identificador del documento al cual se le agregarán las etiquetas.
 * @param {string} tags - Una cadena de texto con etiquetas separadas por comas que se agregarán al recurso.
 * @param {boolean} permisoEditarTags - Un valor booleano que indica si el usuario tiene permiso para editar las etiquetas del recurso.
 * @param {string} urlBaseTags - La URL base utilizada para construir enlaces a las etiquetas.
 */
function AccionRecurso_AgregarTags_Aceptar(urlAgregarTags, documentoID, tags, permisoEditarTags, urlBaseTags) {
    MostrarUpdateProgress();

    var listaTags = tags.split(',');

    var lista = {};
    for (var i = 0; i < listaTags.length; i++) {
        if (listaTags[i].trim() != "") {
            lista[i] = listaTags[i];
        }
    }

    var dataPost = {
        Tags: lista
    }

    GnossPeticionAjax(urlAgregarTags, dataPost, true).done(function () {
        var html = textoRecursos.tagsOK;

        // EnviarAccGogAnac('Acciones sociales', 'Añadir etiquetas', urlAgregarTags);

        var htmlTags = "";
        //var separador = ", ";
        if (permisoEditarTags) {
            separador = "";
        }
        $.each(tags.split(','), function () {
            tag = this.trim();
            if (tag != "") {
                var enlaceCompleto = urlBaseTags + "/" + tag;
                htmlTags += "<li><a href=\"" + enlaceCompleto + "\" rel=\"sioc:topic\" resource=\"" + enlaceCompleto + "\"><span typeof=\"sioc_t:Tag\" property=\"dcterms:name\" about=\"" + enlaceCompleto + "\">" + tag + "</span></a></li>";
                //separador = ", ";
            }
        });

        // Cambio por nuevo Front
        //var ul = $('#despAccionRec_' + documentoID).parents('.wrapDescription').children('.group.etiquetas').find('ul');
        const ul = $('.group.etiquetas').find('.listTags');
        var ultimoLI = ul.children().last();

        if (ultimoLI.find('a').hasClass("verTodasCategoriasEtiquetas")) {
            if (permisoEditarTags) {
                ul.children().each(function () {
                    if (!$(this).find('a').hasClass("verTodasCategoriasEtiquetas")) {
                        $(this).remove();
                    }
                });
            }
            ultimoLI.before(htmlTags);

            if (ultimoLI.find('a').hasClass('mas')) {
                ultimoLI.find('a').click();
            }
        }
        else {
            if (permisoEditarTags) {
                ul.html(htmlTags);
            }
            else {
                ultimoLI.after(htmlTags);
            }
        }

        DesplegarResultadoAccionMVC("despAccionRec_" + documentoID, true, html);

        OcultarUpdateProgress();
    }).fail(function (data) {
        var html = textoRecursos.tagsFALLO;
        var numError = data;
        if (numError == "0") {
            html = form.errordtag;
        }

        DesplegarResultadoAccionMVC("despAccionRec_" + documentoID, false, html);

        OcultarUpdateProgress();
    });
}



/**
 * AccionRecurso_AgregarCategorias_Aceptar
 * Envía una solicitud AJAX para agregar una lista de categorías a un recurso y actualiza la interfaz de usuario en función del resultado.
 *
 * @param {string} urlAgregarCategorias - La URL para la solicitud AJAX que se utiliza para agregar las categorías al recurso.
 * @param {number|string} documentoID - El identificador del documento al cual se le agregarán las categorías.
 * @param {string} categorias - Una cadena de texto con categorías separadas por comas que se agregarán al recurso.
 * @param {boolean} permisoEditarCategorias - Un valor booleano que indica si el usuario tiene permiso para editar las categorías del recurso.
 * @param {string} urlBaseCategorias - La URL base utilizada para construir enlaces a las categorías.
 */
function AccionRecurso_AgregarCategorias_Aceptar(urlAgregarCategorias, documentoID, categorias, permisoEditarCategorias, urlBaseCategorias) {
    MostrarUpdateProgress();

    var listaCategorias = categorias.split(',');

    var lista = {};
    for (var i = 0; i < listaCategorias.length; i++) {
        if (listaCategorias[i].trim() != "") {
            lista[i] = listaCategorias[i];
        }
    }

    var dataPost = {
        Categories: lista
    }

    GnossPeticionAjax(urlAgregarCategorias, dataPost, true).done(function () {
        var html = textoRecursos.categoriasOK;

        //EnviarAccGogAnac('Acciones sociales', 'Añadir recurso a categoría', urlAgregarCategorias);

        var htmlCategorias = "";
        var separador = "";
        /*if (permisoEditarCategorias) {
            separador = "";
        }*/
        $.each(categorias.split(','), function () {
            categoriaID = this.trim();
            if (categoriaID != "") {
                var nombreCat = $('#despAccionRec_' + documentoID).find('.divCategorias').find('.' + categoriaID).find('label').text();
                var enlaceCompleto = urlBaseCategorias + "/" + nombreCat.toLowerCase().replace(' ', '-') + "/" + categoriaID;
                htmlCategorias += "<li>" + separador + "<a href=\"" + enlaceCompleto + "\"><span>" + nombreCat + "</span></a></li>";
                //separador = ", ";
            }
        });

        const ul = $('.group.categorias').find('.listCat');
        var ultimoLI = ul.children().last();

        if (ultimoLI.find('a').hasClass("verTodasCategoriasEtiquetas")) {
            //if (permisoEditarCategorias) {
            ul.children().each(function () {
                if (!$(this).find('a').hasClass("verTodasCategoriasEtiquetas")) {
                    $(this).remove();
                }
            });
            //}
            ultimoLI.before(htmlCategorias);

            if (ultimoLI.find('a').hasClass('mas')) {
                ultimoLI.find('a').click();
            }
        }
        else {           
            ul.html(htmlCategorias);
        }

        DesplegarResultadoAccionMVC("despAccionRec_" + documentoID, true, html);

        OcultarUpdateProgress();
    }).fail(function (data) {
        var html = textoRecursos.categoriasFALLO;
        var numError = data;
        if (numError == "0") {
            html = form.errordcategoriaSelect;
        }

        DesplegarResultadoAccionMVC("despAccionRec_" + documentoID, false, html);

        OcultarUpdateProgress();
    });
}

/**
 * Método que se ejecuta desde la acción de "Compartir" para actualizar las comunidades existentes y así poder compartir el recurso donde se deseee.
 * @param {any} urlAccionCambiarCompartir: Llamada URL para proceder a realizar la acción de Compartir-Cambiar y así actualizar la lista
 * @param documentoID: El ID del documento o del recurso en cuestión
 * @param {any} personaID: El id de la persona que ha realizado la acción
 * @param {any} pListaUrlsAutocompletar
 */
function AccionRecurso_Compartir_Cambiar(urlAccionCambiarCompartir, documentoID, personaID, pListaUrlsAutocompletar) {
    MostrarUpdateProgress();
    var panelDespl = $('#despAccionRec_' + documentoID);
    var txtBaseRecursos = panelDespl.find('#ddlCompartir option:selected');

    var dataPost = {
        SelectedBR: txtBaseRecursos.val()
    }

    GnossPeticionAjax(urlAccionCambiarCompartir, dataPost, true).done(function (data) {
        var html = "";

        // Comprobar si es un objeto o array (La versión 5 devuelve un objeto de arrays)             
        if (!Array.isArray(data)) {
            // V5 -> Objeto de arrays -> Acceder al elemento 2 del objeto
            for (var i in data.$values) {
                html += data.$values[i].html;
            }
        } else {
            // V4 -> Array solo ha sido devuelto
            for (var i in data) {
                html += data[i].html;
            }
        }

        // Actualizarlo por el nuevo Front
        // panelDespl.find('#divSelCatTesauro').html(html);
        //panelDespl.find('.divCategorias').html(html);        
        panelDespl.find('#panCategoriasTesauroArbol').html(html);
        // Llamar a inicializar los despliegues para acción "Compartir" en Recurso        
        accionDesplegarCategorias.init()

        panelDespl.find('#txtSeleccionados').val('');

        var liBaseRecursos = panelDespl.find('#panContenedorBRAgregadas').find('ul.icoEliminar #' + txtBaseRecursos.val());
        if (liBaseRecursos.length > 0) {
            var catSeleccionadas = liBaseRecursos.find('input').val().split('$$')[1]
            panelDespl.find('#txtSeleccionados').val(catSeleccionadas);
            $.each(catSeleccionadas.split(','), function () {
                if (this != "") {
                    panelDespl.find('#divSelCatTesauro').find('span.' + this + ' input').prop('checked', true);
                    var imgVerMas = panelDespl.find('#divSelCatTesauro').find('span.' + this + ' input').parents('.panHijos').parent().children('img');
                    $.each(imgVerMas, function () {
                        if (this.src.indexOf('verMas') > 0) {
                            MVCDesplegarTreeView(this);
                        }
                    });
                }
            });
        }

        if (panelDespl.find('#panEditoresLectores').length > 0) {
            if (txtBaseRecursos.attr("rel") == "private") {
                panelDespl.find('#panEditoresLectores').find('#panLectores').show();
                panelDespl.find('#panEditoresLectores').find('#panEditores').show();
            }
            else if (txtBaseRecursos.attr("rel") == "public") {
                panelDespl.find('#panEditoresLectores').find('#panLectores').hide();
                panelDespl.find('#panEditoresLectores').find('#panEditores').show();
            }
            else if (txtBaseRecursos.attr("rel") == "org") {
                panelDespl.find('#panEditoresLectores').find('#panLectores').hide();
                panelDespl.find('#panEditoresLectores').find('#panEditores').hide();
            }
        }

        AccionRecurso_Compartir_GenerarAutocompletar(documentoID, personaID, pListaUrlsAutocompletar);

        OcultarUpdateProgress();
    });
}


/**
 * Envía una solicitud AJAX para agregar un recurso a una lista de categorías en el espacio personal del usuario y actualiza la interfaz de usuario en función del resultado.
 *
 * @param {string} urlAceptarGuardarEn - La URL para la solicitud AJAX que se utiliza para agregar el recurso a las categorías seleccionadas.
 * @param {string} urlCargarCompartidos - La URL para la solicitud AJAX que se utiliza para recargar la lista de recursos compartidos.
 * @param {number|string} documentoID - El identificador del documento que se agregará a las categorías seleccionadas.
 */
function AccionRecurso_AgregarABR_Aceptar(urlAceptarGuardarEn, urlCargarCompartidos, documentoID) {
    MostrarUpdateProgress();
    var panelDespl = $('#despAccionRec_' + documentoID);
    var categorias = panelDespl.find('#txtSeleccionados').val().split(',');

    var lista = {};
    for (var i = 0; i < categorias.length; i++) {
        if (categorias[i].trim() != "") {
            lista[i] = categorias[i];
        }
    }

    var dataPost = {
        categoriesList: lista
    }

    GnossPeticionAjax(urlAceptarGuardarEn, dataPost, true).done(function () {
        $('#resource_' + documentoID + ' .acc_guardar').remove();
        $('#divGroupAccionesRec .opAddPersonal').remove();

        DesplegarResultadoAccionMVC("despAccionRec_" + documentoID, true, textoRecursos.compartirBROK);

        if (typeof shareAnalitics != 'undefined') {
            shareAnalitics.sharePersonalSpace();
        }
    }).fail(function () {
        DesplegarResultadoAccionMVC("despAccionRec_" + documentoID, false, textoRecursos.compartirBRFALLO);

    }).always(function () {
        if ($('#divCompartido').length > 0) {
            var dataPost2 = {
                callback: 'Compartidos|CargarCompartidos'
            }
            $.post(urlCargarCompartidos, dataPost2, function (data) {
                $('#divCompartido').html(data);
            });
        }
        OcultarUpdateProgress();
    });
}

/** 
 * Envía una solicitud AJAX para compartir un recurso con una lista de categorías, editores y lectores, y actualiza la interfaz de usuario en función del resultado.
 *
 * @param {string} urlAceptarCompartir - La URL para la solicitud AJAX que se utiliza para compartir el recurso.
 * @param {number|string} documentoID - El identificador del documento que se compartirá.
 * @param {string} urlDocumento - La URL para recargar la lista de recursos compartidos después de la operación.
 */
function AccionRecurso_Compartir_Aceptar(urlAceptarCompartir, documentoID, urlDocumento) {
    MostrarUpdateProgress();
    var panelDespl = $('#despAccionRec_' + documentoID);
    var inputsBaseRecursos = panelDespl.find('#panContenedorBRAgregadas').find('ul.icoEliminar li input');
    var categorias = {};
    var contCategorias = 0;
    $.each(inputsBaseRecursos, function () {
        categorias[contCategorias] = $(this).val();
        contCategorias++;
    });

    var editores = {};
    var txtEditores = panelDespl.find('#panEditores').find('#divContEditores').find('#txtHackEditores_' + documentoID);
    if (txtEditores.length > 0) {
        var listaEditores = txtEditores.val().split(',');
        var contEditores = 0;
        for (var i = 0; i < listaEditores.length; i++) {
            if (listaEditores[i].trim() != "") {
                editores[contEditores] = listaEditores[i];
                contEditores++;
            }
        }
    }

    var lectores = {};
    var txtLectores = panelDespl.find('#panLectores').find('#divContLectores').find('#txtHackLectores_' + documentoID);
    if (txtLectores.length > 0) {
        var listaLectores = txtLectores.val().split(',');
        var contLectores = 0;
        for (var i = 0; i < listaLectores.length; i++) {
            if (listaLectores[i].trim() != "") {
                lectores[contLectores] = listaLectores[i];
                contLectores++;
            }
        }
    }

    var visibleSoloEditores = panelDespl.find('#panLectores').find('#rbLectoresEditores').is(':checked');

    var dataPost = {
        Categories: categorias,
        Editors: editores,
        Readers: lectores,
        Private: visibleSoloEditores
    }

    GnossPeticionAjax(urlAceptarCompartir, dataPost, true).done(function () {
        DesplegarResultadoAccionMVC("despAccionRec_" + documentoID, true, textoRecursos.compartirOK);

        if (typeof shareAnalitics != 'undefined') {
            if (typeof shareAnalitics != 'undefined') {
                shareAnalitics.shareCommunity();
            }
        }

    }).fail(function () {
        DesplegarResultadoAccionMVC("despAccionRec_" + documentoID, false, textoRecursos.compartirFALLO);
    }).always(function () {
        OcultarUpdateProgress();
        //if ($('#divCompartido').length > 0) {
        var dataPost2 = {
            callback: "Compartidos|CargarCompartidos"
        }

        GnossPeticionAjax(urlDocumento, dataPost2, true).done(function (data) {
            // Comprobar que existe el elemento -> Si no, crearlo
            if (($(".info-recurso", "#contAutoresEditoresLectores").find("#divCompartido").length) == 0) {
                const divCompartido = `<div class="group compartida" id="divCompartido"><p class="group-title">Compartida con: </p></div>`;
                $(".info-recurso", "#contAutoresEditoresLectores").append(divCompartido);
            }
            $('#divCompartido').html(data);
        });         
    });
}


/**
 * Inicializa los campos de autocompletar para los editores y lectores de un recurso.
 *
 * @param {number|string} documentoID - El identificador del documento que se compartirá.
 * @param {number|string} personaID - El identificador de la persona que está realizando la acción.
 * @param {Array} pListaUrlsAutocompletar - Una lista de URLs utilizadas para las solicitudes de autocompletar.
 */
function AccionRecurso_Compartir_GenerarAutocompletar(documentoID, personaID, pListaUrlsAutocompletar) {
    var panelDespl = $('#despAccionRec_' + documentoID);

    var panEditores = panelDespl.find('#panEditores').find('#divContEditores');
    var panLectores = panelDespl.find('#panLectores').find('#divContLectores');

    CargarAutocompletarLectoresEditoresPorBaseRecursos(panEditores.find('#txtEditores_' + documentoID), panelDespl.find('#ddlCompartir option:selected').val(), personaID, 'txtHackEditores_' + documentoID, pListaUrlsAutocompletar, 'despAccionRec_' + documentoID, 'panContenedorEditores', 'txtHackEditores_' + documentoID);

    CargarAutocompletarLectoresEditoresPorBaseRecursos(panLectores.find('#txtLectores_' + documentoID), panelDespl.find('#ddlCompartir option:selected').val(), personaID, 'txtHackLectores_' + documentoID, pListaUrlsAutocompletar, 'despAccionRec_' + documentoID, 'panContenedorLectores', 'txtHackLectores_' + documentoID);
}

/**
 * Método para agregar una categoría desde la ficha Recurso al acceder a "Compartir.
 * Cuando se hace click sobre una categoría, esta se posiciona en un panel informativo para que el usuario posteriormente, confirme la acción.
 * Se ha incluido un botón de "delete" para que quede más intuitivo y parecido al nuevo Front.
 * @param documentoID: El id del documento que será asociado a una determinada comunidad.
 */
function AccionRecurso_Compartir_Agregar(documentoID) {
    MostrarUpdateProgress();

    var panelDespl = $('#despAccionRec_' + documentoID);

    var txtBaseRecursosID = panelDespl.find('#ddlCompartir option:selected').val();
    var txtSeleccionadas = panelDespl.find('#txtSeleccionados').val();
    // Cambiar debido al nuevo Front
    //var tieneCategorias = $('#divSelCatTesauro', panelDespl).children().length > 0;    
    var tieneCategorias = $('.divCategorias', panelDespl).children().length > 0;

    if (txtBaseRecursosID != "" && (txtSeleccionadas != "" || !tieneCategorias)) {
        var html = "<li id='" + txtBaseRecursosID + "' class='d-flex align-content-start'>" +
            "   <input type=\"hidden\" value=\"" + txtBaseRecursosID + "$$" + txtSeleccionadas + "\" />" +
            "<span class=`mr-2`><strong>" + panelDespl.find('#ddlCompartir option:selected').text() + "</strong></span>";

        var numCat = 0;
        $.each(txtSeleccionadas.split(','), function () {
            if (this != "") {
                // Cambiar debido al nuevo Front
                // var nombreCat = panelDespl.find('#divSelCatTesauro').find('span.' + this + ' label').text();
                var nombreCat = panelDespl.find('.divCategorias').find('div.' + this + ' label').text();
                if (numCat > 0) {
                    html += "   <label>,</label>";
                }
                html += "   <label class='ml-2'>" + nombreCat + "</label>";

                numCat++;
            }
        });

        html += "   <a onclick=\"AccionRecurso_Compartir_Eliminar('" + txtBaseRecursosID + "', '" + documentoID + "')\" class=\"remove\"><span class=\"material-icons\" style=\"cursor:pointer\">delete</span></a>" +
            "</li>";

        panelDespl.find('#panContenedorBRAgregadas').find('ul.icoEliminar #' + txtBaseRecursosID).remove();
        panelDespl.find('#panContenedorBRAgregadas').find('ul.icoEliminar').append(html);

        panelDespl.find('#panContenedorBRAgregadas').show();
        panelDespl.find('#panBotonAceptar').show();
    }

    OcultarUpdateProgress();
}


/** 
 * Elimina una categoría de recursos seleccionada y actualiza la interfaz de usuario.
 *
 * @param {number|string} baseRecursosID - El identificador de la base de recursos que se eliminará.
 * @param {number|string} documentoID - El identificador del documento asociado a la acción.
 */
function AccionRecurso_Compartir_Eliminar(baseRecursosID, documentoID) {
    var panelDespl = $('#despAccionRec_' + documentoID);
    var txtBaseRecursosID = panelDespl.find('#ddlCompartir option:selected').val();
    if (txtBaseRecursosID == baseRecursosID) {
        panelDespl.find('#txtSeleccionados').val('');
        panelDespl.find('#divSelCatTesauro').find('input:checked').prop('checked', false);
    }
    panelDespl.find('#panContenedorBRAgregadas').find('ul.icoEliminar #' + baseRecursosID).remove();

    if (panelDespl.find('#panContenedorBRAgregadas').find('ul.icoEliminar').children().length == 0) {
        panelDespl.find('#panContenedorBRAgregadas').hide();
        panelDespl.find('#panBotonAceptar').hide();
    }
}

/**
 * Acción para descompartir un recurso de una determinada comunidad
 * @param {any} urlDescompartir: Url a la que habrá que llamar para aplicar la acción de descompartir
 * @param {any} btnDescompartir: El botón de la papelera de la comunidad de la que se desea descompartir
 * @param {any} brID: 
 * @param {any} proyID: Id del proyecto
 * @param {any} orgID: Id de la organización
 */
function AccionRecurso_Descompartir(urlDescompartir, btnDescompartir, brID, proyID, orgID) {
    // Mostrar loading
    MostrarUpdateProgress();

    const dataPost = {
        BR: brID,
        ProyectID: proyID,
        OrganizationID: orgID
    };

    GnossPeticionAjax(urlDescompartir, dataPost, true)
        .done(function () {
            const ul = $(btnDescompartir).parent().parent();
            $(btnDescompartir).parent().remove();
            if (ul.children().length == 0) {
                ul.parent().remove();
            }
        })
        .always(function () {
            OcultarUpdateProgress();
        });
}


/**
 * Maneja la acción de votar en una encuesta y actualiza la interfaz de usuario con los resultados.
 *
 * @param {string} urlPagina - La URL a la que se enviará la solicitud de votación.
 * @param {number|string} documentoID - El identificador del documento asociado a la encuesta.
 */
function AccionRecurso_Encuesta_Votar(urlPagina, documentoID) {
    MostrarUpdateProgress();
    var panelContenedor = $('#encuesta_' + documentoID);
    var opcionEncuesta = panelContenedor.find('input[type = radio]:checked');

    if (opcionEncuesta.length > 0) {
        var dataPost = {
            callback: 'Encuesta|Votar|' + opcionEncuesta.attr('id')
        }

        $.post(urlPagina, dataPost, function (data) {
            panelContenedor.html(data);
            OcultarUpdateProgress();
        });
    }
    else {
        OcultarUpdateProgress();
    }
}


function CambiarEstadoDocumento(urlCambiarEstado, documentoID, transicionID) {
    MostrarUpdateProgress();

    var comentario = encodeURIComponent($('#inptComentario_' + documentoID).val());
    var datosPost = {
        pComentario: comentario,
        pTransicionID: transicionID
    };
    GnossPeticionAjax(urlCambiarEstado, datosPost, true).done(function (data) {
        mostrarNotificacion("success", data);
        setTimeout(function () {
            window.location.reload();
        }, 1500); 
        OcultarUpdateProgress();
    }).fail(function (data) {
        mostrarNotificacion("error", data);
    }).always(function () {
        OcultarUpdateProgress();
    });
}

/** 
 * Maneja la creación de un nuevo comentario en un documento específico.
 *
 * @param {string} urlCrearComentario - La URL a la que se enviará la solicitud de creación de comentario.
 * @param {number|string} documentoID - El identificador del documento asociado al comentario.
 */
function Comentario_CrearComentario(urlCrearComentario, documentoID) {
    if ($('#txtNuevoComentario_' + documentoID).val() != '') {
        MostrarUpdateProgress();
        $('#error_' + documentoID).html('');
        var descripcion = encodeURIComponent($('#txtNuevoComentario_' + documentoID).val());
        var datosPost = {
            Description: descripcion
        };
        GnossPeticionAjax(urlCrearComentario, datosPost, true).done(function (data) {
            $('span#numComentarios').text(parseInt($('span#numComentarios').text()) + 1);
            // Vaciar el contenido del comentario realizado
            const editors = tinymce.get();
            const relatedTinyMCEId = $('#txtNuevoComentario_' + documentoID).data("editorrelated");
            // Iterar sobre las instancias para buscar la que coincide con el ID 'txtMensaje'
            editors.forEach(function (editor) {
                if (editor.id === relatedTinyMCEId) {
                    $('#txtNuevoComentario_' + documentoID).val("");
                    editor.setContent("");
                }
            });

            var html = "";
            // Comprobar si es un objeto o array (La versión 5 devuelve un objeto de arrays)             
            if (!Array.isArray(data)) {
                // V5 -> Objeto de arrays -> Acceder al elemento 2 del objeto
                for (var i in data.$values) {
                    html += data.$values[i].html;
                }
            } else {
                // V4 -> Array solo ha sido devuelto
                for (var i in data) {
                    html += data[i].html;
                }
            }

            $('#panComentarios').html(html);
            if ((typeof CompletadaCargaComentarios != 'undefined')) {
                CompletadaCargaComentarios();
            }

            if (typeof commentsAnalitics != 'undefined') {
                var comentarioID = $('#panComentarios .comment').first().attr('id');
                commentsAnalitics.commentCreated(comentarioID);
            }

            //Evento matomo 

            var matomoConfigurado = $('#inpt_matomoConfigurado').val();

            if (matomoConfigurado != undefined && matomoConfigurado == 'True') {
                _paq.push(['trackEvent', 'Evento recurso', 'Comentar', 'Comentar']);
            }

            OcultarUpdateProgress();
        });
    }
    else {
        $('#error_' + documentoID).html(comentarios.comentarioError);
    }
}


/**
 * Maneja el rechazo de una solicitud para una nueva comunidad.
 *
 * @param {string} urlRechazar - La URL a la que se enviará la solicitud de rechazo.
 * @param {number|string} peticionID - El identificador de la solicitud que se va a rechazar.
 */
function Peticion_RechazarSolicitudNuevaComunidad(urlRechazar, peticionID) {
    MostrarUpdateProgress();

    //contamos comentarios hijos para restarselos
    var panComentario = $('#' + peticionID);
    var datosPost = {
        peticion_id: peticionID
    };
    GnossPeticionAjax(urlRechazar, datosPost, true).done(function (data) {
        $('span#panNumResultados').text(parseInt($('span#panNumResultados').text()) - 1);
        $('#' + peticionID).remove();

        OcultarUpdateProgress();
    });
}


/** 
 * Maneja la aceptación de una solicitud para una nueva comunidad.
 * @param {string} urlAceptar - La URL a la que se enviará la solicitud de aceptación.
 * @param {number|string} peticionID - El identificador de la solicitud que se va a aceptar.
 */
function Peticion_AceptarSolicitudNuevaComunidad(urlRechazar, peticionID) {
    MostrarUpdateProgress();

    //contamos comentarios hijos para restarselos
    var panComentario = $('#' + peticionID);
    var datosPost = {
        peticion_id: peticionID
    };
    GnossPeticionAjax(urlRechazar, datosPost, true).done(function (data) {
        $('span#panNumResultados').text(parseInt($('span#panNumResultados').text()) - 1);
        $('#' + peticionID).remove();

        OcultarUpdateProgress();
    });
}


/**
 * Acción para rechazar a un usuario de la comunidad
 * @param {any} urlRechazar: Url de api al que realizar la petición
 * @param {any} solicitudID: ID de la solicitud
 */
function Solicitud_RechazarSolicitudUsuarioComunidad(urlRechazar, solicitudID) {
    MostrarUpdateProgress();

    //contamos comentarios hijos para restarselos
    var panComentario = $('#' + solicitudID);
    var datosPost = {
        solicitud_id: solicitudID
    };
    GnossPeticionAjax(urlRechazar, datosPost, true).done(function (data) {
        $('span#panNumResultados').text(parseInt($('span#panNumResultados').text()) - 1);
        $('#' + solicitudID).remove();

        OcultarUpdateProgress();
    });
}
/**
 * Acción para aceptar a un usuario de la comunidad
 * @param {any} urlAceptar: Url de api al que realizar la petición
 * @param {any} solicitudID: ID de la solicitud
 */
function Solicitud_AceptarSolicitudUsuarioComunidad(urlAceptar, solicitudID) {
    MostrarUpdateProgress();

    //contamos comentarios hijos para restarselos
    var panComentario = $('#' + solicitudID);
    var datosPost = {
        solicitud_id: solicitudID
    };
    GnossPeticionAjax(urlAceptar, datosPost, true).done(function (data) {
        $('span#panNumResultados').text(parseInt($('span#panNumResultados').text()) - 1);
        $('#' + solicitudID).remove();

        OcultarUpdateProgress();
    });
}
/**
 * Acción para eliminar un comentario dentro de la ficha de un recurso
 * @param {any} urlEliminar: Url de api al que realizar la petición de borrado
 * @param {any} comentarioID: ID del comentario que se desea eliminar
 */
function Comentario_EliminarComentario(urlEliminar, comentarioID) {
    MostrarUpdateProgress();

    //contamos comentarios hijos para restarselos
    var panComentario = $('#' + comentarioID);
    var numComentariosRestar = $(panComentario).find('.comentario-contenido').length;

    GnossPeticionAjax(urlEliminar, null, true).done(function (data) {
        $('span#numComentarios').text(parseInt($('span#numComentarios').text()) - numComentariosRestar);

        $('#panComentarios #' + comentarioID).remove();

        if ((typeof CompletadaCargaEliminarComentarios != 'undefined')) {
            CompletadaCargaEliminarComentarios();
        }

        if (typeof commentsAnalitics != 'undefined') {
            commentsAnalitics.commentDeleted(comentarioID);
        }

        OcultarUpdateProgress();
    });
}


/**  
 * Método que se ejecuta cuando se pulsa en "Editar" un comentario dentro de la ficha de un recurso
 * @param {any} urlResponder: Url del comentario que se editará 
 * @param {any} comentarioID: ID del comentario que será editado
 */
function Comentario_EditarComentario(urlEditar, comentarioID) {
    // Panel dinamico del modal padre donde se insertara la vista "hija"
    const $modalDinamicContentPanel = $('#modal-container').find('#modal-dinamic-content #content');

    // Contenido del comentario a editar
    const panComentario = $('#' + comentarioID);
    const panTextoComentario = $(panComentario).find('.comentario-contenido');
    const mensajeAntiguo = panTextoComentario.html();

    // Plantilla del panel html que se cargará en el modal contenedor al pulsar en la acción
    let plantillaPanelHtml = '';
    // Cabecera del panel
    plantillaPanelHtml += '<div class="modal-header">';
    plantillaPanelHtml += '<p class="modal-title"><span class="material-icons">label</span>Editar comentario</p>';
    plantillaPanelHtml += '<span class="material-icons cerrar" data-dismiss="modal" aria-label="Close">close</span>';
    plantillaPanelHtml += '</div>';
    // Cuerpo del panel
    plantillaPanelHtml += '<div class="modal-body">';
    plantillaPanelHtml += '<div class="formulario-edicion">';
    // Cuerpo de la respuesta - TextArea
    plantillaPanelHtml += '<div class="form-group">';
    plantillaPanelHtml += '<textarea class="form-control cke mensajes" id="txtComentario_Editar_' + comentarioID + '" rows="3">' + mensajeAntiguo + '</textarea>';
    plantillaPanelHtml += '</div>';

    // Contenedor de mensajes y botones
    plantillaPanelHtml += '<div id="modal-dinamic-action-response">';
    // Posibles mensajes de info
    plantillaPanelHtml += '<div>';
    plantillaPanelHtml += '<div class="menssages_' + comentarioID + '" id="menssages_' + comentarioID + '">';
    plantillaPanelHtml += '<div class="ok"></div>';
    plantillaPanelHtml += '<div class="ko"></div>';
    plantillaPanelHtml += '</div>';
    plantillaPanelHtml += '</div>';
    // Panel de botones para la acción
    plantillaPanelHtml += '<div id="modal-dinamic-action-buttons" class="form-actions">'
    plantillaPanelHtml += '<button class="btn btn-primary">Editar comentario</button>'
    plantillaPanelHtml += '</div>';
    plantillaPanelHtml += '</div>';
    plantillaPanelHtml += '</div>';

    // Meter el código de la vista modal en el contenedor padre que viene identificado por el id #modal-container
    // En concreto, hay que buscar la etiqueta modal-dinamic-content #content e insertar el código
    $modalDinamicContentPanel.html(plantillaPanelHtml);

    // Acceso a los botones
    const botones = $modalDinamicContentPanel.find('#modal-dinamic-action-buttons > button');

    // Asignación de la función al botón "Sí" o de acción
    $(botones[0]).on("click", function () {
        Comentario_EnviarEditarComentario(urlEditar, comentarioID);
    });
}


/**
 * Permite la edición de un comentario en la interfaz de usuario sin modal
 * @param {string} urlEditar - La URL a la que se enviará la solicitud de edición.
 * @param {number|string} comentarioID - El identificador del comentario que se va a editar.
 */
function Comentario_Editar_JIRA(urlEditar, comentarioID) {

    // 0 - Botón de editar, guardar, cancelar
    const $btnEditar = $(`#edit-comment-${comentarioID}`);
    const $btnEditarMobile = $(`#edit-comment-mobile-${comentarioID}`);
    const $btnCancelar = $(`#cancel-comment-${comentarioID}`);
    const $btnCancelarMobile = $(`#cancel-comment-mobile-${comentarioID}`);
    const $btnGuardar = $(`#save-comment-${comentarioID}`);
    const $btnGuardarMobile = $(`#save-comment-mobile${comentarioID}`);

    // 1 - Detectar la caja de comentarios 
    const $comentarioEditar = $(`#${comentarioID}`).find('.comentario-contenido');
    // 2- Texto del comentario que se desea editar
    const textoComentarioEditar = $comentarioEditar.html();
    // 3- Crear un input text area dinámico para el CKEditor
    let ckeEditor = `<div class="form-group">`;
    ckeEditor += `<textarea cols="20" rows="3" id="txtComentario_Editar_${comentarioID}" class="cke ckeSimple comentarios">${textoComentarioEditar}</textarea>`;
    ckeEditor += `</div>`;

    // 4- Ocultar el comentario actual 
    $comentarioEditar.fadeOut();
    // 5- Añadir el CKEditor / TextArea
    $comentarioEditar.parent().append(ckeEditor)

    // 6- Ocultar el botón de Editar (ya se está editando) y mostrar "Guardar" y "Cancelar"
    $btnEditar.parent().toggleClass('d-none');
    $btnEditarMobile.parent().toggleClass('d-none');
    $btnCancelar.parent().toggleClass('d-none');
    $btnCancelarMobile.parent().toggleClass('d-none');
    $btnGuardar.parent().toggleClass('d-none');
    $btnGuardarMobile.parent().toggleClass('d-none');

}


/**
 * Cancela la edición de un comentario en la interfaz de usuario, mostrando el comentario original sin modal
 *
 * @param {number|string} comentarioID - El identificador del comentario que se está editando.
 */
function Comentario_Cancelar_JIRA(comentarioID) {

    // 0 - Botón de editar, guardar, cancelar
    const $btnEditar = $(`#edit-comment-${comentarioID}`);
    const $btnEditarMobile = $(`#edit-comment-mobile-${comentarioID}`);
    const $btnCancelar = $(`#cancel-comment-${comentarioID}`);
    const $btnCancelarMobile = $(`#cancel-comment-mobile-${comentarioID}`);
    const $btnGuardar = $(`#save-comment-${comentarioID}`);
    const $btnGuardarMobile = $(`#save-comment-mobile${comentarioID}`);

    // 1 - Detectar la caja de comentarios que estará oculta
    const $comentarioEditar = $(`#${comentarioID}`).find('.comentario-contenido');
    // 2 - Detectar el CKEditor que estará visible
    const $ckEditor = $(`#txtComentario_Editar_${comentarioID}`);
    // 3 - Remover el CKEditor actual (el padre contenedor) ya que se ha cancelado la acción
    $ckEditor.parent().remove();
    // 4 - Volver a mostrar la caja anterior del texto    
    $comentarioEditar.fadeIn();

    // 5- Ocultar el botón de Editar (ya se está editando) y mostrar "Guardar" y "Cancelar"
    $btnEditar.parent().toggleClass('d-none');
    $btnEditarMobile.parent().toggleClass('d-none');
    $btnCancelar.parent().toggleClass('d-none');
    $btnCancelarMobile.parent().toggleClass('d-none');
    $btnGuardar.parent().toggleClass('d-none');
    $btnGuardarMobile.parent().toggleClass('d-none');

}

/**
 * Acción de Enviar al servidor el comentario editado
 * @param {any} urlEditar: URL donde hay que hacer la petición para realizar la edición del comentario
 * @param {any} comentarioID: Id del comentario que se desea editar
 */
function Comentario_EnviarEditarComentario_JIRA(urlEditar, comentarioID) {
    if ($('#txtComentario_Editar_' + comentarioID).val() != '') {
        MostrarUpdateProgress();

        var descripcion = $('#txtComentario_Editar_' + comentarioID).val();

        var datosPost = {
            Description: encodeURIComponent(descripcion)
        };

        GnossPeticionAjax(urlEditar, datosPost, true).done(function (data) {
            if (typeof commentsAnalitics != 'undefined') {
                commentsAnalitics.commentModified(comentarioID);
            }
            // Ocultar el modal al haber editado el comentario            
            const panComentario = $('#' + comentarioID);
            const panTextoComentario = $(panComentario).find('.comentario-contenido');
            panTextoComentario.html(descripcion);
            // Ocultamos ckEditor y volvemos a la posición original
            Comentario_Cancelar_JIRA(comentarioID);
        })
            .always(function () {
                OcultarUpdateProgress();
            });
    }
}

/**
 * Acción de Enviar al servidor el comentario editado
 * @param {any} urlEditar: URL donde hay que hacer la petición para realizar la edición del comentario
 * @param {any} comentarioID: Id del comentario que se desea editar
 */
function Comentario_EnviarEditarComentario(urlEditar, comentarioID) {
    if ($('#txtComentario_Editar_' + comentarioID).val() != '') {
        MostrarUpdateProgress();
        $('#error_' + comentarioID).html('');
        var descripcion = $('#txtComentario_Editar_' + comentarioID).val();

        var datosPost = {
            Description: encodeURIComponent(descripcion)
        };

        GnossPeticionAjax(urlEditar, datosPost, true).done(function (data) {
            if (typeof commentsAnalitics != 'undefined') {
                commentsAnalitics.commentModified(comentarioID);
            }
            // Ocultar el modal al haber editado el comentario
            $('#modal-container').modal('hide');
            const panComentario = $('#' + comentarioID);
            const panTextoComentario = $(panComentario).find('.comentario-contenido');
            panTextoComentario.html(descripcion);
        })
            .always(function () {
                OcultarUpdateProgress();
            });
    }
    else {
        $('#error_' + comentarioID).html(comentarios.comentarioError);
    }
}

/** 
 * Crea un modal para responder a un comentario y configura la acción del botón de enviar. Se ejecuta cuando se pulsa en "Responder" un comentario dentro de la ficha de un recurso
 * @param {string} urlResponder - La URL del endpoint donde se enviará la respuesta al comentario.
 * @param {number|string} comentarioID - El identificador del comentario al que se está respondiendo.
 */
function Comentario_ResponderComentario(urlResponder, comentarioID) {
    // Panel dinamico del modal padre donde se insertara la vista "hija"
    const $modalDinamicContentPanel = $('#modal-container').find('#modal-dinamic-content #content');

    // Plantilla del panel html que se cargará en el modal contenedor al pulsar en la acción
    let plantillaPanelHtml = '';
    // Cabecera del panel
    plantillaPanelHtml += '<div class="modal-header">';
    plantillaPanelHtml += '<p class="modal-title"><span class="material-icons">label</span>' + comentarios.responderComentario + '</p>';
    plantillaPanelHtml += '<span class="material-icons cerrar" data-dismiss="modal" aria-label="Close">close</span>';
    plantillaPanelHtml += '</div>';
    // Cuerpo del panel
    plantillaPanelHtml += '<div class="modal-body">';
    plantillaPanelHtml += '<div class="formulario-edicion">';
    // Cuerpo de la respuesta - TextArea
    plantillaPanelHtml += '<div class="form-group">';
    plantillaPanelHtml += '<textarea class="form-control cke comentarios" id="txtComentario_Responder_' + comentarioID + '" rows="3"> </textarea>';
    plantillaPanelHtml += '</div>';

    // Contenedor de mensajes y botones
    plantillaPanelHtml += '<div id="modal-dinamic-action-response">';
    // Posibles mensajes de info
    plantillaPanelHtml += '<div>';
    plantillaPanelHtml += '<div class="menssages_' + comentarioID + '" id="menssages_' + comentarioID + '">';
    plantillaPanelHtml += '<div class="ok"></div>';
    plantillaPanelHtml += '<div class="ko"></div>';
    plantillaPanelHtml += '</div>';
    plantillaPanelHtml += '</div>';
    // Panel de botones para la acción
    plantillaPanelHtml += '<div id="modal-dinamic-action-buttons" class="form-actions">'
    plantillaPanelHtml += '<button class="btn btn-primary">' + mensajes.enviar + '</button>'
    plantillaPanelHtml += '</div>';
    plantillaPanelHtml += '</div>';
    plantillaPanelHtml += '</div>';

    // Meter el código de la vista modal en el contenedor padre que viene identificado por el id #modal-container
    // En concreto, hay que buscar la etiqueta modal-dinamic-content #content e insertar el código
    $modalDinamicContentPanel.html(plantillaPanelHtml);

    // Acceso a los botones
    const botones = $modalDinamicContentPanel.find('#modal-dinamic-action-buttons > button');

    // Asignación de la función al botón "Sí" o de acción
    $(botones[0]).on("click", function () {
        Comentario_EnviarResponderComentario(urlResponder, comentarioID);
    });

}

/** 
 * Envía una respuesta a un comentario y actualiza la interfaz de usuario con la nueva respuesta.
 *
 * @param {string} urlResponder - La URL del endpoint donde se enviará la respuesta al comentario.
 * @param {number|string} comentarioID - El identificador del comentario al que se está respondiendo.
 */
function Comentario_EnviarResponderComentario(urlResponder, comentarioID) {
    if ($('#txtComentario_Responder_' + comentarioID).val() != '') {
        MostrarUpdateProgress();
        $('#error_' + comentarioID).html('');
        var descripcion = encodeURIComponent($('#txtComentario_Responder_' + comentarioID).val().replace(/\n/g, ''));
        var datosPost = {
            Description: descripcion
        };

        GnossPeticionAjax(urlResponder, datosPost, true).done(function (data) {
            // Ocultar el modal al haber enviado correctamente la respuesta
            $('#modal-container').modal('hide');
            $('span#numComentarios').text(parseInt($('span#numComentarios').text()) + 1);
            var html = "";
            // Comprobar si es un objeto o array (La versión 5 devuelve un objeto de arrays)           
            if (!Array.isArray(data)) {
                // V5 -> Objeto de arrays -> Acceder al elemento 2 del objeto
                for (var i in data.$values) {
                    html += data.$values[i].html;
                }
            } else {
                // V4 -> Array solo ha sido devuelto
                for (var i in data) {
                    html += data[i].html;
                }
            }

            $('#panComentarios').html(html);
            if ((typeof CompletadaCargaComentarios != 'undefined')) {
                CompletadaCargaComentarios();
            }

            if (typeof commentsAnalitics != 'undefined') {
                var comentarioRespuestaID = $('#panComentarios #' + comentarioID + ' .comment').first().attr('id');
                commentsAnalitics.commentCreated(comentarioRespuestaID);
            }
            OcultarUpdateProgress();
        });
    }
    else {
        $('#error_' + comentarioID).html(comentarios.comentarioError);
    }
}

/** 
 * Solicita la carga de una faceta específica desde el servidor y actualiza el contenido de un panel con la faceta obtenida.
 *
 * @param {string} pUrlFac - La URL del servicio web para cargar la faceta.
 * @param {string} pGrafo - El identificador del grafo de datos a utilizar.
 * @param {string} pFaceta - El identificador de la faceta que se desea cargar.
 * @param {object} pParametros - Parámetros adicionales para la solicitud.
 * @param {string} pIdentidad - El identificador del usuario o entidad que realiza la solicitud.
 * @param {string} pLanguageCode - El código de idioma para la solicitud.
 * @param {string} pPanelID - El ID del panel HTML donde se mostrará la faceta cargada.
 */
function MontarFacetaFichaFormSem(pUrlFac, pGrafo, pFaceta, pParametros, pIdentidad, pLanguageCode, pPanelID) {

    pIdentidad = pIdentidad.toUpperCase();
    var servicio = new WS(pUrlFac, WSDataType.jsonp);

    var metodo = 'CargarFacetas';
    var params = {};
    params['pProyectoID'] = pGrafo;
    //params['bool_esMyGnoss'] = 'False';
    params['pEstaEnProyecto'] = true;
    params['pEsUsuarioInvitado'] = (pIdentidad == 'FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF');
    params['pIdentidadID'] = pIdentidad;
    params['pUbicacionBusqueda'] = 'MetahomeCatalogo';
    params['pParametros'] = pParametros;
    params['pLanguageCode'] = pLanguageCode;
    params['pPrimeraCarga'] = false;
    params['pAdministradorVeTodasPersonas'] = false;
    params['pTipoBusqueda'] = 0;
    params['pNumeroFacetas'] = 1;
    params['pFaceta'] = pFaceta;
    params['pGrafo'] = pGrafo;
    params['pFiltroContexto'] = '';
    params['pParametros_adiccionales'] = 'factFormSem=true';
    params['pUsarMasterParaLectura'] = false;

    servicio.call(metodo, params, function (data) {
        var descripcion = data;
        $('#' + pPanelID).show();
        $('#' + pPanelID).html(descripcion);
        $('.facetedSearchBox', $('#' + pPanelID)).hide();

        if ((typeof CompletadaCargaFacetasFichaRec != 'undefined')) {
            CompletadaCargaFacetasFichaRec();
        }
    });
}

/** 
 * Gestiona la paginación de un selector de entidades en una interfaz de usuario. Actualiza el contenido del selector
 * según el enlace clicado y maneja la carga de datos adicionales si es necesario.
 *
 * @param {HTMLElement} link - El enlace que ha sido clicado para activar la paginación (debe ser un elemento con clase 'sigPagSelectEnt' para avanzar o un enlace para retroceder).
 * @param {string} urlAccion - La URL para la solicitud de datos adicionales.
 * @param {string} entidad - El nombre de la entidad que se está paginando.
 * @param {string} propiedad - La propiedad de la entidad para la cual se realiza la paginación.
 * @param {number} elemsPag - El número de elementos por página.
 * @param {number} totalElem - El número total de elementos disponibles para la paginación.
 */
function PaginarSelectorEnt(link, urlAccion, entidad, propiedad, elemsPag, totalElem) {
    var divPadre = $($(link).parent('.pagSelectEnt')[0]);

    if (divPadre.data('pag') == null) {
        divPadre.data('pag', 0);
        divPadre.data('elemsCarg', 1);
    }

    var pagActual = divPadre.data('pag');
    var elemsCarg = divPadre.data('elemsCarg');

    if (link.className == 'sigPagSelectEnt') {
        if (totalElem <= (elemsPag * (pagActual + 1))) {
            return;
        }

        pagActual++;

        if (pagActual > (elemsCarg - 1)) {
            var divCargando = $(divPadre.parent().children('.loadpagSelectEnt')[0]);
            var dataPost = { entidad: entidad, propiedad: propiedad, inicioPag: (elemsPag * pagActual) };
            GnossPeticionAjax(urlAccion, dataPost, true).done(function (data) {
                divCargando.before('<div class="pagSelEnt" style="display:none;">' + data + '</div>');
                elemsCarg++;
                divPadre.data('elemsCarg', elemsCarg);
            }).fail(function () {
                pagActual--;
            }).always(function () {
                divCargando.css('display', 'none');
                divPadre.css('display', '');
                AjustarDatosSelectorEntPaginado(pagActual, divPadre);
            });

            divCargando.css('display', '');
            divPadre.css('display', 'none');
        }
    }
    else if (pagActual > 0) {
        pagActual--;
    }

    divPadre.data('pag', pagActual);

    AjustarDatosSelectorEntPaginado(pagActual, divPadre);
}


/** 
 * Ajusta la visualización de los datos en un selector paginado de entidades en función de la página actual.
 * Muestra los datos de la página actual y oculta los datos de las demás páginas.
 *
 * @param {number} pagActual - El índice de la página actualmente activa.
 * @param {jQuery} divPadre - El contenedor que contiene los elementos paginados.
 */
function AjustarDatosSelectorEntPaginado(pagActual, divPadre) {
    var divDatos = divPadre.parent().children('.pagSelEnt');

    for (var i = 0; i < divDatos.length; i++) {
        if (i == pagActual) {
            $(divDatos[i]).css('display', '');
        }
        else {
            $(divDatos[i]).css('display', 'none');
        }
    }
}

/**
 * Inicializa un mapa de Google Maps en un elemento HTML especificado y dibuja una ruta sobre el mapa.
 * Ajusta el mapa para mostrar la ruta completa y permite especificar el color de la línea de la ruta.
 * 
 * @param {String} pDivID - ID del `div` HTML donde se mostrará el mapa.
 * @param {String} pRoute - Ruta en formato JSON con coordenadas geográficas separadas por punto y coma.
 * @param {String} pColor - Color de la línea de la ruta en formato hexadecimal. Si se deja vacío, se usa rojo.
 */
function InitializeRouteMapGoogle(pDivID, pRoute, pColor) {
    try {
        var mapOptions = {
            zoom: 3,
            center: new google.maps.LatLng(0, 0),
            mapTypeId: google.maps.MapTypeId.TERRAIN
        };

        var map = new google.maps.Map(document.getElementById(pDivID), mapOptions);

        var mapbounds = new google.maps.LatLngBounds();

        pRoute = pRoute.replace(/\;/g, ',');
        var puntosParseados = JSON && JSON.parse(pRoute) || $.parseJSON(pRoute);
        var puntosRuta = [];
        for (var i = 0; i < puntosParseados["geo:coordinates"].length; i++) {
            puntosRuta[i] = new google.maps.LatLng(puntosParseados["geo:coordinates"][i][0], puntosParseados["geo:coordinates"][i][1]);

            mapbounds.extend(puntosRuta[i]);
        }

        map.fitBounds(mapbounds);
        map.setCenter(mapbounds.getCenter());

        if (pColor == '' || pColor == null) {
            pColor = '#FF0000';
        }

        var ruta = new google.maps.Polyline({
            path: puntosRuta,
            geodesic: true,
            strokeColor: pColor,
            strokeOpacity: 1.0,
            strokeWeight: 2
        });

        ruta.setMap(map);
    }
    catch (ex) {
    }
}

/**
 * Maneja la acción del botón `btnAccionSemCms` enviando una solicitud AJAX y mostrando el resultado.
 * La función verifica si existe una función personalizada `AccionFichaRecSemCmsPersonalizado`, en cuyo caso, la llama en lugar de ejecutar el código por defecto. Luego, prepara los datos necesarios para la solicitud AJAX, la envía al servidor y maneja la respuesta mostrando mensajes de éxito o error.
 */
function AccionFichaRecSemCms(boton) {
    if (typeof (AccionFichaRecSemCmsPersonalizado) != 'undefined') {
        AccionFichaRecSemCmsPersonalizado(boton);
        return;
    }

    var that = boton;
    var idBtn = $(boton).attr('id');
    idBtn = idBtn.replace('btn_', '');
    var arg = {};
    arg.ActionID = idBtn;
    arg.ActionEntityID = $(boton).attr('rel');
    $('#errorbtn_' + idBtn).remove();

    if (arg.ActionEntityID == null || arg.ActionEntityID == '') {
        return;
    }

    MostrarUpdateProgressTime(0);

    GnossPeticionAjax(urlActionSemCms, arg, true).done(function (data) {
        $(that).after('<div id="errorbtn_' + idBtn + '" class="ok" style="display:block">' + data + '</div>');
    }).fail(function (data) {
        var mensajeError = data;
        if (data == "NETWORKERROR") {
            mensajeError = 'Has perdido la conexión. Comprueba tu conexión a internet y verifica que tus cambios se han guardado correctamente.';
        }
        $(that).after('<div id="errorbtn_' + idBtn + '" class="ko" style="display:block">' + mensajeError + '</div>');
    }).always(function () {
        OcultarUpdateProgress();
    });
}

/**/ 
/*MVC.FichaPerfil.js*/ 
/**
 * Acción de seguir a un usuario de una comunidad. Se ejecuta cuando se pulsa en el botón "Seguir"
 * @param {any} that: El botón que ha disparado la acción
 * @param {any} urlSeguirPerfil: URL a la que hay que llamar para realizar la llamda de "No seguir"
 */
function AccionPerfil_Seguir(that, urlSeguirPerfil) {

    // Icono del botón clickeado
    const buttonIcon = $(that).find("span.material-icons");
    const oldIconButton = buttonIcon.text();
    const oldTextButton = $(that).find(".texto").text();

    // Textos e iconos una vez pulsado el botón
    let newTextButton = "";
    let newIconButton = "";
    const loadingClass = "spinner-border spinner-border-sm mr-2 pr-0";


    // Acción de follow unfollow
    const followJsAction = $(that).attr('onclick');
    if (followJsAction.indexOf("unfollow") >= 0) {
        // Se desea NO seguir el perfil
        newTextButton = accionesUsuarios.seguir;
        newIconButton = "person_add_alt_1";
    } else {
        // Se desea seguir el perfil
        newTextButton = accionesUsuarios.dejarDeSeguir;
        newIconButton = "person_remove_alt_1";
    }

    // Quitar el icono para mostrar el loading
    buttonIcon.text('');
    // Mostrar el loading
    buttonIcon.addClass(loadingClass);

    GnossPeticionAjax(urlSeguirPerfil, null, true)
        .done(function (data) {
            // Quitar el Loading 
            buttonIcon.removeClass(loadingClass);
            // Añadir el nuevo método para seguir/no seguir
            if (data == "invitado") {
                operativaLoginEmergente.init();
                return;
            }
            $(that).attr('onclick', followJsAction.replace('follow', 'unfollow'));
            ChangeButtonAndText(that, newTextButton, newIconButton, null);
        })
        .fail(function (data) {
            // Dejar los iconos y textos anteriores del botón
            ChangeButtonAndText(that, oldTextButton, oldIconButton, null);

            if (data == "invitado") {
                operativaLoginEmergente.init();
                return;
            }
        })
        .always(function (data) {
            buttonIcon.removeClass(loadingClass);
        });
}

/**
 * Acción de no seguir a un usuario de una comunidad. Se ejecuta cuando se pulsa en el botón "No seguir"
 * Cambiará el nombre al botón indicando "Siguiendo"
 * Eliminará el atributo onClick para que no se vuelva a ejecutar la acción del botón
 * @param {any} that: El botón que ha disparado la acción
 * @param {any} urlNoSeguirPerfil: URL a la que hay que llamar para realizar la llamda de "No seguir"
 */
function AccionPerfil_NoSeguir(that, urlNoSeguirPerfil) {

    // Icono del botón
    var buttonIcon = $(that).find("span.material-icons");
    // Texto del botón
    var textButton = $(buttonIcon).siblings();
    // Textos e iconos una vez pulsado el botón
    var newTextButton = accionesUsuarios.sinSeguimiento;
    var newIconButton = "person_outline";

    GnossPeticionAjax(urlNoSeguirPerfil, null, true).fail(function (data) {
        if (data == 'invitado') {
            operativaLoginEmergente.init();
        }
    });
    ChangeButtonAndText(that, newTextButton, newIconButton, "btn-primary");
}

/**
 * Acción de seguir/no seguir a un usuario. Cambiará la lógica de todos los botones de seguimiento del usuario en la lista de recursos
 * @param {*} that 
 * @param {*} urlSeguir 
 * @param {string} followUser : Id del usuario (alias) del usuario para seguir/no seguir 
 */
function AccionPerfil_Seguir_Listado(that, urlSeguir, followUser) {

    // Actual item sobre el que se ha presionado para Seguir/No seguir
    const currentFollowActionItem = $(that).parent();
    // Icono actual de la acción de seguir
    const currentFollowActionIcon = currentFollowActionItem.find(".icon");
    // Texto actual de la acción de seguir
    const currentFollowActionText = currentFollowActionItem.find(".texto");

    // Cada uno de los items que aparecerán en la lista para seguir o no al usuario
    const listFollowActionItem = $(`.followAction_${followUser}`);
    // Cada uno de los iconos que aparecerán en la lista para seguir o no al usuario
    const listFollowActionIcon = listFollowActionItem.find(".icon");
    const listFollowActionText = listFollowActionItem.find(".texto");


    // Acción de follow unfollow
    const followJsAction = listFollowActionText.attr('onclick');

    // Textos e iconos una vez pulsado el botón para seguir/no seguir    
    const followText = accionesUsuarios.seguir;
    const noFollowText = accionesUsuarios.dejarDeSeguir;
    const followIcon = "person_add_alt_1";
    const noFollowIcon = "person_remove";
    const loadingClass = "spinner-border spinner-border-sm texto-primario";

    // 1 - Quitar el contenido del icono
    currentFollowActionIcon.text('');
    // 2 - Añadir la clase de loading (petición in progress)
    currentFollowActionIcon.addClass(loadingClass);

    GnossPeticionAjax(urlSeguir, null, true)
        .done(function (data) {
            // Finalizar Loading
            currentFollowActionIcon.removeClass(loadingClass);
            // Comprobar si se estaba siguiendo el perfil seleccionado
            if (currentFollowActionItem.attr('data-follow') != undefined) {
                // No se desea seguir al usuario - Se estaba siguiendo al usuario                             

                // Añadir texto de "Seguir" e icono a la lista
                listFollowActionText.text(followText);
                listFollowActionIcon.text(followIcon);
                // Quitar el atributo
                listFollowActionItem.removeAttr('data-follow');
                // Añadir la acción para seguir                
                listFollowActionText.attr('onclick', followJsAction.replace('unfollow', 'follow'));

            } else {
                // Se desea seguir al usuario - No se estaba siguiendo al usuario            
                // Añadir texto e icono de "No Seguir"
                // Cambiar la acción de Seguir/No seguir                
                listFollowActionText.text(noFollowText);
                listFollowActionIcon.text(noFollowIcon);
                listFollowActionItem.attr('data-follow', '');
                listFollowActionText.attr('onclick', followJsAction.replace('follow', 'unfollow'));

            }
        })
        .fail(function (data) {
            // Dejarlo como estaba
            if (currentFollowActionItem.attr('data-follow') != undefined || currentFollowActionItem.attr('data-follow') != false) {
                currentFollowActionText.text(noFollowText);
                currentFollowActionIcon.text(noFollowIcon);
            } else {
                currentFollowActionText.text(followText);
                currentFollowActionIcon.text(followIcon);
            }

        })
        .always(function (data) {
            currentFollowActionIcon.removeClass(loadingClass);
        });
}


/**
 * Cambiar el texto, el icono (material-icons) y eliminar el evento click de un botón
 * @param {any} button: El botón que se desea modificar
 * @param {any} newTextButton: El nuevo texto que tendrá el botón
 * @param {any} newIconButton: El nuevo icono (material-icons) que tendrá el botón
 * @param {any} classToBeDeleted: La clase que se eliminará del botón 
 */
function ChangeButtonAndText(button, newTextButton, newIconButton, classToBeDeleted) {

    // Icono del botón
    var buttonIcon = $(button).find("span.material-icons");
    var textButton = "";

    if (buttonIcon.length == 0) {
        // No hay botón --> Buscar un span
        buttonIcon = $(button).siblings();
        textButton = $(button);

    } else {
        textButton = $(buttonIcon).siblings();
    }

    // Cambiar nombre al botón 
    textButton.text(newTextButton);
    // Cambiar el icono al botón
    buttonIcon.text(newIconButton);
    // Añadir cursor: normal
    if (classToBeDeleted != undefined) {
        $(button).css("cursor", "auto");
        // Quitar el estilo de botón (no clickeable)
        $(button).removeClass(classToBeDeleted);
    }
}


/**
 * Acción que se ejecuta cuando se pulsa sobre las acciones disponibles de un perfil para mandar un "Email". 
 * @param {string} titulo: Título que tendrá el panel modal 
 * @param {string} texto: El texto o mensaje a modo de título que se mostrará para que el usuario sepa la acción que se va a realizar
 * @param {string} id: Identificador de la persona sobre el que se aplicará la acción 
 * @param {any} idModalPanel: Panel modal contenedor donde se insertará este HTML (Por defecto será #modal-container)
 * */
function AccionEnviarMensajeMVC(urlPagina, id, titulo, idModalPanel = "#modal-container") {

    // Panel dinámico del modal padre donde se insertará la vista "hija"
    const $modalDinamicContentPanel = $('#modal-container').find('#modal-dinamic-content #content');

    // Declaración de la acción que se realizará al hacer click en Enviar mensaje
    var accion = "EnviarMensaje('" + urlPagina + "', '" + id + "');";

    // Plantilla del panel html que se cargará en el modal contenedor al pulsar en la acción
    var plantillaPanelHtml = '';
    // Cabecera del panel
    plantillaPanelHtml += '<div class="modal-header">';
    plantillaPanelHtml += '<p class="modal-title"><span class="material-icons">label</span>' + titulo + '</p>';
    plantillaPanelHtml += '<span class="material-icons cerrar" data-dismiss="modal" aria-label="Close">close</span>';
    plantillaPanelHtml += '</div>';
    // Cuerpo del panel
    plantillaPanelHtml += '<div class="modal-body">';
    plantillaPanelHtml += '<div class="formulario-edicion">';
    // Asunto del email - InputText
    plantillaPanelHtml += '<div class="form-group">';
    plantillaPanelHtml += '<label class="control-label" for="txtAsunto_' + id + '">' + mensajes.asunto + '</label>';
    plantillaPanelHtml += '<input type="text" class="form-control" id="txtAsunto_' + id + '" rows="3"> </textarea>';
    plantillaPanelHtml += '</div>';

    // Cuerpo del email - TextArea
    plantillaPanelHtml += '<div class="form-group">';
    plantillaPanelHtml += '<label class="control-label">' + mensajes.descripcion + '</label>';
    plantillaPanelHtml += '<textarea class="form-control cke mensajes" id="txtDescripcion_' + id + '" rows="3"> </textarea>';
    plantillaPanelHtml += '</div>';

    // Contenedor de mensajes y botones
    plantillaPanelHtml += '<div id="modal-dinamic-action-response">';
    // Posibles mensajes de info
    plantillaPanelHtml += '<div>';
    plantillaPanelHtml += '<div class="menssages_' + id + '" id="menssages">';
    plantillaPanelHtml += '<div class="ok"></div>';
    plantillaPanelHtml += '<div class="ko"></div>';
    plantillaPanelHtml += '</div>';
    plantillaPanelHtml += '</div>';
    // Panel de botones para la acción
    plantillaPanelHtml += '<div id="modal-dinamic-action-buttons" class="form-actions">'
    plantillaPanelHtml += '<button class="btn btn-primary">' + mensajes.enviar + '</button>'
    plantillaPanelHtml += '</div>';
    plantillaPanelHtml += '</div>';
    plantillaPanelHtml += '</div>';

    // Meter el código de la vista modal en el contenedor padre que viene identificado por el id #modal-container
    // En concreto, hay que buscar la etiqueta modal-dinamic-content #content e insertar el código
    $modalDinamicContentPanel.html(plantillaPanelHtml);

    // Acceso a los botones
    const botones = $modalDinamicContentPanel.find('#modal-dinamic-action-buttons > button');

    // Asignación de la función al botón "Sí" o de acción
    $(botones[0]).on("click", function () {
        EnviarMensaje(urlPagina, id);
    });

}

/**
 * Acción que se ejecuta para mandar un email a un contacto. Ej: Desde el perfil de un usuario, se puede pulsar en el botón "Escribir mensaje".
 * Esta acción es ejecutada desde AccionEnviarMensajeMVC
 * @param {any} urlPagina
 * @param {any} id
 */
function EnviarMensaje(urlPagina, id) {
    if ($('#txtAsunto_' + id).val() != '' && $('#txtDescripcion_' + id).val() != '') {
        var asunto = $('#txtAsunto_' + id).val();
        var mensaje = $('#txtDescripcion_' + id).val();
        MostrarUpdateProgress();

        var dataPost = {
            callback: "Accion_EnviarMensaje",
            asunto: asunto,
            mensaje: encodeURIComponent(mensaje.replace(/\n/g, ''))
        }

        GnossPeticionAjax(urlPagina, dataPost, true).done(function (data) {                        
            DesplegarResultadoAccionMVC("modal-dinamic-action-response", true, mensajes.mensajeEnviado);
            // Esperar 1,5 segundos y ocultar el panel
            setTimeout(() => {
                $('#modal-container').modal('hide');
            }, 1500)
        }).fail(function (data) {            
            DesplegarResultadoAccionMVC("modal-dinamic-action-response", false, "Se ha producido un error al enviar el mensaje. Por favor insertándolo de nuevo más tarde.");
        }).always(function (data) {
            OcultarUpdateProgress();
        });
    }
    else {
        $('.menssages_' + id).find(".ko").css("display", "block");
        $('.menssages_' + id).find(".ko").html(mensajes.mensajeError);
    }
}


/**
 * Envía una solicitud POST a la página especificada para agregar un contacto a la organización. 
 * La función envía una solicitud AJAX POST a `urlPagina` con un parámetro `callback` que indica la acción a realizar, en este caso, agregar un contacto. 
 * @param {string} urlPagina - La URL a la que se envía la solicitud POST para agregar el contacto. 
 * @returns {void} - La función no devuelve ningún valor.  
 */
function AgregarContactoOrg(urlPagina) {
    var dataPost = {
        callback: "Accion_AgregarContactoOrg"
    }
    $.post(urlPagina, dataPost);
}

/**
 * Envía una solicitud POST a la página especificada para eliminar un contacto de la organización.
 * La función envía una solicitud AJAX POST a `urlPagina` con un parámetro `callback` que indica la acción a realizar, en este caso, eliminar un contacto.
 * @param {string} urlPagina - La URL a la que se envía la solicitud POST para eliminar el contacto.
 * @returns {void} - La función no devuelve ningún valor.
 */
function EliminarContactoOrg(urlPagina) {
    var dataPost = {
        callback: "Accion_EliminarContactoOrg"
    }
    $.post(urlPagina, dataPost);
}

/**
 * Acción para expulsar a una persona de una comunidad. Se ejecuta cuando (por ejemplo) se selecciona desde listado de personas, la opción de "Expulsar"
 * Se cargará un nuevo modal para hacer la gestión de la explusión
 * @param {any} urlPagina: Url a la que se realizará la petición para expulsar a la persona
 * @param {any} id: Identificador de la persona a la que se expulsará
 * @param {any} titulo: Titulo de la ventana modal
 * @param {any} textoBotonPrimario: Titulo del botón primario
 * @param {any} textoBotonSecundario: Titulo del botón secundario (No/Cancelar)
 * @param {any} texto: Explicación de la acción de expulsar usuario
  * @param {any} accionCambiarNombreHtml: Accion JS que servirá para cambiar el nombre del elemento una vez se proceda a expulsar a una persona.
 * @param {any} idModalPanel: Panel modal contenedor donde se insertará este HTML (Por defecto será #modal-container)
 */
function Expulsar(urlPagina, id, titulo, textoBotonPrimario, textoBotonSecundario, texto, accionCambiarNombreHtml, idModalPanel = "#modal-container") {

    // Acción que se ejecutará al pulsar sobre el botón primario (Realizar la acción de Expulsar)
    var accion = "EnviarAccionExpulsar('" + urlPagina + "', '" + id + "');";

    // Panel dinámico del modal padre donde se insertará la vista "hija"
    const $modalDinamicContentPanel = $('#modal-container').find('#modal-dinamic-content #content');

    // Plantilla del panel html que se cargará en el modal contenedor al pulsar en la acción
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
    plantillaPanelHtml += '<label class="control-label">' + texto + '</label>';
    plantillaPanelHtml += '</div>';

    // Cuerpo del panel -> TextArea para enviar un email explicando la razón de la expulsión
    plantillaPanelHtml += '<div class="form-group">';
    plantillaPanelHtml += '<label for="txtMotivoExpulsion_' + id + '">' + accionesUsuarioAdminComunidad.motivoExpulsion + '</label>';
    plantillaPanelHtml += '<textarea class="form-control" id="txtMotivoExpulsion_' + id + '" rows="3"></textarea>';
    plantillaPanelHtml += '</div>';

    plantillaPanelHtml += '<div id="modal-dinamic-action-response">';
    // Posibles mensajes de info
    plantillaPanelHtml += '<div>';
    plantillaPanelHtml += '<div id="menssages">';
    plantillaPanelHtml += '<div class="ok"></div>';
    plantillaPanelHtml += '<div class="ko"></div>';
    plantillaPanelHtml += '</div>';
    plantillaPanelHtml += '</div>';
    // Panel de botones para la acción
    plantillaPanelHtml += '<div id="modal-dinamic-action-buttons" class="form-actions">'
    plantillaPanelHtml += '<button data-dismiss="modal" class="btn btn-primary">' + textoBotonSecundario + '</button>'
    plantillaPanelHtml += '<button class="btn btn-outline-primary ml-1" onclick="' + accion + '">' + textoBotonPrimario + ", " + accionesUsuarioAdminComunidad.expulsarUsuario + '</button>'
    plantillaPanelHtml += '</div>';
    plantillaPanelHtml += '</div>';
    plantillaPanelHtml += '</div>';

    // Meter el código de la vista modal en el contenedor padre que viene identificado por el id #modal-container
    // En concreto, hay que buscar la etiqueta modal-dinamic-content #content e insertar el código
    $modalDinamicContentPanel.html(plantillaPanelHtml);

    // Asignar acciones adicionales al botón primario (Cambiar nombre del html)
    // Acceso a los botones
    const botones = $modalDinamicContentPanel.find('#modal-dinamic-action-buttons > button');

    // Asignación de la función al botón "Sí" o de acción
    $(botones[1]).on("click", function () {
        // Ocultar el panel modal de bootstrap si hiciera falta        
    }).click(accionCambiarNombreHtml);
}

/**
 * Enviar la nueva petición del cambio de expulsión de una comunidad a un perfil sobre el que se ha pulsado el botón de "Sí, Expulsar" del modal.
 * @param {any} urlPagina: Url donde se lanzará la petición para cambiar el rol
 * @param {any} id: Identificador de la persona 
 */
function EnviarAccionExpulsar(urlPagina, id) {

    if ($('#txtMotivoExpulsion_' + id).val() != '') {
        var motivo = $('#txtMotivoExpulsion_' + id).val();
        MostrarUpdateProgress();

        var dataPost = {
            callback: "Accion_Expulsar",
            motivo: motivo
        }

        GnossPeticionAjax(urlPagina, dataPost, true).done(function (data) {
            // Cambiado por el nuevo front
            // DesplegarResultadoAccionMVC("desplegable_" + id, true, accionesUsuarioAdminComunidad.miembroExpulsado);
            DesplegarResultadoAccionMVC("modal-dinamic-action-response", true, accionesUsuarioAdminComunidad.miembroExpulsado);
            // Esperar 1 segundo y cerrar la ventana
            setTimeout(() => {
                $('#modal-container').modal('hide');
            }, 1500)

        }).fail(function (data) {
            // Cambiado por nuevo front
            //DesplegarResultadoAccionMVC("desplegable_" + id, false, "");
            DesplegarResultadoAccionMVC("modal-dinamic-action-response", false, "Error al tratar de expulsar al perfil de la comunidad. Por favor, insertándolo de nuevo más tarde.");
        }).always(function (data) {
            OcultarUpdateProgress();
        });
    }
    else {
        // Cambiado por nuevo Front
        //$('#error_' + id).html(accionesUsuarioAdminComunidad.expulsionMotivoVacio);
        DesplegarResultadoAccionMVC("modal-dinamic-action-response", false, accionesUsuarioAdminComunidad.expulsionMotivoVacio);
    }
}

/**
 * Acción que se ejecuta cuando se pulsa sobre la acción de "Cambiar rol" disponible en un item/recurso de tipo "Perfil" encontrado por el buscador.  
 * @param {string} id: Identificador del recurso (en este caso de la persona) sobre el que se aplicará la acción 
 * @param {any} rol: El rol actual del recurso (Perfil) clickeado
 * @param {any} urlPagina: Pagina sobre la que se lanzará la llamada para realizar la acción de cambiar rol
 * @param {any} idModalPanel: Panel modal contenedor donde se insertará este HTML (Por defecto será #modal-container)
 */
function CambiarRol(id, rol, urlPagina, idModalPanel = "#modal-container") {

    // Acción que se ejecutará al pulsar sobre el botón primario (Realizar la acción de cambiar rol)
    var accion = "EnviarAccionCambiarRol('" + urlPagina + "', '" + id + "', '" + rol + "');";
    // Permisos para pintar los checkbox a mostrar al usuario
    var checkedAdmin = '';
    var checkedSupervisor = '';
    var checkedUsuario = '';

    if (rol == 0) {
        checkedAdmin = ' checked';
    }
    else if (rol == 1) {
        checkedSupervisor = ' checked';
    }
    else if (rol == 2) {
        checkedUsuario = ' checked';
    }

    // Panel dinámico del modal padre donde se insertará la vista "hija"
    const $modalDinamicContentPanel = $('#modal-container').find('#modal-dinamic-content #content');

    // Plantilla del panel html que se cargará en el modal contenedor al pulsar en la acción
    var plantillaPanelHtml = '';
    // Cabecera del panel
    plantillaPanelHtml += '<div class="modal-header">';
    plantillaPanelHtml += '<p class="modal-title"><span class="material-icons">label</span>' + accionesUsuarioAdminComunidad.cambiarRol + '</p>';
    plantillaPanelHtml += '<span class="material-icons cerrar" data-dismiss="modal" aria-label="Close">close</span>';
    plantillaPanelHtml += '</div>';
    // Cuerpo del panel
    plantillaPanelHtml += '<div class="modal-body">';
    plantillaPanelHtml += '<div class="formulario-edicion">';
    plantillaPanelHtml += '<div class="form-group">';
    plantillaPanelHtml += '<label class="control-label">' + accionesUsuarioAdminComunidad.selecionaRol + '</label>';
    plantillaPanelHtml += '</div>';
    // Cuerpo del panel -> Opciones de checkbox a cargar
    plantillaPanelHtml += '<div class="form-group">';
    plantillaPanelHtml += '<div class="themed primary custom-control custom-radio">';
    plantillaPanelHtml += '<input name="cambiarRol_' + id + '" type="radio" id="cambiarRol_administrador' + id + '" value="0" class="custom-control-input"' + checkedAdmin + '/>';
    plantillaPanelHtml += '<label class="custom-control-label" for="cambiarRol_administrador' + id + '">' + accionesUsuarioAdminComunidad.administrador + '</label>';
    plantillaPanelHtml += '</div>';
    plantillaPanelHtml += '<div name="cambiarRol' + id + '" class="themed primary custom-control custom-radio">';
    plantillaPanelHtml += '<input name="cambiarRol_' + id + '" type="radio" id="cambiarRol_supervisor' + id + '" value="1" class="custom-control-input"' + checkedSupervisor + '/>';
    plantillaPanelHtml += '<label class="custom-control-label" for="cambiarRol_supervisor' + id + '">' + accionesUsuarioAdminComunidad.supervisor + '</label>';
    plantillaPanelHtml += '</div>';
    plantillaPanelHtml += '<div class="themed primary custom-control custom-radio">';
    plantillaPanelHtml += '<input name="cambiarRol_' + id + '" type="radio" id="cambiarRol_usuario' + id + '" value="2" class="custom-control-input"' + checkedUsuario + '/>';
    plantillaPanelHtml += '<label class="custom-control-label" for="cambiarRol_usuario' + id + '">' + accionesUsuarioAdminComunidad.usuario + '</label>';
    plantillaPanelHtml += '</div>';
    plantillaPanelHtml += '</div>';

    plantillaPanelHtml += '<div id="modal-dinamic-action-response">';
    // Posibles mensajes de info
    plantillaPanelHtml += '<div>';
    plantillaPanelHtml += '<div id="menssages">';
    plantillaPanelHtml += '<div class="ok"></div>';
    plantillaPanelHtml += '<div class="ko"></div>';
    plantillaPanelHtml += '</div>';
    plantillaPanelHtml += '</div>';
    // Panel de botones para la acción
    plantillaPanelHtml += '<div id="modal-dinamic-action-buttons" class="form-actions">'
    plantillaPanelHtml += '<button class="btn btn-primary" onclick="' + accion + '">' + accionesUsuarioAdminComunidad.cambiarRol + '</button>'
    plantillaPanelHtml += '</div>';
    plantillaPanelHtml += '</div>';
    plantillaPanelHtml += '</div>';

    // Meter el código de la vista modal en el contenedor padre que viene identificado por el id #modal-container
    // En concreto, hay que buscar la etiqueta modal-dinamic-content #content e insertar el código
    $modalDinamicContentPanel.html(plantillaPanelHtml);

}

/**
 * Enviar la nueva petición del cambio de rol una vez se ha pulsado sobre el botón de "Cambiar rol"
 * @param {any} urlPagina: Url donde se lanzará la petición para cambiar el rol
 * @param {any} id: Identificador de la persona
 * @param {any} rol: Rol actual de la persona. Si es el mismo, no hará nada
 */
function EnviarAccionCambiarRol(urlPagina, id, rol) {
    var rolNuevo = $('input[name="cambiarRol_' + id + '"]:checked').val();

    if (rolNuevo != rol) {
        MostrarUpdateProgress();

        var dataPost = {
            callback: "Accion_CambiarRol",
            rol: rolNuevo
        }

        GnossPeticionAjax(urlPagina, dataPost, true).done(function (data) {
            // Cambiado mensaje por el nuevo Front
            // CambiarNombreElemento(id + '_CambiarRol', accionesUsuarioAdminComunidad.rolCambiado);
            CambiarTextoAndEliminarAtributos(id + '_CambiarRol', accionesUsuarioAdminComunidad.rolCambiado, ['onclick', 'data-target', 'data-toggle']);
            DesplegarResultadoAccionMVC("modal-dinamic-action-response", true, accionesUsuarioAdminComunidad.rolCambiado);
            // Esperar 1 segundo y cerrar la ventana
            setTimeout(() => {
                $('#modal-container').modal('hide');
            }, 1500)
        }).fail(function (data) {
            DesplegarResultadoAccionMVC("modal-dinamic-action-response", false, "Error al tratar de cambiar el rol. Por favor, insertándolo de nuevo más tarde.");
        }).always(function (data) {
            OcultarUpdateProgress();
        });
    }
}

/**
 * Envía una solicitud POST a la página especificada para realizar la acción de readmisión. 
 * La función envía una solicitud AJAX POST a `urlPagina` con un parámetro `callback` que indica la acción a realizar, en este caso, readmitir. 
 * @param {string} urlPagina - La URL a la que se envía la solicitud POST para readmitir. 
 * @returns {void} - La función no devuelve ningún valor.
 */
function Readmitir(urlPagina) {
    var dataPost = {
        callback: "Accion_Readmitir"
    }
    $.post(urlPagina, dataPost);

    setTimeout(() => {
        $('#modal-container').modal('hide');
    }, 1500)
}

/**
 * Envía una solicitud POST a la página especificada para realizar la acción de bloqueo de un usuario.
 * La función envía una solicitud AJAX POST a `urlPagina` con un parámetro `callback` que indica la acción a realizar, en este caso, bloquear.
 * @param {string} urlPagina - La URL a la que se envía la solicitud POST para bloquear.
 * @returns {void} - La función no devuelve ningún valor.
 */
function Bloquear(urlPagina) {
    var dataPost = {
        callback: "Accion_Bloquear"
    }
    $.post(urlPagina, dataPost);

    setTimeout(() => {
        $('#modal-container').modal('hide');
    }, 1500)
}

/**
 * Envía una solicitud POST a la página especificada para realizar la acción de desbloquear a un usuario.
 * La función envía una solicitud AJAX POST a `urlPagina` con un parámetro `callback` que indica la acción a realizar, en este caso, bloquear.
 * @param {string} urlPagina - La URL a la que se envía la solicitud POST para bloquear.
 * @returns {void} - La función no devuelve ningún valor.
 */
function Desbloquear(urlPagina) {
    var dataPost = {
        callback: "Accion_Desbloquear"
    }
    $.post(urlPagina, dataPost);

    setTimeout(() => {
        $('#modal-container').modal('hide');
    }, 1500)
}

/**
 * Envía una solicitud POST para enviar un boletín informativo y oculta el modal después de un retraso.
 * @param {string} urlPagina - La URL a la que se envía la solicitud POST para enviar el boletín.
 * @returns {void} 
 */
function EnviarNewsletter(urlPagina) {
    var dataPost = {
        callback: "Accion_EnviarNewsletter"
    }
    $.post(urlPagina, dataPost);

    setTimeout(() => {
        $('#modal-container').modal('hide');
    }, 1500)
}


/**
 * Envía una solicitud POST para no enviar un boletín informativo y oculta el modal después de un retraso.
 * @param {string} urlPagina - La URL a la que se envía la solicitud POST para no enviar el boletín.
 * @returns {void}
 */
function NoEnviarNewsletter(urlPagina) {
    var dataPost = {
        callback: "Accion_NoEnviarNewsletter"
    }
    $.post(urlPagina, dataPost);

    setTimeout(() => {
        $('#modal-container').modal('hide');
    }, 1500)
}

/**/ 
/*MVC.Registro.js*/ 
/**
 * Comprueba si las condiciones de uso están aceptadas durante el registro de un usuario logeado.
 * Si hay errores en las condiciones, los muestra en el div correspondiente.
 * @returns {boolean} - Devuelve `true` si hay errores en las condiciones, de lo contrario `false`.
 */
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

/**
 * Valida los datos del formulario de registro de usuario, incluyendo nombre, apellidos, usuario, email, contraseña y más.
 * Muestra errores en los divs correspondientes si se encuentran problemas en los datos del registro.
 * @param {number} pEdadMinimaRegistro - Edad mínima requerida para el registro (opcional, pero no se usa en la función).
 * @returns {boolean} - Devuelve `true` si se encontraron errores en los datos de registro, `false` si todos los datos son válidos.
 */
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

/**
 * Valida el campo de login del usuario asegurando que el nombre de usuario no exceda los 12 caracteres. 
 * @param {string} pTxtLogin - El ID del campo de texto del login a validar.
 * @returns {string} - Un mensaje de error si el nombre de usuario es inválido, una cadena vacía si es válido.
 */
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

/**
 * Realiza validaciones para los campos de email proporcionados.
 *
 * @param {string} pTxtEmail - ID del campo de email principal.
 * @param {string} pTxtEmailTutor - ID del campo de email del tutor.
 * @returns {string} - Mensajes de error generados durante la validación.
 */
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
            if (textEmailTutor == '') {
                error += AgregarErrorReg(error, ValidarEmailIntroducido(pTxtEmail, 'lblEmail'));
            }
        }
    }
    else {
        error += AgregarErrorReg(error, ValidarEmailIntroducido(pTxtEmail, 'lblEmail'));
    }

    return error;
}

/**
 * Verifica si el campo de email del tutor tiene un valor válido. 
 * @param {string} pTxtMail - ID del campo de email del tutor.
 * @param {string} pLblMail - ID del label asociado al campo de email.
 * @returns {boolean} - `true` si el campo de email tiene un valor, `false` si está vacío o indefinido.
 */
function ValidacionEmailTutor(pTxtMail, pLblMail) {
    var textEmail = $('#' + pTxtMail).val();
    if (textEmail != undefined && textEmail == '') {
        return false;
    }
    return true;
}

/**
 * Verifica que todas las cláusulas obligatorias estén aceptadas y actualiza la lista de cláusulas seleccionadas. 
 * @returns {string} - Mensaje de error si alguna cláusula obligatoria no está aceptada, vacío si todo está correcto.
 */
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


/**
 * Muestra el panel adicional y desplaza la vista hacia él. 
 * Esta función hace visible el panel de registro, muestra el formulario dentro de él, 
 * y desplaza la vista del navegador hacia la parte superior del panel.  
 * @returns {boolean} - Siempre retorna `false` para evitar comportamientos por defecto del evento.
 */
function MostrarPanelExtra() {
    $('#despleReg').show();
    $('#despleReg .stateShowForm').show();
    $('html,body').animate({ scrollTop: $('#despleReg').offset().top }, 'slow');
    return false;
}


/**
 * Verifica la validez del email introducido y realiza una solicitud POST para comprobar su existencia en el servidor. 
 * @param {string} pUrlPagina - URL de la página del servidor a la que se enviará la solicitud POST.
 * @returns {void} - No retorna ningún valor, maneja la lógica de validación y llamada al servidor.
 */
function ComprobarEmailUsuario(pUrlPagina) {
    var validarEmail = ValidarEmailIntroducido('txtEmail', 'lblEmail');
    if (validarEmail == '') {
        var dataPost = {
            callback: "comprobarEmail",
            email: $('#txtEmail').val()
        }
        $.post(pUrlPagina, dataPost, function (data) {
            ProcesarEmailComprobado(data);
        });
    } else {
        if ($('#divKodatosUsuario').length == 0) {
            $('#lblEmail').parent().after('<p id="divKoEmail"></p>');
        }
        var mensaje = validarEmail;
        crearError(mensaje, '#divKodatosUsuario');
        //$('#lblEmail').attr('class', 'ko');
        $('#txtEmail').addClass('is-invalid');
        $('#txtEmail').removeClass('is-valid');
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
    else {
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


/**
 * Valida el campo de registro especificado llamando a la función de validación correspondiente.
 * @param {string} pCampo - El identificador del campo a validar. 
 * @returns {void} - No retorna ningún valor, llama a funciones de validación específicas basadas en el valor de `pCampo`.
 */
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


/**
 * Valida una fecha en formato DDMMAAAA (día/mes/año) asegurando que es una fecha válida.
 *
 * @param {string} fecha - La fecha en formato DDMMAAAA a validar.
 * @returns {boolean} - Retorna `true` si la fecha es válida, de lo contrario, retorna `false`.
 */
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


/**
 * Valida la fecha de nacimiento en formato DDMMAAAA asegurando que la persona cumple con una edad mínima.
 * Actualiza el estado del campo de fecha de nacimiento con un mensaje de error si no se cumple con los requisitos.
 * @param {string} pFechaNacimiento - La fecha de nacimiento en formato DDMMAAAA.
 * @param {string} pLblFechaNacimiento - El ID del elemento del label asociado a la fecha de nacimiento.
 * @param {number} pEdadMinima - La edad mínima que debe cumplir la persona.
 * @returns {string} - Mensaje de error si la fecha no es válida o no se cumple con la edad mínima; de lo contrario, retorna una cadena vacía.
 */
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


/**
 * Valida si un campo de formulario obligatorio está vacío y actualiza la clase del elemento de etiqueta asociado.
 * Retorna un mensaje de error si el campo está vacío.
 * @param {jQuery} $campo - El campo de formulario que se va a validar.
 * @param {jQuery} $lblCampo - La etiqueta asociada al campo que se actualizará según la validación.
 * @returns {string} Mensaje de error si el campo está vacío, cadena vacía si el campo es válido.
 */
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


/**
 * Valida si un campo de selección obligatorio tiene el valor por defecto y actualiza la clase del elemento de etiqueta asociado.
 * Retorna un mensaje de error si el valor del campo es el valor por defecto.
 * @param {jQuery} $campo - El campo de selección que se va a validar.
 * @param {jQuery} $lblCampo - La etiqueta asociada al campo que se actualizará según la validación.
 * @param {string} [mensajeError=form.camposVacios] - Mensaje de error personalizado si el campo tiene el valor por defecto.
 * @param {string} [valorPorDefecto='00000000-0000-0000-0000-000000000000'] - Valor que indica un campo no seleccionado.
 * @returns {string} Mensaje de error si el valor del campo es el valor por defecto, cadena vacía si el campo es válido.
 */
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

/**
 * Configura un campo de entrada para utilizar la funcionalidad de autocompletado con parámetros personalizados. 
 * @param {string} inputID - El ID del campo de entrada donde se aplicará el autocompletado.
 * @param {Object} [argumentos={}] - Un objeto con parámetros adicionales que se enviarán en la solicitud de autocompletado.
 * @param {string} proyectoID - El ID del proyecto que se incluirá en la solicitud de autocompletado.
 * @param {string} [urlServicio='/AutoCompletarDatoExtraProyectoVirtuoso'] - La URL del servicio de autocompletado. Por defecto es la URL especificada en el campo `#inpt_urlServicioAutocompletar`.
 * @param {number} [minChars=1] - Número mínimo de caracteres requeridos para activar el autocompletado. Por defecto es 1.
 * @param {number} [delay=0] - Tiempo de espera en milisegundos antes de enviar la solicitud de autocompletado. Por defecto es 0.
 * @returns {void}
 */
function PrepararAutocompletar(inputID, argumentos, proyectoID) {
    $('#' + inputID).unautocomplete().unbind("focus")
    .autocomplete(
        null,
        {
            url: $('#inpt_urlServicioAutocompletar').val() + "/AutoCompletarDatoExtraProyectoVirtuoso",
            type: "POST",
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


/**
 * Obtiene los valores de varios campos de entrada basados en una lista de IDs y los concatena en una cadena, separada por '|' (barra vertical). 
 * @param {string} [variables=''] - Una cadena con IDs de campos de entrada separados por '|'. Si no se proporciona o está vacía, se devuelve una cadena vacía.
 * @returns {string} Una cadena con los valores de los campos de entrada, separados por '|' (barra vertical).
 */
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
}/**/ 

/*MVC.Comun.js*/ 
/**
 * Configura un campo de autocompletar para buscar lectores y editores basados en una base de recursos específica. 
 * @param {jQuery} control - El campo de entrada que se utilizará para el autocompletado.
 * @param {string} baseRecursosID - El ID de la base de recursos para la búsqueda.
 * @param {string} personaID - El ID de la persona que realiza la búsqueda.
 * @param {string} pTxtValSeleccID - El ID del campo donde se guardarán los valores seleccionados.
 * @param {Array<string>} pListaUrlsAutocompletar - Lista de URLs para el servicio de autocompletar.
 * @param {string} panelContenedorID - El ID del panel que contendrá los resultados del autocompletado.
 * @param {string} panResultadosID - El ID del panel donde se mostrarán los resultados.
 * @param {string} txtHackID - El ID del campo oculto para el hack de autocompletado. 
 */
function CargarAutocompletarLectoresEditoresPorBaseRecursos(control, baseRecursosID, personaID, pTxtValSeleccID, pListaUrlsAutocompletar, panelContenedorID, panResultadosID, txtHackID) {
    control.unautocomplete().autocomplete(
        null,
        {
            servicio: new WS(pListaUrlsAutocompletar, WSDataType.jsonp),
            metodo: 'AutoCompletarLectoresEditoresPorBaseRecursos',
            delay: 0,
            multiple: true,
            scroll: false,
            selectFirst: false,
            minChars: 1,
            width: 300,
            cacheLength: 0,
            NoPintarSeleccionado: true,
            txtValoresSeleccID: pTxtValSeleccID,
            extraParams: {
                baserecursos: baseRecursosID,
                persona: personaID
            }
        }
    );
    control.result(function (event, data, formatted) {
        seleccionarAutocompletarMVC(data[0], data[1], control, panelContenedorID, panResultadosID, txtHackID);
    });
}

/**
 * Añade un nuevo elemento al panel de resultados del autocompletado y actualiza el campo oculto con la identidad del elemento. 
 * @param {string} nombre - El nombre del elemento seleccionado.
 * @param {string} identidad - La identidad del elemento seleccionado.
 * @param {jQuery} txtFiltro - El campo de entrada donde se muestra el autocompletado.
 * @param {string} panelContenedorID - El ID del panel que contiene los resultados del autocompletado.
 * @param {string} panResultadosID - El ID del panel donde se mostrarán los resultados.
 * @param {string} txtHackID - El ID del campo oculto para almacenar la identidad del elemento. 
 */
function seleccionarAutocompletarMVC(nombre, identidad, txtFiltro, panelContenedorID, panResultadosID, txtHackID) {
    txtFiltro.val('');

    $('#selector').css('display', 'none');

    var contenedor = $('#' + panelContenedorID).find('#' + panResultadosID);

    if (contenedor.html().trim().indexOf('<ul') == 0) {
        contenedor.html(contenedor.html().replace('<ul class=\"icoEliminar\">', '').replace('</ul>', ''));
    }
    // Cambiado por nuevo Front
    // contenedor.html('<ul class=\"icoEliminar\">' + contenedor.html() + '<li>' + nombre + '<a class="remove" onclick="javascript:eliminarAutocompletarMVC(this,\'' + identidad + '\',\'' + panelContenedorID + '\',\'' + panResultadosID + '\',\'' + txtHackID + '\');">' + textoRecursos.Eliminar + '</a></li>' + '</ul>');
    // Construyo el editor
    const editorSeleccionadoHtml = `
        <div class="tag">
            <div class="tag-wrap">
                <span class="tag-text">${nombre}</span>
                <span class="tag-remove material-icons"
                      onclick="javascript:eliminarAutocompletarMVC(this,'${identidad}','${panelContenedorID}','${panResultadosID}','${txtHackID}');">close
                </span>
                <input type="hidden" value="${identidad}"/>
            </div>
        </div>
    `;
    // Añadir editor al contenedor
    contenedor.append(editorSeleccionadoHtml);

    // Añadirlo al input txtHack
    var txtHack = $('#' + panelContenedorID).find('#' + txtHackID);
    txtHack.val(txtHack.val() + "," + identidad);

    contenedor.css('display', '');
}

/**
 * Elimina un elemento del panel de resultados del autocompletado y actualiza el campo oculto para eliminar la identidad del elemento. 
 * @param {HTMLElement} that - El elemento HTML del botón de eliminación.
 * @param {string} identidad - La identidad del elemento a eliminar.
 * @param {string} panelContenedorID - El ID del panel que contiene los resultados del autocompletado.
 * @param {string} panResultadosID - El ID del panel donde se mostrarán los resultados.
 * @param {string} txtHackID - El ID del campo oculto para almacenar las identidades de los elementos seleccionados. 
 */
function eliminarAutocompletarMVC(that, identidad, panelContenedorID, panResultadosID, txtHackID) {
    $(that).parent().parent().remove();

    var txtHack = $('#' + panelContenedorID).find('#' + txtHackID);
    txtHack.val(txtHack.val().replace(',' + identidad, ''));

    if (txtHack.val() == "") {
        var contenedor = $('#' + panelContenedorID).find('#' + panResultadosID);
        contenedor.css('display', 'none');
    }
}


/**
 * Carga más actividades recientes y actualiza el panel de actividades.  
 * Esta función envía una solicitud POST a una URL para cargar más actividades recientes basadas en el tipo de actividad y los parámetros proporcionados. Luego, actualiza el contenido del panel de actividades y obtiene acciones adicionales para el listado.
 * @param {string} pUrlLoadMoreActivity - La URL a la que se envía la solicitud POST para cargar más actividades.
 * @param {string} pUrlLoadActions - La URL para obtener acciones adicionales para el listado de actividades.
 * @param {string} pTipoActividad - El tipo de actividad para filtrar las actividades recientes.
 * @param {number} pNumPeticion - El número de petición para la solicitud de actividades.
 * @param {string} idPanel - El ID del panel donde se mostrarán las actividades recientes.
 * @param {string} pComponentKey - La clave del componente para la solicitud.
 * @param {string} pProfileKey - La clave del perfil para la solicitud.
 */
function ActividadReciente_MostrarMas(pUrlLoadMoreActivity, pUrlLoadActions, pTipoActividad, pNumPeticion, idPanel, pComponentKey, pProfileKey) {
    MostrarUpdateProgress();
    var datosPost = {
        NumPeticion: pNumPeticion,
        TypeActivity: pTipoActividad,
        ComponentKey: pComponentKey,
        ProfileKey: pProfileKey
    };

    $.post(pUrlLoadMoreActivity, datosPost, function (data) {
        $("#actividadReciente_" + idPanel).replaceWith(data);
        OcultarUpdateProgress();
        ObtenerAccionesListadoMVC(pUrlLoadActions);
    });
}


var ObteniendoAcciones = false;
/**
 * Obtiene acciones adicionales para una lista de recursos y actualiza los paneles correspondientes.  
 * Esta función envía una solicitud POST a una URL proporcionada para obtener una lista de acciones para los recursos en la página. Luego, actualiza los paneles de acciones con el HTML recibido. 
 * @param {string} pUrlPagina - La URL a la que se envía la solicitud POST para obtener las acciones. 
 */
function ObtenerAccionesListadoMVC(pUrlPagina) {
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
 * Alterna la visibilidad de un panel de imagen de usuario y cambia el ícono del botón.
 * Esta función cambia el texto del botón entre "person" y "person_search" para alternar entre dos estados. Además, muestra u oculta un panel de imagen de usuario basado en su clase de CSS.
 * @param {string} pBoton - Selector o referencia al botón que se usa para alternar el panel.
 * @param {string} pPanel - Selector o referencia al panel de imagen de usuario que se muestra u oculta.
 */
function DesplegarUserImgMasMVC(pBoton, pPanel) {
    const $boton = $(pBoton);
    const $panel = $(pPanel);

    if ($boton.text().trim() == "person_search") {
        $boton.text("person");
    } else {
        $boton.text("person_search");
    }

    $panel.toggleClass("d-none");
}

/**
 * Realiza una petición AJAX utilizando jQuery con opciones configurables y maneja la respuesta.
 * Esta función realiza una solicitud POST a una URL dada, maneja el progreso de la carga, y resuelve o rechaza una promesa en función de la respuesta del servidor. 
 * También gestiona los errores de red y redirige a una URL si es necesario.
 * @param {string} url - La URL a la que se enviará la solicitud AJAX.
 * @param {object|FormData} parametros - Los parámetros a enviar en la solicitud. Puede ser un objeto o una instancia de FormData.
 * @param {boolean} traerJson - Indica si se espera una respuesta en formato JSON.
 * @param {boolean} [redirectActive=true] - Indica si se debe redirigir a una URL especificada en la respuesta.
 * @returns {Promise} Una promesa que se resuelve con la respuesta del servidor o se rechaza con un mensaje de error.
 */
function GnossPeticionAjax(url, parametros, traerJson, redirectActive = true) {
    var defr = $.Deferred();

    var esFormData = parametros instanceof FormData;

    $.ajax({
        url: url,
        type: "POST",
        headers: {
            Accept: traerJson ? 'application/json' : '*/*'
        },
        processData: !esFormData,
        contentType: esFormData ? false : 'application/x-www-form-urlencoded',
        data: parametros,
        xhr: function () {
            var xhr = new window.XMLHttpRequest();
            // Handle progress
            //Upload progress
            xhr.upload.addEventListener("progress", function (evt) {
                if (evt.lengthComputable) {
                    var percentComplete = evt.loaded / evt.total;
                    //Do something with upload progress
                    defr.notify(Math.round(percentComplete * 100));
                }
            }, false);

            return xhr;
        }
    }).done(function (response) {
        if (response == null || response.Status == undefined) {
            defr.resolve(response);
        }
        if (response.Status == "NOLOGIN") {
            //Hacer una peticion a la web, que nos devuelva la url del servicio de login + token
            //Hacer la peticion al servicio de login a recuperar la sesion
            //Si no estamos conectados, mostrar un panel de login
            //Si estamos conectados, re-llamar a esta funcion
            defr.reject("invitado");
        }
        else if (response.Status == "OK") {
            if (response.UrlRedirect != null) {
                if (redirectActive) { location.href = response.UrlRedirect; }
                else { defr.resolve(response.UrlRedirect); }
            }
            else if (response.Html != null) {
                defr.resolve(response.Html);
            }
            else {
                defr.resolve(response.Message);
            }
        }
        else if (response.Status == "Error") {
            defr.reject(response.Message);
        }
    }).fail(function (er) {
        //Comprobar el error
        var newtWorkError = er.readyState == 0;// && er.statusText == "error";
        if (newtWorkError) {
            defr.reject("NETWORKERROR");
        }
        else {
            defr.reject(er.statusText);
        }
    });

    return defr;
}


/**
 * Objeto que maneja el comportamiento para redirigir a una ficha de recurso en caso de errores de red.
 * Este objeto proporciona métodos para intentar obtener una ficha de recurso mediante repetidas solicitudes
 * y manejar errores de conexión. Si no se puede obtener la ficha, se llama a una función de manejo de errores.
 */
var comportamientoNetworkError = {
    intentarRedirigirFichaRecurso: function (url, funcion) {
        if (documentoID != null && documentoID != '') {
            url += "?documentoID=" + documentoID;
        }
        this.urlObtenerFicha = url;
        this.funcionEjecutar = funcion;
        this.cont = 0;
        var that = this;
        //setTimeout(function () { that.obtenerFichaRecurso(); }, 1000);
        that.obtenerFichaRecurso();
    },
    obtenerFichaRecurso: function () {
        var that = this;
        $.ajax({
            url: that.urlObtenerFicha,
            method: 'GET',
            async: false,
            success: function (data) {
                //la ficha existe, redirigir
                if (data != '') {
                    document.location.href = data;
                }
                else {
                    if (that.cont < 10) {
                        that.cont++;
                        setTimeout(function () { that.obtenerFichaRecurso(); }, 1000);
                    }
                    else {
                        //mostrar error
                        that.obtenerMensajeError();
                    }
                }
            },
            error: function (data) {
                //la ficha no existe
                if (data.readyState == 0 && that.cont < 10) {
                    that.cont++;
                    setTimeout(function () { that.obtenerFichaRecurso(); }, 1000);
                }
                else {
                    //mostrar error
                    that.obtenerMensajeError();
                }
            }
        });
    },
    obtenerMensajeError: function () {
        this.funcionEjecutar('Has perdido la conexión y no se ha podido recuperar el recurso. Comprueba tu conexión a internet y verifica que tus cambios se han guardado correctamente ');
    }
}

/**
 * Genera un identificador único global (GUID) en formato estándar.
 * Esta función crea un GUID de 128 bits (16 bytes) en formato hexadecimal, que se puede utilizar como un identificador único en aplicaciones web.
 * El GUID generado sigue el formato: `xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx`, donde `x` es un dígito hexadecimal.
 * @returns {string} El GUID generado en formato `xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx`.
 */
function guidGenerator() {
    var S4 = function () {
        return (((1 + Math.random()) * 0x10000) | 0).toString(16).substring(1);
    };
    return (S4() + S4() + "-" + S4() + "-" + S4() + "-" + S4() + "-" + S4() + S4() + S4());
}

/**
 * Extiende la funcionalidad de String añadiendo un método que verifica si una cadena de texto termina con un sufijo específico. 
 * @param {string} suffix - El sufijo que se busca en el final de la cadena.
 * @returns {boolean} `true` si la cadena termina con el sufijo, de lo contrario `false`. 
 */
String.prototype.EndsWith = function (suffix) {
    return this.indexOf(suffix, this.length - suffix.length) !== -1;
};

/**
 * Extiende la funcionalidad de String añadiendo un método que verifica si una cadena de texto comienza con una subcadena específica desde una posición dada.
 * @param {string} searchString - La subcadena que se busca al principio de la cadena.
 * @param {number} [position=0] - La posición en la cadena desde donde comenzar la búsqueda (por defecto es 0).
 * @returns {boolean} `true` si la cadena comienza con la subcadena desde la posición dada, de lo contrario `false`.
 */
String.prototype.StartsWith = function (searchString, position) {
    position = position || 0;
    return this.lastIndexOf(searchString, position) === position;
};

/**
 * Añadir funcionalidad a Date cuyo método sirve para formatear una fecha en un formato específico. 
 * @function format
 * @param {string} [format="MM/dd/yyyy"] - El formato deseado para la fecha. Los formatos soportados son: 
 * @returns {string} La fecha formateada según el formato especificado.
 */
Date.prototype.format = function (format) {
    var date = this,
        day = date.getDate(),
        month = date.getMonth() + 1,
        year = date.getFullYear(),
        hours = date.getHours(),
        minutes = date.getMinutes(),
        seconds = date.getSeconds();

    if (!format) {
        format = "MM/dd/yyyy";
    }

    format = format.replace("MM", month.toString().replace(/^(\d)$/, '0$1'));

    if (format.indexOf("yyyy") > -1) {
        format = format.replace("yyyy", year.toString());
    } else if (format.indexOf("yy") > -1) {
        format = format.replace("yy", year.toString().substr(2, 2));
    }

    format = format.replace("dd", day.toString().replace(/^(\d)$/, '0$1'));

    if (format.indexOf("t") > -1) {
        if (hours > 11) {
            format = format.replace("t", "pm");
        } else {
            format = format.replace("t", "am");
        }
    }

    if (format.indexOf("HH") > -1) {
        format = format.replace("HH", hours.toString().replace(/^(\d)$/, '0$1'));
    }

    if (format.indexOf("hh") > -1) {
        if (hours > 12) {
            hours -= 12;
        }

        if (hours === 0) {
            hours = 12;
        }
        format = format.replace("hh", hours.toString().replace(/^(\d)$/, '0$1'));
    }

    if (format.indexOf("mm") > -1) {
        format = format.replace("mm", minutes.toString().replace(/^(\d)$/, '0$1'));
    }

    if (format.indexOf("ss") > -1) {
        format = format.replace("ss", seconds.toString().replace(/^(\d)$/, '0$1'));
    }

    return format;
};

/**
 * Método para filtrar elementos. En este caso, en la lista de Categorías, modo "Lista" de la ficha Recurso.
 * Al teclear en el input, filtrará (ocultará) los elementos que no correspondan con el texto de la búsqueda
 * @param {any} txt
 * @param {any} panDesplID
 */
function MVCFiltrarListaSelCat(txt, panDesplID) {
    var cadena = $(txt).val();
    // Eliminamos posibles tildes para búsqueda ampliada
    cadena = cadena.normalize('NFD').replace(/[\u0300-\u036f]/g, "");
    var itemsListado = $('#' + panDesplID).find('#divSelCatLista').children('div');
    itemsListado.show();
    itemsListado.each(function () {
        //var nombreCat = $(this).find('span label').text();
        // Cambia al ser nuevo front - Nuevo estilo de cada item de categorías        
        var nombreCat = $(this).find('div div label').text();
        // Eliminamos posibles tildes para búsqueda ampliada
        nombreCat = nombreCat.normalize('NFD').replace(/[\u0300-\u036f]/g, "");
        if (nombreCat.toLowerCase().indexOf(cadena.toLowerCase()) < 0) {
            $(this).hide();
        }
    });
}

/**
 * Método para filtrar elementos. En este caso, en la lista de Categorías, modo "Árbol" de la ficha Recurso.
 * Al teclear en el input, filtrará (ocultará) los elementos que no correspondan con el texto de la búsqueda
 * @param {any} txt
 * @param {any} panDesplID
 */
function MVCFiltrarListaSelCatArbol(txt, id, completion = undefined) {
    var cadena = $(txt).val();
    // Eliminamos posibles tildes para búsqueda ampliada
    cadena = cadena.normalize('NFD').replace(/[\u0300-\u036f]/g, "");
    var itemsListado = $('#' + id).find('.divTesArbol .categoria-wrap');
    itemsListado.show();
    if (cadena == '') {
        //$('.boton-desplegar').removeClass('mostrar-hijos');
    } else {
        itemsListado.each(function () {
            var boton = $(this).find('.boton-desplegar');
            boton.removeClass('mostrar-hijos');
            var nombreCat = $(this).find('.categoria label').text();
            // Eliminamos posibles tildes para búsqueda ampliada
            nombreCat = nombreCat.normalize('NFD').replace(/[\u0300-\u036f]/g, "");
            boton.trigger('click');
            if (nombreCat.toLowerCase().indexOf(cadena.toLowerCase()) < 0) {
                $(this).hide();
            }

            var categoriaHijo = $(this).find('.panHijos').children('.categoria-wrap');
            categoriaHijo.each(function () {
                var nombreCatHijo = $(this).find('.categoria label').text().normalize('NFD').replace(/[\u0300-\u036f]/g, "");
                if (nombreCatHijo.toLowerCase().indexOf(cadena.toLowerCase()) < 0) {
                    $(this).hide();
                }
            });
        });
    }
    if (completion != undefined) {
        completion();
    }
}

/**
 * Función que filtrará (dejará visibles u ocultará) items en base a la búsqueda introducida
 * @param {any} txt: Será el input text en formato jquery donde se introduce la cadena de búsqueda
 * @param {any} panelId: Id del panel donde se han de buscar los items
 * @param {any} classItem: Clase del item que será ocultado o mostrado según la búsqueda
 * @param {any} classItemWhereToFind: Clases donde se ha de buscar lo introducido en el txt
 */
function MVCFiltrarItems(txt, panelId, classItem, classItemWhereToFind) {
    // Cadena de texto a buscar (minúsculas)
    var cadena = $(txt).val().toLowerCase();
    // Eliminamos posibles tildes para búsqueda ampliada
    cadena = cadena.normalize('NFD').replace(/[\u0300-\u036f]/g, "");
    // Items a buscar
    var itemsListado = $('#' + panelId).find(`.${classItem}`);

    itemsListado.each(function () {
        // Texto de cada item a buscar para poder comparar
        var nombre = $(this).find(`.${classItemWhereToFind}`).text().toLowerCase();
        // Eliminamos posibles tildes para búsqueda ampliada
        nombre = nombre.normalize('NFD').replace(/[\u0300-\u036f]/g, "");
        // Filtrar -> Ocultar/Mostrar
        if (nombre.indexOf(cadena) < 0) {
            $(this).hide();
        } else {
            $(this).show();
        }
    });
}

/**
 * Marcar cada uno de los checkbox que haya en un determinado panel
 * @param {any} pCheck: checks que se buscarán (input)
 * @param {any} panDesplID: el panel donde se buscarán los checks
 * @param {any} hackedInputId: opcional. El input donde se añadirán los checks
 */
function MVCMarcarTodosElementosCat(pCheck, panDesplID, hackedInputId = undefined) {
    // items seleccionados
    var txtSeleccionados = "";

    var itemsListado = $('#' + panDesplID).find('#divSelCatLista').children('div');

    var marcarChecks = $(pCheck).is(':checked');

    // Input donde se guardarán los input seleccionados/desmarcados
    var inputTxtSeleccionados = '';

    // Observar un panel u otro
    if (hackedInputId == undefined) {
        inputTxtSeleccionados = $('#' + panDesplID).find('#txtSeleccionados');
    } else {
        inputTxtSeleccionados = $('#' + panDesplID).find(`#${hackedInputId}`);
    }


    itemsListado.each(function () {
        //var claseInput = $(this).find('span').attr('class');
        const claseInput = $(this).find('input').attr('data-item');

        /*
        $('#' + panDesplID).find('#divSelCatTesauro').find('span.' + claseInput + ' input').prop('checked', marcarChecks);
        $('#' + panDesplID).find('#divSelCatLista').find('span.' + claseInput + ' input').prop('checked', marcarChecks);
        */
        $('#' + panDesplID).find('#divSelCatTesauro').find(`[data-item=${claseInput}]`).prop('checked', $(pCheck).is(':checked'));
        $('#' + panDesplID).find('#divSelCatLista').find(`[data-item=${claseInput}]`).prop('checked', $(pCheck).is(':checked'));

        if (marcarChecks) {
            txtSeleccionados += claseInput + ",";
        }
    });

    //$('#' + panDesplID).find(inputTxtSeleccionados).val(txtSeleccionados);
    inputTxtSeleccionados.val(txtSeleccionados);
}

/**
 * Acción que se ejecuta cuando se selecciona un item de categoría para ser seleccionado e introducido en un input vacío para así tener control sobre el elemento que se ha seleccionado.
 * @param {any} pCheck: Será el input check que se ha seleccionado.
 * @param {any} panDesplID: El id del panel donde se encontrará el input vacío.
 * @param {any} hackedInputId: El id del inputId que estará oculto que se utilizará para establecer opciones que puedan servir para mandar al servidor. Si no se pasa nada, se hará caso al panDesplID. En caso contrario, se accederá al panDesplIDSecundario para buscar ese input
 */
function MVCMarcarElementoSelCat(pCheck, panDesplID, hackedInputId = undefined) {
    // Debido al nuevo Front - No se accede al padre sino al ID del propio Input
    //var claseInput = $(pCheck).parent().attr("class");
    var claseInput = $(pCheck).attr("data-item");

    var txtSeleccionados = '';

    // Observar un panel u otro
    if (hackedInputId == undefined) {
        txtSeleccionados = $('#' + panDesplID).find('#txtSeleccionados');
    } else {
        txtSeleccionados = $('#' + panDesplID).find('#' + hackedInputId);
    }

    if ($(pCheck).is(':checked')) {
        txtSeleccionados.val(txtSeleccionados.val() + claseInput + ',');
    }
    else {
        txtSeleccionados.val(txtSeleccionados.val().replace(claseInput + ',', ''));
    }

    /*
     * $('#' + panDesplID).find('#divSelCatTesauro').find('span.' + claseInput + ' input').prop('checked', $(pCheck).is(':checked'));
    $('#' + panDesplID).find('#divSelCatLista').find('span.' + claseInput + ' input').prop('checked', $(pCheck).is(':checked'));
    */

    $('#' + panDesplID).find('#divSelCatTesauro').find(`[data-item=${claseInput}]`).prop('checked', $(pCheck).is(':checked'));
    $('#' + panDesplID).find('#divSelCatLista').find(`[data-item=${claseInput}]`).prop('checked', $(pCheck).is(':checked'));

    MVCComprobarChecks(panDesplID);
}

/**
 * Método para comprobar si se han chequeado todos los checks
 * @param {any} panDesplID
 */
function MVCComprobarChecks(panDesplID) {    
    const itemsListado = $('#' + panDesplID).find('#divSelCatLista').children('div').find("input");    
    const numItemsChecked = $('#' + panDesplID).find('#divSelCatLista').children('div').find("input:checked").length;

    if (numItemsChecked == itemsListado.length) {
        $('#chkSeleccionarTodos').prop('checked', true);
    }
    else {
        $('#chkSeleccionarTodos').prop('checked', false);
    }
}
/*Fin Tesauros*/
var inicializadoSubirRecurso = false;

/**
 * Este método se ejecuta cuando finaliza la carga de los recursos (por ejemplo, los elementos
 * de un listado).
 * Aunque no está inicializado, en cada comunidad/proyecto, se puede definir la función `CompletadaCargaRecursosComunidad()`
 * para extender o modificar el comportamiento por defecto tras la carga de recursos.
 */
function CompletadaCargaRecursos() {
    if (typeof (window.CompletadaCargaRecursosComunidad) == 'function') {
        CompletadaCargaRecursosComunidad();
    }
}

/**
 * Este método se ejecuta cuando finaliza la carga de las facetas 
 * Aunque no está inicializado por defecto, en cada comunidad/proyecto, se puede definir la función `CompletadaCargaFacetas()`
 * para extender o modificar el comportamiento por defecto tras la carga de recursos.
 */
function CompletadaCargaFacetas() {
    if (typeof (window.comportamientoCargaFacetasComunidad) == 'function') {
        comportamientoCargaFacetasComunidad();
    }
}

/**
 * Inicializa los eventos para los botones de una interfaz de carga de recursos externos. Inicializa los eventos para subir recursos externos con una URL base específica
 * Configura acciones para cada botón, incluyendo mostrar un progreso de actualización y manejar solicitudes AJAX.
 * @param {string} urlPaginaSubir - La URL base para las solicitudes AJAX relacionadas con la carga de recursos.
 * @returns {void}
 */
function InicializarSubirRecursoExt(urlPaginaSubir) {
    if (inicializadoSubirRecurso) { return; }

    $("#linkNota").click(function () {
        MostrarUpdateProgress();

        GnossPeticionAjax(urlPaginaSubir + '/selectresource', { TypeResourceSelected: 0 }, true).always(function () {
            OcultarUpdateProgress();
        });
    });

    $("#lbSiguienteURL").click(function () {
        validarUrlExt(urlPaginaSubir, false);
    });

    $("#lbSiguienteReferencia").click(function () { validarDocFisicoExt(urlPaginaSubir, false); });

    $("#lbSiguienteWiki").click(function () {
        var url = document.getElementById("txtArticuloWiki").value;

        if (url == '') {
            return false;
        }

        MostrarUpdateProgress();

        GnossPeticionAjax(urlPaginaSubir + '/selectresource', { TypeResourceSelected: 3, Link: url }, true).fail(function (data) {
            $('#pWikiError').remove();
            $('#divWiki fieldset').append('<div id="pWikiError" class="ko" style="display:block;"><p>' + data + '</p></div>');
        }).always(function () {
            OcultarUpdateProgress();
        });
    });

    $("#lbSiguienteArchivo").click(function () { validarDocAdjuntarExt(urlPaginaSubir, false, null); });

    inicializadoSubirRecurso = true;
}

/**
 * Muestra un panel de carga de recursos basado en el nombre del recurso especificado.
 * Oculta todos los otros paneles de recursos y configura las fuentes para los iframes si es necesario.
 * @param {string} nombre - El nombre del tipo de recurso a mostrar. Puede ser 'Archivo', 'Referencia', 'URL', 'Brightcove', 'TOP', o 'Wiki'.
 * @returns {void}
 */
function mostrarPanSubirRecurso(nombre) {

    if (typeof (InicioMostrarPanSubirRecurso) != "undefined") {
        InicioMostrarPanSubirRecurso(nombre);
    }

    if (document.getElementById('panEnlRep') != null) {
        $('#panEnlRep').remove();
    }

    if (document.getElementById("divArchivo") != null) {
        document.getElementById("divArchivo").style.display = "none";
    }
    if (document.getElementById("divReferenciaDoc") != null) {
        document.getElementById("divReferenciaDoc").style.display = "none";
    }
    if (document.getElementById("divURL") != null) {
        document.getElementById("divURL").style.display = "none";
    }
    if (document.getElementById("divBrightcove") != null) {
        document.getElementById("divBrightcove").style.display = "none";
    }
    if (document.getElementById("divTOP") != null) {
        document.getElementById("divTOP").style.display = "none";
    }
    if (document.getElementById("divWiki") != null) {
        document.getElementById("divWiki").style.display = "none";
    }

    switch (nombre) {
        case 'Archivo':
            document.getElementById("divArchivo").style.display = "block";
            break;
        case 'Referencia':
            document.getElementById("divReferenciaDoc").style.display = "block";
            break;
        case 'URL':
            document.getElementById("divURL").style.display = "block";
            break;
        case 'Brightcove':
            document.getElementById("divBrightcove").style.display = "block";
            var srcAux = $('#iframeBrightcove').attr('srcAux');
            var onloadAux = $('#iframeBrightcove').attr('onloadAux');
            if (srcAux != null && srcAux != "") {
                $('#iframeBrightcove').attr('src', srcAux);
                $('#iframeBrightcove').removeAttr('srcAux');
            }
            if (onloadAux != null && onloadAux != "") {
                $('#iframeBrightcove').attr('onload', onloadAux);
                $('#iframeBrightcove').removeAttr('onloadAux');
            }
            break;
        case 'TOP':
            document.getElementById("divTOP").style.display = "block";
            var srcAux = $('#iframeTOP').attr('srcAux');
            var onloadAux = $('#iframeTOP').attr('onloadAux');
            if (srcAux != null && srcAux != "") {
                $('#iframeTOP').attr('src', srcAux);
                $('#iframeTOP').removeAttr('srcAux');
            }
            if (onloadAux != null && onloadAux != "") {
                $('#iframeTOP').attr('onload', onloadAux);
                $('#iframeTOP').removeAttr('onloadAux');
            }
            break;
        case 'Wiki':
            document.getElementById("divWiki").style.display = "block";
            break;
    }
}

/**
 * /**
 * Acción que se ejecuta para comprobar que el Link adjunto es correcto. Es el paso previo que se realiza antes de poder crear un recurso de tipo "Enlace externo" 
 * @param {any} urlPaginaSubir: Url o enlace escrito por el usuario
 * @param {any} omitirCompRep
 */
function validarUrlExt(urlPaginaSubir, omitirCompRep) {
    try //Intentamos validar la url
    {
        var lblUrl = document.getElementById("lblIntroducirURL");
        var url = document.getElementById("txtURLDoc");

        // Panel donde se mostrará posibles errores en la subida del un recurso de tipo Enlace Externo (Nuevo Front)
        const panelResourceFileErrorMessage = $('#modal-add-resource-link-messages-wrapper .ko');
        // Vaciar el panel de posibles errores anteriores y ocultarlo
        panelResourceFileErrorMessage.empty().hide();

        var regexURL = /^(http(s)?:\/\/.)[-a-zA-Z0-9@:%.\/_\+~#=]{2,256}\.[a-z]{2,6}\b([-a-zA-Z0-9@:%_\+.~#?&\/\/=]*)$/i;
        if (url.value.length > 0 && url.value.match(regexURL)) {
            let enlaceASubir = url.value;
            if (enlaceASubir.includes("riamlab.sharepoint.com") || enlaceASubir.includes("riamlab-my.sharepoint.com")) {
                var oneDrive = $("#inpt_oneDrivePermitido").val();
                if (enlaceASubir.includes("riamlab-my.sharepoint.com") && oneDrive == "False") {
                    MostrarUpdateProgress();
                    GnossPeticionAjax(urlPaginaSubir + '/selectresource', { TypeResourceSelected: 1, Link: url.value, SkipRepeat: omitirCompRep }, true).done(function (data) {
                        panelResourceFileErrorMessage.append(data).show();
                    }).fail(function (data) {
                        document.getElementById("lblIntroducirURL").style.color = "Red";
                    }).always(function () {
                        OcultarUpdateProgress();
                    });

                    return true;
                } else {
                    let urlDestino = $("#inpt_baseUrlBusqueda").val();
                    urlDestino = urlDestino + "/comprobar-token-sp?pLink=" + enlaceASubir + "&pTypeResourceSelected=1&pSkipRepeat=" + omitirCompRep + "&pUrlPaginaSubir=" + urlPaginaSubir;
                    window.location.href = urlDestino;
                }
            } else {
                MostrarUpdateProgress();
                GnossPeticionAjax(urlPaginaSubir + '/selectresource', { TypeResourceSelected: 1, Link: url.value, SkipRepeat: omitirCompRep }, true).done(function (data) {
                    // No ocultarlo para reintentar de nuevo
                    //$('#divURL').hide();
                    // Cambio por nuevo Front
                    //$('#liURL').append(data);                
                    // Añadir el mensaje y mostrarlo
                    panelResourceFileErrorMessage.append(data).show();

                }).fail(function (data) {
                    document.getElementById("lblIntroducirURL").style.color = "Red";
                }).always(function () {
                    OcultarUpdateProgress();
                });

                return true;
            }
        }
        else {
            lblUrl.style.color = "Red";
            return false;
        }
    } catch (e) {
        //Error provocado porque no existe el elemento url (no es un recurso con url)
    }
    return true;
}

/**
 * Valida la ubicación del documento físico y realiza una solicitud AJAX para agregar el recurso. 
 * @param {string} urlPaginaSubir - URL del servicio para subir recursos.
 * @param {boolean} omitirCompRep - Indica si se debe omitir la comprobación de documentos repetidos.
 * @returns {boolean} - Retorna `true` si el documento es válido, `false` en caso contrario.
 */
function validarDocFisicoExt(urlPaginaSubir, omitirCompRep) {
    try
    {
        var doc = document.getElementById("txtUbicacionDoc");

        if (doc.value.length > 0) {
            MostrarUpdateProgress();

            GnossPeticionAjax(urlPaginaSubir + '/selectresource', { TypeResourceSelected: 2, Link: doc.value, SkipRepeat: omitirCompRep }, true).done(function (data) {
                $('#divReferenciaDoc').hide();
                $('#liReferenciaDoc').append(data);
            }).fail(function (data) {
                document.getElementById("lblDescribaUbic").style.color = "Red";
            }).always(function () {
                OcultarUpdateProgress();
            });

            return true;
        } else {
            document.getElementById("lblDescribaUbic").style.color = 'Red';
            return false;
        }
    } catch (e) {
        //Error porque no existe el elemento doc (no es un elemento físico)
    }
}

/**
 * Acción que se ejecuta para comprobar que el fichero adjunto es correcto. Es el paso previo que se realiza antes de poder crear un recurso de tipo "Adjunto"
 * @param {any} urlPaginaSubir: Página a la que se redireccionará para completar la creación del recurso de tipo "Adjunto"
 * @param {Boolean} omitirCompRep
 * @param {Boolean} extraArchivo
 */
function validarDocAdjuntarExt(urlPaginaSubir, omitirCompRep, extraArchivo) {
    try
    {
        var lblDoc = document.getElementById("lblSelecionaUnDoc");
        var doc = document.getElementById("fuExaminar");
        // Panel donde se mostrará posibles errores en la subida del archivo (Nuevo Front)
        const panelResourceFileErrorMessage = $('#modal-add-resource-file-messages-wrapper .ko');
        // Vaciar el panel de posibles errores anteriores y ocultarlo
        panelResourceFileErrorMessage.empty().hide();

        if (omitirCompRep) {
            MostrarUpdateProgress();

            GnossPeticionAjax(urlPaginaSubir + '/selectresource', { TypeResourceSelected: 4, ExtraFile: extraArchivo, SkipRepeat: omitirCompRep }, true).fail(function (data) {
                document.getElementById("lblDescribaUbic").style.color = "Red";
            }).always(function () {
                OcultarUpdateProgress();
            });
        }
        else if (doc.value.length > 0) {
            var data = new FormData();
            var files = $("#fuExaminar").get(0).files;
            if (files.length > 0) {
                MostrarUpdateProgressTime(0);

                var bar = $('#progressBarArchivo .bar');
                var percent = $('#progressBarArchivo .percent');

                var percentVal = '0%';
                bar.width(percentVal);
                percent.html(percentVal);

                $('#lbSiguienteArchivo').hide();
                $('#progressBarArchivo').show();

                data.append("TypeResourceSelected", 4);
                data.append("File", files[0]);
                data.append("FileName", files[0].name);

                GnossPeticionAjax(urlPaginaSubir + '/selectresource', data, true).done(function (data) {
                    // No ocultar nada para reitentar el envío o subida de un fichero
                    //$('#divArchivo').hide();
                    // Cambio por nuevo Front
                    // $('#liArchivo').append(data);
                    // Añado el error y muestro el div
                    panelResourceFileErrorMessage.append(data).show();
                    // Mostrar de nuevo botón de "Siguiente" para reintentar envío de fichero
                    $('#lbSiguienteArchivo').show();

                }).fail(function (data) {
                    document.getElementById("lblSelecionaUnDoc").style.color = "Red";
                    $('#pArchError').remove();
                    var mensajeError = data;
                    if (data == "NETWORKERROR") {
                        mensajeError = 'Has perdido la conexión. Comprueba tu conexión a internet e intenta adjuntar el recurso de nuevo.';
                    }
                    // Cambiado por nuevo front
                    // $('#divArchivo fieldset').append('<div id="pArchError" class="ko" style="display:block;"><p>' + mensajeError + '</p></div>');
                    panelResourceFileErrorMessage.append('<p>' + mensajeError + '</p>').show();
                    $('#lbSiguienteArchivo').show();
                    $('#progressBarArchivo').hide();
                }).progress(function (progreso) {
                    var percentVal = progreso + '%';
                    bar.width(percentVal);
                    percent.html(percentVal);
                }).always(function () {
                    OcultarUpdateProgress();
                });
            }
            return true;
        } else {
            lblDoc.style.color = 'Red';
            return false;
        }
    } catch (e) {
        //Error provado porque no existe el elemento doc (no es un recurso nuevo archivo)
    }
}

/**/ 
/*MVC.ComAdmin.js*/ 
/**
 * Habilita o deshabilita campos de entrada de archivos en un formulario de subida dependiendo del tipo de archivo seleccionado.
 * Actualiza el valor del campo `tipo_archivo` basado en la selección del usuario entre `js/css` o `zip`.
 */
function FormularioSubidaEstilos() {
    if (document.getElementById('js/css').checked) {
        document.getElementById('archivo_js').disabled = false;
        document.getElementById('archivo_css').disabled = false;
        document.getElementById('archivo_zip').disabled = true;
        document.getElementById('tipo_archivo').value = 'js_css';
    }
    else if (document.getElementById('zip').checked) {
        document.getElementById('archivo_js').disabled = true;
        document.getElementById('archivo_css').disabled = true;
        document.getElementById('archivo_zip').disabled = false;
        document.getElementById('tipo_archivo').value = 'zip';
    }
}

/**
 * Operativa para hacer Zoom en las imágenes. Sólo se ejecutará si existe la librería instalada
 */
const operativaImagesZooming = {

    /**
     * Inicializar operativa
     */
    init: function (pParams) {
        this.pParams = pParams;
        this.config(pParams);
        this.configEvents();
        this.triggerEvents();
    },

    /*
     * Inicializar elementos de la vista
     * */
    config: function (pParams) {
        // Elementos del DOM

        /* Nombre de la clase a las que se aplicará el comportamiento de imágenes en Zoom */
        this.imageZoomingClassName = "zooming";
    },

    /**
     * Configuración de eventos de elementos del Dom (Botones, Inputs...)     
     */
    configEvents: function (pParams) {

    },

    /**
     * Lanzar comportamientos u operativas necesarias para el funcionamiento de la sección
     */
    triggerEvents: function () {
        const that = this;

        // Ejecutar operativa si existe la librería zooming
        if (typeof Zooming !== 'undefined') {
            that.setupZoomImagesImages();
        }
    },

    setupObserverForImageZooming: function (classNames, callback) {
        // Función recursiva para buscar elementos con las clases deseadas
        function buscarElementos(node) {
            if (node.nodeType === Node.ELEMENT_NODE) {
                classNames.forEach(function (className) {
                    if ($(node).hasClass(className)) {
                        callback($(node));
                    }
                });
            }
            node.childNodes.forEach(buscarElementos);
        }

        // Ejecutar la búsqueda en el documento completo
        buscarElementos(document.body);

        // Configurar el MutationObserver para observar cambios en el DOM
        let observer = new MutationObserver(function (mutations) {
            mutations.forEach(function (mutation) {
                mutation.addedNodes.forEach(function (node) {
                    buscarElementos(node);
                });
            });
        });
        observer.observe(document.body, { childList: true, subtree: true });
    },

    /**
     * Método que revisará las imágenes y las prepara para tengan la opción del "Zoom" haciendo uso de la librería "zooming"
     */
    setupZoomImagesImages: function () {
        const that = this;

        // Configurar el disparador para la faceta
        that.setupObserverForImageZooming([`${that.imageZoomingClassName}`], function (element) {
            new Zooming().listen(`.${that.imageZoomingClassName}`);
        });
    },
}

/**
 * Operariva que permite cargar mas comentarios de un recurso
 */
const operativaCargarMasComentarios = {
    /**
     * Acción para inicializar elementos y eventos
     * @param {any} pParams
     */
    init: function (pParams) {
        this.config(pParams);
        this.configEvents(pParams);
    },

    /**
     * Opciones de configuración de la vista (recoger ids para poder interactuar)
     * @param {any} pParams
     */
    config: function (pParams) {

        // Inicialización de variables
        this.btnLoadMoreComments = $(`.${pParams.config.btnLoadMoreCommentsClassName}`);
 	    this.urlLoadMoreComments = pParams.config.urlLoadMoreComments;
        this.panComentariosIdName = pParams.config.panComentariosIdName;
        this.comentarioClassName = pParams.config.comentarioClassName;
        this.loadingMoreResultClassName = pParams.config.loadingMoreResultClassName;
        this.numComentariosIdName = pParams.config.numComentariosIdName;
    },
    /**
     * Configuración de los eventos de los items html
     */
    configEvents: function (pParams) {
        const that = this;

        this.btnLoadMoreComments.off().on("click", function () {
                that.loadMoreComments();
            });
    },
    loadMoreComments: function () {
        const that = this;

        let inicio = $(`#${that.panComentariosIdName} .${that.comentarioClassName}`).length;
        if(inicio == undefined) inicio = 0;

        // Mostrado de loading
        $(`.${that.loadingMoreResultClassName}`).removeClass("d-none");

        let dataPost = {
            pInicio: inicio
        }

        this.btnLoadMoreComments.prop("disabled", true);

        // Realizar la petición para actualizar el campo
        GnossPeticionAjax(
            that.urlLoadMoreComments,
            dataPost,
            true
        ).done(function (data) {
            data.$values.forEach(function (item){
                $(`#${that.panComentariosIdName}`).append(item.html);
            });

            if($(`#${that.panComentariosIdName} .${that.comentarioClassName}`).length == $(`#${that.numComentariosIdName}`).text()){
                that.btnLoadMoreComments.addClass("d-none");
            }
        }).fail(function (data) {
            mostrarNotificacion("error",data);
        }).always(function () {
            // Ocultar loading
            that.btnLoadMoreComments.prop("disabled", false);
            $('.loading-more-results').addClass("d-none");
        })
    },
}

/**
 * Operativa para el comportamiento de la pagina "Mis Comunidades"
 */
const operativaMisComunidades = {
    
    init: function () {
        this.configEvents();
    },

    /**
     * Asignamos los eventos a los items del drop down
     */
    configEvents: function () {
        const that = this;

        $($("#panel-orderBy a.item-dropdown.my-community")).on("click", function () {
            that.orderMyCommunities(this);
        });
    },

    /**
     * Ordenamos segun el item seleccionado
     */
    orderMyCommunities: function (pItem) {
        let item = $(pItem);
        let orderBy = item.data("orderby");
        let order = item.data("order");

        let communityList = $(".resource-list-wrap .card-community").get();
        let campo = "";
        let isDate = false;
        switch(orderBy){
            case "foaf:firstName":
                campo = "name";
                break;
            case "gnoss:hasnumerorecursos":
                campo = "resources";
                break;
            case "gnoss:hasparticipanteIdentidadID":
                campo = "persons";
                break;
            case "gnoss:hasfechaAlta":
                campo = "creationdate";
                isDate = true;
                break;
        }

        communityList.sort(function(a,b){
            
            let item1 = $(a).find(`.${campo}`).data(campo);
            let item2 = $(b).find(`.${campo}`).data(campo);

            if(campo === "name"){
                item1 = item1.toLowerCase();
                item2 = item2.toLowerCase();
            }

            if(isDate){
                item1 = item1.split('/').reverse().join(),
                item2 = item2.split('/').reverse().join();
            }

            if(order.includes("asc")){
                return item1 > item2 ? 1 : (item1 < item2) ? -1 : 0;
            }else{
                return item1 < item2 ? 1 : (item1 > item2) ? -1 : 0;
            }
        });

        $(".resource-list-wrap").empty().append(communityList);
        
    }  
}

/**
 * Renderiza un elemento de la lista de resultados del servicio de autocomplete.
 * 
 * @param {Object} item - Objeto que representa el ítem a renderizar, típicamente contiene datos como imagen, título, peso, etc.
 * @param {string} term - El término de búsqueda que originó los resultados de autocomplete.
 * @returns {string} - HTML string que representa el ítem renderizado para ser insertado en el DOM.
 */
function autocompleteFillListItem(item, term) {

    // Comprobación de versión del customizada para el proyecto
    if (typeof autocompleteCustomFillListItem === 'function') {
        return autocompleteCustomFillListItem(item, term);
    } else {
        // Comprobar si la imagen es válida o no
        const imageUrl = item.data.imagen ? item.data.imagen : '';
        const imagePlaceholder = imageUrl
            ? `<img src="${imageUrl}" alt="${item.data.titulo}" class="autocomplete__result-item-img">`
            : item.data.textoBuscableFaceta
            ? `<span class="material-icons autocomplete__result-item-placeholder">sort</span>`
            : `<span class="material-icons autocomplete__result-item-placeholder">search</span>`;

        // Comprobar si la URL y otros datos existen
        const subtipoData = item.data.subtipo ? item.data.subtipo : '';
        const url = item.data.urlRecurso ? item.data.urlRecurso : item.data.textoBuscableFaceta ? window.location.href.split('?')[0] + '?' + item.data.textoBuscableFaceta : '';
        item.data.urlRecurso = url;
        return `
              <div class="autocomplete__result-item">
                <div class="autocomplete__result-item-image">
                  ${imagePlaceholder}
                </div>
                <div class="autocomplete__result-item-content">
                  <div class="autocomplete__result-item-title">${item.data.titulo}</div>
                  <div class="autocomplete__result-item-subtype">${subtipoData}</div>
                </div>
              </div>
        `;
    }
}


/**
 * Inicializa un selector de fecha y hora en el elemento especificado con las opciones dadas.
 *
 * Esta función limpia cualquier inicialización previa del selector de fecha y hora en el
 * selector especificado y configura un nuevo selector de fecha y hora con las opciones proporcionadas,
 * fusionadas con la configuración predeterminada.
 *
 * @param {string} selector - El selector de jQuery que identifica el(los) elemento(s)
 *                            a los que se les adjuntará el selector de fecha y hora.
 * @param {Object} [options={}] - Configuraciones opcionales para el selector de fecha y hora.
 * @param {boolean} [options.enableTime=false] - Indica si se debe habilitar el selector de hora.
 * @param {string} [options.defaultTime='12:00'] - La hora predeterminada si se habilita el selector de hora.
 * @param {number} [options.yearStart] - El año inicial en el rango de años.
 * @param {number} [options.yearEnd] - El año final en el rango de años.
 * @param {string} [options.lang='en'] - El idioma para el selector de fecha y hora, por defecto toma el valor de '#inpt_Idioma' o 'en'.
 * @param {number} [options.dayOfWeekStart=1] - El día de inicio de la semana (0 para domingo, 1 para lunes).
 * @param {function} [options.onSelectDate=() => {}] - Función de devolución de llamada cuando se selecciona una fecha.
 * @param {function} [options.onSelectTime=() => {}] - Función de devolución de llamada cuando se selecciona una hora.
 * *****************************************************
 * Ejemplo de uso
 * 
    initCustomDatePicker('.calenFormSem', {
        yearStart: 2000,
        yearEnd: 2030,
        enableTime: true, // Activa el configuración para la "Hora"
    });
 */
function initCustomDatePicker(selector, options = {}) {
    if (typeof $.datetimepicker === 'undefined') {
        console.warn("Datetimepicker no está cargado. No se puede inicializar.");
        return;
    }

    // Limpiar cualquier rastro de inicializaciones previas
    $(selector).off();
    $(selector).removeClass('xdsoft_input');
    $(selector).removeData('xdsoft_datetimepicker');

    const defaultYearStart = new Date().getFullYear() - 110; // Hace 110 años a partir del año actual
    const defaultYearEnd = new Date(new Date().setFullYear(new Date().getFullYear() + 100)).getFullYear(); // Dentro de 100 años a partir del año actual    

    // Si no se pasan valores, se asignan los valores por defecto
    const yearStart = options.yearStart !== undefined ? options.yearStart : defaultYearStart;
    const yearEnd = options.yearEnd !== undefined ? options.yearEnd : defaultYearEnd;

    // Valores predeterminados
    const defaultOptions = {
        format: options.enableTime ? 'd/m/Y H:i:s' : 'd/m/Y', // Añade hora solo si está habilitada
        timepicker: options.enableTime || false, // Activa o desactiva el selector de hora
        datepicker: true, // Mantiene el selector de fecha activo
        defaultTime: options.enableTime ? '12:00' : false, // Hora predeterminada solo si está habilitada
        yearStart: yearStart !== null ? yearStart : undefined, // Usar undefined si no hay valor
        yearEnd: yearEnd !== null ? yearEnd : undefined, // Usar undefined si no hay valor        
        dayOfWeekStart: 1, // Comienza la semana en lunes
        onSelectDate: () => {
            $(selector).datetimepicker('hide');
        },
        onSelectTime: () => {
            $(selector).datetimepicker('hide');
        },
    };

    // Combina los valores predeterminados con los proporcionados
    const config = {
        ...defaultOptions,
        ...options, // Sobrescribe los valores predeterminados con las opciones del usuario
    };
    
    // Idioma del datepicker
    $.datetimepicker.setLocale($('#inpt_Idioma').val() || 'en');
    // Retrasar la inicialización para poder "destruir" posibles comportamientos asociados previos
    setTimeout(() => {
        $(selector).datetimepicker(config);        
    }, 3000);
}