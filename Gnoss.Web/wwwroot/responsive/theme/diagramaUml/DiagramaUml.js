import mermaid from "https://cdn.jsdelivr.net/npm/mermaid@10/dist/mermaid.esm.min.mjs";

const config = {
    startOnLoad: true,
    securityLevel: 'loose',
    themeVariables: {
        fontSize: '12px',
        nodeSpacing: 50,
        edgeLength: 100,
        arrowMarkerAbsolute: true,
        diagramPadding: 50,
    },
};

mermaid.initialize(config);

document.addEventListener("DOMContentLoaded", function () {
    if (typeof window.jsonData !== 'undefined') {
        const container = document.getElementById("mermaid-container");
        const botonDescargar = document.getElementById("download-diagram");
        const botonEditar = document.getElementById("edit-diagram");

        const datosClases = window.jsonData.Clases;
        const datosPropiedades = window.jsonData.Propiedades;
        var datosRelaciones = window.jsonData.Relaciones;

        const propiedadesClases = {};
        const claseURIMap = {};

        const ontologias;
        const nombresOntologias = {};

        datosClases.forEach(item => {
            ontologias[item.Ontologia] = item.Ontologia;

            if (!nombresOntologias[item.Ontologia]) {
                nombresOntologias[item.Ontologia] = [];
            }

            const nombreClase = item.NombreClase.includes('#')
                ? item.NombreClase.split('#').pop()
                : item.NombreClase.split('/').pop();

            nombresOntologias[item.Ontologia].push(nombreClase);

            if (!propiedadesClases[nombreClase]) {
                propiedadesClases[nombreClase] = [];
            }

            claseURIMap[nombreClase] = item.NombreClase;
        });

        datosPropiedades.forEach(prop => {
            const nombrePropiedad = prop.NombrePropiedad.includes('#') ? prop.NombrePropiedad.split('#').pop() : prop.NombrePropiedad.split('/').pop();
            const dominio = prop.Dominio.includes('#') ? prop.Dominio.split('#').pop() : prop.Dominio.split('/').pop();

            if (propiedadesClases[dominio]) {
                propiedadesClases[dominio].push({
                    NombrePropiedad: nombrePropiedad,
                    Cardinalidad: prop.Cardinalidad,
                    Rango: prop.Rango.includes('#') ? prop.Rango.split('#').pop() : prop.Rango.split('/').pop(),
                });
            }
        });

        function generateMermaidCode() {
            let mermaidCode = 'classDiagram\n';
            mermaidCode += 'direction LR\n';

            Object.entries(nombresOntologias).forEach(([ontologia, clases]) => {
                mermaidCode += `namespace ${ontologia} {\n`;

                clases.forEach(nombreClase => {
                    if (propiedadesClases[nombreClase]) {
                        mermaidCode += `class ${nombreClase} {\n`;
                        if (propiedadesClases[nombreClase].length === 0) {
                            mermaidCode += `    \n`;
                        } else {
                            propiedadesClases[nombreClase].forEach(prop => {
                                mermaidCode += `    ${prop.NombrePropiedad}[${prop.Cardinalidad}]: ${prop.Rango}\n`;
                            });
                        }
                        mermaidCode += '}\n';
                    }
                });

                mermaidCode += '}\n';
            });

            datosRelaciones.forEach(relacion => {
                const origen = relacion.Origen.includes('#') ? relacion.Origen.split('#').pop() : relacion.Origen.split('/').pop();
                const destino = relacion.Destino.includes('#') ? relacion.Destino.split('#').pop() : relacion.Destino.split('/').pop();

                if (relacion.Tipo === 'Herencia') {
                    mermaidCode += `${origen} --|> ${destino}\n`;
                } else {
                    mermaidCode += `${origen} --> ${destino}\n`
                }
            });

            return mermaidCode;
        }

        const modal = document.createElement("div");
        modal.style.position = "fixed";
        modal.style.top = "50%";
        modal.style.left = "50%";
        modal.style.transform = "translate(-50%, -50%)";
        modal.style.backgroundColor = "white";
        modal.style.padding = "20px";
        modal.style.border = "1px solid #ccc";
        modal.style.boxShadow = "0 0 10px rgba(0, 0, 0, 0.1)";
        modal.style.zIndex = "1000";
        modal.style.display = "none";
        document.body.appendChild(modal);

        renderDiagram();

        function renderDiagram() {
            const mermaidCode = generateMermaidCode();
            container.innerHTML = `<pre class="mermaid">${mermaidCode}</pre>`;
            mermaid.contentLoaded();
        }

        function editDiagram() {
            const classTitleElement = this.querySelector('foreignObject.classTitle span.nodeLabel');
            if (classTitleElement) {
                const className = classTitleElement.textContent.trim();
                const properties = propiedadesClases[className] || [];
                const propiedadesOriginales = JSON.parse(JSON.stringify(properties));

                modal.innerHTML = `
                    <div id=error-message>
                        <h4>Error:</h4>
                    </div>
                    <h3>Editar clase: ${className}</h3>
                    <div>
                        <label>Nombre de la clase:</label>
                        <input type="text" id="class-name" value="${className}" />
                    </div>
                    <div id="attributes-container">
                        <h4>Propiedades:</h4>
                    </div>
                    <button id="add-property">Agregar propiedad</button>
                    <button id="save-changes">Guardar cambios</button>
                    <button id="close-modal">Cerrar</button>
                `;

                const classNameInput = modal.querySelector("#class-name");
                const attributesContainer = modal.querySelector("#attributes-container");
                const errorMessageDiv = modal.querySelector("#error-message");
                errorMessageDiv.style.display = "none";

                function renderProperties(props) {
                    attributesContainer.innerHTML = '<h4>Propiedades:</h4>';
                    props.forEach((prop, index) => {
                        if (prop && prop.NombrePropiedad && prop.Cardinalidad && prop.Rango) {
                            const propertyRow = document.createElement("div");
                            propertyRow.className = "property-row";
                            propertyRow.setAttribute("data-index", index.toString());
                            propertyRow.innerHTML = `
                                <label>Nombre:</label>
                                <input type="text" value="${prop.NombrePropiedad}" data-field="NombrePropiedad" />
                                <label>Cardinalidad:</label>
                                <input type="text" value="${prop.Cardinalidad}" data-field="Cardinalidad" />
                                <label>Rango:</label>
                                <input type="text" value="${prop.Rango}" data-field="Rango" />
                                <button class="delete-property">Eliminar</button>
                            `;
                            attributesContainer.appendChild(propertyRow);
                        } else {
                            console.warn('Propiedad inválida encontrada al mostrar en modal:', prop);
                        }
                    });
                }

                renderProperties(properties);

                function collectProperties() {
                    const propertyRows = attributesContainer.querySelectorAll(".property-row");
                    const collectedProperties = [];
                    let isValid = true;
                    let errorMessage = '';

                    propertyRows.forEach((row, index) => {
                        const inputs = row.querySelectorAll("input");
                        const property = {};
                        let hasEmptyField = false;
                        let emptyFields = [];

                        inputs.forEach(input => {
                            const field = input.getAttribute("data-field");
                            const value = input.value.trim();
                            property[field] = value;
                            if (value === '') {
                                hasEmptyField = true;
                                emptyFields.push(field);
                            }
                        });

                        if (hasEmptyField) {
                            isValid = false;
                            errorMessage += `Propiedad ${index + 1} tiene campos vacíos: ${emptyFields.join(', ')}. `;
                        } else if (property.NombrePropiedad && property.Cardinalidad && property.Rango) {
                            collectedProperties.push(property);
                        } else {
                            isValid = false;
                            errorMessage += `Propiedad ${index + 1} está incompleta. `;
                        }
                    });

                    return {
                        isValid,
                        properties: collectedProperties,
                        errorMessage: errorMessage.trim()
                    };
                }

                const addPropertyButton = modal.querySelector("#add-property");
                addPropertyButton.addEventListener("click", () => {
                    const result = collectProperties();

                    if (!result.isValid) {
                        errorMessageDiv.textContent = result.errorMessage;
                        errorMessageDiv.style.display = "block";
                        return;
                    }

                    errorMessageDiv.style.display = "none";
                    let currentProperties = Array.isArray(result.properties) ? result.properties : [];

                    const newProp = {
                        NombrePropiedad: "NuevaPropiedad",
                        Cardinalidad: "1",
                        Rango: "string"
                    };
                    currentProperties.push(newProp);

                    renderProperties(currentProperties);
                });

                attributesContainer.addEventListener("click", (e) => {
                    if (e.target.classList.contains("delete-property")) {
                        const result = collectProperties();
                        const currentProperties = result.properties;
                        const row = e.target.closest(".property-row");
                        const index = parseInt(row.getAttribute("data-index"));
                        currentProperties.splice(index, 1);
                        renderProperties(currentProperties);
                        errorMessageDiv.style.display = "none";
                    }
                });

                const saveButton = modal.querySelector("#save-changes");
                saveButton.addEventListener("click", () => {
                    const nuevoNombreClase = classNameInput.value.trim();
                    const result = collectProperties();

                    if (!nuevoNombreClase) {
                        errorMessageDiv.textContent = "El nombre de la clase no puede estar vacío.";
                        errorMessageDiv.style.display = "block";
                        return;
                    }

                    if (!result.isValid) {
                        errorMessageDiv.textContent = result.errorMessage;
                        errorMessageDiv.style.display = "block";
                        return;
                    }

                    const nuevasPropiedades = result.properties;
                    errorMessageDiv.style.display = "none";

                    const propiedadesAñadidas = [];
                    const propiedadesModificadas = [];
                    const propiedadesEliminadas = [];

                    nuevasPropiedades.forEach(nuevaProp => {
                        const existe = propiedadesOriginales.find(origProp =>
                            origProp.NombrePropiedad === nuevaProp.NombrePropiedad &&
                            origProp.Cardinalidad === nuevaProp.Cardinalidad &&
                            origProp.Rango === nuevaProp.Rango
                        );
                        if (!existe) {
                            const propOriginal = propiedadesOriginales.find(origProp =>
                                origProp.NombrePropiedad === nuevaProp.NombrePropiedad
                            );
                            if (propOriginal) {
                                propiedadesModificadas.push({
                                    original: propOriginal,
                                    nueva: nuevaProp
                                });
                            } else {
                                propiedadesAñadidas.push(nuevaProp);
                            }
                        }
                    });

                    propiedadesOriginales.forEach(origProp => {
                        const existe = nuevasPropiedades.find(nuevaProp =>
                            nuevaProp.NombrePropiedad === origProp.NombrePropiedad
                        );
                        if (!existe) {
                            propiedadesEliminadas.push(origProp);
                        }
                    });

                    propiedadesAñadidas.forEach(prop => {
                        if (prop.Rango in propiedadesClases) {
                            const origenURI = claseURIMap[className];
                            const destinoURI = claseURIMap[prop.Rango];
                            const nuevaRelacion = {
                                Origen: origenURI,
                                Destino: destinoURI,
                                Tipo: "Asociacion"
                            };

                            const relacionExistente = datosRelaciones.some(rel =>
                                rel.Origen === nuevaRelacion.Origen &&
                                rel.Destino === nuevaRelacion.Destino &&
                                rel.Tipo === nuevaRelacion.Tipo
                            );

                            if (!relacionExistente) {
                                datosRelaciones.push(nuevaRelacion);
                            }
                        }
                    });

                    propiedadesModificadas.forEach(mod => {
                        const { original, nueva } = mod;

                        if (nueva.Rango !== original.Rango && nueva.Rango in propiedadesClases) {
                            const origenURI = claseURIMap[className];
                            const destinoURI = claseURIMap[nueva.Rango];
                            const nuevaRelacion = {
                                Origen: origenURI,
                                Destino: destinoURI,
                                Tipo: "Asociacion"
                            };

                            const relacionExistente = datosRelaciones.some(rel =>
                                rel.Origen === nuevaRelacion.Origen &&
                                rel.Destino === nuevaRelacion.Destino &&
                                rel.Tipo === nuevaRelacion.Tipo
                            );

                            if (!relacionExistente) {
                                datosRelaciones.push(nuevaRelacion);
                            }
                        }

                        if (original.Rango in propiedadesClases && original.Rango !== nueva.Rango) {
                            const origenURI = claseURIMap[className];
                            const destinoURI = claseURIMap[original.Rango];
                            datosRelaciones = datosRelaciones.filter(rel =>
                                !(rel.Origen === origenURI &&
                                    rel.Destino === destinoURI &&
                                    rel.Tipo === "Asociacion")
                            );
                        }
                    });

                    propiedadesEliminadas.forEach(prop => {
                        if (prop.Rango in propiedadesClases) {
                            const origenURI = claseURIMap[className];
                            const destinoURI = claseURIMap[prop.Rango];
                            datosRelaciones = datosRelaciones.filter(rel =>
                                !(rel.Origen === origenURI &&
                                    rel.Destino === destinoURI &&
                                    rel.Tipo === "Asociacion")
                            );
                        }
                    });


                    if (nuevoNombreClase !== className) {
                        propiedadesClases[nuevoNombreClase] = nuevasPropiedades;
                        delete propiedadesClases[className];

                        Object.keys(propiedadesClases).forEach(clase => {
                            propiedadesClases[clase] = propiedadesClases[clase].map(prop => {
                                if (prop.Rango === className) {
                                    return {
                                        ...prop,
                                        Rango: nuevoNombreClase
                                    };
                                }
                                return prop;
                            });
                        });

                        // Actualizar claseURIMap
                        const uriOriginal = claseURIMap[className];
                        delete claseURIMap[className];
                        claseURIMap[nuevoNombreClase] = uriOriginal.replace(className, nuevoNombreClase);


                        const relacionesActualizadas = datosRelaciones.map(relacion => {
                            if (relacion && typeof relacion === 'object' && typeof relacion.Origen === 'string' && typeof relacion.Destino === 'string' && typeof relacion.Tipo === 'string') {
                                const origen = relacion.Origen.includes('#') ? relacion.Origen.split('#').pop() : relacion.Origen.split('/').pop();
                                const destino = relacion.Destino.includes('#') ? relacion.Destino.split('#').pop() : relacion.Destino.split('/').pop();

                                const relacionActualizada = {
                                    Origen: relacion.Origen,
                                    Destino: relacion.Destino,
                                    Tipo: relacion.Tipo
                                };

                                if (origen === className) {
                                    const partesOrigen = relacion.Origen.split('#');
                                    if (partesOrigen.length > 1) {
                                        relacionActualizada.Origen = partesOrigen[0] + "#" + nuevoNombreClase;
                                    } else {
                                        const partesSlash = relacion.Origen.split('/');
                                        const nuevasPartesOrigen = [];
                                        for (let i = 0; i < partesSlash.length - 1; i++) {
                                            nuevasPartesOrigen.push(partesSlash[i]);
                                        }
                                        nuevasPartesOrigen.push(nuevoNombreClase);
                                        relacionActualizada.Origen = nuevasPartesOrigen.join('/');
                                    }
                                }

                                if (destino === className) {
                                    const partesDestino = relacion.Destino.split('#');
                                    if (partesDestino.length > 1) {
                                        relacionActualizada.Destino = partesDestino[0] + "#" + nuevoNombreClase;
                                    } else {
                                        const partesSlash = relacion.Destino.split('/');
                                        const nuevasPartesDestino = [];
                                        for (let i = 0; i < partesSlash.length - 1; i++) {
                                            nuevasPartesDestino.push(partesSlash[i]);
                                        }
                                        nuevasPartesDestino.push(nuevoNombreClase);
                                        relacionActualizada.Destino = nuevasPartesDestino.join('/');
                                    }
                                }

                                return relacionActualizada;
                            } else {
                                console.warn('Relación inválida encontrada al actualizar:', relacion);
                                return null;
                            }
                        });

                        datosRelaciones = relacionesActualizadas.filter(relacion =>
                            relacion &&
                            typeof relacion === 'object' &&
                            typeof relacion.Origen === 'string' &&
                            typeof relacion.Destino === 'string' &&
                            typeof relacion.Tipo === 'string'
                        );
                    } else {
                        propiedadesClases[className] = nuevasPropiedades;
                    }

                    modal.style.display = "none";
                    renderDiagram();

                    setTimeout(function () {
                        const classElements = container.querySelectorAll('g.node.default');
                        classElements.forEach(classElement => {
                            classElement.addEventListener('click', editDiagram);
                            classElement.addEventListener('mouseover', over, false);
                            classElement.addEventListener('mouseout', out, false);
                        });
                    }, 100);
                });

                const closeButton = modal.querySelector("#close-modal");
                closeButton.addEventListener("click", () => {
                    modal.style.display = "none";
                });

                modal.style.display = "block";
            }
        }

        botonDescargar.addEventListener("click", function () {
            const svgElement = container.querySelector("svg");
            if (!svgElement) {
                alert("El diagrama aún no está disponible.");
                return;
            }
            const serializer = new XMLSerializer();
            let source = serializer.serializeToString(svgElement);
            const blob = new Blob([source], { type: "image/svg+xml;charset=utf-8" });
            const url = URL.createObjectURL(blob);
            const a = document.createElement("a");
            a.href = url;
            a.download = "diagrama.svg";
            document.body.appendChild(a);
            a.click();
            document.body.removeChild(a);
            URL.revokeObjectURL(url);
        });

        function over() {
            var rect = this.querySelector("rect");
            rect.setAttribute("style", "fill: #b7a3e3");
        }
        function out() {
            var rect = this.querySelector("rect");
            rect.removeAttribute("style");
        }

        botonEditar.addEventListener("click", function () {
            var svgElement = document.querySelector("svg").innerHTML;

            var contenedorBotones = document.getElementById("contenedorBotones");
            var botonGuardar = document.getElementById("boton-guardar");
            botonEditar.style.visibility = "hidden";

            if (!botonGuardar) {
                botonGuardar = document.createElement("button");
                botonGuardar.setAttribute("class", "btn btn-primary")
                botonGuardar.setAttribute("id", "boton-guardar");
                botonGuardar.innerHTML = 'Guardar';
                contenedorBotones.appendChild(botonGuardar);
            }

            botonDescargar.style.visibility = "hidden";
            const classElements = container.querySelectorAll('g.node.default');
            classElements.forEach(classElement => {
                classElement.addEventListener('click', editDiagram);
                classElement.addEventListener('mouseover', over, false);
                classElement.addEventListener('mouseout', out, false);
            });


            botonGuardar.addEventListener("click", function () {
                contenedorBotones.removeChild(botonGuardar);
                botonEditar.style.visibility = "visible";
                botonDescargar.style.visibility = "visible";
                setTimeout(function () {
                    const classElementsGuardar = container.querySelectorAll('g.node.default');
                    classElementsGuardar.forEach(classElementG => {
                        classElementG.removeEventListener('click', editDiagram);
                        classElementG.removeEventListener('mouseover', over, false);
                        classElementG.removeEventListener('mouseout', out, false);
                    });
                }, 100);
            });
            
        });
    }
});
