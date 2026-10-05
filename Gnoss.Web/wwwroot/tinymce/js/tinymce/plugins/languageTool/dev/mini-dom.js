/* mini-dom.js — shim DOM mínimo para ejecutar los módulos del plugin en Node.
 * Soporta lo que usan languageToolMapper.js y languageToolUi.js: creación de
 * nodos y texto, childNodes, insertBefore/removeChild, className/style/attributes,
 * addEventListener/dispatchEvent. No es un DOM completo.
 */
"use strict";

function makeElement(tag) {
    const el = {
        nodeType: 1,
        tagName: String(tag).toUpperCase(),
        childNodes: [],
        parentNode: null,
        attributes: {},
        className: "",
        id: "",
        style: {},
        toString() { return '<' + this.tagName + '>'; }
    };

    Object.defineProperty(el, "firstChild", {
        get() { return this.childNodes[0] || null; }
    });

    el.appendChild = function (child) {
        if (child.parentNode) child.parentNode.removeChild(child);
        child.parentNode = this;
        this.childNodes.push(child);
        return child;
    };

    el.insertBefore = function (child, ref) {
        if (child.parentNode) child.parentNode.removeChild(child);
        const idx = ref ? this.childNodes.indexOf(ref) : -1;
        const t = idx >= 0 ? idx : this.childNodes.length;
        child.parentNode = this;
        this.childNodes.splice(t, 0, child);
        return child;
    };

    el.removeChild = function (child) {
        const idx = this.childNodes.indexOf(child);
        if (idx === -1) return child;
        this.childNodes.splice(idx, 1);
        child.parentNode = null;
        return child;
    };

    el.setAttribute = function (name, value) {
        this.attributes[name] = String(value);
        if (name === "id") this.id = String(value);
    };
    el.getAttribute = function (name) {
        return this.attributes[name] === undefined ? null : this.attributes[name];
    };

    Object.defineProperty(el, "textContent", {
        get() {
            return this.childNodes.map(n => n.nodeValue != null ? n.nodeValue : '').join("");
        },
        set(value) {
            this.childNodes = [];
            this.appendChild(makeText(String(value == null ? '' : value)));
        }
    });

    el.disabled = false;

    el._listeners = {};
    el.addEventListener = function (type, fn) {
        (this._listeners[type] = this._listeners[type] || []).push(fn);
    };
    el.removeEventListener = function (type, fn) {
        const arr = this._listeners[type] || [];
        const i = arr.indexOf(fn);
        if (i >= 0) arr.splice(i, 1);
    };
    el.dispatchEvent = function (event) {
        const ev = Object.assign({}, event, {
            type: event.type,
            target: event.target || this,
            currentTarget: this,
            stopPropagation() {}
        });
        const arr = (this._listeners[ev.type] || []).slice();
        for (const fn of arr) fn(ev);
        return true;
    };
    el.trigger = function (type, extra) {
        return el.dispatchEvent(Object.assign({}, extra || {}, { type: type, target: this }));
    };

    return el;
}

function makeText(value) {
    return {
        nodeType: 3,
        nodeValue: String(value),
        tagName: "#text",
        childNodes: [],
        parentNode: null,
        firstChild: null,
        nodeName: "#text",
        trget() {}
    };
}

function createDocument() {
    const document = makeElement("#document");
    document.body = makeElement("BODY");
    document._byId = {};

    document.createElement = makeElement;
    document.createTextNode = makeText;

    document.getElementById = function (id) { return document._byId[id] || null; };

    const origSetAttr = document.body.setAttribute;
    document.body.setAttribute = function (name, value) {
        origSetAttr.call(this, name, value);
        if (name === "id") document._byId[this.id] = this;
    };
    // registro de ids poco necesario; se deja el cuerpo simple
    document.documentElement = makeElement("HTML");

    return document;
}

module.exports = { makeElement, makeText, createDocument };