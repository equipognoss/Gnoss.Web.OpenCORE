/**
 * Operativa de funcionamiento de Tipo de Contenidos y Permisos
 */
const operativaGestionMatomo = {

    /**
     * Inicializar operativa
     */
    init: function (pParams) {
        this.pParams = pParams;
        this.config(pParams);
        this.configRutas(pParams);
        this.configEvents();
        this.triggerEvents();
    },

    /**
     * Lanzar comportamientos u operativas necesarias para el funcionamiento de la sección
     */
    triggerEvents: function () {
        const that = this;

        // Carga inicial de datos
        that.chargeTablesAndGraphs();
    },

    /**
     * Configuración de las rutas de acciones necesarias para hacer peticiones a backend
     */
    configRutas: function (pParams) {
        // Url para editar un certificado
        this.urlBase = refineURL();
    },

    /*
     * Inicializar elementos de la vista
     * */
    config: function (pParams) {
        const that = this;

        this.txtBuscarUsuariosId = "txtBuscarUsuarios";
        this.txtBuscarUsuariosHackId = "txtBuscarUsuarios_Hack";
        this.txtBuscarUsuarios = $(`#${that.txtBuscarUsuariosId}`);
        this.txtBuscarUsuariosHack = $(`#${that.txtBuscarUsuariosHackId}`);
        this.inputFechaInicioId = "fechaInicio";
        this.inputFechaInicio = $(`#${this.inputFechaInicioId}`);
        this.inputFechaFinId = "fechaFinal";
        this.inputFechaFin = $(`#${this.inputFechaFinId}`);
        this.inputProyectoId = $("#inpt_proyID");

        this.inputLupaId = "inputLupa";
        this.panFiltrosId = "panFiltros"
        this.panFiltros = $(`#${this.panFiltrosId}`);
        this.panListadoFiltros = $("#panListadoFiltros");
        this.removeUserFilterClassName = "removeUserFilter";

        //Tabla: Resumen de Visitas
        this.lastMonthThVisits = $("#lastMonthThVisits");
        this.last2MonthThVisits = $("#last2MonthThVisits");
        this.last3MonthThVisits = $("#last3MonthThVisits");
        this.lastYearThVisits = $("#lastYearThVisits");
        this.last7ThVisits = $("#last7ThVisits");
        this.yesterdayThVisits = $("#yesterdayThVisits");
        this.todayThVisits = $("#todayThVisits");
        this.lastMonthThUsers = $("#lastMonthThUsers");
        this.last2MonthThUsers = $("#last2MonthThUsers");
        this.last3MonthThUsers = $("#last3MonthThUsers");
        this.lastYearThUsers = $("#lastYearThUsers");
        this.last7ThUsers = $("#last7ThUsers");
        this.yesterdayThUsers = $("#yesterdayThUsers");
        this.todayThUsers = $("#todayThUsers");
        this.tableLastVisitors = $("#table_last_visitors");
        // Botón descarga en CSV Datos de Recursos
        this.parellelBtId = "parellelBt";
        this.parellelBt = $(`#${this.parellelBtId}`);
        this.parellelBtDownloadsId = "parellelBtDownloads";
        this.parellelBtDownloads = $(`#${this.parellelBtDownloadsId}`);
        this.parellelBtVotesId = "parellelBtVotes";
        this.parellelBtVotes = $(`#${this.parellelBtVotesId}`);
        this.parellelBtCommentsId = "parellelBtComments";
        this.parellelBtComments = $(`#${this.parellelBtCommentsId}`);
        this.parellelBtPagesId = "parellelBtPages";
        this.parellelBtPages = $(`#${this.parellelBtPagesId}`);
        this.parellelBtResourcesId = "parellelBtResources";
        this.parellelBtResources = $(`#${this.parellelBtResourcesId}`)

        // Variables con los datos a pintar utilizados en construcción de ficheros CSV
        this.recursosDownload = new Object();
        this.recursosDownloadName = new Object(); //Pair id-name
        this.recursosDownloadURL = new Object();
        this.recursos = new Object();
        this.recursosDownloadNameVoted = new Object(); //Pair id-name
        this.recursosDownloadURLVoted = new Object(); //Pair id-url		
        this.recursosComent = new Object();
        this.recursosComentName = new Object(); //Pair id-name
        this.recursosComentURL = new Object(); //Pair id-url			
        this.recursosName = new Object(); // Par idRecurso - pageTitle
        this.recursosUrl = new Object(); // Par idRecurso - url
        this.recursosVisitas = new Object(); // Par idRecurso - visitas
        this.recursosTiempoTotal = new Object(); // Par idRecurso - time_spent
        this.recursosTiempoMedio = new Object(); // Par idRecurso - tMedio
        this.paginasUrl = new Object(); // Par url - pageTitle
        this.paginasVisitas = new Object(); // Par url - visitas
        this.paginasTiempoTotal = new Object(); // Par url - time_spent
        this.paginasTiempoMedio = new Object(); // Par url - tMedio

        this.nombrePerfilPorUsuarioId = new Object();
        this.visitasUnicasPorUsuarioId = new Object();
        this.urlPerfilPorUsuarioID = new Object();
        this.tiempoTotalPorUsuarioId = new Object();

        // Referencia de gráficos 
        this.chartIdTotalVisits = undefined;
        this.chartId1 = undefined;
        this.chartId2 = undefined;
        this.chartId3 = undefined;
        this.chartId4 = undefined;
    },

    /**
     * Configuración de eventos de elementos del Dom (Botones, Inputs...)     
     */
    configEvents: function (pParams) {
        const that = this;

        // Comportamiento autocomplete a input de selección de Grupos para Privacidad de página
        this.txtBuscarUsuarios.autocomplete(
            null,
            {
                url: $('#inpt_urlServicioAutocompletar').val() + "/AutoCompletarSeleccionUsuariosMatomo",
                delay: 0,
                scroll: false,
                max: 30,
                selectFirst: false,
                minChars: 1,
                width: 300,
                cacheLength: 0,
                NoPintarSeleccionado: true,
                multiple: true,
                extraParams: {
                    grupo: '',
                    identidad: $('#inpt_identidadID').val(),
                    organizacion: $('#inpt_organizacionID').val() == "00000000-0000-0000-0000-000000000000" ? "" : $('#inpt_organizacionID').val(),
                    proyecto: $('#inpt_proyID').val(),
                    bool_edicion: 'true',
                    bool_traergrupos: 'false',
                    bool_traerperfiles: 'true'
                }
            }
        );

        // Comportamiento resultado cuando se selecciona una resultado de autocomplete de Grupos
        this.txtBuscarUsuarios.result(function (event, data, formatted) {
            //that.aceptarEditorSelectorUsuRec(this, data[0], data[1]);
            const dataName = data[0];
            const dataId = data[1];
            const input = $(event.currentTarget);
            that.handleSelectAutocompleteItem(dataName, dataId);
        });

        // Botón/es para confirmar la eliminación de un filtrado activo de usuario       
        configEventByClassName(`${that.removeUserFilterClassName}`, function (element) {
            const $jqueryElement = $(element);
            $jqueryElement.off().on("click", function () {
                // Eliminar filtros activos
                that.handleSelectAutocompleteItem("", "", true);
            });
        });

        // Click para la descarga de CSV de "Datos de recursos"
        this.parellelBt.on("click", function () {
            const csv = that.handleDownloadCSV();

            const blob = new Blob([csv], { type: 'text/csv' });
            const url = window.URL.createObjectURL(blob)
            const a = document.createElement('a')
            a.setAttribute('href', url)
            a.setAttribute('download', 'ResourcesData.csv');
            a.click();
        });

        this.parellelBtDownloads.on("click", function () {
            const csvDownloads = that.handleResourcesDownloadsCSV();

            const blob = new Blob([csvDownloads], { type: 'text/csv' });
            const url = window.URL.createObjectURL(blob)
            const aDownloads = document.createElement('a')
            aDownloads.setAttribute('href', url)
            aDownloads.setAttribute('download', 'ResourcesDownloadsData.csv');
            aDownloads.click();
        });

        this.parellelBtVotes.on("click", function () {
            const csvVotes = that.handleResourcesVotesCSV();

            const blob = new Blob([csvVotes], { type: 'text/csv' });
            const url = window.URL.createObjectURL(blob)
            const aVotes = document.createElement('a')
            aVotes.setAttribute('href', url)
            aVotes.setAttribute('download', 'ResourcesVotesData.csv');
            aVotes.click();
        });

        this.parellelBtResources.on("click", function () {
            const csvResources = that.handleTopResourcesCSV();

            const blob = new Blob([csvResources], { type: 'text/csv' });
            const url = window.URL.createObjectURL(blob)
            const aResources = document.createElement('a')
            aResources.setAttribute('href', url)
            aResources.setAttribute('download', 'TopResourcesData.csv');
            aResources.click();
        });

        this.parellelBtComments.on("click", function () {
            const csvComments = that.handleResourcesCommentsCSV();

            const blob = new Blob([csvComments], { type: 'text/csv' });
            const url = window.URL.createObjectURL(blob)
            const aComments = document.createElement('a')
            aComments.setAttribute('href', url)
            aComments.setAttribute('download', 'ResourcesCommentsData.csv');
            aComments.click();
        });

        this.parellelBtPages.on("click", function () {
            const csvPages = that.handleDownloadCSVPages();

            const blobPages = new Blob([csvPages], { type: 'text/csv' });
            const urlPages = window.URL.createObjectURL(blobPages)
            const aPages = document.createElement('a')
            aPages.setAttribute('href', urlPages)
            aPages.setAttribute('download', 'PagesData.csv');
            aPages.click();
        });


        // ComboBox de cambio de fecha para actualización de los datos (Fecha inicio / Fecha fin)
        this.inputFechaInicio.on("change", function () {
            that.chargeTablesAndGraphs();
        });

        this.inputFechaFin.on("change", function () {
            that.chargeTablesAndGraphs();
        });
    },

    /**
     * Método para la selección realizada de un item haciendo uso de AutoComplete 
     * @param {string} input: Input que ha lanzado el autocomplete
     * @param {string} dataName: Nombre o dato nombre del usuario seleccionado del panel autoComplete     
     * @param {bool} removeFilter : Dato booleano que indicará si es necesario eliminar el filtro.
     */
    handleSelectAutocompleteItem: function (dataName, dataId, removeFilter = false) {
        const that = this;
        let filterUserValue = "";

        if (removeFilter == false) {
            // Crear y mostrar el filtro según lo seleccionado
            let usuarioSeleccionado = `
			<li>${dataName}
				<a rel="nofollow" 
					style="cursor: pointer;"
					class="remove faceta removeUserFilter" 
					data-url=${that.urlBase}>eliminar
				</a>
			</li>		
			`;
            that.panFiltros.removeClass("d-none");
            // Añadir el item en el contenedor de items para su visualización
            that.panListadoFiltros.html(usuarioSeleccionado);
            filterUserValue = dataId.trim();

            // Pintar el usuario que se utilizará para cargar los datos mediante "chargeTable/chargeGraphs"		
            that.txtBuscarUsuariosHack.val(filterUserValue);
        } else {
            // Eliminar el filtro
            that.panListadoFiltros.empty();
            // Pintar el usuario que se utilizará para cargar los datos mediante "chargeTable/chargeGraphs"		
            that.txtBuscarUsuariosHack.val("");
            this.txtBuscarUsuarios.val("");
            that.panFiltros.addClass("d-none");
        }

        // Recargar datos
        that.chargeTablesAndGraphs();
    },

    /**
     * Método para cargar los datos de tablas y gráficos
     * @param {*} inputValue Valor introducido en el input de búsqueda correspondiente con el usuario buscado. Por defecto, no habrá usuario por buscar
     */
    chargeTablesAndGraphs: function () {
        const that = this;

        loadingMostrar(undefined, "Cargando los datos de la comunidad. Por favor, espere ...");

        // Obtener el valor que se haya metido en la búsqueda del usuario para un nombre (Debe ser Autocomplete)
        const inputValue = that.txtBuscarUsuariosHack.val().trim();
        const proyectoId = that.inputProyectoId.val();

        const graphsPromises = that.chargeGraphs(inputValue, proyectoId);
        const tablePromises = that.chargeTables(inputValue, proyectoId);

        // Esperar a que ambas promesas se resuelvan
        Promise.all([tablePromises, graphsPromises])
            .then(() => {
                // Fin de carga de datos
                loadingOcultar();
            })
            .catch((error) => {
                // Manejar errores si es necesario
                console.error('Error - chargeTablesAndGraphs:', error);
                mostrarNotificacion("error", "Se ha producido un error al obtener los datos estadísticos de la comunidad. Inténtelo de nuevo más tarde.");
            });
    },

    /**
     * Método para cargar los datos estadísticos en las tablas.
     * @param {*} inputValue Valor introducido en el input de búsqueda correspondiente con el usuario buscado. Por defecto, no habrá usuario por buscar
     */
    chargeTables: function (inputValue, proyectoId) {
        const that = this;

        const promises = [
            that.chargeResumenDeVisitasTable(inputValue, proyectoId),
            that.chargePaginasDestacadasTable(inputValue, proyectoId, this.inputFechaInicio.val(), this.inputFechaFin.val()),
            that.chargeVotosDeRecursosTable(inputValue, proyectoId, this.inputFechaInicio.val(), this.inputFechaFin.val()),
            that.chargeComentariosEnRecursosTable(inputValue, proyectoId, this.inputFechaInicio.val(), this.inputFechaFin.val()),
            that.chargeBusquedasDestacadasTable(inputValue, proyectoId, this.inputFechaInicio.val(), this.inputFechaFin.val()),
            that.chargeDescargaDeRecursosTable(inputValue, proyectoId, this.inputFechaInicio.val(), this.inputFechaFin.val()),
            that.chargeVisitantesFrecuentesVisitantesRecientesTable(inputValue, proyectoId, this.inputFechaInicio.val(), this.inputFechaFin.val()),
            that.chargeSitiosWebReferenciadosTable(inputValue, proyectoId, this.inputFechaInicio.val(), this.inputFechaFin.val()),
            that.chargeUsuariosRecientesTable(inputValue, proyectoId, this.inputFechaInicio.val(), this.inputFechaFin.val()),
            that.chargeRecursosDestacadosTable(inputValue, proyectoId, this.inputFechaInicio.val(), this.inputFechaFin.val()),
        ];

        // Devolver una promesa que se resuelve cuando todas las promesas en el array se resuelven
        return Promise.all(promises);
    },

    /**
     * Método para cargar los datos estadísticos en los gráficos.
     * @param {*} inputValue Valor introducido en el input de búsqueda correspondiente con el usuario buscado. Por defecto, no habrá usuario por buscar
     */
    /*chargeGraphs: function(inputValue = ""){		
        const that = this;
    	
        that.chargeVisitasComunidadGraph(inputValue, this.inputFechaInicio.val(), this.inputFechaFin.val());
        that.chargeNavegacionWebGraph(inputValue, this.inputFechaInicio.val(), this.inputFechaFin.val());
        that.chargePlataformaGraph(inputValue, this.inputFechaInicio.val(), this.inputFechaFin.val());
        that.chargeDispositivosGraph(inputValue, this.inputFechaInicio.val(), this.inputFechaFin.val());
        that.chargePaisesGraph(inputValue, this.inputFechaInicio.val(), this.inputFechaFin.val());
    },
    */

    /**
     * Método para cargar los datos estadísticos en los gráficos.
     * @param {*} inputValue Valor introducido en el input de búsqueda correspondiente con el usuario buscado. Por defecto, no habrá usuario por buscar
     */
    chargeGraphs: function (inputValue = "", proyectoId) {
        const that = this;

        const promises = [
            that.chargeVisitasComunidadGraph(inputValue, proyectoId, this.inputFechaInicio.val(), this.inputFechaFin.val()),
            that.chargeNavegacionWebGraph(inputValue, proyectoId, this.inputFechaInicio.val(), this.inputFechaFin.val()),
            that.chargePlataformaGraph(inputValue, proyectoId, this.inputFechaInicio.val(), this.inputFechaFin.val()),
            that.chargeDispositivosGraph(inputValue, proyectoId, this.inputFechaInicio.val(), this.inputFechaFin.val()),
            that.chargePaisesGraph(inputValue, proyectoId, this.inputFechaInicio.val(), this.inputFechaFin.val()),
        ];

        // Devolver una promesa que se resuelve cuando todas las promesas en el array se resuelven
        return Promise.all(promises);
    },

    /**
     * Método para realizar las peticiones para la obtención de los datos estadísticos de la tabla "Resumen de visitas".
     * @param {*} inputValue Valor introducido en el input de búsqueda correspondiente con el usuario buscado
     */
    chargeResumenDeVisitasTable(inputValue, proyectoId) {
        const that = this;
        let urlTotalVisits = "";
        let urlTotalVisitsMonth = "";
        let urlTotalUsers = "";
        let urlTotalUsersMonth = "";

        /*************** RESUMEN DE VISITAS **************/
        // Construcción de URLs
        if (inputValue) {
            urlTotalVisits = `${that.urlBase}/graphic?method=VisitsSummary.getVisits&filter_limit=-1&period=month&date=last12&force_api_session=1&idSubtable=1&segment=dimension3==${proyectoId};userId==${inputValue}`;
            urlTotalVisitsMonth = `${that.urlBase}/graphic?&method=VisitsSummary.getVisits&filter_limit=-1&period=day&date=last7&force_api_session=1&idSubtable=1&segment=dimension3==${proyectoId};userId==${inputValue}`;
            urlTotalUsers = `${that.urlBase}/graphic?&method=VisitsSummary.getUniqueVisitors&filter_limit=-1&period=month&date=last12&force_api_session=1&idSubtable=1&segment=dimension3==${proyectoId};userId==${inputValue}`;
            urlTotalUsersMonth = `${that.urlBase}/graphic?method=VisitsSummary.getUniqueVisitors&filter_limit=-1&period=day&date=last7&force_api_session=1&idSubtable=1&segment=dimension3==${proyectoId};userId==${inputValue}`;
        } else {
            urlTotalVisits = `${that.urlBase}/graphic?method=VisitsSummary.getVisits&filter_limit=-1&period=month&date=last12&segment=dimension3==${proyectoId}`;
            urlTotalVisitsMonth = `${that.urlBase}/graphic?&method=VisitsSummary.getVisits&filter_limit=-1&period=day&date=last7&segment=dimension3==${proyectoId}`;
            urlTotalUsers = `${that.urlBase}/graphic?&method=VisitsSummary.getUniqueVisitors&filter_limit=-1&period=month&date=last12&segment=dimension3==${proyectoId}`;
            urlTotalUsersMonth = `${that.urlBase}/graphic?method=VisitsSummary.getUniqueVisitors&filter_limit=-1&period=day&date=last7&segment=dimension3==${proyectoId}`;
        }

        // Array de Promesas para todas las peticiones
        const fetchPromises = [
            // 1.1- Resumen de visitas (Visitas Días)
            that.resumenDeVisitasTableFetchVisitasDias(urlTotalVisits),
            // 1.2- Resumen de visitas (Usuarios)
            that.resumenDeVisitasTableFetchVisitasUsuarios(urlTotalVisitsMonth),
            // 1.3- Resumen de visitas (Visitas de usuarios)
            that.resumenDeVisitasTableFetchVisitasDeUsuarios(urlTotalUsers),
            // 1.4- Resumen de visitas (Total visitas usuarios mes)
            that.resumenDeVisitasTableFetchTotalVisitasUsuariosMes(urlTotalUsersMonth),
        ];

        // Devolver una nueva Promesa que se resolverá cuando todas las peticiones se completen
        return Promise.all(fetchPromises)
            .catch(error => {
                // Manejar cualquier error que ocurra durante las peticiones
                mostrarNotificacion("error", "Se ha producido un error al obtener los datos estadísticos de la comunidad. Inténtelo de nuevo más tarde.");
                throw error; // Lanzar el error para que la Promesa se rechace
            });
    },

    /**
     * Método fetch para obtener los datos de Visitas Días para la tabla "Resumen de visitas"
     * @param {*} urlTotalVisits 
     */
    resumenDeVisitasTableFetchVisitasDias: function (urlTotalVisits) {
        const that = this;

        // 1.1- Resumen de visitas (Visitas Días)  
        return new Promise((resolve, reject) => {
            GnossPeticionAjax(
                urlTotalVisits,
                null,
                true
            ).done(function (data) {
                // Variables a utilizar para pintado
                let lastYear = 0;
                let lastMonth = 0;
                let monthCounter = 0;
                let last2Months = 0;
                let last3Months = 0;
                // Datos obtenidos
                const dataVisits = data;
                for (var i in dataVisits) {
                    monthCounter++;
                    lastYear += dataVisits[i];
                    //WE COUNT THE DIFFERENT VISITS EACH MONTH
                    switch (monthCounter) {
                        case 12:
                            lastMonth += dataVisits[i];
                            last2Months += dataVisits[i];
                            last3Months += dataVisits[i];
                            break;
                        case 11:
                            last2Months += dataVisits[i];
                            last3Months += dataVisits[i];
                            break;
                        case 10:
                            last3Months += dataVisits[i];
                            break;
                        default:
                            break;
                    }
                }
                // Pintado de datos de Visitas
                that.lastMonthThVisits.text(lastMonth);
                that.last2MonthThVisits.text(last2Months);
                that.last3MonthThVisits.text(last3Months);
                that.lastYearThVisits.text(lastYear);
                resolve();
            }).fail(function (data) {
                // Mostrar error al tratar de crear una nueva página
                mostrarNotificacion("error", "Se ha producido un error al obtener los datos estadísticos de la comunidad. Inténtelo de nuevo más tarde.");
                reject();
            });
        });
    },

    /**
     * Método fetch para obtener los datos de Visitas de usuarios para la tabla "Resumen de visitas"
     * @param {*} urlTotalVisitsMonth 
     */
    resumenDeVisitasTableFetchVisitasUsuarios: function (urlTotalVisitsMonth) {
        const that = this;

        return new Promise((resolve, reject) => {
            // 1.2- Resumen de visitas (Usuarios)
            GnossPeticionAjax(
                urlTotalVisitsMonth,
                null,
                true
            ).done(function (data) {
                // Variables a utilizar para pintado
                let last7 = 0;
                let yesterday = 0;
                let today = 0;
                let day = 0;

                // Datos obtenidos
                const dataVisits = data;

                for (var i in dataVisits) {
                    last7 += dataVisits[i];
                    day++
                    if (day == 6) {
                        yesterday = dataVisits[i];
                    }
                    if (day == 7) {
                        today = dataVisits[i];
                    }
                }
                // Pintado de datos de Visitas
                that.last7ThVisits.text(last7);
                that.yesterdayThVisits.text(yesterday);
                that.todayThVisits.text(today);
                resolve();
            }).fail(function (data) {
                // Mostrar error al tratar de crear una nueva página
                mostrarNotificacion("error", "Se ha producido un error al obtener los datos estadísticos de la comunidad. Inténtelo de nuevo más tarde.");
                reject();
            });
        });

    },

    /**
     * Método fetch para obtener los datos de Visitas en detalle de usuarios para la tabla "Resumen de visitas"
     * @param {*} urlTotalUsers 
     */
    resumenDeVisitasTableFetchVisitasDeUsuarios: function (urlTotalUsers) {
        const that = this;

        return new Promise((resolve, reject) => {
            GnossPeticionAjax(
                urlTotalUsers,
                null,
                true
            ).done(function (data) {
                let lastYearUsers = 0;
                let lastMonthUsers = 0;
                let monthCounterUsers = 0;
                let last2MonthsUsers = 0;
                let last3MonthsUsers = 0;
                let dataUsers = data;

                for (var i in dataUsers) {
                    monthCounterUsers++;
                    lastYearUsers += dataUsers[i]
                    // Contabilizar las diferentes visitas por mes
                    switch (monthCounterUsers) {
                        case 12:
                            lastMonthUsers += dataUsers[i];
                            last2MonthsUsers += dataUsers[i];
                            last3MonthsUsers += dataUsers[i];
                            break;
                        case 11:
                            last2MonthsUsers += dataUsers[i];
                            last3MonthsUsers += dataUsers[i];
                            break;
                        case 10:
                            last3MonthsUsers += dataUsers[i];
                            break;
                        default:
                            break;
                    }
                }
                // Pintado de los datos				
                that.lastMonthThUsers.text(lastMonthUsers);
                that.last2MonthThUsers.text(last2MonthsUsers);
                that.last3MonthThUsers.text(last3MonthsUsers);
                that.lastYearThUsers.text(lastYearUsers);
                resolve();

            }).fail(function (data) {
                // Mostrar error al tratar de crear una nueva página
                mostrarNotificacion("error", "Se ha producido un error al obtener los datos estadísticos de la comunidad. Inténtelo de nuevo más tarde.");
                reject();
            });

        });
    },

    /**
     * Método fetch para obtener los datos de Visitas totales de usuarios para la tabla "Resumen de visitas"
     * @param {*} urlTotalUsersMonth 
     */
    resumenDeVisitasTableFetchTotalVisitasUsuariosMes: function (urlTotalUsersMonth) {
        const that = this;

        return new Promise((resolve, reject) => {
            GnossPeticionAjax(
                urlTotalUsersMonth,
                null,
                true
            ).done(function (data) {
                let last7Users = 0;
                let yesterdayUsers = 0;
                let todayUsers = 0;
                let dayUsers = 0;
                const dataUsers = data;

                for (var i in dataUsers) {
                    last7Users += dataUsers[i];
                    dayUsers++;
                    if (dayUsers == 6) {
                        yesterdayUsers = dataUsers[i];
                    }
                    if (dayUsers == 7) {
                        todayUsers = dataUsers[i];
                    }
                }

                // Pintado de los datos				
                that.last7ThUsers.text(last7Users);
                that.yesterdayThUsers.text(yesterdayUsers);
                that.todayThUsers.text(todayUsers);
                resolve();

            }).fail(function (data) {
                // Mostrar error al tratar de crear una nueva página
                mostrarNotificacion("error", "Se ha producido un error al obtener los datos estadísticos de la comunidad. Inténtelo de nuevo más tarde.");
                reject();
            });
        });


    },


    /**
     * Método para realizar las peticiones para la obtención de los datos estadísticos de la tabla "Páginas destacadas".
     * @param {*} inputValue Nombre del usuario por el que filtrar
     * @param {*} fechaInicio Fecha inical para obtención del filtrado de los datos
     * @param {*} fechaFin Fecha inical para obtención del filtrado de los datos
     */
    chargePaginasDestacadasTable(inputValue, proyectoId, fechaInicio, fechaFin) {
        const that = this;
        // Peticion de todas las acciones de las paginas que registra matomo
        let urlMostActionsElements = "";

        if (inputValue) {
            urlMostActionsElements = `${that.urlBase}/graphic?&method=Live.getLastVisitsDetails&filter_limit=-1&force_api_session=1&idSubtable=1&period=range&date=${fechaInicio},${fechaFin}&segment=dimension3==${proyectoId};userId==${inputValue}`;
        } else {
            urlMostActionsElements = `${that.urlBase}/graphic?&method=Live.getLastVisitsDetails&filter_limit=-1&force_api_session=1&idSubtable=1&period=range&date=${fechaInicio},${fechaFin}&segment=dimension3==${proyectoId}`;
        }

        return new Promise((resolve, reject) => {
            // Cargar de datos
            GnossPeticionAjax(
                urlMostActionsElements,
                null,
                true
            ).done(function (data) {
                // Almacenamos los pageTitle de cada paginas
                that.paginasUrl = new Object();

                // Por cada url el numero de visitas
                that.paginasVisitas = new Object();
                that.paginasTiempoTotal = new Object();
                that.paginasTiempoMedio = new Object();

                // Objeto que contedrá un set de URLs por cada visita
                var urlsPorIdVisita = new Object();
                // Datos obtenidos								
                for (var i in data) {
                    var idVisita = data[i]["idVisit"];
                    urlsPorIdVisita[idVisita] = new Set();
                    for (var pag in data[i]["actionDetails"]) {
                        if ((typeof data[i]["actionDetails"][pag] !== "undefined") && (data[i]["actionDetails"][pag]["type"] == "action")) {
                            if (that.paginasUrl[data[i]["actionDetails"][pag]["url"]] == undefined) {
                                that.paginasUrl[data[i]["actionDetails"][pag]["url"]] = data[i]["actionDetails"][pag]["pageTitle"];
                            } else if (!that.paginasUrl[data[i]["actionDetails"][pag]["url"]].includes(data[i]["actionDetails"][pag]["pageTitle"])) {
                                that.paginasUrl[data[i]["actionDetails"][pag]["url"]] += ` | ${data[i]["actionDetails"][pag]["pageTitle"]}`;
                            }

                            // Contabilizamos las visitas
                            if (that.paginasVisitas[data[i]["actionDetails"][pag]["url"]] == undefined) {
                                that.paginasVisitas[data[i]["actionDetails"][pag]["url"]] = 1;
                            } else if (!urlsPorIdVisita[idVisita].has(data[i]["actionDetails"][pag]["url"])) {
                                that.paginasVisitas[data[i]["actionDetails"][pag]["url"]] += 1;

                            }
                            urlsPorIdVisita[idVisita].add(data[i]["actionDetails"][pag]["url"]);

                            // Recogemos el tiempo en segundos que ha registrado cada visita
                            if (that.paginasTiempoTotal[data[i]["actionDetails"][pag]["url"]] == undefined) {
                                that.paginasTiempoTotal[data[i]["actionDetails"][pag]["url"]] = data[i]["actionDetails"][pag]["timeSpent"];
                            } else {
                                that.paginasTiempoTotal[data[i]["actionDetails"][pag]["url"]] += data[i]["actionDetails"][pag]["timeSpent"];
                            }
                        }
                    }
                }

                // Calculamos el tiempo medio
                for (var id in that.paginasUrl) {
                    let time = that.paginasTiempoTotal[id] / that.paginasVisitas[id];
                    var hours = Math.floor(time / 3600).toString();
                    time = time - hours * 3600;
                    var minutes = Math.floor(time / 60).toString();
                    var seconds = (time - minutes * 60);
                    var secondsRound = Math.floor(seconds).toString()
                    var finalTime = hours.padStart(2, '0') + ":" + minutes.padStart(2, '0') + ":" + secondsRound.padStart(2, '0');
                    that.paginasTiempoMedio[id] = finalTime;
                }
                // Cogemos los 5 primeros
                var items = Object.keys(that.paginasVisitas).map(function (key) {
                    return [key, that.paginasVisitas[key]];
                });

                // Ordenamos por el numero de visitas
                items.sort(function (first, second) {
                    return second[1] - first[1];
                });

                var arrayTopV = items.slice(0, 5);

                // Pintado de datos por cada línea
                for (var i = 1; i < 6; i++) {
                    const tdNombre = $(`#top${i}Name`);
                    const tdVisitas = $(`#top${i}Visits`);
                    const tdTime = $(`#top${i}VisitsTime`);
                    tdNombre.text("");
                    tdVisitas.text("");
                    tdTime.text("");
                }

                let cont = 0;
                for (var key in arrayTopV) {
                    cont++;
                    const tdNombre = $(`#top${cont}Name`);
                    const tdVisitas = $(`#top${cont}Visits`);
                    const tdTime = $(`#top${cont}VisitsTime`);

                    if (arrayTopV[key][0] != null && arrayTopV[key][1] != null) {
                        const aNombre = $(`#aNombreTopPD${cont}`);
                        if (aNombre.length > 0) {
                            aNombre.attr('href', arrayTopV[key][0]);
                        } else {

                            const a = $('<a>').attr('href', arrayTopV[key][0])
                                .text(that.paginasUrl[arrayTopV[key][0]])
                                .attr('id', 'aNombreTopPD' + cont);
                            tdNombre.append(a);
                        }
                        tdVisitas.text(arrayTopV[key][1]);
                        tdTime.text(that.paginasTiempoMedio[arrayTopV[key][0]]);
                    }
                }

                resolve();
            }).fail(function (data) {
                // Mostrar error al tratar de crear una nueva página
                mostrarNotificacion("error", "Se ha producido un error al obtener los datos estadísticos de la comunidad. Inténtelo de nuevo más tarde.");
                reject();
            });
        });
    },

    /**
     * Método para realizar las peticiones para la obtención de los datos estadísticos de la tabla "Votos de recursos".
     * @param {*} inputValue Nombre del usuario por el que filtrar
     * @param {*} fechaInicio Fecha inical para obtención del filtrado de los datos
     * @param {*} fechaFin Fecha inical para obtención del filtrado de los datos
     */
    chargeVotosDeRecursosTable(inputValue, proyectoId, fechaInicio, fechaFin) {
        const that = this;
        let urltotalTimeSpendByAllUsers = "";

        if (inputValue) {
            urltotalTimeSpendByAllUsers = `${that.urlBase}/graphic-downloads?method=Live.getLastVisitsDetails&filter_limit=-1&force_api_session=1&idSubtable=1&period=day&date=${fechaInicio},${fechaFin}&segment=dimension3==${proyectoId};eventName==Me+gusta;userId==${inputValue}`;
        } else {
            urltotalTimeSpendByAllUsers = `${that.urlBase}/graphic-downloads?method=Live.getLastVisitsDetails&filter_limit=-1&segment=dimension3==${proyectoId};eventName==Me+gusta&force_api_session=1&idSubtable=1&period=day&date=${fechaInicio},${fechaFin}`;
        }

        return new Promise((resolve, reject) => {
            // Cargar de datos
            GnossPeticionAjax(
                urltotalTimeSpendByAllUsers,
                null,
                true
            ).done(function (data) {
                // Variables a utilizar para pintado			
                that.recursos = new Object();
                that.recursosDownloadNameVoted = new Object(); //Pair id-name
                that.recursosDownloadURLVoted = new Object(); //Pair id-url	

                // Carga de los datos			
                for (var i in data) {
                    for (var det in data[i]["actionDetails"]) {
                        if ((typeof data[i]["actionDetails"][det] !== "undefined") && (data[i]["actionDetails"][det]["eventName"] == "Me gusta") && (data[i]["actionDetails"][det]["dimension4"] != null)) {
                            if (that.recursos[data[i]["actionDetails"][det]["ResourceID"]] !== undefined) {
                                that.recursos[data[i]["actionDetails"][det]["ResourceID"]] += 1
                            } else {
                                that.recursos[data[i]["actionDetails"][det]["ResourceID"]] = 1
                            }
                            if (that.recursosDownloadNameVoted[data[i]["actionDetails"][det]["ResourceID"]] === undefined) {
                                that.recursosDownloadNameVoted[data[i]["actionDetails"][det]["ResourceID"]] = data[i]["actionDetails"][det]["ResourceName"]
                            }
                            if (that.recursosDownloadURLVoted[data[i]["actionDetails"][det]["ResourceID"]] === undefined) {
                                that.recursosDownloadURLVoted[data[i]["actionDetails"][det]["ResourceID"]] = data[i]["actionDetails"][det]["ResourceURL"]
                            }
                        }
                    }
                }

                // Create items array
                var items = Object.keys(that.recursos).map(function (key) {
                    return [key, that.recursos[key]];
                });

                // Sort the array based on the second element
                items.sort(function (first, second) {
                    return second[1] - first[1];
                });

                // Create a new array with only the first 5 items
                const nuevo = items.slice(0, 5);

                // Pintado de datos 
                let cont = 0;
                for (var i = 1; i < 6; i++) {
                    const tdNombre = $(`#top${i}ResourceUrl`);
                    const tdVisitas = $(`#top${i}ResourceVotes`);
                    tdNombre.text("");
                    tdVisitas.text("");
                }

                for (var key in nuevo) {
                    cont++;
                    const tdNombre = $(`#top${cont}ResourceUrl`);
                    const tdVisitas = $(`#top${cont}ResourceVotes`);
                    if (nuevo[key][0] != null && nuevo[key][1] != null) {
                        const aNombre = $(`#aNombreVote${cont}`);
                        if (aNombre.length > 0) {
                            var a = $('<a>').attr('href', that.recursosDownloadURLVoted[nuevo[key][0]]);
                        } else {
                            var a = $('<a>').attr('href', that.recursosDownloadURLVoted[nuevo[key][0]]);

                            if (that.recursosDownloadNameVoted[nuevo[key][0]] != null) {
                                a.text(that.recursosDownloadNameVoted[nuevo[key][0]]);
                            } else {
                                a.text(nuevo[key][0]);
                            }

                            a.attr('id', 'aNombreVote' + cont);
                            tdNombre.append(a);
                        }
                        tdVisitas.text(nuevo[key][1]);
                    }
                }
                resolve();
            }).fail(function (data) {
                // Mostrar error al tratar de crear una nueva página
                mostrarNotificacion("error", "Se ha producido un error al obtener los datos estadísticos de la comunidad. Inténtelo de nuevo más tarde.");
                reject();
            });
        });
    },

    /**
     * Método para realizar las peticiones para la obtención de los datos estadísticos de la tabla "Comentarios en recursos".
     * @param {*} inputValue Nombre del usuario por el que filtrar
     * @param {*} fechaInicio Fecha inical para obtención del filtrado de los datos
     * @param {*} fechaFin Fecha inical para obtención del filtrado de los datos
     */
    chargeComentariosEnRecursosTable(inputValue, proyectoId, fechaInicio, fechaFin) {
        const that = this;
        let urlMostComentedElements = "";

        if (inputValue) {
            urlMostComentedElements = `${that.urlBase}/graphic-downloads?&method=Live.getLastVisitsDetails&filter_limit=-1&force_api_session=1&idSubtable=1&period=day&date=${fechaInicio},${fechaFin}&segment=dimension3==${proyectoId};eventName==Comentar;userId==${inputValue}`;
        } else {
            urlMostComentedElements = `${that.urlBase}/graphic-downloads?&method=Live.getLastVisitsDetails&filter_limit=-1&segment=dimension3==${proyectoId};eventName==Comentar&force_api_session=1&idSubtable=1&period=day&date=${fechaInicio},${fechaFin}`;
        }

        return new Promise((resolve, reject) => {
            // Cargar de datos
            GnossPeticionAjax(
                urlMostComentedElements,
                null,
                true
            ).done(function (data) {
                // Variables a utilizar para pintado			
                that.recursosComent = new Object();
                that.recursosComentName = new Object(); //Pair id-name
                that.recursosComentURL = new Object(); //Pair id-url

                // Carga de los datos			
                for (var i in data) {
                    for (var det in data[i]["actionDetails"]) {
                        if ((typeof data[i]["actionDetails"][det] !== "undefined") && (data[i]["actionDetails"][det]["eventAction"] == "Comentar")) {
                            if (that.recursosComent[data[i]["actionDetails"][det]["ResourceID"]] !== undefined) {
                                that.recursosComent[data[i]["actionDetails"][det]["ResourceID"]] += 1
                            } else {
                                that.recursosComent[data[i]["actionDetails"][det]["ResourceID"]] = 1
                            }
                            if (that.recursosComentName[data[i]["actionDetails"][det]["ResourceID"]] === undefined) {
                                that.recursosComentName[data[i]["actionDetails"][det]["ResourceID"]] = data[i]["actionDetails"][det]["ResourceName"]
                            }
                            if (that.recursosComentURL[data[i]["actionDetails"][det]["ResourceID"]] === undefined) {
                                that.recursosComentURL[data[i]["actionDetails"][det]["ResourceID"]] = data[i]["actionDetails"][det]["ResourceURL"]
                            }
                        }
                    }
                }

                // Create items array
                var items = Object.keys(that.recursosComent).map(function (key) {
                    return [key, that.recursosComent[key]];
                });
                // Sort the array based on the second element
                items.sort(function (first, second) {
                    return second[1] - first[1];
                });
                // Create a new array with only the first 5 items
                const nuevo = items.slice(0, 5);

                //Pintado de datos
                let cont = 0;
                for (var i = 1; i < 6; i++) {
                    const tdNombre = $(`#top${i}ResourceUrl_Coments`);
                    const tdVisitas = $(`#top${i}Resource_Coments`);
                    tdNombre.text("");
                    tdVisitas.text("");
                }
                for (var key in nuevo) {
                    cont++;
                    const tdNombre = $(`#top${cont}ResourceUrl_Coments`);
                    const tdVisitas = $(`#top${cont}Resource_Coments`);

                    if (nuevo[key][0] != null && nuevo[key][1] != null) {
                        var aNombre = $("#aNombreCom" + cont);

                        if (aNombre.length > 0) {
                            const a = $('<a>').attr('href', that.recursosComentURL[nuevo[key][0]]);
                        } else {
                            var a = $('<a>').attr('href', that.recursosComentURL[nuevo[key][0]])
                                .text(that.recursosComentName[nuevo[key][0]])
                                .attr('id', 'aNombreCom' + cont);

                            tdNombre.append(a);
                        }
                        tdVisitas.text(nuevo[key][1]);
                    }
                }
                resolve();

            }).fail(function (data) {
                // Mostrar error al tratar de crear una nueva página
                mostrarNotificacion("error", "Se ha producido un error al obtener los datos estadísticos de la comunidad. Inténtelo de nuevo más tarde.");
                reject();
            });
        });


    },

    /**
     * Método para realizar las peticiones para la obtención de los datos estadísticos de la tabla "Búsquedas destacadas".
     * @param {*} inputValue Nombre del usuario por el que filtrar
     * @param {*} fechaInicio Fecha inical para obtención del filtrado de los datos
     * @param {*} fechaFin Fecha inical para obtención del filtrado de los datos
     */
    chargeBusquedasDestacadasTable(inputValue, proyectoId, fechaInicio, fechaFin) {
        const that = this;
        let urlMostSearchedElements = "";

        if (inputValue) {
            urlMostSearchedElements = `${that.urlBase}/graphic?method=Live.getLastVisitsDetails&filter_limit=-1&force_api_session=1&idSubtable=1&period=day&date=${fechaInicio},${fechaFin}&segment=dimension3==${proyectoId};userId==${inputValue}`;
        } else {
            urlMostSearchedElements = `${that.urlBase}/graphic?method=Live.getLastVisitsDetails&filter_limit=-1&force_api_session=1&idSubtable=1&period=day&date=${fechaInicio},${fechaFin}&segment=dimension3==${proyectoId}`;
        }

        return new Promise((resolve, reject) => {
            // Cargar de datos
            GnossPeticionAjax(
                urlMostSearchedElements,
                null,
                true
            ).done(function (data) {
                // Variables a utilizar para pintado			
                let recursosSearch = new Object();
                // Carga de los datos			
                for (var i in data) {
                    for (var det in data[i]["actionDetails"]) {
                        if ((typeof data[i]["actionDetails"][det] !== "undefined") && (data[i]["actionDetails"][det]["type"] == "search")) {
                            if (recursosSearch[data[i]["actionDetails"][det]["siteSearchKeyword"]] !== undefined) {
                                recursosSearch[data[i]["actionDetails"][det]["siteSearchKeyword"]] += 1
                            } else {
                                recursosSearch[data[i]["actionDetails"][det]["siteSearchKeyword"]] = 1
                            }
                        }
                    }
                }

                // Create items array
                let items = Object.keys(recursosSearch).map(function (key) {
                    return [key, recursosSearch[key]];
                });
                // Sort the array based on the second element
                items.sort(function (first, second) {
                    return second[1] - first[1];
                });

                // Create a new array with only the first 5 items
                const nuevo = items.slice(0, 5);
                //Pintado de datos        	
                let cont = 0;
                for (var i = 1; i < 6; i++) {
                    const tdNombre = $(`#top${i}Search`);
                    const tdVisitas = $(`#top${i}SearchNumber`);
                    tdNombre.text("");
                    tdVisitas.text("");
                }
                for (var key in nuevo) {
                    cont++;
                    var tdNombre = $(`#top${cont}Search`);
                    var tdVisitas = $(`#top${cont}SearchNumber`);
                    if (nuevo[key][0] != null && nuevo[key][1] != null) {
                        tdNombre.text(nuevo[key][0]);
                        tdVisitas.text(nuevo[key][1]);
                    }
                }
                resolve();
            }).fail(function (data) {
                // Mostrar error al tratar de crear una nueva página
                mostrarNotificacion("error", "Se ha producido un error al obtener los datos estadísticos de la comunidad. Inténtelo de nuevo más tarde.");
                reject();
            });
        });
    },


    /**
     * Método para realizar las peticiones para la obtención de los datos estadísticos de la tabla "Descarga de recursos".
     * @param {*} inputValue Nombre del usuario por el que filtrar
     * @param {*} fechaInicio Fecha inical para obtención del filtrado de los datos
     * @param {*} fechaFin Fecha inical para obtención del filtrado de los datos
     */
    chargeDescargaDeRecursosTable(inputValue, proyectoId, fechaInicio, fechaFin) {
        const that = this;
        let urlMostDownloadedElements = "";

        if (inputValue) {
            urlMostDownloadedElements = `${that.urlBase}/graphic-downloads?&method=Live.getLastVisitsDetails&filter_limit=-1&force_api_session=1&idSubtable=1&period=day&date=${fechaInicio},${fechaFin}&segment=dimension3==${proyectoId};actionUrl=@download;userId==${inputValue}`;
        } else {
            urlMostDownloadedElements = `${that.urlBase}/graphic-downloads?method=Live.getLastVisitsDetails&filter_limit=-1&segment=dimension3==${proyectoId};actionUrl=@download&force_api_session=1&idSubtable=1&period=day&date=${fechaInicio},${fechaFin}`;
        }

        return new Promise((resolve, reject) => {
            // Cargar de datos
            GnossPeticionAjax(
                urlMostDownloadedElements,
                null,
                true
            ).done(function (data) {
                // Variables a utilizar para pintado			
                that.recursosDownload = new Object();
                that.recursosDownloadName = new Object(); //Pair id-name
                that.recursosDownloadURL = new Object();

                // Carga de los datos			
                for (var i in data) {
                    for (var det in data[i]["actionDetails"]) {
                        if ((typeof data[i]["actionDetails"][det] !== "undefined") && (data[i]["actionDetails"][det]["type"] == "download")) {
                            if (that.recursosDownload[data[i]["actionDetails"][det]["ResourceID"]] !== undefined) {
                                that.recursosDownload[data[i]["actionDetails"][det]["ResourceID"]] += 1;
                            } else {
                                that.recursosDownload[data[i]["actionDetails"][det]["ResourceID"]] = 1;
                            }
                            if (that.recursosDownloadName[data[i]["actionDetails"][det]["ResourceID"]] === undefined) {
                                that.recursosDownloadName[data[i]["actionDetails"][det]["ResourceID"]] = data[i]["actionDetails"][det]["ResourceName"];
                            }
                            if (that.recursosDownloadURL[data[i]["actionDetails"][det]["ResourceID"]] === undefined) {
                                that.recursosDownloadURL[data[i]["actionDetails"][det]["ResourceID"]] = data[i]["actionDetails"][det]["ResourceURL"];
                            }
                        }
                    }
                }

                // Create items array
                let items = Object.keys(that.recursosDownload).map(function (key) {
                    return [key, that.recursosDownload[key]];
                });
                // Sort the array based on the second element
                items.sort(function (first, second) {
                    return second[1] - first[1];
                });
                // Create a new array with only the first 5 items
                const nuevo = items.slice(0, 5);

                //Pintado de datos        	
                let cont = 0;
                for (var i = 1; i < 6; i++) {
                    const tdNombre = $(`#top${i}Download`);
                    const tdVisitas = $(`#top${i}DownloadNumber`);
                    tdNombre.text("");
                    tdVisitas.text("");
                }
                for (var key in nuevo) {
                    cont++;
                    const tdNombre = $(`#top${cont}Download`);
                    const tdVisitas = $(`#top${cont}DownloadNumber`);
                    if (nuevo[key][0] != null && nuevo[key][1] != null) {
                        const aNombre = $(`#aNombreDownload${cont}`);

                        if (aNombre.length > 0) {
                            a.attr('href', nuevo[key][0]);
                        } else {
                            const a = $('<a>').attr('href', that.recursosDownloadURL[nuevo[key][0]])
                                .text(that.recursosDownloadName[nuevo[key][0]])
                                .attr('id', 'aNombreDownload' + cont);
                            tdNombre.append(a);
                        }
                        tdVisitas.text(nuevo[key][1]);
                    }
                }
                resolve();
            }).fail(function (data) {
                // Mostrar error al tratar de crear una nueva página
                mostrarNotificacion("error", "Se ha producido un error al obtener los datos estadísticos de la comunidad. Inténtelo de nuevo más tarde.");
                reject();
            });
        });
    },

    chargeRecursosDestacadosTable(inputValue, proyectoId, pFechaInicio, pFechaFin) {
        const that = this;
        let urlTopResources = "";
        if (inputValue) {
            urlTopResources = `${that.urlBase}/graphic-resources?&method=Live.getLastVisitsDetails&filter_limit=-1&force_api_session=1&idSubtable=1&period=day&date=${pFechaInicio},${pFechaFin}&segment=dimension3==${proyectoId};actionUrl=@/recurso/;userId==${inputValue}`;
        } else {
            urlTopResources = `${that.urlBase}/graphic-resources?method=Live.getLastVisitsDetails&filter_limit=-1&segment=dimension3==${proyectoId};actionUrl=@/recurso/&force_api_session=1&idSubtable=1&period=day&date=${pFechaInicio},${pFechaFin}`;
        }
        return new Promise((resolve, reject) => {
            GnossPeticionAjax(
                urlTopResources,
                null,
                true
            ).done(function (data) {
                that.recursosName = new Object();
                that.recursosUrl = new Object();
                that.recursosVisitas = new Object();
                that.recursosTiempoTotal = new Object();
                that.recursosTiempoMedio = new Object();

                var recursosPorIdVisita = new Object();
                var idVisita;

                for (var i in data) {
                    idVisita = data[i]["idVisit"];
                    recursosPorIdVisita[idVisita] = new Set();

                    for (var det in data[i]["actionDetails"]) {

                        if ((typeof data[i]["actionDetails"][det] !== "undefined") && (data[i]["actionDetails"][det]["type"] == "action")) {
                            let url = data[i]["actionDetails"][det]["url"];
                            if (url.includes("/recurso/") && data[i]["actionDetails"][det]["ResourceID"] !== null) {
                                let idRecurso = data[i]["actionDetails"][det]["ResourceID"];
                                // Guardamos el nombre de la pagina del recurso
                                if (that.recursosName[idRecurso] == undefined) {
                                    that.recursosName[idRecurso] = data[i]["actionDetails"][det]["pageTitle"];
                                }
                                // Guardamos la url del recurso
                                if (that.recursosUrl[idRecurso] == undefined) {
                                    that.recursosUrl[idRecurso] = url;
                                }
                                // Guardamos la visita al recurso
                                if (that.recursosVisitas[idRecurso] == undefined) {
                                    that.recursosVisitas[idRecurso] = 1;
                                } else if (!recursosPorIdVisita[idVisita].has(idRecurso)) {
                                    that.recursosVisitas[idRecurso] += 1;
                                }
                                recursosPorIdVisita[idVisita].add(idRecurso);
                                // Guardamos el tiempo total de la visita
                                if (that.recursosTiempoTotal[idVisita] == undefined) {
                                    that.recursosTiempoTotal[idRecurso] = data[i]["actionDetails"][det]["timeSpent"];
                                } else {
                                    that.recursosTiempoTotal[idRecurso] += data[i]["actionDetails"][det]["timeSpent"];
                                }
                            }
                        }
                    }
                }

                // Calculamos el tiempo medio
                for (var idRecurso in that.recursosName) {
                    let time = that.recursosTiempoTotal[idRecurso] / that.recursosVisitas[idRecurso];
                    var hours = Math.floor(time / 3600).toString();
                    time = time - hours * 3600;
                    var minutes = Math.floor(time / 60).toString();
                    var seconds = (time - minutes * 60);
                    var secondsRound = Math.floor(seconds).toString()
                    var finalTime = hours.padStart(2, '0') + ":" + minutes.padStart(2, '0') + ":" + secondsRound.padStart(2, '0');
                    that.recursosTiempoMedio[idRecurso] = finalTime;
                }

                // Cogemos los 5 primeros
                var items = Object.keys(that.recursosVisitas).map(function (key) {
                    return [key, that.recursosVisitas[key]];
                });

                // Ordenamos por el numero de visitas
                items.sort(function (first, second) {
                    return second[1] - first[1];
                });

                // Par idRecurso - visitas
                var arrayTopRecursos = items.slice(0, 5);

                // Pintado de datos por cada línea

                for (var i = 1; i < 6; i++) {
                    const tdNombre = $(`#top${i}ResourceName`);
                    const tdVisitas = $(`#top${i}ResourceVisits`);
                    const tdTime = $(`#top${i}ResourceVisitsTime`);
                    tdNombre.text("");
                    tdVisitas.text("");
                    tdTime.text("");
                }

                let cont = 0;
                for (var key in arrayTopRecursos) {
                    cont++;
                    const tdNombre = $(`#top${cont}ResourceName`);
                    const tdVisitas = $(`#top${cont}ResourceVisits`);
                    const tdTime = $(`#top${cont}ResourceVisitsTime`);
                    if (arrayTopRecursos[key][0] != null && arrayTopRecursos[key][1] != null) {
                        const aNombre = $(`#aNombreTopRecurso${cont}`);
                        if (aNombre.length > 0) {
                            aNombre.attr('href', that.recursosUrl[arrayTopRecursos[key][0]]);
                        } else {
                            const a = $('<a>').attr('href', that.recursosUrl[arrayTopRecursos[key][0]])
                                .text(that.recursosName[arrayTopRecursos[key][0]])
                                .attr('id', `aNombreTopRecurso${cont}`);
                            tdNombre.append(a);
                        }
                    }
                    tdVisitas.text(arrayTopRecursos[key][1]);
                    tdTime.text(that.recursosTiempoMedio[arrayTopRecursos[key][0]]);
                }
                resolve();
            }).fail(function (data) {
                // Mostrar error al tratar de crear una nueva página
                mostrarNotificacion("error", "Se ha producido un error al obtener los datos estadísticos de la comunidad. Inténtelo de nuevo más tarde.");
                reject();
            });
        });
    },

    /**
     *  Método para realizar las peticiones para la obtención de los datos estadísticos de las tablas "Visitantes frecuentes y Visitantes recientes".
     * @param {*} inputValue Nombre del usuario por el que filtrar
     * @param {*} fechaInicio Fecha inical para obtención del filtrado de los datos
     * @param {*} fechaFin Fecha inical para obtención del filtrado de los datos
     */
    chargeVisitantesFrecuentesVisitantesRecientesTable(inputValue, proyectoId, pFechaInicio, pFechaFin) {
        const that = this;
        const urlTopVisitors = `${that.urlBase}/graphic-users?module=API&method=UserId.getUsers&format=json&force_api_session=1&idSubtable=1&period=day&date=${pFechaInicio},${pFechaFin}&segment=dimension3==${proyectoId}`;
        var divMatomoVisitantesFrecuentes = $("#div_table_frecuent_visitors");
        var divMatomoVisitantesRecientes = $("#div_table_recent_visitors");
        return new Promise((resolve, reject) => {
            if (inputValue) {
                divMatomoVisitantesFrecuentes.addClass("d-none");
                divMatomoVisitantesRecientes.addClass("d-none");
                resolve();
            } else {
                if (divMatomoVisitantesFrecuentes.hasClass("d-none") && divMatomoVisitantesRecientes.hasClass("d-none")) {
                    divMatomoVisitantesFrecuentes.removeClass("d-none");
                    divMatomoVisitantesRecientes.removeClass("d-none");
                }
                // Cargar de datos
                GnossPeticionAjax(
                    urlTopVisitors,
                    null,
                    true
                ).done(function (data) {
                    // Variables a utilizar para pintado		
                    that.nombrePerfilPorUsuarioId = new Object();
                    that.visitasUnicasPorUsuarioId = new Object();
                    that.urlPerfilPorUsuarioID = new Object();
                    that.tiempoTotalPorUsuarioId = new Object();
                    // Recogemos los datos
                    for (var dia in data) {
                        for (var datosVisitante in data[dia]) {
                            if ((data[dia][datosVisitante]["nombrePerfil"] != null) && (typeof data[dia][datosVisitante] !== "undefined")) {
                                let usuarioId = data[dia][datosVisitante]["segment"].split("==")[1]
                                if (that.nombrePerfilPorUsuarioId[usuarioId] == undefined) {
                                    that.nombrePerfilPorUsuarioId[usuarioId] = data[dia][datosVisitante]["nombrePerfil"];
                                    that.urlPerfilPorUsuarioID[usuarioId] = data[dia][datosVisitante]["urlPerfil"]
                                    that.visitasUnicasPorUsuarioId[usuarioId] = data[dia][datosVisitante]["nb_uniq_visitors"];
                                    that.tiempoTotalPorUsuarioId[usuarioId] = data[dia][datosVisitante]["sum_visit_length"];
                                } else {
                                    that.visitasUnicasPorUsuarioId[usuarioId] += data[dia][datosVisitante]["nb_uniq_visitors"];
                                    that.tiempoTotalPorUsuarioId[usuarioId] += data[dia][datosVisitante]["sum_visit_length"];
                                }
                            }
                        }
                    }
                    // Ordenamos segun el numero de visitas unicas
                    var visitantesFrecuentes = Object.keys(that.visitasUnicasPorUsuarioId).map(function (key) {
                        return [key, that.visitasUnicasPorUsuarioId[key]];
                    });

                    visitantesFrecuentes.sort(function (first, second) {
                        return second[1] - first[1];
                    });

                    var TopVisFrecuentes = visitantesFrecuentes.slice(0, 5);

                    // Pintando Visitantes Frecuentes

                    let cont = 0;
                    for (var i = 1; i < 6; i++) {
                        const tdNombre = $(`#top${i}Visit`);
                        const tdVisitas = $(`#top${i}VisitNumber`);
                        const tdTime = $(`#top${i}Time`);
                        tdNombre.text("");
                        tdVisitas.text("");
                        tdTime.text("");
                    }
                    for (var key in TopVisFrecuentes) {
                        var idUsuario = TopVisFrecuentes[key][0];
                        cont++;
                        const tdNombre = $(`#top${cont}Visit`);
                        const tdVisitas = $(`#top${cont}VisitNumber`);
                        const tdTime = $(`#top${cont}Time`);

                        if (TopVisFrecuentes[key][0] != null && TopVisFrecuentes[key][1] != null) {
                            const aNombre = $(`#aNombreTopV${cont}`);
                            if (aNombre.length > 0) {
                                aNombre.attr('href', that.urlPerfilPorUsuarioID[idUsuario]);
                            } else {
                                const a = $('<a>').attr('href', that.urlPerfilPorUsuarioID[idUsuario])
                                    .text(that.nombrePerfilPorUsuarioId[idUsuario])
                                    .attr('id', 'aNombreTopV' + cont);

                                tdNombre.append(a);
                            }

                            let time = that.tiempoTotalPorUsuarioId[idUsuario];
                            const hours = Math.floor(time / 3600);
                            time = time - hours * 3600;
                            const minutes = Math.floor(time / 60);
                            const seconds = time - minutes * 60;
                            const finalTime = str_pad_left(hours, '0', 2) + ':' + str_pad_left(minutes, '0', 2) + ':' + str_pad_left(seconds, '0', 2);
                            tdTime.text(finalTime);
                            tdVisitas.text(that.visitasUnicasPorUsuarioId[idUsuario]);
                        }
                    }

                    // Ordenamos segun el numero de visitas unicas
                    var visitantesRecientes = Object.keys(that.tiempoTotalPorUsuarioId).map(function (key) {
                        return [key, that.tiempoTotalPorUsuarioId[key]];
                    });

                    visitantesRecientes.sort(function (first, second) {
                        return second[1] - first[1];
                    });

                    var TopVisRecientes = visitantesRecientes.slice(0, 5);

                    //Pintado de "Visitas recientes"
                    let contRecent = 0;
                    for (var i = 1; i < 6; i++) {
                        const tdNombre = $(`#top${i}VisitRecent`);
                        const tdVisitas = $(`#top${i}VisitNumberRecent`);
                        const tdTime = $(`#top${i}TimeRecent`);
                        tdNombre.text("");
                        tdVisitas.text("");
                        tdTime.text("");
                    }

                    for (var key in TopVisRecientes) {
                        contRecent++;
                        const tdNombre = $(`#top${contRecent}VisitRecent`);
                        const tdVisitas = $(`#top${contRecent}VisitNumberRecent`);
                        const tdTime = $(`#top${contRecent}TimeRecent`);

                        if (TopVisRecientes[key][0] != null && TopVisRecientes[key][1] != null) {
                            let usuarioId = TopVisRecientes[key][0];
                            const aNombre = $(`#aNombreRecV${contRecent}`);

                            if (aNombre.length > 0) {
                                aNombre.attr('href', that.urlPerfilPorUsuarioID[usuarioId]);
                            } else {
                                var a = $('<a>').attr('href', that.urlPerfilPorUsuarioID[usuarioId])
                                    .text(that.nombrePerfilPorUsuarioId[usuarioId])
                                    .attr('id', `aNombreRecV${contRecent}`);

                                tdNombre.append(a);
                            }

                            let time = that.tiempoTotalPorUsuarioId[usuarioId];
                            const hours = Math.floor(time / 3600);
                            time = time - hours * 3600;
                            const minutes = Math.floor(time / 60);
                            const seconds = time - minutes * 60;
                            const finalTime = str_pad_left(hours, '0', 2) + ':' + str_pad_left(minutes, '0', 2) + ':' + str_pad_left(seconds, '0', 2);
                            tdTime.text(finalTime);
                            tdVisitas.text(that.visitasUnicasPorUsuarioId[usuarioId]);
                        }
                    }
                    resolve();
                }).fail(function (data) {
                    // Mostrar error al tratar de crear una nueva página
                    mostrarNotificacion("error", "Se ha producido un error al obtener los datos estadísticos de la comunidad. Inténtelo de nuevo más tarde.");
                    reject();
                });
            }
        });
    },


    /**
     * Método para realizar las peticiones para la obtención de los datos estadísticos de la tabla "Sitios web más referenciados".
     * @param {*} inputValue Nombre del usuario por el que filtrar
     * @param {*} fechaInicio Fecha inical para obtención del filtrado de los datos
     * @param {*} fechaFin Fecha inical para obtención del filtrado de los datos
     */
    chargeSitiosWebReferenciadosTable(inputValue, proyectoId, fechaInicio, fechaFin) {
        const that = this;
        let urlReferringWebsites = "";

        if (inputValue) {
            urlReferringWebsites = `${that.urlBase}/graphic?method=Referrers.getWebsites&force_api_session=1&idSubtable=1&period=day&date=${fechaInicio},${fechaFin}&segment=dimension3==${proyectoId};userId==${inputValue}`;
        } else {
            urlReferringWebsites = `${that.urlBase}/graphic?method=Referrers.getWebsites&force_api_session=1&idSubtable=1&period=day&date=${fechaInicio},${fechaFin}&segment=dimension3==${proyectoId}`;
        }

        return new Promise((resolve, reject) => {
            // Cargar de datos
            GnossPeticionAjax(
                urlReferringWebsites,
                null,
                true
            ).done(function (data) {
                // Variables a utilizar para pintado			
                let recursosReferringWebsites = new Object();

                // Carga de los datos			
                for (var i in data) {
                    for (var det in data[i]) {
                        if ((typeof data[i][det]["label"] !== "undefined")) {
                            if (recursosReferringWebsites[data[i][det]["label"]] !== undefined) {
                                recursosReferringWebsites[data[i][det]["label"]] += data[i][det]["nb_uniq_visitors"]
                            } else {
                                recursosReferringWebsites[data[i][det]["label"]] = data[i][det]["nb_uniq_visitors"]
                            }
                        }
                    }
                }
                // Create items array
                let items = Object.keys(recursosReferringWebsites).map(function (key) {
                    return [key, recursosReferringWebsites[key]];
                });

                // For top visitors we sort the array based on the second element
                items.sort(function (first, second) {
                    return second[1] - first[1];
                });
                // For top visitors we create a new array with only the first 5 items
                const nuevo = items.slice(0, 5);

                //Pintado de datos 
                let cont = 0;
                for (var i = 1; i < 6; i++) {
                    const tdNombre = $(`#top${i}Referr`);
                    const tdVisitas = $(`#top${i}ReferrNumber`);
                    tdNombre.text("");
                    tdVisitas.text("");
                }

                for (var key in nuevo) {
                    cont++;
                    const tdNombre = $(`#top${cont}Referr`);
                    const tdVisitas = $(`#top${cont}ReferrNumber`);

                    if (nuevo[key][0] != null && nuevo[key][1] != null) {
                        tdNombre.text(nuevo[key][0]);
                        tdVisitas.text(nuevo[key][1]);
                    }
                }
                resolve();
            }).fail(function (data) {
                // Mostrar error al tratar de crear una nueva página
                mostrarNotificacion("error", "Se ha producido un error al obtener los datos estadísticos de la comunidad. Inténtelo de nuevo más tarde.");
                reject();
            });
        });
    },


    /**
     * Método para realizar las peticiones para la obtención de los datos estadísticos de la tabla "Visitas Recientes"
     * Refiriendose a los usuarios que han visitado alguna pagina entre hoy y ayer.
     * @param {*} inputValue Nombre del usuario por el que filtrar
     * @param {*} fechaInicio Fecha inical para obtención del filtrado de los datos
     * @param {*} fechaFin Fecha inical para obtención del filtrado de los datos
     */
    chargeUsuariosRecientesTable(inputValue, proyectoId, fechaInicio, fechaFin) {
        const that = this;
        let urlLastHourUsers = "";

        if (inputValue) {
            urlLastHourUsers = `${that.urlBase}/graphic?&method=Live.getLastVisitsDetails&filter_limit=-1&period=day&date=last1&segment=dimension3==${proyectoId};userId==${inputValue}`;
        } else {
            urlLastHourUsers = `${that.urlBase}/graphic?&method=Live.getLastVisitsDetails&filter_limit=-1&period=day&date=last1&segment=dimension3==${proyectoId}`;
        }

        return new Promise((resolve, reject) => {
            // Cargar de datos
            GnossPeticionAjax(
                urlLastHourUsers,
                null,
                true
            ).done(function (data) {
                // Variables a utilizar para pintado			
                let userIdPaginaVisitada = new Object();

                // Carga de los datos			
                for (var i in data) {
                    if (data[i]["userId"]) {
                        var dateLastAction = data[i]["lastActionTimestamp"] * 1000; //This data is in secondds and we need  it in miliseconds
                        var end = Date.now();
                        var hourMili = 60 * 60000; //Users in the last hour
                        var difference = end - dateLastAction
                        if (Math.abs(difference) < hourMili) {
                            if (userIdPaginaVisitada[data[i]["userId"]] === undefined) {
                                var tam = data[i]["actionDetails"].length - 1
                                while (tam >= 0) {
                                    if ((data[i]["actionDetails"][tam]["pageTitle"] != null) && (userIdPaginaVisitada[data[i]["userId"]] === undefined)) {
                                        userIdPaginaVisitada[data[i]["userId"]] = data[i]["actionDetails"][tam]["pageTitle"]
                                    }
                                    tam = tam - 1
                                }
                            }
                        }
                    }
                }
                var contRows = 1;
                //Pintado de datos 
                const divTable = $("#div_table_last_users");
                divTable.empty();

                if (Object.keys(userIdPaginaVisitada).length != 0) {
                    const divTableLastUsers = `
					<div class="table-responsive matomoContainer__table">
					<h3 class="matomoContainer__header">Visitas recientes</h3>
					<hr>
						<table id="table_last_visitors" class="table table-bordered">
							<thead>
								<tr>
									<th>Usuario</th>
									<th>Página visitada</th>									
								</tr>
							</thead>
							<tbody>								
							</tbody>
						</table>
					</div>					
					`;
                    // Añadir la tabla 
                    divTable.html(divTableLastUsers);
                    //divTable.removeClass("d-none");
                    // Array con los datos a almacenar
                    let userItems = [];
                    let pageItems = [];

                    // Añadir filas					
                    for (var users in userIdPaginaVisitada) {

                        const aNombre = $("#aNombreLast" + contRows)[0];
                        if (aNombre != null) {
                            aNombre.setAttribute('href', that.urlPerfilPorUsuarioID[users]);
                        } else {
                            var a = $('<a>').attr('href', that.urlPerfilPorUsuarioID[users])
                                .text(that.nombrePerfilPorUsuarioId[users])
                                .attr('id', "aNombreLast" + contRows);

                            userItems.push(a);
                        }
                        pageItems.push(userIdPaginaVisitada[users]);
                        contRows++;
                    }
                    // Itera sobre los arrays y agregar filas a la tabla
                    for (let i = 0; i < userItems.length; i++) {
                        $("#table_last_visitors tbody").append(
                            $("<tr>").append(
                                $("<td>").append(userItems[i]),
                                $("<td>").text(pageItems[i])
                            )
                        );
                    }
                }
                resolve();
            }).fail(function (data) {
                // Mostrar error al tratar de crear una nueva página
                mostrarNotificacion("error", "Se ha producido un error al obtener los datos estadísticos de la comunidad. Inténtelo de nuevo más tarde.");
                reject();
            });
        });
    },


    handleDownloadCSVPages: function () {
        const that = this;
        var csv = "Page URL, Page name, Number of visits, Average time \n";
        var linea = "";
        let arrayIds = new Array()
        let items = Object.keys(that.paginasVisitas).map(function (key) {
            return [key, that.paginasVisitas[key]];
        });
        items.sort(function (first, second) {
            return second[1] - first[1]
        });

        for (var id in Object.fromEntries(items)) {
            arrayIds.push(id)
        }
        let uniqueChars = [...new Set(arrayIds)];
        for (var id = 0; id < uniqueChars.length; id++) {
            let url = uniqueChars[id];
            linea = ["\"" + url + "\"", "\"" + that.paginasUrl[url] + "\"", "\"" + that.paginasVisitas[url] + "\"", "\"" + that.paginasTiempoMedio[url] + "\""].join(",")
            linea += "\n"
            csv += linea;
        }
        return csv;
    },

    /**
     * Método para la descarga de datos vía fichero CSV
     * @returns 
     */
    handleDownloadCSV: function () {
        const that = this;

        let linea = "";
        let arrayIds = new Array()
        for (var id in that.recursos) {
            arrayIds.push(id);
        }
        for (var id in that.recursosDownload) {
            arrayIds.push(id);
        }
        for (var id in that.recursosComent) {
            arrayIds.push(id);
        }
        let uniqueChars = [...new Set(arrayIds)];

        var csv = "Resource id, Resource name, Resource URL, Number of votes, Number of downloads, Number of coments \n";
        for (var id in uniqueChars) {
            let idEstandar = uniqueChars[id].toString().replace("\"", "\"\"")
            if (idEstandar in that.recursos) {
                if (that.recursosDownloadNameVoted[idEstandar] && that.recursosDownloadURLVoted[idEstandar]) {
                    const downloadEstandar = that.recursosDownloadNameVoted[idEstandar].replace("\"", "\"\"");
                    const nombreEstandar = that.recursosDownloadURLVoted[idEstandar].replace("\"", "\"\"");
                    linea = ["\"" + idEstandar + "\"", "\"" + downloadEstandar + "\"", "\"" + nombreEstandar + "\"", that.recursos[idEstandar], that.recursosDownload[idEstandar], that.recursosComent[idEstandar]].join(",");
                    linea += "\n";
                }
            }
            else {
                if (idEstandar in that.recursosDownload) {
                    if (that.recursosDownloadName[idEstandar] && that.recursosDownloadURL[idEstandar]) {
                        var downloadEstandar = that.recursosDownloadName[idEstandar].replace("\"", "\"\"")
                        var nombreEstandar = that.recursosDownloadURL[idEstandar].replace("\"", "\"\"")
                        linea = ["\"" + idEstandar + "\"", "\"" + downloadEstandar + "\"", "\"" + nombreEstandar + "\"", that.recursos[idEstandar], that.recursosDownload[idEstandar], that.recursosComent[idEstandar]].join(",")
                        linea += "\n"
                    }
                }
                else {
                    if (idEstandar in that.recursosComent) {
                        if (that.recursosComentName[idEstandar] && that.recursosComentURL[idEstandar]) {
                            var downloadEstandar = that.recursosComentName[idEstandar].replace("\"", "\"\"")
                            var nombreEstandar = that.recursosComentURL[idEstandar].replace("\"", "\"\"")
                            linea = ["\"" + idEstandar + "\"", "\"" + downloadEstandar + "\"", "\"" + nombreEstandar + "\"", that.recursos[idEstandar], that.recursosDownload[idEstandar], that.recursosComent[idEstandar]].join(",")
                            linea += "\n"
                        }
                    }
                }
            }
            csv += linea;
        }
        return csv;
    },
    /**
     * Metodo para la descarga de datos sobre las descargas de recursos via fichero CSV
     * @returns
     */
    handleResourcesDownloadsCSV: function () {
        const that = this;
        let linea = "";
        let arrayIds = new Array();
        let items = Object.keys(that.recursosDownload).map(function (key) {
            return [key, that.recursosDownload[key]];
        });
        items.sort(function (first, second) {
            return second[1] - first[1]
        });

        for (var id in Object.fromEntries(items)) {
            arrayIds.push(id)
        }

        let uniqueChars = [...new Set(arrayIds)];

        var csv = "Resource id, Resource name, Resource URL, Number of downloads \n";
        for (var id in uniqueChars) {
            let idEstandar = uniqueChars[id].toString().replace("\"", "\"\"")
            if (that.recursosDownloadName[idEstandar] && that.recursosDownloadURL[idEstandar]) {
                var downloadEstandar = that.recursosDownloadName[idEstandar].replace("\"", "\"\"")
                var nombreEstandar = that.recursosDownloadURL[idEstandar].replace("\"", "\"\"")
                linea = ["\"" + idEstandar + "\"", "\"" + downloadEstandar + "\"", "\"" + nombreEstandar + "\"", that.recursosDownload[idEstandar]].join(",")
                linea += "\n"
            }
            csv += linea;
        }
        return csv;
    },

    /**
     * Metodo para la descarga de la seccion "Recursos mas visitados" via CSV
     */
    handleTopResourcesCSV: function () {
        const that = this;

        let linea = "";
        let arrayIds = new Array()
        let items = Object.keys(that.recursosVisitas).map(function (key) {
            return [key, that.recursosVisitas[key]];
        });
        items.sort(function (first, second) {
            return second[1] - first[1]
        });

        for (var id in Object.fromEntries(items)) {
            arrayIds.push(id)
        }

        let uniqueChars = [...new Set(arrayIds)];

        var csv = "Resource id, Resource name, Resource URL, Number of visits, Average time\n";
        for (var id in uniqueChars) {
            let recursoId = uniqueChars[id];
            if (that.recursosName[recursoId]) {
                linea = ["\"" + recursoId + "\"", "\"" + that.recursosName[recursoId] + "\"", "\"" + that.recursosUrl[recursoId] + "\"", that.recursosTiempoMedio[recursoId]].join(",");
                linea += "\n"
                csv += linea;
            }
        }
        return csv;
    },
    /**
     * Metodo para la descarga de datos sobre los votos en los recursos via CSV
     * @returns
     */
    handleResourcesVotesCSV: function () {
        const that = this;

        let linea = "";
        let arrayIds = new Array()
        let items = Object.keys(that.recursos).map(function (key) {
            return [key, that.recursos[key]];
        });
        items.sort(function (first, second) {
            return second[1] - first[1]
        });

        for (var id in Object.fromEntries(items)) {
            arrayIds.push(id)
        }

        let uniqueChars = [...new Set(arrayIds)];

        var csv = "Resource id, Resource name, Resource URL, Number of votes \n";
        for (var id in uniqueChars) {
            let idEstandar = uniqueChars[id].toString().replace("\"", "\"\"")
            if (that.recursosDownloadNameVoted[idEstandar] && that.recursosDownloadURLVoted[idEstandar]) {
                const downloadEstandar = that.recursosDownloadNameVoted[idEstandar].replace("\"", "\"\"");
                const nombreEstandar = that.recursosDownloadURLVoted[idEstandar].replace("\"", "\"\"");
                linea = ["\"" + idEstandar + "\"", "\"" + nombreEstandar + "\"", "\"" + downloadEstandar + "\"", that.recursos[idEstandar]].join(",");
                linea += "\n";
            }
            csv += linea;
        }
        return csv;
    },
    /**
     * Metodo para la descarga de datos sobre los comentarios en los recursos via CSV
     * @returns
     */
    handleResourcesCommentsCSV: function () {
        const that = this;

        let linea = "";
        let arrayIds = new Array()
        let items = Object.keys(that.recursosComent).map(function (key) {
            return [key, that.recursosComent[key]];
        });
        items.sort(function (first, second) {
            return second[1] - first[1]
        });

        for (var id in Object.fromEntries(items)) {
            arrayIds.push(id)
        }
        let uniqueChars = [...new Set(arrayIds)];

        var csv = "Resource id, Resource name, Resource URL, Number of coments \n";
        for (var id in uniqueChars) {
            let idEstandar = uniqueChars[id].toString().replace("\"", "\"\"")
            if (that.recursosComentName[idEstandar] && that.recursosComentURL[idEstandar]) {
                var downloadEstandar = that.recursosComentName[idEstandar].replace("\"", "\"\"")
                var nombreEstandar = that.recursosComentURL[idEstandar].replace("\"", "\"\"")
                linea = ["\"" + idEstandar + "\"", "\"" + downloadEstandar + "\"", "\"" + nombreEstandar + "\"", that.recursosComent[idEstandar]].join(",")
                linea += "\n"
            }
            csv += linea;
        }
        return csv;
    },
    /**
     * Método para realizar las peticiones para la obtención de los datos estadísticos del gráfico "Navegación Web".
     * @param {*} inputValue Nombre del usuario por el que filtrar
     * @param {*} fechaInicio Fecha inical para obtención del filtrado de los datos
     * @param {*} fechaFin Fecha inical para obtención del filtrado de los datos
     */
    chargeNavegacionWebGraph: function (inputValue, proyectoId, fechaInicio, fechaFin) {
        const that = this;

        let urlBrowserUsers = "";

        if (inputValue) {
            urlBrowserUsers = `${that.urlBase}/graphic?method=DevicesDetection.getBrowsers&filter_limit=-1&period=day&date=${fechaInicio},${fechaFin}&force_api_session=1&idSubtable=1&segment=dimension3==${proyectoId};userId==${inputValue}`;
        } else {
            urlBrowserUsers = `${that.urlBase}/graphic?method=DevicesDetection.getBrowsers&filter_limit=-1&period=day&date=${fechaInicio},${fechaFin}&segment=dimension3==${proyectoId}`;
        }

        return new Promise((resolve, reject) => {
            // Cargar de datos
            GnossPeticionAjax(
                urlBrowserUsers,
                null,
                true
            ).done(function (data) {
                // Variables a utilizar para pintado			
                let browser = new Map();

                // Carga de los datos			
                for (var i in data) {
                    if (data[i].length != 0) {
                        for (var n in data[i]) {
                            if (!isEmpty(data[i][n].label)) {
                                if (!browser.has(data[i][n].label)) {
                                    browser.set(data[i][n].label, data[i][n].nb_users);
                                } else {
                                    var oldValue = browser.get(data[i][n].label); //It already has the browser identified so we need its visits
                                    browser.set(data[i][n].label, data[i][n].nb_users + oldValue); //We add the visits to the browser that already exists in map
                                }
                            }

                        }
                    }
                }
                const arrayCountry = Array.from(browser.entries());
                const arrayQuantity = Array.from(browser.values());
                const browsers = arrayCountry.map(function (elem) { return `${elem[0]} (${elem[1]})` });
                const quantity = arrayQuantity.map(function (elem) { return elem });

                const chrt1 = $("#chartId_3")[0].getContext("2d");

                // Pintado de datos	(gráfico)		
                if (that.chartId1) {
                    that.chartId1.destroy();
                }

                // Graph Data
                const dataChartId1 = {
                    labels: browsers,
                    datasets: [{
                        data: quantity,
                        backgroundColor: [
                            '#4dc9f6',
                            '#f67019',
                            '#f53794',
                            '#537bc4',
                            '#acc236',
                            '#166a8f',
                            '#00a950',
                            '#58595b',
                            '#8549ba'
                        ],
                        borderWidth: 1,
                    }],
                };

                // Graph config
                const configChartId1 = {
                    data: dataChartId1,
                    type: "pie",
                    options: {
                        responsive: false,
                        maintainAspectRatio: false,
                    }
                };

                // Render graph
                that.chartId1 = new Chart(chrt1, configChartId1);
                // Mostrado centrado
                that.chartId1.canvas.style.display = 'inline-block';
                // Resolver la promesa OK
                resolve();
            }).fail(function (data) {
                // Mostrar error al tratar de crear una nueva página
                mostrarNotificacion("error", "Se ha producido un error al obtener los datos estadísticos de la comunidad. Inténtelo de nuevo más tarde.");
                // Resolver la promesa KO
                reject();
            });
        });
    },

    /**
     * Método para realizar las peticiones para la obtención de los datos estadísticos del gráfico "Plataforma".
     * @param {*} inputValue Nombre del usuario por el que filtrar
     * @param {*} fechaInicio Fecha inical para obtención del filtrado de los datos
     * @param {*} fechaFin Fecha inical para obtención del filtrado de los datos
     */
    chargePlataformaGraph: function (inputValue, proyectoId, fechaInicio, fechaFin) {
        const that = this;

        let urlOSUsers = "";

        if (inputValue) {
            urlOSUsers = `${that.urlBase}/graphic?&method=DevicesDetection.getOsFamilies&filter_limit=-1&period=day&date=${fechaInicio},${fechaFin}&segment=dimension3==${proyectoId};userId==${inputValue}`;
        } else {
            urlOSUsers = `${that.urlBase}/graphic?&method=DevicesDetection.getOsFamilies&filter_limit=-1&period=day&date=${fechaInicio},${fechaFin}&segment=dimension3==${proyectoId}`;
        }

        return new Promise((resolve, reject) => {
            // Cargar de datos
            GnossPeticionAjax(
                urlOSUsers,
                null,
                true
            ).done(function (data) {
                // Variables a utilizar para pintado			
                let OS = new Map();

                // Carga de los datos			
                for (var i in data) {
                    if (data[i].length != 0) {
                        for (var n in data[i]) {
                            if (!isEmpty(data[i][n].label)) {
                                if (!OS.has(data[i][n].label)) {
                                    OS.set(data[i][n].label, data[i][n].nb_users);
                                } else {
                                    var oldValue = OS.get(data[i][n].label);
                                    OS.set(data[i][n].label, data[i][n].nb_users + oldValue);
                                }
                            }

                        }
                    }
                }
                var arrayCountry = Array.from(OS.entries());
                var arrayQuantity = Array.from(OS.values());
                var browsers = arrayCountry.map(function (elem) { return `${elem[0]} (${elem[1]})` });
                var quantity = arrayQuantity.map(function (elem) { return elem });
                const chrt2 = $("#chartId_7")[0].getContext("2d");

                // Pintado de datos	(gráfico)		
                if (that.chartId2) {
                    that.chartId2.destroy();
                }

                // Graph Data
                const dataChartId2 = {
                    labels: browsers,
                    datasets: [{
                        data: quantity,
                        backgroundColor: [
                            '#4dc9f6',
                            '#f67019',
                            '#f53794',
                            '#537bc4',
                            '#acc236',
                            '#166a8f',
                            '#00a950',
                            '#58595b',
                            '#8549ba'
                        ],
                        borderWidth: 1,
                    }],
                };

                // Graph config
                const configChartId2 = {
                    data: dataChartId2,
                    type: "pie",
                    options: {
                        responsive: false,
                        maintainAspectRatio: false,
                    }
                }

                // Render graph
                that.chartId2 = new Chart(chrt2, configChartId2);
                // Contenido Centrado
                that.chartId2.canvas.style.display = 'inline-block';
                // Resolver la promesa OK
                resolve();
            }).fail(function (data) {
                // Mostrar error al tratar de crear una nueva página				
                mostrarNotificacion("error", "Se ha producido un error al obtener los datos estadísticos de la comunidad. Inténtelo de nuevo más tarde.");
                // Resolver la promesa KO
                reject();
            });

        });


    },

    /**
     * Método para realizar las peticiones para la obtención de los datos estadísticos del gráfico "Dispositivos".
     * @param {*} inputValue Nombre del usuario por el que filtrar
     * @param {*} fechaInicio Fecha inical para obtención del filtrado de los datos
     * @param {*} fechaFin Fecha inical para obtención del filtrado de los datos
     */
    chargeDispositivosGraph: function (inputValue, proyectoId, fechaInicio, fechaFin) {
        const that = this;

        let urlDevicesUsers = "";

        if (inputValue) {
            urlDevicesUsers = `${that.urlBase}/graphic?method=DevicesDetection.getType&filter_limit=-1&period=day&date=${fechaInicio},${fechaFin}&segment=dimension3==${proyectoId};userId==${inputValue}`;
        } else {
            urlDevicesUsers = `${that.urlBase}/graphic?method=DevicesDetection.getType&filter_limit=-1&period=day&date=${fechaInicio},${fechaFin}&segment=dimension3==${proyectoId}`;
        }

        return new Promise((resolve, reject) => {
            // Cargar de datos
            GnossPeticionAjax(
                urlDevicesUsers,
                null,
                true
            ).done(function (data) {
                // Variables a utilizar para pintado			
                let devices = new Map();

                // Carga de los datos			
                for (var i in data) {
                    if (data[i].length != 0) {
                        for (var n in data[i]) {
                            if (!isEmpty(data[i][n].label)) {
                                if (!devices.has(data[i][n].label)) {
                                    devices.set(data[i][n].label, data[i][n].nb_users);
                                } else {
                                    var oldValue = devices.get(data[i][n].label);
                                    devices.set(data[i][n].label, data[i][n].nb_users + oldValue);
                                }
                            }

                        }
                    }
                }

                const arrayDevice = Array.from(devices.entries());
                const arrayQuantity = Array.from(devices.values());
                var devicesInfo = arrayDevice.map(function (elem) { return `${elem[0]} (${elem[1]})` });
                var quantityInfo = arrayQuantity.map(function (elem) { return elem });

                const chrt3 = $("#chartId_8")[0].getContext("2d");
                // Pintado de datos	(gráfico)		
                if (that.chartId3) {
                    that.chartId3.destroy();
                }

                // Graph Data
                const dataChartId3 = {
                    labels: devicesInfo,
                    datasets: [{
                        data: quantityInfo,
                        backgroundColor: [
                            '#4dc9f6',
                            '#f67019',
                            '#f53794',
                            '#537bc4',
                            '#acc236',
                            '#166a8f',
                            '#00a950',
                            '#58595b',
                            '#8549ba'
                        ],
                        borderWidth: 1,
                    }],
                };

                // Graph config
                const configChartId3 = {
                    data: dataChartId3,
                    type: "pie",
                    options: {
                        responsive: false,
                        maintainAspectRatio: false,
                    }
                }

                // Render graph
                that.chartId3 = new Chart(chrt3, configChartId3);
                // Contenido Centrado
                that.chartId3.canvas.style.display = 'inline-block';
                resolve();

            }).fail(function (data) {
                // Mostrar error al tratar de crear una nueva página
                mostrarNotificacion("error", "Se ha producido un error al obtener los datos estadísticos de la comunidad. Inténtelo de nuevo más tarde.");
                reject();
            });
        });

    },


    /**
     * Método para realizar las peticiones para la obtención de los datos estadísticos del gráfico "Países".
     * @param {*} inputValue Nombre del usuario por el que filtrar
     * @param {*} fechaInicio Fecha inical para obtención del filtrado de los datos
     * @param {*} fechaFin Fecha inical para obtención del filtrado de los datos
     */
    chargePaisesGraph: function (inputValue, proyectoId, fechaInicio, fechaFin) {
        const that = this;

        let urlCountryUsers = "";

        if (inputValue) {
            urlCountryUsers = `${that.urlBase}/graphic?method=UserCountry.getCountry&filter_limit=-1&period=day&date=${fechaInicio},${fechaFin}&segment=dimension3==${proyectoId};userId==${inputValue}`;
        } else {
            urlCountryUsers = `${that.urlBase}/graphic?method=UserCountry.getCountry&filter_limit=-1&period=day&date=${fechaInicio},${fechaFin}&segment=dimension3==${proyectoId}`;
        }

        return new Promise((resolve, reject) => {
            // Cargar de datos
            GnossPeticionAjax(
                urlCountryUsers,
                null,
                true
            ).done(function (data) {
                // Variables a utilizar para pintado			
                let country = new Map();

                // Carga de los datos
                for (var i in data) {
                    if (data[i].length != 0) {
                        for (var n in data[i]) {
                            if (!isEmpty(data[i][n].label)) { //To not get the undefined eleemts 
                                if (!country.has(data[i][n].label)) {
                                    country.set(data[i][n].label, data[i][n].nb_visits); //We add the new country with its visits to the map
                                } else {
                                    var oldValue = country.get(data[i][n].label); //It already has a country so we need its visits
                                    country.set(data[i][n].label, data[i][n].nb_visits + oldValue); //We add the visits to the country that already exists in map
                                }
                            }
                        }
                    }
                }
                const arrayCountry = Array.from(country.entries());
                const arrayQuantity = Array.from(country.values());
                const countries = arrayCountry.map(function (elem) { return `${elem[0]} (${elem[1]})` });
                const quantity = arrayQuantity.map(function (elem) { return elem });
                const chrt4 = $("#chartId_2")[0].getContext("2d");

                if (that.chartId4) {
                    that.chartId4.destroy();
                }

                // Graph Data chartId4
                const dataChartId4 = {
                    labels: countries,
                    datasets: [{
                        data: quantity,
                        backgroundColor: [
                            '#4dc9f6',
                            '#f67019',
                            '#f53794',
                            '#537bc4',
                            '#acc236',
                            '#166a8f',
                            '#00a950',
                            '#58595b',
                            '#8549ba'
                        ],
                        borderWidth: 1,
                    }],
                };

                // Graph config
                const configChartId4 = {
                    data: dataChartId4,
                    type: "pie",
                    options: {
                        responsive: false,
                        maintainAspectRatio: false,
                    }
                }

                // Render graph
                that.chartId4 = new Chart(chrt4, configChartId4);
                // Contenido Centrado
                that.chartId4.canvas.style.display = 'inline-block';
                resolve();

            }).fail(function (data) {
                // Mostrar error al tratar de crear una nueva página
                mostrarNotificacion("error", "Se ha producido un error al obtener los datos estadísticos de la comunidad. Inténtelo de nuevo más tarde.");
                reject();
            });
        });

    },

    /**
     * Método para realizar las peticiones para la obtención de los datos estadísticos del gráfico "Visitas a la comunidad".
     * @param {*} inputValue Nombre del usuario por el que filtrar
     * @param {*} fechaInicio Fecha inical para obtención del filtrado de los datos
     * @param {*} fechaFin Fecha inical para obtención del filtrado de los datos
     */
    chargeVisitasComunidadGraph: function (inputValue, proyectoId, fechaInicio, fechaFin) {
        const that = this;

        let urlTotalVisits = "";
        let urlUniqueUsers = "";

        // Url visitas totales
        if (inputValue) {
            urlTotalVisits = `${that.urlBase}/graphic?&method=VisitsSummary.getVisits&filter_limit=-1&period=day&date=${fechaInicio},${fechaFin}&force_api_session=1&idSubtable=1&segment=dimension3==${proyectoId};userId==${inputValue}`;
        } else {
            urlTotalVisits = `${that.urlBase}/graphic?&method=VisitsSummary.getVisits&filter_limit=-1&period=day&date=${fechaInicio},${fechaFin}&segment=dimension3==${proyectoId}`;
        }
        // Url visitas únicas
        if (inputValue) {
            urlUniqueUsers = `${that.urlBase}/graphic?method=VisitsSummary.getUniqueVisitors&filter_limit=-1&period=day&date=${fechaInicio},${fechaFin}&force_api_session=1&idSubtable=1&segment=dimension3==${proyectoId};userId==${inputValue}`;
        } else {
            urlUniqueUsers = `${that.urlBase}/graphic?method=VisitsSummary.getUniqueVisitors&filter_limit=-1&period=day&date=${fechaInicio},${fechaFin}&segment=dimension3==${proyectoId}`;
        }


        return new Promise((resolve, reject) => {
            // Cargar de datos - Visitas totales
            GnossPeticionAjax(
                urlTotalVisits,
                null,
                true
            ).done(function (data) {
                // Variables a utilizar para pintado			
                let days_0 = new Array();
                let people_0 = new Array();

                const dataVisits = data;
                for (var i in dataVisits) {
                    days_0.push(i);
                    people_0.push(dataVisits[i]);
                }
                const dateVisits = days_0.map(function (elem) { return elem });
                const quantityVisits = people_0.map(function (elem) { return elem });

                // Obtención de los datos únicos
                GnossPeticionAjax(
                    urlUniqueUsers,
                    null,
                    true
                ).done(function (data) {
                    // Variables a utilizar para pintado
                    let days_4 = new Array();
                    let people_4 = new Array();

                    let dataVisitors = data;
                    for (var i in dataVisitors) {
                        days_4.push(i);
                        people_4.push(dataVisitors[i]);
                    }

                    const dateVisitors = days_4.map(function (elem) { return elem });
                    const quantityVisitors = people_4.map(function (elem) { return elem });
                    const chrt = $("#chartId_0_line")[0].getContext("2d");


                    //If there is an instance of the graphic destroy it to avoid problems
                    if (that.chartIdTotalVisits) {
                        that.chartIdTotalVisits.destroy();
                    }

                    that.chartIdTotalVisits = new Chart(chrt, {
                        type: 'line',
                        data: {
                            datasets: [{
                                label: 'Usuarios',
                                fill: true,
                                data: quantityVisitors,
                                backgroundColor: ['lightblue', 'lightblue', 'lightblue', 'lightblue', 'lightblue', 'lightblue'],
                                borderColor: ['blue', 'blue', 'blue', 'blue', 'blue', 'blue'],
                                borderWidth: 1,

                            }, {
                                label: 'Visitas',
                                fill: true,
                                data: quantityVisits,
                                backgroundColor: ['lightgreen', 'lightgreen', 'lightgreen', 'lightgreen', 'lightgreen', 'lightgreen'],
                                borderColor: ['green', 'green', 'green', 'green', 'green', 'green'],
                                borderWidth: 1,
                            }],
                            labels: dateVisitors
                        },
                        options: {
                            responsive: false,
                            maintainAspectRatio: false,
                        }
                    });
                    // Centrado
                    that.chartIdTotalVisits.canvas.style.display = 'inline-block';
                    resolve();
                }).fail(function (data) {
                    // Mostrar error al tratar de crear una nueva página
                    mostrarNotificacion("error", "Se ha producido un error al obtener los datos estadísticos de la comunidad. Inténtelo de nuevo más tarde.");
                    reject();
                });
            }).fail(function (data) {
                // Mostrar error al tratar de crear una nueva página
                mostrarNotificacion("error", "Se ha producido un error al obtener los datos estadísticos de la comunidad. Inténtelo de nuevo más tarde.");
                reject();
            });
        });

    },
}


//Auxiliar functions
function isEmpty(value) {
    return (value == null || (typeof value === "string" && value.trim().length === 0));
}

function isNotEmpty(value) {
    return (value != null && (typeof value === "string" && value.trim().length !== 0));
}

function isEmptyReturn(value) {
    return (value == null || (typeof value === "string" && value.trim().length === 0));
}
function filterUndefined(arrayEntry) {
    var arrayExit = [];
    for (var e in arrayEntry) { //We filter the undefined data in the array
        if (!isEmpty(arrayEntry[e][0])) {
            arrayExit.push(arrayEntry[e]);
        }
    }
    return arrayExit;
}
function myFunction(num) {
    return num;
}
function str_pad_left(string, pad, length) { return (new Array(length + 1).join(pad) + string).slice(-length); }


