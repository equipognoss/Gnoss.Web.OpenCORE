( function() {
    CKEDITOR.plugins.add( 'vimeo',
    {
		requires: 'dialog',
		lang: 'en,es,pt,ca,eu,ga,fr,de,it', // %REMOVE_LINE_CORE%
        icons: 'vimeo', // %REMOVE_LINE_CORE%
        hidpi: true, // %REMOVE_LINE_CORE%
        init: function( editor )
        {
			var pluginName = 'vimeo';

			CKEDITOR.dialog.add( pluginName, this.path + 'dialogs/vimeo.js' );
			
			var allowed = 'iframe[*]';
			
            editor.addCommand( pluginName, new CKEDITOR.dialogCommand( pluginName,
                { allowedContent: allowed }
            ) );

            editor.ui.addButton && editor.ui.addButton( 'Vimeo', {
                label: 'Vimeo',
                command: pluginName,
				icon: this.path + "images/vimeo.png"
            } );
        }
    } );
} )();