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

function lanzarAnalytics(googleId, clientHash) {
    //ga('create', googleId, {
    //    'storage': 'none',
    //    'clientId': clientHash
    //});
    //ga('set', 'anonymizeIp', true);
    //ga('send', 'pageview');


    (function (w, d, s, l, i) {
    //    w[l] = w[l] || []; w[l].push({
    //        'gtm.start':
    //            new Date().getTime(), event: 'gtm.js'
    //    });
    //    //var f = d.getElementsByTagName(s)[0],
    //    //    j = d.createElement(s), dl = l != 'dataLayer' ? '&l=' + l : ''; j.async = true; j.src =
    //    //    'https://www.googletagmanager.com/gtm.js?id=' + i + dl; f.parentNode.insertBefore(j, f);
        var f = d.getElementsByTagName(s)[0],
            j = d.createElement(s), dl = l != 'dataLayer' ? '&l=' + l : ''; j.async = true; j.src =
                'https://www.googletagmanager.com/gtag/js?id=' + i + dl; f.parentNode.insertBefore(j, f);
    })(window, document, 'script', 'dataLayer', googleId);
   
    window.dataLayer = window.dataLayer || [];

    function gtag() { dataLayer.push(arguments); }
    gtag('consent', 'default', {
        'ad_storage': 'denied',
        'analytics_storage': 'denied'
    });
    gtag('js', new Date());
    gtag('config', googleId);
}
function contarVisitaNoCookie(googleId) {
    $.get("https://servicios60.gnoss.com/gnosstoken/token?userId=" + $('.inpt_usuarioID').val(), function (data) {
        var clientHash = data;
        //cargarAnalytics(googleId);
        lanzarAnalytics(googleId, clientHash);
    });
}