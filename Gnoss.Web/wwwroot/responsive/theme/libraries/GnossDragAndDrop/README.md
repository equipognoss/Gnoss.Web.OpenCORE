---

# GnossDragAndDrop

### VERSION:

0.0.1

### REQUIRES:

jQuery ^3.x

## BASIC IMPLEMENTATION

### HTML:

    <input type="file" name="" id="prueba_drag" accept="image/*">

### JS:

    $('#prueba_drag').GnossDragAndDrop();

## ADVANCE CUSTOMIZATION

### OPTIONS:

Some options can be configured when initialize the plugin:

**maxSize**

Maximum size allowed for the file. If exceded it will show an error.
Must be in KB. Default size is 2000KB (2MB)

Example:

    $('#prueba_drag').GnossDragAndDrop({
        maxSize: 2000
    });

**acceptedFiles**

Type of file the plugin will accept
Default value is accept all extenxions

Value can be:

-   "\*" for all kind of files (default)
-   "image" or "image/\*" for all kind of image files
-   "audio" or "audio/\*" for all kind of audio files
-   "video" or "video/\*" for all kind of video files
-   Array of extensions ['jpg','png','txt',...]

Examples:

    $('#prueba_drag').GnossDragAndDrop({
        acceptedFiles: '*'
    });

    $('#prueba_drag').GnossDragAndDrop({
        acceptedFiles: 'audio/*'
    });

    $('#prueba_drag').GnossDragAndDrop({
        acceptedFiles: 'image'
    });


    $('#prueba_drag').GnossDragAndDrop({
        acceptedFiles: ['jpg','png','pdf','gif']
    });

**beforeValidation**

This function is executed right before the plugin starts validating the added file.
The two parameters available in this function are the plugin itself and the object files (the input value);

    $('#prueba_drag').GnossDragAndDrop({
        beforeValidation: function (plugin, files) {
            // your code
        },
    });

**onFileAdded**

This function is executed when the added file has been validated succesfully.
The two parameters available in this function are the plugin itself and the object files (the input value);

    $('#prueba_drag').GnossDragAndDrop({
        onFileAdded: function (plugin, files) {
            // your code
        },
    });

**onFileRemoved**

This function is executed when the file has been removed.
The two parameters available in this function are the plugin itself and the object files (the input value);

    $('#prueba_drag').GnossDragAndDrop({
        onFileRemoved: function (plugin, files) {
            // your code
        },
    });

### PRELOADED FILES

If you want to load a file on the page load to the input file the file url must be added as a data to the input.
For image files should be with the attribute data-image-url, like this:
<input class="dragAndDrop" type="file" data-image-url="http://dominio.com/image.png">

For image files should be with the attribute data-image-url, like this:
<input class="dragAndDrop" type="file" data-file-url="http://dominio.com/file.txt">

### TEXTS:

Plugin display has 3 text and 2 error messages:

-   Title. Default: "Arrastra y suelta en la zona punteada o haz clic para añadir el archivo"
-   Format. Default: "Archivos en formato [formats added dinamically]"
-   Peso. Default: "Peso máximo [max size added dinamically]"
-   Format error. Default: "Formato de archivo no permitido. Formatos admitidos [formats added dinamically]"
-   Size error. Default: "Formato de archivo no permitido, no puede ser más grande de [max size added dinamically]"

For different texts (or languages) input has to include these "data" with the desired texts:

    data-title-text="Drag and drop in the dotted area or click to add the file"
    data-format-text="Files in format .PNG, .JPG"
    data-size-text="Maximum weight 250kb"
    data-format-error="File format not supported. Formats allowed: .png .jpg"
    data-size-error="Maximum weight exceded. Maximum weight allowed 250kb"

Example:

    <input
        type="file" name=""
        id="prueba_drag"
        accept="image/*"
        data-size-text="Maximum weight 250kb"
        data-size-error="Maximum weight exceded. Maximum weight allowed 250kb"
    >

### METHODS:

**resetPlugin**

If you want to remove the file from the droparea, you can call resetPlugin(). This method removes the file form the input and also the name and image preview.

    var dragDrop_instance = $('#prueba_drag').GnossDragAndDrop(options);
    dragDrop_instance.GnossDragAndDrop('resetPlugin');

**addFile**

To add a file programatically you can use this method and the second parameter must be the file url.

    $('#prueba_drag').GnossDragAndDrop('addFile', 'http://dominio.com/file.txt');

To add an image a third parameter must be included as true:

    $('#prueba_drag').GnossDragAndDrop('addFile', 'http://dominio.com/image.png', true);

**displayError**

To show a custom error message call to displayError passing the error message

    $('#prueba_drag').GnossDragAndDrop('displayError', 'This is an example of error message');

**displaySizeError**

This function will show the Size error messege (Unless there is one message configured by the user with the "data-size-error" it will show the default message)

    $('#prueba_drag').GnossDragAndDrop('displaySizeError');

**displayFormatError**

This function will show the Size error messege (Unless there is one message configured by the user with the "data-format-error" it will show the default message)

Example:

    $('#prueba_drag').GnossDragAndDrop('displayFormatError');

**getFileExtension**

Return file extension

Example:

    $('#prueba_drag').GnossDragAndDrop('getFileExtension');

**getFile**

Return file

    $('#prueba_drag').GnossDragAndDrop('getFile');

**destroy**

Destroys plugin completely (removes input data and added html)

    $('#prueba_drag').GnossDragAndDrop('destroy');
