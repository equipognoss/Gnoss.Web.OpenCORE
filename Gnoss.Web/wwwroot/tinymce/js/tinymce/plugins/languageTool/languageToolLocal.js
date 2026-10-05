/* languageToolLocal.js
 * Corrector local basado en el diccionario embebido (languageToolDict.es.js).
 * Sin servidor: funciona abriendo el HTML directamente en el navegador.
 * API:
 *   Local.check(text, language) -> Promise<matches[]>
 *   Local.tokenize(text)        -> [{word, offset, length}]
 */
(function (global) {
    "use strict";

    const ns = (global.LanguageTool = global.LanguageTool || {});

    // Palabras extra (aceptadas) y "BAD" (siempre error coloquial) definidas en
    // languageToolExtra.es.js. Se usa lo que exista; son opcionales.
    const EXTRA_WORDS = (ns.Extra && ns.Extra.WORDS) || new Set();
    const BAD = (ns.Extra && ns.Extra.BAD) || {};

    // Palabras técnicas / propias del proyecto o de la demo que no están en el
    // diccionario pero se consideran correctas (sin tildes, en minúscula).
    const EXCEPT = new Set([
        "lorem", "ipsum", "dolor", "sit", "amet", "consectetur", "adipiscing",
        "elit", "gnoss", "moodle", "latex", "tiny", "mce", "tinymce",
        "codeeditor", "toolbar", "wordpress", "github", "http", "https", "url",
        "ckeditor", "textarea"
    ].map(w => w.toLowerCase()));

    // Conjunciones/artículos que pueden repetirse de forma válida en texto.
    const REPEAT_ALLOWED = new Set([
        "el", "la", "los", "las", "de", "del", "a", "al", "que", "en", "y",
        "con", "por", "para", "como", "un", "una", "o", "e", "u", "ni", "su",
        "sus", "sin", "no", "es", "se", "lo", "le", "más", "mas"
    ].map(w => w.toLowerCase()));

    const TOKEN_RE = /[\p{L}\p{M}]+/gu;

    // URL del chunk perezoso del diccionario (lo fija el plugin con
    // Local.setDictUrl). Vacía: solo se usa el dict ya inyectado (tests/demo).
    let DICT_URL = '';

    function ensureDict() {
        const dict = ns.Dict;
        if (!dict || !dict.load || dict.ready) return Promise.resolve();
        return dict.load(DICT_URL);
    }

    function setDictUrl(url) {
        DICT_URL = url || '';
    }

    function tokenize(text) {
        const tokens = [];
        let m;
        while ((m = TOKEN_RE.exec(text)) !== null) {
            if (m[0].length > 0) {
                tokens.push({ word: m[0], offset: m.index, length: m[0].length });
            }
        }
        return tokens;
    }

    function isAcronym(word) {
        if (word.length <= 1) return false;
        let hasUpper = false, hasLower = false;
        for (const ch of word) {
            if (/[A-ZÁÉÍÓÚÜÑ]/.test(ch)) hasUpper = true;
            else if (/[a-záéíóúüñ]/.test(ch)) hasLower = true;
        }
        return hasUpper && !hasLower && word.length > 1;
    }

    /* Respeta la caja de la palabra mal escrita al proponer sustituciones:
     * - Toda MAYÚSCULA ("HERROR") -> sugerencia en mayúsculas ("ERROR"),
     *   funcionando con acentos y ñ gracias a toUpperCase() nativo.
     * - Title Case ("Herrror", "Aver") -> se capitaliza el primer carácter.
     * - El resto (minúsculas, mixta interna "boY") -> se deja tal cual. */
    function applyWordCase(original, suggestion) {
        if (!suggestion || !original) return suggestion;
        if (original === original.toUpperCase() && original !== original.toLowerCase()) {
            return suggestion.toUpperCase();
        }
        const first = original.charAt(0);
        if (first === first.toUpperCase() && first !== first.toLowerCase() &&
            original.slice(1) === original.slice(1).toLowerCase()) {
            return suggestion.charAt(0).toUpperCase() + suggestion.slice(1);
        }
        return suggestion;
    }

    /* Acepta sustantivos en -ción/-ciones derivados de un verbo que sí esté en
     * el diccionario ("información" -> "informar"). Cubre también la grafía sin
     * tilde gracias a Dict.has (índice sin acentos). */
    function isCionDerived(word, D) {
        let stem = null;
        if (word.endsWith("ciones")) stem = word.slice(0, -6);
        else if (word.endsWith("ción") || word.endsWith("cion")) stem = word.slice(0, -4);
        else if (word.endsWith("siones")) stem = word.slice(0, -6);
        else if (word.endsWith("sión") || word.endsWith("sion")) stem = word.slice(0, -4);
        if (!stem || stem.length < 4) return false;
        return D.has(stem + "r") || D.has(stem + "ar") || D.has(stem + "er") || D.has(stem + "ir");
    }

    function check(text, language) {
        return ensureDict().then(() => {
            const lang = language ? String(language).toLowerCase() : "es";
            if (lang !== "es") return [];
            const D = ns.Dict;
            if (!D || !D.has) return [];
            if (!D.ready) return [];
            const tokens = tokenize(String(text || ""));
            const matches = [];

            for (let i = 0; i < tokens.length; i++) {
                const t = tokens[i];
                const lower = t.word.toLowerCase();

                // Coloquialismos/SMS/BAD: se marcan SIEMPRE, aunque estén en el
                // diccionario (o aunque sean palabras correctas mal usadas).
                if (Object.prototype.hasOwnProperty.call(BAD, lower)) {
                    const repl = (BAD[lower] || []).map(function (r) { return applyWordCase(t.word, r); });
                    matches.push({
                        offset: t.offset,
                        length: t.length,
                        message: "Forma coloquial; se recomienda " + (repl.length ? "el correcto" : "revisar la palabra"),
                        replacements: repl,
                        ruleId: "LOCAL_BAD",
                        category: "Estilo",
                        ruleDescription: "Palabra coloquial no recomendada"
                    });
                    continue;
                }

                if (lower === "aver") {
                    const repl = ["a ver", "haber"].map(function (r) { return applyWordCase(t.word, r); });
                    matches.push({
                        offset: t.offset,
                        length: t.length,
                        message: "Confusión: ¿\"a ver\" o \"haber\"?",
                        replacements: repl,
                        ruleId: "LOCAL_A_VER_HABER",
                        category: "Ortografía",
                        ruleDescription: "Confusión 'aver'"
                    });
                    continue;
                }

                // Una palabra toda en MAYÚSCULAS se trata como sigla/acrónimo
                // solo si no parece una falta ortográfica (no hay sugerencias
                // cercanas): "HERROR" se marca y "RENFE"/"HTML" se dejan pasar.
                if (isAcronym(t.word) && D.suggest(lower).length === 0) continue;
                if (lower.length <= 1) continue;
                if (EXCEPT.has(lower)) continue;

                // Conocida: en el diccionario, en las palabras extra o derivada
                // morfológicamente (sustantivos en -ción de verbos).
                if (D.has(lower) || EXTRA_WORDS.has(lower) || isCionDerived(lower, D)) {
                    if (i > 0) {
                        const prev = tokens[i - 1].word.toLowerCase();
                        if (prev === lower && !REPEAT_ALLOWED.has(lower)) {
                            matches.push({
                                offset: t.offset,
                                length: t.length,
                                message: "Palabra repetida",
                                replacements: [],
                                ruleId: "LOCAL_REPEATED",
                                category: "Estilo",
                                ruleDescription: "Palabra repetida consecutivamente"
                            });
                        }
                    }
                    continue;
                }

                const repl = D.suggest(lower).slice(0, 6).map(function (r) { return applyWordCase(t.word, r); });
                if (repl.length > 0) {
                    matches.push({
                        offset: t.offset,
                        length: t.length,
                        message: "Posible error ortográfico",
                        replacements: repl,
                        ruleId: "LOCAL_DICT",
                        category: "Ortografía",
                        ruleDescription: "Palabra no encontrada en el diccionario"
                    });
                }
            }
            return matches;
        });
    }

    ns.Local = {
        check,
        tokenize,
        EXCEPT,
        REPEAT_ALLOWED,
        ensureDict,
        setDictUrl
    };
})(typeof window !== "undefined" ? window : globalThis);