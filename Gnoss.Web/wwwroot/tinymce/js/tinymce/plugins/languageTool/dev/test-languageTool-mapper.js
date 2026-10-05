/* test-languageTool-mapper.js — pruebas del marcado de errores en el editor (mini-DOM) */
"use strict";

global.window = global;
global.document = require("./mini-dom.js").createDocument();
const path = require("path");

const pluginDir = path.join(__dirname, "..");

require(path.join(pluginDir, "languageToolDict.es.js"));
require(path.join(pluginDir, "languageToolDict.es.chunk.js"));
require(path.join(pluginDir, "languageToolLocal.js"));
require(path.join(pluginDir, "languageToolMapper.js"));

const Mapper = global.LanguageTool.Mapper;
const Local = global.LanguageTool.Local;
const { makeElement, makeText } = require("./mini-dom.js");

let passed = 0, failed = 0;
function ok(cond, label) {
    if (cond) { passed++; console.log("OK", label); }
    else { failed++; console.log("FAIL", label); }
}

function countMarkers(root) {
    let n = 0;
    (function walk(node) {
        if (!node || node.nodeType !== 1) return;
        if (node.className && String(node.className).split(/\s+/).indexOf("lt-marker") !== -1) n++;
        node.childNodes.forEach(walk);
    })(root);
    return n;
}

// Editor simulado: <p>Este texto tiene un <strong>herror</strong>.<br><p>Y ecxepcion aqui</p>
function buildEditor() {
    const body = makeElement("BODY");
    const p1 = makeElement("P");
    p1.appendChild(makeText("Este texto tiene un "));
    const strong = makeElement("STRONG");
    strong.appendChild(makeText("herror"));
    p1.appendChild(strong);
    p1.appendChild(makeText("."));
    body.appendChild(p1);
    const p2 = makeElement("P");
    p2.appendChild(makeText("Y ecxepcion aqui"));
    body.appendChild(p2);
    return { getBody: function () { return body; } };
}

// Nota: entre <p> no hay \n en el texto extraído (los bloques se concatenan).
const EXPECTED = "Este texto tiene un herror.Y ecxepcion aqui";

const editor = buildEditor();
const extracted = Mapper.extractText(editor);
ok(extracted.text === EXPECTED, "extractText === texto esperado");
ok(extracted.nodes.length >= 4, "varios nodos de texto indexados: " + extracted.nodes.length);

Local.ensureDict().then(() => Local.check(EXPECTED, "es")).then(matches => {
    ok(matches.length === 2, "checker local encuentra 2 errores");

    const n = Mapper.markMatches(editor, matches);
    ok(n === 2, "markMatches marca 2 errores");
    ok(countMarkers(editor.getBody()) === 2, "2 spans .lt-marker en el DOM");

    // El texto plano debe permanecer idéntico tras marcar
    const extracted2 = Mapper.extractText(editor);
    ok(extracted2.text === EXPECTED, "el texto plano se mantiene idéntico tras marcar");

    // Info en los marcadores
    const markers = [];
    (function walk(node) {
        if (!node || node.nodeType !== 1) return;
        if (node.className && String(node.className).split(/\s+/).indexOf("lt-marker") !== -1) markers.push(node);
        node.childNodes.forEach(walk);
    })(editor.getBody());
    ok(markers[0].getAttribute("data-lt-rule") === "LOCAL_DICT", "marcador trae data-lt-rule");
    ok(markers[0].getAttribute("data-lt-offset") !== null, "marcador trae data-lt-offset");

    // Sustituir "herror" por "error"
    const m0 = matches.find(m => { const w = EXPECTED.slice(m.offset, m.offset + m.length); return w.toLowerCase() === "herror"; });
    ok(!!m0, "match de herror localizado");
    Mapper.replaceAt(editor, m0.offset, m0.length, "error");
    const textAfter = Mapper.extractText(editor).text;
    ok(textAfter === EXPECTED.replace("herror", "error"), "replaceAt sustituye herror -> error: " + textAfter.split("\n")[0]);

    // clearMatches deja el DOM sin marcadores
    Mapper.clearMatches(editor);
    ok(countMarkers(editor.getBody()) === 0, "clearMatches elimina los marcadores");

    console.log(passed + " OK, " + failed + " FAIL");
    process.exit(failed ? 1 : 0);
});