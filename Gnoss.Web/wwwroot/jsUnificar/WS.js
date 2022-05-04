WSDataType = { json: "json", jsonp: "jsonp" };

WS = function(service, dataType) {
    this.service = service;
    if (dataType)
        this.dataType = dataType;
};

var indicesWS = {};
WS.prototype = {
    dataType: WSDataType.json,
    service: null,
    call: function(pMethod, pArgs, pCallback, pError) {
        var url = null;
        var service = this.service;
        service = this.obtenerUrl(service);
        if (service[service.length - 1] != "/") service += "/";
        if (this.dataType == WSDataType.jsonp) {
            url = $.jmsajaxurl({
                url: service,
                method: pMethod,
                data: pArgs != null ? pArgs : {}
            });
        } else {
            url = service + pMethod;
        }
        $.ajax({
            type: this.dataType == WSDataType.json ? "POST" : "POST",
            url: url + ((this.dataType == WSDataType.jsonp) ? "&format=json" : ""),
            data: ((this.dataType == WSDataType.json) ? JSON.stringify(pArgs) : ""),
            cache: false,
            contentType: "application/json; charset=utf-8",
            dataType: this.dataType
        })
        .done(function (response) {          
            if (pCallback)
            {
                if(response.d != null){
                    pCallback(response.d);
                }
                else{
                    pCallback(response);
                }
            }
        })
        .fail(function (data) {
            if (pError) {
                pError();
            }
        });
    },
    obtenerUrl: function (service) {
        var url = service;
        if (url.indexOf(',') != -1) {
            var urlMultiple = url.split(',');
            if (indicesWS[service] == null)
            {
                indicesWS[service] = null;
            }
            if (indicesWS[service] == null)
            {
                indicesWS[service] = this.aleatorio(0, urlMultiple.length - 1);
            } else if (indicesWS[service] > urlMultiple.length - 1)
            {
                indicesWS[service] = 0;
            }
            url = urlMultiple[indicesWS[service]];
            indicesWS[service]++;
        }
        return url;
    },
    aleatorio: function (inferior, superior) {
        numPosibilidades = superior - inferior;
        aleat = Math.random() * numPosibilidades;
        aleat = Math.round(aleat);
        return parseInt(inferior) + aleat;
    }
};