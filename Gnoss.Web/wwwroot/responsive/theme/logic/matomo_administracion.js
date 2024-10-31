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
    triggerEvents: function(){
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
		this.parellelBtPagesId = "parellelBtPages";
		this.parellelBtPages = $(`#${this.parellelBtPagesId}`);

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
		this.paginas = new Object();
		this.paginasTiempoTotal = new Object();
		this.paginasTiempoMedio = new Object();

		this.idAsociadoConNombre = new Object();	
		this.nombreAsociadoUrl = new Object();	

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
			that.handleSelectAutocompleteItem(dataName);
		});

		// Botón/es para confirmar la eliminación de un filtrado activo de usuario       
        configEventByClassName(`${that.removeUserFilterClassName}`, function(element){
            const $jqueryElement = $(element);
            $jqueryElement.off().on("click", function(){
				// Eliminar filtros activos
				that.handleSelectAutocompleteItem("", true);
			});	                        
        });
		
		// Click para la descarga de CSV de "Datos de recursos"
		this.parellelBt.on("click", function(){
			const csv = that.handleDownloadCSV();
			
			const blob = new Blob([csv], { type: 'text/csv' });
			const url = window.URL.createObjectURL(blob)
			const a = document.createElement('a') 
			a.setAttribute('href', url)
			a.setAttribute('download', 'ResourcesData.csv');
			a.click(); 
		});

		this.parellelBtPages.on("click", function(){
			const csvPages = that.handleDownloadCSVPages();

			const blobPages = new Blob([csvPages], { type: 'text/csv' });
			const urlPages = window.URL.createObjectURL(blobPages)
			const aPages = document.createElement('a')
			aPages.setAttribute('href', urlPages)
			aPages.setAttribute('download', 'PagesData.csv');
			aPages.click();
		});


		// ComboBox de cambio de fecha para actualización de los datos (Fecha inicio / Fecha fin)
		this.inputFechaInicio.on("change", function(){
			that.chargeTablesAndGraphs();
		});

		this.inputFechaFin.on("change", function(){
			that.chargeTablesAndGraphs();
		});			
    },

    /**
     * Método para la selección realizada de un item haciendo uso de AutoComplete 
     * @param {string} input: Input que ha lanzado el autocomplete
     * @param {string} dataName: Nombre o dato nombre del usuario seleccionado del panel autoComplete     
	 * @param {bool} removeFilter : Dato booleano que indicará si es necesario eliminar el filtro.
     */
    handleSelectAutocompleteItem: function(dataName, removeFilter = false){
		const that = this;	
		let filterUserValue = "";

		if (removeFilter == false){
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
			filterUserValue = dataName.trim();

			// Pintar el usuario que se utilizará para cargar los datos mediante "chargeTable/chargeGraphs"		
			that.txtBuscarUsuariosHack.val(filterUserValue);
		}else{			
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
	chargeTablesAndGraphs: function(){
		const that = this;

		loadingMostrar(undefined,"Cargando los datos de la comunidad. Por favor, espere ...");

		// Obtener el valor que se haya metido en la búsqueda del usuario para un nombre (Debe ser Autocomplete)
		const inputValue = that.txtBuscarUsuariosHack.val().trim();

		const graphsPromises = that.chargeGraphs(inputValue);
		const tablePromises = that.chargeTables(inputValue);

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
	chargeTables: function(inputValue){		
		const that = this;	
					
		const promises = [
			that.chargeResumenDeVisitasTable(inputValue),
			that.chargePaginasDestacadasTable(inputValue, this.inputFechaInicio.val(), this.inputFechaFin.val()),
			that.chargeVotosDeRecursosTable(inputValue, this.inputFechaInicio.val(), this.inputFechaFin.val()),
			that.chargeComentariosEnRecursosTable(inputValue, this.inputFechaInicio.val(), this.inputFechaFin.val()),
			that.chargeBusquedasDestacadasTable(inputValue, this.inputFechaInicio.val(), this.inputFechaFin.val()),
			that.chargeDescargaDeRecursosTable(inputValue, this.inputFechaInicio.val(), this.inputFechaFin.val()),
			that.chargeVisitasDestacadasVisitasRecientesTable(inputValue, this.inputFechaInicio.val(), this.inputFechaFin.val()),
			that.chargeSitiosWebReferenciadosTable(inputValue, this.inputFechaInicio.val(), this.inputFechaFin.val()),
			that.chargeUsuariosRecientesTable(inputValue, this.inputFechaInicio.val(), this.inputFechaFin.val()),			
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
	chargeGraphs: function(inputValue = "") {
		const that = this;
	
		const promises = [
			that.chargeVisitasComunidadGraph(inputValue, this.inputFechaInicio.val(), this.inputFechaFin.val()),
			that.chargeNavegacionWebGraph(inputValue, this.inputFechaInicio.val(), this.inputFechaFin.val()),
			that.chargePlataformaGraph(inputValue, this.inputFechaInicio.val(), this.inputFechaFin.val()),
			that.chargeDispositivosGraph(inputValue, this.inputFechaInicio.val(), this.inputFechaFin.val()),
			that.chargePaisesGraph(inputValue, this.inputFechaInicio.val(), this.inputFechaFin.val()),			  	
		];
	
		// Devolver una promesa que se resuelve cuando todas las promesas en el array se resuelven
		return Promise.all(promises);
	  },	

	/**
	 * Método para realizar las peticiones para la obtención de los datos estadísticos de la tabla "Resumen de visitas".
	 * @param {*} inputValue Valor introducido en el input de búsqueda correspondiente con el usuario buscado
	 */	
	chargeResumenDeVisitasTable(inputValue){
		const that = this;
		let urlTotalVisits = "";
		let urlTotalVisitsMonth = "";
		let urlTotalUsers = "";
		let urlTotalUsersMonth = "";

		/*************** RESUMEN DE VISITAS **************/		
		// Construcción de URLs
		if (inputValue) {
			urlTotalVisits = `${that.urlBase}/graphic?method=VisitsSummary.getVisits&period=month&date=last12&force_api_session=1&idSubtable=1&segment=userId==${inputValue}`;
			urlTotalVisitsMonth = `${that.urlBase}/graphic?&method=VisitsSummary.getVisits&period=day&date=last7&force_api_session=1&idSubtable=1&segment=userId==${inputValue}`;
			urlTotalUsers = `${that.urlBase}/graphic?&method=VisitsSummary.getUniqueVisitors&period=month&date=last12&force_api_session=1&idSubtable=1&segment=userId==${inputValue}`;
			urlTotalUsersMonth = `${that.urlBase}/graphic?method=VisitsSummary.getUniqueVisitors&period=day&date=last7&force_api_session=1&idSubtable=1&segment=userId==${inputValue}`;
		} else {
			urlTotalVisits = `${that.urlBase}/graphic?method=VisitsSummary.getVisits&period=month&date=last12`;			
			urlTotalVisitsMonth = `${that.urlBase}/graphic?&method=VisitsSummary.getVisits&period=day&date=last7`;
			urlTotalUsers = `${that.urlBase}/graphic?&method=VisitsSummary.getUniqueVisitors&period=month&date=last12`;
			urlTotalUsersMonth = `${that.urlBase}/graphic?method=VisitsSummary.getUniqueVisitors&period=day&date=last7`;
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
	resumenDeVisitasTableFetchVisitasDias: function(urlTotalVisits){
		const that = this;

        // 1.1- Resumen de visitas (Visitas Días)  
		return new Promise((resolve, reject) => {
			GnossPeticionAjax(                
				urlTotalVisits,
				null,
				true
			).done(function (data) {
				// Variables a utilizar para pintado
				let lastYear= 0;
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
	resumenDeVisitasTableFetchVisitasUsuarios: function(urlTotalVisitsMonth){
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
				let day= 0;		

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
	resumenDeVisitasTableFetchVisitasDeUsuarios: function(urlTotalUsers){
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
	resumenDeVisitasTableFetchTotalVisitasUsuariosMes: function(urlTotalUsersMonth){
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
	chargePaginasDestacadasTable(inputValue, fechaInicio, fechaFin){
		const that = this;
		let urltotalTimeSpendByAllUsers = "";

		if (inputValue) {
			urltotalTimeSpendByAllUsers = `${that.urlBase}/graphic?&method=Actions.getPageTitles&filter_limit=5&period=day&date=${fechaInicio},${fechaFin}&segment=userId==${inputValue}`;
		} else {
			urltotalTimeSpendByAllUsers = `${that.urlBase}/graphic?&method=Actions.getPageTitles&filter_limit=5&period=day&date=${fechaInicio},${fechaFin}`;
		}

		return new Promise((resolve, reject) => {
			// Cargar de datos
			GnossPeticionAjax(                
				urltotalTimeSpendByAllUsers,
				null,
				true
			).done(function (data) {
				// Variables a utilizar para pintado			
				that.paginas = new Object();
				that.paginasTiempoTotal = new Object();
				that.paginasTiempoMedio = new Object();	

				// Datos obtenidos								
				for (var i in data) {
					for (var pag in data[i]) {
						if (typeof data[i][pag]["label"] !== "undefined") {
							//YA esta la key
							if (that.paginas[data[i][pag]["label"]] !== undefined) {
								that.paginas[data[i][pag]["label"]] += data[i][pag]["nb_visits"];
								that.paginasTiempoTotal[data[i][pag]["label"]] += data[i][pag]["sum_time_spent"];
							} else {
								that.paginas[data[i][pag]["label"]] = data[i][pag]["nb_visits"];
								that.paginasTiempoTotal[data[i][pag]["label"]] = data[i][pag]["sum_time_spent"];
							}						
						}					
					}
				}
				
				for (var id in that.paginas) {
					let time  = that.paginasTiempoTotal[id] / that.paginas[id];
					//var time = paginasTiempoTotal[nuevo[key][0]] / paginas[id][1]
					var hours = Math.floor(time / 3600).toString();
					time = time - hours * 3600;
					var minutes = Math.floor(time / 60).toString();
					var seconds = (time - minutes * 60);
					var secondsRound = Math.floor(seconds).toString()
					var finalTime = hours.padStart(2, '0') + ":" + minutes.padStart(2, '0') + ":" + secondsRound.padStart(2, '0');
					that.paginasTiempoMedio[id] = finalTime;
				}	
				
				// Create items array
				var items = Object.keys(that.paginas).map(function (key) {
					return [key, that.paginas[key]];
				});

				// Sort the array based on the second element
				items.sort(function (first, second) {
					return second[1] - first[1];
				});

				// Create a new array with only the first 5 items
				var nuevo = items.slice(0, 5);			

				// Pintado de datos por cada línea
				let cont = 0;
				for (var key in nuevo) {
					cont++;
					const tdNombre = $(`#top${cont}Name`);
					const tdVisitas = $(`#top${cont}Visits`);
					const tdTime = $(`#top${cont}VisitsTime`);				
					if (nuevo[key][0] != null && nuevo[key][1]!=null) {
						tdNombre.text(nuevo[key][0]);
						tdVisitas.text(nuevo[key][1]);
						let time = that.paginasTiempoTotal[nuevo[key][0]] / nuevo[key][1]
						const hours = Math.floor(time / 3600).toString();
						time = time - hours * 3600;
						const minutes = Math.floor(time / 60).toString();
						const seconds = (time - minutes * 60)
						const secondsRound = Math.floor(seconds).toString()
						let finalTime = hours.padStart(2, '0') + ":" + minutes.padStart(2, '0') + ":" + secondsRound.padStart(2, '0');
						tdTime.text(finalTime);
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
	chargeVotosDeRecursosTable(inputValue, fechaInicio, fechaFin){
		const that = this;
		let urltotalTimeSpendByAllUsers = "";

		if (inputValue) {
			urltotalTimeSpendByAllUsers = `${that.urlBase}/graphic-downloads?method=Live.getLastVisitsDetails&force_api_session=1&idSubtable=1&period=day&date=${fechaInicio},${fechaFin}&segment=eventName==Me+gusta;userId==${inputValue}`;
		} else {
			urltotalTimeSpendByAllUsers = `${that.urlBase}/graphic-downloads?method=Live.getLastVisitsDetails&segment=eventName==Me+gusta&force_api_session=1&idSubtable=1&period=day&date=${fechaInicio},${fechaFin}`;
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
						if ((typeof data[i]["actionDetails"][det] !== "undefined") && (data[i]["actionDetails"][det]["eventName"] == "Me gusta") && (data[i]["actionDetails"][det]["dimension4"] !=null)) {
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
	chargeComentariosEnRecursosTable(inputValue, fechaInicio, fechaFin){
		const that = this;
		let urlMostComentedElements = "";

		if (inputValue) {
			urlMostComentedElements = `${that.urlBase}/graphic-downloads?&method=Live.getLastVisitsDetails&force_api_session=1&idSubtable=1&period=day&date=${fechaInicio},${fechaFin}&segment=eventName==Comentario;userId==${inputValue}`;
		} else {
			urlMostComentedElements = `${that.urlBase}/graphic-downloads?&method=Live.getLastVisitsDetails&segment=eventName==Comentario&force_api_session=1&idSubtable=1&period=day&date=${fechaInicio},${fechaFin}`;
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
						if ((typeof data[i]["actionDetails"][det] !== "undefined") && (data[i]["actionDetails"][det]["eventAction"] == "Comentario")) {
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
	chargeBusquedasDestacadasTable(inputValue, fechaInicio, fechaFin){
		const that = this;		
		let urlMostSearchedElements = "";

		if (inputValue) {
			urlMostSearchedElements = `${that.urlBase}/graphic?&method=Live.getLastVisitsDetails&force_api_session=1&idSubtable=1&period=day&date=${fechaInicio},${fechaFin}`;			
		} else {
			urlMostSearchedElements = `${that.urlBase}/graphic?method=Live.getLastVisitsDetails&force_api_session=1&idSubtable=1&period=day&date=${fechaInicio},${fechaFin}`;			
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
	chargeDescargaDeRecursosTable(inputValue, fechaInicio, fechaFin){
		const that = this;
		let urlMostDownloadedElements = "";	

		if (inputValue) {
			urlMostDownloadedElements = `${that.urlBase}/graphic-downloads?&method=Live.getLastVisitsDetails&force_api_session=1&idSubtable=1&period=day&date=${fechaInicio},${fechaFin}&segment=actionUrl=@download;userId==${inputValue}`;
		} else {
			urlMostDownloadedElements = `${that.urlBase}/graphic-downloads?method=Live.getLastVisitsDetails&segment=actionUrl=@download&force_api_session=1&idSubtable=1&period=day&date=${fechaInicio},${fechaFin}`;
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


	/**
	 * Método para realizar las peticiones para la obtención de los datos estadísticos de las tablas "Visitas destacadas y Visitas recientes".
	 * @param {*} inputValue Nombre del usuario por el que filtrar
	 * @param {*} fechaInicio Fecha inical para obtención del filtrado de los datos
	 * @param {*} fechaFin Fecha inical para obtención del filtrado de los datos
	 */	
	chargeVisitasDestacadasVisitasRecientesTable(inputValue, fechaInicio, fechaFin){
		const that = this;
		const urlTopVisitors = `${that.urlBase}/graphic-users?module=API&method=UserId.getUsers&format=json&force_api_session=1&idSubtable=1&period=day&date=${fechaInicio},${fechaFin}`;
	
		return new Promise((resolve, reject) => {
			// Cargar de datos
			GnossPeticionAjax(                
				urlTopVisitors,
				null,
				true
			).done(function (data) {
				// Variables a utilizar para pintado			
				let recursosTopVisitors = new Object();
				that.nombreAsociadoUrl = new Object();
				let segundosVisita = new Object();
				that.idAsociadoConNombre = new Object();
							
				// Carga de los datos			
				for (var i in data) {
					for (var det in data[i]) {
						if ((data[i][det]["nombrePerfil"] != null)) {
							if (recursosTopVisitors[data[i][det]["nombrePerfil"]] !== undefined) {
								recursosTopVisitors[data[i][det]["nombrePerfil"]] += data[i][det]["nb_uniq_visitors"]
								segundosVisita[data[i][det]["nombrePerfil"]] += data[i][det]["sum_visit_length"]
							} else {
								recursosTopVisitors[data[i][det]["nombrePerfil"]] = data[i][det]["nb_uniq_visitors"]
								segundosVisita[data[i][det]["nombrePerfil"]] = data[i][det]["sum_visit_length"]
							}
							if (that.nombreAsociadoUrl[data[i][det]["nombrePerfil"]] === undefined) {
								that.nombreAsociadoUrl[data[i][det]["nombrePerfil"]] = data[i][det]["urlPerfil"]
							}
							if (that.idAsociadoConNombre[data[i][det]["idPerfil"]] === undefined) {
								that.idAsociadoConNombre[data[i][det]["idPerfil"]] = data[i][det]["nombrePerfil"]
							}
						}
		
					}
				}
				// Create items array
				var items = Object.keys(segundosVisita).map(function (key) {
					return [key, segundosVisita[key]];
				});
				//for recent we get the last 5
				var itemsRecent = Object.fromEntries(
					Object.entries(segundosVisita).slice(Math.max(Object.entries(segundosVisita).length - 5, 0))
				);
				// For top visitors we sort the array based on the second element
				items.sort(function (first, second) {
					return second[1] - first[1];
				});
				// For top visitors we create a new array with only the first 5 items
				const nuevo = items.slice(0, 5);	

				// Pintado de "Visitas destacadas"       	
				let cont = 0;
				for (var i = 1; i < 6; i++) { 
					const tdNombre = $(`#top${i}Visit`);
					const tdVisitas = $(`#top${i}VisitNumber`);
					const tdTime = $(`#top${i}Time`);				
					tdNombre.text("");
					tdVisitas.text("");
					tdTime.text("");
				}	
				for (var key in nuevo) {
					cont++;
					const tdNombre = $(`#top${cont}Visit`);
					const tdVisitas = $(`#top${cont}VisitNumber`);
					const tdTime = $(`#top${cont}Time`);	

					if (nuevo[key][0] != null && nuevo[key][1] != null) {
						const aNombre = $(`#aNombreTopV${cont}`);					
						if (aNombre.length > 0) {
							aNombre.attr('href', that.nombreAsociadoUrl[nuevo[key][0]]);
						} else {
							const a = $('<a>').attr('href', that.nombreAsociadoUrl[nuevo[key][0]])
											.text(nuevo[key][0])
											.attr('id', 'aNombreTopV' + cont);
						
							tdNombre.append(a);
						}					

						let time = segundosVisita[nuevo[key][0]];
						const hours = Math.floor(time / 3600);
						time = time - hours * 3600;
						const minutes = Math.floor(time / 60);
						const seconds = time - minutes * 60;					
						const finalTime = str_pad_left(hours, '0', 2) + ':' + str_pad_left(minutes, '0', 2) + ':' + str_pad_left(seconds, '0', 2);
						tdTime.text(finalTime);
						tdVisitas.text(recursosTopVisitors[nuevo[key][0]]);
					}
				}	

				// Pintado de "Visitas recientes"			
				let contRecent = 0;
				for (var i = 1; i < 6; i++) {
					const tdNombre = $(`#top${i}VisitRecent`);
					const tdVisitas = $(`#top${i}VisitNumberRecent`);
					const tdTime = $(`#top${i}TimeRecent`);
					tdNombre.text("");
					tdVisitas.text("");
					tdTime.text("");
				}	
				
				for (var key in itemsRecent) {
					contRecent++;
					const tdNombre = $(`#top${contRecent}VisitRecent`);
					const tdVisitas = $(`#top${contRecent}VisitNumberRecent`);
					const tdTime = $(`#top${contRecent}TimeRecent`);

					if (itemsRecent[key] != null && key != null) {
						const aNombre = $(`#aNombreRecV${contRecent}`);

						if (aNombre.length > 0) {
							aNombre.attr('href', that.nombreAsociadoUrl[key]);
						} else {
							var a = $('<a>').attr('href', that.nombreAsociadoUrl[key])
											.text(key)
											.attr('id', `aNombreRecV${contRecent}`);
						
							tdNombre.append(a);
						}
											
						let time = itemsRecent[key];
						const hours = Math.floor(time / 3600);
						time = time - hours * 3600;
						const minutes = Math.floor(time / 60);
						const seconds = time - minutes * 60;
						const finalTime = str_pad_left(hours, '0', 2) + ':' + str_pad_left(minutes, '0', 2) + ':' + str_pad_left(seconds, '0', 2);
						tdTime.text(finalTime);
						tdVisitas.text(recursosTopVisitors[key]);
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
	 * Método para realizar las peticiones para la obtención de los datos estadísticos de la tabla "Sitios web más referenciados".
	 * @param {*} inputValue Nombre del usuario por el que filtrar
	 * @param {*} fechaInicio Fecha inical para obtención del filtrado de los datos
	 * @param {*} fechaFin Fecha inical para obtención del filtrado de los datos
	 */	
	chargeSitiosWebReferenciadosTable(inputValue, fechaInicio, fechaFin){
		const that = this;
		let urlReferringWebsites = "";	

		if (inputValue) {
			urlReferringWebsites = `${that.urlBase}/graphic?method=Referrers.getWebsites&force_api_session=1&idSubtable=1&period=day&date=${fechaInicio},${fechaFin}&segment=userId==${inputValue}`;
		} else {
			urlReferringWebsites = `${that.urlBase}/graphic?method=Referrers.getWebsites&force_api_session=1&idSubtable=1&period=day&date=${fechaInicio},${fechaFin}`;
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
	 * Método para realizar las peticiones para la obtención de los datos estadísticos de la tabla "Sitios web más referenciados".
	 * @param {*} inputValue Nombre del usuario por el que filtrar
	 * @param {*} fechaInicio Fecha inical para obtención del filtrado de los datos
	 * @param {*} fechaFin Fecha inical para obtención del filtrado de los datos
	 */	
	chargeUsuariosRecientesTable(inputValue, fechaInicio, fechaFin){
		const that = this;
		let urlLastHourUsers = "";	

		if (inputValue) {
			urlLastHourUsers = `${that.urlBase}/graphic?&method=Live.getLastVisitsDetails&filter_limit=5&period=day&date=last1&segment=userId==${inputValue}`;
		} else {
			urlLastHourUsers = `${that.urlBase}/graphic?&method=Live.getLastVisitsDetails&filter_limit=5&period=day&date=last1`;
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
						var dateLastAction = data[i]["lastActionTimestamp"]*1000; //This data is in secondds and we need  it in miliseconds
						var end = Date.now();
						var hourMili= 60 * 60000; //Users in the last hour
						var difference = end - dateLastAction
						if (Math.abs(difference) < hourMili) {
							if (userIdPaginaVisitada[data[i]["userId"]] === undefined) {
								var tam = data[i]["actionDetails"].length - 1
								while (tam>=0) {			
									if ((data[i]["actionDetails"][tam]["pageTitle"] != null) && (userIdPaginaVisitada[data[i]["userId"]]===undefined) ) {
										userIdPaginaVisitada[data[i]["userId"]] = data[i]["actionDetails"][tam]["pageTitle"]
									}
									tam=tam-1
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
					divTable.removeClass("d-none");
					// Array con los datos a almacenar
					let userItems = [];
					let pageItems = [];

					// Añadir filas					
					for (var users in userIdPaginaVisitada) {

						const aNombre = $("#aNombreLast" + contRows)[0]; 
						if (aNombre != null) {
							aNombre.setAttribute('href', that.nombreAsociadoUrl[that.idAsociadoConNombre[users]]);
						} else {
							var a = $('<a>').attr('href', that.nombreAsociadoUrl[that.idAsociadoConNombre[users]])
											.text(that.idAsociadoConNombre[users])
											.attr('id', "aNombreLast" + contRows);
							
							userItems.push(a);
						}					
						pageItems.push(userIdPaginaVisitada[users]);
						contRows++;
					}					
					// Itera sobre los arrays y agregar filas a la tabla
					for (let i = 0; i < userItems.length; i++) {					
						that.tableLastVisitors.append(					
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


	handleDownloadCSVPages: function(){
		const that = this;
		var csv = "Page name, Number of visits, Average time \n";
		var linea = "";
		let arrayIds = new Array()
		for (var id in that.paginas) {
			arrayIds.push(id)
		}
		let uniqueChars = [...new Set(arrayIds)];
		for (var id in uniqueChars) {
			if (uniqueChars[id] in that.paginas) {
				linea = ["\"" + uniqueChars[id] + "\"", "\"" + that.paginas[uniqueChars[id]] + "\"", "\"" + that.paginasTiempoMedio[uniqueChars[id]] + "\""].join(",")
				linea += "\n"
			}
				
			csv += linea;
		}
		return csv;
	},

	/**
	 * Método para la descarga de datos vía fichero CSV
	 * @returns 
	 */
	handleDownloadCSV: function(){
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
	 * Método para realizar las peticiones para la obtención de los datos estadísticos del gráfico "Navegación Web".
	 * @param {*} inputValue Nombre del usuario por el que filtrar
	 * @param {*} fechaInicio Fecha inical para obtención del filtrado de los datos
	 * @param {*} fechaFin Fecha inical para obtención del filtrado de los datos
	 */
	chargeNavegacionWebGraph: function(inputValue, fechaInicio, fechaFin){
		const that = this;
		
		let urlBrowserUsers = "";	

		if (inputValue) {
			urlBrowserUsers = `${that.urlBase}/graphic?method=DevicesDetection.getBrowsers&period=day&date=${fechaInicio},${fechaFin}&force_api_session=1&idSubtable=1&segment=userId==${inputValue}`;
		} else {
			urlBrowserUsers = `${that.urlBase}/graphic?method=DevicesDetection.getBrowsers&period=day&date=${fechaInicio},${fechaFin}`;
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
	chargePlataformaGraph: function(inputValue, fechaInicio, fechaFin){
		const that = this;
		
		let urlOSUsers = "";	

		if (inputValue) {
			urlOSUsers = `${that.urlBase}/graphic?&method=DevicesDetection.getOsFamilies&period=day&date=${fechaInicio},${fechaFin}&segment=userId==${inputValue}`;
		} else {
			urlOSUsers = `${that.urlBase}/graphic?&method=DevicesDetection.getOsFamilies&period=day&date=${fechaInicio},${fechaFin}`;
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
	chargeDispositivosGraph: function(inputValue, fechaInicio, fechaFin){
		const that = this;
		
		let urlDevicesUsers = "";	

		if (inputValue) {
			urlDevicesUsers = `${that.urlBase}/graphic?method=DevicesDetection.getType&period=day&date=${fechaInicio},${fechaFin}&segment=userId==${inputValue}`;
		} else {
			urlDevicesUsers = `${that.urlBase}/graphic?method=DevicesDetection.getType&period=day&date=${fechaInicio},${fechaFin}`;
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
	chargePaisesGraph: function(inputValue, fechaInicio, fechaFin){
		const that = this;
		
		let urlCountryUsers = "";	

		if (inputValue) {
			urlCountryUsers = `${that.urlBase}/graphic?method=UserCountry.getCountry&period=day&date=${fechaInicio},${fechaFin}&segment=userId==${inputValue}`;
		} else {
			urlCountryUsers = `${that.urlBase}/graphic?method=UserCountry.getCountry&period=day&date=${fechaInicio},${fechaFin}`;
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
	chargeVisitasComunidadGraph: function(inputValue, fechaInicio, fechaFin){
		const that = this;
		
		let urlTotalVisits = "";
		let urlUniqueUsers = "";

		// Url visitas totales
		if (inputValue) {
			urlTotalVisits = `${that.urlBase}/graphic?&method=VisitsSummary.getVisits&period=day&date=${fechaInicio},${fechaFin}&force_api_session=1&idSubtable=1&segment=userId==${inputValue}`;
		} else {
			urlTotalVisits = `${that.urlBase}/graphic?&method=VisitsSummary.getVisits&period=day&date=${fechaInicio},${fechaFin}`;
		}
		// Url visitas únicas
		if (inputValue) {
			urlUniqueUsers = `${that.urlBase}/graphic?method=VisitsSummary.getUniqueVisitors&period=day&date=${fechaInicio},${fechaFin}&force_api_session=1&idSubtable=1&segment=userId==${inputValue}`;
		} else {
			urlUniqueUsers = `${that.urlBase}/graphic?method=VisitsSummary.getUniqueVisitors&period=day&date=${fechaInicio},${fechaFin}`;
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


