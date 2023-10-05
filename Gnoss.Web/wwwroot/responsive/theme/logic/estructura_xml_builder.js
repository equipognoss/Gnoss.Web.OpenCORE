/**
 ***************************************************************************************
 * Logica / Operativas de comportamiento JS para la sección de Estructura, en concreto, el XML Builder de páginas de la Comunidad del DevTools
 * *************************************************************************************
 */

const operativaGestionXMLBuilder = {

    /* Inicializar operativa */
    init: function (parametrosGenerales) {
        this.config(parametrosGenerales);
        this.configModales();
        this.configRutas();
        this.configEvents();
        this.configSortables();
    },

    /* Inicializar elementos de la vista */
    config: function (parametrosGenerales) {
        // Área donde estarán todas las filas de la página. Aquí es donde se añadirán nuevas
        this.rowList = $('#xmlrow-list');
        // Botón para añadir más filas a la página CMS
        this.btnAddRow = $("#btnAddRowXML");
        // Area donde están los diseños básicos
        this.basicDesigns = $("#disenos-basicos");
        // Area donde están los diseños de columna predefinidos
        this.columnDesigns = $("#default-rows");

        /* Botones de acción de fila */
        this.btnEditRowClassName = 'js-action-edit-row';
        this.btnDeleteRowClassName = 'js-action-delete';
        this.btnDragRowClassName = 'js-action-handle-row';

        //Tab de Entidades
        this.componentsEntityContent = $('#entity-tab-content')

        /* Clase del bloque de las entidades */
        this.entityBlock = 'entity-block';

        // Listado donde se alojarán los componentes listados (cada lista item)
        this.createdListEntitiesClassName = 'created-entities';
        this.createdListEntities = $(`.${this.createdListEntitiesClassName}`);

        // Rows pertenecientes a los paneles de "Componentes" y  "Diseños"
        this.componentListItemClassName = 'builder-item';

        //Filas nuevas
        this.rows = 'xmlcontent';

        //Botón de guardar
        this.btnSave = $('#saveXML');

        //Borrar propiedad
        this.btnRemoveProperty = 'borrarpropiedad';

        //Borrar atributo
        this.btnRemoveAttribute = 'deleteAttribute';

        //Borrar representante
        this.btnRemoveRepresentante = 'deleteRepresentante';

        //Borrar atrNombre
        this.btnRemoveAtrNombre = 'delete-atrnombre';

        //Borrar atrNombreLectura
        this.btnRemoveAtrNombreLectura = 'delete-atrnombrelectura';

        //Borrar propiedad
        this.btnEditProperty = 'editarpropiedad';

        //Elementos de collapse
        this.lateralCollapse = $('.lateral-collapse');
        this.entityCollapseClassName = 'js-view-row';

        //Combobox atributos
        this.comboboxAttributes = $('select#attribute-name-input');

        //Combobox representantes
        this.comboboxRepresentantes = $('select#representante-tipo-input');

        //Ayudas atributos
        this.helpAttributes = $('.textHelpAttributes');

        //Buscador lateral
        this.buscadorPropiedades = $('#buscador-propiedades');

        // Configuración de propiedades/entidades
        this.parametrosGenerales = 
        {
            configuracionGeneral: [],
            entidades: {},
        };
        this.parametrosGenerales = parametrosGenerales;        
    },

    /* Configuración de los modales de atributos, entidades y configuración general */
    configModales: function (){
        const that = this;

        //Modal atributos propiedades
        let modalPropiedades = $('#modal-editar-atributos-propiedad')[0];
        let formularioAtributos = $('.formulario-edicion');
        this.showForm(formularioAtributos);
        this.closeForm(formularioAtributos);

        //Añadir atributo
        let divTable = $('#row-attributes-table');
        let bodyTable = divTable.find('tbody');
        let btnAddAttribute = $('#add-attribute');
        btnAddAttribute.off().on('click', function(){
            let entityid = modalPropiedades.dataset.entityid;
            let propertyid = modalPropiedades.dataset.propertyid;

            let nameInput = $('#attribute-name-input');
            let valueInput = $('#attribute-value-input');

            let atributo = nameInput.val();
            let valoratributo = valueInput.val();

            if(atributo != "" && valoratributo != ""){
                let row = bodyTable.find('#' + atributo);
                //Si no existe la fila se añade
                if(row.length == 0)
                {
                    let newRow = 
                    `<tr>
                        <td class="propertyAttribute">${atributo}</td>
                        <td id="${atributo}">${valoratributo}</td>
                        <td class="delete deleteAttribute"><span class="material-icons">delete</span></td>
                    </tr>`;

                    bodyTable.append($(newRow));

                    //Comprobamos si hay ya datos de la entidad
                    if(!that.parametrosGenerales.entidades[entityid]){
                        that.parametrosGenerales.entidades[entityid] = {};
                        that.parametrosGenerales.entidades[entityid].propiedadesVisualizacion =  {};
                    }

                    //Y comprobamos si hay ya datos de la propiedad
                    if(!that.parametrosGenerales.entidades[entityid].propiedadesVisualizacion[propertyid]){
                        that.parametrosGenerales.entidades[entityid].propiedadesVisualizacion[propertyid] = {};
                    }
                    
                    that.parametrosGenerales.entidades[entityid].propiedadesVisualizacion[propertyid][atributo] = valoratributo;
                }
                else
                {
                    row.text(valoratributo);
                    that.parametrosGenerales.entidades[entityid].propiedadesVisualizacion[propertyid][atributo] = valoratributo;
                }
            }

            valueInput.val("");
        });

        //Combobox atributos
        that.comboboxAttributes.off().on('change', function(evt){
            //Ocultamos todos los elementos
            Array.from(that.helpAttributes).forEach( e => {
                let elemento = $(e);
                if(!elemento.hasClass("d-none")){
                    elemento.addClass("d-none");
                }
            });

            //Mostramos el que hemos elegido
            $(".textHelp" + evt.currentTarget.value).removeClass("d-none");

            if(evt.currentTarget.value == "Tipo"){
                let input = $('#attribute-value-input');

                let comboBoxTipo = 
                `
                <select id="attribute-value-input" class="form-control not-outline js-select2">
                    <option value="ul">Etiqueta ul</option>
                    <option value="SinParrafo">Sin párrafo</option>
                    <option value="Sustituir">Sustituir</option>
                    <option value="GrupoContenido">Grupo de contenido</option>
                </select>
                <small class="form-text text-muted textHelpTipo">
                    <span>Dando valor a este atributo, se puede variar la forma en la que se muestra el valor o los valores de la propiedad.</span>
                    <ul>
                        <li class="textHelpTipoValues textHelpul">"ul": las entidades valor de la propiedad aparecerán en etiquetas HTML li dentro de un ul.</li>
                        <li class="textHelpTipoValues textHelpSinParrafo d-none">"SinParrafo": Los elementos se muestran directamente sin el elemento p.</li>
                        <li class="textHelpTipoValues textHelpSustituir d-none">"Sustituir": Solo se puede colocar en propiedades que sean selección de entidades. Así, se mostrarán como si fuesen valores de la propiedad.</li>
                        <li class="textHelpTipoValues textHelpGrupoContenido d-none">"GrupoContenido": Solo se puede colocar en propiedades que sean selección de entidades. Los datos se muestran dentro de una estructura de divs con clases "group" y "contentgroup".</li>
                    </ul>
                </small>
                `;

                input.replaceWith(comboBoxTipo);

                //Y le aplicamos el mismo comportamiento que para el comboBox de atributos
                input.off().on('change', function(evt){
                    let helpTipoValues = $('.textHelpTipoValues');

                    //Ocultamos todos los elementos
                    Array.from(helpTipoValues).forEach( e => {
                    let elemento = $(e);
                        if(!elemento.hasClass("d-none")){
                            elemento.addClass("d-none");
                        }
                    });

                    //Mostramos el que hemos elegido
                    $(".textHelp" + evt.currentTarget.value).removeClass("d-none");
                });

                //Aplicamos select2
                input = $('#attribute-value-input');
                let defaultOptions = {
                    minimumResultsForSearch: 10,
                    width: '100%',
                };
                input.select2(defaultOptions);
            }
            else{
                let input = $('#attribute-value-input');
                if(input[0].tagName == "SELECT"){
                    let inputText =`<input placeholder="Valor del atributo" type="text" id="attribute-value-input" class="form-control not-outline">`;

                    let helpTipoValues = $('.textHelpTipo');
                    helpTipoValues.remove();
    
                    input.replaceWith(inputText);
                }
            }
        });

        //Borrar atributos
        configEventByClassName(`${that.btnRemoveAttribute}`, function (element) {
            let $jqueryElement = $(element);
            $jqueryElement.off().on('click', function (evt) {
                let row = $jqueryElement.closest("tr");
                let rowid = row.find('.propertyAttribute').text();
                row.remove();

                //Eliminamos el atributo de los parametros generales
                let dataset = $('#modal-editar-atributos-propiedad')[0].dataset;
                let propertyid = dataset.propertyid;
                let entityid = dataset.entityid;
                delete that.parametrosGenerales.entidades[entityid].propiedadesVisualizacion[propertyid][rowid];
            })
        });

        //Modal elementos
        configEventByClassName(`${that.btnEditRowClassName}`, function (element) {
            let $jqueryElement = $(element);

            $jqueryElement.off().on('click', function () {
                let idEntity = undefined;
                let header = $jqueryElement.closest('.cmsrow-header');
                let name = $(header).find('.name');
                idEntity = name.text();

                if(idEntity != "Entidad nueva"){
                    that.cargarModalEditarEntidades(idEntity);

                    //Cargar combobox
                    let comboBox = that.getComboBoxPropiedades(idEntity);
                    $('#representante-name-input').html(comboBox);
                    $('#campo-orden-input').html(comboBox);
                    $('#representante-orden-input').html(comboBox);

                    $('#modal-editar-elementos-entidad').modal('show');
                }
                else{
                    mostrarNotificacion("error", "No puedes modificar los elementos de una entidad vacía");
                }
            })
        });

        //Representantes
        let formularioRepresentates = $('#v-pills-representantes');
        this.showForm(formularioRepresentates);
        this.closeForm(formularioRepresentates);

        //Combobox representantes
        that.comboboxRepresentantes.off().on('change', function(evt){
            let comboBox = evt.currentTarget;
            let formularioCaracteres = $('#numcaracteres-input').parent();
            if(comboBox.value == "3"){
                formularioCaracteres.removeClass("d-none");
            }
            else{
                formularioCaracteres.addClass("d-none");
            }
        });

        //Añadir representante
        let modalElementos = $('#modal-editar-elementos-entidad')[0];
        let divTableRepresentante = $('#row-representantes-table');
        let bodyTableRepresentante = divTableRepresentante.find('tbody');
        let btnAddRepresentante = $('#add-representante');
        btnAddRepresentante.off().on('click', function(){
            let entityid = modalElementos.dataset.entityid;
            
            let nameInput = $('#representante-name-input');
            let tipoInput = $('#representante-tipo-input');
            let numCaracteresInput = $('#numcaracteres-input');

            let atributo = nameInput.val();
            let tipo = tipoInput.val();
            let tipoText = tipoInput.find(`[value="${tipo}"]`).text()
            let numCaracteres = "0";
            if(tipo == "3"){
                let numero = Number(numCaracteresInput.val())
                if(!isNaN(numero)){
                    numCaracteres = numCaracteresInput.val();
                }
                else{
                    numCaracteres = ""
                    mostrarNotificacion("error", "El número de caracteres debe ser válido");
                }
            }
            
            if(tipo=="0" || tipo=="1" || tipo=="2" || (tipo=="3" && numCaracteres!="")){
                let tdRepresentante = bodyTableRepresentante.find(`[id='${atributo}']`);
                let tdNumCaracteres = tdRepresentante.closest('tr').find('.numcaracteres');
                //Si no existe la fila se añade
                if(tdRepresentante.length == 0)
                {
                    let newRow = 
                    `<tr>
                        <td class="propertyAttribute">${atributo}</td>
                        <td id="${atributo}">${tipoText}</td>
                        <td class="numcaracteres">${numCaracteres}</td>
                        <td class="delete deleteRepresentante"><span class="material-icons">delete</span></td>
                    </tr>`;

                    bodyTableRepresentante.append($(newRow));

                    if(!that.parametrosGenerales.entidades[entityid]){
                        that.parametrosGenerales.entidades[entityid] = {};
                    }
                    
                    if(!that.parametrosGenerales.entidades[entityid].representantes){
                        that.parametrosGenerales.entidades[entityid].representantes =  {};
                    }

                    that.parametrosGenerales.entidades[entityid].representantes[atributo] = {};
                    that.parametrosGenerales.entidades[entityid].representantes[atributo].tipo = tipo;
                    that.parametrosGenerales.entidades[entityid].representantes[atributo].numCaracteres = numCaracteres;
                }
                else
                {
                    tdRepresentante.text(tipoText);
                    tdNumCaracteres.text(numCaracteres);
                    that.parametrosGenerales.entidades[entityid].representantes[atributo].tipo = tipo;
                    that.parametrosGenerales.entidades[entityid].representantes[atributo].numCaracteres = numCaracteres;
                }
                numCaracteresInput.val("");
            }
            
        });

        //Borrar representantes
        configEventByClassName(`${that.btnRemoveRepresentante}`, function (element) {
            let $jqueryElement = $(element);
            $jqueryElement.off().on('click', function (evt) {
                let row = $jqueryElement.closest("tr");
                let rowid = row.find('.propertyAttribute').text();
                row.remove();

                //Eliminamos el atributo de los parametros generales
                let dataset = $('#modal-editar-elementos-entidad')[0].dataset;
                let entityid = dataset.entityid;
                delete that.parametrosGenerales.entidades[entityid].representantes[rowid];
            })
        });

        //Clases CSS
        //Añadir CSS Panel
        let btnCssPanel = $('#add-css-panel');
        let panelValue = $('#css-panel-value');
        btnCssPanel.off().on('click', function(){
            let entityid = modalElementos.dataset.entityid;
            let panelInput = $('#css-panel-input');
            let panel = panelInput.val();

            if(panel != ""){
                let html = 
                `
                    <span>${panel}</span>
                    <span id="delete-css-panel" class="delete material-icons">delete</span>
                `;

                panelValue.html(html);

                //Si no hay datos de la entidad, se crea el elemento
                if(!that.parametrosGenerales.entidades[entityid]){
                   that.parametrosGenerales.entidades[entityid] = {};
                }

                that.parametrosGenerales.entidades[entityid].clasecsspanel = panel;

                panelInput.val("");
            }
        });

        //Borrar CSS Panel
        configEventById('delete-css-panel', function(element){
            let $jqueryElement = $(element);
            $jqueryElement.off().on('click', function (evt) {
                let entityid = modalElementos.dataset.entityid;
                panelValue.html("");
                delete that.parametrosGenerales.entidades[entityid].clasecsspanel;
            })
        })

        //Añadir CSS Título
        let btnCssTitulo = $('#add-css-titulo');
        let tituloValue = $('#css-titulo-value');
        btnCssTitulo.off().on('click', function(){
            let entityid = modalElementos.dataset.entityid;
            let tituloInput = $('#css-titulo-input');
            let titulo = tituloInput.val();

            if(titulo != ""){
                let html = 
                `
                    <span>${titulo}</span>
                    <span id="delete-css-titulo" class="delete material-icons">delete</span>
                `;

                tituloValue.html(html);

                //Si no hay datos de la entidad, se crea el elemento
                if(!that.parametrosGenerales.entidades[entityid]){
                    that.parametrosGenerales.entidades[entityid] = {};
                }

                that.parametrosGenerales.entidades[entityid].clasecsstitulo = titulo;

                tituloInput.val("");
            }
        });

        //Borrar CSS Título
        configEventById('delete-css-titulo', function(element){
            let $jqueryElement = $(element);
            $jqueryElement.off().on('click', function (evt) {
                let entityid = modalElementos.dataset.entityid;
                tituloValue.html("");
                delete that.parametrosGenerales.entidades[entityid].clasecsstitulo;
            })
        });

        //Etiquetas HTML
        //Añadir Etiqueta HTML Edición
        let btnTagEdicion = $('#add-tag-edicion');
        let tagEdicionValue = $('#html-tag-edicion-value');
        btnTagEdicion.off().on('click', function(){
            let entityid = modalElementos.dataset.entityid;
            let tagEdicionInput = $('#html-tag-edicion');
            let tag = tagEdicionInput.val();

            if(tag != ""){
                let html = 
                `
                    <span>${tag}</span>
                    <span id="delete-tag-edicion" class="delete material-icons">delete</span>
                `;

                tagEdicionValue.html(html);

                //Si no hay datos de la entidad, se crea el elemento
                if(!that.parametrosGenerales.entidades[entityid]){
                    that.parametrosGenerales.entidades[entityid] = {};
                }

                that.parametrosGenerales.entidades[entityid].tagnametituloedicion = tag;

                tagEdicionInput.val("");
            }
        });

        //Borrar Etiqueta HTML Edición
        configEventById('delete-tag-edicion', function(element){
            let $jqueryElement = $(element);
            $jqueryElement.off().on('click', function (evt) {
                let entityid = modalElementos.dataset.entityid;
                tagEdicionValue.html("");
                delete that.parametrosGenerales.entidades[entityid].tagnametituloedicion;
            })
        });

        //Añadir Etiqueta HTML Lectura
        let btnTagLectura = $('#add-tag-lectura');
        let tagLecturaValue = $('#html-tag-lectura-value');
        btnTagLectura.off().on('click', function(){
            let entityid = modalElementos.dataset.entityid;
            let tagLecturaInput = $('#html-tag-lectura');
            let tag = tagLecturaInput.val();

            if(tag != ""){
                let html = 
                `
                    <span>${tag}</span>
                    <span id="delete-tag-lectura" class="delete material-icons">delete</span>
                `;

                tagLecturaValue.html(html);

                //Si no hay datos de la entidad, se crea el elemento
                if(!that.parametrosGenerales.entidades[entityid]){
                    that.parametrosGenerales.entidades[entityid] = {};
                }
    
                that.parametrosGenerales.entidades[entityid].tagnametitulolectura = tag;
    
                tagLecturaInput.val("");
            }
        });

        //Borrar Etiqueta HTML Edición
        configEventById('delete-tag-lectura', function(element){
            let $jqueryElement = $(element);
            $jqueryElement.off().on('click', function (evt) {
                let entityid = modalElementos.dataset.entityid;
                tagLecturaValue.html("");
                delete that.parametrosGenerales.entidades[entityid].tagnametitulolectura;
            })
        });

        //Títulos
        //AtrNombre
        let formularioTituloEdicion = $('#formulario-titulo-edicion');
        this.showForm(formularioTituloEdicion);
        this.closeForm(formularioTituloEdicion);

        //Añadir Título Edición
        let divAtrNombre = $('#row-atrnombre-table');
        let bodyTableAtrNombre = divAtrNombre.find('tbody');
        let btnAddAtrNombre = $('#add-atrnombre');
        btnAddAtrNombre.off().on('click', function(){
            let entityid = modalElementos.dataset.entityid;

            let tituloInput = $('#atrnombre-value');
            let langInput = $('#atrnombre-lang-input');

            let titulo = tituloInput.val();
            let lang = langInput.val();

            if(titulo != ""){
                let row = bodyTableAtrNombre.find('#' + lang);
                //Si no existe la fila se añade
                if(row.length == 0)
                {
                    let newRow = 
                    `<tr>
                        <td id="${lang}">${titulo}</td>
                        <td class="propertyAttribute">${lang}</td>
                        <td class="delete delete-atrnombre"><span class="material-icons">delete</span></td>
                    </tr>`;

                    bodyTableAtrNombre.append($(newRow));

                    if(!that.parametrosGenerales.entidades[entityid]){
                        that.parametrosGenerales.entidades[entityid] = {};
                    }

                    if(!that.parametrosGenerales.entidades[entityid].atrnombres){
                        that.parametrosGenerales.entidades[entityid].atrnombres =  {};
                    }
                    
                    that.parametrosGenerales.entidades[entityid].atrnombres[lang] = {};
                    that.parametrosGenerales.entidades[entityid].atrnombres[lang] = titulo;

                }
                else
                {
                    row.text(titulo);
                    that.parametrosGenerales.entidades[entityid].atrnombres[lang] = titulo;
                }
            }

            tituloInput.val("");
        });

        //Borrar AtrNombre
        configEventByClassName(`${that.btnRemoveAtrNombre}`, function (element) {
            let $jqueryElement = $(element);
            $jqueryElement.off().on('click', function (evt) {
                let row = $jqueryElement.closest("tr");
                let rowid = row.find('.propertyAttribute').text();
                row.remove();

                //Eliminamos el atributo de los parametros generales
                let dataset = $('#modal-editar-elementos-entidad')[0].dataset;
                let entityid = dataset.entityid;
                delete that.parametrosGenerales.entidades[entityid].atrnombres[rowid];
            })
        });

        //AtrNombreLectura
        let formularioTituloVisualizacion = $('#formulario-titulo-visualizacion');
        this.showForm(formularioTituloVisualizacion);
        this.closeForm(formularioTituloVisualizacion);

        //Añadir Título Lectura
        let divAtrNombreLectura = $('#row-atrnombrelectura-table');
        let bodyTableAtrNombreLectura = divAtrNombreLectura.find('tbody');
        let btnAddAtrNombreLectura = $('#add-atrnombrelectura');
        btnAddAtrNombreLectura.off().on('click', function(){
            let entityid = modalElementos.dataset.entityid;

            let tituloInput = $('#atrnombrelectura-value');
            let langInput = $('#atrnombrelectura-lang-input');

            let titulo = tituloInput.val();
            let lang = langInput.val();

            if(titulo != ""){
                let row = bodyTableAtrNombreLectura.find('#' + lang);
                //Si no existe la fila se añade
                if(row.length == 0)
                {
                    let newRow = 
                    `<tr>
                        <td id="${lang}">${titulo}</td>
                        <td class="propertyAttribute">${lang}</td>
                        <td class="delete delete-atrnombrelectura"><span class="material-icons">delete</span></td>
                    </tr>`;

                    bodyTableAtrNombreLectura.append($(newRow));

                    if(!that.parametrosGenerales.entidades[entityid]){
                        that.parametrosGenerales.entidades[entityid] = {};
                    }

                    if(!that.parametrosGenerales.entidades[entityid].atrnombrelecturas){
                        that.parametrosGenerales.entidades[entityid].atrnombrelecturas =  {};
                    }
                    
                    that.parametrosGenerales.entidades[entityid].atrnombrelecturas[lang] = {};
                    that.parametrosGenerales.entidades[entityid].atrnombrelecturas[lang] = titulo;

                }
                else
                {
                    row.text(titulo);
                    that.parametrosGenerales.entidades[entityid].atrnombrelecturas[lang] = titulo;
                }
            }

            tituloInput.val("");
        });

        //Borrar AtrNombreLectura
        configEventByClassName(`${that.btnRemoveAtrNombreLectura}`, function (element) {
            let $jqueryElement = $(element);
            $jqueryElement.off().on('click', function (evt) {
                let row = $jqueryElement.closest("tr");
                let rowid = row.find('.propertyAttribute').text();
                row.remove();

                //Eliminamos el atributo de los parametros generales
                let dataset = $('#modal-editar-elementos-entidad')[0].dataset;
                let entityid = dataset.entityid;
                delete that.parametrosGenerales.entidades[entityid].atrnombrelecturas[rowid];
            })
        });

        //Añadir microdatos
        let btnMicrodatos = $('#add-microdatos');
        let microdatosValue = $('#microdatos-value');
        btnMicrodatos.off().on('click', function(){
            let entityid = modalElementos.dataset.entityid;
            let microdatosInput = $('#microdatos-input');
            let microdatos = microdatosInput.val();

            if(microdatos != ""){
                let html = 
                `
                    <span>${microdatos}</span>
                    <span id="delete-microdatos" class="delete material-icons">delete</span>
                `;

                microdatosValue.html(html);

                //Si no hay datos de la entidad, se crea el elemento
                if(!that.parametrosGenerales.entidades[entityid]){
                    that.parametrosGenerales.entidades[entityid] = {};
                }
    
                that.parametrosGenerales.entidades[entityid].microdatos = microdatos;
    
                microdatosInput.val("");
            }
        });

        //Borrar microdatos
        configEventById('delete-microdatos', function(element){
            let $jqueryElement = $(element);
            $jqueryElement.off().on('click', function (evt) {
                let entityid = modalElementos.dataset.entityid;
                microdatosValue.html("");
                delete that.parametrosGenerales.entidades[entityid].microdatos;
            })
        });

        //Añadir campo orden
        let btnCampoOrden = $('#add-campo-orden');
        let campoOrdenValue = $('#campo-orden-value');
        btnCampoOrden.off().on('click', function(){
            let entityid = modalElementos.dataset.entityid;
            let campoOrdenInput = $('#campo-orden-input');
            let campoOrden = campoOrdenInput.val();

            let html = 
                `
                    <span>${campoOrden}</span>
                    <span id="delete-campo-orden" class="delete material-icons">delete</span>
                `;

                campoOrdenValue.html(html);

                //Si no hay datos de la entidad, se crea el elemento
                if(!that.parametrosGenerales.entidades[entityid]){
                    that.parametrosGenerales.entidades[entityid] = {};
                }
    
                that.parametrosGenerales.entidades[entityid].campoorden = campoOrden;
        });

        //Borrar microdatos
        configEventById('delete-campo-orden', function(element){
            let $jqueryElement = $(element);
            $jqueryElement.off().on('click', function (evt) {
                let entityid = modalElementos.dataset.entityid;
                campoOrdenValue.html("");
                delete that.parametrosGenerales.entidades[entityid].campoorden;
            })
        });

        //Añadir campo representante orden
        let btnCampoRepresentanteOrden = $('#add-representante-orden');
        let campoRepresentanteOrdenValue = $('#representante-orden-value');
        btnCampoRepresentanteOrden.off().on('click', function(){
            let entityid = modalElementos.dataset.entityid;
            let campoRepresentanteOrdenInput = $('#representante-orden-input');
            let campoRepresentanteOrden = campoRepresentanteOrdenInput.val();

            let html = 
                `
                    <span>${campoRepresentanteOrden}</span>
                    <span id="delete-representante-orden" class="delete material-icons">delete</span>
                `;

                campoRepresentanteOrdenValue.html(html);

                //Si no hay datos de la entidad, se crea el elemento
                if(!that.parametrosGenerales.entidades[entityid]){
                    that.parametrosGenerales.entidades[entityid] = {};
                }
    
                that.parametrosGenerales.entidades[entityid].camporepresentanteorden = campoRepresentanteOrden;
        });

        //Borrar campo representante orden
        configEventById('delete-representante-orden', function(element){
            let $jqueryElement = $(element);
            $jqueryElement.off().on('click', function (evt) {
                let entityid = modalElementos.dataset.entityid;
                campoRepresentanteOrdenValue.html("");
                delete that.parametrosGenerales.entidades[entityid].camporepresentanteorden;
            })
        });

        //Cargar el modal de configuración general
        $('#elementos-configuracion-general').on('click', function(){
            that.cargarModalConfiguracionGeneral();
        })
    },

    showForm: function (formulario) {
        let btnNewAttribute = $(formulario.find('.new-formulario')[0]);
        let inputs = $(formulario.find('.formulario-new-element')[0]);
        btnNewAttribute.off().on('click', function(){
            btnNewAttribute.addClass('d-none');
            inputs.css('display','flex');
        });
    },

    closeForm: function (formulario) {
        let btnNewAttribute = $(formulario.find('.new-formulario')[0]);
        let btnCloseForm = $(formulario.find('.close-formulario')[0]);
        let inputs = $(formulario.find('.formulario-new-element')[0]);
        btnCloseForm.off().on('click', function(){
            inputs.css('display','none');
            btnNewAttribute.removeClass('d-none');
        });
    },

    /* Configuración de la ruta del controlador */
    configRutas: function () {
        this.urlBase = refineURL();
        this.urlSaveXMLPage = this.urlBase;
    },

    /* Configuración de los eventos de la página */
    configEvents: function () {
        const that = this;

        //Añadir nueva fila
        this.btnAddRow.off().on('click', function (event) {
            that.addNewRow();
        });

        //Añadir funcionalidad drag-n-drop a las entidades
        var list = document.getElementById('xmlrow-list') 
        Sortable.create(list, {
            handle: '.' + that.btnDragRowClassName
        })

        //Añadir funcionalidad drag-n-drop a las propiedades del panel lateral
        Array.from(this.createdListEntities).forEach(e =>
            Sortable.create(e, {
                group: {
                    name: 'new',
                    pull: 'clone',
                    put: 'false',
                },
                handle: '.handler',
                sort: false,
                onRemove: function (evt) {
                    if (this.options.group.name == 'new') {
                        let entityid = evt.from.dataset.entityid;
                        this.options.group.name = entityid;
                    }
                }
            })
        );

        // Añadir funcionalidad drag-n-drop a las nuevas filas       
        configEventByClassName(`${that.rows}`, function (element) {
            let $jqueryElement = $(element);
            Array.from($jqueryElement).forEach(e =>
                Sortable.create(e,
                    {
                        group: 'new',
                        handle: '.handler',
                        onAdd: function (evt) {
                            let entityid = evt.from.dataset.entityid;
                            if (evt.to.dataset.entityid === undefined) {
                                evt.to.dataset.entityid = entityid;
                                this.options.group.name = entityid;
                                
                                //Hacer que el otro elemento también pertenezca al grupo y de esta forma se puedan arrastrar elementos
                                let elemento = $(this.el).siblings(".xmlcontent")[0];
                                let otro = Sortable.get(elemento);
                                otro.options.group.name = entityid;
                            }

                            let li = evt.item;
                            let propiedadId = $(li).find('.name').text();
                            let liexistentes = $(evt.to).find(`.name:contains(${propiedadId})`);
                            let ficha = evt.to.dataset.ficha;
                            //Es 1 porque al hacer la comprobación ya se ha añadido el elemento
                            if(ficha == "visualizacion" || (ficha == "edicion" && liexistentes.length == 1))
                            {
                                //Añadimos botón de borrar
                                let divBorrar = $(li).find('.borrarpropiedad');
                                if(divBorrar.length == 0){
                                    let borrar = $(`<div class="delete borrarpropiedad"><span class="material-icons">delete</span></div>`);
                                    let handler = $(li).find('.handler');
                                    handler.before(borrar);
                                }
                    
                                //Añadimos botón de editar
                                let divEditar = $(li).find('.editarpropiedad');
                                if(divEditar.length == 0 && ficha == "visualizacion"){
                                    let borrar = $(li).find('.borrarpropiedad');
                                    let editar = $(`<div class="editarpropiedad"><span class="material-icons">edit</span></div>`);
                                    borrar.before(editar);
                                }
                                else{
                                    //Si ya tenía el botón y es la ficha de edición, lo quitamos
                                    divEditar.remove();
                                }
                                
                                //Cambiar el nombre
                                let row = $(`ul[data-entityid='${entityid}'].xmlcontent`);
                                let divname = row.closest('.cmsrow-wrap').find('.cmsrow-header .name');
                                if (divname.text() != entityid)
                                {
                                    divname.text(entityid);

                                    //Colocamos el id para controlar la visibilidad
                                    let cmsrowwrap = li.closest('.cmsrow-wrap');
                                    let visibilidad = $(cmsrowwrap).find('.js-view-row')[0];
                                    visibilidad.dataset.target = `.cmsrow-content[data-entityid='${entityid}']`;

                                    let cmsrowcontent = $(cmsrowwrap).find('.cmsrow-content')[0];
                                    cmsrowcontent.dataset.entityid = entityid;
                                }
                            }
                            else{
                                //Evitamos las propiedades duplicadas en la ficha de edición
                                $(li).remove();
                                mostrarNotificacion("error", "La propiedad " + propiedadId + " ya está en la ficha de edición");
                            }
                        }
                    }
                )
            );
        });

        //Eliminación de las filas
        configEventByClassName(`${that.btnDeleteRowClassName}`, function (element) {
            let $jqueryElement = $(element);
            $jqueryElement.off().on('click', function () {
                $('#modal-confirmar-eliminar').modal('show');

                $('.js-delete-yes').off().on('click', function () {
                    //Borramos el elemento de la vista
                    let row = $jqueryElement.closest(".cmsrow");
                    let rowid = row.find('.cmsrow-header .name').text();
                    row.remove();

                    delete that.parametrosGenerales.entidades[rowid];

                    //Rescatamos la lista para cambiarle el nombre al elemento Sortable y así poder arrastrarlo de nuevo
                    let ulproperties = document.querySelector(`ul[data-entityid='${rowid}'].created-entities`);
                    if(ulproperties){
                        let sortable = Sortable.get(ulproperties);
                        sortable.options.group.name = 'new';
                    }
                })
            })
        });

        //Eliminación de propiedades
        configEventByClassName(`${that.btnRemoveProperty}`, function (element) {
            let $jqueryElement = $(element);
            $jqueryElement.off().on('click', function () {
                $('#modal-confirmar-eliminar').modal('show');

                $('.js-delete-yes').off().on('click', function () {
                    let row = $jqueryElement.closest("li");

                    //Lo eliminamos de los parámetros
                    let ficha = row.closest("ul")[0].dataset.ficha;
                    let propertyid = row.find(".name").text();
                    let rowwrap = $(row).closest(".cmsrow-wrap");
                    let entityid = rowwrap.find(".cmsrow-header .name").text();

                    if(that.parametrosGenerales.entidades[entityid] && ficha!="edicion"){
                        if(that.parametrosGenerales.entidades[entityid].propiedadesVisualizacion){
                            delete that.parametrosGenerales.entidades[entityid].propiedadesVisualizacion[propertyid];
                        }
                    }
                    
                    //Y de la interfaz
                    row.remove();
                })
            })
        });

        //Edición de propiedades
        configEventByClassName(`${that.btnEditProperty}`, function (element) {
            let $jqueryElement = $(element);
            
            $jqueryElement.off().on('click', function () {
                let row = $jqueryElement.closest("li");
                let idProperty = row.find(".name").text();
                let ul = $(row).closest("ul");
                let idEntity = ul[0].dataset.entityid;

                that.cargarModalEditarPropiedades(idEntity, idProperty);
                $('#modal-editar-atributos-propiedad').modal('show');
            })
        });

        //Botón de guardado de las vistas
        this.btnSave.off().on('click', function () {
            that.saveXMLPage();
        })

        //Collapse panel lateral
        Array.from(this.lateralCollapse).forEach(e => {
            $(e).off().on('click', function(){
               let icono = $(e).find('.material-icons');
               let iconotext = icono.text();
               if(iconotext == "expand_more"){
                    icono.text("expand_less");
               }
               else{
                    icono.text("expand_more");
               }
            })
        });

        //Collapse entidades
        configEventByClassName(`${that.entityCollapseClassName}`, function (element) {
            let $jqueryElement = $(element);
            $jqueryElement.off().on('click', function () {
                let icono = $jqueryElement.find('.material-icons');
                let iconotext = icono.text();
                if(iconotext == "visibility"){
                    icono.text("visibility_off");
                }
                else{
                    icono.text("visibility");
                }
            })
        });

        // Buscador panel lateral de propiedades
        this.buscadorPropiedades.off().on("keyup", function (event) {
            const input = $(this);
            clearTimeout(that.timer);
            that.timer = setTimeout(function () {                                                                        
                // Acción de buscar / filtrar
                that.buscarPropiedad(input);                                         
            }, 500);
        });
    },

    /* Configuración de los elmentos arrastrables de la página */
    configSortables: function () {
        let elementos = $(".xmlcontent")
        Array.from(elementos).forEach(e => {
            let entityid = e.dataset.entityid
            //Creamos los elementos sortable
            let sortable = Sortable.get(e);
            sortable.options.group.name = entityid

            let ulproperties = document.querySelector(`ul[data-entityid='${entityid}'].created-entities`);
            let sortablepanel = Sortable.get(ulproperties);
            sortablepanel.options.group.name = entityid;
        })
    },

    /* Métodos adicionales */
    
    /* Añade una nueva entidad al área de trabajo */
    addNewRow: function() {
        let rowTemplate = this.getRowHtmlTemplate();
        this.rowList.append($(rowTemplate));
    },

    /* Devuelve la plantilla HTML de una entidad vacía */
    getRowHtmlTemplate: function () {
        return `<li class="builder-item cmsrow js-cmsrow" data-columns="12">
                    <div class="cmsrow-wrap">
                        <div class="cmsrow-header">
                            <div class="name">Entidad nueva</div>
                            <div class="js-cmsrow-actions cmsrow-actions">
                                <ul class="no-list-style">
                                    <li>
                                        <a class="round-icon-button js-view-row" data-toggle="collapse">
                                            <span class="material-icons">visibility</span>
                                        </a>
                                    </li>
                                    <li>
                                        <a class="action-edit round-icon-button js-action-edit-row">
                                            <span class="material-icons">edit</span>
                                        </a>
                                    </li>
                                    <li>
                                        <a class="delete action-delete round-icon-button js-action-delete">
                                            <span class="material-icons">delete</span>
                                        </a>
                                    </li>
                                    <li>
                                        <a class="action-handle round-icon-button js-action-handle-row">
                                            <span class="material-icons">drag_handle</span>
                                        </a>
                                    </li>
                                </ul>
                            </div>
                        </div>
                        <div class="cmsrow-content collapse show">
                            <ul class="columns-list no-list-style">
                                <li class="cmscolumn js-cmscolumn" data-columnclass="span11 break" data-spanclass="span11" data-percent="100" style="width: calc(100% - 16px);">
                                    <div class="cmscolumn-wrap">
                                        <div class="cmscolumn-content">
                                            <span class="xmlorden">Edición</span>
                                            <ul class="xmlcontent" data-ficha="edicion"></ul>
                                            <hr>
                                            <span class="xmlorden">Visualización</span>
                                            <ul class="xmlcontent" data-ficha="visualizacion"></ul>
                                        </div>
                                    </div>
                                </li>
                            </ul>
                        </div>
                    </div>
                </li>`;
    },

    /* Realiza el guardado de los elementos realizando una petición al controlador */
    saveXMLPage: function () {
        const that = this;
        const arg = {};

        that.guardarConfiguracionGeneral();
        arg.configuracionGeneral = that.parametrosGenerales.configuracionGeneral; 
        arg.listaEntidades = that.getEntities();
       
        loadingMostrar();
        GnossPeticionAjax(`${that.urlSaveXMLPage}/save`, arg, true
        ).done(function (data) {
            mostrarNotificacion("success", "Las vistas se han guardado correctamente.");
        }).fail(function (error) {
            mostrarNotificacion("error", error);
        }).always(function (){
            loadingOcultar();
        });
    },

    /* Devuelve la estructura de entidades y propiedades */
    getEntities: function(){
        const that = this;

        let entidades = []
        let xmlContentList = $('.xmlcontent');
        Array.from(xmlContentList).forEach(e => {
            let entity = {};
            entity.ID = e.dataset.entityid;

            //Almacenamos los elementos de la entidad
            let elementosEntidad = {};
            if(that.parametrosGenerales.entidades[entity.ID]){
                elementosEntidad = that.parametrosGenerales.entidades[entity.ID];
            }

            //Comprobamos si la lista anterior pertenece a la misma entidad
            let entidadAnterior = entidades.pop();
            if(entidadAnterior && entidadAnterior.ID == entity.ID){
                entidadAnterior.PropiedadesOrdenEntidadLectura = [];

                //Almacenamos las propiedades de la vista de visualización
                let props = $(e).find('li');
                Array.from(props).forEach(p => {
                    let propiedad = {};

                    let url = $(p).find('.name').text();
                    propiedad.ID = url;

                    let atributos = {};
                    //Almacenamos los atributos de la propiedad 
                    if(elementosEntidad.propiedadesVisualizacion){
                        if(elementosEntidad.propiedadesVisualizacion[url]){
                            atributos = elementosEntidad.propiedadesVisualizacion[url];
                        }
                    }
                    propiedad.Atributos = atributos;
                    entidadAnterior.PropiedadesOrdenEntidadLectura.push(propiedad);
                });

                entidades.push(entidadAnterior);
            }
            else{
                //Introducimos la entidad anterior
                if(entidadAnterior){
                    entidades.push(entidadAnterior);
                }

                //Y construimos la nueva
                entity.PropiedadesOrdenEntidad = [];
                entity.Representantes = [];
                entity.AtrNombres = [];
                entity.AtrNombreLecturas = [];
               
                let props = $(e).find('li');
                Array.from(props).forEach(p => {
                    let propiedad = {};
    
                    let url = $(p).find('.name').text();
                    propiedad.ID = url;
    
                    entity.PropiedadesOrdenEntidad.push(propiedad);
                });
    
                //Almacenamos los representantes de la propiedad
                let representantes = elementosEntidad.representantes;
                if(representantes){
                    let atributos = Object.keys(representantes);
                    atributos.forEach(atributo => {
                        let representante = {};
                        representante.Valor = atributo;
                        representante.Tipo = representantes[atributo].tipo;
                        representante.NumCaracteres = representantes[atributo].numCaracteres;
    
                        entity.Representantes.push(representante);
                    });
                }
    
                //Almacenamos las clases CSS
                let cssPanel = elementosEntidad.clasecsspanel;
                if(cssPanel){
                    entity.ClaseCssPanel = cssPanel;
                }
    
                let cssTitulo = elementosEntidad.clasecsstitulo;
                if(cssTitulo){
                    entity.ClaseCssTitulo = cssTitulo;
                }
    
                //Almacenamos las etiquetas HTML
                let tagEdicion = elementosEntidad.tagnametituloedicion;
                if(tagEdicion){
                    entity.TagNameTituloEdicion = tagEdicion;
                }
    
                let tagLectura = elementosEntidad.tagnametitulolectura;
                if(tagLectura){
                    entity.TagNameTituloLectura = tagLectura;
                }
    
                //Almacenamos los títulos
                //AtrNombre (Edicion)
                let atrnombres = elementosEntidad.atrnombres;
                if(atrnombres){
                    let idiomas = Object.keys(atrnombres);
                    idiomas.forEach(idioma => {
                        let atrnombre = {};
                        atrnombre.Text = atrnombres[idioma];
                        atrnombre.Lang = idioma;
    
                        entity.AtrNombres.push(atrnombre);
                    });
                }
    
                //AtrNombreLectura (Visualización)
                let atrnombrelecturas = elementosEntidad.atrnombrelecturas;
                if(atrnombrelecturas){
                    let idiomas = Object.keys(atrnombrelecturas);
                    idiomas.forEach(idioma => {
                        let atrnombrelectura = {};
                        atrnombrelectura.Text = atrnombrelecturas[idioma];
                        atrnombrelectura.Lang = idioma;
    
                        entity.AtrNombreLecturas.push(atrnombrelectura);
                    });
                }
    
                //Almacenamos los microdatos
                let microdatos = elementosEntidad.microdatos;
                if(microdatos){
                    entity.Microdatos = microdatos;
                }
    
                //Almacenamos campo orden
                let campoorden = elementosEntidad.campoorden;
                if(campoorden){
                    entity.CampoOrden = campoorden;
                }
    
                //Almacenamos campo representante orden
                let camporepresentanteorden = elementosEntidad.camporepresentanteorden;
                if(camporepresentanteorden){
                    entity.CampoRepresentanteOrden = camporepresentanteorden;
                }
    
                entidades.push(entity);
            }
        })
        return entidades;
    },

    /* Carga los datos en el modal de la propiedad que queremos editar */
    cargarModalEditarPropiedades: function(idEntity, idProperty){
        const that = this;

        let propiedades = undefined;

        //Si hay datos de la entidad...
        if(that.parametrosGenerales.entidades[idEntity]){
            //...y hay datos de la propiedad
            if(that.parametrosGenerales.entidades[idEntity].propiedadesVisualizacion){
                propiedades = that.parametrosGenerales.entidades[idEntity].propiedadesVisualizacion[idProperty];
            }
        }
        
        let modal = $("#modal-editar-atributos-propiedad");
        modal[0].dataset.entityid = idEntity;
        modal[0].dataset.propertyid = idProperty;

        let tituloModal = modal.find('#nombre-propiedad-modal');
        tituloModal.text(idProperty);

        let tabla = $("#modal-editar-atributos-propiedad tbody");
        tabla.html("");
        if(propiedades != undefined){
            for(let atributo in propiedades){
                let newRow = 
                    `<tr>
                        <td class="propertyAttribute">${atributo}</td>
                        <td id="${atributo}">${propiedades[atributo]}</td>
                        <td class="delete deleteAttribute"><span class="material-icons">delete</span></td>
                    </tr>`;

                    tabla.append($(newRow));
            };
        }
    },

    /* Carga los datos en el modal de la entidad que queremos editar */
    cargarModalEditarEntidades: function(idEntity){
        const that = this;

        let modal = $('#modal-editar-elementos-entidad');
        modal[0].dataset.entityid = idEntity;

        let tituloModal = modal.find('#nombre-entidad-modal');
        tituloModal.text(idEntity);

        //Representantes
        let representantes = undefined;

        //Clases CSS
        let clasecsspanel = undefined;
        let clasecsstitulo = undefined;

        //Etiquetas HTML
        let tagnametituloedicion = undefined;
        let tagnametitulolectura = undefined;

        //Títulos
        let atrnombres = undefined;
        let atrnombrelecturas = undefined;

        //Microdatos
        let microdatos = undefined;

        //Orden
        let campoorden = undefined;
        let camporepresentanteorden = undefined;

        //Si hay datos de la entidad...
        if(that.parametrosGenerales.entidades[idEntity]){
            let entidad = that.parametrosGenerales.entidades[idEntity]
            representantes = entidad.representantes;
            
            clasecsspanel = entidad.clasecsspanel;
            clasecsstitulo = entidad.clasecsstitulo;

            tagnametituloedicion = entidad.tagnametituloedicion;
            tagnametitulolectura = entidad.tagnametitulolectura;

            atrnombres = entidad.atrnombres;
            atrnombrelecturas = entidad.atrnombrelecturas;

            microdatos = entidad.microdatos;

            campoorden = entidad.campoorden;
            camporepresentanteorden = entidad.camporepresentanteorden;
        }
        
        let tabla = $("#row-representantes-table tbody");
        tabla.html("");
        if(representantes != undefined){
            let atributos = Object.keys(representantes);
            let tipoInput = $('#representante-tipo-input');
            atributos.forEach(atributo => {
                let tipoText = tipoInput.find(`[value="${representantes[atributo].tipo}"]`).text()
                let newRow = 
                    `<tr>
                        <td class="propertyAttribute">${atributo}</td>
                        <td id="${atributo}">${tipoText}</td>
                        <td class="numcaracteres">${representantes[atributo].numCaracteres}</td>
                        <td class="delete deleteRepresentante"><span class="material-icons">delete</span></td>
                    </tr>`;

                    tabla.append($(newRow));
            });
        }

        let panelValue = $('#css-panel-value');
        panelValue.html("");
        if(clasecsspanel != undefined){
            let html = 
            `
                <span>${clasecsspanel}</span>
                <span id="delete-css-panel" class="delete material-icons">delete</span>
            `;

            panelValue.html(html);
        }

        let tituloValue = $('#css-titulo-value');
        tituloValue.html("");
        if(clasecsstitulo != undefined){
            let html = 
            `
                <span>${clasecsstitulo}</span>
                <span id="delete-css-titulo" class="delete material-icons">delete</span>
            `;

            tituloValue.html(html);  
        }

        let tagEdicionValue = $('#html-tag-edicion-value');
        tagEdicionValue.html("");
        if(tagnametituloedicion != undefined){
            let html = 
            `
                <span>${tagnametituloedicion}</span>
                <span id="delete-tag-edicion" class="delete material-icons">delete</span>
            `;

            tagEdicionValue.html(html);
        }

        let tagLecturaValue = $('#html-tag-lectura-value');
        tagLecturaValue.html("");
        if(tagnametitulolectura != undefined){
            let html = 
            `
                <span>${tagnametitulolectura}</span>
                <span id="delete-tag-lectura" class="delete material-icons">delete</span>
            `;

            tagLecturaValue.html(html);
        }

        let tablaAtrNombre = $("#row-atrnombre-table tbody");
        tablaAtrNombre.html("");
        if(atrnombres != undefined){
            let atributos = Object.keys(atrnombres);
            atributos.forEach(atributo => {
                let newRow = 
                    `<tr>
                        <td id="${lang}">${atrnombres[atributo]}</td>
                        <td class="propertyAttribute">${lang}</td>
                        <td class="delete delete-atrnombre"><span class="material-icons">delete</span></td>
                    </tr>`;

                    tablaAtrNombre.append($(newRow));
            });
        }

        let tablaAtrNombreLectura = $("#row-atrnombrelectura-table tbody");
        tablaAtrNombreLectura.html("");
        if(atrnombrelecturas != undefined){
            let atributos = Object.keys(atrnombrelecturas);
            atributos.forEach(atributo => {
                let newRow = 
                    `<tr>
                        <td id="${atributo}">${atrnombrelecturas[atributo]}</td>
                        <td class="propertyAttribute">${atributo}</td>
                        <td class="delete delete-atrnombrelectura"><span class="material-icons">delete</span></td>
                    </tr>`;

                    tablaAtrNombreLectura.append($(newRow));
            });
        }

        let microdatosValue = $('#microdatos-value');
        microdatosValue.html("");
        if(microdatos != undefined){
            let html = 
            `
                <span>${microdatos}</span>
                <span id="delete-microdatos" class="delete material-icons">delete</span>
            `;

            microdatosValue.html(html);
        }

        let campoOrdenValue = $('#campo-orden-value');
        campoOrdenValue.html("");
        if(campoorden != undefined){
            let html = 
            `
                <span>${campoorden}</span>
                <span id="delete-campo-orden" class="delete material-icons">delete</span>
            `;

            campoOrdenValue.html(html);
        }

        let campoRepresentanteValue = $('#representante-orden-value');
        campoRepresentanteValue.html("");
        if(camporepresentanteorden != undefined){
            let html = 
            `
                <span>${camporepresentanteorden}</span>
                <span id="delete-representante-orden" class="delete material-icons">delete</span>
            `;

            campoRepresentanteValue.html(html);
        }
    },

    /* Devuelve el HTML del ComboBox de propiedades */
    getComboBoxPropiedades: function(idEntity){
        let propiedades = [];
        let html = ``;
        
        let ulproperties = $(`ul.created-entities[data-entityid='${idEntity}']`);
        let elementos = Array.from(ulproperties.children());

        elementos.forEach(e => {
            let propiedad = $(e).find('.name').text();
            html += `<option>${propiedad}</option>\n`;
            propiedades.push(propiedad);
        }); 

        html = html.slice(0,-1); //quitar el último salto de línea

        return html;
    },

    /* Recoge los elementos de la configuración general que se han seleccionado */
    guardarConfiguracionGeneral: function(){
        const that = this;

        that.parametrosGenerales.configuracionGeneral = [];

        //Guardamos los elementos que están seleccionados
        $('#modal-elementos-configuracion-general input:checked').each(function() {
            that.parametrosGenerales.configuracionGeneral.push($(this).attr('id'));
        });
    },

    /* Carga los datos en el modal de la configuración general */
    cargarModalConfiguracionGeneral: function()
    {
        const that = this;

        let elementos = that.parametrosGenerales.configuracionGeneral;
        elementos.forEach(e => {
            $('#'+ e).attr('checked', true);
        });
    },

    /* Buscar propiedades del panel lateral */
    buscarPropiedad: function(input){     
        const that = this;
        // Cadena introducida para filtrar/buscar
        let cadena = input.val();
        // Eliminamos posibles tildes para búsqueda ampliada
        cadena = cadena.normalize('NFD').replace(/[\u0300-\u036f]/g, "").toLowerCase();

        // Listado de componentes donde se buscará
        let propiedades = $('.created-entities li');

        // Recorrer los items/compontes para realizar la búsqueda de palabras clave
        $.each(propiedades, function(){
            let propiedad = $(this);           
            // Seleccionar el nombre sólo "visible" de la traducción. Quitamos caracteres extraños, tildes para poder hacer bien la búsqueda
            let propiedadContenido = propiedad.find(".name").not('.d-none').html().trim().normalize("NFD").replace(/[\u0300-\u036f]/g, "").toLowerCase();

            if (propiedadContenido.includes(cadena)){
                // Mostrar propiedad
                propiedad.removeClass("d-none");                
            }
            else{
                // Ocultar propiedad
                propiedad.addClass("d-none");
            }
        });
        
        // Bloques de entidades
        let entidades = $('.entidad-panel-lateral');
        $.each(entidades, function(){
            let entidad = $(this);
            // Se comprueba si tiene propiedades que no están ocultas
            let propiedadesMostradas = entidad.find('li').not('.d-none');
            // Si todas están ocultas...
            if(propiedadesMostradas.length == 0){
                //... se oculta la entidad
                entidad.addClass("d-none");
            }
            else
            {
                entidad.removeClass("d-none");
            }
        });
    },
}

