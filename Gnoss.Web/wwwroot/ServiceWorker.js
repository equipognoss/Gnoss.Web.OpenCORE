//listener
self.addEventListener('fetch', function (event) { });
//listener
self.addEventListener('push', function (e) {
    var body;

    if (e.data) {
        body = e.data.text();
    } else {
        body = "Standard Message";
    }

    var options = {
        body: body,
        vibrate: [100, 50, 100],
        data: {
            dateOfArrival: Date.now()
        },
        actions: [
            {
                action: "close", title: "Cerrar",
            }
        ]
    };
    e.waitUntil(
        self.registration.showNotification("Mensaje de Gnoss", options)
    );
});
//listener
self.addEventListener('notificationclick', function (e) {
    var notification = e.notification;
    var action = e.action;

    if (action === 'close') {
        notification.close();
    } else {
        //otra accion que no sea cerrar
        notification.close();
    }
});