/*
Copyright (c) 2003-2011, CKSource - Frederico Knabben. All rights reserved.
For licensing, see LICENSE.html or http://ckeditor.com/license
*/

(function()
{
	/*
	 * It is possible to set things in three different places.
	 * 1. As attributes in the object tag.
	 * 2. As param tags under the object tag.
	 * 3. As attributes in the embed tag.
	 * It is possible for a single attribute to be present in more than one place.
	 * So let's define a mapping between a sementic attribute and its syntactic
	 * equivalents.
	 * Then we'll set and retrieve attribute values according to the mapping,
	 * instead of having to check and set each syntactic attribute every time.
	 *
	 * Reference: http://kb.adobe.com/selfservice/viewContent.do?externalId=tn_12701
	 */
	 
	//FERNANDO : Para transformar las urls de los videos que insertamos en el flash
	var oWebVideo = {
		Objects : [ "<object width=\"(?<width>\\d+)\" height=\"(?<height>\\d+)\".* name=\"movie\"\\s+value=\"(?<url>[^\"]*)\".*<\\/object>" ,
				// youtube style only embed
						"<embed src=\"(?<url>[^\"]*)\".* width=\"(?<width>\\d+)\" height=\"(?<height>\\d+)\".*<\\/embed>" ,
				// google video
						"<embed .*width:(?<width>\\d+)px.*height:(?<height>\\d+)px.* src=\"(?<url>[^\"]*)\".*<\\/embed>" ,
				// invalid syntax but anyway...
						"<embed src=\"(?<url>[^\"]*)\".* width=\"(?<width>\\d+)px\" height=\"(?<height>\\d+)px\".*<\\/embed>"
		] ,

		webPages : [{re:/http:\/\/www\.youtube\.com\/watch\?v=([^&]*)(&.*|$)/, url:'http://www.youtube.com/v/$1', width:425, height:344} ,
		                        {re:/http:\/\/youtu\.be\/([^&]*)(&.*|$)/, url:'http://www.youtube.com/v/$1', width:425, height:344} ,
								{re:/http:\/\/video\.google\.(.*)\/videoplay\?docid=([^&]*)(&.*|$)/, url:'http://video.google.$1/googleplayer.swf?docid=$2', width:400, height:326}, 
								{re:/http:\/\/www\.mtvmusic\.com\/video\/\?id=([^&]*)(&.*|$)/, url:'http://media.mtvnservices.com/mgid:uma:video:mtvmusic.com:$1', width:320, height:271} ,
								{re:/http:\/\/www\.metacafe\.com\/watch\/(.*?)\/(.*?)(\/.*|$)/, url:'http://www.metacafe.com/fplayer/$1/$2.swf', width:400, height:345} 
		],

		// Parses the suplied HTML and returns an object with the url of the video, its width and height
		ParseHtml : function( html)
		{
			// Check if it's a valid swf and skip the tests
			var swfFile = new RegExp(".*\.swf$", i) ;
			if ( swfFile.test(html) )
					return html ;

			// Generic system to work with any proposed embed by the site (as long as it matches the previous regexps
			for(var i=0; i< this.Objects.length; i++)
			{
				// Using XRegExp to work with named captures: http://stevenlevithan.com/regex/xregexp/
				var re = new XRegExp( this.Objects[i] ) ;
				var parts = re.exec( html ) ;
				if (parts)
					return parts.url;
			}

			// Ability to paste the url of the web site and extract the correct info. It needs to be adjusted for every site.
			for(var i=0; i< this.webPages.length; i++)
			{
				var page = this.webPages[i] ;
				var oMatch = html.match( page.re ) ;
				if (oMatch)
				{
					return html.replace(page.re, page.url);
				}
			}
			
			return html;
		}
	};
	
	var ATTRTYPE_OBJECT = 1,
		ATTRTYPE_PARAM = 2,
		ATTRTYPE_EMBED = 4;

	var attributesMap =
	{
		id : [ { type : ATTRTYPE_OBJECT, name :  'id' } ],
		classid : [ { type : ATTRTYPE_OBJECT, name : 'classid' } ],
		codebase : [ { type : ATTRTYPE_OBJECT, name : 'codebase'} ],
		pluginspage : [ { type : ATTRTYPE_EMBED, name : 'pluginspage' } ],
		src : [ { type : ATTRTYPE_PARAM, name : 'movie' }, { type : ATTRTYPE_EMBED, name : 'src' }, { type : ATTRTYPE_OBJECT, name :  'data' } ],
		name : [ { type : ATTRTYPE_EMBED, name : 'name' } ],
		align : [ { type : ATTRTYPE_OBJECT, name : 'align' } ],
		title : [ { type : ATTRTYPE_OBJECT, name : 'title' }, { type : ATTRTYPE_EMBED, name : 'title' } ],
		'class' : [ { type : ATTRTYPE_OBJECT, name : 'class' }, { type : ATTRTYPE_EMBED, name : 'class'} ],
		width : [ { type : ATTRTYPE_OBJECT, name : 'width' }, { type : ATTRTYPE_EMBED, name : 'width' } ],
		height : [ { type : ATTRTYPE_OBJECT, name : 'height' }, { type : ATTRTYPE_EMBED, name : 'height' } ],
		hSpace : [ { type : ATTRTYPE_OBJECT, name : 'hSpace' }, { type : ATTRTYPE_EMBED, name : 'hSpace' } ],
		vSpace : [ { type : ATTRTYPE_OBJECT, name : 'vSpace' }, { type : ATTRTYPE_EMBED, name : 'vSpace' } ],
		style : [ { type : ATTRTYPE_OBJECT, name : 'style' }, { type : ATTRTYPE_EMBED, name : 'style' } ],
		type : [ { type : ATTRTYPE_EMBED, name : 'type' } ]
	};

	var names = [ 'play', 'loop', 'menu', 'quality', 'scale', 'salign', 'wmode', 'bgcolor', 'base', 'flashvars', 'allowScriptAccess',
		'allowFullScreen' ];
	for ( var i = 0 ; i < names.length ; i++ )
		attributesMap[ names[i] ] = [ { type : ATTRTYPE_EMBED, name : names[i] }, { type : ATTRTYPE_PARAM, name : names[i] } ];
	names = [ 'allowFullScreen', 'play', 'loop', 'menu' ];
	for ( i = 0 ; i < names.length ; i++ )
		attributesMap[ names[i] ][0]['default'] = attributesMap[ names[i] ][1]['default'] = true;

	var defaultToPixel = CKEDITOR.tools.cssLength;

	function loadValue( objectNode, embedNode, paramMap )
	{
		var attributes = attributesMap[ this.id ];
		if ( !attributes )
			return;

		var isCheckbox = ( this instanceof CKEDITOR.ui.dialog.checkbox );
		for ( var i = 0 ; i < attributes.length ; i++ )
		{
			var attrDef = attributes[ i ];
			switch ( attrDef.type )
			{
				case ATTRTYPE_OBJECT:
					if ( !objectNode )
						continue;
					if ( objectNode.getAttribute( attrDef.name ) !== null )
					{
						var value = objectNode.getAttribute( attrDef.name );
						if ( isCheckbox )
							this.setValue( value.toLowerCase() == 'true' );
						else
							this.setValue( value );
						return;
					}
					else if ( isCheckbox )
						this.setValue( !!attrDef[ 'default' ] );
					break;
				case ATTRTYPE_PARAM:
					if ( !objectNode )
						continue;
					if ( attrDef.name in paramMap )
					{
						value = paramMap[ attrDef.name ];
						if ( isCheckbox )
							this.setValue( value.toLowerCase() == 'true' );
						else
							this.setValue( value );
						return;
					}
					else if ( isCheckbox )
						this.setValue( !!attrDef[ 'default' ] );
					break;
				case ATTRTYPE_EMBED:
					if ( !embedNode )
						continue;
					if ( embedNode.getAttribute( attrDef.name ) )
					{
						value = embedNode.getAttribute( attrDef.name );
						if ( isCheckbox )
							this.setValue( value.toLowerCase() == 'true' );
						else
							this.setValue( value );
						return;
					}
					else if ( isCheckbox )
						this.setValue( !!attrDef[ 'default' ] );
			}
		}
	}

	function commitValue( objectNode, embedNode, paramMap )
	{
		var attributes = attributesMap[ this.id ];
		if ( !attributes )
			return;

		var isRemove = ( this.getValue() === '' ),
			isCheckbox = ( this instanceof CKEDITOR.ui.dialog.checkbox );

		for ( var i = 0 ; i < attributes.length ; i++ )
		{
			var attrDef = attributes[i];
			switch ( attrDef.type )
			{
				case ATTRTYPE_OBJECT:
					// Avoid applying the data attribute when not needed (#7733)
					if ( !objectNode || ( attrDef.name == 'data' && embedNode && !objectNode.hasAttribute( 'data' ) ) )
						continue;
					var value = this.getValue();
					if ( isRemove || isCheckbox && value === attrDef[ 'default' ] )
						objectNode.removeAttribute( attrDef.name );
					else
						objectNode.setAttribute( attrDef.name, value );
					break;
				case ATTRTYPE_PARAM:
					if ( !objectNode )
						continue;
					value = this.getValue();
					if ( isRemove || isCheckbox && value === attrDef[ 'default' ] )
					{
						if ( attrDef.name in paramMap )
							paramMap[ attrDef.name ].remove();
					}
					else
					{
						if ( attrDef.name in paramMap )
							paramMap[ attrDef.name ].setAttribute( 'value', value );
						else
						{
							var param = CKEDITOR.dom.element.createFromHtml( '<cke:param></cke:param>', objectNode.getDocument() );
							param.setAttributes( { name : attrDef.name, value : value } );
							if ( objectNode.getChildCount() < 1 )
								param.appendTo( objectNode );
							else
								param.insertBefore( objectNode.getFirst() );
						}
					}
					break;
				case ATTRTYPE_EMBED:
					if ( !embedNode )
						continue;
					value = this.getValue();
					if ( isRemove || isCheckbox && value === attrDef[ 'default' ])
						embedNode.removeAttribute( attrDef.name );
					else
						embedNode.setAttribute( attrDef.name, value );
			}
		}
	}

	CKEDITOR.dialog.add( 'flash', function( editor )
	{
		var makeObjectTag = !editor.config.flashEmbedTagOnly,
			makeEmbedTag = editor.config.flashAddEmbedTag || editor.config.flashEmbedTagOnly;

		var previewPreloader,
			previewAreaHtml = '<div>' + CKEDITOR.tools.htmlEncode( editor.lang.common.preview ) +'<br>' +
			'<div id="cke_FlashPreviewLoader' + CKEDITOR.tools.getNextNumber() + '" style="display:none"><div class="loading">&nbsp;</div></div>' +
			'<div id="cke_FlashPreviewBox' + CKEDITOR.tools.getNextNumber() + '" class="FlashPreviewBox"></div></div>';

		return {
			title : editor.lang.flash.title,
			minWidth : 420,
			minHeight : 310,
			onShow : function()
			{
				// Clear previously saved elements.
				this.fakeImage = this.objectNode = this.embedNode = null;
				previewPreloader = new CKEDITOR.dom.element( 'embed', editor.document );

				// Try to detect any embed or object tag that has Flash parameters.
				var fakeImage = this.getSelectedElement();
				if ( fakeImage && fakeImage.data( 'cke-real-element-type' ) && fakeImage.data( 'cke-real-element-type' ) == 'flash' )
				{
					this.fakeImage = fakeImage;

					var realElement = editor.restoreRealElement( fakeImage ),
						objectNode = null, embedNode = null, paramMap = {};
					if ( realElement.getName() == 'cke:object' )
					{
						objectNode = realElement;
						var embedList = objectNode.getElementsByTag( 'embed', 'cke' );
						if ( embedList.count() > 0 )
							embedNode = embedList.getItem( 0 );
						var paramList = objectNode.getElementsByTag( 'param', 'cke' );
						for ( var i = 0, length = paramList.count() ; i < length ; i++ )
						{
							var item = paramList.getItem( i ),
								name = item.getAttribute( 'name' ),
								value = item.getAttribute( 'value' );
							paramMap[ name ] = value;
						}
					}
					else if ( realElement.getName() == 'cke:embed' )
						embedNode = realElement;

					this.objectNode = objectNode;
					this.embedNode = embedNode;

					this.setupContent( objectNode, embedNode, paramMap, fakeImage );
				}
			},
			onOk : function()
			{
				// If there's no selected object or embed, create one. Otherwise, reuse the
				// selected object and embed nodes.
				var objectNode = null,
					embedNode = null,
					paramMap = null;
				if ( !this.fakeImage )
				{
					if ( makeObjectTag )
					{
						objectNode = CKEDITOR.dom.element.createFromHtml( '<cke:object></cke:object>', editor.document );
						var attributes = {
							classid : 'clsid:d27cdb6e-ae6d-11cf-96b8-444553540000',
							codebase : 'http://download.macromedia.com/pub/shockwave/cabs/flash/swflash.cab#version=6,0,40,0'
						};
						objectNode.setAttributes( attributes );
					}
					if ( makeEmbedTag )
					{
						embedNode = CKEDITOR.dom.element.createFromHtml( '<cke:embed></cke:embed>', editor.document );
						embedNode.setAttributes(
							{
								type : 'application/x-shockwave-flash',
								pluginspage : 'http://www.macromedia.com/go/getflashplayer'
							} );
						if ( objectNode )
							embedNode.appendTo( objectNode );
					}
				}
				else
				{
					objectNode = this.objectNode;
					embedNode = this.embedNode;
				}

				// Produce the paramMap if there's an object tag.
				if ( objectNode )
				{
					paramMap = {};
					var paramList = objectNode.getElementsByTag( 'param', 'cke' );
					for ( var i = 0, length = paramList.count() ; i < length ; i++ )
						paramMap[ paramList.getItem( i ).getAttribute( 'name' ) ] = paramList.getItem( i );
				}

				// A subset of the specified attributes/styles
				// should also be applied on the fake element to
				// have better visual effect. (#5240)
				var extraStyles = {}, extraAttributes = {};
				this.commitContent( objectNode, embedNode, paramMap, extraStyles, extraAttributes );

				// Refresh the fake image.
				var newFakeImage = editor.createFakeElement( objectNode || embedNode, 'cke_flash', 'flash', true );
				newFakeImage.setAttributes( extraAttributes );
				newFakeImage.setStyles( extraStyles );
				if ( this.fakeImage )
				{
					newFakeImage.replace( this.fakeImage );
					editor.getSelection().selectElement( newFakeImage );
				}
				else
					editor.insertElement( newFakeImage );
			},

			onHide : function()
			{
				if ( this.preview )
					this.preview.setHtml('');
			},

			contents : [
				{
					id : 'info',
					label : editor.lang.common.generalTab,
					accessKey : 'I',
					elements :
					[
						{
							type : 'vbox',
							padding : 0,
							children :
							[
								{
									type : 'hbox',
									widths : [ '280px', '110px' ],
									align : 'right',
									children :
									[
										{
											id : 'src',
											type : 'text',
											label : editor.lang.common.url,
											required : true,
											validate : CKEDITOR.dialog.validate.notEmpty( editor.lang.flash.validateSrc ),
											setup : loadValue,
											commit : commitValue,
											onLoad : function()
											{
												var dialog = this.getDialog(),
												updatePreview = function( src ){
												    //FERNANDO : Para transformar las urls de los videos que insertamos en el flash
												    previewPreloader.setAttribute( 'src', oWebVideo.ParseHtml(src) );
													// Query the preloader to figure out the url impacted by based href.
													//previewPreloader.setAttribute( 'src', src );
													dialog.preview.setHtml( '<embed height="100%" width="100%" src="'
														+ CKEDITOR.tools.htmlEncode( previewPreloader.getAttribute( 'src' ) )
														+ '" type="application/x-shockwave-flash"></embed>' );
												};
												// Preview element
												dialog.preview = dialog.getContentElement( 'info', 'preview' ).getElement().getChild( 3 );

												// Sync on inital value loaded.
												this.on( 'change', function( evt ){

														if ( evt.data && evt.data.value )
															updatePreview( evt.data.value );
													} );
												// Sync when input value changed.
												this.getInputElement().on( 'change', function( evt ){

													updatePreview( this.getValue() );
												}, this );
											}
										},
										{
											type : 'button',
											id : 'browse',
											filebrowser : 'info:src',
											hidden : true,
											// v-align with the 'src' field.
											// TODO: We need something better than a fixed size here.
											style : 'display:inline-block;margin-top:10px;',
											label : editor.lang.common.browseServer
										}
									]
								}
							]
						},
						{
							type : 'hbox',
							widths : [ '25%', '25%', '25%', '25%', '25%' ],
							children :
							[
								{
									type : 'text',
									id : 'width',
									style : 'width:95px',
									label : editor.lang.common.width,
									validate : CKEDITOR.dialog.validate.htmlLength( editor.lang.common.invalidHtmlLength.replace( '%1', editor.lang.common.width ) ),
									setup : loadValue,
									commit : commitValue
								},
								{
									type : 'text',
									id : 'height',
									style : 'width:95px',
									label : editor.lang.common.height,
									validate : CKEDITOR.dialog.validate.htmlLength( editor.lang.common.invalidHtmlLength.replace( '%1', editor.lang.common.height ) ),
									setup : loadValue,
									commit : commitValue
								},
								{
									type : 'text',
									id : 'hSpace',
									style : 'width:95px',
									label : editor.lang.flash.hSpace,
									validate : CKEDITOR.dialog.validate.integer( editor.lang.flash.validateHSpace ),
									setup : loadValue,
									commit : commitValue
								},
								{
									type : 'text',
									id : 'vSpace',
									style : 'width:95px',
									label : editor.lang.flash.vSpace,
									validate : CKEDITOR.dialog.validate.integer( editor.lang.flash.validateVSpace ),
									setup : loadValue,
									commit : commitValue
								}
							]
						},

						{
							type : 'vbox',
							children :
							[
								{
									type : 'html',
									id : 'preview',
									style : 'width:95%;',
									html : previewAreaHtml
								}
							]
						}
					]
				},
				{
					id : 'Upload',
					hidden : true,
					filebrowser : 'uploadButton',
					label : editor.lang.common.upload,
					elements :
					[
						{
							type : 'file',
							id : 'upload',
							label : editor.lang.common.upload,
							size : 38
						},
						{
							type : 'fileButton',
							id : 'uploadButton',
							label : editor.lang.common.uploadSubmit,
							filebrowser : 'info:src',
							'for' : [ 'Upload', 'upload' ]
						}
					]
				},
				{
					id : 'properties',
					label : editor.lang.flash.propertiesTab,
					elements :
					[
						{
							type : 'hbox',
							widths : [ '50%', '50%' ],
							children :
							[
								{
									id : 'scale',
									type : 'select',
									label : editor.lang.flash.scale,
									'default' : '',
									style : 'width : 100%;',
									items :
									[
										[ editor.lang.common.notSet , ''],
										[ editor.lang.flash.scaleAll, 'showall' ],
										[ editor.lang.flash.scaleNoBorder, 'noborder' ],
										[ editor.lang.flash.scaleFit, 'exactfit' ]
									],
									setup : loadValue,
									commit : commitValue
								},
								{
									id : 'allowScriptAccess',
									type : 'select',
									label : editor.lang.flash.access,
									'default' : '',
									style : 'width : 100%;',
									items :
									[
										[ editor.lang.common.notSet , ''],
										[ editor.lang.flash.accessAlways, 'always' ],
										[ editor.lang.flash.accessSameDomain, 'samedomain' ],
										[ editor.lang.flash.accessNever, 'never' ]
									],
									setup : loadValue,
									commit : commitValue
								}
							]
						},
						{
							type : 'hbox',
							widths : [ '50%', '50%' ],
							children :
							[
								{
									id : 'wmode',
									type : 'select',
									label : editor.lang.flash.windowMode,
									'default' : '',
									style : 'width : 100%;',
									items :
									[
										[ editor.lang.common.notSet , '' ],
										[ editor.lang.flash.windowModeWindow, 'window' ],
										[ editor.lang.flash.windowModeOpaque, 'opaque' ],
										[ editor.lang.flash.windowModeTransparent, 'transparent' ]
									],
									setup : loadValue,
									commit : commitValue
								},
								{
									id : 'quality',
									type : 'select',
									label : editor.lang.flash.quality,
									'default' : 'high',
									style : 'width : 100%;',
									items :
									[
										[ editor.lang.common.notSet , '' ],
										[ editor.lang.flash.qualityBest, 'best' ],
										[ editor.lang.flash.qualityHigh, 'high' ],
										[ editor.lang.flash.qualityAutoHigh, 'autohigh' ],
										[ editor.lang.flash.qualityMedium, 'medium' ],
										[ editor.lang.flash.qualityAutoLow, 'autolow' ],
										[ editor.lang.flash.qualityLow, 'low' ]
									],
									setup : loadValue,
									commit : commitValue
								}
							]
						},
						{
							type : 'hbox',
							widths : [ '50%', '50%' ],
							children :
							[
								{
									id : 'align',
									type : 'select',
									label : editor.lang.common.align,
									'default' : '',
									style : 'width : 100%;',
									items :
									[
										[ editor.lang.common.notSet , ''],
										[ editor.lang.common.alignLeft , 'left'],
										[ editor.lang.flash.alignAbsBottom , 'absBottom'],
										[ editor.lang.flash.alignAbsMiddle , 'absMiddle'],
										[ editor.lang.flash.alignBaseline , 'baseline'],
										[ editor.lang.common.alignBottom , 'bottom'],
										[ editor.lang.common.alignMiddle , 'middle'],
										[ editor.lang.common.alignRight , 'right'],
										[ editor.lang.flash.alignTextTop , 'textTop'],
										[ editor.lang.common.alignTop , 'top']
									],
									setup : loadValue,
									commit : function( objectNode, embedNode, paramMap, extraStyles, extraAttributes )
									{
										var value = this.getValue();
										commitValue.apply( this, arguments );
										value && ( extraAttributes.align = value );
									}
								},
								{
									type : 'html',
									html : '<div></div>'
								}
							]
						},
						{
							type : 'fieldset',
							label : CKEDITOR.tools.htmlEncode( editor.lang.flash.flashvars ),
							children :
							[
								{
									type : 'vbox',
									padding : 0,
									children :
									[
										{
											type : 'checkbox',
											id : 'menu',
											label : editor.lang.flash.chkMenu,
											'default' : true,
											setup : loadValue,
											commit : commitValue
										},
										{
											type : 'checkbox',
											id : 'play',
											label : editor.lang.flash.chkPlay,
											'default' : true,
											setup : loadValue,
											commit : commitValue
										},
										{
											type : 'checkbox',
											id : 'loop',
											label : editor.lang.flash.chkLoop,
											'default' : true,
											setup : loadValue,
											commit : commitValue
										},
										{
											type : 'checkbox',
											id : 'allowFullScreen',
											label : editor.lang.flash.chkFull,
											'default' : true,
											setup : loadValue,
											commit : commitValue
										}
									]
								}
							]
						}
					]
				},
				{
					id : 'advanced',
					label : editor.lang.common.advancedTab,
					elements :
					[
						{
							type : 'hbox',
							widths : [ '45%', '55%' ],
							children :
							[
								{
									type : 'text',
									id : 'id',
									label : editor.lang.common.id,
									setup : loadValue,
									commit : commitValue
								},
								{
									type : 'text',
									id : 'title',
									label : editor.lang.common.advisoryTitle,
									setup : loadValue,
									commit : commitValue
								}
							]
						},
						{
							type : 'hbox',
							widths : [ '45%', '55%' ],
							children :
							[
								{
									type : 'text',
									id : 'bgcolor',
									label : editor.lang.flash.bgcolor,
									setup : loadValue,
									commit : commitValue
								},
								{
									type : 'text',
									id : 'class',
									label : editor.lang.common.cssClass,
									setup : loadValue,
									commit : commitValue
								}
							]
						},
						{
							type : 'text',
							id : 'style',
							validate : CKEDITOR.dialog.validate.inlineStyle( editor.lang.common.invalidInlineStyle ),
							label : editor.lang.common.cssStyle,
							setup : loadValue,
							commit : commitValue
						}
					]
				}
			]
		};
	} );
})();
if(!window.XRegExp){(function(){var D={exec:RegExp.prototype.exec,match:String.prototype.match,replace:String.prototype.replace,split:String.prototype.split},C={part:/(?:[^\\([#\s.]+|\\(?!k<[\w$]+>)[\S\s]?|\((?=\?(?!#|<[\w$]+>)))+|(\()(?:\?(?:(#)[^)]*\)|<([$\w]+)>))?|\\k<([\w$]+)>|(\[\^?)|([\S\s])/g,replaceVar:/(?:[^$]+|\$(?![1-9$&`']|{[$\w]+}))+|\$(?:([1-9]\d*|[$&`'])|{([$\w]+)})/g,extended:/^(?:\s+|#.*)+/,quantifier:/^(?:[?*+]|{\d+(?:,\d*)?})/,classLeft:/&&\[\^?/g,classRight:/]/g},A=function(H,F,G){for(var E=G||0;E<H.length;E++){if(H[E]===F){return E}}return -1},B=/()??/.exec("")[1]!==undefined;XRegExp=function(N,H){if(N instanceof RegExp){if(H!==undefined){throw TypeError("can't supply flags when constructing one RegExp from another")}return N.addFlags()}var H=H||"",E=H.indexOf("s")>-1,J=H.indexOf("x")>-1,O=false,Q=[],G=[],F=C.part,K,I,M,L,P;F.lastIndex=0;while(K=D.exec.call(F,N)){if(K[2]){if(!C.quantifier.test(N.slice(F.lastIndex))){G.push("(?:)")}}else{if(K[1]){Q.push(K[3]||null);if(K[3]){O=true}G.push("(")}else{if(K[4]){L=A(Q,K[4]);G.push(L>-1?"\\"+(L+1)+(isNaN(N.charAt(F.lastIndex))?"":"(?:)"):K[0])}else{if(K[5]){if(N.charAt(F.lastIndex)==="]"){G.push(K[5]==="["?"(?!)":"[\\S\\s]");F.lastIndex++}else{I=XRegExp.matchRecursive("&&"+N.slice(K.index),C.classLeft,C.classRight,"",{escapeChar:"\\"})[0];G.push(K[5]+I+"]");F.lastIndex+=I.length+1}}else{if(K[6]){if(E&&K[6]==="."){G.push("[\\S\\s]")}else{if(J&&C.extended.test(K[6])){M=D.exec.call(C.extended,N.slice(F.lastIndex-1))[0].length;if(!C.quantifier.test(N.slice(F.lastIndex-1+M))){G.push("(?:)")}F.lastIndex+=M-1}else{G.push(K[6])}}}else{G.push(K[0])}}}}}}P=RegExp(G.join(""),D.replace.call(H,/[sx]+/g,""));P._x={source:N,captureNames:O?Q:null};return P};RegExp.prototype.exec=function(I){var G=D.exec.call(this,I),F,H,E;if(G){if(B&&G.length>1){E=new RegExp("^"+this.source+"$(?!\\s)",this.getNativeFlags());D.replace.call(G[0],E,function(){for(H=1;H<arguments.length-2;H++){if(arguments[H]===undefined){G[H]=undefined}}})}if(this._x&&this._x.captureNames){for(H=1;H<G.length;H++){F=this._x.captureNames[H-1];if(F){G[F]=G[H]}}}if(this.global&&this.lastIndex>(G.index+G[0].length)){this.lastIndex--}}return G};String.prototype.match=function(E){if(!(E instanceof RegExp)){E=new XRegExp(E)}if(E.global){return D.match.call(this,E)}return E.exec(this)};String.prototype.replace=function(F,G){var E=(F._x||{}).captureNames;if(!(F instanceof RegExp&&E)){return D.replace.apply(this,arguments)}if(typeof G==="function"){return D.replace.call(this,F,function(){arguments[0]=new String(arguments[0]);for(var H=0;H<E.length;H++){if(E[H]){arguments[0][E[H]]=arguments[H+1]}}return G.apply(window,arguments)})}else{return D.replace.call(this,F,function(){var H=arguments;return D.replace.call(G,C.replaceVar,function(J,I,M){if(I){switch(I){case"$":return"$";case"&":return H[0];case"`":return H[H.length-1].slice(0,H[H.length-2]);case"'":return H[H.length-1].slice(H[H.length-2]+H[0].length);default:var K="";I=+I;while(I>E.length){K=D.split.call(I,"").pop()+K;I=Math.floor(I/10)}return(I?H[I]:"$")+K}}else{if(M){var L=A(E,M);return L>-1?H[L+1]:J}else{return J}}})})}};String.prototype.split=function(J,F){if(!(J instanceof RegExp)){return D.split.apply(this,arguments)}var G=[],E=J.lastIndex,K=0,I=0,H;if(F===undefined||+F<0){F=false}else{F=Math.floor(+F);if(!F){return[]}}if(!J.global){J=J.addFlags("g")}else{J.lastIndex=0}while((!F||I++<=F)&&(H=J.exec(this))){if(J.lastIndex>K){G=G.concat(this.slice(K,H.index),(H.index===this.length?[]:H.slice(1)));K=J.lastIndex}if(!H[0].length){J.lastIndex++}}G=K===this.length?(J.test("")?G:G.concat("")):(F?G:G.concat(this.slice(K)));J.lastIndex=E;return G}})()}RegExp.prototype.getNativeFlags=function(){return(this.global?"g":"")+(this.ignoreCase?"i":"")+(this.multiline?"m":"")+(this.extended?"x":"")+(this.sticky?"y":"")};RegExp.prototype.addFlags=function(A){var B=new XRegExp(this.source,(A||"")+this.getNativeFlags());if(this._x){B._x={source:this._x.source,captureNames:this._x.captureNames?this._x.captureNames.slice(0):null}}return B};RegExp.prototype.call=function(A,B){return this.exec(B)};RegExp.prototype.apply=function(B,A){return this.exec(A[0])};XRegExp.cache=function(C,A){var B="/"+C+"/"+(A||"");return XRegExp.cache[B]||(XRegExp.cache[B]=new XRegExp(C,A))};XRegExp.escape=function(A){return A.replace(/[-[\]{}()*+?.\\^$|,#\s]/g,"\\$&")};XRegExp.matchRecursive=function(P,D,S,F,B){var B=B||{},V=B.escapeChar,K=B.valueNames,F=F||"",Q=F.indexOf("g")>-1,C=F.indexOf("i")>-1,H=F.indexOf("m")>-1,U=F.indexOf("y")>-1,F=F.replace(/y/g,""),D=D instanceof RegExp?(D.global?D:D.addFlags("g")):new XRegExp(D,"g"+F),S=S instanceof RegExp?(S.global?S:S.addFlags("g")):new XRegExp(S,"g"+F),I=[],A=0,J=0,N=0,L=0,M,E,O,R,G,T;if(V){if(V.length>1){throw SyntaxError("can't supply more than one escape character")}if(H){throw TypeError("can't supply escape character when using the multiline flag")}G=XRegExp.escape(V);T=new RegExp("^(?:"+G+"[\\S\\s]|(?:(?!"+D.source+"|"+S.source+")[^"+G+"])+)+",C?"i":"")}while(true){D.lastIndex=S.lastIndex=N+(V?(T.exec(P.slice(N))||[""])[0].length:0);O=D.exec(P);R=S.exec(P);if(O&&R){if(O.index<=R.index){R=null}else{O=null}}if(O||R){J=(O||R).index;N=(O?D:S).lastIndex}else{if(!A){break}}if(U&&!A&&J>L){break}if(O){if(!A++){M=J;E=N}}else{if(R&&A){if(!--A){if(K){if(K[0]&&M>L){I.push([K[0],P.slice(L,M),L,M])}if(K[1]){I.push([K[1],P.slice(M,E),M,E])}if(K[2]){I.push([K[2],P.slice(E,J),E,J])}if(K[3]){I.push([K[3],P.slice(J,N),J,N])}L=N}else{I.push(P.slice(E,J))}if(!Q){break}}}else{D.lastIndex=S.lastIndex=0;throw Error("subject data contains unbalanced delimiters")}}if(J===N){N++}}if(Q&&!U&&K&&K[0]&&P.length>L){I.push([K[0],P.slice(L),L,P.length])}D.lastIndex=S.lastIndex=0;return I};
