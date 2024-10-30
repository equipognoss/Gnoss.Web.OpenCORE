var configuracion = {
    idioma: 'es'
};
var suscripciones = {
    enviarEnlace: 'Enviar enlace',
    URLenviarEnlace: 'enviar-enlace',
    marcarLeido: 'Marcar como leído'
};

var metaBuscador = {
    personasYorganizaciones: 'Personas y organizaciones',
    Debates: 'Debates',
    Preguntas: 'Preguntas',
    Recursos: 'Recursos',
    BusquedaAvanzada: 'Búqueda avanzada'
};

var mensajes = {
    enviarMensaje: 'Enviar mensaje',
    enviar: 'Enviar',
    asunto: 'Asunto',
    descripcion: 'Descripción',
    mensajeEnviado: 'Mensaje enviado',
    mensajeError: 'El asunto y la descripción no pueden estar vacíos',
    sinLeer: 'sin leer',
    nuevos: 'nuevos',
    infoPerderMen: 'El mensaje actual no ha sido enviado y se perderá.',
    infoPerderMen2: 'El mensaje actual no ha sido enviado y se perderá. \n\n¿Seguro que quieres abandonar esta página?',
    recibidos: 'Mensajes recibidos',
    enviados: 'Mensajes enviados',
    eliminados: 'Mensajes eliminados',
    mensaje: 'Mensaje',
    volverlistado: 'volver a la lista de mensajes',
    agrupacionGrafoTitulo: ' entidades relacionadas de tipo '
};
var dialogo = {
    confirmarSuscripcion: '<p>¿Deseas suscribirte al perfil de <em>@1@</em>?</p>',
    eliminarSuscripcion: '<p>¿Deseas eliminar tu suscripcion al perfil de <em>@1@</em>?</p>',
    confirmarSuscripcionBlog: '<p>¿Deseas suscribirte al blog <em>@1@</em> ?</p>',
    eliminarSuscripcionBlog: '<p>¿Deseas eliminar tu suscripcion al blog <em>@1@</em> ?</p>'
};
var comentarios = {
    comentario: 'Comentario',
    editarComentario: 'Editar comentario',
    responderComentario: 'Responder comentario',
    publicarcomentario: 'Publicar comentario',
    enviar: 'Enviar',
    guardar: 'Guardar',
    comentarioError: 'El comentario no puede enviarse vacío'
};
var borr = {
    si: 'sí',
    no: 'no',
    eliminar: 'eliminar',
    suscripcion: '¿Seguro que deseas cancelar esta suscripción?',
    borrador: '¿Seguro que deseas eliminar este borrador?',
    comentario: '¿Seguro que deseas eliminar este comentario?'
};
var invitaciones = {
    correosVacios: 'Debes introducir al menos una dirección de correo electrónico.',
    correoErroneo: 'Alguno de los correos introducidos no es válido.',
    usuariosVacios: 'Debes seleccionar al menos un usuario.',
    aceptar: '¿Deseas aceptar la invitación?',
    ignorar: '¿Deseas ignorar la invitación?',
    correoPerteneceOrg: 'El correo introducido pertenece a un usuario que ya participa en la organización',
    correoPerteneceCom: 'El correo introducido pertenece a un usuario que ya participa en la comunidad',
    invitarContactoAceptar: '¿Quieres hacer contacto a @1@? Vamos a notificar a @2@ que quieres añadirlo como contacto.',
    ingnorarContactoAceptar: '¿Quieres ignorar a @1@? No será sugerido más.',
    ingnorarProyAceptar: '¿Quieres ignorar el proyecto @1@? No será sugerido más.',
    haztemiembroProyAceptar: '¿Quieres hacerte miembro del proyecto @1@?',
    solicitaraccesoProyAceptar: '¿Quieres solicitar acceso al proyecto @1@?'
};
var susc = {
    aceptarSuscCat: '¿Deseas suscribirte a esta categoria?',
    eliminarSuscCat: '¿Deseas eliminar la suscripción a esta categoria?'
};

var relacionesTags = {
    avisolegal: "Esta información pertenece a terceros. RIAM no se responsabiliza de la veracidad de los datos ni de la información contenida"
};

var widget = {
    verCodigo: 'Ver el código del widget',
    verOpciones: 'Ver las opciones de configuración'
};
var calendario = {
    idioma: 'es',
    desde: 'Desde',
    hasta: 'Hasta'
};
var categorias = {
    nombre: 'Nombre',
    nombreEspanol: 'Nombre (Español)',
    nombreIngles: 'Nombre (Inglés)',
    nombrePortugues: 'Nombre (Portugués)',
    espanol: 'Español',
    ingles: 'Inglés',
    portugues: 'Portugués',
    desactivarEdicionMultIidioma: '¿Estás seguro de que quieres desactivar la edición en varios idiomas del índice de categorías de tu comunidad? Si lo haces, se tomará como idioma de la comunidad el que hayas elegido por defecto para la comunidad',
    seleccionarIdioma: 'Elije el idioma que quieres mantener: ',
    aceptar: 'Aceptar',
    cancelar: 'Cancelar'
};
var form = {
    mas: 'Más',
    masMIN: 'más',
    menos: 'menos',
    muysegura: 'Muy segura',
    segura: 'Segura',
    pocosegura: 'Poco segura',
    insegura: 'Insegura',
    errorLoginVacio: 'Debes introducir usuario y contraseña',
    errorLogin: 'El usuario o la contraseña son incorrectos',
    usuarioCorto: 'El nombre de usuario es demasiado corto (mínimo 4 caracteres)',
    longitudUsuario: 'El nombre de usuario debe contener entre 4 y 12 caracteres',
    formatoUsuario: 'El nombre de usuario no puede contener espacios ni caracteres especiales, salvo: . - _',
    formatoNombre: 'El nombre de usuario no puede contener caracteres especiales',
    formatoNombreCaracteresRepetidos: 'El nombre de usuario no puede contener mas de 2 caracteres iguales consecutivos',
    formatoApellidos: 'Los apellidos del usuario no pueden contener caracteres especiales',
    formatoApellidosCaracteresRepetidos: 'Los apellidos del usuario no pueden contener mas de 2 caracteres iguales consecutivos',
    formatoProvincia: 'La provincia no puede contener los caracteres < o >',
    formatoCP: 'El código postal no puede contener los caracteres < o >',
    formatoLocalidad: 'La localidad no puede contener los caracteres < o >',
    formatoRazonSocial: 'La razón social no puede contener los caracteres < o >',
    formatoAlias: 'El nombre de la organización no puede contener los caracteres < , > o ","',
    formatoCIF: 'El CIF no puede contener los caracteres < o >',
    formatoDireccion: 'La dirección no puede contener los caracteres < o >',
    pwCorta: 'La contraseña debe tener entre 6 y 12 caracteres.',
    pwFormato: 'La contraseña debe tener entre 6 y 12 caracteres, al menos una letra y un número, y no puede contener caracteres especiales, excepto: # _ $ *',
    organizacionRepetida: 'El nombre de esa organización ya está siendo utilizado.',
    pwIgual: 'Los campos de contraseña no coinciden',
    emailValido: 'El correo electrónico no es válido',
    obligatorio: ' es un campo obligatorio',
    aceptarLegal: 'Debes leer y aceptar las condiciones de acceso y uso',
    mayorEdad: 'Para ser miembro de la comunidad debes tener al menos 18 años',
    camposVacios: 'Debes rellenar todos los campos marcados con *',
    usuariorepetido: 'El nombre de usuario ya está siendo utilizado.',
    emailrepetido: 'La dirección de correo electrónico ya está siendo utilizada.',
    paginaweb: 'La página web no es válida',
    captchaincorrecto: 'El resultado de la operación matemática es incorrecto',
    nifincorrecto: 'El campo <i>Número de identificación fiscal</i> es erroneo',
    cifincorrecto: 'El campo <i>Código de identificación fiscal</i> es erroneo',
    fechafunincorrecta: 'Debes establecer la fecha de fundación',
    fechanacincorrecta: 'Debes establecer la fecha de nacimiento',
    fechanacinsuficiente: 'Debes tener 14 años para poder registrarse',
    fechanacinsuficiente18: 'Para poder registrarte tienes que tener 18 años',
    fechafunincorrectaformato: 'Debes establecer una fecha de fundación correcta',
    fechanacincorrectaformato: 'Debes establecer una fecha de nacimiento correcta',
    sexoincorrecto: 'Debes establecer tu sexo',
    tipoorgincorrecta: 'Debes establecer el tipo de organización',
    sectorincorrecto: 'Debes establecer el sector',
    paisincorrecto: 'Debes establecer el país',
    provinciaincorrecta: 'Debes establecer la provincia',
    numeroemplincorrecto: 'Debes establecer el número de empleados',
    modooperacionincorrecto: 'Debes establecer el modo de operación de la organización',
    nombrecortoincorrecto: 'El nombre de tu página personal debe tener entre 4 y 30 caracteres que pueden ser números, letras o guión ("-")',
    grupoincorrecto: 'Los grupos deben ser letras separadas por comas',
    nombrecortoincorrectocentro: 'El nombre corto del centro debe tener entre 2 y 10 caracteres que pueden ser números, letras, espacio (" ") o guión ("-")',
    nombrecortoincorrectoasignatura: 'El nombre corto de la asignatura debe tener entre 2 y 10 caracteres que pueden ser números, letras, espacio (" ") o guión ("-")',
    nombrecortoincorrectocurso: 'El curso no puede tener más de 10 caracteres que deben ser números',
    debesseleccionartipoclase: 'Debes seleccionar el tipo de clase',
    nombrecortorepetido: 'El nombre de la página personal ya está siendo utilizado por otro usuario',
    nombrecortoincorrectoorg: 'El nombre de tu página corporativa debe tener entre 4 y 30 caracteres que pueden ser números, letras o guión ("-")',
    nombrecortorepetidoorg: 'El nombre de la página corporativa ya está siendo utilizado por otra organización',
    nombrecortorepetidoclase: 'El nombre de la clase ya está siendo utilizado',
    proyectonoseleccionado: 'Debes seleccionar al menos una comunidad',
    nombrecortousuario: 'El nombre de la página corporativa no puede ser el mismo que el de la página del usuario creado',
    perderdatosbio: 'Has realizado cambios que no has guardado.\nSi continúas y abandonas la página, todos tus cambios se perderán.',
    generarversionauto: 'El documento ha cambiado, ¿Deseas generar una nueva versión del recurso?',
    generarversionautoinfo: 'Si no generas versión se reemplazará el documento actual',
    especificaDocumento: 'Debes subir un documento',
    errordtitulo: 'Debes indicar un título',
    errordurl: 'Debes indicar una URL',
    errorddescripcion: 'Debes indicar una descripción',
    errordtag: 'Debes indicar al menos una etiqueta',
    errordcategoria: 'Es obligatorio vincular un recurso a una categoría',
    errordcategoriaSelect: 'Es obligatorio seleccionar una categoría',
    errordcategoriapregunta: 'Es obligatorio vincular una pregunta a una categoría',
    errordcategoriadebate: 'Es obligatorio vincular un debate a una categoría',
    errordcategoriaencuesta: 'Es obligatorio vincular una encuesta a una categoría',
    nombrecortocomunidadincorrecto: 'El nombre corto de tu comunidad debe tener entre 4 y 30 caracteres que pueden ser números, letras o guión ("-")',
    campossolicitudcomunidadincompletos: 'Debes completar todos los campos para enviar la solicitud.',
    nombrecomunidadincorrecto: 'El nombre de tu comunidad no puede contener los caracteres < o >',
    errorVersiones: 'Debes seleccionar 2 versiones',
    caracteresRestantes: 'Caracteres restantes: ',
    centroestudiosincorrecto: 'Debes establecer el centro de estudios',
    areaestudiosincorrecto: 'Debes establecer el area de estudios',
    fichaTecnicaPDF: 'Ficha técnica / Technical sheet – PDF',
    eliminar: 'eliminar',
    tagsPropuestos: 'Etiquetas propuestas:',
    cargando: 'Cargando...',
    selectImg: 'Selecciona una imagen',
    validExtensions: 'Extensiones validas: .jpg, .jpeg, .gif, .png',
    cargando: 'Cargando',
    htmlincrustado: 'Html incrustado',
    html: 'Html',
    vermas: 'Ver más',
    vermenos: 'Ver menos'
};
//La estructura de tiempos varía en inglés y en español:hace 2 días, 2 days ago. Solución provisional 'Hace' lo dejamos en blanco, ponemos ago después de expresiones temporales-->
var tiempo = {
    hace: 'Hace',
    el: 'El',
    eldia: 'El',
    ayer: 'Ayer',
    haceMinus: 'hace',
    elMinus: 'el',
    ayerMinus: 'ayer',
    minuto: 'minuto',
    minutos: 'minutos',
    hora: 'hora',
    horas: 'horas',
    dia: 'día',
    dias: 'días',
    semana: 'semana',
    fechaBarras: '@1@/@2@/@3@',
    fechaPuntos: '@1@.@2@.@3@'
};

var textoFormSem = {
    confimEliminar: '¿Desea eliminar el elemento \\\'@1@\\\'?',
    confimEliminarEntidad: '¿Desea eliminar la entidad?',
    propiedadObliValor: 'Esta propiedad debe tener valor obligatoriamente.',
    errorFecha: 'El valor del campo no es una fecha correcta (Ejm: 26/12/2009).',
    errorNumEntero: 'El valor del campo debe ser un número entero.',
    errorNum: 'El valor del campo debe ser un número.',
    valorExcedeNumCarac: 'El valor del campo excede el número máximo de caracteres, que es @1@.',
    valorMenosNumCarac: 'El valor del campo no contiene el mínimo de caracteres, que es @1@.',
    valorNoNumCarac: 'El valor del campo no contiene el número de caracteres obligados, que es @1@.',
    valorNoEntreNumCarac: 'El valor del campo debe contener un número de caracteres entre @1@ y @2@, ambos inclusive.',
    cardi1: 'Esta propiedad debe tener 1 elemento.',
    cardiVarios: 'Esta propiedad debe tener @1@ elementos.',
    maxCardi1: 'Esta propiedad como máximo puede tener 1 elemento.',
    maxCardiVarios: 'Esta propiedad como máximo puede tener @1@ elementos.',
    minCardi1: 'Esta propiedad como mínimo debe tener 1 elemento.',
    minCardiVarios: 'Esta propiedad como mínimo debe tener @1@ elementos.',
    algunCampoMalIntro: 'No se puede guardar. Algún campo está mal introducido.',
    malTags: 'Debe introducir al menos una etiqueta.',
    cargandoElem: 'Cargando elemento',
    errorCarMasAtr: 'Todos los recursos deben tener título, descripción y tags.',
    propSinIdiomaDefecto: 'La propiedad no tiene introducido el texto en el idioma por defecto.',
    hayPropEditandose: 'Hay alguna propiedad en edición sin guardar, debes guardarla antes de continuar.',
    errorSubirAdjunto: 'Se ha producido un error al subir el adjunto. Intentalo de nuevo.',
    errorNoTodosIdiomas: 'Esta propiedad debe tener valor en todos los idiomas usados en el resto de propiedades.',
    errorSubirJCropSize: 'La imagen seleccionada no tiene el tamaño mínimo necesario. Tamaño mínimo: @1@ x @2@ px.',
    errorFechaConHora: 'El valor del campo no es una fecha con hora correcta (Ejm: 26/12/2009 17:15:30).',
    SubtipoObligatorio: "Debe seleccionar el subtipo"
};

var textoRecursos = {
    EditoresOK: 'Se han agregado los editores correctamente.',
    LectoresOK: 'Se han agregado los lectores correctamente.',
    duplicarFALLO: 'Se ha producido un error al duplicar el recurso, inténtalo más tarde.',
    compartirOK: 'Se ha compartido el recurso correctamente.',
    compartirFALLO: 'Se ha producido un error al compartir el recurso, inténtalo más tarde.',
    compartirBROK: 'Se ha guardado el recurso correctamente.',
    compartirBRFALLO: 'Se ha producido un error al guardar el recurso, inténtalo más tarde.',
    comentBloqOK: 'Se han bloqueado los comentarios del recurso correctamente.',
    comentDesBloqOK: 'Se han desbloqueado los comentarios del recurso correctamente.',
    categoriasOK: 'Se han agregado las categorías correctamente.',
    categoriasFALLO: 'Se ha producido un error al agregar categorías, inténtalo más tarde.',
    tagsOK: 'Se han agregado los tags correctamente.',
    tagsFALLO: 'Se ha producido un error al agregar etiquetas, inténtalo más tarde.',
    certificacionOK: 'Se ha certificado correctamente.',
    certificacionFALLO: 'Se ha producido un error al certificar el recurso, inténtalo más tarde.',
    recursoVinculadoOK: 'Se ha vinculado el recurso correctamente.',
    compararVersionesFALLO: 'Se ha producido un error al comparar versiones, inténtalo más tarde.',
    restaurarFALLO: 'Se ha producido un error al restaurar la versión, inténtalo más tarde.',
    Eliminar: 'Eliminar',
    SolicitarCatOK: 'Se ha realizado la solicitud correctamente.',
    SolicitarCatFALLO: 'No ha rellenado los campos correctamente.',
    DesvincularOK: 'Se ha desvinculado el recurso correctamente.',
    DesvincularFALLO: 'Se ha producido un error al desvicular el recurso.',
    subiendoArchivo: 'Subiendo archivo',
    VerRecVincGrafoRec: 'Ver los @1@ recursos relacionados >>',
    VerRecVincGrafoRecSubTipo: 'Ver los @1@ recursos relacionados de tipo @2@ >>',
    guardando: 'Guardando',
    archivoSubido: 'Archivo subido',
    publicar: 'Publicar',
    seguroEliminar: '¿Estás seguro de que deseas eliminar los recursos seleccionados?',
    soloUnaCat: 'Sólo puede seleccionarse una categoría.',
    almenosunaCat: 'Debes seleccionar al menos una categoría.',
    guardadoOK: 'Se ha guardado correctamente.',
    soloUnElem: 'Sólo puede seleccionarse un elemento.',
    almenosunElem: 'Debes seleccionar al menos un elemento.',
    recursos: 'recursos',
    recurso: 'recurso'
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
var lang = 'es';

if ($.datepicker != undefined) {
    $.datepicker.regional['es'] = {
        clearText: 'Borrar',
        clearStatus: '',
        closeText: 'Cerrar',
        closeStatus: '',
        prevText: '&#x3c;Ant',
        prevStatus: '',
        prevBigText: '&#x3c;&#x3c;',
        prevBigStatus: '',
        nextText: 'Sig&#x3e;',
        nextStatus: '',
        nextBigText: '&#x3e;&#x3e;',
        nextBigStatus: '',
        currentText: 'Hoy',
        currentStatus: '',
        monthNames: ['Enero', 'Febrero', 'Marzo', 'Abril', 'Mayo', 'Junio', 'Julio', 'Agosto', 'Septiembre', 'Octubre', 'Noviembre', 'Diciembre'],
        monthNamesShort: ['Ene', 'Feb', 'Mar', 'Abr', 'May', 'Jun', 'Jul', 'Ago', 'Sep', 'Oct', 'Nov', 'Dic'],
        monthStatus: '',
        yearStatus: '',
        weekHeader: 'Sm',
        weekStatus: '',
        dayNames: ['Domingo', 'Lunes', 'Martes', 'Mi&eacute;rcoles', 'Jueves', 'Viernes', 'S&aacute;bado'],
        dayNamesShort: ['Dom', 'Lun', 'Mar', 'Mi&eacute;', 'Juv', 'Vie', 'S&aacute;b'],
        dayNamesMin: ['D', 'L', 'M', 'X', 'J', 'V', 'S'],
        dayStatus: 'DD',
        dateStatus: 'D, M d',
        dateFormat: 'dd/mm/yy',
        buttonText: ' ',
        firstDay: 1,
        initStatus: '',
        isRTL: false
    };
    $.datepicker.setDefaults($.datepicker.regional['es']);
}


var accionesUsuarioAdminComunidad = {
    expulsarUsuario: 'Expulsar miembro',
    motivoExpulsion: 'Introduce el motivo de la expulsión (se enviará un correo al miembro expulsado): ',
    expulsionMotivoVacio: 'Es obligatorio introducir el motivo de la expulsión',
    miembroExpulsado: 'Miembro expulsado',
    cambiarRol: 'Cambiar rol',
    selecionaRol: 'Selecciona el rol que desempeñará en la comunidad el usuario seleccionado',
    rolCambiado: 'Rol cambiado',
    administrador: 'Administrador',
    supervisor: 'Supervisor',
    usuario: 'Usuario'
};

var descargarTraduccionesErrores = {
    error: '',
    errorNoHayArchivos: 'No hay archivos que descargar',
    errorFecha: 'La fecha final debe ser mayor que la fecha inicial',
    validar: 'Validado',
    errorValidar: 'Archivo no validado'
};

var espacioPersonal = {
    avisoEditarOrganizarEnCategorias: 'Selecciona un recurso antes de intentar editar su categoría',
};

var facetas = {
    siguientes: "Siguientes",
    anteriores: "Anteriores",
};

var accionesUsuarios = {
    seguir: "Seguir",
    dejarDeSeguir: "Dejar de seguir",
    sinSeguimiento: "Sin seguimiento",
};