var navegacionSecciones = {
    init: function () {
        this.config();
        this.comportamiento();
    },
    config: function () {
        this.section = $(".section");
        this.panel_lateral = $(".panel-lateral");
        this.menu = this.panel_lateral.find("#menu");
    },
    comportamiento: function () {
        var that = this;
        this.section.off("click").on("click", function () {
            const data = $(this).attr("data-section");
            const parent_section = $(this).attr("data-parent-section");

            if (parent_section !== undefined) {
                that.menu
                    .find("." + data)
                    .get(0)
                    .click();
            } else {
                that.menu.find("a").removeClass("activo");
                that.section.remove();
                that.menu.find("." + data + "> a").addClass("activo");
                that.menu.find("#" + data).collapse("show");
                if (data == "comunidad") {
                    var comunidadHtml = that.pintarSeccionesComunidad();
                    $(".section-list-wrap").append($(comunidadHtml));
                }
                navegacionSecciones.init();
                MostrarUpdateProgressTime(1200);
            }
        });
    },
    pintarSeccionesComunidad: function () {
        return `<div class="section" data-parent-section="comunidad" data-section="info-comunidad">
                    <h2>Información general</h2>
                    <p>Lorem, ipsum dolor sit amet consectetur adipisicing elit. Libero ipsum velit dignissimos debitis natus reprehenderit nam ipsam. Nobis modi, quas blanditiis quisquam similique laborum nesciunt odit perspiciatis beatae repudiandae enim?</p>
                </div>
                <div class="section" data-parent-section="comunidad" data-section="permisos-comunidad">
                    <h2>Tipos de contenido / Permisos</h2>
                    <p>Lorem, ipsum dolor sit amet consectetur adipisicing elit. Libero ipsum velit dignissimos debitis natus reprehenderit nam ipsam. Nobis modi, quas blanditiis quisquam similique laborum nesciunt odit perspiciatis beatae repudiandae enim?</p>
                </div>
                <div class="section" data-parent-section="comunidad" data-section="categorias-comunidad">
                    <h2>Categorías</h2>
                    <p>Lorem, ipsum dolor sit amet consectetur adipisicing elit. Libero ipsum velit dignissimos debitis natus reprehenderit nam ipsam. Nobis modi, quas blanditiis quisquam similique laborum nesciunt odit perspiciatis beatae repudiandae enim?</p>
                </div>
                <div class="section" data-parent-section="comunidad" data-section="interaccion-comunidad">
                    <h2>Interacción social</h2>
                    <p>Lorem, ipsum dolor sit amet consectetur adipisicing elit. Libero ipsum velit dignissimos debitis natus reprehenderit nam ipsam. Nobis modi, quas blanditiis quisquam similique laborum nesciunt odit perspiciatis beatae repudiandae enim?</p>
                </div>
                <div class="section" data-parent-section="comunidad" data-section="miembros-comunidad">
                    <h2>Miembros</h2>
                    <p>Lorem, ipsum dolor sit amet consectetur adipisicing elit. Libero ipsum velit dignissimos debitis natus reprehenderit nam ipsam. Nobis modi, quas blanditiis quisquam similique laborum nesciunt odit perspiciatis beatae repudiandae enim?</p>
                </div>
                <div class="section" data-parent-section="comunidad" data-section="roles-comunidad">
                    <h2>Roles</h2>
                    <p>Lorem, ipsum dolor sit amet consectetur adipisicing elit. Libero ipsum velit dignissimos debitis natus reprehenderit nam ipsam. Nobis modi, quas blanditiis quisquam similique laborum nesciunt odit perspiciatis beatae repudiandae enim?</p>
                </div>
                <div class="section" data-parent-section="comunidad" data-section="certificacion-comunidad">
                    <h2>Niveles de certificación</h2>
                    <p>Lorem, ipsum dolor sit amet consectetur adipisicing elit. Libero ipsum velit dignissimos debitis natus reprehenderit nam ipsam. Nobis modi, quas blanditiis quisquam similique laborum nesciunt odit perspiciatis beatae repudiandae enim?</p>
                </div>
                <div class="section" data-parent-section="comunidad" data-section="redes-sociales-comunidad">
                    <h2>Integración de redes sociales</h2>
                    <p>Lorem, ipsum dolor sit amet consectetur adipisicing elit. Libero ipsum velit dignissimos debitis natus reprehenderit nam ipsam. Nobis modi, quas blanditiis quisquam similique laborum nesciunt odit perspiciatis beatae repudiandae enim?</p>
                </div>`;
    },
    pintarSeccionesEstructura: function () {
        return `<div class='section' data-parent-section='estructura' data-section='paginas-estructura'>
                    <h2>Páginas</h2>
                    <p>Lorem, ipsum dolor sit amet consectetur adipisicing elit. Libero ipsum velit dignissimos debitis natus reprehenderit nam ipsam. Nobis modi, quas blanditiis quisquam similique laborum nesciunt odit perspiciatis beatae repudiandae enim?</p>
                </div>
                <div class='section' data-parent-section='estructura' data-section='componentes-estructura'>
                    <h2>Componentes</h2>
                    <p>Lorem, ipsum dolor sit amet consectetur adipisicing elit. Libero ipsum velit dignissimos debitis natus reprehenderit nam ipsam. Nobis modi, quas blanditiis quisquam similique laborum nesciunt odit perspiciatis beatae repudiandae enim?</p>
                </div>
                <div class='section' data-parent-section='estructura' data-section='multimedia-estructura'>
                    <h2>Multimedia</h2>
                    <p>Lorem, ipsum dolor sit amet consectetur adipisicing elit. Libero ipsum velit dignissimos debitis natus reprehenderit nam ipsam. Nobis modi, quas blanditiis quisquam similique laborum nesciunt odit perspiciatis beatae repudiandae enim?</p>
                </div>
                <div class='section' data-parent-section='estructura' data-section='redirecciones-estructura'>
                    <h2>Redirecciones</h2>
                    <p>Lorem, ipsum dolor sit amet consectetur adipisicing elit. Libero ipsum velit dignissimos debitis natus reprehenderit nam ipsam. Nobis modi, quas blanditiis quisquam similique laborum nesciunt odit perspiciatis beatae repudiandae enim?</p>
                </div>
                <div class='section' data-parent-section='estructura' data-section='exportar-todo-estructura'>
                    <h2>Exportar todo...</h2>
                    <p>Lorem, ipsum dolor sit amet consectetur adipisicing elit. Libero ipsum velit dignissimos debitis natus reprehenderit nam ipsam. Nobis modi, quas blanditiis quisquam similique laborum nesciunt odit perspiciatis beatae repudiandae enim?</p>
                </div>`;
    },
};

var navegacionPestanasDropdown = {
    init: function () {
        $('.nav-tabs').on('shown.bs.tab', 'a', function (e) {
            // Activa y desactiva las pestañas con también opción en dropdown
            if (e.relatedTarget) {
                $('.item-dropdown').removeClass('active');
            } else {
                $('.item-dropdown').removeClass('active');
            }
            $(e.target).addClass('active');
        });
    },
};

var cambioVistaSeccion = {
    init: function () {
        this.config();
        this.comportamiento();
        return;
    },
    config: function () {
        this.body = $("body");
        return;
    },
    comportamiento: function () {
        var accionesSeccion = this.body.find(".acciones-seccion");
        var visualizacion = accionesSeccion.find(".visualizacion");
        var sectionList = this.body.find(".section-list");
        var dropdownMenu = visualizacion.find(".dropdown-menu");
        var dropdownToggle = visualizacion.find(".dropdown-toggle");
        var dropdownToggleIcon = dropdownToggle.find(".material-icons");
        var modosVisualizacion = dropdownMenu.find("a");

        modosVisualizacion.on('click', function (e) {
            e.preventDefault();
            var item = $(this);
            var clase = item.data('class-resource-list');

            modosVisualizacion.removeClass('activeView');
            item.addClass('activeView');

            if (clase != "") {
                var icon = item.find('.material-icons').text();
                dropdownToggleIcon.text(icon);
                sectionList.removeClass('compacView listView mosaicView');
                sectionList.addClass(clase);
            }
        });

        return;
    }
};

var categoriasEdition = {
    init: function () {
        this.categories.init();
    },
    categories: {
        init: function () {
            this.initCategories();
            this.addCategoryModalEvent();
        },
        initCategories: function () {
            const categories_lists = document.querySelectorAll(
                ".js-community-categories-list"
            );
            const categoriesOptinos = this.getAddedCategoryOptions();
            categories_lists.forEach((category_list) => {
                Sortable.create(category_list, categoriesOptinos);
            });
        },
        getAddedCategoryOptions: function () {
            var that = this;
            return {
                group: {
                    name: "js-community-categories-list",
                },
                sort: true,
                fallbackOnBody: true,
                swapThreshold: 0.65,
                animation: 150,
                handle: ".js-component-sortable-category",
                onAdd: function (evt) { },
                onChoose: function (evt) { },
                onUnChoose: function (evt) { },
            };
        },
        addCategoryModalEvent: function () {
            var modal_new_category = $("#modal-nueva-categoria");
            var button = modal_new_category.find(".panelBotonera .btn-primary");

            button.off("click").on("click", function () {
                categoriasEdition.categories.onCategoryCreated();
            });
        },
        onCategoryCreated: function () {
            var added_categories_list = $("#id-added-categories-list");

            // Hacer llamada ajax
            this.addNewCategory();
        },
        addNewCategory: function () {
            var list = $("#id-added-categories-list");
            var componentHtml = this.getCategoryTemplate();
            list.append($(componentHtml));
        },
        getCategoryTemplate: function () {
            return `<li class="component-wrap">
                        <div class="component type-category" data-component-type="category" data-id="">
                            <div class="component-header-wrap">
                                <div class="component-header">
                                    <div class="component-sortable js-component-sortable-category">
                                        <span class="material-icons-outlined sortable-icon">drag_handle</span>
                                    </div>
                                    <div class="component-header-content">
                                        <div class="component-header-left">
                                            <div class="component-name-wrap">
                                                <span class="component-icon"></span>
                                                <span class="component-name">Nuevo</span>
                                                <a class="action-collapse round-icon-button js-action-collapse" href="#">
                                                    <span class="material-icons">expand_more</span>
                                                </a>
                                            </div>
                                        </div>
                                        <div class="component-header-right">
                                            <div class="component-actions-wrap">
                                                <ul class="no-list-style component-actions">
                                                    <li>
                                                        <a class="action-edit round-icon-button js-action-edit-component">
                                                            <span class="material-icons">edit</span>
                                                        </a>
                                                    </li>
                                                    <li>
                                                        <a class="action-add round-icon-button js-action-add" href="#">
                                                            <span class="material-icons">create_new_folder</span>
                                                        </a>
                                                    </li>
                                                    <li>
                                                        <a class="action-delete round-icon-button js-action-delete" href="#">
                                                            <span class="material-icons">delete</span>
                                                        </a>
                                                    </li>
                                                </ul>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="component-content">
                                <ul class="no-list-style js-added-subcategories-list component-list"></ul>
                            </div>
                        </div>
                    </li>`;
        },
    },
};

var CategoriasManagement = {
    init: function () {
        accionesComponentesCategorias.init();
        ///////////////////////////////////categoriasEdition.init();
    }
};

var accionesComponentesCategorias = {
    init: function () {
        this.componentsSearch();
        // Esperar 0,5 segundos a si el usuario ha dejado de escribir para iniciar búsqueda        
        this.timeWaitingForUserToType = 500
    },
    componentsSearch: function () {
        var that = this;
        var $filters = $("#filtro-categorias");
        $filters.on("keyup", function () {
            clearTimeout(that.timer);
            $filter = $(this);
            that.timer = setTimeout(function () {
                const text = $filter.val().toLowerCase();
                that.search(text);
            }, that.timeWaitingForUserToType);
        });
    },
    search: function (text, $componentsToSearchIn = $('.js-community-categories-list')) {

        const $components = $componentsToSearchIn.find(".component-wrap");
        const that = this;

        // Eliminamos posibles tildes para búsqueda ampliada
        text = text.normalize("NFD").replace(/[\u0300-\u036f]/g, "");

        if (text.length == 0) {
            $components.show();
        } else {
            $components.each(function (index, element) {
                var $component = $(this);
                var name = $component
                    .find(
                        "> .component > .component-header-wrap .component-name"
                    )
                    .text()
                    .toLowerCase();
                // Eliminamos posibles tildes para búsqueda ampliada
                name = name.normalize('NFD').replace(/[\u0300-\u036f]/g, "");
                if (name.includes(text)) {
                    $component.show();
                    // Mostrar también a sus padres
                    const parents = $component.parents(".component-wrap");
                    parents.show();
                    // Mostrar también los paneles hijos
                    const arr = Array.from(parents);
                    parents.length > 0 && arr.forEach(item => {
                        $(item).find(".categoryChildrenPanel").removeClass("d-none");
                        // Cambiar el icono a desplegado
                        $(item).find(".showHide-icon").first().removeClass("collapsed").addClass("expanded").html("remove_circle_outline");
                    });
                } else {
                    $component.hide();
                }
                const $componentRecursive = $component.find(".js-community-categories-list");
                $componentRecursive.each(function () {
                    const parent = $(this).parent(".js-community-categories-list");
                    if (parent.length != 0) {
                        that.search(text, $(this), parent);
                    }
                });
            });
        }
    },
};

var certificadosEdition = {
    init: function () {
        this.certifications.initCertifications();
    },
    certifications: {
        initCertifications: function () {
            var added_certifications = document.getElementById(
                "id-added-certifications-list"
            );
            var added_certifications_options =
                this.getAddedCertificationOptions();
            Sortable.create(added_certifications, added_certifications_options);
        },
        getAddedCertificationOptions: function () {
            return {
                group: {
                    name: "id-added-certifications-list",
                },
                sort: true,
                dragoverBubble: true,
                handle: ".js-component-sortable-certification",
                onAdd: function (evt) { },
                onChoose: function (evt) { },
                onUnChoose: function (evt) { },
            };
        },
    },
};

var CertificationsManagement = {
    init: function () {
        certificadosEdition.init();
    },
};

var plegarSubFacetas = {
    init: function () {
        this.config();
        this.comportamiento();
        return;
    },
    config: function () {
        this.body = body;
        return;
    },
    comportamiento: function () {
        $('.desplegarSubFaceta .material-icons').unbind().click(function () {
            var padre = $(this).closest('a');
            var icono = $(this);
            var icono_texto = icono.text().trim();
            if (icono_texto == 'expand_more') {
                padre.removeClass('ocultarSubFaceta');
                icono.text('expand_less');
            } else {
                padre.addClass('ocultarSubFaceta');
                icono.text('expand_more');
            }
            if (icono_texto == 'add') {
                padre.removeClass('ocultarSubFaceta');
                icono.text('remove');
            } else {
                padre.addClass('ocultarSubFaceta');
                icono.text('add');
            }
            alturizarBodyTamanoFacetas.init();
            return false;
        });
    }
};


/**
 * Comportamiento Cargar facetas extraida de Front para Plegado/Desplegado de Facetas
 */
function comportamientoCargaFacetasComunidad() {
    alturizarBodyTamanoFacetas.init();
    plegarFacetasCabecera.init();
    plegarSubFacetas.init();
    facetasVerMasVerTodos.init();
    // Funcionamiento de botones de navegación de Facetas en Modal (Siguente, Desplegar...)
    comportamientoFacetasPopUp.init();
};


/**
 * Comportamiento para plegar Facetas extraida de Front
 */
var plegarFacetasCabecera = {
    init: function () {
        this.config();
        this.comportamiento();
        return;
    },
    config: function () {
        this.body = body;
        return;
    },
    comportamiento: function () {
        var that = this;
        this.facetas = $("body").find('#panFacetas');
        var facetasTitle = this.facetas.find('.faceta-title');

        facetasTitle.off('click').on('click', function (e) {
            var title = $(this);
            var target = $(e.target);
            var box = title.parents('.box');

            if (target.hasClass('search-icon')) {
                e.preventDefault();
                e.stopPropagation();
            } else {
                that.mostrarOcultarFaceta(box);
            }

            alturizarBodyTamanoFacetas.init();
        });
    },
    mostrarOcultarFaceta: function (box) {
        box.toggleClass('plegado');
    },
    mostrarFaceta: function (box) {
        box.removeClass('plegado');
    },
};

/**
 * Comportamiento para plegar subFacetas extraida de Front
 */
var plegarSubFacetas = {
    init: function () {
        this.config();
        this.comportamiento();
        return;
    },
    config: function () {
        this.body = body;
        return;
    },
    comportamiento: function () {
        $('.desplegarSubFaceta .material-icons').unbind().click(function () {
            var padre = $(this).closest('a');
            var icono = $(this);
            var icono_texto = icono.text().trim();
            if (icono_texto == 'expand_more') {
                padre.removeClass('ocultarSubFaceta');
                icono.text('expand_less');
            } else {
                padre.addClass('ocultarSubFaceta');
                icono.text('expand_more');
            }
            alturizarBodyTamanoFacetas.init();
            return false;
        });
    }
};

/**
 * Comportamiento para Ver más / Ver todos en Facetas extraida de Front
 */
var facetasVerMasVerTodos = {
    init: function () {
        this.config();
        this.comportamientoVerMas();
        this.comportamientoVerTodos();
        return;
    },
    config: function () {
        this.body = body;
        return;
    },
    comportamientoVerTodos: function () { },
    comportamientoVerMas: function () {

        $('.moreResults .ver-mas').off('click').on('click', function () {
            var facetaContainer = $(this).closest('.facetedSearch');
            facetaContainer.find('ul.listadoFacetas > .ocultar').show(200);
            facetaContainer.find('.ver-mas').css('display', 'none');
            facetaContainer.find('.ver-menos').css('display', 'flex');
            alturizarBodyTamanoFacetas.init();
        });

        $('.moreResults .ver-menos').off('click').on('click', function () {
            var facetaContainer = $(this).closest('.facetedSearch');
            facetaContainer.find('ul.listadoFacetas > .ocultar').hide(200);
            facetaContainer.find('.ver-menos').css('display', 'none');
            facetaContainer.find('.ver-mas').css('display', 'flex');
            alturizarBodyTamanoFacetas.init();
            return false;
        });
    }
};

var alturizarBodyTamanoFacetas = {
    init: function () {
        this.config();
        this.calcularAltoFacetas();
        return;
    },
    config: function () {
        this.body = $("body");
        this.panFacetas = this.body.find('#panFacetas');
        this.container = this.body.find('main[role="main"] > .container');
        return;
    },
    calcularAltoFacetas: function () {
        var altoFacetas = this.panFacetas.height();

        this.container.css({
            'min-height': altoFacetas
        });

        return;
    }
};

/**
 * Comportamiento de facetas pop up para que se carguen una vez se pulse en el botón de "Ver mÃ¡s".
 * Se harÃ¡ la llamada para la obtención de Facetas y se muestran en un panel modal.  
 * Extraido del Front
 * */
const comportamientoFacetasPopUp = {
    init: function () {
        // Objetivo Observable
        const that = this;

        if ($("#panFacetas").length > 0 || $("#divFac").length > 0) {
            const target = $("#panFacetas").length == 0 ? $("#divFac")[0] : $("#panFacetas")[0];
            // Teclas que se ignorarán si se pulsan en el input para que no dispare la búsqueda (Flechas, Supr, Windows, Ctrol, Alt, Bloq. Mayus, Inicio, Alt, Escape)
            this.ignoreKeysToBuscador = [37, 38, 39, 40, 46, 91, 17, 18, 20, 36, 18, 27];
            this.timeWaitingForUserToType = 400; // Esperar 1 segundos a si el usuario ha dejado de escribir para iniciar búsqueda

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
        //1Âº Nombre de la faceta
        //2Âº Titulo del buscador
        //3Âº True para ordenar por orden alfabÃ©tico, False para utilizar el orden por defecto
        var that = this;

        /* Esquema que tendrÃ¡
        this.facetasConPopUp = [
            [
                "sioc_t:Tag",
                "Busca por TAGS",
                true,
            ], //Tags            
        ];*/

        // Array de Facetas que tendrÃ¡ visualización con PopUp
        this.facetasConPopUp = [];

        // Recoger todos posibles botones de "Ver mÃ¡s" para construir un array de facetasConPopUp                
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
            // Construir el objeto Array de la faceta para luego añadirlo a facetasConPopUp
            const facetaArray = [
                $(this).data('facetkey'), // Faceta para bÃºsqueda
                $(this).data('facetname'), // TÃ­tulo de la faceta
                true,                     // Ordenar por orden alfabÃ©tico,                
            ];
            that.facetasConPopUp.push(facetaArray);
        });

        for (i = 0; i < this.facetasConPopUp.length; i++) {
            // Faceta de tipo de bÃºsqueda
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
                        ObtenerHash2().replace(/&/g, "|").replace("#", ""),
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


        // Petición al servicio para obtención de Facetas                
        $.post(obtenerUrl($('input.inpt_UrlServicioFacetas').val()) + "/" + metodo, params, function (data) {
            var htmlRespuesta = $("<div>").html(data);
            that.arrayTotales = new Array($(htmlRespuesta).find(".faceta").length);
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
            // Header navegación de resultados de facetas
            const actionButtonsResultados = `<div class="action-buttons-resultados">
                                                <ul class="no-list-style">
                                                    <li class="js-anterior-facetas-modal">
                                                        <span class="material-icons">navigate_before</span>
                                                        <span class="texto">${facetas.anteriores}</span>
                                                    </li>
                                                    <li class="js-siguiente-facetas-modal">
                                                        <span class="texto">${facetas.siguientes}</span>
                                                        <span class="material-icons">navigate_next</span>
                                                    </li>
                                                </ul>
                                            </div>`;

            // Añadir al header de navegación de facetas (Anterior)
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
        var ul = $(`<ul class="listadoFacetas">`);

        this.fin = true;

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
 * Comportamiento del panel lateral para dispositivos móviles
 */
const comportamientoPanelLateralMobile = {

    // Inicializar comportamiento
    init: function () {
        this.config();
        this.configEvents();
        return;
    },

    config: function () {
        // Panel lateral a ocultar o cerrar
        this.btnTriggerPanelLateral = $("#panel-lateral-trigger");
        // Botón del menú para mostrar/ocultar el panel
        this.panelLateral = $("#panel-lateral");
        // Capa overlay para ocultar el resto de contenido cuando el panel móvil esté visible
        this.mobileOverlay = $("#mobileOverlay");
        return;
    },

    /**
     * Configurar eventos de elementos del Dom
     */
    configEvents: function () {
        const that = this;

        // Click en el botón para ocultar o mostrar el panel del usuario
        that.btnTriggerPanelLateral.on("click", function () {
            that.showHidePanelLateral();
        });

        // Click en la capa overlay para ocultar panel del usuario en modo móvil.
        that.mobileOverlay.on("click", function () {
            that.showHidePanelLateral();
        })
    },

    /**
     * Método para mostrar u ocultar el panel lateral en dispositivos móviles/tablets
     */
    showHidePanelLateral: function () {
        const that = this;
        // Mostrar/Ocultar el panel
        that.panelLateral.toggleClass('show');
        // Cambiar el icono dependiendo del estado del panel lateral
        that.panelLateral.hasClass('show') ? that.btnTriggerPanelLateral.html("close") : that.btnTriggerPanelLateral.html("menu");
        that.mobileOverlay.toggleClass('show');
    },
};

/*
* Comportamiento de ocultación del panel lateral del usuario.
*/
const comportamientoOcultarPanelLateral = {

    /**
     * Configurar lanzamiento del comportamiento
     */
    init: function () {
        this.config();
        this.configEvents();
        this.initialState();
    },

    /**
     * Configurar elementos del DOM y variables de uso
     */
    config: function () {
        // Elementos del Dom
        // Panel lateral del usuario
        this.panelLateral = $('#panel-lateral');
        // Botón para mostrar/ocultar el panel lateral del usuario
        this.btnTogglePanelLateral = $('#btnTogglePanelLateral');
        // Título de la comunidad del panel lateral
        this.headerTitulo = $('#header-title');
        // Arrow down al lado del título
        this.headerArrowdown = $('#header-title-arrow-down');
        // Nombre de clase del botón para indicarle el estado
        this.panelLateralOpenClassName = 'hidePanel';
        // Enlaces de las categorías de Devtools para navegación
        this.navigationSections = $('.parent-navigation');
    },

    /**
     * Configurar eventos de los elementos del DOM
     */
    configEvents: function () {
        const that = this;

        // Click en el botón para cerrar u ocultar el panel lateral del usuario
        that.btnTogglePanelLateral.on("click", function () {
            if ($(this).hasClass(that.panelLateralOpenClassName)) {
                // Ocultarlo
                that.handleToggleOpenClosePanelLateral(false);
                // Ocultar las subsecciones de navegación
                that.handleCollapseSubsecciones();
            } else {
                // Mostrarlo
                that.handleToggleOpenClosePanelLateral(true);
                that.handleCollapseSubsecciones(true);
            }
        });

        // Cada uno de los links que abren el menú de navegación
        this.navigationSections.on("click", function () {
            // Mostrar menú si no está cerrado
            if (!that.btnTogglePanelLateral.hasClass(that.panelLateralOpenClassName)) {
                that.handleToggleOpenClosePanelLateral(true);
            }
        });
    },

    /**
     * Configurar el estado inicial del panel lateral del usuario de las Devtools
     */
    initialState: function () {
        this.btnTogglePanelLateral.addClass(this.panelLateralOpenClassName);
    },

    /**
     * Colapsar o cerrar las subsecciones de navegación si se pulsa sobre el botón de "Cerrar panelLateral"
     */
    handleCollapseSubsecciones: function (show = false) {
        const that = this;

        this.navigationSections.each(function () {
            const menuParent = $(this);
            // Comprobar si está o no expandido el panel
            const isExpanded = menuParent.attr("aria-expanded");
            // Panel correspondiente a la navegación
            const panelCollapsable = menuParent.siblings("div");
            // Ocultar el panel
            if (isExpanded == "true") {
                panelCollapsable.removeClass("show");
            }
            // Mostrar el panel que estaba desplegado
            if (show == true && menuParent.hasClass("activo")) {
                panelCollapsable.addClass("show");
            }
        });

    },

    /**
     * Método para abrir/cerrar el menú lateral del usuario
     * @param {*} pShow: Indicará si se desea cerrar o no el panel lateral del usuario
     */
    handleToggleOpenClosePanelLateral: function (pShow) {

        if (pShow == true) {
            // Ensanchar el panelLateral
            this.panelLateral.removeClass(this.panelLateralOpenClassName);
            // No mostrar el header título ni su arrow
            this.headerTitulo.removeClass("d-none");
            this.headerArrowdown.removeClass("d-none");
            // Cambiar el icono del botón y añadirle clase de "open"
            this.btnTogglePanelLateral.html("chevron_left");
        } else {
            // Mostrar el header título
            this.headerTitulo.addClass("d-none");
            this.headerArrowdown.addClass("d-none");
            // Cerrar el panelLateral
            this.panelLateral.addClass(this.panelLateralOpenClassName);
            // Cambiar el icono del botón
            this.btnTogglePanelLateral.html("chevron_right");
        }
        // Añadir / Eliminar la clase al botón para indicar si está abierto o no
        this.btnTogglePanelLateral.toggleClass(this.panelLateralOpenClassName);
    },
};

/**
 * Operativa para buscar palabras claves cuando se utilice el buscador del navegador lateral del usuario de las Devtools
 * Dependiendo de las palabras clave a buscar, se mostrarán u ocultarán los menús donde estas opciones estén en el menú correspondiente
 */
const comportamientoHeaderSearchable = {

    /**
     * Configurar lanzamiento del comportamiento
     */
    init: function () {
        this.config();
        this.configEvents();
    },

    /**
     * Configurar elementos del DOM y variables de uso
     */
    config: function () {
        // Elementos del Dom
        // Input donde se realizará la búsqueda
        this.inputHeaderSearchable = $('#input-header-searchable');
        // Icono de lupa para búsqueda
        this.iconHeaderSearchable = $('#input-header-searchable-icon');
        // Bloques / Secciones con el subbloque colapsado 
        this.navSections = $(".group-collapse");
        // Subsecciones que pueden colapsarse o no (show)
        this.navSubsections = $(".group-collapse .collapse");
        // Links de las subsecciones. Contendrá la propiedad data-searchable con las palabras clave
        this.navLinkSubsections = $(".group-collapse .collapse li a");
        // Loading del buscador header en el menú del usuario
        this.searcherLoading = $("#header-searcher-loading");

        // Variables de uso
        // Esperar 0,5 segundos a si el usuario ha dejado de escribir para iniciar búsqueda
        this.timeWaitingForUserToType = 500;
        // Configurar que por defecto no se está realizando búsqueda alguna
        this.isSearching = false;
        // Configurar si el elemento se ha encontrado
        this.keywordFound = false;
        this.foundInSubsection = false;
        // Indice de la sección que se está analizando para encontrar la keyword
        this.sectionIndex = undefined;

    },

    /**
     * Configurar eventos de los elementos del DOM
     */
    configEvents: function () {
        const that = this;

        // Input donde se realizará la búsqueda
        this.inputHeaderSearchable.on("keyup", function (event) {
            clearTimeout(that.timer);
            that.timer = setTimeout(function () {
                if (!that.isSearching) {
                    that.isSearching = true;
                    that.keywordFound = false;
                    // Activo el loading
                    that.searcherLoading.removeClass("d-none");
                    that.handleHeaderSearch(function () {
                        that.isSearching = false
                        setTimeout(function () {
                            that.searcherLoading.addClass("d-none");
                        }, 300);
                    });
                }
            }, that.timeWaitingForUserToType);
        });
    },

    /**
     * Método para realizar la búsqueda a en el headersearcher
     */
    handleHeaderSearch: function (completion) {
        const that = this;
        let cadena = this.inputHeaderSearchable.val();
        // Eliminamos posibles tildes para búsqueda ampliada
        cadena = cadena.normalize('NFD').replace(/[\u0300-\u036f]/g, "").toLowerCase();

        // Buscar dentro de cada bloque        
        $.each(this.navSubsections, function (index) {
            const sectionLinks = $(this).find("a");
            comportamientoHeaderSearchable.sectionIndex = index;

            // Recorrer los links de cada sección para realizar la búsqueda de palabras clave
            $.each(sectionLinks, function () {
                // Keywords de la sección 
                if ($(this).data("searchable") != undefined) {
                    // Sección padre donde se encontraría este enlace
                    let section = undefined;
                    const keyWords = $(this).data("searchable").normalize('NFD').replace(/[\u0300-\u036f]/g, "").toLowerCase();
                    // Array de los keywords de la sección                
                    const arrayKeyWords = keyWords.split(',');
                    // Buscar si el texto se encuentra en las keywords
                    arrayKeyWords.find(element => {
                        if (element.includes(cadena) && cadena != "") {
                            comportamientoHeaderSearchable.foundInSubsection = true;
                        }
                    });

                    // La búsqueda se encuentra en esta sección
                    if (comportamientoHeaderSearchable.foundInSubsection == true) {
                        // Mostrar el item si estuviera oculto
                        $(this).parent().removeClass("d-none");
                        // Cambiar el color a primario para destacar del resto de coincidencias encontradas
                        $(this).css("color", "var(--c-primario)");
                        // Indicar que se ha encontrado la keyWord en la sección analizada.
                        comportamientoHeaderSearchable.keywordFound = true
                    } else if (cadena == "") {
                        // Se ha eliminado la búsqueda -> Dejarlo por defecto
                        $(this).parent().removeClass("d-none");
                        $(this).css("color", "");
                        $(this).attr("href", refineURL($(this).attr("href")));
                    } else {
                        $(this).parent().addClass("d-none");
                        // Dejar el color por defecto
                        $(this).css("color", "");
                    }
                    // Restablecemos la búsqueda por subSección
                    comportamientoHeaderSearchable.foundInSubsection = false;
                }
            });
            const section = $(comportamientoHeaderSearchable.navSubsections[comportamientoHeaderSearchable.sectionIndex]);
            // Mostrar u ocultar la sección padre en caso de encontrarse la keyword
            comportamientoHeaderSearchable.handleShowHideSection(section, comportamientoHeaderSearchable.keywordFound);
            // Restablecemos la búsqueda para las demás secciones            
            comportamientoHeaderSearchable.keywordFound = false;
        });

        // Devolver comportamiento        
        if (completion != undefined) {
            completion();
        }
    },

    /**
     * Mostrar u ocultar una sección de la navegación izquierda dependiendo de la búsqueda realizada
     */
    handleShowHideSection: function (section, found) {
        // Link padre del menú de navegación del bloque
        const parentLink = section.siblings("a");

        if (found == true) {
            // Mostrar la sección                        
            section.addClass("show");
            // Establecer aria-expanded en el link
            parentLink.attr("aria-expanded", "true");
        } else {
            // Ocultar la sección la sección                        
            section.removeClass("show");
            // Establecer aria-expanded en el link
            parentLink.attr("aria-expanded", "false");
        }
    },
};

/**
 * Operativa de mostrar el icono correspondiente según el estado de la sección (Collapse / No collapse)
 */
const comportamientoSidebarDropdownSections = {
    /**
     * Configurar lanzamiento del comportamiento
     */
    init: function () {
        this.config();
        this.configEvents();
    },

    /**
     * Configurar elementos del DOM y variables de uso
     */
    config: function () {
        // Elementos del Dom
        // Iconos de collapse la sección del panel lateral de navegación
        this.iconCollapse = $(".icon-collapse");
        // Nombre del icono para mostrar el panel colapsado
        this.iconCollapseName = "keyboard_arrow_down";
        // Nombre del icono para mostrar el panel NO colapsado
        this.iconNoCollapseName = "keyboard_arrow_up";
        // Secciones de navegación del menú lateral 
        this.parentNavigation = $(".parent-navigation");
    },

    /**
     * Configurar eventos de los elementos del DOM
     */
    configEvents: function () {
        const that = this;

        // Click en las secciones laterales de navegación   
        this.parentNavigation.on("click", function () {
            const collapseIconArrow = $(this).find("icon-collapse");
            // Indica si el panel está expandido o desplegado
            const isExpanded = ($(this).attr("aria-expanded"));
            // Cambiar icono según estado
            if (stringToBoolean(isExpanded) == true) {
                $(this).find(that.iconCollapse).html(that.iconCollapseName);
            } else {
                $(this).find(that.iconCollapse).html(that.iconNoCollapseName);
            }
        });
    },
};

/**
 * Gestión/Edición de páginas desde Listado de páginas de la Comunidad
 */
var paginasEdition = {
    init: function () {
        this.initPages();
        this.addPageModalEvent()
    },
    initPages: function () {
        const pages_lists = document.querySelectorAll(
            '.js-community-pages-list'
        );
        const pagesOptions = this.getAddedPageOptions();
        pages_lists.forEach((page_list) => {
            Sortable.create(page_list, pagesOptions);
        });
    },
    getAddedPageOptions: function () {
        return {
            group: {
                name: 'js-community-pages-list',
            },
            sort: true,
            fallbackOnBody: true,
            swapThreshold: 0.65,
            animation: 150,
            handle: '.js-component-sortable-page',
            onAdd: function (evt) { },
            onChoose: function (evt) { },
            onUnChoose: function (evt) { },
        };
    },
    addPageModalEvent: function () {
        var modal_new_page = $('#modal-configuracion-pagina');
        var button = modal_new_page.find('.panelBotonera .btn-primary');

        button.off('click').on('click', function () {
            paginasEdition.onPageCreated();
        });
    },
    onPageCreated: function () {
        var added_pages_list = $('#id-added-pages-list');

        // Hacer llamada ajax
        this.addNewPage();
        mostrarNotificacion(
            'success',
            'Has [ certificado | enviado | etiquetado | guardado ] el recurso'
        );
    },
    addNewPage: function () {
        var list = $('#id-added-pages-list');
        var componentHtml = this.getPageTemplate();
        list.append($(componentHtml));
    },
    getPageTemplate: function (titulo) {
        return `<li class='component-wrap'>
                    <div class='component ' data-id=''>
                        <div class='component-header-wrap'>
                            <div class='component-header'>
                                <div class='component-sortable js-component-sortable-page'>
                                    <span class='material-icons-outlined sortable-icon'>drag_handle</span>
                                </div>
                                <div class='component-header-content'>
                                    <div class='component-header-left'>
                                        <div class='component-name-wrap'>
                                            <span class='material-icons'>public</span>
                                            <span class='component-name'>Nuevo</span>
                                        </div>
                                        <div class='component-url-wrap'>
                                            <span class='component-url'>/</span>
                                        </div>
                                        <div class='component-tipo-wrap'>
                                            <span class='component-tipo'>Home</span>
                                        </div>
                                        <div class='component-estado-wrap'>
                                            <span class='component-estado'>Publicada</span>
                                        </div>
                                        <div class='component-visible-wrap'>
                                            <span class='component-visible'>Sí</span>
                                        </div>
                                        <div class='component-fecha-wrap'>
                                            <span class='component-fecha'>01 / 01 / 2022</span>
                                        </div>
                                    </div>
                                    <div class='component-header-right'>
                                        <div class='component-actions-wrap'>
                                            <ul class='no-list-style component-actions'>
                                                <li>
                                                    <a class='action-edit round-icon-button js-action-edit-component'>
                                                        <span class='material-icons'>edit</span>
                                                    </a>
                                                </li>
                                                <li>
                                                    <a class='action-delete round-icon-button js-action-delete' href='#'>
                                                        <span class='material-icons'>delete</span>
                                                    </a>
                                                </li>
                                            </ul>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class='component-content'>
                            <ul class='js-community-pages-list component-list no-list-style'></ul>
                        </div>
                    </div>
                </li>`;
    },
};

var PagesManagement = {
    init: function () {
        paginasEdition.init();
    }
}

/* Operativa para gestión de páginas del CMS - CMS Builder */
var PageBuilderEdition = {
    init: function () {
        this.rowsEdition.initRows();
        this.columnsEdition.initColumns();
        this.componentsEdition.initComponents();
        this.plantillasPredeterminadas.init();
    },
    rowsEdition: {
        initRows: function () {
            this.initDefaultRowsList();
            this.initRowsList();
        },
        initRowsList: function () {
            var cmsrow_list = document.getElementById("cmsrow-list");
            var cmsrow_list_options = this.getRowsListOptions();

            Sortable.create(cmsrow_list, cmsrow_list_options);

            this.addRowButtonEvent();
            this.updateAttributesDisplay();
        },
        initDefaultRowsList: function () {
            if (typeof this.defaultRowsSortable !== "undefined") return;

            var defaultRowsList = document.querySelector(".js-default-rows-list");
            var default_options = this.getDefaultRowsListOptions();

            this.defaultRowsSortable = Sortable.create(defaultRowsList, default_options);
        },
        getRowsListOptions: function () {
            return {
                group: {
                    name: "cmsrow-list",
                    put: ["js-default-rows-list"],
                },
                sort: true,
                dragoverBubble: true,
                handle: ".js-action-handle-row",
                onAdd: function (evt) {
                    PageBuilderEdition.rowsEdition.onDefaultRowAdded(evt);
                },
                onSort: function (evt) {
                    accionesFilasPageBuilder.countRows();
                },
            };
        },
        getDefaultRowsListOptions: function () {
            return {
                group: {
                    name: "js-default-rows-list",
                    pull: "clone",
                },
                sort: false,
                dragoverBubble: true,
                handle: ".handler",
            };
        },
        addRowButtonEvent: function () {
            var button = $(".builder-editor .botonera .btn");
            button.off("click").on("click", function (evt) {
                PageBuilderEdition.rowsEdition.onRowCreated(evt);
            });
        },
        onRowCreated: function () {
            var cmsrow_list = $("#cmsrow-list");
            var rowTemplate = this.getRowHtmlTemplate();
            cmsrow_list.append($(rowTemplate));
            // Indicar que hay cambios en el CMS para posibilitar el guardado
            operativaGestionCMSBuilderGuardadoSeguridad.setIsNecessarySaveCmsHtmlContentInPageBuider(true);
            PageBuilderManagement.init();
        },
        getRowHtmlTemplate: function () {
            // Crear un Guid para la nueva row añadida
            const rowId = guidGenerator();
            return `<li id="${rowId}" class="cmsrow js-cmsrow">
                        ${this.getRowWrapTemplate()}
                    </li>`;
        },
        getRowWrapTemplate: function () {
            var cmsrow = $(".js-cmsrow").length;
            return `<div class="cmsrow-wrap">
                        <div class="cmsrow-header">
                            <div class="name">
                                Fila
                                <span class="number">
                                    ${cmsrow + 1}
                                </span>
                                <span class="attributes-display"></span>
                                </div>
                            <div class="js-cmsrow-actions cmsrow-actions">
                                <ul class="no-list-style">
                                    <li>
                                        <a class="action-clone round-icon-button js-action-clone">
                                            <span class="material-icons">content_copy</span>
                                        </a>
                                    </li>
                                    <li>
                                        <a class="action-edit round-icon-button js-action-edit-row" >
                                            <span class="material-icons">edit</span>
                                        </a>
                                    </li>
                                    <li>
                                        <a class="action-delete round-icon-button js-action-delete">
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
                        <div class="cmsrow-content">
                            <ul class="columns-list no-list-style"></ul>
                        </div>
                    </div>`;
        },
        updateAttributesDisplay: function () {
            $(".js-cmsrow").each(function (index, element) {
                const $row = $(this);
                const $display = $row.find(".attributes-display");
                let attributes = $row.attr("data-row-attributes");

                $display.text("");

                if (!attributes) return;

                attributes = attributes.split(",");

                let attributesText = "[ ";

                for (let index = 0; index < attributes.length; index++) {
                    if (index !== 0) {
                        attributesText += ", ";
                    }
                    const attribute = attributes[index];
                    attributesText += attribute; // attribute.split("|")[1];
                }
                $display.text(attributesText + " ]");
            });
        },
        onDefaultRowAdded: function (event) {
            var $item = $(event.item);
            $item.removeClass();
            $item.addClass("cmsrow").addClass("js-cmsrow");
            $item.html(this.getRowWrapTemplate());
            // Crear un Guid para la nueva row añadida
            const rowId = guidGenerator();
            $item.attr("id", rowId);

            this.buildPredefinedColumns($item);

            PageBuilderManagement.init();
            // Indicar que hay cambios en el CMS para posibilitar el guardado
            operativaGestionCMSBuilderGuardadoSeguridad.setIsNecessarySaveCmsHtmlContentInPageBuider(true);

            accionesFilasPageBuilder.countRows();
        },
        buildPredefinedColumns: function ($item) {
            dataColumns = $item.attr("data-columns");

            let columnsPercentage = [];

            switch (dataColumns) {
                case "12":
                    columnsPercentage = ["100"];
                    break;
                case "6+6":
                    columnsPercentage = ["50", "50"];
                    break;
                case "8+4":
                    columnsPercentage = ["66.66", "33.33"];
                    break;
                case "4+4+4":
                    columnsPercentage = ["33.33", "33.33", "33.33"];
                    break;
                case "3+3+3+3":
                    columnsPercentage = ["25", "25", "25", "25"];
                    break;
                case "6+3+3":
                    columnsPercentage = ["50", "25", "25"];
                    break;
                case "9+3":
                    columnsPercentage = ["75", "25"];
                    break;
                default:
                    break;
            }

            columnsPercentage.forEach((percent) => {
                const column = $(PageBuilderEdition.columnsEdition.getColumnHtmlTemplate(percent));
                column.css("width", PageBuilderEdition.columnsEdition.getColumnWidthByPercent(percent));
                column.attr("data-percent", percent);
                $item.find(".columns-list").append(column);
            });
        },
    },
    columnsEdition: {
        initColumns: function () {
            this.initDefaultColumnsList();
            this.initColumnsList();
        },
        initDefaultColumnsList: function () {
            if (typeof this.defaultColumnsSortable !== "undefined") return;

            var defaultColumnsList = document.querySelector(".js-default-columns-list");
            var default_options = this.getDefaultColumnsListOptions();

            this.defaultColumnsSortable = Sortable.create(defaultColumnsList, default_options);
        },
        initColumnsList: function () {
            var columns_list = document.getElementsByClassName("columns-list");
            var columns_list_options = this.getColumnsListOptions();

            for (var i = 0; i < columns_list.length; i++) {
                Sortable.create(columns_list[i], columns_list_options);
            }
        },
        getColumnsListOptions: function () {
            return {
                group: {
                    name: "columns-list",
                    put: ["js-default-columns-list"],
                },
                sort: true,
                dragoverBubble: true,
                handle: ".js-action-handle-row",
                onAdd: function (evt) {
                    var item = $(evt.item);
                    if (!item.hasClass("auto-column")) {
                        PageBuilderEdition.columnsEdition.onBasicColumnAdded(evt);
                    } else {
                        // Añadir columna auto que ocupe el ancho disponible
                        PageBuilderEdition.columnsEdition.onAutoColumnAdded(evt);
                    }

                },
            };
        },
        getDefaultColumnsListOptions: function () {
            return {
                group: {
                    name: "js-default-columns-list",
                    pull: "clone",
                },
                sort: false,
                dragoverBubble: true,
                handle: ".handler",
                onMove: function (evt) {
                    const to = $(evt.to);
                    const item = $(evt.dragged);
                    const itemPercent = parseInt(item.data("percent"));
                    const lis = to.find("li.cmscolumn");

                    if (lis.length > 0) {
                        let sum = 0;
                        lis.each(function () {
                            sum += parseInt($(this).data("percent"));
                        });

                        // Tener en cuenta las columnas "auto"
                        if (!item.hasClass("auto-column")) {
                            if (sum + itemPercent > 100) {
                                return false;
                            }
                        } else {
                            // Espacio mínimo
                            if (sum >= 83.33) {
                                return false;
                            }
                        }
                    }
                },
            };
        },
        onBasicColumnAdded: function (event) {
            var item = $(event.item);
            item.removeClass("builder-item").addClass("cmscolumn");
            // Crear un Guid para la nueva columna añadida
            const columnId = guidGenerator();
            item.attr("id", columnId);
            // Añadir 'data-columnclass' para el guardado en BD a partir del 'data-spanclass' el item arrastrado
            const columnClassName = `${item.data("spanclass")} break`;
            item.data("columnclass", columnClassName);

            var dataPercent = item.attr("data-percent");

            item.css("width", this.getColumnWidthByPercent(dataPercent));

            var columnTemplate = this.getColumnWrapHtmlTemplate();
            item.empty().append(columnTemplate);
            // Indicar que hay cambios en el CMS para posibilitar el guardado
            operativaGestionCMSBuilderGuardadoSeguridad.setIsNecessarySaveCmsHtmlContentInPageBuider(true);
            PageBuilderManagement.init();
        },
        getColumnWrapHtmlTemplate: function () {
            return `<div class="cmscolumn-wrap">
                        <div class="cmscolumn-header">
                            <div class="cmscolumn-actions js-cmscolumn-actions">
                                <ul class="no-list-style">
                                    <li>
                                        <a class="action-delete round-icon-button js-action-delete-column">
                                            <span class="material-icons">delete</span>
                                        </a>
                                    </li>
                                </ul>
                            </div>
                        </div>
                        <div class="cmscolumn-content">
                            <ul class="components-list no-list-style"></ul>
                        </div>
                    </div>`;
        },
        /**
         * Añadir una columna
         * @param {*} percentage : Dependiendo del percetage, añadir información del span para guardarlo en BD
         * @returns 
         */
        getColumnHtmlTemplate: function (percentage = undefined) {
            let spanColumnClass = "";
            if (percentage != undefined) {
                switch (percentage) {
                    case "100":
                        spanColumnClass = "span11";
                        break;
                    case "50":
                        spanColumnClass = "span12";
                        break;
                    case "33.33":
                        spanColumnClass = "span13";
                        break;
                    case "25":
                        spanColumnClass = "span14";
                        break;
                    case "16.66":
                        spanColumnClass = "span16";
                        break;
                    case "66.66":
                        spanColumnClass = "span23";
                        break;
                    case "75":
                        spanColumnClass = "span34";
                        break;
                    case "37.5":
                        spanColumnClass = "span38";
                        break;
                    case "62.5":
                        spanColumnClass = "span58";
                        break;
                }
            }
            // Crear guid/id para cada columna
            const columnId = guidGenerator();
            // Añadir 'data-columnclass' para el guardado en BD
            const columnClassName = `${spanColumnClass} break`;
            return `<li id="${columnId}" 
                    class="cmscolumn js-cmscolumn"
                    data-columnclass="${columnClassName}"
                    data-spanclass="${spanColumnClass}">
                        ${this.getColumnWrapHtmlTemplate()}
                    </li>`;
        },
        getColumnWidthByPercent: function (percent) {
            return "calc(" + percent + "% - 16px)";
        },

        /**
         * Método que se ejecuta al añadir una columna de tipo "Auto" para que se rellene el espacio sin usar de la fila
         * @param {*} event 
         */
        onAutoColumnAdded: function (event) {
            var item = $(event.item);
            item.removeClass("builder-item").addClass("cmscolumn");
            // Crear un Guid para la nueva columna añadida
            const columnId = guidGenerator();
            item.attr("id", columnId);

            // Calcular espacio libre para el tamaño de la columna auto
            const dataPercent = this.getFreeColumnsAvailableForAutoColumn(item);
            const classNameFromFreePercentage = this.getSpanClassFromFreePercentage(dataPercent).replace(/-/g, '');
            const columnClassName = `${classNameFromFreePercentage} break`;
            // Asignar a columnclass y dataPercent
            item.data("columnclass", columnClassName);
            item.data("percent", dataPercent);
            item.data("spanclass", classNameFromFreePercentage);

            // Asignar el porcentaje para la vista preliminar del elemento en el editor           
            item.css("width", this.getColumnWidthByPercent(dataPercent));

            var columnTemplate = this.getColumnWrapHtmlTemplate();
            item.empty().append(columnTemplate);
            // Indicar que hay cambios en el CMS para posibilitar el guardado
            operativaGestionCMSBuilderGuardadoSeguridad.setIsNecessarySaveCmsHtmlContentInPageBuider(true);
            PageBuilderManagement.init();
        },

        /**
         * Devuelve el nº de columnas disponibles de la fila y en base al nº de columnas disponible, devolver la clase o spanClass necesario
         * @param {*} item Item que se ha arrastrado. En este caso, el de tipo "auto"
         */
        getFreeColumnsAvailableForAutoColumn: function (item) {
            // Obtener el contenedor donde se ha alojado la celda auto
            const columnsListSpace = item.closest(".columns-list");
            const siblingsColumns = columnsListSpace.children("li.cmscolumn").not(item);
            let dataPercentageForAutoColumn = 100;
            let rowPercentageUsage = 0;

            if (siblingsColumns.length > 0) {
                siblingsColumns.each(function () {
                    rowPercentageUsage += parseFloat($(this).attr("data-percent"));
                });
            }
            const freePercentage = dataPercentageForAutoColumn - rowPercentageUsage;
            console.log(freePercentage);
            return freePercentage;
        },

        /**
         * Devuelve la clase span correspondiente en base al porcentaje libre.
         * @param {number} freePercentage El porcentaje libre en la fila.
         * @returns {string} La clase span correspondiente.
         */
        getSpanClassFromFreePercentage: function (freePercentage) {
            if (freePercentage >= 100) return 'span-1-1';
            if (freePercentage >= 83.00) return 'span-5-6';
            if (freePercentage >= 75.00) return 'span-3-4';
            if (freePercentage >= 66.66) return 'span-2-3';
            if (freePercentage >= 62.5) return 'span-5-8';
            if (freePercentage >= 50) return 'span-1-2';
            if (freePercentage >= 37.5) return 'span-3-8';
            if (freePercentage >= 33.33) return 'span-1-3';
            if (freePercentage >= 25) return 'span-1-4';
            if (freePercentage >= 16.66) return 'span-1-6';
            if (freePercentage >= 8.33) return 'span-1-12';
            return 'Sin Span válido'; // Devuelve una cadena vacía si no hay suficiente espacio para ninguna columna.
        }
    },
    componentsEdition: {
        initComponents: function () {
            this.initCreatedComponentsList();
            this.initNewComponentsList();
            this.initComponentsLists();
        },
        initCreatedComponentsList: function () {
            // Permitir configuración para componentes creados dinámicamente/buscados
            // if (typeof this.createdComponentsSortable !== "undefined") return;

            var createdComponentsList = document.querySelector(".js-created-components-list");
            var options = this.getCreatedComponentsListOptions();

            this.createdComponentsSortable = Sortable.create(createdComponentsList, options);
        },
        initNewComponentsList: function () {
            if (typeof this.newComponentsSortable !== "undefined") return;

            var newComponentsList = document.querySelector(".js-new-component-list");
            var options = this.getNewComponentsListOptions();

            this.newComponentsSortable = Sortable.create(newComponentsList, options);
        },
        initComponentsLists: function () {
            var components_list = document.getElementsByClassName("components-list");
            var options = this.getComponentsListOptions();

            for (var i = 0; i < components_list.length; i++) {
                Sortable.create(components_list[i], options);
            }
        },
        getComponentsListOptions: function () {
            return {
                group: {
                    name: "components-list",
                    put: ["js-created-components-list", "js-new-components-list"],
                },
                sort: true,
                dragoverBubble: true,
                handle: ".js-action-handle-row",
                onAdd: function (evt) {
                    var item = $(evt.item);
                    PageBuilderEdition.componentsEdition.onComponentAdded(evt);
                },
            };
        },
        getNewComponentsListOptions: function () {
            return {
                group: {
                    name: "js-new-components-list",
                    pull: "clone",
                },
                sort: false,
                dragoverBubble: true,
                handle: ".handler",
            };
        },
        getCreatedComponentsListOptions: function () {
            return {
                group: {
                    name: "js-created-components-list",
                    pull: "clone",
                },
                sort: false,
                dragoverBubble: true,
                handle: ".handler",
            };
        },
        onComponentAdded: function (event) {
            var $item = $(event.item);
            const $name = $item.find(".name").clone(true);

            const id = $item.attr("data-id");

            let idHtml = "";
            if (id) {
                idHtml = this.getIdHtml(id);
            }

            $name.append(idHtml);

            $item.removeClass("builder-item").addClass("cmscomponent");
            $item.html(this.getComponentWrapHtmlTemplate($name.get(0).outerHTML, id));
            // Indicar que hay cambios en el CMS para posibilitar el guardado
            operativaGestionCMSBuilderGuardadoSeguridad.setIsNecessarySaveCmsHtmlContentInPageBuider(true);
            PageBuilderManagement.init();

            // Mostrar modal para crear el nuevo componente arrastrado (Sólo si el componente es nuevo)
            if ($item.attr("id") == "" || $item.attr("id") == undefined) {
                operativaGestionCMSBuilder.onComponentAddedLoadModal($item);
            }
        },
        getComponentWrapHtmlTemplate: function (name, id) {

            // Construir la ruta para editar el recurso a partir del item arrastrado            
            let urlSaveComponent = '';
            let urlEditComponent = '';

            if (id != undefined) {
                urlSaveComponent = operativaGestionCMSBuilder.pParams.urlSaveComponentTemplate.replace("COMPONENT_KEY", id);
                urlEditComponent = operativaGestionCMSBuilder.pParams.urlEditComponentTemplate.replace("COMPONENT_KEY", id);
            }

            return `<div class="cmscomponent-wrap">
                        <div class="cmscomponent-header">
                            ${name}
                            <div class="cmscomponent-actions js-cmscomponent-actions dropdown">
                                <a href="#" class="dropdown-toggle" role="button" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
                                    <span class="material-icons">more_vert</span>
                                </a>
                                <div class="dropdown-menu basic-dropdown dropdown-icons dropdown-menu-right">
                                    <ul class="no-list-style">
                                        <li>
                                            <a class="item-dropdown js-edit-component btnEditComponentFromCMSBuilder" 
                                               data-target="#modal-container" 
                                               data-urleditcomponent="${urlEditComponent}" 
                                               data-urlsave="${urlSaveComponent}" 
                                               data-toggle="modal">
                                                <span class="material-icons">create</span>
                                                <span class="texto">Editar</span>
                                            </a>                                            
                                        </li>
                                        <li>
                                            <a class="item-dropdown js-delete-component">
                                                <span class="material-icons">delete</span>
                                                <span class="texto">Eliminar</span>
                                            </a>
                                        </li>
                                    </ul>
                                </div>
                            </div>
                        </div>
                    </div>`;
        },
        getIdHtml: function (id) {
            return `<div class="cmscomponent-id-wrap">
                        <span class="cmscomponent-id">${id}</span>
                        <span class="js-copy-component-id material-icons">content_copy</span>
                    </div>`;
        },
    },
    plantillasPredeterminadas: {
        init: function () { },
    },
};

var PageBuilderManagement = {
    init: function () {
        PageBuilderEdition.init();
        accionesFilasPageBuilder.init();
        accionesColumnasPageBuilder.init();
        accionesComponentesPageBuilder.init();
    },
};

var accionesFilasPageBuilder = {
    init: function () {
        this.config();
        this.actionEdit();
        this.actionClone();
        this.actionDelete();
        editRowModal.init();
    },
    config: function () { },
    countRows: function () {
        for (let i = 0; i < $(".js-cmsrow").length; i++) {
            var number = $(".js-cmsrow").find(".number");
            $(number[i])
                .empty()
                .text(i + 1);
        }
    },
    actionEdit: function () {
        const that = this;
        var edit_buttons = $(".js-cmsrow-actions .js-action-edit-row");
        edit_buttons.off("click").on("click", function () {
            const $row = $(this).closest(".js-cmsrow");
            editRowModal.openEditionModal($row);
        });
    },
    actionDelete: function () {
        var that = this;
        var delete_buttons = $(".js-cmsrow-actions .js-action-delete");
        delete_buttons.off("click").on("click", function () {
            var cmsrow = $(this).closest(".cmsrow");
            pagebuilderConfirmationModal.openModal(cmsrow, that.deleteRow.bind(that));
        });
    },
    deleteRow: function (row) {
        row.remove();
        this.countRows();
    },
    actionClone: function () {
        var that = this;
        var clone_buttons = $(".js-cmsrow-actions .js-action-clone");
        clone_buttons.off("click").on("click", function () {
            const currentRow = $(this).closest(".cmsrow");
            const clonedRow = currentRow.clone(true);
            // Antes de añadir la row 'clonada' cambiar los Ids de las filas y columnas.
            that.updateIdsForClonedItem(currentRow);
            clonedRow.insertAfter(currentRow);
            that.countRows();
        });
    },

    /**
     * Método para actualizar los ids de la fila clonada. Será necesario actualizar las filas y columnas ya que
     * en el proceso de guardado, no puede haber items con IDS idénticos.     
     * @param {jqueryElement} currentRow 
     */
    updateIdsForClonedItem: function (currentRow) {
        // Columnas existentes de la fila que se va a clonar
        const columnsFromClonedRow = currentRow.find(`.cmscolumn`);

        columnsFromClonedRow.each(function () {
            const column = $(this);
            const newColumnId = guidGenerator();
            // Cambiar el id de la columna
            column.attr("id", newColumnId);
        });

        // Cambiar el id de la fila clonada
        const newRowId = guidGenerator();
        currentRow.attr("id", newRowId);
    },
};

var accionesColumnasPageBuilder = {
    init: function () {
        this.actionDelete();
        this.actionCopyId();
    },
    actionDelete: function () {
        var that = this;
        const $delete_buttons = $(".js-cmscolumn-actions .js-action-delete-column");

        $delete_buttons.off("click").on("click", function () {
            const $column = $(this).closest(".cmscolumn");
            pagebuilderConfirmationModal.openModal($column, that.deleteColumn.bind(that));
        });
    },
    deleteColumn: function (column) {
        column.remove();
    },
    actionCopyId: function () {
        const that = this;
        const $copyButtons = $(".js-copy-component-id");
        $copyButtons.off("click").on("click", function () {
            const $spanId = $(this).siblings(".cmscomponent-id");
            that.copyTextToClipboard($spanId.text());
        });
    },

    fallbackCopyTextToClipboard: function (text) {
        const textArea = document.createElement("textarea");
        textArea.value = text;

        // Avoid scrolling to bottom
        textArea.style.top = "0";
        textArea.style.left = "0";
        textArea.style.position = "fixed";

        document.body.appendChild(textArea);
        textArea.focus();
        textArea.select();

        try {
            const successful = document.execCommand("copy");
            if (successful) {
                mostrarNotificacion("success", "ID copiado al portapapeles");
            }
        } catch (err) {
            console.error(err);
        }

        document.body.removeChild(textArea);
    },

    copyTextToClipboard: function (text) {
        const that = this;

        if (!navigator.clipboard) {
            that.fallbackCopyTextToClipboard(text);
            return;
        }
        navigator.clipboard.writeText(text).then(
            function () {
                mostrarNotificacion("success", "ID copiado al portapapeles");
            },
            function (err) {
                console.error(err);
            }
        );
    },
};

var accionesComponentesPageBuilder = {
    init: function () {
        this.actionDelete();
    },
    actionDelete: function () {
        var that = this;
        var delete_buttons = $(".js-cmscomponent-actions .js-delete-component");
        delete_buttons.off("click").on("click", function () {
            var $component = $(this).closest(".cmscomponent");
            pagebuilderConfirmationModal.openModal($component, that.deleteComponent.bind(that));
        });
    },
    deleteComponent: function ($component) {
        $component.remove();
    },
};

var pagebuilderConfirmationModal = {
    /**
     * @param {object} element to be deleted
     * @param {function} callbackFunction to be called when confirm 
     */
    openModal: function (element, callbackFunction) {
        $("#modal-confirmar-eliminar").modal({ backdrop: "static", keyboard: false }, "show");

        $("#modal-confirmar-eliminar")
            .find(".js-delete-yes")
            .off("click")
            .on("click", function () {
                // Indicar que hay cambios en el CMS para posibilitar el guardado
                operativaGestionCMSBuilderGuardadoSeguridad.setIsNecessarySaveCmsHtmlContentInPageBuider(true);
                callbackFunction(element);
            });
    },
};

var editRowModal = {
    init: function () {
        const that = this;
        this.config();
        this.saveAttributeEvent();

        const new_attribute_button = this.editModal.find(".new-attribute");
        new_attribute_button.off("click").on("click", function () {
            that.editModal.addClass("editing");
        });

        const close_attribute_button = this.editModal.find(".close-new-attribute");
        close_attribute_button.off("click").on("click", function () {
            that.resetForm();
        });
    },
    config: function () {
        this.editModal = $("#modal-editar-atributos-fila");
        this.nameInput = $("#attribute-name-input");
        this.valueInput = $("#attribute-value-input");
        this.editingAttribute = false;
    },
    getRowAttributes: function ($row) {
        const attributes = $row.attr("data-row-attributes");
        if (!attributes) return [""];
        return attributes.split(",");
    },
    openEditionModal: function ($row) {
        this.editedRow = $row;
        const attributes = this.getRowAttributes($row);
        this.buildAttributesList(attributes);
        this.resetForm();
        this.editModal.modal("show");
    },
    buildAttributesList: function (attributes) {
        const that = this;

        that.cleanAttributeList();

        attributes.forEach((attribute) => {
            if (attribute === "") return;
            that.addRowAttribute(that.getAttributeTrHtml(attribute.split("|")));
        });

        PageBuilderEdition.rowsEdition.updateAttributesDisplay();
    },
    /**
     * @param {Array} ['name','value'] attribute
     * @returns
     */
    getAttributeTrHtml: function (attribute) {
        return `<tr>
                    <td class="td-name">${attribute[0]}</td>
                    <td class="td-value">${attribute[1]}</td>
                    <td class="td-acciones">
                        <div class="dropdown">
                            <a class="dropdown-toggle" data-toggle="dropdown" href="#" role="button" aria-haspopup="true" aria-expanded="false">
                                <span class="material-icons">edit</span>
                                Editar
                            </a>
                            <div class="dropdown-menu dropdown-menu-left basic-dropdown">
                                <ul class="no-list-style">
                                    <li>
                                        <a class="item-dropdown js-edit-attribute"><span class="material-icons">edit</span>Editar</a>
                                    </li>
                                    <li>
                                        <a class="item-dropdown js-delete-attribute"><span class="material-icons">delete</span>Borrar</a>
                                    </li>
                                </ul>
                            </div>
                        </div>
                    </td>
                </tr>`;
    },
    addRowAttribute: function (attributeTr) {
        const table = this.editModal.find(".row-attributes-table");
        const tableBody = table.find("tbody");
        tableBody.append(attributeTr);
        this.addAttributesEvents();
    },
    cleanAttributeList: function () {
        const table = this.editModal.find(".row-attributes-table");
        const tableBody = table.find("tbody");
        tableBody.html("");
    },
    addAttributesEvents: function () {
        const that = this;
        const table = this.editModal.find(".row-attributes-table");
        const acciones = table.find(".td-acciones");
        const deleteAcciones = acciones.find(".js-delete-attribute");
        const editAcciones = acciones.find(".js-edit-attribute");

        editAcciones.off("click").on("click", function () {
            const trAttribute = $(this).closest("tr");
            that.openEditionAttribute(trAttribute);
        });

        deleteAcciones.off("click").on("click", function () {
            $(this).closest("tr").remove();
            that.updateAttributesData();
            PageBuilderEdition.rowsEdition.updateAttributesDisplay();
        });
    },
    openEditionAttribute: function (trAttribute) {
        this.editingAttribute = trAttribute;

        const name = trAttribute.find(".td-name").text();
        const value = trAttribute.find(".td-value").text();
        this.nameInput.val(name);
        this.valueInput.val(value);

        this.editModal.addClass("editing");
    },
    updateAttributesData: function () {
        const that = this;
        const table = this.editModal.find(".row-attributes-table");
        const attributeTrs = table.find("tbody tr");
        const attributes = [];
        /// Atributo y Valor que se guardará en formato para ser enviando a BackEnd
        let attributeValueStringToBackEnd = "";
        attributeTrs.each(function (index, element) {
            const tr = $(this);
            attributes.push(`${tr.find(".td-name").text()}|${tr.find(".td-value").text()}`);
        });
        this.editedRow.attr("data-row-attributes", attributes.join());
        // Guardar los atributos en el formato correcto para que sean guardados en backEnd
        $.each(attributes, function (key, value) {
            const attributeValueArray = value.split("|");
            // Obtener el atributo y value
            const attribute = attributeValueArray[0];
            const attributeValue = attributeValueArray[1];
            // Añadir el atributo y value en formato para ser guardado en backEnd
            attributeValueStringToBackEnd += `${attribute}---${attributeValue}~~~`;
        });
        // Añadir los atributos y valores a la fila para ser guardada en BackEnd
        this.editedRow.data("aux", attributeValueStringToBackEnd);
    },
    saveAttributeEvent: function () {
        const that = this;
        const addAttributeButton = this.editModal.find(".add-attribute");
        addAttributeButton.off("click").on("click", function () {
            const name = that.nameInput.val();
            const value = that.valueInput.val();

            if (name !== "" && value !== "") {
                if (that.editingAttribute) {
                    that.updateAttribute(name, value);
                } else {
                    that.addRowAttribute(that.getAttributeTrHtml([name, value]));
                }
                that.updateAttributesData();
                PageBuilderEdition.rowsEdition.updateAttributesDisplay();
                that.resetForm();
            }
        });
    },
    updateAttribute: function (name, value) {
        this.editingAttribute.find(".td-name").text(name);
        this.editingAttribute.find(".td-value").text(value);
    },
    resetForm: function () {
        this.nameInput.val("");
        this.valueInput.val("");
        this.editingAttribute = false;
        this.editModal.removeClass("editing");
    },
};


/* Gestión de componentes (Drag) */
var componentesEdition = {
    init: function () {
        this.initComponents();
    },
    initComponents: function () {
        const components_lists = document.querySelectorAll(".js-community-components-list");
        const componentsOptions = this.getAddedComponentOptions();
        components_lists.forEach((component_list) => {
            Sortable.create(component_list, componentsOptions);
        });
    },
    getAddedComponentOptions: function () {
        return {
            group: {
                name: "js-community-components-list",
            },
            sort: true,
            fallbackOnBody: true,
            swapThreshold: 0.65,
            animation: 150,
            handle: ".js-component-sortable-component",
            onAdd: function (evt) { },
            onChoose: function (evt) { },
            onUnChoose: function (evt) { },
        };
    },
};

var ComponentesPageManagement = {
    init: function () {
        componentesEdition.init();
    },
};

function dragAndDropGnoss() {
    var acceptedFiles = "*";
    var maxSizeAllowed = "102400"; //KB
    if ($('.dragAndDrop').length > 0) {
        acceptedFiles = $('.dragAndDrop').attr('accept').split(',');
        maxSizeAllowed = $('.dragAndDrop').attr('max-size');
    }
    if (acceptedFiles.length <= 1) {
        acceptedFiles = acceptedFiles[0];
    }
    const options = {
        //acceptedFiles: ['jpg', 'png', 'txt', 'doc', 'docx'],
        acceptedFiles: acceptedFiles,
        maxSize: maxSizeAllowed,
        beforeValidation: function (plugin, files) {
        },
        onFileAdded: function (plugin, files) {
        },
        onFileRemoved: function (plugin) {
        }
    };
    $('.dragAndDrop').GnossDragAndDrop(options);
}


// Comportamiento de autocompletar en workflows
const autocompletarWorkflows = {
    init: function () {
        this.config();
        this.exec();
        this.bindFormEvents();
        this.addTagEvents();
    },
    config: function () {
        this.modalWorkflow = $('#modal-editar-workflow, #modal-nuevo-workflow');
        this.autocompletar = this.modalWorkflow.find('.form-group>.form-group.autocompletar');
    },
    exec: function () {
        // click en el input del modal abre el formulario oculto
        this.autocompletar.find('>.input-wrap').on('click', (e) => {
            this.toggleForm(e);
        });

        // boton más del input
        this.autocompletar.find('>.input-wrap .btn').off('click').on('click', (e) => {
            e.preventDefault();
            // no hacer click si el input esta disabled
            if ($(e.currentTarget).siblings('input').attr('disabled')) return;

            // muestra el formulario oculto
            this.toggleForm(e);
        });

        // select tipo recursos
        this.autocompletar.off().find('.select-recursos').on('change', (e) => {
            e.preventDefault();
            const target = $(e.currentTarget);
            this.onSelectAutocomplete(target);
        });
    },
    bindFormEvents: function () {
        // cancelamos el evento autocompletar del jquery.autocomplete.js
        this.autocompletar.off('click');

        // click en aceptar del formulario
        this.autocompletar.find('.formulario-oculto .btn-primary').off('click').on('click', (e) => {
            e.preventDefault();
            const parent = $(e.currentTarget).closest('.autocompletar');

            // recoger datos del formulario
            this.onFormCompleted(parent);

            // oculta el formulario y borra los campos
            this.resetForm(parent);
        });

        // click en cancelar del formulario
        this.autocompletar.find('.formulario-oculto .btn-grey').off('click').on('click', (e) => {
            e.preventDefault();
            const parent = $(e.currentTarget).closest('.autocompletar');
            if (parent.find('.formulario-oculto').hasClass("editando")) {
                let id = $(e.currentTarget).parent().attr("data-id");
                parent.find('.formulario-oculto').removeClass("editando");
                parent.find(`.tag-list .tag[data-id="${id}"]`).removeClass("d-none");
            }
            
            this.resetForm(parent);
        });
    },
    toggleForm: function (e) {
        const parent = $(e.currentTarget).closest('.autocompletar');
        const inputWrap = parent.find('>.input-wrap');
        const formOculto = parent.find('.formulario-oculto');
        const modalID = parent.parents('.modal-flujo').attr('id')
        if (!parent.hasClass('active')) {
            const lang = $('#panContenidoMultiIdioma .nav-tabs a[aria-selected="true"]').attr('id').split('_')[1];
            const titleInputs = formOculto.find('.nombre-form-oculto');
            titleInputs.hide();
            const titleInputActive = formOculto.find(`.nombre-form-oculto-${lang}`);
            titleInputs.val(inputWrap.find('input[type="text"]').val());
            titleInputActive.show();

            //const inputClone = inputWrap.clone(true);
            //inputClone.prependTo(formOculto);
            inputWrap.hide();
            parent.addClass('active');
            parent.find('#estado-inicial').attr('checked', true);
            formOculto.attr('data-id', guidGenerator());
            formOculto.removeClass('d-none').addClass('d-block');
            //formOculto.find('>.input-wrap input[type="text"]').focus();
            titleInputActive.focus();
        }
        if (parent.hasClass("autocompletar-transiciones") && $(e.currentTarget).hasClass("input-wrap")) {
            let estados = parent.parents('.tab-content').first().find('.autocompletar-estados .tag-list').children();
            let that = this;
            estados.each(function () {
                let estado = $(this);
                let typeStateValue = estado.find('input[type="hidden"]').data("tipo-estado");
                let id = estado.attr("data-id");
                let tagValue = estado.find(".tag-label").clone().children().remove().end().text().trim();
                let tagValues = estado.attr("title");
                let isPublic = estado.attr("data-publico");
                if (typeStateValue == '0' || typeStateValue == '1') {
                    that.updateOption(modalID, '.select-origin', id, typeStateValue, isPublic, tagValue, null, tagValues);

                    if (typeStateValue == '1') {
                        that.updateOption(modalID, '.select-end', id, typeStateValue, isPublic, tagValue, null, tagValues);
                    }
                } else if (typeStateValue == '2') {
                    that.updateOption(modalID, '.select-end', id, typeStateValue, isPublic, tagValue, null, tagValues);
                }
            });
        }
    },
    resetForm: function (parent) {
        parent.removeClass('active');
        const formOculto = parent.find('.formulario-oculto');
        //parent.find('>.input-wrap').find('input[type="text"]').val('');
        formOculto.attr('data-id', '');
        formOculto.find('.nombre-form-oculto').each(function () {
            $(this).val('');
        });
        parent.find('>.input-wrap').show();
        formOculto.removeClass('d-block').addClass('d-none');
        formOculto.find('>.input-wrap').remove();
        formOculto.find('.tag-list').html('');
        formOculto.find('select').val('');
    },
    onFormCompleted: function (parent) {
        const lang = $('#panContenidoMultiIdioma .nav-tabs a[aria-selected="true"]').attr('id').split('_')[1];
        const formOculto = parent.find('.formulario-oculto');
        formOculto.hasClass("editando") && formOculto.removeClass("editando");
        const list = parent.find('>.tag-list');
        const inputHidden = parent.children('input[type="hidden"]');
        const id = formOculto.attr('data-id');
        const modalID = parent.parents('.modal-flujo').attr('id')
        // Si ha sido editado borrar el anterior tag
        list.find(`.tag[data-id="${id}"]`).remove();
        const tagValues = formOculto.find('.nombre-form-oculto');
        let tagValue = formOculto.find(`.nombre-form-oculto-${lang}`).val().trim();
        // Si el titulo en el idioma por defecto está vacio no guardamos la tag
        const defaultLang = $("#idiomaDefecto").val();
        if (!formOculto.find(`.nombre-form-oculto-${defaultLang}`).val().trim()) {
            mostrarNotificacion("error", "No puedes dejar el titulo vacio en el idioma por defecto");
            return;
        }

        // añade estados o transiciones segun en el formulario que estes
        if (parent.hasClass('autocompletar-estados')) {
            //Todo: añadir validación
            let typeState = parent.find('input[name="tipo-estado"]:checked');
            let typeStateValue = typeState.val();
            let typeStateText = (typeStateValue === '0') ? 'Estado Inicial' : (typeStateValue === '1') ? 'Estado Intermedio': 'Estado final';
            let tagEditorsList = parent.find(".editores .tag-list").children();
            let tagReadersList = parent.find(".lectores .tag-list").children();
            let statePrivacy = formOculto.find('#estado-publico-si').prop('checked');
            let stateColor = formOculto.find('input[type="color"]').val();
            list.append(this.addTagState(id, this.formatMultiLangText(tagValues), tagValue, statePrivacy, stateColor, typeStateText, typeStateValue, tagEditorsList, tagReadersList));
            $(`#${modalID} .select-origin option[value=${id}]`).remove();
            $(`#${modalID} .select-end option[value=${id}]`).remove();
            // añade los estados en las transiciones
            if (typeStateValue == '0' || typeStateValue == '1') {
                this.updateOption(modalID, '.select-origin', id, typeStateValue, statePrivacy, tagValue, tagValues);

                if (typeStateValue == '1') {
                    this.updateOption(modalID, '.select-end', id, typeStateValue, statePrivacy, tagValue, tagValues);
                }
            } else if (typeStateValue == '2') {
                this.updateOption(modalID, '.select-end', id, typeStateValue, statePrivacy, tagValue, tagValues);
            }

        } else if (parent.hasClass('autocompletar-transiciones')) {
            //Todo: añadir validación
            const origin = formOculto.find('.select-origin option:selected');
            const end = formOculto.find('.select-end option:selected');

            let tagResponsiblesList = formOculto.find(".responsables .tag-list").children();
            list.append(this.addTagState(id, this.formatMultiLangText(tagValues), tagValue,null, null, null, null, null, null, tagResponsiblesList, origin, end));
        } else {
            list.append(this.addSpanTag(tagValue));
        }

        this.sortTags(list.children(), parent);
        this.onChecked();
        this.addTagEvents();
        this.updateHiddenInput(list, inputHidden);
    },
    onChecked: function () {
        const list = $('.autocompletar.autocompletar-estados').find('>.tag-list');
        // si hay 2 estados o mas toggle disabled en el formulario
        if (list.find('.tag').length >= 2) {
            list.closest('.tab-pane').find('.autocompletar-transiciones>.input-wrap input[type="text"]').attr('disabled', false);
            list.closest('.tab-pane').find('.autocompletar-recursos>select').attr('disabled', false);
        } else {
            list.closest('.tab-pane').find('.autocompletar-transiciones>.input-wrap input[type="text"]').attr('disabled', true);
            list.closest('.tab-pane').find('.autocompletar-recursos>select').attr('disabled', true);
        }
    },
    onSelectAutocomplete: function (elem) {
        const target = $(elem);
        const parent = target.closest('.autocompletar');
        const list = parent.find('>.tag-list');
        const inputHidden = parent.parent().children('input[type="hidden"]');

        const tagValue = target.val().trim();
        const tagText = target.find(":selected").text()
        const tagOntoID = target.find(":selected").data("onto-id");
        const tagOntoName = target.find(":selected").data("onto-name");

        if (!tagValue) return;

        // si es el select de recursos, eliminar la opción seleccionada
        if (tagValue != "5") {
            target.find('option[value="' + tagValue + '"]').remove();
        } else {
            target.find('option[value="' + tagValue + '"][data-onto-id=' + tagOntoID + ']').remove();
        }
        list.append(this.addTagResourceType(tagValue, tagOntoName, tagOntoID, tagText));
        target.val('');

        this.addTagEvents();
        this.updateHiddenInput(list, inputHidden);
    },
    addTagResourceType: function (tagValue, tagOntoName, tagOntoID, tagText) {
        return `
                <div class="tag" data-id="${tagOntoID}" data-onto-name="${tagOntoName}" title="${tagText}" >
                    <div class="tag-wrap">
                        <span class="tag-text">
                            <span class="tag-label">${tagText}</span>
                        </span>
                        <span class="tag-remove material-icons">delete</span>
                    </div>
                    <input type="hidden" value="${tagValue}">
                </div>
            `
    },

    addTagState: function (stateID, stateNameMultiLang, stateName, statePrivacy, stateColor, stateText, stateType, stateEditorsList, stateReadersList, responsiblesList, origin, end) {
        let editorsHtml = "";
        if (stateEditorsList !== null && stateEditorsList !== undefined) {
            stateEditorsList.each((i, element) => {
                editorsHtml += this.addSpanTag($(element));
            });
        }

        let readersHtml = "";
        if (stateReadersList !== null && stateReadersList !== undefined) {
            stateReadersList.each((i, element) => {
                readersHtml += this.addSpanTag($(element));
            });
        }

        let responsiblesHtml = "";
        if (responsiblesList !== null && responsiblesList !== undefined) {
            responsiblesList.each((i, element) => {
                responsiblesHtml += this.addSpanTag($(element));
            });
        }

        return `
            <div class="tag" data-id="${stateID}" title="${stateNameMultiLang}">
                <div class="tag-wrap">
                    <span class="tag-text">
                    <span class="tag-label">${stateName} ${stateReadersList != null && stateReadersList != undefined ? `<span class="tag-state">-${stateText} </span>` : ''}</span>
                        ${origin || (origin && end) ? `
                            
                            <span class="users responsibles">
                                <span class="user-label">Responsable</span>
                                ${responsiblesHtml}
                            </span>
                            <span class="transition">
                                <span class="transition-label" data-publico="${origin.attr('data-publico') }" data-tipo="${origin.attr('data-tipo')}" data-id="${origin.attr('data-id')}" title="${origin.attr('data-text')}">${origin.text()}</span>
                                ${end !== "" ? `<span class="material-icons arrow-icon">arrow_right_alt</span>` : ''}
                                <span class="transition-label" data-publico="${end.attr('data-publico') }" data-tipo="${end.attr('data-tipo')}" data-id="${end.attr('data-id')}" title="${end.attr('data-text')}">${end.text()}</span>
                            </span>
                        ` : `
                                    <span class="users editors">
                                        <span class="user-label">Editores</span>
                                            ${editorsHtml}  
                                    </span>
                                    <span class="users readers">
                                        <span class="user-label">Lectores</span>
                                            ${readersHtml}
                                    </span>
                                </span>
                        `
            }
                    </span>
                    <span class="tag-edit material-icons">edit</span>
                    <span class="tag-remove material-icons">delete</span>
                </div>
                ${stateReadersList == null ?
                `<input type="hidden" data-estado-origen-id="${origin.attr('data-id')}" data-estado-fin-id="${end.attr('data-id')}" value="${stateNameMultiLang}">` :
                `<input type="hidden" data-tipo-estado="${stateType}" data-estado-color="${stateColor}" data-estado-publico="${statePrivacy}" value="${stateNameMultiLang}">`}
            </div>
        `
    },

    addPersonTag: function (tag) {
        let tagID = $(tag).attr("data-id");
        let tagValue = $(tag).text();
        let tagGrupo = $(tag).attr("data-grupo");
        return `
                <div class="tag" data-grupo="${tagGrupo}" data-id="${tagID}" title="${tagValue}" >
                    <div class="tag-wrap">
                        <span class="tag-text">
                            <span class="tag-label">${tagValue}</span>
                        </span>
                        <span class="tag-remove custom material-icons">delete</span>
                    </div>
                </div>
            `
    },

    addSpanTag: function (tag) {
        let tagID = $(tag).attr("data-id");
        let tagValue = $(tag).attr('title');
        let grupo = $(tag).attr('data-grupo')
        return `<span data-grupo="${grupo}" data-id="${tagID}">${tagValue}</span>`;
    },

    addTagEvents: function () {
        // editar tag
        this.autocompletar.find('.tag-list .tag-edit').off('click').on('click', (e) => {
            const tag = $(e.currentTarget).closest('.tag');
            tag.parent().children().removeClass("d-none")
            this.editTag(e, tag);
        });

        // eliminar tag
        this.autocompletar.find('.tag-list .tag-remove').off('click').on('click', (e) => {
            const tag = $(e.currentTarget).closest('.tag');
            const tagValue = tag.find('input[type="hidden"]').val();
            const autocompletar = $(e.currentTarget).closest('.autocompletar');
            const parent = autocompletar.closest('.tab-content');
            const isNewWorkflow = parent.parents("#modal-nuevo-workflow").length == 1;

            // si es el select de recursos, volver a añadir la opción eliminada
            if (autocompletar.hasClass('autocompletar-recursos')) {
                const select = parent.find('.autocompletar-recursos').find('select');
                const tagText = tag.attr("title");
                let option = '<option value="' + tagValue + '">' + tagText + '</option>';
                if (tagValue == '5') {
                    let tagOntoID = tag.data('id');
                    let tagOntoName = tag.data('onto-name');
                    option = `<option data-onto-id="${tagOntoID} data-onto-name="${tagOntoName}" value="${tagValue}>${tagText}</option>"`;
                }
                select.append(option);
                tag.remove();
            } else if (autocompletar.hasClass('autocompletar-transiciones')) {
                if (!isNewWorkflow) {
                    this.handleDeleteTransition(tag.attr('data-id'), tag, autocompletar);
                } else {
                    tag.remove();
                }
            } else {
                // no eliminamos un estado si ya existe en una transicion
                const transitionTags = parent.find('.autocompletar-transiciones .tag').not('.d-none');;
                if (transitionTags.length < 1) this.removeTag(tag);
                let labelsList = [];
                transitionTags.each((i, transitionTag) => {
                    const transition = $(transitionTag).find('.transition');
                    const labels = transition.find('.transition-label');
                    if (labels.length === 0) this.removeTag(tag);
                    labels.each((j, label) => {
                        labelsList.push(label);
                    });
                });
                const matchFound = labelsList.some(label => {
                    return $(label).attr("title").trim().toLowerCase() === tagValue.trim().toLowerCase();
                });
                if (!matchFound && !isNewWorkflow) {
                    // Comprobamos si afecta alguna recurso
                    this.handleDeleteState(tag.attr('data-id'), tag, parent.find('.autocompletar-transiciones'));
                } else if (!matchFound) {
                    parent.find(`.autocompletar-transiciones select.select-origin option[value="${tag.attr('data-id') }"]`).remove();
                    parent.find(`.autocompletar-transiciones select.select-end option[value="${tag.attr('data-id')}"]`).remove();
                    tag.remove();
                }
                else {
                    mostrarNotificacion("error", "No se puede borrar un estado que pertenece a una transicion");
                }
            }

            //const list = $(e.currentTarget).closest('.tag-list');
            //const inputHidden = autocompletar.children('input[type="hidden"]');
            this.onChecked(autocompletar);
            //this.updateHiddenInput(list, inputHidden);
        });
    },
    editTag: function (e, tag) {
        this.toggleForm(e);

        // rellenar el input con el valor de la tag
        const parent = tag.closest('.autocompletar');
        const list = parent.find('>.tag-list');
        const modalID = parent.parents('.modal-flujo').attr('id');
        // recorremos los usuarios de la tag y los añadimos al formulario
        const formOculto = parent.find('.formulario-oculto');
        !formOculto.hasClass("editando") && formOculto.addClass("editando");
        formOculto.attr('data-id', tag.attr('data-id'));

        // Idioma actual
        const lang = $('#panContenidoMultiIdioma .nav-tabs a[aria-selected="true"]').attr('id').split('_')[1];
        // Obtenemos los inputs multi idioma
        const titleInputs = formOculto.find('.nombre-form-oculto');
        titleInputs.hide();
        // Elegimos el input del idioma actual y le asignamos su valor actual
        const titleInputActive = formOculto.find(`.nombre-form-oculto-${lang}`);
        titleInputActive.val(tag.find('.tag-label').clone().children().remove().end().text().trim());
        // Obtenemos el resto de idiomas y asignamos los valores de la tag su input correspondiente
        const langs = [];
        let context = this;
        $('#panContenidoMultiIdioma .nav-tabs a[aria-selected="false"]').each(function () {
            let otherLang = $(this).attr('id').split('_')[1];
            let input = formOculto.find(`.nombre-form-oculto-${otherLang}`);
            input.val(context.getLanguageValue(tag, otherLang));
        });
        // Mostramos el input con el idioma actual
        titleInputActive.show();

        if (parent.hasClass('autocompletar-estados')) {
            let $datosEstado = tag.find('input[type="hidden"]');
            let color = $datosEstado.data("estado-color");
            // Tipo de estado 
            if ($datosEstado.data("tipo-estado") == 0) {
                formOculto.find('input#estado-inicial').prop('checked', true);
                color = color == "" ? "#80C8F7" : color;
            } else if ($datosEstado.data("tipo-estado") == 1) {
                formOculto.find('input#estado-intermedio').prop('checked', true);
                color = color == "" ? "#FFB74D" : color;
            } else if ($datosEstado.data("tipo-estado") == 2) {
                formOculto.find('input#estado-final').prop('checked', true);
                color = color == "" ? "#94c748" : color;
            }
            // Privacidad de estado
            if ($datosEstado.data("estado-publico")) {
                formOculto.find('input#estado-publico-si').trigger('click');
            } else {
                formOculto.find('input#estado-publico-no').trigger('click');
            }
            // Input Color
            formOculto.find('input[type="color"]').val(color);
        } else if (parent.hasClass('autocompletar-transiciones')) {
            // Hay que cargar las opciones disponibles
            let estados = parent.parents('.tab-content').first().find('.autocompletar-estados .tag-list').children();
            let that = this;
            estados.each(function () {
                let estado = $(this);
                let typeStateValue = estado.find('input[type="hidden"]').attr("data-tipo-estado");
                let id = estado.attr("data-id");
                let tagValue = estado.find(".tag-label").clone().children().remove().end().text().trim();
                let tagValues = estado.attr("title");
                let isPublic = estado.find('input[type="hidden"]').attr("data-estado-publico");
                if (typeStateValue == '0' || typeStateValue == '1') {
                    that.updateOption(modalID, '.select-origin', id, typeStateValue, isPublic, tagValue, null, tagValues);

                    if (typeStateValue == '1') {
                        that.updateOption(modalID, '.select-end', id, typeStateValue, isPublic, tagValue, null, tagValues);
                    }
                } else if (typeStateValue == '2') {
                    that.updateOption(modalID, '.select-end', id, typeStateValue, isPublic, tagValue, null, tagValues);
                }
            });
        }

        tag.find('.users span:not(.user-label)').each((i, elem) => {
            const editors = $(elem).parent().hasClass('editors');
            const editorsList = formOculto.find('.editores .tag-list');
            const readers = $(elem).parent().hasClass('readers');
            const readersList = formOculto.find('.lectores .tag-list');
            const responsibles = $(elem).parent().hasClass('responsibles');
            const responsiblesList = formOculto.find('.responsables .tag-list');

            const tagValue = $(elem).text();
            const tagID = $(elem).attr("id");

            if (editors) editorsList.append(this.addPersonTag(elem));
            if (readers) readersList.append(this.addPersonTag(elem));
            if (responsibles) responsiblesList.append(this.addPersonTag(elem));
        });

        // rellenar selects de estados en transiciones
        const transition = tag.find('.transition');
        const origin = transition.find('.transition-label').first();
        const end = transition.find('.transition-label').last();
        formOculto.find('.select-origin').val(origin.attr('data-id'));
        formOculto.find('.select-end').val(end.attr('data-id'));

        // eliminar la tag
        const inputHidden = parent.parent().children('input[type="hidden"]');
        /*this.removeTag(tag);*/
        tag.addClass("d-none");
        this.updateHiddenInput(list, inputHidden);
    },
    removeTag: function (tag) {
        tag.remove();
    },
    updateHiddenInput: function (list, inputHidden) {
        const tagValues = list.find('.tag').map(function () {
            return $(this).find('input[type="hidden"]').val();
        }).get();
        inputHidden.val(tagValues.join(','));
    },
    formatMultiLangText: function (elements) {
        let text = '';
        elements.each(function () {
            let value = $(this).val();
            let language = $(this).attr('lang')
            if (value) {
                text += `${value}@${language}|||`;
            }
        });

        return text;
    },
    getLanguageValue: function(element, lang) {
        let value = '';
        let $tag = $(element);
        let multiLangValues = $tag.attr('title').split("|||");
        multiLangValues.forEach(element => {
            if (element.includes(`@${lang}`)) {
                value = element.split("@")[0];
            }
        });
        return value;
    },
    updateOption: function (formID, selectClass, id, type, isPublic, tagValue, tagValues, formattedValues) {
        let option = $(`#${formID} ${selectClass} option[value="${id}"]`);
        let dataText = tagValues == undefined ? formattedValues : this.formatMultiLangText(tagValues);
        if (option.length === 0) {
            $(selectClass).append(`<option data-publico="${isPublic}" data-tipo="${type}" data-id="${id}" value="${id}" data-text="${dataText}">${tagValue}</option>`);
        } else {
            option.text(tagValue);
            option.attr("data-text", dataText);
            option.attr("data-tipo", type);
        }
    },
    handleDeleteTransition: function (id, tag, autocompletar) {
        const that = this;
        let dataPost = {
            pTransicionID: id,
            pFlujoID: operativaGestionFlujos.workflowID
        }
        loadingMostrar();
        GnossPeticionAjax(
            operativaGestionFlujos.urlDeleteTransition,
            dataPost,
            true
        ).done(function (data) {
            loadingOcultar();
            tag.addClass("d-none");
            mostrarNotificacion("success", "Transicion marcado como eliminado correctamente, guarda los cambios para aplicar el cambio");
            that.onChecked(autocompletar);
        }).fail(function (data) {
            loadingOcultar();
            mostrarNotificacion("error", data);
        });
    },
    handleDeleteState: function (id, tag, autocompletar) {
        const that = this;
        let dataPost = {
            pEstadoID: id,
            pFlujoID: operativaGestionFlujos.workflowID
        }
        loadingMostrar();
        GnossPeticionAjax(
            operativaGestionFlujos.urlDeleteState,
            dataPost,
            true
        ).done(function (data) {
            loadingOcultar();
            tag.addClass("d-none");
            let deletedTransitions = autocompletar.find('.tag-list .tag.d-none');
            deletedTransitions.each(function () {
                let deletedTransition = $(this);
                deletedTransition.find(`.transition-label[data-id="${id}"]`).attr("data-eliminado", true);
            });
            mostrarNotificacion("success", "Estado marcado eliminado correctamente, guarda los cambios para aplicar el cambio");
            // Hay que eliminar los option del estado marcado como eliminado
            autocompletar.find(`select.select-origin option[value="${id}"]`).remove();
            autocompletar.find(`select.select-end option[value="${id}"]`).remove();
        }).fail(function (data) {
            loadingOcultar();
            mostrarNotificacion("error", data);
        })
    },
    sortTags: function (tags, autocompletar) {
        tags.sort(function (a, b) {
            let titleA = $(a).find('.tag-label').text().toLowerCase();
            let titleB = $(b).find('.tag-label').text().toLowerCase();

            if (autocompletar.hasClass("autocompletar-transiciones")) {
                return titleA.localeCompare(titleB);
            }

            var stateTypeA = parseInt($(a).find('input').data('tipo-estado'));
            var stateTypeB = parseInt($(b).find('input').data('tipo-estado'));

            if (stateTypeA != stateTypeB) {
                return stateTypeA - stateTypeB;
            }

            return titleA.localeCompare(titleB);
        });

        let tagList = autocompletar.find('.tag-list').html('');
        tags.each(function (i, element) {
            tagList.append(element);
        });
    }
};



const calcHeightModalComparar = {
    init: function () {
        this.config();
        if ($(window).width() >= 991) this.exec();
    },
    config: function () {
        this.compararComponentes = $('#modal-comparar-componentes');
        this.compararVersiones = $('#modal-comparar-versiones');
        this.compararEstructura = $('#modal-comparar-estructura');
    },
    exec: function () {
        if (this.compararComponentes.hasClass("show")) {
            this.matchElementHeights(this.compararComponentes, ".comparar-version", ".form-group.contenedorListaIds, .form-group.contenedorPrincipalMailList, .form-group.contenedorPrincipalMenuList");
        }
        if (this.compararVersiones.hasClass("show")) {
            this.matchElementHeights(this.compararVersiones, ".comparar-version", ".form-group.editarFiltroOrden, .form-group.editarExportaciones, .form-group.editarFacetas");
        }
        if (this.compararEstructura.hasClass("show")) {
            this.matchElementHeights(this.compararEstructura, ".comparar-version", ".cmsrow, .cmsrow-content");
        }

        this.compararComponentes.on('hidden.bs.modal', (e) => {
            $(e.currentTarget).find(".comparar-version .form-group").height('auto');
        });

        this.compararVersiones.on('hidden.bs.modal', (e) => {
            $(e.currentTarget).find(".comparar-version .form-group").height('auto');
        });

        this.compararEstructura.on('hidden.bs.modal', (e) => {
            $(e.currentTarget).find(".comparar-version .cmsrow").height('auto');
        });
    },
    matchElementHeights: function (modal, columnClass, itemSelector) {
        const col1 = modal.find(columnClass).eq(0).find(itemSelector);
        const col2 = modal.find(columnClass).eq(1).find(itemSelector);

        const length = Math.min(col1.length, col2.length);

        for (let i = 0; i < length; i++) {
            $(col1[i]).height('auto');
            $(col2[i]).height('auto');

            const height1 = $(col1[i]).height();
            const height2 = $(col2[i]).height();
            const maxHeight = Math.max(height1, height2);

            $(col1[i]).height(maxHeight);
            $(col2[i]).height(maxHeight);
        }
    },
};

const launchTooltip = {
    init: function () {
        $('.btn-tooltip').tooltip();
    },
};

const changeRolePermissions = {
    init: function () {
        this.config();
        this.exec();
    },
    config: function () {
        this.modalNuevo = $('#modal-nuevo-rol');
        this.modalEditar = $('#modal-editar-rol');
    },
    exec: function () {
        //this.modalNuevo.on('shown.bs.modal', this.handleModalShown.bind(this));
        //this.modalEditar.on('shown.bs.modal', this.handleModalShown.bind(this));
    },
    handleModalShown: function (e) {
        this.selectAll(e);
        this.selectRoles(e);
    },
    selectAll: function (evt) {
        const modal = $(evt.currentTarget).attr('id');
        //const cardHeader = $(evt.currentTarget).find('.card-header');        
        const cardHeader = $(`#${modal}`).find('.card-header');
        const checkboxes = cardHeader.find('.custom-checkbox input[type="checkbox"]');

        checkboxes.off('change').on('change', (e) => {
            const all = $(e.currentTarget).closest('.card').find('.card-body input[type="checkbox"]');
            const isChecked = $(e.currentTarget).is(':checked');

            if (all.length) {
                all.each((i, item) => {
                    $(item).prop('checked', isChecked);
                });
            }
        });
    },
    selectRoles: function (evt) {
        const cardBody = $(evt.currentTarget).find('.card-body');
        const checkboxes = cardBody.find('.custom-checkbox input[type="checkbox"]');

        checkboxes.off('change').on('change', (e) => {
            const group = $(e.currentTarget).parents('.custom-control').next('.custom-control__group');
            const isChecked = $(e.currentTarget).is(':checked');

            if (group.length) {
                const childrens = group.find('input[type="checkbox"]');

                childrens.each((i, item) => {
                    $(item).prop('checked', isChecked);
                });
            }

            const allCheckbox = $(e.currentTarget).parents('.card').find('.card-header input[type="checkbox"]');
            const allChecked = checkboxes.length && checkboxes.filter(':checked').length === checkboxes.length;
            allCheckbox.prop('checked', allChecked);
        });
    },
};

$(function () {
    /////// Quitado -> Navegación secciones gestionada via BackEnd
    /////// navegacionSecciones.init();
    navegacionPestanasDropdown.init();
    cambioVistaSeccion.init();

    if ($('body').hasClass('edicionCategorias')) {
        // Se gestiona directamente en la vista
        // CategoriasManagement.init();
    }

    if ($('body').hasClass('edicionCertificados')) {
        // Se gestionan directamente en la vista
        //CertificationsManagement.init();
    }

    // Cargar comportamiento de Facetas (Plegar, Desplegar, Modales)
    if ($("body").hasClass('listado')) {
        comportamientoCargaFacetasComunidad();
    }

    // Cargar comportamiento del menú lateral de usuario para dispositivos móviles
    comportamientoPanelLateralMobile.init();

    // Cargar comportamiento del menú lateral de usuario para ocultar/mostrar
    comportamientoOcultarPanelLateral.init();

    // Iniciar comportamiento de búsqueda desde el header
    comportamientoHeaderSearchable.init();

    // Iniciar comportamientos de plegar/desplegar secciones del menú lateral 
    comportamientoSidebarDropdownSections.init();

    // Iniciar comportamiento DragAndDrop
    dragAndDropGnoss();

    // Iniciar comportamiento de Edición de Páginas. Se hará desde estructura.js, en concreto desde operativaEstructura.operativaPaginasSortable
    /*if ($('body').hasClass('edicionPaginas')) {
        PagesManagement.init();
    }*/

    // Iniciar comportamiento de PageBuilder
    if ($('body').hasClass('page-builder')) {
        PageBuilderManagement.init();
    }

    // Iniciar comportamiento de Componentes CMS (Drag & Drop). Se añadirá mediante uno observer cuando se cargue el modal de "Contenedor de Componentes CMS"
    /*if ($('body').hasClass('edicionComponentes')) {
        ComponentesPageManagement.init();
    }*/

    // Iniciar el comportamiento de los Workflows
    if (body.hasClass('workflows')) {
        autocompletarWorkflows.init();
    }

    // Iniciar el comportamiento para calcular las alturas de los campos de los modales de comparación de páginas y componentes
    //calcHeightModalComparar.init();

    // Iniciar tooltips en permisos
    launchTooltip.init();

    // Iniciar comportamiento del cambio de los checkboxes en los roles
    changeRolePermissions.init();
});
