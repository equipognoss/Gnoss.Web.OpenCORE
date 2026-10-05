/* languageToolExtra.es.js — Palabras adicionales del corrector local de español.
 *
 * Edita este fichero para personalizar el corrector sin regenerar el diccionario:
 *
 *   WORDS:  lista de palabras EXTRA que se consideran correctas aunque no estén
 *           en el diccionario generado (lenguaje técnico, nombres, marcas...).
 *
 *   BAD:    lista de "errores voluntarios" (coloquialismos, SMS, apocopes) que
 *           SIEMPRE se subrayan aunque existan en el diccionario. Cada entrada
 *           lleva las correcciones sugeridas (vacías o con una): 
 *             "ke":   ["que"]
 *             "d":    ["de"]
 *
 * Todo en minúsculas y sin tildes.
 */
(function (global) {
    "use strict";

    const ns = (global.LanguageTool = global.LanguageTool || {});

    ns.Extra = {
        WORDS: new Set([
            "app", "apps", "web", "backend", "frontend", "online", "email",
            "plugin", "plugins", "macro", "interfaz", "pdf", "rtf", "csv",
            "xlsx", "docx", "pptx", "ok", "meet", "zoom", "tienda", "moodle",
            "sakai", "lms", "crm", "erp", "sass"
        ]),

        BAD: {
            "ke": ["que"],
            "kes": ["que"],
            "xq": ["porque", "por qué"],
            "xke": ["porque", "por qué"],
            "xfa": ["por favor"],
            "tb": ["también"],
            "tmb": ["también"],
            "asies": ["así es"],
            "grasias": ["gracias"],
            "asier": ["hacer"],
            "aser": ["hacer"],
            "aber": ["haber", "a ver"],
            "porfa": ["por favor"],
            "muxo": ["mucho"],
            "musho": ["mucho"],
            "vueno": ["bueno"],
            "bienbenido": ["bienvenido"],
            "aunqe": ["aunque"],
            "boy": ["voy"],
            "fono": [],
            "ey": ["eh"]
        }
    };
})(typeof window !== "undefined" ? window : globalThis);