var configuracion = {
    idioma: 'en'
};
var suscripciones = {
    enviarEnlace: 'Send link',
    URLenviarEnlace: 'send-link',
    marcarLeido: 'Mark as read'
};
var metaBuscador = {
    personasYorganizaciones: 'People and organizations',
    Debates: 'Debates',
    Preguntas: 'Questions',
    Recursos: 'Resources',
    BusquedaAvanzada: 'Advanced search'
};
var mensajes = {
    enviarMensaje: 'Send message',
    enviar: 'Send',
    asunto: 'Subject',
    descripcion: 'Description',
    mensajeEnviado: 'Message sent',
    mensajeError: 'Subject and Description are required',
    sinLeer: 'unread',
    nuevos: 'news',
    infoPerderMen: 'The message has not been sent. It will be discarded.',
    infoPerderMen2: 'El mensaje actual no ha sido enviado y se perderá. \n\n¿Seguro que quieres abandonar esta página?',
    recibidos: 'Inbox messages',
    enviados: 'Sent messages',
    eliminados: 'Deleted messages',
    mensaje: 'Message',
    volverlistado: 'back to the messages list',
    agrupacionGrafoTitulo: ' related entities of type '
};
var dialogo = {
    confirmarSuscripcion: '<p>Do you want to subscribe to the profile of <em>@1@</em> ?</p>',
    eliminarSuscripcion: '<p>Do you want to cancel your subscription to the profile of <em>@1@</em>?</p>',
    confirmarSuscripcionBlog: '<p>Do you want to subscribe to the blog <em>@1@</em> ?</p>',
    eliminarSuscripcionBlog: '<p>Do you want to cancel your subscription to the blog?</p>'
};
var comentarios = {
    comentario: 'Comment',
    editarComentario: 'Edit comment',
    responderComentario: 'Reply comment',
    publicarcomentario: 'Post a comment',
    enviar: 'Send',
    guardar: 'Save',
    comentarioError: 'The comment can not be empty'
};
var borr = {
    si: 'yes',
    no: 'no',
    eliminar: 'delete',
    suscripcion: 'Are you sure you want to cancel this subscription?',
    borrador: 'Are you sure you want to discard this draft?',
    comentario: 'Are you sure you want to delete this comment?'
};
var invitaciones = {
    correosVacios: 'Please specify at least one email address.',
    correoErroneo: 'Some of the  inserted emails is not valid.',
    usuariosVacios: 'Please specify at least one user.',
    aceptar: 'Do you want to accept the invitation?',
    ignorar: 'Do you want to ignore the invitation?',
    correoPerteneceOrg: 'The introduced email belongs to a user that is already participating in the organization',
    correoPerteneceCom: 'The introduced email belongs to a user that is already participating in the community',
    invitarContactoAceptar: 'Do you want to add @1@ as contact? @2@ will be notified that you want to add him/her as contact.',
    ingnorarContactoAceptar: '¿Quieres ignorar a @1@? No será sugerido más.',
    ingnorarProyAceptar: '¿Quieres ignorar el proyecto @1@? No será sugerido más.',
    haztemiembroProyAceptar: '¿Quieres hacerte miembro del proyecto @1@?',
    solicitaraccesoProyAceptar: '¿Quieres solicitar acceso al proyecto @1@?'
};
var susc = {
    aceptarSuscCat: 'Do you want to subscribe to this category?',
    eliminarSuscCat: 'Do you want to unsubscribe from this category?'
};

var relacionesTags = {
    avisolegal: "This information belongs to third parties. RIAM is not responsible for the accuracy of the data or information contained"
};

var widget = {
    verCodigo: 'See the code of the widget',
    verOpciones: 'See the setting options'
};
var calendario = {
    idioma: 'en',
    desde: 'From',
    hasta: 'To'
};
var categorias = {
    nombre: 'Name',
    nombreEspanol: 'Name (Spanish)',
    nombreIngles: 'Name (English)',
    nombrePortugues: 'Name (Portuguese)',
    espanol: 'Spanish',
    ingles: 'English',
    portugues: 'Portuguese',
    desactivarEdicionMultIidioma: '¿Estás seguro de que quieres desactivar la edición en varios idiomas del índice de categorías de tu comunidad? Si lo haces, se tomará como idioma de la comunidad el que hayas elegido por defecto para la comunidad',
    seleccionarIdioma: 'Elije el idioma que quieres mantener: ',
    aceptar: 'Accept',
    cancelar: 'Cancel'
};
var form = {
    mas: 'More',
    masMIN: 'more',
    menos: 'fewer',
    muysegura: 'High safe',
    segura: 'Safe',
    pocosegura: 'Low safe',
    insegura: 'Unsafe',
    errorLoginVacio: 'You must enter username and password',
    errorLogin: 'The username or password is incorrect.',
    usuarioCorto: 'The username is too short (Minimum of 4 characters in length)',
    longitudUsuario: 'The username must have between 4 and 12 characters',
    formatoUsuario: 'The username cannot contain spaces or special characters, except:. - _',
    formatoNombre: 'The username cannot contain special characters',
    formatoNombreCaracteresRepetidos: 'El nombre de usuario no puede contener mas de 2 caracteres iguales consecutivos',
    formatoApellidos: 'The Last Name cannot contain special characters',
    formatoApellidosCaracteresRepetidos: 'Los apellidos del usuario no pueden contener mas de 2 caracteres iguales consecutivos',
    formatoProvincia: 'The region name cannot contain < or >',
    formatoCP: 'The Postal Code cannot contain < or >',
    formatoLocalidad: 'The location cannot contain < or >',
    formatoRazonSocial: 'The organization name cannot contain < or >',
    formatoAlias: 'The organization name cannot contain < , > or ","',
    formatoCIF: 'The Fiscal Identification Number cannot contain < or >',
    formatoDireccion: 'The address cannot contain < or >',
    pwCorta: 'The password must have between 6 and 12 characters.',
    pwFormato: 'The password must have between 6 and 12 characters, at least one letter and one number. It cannot contain special characters, except: # _ $ *',
    organizacionRepetida: 'The name of that organization is already in beeing used.',
    pwIgual: 'The password fields do not match',
    emailValido: 'The email is not valid',
    obligatorio: ' is a required field',
    aceptarLegal: 'You must read and accept the terms of access and use',
    mayorEdad: 'Para ser miembro de la comunidad debes tener al menos 18 años',
    camposVacios: 'You must fill in all the fields marked as *',
    usuariorepetido: 'The username is not available.',
    emailrepetido: 'The email address is already in use',
    paginaweb: 'The website name is not valid',
    captchaincorrecto: 'The result of the math is wrong.',
    nifincorrecto: 'The field <i>Fiscal Identification Number</i> is wrong',
    cifincorrecto: 'The field <i>Taxpayer Identification Code</i> is wrong',
    fechafunincorrecta: 'You must set the date of constitution',
    fechanacincorrecta: 'You must set the date of birth',
    fechanacinsuficiente: 'You must be 14 years old to sign up',
    fechanacinsuficiente18: 'You must be 18 years old to sign up',
    fechafunincorrectaformato: 'You must set a valid date of constitution',
    fechanacincorrectaformato: 'You must set a valid date of birth',
    sexoincorrecto: 'You must set your gender',
    tipoorgincorrecta: 'You must set the type of organization',
    sectorincorrecto: 'You must set the sector',
    paisincorrecto: 'You must set a country',
    provinciaincorrecta: 'You must set the state',
    numeroemplincorrecto: 'You must set the number of employees',
    modooperacionincorrecto: 'You must set the operation mode of the organization',
    nombrecortoincorrecto: 'The name of your personal web must have between 4 and 30 characters that can be numbers, letters or "-"',
    grupoincorrecto: 'The group names must be letters separated by commas',
    nombrecortoincorrectocentro: 'The short name of the center must have between 2 and 10 characters that can be numbers, letters or dash ("-")',
    nombrecortoincorrectoasignatura: 'The short name of the subject must have between 2 and 10 characters that can be numbers, letters or dash ("-")',
    nombrecortoincorrectocurso: 'The course cannot have more than 10 characters that must be numbers',
    debesseleccionartipoclase: 'You must select the type of class',
    nombrecortorepetido: 'The name of the personal web is already being used by another user',
    nombrecortoincorrectoorg: 'The name of your own corporative web must have between 4 and 30 characters that can be numbers, letters or "-"',
    nombrecortorepetidoorg: 'The name of the corporative web is already being used by another organization',
    nombrecortorepetidoclase: 'The class name is already being used',
    proyectonoseleccionado: 'You must select at least one community',
    nombrecortousuario: 'The name of the corporative web must be different from the webpage of the user created',
    perderdatosbio: 'You have made changes that you have not saved.\nIf you go on and leave the page, all your changes will be lost.',
    generarversionauto: 'The document has changed. Do you want to generate a new version of the resources?',
    generarversionautoinfo: 'If you don´t generate a version, the current document will be replaced',
    especificaDocumento: 'You must upload a document',
    errordtitulo: 'A title is required',
    errordurl: 'An URL is required',
    errorddescripcion: 'A description is required',
    errordtag: 'At least one tag is required',
    errordcategoria: 'You must link the resource to a category',
    errordcategoriaSelect: 'Es obligatorio seleccionar una categoría',
    errordcategoriapregunta: 'You must link the question to a category',
    errordcategoriadebate: 'You must link the discussion to a category',
    errordcategoriaencuesta: 'You must link the poll to a category',
    nombrecortocomunidadincorrecto: 'The short name of your community must have between 4 and 30 characters that can be numbers, letters or "-"',
    campossolicitudcomunidadincompletos: 'You must fill in all the fields to submit your request.',
    nombrecomunidadincorrecto: 'The name of your community cannot contain < or >',
    errorVersiones: 'You must check 2 versions',
    caracteresRestantes: 'Characters left: ',
    centroestudiosincorrecto: 'You must set the studies center',
    areaestudiosincorrecto: 'You must set the studies area',
    fichaTecnicaPDF: 'Ficha técnica / Technical sheet – PDF',
    eliminar: 'delete',
    tagsPropuestos: 'Suggested tags:',
    cargando: 'Loading...',
    selectImg: 'Select a image',
    cargando: 'Loading',
    htmlincrustado: 'Html incrustado',
    html: 'Html',
    vermas: 'Ver más',
    vermenos: 'Ver menos'
};
//La estructura de tiempos varía en inglés y en español:hace 2 días, 2 days ago. Solución provisional 'Hace' lo dejamos en blanco, ponemos ago después de expresiones temporales-->
var tiempo = {
    hace: '',
    el: 'On',
    eldia: 'On',
    ayer: 'Yesterday',
    haceMinus: '',
    elMinus: 'on',
    ayerMinus: 'yesterday',
    minuto: 'minute ago',
    minutos: 'minutes ago',
    hora: 'hour ago',
    horas: 'hours ago',
    dia: 'day ago',
    dias: 'days ago',
    semana: 'week ago',
    fechaBarras: '@1@/@2@/@3@',
    fechaPuntos: '@1@.@2@.@3@'
};


var textoFormSem = {
    confimEliminar: 'Do you want to remove the item \\\'@1@\\\'?',
    confimEliminarEntidad: 'Do you want to remove the entity?',
    propiedadObliValor: 'A value for this property is required.',
    errorFecha: 'The value of this field must be date-type (Eg: 04/12/2009).',
    errorNumEntero: 'The value of this field must be an integer.',
    errorNum: 'The value of this field must be a number.',
    valorExcedeNumCarac: 'The value of the field exceeds the maximum number of characters, which is @1@.',
    valorMenosNumCarac: 'The value of the field does not have the minimum number of character, which is @1@.',
    valorNoNumCarac: 'The value of the field does not have the required number of characters, which is @1@.',
    valorNoEntreNumCarac: 'The value of the field must have a number of El valor del campo debe contener un número de caracteres entre @1@ y @2@, ambos inclusive.',
    cardi1: 'This property must have 1 element.',
    cardiVarios: 'This property must have @1@ elements.',
    maxCardi1: 'This property cannot have more than 1 element.',
    maxCardiVarios: 'This property cannot have more than @1@ elements.',
    minCardi1: 'At least 1 element is required for this property.',
    minCardiVarios: 'At least @1@ elements are required for this property',
    algunCampoMalIntro: 'You cannot save. Some field is incorrect.',
    malTags: 'At least one tag is required.',
    cargandoElem: 'Loading element',
    errorCarMasAtr: 'Todos los recursos deben tener título, descripción y tags.',
    propSinIdiomaDefecto: 'La propiedad no tiene introducido el texto en el idioma por defecto.',
    hayPropEditandose: 'Hay alguna propiedad en edición sin guardar, debes guardarla antes de continuar.',
    errorSubirAdjunto: 'Se ha producido un error al subir el adjunto. Intentalo de nuevo.',
    errorNoTodosIdiomas: 'Esta propiedad debe tener valor en todos los idiomas usados en el resto de propiedades.',
    errorSubirJCropSize: 'La imagen seleccionada no tiene el tamaño mínimo necesario. Tamaño mínimo: @1@ x @2@ px.',
    errorFechaConHora: 'El valor del campo no es una fecha con hora correcta (Ejm: 26/12/2009 17:15:30).'
};

var textoRecursos = {
    EditoresOK: 'Editors have been added correctly.',
    LectoresOK: 'Readers have been added correctly.',
    compartirOK: 'The resource has been shared correctly.',
    compartirBROK: 'The resource has been saved correctly.',
    comentBloqOK: 'The comments of the resource have been blocked correctly.',
    comentDesBloqOK: 'The comments of the resource have been unblocked correctly.',
    categoriasOK: 'The categories have been added successfully.',
    categoriasFALLO: 'There was an error adding categories, try again later.',
    tagsOK: 'Tags have been added correctly.',
    tagsFALLO: 'There was an error adding tags, try again later.',
    certificacionOK: 'The resource has been properly certified.',
    certificacionFALLO: 'There was an error certifying the resource, try again later.',
    recursoVinculadoOK: 'The resource has been linked correctly.',
    compararVersionesFALLO: 'There was an error to comparing versions, try again later.',
    restaurarFALLO: 'There was an error restoring the version, try again later.',
    Eliminar: 'Delete',
    SolicitarCatOK: 'The application has been made correctly.',
    SolicitarCatFALLO: 'You have not filled the fields correctly.',
    DesvincularOK: 'The resource has been properly disconnected.',
    DesvincularFALLO: 'There was an error unlinking the resource.',
    subiendoArchivo: 'Upload file',
    VerRecVincGrafoRec: 'See the @1@ related resources >>',
    VerRecVincGrafoRecSubTipo: 'Ver los @1@ recursos relacionados de tipo @2@ >>',
    guardando: 'Saving',
    archivoSubido: 'Archivo subido',
    publicar: 'Publicar',
    seguroEliminar: '¿Estás seguro de que deseas eliminar los recursos seleccionados?',
    soloUnaCat: 'Sólo puede seleccionarse una categoría.',
    almenosunaCat: 'Debes seleccionar al menos una categoría.',
    guardadoOK: 'Saved successfully.',
    soloUnElem: 'You only can select one element.',
    almenosunElem: 'You must to select at least one element.',
    recursos: 'resources',
    recurso: 'resource'
};

var textChat = {
    chats: 'Chats',
    iniciarChat: 'Iniciar Chat',
    desconectar: 'Desconectar',
    conectar: 'Conectar',
    contraer: 'Contraer',
    expandir: 'Expandir',
    atras: 'Atrás',
    enviar: 'Enviar',
    cargando: 'Cargando',
    cargarMensAnt: 'Cargar Mensajes Anteriores',
    errorSend: 'Se ha producido un error al enviar el mensaje.',
    errorReg: 'Se ha producido un error al activar el chat.',
    errorDesac: 'Se ha producido un error al dactivar el chat.',
    errorChat: 'Se ha producido un error al cargar la conversación.',
    errorCargarMenAnt: 'Se ha producido un error al cargar los mensajes anteriores.',
    enviando: 'Enviando',
    enviado: 'Enviado'
};

var consultorTic = {
    errorPregNoPre: 'Debes formular una pregunta.',
    errorPregNoTema: 'Debes seleccionar un tema para consulta.',
    errorNoCom: 'Debes ser miembro de la comunidad.',
    errorLength: 'La longitud de la pregunta no puede superar los 1000 caracteres. Tu pregunta tiene @1@ caracteres.'
};

var controlesRapidos = {
    errorRellCampos: 'Debes rellenar todos los campos.',
    errorMalGuard: 'Se ha produccido un error a la hora de guardar. Inténtalo más tarde',
    errorFaltanTags: 'Se ha produccido un error a la hora de guardar. Es necesario insertar al menos una etiqueta.',
    errorFaltaDesc: 'Se ha produccido un error a la hora de guardar. Es necesario insertar una descripción.',
    publicCorrect: 'Recurso publicado correctamente.',
    CVGuardCorrect: 'Información guardada correctamente.',
    errorVideoBri: 'Se ha produccido un error al subir el video, inténtalo más tarde.'
};

var ExpresionRegularNombres = /^([a-zA-Z0-9-\sñÑüÜáéíóúÁÉÍÓÚàèìòùÀÈÌÒÙâêîôûÂÊÎÔÛ'´`Çç]{0,})$/;
var ExpresionNombreCorto = /^([a-zA-Z0-9ñÑüÜáéíóúÁÉÍÓÚàèìòùÀÈÌÒÙâêîôûÂÊÎÔÛ'´`çÇ-]{4,30})$/;
var ExpresionNombreCortoCentro = /^([a-zA-Z0-9ñÑüÜáéíóúÁÉÍÓÚàèìòùÀÈÌÒÙâêîôûÂÊÎÔÛ '´`çÇ-]{2,10})$/;
var lang = 'en';

if ($.datepicker != undefined) {
    $.datepicker.regional['en-us'] = {
        clearText: 'Delete',
        clearStatus: '',
        closeText: 'Close',
        closeStatus: '',
        prevText: '&#x3c;Ant',
        prevStatus: '',
        prevBigText: '&#x3c;&#x3c;',
        prevBigStatus: '',
        nextText: 'Sig&#x3e;',
        nextStatus: '',
        nextBigText: '&#x3e;&#x3e;',
        nextBigStatus: '',
        currentText: 'Today',
        currentStatus: '',
        monthNames: ['January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December'],
        monthNamesShort: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'],
        monthStatus: '',
        yearStatus: '',
        weekHeader: 'Sm',
        weekStatus: '',
        dayNames: ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'],
        dayNamesShort: ['Sun', 'Mon', 'Tue', 'Wed;', 'Thu', 'Fri', 'Sat'],
        dayNamesMin: ['S', 'M', 'T', 'W', 'T', 'F', 'S'],
        dayStatus: 'DD',
        dateStatus: 'D, M d',
        dateFormat: 'dd/mm/yy',
        buttonText: ' ',
        firstDay: 1,
        initStatus: '',
        isRTL: false
    };
    $.datepicker.setDefaults($.datepicker.regional['en-us']);
}

var accionesUsuarioAdminComunidad = {
    expulsarUsuario: 'Expel member',
    motivoExpulsion: 'Enter the reason for the expulsion (an email is going to be sent to the member): ',
    expulsionMotivoVacio: 'The reason for the expulsion can\'t be empty',
    miembroExpulsado: 'Member expelled',
    cambiarRol: 'Change rol',
    selecionaRol: 'Select the rol the user performs in the community',
    rolCambiado: 'Rol changed',
    administrador: 'Administrator',
    supervisor: 'Supervisor',
    usuario: 'User'
};