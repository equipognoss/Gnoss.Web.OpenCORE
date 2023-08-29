/**
 ***************************************************************************************
 * Logica / Operativas de comportamiento JS para la sección de Estructura, en concreto, el CMS Builder de páginas de la Comunidad del DevTools
 * *************************************************************************************
 */





const operativaConfiguracionOC = {

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
     * Método a ejecutar para inicialización de funcionalidad necesaria 
     */
    triggerEvents: function(){
        const that = this;
    },  

    
    

    /**
     * Configuración de las rutas de acciones necesarias para hacer peticiones a backend
     */
     configRutas: function (pParams) {
        this.urlBase = refineURL();
        // this.urlSave = `${this.urlBase}/save`;
        
    },
    
    /*
     * Inicializar elementos de la vista
     * */
    config: function (pParams) {

        this.propertyItemClassName = "propertyItem";
        this.valorDefecto = "valorDefecto";
        this.eliminarValorCombo = "eliminarValor";
        this.eliminarMiniatura = "eliminarMiniatura";
        this.eliminarOpenSD = "eliminarOpenSD";
        this.turnMultidiomaOnClassName = "turnMultidiomaOn";
        this.turnMultidiomaOffClassName = "turnMultidiomaOff";
        this.checkMultidiomaClassName = "checkMultidioma";
        this.btnConfigurarClassName = "btnconfigurar";
        this.entidadClassName = "entidad";
        this.propiedadClassName = "propiedad";
        this.botonMiniaturasClassName = "btnMiniaturas";
        this.botonOpenSeaDragonClassName = "btnOpenSeaDragon";
        this.expandir = "expandir";
        this.desplegarTodo = "desplegarTodo";
        // Input donde se realizará la búsqueda
        this.inputHeaderSearchable = $('#input-header-searchable-propiedades');
        // Icono de lupa para búsqueda
        this.iconHeaderSearchable = $('#input-header-searchable-icon');
        this.cerrarSelector = "cerrar-selector";
        // Icono para arrastrar una miniatura
        this.sortableMiniaturaIconClassName = 'js-component-sortable-miniatura';
        this.listaMiniatura = "lista-miniaturas";
        this.sortableCombo = 'js-component-sortable-valor';
        this.configuracionValores = "configuracionValores";
        this.tabsClasificacion = "tabClasificacion";
    },  
    
    /**
     * Configuración de eventos de elementos del Dom (Botones, Inputs...)     
     */
     configEvents: function (pParams) {
         const that = this;

         // Input para realizar búsquedas de páginas
         this.inputHeaderSearchable.off().on("keyup", function (event) {
             clearTimeout(that.timer);
             that.timer = setTimeout(function () {
                 that.handleHeaderSearch();
             }, 500);
         });
    
        // Botón para editar un componente dentro de una columna CMS        
         configEventByClassName(`${that.btnEditComponentFromCMSBuilderClassName}`, function(element){
            const $jqueryElement = $(element);
            $jqueryElement.off().on("click", function(){                                                 
                const editButton = $(this);                                
                that.handleLoadComponentModalForEdit(editButton);             
            });	 
         });

         configEventByClassName(`${that.botonMiniaturasClassName}`, function (element) {
             const $item = $(element);
             $item.off().on("click", function () {
                 $id = $item.data("miniatura");
                 if ($id != undefined) {
                     /*new Sortable(element, { animation: 150, ghostClass: 'blue-background-color' });*/
                     //setTimeout(setupBasicSortableList($id, this.sortableMiniaturaIconClassName), 5000);
                     setTimeout(new Sortable(document.getElementById($id), { animation: 150, ghostClass: 'blue-background-color' }), 5000);
                    
                 }
             });
             
         });

         configEventByClassName(`${that.cerrarSelector}`, function (element) {
             const $item = $(element);
             $item.off().on("click", function () {
                 $nombreDiv = $item.data("content");
                 $ventanaModal = $(`*[data-contentselector="${$nombreDiv}"]`);
                 $grafo = $(`select[name="${$nombreDiv}@grafoReferenciado"]`).val();
                 $entidad = $(`select[name="${$nombreDiv}@entidadReferenciada"]`).val();
                 $propiedad = $(`select[name="${$nombreDiv}@propiedadReferenciada"]`).val();
                 if ($grafo != undefined && $entidad != undefined && $propiedad != undefined) {
                     operativaConfiguracionOC.quitarAvisoSelector($nombreDiv);
                 }
                 $ventanaModal.modal('hide');
                 
             });
         });

         configEventByClassName(`${that.tabsClasificacion}`, function (element) {
             const $item = $(element);
             $item.off().on("click", function () {
                 $tabs = $(".tabClasificacion");
                 Array.from($tabs).forEach(x => $(x).removeClass("active"));
                 $item.addClass("active");
                 $tipo = $item.data("tipopropiedades");
                 $propiedades = $(".property-row");
                 $arrayPropiedades = Array.from($propiedades);
                 $arrayPropiedades.forEach(x => $(x).addClass("d-none"));
                 $simples = ["string", "boolean", "date", "numerical"];
                 $objeto = ["external", "auxiliar"];
                 switch ($tipo) {
                     case "todas":
                         $arrayPropiedades.forEach(element => $(element).removeClass("d-none"));
                         break;
                     case "simples":
                         $arrayPropiedades.forEach(function (element) {
                             $tipoPropiedad = $(element).data("tipopropiedad");
                             if ($simples.includes($tipoPropiedad)) {
                                 $(element).removeClass("d-none");
                             }
                         });
                         break;
                     case "objeto":
                         $arrayPropiedades.forEach(function (element) {
                             $tipoPropiedad = $(element).data("tipopropiedad");
                             if ($objeto.includes($tipoPropiedad)) {
                                 $(element).removeClass("d-none");
                             }
                         });
                         break;
                 }

             });
         });

         configEventByClassName(`${that.configuracionValores}`, function (element) {
             const $item = $(element);
             $item.off().on("click", function () {
                 $nombreDiv = $item.data("nombrediv");
                 $id = $(`*[data-contenedorvalores="${$nombreDiv}@contenedorValores"]`).attr("id");
                 new Sortable(document.getElementById($id), { animation: 150, ghostClass: 'blue-background-color' });

             });
         });

         configEventByClassName(`${that.propertyItemClassName}`, function (element) {
             const $item = $(element);
             $item.off().on("click", function () { 
                 //Obtiene el valor de data-propertyitem
                 $propertyitem = $item.data("propertyitem");
                 //Busca el elemento cuyo data-propertycontent coincide con el data-propertyitem
                 $find = `*[data-propertycontent="${$propertyitem}"]`;
                 //Obtiene el elemento 
                 $toScroll = $($find);
                 
                 
                 $(`.propertyContent`).addClass("d-none");
                 $(`.propertyContent`).siblings(`hr`).addClass("d-none");
                 $toScroll.removeClass("d-none");

                 //operativaConfiguracionOC.scrollToTop($toScroll, true, 120);                                
            });
         });

         configEventByClassName(`${that.expandir}`, function (element) {
             const $item = $(element);
             $item.off().on("click", function () {
                 $atributos = $item.parents(".component-header-wrap").siblings(".component-content");
                 if ($item.html() === "expand_more") {
                     $atributos.addClass("d-none");
                     $item.html("navigate_next");
                 }
                 else {
                     $atributos.removeClass("d-none");
                     $item.html("expand_more");
                 }
                 
                 
             });
         });

         configEventByClassName(`${that.desplegarTodo}`, function (element) {
             const $item = $(element);
             $item.off().on("click", function () {
                 if ($item.html() === "<small>Desplegar todo</small>") {
                     Array.from($(".component-content")).forEach((element) => $(element).removeClass("d-none"));
                     Array.from($(".expandir")).forEach((element) => $(element).html("expand_more"));
                     $item.html(`<small>Contraer todo</small>`);
                 }
                 else {
                     Array.from($(".component-content")).forEach((element) => $(element).addClass("d-none"));
                     Array.from($(".expandir")).forEach((element) => $(element).html("navigate_next"));
                     $item.html("<small>Desplegar todo</small>");
                 }


             });
         });

         configEventByClassName(`${that.valorDefecto}`, function (element) {
             const $item = $(element);
             $item.off().on("click", function () {
                 $div = $item.data("div");
                 $valor = $item.data("contienevalor");
                 $hackDefecto = $(`input[data-hackvalordefecto="${$div}@hackValorDefecto"]`);
                 
                 if ($item.checked) {
                     $hackDefecto.val($valor);
                     Array.from($(`.valorDefecto`)).forEach(x => x.checked = false);
                     Array.from($item).forEach(x => x.checked = true);
                 }
             });
         });

         configEventByClassName(`${that.eliminarValorCombo}`, function (element) {
             const $item = $(element);
             
             $item.off().on("click", function () {

                 $valor = $item.data("contienevalor");
                 $nombreDiv = $item.data("nombrediv");
                 $hack = $(`*[data-hackvalores="${$nombreDiv}@hackValores"]`);
                 $hack.val(`${$hack.val().replace($valor, "")}`);
                 $(`li[data-contienevalor="${$valor}"]`).remove();
             })
         });

         configEventByClassName(`${that.eliminarMiniatura}`, function (element) {
             const $item = $(element);
             $item.off().on("click", function () {
                 $parent = $item.parents(".miniatura-row");
                 $parent.remove();
             })
         });

         configEventByClassName(`${that.eliminarOpenSD}`, function (element) {
             const $item = $(element);
             $item.off().on("click", function () {
                 $nombreDiv = $item.data("contienediv");
                 $propiedadAncho = $(`select[name="${$nombreDiv}@opensdAncho"]`);
                 $propiedadAncho.val("").trigger("change");
                 $propiedadAlto = $(`select[name="${$nombreDiv}@opensdAlto"]`);
                 $propiedadAlto.val("").trigger("change");
             })
         });

         configEventByClassName(`${that.turnMultidiomaOnClassName}`, function (element) {
             const $item = $(element);
             $item.off().on("click", function () {
                 $('.multidioma').removeClass("d-none");
                 $('.checkMultidioma').attr("checked", "checked");
             })
         });

         configEventByClassName(`${that.turnMultidiomaOffClassName}`, function (element) {
             const $item = $(element);
             $item.off().on("click", function (e) {
                 $('.multidioma').addClass("d-none");
             })
         });

         configEventByClassName(`${that.checkMultidiomaClassName}`, function (element) {
             const $item = $(element);
             $item.off().on("click", function () {
                 $item.removeAttr("checked");
             })
         });

         configEventByClassName(`${that.btnConfigurarClassName}`, function (element) {
             const $item = $(element);
             $item.off().on("click", function () {
                 //Rellenar campos con valores por defecto
                 $div = $item.siblings(".selectorEntidad").data("contentselector");
                 $entidad = $item.siblings(".selectorEntidad").data("entitymodel");
                 $propiedad = $item.siblings(".selectorEntidad").data("propertymodel");
                 if ($div !== undefined && $entidad !== undefined && $propiedad !== undefined) {
                     operativaConfiguracionOC.cargarEntidades($div, true, $entidad, $propiedad);
                 }
                 //Abrir modal
                 $ventanaModal = $(`*[data-contentselector="${$item.data('content')}"]`);
                 $smartwizard = $ventanaModal.find('.smartwizard');
                 $smartwizard.smartWizard({
                     selected: 0, // Initial selected step, 0 = first step 
                     theme: 'square', // theme for the wizard, related css need to include for other than default theme 
                     justified: true, // Nav menu justification. true/false 
                     autoAdjustHeight: true, // Automatically adjust content height 
                     backButtonSupport: true, // Enable the back button support 
                     enableUrlHash: false, // Enable selection of the step based on url hash
                     enableFinishButton: true, //Enable finish button
                     transition: {
                         animation: 'fade', // Animation effect on navigation, none|fade|slideHorizontal|slideVertical|slideSwing|css(Animation CSS class also need to specify 
                         speed: '400', // Animation speed. Not used if animation is 'css' 
                         easing: '', // Animation easing. Not supported without a jQuery easing plugin. Not used if animation is 'css' 
                         prefixCss: '', // Only used if animation is 'css'. Animation CSS prefix 
                         fwdShowCss: '', // Only used if animation is 'css'. Step show Animation CSS on forward direction 
                         fwdHideCss: '', // Only used if animation is 'css'. Step hide Animation CSS on forward direction 
                         bckShowCss: '', // Only used if animation is 'css'. Step show Animation CSS on backward direction 
                         bckHideCss: '', // Only used if animation is 'css'. Step hide Animation CSS on backward direction 
                     },
                     toolbar: {
                        position: 'bottom', // none|top|bottom|both 
                        showNextButton: true, // show/hide a Next button 
                        showPreviousButton: true, // show/hide a Previous button 
                        extraHtml: '' // Extra html to show on toolbar 
                     },
                     toolbarSettings: { toolbarButtonPosition: 'none' },
                     anchor: {
                        enableNavigation: true, // Enable/Disable anchor navigation 
                        enableNavigationAlways: false, // Activates all anchors clickable always 
                        enableDoneState: true, // Add done state on visited steps 
                        markPreviousStepsAsDone: true, // When a step selected by url hash, all previous steps are marked done 
                        unDoneOnBackNavigation: false, // While navigate back, done state will be cleared 
                        enableDoneStateNavigation: true // Enable/Disable the done state navigation 
                     },
                     keyboard: {
                        keyNavigation: false, // Enable/Disable keyboard navigation(left and right keys are used if enabled) 
                        keyLeft: [37], // Left key code 
                        keyRight: [39] // Right key code 
                     },
                     lang: { // Language variables for button 
                         next: 'Siguiente',
                         previous: 'Anterior'
                     },
                     disabledSteps: [], // Array Steps disabled 
                     errorSteps: [], // Array Steps error 
                     warningSteps: [], // Array Steps warning 
                     hiddenSteps: [], // Hidden steps 
                     getContent: null // Callback function for content loading 
                 });
                 $ventanaModal.modal('show');
                 
             })
         });
         
         
    },
    
    /**
     * Método para realizar la búsqueda a en el headersearcher
     */
    handleHeaderSearch: function () {
        const that = this;
        let cadena = this.inputHeaderSearchable.val();
        
        // Eliminamos posibles tildes para búsqueda ampliada
        cadena = cadena.normalize('NFD').replace(/[\u0300-\u036f]/g, "").toLowerCase();

        // Cada una de las filas que muestran las propiedad y entidades
        const propertyEntityInfo = $(".propertyContent");
        Array.from($(".tituloEntidad")).forEach((element) => $(element).addClass("d-none"));
        // Buscar dentro de cada propiedad/entidad       
        $.each(propertyEntityInfo, function (index) {
            const propertyEntityInfoRow = $(this);
            $itemEntidadPadre = null;
            Array.from(propertyEntityInfoRow.parents()).forEach(function (x) {
                if ($(x).hasClass("tituloEntidad")) {
                    $itemEntidadPadre = x;
                }
            });
            // Seleccionamos el nombre
            const entityName = $(this).parents().find(".tituloEntidad").html().trim().normalize("NFD").replace(/[\u0300-\u036f]/g, "").toLowerCase();
            const infoName = $(this).find(".searchableName").html().trim().normalize("NFD").replace(/[\u0300-\u036f]/g, "").toLowerCase();
            if (infoName.includes(cadena) || entityName.includes(cadena)) {
                // Mostrar fila
                propertyEntityInfoRow.removeClass("d-none");
                if (propertyEntityInfoRow.find(".expandir").html() === "navigate_next") {
                    propertyEntityInfoRow.find(".expandir").click();
                }
                if ($itemEntidadPadre != null) {
                    $($itemEntidadPadre).removeClass("d-none");
                    if ($($itemEntidadPadre).find(".expandir").html() === "navigate_next") {
                        $($itemEntidadPadre).find(".expandir").click();
                    }
                }
                
            } else {
                // Ocultar fila resultado
                propertyEntityInfoRow.addClass("d-none");
            }
        });
    },

    checkSelectorEntidad: function (ventana) {
        alert("Has acabado la configuracion");
    },

    scrollToTop: function (
        pToElement,
        pBlinkElement,
        heightDistance,
        pCompletion = undefined,
        pDuration = 750
    ) {
        $([document.documentElement, document.body]).animate(
            {
                //Llevamos el elemento hasta la altura que queremos. Cuanto más grande sea heightDistance, más abajo estará
                scrollTop:
                    pToElement.offset().top - heightDistance,
            },
            {
                //Duracion del desplazamiento
                duration: pDuration,
                complete: function () {
                    // Blink element
                    pBlinkElement == true && pToElement.delay(100).fadeOut().fadeIn("slow");
                    pCompletion != undefined && pCompletion;
                },
            }
        );
    },

    showDiv: function (nombreDiv, selectedIndex) {
        $find = $(`*[data-contentcombo="${nombreDiv}@comboValores"]`);
        Array.from($find).forEach((x) => $(x).addClass('d-none'));
        $find2 = $(`*[data-contentcombo="${nombreDiv}@confImagen"]`);
        Array.from($find2).forEach((x) => $(x).addClass('d-none'));
        if (selectedIndex == 11) { //combo de valores
            $cadena = `${nombreDiv}@comboValores`;
            this.showDivEspecifico($cadena);
            $id = $(`*[data-contenedorvalores="${nombreDiv}@contenedorValores"]`).attr("id");
            setTimeout(new Sortable(document.getElementById($id), { animation: 150, ghostClass: 'blue-background-color' }), 5000);
        }
        if (selectedIndex == 10) { //imagen
            $cadena = `${nombreDiv}@confImagen`;
            this.showDivEspecifico($cadena);
        }
        
    },
    
    showDivEspecifico: function (nombreDiv) {
        $find = $(`*[data-contentcombo="${nombreDiv}"]`);
        Array.from($find).forEach((x) =>$(x).removeClass('d-none'));
    }, 

    addValue: function (nombreDiv) {
        //Coger el valor que se acaba de introducir
        $find = $(`*[data-contentvalor="${nombreDiv}@valor"]`);
        $valor = $find.val();
        if ($valor) {
            //Añadir al hack de valores del combo
            $hack = $(`*[data-hackvalores="${nombreDiv}@hackValores"]`);
            $hack.val(`${$hack.val()},${$valor}`);
            //Crear etiqueta con el valor
            $html = `<li class="component-wrap valor-combo-row" data-contienevalor="${$valor}">
                    <div class="component">
                        <div class="component-header-wrap">
                            <div class="component-header">
                                <div class="component-sortable js-component-sortable-valor">
                                    <span class="material-icons-outlined sortable-icon">drag_handle</span>
                                </div>
                                <div class="component-header-content">
                                    <div class="component-header-left">
                                        <div class="component-defecto-wrap">
                                            <span class="component-defecto"><input type="checkbox" class="valorDefecto" data-contienevalor="${$valor}" data-div="${nombreDiv}" /></span>
                          
                                        </div>
                                        <div class="component-valor-wrap">
                                            <span class="component-valor">${$valor}</span>
                                        </div>
                                        
                                        <div class="component-tipo-wrap">
                                            <span class="component-eliminar">
                                                <button type="button" class="eliminarMiniatura">
                                                    <span class="material-icons-outlined eliminarValor" data-contienevalor="${$valor}" data-nombrediv="${nombreDiv}">
                                                        delete
                                                    </span>
                                                </button>
                                                </button>
                                            </span>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </li>`;
            $contenedor = $(`*[data-contenedorvalores="${nombreDiv}@contenedorValores"]`);
            $contenedor.append($html);            
            $find.val("");
        }
        else {
            mostrarNotificacion("error", "Debes introducir un valor");
        }
            
    }, 

    addMini: function (nombreDiv) {
        $find = $(`*[data-description="${nombreDiv}@configImagen"]`);
        $tipo = $(`select[name="${nombreDiv}@tipoMiniatura"]`).val();
        $ancho = $(`*[name="${nombreDiv}@anchoMiniatura"]`).val();
        $alto = $(`*[name="${nombreDiv}@altoMiniatura"]`).val();
        $num = intGenerator();
        if ($ancho != "" && $alto != "") {
            $htmlMiniatura = `<li class="component-wrap miniatura-row">
                                    <div class="component">
                                        <div class="component-header-wrap">
                                            <div class="component-header">
                                            <div class="component-sortable js-component-sortable-faceta">
                                                    <span class="material-icons-outlined sortable-icon">drag_handle</span>
                                                </div>
                                                <div class="component-header-content">
                                                    <div class="component-header-left">
                                                        <div class="component-tipo-wrap">
                                                            <span class="component-tipo">${$tipo}</span>
                                                            <input type="hidden" name="${nombreDiv}@miniatura@minTipo@${$num}" value="${$tipo}">
                                                        </div>
                                                        <div class="component-alto-wrap">
                                                            <span class="component-alto">${$ancho}</span>
                                                            <input type="hidden" name="${nombreDiv}@miniatura@minAncho@${$num}" value="${$ancho}">
                                                        </div>
                                                        <div class="component-ancho-wrap">
                                                            <span class="component-ancho">${$alto}</span>
                                                            <input type="hidden" name="${nombreDiv}@miniatura@minAlto@${$num}" value="${$alto}">
                                                        </div>
                                                        <div class="component-tipo-wrap">
                                                            <span class="component-eliminar">
                                                                <button type="button" class="eliminarMiniatura"><span class="material-icons-outlined">
                                                                        delete
                                                                    </span></button>
                                                            </span>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </li>`;
            $find.append($htmlMiniatura);
            $(`*[name="${nombreDiv}@anchoMiniatura"]`).val("").trigger("change");
            $(`*[name="${nombreDiv}@altoMiniatura"]`).val("").trigger("change");
        }
        else {
            mostrarNotificacion("error", "Debe dar valor al ancho y al alto");

        }
    },

    addOpenSD: function (nombreDiv) {
        $find = $(`*[data-opensd="${nombreDiv}"]`);
        $find.removeClass("d-none");
    },

    checkDescripcion: function () {
        $selectTitulo = $(`select[name="titulo"]`);
        $titulo = $selectTitulo.val();
        $selectDescripcion = $(`select[name="descripcion"]`);
        $descripcion = $selectDescripcion.val();

        if ($descripcion === $titulo) {
            $selectDescripcion.val("").trigger("change");
            mostrarNotificacion("error", "La descripción no puede ser igual que el título");
        }
    },

    quitarAvisoSelector: function (nombreDiv) {
        $hide = $(`*[data-content="${nombreDiv}"]`);
        $hide.addClass("d-none");
    },

    cargarEntidades: function (nombreDiv, usarDefecto, entidadModelo, propiedadModelo) {
        $selectGrafo = $(`select[name="${nombreDiv}@grafoReferenciado"]`);
        $grafo = $selectGrafo.val();
        $rellenar = $(`*[data-entidades="${nombreDiv}@entidades"]`);
        $html = ``;

        // Creación del objeto a enviar a la petición
        const params = {
            pOntologia: $grafo
        }
        MostrarUpdateProgress();
        // Realizar la petición para obtener la vista 
        GnossPeticionAjax(
            "http://depuracion.net/comunidad/testing3/administrar-configuracion-oc/get-entities",
            params,
            true
        ).done(function (data) {
            if (usarDefecto && $rellenar.val() === null) {
                data.$values.forEach(function (x) {
                    if (x.startsWith(entidadModelo)) {
                        $html += `<option class="entidad" value="${x}" selected>${x}</option>`;
                    }
                    else {
                        $html += `<option class="entidad" value="${x}">${x}</option>`;
                    }
                });
                $rellenar.html($html);
            }
            if(!usarDefecto) {
                data.$values.forEach(x => $html += `<option class="entidad" value="${x}">${x}</option>`);
                $rellenar.html($html);
            }
            OcultarUpdateProgress();
            operativaConfiguracionOC.cargarPropiedades(nombreDiv, usarDefecto, entidadModelo, propiedadModelo);
        }).fail(function (data) {
            // Petición KO
            mostrarError("error", "Se ha producido un error al obtener las entidades");
        });
    },

    cargarPropiedades: function (nombreDiv, usarDefecto, entidadModelo, propiedadModelo) {
        $selectGrafo = $(`select[name="${nombreDiv}@grafoReferenciado"]`);
        $grafo = $selectGrafo.val();
        $selectEntidad = $(`select[name="${nombreDiv}@entidadReferenciada"]`);
        $entidad = $selectEntidad.val();
        $rellenar = $(`*[data-propiedades="${nombreDiv}@propiedades"]`);
        $rellenarmostrar = $(`*[data-propiedadesmostrar="${nombreDiv}@propiedadesmostrar"]`);
        $propiedadesMostrar = $rellenarmostrar.data("propslectura");
        $html = ``;
        $htmlmostrar = `<div class="propertyList">`;
        // Creación del objeto a enviar a la petición
        const params = {
            pOntologia: $grafo,
            pEntidad: $entidad
        }

        MostrarUpdateProgress();
        // Realizar la petición para obtener la vista 
        GnossPeticionAjax(
            "http://depuracion.net/comunidad/testing3/administrar-configuracion-oc/get-properties",
            params,
            true
        ).done(function (data) {
            if (usarDefecto && $rellenar.val() === null) {
                data.$values.forEach(function (x) {
                    if (x === propiedadModelo) {
                        $html += `<option class="propiedad" value="${x}" selected>${x}</option>`;
                    }
                    else {
                        $html += `<option class="propiedad" value="${x}">${x}</option>`;
                    }
                });
                data.$values.forEach(function (x) {
                    if ($propiedadesMostrar.includes(x)) {
                        $htmlmostrar += `<div class="form-check">
                        <input class="form-check-input" name="${nombreDiv}@propiedadesSeleccionadas" type="checkbox" value="${x}" checked>
                        <label class="form-check-label" for="${nombreDiv}@propiedadesSeleccionadas">${x}</label>
                    </div>`;
                    }
                    else {
                        $htmlmostrar += `<div class="form-check">
                        <input class="form-check-input" name="${nombreDiv}@propiedadesSeleccionadas" type="checkbox" value="${x}">
                        <label class="form-check-label" for="${nombreDiv}@propiedadesSeleccionadas">${x}</label>
                    </div>`;
                    }
                });
                $rellenar.html($html);
                $rellenarmostrar.html($htmlmostrar);
            }
            if (!usarDefecto) {
                data.$values.forEach(x => $html += `<option class="propiedad" value="${x}">${x}</option>`);
                data.$values.forEach(x => $htmlmostrar += `<div class="form-check">
                        <input class="form-check-input" name="${nombreDiv}@propiedadesSeleccionadas" type="checkbox" value="${x}">
                        <label class="form-check-label" for="${nombreDiv}@propiedadesSeleccionadas">${x}</label>
                    </div>`);
                $rellenar.html($html);
                $rellenarmostrar.html($htmlmostrar);
            }
            OcultarUpdateProgress();
        }).fail(function (data) {
            // Petición KO
            mostrarError("error", "Se ha producido un error al obtener las propiedades");
        });
    },

   


}