function postEliminarImagen(postUrl) {
    const form = document.createElement('form');
    form.method = 'post';
    form.action = postUrl + '/doPostDeleteImage';
    console.log(form.action)
    document.body.appendChild(form);
    form.submit();
}