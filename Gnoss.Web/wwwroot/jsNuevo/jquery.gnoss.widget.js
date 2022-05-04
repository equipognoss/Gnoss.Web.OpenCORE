/*!
 * jQuery JavaScript Library v1.6.1
 * http://jquery.com/
 *
 * Copyright 2011, John Resig
 * Dual licensed under the MIT or GPL Version 2 licenses.
 * http://jquery.org/license
 *
 * Includes Sizzle.js
 * http://sizzlejs.com/
 * Copyright 2011, The Dojo Foundation
 * Released under the MIT, BSD, and GPL Licenses.
 *
 * Date: Thu May 12 15:04:36 2011 -0400
 */

var widgetGNOSS = {
	cssWidget: '.widget',
	cssBodyWidget: '.widget-usuario-body-gnoss',
	init: function(){
		this.config();
		return;
	},
	config: function(){
		this.widget = $('#section ' + this.cssWidget);
		return;
	},
}
$(function(){
	widgetGNOSS.init();
});