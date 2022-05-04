( function() {
    CKEDITOR.plugins.add( 'youtube',
    {
		requires: 'dialog',
		lang: 'en,es,pt,ca,eu,ga,fr,de,it', // %REMOVE_LINE_CORE%
        icons: 'youtube', // %REMOVE_LINE_CORE%
        hidpi: true, // %REMOVE_LINE_CORE%
        init: function( editor )
        {
			var pluginName = 'youtube';

			CKEDITOR.dialog.add( pluginName, this.path + 'dialogs/youtube.js' );
			
			var allowed = 'iframe[*]';
			
            editor.addCommand( pluginName, new CKEDITOR.dialogCommand( pluginName,
                { allowedContent: allowed }
            ) );

            editor.ui.addButton && editor.ui.addButton( 'Youtube', {
                label: 'Youtube',
                command: pluginName,
				icon: this.path + "images/youtube.png"
            } );
        }
    } );
} )();
