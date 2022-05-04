function FormularioSubidaEstilos() {
    if (document.getElementById('js/css').checked) {
        document.getElementById('archivo_js').disabled = false;
        document.getElementById('archivo_css').disabled = false;
        document.getElementById('archivo_zip').disabled = true;
        document.getElementById('tipo_archivo').value = 'js_css';
    }
    else if (document.getElementById('zip').checked) {
        document.getElementById('archivo_js').disabled = true;
        document.getElementById('archivo_css').disabled = true;
        document.getElementById('archivo_zip').disabled = false;
        document.getElementById('tipo_archivo').value = 'zip';

    }
}