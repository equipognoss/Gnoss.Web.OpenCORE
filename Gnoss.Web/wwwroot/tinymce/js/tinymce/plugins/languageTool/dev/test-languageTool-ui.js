/* test-languageTool-ui.js — pruebas del modal estilo Word (mini-DOM) */
"use strict";

const path = require("path");
global.window = global;
global.document = require("./mini-dom.js").createDocument();

const pluginDir = path.join(__dirname, "..");
require(path.join(pluginDir, "languageToolUi.js"));

const UI = global.LanguageTool.UI;
const { makeElement } = require("./mini-dom.js");

let passed = 0, failed = 0;
function ok(cond, label) {
    if (cond) { passed++; console.log("OK", label); }
    else { failed++; console.log("FAIL", label); }
}

function find(root, cls) {
    const out = [];
    (function walk(node) {
        if (!node || node.nodeType !== 1) return;
        if (node.className && String(node.className).split(/\s+/).indexOf(cls) !== -1) out.push(node);
        node.childNodes.forEach(walk);
    })(root);
    return out;
}

const text = "Este texto tiene un herror y tambien aqui ecxepcion";
const herrorOffset = text.indexOf("herror");
const ecxepcionOffset = text.indexOf("ecxepcion");
const matches = [
    { offset: herrorOffset, length: 6, message: "Posible error ortográfico", replacements: ["error", "horror"], ruleId: "LOCAL_DICT", category: "Ortografía" },
    { offset: ecxepcionOffset, length: 9, message: "Posible error ortográfico", replacements: ["excepción"], ruleId: "LOCAL_DICT", category: "Ortografía" }
];

let applied = null, ignored = null, ignoredAll = null, closed = false;
let idx = 1;
const state = {
    editor: {},
    text: text,
    matches: matches,
    index: () => idx,
    setIndex: i => { idx = i; },
    onApply: (m, r) => { applied = { m, r }; },
    onIgnore: (m) => { ignored = m; },
    onIgnoreAll: (m) => { ignoredAll = m; },
    onClose: () => { closed = true; }
};

const api = UI.openModal(state);

ok(find(global.document.body, "lt-modal").length === 1, "el modal está en el DOM");
ok(find(global.document.body, "lt-overlay").length === 1, "existe el overlay");

// Progreso "Error X de Y"
const progress = find(global.document.body, "lt-progress")[0];
ok(progress && progress.textContent === "Error 2 de 2", "progreso 2 de 2");

// Palabra en contexto
const wordEl = find(global.document.body, "lt-ctx-word")[0];
ok(wordEl && wordEl.textContent === "ecxepcion", "contexto muestra la palabra del error actual");

// Sugerencias
const suggestions = find(global.document.body, "lt-suggestion");
ok(suggestions.length === 1 && suggestions[0].textContent === "excepción", "1 sugerencia del error actual");
ok(find(global.document.body, "lt-suggestion").length === 1, "nº sugerencias del error actual");

// Aceptar aplica la primera sugerencia del error actual
const btnAccept = find(global.document.body, "lt-btn-primary")[0];
btnAccept.trigger("click");
ok(applied && applied.r === "excepción", "Aceptar aplica la primera sugerencia");

// Click en sugerencia
suggestions[0].trigger("click");
ok(!!applied, "click en sugerencia dispara onApply");

// Navegación Anterior
const btnPrev = find(global.document.body, "lt-btn-nav")[0];
btnPrev.trigger("click");
ok(idx === 0, "Anterior navega al primer error");
ok(find(global.document.body, "lt-ctx-word")[0].textContent === "herror", "contexto ahora muestra 'herror'");

// Ignorar (seleccionar por texto para no confundir con Aceptar)
const btnIgnore = find(global.document.body, "lt-btn").filter(b => b.textContent === "Ignorar")[0];
btnIgnore.trigger("click");
ok(ignored === matches[0], "Ignorar invoca onIgnore con el error actual");

// Ignorar todas
const btns = find(global.document.body, "lt-btn");
const btnIgnoreAll = btns.filter(b => b.textContent === "Ignorar todas")[0];
btnIgnoreAll.trigger("click");
ok(ignoredAll === matches[0], "Ignorar todas invoca onIgnoreAll");

// Siguiente
const btnNext = find(global.document.body, "lt-btn-nav")[1];
btnNext.trigger("click");
ok(idx === 1, "Siguiente vuelve al segundo error");

// Cerrar (botón ×)
const closeBtn = find(global.document.body, "lt-modal-close")[0];
closeBtn.trigger("click");
ok(closed === true, "onClose se invoca");
ok(find(global.document.body, "lt-overlay").length === 0, "el overlay se elimina del DOM");

// Texto dinámico: tras corregir, el contexto usa el texto ACTUAL y los nuevos
// offsets (el siguiente error se desplaza por un reemplazo de distinta longitud).
let dynText = "el herror y tambien";
let dynMatches = [
    { offset: 3, length: 6, message: "Posible error ortográfico", replacements: ["error"], ruleId: "LOCAL_DICT" },
    { offset: 12, length: 7, message: "Posible error ortográfico", replacements: ["también"], ruleId: "LOCAL_DICT" }
];
let dynIdx = 0;
const dynState = {
    editor: {},
    text: () => dynText,
    matches: dynMatches,
    index: () => dynIdx,
    setIndex: i => { dynIdx = i; }
};
const dynApi = UI.openModal(dynState);
ok(find(global.document.body, "lt-ctx-word")[0].textContent === "herror", "modal con getter: muestra el primer error");

// "herror" (6) se corrige por "error" (5): el texto cambia de longitud y
// "tambien" (que estaba en 11) pasa a estar en 10.
dynText = "el error y tambien";
dynMatches.splice(0, dynMatches.length, { offset: 11, length: 7, message: "Posible error ortográfico", replacements: ["también"], ruleId: "LOCAL_DICT" });
dynApi.render();
ok(find(global.document.body, "lt-ctx-word")[0].textContent === "tambien", "getter: tras corregir muestra el siguiente error con el offset desplazado");
ok(find(global.document.body, "lt-ctx-before")[0].textContent.indexOf("el error y") !== -1, "getter: el contexto pinta el texto corregido");
ok(find(global.document.body, "lt-progress")[0].textContent === "Error 1 de 1", "getter: el progreso se recalcula tras la corrección");
const navBtns = find(global.document.body, "lt-btn-nav");
ok(navBtns[0].disabled === true && navBtns[1].disabled === true, "getter: con 1 error restante, Anterior/Siguiente deshabilitados");
dynApi.close();
ok(find(global.document.body, "lt-overlay").length === 0, "el segundo modal se cierra y se limpia el DOM");

// Regresión: el plugin ESTA VEZ reasigna state.matches a un array NUEVO tras
// cada re-check (no muta el mismo). El modal debe renderizar contra el array
// vigente (via getter) o el "Error X de N" se queda en el total antiguo.
let swapText = "me boy de bacaciones a la plalla";
let swapMatches = [
    { offset: 3, length: 3, message: "¿Querías decir 'voy'?", replacements: ["voy"], ruleId: "LOCAL_DICT" },
    { offset: 10, length: 10, message: "Posible error", replacements: ["vacaciones"], ruleId: "LOCAL_DICT" },
    { offset: 26, length: 6, message: "Posible error", replacements: ["playa"], ruleId: "LOCAL_DICT" }
];
let swapIdx = 0;
const swapState = {
    editor: {},
    text: () => swapText,
    matches: () => swapMatches,
    index: () => swapIdx,
    setIndex: i => { swapIdx = i; },
    onApply: () => {},
    onClose: () => {}
};
const swapApi = UI.openModal(swapState);
ok(find(global.document.body, "lt-progress")[0].textContent === "Error 1 de 3", "swap: arranca en Error 1 de 3");
ok(find(global.document.body, "lt-ctx-word")[0].textContent === "boy", "swap: primer error es 'boy'");

// Simula applySuggestion: se corrige boy->voy, el texto cambia y el re-check
// devuelve un array NUEVO con 2 errores (el plugin lo sustituye por completo).
swapText = "me voy de bacaciones a la plalla";
swapMatches = [
    { offset: 10, length: 10, message: "Posible error", replacements: ["vacaciones"], ruleId: "LOCAL_DICT" },
    { offset: 26, length: 6, message: "Posible error", replacements: ["playa"], ruleId: "LOCAL_DICT" }
];
swapIdx = 0;
swapApi.render();
ok(find(global.document.body, "lt-progress")[0].textContent === "Error 1 de 2", "swap: el progreso avanza contra el array NUEVO (1 de 2)");
ok(find(global.document.body, "lt-ctx-word")[0].textContent === "bacaciones", "swap: muestra el siguiente error (ya no 'boy')");
ok(find(global.document.body, "lt-ctx-before")[0].textContent.indexOf("me voy de") !== -1, "swap: el contexto pinta el texto corregido");
swapApi.close();

console.log(passed + " OK, " + failed + " FAIL");
process.exit(failed ? 1 : 0);