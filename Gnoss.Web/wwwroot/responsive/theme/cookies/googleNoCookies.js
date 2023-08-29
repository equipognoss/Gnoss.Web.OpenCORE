function cyrb53(str, seed = 0) {
    let h1 = 0xdeadbeef ^ seed,
        h2 = 0x41c6ce57 ^ seed;
    for (let i = 0, ch; i < str.length; i++) {
        ch = str.charCodeAt(i);
        h1 = Math.imul(h1 ^ ch, 2654435761);
        h2 = Math.imul(h2 ^ ch, 1597334677);
    }
    h1 = Math.imul(h1 ^ h1 >>> 16, 2246822507) ^ Math.imul(h2 ^ h2 >>> 13, 3266489909);
    h2 = Math.imul(h2 ^ h2 >>> 16, 2246822507) ^ Math.imul(h1 ^ h1 >>> 13, 3266489909);
    return 4294967296 * (2097151 & h2) + (h1 >>> 0);
}

function crearHash(clientIP) {
    let validityInterval = Math.round(new Date() / 1000 / 3600 / 24 / 4);
    let usrID = $('#inpt_usuarioID').val();
    let clientIDSource = clientIP + ";" + window.location.host + ";" + navigator.userAgent + ";" + navigator.language + ";" + usrID + ";" + validityInterval;
    let clientIDHashed = cyrb53(clientIDSource).toString(16);
    return clientIDHashed;
}
function cargarAnalytics() {
    (function (i, s, o, g, r, a, m) {
        i['GoogleAnalyticsObject'] = r; i[r] = i[r] || function () {
            (i[r].q = i[r].q || []).push(arguments)
        }, i[r].l = 1 * new Date(); a = s.createElement(o),
            m = s.getElementsByTagName(o)[0]; a.async = 1; a.src = g; m.parentNode.insertBefore(a, m)
    })(window, document, 'script', 'https://www.google-analytics.com/analytics.js', 'ga');
}
function lanzarAnalytics(googleId, clientHash) {
    ga('create', googleId, {
        'storage': 'none',
        'clientId': clientHash
    });
    ga('set', 'anonymizeIp', true);
    ga('send', 'pageview');
}
function contarVisitaNoCookie(googleId) {
    $.get("https://servicios60.gnoss.com/gnosstoken/token?userId=" + $('.inpt_usuarioID').val(), function (data) {
        var clientHash = data;
        cargarAnalytics();
        lanzarAnalytics(googleId, clientHash);
    });
}