/*!
 * jQuery JavaScript Library v1.6.1
 * http://jquery.com/
 *
 * Copyright 2011, John Resig
 * Dual licensed under the MIT or GPL Version 2 licenses.
 * http://jquery.org/license
 *
 * Includes Sizzle.js
 * http://sizzlejs.com/
 * Copyright 2011, The Dojo Foundation
 * Released under the MIT, BSD, and GPL Licenses.
 *
 * Date: Thu May 12 15:04:36 2011 -0400
 */


var widgetGnoss = {
    widget: function (object) {
        var codigo = widgetGnoss.codigoWidget(object.ancho, object.background, object.urlComunidad, object.urlStatic, object.nombreComunidad, object.urlLogo, object.pagina);
        document.write(codigo);
    },
    focus: function (input) {
        this.defaultInput = input;
        this.defaultInputText = this.defaultInput.attributes['value'].value;
        var texto = this.defaultInput.value;
        if (texto == '' || texto == this.defaultInputText) {
            this.defaultInput.value = '';
            this.defaultInput.className = this.defaultInput.className.replace('defaultText', '').trim()
        }
    },
    blur: function (input) {
        this.defaultInput = input;
        this.defaultInputText = this.defaultInput.attributes['value'].value;
        var texto = this.defaultInput.value;
        if (texto == '' || texto == this.defaultInputText) {
            this.defaultInput.className = this.defaultInput.className.trim() + ' defaultText';
            this.defaultInput.value = this.defaultInputText;
        }
    },
    comprobarTecla: function (input, urlComunidad, pagina, event) {
        this.defaultInput = input;
        if ((event.which == 13) || (event.keyCode == 13)) {
            this.enviarFrom(input, urlComunidad, pagina); 
        }
    },
    enviarFrom: function (input, urlComunidad, pagina) {
        this.defaultInput = input;
        this.defaultInputText = this.defaultInput.attributes['value'].value;
        var texto = this.defaultInput.value;
        if (texto == this.defaultInputText)
        {
            texto = '';
        }
        var url = urlComunidad + '/' + pagina + '?search=';
        if (texto == '') { url = url.replace('?search=', ''); };
        window.open(url + texto);
    },
    codigoWidget: function (ancho, background, urlComunidad, urlStatic, nombreComunidad, urlLogo, pagina) {

        var esDidactalia = nombreComunidad.toLowerCase().indexOf('Didactalia') >= 0;
        var esInevery = nombreComunidad.toLowerCase().indexOf('inevery crea') >= 0 || nombreComunidad.toLowerCase().indexOf('ineverycrea') >= 0;

        if (background != '#ffffff') {
            background = '#D9DBF7';
            if (esInevery) {
                background = 'whiteSmoke';
            }
        }

        var html = '';
        html += '<style type="text/css">';
        html += '.-wbg *{font-family:"Lucida Sans Unicode", "Lucida Grande", "Lucida Sans", Arial, sans-serif; font-size:13px; margin:0; padding:0;}';
        html += '.-wbg{background:#D9DBF7; padding:10px; max-width:640px; min-width:150px;}';
        html += '.-wbg-community{color:#555;}';
        if (esInevery) {
            html += '.-wbg-author{color:#888888; text-align:right; font-size: 10px;}';

            html += '.-wbg-finder{border:1px solid #ccc; background:#fff; position:relative; margin:3px 0;-moz-border-radius: 5px;-webkit-border-radius: 5px;border-radius: 5px; padding: 2px;}';

            html += '.-wbg-finder input#gnoss_btnBuscar{text-indent:-9999px; overflow:hidden; width:32px; height: 32px; position:absolute; top:0; right:0; background:#00B5C4 url("' + urlStatic + '/cssNuevo/resources/spriteIcons.png?v=2.1.1880") no-repeat -216px -494px;border-radius: 0 5px 5px 0;}';

        } else {
            html += '.-wbg-author{color:#8186BD; text-align:right; font-size: 10px;}';
            html += '.-wbg-finder{border:1px solid #ccc; background:#fff; position:relative; margin:3px 0;-moz-border-radius: 2px;-webkit-border-radius: 2px;border-radius: 2px;;}';
            html += '.-wbg-finder input#gnoss_btnBuscar{text-indent:-9999px; overflow:hidden; width:28px; position:absolute; top:0; right:0; background:#FF8300 url("' + urlStatic + '/cssNuevo/resources/spriteIcons.png?v=2.1.1880") no-repeat -217px -496px;}';

        }
        html += '.-wbg-finder input{border:0; height:28px;}';
        html += '.-wbg-finder input#gnoss_txtBusqueda{width:90%; padding:0 6px;}';
        html += '.-wbg-finder input.defaultText {color: #AAAAAA; font-style: italic; font-size: 10px;}';
        html += '</style>';

        pagina = 'busqueda';

        var estilo = "";
        if ((ancho != null && ancho != "") || (background != null && background != "")) {
            estilo = "style=\"";
            if (ancho != null && ancho != "") { estilo += "width: " + ancho + ";" }
            if (background != null && background != "") {
                if (esInevery && background != '#ffffff') {
                    estilo += "background-color: whiteSmoke; background-image: -moz-linear-gradient(center top , whiteSmoke, #DEDEDE);"
                }
                else {
                    estilo += "background: " + background + ";"
                }
            }
            estilo += "\"";
        }

        html += '<div class="-wbg" ' + estilo + '>';
        html += '<div class="-wbg-section">';
        var textoMarcaAgua = '';
        if (esDidactalia) {
            textoMarcaAgua = 'Busca recursos educativos';
        }
        else if (esInevery) {
            textoMarcaAgua = 'Busca recursos creativos';
            html += '<img alt="Inevery CREA" title="' + nombreComunidad + '" src="http://content1.ineverycrea.net/imagenes/proyectos/' + $('.inpt_proyID').val() + '.png?v=35" style="border-width: 0px; width: 160px;">';
        }
        else {
            textoMarcaAgua = 'Buscar contenidos';
            html += '<p class="-wbg-community">Encontrar recursos en <strong>' + nombreComunidad + '<\/strong><\/p>';
        }
        html += '<div class="-wbg-finder">';
        html += '<input type="text" onkeydown="widgetGnoss.comprobarTecla(this, \'' + urlComunidad + '\', \'' + pagina + '\', event)" autocomplete="off" id="gnoss_txtBusqueda" class="defaultText" value="' + textoMarcaAgua + '" onfocus="widgetGnoss.focus(this);" onblur="widgetGnoss.blur(this);">';
        html += '<input type="button" id="gnoss_btnBuscar" value="Buscar" onclick="widgetGnoss.enviarFrom(this.previousSibling, \'' + urlComunidad + '\', \'' + pagina + '\')">';
        html += '<\/div>';
        if (esDidactalia) {
            html += '<p class="-wbg-author">Powered by <a href="' + urlComunidad + '"><img style="margin-bottom: -4px;;" src="' + urlStatic + '/img/logo_didactalia.png" title="' + nombreComunidad + '" alt="' + nombreComunidad + '"></a><\/p>';
        }
        else if (esInevery) {
            html += '<p class="-wbg-author">Powered by <a href="http://gnoss.com"><img alt="GNOSS" title="GNOSS" src="' + urlStatic + '/img/logo_gnoss_peque_gris.png" style="height: 12px; margin-bottom: -3px;"></a></p>';
        }
        else {
            html += '<p class="-wbg-author">Powered by <a href="http://gnoss.com"><img style="margin-bottom: -4px;;" src="' + urlStatic + '/img/logo_gnoss_peque.png" title="GNOSS" alt="GNOSS"></a><\/p>';
        }
        html += '<\/div>';
        html += '<\/div>';
        return html;
    }
}