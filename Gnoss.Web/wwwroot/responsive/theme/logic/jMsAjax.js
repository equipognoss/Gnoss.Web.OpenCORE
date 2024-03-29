/*
* jMsAjax 0.2.2 - Microsoft Ajax jQuery Plugin
*
* Copyright (c) 2008 Adam Schröder (schotime.net)
* Dual licensed under the MIT (MIT-LICENSE.txt)
* and GPL (GPL-LICENSE.txt) licenses.
*
* $Date: 2008-07-12 $
*/
(function($) {
    $.jmsajaxurl = function(options) {
        var url = options.url;
        if (url[url.length - 1] != "/") url += "/";
        url += options.method; if (options.data) {

            var data = ""; for (var i in options.data) {
                if (data != "")
                    data += "&"; data += i + "=" + msJSON.stringify(options.data[i]);
            }
            url += "?" + data; data = null; options.data = "{}";
        }
        return url;
    };
    $.jmsajax = function(options) {
        var defaults = { type: "POST", dataType: "msjson", data: {}, beforeSend: function(xhr) { xhr.setRequestHeader("Content-type", "application/json; charset=utf-8"); }, contentType: "application/json; charset=utf-8", error: function(x, s, m) { alert("Status: " + ((x.statusText) ? x.statusText : "Unknown") + "\nMessage: " + msJSON.parse(((x.responseText) ? x.responseText : "Unknown")).Message); } }; var options = $.extend(defaults, options); if (options.method)
            options.url += "/" + options.method; if (options.data) {
            if (options.type == "GET") {
                var data = ""; for (var i in options.data) {
                    if (data != "")
                        data += "&"; data += i + "=" + msJSON.stringify(options.data[i]);
                }
                options.url += "?" + data; data = null; options.data = "{}";
            }
            else if (options.type == "POST")
            { options.data = msJSON.stringify(options.data); }
        }
        if (options.success) {
            if (options.dataType) {
                if (options.dataType == "msjson") {
                    var base = options.success; options.success = function(response, status) {
                        var y = dateparse(response); if (options.version) {
                            if (options.version >= 3.5)
                                y = y.d;
                        }
                        else {
                            if (response.indexOf("{\"d\":") == 0)
                                y = y.d;
                        }
                        base(y, status);
                    }
                }
            }
        }
        return $.ajax(options);
    }; dateparse = function(data) {
        try {
            return msJSON.parse(data, function(key, value) {
                var a; if (typeof value === "string") {
                    if (value.indexOf("Date") >= 0)
                    { a = /^\/Date\((-?[0-9]+)\)\/$/.exec(value); if (a) { return new Date(parseInt(a[1], 10)); } }
                }
                return value;
            });
        }
        catch (e) { return null; }
    }
    msJSON = function() {
        function f(n) { return n < 10 ? '0' + n : n; }
        //Date.prototype.toJSON=function(key){return this.getUTCFullYear()+'-'+f(this.getUTCMonth()+1)+'-'+f(this.getUTCDate())+'T'+f(this.getUTCHours())+':'+f(this.getUTCMinutes())+':'+f(this.getUTCSeconds())+'Z';};
        var cx = /[\u0000\u00ad\u0600-\u0604\u070f\u17b4\u17b5\u200c-\u200f\u2028-\u202f\u2060-\u206f\ufeff\ufff0-\uffff]/g, escapeable = /[\\\"\x00-\x1f\x7f-\x9f\u00ad\u0600-\u0604\u070f\u17b4\u17b5\u200c-\u200f\u2028-\u202f\u2060-\u206f\ufeff\ufff0-\uffff]/g, gap, indent, meta = { '\b': '\\b', '\t': '\\t', '\n': '\\n', '\f': '\\f', '\r': '\\r', '"': '\\"', '\\': '\\\\' }, rep; function quote(string) {
            escapeable.lastIndex = 0; return escapeable.test(string) ? '"' + string.replace(escapeable, function(a) {
                var c = meta[a]; if (typeof c === 'string') { return c; }
                return '\\u' + ('0000' +
	(+(a.charCodeAt(0))).toString(16)).slice(-4);
            }) + '"' : '"' + string + '"';
        }
        function str(key, holder) {
            var i, k, v, length, mind = gap, partial, value = holder[key]; if (value && typeof value === 'object' && typeof value.toJSON === 'function') { value = value.toJSON(key); }
            if (typeof rep === 'function') { value = rep.call(holder, key, value); }
            switch (typeof value) {
                case 'string': return quote(value); case 'number': return isFinite(value) ? String(value) : 'null'; case 'boolean': case 'null': return String(value); case 'object': if (!value) { return 'null'; }
                    if (value.toUTCString) { return '"\\/Date(' + (value.getTime()) + ')\\/"'; }
                    gap += indent; partial = []; if (typeof value.length === 'number' && !(value.propertyIsEnumerable('length'))) {
                        length = value.length; for (i = 0; i < length; i += 1) { partial[i] = str(i, value) || 'null'; }
                        v = partial.length === 0 ? '[]' : gap ? '[\n' + gap +
	partial.join(',\n' + gap) + '\n' +
	mind + ']' : '[' + partial.join(',') + ']'; gap = mind; return v;
                    }
                    if (rep && typeof rep === 'object') { length = rep.length; for (i = 0; i < length; i += 1) { k = rep[i]; if (typeof k === 'string') { v = str(k, value, rep); if (v) { partial.push(quote(k) + (gap ? ': ' : ':') + v); } } } } else { for (k in value) { if (Object.hasOwnProperty.call(value, k)) { v = str(k, value, rep); if (v) { partial.push(quote(k) + (gap ? ': ' : ':') + v); } } } }
                    v = partial.length === 0 ? '{}' : gap ? '{\n' + gap + partial.join(',\n' + gap) + '\n' +
	mind + '}' : '{' + partial.join(',') + '}'; gap = mind; return v;
            }
        }
        return { stringify: function(value, replacer, space) {
            var i; gap = ''; indent = ''; if (typeof space === 'number') { for (i = 0; i < space; i += 1) { indent += ' '; } } else if (typeof space === 'string') { indent = space; }
            rep = replacer; if (replacer && typeof replacer !== 'function' && (typeof replacer !== 'object' || typeof replacer.length !== 'number')) { throw new Error('JSON.stringify'); }
            return str('', { '': value });
        }, parse: function(text, reviver) {
            var j; function walk(holder, key) {
                var k, v, value = holder[key]; if (value && typeof value === 'object') { for (k in value) { if (Object.hasOwnProperty.call(value, k)) { v = walk(value, k); if (v !== undefined) { value[k] = v; } else { delete value[k]; } } } }
                return reviver.call(holder, key, value);
            }
            cx.lastIndex = 0; if (cx.test(text)) {
                text = text.replace(cx, function(a) {
                    return '\\u' + ('0000' +
	(+(a.charCodeAt(0))).toString(16)).slice(-4);
                });
            }
            if (/^[\],:{}\s]*$/.test(text.replace(/\\(?:["\\\/bfnrt]|u[0-9a-fA-F]{4})/g, '@').replace(/"[^"\\\n\r]*"|true|false|null|-?\d+(?:\.\d*)?(?:[eE][+\-]?\d+)?/g, ']').replace(/(?:^|:|,)(?:\s*\[)+/g, ''))) { j = eval('(' + text + ')'); return typeof reviver === 'function' ? walk({ '': j }, '') : j; }
            throw new SyntaxError('JSON.parse');
        }
        };
    } ();
})(jQuery);