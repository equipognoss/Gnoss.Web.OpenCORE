/*
 * Genera el diccionario español embebido a partir de una lista de palabras.
 *
 * Salida (dos/tres ficheros, no uno):
 *   <base>.js        bootstrap diminuto: define ns.Dict con carga PEREZOSA
 *   <base>.chunk.js  los ~4 MB de palabras (JSONP; funciona con file://)
 *   <base>.json      (opcional, con --json) array de palabras para fetch/HTTP
 *
 * Fuente por defecto: diccionario RAE (rla-es) expandido morfológicamente con
 * dev/build-rae-dict.js. Lemas extra en dev/dict-extra.es.txt.
 *
 * Uso:
 *   node dev/build-dictionary.js <fuente.txt> <base-destino> [--json]
 * Ejemplo:
 *   node dev/build-dictionary.js dev/rae_words.txt \
 *     tinymce/js/tinymce/plugins/languageTool/languageToolDict.es
 */
"use strict";

const fs = require("fs");
const path = require("path");

const WORD_REGEX = /^\p{L}+$/u;

function buildDictionary(sourcePath) {
    const lines = fs.readFileSync(sourcePath, "utf8").split(/\r?\n/);
    const words = new Set();

    for (const line of lines) {
        const trimmed = line.trim();
        if (!trimmed) continue;
        const word = trimmed.split(/\s+/)[0].toLowerCase();
        if (word.length === 0) continue;
        if (!WORD_REGEX.test(word)) continue;
        words.add(word);
    }

    // Lemas extra (palabras comunes ausentes de la lista RAE, p.ej. -ción)
    const extraPath = path.join(path.dirname(sourcePath), "dict-extra.es.txt");
    if (fs.existsSync(extraPath)) {
        for (const line of fs.readFileSync(extraPath, "utf8").split(/\r?\n/)) {
            const w = line.trim().toLowerCase();
            if (w) words.add(w);
        }
    }

    return Array.from(words).sort();
}

function damerauLevenshtein(a, b) {
    const m = a.length, n = b.length;
    const d = [];
    for (let i = 0; i <= m; i++) d[i] = [i];
    for (let j = 0; j <= n; j++) d[0][j] = j;
    for (let i = 1; i <= m; i++) {
        for (let j = 1; j <= n; j++) {
            const cost = a[i - 1] === b[j - 1] ? 0 : 1;
            d[i][j] = Math.min(d[i - 1][j] + 1, d[i][j - 1] + 1, d[i - 1][j - 1] + cost);
            if (i > 1 && j > 1 && a[i - 1] === b[j - 2] && a[i - 2] === b[j - 1]) {
                d[i][j] = Math.min(d[i][j], d[i - 2][j - 2] + cost);
            }
        }
    }
    return d[m][n];
}

const DAMERAU_SRC = damerauLevenshtein.toString();

const BOOTSTRAP = `/* Auto-generado por dev/build-dictionary.js — NO editar a mano. */
(function (global) {
    "use strict";

    var ns = (global.LanguageTool = global.LanguageTool || {});

    var RAW_MARK = "__DICT_RAW__";
    var SET = null, UA = null, B1 = null, B2 = null, COUNT = 0;
    var MAX_CANDIDATES = 6;

    ${DAMERAU_SRC}

    function ACC(s) {
        return String(s).toLowerCase().normalize("NFD")
            .replace(/[\\u0300-\\u0304\\u0306-\\u0308\\u030a-\\u036f]/g, "")
            .replace(/\\u00f1/g, "n");
    }

    function build(raw) {
        if (SET) return;
        var W = String(raw || "").split(" ");
        SET = Object.create(null);
        UA = Object.create(null);
        B1 = Object.create(null);
        B2 = Object.create(null);
        var i, w, a, k;
        COUNT = W.length;
        for (i = 0; i < W.length; i++) {
            w = W[i];
            if (!w) continue;
            SET[w] = true;
            a = ACC(w);
            UA[a] = true;
            k = a.charAt(0) + "|" + a.length;
            (B1[k] || (B1[k] = [])).push(w);
            if (a.length > 1) {
                k = a.charAt(1) + "|" + a.length;
                (B2[k] || (B2[k] = [])).push(w);
            }
        }
        if (global.LanguageTool) delete global.LanguageTool[RAW_MARK];
    }

    function scriptLoad(url, onload, onerror) {
        var s = global.document.createElement("script");
        s.async = false;
        s.src = url;
        s.onload = onload;
        s.onerror = function () { onerror(new Error('No se pudo cargar ' + url)); };
        global.document.head.appendChild(s);
    }

    function pushCand(cand, word) {
        if (!SET) return;
        if (SET[word] !== true && UA[ACC(word)] !== true) return;
        cand[word] = true;
    }

    ns.Dict = {
        load: function (url) {
            var self = this;
            if (this._p) return this._p;
            this._p = new Promise(function (resolve, reject) {
                var raw = global.LanguageTool && global.LanguageTool[RAW_MARK];
                if (raw) { build(raw); resolve(); return; }
                var u = url || global.LANGUAGE_TOOL_DICT_URL || '';
                if (!u) { resolve(); return; }
                if (/\\.json(\\?|$)/i.test(u)) {
                    global.fetch(u).then(function (r) { return r.json(); })
                        .then(function (data) {
                            build(Array.isArray(data) ? data.join(" ") : String(data));
                            resolve();
                        }).catch(reject);
                } else {
                    scriptLoad(u, function () {
                        build(global.LanguageTool && global.LanguageTool[RAW_MARK]);
                        resolve();
                    }, reject);
                }
            });
            return this._p;
        },
        get ready() { return !!SET; },
        has: function (word) {
            if (!SET) return false;
            if (SET[word] === true) return true;
            return UA[ACC(word)] === true;
        },
        suggest: function (input) {
            if (!SET) return [];
            var word = ACC(input);
            if (!word) return [];
            var L = word.length;
            var cand = Object.create(null);
            var arr, j, d, res = [];
            function collect(bucket) {
                arr = bucket;
                if (!arr) return;
                for (j = 0; j < arr.length; j++) cand[arr[j]] = true;
            }
            collect(B1[word.charAt(0) + "|" + L]);
            collect(B1[word.charAt(0) + "|" + (L + 1)]);
            collect(B1[word.charAt(0) + "|" + (L - 1)]);
            collect(B1[word.charAt(0) + "|" + (L + 2)]);
            collect(B1[word.charAt(0) + "|" + (L - 2)]);
            if (word.length > 1) {
                collect(B2[word.charAt(1) + "|" + L]);
                collect(B2[word.charAt(1) + "|" + (L + 1)]);
                collect(B2[word.charAt(1) + "|" + (L - 1)]);
            }
            for (var i = 0; i < word.length; i++) {
                pushCand(cand, word.slice(0, i) + word.slice(i + 1));
            }
            for (i = 0; i < word.length - 1; i++) {
                pushCand(cand, word.slice(0, i) + word.charAt(i + 1) + word.charAt(i) + word.slice(i + 2));
            }
            for (var c in cand) {
                d = damerauLevenshtein(word, ACC(c));
                if (d <= 2) res.push([c, d]);
            }
            res.sort(function (x, y) {
                if (x[1] !== y[1]) return x[1] - y[1];
                return x[0] < y[0] ? -1 : x[0] > y[0] ? 1 : 0;
            });
            return res.slice(0, MAX_CANDIDATES).map(function (e) { return e[0]; });
        },
        get size() { return COUNT; }
    };
})(typeof window !== "undefined" ? window : globalThis);
`;

function generateAll(words, basePath, opts) {
    opts = opts || {};
    const raw = words.join(" ");

    const chunk = `/* Auto-generado por dev/build-dictionary.js — chunk perezoso. NO editar a mano. */
(function (g) {
    var ns = (g.LanguageTool = g.LanguageTool || {});
    ns.__DICT_RAW__ = ${JSON.stringify(raw)};
})(typeof window !== "undefined" ? window : globalThis);
`;

    fs.writeFileSync(basePath + ".js", BOOTSTRAP, "utf8");
    fs.writeFileSync(basePath + ".chunk.js", chunk, "utf8");
    if (opts.json) {
        fs.writeFileSync(basePath + ".json",
            "[\n" + words.map(w => JSON.stringify(w)).join(",\n") + "\n]", "utf8");
    }
    return words.length;
}

if (require.main === module) {
    const argv = process.argv.slice(2);
    if (argv.length < 2) {
        console.error("Uso: node dev/build-dictionary.js <fuente.txt> <base-destino>");
        console.error("  Escribe <base>.js (bootstrap), <base>.chunk.js (lazy) y <base>.json");
        process.exit(1);
    }
    const sourcePath = path.resolve(argv[0]);
    let basePath = path.resolve(argv[1]);
    if (basePath.endsWith(".js")) basePath = basePath.slice(0, -3);
    const wantJson = argv.indexOf("--json") !== -1;
    const words = buildDictionary(sourcePath);
    const count = generateAll(words, basePath, { json: wantJson });
    const exts = wantJson ? [".js", ".chunk.js", ".json"] : [".js", ".chunk.js"];
    for (const ext of exts) {
        const kb = (fs.statSync(basePath + ext).size / 1024).toFixed(1);
        console.log("  " + ext + ": " + kb + " KB");
    }
    console.log(`OK ${count} palabras únicas -> ${basePath}.*`);
}

module.exports = { buildDictionary, generateAll };