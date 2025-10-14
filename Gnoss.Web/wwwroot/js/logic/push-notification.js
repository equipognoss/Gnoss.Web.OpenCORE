/* NOTIFICACIONES PUSH */

let rutaEjecucion = $('#inpt_rutaEjecucionWeb').val();
if (rutaEjecucion == null || rutaEjecucion == undefined) {
    rutaEjecucion = "";
}
if ('serviceWorker' in navigator) {
    window.addEventListener("load", () => {
        navigator.serviceWorker.register("/" + rutaEjecucion + "ServiceWorker.js");
    });
}
if ('serviceWorker' in navigator) {
    window.addEventListener("load", () => {
        navigator.serviceWorker.register("/" + rutaEjecucion + "ServiceWorker.js")
            .then((reg) => {
                if (Notification.permission === "granted") {
                    getSubscription(reg);
                } else if (Notification.permission === "denied") {
                    blockSubscription();
                } else {
                    requestNotificationAccess(reg);
                }
            });
    });
}

function requestNotificationAccess(reg) {
    let notificacionesPermitidas = $('#inpt_Notificaciones').val();
    if (notificacionesPermitidas != "") {
        Notification.requestPermission(function (status) {
            if (status == "granted") {
                getSubscription(reg);
            } else if (status == "denied") {
                blockSubscription();
            }
        });
    }
}

function blockSubscription() {
    let urlPeticion = location.protocol + "//" + location.host + "/PeticionesAJAX/DesuscribirseNotificacionesPush";

    GnossPeticionAjax(
        urlPeticion,
        null,
        true
    ).done(function (data) {

    }).fail(function (data) {

    }).always(function () {

    });
}

function fillSubscribeFields(sub) {
    let endpoint = sub.endpoint;
    let p256dh = arrayBufferToBase64(sub.getKey("p256dh"));
    let auth = arrayBufferToBase64(sub.getKey("auth"));
    let urlPeticion = location.protocol + "//" + location.host + "/PeticionesAJAX/SuscribirseNotificacionesPush";

    let datapost =
    {
        pEndpoint: endpoint,
        pP256dh: p256dh,
        pAuth: auth
    };

    GnossPeticionAjax(
        urlPeticion,
        datapost,
        true
    ).done(function (data) {

    }).fail(function (data) {

    }).always(function () {

    });
}

function arrayBufferToBase64(buffer) {
    let binary = '';
    let bytes = new Uint8Array(buffer);
    let len = bytes.byteLength;
    for (let i = 0; i < len; i++) {
        binary += String.fromCharCode(bytes[i]);
    }
    return window.btoa(binary);
}