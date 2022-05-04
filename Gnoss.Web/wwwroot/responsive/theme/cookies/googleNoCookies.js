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