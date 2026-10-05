/* languageToolApi.js
 * Cliente del endpoint de LanguageTool. Contrato esperado del servidor:
 *   POST {endpoint}
 *   body: { text, language }
 *   respuesta 200: { matches: [ { offset, length, message, replacements, rule, category, ... } ] }
 * Errores categorizados: NETWORK | TIMEOUT | SERVICE_UNAVAILABLE | HTTP_ERROR | NO_ENDPOINT
 */
(function (global) {
    "use strict";

    const ns = (global.LanguageTool = global.LanguageTool || {});

    function statusCode(err) {
        if (err && err.name === "AbortError") return "TIMEOUT";
        if (err && err.type === "network-error") return "NETWORK";
        if (err && err.code) return err.code;
        return "NETWORK";
    }

    function requestCheck(config) {
        const endpoint = config.endpoint;
        if (!endpoint) {
            return Promise.reject({ code: "NO_ENDPOINT", message: "endpoint vacío" });
        }
        return new Promise(function (resolve, reject) {
            const controller = (typeof AbortController !== "undefined") ? new AbortController() : null;
            const timeoutMs = config.timeoutMs || 15000;
            const timer = setTimeout(() => {
                if (controller) controller.abort();
                else reject({ code: "TIMEOUT", message: "timeout" });
            }, timeoutMs);

            fetch(endpoint, {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({
                    text: config.text,
                    language: config.language || "es"
                }),
                signal: controller ? controller.signal : undefined
            }).then(function (resp) {
                clearTimeout(timer);
                if (!resp.ok) {
                    reject({ code: resp.status === 503 ? "SERVICE_UNAVAILABLE" : "HTTP_ERROR", status: resp.status, message: "HTTP " + resp.status });
                    return;
                }
                resp.json().then(function (json) {
                    resolve(json || { matches: [] });
                }, function (err) {
                    reject({ code: "NETWORK", message: "JSON inválido", cause: err });
                });
            }, function (err) {
                clearTimeout(timer);
                reject({ code: statusCode(err), message: "fetch fallo", cause: err });
            });
        });
    }

    function normalize(response) {
        if (!response) return [];
        const raw = Array.isArray(response) ? response : (response.matches || (response.data && response.data.matches) || []);
        return raw.map(function (m) {
            return {
                offset: m.offset | 0,
                length: m.length | 0,
                message: m.message || m.msg || "",
                replacements: Array.isArray(m.replacements) ? m.replacements.map(r => r.value != null ? r.value : r) : [],
                ruleId: m.ruleId || m.rule || "",
                category: m.category || "spelling",
                context: m.context || null
            };
        });
    }

    ns.Api = {
        requestCheck,
        normalize
    };
})(typeof window !== "undefined" ? window : globalThis);