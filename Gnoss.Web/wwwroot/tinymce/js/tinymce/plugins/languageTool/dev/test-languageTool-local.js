/* test-languageTool-local.js — pruebas del corrector local (diccionario + reglas) */
"use strict";

global.window = global;
const path = require("path");

const pluginDir = path.join(__dirname, "..");

require(path.join(pluginDir, "languageToolDict.es.js"));
require(path.join(pluginDir, "languageToolDict.es.chunk.js"));
require(path.join(pluginDir, "languageToolExtra.es.js"));
require(path.join(pluginDir, "languageToolLocal.js"));

const D = global.LanguageTool.Dict;
const Local = global.LanguageTool.Local;

let passed = 0, failed = 0;
function ok(cond, label) {
    if (cond) { passed++; console.log("OK", label); }
    else { failed++; console.log("FAIL", label); }
}

function isUpper(s) { return s.slice(0).toUpperCase() === s && s.toLowerCase() !== s; }
function isTitleCase(s) {
    const c = s.charAt(0);
    return c === c.toUpperCase() && c !== c.toLowerCase() &&
        s.slice(1) === s.slice(1).toLowerCase();
}

(async function main() {
    ok(D.size === 0, "bootstrap: dict aún sin cargar (perezoso)");
    await Local.ensureDict();
    ok(D.size >= 100000, "chunk cargado: " + D.size + " palabras");

    // Diccionario (fuente RAE expandida: acentos y flexiones)
    ok(D.has("información") === true, "información en el diccionario (lema extra)");
    ok(D.has("informacion") === true, "informacion (sin tilde) aceptado por índice sin acentos");
    ok(D.has("excepción") === true, "excepción en el diccionario");
    ok(D.has("mañana") === true, "mañana en el diccionario (con ñ y acento)");
    ok(D.has("manana") === true, "manana aceptado por índice sin acentos");
    ok(D.has("configurando") === true, "configurando (gerundio regular) sí está");
    ok(D.has("cantando") === true, "cantando (gerundio regular) sí está");
    ok(D.has("boy") === true, "boy es palabra RAE (se marca igual por la lista BAD)");

    // Sugerencias
    const s1 = D.suggest("herror");
    ok(s1.indexOf("error") !== -1, "herror sugiere error -> " + s1.join(", "));
    const s2 = D.suggest("ecxepcion");
    ok(s2.indexOf("excepción") !== -1, "ecxepcion sugiere excepción -> " + s2.join(", "));
    const s3 = D.suggest("informacion");
    ok(s3.indexOf("información") !== -1, "informacion sugiere información -> " + s3.slice(0, 5).join(", "));

    // Checker local: texto con errores
    const text = "Este texto tiene un herror.\nY ecxepcion aqui";
    const matches = await Local.check(text, "es");
    ok(matches.length === 2, "2 errores detectados en texto de prueba (real: " + matches.length + ")");
    ok(matches.every(m => m.offset >= 0 && m.length > 0), "offsets/longitudes válidos en todos los errores");

    // Palabras coloquiales y BAD se marcan aunque estén en el diccionario
    const bad = await Local.check("ke boy xq voy información informaciones", "es");
    ok(bad.length === 3, "ke + boy + xq marcados (información/derivados aceptados) -> " + bad.length);
    ok(bad.some(m => m.ruleId === "LOCAL_BAD" && m.replacements[0] === "que"), "ke -> sugiere 'que'");
    ok(bad.some(m => m.ruleId === "LOCAL_BAD" && m.replacements[0] === "voy"), "boy -> sugiere 'voy'");
    ok(!bad.some(m => m.ruleId === "LOCAL_DICT"), "ningún falso positivo DICT en 'información informaciones'");

    // Texto limpio
    const clean = await Local.check("Este texto esta bien escrito y correcto. La información es configurando, mañana", "es");
    ok(clean.length === 0, "texto limpio: 0 errores (real: " + clean.length + ")");

    // Idioma no español
    const en = await Local.check(text, "en");
    ok(en.length === 0, "idioma 'en' no usa el diccionario local -> 0 errores");

    // Tokenización con offsets
    const tokens = Local.tokenize("hola mundo");
    ok(tokens.length === 2 && tokens[0].offset === 0 && tokens[1].offset === 5, "tokenize con offsets correctos");

    // Respeto de mayúsculas/minúsculas en las sugerencias
    const cs = await Local.check("HERROR herrror AVER boY BOY", "es");
    ok(cs.length === 5, "5 errores en texto de caja (real: " + cs.length + ")");
    const mH = cs[0];
    ok(mH && mH.ruleId === "LOCAL_DICT" && mH.replacements.length > 0 && mH.replacements.every(isUpper),
        "HERROR (todo mayúsculas) -> sugerencias en mayúsculas: " + mH.replacements.join(", "));
    const mh = cs[1];
    ok(mh && mh.ruleId === "LOCAL_DICT" && mh.replacements.length > 0 && mh.replacements.every(r => r === r.toLowerCase()),
        "herror (minúsculas) -> sugerencias sin cambiar");
    const mA = cs[2];
    ok(mA && mA.ruleId === "LOCAL_A_VER_HABER" && mA.replacements[0] === "A VER" && mA.replacements[1] === "HABER",
        "AVER -> 'A VER' y 'HABER'");
    const mBoY = cs[3];
    ok(mBoY && mBoY.ruleId === "LOCAL_BAD" && mBoY.replacements[0] === "voy",
        "boY (mayúscula interna) -> sugerencia sin cambiar");
    const mB = cs[4];
    ok(mB && mB.ruleId === "LOCAL_BAD" && mB.replacements[0] === "VOY",
        "BOY -> sugiere 'VOY'");

    const ts = await Local.check("Herrror Aver", "es");
    const tH = ts[0];
    ok(tH && tH.ruleId === "LOCAL_DICT" && tH.replacements.length > 0 && tH.replacements.every(isTitleCase),
        "Herrror (Title Case) -> sugerencias capitalizadas: " + tH.replacements.join(", "));
    const tA = ts[1];
    ok(tA && tA.ruleId === "LOCAL_A_VER_HABER" && tA.replacements[0] === "A ver" && tA.replacements[1] === "Haber",
        "Aver (Title Case) -> 'A ver' y 'Haber'");

    // Regresión: palabras bien escritas con mayúsculas no se marcan
    const okUpper = await Local.check("ERROR VOY Información", "es");
    ok(okUpper.length === 0, "ERROR/VOY/Información (mayúsculas correctas) no se marcan (real: " + okUpper.length + ")");

    console.log(passed + " OK, " + failed + " FAIL");
    process.exit(failed ? 1 : 0);
})().catch(e => {
    console.error("error inesperado", e);
    process.exit(1);
});