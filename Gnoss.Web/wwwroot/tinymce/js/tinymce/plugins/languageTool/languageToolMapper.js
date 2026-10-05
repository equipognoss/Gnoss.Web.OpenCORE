/* languageToolMapper.js
 * Extracción de texto plano del editor y marcado de errores con <span class="lt-marker">.
 * Trabaja sobre offsets de caracteres sobre el texto plano (por eso los marcadores
 * llevan data-lt-offset / data-lt-length). API:
 *   Mapper.extractText(editor)          -> { text, nodes, offsets }
 *   Mapper.markMatches(editor, matches)-> nº de errores marcados
 *   Mapper.clearMatches(editor)
 *   Mapper.replaceAt(editor, offset, length, replacement)
 */
(function (global) {
    "use strict";

    const ns = (global.LanguageTool = global.LanguageTool || {});

    function textNodes(root) {
        const nodes = [], offsets = [], parts = [];
        let total = 0;
        (function walk(node) {
            if (!node) return;
            if (node.nodeType === 3) {
                const t = node.nodeValue == null ? "" : String(node.nodeValue);
                if (t.length) {
                    nodes.push(node);
                    offsets.push(total);
                    parts.push(t);
                    total += t.length;
                }
                return;
            }
            const children = node.childNodes;
            for (let i = 0; i < children.length; i++) walk(children[i]);
        })(root);
        return { nodes, offsets, text: parts.join("") };
    }

    function extractText(editor) {
        return textNodes(editor.getBody());
    }

    function clearMatches(editor) {
        const body = editor.getBody();
        if (!body) return;
        const markers = [];
        (function scan(node) {
            if (!node || node.nodeType !== 1) return;
            if (node.className && String(node.className).split(/\s+/).indexOf("lt-marker") !== -1) {
                markers.push(node);
            }
            const children = node.childNodes;
            for (let i = 0; i < children.length; i++) scan(children[i]);
        })(body);
        markers.forEach(function (m) {
            const parent = m.parentNode;
            if (!parent) return;
            while (m.firstChild) parent.insertBefore(m.firstChild, m);
            parent.removeChild(m);
        });
    }

    function wrapPart(node, start, end, attrs) {
        const full = node.nodeValue || "";
        const safeStart = Math.max(0, start);
        const safeEnd = Math.min(full.length, end);
        if (safeEnd <= safeStart) return null;
        const d = global.document;
        const parent = node.parentNode;
        const before = d.createTextNode(full.slice(0, safeStart));
        const after = d.createTextNode(full.slice(safeEnd));
        const span = d.createElement("span");
        span.className = "lt-marker";
        Object.keys(attrs).forEach(function (k) { span.setAttribute(k, String(attrs[k])); });
        span.appendChild(d.createTextNode(full.slice(safeStart, safeEnd)));
        parent.insertBefore(before, node);
        parent.insertBefore(span, node);
        parent.insertBefore(after, node);
        parent.removeChild(node);
        return span;
    }

    function markMatches(editor, matches) {
        clearMatches(editor);
        if (!matches || !matches.length) return 0;
        const sorted = matches.slice().sort((a, b) => (a.offset || 0) - (b.offset || 0));
        let covered = -1;
        let count = 0;
        sorted.forEach(function (m) {
            const offset = m.offset | 0;
            const length = Math.max(0, m.length | 0);
            if (length <= 0) return;
            const start = offset;
            const end = offset + length;
            if (start < covered) return;
            const index = textNodes(editor.getBody());
            const nodes = index.nodes, offs = index.offsets;
            let wrapped = false;
            for (let i = 0; i < nodes.length; i++) {
                const os = offs[i];
                const nodeLen = (nodes[i].nodeValue || "").length;
                const oe = os + nodeLen;
                if (oe <= start || os >= end) continue;
                const ls = Math.max(0, start - os);
                const le = Math.min(nodeLen, end - os);
                wrapPart(nodes[i], ls, le, {
                    "data-lt-rule": m.ruleId || m.rule || "",
                    "data-lt-message": m.message || "",
                    "data-lt-offset": String(start),
                    "data-lt-length": String(length)
                });
                wrapped = true;
            }
            if (wrapped) { count++; covered = end; }
        });
        return count;
    }

    function nodeAt(index, pos, preferAfter) {
        const nodes = index.nodes, offs = index.offsets;
        for (let i = 0; i < nodes.length; i++) {
            const os = offs[i];
            const oe = os + (nodes[i].nodeValue || "").length;
            if (pos >= os && pos <= oe) {
                if (pos === oe && preferAfter) continue;
                return { node: nodes[i], local: pos - os, index: i };
            }
        }
        return null;
    }

    function replaceAt(editor, offset, length, replacement) {
        const start = offset | 0;
        const end = start + (length | 0);
        if (start < 0 || end <= start) return;
        let index = textNodes(editor.getBody());
        const si = nodeAt(index, start, false);
        if (!si) return;
        index = textNodes(editor.getBody());
        const ei = nodeAt(index, end, false);
        if (!ei) return;
        const startNode = si.node;
        const endNode = ei.node;
        const startLocal = si.local;
        const endLocal = ei.local;
        const startText = startNode.nodeValue || "";
        const endText = endNode.nodeValue || "";
        if (si.index === ei.index) {
            startNode.nodeValue = startText.slice(0, startLocal) + replacement + startText.slice(endLocal);
            return;
        }
        // Rango entre dos nodos de texto distintos
        startNode.nodeValue = startText.slice(0, startLocal) + replacement;
        endNode.nodeValue = endText.slice(endLocal);
        index = textNodes(editor.getBody());
        for (let i = si.index + 1; i < ei.index; i++) {
            if (index.nodes[i]) index.nodes[i].nodeValue = "";
        }
    }

    ns.Mapper = {
        extractText,
        clearMatches,
        markMatches,
        replaceAt
    };
})(typeof window !== "undefined" ? window : globalThis);