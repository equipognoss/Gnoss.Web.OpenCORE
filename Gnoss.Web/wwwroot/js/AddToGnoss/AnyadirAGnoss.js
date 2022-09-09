// Archivo JScript para la función Añadir a Gnoss

EjecutarFunciones();

function EjecutarFunciones()
{
    var descp=(window.getSelection?window.getSelection():(document.getSelection)?document.getSelection():'')+'',e=encodeURIComponent,g=document.getElementsByTagName('meta'),i=(g)?g.length:0,tags='',n,redireccionar,parametros,url;
    
    if (descp == '' && document.selection != null)
    {
        descp = document.selection.createRange().text;
    }
    
    redireccionar=function(){
        location.href=url;
    };
    
    function obtenerMetaInfo(cadena){
        var v,i=(g)?g.length:0;
        while(i--){
            n=g[i].getAttribute('name');
            if(n){
                if(n.toLowerCase()==cadena){
                    v=g[i].getAttribute('content');
                    break;
                }
            }
        };
        return (v)?e(v.substring(0,500)):'';
    };
    
    // Limpia la descripción de caracteres espurios.
    function LimpiarDescripcion(pTexto)
    {
        return pTexto.replace(/&/g,"[-and-]").replace(/\n/g,"[-salto-]").replace(/\t/g,"[-tab-]").replace(/#/g,"[-dilla-]").replace(/:/g,"[-2puntos-]")
    }
    
    var encodeDescp = true;
    
    if (descp == '')
    {
        encodeDescp = false;
    }
    
    descp = (descp=='')?obtenerMetaInfo('description'):descp.substring(0,1000);
    descp = LimpiarDescripcion(descp)
    
    if (encodeDescp)
    {
        descp = e(descp);
    }
    
    if (descp.length > 1000)
    {
        descp = descp.substring(0,1000);
    }
    
    tags=(tags=='')?obtenerMetaInfo('keywords'):tags.substring(0,500);
    
    var parmVersion = '';
    
    if (typeof(verAddTo) != "undefined")
    {
        parmVersion = '&verAddTo='+ verAddTo;
    }
    
    parametros='?addToGnoss=' +e(location.href) + parmVersion +'&titl='+e(LimpiarDescripcion(document.title.replace(/\n/g,"").replace(/\t/g,""))) +'&descp='+descp +'&tags='+ tags;url = urlAddTo +parametros;
    
    if(/Firefox/.test(navigator.userAgent))
        setTimeout(redireccionar,0);
    else 
        redireccionar();
}

