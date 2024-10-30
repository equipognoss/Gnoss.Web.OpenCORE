/**
 ***************************************************************************************
 * Logica / Operativas de comportamiento JS para la sección de Grafos de conocimiento de la Comunidad del DevTools
 * *************************************************************************************
 */

/**
  * Operativa para la gestión y configuración de Facetas de la comunidad
  */
const operativaGestionFacetas = {

    /**
    * Inicializar operativa
    */
     init: function (pParams) {
        this.pParams = pParams;
        this.config(pParams);
        this.configEvents();
        this.configRutas(); 
        this.triggerEvents();            
    },

    /**
     * Lanzar comportamientos u operativas necesarias para el funcionamiento de la sección
     */
    triggerEvents: function(){
        const that = this;           

        // Indicar el nº de items existente
        that.handleCheckNumberOfFacetas();
        
        // Inicializar comportamientos de select2
        comportamientoInicial.iniciarSelects2(); 

        // Operativa multiIdioma
        // Parámetros para la operativa multiIdioma (helpers.js)
        this.operativaMultiIdiomaParams = {
            // Nº máximo de pestañas con idiomas a mostrar. Los demás quedarán ocultos
            numIdiomasVisibles: 3,
            // Establecer 1 tab por cada input (true, false) - False es la forma vieja
            useOnlyOneTab: true,
            panContenidoMultiIdiomaClassName: "panContenidoMultiIdioma",
            // No permitir padding bottom y si padding top
            allowPaddingBottom: false,                                              
        };  

        // Configurar arrastrar facetas        
        setupBasicSortableList(that.facetaListContainerId, that.sortableFacetaIconClassName, undefined, undefined, undefined, that.handleSaveFacetasSorted);    


        // Inicializar operativa multiIdioma
        operativaMultiIdioma.init(this.operativaMultiIdiomaParams);    
         
        that.setupInputsFacetsForAutocomplete();
    },   

    /**
     * Configuración de las rutas de acciones necesarias para hacer peticiones a backend
     */
    configRutas: function (pParams) {
        // Url para editar un certificado
        this.urlBase = refineURL();
        this.urlSave = `${this.urlBase}/save`;
        this.urlAddNewFilter = `${this.urlBase}/new-filtro`;
 
        // Flag para controlar borrado
        this.confirmDeleteFaceta = false;
        // Flag para controlar posibles errores
        this.errorsBeforeSaving = false;
        
    },  

    /*
    * Inicializar elementos de la vista
    * */
    config: function (pParams) {        
        // Contenedor modal
        this.modalContainer = $("#modal-container");
        this.modalFacetaClassName = "modal-faceta";
        this.modalDeleteFaceta = $("#modal-delete-faceta");

        // Tab de idioma de la página para cambiar la visualización en el idioma deseado de las facetas
        this.tabIdiomaItem = $(".tabIdiomaItem "); 
        // Cada una de las labels (Nombre literal) en los diferentes idiomas (por `data-languageitem`)
        this.labelLanguageComponentClassName = "language-component";

        // Botón para añadir una nueva faceta
        this.linkAddNewFaceta = $(".linkAddNewFaceta");
        // Botón para añadir una nueva faceta propuesta
        this.linkAddNewFacetaPropuesta = $(".linkAddNewFacetaPropuesta");
        // Icono para arrastrar una faceta
        this.sortableFacetaIconClassName = 'js-component-sortable-faceta';

        // Buscador de parametros
        this.txtBuscarFaceta = $("#txtBuscarFaceta");
        this.txtBuscarFacetaPropuesta = $("#txtBuscarFacetaPropuesta");
        // Contenedor de facetas
        this.facetaListContainerId = 'id-added-faceta-list';
        this.facetaListContainer = $(`#${this.facetaListContainerId}`);
        // Nombre de la fila de cada faceta
        this.facetaListItemClassName = "faceta-row";
        this.facetaSuggestListItemClassName = "faceta-row-suggest";
        // Botón de editar faceta
        this.btnEditFacetaClassName = "btnEditFaceta";                
        // Botón para eliminar una faceta
        this.btnDeleteFacetaClassName = "btnDeleteFaceta";
        // Botón para confirmar la eliminación de una faceta
        this.btnConfirmDeleteFacetaClassName = "btnConfirmDeleteFaceta";
        // Contador de número de items existentes
        this.numFaceta = $("#numFacetas");        

        /* Inputs del modal de creación de Faceta */
        // Botón para guardar faceta
        this.btnSaveFacetaClassName = "btnSaveFaceta";
        // Botón para añadir un filtro a la faceta
        this.linkAddFiltroFacetaClassName = "linkAddFiltroFaceta";
        // Contenedor del listado de filtros de la faceta
        this.facetaFilterListContainerClassName = "id-added-faceta-filter-list";
        // Fila de cada filtro de una faceta
        this.filaFiltroClassName = "filter-row";
        // Botón para editar un filtro
        this.btnEditFilterClassName = "btnEditFilter";
        // Botón para eliminar un filtro
        this.btnDeleteFilterClassName= "btnDeleteFilter";
        // Input del nombre del filtro de una faceta
        this.txtNombreFiltroClassName = "txtNombreFiltro";
        // Input de la condición del filtro de una faceta
        this.txtCondicionFiltroClassName = "txtCondicionFiltro";
        // Input oculto y nombre de la clase donde se guardarán los objetos de conocimiento seleccionados
        this.txtObjetosConocimientoHiddenClassName = "ObjetosConocimiento";
        // Cada item de objetos de conocimiento del select
        this.objetoConocimientoItemClassName = "objetoConocimientoItem"; 
        // Panel que contiene las opciones (Sí / No ) de reciprocidad de una faceta. También contiene el input con su valor
        this.panReciprocaClassName = "panReciproca";
        // Panel que contiene el valor de la reciprocidad de una faceta. (Puede estar oculto)
        this.panReciprocaDetailClassName = "panReciprocaDetail";
        // Input de la reciprocidad de la faceta
        this.txtReciprocaClassName = "Reciprocidad";
        // Html donde se muestra el nombre del filtro
        this.componentFilterNameClassName = "component-filterName";
        // Panel contenedor de objetos de conocimiento de la faceta
        this.panObjetosConocimientoClassName = "panObjetosConocimiento";

        /* Inputs checkbox dentro del modal para editar los datos de la faceta */
        this.txtClaveFaceta = "ClaveFaceta";
        this.txtFacetaFilterClassName = "facetaFilter";
        this.componentFacetaClassName = "component-faceta";
        this.componentFacetaNameClassName = "component-name";
        this.txtFacetaTypeClassName = "Type";
        this.chkReciprocaClassName = "chkReciproca";
        this.chkAutocompletarClassName = "chkAutocompletar";
        this.cmbSelectObjetosConocimientoClassName = "cmbSelectObjetosConocimiento";
        this.contenedorObjetosConocimientoClassName = "contenedorObjetosConocimiento"
        // Botón para quitar un item de tipo label
        this.tagRemoveGrupoClassName = "tag-remove-group";
        this.tagRemoveObjetoConocimientoClassName = "tag-remove-objeto-conocimiento";
        this.cmbSelectObjetosConocimientoClassName = "cmbSelectObjetosConocimiento";
        this.cmbPresentacionClassName = "cmbPresentacion";
        this.cmbComportamientoClassName = "cmbComportamiento";
        this.cmbAlgoritmoTransformacionClassName = "cmbAlgoritmoTransformacion";
        this.cmbDisenyoClassName = "cmbDisenyo";
        this.txtNumElementosVisiblesClassName = "NumElementosVisibles";
        this.chkOcultaEnFacetasClassName = "chkOcultaEnFacetas";        
        this.chkComportamientoORClassName = "chkComportamientoOR";
        this.chkOcultarEnFiltrosClassName = "chkOcultarEnFiltros";
        this.chkPriorizarOrdenResultadosClassName = "chkPriorizarOrdenResultados";
        this.txtTabPrivacidadGruposClassName = "TabPrivacidadGrupos";
        this.TabValoresPrivacidadGruposClassName = "TabValoresPrivacidadGrupos"
        this.txtCondicionClassName = "condicion";        
        this.btnAddConditionClassName = "btnAddConditionPanCondition";
        this.inputAddConditionClassName = "inputAddConditionFaceta";
        this.btnRemoveConditionItemClassName = "tag-remove-condicion";
        this.conditionListClassName = "panListadoCondiciones";
        this.auxCondicionesSeleccionadasClassName = "auxCondicionesSeleccionadas";
        this.chkInmutableClassName = "chkInmutable";

        // Modal para mover una faceta
        this.modalMoveFacetaId = "modal-move-faceta";
        this.modalMoveFaceta = $(`#${this.modalMoveFacetaId}`);
        // Select de la faceta a elegir para mover
        this.cmbListaFacetasClassName = "cmbListaFacetas";
        this.cmbListaFacetas = $(`.${this.cmbListaFacetasClassName}`);
        // Título de la faceta del modal que se desea mover
        this.modalTitleMoveFacetaClassName = `move-faceta-title`;
        this.modalTitleMoveFaceta = $(`.${this.modalTitleMoveFacetaClassName}`);
        // Botón para guardar movimiento de la faceta
        this.btnSaveFacetaMoveClassName = "btnSaveFacetaMove";
        this.btnSaveFacetaMove = $(`.${this.btnSaveFacetaMoveClassName}`);
        this.btnMoveFacetaClassName = "btnMoveFaceta";   
        // Backup de los items ordenados en caso de que no se proceda al guardado correcto
        this.backupArraySortableListItems = undefined;     
       
        // Fila de la faceta que está siendo editada
        this.filaFaceta = undefined;
        // Flags para confirmar la eliminación de una faceta
        this.confirmDeleteFaceta = false;
       
    },   

    /**
     * Configuración de eventos de elementos del Dom (Botones, Inputs...)     
     */
    configEvents: function (pParams) {
        const that = this;

        // Comportamientos del modal container 
        this.modalContainer.on('show.bs.modal', (e) => {
            // Aparición del modal
            // Establecer quién ha disparado el modal para poder cambiar sus atributos una vez se guarden los grupos desde el modal
            that.triggerModalContainer = $(e.relatedTarget);                              
        })
        .on('hide.bs.modal', (e) => {
            // Acción de ocultar el modal
        })
        .on('hidden.bs.modal', (e) => {
            // Vaciar el modal
            resetearModalContainer(); 
        });

        // Comportamientos del modal de editar/crear
        configEventByClassName(`${that.modalFacetaClassName}`, function(element){
            const $modal = $(element);
            $modal
            .on('show.bs.modal', (e) => {
                // Aparición del modal
                that.triggerModalContainer = $(e.relatedTarget);
            })
            .on('hide.bs.modal', (e) => {
               // Eliminar la fila si es de reciente creación y no se ha guardado
               if (that.filaFaceta.hasClass("newFaceta")){
                    that.filaFaceta.remove();                    
                    that.handleCheckNumberOfFacetas();
               }
            }); 
        }); 

        // Comportamientos del modal de eliminar faceta
        that.modalDeleteFaceta.
        on('show.bs.modal', (e) => {
            // Aparición del modal
            that.triggerModalContainer = $(e.relatedTarget);
        })
        .on('hide.bs.modal', (e) => {
            // Acción de ocultar el modal
            // Si el modal se cierra sin confirmar borrado, desactiva el borrado
            if (that.confirmDeleteFaceta == false){
                that.handleSetDeleteFaceta(false);                
            }       
        }); 
           
        // Botón para editar la faceta
        configEventByClassName(`${that.btnEditFacetaClassName}`, function(element){
            const $jqueryElement = $(element);
            $jqueryElement.off().on("click", function(){
                that.filaFaceta = $(this).closest(`.${that.facetaListItemClassName}`);
                // Indicar en la row que se está modificando
                that.filaFaceta.addClass("modified");
            });	                        
        });

        // Búsquedas de facetas
        this.txtBuscarFaceta.off().on("keyup", function (event) {
            const input = $(this);
            clearTimeout(that.timer);
            that.timer = setTimeout(function () {                                                                        
                // Acción de buscar / filtrar
                that.handleSearchFaceta(input);                                         
            }, 500);
        });        
        this.txtBuscarFacetaPropuesta.off().on("keyup", function (event) {
            const input = $(this);
            clearTimeout(that.timer);
            that.timer = setTimeout(function () {
                // Acción de buscar / filtrar
                that.handleSearchFacetaPropuesta(input);
            }, 500);
        });        
        // Botón/es para confirmar la eliminación de una faceta (Modal -> Sí)        
        configEventByClassName(`${that.btnConfirmDeleteFacetaClassName}`, function(element){
            const $jqueryElement = $(element);
            $jqueryElement.off().on("click", function(){
                that.confirmDeleteFaceta = true;
                that.handleDeleteFaceta();              
            });	                        
        });  
        
        // Botón/es para añadir un tipo de faceta a la comunidad
        this.linkAddNewFaceta.on("click", function(){
            const type = $(this).data("type");
            const url = $(this).data("url");
            that.handleLoadCreateFaceta(type, url);            
        });   
        // Botón/es para añadir un tipo de faceta propuesta a la comunidad
        this.linkAddNewFacetaPropuesta.on("click", function () {
            const id = $(this).data("suggest");
            const url = $(this).data("url");
            that.handleLoadCreateFacetaPropuesta(id, url);
        });

        // Botón del modal para poder mover una faceta de forma "manual" justo debajo de la elegida
        this.btnSaveFacetaMove.on("click", function () {
            that.handleMoveFacetManually();
        });        

        // Botón para eliminar una faceta
        configEventByClassName(`${that.btnDeleteFacetaClassName}`, function(element){
            const $btnDelete = $(element);
            $btnDelete.off().on("click", function(){                
                // Fila correspondiente a la faceta eliminada
                that.filaFaceta = $btnDelete.closest(`.${that.facetaListItemClassName}`);                
                // Marcar el parámetro como "Eliminar" a modo informativo al usuario
                that.handleSetDeleteFaceta(true);
            });	                        
        }); 
        
        // Botón para añadir un filtro a una faceta
        configEventByClassName(`${that.linkAddFiltroFacetaClassName}`, function(element){
            const $btnAddFiltro = $(element);
            $btnAddFiltro.off().on("click", function(){                  
                that.handleAddFilter($btnAddFiltro);                              
            });	                        
        }); 

        // Botón para eliminar un filtro de una faceta
        configEventByClassName(`${that.btnDeleteFilterClassName}`, function(element){
            const $btnDelete = $(element);
            $btnDelete.off().on("click", function(){                
                const btnDeleted = $(this);                    
                that.handleDeleteFilter(btnDeleted, true);
            });	                        
        }); 
        
        // Check de propiedad recíproca        
        configEventByClassName(`${that.chkReciprocaClassName}`, function(element){
            const $chkReciproca = $(element);
            $chkReciproca.off().on("change", function(){                                                          
                that.handleChangeReciproca($chkReciproca);
            });	                        
        }); 

        // Configurar los inputs para autocompletar de Grupos -> Privacidad de faceta        
        configEventByClassName(`${that.txtTabPrivacidadGruposClassName}`, function(element){
            const $txtTabPrivacidadGrupos = $(element);
            if ($txtTabPrivacidadGrupos.length > 0){
                that.handleSetupAutoCompleteForInput($txtTabPrivacidadGrupos);
            }    
        });  
                       
        // Botón (X) para poder eliminar grupos desde el Tag                           
        configEventByClassName(`${that.tagRemoveGrupoClassName}`, function(element){
            const $jqueryElement = $(element);
            $jqueryElement.off().on("click", function(){                 
                const $itemRemoved = $jqueryElement.parent().parent();                                                                                                              
                that.handleClickBorrarTagItem($itemRemoved);                
            });	                        
        });  
        
        // Botón (X) para poder eliminar objetos de conocimiento desde el Tag                           
        configEventByClassName(`${that.tagRemoveObjetoConocimientoClassName}`, function(element){
            const $jqueryElement = $(element);
            $jqueryElement.off().on("click", function(){                 
                const $itemRemoved = $jqueryElement.parent().parent();                                                                                                              
                that.handleDeleteObjetoConocimiento($itemRemoved);                
            });	                        
        });            
        
        // Click sobre un objeto de conocimiento para ser seleccionado                
        configEventByClassName(`${that.cmbSelectObjetosConocimientoClassName}`, function(element){
            const $jqueryElement = $(element);
            $jqueryElement.off().on("select2:select", function(e){                                 
                const itemSelected = e;
                that.handleObjetoConocimientoSelected(e);                
            });	                        
        });
        
        // Input del nombre del filtro de una faceta. Mostrar aviso de que no esté vacío.
        configEventByClassName(`${that.txtNombreFiltroClassName}`, function(element){
            const $input = $(element);
            $input.on("blur", function(){                                                 
                comprobarInputNoVacio($input, true, false, "El nombre del filtro de una faceta no puede estar vacío.", 0);
            });	                        
        });

        // Input del nombre del filtro de una faceta. Actualizar el input correspondiente en la fila
        configEventByClassName(`${that.txtNombreFiltroClassName}`, function(element){
            const $input = $(element);
            $input.on("keyup", function(e){                                                 
                // Controlar el cambio de nombre del filtro
                that.handleKeyupForFilterName($input);
            });	                        
        });

        // Input del nombre del filtro de una faceta. Mostrar aviso de que no esté vacío.
        configEventByClassName(`${that.txtReciprocaClassName}`, function(element){
            const $input = $(element);
            $input.on("blur", function(){                                                 
                comprobarInputNoVacio($input, true, false, "El valor de la faceta recíproca no puede estar vacío.", 0);
            });	                        
        });

        // Botón para guardar facetas        
        configEventByClassName(`${that.btnSaveFacetaClassName}`, function(element){
            const $saveButton = $(element);
            $saveButton.on("click", function(){   
                that.handleSaveFacetas();                                                              
            });	                        
        });

        // Tab de cada uno de los idiomas disponibles para cambiar la visualización en la lista de cláusulas
        this.tabIdiomaItem.off().on("click", function(){            
            that.handleViewFacetaLanguageInfo();                        
            // Aplicar la búsqueda para el idioma activo
            that.timer = setTimeout(function () {                                                                                                        
                that.handleSearchFaceta(that.txtBuscarFaceta);
            }, 500);            
        });  
        
        // Input de "Faceta". Controlar el cambio para actualizar el item correspondiente en el listado        
        configEventByClassName(`${that.txtFacetaFilterClassName}`, function(element){
            const $input = $(element);
            $input.on("input", function(){                                                 
                that.handleFacetaInputChanged($input);
            });	                        
        });

        // Botón (X) para poder eliminar condiciones                           
        configEventByClassName(`${that.btnRemoveConditionItemClassName}`, function(element){
            const $jqueryElement = $(element);
            $jqueryElement.off().on("click", function(){                 
                const $itemRemoved = $jqueryElement.parent().parent();                                                                                                              
                that.handleDeleteCondicion($itemRemoved);                
            });	                        
        });  
              
        // Enter sobre la faceta para añadir la condición
        configEventByClassName(`${that.inputAddConditionClassName}`, function(element){                        
            const $input = $(element);     
            $input.on("keyup", function(event){ 
                if(event.key == "Enter"){
                    event.preventDefault();
                    that.handleCrearCondicion($input);
                    $input[0].value = "";
                } 
            });
        });

        // Botón para añadir la condición a la faceta
        configEventByClassName(`${that.btnAddConditionClassName}`, function(element){                        
            const $addButton = $(element);     
            $addButton.on("click", function(event){        
                // Obtener el input de la condición según la fila                         
                const input = $addButton.siblings(`.${that.inputAddConditionClassName}`);                  
                that.handleCrearCondicion(input);                
                input.val("");
            });
        });

        // Botón para añadir la condición a la faceta
        configEventByClassName(`${that.btnMoveFacetaClassName}`, function(element){                        
            const $jqueryElement = $(element);
            $jqueryElement.off().on("click", function(){
                that.filaFaceta = $(this).closest(`.${that.facetaListItemClassName}`);                                                                
                that.handleSetFacetTitleInModal();
            });	 
        });
            
    },
    /**
     ****************************************************************************************************************************
     * Acciones a realizar, funciones sobre las facetas
     * **************************************************************************************************************************
     */ 

    /**
     * Método para asignar el nombre de la faceta al modal de "Mover" si se desea establecer un nombre     
     */
     handleSetFacetTitleInModal: function(){
        const that = this;

        // Resetear posibles options que se han ocultado previamente
        // Obtén todas las opciones dentro del select
        that.cmbListaFacetas.find('option').prop("disabled", false);        
        
        if (that.filaFaceta == undefined){
            return;
        }
        // Nombre de la faceta seleccionada
        const facetName = that.filaFaceta.find(`.${that.componentFacetaNameClassName}:not(.d-none)`).text().trim();
        that.modalTitleMoveFaceta.text(facetName);
        const facetSelectedKey = that.filaFaceta.prop("id");
        const facetSelectedOption = that.cmbListaFacetas.find(`option[name="${facetSelectedKey}"]`);                
        // Deshabilita la opción (opcional, puedes comentar esta línea si solo quieres ocultarla visualmente)
        facetSelectedOption.prop('disabled', true);        

        // Actualiza el select2 después de modificar las opciones
        that.cmbListaFacetas.trigger('change');        
     },

     
     /**
      * Método para comprobar y ejecutar la ordenación o movimiento de una faceta debajo de la seleccionada. Si ha habido un error, revertirá el movimiento realizado
      * @returns 
      */
     handleMoveFacetManually: function(){     
        const that = this;
        // En that.filaFaceta se guarda la faceta que se ha seleccionado y se desesa mover
        if (that.filaFaceta == undefined){
            return;
        }

        const facetSelectedId = that.filaFaceta.prop("id");
        // Faceta seleccionada para posicionar justo debajo la filaFaceta
        const facetMoveBelowId = that.cmbListaFacetas.find(':selected').attr("name");

        // Comprobar que la faceta seleccionada y la faceta destino son diferentes
        if (facetSelectedId === facetMoveBelowId){
            mostrarNotificacion("error", "No es posible mover la faceta debajo de sí misma. Por favor, selecciona otra faceta.");
            return;
        }
                
        // Obtén la instancia de Sortable para la lista
        const sortableList = Sortable.get(document.getElementById(`${that.facetaListContainerId}`));           
                       
        // Buscar los índices
        const sortableListItems = document.getElementsByClassName(`${that.facetaListItemClassName}`);

        let indexToMove = -1;
        let newIndex = -1;
        
        for (let i = 0; i < sortableListItems.length; i++) {
          const currentId = sortableListItems[i].id;
        
          if (currentId === facetSelectedId) {
            indexToMove = i;
          }
        
          if (currentId === facetMoveBelowId) {
            newIndex = i;
          }
        
          if (indexToMove !== -1 && newIndex !== -1) {
            // Ambos índices han sido encontrados, puedes salir del bucle
            break;
          }
        }
         
        const arraySortableListItems = sortableList.toArray();
        // Copia de los items sin mover por si se produce un error
        that.backupArraySortableListItems = arraySortableListItems;
        // Remueve el elemento de la posición original
        const itemToMove = arraySortableListItems.splice(indexToMove, 1)[0];

        // Inserta el elemento en la nueva posición
        arraySortableListItems.splice(newIndex, 0, itemToMove);

        // Actualiza la representación visual de la lista
        sortableList.sort(arraySortableListItems);        

        // Proceder al guardado
        that.handleSaveFacetasSorted(function(isOk, data){                        
            if (isOk == requestFinishResult.ok){                
                // Datos guardados correctamente al haber ordenado los items. Cerrar modal de mover items   
                dismissVistaModal(that.modalMoveFaceta);   
                // Actualizar el select del modal
                that.handeleUpdateFacetListInModal();
            }else{
                // Error al guardar items, restaurar el orden previo de los items
                const errorMessage = data != undefined ? data : "Se ha producido un error al mover la faceta. Por favor, inténtalo de nuevo más tarde.";
                mostrarNotificacion("error", errorMessage);
                // Restaurar el movimiento de la faceta por el error
                sortableList.sort(that.backupArraySortableListItems);                                
            }
        });        
     },

     /**
      * Método para actualizar la lista de facetas en base al actual listado
      */
     handeleUpdateFacetListInModal: function(){
        const that = this;
        // Html de los optionItems que se actualizará cuando haya cambios
        let facetOptionHtml = '';


        $(`.${that.facetaListItemClassName}`).each(function() {
            const id = $(this).prop('id');
            const dataName = $(this).data('name');
            const dataValue = $(this).data('value');        
            facetOptionHtml += `
                <option class="faceta-item"
                    value="${id}"
                    name="${dataValue}">
                    ${dataName}                                        
                </option>`;                    
        });
        
        // Agrega las opciones generadas dinámicamente al select        
        this.cmbListaFacetas.html(facetOptionHtml)        
        comportamientoInicial.iniciarSelects2();
     },

    /**
     * Método que se ejecuta al influir en el input faceta. Genera una lista de valores de autocompletado
     * @param {*} inputFaceta 
     */
    setupInputsFacetsForAutocomplete: function () { 
        const that = this;

        const inputFaceta = $(`.${that.txtClaveFaceta}`);
        inputFaceta.autocomplete(
            null,
            {
                url: $('#inpt_urlServicioAutocompletar').val() + "/AutoCompletarOntologia",
                delay: 0,
                scroll: false,
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
                    proyecto: $('#inpt_proyID').val()
                }
            }
        );


        //Comportamiento resultado cuando se selecciona una resultado de autocomplete 
        inputFaceta.result(function (event, data, formatted) {             
            $(this).val(data[0]);             
        });        
        
    },
     
    
    /**
     * Método para añadir una condicion al contenedor
     * 
     */
    handleCrearCondicion: function(input){
        const that = this;

        const condicion = input.val().trim();
        if(condicion.length > 0)
        {      
            
            // Split por "|" por si han metido la condición de seguido
            const conditionsArray = condicion.split("|");

            conditionsArray.forEach(conditionItem => {                            
                // Crear el item y añadirlo al contenedor
                let item = '';
                item += '<div class="tag" id="'+ conditionItem +'">';
                    item += '<div class="tag-wrap">';
                        item += '<span class="tag-text">' + conditionItem + '</span>';
                        item += "<span class=\"tag-remove tag-remove-condicion material-icons remove\">close</span>";                
                    item += '</div>';
                item += '</div>';        
                    
                // Contenedor de items donde se añadirá el nuevo item seleccionado para su visualización            
                const currentContenedorCondiciones =  that.filaFaceta.find(`.${that.conditionListClassName}`); 
                currentContenedorCondiciones.append(item);

                // Input oculto donde se añadirá el nuevo item seleccionado
                const inputHack = that.filaFaceta.find(`.${that.auxCondicionesSeleccionadasClassName}`);                
                inputHack.val(inputHack.val() + conditionItem + '|');
            }); 
        }
    },
    

    /**
     * Método para quitar la condicion una vez se ha pulsado en (x)
     * 
     */
    handleDeleteCondicion: function(itemRemoved){
        const that = this;
        // Etiqueta del item eliminado        
        //const key = itemRemoved.data("key"); 
        const condicion = itemRemoved.prop("id");
        //const data = itemRemoved.find(".tag-text").html().trim();
        // Panel de objetos de conocimiento que está siendo editado
        const currentPanelObjetosConocimiento = that.filaFaceta.find(`.${"panCondiciones"}`);    
        // Input oculto donde se añadirá el nuevo item seleccionado
        const inputHack = currentPanelObjetosConocimiento.find(`.${"auxCondicionesSeleccionadas"}`);              
        // Obtener los items los items que hay
        const itemsId = inputHack.val().split("|");
        // Eliminar el id seleccionado
        itemsId.splice( $.inArray(condicion, itemsId), 1 );
        // Pasarle los datos de los grupos actualizados al input hidden
        inputHack.val(itemsId.join("|")); 

        // Eliminar el tag/item del panel de tags
        itemRemoved.remove();

    },

    /**
     * Método que se ejecuta cuando cambia la faceta. Necesario para actualizar la información de la faceta en la lista de facetas
     * @param {*} input 
     */
    handleFacetaInputChanged: function(input){
        const that = this;

        // Faceta a actualizar
        const facetaRow = input.closest(`.${that.facetaListItemClassName}`);
        // Nombre de la faceta en el listado de facetas
        const componentFaceta = facetaRow.find(`.${that.componentFacetaClassName}`);
        // Actualizar el contenido de la faceta
        componentFaceta.html(input.val().trim());        
    },

    /**
     * Método para eliminar filtros activos de búsquedas de recursos multimedia en la sección de filtros activos
     * @param {} filtro 
     */
    LimpiarFiltroComponentes: function(filtro){
        // Mostrar loading
        loadingMostrar();
        // Página a cargar
        let nuevaPagina = "";
        indice = document.location.href.indexOf("?");
        nuevaPagina = document.location.href.substr(0, indice + 1);
        // Recarga de la página la página
        document.location = nuevaPagina;        
    },
    
    
    /**
     * Método que se ejecuta al cambiar el valor de un input. Esta función está enlazada desde la vista, pero enlazada desde el helpers.js ya que 
     * se utiliza para ejecutar un comportamiento sobre inputs dinámicos generados de forma dinámica mediante la operativaMultiIdioma.
     * @param {*} event 
     */
    handleOnInputTabNameChanged: function(event){
        const that = this;
        // Input que ha sido actualizado
        const input = $(event.currentTarget);
        // Idioma del input que ha sido actualizado
        const language = input.data("language");
        // Valor actual del input
        const value = input.val().trim();

        // Faceta a actualizar
        const facetaRow = input.closest(`.${that.facetaListItemClassName}`);
        // Nombre a actualizar en el listado de facetas        
        let componentNameFaceta = undefined ;
        
        if (language != undefined){
            componentNameFaceta = facetaRow.find(`.${that.componentFacetaNameClassName}`).filter(`[data-languageitem='${language}']`);
        }else{
            componentNameFaceta = facetaRow.find(`.${that.componentFacetaNameClassName}`);
        }
                
        // Actualizar el contenido de la faceta
        componentNameFaceta.html(value);
    },

    /**
     * Método para guardar las facetas sólo cuando se ha procedido a su ordenación.
     */
    handleSaveFacetasSorted: function(completion = undefined){
        const that = operativaGestionFacetas;

        that.handleSaveFacetas(function(isOk, data){
            if (isOk != requestFinishResult.ok){                
                // Datos guardados al haber ordenado los items. No hacer nada                
            }            
            if (typeof completion === 'function') {
                completion(isOk, data)
            }

        });

        

    },
    

    /**
     * Método para cambiar el nombre del menu item cuando se esté editando
     * @param {jqueryElement} input Input donde se está introduciendo el nombre del filtro
     */
     handleKeyupForFilterName: function(input){                        
        const that = this;
        // Detectar la fila a editar
        const filterRow = input.closest(`.${that.filaFiltroClassName}`);  
        const newFilterName = input.val();  
        // Obtener el id del input que se está modificando para modificar el label correspondiente y cambiar el nombre para acceder a su label
        const labelFilterName = filterRow.find(`.component-filterName`);
        // Asignar lo tecleado (Nuevo nombre del parámetro)
        labelFilterName.html(newFilterName.trim());        
    },    

    /**
     * Método para controlar la selección de un objeto de conocimiento (select2)
     * @param {jqueryElement} itemSelected Elemento seleccionado del select2 
     */
    handleObjetoConocimientoSelected: function(itemSelected){
        const that = this;
        // Etiqueta del item seleccionado
        const jqueryItemSelected = $(itemSelected.params.data.element);
        const key = String(jqueryItemSelected.data("key")).trim(); 
        if (key == "undefined"){
            return;
        }
        const id = String(jqueryItemSelected.data("value")).trim();
        const data = String(jqueryItemSelected.data("title")).trim();
        
        // 1- Marcarlo como disabled
        jqueryItemSelected.prop('disabled', true);
        // 2- Crear el tag y añadirlo al contenedor correspondiente de la filaFaceta
        let item = '';
        item += '<div data-key="'+ key +'" class="tag" id="'+ id +'">';
            item += '<div class="tag-wrap">';
                item += '<span class="tag-text">' + data + '</span>';
                item += "<span class=\"tag-remove tag-remove-objeto-conocimiento material-icons\">close</span>";                
            item += '</div>';
        item += '</div>';        
               
        // Contenedor de items donde se añadirá el nuevo item seleccionado para su visualización
        const currentContenedorObjetosConocimiento = that.filaFaceta.find(`.${that.contenedorObjetosConocimientoClassName}`);
        currentContenedorObjetosConocimiento.append(item);
        
        const currentPanelObjetosConocimiento = that.filaFaceta.find(`.${that.panObjetosConocimientoClassName}`);    
        // Input oculto donde se añadirá el nuevo item seleccionado
        const inputHack = currentPanelObjetosConocimiento.find("input[type=hidden]").first();
        // Añadido el id del item seleccionado al inputHack                
        inputHack.val(inputHack.val() + id + ',');
    },
    

    /**
     * Método para quitar el objeto de conocimiento una vez se ha pulsado en (x) del objeto de conocimiento. Volvería a estar disponible en el select
     * para volver a ser elegido si hiciera falta.
     * @param {jqueryElement} Tag correspondiente al elemento que ha sido eliminado y que debe ser activado de nuevo en el select2 de objetos de conocimiento
     */
    handleDeleteObjetoConocimiento: function(itemRemoved){
        const that = this;
        // Etiqueta del item eliminado        
        const key = itemRemoved.data("key"); 
        const id = itemRemoved.prop("id");
        const data = itemRemoved.find(".tag-text").html().trim();

        // Panel de objetos de conocimiento que está siendo editado
        const currentPanelObjetosConocimiento = that.filaFaceta.find(`.${that.panObjetosConocimientoClassName}`);    
        // Input oculto donde se añadirá el nuevo item seleccionado
        const inputHack = currentPanelObjetosConocimiento.find("input[type=hidden]").first();               
        // Obtener los items los items que hay
        const itemsId = inputHack.val().split(",");
        // Eliminar el id seleccionado
        itemsId.splice( $.inArray(id, itemsId), 1 );
        // Pasarle los datos de los grupos actualizados al input hidden
        inputHack.val(itemsId.join(",")); 

        // Select de Objetos de conocimiento de la faceta editada
        const cmbSelectObjetosConocimiento = currentPanelObjetosConocimiento.find(`.${that.cmbSelectObjetosConocimientoClassName}`);
        // Item del select que debe volver a activarse
        const selectItem = cmbSelectObjetosConocimiento.find(`[data-key='${key}']`);
        selectItem.prop('disabled', false);

        // Eliminar el tag/item del panel de tags
        itemRemoved.remove();
    },

    /**
     * Método para eliminar una faceta previa confirmación realizada desde el modal
     */
     handleDeleteFaceta: function(){
        const that = this;                  
        // 1 - Llamar al método para el guardado de facetas
        that.handleSaveFacetas(function(isOk, data){
            if (isOk == requestFinishResult.ok){                
                dismissVistaModal(that.modalDeleteFaceta);
                // 3 - Ocultar loading
                loadingOcultar();                
                // 4 - Ocultar la fila que se desea eliminar
                that.filaFaceta.addClass("d-none");
                // 5 - Eliminar la fila que se acaba de eliminar                                
                that.filaFaceta.remove();
                // 6 - Actualizar el contador de nº de facetas            
                that.handleCheckNumberOfFacetas();                                
                // El guardado ha sido correcto
                setTimeout(function() {
                    mostrarNotificacion("success", "La faceta se ha eliminado correctamente.");
                },500);                
            }else{
                // Se ha producido un error en el borrado
                mostrarNotificacion("error", "Se ha producido un error al tratar de eliminar la faceta. Contacta con el administrador.");
            }
        });      
    },      

    /**
     * Método para gestionar el borrado de un item de tipo Tag cuando se pulsa en el botón (x). Se utilizará para el borrado de items de perfiles o grupos
     * Borrará el item de la pantalla y eliminará el item del input oculto (inputHack) que es el que recoge todos los valores seleccionados
     * @param {jqueryObject} itemDeleted 
     */
     handleClickBorrarTagItem: function(itemDeleted){
        const that = this;

        // Panel o sección donde se encuentra el panel de Tags a Eliminar (input_hack)
        const panelTagItem = itemDeleted.parent().parent();

        // Buscar el input oculto y seleccionar la propiedad del id que corresponde con el grupo a eliminar
        const idItemDeleted = itemDeleted.prop("id"); 
        // Items id dependiendo del tipo a borrar (Perfil, Grupo)
        let itemsId = "";
        
        // Borrar un grupo en la sección de Grupos correspondiente            
        const $inputHack = panelTagItem.find(`.${that.TabValoresPrivacidadGruposClassName}`);                  
     
        itemsId = that.filaFaceta.find($inputHack).val().split(",");
        itemsId.splice( $.inArray(idItemDeleted, itemsId), 1 );
        // Pasarle los datos de los grupos actuales al input hidden
        that.filaFaceta.find($inputHack).val(itemsId.join(",")); 
        // Eliminar el grupo del contenedor visual
        itemDeleted.remove();
     },

    /**
     * Método para configurar un input para que sea de tipo autocomplete. Se configurará el comportamiento autocomplete y el result (opción pulsada)
     * @param {jqueryElement} input 
     */
    handleSetupAutoCompleteForInput(input){
        const that = this;

        // Comportamiento autocomplete a input de selección de Perfiles para Privacidad de Faceta
        input.autocomplete(
            null,
            {
                url: $('#inpt_urlServicioAutocompletar').val() + "/AutoCompletarLectoresEditores",
                delay: 0,
                scroll: false,
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
                    bool_traergrupos: 'true',
                    bool_traerperfiles: 'false'
                }
            }
        );

        // Comportamiento resultado cuando se selecciona una resultado de autocomplete         
        input.result(function (event, data, formatted) {             
            // Nombre del grupo           
            const dataName = data[0];
            // Id del grupo
            const dataId = data[1];
            const input = $(event.currentTarget);
            that.handleSelectAutocompleteItem(input, dataName, dataId);            
        });
    },

    /**
     * Método para la selección realizada de un item haciendo uso de AutoComplete 
     * @param {string} input: Input que ha lanzado el autocomplete
     * @param {string} dataName: Nombre o dato nombre del item seleccionado del panel autoComplete
     * @param {string} dataId : Dato Id correspondiente al item seleccionado del panel autoComplete
     */
     handleSelectAutocompleteItem: function(input, dataName, dataId){
        // Panel/Sección donde se ha realizado toda la operativa
        const tagsSection = $(input).parent().parent();

        // Contenedor de items donde se añadirá el nuevo seleccionado para su visualización
        const tagContainer = tagsSection.find(".tag-list");
        // Input oculto donde se añadirá el nuevo item seleccionado
        const inputHack = tagsSection.find("input[type=hidden]").first();
        // Añadido el id del item seleccionado al inputHack                
        inputHack.val(inputHack.val() + dataId + ',');
    
        // Etiqueta del item seleccionado
        let editorSeleccionadoHtml = '';
        editorSeleccionadoHtml += '<div class="tag" id="'+ dataId +'">';
            editorSeleccionadoHtml += '<div class="tag-wrap">';
                editorSeleccionadoHtml += '<span class="tag-text">' + dataName + '</span>';
                editorSeleccionadoHtml += "<span class=\"tag-remove tag-remove-group material-icons\">close</span>";                
            editorSeleccionadoHtml += '</div>';
        editorSeleccionadoHtml += '</div>';

        // Añadir el item en el contenedor de items para su visualización
        tagContainer.append(editorSeleccionadoHtml);

        // Vaciar el input donde se ha escrito 
        $(input).val('');
    },

   
    /**
     * Método para controlar la aparición del panel de "reciprocidad" de la faceta      
     * @param {jqueryElement} check 
     */
    handleChangeReciproca: function(check){
        const value = check.data("value");
        const panelReciprocaDetail = this.filaFaceta.find(`.${this.panReciprocaDetailClassName}`);
        if (value == "si"){
            // Mostrar el panel de Reciprocidad
            panelReciprocaDetail.removeClass("d-none");            
        }else{
            // Ocultar el panel de Reciprocidad
            panelReciprocaDetail.addClass("d-none");
        }        
    },
    
    /**
     * Método para marcar un filtro personalizado como "Eliminado". 
     * Si se decide eliminar, no se elimina por completo hasta que se guarda la sección de facetas.
     * @param {*} btnDeleted : Botón para borrar o revertir el borrado del filtro
     * @param {bool} deleteFilter : Valor booleano de si se desea eliminar el filtro
     */
     handleDeleteFilter: function(btnDeleted, deleteFilter){
        const that = this;
        
        // Fila del filtro que se desea eliminar/restaurar
        const filaFiltro = btnDeleted.closest(`.${this.filaFiltroClassName}`);
        
        // Botón de eliminación y edición del filtro
        const btnDeleteCurrentFilter = filaFiltro.find(`.${this.btnDeleteFilterClassName}`);
        const btnEditCurrentFilter = filaFiltro.find(`.${this.btnEditFilterClassName}`);
        let btnRevertirFiltroDeleted = filaFiltro.find(".btnRevertirFiltroDeleted");
        // Area donde están los botones de acción (Editar, Eliminar, Revertir)
        const areaComponentActions = filaFiltro.find(".component-actions");
        
        if (deleteFilter){
            // Realizar el "borrado" del filtro
            // 1 - Marcar la opción "TabEliminada" a true           
            filaFiltro.find('[name="TabEliminada"]').val("true");                                   
            // 2 - Crear y añadir un botón de revertir el borrado de la página
            btnRevertirFiltroDeleted = `
            <li>
                <a class="action-delete round-icon-button js-action-delete btnRevertirFiltroDeleted" href="javascript: void(0);">
                    <span class="material-icons">restore_from_trash</span>
                </a>
            </li>
            `;
            // 3 - Ocultar los botones de editar y eliminar
            btnDeleteCurrentFilter.addClass("d-none");
            // Dejarlo como disabled
            btnEditCurrentFilter.addClass("inactiveLink");
            // 4 - Añadir el botón de revertir
            areaComponentActions.append(btnRevertirFiltroDeleted);            
            // 5- Añadir comportamiento de revertir          
            filaFiltro.find(".btnRevertirFiltroDeleted").off().on("click", function(){
                that.handleDeleteFilter($(this), false);
            });
            // 6- Añadir la clase de "deleted" a la fila          
            filaFiltro.addClass("deleted");
            
            // 7- Colapsar el panel si se desea eliminar y este está abierto
            const collapseId = filaFiltro.data("collapseid");
            $(`#${collapseId}`).removeClass("show");
        }else{
            // Revertir el "borrado"
            // 1 - Marcar la opción "TabEliminada" a false            
            filaFiltro.find('[name="TabEliminada"]').val("false");  
            // 2 - Eliminar el botón de revertir la filtro
            btnRevertirFiltroDeleted.remove();
            // 3 - Mostrar de nuevo los botones de editar y eliminar la página actual
            btnDeleteCurrentFilter.removeClass("d-none");
            btnEditCurrentFilter.removeClass("inactiveLink");    
            // 4 - Quitar la clase de "deleted" a la fila          
            filaFiltro.removeClass("deleted");
        }
    },  


    /**
     * Método para añadir un nuevo filtro a una faceta
     * @param {*} button 
     */
    handleAddFilter: function(button){            
        const that = this;

        // Panel listado de filtros
        const currentListFilters = that.filaFaceta.find(`.${that.facetaFilterListContainerClassName}`);
        
        // Mostrar loading para petición
        loadingMostrar();

        // Realizar la petición de nuevo filtro
        GnossPeticionAjax(
            that.urlAddNewFilter,
            null,
            true)
        .done(function (data) {
            // OK - Añadir filtro
            // Añadir el nuevo filtro creado al listado de filtros
            currentListFilters.append(data); 
            // Obtener el último filtro añadido
            const newFilterAdded = currentListFilters.children().last();                   
            // Mostrar los detalles del nuevo filtro creado
            newFilterAdded.find(`.${that.btnEditFilterClassName}`).trigger("click");                        
        }).fail(function (data) {
            // KO en creación del filtro           
            mostrarNotificacion("error", data);
        }).always(function () {
            // Ocultar loading
            loadingOcultar();           
        });        
    },


    /**
     * Método para marcar o desmarcar el parámetro como "Eliminado" dependiendo de la elección vía Modal
     * @param {Bool} deleteFaceta que indicará si se desea eliminar o no el item
     */
    handleSetDeleteFaceta: function(deleteFaceta){
        const that = this;

        if (deleteFaceta){
            // Realizar el "borrado"
            // 1 - Marcar la opción "TabEliminada" a true
            that.filaFaceta.find('[name="TabEliminada"]').val("true");           
            // 2 - Añadir la clase de "deleted" a la fila
            that.filaFaceta.addClass("deleted");
        }else{
            // 1 - Marcar la opción "TabEliminada" a false
            that.filaFaceta.find('[name="TabEliminada"]').val("false");            
            // 2 - Eliminar la clase de "deleted" a la fila
            that.filaFaceta.removeClass("deleted");
        }
    },  

    /**
     * Método para recoger los datos y proceder a su guardado
     */
    
    handleSaveFacetas: function(completion = undefined){            
        const that = this;
        // Objeto donde se construirán las facetas
        that.ListaFacetas = {};

        // Reseter flag de error por posibles errores que se han producido anteriormente
        that.errorsBeforeSaving = false;

        // Comprobación de erres antes de proceder al guardado - Comprobación de categorías cookies
        that.checkErrorsBeforeSaving();
       
        // Si todo ha ido bien, proceder con la recogida de datos y su guardado
        if (that.errorsBeforeSaving == false) {
            // Contador para crear el objeto de datos correctamente
            let cont = 0;        
            $(`.${this.facetaListItemClassName}`).each(function () {
                that.getFacetasData($(this), cont++);
            });            
            // Guardar datos            
            that.handleSave(completion);                        
        }
    },

    /**
     * Método para comprobar que no se encuentra ningún error antes de realizar el guardado.
     * El resultado de esas comprobaciones se guarda en 'errorsBeforeSaving'
     */
     checkErrorsBeforeSaving: function(){
        const that = this;
        
        // Flag para control de errores
        that.errorsBeforeSaving = false;                
        
        // Comprobar nombres vacíos
        that.comprobarNombresVacios(); 
                          
        // Comprobar facetas facías
        if (that.errorsBeforeSaving == false){
            that.comprobarFacetasVacias()
        }
        // Comprobar objetos de conocimiento
        if (that.errorsBeforeSaving == false){
            that.comprobarObjetosConocimientoVacios();
        }   
        // Comprobar que no haya filtros vacíos
        if (that.errorsBeforeSaving == false){
            that.comprobarFiltrosVacios();
        }
    },

    /**
     * Método para comprobar que los nombres no estén vacío.
     */
    comprobarNombresVacios: function(){
        const that = this;

        // Comprobación de que el nombre válido        
        let inputsNombre = $(`.${that.facetaListItemClassName} input[name="TabName"]:not(":disabled")`);        
        // Comprobación de los nombres
        inputsNombre.each(function () {
            if (that.errorsBeforeSaving == false){
                that.comprobarNombreVacio($(this));
            }
        });
    },  
    
    /**
     * Método para comprobar que el nombre corto y ó texto no esté vacío. Habrá que tener en cuenta el multiIdioma.
     * @param {*} inputName: Input a comprobar     
     */
    comprobarNombreVacio: function (inputName) {
        const that = this
        
        // Contiene los tabs en idiomas y el div con los inputs en idiomas completo
        const formularioEdicion = inputName.closest(".formulario-edicion");
        const panInputsMultiIdioma = inputName.closest(".inputsMultiIdioma");
        // Contenedor donde se encuentran datos básicos
        const panMultiIdioma = formularioEdicion.find(".panContenidoMultiIdioma.basicInfo");

        // Id del input a tratar
        const inputId = inputName.attr("id");

        const listaTextos = [];
        
        if (operativaMultiIdioma.listaIdiomas.length > 1 && panMultiIdioma.length > 0) {
            let textoMultiIdioma = "";
            // Comprobar que hay al menos un texto por defecto para el nombre
            let textoIdiomaDefecto = panMultiIdioma.find(`#input_${inputId}_${operativaMultiIdioma.idiomaPorDefecto}`).val(); //panMultiIdioma.find('#edicion_' + that.IdiomaPorDefecto + ' input').val();
            
            if (textoIdiomaDefecto == null || textoIdiomaDefecto == "") {
                const fila = inputName.closest(".component-wrap.faceta-row");
                that.mostrarErrorNombreVacio(fila, panMultiIdioma.find(`#input_${inputId}_${operativaMultiIdioma.idiomaPorDefecto}`));
                that.errorsBeforeSaving = true;
                return;
            }
            // Recorrer todos los idiomas para detectar posibles problemas con el nombre                
            $.each(operativaMultiIdioma.listaIdiomas, function () {
                // Obtención del Key del idioma
                const idioma = this.key;
                // Asignar el valor por defecto de la ruta al idioma si este no dispone de valor para Nombre
                let textoIdioma = $(`#input_${inputId}_${idioma}`).val();
                if (textoIdioma == null || textoIdioma == "") {
                    textoIdioma = textoIdiomaDefecto;
                    $(`#input_${inputId}_${idioma}`).val(textoIdioma);
                }
                // Escribir el nombre del multiIdioma en el campo Hidden
                textoMultiIdioma += textoIdioma + "@" + idioma + "|||";
                
                listaTextos.push({ "key": idioma, "value": textoIdioma });
            });            
            inputName.val(textoMultiIdioma);
        }else{
            // Comprobar si había multiIdioma configurado antes
            if (panMultiIdioma.length > 0){
                // Había MultiIdioma -> Coger el input del idioma por defecto 
                let textoIdiomaDefecto = panMultiIdioma.find(`#input_${inputId}_${operativaMultiIdioma.idiomaPorDefecto}`).val();
                inputName.val(textoIdiomaDefecto);
            }
        }        
    },

    /**
     * Método para comprobar que las facetas no estén vacías.
     */
    comprobarFacetasVacias: function(){
        const that = this;

        // Comprobación de que el componente       
        let inputsFaceta = $(`.${that.facetaListItemClassName} input[name="${that.txtClaveFaceta}"]:not(":disabled")`);        
        // Comprobación de las facetas
        inputsFaceta.each(function () {
            if (that.errorsBeforeSaving == false){
                if ($(this).val().trim() == '') {
                    // Si la faceta está vacía y la reciprocidad no está asignada > Error.
                    const fila = $(this).closest(`.${that.facetaListItemClassName}`);
                    // Valor del check que está seleccionado de reciprocidad
                    const checkReciprocidadValue = fila.find(`.${that.chkReciprocaClassName}:checked`).data("value");
                    const reciprocidadValue = fila.find(`.${that.txtReciprocaClassName}`).val().trim();
                    // Comprobar que no hay error de reciprocidad            
                    if (checkReciprocidadValue == "si" && reciprocidadValue.length == 0 ){
                        that.errorsBeforeSaving = true;
                        that.mostrarErrorFacetaVacia();
                    }
                }                
            }
        });
    }, 

    /**
     * Método para comprobar que los objetos de conocimiento de la faceta no estén vacías.
     */    
    comprobarObjetosConocimientoVacios: function(){        
        const that = this;        
        const inputsObjetosConocimiento = $(`.${that.facetaListItemClassName} input[name="${that.txtObjetosConocimientoHiddenClassName}"]:not(":disabled")`);// $('.row:not(".ui-state-disabled") input[name = "ObjetosConocimiento"]:not(":disabled")');
        inputsObjetosConocimiento.each(function () {
            if ($(this).val().trim() == '' && that.errorsBeforeSaving != true) {
                that.errorsBeforeSaving = true;
                that.mostrarErrorObjetoConocimientoVacio();
                return;                               
            }
        });  
    },

    /**
     * Método para comprobar que los filtros de una faceta no estén vacíos
     */
    comprobarFiltrosVacios: function(){
        const that = this;

        // Comprobación de que el filtro es válido        
        let inputsFiltros = $(`.${that.facetaListItemClassName} .${that.facetaFilterListContainerClassName} .${that.filaFiltroClassName}:not(".deleted")`);        
        // Comprobación de los nombres
        inputsFiltros.each(function () {
            if (that.errorsBeforeSaving == false){
                that.comprobarFiltroVacio($(this));
            }
        });
    }, 

    /**
     * Método para comprobar que un determinado filtro no está vacío
     * @param {*} fila 
     */
    comprobarFiltroVacio: function(fila){                
        const that = this
        
        // Panel donde se podrá localizar el input que almacena el valor del input para el filtroOrden
        const panelFilterInfo = fila.find('.faceta-filter-info');
        
        // Inputs que contendrá el nombre y la condición del filtro
        const inputFilterName = panelFilterInfo.find(`.${this.txtNombreFiltroClassName}`);        
                        
        if (inputFilterName.val() == "") {
            that.mostrarErrorFiltroVacio();
            that.errorsBeforeSaving = true;                 
        }                            
    },

    /**
     * Método para mostrar el error cuando el nombre de la faceta esté vacío
     */
    mostrarErrorNombreVacio: function(){
        mostrarNotificacion("error", "El nombre de la faceta no puede estar vacío.");
    },

    /**
     * Método para mostrar el error cuando las facetas sean recíprocas y estas no tengan un nombre asociado.
     */
    mostrarErrorFacetaVacia: function(){
        mostrarNotificacion("error", "La faceta está configurada como 'Recíproca' pero no se le ha asignado un valor.");        
    },
    
    /**
     * Método para mostrar un error cuando el objeto de conocimiento de una faceta esté vacío
     */
    mostrarErrorObjetoConocimientoVacio: function(){
        mostrarNotificacion("error", "La faceta debe tener al menos un objeto de conocimiento.");
    },

    /**
     * Método para mostrar un error cuando el nombre del filtro de una faceta esté vacío.
     */
    mostrarErrorFiltroVacio: function(){
        mostrarNotificacion("error", "El nombre de los filtros de la faceta no pueden estar vacíos.");
    },


    /**
     * Método para obtener los datos de cada una de las facetas y construir el objeto a enviar a backend.
     * @param {jqueryElement} fila Fila que se revisará para su obtención de datos
     * @param {int} num Número que se asignará para la construcción del objeto
     */
    getFacetasData: function(fila, num){

        const that = this;
        // Id de la faceta
        const id = fila.attr('id');
        // Contenido o datos de la faceta
        const panelEdicion = fila.find(`.${this.modalFacetaClassName}`);
        // Prefijo para guardado de la pestaña/página
        const prefijoClave = 'ListaFacetas[' + num + ']';
        // Nombre de la faceta
        that.ListaFacetas[prefijoClave + '.Name'] = panelEdicion.find('[name="TabName"]').val().trim();
        // Id de la faceta
        that.ListaFacetas[prefijoClave + '.AgrupacionID'] = fila.attr("id");  
        // Obtener valor de los objetos de conocimiento (hidden input)
        const objetosConocimientoValor = panelEdicion.find(`[name="${that.txtObjetosConocimientoHiddenClassName}"]`).val();
        // Solo si hay objetos de conocimiento
        if (objetosConocimientoValor != undefined) {
            // Obtener cada objeto de conocimiento (array)
            const objetosConocimiento = objetosConocimientoValor.split(',');
            let numOC = 0;
            $.each(objetosConocimiento, function () {
                const oc = this.trim();
                if (oc != "") {
                    that.ListaFacetas[prefijoClave + '.ObjetosConocimiento[' + numOC + ']'] = oc;
                    numOC++;
                }
            });

            // Tipo de la faceta
            that.ListaFacetas[prefijoClave + '.Type'] = panelEdicion.find(`[name="${that.txtFacetaTypeClassName}"]`).val();
            // Clave de la faceta
            that.ListaFacetas[prefijoClave + '.ClaveFaceta'] = panelEdicion.find(`[name="${that.txtClaveFaceta}"]`).val().trim();
            // Reciprocidad de la faceta
            let reciprocidad = "";                 
            // Obtener la reciprocidad de la faceta si está activada
            if (panelEdicion.find(`#${that.chkReciprocaClassName}_SI_${id}`).is(':checked')){
                reciprocidad = panelEdicion.find(`[name="${that.txtReciprocaClassName}"]`).val();      
            }
            // Reciprocidad de la faceta
            that.ListaFacetas[prefijoClave + '.Reciprocidad'] = reciprocidad;
            // Obtener información de si está o no eliminada la faceta
            that.ListaFacetas[prefijoClave + '.Deleted'] = panelEdicion.find('[name="TabEliminada"]').val();
            // Orden de la faceta
            that.ListaFacetas[prefijoClave + '.Orden'] = num;

            // Presentación de la faceta
            that.ListaFacetas[prefijoClave + '.Presentacion'] = panelEdicion.find(`.${that.cmbPresentacionClassName}`).val();
            that.ListaFacetas[prefijoClave + '.Disenyo'] = panelEdicion.find(`.${that.cmbDisenyoClassName}`).val();
            that.ListaFacetas[prefijoClave + '.AlgoritmoTransformacion'] = panelEdicion.find(`.${that.cmbAlgoritmoTransformacionClassName}`).val();
            // Revisión de filtros de la faceta
            that.ListaFacetas[prefijoClave + '.Filtros'] = panelEdicion.find('[name="Filtros"]').val();             
            // Recorrer y obtener los filtros si es que los hay
            that.ListaFacetas[prefijoClave + '.Filtros'] = '';
            let numExport = 0;
            // Panel donde estarán todos los filtros
            const panelEditarFiltros = panelEdicion.find(`.${that.facetaFilterListContainerClassName}`);
            // Recorrer cada filtro para la obtención de datos
            $(`.${that.filaFiltroClassName}`, panelEditarFiltros).each(function () {
                // Panel de detalles del filtro
                const panFiltro = $(this).find(".faceta-filter-info");
                // Prefijo para cada filtro de la faceta
                const prefijoFiltrosClave = prefijoClave + '.ListaFiltrosFacetas[' + numExport + ']';
                that.ListaFacetas[prefijoFiltrosClave + '.Orden'] = numExport;
                that.ListaFacetas[prefijoFiltrosClave + '.Deleted'] = panFiltro.find('[name="TabEliminada"]').val();
                that.ListaFacetas[prefijoFiltrosClave + '.Nombre'] = panFiltro.find(`.${that.txtNombreFiltroClassName}`).val();
                that.ListaFacetas[prefijoFiltrosClave + '.Condicion'] = panFiltro.find(`.${that.txtCondicionFiltroClassName}`).val();
                // Asignar los datos del filtro
                that.ListaFacetas[prefijoClave + '.Filtros'] += that.ListaFacetas[prefijoFiltrosClave + '.Nombre'] + ',';
                numExport++;
            });

            // Nº de elementos visibles
            that.ListaFacetas[prefijoClave + '.NumElementosVisibles'] = panelEdicion.find(`.${that.txtNumElementosVisiblesClassName}`).val();
            // Obtención de datos (Checkbox / Combobox)
            // Check de Autocompletar
            that.ListaFacetas[prefijoClave + '.Autocompletar'] = $(`#${that.chkAutocompletarClassName}_SI_${id}`).is(':checked'); // panelEdicion.find('[name="Autocompletar"]').is(':checked');
            // ComboBox de comportamiento
            that.ListaFacetas[prefijoClave + '.Comportamiento'] = panelEdicion.find(`.${that.cmbComportamientoClassName}`).val();
            that.ListaFacetas[prefijoClave + '.OcultaEnFacetas'] = $(`#${that.chkOcultaEnFacetasClassName}_SI_${id}`).is(':checked'); // panelEdicion.find('[name="OcultaEnFacetas"]').is(':checked');
            that.ListaFacetas[prefijoClave + '.OcultaEnFiltros'] = $(`#${that.chkOcultarEnFiltrosClassName}_SI_${id}`).is(':checked'); // panelEdicion.find('[name="OcultaEnFiltros"]').is(':checked');
            that.ListaFacetas[prefijoClave + '.ComportamientoOR'] = $(`#${that.chkComportamientoORClassName}_SI_${id}`).is(':checked'); // panelEdicion.find('[name="ComportamientoOR"]').is(':checked');
            that.ListaFacetas[prefijoClave + '.Inmutable'] = $(`#${that.chkInmutableClassName}_SI_${id}`).is(':checked'); // panelEdicion.find('[name="chkInmutable"]').is(':checked');
            that.ListaFacetas[prefijoClave + '.PriorizarOrdenResultados'] = $(`#${that.chkPriorizarOrdenResultadosClassName}_SI_${id}`).is(':checked'); //panelEdicion.find('[name="PriorizarOrdenResultados"]').is(':checked');            

            // Flag indicadora de si el modal ha sido modificado o no
            const modified = $(`#${id}`).hasClass("modified");            
            that.ListaFacetas[prefijoClave + '.Modified'] = modified;

            // Privacidad de los grupos para la faceta
            const privacidadGrupos = panelEdicion.find(`[name="${that.TabValoresPrivacidadGruposClassName}"]`).val().split(',');
            for (var i = 0; i < privacidadGrupos.length; i++) {
                if (privacidadGrupos[i].trim() != "") {
                    const prefijoPrivacidadGrupos = prefijoClave + '.PrivacidadGrupos[' + i + ']';
                    that.ListaFacetas[prefijoPrivacidadGrupos + '.Key'] = privacidadGrupos[i].substr(2);
                    that.ListaFacetas[prefijoPrivacidadGrupos + '.Value'] = "";
                }
            }             
            // Condición            
            that.ListaFacetas[prefijoClave + '.Condicion'] = panelEdicion.find(`.${that.auxCondicionesSeleccionadasClassName}`).val();
        }
    },
    
    /**
     * Método para guardar los datos recogidos de facetas
     * @param {function} completion : Función o comportamiento a realizar una vez se realice el proceso de guardar facetas. Por defecto será undefined
     */
    handleSave: function(completion = undefined){
        const that = this;

        // Mostrar loading
        loadingMostrar();

        // Realizar petición para el guardado de datos
        GnossPeticionAjax(
            that.urlSave,
            that.ListaFacetas,
            true
        ).done(function (data) {
            // OK Guardado correcto
            // Quitar aquella fila el flag de nueva creación
            $(".newFaceta").removeClass("newFaceta");
            if (completion == undefined){
                // Mostrar mensaje OK y recargar la página
                const modal = $(that.filaFaceta).find(`.${that.modalFacetaClassName}`);                                          
                dismissVistaModal(modal);                
                // Recargar la página para recargar los cambios sólo en edición o creación                
                setTimeout(function() {                                                                          
                    mostrarNotificacion("success", "Los cambios se han guardado correctamente.");                                     
                    // Recargar la página
                    // location.reload();                                                   
                }
                ,500);            
            }else{
                // Devolver a completion para posterior ejecución
                completion(requestFinishResult.ok, data)
            }        

        }).fail(function (data) {
            if (data == ""){
                mostrarNotificacion("error", "Se ha producido un error al tratar de guardar los datos. Por favor, contacta con el administrador.");                              
            }else{
                mostrarNotificacion("error", data);
            }
            
        }).always(function () {
            // Ocultar el loading
            loadingOcultar();
        });
    },
   
    
    /**
     * Método para llamar el modal para crear una nueva faceta
     * @param {*} url 
     */
    handleLoadCreateFaceta: function(type, url){
        const that = this;
       
        // Mostrar loading hasta que finalice la petición
        loadingMostrar();   

        const dataPost = {
            TipoFaceta: type
        }

        // Realizar la petición de la cláusula a crear
        GnossPeticionAjax(                
            url,
            dataPost,
            true
        ).done(function (data) {            
            // Añadir la nueva fila al listado            
            that.facetaListContainer.append(data);                        
            // Referencia a la nueva
            that.newFaceta = that.facetaListContainer.children().last();
            // Añadirle el flag de "nuevo" para saber que es de reciente creación.
            that.newFaceta.addClass("newFaceta");   
            // Establecer como fila seleccionada la nueva creada
            that.filaFaceta = that.newFaceta;    
            // Actualizar el nº de items
            that.handleCheckNumberOfFacetas();               
            // Ejecutar scripts iniciales para idioma y select2
            that.triggerEvents();
            // Abrir el modal para editar/crear el nuevo item añadido          
            that.newFaceta.find(`.${that.btnEditFacetaClassName}`).trigger("click");
        }).fail(function (data) {
            // Mostrar error al tratar de crear un item
            mostrarNotificacion("error", data);                
        }).always(function () {
            // Ocultar loading de la petición
            loadingOcultar();
        });           
    },
    /**
     * Método para llamar el modal para crear una nueva faceta
     * @param {*} url 
     */
    handleLoadCreateFacetaPropuesta: function (id, url) {
        const that = this;

        // Mostrar loading hasta que finalice la petición
        loadingMostrar();

        const dataPost = {
            pestanyaPropuestaID: id
        }

        // Realizar la petición de la cláusula a crear
        GnossPeticionAjax(
            url,
            dataPost,
            true
        ).done(function (data) {
            // Añadir la nueva fila al listado            
            that.facetaListContainer.append(data);
            // Referencia a la nueva
            that.newFaceta = that.facetaListContainer.children().last();
            // Añadirle el flag de "nuevo" para saber que es de reciente creación.
            that.newFaceta.addClass("newFaceta");
            // Establecer como fila seleccionada la nueva creada
            that.filaFaceta = that.newFaceta;
            // Actualizar el nº de items
            that.handleCheckNumberOfFacetas();
            // Ejecutar scripts iniciales para idioma y select2
            that.triggerEvents();
            // Abrir el modal para editar/crear el nuevo item añadido          
            that.newFaceta.find(`.${that.btnEditFacetaClassName}`).trigger("click");
        }).fail(function (data) {
            // Mostrar error al tratar de crear un item
            mostrarNotificacion("error", data);
        }).always(function () {
            // Ocultar loading de la petición
            loadingOcultar();
        });
    },
    /**
     * Método para obtener el número total de facetas existentes y actualizar su contador.
     */
     handleCheckNumberOfFacetas: function(){
        const that = this;
        const numberFacetas = that.facetaListContainer.find($(`.${that.facetaListItemClassName}`)).length;
        // Mostrar el nº de items                
        that.numFaceta.html(numberFacetas);

        // No mostrar el item si no hay resultados
        numberFacetas == 0 ? that.numFaceta.addClass("d-none") : that.numFaceta.removeClass("d-none");        
    },

    /**
     * Método para realizar búsquedas y filtrados
     * @param {*} input 
     */    
    handleSearchFaceta: function(input){

        const that = this;
        // Cadena introducida para filtrar/buscar
        let cadena = input.val();
        // Eliminamos posibles tildes para búsqueda ampliada
        cadena = cadena.normalize('NFD').replace(/[\u0300-\u036f]/g, "").toLowerCase();

        // Listado de componentes donde se buscará
        const facetas = $(`.${that.facetaListItemClassName}`);

        // Recorrer los items para realizar la búsqueda de palabras clave
        $.each(facetas, function(index){
            const facetaItem = $(this);
            // Seleccionar el nombre sólo "visible" de la traducción. Quitamos caracteres extraños, tildes para poder hacer bien la búsqueda
            const componentContenido = facetaItem.find(".component-name").not('.d-none').html().trim().normalize("NFD").replace(/[\u0300-\u036f]/g, "").toLowerCase();;                        
            const componentTipo = facetaItem.find(".component-tipo").not('.d-none').html().trim().normalize("NFD").replace(/[\u0300-\u036f]/g, "").toLowerCase();;
            const componentFaceta = facetaItem.find(".component-faceta").not('.d-none').html().trim().normalize("NFD").replace(/[\u0300-\u036f]/g, "").toLowerCase();;

            if (componentContenido.includes(cadena) || componentTipo.includes(cadena) || componentFaceta.includes(cadena)){
                // Mostrar la fila
                facetaItem.removeClass("d-none");                
            }else{
                // Ocultar la fila
                facetaItem.addClass("d-none");
            }            
        });                     
    },

    /**
     * Método para realizar búsquedas y filtrados
     * @param {*} input 
     */
    handleSearchFacetaPropuesta: function (input) {

        const that = this;
        // Cadena introducida para filtrar/buscar
        let cadena = input.val();
        // Eliminamos posibles tildes para búsqueda ampliada
        cadena = cadena.normalize('NFD').replace(/[\u0300-\u036f]/g, "").toLowerCase();

        // Listado de componentes donde se buscará
        const facetas = $(`.${that.facetaSuggestListItemClassName}`);

        // Recorrer los items para realizar la búsqueda de palabras clave
        $.each(facetas, function (index) {
            const facetaItem = $(this);
            // Seleccionar el nombre sólo "visible" de la traducción. Quitamos caracteres extraños, tildes para poder hacer bien la búsqueda
            const componentContenido = facetaItem.find(".component-name").not('.d-none').html().trim().normalize("NFD").replace(/[\u0300-\u036f]/g, "").toLowerCase();;
            const componentTipo = facetaItem.find(".component-tipo").not('.d-none').html().trim().normalize("NFD").replace(/[\u0300-\u036f]/g, "").toLowerCase();;
            const componentFaceta = facetaItem.find(".component-faceta").not('.d-none').html().trim().normalize("NFD").replace(/[\u0300-\u036f]/g, "").toLowerCase();;

            if (componentContenido.includes(cadena) || componentTipo.includes(cadena) || componentFaceta.includes(cadena)) {
                // Mostrar la fila
                facetaItem.removeClass("d-none");
            } else {
                // Ocultar la fila
                facetaItem.addClass("d-none");
            }
        });
    },
    /**
     * Método para cambiar la visualización al idioma de la traducción deseada
     */
    handleViewFacetaLanguageInfo: function(){    
        const that = this;
        // Comprobar el item que está activo en el tab obteniendo el data-language de la opción "active"
        setTimeout(function(){             
            // Detectar el tab item activo para conocer el idioma en el que se desean mostrar las páginas
            const tabLanguageActive = that.tabIdiomaItem.filter(".active");
            // Obtener el idioma del tabLanguageActivo
            const languageActive = tabLanguageActive.data("language");
            // Ocultar todas las labels y mostrar únicamente las del idioma seleccionado
            $(`.${that.labelLanguageComponentClassName}`).addClass("d-none");
            // Mostrar sólo las labelsLanguageComponent del idioma seleccionado
            $(`.${that.labelLanguageComponentClassName}`).filter(function () {
                return $(this).data("languageitem") == languageActive;
            }).removeClass("d-none");

        },250);                
    }, 
}



/**
  * Operativa para la gestión y configuración de Contextos de la comunidad desde Información Contextual
  */
 const operativaGestionContextos = {

    /**
    * Inicializar operativa
    */
     init: function (pParams) {
        this.pParams = pParams;
        this.config(pParams);
        this.configEvents();
        this.configRutas(); 
        this.triggerEvents();            
    },

    /**
     * Lanzar comportamientos u operativas necesarias para el funcionamiento de la sección
     */
    triggerEvents: function(){
        const that = this;           

        // Indicar el nº de items existente
        that.handleCheckNumberOfContextos();
        
        // Inicializar comportamientos de select2
        comportamientoInicial.iniciarSelects2(); 

        // Operativa multiIdioma
        // Parámetros para la operativa multiIdioma (helpers.js)
        this.operativaMultiIdiomaParams = {
            // Nº máximo de pestañas con idiomas a mostrar. Los demás quedarán ocultos
            numIdiomasVisibles: 3,
            // Establecer 1 tab por cada input (true, false) - False es la forma vieja
            useOnlyOneTab: true,
            panContenidoMultiIdiomaClassName: "panContenidoMultiIdioma",
            // No permitir padding bottom y si padding top
            allowPaddingBottom: false,                                              
        };  

        // Configurar arrastrar contextos        
        setupBasicSortableList(that.contextoListContainerId, that.sortableContextoIconClassName, undefined, undefined, undefined, that.handleSaveContextosSorted);    

        // Inicializar operativa multiIdioma
        operativaMultiIdioma.init(this.operativaMultiIdiomaParams);           
    },   

    /**
     * Configuración de las rutas de acciones necesarias para hacer peticiones a backend
     */
    configRutas: function (pParams) {
        // Url para editar un certificado
        this.urlBase = refineURL();
        this.urlSave = `${this.urlBase}/save`;        
 
        // Flag para controlar borrado
        this.confirmDeleteContexto = false;
        // Flag para controlar posibles errores
        this.errorsBeforeSaving = false;
        
    },  

    /*
    * Inicializar elementos de la vista
    * */
    config: function (pParams) {        
        // Contenedor modal
        this.modalContainer = $("#modal-container");
        this.modalContextoClassName = "modal-contexto";
        this.modalDeleteContexto = $("#modal-delete-contexto");

        // Tab de idioma de la página para cambiar la visualización en el idioma deseado
        this.tabIdiomaItem = $(".tabIdiomaItem "); 
        // Cada una de las labels (Nombre literal) en los diferentes idiomas (por `data-languageitem`)
        this.labelLanguageComponentClassName = ".language-component";       

        // Botón para añadir un contexto
        this.linkAddNewContexto = $(".linkAddNewContexto");
        // Icono para arrastrar un contexto
        this.sortableContextoIconClassName = 'js-component-sortable-contexto';

        // Buscador
        this.txtBuscarContexto = $("#txtBuscarContexto");
        // Contenedor de contextos
        this.contextoListContainerId = 'id-added-contexto-list';
        this.contextoListContainer = $(`#${this.contextoListContainerId}`);
        // Nombre de la fila de cada contexto
        this.contextoListItemClassName = "contexto-row";
        // Botón de editar contexto
        this.btnEditContextoClassName = "btnEditContexto";                
        // Botón para eliminar un contexto
        this.btnDeleteContextoClassName = "btnDeleteContexto";
        // Botón para confirmar la eliminación de un contexto
        this.btnConfirmDeleteContextoClassName = "btnConfirmDeleteContexto";
        // Contador de número de items existentes
        this.numContexto = $("#numContextos");        

        /* Inputs del modal de creación de Contexto */
        // Botón para guardar faceta
        this.btnSaveContextoClassName = "btnSaveContexto";
        
        /* Inputs checkbox dentro del modal para editar los datos de la faceta */        
        this.txtNameClassName = "Name";
        this.txtContenidoClassName = "Contenido";
        this.txtTipoGadgetClassName = "TipoGadget";
        this.txtShortNameClassName = "ShortName";
        this.chkCargaAjaxClassName = "chkCargaAjax";
        this.txtClasesClassName = "Clases";
        this.chkVisibleClassName = "chkVisible";
        this.txtFiltrosDestinoClassName = "FiltrosDestino";        
        this.txtComunidadOrigenClassName = "ComunidadOrigen";
        this.txtFiltrosOrigenClassName = "FiltrosOrigen";
        this.txtRelacionOrigenDestinoClassName = "RelacionOrigenDestino"
        this.txtNumResultadosClassName = "NumResultados";
        this.OrdenResultadosClassName = "OrdenResultados";
        this.selectImagen = "selectImagen";
        this.chkMostrarEnlaceOriginalClassName = "chkMostrarEnlaceOriginal";
        this.chkMostrarVerMasClassName = "chkMostrarVerMas";
        this.chkAbrirEnPestanyaNuevaClassName = "chkAbrirEnPestanyaNueva";
        this.txtNamespacesExtraClassName = "NamespacesExtra";
        this.txtResultadosExcluirClassName = "ResultadosExcluir";

        
        // Fila del contexto que está siendo editada
        this.filaContexto = undefined;
        // Flags para confirmar la eliminación de un contexto
        this.confirmDeleteContexto = false;
    },   

    /**
     * Configuración de eventos de elementos del Dom (Botones, Inputs...)     
     */
    configEvents: function (pParams) {
        const that = this;

        // Comportamientos del modal container 
        this.modalContainer.on('show.bs.modal', (e) => {
            // Aparición del modal
            // Establecer quién ha disparado el modal para poder cambiar sus atributos una vez se guarden los grupos desde el modal
            that.triggerModalContainer = $(e.relatedTarget);                              
        })
        .on('hide.bs.modal', (e) => {
            // Acción de ocultar el modal
        })
        .on('hidden.bs.modal', (e) => {
            // Vaciar el modal
            resetearModalContainer(); 
        });

        // Comportamientos del modal de editar/crear
        configEventByClassName(`${that.modalContextoClassName}`, function(element){
            const $modal = $(element);
            $modal
            .on('show.bs.modal', (e) => {
                // Aparición del modal
                that.triggerModalContainer = $(e.relatedTarget);
            })
            .on('hide.bs.modal', (e) => {
               // Eliminar la fila si es de reciente creación y no se ha guardado
               if (that.filaContexto.hasClass("newContexto")){
                    that.filaContexto.remove();                    
                    that.handleCheckNumberOfContextos();
               }
            }); 
        }); 

        // Comportamientos del modal de eliminar contexto
        that.modalDeleteContexto.
        on('show.bs.modal', (e) => {
            // Aparición del modal
            that.triggerModalContainer = $(e.relatedTarget);
        })
        .on('hide.bs.modal', (e) => {
            // Acción de ocultar el modal
            // Si el modal se cierra sin confirmar borrado, desactiva el borrado
            if (that.confirmDeleteContexto == false){
                that.handleSetDeleteContexto(false);                
            }       
        }); 
           
        // Botón para editar el contexto
        configEventByClassName(`${that.btnEditContextoClassName}`, function(element){
            const $jqueryElement = $(element);
            $jqueryElement.off().on("click", function(){
                that.filaContexto = $(this).closest(`.${that.contextoListItemClassName}`);
                // Indicar en la row que se está modificando
                that.filaContexto.addClass("modified");
            });	                        
        });

        // Búsquedas de contextos
        this.txtBuscarContexto.off().on("keyup", function (event) {
            const input = $(this);
            clearTimeout(that.timer);
            that.timer = setTimeout(function () {                                                                        
                // Acción de buscar / filtrar
                that.handleSearchContexto(input);                                         
            }, 500);
        });        
        
        // Botón/es para confirmar la eliminación de un contexto (Modal -> Sí)        
        configEventByClassName(`${that.btnConfirmDeleteContextoClassName}`, function(element){
            const $jqueryElement = $(element);
            $jqueryElement.off().on("click", function(){
                that.confirmDeleteContexto = true;
                that.handleDeleteContexto();              
            });	                        
        });  
        
        // Botón/es para añadir un tipo de contexto a la comunidad
        this.linkAddNewContexto.on("click", function(){
            const type = $(this).data("type");
            const url = $(this).data("url");
            that.handleLoadCreateContexto(type, url);            
        });        

        // Botón para eliminar un contexto
        configEventByClassName(`${that.btnDeleteContextoClassName}`, function(element){
            const $btnDelete = $(element);
            $btnDelete.off().on("click", function(){                
                // Fila correspondiente al contexto eliminado
                that.filaContexto = $btnDelete.closest(`.${that.contextoListItemClassName}`);                
                // Marcar el parámetro como "Eliminar" a modo informativo al usuario
                that.handleSetDeleteContexto(true);
            });	                        
        });                                                       
        
        // Botón para guardar contexto        
        configEventByClassName(`${that.btnSaveContextoClassName}`, function(element){
            const $saveButton = $(element);
            $saveButton.on("click", function(){   
                that.handleSaveContextos();                                              
                
            });	                        
        });

        // Tab de cada uno de los idiomas disponibles para cambiar la visualización en la lista de cláusulas
        this.tabIdiomaItem.off().on("click", function(){            
            that.handleViewContextoLanguageInfo();                        
            // Aplicar la búsqueda para el idioma activo
            that.timer = setTimeout(function () {                                                                                                        
                that.handleSearchContexto(that.txtBuscarContexto);
            }, 500);            
        });    
        
        // Comprobación de que no esté vacío el campo de Contenido               
        configEventByClassName(`${that.txtContenidoClassName}`, function(element){
            const $textArea = $(element);
            $textArea.on("blur", function(){   
                comprobarInputNoVacio($textArea, true, false, "El contenido de un contexto no puede estar vacío.", 0);
            });	                        
        });

        // Comprobación de que no esté vacío el campo de Comunidad origen
        configEventByClassName(`${that.txtComunidadOrigenClassName}`, function(element){
            const $textArea = $(element);
            $textArea.on("blur", function(){   
                comprobarInputNoVacio($textArea, true, false, "La comunidad origen de un contexto no puede estar vacía.", 0);
            });	                        
        });        

        // Comprobación de que no esté vacío el campo de Relación-Origen-Destino        
        configEventByClassName(`${that.txtRelacionOrigenDestinoClassName}`, function(element){
            const $textArea = $(element);
            $textArea.on("blur", function(){   
                comprobarInputNoVacio($textArea, true, false, "La relación origen destino de un contexto no puede estar vacía.", 0);
            });	                        
        });    
        
        // Comprobación de que no esté vacío el campo de ShortName       
        configEventByClassName(`${that.txtShortNameClassName}`, function(element){
            const $input = $(element);
            $input.on("blur", function(){   
                comprobarInputNoVacio($input, true, false, "El nombre corto del contexto no puede estar vacío.", 0);
            });	                        
        });
    },

    /**
     * Método para guardar los contextos sólo cuando se ha procedido a su ordenación.
     */
    handleSaveContextosSorted: function(){
        const that = operativaGestionContextos;

        that.handleSaveContextos(function(isOk, data){
            if (isOk == requestFinishResult.ok){                
                // Datos guardados al haber ordenado los items. No hacer nada                         
            }
        });
    }, 
  
    /**
     * Método para eliminar un Contexto previa confirmación realizada desde el modal
     */
     handleDeleteContexto: function(){
        const that = this;                  
        // 1 - Llamar al método para el guardado de contextos
        that.handleSaveContextos(function(isOk, data){
            if (isOk == requestFinishResult.ok){                
                dismissVistaModal(that.modalDeleteContexto);
                // 3 - Ocultar loading
                loadingOcultar();                
                // 4 - Ocultar la fila que se desea eliminar
                that.filaContexto.addClass("d-none");
                // 5 - Eliminar la fila que se acaba de eliminar                                
                that.filaContexto.remove();
                // 6 - Actualizar el contador de nº de contextos
                that.handleCheckNumberOfContextos();                                
                // El guardado ha sido correcto
                setTimeout(function() {
                    mostrarNotificacion("success", "El contexto se ha eliminado correctamente.");
                },500);                
            }else{
                // Se ha producido un error en el borrado
                mostrarNotificacion("error", "Se ha producido un error al tratar de eliminar el contexto. Contacta con el administrador.");
            }
        });      
    },      
       
    /**
     * Método para marcar o desmarcar el parámetro como "Eliminado" dependiendo de la elección vía Modal
     * @param {Bool} deleteContexto que indicará si se desea eliminar o no el item
     */
    handleSetDeleteContexto: function(deleteContexto){
        const that = this;

        if (deleteContexto){
            // Realizar el "borrado"
            // 1 - Marcar la opción "TabEliminada" a true
            that.filaContexto.find('[name="TabEliminada"]').val("true");           
            // 2 - Añadir la clase de "deleted" a la fila
            that.filaContexto.addClass("deleted");
        }else{
            // 1 - Marcar la opción "TabEliminada" a false
            that.filaContexto.find('[name="TabEliminada"]').val("false");            
            // 2 - Eliminar la clase de "deleted" a la fila
            that.filaContexto.removeClass("deleted");
        }
    },  

    /**
     * Método para recoger los datos y proceder a su guardado
     */
    handleSaveContextos: function(completion = undefined){            
        const that = this;
        // Objeto donde se construirán los contextos
        that.ListaGadgets = {};

        // Reseter flag de error por posibles errores que se han producido anteriormente
        that.errorsBeforeSaving = false;

        // Comprobación de erres antes de proceder al guardado - Comprobación de categorías cookies
        that.checkErrorsBeforeSaving();
       
        // Si todo ha ido bien, proceder con la recogida de datos y su guardado
        if (that.errorsBeforeSaving == false) {
            // Contador para crear el objeto de datos correctamente
            let cont = 0;        
            $(`.${this.contextoListItemClassName}`).each(function () {
                that.getContextosData($(this), cont++);
            });            
            // Guardar datos            
            that.handleSave(completion);                        
        }
    },

    /**
     * Método para comprobar que no se encuentra ningún error antes de realizar el guardado.
     * El resultado de esas comprobaciones se guarda en 'errorsBeforeSaving'
     */
     checkErrorsBeforeSaving: function(){
        const that = this;
        
        // Flag para control de errores
        that.errorsBeforeSaving = false;                
                
        that.comprobarNombresVacios(); 

        // Comprobar que contenidos no estén vacíos
        if (that.errorsBeforeSaving == false){
            that.comprobarContenidosVacios();
        }   
        
        // Comprobar que nombres cortos no estén vacíos
        if (that.errorsBeforeSaving == false){
            that.comprobarNombresCortosRepetidos();
        }  
        
        // Comprobar que nombres cortos no contengan espacios ni caracteres especiales
        if (that.errorsBeforeSaving == false){
            that.comprobarNombresCortosConEspacios();
        }          
        
        // Comprobar que los demás campos no estén vacíos
        if (that.errorsBeforeSaving == false){            
            that.comprobarCamposVacios();
        }          
    },

    /**
     * Método para comprobar que los nombres no estén vacío.
     */
    comprobarNombresVacios: function(){
        const that = this;
        
        // Comprobación de que el nombre válido        
        let inputsNombre = $(`.${that.contextoListItemClassName} input[name="${that.txtNameClassName}"]:not(":disabled")`);         
        // Comprobación de los nombres
        inputsNombre.each(function () {
            if (that.errorsBeforeSaving == false){
                that.comprobarNombreVacio($(this));
            }
        });
        
    },  
    
    /**
     * Método para comprobar que el nombre corto y ó texto no esté vacío. Habrá que tener en cuenta el multiIdioma.
     * @param {*} inputName: Input a comprobar     
     */
    comprobarNombreVacio: function (inputName) {
        const that = this
        
        // Contiene los tabs en idiomas y el div con los inputs en idiomas completo
        const formularioEdicion = inputName.closest(".formulario-edicion");
        const panInputsMultiIdioma = inputName.closest(".inputsMultiIdioma");
        // Contenedor donde se encuentran datos básicos
        const panMultiIdioma = formularioEdicion.find(".panContenidoMultiIdioma.basicInfo");

        // Id del input a tratar
        const inputId = inputName.attr("id");

        const listaTextos = [];
        
        if (operativaMultiIdioma.listaIdiomas.length > 1 && panMultiIdioma.length > 0) {
            let textoMultiIdioma = "";
            // Comprobar que hay al menos un texto por defecto para el nombre
            let textoIdiomaDefecto = panMultiIdioma.find(`#input_${inputId}_${operativaMultiIdioma.idiomaPorDefecto}`).val(); //panMultiIdioma.find('#edicion_' + that.IdiomaPorDefecto + ' input').val();
            
            if (textoIdiomaDefecto == null || textoIdiomaDefecto == "") {
                const fila = inputName.closest(".component-wrap.contexto-row");
                that.mostrarErrorNombreVacio(fila, panMultiIdioma.find(`#input_${inputId}_${operativaMultiIdioma.idiomaPorDefecto}`));
                that.errorsBeforeSaving = true;
                return;
            }
            // Recorrer todos los idiomas para detectar posibles problemas con el nombre                
            $.each(operativaMultiIdioma.listaIdiomas, function () {
                // Obtención del Key del idioma
                const idioma = this.key;
                // Asignar el valor por defecto de la ruta al idioma si este no dispone de valor para Nombre
                let textoIdioma = $(`#input_${inputId}_${idioma}`).val();
                if (textoIdioma == null || textoIdioma == "") {
                    textoIdioma = textoIdiomaDefecto;
                    $(`#input_${inputId}_${idioma}`).val(textoIdioma);
                }
                // Escribir el nombre del multiIdioma en el campo Hidden
                textoMultiIdioma += textoIdioma + "@" + idioma + "|||";
                
                listaTextos.push({ "key": idioma, "value": textoIdioma });
            });            
            inputName.val(textoMultiIdioma);
        }               
    },

    
    /**
     * Método para mostrar el error cuando el nombre esté vacío 
     * @param {*} fila Fila del error
     * @param {*} input Input que ha generado el error
     */
    mostrarErrorNombreVacio: function(fila, input){
        mostrarNotificacion("error", "El nombre no puede estar vacío.");
    },

    /**
     * Método para comprobar que los contextos no estén vacías.
     */
     comprobarContenidosVacios: function(){
        const that = this;
        
        // Comprobación de que el nombre válido        
        let inputsNombre = $(`.${that.contextoListItemClassName} textarea[name="${that.txtContenidoClassName}"]:not(":disabled")`);         
        // Comprobación 
        inputsNombre.each(function () {
            if (that.errorsBeforeSaving == false){
                that.comprobarContenidoVacio($(this));
            }
        }); 
    }, 

    /**
     * Método para comprobar que el nombre corto y ó texto no esté vacío. Habrá que tener en cuenta el multiIdioma.
     * @param {*} inputName: Input a comprobar     
     */
     comprobarContenidoVacio: function (inputName) {
        const that = this
        
        // Contiene los tabs en idiomas y el div con los inputs en idiomas completo
        const formularioEdicion = inputName.closest(".formulario-edicion");
        const panInputsMultiIdioma = inputName.closest(".inputsMultiIdioma");
        // Contenedor donde se encuentran datos básicos
        const panMultiIdioma = formularioEdicion.find(".panContenidoMultiIdioma.basicInfo");

        // Id del input a tratar
        const inputId = inputName.attr("id");

        const listaTextos = [];

        // El campo "Contenido" puede ser multiIdioma o no. Revisarlo si al menos contiene un id
        if (inputId != undefined){        
            if (operativaMultiIdioma.listaIdiomas.length > 1 && panMultiIdioma.length > 0) {
                let textoMultiIdioma = "";
                // Comprobar que hay al menos un texto por defecto para el nombre
                let textoIdiomaDefecto = panMultiIdioma.find(`#input_${inputId}_${operativaMultiIdioma.idiomaPorDefecto}`).val(); //panMultiIdioma.find('#edicion_' + that.IdiomaPorDefecto + ' input').val();
                
                if (textoIdiomaDefecto == null || textoIdiomaDefecto == "") {
                    const fila = inputName.closest(".component-wrap.contexto-row");
                    that.mostrarErrorContenidoVacio(fila, panMultiIdioma.find(`#input_${inputId}_${operativaMultiIdioma.idiomaPorDefecto}`));
                    that.errorsBeforeSaving = true;
                    return;
                }
                // Recorrer todos los idiomas para detectar posibles problemas con el nombre                
                $.each(operativaMultiIdioma.listaIdiomas, function () {
                    // Obtención del Key del idioma
                    const idioma = this.key;
                    // Asignar el valor por defecto de la ruta al idioma si este no dispone de valor para Nombre
                    let textoIdioma = $(`#input_${inputId}_${idioma}`).val();
                    if (textoIdioma == null || textoIdioma == "") {
                        textoIdioma = textoIdiomaDefecto;
                        $(`#input_${inputId}_${idioma}`).val(textoIdioma);
                    }
                    // Escribir el nombre del multiIdioma en el campo Hidden
                    textoMultiIdioma += textoIdioma + "@" + idioma + "|||";
                    
                    listaTextos.push({ "key": idioma, "value": textoIdioma });
                });            
                inputName.val(textoMultiIdioma);
            }  
        }             
    },   
    
    mostrarErrorContenidoVacio: function(){
        mostrarNotificacion("error", "El contenido del contexto no puede estar vacío.");
    }, 

    /**
     * Método para comprobar que los nombres cortos de los contextos no estén repetidos
     */
    comprobarNombresCortosRepetidos: function(){
        const that = this;
        
        // Comprobación de que el nombre válido        
        let inputsNombre = $(`.${that.contextoListItemClassName} input[name="${that.txtShortNameClassName}"]:not(":disabled")`);         
        // Comprobación 
        inputsNombre.each(function () {
            if (that.errorsBeforeSaving == false){
                that.comprobarNombreCortoRepetido($(this));
            }
        });
    },

    /**
     * Método para comprobar que los nombres cortos de los contextos no estén repetidos
     * @param {*} inputName: Input a comprobar     
     */
    comprobarNombreCortoRepetido: function(input){
        const that = this;
        const fila = input.closest(`.${that.contextoListItemClassName}`);

        const inputsNombresCortos = $(`.${that.contextoListItemClassName} input[name="${that.txtShortNameClassName}"]:not(":disabled")`);        
        const nombreCorto = input.val();
        
        inputsNombresCortos.each(function () {
            if (!that.errorsBeforeSaving){
                const inputCompare = $(this);
                if (inputCompare.closest(`.${that.contextoListItemClassName}`).attr('id') != fila.attr('id')) {
                    if (inputCompare.val() == nombreCorto) {
                        that.errorsBeforeSaving = true;
                    }
                }
            }            
        });
        if (that.errorsBeforeSaving) {
            that.mostrarErrorNombreCortoRepetido(fila);
        }           
    },    

    /**
     * Método para mostrar el error cuando el nombre corto del contexto esté repetido
     * @param {*} fila 
     */
    mostrarErrorNombreCortoRepetido: function (fila) {
        //var inputNombreCorto = $('input[name = "ShortName"]', fila).first();        
        mostrarNotificacion("error", "El nombre corto del contexto no puede estar vacío.");        
    },

    /**
     * Método para comprobar que los nombres cortos de los contextos no contengan espacios
     */
    comprobarNombresCortosConEspacios: function(){
        const that = this;
        
        // Comprobación de que el nombre válido        
        let inputsNombre = $(`.${that.contextoListItemClassName} input[name="${that.txtShortNameClassName}"]:not(":disabled")`);         
        // Comprobación 
        inputsNombre.each(function () {
            if (that.errorsBeforeSaving == false){
                that.comprobarNombreCortoConEspacios($(this));
            }
        });
    },

    /**
     * Método para comprobar que que el nombre corto del contexto no contenga espacios. 
     * @param {*} input Input que será analizado     
     */
     comprobarNombreCortoConEspacios: function(input){
        const that = this;
                
        const shortName = input.val();

        // Input que no esté vacío
        if(shortName.trim().length == 0){
            that.mostrarErrorNombreCortoVacio(input.closest('.contexto-row'));
            return;
        }

        // Input que no contenga espacios en blanco
        if (shortName.trim().indexOf(" ") >= 0) {
            that.mostrarErrorNombreCortoEspacios(input.closest('.contexto-row'));
            that.errorsBeforeSaving = true;
            return;
        }

        // Input que no contenga '#'
        if (shortName.indexOf("#") >= 0) {
            that.mostrarErrorNombreCortoCaracteresEspeciales(input.closest('.contexto-row'));
            that.errorsBeforeSaving = true;
        }                
    },  
    
    /**
     * Método para mostrar el error de que el nombre corto de un contexto está vacío
     * @param {*} fila 
     */
    mostrarErrorNombreCortoVacio(){
        //const inputUrl = $('input[name = "ShortName"]', fila).first();
        mostrarNotificacion("error", "El nombre corto del contexto no puede estar vacío."); 
    },
    
    /**
     * Método para mostrar el error de que el nombre corto de un contexto contiene espacios
     * @param {*} fila 
     */
    mostrarErrorNombreCortoEspacios: function(fila){        
        //const inputUrl = $('input[name = "ShortName"]', fila).first();
        mostrarNotificacion("error", "El nombre corto del contexto no puede contener espacios.");                
    },

    /**
     * Método para mostrar el error de que el nombre corto de un contexto contiene caracteres especiales
     * @param {*} fila
     */
    mostrarErrorNombreCortoCaracteresEspeciales: function(fila){
        //const inputUrl = $('input[name = "ShortName"]', fila).first();
        mostrarNotificacion("error", "El nombre corto no puede contener el caracter especial '#'.");        
    },

    /**
     * Método para comprobar que los campos no estén vacíos
     */
    comprobarCamposVacios: function(){
        const that = this;

        // Comprobación de que el nombre Contenido        
        let inputsNombre = $(`.${that.contextoListItemClassName}:not(".deleted") textarea[name="${that.txtContenidoClassName}"]:not(":disabled")`);         
        // Comprobación 
        inputsNombre.each(function () {
            if (that.errorsBeforeSaving == false){
                if ($(this).val() == "") {
                    that.mostrarErrorCampoVacio($(this));
                    that.errorsBeforeSaving = true;
                }                
            }
        });

        // Comprobación de que el nombre Contenido        
        inputsNombre = $(`.${that.contextoListItemClassName}:not(".deleted") textarea[name="${that.txtComunidadOrigenClassName}"]:not(":disabled")`);         
        // Comprobación 
        inputsNombre.each(function () {
            if (that.errorsBeforeSaving == false){
                if ($(this).val() == "") {
                    that.mostrarErrorCampoVacio($(this));
                    that.errorsBeforeSaving = true;
                }                
            }
        });

        inputsNombre = $(`.${that.contextoListItemClassName}:not(".deleted") textarea[name="${that.txtRelacionOrigenDestinoClassName}"]:not(":disabled")`);         
        // Comprobación 
        inputsNombre.each(function () {
            if (that.errorsBeforeSaving == false){
                if ($(this).val() == "") {
                    that.mostrarErrorCampoVacio($(this));
                    that.errorsBeforeSaving = true;
                }                
            }
        });              
    },


    /**
     * Método para mostar el error de que el campo no puede estar vacío.
     */
    mostrarErrorCampoVacio: function(input){
        // var fila = input.closest('.row');
        mostrarNotificacion("error", "El campo del contexto no puede estar vacío."); 
        input.trigger("blur");               
    },    

    /**
     * Método para obtener los datos de cada una de laos contextos y construir el objeto a enviar a backend.
     * @param {jqueryElement} fila Fila que se revisará para su obtención de datos
     * @param {int} num Número que se asignará para la construcción del objeto
     */
    getContextosData: function(fila, num){
        const that = this;

        // Id
        const id = fila.attr('id');
        // Contenido o datos del contexto
        const panelEdicion = fila.find(`.${this.modalContextoClassName}`);

        // Tipo de contexto
        var tipoGadget = panelEdicion.find('[name="TipoGadget"]').val();
        // Prefijo para el guardado del contexto
        var prefijoClave = 'ListaGadgets[' + num + ']';

        // Key
        that.ListaGadgets[prefijoClave + '.Key'] = id;
        // Es borrada?
        that.ListaGadgets[prefijoClave + '.Deleted'] = panelEdicion.find('[name="TabEliminada"]').val();
        // Posición
        that.ListaGadgets[prefijoClave + '.Orden'] = num;
        // Name        
        that.ListaGadgets[prefijoClave + '.Name'] = panelEdicion.find(`[name="${that.txtNameClassName}"]`).val();
        // Clases
        that.ListaGadgets[prefijoClave + '.Clases'] = panelEdicion.find(`[name="${that.txtClasesClassName}"]`).val();
        // Visible
        that.ListaGadgets[prefijoClave + '.Visible'] = panelEdicion.find($(`#${that.chkVisibleClassName}_SI_${id}`)).is(':checked');
        // Tipo Gadget                                                           
        that.ListaGadgets[prefijoClave + '.TipoGadget'] = tipoGadget;
        // Ajax
        that.ListaGadgets[prefijoClave + '.Ajax'] = panelEdicion.find($(`#${that.chkCargaAjaxClassName}_SI_${id}`)).is(':checked');
        // Filtros destino
        that.ListaGadgets[prefijoClave + '.FiltrosDestino'] = panelEdicion.find(`[name="${that.txtFiltrosDestinoClassName}"]`).val();
        // Nombre corto
        that.ListaGadgets[prefijoClave + '.ShortName'] = panelEdicion.find(`[name="${that.txtShortNameClassName}"]`).val();

        // Contenido según el tipo de componente
        if (tipoGadget == 'HtmlIncrustado' || tipoGadget == 'Consulta' || tipoGadget == 'CMS') {
            that.ListaGadgets[prefijoClave + '.Contenido'] = encodeURIComponent(panelEdicion.find(`[name="${that.txtContenidoClassName}"]`).val());
        }else if (tipoGadget == 'RecursosContextos') {
            const prefijoClaveContexto = prefijoClave + '.Contexto';
            // Comunidad origen
            that.ListaGadgets[prefijoClaveContexto + '.ComunidadOrigen'] = panelEdicion.find(`[name="${that.txtComunidadOrigenClassName}"]`).val();
            // Filtros origen
            that.ListaGadgets[prefijoClaveContexto + '.FiltrosOrigen'] = panelEdicion.find(`[name="${that.txtFiltrosOrigenClassName}"]`).val();
            // Relación origen destino
            that.ListaGadgets[prefijoClaveContexto + '.RelacionOrigenDestino'] = panelEdicion.find(`[name="${that.txtRelacionOrigenDestinoClassName}"]`).val();
            // Nº de resultados
            that.ListaGadgets[prefijoClaveContexto + '.NumResultados'] = panelEdicion.find(`[name="${that.txtNumResultadosClassName}"]`).val();
            // Orden resultados
            that.ListaGadgets[prefijoClaveContexto + '.OrdenResultados'] = panelEdicion.find(`[name="${that.OrdenResultadosClassName}"]`).val();
            // Imagen
            that.ListaGadgets[prefijoClaveContexto + '.Imagen'] = panelEdicion.find(`[name="${that.selectImagen}"]`).val();
            // Mostrar enlace original
            that.ListaGadgets[prefijoClaveContexto + '.MostrarEnlaceOriginal'] = panelEdicion.find($(`#${that.chkMostrarEnlaceOriginalClassName}_SI_${id}`)).is(':checked');
            that.ListaGadgets[prefijoClaveContexto + '.MostrarVerMas'] = panelEdicion.find($(`#${that.chkMostrarVerMasClassName}_SI_${id}`)).is(':checked');
            that.ListaGadgets[prefijoClaveContexto + '.AbrirEnPestanyaNueva'] = panelEdicion.find($(`#${that.chkAbrirEnPestanyaNuevaClassName}_SI_${id}`)).is(':checked');
            // NamespacesExtra
            that.ListaGadgets[prefijoClaveContexto + '.NamespacesExtra'] = panelEdicion.find(`[name="${that.txtNamespacesExtraClassName}"]`).val();
            // Resultdos excluir
            that.ListaGadgets[prefijoClaveContexto + '.ResultadosExcluir'] = panelEdicion.find(`[name="${that.txtResultadosExcluirClassName}"]`).val();
        }
    },
    
    /**
     * Método para guardar los datos recogidos de contextos
     * @param {function} completion : Función o comportamiento a realizar una vez se realice el proceso de guardar contextos. Por defecto será undefined
     */
    handleSave: function(completion = undefined){
        const that = this;

        // Mostrar loading
        loadingMostrar();

        // Realizar petición para el guardado de datos
        GnossPeticionAjax(
            that.urlSave,
            that.ListaGadgets,
            true
        ).done(function (data) {
            // OK Guardado correcto
            // Quitar aquella fila el flag de nueva creación
            $(".newContexto").removeClass("newContexto");
            if (completion == undefined){
                // Mostrar mensaje OK y recargar la página
                const modal = $(that.filaContexto).find(`.${that.modalContextoClassName}`);                                          
                dismissVistaModal(modal);                
                // Recargar la página para recargar los cambios sólo en edición o creación                
                setTimeout(function() {                                                                          
                    mostrarNotificacion("success", "Los cambios se han guardado correctamente.");                                     
                    // Recargar la página
                    location.reload();                                                   
                }
                ,500);            
            }else{
                // Devolver a completion para posterior ejecución
                completion(requestFinishResult.ok, data)
            }        

        }).fail(function (data) {
            mostrarNotificacion("error", data);                              
        }).always(function () {
            // Ocultar el loading
            loadingOcultar();
        });
    },
   
    
    /**
     * Método para llamar el modal para crear una nueva faceta
     * @param {*} url 
     */
    handleLoadCreateContexto: function(type, url){
        const that = this;
       
        // Mostrar loading hasta que finalice la petición
        loadingMostrar();   

        const dataPost = {
            TipoGadget: type,
        }

        // Realizar la petición de la cláusula a crear
        GnossPeticionAjax(                
            url,
            dataPost,
            true
        ).done(function (data) {            
            // Añadir la nueva fila al listado            
            that.contextoListContainer.append(data);                        
            // Referencia a la nueva
            that.newContexto = that.contextoListContainer.children().last();
            // Añadirle el flag de "nuevo" para saber que es de reciente creación.
            that.newContexto.addClass("newContexto");   
            // Establecer como fila seleccionada la nueva creada
            that.filaContexto = that.newContexto;    
            // Actualizar el nº de items
            that.handleCheckNumberOfContextos();
            // Ejecutar scripts iniciales para idioma y select2
            that.triggerEvents();
            // Abrir el modal para editar/crear el nuevo item añadido          
            that.newContexto.find(`.${that.btnEditContextoClassName}`).trigger("click");
        }).fail(function (data) {
            // Mostrar error al tratar de crear un item
            mostrarNotificacion("error", data);                
        }).always(function () {
            // Ocultar loading de la petición
            loadingOcultar();
        });                  
    },

    /**
     * Método para obtener el número total de facetas existentes y actualizar su contador.
     */
     handleCheckNumberOfContextos: function(){
        const that = this;
        const numberContextos = that.contextoListContainer.find($(`.${that.contextoListItemClassName}`)).length;
        // Mostrar el nº de items                
        that.numContexto.html(numberContextos);
    },

    /**
     * Método para realizar búsquedas y filtrados
     * @param {*} input 
     */    
    handleSearchContexto: function(input){

        const that = this;
        // Cadena introducida para filtrar/buscar
        let cadena = input.val();
        // Eliminamos posibles tildes para búsqueda ampliada
        cadena = cadena.normalize('NFD').replace(/[\u0300-\u036f]/g, "").toLowerCase();

        // Listado de componentes donde se buscará
        const contextos = $(`.${that.contextoListItemClassName}`);

        // Recorrer los items para realizar la búsqueda de palabras clave
        $.each(contextos, function(index){
            const contextoItem = $(this);
            // Seleccionar el nombre sólo "visible" de la traducción. Quitamos caracteres extraños, tildes para poder hacer bien la búsqueda
            const componentName = contextoItem.find(".component-name").not('.d-none').html().trim().normalize("NFD").replace(/[\u0300-\u036f]/g, "").toLowerCase();                        
            const componentTipo = contextoItem.find(".component-tipo").not('.d-none').html().trim().normalize("NFD").replace(/[\u0300-\u036f]/g, "").toLowerCase();            

            if (componentName.includes(cadena) || componentTipo.includes(cadena)){
                // Mostrar la fila
                contextoItem.removeClass("d-none");                
            }else{
                // Ocultar la fila
                contextoItem.addClass("d-none");
            }            
        });                     
    },


    /**
     * Método para cambiar la visualización al idioma de la traducción deseada
     */
    handleViewContextoLanguageInfo: function(){    
        const that = this;
        // Comprobar el item que está activo en el tab obteniendo el data-language de la opción "active"
        setTimeout(function(){             
            // Detectar el tab item activo para conocer el idioma en el que se desean mostrar las páginas
            const tabLanguageActive = that.tabIdiomaItem.filter(".active");
            // Obtener el idioma del tabLanguageActivo
            const languageActive = tabLanguageActive.data("language");
            // Ocultar todas las labels y mostrar únicamente las del idioma seleccionado
            $(`.${that.labelLanguageComponentClassName}`).addClass("d-none");
            // Mostrar sólo las labelsLanguageComponent del idioma seleccionado
            $(`.${that.labelLanguageComponentClassName}`).filter(function () {
                return $(this).data("languageitem") == languageActive;
            }).removeClass("d-none");

        },250);                
    }, 
}

/**
  * Operativa para la gestión y configuración de Sugerencia de búsqueda (Autocompletado) de la comunidad desde "Sugerencias de búsqueda"
  */
const operativaGestionSugerenciasDeBusqueda = {
   /**
    * Inicializar operativa
    */
    init: function (pParams) {
        this.pParams = pParams;
        this.config(pParams);
        this.configEvents();
        this.configRutas(); 
        this.triggerEvents();            
    },

    /**
     * Lanzar comportamientos u operativas necesarias para el funcionamiento de la sección
     */
    triggerEvents: function(){
        const that = this;
        
        // Configurar inputs para autocomplete
        that.setupInputForAutocomplete(that.txtPropiedadAutocompletar, that.inputPropiedadesAutocompletar);
        that.setupInputForAutocomplete(that.txtPropiedadPalabrasTextoLibre, that.inputTextoLibreAutocompletar);
    },   

    /**
     * Configuración de las rutas de acciones necesarias para hacer peticiones a backend
     */
    configRutas: function (pParams) {
        this.urlBase = refineURL();
        this.urlSave = `${this.urlBase}/save`;
        this.urlRegenerar = `${this.urlBase}/regenerar`; 
        // Objeto donde se guardarán los items para su guardado
        this.Options = {};
    },  

    /*
    * Inicializar elementos de la vista
    * */
    config: function (pParams) {        
        // Contenedor modal
        this.modalContainer = $("#modal-container");

        // Input text de Propiedades para autocompletar
        this.txtPropiedadAutocompletar = $("#txtPropiedadAutocompletar");
        // Button de Añadir Propiedades para autocompletar
        this.btnAddPropiedadAutocompletar = $("#btnAddPropiedadAutocompletar");
        // Input text de Palabras para texto libre
        this.txtPropiedadPalabrasTextoLibre = $("#txtPropiedadPalabrasTextoLibre");
        // Button de Añadir propiedades para búsqueda de texto libre
        this.btnAddPropiedadPalabrasTextolLibre = $("#btnAddPropiedadPalabrasTextolLibre");
        // Botón X para quitar un tag de tipo Propiedades autocompletar
        this.btnRemoveTagPropertyAutocompleteClassName = 'tag-remove-property-autocomplete';
        this.btnRemoveTagTextAutocompleteClassName = 'tag-remove-text-autocomplete';        
        // Input oculto que contendrá las propiedades autocompletar
        this.inputPropiedadesAutocompletar = $('[name="PropiedadesAutocompletar"]');
        // Input oculto que contendrá el texto libre para autocompletar
        this.inputTextoLibreAutocompletar = $('[name="PropiedadPalabrasTextoLibre"]');
        // Botón para regenerar el autocompletar
        this.btnRegenerarAutocomplete = $("#btnRegenerarAutocomplete");
        // Botón para guardar los cambios
        this.btnGuardarAutocompletar = $("#btnGuardarAutocompletar");
    },   


    /**
     * Configuración de eventos de elementos del Dom (Botones, Inputs...)      
     * @param {*} pParams Parámetros adicionales pasados desde la View
     */
    configEvents: function (pParams) {
        const that = this;

        // Comportamientos del modal container 
        this.modalContainer.on('show.bs.modal', (e) => {
            // Aparición del modal
            // Establecer quién ha disparado el modal para poder cambiar sus atributos una vez se guarden los grupos desde el modal
            that.triggerModalContainer = $(e.relatedTarget);                              
        })
        .on('hide.bs.modal', (e) => {
            // Acción de ocultar el modal
        })
        .on('hidden.bs.modal', (e) => {
            // Vaciar el modal
            resetearModalContainer(); 
        });
                                                                  
        /**
         * Click en "Añadir etiqueta" que no se haya seleccionado vía "autocomplete"
         */
        this.btnAddPropiedadAutocompletar.on("click", function(){
            // Añadir propiedad si se ha escrito algo
            if (that.txtPropiedadAutocompletar.length > 0){
                // Añadir tambien tag para TextoLibre
                // that.txtPropiedadPalabrasTextoLibre.val(that.txtPropiedadAutocompletar.val());                
                //PintarTags(that.txtPropiedadAutocompletar);
                //PintarTags(that.txtPropiedadPalabrasTextoLibre);
                that.handleSelectAutocompleteProperty($(this));
            }            
        });            
            
        /**
         * Comportamiento resultado cuando se selecciona una resultado de autocomplete de Propiedades para autocompletar
         */
        this.txtPropiedadAutocompletar.result(function (event, data, formatted) {            
            const dataName = data[0];
            const dataId = data[1];            
            const input = $(event.currentTarget);
            that.handleSelectAutocompleteAutocompleteProperty(input, dataName, dataId, true, true);
        });       

        // Botón (X) para poder eliminar items desde el Tag                           
        configEventByClassName(`${that.btnRemoveTagPropertyAutocompleteClassName}`, function(element){
            const $jqueryElement = $(element);
            $jqueryElement.off().on("click", function(){                 
                const $itemRemoved = $jqueryElement.parent().parent();                                                                              
                that.handleClickBorrarTagAutocompleteProperty($itemRemoved, true, false);                
            });	                        
        }); 
        
        // Botón (X) para poder eliminar items desde el Tag                           
        configEventByClassName(`${that.btnRemoveTagTextAutocompleteClassName}`, function(element){
            const $jqueryElement = $(element);
            $jqueryElement.off().on("click", function(){                 
                const $itemRemoved = $jqueryElement.parent().parent();                                                                              
                that.handleClickBorrarTagAutocompleteProperty($itemRemoved, false, true);                
            });	                        
        });         
        
        // Click en "Añadir etiqueta" que no se haya seleccionado vía "autocomplete" para texto libre
        this.btnAddPropiedadPalabrasTextolLibre.on("click", function(){
            // Añadir propiedad si se ha escrito algo
            if (that.txtPropiedadPalabrasTextoLibre.length > 0){
                PintarTags(that.txtPropiedadPalabrasTextoLibre, true);
            }            
        });  

        /**
         * Comportamiento resultado cuando se selecciona una resultado de autocomplete de para autocompletar
         */
         this.txtPropiedadPalabrasTextoLibre.result(function (event, data, formatted) {            
            const dataName = data[0];
            const dataId = data[1];
            const input = $(event.currentTarget);
            that.handleSelectAutocompleteAutocompleteProperty(input, dataName, dataId, false, true);
        });
     
        // Botón para regenerar autoComplete
        this.btnRegenerarAutocomplete.on("click", function(){
            that.handleRegenerarAutocomplete();
        });   
        
        // Click para guardar los datos en backend
        this.btnGuardarAutocompletar.on("click", function(){
            that.handleSave();
        });      
    },

    /**
     * Método para obtener los datos que serán guardados posteriormente en backend
     */
    obtenerDatos: function(){
        const that = this;

        // Datos de Propiedades para autocompletar
        const stringAutocompletar = that.inputPropiedadesAutocompletar.val().length > 1 ? that.inputPropiedadesAutocompletar.val() : "";
        // Datos de Texto para autocompletar
        const stringTxtLibre = that.inputTextoLibreAutocompletar.val().length > 1 ? that.inputTextoLibreAutocompletar.val() : "";

        // Datos a enviar a backEnd
        that.Options['TagsAutocompletar'] = stringAutocompletar;
        that.Options['TagsTxtLibre'] = stringTxtLibre;        
    },
    
    /**
     * Método para regenear el autocomplete
     */
    handleRegenerarAutocomplete: function(){  
        const that = this;                          
        // Obtener los datos        
        that.obtenerDatos();
        
        loadingMostrar();

        // Hacer petición para regenerar los datos
        GnossPeticionAjax(
        that.urlRegenerar,
        that.Options,
        true
        ).done(function (data) {
            mostrarNotificacion("success", "El servicio de autocompletar se ha regenerado correctamente.");            
        }).fail(function (data) {                        
            mostrarNotificacion("error", "Se ha producido un error al regenerar el servicio de autocompletar. Contacta con el administrador."); 
        }).always(function () {
            loadingOcultar();
        });
    },

    /**
     * Método para añadir un tag de Propiedades para autocompletar
     */
    handleSelectAutocompleteProperty: function(){
        const that = this;
        
        // Añadir propiedad para autocompletar
        let propertyId = guidGenerator();
        const propertyValue = that.txtPropiedadAutocompletar.val().trim();

        if (propertyValue.trim().length == 0){
            return;
        }

        // Panel/Sección donde se ha realizado toda la operativa
        let tagsSection = that.txtPropiedadAutocompletar.parent().parent();

        // Contenedor de items donde se añadirá el nuevo seleccionado para su visualización
        let tagContainer = tagsSection.find(".tag-list");
        // Input oculto donde se añadirá el nuevo item seleccionado
        let inputHack = tagsSection.find("input[type=hidden]").first();
        // Añadido el id del item seleccionado al inputHack                
        inputHack.val(inputHack.val() + propertyValue + ',');
    
        // Etiqueta del item seleccionado        
        let autocompletePropertyTag = '';
        autocompletePropertyTag += '<div class="tag" id="'+ propertyId +'" title="'+ propertyValue +'">';
            autocompletePropertyTag += '<div class="tag-wrap">';
                autocompletePropertyTag += '<span class="tag-text">' + propertyValue + '</span>';
                autocompletePropertyTag += "<span class=\"tag-remove tag-remove-property-autocomplete material-icons\">close</span>";                
            autocompletePropertyTag += '</div>';
        autocompletePropertyTag += '</div>';

        // Añadir el item en el contenedor de items para su visualización
        tagContainer.append(autocompletePropertyTag);        

        // Añadir texto libre autocompletar    
        propertyId = guidGenerator();    

        // Panel/Sección donde se ha realizado toda la operativa
        tagsSection = that.txtPropiedadPalabrasTextoLibre.parent().parent();

        // Contenedor de items donde se añadirá el nuevo seleccionado para su visualización
        tagContainer = tagsSection.find(".tag-list");
        // Input oculto donde se añadirá el nuevo item seleccionado
        inputHack = tagsSection.find("input[type=hidden]").first();
        // Añadido el id del item seleccionado al inputHack                
        inputHack.val(inputHack.val() + propertyValue + ',');

        // Etiqueta del item seleccionado        
        autocompletePropertyTag = '';
            autocompletePropertyTag += '<div class="tag" id="'+ propertyId +'" title="'+ propertyValue +'">';
                autocompletePropertyTag += '<div class="tag-wrap">';
                    autocompletePropertyTag += '<span class="tag-text">' + propertyValue + '</span>';
                    // autocompletePropertyTag += "<span class=\"tag-remove tag-remove-property-autocomplete material-icons\">close</span>";
                    autocompletePropertyTag += '<input type="hidden" value="'+ propertyId +'" />';
                autocompletePropertyTag += '</div>';
            autocompletePropertyTag += '</div>';

        // Añadir el item en el contenedor de items para su visualización
        tagContainer.append(autocompletePropertyTag);

        // Vaciar el input donde se ha escrito 
        that.txtPropiedadAutocompletar.val('');         
    },


    /**
     * Método para la selección realizada de un item haciendo uso de AutoComplete 
     * @param {string} input: Input que ha lanzado el autocomplete
     * @param {string} dataName: Nombre o dato seleccionado del panel autoComplete
     * @param {string} dataId : Dato Id correspondiente al item seleccionado del panel autoComplete
     * @param {bool} addItemInAutoCompletePropertyList: Indica si es necesario añadirlo en la sección Propiedades para autocompletar
     * @param {bool} addItemInFreeTextSearch: Indica si es necesario añadirlo en la sección Texto Libre
     * */
    handleSelectAutocompleteAutocompleteProperty: function(input, dataName, dataId, addItemInAutoCompletePropertyList, addItemInFreeTextSearch){  
        const that = this;

        // Panel/Sección donde se ha realizado toda la operativa
        let tagsSection = $(input).parent().parent();

        // Contenedor de items donde se añadirá el nuevo seleccionado para su visualización
        let tagContainer = tagsSection.find(".tag-list");
        // Input oculto donde se añadirá el nuevo item seleccionado
        let inputHack = tagsSection.find("input[type=hidden]").first();



        // Añadido el id del item seleccionado al inputHack  
        if (dataId == undefined ){
            dataId = `${Math.floor(Math.random() * (1000 - 9999 + 1)) + 1000}_${dataName}`;
        }

        if (addItemInAutoCompletePropertyList){
            // Añadir el item al inputHack
            inputHack.val(inputHack.val() + dataName + ',')            
            
            // Etiqueta del item seleccionado
            let autocompletePropertyTag = '';
            autocompletePropertyTag += '<div class="tag" id="'+ dataId +'" title="'+ dataName +'" >';
                autocompletePropertyTag += '<div class="tag-wrap">';
                    autocompletePropertyTag += '<span class="tag-text">' + dataName + '</span>';
                    autocompletePropertyTag += "<span class=\"tag-remove tag-remove-property-autocomplete material-icons\">close</span>";                
                autocompletePropertyTag += '</div>';
            autocompletePropertyTag += '</div>';

            // Añadir el item en el contenedor de items para su visualización
            tagContainer.append(autocompletePropertyTag);
        }

        if (addItemInFreeTextSearch){
            // Añadir sólo en sección de Búsquedas por texto libre si así se necesitara
            // Añadir texto libre autocompletar    
            propertyId = guidGenerator();    

            // Panel/Sección donde se ha realizado toda la operativa
            tagsSection = that.txtPropiedadPalabrasTextoLibre.parent().parent();

            // Contenedor de items donde se añadirá el nuevo seleccionado para su visualización
            tagContainer = tagsSection.find(".tag-list");
            // Input oculto donde se añadirá el nuevo item seleccionado
            inputHack = tagsSection.find("input[type=hidden]").first();
            // Añadido el id del item seleccionado al inputHack                
            inputHack.val(inputHack.val() + dataName + ',');

            // Etiqueta del item seleccionado        
            autocompletePropertyTag = '';
                autocompletePropertyTag += '<div class="tag" id="'+ dataId +'" title="'+ dataName +'">';
                    autocompletePropertyTag += '<div class="tag-wrap">';
                        autocompletePropertyTag += '<span class="tag-text">' + dataName + '</span>';
                        if (!addItemInAutoCompletePropertyList){
                            autocompletePropertyTag += "<span class=\"tag-remove tag-remove-property-autocomplete material-icons\">close</span>"; 
                        }
                        // autocompletePropertyTag += "<span class=\"tag-remove tag-remove-property-autocomplete material-icons\">close</span>";                        
                    autocompletePropertyTag += '</div>';
                autocompletePropertyTag += '</div>';

            // Añadir el item en el contenedor de items para su visualización
            tagContainer.append(autocompletePropertyTag);   
        }

           
        // Vaciar el input donde se ha escrito 
        $(input).val('');   
    },

    /**
     * Método para la selección realizada de un item haciendo uso de AutoComplete 
     * @param {string} input: Input que ha lanzado el autocomplete
     * @param {string} dataName: Nombre o dato seleccionado del panel autoComplete
     * @param {string} dataId : Dato Id correspondiente al item seleccionado del panel autoComplete
     * */
     handleSelectAutocompleteAutocompleteText: function(input, dataName, dataId){     
        // Panel/Sección donde se ha realizado toda la operativa
        const tagsSection = $(input).parent().parent();

        // Contenedor de items donde se añadirá el nuevo seleccionado para su visualización
        const tagContainer = tagsSection.find(".tag-list");
        // Input oculto donde se añadirá el nuevo item seleccionado
        const inputHack = tagsSection.find("input[type=hidden]").first();
        // Añadido el id del item seleccionado al inputHack                
        inputHack.val(inputHack.val() + dataId + ',');
    
        // Etiqueta del item seleccionado
        let autocompletePropertyTag = '';
        autocompletePropertyTag += '<div class="tag" id="'+ dataId +'">';
            autocompletePropertyTag += '<div class="tag-wrap">';
                autocompletePropertyTag += '<span class="tag-text">' + dataName + '</span>';
                autocompletePropertyTag += "<span class=\"tag-remove tag-remove-text-autocomplete material-icons\">close</span>";                
            autocompletePropertyTag += '</div>';
        autocompletePropertyTag += '</div>';

        // Añadir el item en el contenedor de items para su visualización
        tagContainer.append(autocompletePropertyTag);

        // Vaciar el input donde se ha escrito 
        $(input).val('');   
    },    

    /**
     * Método para gestionar el borrado de un item de tipo Tag cuando se pulsa en el botón (x). Se utilizará para el borrado de items de propiedades autocomplete
     * Borrará el item de la pantalla y eliminará el item del input oculto (inputHack) que es el que recoge todos los valores seleccionados
     * @param {jqueryObject} itemDeleted      
     * @param {jqueryObject} deleteInTextoLibre Indica si hace falta eliminar el item de textoLibre       
     * @param {boolean} deleteFromTextoLibre Indica si hay que borrarlo de Texto o de propiedad 
     */
     handleClickBorrarTagAutocompleteProperty: function(itemDeleted, deleteInTextoLibre = false, deleteFromTextoLibre = false){
        const that = this;

        // Panel o sección donde se encuentra el panel de Tags a Eliminar (input_hack)
        const panelTagItem = itemDeleted.parent().parent();

        // Buscar el input oculto y seleccionar la propiedad corresponde con item a eliminar
        const idItemDeleted = itemDeleted.prop("title"); 
        // Items id dependiendo del tipo a borrar (Perfil, Grupo)
        let itemsId = "";
        // Input del que habrá que eliminar el item seleccionado
        let inputPropiedadesAutocompletar = that.inputPropiedadesAutocompletar;

        if (deleteFromTextoLibre == true){
            inputPropiedadesAutocompletar = that.inputTextoLibreAutocompletar;
        }

        let $inputHack = panelTagItem.find(inputPropiedadesAutocompletar);
        
        itemsId = $inputHack.val().split(",");
        itemsId.splice( $.inArray(idItemDeleted, itemsId), 1 );
        // Pasarle los datos al input hidden
        $inputHack.val(itemsId.join(",")); 
        // Eliminar el item del contenedor visual
        itemDeleted.remove();

        // Eliminar también de la sección TextoLibre
        if (deleteInTextoLibre){
            // Panel/Sección donde se ha realizado toda la operativa
            tagsSection = that.txtPropiedadPalabrasTextoLibre.parent().parent();

            // Contenedor de items donde se añadirá el nuevo seleccionado para su visualización
            tagContainer = tagsSection.find(".tag-list");
            const propertyTextoLibreTag = tagContainer.find($(`[title='${idItemDeleted}']`));      
            propertyTextoLibreTag.remove();
            // Eliminar del inputHack
            $inputHack = tagsSection.find("input[type=hidden]").first();            
            itemsId = $inputHack.val().split(",");
            itemsId.splice( $.inArray(idItemDeleted, itemsId), 1 );
            // Pasarle los datos al input hidden
            $inputHack.val(itemsId.join(","));             
        }
     },    


    /**
     * Método para configurar autocomplete de un input de tipo Texto en particular
     * @param {jqueryElement} input de tipo sobre el que se configurará el servicio de autocomplete
     * @param {jqueryObject} inputHack Hack que contiene la lista de valores ya existente
     */
    setupInputForAutocomplete: function (input, inputHack ) {

        input.autocomplete(
            null,
            {
                url: $('#inpt_urlServicioAutocompletar').val() + "/AutoCompletarOntologia",
                delay: 0,
                scroll: false,
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
                    lista: inputHack.val().length == 0 ? "" : inputHack.val(),
                }
            }
        );
    },

    /**
     * Método para guardar los datos relativos a administración de búsquedas-> Autocomplete
     */
    handleSave: function(){
        const that = this;

        // Mostrar Loading
        loadingMostrar();

        // Obtener los datos
        that.obtenerDatos();

        // Hacer petición
        GnossPeticionAjax(
        that.urlSave,
        that.Options,
        true
        ).done(function (data) {
            mostrarNotificacion("success", "Los cambios se han guardado correctamente.");             
        }).fail(function (data) {
            mostrarNotificacion("error", "Se ha producido un error al tratar de guardar los cambios. Contacta con el administrador.");            
        }).always(function () {            
            loadingOcultar();
        });
    },
}

/**
  * Operativa para la gestión de Mapa de la comunidad desde "Mapa"
  */
 const operativaGestionMapa = {
    /**
     * Inicializar operativa
     */
     init: function (pParams) {
         this.pParams = pParams;
         this.config(pParams);
         this.configEvents();
         this.configRutas(); 
         this.triggerEvents();            
     },
 
     /**
      * Lanzar comportamientos u operativas necesarias para el funcionamiento de la sección
      */
     triggerEvents: function(){
         const that = this;        
     },   
 
     /**
      * Configuración de las rutas de acciones necesarias para hacer peticiones a backend
      */
     configRutas: function (pParams) {
         this.urlBase = refineURL();
         this.urlSave = `${this.urlBase}/save-map`;                  
     },  
 
    /*
    * Inicializar elementos de la vista
    * */
    config: function (pParams) {        
        // Contenedor modal
        this.modalContainer = $("#modal-container");

        // Input text propiedad latitud
        this.pPropiedadLatitud = $("#pPropiedadLatitud");
        // Input text propiedad longitud
        this.pPropiedadLongitud = $("#pPropiedadLongitud");
        // Input text propiedad ruta
        this.pPropiedadRuta = $("#pPropiedadRuta");
        // Input txt propiedad color ruta
        this.pPropiedadColorRuta = $("#pPropiedadColorRuta");

        this.btnGuardarMapa = $("#btnGuardarMapa");
     },   
  
    /**
    * Configuración de eventos de elementos del Dom (Botones, Inputs...)      
    * @param {*} pParams Parámetros adicionales pasados desde la View
    */
    configEvents: function (pParams) {
         const that = this;
 
         // Comportamientos del modal container 
         this.modalContainer.on('show.bs.modal', (e) => {
             // Aparición del modal
             // Establecer quién ha disparado el modal para poder cambiar sus atributos una vez se guarden los grupos desde el modal
             that.triggerModalContainer = $(e.relatedTarget);                              
         })
         .on('hide.bs.modal', (e) => {
             // Acción de ocultar el modal
         })
         .on('hidden.bs.modal', (e) => {
             // Vaciar el modal
             resetearModalContainer(); 
         });
                                                                                                 
         // Click para guardar los datos en backend
         this.btnGuardarMapa.on("click", function(){
             that.handleSave();
         });
              
     },


    /**
    * Método para guardar los datos de configuración del mapa.
    */
    handleSave: function(){
        const that = this;                
        that.pMapa = {};
        
        loadingMostrar();
        // Construir objeto para envío a backend
        that.pMapa.Latidud = that.pPropiedadLatitud.val();
        that.pMapa.Longitud = that.pPropiedadLongitud.val();
        that.pMapa.Ruta = that.pPropiedadRuta.val();
        that.pMapa.ColorRuta = that.pPropiedadColorRuta.val();
                                
        GnossPeticionAjax(
            that.urlSave,
            that.pMapa,
            true
        ).done(function (data) {            
            mostrarNotificacion("success", "Los cambios se han guardado correctamente.");
        }).fail(function (data) {
            mostrarNotificacion("error", "Se ha producido un error al tratar de guardar la configuración del mapa. Contacta con el administrador.");
        }).always(function () {
            loadingOcultar();
        });                        
     },
 }


 /** Operativa para la gestión de Gráficos o Charts que se utilizaban en la versión anterior. Es accesible 
  * desde la url: http://depuracion.net/comunidad/testing-publica/administrar-charts
  */
const operativaGestionCharts = {

    /**
     * Inicializar operativa
     */
     init: function (pParams) {
        this.pParams = pParams;
        // Permitir o no guardar datos (CI/CD -> Pasado desde la vista)
        if (this.allowSaveData == undefined){
            this.allowSaveData = pParams.allowSaveData;
        }        
        this.config(pParams);
        this.configEvents();
        this.configRutas(); 
        this.triggerEvents();                    
    },

    /**
     * Configuración de las rutas de acciones necesarias para hacer peticiones a backend
     */
    configRutas: function (pParams) {
        // Url de la página actual donde se encuentra el listado de páginas        
        this.urlGraph = refineURL();
        // Url para crear un gráfico
        this.urlAddNewGraph = `${this.urlGraph}/new-fila`;
        // Url para guardar todos los gráficos        
        this.urlSaveGraphs = `${this.urlGraph}/save-charts`; 
        this.urlDeleteGraph = `${this.urlGraph}/delete-chart`; 

        // Variable que contendrá la fila o página activa. Por defecto "undefined"
        this.filaGrafico = this.filaGrafico != undefined ? this.filaGrafico : undefined;
    },     
    

    /*
     * Inicializar elementos de la vista
     * */
    config: function (pParams) {
            
        /* Idiomas Tabs */
        // Tab de idioma de la página para cambiar la visualización en el idioma deseado de las páginas
        this.tabIdiomaItem = $(".tabIdiomaItem ");
        // Cada una de las labels (Nombre y Url) en los diferentes idiomas (por `data-languageitem`) que contiene el valor en el idioma correspondiente
        this.labelLanguageComponentClassName = "language-component";
        // Clase de cada una fila de las páginas existentes
        this.graphRowClassName = "graph-row";
        // Clase del icono para permitir la ordenación de gráficos
        this.sortableGraphIconClassName = "js-component-sortable-graph";        
        // Botón para guardado de todas la estructura de las páginas
        this.btnGuardarEstructuraPaginas = $('#btnGuardarPaginas');
        // Botón para guardado de un gráfico
        this.btnSaveGraphClassName = "btnGuardarGrafico";
        // Input para buscar gráficos
        this.inputTxtBuscarGrafico = $('#txtBuscarGrafico');
        // Contenedor de todas los gráficos
        this.graphListContainerId = "id-added-graphs-list";
        this.graphListContainer = $(`#${this.graphListContainerId}`);

        // Botones para añadir páginas: Tipo de páginas según su data-pagetype*/
        this.btnAddGraph = $("#btnCrearGrafico");
        // Botón para editar el gráfico. Ejecutará el mostrado del modal para su edición        
        this.btnEditGraphClassName = "btnEditGraph";
        // Botón para eliminar el gráfico
        this.btnDeleteGraphClassName = "btnDeleteGraph";
        // Botón para confirmar el borrado del gráfico (Modal)
        this.btnConfirmDeleteGraph = $(".btnConfirmDeleteGraph");
        // Botón de no confirmar el borrado de la página (Modal)
        this.btnNotConfirmDeleteGraph = $(".btnNotConfirmDeleteGraph");        
        
       
        // Modal container dinámico
        this.modalContainer = $('#modal-container');
        // Modal de cada una de los gráficos
        this.modalGraphClassName = "modal-graph";
        // Modal de eliminación de gráficos
        this.modalDeleteGraphClassName = "modal-confirmDelete";
        
        // Flag para detectar error en el nombre del input vacío
        this.errorNombreVacio = false;
        this.errorDuranteObtenerDatosGraph = false;

        /* Flags para confirmar la eliminación de una página */
        this.confirmDeleteGraph = false;
        // Flag para indicar el item a borrar para ser eliminado una vez se guarde
        this.pendingToBeRemovedClassName = "pendingToBeRemoved";

        // Contador de páginas
        this.numGraficos = $("#numGraficos");

    }, 

    /**
     * Lanzar comportamientos u operativas necesarias para el funcionamiento de la sección
     */
     triggerEvents: function(){
        const that = this;
        
        // Operativa multiIdiomas
        // Parámetros para la operativa multiIdioma (helpers.js)
        this.operativaMultiIdiomaParams = {
            // Nº máximo de pestañas con idiomas a mostrar. Los demás quedarán ocultos
            numIdiomasVisibles: 3,
            // Establecer 1 tab por cada input (true, false) - False es la forma vieja
            useOnlyOneTab: true,
            panContenidoMultiIdiomaClassName: "panContenidoMultiIdioma",
            // No permitir padding bottom y si padding top
            allowPaddingBottom: false,            
        };  

        // Inicializar operativa multiIdioma
        operativaMultiIdioma.init(this.operativaMultiIdiomaParams);  
        
        // Setup de sortable tabla para los gráficos        
        setupBasicSortableList(that.graphListContainerId, that.sortableGraphIconClassName, undefined, undefined, undefined, () => {that.setCurrentDraggedGraphToSave(event)});

    },     
    
    /**
     * Configuración de eventos de elementos del Dom (Botones, Inputs...)     
     */
     configEvents: function (pParams) {
        const that = this;
                       
        this.modalContainer.on('show.bs.modal', (e) => {
            // Aparición del modal
            that.triggerModalContainer = $(e.relatedTarget);
        })
        .on('hide.bs.modal', (e) => {
            // Acción de ocultar el modal
        })
        .on('hidden.bs.modal', (e) => {
            // Vaciar el modal
            resetearModalContainer();            
        });

        // Comportamientos del modal de editar/crear
        configEventByClassName(`${that.modalGraphClassName}`, function(element){
            const $modal = $(element);
            $modal
            .on('show.bs.modal', (e) => {
                // Aparición del modal
                that.triggerModalContainer = $(e.relatedTarget);
            })
            .on('hide.bs.modal', (e) => {
                // Acción de ocultar el modal
                // El modal que se va a ocultar/cerrar
                const currentModal = $(e.currentTarget);                                
                that.handleCloseGraphModal(currentModal);
            }); 
        });        

        // Comportamientos del modal que son individuales para el borrado de gráficos
        $(`.${this.modalDeleteGraphClassName}`).on('show.bs.modal', (e) => {
            // Aparición del modal
        })
        .on('hide.bs.modal', (e) => {
            // Acción de ocultar el modal
            // Si el modal se cierra sin confirmar borrado, desactiva el borrado
            if (that.confirmDeleteGraph == false){
                that.handleSetDeleteGraph(false);                
            }            
        });        

        // Tab de cada uno de los idiomas disponibles para cambiar la visualización y poder ver el nombre y url del idioma elegido en la página principal
        this.tabIdiomaItem.off().on("click", function(){
            that.handleViewGraphLanguageInfo();
        });

        // Botón/es para añadir un gráfico a la comunidad
        this.btnAddGraph.off().on("click", function(){
            that.handleAddNewGraph();            
        });

        // Input para realizar búsquedas de páginas
        this.inputTxtBuscarGrafico.off().on("keyup", function (event) {
            clearTimeout(that.timer);
            that.timer = setTimeout(function () {                                  
                that.handleSearchGraphByTitle();
            }, 500);
        });
        
        // Botón/es para eliminar una determinado gráfico al pulsar en el botón de papelera
        configEventByClassName(`${that.btnDeleteGraphClassName}`, function(element){
            const $btnDelete = $(element);
            $btnDelete.off().on("click", function(){                
                // Fila correspondiente
                that.filaGrafico = $btnDelete.closest(`.${that.graphRowClassName}`);                
                // Marcar el parámetro como "Eliminar" a modo informativo al usuario
                that.handleSetDeleteGraph(true);
            });	                        
        });        

        // Botón/es para confirmar la eliminación de un gráfico (Modal -> Sí)
        this.btnConfirmDeleteGraph.off().on("click", function(){
            that.confirmDeleteGraph = true;
            that.handleDeleteGraph();
        });

        // Botón para editar un determinado gráfico
        configEventByClassName(`${that.btnEditGraphClassName}`, function(element){
            const $editButton = $(element);
            $editButton.off().on("click", function(){                
                // Establecer la clase de "modified" a la fila
                that.filaGrafico = $(this).closest(`.${that.graphRowClassName}`);
                that.filaGrafico.addClass("modified");                
            });	                        
        });  

        // Botón para guardar un determinado gráfico
        configEventByClassName(`${that.btnSaveGraphClassName}`, function(element){
            const $saveButton = $(element);
            $saveButton.off().on("click", function(){                
                // Establecer la clase de "modified" a la fila
                that.filaGrafico = $(this).closest(`.${that.graphRowClassName}`);
                that.handleSaveCurrentGraph(true);               
            });	                        
        });  
    }, 


    /**
     * Método que se ejecuta al cambiar el valor de un input. Esta función está enlazada desde la vista, pero enlazada desde el helpers.js ya que 
     * se utiliza para ejecutar un comportamiento sobre inputs dinámicos generados de forma dinámica mediante la operativaMultiIdioma.
     * @param {*} event 
     */
    handleOnInputTabNameChanged: function(event){
        const that = this;
        // Input que ha sido actualizado
        const input = $(event.currentTarget);
        // Idioma del input que ha sido actualizado
        const language = input.data("language");
        // Valor actual del input
        const value = input.val().trim();

        // Página a actualizar
        const pageRow = input.closest(`.${that.graphRowClassName}`);
        // Nombre a actualizar en el listado de páginas        
        let componentPageName = undefined ;
        
        if (language != undefined){
            componentPageName = pageRow.find(`.${that.componentPageNameClassName}`).filter(`[data-languageitem='${language}']`);
        }else{
            componentPageName = pageRow.find(`.${that.componentPageNameClassName}`);
        }
                
        // Actualizar el contenido de la faceta
        componentPageName.html(value);
    },

    /**
     * Método que se ejecutará cuando se cierre el modal de detalles de una página
     * @param {jqueryObject} currentModal Modal que se va a cerrar
     */
    handleCloseGraphModal: function(currentModal){
        const that = this;

        // Hacer scroll top cuando se cierre el modal
        scrollInModalViewToTop(currentModal);
        // Colapsar todos los menús desplegables del modal para que no se queden abiertos
        const collapsePanels = currentModal.find('.collapse');
        collapsePanels.collapse("hide");
        // Quitar la opción de guardar cambios si no se pulsa en "Guardar". Si se pulsa en guardar se procederá al guardado
        that.filaGrafico.removeClass("modified");

        // Tener en cuenta si el gráfico es de reciente creación y por tanto no se desea guardar
        if (that.filaGrafico.hasClass("newGraph")){
            // Eliminar la fila que se acaba de eliminar                                
            that.filaGrafico.remove();
            // Actualizar el contador de nº de gráficos           
            that.updateNumGraphs();
        }
    },
    
    /**
     * Método para realizar búsquedas de gráficos
     */
    handleSearchGraphByTitle: function(){
        const that = this;
        let cadena = this.inputTxtBuscarGrafico.val();
        // Eliminamos posibles tildes para búsqueda ampliada
        cadena = cadena.normalize('NFD').replace(/[\u0300-\u036f]/g, "").toLowerCase();
        
        // Cada una de las filas que muestran la página
        const rowGraphs = that.graphListContainer.find(".component-wrap.graph-row");        

        // Buscar dentro de cada fila       
        $.each(rowGraphs, function(index){
            const rowGraph = $(this);
            // Seleccionamos el nombre de la página y quitamos caracteres extraños, tiles para poder hacer bien la búsqueda
            const pageName = $(this).find(".component-name").not('.d-none').html().trim().normalize("NFD").replace(/[\u0300-\u036f]/g, "").toLowerCase();            
            if (pageName.includes(cadena)){
                // Mostrar fila resultado y sus respectivos padres
                rowGraph.removeClass("d-none");
            }else{
                // Ocultar fila resultado
                rowGraph.addClass("d-none");
            }            
        });
    },

    /**
     * Método para poder cambiar entre idiomas y poder visualizar los datos de los gráficos en el idioma deseado sin tener que acceder al modal de cada gráfico.
     * Gestionará el click en el tab de idiomas principal del gráfico
     */
    handleViewGraphLanguageInfo: function(){
        const that = this;
        // Comprobar el item que está activo en el tab obteniendo el data-language de la opción "active"
        setTimeout(function(){             
            // Detectar el tab item activo para conocer el idioma en el que se desean mostrar las páginas
            const tabLanguageActive = that.tabIdiomaItem.filter(".active");
            // Obtener el idioma del tabLanguageActivo
            const languageActive = tabLanguageActive.data("language");
            // Ocultar todas las labels y mostrar únicamente las del idioma seleccionado
            $(`.${that.labelLanguageComponentClassName}`).addClass("d-none");
            // Mostrar sólo las labelsLanguageComponent del idioma seleccionado
            $(`.${that.labelLanguageComponentClassName}`).filter(function () {
                return $(this).data("languageitem") == languageActive;
            }).removeClass("d-none");

        },250);
    },

    
    /**
     * Método para añadir una nuevo gráfico
     */
    handleAddNewGraph: function(){
        const that = this;

        // Mostrar loading hasta que finalice la petición para crear una nueva página
        loadingMostrar();    
        // Construir el objeto/modelo para petición
        /*const dataPost = {
            TipoPestanya: pageType,
            nameonto: nameonto
        }*/

        // Realizar la petición de la página a crear
        GnossPeticionAjax(                
            that.urlAddNewGraph,
            null,
            true
        ).done(function (data) {                   
            // Añadir la nueva fila al listado
            that.graphListContainer.append(data);
            // Referencia a la nueva página añadida
            const newGraph = that.graphListContainer.children().last();
            // Iniciar operativa de multiIdioma (Desde "triggerEvents" Para nombre del gráfico)            
            operativaGestionCharts.triggerEvents()
            // Abrir el modal para poder editar/gestionar la nueva página añadida                     
            newGraph.find(`.${that.btnEditGraphClassName}`).trigger("click"); 
            // Guardar la fila del gráfico activo para detectar su borrado si hiciera falta
            that.filaGrafico = newGraph;                              
            // Aumentar el nº del contador de gráficos
            that.updateNumGraphs();                            
        }).fail(function (data) {
            // Mostrar error al tratar de crear una nueva página
            mostrarNotificacion("error", data);                
        }).always(function () {
            // Ocultar loading de la petición
            loadingOcultar();
        });                    
    },


    /**
     * Método para marcar o desmarcar el gráfico como "Eliminada" dependiendo de la elección vía Modal
     * @param {Bool} deleteItem que indicará si se desea eliminar o no el gráfico
     */
    handleSetDeleteGraph: function(deleteItem){
        const that = this;

        if (deleteItem){
            // Realizar el "borrado" del gráfico
            // 1 - Marcar la opción "TabEliminada" a true
            that.filaGrafico.find('[name="TabEliminada"]').val("true");           
            // 2 - Añadir la clase de "deleted" a la fila
            that.filaGrafico.addClass("deleted");
        }else{
            // 1 - Marcar la opción "TabEliminada" a false
            that.filaGrafico.find('[name="TabEliminada"]').val("false");            
            // 2 - Eliminar la clase de "deleted" a la fila
            that.filaGrafico.removeClass("deleted");
        }
    },  
    
    /**
     * Método para eliminar un gráfico previa confirmación realizada desde el modal     
     */
    handleDeleteGraph: function(){
        // Mostrar loading
        loadingMostrar();

        const that = this;     

        that.elementoDelete = $(".deleted");
        that.ChartID = that.elementoDelete.data("graphkey");

        const dataPost = {
            ChartID: that.ChartID
        }

        GnossPeticionAjax(                
            that.urlDeleteGraph,
            dataPost,
            true
        ).done(function (data) {                   
           // Resetear el estado de borrado del gráfico
           that.confirmDeleteGraph = false;                
           // 2 - Ocultar el modal de eliminación de la página
           //const modalDeletePage = $(that.filaPagina).find(`.${that.modalDeletePageClassName}`);                                          
           const modalDeleteGraph = $(`.${that.modalDeleteGraphClassName}`);                                          
           dismissVistaModal(modalDeleteGraph);
           // 3 - Ocultar loading
           loadingOcultar();                
           // 4 - Ocultar la fila que se desea eliminar
           that.filaGrafico.addClass("d-none");
           // 5 - Eliminar la fila que se acaba de eliminar                                
           that.filaGrafico.remove();
           // 6 - Actualizar el contador de nº de páginas            
            that.updateNumGraphs(); 

            mostrarNotificacion("succes", "Se ha eliminado el gráfico correctamente.");
        }).fail(function (data) {
            mostrarNotificacion("error", "Se ha producido un error al tratar de eliminar el gráfico. Contacta con el administrador de la comunidad.");
        }).always(function () {
            // Ocultar loading de la petición
            loadingOcultar();
        });                      
    }, 
    
    /**
     * Método para actualizar el nº de gráficos existentes (Añadir, Eliminar...)      
     */
    updateNumGraphs: function(){
        const that = this;

        const graphNumber = this.graphListContainer.find($(`.${that.graphRowClassName}`)).length;
        // Mostrar el nº de items                
        that.numGraficos.html(graphNumber);

        // No mostrar el item si no hay resultados
        graphNumber == 0 ? that.numGraficos.addClass("d-none") : that.numGraficos.removeClass("d-none");
    },


    /* Guardado de items */
    /************************************************************/
    
    /**
     * Método para el guardado del gráfico del gráfico en el que se encuentra el usuario
     * @param {bool} reloadPage Indicar si se desea recargar la página actual. Por defecto true.
     */
    handleSaveCurrentGraph: function(reloadPage = false){
        const that = this;

        that.handleSaveGraphs(function(error){
            if (error == false){
                // 2 - Ocultar el modal
                const modalGraph = $(that.filaGrafico).find(`.${that.modalGraphClassName}`);                                          
                mostrarNotificacion("success","Los cambios se han guardado correctamente");
                dismissVistaModal(modalGraph);
                // Resetear la fila que se ha guardado
                that.filaGrafico = undefined;  
                if (reloadPage == true){                    
                    location.reload();
                }                                             
            }
        });
    },

    /**
     * Método que construirá un objeto para conocer la lista de gráficos y su orden
     * @returns 
     */
    handleGetListGraphWithCurrentOrder: function(){
        const that = this;
        // Lista de los gráficos con su orden
        const graphListWithOrder = [];
        const currentGraphList = $(`.${that.graphRowClassName}`);

        // Construir la lista con el orden y el id
        $.each(currentGraphList, function () {
            const graphListItem = $(this);
            const graphListItemId = graphListItem.prop("id");
            const graphListItemOrden = graphListItem.index();
            const currentGraphListInfo = {
                chartId: graphListItemId,
                orden: graphListItemOrden,
            }            
            graphListWithOrder.push(currentGraphListInfo);
        }); 

        return graphListWithOrder;        
    },
    
    
    /**
     * Método que establecerá la fila que está siendo movida para proceder al guardado correcto.
     */
    setCurrentDraggedGraphToSave: function(event){
        const that = this;

        // Obtener la fila que está siendo movida o "dragged"                
        const iconFromDraggedRow = $(event.target);        
        that.filaGrafico = iconFromDraggedRow.closest(`.${that.graphRowClassName}`);  
        // Proceder al guardado de la fila del gráfico
        that.handleSaveCurrentGraph();
    },


    /**
     * Método que se ejecutará cuando se pulse en el botón de "Guardar" para proceder al guardado de la información     
     * @param {*} completion : Acciones a realizar una vez finalice el procesado de guardardo
     */
    handleSaveGraphs: function ( completion ) {
        var that = this;
        // Mostrar loading
        loadingMostrar();
        
        // Lista de items
        const rowGraphs = that.graphListContainer.find(".component-wrap.graph-row");

        // Se desean guardar los gráficos  -> Quitar clase de "newPage" para NO eliminar la página de reciente creación
        // $(".newGraph").removeClass("newGraph");

        // Resetear los errores globales previos (Flags urlRepetidos, urlVacíos,)
        that.errorInputVacio = false;
        
        // Comprobar que no hay errores antes de proceder con el guardado
        if (!that.comprobarErroresGuardado()) {
            // Listado de items donde se irán añadiendo para su posterior guardado
            that.ListaGraficos = {};
            let cont = 0;    
            
            // Guardado de Santi            
            that.Name = that.filaGrafico.find('[name="TabName"]').val();
            that.Select = that.filaGrafico.find('[name="TextoSelect"]').val();
            that.Where = that.filaGrafico.find('[name="TextoWhere"]').val();
            that.Javascript = that.filaGrafico.find('[name="TextoJavascript"]').val();
            that.FuncionJS = that.filaGrafico.find('[name="NombreFuncionJS"]').val();
            that.Eliminada = that.filaGrafico.find('[name="TabEliminada"]').val();
            that.ChartID = that.filaGrafico.find('[name="TabChartID"]').val();
            // Obtener el orden real
            that.Orden = that.filaGrafico.index();// that.filaGrafico.find('[name="TabOrden"]').val();
            const listGraphItemWithCurrentOrder = that.handleGetListGraphWithCurrentOrder();                 
            
            const dataPost = {
                Where: that.Where,
                Nombre: that.Name,
                Select: that.Select,
                Javascript: that.Javascript,
                FuncionJS: that.FuncionJS,
                ChartID: that.ChartID,
                Orden: that.Orden,
                Eliminada: that.Eliminada,
                ChartViewInfoOrderList: listGraphItemWithCurrentOrder,
            };

            GnossPeticionAjax(
                that.urlSaveGraphs,
                dataPost,
                true
            ).done(function (data) {    
                // Establecer el orden en el input correspondiente
                const newOrden = that.filaGrafico.index();
                that.filaGrafico.find('[name="TabOrden"]').val(newOrden);                
                completion != undefined && completion(false);        
            }).fail(function (data) {
                mostrarNotificacion("error",data);
                completion != undefined && completion(true);        
            }).always(function () {
                // Ocultar loading de la petición
                loadingOcultar();
                that.filaGrafico.removeClass("modified");
            });  
            //*************/

            // Recorrer cada página para obtener los datos y guardarlos            
            /*rowGraphs.each(function () {
                if (that.errorDuranteObtenerDatosPestaya == false){
                    that.obtenerDatosPestanya($(this), cont++);
                }                
            });

            // Realizar Guardado de páginas si no se han producido errores
            if (!that.errorDuranteObtenerDatosPestaya){
                that.savePages( function(savePagesError){
                    error = savePagesError;
                    // Resetear flag de confirmar eliminación de página
                    if (error == true){that.confirmDeleteGraph = false;} 
                    loadingOcultar();
                    // Eliminar los items pendientes de ser eliminados y no mostrados una vez se ha guardado la página
                    $(`.${that.pendingToBeRemovedClassName}`).remove();
                    completion != undefined && completion(error);
                });                
            }else{
                // Se ha producido algún error durante el guardado
                completion != undefined && completion(true);
                loadingOcultar();
            }
            */
        }
        else {
            loadingOcultar();
        }

    },

    /**
     * Método que mostrará un mensaje si se ha producido un error durante el proceso de guardado.
     */
    mostrarErrorGuardado: function(){
        var esPre = "False";
        var entornoBloqueado = "False";

            if (esPre == "True") {
                $('input.guardarTodo').before('<div class="error general">No se permite guardar porque estás en el entorno de preproducción</div > ');
            }
            else if (entornoBloqueado == "True") {
                $('input.guardarTodo').before('<div class="error general">El entorno actual esta bloqueado. Hay que desplegar la versión de preproducción antes de hacer cambios</div>');
            }
            else {
                $('input.guardarTodo').before('<div class="error general">Ha habido errores en el guardado de las pesta&#xF1;as, revisa los errores marcados</div > ');
            }
    },

    /**
     * Método que comprobará si hay algún error antes de proceder con el guardado de las páginas. Se ejecuta desde 'handleSavePages'.
     * @returns bool: Devolverá un valor booleano indicando si hay algún error o no.
     */
    comprobarErroresGuardado: function(){
        const that = this;
        let error = false;

        // Comprobación del nombre de las páginas (Vacíos)
        if (that.comprobarNombresVacios()) {
            error = true;
        }

        // Comprobar que hay datos en los diferentes valores de los gráficos
        /*
        if (!error && that.comprobarInputsGraficoVacios()) {
            error = true;
        }
        */       
               
        return error;
    },

    /**
     * Método para comprobar que no inputs vacíos para hacer el guardado
     * @returns bool: Devuelve un valor booleano indicando si todo está OK para proceder con el guardado de las páginas.
     */
    comprobarInputsGraficoVacios: function(){        
        const that = this;              
        // Inputs con las rutas de páginas que hayan sido modificadas y no borradas (Más velocidad)
        const inputsRutas = $('.graph-row:not(".deleted").modified input[name="TabUrl"]:not(":disabled")');
        inputsRutas.each(function () {
            // Comprobar las rutas de las páginas para que no estén repetidas
            if (that.errorRutaRepetida == false && that.errorRutaVacia == false){
                that.comprobarUrlRepetida($(this))
            }            
        });
        return that.errorRutaRepetida || that.errorRutaVacia;
    },

    /**
     * Método para comprobar que no hay Nombres vacíos en las páginas creadas/editadas
     * @returns bool: Devuelve un valor booleano indicando si todo está OK para proceder con el guardado de las páginas.
     */
    comprobarNombresVacios: function(){
        const that = this;        
        // Inputs con los nombres de todas las páginas que hayan sido modificadas y no borradas (Más velocidad)        
        const inputsNombre = $('.graph-row:not(".deleted").modified .inputsMultiIdioma.basicInfo input[name="TabName"]');
        inputsNombre.each(function () {
            if (that.errorNombreVacio == false){                
                that.comprobarNombreVacio($(this));
            }  
        });
        return that.errorNombreVacio;
    },

    /**
     * Método para comprobar que el nombre de una página no está vacío.
     * @param {*} inputName: Input a comprobar
     * @returns bool: Devuelve un valor booleano indicando si todo está OK para proceder con el guardado de las páginas.
     */
    comprobarNombreVacio: function (inputName) {
        const that = this
        
        //const panMultiIdioma = $('#edicion_multiidioma', inputName.parent());
        // Panel multiIdioma donde se encuentra el input
        // Contiene los tabs en idiomas y el div con los inputs en idiomas completo
        const formularioEdicion = inputName.closest(".formulario-edicion");
        const panInputsMultiIdioma = inputName.closest(".inputsMultiIdioma");
        // Contenedor donde se encuentran datos básicos de la página (Nombre, Url/Ruta, MetaDescripción )
        const panMultiIdioma = formularioEdicion.find(".panContenidoMultiIdioma.basicInfo");

        // Id del input a tratar
        const inputId = inputName.attr("id");

        const listaTextos = [];
        
        if (operativaMultiIdioma.listaIdiomas.length > 1 && panMultiIdioma.length > 0) {
            let textoMultiIdioma = "";
            // Comprobar que hay al menos un texto por defecto para el nombre
            let textoIdiomaDefecto = panMultiIdioma.find(`#input_${inputId}_${operativaMultiIdioma.idiomaPorDefecto}`).val(); //panMultiIdioma.find('#edicion_' + that.IdiomaPorDefecto + ' input').val();
            
            if (textoIdiomaDefecto == null || textoIdiomaDefecto == "") {
                const fila = inputName.closest(".component-wrap.graph-row");
                that.mostrarErrorNombreVacio(fila, panMultiIdioma.find(`#input_${inputId}_${operativaMultiIdioma.idiomaPorDefecto}`));
                that.errorNombreVacio = true;
                return true;
            }
            // Recorrer todos los idiomas para detectar posibles problemas con el nombre                
            $.each(operativaMultiIdioma.listaIdiomas, function () {
                // Obtención del Key del idioma
                const idioma = this.key;
                // Asignar el valor por defecto de la ruta al idioma si este no dispone de valor para Nombre
                let textoIdioma = $(`#input_${inputId}_${idioma}`).val();
                if (textoIdioma == null || textoIdioma == "") {
                    textoIdioma = textoIdiomaDefecto;
                    $(`#input_${inputId}_${idioma}`).val(textoIdioma);
                }
                // Escribir el nombre del multiIdioma en el campo Hidden
                textoMultiIdioma += textoIdioma + "@" + idioma + "|||";
                
                listaTextos.push({ "key": idioma, "value": textoIdioma });
            });            
            inputName.val(textoMultiIdioma);
        }else{
            // Sin multiIdioma.
            let textoIdiomaDefecto = panMultiIdioma.find(`#input_${inputId}_${operativaMultiIdioma.idiomaPorDefecto}`).val();
            // Establecer el nombre en el input correspondiente
            inputName.val(textoIdiomaDefecto);
        }
        return false;
    },  
    
    /**
     * Método para mostrar el error del nombre vacío encontrado en una página. Se muestra cuando el nombre de una página en el idioma por defecto no existe.
     * @param {jqueryObject} fila : Elemento jquery de la fila correspondiente a donde se ha encontrado el error.
     * @param {jqueryObject} input : Elemento jquery del input donde se ha producido el error. Puede que ser undefined
     */
    mostrarErrorNombreVacio: function(fila, input){        
        /*var inputUrl = $('input[name = "TabName"]', fila).first();*/                
        const btnEditPage = $(".btnEditPage", fila);
        // Abrir el modal para acceder a modificar el nombre de la página
        btnEditPage.trigger( "click" );

        if (input != undefined){
            comprobarInputNoVacio(input, true, false, "El nombre de la página no puede estar vacío.", 0);
        }

        setTimeout(function(){  
            mostrarNotificacion("error", "El nombre de la página no puede estar vacío.");
        },1000);  
    },


    // ELIMINAR POSIBLES MÉTODOS 

    /**
     * Método para comprobar y recoger los textos por defecto para el buscador de una página semántica
     * @returns bool: Devuelve un valor booleano indicando si todo está OK para proceder con el guardado de las páginas.
     */    
    comprobarTextosBuscadorPorDefecto: function(){
        const that = this;        
        // Inputs con los nombres de todas las páginas que hayan sido modificadas y no borradas (Más velocidad)        
        const inputsTextoBuscadorPorDefecto = $('.page-row:not(".deleted").modified .inputsMultiIdioma.basicInfo input[name="TabFiltroTextoDefectoBuscador"]:not(":disabled")');
        inputsTextoBuscadorPorDefecto.each(function () {            
            that.comprobarTextoBuscadorPorDefecto($(this));              
        });
        // Nunca habrá error ya que el texto puede ser vacío
        return false;
    },

     /**
     * Método para comprobar que el nombre de una página no está vacío.
     * @param {*} inputTextoDefecto: Input a comprobar
     * @returns bool: Devuelve un valor booleano indicando si todo está OK para proceder con el guardado de las páginas.
     */
    comprobarTextoBuscadorPorDefecto: function(inputTextoDefecto){
        const that = this
        
        // Panel multiIdioma donde se encuentra el input
        // Contiene los tabs en idiomas y el div con los inputs en idiomas completo
        const formularioEdicion = inputTextoDefecto.closest(".formulario-edicion");
        const panInputsMultiIdioma = inputTextoDefecto.closest(".inputsMultiIdioma");
        // Contenedor donde se encuentran datos básicos de la página (Nombre, Url/Ruta, MetaDescripción )
        const panMultiIdioma = formularioEdicion.find(".panContenidoMultiIdioma.basicInfo");

        // Id del input a tratar
        const inputId = inputTextoDefecto.attr("id");

        const listaTextos = [];
        
        if (operativaMultiIdioma.listaIdiomas.length > 1 && panMultiIdioma.length > 0) {
            let textoMultiIdioma = "";
            // Comprobar que hay al menos un texto por defecto para el texto
            let textoIdiomaDefecto = panMultiIdioma.find(`#input_${inputId}_${operativaMultiIdioma.idiomaPorDefecto}`).val(); //panMultiIdioma.find('#edicion_' + that.IdiomaPorDefecto + ' input').val();

            // Recorrer todos los idiomas para detectar posibles problemas con el text                
            $.each(operativaMultiIdioma.listaIdiomas, function () {
                // Obtención del Key del idioma
                const idioma = this.key;
                // Asignar el valor por defecto de la ruta al idioma si este no dispone de valor para texto
                let textoIdioma = $(`#input_${inputId}_${idioma}`).val();
                if (textoIdioma == null || textoIdioma == "") {
                    textoIdioma = textoIdiomaDefecto;
                    $(`#input_${inputId}_${idioma}`).val(textoIdioma);
                }
                // Escribir el texto del multiIdioma en el campo Hidden
                textoMultiIdioma += textoIdioma + "@" + idioma + "|||";
                
                listaTextos.push({ "key": idioma, "value": textoIdioma });
            });            
            inputTextoDefecto.val(textoMultiIdioma);
        }else{
            // Sin multiIdioma.
            let textoIdiomaDefecto = panMultiIdioma.find(`#input_${inputId}_${operativaMultiIdioma.idiomaPorDefecto}`).val();
            // Establecer el nombre en el input correspondiente
            inputTextoDefecto.val(textoIdiomaDefecto);
        }
        return false;
    },


    /**
     * Método para comprobar que el corto de una página no está repetido.
     * @returns bool: Devuelve un valor booleano indicando si todo está OK para proceder con el guardado de las páginas.
     */
    comprobarNombresCortosRepetidos: function(){
        const that = this;
        let errorRepetido = false;
        
        // var inputsNombresCortos = $('.row.pestanya:not(".ui-state-disabled") .modified input[name = "TabShortName"]');
        //const inputsNombresCortos = $('.page-row:not(".deleted") input[name="TabShortName"]:not(":disabled")');
        // Inputs con nombres cortos de páginas que hayan sido modificadas y no borradas (Más velocidad)        
        const inputsNombresCortos = $('.page-row:not(".deleted").modified input[name="TabShortName"]:not(":disabled")');
        
        inputsNombresCortos.each(function () {
            if (that.comprobarNombreCortoRepetido($(this))) {
                errorRepetido = true;
            }
        });
        return errorRepetido;        
    },

    /**
     * Método para comprobar el corto de una determinada página no está repetido.
     * @param {jqueryObject} inputNombreCorto : Input a comprobar
     * @returns bool: Devuelve un valor booleano indicando si todo está OK para proceder con el guardado de las páginas.
     */
    comprobarNombreCortoRepetido: function (inputNombreCorto) {
        var that = this;
        $('.error', inputNombreCorto.parent()).remove();

        var fila = inputNombreCorto.closest('.row');

        var inputsNombresCortos = $('.row.pestanya:not(".ui-state-disabled") input[name = "TabShortName"]');
        var nombreCorto = inputNombreCorto.val();
        var errorRepetido = false;
        var inputRepetido = null;

        if (nombreCorto != "") {
            inputsNombresCortos.each(function () {
                var inputCompare = $(this);
                if (inputCompare.closest('.row').attr('id') != fila.attr('id')) {
                    if (inputCompare.val() == nombreCorto) {
                        errorRepetido = true;
                        inputRepetido = inputCompare;
                    }
                }
            });
        }
        if (errorRepetido) {
            that.mostrarErrorNombreCortoRepetido(fila);
            if (inputRepetido != null) {
                var filaRepetidp = inputRepetido.closest('.row');
                that.mostrarErrorUrlRepetida(filaRepetida);
            }
        }
        return errorRepetido;
    },   
    
    /**
     * Método para mostrar el error debido a que el nombre corto de la página está repetido
     * @param {*} fila 
     */
    mostrarErrorNombreCortoRepetido: function (fila) {
        var inputNombreCorto = $('input[name = "TabShortName"]', fila).first();
        inputNombreCorto.after("<span class=\"error\">El Nombre Corto no puede estar repetido</span>");
        $('.panEdicion', fila).addClass('edit');
        $('.panEdicion', fila).addClass('modified');
    },  

    /**
     * Método para comprobar que la URL de una página no esté repetida. Recorrerá todas las filas buscando los campos URL de los idiomas, y los comparará con 
     * los idiomas del inputUrl.
     * @param {*} inputUrl : Input a comprobar
     * @returns bool: Devuelve un valor booleano indicando si todo está OK para proceder con el guardado de las páginas.
     */
    comprobarUrlRepetida: function(inputUrl){
        const that = this;
        // Panel multiIdioma donde se encuentra el input
        // Contiene los tabs en idiomas y el div con los inputs en idiomas completo
        const formularioEdicion = inputUrl.closest(".formulario-edicion");
        const panInputsMultiIdioma = inputUrl.closest(".inputsMultiIdioma");
        // Contenedor donde se encuentran datos básicos de la página (Nombre, Url/Ruta, MetaDescripción )
        const panMultiIdioma = formularioEdicion.find(".panContenidoMultiIdioma.basicInfo");

        // Id del input a tratar
        const inputId = inputUrl.attr("id");

        const listaTextos = [];

        // Comprobar que hay múltiples idiomas para edición de páginas
        if (operativaMultiIdioma.listaIdiomas.length > 1 && panMultiIdioma.length > 0) {
            let rutaPorDefecto = formularioEdicion.find(`#input_${inputId}_${operativaMultiIdioma.idiomaPorDefecto}`).val(); //panMultiIdioma.find('.edicion_' + that.IdiomaPorDefecto + ' input').val();
            if (rutaPorDefecto == "") {
                that.errorRutaVacia = true;
            }
            else {
                // Input oculto donde se crearán los idiomas asignados para una determinada página
                let textoMultiIdioma = "";
                $.each(operativaMultiIdioma.listaIdiomas, function () {
                    // Obtención del Key del idioma
                    const idioma = this.key;
                    // Acceso al valor del input del idioma analizado
                    //let textoIdioma = panMultiIdioma.find('#edicion_' + idioma + ' input').val();
                    let textoIdioma = $(`#input_${inputId}_${idioma}`).val();
                    // Asignar el valor por defecto de la ruta al idioma si este no dispone de valor para URL
                    if (textoIdioma == null || textoIdioma == "") {
                        textoIdioma = rutaPorDefecto;                        
                        $(`#input_${inputId}_${idioma}`).val(textoIdioma);
                    }
                    // Escribir el nombre del multiIdioma en el campo Hidden
                    textoMultiIdioma += textoIdioma + "@" + idioma + "|||";
                
                    listaTextos.push({ "key": idioma, "value": textoIdioma });
                });
                inputUrl.val(textoMultiIdioma);
            }
        }else{
            // Sin multiIdioma.
            let textoIdiomaDefecto = panMultiIdioma.find(`#input_${inputId}_${operativaMultiIdioma.idiomaPorDefecto}`).val();
            // Establecer el nombre en el input correspondiente
            inputUrl.val(textoIdiomaDefecto);
        }
        // Fila correspondiente a la página cuyo valor URL está siendo analizada
        const fila = inputUrl.closest('.component-wrap.page-row');

        // Comprobar si hay error por ruta vacía
        if (that.errorRutaVacia) {
            that.mostrarErrorUrlVacia(fila, formularioEdicion.find(`#input_${inputId}_${operativaMultiIdioma.idiomaPorDefecto}`));
            return that.errorRutaVacia;
        }

        // Comprobación de que las Rutas existentes de que no estárán repetidas
        //const inputsRutas = $('.page-row:not(".deleted") input[name="TabUrl"]'); // $('.row.pestanya:not(".ui-state-disabled") input[name = "TabUrl"]');
        // const inputsRutas = $('.page-row:not(".deleted") input[name="TabUrl"]'); // $('.row.pestanya:not(".ui-state-disabled") input[name = "TabUrl"]');
        // Obtener los inputs de las rutas de las páginas menos las disabled (Página Home)
        const inputsRutas = $('.page-row:not(".deleted") input[name="TabUrl"]:not(":disabled")');
        // Obtención de la ruta
        const ruta = inputUrl.val();
        const tipoRuta = $("input[name='TabType']", fila).val();
        let errorRepetida = false;
        let inputRepetida = null;

        // Comprobación del tipo de Enlace
        if (tipoRuta != "EnlaceInterno" && tipoRuta != "EnlaceExterno") {
            if (that.errorRutaRepetida == false){
                inputsRutas.each(function () {
                    // Input a comparar
                    const inputCompare = $(this);
                    // Id del input a comparar
                    const inputCompareId = $(this).attr("id");
                    // Fila que se comparará
                    const filaCompare = inputCompare.closest('.component-wrap.page-row');
                    // Tipo de ruta a comparar
                    const tipoRutaCompare = $("input[name='TabType']", filaCompare).val();
    
                    if (filaCompare.attr('id') != fila.attr('id') && tipoRutaCompare != "EnlaceExterno" && tipoRutaCompare != "EnlaceInterno") {
                        // Panel donde se muestran los datos en los diferentes idiomas de cada página
                        //const panCompareMultiIdioma = $('.inputsMultiIdioma', inputCompare.parent().parent());
                        const panCompareMultiIdioma = inputCompare.parent().parent();
                        
                        if (operativaMultiIdioma.listaIdiomas.length > 1 && panCompareMultiIdioma.length > 0) {
                            // Revisar el input en cada idioma para que no estén repetidas    
                            $.each(operativaMultiIdioma.listaIdiomas, function () {
                                const idioma = this.key;                             
                                // Acceder a los inputs en todos los idiomas
                                const formularioEdicion = inputUrl.closest(".formulario-edicion");
                                const panMultiIdioma = formularioEdicion.find(".panContenidoMultiIdioma.basicInfo");
                                // Contenido del idioma a revisar
                                let textoIdioma = "";
                                // Obtener campos URL diferentes a pagina de tipo Home -> Home no puede cambiar la url
                                if (inputCompareId != undefined){
                                    textoIdioma = $(`#input_${inputCompareId}_${idioma}`).val();
                                    // Establecer en el idioma la url con el idioma por defecto
                                    if (textoIdioma == null || textoIdioma == "") {
                                        // Establecer el valor del idioma por defecto en el idioma que no contiene un valor URL
                                        textoIdioma = $(`#input_${inputCompareId}_${operativaMultiIdioma.idiomaPorDefecto}`).val();
                                        $(`#input_${inputCompareId}_${idioma}`).val(textoIdioma);
                                    }
                                    // Buscar si ya existe la url insertada
                                    if (listaTextos.findValueByKey(idioma) == textoIdioma) {
                                        errorRepetida = true;
                                        inputRepetida = inputCompare;
                                        return;
                                    }
                                }
                            });
                        }
                        else if (operativaMultiIdioma.listaIdiomas.length > 1) {
                            if (inputCompare.val() == "") {
                                $.each(operativaMultiIdioma.listaIdiomas, function () {
                                    let idioma = this.key
                                    if (listaTextos.findValueByKey(idioma) == "") {
                                        errorRepetida = true;
                                        inputRepetida = inputCompare;
                                    }
                                });
                            }
                            else {
                                let textoMultiIdioma = inputCompare.val().split('|||');
    
                                $.each(textoMultiIdioma, function () {
                                    if (this != "") {
                                        /*var objetoIdioma = that.obtenerTextoYClaveDeIdioma(this);
                                        */
                                        if (listaTextos.findValueByKey(objetoIdioma.key) == objetoIdioma.value) {
                                            errorRepetida = true;
                                            inputRepetida = inputCompare;
                                        }
                                    }
                                });
                            }
                        }
                        else {
                            if (inputCompare.val() == ruta) {
                                errorRepetida = true;
                                inputRepetida = inputCompare;
                            }
                        }
                    }
                });
            }
        }
        if (errorRepetida) {
            that.errorRutaRepetida = true;
            that.mostrarErrorUrlRepetida(fila);
            /* Abrir la página con la que está repetida
                if (inputRepetida != null) {
                var filaRepetida = inputRepetida.closest('.component-wrap.page-row');
                that.mostrarErrorUrlRepetida(filaRepetida);
            }*/
        }
        return errorRepetida;
    },

    /**
     * Método para mostrar el error debido a que la URL de una página está repetida
     * @param {jqueryObject} fila : Fila correspondiente al input donde se ha encontrado el error
     */
    mostrarErrorUrlRepetida: function (fila) {
        // var inputUrl = $('input[name = "TabUrl"]', fila).first();
        // inputUrl.after("<span class=\"error\">La Ruta no puede estar repetida</span>");
        // Botón de la página para editar detalles
        const btnEditPage = $(".btnEditPage", fila);
        // Abrir el modal para acceder a modificar la URL de la página
        // btnEditPage.trigger( "click" );
        setTimeout(function(){
            mostrarNotificacion("error", "La ruta de la página no puede estar repetida.");
        },1000);        
    },  
    
    /**
     * Método para mostrar el error debido a que la URL de una página está vacía.
     * @param {*} fila 
     * @param {*} input: Input correspondiente donde se ha encontrado error por estar vacío.
     */
    mostrarErrorUrlVacia: function (fila, input) {
        const btnEditPage = $(".btnEditPage", fila);
        // Abrir el modal para acceder a modificar la URL de la página
        // btnEditPage.trigger( "click" );

        if (input != undefined){
            comprobarInputNoVacio(input, true, false, "La ruta de la página no puede estar vacía.", 0);
        }

        setTimeout(function(){
            mostrarNotificacion("error", "La ruta de la página no puede estar vacía.");
        },1000);          
    },    
    
    /**
     * Método para comprobar el orden de los filtros de orden de las páginas
     * @returns
     */
    comprobarFiltrosOrden: function(){
        const that = this;
                
        //const filtros = $('.page-row:not(".deleted") .filter-row:not(".deleted")'); // $('.row.pestanya:not(".ui-state-disabled") .modified ol.filtrosOrdenSortable li.row.filtroOrden:not(".ui-state-disabled")');
        // Filtro de orden de todas las páginas a revisar que hayan sido modificadas y no borradas (Más velocidad)
        const filtros = $('.page-row:not(".deleted").modified .filter-row:not(".deleted")');

        filtros.each(function () {
            if (that.errorFiltrosOrden == false){
                that.comprobarFiltroOrden($(this));                
            }
        });
        return that.errorFiltrosOrden;
    },


    /**
     * Método para comprobar el orden del filtro de una determinada página.
     * @param {jqueryObject} filaFiltroOrden : Objeto jquery que corresponde con la fila donde se encuentra el filtro
     * @returns : bool: Devuelve un valor que indica si se puede proceder al guardado de las páginas.
     */
    comprobarFiltroOrden: function(filaFiltroOrden){
        const that = this
        
        // Panel donde se podrá localizar el input que almacena el valor del input para el filtroOrden
        const panelOrderFilterInfo = filaFiltroOrden.find('.filter-order-info');
        // Filas de páginas
        const filasPaginas = $("#id-added-pages-list").find(".component-wrap.page-row");
        // Fila de la página cuyos filtros de orden están siendo revisados
        const filaPagina = panelOrderFilterInfo.closest(filasPaginas);

        // Inputs que contendrá todos los idiomas del filtro en concreto y el nombre del filtro
        const inputFilterValueLanguages = panelOrderFilterInfo.find('[name="TabName"]');
        const inputId = inputFilterValueLanguages.attr('id');
        const inputFilterName = panelOrderFilterInfo.find('[name="Filtro"]');


        let textoMultiIdioma = "";
        // Comprobar que hay al menos un texto por defecto para el nombre del filtro
        let textoIdiomaDefecto = $(`#input_${inputId}_${operativaMultiIdioma.idiomaPorDefecto}`).val();
        if (textoIdiomaDefecto == null || textoIdiomaDefecto == "") {
            const fila = filaFiltroOrden.closest(".component-wrap.page-row");
            that.mostrarErrorCampoFiltroOrdenVacio(filaFiltroOrden, filaPagina, $(`#input_${inputId}_${operativaMultiIdioma.idiomaPorDefecto}`));
            that.errorFiltrosOrden = true;
            return that.errorFiltrosOrden;
        }
        
        // Recorrer el contenido de los inputs en los diferentes idiomas al input correspondiente
        $.each(operativaMultiIdioma.listaIdiomas, function () {
            // Obtención del Key del idioma
            const idioma = this.key;
            // Asignar el valor por defecto de la ruta al idioma si este no dispone de valor para el Nombre del filtro
            let textoIdioma = $(`#input_${inputId}_${idioma}`).val();
            if (textoIdioma == null || textoIdioma == "") {
                textoIdioma = textoIdiomaDefecto;
                $(`#input_${inputId}_${idioma}`).val(textoIdioma);
            }
            // Escribir el nombre del multiIdioma en el campo Hidden
            textoMultiIdioma += textoIdioma + "@" + idioma + "|||";
        });            
        inputFilterValueLanguages.val(textoMultiIdioma);


        // Comprobar que el filtro orden tiene asignado nombre y valor
        if (inputFilterValueLanguages.val().trim() == "" || inputFilterName.val().trim() == "") {
            that.mostrarErrorCampoFiltroOrdenVacio(filaFiltroOrden, filaPagina, $(`#input_${inputId}_${operativaMultiIdioma.idiomaPorDefecto}`));
            that.errorFiltrosOrden = true;
            return that.errorFiltrosOrden;
        }
        return that.errorFiltrosOrden;
    },

    /**
     * Método para mostrar el error de que una página tiene un filtro de orden no tiene valor o no tiene nombre asignado.
     * @param {jqueryObject} filaFiltroOrden : Fila del Filtro orden que no tiene valor
     * @param {jqueryObject} filaPagina : Fila de la página que está siendo revisada
     */
    mostrarErrorCampoFiltroOrdenVacio: function (filaFiltroOrden, filaPagina, input){

        // Obtener el identificador de la página con error
        const pageId = filaPagina.attr("id");
        // Construcción del identificador del modal
        const modalId = `modal-configuracion-pagina_${pageId}`;

        // Botón para editar la página
        const btnEditPage = $(".btnEditPage", filaPagina);
        // Botón para editar el filtroOrden
        const btnEditFilter = $(".btnEditFilter", filaFiltroOrden);

        // Abrir el modal y el la fila del filtro que tiene errores para ser editados
        btnEditPage.trigger( "click" );       
       
        // Mostrar el mensaje del error en el input correspondiente
        if (input != undefined){
            comprobarInputNoVacio(input, true, false, "El nombre y el filtro no pueden estar vacios.", 0);
        }          

        setTimeout(function(){ 
            // Realizar scroll hasta el elemento indicado
            scrollInModalView(btnEditFilter, modalId, function(){                
                btnEditFilter.trigger( "click" );
                mostrarNotificacion("error", "El nombre y el filtro no pueden estar vacios.");
            });            
        },1000);  
  
    },

    /**
     * Método para mostrar el error de que una página (de tipo búsqueda semántica) el campo filtro está vacío.
     * @param {String} pageId : Id de la página que está siendo tratada
     * @param {jqueryObject} input : Input donde que ha producido el error.
     */
     mostrarErrorCampoFiltroVacio: function (pageId, input){

        // Obtener el identificador de la página con error
        const filaPagina = $(`.page-row#${pageId}`);
        // Construcción del identificador del modal
        const modalId = `modal-configuracion-pagina_${pageId}`;

        // Botón para editar la página
        const btnEditPage = $(".btnEditPage", filaPagina);

        // Abrir el modal y el la fila del filtro que tiene errores para ser editados
        btnEditPage.trigger( "click" );  

        // Mostrar el mensaje del error en el input correspondiente
        if (input != undefined){            
            comprobarInputNoVacio(input, true, false, "El Campo Filtro de la página no puede estar vacío", 0);
        }          

        setTimeout(function(){ 
            // Realizar scroll hasta el elemento indicado
            scrollInModalView(input, modalId, function(){                                
                mostrarNotificacion("error", "El Campo Filtro de la página no puede estar vacío.");
            });            
        },1000);  

    },    

    /**
     * Método para comprobar las exportaciones existentes en las páginas
     * @returns bool: Devuelve valor que indica si se puede proceder al guardado de páginas
     */
    comprobarExportaciones: function(){
        const that = this;

        // Exportaciones de todas las páginas a revisar que hayan sido modificadas y no borradas (Más velocidad)
        const exportaciones = $('.page-row:not(".deleted").modified .exportation-row:not(".deleted")');
        
        exportaciones.each(function () {
            if (that.errorExportaciones == false){
                that.comprobarExportacion($(this))
            }
        });
        return that.errorExportaciones;
    },


    /**
    * Método para comprobar las exportaciones creadas de las páginas
    * @param {jqueryObject} filaExportacion : Objeto jquery que corresponde con la fila donde se encuentra el la exportación a revisar
    * @returns : bool: Devuelve un valor que indica si se puede proceder al guardado de las páginas.
    */
    comprobarExportacion: function(filaExportacion){
        const that = this
        
        // Panel donde se podrá localizar el nombre de la exportación a revisar
        const panelExportationInfo = filaExportacion.find('.exportation-info');
        // Filas de páginas
        const filasPaginas = $("#id-added-pages-list").find(".component-wrap.page-row");
        // Fila de la página cuyos filtros de orden están siendo revisados
        const filaPagina = panelExportationInfo.closest(filasPaginas);

        // Input del nombre de la exportación
        const inputExportationName = panelExportationInfo.find('[name="Nombre"]').first();                

        // Comprobar que el Nombre de la exportación NO está vacío
        if (inputExportationName.val().trim() == "") {
            that.mostrarErrorNombreExportacionVacio(filaPagina, filaExportacion, inputExportationName);
            that.errorExportaciones = true;
            return true;
        }
        
        // Comprobación de que hay al menos 1 propiedad asociada a la fila exportación que se está revisando
        const propiedades = filaExportacion.find(".exportation-info").find('.property-row:not(".deleted")');
                
        if (propiedades.length == 0) {
            that.mostrarErrorExportacionSinPropiedades(filaPagina, filaExportacion);
            that.errorExportaciones = true;
            return true;
        }

        // Comprobación de que si hay propiedades, estás contiene datos válidos
        propiedades.each(function () {
            if (that.comprobarPropiedadExportacion($(this))) {
                that.errorExportaciones = true;
                return;
            }
        });
        return that.errorExportaciones;
    },

    /**
     * Método para mostrar el error cuando el nombre de una exportación está vacío. La comprobación se hace al pulsar en "Guardar" las páginas
     * @param {jqueryObject} filaPagina : Fila de la página que está siendo revisada
     * @param {jqueryObject} filaExportacion : Fila de la exportación que ha dado el error
     * @param {*} inputExportationName : Input que se ha encontrado con valor vacío
     */
    mostrarErrorNombreExportacionVacio: function(filaPagina, filaExportacion, inputExportationName){
        // Filas de páginas
        const filasPaginas = $("#id-added-pages-list").find(".component-wrap.page-row");

        // Obtener el identificador de la página con error
        const pageId = filaPagina.attr("id");
        // Construcción del identificador del modal
        const modalId = `modal-configuracion-pagina_${pageId}`

        // Botón para editar la página
        const btnEditPage = $(".btnEditPage", filaPagina);
        // Botón para editar la exportación
        const btnEditExportation = $(".btnEditExportation", filaExportacion);  
        
        // Abrir el modal y los paneles donde se encuentra el error
        btnEditPage.trigger( "click" );
        btnEditExportation.trigger("click");        

        // Mostrar el mensaje del error en el input correspondiente
        if (inputExportationName != undefined){
            comprobarInputNoVacio(inputExportationName, true, false, "Los nombres de las exportaciones no pueden estar vacíos.", 0);
        }

        setTimeout(function(){ 
           
            // Realizar scroll hasta el elemento indicado
            scrollInModalView(btnEditExportation, modalId, function(){                
                mostrarNotificacion("error", "Los nombres de las exportaciones no pueden estar vacíos.");
            });            
        },1000);  
    },

    /**
     * Método para mostrar el error cuando una exportació no tiene asigandas propiedades
     * @param {jqueryObject} filaPagina : Fila de la página que está siendo revisada
     * @param {jqueryObject} filaExportacion : Fila de la exportación que ha dado el error
     */
    mostrarErrorExportacionSinPropiedades: function(filaPagina, filaExportacion){

        // Filas de páginas
        const filasPaginas = $("#id-added-pages-list").find(".component-wrap.page-row");

        // Obtener el identificador de la página con error
        const pageId = filaPagina.attr("id");
        // Construcción del identificador del modal
        const modalId = `modal-configuracion-pagina_${pageId}`

        // Botón para editar la página
        const btnEditPage = $(".btnEditPage", filaPagina);
        // Botón para editar la exportación
        const btnEditExportation = $(".btnEditExportation", filaExportacion);  
        
        // Abrir el modal y los paneles donde se encuentra el error
        btnEditPage.trigger( "click" );
        btnEditExportation.trigger("click");        

        setTimeout(function(){ 
            // Realizar scroll hasta el elemento indicado
            scrollInModalView(btnEditExportation, modalId, function(){                
                mostrarNotificacion("error", "Las exportaciones deben tener por lo menos una propiedad.");
            });            
        },1000);  

    },

    /**
     * Método para comprobar que la propiedad asignada a una exportación es válida (Dispone de Nombre y de propiedad)
     * @param {jqueryObject} filaPropiedad : Fila de la propiedad que va a ser revisada
     */
    comprobarPropiedadExportacion: function(filaPropiedad){
        const that = this
        // Fila de exportación que está siendo revisada donde se contiene toda la información
        const panelExportationInfo = filaPropiedad.find('.property-info');

        // Inputs de información de la exportación        
        // Input del nombre de la exportación
        const inputPropertyName = panelExportationInfo.find('[name="Nombre"]');
        // Input con el nombre de la propiedad asociada a dicha propiedad
        const inputPropertyPropertyName = panelExportationInfo.find('[name="Propiedad"]');
        
        if (inputPropertyName.val().trim() == "" || inputPropertyPropertyName.val().trim() == "") {
            that.mostrarErrorCampoPropiedadVacio(filaPropiedad, inputPropertyPropertyName);
            return true;
        }

        return false;        
    },
    
    /**
     * Método para mostrar el error de que el campo de una propiedad no puede estar vacío.
     * @param {*} filaPropiedad 
     * @param {*} input 
     */
    mostrarErrorCampoPropiedadVacio: function(filaPropiedad, input){
        // Filas de páginas
        const filasPaginas = $("#id-added-pages-list").find(".component-wrap.page-row");
        // Fila de la página cuyas propiedades están con el error
        const filaPagina = filaPropiedad.closest(filasPaginas);
        const filaExportation = filaPropiedad.closest(".exportation-row");
        // Obtener el identificador de la página con error
        const pageId = filaPagina.attr("id");
        // Construcción del identificador del modal
        const modalId = `modal-configuracion-pagina_${pageId}`
        
        // Botón para editar la página
        const btnEditPage = $(".btnEditPage", filaPagina);
        // Botón para editar la exportación
        const btnEditExportation = $(".btnEditExportation", filaExportation);        
        // Botón para editar la propiedad de la exportación
        const btnEditProperty = $(".btnEditProperty", filaPropiedad);
        
        // Abrir el modal y los paneles donde se encuentra el error
        btnEditPage.trigger( "click" );
        btnEditExportation.trigger("click");
        btnEditProperty.trigger( "click" );

        if (input != undefined){
            comprobarInputNoVacio(input, true, false, "Los campos de las propiedades no pueden estar vacios.", 0);
        }

        setTimeout(function(){ 
            // Realizar scroll hasta el elemento indicado
            scrollInModalView(btnEditExportation, modalId, function(){                
                mostrarNotificacion("error", "Los campos de las propiedades no pueden estar vacios.");
            });            
        },1000);         
    },

    /**
     * Método para comprobar que no hay Metadescripciones vacíos en las páginas creadas/editadas     
     */
    comprobarMetadescripciones: function(){
        const that = this;        
        // Inputs con las metadescripciones de todas las páginas
        // const inputsMetadescripcion = $('.page-row:not(".deleted") .inputsMultiIdioma.basicInfo input[name="TabMetaDescription"]:not(":disabled")');
        // Textareas con las metadescripciones 
        const inputsMetadescripcion = $('.page-row:not(".deleted") .inputsMultiIdioma.basicInfo [name="TabMetaDescription"]:not(":disabled")');
        inputsMetadescripcion.each(function () {
            that.comprobarMetadescripcion($(this));            
        });        
    },

    /**
     * Método para validar la metadescripción de una página. Se aplicará la metadescripción del idioma por defecto para todos los idiomas si no hay una establecida.
     * @param {*} inputMetadescription: Input a comprobar
     * @returns bool: Devuelve un valor booleano indicando si todo está OK para proceder con el guardado de las páginas.
     */
    comprobarMetadescripcion: function(inputMetadescription){  
        const that = this

        // Contiene los tabs en idiomas y el div con los inputs en idiomas completo
        const formularioEdicion = inputMetadescription.closest(".formulario-edicion");
        const panInputsMultiIdioma = inputMetadescription.closest(".inputsMultiIdioma");
        // Contenedor donde se encuentran datos básicos de la página (Nombre, Url/Ruta, MetaDescripción )
        const panMultiIdioma = formularioEdicion.find(".panContenidoMultiIdioma.basicInfo");

        // Id del input a tratar
        const inputId = inputMetadescription.attr("id");

        const listaTextos = [];
        
        if (operativaMultiIdioma.listaIdiomas.length > 1 && panMultiIdioma.length > 0) {
            let textoMultiIdioma = "";
            // Seleccionar el texto del idioma por defecto para la metadescripción
            let textoIdiomaDefecto = panMultiIdioma.find(`#input_${inputId}_${operativaMultiIdioma.idiomaPorDefecto}`).val();
            
            // Recorrer todos los idiomas para asignarlos al input correspondiente. Si algún idioma no tiene metadescripción, asignarle la del idioma por defecto
            $.each(operativaMultiIdioma.listaIdiomas, function () {
                // Obtención del Key del idioma
                const idioma = this.key;
                // Asignar el valor por defecto de la ruta al idioma si este no dispone de valor para Nombre
                let textoIdioma = $(`#input_${inputId}_${idioma}`).val();
                if (textoIdioma == null || textoIdioma == "") {
                    textoIdioma = textoIdiomaDefecto;
                    $(`#input_${inputId}_${idioma}`).val(textoIdioma);
                }
                // Escribir el nombre del multiIdioma en el campo Hidden
                textoMultiIdioma += textoIdioma + "@" + idioma + "|||";
                
                listaTextos.push({ "key": idioma, "value": textoIdioma });
            });            
            inputMetadescription.val(textoMultiIdioma);
        }else{
            // Sin multiIdioma.
            let textoIdiomaDefecto = panMultiIdioma.find(`#input_${inputId}_${operativaMultiIdioma.idiomaPorDefecto}`).val();
            // Establecer el nombre en el input correspondiente
            inputMetadescription.val(textoIdiomaDefecto);
        }
        return false;
    },
    

    /**
     * 
     */

    /**
     * Método para obtener los datos de cada página para proceder a su guardado. Se ejecuta desde 'handleSavePages'
     * @param {*} fila : Fila de la que hay que recoger los datos
     * @param {*} num : Nº de la página que se está analizando     
     */
    obtenerDatosPestanya: function(fila, num){        
        const that = this;
        // Id de la página
        const id = fila.attr('id');
        // Contenido o datos de la página dentro del modal
        // const panelEdicion = fila.find('.modal-page').last();
        const panelEdicion = fila.find(`#modal-configuracion-pagina_${id}`);
        // Prefijo para guardado de la pestaña/página
        const prefijoClave = 'ListaPestanyas[' + num + ']';

        // Recogida de datos principales de la página 
        that.ListaPestanyas[prefijoClave + '.Key'] = fila.attr('id');
        that.ListaPestanyas[prefijoClave + '.Url'] = panelEdicion.find('[name="TabUrl"]').val();
        that.ListaPestanyas[prefijoClave + '.EsUrlPorDefecto'] = panelEdicion.find('[name="TabUrl"]').is(':disabled');
        that.ListaPestanyas[prefijoClave + '.ShortName'] = panelEdicion.find('[name="TabShortName"]').val();
        that.ListaPestanyas[prefijoClave + '.Type'] = panelEdicion.find('[name="TabType"]').val();
        that.ListaPestanyas[prefijoClave + '.Deleted'] = panelEdicion.find('[name="TabEliminada"]').val();
        that.ListaPestanyas[prefijoClave + '.ParentTabKey'] = panelEdicion.find('[name="ParentTabKey"]').val();
        that.ListaPestanyas[prefijoClave + '.Order'] = num; //panelEdicion.find('[name="TabOrden"]').val();
        that.ListaPestanyas[prefijoClave + '.FechaModificacion'] = panelEdicion.find('[name="TabFechaModificacion"]').val();

        // Indicador de si la página ha sido editada / recién creada
        const modified = fila.hasClass("modified");
        that.ListaPestanyas[prefijoClave + '.Modified'] = modified;

        // Recoger los datos de la página que haya podido sufrir cambios
        if (modified)
        {
            // Recoger datos básicos de la página
            that.ListaPestanyas[prefijoClave + '.Name'] = panelEdicion.find('[name="TabName"]').val();
            that.ListaPestanyas[prefijoClave + '.EsNombrePorDefecto'] = panelEdicion.find('[name="TabName"]').is(':disabled');
            that.ListaPestanyas[prefijoClave + '.OpenInNewWindow'] = panelEdicion.find(`#TabOpenInNewWindow_SI_${fila.attr('id')}`).is(':checked'); // panelEdicion.find('[name="TabOpenInNewWindow"]').is(':checked');
            that.ListaPestanyas[prefijoClave + '.ClassCSSBody'] = panelEdicion.find('[name="TabClassCSSBody"]').val();
            that.ListaPestanyas[prefijoClave + '.MetaDescription'] = panelEdicion.find('[name="TabMetaDescription"]').val();
            
            that.ListaPestanyas[prefijoClave + '.Active'] = panelEdicion.find(`#TabActive_SI_${fila.attr('id')}`).is(':checked'); // panelEdicion.find('[name="TabActive"]').is(':checked');
            that.ListaPestanyas[prefijoClave + '.Visible'] = panelEdicion.find(`#TabVisible_SI_${fila.attr('id')}`).is(':checked'); // panelEdicion.find('[name="TabVisible"]').is(':checked');

            // Comprobación del tipo de Privacidad
            that.ListaPestanyas[prefijoClave + '.Privacidad'] = panelEdicion.find('[name="TabPrivacidad"]').val();

            // Comprobación de detalles de Privacidad (¿Existe Panel de privacidad?)
            let visibleSinAcceso = undefined;
            if (panelEdicion.find('.edit-privacy').length > 0){
                // Input 'Html alternativo'
                that.ListaPestanyas[prefijoClave + '.HtmlAlternativoPrivacidad'] = encodeURIComponent(panelEdicion.find('[name="TabHtmlAlternativoPrivacidad"]').val().replace(/\n/g, ''));
                const tabVisibleSinAccesoFila = `TabVisibleSinAcceso_${fila.attr('id')}`;
                // Input 'Visible en el menú para usuarios sin acceso'
                visibleSinAcceso = $("input[name="+tabVisibleSinAccesoFila+"]:checked").data("value") == "si" ? true : false;                                        

                // Comprobación de Perfiles/Grupos para la Privacidad de la página
                if (panelEdicion.find('[name="TabPrivacidad"]').val() == '2') {

                    // Añadir perfiles de usuarios privacidad de la página
                    const privacidadPerfiles = panelEdicion.find('[name="TabValoresPrivacidadPerfiles"]').val().split(',');
                    for (let i = 0; i < privacidadPerfiles.length; i++) {
                        if (privacidadPerfiles[i] != "") {
                            const prefijoPrivacidadPerfiles = prefijoClave + '.PrivacidadPerfiles[' + i + ']';
                            that.ListaPestanyas[prefijoPrivacidadPerfiles + '.Key'] = privacidadPerfiles[i];
                            that.ListaPestanyas[prefijoPrivacidadPerfiles + '.Value'] = "";
                        }
                    }

                    // Añadir perfiles de grupos de usuarios para la privacidad de la página
                    const privacidadGrupos = panelEdicion.find('[name="TabValoresPrivacidadGrupos"]').val().split(',');
                    for (let i = 0; i < privacidadGrupos.length; i++) {
                        if (privacidadGrupos[i].trim() != "") {
                            var prefijoPrivacidadGrupos = prefijoClave + '.PrivacidadGrupos[' + i + ']';
                            that.ListaPestanyas[prefijoPrivacidadGrupos + '.Key'] = privacidadGrupos[i].substr(2);
                            that.ListaPestanyas[prefijoPrivacidadGrupos + '.Value'] = "";
                        }
                    }
                }
            }

            // Establecer valor de 'Visible en el menú para usuarios sin acceso'
            that.ListaPestanyas[prefijoClave + '.VisibleSinAcceso'] = visibleSinAcceso;

            // Input TabHomeCMS para "Editar la Home con el CMS" 
            const tabHomeCMSName = `TabHomeCMS_${fila.attr('id')}`; 
            const tabHomeCheckBox = $("input[name="+tabHomeCMSName+"]");
            
            if (tabHomeCheckBox.length > 0){
                const homeCMS = $("input[name="+tabHomeCMSName+"]:checked").data("value") == "si" ? true : false;

                let homeTodosUsuarios = false,
                    homeMiembros = false,
                    homeNoMiembros = false;
                // Controlar si la página se desea para todos los usuarios
                if (homeCMS == true) {
                    const tipoHomeCMSName = `TabTypeHomeCMS_${fila.attr('id')}`;

                    const tipoHomeCMS = panelEdicion.find("[name="+ tipoHomeCMSName +"]:checked").data("value");
                    if (tipoHomeCMS == 'Unica') {
                        homeTodosUsuarios = true;
                    }
                    else {                                            
                        homeMiembros = panelEdicion.find(`[name="${that.checkboxTabHomeMiembrosCMSClassName}"]`).is(':checked');
                        homeNoMiembros = panelEdicion.find(`[name="${that.checkboxTabHomeNoMiembrosCMSClassName}"]`).is(':checked');
                    }
                }

                // Establecer valores para editar Home con CMS
                const prefijoHomeCMSClave = prefijoClave + '.HomeCMS';
                that.ListaPestanyas[prefijoHomeCMSClave + '.HomeTodosUsuarios'] = homeTodosUsuarios;
                that.ListaPestanyas[prefijoHomeCMSClave + '.HomeMiembros'] = homeMiembros;
                that.ListaPestanyas[prefijoHomeCMSClave + '.HomeNoMiembros'] = homeNoMiembros;
            }

            // Panel para editar opciones de búsqueda de la página (si dispone de la opción)
            const panelEditarBusqueda = panelEdicion.find('.editarOpcionesBusqueda');
            if (panelEditarBusqueda.length > 0) {
                // Prefijo para guardado en BD
                const prefijoBusquedaClave = prefijoClave + '.OpcionesBusqueda';

                that.ListaPestanyas[prefijoBusquedaClave + '.ValoresPorDefecto'] = 'true';
                // Seleccionar el campo Filtro de búsqueda
                if (panelEditarBusqueda.find('.auxFiltrosSeleccionados').val().trim() == ""){
                    that.errorDuranteObtenerDatosPestaya = true;
                    // Mostrar el error
                    const inputTabCampoFiltroError = panelEditarBusqueda.find('.auxFiltrosSeleccionados');
                    that.mostrarErrorCampoFiltroVacio(id, inputTabCampoFiltroError);
                    return;
                }
                
                that.ListaPestanyas[prefijoBusquedaClave + '.CampoFiltro'] = panelEditarBusqueda.find('.auxFiltrosSeleccionados').val();

                // Panel que contiene el orden de los filtros
                const panelFiltrosOrden = panelEdicion.find('.id-added-filter-list');
                // Comprobar si hay filtros de orden
                if (panelFiltrosOrden.length > 0) {
                    // Prefijo para guardado en BD
                    const prefijoFiltrosOrden = prefijoBusquedaClave + '.FiltrosOrden';
                    // Nº del filtro
                    let numFiltro = 0;                    
                    // Recorrer cada filtro para la obtención de sus datos
                    $('.filter-row', panelFiltrosOrden).each(function () {
                        // Panel de detalles del filtro
                        const panFiltro = $(this).find(".filter-order-info"); // $(this).children('.panEdicion');
                        // Prefijo para guardado en BD
                        const prefijoFiltroOrdenClave = prefijoFiltrosOrden + '[' + numFiltro + ']';
                        // Obtención de los datos del Filtro
                        that.ListaPestanyas[prefijoFiltroOrdenClave + '.Orden'] = numFiltro;
                        that.ListaPestanyas[prefijoFiltroOrdenClave + '.Deleted'] = panFiltro.find('[name="TabEliminada"]').val();
                        that.ListaPestanyas[prefijoFiltroOrdenClave + '.Nombre'] = panFiltro.find('[name="TabName"]').val();
                        that.ListaPestanyas[prefijoFiltroOrdenClave + '.Filtro'] = panFiltro.find('[name="Filtro"]').val();

                        numFiltro++;
                    });                    

                }

                // Establecer valores para búsquedas y Filtros de página
                that.ListaPestanyas[prefijoBusquedaClave + '.NumeroResultados'] = panelEditarBusqueda.find('[name="TabNumeroResultados"]').val();                                        
                // Input 'Mostrar facetas"
                const tabMostrarFacetasName = `TabMostrarFacetas_${fila.attr('id')}`;                
                that.ListaPestanyas[prefijoBusquedaClave + '.MostrarFacetas'] = $("input[name="+tabMostrarFacetasName+"]:checked").data("value") == "si" ? true : false; 
                // Input 'Agrupar facetas por tipo'
                const tabMostrarFacetasPorTipoName = `TabAgruparFacetasPorTipo_${fila.attr('id')}`;                
                that.ListaPestanyas[prefijoBusquedaClave + '.AgruparFacetasPorTipo'] = $("input[name="+tabMostrarFacetasPorTipoName+"]:checked").data("value") == "si" ? true : false; 
                // Input 'Mostrar página en el buscador de la cabecera' - Deprecado CORE-4941 - De momento lo pongo como true para el CICD
                const tabMostrarEnBusquedaCabeceraName = `TabMostrarEnBusquedaCabecera_${fila.attr('id')}`;                
                //that.ListaPestanyas[prefijoBusquedaClave + '.MostrarEnBusquedaCabecera'] = $("input[name="+tabMostrarEnBusquedaCabeceraName+"]:checked").data("value") == "si" ? true : false;                
                that.ListaPestanyas[prefijoBusquedaClave + '.MostrarEnBusquedaCabecera'] = true;                
                // Input 'Mostrar caja de busqueda' - Deprecado CORE-4941 - De momento lo pongo como true para el CICD
                const tabMostrarCajaBusquedaName = `TabMostrarCajaBusqueda_${fila.attr('id')}`;                
                // that.ListaPestanyas[prefijoBusquedaClave + '.MostrarCajaBusqueda'] = $("input[name="+tabMostrarCajaBusquedaName+"]:checked").data("value") == "si" ? true : false;                
                that.ListaPestanyas[prefijoBusquedaClave + '.MostrarCajaBusqueda'] = true;                
                
                // Input 'Proyecto origen de búsqueda'
                that.ListaPestanyas[prefijoBusquedaClave + '.ProyectoOrigenBusqueda'] = panelEditarBusqueda.find('[name="TabProyectoOrigenBusqueda"]').val();
                // Input 'Ocultar resultados sin filtros'
                const tabOcultarResultadosSinFiltrosName = `TabOcultarResultadosSinFiltros_${fila.attr('id')}`;                
                that.ListaPestanyas[prefijoBusquedaClave + '.OcultarResultadosSinFiltros'] = $("input[name="+tabOcultarResultadosSinFiltrosName+"]:checked").data("value") == "si" ? true : false;
                // Input 'Texto de busqueda sin resultados'
                that.ListaPestanyas[prefijoBusquedaClave + '.TextoBusquedaSinResultados'] = panelEditarBusqueda.find('[name="TabTextoBusquedaSinResultados"]').val();
                // Input 'Ignorar la privacidad en búsqueda'
                const tabIgnorarPrivacidadEnBusquedaName = `TabIgnorarPrivacidadEnBusqueda_${fila.attr('id')}`;                
                that.ListaPestanyas[prefijoBusquedaClave + '.IgnorarPrivacidadEnBusqueda'] = $("input[name="+tabIgnorarPrivacidadEnBusquedaName+"]:checked").data("value") == "si" ? true : false;
                // Input 'Omitir la carga inicial de facetas y resultados'
                const tabOmitirCargaInicialFacetasResultadosName = `TabOmitirCargaInicialFacetasResultados_${fila.attr('id')}`;                
                that.ListaPestanyas[prefijoBusquedaClave + '.OmitirCargaInicialFacetasResultados'] = $("input[name="+tabOmitirCargaInicialFacetasResultadosName+"]:checked").data("value") == "si" ? true : false;
                // Input 'Filtro de RelacionMandatory'
                that.ListaPestanyas[prefijoBusquedaClave + '.RelacionMandatory'] = panelEditarBusqueda.find('[name="TabFiltroRelacionMandatory"]').val();
                // Input 'Filtro de TextoDefectoBuscador'                
                that.ListaPestanyas[prefijoBusquedaClave + '.TextoDefectoBuscador'] = panelEdicion.find('[name="TabFiltroTextoDefectoBuscador"]').length > 0 ? panelEdicion.find('[name="TabFiltroTextoDefectoBuscador"]').val() : "";
                // Opciones de "Vistas disponibles" de resultados (Listado, Mosaico, Mapa...)
                // Prefijo para guardado en BD
                const prefijoBusquedaOpcionesVistasClave = prefijoBusquedaClave + '.OpcionesVistas';

                // Input de cual será la vista por defecto para visualizar los resultados
                that.ListaPestanyas[prefijoBusquedaOpcionesVistasClave + '.VistaPorDefecto'] = panelEditarBusqueda.find('[name="' + id + '_VistaPorDefecto"]:checked').val();
                // Input 'Vista Listado'
                that.ListaPestanyas[prefijoBusquedaOpcionesVistasClave + '.VistaListado'] = panelEditarBusqueda.find('[name="TabVistaListado"]').is(':checked');
                // Input 'Vista Mosaico'
                that.ListaPestanyas[prefijoBusquedaOpcionesVistasClave + '.VistaMosaico'] = panelEditarBusqueda.find('[name="TabVistaMosaico"]').is(':checked');
                // Input 'Vista Mapa'
                that.ListaPestanyas[prefijoBusquedaOpcionesVistasClave + '.VistaMapa'] = panelEditarBusqueda.find('[name="TabVistaMapa"]').is(':checked');
                // Input 'Vista Gráfico'
                that.ListaPestanyas[prefijoBusquedaOpcionesVistasClave + '.VistaGrafico'] = panelEditarBusqueda.find('[name="TabVistaGrafico"]').is(':checked');
                // Input datos de la vista "Mapa"
                that.ListaPestanyas[prefijoBusquedaOpcionesVistasClave + '.PosicionCentralMapa'] = panelEditarBusqueda.find('[name="TabPosicionCentralMapa"]').val();
                that.ListaPestanyas[prefijoBusquedaOpcionesVistasClave + '.PropiedadLatitud'] = ''; // No existe -> Manda undefined panelEditarBusqueda.find('[name="TabPropiedadLatitud"]').val();
                that.ListaPestanyas[prefijoBusquedaOpcionesVistasClave + '.PropiedadLongitud'] = ''; // No existe -> Manda undefined panelEditarBusqueda.find('[name="TabPropiedadLongitud"]').val();
            }  

            //TFG FRAN
            var panelOpcionesDashboard = panelEdicion.find('.editarOpcionesDashboard');
            if (panelOpcionesDashboard.length > 0) {
                var prefijoDashboardClave = prefijoClave + '.OpcionesDashboard';

                var panelAsistentes = panelEdicion.find('.editarAsistentes');
                if (panelAsistentes.length > 0) {
                    //Obtenemos los id de los ejemplos de los gráficos
                    var divGraficos = panelEdicion.find('.ejemplosGraficos');
                    var graficos = divGraficos.find('.generados');
                    var idGraficos = [];
                    var numAsistente = 0;
                    for (var i = 0; i < graficos.length; i++) {
                        idGraficos.push(graficos[i].id);
                    }

                    //Recorremos los asistentes que hayan sido añadidos
                    $('.asistente-row.asisAnyadido', panelAsistentes).each(function () {
                        var panAsistente = $(this).children('.panEdicion');
                        numAsistente = idGraficos.indexOf($(this).attr('id') + 'GrafPreview');
                        var prefijoAsistenteClave = prefijoDashboardClave + '[' + numAsistente + ']';

                        that.ListaPestanyas[prefijoAsistenteClave + '.AsisID'] = panAsistente.closest('.asistente-row').attr('id');
                        that.ListaPestanyas[prefijoAsistenteClave + '.Orden'] = numAsistente;
                        that.ListaPestanyas[prefijoAsistenteClave + '.Nombre'] = panAsistente.find('[name="nGrafico"]').val();
                        that.ListaPestanyas[prefijoAsistenteClave + '.Select'] = panAsistente.find('[name="selectGrafico"]').val();
                        var where = "";
                        where = panAsistente.find('[name="whereGrafico"]')[0].value;
                        where = where + "|||" + panAsistente.find('[name="groupbyGrafico"]').val();
                        where = where + "|||" + panAsistente.find('[name="orderbyGrafico"]').val();
                        where = where + "|||" + panAsistente.find('[name="limitGrafico"]').val();
                        that.ListaPestanyas[prefijoAsistenteClave + '.Where'] = where;
                        that.ListaPestanyas[prefijoAsistenteClave + '.Titulo'] = panAsistente.find('[name="mtGrafico"]').prop('checked');
                        that.ListaPestanyas[prefijoAsistenteClave + '.Tamano'] = panAsistente.find('[name="tamGrafico"]').val();

                        var tipo = panAsistente.find('[name="tGrafico"]').val();
                        that.ListaPestanyas[prefijoAsistenteClave + '.Tipo'] = tipo;
                        var prefijoDatasets = prefijoAsistenteClave + '.OpcionesDatasets';
                        if (tipo == '1') {
                            var divBarras = panAsistente.find('.opcionesBarras');
                            that.ListaPestanyas[prefijoAsistenteClave + '.PropExtra'] = divBarras.find('[name="horizontal"]').prop('checked');
                            that.ListaPestanyas[prefijoAsistenteClave + '.Labels'] = panAsistente.find('[name="labelsGrafico"]').val();
                            var panelDatasets = divBarras.find('.datasets');
                            var numDataset = 0;
                            //Recorremos datasets
                            $('.dataset', panelDatasets).each(function () {
                                var prefijoDatasetClave = prefijoDatasets + '[' + numDataset + ']';
                                that.ListaPestanyas[prefijoDatasetClave + '.DatasetID'] = $(this).attr('id');
                                that.ListaPestanyas[prefijoDatasetClave + '.Datos'] = $(this).find('[name="datosDataset"]').val();
                                that.ListaPestanyas[prefijoDatasetClave + '.Nombre'] = $(this).find('[name="nombreDataset"]').val();
                                that.ListaPestanyas[prefijoDatasetClave + '.Color'] = $(this).find('[name="colorDataset"]').val();
                                that.ListaPestanyas[prefijoDatasetClave + '.Orden'] = numDataset;
                                numDataset++;
                            });
                        }
                        if (tipo == '2') {
                            var divLineas = panAsistente.find('.opcionesLineas');
                            that.ListaPestanyas[prefijoAsistenteClave + '.PropExtra'] = divLineas.find('[name="area"]').prop('checked');
                            that.ListaPestanyas[prefijoAsistenteClave + '.Labels'] = panAsistente.find('[name="labelsGrafico"]').val();
                            var panelDatasets = divLineas.find('.datasets');
                            var numDataset = 0;
                            //Recorremos datasets
                            $('.dataset', panelDatasets).each(function () {
                                var prefijoDatasetClave = prefijoDatasets + '[' + numDataset + ']';
                                that.ListaPestanyas[prefijoDatasetClave + '.DatasetID'] = $(this).attr('id');
                                that.ListaPestanyas[prefijoDatasetClave + '.Datos'] = $(this).find('[name="datosDataset"]').val();
                                that.ListaPestanyas[prefijoDatasetClave + '.Nombre'] = $(this).find('[name="nombreDataset"]').val();
                                that.ListaPestanyas[prefijoDatasetClave + '.Color'] = $(this).find('[name="colorDataset"]').val();
                                that.ListaPestanyas[prefijoDatasetClave + '.Orden'] = numDataset;
                                numDataset++;
                            });
                        }
                        if (tipo == '3') {
                            var divCirculos = panAsistente.find('.opcionesCirculos');
                            that.ListaPestanyas[prefijoAsistenteClave + '.Labels'] = panAsistente.find('[name="labelsGrafico"]').val();
                            var prefijoDatasetClave = prefijoDatasets + '[0]';
                            that.ListaPestanyas[prefijoDatasetClave + '.DatasetID'] = divCirculos.find('.dataset').attr('id');
                            that.ListaPestanyas[prefijoDatasetClave + '.Datos'] = divCirculos.find('[name="datosDataset"]').val();
                            that.ListaPestanyas[prefijoDatasetClave + '.Nombre'] = 'Dataset';
                            that.ListaPestanyas[prefijoDatasetClave + '.Color'] = 'Aleatorio';
                            that.ListaPestanyas[prefijoDatasetClave + '.Orden'] = 0;
                        }
                        if (tipo == '4') {
                            var divTabla = panAsistente.find('.opcionesTabla');
                            that.ListaPestanyas[prefijoAsistenteClave + '.Labels'] = '';
                            var panelDatasets = divTabla.find('.datasets');
                            var numDataset = 0;
                            //Recorremos datasets
                            $('.dataset', panelDatasets).each(function () {
                                var prefijoDatasetClave = prefijoDatasets + '[' + numDataset + ']';
                                that.ListaPestanyas[prefijoDatasetClave + '.DatasetID'] = $(this).attr('id');
                                that.ListaPestanyas[prefijoDatasetClave + '.Datos'] = $(this).find('[name="datosDataset"]').val();
                                that.ListaPestanyas[prefijoDatasetClave + '.Nombre'] = $(this).find('[name="nombreDataset"]').val();
                                that.ListaPestanyas[prefijoDatasetClave + '.Color'] = 'Aleatorio';
                                that.ListaPestanyas[prefijoDatasetClave + '.Orden'] = numDataset;
                                numDataset++;
                            });
                        }
                        if (tipo == '5') {
                            var divHeat = panAsistente.find('.opcionesHeatMap');
                            that.ListaPestanyas[prefijoAsistenteClave + '.Labels'] = '';
                            var panelDatasets = divHeat.find('.datasets');
                            var numDataset = 0;
                            //Recorremos datasets
                            $('.dataset', panelDatasets).each(function () {
                                var prefijoDatasetClave = prefijoDatasets + '[' + numDataset + ']';
                                that.ListaPestanyas[prefijoDatasetClave + '.DatasetID'] = $(this).attr('id');
                                that.ListaPestanyas[prefijoDatasetClave + '.Datos'] = $(this).find('[name="datosDataset"]').val();
                                that.ListaPestanyas[prefijoDatasetClave + '.Nombre'] = $(this).find('[name="nombreDataset"]').val();
                                if (numDataset < 2) {
                                    that.ListaPestanyas[prefijoDatasetClave + '.Color'] = 'Aleatorio';
                                } else {
                                    that.ListaPestanyas[prefijoDatasetClave + '.Color'] = $(this).find('[name="colorDataset"]').val();
                                }
                                that.ListaPestanyas[prefijoDatasetClave + '.Orden'] = numDataset;
                                numDataset++;
                            });
                        }
                    });
                }

            }

            
            // Comprobación de facetas de la página (Si hay facetas asociadas)
            const panelEdicionFacetas = panelEdicion.find('.id-added-facet-list');
            if (panelEdicionFacetas.length > 0) {
                // Nº de la faceta
                let numFaceta = 0;
                // Recorrer cada faceta para la obtención de sus datos
                $('.facet-row', panelEdicionFacetas).each(function () {
                    // Prefijo para guardado en BD
                    const prefijoFacetasClave = prefijoClave + '.ListaFacetas[' + numFaceta + ']';

                    // Panel de detalles de la faceta
                    const panFaceta = $(this).find(".facet-info");
                    // Select / ComboBox de facetas
                    const selectorFacetas = panFaceta.find('.cmbListaFacetas');
                    // Select / ComboBox de Objetos de conocimiento
                    const selectorOC = panFaceta.find('.selectObjetosConocimiento');                    

                    that.ListaPestanyas[prefijoFacetasClave + '.Faceta'] = selectorFacetas.children('option:selected').val();
                    that.ListaPestanyas[prefijoFacetasClave + '.ObjetoConocimiento'] = selectorOC.children('option:selected').text();
                    that.ListaPestanyas[prefijoFacetasClave + '.Deleted'] = panFaceta.find('[name="TabEliminada"]').val();
                    that.ListaPestanyas[prefijoFacetasClave + '.ClavePestanya'] = fila.attr('id');
                    //that.ListaPestanyas[prefijoFacetasClave + '.Key'] = selectorFacetas.children('option:selected').val();
                    //that.ListaPestanyas[prefijoFacetasClave + '.Value'] = selectorOC.children('option:selected').text();
                    numFaceta++;                        
                });                
            }

            // Comprobación de las exportaciones de la página (Si hay exportaciones asociadas)
            const panelEditarExportaciones = panelEdicion.find('.id-added-exportation-list');
            if (panelEditarExportaciones.length > 0){
                // Nº de la exportación
                let numExport = 0;
                // Recorrer cada exportación para la obtención de sus datos
                $('.exportation-row', panelEditarExportaciones).each(function () {
                    // Prefijo para guardado en BD
                    const prefijoExportacionesClave = prefijoClave + '.ListaExportaciones[' + numExport + ']';

                    // Panel de detalles de la exportacion
                    const panExportacion = $(this).find(".exportation-info");
                    
                    // Establecer datos de la exportación
                    that.ListaPestanyas[prefijoExportacionesClave + '.Key'] = $(this).attr('id');
                    that.ListaPestanyas[prefijoExportacionesClave + '.Orden'] = numExport;
                    that.ListaPestanyas[prefijoExportacionesClave + '.Deleted'] = panExportacion.find('[name="TabEliminada"]').val();
                    that.ListaPestanyas[prefijoExportacionesClave + '.Nombre'] = panExportacion.find('[name="Nombre"]').val(); 
                    
                    // Obtener los grupos de la exportación
                    const privacidadGruposExport = panExportacion.find('[name="TabValoresPrivacidadGrupos"]').val().split(',');
                    for (let i = 0; i < privacidadGruposExport.length; i++) {
                        if (privacidadGruposExport[i].trim() != "") {
                            const prefijoPrivacidadGrupos = prefijoExportacionesClave + '.GruposPermiso[' + i + ']';
                            that.ListaPestanyas[prefijoPrivacidadGrupos + '.Key'] = privacidadGruposExport[i].substr(2);
                            that.ListaPestanyas[prefijoPrivacidadGrupos + '.Value'] = "";
                        }
                    }  
                    
                    // Obtener las propiedades de la exportación
                    let numPropExport = 0;
                    // Recorrer cada propiedad de exportación para la obtención de sus datos
                    $('.property-row', panExportacion).each(function () {
                        // Panel de detalles de la propiedad
                        const panPropExportacion = $(this).find(".property-info");
                        // Prefijo para guardado en BD
                        const prefijoPropExportacionesClave = prefijoExportacionesClave + '.ListaPropiedades[' + numPropExport + ']';
                        
                        // Establecer datos de la exportación
                        that.ListaPestanyas[prefijoPropExportacionesClave + '.Orden'] = numPropExport;
                        that.ListaPestanyas[prefijoPropExportacionesClave + '.Deleted'] = panPropExportacion.find('[name="TabEliminada"]').val();
                        that.ListaPestanyas[prefijoPropExportacionesClave + '.Nombre'] = panPropExportacion.find('[name="Nombre"]').val();
                        that.ListaPestanyas[prefijoPropExportacionesClave + '.Ontologia'] = panPropExportacion.find('[name="Ontologia"]').val();
                        that.ListaPestanyas[prefijoPropExportacionesClave + '.Propiedad'] = panPropExportacion.find('[name="Propiedad"]').val();
                        that.ListaPestanyas[prefijoPropExportacionesClave + '.DatoExtraPropiedad'] = panPropExportacion.find('[name="DatoExtraPropiedad"]').val();

                        numPropExport++;
                    });
                    numExport++;  
                }); 
                              
            }
            
            // Devolvería false ya que no hay errores durante obtención de datos de las páginas
            return that.errorDuranteObtenerDatosPestaya;         
        }
    },

    /**
     * Método para proceder al guardado de las páginas en BD.
     * El guardado se realiza una vez se ha comprobado que no hay errores (comprobarErroresGuardado) y que se han obtenidos los datos de las páginas modificadas (obtenerDatosPestanya)
     * Se envían las páginas que se han modificado
     * @param {*} completion: Cuando finalice de procesar la tarea, conjunto de acciones que se desean realizar
     */
    savePages: function(completion){
        const that = this;

        // Realizar la petición para el guardado de páginas
        GnossPeticionAjax(
            that.urlSavePages,
            that.ListaPestanyas,
            true
        ).done(function (data) {
            // OK - Mostrar el mensaje de guardado correcto
            mostrarNotificacion("success","Los cambios se han guardado correctamente");                        
            completion != undefined && completion(false);             
        }).fail(function (data) {
            // KO - Mostrar el error del guardado de páginas realizado
            let error = data.split('|||');
            if (data != "Contacte con el administrador del Proyecto, no es posible atender la petición.") {
                that.mostrarErrorGuardado();
                if (error[0] == "RUTA REPETIDA") {
                    that.mostrarErrorUrlRepetida($('#' + error[1]));
                    completion != undefined && completion(true);
                }
                else if (error[0] == "NOMBRE VACIO") {
                    that.mostrarErrorNombreVacio($('#' + error[1]));
                    completion != undefined && completion(true);
                }
                else if (error[0] == "PROYECTO_ORIGEN_BUSQUEDA_PRIVADO") {
                    that.mostrarErrorPoyectoOrigenBusquedaPrivado($('#' + error[1]));
                    completion != undefined && completion(true);
                } else if (error[0] == "invitado") {
                    mostrarNotificacion("error", "La sesión de usuario ha caducado. Accede con tu usuario y credenciales para poder continuar.")
                    completion != undefined && completion(true);
                } else if (error[0] == "ERROR CONCURRENCIA") {
                    mostrarNotificacion("error", error[1])
                    completion != undefined && completion(true);
                }
            }
            else
            {
                that.mostrarErrorGuardadoFallo(data);
                completion != undefined && completion(true);
            }

        }).always(function () {
            // Ocultar el loading
            loadingOcultar();
        });
    },

    /**
     * Método para mostrar posibles errores durante el proceso del guardado de páginas. Estos errores son devueltos por el servidor
     * una vez el método "savePages" ha sido ejecutado
     */
    mostrarErrorGuardado: function () {
        let esPre = "False";
        let entornoBloqueado = "False";

            if (esPre == "True") {
                mostrarNotificacion("error", "No se permite guardar porque estás en el entorno de preproducción");                
            }
            else if (entornoBloqueado == "True") {                
                mostrarNotificacion("error", "El entorno actual esta bloqueado. Hay que desplegar la versión de preproducción antes de hacer cambios");
            }
            else {
                mostrarNotificacion("error", "Ha habido errores en el guardado de las pesta&#xF1;as, revisa los errores marcados");                
            }
    },  


    /**
     * Método para mostrar posibles errores durante el proceso del guardado de páginas. Estos errores son devueltos por el servidor
     * una vez el método "savePages" ha sido ejecutado
     * @param {jqueryObject} fila 
     */
    mostrarErrorPoyectoOrigenBusquedaPrivado: function (fila) {
        const that = this;
        const inputUrl = $('input[name = "TabName"]', fila).first();
        mostrarNotificacion("error", "El proyecto no puede ser privado");        
    },    
}